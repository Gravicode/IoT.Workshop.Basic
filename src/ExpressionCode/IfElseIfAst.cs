// Decompiled with JetBrains decompiler
// Type: ExpressionCode.IfElseIfAst
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal class IfElseIfAst : IfElseAst
  {
    public IfAst[] elseIfList;

    public IfElseIfAst(
      Token token,
      ExpressionAst condition,
      StatementBlockAst body,
      IfAst[] elseIfList,
      StatementBlockAst elseBody)
      : base(token, condition, body, elseBody)
    {
      this.NodeType = AstNodeType.IfElseIf;
      this.condition = condition;
      this.body = body;
      this.elseIfList = elseIfList;
      this.elseBody = elseBody;
    }
  }
}
