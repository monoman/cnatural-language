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
using stab.lang;
using stab.query;
using cnatural.helpers;

namespace cnatural.parser {
    public class PreprocessedTextScanner : IScanner {
        private String filename;
        private String lineFilename;
		private int tabWidth;
        private SourceCodeScanner scanner;
        private ArrayList<SourceInfo> sourceInfos;
        
        public PreprocessedTextScanner(CodeErrorManager codeErrorManager, PreprocessedText preprocessedText) {
            this.CodeErrorManager = codeErrorManager;
            this.PreprocessedText = preprocessedText;
			this.tabWidth = 4;
        }
        
        public CodeErrorManager CodeErrorManager^;
        
        public PreprocessedText PreprocessedText^;
        
        public String Filename {
            get {
                return lineFilename ?? filename;
            }
            set {
                filename = value;
				if (scanner != null ) {
					scanner.Filename = value;
				}
            }
        }
        
        public char[] Text {
            get {
                return this.PreprocessedText.Text;
            }
        }
        
        public int TabWidth {
            get {
				return tabWidth;
            }
            set {
				tabWidth = value;
				if (scanner != null) {
					scanner.TabWidth = value;
				}
            }
        }
        
        public int StartPosition {
            get {
                return this.Scanner.StartPosition;
            }
        }
        
        public int EndPosition {
            get {
                return this.Scanner.EndPosition;
            }
        }
        
        public int StartLine {
            get {
                return this.Scanner.StartLine;
            }
        }
        
        public int EndLine {
            get {
                return this.Scanner.EndLine;
            }
        }
        
        public int StartColumn {
            get {
                return this.Scanner.StartColumn;
            }
        }
        
        public int EndColumn {
            get {
                return this.Scanner.EndColumn;
            }
        }
        
        public Keyword Keyword {
            get {
                return this.Scanner.Keyword;
            }
        }

        public int Part^;
        
        public RestorePoint createRestorePoint() {
            return new RestorePoint(StartPosition, Scanner.EndOffset - StartPosition + 1, Part, StartLine, StartColumn,
                Scanner.CodeErrorManager.ErrorCount, Scanner.CodeErrorManager.DisabledWarnings);
        }
        
        public void restore(RestorePoint restorePoint) {
            this.Scanner.restore(restorePoint);
            this.Part = restorePoint.Part;
        }
        
        public LexicalUnit nextLexicalUnit() {
            LexicalUnit result;
            while ((result = this.Scanner.nextLexicalUnit()) == LexicalUnit.EndOfStream) {
                if (this.Part + 1 >= sourceInfos.size()) {
                    return result;
                }
                var sourceInfo = sourceInfos.get(++this.Part);
                var source = sourceInfo.source;
                int line = initializeLine(source, sourceInfo.line);
                this.Scanner.initialize(source.Position, source.Length, line, 0, this.CodeErrorManager.ErrorCount, sourceInfo.disabledWarnings);
                scanner.Filename = this.Filename;
            }
            return result;
        }
        
        public void addError(ParseErrorId errorId, int level, String message) {
            this.Scanner.addError(errorId, level, message);
        }
        
        private SourceCodeScanner Scanner {
            get {
                if (scanner == null) {
                    sourceInfos = new ArrayList<SourceInfo>();
                    computeSourceInfos(this.PreprocessedText.InputSectionParts, null, Query.emptyInt());
                    if (sourceInfos.size() > 0) {
                        var info = sourceInfos.get(0);
                        var source = info.source;
                        this.CodeErrorManager.DisabledWarnings = info.disabledWarnings;
                        int line = initializeLine(source, info.line);
                        scanner = new SourceCodeScanner(this.CodeErrorManager, this.Text, source.Position, source.Length, line, 0);
                        scanner.Filename = this.filename;
						scanner.TabWidth = this.tabWidth;
                    } else {
                        scanner = new SourceCodeScanner(this.CodeErrorManager, this.Text, 0, 0, 0, 0);
                    }
                }
                return scanner;
            }
        }
        
        private int initializeLine(InputSectionPart input, InputSectionPart line) {
            if (line == null || line.Hidden || line.Default) {
                lineFilename = null;
                return input.Line;
            } else {
                lineFilename =(Helper.isNullOrEmpty(line.Filename)) ? null : line.Filename;
                return line.Line;
            }
        }

        private IntIterable computeSourceInfos(Iterable<InputSectionPart> inputSectionParts, InputSectionPart line, IntIterable disabledWarnings) {
            foreach (var input in inputSectionParts) {
                Iterable<InputSectionPart> children = null;
                switch (input.InputSectionPartKind) {
                case SourceCode:
                    var info = new SourceInfo();
                    info.source = input;
                    info.line = line;
                    info.disabledWarnings = disabledWarnings;
                    sourceInfos.add(info);
                    break;
                    
                case Region:
                    children = input.InputSectionParts;
                    break;
                    
                case If:
                case Elif:
                case Else:
                    if (!input.SkippedSection) {
                        children = input.InputSectionParts;
                    }
                    break;
                    
                case Line:
                    line = input;
                    break;
                    
                case Pragma:
                    if (input.Restore) {
                        if (input.Warnings.any()) {
                            disabledWarnings = disabledWarnings.except(input.Warnings);
                        } else {
                            disabledWarnings = input.Warnings;
                        }
                    } else {
                        if (input.Warnings.any()) {
                            disabledWarnings = disabledWarnings.union(input.Warnings);
                        } else {
                            disabledWarnings = input.Warnings;
                            this.CodeErrorManager.disableAllWarnings();
                        }
                    }
                    this.CodeErrorManager.DisabledWarnings = disabledWarnings;
                    break;
                    
                case Diagnostic:
                    String filename;
                    int lineNumber;
                    if (line == null || line.Hidden || line.Default) {
                        filename = this.Filename;
                        lineNumber = input.Line;
                    } else {
                        filename = (Helper.isNullOrEmpty(line.Filename)) ? this.Filename : line.Filename;
                        lineNumber = line.Line;
                    }
                    var id = (input.Error) ? ParseErrorId.ErrorDiagnostic : ParseErrorId.WarningDiagnostic;
                    this.CodeErrorManager.addError(filename, id.ordinal(), (input.Error) ? 0 : 1,
                        Resources.getMessage(id, input.getMessage()), lineNumber, 0);
                    break;
                }
                if (children != null) {
                    disabledWarnings = computeSourceInfos(children, line, disabledWarnings);
                }
            }
            return disabledWarnings;
        }
        
        private class SourceInfo {
            InputSectionPart line;
            IntIterable disabledWarnings;
            InputSectionPart source;
        }
    }
}
