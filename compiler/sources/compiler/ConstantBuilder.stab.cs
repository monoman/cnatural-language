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
using stab.reflection;
using cnatural.parser;
using cnatural.syntaxtree;

namespace cnatural.compiler {

    class ConstantBuilder : ExpressionHandler<Boolean, Object> {
        private CompilerContext context;
        
        ConstantBuilder(CompilerContext context)
            : super(true) {
            this.context = context;
        }
        
        Object buildConstant(ExpressionNode expression) {
            return buildConstant(expression, Boolean.FALSE);
        }
        
        Object buildConstant(ExpressionNode expression, Boolean negative) {
            return handleExpression(expression, negative, true);
        }

        protected override Object handleBinary(BinaryExpressionNode binary, Boolean negative, bool nested) {
            var li = binary.LeftOperand.getUserData(typeof(ExpressionInfo));
            if (li.Value == null) {
                handleExpression(binary.LeftOperand, Boolean.FALSE, nested);
            }
            var ri = binary.RightOperand.getUserData(typeof(ExpressionInfo));
            if (ri.Value == null) {
                handleExpression(binary.RightOperand, Boolean.FALSE, nested);
            }
            switch (binary.Operator) {
            case Add:
                switch (li.Type.TypeKind) {
                case Byte: {
                    int left = ((Byte)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left + ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left + ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left + ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left + ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left + ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left + ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left + ((Double)ri.Value).doubleValue());
                    default:
                        if (ri.Type == context.TypeSystem.StringType) {
                            return left + ((String)ri.Value);
                        }
                        break;
                    }
                    break;
                }
                case Short: {
                    int left = ((Short)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left + ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left + ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left + ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left + ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left + ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left + ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left + ((Double)ri.Value).doubleValue());
                    default:
                        if (ri.Type == context.TypeSystem.StringType) {
                            return left + ((String)ri.Value);
                        }
                        break;
                    }
                    break;
                }
                case Char: {
                    int left = ((Character)li.Value).charValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left + ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left + ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left + ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left + ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left + ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left + ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left + ((Double)ri.Value).doubleValue());
                    default:
                        if (ri.Type == context.TypeSystem.StringType) {
                            return left + ((String)ri.Value);
                        }
                        break;
                    }
                    break;
                }
                case Int: {
                    int left = ((Integer)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left + ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left + ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left + ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left + ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left + ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left + ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left + ((Double)ri.Value).doubleValue());
                    default:
                        if (ri.Type == context.TypeSystem.StringType) {
                            return left + ((String)ri.Value);
                        }
                        break;
                    }
                    break;
                }
                case Long: {
                    long left = ((Long)li.Value).longValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Long.valueOf(left + ((Byte)ri.Value).intValue());
                    case Short:
                        return Long.valueOf(left + ((Short)ri.Value).intValue());
                    case Char:
                        return Long.valueOf(left + ((Character)ri.Value).charValue());
                    case Int:
                        return Long.valueOf(left + ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left + ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left + ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left + ((Double)ri.Value).doubleValue());
                    default:
                        if (ri.Type == context.TypeSystem.StringType) {
                            return left + ((String)ri.Value);
                        }
                        break;
                    }
                    break;
                }
                case Float: {
                    float left = ((Float)li.Value).floatValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Float.valueOf(left + ((Byte)ri.Value).intValue());
                    case Short:
                        return Float.valueOf(left + ((Short)ri.Value).intValue());
                    case Char:
                        return Float.valueOf(left + ((Character)ri.Value).charValue());
                    case Int:
                        return Float.valueOf(left + ((Integer)ri.Value).intValue());
                    case Long:
                        return Float.valueOf(left + ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left + ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left + ((Double)ri.Value).doubleValue());
                    default:
                        if (ri.Type == context.TypeSystem.StringType) {
                            return left + ((String)ri.Value);
                        }
                        break;
                    }
                    break;
                }
                case Double: {
                    double left = ((Double)li.Value).doubleValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Double.valueOf(left + ((Byte)ri.Value).intValue());
                    case Short:
                        return Double.valueOf(left + ((Short)ri.Value).intValue());
                    case Char:
                        return Double.valueOf(left + ((Character)ri.Value).charValue());
                    case Int:
                        return Double.valueOf(left + ((Integer)ri.Value).intValue());
                    case Long:
                        return Double.valueOf(left + ((Long)ri.Value).longValue());
                    case Float:
                        return Double.valueOf(left + ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left + ((Double)ri.Value).doubleValue());
                    default:
                        if (ri.Type == context.TypeSystem.StringType) {
                            return left + ((String)ri.Value);
                        }
                        break;
                    }
                    break;
                }
                default: {
                    String left = (String)li.Value;
                    switch (ri.Type.TypeKind) {
					case Boolean:
                        return left + ((Boolean)ri.Value).booleanValue();
                    case Byte:
                        return left + ((Byte)ri.Value).intValue();
                    case Short:
                        return left + ((Short)ri.Value).intValue();
                    case Char:
                        return left + ((Character)ri.Value).charValue();
                    case Int:
                        return left + ((Integer)ri.Value).intValue();
                    case Long:
                        return left + ((Long)ri.Value).longValue();
                    case Float:
                        return left + ((Float)ri.Value).floatValue();
                    case Double:
                        return left + ((Double)ri.Value).doubleValue();
                    default:
                        if (ri.Type == context.TypeSystem.StringType) {
                            return left + ((String)ri.Value);
                        }
                        break;
                    }
                    break;
                }
                }
                break;
            case Subtract:
                switch (li.Type.TypeKind) {
                case Byte: {
                    int left = ((Byte)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left - ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left - ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left - ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left - ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left - ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left - ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left - ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Short: {
                    int left = ((Short)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left - ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left - ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left - ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left - ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left - ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left - ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left - ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Char: {
                    int left = ((Character)li.Value).charValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left - ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left - ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left - ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left - ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left - ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left - ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left - ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Int: {
                    int left = ((Integer)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left - ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left - ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left - ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left - ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left - ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left - ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left - ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Long: {
                    long left = ((Long)li.Value).longValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Long.valueOf(left - ((Byte)ri.Value).intValue());
                    case Short:
                        return Long.valueOf(left - ((Short)ri.Value).intValue());
                    case Char:
                        return Long.valueOf(left - ((Character)ri.Value).charValue());
                    case Int:
                        return Long.valueOf(left - ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left - ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left - ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left - ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Float: {
                    float left = ((Float)li.Value).floatValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Float.valueOf(left - ((Byte)ri.Value).intValue());
                    case Short:
                        return Float.valueOf(left - ((Short)ri.Value).intValue());
                    case Char:
                        return Float.valueOf(left - ((Character)ri.Value).charValue());
                    case Int:
                        return Float.valueOf(left - ((Integer)ri.Value).intValue());
                    case Long:
                        return Float.valueOf(left - ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left - ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left - ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Double: {
                    double left = ((Double)li.Value).doubleValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Double.valueOf(left - ((Byte)ri.Value).intValue());
                    case Short:
                        return Double.valueOf(left - ((Short)ri.Value).intValue());
                    case Char:
                        return Double.valueOf(left - ((Character)ri.Value).charValue());
                    case Int:
                        return Double.valueOf(left - ((Integer)ri.Value).intValue());
                    case Long:
                        return Double.valueOf(left - ((Long)ri.Value).longValue());
                    case Float:
                        return Double.valueOf(left - ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left - ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                }
                break;
            case Divide:
                switch (li.Type.TypeKind) {
                case Byte: {
                    int left = ((Byte)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left / ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left / ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left / ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left / ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left / ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left / ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left / ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Short: {
                    int left = ((Short)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left / ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left / ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left / ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left / ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left / ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left / ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left / ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Char: {
                    int left = ((Character)li.Value).charValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left / ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left / ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left / ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left / ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left / ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left / ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left / ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Int: {
                    int left = ((Integer)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left / ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left / ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left / ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left / ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left / ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left / ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left / ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Long: {
                    long left = ((Long)li.Value).longValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Long.valueOf(left / ((Byte)ri.Value).intValue());
                    case Short:
                        return Long.valueOf(left / ((Short)ri.Value).intValue());
                    case Char:
                        return Long.valueOf(left / ((Character)ri.Value).charValue());
                    case Int:
                        return Long.valueOf(left / ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left / ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left / ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left / ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Float: {
                    float left = ((Float)li.Value).floatValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Float.valueOf(left / ((Byte)ri.Value).intValue());
                    case Short:
                        return Float.valueOf(left / ((Short)ri.Value).intValue());
                    case Char:
                        return Float.valueOf(left / ((Character)ri.Value).charValue());
                    case Int:
                        return Float.valueOf(left / ((Integer)ri.Value).intValue());
                    case Long:
                        return Float.valueOf(left / ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left / ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left / ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Double: {
                    double left = ((Double)li.Value).doubleValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Double.valueOf(left / ((Byte)ri.Value).intValue());
                    case Short:
                        return Double.valueOf(left / ((Short)ri.Value).intValue());
                    case Char:
                        return Double.valueOf(left / ((Character)ri.Value).charValue());
                    case Int:
                        return Double.valueOf(left / ((Integer)ri.Value).intValue());
                    case Long:
                        return Double.valueOf(left / ((Long)ri.Value).longValue());
                    case Float:
                        return Double.valueOf(left / ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left / ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                }
                break;
            case Multiply:
                switch (li.Type.TypeKind) {
                case Byte: {
                    int left = ((Byte)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left / ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left / ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left / ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left / ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left / ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left / ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left / ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Short: {
                    int left = ((Short)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left / ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left / ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left / ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left / ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left / ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left / ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left / ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Char: {
                    int left = ((Character)li.Value).charValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left / ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left / ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left / ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left / ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left / ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left / ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left / ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Int: {
                    int left = ((Integer)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left / ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left / ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left / ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left / ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left / ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left / ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left / ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Long: {
                    long left = ((Long)li.Value).longValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Long.valueOf(left / ((Byte)ri.Value).intValue());
                    case Short:
                        return Long.valueOf(left / ((Short)ri.Value).intValue());
                    case Char:
                        return Long.valueOf(left / ((Character)ri.Value).charValue());
                    case Int:
                        return Long.valueOf(left / ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left / ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left / ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left / ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Float: {
                    float left = ((Float)li.Value).floatValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Float.valueOf(left / ((Byte)ri.Value).intValue());
                    case Short:
                        return Float.valueOf(left / ((Short)ri.Value).intValue());
                    case Char:
                        return Float.valueOf(left / ((Character)ri.Value).charValue());
                    case Int:
                        return Float.valueOf(left / ((Integer)ri.Value).intValue());
                    case Long:
                        return Float.valueOf(left / ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left / ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left / ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Double: {
                    double left = ((Double)li.Value).doubleValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Double.valueOf(left / ((Byte)ri.Value).intValue());
                    case Short:
                        return Double.valueOf(left / ((Short)ri.Value).intValue());
                    case Char:
                        return Double.valueOf(left / ((Character)ri.Value).charValue());
                    case Int:
                        return Double.valueOf(left / ((Integer)ri.Value).intValue());
                    case Long:
                        return Double.valueOf(left / ((Long)ri.Value).longValue());
                    case Float:
                        return Double.valueOf(left / ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left / ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                }
                break;
            case Modulo:
                switch (li.Type.TypeKind) {
                case Byte: {
                    int left = ((Byte)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left % ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left % ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left % ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left % ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left % ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left % ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left % ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Short: {
                    int left = ((Short)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left % ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left % ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left % ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left % ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left % ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left % ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left % ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Char: {
                    int left = ((Character)li.Value).charValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left % ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left % ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left % ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left % ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left % ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left % ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left % ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Int: {
                    int left = ((Integer)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(left % ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(left % ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(left % ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(left % ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left % ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left % ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left % ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Long: {
                    long left = ((Long)li.Value).longValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Long.valueOf(left % ((Byte)ri.Value).intValue());
                    case Short:
                        return Long.valueOf(left % ((Short)ri.Value).intValue());
                    case Char:
                        return Long.valueOf(left % ((Character)ri.Value).charValue());
                    case Int:
                        return Long.valueOf(left % ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(left % ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left % ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left % ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Float: {
                    float left = ((Float)li.Value).floatValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Float.valueOf(left % ((Byte)ri.Value).intValue());
                    case Short:
                        return Float.valueOf(left % ((Short)ri.Value).intValue());
                    case Char:
                        return Float.valueOf(left % ((Character)ri.Value).charValue());
                    case Int:
                        return Float.valueOf(left % ((Integer)ri.Value).intValue());
                    case Long:
                        return Float.valueOf(left % ((Long)ri.Value).longValue());
                    case Float:
                        return Float.valueOf(left % ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left % ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Double: {
                    double left = ((Double)li.Value).doubleValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Double.valueOf(left % ((Byte)ri.Value).intValue());
                    case Short:
                        return Double.valueOf(left % ((Short)ri.Value).intValue());
                    case Char:
                        return Double.valueOf(left % ((Character)ri.Value).charValue());
                    case Int:
                        return Double.valueOf(left % ((Integer)ri.Value).intValue());
                    case Long:
                        return Double.valueOf(left % ((Long)ri.Value).longValue());
                    case Float:
                        return Double.valueOf(left % ((Float)ri.Value).floatValue());
                    case Double:
                        return Double.valueOf(left % ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                }
                break;
            case Equal:
                switch (li.Type.TypeKind) {
                case Byte: {
                    int left = ((Byte)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left == ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left == ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left == ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left == ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left == ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left == ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left == ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Short: {
                    int left = ((Short)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left == ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left == ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left == ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left == ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left == ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left == ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left == ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Char: {
                    int left = ((Character)li.Value).charValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left == ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left == ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left == ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left == ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left == ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left == ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left == ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Int: {
                    int left = ((Integer)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left == ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left == ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left == ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left == ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left == ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left == ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left == ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Long: {
                    long left = ((Long)li.Value).longValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left == ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left == ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left == ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left == ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left == ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left == ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left == ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Float: {
                    float left = ((Float)li.Value).floatValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left == ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left == ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left == ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left == ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left == ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left == ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left == ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Double: {
                    double left = ((Double)li.Value).doubleValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left == ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left == ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left == ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left == ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left == ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left == ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left == ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                }
                break;
            case NotEqual:
                switch (li.Type.TypeKind) {
                case Byte: {
                    int left = ((Byte)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left != ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left != ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left != ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left != ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left != ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left != ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left != ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Short: {
                    int left = ((Short)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left != ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left != ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left != ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left != ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left != ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left != ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left != ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Char: {
                    int left = ((Character)li.Value).charValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left != ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left != ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left != ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left != ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left != ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left != ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left != ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Int: {
                    int left = ((Integer)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left != ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left != ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left != ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left != ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left != ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left != ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left != ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Long: {
                    long left = ((Long)li.Value).longValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left != ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left != ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left != ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left != ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left != ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left != ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left != ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Float: {
                    float left = ((Float)li.Value).floatValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left != ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left != ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left != ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left != ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left != ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left != ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left != ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Double: {
                    double left = ((Double)li.Value).doubleValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left != ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left != ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left != ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left != ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left != ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left != ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left != ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                }
                break;
            case GreaterThanOrEqual:
                switch (li.Type.TypeKind) {
                case Byte: {
                    int left = ((Byte)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left >= ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left >= ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left >= ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left >= ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left >= ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left >= ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left >= ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Short: {
                    int left = ((Short)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left >= ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left >= ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left >= ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left >= ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left >= ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left >= ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left >= ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Char: {
                    int left = ((Character)li.Value).charValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left >= ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left >= ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left >= ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left >= ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left >= ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left >= ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left >= ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Int: {
                    int left = ((Integer)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left >= ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left >= ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left >= ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left >= ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left >= ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left >= ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left >= ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Long: {
                    long left = ((Long)li.Value).longValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left >= ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left >= ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left >= ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left >= ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left >= ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left >= ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left >= ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Float: {
                    float left = ((Float)li.Value).floatValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left >= ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left >= ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left >= ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left >= ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left >= ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left >= ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left >= ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Double: {
                    double left = ((Double)li.Value).doubleValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left >= ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left >= ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left >= ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left >= ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left >= ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left >= ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left >= ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                }
                break;
            case LessThan:
                switch (li.Type.TypeKind) {
                case Byte: {
                    int left = ((Byte)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left < ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left < ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left < ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left < ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left < ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left < ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left < ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Short: {
                    int left = ((Short)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left < ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left < ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left < ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left < ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left < ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left < ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left < ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Char: {
                    int left = ((Character)li.Value).charValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left < ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left < ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left < ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left < ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left < ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left < ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left < ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Int: {
                    int left = ((Integer)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left < ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left < ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left < ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left < ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left < ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left < ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left < ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Long: {
                    long left = ((Long)li.Value).longValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left < ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left < ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left < ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left < ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left < ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left < ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left < ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Float: {
                    float left = ((Float)li.Value).floatValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left < ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left < ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left < ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left < ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left < ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left < ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left < ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Double: {
                    double left = ((Double)li.Value).doubleValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left < ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left < ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left < ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left < ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left < ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left < ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left < ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                }
                break;
            case LessThanOrEqual:
                switch (li.Type.TypeKind) {
                case Byte: {
                    int left = ((Byte)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left <= ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left <= ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left <= ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left <= ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left <= ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left <= ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left <= ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Short: {
                    int left = ((Short)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left <= ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left <= ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left <= ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left <= ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left <= ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left <= ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left <= ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Char: {
                    int left = ((Character)li.Value).charValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left <= ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left <= ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left <= ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left <= ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left <= ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left <= ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left <= ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Int: {
                    int left = ((Integer)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left <= ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left <= ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left <= ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left <= ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left <= ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left <= ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left <= ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Long: {
                    long left = ((Long)li.Value).longValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left <= ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left <= ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left <= ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left <= ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left <= ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left <= ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left <= ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Float: {
                    float left = ((Float)li.Value).floatValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left <= ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left <= ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left <= ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left <= ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left <= ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left <= ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left <= ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Double: {
                    double left = ((Double)li.Value).doubleValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left <= ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left <= ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left <= ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left <= ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left <= ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left <= ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left <= ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                }
                break;
            case GreaterThan:
                switch (li.Type.TypeKind) {
                case Byte: {
                    int left = ((Byte)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left > ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left > ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left > ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left > ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left > ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left > ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left > ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Short: {
                    int left = ((Short)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left > ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left > ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left > ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left > ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left > ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left > ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left > ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Char: {
                    int left = ((Character)li.Value).charValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left > ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left > ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left > ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left > ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left > ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left > ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left > ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Int: {
                    int left = ((Integer)li.Value).intValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left > ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left > ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left > ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left > ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left > ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left > ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left > ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Long: {
                    long left = ((Long)li.Value).longValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left > ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left > ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left > ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left > ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left > ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left > ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left > ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Float: {
                    float left = ((Float)li.Value).floatValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left > ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left > ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left > ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left > ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left > ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left > ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left > ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                case Double: {
                    double left = ((Double)li.Value).doubleValue();
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Boolean.valueOf(left > ((Byte)ri.Value).intValue());
                    case Short:
                        return Boolean.valueOf(left > ((Short)ri.Value).intValue());
                    case Char:
                        return Boolean.valueOf(left > ((Character)ri.Value).charValue());
                    case Int:
                        return Boolean.valueOf(left > ((Integer)ri.Value).intValue());
                    case Long:
                        return Boolean.valueOf(left > ((Long)ri.Value).longValue());
                    case Float:
                        return Boolean.valueOf(left > ((Float)ri.Value).floatValue());
                    case Double:
                        return Boolean.valueOf(left > ((Double)ri.Value).doubleValue());
                    }
                    break;
                }
                }
                break;
            case LeftShift:
                switch (li.Type.TypeKind) {
                case Byte:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Byte)li.Value).intValue() << ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Byte)li.Value).intValue() << ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Byte)li.Value).intValue() << ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Byte)li.Value).intValue() << ((Integer)ri.Value).intValue());
                    }
                    break;
                case Short:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Short)li.Value).intValue() << ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Short)li.Value).intValue() << ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Short)li.Value).intValue() << ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Short)li.Value).intValue() << ((Integer)ri.Value).intValue());
                    }
                    break;
                case Char:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Character)li.Value).charValue() << ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Character)li.Value).charValue() << ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Character)li.Value).charValue() << ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Character)li.Value).charValue() << ((Integer)ri.Value).intValue());
                    }
                    break;
                case Int:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Integer)li.Value).intValue() << ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Integer)li.Value).intValue() << ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Integer)li.Value).intValue() << ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Integer)li.Value).intValue() << ((Integer)ri.Value).intValue());
                    }
                    break;
                case Long:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Long.valueOf(((Long)li.Value).longValue() << ((Byte)ri.Value).intValue());
                    case Short:
                        return Long.valueOf(((Long)li.Value).longValue() << ((Short)ri.Value).intValue());
                    case Char:
                        return Long.valueOf(((Long)li.Value).longValue() << ((Character)ri.Value).charValue());
                    case Int:
                        return Long.valueOf(((Long)li.Value).longValue() << ((Integer)ri.Value).intValue());
                    }
                    break;
                }
                break;
            case RightShift:
                switch (li.Type.TypeKind) {
                case Byte:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Byte)li.Value).intValue() >> ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Byte)li.Value).intValue() >> ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Byte)li.Value).intValue() >> ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Byte)li.Value).intValue() >> ((Integer)ri.Value).intValue());
                    }
                    break;
                case Short:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Short)li.Value).intValue() >> ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Short)li.Value).intValue() >> ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Short)li.Value).intValue() >> ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Short)li.Value).intValue() >> ((Integer)ri.Value).intValue());
                    }
                    break;
                case Char:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Character)li.Value).charValue() >> ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Character)li.Value).charValue() >> ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Character)li.Value).charValue() >> ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Character)li.Value).charValue() >> ((Integer)ri.Value).intValue());
                    }
                    break;
                case Int:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Integer)li.Value).intValue() >> ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Integer)li.Value).intValue() >> ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Integer)li.Value).intValue() >> ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Integer)li.Value).intValue() >> ((Integer)ri.Value).intValue());
                    }
                    break;
                case Long:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Long.valueOf(((Long)li.Value).longValue() >> ((Byte)ri.Value).intValue());
                    case Short:
                        return Long.valueOf(((Long)li.Value).longValue() >> ((Short)ri.Value).intValue());
                    case Char:
                        return Long.valueOf(((Long)li.Value).longValue() >> ((Character)ri.Value).charValue());
                    case Int:
                        return Long.valueOf(((Long)li.Value).longValue() >> ((Integer)ri.Value).intValue());
                    }
                    break;
                }
                break;
            case UnsignedRightShift:
                switch (li.Type.TypeKind) {
                case Byte:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Byte)li.Value).intValue() >>> ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Byte)li.Value).intValue() >>> ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Byte)li.Value).intValue() >>> ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Byte)li.Value).intValue() >>> ((Integer)ri.Value).intValue());
                    }
                    break;
                case Short:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Short)li.Value).intValue() >>> ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Short)li.Value).intValue() >>> ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Short)li.Value).intValue() >>> ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Short)li.Value).intValue() >>> ((Integer)ri.Value).intValue());
                    }
                    break;
                case Char:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Character)li.Value).charValue() >>> ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Character)li.Value).charValue() >>> ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Character)li.Value).charValue() >>> ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Character)li.Value).charValue() >>> ((Integer)ri.Value).intValue());
                    }
                    break;
                case Int:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Integer)li.Value).intValue() >>> ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Integer)li.Value).intValue() >>> ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Integer)li.Value).intValue() >>> ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Integer)li.Value).intValue() >>> ((Integer)ri.Value).intValue());
                    }
                    break;
                case Long:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Long.valueOf(((Long)li.Value).longValue() >>> ((Byte)ri.Value).intValue());
                    case Short:
                        return Long.valueOf(((Long)li.Value).longValue() >>> ((Short)ri.Value).intValue());
                    case Char:
                        return Long.valueOf(((Long)li.Value).longValue() >>> ((Character)ri.Value).charValue());
                    case Int:
                        return Long.valueOf(((Long)li.Value).longValue() >>> ((Integer)ri.Value).intValue());
                    }
                    break;
                }
                break;
            case And:
                switch (li.Type.TypeKind) {
                case Boolean:
                    return Boolean.valueOf(((Boolean)li.Value).booleanValue() & ((Boolean)ri.Value).booleanValue());
                case Byte:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Byte)li.Value).intValue() & ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Byte)li.Value).intValue() & ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Byte)li.Value).intValue() & ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Byte)li.Value).intValue() & ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(((Byte)li.Value).intValue() & ((Long)ri.Value).longValue());
                    }
                    break;
                case Short:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Short)li.Value).intValue() & ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Short)li.Value).intValue() & ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Short)li.Value).intValue() & ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Short)li.Value).intValue() & ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(((Short)li.Value).intValue() & ((Long)ri.Value).longValue());
                    }
                    break;
                case Char:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Character)li.Value).charValue() & ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Character)li.Value).charValue() & ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Character)li.Value).charValue() & ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Character)li.Value).charValue() & ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(((Character)li.Value).charValue() & ((Long)ri.Value).longValue());
                    }
                    break;
                case Int:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Integer)li.Value).intValue() & ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Integer)li.Value).intValue() & ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Integer)li.Value).intValue() & ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Integer)li.Value).intValue() & ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(((Integer)li.Value).intValue() & ((Long)ri.Value).longValue());
                    }
                    break;
                case Long:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Long.valueOf(((Long)li.Value).longValue() & ((Byte)ri.Value).intValue());
                    case Short:
                        return Long.valueOf(((Long)li.Value).longValue() & ((Short)ri.Value).intValue());
                    case Char:
                        return Long.valueOf(((Long)li.Value).longValue() & ((Character)ri.Value).charValue());
                    case Int:
                        return Long.valueOf(((Long)li.Value).longValue() & ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(((Long)li.Value).longValue() & ((Long)ri.Value).longValue());
                    }
                    break;
                }
                break;
            case Or:
                switch (li.Type.TypeKind) {
                case Boolean:
                    return Boolean.valueOf(((Boolean)li.Value).booleanValue() & ((Boolean)ri.Value).booleanValue());
                case Byte:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Byte)li.Value).intValue() | ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Byte)li.Value).intValue() | ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Byte)li.Value).intValue() | ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Byte)li.Value).intValue() | ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(((Byte)li.Value).intValue() | ((Long)ri.Value).longValue());
                    }
                    break;
                case Short:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Short)li.Value).intValue() | ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Short)li.Value).intValue() | ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Short)li.Value).intValue() | ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Short)li.Value).intValue() | ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(((Short)li.Value).intValue() | ((Long)ri.Value).longValue());
                    }
                    break;
                case Char:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Character)li.Value).charValue() | ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Character)li.Value).charValue() | ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Character)li.Value).charValue() | ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Character)li.Value).charValue() | ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(((Character)li.Value).charValue() | ((Long)ri.Value).longValue());
                    }
                    break;
                case Int:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Integer)li.Value).intValue() | ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Integer)li.Value).intValue() | ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Integer)li.Value).intValue() | ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Integer)li.Value).intValue() | ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(((Integer)li.Value).intValue() | ((Long)ri.Value).longValue());
                    }
                    break;
                case Long:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Long.valueOf(((Long)li.Value).longValue() | ((Byte)ri.Value).intValue());
                    case Short:
                        return Long.valueOf(((Long)li.Value).longValue() | ((Short)ri.Value).intValue());
                    case Char:
                        return Long.valueOf(((Long)li.Value).longValue() | ((Character)ri.Value).charValue());
                    case Int:
                        return Long.valueOf(((Long)li.Value).longValue() | ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(((Long)li.Value).longValue() | ((Long)ri.Value).longValue());
                    }
                    break;
                }
                break;
            case Xor:
                switch (li.Type.TypeKind) {
                case Boolean:
                    return Boolean.valueOf(((Boolean)li.Value).booleanValue() & ((Boolean)ri.Value).booleanValue());
                case Byte:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Byte)li.Value).intValue() ^ ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Byte)li.Value).intValue() ^ ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Byte)li.Value).intValue() ^ ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Byte)li.Value).intValue() ^ ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(((Byte)li.Value).intValue() ^ ((Long)ri.Value).longValue());
                    }
                    break;
                case Short:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Short)li.Value).intValue() ^ ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Short)li.Value).intValue() ^ ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Short)li.Value).intValue() ^ ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Short)li.Value).intValue() ^ ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(((Short)li.Value).intValue() ^ ((Long)ri.Value).longValue());
                    }
                    break;
                case Char:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Character)li.Value).charValue() ^ ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Character)li.Value).charValue() ^ ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Character)li.Value).charValue() ^ ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Character)li.Value).charValue() ^ ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(((Character)li.Value).charValue() ^ ((Long)ri.Value).longValue());
                    }
                    break;
                case Int:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Integer.valueOf(((Integer)li.Value).intValue() ^ ((Byte)ri.Value).intValue());
                    case Short:
                        return Integer.valueOf(((Integer)li.Value).intValue() ^ ((Short)ri.Value).intValue());
                    case Char:
                        return Integer.valueOf(((Integer)li.Value).intValue() ^ ((Character)ri.Value).charValue());
                    case Int:
                        return Integer.valueOf(((Integer)li.Value).intValue() ^ ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(((Integer)li.Value).intValue() ^ ((Long)ri.Value).longValue());
                    }
                    break;
                case Long:
                    switch (ri.Type.TypeKind) {
                    case Byte:
                        return Long.valueOf(((Long)li.Value).longValue() ^ ((Byte)ri.Value).intValue());
                    case Short:
                        return Long.valueOf(((Long)li.Value).longValue() ^ ((Short)ri.Value).intValue());
                    case Char:
                        return Long.valueOf(((Long)li.Value).longValue() ^ ((Character)ri.Value).charValue());
                    case Int:
                        return Long.valueOf(((Long)li.Value).longValue() ^ ((Integer)ri.Value).intValue());
                    case Long:
                        return Long.valueOf(((Long)li.Value).longValue() ^ ((Long)ri.Value).longValue());
                    }
                    break;
                }
                break;
            case LogicalAnd:
                return Boolean.valueOf(((Boolean)li.Value).booleanValue() && ((Boolean)ri.Value).booleanValue());
            case LogicalOr:
                return Boolean.valueOf(((Boolean)li.Value).booleanValue() || ((Boolean)ri.Value).booleanValue());
            }
            throw context.error(CompileErrorId.BinaryOperatorNotApplicable, binary,
                    CompilerHelper.getDisplayName(binary.Operator),
                    BytecodeHelper.getDisplayName(li.Type), BytecodeHelper.getDisplayName(ri.Type));
        }
        
        protected override Object handleLiteral(LiteralExpressionNode literal, Boolean negative, bool nested) {
            var info = literal.getUserData(typeof(ExpressionInfo));
            switch (literal.LiteralKind) {
            case Character: {
                char cvalue = ParserHelper.unescapeChar(context.Text, literal.ValueOffset + 1, literal.ValueLength - 2);
                info.Value = Character.valueOf((negative) ? (char)-cvalue : cvalue);
                info.Type = context.TypeSystem.CharType;
                break;
            }
            case String: {
                info.Value = ParserHelper.unescapeString(context.Text, literal.ValueOffset + 1, literal.ValueLength - 2);
                info.Type = context.TypeSystem.StringType;
                break;
            }
            case VerbatimString: {
                info.Value = ParserHelper.unescapeVerbatimString(context.Text, literal.ValueOffset + 2, literal.ValueLength - 3);
                info.Type = context.TypeSystem.StringType;
                break;
            }
            case DecimalInteger: {
                var value = context.getText(literal.ValueOffset, literal.ValueLength);
                try {
                    int ivalue = (negative) ? Integer.parseInt("-" + value) : Integer.parseInt(value);
                    info.Value = Integer.valueOf(ivalue);
                    info.Type = context.TypeSystem.IntType;
                } catch (NumberFormatException e1) {
                    try {
                        long lvalue = (negative) ? Long.parseLong("-" + value) : Long.parseLong(value);
                        info.Value = Long.valueOf(lvalue);
                        info.Type = context.TypeSystem.LongType;
                    } catch (NumberFormatException e2) {
                        throw context.error(CompileErrorId.IntegralConstantTooLarge, literal);
                    }
                }
                break;
            }
            case HexadecimalInteger: {
                String value = context.getText(literal.ValueOffset, literal.ValueLength);
                try {
                    int ivalue = ParserHelper.decodeHexadecimalInteger(value);
                    info.Value = Integer.valueOf((negative) ? -ivalue : ivalue);
                    info.Type = context.TypeSystem.IntType;
                } catch (NumberFormatException e) {
                    try {
                        long lvalue = ParserHelper.decodeHexadecimalLong(value);
                        info.Value = Long.valueOf((negative) ? -lvalue : lvalue);
                        info.Type = context.TypeSystem.LongType;
                    } catch (NumberFormatException e2) {
                        throw context.error(CompileErrorId.IntegralConstantTooLarge, literal);
                    }
                }
                break;
            }
            case Long: {
                String value = context.getText(literal.ValueOffset, literal.ValueLength - 1);
                try {
                    long lvalue = (negative) ? Long.parseLong("-" + value) : Long.parseLong(value);
                    info.Value = Long.valueOf(lvalue);
                    info.Type = context.TypeSystem.LongType;
                } catch (NumberFormatException e2) {
                    throw context.error(CompileErrorId.IntegralConstantTooLarge, literal);
                }
                break;
            }
            case HexadecimalLong: {
                String value = context.getText(literal.ValueOffset, literal.ValueLength - 1);
                try {
                    long lvalue = ParserHelper.decodeHexadecimalLong(value);
                    info.Value = Long.valueOf((negative) ? -lvalue : lvalue);
                    info.Type = context.TypeSystem.LongType;
                } catch (NumberFormatException e) {
                    throw context.error(CompileErrorId.IntegralConstantTooLarge, literal);
                }
                break;
            }
            case Float: {
                String value = context.getText(literal.ValueOffset, literal.ValueLength - 1);
                try {
                    if (value[0] == '.') {
                        value = "0" + value;
                    }
                    float fvalue = Float.parseFloat(value);
                    info.Value = Float.valueOf((negative) ? -fvalue : fvalue);
                    info.Type = context.TypeSystem.FloatType;
                } catch (NumberFormatException e2) {
                    throw new IllegalStateException("Internal error: float parsing failed");
                }
                break;
            }
            case Double: {
                String value = context.getText(literal.ValueOffset, literal.ValueLength - 1);
                try {
                    if (value[0] == '.') {
                        value = "0" + value;
                    }
                    double dvalue = Double.parseDouble(value);
                    info.Value = Double.valueOf((negative) ? -dvalue : dvalue);
                    info.Type = context.TypeSystem.DoubleType;
                } catch (NumberFormatException e2) {
                    throw new IllegalStateException("Internal error: double parsing failed");
                }
                break;
            }
            case Real: {
                String value = context.getText(literal.ValueOffset, literal.ValueLength);
                try {
                    if (value[0] == '.') {
                        value = "0" + value;
                    }
                    double dvalue = Double.parseDouble(value);
                    info.Value = Double.valueOf((negative) ? -dvalue : dvalue);
                    info.Type = context.TypeSystem.DoubleType;
                } catch (NumberFormatException e1) {
                    throw new IllegalStateException("Internal error: double parsing failed");
                }
                break;
            }
            case Null: {
                break;
            }
            case True: {
                info.Value = Boolean.TRUE;
                info.Type = context.TypeSystem.BooleanType;
                break;
            }
            case False: {
                info.Value = Boolean.FALSE;
                info.Type = context.TypeSystem.BooleanType;
                break;
            }
            default:
                throw new RuntimeException("Internal error: unhandled literal " + literal.getLiteralKind());
            }
            return (info == null) ? null : info.Value;
        }
        
        protected override Object handleUnary(UnaryExpressionNode unary, Boolean negative, bool nested) {
            var ei = unary.Operand.getUserData(typeof(ExpressionInfo));
            negative = Boolean.FALSE;
            switch (unary.Operator) {
            case Complement:
                if (ei.Value == null) {
                    handleExpression(unary.Operand, Boolean.FALSE, nested);
                }
                switch (ei.Type.TypeKind) {
                case Byte:
                    ei.Value = Integer.valueOf(~((Byte)ei.Value).intValue());
                    break;
                case Short:
                    return Integer.valueOf(~((Short)ei.Value).intValue());
                case Char:
                    return Integer.valueOf(~((Character)ei.Value).charValue());
                case Int:
                    return Integer.valueOf(~((Integer)ei.Value).intValue());
                case Long:
                    return Long.valueOf(~((Long)ei.Value).longValue());
                }
                break;
            case Minus:
                negative = Boolean.TRUE;
                goto case Plus;
            case Plus:
                if (ei.Value == null) {
                    handleExpression(unary.Operand, negative, nested);
                }
                switch (ei.Type.TypeKind) {
                case Byte:
                    return Integer.valueOf(((Byte)ei.Value).intValue());
                case Short:
                    return Integer.valueOf(((Short)ei.Value).intValue());
                case Char:
                    return Integer.valueOf(((Character)ei.Value).charValue());
                case Int:
                case Long:
                case Float:
                case Double:
                    return ei.Value;
                }
                break;
            case Not:
                if (ei.Value == null) {
                    handleExpression(unary.Operand, Boolean.FALSE, nested);
                }
                return Boolean.valueOf(!((Boolean)ei.Value).booleanValue());
            }
            throw context.error(CompileErrorId.UnaryOperatorNotApplicable, unary,
                    CompilerHelper.getDisplayName(unary.Operator), BytecodeHelper.getDisplayName(ei.Type));
        }
    }
}
