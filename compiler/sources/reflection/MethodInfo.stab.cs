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
using org.objectweb.asm;
using stab.query;
using cnatural.helpers;

namespace stab.reflection {

    public class ParameterInfo : UserDataContainer {
        String name;
        List<AnnotationValue> annotations;
    
        ParameterInfo(int position, TypeInfo type) {
            this.Position = position;
            this.Type = type;
        }
        
        public int Position^;
        
        public TypeInfo Type^;
        
        public String Name {
            get {
                if (name == null) {
                    return "p" + this.Position;
                } else {
                    return name;
                }
            }
        }
        
        public Iterable<AnnotationValue> Annotations {
            get {
                return annotations;
            }
        }
    }

    public abstract class MethodInfo : UserDataContainer {
        protected String signature*;
        TypeInfo declaringType;
        
        protected MethodInfo(TypeInfo declaringType, String name) {
            this.declaringType = declaringType;
            this.Name = name;
        }
        
        public TypeInfo DeclaringType {
            get {
            	return declaringType;
            }
        }

        public String Name^;

        public virtual Iterable<AnnotationValue> Annotations {
            get {
                return Query.empty();
            }
        }
        
        public virtual MethodInfo GenericMethodDefinition {
            get {
                return null;
            }
        }

        public virtual String Signature {
            get {
                if (signature == null) {
                    signature = ReflectionHelper.getMethodTypeSignature(this);
                }
                return signature;
            }
        }
        
        public abstract String Descriptor { get; }
        
        public abstract TypeInfo ReturnType { get; }
        
        public abstract Iterable<ParameterInfo> Parameters { get; }
        
        public abstract Iterable<TypeInfo> Exceptions { get; }
        
        public abstract Iterable<TypeInfo> GenericArguments { get; }

        public abstract AnnotationArgument DefaultValue { get; }

        public bool IsAbstract {
            get {
                return (this.Modifiers & Opcodes.ACC_ABSTRACT) != 0;
            }
        }
        
        public bool IsBridge {
            get {
                return (this.Modifiers & Opcodes.ACC_BRIDGE) != 0;
            }
        }
        
        public bool IsFinal {
            get {
                return (this.Modifiers & Opcodes.ACC_FINAL) != 0;
            }
        }
        
        public bool IsNative {
            get {
                return (this.Modifiers & Opcodes.ACC_NATIVE) != 0;
            }
        }
        
        public bool IsPrivate {
            get {
                return (this.Modifiers & Opcodes.ACC_PRIVATE) != 0;
            }
        }
        
        public bool IsProtected {
            get {
                return (this.Modifiers & Opcodes.ACC_PROTECTED) != 0;
            }
        }
        
        public bool IsPublic {
            get {
                return (this.Modifiers & Opcodes.ACC_PUBLIC) != 0;
            }
        }
        
        public bool IsStatic {
            get {
                return (this.Modifiers & Opcodes.ACC_STATIC) != 0;
            }
        }
        
        public bool IsStrict {
            get {
                return (this.Modifiers & Opcodes.ACC_STRICT) != 0;
            }
        }

        public bool IsSynchronized {
            get {
                return (this.Modifiers & Opcodes.ACC_SYNCHRONIZED) != 0;
            }
        }
        
        public bool IsSynthetic {
            get {
                return (this.Modifiers & Opcodes.ACC_SYNTHETIC) != 0;
            }
        }
        
        public bool IsVarargs {
            get {
                return (this.Modifiers & Opcodes.ACC_VARARGS) != 0;
            }
        }
        
        public bool IsConstructor {
            get {
                return this.Name.equals("<init>");
            }
        }
        
        public bool IsClassInitializer {
            get {
                return this.Name.equals("<clinit>");
            }
        }
        
        public bool IsExcludedFromCompilation;
        
        public bool hasSameParameters(MethodInfo method) {
            if (this.IsVarargs != method.IsVarargs) {
                return false;
            }
            if (this.Parameters.count() != method.Parameters.count()) {
                return false;
            }
            FunctionTT<ParameterInfo, TypeInfo> f = p => ReflectionHelper.getRawType(this.DeclaringType, p.Type);
            return this.Parameters.select(f).sequenceEqual(method.Parameters.select(f));
        }

        public bool IsClosed {
            get {
                foreach (var arg in this.GenericArguments) {
                    if (!arg.IsGenericParameter || !this.OriginalMethodDefinition.GenericArguments.contains(arg.OriginalTypeDefinition)) {
                        return true;
                    }
                }
                return false;
            }
        }
        
        public MethodInfo OriginalMethodDefinition {
            get {
                var result = this;
                while (result.GenericMethodDefinition != null) {
                    result = result.GenericMethodDefinition;
                }
                return result;
            }
        }
        
        public bool isOverriding(MethodInfo method) {
            if (this.IsStatic || this == method || method.IsPrivate) {
                return false;
            }
            if (!this.Name.equals(method.Name)) {
                return false;
            }
            if (!this.DeclaringType.canAccessMember(method.DeclaringType, method.IsPublic, method.IsProtected, method.IsPrivate)) {
                return false;
            }
            return this.hasSameParameters(method);
        }
        
        protected abstract int Modifiers { get; }
    }
    
}
