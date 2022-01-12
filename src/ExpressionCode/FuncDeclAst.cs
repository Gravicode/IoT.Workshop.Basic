// Decompiled with JetBrains decompiler
// Type: ExpressionCode.FuncDeclAst
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal class FuncDeclAst : StatementAst
  {
    public string functionName;
    public string[] parameters;
    public StatementBlockAst body;
    public Scope parentScope;

    public FuncDeclAst(Token identifier, string[] parameters, StatementBlockAst body)
      : base(identifier)
    {
      this.NodeType = AstNodeType.FuncDecl;
      this.functionName = StringTable.Intern((string) identifier.value);
      this.parameters = parameters;
      this.body = body;
    }
  }
}
