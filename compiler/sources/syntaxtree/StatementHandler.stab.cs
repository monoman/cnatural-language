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

namespace cnatural.syntaxtree {

    public abstract class StatementHandler<TSource, TResult> {
        private bool failWhenUnhandled;

        protected StatementHandler(bool failWhenUnhandled) {
            this.failWhenUnhandled = failWhenUnhandled;
        }

        public virtual TResult handleStatement(StatementNode statement, TSource source) {
            switch (statement.StatementKind) {
            case Block:
                return handleBlock((BlockStatementNode)statement, source);

            case Break:
                return handleBreak((BreakStatementNode)statement, source);
                
            case Continue:
                return handleContinue((ContinueStatementNode)statement, source);
                
            case Do:
                return handleDo((DoStatementNode)statement, source);
                
            case Empty:
                return handleEmpty((EmptyStatementNode)statement, source);
                
            case Expression:
                return handleExpression((ExpressionStatementNode)statement, source);

            case For:
                return handleFor((ForStatementNode)statement, source);
                
            case Foreach:
                return handleForeach((ForeachStatementNode)statement, source);
            
            case Goto:
                return handleGoto((GotoStatementNode)statement, source);
                
            case GotoCase:
                return handleGotoCase((GotoCaseStatementNode)statement, source);
                
            case If:
                return handleIf((IfStatementNode)statement, source);

            case Labeled:
                return handleLabeled((LabeledStatementNode)statement, source);
                
            case LocalDeclaration:
                return handleLocalDeclaration((LocalDeclarationStatementNode)statement, source);
                
            case Return:
                return handleReturn((ReturnStatementNode)statement, source);
                
            case Switch:
                return handleSwitch((SwitchStatementNode)statement, source);
                
            case Synchronized:
                return handleSynchronized((SynchronizedStatementNode)statement, source);
                
            case Throw:
                return handleThrow((ThrowStatementNode)statement, source);
                
            case Try:
                return handleTry((TryStatementNode)statement, source);
                
            case Using:
                return handleUsing((UsingStatementNode)statement, source);
            
            case While:
                return handleWhile((WhileStatementNode)statement, source);
                
            case Yield:
                return handleYield((YieldStatementNode)statement, source);
                
            default:
                throw new IllegalStateException("Unhandled statement kind: " + statement.StatementKind);
            }
        }
        
        protected virtual TResult handleBlock(BlockStatementNode block, TSource source) {
            return defaultHandler();
        }

        protected virtual TResult handleBreak(BreakStatementNode breakStatement, TSource source) {
            return defaultHandler();
        }

        protected virtual TResult handleContinue(ContinueStatementNode continueStatement, TSource source) {
            return defaultHandler();
        }

        protected virtual TResult handleDo(DoStatementNode doStatement, TSource source) {
            return defaultHandler();
        }
        
        protected virtual TResult handleEmpty(EmptyStatementNode empty, TSource source) {
            return defaultHandler();
        }
        
        protected virtual TResult handleExpression(ExpressionStatementNode expression, TSource source) {
            return defaultHandler();
        }

        protected virtual TResult handleFor(ForStatementNode forStatement, TSource source) {
            return defaultHandler();
        }

        protected virtual TResult handleForeach(ForeachStatementNode foreachStatement, TSource source) {
            return defaultHandler();
        }

        protected virtual TResult handleGoto(GotoStatementNode gotoStatement, TSource source) {
            return defaultHandler();
        }

        protected virtual TResult handleGotoCase(GotoCaseStatementNode gotoCase, TSource source) {
            return defaultHandler();
        }

        protected virtual TResult handleIf(IfStatementNode ifStatement, TSource source) {
            return defaultHandler();
        }
        
        protected virtual TResult handleLabeled(LabeledStatementNode labeled, TSource source) {
            return defaultHandler();
        }
        
        protected virtual TResult handleLocalDeclaration(LocalDeclarationStatementNode localDeclaration, TSource source) {
            return defaultHandler();
        }
        
        protected virtual TResult handleReturn(ReturnStatementNode returnStatement, TSource source) {
            return defaultHandler();
        }
        
        protected virtual TResult handleSwitch(SwitchStatementNode switchStatement, TSource source) {
            return defaultHandler();
        }
        
        protected virtual TResult handleSynchronized(SynchronizedStatementNode synchronizedStatement, TSource source) {
            return defaultHandler();
        }
        
        protected virtual TResult handleThrow(ThrowStatementNode throwStatement, TSource source) {
            return defaultHandler();
        }
        
        protected virtual TResult handleTry(TryStatementNode tryStatement, TSource source) {
            return defaultHandler();
        }
        
        protected virtual TResult handleUsing(UsingStatementNode usingStatement, TSource source) {
            return defaultHandler();
        }
        
        protected virtual TResult handleWhile(WhileStatementNode whileStatement, TSource source) {
            return defaultHandler();
        }
        
        protected virtual TResult handleYield(YieldStatementNode yield, TSource source) {
            return defaultHandler();
        }
        
        private TResult defaultHandler() {
            if (failWhenUnhandled) {
                throw new IllegalStateException();
            } else {
                return null;
            }
        }
    }
}
