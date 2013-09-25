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
using java.util;

namespace stab.tools.syntaxtree {

	public class QueryExpressionNode : ExpressionNode {
		public QueryExpressionNode()
			: super(ExpressionKind.Query) {
			this.From = new FromQueryBodyClauseNode();
			this.Body = new QueryBodyNode();
		}
		
		public FromQueryBodyClauseNode From^;
		
		public QueryBodyNode Body^;
	}
	
	public enum QueryBodyClauseKind {
		From,
		Join,
		Let,
		Orderby,
		Where
	}
	
	public abstract class QueryBodyClauseNode : SyntaxNode {
		protected QueryBodyClauseNode(QueryBodyClauseKind queryBodyClauseKind) {
			this.QueryBodyClauseKind = queryBodyClauseKind;
		}
	
		public QueryBodyClauseKind QueryBodyClauseKind^;
	}
	
	public class FromQueryBodyClauseNode : QueryBodyClauseNode {
		public FromQueryBodyClauseNode()
			: super(QueryBodyClauseKind.From) {
		}
		
		public TypeReferenceNode Type;
		
		public ExpressionNode Origin;
		
		public int NameOffset;
		
		public int NameLength;
	}

	public class JoinQueryBodyClauseNode : QueryBodyClauseNode {
		public JoinQueryBodyClauseNode()
			: super(QueryBodyClauseKind.Join) {
		}
		
		public TypeReferenceNode Type;
		
		public int NameOffset;
		
		public int NameLength;
		
		public ExpressionNode Origin;
		
		public ExpressionNode EqualsLeftOperand;
		
		public ExpressionNode EqualsRightOperand;
		
		public int ResultOffset;
		
		public int ResultLength;
	}
	
	public class LetQueryBodyClauseNode : QueryBodyClauseNode {
		public LetQueryBodyClauseNode()
			: super(QueryBodyClauseKind.Let) {
		}
		
		public int NameOffset;
		
		public int NameLength;
		
		public ExpressionNode Value;
	}
	
	public class OrderbyQueryBodyClauseNode : QueryBodyClauseNode {
		public OrderbyQueryBodyClauseNode()
			: super(QueryBodyClauseKind.Orderby) {
			this.Orderings = new ArrayList<OrderingNode>();
		}
		
		public List<OrderingNode> Orderings^;
	}

	public class OrderingNode : SyntaxNode {
		public OrderingNode() {
		}
		
		public bool Descending;
		
		public ExpressionNode KeySelector;
	}
	
	public class WhereQueryBodyClauseNode : QueryBodyClauseNode {
		public WhereQueryBodyClauseNode()
			: super(QueryBodyClauseKind.Where) {
		}
		
		public ExpressionNode Predicat;
	}
	
	public class QueryBodyNode : SyntaxNode {
		public QueryBodyNode() {
			this.Clauses = new ArrayList<QueryBodyClauseNode>();
		}
		
		public ExpressionNode SelectOrGroup;
		
		public ExpressionNode By;
		
		public QueryContinuationNode Continuation;
		
		public List<QueryBodyClauseNode> Clauses;
	}
	
	public class QueryContinuationNode : SyntaxNode {
		public QueryContinuationNode() {
			this.Body = new QueryBodyNode();
		}
		
		public int NameOffset;
		
		public int NameLength;
		
		public QueryBodyNode Body^;
	}
}
