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
using cnatural.helpers;

namespace cnatural.parser {

    public class SourceCodeScanner : ScannerBase, IScanner {

        public SourceCodeScanner(CodeErrorManager codeErrorManager, char[] text)
            : this(codeErrorManager, text, 0, sizeof(text), 0, 0) {
        }
    
        public SourceCodeScanner(CodeErrorManager codeErrorManager, char[] text, int position, int length, int line, int column)
            : super(codeErrorManager, text, position, length, line, column) {
        }

        public int StartLine^;
        
        public int EndLine {
            get {
                return Line;
            }
        }
        
        public int StartColumn^;
        
        public int EndColumn {
            get {
                return Column;
            }
        }
        
        public int StartPosition^;
        
        public int EndPosition {
            get {
                return Position;
            }
        }
        
        public int Part {
            get {
                return 0;
            }
        }
        
        public Keyword Keyword^;
        
        public RestorePoint createRestorePoint() {
            return new RestorePoint(StartPosition, EndOffset - StartPosition + 1, Part, StartLine, StartColumn,
                CodeErrorManager.ErrorCount, CodeErrorManager.DisabledWarnings);
        }

        public void restore(RestorePoint restorePoint) {
            super.initialize(restorePoint.StartPosition, restorePoint.TextLength, restorePoint.StartLine,
                restorePoint.StartColumn, restorePoint.ErrorCount, restorePoint.DisabledWarnings);
        }
        
        public override void addError(ParseErrorId errorId, int level, String message) {
            CodeErrorManager.addError(Filename, errorId.ordinal() + 1, level, message, StartLine + 1, StartColumn + 1);
        }
        
        public LexicalUnit nextLexicalUnit() {
            this.StartPosition = this.Position;
            this.StartLine = this.Line;
            this.StartColumn = this.Column;

            switch (this.Next) {
            case -1:
                return LexicalUnit.EndOfStream;

            case '\r':
                this.Line++;
                this.Column = -1;
                if (advance() == '\n') {
                    this.Column = -1;
                    advance();
                }
                return LexicalUnit.NewLine;

            case '\u0085':
            case '\u2028':
            case '\u2029':
            case '\n':
                this.Line++;
                this.Column = -1;
                advance();
                return LexicalUnit.NewLine;

            case ' ':
            case '\t':
            case '\v':
            case '\f':
                scanWhitespaces();
                return LexicalUnit.Whitespace;

            case '{':
                advance();
                return LexicalUnit.OpenBrace;

            case '}':
                advance();
                return LexicalUnit.CloseBrace;

            case '[':
                advance();
                return LexicalUnit.OpenBracket;

            case ']':
                advance();
                return LexicalUnit.CloseBracket;

            case '(':
                advance();
                return LexicalUnit.OpenParenthesis;

            case ')':
                advance();
                return LexicalUnit.CloseParenthesis;

            case ',':
                advance();
                return LexicalUnit.Comma;

            case ';':
                advance();
                return LexicalUnit.SemiColon;

            case '~':
                advance();
                return LexicalUnit.Complement;

            case '>':
                if (advance() == '=') {
                    advance();
                    return LexicalUnit.GreaterThanOrEqual;
                }
                return LexicalUnit.GreaterThan;

            case '?':
                switch (advance()) {
                case '?':
                    advance();
                    return LexicalUnit.NullCoalescing;
                case '.':
                    advance();
                    return LexicalUnit.NullSafeMemberAccess;
                default:
                    return LexicalUnit.QuestionMark;
                }

            case '!':
                if (advance() == '=') {
                    advance();
                    return LexicalUnit.NotEqual;
                }
                return LexicalUnit.Not;

            case ':':
                advance();
                return LexicalUnit.Colon;

            case '*':
                if (advance() == '=') {
                    advance();
                    return LexicalUnit.MultiplyAssign;
                }
                return LexicalUnit.Multiply;

            case '%':
                if (advance() == '=') {
                    advance();
                    return LexicalUnit.ModuloAssign;
                }
                return LexicalUnit.Percent;

            case '^':
                if (advance() == '=') {
                    advance();
                    return LexicalUnit.XorAssign;
                }
                return LexicalUnit.Xor;

            case '<':
                switch (advance()) {
                case '=':
                    advance();
                    return LexicalUnit.LessThanOrEqual;

                case '<':
                    if (advance() == '=') {
                        advance();
                        return LexicalUnit.LeftShiftAssign;
                    }
                    return LexicalUnit.LeftShift;

                default:
                    return LexicalUnit.LessThan;
                }

            case '+':
                switch (advance()) {
                case '+':
                    advance();
                    return LexicalUnit.Increment;

                case '=':
                    advance();
                    return LexicalUnit.AddAssign;

                default:
                    return LexicalUnit.Plus;
                }

            case '-':
                switch (advance()) {
                case '-':
                    advance();
                    return LexicalUnit.Decrement;

                case '=':
                    advance();
                    return LexicalUnit.SubtractAssign;

                default:
                    return LexicalUnit.Minus;
                }

            case '&':
                switch (advance()) {
                case '&':
                    advance();
                    return LexicalUnit.LogicalAnd;

                case '=':
                    advance();
                    return LexicalUnit.AndAssign;

                default:
                    return LexicalUnit.And;
                }

            case '|':
                switch (advance()) {
                case '|':
                    advance();
                    return LexicalUnit.LogicalOr;

                case '=':
                    advance();
                    return LexicalUnit.OrAssign;

                default:
                    return LexicalUnit.Or;
                }

            case '=':
                switch (advance()) {
                case '=':
                    advance();
                    return LexicalUnit.Equal;

                case '>':
                    advance();
                    return LexicalUnit.Lambda;

                default:
                    return LexicalUnit.Assign;
                }

            case 'a':
                switch (advance()) {
                case 'b':
                    return scanKeyword("bstract", Keyword.Abstract);
                case 's':
                    switch (advance()) {
                    case 'c':
                        return scanContextualKeyword("cending", Keyword.Ascending);
					case 'y':
						return scanContextualKeyword("ync", Keyword.Async);
                    default:
                        return scanKeyword("", Keyword.As);
                    }
                default:
                    return scanKeyword("", Keyword.None);
                }

            case 'b':
                switch (advance()) {
                case 'o':
					if (advance() != 'o' || advance() != 'l')
						return scanKeyword("", Keyword.None);
					if (advance() == 'e')
						return scanKeyword("ean", Keyword.Boolean);
					return scanKeyword("", Keyword.Boolean);
                case 'r':
                    return scanKeyword("reak", Keyword.Break);
                case 'y':
                    switch (advance()) {
                    case 't':
                        return scanKeyword("te", Keyword.Byte);
                    default:
                        return scanContextualKeyword("", Keyword.By);
                    }
                default:
                    return scanKeyword("", Keyword.None);
                }

            case 'c':
                switch (advance()) {
                case 'a':
                    switch (advance()) {
                    case 's':
                        return scanKeyword("se", Keyword.Case);
                    case 't':
                        return scanKeyword("tch", Keyword.Catch);
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                case 'h':
                    switch (advance()) {
                    case 'a':
                        return scanKeyword("ar", Keyword.Char);
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                case 'l':
                    return scanKeyword("lass", Keyword.Class);
                case 'o':
                    switch (advance()) {
                    case 'n':
                        switch (advance()) {
                        case 't':
                            return scanKeyword("tinue", Keyword.Continue);
                        default:
                            return scanKeyword("", Keyword.None);
                        }
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                default:
                    return scanKeyword("", Keyword.None);
                }

            case 'd':
                switch (advance()) {
                case 'e':
                    switch (advance()) {
                    case 'f':
                        return scanKeyword("fault", Keyword.Default);
                    case 'l':
                        return scanKeyword("legate", Keyword.Delegate);
                    case 's':
                        return scanContextualKeyword("scending", Keyword.Descending);
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                case 'o':
                    switch (advance()) {
                    case 'u':
                        return scanKeyword("uble", Keyword.Double);
                    default:
                        return scanKeyword("", Keyword.Do);
                    }
                default:
                    return scanKeyword("", Keyword.None);
                }

            case 'e':
                switch (advance()) {
                case 'l':
                    return scanKeyword("lse", Keyword.Else);
                case 'n':
                    return scanKeyword("num", Keyword.Enum);
                case 'q':
                    return scanContextualKeyword("quals", Keyword.Equals);
                default:
                    return scanKeyword("", Keyword.None);
                }

            case 'f':
                switch (advance()) {
                case 'a':
                    return scanKeyword("alse", Keyword.False);
                case 'i':
                    switch (advance()) {
                    case 'n':
                        switch (advance()) {
                        case 'a':
                            switch (advance()) {
                            case 'l':
                                switch (advance()) {
                                case 'l':
                                    return scanKeyword("ly", Keyword.Finally);
                                default:
                                    return scanKeyword("", Keyword.Final);
                                }
                            default:
                                return scanKeyword("", Keyword.None);
                            }
                        default:
                            return scanKeyword("", Keyword.None);
                        }
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                case 'l':
                    return scanKeyword("loat", Keyword.Float);
                case 'o':
                    switch (advance()) {
                    case 'r':
                        switch (advance()) {
                        case 'e':
                            return scanKeyword("each", Keyword.Foreach);
                        default:
                            return scanKeyword("", Keyword.For);
                        }
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                case 'r':
                    return scanContextualKeyword("rom", Keyword.From);
                default:
                    return scanKeyword("", Keyword.None);
                }

            case 'g':
                switch (advance()) {
                case 'e':
                    return scanContextualKeyword("et", Keyword.Get);
                case 'o':
                    return scanKeyword("oto", Keyword.Goto);
                case 'r':
                    return scanContextualKeyword("roup", Keyword.Group);
                default:
                    return scanKeyword("", Keyword.None);
                }

            case 'i':
                switch (advance()) {
                case 'f':
                    return scanKeyword("f", Keyword.If);
                case 'n':
                    switch (advance()) {
                    case 's':
                        return scanKeyword("stanceof", Keyword.Instanceof);
                    case 't':
                        switch (advance()) {
                        case 'e':
                            return scanKeyword("erface", Keyword.Interface);
                        case 'o':
                            return scanContextualKeyword("o", Keyword.Into);
                        default:
                            return scanKeyword("", Keyword.Int);
                        }
                    default:
                        return scanKeyword("", Keyword.In);
                    }
                default:
                    return scanKeyword("", Keyword.None);
                }

            case 'j':
                return scanContextualKeyword("join", Keyword.Join);

            case 'l':
                switch (advance()) {
                case 'e':
                    return scanContextualKeyword("et", Keyword.Let);
                case 'o':
                    switch (advance()) {
                    case 'n':
                        return scanKeyword("ng", Keyword.Long);
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                default:
                    return scanKeyword("", Keyword.None);
                }

            case 'n':
                switch (advance()) {
                case 'a':
                    switch (advance()) {
                    case 'm':
						return scanContextualKeyword("mespace", Keyword.Package);
                    case 't':
						return scanKeyword("tive", Keyword.Native);
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                case 'e':
                    return scanKeyword("ew", Keyword.New);
                case 'u':
                    return scanKeyword("ull", Keyword.Null);
                default:
                    return scanKeyword("", Keyword.None);
                }

            case 'o':
                switch (advance()) {
                case 'n':
                    return scanContextualKeyword("n", Keyword.On);
                case 'r':
                    return scanContextualKeyword("rderby", Keyword.Orderby);
                case 'v':
                    return scanKeyword("verride", Keyword.Override);
                default:
                    return scanKeyword("", Keyword.None);
                }

            case 'p':
                switch (advance()) {
                case 'a':
                    switch (advance()) {
                    case 'c':
                        return scanKeyword("ckage", Keyword.Package);
                    case 'r':
                        switch (advance()) {
                        case 'a':
                            return scanKeyword("ams", Keyword.Params);
                        case 't':
                            return scanContextualKeyword("tial", Keyword.Partial);
                        default:
                            return scanKeyword("", Keyword.None);
                        }
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                case 'r':
                    switch (advance()) {
                    case 'i':
                        return scanKeyword("ivate", Keyword.Private);
                    case 'o':
						return scanKeyword("otected", Keyword.Protected);
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                case 'u':
                    return scanKeyword("ublic", Keyword.Public);
                default:
                    return scanKeyword("", Keyword.None);
                }

            case 'r':
                return scanKeyword("return", Keyword.Return);

            case 's':
                switch (advance()) {
                case 'e':
                    switch (advance()) {
                    case 'l':
                        return scanContextualKeyword("lect", Keyword.Select);
                    case 't':
                        return scanContextualKeyword("t", Keyword.Set);
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                case 'h':
                    return scanKeyword("hort", Keyword.Short);
                case 'i':
                    return scanKeyword("izeof", Keyword.Sizeof);
                case 't':
                    switch (advance()) {
                    case 'a':
                        switch (advance()) {
                        case 't':
                            return scanKeyword("tic", Keyword.Static);
                        default:
                            return scanKeyword("", Keyword.None);
                        }
                    case 'r':
                        switch (advance()) {
                        case 'i':
							switch (advance()) {
							case 'c':
								return scanKeyword("ctfp", Keyword.Strictfp);
							case 'n':
								return scanKeyword("ng", Keyword.String);
							default:
								return scanKeyword("", Keyword.None);
							}
                        default:
                            return scanKeyword("", Keyword.None);
                        }
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                case 'u':
                    return scanKeyword("uper", Keyword.Super);
                case 'w':
                    return scanKeyword("witch", Keyword.Switch);
                case 'y':
                    return scanKeyword("ynchronized", Keyword.Synchronized);
                default:
                    return scanKeyword("", Keyword.None);
                }

            case 't':
                switch (advance()) {
                case 'h':
                    switch (advance()) {
                    case 'i':
                        return scanKeyword("is", Keyword.This);
                    case 'r':
                        return scanKeyword("row", Keyword.Throw);
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                case 'r':
                    switch (advance()) {
                    case 'a':
                        return scanKeyword("ansient", Keyword.Transient);
                    case 'u':
                        return scanKeyword("ue", Keyword.True);
                    case 'y':
                        return scanKeyword("y", Keyword.Try);
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                case 'y':
                    return scanKeyword("ypeof", Keyword.Typeof);
                default:
                    return scanKeyword("", Keyword.None);
                }

            case 'u':
                switch (advance()) {
                case 's':
                    switch (advance()) {
                    case 'i':
                        return scanKeyword("ing", Keyword.Using);
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                default:
                    return scanKeyword("", Keyword.None);
                }

            case 'v':
                switch (advance()) {
                case 'a':
                    switch (advance()) {
                    case 'l':
                        return scanContextualKeyword("lue", Keyword.Value);
                    case 'r':
                        return scanContextualKeyword("r", Keyword.Var);
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                case 'i':
                    return scanKeyword("irtual", Keyword.Virtual);
                case 'o':
                    switch (advance()) {
                    case 'i':
                        return scanKeyword("id", Keyword.Void);
                    case 'l':
                        return scanKeyword("latile", Keyword.Volatile);
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                default:
                    return scanKeyword("", Keyword.None);
                }

            case 'w':
                switch (advance()) {
                case 'a':
                    return scanKeyword("ait", Keyword.Wait);
                case 'h':
                    switch (advance()) {
                    case 'e':
                        return scanContextualKeyword("ere", Keyword.Where);
                    case 'i':
                        return scanKeyword("ile", Keyword.While);
                    default:
                        return scanKeyword("", Keyword.None);
                    }
                default:
                    return scanKeyword("", Keyword.None);
                }

            case 'y':
                return scanContextualKeyword("yield", Keyword.Yield);

            case '/':
                switch (advance()) {
                case '/':
                    for (;;) {
                        switch (advance()) {
                        case -1:
                        case '\r':
                        case '\u2028':
                        case '\u2029':
                        case '\n':
                            return LexicalUnit.SingleLineComment;
                        }
                    }
                case '*':
                    var wasReturn = false;
                    for (;;) {
                        switch (advance()) {
                        case -1:
                            throw error(ParseErrorId.UnclosedDelimitedComment);
                            
                        case '\r':
                            this.Line++;
                            this.Column = -1;
                            wasReturn = true;
                            break;
                            
                        case '\n':
                            if (!wasReturn) {
                                this.Line++;
                            }
                            this.Column = -1;
                            wasReturn = false;
                            break;

                        case '\u2028':
                        case '\u2029':
                            this.Line++;
                            this.Column = -1;
                            wasReturn = false;
                            break;
                            
                        case '*':
                            switch (advance()) {
                            case -1:
                                throw error(ParseErrorId.UnclosedDelimitedComment);
                            case '/':
                                advance();
                                return LexicalUnit.DelimitedComment;
                            default:
                                wasReturn = false;
                                break;
                            }
                            break;
                        default:
                            wasReturn = false;
                            break;
                        }
                    }
                case '=':
                    advance();
                    return LexicalUnit.DivideAssign;
                default:
                    return LexicalUnit.Divide;
                }

            case '.':
                switch (advance()) {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return scanDotNumber();
                }
                return LexicalUnit.Dot;

            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                return scanNumber();

            case '\'':
                return scanCharacter();

            case '"':
                return scanString();

            case '@':
                switch (advance()) {
                case '"':
                    return scanVerbatimString();
                case '_':
                    scanIdentifierPart();
                    return LexicalUnit.VerbatimIdentifier;
                case '\\':
                    int unicode = scanUnicodeEscapeSequence();
                    if (!ParserHelper.isIdentifierStartChar(unicode)) {
                        throw error(ParseErrorId.InvalidEscapeSequence);
                    }
                    scanIdentifierPart();
                    return LexicalUnit.VerbatimIdentifier;
                default:
                    if (!ParserHelper.isIdentifierStartChar(this.Next)) {
                        throw error(ParseErrorId.InvalidSourceCodeChar);
                    }
                    scanIdentifierPart();
                    return LexicalUnit.VerbatimIdentifier;
                }

            case '_':
                scanIdentifierPart();
                return LexicalUnit.Identifier;

            case '\\':
                int unicode = scanUnicodeEscapeSequence();
                if (!ParserHelper.isIdentifierStartChar(unicode)) {
                    throw error(ParseErrorId.InvalidEscapeSequence);
                }
                scanIdentifierPart();
                return LexicalUnit.Identifier;

            default:
                if (!ParserHelper.isIdentifierStartChar(this.Next)) {
					System.out.println("Invalid char " + (char)this.Next + " ("+this.Next+")");
                    throw error(ParseErrorId.InvalidSourceCodeChar);
                }
                scanIdentifierPart();
                return LexicalUnit.Identifier;
            }
        }

        private LexicalUnit scanNumber() {
            while (true) {
                switch (advance()) {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    break;
                case '.':
                    switch (advance()) {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        return scanDotNumber();
                    }
                    throw error(ParseErrorId.DecimalDigitsExpected);
                case 'e':
                case 'E':
                    return scanENumber();
                case 'x':
                case 'X':
                    return scanHexNumber();
                case 'l':
                case 'L':
                    advance();
                    return LexicalUnit.LongLiteral;
                case 'f':
                case 'F':
                    advance();
                    return LexicalUnit.FloatLiteral;
                case 'd':
                case 'D':
                    advance();
                    return LexicalUnit.DoubleLiteral;
                default:
                    return LexicalUnit.DecimalIntegerLiteral;
                }
            }
        }

        private LexicalUnit scanDotNumber() {
            while (true) {
                switch (advance()) {
                case 'e':
                case 'E':
                    return scanENumber();
                default:
                    return LexicalUnit.RealLiteral;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    break;
                case 'f':
                case 'F':
                    advance();
                    return LexicalUnit.FloatLiteral;
                case 'd':
                case 'D':
                    advance();
                    return LexicalUnit.DoubleLiteral;
                }
            }
        }

        private LexicalUnit scanENumber() {
            switch (advance()) {
            case '+':
            case '-':
                switch (advance()) {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return scanExponent();
                }
                break;
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                return scanExponent();
            }
            throw error(ParseErrorId.DecimalDigitsExpected);
        }

        private LexicalUnit scanExponent() {
            while (true) {
                switch (advance()) {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    break;
                case 'f':
                case 'F':
                    advance();
                    return LexicalUnit.FloatLiteral;
                case 'd':
                case 'D':
                    advance();
                    return LexicalUnit.DoubleLiteral;
                default:
                    return LexicalUnit.RealLiteral;
                }
            }
        }

        private LexicalUnit scanHexNumber() {
            switch (advance()) {
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
            case 'a':
            case 'b':
            case 'c':
            case 'd':
            case 'e':
            case 'f':
            case 'A':
            case 'B':
            case 'C':
            case 'D':
            case 'E':
            case 'F':
                break;
            default:
                throw error(ParseErrorId.MalformedHexadecimalNumber);
            }
            while (true) {
                switch (advance()) {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    break;
                case 'l':
                case 'L':
                    advance();
                    return LexicalUnit.HexadecimalLongLiteral;
                default:
                    return LexicalUnit.HexadecimalIntegerLiteral;
                }
            }
        }
        
        private LexicalUnit scanCharacter() {
            switch (advance()) {
            case -1:
            case '\r':
            case '\u2028':
            case '\u2029':
            case '\n':
                throw error(ParseErrorId.UnclosedChar);
            case '\'':
                throw error(ParseErrorId.MalformedChar);
            case '\\':
                switch (advance()) {
                case '"':
                case '\\':
                case '\'':
                case '0':
                case 'a':
                case 'b':
                case 'f':
                case 'n':
                case 'r':
                case 't':
                case 'v':
                    advance();
                    break;
                case 'u':
                    var value = 0;
                    for (var i = 0; i < 4; i++) {
                        int digit;
                        if ((digit = ParserHelper.scanHexDigit(advance())) == -1) {
                            throw error(ParseErrorId.HexadecimalDigitExpected);
                        }
                        value = value * 16 + digit;
                        if (value > Character.MAX_VALUE) {
                            throw error(ParseErrorId.InvalidEscapeSequence);
                        }
                    }
                    advance();
                    break;
                case 'U':
                    for (var i = 0; i < 8; i++) {
                        if (ParserHelper.scanHexDigit(advance()) == -1) {
                            throw error(ParseErrorId.HexadecimalDigitExpected);
                        }
                    }
                    advance();
                    break;
                case 'x':
                    scanHexEscapeSequence();
                    break;

                default:
                    throw error(ParseErrorId.InvalidEscapeSequence);
                }
                break;
            default:
                advance();
                break;
            }
            if (this.Next != '\'') {
                throw error(ParseErrorId.MalformedChar);
            }
            advance();
            return LexicalUnit.CharacterLiteral;
        }
        
        private LexicalUnit scanString() {
            for (;;) {
                switch (advance()) {
                case -1:
                case '\r':
                case '\u2028':
                case '\u2029':
                case '\n':
                    throw error(ParseErrorId.UnclosedString);
                case '"':
                    advance();
                    return LexicalUnit.StringLiteral;
                case '\\':
                    switch (advance()) {
                    case '"':
                    case '\\':
                    case '\'':
                    case '0':
                    case 'a':
                    case 'b':
                    case 'f':
                    case 'n':
                    case 'r':
                    case 't':
                    case 'v':
                        break;
                    case 'u':
                        for (var i = 0; i < 4; i++) {
                            if (ParserHelper.scanHexDigit(advance()) == -1) {
                                throw error(ParseErrorId.HexadecimalDigitExpected);
                            }
                        }
                        break;
                    case 'U':
                        for (var i = 0; i < 8; i++) {
                            if (ParserHelper.scanHexDigit(advance()) == -1) {
                                throw error(ParseErrorId.HexadecimalDigitExpected);
                            }
                        }
                        break;
                    case 'x':
                        scanHexEscapeSequence();
                        break;

                    default:
                        throw error(ParseErrorId.InvalidEscapeSequence);
                    }
                    break;
                }
            }
        }
        
        private void scanHexEscapeSequence() {
            switch (advance()) {
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
            case 'a':
            case 'b':
            case 'c':
            case 'd':
            case 'e':
            case 'f':
            case 'A':
            case 'B':
            case 'C':
            case 'D':
            case 'E':
            case 'F':
                switch (advance()) {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    switch (advance()) {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                        switch (advance()) {
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'E':
                        case 'F':
                            advance();
                            break;
                        }
                        break;
                    }
                    break;
                }
                break;

            default:
                throw error(ParseErrorId.InvalidEscapeSequence);
            }
        }
        
        private LexicalUnit scanVerbatimString() {
            for (;;) {
                switch (advance()) {
                case -1:
                    throw error(ParseErrorId.UnclosedVerbatimString);
                case '"':
                    if (advance() != '"') {
                        return LexicalUnit.VerbatimStringLiteral;
                    }
                    advance();
                    break;
                }
            }
        }
        
        private LexicalUnit scanKeyword(String suffix, Keyword keyword) {
            return scanKeyword(suffix, keyword, false);
        }

        private LexicalUnit scanContextualKeyword(String suffix, Keyword keyword) {
            return scanKeyword(suffix, keyword, true);
        }

        private LexicalUnit scanKeyword(String suffix, Keyword keyword, bool contextual) {
            var isKeyword = true;
            for (var i = 0; i < suffix.length(); i++) {
                var c = suffix[i];
                if (this.Next == -1 || (char)this.Next != c) {
                    isKeyword = false;
                    break;
                }
                advance();
            }
            if (this.Next != -1 && ParserHelper.isIdentifierPartChar(this.Next)) {
                scanIdentifierPart();
                return LexicalUnit.Identifier;
            }
            if (isKeyword && keyword != Keyword.None) {
                this.Keyword = keyword;
                return (contextual) ? LexicalUnit.ContextualKeyword : LexicalUnit.Keyword;
            } else {
                this.Keyword = Keyword.None;
                return LexicalUnit.Identifier;
            }
        }


    }
}
