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
using cnatural.compiler;
using cnatural.syntaxtree;

namespace cnatural.eclipse.helpers {

	public class SyntaxTreeHelper {
		public static Iterable<TypeMemberNode> getTypeMembers(CompilationUnitNode compilationUnit) {
			var result = new ArrayList<TypeMemberNode>();
			addTypeMembers(compilationUnit.getBody(), result);
			return result;
		}
		
		public static Iterable<TypeInfo> getTypeMemberDependencies(TypeMemberNode tm) {
			var result = new HashSet<TypeInfo>();
			addTypeMemberDependencies(tm, result);
			return result;
		}
		
		private static void addTypeMembers(PackageBodyNode packageBody, List<TypeMemberNode> types) {
			if (packageBody != null) {
				foreach (var m in packageBody.getMembers()) {
					switch (m.PackageMemberKind) {
					case Package:
						var p = (PackageDeclarationNode)m;
						if (p.Body != null) {
							addTypeMembers(p.Body, types);
						}
						break;
	
					case Class:
						var c = (ClassDeclarationNode)m;
						addTypeMembers(c, types);
						break;
	
					case Interface:
					case Delegate:
						types.add((TypeMemberNode)m);
						break;
					}
				}
			}
		}
		
		private static void addTypeMembers(ClassDeclarationNode classDeclaration, List<TypeMemberNode> types) {
			types.add(classDeclaration);
			foreach (var m in classDeclaration.Members) {
				switch (m.TypeMemberKind) {
				case Class:
					var c = (ClassDeclarationNode)m;
					addTypeMembers(c, types);
					break;
					
				case Interface:
				case Delegate:
					types.add((TypeMemberNode)m);
					break;
				}
			}
		}

		private static void addDependencies(TypeInfo type, Set<TypeInfo> dependencies) {
			if (type == null || type.IsPrimitive || type.IsGenericParameter) {
				return;
			}
			while (type.IsArray) {
				type = type.ElementType;
			}
			foreach (var a in type.GenericArguments) {
				addDependencies(a, dependencies);
			}
			dependencies.add(type);
		}
		
		private static void addTypeMemberDependencies(TypeMemberNode tm, Set<TypeInfo> dependencies) {
			var t = tm.getUserData(typeof(TypeInfo));
			if (t == null) {
				return;
			}
			addAnnotationsDependencies(t.Annotations, dependencies);
			foreach (var ga in t.GenericArguments) {
				foreach (var b in ga.GenericParameterBounds) {
					addDependencies(b, dependencies);
				}
			}
			
			ExpressionDependenciesHandler expressionHandler = null;
			switch (tm.TypeMemberKind) {
			case Class:
				var c = (ClassDeclarationNode)tm;
				addDependencies(t.BaseType, dependencies);
				foreach (var i in t.Interfaces) {
					addDependencies(i, dependencies);
				}
				foreach (var cm in c.Members) {
					switch (cm.TypeMemberKind) {
					case Class:
					case Interface:
					case Delegate:
						addTypeMemberDependencies((TypeMemberNode)cm, dependencies);
						break;
					case EnumConstant:
						var enumConstant = (EnumConstantDeclarationNode)cm;
						addTypeMemberDependencies(enumConstant, dependencies);
						foreach (var e in enumConstant.Arguments) {
							if (expressionHandler == null) {
								expressionHandler = new ExpressionDependenciesHandler(null);
							}
							expressionHandler.handleExpression(e, dependencies, true);
						}
						break;
					case Field:
						foreach (var decl in ((FieldDeclarationNode)cm).Declarators) {
							var f = decl.getUserData(typeof(FieldInfo));
							addDependencies(f.Type, dependencies);
							addAnnotationsDependencies(f.Annotations, dependencies);
							if (decl.Value != null) {
								if (expressionHandler == null) {
									expressionHandler = new ExpressionDependenciesHandler(null);
								}
								expressionHandler.handleExpression(decl.Value, dependencies, true);
							}
						}
						break;
					case Constructor:
						var constructor = (ConstructorDeclarationNode)cm;
						addBodyDependencies(constructor.getUserData(typeof(MethodInfo)), constructor.Body, dependencies);
						if (constructor.Initializer != null) {
							foreach (var e in constructor.Initializer.Arguments) {
								if (expressionHandler == null) {
									expressionHandler = new ExpressionDependenciesHandler(null);
								}
								expressionHandler.handleExpression(e, dependencies, true);
							}
						}
						break;
					case Destructor:
						var destructor = (DestructorDeclarationNode)cm;
						addBodyDependencies(destructor.getUserData(typeof(MethodInfo)), destructor.Body, dependencies);
						break;
					case Method:
						var method = (MethodDeclarationNode)cm;
						addBodyDependencies(method.getUserData(typeof(MethodInfo)), method.Body, dependencies);
						break;
					case Property:
					case Indexer:
						var accessorsOwner = (IAccessorsOwner)cm;
						var get = accessorsOwner.getGetAccessor();
						var set = accessorsOwner.getSetAccessor();
						if (get != null) {
							addBodyDependencies(get.getUserData(typeof(MethodInfo)), get.Body, dependencies);
						}
						if (set != null) {
							addBodyDependencies(set.getUserData(typeof(MethodInfo)), set.Body, dependencies);
						}
						break;
					}
				}
				break;
			case Interface:
			case Delegate:
				foreach (var i in t.getInterfaces()) {
					addDependencies(i, dependencies);
					addAnnotationsDependencies(i.Annotations, dependencies);
				}
				foreach (var meth in t.Methods) {
					addDependencies(meth.ReturnType, dependencies);
					addAnnotationsDependencies(meth.Annotations, dependencies);
					foreach (var p in meth.Parameters) {
						addDependencies(p.Type, dependencies);
						addAnnotationsDependencies(p.Annotations, dependencies);
					}
				}
				break;
			}
		}

		private static void addBodyDependencies(MethodInfo meth, BlockStatementNode body, Set<TypeInfo> dependencies) {
			addAnnotationsDependencies(meth.Annotations, dependencies);
			addDependencies(meth.ReturnType, dependencies);
			foreach (var p in meth.Parameters) {
				addDependencies(p.Type, dependencies);
				addAnnotationsDependencies(p.Annotations, dependencies);
			}
			foreach (var tp in meth.GenericArguments) {
				foreach (var b in tp.GenericParameterBounds) {
					addDependencies(b, dependencies);
				}
			}
			if (body != null) {
				new StatementDependenciesHandler(null).handleStatement(body, dependencies);
			}
		}
		
		private static void addAnnotationsDependencies(Iterable<AnnotationValue> values, Set<TypeInfo> dependencies) {
			foreach (var av in values) {
				foreach (var s in av.getArgumentNames()) {
					addAnnotationDependencies(av.getArgument(s), dependencies);
				}
			}
		}

		private static void addAnnotationDependencies(AnnotationArgument arg, Set<TypeInfo> dependencies) {
			switch (arg.AnnotationArgumentKind) {
			case Annotation:
				foreach (var s in arg.getArgumentNames()) {
					addAnnotationDependencies(arg.getArgument(s), dependencies);
				}
				break;
			case Array:
				foreach (var e in arg.getElements()) {
					addAnnotationDependencies(e, dependencies);
				}
				break;
			case Enum:
			case Type:
				addDependencies(arg.Type, dependencies);
				break;
			}
		}

		class StatementDependenciesHandler : StatementHandler<Set<TypeInfo>, Void> {
			private ExpressionDependenciesHandler expressionHandler;
		
			StatementDependenciesHandler(ExpressionDependenciesHandler expressionHandler)
					: super(false) {
				this.expressionHandler = expressionHandler ?? new ExpressionDependenciesHandler(this);
			}
			
			protected override Void handleBlock(BlockStatementNode block, Set<TypeInfo> dependencies) {
				foreach (StatementNode s in block.Statements) {
					handleStatement(s, dependencies);
				}
				return null;
			}
			
			protected override Void handleDo(DoStatementNode doStatement, Set<TypeInfo> dependencies) {
				expressionHandler.handleExpression(doStatement.Condition, dependencies, true);
				handleStatement(doStatement.Statement, dependencies);
				return null;
			}
			
			protected override Void handleExpression(ExpressionStatementNode expression, Set<TypeInfo> dependencies) {
				expressionHandler.handleExpression(expression.Expression, dependencies, false);
				return null;
			}
			
			protected override Void handleFor(ForStatementNode forStatement, Set<TypeInfo> dependencies) {
				foreach (var s in forStatement.Initializer) {
					handleStatement(s, dependencies);
				}
				if (forStatement.Condition != null) {
					expressionHandler.handleExpression(forStatement.Condition, dependencies, true);
				}
				foreach (var s in forStatement.Iterator) {
					handleStatement(s, dependencies);
				}
				handleStatement(forStatement.Statement, dependencies);
				return null;
			}
			
			protected override Void handleForeach(ForeachStatementNode foreachStatement, Set<TypeInfo> dependencies) {
				if (foreachStatement.Type != null) {
					var info = foreachStatement.getUserData(typeof(MemberInfo));
					addDependencies(info.Type, dependencies);
				}
				expressionHandler.handleExpression(foreachStatement.Source, dependencies, true);
				handleStatement(foreachStatement.Statement, dependencies);
				return null;
			}
			
			protected override Void handleIf(IfStatementNode ifStatement, Set<TypeInfo> dependencies) {
				expressionHandler.handleExpression(ifStatement.Condition, dependencies, true);
				handleStatement(ifStatement.IfTrue, dependencies);
				if (ifStatement.IfFalse != null) {
					handleStatement(ifStatement.IfFalse, dependencies);
				}
				return null;
			}
			
			protected override Void handleLabeled(LabeledStatementNode labeled, Set<TypeInfo> dependencies) {
				handleStatement(labeled.Statement, dependencies);
				return null;
			}
			
			protected override Void handleLocalDeclaration(LocalDeclarationStatementNode localDeclaration, Set<TypeInfo> dependencies) {
				var info = localDeclaration.getUserData(typeof(ExpressionInfo));
				addDependencies(info.Type, dependencies);
				foreach (var decl in localDeclaration.Declarators) {
					if (decl.Value != null) {
						expressionHandler.handleExpression(decl.Value, dependencies, true);
					}
				}
				return null;
			}
			
			protected override Void handleReturn(ReturnStatementNode returnStatement, Set<TypeInfo> dependencies) {
				if (returnStatement.Value != null) {
					expressionHandler.handleExpression(returnStatement.Value, dependencies, true);
				}
				return null;
			}
			
			protected override Void handleSwitch(SwitchStatementNode switchStatement, Set<TypeInfo> dependencies) {
				expressionHandler.handleExpression(switchStatement.Selector, dependencies, true);
				var sinfo = switchStatement.Selector.getUserData(typeof(ExpressionInfo));
				addDependencies(sinfo.Type, dependencies);
				foreach (var section in switchStatement.Sections) {
					if (section.CaseExpression != null) {
						if (!sinfo.Type.IsEnum) {
							expressionHandler.handleExpression(section.CaseExpression, dependencies, true);
						}
					}
					foreach (var s in section.Statements) {
						handleStatement(s, dependencies);
					}
				}
				return null;
			}
			
			protected override Void handleSynchronized(SynchronizedStatementNode synchronizedStatement, Set<TypeInfo> dependencies) {
				expressionHandler.handleExpression(synchronizedStatement.Lock, dependencies, true);
				handleStatement(synchronizedStatement.Statement, dependencies);
				return null;
			}
	
			protected override Void handleThrow(ThrowStatementNode throwStatement, Set<TypeInfo> dependencies) {
				if (throwStatement.Exception != null) {
					expressionHandler.handleExpression(throwStatement.Exception, dependencies, true);
				}
				return null;
			}
	
			protected override Void handleTry(TryStatementNode tryStatement, Set<TypeInfo> dependencies) {
				handleBlock(tryStatement.Block, dependencies);
				foreach (var cc in tryStatement.CatchClauses) {
					if (cc.ExceptionType != null) {
						addDependencies(cc.getUserData(typeof(TypeInfo)), dependencies);
					}
					handleBlock(cc.Block, dependencies);
				}
				if (tryStatement.Finally != null) {
					handleBlock(tryStatement.Finally, dependencies);
				}
				return null;
			}
	
			protected override Void handleUsing(UsingStatementNode usingStatement, Set<TypeInfo> dependencies) {
				handleStatement(usingStatement.ResourceAcquisition, dependencies);
				handleStatement(usingStatement.Statement, dependencies);
				return null;
			}
			
			protected override Void handleWhile(WhileStatementNode whileStatement, Set<TypeInfo> dependencies) {
				expressionHandler.handleExpression(whileStatement.Condition, dependencies, true);
				handleStatement(whileStatement.Statement, dependencies);
				return null;
			}
			
			protected override Void handleYield(YieldStatementNode yield, Set<TypeInfo> dependencies) {
				if (yield.Value != null) {
					expressionHandler.handleExpression(yield.Value, dependencies, true);
				}
				return null;
			}
		}
		
		class ExpressionDependenciesHandler : ExpressionHandler<Set<TypeInfo>, Void> {
			private StatementDependenciesHandler statementHandler;
			
			ExpressionDependenciesHandler(StatementDependenciesHandler statementHandler)
					: super(false) {
				this.statementHandler = statementHandler ?? new StatementDependenciesHandler(this);
			}
			
			protected override Void handleAnonymousObjectCreation(AnonymousObjectCreationExpressionNode anonymousObject, Set<TypeInfo> dependencies, bool nested) {
				foreach (MemberInitializerNode mi in anonymousObject.getMemberDeclarators()) {
					handleExpression(mi.getValue(), dependencies, nested);
				}
				return null;
			}
	
			protected override Void handleArrayCreation(ArrayCreationExpressionNode arrayCreation, Set<TypeInfo> dependencies, bool nested) {
				var info = arrayCreation.getUserData(typeof(ExpressionInfo));
				addDependencies(info.Type, dependencies);
				foreach (var e in arrayCreation.DimensionExpressions) {
					handleExpression(e, dependencies, true);
				}
				if (arrayCreation.Initializer != null) {
					handleExpression(arrayCreation.Initializer, dependencies, true);
				}
				return null;
			}
			
			protected override Void handleArrayInitializer(ArrayInitializerExpressionNode arrayInitializer, Set<TypeInfo> dependencies, bool nested) {
				foreach (var e in arrayInitializer.Values) {
					handleExpression(e, dependencies, true);
				}
				return null;
			}
			
			protected override Void handleAssign(AssignExpressionNode assign, Set<TypeInfo> dependencies, bool nested) {
				handleExpression(assign.Left, dependencies, true);
				handleExpression(assign.Right, dependencies, true);
				return null;
			}
			
			protected override Void handleBinary(BinaryExpressionNode binary, Set<TypeInfo> dependencies, bool nested) {
				handleExpression(binary.LeftOperand, dependencies, true);
				handleExpression(binary.RightOperand, dependencies, true);
				return null;
			}
			
			protected override Void handleCast(CastExpressionNode cast, Set<TypeInfo> dependencies, bool nested) {
				var info = cast.getUserData(typeof(ExpressionInfo));
				addDependencies(info.Type, dependencies);
				handleExpression(cast.Expression, dependencies, true);
				return null;
			}
	
			protected override Void handleConditional(ConditionalExpressionNode conditional, Set<TypeInfo> dependencies, bool nested) {
				handleExpression(conditional.Condition, dependencies, true);
				handleExpression(conditional.IfTrue, dependencies, true);
				handleExpression(conditional.IfFalse, dependencies, true);
				return null;
			}
			
			protected override Void handleElementAccess(ElementAccessExpressionNode elementAccess, Set<TypeInfo> dependencies, bool nested) {
				handleExpression(elementAccess.TargetObject, dependencies, true);
				foreach (var e in elementAccess.Indexes) {
					handleExpression(e, dependencies, true);
				}
				return null;
			}
			
			protected override Void handleInvocation(InvocationExpressionNode invocation, Set<TypeInfo> dependencies, bool nested) {
				handleExpression(invocation.TargetObject, dependencies, nested);
				foreach (var e in invocation.Arguments) {
					handleExpression(e, dependencies, true);
				}
				return null;
			}
			
			protected override Void handleLambda(LambdaExpressionNode lambda, Set<TypeInfo> dependencies, bool nested) {
				var m = lambda.getUserData(typeof(MethodInfo));
				foreach (var p in m.Parameters) {
					addDependencies(p.Type, dependencies);
				}
				statementHandler.handleStatement(lambda.Body, dependencies);
				return null;
			}
	
			protected override Void handleMemberAccess(MemberAccessExpressionNode memberAccess, Set<TypeInfo> dependencies, bool nested) {
				var info = memberAccess.getUserData(typeof(ExpressionInfo));
				var member = info.getMember();
				handleExpression(memberAccess.TargetObject, dependencies, nested);
				if (member == null) {
					var type = info.Type;
		            if (type != null) {
						addDependencies(type, dependencies);
		            }
				}
				return null;
			}
			
			protected override Void handleObjectCreation(ObjectCreationExpressionNode objectCreation, Set<TypeInfo> dependencies, bool nested) {
				var info = objectCreation.getUserData(typeof(ExpressionInfo));
				addDependencies(info.Type, dependencies);
				foreach (var e in objectCreation.Arguments) {
					handleExpression(e, dependencies, true);
				}
				if (objectCreation.Initializer != null) {
					handleExpression(objectCreation.Initializer, dependencies, true);
				}
				return null;
			}
	
			protected override Void handleCollectionInitializer(CollectionInitializerExpressionNode initializer, Set<TypeInfo> dependencies, bool nested) {
				foreach (var e in initializer.Values.selectMany(p => p)) {
					handleExpression(e, dependencies, true);
				}
				return null;
			}
	
			protected override Void handleObjectInitializer(ObjectInitializerExpressionNode initializer, Set<TypeInfo> dependencies, bool nested) {
				foreach (var init in initializer.MemberInitializers) {
					handleExpression(init.Value, dependencies, true);
				}
				return null;
			}
	
			protected override Void handleSimpleName(SimpleNameExpressionNode simpleName, Set<TypeInfo> dependencies, bool nested) {
				var info = simpleName.getUserData(typeof(ExpressionInfo));
				var member = info.Member;
				if (member != null && member.MemberKind == MemberKind.Type) {
					addDependencies(member.Type, dependencies);
				}
				return null;
			}
			
			protected override Void handleSizeof(SizeofExpressionNode sizeofExpression, Set<TypeInfo> dependencies, bool nested) {
				handleExpression(sizeofExpression.Expression, dependencies, nested);
				return null;
			}
	
			protected override Void handleType(TypeExpressionNode type, Set<TypeInfo> dependencies, bool nested) {
				var info = type.getUserData(typeof(ExpressionInfo));
				addDependencies(info.Type, dependencies);
				return null;
			}
			
			protected override Void handleTypeof(TypeofExpressionNode typeofExpression, Set<TypeInfo> dependencies, bool nested) {
				addDependencies(typeofExpression.getUserData(typeof(TypeInfo)), dependencies);
				return null;
			}
			
			protected override Void handleUnary(UnaryExpressionNode unary, Set<TypeInfo> dependencies, bool nested) {
				handleExpression(unary.Operand, dependencies, true);
				return null;
			}
		}
	}
}
