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
using java.lang.annotation;
using java.util;
using stab.query;
using stab.reflection;
using cnatural.helpers;
using cnatural.syntaxtree;

namespace cnatural.compiler {

    class BytecodeGenerator : StatementHandler<Void, Void> {
        private CompilerContext context;
        private ExpressionGenerator expressionGenerator;
        private LabelRemover labelRemover;
        
        BytecodeGenerator(CompilerContext context)
            : super(true) {
            this.context = context;
            this.expressionGenerator = new ExpressionGenerator(this, context);
            this.labelRemover = new LabelRemover();
        }

        static void emitBoxing(CompilerContext context, ExpressionNode expression) {
            var generator = context.MethodGenerationContext.Generator;
            var info = expression.getUserData(typeof(ExpressionInfo));
            switch (info.BoxingKind) {
            case Box:
                generator.emit(Opcode.Invokestatic, info.BoxingMethod);
                break;
            case Unbox:
                generator.emit(Opcode.Invokevirtual, info.BoxingMethod);
                break;
            }
        }

        void generateConstructorsBytecode() {
            foreach (var methodBuilder in context.ConstructorBuilders) {
                var baseConstructor = methodBuilder.DeclaringType.BaseType.getMethod("<init>",
                        (methodBuilder.DeclaringType.IsEnum)
                            ? new ArrayList<TypeInfo> { context.TypeSystem.StringType, context.TypeSystem.IntType }
                            : Query.empty<TypeInfo>());
                context.MethodGenerationContext.initialize(getMethodBuilder(methodBuilder, null), context.LambdaScopes[methodBuilder]);
                var generator = context.MethodGenerationContext.Generator;
                generator.beginScope();
                generator.emit(Opcode.Aload, generator.getLocal("this"));
                if (methodBuilder.DeclaringType.IsEnum) {
                    generator.emit(Opcode.Aload, generator.getLocal("name$0"));
                    generator.emit(Opcode.Iload, generator.getLocal("ordinal$0"));
                }
                generator.emit(Opcode.Invokespecial, baseConstructor);
                initializeLambdaScope(methodBuilder);
                foreach (var c in methodBuilder.DeclaringType.getAllUserData(typeof(ClassDeclarationNode))) {
                    initializeFields(generator, c);
                }
                generator.emit(Opcode.Return);
                generator.emptyScope();
                context.MethodGenerationContext.Iterable = null;
            }
            
            foreach (var typeBuilder in context.TypeBuilders.toList()) {
                if (typeBuilder.IsInterface) {
                    continue;
                }
                context.CurrentType = typeBuilder;
                var methodBuilder = (MethodBuilder)typeBuilder.getMethod("<clinit>", Query.empty<TypeInfo>());
                if (methodBuilder != null) {
                    context.MethodGenerationContext.initialize(getMethodBuilder(methodBuilder, null), context.LambdaScopes[methodBuilder]);
                    var generator = context.MethodGenerationContext.Generator;
                    generator.beginScope();
                    initializeLambdaScope(methodBuilder);
                    foreach (var c in typeBuilder.getAllUserData(typeof(ClassDeclarationNode))) {
                        initializeStaticFields(generator, c);
                    }
                    generator.emit(Opcode.Return);
                    generator.endScope();
                    context.MethodGenerationContext.Iterable = null;
                }
            }
        }
        
        void generateMethodBytecode(MethodDeclarationNode methodDeclaration) {
            var methodBuilder = methodDeclaration.getUserData(typeof(MethodBuilder));
            if (methodDeclaration.Body != null) {
                context.MethodGenerationContext.initialize(getMethodBuilder(methodBuilder, methodDeclaration.Body),
                    context.LambdaScopes[methodBuilder]);
                var generator = context.MethodGenerationContext.Generator;
                generator.beginScope();
                initializeLambdaScope(methodBuilder);
                handleStatement(methodDeclaration.Body, null);
                generator.emptyScope();
                context.MethodGenerationContext.Iterable = null;
            }
            if (!methodBuilder.IsExcludedFromCompilation) {
                var member = MemberInfo.getInfo(methodBuilder);
                emitBridgeMethod((TypeBuilder)methodBuilder.DeclaringType, methodBuilder,
                    member.getOverridenMembers(context.AnnotatedTypeSystem).select(p => p.Method));
            }
        }
        
        void generateFieldBytecode(FieldDeclarationNode fieldDeclaration) {
            foreach (var decl in fieldDeclaration.Declarators) {
                if (decl.Value == null) {
                    continue;
                }
                var fieldBuilder = decl.getUserData(typeof(FieldBuilder));
                if (fieldBuilder.IsStatic) {
                    var info = decl.Value.getUserData(typeof(ExpressionInfo));
                    if (info.IsConstant && info.BoxingKind == BoxingKind.None) {
                        var value = info.Value;
                        switch (fieldBuilder.Type.TypeKind) {
                        case Long:
                            long l = 0;
                            if (value instanceof Long) {
                                l = ((Long)value).longValue();
                            } else if (value instanceof Integer) {
                                l = ((Integer)value).intValue();
                            } else if (value instanceof Character) {
                                l = ((Character)value).charValue();
                            } else if (value instanceof Byte) {
                                l = ((Byte)value).byteValue();
                            } else if (value instanceof Short) {
                                l = ((Short)value).shortValue();
                            }
                            value = Long.valueOf(l);
                            break;
                        case Float:
                            float f = 0;
                            if (value instanceof Float) {
                                f = ((Float)value).floatValue();
                            } else if (value instanceof Long) {
                                f = ((Long)value).longValue();
                            } else if (value instanceof Integer) {
                                f = ((Integer)value).intValue();
                            } else if (value instanceof Character) {
                                f = ((Character)value).charValue();
                            } else if (value instanceof Byte) {
                                f = ((Byte)value).byteValue();
                            } else if (value instanceof Short) {
                                f = ((Short)value).shortValue();
                            }
                            value = Float.valueOf(f);
                            break;
                        case Double:
                            double d = 0;
                            if (value instanceof Double) {
                                d = ((Double)value).doubleValue();
                            } else if (value instanceof Float) {
                                d = ((Float)value).floatValue();
                            } else if (value instanceof Long) {
                                d = ((Long)value).longValue();
                            } else if (value instanceof Integer) {
                                d = ((Integer)value).intValue();
                            } else if (value instanceof Character) {
                                d = ((Character)value).charValue();
                            } else if (value instanceof Byte) {
                                d = ((Byte)value).byteValue();
                            } else if (value instanceof Short) {
                                d = ((Short)value).shortValue();
                            }
                            value = Double.valueOf(d);
                            break;
                        }
                        fieldBuilder.setValue(value);
                    }
                }
            }
        }

        void generateConstructorBytecode(ConstructorDeclarationNode constructorDeclaration) {
            if (constructorDeclaration.Modifiers.contains(Modifier.Static)) {
                return;
            }
            var methodBuilder = constructorDeclaration.getUserData(typeof(MethodBuilder));
            context.MethodGenerationContext.initialize(getMethodBuilder(methodBuilder, constructorDeclaration.Body),
                context.LambdaScopes[methodBuilder]);
            var generator = context.MethodGenerationContext.Generator;
            generator.beginScope();
            var initializer = constructorDeclaration.getInitializer();
            generator.emit(Opcode.Aload, generator.getLocal("this"));
            var initFields = true;
            if (initializer != null) {
                var iinfo = initializer.getUserData(typeof(ExpressionInfo));
                var method = iinfo.Method;
                expressionGenerator.emitArguments(initializer.Arguments, method.Parameters, method.Parameters.count(), method.IsVarargs);
                generator.emit(Opcode.Invokespecial, method);
                initFields = initializer.IsSuper;
            } else {
                var iinfo = constructorDeclaration.getUserData(typeof(ExpressionInfo));
                if (methodBuilder.DeclaringType.IsEnum) {
                    generator.emit(Opcode.Aload, generator.getLocal("name$0"));
                    generator.emit(Opcode.Iload, generator.getLocal("ordinal$0"));
                }
                generator.emit(Opcode.Invokespecial, iinfo.Method);
            }
            initializeLambdaScope(methodBuilder);
            if (initFields) {
                // Partial classes can have more than one declaration
                foreach (var c in methodBuilder.DeclaringType.getAllUserData(typeof(ClassDeclarationNode))) {
                    initializeFields(generator, c);
                }
            }
            handleStatement(constructorDeclaration.Body, null);
            generator.emptyScope();
            context.MethodGenerationContext.Iterable = null;
        }

        void generateDestructorBytecode(DestructorDeclarationNode destructorDeclaration) {
            var methodBuilder = destructorDeclaration.getUserData(typeof(MethodBuilder));
            context.MethodGenerationContext.initialize(getMethodBuilder(methodBuilder, destructorDeclaration.Body),
                context.LambdaScopes[methodBuilder]);
            var generator = context.MethodGenerationContext.Generator;
            generator.beginScope();
            initializeLambdaScope(methodBuilder);
            
            var startLabel = generator.defineLabel();
            generator.markLabel(startLabel);
            handleStatement(destructorDeclaration.Body, null);
            LabelMarker endBodyLabel = generator.defineLabel();
            generator.markLabel(endBodyLabel);
            var fallThrough = destructorDeclaration.Body.getUserData(typeof(StatementInfo)).IsEndPointReachable;
            var parentFinalize = methodBuilder.DeclaringType.BaseType.getMethod("finalize", Query.empty<TypeInfo>());
            
            LabelMarker endLabel = null;
            if (fallThrough) {
                endLabel = generator.defineLabel();
                generator.emit(Opcode.Aload, generator.getLocal("this"));
                generator.emit(Opcode.Invokespecial, parentFinalize);
                generator.emit(Opcode.Goto, endLabel);
            }
            var targetLabel = generator.defineLabel();
            generator.markLabel(targetLabel);
            generator.emit(Opcode.Aload, generator.getLocal("this"));
            generator.emit(Opcode.Invokespecial, parentFinalize);
            generator.emit(Opcode.Athrow);
            generator.exceptionHandler(startLabel, endBodyLabel, targetLabel, null);
            if (endLabel != null) {
                generator.markLabel(endLabel);
                generator.emit(Opcode.Return);
            }
            generator.emptyScope();
            context.MethodGenerationContext.Iterable = null;
        }

        void generateIndexerBytecode(IndexerDeclarationNode indexerDeclaration) {
            var get = indexerDeclaration.GetAccessor;
            var set = indexerDeclaration.SetAccessor;
            var getBuilder = (get == null) ? null : get.getUserData(typeof(MethodBuilder));
            var setBuilder = (set == null) ? null : set.getUserData(typeof(MethodBuilder));
            
            var memberInfo = indexerDeclaration.getUserData(typeof(MemberInfo));
            var overriden = memberInfo.getOverridenMembers(context.getAnnotatedTypeSystem());
            var typeBuilder = (TypeBuilder)(getBuilder ?? setBuilder).DeclaringType;
            
            if (get != null) {
                var methodBuilder = getBuilder;
                context.MethodGenerationContext.initialize(getMethodBuilder(methodBuilder, get.Body), context.LambdaScopes[methodBuilder]);
                var generator = context.MethodGenerationContext.Generator;
                generator.beginScope();
                initializeLambdaScope(methodBuilder);
                handleStatement(get.Body, null);
                generator.emptyScope();
                context.MethodGenerationContext.Iterable = null;
                
                foreach (var mi in overriden.select(p => p.GetAccessor)) {
                    if (mi != null && !mi.Name.equals(methodBuilder.Name)) {
                        emitBridgeMethod(context, typeBuilder, methodBuilder, mi);
                    }
                }
                emitBridgeMethod(typeBuilder, methodBuilder, overriden.select(p => p.GetAccessor));
            }
            if (set != null) {
                var methodBuilder = setBuilder;
                context.MethodGenerationContext.initialize(getMethodBuilder(methodBuilder, set.Body), context.LambdaScopes[methodBuilder]);
                var generator = context.MethodGenerationContext.Generator;
                generator.beginScope();
                initializeLambdaScope(methodBuilder);
                handleStatement(set.Body, null);
                generator.emptyScope();
                context.MethodGenerationContext.Iterable = null;
                
                foreach (var mi in overriden.select(p => p.SetAccessor)) {
                    if (mi != null && !mi.Name.equals(methodBuilder.Name)) {
                        emitBridgeMethod(context, typeBuilder, methodBuilder, mi);
                    }
                }
                emitBridgeMethod(typeBuilder, methodBuilder, overriden.select(p => p.SetAccessor));
            }
        }

        void generatePropertyBytecode(PropertyDeclarationNode propertyDeclaration) {
            var get = propertyDeclaration.GetAccessor;
            var set = propertyDeclaration.SetAccessor;
            var getBuilder = (get == null) ? null : get.getUserData(typeof(MethodBuilder));
            var setBuilder = (set == null) ? null : set.getUserData(typeof(MethodBuilder));
            FieldBuilder fieldBuilder = null;
            var memberInfo = propertyDeclaration.getUserData(typeof(MemberInfo));
            var overriden = memberInfo.getOverridenMembers(context.AnnotatedTypeSystem);
            var typeBuilder = (TypeBuilder)(getBuilder ?? setBuilder).DeclaringType;
            
            if (get != null) {
                var methodBuilder = getBuilder;
                if (!methodBuilder.IsAbstract) {
                    context.MethodGenerationContext.initialize(getMethodBuilder(methodBuilder, get.Body), context.LambdaScopes[methodBuilder]);
                    var generator = context.MethodGenerationContext.Generator;
                    generator.beginScope();
                    initializeLambdaScope(methodBuilder);
                    if (get.Body == null) {
                        fieldBuilder = typeBuilder.defineField("property$" + memberInfo.Name, methodBuilder.ReturnType);
                        fieldBuilder.setSynthetic(true);
                        fieldBuilder.setPrivate(true);
                        fieldBuilder.setStatic(methodBuilder.IsStatic);
                        
                        if (fieldBuilder.IsStatic) {
                            generator.emit(Opcode.Getstatic, fieldBuilder);
                        } else {
                            generator.emit(Opcode.Aload, generator.getLocal("this"));
                            generator.emit(Opcode.Getfield, fieldBuilder);
                        }
                        generator.emit(BytecodeHelper.getReturnOpcode(fieldBuilder.Type));
                    } else {
                        handleStatement(get.Body, null);
                    }
                    generator.emptyScope();
                    context.MethodGenerationContext.Iterable = null;
                }

                foreach (var mi in overriden.select(p => p.GetAccessor)) {
                    if (mi != null && !mi.Name.equals(methodBuilder.Name)) {
                        emitBridgeMethod(context, typeBuilder, methodBuilder, mi);
                    }
                }
                emitBridgeMethod(typeBuilder, methodBuilder, overriden.select(p => p.GetAccessor));
            }
            if (set != null) {
                var methodBuilder = setBuilder;
                if (!methodBuilder.IsAbstract) {
                    context.MethodGenerationContext.initialize(getMethodBuilder(methodBuilder, set.Body), context.LambdaScopes[methodBuilder]);
                    var generator = context.MethodGenerationContext.Generator;
                    generator.beginScope();
                    initializeLambdaScope(methodBuilder);
                    if (set.Body == null) {
                        if (!fieldBuilder.IsStatic) {
                            generator.emit(Opcode.Aload, generator.getLocal("this"));
                        }
                        // Can be null if the property is automatically implemented
                        var local = methodBuilder.getUserData(typeof(LocalMemberInfo));
                        if (local != null && local.IsUsedFromLambda) {
                            var type = fieldBuilder.Type;
                            emitLoadLambdaScope(context, generator, local.Method);
                            generator.emit(BytecodeHelper.getLoadOpcode(type), getLambdaScopeField(context, local));
                        } else {
                            generator.emit(BytecodeHelper.getLoadOpcode(fieldBuilder.Type), generator.getLocal("value"));
                        }
                        if (fieldBuilder.IsStatic) {
                            generator.emit(Opcode.Putstatic, fieldBuilder);
                        } else {
                            generator.emit(Opcode.Putfield, fieldBuilder);
                        }
                        generator.emit(Opcode.Return);
                    } else {
                        handleStatement(set.Body, null);
                    }
                    generator.emptyScope();
                    context.MethodGenerationContext.Iterable = null;
                }

                foreach (var mi in overriden.select(p => p.SetAccessor)) {
                    if (mi != null && !mi.Name.equals(methodBuilder.Name)) {
                        emitBridgeMethod(context, typeBuilder, methodBuilder, mi);
                    }
                }
                emitBridgeMethod(typeBuilder, methodBuilder, overriden.select(p => p.SetAccessor));
            }
        }

        void generateInterfaceMethodAnnotations(MethodDeclarationNode methodDeclaration) {
            var methodBuilder = methodDeclaration.getUserData(typeof(MethodBuilder));
            generateAnnotationsBytecode(methodDeclaration.Annotations, methodBuilder);
            if (methodDeclaration.DefaultValue != null) {
                var annotationValueBuilder = new AnnotationValueBuilder(methodBuilder.ReturnType, false);
                methodBuilder.setDefaultValue(annotationValueBuilder);
                generateAnnotationArgument("", methodDeclaration.DefaultValue, annotationValueBuilder);
            }
        }
        
        void emitAccessorAnnotations(AccessorDeclarationNode accessor, List<AnnotationSectionNode> commonAnnotations) {
            var builder = (accessor == null) ? null : accessor.getUserData(typeof(MethodBuilder));
            if (builder != null) {
                generateAnnotationsBytecode(commonAnnotations, builder);
                generateAnnotationsBytecode(accessor.Annotations, builder);
            }
        }
        
        void generateDelegateBytecode(DelegateDeclarationNode delegateDeclaration) {
            var typeBuilder = delegateDeclaration.getUserData(typeof(TypeBuilder));
            typeBuilder.setSourceFile(PathHelper.getFileName(delegateDeclaration.Filename));
            context.CurrentType = typeBuilder;
            var parameterTypes = Collections.singletonList(context.TypeSystem.ObjectType);
            var methodBuilder = (MethodBuilder)typeBuilder.getMethod("<init>", parameterTypes);

            var baseConstructor = typeBuilder.getBaseType().getMethod("<init>", parameterTypes);
            var gen = methodBuilder.CodeGenerator;
            gen.beginScope();
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.Aload, gen.getLocal("target"));
            gen.emit(Opcode.Invokespecial, baseConstructor);
            gen.emit(Opcode.Return);
            gen.endScope();
        }
        
        void generateImplicitEnumMembersBytecode(TypeBuilder typeBuilder) {
            //
            // valueOf
            //
            var methodBuilder = (MethodBuilder)typeBuilder.getMethod("valueOf",
                    Collections.singletonList(context.TypeSystem.StringType));
            var generator = methodBuilder.CodeGenerator;
            generator.beginScope();
            generator.emit(Opcode.Ldc, typeBuilder);
            generator.emit(Opcode.Aload, generator.getLocal("str"));
            MethodInfo meth = typeBuilder.BaseType.Methods.where(p => p.Name.equals("valueOf")).first();
            generator.emit(Opcode.Invokestatic, meth);
            generator.emit(Opcode.Checkcast, typeBuilder);
            generator.emit(Opcode.Areturn);
            generator.endScope();

            //
            // values
            //
            methodBuilder = (MethodBuilder)typeBuilder.getMethod("values", Query.empty<TypeInfo>());
            generator = methodBuilder.CodeGenerator;
            generator.beginScope();
            generator.emit(Opcode.Getstatic, typeBuilder.getField("ENUM$VALUES"));
            generator.emit(Opcode.Dup);
            LocalInfo local0 = generator.declareLocal(typeBuilder.ArrayType, "local0");
            generator.emit(Opcode.Astore, local0);
            generator.emit(Opcode.Iconst_0);
            generator.emit(Opcode.Aload, local0);
            generator.emit(Opcode.Arraylength);
            generator.emit(Opcode.Dup);
            LocalInfo local1 = generator.declareLocal(context.TypeSystem.IntType, "local1");
            generator.emit(Opcode.Istore, local1);
            generator.emit(Opcode.Anewarray, typeBuilder);
            generator.emit(Opcode.Dup);
            LocalInfo local2 = generator.declareLocal(typeBuilder.ArrayType, "local2");
            generator.emit(Opcode.Astore, local2);
            generator.emit(Opcode.Iconst_0);
            generator.emit(Opcode.Iload, local1);
            generator.emit(Opcode.Invokestatic, context.SystemArraycopyMethod);
            generator.emit(Opcode.Aload, local2);
            generator.emit(Opcode.Areturn);
            generator.endScope();
        }
        
        void emitBridgeMethod(TypeBuilder typeBuilder, MethodInfo bridgedMethod, Iterable<MethodInfo> overridenMethods) {
        	var methods = new HashSet<MethodInfo>();
        	var returnTypes = new HashSet<TypeInfo>();
            foreach (var method in overridenMethods) {
                if (method == null) {
                	continue;
                }
                if (!requireBridge(context, bridgedMethod, method)) {
                	continue;
                }
                if (!isBridgeDefined(context, method.DeclaringType, method) && returnTypes.add(method.ReturnType)) {
                	methods.add(method);
                }
            }
            foreach (var method in methods) {
            	emitBridgeMethod(context, typeBuilder, bridgedMethod, method);
            }
        }
        
        private static bool requireBridge(CompilerContext context, MethodInfo bridgedMethod, MethodInfo method) {
            var def = method.OriginalMethodDefinition;
            var pit = bridgedMethod.Parameters.iterator();
            foreach (var p in def.Parameters) {
                var mp = pit.next();
            	if (getTypeErasure(context, p.Type).OriginalTypeDefinition != getTypeErasure(context, mp.Type).OriginalTypeDefinition) {
            		return true;
            	}
            }
        	return getTypeErasure(context, bridgedMethod.ReturnType).OriginalTypeDefinition != getTypeErasure(context, def.ReturnType).OriginalTypeDefinition;
        }

		private static bool isBridgeDefined(CompilerContext context, TypeInfo type, MethodInfo method) {
            method = method.OriginalMethodDefinition;
        	var pcount = method.Parameters.count();
            foreach (var m in type.Methods) {
                if (m == method 
                        || !m.Name.equals(method.Name)
                        || m.IsVarargs != method.IsVarargs
                        || m.IsPublic != method.IsPublic
                        || m.Parameters.count() != pcount
                        || getTypeErasure(context, m.ReturnType) != getTypeErasure(context, method.ReturnType)) {
                    continue;
                }
            	var compatibleParams = true;
                var pit = method.Parameters.iterator();
                foreach (var p in m.Parameters) {
                    var mp = pit.next();
                    if (getTypeErasure(context, p.Type) != getTypeErasure(context, mp.Type)) {
                        compatibleParams = false;
                        break;
                    }
                }
                if (compatibleParams) {
                	return true;
                }
            }
			return false;
		}
        
        private static TypeInfo getTypeErasure(CompilerContext context, TypeInfo type) {
        	if (type.IsGenericParameter) {
        		return BytecodeHelper.getGenericParameterTypeErasure(context.TypeSystem, type);
        	} else if (type.IsArray) {
        		return getTypeErasure(context, type.ElementType).ArrayType;
        	} else if (type.GenericArguments.any()) {
        		return context.TypeSystem.getGenericType(type.OriginalTypeDefinition,
        				type.GenericArguments.select(p => getTypeErasure(context, p)).toList());
        	} else {
        		return type;
        	}
        }
        
        static void emitBridgeMethod(CompilerContext context, TypeBuilder typeBuilder, MethodInfo bridgedMethod,
                MethodInfo overloadedMethod) {
            if (isBridgeDefined(context, typeBuilder, overloadedMethod)) {
            	return;
            }
            overloadedMethod = overloadedMethod.OriginalMethodDefinition;

            var methodBuilder = typeBuilder.defineMethod(overloadedMethod.Name);
            methodBuilder.setBridge(true);
            methodBuilder.setPublic(true);
            methodBuilder.setFinal(true);
            methodBuilder.setVarargs(overloadedMethod.IsVarargs);
            methodBuilder.setReturnType(getTypeErasure(context, overloadedMethod.ReturnType));
            var pit = bridgedMethod.Parameters.iterator();
            foreach (var p in overloadedMethod.Parameters) {
                ParameterBuilder pb = methodBuilder.addParameter(getTypeErasure(context, p.Type));
                pb.setName(pit.next().Name);
            }
            foreach (var t in overloadedMethod.Exceptions) {
                methodBuilder.addException(t);
            }

            var gen = methodBuilder.CodeGenerator;
            gen.beginScope();

            gen.emit(Opcode.Aload, gen.getLocal("this"));
            var iit = overloadedMethod.Parameters.iterator();
            foreach (var p in bridgedMethod.Parameters) {
                gen.emit(BytecodeHelper.getLoadOpcode(p.Type), gen.getLocal(p.Name));
                if (iit.next().Type.IsGenericParameter) {
                    gen.emit(Opcode.Checkcast, p.Type);
                }
            }
            gen.emit(Opcode.Invokevirtual, bridgedMethod);
            gen.emit(BytecodeHelper.getReturnOpcode(overloadedMethod.ReturnType));
            gen.endScope();
        }
        
        protected override Void handleBlock(BlockStatementNode block, Void source) {
            var info = block.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            emitBeginLabel(info);
            var generator = context.MethodGenerationContext.Generator;
            
            generator.beginScope();
            foreach (var statement in block.Statements) {
                handleStatement(statement, null);
            }
            generator.endScope();
            return null;
        }

        protected override Void handleBreak(BreakStatementNode breakStatement, Void source) {
            var info = breakStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            emitBeginLabel(info);
            var generator = context.MethodGenerationContext.Generator;
            if (info.Target == null) {
                var scope = breakStatement.getUserData(typeof(StatementScope));
                var done = false;
                while (!done && scope != null) {
                    var node = scope.statement;
                    switch (node.StatementKind) {
                    case Do:
                    case For:
                    case Foreach:
                    case Switch:
                    case While:
                        emitEnclosingFinally(breakStatement, node);
                        var ninfo = node.getUserData(typeof(StatementInfo));
                        if (ninfo.EndLabel == null) {
                            ninfo.EndLabel = generator.defineLabel();
                        }
                        generator.emit(Opcode.Goto, ninfo.EndLabel);
                        done = true;
                        break;
                    }
                    scope = scope.next;
                }
            } else {
                var tinfo = info.Target.getUserData(typeof(StatementInfo));
                if (tinfo.BeginLabel == null) {
                    tinfo.BeginLabel = generator.defineLabel();
                }
                emitEnclosingFinally(breakStatement, info.Target);
                generator.emit(Opcode.Goto, tinfo.BeginLabel);
            }
            return null;
        }
        
        protected override Void handleContinue(ContinueStatementNode continueStatement, Void source) {
            var info = continueStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            emitBeginLabel(info);
            var generator = context.MethodGenerationContext.Generator;
            emitEnclosingFinally(continueStatement, info.Target);
            var tinfo = info.Target.getUserData(typeof(StatementInfo));
            generator.emit(Opcode.Goto, tinfo.ContinueLabel);
            return null;
        }

        protected override Void handleDo(DoStatementNode doStatement, Void source) {
            var info = doStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            if (info.BeginLabel == null) {
                info.BeginLabel = generator.defineLabel();
            }
            var startLabel = info.BeginLabel;
            var conditionLabel = generator.defineLabel();
            info.ContinueLabel = conditionLabel;
            var cinfo = doStatement.Condition.getUserData(typeof(ExpressionInfo));
            cinfo.Negate = false;
            if (cinfo.IsConstant) {
                if (((Boolean)cinfo.Value).booleanValue()) {
                    generator.markLabel(startLabel);
                    generator.beginScope();
                    handleStatement(doStatement.Statement, source);
                    generator.endScope();
                    generator.markLabel(conditionLabel);
                    generator.emit(Opcode.Goto, startLabel);
                } else {
                    generator.beginScope();
                    handleStatement(doStatement.Statement, source);
                    generator.endScope();
                }
            } else {
                if (info.EndLabel == null) {
                    info.EndLabel = generator.defineLabel();
                }
                generator.markLabel(startLabel);
                handleStatement(doStatement.Statement, source);
                generator.markLabel(conditionLabel);
                expressionGenerator.handleExpression(doStatement.Condition, new TargetLabels(info.EndLabel, startLabel), true);
            }
            if (info.EndLabel != null) {
                generator.markLabel(info.EndLabel);
            }
            return null;
        }

        protected override Void handleEmpty(EmptyStatementNode empty, Void source) {
            var info = empty.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            emitBeginLabel(info);
            return null;
        }
        
        protected override Void handleExpression(ExpressionStatementNode expression, Void source) {
            var info = expression.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            emitBeginLabel(info);
            expressionGenerator.handleExpression(expression.Expression, null, false);
            return null;
        }

        protected override Void handleFor(ForStatementNode forStatement, Void source) {
            StatementInfo info = forStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            generator.beginScope();
            emitBeginLabel(info);
            
            var startLabel = generator.defineLabel();
            var conditionLabel = generator.defineLabel();
            LabelMarker continueLabel;
            if (forStatement.Iterator.isEmpty()) {
                continueLabel = conditionLabel;
            } else {
                continueLabel = generator.defineLabel();
            }
            info.ContinueLabel = continueLabel;
            ExpressionInfo cinfo = null;
            if (forStatement.Condition != null) {
                cinfo = forStatement.Condition.getUserData(typeof(ExpressionInfo));
            }
            if (cinfo != null) {
                cinfo.Negate = false;
            }
            foreach (var s in forStatement.Initializer) {
                handleStatement(s, source);
            }
            if (cinfo != null && cinfo.IsConstant) {
                if (((Boolean)cinfo.Value).booleanValue()) {
                    generator.markLabel(startLabel);
                    handleStatement(forStatement.Statement, source);
                    generator.markLabel(continueLabel);
                    foreach (var s in forStatement.Iterator) {
                        handleStatement(s, source);
                    }
                    generator.markLabel(conditionLabel);
                    generator.emit(Opcode.Goto, startLabel);
                }
            } else {
                if (forStatement.Condition != null) {
                    generator.emit(Opcode.Goto, conditionLabel);
                }
                generator.markLabel(startLabel);
                handleStatement(forStatement.Statement, source);
                generator.markLabel(continueLabel);
                foreach (var s in forStatement.Iterator) {
                    handleStatement(s, source);
                }
                if (forStatement.Condition == null) {
                    generator.emit(Opcode.Goto, startLabel);
                } else {
                    if (info.EndLabel == null) {
                        info.EndLabel = generator.defineLabel();
                    }
                    generator.markLabel(conditionLabel);
                    expressionGenerator.handleExpression(forStatement.Condition, new TargetLabels(info.EndLabel, startLabel), true);
                }
            }
            generator.endScope();
            if (info.EndLabel != null) {
                generator.markLabel(info.EndLabel);
            }
            return null;
        }
        
        protected override Void handleForeach(ForeachStatementNode foreachStatement, Void source) {
            var info = foreachStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            generator.beginScope();
            var startLabel = generator.defineLabel();
            if (info.BeginLabel == null) {
                info.BeginLabel = generator.defineLabel();
            }

            int foreachCount = context.MethodGenerationContext.nextForeachStatement();
            var conditionLabel = info.getBeginLabel();
            var sinfo = foreachStatement.Source.getUserData(typeof(ExpressionInfo));
            var isArraySource = sinfo.Type.IsArray;
            var local = foreachStatement.getUserData(typeof(LocalMemberInfo));
            
            expressionGenerator.handleExpression(foreachStatement.Source, null, true);
            
            if (isArraySource) {
                var arrayType = sinfo.Type;
                var elementType = arrayType.ElementType;
                var arrayVar = "foreach$array" + foreachCount;
                var arraylengthVar = "foreach$arraylength" + foreachCount;
                var indexVar = "foreach$index" + foreachCount;
                
                var continueLabel = generator.defineLabel();
                info.ContinueLabel = continueLabel;
                
                generator.emit(Opcode.Dup);
                generator.emit(Opcode.Astore, generator.declareLocal(arrayType, arrayVar));
                generator.emit(Opcode.Arraylength);
                generator.emit(Opcode.Istore, generator.declareLocal(context.TypeSystem.IntType, arraylengthVar));
                generator.emit(Opcode.Iconst_0);
                generator.emit(Opcode.Istore, generator.declareLocal(context.TypeSystem.IntType, indexVar));
                
                generator.emit(Opcode.Goto, conditionLabel);
                
                generator.markLabel(startLabel);
                if (local.IsUsedFromLambda) {
                    emitLoadLambdaScope(context, generator, local.Method);
                }
                generator.emit(Opcode.Aload, generator.getLocal(arrayVar));
                generator.emit(Opcode.Iload, generator.getLocal(indexVar));
                generator.emit(BytecodeHelper.getAloadOpcode(elementType));
                if (local.IsUsedFromLambda) {
                    generator.emit(Opcode.Putfield, getLambdaScopeField(context, local));
                } else {
                    generator.emit(BytecodeHelper.getStoreOpcode(elementType), generator.declareLocal(elementType, local.Name));
                }
                
                handleStatement(foreachStatement.Statement, source);
                
                generator.markLabel(continueLabel);
                generator.emit(Opcode.Iinc, generator.getLocal(indexVar), 1);
                
                generator.markLabel(conditionLabel);
                generator.emit(Opcode.Iload, generator.getLocal(indexVar));
                generator.emit(Opcode.Iload, generator.getLocal(arraylengthVar));
                generator.emit(Opcode.If_icmplt, startLabel);
            } else {
                var iterableType = foreachStatement.getUserData(typeof(TypeInfo));
                var iteratorMethod = iterableType.getMethod("iterator", Query.empty<TypeInfo>());
                var iteratorType = iteratorMethod.getReturnType();
                var elementType = BytecodeHelper.getIterableOrIteratorElementType(iterableType);
                var baseIterator = context.TypeSystem.getType("java/util/Iterator");
                var hasNextMethod = baseIterator.getMethod("hasNext", Query.empty<TypeInfo>());
                MethodInfo nextMethod;
                var requireCast = false;
                switch (elementType.TypeKind) {
                case Boolean:
                case Byte:
                case Char:
                case Short:
                case Int:
                case Long:
                case Float:
                case Double:
                    nextMethod = iteratorType.getMethod("next" + elementType.TypeKind, Query.empty<TypeInfo>());
                    break;
                case UpperBoundedWildcard:
                    nextMethod = baseIterator.getMethod("next", Query.empty<TypeInfo>());
                    elementType = elementType.WildcardBound;
                    requireCast = elementType != context.TypeSystem.ObjectType;
                    break;
                case LowerBoundedWildcard:
                case UnboundedWildcard:
                    nextMethod = baseIterator.getMethod("next", Query.empty<TypeInfo>());
                    elementType = context.TypeSystem.ObjectType;
                    break;
                case GenericParameter:
                    nextMethod = baseIterator.getMethod("next", Query.empty<TypeInfo>());
                    elementType = BytecodeHelper.getGenericParameterTypeErasure(context.TypeSystem, elementType);
                    requireCast = elementType != context.TypeSystem.ObjectType;
                    break;
                default:
                    nextMethod = baseIterator.getMethod("next", Query.empty<TypeInfo>());
                    requireCast = elementType != context.TypeSystem.ObjectType;
                    break;
                }
                var iteratorVar = "foreach$iterator" + foreachCount;

                var dispose = BytecodeHelper.getDisposeMethod(context.AnnotatedTypeSystem, sinfo.Type);
                LocalInfo iterableLocal = null;
                if (dispose != null) {
                    var iterableVar = "foreach$iterable" + foreachCount;
                    iterableLocal = generator.declareLocal(sinfo.Type, iterableVar);
                    foreachStatement.Source.addOrReplaceUserData(iterableLocal);
                    generator.emit(Opcode.Dup);
                    generator.emit(Opcode.Astore, iterableLocal);
                }
                
                generator.emit(Opcode.Invokeinterface, iteratorMethod);
                generator.emit(Opcode.Astore, generator.declareLocal(iteratorType, iteratorVar));
                info.ContinueLabel = conditionLabel;
                
                generator.emit(Opcode.Goto, conditionLabel);
                generator.markLabel(startLabel);
                
                if (local.IsUsedFromLambda) {
                    emitLoadLambdaScope(context, generator, local.Method);
                }
                generator.emit(Opcode.Aload, generator.getLocal(iteratorVar));
                generator.emit(Opcode.Invokeinterface, nextMethod);
                if (requireCast) {
                    generator.emit(Opcode.Checkcast, elementType);
                }
                if (local.IsUsedFromLambda) {
                    generator.emit(Opcode.Putfield, getLambdaScopeField(context, local));
                } else {
                    generator.emit(BytecodeHelper.getStoreOpcode(elementType), generator.declareLocal(elementType, local.Name));
                }
                
                handleStatement(foreachStatement.Statement, source);
                generator.markLabel(conditionLabel);
                
                generator.emit(Opcode.Aload, generator.getLocal(iteratorVar));
                generator.emit(Opcode.Invokeinterface, hasNextMethod);
                generator.emit(Opcode.Ifne, startLabel);
                
                if (dispose != null) {
                    var endForeachLabel = generator.defineLabel();
                    generator.markLabel(endForeachLabel);
                    if (info.EndLabel == null) {
                        info.EndLabel = generator.defineLabel();
                    }
                    generator.emit(Opcode.Aload, iterableLocal);
                    generator.emit(Opcode.Invokevirtual, dispose);
                    if (dispose.ReturnType != context.TypeSystem.VoidType) {
                        generator.emit((dispose.ReturnType.IsCategory2) ? Opcode.Pop2 : Opcode.Pop);
                    }
                    generator.emit(Opcode.Goto, info.EndLabel);
                    
                    var targetLabel = generator.defineLabel();
                    generator.markLabel(targetLabel);
                    generator.emit(Opcode.Aload, iterableLocal);
                    generator.emit(Opcode.Invokevirtual, dispose);
                    if (dispose.ReturnType != context.TypeSystem.VoidType) {
                        generator.emit((dispose.ReturnType.IsCategory2) ? Opcode.Pop2 : Opcode.Pop);
                    }
                    generator.emit(Opcode.Athrow);
                    generator.exceptionHandler(startLabel, endForeachLabel, targetLabel, null);
                }
            }
                
            generator.endScope();
            if (info.EndLabel != null) {
                generator.markLabel(info.EndLabel);
            }
            return null;
        }

        protected override Void handleGoto(GotoStatementNode gotoStatement, Void source) {
            var info = gotoStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            emitEnclosingFinally(gotoStatement, info.Target);
            emitBeginLabel(info);
            var generator = context.MethodGenerationContext.Generator;
            var tinfo = info.Target.getUserData(typeof(StatementInfo));
            if (tinfo.BeginLabel == null) {
                tinfo.BeginLabel = generator.defineLabel();
            }
            generator.emit(Opcode.Goto, tinfo.BeginLabel);
            return null;
        }

        protected override Void handleGotoCase(GotoCaseStatementNode gotoCase, Void source) {
            var info = gotoCase.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            emitBeginLabel(info);
            emitEnclosingFinally(gotoCase, info.getTarget());
            var generator = context.MethodGenerationContext.Generator;
            var tinfo = info.Target.getUserData(typeof(StatementInfo));
            if (tinfo.BeginLabel == null) {
                tinfo.BeginLabel = generator.defineLabel();
            }
            generator.emit(Opcode.Goto, tinfo.BeginLabel);
            return null;
        }

        protected override Void handleIf(IfStatementNode ifStatement, Void source) {
            var info = ifStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            emitBeginLabel(info);
            var generator = context.MethodGenerationContext.Generator;
            
            var condition = ifStatement.Condition;
            var ifTrue = ifStatement.IfTrue;
            var ifFalse = ifStatement.IfFalse;
            var cinfo = condition.getUserData(typeof(ExpressionInfo));
            
            if (cinfo.IsConstant) {
                if ((Boolean)cinfo.Value) {
                    handleStatement(ifTrue, null);
                } else if (ifFalse != null) {
                    handleStatement(ifFalse, null);
                }
            } else {
                var thenLabel = generator.defineLabel();
                var elseLabel = generator.defineLabel();
                expressionGenerator.handleExpression(condition, new TargetLabels(thenLabel, elseLabel), true);
                generator.markLabel(thenLabel);
                handleStatement(ifTrue, null);
                if (ifFalse == null) {
                    generator.markLabel(elseLabel);
                } else {
                    LabelMarker endLabel = null;
                    var tinfo = ifTrue.getUserData(typeof(StatementInfo));
                    if (tinfo.IsEndPointReachable) {
                        endLabel = generator.defineLabel();
                        generator.emit(Opcode.Goto, endLabel);
                    }
                    generator.markLabel(elseLabel);
                    handleStatement(ifFalse, null);
                    if (endLabel != null) {
                        generator.markLabel(endLabel);
                    }
                }
            }
            return null;
        }
        
        protected override Void handleLabeled(LabeledStatementNode labeled, Void source) {
            var info = labeled.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            emitBeginLabel(info);
            handleStatement(labeled.Statement, null);
            return null;
        }
        
        protected override Void handleLocalDeclaration(LocalDeclarationStatementNode localDeclaration, Void source) {
            var info = localDeclaration.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            emitBeginLabel(info);
            var generator = context.MethodGenerationContext.Generator;

            foreach (var decl in localDeclaration.Declarators) {
                var local = decl.getUserData(typeof(LocalMemberInfo));
                var type = local.Type;
                if (decl.Value != null) {
                    if (local.IsUsedFromLambda) {
                        emitLoadLambdaScope(context, generator, local.Method);
                    }
                    expressionGenerator.handleExpression(decl.Value, null, true);
                    emitConversion(context, type, decl.Value);
                    if (local.IsUsedFromLambda) {
                        generator.emit(Opcode.Putfield, getLambdaScopeField(context, local));
                    } else {
                        generator.emit(BytecodeHelper.getStoreOpcode(type), generator.declareLocal(type, local.Name));
                    }
                } else {
                    if (!local.Unused && !local.IsUsedFromLambda) {
						generator.declareLocal(type, local.Name);
					}
                }
            }
            return null;
        }
        
        protected override Void handleReturn(ReturnStatementNode returnStatement, Void source) {
            var info = returnStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            emitBeginLabel(info);
            var generator = context.MethodGenerationContext.Generator;

            var returnType = generator.Method.ReturnType;
            if (returnStatement.Value != null) {
                expressionGenerator.handleExpression(returnStatement.Value, null, true);
                emitConversion(context, returnType, returnStatement.Value);
            }
            
            var scope = returnStatement.getUserData(typeof(StatementScope));
            while (scope != null) {
                var statement = scope.statement;
                switch (statement.StatementKind) {
                case Try:
                    var tryStatement = (TryStatementNode)statement;
                    if (tryStatement.Finally != null) {
                        labelRemover.handleStatement(tryStatement.Finally, null);
                        handleStatement(tryStatement.Finally, null);
                    }
                    break;
                case Synchronized:
                    generator.emit(Opcode.Aload, statement.getUserData(typeof(LocalInfo)));
                    generator.emit(Opcode.Monitorexit);
                    break;
                }
                scope = scope.next;
            }
            if (context.MethodGenerationContext.IsInDestructor) {
                var parentFinalize = context.CurrentType.BaseType.getMethod("finalize", Query.empty<TypeInfo>());
                generator.emit(Opcode.Aload, generator.getLocal("this"));
                generator.emit(Opcode.Invokespecial, parentFinalize);
            }
            generator.emit(BytecodeHelper.getReturnOpcode(returnType));
            return null;
        }

        protected override Void handleSwitch(SwitchStatementNode switchStatement, Void source) {
            var info = switchStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            emitBeginLabel(info);
            var generator = context.MethodGenerationContext.Generator;
            info.EndLabel = generator.defineLabel();
            var sinfo = switchStatement.Selector.getUserData(typeof(ExpressionInfo));
            if (sinfo.Type.IsNumeric || sinfo.Type.IsEnum) {
                var cases = new ArrayList<Integer>();
                int minValue = 0;
                int maxValue = 0;
                expressionGenerator.handleExpression(switchStatement.Selector, null, true);
                if (sinfo.Type.IsEnum) {
                    generator.emit(Opcode.Invokevirtual,
                        context.TypeSystem.getType("java/lang/Enum").getMethod("ordinal", Query.empty<TypeInfo>()));
                }
                SwitchSectionNode defaultSection = null;
                foreach (var section in switchStatement.Sections) {
                    if (section.CaseExpression == null) {
                        defaultSection = section;
                        continue;
                    }
                    Integer value;
                    if (sinfo.Type.IsEnum) {
                        value = (Integer)section.CaseExpression.getUserData(typeof(Integer));
                    } else {
	                    int cvalue;
                        var cinfo = section.CaseExpression.getUserData(typeof(ExpressionInfo));
	                    if (cinfo.Value instanceof Character) {
	                        cvalue = ((Character)cinfo.Value).charValue();
	                    } else {
	                        cvalue = ((Number)cinfo.Value).intValue();
	                    }
                        value = Integer.valueOf(cvalue);
                    }

                    if (cases.isEmpty()) {
                        minValue = maxValue = value.intValue();
                    } else {
                        int v = value.intValue();
                        if (v < minValue) {
                            minValue = v;
                        } else if (v > maxValue) {
                            maxValue = v;
                        }
                    }
                    cases.add(value);
                }
                if (cases.size() > 0) {
                    var labels = new LabelMarker[cases.size()];
                    for (int i = 0; i < sizeof(labels); i++) {
                        labels[i] = generator.defineLabel();
                    }
                    var sortedLabels = new LabelMarker[sizeof(labels)];
                    System.arraycopy(labels, 0, sortedLabels, 0, sizeof(labels));
                    LabelMarker defaultLabel;
                    if (defaultSection != null) {
                        defaultLabel = generator.defineLabel();
                    } else {
                        defaultLabel = info.getEndLabel();
                    }
                    if (maxValue - minValue == cases.size() - 1) {
                        for (int i = 0; i < sizeof(labels); i++) {
                            int value = cases[i].intValue();
                            for (int j = i + 1; j < sizeof(labels); j++) {
                                int value2 = cases[j].intValue();
                                if (value2 < value) {
                                    var tmpi = cases[i];
                                    cases[i] = cases[j];
                                    cases[j] = tmpi;
                                    value = value2;
                                    var tmpl = sortedLabels[i];
                                    sortedLabels[i] = sortedLabels[j];
                                    sortedLabels[j] = tmpl;
                                }
                            }
                        }
                        generator.emit(Opcode.Tableswitch, minValue, maxValue, sortedLabels, defaultLabel);
                    } else {
                        var keys = new int[cases.size()];
                        for (int i = 0; i < sizeof(keys); i++) {
                            keys[i] = cases[i].intValue();
                        }
                        for (int i = 0; i < sizeof(labels); i++) {
                            int value = keys[i];
                            for (int j = i + 1; j < sizeof(labels); j++) {
                                int value2 = keys[j];
                                if (value2 < value) {
                                    value = value2;
                                    int tmp = keys[i];
                                    keys[i] = keys[j];
                                    keys[j] = tmp;
                                    LabelMarker tmpl = sortedLabels[i];
                                    sortedLabels[i] = sortedLabels[j];
                                    sortedLabels[j] = tmpl;
                                }
                            }
                        }
                        generator.emit(Opcode.Lookupswitch, keys, sortedLabels, defaultLabel);
                    }
                    int i = 0;
                    foreach (var section in switchStatement.Sections) {
                        if (section.CaseExpression == null) {
                            generator.markLabel(defaultLabel);
                        } else {
                            generator.markLabel(labels[i++]);
                        }
                        foreach (var s in section.Statements) {
                            handleStatement(s, null);
                        }
                    }
                } else if (defaultSection != null) {
                    generator.emit(Opcode.Pop);
                    foreach (var s in defaultSection.Statements) {
                        handleStatement(s, null);
                    }
                } else {
                    generator.emit(Opcode.Pop);
                }
            } else {
                var objectType = context.TypeSystem.ObjectType;
                if (switchStatement.Sections.size() < 7) {
					var tests = new ArrayList<ArrayList<String>>();
					var statements = new ArrayList<List<StatementNode>>();
					List<StatementNode> defaultStatements = null;
					ArrayList<String> run = null;
					var currentIsDefault = false;
					int ntests = 0;
					foreach (var section in switchStatement.Sections) {
						if (section.CaseExpression == null) {
							if (section.Statements.isEmpty()) {
								currentIsDefault = true;
							} else {
								defaultStatements = section.Statements;
								currentIsDefault = false;
							}
							run = null;
						} else if (currentIsDefault) {
							if (!section.Statements.isEmpty()) {
								defaultStatements = section.Statements;
								currentIsDefault = false;
							}
						} else {
							if (run == null) {
								run = new ArrayList<String>();
							}
							var cinfo = section.CaseExpression.getUserData(typeof(ExpressionInfo));
							if (cinfo == null) {
								run.add(null);
							} else {
								run.add((String)cinfo.Value);
							}
							if (!section.Statements.isEmpty()) {
								tests.add(run);
								ntests += run.size();
								run = null;
								statements.add(section.Statements);
							}
						}
					}
					
                    expressionGenerator.handleExpression(switchStatement.Selector, null, true);
                    var selectorVar = generator.declareLocal(objectType, "switchselector$" + context.MethodGenerationContext.nextStringSwitch());
                    generator.emit(Opcode.Astore, selectorVar);
					if (ntests > 0) {
						var eq = objectType.getMethod("equals", Collections.singleton(objectType));
						var tit = tests.iterator();
						var sit = statements.iterator();
						while (tit.hasNext()) {
							var strings = tit.next();
							var stmts = sit.next();
							
							var thenLabel = (strings.size() == 1) ? null : generator.defineLabel();
							LabelMarker elseLabel = null;
							for (int i = 0; i < strings.size(); i++) {
								var s = strings[i];
								generator.emit(Opcode.Aload, selectorVar);
								if (i == strings.size() - 1) {
									elseLabel = generator.defineLabel();
									if (s == null) {
										generator.emit(Opcode.Ifnonnull, elseLabel);
									} else {
										generator.emit(Opcode.Ldc, s);
										generator.emit(Opcode.Invokevirtual, eq);
										generator.emit(Opcode.Ifeq, elseLabel);
									}
									if (thenLabel != null) {
										generator.markLabel(thenLabel);
									}
								} else {
									if (s == null) {
										generator.emit(Opcode.Ifnull, thenLabel);
									} else {
										generator.emit(Opcode.Ldc, s);
										generator.emit(Opcode.Invokevirtual, eq);
										generator.emit(Opcode.Ifne, thenLabel);
									}
								}
							}
							foreach (var s in stmts) {
								handleStatement(s, null);
							}
							generator.markLabel(elseLabel);
						}
						if (defaultStatements != null) {
							foreach (var s in defaultStatements) {
								handleStatement(s, null);
							}
						}
					}
                } else {
					var cases = new ArrayList<String>();
					SwitchSectionNode defaultSection = null;
					foreach (var section in switchStatement.Sections) {
						if (section.CaseExpression == null) {
							defaultSection = section;
							continue;
						}
						var cinfo = section.CaseExpression.getUserData(typeof(ExpressionInfo));
						if (cinfo == null) {
							cases.add(null);
						} else {
							cases.add((String)cinfo.Value);
						}
					}
					
                    int n = context.CurrentType.Fields.count(p => p.Name.startsWith("stringswitch$"));
                    var mapType = context.TypeSystem.getType("java/util/Map");
                    var argTypes = new ArrayList<TypeInfo> { objectType, objectType };
                    mapType = context.TypeSystem.getGenericType(mapType, argTypes);
                    var hashMapType = context.TypeSystem.getType("java/util/HashMap");
                    var strings = ((TypeBuilder)context.CurrentType).defineField("stringswitch$" + n, hashMapType);
                    strings.setFinal(true);
                    strings.setStatic(true);
                    strings.setSynthetic(true);
                    strings.setPrivate(true);

                    var getIntegerLabel = generator.defineLabel();
                    generator.emit(Opcode.Getstatic, strings);
                    generator.emit(Opcode.Dup);
                    generator.emit(Opcode.Ifnonnull, getIntegerLabel);
                    generator.emit(Opcode.Pop);
                    
                    // Initialize the static map
                    generator.emit(Opcode.New, hashMapType);
                    generator.emit(Opcode.Dup);
                    generator.emit(Opcode.Invokespecial, hashMapType.getMethod("<init>", Query.empty<TypeInfo>()));
                    var valueOf = context.TypeSystem.getBoxingMethod(context.TypeSystem.IntType);
                    var intValue = context.TypeSystem.getUnboxingMethod(context.TypeSystem.IntType);
                    var put = mapType.getMethod("put", argTypes);
                    var get = mapType.getMethod("get", Collections.singletonList(objectType));
                    var labels = new LabelMarker[cases.size()];
                    var defaultLabel = generator.defineLabel();
                    for (int i = 0; i < cases.size(); i++) {
                        labels[i] = generator.defineLabel();
                        generator.emit(Opcode.Dup);
                        generator.emit(Opcode.Ldc, cases.get(i));
                        BytecodeHelper.emitIntConstant(generator, i);
                        generator.emit(Opcode.Invokestatic, valueOf);
                        generator.emit(Opcode.Invokeinterface, put);
                        generator.emit(Opcode.Pop);
                    }
                    generator.emit(Opcode.Dup);
                    generator.emit(Opcode.Putstatic, strings);

                    // Get the Integer code of the string
                    generator.markLabel(getIntegerLabel);
                    expressionGenerator.handleExpression(switchStatement.Selector, null, true);
                    generator.emit(Opcode.Invokeinterface, get);
                    generator.emit(Opcode.Dup);
                    var getIntLabel = generator.defineLabel();
                    generator.emit(Opcode.Ifnonnull, getIntLabel);
                    generator.emit(Opcode.Pop);
                    BytecodeHelper.emitIntConstant(generator, cases.size());
                    var switchLabel = generator.defineLabel();
                    generator.emit(Opcode.Goto, switchLabel);
                    
                    generator.markLabel(getIntLabel);
                    generator.emit(Opcode.Checkcast, context.TypeSystem.getType("java/lang/Integer"));
                    generator.emit(Opcode.Invokevirtual, intValue);

                    // Tablewitch
                    generator.markLabel(switchLabel);
                    generator.emit(Opcode.Tableswitch, 0, cases.size() - 1, labels, defaultLabel);
                    int i = 0;
					var defaultMarked = false;
                    foreach (var section in switchStatement.Sections) {
                        if (section.CaseExpression == null) {
                            generator.markLabel(defaultLabel);
							defaultMarked = true;
                        } else {
                            generator.markLabel(labels[i++]);
                        }
                        foreach (var s in section.Statements) {
                            handleStatement(s, null);
                        }
                    }
					if (!defaultMarked) {
						generator.markLabel(defaultLabel);
					}
                }
            }
            generator.markLabel(info.EndLabel);
            return null;
        }

        protected override Void handleSynchronized(SynchronizedStatementNode synchronizedStatement, Void source) {
            var info = synchronizedStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            emitBeginLabel(info);
            var generator = context.MethodGenerationContext.Generator;
            
            expressionGenerator.handleExpression(synchronizedStatement.Lock, null, true);
            var local = generator.declareLocal(context.TypeSystem.getType("java/lang/Object"), "synchro$lock");
            synchronizedStatement.addOrReplaceUserData(local);
            generator.emit(Opcode.Dup);
            generator.emit(Opcode.Astore, local);
            generator.emit(Opcode.Monitorenter);
            var startLabel = generator.defineLabel();
            generator.markLabel(startLabel);
            handleStatement(synchronizedStatement.Statement, null);
            var endSyncLabel = generator.defineLabel();
            generator.markLabel(endSyncLabel);
            var fallThrough = synchronizedStatement.Statement.getUserData(typeof(StatementInfo)).IsEndPointReachable;
            if (fallThrough) {
                if (info.EndLabel == null) {
                    info.EndLabel = generator.defineLabel();
                }
                generator.emit(Opcode.Aload, local);
                generator.emit(Opcode.Monitorexit);
                generator.emit(Opcode.Goto, info.EndLabel);
            }
            var targetLabel = generator.defineLabel();
            generator.markLabel(targetLabel);
            generator.emit(Opcode.Aload, local);
            generator.emit(Opcode.Monitorexit);
            generator.emit(Opcode.Athrow);
            generator.exceptionHandler(startLabel, endSyncLabel, targetLabel, null);
            if (info.EndLabel != null) {
                generator.markLabel(info.EndLabel);
            }
            return null;
        }

        protected override Void handleThrow(ThrowStatementNode throwStatement, Void source) {
            var info = throwStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            emitBeginLabel(info);
            var generator = context.MethodGenerationContext.Generator;
            if (context.Iterables[context.MethodGenerationContext.CurrentMethod] != null) {
                generator.emit(Opcode.Aload, generator.getLocal("this"));
                generator.emit(Opcode.Iconst_M1);
                generator.emit(Opcode.Putfield, context.MethodGenerationContext.CurrentMethod.DeclaringType.getField("state$0"));
            }
            if (throwStatement.Exception == null) {
                var catchVars = context.MethodGenerationContext.CatchVariables;
                if (catchVars.isEmpty()) {
                    throw new RuntimeException("Internal error: Throw used outside a catch clause");
                }
                var local = catchVars[catchVars.size() - 1];
                generator.emit(Opcode.Aload, local);
            } else {
                expressionGenerator.handleExpression(throwStatement.Exception, null, true);
            }
            generator.emit(Opcode.Athrow);
            return null;
        }

        protected override Void handleTry(TryStatementNode tryStatement, Void source) {
            var info = tryStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            if (info.BeginLabel == null) {
                info.BeginLabel = generator.defineLabel();
            }
            generator.markLabel(info.BeginLabel);
            handleStatement(tryStatement.Block, source);
            LabelMarker finallyLabel = null;
            if (tryStatement.Finally != null) {
                finallyLabel = generator.defineLabel();
            }
            var fallThrough = tryStatement.Block.getUserData(typeof(StatementInfo)).IsEndPointReachable;
            if (fallThrough) {
                if (info.EndLabel == null) {
                    info.EndLabel = generator.defineLabel();
                }
                generator.emit(Opcode.Goto, info.EndLabel);
            }
            var endBlockLabel = generator.defineLabel();
            generator.markLabel(endBlockLabel);
            foreach (var node in tryStatement.CatchClauses) {
                var targetLabel = generator.defineLabel();
                generator.markLabel(targetLabel);
                generator.beginScope();
                var catchVars = context.getMethodGenerationContext().getCatchVariables();
                TypeInfo etype;
                if (node.ExceptionType == null) {
                    etype = context.TypeSystem.getType("java/lang/Throwable");
                    var local = generator.declareLocal(etype, "throwable$exception");
                    generator.emit(Opcode.Astore, local);
                    catchVars.add(local);
                } else {
                    etype = node.getUserData(typeof(TypeInfo));
                    LocalInfo local;
                    if (node.NameLength == 0) {
                        local = generator.declareLocal(etype, "anonymous$exception");
	                    generator.emit(Opcode.Astore, local);
                    } else {
                        local = generator.declareLocal(etype, context.getIdentifier(node.NameOffset, node.NameLength));
	                    generator.emit(Opcode.Astore, local);
                    	var linfo = node.getUserData(typeof(LocalMemberInfo));
	                    if (linfo.IsUsedFromLambda) {
	                        emitLoadLambdaScope(context, generator, linfo.Method);
		                    generator.emit(Opcode.Aload, local);
	                        generator.emit(Opcode.Putfield, getLambdaScopeField(context, linfo));
	                    }
                    }
                    catchVars.add(local);
                }
                var stmts = node.Block.Statements;
                foreach (var s in stmts) {
                    handleStatement(s, source);
                }
                var gotoEnd = true;
                if (stmts.size() > 0) {
                    gotoEnd = stmts[stmts.size() - 1].getUserData(typeof(StatementInfo)).IsEndPointReachable;
                }
                if (gotoEnd) {
                    fallThrough = true;
                    if (info.EndLabel == null) {
                        info.EndLabel = generator.defineLabel();
                    }
                    generator.emit(Opcode.Goto, info.EndLabel);
                }
                if (tryStatement.Finally != null) {
                    var endCatchLabel = generator.defineLabel();
                    generator.markLabel(endCatchLabel);
                    generator.exceptionHandler(targetLabel, endCatchLabel, finallyLabel, null);
                }
                generator.exceptionHandler(info.BeginLabel, endBlockLabel, targetLabel, etype);
                catchVars.remove(catchVars.size() - 1);
                generator.endScope();
            }
            if (tryStatement.Finally != null) {
                generator.markLabel(finallyLabel);
                generator.beginScope();
                var local = generator.declareLocal(context.TypeSystem.getType("java/lang/Throwable"), "finally$exception", true);
                generator.emit(Opcode.Astore, local);
                labelRemover.handleStatement(tryStatement.Finally, null);
                handleStatement(tryStatement.Finally, source);
                generator.emit(Opcode.Aload, local);
                generator.emit(Opcode.Athrow);
                generator.exceptionHandler(info.BeginLabel, endBlockLabel, finallyLabel, null);
                generator.endScope();
            }
            if (info.getEndLabel() != null) {
                generator.markLabel(info.EndLabel);
            }
            if (fallThrough && tryStatement.Finally != null) {
                generator.beginScope();
                labelRemover.handleStatement(tryStatement.Finally, null);
                handleStatement(tryStatement.Finally, source);
                generator.endScope();
            }
            return null;
        }

        protected override Void handleUsing(UsingStatementNode usingStatement, Void source) {
            var info = usingStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            emitBeginLabel(info);
            var generator = context.MethodGenerationContext.Generator;
            generator.beginScope();
            var resource = usingStatement.ResourceAcquisition;
            if (resource.StatementKind == StatementKind.Expression) {
                var expr = ((ExpressionStatementNode)resource).Expression;
                expressionGenerator.handleExpression(expr, null, true);
                var local = generator.declareLocal(expr.getUserData(typeof(ExpressionInfo)).Type, "using$local0");
                resource.addOrReplaceUserData(local);
                generator.emit(Opcode.Dup);
                generator.emit(Opcode.Astore, local);
            } else {
                handleStatement(resource, null);
            }
            var startLabel = generator.defineLabel();
            generator.markLabel(startLabel);
            var ninstructions = generator.Instructions.count();
            handleStatement(usingStatement.Statement, null);
            var emptyStatement = ninstructions == generator.Instructions.count();
            
            var endUsingLabel = generator.defineLabel();
            generator.markLabel(endUsingLabel);
            emitDispose(usingStatement);
            
            if (!emptyStatement) {
                if (info.EndLabel == null) {
                    info.EndLabel = generator.defineLabel();
                }
                generator.emit(Opcode.Goto, info.EndLabel);
                
                var targetLabel = generator.defineLabel();
                generator.markLabel(targetLabel);
                emitDispose(usingStatement);
                generator.emit(Opcode.Athrow);
                generator.exceptionHandler(startLabel, endUsingLabel, targetLabel, null);
            }
            
            generator.endScope();
            if (info.EndLabel != null) {
                generator.markLabel(info.EndLabel);
            }
            return null;
        }

        private void emitDispose(UsingStatementNode usingStatement) {
            var generator = context.MethodGenerationContext.Generator;
            var resource = usingStatement.ResourceAcquisition;
            if (resource.StatementKind == StatementKind.Expression) {
                var local = resource.getUserData(typeof(LocalInfo));
                var endLabel = generator.defineLabel();
                generator.emit(Opcode.Aload, local);
                generator.emit(Opcode.Ifnull, endLabel);
                generator.emit(Opcode.Aload, local);
                var dispose = BytecodeHelper.getDisposeMethod(context.AnnotatedTypeSystem, local.Type);
                generator.emit((dispose.DeclaringType.IsInterface) ? Opcode.Invokeinterface : Opcode.Invokevirtual, dispose);
                if (dispose.ReturnType != context.TypeSystem.VoidType) {
                    generator.emit((dispose.ReturnType.IsCategory2) ? Opcode.Pop2 : Opcode.Pop);
                }
                generator.markLabel(endLabel);
            } else {
                foreach (var decl in ((LocalDeclarationStatementNode)resource).Declarators) {
                    var l = decl.getUserData(typeof(LocalMemberInfo));
                    var endLabel = generator.defineLabel();
                    generator.emit(Opcode.Aload, generator.getLocal(l.Name));
                    generator.emit(Opcode.Ifnull, endLabel);
                    generator.emit(Opcode.Aload, generator.getLocal(l.Name));
                    var dispose = BytecodeHelper.getDisposeMethod(context.AnnotatedTypeSystem, l.Type);
                    generator.emit((dispose.DeclaringType.IsInterface) ? Opcode.Invokeinterface : Opcode.Invokevirtual, dispose);
                    if (dispose.ReturnType != context.TypeSystem.VoidType) {
                        generator.emit((dispose.ReturnType.IsCategory2) ? Opcode.Pop2 : Opcode.Pop);
                    }
                    generator.markLabel(endLabel);
                }
            }
        }
        
        protected override Void handleWhile(WhileStatementNode whileStatement, Void source) {
            var info = whileStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            var startLabel = generator.defineLabel();
            if (info.BeginLabel == null) {
                info.BeginLabel = generator.defineLabel();
            }
            var conditionLabel = info.BeginLabel;
            info.ContinueLabel = conditionLabel;
            var cinfo = whileStatement.Condition.getUserData(typeof(ExpressionInfo));
            cinfo.Negate = false;
            if (cinfo.IsConstant) {
                if (((Boolean)cinfo.Value).booleanValue()) {
                    generator.markLabel(startLabel);
                    handleStatement(whileStatement.Statement, source);
                    generator.markLabel(conditionLabel);
                    generator.emit(Opcode.Goto, startLabel);
                }
            } else {
                generator.emit(Opcode.Goto, conditionLabel);
                generator.markLabel(startLabel);
                handleStatement(whileStatement.getStatement(), source);
                generator.markLabel(conditionLabel);
                if (info.EndLabel == null) {
                    info.EndLabel = generator.defineLabel();
                }
                expressionGenerator.handleExpression(whileStatement.Condition, new TargetLabels(info.getEndLabel(), startLabel), true);
            }
            if (info.EndLabel != null) {
                generator.markLabel(info.EndLabel);
            }
            return null;
        }

        protected override Void handleYield(YieldStatementNode yieldStatement, Void source) {
            var info = yieldStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            int yieldCount = context.MethodGenerationContext.YieldCount + 1;
            if (yieldStatement.getValue() != null) {
                context.MethodGenerationContext.YieldCount = yieldCount;
            }
            
            emitBeginLabel(info);
            var generator = context.MethodGenerationContext.Generator;
            if (yieldStatement.Value == null) {
                var scope = yieldStatement.getUserData(typeof(StatementScope));
                while (scope != null) {
                    var node = scope.statement;
                    switch (node.StatementKind) {
                    case Try:
                        var tryStatement = (TryStatementNode)node;
                        if (tryStatement.Finally != null) {
                            labelRemover.handleStatement(tryStatement.Finally, null);
                            handleStatement(tryStatement.Finally, source);
                        }
                        break;
                    case Synchronized:
                        var synchronizedStatement = (SynchronizedStatementNode)node;
                        generator.emit(Opcode.Aload, synchronizedStatement.getUserData(typeof(LocalInfo)));
                        generator.emit(Opcode.Monitorexit);
                        break;
                    }
                    scope = scope.next;
                }

                generator.emit(Opcode.Aload, generator.getLocal("this"));
                generator.emit(Opcode.Iconst_M1);
                generator.emit(Opcode.Putfield, context.MethodGenerationContext.CurrentMethod.DeclaringType.getField("state$0"));
                generator.emit(Opcode.Iconst_0);
            } else {
                generator.emit(Opcode.Aload, generator.getLocal("this"));
                expressionGenerator.handleExpression(yieldStatement.Value, null, true);
                generator.emit(Opcode.Putfield, context.MethodGenerationContext.CurrentMethod.DeclaringType.getField("current$0"));
                generator.emit(Opcode.Aload, generator.getLocal("this"));
                BytecodeHelper.emitIntConstant(generator, yieldCount);
                generator.emit(Opcode.Putfield, context.MethodGenerationContext.CurrentMethod.DeclaringType.getField("state$0"));
                foreach (var s in generator.getAllLocalNames()) {
                    if (s.equals("this")) {
                        continue;
                    }
                    var local = generator.getLocal(s);
                    var f = context.MethodGenerationContext.CurrentMethod.DeclaringType.getField(s);
                    if (f == null) {
                        var typeBuilder = (TypeBuilder)context.MethodGenerationContext.CurrentMethod.DeclaringType;
                        var fb = typeBuilder.defineField(local.Name, local.getType());
                        fb.setPrivate(true);
                        f = fb;
                    }
                    generator.emit(Opcode.Aload, generator.getLocal("this"));
                    generator.emit(BytecodeHelper.getLoadOpcode(local.Type), local);
                    generator.emit(Opcode.Putfield, f);
                }
                generator.emit(Opcode.Iconst_1);
            }
            generator.emit(Opcode.Ireturn);
            if (yieldStatement.Value != null) {
                generator.markLabel(context.MethodGenerationContext.YieldLabels[yieldCount]);
                foreach (String s in generator.getAllLocalNames()) {
                    if (s.equals("this")) {
                        continue;
                    }
                    var local = generator.getLocal(s);
                    var f = context.MethodGenerationContext.CurrentMethod.DeclaringType.getField(s);
                    generator.emit(Opcode.Aload, generator.getLocal("this"));
                    generator.emit(Opcode.Getfield, f);
                    generator.emit(BytecodeHelper.getStoreOpcode(local.Type), local);
                }
            }
            return null;
        }
        
        static void emitStringBuilderAppend(CompilerContext context, ExpressionInfo info) {
            var generator = context.MethodGenerationContext.Generator;
            var argType = (info == null) ? context.TypeSystem.StringType : info.Type;
            Iterable<TypeInfo> argList;
            if (argType.IsPrimitive) {
                argList = Query.singleton(argType);
            } else {
                argList = Query.singleton(context.TypeSystem.ObjectType);
            }
            generator.emit(Opcode.Invokevirtual, context.StringBuilderType.getMethod("append", argList));
        }

        static void emitLoadLambdaScope(CompilerContext context, CodeGenerator generator, MethodInfo localMethod) {
            if (context.MethodGenerationContext.CurrentMethod == localMethod) {
                getLambdaScope(context, localMethod);
                generator.emit(Opcode.Aload, generator.getLocal("lambda$scope"));
            } else {
                generator.emit(Opcode.Aload, generator.getLocal("this"));
            }
        }
        
        static FieldInfo getLambdaScopeField(CompilerContext context, LocalMemberInfo local) {
            var mcontext = context.MethodGenerationContext;
            var result = mcontext.LocalFields[local];
            if (result == null) {
            	result = context.localFields[local];
            	if (result == null) {
            		throw new IllegalStateException(local.Name);
            	}
            	mcontext.LocalFields[local] = result;
            	if (local.IsParameter) {
	            	mcontext.ParametersUsedInLambda.add(local);
	                getLambdaScope(context, local.Method);
                }
            }
            mcontext.IsLambdaScopeUsed = true;
            return result;
        }
        
        static TypeBuilder getLambdaScope(CompilerContext context, MethodInfo localMethod) {
            var mcontext = context.MethodGenerationContext;
            var generator = mcontext.Generator;
            if (mcontext.CurrentMethod == localMethod) {
                if (!mcontext.IsInLambda && !mcontext.IsLambdaScopeInitialized) {
                    var type = mcontext.LambdaScope;
                    generator.emit(Opcode.New, type);
                    generator.emit(Opcode.Dup);
                    generator.emit(Opcode.Invokespecial, type.getMethod("<init>", Query.empty<TypeInfo>()));
                    generator.emit(Opcode.Astore, generator.declareLocal(type, "lambda$scope"));
                    mcontext.IsLambdaScopeInitialized = true;
                }
                if (mcontext.IsLambdaScopeInitialized) {
                    var field = mcontext.LambdaScope.getField("this$0");
                    if (field != null && !mcontext.IsLambdaScopeThisInitialized && !mcontext.IsInLambda) {
						generator.emit(Opcode.Aload, generator.getLocal("lambda$scope"));
                        generator.emit(Opcode.Aload, generator.getLocal("this"));
                        generator.emit(Opcode.Putfield, field);
                        mcontext.IsLambdaScopeThisInitialized = true;
                    }
                }
                var allParams = mcontext.ParametersUsedInLambdas;
                var lparams = mcontext.ParametersUsedInLambda;
                if (!lparams.isEmpty()) {
                    foreach (var param in lparams.toArray(new LocalMemberInfo[lparams.size()])) {
                        if (allParams.contains(param) || param.Method != mcontext.CurrentMethod) {
                            continue;
                        }
                        if (mcontext.IsInLambda) {
                            generator.emit(Opcode.Aload, generator.getLocal("this"));
                        } else {
                            generator.emit(Opcode.Aload, generator.getLocal("lambda$scope"));
                        }
                        generator.emit(BytecodeHelper.getLoadOpcode(param.Type), generator.getLocal(param.Name));
                        generator.emit(Opcode.Putfield, context.getLocalField(mcontext.LambdaScope, param));
                        allParams.add(param);
                        lparams.remove(param);
                    }
                }
            }
            mcontext.IsLambdaScopeUsed = true;
            return mcontext.LambdaScope;
        }
        
        static void emitThisAccess(CompilerContext context, CodeGenerator generator) {
            if (context.MethodGenerationContext.IsInLambda) {
                var field = context.MethodGenerationContext.LambdaScope.getField("this$0");
                context.MethodGenerationContext.IsLambdaScopeUsed = true;
                generator.emit(Opcode.Aload, generator.getLocal("this"));
                generator.emit(Opcode.Getfield, field);
            } else if (context.MethodGenerationContext.IsInIterable) {
                var field = context.MethodGenerationContext.CurrentMethod.DeclaringType.getField("this$0");
                generator.emit(Opcode.Aload, generator.getLocal("this"));
                generator.emit(Opcode.Getfield, field);
            } else {
                generator.emit(Opcode.Aload, generator.getLocal("this"));
            }
        }
        
        static void emitGenericCast(CompilerContext context, TypeInfo target, TypeInfo source) {
            if (target == source) {
                return;
            }
            var generator = context.MethodGenerationContext.Generator;
            if (source.IsGenericParameter) {
                switch (target.TypeKind) {
                case UnboundedWildcard:
                case LowerBoundedWildcard:
                    return;
                case UpperBoundedWildcard:
                    target = target.WildcardBound;
                    break;
                }
                if (source.GenericParameterBounds.count() == 1) {
                    if (!target.isAssignableFrom(source.GenericParameterBounds.single())) {
                        generator.emit(Opcode.Checkcast, target);
                    }
                } else {
                    generator.emit(Opcode.Checkcast, target);
                }
            } else if (isGenericArray(source)) {
                generator.emit(Opcode.Checkcast, target);
            }
        }
        
        private static bool isGenericArray(TypeInfo type) {
            if (!type.IsArray) {
                return false;
            }
            do {
                type = type.ElementType;
            } while (type.IsArray);
            return type.IsGenericParameter;
        }
        
        static void emitConversion(CompilerContext context, TypeInfo targetType, ExpressionNode expression) {
            var generator = context.MethodGenerationContext.Generator;
            var info = expression.getUserData(typeof(ExpressionInfo));
            if (info == null) {
                return;
            }
            var type = info.Type;
            if (type == targetType) {
                return;
            }
            BytecodeHelper.emitNumericConversion(generator, type, targetType);
            if (!targetType.IsPrimitive && info.BoxingKind == BoxingKind.Unbox) {
                generator.emit(Opcode.Invokestatic, context.TypeSystem.getBoxingMethod((targetType.IsObject) ? type : targetType));
            }
        }
        
        void emitArray(int dimensions, TypeInfo type, Iterator<ExpressionNode> values) {
            var generator = context.MethodGenerationContext.Generator;
            if (values == null) {
                BytecodeHelper.emitNewarray(generator, dimensions, type.ElementType);
            } else {
                BytecodeHelper.emitNewarray(generator, 1, type.ElementType);
                var opcode = BytecodeHelper.getAstoreOpcode(type.ElementType);
                int i = 0;
                while (values.hasNext()) {
                    var e = values.next();
                    generator.emit(Opcode.Dup);
                    BytecodeHelper.emitIntConstant(generator, i++);
                    expressionGenerator.emitNestedExpression(e, type);
                    generator.emit(opcode);
                }
            }
        }

        void generateAnnotationsBytecode(List<AnnotationSectionNode> annotations, TypeBuilder typeBuilder) {
            foreach (var section in annotations) {
                foreach (var annotation in section.Annotations) {
                    var type = annotation.getUserData(typeof(TypeInfo));
                    var retentionPolicy = BytecodeHelper.getRetentionPolicy(type);
                    if (retentionPolicy != RetentionPolicy.SOURCE) {
                        generateAnnotationBytecode(annotation, typeBuilder.addAnnotation(type, retentionPolicy == RetentionPolicy.RUNTIME));
                    }
                }
            }
        }
        
        void generateAnnotationsBytecode(List<AnnotationSectionNode> annotations, FieldBuilder fieldBuilder) {
            foreach (var section in annotations) {
                foreach (var annotation in section.Annotations) {
                    var type = annotation.getUserData(typeof(TypeInfo));
                    var retentionPolicy = BytecodeHelper.getRetentionPolicy(type);
                    if (retentionPolicy != RetentionPolicy.SOURCE) {
                        generateAnnotationBytecode(annotation, fieldBuilder.addAnnotation(type, retentionPolicy == RetentionPolicy.RUNTIME));
                    }
                }
            }
        }
        
        void generateAnnotationsBytecode(List<AnnotationSectionNode> annotations, MethodBuilder methodBuilder) {
            foreach (var section in annotations) {
                foreach (var annotation in section.Annotations) {
                    var type = annotation.getUserData(typeof(TypeInfo));
                    var retentionPolicy = BytecodeHelper.getRetentionPolicy(type);
                    if (retentionPolicy != RetentionPolicy.SOURCE) {
                        generateAnnotationBytecode(annotation, methodBuilder.addAnnotation(type, retentionPolicy == RetentionPolicy.RUNTIME));
                    }
                }
            }
        }

        void generateAnnotationsBytecode(List<AnnotationSectionNode> annotations, ParameterBuilder parameterBuilder) {
            foreach (var section in annotations) {
                foreach (var annotation in section.Annotations) {
                    var type = annotation.getUserData(typeof(TypeInfo));
                    var retentionPolicy = BytecodeHelper.getRetentionPolicy(type);
                    if (retentionPolicy != RetentionPolicy.SOURCE) {
                        generateAnnotationBytecode(annotation, parameterBuilder.addAnnotation(type, retentionPolicy == RetentionPolicy.RUNTIME));
                    }
                }
            }
        }

        private void generateAnnotationBytecode(AnnotationCreationNode annotation, AnnotationValueBuilder annotationBuilder) {
            var e = annotation.Initializer;
            if (e != null) {
                if (e.ExpressionKind == ExpressionKind.ObjectInitializer) {
                    foreach (var mi in ((ObjectInitializerExpressionNode)e).MemberInitializers) {
                        var name = context.getIdentifier(mi.NameOffset, mi.NameLength);
                        generateAnnotationArgument(name, mi.Value, annotationBuilder);
                    }
                } else {
                    generateAnnotationArgument("value", e, annotationBuilder);
                }
            }
        }

        private void generateAnnotationArgument(String name, ExpressionNode expression, AnnotationValueBuilder annotationBuilder) {
            var info = expression.getUserData(typeof(ExpressionInfo));
            if (info.IsConstant) {
                switch (info.Type.TypeKind) {
                case Reference:
                    annotationBuilder.setStringArgument(name, (String)info.Value);
                    break;
                case Boolean:
                    annotationBuilder.setBooleanArgument(name, (Boolean)info.Value);
                    break;
                case Byte:
                    annotationBuilder.setByteArgument(name, (Byte)info.Value);
                    break;
                case Char:
                    annotationBuilder.setCharArgument(name, (Character)info.Value);
                    break;
                case Short:
                    annotationBuilder.setShortArgument(name, (Short)info.Value);
                    break;
                case Int:
                    annotationBuilder.setIntArgument(name, (Integer)info.Value);
                    break;
                case Long:
                    annotationBuilder.setLongArgument(name, (Long)info.Value);
                    break;
                case Float:
                    annotationBuilder.setFloatArgument(name, (Float)info.Value);
                    break;
                case Double:
                    annotationBuilder.setDoubleArgument(name, (Double)info.Value);
                    break;
                default:
                    throw new IllegalStateException();
                }
            } else {
                switch (expression.ExpressionKind) {
                case Annotation:
                    var annotationSection = (AnnotationSectionNode)expression;
                    var valueBuilder = annotationBuilder.setAnnotationArgument(name, annotationSection.getUserData(typeof(TypeInfo)), true);
                    foreach (var a in annotationSection.Annotations) {
                        generateAnnotationBytecode(a, valueBuilder);
                    }
                    break;
                case ArrayInitializer:
                    var arrayBuilder = annotationBuilder.setArrayArgument(name);
                    foreach (var e in ((ArrayInitializerExpressionNode)expression).Values) {
                        generateAnnotationArrayArgumentElement(e, arrayBuilder);
                    }
                    break;
                case Typeof:
                    annotationBuilder.setTypeArgument(name, expression.getUserData(typeof(TypeInfo)));
                    break;
                default:
                    var mi = info.Members.single();
                    annotationBuilder.setEnumArgument(name, mi.Field.DeclaringType, mi.Field.Name);
                    break;
                }
            }
        }
        
        private void generateAnnotationArrayArgumentElement(ExpressionNode expression, AnnotationArrayValueBuilder arrayBuilder) {
            var info = expression.getUserData(typeof(ExpressionInfo));
            if (info.IsConstant) {
                switch (info.Type.TypeKind) {
                case Reference:
                    arrayBuilder.addStringArgument((String)info.Value);
                    break;
                case Boolean:
                    arrayBuilder.addBooleanArgument((Boolean)info.Value);
                    break;
                case Byte:
                    arrayBuilder.addByteArgument((Byte)info.Value);
                    break;
                case Char:
                    arrayBuilder.addCharArgument((Character)info.Value);
                    break;
                case Short:
                    arrayBuilder.addShortArgument((Short)info.Value);
                    break;
                case Int:
                    arrayBuilder.addIntArgument((Integer)info.Value);
                    break;
                case Long:
                    arrayBuilder.addLongArgument((Long)info.Value);
                    break;
                case Float:
                    arrayBuilder.addFloatArgument((Float)info.Value);
                    break;
                case Double:
                    arrayBuilder.addDoubleArgument((Double)info.Value);
                    break;
                default:
                    throw new IllegalStateException();
                }
            } else {
                switch (expression.ExpressionKind) {
                case Annotation:
                    var annotationSection = (AnnotationSectionNode)expression;
                    var valueBuilder = arrayBuilder.addAnnotationArgument(annotationSection.getUserData(typeof(ExpressionInfo)).Type, true);
                    foreach (var a in annotationSection.Annotations) {
                        generateAnnotationBytecode(a, valueBuilder);
                    }
                    break;
                case ArrayInitializer:
                    arrayBuilder = arrayBuilder.addArrayArgument();
                    foreach (var e in ((ArrayInitializerExpressionNode)expression).Values) {
                        generateAnnotationArrayArgumentElement(e, arrayBuilder);
                    }
                    break;
                case Typeof:
                    arrayBuilder.addTypeArgument(expression.getUserData(typeof(TypeInfo)));
                    break;
                default:
                    var mi = info.Members.single();
                    arrayBuilder.addEnumArgument(mi.Field.DeclaringType, mi.Field.Name);
                    break;
                }
            }
        }

        private void emitEnclosingFinally(StatementNode statement, StatementNode target) {
            var generator = context.MethodGenerationContext.Generator;
            var scope = statement.getUserData(typeof(StatementScope));
            var targetScope = target.getUserData(typeof(StatementScope));
            while (scope != targetScope) {
                var node = scope.statement;
                switch (node.StatementKind) {
                case Try:
                    var tryStatement = (TryStatementNode)node;
                    if (tryStatement.Finally != null) {
                        handleStatement(tryStatement.Finally, null);
                    }
                    break;
                case Synchronized:
                    generator.emit(Opcode.Aload, node.getUserData(typeof(LocalInfo)));
                    generator.emit(Opcode.Monitorexit);
                    break;
                case Foreach:
                    var local = ((ForeachStatementNode)node).getUserData(typeof(LocalInfo));
                    if (local != null) {
                        var dispose = BytecodeHelper.getDisposeMethod(context.AnnotatedTypeSystem, local.Type);
                        generator.emit(Opcode.Aload, local);
                        generator.emit(Opcode.Invokevirtual, dispose);
                        if (dispose.ReturnType != context.TypeSystem.VoidType) {
                            generator.emit((dispose.ReturnType.IsCategory2) ? Opcode.Pop2 : Opcode.Pop);
                        }
                    }
                    break;
                case Using:
                    emitDispose((UsingStatementNode)node);
                    break;
                }
                scope = scope.next;
            }
        }

        private MethodBuilder getMethodBuilder(MethodBuilder methodBuilder, BlockStatementNode body) {
            if (body == null) {
                return methodBuilder;
            }
            var info = body.getUserData(typeof(StatementInfo));
            if (info.YieldCount == 0) {
                return methodBuilder;
            }
            context.MethodGenerationContext.YieldLabels.clear();
            var iterableType = context.Iterables[methodBuilder];
            if (iterableType == null) {
                return methodBuilder;
            }
            TypeInfo objectType = context.TypeSystem.getObjectType();
            TypeInfo elementType = BytecodeHelper.getIterableOrIteratorElementType(methodBuilder.ReturnType);
            TypeInfo iterableInterface;
            switch (elementType.TypeKind) {
            case Boolean:
                iterableInterface = context.TypeSystem.getType("stab/lang/BooleanIterable");
                break;
            case Byte:
                iterableInterface = context.TypeSystem.getType("stab/lang/ByteIterable");
                break;
            case Short:
                iterableInterface = context.TypeSystem.getType("stab/lang/ShortIterable");
                break;
            case Char:
                iterableInterface = context.TypeSystem.getType("stab/lang/CharIterable");
                break;
            case Int:
                iterableInterface = context.TypeSystem.getType("stab/lang/IntIterable");
                break;
            case Long:
                iterableInterface = context.TypeSystem.getType("stab/lang/LongIterable");
                break;
            case Float:
                iterableInterface = context.TypeSystem.getType("stab/lang/FloatIterable");
                break;
            case Double:
                iterableInterface = context.TypeSystem.getType("stab/lang/DoubleIterable");
                break;
            default:
                iterableInterface = context.TypeSystem.getGenericType(context.TypeSystem.getType("java/lang/Iterable"), Collections.singletonList(elementType));
                break;
            }
            iterableType.addInterface(iterableInterface);
            var iteratorInterface = iterableInterface.getMethod("iterator", Query.empty<TypeInfo>()).ReturnType;
            
            FieldBuilder thisField = null;
            if (!methodBuilder.IsStatic) {
                thisField = iterableType.defineField("this$0", methodBuilder.DeclaringType);
                thisField.setPrivate(true);
            }
            var fields = new ArrayList<FieldBuilder>();
            foreach (var p in methodBuilder.Parameters) {
                var f = iterableType.defineField(p.Name, p.Type);
                f.setPrivate(true);
                fields.add(f);
            }

            //
            // Iterable() constructor
            //
            
            var iterableInit = iterableType.defineMethod("<init>");
            iterableInit.setReturnType(context.TypeSystem.VoidType);
            if (!methodBuilder.IsStatic) {
                var pb = iterableInit.addParameter(methodBuilder.DeclaringType);
                pb.setName("this$0");
            }
            foreach (var p in methodBuilder.Parameters) {
                var pb = iterableInit.addParameter(p.Type);
                pb.setName(p.Name);
            }
            var baseConstructor = objectType.getMethod("<init>", Query.empty<TypeInfo>());
            var gen = iterableInit.CodeGenerator;
            gen.beginScope();
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.Invokespecial, baseConstructor);
            if (!methodBuilder.IsStatic) {
                gen.emit(Opcode.Aload, gen.getLocal("this"));
                gen.emit(Opcode.Aload, gen.getLocal("this$0"));
                gen.emit(Opcode.Putfield, thisField);
            }
            foreach (var f in fields) {
                gen.emit(Opcode.Aload, gen.getLocal("this"));
                gen.emit(BytecodeHelper.getLoadOpcode(f.Type), gen.getLocal(f.Name));
                gen.emit(Opcode.Putfield, f);
            }
            
            iterableType.addInterface(iteratorInterface);
            var moveNextField = iterableType.defineField("moveNext$0", context.TypeSystem.BooleanType);
            moveNextField.setPrivate(true);

            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.Iconst_1);
            gen.emit(Opcode.Putfield, moveNextField);
            
            gen.emit(Opcode.Return);
            gen.endScope();

            //
            //	public final Iterator iterator() {
            //		return new Iterator([this], p1, p2, ...);
            //  }
            //
            
            var iteratorMethod = iterableType.defineMethod("iterator");
            iteratorMethod.setPublic(true);
            iteratorMethod.setFinal(true);
            iteratorMethod.setReturnType(iteratorInterface);
            gen = iteratorMethod.CodeGenerator;
            gen.beginScope();
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.New, iterableType);
            gen.emit(Opcode.Dup);
            if (!methodBuilder.IsStatic) {
                gen.emit(Opcode.Aload, gen.getLocal("this"));
                gen.emit(Opcode.Getfield, thisField);
            }
            foreach (var f in fields) {
                gen.emit(Opcode.Aload, gen.getLocal("this"));
                gen.emit(Opcode.Getfield, f);
            }
            gen.emit(Opcode.Invokespecial, iterableInit);
            gen.emit(Opcode.Areturn);
            gen.endScope();

            //
            // method body : return new Iterable([this], p0, p1, ...)[.iterator()];
            //
            
            gen = methodBuilder.CodeGenerator;
            gen.beginScope();
            gen.emit(Opcode.New, iterableType);
            gen.emit(Opcode.Dup);
            if (!methodBuilder.IsStatic) {
                gen.emit(Opcode.Aload, gen.getLocal("this"));
            }
            foreach (var p in methodBuilder.Parameters) {
                gen.emit(BytecodeHelper.getLoadOpcode(p.Type), gen.getLocal(p.Name));
            }
            gen.emit(Opcode.Invokespecial, iterableInit);
            gen.emit(Opcode.Areturn);
            gen.endScope();

            //
            // Bridge
            //
            
            if (elementType.IsPrimitive) {
                var bridgeMethod = iterableType.defineMethod("iterator");
                bridgeMethod.setPublic(true);
                bridgeMethod.setFinal(true);
                bridgeMethod.setBridge(true);
                bridgeMethod.setSynthetic(true);
                bridgeMethod.setReturnType(iteratorInterface.Interfaces.single());
                gen = bridgeMethod.CodeGenerator;
                gen.beginScope();
                gen.emit(Opcode.Aload, gen.getLocal("this"));
                gen.emit(Opcode.Invokevirtual, iteratorMethod);
                gen.emit(Opcode.Areturn);
                gen.endScope();
            }
            
            //
            // Iterator implementation ================================================================
            //

            var hasNextField = iterableType.defineField("hasNext$0", context.TypeSystem.BooleanType);
            hasNextField.setPrivate(true);
            var currentField = iterableType.defineField("current$0", elementType);
            currentField.setPrivate(true);
            var stateField = iterableType.defineField("state$0", context.TypeSystem.IntType);
            stateField.setPrivate(true);

            MethodBuilder moveNextMethod = iterableType.defineMethod("moveNext");
            moveNextMethod.setPrivate(true);
            moveNextMethod.setReturnType(context.TypeSystem.BooleanType);

            //
            //	private void advance() {
            //		if (moveNext) {
            //			hasNext = moveNext();
            //			moveNext = false;
            //		}
            //	}
            //
            
            var advanceMethod = iterableType.defineMethod("advance");
            advanceMethod.setPrivate(true);
            advanceMethod.setReturnType(context.TypeSystem.VoidType);
            gen = advanceMethod.CodeGenerator;
            gen.beginScope();
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.Getfield, moveNextField);
            var elseLabel = gen.defineLabel();
            gen.emit(Opcode.Ifeq, elseLabel);
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.Invokevirtual, moveNextMethod);
            gen.emit(Opcode.Putfield, hasNextField);
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.Iconst_0);
            gen.emit(Opcode.Putfield, moveNextField);
            gen.markLabel(elseLabel);
            gen.emit(Opcode.Return);
            gen.endScope();

            //
            //	public final void remove() {
            //		throw new UnsupportedOperationException();
            // 	}
            //
            
            var removeMethod = iterableType.defineMethod("remove");
            removeMethod.setPublic(true);
            removeMethod.setFinal(true);
            removeMethod.setReturnType(context.TypeSystem.VoidType);
            gen = removeMethod.CodeGenerator;
            gen.beginScope();
            var exceptionType = context.TypeSystem.getType("java/lang/UnsupportedOperationException");
            gen.emit(Opcode.New, exceptionType);
            gen.emit(Opcode.Dup);
            gen.emit(Opcode.Invokespecial, exceptionType.getMethod("<init>", Query.empty<TypeInfo>()));
            gen.emit(Opcode.Athrow);
            gen.endScope();

            //
            //	public final bool hasNext() {
            //		advance();
            //		return hasNext;
            //	}
            //

            var hasNextMethod = iterableType.defineMethod("hasNext");
            hasNextMethod.setPublic(true);
            hasNextMethod.setFinal(true);
            hasNextMethod.setReturnType(context.TypeSystem.BooleanType);
            gen = hasNextMethod.CodeGenerator;
            gen.beginScope();
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.Invokevirtual, advanceMethod);
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.Getfield, hasNextField);
            gen.emit(Opcode.Ireturn);
            gen.endScope();
            
            //
            //	public final T next() {
            //		advance();
            //		if (!hasNext) {
            //			throw new NoSuchElementException();
            //		}
            //		moveNext = true;
            //		return BOX(current);
            //	}
            //

            MethodBuilder nextPrimitiveMethod = null;
            if (elementType.IsPrimitive) {
                nextPrimitiveMethod = iterableType.defineMethod("next" + elementType.TypeKind);
                nextPrimitiveMethod.setPublic(true);
                nextPrimitiveMethod.setFinal(true);
                nextPrimitiveMethod.setReturnType(elementType);
                gen = nextPrimitiveMethod.CodeGenerator;
                gen.beginScope();
                emitNextBody(gen, advanceMethod, hasNextField, moveNextField, currentField);
                gen.endScope();
            }
            
            var nextMethod = iterableType.defineMethod("next");
            nextMethod.setPublic(true);
            nextMethod.setFinal(true);
            MethodInfo boxingMethod = null;
            if (elementType.IsPrimitive) {
                boxingMethod = context.TypeSystem.getBoxingMethod(elementType);
                nextMethod.setReturnType(boxingMethod.ReturnType);
            } else {
                nextMethod.setReturnType(elementType);
            }
            gen = nextMethod.CodeGenerator;
            gen.beginScope();
            if (elementType.IsPrimitive) {
                gen.emit(Opcode.Aload, gen.getLocal("this"));
                gen.emit(Opcode.Invokevirtual, nextPrimitiveMethod);
                gen.emit(Opcode.Invokestatic, boxingMethod);
                gen.emit(Opcode.Areturn);
            } else {
                emitNextBody(gen, advanceMethod, hasNextField, moveNextField, currentField);
            }
            gen.endScope();

            // Bridge
            if (!BytecodeHelper.isCompiledToObject(nextMethod.ReturnType)) {
                var nextBridgeMethod = iterableType.defineMethod("next");
                nextBridgeMethod.setPublic(true);
                nextBridgeMethod.setFinal(true);
                nextBridgeMethod.setBridge(true);
                nextBridgeMethod.setReturnType(objectType);
                gen = nextBridgeMethod.CodeGenerator;
                gen.beginScope();
                gen.emit(Opcode.Aload, gen.getLocal("this"));
                gen.emit(Opcode.Invokevirtual, nextMethod);
                gen.emit(Opcode.Areturn);
                gen.endScope();
            }

            //
            // moveNext control table
            //
            
            gen = moveNextMethod.CodeGenerator;
            gen.beginScope();
            context.MethodGenerationContext.YieldLabels.clear();
            var startLabel = gen.defineLabel();
            context.MethodGenerationContext.YieldLabels.add(startLabel);
            for (int i = 0; i < info.YieldCount; i++) {
                context.MethodGenerationContext.YieldLabels.add(gen.defineLabel());
            }
            var labels = context.MethodGenerationContext.YieldLabels.toArray(new LabelMarker[info.YieldCount]);
            var defaultLabel = gen.defineLabel();
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.Getfield, stateField);
            gen.emit(Opcode.Tableswitch, 0, info.YieldCount, labels, defaultLabel);
            gen.markLabel(defaultLabel);
            gen.emit(Opcode.Iconst_0);
            gen.emit(Opcode.Ireturn);
            gen.markLabel(startLabel);
            foreach (var f in fields) {
                gen.emit(Opcode.Aload, gen.getLocal("this"));
                gen.emit(Opcode.Getfield, f);
                gen.emit(BytecodeHelper.getStoreOpcode(f.Type), gen.declareLocal(f.Type, f.Name));
            }
            
            context.MethodGenerationContext.Iterable = iterableType;
            return moveNextMethod;
        }
        
        private void emitNextBody(CodeGenerator gen, MethodInfo advanceMethod, FieldInfo hasNextField,
                FieldInfo moveNextField, FieldInfo currentField) {
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.Invokevirtual, advanceMethod);
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.Getfield, hasNextField);
            var elseLabel = gen.defineLabel();
            gen.emit(Opcode.Ifne, elseLabel);
            var exceptionType = context.TypeSystem.getType("java/util/NoSuchElementException");
            gen.emit(Opcode.New, exceptionType);
            gen.emit(Opcode.Dup);
            gen.emit(Opcode.Invokespecial, exceptionType.getMethod("<init>", Query.empty<TypeInfo>()));
            gen.emit(Opcode.Athrow);
            gen.markLabel(elseLabel);
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.Iconst_1);
            gen.emit(Opcode.Putfield, moveNextField);
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.Getfield, currentField);
            gen.emit(BytecodeHelper.getReturnOpcode(currentField.Type));
        }

        private void initializeLambdaScope(MethodInfo method) {
        	var lambdaScope = context.MethodGenerationContext.LambdaScope;
            if (lambdaScope != null && lambdaScope.Fields.any()) {
                getLambdaScope(context, method);
                foreach (var p in method.Parameters) {
                    var pi = (LocalMemberInfo)MemberInfo.getInfo(p, method);
                    if (pi.IsUsedFromLambda) {
                        getLambdaScopeField(context, pi);
                    }
                }
            }
        }
        
        private void emitBeginLabel(StatementInfo info) {
            if (info.BeginLabel == null) {
                if (info.IsTargeted) {
                    info.BeginLabel = context.MethodGenerationContext.Generator.defineLabel();
                }
            }
            if (info.BeginLabel != null) {
                context.MethodGenerationContext.Generator.markLabel(info.BeginLabel);
            }
        }
        
        private void initializeFields(CodeGenerator generator, ClassDeclarationNode classDeclaration) {
            context.MethodGenerationContext.Generator = generator;
            foreach (var member in classDeclaration.getMembers()) {
                if (member.TypeMemberKind == TypeMemberKind.Field) {
                    var fieldDeclaration = (FieldDeclarationNode)member;
                    foreach (var decl in fieldDeclaration.Declarators) {
                        if (decl.Value == null) {
                            continue;
                        }
                        var fieldBuilder = decl.getUserData(typeof(FieldBuilder));
                        if (!fieldBuilder.IsStatic) {
                            generator.emit(Opcode.Aload, generator.getLocal("this"));
                            if (decl.Value.ExpressionKind == ExpressionKind.ArrayInitializer) {
                                var init = (ArrayInitializerExpressionNode)decl.Value;
                                BytecodeHelper.emitIntConstant(generator, init.Values.size());
                                emitArray(0, fieldBuilder.Type, init.Values.iterator());
                            } else {
                                expressionGenerator.handleExpression(decl.Value, null, true);
                                emitConversion(context, fieldBuilder.Type, decl.Value);
                            }
                            generator.emit(Opcode.Putfield, fieldBuilder);
                        }
                    }
                }
            }
        }
        
        private void initializeStaticFields(CodeGenerator generator, ClassDeclarationNode classDeclaration) {
            context.MethodGenerationContext.Generator = generator;
            int nEnumConstant = 0;
            foreach (var member in classDeclaration.Members) {
                switch (member.TypeMemberKind) {
                case Field:
                    var fieldDeclaration = (FieldDeclarationNode)member;
                    foreach (var decl in fieldDeclaration.Declarators) {
                        if (decl.Value == null) {
                            continue;
                        }
                        var fieldBuilder = decl.getUserData(typeof(FieldBuilder));
                        if (fieldBuilder.IsStatic) {
                            var ei = decl.getValue().getUserData(typeof(ExpressionInfo));
                            if (ei != null && (!ei.IsConstant || ei.BoxingKind != BoxingKind.None)) {
                                if (decl.Value.ExpressionKind == ExpressionKind.ArrayInitializer) {
                                    var init = (ArrayInitializerExpressionNode)decl.Value;
                                    BytecodeHelper.emitIntConstant(generator, init.Values.size());
                                    emitArray(0, fieldBuilder.Type, init.Values.iterator());
                                } else {
                                    expressionGenerator.handleExpression(decl.Value, null, true);
                                    emitConversion(context, fieldBuilder.Type, decl.Value);
                                }
                                generator.emit(Opcode.Putstatic, fieldBuilder);
                            }
                        }
                    }
                    break;
                case EnumConstant: {
                    var enumConstantDeclaration = (EnumConstantDeclarationNode)member;
                    var constructor = enumConstantDeclaration.getUserData(typeof(ExpressionInfo)).Method;
                    var field = enumConstantDeclaration.getUserData(typeof(FieldBuilder));
                    generator.emit(Opcode.New, constructor.DeclaringType);
                    generator.emit(Opcode.Dup);
                    generator.emit(Opcode.Ldc, field.Name);
                    BytecodeHelper.emitIntConstant(generator, nEnumConstant++);
					expressionGenerator.emitArguments(enumConstantDeclaration.Arguments, constructor.Parameters.skip(2),
							constructor.Parameters.count() - 2, constructor.IsVarargs);
                    generator.emit(Opcode.Invokespecial, constructor);
                    generator.emit(Opcode.Putstatic, field);
                    break;
                }
                case Constructor:
                    var constructor = (ConstructorDeclarationNode)member;
                    if (constructor.Modifiers.contains(Modifier.Static)) {
                        handleStatement(constructor.Body, null);
                    }
                    break;
                }
            }

            var typeInfo = classDeclaration.getUserData(typeof(TypeInfo));
            if (typeInfo.IsEnum) {
                BytecodeHelper.emitIntConstant(generator, nEnumConstant);
                generator.emit(Opcode.Anewarray, typeInfo);
                nEnumConstant = 0;
                foreach (var member in classDeclaration.Members) {
                    if (member.TypeMemberKind == TypeMemberKind.EnumConstant) {
                        var enumConstantDeclaration = (EnumConstantDeclarationNode)member;
                        var field = enumConstantDeclaration.getUserData(typeof(FieldBuilder));
                        generator.emit(Opcode.Dup);
                        BytecodeHelper.emitIntConstant(generator, nEnumConstant++);
                        generator.emit(Opcode.Getstatic, field);
                        generator.emit(Opcode.Aastore);
                    }
                }
                generator.emit(Opcode.Putstatic, typeInfo.getField("ENUM$VALUES"));
            }
        }

        private class LabelRemover : StatementHandler<Void, Void> {
            LabelRemover()
                : super(true) {
            }
            
            protected override Void handleBlock(BlockStatementNode block, Void source) {
                var info = block.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                    foreach (var s in block.Statements) {
                        handleStatement(s, null);
                    }
                }
                return null;
            }
            
            protected override Void handleBreak(BreakStatementNode breakStatement, Void source) {
                var info = breakStatement.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                }
                return null;
            }
            
            protected override Void handleContinue(ContinueStatementNode continueStatement, Void source) {
                var info = continueStatement.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                }
                return null;
            }
            
            protected override Void handleDo(DoStatementNode doStatement, Void source) {
                var info = doStatement.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                    handleStatement(doStatement.Statement, null);
                }
                return null;
            }
            
            protected override Void handleEmpty(EmptyStatementNode empty, Void source) {
                var info = empty.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                }
                return null;
            }
            
            protected override Void handleExpression(ExpressionStatementNode expression, Void source) {
                var info = expression.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                }
                return null;
            }
            
            protected override Void handleFor(ForStatementNode forStatement, Void source) {
                var info = forStatement.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                    handleStatement(forStatement.Statement, null);
                }
                return null;
            }
            
            protected override Void handleForeach(ForeachStatementNode foreachStatement, Void source) {
                var info = foreachStatement.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                    handleStatement(foreachStatement.Statement, null);
                }
                return null;
            }
            
            protected override Void handleGoto(GotoStatementNode gotoStatement, Void source) {
                var info = gotoStatement.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                }
                return null;
            }
            
            protected override Void handleGotoCase(GotoCaseStatementNode gotoCase, Void source) {
                var info = gotoCase.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                }
                return null;
            }
            
            protected override Void handleIf(IfStatementNode ifStatement, Void source) {
                var info = ifStatement.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                    handleStatement(ifStatement.IfTrue, null);
                    if (ifStatement.IfFalse != null) {
                        handleStatement(ifStatement.IfFalse, null);
                    }
                }
                return null;
            }
            
            protected override Void handleLabeled(LabeledStatementNode labeled, Void source) {
                var info = labeled.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                    handleStatement(labeled.Statement, source);
                }
                return null;
            }
            
            protected override Void handleLocalDeclaration(LocalDeclarationStatementNode localDeclaration, Void source) {
                var info = localDeclaration.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                }
                return null;
            }
            
            protected override Void handleReturn(ReturnStatementNode returnStatement, Void source) {
                var info = returnStatement.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                }
                return null;
            }
            
            protected override Void handleSwitch(SwitchStatementNode switchStatement, Void source) {
                var info = switchStatement.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                    foreach (var section in switchStatement.Sections) {
                        foreach (var s in section.Statements) {
                            handleStatement(s, null);
                        }
                    }
                }
                return null;
            }
            
            protected override Void handleSynchronized(SynchronizedStatementNode synchronizedStatement, Void source) {
                var info = synchronizedStatement.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                    handleStatement(synchronizedStatement.Statement, null);
                }
                return null;
            }
            
            protected override Void handleThrow(ThrowStatementNode throwStatement, Void source) {
                var info = throwStatement.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                }
                return null;
            }
            
            protected override Void handleTry(TryStatementNode tryStatement, Void source) {
                var info = tryStatement.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                    handleStatement(tryStatement.Block, source);
                    foreach (var clause in tryStatement.getCatchClauses()) {
                        var stmts = clause.Block.Statements;
                        foreach (var s in stmts) {
                            handleStatement(s, null);
                        }
                    }
                    if (tryStatement.Finally != null) {
                        handleStatement(tryStatement.Finally, null);
                    }
                }
                return null;
            }
            
            protected override Void handleWhile(WhileStatementNode whileStatement, Void source) {
                var info = whileStatement.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                    handleStatement(whileStatement.Statement, null);
                }
                return null;
            }
            
            protected override Void handleYield(YieldStatementNode yield, Void source) {
                var info = yield.getUserData(typeof(StatementInfo));
                if (info != null) {
                    info.BeginLabel = null;
                    info.ContinueLabel = null;
                    info.EndLabel = null;
                }
                return null;
            }
        }
    }
}
