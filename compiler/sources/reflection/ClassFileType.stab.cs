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
using java.lang.reflect;
using java.io;
using java.util;
using java.util.zip;
using org.objectweb.asm;
using stab.query;
using stab.tools.bytecode.signature;
using stab.tools.helpers;

using AsmType = org.objectweb.asm.Type;

namespace stab.reflection {

    class ClassFileType : ClassType {
        private static HashMap<String, NumericTypeKind> TypeKinds;
        static ClassFileType() {
            TypeKinds = new HashMap<String, NumericTypeKind>();
            TypeKinds["java/lang/Byte"] = NumericTypeKind.Byte;
            TypeKinds["java/lang/Short"] = NumericTypeKind.Short;
            TypeKinds["java/lang/Character"] = NumericTypeKind.Char;
            TypeKinds["java/lang/Integer"] = NumericTypeKind.Int;
            TypeKinds["java/lang/Long"] = NumericTypeKind.Long;
            TypeKinds["java/lang/Float"] = NumericTypeKind.Float;
            TypeKinds["java/lang/Double"] = NumericTypeKind.Double;
        }
    
        private Library typeSystem;
        private int modifiers;
        private int nestedModifiers;
        private String descriptor;
        private NumericTypeKind numericTypeKind;
        private List<AnnotationValue> annotations;
        private RawClassInfo classInfo;
        private Scope<String, TypeInfo> genericsScope;
        
        MethodInfo boxingMethod;
        MethodInfo unboxingMethod;

        ClassFileType(Library typeSystem, String name, byte[] bytes)
            	: super(typeSystem, name) {
            this.typeSystem = typeSystem;
            this.descriptor = "L" + name + ";";
            numericTypeKind = TypeKinds[name];
            if (numericTypeKind == null) {
                numericTypeKind = NumericTypeKind.None;
            }
            new ClassReader(bytes).accept(new OutlineVisitor(this), ClassReader.SKIP_CODE | ClassReader.SKIP_DEBUG | ClassReader.SKIP_FRAMES);
            
            this.genericsScope = new Scope<String, TypeInfo>();
            this.genericsScope.enterScope();
        }

        public override String Descriptor {
            get {
                return descriptor;
            }
        }
        
        public override NumericTypeKind NumericTypeKind {
            get {
                return numericTypeKind;
            }
        }
        
        public override Iterable<AnnotationValue> Annotations {
            get {
                initializeBaseTypes();
                return annotations;
            }
        }

        protected override int Modifiers {
            get {
                return modifiers;
            }
        }
        
        protected override int NestedModifiers {
            get {
                return nestedModifiers;
            }
        }
        
		private static bool hasGenericArguments(TypeInfo type) {
			if (type instanceof ClassFileType) {
				var classInfo = ((ClassFileType)type).classInfo;
				if (classInfo != null && classInfo.name != null) {
					return classInfo.signature != null && classInfo.signature.FormalTypeParameters.any();
				}
			}
			return type.GenericArguments.any();
		}
		
        protected override void initializeBaseTypes() {
            if (classInfo == null || classInfo.name == null) {
                return;
            }
            classInfo.name = null;

            ReflectionHelper.declareEnclosingGenericParameters(genericsScope, this);
            if (classInfo.signature == null) {
                genericArguments = Query.empty();
                if (classInfo.superName != null) {
                    baseType = typeSystem.getType(classInfo.superName);
					if (hasGenericArguments(baseType)) {
                        baseType = baseType.RawType;
                    }
                    if (classInfo.interfaces != null) {
                        var ints = new ArrayList<TypeInfo>(sizeof(classInfo.interfaces));
                        foreach (var s in classInfo.interfaces) {
                            var t = typeSystem.getType(s);
							if (hasGenericArguments(t)) {
                                t = t.RawType;
                            }
                            ints.add(t);
                        }
                        interfaces = ints;
                    } else {
                        interfaces = Query.empty();
                    }
                } else {
                    interfaces = Query.empty();
                }
            } else {
                if (classInfo.signature.FormalTypeParameters.any()) {
                    var genArgs = new ArrayList<TypeInfo>();
                    foreach (var t in classInfo.signature.FormalTypeParameters) {
                        var tp = new GenericParameterType(this.Library, t.Name, null);
                        genArgs.add(tp);
                        genericsScope.declareBinding(t.Name, tp);
                    }
                    genericArguments = genArgs;
                    int i = 0;
                    foreach (var t in classInfo.signature.FormalTypeParameters) {
                        if (t.FormalTypeParameterBounds.any()) {
                            foreach (var ts in t.FormalTypeParameterBounds) {
                                ((GenericParameterType)genArgs[i]).genericParameterBounds.add(getTypeInfo(ts));
                            }
                        }
                        i++;
                    }
                } else {
                    genericArguments = Query.empty();
                }
                if (classInfo.signature.Superclass != null) {
                    baseType = getTypeInfo(classInfo.signature.Superclass);
                }
                int n = classInfo.signature.Superinterfaces.count();
                if (n > 0) {
                    var ints = new ArrayList<TypeInfo>(n);
                    foreach (var t in classInfo.signature.Superinterfaces) {
                        ints.add(getTypeInfo(t));
                    }
                    interfaces = ints;
                } else {
                    interfaces = Query.empty();
                }
                classInfo.signature = null;
            }

            if (classInfo.annotations == null) {
                this.annotations = Collections.emptyList();
            } else {
                this.annotations = buildAnnotationValues(classInfo.annotations);
            }
        }
        
        protected override void initializeFields() {
            if (classInfo == null || classInfo.fields == null) {
                return;
            }
            initializeBaseTypes();

            foreach (var f in classInfo.fields) {
                var field = new FieldInfo(this, f.modifiers, f.name, f.value);
				try {
					if (f.signature == null) {
						field.type = getTypeInfo(f.type);
					} else {
							field.type = getTypeInfo(f.signature);
					}
					if (f.annotations == null) {
						field.annotations = Collections.emptyList();
					} else {
						field.annotations = buildAnnotationValues(f.annotations);
					}
				} catch (Exception e) {
					System.out.print("Error on " + field);
					System.out.println((f.signature == null) ? (" by type " + f.type) : (" by signature " + f.signature.Name) );
					continue;
				}
                fields.add(field);
            }
            
            classInfo.fields = null;
            if (classInfo.methods == null) {
                classInfo = null;
                genericsScope = null;
            }
        }
        
        protected override void initializeMethods() {
            if (classInfo == null || classInfo.methods == null) {
                return;
            }
            initializeBaseTypes();
            
            foreach (var m in classInfo.methods) {
                var method = new ClassFileMethod(this, m.name, m.descriptor);
                method.modifiers = m.modifiers;
                methods.add(method);
                if (m.signature == null) {
                    method.returnType = getTypeInfo(m.returnType);
                    int i = 0;
                    foreach (var p in m.parameters) {
                        var pi = new ParameterInfo(i++, getTypeInfo(p.type));
                        method.parameters.add(pi);
                        if (p.annotations == null) {
                            pi.annotations = Collections.emptyList();
                        } else {
                            pi.annotations = buildAnnotationValues(p.annotations);
                        }
                    }
                    if (m.exceptions == null) {
                        method.exceptions = Collections.emptyList();
                    } else {
                        var ex = new ArrayList<TypeInfo>();
                        foreach (var s in  m.exceptions) {
                            var t = typeSystem.getType(s);
							if (hasGenericArguments(t)) {
                                t = t.RawType;
                            }
                            ex.add(t);
                        }
                        method.exceptions = ex;
                    }
                    method.genericArguments = Collections.emptyList();
                } else {
                    genericsScope.enterScope();
                    var genArgs = new ArrayList<TypeInfo>();
                    foreach (var t in m.signature.FormalTypeParameters) {
                        var tp = new GenericParameterType(this.Library, t.Name, null);
                        genArgs.add(tp);
                        genericsScope.declareBinding(t.Name, tp);
                    }
                    method.genericArguments = genArgs;
                    int i = 0;
                    foreach (var t in m.signature.getFormalTypeParameters()) {
                        if (t.FormalTypeParameterBounds.any()) {
                            foreach (var ts in t.FormalTypeParameterBounds) {
                                ((GenericParameterType)genArgs[i]).genericParameterBounds.add(getTypeInfo(ts));
                            }
                        }
                        i++;
                    }
                    method.returnType = getTypeInfo(m.signature.ReturnType);
                    i = 0;
                    foreach (var param in m.signature.Parameters) {
                        var annotations = m.parameters[i].annotations;
                        var pi = new ParameterInfo(i++, getTypeInfo(param));
                        method.parameters.add(pi);
                        if (annotations == null) {
                            pi.annotations = Collections.emptyList();
                        } else {
                            pi.annotations = buildAnnotationValues(annotations);
                        }
                    }
                    var excepts = new ArrayList<TypeInfo>();
                    foreach (var t in m.signature.Exceptions) {
                        excepts.add(getTypeInfo(t));
                    }
                    method.exceptions = excepts;
                    genericsScope.leaveScope();
                }
                if (m.defaultValue != null) {
                    method.defaultValue = buildAnnotationArrayElement(m.defaultValue);
                }
                if (m.annotations == null) {
                    method.annotations = Collections.emptyList();
                } else {
                    method.annotations = buildAnnotationValues(m.annotations);
                }
            }
            
            classInfo.methods = null;
            if (classInfo.fields == null) {
                classInfo = null;
                genericsScope = null;
            }
        }
        
        protected override void initializeNestedTypes() {
        }
        
        private ArrayList<AnnotationValue> buildAnnotationValues(ArrayList<RawAnnotationValue> annotations) {
            var result = new ArrayList<AnnotationValue>();
            foreach (var av in annotations) {
                var b = new AnnotationValueBuilder(getTypeInfo(av.Type), av.IsRuntimeVisible);
                buildAnnotationArgument(b, av);
                result.add(b);
            }
            return result;
        }
        
        private void buildAnnotationArgument(AnnotationValueBuilder b, RawAnnotationArgument RawAnnotation) {
            foreach (var s in RawAnnotation.ArgumentNames) {
                var arg = RawAnnotation.getArgument(s);
                switch (arg.AnnotationArgumentKind) {
                case Boolean:
                    b.setBooleanArgument(s, (Boolean)arg.Value);
                    break;
                case Byte:
                    b.setByteArgument(s, (Byte)arg.Value);
                    break;
                case Char:
                    b.setCharArgument(s, (Character)arg.Value);
                    break;
                case Short:
                    b.setShortArgument(s, (Short)arg.Value);
                    break;
                case Int:
                    b.setIntArgument(s, (Integer)arg.Value);
                    break;
                case Long:
                    b.setLongArgument(s, (Long)arg.Value);
                    break;
                case Float:
                    b.setFloatArgument(s, (Float)arg.Value);
                    break;
                case Double:
                    b.setDoubleArgument(s, (Double)arg.Value);
                    break;
                case String:
                    b.setStringArgument(s, (String)arg.Value);
                    break;
                case Enum:
                    b.setEnumArgument(s, getTypeInfo(arg.Type), arg.Name);
                    break;
                case Type:
                    b.setTypeArgument(s, getTypeInfo(arg.Type));
                    break;
                case Array:
                    var ab = b.setArrayArgument(s);
                    foreach (var e in arg.Elements) {
                        buildAnnotationArrayElement(ab, e);
                    }
                    break;
                default:
                    var vb = b.setAnnotationArgument(s, getTypeInfo(arg.Type), arg.IsRuntimeVisible);
                    buildAnnotationArgument(vb, arg);
                    break;
                }
            }
        }

        private AnnotationArgument buildAnnotationArrayElement(RawAnnotationArgument arg) {
            switch (arg.AnnotationArgumentKind) {
            case Boolean:
            case Byte:
            case Char:
            case Short:
            case Int:
            case Long:
            case Float:
            case Double:
            case String:
                return new SimpleAnnotationArgument(arg.AnnotationArgumentKind, arg.Value);
            case Enum:
                return new EnumAnnotationArgument(getTypeInfo(arg.Type), arg.Name);
            case Type:
                return new TypeAnnotationArgument(getTypeInfo(arg.Type));
            case Array:
                var ab = new AnnotationArrayValueBuilder();
                foreach (var e in arg.Elements) {
                    buildAnnotationArrayElement(ab, e);
                }
                return ab;
            default:
                var vb = new AnnotationValueBuilder(getTypeInfo(arg.Type), arg.IsRuntimeVisible);
                buildAnnotationArgument(vb, arg);
                return vb;
            }
        }

        private void buildAnnotationArrayElement(AnnotationArrayValueBuilder b, RawAnnotationArgument arg) {
            switch (arg.AnnotationArgumentKind) {
            case Boolean:
                b.addBooleanArgument((Boolean)arg.Value);
                break;
            case Byte:
                b.addByteArgument((Byte)arg.Value);
                break;
            case Char:
                b.addCharArgument((Character)arg.Value);
                break;
            case Short:
                b.addShortArgument((Short)arg.Value);
                break;
            case Int:
                b.addIntArgument((Integer)arg.Value);
                break;
            case Long:
                b.addLongArgument((Long)arg.Value);
                break;
            case Float:
                b.addFloatArgument((Float)arg.Value);
                break;
            case Double:
                b.addDoubleArgument((Double)arg.Value);
                break;
            case String:
                b.addStringArgument((String)arg.Value);
                break;
            case Enum:
                b.addEnumArgument(getTypeInfo(arg.Type), arg.Name);
                break;
            case Type:
                b.addTypeArgument(getTypeInfo(arg.Type));
                break;
            case Array:
                var ab = b.addArrayArgument();
                foreach (var e in arg.Elements) {
                    buildAnnotationArrayElement(ab, e);
                }
                break;
            default:
                var vb = b.addAnnotationArgument(getTypeInfo(arg.Type), arg.IsRuntimeVisible);
                buildAnnotationArgument(vb, arg);
                break;
            }
        }

        private TypeInfo getTypeInfo(AsmType type) {
            switch (type.getSort()) {
            case AsmType.ARRAY:
                var elt = getTypeInfo(type.getElementType());
                for (int i = type.getDimensions(); i > 0; --i) {
                    elt = elt.ArrayType;
                }
                return elt;
                
            case AsmType.BOOLEAN:
                return this.Library.BooleanType;
                
            case AsmType.BYTE:
                return this.Library.ByteType;

            case AsmType.CHAR:
                return this.Library.CharType;
                
            case AsmType.DOUBLE:
                return this.Library.DoubleType;
                
            case AsmType.FLOAT:
                return this.Library.FloatType;
                
            case AsmType.INT:
                return this.Library.IntType;
                
            case AsmType.LONG:
                return this.Library.LongType;
                
            case AsmType.SHORT:
                return this.Library.ShortType;
                
            case AsmType.VOID:
                return this.Library.VoidType;

            default:
                var result = typeSystem.getType(type.getInternalName());
				if (hasGenericArguments(result)) {
					result = result.RawType;
				}
                return result;
            }
        }
        
        static RawAnnotationArgument buildAnnotationArgument(Object value) {
            if (value instanceof Integer) {
                return new RawSimpleValue(AnnotationArgumentKind.Int, value);
            } else if (value instanceof String) {
                return new RawSimpleValue(AnnotationArgumentKind.String, value);
            } else if (value instanceof Boolean) {
                return new RawSimpleValue(AnnotationArgumentKind.Boolean, value);
            } else if (value instanceof AsmType) {
                return new RawTypeValue((AsmType)value);
            } else if (value instanceof Character) {
                return new RawSimpleValue(AnnotationArgumentKind.Char, value);
            } else if (value instanceof Short) {
                return new RawSimpleValue(AnnotationArgumentKind.Short, value);
            } else if (value instanceof Long) {
                return new RawSimpleValue(AnnotationArgumentKind.Long, value);
            } else if (value instanceof Float) {
                return new RawSimpleValue(AnnotationArgumentKind.Float, value);
            } else if (value instanceof Double) {
                return new RawSimpleValue(AnnotationArgumentKind.Double, value);
            } else {
                var array = new RawArrayValue();
                int len = Array.getLength(value);
                for (int i = 0; i < len; i++) {
                    array.elements.add(buildAnnotationArgument(Array.get(value, i)));
                }
                return array;
            }
        }
        
        private TypeInfo getTypeInfo(TypeSignature signature) {
            TypeInfo result;
            switch (signature.TypeSignatureKind) {
            case ClassType:
                var name = signature.Name.substring(1, signature.Name.length() - 1);
                result = typeSystem.getType(name);
                if (signature.TypeArguments.any()) {
                    var typeArgs = new ArrayList<TypeInfo>();
                    foreach (var sig in signature.TypeArguments) {
						typeArgs.add(getTypeInfo(sig));						
                    }
                    result = typeSystem.getGenericTypeCore(result, typeArgs, null);
                }
                break;

            case ArrayType:
                result = getTypeInfo(signature.ElementType).ArrayType;
                break;

            case Boolean:
                result = this.Library.BooleanType;
                break;
                
            case Byte:
                result = this.Library.ByteType;
                break;
                
            case Char:
                result = this.Library.CharType;
                break;
                
            case Double:
                result = this.Library.DoubleType;
                break;
                
            case Float:
                result = this.Library.FloatType;
                break;
                
            case Int:
                result = this.Library.IntType;
                break;
                
            case Long:
                result = this.Library.LongType;
                break;
                
            case Short:
                result = this.Library.ShortType;
                break;
                
            case Void:
                result = this.Library.VoidType;
                break;
                
            case UnboundedWildcardType:
                result = this.Library.UnboundedWildcard;
                break;
                
            case UpperBoundedWildcardType:
                result = getTypeInfo(signature.WildcardBound).UpperBoundedWildcard;
                break;
                
            case LowerBoundedWildcardType:
                result = getTypeInfo(signature.WildcardBound).LowerBoundedWildcard;
                break;

            case TypeVariable:
				result = genericsScope.getBindingValue(signature.Name);
                break;
                
            default:
                throw new RuntimeException("Unexpected type signature: " + signature.Name);
            }
            return result;
        }
        
        private class OutlineVisitor : ClassVisitor {
            private ClassFileType classFileType;
            private FieldInfoBuilder fieldInfoBuilder;
            private MethodInfoBuilder methodInfoBuilder;

            OutlineVisitor(ClassFileType classFileType) {
                this.classFileType = classFileType;
                this.fieldInfoBuilder = new FieldInfoBuilder();
                this.methodInfoBuilder = new MethodInfoBuilder();
            }
        
            public void visit(int version, int access, String name, String signature, String superName, String[] interfaces) {
                classFileType.modifiers = access;
                var classInfo = new RawClassInfo();
                classFileType.classInfo = classInfo;
                classInfo.name = name;
                if (signature == null) {
                    classInfo.superName = superName;
                    classInfo.interfaces = interfaces;
                } else {
                    var parser = new SignatureParser(signature);
                    classInfo.signature = parser.parseClassSignature();
                }
            }
            
            public AnnotationVisitor visitAnnotation(String desc, bool visible) {
                var annotation = new RawAnnotationValue(AsmType.getType(desc), visible);
                if (classFileType.classInfo.annotations == null) {
                	classFileType.classInfo.annotations = new ArrayList<RawAnnotationValue>();
                }
                classFileType.classInfo.annotations.add(annotation);
                return new RawAnnotationValueBuilder(annotation);
            }
            
            public FieldVisitor visitField(int access, String name, String desc, String signature, Object value) {
                var field = new RawFieldInfo();
                field.modifiers = access;
                field.name = name;
                field.value = value;
                classFileType.classInfo.fields.add(field);
                if (signature == null) {
                    field.type = AsmType.getType(desc);
                } else {
                    var parser = new SignatureParser(signature);
                    field.signature = parser.parseFieldTypeSignature();
                }
                fieldInfoBuilder.field = field;
                return fieldInfoBuilder;
            }
            
            public MethodVisitor visitMethod(int access, String name, String desc, String signature, String[] exceptions) {
                var method = new RawMethodInfo();
                method.modifiers = access;
                method.name = name;
                method.descriptor = desc;
                classFileType.classInfo.methods.add(method);
                
                if (signature == null) {
                    method.returnType = AsmType.getReturnType(desc);
                    foreach (var t in AsmType.getArgumentTypes(desc)) {
                        var p = new RawParameterInfo();
                        p.type = t;
                        method.parameters.add(p);
                    }
                    method.exceptions = exceptions;
                } else {
                    var parser = new SignatureParser(signature);
                    method.signature = parser.parseMethodTypeSignature();
                    foreach (var s in method.signature.Parameters) {
                        method.parameters.add(new RawParameterInfo());
                    }
                }
                
                methodInfoBuilder.method = method;
                return methodInfoBuilder;
            }
            
            public void visitInnerClass(String name, String outerName, String innerName, int access) {
                if (!name.equals(classFileType.FullName)) {
                    // Inner classes with outerName == null are method classes: ignore them.
                    if (outerName != null && outerName.equals(classFileType.FullName)) {
                        var nestedType = (ClassFileType)classFileType.typeSystem.getType(name);
                        nestedType.nestedModifiers = access;
                        nestedType.declaringType = classFileType;
                        classFileType.nestedTypes.add(nestedType);
                    }
                }
            }
            
            public void visitEnd() {}
            public void visitAttribute(Attribute attr) {}
            public void visitOuterClass(String owner, String name, String desc) {}
            public void visitSource(String source, String debug) {}
        }
        
        private class RawClassInfo {
            String name;
            String superName;
            String[] interfaces;
            ClassSignature signature;
            ArrayList<RawAnnotationValue> annotations;
            ArrayList<RawFieldInfo> fields = new ArrayList<RawFieldInfo>();
            ArrayList<RawMethodInfo> methods = new ArrayList<RawMethodInfo>();
        }

        private class RawFieldInfo {
            ArrayList<RawAnnotationValue> annotations;
            int modifiers;
            String name;
            Object value;
            AsmType type;
            TypeSignature signature;
        }

        private class RawMethodInfo {
            ArrayList<RawAnnotationValue> annotations;
            ArrayList<RawParameterInfo> parameters = new ArrayList<RawParameterInfo>();
            RawAnnotationArgument defaultValue;
            int modifiers;
            String name;
            String descriptor;
            AsmType returnType;
            String[] exceptions;
            MethodTypeSignature signature;
        }

        private class RawParameterInfo {
            ArrayList<RawAnnotationValue> annotations;
            AsmType type;
        }

        private class FieldInfoBuilder : FieldVisitor {
            RawFieldInfo field;

            public AnnotationVisitor visitAnnotation(String desc, bool visible) {
                var annotation = new RawAnnotationValue(AsmType.getType(desc), visible);
                if (field.annotations == null) {
                	field.annotations = new ArrayList<RawAnnotationValue>();
                }
                field.annotations.add(annotation);
                return new RawAnnotationValueBuilder(annotation);
            }

            public void visitAttribute(Attribute attr) {}
            public void visitEnd() {}
        }
        
        private class MethodInfoBuilder : MethodVisitor {
            RawMethodInfo method;
            
            public AnnotationVisitor visitAnnotation(String desc, bool visible) {
                var annotation = new RawAnnotationValue(AsmType.getType(desc), visible);
                if (method.annotations == null) {
                	method.annotations = new ArrayList<RawAnnotationValue>();
                }
                method.annotations.add(annotation);
                return new RawAnnotationValueBuilder(annotation);
            }

            public AnnotationVisitor visitAnnotationDefault() {
                return new RawAnnotationDefaultValueBuilder(method);
            }

            public AnnotationVisitor visitParameterAnnotation(int parameter, String desc, bool visible) {
                var annotation = new RawAnnotationValue(AsmType.getType(desc), visible);
                var par = method.parameters.get(parameter);
                if (par.annotations == null) {
                	par.annotations = new ArrayList<RawAnnotationValue>();
                }
                par.annotations.add(annotation);
                return new RawAnnotationValueBuilder(annotation);
            }
            
            public void visitAttribute(Attribute attr) {}
            public void visitCode() {}
            public void visitEnd() {}
            public void visitFieldInsn(int opcode, String owner, String name, String desc) {}
            public void visitFrame(int type, int nLocal, Object[] local, int nStack, Object[] stack) {}
            public void visitIincInsn(int var, int increment) {}
            public void visitInsn(int opcode) {}
            public void visitIntInsn(int opcode, int operand) {}
            public void visitJumpInsn(int opcode, Label label) {}
            public void visitLabel(Label label) {}
            public void visitLdcInsn(Object cst) {}
            public void visitLineNumber(int line, Label start) {}
            public void visitLocalVariable(String name, String desc, String signature, Label start, Label end, int index) {}
            public void visitLookupSwitchInsn(Label dflt, int[] keys, Label[] labels) {}
            public void visitMaxs(int maxStack, int maxLocals) {}
            public void visitMethodInsn(int opcode, String owner, String name, String desc) {}
            public void visitMultiANewArrayInsn(String desc, int dims) {}
            public void visitTableSwitchInsn(int min, int max, Label dflt, Label[] labels) {}
            public void visitTryCatchBlock(Label start, Label end, Label handler, String type) {}
            public void visitTypeInsn(int opcode, String type) {}
            public void visitVarInsn(int opcode, int var) {}
        }
        
        private class ArrayVisitor : AnnotationVisitor {
            private RawArrayValue array;
            
            ArrayVisitor(RawArrayValue array) {
                this.array = array;
            }

            public void visit(String name, Object value) {
                array.elements.add(buildAnnotationArgument(value));
            }

            public AnnotationVisitor visitAnnotation(String name, String desc) {
                var annotation = new RawAnnotationValue(AsmType.getType(desc), false);
                array.elements.add(annotation);
                return new RawAnnotationValueBuilder(annotation);
            }

            public AnnotationVisitor visitArray(String name) {
                var array = new RawArrayValue();
                this.array.elements.add(array);
                return new ArrayVisitor(array);
            }

            public void visitEnum(String name, String desc, String value) {
                array.elements.add(new RawEnumValue(AsmType.getType(desc), value));
            }

            public void visitEnd() {}
        }

        private class RawAnnotationValueBuilder : AnnotationVisitor {
            private RawAnnotationValue annotation;
            
            RawAnnotationValueBuilder(RawAnnotationValue annotation) {
                this.annotation = annotation;
            }
            
            public void visit(String name, Object value) {
                annotation.arguments.put(name, buildAnnotationArgument(value));
            }

            public AnnotationVisitor visitAnnotation(String name, String desc) {
                var annotation = new RawAnnotationValue(AsmType.getType(desc), false);
                this.annotation.arguments.put(name, annotation);
                return new RawAnnotationValueBuilder(annotation);
            }

            public AnnotationVisitor visitArray(String name) {
                var array = new RawArrayValue();
                annotation.arguments.put(name, array);
                return new ArrayVisitor(array);
            }

            public void visitEnum(String name, String desc, String value) {
                annotation.arguments.put(name, new RawEnumValue(AsmType.getType(desc), value));
            }

            public void visitEnd() {}
        }

        private class RawAnnotationDefaultValueBuilder : AnnotationVisitor {
            private RawMethodInfo method;
            
            public RawAnnotationDefaultValueBuilder(RawMethodInfo method) {
                this.method = method;
            }

            public void visit(String name, Object value) {
                this.method.defaultValue = buildAnnotationArgument(value);
            }

            public AnnotationVisitor visitAnnotation(String name, String desc) {
                var annotation = new RawAnnotationValue(AsmType.getType(desc), false);
                this.method.defaultValue = annotation;
                return new RawAnnotationValueBuilder(annotation);
            }

            public AnnotationVisitor visitArray(String name) {
                var array = new RawArrayValue();
                this.method.defaultValue = array;
                return new ArrayVisitor(array);
            }

            public void visitEnum(String name, String desc, String value) {
                this.method.defaultValue = new RawEnumValue(AsmType.getType(desc), value);
            }

            public void visitEnd() {}
        }

        private abstract class RawAnnotationArgument {
            protected RawAnnotationArgument(AnnotationArgumentKind RawAnnotationArgumentKind) {
                this.AnnotationArgumentKind = RawAnnotationArgumentKind;
            }
            
            public AnnotationArgumentKind AnnotationArgumentKind^;
            
            public virtual Iterable<String> ArgumentNames {
                get {
                    throw new UnsupportedOperationException();
                }
            }
            
            public virtual RawAnnotationArgument getArgument(String name) {
                throw new UnsupportedOperationException();
            }
            
            public virtual Object Value {
                get {
                    throw new UnsupportedOperationException();
                }
            }
            
            public virtual Iterable<RawAnnotationArgument> Elements {
                get {
                    throw new UnsupportedOperationException();
                }
            }
            
            public virtual bool IsRuntimeVisible {
                get {
                    throw new UnsupportedOperationException();
                }
            }

            public virtual AsmType Type {
                get {
                    throw new UnsupportedOperationException();
                }
            }
            
            public virtual String Name {
                get {
                    throw new UnsupportedOperationException();
                }
            }
        }

        private class RawSimpleValue : RawAnnotationArgument {
            private Object value;
            
            RawSimpleValue(AnnotationArgumentKind annotationArgumentKind, Object value)
                : super(annotationArgumentKind) {
                this.value = value;
            }

            public override Object Value {
                get {
                    return value;
                }
            }
        }
        
        private class RawArrayValue : RawAnnotationArgument {
            ArrayList<RawAnnotationArgument> elements;
            
            RawArrayValue()
                : super(AnnotationArgumentKind.Array) {
                elements = new ArrayList<RawAnnotationArgument>();
            }
            
            public override Iterable<RawAnnotationArgument> Elements {
                get {
                    return elements;
                }
            }
        }
        
        private class RawEnumValue : RawAnnotationArgument {
            private AsmType type;
            private String name;
            
            RawEnumValue(AsmType type, String name)
                : super(AnnotationArgumentKind.Enum) {
                this.type = type;
                this.name = name;
            }
            
            public override AsmType Type {
                get {
                    return type;
                }
            }
            
            public override String Name {
                get {
                    return name;
                }
            }
        }
        
        private class RawTypeValue : RawAnnotationArgument {
            private AsmType type;
            
            RawTypeValue(AsmType type)
                : super(AnnotationArgumentKind.Type) {
                this.type = type;
            }
            
            public override AsmType Type {
                get {
                    return type;
                }
            }
        }

        private class RawAnnotationValue : RawAnnotationArgument {
            private AsmType type;
            private bool runtimeVisible;
            HashMap<String, RawAnnotationArgument> arguments;
            
            RawAnnotationValue(AsmType type, bool runtimeVisible)
                : super(AnnotationArgumentKind.Annotation) {
                this.type = type;
                this.runtimeVisible = runtimeVisible;
                this.arguments = new HashMap<String, RawAnnotationArgument>();
            }

            public override AsmType Type {
                get {
                    return type;
                }
            }

            public override bool IsRuntimeVisible {
                get {
                    return runtimeVisible;
                }
            }
            
            public override RawAnnotationArgument getArgument(String name) {
                return arguments.get(name);
            }
            
            public override Iterable<String> ArgumentNames {
                get {
                    return arguments.keySet();
                }
            }
        }

    }
}
