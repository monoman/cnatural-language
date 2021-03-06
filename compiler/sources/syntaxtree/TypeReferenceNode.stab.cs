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

	public enum TypeReferenceKind {
		Boolean,
		Byte,
		Char,
		Double,
		Float,
		Int,
		Long,
		Short,
		Void,
		String,
		SimpleName,
		Qualified,
		Array,
		Wildcard,
		UpperBoundedWildcard,
		LowerBoundedWildcard
	}

	public abstract class TypeReferenceNode : SyntaxNode {
		public TypeReferenceNode(TypeReferenceKind typeReferenceKind) {
			this.TypeReferenceKind = typeReferenceKind;
		}
		
		public TypeReferenceKind TypeReferenceKind^;
	}
	
	public class PrimitiveTypeReferenceNode : TypeReferenceNode {
		public PrimitiveTypeReferenceNode(TypeReferenceKind typeReferenceKind)
			: super(typeReferenceKind) {
		}
	}
	
	public class ArrayTypeReferenceNode : TypeReferenceNode {
		public ArrayTypeReferenceNode()
			: super(TypeReferenceKind.Array) {
		}
		
		public TypeReferenceNode ElementType;
	}
	
	public class WildcardTypeReferenceNode : TypeReferenceNode {
		public WildcardTypeReferenceNode(TypeReferenceKind typeReferenceKind)
			: super(typeReferenceKind) {
		}
		
		public TypeReferenceNode Bound;
	}
	
	public class SimpleNameTypeReferenceNode : TypeReferenceNode {
		public SimpleNameTypeReferenceNode()
			: super(TypeReferenceKind.SimpleName) {
			this.TypeArguments = new ArrayList<TypeReferenceNode>();
		}
		
		public int NameOffset ;
		public int NameLength;
		public List<TypeReferenceNode> TypeArguments^;
	}
	
	public class QualifiedTypeReferenceNode : TypeReferenceNode {
		public QualifiedTypeReferenceNode()
			: super(TypeReferenceKind.Qualified) {
		}
		
		public TypeReferenceNode EnclosingType;
		
		public SimpleNameTypeReferenceNode SimpleName;
	}
	
}
