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
using stab.lang;

namespace cnatural.syntaxtree {

    public class QueryScope {
        QueryScope next;
        private String identifier;
        private HashSet<String> names;
        private HashMap<String, SourceInfo> sourceInfos;
        
        QueryScope(String identifier, QueryScope next) {
            this.next = next;
            this.identifier = identifier;
            this.names = new HashSet<String>();
            this.sourceInfos = new HashMap<String, SourceInfo>();
        }
        
        void declareName(String name, String filename, int line, IntIterable disabledWarnings) {
            names.add(name);
            sourceInfos[name] = new SourceInfo(filename, line, disabledWarnings);
        }
        
        ExpressionNode getQualifiedName(String name, SimpleNameExpressionNode simpleName) {
            var s = this;
            var isDeclared = false;
            SourceInfo infos = null;
            int n = 0;
            do {
                n++;
                if (s.names.contains(name)) {
                    isDeclared = true;
                    infos = s.sourceInfos[name];
                    break;
                }
            } while ((s = s.next) != null);

            if (!isDeclared) {
                return simpleName;
            }
            ExpressionNode target = null;
            s = this;
            for (int i = 0; i < n; i++, s = s.next) {
                var member = new SimpleNameExpressionNode { Name = s.identifier };
                if (target == null) {
                    target = member;
                } else {
                    target = new MemberAccessExpressionNode { TargetObject = target, Member = member };
                }
                target.Filename = infos.filename;
                target.Line = infos.line;
                target.DisabledWarnings = infos.disabledWarnings;
            }
            var member = new SimpleNameExpressionNode { Name = name };
            member.Filename = infos.filename;
            member.Line = infos.line;
            member.DisabledWarnings = infos.disabledWarnings;
            target = new MemberAccessExpressionNode { TargetObject = target, Member = member };
            target.Filename = infos.filename;
            target.Line = infos.line;
            target.DisabledWarnings = infos.disabledWarnings;
            return target;
        }
        
        private class SourceInfo {
            String filename;
            int line;
            IntIterable disabledWarnings;
            
            SourceInfo(String filename, int line, IntIterable disabledWarnings) {
                this.filename = filename;
                this.line = line;
                this.disabledWarnings = disabledWarnings;
            }
        }
    }
    
    public class QueryTranslator : StatementHandler<QueryScope, Void> {
        private ExpressionTranslator expressionTranslator;
        int transparentIdentifierCount;
    
        public QueryTranslator(char[] text)
            : super(true) {
            this.expressionTranslator = new ExpressionTranslator(text, this);
        }

        public void translate(StatementNode statement) {
            transparentIdentifierCount = 0;
            handleStatement(statement, null);
        }
        
        public ExpressionNode translate(ExpressionNode statement, bool resetIdentifierCount) {
            if (resetIdentifierCount) {
                transparentIdentifierCount = 0;
            }
            return expressionTranslator.handleExpression(statement, null, false);
        }
        
        protected override Void handleBlock(BlockStatementNode block, QueryScope scope) {
            foreach (var s in block.Statements) {
                handleStatement(s, scope);
            }
            return null;
        }
        
        protected override Void handleBreak(BreakStatementNode breakStatement, QueryScope scope) {
            return null;
        }

        protected override Void handleContinue(ContinueStatementNode continueStatement, QueryScope scope) {
            return null;
        }

        protected override Void handleDo(DoStatementNode doStatement, QueryScope scope) {
            handleStatement(doStatement.Statement, scope);
            doStatement.Condition = expressionTranslator.handleExpression(doStatement.Condition, scope, false);
            return null;
        }
        
        protected override Void handleEmpty(EmptyStatementNode empty, QueryScope scope) {
            return null;
        }
        
        protected override Void handleExpression(ExpressionStatementNode expression, QueryScope scope) {
            expression.Expression = expressionTranslator.handleExpression(expression.Expression, scope, false);
            return null;
        }

        protected override Void handleFor(ForStatementNode forStatement, QueryScope scope) {
            foreach (var s in forStatement.Initializer) {
                handleStatement(s, null);
            }
            var condition = forStatement.Condition;
            if (condition != null) {
                forStatement.Condition = expressionTranslator.handleExpression(condition, scope, false);
            }
            foreach (var s in forStatement.Iterator) {
                handleStatement(s, scope);
            }
            handleStatement(forStatement.Statement, scope);
            return null;
        }
        
        protected override Void handleForeach(ForeachStatementNode foreachStatement, QueryScope scope) {
            foreachStatement.Source = expressionTranslator.handleExpression(foreachStatement.Source, scope, false);
            handleStatement(foreachStatement.Statement, scope);
            return null;
        }

        protected override Void handleGoto(GotoStatementNode gotoStatement, QueryScope scope) {
            return null;
        }
        
        protected override Void handleGotoCase(GotoCaseStatementNode gotoCase, QueryScope scope) {
            return null;
        }

        protected override Void handleIf(IfStatementNode ifStatement, QueryScope scope) {
            ifStatement.Condition = expressionTranslator.handleExpression(ifStatement.Condition, scope, false);
            handleStatement(ifStatement.IfTrue, scope);
            if (ifStatement.IfFalse != null) {
                handleStatement(ifStatement.IfFalse, scope);
            }
            return null;
        }
        
        protected override Void handleLabeled(LabeledStatementNode labeled, QueryScope scope) {
            handleStatement(labeled.Statement, null);
            return null;
        }
        
        protected override Void handleLocalDeclaration(LocalDeclarationStatementNode localDeclaration, QueryScope scope) {
            foreach (var decl in localDeclaration.Declarators) {
                if (decl.Value != null) {
                    decl.Value = expressionTranslator.handleExpression(decl.Value, scope, false);
                }
            }
            return null;
        }
        
        protected override Void handleReturn(ReturnStatementNode returnStatement, QueryScope scope) {
            if (returnStatement.Value != null) {
                returnStatement.Value = expressionTranslator.handleExpression(returnStatement.Value, scope, false);
            }
            return null;
        }
        
        protected override Void handleSwitch(SwitchStatementNode switchStatement, QueryScope scope) {
            switchStatement.Selector = expressionTranslator.handleExpression(switchStatement.Selector, scope, false);
            foreach (var section in switchStatement.Sections) {
                foreach (var s in section.Statements) {
                    handleStatement(s, scope);
                }
            }
            return null;
        }
        
        protected override Void handleSynchronized(SynchronizedStatementNode synchronizedStatement, QueryScope scope) {
            synchronizedStatement.Lock = expressionTranslator.handleExpression(synchronizedStatement.Lock, scope, false);
            handleStatement(synchronizedStatement.Statement, scope);
            return null;
        }

        protected override Void handleThrow(ThrowStatementNode throwStatement, QueryScope scope) {
            if (throwStatement.Exception != null) {
                throwStatement.Exception = expressionTranslator.handleExpression(throwStatement.Exception, scope, false);
            }
            return null;
        }
        
        protected override Void handleTry(TryStatementNode tryStatement, QueryScope scope) {
            handleStatement(tryStatement.Block, scope);
            foreach (var clause in tryStatement.CatchClauses) {
                foreach (var s in clause.Block.Statements) {
                    handleStatement(s, scope);
                }
            }
            if (tryStatement.Finally != null) {
                handleStatement(tryStatement.Finally, scope);
            }
            return null;
        }

        protected override Void handleUsing(UsingStatementNode usingStatement, QueryScope scope) {
            handleStatement(usingStatement.ResourceAcquisition, scope);
            handleStatement(usingStatement.Statement, scope);
            return null;
        }
        
        protected override Void handleWhile(WhileStatementNode whileStatement, QueryScope scope) {
            whileStatement.Condition = expressionTranslator.handleExpression(whileStatement.Condition, scope, false);
            handleStatement(whileStatement.Statement, scope);
            return null;
        }
        
        protected override Void handleYield(YieldStatementNode yieldStatement, QueryScope scope) {
            if (yieldStatement.Value != null) {
                yieldStatement.Value = expressionTranslator.handleExpression(yieldStatement.Value, scope, false);
            }
            return null;
        }
        
        private class ExpressionTranslator : ExpressionHandler<QueryScope, ExpressionNode> {
            private QueryTranslator statementTranslator;
            private char[] text;
            
            ExpressionTranslator(char[] text, QueryTranslator statementTranslator)
                : super(true) {
                this.statementTranslator = statementTranslator;
                this.text = text;
            }
            
            protected override ExpressionNode handleAnonymousObjectCreation(AnonymousObjectCreationExpressionNode anonymousObject,
                    QueryScope scope, bool nested) {
                foreach (var decl in anonymousObject.MemberDeclarators) {
                    decl.Value = handleExpression(decl.Value, scope, nested);
                }
                return anonymousObject;
            }
            
            protected override ExpressionNode handleArrayCreation(ArrayCreationExpressionNode arrayCreation, QueryScope scope, bool nested) {
                var dimensions = arrayCreation.DimensionExpressions;
                var size = dimensions.size();
                for (int i = 0; i < size; i++) {
                    dimensions[i] = handleExpression(dimensions[i], scope, nested);
                }
                if (arrayCreation.Initializer != null) {
                    handleExpression(arrayCreation.Initializer, scope, nested);
                }
                return arrayCreation;
            }
            
            protected override ExpressionNode handleArrayInitializer(ArrayInitializerExpressionNode arrayInitializer, QueryScope scope,
                    bool nested) {
                var values = arrayInitializer.Values;
                var size = values.size();
                for (int i = 0; i < size; i++) {
                    values[i] = handleExpression(values[i], scope, nested);
                }
                return arrayInitializer;
            }
            
            protected override ExpressionNode handleAssign(AssignExpressionNode assign, QueryScope scope, bool nested) {
                assign.Left = handleExpression(assign.Left, scope, nested);
                assign.Right = handleExpression(assign.Right, scope, nested);
                return assign;
            }
            
            protected override ExpressionNode handleBinary(BinaryExpressionNode binary, QueryScope scope, bool nested) {
                binary.LeftOperand = handleExpression(binary.LeftOperand, scope, nested);
                binary.RightOperand = handleExpression(binary.RightOperand, scope, nested);
                return binary;
            }
            
            protected override ExpressionNode handleCast(CastExpressionNode cast, QueryScope scope, bool nested) {
                cast.Expression = handleExpression(cast.Expression, scope, nested);
                return cast;
            }
            
            protected override ExpressionNode handleCollectionInitializer(CollectionInitializerExpressionNode initializer, QueryScope scope,
                    bool nested) {
                foreach (var args in initializer.Values) {
                    int size = args.size();
                    for (int i = 0; i < size; i++) {
                        args[i] = handleExpression(args[i], scope, nested);
                    }
                }
                return initializer;
            }
            
            protected override ExpressionNode handleConditional(ConditionalExpressionNode conditional, QueryScope scope, bool nested) {
                conditional.Condition = handleExpression(conditional.Condition, scope, nested);
                conditional.IfTrue = handleExpression(conditional.IfTrue, scope, nested);
                conditional.IfFalse = handleExpression(conditional.IfFalse, scope, nested);
                return conditional;
            }
            
            protected override ExpressionNode handleElementAccess(ElementAccessExpressionNode elementAccess, QueryScope scope, bool nested) {
                elementAccess.TargetObject = handleExpression(elementAccess.TargetObject, scope, nested);
                var indexes = elementAccess.Indexes;
                var size = indexes.size();
                for (int i = 0; i < size; i++) {
                    indexes[i] = handleExpression(indexes[i], scope, nested);
                }
                return elementAccess;
            }
            
            protected override ExpressionNode handleInvocation(InvocationExpressionNode invocation, QueryScope scope, bool nested) {
                invocation.TargetObject = handleExpression(invocation.TargetObject, scope, nested);
                var args = invocation.Arguments;
                var size = args.size();
                for (int i = 0; i < size; i++) {
                    args[i] = handleExpression(args[i], scope, nested);
                }
                return invocation;
            }
            
            protected override ExpressionNode handleLambda(LambdaExpressionNode lambda, QueryScope scope, bool nested) {
                statementTranslator.handleStatement(lambda.Body, scope);
                return lambda;
            }
            
            protected override ExpressionNode handleLiteral(LiteralExpressionNode literal, QueryScope scope, bool nested) {
                return literal;
            }
            
            protected override ExpressionNode handleMemberAccess(MemberAccessExpressionNode memberAccess, QueryScope scope, bool nested) {
                memberAccess.TargetObject = handleExpression(memberAccess.TargetObject, scope, nested);
                return memberAccess;
            }
            
            protected override ExpressionNode handleObjectCreation(ObjectCreationExpressionNode objectCreation, QueryScope scope, bool nested) {
                var args = objectCreation.Arguments;
                var size = args.size();
                for (int i = 0; i < size; i++) {
                    args[i] = handleExpression(args[i], scope, nested);
                }
                if (objectCreation.Initializer != null) {
                    handleExpression(objectCreation.Initializer, scope, nested);
                }
                return objectCreation;
            }
            
            protected override ExpressionNode handleObjectInitializer(ObjectInitializerExpressionNode initializer, QueryScope scope,
                    bool nested) {
                foreach (var mi in initializer.MemberInitializers) {
                    mi.Value = handleExpression(mi.Value, scope, nested);
                }
                return initializer;
            }
            
            protected override ExpressionNode handleQuery(QueryExpressionNode query, QueryScope scope, bool nested) {
                var from = query.From;
                from.Origin = handleExpression(from.Origin, scope, true);
                var body = query.Body;
                var name = new String(text, from.NameOffset, from.NameLength);
                var result = translateQuery(scope, from.Type, from.NameOffset, from.NameLength, name, from.Origin, body);
				result.addUserData(query);
				return result;
            }
            
            private ExpressionNode translateQuery(QueryScope scope, TypeReferenceNode type, int nameOffset, int nameLength, String name,
                    ExpressionNode origin, QueryBodyNode body) {
                for (;;) {
                    if (type != null) {
                        origin = generateCast(origin, type);
                    }

                    int n = 1;
                    int count = body.Clauses.size();
                    var skipSelect = false;
                    var whereAndOrderbyCount = 0;
                    foreach (var clause in body.Clauses) {
                        switch (clause.QueryBodyClauseKind) {
                        case Join: {
                            var joinClause = (JoinQueryBodyClauseNode)clause;
                            joinClause.Origin = handleExpression(joinClause.Origin, scope, false);
                            if (joinClause.Type != null) {
                                joinClause.Origin = generateCast(joinClause.Origin, joinClause.Type);
                            }
                            var isGroupJoin = joinClause.ResultLength > 0;
                            var member = new MemberAccessExpressionNode {
                                TargetObject = origin,
                                Member = new SimpleNameExpressionNode { Name = (isGroupJoin) ? "groupJoin" : "join" }
                            };
                            copyNodeInfos(clause, member);
                            var invocation = new InvocationExpressionNode { TargetObject = member };
                            copyNodeInfos(clause, invocation);

                            invocation.Arguments.add(joinClause.Origin);
                            
                            var expr = handleExpression(joinClause.EqualsLeftOperand, scope, false);
                            var lambdaBody = new ExpressionStatementNode { Expression = expr };
                            copyNodeInfos(clause, lambdaBody);
                            var lambda = new LambdaExpressionNode { Body = lambdaBody };
                            copyNodeInfos(clause, lambda);
                            var param = new ParameterNode { Name = name, NameOffset = nameOffset, NameLength = nameLength };
                            copyNodeInfos(clause, param);
                            lambda.Parameters.add(param);
                            invocation.Arguments.add(lambda);
                            
                            expr = handleExpression(joinClause.EqualsRightOperand, scope, false);
                            lambdaBody = new ExpressionStatementNode { Expression = expr };
                            copyNodeInfos(clause, lambdaBody);
                            lambda = new LambdaExpressionNode { Body = lambdaBody };
                            copyNodeInfos(clause, lambda);
                            var joinOffset = joinClause.NameOffset;
                            var joinLength = joinClause.NameLength;
                            var name2 = new String(text, joinOffset, joinLength);
                            param = new ParameterNode { Name = name2, NameOffset = joinOffset, NameLength = joinLength };
                            copyNodeInfos(clause, param);
                            lambda.Parameters.add(param);
                            invocation.Arguments.add(lambda);

                            String name3 = null;
                            var resultOffset = joinClause.ResultOffset;
                            var resultLength = joinClause.ResultLength;
                            if (isGroupJoin) {
                                name3 = new String(text, resultOffset, resultLength);
                            }
                            
                            lambda = new LambdaExpressionNode();
                            copyNodeInfos(clause, lambda);
                            param = new ParameterNode { Name = name, NameOffset = nameOffset, NameLength = nameLength };
                            copyNodeInfos(clause, param);
                            lambda.Parameters.add(param);
                            if (isGroupJoin) {
                                param = new ParameterNode { Name = name3, NameOffset = resultOffset, NameLength = resultLength };
                            } else {
                                param = new ParameterNode { Name = name2, NameOffset = joinOffset, NameLength = joinLength };
                            }
                            copyNodeInfos(clause, param);
                            lambda.Parameters.add(param);
                            invocation.Arguments.add(lambda);
                            
                            if (n == count && body.By == null) {
                                lambdaBody = new ExpressionStatementNode { Expression = handleExpression(body.SelectOrGroup, scope, false) };
                                copyNodeInfos(clause, lambdaBody);
                                lambda.Body = lambdaBody;
                                skipSelect = true;
                            } else {
                                var identifier = "query$id" + statementTranslator.transparentIdentifierCount++;
                                scope = new QueryScope(identifier, scope);
                                scope.declareName(name ?? new String(text, nameOffset, nameLength),
                                        clause.Filename, clause.Line, clause.DisabledWarnings);
                                if (isGroupJoin) {
                                    scope.declareName(name3, clause.Filename, clause.Line, clause.DisabledWarnings);
                                } else {
                                    scope.declareName(name2, clause.Filename, clause.Line, clause.DisabledWarnings);
                                }
                                
                                var aobj = new AnonymousObjectCreationExpressionNode();
                                copyNodeInfos(clause, aobj);
                                
                                var value = new SimpleNameExpressionNode { Name = name, NameOffset = nameOffset, NameLength = nameLength };
                                copyNodeInfos(clause, value);
                                var memberInit = new MemberInitializerNode { Value = value };
                                copyNodeInfos(clause, memberInit);
                                aobj.MemberDeclarators.add(memberInit);
                                
                                if (isGroupJoin) {
                                    value = new SimpleNameExpressionNode { Name = name3, NameOffset = resultOffset, NameLength = resultLength };
                                } else {
                                    value = new SimpleNameExpressionNode { Name = name2, NameOffset = joinOffset, NameLength = joinLength };
                                }
                                copyNodeInfos(clause, value);
                                memberInit = new MemberInitializerNode { Value = value };
                                copyNodeInfos(clause, memberInit);
                                aobj.MemberDeclarators.add(memberInit);

                                lambdaBody = new ExpressionStatementNode { Expression = aobj };
                                copyNodeInfos(clause, lambdaBody);
                                lambda.Body = lambdaBody;
                                
                                name = identifier;
                                nameOffset = 0;
                                nameLength = 0;
                            }
                            origin = invocation;
                            break;
                        }
                        case From: {
                            var fromClause = (FromQueryBodyClauseNode)clause;
                            fromClause.Origin = handleExpression(fromClause.Origin, scope, false);
                            if (fromClause.Type != null) {
                                fromClause.Origin = generateCast(fromClause.Origin, fromClause.Type);
                            }
                            var member = new MemberAccessExpressionNode {
                                TargetObject = origin,
                                Member = new SimpleNameExpressionNode { Name = "selectMany" }
                            };
                            copyNodeInfos(clause, member);
                            copyNodeInfos(clause, member.Member);
                            var invocation = new InvocationExpressionNode { TargetObject = member };
                            copyNodeInfos(clause, invocation);
                            
                            var lambdaBody = new ExpressionStatementNode { Expression = fromClause.Origin };
                            copyNodeInfos(clause, lambdaBody);
                            var lambda = new LambdaExpressionNode { Body = lambdaBody };
                            copyNodeInfos(clause, lambda);
                            var param = new ParameterNode { Name = name, NameOffset = nameOffset, NameLength = nameLength };
                            copyNodeInfos(clause, param);
                            lambda.Parameters.add(param);
                            invocation.Arguments.add(lambda);

                            lambda = new LambdaExpressionNode();
                            copyNodeInfos(clause, lambda);
                            param = new ParameterNode { Name = name, NameOffset = nameOffset, NameLength = nameLength };
                            copyNodeInfos(clause, param);
                            lambda.Parameters.add(param);
                            var fromOffset = fromClause.NameOffset;
                            var fromLength = fromClause.NameLength;
                            var name2 = new String(text, fromOffset, fromLength);
                            param = new ParameterNode { Name = name2, NameOffset = fromOffset, NameLength = fromLength };
                            copyNodeInfos(clause, param);
                            lambda.Parameters.add(param);
                            invocation.Arguments.add(lambda);

                            if (n == count && body.By == null) {
                                lambdaBody = new ExpressionStatementNode { Expression = handleExpression(body.SelectOrGroup, scope, false) };
                                copyNodeInfos(clause, lambdaBody);
                                lambda.Body = lambdaBody;
                                skipSelect = true;
                            } else {
                                var identifier = "query$id" + statementTranslator.transparentIdentifierCount++;
                                scope = new QueryScope(identifier, scope);
                                scope.declareName(name ?? new String(text, nameOffset, nameLength),
                                        clause.Filename, clause.Line, clause.DisabledWarnings);
                                scope.declareName(name2, clause.Filename, clause.Line, clause.DisabledWarnings);
                                
                                var aobj = new AnonymousObjectCreationExpressionNode();
                                copyNodeInfos(clause, aobj);
                                
                                var value = new SimpleNameExpressionNode { Name = name, NameOffset = nameOffset, NameLength = nameLength };
                                copyNodeInfos(clause, value);
                                var memberInit = new MemberInitializerNode { Value = value };
                                copyNodeInfos(clause, memberInit);
                                aobj.MemberDeclarators.add(memberInit);
                                
                                value = new SimpleNameExpressionNode { Name = name2, NameOffset = fromOffset, NameLength = fromLength };
                                copyNodeInfos(clause, value);
                                memberInit = new MemberInitializerNode { Value = value };
                                copyNodeInfos(clause, memberInit);
                                aobj.MemberDeclarators.add(memberInit);

                                lambdaBody = new ExpressionStatementNode { Expression = aobj };
                                copyNodeInfos(clause, lambdaBody);
                                lambda.Body = lambdaBody;
                                
                                name = identifier;
                                nameOffset = 0;
                                nameLength = 0;
                            }
                            origin = invocation;
                            break;
                        }
                        case Let: {
                            var letClause = (LetQueryBodyClauseNode)clause;
                            var member = new MemberAccessExpressionNode {
                                TargetObject = origin,
                                Member = new SimpleNameExpressionNode { Name = "select" }
                            };
                            copyNodeInfos(clause, member);
                            copyNodeInfos(clause, member.Member);
                            var invocation = new InvocationExpressionNode { TargetObject = member };
                            copyNodeInfos(clause, invocation);

                            var expr = handleExpression(letClause.Value, scope, false);
                            
                            var identifier = "query$id" + statementTranslator.transparentIdentifierCount++;
                            scope = new QueryScope(identifier, scope);
                            scope.declareName(name ?? new String(text, nameOffset, nameLength),
                                    clause.Filename, clause.Line, clause.DisabledWarnings);
                            var letOffset = letClause.NameOffset;
                            var letLength = letClause.NameLength;
                            var name2 = new String(text, letOffset, letLength);
                            scope.declareName(name2, clause.Filename, clause.Line, clause.DisabledWarnings);
                                
                            var aobj = new AnonymousObjectCreationExpressionNode();
                            copyNodeInfos(clause, aobj);
                            
                            var value = new SimpleNameExpressionNode { Name = name, NameOffset = nameOffset, NameLength = nameLength };
                            copyNodeInfos(clause, value);
                            var memberInit = new MemberInitializerNode { Value = value };
                            copyNodeInfos(clause, memberInit);
                            aobj.MemberDeclarators.add(memberInit);
                            
                            memberInit = new MemberInitializerNode { NameOffset = letOffset, NameLength = letLength, Value = expr };
                            copyNodeInfos(clause, memberInit);
                            aobj.MemberDeclarators.add(memberInit);
                            
                            var lambdaBody = new ExpressionStatementNode { Expression = aobj };
                            copyNodeInfos(clause, lambdaBody);
                            var lambda = new LambdaExpressionNode { Body = lambdaBody };
                            copyNodeInfos(clause, lambda);
                            var param = new ParameterNode { Name = name, NameOffset = nameOffset, NameLength = nameLength };
                            copyNodeInfos(clause, param);
                            lambda.Parameters.add(param);
                            invocation.Arguments.add(lambda);
                            
                            name = identifier;
                            nameOffset = 0;
                            nameLength = 0;
                            origin = invocation;
                            break;
                        }
                        case Where: {
                            var whereClause = (WhereQueryBodyClauseNode)clause;
                            var member = new MemberAccessExpressionNode {
                                TargetObject = origin,
                                Member = new SimpleNameExpressionNode { Name = "where" }
                            };
                            copyNodeInfos(clause, member);
                            copyNodeInfos(clause, member.Member);
                            var invocation = new InvocationExpressionNode { TargetObject = member };
                            copyNodeInfos(clause, invocation);

                            var expr = handleExpression(whereClause.Predicat, scope, false);
                            var lambdaBody = new ExpressionStatementNode { Expression = expr };
                            copyNodeInfos(clause, lambdaBody);
                            var lambda = new LambdaExpressionNode { Body = lambdaBody };
                            copyNodeInfos(clause, lambda);
                            var param = new ParameterNode { Name = name, NameOffset = nameOffset, NameLength = nameLength };
                            copyNodeInfos(clause, param);
                            lambda.Parameters.add(param);
                            invocation.Arguments.add(lambda);

                            whereAndOrderbyCount++;
                            origin = invocation;
                            break;
                        }
                        case Orderby: {
                            var orderby = (OrderbyQueryBodyClauseNode)clause;
                            var first = true;
                            foreach (var ordering in orderby.Orderings) {
                                var method = (first) ? "orderBy" : "thenBy";
                                if (ordering.Descending) {
                                    method += "Descending";
                                }
                                var member = new MemberAccessExpressionNode {
                                    TargetObject = origin,
                                    Member = new SimpleNameExpressionNode { Name = method }
                                };
                                copyNodeInfos(clause, member);
                                copyNodeInfos(clause, member.Member);
                                var invocation = new InvocationExpressionNode { TargetObject = member };
                                copyNodeInfos(clause, invocation);

                                var expr = handleExpression(ordering.KeySelector, scope, false);
                                var lambdaBody = new ExpressionStatementNode { Expression = expr };
                                copyNodeInfos(clause, lambdaBody);
                                var lambda = new LambdaExpressionNode { Body = lambdaBody };
                                copyNodeInfos(clause, lambda);
                                var param = new ParameterNode { Name = name, NameOffset = nameOffset, NameLength = nameLength };
                                copyNodeInfos(clause, param);
                                lambda.Parameters.add(param);
                                invocation.Arguments.add(lambda);

                                origin = invocation;
                                first = false;
                            }
                            whereAndOrderbyCount++;
                            break;
                        }
                        default:
                            throw new Exception("Internal error");
                        }
                        n++;
                    }
                    
                    var continuation = body.Continuation;
                    if (body.By == null) {
                        if (!skipSelect) {
                            var generateSelect = true;
                            var select = body.SelectOrGroup;
                            if ((continuation != null || type != null || count > 0) && count == whereAndOrderbyCount) {
                                if (select.ExpressionKind == ExpressionKind.SimpleName) {
                                    var selectName = (SimpleNameExpressionNode)select;
                                    var sname = new String(text, selectName.NameOffset, selectName.NameLength);
                                    if (name == null) {
                                        name = new String(text, nameOffset, nameLength);
                                    }
                                    if (sname.equals(name)) {
                                        generateSelect = false;
                                    }
                                    name = sname;
                                }
                            }
                            if (generateSelect) {
                                var member = new MemberAccessExpressionNode {
                                    TargetObject = origin,
                                    Member = new SimpleNameExpressionNode { Name = "select" }
                                };
                                copyNodeInfos(body, member);
                                copyNodeInfos(body, member.Member);
                                var invocation = new InvocationExpressionNode { TargetObject = member };
                                copyNodeInfos(body, invocation);
                                var lambdaBody = new ExpressionStatementNode { Expression = handleExpression(select, scope, false) };
                                copyNodeInfos(body, lambdaBody);
                                var lambda = new LambdaExpressionNode { Body = lambdaBody };
                                copyNodeInfos(body, lambda);
                                var param = new ParameterNode { Name = name, NameOffset = nameOffset, NameLength = nameLength };
                                copyNodeInfos(body, param);
                                lambda.Parameters.add(param);
                                invocation.Arguments.add(lambda);
                                origin = invocation;
                            }
                        }
                    } else {
                        var group = body.SelectOrGroup;
                        var by = body.By;
                        var member = new MemberAccessExpressionNode {
                            TargetObject = origin,
                            Member = new SimpleNameExpressionNode { Name = "groupBy" }
                        };
                        copyNodeInfos(body, member);
                        copyNodeInfos(body, member.Member);
                        var invocation = new InvocationExpressionNode { TargetObject = member };
                        copyNodeInfos(body, invocation);

                        var lambdaBody = new ExpressionStatementNode { Expression = handleExpression(by, scope, false) };
                        copyNodeInfos(body, lambdaBody);
                        var lambda = new LambdaExpressionNode { Body = lambdaBody };
                        copyNodeInfos(body, lambda);
                        var param = new ParameterNode { Name = name, NameOffset = nameOffset, NameLength = nameLength };
                        copyNodeInfos(body, param);
                        lambda.Parameters.add(param);
                        invocation.Arguments.add(lambda);

                        var generateGroup = true;
                        if (group.ExpressionKind == ExpressionKind.SimpleName) {
                            var groupName = (SimpleNameExpressionNode)group;
                            var gname = new String(text, groupName.NameOffset, groupName.NameLength);
                            if (name == null) {
                                name = new String(text, nameOffset, nameLength);
                            }
                            if (gname.equals(name)) {
                                generateGroup = false;
                            }
                        }
                        if (generateGroup) {
                            lambdaBody = new ExpressionStatementNode { Expression = handleExpression(group, scope, false) };
                            copyNodeInfos(body, lambdaBody);
                            lambda = new LambdaExpressionNode { Body = lambdaBody };
                            copyNodeInfos(body, lambda);
                            param = new ParameterNode { Name = name, NameOffset = nameOffset, NameLength = nameLength };
                            copyNodeInfos(body, param);
                            lambda.Parameters.add(param);
                            invocation.Arguments.add(lambda);
                        }
                        origin = invocation;
                    }
                    
                    if (continuation != null) {
                        type = null;
                        name = new String(text, continuation.NameOffset, continuation.NameLength);
                        nameOffset = continuation.NameOffset;
                        nameLength = continuation.NameLength;
                        body = continuation.Body;
                    } else {
                        return origin;
                    }
                }
            }

            private static ExpressionNode generateCast(ExpressionNode origin, TypeReferenceNode type) {
                var member = new MemberAccessExpressionNode {
                    TargetObject = origin,
                    Member = new SimpleNameExpressionNode { Name = "cast" }
                };
                copyNodeInfos(origin, member);
                var invocation = new InvocationExpressionNode { TargetObject = member };
                copyNodeInfos(origin, invocation);
                var typeofNode = new TypeofExpressionNode { Type = type };
                copyNodeInfos(origin, typeofNode);
                invocation.Arguments.add(typeofNode);
                return invocation;
            }
            
            private static void copyNodeInfos(SyntaxNode from, SyntaxNode to) {
                to.Filename = from.Filename;
                to.Line = from.Line;
                to.DisabledWarnings = from.DisabledWarnings;
            }
            
            protected override ExpressionNode handleSimpleName(SimpleNameExpressionNode simpleName, QueryScope scope, bool nested) {
                if (scope == null) {
                    return simpleName;
                } else {
                    var name = simpleName.Name ?? new String(text, simpleName.NameOffset, simpleName.NameLength);
                    return scope.getQualifiedName(name, simpleName);
                }
            }
            
            protected override ExpressionNode handleSizeof(SizeofExpressionNode sizeofExpression, QueryScope scope, bool nested) {
                sizeofExpression.Expression = handleExpression(sizeofExpression.Expression, scope, nested);
                return sizeofExpression;
            }
            
            protected override ExpressionNode handleSuperAccess(SuperAccessExpressionNode superAccess, QueryScope scope, bool nested) {
                return superAccess;
            }
            
            protected override ExpressionNode handleThisAccess(ThisAccessExpressionNode thisAccess, QueryScope scope, bool nested) {
                return thisAccess;
            }
            
            protected override ExpressionNode handleType(TypeExpressionNode type, QueryScope scope, bool nested) {
                return type;
            }
            
            protected override ExpressionNode handleTypeof(TypeofExpressionNode typeofExpression, QueryScope scope, bool nested) {
                return typeofExpression;
            }
            
            protected override ExpressionNode handleUnary(UnaryExpressionNode unary, QueryScope scope, bool nested) {
                unary.Operand = handleExpression(unary.Operand, scope, nested);
                return unary;
            }
        }
    }
}
