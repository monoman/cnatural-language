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
using stab.lang;
using cnatural.helpers;

namespace cnatural.parser {

    public enum InputSectionPartKind {
        SourceCode,
        Definition,
        Line,
        Diagnostic,
        Region,
        Pragma,
        If,
        Elif,
        Else,
		Endif
    }

    public abstract class InputSectionPart : UserDataContainer {
        protected InputSectionPart(InputSectionPartKind inputSectionPartKind, int position, int length, int line) {
            this.InputSectionPartKind = inputSectionPartKind;
            this.Position = position;
            this.Length = length;
            this.Line = line;
        }

        public InputSectionPartKind InputSectionPartKind^;
        
        public int Position^;
        
        public int Length^;
        
        public int Line^;
        
        public virtual Iterable<InputSectionPart> InputSectionParts {
            get {
                throw new IllegalStateException();
            }
        }
        
        public virtual bool SkippedSection {
            get {
                throw new IllegalStateException();
            }
        }
        
        public virtual bool ExpressionValue {
            get {
                throw new IllegalStateException();
            }
        }
        
        public virtual bool Define {
            get {
                throw new IllegalStateException();
            }
        }
        
        public virtual String Symbol {
            get {
                throw new IllegalStateException();
            }
        }
        
        public virtual bool Error {
            get {
                throw new IllegalStateException();
            }
        }
        
        public virtual String Message {
            get {
                throw new IllegalStateException();
            }
        }
        
        public virtual String Filename {
            get {
                throw new IllegalStateException();
            }
        }
        
        public virtual bool Hidden {
            get {
                throw new IllegalStateException();
            }
        }
        
        public virtual bool Default {
            get {
                throw new IllegalStateException();
            }
        }
        
        public virtual bool Restore {
            get {
                throw new IllegalStateException();
            }
        }

        public virtual IntIterable Warnings {
            get {
                throw new IllegalStateException();
            }
        }
        
        public virtual String StartMessage {
            get {
                throw new IllegalStateException();
            }
        }
        
        public virtual String EndMessage {
            get {
                throw new IllegalStateException();
            }
        }
    }
    
    abstract class CompoundInputSectionPart : InputSectionPart {
        private Iterable<InputSectionPart> inputSectionParts;
        
        protected CompoundInputSectionPart(InputSectionPartKind type, int position, int length, int line,
            Iterable<InputSectionPart> inputSectionParts)
            : super(type, position, length, line) {
            this.inputSectionParts = inputSectionParts;
        }

        public override Iterable<InputSectionPart> InputSectionParts {
            get {
                return inputSectionParts;
            }
        }
    }

    abstract class SkippableInputSectionPart : CompoundInputSectionPart {
        private bool skippedSection;
        
        protected SkippableInputSectionPart(InputSectionPartKind type, int position, int length, int line,
            Iterable<InputSectionPart> inputSectionParts, bool skippedSection)
            : super(type, position, length, line, inputSectionParts) {
            this.skippedSection = skippedSection;
        }

        public override bool SkippedSection {
            get {
                return skippedSection;
            }
        }
    }

    abstract class ConditionalInputSectionPart : SkippableInputSectionPart {
        private bool expressionValue;
        
        protected ConditionalInputSectionPart(InputSectionPartKind type, int position, int length, int line,
            Iterable<InputSectionPart> parts, bool skippedSection, bool expressionValue)
            : super(type, position, length, line, parts, skippedSection) {
            this.expressionValue = expressionValue;
        }

        public override bool ExpressionValue {
            get {
                return expressionValue;
            }
        }
    }

    class DefinitionSectionPart : InputSectionPart {
        private bool define;
        private String symbol;
        
        DefinitionSectionPart(int position, int length, int line, bool define, String symbol)
            : super(InputSectionPartKind.Definition, position, length, line) {
            this.define = define;
            this.symbol = symbol;
        }

        public override bool Define {
            get {
                return define;
            }
        }

        public override String Symbol {
            get {
                return symbol;
            }
        }
    }

    class DiagnosticSectionPart : InputSectionPart {
        private bool error;
        private String message;
        
        DiagnosticSectionPart(int position, int length, int line, bool error, String message)
            : super(InputSectionPartKind.Diagnostic, position, length, line) {
            this.error = error;
            this.message = message;
        }

        public override bool Error {
            get {
                return error;
            }
        }

        public override String Message {
            get {
                return message;
            }
        }
    }


    class ElifSectionPart : ConditionalInputSectionPart {
        ElifSectionPart(int position, int length, int line, Iterable<InputSectionPart> parts, bool skippedSection, bool expressionValue)
            : super(InputSectionPartKind.Elif, position, length, line, parts, skippedSection, expressionValue) {
        }
    }


    class IfSectionPart : ConditionalInputSectionPart {
        IfSectionPart(int position, int length, int line, Iterable<InputSectionPart> parts, bool skippedSection, bool expressionValue)
            : super(InputSectionPartKind.If, position, length, line, parts, skippedSection, expressionValue) {
        }
    }

    class ElseSectionPart : SkippableInputSectionPart {
        ElseSectionPart(int position, int length, int line, Iterable<InputSectionPart> parts, bool skippedSection)
            : super(InputSectionPartKind.Else, position, length, line, parts, skippedSection) {
        }
    }

    class LineSectionPart : InputSectionPart {
        private String filename;
        private bool hidden;
        private bool @default;
        
        LineSectionPart(int position, int length, int line, String filename)
            : super(InputSectionPartKind.Line, position, length, line) {
            this.filename = filename;
        }

        LineSectionPart(int position, int length, int line, bool hidden)
            : super(InputSectionPartKind.Line, position, length, line) {
            this.hidden = hidden;
            this.@default = !hidden;
        }

        public override String Filename {
            get {
                return filename;
            }
        }

        public override bool Hidden {
            get {
                return hidden;
            }
        }

        public override bool Default {
            get {
                return @default;
            }
        }
    }

    class PragmaSectionPart : InputSectionPart {
        private bool restore;
        private IntIterable warnings;

        PragmaSectionPart(int position, int length, int line, bool restore, IntIterable warnings)
            : super(InputSectionPartKind.Pragma, position, length, line) {
            this.restore = restore;
            this.warnings = warnings;
        }

        public override bool Restore {
            get {
                return restore;
            }
        }

        public override IntIterable Warnings {
            get {
                return warnings;
            }
        }
    }


    class RegionSectionPart : CompoundInputSectionPart {
        private String startMessage;
        private String endMessage;
        
        RegionSectionPart(int position, int length, int line, Iterable<InputSectionPart> parts, String startMessage, String endMessage)
            : super(InputSectionPartKind.Region, position, length, line, parts) {
            this.startMessage = startMessage;
            this.endMessage = endMessage;
        }

        public override String StartMessage {
            get {
                return startMessage;
            }
        }

        public override String EndMessage {
            get {
                return endMessage;
            }
        }
    }

    class SourceCodeSectionPart : InputSectionPart {
        SourceCodeSectionPart(int position, int length, int line)
            : super(InputSectionPartKind.SourceCode, position, length, line) {
        }
    }

    class EndifSectionPart : InputSectionPart {
        EndifSectionPart(int position, int length, int line)
            : super(InputSectionPartKind.Endif, position, length, line) {
        }
    }

}
