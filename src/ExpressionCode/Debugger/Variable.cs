// Decompiled with JetBrains decompiler
// Type: ExpressionCode.Debugger.Variable
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode.Debugger
{
  public class Variable
  {
    public string Name { get; private set; }

    public object Value { get; private set; }

    public bool IsConstant { get; private set; }

    public Variable(string name, object value, bool isConstant)
    {
      this.Name = name;
      this.Value = value;
      this.IsConstant = isConstant;
    }
  }
}
