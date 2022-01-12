// Decompiled with JetBrains decompiler
// Type: ExpressionCode.TokenType
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal enum TokenType
  {
    None,
    Sof,
    Eof,
    Int,
    Float,
    String,
    Array,
    Identifier,
    Plus,
    Minus,
    Times,
    Divide,
    Mod,
    Assign,
    Eq,
    Neq,
    Lt,
    Leq,
    Gt,
    Geq,
    And,
    Or,
    Indent,
    Outdent,
    Comma,
    SemiColon,
    Colon,
    LParen,
    RParen,
    LBrace,
    RBrace,
    LBracket,
    RBracket,
    VarDecl,
    ConstDecl,
    Func,
    Return,
    If,
    ElseIf,
    Else,
    While,
    Break,
    Continue,
    Locate,
    Print,
    Cls,
    Begin,
    End,
    BuiltIn,
    FullExpression,
    Error,
  }
}
