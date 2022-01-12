// Decompiled with JetBrains decompiler
// Type: ExpressionCode.FloatAst
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal class FloatAst : ExpressionAst
  {
    public double value;

    public FloatAst(Token token)
      : base(token)
    {
      this.NodeType = AstNodeType.Float;
      this.value = (double) token.value;
    }

    public FloatAst(double value, ushort line, ushort col, ushort length)
    {
      this.NodeType = AstNodeType.Float;
      this.Line = line;
      this.Col = col;
      this.Length = length;
    }

    public FloatAst(double value, AstNode first, AstNode last)
    {
      this.NodeType = AstNodeType.Float;
      this.value = value;
      this.Col = first.Col;
      this.Length = (ushort) ((uint) last.Col - (uint) first.Col + (uint) last.Length);
    }

    public FloatAst Update(double value, AstNode first, AstNode last)
    {
      this.value = value;
      this.Col = first.Col;
      this.Length = (ushort) ((uint) last.Col - (uint) first.Col + (uint) last.Length);
      return this;
    }

    public void Negate() => this.value = -this.value;
  }
}
