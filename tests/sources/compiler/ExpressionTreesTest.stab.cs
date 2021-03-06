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
using java.io;
using java.lang;
using java.util;
using java.nio.charset;
using org.junit;
using stab.query;
using cnatural.helpers;
using stab.tree;

namespace cnatural.compiler.test {

	public class ExpressionTreesTest {
		[Test]
		public void value() {
			ExpressionTree<FunctionInt> tree = () => 1;
			doTest("value", tree);
		}

		[Test]
		public void variable() {
			ExpressionTree<FunctionIntInt> tree = p => p;
			doTest("variable", tree);
		}

		[Test]
		public void unary() {
			ExpressionTree<FunctionIntInt> tree = p => -p;
			doTest("unary", tree);
		}

		[Test]
		public void cast() {
			ExpressionTree<FunctionDoubleInt> tree = p => (int)p;
			doTest("cast", tree);
		}

		[Test]
		public void binary() {
			ExpressionTree<FunctionIntInt> tree = p => p + 1;
			doTest("binary", tree);
		}

		int testField;
		
		[Test]
		public void field() {
			ExpressionTree<FunctionInt> tree = () => testField;
			doTest("field", tree);
		}
		
		int testMethod(int i) {
			return i;
		}

		[Test]
		public void method() {
			ExpressionTree<FunctionIntInt> tree = p => testMethod(p);
			doTest("method", tree);
		}

		[Test]
		public void returnStatement() {
			ExpressionTree<FunctionInt> tree = () => { return 1; };
			doTest("returnStatement", tree);
		}

		[Test]
		public void local() {
			ExpressionTree<FunctionIntInt> tree = p => { int i = p; return i; };
			doTest("local", tree);
		}

		[Test]
		public void ifStatement() {
			ExpressionTree<FunctionIntInt> tree = p => {
				if (p >= 0) {
					return p;
				} else
					return -p;
			};
			doTest("ifStatement", tree);
		}

		[Test]
		public void whileStatement() {
			ExpressionTree<FunctionIntInt> tree = p => {
				while (p > 0) {
					--p;
				}
				return 0;
			};
			doTest("whileStatement", tree);
		}

		[Test]
		public void label() {
			ExpressionTree<FunctionInt> tree = () => {
				Label:
				goto Label;
			};
			doTest("label", tree);
		}

		[Test]
		public void throwStatement() {
			ExpressionTree<FunctionInt> tree = () => {
				throw new IllegalStateException();
			};
			doTest("throwStatement", tree);
		}

		[Test]
		public void throwStatement2() {
			ExpressionTree<FunctionIntInt> tree = p => {
				throw new IllegalStateException("Message " + p);
			};
			doTest("throwStatement2", tree);
		}
		
		[Test]
		public void tryStatement() {
			ExpressionTree<FunctionInt> tree = () => {
				try {
					return 1;
				} catch (RuntimeException e) {
					throw e;
				}
			};
			doTest("tryStatement", tree);
		}
		
		[Test]
		public void tryStatement2() {
			ExpressionTree<FunctionInt> tree = () => {
				try {
					return 1;
				} catch (RuntimeException e) {
					throw e;
				} catch (Exception e) {
					throw e;
				} finally {
					return 0;
				}
			};
			doTest("tryStatement2", tree);
		}

		[Test]
		public void whileStatement2() {
			ExpressionTree<FunctionIntInt> tree = p => {
				while (p > 0) {
					if (p == 3) {
						break;
					}
					p -= 2;
				}
				return p * p;
			};
			doTest("whileStatement2", tree);
		}

		[Test]
		public void switchStatement() {
			ExpressionTree<FunctionIntInt> tree = p => {
				switch (p) {
				case 1:
				case 2:
					return p;
				case 3:
					break;
				default:
					return p / 2;
				}
				return p % 2;
			};
			doTest("switchStatement", tree);
		}

		[Test]
		public void switchStatement2() {
			ExpressionTree<FunctionIntInt> tree = p => {
				switch (p) {
				case 1:
				case 2:
					return p;
				case 3:
					break;
				default:
					goto case 2;
				}
				return p % 2;
			};
			doTest("switchStatement2", tree);
		}

		[Test]
		public void synchronizedStatement() {
			var obj = "STR";
			ExpressionTree<FunctionInt> tree = () => {
				synchronized (obj) {
					return obj.hashCode();
				}
			};
			doTest("synchronizedStatement", tree);
		}
		
		private void doTest(String test, LambdaExpression lambda) {
			variables = new ArrayList<VariableExpression>();
			labels = new ArrayList<Label>();
			switchLabels = new ArrayList<SwitchLabel>();
			
			var result = lambdaToString(lambda);
			
			var userDir = System.getProperty("user.dir");
			var generatedPath = PathHelper.combine(userDir, "tests/resources/ExpressionTreesTest/generated");
			var generatedDir = new File(generatedPath);
			if (!generatedDir.exists()) {
				generatedDir.mkdir();
			}
			var fileWriter = new FileWriter(PathHelper.combine(generatedPath, test + ".txt"));
			fileWriter.write(result);
			fileWriter.close();
		
			var referencePath = PathHelper.combine(PathHelper.combine(userDir, "tests/resources/ExpressionTreesTest/references"), test + ".txt");
			var referenceFile = new File(referencePath);
			String reference = null;
			if (referenceFile.exists()) {
				var fileReader = new InputStreamReader(new FileInputStream((referencePath)), Charset.forName("UTF-8"));
				reference = readToEnd(fileReader);
			} else {
				Assert.fail("No reference for '" + test + "'");
			}
			
			if (reference != null) {
				var genReader = new BufferedReader(new StringReader(result));
				var refReader = new BufferedReader(new StringReader(reference));
				for (;;) {
					var genLine = genReader.readLine();
					var refLine = refReader.readLine();
					if (genLine == null && refLine == null) {
						break;
					}
					Assert.assertEquals(refLine, genLine);
				}
			}
		}
		
		private String lambdaToString(LambdaExpression lambda) {
			var sb = new StringBuilder();
			sb.append("stab.tree.ExpressionTree<");
			sb.append(lambda.Type);
			sb.append("> tree =\n\t(");
			var first = true;
			foreach (var p in lambda.Parameters) {
				if (first) {
					first = false;
				} else {
					sb.append(", ");
				}
				variableToString(p, sb);
			}
			sb.append(") =>\n");
			statementToString(lambda.Body, sb, "    ");
			return sb.toString();
		}
		
		private List<SwitchLabel> switchLabels;
		
		private void statementToString(Statement statement, StringBuilder sb, String indent) {
			switch (statement.StatementKind) {
			case Block:
				sb.append(indent).append("{\n");
				foreach (var s in ((BlockStatement)statement).Statements) {
					statementToString(s, sb, indent + "    ");
				}
				sb.append(indent).append("}");
				break;

			case Break:
				sb.append(indent).append("break;");
				break;
				
			case Continue:
				sb.append(indent).append("continue;");
				break;
				
			case Do:
				var doStatement = (LoopStatement)statement;
				sb.append(indent).append("do\n");
				statementToString(doStatement.Body, sb, indent + "    ");
				sb.append(indent).append("while (");
				expressionToString(doStatement.Condition, sb, "");
				sb.append(");");
				break;

			case Empty:
				sb.append(";");
				break;
				
			case Goto:
				sb.append(indent).append("goto ");
				labelToString(((GotoStatement)statement).Label, sb);
				sb.append(";");
				break;

			case GotoCase: {
				var gotoCase = (GotoCaseStatement)statement;
				sb.append(indent).append("goto ");
				int n = switchLabels.indexOf(gotoCase.Label);
				if (n == -1) {
					n = switchLabels.size();
					switchLabels.add(gotoCase.Label);
				}
				if (gotoCase.Label.Default) {
					sb.append("default ").append("#").append(n).append("#");
				} else {
					sb.append("case ").append("#").append(n).append("#");
					if (gotoCase.Label.Name != null) {
						sb.append("\"").append(gotoCase.Label.Name).append("\"");
					} else {
						sb.append(gotoCase.Label.Value);
					}
				}
				
				sb.append(";");
				break;
			}
				
			case Expression:
				var expression = (ExpressionStatement)statement;
				expressionToString(expression.Expression, sb, indent);
				sb.append(";");
				break;
				
			case If:
				var ifStatement = (IfStatement)statement;
				sb.append(indent).append("if (");
				expressionToString(ifStatement.Condition, sb, "");
				sb.append(")\n");
				statementToString(ifStatement.IfTrue, sb, indent + "    ");
				if (ifStatement.IfFalse != null) {
					sb.append(indent).append("else\n");
					statementToString(ifStatement.IfFalse, sb, indent + "    ");
				}
				break;
				
			case Labeled:
				var labeled = (LabeledStatement)statement;
				sb.append(indent);
				labelToString(labeled.Label, sb);
				sb.append(":\n");
				statementToString(labeled.Statement, sb, indent);
				break;
				
			case Return:
				sb.append(indent).append("return ");
				expressionToString(((ReturnStatement)statement).Value, sb, "");
				sb.append(";");
				break;

			case Switch:
				var switchStatement = (SwitchStatement)statement;
				sb.append(indent).append("switch (");
				expressionToString(switchStatement.Expression, sb, "");
				sb.append(") {\n");
				foreach (var section in switchStatement.Sections) {
					foreach (var label in section.Labels) {
						int n = switchLabels.indexOf(label);
						if (n == -1) {
							n = switchLabels.size();
							switchLabels.add(label);
						}
						sb.append(indent);
						if (label.Default) {
							sb.append("#").append(n).append("#").append("default:\n");
						} else {
							sb.append("#").append(n).append("#").append("case ");
							if (label.Name != null) {
								sb.append("\"").append(label.Name).append("\"");
							} else {
								sb.append(label.Value);
							}
							sb.append(":\n");
						}
					}
					foreach (var s in section.Statements) {
						statementToString(s, sb, indent + "    ");
					}
				}
				sb.append(indent).append("}");
				break;
				
			case Synchronized:
				var synchronizedStatement = (SynchronizedStatement)statement;
				sb.append(indent).append("synchronized (");
				expressionToString(synchronizedStatement.Lock, sb, "");
				sb.append(")\n");
				statementToString(synchronizedStatement.Body, sb, indent + "    ");
				break;
				
			case Throw:
				sb.append(indent).append("throw ");
				expressionToString(((ThrowStatement)statement).Exception, sb, "");
				sb.append(";");
				break;
				
			case Try:
				var tryStatement = (TryStatement)statement;
				sb.append(indent).append("try\n");
				statementToString(tryStatement.Body, sb, indent);
				foreach (var c in tryStatement.CatchClauses) {
					sb.append(indent).append("catch (");
					variableToString(c.Variable, sb);
					sb.append(")\n");
					statementToString(c.Body, sb, indent);
				}
				if (tryStatement.Finally != null) {
					sb.append(indent).append("finally\n");
					statementToString(tryStatement.Finally, sb, indent);
				}
				break;
				
			case While:
				var whileStatement = (LoopStatement)statement;
				sb.append(indent).append("while (");
				expressionToString(whileStatement.Condition, sb, "");
				sb.append(")\n");
				statementToString(whileStatement.Body, sb, indent + "    ");
				break;
				
			default:
				throw new IllegalStateException("Unhandled statement " + statement.StatementKind);
			}
			sb.append("\n");
		}
		
		private void expressionToString(Expression expression, StringBuilder sb, String indent) {
			switch (expression.ExpressionKind) {
			case Binary:
				sb.append(indent);
				binaryToString((BinaryExpression)expression, sb);
				break;
				
			case Field:
				sb.append(indent);
				fieldToString((FieldExpression)expression, sb);
				break;
				
			case MethodCall:
				sb.append(indent);
				methodCallToString((MethodCallExpression)expression, sb);
				break;
				
			case Local:
			case Parameter:
				sb.append(indent);
				variableToString((VariableExpression)expression, sb);
				break;

			case NewObject:
				sb.append(indent);
				newObjectToString((NewObjectExpression)expression, sb);
				break;
				
			case Unary:
				sb.append(indent);
				unaryToString((UnaryExpression)expression, sb);
				break;
				
			case Value:
				sb.append(indent);
				valueToString((ValueExpression)expression, sb);
				break;
				
			default:
				throw new IllegalStateException("Unhandled expression " + expression.ExpressionKind);
			}
		}
		
		private void binaryToString(BinaryExpression binary, StringBuilder sb) {
			expressionToString(binary.Left, sb, "");
			switch (binary.Operator) {
			case Add:
				sb.append(" + ");
				break;
			case AddAssign:
				sb.append(" += ");
				break;
			case And:
				sb.append(" & ");
				break;
			case AndAssign:
				sb.append(" &= ");
				break;
			case Assign:
				sb.append(" = ");
				break;
			case Divide:
				sb.append(" / ");
				break;
			case DivideAssign:
				sb.append(" /= ");
				break;
			case Element:
				sb.append("[");
				break;
			case Equal:
				sb.append("==");
				break;
			case GreaterThan:
				sb.append(" > ");
				break;
			case GreaterThanOrEqual:
				sb.append(" >= ");
				break;
			case LeftShift:
				sb.append(" << ");
				break;
			case LeftShiftAssign:
				sb.append(" <<= ");
				break;
			case LessThan:
				sb.append(" < ");
				break;
			case LessThanOrEqual:
				sb.append(" <= ");
				break;
			case LogicalAnd:
				sb.append(" && ");
				break;
			case LogicalOr:
				sb.append(" || ");
				break;
			case Modulo:
				sb.append(" % ");
				break;
			case ModuloAssign:
				sb.append(" %= ");
				break;
			case Multiply:
				sb.append(" * ");
				break;
			case MultiplyAssign:
				sb.append(" *= ");
				break;
			case NotEqual:
				sb.append(" != ");
				break;
			case NullCoalescing:
				sb.append(" ?? ");
				break;
			case Or:
				sb.append(" | ");
				break;
			case OrAssign:
				sb.append(" |= ");
				break;
			case RightShift:
				sb.append(" >> ");
				break;
			case RightShiftAssign:
				sb.append(" >>= ");
				break;
			case Subtract:
				sb.append(" - ");
				break;
			case SubtractAssign:
				sb.append(" -= ");
				break;
			case UnsignedRightShift:
				sb.append(" >>> ");
				break;
			case UnsignedRightShiftAssign:
				sb.append(" >>>= ");
				break;
			case Xor:
				sb.append(" ^ ");
				break;
			case XorAssign:
				sb.append(" ^= ");
				break;
			}
			expressionToString(binary.Right, sb, "");
			switch (binary.Operator) {
			case Element:
				sb.append("]");
				break;
			}
		}

		private void fieldToString(FieldExpression field, StringBuilder sb) {
			expressionToString(field.Target, sb, "");
			sb.append(".").append(field.Field.getName());
		}

		private void methodCallToString(MethodCallExpression methodCall, StringBuilder sb) {
			expressionToString(methodCall.Target, sb, "");
			sb.append(".").append(methodCall.Method.getName()).append("(");
			var first = true;
			foreach (var a in methodCall.Arguments) {
				if (first) {
					first = false;
				} else {
					sb.append(", ");
				}
				expressionToString(a, sb, "");
			}
			sb.append(")");
		}
		
		private void newObjectToString(NewObjectExpression newObject, StringBuilder sb) {
			sb.append("new ").append(newObject.Constructor.getDeclaringClass()).append("(");
			var first = true;
			foreach (var a in newObject.Arguments) {
				if (first) {
					first = false;
				} else {
					sb.append(", ");
				}
				expressionToString(a, sb, "");
			}
			sb.append(")");
			if (newObject.Initializers.any()) {
				sb.append(" { ");
				first = true;
				foreach (var i in newObject.Initializers) {
					if (first) {
						first = false;
					} else {
						sb.append(", ");
					}
					sb.append(i.Member.getDeclaringClass()).append(".").append(i.Member.getName()).append("(");
					var firstArg = true;
					foreach (var a in i.Arguments) {
						if (firstArg) {
							firstArg = false;
						} else {
							sb.append(", ");
						}
						expressionToString(a, sb, "");
					}
					sb.append(")");
				}
				sb.append(" }");
			}
		}
		
		private void unaryToString(UnaryExpression unary, StringBuilder sb) {
			switch (unary.Operator) {
			case Cast:
				sb.append("(").append(unary.Type).append(")");
				break;
			case Complement:
				sb.append("~");
				break;
			case Minus:
				sb.append("-");
				break;
			case Not:
				sb.append("!");
				break;
			case Plus:
				sb.append("+");
				break;
			case PreDecrement:
				sb.append("--");
				break;
			case PreIncrement:
				sb.append("++");
				break;
			case Sizeof:
				sb.append("sizeof(");
				break;
			}
			expressionToString(unary.Operand, sb, "");
			switch (unary.Operator) {
			case As:
				sb.append(" as ").append(unary.Type);
				break;
			case PostDecrement:
				sb.append("--");
				break;
			case PostIncrement:
				sb.append("++");
				break;
			case Sizeof:
				sb.append(")");
				break;
			}
		}

		private void valueToString(ValueExpression value, StringBuilder sb) {
			sb.append("#Value#").append(value.Type).append("#").append((value.Value == this) ? "this" : value.Value).append("#");
		}
		
		private List<VariableExpression> variables;
	
		private void variableToString(VariableExpression variable, StringBuilder sb) {
			var n = variables.indexOf(variable);
			if (n == -1) {
				n = variables.size();
				variables.add(variable);
			}
			sb.append("#").append(variable.ExpressionKind).append(n).append("#").append(variable.Type).append("#").append(variable.Name).append("#");
		}
		
		private List<Label> labels;
	
		private void labelToString(Label label, StringBuilder sb) {
			var n = labels.indexOf(label);
			if (n == -1) {
				n = labels.size();
				labels.add(label);
			}
			sb.append("#Label").append(n).append("#").append(label.Name).append("#");
		}
		
		private String readToEnd(Reader reader) {
			var sb = new StringBuilder();
			var buff = new char[1024];
			int read;
			while ((read = reader.read(buff)) != -1) {
				sb.append(buff, 0, read);
			}
			return sb.toString();
		}
	}
}
