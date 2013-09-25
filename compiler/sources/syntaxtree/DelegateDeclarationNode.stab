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

	public class DelegateDeclarationNode : TypeMemberNode, IPackageMember {
		public DelegateDeclarationNode()
			: super(TypeMemberKind.Delegate) {
			this.TypeParameters = new ArrayList<SimpleNameTypeReferenceNode>();
			this.Parameters = new ArrayList<ParameterNode>();
			this.ConstraintsClauses = new ArrayList<TypeParameterConstraintsClauseNode>();
		}
		
		public PackageMemberKind PackageMemberKind {
			get {
				return PackageMemberKind.Delegate;
			}
		}
		
		public TypeReferenceNode ReturnType;
		
		public int NameOffset;
		
		public int NameLength;
		
		public List<SimpleNameTypeReferenceNode> TypeParameters^;
		
		public List<ParameterNode> Parameters^;
		
		public List<TypeParameterConstraintsClauseNode> ConstraintsClauses^;
	}
	
}
