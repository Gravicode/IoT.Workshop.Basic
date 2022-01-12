// Decompiled with JetBrains decompiler
// Type: ExpressionCode.ScriptEngine
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

using ExpressionCode.Debugger;
using ExpressionCode.IO;
using System;

namespace ExpressionCode
{
  public class ScriptEngine
  {
    private AstExecutor astExecutor;

    public event DebugEventHandler DebugEvent;

    public ScriptEngine(params object[] builtIns)
    {
      this.astExecutor = new AstExecutor();
      this.astExecutor.LoadBuiltIn(typeof (Stdlib));
      foreach (object builtIn in builtIns)
      {
        Type builtins = builtIn as Type;
        if ((object) builtins != null)
          this.astExecutor.LoadBuiltIn(builtins);
        else
          this.astExecutor.LoadBuiltIn(builtIn);
      }
    }

    public void SetConsole(IConsole console) => Stdlib.SetConsole(console);

    public void ResetEnvironment() => this.astExecutor.Reset();

    public void Run(string script, bool debug = false) => this.Run((TextReaderEx) new StringReaderEx(script), debug);

    private void Run(TextReaderEx code, bool debug)
    {
      if (Stdlib.Console == null)
        this.SetConsole((IConsole) new DefaultConsole());
      AstNode ast = new Parser(new Scanner(code)).Parse();
      this.astExecutor.DebugCallback = debug ? new DebugCallback(this.DebugCallbackHandler) : (DebugCallback) null;
      this.astExecutor.Execute(ast);
    }

    public string[] GetAllFunctions() => this.astExecutor.GetAllFunctions();

    public Variable[] GetGlobalVariables() => this.astExecutor.GetGlobalVariables();

    public object GetFunction(string functionName) => this.astExecutor.GetFunction(functionName);

    public object GetVariable(string variableName) => this.astExecutor.GetValue(variableName);

    public void SetVariable(string variableName, object value) => this.astExecutor.SetValue(variableName, value);

    public object Invoke(string functionName, params object[] args) => this.astExecutor.Invoke(functionName, args);

    public object Invoke(object function, params object[] args) => this.astExecutor.Invoke(function, args);

    public void SetBreakpoint(ushort line) => this.astExecutor.SetBreakpoint(line);

    public void ClearBreakpoint(ushort line) => this.astExecutor.ClearBreakpoint(line);

    public ushort[] GetBreakpoints() => this.astExecutor.GetBreakpoints();

    private DebugAction DebugCallbackHandler()
    {
      if (this.DebugEvent == null)
        return DebugAction.StepOver;
      DebugEventArgs e = new DebugEventArgs(this.astExecutor.HitBreakpoint, this.astExecutor.SrcLine, this.astExecutor.SrcCol, this.astExecutor.SrcLength, this.astExecutor.GetLocals(), this.astExecutor.GetCallStack());
      this.DebugEvent(this, e);
      return e.Action;
    }
  }
}
