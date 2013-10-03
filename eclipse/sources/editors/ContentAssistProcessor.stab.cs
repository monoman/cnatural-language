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
using org.eclipse.jface.text;
using org.eclipse.jface.text.contentassist;
using org.eclipse.jface.text.source;
using stab.query;
using stab.reflection;
using cnatural.compiler;
using cnatural.helpers;
using cnatural.parser;
using cnatural.syntaxtree;

namespace cnatural.eclipse.editors {

	public class ContentAssistProcessor : IContentAssistProcessor {
		private static final ICompletionProposal[] EMPTY_PROPOSALS = CompletionProposalHelper.createEmpty();
		private static final Iterable<String> STATEMENT_TEXTS = new ArrayList<String> {
			"bool", "boolean", "byte", "char", "do", "double", "float", "for", "foreach", "if", "int",
			"long", "new", "return", "short", "string", "super", "synchronized", "this", "using", "while" };
		private static final Iterable<String> EXPRESSION_TEXTS = new ArrayList<String> {
			"false", "instanceof", "new", "null", "sizeof", "super", "this", "true", "typeof" };
		private static final Iterable<String> TYPE_TEXTS = new ArrayList<String> {
			"bool", "boolean", "byte", "char", "delegate", "double", "float", "int", "long", "short", "string" };
	
		private SourceEditor editor;
		private ISourceViewer sourceViewer;
		
		private bool inEditedText;
		private int actualOffset;
		private int actualTokenIndex;
		private Token actualToken;
		private int offset;
		private int tokenIndex;
		private Token token;
		private char[] text;
		private List<Token> tokens;
		private ContentAssistContext context;
		private StatementProcessor statementProcessor;
		private ExpressionProcessor expressionProcessor;

		public ContentAssistProcessor(SourceEditor editor, ISourceViewer sourceViewer) {
			this.editor = editor;
			this.sourceViewer = sourceViewer;
			this.statementProcessor = new StatementProcessor(this);
			this.expressionProcessor = new ExpressionProcessor(this);
		}
		
		public IContextInformation[] computeContextInformation(ITextViewer viewer, int offset) {
			return null;
		}
		
		public char[] getContextInformationAutoActivationCharacters() {
			return null;
		}
	
		public String getErrorMessage() {
			return null;
		}
		
		public IContextInformationValidator getContextInformationValidator() {
			return null;
		}
		
		public char[] getCompletionProposalAutoActivationCharacters() {
			return null;
		}
		
		public ICompletionProposal[] computeCompletionProposals(ITextViewer viewer, int offset) {
			Environment.trace(this, "compute completion");
			var t0 = System.nanoTime();

			var file = editor.getFile();
			if (file == null) {
				Environment.trace(this, "no file");
				return EMPTY_PROPOSALS;
			}
			
			var projectManager = Environment.getProjectManager(file);
			if (projectManager == null) {
				Environment.trace(this, "not a project with Stab nature");
				return EMPTY_PROPOSALS;
			}
			
			var highlighter = editor.Highlighter;
			if (highlighter == null) {
				Environment.trace(this, "no highlighter");
				return EMPTY_PROPOSALS;
			}
			
			var tokens = highlighter.Tokens;
			if (tokens == null || tokens.size() == 0) {
				Environment.trace(this, "no tokens");
				return EMPTY_PROPOSALS;
			}
			
			var tokenIndex = Highlighter.getPositionIndex(tokens, offset);
			if (tokenIndex == -1) {
				Environment.trace(this, "no token at " + offset);
				return EMPTY_PROPOSALS;
			}

			var token = tokens[tokenIndex];
			var beforeToken = token.offset == offset;

			Environment.trace(this, "token " + token.LexicalUnit + " " + token.IsUpdated);
			
			if (token.SemanticStyle != null && !beforeToken) {
				switch (token.SemanticStyle) {
				case Preprocessor:
				case SkippedSource:
					Environment.trace(this, "in preprocessor part");
					return EMPTY_PROPOSALS;
				case DelimitedComment:
				case SingleLineComment:
					Environment.trace(this, "in comment");
					return EMPTY_PROPOSALS;
				}
			}
			if (token.LexicalUnit == LexicalUnit.NewLine) {
				if (tokenIndex > 0 && tokens[tokenIndex - 1].LexicalUnit == LexicalUnit.SingleLineComment) {
					Environment.trace(this, "in comment");
					return EMPTY_PROPOSALS;
				}
			}

			var compilationUnit = highlighter.CompilationUnit;
			if (compilationUnit == null) {
				Environment.trace(this, "no AST");
				return EMPTY_PROPOSALS;
			}

			this.inEditedText = token.IsUpdated || (beforeToken && tokenIndex > 0 && tokens[tokenIndex - 1].IsUpdated);
			this.actualOffset = offset;
			this.actualTokenIndex = tokenIndex;
			this.actualToken = token;
			if (this.inEditedText) {
				var tk = tokens[--tokenIndex];
				while (tokenIndex > 0) {
					if (tokens[tokenIndex - 1].IsUpdated) {
						tk = tokens[--tokenIndex];
					} else {
						break;
					}
				}
				this.offset = tk.offset;
				this.token = tk;
			} else {
				this.offset = offset;
				this.token = token;
			}
			this.tokenIndex = tokenIndex;
			this.tokens = tokens;
			this.text = highlighter.Text;
			this.context = new ContentAssistContext(highlighter.TypeSystem, highlighter.AnnotatedTypeSystem);
			
			var result = computeCompletionProposalsInPackage(Query.empty<String>(), compilationUnit.Body);
			Environment.trace(this, "compute completion done in " + ((System.nanoTime() - t0) / 1e6) + "ms");
			return result;
		}
		
		private ICompletionProposal[] computeCompletionProposalsInPackage(Iterable<String> names, PackageBodyNode body) {
			var beforeMembers = true;
			if (body != null) {
				foreach (var u in body.UsingDirectives) {
					var bounds = u.getUserData(typeof(SyntaxNodeBounds));
					if (bounds == null || bounds.isAfterOffset(offset)) {
						break;
					}
					if (bounds.isBeforeOffset(offset)) {
						continue;
					}
		
					// TODO
					Environment.trace(this, "using");
					return EMPTY_PROPOSALS;
				}

				context.enterPackage(names, body);
				
				foreach (var m in body.Members) {
					var bounds = ((SyntaxNode)m).getUserData(typeof(SyntaxNodeBounds));
					if (bounds == null || bounds.isAfterOffset(offset)) {
						break;
					}
					beforeMembers = false;
					if (bounds.isBeforeOffset(offset)) {
						continue;
					}
					switch (m.PackageMemberKind) {
					case Package:
						var pkg = (PackageDeclarationNode)m;
						return computeCompletionProposalsInPackage(pkg.Identifiers.select(p => p.getUserData(typeof(String))), pkg.Body);
					case Class:
						return computeCompletionProposalsInClass((ClassDeclarationNode)m);
					case Interface:
						return computeCompletionProposalsInInterface((InterfaceDeclarationNode)m);
					case Delegate:
						return computeCompletionProposalsInDelegate((DelegateDeclarationNode)m);
					}
				}
				if (beforeMembers) {
				}
			}
			
			// TODO
			Environment.trace(this, "package " + beforeMembers);
			return EMPTY_PROPOSALS;
		}
		
		private ICompletionProposal[] computeCompletionProposalsInClass(ClassDeclarationNode declaration) {
			var beforeMembers = true;
			foreach (var m in declaration.Members) {
				var bounds = ((SyntaxNode)m).getUserData(typeof(SyntaxNodeBounds));
				if (bounds == null || bounds.isAfterOffset(offset)) {
					break;
				}
				beforeMembers = false;
				if (bounds.isBeforeOffset(offset)) {
					continue;
				}
				context.enterClass(declaration);
				switch (m.TypeMemberKind) {
				case Class:
					return computeCompletionProposalsInClass((ClassDeclarationNode)m);
				case Interface:
					return computeCompletionProposalsInInterface((InterfaceDeclarationNode)m);
				case Delegate:
					return computeCompletionProposalsInDelegate((DelegateDeclarationNode)m);
				case Method:
					return computeCompletionProposalsInMethod((MethodDeclarationNode)m);
				case Property:
					return computeCompletionProposalsInProperty((PropertyDeclarationNode)m);
				case Indexer:
					return computeCompletionProposalsInIndexer((IndexerDeclarationNode)m);
				case Constructor:
					return computeCompletionProposalsInConstructor((ConstructorDeclarationNode)m);
				case Destructor:
					return computeCompletionProposalsInDestructor((DestructorDeclarationNode)m);
				case Field:
					return computeCompletionProposalsInField((FieldDeclarationNode)m);
				case EnumConstant:
					return computeCompletionProposalsInEnumConstant((EnumConstantDeclarationNode)m);
				}
			}
			
			// TODO
			Environment.trace(this, "class " + beforeMembers);
			return computeCompletionProposalsOutsideExpression(declaration);
		}
		
		private ICompletionProposal[] computeCompletionProposalsInInterface(InterfaceDeclarationNode declaration) {
			var beforeMembers = true;
			foreach (var m in declaration.Members) {
				var bounds = ((SyntaxNode)m).getUserData(typeof(SyntaxNodeBounds));
				if (bounds == null || bounds.isAfterOffset(offset)) {
					break;
				}
				beforeMembers = false;
				if (bounds.isBeforeOffset(offset)) {
					continue;
				}
				context.enterInterface(declaration);
				switch (m.TypeMemberKind) {
				case Method:
					return computeCompletionProposalsInMethod((MethodDeclarationNode)m);
				case Property:
					return computeCompletionProposalsInProperty((PropertyDeclarationNode)m);
				case Indexer:
					return computeCompletionProposalsInIndexer((IndexerDeclarationNode)m);
				}
			}
				
			// TODO
			Environment.trace(this, "interface " + beforeMembers);
			return computeCompletionProposalsOutsideExpression(declaration);
		}
		
		private ICompletionProposal[] computeCompletionProposalsInDelegate(DelegateDeclarationNode declaration) {
		
			// TODO
			Environment.trace(this, "delegate");
			return computeCompletionProposalsOutsideExpression(declaration);
		}
		
		private ICompletionProposal[] computeCompletionProposalsInMethod(MethodDeclarationNode declaration) {
			var m = declaration.getUserData(typeof(MethodInfo));
			if (containsOffset(declaration.ReturnType)) {
				return computeCompletionProposalsInType(declaration.ReturnType, m.ReturnType);
			}
			var pit = m.Parameters.iterator();
			foreach (var p in declaration.Parameters) {
				var pt = pit.next().Type;
			
				var bounds = p.getUserData(typeof(SyntaxNodeBounds));
				if (bounds == null || bounds.isAfterOffset(offset)) {
					break;
				}
				if (bounds.isBeforeOffset(offset)) {
					continue;
				}
			
				if (containsOffset(p.Type)) {
					return computeCompletionProposalsInType(p.Type, pt);
				}
	
				Environment.trace(this, "method parameter");
				return computeCompletionProposalsOutsideExpression(declaration);
			}
			var body = declaration.Body;
			if (body != null && containsOffset(body)) {
				context.enterMethod(declaration);
				return statementProcessor.handleStatement(body, null);
			}
		
			// TODO
			Environment.trace(this, "method");
			return computeCompletionProposalsOutsideExpression(declaration);
		}
		
		private ICompletionProposal[] computeCompletionProposalsInProperty(PropertyDeclarationNode declaration) {
			var get = declaration.GetAccessor;
			var set = declaration.SetAccessor;
			TypeInfo t = null;
			if (get != null) {
				var m = get.getUserData(typeof(MethodInfo));
				t = m.ReturnType;
			} else if (set != null) {
				var m = set.getUserData(typeof(MethodInfo));
				t = m.Parameters.first().Type;
			}
			
			if (containsOffset(declaration.Type)) {
				return computeCompletionProposalsInType(declaration.Type, t);
			}
			if (get != null && containsOffset(get)) {
				var body = get.Body;
				if (body != null && containsOffset(body)) {
					context.enterPropertyGetter(declaration);
					return statementProcessor.handleStatement(body, null);
				}
		
				// TODO
				Environment.trace(this, "property getter");
				return EMPTY_PROPOSALS;
			}
			if (set != null && containsOffset(set)) {
				var body = set.Body;
				if (body != null && containsOffset(body)) {
					context.enterPropertySetter(declaration);
					return statementProcessor.handleStatement(body, null);
				}
		
				// TODO
				Environment.trace(this, "property setter");
				return EMPTY_PROPOSALS;
			}
			
			// TODO
			Environment.trace(this, "property");
			return computeCompletionProposalsOutsideExpression(declaration);
		}
		
		private ICompletionProposal[] computeCompletionProposalsInIndexer(IndexerDeclarationNode declaration) {
			var get = declaration.GetAccessor;
			var set = declaration.SetAccessor;
			TypeInfo t = null;
			MethodInfo m = null;
			if (get != null) {
				m = get.getUserData(typeof(MethodInfo));
				t = m.ReturnType;
			} else if (set != null) {
				m = set.getUserData(typeof(MethodInfo));
				t = m.Parameters.last().Type;
			}
			
			if (containsOffset(declaration.Type)) {
				return computeCompletionProposalsInType(declaration.Type, t);
			}
			
			var pit = m.Parameters.iterator();
			foreach (var p in declaration.Parameters) {
				var pt = pit.next().Type;
				
				var bounds = p.getUserData(typeof(SyntaxNodeBounds));
				if (bounds == null || bounds.isAfterOffset(offset)) {
					break;
				}
				if (bounds.isBeforeOffset(offset)) {
					continue;
				}
			
				if (containsOffset(p.Type)) {
					return computeCompletionProposalsInType(p.Type, pt);
				}
	
				Environment.trace(this, "indexer parameter");
				return EMPTY_PROPOSALS;
			}
			var accessor = declaration.GetAccessor;
			if (accessor != null && containsOffset(accessor)) {
				var body = accessor.Body;
				if (body != null && containsOffset(body)) {
					context.enterIndexerGetter(declaration);
					return statementProcessor.handleStatement(body, null);
				}
		
				// TODO
				Environment.trace(this, "indexer getter");
				return EMPTY_PROPOSALS;
			}
			accessor = declaration.SetAccessor;
			if (accessor != null && containsOffset(accessor)) {
				var body = accessor.Body;
				if (body != null && containsOffset(body)) {
					context.enterIndexerSetter(declaration);
					return statementProcessor.handleStatement(body, null);
				}
		
				// TODO
				Environment.trace(this, "indexer setter");
				return EMPTY_PROPOSALS;
			}
		
			// TODO
			Environment.trace(this, "indexer");
			return computeCompletionProposalsOutsideExpression(declaration);
		}
		
		private ICompletionProposal[] computeCompletionProposalsInConstructor(ConstructorDeclarationNode declaration) {
			var pit = declaration.getUserData(typeof(MethodInfo)).Parameters.iterator();
			foreach (var p in declaration.Parameters) {
				var pt = pit.next().Type;
			
				var bounds = p.getUserData(typeof(SyntaxNodeBounds));
				if (bounds == null || bounds.isAfterOffset(offset)) {
					break;
				}
				if (bounds.isBeforeOffset(offset)) {
					continue;
				}
			
				if (containsOffset(p.Type)) {
					return computeCompletionProposalsInType(p.Type, pt);
				}
	
				Environment.trace(this, "constructor parameter");
				return computeCompletionProposalsOutsideExpression(declaration);
			}
		
			var body = declaration.Body;
			if (body != null && containsOffset(body)) {
				context.enterConstructor(declaration);
				return statementProcessor.handleStatement(body, null);
			}
			
			Environment.trace(this, "constructor");
			return computeCompletionProposalsOutsideExpression(declaration);
		}
		
		private ICompletionProposal[] computeCompletionProposalsInDestructor(DestructorDeclarationNode declaration) {
			var body = declaration.Body;
			if (body != null && containsOffset(body)) {
				return statementProcessor.handleStatement(body, null);
			}
		
			Environment.trace(this, "destructor");
			return computeCompletionProposalsOutsideExpression(declaration);
		}
		
		private ICompletionProposal[] computeCompletionProposalsInField(FieldDeclarationNode declaration) {
			var f = declaration.Declarators[0].getUserData(typeof(FieldInfo));
			
			if (containsOffset(declaration.Type)) {
				return computeCompletionProposalsInType(declaration.Type, f.Type);
			}

			foreach (var decl in declaration.Declarators) {
				var bounds = decl.getUserData(typeof(SyntaxNodeBounds));
				if (bounds == null || bounds.isAfterOffset(offset)) {
					break;
				}
				if (bounds.isBeforeOffset(offset)) {
					continue;
				}
				
				Environment.trace(this, "field declarator");
				if (decl.Value != null) {
					return expressionProcessor.handleExpression(decl.Value, null, true);
				}
				return EMPTY_PROPOSALS;
			}
			
			Environment.trace(this, "field");
			return computeCompletionProposalsOutsideExpression(declaration);
		}
		
		private ICompletionProposal[] computeCompletionProposalsInEnumConstant(EnumConstantDeclarationNode declaration) {
			foreach (var e in declaration.Arguments) {
				var bounds = e.getUserData(typeof(SyntaxNodeBounds));
				if (bounds == null || bounds.isAfterOffset(offset)) {
					break;
				}
				if (bounds.isBeforeOffset(offset)) {
					continue;
				}
				return expressionProcessor.handleExpression(e, null, true);
			}
		
			Environment.trace(this, "enum constant");
			return EMPTY_PROPOSALS;
		}

		private ICompletionProposal[] computeCompletionProposalsInSimpleType(TypeReferenceNode type) {
			var token = this.token;
			if (token.offset == offset) {
				token = tokens[tokenIndex - 1];
			}
			var length = token.length - (offset - token.offset);
			var prefix = new String(text, token.offset, offset - token.offset);
			return CompletionProposalHelper.createCompletionProposals(offset, length, prefix, context.getRootPackages(),
					context.getTypeMembers(), TYPE_TEXTS);
		}
		
		private ICompletionProposal[] computeCompletionProposalsInType(TypeReferenceNode type, TypeInfo Type) {
			Environment.trace(this, "type");

			if (inEditedText) {
				Environment.trace(this, "edited type");
				return EMPTY_PROPOSALS;
			}
			
			switch (type.TypeReferenceKind) {
			case Boolean:
			case Byte:
			case Char:
			case Double:
			case Float:
			case Int:
			case Long:
			case Short:
			case Void:
				return computeCompletionProposalsInSimpleType(type);
				
			case SimpleName:
				var simple = (SimpleNameTypeReferenceNode)type;
				var bounds = simple.getUserData(typeof(SyntaxNodeBounds));
				var nameToken = bounds.StartToken;
				if (offset <= nameToken.offset + nameToken.length) {
					return computeCompletionProposalsInSimpleType(type);
				} else {
					var ait = Type.GenericArguments.iterator();
					foreach (var arg in simple.TypeArguments) {
						var at = ait.next();
					
						bounds = arg.getUserData(typeof(SyntaxNodeBounds));
						if (bounds == null || bounds.isAfterOffset(offset)) {
							break;
						}
						if (bounds.isBeforeOffset(offset)) {
							continue;
						}
						return computeCompletionProposalsInType(arg, at);
					}
					break;
				}
	
			case Qualified:
				var qualified = (QualifiedTypeReferenceNode)type;
				if (containsOffset(qualified.SimpleName)) {
					String prefix;
					int length;
					if (offset == token.offset) {
						var tk = tokens[tokenIndex - 1];
						length = 0;
						prefix = new String(text, tk.offset, tk.length);
					} else {
						length = token.length - (offset - token.offset);
						prefix = new String(text, token.offset, offset - token.offset);
					}
					Iterable<MemberInfo> typeMembers;
					if (Type.DeclaringType == null) {
						typeMembers = context.getTypeMembers().where(p => p.Type.PackageName.equals(Type.PackageName));
					} else {
						typeMembers = context.getTypeMembers().where(p => p.Type.DeclaringType == Type.DeclaringType);
					}
					return CompletionProposalHelper.createCompletionProposals(offset, length, prefix, Query.empty<String>(),
							typeMembers, TYPE_TEXTS);
				} else if (containsOffset(qualified.EnclosingType)) {
					if (Type.DeclaringType != null) {
						return computeCompletionProposalsInType(qualified.EnclosingType, Type.DeclaringType);
					}
				}
				break;
						
			case Array:
				var arrayType = (ArrayTypeReferenceNode)type;
				if (containsOffset(arrayType.ElementType)) {
					return computeCompletionProposalsInType(arrayType.ElementType, Type.ElementType);
				}
				break;
				
			case LowerBoundedWildcard:
			case UpperBoundedWildcard:
				var wildcard = (WildcardTypeReferenceNode)type;
				if (containsOffset(wildcard.Bound)) {
					return computeCompletionProposalsInType(wildcard.Bound, Type.WildcardBound);
				}
				break;
			}
		
			return CompletionProposalHelper.createCompletionProposals(offset, 0, "", context.getRootPackages(),
					context.getTypeMembers(), TYPE_TEXTS);
		}
		
		private ICompletionProposal[] computeCompletionProposalsInStatement(SyntaxNode syntaxNode) {
			return computeCompletionProposals(syntaxNode, false);
		}

		private ICompletionProposal[] computeCompletionProposalsInExpression(SyntaxNode syntaxNode) {
			return computeCompletionProposals(syntaxNode, true);
		}

		private ICompletionProposal[] computeCompletionProposals(SyntaxNode syntaxNode, bool expression) {
			var prefix = "";
			int length = 0;
			if (inEditedText) {
				var editedTokens = getEditedTokens(syntaxNode).toList();
				int size = editedTokens.size();
				if (size == 1 || (size > 1 && isExpressionSeparator(editedTokens[size - 2]))) {
					if (expression && editedTokens[size - 1].LexicalUnit == LexicalUnit.Dot) {
						var tinfo = syntaxNode.getUserData(typeof(ExpressionInfo));
						if (actualToken.offset < actualOffset) {
							length = actualToken.length - (actualOffset - actualToken.offset);
							prefix = new String(text, actualToken.offset, actualOffset - actualToken.offset);
						}
						return CompletionProposalHelper.createCompletionProposals(actualOffset, length, prefix, Query.empty<String>(),
								context.getMembers(tinfo.Type, tinfo.Member != null && tinfo.Member.MemberKind == MemberKind.Type),
								Query.empty<String>());
					}
				} else if ((size == 2 || (size > 2 && isExpressionSeparator(editedTokens[size - 3]))) &&
						editedTokens[size - 1].LexicalUnit == LexicalUnit.Dot) {
					var tk = editedTokens[size - 2];
					switch (tk.LexicalUnit) {
					case Identifier:
					case Keyword:
					case ContextualKeyword:
						if (actualToken.offset < actualOffset) {
							length = actualToken.length - (actualOffset - actualToken.offset);
							prefix = new String(text, actualToken.offset, actualOffset - actualToken.offset);
						}
						var text = new String(text, tk.offset, tk.length);
						return CompletionProposalHelper.createCompletionProposals(actualOffset, length, prefix, Query.empty<String>(),
								context.getMembers(text), Query.empty<String>());
					}
				} else if ((size == 3 || (size > 3 && isExpressionSeparator(editedTokens[size - 4]))) &&
						editedTokens[size - 2].LexicalUnit == LexicalUnit.Dot && actualToken.offset == actualOffset) {
					var tk = editedTokens[size - 3];
					switch (tk.LexicalUnit) {
					case Identifier:
					case Keyword:
					case ContextualKeyword:
						var tk2 = editedTokens[size - 1];
						switch (tk2.LexicalUnit) {
						case Identifier:
						case Keyword:
						case ContextualKeyword:
							prefix = new String(text, tk2.offset, actualOffset - tk2.offset);
							var text = new String(text, tk.offset, tk.length);
							return CompletionProposalHelper.createCompletionProposals(actualOffset, length, prefix, Query.empty<String>(),
									context.getMembers(text), Query.empty<String>());
						}
						break;
					}
				}
			}
			
			var token = this.actualToken;
			if (token.offset == actualOffset) {
				token = tokens[actualTokenIndex - 1];
			}
			switch (token.LexicalUnit) {
			case Identifier:
			case Keyword:
			case ContextualKeyword:
				length = token.length - (actualOffset - token.offset);
				prefix = new String(text, token.offset, actualOffset - token.offset);
				break;
			}
			if (expression) {
				return CompletionProposalHelper.createCompletionProposals(actualOffset, length, prefix,
						(length == 0) ? context.getRootPackages() : Query.empty<String>(),
						context.getMembers(), EXPRESSION_TEXTS);
			} else {
				return CompletionProposalHelper.createCompletionProposals(actualOffset, length, prefix,
						(length == 0) ? context.getRootPackages() : Query.empty<String>(),
						context.getMembers(), STATEMENT_TEXTS);
			}
		}

		private ICompletionProposal[] computeCompletionProposalsOutsideExpression(SyntaxNode syntaxNode) {
			var prefix = "";
			int length = 0;
			if (inEditedText) {
				Environment.trace(this, "edited outside expression");
			}
			
			var token = this.actualToken;
			if (token.offset == actualOffset) {
				token = tokens[actualTokenIndex - 1];
			}
			switch (token.LexicalUnit) {
			case Identifier:
			case Keyword:
			case ContextualKeyword:
				length = token.length - (actualOffset - token.offset);
				prefix = new String(text, token.offset, actualOffset - token.offset);
				break;
			}
			return CompletionProposalHelper.createCompletionProposals(actualOffset, length, prefix,
					(length == 0) ? context.getRootPackages() : Query.empty<String>(),
					context.getTypeMembers(), TYPE_TEXTS);
		}
		
		private Iterable<Token> getEditedTokens(SyntaxNode syntaxNode) {
			for (int i = tokenIndex; i < actualTokenIndex; i++) {
				var tk = tokens[i];
				switch (tk.LexicalUnit) {
				case Whitespace:
				case DelimitedComment:
				case SingleLineComment:
				case NewLine:
					break;
				default:
					yield return tk;
					break;
				}
			}
		}
		
		private static bool isExpressionSeparator(Token token) {
			switch (token.LexicalUnit) {
			case Dot:
			case Identifier:
			case StringLiteral:
			case VerbatimIdentifier:
			case VerbatimStringLiteral:
				return false;
			case ContextualKeyword:
			case Keyword:
				switch (token.Keyword) {
				case Break:
				case Return:
					return true;
				default:
					return false;
				}
			default:
				return true;
			}
		}

		private bool containsOffset(SyntaxNode syntaxNode) {
			var bounds = syntaxNode.getUserData(typeof(SyntaxNodeBounds));
			if (bounds == null) {
				return false;
			}
			return bounds.containsOffset(offset);
		}
		
		private bool editedText(SyntaxNode syntaxNode) {
			var bounds = syntaxNode.getUserData(typeof(SyntaxNodeBounds));
			var startToken = bounds.StartToken;
			var endToken = bounds.EndToken;
			return endToken.offset + endToken.length - startToken.offset != syntaxNode.EndPosition - syntaxNode.StartPosition;
		}
		
		private bool isAfterEndToken(SyntaxNode syntaxNode) {
			var bounds = syntaxNode.getUserData(typeof(SyntaxNodeBounds));
			var endToken = bounds.EndToken;
			return endToken.offset + endToken.length == offset;
		}
		
		private Token getPreviousToken(int offset) {
			int index = Highlighter.getPositionIndex(tokens, offset);
			for (int i = index - 1; i >= 0; --i) {
				var token = tokens[i];
				switch (token.LexicalUnit) {
				case Whitespace:
				case DelimitedComment:
				case SingleLineComment:
				case NewLine:
					break;
				default:
					return token;
				}
			}
			return null;
		}

		private class StatementProcessor : StatementHandler<Void, ICompletionProposal[]> {
			private ContentAssistProcessor processor;
		
			StatementProcessor(ContentAssistProcessor processor)
					: super(false) {
				this.processor = processor;
			}
			
			protected override ICompletionProposal[] handleBlock(BlockStatementNode block, Void source) {
				foreach (var s in block.Statements) {
					var bounds = s.getUserData(typeof(SyntaxNodeBounds));
					if (bounds == null || bounds.isAfterOffset(processor.offset)) {
						break;
					}
					if (bounds.isBeforeOffset(processor.offset)) {
						if (s.StatementKind == StatementKind.LocalDeclaration) {
							processor.context.declareLocals((LocalDeclarationStatementNode)s);
						}
					} else {
						return handleStatement(s, null);
					}
				}
		
				Environment.trace(this, "block");
				return processor.computeCompletionProposalsInStatement(block);
			}
			
			protected override ICompletionProposal[] handleBreak(BreakStatementNode breakStatement, Void source) {
				Environment.trace(this, "break");
				return processor.computeCompletionProposalsInStatement(breakStatement);
			}
	
			protected override ICompletionProposal[] handleContinue(ContinueStatementNode continueStatement, Void source) {
				Environment.trace(this, "continue");
				return processor.computeCompletionProposalsInStatement(continueStatement);
			}
	
			protected override ICompletionProposal[] handleDo(DoStatementNode doStatement, Void source) {
				if (processor.containsOffset(doStatement.Statement)) {
					return handleStatement(doStatement.Statement, null);
				}
				if (processor.containsOffset(doStatement.Condition)) {
					return processor.expressionProcessor.handleExpression(doStatement.Condition, null, true);
				}
		
				Environment.trace(this, "do");
				return processor.computeCompletionProposalsInStatement(doStatement);
			}
			
			protected override ICompletionProposal[] handleExpression(ExpressionStatementNode expression, Void source) {
				if (processor.containsOffset(expression.Expression)) {
					return processor.expressionProcessor.handleExpression(expression.Expression, null, false);
				}
				
				Environment.trace(this, "expression statement");
				if (processor.isAfterEndToken(expression)) {
					return processor.computeCompletionProposalsInStatement(expression);
				} else {
					return processor.computeCompletionProposalsInExpression(expression);
				}
			}
			
			protected override ICompletionProposal[] handleFor(ForStatementNode forStatement, Void source) {
				foreach (var s in forStatement.Initializer) {
					var bounds = s.getUserData(typeof(SyntaxNodeBounds));
					if (bounds == null || bounds.isAfterOffset(processor.offset)) {
						break;
					}
					if (bounds.isBeforeOffset(processor.offset)) {
						if (s.StatementKind == StatementKind.LocalDeclaration) {
							processor.context.declareLocals((LocalDeclarationStatementNode)s);
						}
					} else {
						return handleStatement(s, null);
					}
				}
				if (forStatement.Condition != null && processor.containsOffset(forStatement.Condition)) {
					return processor.expressionProcessor.handleExpression(forStatement.Condition, null, true);
				}
				foreach (var s in forStatement.Iterator) {
					var bounds = s.getUserData(typeof(SyntaxNodeBounds));
					if (bounds == null || bounds.isAfterOffset(processor.offset)) {
						break;
					}
					if (bounds.isBeforeOffset(processor.offset)) {
						continue;
					}
					return handleStatement(s, null);
				}
				if (processor.containsOffset(forStatement.Statement)) {
					return handleStatement(forStatement.Statement, null);
				}
		
				Environment.trace(this, "for");
				return processor.computeCompletionProposalsInStatement(forStatement);
			}
			
			protected override ICompletionProposal[] handleForeach(ForeachStatementNode foreachStatement, Void source) {
				if (foreachStatement.Type != null && processor.containsOffset(foreachStatement.Type)) {
					var info = foreachStatement.getUserData(typeof(MemberInfo));
					return processor.computeCompletionProposalsInType(foreachStatement.Type, info.Type);
				}
				if (processor.containsOffset(foreachStatement.Source)) {
					return processor.expressionProcessor.handleExpression(foreachStatement.Source, null, true);
				}
				if (processor.containsOffset(foreachStatement.Statement)) {
					processor.context.declareLocal(foreachStatement);
					return handleStatement(foreachStatement.Statement, null);
				}
		
				Environment.trace(this, "foreach");
				return processor.computeCompletionProposalsInStatement(foreachStatement);
			}
			
			protected override ICompletionProposal[] handleGoto(GotoStatementNode gotoStatement, Void source) {
				Environment.trace(this, "goto");
				return processor.computeCompletionProposalsInStatement(gotoStatement);
			}
			
			protected override ICompletionProposal[] handleGotoCase(GotoCaseStatementNode gotoCase, Void source) {
				// TODO
				Environment.trace(this, "goto case");
				return EMPTY_PROPOSALS;
			}
			
			protected override ICompletionProposal[] handleIf(IfStatementNode ifStatement, Void source) {
				if (processor.containsOffset(ifStatement.Condition)) {
					return processor.expressionProcessor.handleExpression(ifStatement.Condition, null, true);
				}
				if (processor.containsOffset(ifStatement.IfTrue)) {
					return handleStatement(ifStatement.IfTrue, null);
				}
				if (ifStatement.IfFalse != null && processor.containsOffset(ifStatement.IfFalse)) {
					return handleStatement(ifStatement.IfFalse, null);
				}
		
				Environment.trace(this, "if");
				return processor.computeCompletionProposalsInStatement(ifStatement);
			}
			
			protected override ICompletionProposal[] handleLabeled(LabeledStatementNode labeled, Void source) {
				if (processor.containsOffset(labeled.Statement)) {
					return handleStatement(labeled.Statement, null);
				}
		
				Environment.trace(this, "labeled");
				return processor.computeCompletionProposalsInStatement(labeled);
			}
			
			protected override ICompletionProposal[] handleLocalDeclaration(LocalDeclarationStatementNode localDeclaration, Void source) {
				if (localDeclaration.Type != null && processor.containsOffset(localDeclaration.Type)) {
					var info = localDeclaration.getUserData(typeof(ExpressionInfo));
					return processor.computeCompletionProposalsInType(localDeclaration.Type, info.Type);
				}
				foreach (var decl in localDeclaration.Declarators) {
					if (decl.Value != null && processor.containsOffset(decl.Value)) {
						return processor.expressionProcessor.handleExpression(decl.Value, null, true);
					}
				}
		
				Environment.trace(this, "local declaration");
				return processor.computeCompletionProposalsInStatement(localDeclaration);
			}
			
			protected override ICompletionProposal[] handleReturn(ReturnStatementNode returnStatement, Void source) {
				if (returnStatement.Value != null && processor.containsOffset(returnStatement.Value)) {
					return processor.expressionProcessor.handleExpression(returnStatement.Value, null, false);
				}
		
				Environment.trace(this, "return");
				var bounds = returnStatement.getUserData(typeof(SyntaxNodeBounds));
				var startToken = bounds.StartToken;
				if (processor.offset > startToken.offset + startToken.length && processor.offset < returnStatement.EndPosition) {
					return processor.computeCompletionProposalsInExpression(returnStatement);
				}
				return processor.computeCompletionProposalsInStatement(returnStatement);
			}
			
			protected override ICompletionProposal[] handleSwitch(SwitchStatementNode switchStatement, Void source) {
				if (processor.containsOffset(switchStatement.Selector)) {
					return processor.expressionProcessor.handleExpression(switchStatement.Selector, null, true);
				}
				foreach (var section in switchStatement.Sections) {
					var bounds = section.getUserData(typeof(SyntaxNodeBounds));
					if (bounds == null || bounds.isAfterOffset(processor.offset)) {
						break;
					}
					foreach (var s in section.Statements) {
						if (processor.containsOffset(s)) {
							return handleStatement(s, null);
						}
						if (s.StatementKind == StatementKind.LocalDeclaration) {
							processor.context.declareLocals((LocalDeclarationStatementNode)s);
						}
					}
				}
		
				Environment.trace(this, "switch");
				return processor.computeCompletionProposalsInStatement(switchStatement);
			}
			
			protected override ICompletionProposal[] handleSynchronized(SynchronizedStatementNode synchronizedStatement, Void source) {
				if (processor.containsOffset(synchronizedStatement.Lock)) {
					return processor.expressionProcessor.handleExpression(synchronizedStatement.Lock, null, true);
				}
				if (processor.containsOffset(synchronizedStatement.Statement)) {
					return handleStatement(synchronizedStatement.Statement, null);
				}
		
				Environment.trace(this, "synchronized");
				return processor.computeCompletionProposalsInStatement(synchronizedStatement);
			}
			
			protected override ICompletionProposal[] handleThrow(ThrowStatementNode throwStatement, Void source) {
				if (throwStatement.Exception != null && processor.containsOffset(throwStatement.Exception)) {
					return processor.expressionProcessor.handleExpression(throwStatement.Exception, null, true);
				}
		
				Environment.trace(this, "throw");
				return processor.computeCompletionProposalsInStatement(throwStatement);
			}
			
			protected override ICompletionProposal[] handleTry(TryStatementNode tryStatement, Void source) {
				if (processor.containsOffset(tryStatement.Block)) {
					return handleBlock(tryStatement.Block, null);
				}
				foreach (var cc in tryStatement.CatchClauses) {
					var bounds = cc.getUserData(typeof(SyntaxNodeBounds));
					if (bounds == null || bounds.isAfterOffset(processor.offset)) {
						break;
					}
					if (bounds.isBeforeOffset(processor.offset)) {
						continue;
					}
					if (cc.ExceptionType != null && processor.containsOffset(cc.ExceptionType)) {
						processor.context.declareLocal(cc);
						var t = cc.getUserData(typeof(TypeInfo));
						return processor.computeCompletionProposalsInType(cc.ExceptionType, t);
					}
					if (processor.containsOffset(cc.Block)) {
						return handleBlock(cc.Block, null);
					}
				}
				if (tryStatement.Finally != null && processor.containsOffset(tryStatement.Finally)) {
					return handleBlock(tryStatement.Finally, null);
				}
		
				Environment.trace(this, "try");
				return processor.computeCompletionProposalsInStatement(tryStatement);
			}
			
			protected override ICompletionProposal[] handleUsing(UsingStatementNode usingStatement, Void source) {
				if (processor.containsOffset(usingStatement.ResourceAcquisition)) {
					return handleStatement(usingStatement.ResourceAcquisition, null);
				}
				if (processor.containsOffset(usingStatement.Statement)) {
					if (usingStatement.ResourceAcquisition.StatementKind == StatementKind.LocalDeclaration) {
						processor.context.declareLocals((LocalDeclarationStatementNode)usingStatement.ResourceAcquisition);
					}
					return handleStatement(usingStatement.Statement, null);
				}
		
				Environment.trace(this, "using");
				return processor.computeCompletionProposalsInStatement(usingStatement);
			}
			
			protected override ICompletionProposal[] handleWhile(WhileStatementNode whileStatement, Void source) {
				if (processor.containsOffset(whileStatement.Condition)) {
					return processor.expressionProcessor.handleExpression(whileStatement.Condition, null, true);
				}
				if (processor.containsOffset(whileStatement.Statement)) {
					return handleStatement(whileStatement.Statement, null);
				}
		
				Environment.trace(this, "while");
				return processor.computeCompletionProposalsInStatement(whileStatement);
			}
			
			protected override ICompletionProposal[] handleYield(YieldStatementNode yield, Void source) {
				if (yield.Value != null && processor.containsOffset(yield.Value)) {
					return processor.expressionProcessor.handleExpression(yield.Value, null, false);
				}
		
				Environment.trace(this, "yield");
				var bounds = yield.getUserData(typeof(SyntaxNodeBounds));
				var startToken = bounds.StartToken;
				// TODO: skip return or break
				if (processor.offset > startToken.offset + startToken.length && processor.offset < yield.EndPosition) {
					return processor.computeCompletionProposalsInExpression(yield);
				}
				return processor.computeCompletionProposalsInStatement(yield);
			}
		}
		
		private class ExpressionProcessor : ExpressionHandler<Void, ICompletionProposal[]> {
			private ContentAssistProcessor processor;
			
			ExpressionProcessor(ContentAssistProcessor processor)
					: super(false) {
				this.processor = processor;
			}
			
			protected override ICompletionProposal[] handleAnonymousObjectCreation(AnonymousObjectCreationExpressionNode anonymousObject,
					Void source, bool nested) {
				foreach (var mi in anonymousObject.MemberDeclarators) {
					var bounds = mi.getUserData(typeof(SyntaxNodeBounds));
					if (bounds == null || bounds.isAfterOffset(processor.offset)) {
						break;
					}
					if (bounds.isBeforeOffset(processor.offset)) {
						continue;
					}
					if (processor.containsOffset(mi.Value)) {
						return handleExpression(mi.Value, source, true);
					}
			
					Environment.trace(this, "member declarator");
					return EMPTY_PROPOSALS;
				}
		
				Environment.trace(this, "anonymous object");
				return processor.computeCompletionProposalsInExpression(anonymousObject);
			}
			
			protected override ICompletionProposal[] handleArrayCreation(ArrayCreationExpressionNode arrayCreation, Void source, bool nested) {
				if (arrayCreation.Type != null && processor.containsOffset(arrayCreation.Type)) {
					var info = arrayCreation.getUserData(typeof(ExpressionInfo));
					return processor.computeCompletionProposalsInType(arrayCreation.Type, info.Type);
				}
				foreach (var e in arrayCreation.DimensionExpressions) {
					var bounds = e.getUserData(typeof(SyntaxNodeBounds));
					if (bounds == null || bounds.isAfterOffset(processor.offset)) {
						break;
					}
					if (bounds.isBeforeOffset(processor.offset)) {
						continue;
					}
					return handleExpression(e, source, true);
				}
				if (arrayCreation.Initializer != null && processor.containsOffset(arrayCreation.Initializer)) {
					return handleExpression(arrayCreation.Initializer, null, true);
				}
		
				Environment.trace(this, "array creation");
				return processor.computeCompletionProposalsInExpression(arrayCreation);
			}
			
			protected override ICompletionProposal[] handleArrayInitializer(ArrayInitializerExpressionNode arrayInitializer, Void source,
					bool nested) {
				foreach (var e in arrayInitializer.Values) {
					var bounds = e.getUserData(typeof(SyntaxNodeBounds));
					if (bounds == null || bounds.isAfterOffset(processor.offset)) {
						break;
					}
					if (bounds.isBeforeOffset(processor.offset)) {
						continue;
					}
					return handleExpression(e, source, true);
				}
		
				Environment.trace(this, "array initializer");
				return processor.computeCompletionProposalsInExpression(arrayInitializer);
			}
			
			protected override ICompletionProposal[] handleAssign(AssignExpressionNode assign, Void source, bool nested) {
				if (processor.containsOffset(assign.Left)) {
					return handleExpression(assign.Left, null, true);
				}
				if (processor.containsOffset(assign.Right)) {
					return handleExpression(assign.Right, null, true);
				}
		
				Environment.trace(this, "assign");
				return processor.computeCompletionProposalsInExpression(assign);
			}
			
			protected override ICompletionProposal[] handleBinary(BinaryExpressionNode binary, Void source, bool nested) {
				if (processor.containsOffset(binary.LeftOperand)) {
					return handleExpression(binary.LeftOperand, null, true);
				}
				if (processor.containsOffset(binary.RightOperand)) {
					return handleExpression(binary.RightOperand, null, true);
				}
		
				Environment.trace(this, "binary");
				return processor.computeCompletionProposalsInExpression(binary);
			}
	
			protected override ICompletionProposal[] handleCast(CastExpressionNode cast, Void source, bool nested) {
				if (processor.containsOffset(cast.TargetType)) {
				var info = cast.getUserData(typeof(ExpressionInfo));
					return processor.computeCompletionProposalsInType(cast.TargetType, info.Type);
				}
				if (processor.containsOffset(cast.Expression)) {
					return handleExpression(cast.Expression, null, true);
				}
		
				Environment.trace(this, "cast");
				return processor.computeCompletionProposalsInExpression(cast);
			}
			
			protected override ICompletionProposal[] handleConditional(ConditionalExpressionNode conditional, Void source, bool nested) {
				if (processor.containsOffset(conditional.Condition)) {
					return handleExpression(conditional.Condition, null, true);
				}
				if (processor.containsOffset(conditional.IfTrue)) {
					return handleExpression(conditional.IfTrue, null, true);
				}
				if (processor.containsOffset(conditional.IfFalse)) {
					return handleExpression(conditional.IfFalse, null, true);
				}
		
				Environment.trace(this, "conditional");
				return processor.computeCompletionProposalsInExpression(conditional);
			}
			
			protected override ICompletionProposal[] handleElementAccess(ElementAccessExpressionNode elementAccess, Void source, bool nested) {
				if (processor.containsOffset(elementAccess.TargetObject)) {
					return handleExpression(elementAccess.TargetObject, null, true);
				}
				foreach (var e in elementAccess.Indexes) {
					var bounds = e.getUserData(typeof(SyntaxNodeBounds));
					if (bounds == null || bounds.isAfterOffset(processor.offset)) {
						break;
					}
					if (bounds.isBeforeOffset(processor.offset)) {
						continue;
					}
					return handleExpression(e, source, true);
				}
		
				Environment.trace(this, "element access");
				return processor.computeCompletionProposalsInExpression(elementAccess);
			}
			
			protected override ICompletionProposal[] handleInvocation(InvocationExpressionNode invocation, Void source, bool nested) {
				if (processor.containsOffset(invocation.TargetObject)) {
					return handleExpression(invocation.TargetObject, null, true);
				}
				foreach (var e in invocation.Arguments) {
					var bounds = e.getUserData(typeof(SyntaxNodeBounds));
					if (bounds == null || bounds.isAfterOffset(processor.offset)) {
						break;
					}
					if (bounds.isBeforeOffset(processor.offset)) {
						continue;
					}
					return handleExpression(e, source, true);
				}
		
				Environment.trace(this, "invocation");
				return processor.computeCompletionProposalsInExpression(invocation);
			}
			
			protected override ICompletionProposal[] handleLambda(LambdaExpressionNode lambda, Void source, bool nested) {
				var m = lambda.getUserData(typeof(MethodInfo));
				var pit = m.Parameters.iterator();
				foreach (var p in lambda.Parameters) {
					var pt = pit.next().Type;
					
					var bounds = p.getUserData(typeof(SyntaxNodeBounds));
					if (bounds == null || bounds.isAfterOffset(processor.offset)) {
						break;
					}
					if (bounds.isBeforeOffset(processor.offset)) {
						continue;
					}
				
					if (p.Type != null && processor.containsOffset(p.Type)) {
						return processor.computeCompletionProposalsInType(p.Type, pt);
					}
		
					Environment.trace(this, "lambda parameter");
					return EMPTY_PROPOSALS;
				}
				
				if (processor.containsOffset(lambda.Body)) {
		            processor.context.enterLambda(lambda);
					return processor.statementProcessor.handleStatement(lambda.Body, null);
				}
		
				Environment.trace(this, "lambda");
				return EMPTY_PROPOSALS;
			}

			protected override ICompletionProposal[] handleLiteral(LiteralExpressionNode literal, Void source, bool nested) {
				Environment.trace(this, "literal");
				return processor.computeCompletionProposalsInExpression(literal);
			}
			
			protected override ICompletionProposal[] handleMemberAccess(MemberAccessExpressionNode memberAccess, Void source, bool nested) {
				if (processor.containsOffset(memberAccess.TargetObject)) {
					return handleExpression(memberAccess.TargetObject, null, true);
				}
				if (processor.containsOffset(memberAccess.Member)) {
					Environment.trace(this, "member access name");
					int index = Highlighter.getPositionIndex(processor.tokens, processor.offset);
					var token = processor.tokens[index];
					if (token.offset == processor.offset) {
						token = processor.tokens[index - 1];
					}
					var length = token.length - (processor.offset - token.offset);
					var prefix = new String(processor.text, token.offset, processor.offset - token.offset);
					var tinfo = memberAccess.TargetObject.getUserData(typeof(ExpressionInfo));
					return CompletionProposalHelper.createCompletionProposals(processor.offset, length, prefix, Query.empty<String>(),
							processor.context.getMembers(tinfo.Type, tinfo.Member != null && tinfo.Member.MemberKind == MemberKind.Type),
							Query.empty<String>());
				}
		
				Environment.trace(this, "member access");
				var token = processor.getPreviousToken(processor.offset);
				if (token.LexicalUnit == LexicalUnit.Dot) {
					if (processor.inEditedText) {
						return processor.computeCompletionProposalsInExpression(memberAccess.TargetObject);
					} else {
						var info = memberAccess.getUserData(typeof(ExpressionInfo));
						var member = info.Member;
						if (member == null) {
							var type = info.Type;
							if (type == null) {
								// Package
							} else {
								return CompletionProposalHelper.createCompletionProposals(processor.offset, 0, "", Query.empty<String>(),
										processor.context.getMembers(type, true), Query.empty<String>());
							}
						} else {
							var tinfo = memberAccess.TargetObject.getUserData(typeof(ExpressionInfo));
							return CompletionProposalHelper.createCompletionProposals(processor.offset, 0, "", Query.empty<String>(),
									processor.context.getMembers(tinfo.Type, tinfo.Member != null && tinfo.Member.MemberKind == MemberKind.Type),
									Query.empty<String>());
						}
					}
				}
				return EMPTY_PROPOSALS;
			}
			
			protected override ICompletionProposal[] handleObjectCreation(ObjectCreationExpressionNode objectCreation, Void source, bool nested) {
				if (processor.containsOffset(objectCreation.Type)) {
					var info = objectCreation.getUserData(typeof(ExpressionInfo));
					return processor.computeCompletionProposalsInType(objectCreation.Type, info.Type);
				}
				foreach (var e in objectCreation.Arguments) {
					var bounds = e.getUserData(typeof(SyntaxNodeBounds));
					if (bounds == null || bounds.isAfterOffset(processor.offset)) {
						break;
					}
					if (bounds.isBeforeOffset(processor.offset)) {
						continue;
					}
					return handleExpression(e, source, true);
				}
				if (objectCreation.Initializer != null && processor.containsOffset(objectCreation.Initializer)) {
					return handleExpression(objectCreation.Initializer, null, true);
				}
		
				Environment.trace(this, "object creation");
				return processor.computeCompletionProposalsInExpression(objectCreation);
			}
			
			protected override ICompletionProposal[] handleCollectionInitializer(CollectionInitializerExpressionNode initializer, Void source,
					bool nested) {
				foreach (var e in initializer.Values.selectMany(p => p)) {
					var bounds = e.getUserData(typeof(SyntaxNodeBounds));
					if (bounds == null || bounds.isAfterOffset(processor.offset)) {
						break;
					}
					if (bounds.isBeforeOffset(processor.offset)) {
						continue;
					}
					return handleExpression(e, source, true);
				}
		
				Environment.trace(this, "collection initializer");
				return processor.computeCompletionProposalsInExpression(initializer);
			}
			
			// TODO: handleQuery
			
			protected override ICompletionProposal[] handleObjectInitializer(ObjectInitializerExpressionNode initializer, Void source,
					bool nested) {
				foreach (var init in initializer.getMemberInitializers()) {
					var bounds = init.getUserData(typeof(SyntaxNodeBounds));
					if (bounds == null || bounds.isAfterOffset(processor.offset)) {
						break;
					}
					if (bounds.isBeforeOffset(processor.offset)) {
						continue;
					}
					if (processor.containsOffset(init.Value)) {
						return handleExpression(init.Value, null, true);
					}
			
					Environment.trace(this, "object member initializer");
					return processor.computeCompletionProposalsInExpression(initializer);
				}
		
				Environment.trace(this, "object initializer");
				return processor.computeCompletionProposalsInExpression(initializer);
			}
			
			protected override ICompletionProposal[] handleSimpleName(SimpleNameExpressionNode simpleName, Void source, bool nested) {
				Environment.trace(this, "simple name");
				return processor.computeCompletionProposalsInExpression(simpleName);
			}
			
	
			protected override ICompletionProposal[] handleSizeof(SizeofExpressionNode sizeofExpression, Void source, bool nested) {
				if (processor.containsOffset(sizeofExpression.Expression)) {
					return handleExpression(sizeofExpression.Expression, null, true);
				}
				Environment.trace(this, "sizeof");
				return processor.computeCompletionProposalsInExpression(sizeofExpression);
			}
			
			protected override ICompletionProposal[] handleSuperAccess(SuperAccessExpressionNode superAccess, Void source, bool nested) {
				Environment.trace(this, "super access");
				return processor.computeCompletionProposalsInExpression(superAccess);
			}
			
			protected override ICompletionProposal[] handleThisAccess(ThisAccessExpressionNode thisAccess, Void source, bool nested) {
				Environment.trace(this, "this access");
				return processor.computeCompletionProposalsInExpression(thisAccess);
			}
	
			protected override ICompletionProposal[] handleType(TypeExpressionNode type, Void source, bool nested) {
				var info = type.getUserData(typeof(ExpressionInfo));
				return processor.computeCompletionProposalsInType(type.TypeReference, info.Type);
			}
			
			protected override ICompletionProposal[] handleTypeof(TypeofExpressionNode typeofExpression, Void source, bool nested) {
				if (processor.containsOffset(typeofExpression.Type)) {
					return processor.computeCompletionProposalsInType(typeofExpression.Type, typeofExpression.getUserData(typeof(TypeInfo)));
				}
		
				Environment.trace(this, "typeof");
				if (!processor.inEditedText) {
					var token = processor.getPreviousToken(processor.offset);
					if (token != null && token.LexicalUnit == LexicalUnit.OpenParenthesis) {
						return CompletionProposalHelper.createCompletionProposals(processor.offset, 0, "", processor.context.getRootPackages(),
								processor.context.getTypeMembers(), TYPE_TEXTS);
					}
				}
				return processor.computeCompletionProposalsInExpression(typeofExpression);
			}
			
			protected override ICompletionProposal[] handleUnary(UnaryExpressionNode unary, Void source, bool nested) {
				if (processor.containsOffset(unary.Operand)) {
					return handleExpression(unary.Operand, null, true);
				}
				
				Environment.trace(this, "unary");
				return processor.computeCompletionProposalsInExpression(unary);
			}
		}
	}
}
