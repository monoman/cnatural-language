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
using stab.tools.helpers;
using stab.tools.parser;
using stab.tools.syntaxtree;

namespace stab.tools.compiler {

    class CompilerContext {
        HashMap<LocalMemberInfo, FieldInfo> localFields;
        private CompileErrorId id;
        private SyntaxNode node;
        private Object[] arguments;
    
        #region Reflection cache
        private TypeInfo stringBuilderType;
        private MethodInfo stringBuilderConstructor;
        private MethodInfo objectToStringMethod;
        private MethodInfo systemArraycopyMethod;
        
        TypeInfo StringBuilderType {
            get {
                if (stringBuilderType == null) {
                    stringBuilderType = this.TypeSystem.getType("java/lang/StringBuilder");
                }
                return stringBuilderType;
            }
        }
        
        MethodInfo StringBuilderConstructor {
            get {
                if (stringBuilderConstructor == null) {
                    stringBuilderConstructor = this.StringBuilderType.getMethod("<init>", Query.empty<TypeInfo>());
                }
                return stringBuilderConstructor;
            }
        }
        
        MethodInfo ObjectToStringMethod {
            get {
                if (objectToStringMethod == null) {
                    objectToStringMethod = this.TypeSystem.ObjectType.getMethod("toString", Query.empty<TypeInfo>());
                }
                return objectToStringMethod;
            }
        }
        
        MethodInfo SystemArraycopyMethod {
            get {
                if (systemArraycopyMethod == null) {
                    var objectType = this.TypeSystem.ObjectType;
                    var intType = this.TypeSystem.IntType;
                    var parameters = new ArrayList<TypeInfo> { objectType, intType, objectType, intType, intType };
                    systemArraycopyMethod = this.TypeSystem.getType("java/lang/System").getMethod("arraycopy", parameters);
                }
                return systemArraycopyMethod;
            }
        }
        
        #endregion
    
        CompilerContext(CompilerParameters parameters, CompilerResults results) {
            this.Parameters = parameters;
            this.Results = results;
            this.CompilationUnits = new ArrayList<CompilationUnitNode>();
            this.TypeBuilders = new ArrayList<TypeBuilder>();
            this.Iterables = new HashMap<MethodBuilder, TypeBuilder>();
            this.ConstructorBuilders = new ArrayList<MethodBuilder>();
            this.CodeValidationContext = new CodeValidationContext(this);
            this.MethodGenerationContext = new MethodGenerationContext();
            this.LambdaScopes = new HashMap<MethodBuilder, TypeBuilder>();
            this.ConstantBuilder = new ConstantBuilder(this);
            this.PrivateAccessors = new HashMap<Object, MethodInfo>();
            this.PrivateMutators = new HashMap<Object, MethodInfo>();
            this.localFields = new HashMap<LocalMemberInfo, FieldInfo>();
        }
        
        public CompilerParameters Parameters^;
        
        public CompilerResults Results^;
        
        public List<CompilationUnitNode> CompilationUnits^;
        
        bool HasErrors {
            get {
                return this.Results.codeErrorManager.HasErrors;
            }
        }
        
        public Library TypeSystem;
        
        public Library AnnotatedTypeSystem;
        
        public MemberResolver MemberResolver;
        
        public char[] Text;

        public Set<String> Symbols;
        
        public List<TypeBuilder> TypeBuilders^;
        
        public List<MethodBuilder> ConstructorBuilders^;
        
        public HashMap<MethodBuilder, TypeBuilder> Iterables^;
        
        public TypeInfo CurrentType;
        
        public CodeValidationContext CodeValidationContext^;

        public MethodGenerationContext MethodGenerationContext^;
        
        public HashMap<MethodBuilder, TypeBuilder> LambdaScopes^;
        
        public HashMap<Object, MethodInfo> PrivateAccessors^;
        
        public HashMap<Object, MethodInfo> PrivateMutators^;
        
        public ConstantBuilder ConstantBuilder^;
        
        TypeInfo getType(String fullName, SyntaxNode node) {
            if (!this.TypeSystem.typeExists(fullName)) {
                throw error(CompileErrorId.TypeNotFound, node, fullName.replace('/', '.').replace('$', '.'));
            }
            return this.TypeSystem.getType(fullName);
        }
        
        String getText(int offset, int length) {
            return new String(this.Text, offset, length);
        }
        
        void appendText(StringBuilder sb, int offset, int length) {
            sb.append(this.Text, offset, length);
        }
        
        String getIdentifier(int offset, int length) {
            var sb = new StringBuilder();
            ParserHelper.unescapeIdentifier(sb, this.Text, offset, length);
            return sb.toString();
        }
        
        void appendIdentifier(StringBuilder sb, int offset, int length) {
            ParserHelper.unescapeIdentifier(sb, this.Text, offset, length);
        }
        
        FieldInfo getLocalField(TypeBuilder lambdaScope, LocalMemberInfo local) {
            var result = localFields[local];
            if (result == null) {
            	int n = lambdaScope.Fields.count(p => p.Name.startsWith("local"));
                result = lambdaScope.defineField("local" + n + '$' + local.Name, local.Type);
                localFields[local] = result;
            }
            return result;
        }
        
        void disableErrors() {
            this.Results.codeErrorManager.disable();
        }

        void restoreErrors() {
            this.Results.codeErrorManager.restore();
        }
        
        bool IsErrorHandlingDisabled {
            get {
                return this.Results.codeErrorManager.IsDisabled;
            }
        }
        
        CodeErrorException error(CompileErrorId id, SyntaxNode node, params Object[] arguments) {
            addError(id, node, arguments);
            return new CodeErrorException();
        }
        
        void addError(CompileErrorId id, SyntaxNode node, params Object[] arguments) {
            addError(0, id, node, arguments);
        }
        
        void addWarning(CompileErrorId id, SyntaxNode node, params Object[] arguments) {
            addError(1, id, node, arguments);
        }
        
        public bool DisableIgnoredErrorTracking;
        
        bool addIgnoredError() {
        	if (this.id != null && !this.IsErrorHandlingDisabled) {
        		addError(this.id, this.node, this.arguments);
        		return true;
        	} else {
        		return false;
        	}
        }
        
        void clearIgnoredError() {
    		this.id = null;
    		this.node = null;
    		this.arguments = null;
        }
        
        private void addError(int level, CompileErrorId id, SyntaxNode node, params Object[] arguments) {
        	if (this.IsErrorHandlingDisabled && !this.DisableIgnoredErrorTracking && level == 0 && this.id == null) {
        		this.id = id;
        		this.node = node;
        		this.arguments = arguments;
        	}
            this.Results.codeErrorManager.DisabledWarnings = node.DisabledWarnings;
            this.Results.codeErrorManager.addError(node.Filename, id.ordinal() + 161, level,
                Resources.getMessage(id, arguments), node.Line + 1, node.Column + 1);
        }
    }
    
    class CodeValidationContext {
        private CompilerContext context;
        private ArrayList<MethodBuilder> methods;
        private ArrayList<ArrayList<TypeInfo>> returnTypes;
        private ArrayList<List<TypeInfo>> lambdaParameters;
        
        CodeValidationContext(CompilerContext context) {
            this.context = context;
            this.methods = new ArrayList<MethodBuilder>();
            this.returnTypes = new ArrayList<ArrayList<TypeInfo>>();
            this.lambdaParameters = new ArrayList<List<TypeInfo>>();
        }
        
        void enterMethod(MethodBuilder methodBuilder) {
            context.MemberResolver.enterMethod(methodBuilder);
            this.IsStatic = methodBuilder.IsStatic;
            methods.add(methodBuilder);
            this.YieldCount = 0;
        }
        
        void enterLambdaMethod(MethodBuilder methodBuilder) {
            context.MemberResolver.enterMethod(methodBuilder, true);
            methods.add(methodBuilder);
            returnTypes.add(new ArrayList<TypeInfo>());
        }

        void leaveLambdaMethod() {
            methods.remove(methods.size() - 1);
            context.MemberResolver.leaveMethod();
            returnTypes.remove(returnTypes.size() - 1);
        }
        
        void leaveMethod() {
            methods.remove(methods.size() - 1);
            context.MemberResolver.leaveMethod();
        }

        public bool IsStatic;
        
        bool IsInMethod {
            get {
                return methods.size() > 0;
            }
        }
        
        bool IsInExpressionTree {
        	get {
	            return this.CurrentMethod != this.RootMethod && this.CurrentMethod.DeclaringType.FullName.indexOf('#') != -1;
        	}
        }
        
        bool IsInNestedMethod {
            get {
                return methods.size() > 1;
            }
        }
        
        bool IsInLambda {
            get {
                return returnTypes.size() > 0 && !this.IsInExpressionTree;
            }
        }
        
        MethodBuilder RootMethod {
            get {
                return methods[0];
            }
        }
        
        MethodBuilder CurrentMethod {
            get {
                return methods[methods.size() - 1];
            }
        }

        public int YieldCount;
        
        public int LambdaCount;
        
        void pushLambdaParameters(List<TypeInfo> parameters) {
            lambdaParameters.add(parameters);
        }
        
        void popLambdaParameters() {
            lambdaParameters.remove(lambdaParameters.size() - 1);
        }
        
        List<TypeInfo> LambdaParameters {
            get {
                return lambdaParameters[lambdaParameters.size() - 1];
            }
        }
        
        List<TypeInfo> LambdaReturnTypes {
            get {
                return returnTypes[returnTypes.size() - 1];
            }
        }
        
        public TypeInfo LambdaReturnType;
    }
    
    class MethodGenerationContext {
        private ArrayList<MethodBuilder> methods;
        private bool destructor;
        private int foreachStatement;
        private int stringSwitch;
        private int generatedLocal;
    
        MethodGenerationContext() {
            this.methods = new ArrayList<MethodBuilder>();
            this.YieldLabels = new ArrayList<LabelMarker>();
            this.ParametersUsedInLambdas = new HashSet<LocalMemberInfo>();
            this.ParametersUsedInLambda = new ArrayList<LocalMemberInfo>();
            this.CatchVariables = new ArrayList<LocalInfo>();
            this.TreeLocals = new HashMap<LocalMemberInfo, LocalInfo>();
            this.TreeLabels = new HashMap<StatementNode, LocalInfo>();
            this.LocalFields = new HashMap<LocalMemberInfo, FieldInfo>();
            this.Labels = new ArrayList<HashMap<Object, LocalInfo>>();
        }
        
        void initialize(MethodBuilder methodBuilder, TypeBuilder lambdaScope) {
            this.methods.clear();
            this.ParametersUsedInLambdas.clear();
            this.ParametersUsedInLambda.clear();
            this.CatchVariables.clear();
            this.TreeLocals.clear();
            this.TreeLabels.clear();
            this.LocalFields.clear();
            this.Labels.clear();
            
            this.methods.add(methodBuilder);
            this.LambdaScope = lambdaScope;
            this.Generator = methodBuilder.CodeGenerator;
            this.destructor = methodBuilder.Name.equals("finalize") && !methodBuilder.Parameters.any();
            this.IsLambdaScopeUsed = false;
            this.IsLambdaScopeInitialized = false;
            this.IsLambdaScopeThisInitialized = false;
            this.IsBuildingString = false;
            this.foreachStatement = 0;
            this.YieldCount = 0;
            this.stringSwitch = 0;
            this.PreviousLineNumber = 0;
            this.generatedLocal = 0;
        }
        
        void enterLambda(MethodBuilder method) {
            this.PreviousLineNumber = 0;
            this.methods.add(method);
            this.Generator = method.CodeGenerator;
        }
        
        void leaveLambda() {
            this.methods.remove(this.methods.size() - 1);
            this.Generator = this.CurrentMethod.CodeGenerator;
        }
        
        bool IsInDestructor {
            get {
                return destructor && !IsInLambda;
            }
        }

        bool IsInLambda {
            get {
                return methods.size() > 1;
            }
        }
        
        bool IsInIterable {
            get {
                return this.Iterable != null;
            }
        }
        
        MethodBuilder RootMethod {
            get {
                return methods[0];
            }
        }
        
        MethodBuilder CurrentMethod {
            get {
                return methods[methods.size() - 1];
            }
        }
        
        public bool IsLambdaScopeUsed;
        
        public bool IsLambdaScopeInitialized;
        
        public bool IsLambdaScopeThisInitialized;
        
        public TypeBuilder LambdaScope^;
        
        public HashSet<LocalMemberInfo> ParametersUsedInLambdas^;
        
        public ArrayList<LocalMemberInfo> ParametersUsedInLambda^;
        
        public CodeGenerator Generator;

        public TypeBuilder Iterable;
        
        public List<LabelMarker> YieldLabels^;
        
        public int YieldCount;
        
        public List<LocalInfo> CatchVariables^;
        
        public bool IsBuildingString;
        
        public int PreviousLineNumber;
        
        int nextForeachStatement() {
            return foreachStatement++;
        }
        
        int nextStringSwitch() {
            return stringSwitch++;
        }
        
        int nextGeneratedLocal() {
            return generatedLocal++;
        }
        
        public Map<LocalMemberInfo, LocalInfo> TreeLocals^;
        
        public Map<StatementNode, LocalInfo> TreeLabels^;
        
        public HashMap<LocalMemberInfo, FieldInfo> LocalFields^;

        public List<HashMap<Object, LocalInfo>> Labels^;
        
    }
}
