// Decompiled with JetBrains decompiler
// Type: ExpressionCode.AstNodeType
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal enum AstNodeType
  {
    None,
    Statement,
    Script,
    Nop,
    ExpressionStatement,
    StatementBlock,
    VarDeclPre,
    VarDecl,
    FuncDecl,
    BuiltInDecl,
    Return,
    Break,
    Continue,
    AssignementPre,
    Assignement,
    If,
    IfElse,
    IfElseIf,
    While,
    Locate,
    Print,
    Cls,
    EndOfSource,
    Expression,
    Negate,
    Float,
    Int,
    String,
    ArrayLiteral,
    IndexAccessorPre,
    IndexAccessor,
    VariableAccess,
    ConstValue,
    FuncCall,
    BinOpPre,
    BinOpConst,
    BinOp,
    LastNode,
  }
}
