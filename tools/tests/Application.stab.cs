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
using org.junit.runner;
using org.junit.runner.notification;
 
namespace stab.tools.tests {

	public class Application {
		public static void main(String[] args) {
			System.exit(new Application().run(args));
		}
		
		public int run(params String[] args) {
			bool wasSuccessful = true;
			if (sizeof(args) > 0) {
				System.out.println();
				System.out.print("Running tests for selectors: ");
				foreach (var arg in args) {
					System.out.print(arg);
					System.out.print(" ");
				}
				System.out.println();
				System.out.println();
				foreach (var arg in args) {
					switch (arg) {
					case "Expressions":
					case "ExpressionsError":
					case "Statements":
					case "StatementsError":
					case "ObjectModel":
					case "ObjectModelError":
					case "Library":
					case "ExpressionTrees":
						wasSuccessful &= runTests(new Class<?>[] {
							Class.forName("stab.tools.compiler.test." + arg + "Test"),
						});
						break;
					case "Parser":
						wasSuccessful &= runTests(new Class<?>[] {
							Class.forName("stab.tools.parser.test.SourceCodeScannerTest"),
							Class.forName("stab.tools.parser.test.PreprocessorTest"),
							Class.forName("stab.tools.parser.test.PreprocessedTextScannerTest"),
							Class.forName("stab.tools.parser.test.ParseExpressionTest"),
							Class.forName("stab.tools.parser.test.ParseCompilationUnitTest"),
						});
						break;
					case "Errors":
						wasSuccessful &= runTests(new Class<?>[] {
							Class.forName("stab.tools.compiler.test.SyntaxErrorTest"),
							Class.forName("stab.tools.compiler.test.ObjectModelErrorTest"),
							Class.forName("stab.tools.compiler.test.StatementsErrorTest"),
							Class.forName("stab.tools.compiler.test.ExpressionsErrorTest"),
						});
						break;
					default:
						System.out.println("Unrecognized selector: " + arg);
						return 100;
					}
				}
			} else {
				System.out.println();
				System.out.println("Running all tests...");
				System.out.println();
				wasSuccessful = runTests(new Class<?>[] {
					Class.forName("stab.tools.helpers.test.UserDataContainerTest"),
					Class.forName("stab.tools.parser.test.SourceCodeScannerTest"),
					Class.forName("stab.tools.parser.test.PreprocessorTest"),
					Class.forName("stab.tools.parser.test.PreprocessedTextScannerTest"),
					Class.forName("stab.tools.parser.test.ParseExpressionTest"),
					Class.forName("stab.tools.parser.test.ParseCompilationUnitTest"),
					Class.forName("stab.reflection.test.TypeSystemTest"),
					Class.forName("stab.reflection.test.TypeBuilderTest"),
					Class.forName("stab.tools.compiler.test.MemberInfoTest"),
					Class.forName("stab.tools.compiler.test.MemberResolverTest"),
					Class.forName("stab.tools.compiler.test.ExpressionsTest"),
					Class.forName("stab.tools.compiler.test.ObjectModelTest"),
					Class.forName("stab.tools.compiler.test.StatementsTest"),
					Class.forName("stab.tools.compiler.test.LibraryTest"),
					Class.forName("stab.tools.compiler.test.ExpressionTreesTest"),
					Class.forName("stab.tools.compiler.test.SyntaxErrorTest"),
					Class.forName("stab.tools.compiler.test.ObjectModelErrorTest"),
					Class.forName("stab.tools.compiler.test.StatementsErrorTest"),
					Class.forName("stab.tools.compiler.test.ExpressionsErrorTest"),
					Class.forName("stab.tools.compiler.test.IntegrationTest"),
					Class.forName("stab.tools.syntaxtree.test.QueryTranslatorTest"),
				});
				if (wasSuccessful) {
					System.out.println();
					System.out.println("Moving binaries...");
					move("annotated/stabal.jar", "bin/stabal.jar");
					move("compiler/stabc.jar", "bin/stabc.jar");
					move("runtime/stabrt.jar", "bin/stabrt.jar");
				}
			}		
			return wasSuccessful ? 0 : 1;
		}
		
		private static bool runTests(Class<?>[] list) {
			JUnitCore core = new JUnitCore();
			core.addListener(new TestListener());
			Result result = core.run(list);
			return result.wasSuccessful();
		}
		
		private static void move(String sourcePath, String targetPath) {
			var sourceFile = new File(sourcePath);
			var targetFile = new File(targetPath);
			copy(sourceFile, targetFile);
			sourceFile.delete();
		}

		private static byte[] buf = new byte[4096];
		
		private static void copy(File src, File dst) {
	    	var is = new FileInputStream(src);
    		var os = new FileOutputStream(dst);
		    int len;
    		while ((len = is.read(buf)) > 0) {
        		os.write(buf, 0, len);
    		}
    		is.close();
    		os.close();
		}
 	}
 	
 	class TestListener : RunListener {
 		private String className;
 		private int globalCount;
 		private int count;
 		private int success;
 		private long startTime;
 	
		public override void testFinished(Description d) {
			globalCount++;
			success++;
			if (!d.getClassName().equals(className)) {
				if (className != null) {
					System.out.println(className + ": " + success + "/" + count);
				}
				className = d.getClassName();
				count = 0;
				success = 0;
			}
			count++;
		}
		
		public override void testFailure(Failure f) {
			success--;
			System.out.println("Failure: " + f.getTestHeader());
			f.getException().printStackTrace();
		}

		public override void testRunStarted(Description d) {
			startTime = System.nanoTime();
		}
		
		public override void testRunFinished(Result r) {
			System.out.println(className + ": " + success + "/" + count);
			System.out.println();
			System.out.println(String.format("%d tests run in %.2fs", globalCount, (System.nanoTime() - startTime) / 1e9));
		}
 	}
}
