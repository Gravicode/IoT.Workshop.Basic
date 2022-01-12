// Decompiled with JetBrains decompiler
// Type: ExpressionCode.AstNode
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal abstract class AstNode
  {
    private ulong loc;

    public ushort Line
    {
      get => (ushort) ((this.loc & 281470681743360UL) >> 32);
      set => this.loc = (ulong) ((long) this.loc & -281470681743361L | (long) value << 32);
    }

    public ushort Col
    {
      get => (ushort) ((this.loc & 4293918720UL) >> 20);
      set => this.loc = (ulong) ((long) this.loc & -4293918721L | (long) value << 20);
    }

    public ushort Length
    {
      get => (ushort) ((this.loc & 1048320UL) >> 8);
      set => this.loc = (ulong) ((long) this.loc & -1048321L | (long) ((int) value & 4095) << 8);
    }

    public AstNodeType NodeType
    {
      get => (AstNodeType) (byte) (this.loc & (ulong) byte.MaxValue);
      set => this.loc = this.loc & 18446744073709551360UL | (ulong) (byte) value;
    }

    protected AstNode()
    {
    }

    protected AstNode(Token token)
    {
      this.Line = token.line;
      this.Col = token.col;
      this.Length = token.length;
    }

    protected AstNode(AstNode innerNode)
    {
      this.Line = innerNode.Line;
      this.Col = innerNode.Col;
      this.Length = innerNode.Length;
    }

    public AstNode WithSpan(ushort col, ushort length)
    {
      this.Col = col;
      this.Length = length;
      return this;
    }

    public AstNode WithSpan(AstNode first, AstNode last)
    {
      this.Line = first.Line;
      this.Col = first.Col;
      this.Length = (ushort) ((uint) last.Col - (uint) first.Col + (uint) last.Length);
      return this;
    }

    public virtual AstNode Optimize() => this;
  }
}
