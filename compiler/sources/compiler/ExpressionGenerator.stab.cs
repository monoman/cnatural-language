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
using stab.query;
using stab.reflection;
using cnatural.syntaxtree;

namespace cnatural.compiler {

    class TargetLabels {
        LabelMarker ifTrue;
        LabelMarker ifFalse;
        
        TargetLabels(LabelMarker ifTrue, LabelMarker ifFalse) {
            this.ifTrue = ifTrue;
            this.ifFalse = ifFalse;
        }
    }

    class ExpressionGenerator : ExpressionHandler<TargetLabels, Void> {
        private BytecodeGenerator bytecodeGenerator;
        private CompilerContext context;
        private ExpressionTreeGenerator expressionTreeGenerator;
    
        ExpressionGenerator(BytecodeGenerator bytecodeGenerator, CompilerContext context)
            : super(true) {
            this.bytecodeGenerator = bytecodeGenerator;
            this.context = context;
            this.expressionTreeGenerator = new ExpressionTreeGenerator(context);
        }
        
        public override Void handleExpression(ExpressionNode expression, TargetLabels labels, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            if (expression.Line > context.MethodGenerationContext.PreviousLineNumber) {
                var label = generator.defineLabel();
                generator.markLabel(label);
                generator.lineNumber(expression.Line + 1, label);
                context.MethodGenerationContext.PreviousLineNumber = expression.Line;
            }            
            var info = expression.getUserData(typeof(ExpressionInfo));
            if (info == null) {
                generator.emit(Opcode.Aconst_Null);
                return null;
            }
            if (!info.IsConstant) {
                return super.handleExpression(expression, labels, nested);
            }
            var value = info.Value;
            if (info.Type.IsBoolean) {
                if (((Boolean)value).booleanValue()) {
                    generator.emit(Opcode.Iconst_1);
                } else {
                    generator.emit(Opcode.Iconst_0);
                }
                emitBoxing(expression);
                if (labels != null) {
                    generator.emit((info.Negate) ? Opcode.Ifeq : Opcode.Ifne, labels.ifFalse);
                }
            } else {
                switch (info.Type.NumericTypeKind) {
                case Char:
                case Byte:
                case Short:
                case Int:
                    BytecodeHelper.emitIntConstant(generator, value);
                    break;

                case Long:
                    BytecodeHelper.emitLongConstant(generator, value);
                    break;

                case Float:
                    BytecodeHelper.emitFloatConstant(generator, value);
                    break;

                case Double:
                    BytecodeHelper.emitDoubleConstant(generator, value);
                    break;

                default:
                    generator.emit(Opcode.Ldc, value);
                    break;
                }
                emitBoxing(expression);
            }
            return null;
        }
        
        protected override Void handleAnonymousObjectCreation(AnonymousObjectCreationExpressionNode anonymousObject,
                TargetLabels labels, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            var type = anonymousObject.getUserData(typeof(ExpressionInfo)).Type;
            CompilerHelper.initializeAnonymousType(context, type);
            
            var constructor = type.Methods.where(p => p.Name.equals("<init>")).first();
            generator.emit(Opcode.New, type);
            if (nested) {
                generator.emit(Opcode.Dup);
            }
            foreach (var decl in anonymousObject.MemberDeclarators) {
                handleExpression(decl.Value, null, true);
            }
            generator.emit(Opcode.Invokespecial, constructor);
            return null;
        }
        
        protected override Void handleArrayCreation(ArrayCreationExpressionNode arrayCreation, TargetLabels labels, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            foreach (var e in arrayCreation.DimensionExpressions) {
                emitNestedExpression(e, context.TypeSystem.IntType);
            }
            var type = arrayCreation.getUserData(typeof(ExpressionInfo)).Type;
            var initializer = arrayCreation.Initializer;
            int dimensions = arrayCreation.DimensionExpressions.size();
            if (dimensions <= 1) {
                if (dimensions == 0) {
                    BytecodeHelper.emitIntConstant(generator, initializer.Values.size());
                }
            }
            bytecodeGenerator.emitArray(dimensions, type, (initializer == null) ? null : initializer.Values.iterator()); 
            return null;
        }
        
        protected override Void handleArrayInitializer(ArrayInitializerExpressionNode arrayInitializer, TargetLabels labels, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            var type = arrayInitializer.getUserData(typeof(ExpressionInfo)).Type;
            BytecodeHelper.emitIntConstant(generator, arrayInitializer.Values.size());
            bytecodeGenerator.emitArray(0, type, arrayInitializer.Values.iterator()); 
            return null;
        }
        
        protected override Void handleAssign(AssignExpressionNode assign, TargetLabels labels, bool nested) {
            new AssignExpressionGenerator(context, this).handleExpression(assign.Left, assign, nested);
            emitBoxing(assign);
            emitTest(assign.getUserData(typeof(ExpressionInfo)), labels);
            return null;
        }
        
        protected override Void handleBinary(BinaryExpressionNode binary, TargetLabels labels, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            var info = binary.getUserData(typeof(ExpressionInfo));
            var left = binary.LeftOperand;
            var right = binary.RightOperand;
            var li = left.getUserData(typeof(ExpressionInfo));
            var ri = right.getUserData(typeof(ExpressionInfo));
            var leftIsZero = li != null && CompilerHelper.isZero(li);
            var rightIsZero = ri != null && CompilerHelper.isZero(ri);

            switch (binary.Operator) {
            case Add:
                var stringType = context.TypeSystem.StringType;
                if (info.Type == stringType) {
                    var isBuildingString = context.MethodGenerationContext.IsBuildingString;
                    if (!isBuildingString) {
                        generator.emit(Opcode.New, context.StringBuilderType);
                        generator.emit(Opcode.Dup);
                        generator.emit(Opcode.Invokespecial, context.StringBuilderConstructor);
                    }
                    var isString = li != null && li.Type == stringType;
                    var isStringAdd = left.ExpressionKind == ExpressionKind.Binary && isString;
                    context.MethodGenerationContext.IsBuildingString = isStringAdd;
                    handleExpression(left, null, true);
                    if (!isStringAdd || (isString && li.IsConstant)) {
                        BytecodeGenerator.emitStringBuilderAppend(context, li);
                    }
                    isString = ri != null && ri.Type == stringType;
                    isStringAdd = right.ExpressionKind == ExpressionKind.Binary && isString;
                    context.MethodGenerationContext.IsBuildingString = isStringAdd;
                    handleExpression(right, null, true);
                    if (!isStringAdd || (isString && li.IsConstant)) {
                        BytecodeGenerator.emitStringBuilderAppend(context, ri);
                    }
                    if (!isBuildingString) {
                        generator.emit(Opcode.Invokevirtual, context.ObjectToStringMethod);
                    }
                    context.MethodGenerationContext.IsBuildingString = isBuildingString;
                } else {
                    emitOperands(left, false, right, false);
                    switch (info.Type.NumericTypeKind) {
                    case Byte:
                    case Char:
                    case Short:
                    case Int:
                        generator.emit(Opcode.Iadd);
                        break;
                    case Long:
                        generator.emit(Opcode.Ladd);
                        break;
                    case Float:
                        generator.emit(Opcode.Fadd);
                        break;
                    case Double:
                        generator.emit(Opcode.Dadd);
                        break;
                    default:
                        if (BytecodeHelper.isDelegateType(info.Type)) {
                            var delegateType = context.TypeSystem.getType("stab/lang/Delegate");
                            var argTypes = new ArrayList<TypeInfo> { delegateType, delegateType };
                            generator.emit(Opcode.Invokestatic, delegateType.getMethod("combine", argTypes));
                            generator.emit(Opcode.Checkcast, info.Type);
                        } else {
                            throw new Exception("Internal error");
                        }
                        break;
                    }
                    emitBoxing(binary);
                }
                break;
            
            case Subtract:
                emitOperands(left, false, right, false);
                switch (info.getType().NumericTypeKind) {
                case Byte:
                case Char:
                case Short:
                case Int:
                    generator.emit(Opcode.Isub);
                    break;
                case Long:
                    generator.emit(Opcode.Lsub);
                    break;
                case Float:
                    generator.emit(Opcode.Fsub);
                    break;
                case Double:
                    generator.emit(Opcode.Dsub);
                    break;
                default:
                    if (BytecodeHelper.isDelegateType(info.Type)) {
                        var delegateType = context.getTypeSystem().getType("stab/lang/Delegate");
                        var argTypes = new ArrayList<TypeInfo> { delegateType, delegateType };
                        generator.emit(Opcode.Invokestatic, delegateType.getMethod("remove", argTypes));
                        generator.emit(Opcode.Checkcast, info.Type);
                    } else {
                        throw new RuntimeException("Internal error");
                    }
                    break;
                }
                emitBoxing(binary);
                break;

            case Multiply:
                emitOperands(left, false, right, false);
                Opcode opcode;
                switch (info.Type.NumericTypeKind) {
                case Byte:
                case Char:
                case Short:
                case Int:
                    opcode = Opcode.Imul;
                    break;
                case Long:
                    opcode = Opcode.Lmul;
                    break;
                case Float:
                    opcode = Opcode.Fmul;
                    break;
                case Double:
                    opcode = Opcode.Dmul;
                    break;
                default:
                    throw new Exception("Internal error");
                }
                generator.emit(opcode);
                emitBoxing(binary);
                break;
                
            case Divide:
                emitOperands(left, false, right, false);
                switch (info.Type.NumericTypeKind) {
                case Byte:
                case Char:
                case Short:
                case Int:
                    opcode = Opcode.Idiv;
                    break;
                case Long:
                    opcode = Opcode.Ldiv;
                    break;
                case Float:
                    opcode = Opcode.Fdiv;
                    break;
                case Double:
                    opcode = Opcode.Ddiv;
                    break;
                default:
                    throw new Exception("Internal error");
                }
                generator.emit(opcode);
                emitBoxing(binary);
                break;
                
            case Modulo:
                emitOperands(left, false, right, false);
                switch (info.Type.NumericTypeKind) {
                case Byte:
                case Char:
                case Short:
                case Int:
                    opcode = Opcode.Irem;
                    break;
                case Long:
                    opcode = Opcode.Lrem;
                    break;
                case Float:
                    opcode = Opcode.Frem;
                    break;
                case Double:
                    opcode = Opcode.Drem;
                    break;
                default:
                    throw new RuntimeException("Internal error");
                }
                generator.emit(opcode);
                emitBoxing(binary);
                break;

            case And:
                emitOperands(left, false, right, false);
                if (info.Type.IsBoolean) {
                    opcode = Opcode.Iand;
                } else {
                    switch (info.Type.NumericTypeKind) {
                    case Byte:
                    case Char:
                    case Short:
                    case Int:
                        opcode = Opcode.Iand;
                        break;
                    case Long:
                        opcode = Opcode.Land;
                        break;
                    default:
                        throw new Exception("Internal error");
                    }
                }
                generator.emit(opcode);
                emitBoxing(binary);
                break;
                
            case Or:
                emitOperands(left, false, right, false);
                if (info.Type.IsBoolean) {
                    opcode = Opcode.Ior;
                } else {
                    switch (info.Type.NumericTypeKind) {
                    case Byte:
                    case Char:
                    case Short:
                    case Int:
                        opcode = Opcode.Ior;
                        break;
                    case Long:
                        opcode = Opcode.Lor;
                        break;
                    default:
                        throw new Exception("Internal error");
                    }
                }
                generator.emit(opcode);
                emitBoxing(binary);
                break;
                
            case Xor:
                emitOperands(left, false, right, false);
                if (info.Type.IsBoolean) {
                    opcode = Opcode.Ixor;
                } else {
                    switch (info.Type.NumericTypeKind) {
                    case Byte:
                    case Char:
                    case Short:
                    case Int:
                        opcode = Opcode.Ixor;
                        break;
                    case Long:
                        opcode = Opcode.Lxor;
                        break;
                    default:
                        throw new Exception("Internal error");
                    }
                }
                generator.emit(opcode);
                emitBoxing(binary);
                break;
                
            case LessThan:
                if (labels != null) {
                    if (info.Negate) {
                        emitComparison(binary, leftIsZero, rightIsZero, Opcode.Ifle, Opcode.Ifge, Opcode.If_icmpge, labels.ifFalse);
                    } else {
                        emitComparison(binary, leftIsZero, rightIsZero, Opcode.Ifgt, Opcode.Iflt, Opcode.If_icmplt, labels.ifFalse);
                    }
                } else {
                    var elseLabel = generator.defineLabel();
                    var endLabel = generator.defineLabel();
                    if (info.Negate) {
                        emitComparison(binary, leftIsZero, rightIsZero, Opcode.Ifle, Opcode.Ifge, Opcode.If_icmpge, elseLabel);
                    } else {
                        emitComparison(binary, leftIsZero, rightIsZero, Opcode.Ifgt, Opcode.Iflt, Opcode.If_icmplt, elseLabel);
                    }
                    generator.emit(Opcode.Iconst_1);
                    generator.emit(Opcode.Goto, endLabel);
                    generator.markLabel(elseLabel);
                    generator.emit(Opcode.Iconst_0);
                    generator.markLabel(endLabel);
                }
                emitBoxing(binary);
                break;
                
            case LessThanOrEqual:
                if (labels != null) {
                    if (info.Negate) {
                        emitComparison(binary, leftIsZero, rightIsZero, Opcode.Iflt, Opcode.Ifgt, Opcode.If_icmpgt, labels.ifFalse);
                    } else {
                        emitComparison(binary, leftIsZero, rightIsZero, Opcode.Ifge, Opcode.Ifle, Opcode.If_icmple, labels.ifFalse);
                    }
                } else {
                    var elseLabel = generator.defineLabel();
                    var endLabel = generator.defineLabel();
                    if (info.Negate) {
                        emitComparison(binary, leftIsZero, rightIsZero, Opcode.Iflt, Opcode.Ifgt, Opcode.If_icmpgt, elseLabel);
                    } else {
                        emitComparison(binary, leftIsZero, rightIsZero, Opcode.Ifge, Opcode.Ifle, Opcode.If_icmple, elseLabel);
                    }
                    generator.emit(Opcode.Iconst_1);
                    generator.emit(Opcode.Goto, endLabel);
                    generator.markLabel(elseLabel);
                    generator.emit(Opcode.Iconst_0);
                    generator.markLabel(endLabel);
                }
                emitBoxing(binary);
                break;
                
            case GreaterThan:
                if (labels != null) {
                    if (info.Negate) {
                        emitComparison(binary, leftIsZero, rightIsZero, Opcode.Ifge, Opcode.Ifle, Opcode.If_icmple, labels.ifFalse);
                    } else {
                        emitComparison(binary, leftIsZero, rightIsZero, Opcode.Iflt, Opcode.Ifgt, Opcode.If_icmpgt, labels.ifFalse);
                    }
                } else {
                    var elseLabel = generator.defineLabel();
                    var endLabel = generator.defineLabel();
                    if (info.Negate) {
                        emitComparison(binary, leftIsZero, rightIsZero, Opcode.Ifge, Opcode.Ifle, Opcode.If_icmple, elseLabel);
                    } else {
                        emitComparison(binary, leftIsZero, rightIsZero, Opcode.Iflt, Opcode.Ifgt, Opcode.If_icmpgt, elseLabel);
                    }
                    generator.emit(Opcode.Iconst_1);
                    generator.emit(Opcode.Goto, endLabel);
                    generator.markLabel(elseLabel);
                    generator.emit(Opcode.Iconst_0);
                    generator.markLabel(endLabel);
                }
                emitBoxing(binary);
                break;
                
            case GreaterThanOrEqual:
                if (labels != null) {
                    if (info.Negate) {
                        emitComparison(binary, leftIsZero, rightIsZero, Opcode.Ifgt, Opcode.Iflt, Opcode.If_icmplt, labels.ifFalse);
                    } else {
                        emitComparison(binary, leftIsZero, rightIsZero, Opcode.Ifle, Opcode.Ifge, Opcode.If_icmpge, labels.ifFalse);
                    }
                } else {
                    var elseLabel = generator.defineLabel();
                    var endLabel = generator.defineLabel();
                    if (info.Negate) {
                        emitComparison(binary, leftIsZero, rightIsZero, Opcode.Ifgt, Opcode.Iflt, Opcode.If_icmplt, elseLabel);
                    } else {
                        emitComparison(binary, leftIsZero, rightIsZero, Opcode.Ifle, Opcode.Ifge, Opcode.If_icmpge, elseLabel);
                    }
                    generator.emit(Opcode.Iconst_1);
                    generator.emit(Opcode.Goto, endLabel);
                    generator.markLabel(elseLabel);
                    generator.emit(Opcode.Iconst_0);
                    generator.markLabel(endLabel);
                }
                emitBoxing(binary);
                break;
                
            case Equal:
                if (labels != null) {
                    emitEquality(binary, leftIsZero, rightIsZero, labels.ifFalse, info.Negate);
                } else {
                    var elseLabel = generator.defineLabel();
                    var endLabel = generator.defineLabel();
                    emitEquality(binary, leftIsZero, rightIsZero, elseLabel, info.Negate);
                    generator.emit(Opcode.Iconst_1);
                    generator.emit(Opcode.Goto, endLabel);
                    generator.markLabel(elseLabel);
                    generator.emit(Opcode.Iconst_0);
                    generator.markLabel(endLabel);
                }
                emitBoxing(binary);
                break;
                
            case NotEqual:
                if (labels != null) {
                    emitEquality(binary, leftIsZero, rightIsZero, labels.ifFalse, !info.Negate);
                } else {
                    var elseLabel = generator.defineLabel();
                    var endLabel = generator.defineLabel();
                    emitEquality(binary, leftIsZero, rightIsZero, elseLabel, !info.Negate);
                    generator.emit(Opcode.Iconst_1);
                    generator.emit(Opcode.Goto, endLabel);
                    generator.markLabel(elseLabel);
                    generator.emit(Opcode.Iconst_0);
                    generator.markLabel(endLabel);
                }
                emitBoxing(binary);
                break;
            
            case LogicalAnd:
                if (labels != null) {
                    if (info.Negate) {
                        var then2Label = generator.defineLabel();
                        handleExpression(left, new TargetLabels(then2Label, labels.ifFalse), true);
                        generator.markLabel(then2Label);
                        handleExpression(right, labels, true);
                    } else {
                        var then2Label = generator.defineLabel();
                        handleExpression(left, new TargetLabels(then2Label, labels.ifTrue), true);
                        ri.Negate = false;
                        handleExpression(right, labels, true);
                        generator.markLabel(then2Label);
                    }
                } else {
                    var thenLabel = generator.defineLabel();
                    var elseLabel = generator.defineLabel();
                    var endLabel = generator.defineLabel();
                    handleExpression(left, new TargetLabels(thenLabel, elseLabel), true);
                    handleExpression(right, new TargetLabels(thenLabel, elseLabel), true);
                    generator.markLabel(thenLabel);
                    generator.emit(Opcode.Iconst_1);
                    generator.emit(Opcode.Goto, endLabel);
                    generator.markLabel(elseLabel);
                    generator.emit(Opcode.Iconst_0);
                    generator.markLabel(endLabel);
                }
                emitBoxing(binary);
                break;
                
            case LogicalOr: {
                if (labels != null) {
                    if (info.Negate) {
                        var then2Label = generator.defineLabel();
                        li.Negate = false;
                        handleExpression(left, new TargetLabels(then2Label, labels.ifTrue), true);
                        generator.markLabel(then2Label);
                        handleExpression(right, labels, true);
                    } else {
                        var then2Label = generator.defineLabel();
                        li.Negate = false;
                        handleExpression(left, new TargetLabels(then2Label, labels.ifFalse), true);
                        generator.markLabel(then2Label);
                        ri.Negate = false;
                        handleExpression(right, labels, true);
                    }
                } else {
                    var thenLabel = generator.defineLabel();
                    var then2Label = generator.defineLabel();
                    var elseLabel = generator.defineLabel();
                    var endLabel = generator.defineLabel();
                    li.Negate = false;
                    handleExpression(left, new TargetLabels(then2Label, thenLabel), true);
                    generator.markLabel(then2Label);
                    handleExpression(right, new TargetLabels(thenLabel, elseLabel), true);
                    generator.markLabel(thenLabel);
                    generator.emit(Opcode.Iconst_1);
                    generator.emit(Opcode.Goto, endLabel);
                    generator.markLabel(elseLabel);
                    generator.emit(Opcode.Iconst_0);
                    generator.markLabel(endLabel);
                }
                emitBoxing(binary);
                break;
            }
            case LeftShift:
                handleExpression(left, null, true);
                handleExpression(right, null, true);
                if (info.Type == context.TypeSystem.IntType) {
                    generator.emit(Opcode.Ishl);
                } else if (info.Type == context.TypeSystem.LongType) {
                    generator.emit(Opcode.Lshl);
                } else {
                    throw new Exception("Internal error");
                }
                emitBoxing(binary);
                break;
                
            case RightShift:
                handleExpression(left, null, true);
                handleExpression(right, null, true);
                if (info.Type == context.TypeSystem.IntType) {
                    generator.emit(Opcode.Ishr);
                } else if (info.Type == context.TypeSystem.LongType) {
                    generator.emit(Opcode.Lshr);
                } else {
                    throw new Exception("Internal error");
                }
                emitBoxing(binary);
                break;
                
            case UnsignedRightShift:
                handleExpression(left, null, true);
                handleExpression(right, null, true);
                if (info.Type == context.TypeSystem.IntType) {
                    generator.emit(Opcode.Iushr);
                } else if (info.Type == context.TypeSystem.LongType) {
                    generator.emit(Opcode.Lushr);
                } else {
                    throw new Exception("Internal error");
                }
                emitBoxing(binary);
                break;
                
            case Instanceof:
                handleExpression(left, null, true);
                generator.emit(Opcode.Instanceof, ri.Type);
                emitBoxing(binary);
                if (labels != null) {
                    generator.emit(Opcode.Ifeq, labels.ifFalse);
                }
                break;
                
            case As: {
                handleExpression(left, null, true);
                generator.emit(Opcode.Dup);
                generator.emit(Opcode.Instanceof, ri.Type);
                var elseLabel = generator.defineLabel();
                var endLabel = generator.defineLabel();
                generator.emit(Opcode.Ifne, elseLabel);
                generator.emit(Opcode.Pop);
                generator.emit(Opcode.Aconst_Null);
                generator.emit(Opcode.Goto, endLabel);
                generator.markLabel(elseLabel);
                generator.emit(Opcode.Checkcast, ri.Type);
                generator.markLabel(endLabel);
                emitBoxing(binary);
                break;
            }				
            case NullCoalescing: {
                handleExpression(left, null, true);
                generator.emit(Opcode.Dup);
                var endLabel = generator.defineLabel();
                generator.emit(Opcode.Ifnonnull, endLabel);
                generator.emit(Opcode.Pop);
                handleExpression(right, null, true);
                generator.markLabel(endLabel);
                emitBoxing(binary);
                break;
            }
            default:
                throw new Exception("Internal error");
            }
            return null;
        }

        protected override Void handleCast(CastExpressionNode cast, TargetLabels labels, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            handleExpression(cast.Expression, null, nested);
            var einfo = cast.Expression.getUserData(typeof(ExpressionInfo));
            var info = cast.getUserData(typeof(ExpressionInfo));
            var type = info.Type;
            if (einfo != null && einfo.Type != type) {
                if (einfo.Type.IsNumeric) {
                    BytecodeHelper.emitNumericConversion(generator, einfo.Type, type);
                } else if (info.OriginalType != null) {
                    generator.emit(Opcode.Checkcast, info.OriginalType);
                } else if (info.Type.OriginalTypeDefinition != einfo.Type.OriginalTypeDefinition) {
                    if (!info.Type.IsGenericParameter || info.Type.GenericParameterBounds.any()) {
                        if (type.IsPrimitive) {
                            if (type.TypeKind == TypeKind.Boolean) {
                                var method = context.TypeSystem.getUnboxingMethod(type);
                                generator.emit(Opcode.Checkcast, method.DeclaringType);
                                generator.emit(Opcode.Invokevirtual, method);
                            } else {
                                if (einfo.Type.IsPrimitive) {
                                    BytecodeHelper.emitNumericConversion(generator, einfo.Type, type);
                                } else {
                                    var method = context.TypeSystem.getUnboxingMethod(type);
                                    generator.emit(Opcode.Checkcast, method.DeclaringType);
                                    generator.emit(Opcode.Invokevirtual, method);
                                }
                            }
                        } else {
                            generator.emit(Opcode.Checkcast, type);
                        }
                    }
                }
            }
            emitBoxing(cast);
            emitTest(info, labels);
            return null;
        }
        
        protected override Void handleConditional(ConditionalExpressionNode conditional, TargetLabels labels, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            var thenLabel = generator.defineLabel();
            var elseLabel = generator.defineLabel();
            var endLabel = generator.defineLabel();
            var info = conditional.getUserData(typeof(ExpressionInfo));
            handleExpression(conditional.Condition, new TargetLabels(thenLabel, elseLabel), true);
            generator.markLabel(thenLabel);
            handleExpression(conditional.IfTrue, null, true);
            BytecodeGenerator.emitConversion(context, info.Type, conditional.IfTrue);
            generator.emit(Opcode.Goto, endLabel);
            generator.markLabel(elseLabel);
            handleExpression(conditional.IfFalse, null, true);
            BytecodeGenerator.emitConversion(context, info.Type, conditional.IfFalse);
            generator.markLabel(endLabel);
            emitBoxing(conditional);
            emitTest(info, labels);
            return null;
        }
        
        protected override Void handleElementAccess(ElementAccessExpressionNode elementAccess, TargetLabels labels, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            var target = elementAccess.TargetObject;
            var tinfo = target.getUserData(typeof(ExpressionInfo));
            var ttype = tinfo.Type;
            if (!ttype.IsArray) {
                var method = elementAccess.getUserData(typeof(ExpressionInfo)).Member.GetAccessor;
                if (!method.IsStatic) {
                    handleExpression(target, null, true);
                }
                var arguments = elementAccess.Indexes;
                emitArguments(arguments, method.Parameters, method.Parameters.count(), method.IsVarargs);
                CompilerHelper.emitIndexerAccess(context, method);
                BytecodeGenerator.emitGenericCast(context, method.ReturnType, method.OriginalMethodDefinition.ReturnType);
            } else {
                handleExpression(target, null, true);
                var index = elementAccess.Indexes[0];
                handleExpression(index, null, true);
                BytecodeHelper.emitNumericConversion(generator, index.getUserData(typeof(ExpressionInfo)).Type, context.TypeSystem.IntType);
                generator.emit(BytecodeHelper.getAloadOpcode(ttype.ElementType));
            }
            emitBoxing(elementAccess);
            emitTest(elementAccess.getUserData(typeof(ExpressionInfo)), labels);
            return null;
        }
        
        protected override Void handleInvocation(InvocationExpressionNode invocation, TargetLabels labels, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            var info = invocation.getUserData(typeof(ExpressionInfo));
            var method = info.Method;
            if (method.IsExcludedFromCompilation || CompilerHelper.shouldIgnoreCalls(context, method)) {
                return null;
            }
            if (!method.IsStatic) {
                if (BytecodeHelper.isDelegateType(method.DeclaringType) && method.Name.equals("invoke")) {
                    handleExpression(invocation.TargetObject, null, true);
                } else if (invocation.TargetObject.ExpressionKind == ExpressionKind.MemberAccess || invocation.TargetObject.ExpressionKind == ExpressionKind.NullSafeMemberAccess) {
                    var targetTarget = ((MemberAccessExpressionNode)invocation.TargetObject).TargetObject;
                    handleExpression(targetTarget, null, true);
                    BytecodeGenerator.emitGenericCast(context, method.DeclaringType, targetTarget.getUserData(typeof(ExpressionInfo)).Type);
                } else { // SimpleName
                    BytecodeGenerator.emitThisAccess(context, generator);
                }
            }
            var arguments = invocation.Arguments;
            if (info.IsExtension) {
                var target = (MemberAccessExpressionNode)invocation.TargetObject;
                arguments = new ArrayList<ExpressionNode> { target.TargetObject };
                arguments.addAll(invocation.Arguments);
            }
            emitArguments(arguments, method.Parameters, method.Parameters.count(), method.IsVarargs);
            CompilerHelper.emitMethodAccess(context, method, invocation.TargetObject.getUserData(typeof(ExpressionInfo)).IsSuperCall);
            if (nested) {
                var gm = method.OriginalMethodDefinition;
                if (method != gm) {
                    BytecodeGenerator.emitGenericCast(context, method.ReturnType, gm.ReturnType);
                }
            } else {
                switch (method.ReturnType.TypeKind) {
                case Long:
                case Double:
                    generator.emit(Opcode.Pop2);
                    break;
                case Void:
                    break;
                default:
                    generator.emit(Opcode.Pop);
                    break;
                }
            }
            emitBoxing(invocation);
            emitTest(info, labels);
            return null;
        }
        
        protected override Void handleLambda(LambdaExpressionNode lambda, TargetLabels labels, bool nested) {
            var targetType = lambda.getUserData(typeof(ExpressionInfo)).Type;
            var typeBuilder = context.MethodGenerationContext.LambdaScope;

            if (targetType.FullName.equals("stab/tree/ExpressionTree")) {
                expressionTreeGenerator.generateExpressionTree(lambda);
                return null;
            }
 
            var methodBuilder = lambda.getUserData(typeof(MethodBuilder));
            context.MethodGenerationContext.enterLambda(methodBuilder);
            var gen = methodBuilder.CodeGenerator;
            var oldIsLambdaScopeUsed = context.MethodGenerationContext.IsLambdaScopeUsed;
            context.MethodGenerationContext.IsLambdaScopeUsed = false;
            gen.beginScope();
            bytecodeGenerator.handleStatement(lambda.Body, null);
            gen.endScope();
            var isLambdaScopeUsed = context.MethodGenerationContext.IsLambdaScopeUsed;
            if (!isLambdaScopeUsed) {
                gen.removeThis();
                methodBuilder.setStatic(true);
                typeBuilder.undefineMethod(methodBuilder);
                var currentType = (TypeBuilder)context.MethodGenerationContext.RootMethod.DeclaringType;
                currentType.defineMethod(methodBuilder);
                if (typeBuilder.Methods.count() == 1) {
                	context.TypeBuilders.remove(typeBuilder);
                	currentType.undefineNestedType(typeBuilder);
                }
            }
            context.MethodGenerationContext.leaveLambda();
            context.MethodGenerationContext.IsLambdaScopeUsed = oldIsLambdaScopeUsed;
            if (isLambdaScopeUsed) {
                BytecodeGenerator.getLambdaScope(context, context.MethodGenerationContext.CurrentMethod);
            }
            
            ExpressionNode arg = null;
            if (isLambdaScopeUsed) {
                arg = new SimpleNameExpressionNode();
                var ainfo = new ExpressionInfo(context.MethodGenerationContext.LambdaScope);
                if (context.MethodGenerationContext.IsInLambda) {
                    ainfo.Member = new LocalMemberInfo("this", typeBuilder, context.MethodGenerationContext.CurrentMethod, false);
                } else {
                    ainfo.Member = new LocalMemberInfo("lambda$scope", typeBuilder, context.MethodGenerationContext.CurrentMethod, false);
                }
                arg.addUserData(ainfo);
            }
            if (BytecodeHelper.isDelegateType(targetType)) {
                emitDelegateCreation(targetType, methodBuilder, arg, nested);
            } else {
                emitInterfaceCreation(targetType, methodBuilder, arg, nested);
            }
            return null;
        }
        
        protected override Void handleMemberAccess(MemberAccessExpressionNode memberAccess, TargetLabels labels, bool nested) {
            var info = memberAccess.getUserData(typeof(ExpressionInfo));
            if (info.Method != null) {
                emitDelegateCreation(info.Type, info.Method, memberAccess, nested);
                return null;
            }
            var member = info.Member;
            if (member == null) {
                handleExpression(memberAccess.TargetObject, null, true);
                return null;
            }
            switch (member.MemberKind) {
            case Field: {
                var field = member.Field;
                if (!field.IsStatic) {
                    handleExpression(memberAccess.TargetObject, null, true);
                }
                CompilerHelper.emitFieldAccess(context, field);
                BytecodeGenerator.emitGenericCast(context, field.Type, field.DeclaringType.OriginalTypeDefinition.getField(field.Name).Type);
                emitBoxing(memberAccess);
                emitTest(info, labels);
                return null;
            }
            case Property: {
                var method = member.GetAccessor;
                if (!method.IsStatic) {
                    handleExpression(memberAccess.TargetObject, null, true);
                }
                CompilerHelper.emitPropertyAccess(context, method);
                BytecodeGenerator.emitGenericCast(context, method.ReturnType, method.OriginalMethodDefinition.ReturnType);
                emitBoxing(memberAccess);
                emitTest(info, labels);
                return null;
            }
            default:
                throw new IllegalStateException("Internal error: unhandled member kind: " + member.MemberKind);
            }
        }

        protected override Void handleObjectCreation(ObjectCreationExpressionNode objectCreation, TargetLabels labels, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            var info = objectCreation.getUserData(typeof(ExpressionInfo));
            var method = info.Method;
            if (BytecodeHelper.isDelegateType(info.Type)) {
                emitDelegateCreation(info.Type, method, objectCreation.Arguments[0], nested);
                return null;
            }
            if (CompilerHelper.requireAccessor(context, method.DeclaringType, method.IsPublic, method.IsProtected, method.IsPrivate)) {
                var accessor = context.PrivateAccessors[method];
                if (accessor == null) {
                    var outerClass = (TypeBuilder)context.CurrentType;
                    while ((outerClass = (TypeBuilder)outerClass.DeclaringType) != null) {
                        if (method.DeclaringType.isAssignableFrom(outerClass)) {
                            break;
                        }
                    }
                    int n = outerClass.Methods.count(p => p.Name.startsWith("accessor$"));
                    var methodBuilder = outerClass.defineMethod("accessor$" + n);
                    methodBuilder.setReturnType(method.DeclaringType);
                    methodBuilder.setStatic(true);
                    methodBuilder.setSynthetic(true);
                    methodBuilder.setVarargs(method.IsVarargs);
                    foreach (var p in method.Parameters) {
                        var pb = methodBuilder.addParameter(p.Type);
                        pb.setName(p.Name);
                    }
                    foreach (var t in method.Exceptions) {
                        methodBuilder.addException(t);
                    }
                    var gen = methodBuilder.CodeGenerator;
                    gen.beginScope();
                    gen.emit(Opcode.New, method.DeclaringType);
                    gen.emit(Opcode.Dup);
                    foreach (var p in method.Parameters) {
                        gen.emit(BytecodeHelper.getLoadOpcode(p.Type), gen.getLocal(p.Name));
                    }
                    gen.emit(Opcode.Invokespecial, method);
                    gen.emit(Opcode.Areturn);
                    gen.endScope();
                    accessor = methodBuilder;
                }
                method = accessor;
            }
            if (method != info.Method) {
                emitArguments(objectCreation.Arguments, method.Parameters, method.Parameters.count(), method.IsVarargs);
                generator.emit(Opcode.Invokestatic, method);
                if (!nested) {
                    generator.emit(Opcode.Pop);
                }
            } else {
                generator.emit(Opcode.New, method.DeclaringType);
                if (nested) {
                    generator.emit(Opcode.Dup);
                }
                emitArguments(objectCreation.Arguments, method.Parameters, method.Parameters.count(), method.IsVarargs);
                generator.emit(Opcode.Invokespecial, method);
            }
            emitBoxing(objectCreation);
            var init = objectCreation.Initializer;
            if (init != null) {
                if (init.ExpressionKind == ExpressionKind.ObjectInitializer) {
                    var initializer = (ObjectInitializerExpressionNode)init;
                    foreach (var mi in initializer.MemberInitializers) {
                        MemberInfo memb = mi.getUserData(typeof(MemberInfo));
                        if (!memb.IsStatic) {
                            generator.emit(Opcode.Dup);
                        }
                        handleExpression(mi.Value, null, true);
                        emitBoxing(mi.Value);
                        if (memb.MemberKind == MemberKind.Property) {
                            CompilerHelper.emitPropertyOrIndexerModification(context, memb.SetAccessor);
                            if (memb.SetAccessor.ReturnType != context.TypeSystem.VoidType) {
                                generator.emit((memb.SetAccessor.ReturnType.IsCategory2) ? Opcode.Pop2 : Opcode.Pop);
                            }
                        } else {
                            CompilerHelper.emitFieldModification(context, memb.Field);
                        }
                    }
                } else {
                    var initializer = (CollectionInitializerExpressionNode)init;
                    var addMethod = initializer.getUserData(typeof(MethodInfo));
                    foreach (var args in initializer.Values) {
                        generator.emit(Opcode.Dup);
                        foreach (var e in args) {
                            handleExpression(e, null, true);
                        }
                        CompilerHelper.emitMethodAccess(context, addMethod, false);
                        if (addMethod.ReturnType != context.TypeSystem.VoidType) {
                            generator.emit((addMethod.ReturnType.IsCategory2) ? Opcode.Pop2 : Opcode.Pop);
                        }
                    }
                }
            }
            emitTest(info, labels);
            return null;
        }
        
        protected override Void handleSimpleName(SimpleNameExpressionNode simpleName, TargetLabels labels, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            var info = simpleName.getUserData(typeof(ExpressionInfo));
            var member = info.Member;
            switch (member.MemberKind) {
            case Local: {
                var local = (LocalMemberInfo)member;
                if (local.IsUsedFromLambda) {
                    BytecodeGenerator.emitLoadLambdaScope(context, generator, local.Method);
                    generator.emit(Opcode.Getfield, BytecodeGenerator.getLambdaScopeField(context, local));
                } else {
                    generator.emit(BytecodeHelper.getLoadOpcode(local.Type), generator.getLocal(local.Name));
                }
                emitBoxing(simpleName);
                emitTest(info, labels);
                return null;
            }
            case Field: {
                var field = member.Field;
                if (!field.IsStatic) {
                    BytecodeGenerator.emitThisAccess(context, generator);
                }
                CompilerHelper.emitFieldAccess(context, field);
                BytecodeGenerator.emitGenericCast(context, field.Type, field.DeclaringType.OriginalTypeDefinition.getField(field.Name).Type);
                emitBoxing(simpleName);
                emitTest(info, labels);
                return null;
            }
            case Method: {
                if (info.Method != null) {
                    var e = new ThisAccessExpressionNode();
                    e.addUserData(new ExpressionInfo(context.CurrentType));
                    emitDelegateCreation(info.Type, info.Method, e, nested);
                } else {
                    BytecodeGenerator.emitThisAccess(context, generator);
                    emitBoxing(simpleName);
                }
                return null;
            }
            case Property: {
                var method = member.GetAccessor;
                if (!method.IsStatic) {
                    BytecodeGenerator.emitThisAccess(context, generator);
                }
                CompilerHelper.emitPropertyAccess(context, method);
                BytecodeGenerator.emitGenericCast(context, method.ReturnType, method.OriginalMethodDefinition.ReturnType);
                emitBoxing(simpleName);
                emitTest(info, labels);
                return null;
            }
            default:
                throw new Exception("Internal error: unhandled name kind: " + member.MemberKind);
            }
        }

        protected override Void handleSizeof(SizeofExpressionNode sizeofExpression, TargetLabels labels, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            handleExpression(sizeofExpression.Expression, null, true);
            generator.emit(Opcode.Arraylength);
            return null;
        }
        
        protected override Void handleSuperAccess(SuperAccessExpressionNode superAccess, TargetLabels labels, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            BytecodeGenerator.emitThisAccess(context, generator);
            return null;
        }
        
        protected override Void handleThisAccess(ThisAccessExpressionNode thisAccess, TargetLabels labels, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            BytecodeGenerator.emitThisAccess(context, generator);
            return null;
        }
        
        protected override Void handleTypeof(TypeofExpressionNode typeofExpression, TargetLabels labels, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            var type = typeofExpression.getUserData(typeof(TypeInfo));
            BytecodeHelper.emitTypeof(generator, context.TypeSystem, type);
            return null;
        }
        
        protected override Void handleUnary(UnaryExpressionNode unary, TargetLabels labels, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            var info = unary.getUserData(typeof(ExpressionInfo));
            var type = info.Type;
            switch (unary.Operator) {
            case Complement: {
                handleExpression(unary.Operand, null, true);
                Opcode opcode;
                switch (type.NumericTypeKind) {
                case Byte:
                case Char:
                case Short:
                case Int:
                    generator.emit(Opcode.Iconst_M1);
                    opcode = Opcode.Ixor;
                    break;
                    
                case Long:
                    generator.emit(Opcode.Ldc, new Long(-1));
                    opcode = Opcode.Lxor;
                    break;
                    
                default:
                    throw new Exception("Internal error");
                }
                generator.emit(opcode);
                emitBoxing(unary);
                break;
            }
            case Minus: {
                handleExpression(unary.Operand, null, true);
                Opcode opcode;
                switch (type.NumericTypeKind) {
                case Byte:
                case Char:
                case Int:
                case Short:
                    opcode = Opcode.Ineg;
                    break;
                case Long:
                    opcode = Opcode.Lneg;
                    break;
                case Double:
                    opcode = Opcode.Dneg;
                    break;
                case Float:
                    opcode = Opcode.Fneg;
                    break;
                default:
                    throw new Exception("Internal error");
                }
                generator.emit(opcode);
                emitBoxing(unary);
                break;
            }
            case Not: {
                handleExpression(unary.Operand, null, true);
                if (labels == null) {
                    var elseLabel = generator.defineLabel();
                    var endLabel = generator.defineLabel();
                    generator.emit(Opcode.Ifeq, elseLabel);
                    generator.emit(Opcode.Iconst_0);
                    generator.emit(Opcode.Goto, endLabel);
                    generator.markLabel(elseLabel);
                    generator.emit(Opcode.Iconst_1);
                    generator.markLabel(endLabel);
                } else {
                    if (info.Negate) {
                        generator.emit(Opcode.Ifne, labels.ifFalse);
                    } else {
                        generator.emit(Opcode.Ifeq, labels.ifFalse);
                    }
                }
                emitBoxing(unary);
                break;
            }
            case Plus: {
                handleExpression(unary.Operand, null, true);
                switch (type.NumericTypeKind) {
                case Byte:
                case Int:
                case Short:
                case Long:
                case Double:
                case Float:
                    break;
                default:
                    throw new Exception("Internal error");
                }
                emitBoxing(unary);
                break;
            }
            case PostIncrement:
            case PostDecrement:
            case PreDecrement:
            case PreIncrement: {
                new AssignExpressionGenerator(context, this).handleExpression(unary.Operand, unary, nested);
                emitBoxing(unary);
                break;
            }
            default:
                throw new RuntimeException("Internal error: unhandled unary operator " + unary.Operator);
            }
            return null;
        }
        
        private void emitTest(ExpressionInfo info, TargetLabels labels) {
            if (labels != null) {
                var generator = context.MethodGenerationContext.Generator;
                generator.emit((info.Negate) ? Opcode.Ifeq : Opcode.Ifne, labels.ifFalse);
            }
        }

        private void emitBoxing(ExpressionNode expression) {
            BytecodeGenerator.emitBoxing(context, expression);
        }
        
        void emitNestedExpression(ExpressionNode expression, TypeInfo targetType) {
            handleExpression(expression, null, true);
            BytecodeGenerator.emitConversion(context, targetType, expression);
        }
        
        void emitArguments(List<ExpressionNode> arguments, Iterable<ParameterInfo> parameters, int nparams, bool varargs) {
            int fixedLength = (varargs) ?  nparams - 1 : nparams;
            var generator = context.MethodGenerationContext.Generator;
            var it1 = parameters.iterator();
            var it2 = arguments.iterator();
            int i;
            for (i = 0; i < fixedLength; i++) {
                var p = it1.next();
                var e = it2.next();
                emitNestedExpression(e, p.Type);
            }
            if (varargs) {
                int nvarargs = arguments.size() - fixedLength;
                if (nvarargs == 1) {
                    var paramType = it1.next().Type;
                    var e = arguments[i];
                    var ei = e.getUserData(typeof(ExpressionInfo));
                    if (ei == null) {
                        generator.emit(Opcode.Aconst_Null);
                    } else if (ei.Type.IsArray && paramType.isAssignableFrom(ei.Type)) {
                        handleExpression(e, null, true);
                        BytecodeGenerator.emitConversion(context, paramType, e);
                    } else {
                        BytecodeHelper.emitIntConstant(generator, 1);
                        bytecodeGenerator.emitArray(1, paramType, it2);
                    }
                } else {
                    BytecodeHelper.emitIntConstant(generator, nvarargs);
                    bytecodeGenerator.emitArray(1, it1.next().Type, it2);
                }
            }
        }

        void emitDelegateCreation(TypeInfo delegateType, MethodInfo method, ExpressionNode arg, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            if (arg != null) {
                var argType = arg.getUserData(typeof(ExpressionInfo)).Type;
                if (argType != null && BytecodeHelper.isDelegateType(argType)) {
                    handleExpression(arg, null, nested);
                    return;
                }
            }
            
            var typeInfo = CompilerHelper.createDelegateType(context, delegateType, method);
            generator.emit(Opcode.New, typeInfo);
            if (nested) {
                generator.emit(Opcode.Dup);
            }
            if (!method.IsStatic) {
                handleExpression(arg, null, true);
            }
            generator.emit(Opcode.Invokespecial, typeInfo.Methods.where(p => p.Name.equals("<init>")).first());
        }

        private void emitInterfaceCreation(TypeInfo interfaceType, MethodInfo method, ExpressionNode arg, bool nested) {
            var generator = context.MethodGenerationContext.Generator;
            var currentType = (TypeBuilder)context.CurrentType;
            var prefix = currentType.getFullName() + "$Interface";
            int n = currentType.NestedTypes.count(p => p.FullName.startsWith(prefix));
            var typeBuilder = currentType.defineNestedType("Interface" + n);
            context.TypeBuilders.add(typeBuilder);
            typeBuilder.setSynthetic(true);
            var objectType = context.TypeSystem.getType("java/lang/Object");
            typeBuilder.setBaseType(objectType);
            typeBuilder.addInterface(interfaceType);

            FieldBuilder targetField = null;
            if (!method.IsStatic) {
                targetField = typeBuilder.defineField("target", method.DeclaringType);
                targetField.setPrivate(true);
            }

            var interfaceMethod = interfaceType.Methods.single();
            
            // Interface method
            var methodBuilder = typeBuilder.defineMethod(interfaceMethod.Name);
            methodBuilder.setPublic(true);
            methodBuilder.setFinal(true);
            methodBuilder.setVarargs(method.IsVarargs);
            methodBuilder.setReturnType(method.ReturnType);
            foreach (var p in method.Parameters) {
                var pb = methodBuilder.addParameter(p.Type);
                pb.setName(p.Name);
            }
            foreach (var t in method.Exceptions) {
                methodBuilder.addException(t);
            }
            
            var gen = methodBuilder.CodeGenerator;
            gen.beginScope();
            if (!method.IsStatic) {
                gen.emit(Opcode.Aload, gen.getLocal("this"));
                gen.emit(Opcode.Getfield, targetField);
            }
            foreach (var p in method.Parameters) {
                gen.emit(BytecodeHelper.getLoadOpcode(p.getType()), gen.getLocal(p.getName()));
            }
            gen.emit((method.IsStatic) ? Opcode.Invokestatic : Opcode.Invokevirtual, method);
            gen.emit(BytecodeHelper.getReturnOpcode(method.ReturnType));
            gen.endScope();

            // Bridge
            if (interfaceMethod.DeclaringType.GenericArguments.any()) {
                BytecodeGenerator.emitBridgeMethod(context, typeBuilder, methodBuilder, interfaceMethod);
            }
            
            // Constructor T(scope)
            methodBuilder = typeBuilder.defineMethod("<init>");
            methodBuilder.setReturnType(context.TypeSystem.VoidType);
            if (!method.IsStatic) {
                var parameterBuilder = methodBuilder.addParameter(method.DeclaringType);
                parameterBuilder.setName("target");
            }

            var baseConstructor = objectType.getMethod("<init>", Query.empty<TypeInfo>());
            gen = methodBuilder.CodeGenerator;
            gen.beginScope();
            gen.emit(Opcode.Aload, gen.getLocal("this"));
            gen.emit(Opcode.Invokespecial, baseConstructor);
            if (!method.IsStatic) {
                gen.emit(Opcode.Aload, gen.getLocal("this"));
                gen.emit(Opcode.Aload, gen.getLocal("target"));
                gen.emit(Opcode.Putfield, targetField);
            }
            gen.emit(Opcode.Return);
            gen.endScope();
            
            //
            // Interface object creation
            //
            
            generator.emit(Opcode.New, typeBuilder);
            if (nested) {
                generator.emit(Opcode.Dup);
            }
            if (!method.IsStatic) {
                handleExpression(arg, null, true);
            }
            generator.emit(Opcode.Invokespecial, methodBuilder);
        }
        
        private void emitOperands(ExpressionNode left, bool leftIsZero, ExpressionNode right, bool rightIsZero) {
            var generator = context.MethodGenerationContext.Generator;
            var li = left.getUserData(typeof(ExpressionInfo));
            var ri = right.getUserData(typeof(ExpressionInfo));
            
            switch (li.Type.TypeKind) {
            case Byte:
            case Char:
            case Short:
            case Int: {
                switch (ri.Type.TypeKind) {
                case Byte:
                case Char:
                case Short:
                case Int: {
                    if (!leftIsZero) {
                        handleExpression(left, null, true);
                    }
                    break;
                }
                case Long: {
                    if (!leftIsZero) {
                        handleExpression(left, null, true);
                        generator.emit(Opcode.I2l);
                    }
                    break;
                }
                case Float: {
                    if (!leftIsZero) {
                        handleExpression(left, null, true);
                        generator.emit(Opcode.I2f);
                    }
                    break;
                }
                case Double: {
                    if (!leftIsZero) {
                        handleExpression(left, null, true);
                        generator.emit(Opcode.I2d);
                    }
                    break;
                }
                }
                if (!rightIsZero) {
                    handleExpression(right, null, true);
                }
                break;
            }
            case Long: {
                switch (ri.Type.TypeKind) {
                case Byte:
                case Char:
                case Short:
                case Int: {
                    if (!leftIsZero) {
                        handleExpression(left, null, true);
                    }
                    if (!rightIsZero) {
                        handleExpression(right, null, true);
                        generator.emit(Opcode.I2l);
                    }
                    break;
                }
                case Long: {
                    if (!leftIsZero) {
                        handleExpression(left, null, true);
                    }
                    if (!rightIsZero) {
                        handleExpression(right, null, true);
                    }
                    break;
                }
                case Float: {
                    if (!leftIsZero) {
                        handleExpression(left, null, true);
                        generator.emit(Opcode.L2f);
                    }
                    if (!rightIsZero) {
                        handleExpression(right, null, true);
                    }
                    break;
                }
                case Double: {
                    if (!leftIsZero) {
                        handleExpression(left, null, true);
                        generator.emit(Opcode.L2d);
                    }
                    if (!rightIsZero) {
                        handleExpression(right, null, true);
                    }
                    break;
                }
                }
                break;
            }
            case Float: {
                switch (ri.Type.TypeKind) {
                case Byte:
                case Char:
                case Short:
                case Int: {
                    if (!leftIsZero) {
                        handleExpression(left, null, true);
                    }
                    if (!rightIsZero) {
                        handleExpression(right, null, true);
                        generator.emit(Opcode.I2f);
                    }
                    break;
                }
                case Long: {
                    if (!leftIsZero) {
                        handleExpression(left, null, true);
                    }
                    if (!rightIsZero) {
                        handleExpression(right, null, true);
                        generator.emit(Opcode.L2f);
                    }
                    break;
                }
                case Float: {
                    if (!leftIsZero) {
                        handleExpression(left, null, true);
                    }
                    if (!rightIsZero) {
                        handleExpression(right, null, true);
                    }
                    break;
                }
                case Double: {
                    if (!leftIsZero) {
                        handleExpression(left, null, true);
                        generator.emit(Opcode.F2d);
                    }
                    if (!rightIsZero) {
                        handleExpression(right, null, true);
                    }
                    break;
                }
                }
                break;
            }
            case Double: {
                if (!leftIsZero) {
                    handleExpression(left, null, true);
                }
                switch (ri.Type.TypeKind) {
                case Byte:
                case Char:
                case Short:
                case Int: {
                    if (!rightIsZero) {
                        handleExpression(right, null, true);
                        generator.emit(Opcode.I2d);
                    }
                    break;
                }
                case Long: {
                    if (!rightIsZero) {
                        handleExpression(right, null, true);
                        generator.emit(Opcode.L2d);
                    }
                    break;
                }
                case Float: {
                    if (!rightIsZero) {
                        handleExpression(right, null, true);
                        generator.emit(Opcode.F2d);
                    }
                    break;
                }
                case Double: {
                    if (!rightIsZero) {
                        handleExpression(right, null, true);
                    }
                    break;
                }
                }
                break;
            }
            default:
                handleExpression(left, null, true);
                handleExpression(right, null, true);
                break;
            }
        }
        
        private void emitComparison(BinaryExpressionNode binary, bool leftIsZero, bool rightIsZero,
                Opcode ifLeftZero, Opcode ifRightZero, Opcode intOperation, LabelMarker elseLabel) {
            var generator = context.MethodGenerationContext.Generator;
            switch (binary.getUserData(typeof(TypeInfo)).TypeKind) {
            case Byte:
            case Char:
            case Short:
            case Int:
                emitOperands(binary.LeftOperand, leftIsZero, binary.RightOperand, rightIsZero);
                if (rightIsZero) {
                    generator.emit(ifRightZero, elseLabel);
                } else if (leftIsZero) {
                    generator.emit(ifLeftZero, elseLabel);
                } else {
                    generator.emit(intOperation, elseLabel);
                }
                break;
            case Long:
                if (leftIsZero) {
                    generator.emit(Opcode.Lconst_0);
                }
                emitOperands(binary.LeftOperand, leftIsZero, binary.RightOperand, rightIsZero);
                if (rightIsZero) {
                    generator.emit(Opcode.Lconst_0);
                }
                generator.emit(Opcode.Lcmp);
                generator.emit(ifRightZero, elseLabel);
                break;
            case Float:
                if (leftIsZero) {
                    generator.emit(Opcode.Fconst_0);
                }
                emitOperands(binary.LeftOperand, leftIsZero, binary.RightOperand, rightIsZero);
                if (rightIsZero) {
                    generator.emit(Opcode.Fconst_0);
                }
                generator.emit((ifRightZero == Opcode.Ifgt || ifRightZero == Opcode.Ifge) ? Opcode.Fcmpg : Opcode.Fcmpl);
                generator.emit(ifRightZero, elseLabel);
                break;
            case Double:
                if (leftIsZero) {
                    generator.emit(Opcode.Dconst_0);
                }
                emitOperands(binary.LeftOperand, leftIsZero, binary.RightOperand, rightIsZero);
                if (rightIsZero) {
                    generator.emit(Opcode.Dconst_0);
                }
                generator.emit((ifRightZero == Opcode.Ifgt || ifRightZero == Opcode.Ifge) ? Opcode.Dcmpg : Opcode.Dcmpl);
                generator.emit(ifRightZero, elseLabel);
                break;
            default:
                throw new Exception("Internal error");
            }
        }
        
        private void emitEquality(BinaryExpressionNode binary, bool leftIsZero, bool rightIsZero, LabelMarker label, bool negate) {
            var generator = context.MethodGenerationContext.Generator;
            TypeInfo type = binary.getUserData(typeof(TypeInfo));
            switch (type.TypeKind) {
            case Boolean:
            case Byte:
            case Char:
            case Short:
            case Int:
                emitOperands(binary.getLeftOperand(), leftIsZero, binary.getRightOperand(), rightIsZero);
                if (leftIsZero || rightIsZero) {
                    generator.emit((negate) ? Opcode.Ifne : Opcode.Ifeq, label);
                } else {
                    generator.emit((negate) ? Opcode.If_icmpne : Opcode.If_icmpeq, label);
                }
                break;
            case Long:
                emitOperands(binary.getLeftOperand(), false, binary.getRightOperand(), false);
                generator.emit(Opcode.Lcmp);
                generator.emit((negate) ? Opcode.Ifne : Opcode.Ifeq, label);
                break;
            case Float:
                emitOperands(binary.getLeftOperand(), false, binary.getRightOperand(), false);
                generator.emit(Opcode.Fcmpg);
                generator.emit((negate) ? Opcode.Ifne : Opcode.Ifeq, label);
                break;
            case Double:
                emitOperands(binary.getLeftOperand(), false, binary.getRightOperand(), false);
                generator.emit(Opcode.Dcmpg);
                generator.emit((negate) ? Opcode.Ifne : Opcode.Ifeq, label);
                break;
            default:
                var li = binary.getLeftOperand().getUserData(typeof(ExpressionInfo));
                var ri = binary.getRightOperand().getUserData(typeof(ExpressionInfo));
                if (li == null) {
                    handleExpression(binary.getRightOperand(), null, true);
                    generator.emit((negate) ? Opcode.Ifnonnull : Opcode.Ifnull, label);
                } else if (ri == null) {
                    handleExpression(binary.getLeftOperand(), null, true);
                    generator.emit((negate) ? Opcode.Ifnonnull : Opcode.Ifnull, label);
                } else {
                    handleExpression(binary.getLeftOperand(), null, true);
                    handleExpression(binary.getRightOperand(), null, true);
                    generator.emit((negate) ? Opcode.If_acmpne : Opcode.If_acmpeq, label);
                }
                break;
            }
        }

        void emitArray(int dimensions, TypeInfo type, Iterator<ExpressionNode> values) {
            bytecodeGenerator.emitArray(dimensions, type, values);
        }
    }
}
