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

    class StatementScope {
        StatementScope next;
        StatementNode statement;
        private HashMap<String, StatementNode> labels;
        
        StatementScope(StatementNode statement, StatementScope next) {
            this.statement = statement;
            this.next = next;
        }
        
        void declareLabel(String name, StatementNode statement) {
            if (labels == null) {
                labels = new HashMap<String, StatementNode>();
            }
            labels[name] = statement;
        }
        
        StatementNode getLabeledStatement(String name) {
            var s = this;
            do {
                if (s.labels != null) {
                    var ls = s.labels[name];
                    if (ls != null) {
                        return ls;
                    }
                }
            } while ((s = s.next) != null);
            return null;
        }
    }

    class InitializedLocalInfo {
        LocalMemberInfo local;
        LocalDeclarationStatementNode declaration;
        
        InitializedLocalInfo(LocalMemberInfo local, LocalDeclarationStatementNode declaration) {
            this.local = local;
            this.declaration = declaration;
        }
    }
    
    class UninitializedLocalInfo {
        LocalMemberInfo local;
        LocalDeclarationStatementNode declaration;
        int referenceCount;
        int assignmentCount;
        
        UninitializedLocalInfo(LocalMemberInfo local, LocalDeclarationStatementNode declaration) {
            this.local = local;
            this.declaration = declaration;
        }
    }
    
    class ReachabilityChecker : StatementHandler<Void, StatementInfo> {
        private CompilerContext context;
        private ExpressionChecker expressionChecker;
        private UnreachableStatementsHandler unreachableStatementsHandler;
        private HashMap<LocalMemberInfo, UninitializedLocalInfo> uninitializedLocals;
        private HashMap<LocalMemberInfo, InitializedLocalInfo> initializedLocals;
    
        ReachabilityChecker(CompilerContext context)
            : super(true) {
            this.context = context;
            this.expressionChecker = new ExpressionChecker(this, context);
            this.unreachableStatementsHandler = new UnreachableStatementsHandler(context);
        }
        
        Iterable<UninitializedLocalInfo> checkMethod(MethodInfo method, BlockStatementNode body, bool insertReturn) {
            uninitializedLocals = new HashMap<LocalMemberInfo, UninitializedLocalInfo>();
            initializedLocals = new HashMap<LocalMemberInfo, InitializedLocalInfo>();
            
            var info = handleStatement(body, null);
            if (insertReturn && info.IsEndPointReachable) {
                if (method.ReturnType == context.TypeSystem.VoidType) {
                    var returnStatement = new ReturnStatementNode();
                    returnStatement.addUserData(new StatementInfo());
                    body.Statements.add(returnStatement);
                } else if (context.Iterables[method] != null) {
                    var yieldStatement = new YieldStatementNode();
                    yieldStatement.addUserData(new StatementInfo());
                    body.Statements.add(yieldStatement);
                } else {
                    context.addError(CompileErrorId.MissingReturn, method.getUserData(typeof(SyntaxNode)));
                }
            }
            info.YieldCount = context.CodeValidationContext.YieldCount;
            context.CodeValidationContext.YieldCount = 0;

            foreach (var local in initializedLocals.values()) {
                context.addWarning(CompileErrorId.VariableNeverUsed, local.declaration, local.local.Name);
            }
            
            unreachableStatementsHandler.handleStatement(body, null);
            return uninitializedLocals.values();
        }
        
        static bool isEndPointReachable(StatementNode statement) {
            var info = statement.getUserData(typeof(StatementInfo));
            return (info == null) ? false : info.IsEndPointReachable;
        }
        
        protected override StatementInfo handleBlock(BlockStatementNode block, Void source) {
            var scope = new StatementScope(block, block.getUserData(typeof(StatementScope)));
            var info = new StatementInfo();
            block.addOrReplaceUserData(info);
            int size = block.Statements.size();
            if (size > 0) {
                visitStatements(block.Statements, scope);
                var reachable = isEndPointReachable(block.Statements[size - 1]);
                info.IsEndPointReachable = reachable;
                if (!reachable) {
                    block.removeUserData(typeof(StatementNode));
                }
            } else {
                info.IsEndPointReachable = true;
            }
            return info;
        }

        protected override StatementInfo handleBreak(BreakStatementNode breakStatement, Void source) {
            var info = new StatementInfo();
            breakStatement.addOrReplaceUserData(info);
            var done = false;
            var enclosingStatements = breakStatement.getUserData(typeof(StatementScope));
            while (enclosingStatements != null && !done) {
                var node = enclosingStatements.statement;
                switch (node.StatementKind) {
                case Do:
                case For:
                case Foreach:
                case Switch:
                case While:
                    done = true;
                    info.Target = node.getUserData(typeof(StatementNode));
                    breakStatement.removeUserData(typeof(StatementNode));
                    var sinfo = node.getUserData(typeof(StatementInfo));
                    sinfo.IsEndPointReachable = true;
                    if (info.Target != null) {
                        visitStatement(info.Target);
                        var tinfo = info.Target.getUserData(typeof(StatementInfo));
                        tinfo.IsTargeted = true;
                    } else {
                        node.getUserData(typeof(StatementInfo)).IsTargeted = true;
                    }
                    break;
                }
                enclosingStatements = enclosingStatements.next;
            }
            if (!done) {
                throw context.error(CompileErrorId.BreakOutsideLoop, breakStatement);
            }
            return info;
        }

        protected override StatementInfo handleContinue(ContinueStatementNode continueStatement, Void source) {
            var info = new StatementInfo();
            continueStatement.addOrReplaceUserData(info);
            var done = false;
            var enclosingStatements = continueStatement.getUserData(typeof(StatementScope));
            while (enclosingStatements != null && !done) {
                var node = enclosingStatements.statement;
                switch (node.StatementKind) {
                case Do:
                case For:
                case Foreach:
                case While:
                    done = true;
                    info.Target = node;
                    continueStatement.removeUserData(typeof(StatementNode));
                    var sinfo = node.getUserData(typeof(StatementInfo));
                    sinfo.IsTargeted = true;
                    break;
                }
                enclosingStatements = enclosingStatements.next;
            }
            if (!done) {
                throw context.error(CompileErrorId.ContinueOutsideLoop, continueStatement);
            }
            return info;
        }

        protected override StatementInfo handleDo(DoStatementNode doStatement, Void source) {
            var enclosingStatements = new StatementScope(doStatement, doStatement.getUserData(typeof(StatementScope)));
            var info = new StatementInfo();
            doStatement.addOrReplaceUserData(info);
            doStatement.Statement.addOrReplaceUserData(enclosingStatements);
            var cinfo = doStatement.Condition.getUserData(typeof(ExpressionInfo));
            if (cinfo.IsConstant) {
                var value = (Boolean)cinfo.getValue();
                info.IsEndPointReachable = !value;
                doStatement.Statement.addOrReplaceUserData(enclosingStatements);
                handleStatement(doStatement.Statement, null);
            } else {
                doStatement.Statement.addOrReplaceUserData(enclosingStatements);
                handleStatement(doStatement.Statement, null);
                if (!info.IsEndPointReachable) {
                    info.IsEndPointReachable = isEndPointReachable(doStatement.Statement);
                }
            }
            if (!info.IsEndPointReachable) {
                doStatement.removeUserData(typeof(StatementNode));
            }
            expressionChecker.handleExpression(doStatement.Condition, null, true);
            return info;
        }

        protected override StatementInfo handleEmpty(EmptyStatementNode empty, Void source) {
            var info = new StatementInfo();
            empty.addOrReplaceUserData(info);
            info.IsEndPointReachable = true;
            return info;
        }
        
        protected override StatementInfo handleExpression(ExpressionStatementNode expression, Void source) {
            var info = new StatementInfo();
            expression.addOrReplaceUserData(info);
            info.IsEndPointReachable = true;
            expressionChecker.handleExpression(expression.Expression, null, false);
            return info;
        }
        
        protected override StatementInfo handleFor(ForStatementNode forStatement, Void source) {
            var enclosingStatements = new StatementScope(forStatement, forStatement.getUserData(typeof(StatementScope)));
            var info = new StatementInfo();
            forStatement.addOrReplaceUserData(info);
            foreach (var s in forStatement.Initializer) {
                handleStatement(s, null);
            }
            if (forStatement.Iterator.any()) {
                linkStatements(Collections.singletonList<StatementNode>(forStatement.Statement).concat(forStatement.Iterator), enclosingStatements);
            }
            if (forStatement.Condition != null) {
                var cinfo = forStatement.Condition.getUserData(typeof(ExpressionInfo));
                if (cinfo.IsConstant) {
                    var value = (Boolean)cinfo.Value;
                    info.IsEndPointReachable = !value;
                    if (value) {
                        forStatement.Statement.addOrReplaceUserData(enclosingStatements);
                        visitStatement(forStatement.Statement);
                    }
                } else {
                    expressionChecker.handleExpression(forStatement.Condition, null, true);
                    info.IsEndPointReachable = true;
                    forStatement.Statement.addOrReplaceUserData(enclosingStatements);
                    visitStatement(forStatement.Statement);
                }
            } else {
                info.IsEndPointReachable = false;
                forStatement.Statement.addOrReplaceUserData(enclosingStatements);
                visitStatement(forStatement.Statement);
            }
            return info;
        }

        protected override StatementInfo handleForeach(ForeachStatementNode foreachStatement, Void source) {
            var enclosingStatements = new StatementScope(foreachStatement, foreachStatement.getUserData(typeof(StatementScope)));
            var info = new StatementInfo();
            foreachStatement.addOrReplaceUserData(info);
            foreachStatement.Statement.addOrReplaceUserData(enclosingStatements);
            info.IsEndPointReachable = true;
            expressionChecker.handleExpression(foreachStatement.Source, null, true);
            handleStatement(foreachStatement.Statement, null);
            return info;
        }

        protected override StatementInfo handleGoto(GotoStatementNode gotoStatement, Void source) {
            var info = new StatementInfo();
            gotoStatement.addOrReplaceUserData(info);
            gotoStatement.removeUserData(typeof(StatementNode));
            var enclosingStatements = gotoStatement.getUserData(typeof(StatementScope));
            var label = context.getIdentifier(gotoStatement.LabelOffset, gotoStatement.LabelLength);
            // TODO: check label redefinition
            var target = enclosingStatements.getLabeledStatement(label);
            if (target == null) {
                throw context.error(CompileErrorId.UnresolvedLabel, gotoStatement, label);
            }
            visitStatement(target);
            info.Target = target;
            var tinfo = target.getUserData(typeof(StatementInfo));
            tinfo.IsTargeted = true;
            return info;
        }

        protected override StatementInfo handleGotoCase(GotoCaseStatementNode gotoCase, Void source) {
            var info = new StatementInfo();
            gotoCase.addOrReplaceUserData(info);
            gotoCase.removeUserData(typeof(StatementNode));
            var done = false;
            List<StatementNode> target = null;
            var enclosingStatements = gotoCase.getUserData(typeof(StatementScope));
            while (enclosingStatements != null && !done) {
                var node = enclosingStatements.statement;
                if (node.StatementKind == StatementKind.Switch) {
                    done = true;
                    var addNext = false;
                    var gotoDefault = gotoCase.Expression == null;
                    var sinfo = (gotoDefault) ? null : gotoCase.Expression.getUserData(typeof(ExpressionInfo));
                    String enumField = null;
                    if (!gotoDefault && sinfo == null) {
                        var name = (SimpleNameExpressionNode)gotoCase.Expression;
                        enumField = context.getIdentifier(name.NameOffset, name.NameLength);
                    }
                    foreach (var section in ((SwitchStatementNode)node).Sections) {
                        if (addNext) {
                            if (!section.Statements.isEmpty()) {
                                target = section.Statements;
                                break;
                            }
                        } else {
                            if (section.CaseExpression == null) {
                                if (gotoDefault) {
                                    if (section.Statements.isEmpty()) {
                                        addNext = true;
                                    } else {
                                        target = section.Statements;
                                        break;
                                    }
                                }
                            } else if (gotoCase.Expression != null) {
                                var ordinalValue = section.CaseExpression.getUserData(typeof(Integer));
                                if (ordinalValue != null) {
                                    var name = (SimpleNameExpressionNode)section.CaseExpression;
                                    var label = context.getIdentifier(name.NameOffset, name.NameLength);
                                    if (label.equals(enumField)) {
                                        if (section.Statements.isEmpty()) {
                                            addNext = true;
                                        } else {
                                            target = section.Statements;
                                            break;
                                        }
                                    }
                                } else {
                                    var cinfo = section.CaseExpression.getUserData(typeof(ExpressionInfo));
                                    if (sinfo == null) {
                                        if (cinfo == null) {
                                            if (section.Statements.isEmpty()) {
                                                addNext = true;
                                            } else {
                                                target = section.Statements;
                                                break;
                                            }
                                        }
                                    } else if (cinfo != null) {
                                        if (sinfo.Type.IsNumeric) {
                                            int svalue;
                                            if (sinfo.Value instanceof Character) {
                                                svalue = ((Character)sinfo.Value).charValue();
                                            } else {
                                                svalue = ((Number)sinfo.Value).intValue();
                                            }
                                            int cvalue;
                                            if (cinfo.Value instanceof Character) {
                                                cvalue = ((Character)cinfo.Value).charValue();
                                            } else {
                                                cvalue = ((Number)cinfo.Value).intValue();
                                            }
                                            if (svalue == cvalue) {
                                                if (section.Statements.isEmpty()) {
                                                    addNext = true;
                                                } else {
                                                    target = section.Statements;
                                                    break;
                                                }
                                            }
                                        } else if (sinfo.Value.equals(cinfo.Value)) {
                                            if (section.Statements.isEmpty()) {
                                                addNext = true;
                                            } else {
                                                target = section.Statements;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                enclosingStatements = enclosingStatements.next;
            }
            if (!done) {
                throw context.error(CompileErrorId.InvalidGotoCase, gotoCase);
            }
            if (target == null) {
                throw context.error(CompileErrorId.UnknownGotoLabel, gotoCase);
            }
            visitStatement(target[0]);
            info.Target = target[0];
            var tinfo = target[0].getUserData(typeof(StatementInfo));
            tinfo.IsTargeted = true;
            return info;
        }

        protected override StatementInfo handleIf(IfStatementNode ifStatement, Void source) {
            var scope = new StatementScope(ifStatement, ifStatement.getUserData(typeof(StatementScope)));
            var info = new StatementInfo();
            var condition = ifStatement.Condition;
            var ifTrue = ifStatement.IfTrue;
            var ifFalse = ifStatement.IfFalse;
            var cinfo = condition.getUserData(typeof(ExpressionInfo));
            
            ifStatement.addOrReplaceUserData(info);
            ifTrue.addOrReplaceUserData(scope);
            if (ifFalse != null) {
                ifFalse.addOrReplaceUserData(scope);
            }
            
            if (cinfo.IsConstant) {
                if ((Boolean)cinfo.Value) {
                    var tinfo = handleStatement(ifTrue, null);
                    info.IsEndPointReachable = tinfo.IsEndPointReachable;
                } else {
					if (ifFalse != null) {
						var finfo = handleStatement(ifFalse, null);
						info.IsEndPointReachable = finfo.IsEndPointReachable;
					} else {
						info.IsEndPointReachable = true;
					}
                }
            } else {
                expressionChecker.handleExpression(condition, null, true);
                var tinfo = handleStatement(ifTrue, null);
                if (ifFalse != null) {
                    var finfo = handleStatement(ifFalse, null);
                    info.IsEndPointReachable = tinfo.IsEndPointReachable || finfo.IsEndPointReachable;
                } else {
                    info.IsEndPointReachable = true;
                }
            }
            return info;
        }
        
        protected override StatementInfo handleLabeled(LabeledStatementNode labeled, Void source) {
            var info = new StatementInfo();
            labeled.addOrReplaceUserData(info);
            var statement = labeled.Statement;
            statement.addOrReplaceUserData(labeled.getUserData(typeof(StatementScope)));
            handleStatement(statement, null);
            info.IsEndPointReachable = statement.getUserData(typeof(StatementInfo)).IsEndPointReachable;
            return info;
        }
        
        protected override StatementInfo handleLocalDeclaration(LocalDeclarationStatementNode localDeclaration, Void source) {
            var info = new StatementInfo();
            localDeclaration.addOrReplaceUserData(info);
            info.IsEndPointReachable = true;
            foreach (var decl in localDeclaration.Declarators) {
                var local = decl.getUserData(typeof(LocalMemberInfo));
                if (decl.Value != null) {
                    initializedLocals[local] = new InitializedLocalInfo(local, localDeclaration);
                    expressionChecker.handleExpression(decl.Value, null, true);
                } else {
                    uninitializedLocals[local] = new UninitializedLocalInfo(local, localDeclaration);
                }
            }
            return info;
        }
        
        protected override StatementInfo handleReturn(ReturnStatementNode returnStatement, Void source) {
            if (context.Iterables[context.CodeValidationContext.CurrentMethod] != null) {
                context.addError(CompileErrorId.ReturnInsideIterator, returnStatement);
            }
            returnStatement.removeUserData(typeof(StatementNode));
            var info = new StatementInfo();
            returnStatement.addOrReplaceUserData(info);
            if (returnStatement.Value != null) {
                expressionChecker.handleExpression(returnStatement.Value, null, true);
            }
            return info;
        }
        
        protected override StatementInfo handleSwitch(SwitchStatementNode switchStatement, Void source) {
            var enclosingStatements = new StatementScope(switchStatement, switchStatement.getUserData(typeof(StatementScope)));
            var info = new StatementInfo();
            switchStatement.addOrReplaceUserData(info);
            expressionChecker.handleExpression(switchStatement.Selector, null, true);
            if (switchStatement.Sections.isEmpty()) {
                info.IsEndPointReachable = true;
            } else {
                var sinfo = switchStatement.Selector.getUserData(typeof(ExpressionInfo));
                var isConstant = sinfo.IsConstant;
                var reachableStatements = new ArrayList<List<StatementNode>>();
                List<StatementNode> defaultStatements = null;
                var hasReachableCase = false;
                var addNext = false;
                foreach (var section in switchStatement.Sections) {
                    if (section.Statements.isEmpty()) {
                        continue;
                    }
                    linkStatements(section.Statements, enclosingStatements);
                    if (section.CaseExpression == null) {
                        defaultStatements = section.Statements;
                        addNext = section.Statements.isEmpty();
                    } else if (isConstant && !addNext) {
                        var einfo = section.CaseExpression.getUserData(typeof(ExpressionInfo));
                        if (einfo.Value.equals(sinfo.Value)) {
                            reachableStatements.add(section.Statements);
                            hasReachableCase = true;
                            addNext = section.Statements.isEmpty();
                        }
                    } else {
                        hasReachableCase = true;
                        reachableStatements.add(section.Statements);
                        addNext = section.Statements.isEmpty();
                    }
                }
                if ((!isConstant || !hasReachableCase) && defaultStatements != null && defaultStatements.size() > 0) {
                    reachableStatements.add(defaultStatements);
                }
                foreach (var stmts in reachableStatements) {
                    visitStatement(stmts[0]);
                    if (isEndPointReachable(stmts[stmts.size() - 1])) {
                        throw context.error(CompileErrorId.CaseFallThrough, stmts[stmts.size() - 1]);
                    }
                }
                if (!info.IsEndPointReachable && !isConstant) {
                    if (defaultStatements == null) {
                        info.IsEndPointReachable = true;
                    }
                }
                if (hasReachableCase) {
                    if (sinfo.Type.IsNumeric) {
                        var sections = new ArrayList<SwitchSectionNode>();
                        var cases = new ArrayList<Number>();
                        foreach (var section in switchStatement.Sections) {
                            if (section.CaseExpression == null) {
                                continue;
                            }
                            if (section.Statements.isEmpty() || section.Statements[0].getUserData(typeof(StatementInfo)) != null) {
                                sections.add(section);
                                var si =  section.CaseExpression.getUserData(typeof(ExpressionInfo));
                                Object caseValue = si.Value;
                                if (caseValue instanceof Character) {
                                    var nb = Integer.valueOf(((Character)caseValue).charValue());
                                    si.Value = nb;
                                    cases.add(nb);
                                } else {
                                    cases.add((Number)caseValue);
                                }
                            }
                        }
                        for (int i = 0; i < cases.size(); i++) {
                            int value = cases[i].intValue();
                            for (int j = i + 1; j < cases.size(); j++) {
                                if (value == cases[j].intValue()) {
                                    throw context.error(CompileErrorId.DuplicateCase, sections[j]);
                                }
                            }
                        }
                    } else if (!sinfo.Type.IsEnum) {
                        var sections = new ArrayList<SwitchSectionNode>();
                        var cases = new ArrayList<String>();
                        foreach (var section in switchStatement.Sections) {
                            if (section.CaseExpression == null) {
                                continue;
                            }
                            if (section.Statements.isEmpty() || section.Statements[0].getUserData(typeof(StatementInfo)) != null) {
                                sections.add(section);
                                var cinfo = section.CaseExpression.getUserData(typeof(ExpressionInfo));
                                if (cinfo == null) {
                                    cases.add(null);
                                } else {
                                    cases.add((String)cinfo.Value);
                                }
                            }
                        }
                        for (int i = 0; i < cases.size(); i++) {
                            String value = cases[i];
                            for (int j = i + 1; j < cases.size(); j++) {
                                if ((value == null && cases[j] == null) ||  (value != null && value.equals(cases[j]))) {
                                    throw context.error(CompileErrorId.DuplicateCase, sections[j]);
                                }
                            }
                        }
                    }
                }
            }
            return info;
        }

        protected override StatementInfo handleSynchronized(SynchronizedStatementNode synchronizedStatement, Void source) {
            var enclosingStatements = new StatementScope(synchronizedStatement, synchronizedStatement.getUserData(typeof(StatementScope)));
            var info = new StatementInfo();
            synchronizedStatement.addOrReplaceUserData(info);
            expressionChecker.handleExpression(synchronizedStatement.Lock, null, true);
            synchronizedStatement.Statement.addOrReplaceUserData(enclosingStatements);
            handleStatement(synchronizedStatement.Statement, null);
            info.IsEndPointReachable = synchronizedStatement.Statement.getUserData(typeof(StatementInfo)).IsEndPointReachable;
            return info;
        }

        protected override StatementInfo handleThrow(ThrowStatementNode throwStatement, Void source) {
            var info = new StatementInfo();
            throwStatement.addOrReplaceUserData(info);
            if (throwStatement.Exception != null) {
                expressionChecker.handleExpression(throwStatement.Exception, null, true);
            }
            throwStatement.removeUserData(typeof(StatementNode));
            return info;
        }
        
        protected override StatementInfo handleTry(TryStatementNode tryStatement, Void source) {
            var enclosingStatements = new StatementScope(tryStatement, tryStatement.getUserData(typeof(StatementScope)));
            var info = new StatementInfo();
            tryStatement.addOrReplaceUserData(info);
            tryStatement.Block.addOrReplaceUserData(enclosingStatements);
            handleStatement(tryStatement.Block, null);
            bool isEndPointReachable = tryStatement.Block.getUserData(typeof(StatementInfo)).IsEndPointReachable;
            foreach (var node in tryStatement.CatchClauses) {
                var stmts = node.Block.Statements;
                if (stmts.size() > 0) {
                    visitStatements(stmts, enclosingStatements);
                    if (!isEndPointReachable) {
                        isEndPointReachable = stmts[stmts.size() - 1].getUserData(typeof(StatementInfo)).IsEndPointReachable;
                    }
                }
            }
            if (tryStatement.Finally != null) {
                handleStatement(tryStatement.Finally, source);
                tryStatement.Finally.getUserData(typeof(StatementInfo)).IsEndPointReachable = false;
            }
            info.IsEndPointReachable = isEndPointReachable;
            return info;
        }

        protected override StatementInfo handleUsing(UsingStatementNode usingStatement, Void source) {
            var enclosingStatements = new StatementScope(usingStatement, usingStatement.getUserData(typeof(StatementScope)));
            var info = new StatementInfo();
            usingStatement.addOrReplaceUserData(info);
            usingStatement.ResourceAcquisition.addOrReplaceUserData(enclosingStatements);
            usingStatement.Statement.addOrReplaceUserData(enclosingStatements);
            handleStatement(usingStatement.ResourceAcquisition, null);
            handleStatement(usingStatement.Statement, null);
            info.IsEndPointReachable = usingStatement.Statement.getUserData(typeof(StatementInfo)).IsEndPointReachable;
            return info;
        }

        protected override StatementInfo handleWhile(WhileStatementNode whileStatement, Void source) {
            var enclosingStatements = new StatementScope(whileStatement, whileStatement.getUserData(typeof(StatementScope)));
            var info = new StatementInfo();
            whileStatement.addOrReplaceUserData(info);
            whileStatement.Statement.addOrReplaceUserData(enclosingStatements);
            var cinfo = whileStatement.Condition.getUserData(typeof(ExpressionInfo));
            if (cinfo.IsConstant) {
                var value = (Boolean)cinfo.Value;
                info.IsEndPointReachable = !value;
                if (value) {
                    whileStatement.Statement.addOrReplaceUserData(enclosingStatements);
                    handleStatement(whileStatement.Statement, null);
                }
            } else {
                expressionChecker.handleExpression(whileStatement.Condition, null, true);
                info.IsEndPointReachable = true;
                whileStatement.Statement.addOrReplaceUserData(enclosingStatements);
                handleStatement(whileStatement.Statement, null);
            }
            return info;
        }
        
        protected override StatementInfo handleYield(YieldStatementNode yieldStatement, Void source) {
            var info = new StatementInfo();
            info.IsEndPointReachable = yieldStatement.Value != null;
            yieldStatement.addOrReplaceUserData(info);
            if (yieldStatement.Value != null) {
                expressionChecker.handleExpression(yieldStatement.Value, null, true);
                context.CodeValidationContext.YieldCount++;
            } else {
                yieldStatement.removeUserData(typeof(StatementNode));
            }
            return info;
        }
        
        private void visitStatements(List<StatementNode> statements, StatementScope scope) {
            if (!statements.isEmpty()) {
                linkStatements(statements, scope);
                visitStatement(statements[0]);
            }
        }
        
        private void linkStatements(Iterable<StatementNode> statements, StatementScope scope) {
            StatementNode previous = null;
            foreach (var s in statements) {
                if (s.StatementKind == StatementKind.Labeled) {
                    var ls = (LabeledStatementNode)s;
                    scope.declareLabel(context.getIdentifier(ls.NameOffset, ls.NameLength), ls);
                }
                s.addOrReplaceUserData(scope);
                if (previous != null) {
                    previous.addOrReplaceUserData(s);
                }
                previous = s;
            }
        }
        
        private void visitStatement(StatementNode statement) {
            while (statement != null && statement.getUserData(typeof(StatementInfo)) == null) {
                var info = handleStatement(statement, null);
                if (info.IsEndPointReachable) {
                    statement = statement.getUserData(typeof(StatementNode));
                }
            }
        }
        
        private class ExpressionChecker : ExpressionHandler<Void, Void> {
            private ReachabilityChecker statementChecker;
            private CompilerContext context;
            
            ExpressionChecker(ReachabilityChecker statementChecker, CompilerContext context)
                : super(true) {
                this.statementChecker = statementChecker;
                this.context = context;
            }
            
            protected override Void handleAnonymousObjectCreation(AnonymousObjectCreationExpressionNode anonymousObject,
                    Void source, bool nested) {
                foreach (var decl in anonymousObject.MemberDeclarators) {
                    handleExpression(decl.Value, null, true);
                }
                return null;
            }
            
            protected override Void handleArrayCreation(ArrayCreationExpressionNode arrayCreation, Void source, bool nested) {
                foreach (var e in arrayCreation.DimensionExpressions) {
                    handleExpression(e, null, true);
                }
                if (arrayCreation.Initializer != null) {
                    handleExpression(arrayCreation.Initializer, null, true);
                }
                return null;
            }
            
            protected override Void handleArrayInitializer(ArrayInitializerExpressionNode arrayInitializer, Void source, bool nested) {
                foreach (var e in arrayInitializer.Values) {
                    handleExpression(e, null, true);
                }
                return null;
            }
            
            protected override Void handleAssign(AssignExpressionNode assign, Void source, bool nested) {
                if (assign.Left.ExpressionKind == ExpressionKind.SimpleName) {
                    var linfo = assign.Left.getUserData(typeof(ExpressionInfo));
                    var member = linfo.Member;
                    if (member.MemberKind == MemberKind.Local) {
                        var minfo = statementChecker.uninitializedLocals[member];
                        if (minfo != null) {
                            minfo.assignmentCount++;
                            if (assign.Operator != AssignOperator.Assign) {
                                minfo.referenceCount++;
                            }
                        }
                        if (statementChecker.initializedLocals.containsKey(member)) {
                            statementChecker.initializedLocals.remove(member);
                        }
                    }
                } else {
                    handleExpression(assign.Left, null, true);
                }
                handleExpression(assign.Right, null, true);
                return null;
            }
        
            protected override Void handleBinary(BinaryExpressionNode binary, Void source, bool nested) {
                handleExpression(binary.LeftOperand, null, true);
                handleExpression(binary.RightOperand, null, true);
                return null;
            }
            
            protected override Void handleCast(CastExpressionNode cast, Void source, bool nested) {
                handleExpression(cast.Expression, null, true);
                return null;
            }
            
            protected override Void handleConditional(ConditionalExpressionNode conditional, Void source, bool nested) {
                handleExpression(conditional.Condition, null, true);
                handleExpression(conditional.IfTrue, null, true);
                handleExpression(conditional.IfFalse, null, true);
                return null;
            }
            
            protected override Void handleElementAccess(ElementAccessExpressionNode elementAccess, Void source, bool nested) {
                handleExpression(elementAccess.TargetObject, null, true);
                foreach (var arg in elementAccess.Indexes) {
                    handleExpression(arg, null, true);
                }
                return null;
            }
            
            protected override Void handleInvocation(InvocationExpressionNode invocation, Void source, bool nested) {
                handleExpression(invocation.TargetObject, null, true);
                foreach (var arg in invocation.Arguments) {
                    handleExpression(arg, null, true);
                }
                return null;
            }
            
            protected override Void handleLambda(LambdaExpressionNode lambda, Void source, bool nested) {
                var methodBuilder = lambda.getUserData(typeof(MethodBuilder));
                if (lambda.Body instanceof BlockStatementNode) {
                    var body = (BlockStatementNode)lambda.Body;
                    var info = statementChecker.handleStatement(body, null);
                    if (info.IsEndPointReachable) {
                        if (methodBuilder.ReturnType == context.TypeSystem.VoidType) {
                            var returnStatement = new ReturnStatementNode();
                            returnStatement.addOrReplaceUserData(new StatementInfo());
                            body.Statements.add(returnStatement);
                        } else if (context.Iterables[methodBuilder] != null) {
                            var yieldStatement = new YieldStatementNode();
                            yieldStatement.addOrReplaceUserData(new StatementInfo());
                            body.Statements.add(yieldStatement);
                        } else {
                            context.addError(CompileErrorId.MissingReturn, lambda);
                        }
                    }
                } else {
                    handleExpression(((ExpressionStatementNode)lambda.Body).Expression, null, true);
                    lambda.Body.addOrReplaceUserData(new StatementInfo());
                }
                return null;
            }
            
            protected override Void handleLiteral(LiteralExpressionNode literal, Void source, bool nested) {
                return null;
            }
            
            protected override Void handleMemberAccess(MemberAccessExpressionNode memberAccess, Void source, bool nested) {
                handleExpression(memberAccess.TargetObject, null, true);
                return null;
            }
            
            protected override Void handleObjectCreation(ObjectCreationExpressionNode objectCreation, Void source, bool nested) {
                foreach (var arg in objectCreation.Arguments) {
                    handleExpression(arg, null, true);
                }
                var init = objectCreation.Initializer;
                if (init != null) {
                    if (init.ExpressionKind == ExpressionKind.ObjectInitializer) {
                        var initializer = (ObjectInitializerExpressionNode)init;
                        foreach (var mi in initializer.MemberInitializers) {
                            handleExpression(mi.Value, null, true);
                        }
                    } else {
                        var initializer = (CollectionInitializerExpressionNode)init;
                        foreach (var args in initializer.Values) {
                            foreach (var e in args) {
                                handleExpression(e, null, true);
                            }
                        }
                    }
                }
                return null;
            }
            
            protected override Void handleSimpleName(SimpleNameExpressionNode simpleName, Void source, bool nested) {
                var info = simpleName.getUserData(typeof(ExpressionInfo));
                var minfo = statementChecker.uninitializedLocals[info.Member];
                if (minfo != null) {
                    minfo.referenceCount++;
                }
                if (statementChecker.initializedLocals.containsKey(info.Member)) {
                    statementChecker.initializedLocals.remove(info.Member);
                }
                return null;
            }

            protected override Void handleSizeof(SizeofExpressionNode sizeofExpression, Void source, bool nested) {
                handleExpression(sizeofExpression.Expression, null, true);
                return null;
            }

            protected override Void handleSuperAccess(SuperAccessExpressionNode superAccess, Void source, bool nested) {
                return null;
            }
            
            protected override Void handleThisAccess(ThisAccessExpressionNode thisAccess, Void source, bool nested) {
                return null;
            }
            
            protected override Void handleType(TypeExpressionNode type, Void source, bool nested) {
                return null;
            }
            
            protected override Void handleTypeof(TypeofExpressionNode typeofExpression, Void source, bool nested) {
                return null;
            }
            
            protected override Void handleUnary(UnaryExpressionNode unary, Void source, bool nested) {
                handleExpression(unary.Operand, null, true);
                return null;
            }
        }
        
        private class UnreachableStatementsHandler : StatementHandler<Void, Void> {
            private CompilerContext context;
            private ExpressionChecker expressionChecker;
        
            UnreachableStatementsHandler(CompilerContext context)
                : super(true) {
                this.context = context;
                this.expressionChecker = new ExpressionChecker(this, context);
            }
            
            protected override Void handleBlock(BlockStatementNode block, Void source) {
                var info = block.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, block);
                } else {
                    foreach (var s in block.Statements) {
                        handleStatement(s, null);
                    }
                }
                return null;
            }
            
            protected override Void handleBreak(BreakStatementNode breakStatement, Void source) {
                var info = breakStatement.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, breakStatement);
                }
                return null;
            }

            protected override Void handleContinue(ContinueStatementNode continueStatement, Void source) {
                var info = continueStatement.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, continueStatement);
                }
                return null;
            }
            
            protected override Void handleDo(DoStatementNode doStatement, Void source) {
                var info = doStatement.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, doStatement);
                } else {
                    handleStatement(doStatement.Statement, null);
                    expressionChecker.handleExpression(doStatement.Condition, null, true);
                }
                return null;
            }
            
            protected override Void handleEmpty(EmptyStatementNode empty, Void source) {
                var info = empty.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, empty);
                }
                return null;
            }
            
            protected override Void handleExpression(ExpressionStatementNode expression, Void source) {
                var info = expression.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, expression);
                } else {
                    expressionChecker.handleExpression(expression.Expression, null, true);
                }
                return null;
            }
            
            protected override Void handleFor(ForStatementNode forStatement, Void source) {
                var info = forStatement.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, forStatement);
                } else {
                    foreach (var s in forStatement.Initializer) {
                        handleStatement(s, null);
                    }
                    if (forStatement.Condition != null) {
                        expressionChecker.handleExpression(forStatement.Condition, null, true);
                    }
                    //foreach (var s in forStatement.Iterator) {
                    //	handleStatement(s, null);
                    //}
                    handleStatement(forStatement.Statement, null);
                }
                return null;
            }
            
            protected override Void handleForeach(ForeachStatementNode foreachStatement, Void source) {
                var info = foreachStatement.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, foreachStatement);
                } else {
                    expressionChecker.handleExpression(foreachStatement.Source, null, true);
                    handleStatement(foreachStatement.Statement, null);
                }
                return null;
            }
            
            protected override Void handleGoto(GotoStatementNode gotoStatement, Void source) {
                var info = gotoStatement.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, gotoStatement);
                }
                return null;
            }
            
            protected override Void handleGotoCase(GotoCaseStatementNode gotoCase, Void source) {
                var info = gotoCase.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, gotoCase);
                }
                return null;
            }
            
            protected override Void handleIf(IfStatementNode ifStatement, Void source) {
                var info = ifStatement.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, ifStatement);
                } else {
                    expressionChecker.handleExpression(ifStatement.Condition, null, true);
                    handleStatement(ifStatement.IfTrue, null);
                    if (ifStatement.IfFalse != null) {
                        handleStatement(ifStatement.IfFalse, null);
                    }
                }
                return null;
            }
            
            protected override Void handleLabeled(LabeledStatementNode labeled, Void source) {
                var info = labeled.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, labeled);
                } else {
                    handleStatement(labeled.Statement, source);
                }
                return null;
            }
            
            protected override Void handleLocalDeclaration(LocalDeclarationStatementNode localDeclaration, Void source) {
                var info = localDeclaration.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, localDeclaration);
                } else {
                    foreach (var decl in localDeclaration.Declarators) {
                        if (decl.Value != null) {
                            expressionChecker.handleExpression(decl.Value, null, true);
                        }
                    }
                }
                return null;
            }
            
            protected override Void handleReturn(ReturnStatementNode returnStatement, Void source) {
                var info = returnStatement.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, returnStatement);
                } else {
                    if (returnStatement.Value != null) {
                        expressionChecker.handleExpression(returnStatement.Value, null, true);
                    }
                }
                return null;
            }
            
            protected override Void handleSwitch(SwitchStatementNode switchStatement, Void source) {
                var info = switchStatement.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, switchStatement);
                } else {
                    expressionChecker.handleExpression(switchStatement.Selector, null, true);
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
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, synchronizedStatement);
                } else {
                    expressionChecker.handleExpression(synchronizedStatement.Lock, null, true);
                    handleStatement(synchronizedStatement.Statement, null);
                }
                return null;
            }
            
            protected override Void handleThrow(ThrowStatementNode throwStatement, Void source) {
                var info = throwStatement.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, throwStatement);
                } else {
                    if (throwStatement.Exception != null) {
                        expressionChecker.handleExpression(throwStatement.Exception, null, true);
                    }
                }
                return null;
            }
            
            protected override Void handleTry(TryStatementNode tryStatement, Void source) {
                var info = tryStatement.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, tryStatement);
                } else {
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

            protected override Void handleUsing(UsingStatementNode usingStatement, Void source) {
                var info = usingStatement.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, usingStatement);
                } else {
                    handleStatement(usingStatement.ResourceAcquisition, null);
                    handleStatement(usingStatement.Statement, null);
                }
                return null;
            }
            
            protected override Void handleWhile(WhileStatementNode whileStatement, Void source) {
                var info = whileStatement.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, whileStatement);
                } else {
                    expressionChecker.handleExpression(whileStatement.Condition, null, true);
                    handleStatement(whileStatement.Statement, null);
                }
                return null;
            }
            
            protected override Void handleYield(YieldStatementNode yield, Void source) {
                var info = yield.getUserData(typeof(StatementInfo));
                if (info == null) {
                    context.addWarning(CompileErrorId.UnreachableStatement, yield);
                } else {
                    if (yield.Value != null) {
                        expressionChecker.handleExpression(yield.Value, null, true);
                    }
                }
                return null;
            }
            
            private class ExpressionChecker : ExpressionHandler<Void, Void> {
                private UnreachableStatementsHandler statementHandler;
                private CompilerContext context;
                
                ExpressionChecker(UnreachableStatementsHandler statementHandler, CompilerContext context)
                    : super(true) {
                    this.statementHandler = statementHandler;
                    this.context = context;
                }
                
                protected override Void handleAnonymousObjectCreation(AnonymousObjectCreationExpressionNode anonymousObject,
                        Void source, bool nested) {
                    foreach (var decl in anonymousObject.MemberDeclarators) {
                        handleExpression(decl.Value, null, true);
                    }
                    return null;
                }	

                protected override Void handleArrayCreation(ArrayCreationExpressionNode arrayCreation, Void source, bool nested) {
                    foreach (var e in arrayCreation.DimensionExpressions) {
                        handleExpression(e, null, true);
                    }
                    if (arrayCreation.Initializer != null) {
                        handleExpression(arrayCreation.Initializer, null, true);
                    }
                    return null;
                }
                
                protected override Void handleArrayInitializer(ArrayInitializerExpressionNode arrayInitializer, Void source, bool nested) {
                    foreach (var e in arrayInitializer.Values) {
                        handleExpression(e, null, true);
                    }
                    return null;
                }
                
                protected override Void handleAssign(AssignExpressionNode assign, Void source, bool nested) {
                    handleExpression(assign.Left, null, true);
                    handleExpression(assign.Right, null, true);
                    return null;
                }
            
                protected override Void handleBinary(BinaryExpressionNode binary, Void source, bool nested) {
                    handleExpression(binary.LeftOperand, null, true);
                    handleExpression(binary.RightOperand, null, true);
                    return null;
                }
                
                protected override Void handleCast(CastExpressionNode cast, Void source, bool nested) {
                    handleExpression(cast.Expression, null, true);
                    return null;
                }
                
                protected override Void handleConditional(ConditionalExpressionNode conditional, Void source, bool nested) {
                    handleExpression(conditional.Condition, null, true);
                    handleExpression(conditional.IfTrue, null, true);
                    handleExpression(conditional.IfFalse, null, true);
                    return null;
                }
                
                protected override Void handleElementAccess(ElementAccessExpressionNode elementAccess, Void source, bool nested) {
                    handleExpression(elementAccess.TargetObject, null, true);
                    foreach (var arg in elementAccess.Indexes) {
                        handleExpression(arg, null, true);
                    }
                    return null;
                }
                
                protected override Void handleInvocation(InvocationExpressionNode invocation, Void source, bool nested) {
                    handleExpression(invocation.TargetObject, null, true);
                    foreach (var arg in invocation.Arguments) {
                        handleExpression(arg, null, true);
                    }
                    return null;
                }
                
                protected override Void handleLambda(LambdaExpressionNode lambda, Void source, bool nested) {
                    statementHandler.handleStatement(lambda.Body, null);
                    return null;
                }
                
                protected override Void handleLiteral(LiteralExpressionNode literal, Void source, bool nested) {
                    return null;
                }
                
                protected override Void handleMemberAccess(MemberAccessExpressionNode memberAccess, Void source, bool nested) {
                    handleExpression(memberAccess.TargetObject, null, true);
                    return null;
                }
                
                protected override Void handleObjectCreation(ObjectCreationExpressionNode objectCreation, Void source, bool nested) {
                    foreach (var arg in objectCreation.Arguments) {
                        handleExpression(arg, null, true);
                    }
                    var init = objectCreation.Initializer;
                    if (init != null) {
                        if (init.ExpressionKind == ExpressionKind.ObjectInitializer) {
                            var initializer = (ObjectInitializerExpressionNode)init;
                            foreach (var mi in initializer.MemberInitializers) {
                                handleExpression(mi.Value, null, true);
                            }
                        } else {
                            var initializer = (CollectionInitializerExpressionNode)init;
                            foreach (var args in initializer.Values) {
                                foreach (var e in args) {
                                    handleExpression(e, null, true);
                                }
                            }
                        }
                    }
                    return null;
                }
                
                protected override Void handleSimpleName(SimpleNameExpressionNode simpleName, Void source, bool nested) {
                    return null;
                }

                protected override Void handleSizeof(SizeofExpressionNode sizeofExpression, Void source, bool nested) {
                    handleExpression(sizeofExpression.Expression, null, true);
                    return null;
                }

                protected override Void handleSuperAccess(SuperAccessExpressionNode superAccess, Void source, bool nested) {
                    return null;
                }
                
                protected override Void handleThisAccess(ThisAccessExpressionNode thisAccess, Void source, bool nested) {
                    return null;
                }
                
                protected override Void handleType(TypeExpressionNode type, Void source, bool nested) {
                    return null;
                }
                
                protected override Void handleTypeof(TypeofExpressionNode typeofExpression, Void source, bool nested) {
                    return null;
                }
                
                protected override Void handleUnary(UnaryExpressionNode unary, Void source, bool nested) {
                    handleExpression(unary.Operand, null, true);
                    return null;
                }
            }
        }
    }
}
