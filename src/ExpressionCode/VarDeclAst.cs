// Decompiled with JetBrains decompiler
// Type: ExpressionCode.VarDeclAst
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal class VarDeclAst : StatementAst
  {
    public string variableName;
    public ExpressionAst expression;

    public VarDeclAst(Token identifier, ExpressionAst expression)
      : base(identifier)
    {
      this.NodeType = AstNodeType.VarDeclPre;
      this.variableName = (string) identifier.value;
      this.expression = expression;
    }

    public virtual bool IsConst => false;
  }
}
