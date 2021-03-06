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
using org.junit;
using stab.reflection;
using cnatural.helpers;

namespace cnatural.compiler.test {

	public abstract class ErrorTest {
		private static Library typeSystem;
		protected CompilerParameters parameters*;
		
		[After]
		public void after() {
			parameters = null;
		}
		
		public abstract String ResourcesPath {
			get;
		}
		
		protected void doTest(String className, String[] errors, String[] warnings) {
			var userDir = System.getProperty("user.dir");
			var resourcesPath = PathHelper.combine(userDir, PathHelper.combine("tests/resources", this.ResourcesPath));
			var sourcesPath = PathHelper.combine(resourcesPath, "sources");
			var path = PathHelper.combine(sourcesPath, PathHelper.getFileName(className.replace('.', '/')) + ".stab.cs");

			var compiler = new Compiler();
			if (this.parameters == null) {
				this.parameters = new CompilerParameters();
			}
			if (typeSystem == null) {
				typeSystem = new Library(new String[] { PathHelper.combine(userDir, "runtime/stabrt.jar") });
			}
			this.parameters.TypeSystem = new Library(new String[0], typeSystem);
			this.parameters.AnnotatedTypeSystem = new Library(new String[] { PathHelper.combine(userDir, "annotated/stabal.jar") },
					this.parameters.TypeSystem);

			var results = compiler.compileFromFiles(this.parameters, new File[] { new File(path) });

			var nerrors = 0;
			var nwarnings = 0;
			var testFailed = false;
			foreach (var error in results.Errors) {
				var sb = new StringBuilder();
				sb.append(PathHelper.getFileName(error.Filename));
				if (error.Line > 0) {
					sb.append(" (").append(error.Line);
				}
				if (error.Column > 0) {
					sb.append(", ").append(error.Column);
				}
				sb.append(")");
				if (error.Level == 0) {
					sb.append(" error ");
				} else {
					sb.append(" warning ");
				}
				sb.append(error.Id).append(": ").append(error.Message);
				if (error.Level == 0) {
					if (nerrors == sizeof(errors) || !sb.toString().equals(errors[nerrors++])) {
						System.out.println(sb.toString());
						testFailed = true;
					}
				} else {
					if (nwarnings == sizeof(warnings) || !sb.toString().equals(warnings[nwarnings++])) {
						System.out.println(sb.toString());
						testFailed = true;
					}
				}
			}
            Assert.assertEquals(sizeof(errors), nerrors);
            Assert.assertEquals(sizeof(warnings), nwarnings);
			Assert.assertFalse(testFailed);
		}
	}
}
