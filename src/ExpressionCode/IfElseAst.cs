// Decompiled with JetBrains decompiler
// Type: ExpressionCode.IfElseAst
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal class IfElseAst : IfAst
  {
    public StatementBlockAst elseBody;

    public IfElseAst(
      Token token,
      ExpressionAst condition,
      StatementBlockAst body,
      StatementBlockAst elseBody)
      : base(token, condition, body)
    {
      this.NodeType = AstNodeType.IfElse;
      this.elseBody = elseBody;
    }
  }
}
