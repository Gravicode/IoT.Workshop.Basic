// Decompiled with JetBrains decompiler
// Type: ExpressionCode.FuncCallAst
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal class FuncCallAst : ExpressionAst
  {
    private static int globalSiteId;
    public ExpressionAst functionExpression;
    public ExpressionAst[] arguments;
    public int siteId;

    public FuncCallAst(ExpressionAst functionExpression, ExpressionAst[] arguments)
      : base((AstNode) functionExpression)
    {
      this.NodeType = AstNodeType.FuncCall;
      this.siteId = ++FuncCallAst.globalSiteId;
      this.functionExpression = functionExpression;
      this.arguments = arguments;
    }
  }
}
