// Decompiled with JetBrains decompiler
// Type: ExpressionCode.IntAst
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal class IntAst : ExpressionAst
  {
    public int value;

    public IntAst(Token token)
      : base(token)
    {
      this.NodeType = AstNodeType.Int;
      this.value = (int) token.value;
    }

    public IntAst(int value, ushort line, ushort col, ushort length)
    {
      this.NodeType = AstNodeType.Int;
      this.Line = line;
      this.Col = col;
      this.Length = length;
    }

    public IntAst(int value, AstNode first, AstNode last)
    {
      this.NodeType = AstNodeType.Int;
      this.value = value;
      this.Col = first.Col;
      this.Length = (ushort) ((uint) last.Col - (uint) first.Col + (uint) last.Length);
    }

    public IntAst Update(int value, AstNode first, AstNode last)
    {
      this.value = value;
      this.Col = first.Col;
      this.Length = (ushort) ((uint) last.Col - (uint) first.Col + (uint) last.Length);
      return this;
    }

    public void Negate() => this.value = -this.value;
  }
}
