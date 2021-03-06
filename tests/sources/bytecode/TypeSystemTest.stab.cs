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
using java.io;
using java.nio.charset;
using java.util;
using org.junit;
using stab.query;
using cnatural.helpers;

namespace stab.reflection.test {

	public class TypeSystemTest {
		private String bin = System.getProperty("user.dir") + "/tests/tests.jar";
		
		[Test]
		public void testObjectMethods() {
			var typeSystem = new Library(new String[] {});
			var typeInfo = typeSystem.getType("java/lang/Object");
			var names = Query.asIterable(new String[] { 
				"<init>", "equals", "getClass", "hashCode", "notify", "notifyAll", "toString", "wait", "wait", "wait" });
			Assert.assertTrue(typeInfo.Methods.where(p => p.IsPublic).select(p => p.Name).orderBy(p => p).sequenceEqual(names));
		}
		
		[Test]
		public void testBaseTypes() {
			var typeSystem = new Library(new String[] {});
			var stringType = typeSystem.getType("java/lang/String");
			var types = Query.asIterable(new TypeInfo[] {
				typeSystem.getType("java/lang/Object"),
				typeSystem.getType("java/io/Serializable"),
				typeSystem.getType("java/lang/CharSequence"),
				typeSystem.getGenericType(typeSystem.getType("java/lang/Comparable"), Collections.singletonList(stringType))
			});
			var publicBaseTypes = stringType.getBaseTypes().where(p => p.IsPublic);
			Assert.assertTrue(publicBaseTypes.union(types).sequenceEqual(publicBaseTypes));
			Assert.assertFalse(publicBaseTypes.except(types).any());
		}
		
		[Test]
		public void testSimpleClass() {
			doTest("SimpleClass");
		}
		
		[Test]
		public void testGenericClass() {
			doTest("GenericClass");
		}
		
		[Test]
		public void testConstructedGenericClass() {
			var typeSystem = new Library(new String[] { bin });
			var typeInfo = typeSystem.getType("stab/bytecode/test/classes/GenericClass");
			var args = new ArrayList<TypeInfo>();
			args.add(typeSystem.getType("java/lang/String"));
			doTest("ConstructedGenericClass", typeSystem.getGenericType(typeInfo, args));
		}

		[Test]
		public void testSimpleMethod() {
			doTest("SimpleMethod");
		}

		[Test]
		public void testGenericMethod() {
			doTest("GenericMethod");
		}

		[Test]
		public void testNestedClass() {
			doTest("NestedClass");
		}

		[Test]
		public void testSimpleField() {
			doTest("SimpleField");
		}

		[Test]
		public void testStaticInitializer() {
			doTest("StaticInitializer");
		}

		[Test]
		public void testAnnotation() {
			doTest("Annotation");
		}
		
		private void doTest(String test) {
			var typeSystem = new Library(new String[] { bin });
			var typeInfo = typeSystem.getType("stab/bytecode/test/classes/" + test);
			doTest(test, typeInfo);
		}
		
		private void doTest(String test, TypeInfo typeInfo) {
			String result = typeToString(typeInfo, "");
			
			var userDir = System.getProperty("user.dir");
			var generatedPath = PathHelper.combine(userDir, "Tests/resources/TypeSystemTest/generated");
			var generatedDir = new File(generatedPath);
			if (!generatedDir.exists()) {
				generatedDir.mkdir();
			}
			var fileWriter = new FileWriter(PathHelper.combine(generatedPath, test + ".txt"));
			fileWriter.write(result);
			fileWriter.close();
			
			var expectedPath = PathHelper.combine(userDir, "Tests/resources/TypeSystemTest/expected");
			var expectedFile = new File(PathHelper.combine(expectedPath, test + ".txt"));
			Assert.assertTrue("Expected file not found: " + expectedFile, expectedFile.exists());
			var fileReader = new InputStreamReader(new FileInputStream((expectedFile)), Charset.forName("UTF-8"));
			var reference = readToEnd(fileReader);
			
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
		
		private String typeToString(TypeInfo typeInfo, String indent) {
			var sb = new StringBuilder();
			sb.append(indent).append("// ").append(typeInfo.Descriptor).append(" / ").append(typeInfo.Signature).append("\n");
			annotations(typeInfo.Annotations, indent, sb);
			sb.append(indent);
			if (typeInfo.IsPublic) {
				sb.append("public ");
			}
			if (typeInfo.IsInterface) {
				sb.append("interface ");
			} else {
				sb.append("class ");
			}
			typeName(typeInfo, sb);
			sb.append(" ");

			if (typeInfo.BaseType != null) {
				sb.append(": ");
				typeName(typeInfo.BaseType, sb);
				sb.append(" ");
			}
			var comma = typeInfo.BaseType != null;
			if (typeInfo.Interfaces.any()) {
				if (comma) {
					sb.append(", ");
				} else {
					comma = true;
				}
				var first = true;
				foreach (var t in typeInfo.Interfaces) {
					if (first) {
						first = false;
					} else {
						sb.append(", ");
					}
					typeName(t, sb);
				}
				sb.append(" ");
			}

			foreach (var t in typeInfo.GenericArguments) {
				if (t.IsGenericParameter) {
					if (t.GenericParameterBounds.any()) {
						sb.append("\n").append(indent).append("        ").append("where ");
						typeName(t, sb);
						sb.append(": ");
						bool first = true;
						foreach (var bound in t.GenericParameterBounds) {
							if (first) {
								first = false;
							} else {
								sb.append(", ");
							}
							typeName(bound, sb);
						}
						sb.append(" ");
					}
				}
			}
			sb.append("{\n");

			foreach (var t in typeInfo.NestedTypes) {
				sb.append(typeToString(t, indent + "    "));
			}

			foreach (var f in typeInfo.Fields) {
				sb.append(indent).append("    // ").append(f.Descriptor).append(" / ").append(f.Signature).append("\n");
				
				sb.append(indent).append("    ");
				if (f.IsPublic) {
					sb.append("public ");
				}
				if (f.IsProtected) {
					sb.append("protected ");
				}
				if (f.IsPrivate) {
					sb.append("private ");
				}
				if (f.IsFinal) {
					sb.append("final ");
				}
				if (f.IsStatic) {
					sb.append("static ");
				}
				if (f.IsEnum) {
					sb.append("enum ");
				}
				if (f.IsSynthetic) {
					sb.append("synthetic ");
				}
				if (f.IsTransient) {
					sb.append("transient ");
				}
				if (f.IsVolatile) {
					sb.append("volatile ");
				}
				typeName(f.Type, sb);
				sb.append(" ").append(f.Name).append(";\n");
			}

			foreach (var meth in typeInfo.Methods) {
				sb.append(indent).append("    // ").append(meth.Descriptor).append(" / ").append(meth.Signature).append("\n");
				
				sb.append(indent).append("    ");
				if (meth.IsPublic) {
					sb.append("public ");
				}
				if (meth.IsFinal) {
					sb.append("final ");
				}
				if (meth.IsStatic) {
					sb.append("static ");
				}
				typeName(meth.ReturnType, sb);
				sb.append(" ").append(meth.Name);
				if (meth.GenericArguments.any()) {
					sb.append("<");
					var first = true;
					foreach (var t in meth.GenericArguments) {
						if (first) {
							first = false;
						} else {
							sb.append(", ");
						}
						typeName(t, sb);
					}
					sb.append(">");
				}
				sb.append("(");
				var first = true;
				foreach (var p in meth.Parameters) {
					if (first) {
						first = false;
					} else {
						sb.append(", ");
					}
					typeName(p.Type, sb);
					sb.append(" ");
					sb.append(p.Name);
				}
				sb.append(")").append(" {\n").append(indent).append("    ").append("}\n");
			}

			sb.append(indent).append("}\n");
			return sb.toString();
		}

		private void annotations(Iterable<AnnotationValue> annotations, String indent, StringBuilder sb) {
			foreach (var a in annotations) {
				sb.append(indent).append("[");
				annotation(a, indent, sb);
				sb.append("]\n");
			}
		}

		private void annotation(AnnotationValue a, String indent, StringBuilder sb) {
			sb.append(a.Type.FullName);
			if (a.ArgumentNames.any()) {
				sb.append("(");
				var first = true;
				foreach (var argName in a.ArgumentNames) {
					var arg = a.getArgument(argName);
					if (first) {
						first = false;
					} else {
						sb.append(", ");
					}
					sb.append(arg.Name).append(" = ");
					switch (arg.AnnotationArgumentKind) {
					case Annotation:
						annotation((AnnotationValue)arg, indent, sb);
						break;
						
					case Boolean:
						sb.append(arg.Value);
						break;
						
					default:
						throw new RuntimeException("Unhandled annotation argument kind: " + arg.getAnnotationArgumentKind());
					}
				}
				sb.append(")");
			}
		}
	
		private void typeName(TypeInfo typeInfo, StringBuilder sb) {
			sb.append(typeInfo.FullName);
			if (typeInfo.GenericArguments.any()) {
				sb.append("<");
				var first = true;
				foreach (var t in typeInfo.GenericArguments) {
					if (first) {
						first = false;
					} else {
						sb.append(", ");
					}
					typeName(t, sb);
				}
				sb.append(">");
			}
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