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
using cnatural.helpers;
using cnatural.parser;
using cnatural.parser.test;

namespace cnatural.syntaxtree.test {
	public class QueryTranslatorTest {
		private QueryTranslator translator;
	
		[Test]
		public void testSelect() {
			doTest("Select");
		}
	
		[Test]
		public void testSelectInto() {
			doTest("SelectInto");
		}
	
		[Test]
		public void testTypedSelect() {
			doTest("TypedSelect");
		}
	
		[Test]
		public void testFromSelect() {
			doTest("FromSelect");
		}
	
		[Test]
		public void testFromFromSelect() {
			doTest("FromFromSelect");
		}
	
		[Test]
		public void testLetSelect() {
			doTest("LetSelect");
		}
	
		[Test]
		public void whereSelect() {
			doTest("WhereSelect");
		}
	
		[Test]
		public void joinSelect() {
			doTest("JoinSelect");
		}
	
		[Test]
		public void groupJoinSelect() {
			doTest("GroupJoinSelect");
		}
	
		[Test]
		public void joinWhereSelect() {
			doTest("JoinWhereSelect");
		}
	
		[Test]
		public void groupJoinWhereSelect() {
			doTest("GroupJoinWhereSelect");
		}
	
		[Test]
		public void orderbySelect() {
			doTest("OrderBySelect");
		}
	
		[Test]
		public void orderbySelect2() {
			doTest("OrderBySelect2");
		}
	
		[Test]
		public void groupBy() {
			doTest("GroupBy");
		}
	
		[Test]
		public void groupBy2() {
			doTest("GroupBy2");
		}
	
		[Test]
		public void fromOrderBySelect() {
			doTest("FromOrderBySelect");
		}
	
		[Test]
		public void joinJoinJoinSelect() {
			doTest("JoinJoinJoinSelect");
		}
	
		[Test]
		public void testFromFromWhereSelect() {
			doTest("FromFromWhereSelect");
		}
		
		private void doTest(String test) {
			var userDir = System.getProperty("user.dir");
			var sourcePath = PathHelper.combine(PathHelper.combine(userDir, "tests/resources/QueryTranslatorTest/sources"), test + ".stab.cs");
			var fileReader = new InputStreamReader(new FileInputStream((sourcePath)), Charset.forName("UTF-8"));
			var code = readToEnd(fileReader);
			var text = code.toCharArray();
			translator = new QueryTranslator(text);
			var parser = new Parser();
			var scanner = new SourceCodeScanner(new CodeErrorManager(), text);
			var compilationUnit = parser.parseCompilationUnit(scanner);
			var result = translate(text, compilationUnit);
			
			var generatedPath = PathHelper.combine(userDir, "tests/resources/QueryTranslatorTest/generated");
			var generatedDir = new File(generatedPath);
			if (!generatedDir.exists()) {
				generatedDir.mkdir();
			}
			var fileWriter = new FileWriter(PathHelper.combine(generatedPath, test + ".stab.cs"));
			fileWriter.write(result);
			fileWriter.close();
			
			var expectedPath = PathHelper.combine(userDir, "tests/resources/QueryTranslatorTest/expected");
			var expectedFile = new File(PathHelper.combine(expectedPath, test + ".stab.cs"));
			Assert.assertTrue("Expected file not found: " + expectedFile, expectedFile.exists());
			fileReader = new InputStreamReader(new FileInputStream((expectedFile)), Charset.forName("UTF-8"));
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
		
		private String translate(char[] text, CompilationUnitNode compilationUnit) {
			translate(compilationUnit.Body);
			return new CompilationUnitPrinter().print(text, compilationUnit);
		}

		private void translate(PackageBodyNode packageBody) {
			foreach (var m in packageBody.Members) {
				switch (m.PackageMemberKind) {
				case Package:
					var packageDeclaration = (PackageDeclarationNode)m;
					translate(packageDeclaration.Body);
					break;
				case Class:
					translate((ClassDeclarationNode)m);
					break;
				}
			}
		}

		private void translate(ClassDeclarationNode classDeclaration) {
			foreach (var m in classDeclaration.Members) {
				switch (m.TypeMemberKind) {
				case Method:
					var methodDeclaration = (MethodDeclarationNode)m;
					translator.translate(methodDeclaration.Body);
					break;
				}				
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
