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
namespace stab.tools.parser {

    public enum LexicalUnit {
        EndOfStream,
        AddAssign,
        And,
        AndAssign,
        Assign,
        CharacterLiteral,
        CloseBrace,
        CloseBracket,
        CloseParenthesis,
        Colon,
        Comma,
        Complement,
        ContextualKeyword,
        DecimalIntegerLiteral,
        Decrement,
        DelimitedComment,
        Divide,
        DivideAssign,
        Dot,
        DoubleLiteral,
        Equal,
        FloatLiteral,
        GreaterThan,
        GreaterThanOrEqual,
        HexadecimalIntegerLiteral,
        HexadecimalLongLiteral,
        Identifier,
        Increment,
        Keyword,
        Lambda,
        LeftShift,
        LeftShiftAssign,
        LessThan,
        LessThanOrEqual,
        LogicalAnd,
        LogicalOr,
        LongLiteral,
        Minus,
        ModuloAssign,
        Multiply,
        MultiplyAssign,
        NewLine,
        Not,
        NotEqual,
        NullCoalescing,
		NullSafeMemberAccess,
        OpenBrace,
        OpenBracket,
        OpenParenthesis,
        Or,
        OrAssign,
        Percent,
        Plus,
        QuestionMark,
        RealLiteral,
        SemiColon,
        SingleLineComment,
        StringLiteral,
        SubtractAssign,
        VerbatimIdentifier,
        VerbatimStringLiteral,
        Xor,
        XorAssign,
        Whitespace,
    }
}