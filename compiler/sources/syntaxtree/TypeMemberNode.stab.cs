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

namespace cnatural.syntaxtree {

	public enum TypeMemberKind {
		Class,
		Constructor,
		Delegate,
		Destructor,
		EnumConstant,
		Field,
		Indexer,
		Interface,
		Method,
		Property,
	}

	public enum Modifier {
		Public,
		Protected,
		Private,
		Abstract,
		Final,
		Static,
		Override,
		Virtual,
		Native,
		Strictfp,
		Synchronized,
		Transient,
		Volatile,
		Property
	}
	
	public interface ITypeMember {
		TypeMemberKind TypeMemberKind^;
        int DocumentationOffset;
        int DocumentationLength;
	}
	
	public interface IInterfaceMember : ITypeMember {
	}
	
	public interface IAccessorsOwner {
		EnumSet<Modifier> Modifiers^;
		AccessorDeclarationNode GetAccessor;
		AccessorDeclarationNode SetAccessor;
	}
	
	public class AccessorDeclarationNode : SyntaxNode {
		public AccessorDeclarationNode() {
			this.Annotations = new ArrayList<AnnotationSectionNode>();
			this.Modifiers = EnumSet.noneOf(typeof(Modifier));
		}
		
		public List<AnnotationSectionNode> Annotations^;
		
		public EnumSet<Modifier> Modifiers^;
		
		public BlockStatementNode Body;
		
	}
	
	public class TypeParameterConstraintsClauseNode : SyntaxNode {
		public TypeParameterConstraintsClauseNode() {
			this.Constraints = new ArrayList<TypeReferenceNode>();
		}
		
		public int NameOffset;
		
		public int NameLength;
		
		public List<TypeReferenceNode> Constraints^;
	}
	
	public abstract class TypeMemberNode : SyntaxNode, ITypeMember {
		protected TypeMemberNode(TypeMemberKind typeMemberKind) {
			this.TypeMemberKind = typeMemberKind;
			this.Annotations = new ArrayList<AnnotationSectionNode>();
			this.Modifiers = EnumSet.noneOf(typeof(Modifier));
		}
		
		public TypeMemberKind TypeMemberKind^;
		
        public int DocumentationOffset;
		
        public int DocumentationLength;
        
		public List<AnnotationSectionNode> Annotations^;	
		
		public EnumSet<Modifier> Modifiers^;
	}
	
}
