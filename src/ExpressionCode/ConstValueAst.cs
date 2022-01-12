// Decompiled with JetBrains decompiler
// Type: ExpressionCode.ConstValueAst
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal class ConstValueAst : ExpressionAst
  {
    public object value;

    public ConstValueAst(object value, AstNode declaringNode)
      : base(declaringNode)
    {
      this.NodeType = AstNodeType.ConstValue;
      this.value = value;
    }

    public ConstValueAst(object value, Token declaringToken)
      : base(declaringToken)
    {
      this.NodeType = AstNodeType.ConstValue;
      this.value = value;
    }
  }
}
