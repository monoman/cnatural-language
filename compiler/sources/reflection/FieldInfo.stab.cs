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
using stab.tools.helpers;

namespace stab.reflection {

    public class FieldInfo : UserDataContainer {
        int modifiers;
        TypeInfo type;
        List<AnnotationValue> annotations;
        protected Object value*;
        
		public override string toString() {
			return "Field " + Name + " of " + DeclaringType + " type " + Type + " signature " + Signature;
		}
		
        FieldInfo(TypeInfo declaringType, int modifiers, String name, Object value) {
            this.DeclaringType = declaringType;
            this.modifiers = modifiers;
            this.Name = name;
            this.value = value;
        }
        
        public TypeInfo DeclaringType^;

        public String Name^;
        
        public TypeInfo Type {
            get {
                return type;
            }
        }
        
        public Iterable<AnnotationValue> Annotations {
            get {
                if (annotations == null) {
                    annotations = Collections.emptyList();
                }
                return annotations;
            }
        }
        
        public Object Value {
            get {
                return value;
            }
        }
        
        public String Descriptor {
            get {
                var genericDef = this.DeclaringType.GenericTypeDefinition;
                return (genericDef != null) ? genericDef.getField(this.Name).Descriptor : type.Descriptor;
            }
        }
        
        public String Signature {
            get {
                return (type == null || type.IsPrimitive) ? null : type.Signature;
            }
        }
        
        public bool IsEnum {
            get {
                return (modifiers & Opcodes.ACC_ENUM) != 0;
            }
        }
        
        public bool IsFinal {
            get {
                return (modifiers & Opcodes.ACC_FINAL) != 0;
            }
        }
        
        public bool IsPrivate {
            get {
                return (modifiers & Opcodes.ACC_PRIVATE) != 0;
            }
        }
        
        public bool IsProtected {
            get {
                return (modifiers & Opcodes.ACC_PROTECTED) != 0;
            }
        }
        
        public bool IsPublic {
            get {
                return (modifiers & Opcodes.ACC_PUBLIC) != 0;
            }
        }
        
        public bool IsStatic {
            get {
                return (modifiers & Opcodes.ACC_STATIC) != 0;
            }
        }
        
        public bool IsSynthetic {
            get {
                return (modifiers & Opcodes.ACC_SYNTHETIC) != 0;
            }
        }
        
        public bool IsTransient {
            get {
                return (modifiers & Opcodes.ACC_TRANSIENT) != 0;
            }
        }
        
        public bool IsVolatile {
            get {
                return (modifiers & Opcodes.ACC_VOLATILE) != 0;
            }
        }
        
    }
    
}
