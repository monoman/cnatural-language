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

	public class ObjectModelTest : ExecutionTest {
		protected override String ResourcesPath {
			get {
				return "ObjectModelTest";
			}
		}
		
		[Test]
		public void staticIntField() {
			doTest("StaticIntField", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void intField() {
			doTest("IntField", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void staticMethod() {
			doTest("StaticMethod", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void qualifiedStaticCall() {
			doTest("QualifiedStaticCall", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void instanceMethod() {
			doTest("InstanceMethod", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void thisMethodCall() {
			doTest("ThisMethodCall", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void thisFieldAccess() {
			doTest("ThisFieldAccess", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void inheritance() {
			doTest("Inheritance", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void superCall() {
			doTest("SuperCall", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void shadowing() {
			doTest("Shadowing", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void interfaceCall() {
			doTest("InterfaceCall", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void propertyGet() {
			doTest("PropertyGet", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void staticPropertyGet() {
			doTest("StaticPropertyGet", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void propertySet() {
			doTest("PropertySet", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void staticPropertySet() {
			doTest("StaticPropertySet", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void propertyGetSet() {
			doTest("PropertyGetSet", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void staticPropertyGetSet() {
			doTest("StaticPropertyGetSet", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void AutomaticProperty() {
			doTest("AutomaticProperty", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void staticAutomaticProperty() {
			doTest("StaticAutomaticProperty", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void indexerGet() {
			doTest("IndexerGet", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void staticIndexerGet() {
			doTest("StaticIndexerGet", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void indexerSet() {
			doTest("IndexerSet", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void staticIndexerSet() {
			doTest("StaticIndexerSet", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void indexerGetSet() {
			doTest("IndexerGetSet", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void staticIndexerGetSet() {
			doTest("StaticIndexerGetSet", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void constructor() {
			doTest("Constructor", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void constructorArgument() {
			doTest("ConstructorArgument", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void constantMethodOverload() {
			doTest("ConstantMethodOverload", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void constantMethodOverload2() {
			doTest("ConstantMethodOverload2", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void derivedOverload() {
			doTest("DerivedOverload", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void intVarargs() {
			doTest("IntVarargs", new Class<?>[] {}, new Object[] {}, 6);
		}
		
		[Test]
		public void intVarargsNoArg() {
			doTest("IntVarargsNoArg", new Class<?>[] {}, new Object[] {}, 0);
		}
		
		[Test]
		public void interfaceCall2() {
			doTest("InterfaceCall2", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void genericClass() {
			doTest("GenericClass", new Class<?>[] { typeof(String) }, new Object[] { "STR" }, "STR");
		}
		
		[Test]
		public void delegate_0() {
			doTest("Delegate", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void delegate2() {
			doTest("Delegate2", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void delegateField() {
			doTest("DelegateField", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void delegateStatic() {
			doTest("DelegateStatic", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void delegateLambda() {
			doTest("DelegateLambda", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void delegateLambdaLocal() {
			doTest("DelegateLambdaLocal", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void delegateLambdaArgument() {
			doTest("DelegateLambdaArgument", new Class<?>[] { typeof(int) }, new Object[] { 3 }, 5);
		}
		
		[Test]
		public void delegateLambdaField() {
			doTest("DelegateLambdaField", new Class<?>[] {}, new Object[] {}, 5);
		}
		
		[Test]
		public void delegateLambdaBlock() {
			doTest("DelegateLambdaBlock", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void delegateLambdas() {
			doTest("DelegateLambdas", new Class<?>[] { typeof(int) }, new Object[] { 3 }, 7);
		}
		
		[Test]
		public void delegateNestedLambdas() {
			doTest("DelegateNestedLambdas", new Class<?>[] { typeof(int) }, new Object[] { 3 }, 7);
		}
		
		[Test]
		public void delegateNestedLambdas2() {
			doTest("DelegateNestedLambdas2", new Class<?>[] { typeof(int) }, new Object[] { 3 }, 5);
		}
		
		[Test]
		public void delegateNestedLambdas3() {
			doTest("DelegateNestedLambdas3", new Class<?>[] { typeof(int) }, new Object[] { 3 }, 5);
		}
		
		[Test]
		public void delegateNestedNestedLambdas() {
			doTest("DelegateNestedNestedLambdas", new Class<?>[] { typeof(int) }, new Object[] { 3 }, 11);
		}
		
		[Test]
		public void delegateNestedNestedLambdas2() {
			doTest("DelegateNestedNestedLambdas2", new Class<?>[] { typeof(int) }, new Object[] { 3 }, 7);
		}
		
		[Test]
		public void delegateLambdaSideEffect() {
			doTest("DelegateLambdaSideEffect", new Class<?>[] {}, new Object[] {}, 5);
		}
		
		[Test]
		public void using_0() {
			doTest("Using", new Class<?>[] { typeof(char) }, new Object[] { 'a' }, 'a');
		}
		
		[Test]
		public void usingAlias() {
			doTest("UsingAlias", new Class<?>[] { typeof(String), typeof(String) }, new Object[] { "STR1", "STR2" }, "STR1STR2");
		}
		
		[Test]
		public void usingAlias2() {
			doTest("p2.UsingAlias2", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void usingAliasGenerics() {
			doTest("UsingAliasGenerics", new Class<?>[] { typeof(String), typeof(String) }, new Object[] { "STR1", "STR2" }, "STR2");
		}
		
		[Test]
		public void packages() {
			doTest("p.Packages", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void fieldInit() {
			doTest("FieldInit", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void genericClassField() {
			doTest("p.GenericClassField", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void genericConstraint() {
			doTest("p.GenericConstraint", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void genericConstraints() {
			doTest("p.GenericConstraints", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void genericClass2() {
			doTest("GenericClass2", new Class<?>[] { typeof(String) }, new Object[] { "STR" }, "STR");
		}
		
		[Test]
		public void genericMethod() {
			doTest("GenericMethod", new Class<?>[] { typeof(String) }, new Object[] { "STR" }, "STR");
		}
		
		[Test]
		public void staticInitializer() {
			doTest("StaticInitializer", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void staticInitializer2() {
			doTest("StaticInitializer2", new Class<?>[] {}, new Object[] {}, 6);
		}
		
		[Test]
		public void nestedInterface() {
			doTest("NestedInterface", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void constructor2() {
			doTest("Constructor2", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void nestedClass() {
			doTest("NestedClass", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void nestedClass2() {
			doTest("NestedClass2", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void nestedClassAccess() {
			doTest("NestedClassAccess", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void nestedClassAccess2() {
			doTest("NestedClassAccess2", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void nestedClassMethodAccess() {
			doTest("NestedClassMethodAccess", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void nestedClassMethodAccess2() {
			doTest("NestedClassMethodAccess2", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void nestedClassConstructorAccess() {
			doTest("NestedClassConstructorAccess", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void varargs() {
			doTest("Varargs", new Class<?>[] {}, new Object[] {}, "");
		}
		
		[Test]
		public void interfaceImplementation() {
			doTest("InterfaceImplementation", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void methodOverload() {
			doTest("MethodOverload", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void methodOverload2() {
			doTest("MethodOverload2", new Class<?>[] {}, new Object[] {}, 201);
		}
		
		[Test]
		public void nestedClassBase() {
			doTest("NestedClassBase", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void nestedClassBase2() {
			doTest("NestedClassBase2", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void interfaceBase() {
			doTest("p.InterfaceBase", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void varargsOverload() {
			doTest("VarargsOverload", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void varargsOverride() {
			doTest("VarargsOverride", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void nestedClassSiblings() {
			doTest("NestedClassSiblings", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void nestedClassSiblings2() {
			doTest("NestedClassSiblings2", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void varargsNull() {
			doTest("VarargsNull", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void constructorThis() {
			doTest("ConstructorThis", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void genericBase() {
			doTest("GenericBase", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void genericBase2() {
			doTest("GenericBase2", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void genericArray() {
			doTest("GenericArray", new Class<?>[] { typeof(String) }, new Object[] { "STR" }, "STR");
		}
		
		[Test]
		public void genericArrayConstraint() {
			doTest("GenericArrayConstraint", new Class<?>[] { typeof(String) }, new Object[] { "STR" }, "STR");
		}
		
		[Test]
		public void genericNestedClass() {
			doTest("GenericNestedClass", new Class<?>[] { typeof(String) }, new Object[] { "STR" }, "STR");
		}
		
		[Test]
		public void genericStaticField() {
			doTest("GenericStaticField", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void shortByteOverload() {
			doTest("ShortByteOverload", new Class<?>[] {}, new Object[] {}, 1);
		}

		[Test]
		public void varargsOverload2() {
			doTest("VarargsOverload2", new Class<?>[] {}, new Object[] {}, 1);
		}

		[Test]
		public void nestedClassOverload() {
			doTest("NestedClassOverload", new Class<?>[] {}, new Object[] {}, 2);
		}

		[Test]
		public void nestedClassOverload2() {
			doTest("NestedClassOverload2", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void genericOverload() {
			doTest("GenericOverload", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void packages2() {
			doTest("Packages2", new Class<?>[] {}, new Object[] {}, 3);
		}

		[Test]
		public void nestedClassModification() {
			doTest("NestedClassModification", new Class<?>[] {}, new Object[] {}, 3);
		}

		[Test]
		public void nestedClassModification2() {
			doTest("NestedClassModification2", new Class<?>[] {}, new Object[] {}, true);
		}

		[Test]
		public void varargsArray() {
			doTest("VarargsArray", new Class<?>[] {}, new Object[] {}, "abc");
		}

		[Test]
		public void destructor() {
			doTest("Destructor", new Class<?>[] {}, new Object[] {}, null);
		}

		[Test]
		public void arrayProperty() {
			doTest("ArrayProperty", new Class<?>[] {}, new Object[] {}, 9);
		}

		[Test]
		public void staticShortField() {
			doTest("StaticShortField", new Class<?>[] {}, new Object[] {}, (short)1);
		}

		[Test]
		public void staticLongField() {
			doTest("StaticLongField", new Class<?>[] {}, new Object[] {}, 1L);
		}

		[Test]
		public void longField() {
			doTest("LongField", new Class<?>[] {}, new Object[] {}, 1L);
		}

		[Test]
		public void superField() {
			doTest("SuperField", new Class<?>[] {}, new Object[] {}, 2);
		}

		[Test]
		public void virtualCall() {
			doTest("VirtualCall", new Class<?>[] {}, new Object[] {}, true);
		}

		[Test]
		public void packageAliasing() {
			doTest("PackageAliasing", new Class<?>[] {}, new Object[] {}, "OK");
		}

		[Test]
		public void extensionMethod() {
			doTest("ExtensionMethod", new Class<?>[] {}, new Object[] {}, 3);
		}

		[Test]
		public void indexer() {
			doTest("Indexer", new Class<?>[] {}, new Object[] {}, 3);
		}

		[Test]
		public void indexerStatic() {
			doTest("IndexerStatic", new Class<?>[] {}, new Object[] {}, 5);
		}

		[Test]
		public void indexerKeys() {
			doTest("IndexerKeys", new Class<?>[] {}, new Object[] {}, "ab2.0c");
		}

		[Test]
		public void bridge() {
			doTest("Bridge", new Class<?>[] {}, new Object[] {}, 3);
		}

		[Test]
		public void indexerInterface() {
			doTest("IndexerInterface", new Class<?>[] {}, new Object[] {}, 3);
		}

		[Test]
		public void PropertyInterface() {
			doTest("PropertyInterface", new Class<?>[] {}, new Object[] {}, 3);
		}

		[Test]
		public void fields() {
			doTest("Fields", new Class<?>[] {}, new Object[] {}, 6);
		}

		[Test]
		public void covariance() {
			doTest("Covariance", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void genericConstraints2() {
			doTest("GenericConstraints2", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void staticAccess() {
			doTest("StaticAccess", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void genericNestedInterface() {
			doTest("GenericNestedInterface", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void inheritedMethodCall() {
			doTest("InheritedMethodCall", new Class<?>[] {}, new Object[] {}, "AB");
		}
		
		[Test]
		public void abstractProperty() {
			doTest("AbstractProperty", new Class<?>[] {}, new Object[] {}, "STR");
		}
		
		[Test]
		public void packageProtectedAccess() {
			doTest("PackageProtectedAccess", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void classAnnotation() {
			doTest("ClassAnnotation", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void methodAnnotation() {
			doTest("MethodAnnotation", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void fieldAnnotation() {
			doTest("FieldAnnotation", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void methodAnnotation2() {
			doTest("MethodAnnotation2", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void annotationType() {
			doTest("AnnotationType", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void annotationArray() {
			doTest("AnnotationArray", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void annotationEnum() {
			doTest("AnnotationEnum", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void annotationArray2() {
			doTest("AnnotationArray2", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void indexerVarargs() {
			doTest("IndexerVarargs", new Class<?>[] {}, new Object[] {}, 6);
		}
		
		[Test]
		public void longConstField() {
			doTest("LongConstField", new Class<?>[] {}, new Object[] {}, 2L);
		}
		
		[Test]
		public void floatConstField() {
			doTest("FloatConstField", new Class<?>[] {}, new Object[] {}, 2f);
		}
		
		[Test]
		public void doubleConstField() {
			doTest("DoubleConstField", new Class<?>[] {}, new Object[] {}, 2d);
		}
		
		[Test]
		public void byteConstField() {
			doTest("ByteConstField", new Class<?>[] {}, new Object[] {}, (byte)2);
		}

		[Test]
		public void parameterAnnotation() {
			doTest("ParameterAnnotation", new Class<?>[] {}, new Object[] {}, true);
		}

		[Test]
		public void parameterAnnotations() {
			doTest("ParameterAnnotations", new Class<?>[] {}, new Object[] {}, true);
		}

		[Test]
		public void parameterAnnotationConstructor() {
			doTest("ParameterAnnotationConstructor", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void genericNestedClass2() {
			doTest("GenericNestedClass2", new Class<?>[] {}, new Object[] {}, "OK");
		}
		
		[Test]
		public void genericField() {
			doTest("GenericField", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void genericProperty() {
			doTest("GenericProperty", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void genericMethod2() {
			doTest("GenericMethod2", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void staticFinalLongField() {
			doTest("StaticFinalLongField", new Class<?>[] {}, new Object[] {}, 0L);
		}
		
		[Test]
		public void genericAssignment() {
			doTest("GenericAssignment", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void wildcardArgument() {
			doTest("WildcardArgument", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void wildcardArrayArgument() {
			doTest("WildcardArrayArgument", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void wildcardConstraintArgument() {
			doTest("WildcardConstraintArgument", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void wildcardLowerConstraintArgument() {
			doTest("WildcardLowerConstraintArgument", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void enumDeclaration() {
			doTest("EnumDeclaration", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void autoPropertyNestedAccess() {
			doTest("AutoPropertyNestedAccess", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void wildcardBoundInheritance() {
			doTest("WildcardBoundInheritance", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void wildcardBoundInheritance2() {
			doTest("WildcardBoundInheritance2", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void genericConstraintInheritance() {
			doTest("GenericConstraintInheritance", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void genericMethodImplementation() {
			doTest("GenericMethodImplementation", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void genericMethodImplementation2() {
			doTest("GenericMethodImplementation2", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void genericBridgeBoxing() {
			doTest("GenericBridgeBoxing", new Class<?>[] {}, new Object[] {}, 1);
		}
		
		[Test]
		public void privateOuterField() {
			doTest("p.q.PrivateOuterField", new Class<?>[] {}, new Object[] {}, 2);
		}
		
		[Test]
		public void wildcardBoundParameter() {
			doTest("WildcardBoundParameter", new Class<?>[] {}, new Object[] {}, true);
		}
		
		[Test]
		public void delegateLambdaAnonymous() {
			doTest("DelegateLambdaAnonymous", new Class<?>[] {}, new Object[] {}, 7);
		}
		
		[Test]
		public void lambdaCatchVariable() {
			doTest("LambdaCatchVariable", new Class<?>[] {}, new Object[] {}, "Message");
		}
		
		[Test]
		public void lambdaForeachVariable() {
			doTest("LambdaForeachVariable", new Class<?>[] {}, new Object[] {}, "abc");
		}
		
		[Test]
		public void lambdaStatic() {
			doTest("LambdaStatic", new Class<?>[] {}, new Object[] {}, 3);
		}
		
		[Test]
		public void lambdaInConstructor() {
			doTest("LambdaInConstructor", new Class<?>[] {}, new Object[] {}, 3);
		}
    }
}
