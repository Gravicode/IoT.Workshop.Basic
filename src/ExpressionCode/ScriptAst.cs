// Decompiled with JetBrains decompiler
// Type: ExpressionCode.ScriptAst
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal class ScriptAst : StatementAst
  {
    public StatementBlockAst constants;
    public StatementBlockAst script;

    public ScriptAst(StatementBlockAst constants, StatementBlockAst script)
    {
      this.NodeType = AstNodeType.Script;
      this.constants = constants;
      this.script = script;
    }
  }
}
