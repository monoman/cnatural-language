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
namespace cnatural.syntaxtree {

	public enum ExpressionKind {
		Annotation,
		AnonymousObjectCreation,
		ArrayCreation,
		ArrayInitializer,
		Assign,
		Binary,
		Cast,
		CollectionInitializer,
		Conditional,
		ElementAccess,
		Invocation,
		Lambda,
		Literal,
		MemberAccess,
		NullSafeMemberAccess,
		ObjectCreation,
		ObjectInitializer,
		Query,
		SimpleName,
		Sizeof,
		SuperAccess,
		ThisAccess,
		Type,
		Typeof,
		Unary
	}

	public abstract class ExpressionNode : SyntaxNode {
		protected ExpressionNode(ExpressionKind expressionKind) {
			this.ExpressionKind = expressionKind;
		}
		
		public ExpressionKind ExpressionKind^;
		
		public bool Parenthesized;
	}

	public class ThisAccessExpressionNode : ExpressionNode {
		public ThisAccessExpressionNode()
			: super(ExpressionKind.ThisAccess) {
		}
	}

	public class SuperAccessExpressionNode : ExpressionNode {
		public SuperAccessExpressionNode()
			: super(ExpressionKind.SuperAccess) {
		}
	}
}
