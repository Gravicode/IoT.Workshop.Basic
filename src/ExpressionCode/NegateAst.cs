// Decompiled with JetBrains decompiler
// Type: ExpressionCode.NegateAst
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal class NegateAst : ExpressionAst
  {
    public ExpressionAst inner;

    public NegateAst(Token token, ExpressionAst inner)
      : base(token)
    {
      this.NodeType = AstNodeType.Negate;
      this.inner = inner;
      this.WithSpan((AstNode) this, (AstNode) inner);
    }

    public override AstNode Optimize()
    {
      this.inner = this.inner.Optimize() as ExpressionAst;
      switch (this.inner)
      {
        case FloatAst floatAst:
          floatAst.Negate();
          return this.inner.WithSpan(this.Col, (ushort) ((uint) this.Length + (uint) this.inner.Length));
        case IntAst intAst:
          intAst.Negate();
          return this.inner.WithSpan(this.Col, (ushort) ((uint) this.Length + (uint) this.inner.Length));
        default:
          return base.Optimize();
      }
    }
  }
}
