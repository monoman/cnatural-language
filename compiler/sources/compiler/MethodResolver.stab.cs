/*
   Licensed to the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for additional information regarding copyright ownership.
   The ASF licenses this file to You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
using java.lang;
using java.util;
using stab.query;
using stab.reflection;
using cnatural.syntaxtree;

namespace cnatural.compiler {

    class MethodResolver {
        private ExpressionValidator expressionValidator;
        private CompilerContext context;
        
        MethodResolver(ExpressionValidator expressionValidator, CompilerContext context) {
            this.expressionValidator = expressionValidator;
            this.context = context;
        }

        MethodInfo resolveMethod(Iterable<MethodInfo> methods, Iterable<ExpressionNode> arguments, TypeInfo returnType) {
            return resolveMethod(methods, arguments, returnType, false);
        }
        
        MethodInfo resolveMethod(Iterable<MethodInfo> methods, Iterable<ExpressionNode> arguments,
                TypeInfo returnType, bool extensionMethods) {
            var candidates = new ArrayList<CandidateMethodInfo>();
            var hasLambda = false;
            
            foreach (var method in methods) {
                var parameterCount = method.Parameters.count();
                var argumentCount = arguments.count();
                if (method.IsVarargs) {
                    if (argumentCount < parameterCount - 1) {
                        continue;
                    }
                } else if (argumentCount != parameterCount) {
                    continue;
                }
                var fixedArgumentCount = (method.IsVarargs) ? parameterCount - 1 : parameterCount;
                var isCompatible = true;
                var parameterTypes = new TypeInfo[argumentCount];
                var expandedForm = method.IsVarargs;

                if (!method.IsClosed) {
                    var typeVariableInfos = new HashMap<TypeInfo, TypeVariableInfo>();
                    foreach (var t in method.GenericArguments) {
                        typeVariableInfos[t] = new TypeVariableInfo(t);
                    }
                    
                    // Inference phase 1
                    var nit = arguments.iterator();
                    var pit = method.Parameters.iterator();
                    bool closedParams = true;
                    for (int i = 0; i < fixedArgumentCount; i++) {
                        var paramType = pit.next().Type;
                        var argNode = nit.next();
                        if (paramType.IsClosed) {
                            if (!isArgumentCompatible(argNode, paramType)) {
                                goto continueLookup;
                            }
                        } else {
                            closedParams = false;
                            if (argNode.ExpressionKind == ExpressionKind.Lambda) {
                                hasLambda = true;
                                makeExplicitParameterTypeInference(argNode, paramType, typeVariableInfos);
                            } else {
                                var argInfo = argNode.getUserData(typeof(ExpressionInfo));
                                if (argInfo == null) {
                                    continue;
                                }
                                if (BytecodeHelper.isDelegateType(paramType) || BytecodeHelper.isExpressionTreeType(paramType)) {
                                    makeExplicitParameterTypeInference(argNode, paramType, typeVariableInfos);
                                } else {
                                    ValidationHelper.getType(context, argNode);
                                    makeOutputTypeInference(argNode, paramType, typeVariableInfos);
                                }
                            }
                        }
                    }
                    if (method.IsVarargs) {
                        var paramType = pit.next().Type.ElementType;
                        var isClosedParam = paramType.IsClosed;
                        var first = true;
                        while (nit.hasNext()) {
                            var argNode = nit.next();
                            if (isClosedParam) {
                                switch (isVarargCompatible(argNode, paramType, first)) {
                                case False:
                                    goto continueLookup;
                                case True:
                                    expandedForm = false;
                                    if (nit.hasNext()) {
                                        goto continueLookup;
                                    }
                                    break;
                                }
                            } else {
                                closedParams = false;
                                if (argNode.ExpressionKind == ExpressionKind.Lambda) {
                                    hasLambda = true;
                                    makeExplicitParameterTypeInference(argNode, paramType, typeVariableInfos);
                                } else {
                                    var argInfo = argNode.getUserData(typeof(ExpressionInfo));
                                    if (argInfo == null) {
                                        continue;
                                    }
                                    if (BytecodeHelper.isDelegateType(paramType) || BytecodeHelper.isExpressionTreeType(paramType)) {
                                        makeExplicitParameterTypeInference(argNode, paramType, typeVariableInfos);
                                    } else if (paramType != ValidationHelper.getType(context, argNode)
                                            && !ValidationHelper.isAssignable(context, paramType, argNode)
                                            && first
                                            && ValidationHelper.isAssignable(context, paramType.getArrayType(), argNode)) {
                                        expandedForm = false;
                                        makeOutputTypeInference(argNode, paramType.getArrayType(), typeVariableInfos);
                                    } else {
                                        makeOutputTypeInference(argNode, paramType, typeVariableInfos);
                                    }
                                }
                            }
                            first = false;
                        }
                    }

                    if (closedParams && returnType != null && returnType != context.TypeSystem.VoidType) {
                        makeLowerBoundInference(returnType, method.ReturnType, typeVariableInfos);
                    }
                    
                    // Inference phase 2
                    for (;;) {
                        var varFixed = false;
                        var hasUnfixed = false;
                        foreach (var e in typeVariableInfos.entrySet()) {
                            if (e.Value.fixedType != null) {
                                continue;
                            }
                            if (!containsUnfixedVariables(e.getValue().dependencies, typeVariableInfos)) {
                                if (fixTypeVariable(e.getValue())) {
                                    varFixed = true;
                                    continue;
                                }
                            }
                            hasUnfixed = true;
                        }
                        if (!varFixed) {
                            varFixed = false;
                            foreach (var e in typeVariableInfos.entrySet()) {
                                if (e.Value.fixedType != null) {
                                    continue;
                                }
                                if (hasUnfixedTypeVariablesDependingOn(e.Key, typeVariableInfos, new HashSet<TypeInfo>())) {
                                    if (!e.Value.bounds.isEmpty()) {
                                        if (fixTypeVariable(e.Value)) {
                                            varFixed = true;
                                            continue;
                                        }
                                    }
                                }
                                hasUnfixed = true;
                            }
                        }
                        if (!varFixed && hasUnfixed) {
                            goto continueLookup;
                        }
                        if (!hasUnfixed) {
                            break;
                        }
                        var mit = method.Parameters.iterator();
                        TypeInfo paramType = null;
                        foreach (var e in arguments) {
                            if (mit.hasNext()) {
                                paramType = mit.next().Type;
                            }
                            if (e.ExpressionKind != ExpressionKind.Lambda) {
                                if (!BytecodeHelper.isDelegateType(paramType) && !BytecodeHelper.isExpressionTreeType(paramType)) {
                                    continue;
                                }
                            }
                            var m = getInvokeMethod(paramType);
                            if (hasUnfixedTypeVariables(m.ReturnType, typeVariableInfos)) {
                                hasUnfixed = false;
                                foreach (var p in m.Parameters) {
                                    if (hasUnfixedTypeVariables(p.Type, typeVariableInfos)) {
                                        hasUnfixed = true;
                                        break;
                                    }
                                }
                                if (!hasUnfixed) {
                                    makeOutputTypeInference(e, paramType, typeVariableInfos);
                                }
                            }
                        }
                    }

                    var typeArguments = new ArrayList<TypeInfo>();
                    foreach (var t in method.GenericArguments) {
                        typeArguments.add(typeVariableInfos[t].fixedType);
                    }
                    method = context.TypeSystem.getGenericMethod(method, typeArguments);
                }

                var it1 = arguments.iterator();
                var it2 = method.Parameters.iterator();
                int i;
                for (i = 0; i < fixedArgumentCount; i++) {
                    var argNode = it1.next();
                    var paramType = it2.next().Type;
                    parameterTypes[i] = paramType;
                    if (!isArgumentCompatible(argNode, paramType)) {
                        isCompatible = false;
                        break;
                    } else if (argNode.ExpressionKind == ExpressionKind.Lambda) {
                        hasLambda = true;
                    }
                }
                if (isCompatible && method.IsVarargs) {
                    var paramType = it2.next().Type.ElementType;
                    var first = true;
                    while (isCompatible && it1.hasNext()) {
                        var argNode = it1.next();
                        parameterTypes[i++] = paramType;
                        switch (isVarargCompatible(argNode, paramType, first)) {
                        case False:
                            isCompatible = false;
                            break;
                        case True:
                            expandedForm = false;
                            if (it1.hasNext()) {
                                isCompatible = false;
                                break;
                            } else if (argNode.ExpressionKind == ExpressionKind.Lambda) {
                                hasLambda = true;
                            }
                            break;
                        }
                        first = false;
                    }
                }
                if (isCompatible) {
                    candidates.add(new CandidateMethodInfo(method, parameterTypes, arguments, expandedForm));
                }
            continueLookup:
                ;
            }

            if (candidates.size() == 0) {
                return null;
            }

            CandidateMethodInfo result;
            if (candidates.size() > 1) {
                result = resolveOverloading(candidates, extensionMethods);
                if (result == null) {
                    return null;
                }
            } else {
                result = candidates[0];
            }
            if (hasLambda) {
                int parameterCount = result.method.Parameters.count();
                int argumentCount = arguments.count();
                int fixedArgumentCount = (result.method.IsVarargs) ? parameterCount - 1 : parameterCount;

                var parameterTypes = new TypeInfo[argumentCount];
                var pit = result.method.getParameters().iterator();
                int i;
                for (i = 0; i < fixedArgumentCount; i++) {
                    parameterTypes[i] = pit.next().Type;
                }
                if (result.method.IsVarargs) {
                    if (result.expandedForm) {
                        var paramType = pit.next().Type.ElementType;
                        while (i < argumentCount) {
                            parameterTypes[i++] = paramType;
                        }
                    } else {
                        parameterTypes[i] = pit.next().Type;
                    }
                }
                i = 0;
                foreach (var argNode in arguments) {
					expressionValidator.handleExpression(argNode, parameterTypes[i], true);
                    i++;
                }
            }
            return result.method;
        }
        
        bool resolveDelegate(TypeInfo type, ExpressionNode arg, ExpressionInfo targetInfo, SyntaxNode node) {
            var invokeMethod = getInvokeMethod(type);
            int nparams = invokeMethod.Parameters.count();
            var ainfo = arg.getUserData(typeof(ExpressionInfo));
            if (ainfo.Members == null) {
                return true;
            }
            MemberInfo foundMember = null;
            foreach (var member in ainfo.Members) {
                switch (member.MemberKind) {
                case Method:
                    var meth = member.Method;
                    if (meth.Parameters.count() != nparams || meth.IsVarargs != invokeMethod.IsVarargs) {
                        continue;
                    }
                    if (nparams == 0) {
                        if (foundMember != null) {
                            if (node != null) {
                                throw context.error(CompileErrorId.AmbiguousMembers, node,
                                    BytecodeHelper.getDisplayName(member.DeclaringType) + "." + BytecodeHelper.getDisplayName(member.Method),
                                    BytecodeHelper.getDisplayName(foundMember.DeclaringType) + "."
                                    + BytecodeHelper.getDisplayName(foundMember.Method));
                            } else {
                                return false;
                            }
                        }
                        foundMember = member;
                        ainfo.Member = member;
                        if (meth.IsExcludedFromCompilation || CompilerHelper.shouldIgnoreCalls(context, meth)) {
                            if (node != null) {
                                throw context.error(CompileErrorId.NotGeneratedMethodUsage, node,
                                    BytecodeHelper.getDisplayName(member.DeclaringType) + "." + BytecodeHelper.getDisplayName(member.Method));
                            } else {
                                return false;
                            }
                        }
                        if (targetInfo != null) {
                            targetInfo.Type = type;
                            targetInfo.Method = meth;
                        }
                        continue;
                    }
                    var it1 = meth.Parameters.iterator();
                    var it2 = invokeMethod.Parameters.iterator();
                    var sameParams = true;
                    while (it1.hasNext()) {
                        if (it1.next().Type != it2.next().Type) {
                            sameParams = false;
                            break;
                        }
                    }
                    if (sameParams) {
                        if (foundMember != null) {
                            if (node != null) {
                                throw context.error(CompileErrorId.AmbiguousMembers, node,
                                    BytecodeHelper.getDisplayName(member.DeclaringType) + "." + BytecodeHelper.getDisplayName(member.Method),
                                    BytecodeHelper.getDisplayName(foundMember.DeclaringType) + "." + foundMember.Name);
                            } else {
                                return false;
                            }
                        }
                        foundMember = member;
                        ainfo.Member = member;
                        if (targetInfo != null) {
                            targetInfo.Type = type;
                            targetInfo.Method = meth;
                        }
                        continue;
                    }
                    break;
                case Local:
                    ainfo.Member = member;
                    return true;
                case Field:
                case Property:
                    foundMember = member;
                    ainfo.Member = member;
                    break;
                }
            }
            if (foundMember != null) {
                return true;
            }
            if (node != null) {
                throw context.error(CompileErrorId.NoEligibleOverload, node, ainfo.Members.first().Name);
            } else {
                return false;
            }
        }
        
        private bool isArgumentCompatible(ExpressionNode argNode, TypeInfo paramType) {
            var ainfo = argNode.getUserData(typeof(ExpressionInfo));
            var cleanInfo = false;
            if (ainfo == null && argNode.ExpressionKind == ExpressionKind.Lambda) {
                if (!expressionValidator.handleExpressionNoError(argNode, paramType, true)) {
                    argNode.removeUserData(typeof(ExpressionInfo));
                    return false;
                }
                ainfo = argNode.getUserData(typeof(ExpressionInfo));
                cleanInfo = true;
            }
            if (ainfo == null) {
                return !paramType.IsPrimitive;
            }
            if (BytecodeHelper.isDelegateType(paramType)) {
                if (!resolveDelegate(paramType, argNode, null, null)) {
                    if (cleanInfo) {
                        argNode.removeUserData(typeof(ExpressionInfo));
                    }
                    return false;
                }
            } else if (ValidationHelper.isMethod(argNode)) {
                return false;
            } else if (paramType != ValidationHelper.getType(context, argNode)) {
                if (!ValidationHelper.isAssignable(context, paramType, argNode)) {
                    if (cleanInfo) {
                        argNode.removeUserData(typeof(ExpressionInfo));
                    }
                    return false;
                }
            }
            if (cleanInfo) {
                argNode.removeUserData(typeof(ExpressionInfo));
            }
            return true;
        }

        private VarargCompatible isVarargCompatible(ExpressionNode argNode, TypeInfo paramType, bool first) {
            var ainfo = argNode.getUserData(typeof(ExpressionInfo));
            if (ainfo == null) {
                if (!expressionValidator.handleExpressionNoError(argNode, paramType, true)) {
                    return VarargCompatible.False;
                }
                ainfo = argNode.getUserData(typeof(ExpressionInfo));
            }
            if (ainfo == null) {
                if (paramType.IsPrimitive) {
                    return VarargCompatible.False;
                }
            } else if (BytecodeHelper.isDelegateType(paramType)) {
                if (!resolveDelegate(paramType, argNode, null, null)) {
                    return VarargCompatible.False;
                }
            } else if (ValidationHelper.isMethod(argNode)) {
                return VarargCompatible.False;
            } else if (paramType != ValidationHelper.getType(context, argNode)) {
                if (!ValidationHelper.isAssignable(context, paramType, argNode)) {
                    if (first) {
                        if (ValidationHelper.isAssignable(context, paramType.ArrayType, argNode)) {
                            return VarargCompatible.True;
                        }
                    }
                    return VarargCompatible.False;
                }
            }
            return VarargCompatible.TrueExpanded;
        }

        //
        // Type inference
        //
        
        private bool hasUnfixedTypeVariables(TypeInfo type, HashMap<TypeInfo, TypeVariableInfo> typeVariableInfos) {
            var tv = typeVariableInfos.get(type);
            if (tv != null) {
                return tv.fixedType == null;
            } else if (type.IsArray) {
                return hasUnfixedTypeVariables(type.ElementType, typeVariableInfos);
            } else if (!type.IsClosed) {
                foreach (var t in type.GenericArguments) {
                    if (hasUnfixedTypeVariables(t, typeVariableInfos)) {
                        return true;
                    }
                }
            }
            return false;
        }
    
        private bool containsUnfixedVariables(Iterable<TypeInfo> types, HashMap<TypeInfo, TypeVariableInfo> typeVariableInfos) {
            foreach (var type in types) {
                var tv = typeVariableInfos[type];
                if (tv.fixedType == null) {
                    return true;
                }
            }
            return false;
        }

        private bool hasUnfixedTypeVariablesDependingOn(TypeInfo type, HashMap<TypeInfo, TypeVariableInfo> typeVariableInfos,
                HashSet<TypeInfo> visited) {
            if (visited.contains(type)) {
                return false;
            }
            visited.add(type);
            foreach (var tv in typeVariableInfos.values()) {
                if (tv.fixedType == null) {
                    if (tv.dependencies.contains(type)) {
                        return true;
                    }
                } else if (tv.dependencies.contains(type)) {
                    foreach (var t in tv.dependencies) {
                        if (hasUnfixedTypeVariablesDependingOn(t, typeVariableInfos, visited)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void makeExplicitParameterTypeInference(ExpressionNode expression, TypeInfo toType,
                HashMap<TypeInfo, TypeVariableInfo> typeVariableInfos) {
            var method = getInvokeMethod(toType);
            if (method == null) {
                return;
            }
            if (expression.ExpressionKind == ExpressionKind.Lambda) {
                var lambda = (LambdaExpressionNode)expression;
                if (lambda.Parameters.size() != method.Parameters.count()) {
                	return;
                }
            }
            var rinfo = typeVariableInfos[method.ReturnType];
            if (rinfo != null) {
                foreach (var p in method.Parameters) {
                    if (p.Type == method.ReturnType) {
                        continue;
                    }
                    var pinfo = typeVariableInfos[p.Type];
                    if (pinfo != null) {
                        rinfo.dependencies.add(pinfo.genericParameterType);
                    }
                }
            }

            if (expression.ExpressionKind == ExpressionKind.Lambda) {
                var lambda = (LambdaExpressionNode)expression;
                var fromTypes = new ArrayList<TypeInfo>();
                var pit = method.Parameters.iterator();
                foreach (var p in lambda.Parameters) {
                    var ptype = pit.next().Type;
                    if (p.Type == null) {
                        if (ptype.IsClosed) {
                            fromTypes.add(ptype);
                        } else {
                            return;
                        }
                    } else {
                        fromTypes.add(CompilerHelper.resolveTypeReference(context, context.CurrentType.PackageName, p.Type));
                    }
                }
                context.CodeValidationContext.pushLambdaParameters(fromTypes);
                bool success = expressionValidator.handleExpressionNoError(expression, null, true);
                context.CodeValidationContext.popLambdaParameters();
                var info = expression.getUserData(typeof(ExpressionInfo));
                if (info == null) {
                    return;
                }
                expression.removeUserData(typeof(ExpressionInfo));
                if (!success) {
                    return;
                }
                var mit = method.Parameters.iterator();
                var fit = fromTypes.iterator();
                while (mit.hasNext()) {
                    makeExactInference(fit.next(), mit.next().Type, typeVariableInfos);
                }
                makeExactInference(context.CodeValidationContext.LambdaReturnType, method.ReturnType, typeVariableInfos);
            } else {
                var info = expression.getUserData(typeof(ExpressionInfo));
                if (info == null) {
                    return;
                }
                var paramTypes = new ArrayList<TypeInfo>();
                foreach (var p in method.Parameters) {
                    paramTypes.add(p.Type);
                }
                var meth = resolveMethodGroup(info, method, paramTypes);
                if (meth == null) {
                    return;
                }
                var mit = method.Parameters.iterator();
                var cit = meth.Parameters.iterator();
                while (mit.hasNext()) {
                    makeExactInference(cit.next().Type, mit.next().Type, typeVariableInfos);
                }
                makeExactInference(meth.ReturnType, method.ReturnType, typeVariableInfos);
            }
        }

        private Iterable<TypeInfo> bindGenericParameters(HashMap<TypeInfo, TypeVariableInfo> typeVariables,
                Iterable<TypeInfo> types) {
            return types.select(p => bindGenericParameters(typeVariables, p)).toList();
        }
        
        private TypeInfo bindGenericParameters(HashMap<TypeInfo, TypeVariableInfo> typeVariables, TypeInfo typeInfo) {
            if (typeVariables.containsKey(typeInfo)) {
                return typeVariables[typeInfo].fixedType;
            } else if (typeInfo.IsArray) {
                var t = bindGenericParameters(typeVariables, typeInfo.ElementType);
                return (t == null) ? null : t.ArrayType;
            } else if (!typeInfo.IsClosed && typeInfo.GenericArguments.any()) {
                var tl = bindGenericParameters(typeVariables, typeInfo.GenericArguments);
                if (tl.any(p => p == null)) {
                    return null;
                }
                return context.TypeSystem.getGenericType(typeInfo, tl);
            }
            return typeInfo;
        }

        
        private void makeOutputTypeInference(ExpressionNode expression, TypeInfo toType, HashMap<TypeInfo, TypeVariableInfo> typeVariables) {
            if (expression.ExpressionKind == ExpressionKind.Lambda
                    || BytecodeHelper.isDelegateType(toType)
                    || BytecodeHelper.isExpressionTreeType(toType)) {
                MethodInfo method = getInvokeMethod(toType);
                if (method == null) {
                    return;
                }
                if (expression.ExpressionKind == ExpressionKind.Lambda) {
	                var lambda = (LambdaExpressionNode)expression;
	                if (lambda.Parameters.size() != method.Parameters.count()) {
	                	return;
	                }
	            }
                var paramTypes = new ArrayList<TypeInfo>();
                foreach (var p in method.Parameters) {
                    TypeVariableInfo pinfo = typeVariables[p.Type];
                    if (pinfo != null) {
                        if (pinfo.fixedType != null) {
                            paramTypes.add(pinfo.fixedType);
                        } else {
                            return;
                        }
                    } else {
                        var t = bindGenericParameters(typeVariables, p.Type);
                        if (t == null) {
                            return;
                        } else {
                            paramTypes.add(t);
                        }
                    }
                }
                if (expression.ExpressionKind == ExpressionKind.Lambda) {
                    context.CodeValidationContext.pushLambdaParameters(paramTypes);
                    var success = expressionValidator.handleExpressionNoError(expression, null, true);
                    context.CodeValidationContext.popLambdaParameters();
                    var info = expression.getUserData(typeof(ExpressionInfo));
                    if (info == null) {
                        return;
                    }
                    expression.removeUserData(typeof(ExpressionInfo));
                    if (success) {
                        makeLowerBoundInference(context.CodeValidationContext.LambdaReturnType, method.ReturnType, typeVariables);
                    }
                } else {
                    var info = expression.getUserData(typeof(ExpressionInfo));
                    if (info == null) {
                        return;
                    }
                    var meth = resolveMethodGroup(info, method, paramTypes);
                    if (meth == null) {
                        makeLowerBoundInference(ValidationHelper.getType(context, expression), toType, typeVariables);
                    } else {
                        makeLowerBoundInference(meth.ReturnType, method.ReturnType, typeVariables);
                    }
                }
            } else {
                var info = expression.getUserData(typeof(ExpressionInfo));
                if (info == null) {
                    return;
                }
                makeLowerBoundInference(info.Type, toType, typeVariables);
            }
        }
        
        private static void makeLowerBoundInference(TypeInfo fromType, TypeInfo toType,
                HashMap<TypeInfo, TypeVariableInfo> typeVariables) {
            if (isExcludedFromInference(fromType)) {
                return;
            }
            
            var info = typeVariables[toType];
            if (info != null) {
                info.bounds.add(fromType);
            } else if (fromType.IsArray && toType.IsArray) {
                makeLowerBoundInference(fromType.ElementType, toType.ElementType, typeVariables);
            } else {
                var t = BytecodeHelper.getImplicitConversion(fromType, toType);
                if (t != null) {
                    var fit = t.GenericArguments.iterator();
                    var tit = toType.GenericArguments.iterator();
                    while (fit.hasNext() && tit.hasNext()) {
                        makeExactInference(fit.next(), tit.next(), typeVariables);
                    }
                }
            }
        }

        private static void makeExactInference(TypeInfo fromType, TypeInfo toType, HashMap<TypeInfo, TypeVariableInfo> typeVariables) {
            if (isExcludedFromInference(fromType)) {
                return;
            }
            
            var info = typeVariables[toType];
            if (info != null) {
                info.bounds.add(fromType);
            } else if (fromType.IsArray && toType.IsArray) {
                makeExactInference(fromType.ElementType, toType.ElementType, typeVariables);
            } else if (fromType.OriginalTypeDefinition == toType.OriginalTypeDefinition) {
                var fit = fromType.GenericArguments.iterator();
                var tit = toType.GenericArguments.iterator();
                while (fit.hasNext()) {
                    makeExactInference(fit.next(), tit.next(), typeVariables);
                }
            }
        }

        private MethodInfo resolveMethodGroup(ExpressionInfo info, MethodInfo method, ArrayList<TypeInfo> paramTypes) {
            MethodInfo result = null;
            foreach (var mi in info.Members) {
                if (mi.MemberKind != MemberKind.Method) {
                    return null;
                }
                var meth = mi.Method;
                if (method.IsVarargs != meth.IsVarargs) {
                    continue;
                }
                int nparams = meth.Parameters.count();
                if (nparams != paramTypes.size()) {
                    continue;
                }
                var match = true;
                int i = 0;
                foreach (var p in meth.Parameters) {
                    if (p.Type != paramTypes[i]) {
                        match = false;
                        break;
                    }
                    i++;
                }
                if (match) {
                    if (result != null) {
                        return null;
                    }
                    result = meth;
                }
            }
            return result;
        }

        private bool fixTypeVariable(TypeVariableInfo info) {
            info.fixedType = getFixedType(context.TypeSystem, info.bounds);
            return info.fixedType != null;
        }

        static TypeInfo getFixedType(Library typeSystem, Collection<TypeInfo> bounds) {
            var filteredBounds = new HashSet<TypeInfo>(bounds);
            foreach (var ti in bounds) {
                foreach (var tj in bounds) {
                    if (!BytecodeHelper.hasImplicitConversion(ti, tj)) {
                        filteredBounds.remove(tj);
                    }
                }
            }
            if (filteredBounds.size() != 1) {
                return null;
            } else {
                var result = filteredBounds.single();
                if (result == null) {
                    return typeSystem.ObjectType;
                } else {
                    return result;
                }
            }
        }

        private static bool isExcludedFromInference(TypeInfo type) {
            switch (type.TypeKind) {
            case Boolean:
            case Byte:
            case Char:
            case Double:
            case Float:
            case Int:
            case Long:
            case Short:
            case Void:
            case UnboundedWildcard:
            case UpperBoundedWildcard:
            case LowerBoundedWildcard:
                return true;
            default:
                return false;
            }
            
        }

        //
        // Overload resolution
        //

        private CandidateMethodInfo resolveOverloading(List<CandidateMethodInfo> methodInfos, bool extensionMethods) {
            if (!extensionMethods) {
                //
                // Filter the methods from base types and outer types
                //

                TypeInfo mostNestedType = null;
                foreach (var i in methodInfos) {
                    var type = i.method.DeclaringType;
                    if (mostNestedType == null) {
                        mostNestedType = type;
                    } else {
                        var t = type;
                        while ((t = t.DeclaringType) != null) {
                            if (mostNestedType.isAssignableFrom(t)) {
                                mostNestedType = type;
                                break;
                            }
                        }
                    }
                }
                var mostDerivedType = mostNestedType;
                foreach (var i in methodInfos) {
                    var type = i.method.getDeclaringType();
                    if (mostDerivedType == null) {
                        mostDerivedType = type;
                    } else if (mostDerivedType.isAssignableFrom(type)) {
                        mostDerivedType = type;
                    }
                }
                var filteredMethods = new ArrayList<CandidateMethodInfo>();
                foreach (var i in methodInfos) {
                    if (i.method.DeclaringType == mostDerivedType) {
                        filteredMethods.add(i);
                    }
                }
                if (filteredMethods.size() > 0) {
                    if (filteredMethods.size() == 1) {
                        return filteredMethods[0];
                    } else {
                        methodInfos = filteredMethods;
                    }
                }
            }
            
            //
            // Look for the better function member
            //
            
            var betterFunctionMember = methodInfos[0];
            var found = false;
            for (int i = 1; i < methodInfos.size(); i++) {
                var mi = methodInfos.get(i);
                var m = getBetterMethod(betterFunctionMember, mi);
                if (m == null) {
                    var m1 = betterFunctionMember.method;
                    var m2 = methodInfos.get(i).method;
                    if (m1.GenericMethodDefinition != null) {
                        if (m2.GenericArguments.any()) {
                            m = mi;
                        }
                    } else if (m2.GenericArguments.any()) {
                        m = betterFunctionMember;
                    }
                    if (m1.IsVarargs && betterFunctionMember.expandedForm) {
                        if (!m2.IsVarargs) {
                            m = mi;
                        } else {
                            int c1 = m1.Parameters.count();
                            int c2 = m2.Parameters.count();
                            if (c1 < c2) {
                                m = betterFunctionMember;
                            } else if (c1 > c2) {
                                m = mi;
                            }
                        }
                    } else if (m2.IsVarargs && mi.expandedForm) {
                        m = betterFunctionMember;
                    }
                    if (m1.GenericMethodDefinition != null && m2.GenericMethodDefinition != null) {
                        m1 = m1.OriginalMethodDefinition;
                        m2 = m2.OriginalMethodDefinition;
                        var gmi = getMethodWithMostSpecificParameters(m1, m2);
                        if (gmi == m1) {
                            m = betterFunctionMember;
                        } else if (gmi == m2) {
                            m = mi;
                        }
                    }
                }
                if (m != null) {
                    betterFunctionMember = m;
                    found = true;
                }
            }
            if (found) {
                return betterFunctionMember;
            } else {
                return null;
            }
        }

        private MethodInfo getMethodWithMostSpecificParameters(MethodInfo m1, MethodInfo m2) {
            if (m1 == m2) {
                return m1;
            } else {
                var it1 = m1.Parameters.iterator();
                var it2 = m2.Parameters.iterator();
                var m1MostSpecific = false;
                var m2MostSpecific = false;
                while (it1.hasNext() && it2.hasNext()) {
                    var t1 = it1.next().Type;
                    var t2 = it2.next().Type;
                    var t = getMostSpecificType(t1, t2);
                    if (t == t1) {
                        m1MostSpecific = true;
                    }
                    if (t == t2) {
                        m2MostSpecific = true;
                    }
                }
                if (m1MostSpecific != m2MostSpecific) {
                    if (m1MostSpecific) {
                        return m1;
                    }
                    if (m2MostSpecific) {
                        return m2;
                    }
                }
                return null;
            }
        }

        private TypeInfo getMostSpecificType(TypeInfo t1, TypeInfo t2) {
            if (t1.IsGenericParameter && !t2.IsGenericParameter) {
                return t2;
            }
            if (t2.IsGenericParameter && !t1.IsGenericParameter) {
                return t1;
            }
            if (t1.IsArray && t2.IsArray) {
                var t1e = t1.ElementType;
                var t2e = t2.ElementType;
                var t = getMostSpecificType(t1e, t2e);
                if (t == t1e) {
                    return t1;
                }
                if (t == t2e) {
                    return t2;
                }
                return null;
            }
            int c1 = t1.GenericArguments.count();
            int c2 = t2.GenericArguments.count();
            if (c1 == c2) {
                var it1 = t1.GenericArguments.iterator();
                var it2 = t2.GenericArguments.iterator();
                var t1MostSpecific = false;
                var t2MostSpecific = false;
                while (it1.hasNext()) {
                    var pt1 = it1.next();
                    var pt2 = it2.next();
                    var t = getMostSpecificType(pt1, pt2);
                    if (t == pt1) {
                        t1MostSpecific = true;
                    }
                    if (t == pt2) {
                        t2MostSpecific = true;
                    }
                }
                if (t1MostSpecific != t2MostSpecific) {
                    if (t1MostSpecific) {
                        return t1;
                    }
                    if (t2MostSpecific) {
                        return t2;
                    }
                }
            }
            return null;
        }

        private CandidateMethodInfo getBetterMethod(CandidateMethodInfo m1, CandidateMethodInfo m2) {
            var result = BetterConversionResult.Neither;
            int i = 0;
            foreach (var e in m1.arguments) {
                switch (getBetterConversionFromExpression(m1.parameterTypes[i], m2.parameterTypes[i], e)) {
                case LeftIsBetter:
                    if (result == BetterConversionResult.RightIsBetter) {
                        return null;
                    }
                    result = BetterConversionResult.LeftIsBetter;
                    break;
                case RightIsBetter:
                    if (result == BetterConversionResult.LeftIsBetter) {
                        return null;
                    }
                    result = BetterConversionResult.RightIsBetter;
                    break;
                case Neither:
                    break;
                }
                i++;
            }
            switch (result) {
            case LeftIsBetter:
                return m1;
            case RightIsBetter:
                return m2;
            default:
                if (m1.method.IsSynthetic) {
                    if (!m2.method.IsSynthetic) {
                        return m2;
                    }
                } else if (m2.method.IsSynthetic) {
                    return m1;
                }
                return null;
            }
        }

        private BetterConversionResult getBetterConversionFromExpression(TypeInfo leftType, TypeInfo rightType, ExpressionNode expression) {
            if (leftType == rightType) {
                return BetterConversionResult.Neither;
            }
            if (expression.ExpressionKind == ExpressionKind.Lambda) {
                var leftMethod = getInvokeMethod(leftType);
                var rightMethod = getInvokeMethod(rightType);
                var leftReturnType = leftMethod.ReturnType;
                var rightReturnType = rightMethod.ReturnType;

                var lit = leftMethod.Parameters.iterator();
                var rit = rightMethod.Parameters.iterator();
                List<TypeInfo> paramTypes = null;
                while (lit.hasNext()) {
                    var lt = lit.next().Type;
                    var rt = rit.next().Type;
                    if (lt != rt) {
                        return BetterConversionResult.Neither;
                    }
                    if (paramTypes == null) {
                        paramTypes = new ArrayList<TypeInfo>();
                    }
                    paramTypes.add(lt);
                }
                if (paramTypes != null) {
                    context.CodeValidationContext.pushLambdaParameters(paramTypes);
                } else {
                    context.CodeValidationContext.pushLambdaParameters(Collections.emptyList<TypeInfo>());
                }
                expressionValidator.handleExpressionNoError(expression, null, true);
                context.CodeValidationContext.popLambdaParameters();
                
                var info = expression.getUserData(typeof(ExpressionInfo));
                expression.removeUserData(typeof(ExpressionInfo));
                if (info != null) {
                    if (leftReturnType == context.TypeSystem.VoidType) {
                        return BetterConversionResult.RightIsBetter;
                    }
                    if (rightReturnType == context.TypeSystem.VoidType) {
                        return BetterConversionResult.LeftIsBetter;
                    }
                    return getBetterConversionFromType(leftReturnType, rightReturnType, context.CodeValidationContext.LambdaReturnType);
                }
                return BetterConversionResult.Neither;
            } else {
                var info = expression.getUserData(typeof(ExpressionInfo));
                if (info == null) {
                    return BetterConversionResult.Neither;
                }
                return getBetterConversionFromType(leftType, rightType, ValidationHelper.getType(context, expression));
            }
        }

        private BetterConversionResult getBetterConversionFromType(TypeInfo leftType, TypeInfo rightType, TypeInfo fromType) {
            if (leftType == rightType) {
                return BetterConversionResult.Neither;
            }
            if (fromType == leftType) {
                return BetterConversionResult.LeftIsBetter;
            }
            if (fromType == rightType) {
                return BetterConversionResult.RightIsBetter;
            }
            if (!leftType.isAssignableFrom(rightType) && rightType.isAssignableFrom(leftType)) {
                return BetterConversionResult.LeftIsBetter;
            }
            if (!rightType.isAssignableFrom(leftType) && leftType.isAssignableFrom(rightType)) {
                return BetterConversionResult.RightIsBetter;
            }
            return BetterConversionResult.Neither;
        }

        private static MethodInfo getInvokeMethod(TypeInfo type) {
            if (BytecodeHelper.isDelegateType(type)) {
                return type.Methods.where(p => p.Name.equals("invoke")).firstOrDefault();
            } if (BytecodeHelper.isExpressionTreeType(type)) {
                return getInvokeMethod(type.GenericArguments.first());
            } else {
                return type.Methods.firstOrDefault();
            }
        }
        
        private enum VarargCompatible {
            False,
            True,
            TrueExpanded
        }

        private enum BetterConversionResult {
            Neither,
            LeftIsBetter,
            RightIsBetter
        }
        
        private class TypeVariableInfo {
            TypeInfo genericParameterType;
            HashSet<TypeInfo> bounds;
            HashSet<TypeInfo> dependencies;
            TypeInfo fixedType;
            
            TypeVariableInfo(TypeInfo genericParameterType) {
                this.genericParameterType = genericParameterType;
                this.bounds = new HashSet<TypeInfo>();
                this.dependencies = new HashSet<TypeInfo>();
            }
        }

        private class CandidateMethodInfo {
            MethodInfo method;
            TypeInfo[] parameterTypes;
            Iterable<ExpressionNode> arguments;
            bool expandedForm;

            CandidateMethodInfo(MethodInfo method, TypeInfo[] parameterTypes, Iterable<ExpressionNode> arguments, bool expandedForm) {
                this.method = method;
                this.parameterTypes = parameterTypes;
                this.arguments = arguments;
                this.expandedForm = expandedForm;
            }
        }
    }
}
