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

	public class ExpressionsTest : ExecutionTest {
		protected override String ResourcesPath {
			get {
				return "ExpressionsTest";
			}
		}
		
		[Test]
		public void returnInt() {
			doTest("ReturnInt", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void returnTrue() {
			doTest("ReturnTrue", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void returnFalse() {
			doTest("ReturnFalse", new Class<?>[] {}, new Object[] {}, false);
		}
		
		[Test]
		public void returnString() {
			doTest("ReturnString", new Class<?>[] {}, new Object[] {}, "string");
		}
		
		[Test]
		public void returnNull() {
			doTest("ReturnNull", new Class<?>[] {}, new Object[] {}, null);
		}
		
		[Test]
		public void returnLong() {
			doTest("ReturnLong", new Class<?>[] {}, new Object[] {}, 1L);
		}
		
		[Test]
		public void returnFloat() {
			doTest("ReturnFloat", new Class<?>[] {}, new Object[] {}, 1f);
		}
		
		[Test]
		public void returnDouble() {
			doTest("ReturnDouble", new Class<?>[] {}, new Object[] {}, 1d);
		}
		
		[Test]
		public void fullNameCall() {
			doTest("FullNameCall", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void intMinus() {
			doTest("IntMinus", new Class<?>[] { typeof(int) }, new Object[] { 1 }, -1);
		}
		
		[Test]
		public void intAdd() {
			doTest("IntAdd", new Class<?>[] { typeof(int) }, new Object[] { 1 }, 2);
		}
		
		[Test]
		public void intSubtract() {
			doTest("IntSubtract", new Class<?>[] { typeof(int) }, new Object[] { 3 }, 2);
		}
		
		[Test]
		public void intMultiply() {
			doTest("IntMultiply", new Class<?>[] { typeof(int) }, new Object[] { 3 }, 6);
		}
		
		[Test]
		public void intDivide() {
			doTest("IntDivide", new Class<?>[] { typeof(int) }, new Object[] { 6 }, 3);
		}
		
		[Test]
		public void intRemainder() {
			doTest("IntRemainder", new Class<?>[] { typeof(int) }, new Object[] { 11 }, 2);
		}
		
		[Test]
		public void intComplement() {
			doTest("IntComplement", new Class<?>[] { typeof(int) }, new Object[] { 123 }, ~123);
		}
		
		[Test]
		public void not() {
			doTest("Not", new Class<?>[] { typeof(bool) }, new Object[] { true }, false);
		}
		
		[Test]
		public void not_2() {
			doTest("Not", new Class<?>[] { typeof(bool) }, new Object[] { false }, true);
		}
		
		[Test]
		public void intPlus() {
			doTest("IntPlus", new Class<?>[] { typeof(int) }, new Object[] { 1 }, 1);
		}
		
		[Test]
		public void assignArgument() {
			doTest("AssignArgument", new Class<?>[] { typeof(int) }, new Object[] { 1 }, 2);
		}
		
		[Test]
		public void arrayAccess() {
			doTest("ArrayAccess", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void arrayInitializer() {
			doTest("ArrayInitializer", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void arraySize() {
			doTest("ArraySize", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void intLessThan() {
			doTest("IntLessThan", new Class<?>[] { typeof(int), typeof(int) }, new Object[] { 1, 2 }, true);
		}
		
		[Test]
		public void intLessThan_2() {
			doTest("IntLessThan", new Class<?>[] { typeof(int), typeof(int) }, new Object[] { 2, 1 }, false);
		}
		
		[Test]
		public void intLessThan_3() {
			doTest("IntLessThan", new Class<?>[] { typeof(int), typeof(int) }, new Object[] { 1, 1 }, false);
		}
		
		[Test]
		public void intEqual() {
			doTest("IntEqual", new Class<?>[] { typeof(int), typeof(int) }, new Object[] { 1, 1 }, true);
		}
		
		[Test]
		public void intEqual_2() {
			doTest("IntEqual", new Class<?>[] { typeof(int), typeof(int) }, new Object[] { 1, 2 }, false);
		}
		
		[Test]
		public void intPostIncrement() {
			doTest("IntPostIncrement", new Class<?>[] { typeof(int) }, new Object[] { 1 }, true);
		}
		
		[Test]
		public void longPostIncrement() {
			doTest("LongPostIncrement", new Class<?>[] { typeof(long) }, new Object[] { 1L }, true);
		}
		
		[Test]
		public void intPreIncrement() {
			doTest("IntPreIncrement", new Class<?>[] { typeof(int) }, new Object[] { 1 }, true);
		}
		
		[Test]
		public void longPreIncrement() {
			doTest("LongPreIncrement", new Class<?>[] { typeof(long) }, new Object[] { 1L }, true);
		}
		
		[Test]
		public void intFieldPostIncrement() {
			doTest("IntFieldPostIncrement", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void longFieldPostIncrement() {
			doTest("LongFieldPostIncrement", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void intFieldPreIncrement() {
			doTest("IntFieldPreIncrement", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void longFieldPreIncrement() {
			doTest("LongFieldPreIncrement", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void intArrayPreIncrement() {
			doTest("IntArrayPreIncrement", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void intAddAssign() {
			doTest("IntAddAssign", new Class<?>[] { typeof(int) }, new Object[] { 1 }, true);
		}
		
		[Test]
		public void doubleAddAssign() {
			doTest("DoubleAddAssign", new Class<?>[] { typeof(double) }, new Object[] { 1d }, true);
		}
		
		[Test]
		public void typeof_0() {
			doTest("Typeof", new Class<?>[] {}, new Object[] {}, typeof(int));
		}
		
		[Test]
		public void typeofObject() {
			doTest("TypeofObject", new Class<?>[] {}, new Object[] {}, typeof(Object));
		}
		
		[Test]
		public void doubleIntAdd() {
			doTest("DoubleIntAdd", new Class<?>[] { typeof(double), typeof(int) }, new Object[] { 2d, 3 }, 5d);
		}
		
		[Test]
		public void doubleIntLessThan() {
			doTest("DoubleIntLessThan", new Class<?>[] { typeof(double), typeof(int) }, new Object[] { 2d, 3 }, true);
		}
		
		[Test]
		public void intDoubleEqual() {
			doTest("IntDoubleEqual", new Class<?>[] { typeof(int), typeof(double) }, new Object[] { 1, 1d }, true);
		}
		
		[Test]
		public void logicalAnd() {
			doTest("LogicalAnd", new Class<?>[] { typeof(long), typeof(float) }, new Object[] { 1l, 1f }, true);
		}
		
		[Test]
		public void logicalAnd_1() {
			doTest("LogicalAnd", new Class<?>[] { typeof(long), typeof(float) }, new Object[] { 0l, 1f }, false);
		}
		
		[Test]
		public void logicalOr() {
			doTest("LogicalOr", new Class<?>[] { typeof(short), typeof(byte) }, new Object[] { (short)1, (byte)0 }, true);
		}
		
		[Test]
		public void logicalOr_1() {
			doTest("LogicalOr", new Class<?>[] { typeof(short), typeof(byte) }, new Object[] { (short)0, (byte)0 }, false);
		}
		
		[Test]
		public void logicalAndIf() {
			doTest("LogicalAndIf", new Class<?>[] { typeof(int), typeof(int) }, new Object[] { 1, 1 }, true);
		}
		
		[Test]
		public void logicalAndIf_1() {
			doTest("LogicalAndIf", new Class<?>[] { typeof(int), typeof(int) }, new Object[] { 1, 0 }, false);
		}
		
		[Test]
		public void logicalOrIf() {
			doTest("LogicalOrIf", new Class<?>[] { typeof(int), typeof(int) }, new Object[] { 1, 0 }, true);
		}
		
		[Test]
		public void logicalOrIf_1() {
			doTest("LogicalOrIf", new Class<?>[] { typeof(int), typeof(int) }, new Object[] { 0, 0 }, false);
		}
		
		[Test]
		public void longIntAnd() {
			doTest("LongIntAnd", new Class<?>[] { typeof(long), typeof(int) }, new Object[] { 0xFFFFFFFFFFl, 0xF0F0 }, 0xF0F0L);
		}
		
		[Test]
		public void intLongOr() {
			doTest("IntLongOr", new Class<?>[] { typeof(int), typeof(long) }, new Object[] { 0xF0F0, 0xF0F0F00000l }, 0xF0F0F0F0F0l);
		}

		[Test]
		public void shortRightShift() {
			doTest("ShortRightShift", new Class<?>[] { typeof(short) }, new Object[] { (short)8 }, 2);
		}

		[Test]
		public void constantRightShift() {
			doTest("ConstantRightShift", new Class<?>[] {}, new Object[] {}, (short)30864);
		}

		[Test]
		public void literalChar() {
			doTest("LiteralChar", new Class<?>[] {}, new Object[] {}, "abc");
		}

		[Test]
		public void intConstantXor() {
			doTest("IntConstantXor", new Class<?>[] { typeof(int) }, new Object[] { 0x0F0F }, 0xFFFF);
		}

		[Test]
		public void instanceof_0() {
			doTest("Instanceof", new Class<?>[] { typeof(Object) }, new Object[] { "STR" }, true);
		}

		[Test]
		public void instanceof_1() {
			doTest("Instanceof", new Class<?>[] { typeof(Object) }, new Object[] { new Object() }, false);
		}

		[Test]
		public void verbatimString() {
			doTest("VerbatimString", new Class<?>[] {}, new Object[] {}, "\r\n\" Verbatim\\String \"\r\n");
		}

		[Test]
		public void asOperator() {
			doTest("AsOperator", new Class<?>[] { typeof(Object) }, new Object[] { "STR" }, "STR");
		}

		[Test]
		public void asOperator_1() {
			doTest("AsOperator", new Class<?>[] { typeof(Object) }, new Object[] { new Object() }, null);
		}
		
		[Test]
		public void nullCoalescing() {
			doTest("NullCoalescing", new Class<?>[] { typeof(Object), typeof(Object) }, new Object[] { "STR", null }, "STR");
		}
		
		[Test]
		public void nullCoalescing_1() {
			doTest("NullCoalescing", new Class<?>[] { typeof(Object), typeof(Object) }, new Object[] { null, "STR" }, "STR");
		}
		
		[Test]
		public void nullCoalescing_2() {
			doTest("NullCoalescing", new Class<?>[] { typeof(Object), typeof(Object) }, new Object[] { "STR1", "STR2" }, "STR1");
		}
		
		[Test]
		public void nullCoalescing_3() {
			doTest("NullCoalescing", new Class<?>[] { typeof(Object), typeof(Object) }, new Object[] { null, null }, null);
		}

		[Test]
		public void castToString() {
			doTest("CastToString", new Class<?>[] { typeof(Object) }, new Object[] { "STR" }, "STR");
		}

		[Test]
		public void castToInt() {
			doTest("CastToInt", new Class<?>[] { typeof(float) }, new Object[] { 1f }, 1);
		}

		[Test]
		public void constantLogicalAnd() {
			doTest("ConstantLogicalAnd", new Class<?>[] {}, new Object[] {}, false);
		}
		
		[Test]
		public void conditional() {
			doTest("Conditional", new Class<?>[] { typeof(bool), typeof(Object), typeof(Object) }, new Object[] { true, "STR", null }, "STR");
		}
		
		[Test]
		public void conditional_1() {
			doTest("Conditional", new Class<?>[] { typeof(bool), typeof(Object), typeof(Object) }, new Object[] { false, "STR", null }, null);
		}
		
		[Test]
		public void conditionalIf() {
			doTest("ConditionalIf", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { true, true, false }, true);
		}
		
		[Test]
		public void conditionalIf_1() {
			doTest("ConditionalIf", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { false, true, false }, false);
		}
		
		[Test]
		public void arrayInitializer2() {
			doTest("ArrayInitializer2", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void intArrayAssign() {
			doTest("IntArrayAssign", new Class<?>[] { typeof(int[]), typeof(int), typeof(int) },
				new Object[] { new int[1], 0, 2 }, 2);
		}
		
		[Test]
		public void logicalOr2() {
			doTest("LogicalOr2", new Class<?>[] { typeof(String) }, new Object[] { null }, 1);
		}
		
		[Test]
		public void logicalOr2_1() {
			doTest("LogicalOr2", new Class<?>[] { typeof(String) }, new Object[] { "" }, 1);
		}
		
		[Test]
		public void logicalOr2_2() {
			doTest("LogicalOr2", new Class<?>[] { typeof(String) }, new Object[] { "STR" }, 2);
		}
		
		[Test]
		public void logicalOr3() {
			doTest("LogicalOr3", new Class<?>[] { typeof(String) }, new Object[] { null }, 1);
		}
		
		[Test]
		public void logicalOr3_1() {
			doTest("LogicalOr3", new Class<?>[] { typeof(String) }, new Object[] { "" }, 1);
		}
		
		[Test]
		public void logicalOr3_2() {
			doTest("LogicalOr3", new Class<?>[] { typeof(String) }, new Object[] { "STR" }, 1);
		}
		
		[Test]
		public void logicalOr3_3() {
			doTest("LogicalOr3", new Class<?>[] { typeof(String) }, new Object[] { "sTR" }, 2);
		}
		
		[Test]
		public void logicalOrWhile() {
			doTest("LogicalOrWhile", new Class<?>[] { typeof(int) }, new Object[] { -2 }, -1);
		}
		
		[Test]
		public void logicalOrWhile_1() {
			doTest("LogicalOrWhile", new Class<?>[] { typeof(int) }, new Object[] { 11 }, 9);
		}
		
		[Test]
		public void logicalOrWhile2() {
			doTest("LogicalOrWhile2", new Class<?>[] { typeof(int) }, new Object[] { 0 }, -1);
		}
		
		[Test]
		public void logicalAnd2() {
			doTest("LogicalAnd2", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void logicalAnd3() {
			doTest("LogicalAnd3", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void logicalAnd4() {
			doTest("LogicalAnd4", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) }, new Object[] { true, true, true }, true);
		}
		
		[Test]
		public void logicalAnd4_1() {
			doTest("LogicalAnd4", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) }, new Object[] { true, true, false }, false);
		}
		
		[Test]
		public void logicalAnd4_2() {
			doTest("LogicalAnd4", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) }, new Object[] { true, false, false }, false);
		}
		
		[Test]
		public void logicalAnd5() {
			doTest("LogicalAnd5", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) }, new Object[] { true, true, true }, true);
		}
		
		[Test]
		public void logicalAnd5_1() {
			doTest("LogicalAnd5", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) }, new Object[] { true, true, false }, false);
		}
		
		[Test]
		public void logicalAnd5_2() {
			doTest("LogicalAnd5", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) }, new Object[] { true, false, false }, false);
		}
		
		[Test]
		public void logicalOr4() {
			doTest("LogicalOr4", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) }, new Object[] { false, false, false }, false);
		}
		
		[Test]
		public void logicalOr4_1() {
			doTest("LogicalOr4", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) }, new Object[] { false, false, true }, true);
		}
		
		[Test]
		public void logicalOr4_2() {
			doTest("LogicalOr4", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) }, new Object[] { false, true, false }, true);
		}
		
		[Test]
		public void logicalOr5() {
			doTest("LogicalOr5", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) }, new Object[] { false, false, false }, false);
		}
		
		[Test]
		public void logicalOr5_1() {
			doTest("LogicalOr5", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) }, new Object[] { false, false, true }, true);
		}
		
		[Test]
		public void logicalOr5_2() {
			doTest("LogicalOr5", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) }, new Object[] { false, true, false }, true);
		}

		[Test]
		public void logicalAndOr() {
			doTest("LogicalAndOr", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { false, false, false }, false);
		}

		[Test]
		public void logicalAndOr_1() {
			doTest("LogicalAndOr", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { false, false, true }, true);
		}

		[Test]
		public void logicalAndOr_2() {
			doTest("LogicalAndOr", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { true, false, false }, false);
		}

		[Test]
		public void logicalAndOr_3() {
			doTest("LogicalAndOr", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { true, true, false }, true);
		}

		[Test]
		public void logicalOrAnd() {
			doTest("LogicalOrAnd", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { false, false, false }, false);
		}

		[Test]
		public void logicalOrAnd_1() {
			doTest("LogicalOrAnd", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { false, false, true }, false);
		}

		[Test]
		public void logicalOrAnd_2() {
			doTest("LogicalOrAnd", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { true, false, true }, true);
		}

		[Test]
		public void logicalOrAnd_3() {
			doTest("LogicalOrAnd", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { false, true, true }, true);
		}

		[Test]
		public void logicalAndOr2() {
			doTest("LogicalAndOr2", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { false, false, false }, false);
		}

		[Test]
		public void logicalAndOr2_1() {
			doTest("LogicalAndOr2", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { true, false, true }, true);
		}

		[Test]
		public void logicalAndOr2_2() {
			doTest("LogicalAndOr2", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { true, false, false }, false);
		}

		[Test]
		public void logicalAndOr2_3() {
			doTest("LogicalAndOr2", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { true, true, false }, true);
		}

		[Test]
		public void logicalOrAnd2() {
			doTest("LogicalOrAnd2", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { false, false, false }, false);
		}

		[Test]
		public void logicalOrAnd2_1() {
			doTest("LogicalOrAnd2", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { false, false, true }, false);
		}

		[Test]
		public void logicalOrAnd2_2() {
			doTest("LogicalOrAnd2", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { true, false, false }, true);
		}

		[Test]
		public void logicalOrAnd2_3() {
			doTest("LogicalOrAnd2", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { false, true, true }, true);
		}

		[Test]
		public void logicalOrAndOr() {
			doTest("LogicalOrAndOr", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { false, false, false, false }, false);
		}

		[Test]
		public void logicalOrAndOr_1() {
			doTest("LogicalOrAndOr", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { false, true, true, false }, true);
		}

		[Test]
		public void logicalOrAndOr_2() {
			doTest("LogicalOrAndOr", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { true, false, false, true }, true);
		}

		[Test]
		public void logicalOrAndOr_3() {
			doTest("LogicalOrAndOr", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { false, true, false, false }, false);
		}
		
		[Test]
		public void logicalAndOrAnd() {
			doTest("LogicalAndOrAnd", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { false, false, false, false }, false);
		}

		[Test]
		public void logicalAndOrAnd_1() {
			doTest("LogicalAndOrAnd", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { false, true, true, false }, false);
		}

		[Test]
		public void logicalAndOrAnd_2() {
			doTest("LogicalAndOrAnd", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { true, true, false, false }, true);
		}

		[Test]
		public void logicalAndOrAnd_3() {
			doTest("LogicalAndOrAnd", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { false, true, true, true }, true);
		}

		[Test]
		public void logicalAndOrAnd_4() {
			doTest("LogicalAndOrAnd", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { true, false, true, true }, true);
		}

		[Test]
		public void logicalAndOrAnd_5() {
			doTest("LogicalAndOrAnd", new Class<?>[] { typeof(bool), typeof(bool), typeof(bool), typeof(bool) },
				new Object[] { true, false, true, false }, false);
		}
		
		[Test]
		public void byteAssign() {
			doTest("ByteAssign", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void arrayFieldInitializer() {
			doTest("ArrayFieldInitializer", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void byteArrayInitializer() {
			doTest("ByteArrayInitializer", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void castIntToByte() {
			doTest("CastIntToByte", new Class<?>[] { typeof(int) }, new Object[] { 3 }, 3);
		}
		
		[Test]
		public void castIntToByte_1() {
			doTest("CastIntToByte", new Class<?>[] { typeof(int) }, new Object[] { 0xFFFFFFFF }, -1);
		}
		
		[Test]
		public void stringArrayList() {
			doTest("StringArrayList", new Class<?>[] { typeof(java.util.ArrayList<?>) },
				new Object[] { new java.util.ArrayList<String> { "a", "aa", "aaa" } }, 2);
		}
		
		[Test]
		public void objectArray() {
			doTest("ObjectArray", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void arrayIndexer() {
			doTest("ArrayIndexer", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void arrayIndexerAssign() {
			doTest("ArrayIndexerAssign", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void interfaceToString() {
			doTest("InterfaceToString", new Class<?>[] {}, new Object[] {}, "OK");
		}

		[Test]
		public void arrayOfArray() {
			doTest("ArrayOfArray", new Class<?>[] {}, new Object[] {}, 1);
		}

		[Test]
		public void unboundedTypeof() {
			doTest("UnboundedTypeof", new Class<?>[] {}, new Object[] {}, typeof(java.util.ArrayList<?>));
		}
		
		[Test]
		public void constantNot() {
			doTest("ConstantNot", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void autoboxingBoolean() {
			doTest("AutoboxingBoolean", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void autoboxingArgument() {
			doTest("AutoboxingArgument", new Class<?>[] { typeof(int) }, new Object[] { 2 }, 2);
		}
		
		[Test]
		public void autoboxingReturn() {
			doTest("AutoboxingReturn", new Class<?>[] { typeof(int) }, new Object[] { 2 }, 2);
		}
		
		[Test]
		public void autoboxingReturn2() {
			doTest("AutoboxingReturn2", new Class<?>[] { typeof(Long) }, new Object[] { 2L }, 2L);
		}
		
		[Test]
		public void autoboxingIncrement() {
			doTest("AutoboxingIncrement", new Class<?>[] { typeof(Float) }, new Object[] { 2f }, 3f);
		}
		
		[Test]
		public void autoboxingFieldIncrement() {
			doTest("AutoboxingFieldIncrement", new Class<?>[] {}, new Object[] {}, (byte)3);
		}
		
		[Test]
		public void autoboxingFieldIncrement2() {
			doTest("AutoboxingFieldIncrement2", new Class<?>[] {}, new Object[] {}, (byte)3);
		}
		
		[Test]
		public void autoboxingAssignLocal() {
			doTest("AutoboxingAssignLocal", new Class<?>[] { typeof(Short) }, new Object[] { (short)2 }, (short)2);
		}
		
		[Test]
		public void autoboxingAssignLocal2() {
			doTest("AutoboxingAssignLocal2", new Class<?>[] { typeof(Short) }, new Object[] { (short)2 }, 2);
		}
		
		[Test]
		public void autoboxingAssignLocal3() {
			doTest("AutoboxingAssignLocal3", new Class<?>[] { typeof(short) }, new Object[] { (short)2 }, (short)2);
		}
		
		[Test]
		public void autoboxingUnary() {
			doTest("AutoboxingUnary", new Class<?>[] { typeof(Integer) }, new Object[] { 2 }, -2);
		}
		
		[Test]
		public void autoboxingUnary2() {
			doTest("AutoboxingUnary2", new Class<?>[] { typeof(int) }, new Object[] { 2 }, -2);
		}
		
		[Test]
		public void autoboxingBinary() {
			doTest("AutoboxingBinary", new Class<?>[] { typeof(Character), typeof(Character) }, new Object[] { '\u0001', '\u0002' }, 3);
		}
		
		[Test]
		public void autoboxingBinary2() {
			doTest("AutoboxingBinary2", new Class<?>[] { typeof(Integer), typeof(Long) }, new Object[] { 1, 2L }, true);
		}
		
		[Test]
		public void autoboxingElementAccess() {
			doTest("AutoboxingElementAccess", new Class<?>[] { typeof(Integer) }, new Object[] { 1 }, 2);
		}
		
		[Test]
		public void autoboxingElementAccess2() {
			doTest("AutoboxingElementAccess2", new Class<?>[] { typeof(int) }, new Object[] { 1 }, 3);
		}
		
		[Test]
		public void autoboxingArrayInitializer() {
			doTest("AutoboxingArrayInitializer", new Class<?>[] { typeof(int) }, new Object[] { 1 }, 1);
		}
		
		[Test]
		public void autoboxingFieldIncrement3() {
			doTest("AutoboxingFieldIncrement3", new Class<?>[] {}, new Object[] {}, 3l);
		}
		
		[Test]
		public void autoboxingElementAccess3() {
			doTest("AutoboxingElementAccess3", new Class<?>[] { typeof(Integer) }, new Object[] { 1 }, 1);
		}
		
		[Test]
		public void autoboxingConversions() {
			doTest("AutoboxingConversions", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void autoboxingConversions2() {
			doTest("AutoboxingConversions2", new Class<?>[] {}, new Object[] {}, 2L);
		}
		
		[Test]
		public void autoboxingObject() {
			doTest("AutoboxingObject", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void autoboxingEqual() {
			doTest("AutoboxingEqual", new Class<?>[] {}, new Object[] {}, false);
		}
		
		[Test]
		public void autoboxingEqual2() {
			doTest("AutoboxingEqual2", new Class<?>[] {}, new Object[] {}, false);
		}
		
		[Test]
		public void autoboxingBoolean2() {
			doTest("AutoboxingBoolean2", new Class<?>[] {}, new Object[] {}, 10);
		}
		
		[Test]
		public void autoboxingBoolean3() {
			doTest("AutoboxingBoolean3", new Class<?>[] {}, new Object[] {}, null);
		}
		
		[Test]
		public void autoboxingFieldIncrement4() {
			doTest("AutoboxingFieldIncrement4", new Class<?>[] {}, new Object[] {}, 3L);
		}
		
		[Test]
		public void inferenceExact() {
			doTest("InferenceExact", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void inferenceExact2() {
			doTest("InferenceExact2", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void inferenceExact3() {
			doTest("InferenceExact3", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void inferenceLowerBound() {
			doTest("InferenceLowerBound", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void inferenceExactParameter() {
			doTest("InferenceExactParameter", new Class<?>[] {}, new Object[] {}, 5);
		}
		
		[Test]
		public void inferenceExactParameter2() {
			doTest("InferenceExactParameter2", new Class<?>[] {}, new Object[] {}, 5);
		}
		
		[Test]
		public void inferenceCascaded() {
			doTest("InferenceCascaded", new Class<?>[] {}, new Object[] {}, 82449);
		}
		
		[Test]
		public void inferenceCascaded2() {
			doTest("InferenceCascaded2", new Class<?>[] {}, new Object[] {}, 82449);
		}
		
		[Test]
		public void inferenceCascaded3() {
			doTest("InferenceCascaded3", new Class<?>[] {}, new Object[] {}, 82449);
		}
		
		[Test]
		public void inferenceOutputParameter() {
			doTest("InferenceOutputParameter", new Class<?>[] {}, new Object[] {}, null);
		}
		
		[Test]
		public void mixedArithmetic() {
			doTest("MixedArithmetic", new Class<?>[] { typeof(byte), typeof(short), typeof(char), typeof(int), typeof(long), typeof(float) },
				new Object[] { (byte)7, (short)6, (char)5, 4, 3l, 2f }, 13d);
		}
		
		[Test]
		public void booleanBitwise() {
			doTest("BooleanBitwise", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void doubleArithmetic() {
			doTest("DoubleArithmetic", new Class<?>[] {}, new Object[] {}, -1d);
		}
		
		[Test]
		public void staticArrayInitializer() {
			doTest("StaticArrayInitializer", new Class<?>[] {}, new Object[] {}, 10);
		}
		
		[Test]
		public void delegateTarget() {
			doTest("DelegateTarget", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void delegateMethod() {
			doTest("DelegateMethod", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void delegateDynamicInvoke() {
			doTest("DelegateDynamicInvoke", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void delegateAdd() {
			doTest("DelegateAdd", new Class<?>[] {}, new Object[] {}, 5);
		}
		
		[Test]
		public void delegateAddAssign() {
			doTest("DelegateAddAssign", new Class<?>[] {}, new Object[] {}, 5);
		}
		
		[Test]
		public void delegateAddField() {
			doTest("DelegateAddField", new Class<?>[] {}, new Object[] {}, 5);
		}
		
		[Test]
		public void delegateAddArray() {
			doTest("DelegateAddArray", new Class<?>[] {}, new Object[] {}, 5);
		}
		
		[Test]
		public void byteArithmetic() {
			doTest("ByteArithmetic", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void byteOverflow() {
			doTest("ByteOverflow", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void shortArithmetic() {
			doTest("ShortArithmetic", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void shortOverflow() {
			doTest("ShortOverflow", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void charArithmetic() {
			doTest("CharArithmetic", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void charOverflow() {
			doTest("CharOverflow", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void intArithmetic() {
			doTest("IntArithmetic", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void intOverflow() {
			doTest("IntOverflow", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void longArithmetic() {
			doTest("LongArithmetic", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void floatArithmetic() {
			doTest("FloatArithmetic", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void doubleArithmetic2() {
			doTest("DoubleArithmetic2", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void booleanNot() {
			doTest("BooleanNot", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void booleanNot2() {
			doTest("BooleanNot2", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void castGetfield() {
			doTest("CastGetfield", new Class<?>[] {}, new Object[] {}, 0);
		}
		
		[Test]
		public void divideByZero() {
			doTest("DivideByZero", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void arrayHashcode() {
			doTest("ArrayHashcode", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void nullInstanceof() {
			doTest("NullInstanceof", new Class<?>[] {}, new Object[] {}, false);
		}
		
		[Test]
		public void arrayOfArray2() {
			doTest("ArrayOfArray2", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void arrayOfArray3() {
			doTest("ArrayOfArray3", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void arrayCreation() {
			doTest("ArrayCreation", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void arrayCreation2() {
			doTest("ArrayCreation2", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void arrayInitializer3() {
			doTest("ArrayInitializer3", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void arrayCreation3() {
			doTest("ArrayCreation3", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void minIntegerConstant() {
			doTest("MinIntegerConstant", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void minLongConstant() {
			doTest("MinLongConstant", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void minLongConstant2() {
			doTest("MinLongConstant2", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void stringAddConstants() {
			doTest("StringAddConstants", new Class<?>[] {}, new Object[] {}, "abc");
		}
		
		[Test]
		public void stringAdd() {
			doTest("StringAdd", new Class<?>[] { typeof(String) }, new Object[] { "bc" }, "abc");
		}
		
		[Test]
		public void stringAdd2() {
			doTest("StringAdd2", new Class<?>[] { typeof(int) }, new Object[] { 2 }, "a2b");
		}
		
		[Test]
		public void floatLimits() {
			doTest("FloatLimits", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void floatLimits2() {
			doTest("FloatLimits2", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void doubleLimits() {
			doTest("DoubleLimits", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void doubleLimits2() {
			doTest("DoubleLimits2", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void stringEqual() {
			doTest("StringEqual", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void stringEqual2() {
			doTest("StringEqual2", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void longWidening() {
			doTest("LongWidening", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void doubleLimits3() {
			doTest("DoubleLimits3", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void indexerAddAssign() {
			doTest("IndexerAddAssign", new Class<?>[] {}, new Object[] {}, 5);
		}
		
		[Test]
		public void indexerAddAssign2() {
			doTest("IndexerAddAssign2", new Class<?>[] {}, new Object[] {}, 5l);
		}
		
		[Test]
		public void propertyOpAssign() {
			doTest("PropertyOpAssign", new Class<?>[] { typeof(int) }, new Object[] { 2 }, 3);
		}
		
		[Test]
		public void propertyIncrement() {
			doTest("PropertyIncrement", new Class<?>[] {}, new Object[] {}, 2l);
		}
		
		[Test]
		public void byteWidening() {
			doTest("ByteWidening", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void floatNarrowing() {
			doTest("FloatNarrowing", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void stringAdd3() {
			doTest("StringAdd3", new Class<?>[] { typeof(String) }, new Object[] { "c" }, "abcd");
		}
		
		[Test]
		public void indexerIncrement() {
			doTest("IndexerIncrement", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void indexerIncrement2() {
			doTest("IndexerIncrement2", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void objectInitializer() {
			doTest("ObjectInitializer", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void collectionInitializer() {
			doTest("CollectionInitializer", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void collectionInitializer2() {
			doTest("CollectionInitializer2", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void arrayRuntimeError() {
			doTest("ArrayRuntimeError", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void conditional2() {
			doTest("Conditional2", new Class<?>[] { typeof(bool), typeof(double) }, new Object[] { true, 1d }, 1d);
		}
		
		[Test]
		public void wildcard() {
			doTest("Wildcard", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void wildcard2() {
			doTest("Wildcard2", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void wildcard3() {
			doTest("Wildcard3", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void wildcard4() {
			doTest("Wildcard4", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void primitiveTypeof() {
			doTest("PrimitiveTypeof", new Class<?>[] {}, new Object[] {}, false);
		}
		
		[Test]
		public void explicitGenericCall() {
			doTest("ExplicitGenericCall", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void assignLeftShift() {
			doTest("AssignLeftShift", new Class<?>[] {}, new Object[] {}, 16L);
		}
		
		[Test]
		public void verbatimIdentifier() {
			doTest("VerbatimIdentifier", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void delegateCast() {
			doTest("DelegateCast", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void wildcard5() {
			doTest("Wildcard5", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void genericParameterToString() {
			doTest("GenericParameterToString", new Class<?>[] {}, new Object[] {}, "STR");
		}
		
		[Test]
		public void lambdaEnvironment() {
			doTest("LambdaEnvironment", new Class<?>[] { typeof(String) }, new Object[] { "abc" }, 1);
		}
		
		[Test]
		public void inferenceEnumSet() {
			doTest("InferenceEnumSet", new Class<?>[] {}, new Object[] {}, 0);
		}
		
		[Test]
		public void autoboxingElementAssign() {
			doTest("AutoboxingElementAssign", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void anonymousObject() {
			doTest("AnonymousObject", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void anonymousObject2() {
			doTest("AnonymousObject2", new Class<?>[] {}, new Object[] {}, 20);
		}
		
		[Test]
		public void anonymousObject3() {
			doTest("AnonymousObject3", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void stringStringIntConcat() {
			doTest("StringStringIntConcat", new Class<?>[] {}, new Object[] {}, "ab1");
		}
		
		[Test]
		public void voidLambda() {
			doTest("VoidLambda", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void lambdaAsArgument() {
			doTest("LambdaAsArgument", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void lambdaAsArgument2() {
			doTest("LambdaAsArgument2", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void inferredReturnType() {
			doTest("InferredReturnType", new Class<?>[] {}, new Object[] {}, "s2");
		}
		
		[Test]
		public void delegateField() {
			doTest("DelegateField", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void anonymousObjectDelegate() {
			doTest("AnonymousObjectDelegate", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void delegateAdd2() {
			doTest("DelegateAdd2", new Class<?>[] {}, new Object[] {}, 5);
		}
		
		[Test]
		public void objectInitializer2() {
			doTest("ObjectInitializer2", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void inferenceCascaded4() {
			doTest("InferenceCascaded4", new Class<?>[] {}, new Object[] {}, null);
		}
		
		[Test]
		public void addStringBoolean() {
			doTest("AddStringBoolean", new Class<?>[] {}, new Object[] {}, "true");
		}
		
		[Test]
		public void paramsInference() {
			doTest("ParamsInference", new Class<?>[] {}, new Object[] {}, "1");
		}
		
		[Test]
		public void nestedLambdas() {
			doTest("NestedLambdas", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void lexicalDecomposition() {
			doTest("LexicalDecomposition", new Class<?>[] {}, new Object[] {}, 518);
		}
		
		[Test]
		public void lexicalDecomposition2() {
			doTest("LexicalDecomposition2", new Class<?>[] {}, new Object[] {}, 744);
		}
		
		[Test]
		public void lexicalDecomposition3() {
			doTest("LexicalDecomposition3", new Class<?>[] {}, new Object[] {}, -274);
		}
		
		[Test]
		public void lexicalDecomposition4() {
			doTest("LexicalDecomposition4", new Class<?>[] {}, new Object[] {}, 845);
		}
		
		[Test]
		public void arrayInitializerAutoboxing() {
			doTest("ArrayInitializerAutoboxing", new Class<?>[] { typeof(short) }, new Object[] { (short)3 }, 3);
		}
		
		[Test]
		public void wildcardBoundAssignment() {
			doTest("WildcardBoundAssignment", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void wildcardBoundAssignment2() {
			doTest("WildcardBoundAssignment2", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void genericConstraintAssignment() {
			doTest("GenericConstraintAssignment", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void genericConstraintAssignment2() {
			doTest("GenericConstraintAssignment2", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void objectArrayBoxing() {
			doTest("ObjectArrayBoxing", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void objectArrayBoxing2() {
			doTest("ObjectArrayBoxing2", new Class<?>[] {}, new Object[] {}, 3d);
		}
		
		[Test]
		public void objectArrayBoxing3() {
			doTest("ObjectArrayBoxing3", new Class<?>[] {}, new Object[] {}, 3d);
		}
		
		[Test]
		public void stringAdd4() {
			doTest("StringAdd4", new Class<?>[] {}, new Object[] {}, "abc");
		}
	}
}
