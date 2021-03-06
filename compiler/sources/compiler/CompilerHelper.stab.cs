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
    class CompilerHelper {
        static String getDisplayName(AssignOperator operator) {
            switch (operator) {
            case Add:
                return "+=";
            case And:
                return "&=";
            case Divide:
                return "/=";
            case LeftShift:
                return "<<=";
            case Modulo:
                return "%=";
            case Multiply:
                return "*=";
            case Or:
                return "|=";
            case RightShift:
                return ">>=";
            case Subtract:
                return "-=";
            case UnsignedRightShift:
                return ">>>=";
            case Xor:
                return "^=";
            default:
                throw new IllegalStateException();
            }
        }
    
        static String getDisplayName(BinaryOperator operator) {
            switch (operator) {
            case Add:
                return "+";
            case And:
                return "&";
            case As:
                return "as";
            case Divide:
                return "/";
            case Equal:
                return "==";
            case GreaterThan:
                return ">";
            case GreaterThanOrEqual:
                return ">=";
            case Instanceof:
                return "instanceof";
            case LeftShift:
                return "<<";
            case LessThan:
                return "<";
            case LessThanOrEqual:
                return "<=";
            case LogicalAnd:
                return "&&";
            case LogicalOr:
                return "||";
            case Modulo:
                return "%";
            case Multiply:
                return "*";
            case NotEqual:
                return "!=";
            case NullCoalescing:
                return "??";
            case Or:
                return "|";
            case RightShift:
                return ">>";
            case Subtract:
                return "-";
            case UnsignedRightShift:
                return ">>>";
            case Xor:
                return "^";
            default:
                throw new IllegalStateException();
            }
        }

        static String getDisplayName(UnaryOperator operator) {
            switch (operator) {
            case Complement:
                return "~";
            case Minus:
                return "-";
            case Not:
                return "!";
            case Plus:
                return "+";
            case PostDecrement:
            case PreDecrement:
                return "--";
            case PreIncrement:
            case PostIncrement:
                return "++";
            default:
                throw new IllegalStateException();
            }
        }

        static bool isZero(ExpressionInfo info) {
            if (info.IsConstant) {
                Object value = info.Value;
                if (value instanceof Double) {
                    return ((Double)value).doubleValue() == 0d;
                } else if (value instanceof Float) {
                    return ((Float)value).floatValue() == 0f;
                } else if (value instanceof Long) {
                    return ((Long)value).longValue() == 0l;
                } else if (value instanceof Integer) {
                    return ((Integer)value).intValue() == 0;
                } else if (value instanceof Character) {
                    return ((Character)value).charValue() == 0;
                } else if (value instanceof Byte) {
                    return ((Byte)value).byteValue() == 0;
                } else if (value instanceof Short) {
                    return ((Short)value).shortValue() == 0;
                }
            }
            return false;
        }

        static List<String> getName(CompilerContext context, TypeReferenceNode typeReference) {
            switch (typeReference.TypeReferenceKind) {
            case SimpleName: {
                var simpleName = (SimpleNameTypeReferenceNode)typeReference;
                return new ArrayList<String> { context.getIdentifier(simpleName.NameOffset, simpleName.NameLength) };
            }
            case Qualified: {
                var qualified = (QualifiedTypeReferenceNode)typeReference;
                var simpleName = qualified.SimpleName;
                var result = getName(context, qualified.EnclosingType);
                result.add(context.getIdentifier(simpleName.NameOffset, simpleName.NameLength));
                return result;
            }
            default:
                throw new RuntimeException("Internal error: unhandled type kind: " + typeReference.TypeReferenceKind);
            }
        }

        static String getFullName(CompilerContext context, TypeReferenceNode typeReference) {
            switch (typeReference.TypeReferenceKind) {
            case SimpleName: {
                var simpleName = (SimpleNameTypeReferenceNode)typeReference;
                return context.getIdentifier(simpleName.NameOffset, simpleName.NameLength);
            }
            case Qualified: {
                var qualified = (QualifiedTypeReferenceNode)typeReference;
                var simpleName = qualified.SimpleName;
                return getFullName(context, qualified.EnclosingType) + '/' +
                    context.getIdentifier(simpleName.NameOffset, simpleName.NameLength);
            }
            default:
                throw new RuntimeException("Internal error: unhandled type kind: " + typeReference.TypeReferenceKind);
            }
        }

        static TypeInfo resolveTypeReference(CompilerContext context, String packageName, TypeReferenceNode typeReference) {
            return resolveTypeReference(context, packageName, typeReference, true, true);
        }
    
        static TypeInfo resolveTypeReference(CompilerContext context, String packageName, TypeReferenceNode typeReference,
				bool fail, bool reportRawTypeUsage) {
            switch (typeReference.TypeReferenceKind) {
            case Boolean:
                return context.TypeSystem.BooleanType;
            case Byte:
                return context.TypeSystem.ByteType;
            case Char:
                return context.TypeSystem.CharType;
            case Double:
                return context.TypeSystem.DoubleType;
            case Float:
                return context.TypeSystem.FloatType;
            case Int:
                return context.TypeSystem.IntType;
            case Long:
                return context.TypeSystem.LongType;
            case Short:
                return context.TypeSystem.ShortType;
            case String:
                return context.TypeSystem.StringType;
            case Void:
                return context.TypeSystem.VoidType;
            case Wildcard:
                return context.TypeSystem.UnboundedWildcard;
            case LowerBoundedWildcard:
                return resolveTypeReference(context, packageName, ((WildcardTypeReferenceNode)typeReference).Bound).LowerBoundedWildcard;
            case UpperBoundedWildcard:
                return resolveTypeReference(context, packageName, ((WildcardTypeReferenceNode)typeReference).Bound).UpperBoundedWildcard;
            case Array:
                return resolveTypeReference(context, packageName, ((ArrayTypeReferenceNode)typeReference).ElementType).ArrayType;
            case SimpleName: {
                var simpleName = (SimpleNameTypeReferenceNode)typeReference;
                var name = context.getIdentifier(simpleName.NameOffset, simpleName.NameLength);
                var typeInfo = context.MemberResolver.resolveTypeName(packageName, name);
                if (typeInfo == null) {
                    if (fail) {
                        if (context.MemberResolver.isAmbiguousTypeName(name)) {
                            throw context.error(CompileErrorId.AmbiguousTypeName, simpleName, name);
                        } else {
                            throw context.error(CompileErrorId.UnresolvedTypeName, simpleName, name);
                        }
                    } else {
                        return null;
                    }
                }
                return getGenericType(context, packageName, typeInfo.Type, simpleName.TypeArguments, simpleName, fail, reportRawTypeUsage);
            }
            case Qualified: {
                var qualified = (QualifiedTypeReferenceNode)typeReference;
                var simpleName = qualified.SimpleName;
                var name = context.getIdentifier(simpleName.NameOffset, simpleName.NameLength);
                var enclosingType = resolveTypeReference(context, packageName, qualified.EnclosingType, false, false);
                if (enclosingType == null) {
                    var pkg = getFullName(context, qualified.EnclosingType);
                    if (pkg.indexOf('/') == -1) {
                        var s = context.MemberResolver.getPackageFromAlias(pkg);
                        if (s != null) {
                            pkg = s;
                        }
                    }
                    var fullName = pkg + '/' + name;
                    if (context.TypeSystem.typeExists(fullName)) {
                        var t = context.TypeSystem.getType(fullName);
                        if (!t.IsPublic && !t.PackageName.equals(packageName)) {
                            if (fail) {
                                throw context.error(CompileErrorId.UnresolvedTypeName, qualified, fullName.replace('/', '.').replace('$', '.'));
                            } else {
                                return null;
                            }
                        }
                        return getGenericType(context, packageName, t, simpleName.TypeArguments, simpleName, fail, reportRawTypeUsage);
                    } else {
                        if (fail) {
                            throw context.error(CompileErrorId.UnresolvedTypeName, qualified, fullName.replace('/', '.').replace('$', '.'));
                        } else {
                            return null;
                        }
                    }
                } else {
                    var fullName = enclosingType.FullName + '$' + name;
                    foreach (var t in enclosingType.NestedTypes) {
                        if (t.FullName.equals(fullName)) {
                            return getGenericType(context, packageName, t, simpleName.TypeArguments, simpleName, fail, reportRawTypeUsage);
                        }
                    }
                    if (fail) {
                        throw context.error(CompileErrorId.UnresolvedNestedTypeName, simpleName,
                            BytecodeHelper.getDisplayName(enclosingType) , name);
                    } else {
                        return null;
                    }
                }
            }
            default:
                throw new Exception("Internal error: unhandled type kind: " + typeReference.TypeReferenceKind);
            }
        }
        
        private static TypeInfo getGenericType(CompilerContext context, String packageName, TypeInfo type,
            List<TypeReferenceNode> typeArguments, SyntaxNode node, bool fail, bool reportRawTypeUsage) {
            int nTypeArgs = typeArguments.size();
            if (!type.IsGenericTypeDefinition) {
                if (nTypeArgs > 0) {
                    if (fail) {
                        throw context.error(CompileErrorId.TypeArgumentsNonGeneric, node, BytecodeHelper.getDisplayName(type));
                    } else {
                        return null;
                    }
                }
                return type;
            }
            int args = type.GenericArguments.count();
            if (args > 0 && nTypeArgs == 0) {
                var result = type.RawType;
				if (reportRawTypeUsage) {
					context.addWarning(CompileErrorId.RawTypeUsage, node,
						BytecodeHelper.getDisplayName(result), BytecodeHelper.getDisplayName(type));
				}
                return result;
            } else if (nTypeArgs != args) {
                if (fail) {
                    throw context.error(CompileErrorId.WrongTypeArgumentNumber, node, BytecodeHelper.getDisplayName(type), args);
                } else {
                    return null;
                }
            }
            if (nTypeArgs == 0) {
                return type;
            } else {
                var typeArgs = new ArrayList<TypeInfo>();
                foreach (var t in typeArguments) {
                    var ti = resolveTypeReference(context, packageName, t, fail, reportRawTypeUsage);
                    if (!fail && ti == null) {
                        return null;
                    }
                    if (ti.IsPrimitive) {
                        throw context.error(CompileErrorId.PrimitiveGenericArgument, node);
                    }
                    typeArgs.add(ti);
                }
                return context.TypeSystem.getGenericType(type, typeArgs);
            }
        }
        
        static void convertConstant(CompilerContext context, ExpressionNode expression, ExpressionInfo info, TypeInfo targetType) {
            switch (targetType.NumericTypeKind) {
            case Byte: {
                long value;
                switch (info.Type.TypeKind) {
                case Byte:
                    break;
                case Char:
                case Int:
                case Long:
                case Short:
                    value = ((Number)info.Value).longValue();
                    if (Byte.MIN_VALUE <= value && value <= Byte.MAX_VALUE) {
                        info.Type = context.TypeSystem.ByteType;
                        info.Value = Byte.valueOf((byte)value);
                        break;
                    }
                    goto default;
                default:
                    context.addError(CompileErrorId.NoImplicitConversion, expression,
                            BytecodeHelper.getDisplayName(info.Type),
                            BytecodeHelper.getDisplayName(targetType));
                    break;
                }
                break;
            }
            case Char: {
                long value;
                switch (info.Type.TypeKind) {
                case Char:
                    break;
                case Byte:
                case Int:
                case Long:
                case Short:
                    value = ((Number)info.getValue()).longValue();
                    if (Character.MIN_VALUE <= value && value <= Character.MAX_VALUE) {
                        info.Type = context.TypeSystem.CharType;
                        info.Value = Character.valueOf((char)value);
                        break;
                    }
                    goto default;
                default:
                    context.addError(CompileErrorId.NoImplicitConversion, expression,
                            BytecodeHelper.getDisplayName(info.Type),
                            BytecodeHelper.getDisplayName(targetType));
                    break;
                }
                break;
            }
            case Short: {
                long value;
                switch (info.Type.TypeKind) {
                case Short:
                    break;
                case Byte:
                case Char:
                case Int:
                case Long:
                    value = ((Number)info.Value).longValue();
                    if (Short.MIN_VALUE <= value && value <= Short.MAX_VALUE) {
                        info.Type = context.TypeSystem.ShortType;
                        info.Value = Short.valueOf((short)value);
                        break;
                    }
                    goto default;
                default:
                    context.addError(CompileErrorId.NoImplicitConversion, expression,
                            BytecodeHelper.getDisplayName(info.Type),
                            BytecodeHelper.getDisplayName(targetType));
                    break;
                }
                break;
            }
            case Int: {
                long value;
                switch (info.Type.TypeKind) {
                case Int:
                    break;
                case Byte:
                case Char:
                case Short:
                case Long:
                    value = ((Number)info.getValue()).longValue();
                    if (Integer.MIN_VALUE <= value && value <= Integer.MAX_VALUE) {
                        info.Type = context.TypeSystem.IntType;
                        info.Value = Integer.valueOf((int)value);
                        break;
                    }
                    goto default;
                default:
                    context.addError(CompileErrorId.NoImplicitConversion, expression,
                            BytecodeHelper.getDisplayName(info.Type),
                            BytecodeHelper.getDisplayName(targetType));
                    break;
                }
                break;
            }
            case Long: {
                long value;
                switch (info.Type.TypeKind) {
                case Long:
                    break;
                case Byte:
                case Char:
                case Short:
                case Int:
                    value = ((Number)info.getValue()).longValue();
                    info.Type = context.TypeSystem.LongType;
                    info.Value = Long.valueOf(value);
                    break;
                default:
                    context.addError(CompileErrorId.NoImplicitConversion, expression,
                            BytecodeHelper.getDisplayName(info.Type),
                            BytecodeHelper.getDisplayName(targetType));
                    break;
                }
                break;
            }
            case Float: {
                float value;
                switch (info.Type.TypeKind) {
                case Float:
                    break;
                case Byte:
                case Char:
                case Short:
                case Int:
                case Long:
                    value = ((Number)info.getValue()).floatValue();
                    info.Type = context.TypeSystem.FloatType;
                    info.Value = Float.valueOf(value);
                    break;
                default:
                    context.addError(CompileErrorId.NoImplicitConversion, expression,
                            BytecodeHelper.getDisplayName(info.Type),
                            BytecodeHelper.getDisplayName(targetType));
                    break;
                }
                break;
            }
            case Double: {
                double value;
                switch (info.Type.TypeKind) {
                case Double:
                    break;
                case Byte:
                case Char:
                case Short:
                case Int:
                case Long:
                case Float:
                    value = ((Number)info.Value).doubleValue();
                    info.Type = context.TypeSystem.DoubleType;
                    info.Value = Double.valueOf(value);
                    break;
                }
                break;
            }
            }
        }
        
        public static bool shouldIgnoreCalls(CompilerContext context, MethodInfo method) {
            foreach (var ann in BytecodeHelper.getAnnotations(context.AnnotatedTypeSystem, method)) {
                if (BytecodeHelper.isConditional(ann)) {
                    foreach (var elt in ann.getArgument("value").Elements) {
                        if (context.Symbols.contains(elt.Value)) {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        
        static void initializeAnonymousType(CompilerContext context, TypeInfo type) {
            if (!type.Methods.any(p => p.Name.equals("hashCode"))) {
            	var typeBuilder = (TypeBuilder)type;

                //
                // Constructor body
                //
                var baseConstructor = context.TypeSystem.ObjectType.getMethod("<init>", Query.empty<TypeInfo>());
	            var constructor = (MethodBuilder)type.Methods.where(p => p.Name.equals("<init>")).first();
                var gen = constructor.CodeGenerator;
                gen.beginScope();
                gen.emit(Opcode.Aload, gen.getLocal("this"));
                gen.emit(Opcode.Invokespecial, baseConstructor);
                var it = typeBuilder.Fields.iterator();
                foreach (var p in constructor.Parameters) {
                    gen.emit(Opcode.Aload, gen.getLocal("this"));
                    gen.emit(BytecodeHelper.getLoadOpcode(p.Type), gen.getLocal(p.Name));
                    gen.emit(Opcode.Putfield, it.next());
                }
                gen.emit(Opcode.Return);
                gen.endScope();

                //
                // Object.equals redefinition
                //
                var methodBuilder = typeBuilder.defineMethod("equals");
                methodBuilder.setReturnType(context.TypeSystem.BooleanType);
                methodBuilder.addParameter(context.TypeSystem.ObjectType).setName("obj");
                gen = methodBuilder.CodeGenerator;
                gen.beginScope();
                gen.emit(Opcode.Aload, gen.getLocal("obj"));
                gen.emit(Opcode.Instanceof, typeBuilder);
                var label = gen.defineLabel();
                gen.emit(Opcode.Ifne, label);
                gen.emit(Opcode.Iconst_0);
                gen.emit(Opcode.Ireturn);
                gen.markLabel(label);
                gen.emit(Opcode.Aload, gen.getLocal("obj"));
                gen.emit(Opcode.Checkcast, typeBuilder);
                gen.emit(Opcode.Astore, gen.declareLocal(typeBuilder, "other"));
                var equals = context.TypeSystem.ObjectType.getMethod("equals", Collections.singletonList(context.TypeSystem.ObjectType));
                foreach (var f in typeBuilder.Fields) {
                    gen.emit(Opcode.Aload, gen.getLocal("other"));
                    gen.emit(Opcode.Getfield, f);
                    gen.emit(Opcode.Aload, gen.getLocal("this"));
                    gen.emit(Opcode.Getfield, f);
                    label = gen.defineLabel();
                    switch (f.Type.TypeKind) {
                    case Boolean:
                    case Byte:
                    case Char:
                    case Short:
                    case Int:
                        gen.emit(Opcode.If_icmpeq, label);
                        break;
                    case Long:
                        gen.emit(Opcode.Lcmp);
                        gen.emit(Opcode.Ifeq, label);
                        break;
                    case Float:
                        gen.emit(Opcode.Fcmpg);
                        gen.emit(Opcode.Ifeq, label);
                        break;
                    case Double:
                        gen.emit(Opcode.Dcmpg);
                        gen.emit(Opcode.Ifeq, label);
                        break;
                    default:
                        gen.emit(Opcode.Invokevirtual, equals);
                        gen.emit(Opcode.Ifne, label);
                        break;
                    }
                    gen.emit(Opcode.Iconst_0);
                    gen.emit(Opcode.Ireturn);
                    gen.markLabel(label);
                }
                gen.emit(Opcode.Iconst_1);
                gen.emit(Opcode.Ireturn);
                gen.endScope();
            	
                //
                // Object.hashCode redefinition
                //
                methodBuilder = typeBuilder.defineMethod("hashCode");
                methodBuilder.setReturnType(context.TypeSystem.IntType);
                gen = methodBuilder.CodeGenerator;
                gen.beginScope();
                var hashCode = context.TypeSystem.ObjectType.getMethod("hashCode", Query.empty<TypeInfo>());
                gen.emit(Opcode.Iconst_0);
                gen.emit(Opcode.Istore, gen.declareLocal(context.TypeSystem.IntType, "result"));
                var first = true;
                foreach (var f in typeBuilder.Fields) {
                    gen.emit(Opcode.Aload, gen.getLocal("this"));
                    gen.emit(Opcode.Getfield, f);
                    switch (f.Type.TypeKind) {
                    case Boolean:
                    case Byte:
                    case Char:
                    case Short:
                    case Int:
                        break;
                    case Long:
                        gen.emit(Opcode.L2i);
                        break;
                    case Float:
                        gen.emit(Opcode.F2i);
                        break;
                    case Double:
                        gen.emit(Opcode.D2i);
                        break;
                    default:
                        gen.emit(Opcode.Invokevirtual, hashCode);
                        break;
                    }
                    if (first) {
                        first = false;
                    } else {
                        gen.emit(Opcode.Iload, gen.getLocal("result"));
                        gen.emit(Opcode.Ixor);
                    }
                    gen.emit(Opcode.Istore, gen.getLocal("result"));
                }
                gen.emit(Opcode.Iload, gen.getLocal("result"));
                gen.emit(Opcode.Ireturn);
                gen.endScope();
            }
        }
        
        static TypeInfo createDelegateType(CompilerContext context, TypeInfo delegateType, MethodInfo method) {
            var invokeMethod = delegateType.Methods.where(p => p.Name.equals("invoke")).first();
            
            var currentType = (TypeBuilder)context.CurrentType;
            var prefix = currentType.FullName + "$Delegate";
            int n = currentType.NestedTypes.count(p => p.FullName.startsWith(prefix));
            var typeBuilder = currentType.defineNestedType("Delegate" + n);
            context.TypeBuilders.add(typeBuilder);
            typeBuilder.setSynthetic(true);
            typeBuilder.setBaseType(delegateType);
            
            // invoke(...)
            var methodBuilder = typeBuilder.defineMethod("invoke");
            methodBuilder.setPublic(true);
            methodBuilder.setFinal(true);
            methodBuilder.setVarargs(invokeMethod.IsVarargs);
            methodBuilder.setReturnType(invokeMethod.ReturnType);
            foreach (var p in invokeMethod.Parameters) {
                var pb = methodBuilder.addParameter(p.Type);
                pb.setName(p.Name);
            }
            foreach (var t in invokeMethod.Exceptions) {
                methodBuilder.addException(t);
            }
            context.MethodGenerationContext.enterLambda(methodBuilder);
            
            var delegateTypeInfo = context.TypeSystem.getType("stab/lang/Delegate");
            
            var gen = methodBuilder.CodeGenerator;
            gen.beginScope();
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.Getfield, delegateTypeInfo.getField("delegates"));
            var elseLabel = gen.defineLabel();
            gen.emit(Opcode.Ifnull, elseLabel);
            
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.Getfield, delegateTypeInfo.getField("delegates"));
            var td = gen.declareLocal(delegateTypeInfo.ArrayType, "td$0");
            gen.emit(Opcode.Astore, td);
            LocalInfo local = null;
            if (method.ReturnType != context.TypeSystem.VoidType) {
                local = gen.declareLocal(method.ReturnType, "result$0");
            }
            var idx = gen.declareLocal(context.TypeSystem.IntType, "i$0");
            gen.emit(Opcode.Iconst_0);
            gen.emit(Opcode.Istore, idx);
            var startLabel = gen.defineLabel();
            gen.markLabel(startLabel);
            gen.emit(Opcode.Aload, td);
            gen.emit(Opcode.Iload, idx);
            gen.emit(Opcode.Aaload);
            gen.emit(Opcode.Checkcast, delegateType);
            foreach (var p in invokeMethod.Parameters) {
                gen.emit(BytecodeHelper.getLoadOpcode(p.Type), gen.getLocal(p.Name));
            }
            gen.emit(Opcode.Invokevirtual, invokeMethod);
            if (method.ReturnType != context.TypeSystem.VoidType) {
                var m = invokeMethod.OriginalMethodDefinition;
                BytecodeGenerator.emitGenericCast(context, invokeMethod.ReturnType, m.ReturnType);
                gen.emit(BytecodeHelper.getStoreOpcode(method.ReturnType), local);
            }
            gen.emit(Opcode.Iinc, idx, 1);
            gen.emit(Opcode.Iload, idx);
            gen.emit(Opcode.Aload, td);
            gen.emit(Opcode.Arraylength);
            gen.emit(Opcode.If_icmplt, startLabel);
            if (method.ReturnType != context.TypeSystem.VoidType) {
                gen.emit(BytecodeHelper.getLoadOpcode(method.ReturnType), local);
            }
            gen.emit(BytecodeHelper.getReturnOpcode(method.ReturnType));
            gen.markLabel(elseLabel);
            if (!method.IsStatic) {
                gen.emit(Opcode.Aload, gen.getLocal("this"));
                gen.emit(Opcode.Invokevirtual, delegateTypeInfo.getMethod("getTarget", Query.empty<TypeInfo>()));
                gen.emit(Opcode.Checkcast, method.DeclaringType);
            }
            foreach (var p in invokeMethod.Parameters) {
                gen.emit(BytecodeHelper.getLoadOpcode(p.Type), gen.getLocal(p.Name));
            }
            emitMethodAccess(context, method, false);
            gen.emit(BytecodeHelper.getReturnOpcode(method.ReturnType));
            gen.endScope();
            context.MethodGenerationContext.leaveLambda();

            if (invokeMethod.DeclaringType.GenericArguments.any()) {
                BytecodeGenerator.emitBridgeMethod(context, typeBuilder, methodBuilder, invokeMethod);
            }

            // getMethodCore()
            methodBuilder = typeBuilder.defineMethod("getMethodCore");
            methodBuilder.setProtected(true);
            methodBuilder.setFinal(true);
            methodBuilder.setReturnType(context.TypeSystem.getType("java/lang/reflect/Method"));

            gen = methodBuilder.CodeGenerator;
            gen.beginScope();
            var classType = context.TypeSystem.getType("java/lang/Class");
            BytecodeHelper.emitTypeof(gen, context.TypeSystem, method.DeclaringType);
            gen.emit(Opcode.Ldc, method.Name);
            BytecodeHelper.emitIntConstant(gen, invokeMethod.Parameters.count());
            BytecodeHelper.emitNewarray(gen, 1, classType);

            int i = 0;
            foreach (var p in invokeMethod.Parameters) {
                gen.emit(Opcode.Dup);
                BytecodeHelper.emitIntConstant(gen, i++);
                BytecodeHelper.emitTypeof(gen, context.TypeSystem, p.Type);
                gen.emit(Opcode.Aastore);
            }
            
            var argTypes = new ArrayList<TypeInfo>();
            argTypes.add(context.TypeSystem.StringType);
            argTypes.add(context.TypeSystem.getGenericType(classType, Collections.singletonList(context.TypeSystem.UnboundedWildcard)).ArrayType);
            gen.emit(Opcode.Invokevirtual, classType.getMethod("getMethod", argTypes));
            gen.emit(Opcode.Areturn);
            gen.endScope();
            
            // getMethodSignature()
            methodBuilder = typeBuilder.defineMethod("getMethodSignature");
            methodBuilder.setProtected(true);
            methodBuilder.setFinal(true);
            var objectType = context.TypeSystem.ObjectType;
            methodBuilder.setReturnType(objectType);

            gen = methodBuilder.CodeGenerator;
            gen.beginScope();
            gen.emit(Opcode.Ldc, method.DeclaringType.FullName + "." + method.Name + method.Descriptor);
            gen.emit(Opcode.Areturn);
            gen.endScope();
            
            // Constructor T(object)
            methodBuilder = typeBuilder.defineMethod("<init>");
            methodBuilder.setProtected(true);
            methodBuilder.setReturnType(context.TypeSystem.VoidType);
            if (!method.IsStatic) {
                var parameterBuilder = methodBuilder.addParameter(objectType);
                parameterBuilder.setName("target");
            }

            var baseConstructor = typeBuilder.BaseType.getMethod("<init>", Collections.singletonList(objectType));
            gen = methodBuilder.CodeGenerator;
            gen.beginScope();
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            if (method.IsStatic) {
                gen.emit(Opcode.Aconst_Null);
            } else {
                gen.emit(Opcode.Aload, gen.getLocal("target"));
            }
            gen.emit(Opcode.Invokespecial, baseConstructor);
            gen.emit(Opcode.Return);
            gen.endScope();
            
            return typeBuilder;
        }

		private static TypeBuilder getAccessorTypeBuilder(TypeInfo currentType, TypeInfo memberDeclaringType) {
			var outerClass = (TypeBuilder)currentType.OriginalTypeDefinition;
			memberDeclaringType = memberDeclaringType.OriginalTypeDefinition;
			while ((outerClass = (TypeBuilder)outerClass.DeclaringType) != null) {
                if (memberDeclaringType.isAssignableFrom(outerClass)) {
                    break;
                }
			}
			return outerClass;
		}
        
        static void emitMethodAccess(CompilerContext context, MethodInfo method, bool isSuperCall) {
            var generator = context.MethodGenerationContext.Generator;
            if (requireAccessor(context, method.DeclaringType, method.IsPublic, method.IsProtected, method.IsPrivate)) {
                var accessor = context.PrivateAccessors[method];
                if (accessor == null) {
                	var outerClass = getAccessorTypeBuilder(context.MethodGenerationContext.CurrentMethod.DeclaringType, method.DeclaringType);
                    int n = outerClass.Methods.count(p => p.Name.startsWith("access$"));
                    var methodBuilder = outerClass.defineMethod("access$" + n);
                    methodBuilder.setReturnType(method.ReturnType);
                    methodBuilder.setStatic(method.IsStatic);
                    methodBuilder.setFinal(!method.IsStatic);
                    methodBuilder.setSynthetic(true);
                    methodBuilder.setVarargs(method.IsVarargs);
                    foreach (var p in method.Parameters) {
                        var pb = methodBuilder.addParameter(p.Type);
                        pb.setName(p.Name);
                    }
                    foreach (var t in method.Exceptions) {
                        methodBuilder.addException(t);
                    }
                    var gen = methodBuilder.CodeGenerator;
                    gen.beginScope();
                    if (!method.IsStatic) {
                        gen.emit(Opcode.Aload, gen.getLocal("this"));
                    }
                    foreach (var p in method.Parameters) {
                        gen.emit(BytecodeHelper.getLoadOpcode(p.Type), gen.getLocal(p.Name));
                    }
                    if (method.IsStatic) {
                        gen.emit(Opcode.Invokestatic, method);
                    } else {
                        gen.emit(Opcode.Invokevirtual, method);
                    }
                    gen.emit(BytecodeHelper.getReturnOpcode(method.ReturnType));
                    gen.endScope();
                    accessor = methodBuilder;
                    context.PrivateAccessors[method] = accessor;
                }
                method = accessor;
            }
            if (method.IsStatic) {
                generator.emit(Opcode.Invokestatic, method);
            } else if (method.IsConstructor) {
                generator.emit(Opcode.Invokespecial, method);
            } else if (method.DeclaringType.IsInterface) {
                generator.emit(Opcode.Invokeinterface, method);
            } else {
                if (isSuperCall) {
                    generator.emit(Opcode.Invokespecial, method);
                } else {
                    generator.emit(Opcode.Invokevirtual, method);
                }
            }
        }
        
        static void emitFieldAccess(CompilerContext context, FieldInfo field) {
            var generator = context.MethodGenerationContext.Generator;
            if (requireAccessor(context, field.DeclaringType, field.IsPublic, field.IsProtected, field.IsPrivate)) {
                var accessor = context.PrivateAccessors[field];
                if (accessor == null) {
                	var outerClass = getAccessorTypeBuilder(context.MethodGenerationContext.CurrentMethod.DeclaringType, field.DeclaringType);
                    int n = outerClass.Methods.count(p => p.Name.startsWith("access$"));
                    var methodBuilder = outerClass.defineMethod("access$" + n);
                    methodBuilder.setReturnType(field.Type);
                    methodBuilder.setStatic(field.IsStatic);
                    methodBuilder.setFinal(!field.IsStatic);
                    methodBuilder.setSynthetic(true);
                    var gen = methodBuilder.CodeGenerator;
                    gen.beginScope();
                    if (field.IsStatic) {
                        gen.emit(Opcode.Getstatic, field);
                    } else {
                        gen.emit(Opcode.Aload, gen.getLocal("this"));
                        gen.emit(Opcode.Getfield, field);
                    }
                    gen.emit(BytecodeHelper.getReturnOpcode(field.Type));
                    gen.endScope();
                    accessor = methodBuilder;
                    context.PrivateAccessors[field] = accessor;
                }
                if (field.IsStatic) {
                    generator.emit(Opcode.Invokestatic, accessor);
                } else {
                    generator.emit(Opcode.Invokevirtual, accessor);
                }
            } else if (field.IsStatic) {
                generator.emit(Opcode.Getstatic, field);
            } else {
                generator.emit(Opcode.Getfield, field);
            }
        }

        static void emitPropertyAccess(CompilerContext context, MethodInfo method) {
            var generator = context.MethodGenerationContext.Generator;
            if (requireAccessor(context, method.DeclaringType, method.IsPublic, method.IsProtected, method.IsPrivate)) {
                var accessor = context.PrivateAccessors[method];
                if (accessor == null) {
                	var outerClass = getAccessorTypeBuilder(context.MethodGenerationContext.CurrentMethod.DeclaringType, method.DeclaringType);
                    int n = outerClass.Methods.count(p => p.Name.startsWith("access$"));
                    var type = method.getReturnType();
                    var methodBuilder = outerClass.defineMethod("access$" + n);
                    methodBuilder.setReturnType(type);
                    methodBuilder.setStatic(method.IsStatic);
                    methodBuilder.setFinal(!method.IsStatic);
                    methodBuilder.setSynthetic(true);
                    var gen = methodBuilder.CodeGenerator;
                    gen.beginScope();
                    if (method.IsStatic) {
                        gen.emit(Opcode.Invokestatic, method);
                    } else {
                        gen.emit(Opcode.Aload, gen.getLocal("this"));
                        gen.emit(Opcode.Invokevirtual, method);
                    }
                    gen.emit(BytecodeHelper.getReturnOpcode(type));
                    gen.endScope();
                    accessor = methodBuilder;
                    context.PrivateAccessors[method] = accessor;
                }
                if (method.IsStatic) {
                    generator.emit(Opcode.Invokestatic, accessor);
                } else {
                    generator.emit(Opcode.Invokevirtual, accessor);
                }
            } else {
                if (method.IsStatic) {
                    generator.emit(Opcode.Invokestatic, method);
                } else if (method.DeclaringType.IsInterface) {
                    generator.emit(Opcode.Invokeinterface, method);
                } else {
                    generator.emit(Opcode.Invokevirtual, method);
                }
            }
        }
        
        static void emitIndexerAccess(CompilerContext context, MethodInfo method) {
            var generator = context.MethodGenerationContext.Generator;
            if (requireAccessor(context, method.DeclaringType, method.IsPublic, method.IsProtected, method.IsPrivate)) {
                var accessor = context.PrivateAccessors[method];
                if (accessor == null) {
                	var outerClass = getAccessorTypeBuilder(context.MethodGenerationContext.CurrentMethod.DeclaringType, method.DeclaringType);
                    int n = outerClass.Methods.count(p => p.Name.startsWith("access$"));
                    var type = method.ReturnType;
                    var methodBuilder = outerClass.defineMethod("access$" + n);
                    methodBuilder.setReturnType(type);
                    methodBuilder.setStatic(method.IsStatic);
                    methodBuilder.setFinal(!method.IsStatic);
                    methodBuilder.setSynthetic(true);
                    foreach (var p in method.Parameters) {
                        var pb = methodBuilder.addParameter(p.Type);
                        pb.setName(p.Name);
                    }
                    var gen = methodBuilder.CodeGenerator;
                    gen.beginScope();
                    if (method.IsStatic) {
                        foreach (var p in method.Parameters) {
                            gen.emit(BytecodeHelper.getLoadOpcode(p.Type), gen.getLocal(p.Name));
                        }
                        gen.emit(Opcode.Invokestatic, method);
                    } else {
                        gen.emit(Opcode.Aload, gen.getLocal("this"));
                        foreach (var p in method.Parameters) {
                            gen.emit(BytecodeHelper.getLoadOpcode(p.Type), gen.getLocal(p.Name));
                        }
                        gen.emit(Opcode.Invokevirtual, method);
                    }
                    gen.emit(BytecodeHelper.getReturnOpcode(type));
                    gen.endScope();
                    accessor = methodBuilder;
                    context.PrivateAccessors[method] = accessor;
                }
                if (method.IsStatic) {
                    generator.emit(Opcode.Invokestatic, accessor);
                } else {
                    generator.emit(Opcode.Invokevirtual, accessor);
                }
            } else {
                if (method.IsStatic) {
                    generator.emit(Opcode.Invokestatic, method);
                } else if (method.DeclaringType.IsInterface) {
                    generator.emit(Opcode.Invokeinterface, method);
                } else {
                    generator.emit(Opcode.Invokevirtual, method);
                }
            }
        }

        static void emitFieldModification(CompilerContext context, FieldInfo field) {
            var generator = context.MethodGenerationContext.Generator;
            if (requireAccessor(context, field.DeclaringType, field.IsPublic, field.IsProtected, field.IsPrivate)) {
                var mutator = context.PrivateMutators[field];
                if (mutator == null) {
                	var outerClass = getAccessorTypeBuilder(context.MethodGenerationContext.CurrentMethod.DeclaringType, field.DeclaringType);
                    int n = outerClass.Methods.count(p => p.Name.startsWith("access$"));
                    var methodBuilder = outerClass.defineMethod("access$" + n);
                    methodBuilder.setReturnType(context.TypeSystem.VoidType);
                    methodBuilder.setStatic(field.IsStatic);
                    methodBuilder.setFinal(!field.IsStatic);
                    methodBuilder.setSynthetic(true);
                    var param = methodBuilder.addParameter(field.Type);
                    param.setName(field.Name);
                    var gen = methodBuilder.getCodeGenerator();
                    gen.beginScope();
                    if (field.IsStatic) {
                        gen.emit(BytecodeHelper.getLoadOpcode(field.Type), gen.getLocal(field.Name));
                        gen.emit(Opcode.Putstatic, field);
                    } else {
                        gen.emit(Opcode.Aload, gen.getLocal("this"));
                        gen.emit(BytecodeHelper.getLoadOpcode(field.Type), gen.getLocal(field.Name));
                        gen.emit(Opcode.Putfield, field);
                    }
                    gen.emit(Opcode.Return);
                    gen.endScope();
                    mutator = methodBuilder;
                    context.PrivateMutators[field] = mutator;
                }
                if (field.IsStatic) {
                    generator.emit(Opcode.Invokestatic, mutator);
                } else {
                    generator.emit(Opcode.Invokevirtual, mutator);
                }
            } else if (field.IsStatic) {
                generator.emit(Opcode.Putstatic, field);
            } else {
                generator.emit(Opcode.Putfield, field);
            }
        }
        
        static void emitPropertyOrIndexerModification(CompilerContext context, MethodInfo method) {
            var generator = context.MethodGenerationContext.Generator;
            if (requireAccessor(context, method.DeclaringType, method.IsPublic, method.IsProtected, method.IsPrivate)) {
                var mutator = context.PrivateMutators[method];
                if (mutator == null) {
                    var outerClass = (TypeBuilder)context.MethodGenerationContext.CurrentMethod.DeclaringType;
                    var methodDeclaringType = method.DeclaringType.OriginalTypeDefinition;
                    while ((outerClass = (TypeBuilder)outerClass.DeclaringType) != null) {
                        if (methodDeclaringType.isAssignableFrom(outerClass.OriginalTypeDefinition)) {
                            break;
                        }
                    }
                    int n = outerClass.Methods.count(p => p.Name.startsWith("access$"));
                    var methodBuilder = outerClass.defineMethod("access$" + n);
                    methodBuilder.setReturnType(context.TypeSystem.VoidType);
                    methodBuilder.setStatic(method.IsStatic);
                    methodBuilder.setFinal(!method.IsStatic);
                    methodBuilder.setSynthetic(true);
                    foreach (var p in method.Parameters) {
                        var param = methodBuilder.addParameter(p.Type);
                        param.setName(p.Name);
                    }
                    var gen = methodBuilder.CodeGenerator;
                    gen.beginScope();
                    if (method.IsStatic) {
                        foreach (var p in method.Parameters) {
                            gen.emit(BytecodeHelper.getLoadOpcode(p.Type), gen.getLocal(p.Name));
                        }
                        gen.emit(Opcode.Invokestatic, method);
                    } else {
                        gen.emit(Opcode.Aload, gen.getLocal("this"));
                        foreach (var p in method.Parameters) {
                            gen.emit(BytecodeHelper.getLoadOpcode(p.Type), gen.getLocal(p.Name));
                        }
                        gen.emit(Opcode.Invokevirtual, method);
                    }
                    gen.emit(Opcode.Return);
                    gen.endScope();
                    mutator = methodBuilder;
                    context.PrivateMutators[method] = mutator;
                }
                if (method.IsStatic) {
                    generator.emit(Opcode.Invokestatic, mutator);
                } else {
                    generator.emit(Opcode.Invokevirtual, mutator);
                }
            } else if (method.IsStatic) {
                generator.emit(Opcode.Invokestatic, method);
            } else if (method.DeclaringType.IsInterface) {
                generator.emit(Opcode.Invokeinterface, method);
            } else {
                generator.emit(Opcode.Invokevirtual, method);
            }
        }
        
        static bool requireAccessor(CompilerContext context, TypeInfo declaringType,
                bool isPublic, bool isProtected, bool isPrivate) {
            var currentType = context.MethodGenerationContext.CurrentMethod.DeclaringType;
            if (isPublic || declaringType.OriginalTypeDefinition == currentType.OriginalTypeDefinition) {
                return false;
            }
            if (isProtected && declaringType.isAssignableFrom(currentType)) {
                return false;
            }
            if (!isPrivate && declaringType.PackageName.equals(currentType.PackageName)) {
                return false;
            }
            return true;
        }
    }
}
