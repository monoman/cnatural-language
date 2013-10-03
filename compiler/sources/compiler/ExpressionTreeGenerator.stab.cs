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

    class ExpressionTreeGenerator : StatementHandler<Void, Void> {
        private CompilerContext context;
        private ExpressionGenerator expressionGenerator;
        private TypeInfo expressionType;
        private TypeInfo statementType;
        private TypeInfo binaryOperatorType;
        private TypeInfo unaryOperatorType;
        private TypeInfo unboundedConstructorType;
        private TypeInfo memberInitializerType;
        private MethodInfo blockMethod;
        private MethodInfo makeBreakMethod;
        private MethodInfo makeCatchMethod;
        private MethodInfo makeContinueMethod;
        private MethodInfo makeDoMethod;
        private MethodInfo emptyMethod;
        private MethodInfo expressionMethod;
        private MethodInfo makeForMethod;
        private MethodInfo makeForeachMethod;
        private MethodInfo makeGotoMethod;
        private MethodInfo gotoCaseMethod;
        private MethodInfo makeIfMethod;
        private MethodInfo labelMethod;
        private MethodInfo labeledMethod;
        private MethodInfo makeReturnMethod;
        private MethodInfo makeThrowMethod;
        private MethodInfo switchLabelMethod;
        private MethodInfo switchSectionMethod;
        private MethodInfo makeSwitchMethod;
        private MethodInfo makeSynchronizedMethod;
        private MethodInfo makeTryMethod;
        private MethodInfo makeUsingMethod;
        private MethodInfo makeWhileMethod;
        private MethodInfo valueMethod;
        private MethodInfo getDeclaredMethodMethod;
        private MethodInfo getDeclaredFieldMethod;
        private MethodInfo getDeclaredConstructorMethod;
        private MethodInfo callMethod;
        private MethodInfo invokeMethod;
        private MethodInfo makeInstanceofMethod;
        private MethodInfo localMethod;
        private MethodInfo parameterMethod;
        private MethodInfo lambdaMethod;
        private MethodInfo treeMethod;
        private MethodInfo fieldMethod;
        private MethodInfo binaryMethod;
        private MethodInfo unaryMethod;
        private MethodInfo conditionalMethod;
        private MethodInfo newObjectMethod;
        private MethodInfo newArrayMethod;
        private MethodInfo memberInitializerConstructor;
    
        ExpressionTreeGenerator(CompilerContext context)
            : super(true) {
            this.context = context;
            this.expressionGenerator = new ExpressionGenerator(this, context);
        }
        
        #region Reflection cache
        
        TypeInfo ExpressionType {
            get {
                if (expressionType == null) {
                    expressionType = context.TypeSystem.getType("stab/tree/Expression");
                }
                return expressionType;
            }
        }
        
        TypeInfo StatementType {
            get {
                if (statementType == null) {
                    statementType = context.TypeSystem.getType("stab/tree/Statement");
                }
                return statementType;
            }
        }
        
        TypeInfo UnaryOperatorType {
            get {
                if (unaryOperatorType == null) {
                    unaryOperatorType = context.TypeSystem.getType("stab/tree/UnaryOperator");
                }
                return unaryOperatorType;
            }
        }
        
        TypeInfo BinaryOperatorType {
            get {
                if (binaryOperatorType == null) {
                    binaryOperatorType = context.TypeSystem.getType("stab/tree/BinaryOperator");
                }
                return binaryOperatorType;
            }
        }
        
        TypeInfo UnboundedConstructorType {
            get {
                if (unboundedConstructorType == null) {
                    var type = context.TypeSystem.getType("java/lang/reflect/Constructor");
                    unboundedConstructorType = context.TypeSystem.getGenericType(type, Query.singleton(context.TypeSystem.UnboundedWildcard));
                }
                return unboundedConstructorType;
            }
        }
        
        TypeInfo MemberInitializerType {
            get {
                if (memberInitializerType == null) {
                    memberInitializerType = context.TypeSystem.getType("stab/tree/MemberInitializer");
                }
                return memberInitializerType;
            }
        }
        
        MethodInfo BlockMethod {
            get {
                if (blockMethod == null) {
                    blockMethod = this.StatementType.getMethod("block", Query.singleton(this.StatementType.ArrayType));
                }
                return blockMethod;
            }
        }

        MethodInfo MakeBreakMethod {
            get {
                if (makeBreakMethod == null) {
                    makeBreakMethod = this.StatementType.getMethod("makeBreak", Query.empty<TypeInfo>());
                }
                return makeBreakMethod;
            }
        }

        MethodInfo MakeCatchMethod {
            get {
                if (makeCatchMethod == null) {
                    makeCatchMethod = this.StatementType.getMethod("makeCatch", Query.pair(this.ParameterMethod.ReturnType,
                    		this.BlockMethod.ReturnType));
                }
                return makeCatchMethod;
            }
        }
        
        MethodInfo MakeContinueMethod {
            get {
                if (makeContinueMethod == null) {
                    makeContinueMethod = this.StatementType.getMethod("makeContinue", Query.empty<TypeInfo>());
                }
                return makeContinueMethod;
            }
        }
        
        MethodInfo MakeDoMethod {
            get {
                if (makeDoMethod == null) {
                    makeDoMethod = this.StatementType.getMethod("makeDo", Query.pair(this.StatementType, this.ExpressionType));
                }
                return makeDoMethod;
            }
        }

        MethodInfo EmptyMethod {
            get {
                if (emptyMethod == null) {
                    emptyMethod = this.StatementType.getMethod("empty", Query.empty<TypeInfo>());
                }
                return emptyMethod;
            }
        }

        MethodInfo ExpressionMethod {
            get {
                if (expressionMethod == null) {
                    expressionMethod = this.StatementType.getMethod("expression", Query.singleton(this.ExpressionType));
                }
                return expressionMethod;
            }
        }
        
        MethodInfo MakeForMethod {
            get {
                if (makeForMethod == null) {
                    makeForMethod = this.StatementType.getMethod("makeFor", Query.quadruple(this.StatementType.ArrayType, this.ExpressionType,
                            this.StatementType.ArrayType, this.StatementType));
                }
                return makeForMethod;
            }
        }
        
        MethodInfo MakeForeachMethod {
            get {
                if (makeForeachMethod == null) {
                    makeForeachMethod = this.StatementType.getMethod("makeForeach", Query.triple(this.LocalMethod.ReturnType, this.ExpressionType,
                            this.StatementType));
                }
                return makeForeachMethod;
            }
        }

        MethodInfo MakeGotoMethod {
            get {
                if (makeGotoMethod == null) {
                    makeGotoMethod = this.StatementType.getMethod("makeGoto", Query.singleton(this.LabelMethod.ReturnType));
                }
                return makeGotoMethod;
            }
        }
        
        MethodInfo GotoCaseMethod {
            get {
                if (gotoCaseMethod == null) {
                    gotoCaseMethod = this.StatementType.getMethod("gotoCase", Query.singleton(this.SwitchLabelMethod.ReturnType));
                }
                return gotoCaseMethod;
            }
        }
        
        MethodInfo MakeIfMethod {
            get {
                if (makeIfMethod == null) {
                    makeIfMethod = this.StatementType.getMethod("makeIf", Query.triple(this.ExpressionType, this.StatementType, this.StatementType));
                }
                return makeIfMethod;
            }
        }

        MethodInfo LabelMethod {
            get {
                if (labelMethod == null) {
                    labelMethod = this.StatementType.getMethod("label", Query.singleton(context.TypeSystem.StringType));
                }
                return labelMethod;
            }
        }

        MethodInfo LabeledMethod {
            get {
                if (labeledMethod == null) {
                    labeledMethod = this.StatementType.getMethod("labeled", Query.pair(this.LabelMethod.ReturnType, this.StatementType));
                }
                return labeledMethod;
            }
        }

        MethodInfo MakeReturnMethod {
            get {
                if (makeReturnMethod == null) {
                    makeReturnMethod = this.StatementType.getMethod("makeReturn", Query.singleton(this.ExpressionType));
                }
                return makeReturnMethod;
            }
        }

        MethodInfo MakeThrowMethod {
            get {
                if (makeThrowMethod == null) {
                    makeThrowMethod = this.StatementType.getMethod("makeThrow", Query.singleton(this.ExpressionType));
                }
                return makeThrowMethod;
            }
        }
        
        MethodInfo SwitchLabelMethod {
            get {
                if (switchLabelMethod == null) {
                    switchLabelMethod = this.StatementType.getMethod("switchLabel", Query.triple(context.TypeSystem.StringType,
                    		context.TypeSystem.IntType, context.TypeSystem.BooleanType));
                }
                return switchLabelMethod;
            }
        }

        MethodInfo SwitchSectionMethod {
            get {
                if (switchSectionMethod == null) {
                    switchSectionMethod = this.StatementType.getMethod("switchSection", Query.pair(this.SwitchLabelMethod.ReturnType.ArrayType,
                    		this.StatementType.ArrayType));
                }
                return switchSectionMethod;
            }
        }

        MethodInfo MakeSwitchMethod {
            get {
                if (makeSwitchMethod == null) {
                    makeSwitchMethod = this.StatementType.getMethod("makeSwitch", Query.pair(this.ExpressionType,
                    		this.SwitchSectionMethod.ReturnType.ArrayType));
                }
                return makeSwitchMethod;
            }
        }

        MethodInfo MakeSynchronizedMethod {
            get {
                if (makeSynchronizedMethod == null) {
                    makeSynchronizedMethod = this.StatementType.getMethod("makeSynchronized", Query.pair(this.ExpressionType, this.StatementType));
                }
                return makeSynchronizedMethod;
            }
        }

        MethodInfo MakeTryMethod {
            get {
                if (makeTryMethod == null) {
                    makeTryMethod = this.StatementType.getMethod("makeTry", Query.triple(this.BlockMethod.ReturnType,
                    		this.MakeCatchMethod.ReturnType.ArrayType, this.BlockMethod.ReturnType));
                }
                return makeTryMethod;
            }
        }

        MethodInfo MakeUsingMethod {
            get {
                if (makeUsingMethod == null) {
                    makeUsingMethod = this.StatementType.getMethod("makeUsing", Query.pair(this.StatementType, this.StatementType));
                }
                return makeUsingMethod;
            }
        }
        
        MethodInfo MakeWhileMethod {
            get {
                if (makeWhileMethod == null) {
                    makeWhileMethod = this.StatementType.getMethod("makeWhile", Query.pair(this.ExpressionType, this.StatementType));
                }
                return makeWhileMethod;
            }
        }
        
        MethodInfo ValueMethod {
            get {
                if (valueMethod == null) {
                    var parameters = Query.pair(context.TypeSystem.UnboundedClassType, context.TypeSystem.ObjectType);
                    valueMethod = this.ExpressionType.getMethod("value", parameters);
                }
                return valueMethod;
            }
        }
        
        MethodInfo GetDeclaredMethodMethod {
            get {
                if (getDeclaredMethodMethod == null) {
                    var parameters = Query.pair(context.TypeSystem.StringType, context.TypeSystem.UnboundedClassType.ArrayType);
                    getDeclaredMethodMethod = context.TypeSystem.ClassType.getMethod("getDeclaredMethod", parameters);
                }
                return getDeclaredMethodMethod;
            }
        }
        
        MethodInfo GetDeclaredFieldMethod {
            get {
                if (getDeclaredFieldMethod == null) {
                    var parameters = Query.singleton(context.TypeSystem.StringType);
                    getDeclaredFieldMethod = context.TypeSystem.ClassType.getMethod("getDeclaredField", parameters);
                }
                return getDeclaredFieldMethod;
            }
        }
        
        MethodInfo GetDeclaredConstructorMethod {
            get {
                if (getDeclaredConstructorMethod == null) {
                    var parameters = Query.singleton(context.TypeSystem.UnboundedClassType.ArrayType);
                    getDeclaredConstructorMethod = context.TypeSystem.ClassType.getMethod("getDeclaredConstructor", parameters);
                }
                return getDeclaredConstructorMethod;
            }
        }

        MethodInfo CallMethod {
            get {
                if (callMethod == null) {
                    var parameters = Query.triple(this.ExpressionType, this.GetDeclaredMethodMethod.ReturnType, this.ExpressionType.ArrayType);
                    callMethod = this.ExpressionType.getMethod("call", parameters);
                }
                return callMethod;
            }
        }
        
        MethodInfo InvokeMethod {
            get {
                if (invokeMethod == null) {
                    var parameters = Query.pair(this.ExpressionType, this.ExpressionType.ArrayType);
                    invokeMethod = this.ExpressionType.getMethod("invoke", parameters);
                }
                return invokeMethod;
            }
        }
        
        MethodInfo MakeInstanceofMethod {
            get {
                if (makeInstanceofMethod == null) {
                    var parameters = Query.pair(this.ExpressionType, context.TypeSystem.UnboundedClassType.ArrayType);
                    makeInstanceofMethod = this.ExpressionType.getMethod("makeInstanceof", parameters);
                }
                return makeInstanceofMethod;
            }
        }
        
        MethodInfo LocalMethod {
            get {
                if (localMethod == null) {
                    var parameters = Query.pair(context.TypeSystem.UnboundedClassType, context.TypeSystem.StringType);
                    localMethod = this.ExpressionType.getMethod("local", parameters);
                }
                return localMethod;
            }
        }
        
        MethodInfo ParameterMethod {
            get {
                if (parameterMethod == null) {
                    var parameters = Query.pair(context.TypeSystem.UnboundedClassType, context.TypeSystem.StringType);
                    parameterMethod = this.ExpressionType.getMethod("parameter", parameters);
                }
                return parameterMethod;
            }
        }

        MethodInfo LambdaMethod {
            get {
                if (lambdaMethod == null) {
                    var parameters = Query.triple(context.TypeSystem.UnboundedClassType, this.ParameterMethod.ReturnType.ArrayType,
                            this.StatementType);
                    lambdaMethod = this.ExpressionType.getMethod("lambda", parameters);
                }
                return lambdaMethod;
            }
        }

        MethodInfo TreeMethod {
            get {
                if (treeMethod == null) {
                    treeMethod = this.ExpressionType.Methods.where(p => p.Name.equals("tree")).single();
                }
                return treeMethod;
            }
        }

        MethodInfo FieldMethod {
            get {
                if (fieldMethod == null) {
                    var parameters = Query.pair(this.ExpressionType, this.GetDeclaredFieldMethod.ReturnType);
                    fieldMethod = this.ExpressionType.getMethod("field", parameters);
                }
                return fieldMethod;
            }
        }
        
        MethodInfo BinaryMethod {
            get {
                if (binaryMethod == null) {
                    var parameters = Query.quadruple(context.TypeSystem.UnboundedClassType,
                            this.ExpressionType, this.BinaryOperatorType, this.ExpressionType);
                    binaryMethod = this.ExpressionType.getMethod("binary", parameters);
                }
                return binaryMethod;
            }
        }
        
        MethodInfo UnaryMethod {
            get {
                if (unaryMethod == null) {
                    var parameters = Query.triple(context.TypeSystem.UnboundedClassType, this.UnaryOperatorType, this.ExpressionType);
                    unaryMethod = this.ExpressionType.getMethod("unary", parameters);
                }
                return unaryMethod;
            }
        }
        
        MethodInfo ConditionalMethod {
            get {
                if (conditionalMethod == null) {
                    var parameters = Query.quadruple(context.TypeSystem.UnboundedClassType,
                            this.ExpressionType, this.ExpressionType, this.ExpressionType);
                    conditionalMethod = this.ExpressionType.getMethod("conditional", parameters);
                }
                return conditionalMethod;
            }
        }

        MethodInfo NewObjectMethod {
            get {
                if (newObjectMethod == null) {
                    var parameters = Query.triple(this.UnboundedConstructorType, this.ExpressionType.ArrayType,
                            this.MemberInitializerType.ArrayType);
                    newObjectMethod = this.ExpressionType.getMethod("newObject", parameters);
                }
                return newObjectMethod;
            }
        }

        MethodInfo NewArrayMethod {
            get {
                if (newArrayMethod == null) {
                    var parameters = Query.quadruple(context.TypeSystem.UnboundedClassType, this.ExpressionType.ArrayType,
                            context.TypeSystem.IntType, this.ExpressionType.ArrayType);
                    newArrayMethod = this.ExpressionType.getMethod("newArray", parameters);
                }
                return newArrayMethod;
            }
        }

        MethodInfo MemberInitializerConstructor {
            get {
                if (memberInitializerConstructor == null) {
                    memberInitializerConstructor = this.MemberInitializerType.Methods.where(p => p.Name.equals("<init>")).single();
                }
                return memberInitializerConstructor;
            }
        }
        
        #endregion
        
        void generateExpressionTree(LambdaExpressionNode lambda) {
            expressionGenerator.handleExpression(lambda, null, false);
        }

        protected override Void handleBlock(BlockStatementNode block, Void source) {
            var info = block.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            BytecodeHelper.emitIntConstant(generator, block.Statements.size());
            BytecodeHelper.emitNewarray(generator, 1, this.StatementType);
            int i = 0;
            foreach (var statement in block.Statements) {
                generator.emit(Opcode.Dup);
                BytecodeHelper.emitIntConstant(generator, i++);
                handleStatement(statement, null);
                generator.emit(Opcode.Aastore);
            }
            generator.emit(Opcode.Invokestatic, this.BlockMethod);
            return null;
        }
        
        protected override Void handleBreak(BreakStatementNode breakStatement, Void source) {
            var info = breakStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            generator.emit(Opcode.Invokestatic, this.MakeBreakMethod);
            return null;
        }

        protected override Void handleContinue(ContinueStatementNode continueStatement, Void source) {
            var info = continueStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            generator.emit(Opcode.Invokestatic, this.MakeContinueMethod);
            return null;
        }

        protected override Void handleDo(DoStatementNode doStatement, Void source) {
            var info = doStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            handleStatement(doStatement.Statement, null);
            expressionGenerator.handleExpression(doStatement.Condition, null, true);
            generator.emit(Opcode.Invokestatic, this.MakeDoMethod);
            return null;
        }
        
        protected override Void handleEmpty(EmptyStatementNode empty, Void source) {
            var info = empty.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            generator.emit(Opcode.Invokestatic, this.EmptyMethod);
            return null;
        }
            
        protected override Void handleExpression(ExpressionStatementNode expression, Void source) {
            var info = expression.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            expressionGenerator.handleExpression(expression.Expression, null, true);
            generator.emit(Opcode.Invokestatic, this.ExpressionMethod);
            return null;
        }

        protected override Void handleFor(ForStatementNode forStatement, Void source) {
            var info = forStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            if (forStatement.Initializer == null) {
                generator.emit(Opcode.Aconst_Null);
            } else {
                BytecodeHelper.emitIntConstant(generator, forStatement.Initializer.size());
                BytecodeHelper.emitNewarray(generator, 1, this.StatementType);
                int i = 0;
                foreach (var s in forStatement.Initializer) {
                    generator.emit(Opcode.Dup);
                    BytecodeHelper.emitIntConstant(generator, i++);
                    handleStatement(s, source);
                    generator.emit(Opcode.Aastore);
                }
            }
            if (forStatement.Condition == null) {
                generator.emit(Opcode.Aconst_Null);
            } else {
                expressionGenerator.handleExpression(forStatement.Condition, null, true);
            }
            if (forStatement.Iterator == null) {
                generator.emit(Opcode.Aconst_Null);
            } else {
                BytecodeHelper.emitIntConstant(generator, forStatement.Iterator.size());
                BytecodeHelper.emitNewarray(generator, 1, this.StatementType);
                int i = 0;
                foreach (var s in forStatement.Iterator) {
                    generator.emit(Opcode.Dup);
                    BytecodeHelper.emitIntConstant(generator, i++);
                    handleStatement(s, source);
                    generator.emit(Opcode.Aastore);
                }
            }
            handleStatement(forStatement.Statement, null);
            generator.emit(Opcode.Invokestatic, this.MakeForMethod);
            return null;
        }
        
        protected override Void handleForeach(ForeachStatementNode foreachStatement, Void source) {
            var info = foreachStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            var linfo = foreachStatement.getUserData(typeof(LocalMemberInfo));
            var local = generator.declareLocal(this.LocalMethod.ReturnType, "tree$local" + context.MethodGenerationContext.nextGeneratedLocal());
            context.MethodGenerationContext.TreeLocals[linfo] = local;
            BytecodeHelper.emitTypeof(generator, context.TypeSystem, linfo.Type);
            generator.emit(Opcode.Ldc, linfo.Name);
            generator.emit(Opcode.Invokestatic, this.LocalMethod);
            generator.emit(Opcode.Dup);
            generator.emit(Opcode.Astore, local);
            expressionGenerator.handleExpression(foreachStatement.Source, null, true);
            handleStatement(foreachStatement.Statement, null);
            generator.emit(Opcode.Invokestatic, this.MakeForeachMethod);
            return null;
        }

        protected override Void handleGoto(GotoStatementNode gotoStatement, Void source) {
            var info = gotoStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            var local = context.MethodGenerationContext.TreeLabels[info.Target];
            if (local == null) {
                local = generator.declareLocal(this.LabelMethod.ReturnType, "tree$local" + context.MethodGenerationContext.nextGeneratedLocal());
                generator.emit(Opcode.Ldc, context.getIdentifier(gotoStatement.LabelOffset, gotoStatement.LabelLength));
                generator.emit(Opcode.Invokestatic, this.LabelMethod);
                generator.emit(Opcode.Astore, local);
                context.MethodGenerationContext.TreeLabels[info.Target] = local;
            }
            generator.emit(Opcode.Aload, local);
            generator.emit(Opcode.Invokestatic, this.MakeGotoMethod);
            return null;
        }

        protected override Void handleGotoCase(GotoCaseStatementNode gotoCase, Void source) {
            var info = gotoCase.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            var labels = context.MethodGenerationContext.Labels[context.MethodGenerationContext.Labels.size() - 1];
            LocalInfo label;
            if (gotoCase.Expression == null) {
            	label = labels[null];
            } else {
                var sinfo = gotoCase.Expression.getUserData(typeof(ExpressionInfo));
                if (sinfo == null) {
                    var name = (SimpleNameExpressionNode)gotoCase.Expression;
                    var enumField = context.getIdentifier(name.NameOffset, name.NameLength);
	            	label = labels[enumField];
                } else if (sinfo.Type.IsNumeric) {
                    Integer value;
                    if (sinfo.Value instanceof Character) {
                        value = Integer.valueOf(((Character)sinfo.Value).charValue());
                    } else {
                        value = Integer.valueOf(((Number)sinfo.Value).intValue());
                    }
	            	label = labels[value];
                } else {
                	var value = sinfo.Value;
	            	label = labels[value];
                }
            }
            generator.emit(Opcode.Aload, label);
            generator.emit(Opcode.Invokestatic, this.GotoCaseMethod);
            return null;
        }
        
        protected override Void handleIf(IfStatementNode ifStatement, Void source) {
            var info = ifStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            expressionGenerator.handleExpression(ifStatement.Condition, null, true);
            handleStatement(ifStatement.IfTrue, null);
            if (ifStatement.IfFalse != null) {
                handleStatement(ifStatement.IfFalse, null);
            } else {
                generator.emit(Opcode.Aconst_Null);
            }
            generator.emit(Opcode.Invokestatic, this.MakeIfMethod);
            return null;
        }

        protected override Void handleLabeled(LabeledStatementNode labeled, Void source) {
            var info = labeled.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            var local = context.MethodGenerationContext.TreeLabels[labeled];
            if (local == null) {
                local = generator.declareLocal(this.LabelMethod.ReturnType, "tree$local" + context.MethodGenerationContext.nextGeneratedLocal());
                generator.emit(Opcode.Ldc, context.getIdentifier(labeled.NameOffset, labeled.NameLength));
                generator.emit(Opcode.Invokestatic, this.LabelMethod);
                generator.emit(Opcode.Astore, local);
                context.MethodGenerationContext.TreeLabels[labeled] = local;
            }
            generator.emit(Opcode.Aload, local);
            handleStatement(labeled.Statement, null);
            generator.emit(Opcode.Invokestatic, this.LabeledMethod);
            return null;
        }
        
        protected override Void handleLocalDeclaration(LocalDeclarationStatementNode localDeclaration, Void source) {
            var info = localDeclaration.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            
            foreach (var decl in localDeclaration.Declarators) {
                var linfo = decl.getUserData(typeof(LocalMemberInfo));
                var local = generator.declareLocal(this.LocalMethod.ReturnType, "tree$local" + context.MethodGenerationContext.nextGeneratedLocal());
                context.MethodGenerationContext.TreeLocals[linfo] = local;
                BytecodeHelper.emitTypeof(generator, context.TypeSystem, linfo.Type);
                generator.emit(Opcode.Ldc, linfo.Name);
                generator.emit(Opcode.Invokestatic, this.LocalMethod);
                generator.emit(Opcode.Astore, local);
                if (decl.Value != null) {
                    var vinfo = decl.Value.getUserData(typeof(ExpressionInfo));
                    BytecodeHelper.emitTypeof(generator, context.TypeSystem, vinfo.Type);
                    generator.emit(Opcode.Aload, local);
                    generator.emit(Opcode.Getstatic, this.BinaryOperatorType.getField("Assign"));
                    expressionGenerator.handleExpression(decl.Value, null, true);
                    generator.emit(Opcode.Invokestatic, this.BinaryMethod);
                    generator.emit(Opcode.Invokestatic, this.ExpressionMethod);
                }
            }
            return null;
        }
        
        protected override Void handleReturn(ReturnStatementNode returnStatement, Void source) {
            var info = returnStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            if (returnStatement.Value == null) {
                generator.emit(Opcode.Aconst_Null);
            } else {
                expressionGenerator.handleExpression(returnStatement.Value, null, true);
            }
            generator.emit(Opcode.Invokestatic, this.MakeReturnMethod);
            return null;
        }
        
        protected override Void handleSwitch(SwitchStatementNode switchStatement, Void source) {
            var info = switchStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            expressionGenerator.handleExpression(switchStatement.Selector, null, true);
            var sinfo = switchStatement.Selector.getUserData(typeof(ExpressionInfo));
            var isNumeric = sinfo.Type.IsNumeric;
            var isEnum = sinfo.Type.IsEnum;
			var runs = new ArrayList<List<Object>>();
			var statements = new ArrayList<List<StatementNode>>();
			List<Object> run = null;
			var labels = new HashMap<Object, LocalInfo>();
            foreach (var section in switchStatement.Sections) {
            	Object value;
                if (section.CaseExpression == null) {
                	value = null;
                } else if (isEnum) {
                    int ordinal = section.CaseExpression.getUserData(typeof(Integer)).intValue();
                    value = sinfo.Type.Fields.where(p => p.IsEnum).elementAt(ordinal).Name;
                } else {
					value = section.CaseExpression.getUserData(typeof(ExpressionInfo)).Value;
            	}
            	
	            var local = generator.declareLocal(this.SwitchLabelMethod.ReturnType,
	            		"tree$local" + context.MethodGenerationContext.nextGeneratedLocal());
	            labels[value] = local;
	            if (value == null) {
	                generator.emit(Opcode.Aconst_Null);
	                generator.emit(Opcode.Iconst_0);
	                generator.emit(Opcode.Iconst_1);
	            } else if (isNumeric) {
	                generator.emit(Opcode.Aconst_Null);
	            	BytecodeHelper.emitIntConstant(generator, ((Integer)value).intValue());
	                generator.emit(Opcode.Iconst_0);
	            } else {
	                generator.emit(Opcode.Ldc, value);
	                generator.emit(Opcode.Iconst_0);
	                generator.emit(Opcode.Iconst_1);
	            }
	            generator.emit(Opcode.Invokestatic, this.SwitchLabelMethod);
	            generator.emit(Opcode.Astore, local);
            	
            	if (run == null) {
            		run = new ArrayList<Object>();
            	}
            	run.add(value);
				if (!section.Statements.isEmpty()) {
            		runs.add(run);
            		statements.add(section.Statements);
            		run = null;
            	}
            }
            
            context.MethodGenerationContext.Labels.add(labels);
            
            BytecodeHelper.emitIntConstant(generator, runs.size());
            BytecodeHelper.emitNewarray(generator, 1, this.SwitchSectionMethod.ReturnType);
            int index = 0;
            for (int i = 0; i < runs.size(); i++) {
                generator.emit(Opcode.Dup);
                BytecodeHelper.emitIntConstant(generator, index++);
                
            	run = runs[i];
	            BytecodeHelper.emitIntConstant(generator, run.size());
	            BytecodeHelper.emitNewarray(generator, 1, this.SwitchLabelMethod.ReturnType);
	            for (int r = 0; r < run.size(); r++) {
	                generator.emit(Opcode.Dup);
	                BytecodeHelper.emitIntConstant(generator, r);
		            generator.emit(Opcode.Aload, labels[run[r]]);
	                generator.emit(Opcode.Aastore);
	            }
            	
            	var stmts = statements[i];
	            BytecodeHelper.emitIntConstant(generator, stmts.size());
	            BytecodeHelper.emitNewarray(generator, 1, this.StatementType);
	            for (int s = 0; s < stmts.size(); s++) {
	                generator.emit(Opcode.Dup);
	                BytecodeHelper.emitIntConstant(generator, s);
	                handleStatement(stmts[s], null);
	                generator.emit(Opcode.Aastore);
	            }
            	
	            generator.emit(Opcode.Invokestatic, this.SwitchSectionMethod);
                generator.emit(Opcode.Aastore);
            }
            generator.emit(Opcode.Invokestatic, this.MakeSwitchMethod);
            
            context.MethodGenerationContext.Labels.remove(context.MethodGenerationContext.Labels.size() - 1);
            return null;
        }

        protected override Void handleSynchronized(SynchronizedStatementNode synchronizedStatement, Void source) {
            var info = synchronizedStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
			expressionGenerator.handleExpression(synchronizedStatement.Lock, null, true);
            handleStatement(synchronizedStatement.Statement, null);
            generator.emit(Opcode.Invokestatic, this.MakeSynchronizedMethod);
			return null;
		}
        
		protected override Void handleThrow(ThrowStatementNode throwStatement, Void source) {
            var info = throwStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
			if (throwStatement.Exception == null) {
                generator.emit(Opcode.Aconst_Null);
			} else {
                expressionGenerator.handleExpression(throwStatement.Exception, null, true);
			}
            generator.emit(Opcode.Invokestatic, this.MakeThrowMethod);
			return null;
		}
		
        protected override Void handleTry(TryStatementNode tryStatement, Void source) {
            var info = tryStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            handleStatement(tryStatement.Block, null);

            if (tryStatement.CatchClauses.isEmpty()) {
                generator.emit(Opcode.Aconst_Null);
            } else {
                BytecodeHelper.emitIntConstant(generator, tryStatement.CatchClauses.size());
                BytecodeHelper.emitNewarray(generator, 1, this.MakeCatchMethod.ReturnType);
                int i = 0;
                foreach (var clause in tryStatement.CatchClauses) {
                    generator.emit(Opcode.Dup);
                    BytecodeHelper.emitIntConstant(generator, i++);
                    if (clause.NameLength == 0) {
		                generator.emit(Opcode.Aconst_Null);
                    } else {
			            var linfo = clause.getUserData(typeof(LocalMemberInfo));
			            var local = generator.declareLocal(this.LocalMethod.ReturnType,
			            		"tree$local" + context.MethodGenerationContext.nextGeneratedLocal());
			            context.MethodGenerationContext.TreeLocals[linfo] = local;
			            BytecodeHelper.emitTypeof(generator, context.TypeSystem, linfo.Type);
			            generator.emit(Opcode.Ldc, linfo.Name);
			            generator.emit(Opcode.Invokestatic, this.LocalMethod);
			            generator.emit(Opcode.Dup);
			            generator.emit(Opcode.Astore, local);
                    }
                    if (clause.Block.getUserData(typeof(StatementInfo)) == null) {
                    	clause.Block.addUserData(new StatementInfo());
		            }
	            	handleStatement(clause.Block, null);
		            generator.emit(Opcode.Invokestatic, this.MakeCatchMethod);
                    generator.emit(Opcode.Aastore);
                }
            }
            
            if (tryStatement.Finally == null) {
                generator.emit(Opcode.Aconst_Null);
            } else {
	            handleStatement(tryStatement.Finally, null);
            }
            generator.emit(Opcode.Invokestatic, this.MakeTryMethod);
			return null;
		}

		protected override Void handleUsing(UsingStatementNode usingStatement, Void source) {
            var info = usingStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            handleStatement(usingStatement.ResourceAcquisition, null);
            handleStatement(usingStatement.Statement, null);
            generator.emit(Opcode.Invokestatic, this.MakeUsingMethod);
			return null;
		}
		
        protected override Void handleWhile(WhileStatementNode whileStatement, Void source) {
            var info = whileStatement.getUserData(typeof(StatementInfo));
            if (info == null) {
                return null;
            }
            var generator = context.MethodGenerationContext.Generator;
            expressionGenerator.handleExpression(whileStatement.Condition, null, true);
            handleStatement(whileStatement.Statement, null);
            generator.emit(Opcode.Invokestatic, this.MakeWhileMethod);
            return null;
        }
       
        void emitThisAccess(CompilerContext context, CodeGenerator generator) {
            BytecodeHelper.emitTypeof(generator, context.TypeSystem, context.MethodGenerationContext.CurrentMethod.DeclaringType);
            BytecodeGenerator.emitThisAccess(context, generator);
            generator.emit(Opcode.Invokestatic, this.ValueMethod);
        }
        
        void emitArray(int dimensions, TypeInfo type, Iterator<ExpressionNode> values) {
            var generator = context.MethodGenerationContext.Generator;
            BytecodeHelper.emitNewarray(generator, dimensions, type.ElementType);
            if (values != null) {
                var opcode = BytecodeHelper.getAstoreOpcode(type.ElementType);
                int i = 0;
                while (values.hasNext()) {
                    var e = values.next();
                    generator.emit(Opcode.Dup);
                    BytecodeHelper.emitIntConstant(generator, i++);
                    expressionGenerator.handleExpression(e, null, true);
                    generator.emit(opcode);
                }
            }
        }
            
        private class ExpressionGenerator : ExpressionHandler<Void, Void> {
            private ExpressionTreeGenerator statementGenerator;
            private CompilerContext context;
        
            ExpressionGenerator(ExpressionTreeGenerator statementGenerator, CompilerContext context)
                : super(true) {
                this.statementGenerator = statementGenerator;
                this.context = context;
            }
            
            public override Void handleExpression(ExpressionNode expression, Void source, bool nested) {
                var generator = context.MethodGenerationContext.Generator;
                var info = expression.getUserData(typeof(ExpressionInfo));
                if (info == null) {
                    emitNull();
                    return null;
                }
                if (!info.IsConstant) {
                    return super.handleExpression(expression, null, nested);
                }
                BytecodeHelper.emitTypeof(generator, context.TypeSystem, info.Type);
                var value = info.Value;
                switch (info.Type.TypeKind) {
                case Boolean:
                    if (((Boolean)value).booleanValue()) {
                        generator.emit(Opcode.Iconst_1);
                    } else {
                        generator.emit(Opcode.Iconst_0);
                    }
                    generator.emit(Opcode.Invokestatic, context.TypeSystem.getBoxingMethod(info.Type));
                    break;
                case Char:
                case Byte:
                case Short:
                case Int:
                    BytecodeHelper.emitIntConstant(generator, value);
                    generator.emit(Opcode.Invokestatic, context.TypeSystem.getBoxingMethod(info.Type));
                    break;

                case Long:
                    BytecodeHelper.emitLongConstant(generator, value);
                    generator.emit(Opcode.Invokestatic, context.TypeSystem.getBoxingMethod(info.Type));
                    break;

                case Float:
                    BytecodeHelper.emitFloatConstant(generator, value);
                    generator.emit(Opcode.Invokestatic, context.TypeSystem.getBoxingMethod(info.Type));
                    break;

                case Double:
                    BytecodeHelper.emitDoubleConstant(generator, value);
                    generator.emit(Opcode.Invokestatic, context.TypeSystem.getBoxingMethod(info.Type));
                    break;

                default:
                    generator.emit(Opcode.Ldc, value);
                    break;
                }
                generator.emit(Opcode.Invokestatic, statementGenerator.ValueMethod);
                return null;
            }

            protected override Void handleAnonymousObjectCreation(AnonymousObjectCreationExpressionNode anonymousObject,
                    Void source, bool nested) {
                var generator = context.MethodGenerationContext.Generator;
                var typeInfo = anonymousObject.getUserData(typeof(ExpressionInfo)).Type;
                CompilerHelper.initializeAnonymousType(context, typeInfo);
                
                var constructor = typeInfo.Methods.where(p => p.Name.equals("<init>")).first();
                BytecodeHelper.emitTypeof(generator, context.TypeSystem, typeInfo);
                BytecodeHelper.emitIntConstant(generator, constructor.Parameters.count());
                BytecodeHelper.emitNewarray(generator, 1, context.TypeSystem.ClassType);
                int i = 0;
                foreach (var p in constructor.Parameters) {
                    generator.emit(Opcode.Dup);
                    BytecodeHelper.emitIntConstant(generator, i++);
                    BytecodeHelper.emitTypeof(generator, context.TypeSystem, p.Type);
                    generator.emit(Opcode.Aastore);
                }
                generator.emit(Opcode.Invokevirtual, statementGenerator.GetDeclaredConstructorMethod);
                
                BytecodeHelper.emitIntConstant(generator, constructor.Parameters.count());
                BytecodeHelper.emitNewarray(generator, 1, statementGenerator.ExpressionType);
                i = 0;
                foreach (var decl in anonymousObject.MemberDeclarators) {
                    generator.emit(Opcode.Dup);
                    BytecodeHelper.emitIntConstant(generator, i++);
                    handleExpression(decl.Value, null, true);
                    generator.emit(Opcode.Aastore);
                }
                generator.emit(Opcode.Aconst_Null);
                generator.emit(Opcode.Invokestatic, statementGenerator.NewObjectMethod);
                return null;
            }

            protected override Void handleArrayCreation(ArrayCreationExpressionNode arrayCreation, Void source, bool nested) {
                var generator = context.MethodGenerationContext.Generator;
                var type = arrayCreation.getUserData(typeof(ExpressionInfo)).Type;
                var initializer = arrayCreation.Initializer;
                BytecodeHelper.emitTypeof(generator, context.TypeSystem, type);
                if (arrayCreation.DimensionExpressions.size() == 0) {
                    generator.emit(Opcode.Aconst_Null);
                } else {
                    BytecodeHelper.emitIntConstant(generator, arrayCreation.DimensionExpressions.size());
                    BytecodeHelper.emitNewarray(generator, 1, statementGenerator.ExpressionType);
                    int i = 0;
                    foreach (var e in arrayCreation.DimensionExpressions) {
                        generator.emit(Opcode.Dup);
                        BytecodeHelper.emitIntConstant(generator, i++);
                        handleExpression(e, null, true);
                        generator.emit(Opcode.Aastore);
                    }
                }
                BytecodeHelper.emitIntConstant(generator, arrayCreation.Dimensions);
                if (initializer == null) {
                    generator.emit(Opcode.Aconst_Null);
                } else {
                    BytecodeHelper.emitIntConstant(generator, initializer.Values.size());
                    BytecodeHelper.emitNewarray(generator, 1, statementGenerator.ExpressionType);
                    int i = 0;
                    foreach (var e in initializer.Values) {
                        generator.emit(Opcode.Dup);
                        BytecodeHelper.emitIntConstant(generator, i++);
                        handleExpression(e, null, true);
                        generator.emit(Opcode.Aastore);
                    }
                }
                generator.emit(Opcode.Invokestatic, statementGenerator.NewArrayMethod);
                return null;
            }
            
            protected override Void handleAssign(AssignExpressionNode assign, Void source, bool nested) {
                var generator = context.MethodGenerationContext.Generator;
                var info = assign.getUserData(typeof(ExpressionInfo));
                BytecodeHelper.emitTypeof(generator, context.TypeSystem, info.Type);
                handleExpression(assign.Left, null, true);
                var op = (assign.Operator.equals("Assign")) ? "Assign" : assign.Operator.toString() + "Assign";
                generator.emit(Opcode.Getstatic, statementGenerator.BinaryOperatorType.getField(op));
                handleExpression(assign.Right, null, true);
                generator.emit(Opcode.Invokestatic, statementGenerator.BinaryMethod);
                return null;
            }
            
            protected override Void handleBinary(BinaryExpressionNode binary, Void source, bool nested) {
                var generator = context.MethodGenerationContext.Generator;
                var info = binary.getUserData(typeof(ExpressionInfo));
              	if (binary.Operator == BinaryOperator.As) {
	                BytecodeHelper.emitTypeof(generator, context.TypeSystem, info.Type);
	                generator.emit(Opcode.Getstatic, statementGenerator.UnaryOperatorType.getField("As"));
	                handleExpression(binary.LeftOperand, null, true);
	                generator.emit(Opcode.Invokestatic, statementGenerator.UnaryMethod);
	            } else if (binary.Operator == BinaryOperator.Instanceof) {
	                var rinfo = binary.RightOperand.getUserData(typeof(ExpressionInfo));
	                handleExpression(binary.LeftOperand, null, true);
	                BytecodeHelper.emitTypeof(generator, context.TypeSystem, rinfo.Type);
	                generator.emit(Opcode.Invokestatic, statementGenerator.MakeInstanceofMethod);
	            } else {
	                BytecodeHelper.emitTypeof(generator, context.TypeSystem, info.Type);
	                handleExpression(binary.LeftOperand, null, true);
	                generator.emit(Opcode.Getstatic, statementGenerator.BinaryOperatorType.getField(binary.Operator.toString()));
	                handleExpression(binary.RightOperand, null, true);
	                generator.emit(Opcode.Invokestatic, statementGenerator.BinaryMethod);
                }
                return null;
            }

            protected override Void handleCast(CastExpressionNode cast, Void source, bool nested) {
                var generator = context.MethodGenerationContext.Generator;
                var info = cast.getUserData(typeof(ExpressionInfo));
                BytecodeHelper.emitTypeof(generator, context.TypeSystem, info.Type);
                generator.emit(Opcode.Getstatic, statementGenerator.UnaryOperatorType.getField("Cast"));
                handleExpression(cast.Expression, null, true);
                generator.emit(Opcode.Invokestatic, statementGenerator.UnaryMethod);
                return null;
            }

            protected override Void handleConditional(ConditionalExpressionNode conditional, Void source, bool nested) {
                var generator = context.MethodGenerationContext.Generator;
                var info = conditional.getUserData(typeof(ExpressionInfo));
                BytecodeHelper.emitTypeof(generator, context.TypeSystem, info.Type);
                handleExpression(conditional.Condition, null, true);
                handleExpression(conditional.IfTrue, null, true);
                handleExpression(conditional.IfFalse, null, true);
                generator.emit(Opcode.Invokestatic, statementGenerator.ConditionalMethod);
                return null;
            }
            
            protected override Void handleElementAccess(ElementAccessExpressionNode elementAccess, Void source, bool nested) {
                var generator = context.MethodGenerationContext.Generator;
                var info = elementAccess.getUserData(typeof(ExpressionInfo));
                var target = elementAccess.TargetObject;
                var tinfo = target.getUserData(typeof(ExpressionInfo));
                var ttype = tinfo.Type;
                if (!ttype.IsArray) {
                    var method = elementAccess.getUserData(typeof(ExpressionInfo)).Member.GetAccessor.OriginalMethodDefinition;
                    if (!method.IsStatic) {
                        handleExpression(target, null, true);
                    } else {
                        emitNull();
                    }
                    generator.emit(Opcode.Ldc, method.DeclaringType);
                    generator.emit(Opcode.Ldc, method.Name);
                    BytecodeHelper.emitIntConstant(generator, 1);
                    BytecodeHelper.emitNewarray(generator, 1, context.TypeSystem.ClassType);
                    generator.emit(Opcode.Dup);
                    BytecodeHelper.emitIntConstant(generator, 0);
                    BytecodeHelper.emitTypeof(generator, context.TypeSystem, method.Parameters.first().Type);
                    generator.emit(Opcode.Aastore);
                    generator.emit(Opcode.Invokevirtual, statementGenerator.GetDeclaredMethodMethod);

                    BytecodeHelper.emitIntConstant(generator, method.Parameters.count());
                    BytecodeHelper.emitNewarray(generator, 1, statementGenerator.ExpressionType);
                    var arguments = elementAccess.Indexes;
                    emitArguments(arguments, method.Parameters, method.Parameters.count(), method.IsVarargs);
                    
                    generator.emit(Opcode.Invokestatic, statementGenerator.CallMethod);
                } else {
                    BytecodeHelper.emitTypeof(generator, context.TypeSystem, info.Type);
                    handleExpression(target, null, true);
                    generator.emit(Opcode.Getstatic, statementGenerator.BinaryOperatorType.getField("Element"));
                    var index = elementAccess.Indexes[0];
                    handleExpression(index, null, true);
                    generator.emit(Opcode.Invokestatic, statementGenerator.BinaryMethod);
                }
                return null;
            }
            
            protected override Void handleInvocation(InvocationExpressionNode invocation, Void source, bool nested) {
                var generator = context.MethodGenerationContext.Generator;
                var info = invocation.getUserData(typeof(ExpressionInfo));
                var method = info.Method;
                if (method.IsExcludedFromCompilation || CompilerHelper.shouldIgnoreCalls(context, method)) {
                    return null;
                }
                method = method.OriginalMethodDefinition;
                var isDelegateInvocation = BytecodeHelper.isDelegateType(method.DeclaringType) && method.Name.equals("invoke");
                if (!method.IsStatic) {
                    if (isDelegateInvocation) {
                        handleExpression(invocation.TargetObject, null, true);
                    } else if (invocation.TargetObject.ExpressionKind == ExpressionKind.MemberAccess || invocation.TargetObject.ExpressionKind == ExpressionKind.NullSafeMemberAccess) {
                        var targetTarget = ((MemberAccessExpressionNode)invocation.TargetObject).TargetObject;
                        handleExpression(targetTarget, null, true);
                    } else { // SimpleName
                        statementGenerator.emitThisAccess(context, generator);
                    }
                } else {
                    emitNull();
                }
                if (!isDelegateInvocation) {
                    generator.emit(Opcode.Ldc, method.DeclaringType);
                    generator.emit(Opcode.Ldc, method.Name);
                    
                    BytecodeHelper.emitIntConstant(generator, method.Parameters.count());
                    BytecodeHelper.emitNewarray(generator, 1, context.TypeSystem.ClassType);
                    int i = 0;
                    foreach (var p in method.Parameters) {
                        generator.emit(Opcode.Dup);
                        BytecodeHelper.emitIntConstant(generator, i++);
                        BytecodeHelper.emitTypeof(generator, context.TypeSystem, p.Type);
                        generator.emit(Opcode.Aastore);
                    }
                    generator.emit(Opcode.Invokevirtual, statementGenerator.GetDeclaredMethodMethod);
                }
                
                BytecodeHelper.emitIntConstant(generator, method.Parameters.count());
                BytecodeHelper.emitNewarray(generator, 1, statementGenerator.ExpressionType);
                
                var arguments = invocation.Arguments;
                if (info.IsExtension) {
                    var target = (MemberAccessExpressionNode)invocation.TargetObject;
                    arguments = new ArrayList<ExpressionNode> { target.TargetObject };
                    arguments.addAll(invocation.Arguments);
                }
                emitArguments(arguments, method.Parameters, method.Parameters.count(), method.IsVarargs);
                if (!isDelegateInvocation) {
                    generator.emit(Opcode.Invokestatic, statementGenerator.CallMethod);
                } else {
                    generator.emit(Opcode.Invokestatic, statementGenerator.InvokeMethod);
                }
                return null;
            }
            
            protected override Void handleLambda(LambdaExpressionNode lambda, Void source, bool nested) {
                var methodBuilder = lambda.getUserData(typeof(MethodBuilder));
                ((TypeBuilder)methodBuilder.DeclaringType).undefineMethod(methodBuilder);
                
                var info = lambda.getUserData(typeof(ExpressionInfo));
                
                var generator = context.MethodGenerationContext.Generator;
                generator.beginScope();
                var parameterExpressionType = statementGenerator.ParameterMethod.ReturnType;
                var parameters = new ArrayList<LocalInfo>();
                foreach (var pi in methodBuilder.Parameters) {
                    var local = generator.declareLocal(parameterExpressionType, "tree$local" + context.MethodGenerationContext.nextGeneratedLocal());
                    parameters.add(local);
                    context.MethodGenerationContext.TreeLocals[pi.getUserData(typeof(LocalMemberInfo))] = local;
                    BytecodeHelper.emitTypeof(generator, context.TypeSystem, pi.Type);
                    generator.emit(Opcode.Ldc, pi.Name);
                    generator.emit(Opcode.Invokestatic, statementGenerator.ParameterMethod);
                    generator.emit(Opcode.Astore, local);
                }
                var method = (nested) ? statementGenerator.LambdaMethod : statementGenerator.TreeMethod;
                if (info.Type.FullName.equals("stab/tree/ExpressionTree")) {
                    BytecodeHelper.emitTypeof(generator, context.TypeSystem, info.Type.GenericArguments.single());
                } else {
                    BytecodeHelper.emitTypeof(generator, context.TypeSystem, info.Type);
                }
                BytecodeHelper.emitIntConstant(generator, parameters.count());
                BytecodeHelper.emitNewarray(generator, 1, parameterExpressionType);
                int i = 0;
                foreach (var p in parameters) {
                    generator.emit(Opcode.Dup);
                    BytecodeHelper.emitIntConstant(generator, i++);
                    generator.emit(Opcode.Aload, p);
                    generator.emit(Opcode.Aastore);
                }
                
                statementGenerator.handleStatement(lambda.Body, null);
                generator.emit(Opcode.Invokestatic, method);
                generator.endScope();
                return null;
            }
            
            protected override Void handleMemberAccess(MemberAccessExpressionNode memberAccess, Void source, bool nested) {
                var generator = context.MethodGenerationContext.Generator;
                var info = memberAccess.getUserData(typeof(ExpressionInfo));
                if (info.Method != null) {
                    emitDelegateCreation(info.Type, info.Method, memberAccess, nested);
                    return null;
                }
                var member = info.Member;
                if (member == null) {
                    handleExpression(memberAccess.TargetObject, null, true);
                    return null;
                }
                switch (member.MemberKind) {
                case Field: {
                    var field = member.Field;
                    if (!field.IsStatic) {
                        handleExpression(memberAccess.TargetObject, null, true);
                    } else {
                        emitNull();
                    }
                    generator.emit(Opcode.Ldc, field.DeclaringType);
                    generator.emit(Opcode.Ldc, field.Name);
                    generator.emit(Opcode.Invokevirtual, statementGenerator.GetDeclaredFieldMethod);
                    generator.emit(Opcode.Invokestatic, statementGenerator.FieldMethod);
                    break;
                }
                case Property: {
                    var method = member.GetAccessor;
                    if (!method.IsStatic) {
                        handleExpression(memberAccess.TargetObject, null, true);
                    } else {
                        emitNull();
                    }
                    generator.emit(Opcode.Ldc, method.DeclaringType);
                    generator.emit(Opcode.Ldc, method.Name);
                    BytecodeHelper.emitIntConstant(generator, 0);
                    BytecodeHelper.emitNewarray(generator, 1, context.TypeSystem.ClassType);
                    generator.emit(Opcode.Invokevirtual, statementGenerator.GetDeclaredMethodMethod);
                    BytecodeHelper.emitIntConstant(generator, 0);
                    BytecodeHelper.emitNewarray(generator, 1, statementGenerator.ExpressionType);
                    generator.emit(Opcode.Invokestatic, statementGenerator.CallMethod);
                    break;
                }
                default:
                    throw new Exception("Internal error: unhandled name kind: " + member.MemberKind);
                }
                return null;
            }
            
            protected override Void handleObjectCreation(ObjectCreationExpressionNode objectCreation, Void source, bool nested) {
                var generator = context.MethodGenerationContext.Generator;
                var info = objectCreation.getUserData(typeof(ExpressionInfo));
                var method = info.Method;
                if (BytecodeHelper.isDelegateType(info.Type)) {
                    emitDelegateCreation(info.Type, method, objectCreation.Arguments[0], nested);
                    return null;
                }
                int nparams = method.Parameters.count();
                BytecodeHelper.emitTypeof(generator, context.TypeSystem, info.Type);
                BytecodeHelper.emitIntConstant(generator, nparams);
                BytecodeHelper.emitNewarray(generator, 1, context.TypeSystem.ClassType);
                int i = 0;
                foreach (var p in method.Parameters) {
                    generator.emit(Opcode.Dup);
                    BytecodeHelper.emitIntConstant(generator, i++);
                    BytecodeHelper.emitTypeof(generator, context.TypeSystem, p.Type);
                    generator.emit(Opcode.Aastore);
                }
                generator.emit(Opcode.Invokevirtual, statementGenerator.GetDeclaredConstructorMethod);
                BytecodeHelper.emitIntConstant(generator, nparams);
                BytecodeHelper.emitNewarray(generator, 1, statementGenerator.ExpressionType);
                var arguments = objectCreation.Arguments;
                emitArguments(arguments, method.Parameters, method.Parameters.count(), method.IsVarargs);
                var init = objectCreation.Initializer;
                if (init == null) {
                    generator.emit(Opcode.Aconst_Null);
                } else {
                    if (init.ExpressionKind == ExpressionKind.ObjectInitializer) {
                        var initializer = (ObjectInitializerExpressionNode)init;
                        BytecodeHelper.emitIntConstant(generator, initializer.MemberInitializers.size());
                        BytecodeHelper.emitNewarray(generator, 1, statementGenerator.MemberInitializerType);
                        i = 0;
                        foreach (var mi in initializer.MemberInitializers) {
                            generator.emit(Opcode.Dup);
                            BytecodeHelper.emitIntConstant(generator, i++);
                            MemberInfo memb = mi.getUserData(typeof(MemberInfo));
                            generator.emit(Opcode.New, statementGenerator.MemberInitializerType);
                            generator.emit(Opcode.Dup);
                            BytecodeHelper.emitTypeof(generator, context.TypeSystem, memb.DeclaringType);
                            if (memb.MemberKind == MemberKind.Property) {
                                var meth = memb.SetAccessor;
                                generator.emit(Opcode.Ldc, meth.getName());
                                nparams = meth.Parameters.count();
                                BytecodeHelper.emitIntConstant(generator, nparams);
                                BytecodeHelper.emitNewarray(generator, 1, context.TypeSystem.ClassType);
                                i = 0;
                                foreach (var p in meth.Parameters) {
                                    generator.emit(Opcode.Dup);
                                    BytecodeHelper.emitIntConstant(generator, i++);
                                    BytecodeHelper.emitTypeof(generator, context.TypeSystem, p.Type);
                                    generator.emit(Opcode.Aastore);
                                }
                                generator.emit(Opcode.Invokevirtual, statementGenerator.GetDeclaredMethodMethod);
                            } else {
                                generator.emit(Opcode.Ldc, memb.Name);
                                generator.emit(Opcode.Invokevirtual, statementGenerator.GetDeclaredFieldMethod);
                            }
                            BytecodeHelper.emitIntConstant(generator, 1);
                            BytecodeHelper.emitNewarray(generator, 1, statementGenerator.ExpressionType);
                            generator.emit(Opcode.Dup);
                            BytecodeHelper.emitIntConstant(generator, 0);
                            handleExpression(mi.Value, null, true);
                            generator.emit(Opcode.Aastore);
                            generator.emit(Opcode.Invokespecial, statementGenerator.MemberInitializerConstructor);
                            generator.emit(Opcode.Aastore);
                        }
                    } else {
                        var initializer = (CollectionInitializerExpressionNode)init;
                        var addMethod = initializer.getUserData(typeof(MethodInfo)).OriginalMethodDefinition;
                        BytecodeHelper.emitIntConstant(generator, initializer.Values.size());
                        BytecodeHelper.emitNewarray(generator, 1, statementGenerator.MemberInitializerType);
                        i = 0;
                        foreach (var args in initializer.Values) {
                            generator.emit(Opcode.Dup);
                            BytecodeHelper.emitIntConstant(generator, i++);
                            generator.emit(Opcode.New, statementGenerator.MemberInitializerType);
                            generator.emit(Opcode.Dup);
                            BytecodeHelper.emitTypeof(generator, context.TypeSystem, addMethod.DeclaringType);
                            generator.emit(Opcode.Ldc, addMethod.getName());
                            nparams = addMethod.Parameters.count();
                            BytecodeHelper.emitIntConstant(generator, nparams);
                            BytecodeHelper.emitNewarray(generator, 1, context.TypeSystem.ClassType);
                            i = 0;
                            foreach (var p in addMethod.Parameters) {
                                generator.emit(Opcode.Dup);
                                BytecodeHelper.emitIntConstant(generator, i++);
                                BytecodeHelper.emitTypeof(generator, context.TypeSystem, p.Type);
                                generator.emit(Opcode.Aastore);
                            }
                            generator.emit(Opcode.Invokevirtual, statementGenerator.GetDeclaredMethodMethod);
                            BytecodeHelper.emitIntConstant(generator, args.size());
                            BytecodeHelper.emitNewarray(generator, 1, statementGenerator.ExpressionType);
                            int j = 0;
                            foreach (var e in args) {
                                generator.emit(Opcode.Dup);
                                BytecodeHelper.emitIntConstant(generator, j++);
                                handleExpression(e, null, true);
                                generator.emit(Opcode.Aastore);
                            }
                            generator.emit(Opcode.Invokespecial, statementGenerator.MemberInitializerConstructor);
                            generator.emit(Opcode.Aastore);
                        }
                    }
                }
                generator.emit(Opcode.Invokestatic, statementGenerator.NewObjectMethod);
                return null;
            }
            
            protected override Void handleSimpleName(SimpleNameExpressionNode simpleName, Void source, bool nested) {
                var generator = context.MethodGenerationContext.Generator;
                var info = simpleName.getUserData(typeof(ExpressionInfo));
                var member = info.Member;
                switch (member.MemberKind) {
                case Local: {
                    var local = (LocalMemberInfo)member;
                    if (context.MethodGenerationContext.TreeLocals.containsKey(local)) {
                        generator.emit(Opcode.Aload, context.MethodGenerationContext.TreeLocals[local]);
                    } else {
                        BytecodeHelper.emitTypeof(generator, context.TypeSystem, local.Type);
                        if (local.IsUsedFromLambda) {
                            BytecodeGenerator.emitLoadLambdaScope(context, generator, local.Method);
                            generator.emit(Opcode.Getfield, BytecodeGenerator.getLambdaScopeField(context, local));
                        } else {
                            generator.emit(BytecodeHelper.getLoadOpcode(local.Type), generator.getLocal(local.Name));
                        }
                        BytecodeGenerator.emitBoxing(context, simpleName);
                        if (local.Type.IsPrimitive) {
                            generator.emit(Opcode.Invokestatic, context.TypeSystem.getBoxingMethod(local.Type));
                        }
                        generator.emit(Opcode.Invokestatic, statementGenerator.ValueMethod);
                    }
                    break;
                }
                case Field: {
                    var field = member.Field;
                    if (!field.IsStatic) {
                        statementGenerator.emitThisAccess(context, generator);
                    } else {
                        emitNull();
                    }
                    generator.emit(Opcode.Ldc, field.DeclaringType);
                    generator.emit(Opcode.Ldc, field.Name);
                    generator.emit(Opcode.Invokevirtual, statementGenerator.GetDeclaredFieldMethod);
                    generator.emit(Opcode.Invokestatic, statementGenerator.FieldMethod);
                    break;
                }
                case Method: {
                    if (info.Method != null) {
                        emitDelegateCreation(info.Type, info.Method, simpleName, nested);
                    } else {
                        statementGenerator.emitThisAccess(context, generator);
                    }
                    break;
                }
                case Property: {
                    var method = member.GetAccessor;
                    if (!method.IsStatic) {
                        statementGenerator.emitThisAccess(context, generator);
                    } else {
                        emitNull();
                    }
                    generator.emit(Opcode.Ldc, method.DeclaringType);
                    generator.emit(Opcode.Ldc, method.Name);
                    BytecodeHelper.emitIntConstant(generator, 0);
                    BytecodeHelper.emitNewarray(generator, 1, context.TypeSystem.ClassType);
                    generator.emit(Opcode.Invokevirtual, statementGenerator.GetDeclaredMethodMethod);
                    BytecodeHelper.emitIntConstant(generator, 0);
                    BytecodeHelper.emitNewarray(generator, 1, statementGenerator.ExpressionType);
                    generator.emit(Opcode.Invokestatic, statementGenerator.CallMethod);
                    break;
                }
                default:
                    throw new Exception("Internal error: unhandled name kind: " + member.MemberKind);
                }
                return null;
            }

            protected override Void handleSizeof(SizeofExpressionNode sizeofExpression, Void source, bool nested) {
                var generator = context.MethodGenerationContext.Generator;
                BytecodeHelper.emitTypeof(generator, context.TypeSystem, context.TypeSystem.IntType);
                generator.emit(Opcode.Getstatic, statementGenerator.UnaryOperatorType.getField("Sizeof"));
                handleExpression(sizeofExpression.Expression, null, true);
                generator.emit(Opcode.Invokestatic, statementGenerator.UnaryMethod);
                return null;
            }

            protected override Void handleThisAccess(ThisAccessExpressionNode thisAccess, Void source, bool nested) {
                var generator = context.MethodGenerationContext.Generator;
                statementGenerator.emitThisAccess(context, generator);
                return null;
            }
            
            protected override Void handleTypeof(TypeofExpressionNode typeofExpression, Void source, bool nested) {
                var generator = context.MethodGenerationContext.Generator;
                BytecodeHelper.emitTypeof(generator, context.TypeSystem, context.TypeSystem.ClassType);
                var type = typeofExpression.getUserData(typeof(TypeInfo));
                BytecodeHelper.emitTypeof(generator, context.TypeSystem, type);
                generator.emit(Opcode.Invokestatic, statementGenerator.ValueMethod);
                return null;
            }
            
            protected override Void handleUnary(UnaryExpressionNode unary, Void source, bool nested) {
                var generator = context.MethodGenerationContext.Generator;
                var info = unary.getUserData(typeof(ExpressionInfo));
                BytecodeHelper.emitTypeof(generator, context.TypeSystem, info.Type);
                generator.emit(Opcode.Getstatic, statementGenerator.UnaryOperatorType.getField(unary.Operator.toString()));
                handleExpression(unary.Operand, null, true);
                generator.emit(Opcode.Invokestatic, statementGenerator.UnaryMethod);
                return null;
            }
                
            void emitNull() {
                var generator = context.MethodGenerationContext.Generator;
                generator.emit(Opcode.Aconst_Null);
                generator.emit(Opcode.Aconst_Null);
                generator.emit(Opcode.Invokestatic, statementGenerator.ValueMethod);
            }
        
            void emitArguments(List<ExpressionNode> arguments, Iterable<ParameterInfo> parameters, int nparams, bool varargs) {
                int fixedLength = (varargs) ?  nparams - 1 : nparams;
                var generator = context.MethodGenerationContext.Generator;
                var it1 = parameters.iterator();
                var it2 = arguments.iterator();
                int i;
                for (i = 0; i < fixedLength; i++) {
                    it1.next();
                    var e = it2.next();
                    generator.emit(Opcode.Dup);
                    BytecodeHelper.emitIntConstant(generator, i);
                    handleExpression(e, null, true);
                    generator.emit(Opcode.Aastore);
                }
                if (varargs) {
                    int nvarargs = arguments.size() - fixedLength;
                    generator.emit(Opcode.Dup);
                    BytecodeHelper.emitIntConstant(generator, i);
                    if (nvarargs == 1) {
                        var paramType = it1.next().Type;
                        var e = arguments[i];
                        var ei = e.getUserData(typeof(ExpressionInfo));
                        if (ei == null) {
                            emitNull();
                        } else if (ei.Type.IsArray && paramType.isAssignableFrom(ei.Type)) {
                            handleExpression(e, null, true);
                        } else {
                            BytecodeHelper.emitIntConstant(generator, 1);
                            statementGenerator.emitArray(1, paramType, it2);
                        }
                    } else {
                        BytecodeHelper.emitIntConstant(generator, nvarargs);
                        statementGenerator.emitArray(1, it1.next().Type, it2);
                    }
                    generator.emit(Opcode.Aastore);
                }
            }
            
            void emitDelegateCreation(TypeInfo delegateType, MethodInfo method, ExpressionNode argument, bool nested) {
                if (argument != null) {
                    var argType = argument.getUserData(typeof(ExpressionInfo)).Type;
                    if (argType != null && BytecodeHelper.isDelegateType(argType)) {
                        handleExpression(argument, null, nested);
                        return;
                    }
                }
                
                var generator = context.MethodGenerationContext.Generator;
                var typeInfo = CompilerHelper.createDelegateType(context, delegateType, method);
                var constructor = typeInfo.Methods.where(p => p.Name.equals("<init>")).first();
                BytecodeHelper.emitTypeof(generator, context.TypeSystem, typeInfo);
                BytecodeHelper.emitIntConstant(generator, constructor.Parameters.count());
                BytecodeHelper.emitNewarray(generator, 1, context.TypeSystem.ClassType);
                if (!method.IsStatic) {
                    generator.emit(Opcode.Dup);
                    BytecodeHelper.emitIntConstant(generator, 0);
                    BytecodeHelper.emitTypeof(generator, context.TypeSystem, constructor.Parameters.first().Type);
                    generator.emit(Opcode.Aastore);
                }
                generator.emit(Opcode.Invokevirtual, statementGenerator.GetDeclaredConstructorMethod);
                BytecodeHelper.emitIntConstant(generator, (method.IsStatic) ? 0 : 1);
                BytecodeHelper.emitNewarray(generator, 1, statementGenerator.ExpressionType);
                if (!method.IsStatic) {
                    generator.emit(Opcode.Dup);
                    BytecodeHelper.emitIntConstant(generator, 0);
                    handleExpression(argument, null, true);
                    generator.emit(Opcode.Aastore);
                }
                generator.emit(Opcode.Aconst_Null);
                generator.emit(Opcode.Invokestatic, statementGenerator.NewObjectMethod);
            }
        }
    }
}
