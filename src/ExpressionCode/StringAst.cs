// Decompiled with JetBrains decompiler
// Type: ExpressionCode.StringAst
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal class StringAst : ExpressionAst
  {
    public string value;

    public StringAst(Token token)
      : base(token)
    {
      this.NodeType = AstNodeType.String;
      this.value = (string) token.value;
    }

    public StringAst Update(string value, AstNode first, AstNode last)
    {
      this.value = value;
      this.Col = first.Col;
      this.Length = (ushort) ((uint) last.Col - (uint) first.Col + (uint) last.Length);
      return this;
    }
  }
}
