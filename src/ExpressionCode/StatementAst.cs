// Decompiled with JetBrains decompiler
// Type: ExpressionCode.StatementAst
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal abstract class StatementAst : AstNode
  {
    public StatementAst(Token token)
      : base(token)
    {
      this.NodeType = AstNodeType.Statement;
    }

    public StatementAst(AstNode innerNode)
      : base(innerNode)
    {
      this.NodeType = AstNodeType.Statement;
    }

    protected StatementAst()
    {
    }
  }
}
