// Decompiled with JetBrains decompiler
// Type: ExpressionCode.StatementBlockAst
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal class StatementBlockAst : StatementAst
  {
    public StatementAst[] statements;
    public NopAst endBody;

    public StatementBlockAst(StatementAst[] statements, NopAst endBody)
    {
      this.NodeType = AstNodeType.StatementBlock;
      this.statements = statements;
      this.endBody = endBody;
    }
  }
}
