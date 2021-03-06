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
using org.junit;

namespace cnatural.compiler.test {

	public class StatementsTest : ExecutionTest {
		protected override String ResourcesPath {
			get {
				return "StatementsTest";
			}
		}
		
		[Test]
		public void intLocal() {
			doTest("IntLocal", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void implicitIntLocal() {
			doTest("ImplicitIntLocal", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void intArgument() {
			doTest("IntArgument", new Class<?>[] { typeof(int) }, new Object[] { 1 }, 1);
		}
		
		[Test]
		public void ifArgument() {
			doTest("IfArgument", new Class<?>[] { typeof(bool) }, new Object[] { true }, 1);
		}
		
		[Test]
		public void ifArgument_2() {
			doTest("IfArgument", new Class<?>[] { typeof(bool) }, new Object[] { false }, 2);
		}
		
		[Test]
		public void localVariable() {
			doTest("LocalVariable", new Class<?>[] { typeof(int) }, new Object[] { 2 }, 4);
		}
		
		[Test]
		public void while_0() {
			doTest("While", new Class<?>[] { typeof(int) }, new Object[] { 3 }, 3);
		}
		
		[Test]
		public void whileBreak() {
			doTest("WhileBreak", new Class<?>[] { typeof(int) }, new Object[] { 3 }, true);
		}
		
		[Test]
		public void for_0() {
			doTest("For", new Class<?>[] { typeof(int) }, new Object[] { 5 }, 3);
		}
		
		[Test]
		public void forNoIteration() {
			doTest("ForNoIteration", new Class<?>[] { typeof(int) }, new Object[] { 5 }, 3);
		}
		
		[Test]
		public void forNoDeclaration() {
			doTest("ForNoDeclaration", new Class<?>[] { typeof(int) }, new Object[] { 5 }, 3);
		}
		
		[Test]
		public void forInitializations() {
			doTest("ForInitializations", new Class<?>[] { typeof(int) }, new Object[] { 5 }, 3);
		}
		
		[Test]
		public void continue_0() {
			doTest("Continue", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void nestedIf() {
			doTest("NestedIf", new Class<?>[] { typeof(int) }, new Object[] { 0 }, 2);
		}
		
		[Test]
		public void emptyStatement() {
			doTest("EmptyStatement", new Class<?>[] {}, new Object[] {}, null);
		}
		
		[Test]
		public void switchDefault() {
			doTest("SwitchDefault", new Class<?>[] { typeof(int) }, new Object[] { 1 }, 2);
		}
		
		[Test]
		public void switchCase() {
			doTest("SwitchCase", new Class<?>[] { typeof(int) }, new Object[] { 1 }, 2);
		}
		
		[Test]
		public void switchCase_1() {
			doTest("SwitchCase", new Class<?>[] { typeof(int) }, new Object[] { 2 }, 2);
		}
		
		[Test]
		public void switchCases() {
			doTest("SwitchCases", new Class<?>[] { typeof(int) }, new Object[] { 1 }, 2);
		}
		
		[Test]
		public void switchCases_1() {
			doTest("SwitchCases", new Class<?>[] { typeof(int) }, new Object[] { 2 }, 4);
		}
		
		[Test]
		public void switchCases_2() {
			doTest("SwitchCases", new Class<?>[] { typeof(int) }, new Object[] { 3 }, 3);
		}
		
		[Test]
		public void switchCases2() {
			doTest("SwitchCases2", new Class<?>[] { typeof(int) }, new Object[] { 1 }, 2);
		}
		
		[Test]
		public void switchCases2_1() {
			doTest("SwitchCases2", new Class<?>[] { typeof(int) }, new Object[] { 2 }, 2);
		}
		
		[Test]
		public void switchCases2_2() {
			doTest("SwitchCases2", new Class<?>[] { typeof(int) }, new Object[] { 3 }, 6);
		}
		
		[Test]
		public void switchCases3() {
			doTest("SwitchCases3", new Class<?>[] { typeof(int) }, new Object[] { 1 }, 2);
		}
		
		[Test]
		public void switchCases3_1() {
			doTest("SwitchCases3", new Class<?>[] { typeof(int) }, new Object[] { 2 }, 4);
		}
		
		[Test]
		public void switchCases3_2() {
			doTest("SwitchCases3", new Class<?>[] { typeof(int) }, new Object[] { 3 }, 6);
		}
		
		[Test]
		public void switchCases3_3() {
			doTest("SwitchCases3", new Class<?>[] { typeof(int) }, new Object[] { 4 }, 7);
		}
		
		[Test]
		public void switchCases4() {
			doTest("SwitchCases4", new Class<?>[] { typeof(int) }, new Object[] { 1 }, 2);
		}
		
		[Test]
		public void switchCases4_1() {
			doTest("SwitchCases4", new Class<?>[] { typeof(int) }, new Object[] { 2 }, 4);
		}
		
		[Test]
		public void switchCases4_2() {
			doTest("SwitchCases3", new Class<?>[] { typeof(int) }, new Object[] { 3 }, 6);
		}
		
		[Test]
		public void switchCases4_3() {
			doTest("SwitchCases4", new Class<?>[] { typeof(int) }, new Object[] { 4 }, 5);
		}
		
		[Test]
		public void switchString() {
			doTest("SwitchString", new Class<?>[] { typeof(String) }, new Object[] { "STR" }, 1);
		}
		
		[Test]
		public void switchString_1() {
			doTest("SwitchString", new Class<?>[] { typeof(String) }, new Object[] { "STRR" }, 0);
		}
		
		[Test]
		public void switchString2() {
			doTest("SwitchString2", new Class<?>[] { typeof(String) }, new Object[] { "STR" }, 1);
		}
		
		[Test]
		public void switchString2_1() {
			doTest("SwitchString2", new Class<?>[] { typeof(String) }, new Object[] { "STRR" }, 2);
		}
		
		[Test]
		public void switchString3() {
			doTest("SwitchString3", new Class<?>[] { typeof(String) }, new Object[] { "STR1" }, 1);
		}
		
		[Test]
		public void switchString3_1() {
			doTest("SwitchString3", new Class<?>[] { typeof(String) }, new Object[] { "STR2" }, 3);
		}
		
		[Test]
		public void switchString3_2() {
			doTest("SwitchString3", new Class<?>[] { typeof(String) }, new Object[] { "STR3" }, 2);
		}
		
		[Test]
		public void switchString4() {
			doTest("SwitchString4", new Class<?>[] { typeof(String) }, new Object[] { "STR1" }, 1);
		}
		
		[Test]
		public void switchString4_1() {
			doTest("SwitchString4", new Class<?>[] { typeof(String) }, new Object[] { "STR2" }, 3);
		}
		
		[Test]
		public void switchString4_2() {
			doTest("SwitchString4", new Class<?>[] { typeof(String) }, new Object[] { "STR3" }, 3);
		}
		
		[Test]
		public void switchString5() {
			doTest("SwitchString5", new Class<?>[] { typeof(String) }, new Object[] { "STR1" }, 1);
		}
		
		[Test]
		public void switchString5_1() {
			doTest("SwitchString5", new Class<?>[] { typeof(String) }, new Object[] { "STR8" }, 3);
		}
		
		[Test]
		public void switchString5_2() {
			doTest("SwitchString5", new Class<?>[] { typeof(String) }, new Object[] { "STR5" }, 7);
		}
		
		[Test]
		public void gotoDefault() {
			doTest("GotoDefault", new Class<?>[] { typeof(int) }, new Object[] { 1 }, 5);
		}
		
		[Test]
		public void gotoDefault_1() {
			doTest("GotoDefault", new Class<?>[] { typeof(int) }, new Object[] { 2 }, 3);
		}
		
		[Test]
		public void gotoCase() {
			doTest("GotoCase", new Class<?>[] { typeof(int) }, new Object[] { 1 }, 9);
		}
		
		[Test]
		public void gotoCase_1() {
			doTest("GotoCase", new Class<?>[] { typeof(int) }, new Object[] { 2 }, 6);
		}
		
		[Test]
		public void gotoCase_2() {
			doTest("GotoCase", new Class<?>[] { typeof(int) }, new Object[] { 3 }, 4);
		}
		
		[Test]
		public void gotoCase2() {
			doTest("GotoCase2", new Class<?>[] { typeof(int) }, new Object[] { 1 }, 4);
		}
		
		[Test]
		public void gotoCase2_1() {
			doTest("GotoCase2", new Class<?>[] { typeof(int) }, new Object[] { 2 }, 3);
		}
		
		[Test]
		public void gotoCase2_2() {
			doTest("GotoCase2", new Class<?>[] { typeof(int) }, new Object[] { 3 }, 3);
		}
		
		[Test]
		public void goto_0() {
			doTest("Goto", new Class<?>[] { typeof(int) }, new Object[] { -1 }, 1);
		}
		
		[Test]
		public void goto_1() {
			doTest("Goto", new Class<?>[] { typeof(int) }, new Object[] { 1 }, 1);
		}
		
		[Test]
		public void tryCatch() {
			doTest("TryCatch", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void tryCatch2() {
			doTest("TryCatch2", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void tryCatchNested() {
			doTest("TryCatchNested", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void tryFinally() {
			doTest("TryFinally", new Class<?>[] {}, new Object[] {}, 6);
		}
		
		[Test]
		public void tryFinallyNested() {
			doTest("TryFinallyNested", new Class<?>[] {}, new Object[] {}, 111);
		}
		
		[Test]
		public void tryFinallyNestedReturn() {
			doTest("TryFinallyNestedReturn", new Class<?>[] {}, new Object[] {}, true);
		}

		[Test]
		public void tryFinallyGoto() {
			doTest("TryFinallyGoto", new Class<?>[] {}, new Object[] {}, 2);
		}

		[Test]
		public void synchronized_0() {
			doTest("Synchronized", new Class<?>[] {}, new Object[] {}, null);
		}

		[Test]
		public void yieldString() {
			doTest("YieldString", new Class<?>[] {}, new Object[] {}, "abc");
		}
		
		[Test]
		public void switchEnum() {
			doTest("SwitchEnum", new Class<?>[] {}, new Object[] {}, 2);
		}

		[Test]
		public void yieldLoop() {
			doTest("YieldLoop", new Class<?>[] {}, new Object[] {}, "2, 4, 8, 16, 32, 64, 128, 256");
		}
		
		[Test]
		public void yieldTry() {
			doTest("YieldTry", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void yieldTry2() {
			doTest("YieldTry2", new Class<?>[] {}, new Object[] {}, 12);
		}
		
		[Test]
		public void foreachList() {
			doTest("ForeachList", new Class<?>[] {}, new Object[] {}, "abc");
		}
		
		[Test]
		public void foreachArray() {
			doTest("ForeachArray", new Class<?>[] {}, new Object[] {}, "abc");
		}
		
		[Test]
		public void foreachInt() {
			doTest("ForeachInt", new Class<?>[] {}, new Object[] {}, "0, 1, 2, 3, 4");
		}
		
		[Test]
		public void foreachInt2() {
			doTest("p.ForeachInt2", new Class<?>[] {}, new Object[] {}, "0, 2, 4, 6, 8");
		}
		
		[Test]
		public void switchEnum2() {
			doTest("SwitchEnum2", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void switchChar() {
			doTest("SwitchChar", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void switchBlock() {
			doTest("SwitchBlock", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void controlFlow() {
			doTest("ControlFlow", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void definiteAssignment() {
			doTest("DefiniteAssignment", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void gotoFor() {
			doTest("GotoFor", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void forVariables() {
			doTest("ForVariables", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void tryCatchNested2() {
			doTest("TryCatchNested2", new Class<?>[] {}, new Object[] {}, 6);
		}
		
		[Test]
		public void controlFlow2() {
			doTest("ControlFlow2", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void switchEnum3() {
			doTest("SwitchEnum3", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void tryInFinally() {
			doTest("TryInFinally", new Class<?>[] {}, new Object[] {}, 5);
		}
		
		[Test]
		public void using_0() {
			doTest("Using", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void using2() {
			doTest("Using2", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void using3() {
			doTest("Using3", new Class<?>[] {}, new Object[] {}, 4);
		}
		
		[Test]
		public void ifFalse() {
			doTest("IfFalse", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void localWildcard() {
			doTest("LocalWildcard", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void forBreak() {
			doTest("ForBreak", new Class<?>[] {}, new Object[] {}, "str2");
		}
		
		[Test]
		public void unusedLocal() {
			doTest("UnusedLocal", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void switchString6() {
			doTest("SwitchString6", new Class<?>[] { typeof(String) }, new Object[] { "STR1" }, 1);
		}
		
		[Test]
		public void switchString6_1() {
			doTest("SwitchString6", new Class<?>[] { typeof(String) }, new Object[] { "STR2" }, 1);
		}
		
		[Test]
		public void switchString6_2() {
			doTest("SwitchString6", new Class<?>[] { typeof(String) }, new Object[] { " " }, 0);
		}
		
		[Test]
		public void switchString7() {
			doTest("SwitchString7", new Class<?>[] { typeof(String) }, new Object[] { "STR1" }, 1);
		}
		
		[Test]
		public void switchString7_1() {
			doTest("SwitchString7", new Class<?>[] { typeof(String) }, new Object[] { "STR7" }, 1);
		}
		
		[Test]
		public void switchString7_2() {
			doTest("SwitchString7", new Class<?>[] { typeof(String) }, new Object[] { " " }, 0);
		}
		
		[Test]
		public void switchString8() {
			doTest("SwitchString8", new Class<?>[] { typeof(String) }, new Object[] { "STR1" }, 1);
		}
		
		[Test]
		public void switchString8_1() {
			doTest("SwitchString8", new Class<?>[] { typeof(String) }, new Object[] { "STR7" }, 0);
		}
		
		[Test]
		public void switchString8_2() {
			doTest("SwitchString8", new Class<?>[] { typeof(String) }, new Object[] { " " }, 1);
		}
		
		[Test]
		public void switchString9() {
			doTest("SwitchString9", new Class<?>[] { typeof(String) }, new Object[] { "STR1" }, 1);
		}
		
		[Test]
		public void switchString9_1() {
			doTest("SwitchString9", new Class<?>[] { typeof(String) }, new Object[] { "STR2" }, 0);
		}
		
		[Test]
		public void switchString9_2() {
			doTest("SwitchString9", new Class<?>[] { typeof(String) }, new Object[] { " " }, 1);
		}
		
		[Test]
		public void switchString10() {
			doTest("SwitchString10", new Class<?>[] { typeof(String) }, new Object[] { "STR1" }, 1);
		}
		
		[Test]
		public void switchString10_1() {
			doTest("SwitchString10", new Class<?>[] { typeof(String) }, new Object[] { "STR2" }, 0);
		}
		
		[Test]
		public void switchString10_2() {
			doTest("SwitchString10", new Class<?>[] { typeof(String) }, new Object[] { " " }, 1);
		}
		
		[Test]
		public void tryFinallyLocal() {
			doTest("TryFinallyLocal", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void tryInFinally2() {
			doTest("TryInFinally2", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void for2() {
			doTest("For2", new Class<?>[] {}, new Object[] {}, "AC");
		}
		
		[Test]
		public void for3() {
			doTest("For3", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void returnConversion() {
			doTest("ReturnConversion", new Class<?>[] {}, new Object[] {}, 3);
		}
	}
}
