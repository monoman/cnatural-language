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
using stab.tools.helpers;
using stab.tools.syntaxtree;

namespace stab.tools.compiler {

    class ExpressionValidator : ExpressionHandler<TypeInfo, Void> {
        private CompilerContext context;
        private MethodResolver methodResolver;
    
        ExpressionValidator(CompilerContext context)
            : super(true) {
            this.context = context;
            this.MethodResolver = new MethodResolver(this, context);
        }
        
        public StatementValidator StatementValidator;
        
        public MethodResolver MethodResolver^;
        
        bool handleExpressionNoError(ExpressionNode expression, TypeInfo targetType, bool nested) {
            try {
                context.disableErrors();
                handleExpression(expression, targetType, nested);
            } catch (CodeErrorException) {
            }
            var result = !context.HasErrors;
            context.restoreErrors();
            return result;
        }

        protected override Void handleAnonymousObjectCreation(AnonymousObjectCreationExpressionNode anonymousObject, TypeInfo targetType,
                bool nested) {
            var propertyNames = new ArrayList<String>();
            var propertyTypes = new ArrayList<TypeInfo>();
            foreach (var decl in anonymousObject.MemberDeclarators) {
                handleExpression(decl.Value, null, true);
                String property;
                if (decl.NameLength > 0) {
                    property = context.getIdentifier(decl.NameOffset, decl.NameLength);
                } else {
                    SimpleNameExpressionNode name;
                    switch (decl.Value.ExpressionKind) {
                    case SimpleName:
                        name = (SimpleNameExpressionNode)decl.Value;
                        break;
                    case MemberAccess:
                        name = ((MemberAccessExpressionNode)decl.Value).Member;
                        break;
                    default:
                        throw new Exception("Internal error");
                    }
                    property = context.getIdentifier(name.NameOffset, name.NameLength);
                }
                propertyNames.add(property);
                if (ValidationHelper.isMethod(decl.Value)) {
                    throw context.error(CompileErrorId.UnexpectedMethodReference, decl);
                } else {
                    propertyTypes.add(ValidationHelper.getType(context, decl.Value));
                }
            }
            if (propertyNames.size() != propertyTypes.size()) {
                return null;
            }

            TypeInfo anonymousType = null;
            foreach (var t in context.CurrentType.NestedTypes) {
                if (!t.Name.startsWith("Anonymous") || t.Fields.count() != propertyNames.size()) {
                    continue;
                }
                int i = 0;
                var sameType = true;
                foreach (var f in t.Fields) {
                    if (!f.Name.equals(propertyNames[i] + "$0") || f.Type != propertyTypes[i]) {
                        sameType = false;
                        break;
                    }
                    i++;
                }
                if (sameType) {
                    anonymousType = t;
                    break;
                }
            }
            
            if (anonymousType == null) {
                var currentType = (TypeBuilder)context.CurrentType;
                var prefix = currentType.FullName + "$Anonymous";
                int n = currentType.NestedTypes.count(p => p.FullName.startsWith(prefix));
                var typeBuilder = currentType.defineNestedType("Anonymous" + n);
                anonymousType = typeBuilder;
                typeBuilder.setSynthetic(true);
                typeBuilder.setFinal(true);
                typeBuilder.setBaseType(context.TypeSystem.ObjectType);

                var constructor = typeBuilder.defineMethod("<init>");
                constructor.setReturnType(context.TypeSystem.VoidType);

                //
                // Define the fields, properties and constructor parameters
                //
                int i = 0;
                foreach (var decl in anonymousObject.MemberDeclarators) {
                    var property = propertyNames[i];
                    var type = propertyTypes[i];
                    
                    var fieldBuilder = typeBuilder.defineField(property + "$0", type);
                    fieldBuilder.setSynthetic(true);
                    fieldBuilder.setFinal(true);

                    var paramBuilder = constructor.addParameter(type);
                    paramBuilder.setName(property + "$0");
                    
                    var methodBuilder = typeBuilder.defineMethod("get" + property);
                    methodBuilder.addAnnotation(context.TypeSystem.getType("stab/lang/PropertyGet"), false);
                    methodBuilder.setReturnType(type);
                    var gen = methodBuilder.CodeGenerator;
                    gen.beginScope();
                    gen.emit(Opcode.Aload, gen.getLocal("this"));
                    gen.emit(Opcode.Getfield, fieldBuilder);
                    gen.emit(BytecodeHelper.getReturnOpcode(type));
                    gen.endScope();
                    i++;
                }
            }
            if (!context.IsErrorHandlingDisabled) {
                if (!context.TypeBuilders.contains((TypeBuilder)anonymousType)) {
                    context.TypeBuilders.add((TypeBuilder)anonymousType);
                }
            }
            
            anonymousObject.addOrReplaceUserData(new ExpressionInfo(anonymousType));
            return null;
        }
        
        protected override Void handleArrayCreation(ArrayCreationExpressionNode arrayCreation, TypeInfo targetType, bool nested) {
            var typeRef = arrayCreation.Type;
            TypeInfo type = null;
            if (typeRef != null) {
                type = CompilerHelper.resolveTypeReference(context, context.CurrentType.PackageName, typeRef);
                if (type.IsGenericParameter) {
                    context.addWarning(CompileErrorId.GenericArrayCreation, arrayCreation);
                }
                for (int i = arrayCreation.Dimensions - 1; i >= 0; --i) {
                    type = type.ArrayType;
                }
            }
            var initializer = arrayCreation.Initializer;
            int dimensions = arrayCreation.DimensionExpressions.size();
            if ((dimensions == 0) == (initializer == null)) {
                throw context.error(CompileErrorId.DimensionsAndInitializer, arrayCreation);
            }
            for (int i = 0; i < dimensions; i++) {
                type = type.ArrayType;
                var e = arrayCreation.DimensionExpressions[i];
                handleExpression(e, context.TypeSystem.IntType, true);
                ValidationHelper.setBoxing(context, context.TypeSystem.IntType, e);
                var etype = ValidationHelper.getType(context, e);
                if (!context.TypeSystem.IntType.isAssignableFrom(etype)) {
                    context.addError(CompileErrorId.NoImplicitConversion, e, BytecodeHelper.getDisplayName(etype), "int");
                }
            }
            if (initializer != null) {
                handleExpression(initializer, (type == null) ? targetType : type, true);
                var iinfo = initializer.getUserData(typeof(ExpressionInfo));
                if (iinfo == null) {
                    if (type == null || type.IsPrimitive) {
                        throw context.error(CompileErrorId.ImplicitlyTypedArrayTypeNotFound, arrayCreation);
					}
                } else if (type == null) {
                    arrayCreation.addOrReplaceUserData(new ExpressionInfo(ValidationHelper.getType(context, initializer)));
                } else {
                    var itype = ValidationHelper.getType(context, initializer);
					if (!type.isAssignableFrom(itype)) {
						context.addError(CompileErrorId.NoImplicitConversion, arrayCreation,
								BytecodeHelper.getDisplayName(itype), BytecodeHelper.getDisplayName(type));
					}
                    arrayCreation.addOrReplaceUserData(new ExpressionInfo(type));
                    setArrayInitializersTypes(initializer, type);
                }
            } else {
                arrayCreation.addOrReplaceUserData(new ExpressionInfo(type));
            }
            return null;
        }
        
        protected override Void handleArrayInitializer(ArrayInitializerExpressionNode arrayInitializer, TypeInfo targetType, bool nested) {
            TypeInfo targetElementType = null;
            if (targetType != null) {
                targetElementType = targetType.ElementType;
            }
            var type = targetElementType;
            foreach (var e in arrayInitializer.Values) {
                handleExpression(e, targetElementType, true);
                if (targetElementType != null) {
                    ValidationHelper.setBoxing(context, targetElementType, e);
                }
                var info = e.getUserData(typeof(ExpressionInfo));
                if (type == null) {
                    if (info != null) {
                        type = ValidationHelper.getType(context, e);
                    }
                } else if (!ValidationHelper.isAssignable(context, type, e)) {
                    if (info == null) {
                        if (type.IsPrimitive) {
							throw context.error(CompileErrorId.ArrayInitializerTypeMismatch, arrayInitializer,
									BytecodeHelper.getDisplayName(type));
                        }
                    } else {
                        var etype = ValidationHelper.getType(context, e);
                        if (etype.isAssignableFrom(type)) {
                            type = etype;
                        } else {
							throw context.error(CompileErrorId.ArrayInitializerTypeMismatch, arrayInitializer,
									BytecodeHelper.getDisplayName(type));
                        }
                    }
                }
            }
            if (type != null) {
                arrayInitializer.addOrReplaceUserData(new ExpressionInfo(type.ArrayType));
            }
            return null;
        }
        
        protected override Void handleAssign(AssignExpressionNode assign, TypeInfo targetType, bool nested) {
            var left = assign.Left;
            var right = assign.Right;
            handleExpression(left, targetType, true);
            var lt = checkLeftValue(left);
            handleExpression(right, lt, true);
            
            switch (assign.Operator) {
            case Assign:
                if (BytecodeHelper.isDelegateType(lt) && ValidationHelper.isMethod(right)) {
                    var ri = right.getUserData(typeof(ExpressionInfo));
                    MethodResolver.resolveDelegate(lt, right, ri, right);
                }
                ValidationHelper.setBoxing(context, lt, right);
                if (!ValidationHelper.isAssignable(context, lt, right)) {
                    var ri = right.getUserData(typeof(ExpressionInfo));
                    var rcname = (ri == null) ? "<null>" : BytecodeHelper.getDisplayName(ri.Type);
                    context.addError(CompileErrorId.NoImplicitConversion, right, rcname, BytecodeHelper.getDisplayName(lt));
                }
                break;
                
            case Add:
                if (lt == context.TypeSystem.StringType) {
                    ValidationHelper.getType(context, right);
                    break;
                }
                goto case Subtract;
                
            case Subtract:
                if (BytecodeHelper.isDelegateType(lt)) {
                    if (ValidationHelper.isMethod(right)) {
                        var ri = right.getUserData(typeof(ExpressionInfo));
                        MethodResolver.resolveDelegate(lt, right, ri, right);
                    }
                    var ri = right.getUserData(typeof(ExpressionInfo));
                    if (ri != null && lt != ValidationHelper.getType(context, right)) {
                        addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(assign.Operator), left, right);
                    }
                    break;
                } else if (ValidationHelper.isMethod(right)) {
                    throw context.error(CompileErrorId.UnexpectedMethodReference, right);
                }
                goto case Multiply;
                
            case Multiply:
            case Divide:
            case Modulo:
                ValidationHelper.setBoxing(context, lt, right);
                if (!lt.IsNumeric) {
                    addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(assign.Operator), left, right);
                } else if (!ValidationHelper.isAssignable(context, lt, right)) {
                    var ri = right.getUserData(typeof(ExpressionInfo));
                    var rcname = (ri == null) ? "<null>" : BytecodeHelper.getDisplayName(ri.Type);
                    context.addError(CompileErrorId.NoImplicitConversion, right, rcname, BytecodeHelper.getDisplayName(lt));
                }
                break;
                
            case And:
            case Or:
            case Xor:
                ValidationHelper.setBoxing(context, lt, right);
                var inError = false;
                if (!lt.IsBoolean) {
                    switch (lt.NumericTypeKind) {
                    case Float:
                    case Double:
                    case None:
                        addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(assign.Operator), left, right);
                        inError = true;
                        break;
                    }
                }
                if (!inError && !ValidationHelper.isAssignable(context, lt, right)) {
                    var ri = right.getUserData(typeof(ExpressionInfo));
                    var rcname = (ri == null) ? "<null>" : BytecodeHelper.getDisplayName(ri.Type);
                    context.addError(CompileErrorId.NoImplicitConversion, right, rcname, BytecodeHelper.getDisplayName(lt));
                }
                break;
                
            case LeftShift:
            case RightShift:
            case UnsignedRightShift:
                ValidationHelper.setBoxing(context, context.TypeSystem.IntType, right);
                inError = false;
                switch (lt.NumericTypeKind) {
                case Float:
                case Double:
                case None:
                    addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(assign.Operator), left, right);
                    inError = true;
                    break;
                }
                if (!inError) {
                    var ri = right.getUserData(typeof(ExpressionInfo));
                    if (ri == null) {
                        context.addError(CompileErrorId.NoImplicitConversion, right, "<null>", BytecodeHelper.getDisplayName(lt));
                    } else {
                        var rt = ValidationHelper.getType(context, right);
                        switch (rt.NumericTypeKind) {
                        case Long:
                        case Float:
                        case Double:
                        case None:
                            addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(assign.Operator), left, right);
                            break;
                        }
                    }
                }
                break;
            }
            assign.addOrReplaceUserData(new ExpressionInfo(lt));
            return null;
        }
        
        protected override Void handleBinary(BinaryExpressionNode binary, TypeInfo targetType, bool nested) {
            var left = binary.LeftOperand;
            var right = binary.RightOperand;
            handleExpression(left, null, true);
            handleExpression(right, null, true);
            var li = left.getUserData(typeof(ExpressionInfo));
            var ri = right.getUserData(typeof(ExpressionInfo));
            TypeInfo lt = null;
            TypeInfo rt = null;
            
            if (li != null && li.IsConstant && ri != null && ri.IsConstant) {
                var info = new ExpressionInfo(null);
                info.IsConstant = true;
                switch (binary.Operator) {
                case Divide:
                case Modulo:
                case Add:
                case Subtract:
                case Multiply:
                    try {
                        info.Value = context.ConstantBuilder.buildConstant(binary);
                    } catch (ArithmeticException) {
                        throw context.error(CompileErrorId.DivisionByZero, binary);
                    }
                    lt = ValidationHelper.getType(context, left);
                    rt = ValidationHelper.getType(context, right);
                    if (lt.IsNumeric && rt.IsNumeric) {
                        if (lt.TypeKind == TypeKind.Double || rt.TypeKind == TypeKind.Double) {
                            info.Type = context.TypeSystem.DoubleType;
                        } else if (lt.TypeKind == TypeKind.Float || rt.TypeKind == TypeKind.Float) {
                            info.Type = context.TypeSystem.FloatType;
                        } else if (lt.TypeKind == TypeKind.Long || rt.TypeKind == TypeKind.Long) {
                            info.Type = context.TypeSystem.LongType;
                        } else {
                            info.Type = context.TypeSystem.IntType;
                        }
                    } else {
                        info.Type = context.TypeSystem.StringType;
                    }
                    break;
                    
                case And:
                case Or:
                case Xor:
                    info.Value = context.ConstantBuilder.buildConstant(binary);
                    lt = ValidationHelper.getType(context, left);
                    rt = ValidationHelper.getType(context, right);
                    if (lt.IsBoolean) {
                        info.Type = context.TypeSystem.BooleanType;
                    } else if (lt.TypeKind == TypeKind.Long || rt.TypeKind == TypeKind.Long) {
                        info.Type = context.TypeSystem.LongType;
                    } else {
                        info.Type = context.TypeSystem.IntType;
                    }
                    break;
                    
                case LessThan:
                case LessThanOrEqual:
                case GreaterThan:
                case GreaterThanOrEqual:
                case Equal:
                case NotEqual:
                    info.Value = context.ConstantBuilder.buildConstant(binary);
                    info.Type = context.TypeSystem.BooleanType;
                    break;
                    
                case LeftShift:
                case RightShift:
                case UnsignedRightShift:
                    info.Value = context.ConstantBuilder.buildConstant(binary);
                    lt = ValidationHelper.getType(context, left);
                    if (lt.TypeKind == TypeKind.Long) {
                        info.Type = context.TypeSystem.LongType;
                    } else {
                        info.Type = context.TypeSystem.IntType;
                    }
                    break;

                case LogicalAnd:
                case LogicalOr:
                    info.Value = context.ConstantBuilder.buildConstant(binary);
                    info.Type = context.TypeSystem.BooleanType;
                    break;

                default:
                    throw new Exception("Internal error: unhandled operator: " + binary.getOperator());
                }
                binary.addOrReplaceUserData(info);
                return null;
            }
            
            if (li == null && ri == null) {
                throw context.error(CompileErrorId.BinaryOperatorNotApplicable, binary,
                    CompilerHelper.getDisplayName(binary.Operator), "<null>", "<null>");
            }
            var leftIsMethod = ValidationHelper.isMethod(left);
            var rightIsMethod = ValidationHelper.isMethod(right);
            if (leftIsMethod && rightIsMethod) {
                throw context.error(CompileErrorId.BinaryOperatorNotApplicable, binary,
                    CompilerHelper.getDisplayName(binary.Operator), "<method group>", "<method group>");
            }
            if (!leftIsMethod) {
                lt = (li == null) ? null : ValidationHelper.getType(context, left);
            }
            if (!rightIsMethod) {
                rt = (ri == null) ? null : ValidationHelper.getType(context, right);
            }

            switch (binary.Operator) {
            case Divide:
            case Modulo:
                if (ri != null && CompilerHelper.isZero(ri)) {
                    context.addError(CompileErrorId.DivisionByZero, binary);
                }
                goto case Add;
            case Add:
            case Subtract:
            case Multiply: {
                var info = new ExpressionInfo(null);
                if (lt != null && lt.IsNumeric && rt != null && rt.IsNumeric) {
                    if (lt.NumericTypeKind == NumericTypeKind.Double || rt.NumericTypeKind == NumericTypeKind.Double) {
                        lt = rt = context.TypeSystem.DoubleType;
                    } else if (lt.NumericTypeKind == NumericTypeKind.Float || rt.NumericTypeKind == NumericTypeKind.Float) {
                        lt = rt = context.TypeSystem.FloatType;
                    } else if (lt.NumericTypeKind == NumericTypeKind.Long || rt.NumericTypeKind == NumericTypeKind.Long) {
                        lt = rt = context.TypeSystem.LongType;
                    } else {
                        lt = rt = context.TypeSystem.IntType;
                    }
                    info.Type = lt;
                    ValidationHelper.setBoxing(context, lt, left);
                    ValidationHelper.setBoxing(context, rt, right);
                    binary.addOrReplaceUserData(info);
                } else if ((lt != null && BytecodeHelper.isDelegateType(lt)) || (rt != null && BytecodeHelper.isDelegateType(rt))) {
                    if ((binary.Operator != BinaryOperator.Add && binary.Operator != BinaryOperator.Subtract)
                            || (lt != null && rt != null && lt != rt)){
						info.Type = lt ?? rt;
                        addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                    } else {
                        info.Type = lt ?? rt;
                        if (leftIsMethod) {
                            MethodResolver.resolveDelegate(info.Type, left, li, left);
                        }
                        if (rightIsMethod) {
                            MethodResolver.resolveDelegate(info.Type, right, ri, right);
                        }
                    }
                    binary.addOrReplaceUserData(info);
                } else if ((lt == context.TypeSystem.StringType || rt == context.TypeSystem.StringType) && binary.Operator == BinaryOperator.Add) {
                    info.Type = context.TypeSystem.StringType;
                    binary.addOrReplaceUserData(info);
                } else {
					info.Type = lt ?? rt;
                    binary.addOrReplaceUserData(info);
                    addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                }
                break;
            }
            case And:
            case Or:
            case Xor:
                if (lt == null) {
                    binary.addOrReplaceUserData(new ExpressionInfo(lt ?? rt));
                    addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                } else if (lt.IsNumeric) {
                    if (ri == null || !rt.IsNumeric) {
                        addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                    }
                    if (lt.NumericTypeKind == NumericTypeKind.Long || rt.NumericTypeKind == NumericTypeKind.Long) {
                        lt = rt = context.TypeSystem.LongType;
                    } else {
                        lt = rt = context.TypeSystem.IntType;
                    }
                    ValidationHelper.setBoxing(context, lt, left);
                    ValidationHelper.setBoxing(context, rt, right);
                    binary.addOrReplaceUserData(new ExpressionInfo(lt));
                } else if (lt.IsBoolean) {
                    if (rt == null || !rt.IsBoolean) {
                        addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                    } else {
                        ValidationHelper.setBoxing(context, context.TypeSystem.BooleanType, left);
                        ValidationHelper.setBoxing(context, context.TypeSystem.BooleanType, right);
                    }
                    binary.addOrReplaceUserData(new ExpressionInfo(context.TypeSystem.BooleanType));
                } else {
                    binary.addOrReplaceUserData(new ExpressionInfo(lt ?? rt));
                    addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                }
                break;
                
            case LessThan:
            case LessThanOrEqual:
            case GreaterThan:
            case GreaterThanOrEqual:
                if (lt == null) {
                    addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                } else if (lt.IsNumeric) {
                    if (rt == null || !rt.IsNumeric) {
                        addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                    } else {
                        if (lt.NumericTypeKind == NumericTypeKind.Double || rt.NumericTypeKind == NumericTypeKind.Double) {
                            lt = rt = context.TypeSystem.DoubleType;
                        } else if (lt.NumericTypeKind == NumericTypeKind.Float || rt.NumericTypeKind == NumericTypeKind.Float) {
                            lt = rt = context.TypeSystem.FloatType;
                        } else if (lt.NumericTypeKind == NumericTypeKind.Long || rt.NumericTypeKind == NumericTypeKind.Long) {
                            lt = rt = context.TypeSystem.LongType;
                        } else {
                            lt = rt = context.TypeSystem.IntType;
                        }
                        ValidationHelper.setBoxing(context, lt, left);
                        ValidationHelper.setBoxing(context, rt, right);
                        binary.addOrReplaceUserData(lt);
                    }
                } else {
                    addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                }
				binary.addOrReplaceUserData(new ExpressionInfo(context.TypeSystem.BooleanType));
                break;
                
            case Equal:
            case NotEqual:
                if (lt == null && rt == null) {
                    binary.addOrReplaceUserData(new ExpressionInfo(context.TypeSystem.BooleanType, true));
                    binary.addOrReplaceUserData(context.TypeSystem.ObjectType);
                } else if (lt == null) {
                    if (rt.IsPrimitive) {
                        addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                    }
                    binary.addOrReplaceUserData(new ExpressionInfo(context.TypeSystem.BooleanType));
                    binary.addOrReplaceUserData(rt);
                } else if (rt == null) {
                    if (lt.IsPrimitive) {
                        addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                    }
                    binary.addOrReplaceUserData(new ExpressionInfo(context.TypeSystem.BooleanType));
                    binary.addOrReplaceUserData(lt);
                } else if (lt.IsNumeric || rt.IsNumeric) {
                    if (!lt.IsNumeric || !rt.IsNumeric) {
                        addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                    } else {
                        if (lt.IsPrimitive || rt.IsPrimitive) {
                            if (lt.NumericTypeKind == NumericTypeKind.Double || rt.NumericTypeKind == NumericTypeKind.Double) {
                                lt = rt = context.TypeSystem.DoubleType;
                            } else if (lt.NumericTypeKind == NumericTypeKind.Float || rt.NumericTypeKind == NumericTypeKind.Float) {
                                lt = rt = context.TypeSystem.FloatType;
                            } else if (lt.NumericTypeKind == NumericTypeKind.Long || rt.NumericTypeKind == NumericTypeKind.Long) {
                                lt = rt = context.TypeSystem.LongType;
                            } else {
                                lt = rt = context.TypeSystem.IntType;
                            }
                            ValidationHelper.setBoxing(context, lt, left);
                            ValidationHelper.setBoxing(context, rt, right);
                            binary.addOrReplaceUserData(lt);
                        } else {
                            binary.addOrReplaceUserData(context.TypeSystem.ObjectType);
                        }
                    }
                    binary.addOrReplaceUserData(new ExpressionInfo(context.TypeSystem.BooleanType));
                } else if (lt == context.TypeSystem.BooleanType || rt == context.TypeSystem.BooleanType) {
                    if (lt != context.TypeSystem.BooleanType || rt != context.TypeSystem.BooleanType) {
                        addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                    }
                    binary.addOrReplaceUserData(new ExpressionInfo(context.TypeSystem.BooleanType));
                    binary.addOrReplaceUserData(context.TypeSystem.BooleanType);
				} else if (lt.IsEnum || rt.IsEnum) {
					if (lt != rt) {
                        addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
					}
                    binary.addOrReplaceUserData(new ExpressionInfo(context.TypeSystem.BooleanType));
                    binary.addOrReplaceUserData(lt);
                } else {
                    binary.addOrReplaceUserData(new ExpressionInfo(context.TypeSystem.BooleanType));
                    binary.addOrReplaceUserData(context.TypeSystem.ObjectType);
                }
                break;
                
            case LeftShift:
            case RightShift:
            case UnsignedRightShift:
                if (lt == null || rt == null) {
					binary.addOrReplaceUserData(new ExpressionInfo(lt ?? rt));
					addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                } else {
                    switch (rt.NumericTypeKind) {
                    case Byte:
                    case Char:
                    case Short:
                    case Int:
                        if (lt.IsNumeric) {
                            switch (lt.NumericTypeKind) {
                            case Byte:
                            case Char:
                            case Short:
                            case Int:
                                lt = context.TypeSystem.IntType;
                                break;
                            case Long:
                                lt = context.TypeSystem.LongType;
                                break;
                            default:
                                addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                                break;
                            }
                            ValidationHelper.setBoxing(context, lt, left);
                            ValidationHelper.setBoxing(context, context.TypeSystem.IntType, right);
                        } else {
                            addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                        }
                        break;
                    default:
                        addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                        break;
                    }
					binary.addOrReplaceUserData(new ExpressionInfo(lt));
                }
                break;
                
            case As:
                if (lt != null && lt.IsPrimitive) {
                    throw context.error(CompileErrorId.ReferenceTypeValueExpected, left);
                }
                if (rt.IsPrimitive) {
                    throw context.error(CompileErrorId.ReferenceTypeExpected, right);
                }
                binary.addOrReplaceUserData(new ExpressionInfo(rt));
                break;
                
            case Instanceof:
                if (lt != null && lt.IsPrimitive) {
                    throw context.error(CompileErrorId.ReferenceTypeValueExpected, left);
                }
                if (rt.IsPrimitive) {
                    throw context.error(CompileErrorId.ReferenceTypeExpected, right);
                }
                binary.addOrReplaceUserData(new ExpressionInfo(context.TypeSystem.BooleanType));
                break;
                
            case LogicalOr:
            case LogicalAnd:
                if (lt == null || rt == null || !lt.IsBoolean || !rt.IsBoolean) {
                    addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                } else {
                    lt = ValidationHelper.getType(context, left);
                    rt = ValidationHelper.getType(context, right);
                    ValidationHelper.setBoxing(context, context.TypeSystem.BooleanType, left);
                    ValidationHelper.setBoxing(context, context.TypeSystem.BooleanType, right);
                }
                binary.addOrReplaceUserData(new ExpressionInfo(context.TypeSystem.BooleanType));
                break;
                
            case NullCoalescing:
                if (lt != null || rt != null) {
                    if (lt != null && lt.IsPrimitive) {
                        throw context.error(CompileErrorId.ReferenceTypeValueExpected, left);
                    }
                    if (rt != null && rt.IsPrimitive) {
                        throw context.error(CompileErrorId.ReferenceTypeValueExpected, right);
                    }
                    if (rt == null || (lt != null && lt.isAssignableFrom(rt))) {
                        binary.addOrReplaceUserData(new ExpressionInfo(lt));
                    } else if (lt == null || (lt != null && rt.isAssignableFrom(lt))) {
                        binary.addOrReplaceUserData(new ExpressionInfo(rt));
                    } else {
                        binary.addOrReplaceUserData(new ExpressionInfo(lt ?? rt));
                        addBinaryOperatorNotApplicableErrors(CompilerHelper.getDisplayName(binary.Operator), left, right);
                    }
                }
                break;
                
            default:
                throw new RuntimeException("Internal error: unhandled operator: " + binary.Operator);
            }
            return null;
        }

        protected override Void handleCast(CastExpressionNode cast, TypeInfo targetType, bool nested) {
            var type = CompilerHelper.resolveTypeReference(context, context.CurrentType.PackageName, cast.TargetType);
            handleExpression(cast.Expression, type, true);
            ValidationHelper.setBoxing(context, type, cast.Expression);
            var info = cast.Expression.getUserData(typeof(ExpressionInfo));
            if (info == null) {
                if (type.IsPrimitive) {
                    throw context.error(CompileErrorId.CastFromNullToPrimitiveType, cast);
                }
            } else {
                var etype = ValidationHelper.getType(context, cast.Expression);
                if (etype != type) {
                    if (type.TypeKind == TypeKind.Boolean || etype.TypeKind == TypeKind.Boolean) {
                        throw context.error(CompileErrorId.InvalidCast, cast,
                            BytecodeHelper.getDisplayName(etype), BytecodeHelper.getDisplayName(type));
                    }
                    if (type.OriginalTypeDefinition == etype.OriginalTypeDefinition || type.IsGenericParameter) {
                        context.addWarning(CompileErrorId.UncheckedCast, cast,
                            BytecodeHelper.getDisplayName(etype), BytecodeHelper.getDisplayName(type));
                    }
                }
            }
            cast.addOrReplaceUserData(new ExpressionInfo(type));
            return null;
        }
        
        protected override Void handleConditional(ConditionalExpressionNode conditional, TypeInfo targetType, bool nested) {
            handleExpression(conditional.Condition, context.TypeSystem.BooleanType, true);
            ValidationHelper.setBoxing(context, context.TypeSystem.BooleanType, conditional.Condition);
            var cinfo = conditional.Condition.getUserData(typeof(ExpressionInfo));
            if (cinfo == null || ValidationHelper.getType(context, conditional.Condition) != context.TypeSystem.BooleanType) {
                throw context.error(CompileErrorId.NoImplicitConversion, conditional.Condition,
                        BytecodeHelper.getDisplayName(cinfo == null ? null : cinfo.Type),
                        BytecodeHelper.getDisplayName(context.TypeSystem.BooleanType));
            }
            handleExpression(conditional.IfTrue, targetType, true);
            handleExpression(conditional.IfFalse, targetType, true);
            var tinfo = conditional.IfTrue.getUserData(typeof(ExpressionInfo));
            var finfo = conditional.IfFalse.getUserData(typeof(ExpressionInfo));
            if (tinfo == null && finfo == null) {
                throw context.error(CompileErrorId.ConditionalExpressionTypeInference, conditional);
            } else if (tinfo == null) {
                if (ValidationHelper.getType(context, conditional.IfFalse).IsPrimitive) {
                    throw context.error(CompileErrorId.ReferenceTypeValueExpected, conditional.IfFalse);
                }
                conditional.addOrReplaceUserData(new ExpressionInfo(finfo.Type));
            } else if (finfo == null) {
                if (ValidationHelper.getType(context, conditional.IfTrue).IsPrimitive) {
                    throw context.error(CompileErrorId.ReferenceTypeValueExpected, conditional.IfTrue);
                }
                conditional.addOrReplaceUserData(new ExpressionInfo(tinfo.Type));
            } else {
                if (ValidationHelper.getType(context, conditional.IfTrue).isAssignableFrom(
                        ValidationHelper.getType(context, conditional.IfFalse))) {
                    conditional.addOrReplaceUserData(new ExpressionInfo(tinfo.Type));
                    ValidationHelper.setBoxing(context, tinfo.Type, conditional.IfFalse);
                } else if (finfo.getType().isAssignableFrom(tinfo.getType())) {
                    conditional.addOrReplaceUserData(new ExpressionInfo(finfo.Type));
                    ValidationHelper.setBoxing(context, finfo.Type, conditional.IfTrue);
                } else {
                    throw context.error(CompileErrorId.ConditionalExpressionTypeInference, conditional);
                }
            }
            return null;
        }
        
        protected override Void handleElementAccess(ElementAccessExpressionNode elementAccess, TypeInfo targetType, bool nested) {
            var target = elementAccess.TargetObject;
            handleExpression(target, null, true);
            var tinfo = target.getUserData(typeof(ExpressionInfo));
            if (tinfo == null) {
                throw context.error(CompileErrorId.UnexpectedNull, elementAccess);
            }
            var ttype = ValidationHelper.getType(context, target);
            if (!ttype.IsArray) {
                var staticOnly = false;
                if (tinfo.Member != null && tinfo.Member.MemberKind == MemberKind.Type) {
                    staticOnly = true;
                    if (ttype.GenericTypeDefinition != null && !ttype.IsRaw) {
                        throw context.error(CompileErrorId.GenericStaticAccess, elementAccess);
                    }
                }
                var members = MemberInfo.getIndexers(context.AnnotatedTypeSystem, context.CurrentType, ttype, elementAccess.Indexes.size(), true);
                if (!members.any()) {
                    throw context.error(CompileErrorId.NoIndexerInType, elementAccess, BytecodeHelper.getDisplayName(ttype));
                }
                foreach (var arg in elementAccess.Indexes) {
                    if (arg.ExpressionKind != ExpressionKind.Lambda) {
                        handleExpression(arg, null, true);
                    }
                }
                foreach (var member in members) {
                    var method = member.GetAccessor;
                    if (method != null) {
                        method = MethodResolver.resolveMethod(Collections.singletonList(method), elementAccess.Indexes, targetType);
                    } else {
                        var setArgs = new ArrayList<ExpressionNode>();
                        setArgs.addAll(elementAccess.Indexes);
                        var name = new SimpleNameExpressionNode();
                        name.addOrReplaceUserData(new ExpressionInfo(member.Type));
                        setArgs.add(name);
                        method = MethodResolver.resolveMethod(Collections.singletonList(member.SetAccessor), setArgs, targetType);
                    }
                    if (method != null) {
                        fixArguments(method, elementAccess.Indexes, elementAccess);
                        elementAccess.addOrReplaceUserData(new ExpressionInfo(member.Type) { Member = member });
                        break;
                    }
                }
            } else {
                foreach (var e in elementAccess.Indexes) {
                    handleExpression(e, context.TypeSystem.IntType, true);
                    ValidationHelper.setBoxing(context, context.TypeSystem.IntType, e);
	                var etype = ValidationHelper.getType(context, e);
	                if (!context.TypeSystem.IntType.isAssignableFrom(etype)) {
	                    context.addError(CompileErrorId.NoImplicitConversion, e, BytecodeHelper.getDisplayName(etype), "int");
	                }
                }
                elementAccess.addOrReplaceUserData(new ExpressionInfo(ttype.ElementType));
            }
            return null;
        }
        
        protected override Void handleInvocation(InvocationExpressionNode invocation, TypeInfo targetType, bool nested) {
            handleExpression(invocation.TargetObject, null, true);
            var tinfo = invocation.TargetObject.getUserData(typeof(ExpressionInfo));
            var members = tinfo.Members;
            MethodInfo method = null;
            var arguments = invocation.Arguments;
            foreach (var arg in arguments) {
                if (arg.ExpressionKind != ExpressionKind.Lambda) {
                    handleExpression(arg, null, true);
                }
            }
            var isExtension = false;
            if (members == null) {
                if (!BytecodeHelper.isDelegateType(tinfo.Type)) {
                    throw context.error(CompileErrorId.NoInvocationTargetFound, invocation);
                }
                method = tinfo.Type.Methods.where(p => p.Name.equals("invoke")).single();
            } else {
                var methods = new ArrayList<MethodInfo>();
                foreach (var member in members) {
                    switch (member.MemberKind) {
                    case Field:
                    case Local:
                    case Property:
                        var type = member.Type;
                        if (!BytecodeHelper.isDelegateType(type)) {
                            continue;
                        }
                        method = type.Methods.where(p => p.Name.equals("invoke")).single();
                        method = MethodResolver.resolveMethod(Collections.singletonList(method), invocation.Arguments, targetType);
                        if (method != null) {
							if (context.CodeValidationContext.IsInMethod && context.CodeValidationContext.IsInLambda) {
	                            var currentMethod = context.CodeValidationContext.CurrentMethod;
                            	if (member.MemberKind == MemberKind.Local && member.Method != currentMethod) {
	                                member.IsUsedFromLambda = true;
					                var typeBuilder = context.LambdaScopes[context.CodeValidationContext.RootMethod];
					                context.getLocalField(typeBuilder, (LocalMemberInfo)member);
                            	} else if (!member.IsStatic && invocation.TargetObject.ExpressionKind == ExpressionKind.SimpleName) {
					                var typeBuilder = context.LambdaScopes[context.CodeValidationContext.RootMethod];
					                if (typeBuilder.getField("this$0") == null) {
					                    typeBuilder.defineField("this$0", context.CurrentType);
					                }
                            	}
                            }
                            tinfo.Member = member;
                            goto memberSelectionDone; 
                        }
                        break;
                    
                    case Method:
                        methods.add(member.Method);
                        break;
                    }
                }
                if (method == null) {
                    if (methods.size() > 0) {
                        method = MethodResolver.resolveMethod(methods, invocation.Arguments, targetType);
                    }
                    if (method == null && tinfo.ExtensionMethods != null) {
                        var target = (MemberAccessExpressionNode)invocation.TargetObject;
                        arguments = new ArrayList<ExpressionNode> { target.TargetObject };
                        arguments.addAll(invocation.Arguments);
                        var xmethods = tinfo.ExtensionMethods;
                        if (xmethods.any()) {
	                        method = MethodResolver.resolveMethod(xmethods, arguments, targetType, true);
	                        if (method != null) {
	                            isExtension = true;
	                        }
                        }
                    }
                    if (method == null) {
                    	if (context.addIgnoredError()) {
                    		throw new CodeErrorException();
                    	}
                    	if (methods.count() == 1) {
	                    	method = methods.single();
                    		int nparams = method.Parameters.count();
                    		int nargs = invocation.Arguments.size();
                    		if (nparams == nargs || (method.IsVarargs && nparams < nargs)) {
                    			var typeList = new StringBuilder();
                    			typeList.append("(");
                    			var first = true;
                    			foreach (var arg in invocation.Arguments) {
                    				if (first) {
                    					first = false;
                    				} else {
                    					typeList.append(", ");
                    				}
                    				var type = ValidationHelper.getType(context, arg);
                    				typeList.append(BytecodeHelper.getDisplayName(type));
                    			}
                    			typeList.append(")");
		                        throw context.error(CompileErrorId.CannotApplyInvocation, invocation,
		                        		BytecodeHelper.getDisplayName(method.DeclaringType),
		                        		BytecodeHelper.getDisplayName(method),
		                        		typeList.toString());
	                    	} else {
		                        throw context.error(CompileErrorId.NoMatchingNumberOfArguments, invocation, method.Name, nargs);
	                    	}
                    	}
                        throw context.error(CompileErrorId.NoInvocationTargetFound, invocation);
                    }
					if (context.CodeValidationContext.IsInMethod && context.CodeValidationContext.IsInLambda) {
			        	if ((isExtension || !method.IsStatic) && invocation.TargetObject.ExpressionKind == ExpressionKind.SimpleName) {
			                var typeBuilder = context.LambdaScopes[context.CodeValidationContext.RootMethod];
			                if (typeBuilder.getField("this$0") == null) {
			                    typeBuilder.defineField("this$0", context.CurrentType);
			                }
			        	}
		        	}
                    tinfo.Member = MemberInfo.getInfo(method);
                }
            }
            
        memberSelectionDone:
        	if (!context.CodeValidationContext.IsInLambda) {
	        	context.clearIgnoredError();
        	}
            fixArguments(method, arguments, invocation);
            var info = new ExpressionInfo(method.ReturnType) { Method = method, IsExtension = isExtension };
            invocation.addOrReplaceUserData(info);
            if (!BytecodeHelper.isDeprecated(context.AnnotatedTypeSystem, context.CurrentType)
                    && (!context.CodeValidationContext.IsInMethod ||
						!BytecodeHelper.isDeprecated(context.AnnotatedTypeSystem, context.CodeValidationContext.RootMethod))
                    && (BytecodeHelper.isDeprecated(context.AnnotatedTypeSystem, method))) {
                context.addWarning(CompileErrorId.DeprecatedMethod, invocation,
                        BytecodeHelper.getDisplayName(method.DeclaringType), BytecodeHelper.getDisplayName(method));
            }
            return null;
        }
        
        protected override Void handleLambda(LambdaExpressionNode lambda, TypeInfo targetType, bool nested) {
        	if (!context.CodeValidationContext.IsInMethod) {
                throw context.error(CompileErrorId.LambdaOutsideMethod, lambda);
        	}
            TypeBuilder typeBuilder;
            var inExpressionTree = context.CodeValidationContext.IsInExpressionTree;
            if (!inExpressionTree && targetType != null && !targetType.FullName.equals("stab/tree/ExpressionTree")) {
                typeBuilder = context.LambdaScopes[context.CodeValidationContext.RootMethod];
                if (typeBuilder == null) {
                    int n = context.CurrentType.NestedTypes.where(p => p.Name.startsWith("LambdaScope")).count();
                    typeBuilder = ((TypeBuilder)context.CurrentType).defineNestedType("LambdaScope" + n);
                    typeBuilder.setSourceFile(PathHelper.getFileName(lambda.Filename));
                    context.LambdaScopes[context.CodeValidationContext.RootMethod] = typeBuilder;
                    typeBuilder.setSynthetic(true);
                    typeBuilder.setSuper(true);
                    context.TypeBuilders.add(typeBuilder);
                    
                    var objectType = context.TypeSystem.ObjectType;
                    typeBuilder.setBaseType(objectType);
                    
                    var methodBuilder = typeBuilder.defineMethod("<init>");
                    methodBuilder.setReturnType(context.TypeSystem.VoidType);
                    var gen = methodBuilder.CodeGenerator;
                    gen.beginScope();
                    gen.emit(Opcode.Aload, gen.getLocal("this"));
                    gen.emit(Opcode.Invokespecial, objectType.getMethod("<init>", Query.empty<TypeInfo>()));
                    gen.emit(Opcode.Return);
                    gen.endScope();
                }
            } else if (inExpressionTree) {
                typeBuilder = (TypeBuilder)context.CodeValidationContext.CurrentMethod.DeclaringType;
            } else {
                var prefix = ((TypeBuilder)context.CurrentType).FullName + "#";
                int n = 0;
                while (context.TypeSystem.typeExists(prefix + n)) {
                    n++;
                }
                typeBuilder = context.TypeSystem.defineType(prefix + n);
            }
            
            var methodBuilder = lambda.getUserData(typeof(MethodBuilder));
            if (methodBuilder != null) {
                ((TypeBuilder)methodBuilder.DeclaringType).undefineMethod(methodBuilder);
            }
			var methodName = "lambda$" + context.CodeValidationContext.LambdaCount++;
            methodBuilder = typeBuilder.defineMethod(methodName);
            methodBuilder.setSynthetic(true);
            lambda.addOrReplaceUserData(methodBuilder);
            
            if (targetType == null) {
                int len = lambda.Parameters.size();
                for (int i = 0; i < len; i++) {
                    var p = lambda.Parameters[i];
                    var t = context.CodeValidationContext.LambdaParameters[i];
                    var pb = methodBuilder.addParameter(t);
                    pb.setName(p.Name ?? context.getIdentifier(p.NameOffset, p.NameLength));
                }
            } else {
                MethodInfo method;
                if (BytecodeHelper.isDelegateType(targetType)) {
                    method = targetType.Methods.where(p => p.Name.equals("invoke")).firstOrDefault();
                } else if (targetType.IsInterface) {
                    var methods = getAllMethods(targetType);
                    if (methods.count() != 1) {
                        throw context.error(CompileErrorId.IncompatibleLambdaInterface, lambda,
                            BytecodeHelper.getDisplayName(targetType), methods.count());
                    }
                    method = methods.single();
                } else if (BytecodeHelper.isExpressionTreeType(targetType)) {
                    var typeArg = targetType.GenericArguments.singleOrDefault();
                    if (typeArg == null) {
                        throw context.error(CompileErrorId.InvalidExpressionTreeArgument, lambda);
                    }
                    if (BytecodeHelper.isDelegateType(typeArg)) {
                        method = typeArg.Methods.where(p => p.Name.equals("invoke")).firstOrDefault();
                    } else if (typeArg.IsInterface) {
                        var methods = getAllMethods(typeArg);
                        if (methods.count() != 1) {
                            throw context.error(CompileErrorId.IncompatibleLambdaInterface, lambda,
                                BytecodeHelper.getDisplayName(typeArg), methods.count());
                        }
                        method = methods.single();
                    } else {
                        throw context.error(CompileErrorId.InvalidExpressionTreeArgument, lambda);
                    }
                } else {
                    throw context.error(CompileErrorId.InvalidLambdaTarget, lambda);
                }
                if (method.Parameters.count() != lambda.Parameters.size()) {
                    throw context.error(CompileErrorId.IncompatibleParameterList, lambda, BytecodeHelper.getDisplayName(method.DeclaringType));
                }
                foreach (TypeInfo t in method.Exceptions) {
                    methodBuilder.addException(t);
                }
                int i = 0;
                foreach (var pi in method.Parameters) {
                    var pn = lambda.Parameters[i++];
                    if (pn.Type != null) {
                        var explicitType = CompilerHelper.resolveTypeReference(context, typeBuilder.PackageName, pn.Type);
                        if (!pi.Type.IsGenericTypeDefinition && explicitType != pi.Type) {
                            throw context.error(CompileErrorId.IncompatibleParameterList, lambda,
                                BytecodeHelper.getDisplayName(method.DeclaringType));
                        }
                        var pb = methodBuilder.addParameter(explicitType);
                        pb.setName(pn.Name ?? context.getIdentifier(pn.NameOffset, pn.NameLength));
                    } else {
                        if (pi.Type.IsGenericTypeDefinition) {
                            throw context.error(CompileErrorId.IncompatibleParameterList, lambda,
                                BytecodeHelper.getDisplayName(method.DeclaringType));
                        }
                        var pb = methodBuilder.addParameter(pi.Type);
                        pb.setName(pn.Name ?? context.getIdentifier(pn.NameOffset, pn.NameLength));
                        // TODO: hasLocal(pb.getName) => error
                    }
                }
                methodBuilder.setReturnType((method.ReturnType.IsGenericParameter) ? null : method.ReturnType);
            }
            try {
                context.CodeValidationContext.enterLambdaMethod(methodBuilder);
                if (lambda.Body.StatementKind == StatementKind.Block) {
                    this.StatementValidator.handleStatement(lambda.Body, null);
                    if (targetType == null) {
                        var returnTypes = context.CodeValidationContext.LambdaReturnTypes;
                        TypeInfo rt = returnTypes[0];
                        for (int i = 1; i < returnTypes.size(); i++) {
                            var t = returnTypes[i];
                            if (t == null) {
                                if (rt.IsPrimitive) {
                                    throw context.error(CompileErrorId.NoLambdaReturnTypeInference, lambda);
                                }
                            } else if (rt == null) {
                                if (t.IsPrimitive) {
                                    throw context.error(CompileErrorId.NoLambdaReturnTypeInference, lambda);
                                }
                                rt = t;
                            } else if (!rt.isAssignableFrom(t)) {
                                if (t.isAssignableFrom(rt)) {
                                    rt = t;
                                } else {
                                    throw context.error(CompileErrorId.NoLambdaReturnTypeInference, lambda);
                                }
                            }
                        }
                        if (rt == null) {
                            context.CodeValidationContext.LambdaReturnType = context.TypeSystem.ObjectType;
                        } else {
                            context.CodeValidationContext.LambdaReturnType = rt;
                        }
                        methodBuilder.setReturnType(context.CodeValidationContext.LambdaReturnType);
                    }
                } else {
                    var expr = ((ExpressionStatementNode)lambda.Body).Expression;
                    handleExpression(expr, null, true);
                    var einfo = expr.getUserData(typeof(ExpressionInfo));
                    var returnType = (einfo == null) ? context.TypeSystem.ObjectType : ValidationHelper.getType(context, expr);
                    if (targetType == null || targetType.FullName.equals("stab/tree/ExpressionTree")) {
                        context.CodeValidationContext.LambdaReturnType = returnType;
                        methodBuilder.setReturnType(returnType);
                    } else {
                        var block = new BlockStatementNode();
                        var body = lambda.Body;
                        block.Filename = body.Filename;
                        block.Line = body.Line;
                        block.Column = body.Column;
                        block.DisabledWarnings = body.DisabledWarnings;
                        block.StartPosition = body.StartPosition;
                        block.EndPosition = body.EndPosition;
                        if (returnType == context.TypeSystem.VoidType || methodBuilder.ReturnType == context.TypeSystem.VoidType) {
                            block.Statements.add(body);
                        } else {
                            var returnNode = new ReturnStatementNode();
                            block.Statements.add(returnNode);
                            returnNode.Filename = body.Filename;
                            returnNode.Line = body.Line;
                            returnNode.Column = body.Column;
                            returnNode.DisabledWarnings = body.DisabledWarnings;
                            returnNode.StartPosition = body.StartPosition;
                            returnNode.EndPosition = body.EndPosition;
                            returnNode.Value = expr;
                        }
                        lambda.Body = block;
                        this.StatementValidator.handleStatement(block, null);
                    }
                }
                lambda.addOrReplaceUserData(new ExpressionInfo(targetType));
            } finally {
                context.CodeValidationContext.leaveLambdaMethod();
            }
            return null;
        }
        
        private Iterable<MethodInfo> getAllMethods(TypeInfo interfaceType) {
            var result = interfaceType.Methods;
            foreach (var i in interfaceType.Interfaces) {
                result = result.concat(getAllMethods(i));
            }
            return result.distinct().toList();
        }

        protected override Void handleLiteral(LiteralExpressionNode literal, TypeInfo targetType, bool nested) {
            switch (literal.getLiteralKind()) {
            case Character:
                literal.addOrReplaceUserData(new ExpressionInfo(null, null));
                break;
            case String:
                literal.addOrReplaceUserData(new ExpressionInfo(null, null));
                break;
            case VerbatimString:
                literal.addOrReplaceUserData(new ExpressionInfo(null, null));
                break;
            case DecimalInteger:
                literal.addOrReplaceUserData(new ExpressionInfo(null, null));
                break;
            case HexadecimalInteger:
                literal.addOrReplaceUserData(new ExpressionInfo(null, null));
                break;
            case Long:
                literal.addOrReplaceUserData(new ExpressionInfo(null, null));
                break;
            case HexadecimalLong:
                literal.addOrReplaceUserData(new ExpressionInfo(null, null));
                break;
            case Float:
                literal.addOrReplaceUserData(new ExpressionInfo(null, null));
                break;
            case Double:
                literal.addOrReplaceUserData(new ExpressionInfo(null, null));
                break;
            case Real:
                literal.addOrReplaceUserData(new ExpressionInfo(null, null));
                break;
            case Null:
                break;
            case True:
                literal.addOrReplaceUserData(new ExpressionInfo(null, null));
                break;
            case False:
                literal.addOrReplaceUserData(new ExpressionInfo(null, null));
                break;
            default:
                throw new Exception("Internal error: unhandled literal " + literal.LiteralKind);
            }
            return null;
        }
        
        protected override Void handleMemberAccess(MemberAccessExpressionNode memberAccess, TypeInfo targetType, bool nested) {
            var nameExpression = memberAccess.Member;
            var name = nameExpression.Name ?? context.getIdentifier(nameExpression.NameOffset, nameExpression.NameLength);
            handleExpression(memberAccess.TargetObject, null, true);
            var typeArgs = getTypeArguments(nameExpression.TypeArguments);
            var tinfo = memberAccess.TargetObject.getUserData(typeof(ExpressionInfo));
			if (tinfo == null) {
				throw context.error(CompileErrorId.NullMemberAccess, memberAccess);
			}
            if (tinfo.PackageName != null) {
                var packageName = tinfo.PackageName;
                var found = false;
                foreach (var s in context.MemberResolver.TypeFinder.getSubPackages(packageName)) {
                    if (s.equals(name)) {
                        var info = new ExpressionInfo(null) { PackageName = packageName + '/' + name };
                        memberAccess.addOrReplaceUserData(info);
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    if (context.MemberResolver.TypeFinder.getClasses(packageName).contains(name)) {
                        var type = context.TypeSystem.getType(packageName + '/' + name);
                        if (type.IsGenericTypeDefinition) {
                            if (typeArgs.any()) {
                                if (type.GenericArguments.count() != typeArgs.count()) {
                                    throw context.error(CompileErrorId.WrongTypeArgumentNumber, memberAccess,
                                        BytecodeHelper.getDisplayName(type), type.GenericArguments.count());
                                }
                                type = context.TypeSystem.getGenericType(type, typeArgs);
                            } else {
                                type = type.RawType;
                                context.addWarning(CompileErrorId.RawTypeUsage, memberAccess,
                                    BytecodeHelper.getDisplayName(type.GenericTypeDefinition), BytecodeHelper.getDisplayName(type));
                            }
                        }
                        memberAccess.addOrReplaceUserData(new ExpressionInfo(type));
                        found = true;
                    }
                }
                if (!found) {
                    throw context.error(CompileErrorId.UnresolvedName, nameExpression, name);
                }
            } else {
                if (ValidationHelper.isMethod(memberAccess.TargetObject)) {
                    throw context.error(CompileErrorId.InvalidTargetForMemberAccess, memberAccess.TargetObject, name);
                }
                var ttype = ValidationHelper.getType(context, memberAccess.TargetObject);
                var staticOnly = false;
                if (tinfo.Member != null && tinfo.Member.MemberKind == MemberKind.Type) {
                    staticOnly = true;
                    if (ttype.GenericTypeDefinition != null && !ttype.IsRaw) {
                        context.addError(CompileErrorId.GenericStaticAccess, nameExpression);
                    }
                    ttype = ttype.OriginalTypeDefinition;
                }
				try {
					var members = MemberInfo.getMembers(context.AnnotatedTypeSystem, context.CurrentType, ttype, name, typeArgs, true);
					if (staticOnly) {
						members = members.where(p => p.IsStatic).toList();
					}

					var info = new ExpressionInfo(null) { Members = members };
					if (staticOnly) {
						info.ExtensionMethods = Query.empty<MethodInfo>();
					} else {
						info.ExtensionMethods = context.MemberResolver.getExtensionMethods(ttype, name, typeArgs);
					}
					if (!info.Members.any() && !info.ExtensionMethods.any()) {
						throw context.error(CompileErrorId.NoAccessibleMembers, nameExpression, name, BytecodeHelper.getDisplayName(ttype));
					}
					info.IsSuperCall = memberAccess.TargetObject.ExpressionKind == ExpressionKind.SuperAccess;
					memberAccess.addOrReplaceUserData(info);
				} catch (IllegalArgumentException e) {
                    throw context.error(CompileErrorId.NoAccessibleMembers, nameExpression, name, BytecodeHelper.getDisplayName(ttype));
				}
                if (targetType != null && BytecodeHelper.isDelegateType(targetType)) {
                    MethodResolver.resolveDelegate(targetType, memberAccess, tinfo, memberAccess);
                }
            }
            return null;
        }
        
        protected override Void handleObjectCreation(ObjectCreationExpressionNode objectCreation, TypeInfo targetType, bool nested) {
            var type = CompilerHelper.resolveTypeReference(context, context.CurrentType.PackageName, objectCreation.Type);
            if (BytecodeHelper.isDelegateType(type)) {
                if (objectCreation.Arguments.size() != 1) {
                    throw context.error(CompileErrorId.MethodNameExpected, objectCreation);
                }
                if (objectCreation.Initializer != null) {
                    context.addError(CompileErrorId.DelegateCreationInitializer, objectCreation);
                }
                var arg = objectCreation.Arguments[0];
                handleExpression(arg, null, true);
                
                var info = new ExpressionInfo(type);
                MethodResolver.resolveDelegate(type, arg, info, objectCreation);
                objectCreation.addOrReplaceUserData(info);
            } else {
                if (type.IsAbstract) {
                    context.addError(CompileErrorId.AbstractTypeCreation, objectCreation.Type, BytecodeHelper.getDisplayName(type));
                }
				if (type.HasWildcardArgument) {
                    context.addError(CompileErrorId.WildcardTypeCreation, objectCreation.Type, BytecodeHelper.getDisplayName(type));
				}
                var constructors = new ArrayList<MethodInfo>();
                foreach (var method in type.Methods) {
                    if (method.Name.equals("<init>")) {
                        constructors.add(method);
                    }
                }
                if (constructors.isEmpty()) {
                    throw context.error(CompileErrorId.NoAccessibleConstructors, objectCreation.Type, BytecodeHelper.getDisplayName(type));
                }
                foreach (var arg in objectCreation.Arguments) {
                    if (arg.ExpressionKind != ExpressionKind.Lambda) {
                        handleExpression(arg, null, true);
                    }
                }
                var constructor = MethodResolver.resolveMethod(constructors, objectCreation.Arguments, context.TypeSystem.VoidType);
                if (constructor == null || !context.CurrentType.canAccessMember(constructor.DeclaringType, constructor.IsPublic,
                    constructor.IsProtected, constructor.IsPrivate)) {
                    // TODO: better message
                    throw context.error(CompileErrorId.NoAccessibleConstructors, objectCreation.Type, BytecodeHelper.getDisplayName(type));
                }
                
                fixArguments(constructor, objectCreation.Arguments, objectCreation);
                var info = new ExpressionInfo(type) { Method = constructor };
                objectCreation.addOrReplaceUserData(info);
                if (!BytecodeHelper.isDeprecated(context.AnnotatedTypeSystem, context.CurrentType)
						&& (!context.CodeValidationContext.IsInMethod ||
							!BytecodeHelper.isDeprecated(context.AnnotatedTypeSystem, context.CodeValidationContext.RootMethod))
                        && (BytecodeHelper.isDeprecated(context.AnnotatedTypeSystem, constructor))) {
                    context.addWarning(CompileErrorId.DeprecatedMethod, objectCreation.Type,
                            BytecodeHelper.getDisplayName(constructor.DeclaringType), BytecodeHelper.getDisplayName(constructor));
                }
                
                var init = objectCreation.Initializer;
                if (init != null) {
                    if (init.ExpressionKind == ExpressionKind.ObjectInitializer) {
                        var initializer = (ObjectInitializerExpressionNode)init;
                        foreach (var mi in initializer.MemberInitializers) {
                            var property = context.getIdentifier(mi.NameOffset, mi.NameLength);
                            var members = MemberInfo.getMembers(context.AnnotatedTypeSystem, context.CurrentType, type, property, true);
                            MemberInfo memb = null;
                            foreach (var m in members) {
                                if (m.MemberKind == MemberKind.Property) {
									var setter = m.SetAccessor;
									if (setter != null && context.CurrentType.canAccessMember(setter.DeclaringType, setter.IsPublic,
											setter.IsProtected, setter.IsPrivate)) {
										memb = m;
										break;
									}
								} else if (m.MemberKind == MemberKind.Field) {
                                    memb = m;
                                    break;
                                }
                            }
                            if (memb == null) {
                                throw context.error(CompileErrorId.NoAccessibleMembers, objectCreation,
                                    property, BytecodeHelper.getDisplayName(type));
                            }
                            handleExpression(mi.Value, memb.Type, true);
                            if (!ValidationHelper.isAssignable(context, memb.Type, mi.Value)) {
                                throw context.error(CompileErrorId.NoImplicitConversion, mi,
                                        BytecodeHelper.getDisplayName(info == null ? null : ValidationHelper.getType(context, mi.Value)),
                                        BytecodeHelper.getDisplayName(memb.Type));
                            }
                            mi.addOrReplaceUserData(memb);
                        }
                    } else {
                        var initializer = (CollectionInitializerExpressionNode)init;
                        var argTypeBounds = new ArrayList<HashSet<TypeInfo>>();
                        int nargs = 0;
                        foreach (var args in initializer.Values) {
                            if (nargs == 0) {
                                nargs = args.size();
                                for (int i = 0; i < nargs; i++) {
                                	argTypeBounds.add(new HashSet<TypeInfo>());
                                }
                            } else if (nargs != args.size()) {
                                throw context.error(CompileErrorId.MalformedCollectionInitializer, initializer);
                            }
                            int i = 0;
                            foreach (var e in args) {
                                handleExpression(e, null, true);
                                ExpressionInfo ei = e.getUserData(typeof(ExpressionInfo));
	                            var bounds = argTypeBounds[i++];
                                bounds.add((ei == null) ? null : ValidationHelper.getType(context, e));
                            }
                        }
                        var paramTypes = new ArrayList<TypeInfo>();
                        foreach (var bounds in argTypeBounds) {
                        	var fixedType = MethodResolver.getFixedType(context.TypeSystem, bounds);
                        	if (fixedType == null) {
                                throw context.error(CompileErrorId.InconsistentCollectionInitializerTypes, initializer);
                        	}
                            paramTypes.add(fixedType);
                        }
                        var members = MemberInfo.getMembers(context.AnnotatedTypeSystem, type, "add");
                        var candidates = new ArrayList<MethodInfo>();
                        foreach (var m in members) {
                            if (m.MemberKind == MemberKind.Method) {
                                var meth = m.Method;
                                if (meth.Parameters.count() != nargs) {
                                    continue;
                                }
                                int i = 0;
                                var found = true;
                                foreach (var p in meth.Parameters) {
                                    if (!p.Type.isAssignableFrom(paramTypes[i])) {
                                        found = false;
                                        break;
                                    }
                                    i++;
                                }
                                if (found) {
                                    candidates.add(m.Method);
                                }
                            }
                        }
                        if (candidates.size() == 0) {
                            members = MemberInfo.getMembers(context.AnnotatedTypeSystem, type, "put");
                            foreach (var m in members) {
                                if (m.MemberKind == MemberKind.Method) {
                                    var meth = m.Method;
                                    if (meth.Parameters.count() != nargs) {
                                        continue;
                                    }
                                    int i = 0;
                                    var found = true;
                                    foreach (var p in meth.Parameters) {
                                        if (!p.Type.isAssignableFrom(paramTypes[i])) {
                                            found = false;
                                            break;
                                        }
                                        i++;
                                    }
                                    if (found) {
                                        candidates.add(m.Method);
                                    }
                                }
                            }
                        }
                        if (candidates.size() != 1) {
                            throw context.error(CompileErrorId.NoAddOrPutMethod, initializer, BytecodeHelper.getDisplayName(type));
                        }
                        var method = candidates[0];
                        foreach (var args in initializer.Values) {
                            var it = method.Parameters.iterator();
                            foreach (var e in args) {
                                var p = it.next();
                                if (BytecodeHelper.isDelegateType(p.Type)) {
                                    var ei = e.getUserData(typeof(ExpressionInfo));
                                    if (ei.Members != null) {
                                        MethodResolver.resolveDelegate(p.Type, e, ei, e);
                                    }
                                } else {
                                    ValidationHelper.setBoxing(context, p.Type, e);
                                }
                            }
                        }
                        initializer.addOrReplaceUserData(method);
                    }
                }
            }
            return null;
        }
        
        protected override Void handleSimpleName(SimpleNameExpressionNode simpleName, TypeInfo targetType, bool nested) {
            var name = simpleName.Name ?? context.getIdentifier(simpleName.NameOffset, simpleName.NameLength);
            var typeArgs = getTypeArguments(simpleName.TypeArguments);
            var members = context.MemberResolver.resolveName(context.CurrentType, name, typeArgs);
            if (!members.any()) {
                var packageName = context.MemberResolver.getPackageFromAlias(name);
                if (packageName == null) {
                    if (context.MemberResolver.TypeFinder.getSubPackages(name).any()
                     || context.MemberResolver.TypeFinder.getClasses(name).any()) {
                        packageName = name;
                    } else {
                        throw context.error(CompileErrorId.UnresolvedName, simpleName, name);
                    }
                }
                var info = new ExpressionInfo(null) { PackageName = packageName };
                simpleName.addOrReplaceUserData(info);
            } else {
                if (context.CodeValidationContext.IsStatic) {
                    members = members.where(p => p.IsStatic).toList();
					if (!members.any()) {
                        throw context.error(CompileErrorId.UnresolvedStaticName, simpleName, name);
					}
                }
                var info = new ExpressionInfo(null) { Members = members };
                simpleName.addOrReplaceUserData(info);
                if (targetType != null && BytecodeHelper.isDelegateType(targetType)) {
                    MethodResolver.resolveDelegate(targetType, simpleName, info, simpleName);
                }
            }
            return null;
        }
        
        protected override Void handleSizeof(SizeofExpressionNode sizeofExpression, TypeInfo targetType, bool nested) {
            var expression = sizeofExpression.Expression;
            handleExpression(expression, null, true);
            var einfo = expression.getUserData(typeof(ExpressionInfo));
            if (einfo == null || !ValidationHelper.getType(context, expression).IsArray) {
                context.addError(CompileErrorId.ArrayExpressionExpected, expression);
            }
            sizeofExpression.addOrReplaceUserData(new ExpressionInfo(context.TypeSystem.IntType));
            return null;
        }
        
        protected override Void handleSuperAccess(SuperAccessExpressionNode superAccess, TypeInfo targetType, bool nested) {
            if (context.CodeValidationContext.IsStatic) {
                context.addError(CompileErrorId.StaticSuperAccess, superAccess);
            }
            if (context.CodeValidationContext.IsInNestedMethod) {
                throw context.error(CompileErrorId.NestedMethodSuperAccess, superAccess);
            }
            superAccess.addOrReplaceUserData(new ExpressionInfo(context.CurrentType.BaseType));
            return null;
        }
        
        protected override Void handleThisAccess(ThisAccessExpressionNode thisAccess, TypeInfo targetType, bool nested) {
            if (context.CodeValidationContext.IsStatic) {
                context.addError(CompileErrorId.StaticThisAccess, thisAccess);
            }
            if (context.CodeValidationContext.IsInMethod && context.CodeValidationContext.IsInLambda) {
                var typeBuilder = context.LambdaScopes[context.CodeValidationContext.RootMethod];
                if (typeBuilder.getField("this$0") == null) {
                    typeBuilder.defineField("this$0", context.CurrentType);
                }
            }
            thisAccess.addOrReplaceUserData(new ExpressionInfo(context.CurrentType));
            return null;
        }
        
        protected override Void handleType(TypeExpressionNode type, TypeInfo targetType, bool nested) {
            var t = CompilerHelper.resolveTypeReference(context, context.CurrentType.PackageName, type.TypeReference);
            type.addOrReplaceUserData(new ExpressionInfo(t));
            return null;
        }
        
        protected override Void handleTypeof(TypeofExpressionNode typeofExpression, TypeInfo targetType, bool nested) {
            var type = CompilerHelper.resolveTypeReference(context, context.CurrentType.PackageName, typeofExpression.Type);
            if (type.IsGenericParameter) {
                throw context.error(CompileErrorId.TypeofTypeVariable, typeofExpression, BytecodeHelper.getDisplayName(type));
            } else if (type.GenericArguments.any()) {
                if (!type.GenericArguments.all(p => p.TypeKind == TypeKind.UnboundedWildcard)) {
                    throw context.error(CompileErrorId.TypeofGenericType, typeofExpression, BytecodeHelper.getDisplayName(type));
                }
            }
            typeofExpression.addOrReplaceUserData(type);
            var typeSystem = context.TypeSystem;
            var classType = typeSystem.getType("java/lang/Class");
            if (type.IsPrimitive) {
                if (type == context.TypeSystem.VoidType) {
                    classType = typeSystem.getGenericType(classType, Collections.singletonList(typeSystem.getType("java/lang/Void")));
                } else {
                    classType = typeSystem.getGenericType(classType, Collections.singletonList(typeSystem.getBoxingMethod(type).ReturnType));
                }
            } else {
                classType = typeSystem.getGenericType(classType, Collections.singletonList(type));
            }
            typeofExpression.addOrReplaceUserData(new ExpressionInfo(classType));
            return null;
        }
        
        protected override Void handleUnary(UnaryExpressionNode unary, TypeInfo targetType, bool nested) {
            handleExpression(unary.Operand, null, true);
            var oinfo = unary.Operand.getUserData(typeof(ExpressionInfo));
            if (oinfo == null) {
                throw context.error(CompileErrorId.UnaryOperatorNotApplicable, unary,
                    CompilerHelper.getDisplayName(unary.Operator), "<null>");
            }
            var info = new ExpressionInfo(null);
            TypeInfo type;
            if (oinfo.IsConstant) {
                switch (unary.Operator) {
                case PostDecrement:
                case PostIncrement:
                case PreDecrement:
                case PreIncrement:
                    checkLeftValue(unary.Operand);
                    break;
                case Minus:
                    info.Value = context.ConstantBuilder.buildConstant(unary, Boolean.TRUE);
                    break;
                default:
                    info.Value = context.ConstantBuilder.buildConstant(unary);
                    break;
                }
                info.IsConstant = true;
                type = ValidationHelper.getType(context, unary.Operand);
                info.Type = type;
                unary.addOrReplaceUserData(info);
                return null;
            }
            
            type = ValidationHelper.getType(context, unary.Operand);
            unary.addOrReplaceUserData(info);
            
            switch (unary.Operator) {
            case Complement:
                switch (type.NumericTypeKind) {
                case Byte:
                case Char:
                case Int:
                case Short:
                    info.Type = context.TypeSystem.IntType;
                    ValidationHelper.setBoxing(context, context.TypeSystem.IntType, unary.Operand);
                    break;
                case Long:
                    info.Type = context.TypeSystem.LongType;
                    ValidationHelper.setBoxing(context, context.TypeSystem.LongType, unary.Operand);
                    break;
                default:
                    info.Type = type;
                    context.addError(CompileErrorId.UnaryOperatorNotApplicable, unary,
                        CompilerHelper.getDisplayName(unary.Operator), BytecodeHelper.getDisplayName(type));
                    break;
                }
                break;
            case Minus:
            case Plus:
                switch (type.NumericTypeKind) {
                case Byte:
                case Char:
                case Int:
                case Short:
                    info.Type = context.TypeSystem.IntType;
                    ValidationHelper.setBoxing(context, context.TypeSystem.IntType, unary.Operand);
                    break;
                case Long:
                    info.Type = context.TypeSystem.LongType;
                    ValidationHelper.setBoxing(context, context.TypeSystem.LongType, unary.Operand);
                    break;
                case Float:
                    info.Type = context.TypeSystem.FloatType;
                    ValidationHelper.setBoxing(context, context.TypeSystem.FloatType, unary.Operand);
                    break;
                case Double:
                    info.Type = context.TypeSystem.DoubleType;
                    ValidationHelper.setBoxing(context, context.TypeSystem.DoubleType, unary.Operand);
                    break;
                default:
                    info.Type = type;
                    context.addError(CompileErrorId.UnaryOperatorNotApplicable, unary,
                        CompilerHelper.getDisplayName(unary.Operator), BytecodeHelper.getDisplayName(type));
                    break;
                }
                break;
            case Not:
                if (type.IsBoolean) {
                    info.Type = context.TypeSystem.BooleanType;
                    ValidationHelper.setBoxing(context, context.TypeSystem.BooleanType, unary.Operand);
                } else {
                    info.Type = type;
                    context.addError(CompileErrorId.UnaryOperatorNotApplicable, unary,
                        CompilerHelper.getDisplayName(unary.Operator), BytecodeHelper.getDisplayName(type));
                }
                break;
            case PostIncrement:
            case PostDecrement:
            case PreIncrement:
            case PreDecrement:
                checkLeftValue(unary.Operand);
                info.Type = type;
                switch (type.NumericTypeKind) {
                case Byte:
                case Char:
                case Int:
                case Short:
                    ValidationHelper.setBoxing(context, context.TypeSystem.IntType, unary.Operand);
                    break;

                case Long:
                    ValidationHelper.setBoxing(context, context.TypeSystem.LongType, unary.Operand);
                    break;
                    
                case Float:
                    ValidationHelper.setBoxing(context, context.TypeSystem.FloatType, unary.Operand);
                    break;

                case Double:
                    ValidationHelper.setBoxing(context, context.TypeSystem.DoubleType, unary.Operand);
                    break;
                    
                default:
                    context.addError(CompileErrorId.UnaryOperatorNotApplicable, unary,
                        CompilerHelper.getDisplayName(unary.Operator), BytecodeHelper.getDisplayName(type));
                    break;
                }
                break;
            default:
                throw new Exception("Internal error: unhandled unary operator " + unary.Operator);
            }
            return null;
        }

        private void setArrayInitializersTypes(ArrayInitializerExpressionNode initializer, TypeInfo type) {
            initializer.getUserData(typeof(ExpressionInfo)).Type = type;
            foreach (var e in initializer.Values) {
                if (e.ExpressionKind == ExpressionKind.ArrayInitializer) {
                    setArrayInitializersTypes((ArrayInitializerExpressionNode)e, type.ElementType);
                }
            }
        }
        
        private void addBinaryOperatorNotApplicableErrors(String operator, ExpressionNode left, ExpressionNode right) {
            var li = left.getUserData(typeof(ExpressionInfo));
            var ri = right.getUserData(typeof(ExpressionInfo));
            var lcname = (li == null) ? "<null>" : BytecodeHelper.getDisplayName(li.Type);
            var rcname = (ri == null) ? "<null>" : BytecodeHelper.getDisplayName(ri.Type);
            context.addError(CompileErrorId.BinaryOperatorNotApplicable, left, operator, lcname, rcname);
        }

        private TypeInfo checkLeftValue(ExpressionNode left) {
            var linfo = left.getUserData(typeof(ExpressionInfo));
            if (left.ExpressionKind != ExpressionKind.ElementAccess) {
                if (linfo == null || linfo.Members == null || !linfo.Members.any(p => p.MemberKind != MemberKind.Method)) {
                    throw context.error(CompileErrorId.IllegalLeftValue, left);
                }
            }
            var ltype = ValidationHelper.getType(context, left);
            var member = linfo.Member;
            if (member == null) {
                if (left.ExpressionKind != ExpressionKind.ElementAccess) {
                    context.addError(CompileErrorId.IllegalLeftValue, left);
                }
            } else {
                switch (member.MemberKind) {
                case Field:
                    var field = member.Field;
                    if (field.IsFinal) {
                        if (context.CodeValidationContext.IsInMethod) {
                            if (!context.CodeValidationContext.CurrentMethod.Name.equals((field.IsStatic) ? "<clinit>" : "<init>")) {
                                context.addError(CompileErrorId.FinalFieldModification, left);
                            }
                        } else if (field.DeclaringType != context.CurrentType) {
                            context.addError(CompileErrorId.FinalFieldModification, left);
                        }
                    }
                    break;
                case Indexer:
                    if (member.SetAccessor == null) {
                        context.addError(CompileErrorId.ReadonlyIndexerModification, left);
                    }
                    break;
                case Property:
					var setter = member.SetAccessor;
                    if (setter == null ||
						!context.CurrentType.canAccessMember(setter.DeclaringType, setter.IsPublic, setter.IsProtected, setter.IsPrivate)) {
                        context.addError(CompileErrorId.ReadonlyPropertyModification, left);
                    }
                    break;
                case Local:
                    if (context.CodeValidationContext.IsInExpressionTree && context.CodeValidationContext.CurrentMethod != member.Method) {
                        context.addError(CompileErrorId.ExternalLocalAssignment, left);
                    }
                    break;
                default:
                    context.addError(CompileErrorId.IllegalLeftValue, left);
                    break;
                }
            }
            return ltype;
        }
        
        private Iterable<TypeInfo> getTypeArguments(List<TypeReferenceNode> typeArguments) {
            if (typeArguments.size() == 0) {
                return Query.empty();
            } else {
                var result = new ArrayList<TypeInfo>();
                foreach (var t in typeArguments) {
                    result.add(CompilerHelper.resolveTypeReference(context, context.CurrentType.PackageName, t));
                }
                return result;
            }
        }
        
        private void fixArguments(MethodInfo method, List<ExpressionNode> arguments, ExpressionNode expression) {
            var it1 = method.Parameters.iterator();
            var it2 = arguments.iterator();
            int nParams = method.Parameters.count();
            int len = (method.IsVarargs) ? nParams - 1 : nParams;
            for (int i = 0; i < len; i++) {
                var p = it1.next();
                if (!it2.hasNext()) {
                    break;
                }
                var a = it2.next();
                if (BytecodeHelper.isDelegateType(p.Type)) {
                    var ai = a.getUserData(typeof(ExpressionInfo));
                    if (ai.Members != null) {
                        MethodResolver.resolveDelegate(p.Type, a, ai, expression);
                    }
                } else {
                    ValidationHelper.setBoxing(context, p.Type, a);
                }
            }
            if (method.IsVarargs) {
                var paramType = it1.next().Type.ElementType;
                var first = true;
                while (it2.hasNext()) {
                    var a = it2.next();
                    if (first) {
                        first = false;
                        if (ValidationHelper.isAssignable(context, paramType, a)) {
                            ValidationHelper.setBoxing(context, paramType, a);
                        }
                    } else if (BytecodeHelper.isDelegateType(paramType)) {
                        var ai = a.getUserData(typeof(ExpressionInfo));
                        if (ai.Members != null) {
                            MethodResolver.resolveDelegate(paramType, a, ai, expression);
                        }
                    } else {
                        ValidationHelper.setBoxing(context, paramType, a);
                    }
                }
            }
        }
    }
}