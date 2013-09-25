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

namespace stab.tools.syntaxtree {

	public abstract class ExpressionHandler<TSource, TResult> {
		private bool failWhenUnhandled;
		
		protected ExpressionHandler(bool failWhenUnhandled) {
			this.failWhenUnhandled = failWhenUnhandled;
		}
		
		public virtual TResult handleExpression(ExpressionNode expression, TSource source, bool nested) {
			switch (expression.ExpressionKind) {
			case AnonymousObjectCreation:
				return handleAnonymousObjectCreation((AnonymousObjectCreationExpressionNode)expression, source, nested);
			
			case ArrayCreation:
				return handleArrayCreation((ArrayCreationExpressionNode)expression, source, nested);
			
			case ArrayInitializer:
				return handleArrayInitializer((ArrayInitializerExpressionNode)expression, source, nested);
				
			case Assign:
				return handleAssign((AssignExpressionNode)expression, source, nested);

			case Binary:
				return handleBinary((BinaryExpressionNode)expression, source, nested);

			case Cast:
				return handleCast((CastExpressionNode)expression, source, nested);
				
			case CollectionInitializer:
				return handleCollectionInitializer((CollectionInitializerExpressionNode)expression, source, nested);

			case Conditional:
				return handleConditional((ConditionalExpressionNode)expression, source, nested);
				
			case ElementAccess:
				return handleElementAccess((ElementAccessExpressionNode)expression, source, nested);
				
			case Invocation:
				return handleInvocation((InvocationExpressionNode)expression, source, nested);

			case Lambda:
				return handleLambda((LambdaExpressionNode)expression, source, nested);
				
			case Literal:
				return handleLiteral((LiteralExpressionNode)expression, source, nested);
			
			case MemberAccess:
				return handleMemberAccess((MemberAccessExpressionNode)expression, source, nested);

			case NullSafeMemberAccess:
				return handleMemberAccess((NullSafeMemberAccessExpressionNode)expression, source, nested);

			case ObjectCreation:
				return handleObjectCreation((ObjectCreationExpressionNode)expression, source, nested);
				
			case ObjectInitializer:
				return handleObjectInitializer((ObjectInitializerExpressionNode)expression, source, nested);

			case Query:
				return handleQuery((QueryExpressionNode)expression, source, nested);
				
			case SimpleName:
				return handleSimpleName((SimpleNameExpressionNode)expression, source, nested);
				
			case Sizeof:
				return handleSizeof((SizeofExpressionNode)expression, source, nested);
				
			case SuperAccess:
				return handleSuperAccess((SuperAccessExpressionNode)expression, source, nested);
			
			case ThisAccess:
				return handleThisAccess((ThisAccessExpressionNode)expression, source, nested);
			
			case Type:
				return handleType((TypeExpressionNode)expression, source, nested);
				
			case Typeof:
				return handleTypeof((TypeofExpressionNode)expression, source, nested);
				
			case Unary:
				return handleUnary((UnaryExpressionNode)expression, source, nested);
			
			case Annotation:
			default:
				throw new IllegalStateException("Unhandled expression kind: " + expression.ExpressionKind);
			}
		}
		
		protected virtual TResult handleAnonymousObjectCreation(AnonymousObjectCreationExpressionNode anonymousObject, TSource source,
				bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleArrayCreation(ArrayCreationExpressionNode arrayCreation, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleArrayInitializer(ArrayInitializerExpressionNode arrayInitializer, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleAssign(AssignExpressionNode assign, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleBinary(BinaryExpressionNode binary, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleCast(CastExpressionNode cast, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleCollectionInitializer(CollectionInitializerExpressionNode initializer, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleConditional(ConditionalExpressionNode conditional, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleElementAccess(ElementAccessExpressionNode elementAccess, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleInvocation(InvocationExpressionNode invocation, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleLambda(LambdaExpressionNode lambda, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleLiteral(LiteralExpressionNode literal, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleMemberAccess(MemberAccessExpressionNode memberAccess, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleObjectCreation(ObjectCreationExpressionNode objectCreation, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleObjectInitializer(ObjectInitializerExpressionNode initializer, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleQuery(QueryExpressionNode query, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleSimpleName(SimpleNameExpressionNode simpleName, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleSizeof(SizeofExpressionNode sizeofExpression, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleSuperAccess(SuperAccessExpressionNode superAccess, TSource source, bool nested) {
			return defaultHandler();
		}
		
		protected virtual TResult handleThisAccess(ThisAccessExpressionNode thisAccess, TSource source, bool nested) {
			return defaultHandler();
		}

		protected virtual TResult handleType(TypeExpressionNode type, TSource source, bool nested) {
			return defaultHandler();
		}

		protected virtual TResult handleTypeof(TypeofExpressionNode typeofExpression, TSource source, bool nested) {
			return defaultHandler();
		}

		protected virtual TResult handleUnary(UnaryExpressionNode unary, TSource source, bool nested) {
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
