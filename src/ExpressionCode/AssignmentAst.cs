// Decompiled with JetBrains decompiler
// Type: ExpressionCode.AssignmentAst
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal class AssignmentAst : StatementAst
  {
    public string variableName;
    public IndexAccessorAst indexer;
    public ExpressionAst expression;

    public AssignmentAst(Token identifier, ExpressionAst expression)
      : base(identifier)
    {
      this.NodeType = AstNodeType.AssignementPre;
      this.variableName = (string) identifier.value;
      this.expression = expression;
    }

    public AssignmentAst(Token root, IndexAccessorAst indexer, ExpressionAst expression)
      : base(root)
    {
      this.NodeType = AstNodeType.AssignementPre;
      this.indexer = indexer;
      this.expression = expression;
    }
  }
}
