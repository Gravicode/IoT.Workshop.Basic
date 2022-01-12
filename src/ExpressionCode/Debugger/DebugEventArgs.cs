// Decompiled with JetBrains decompiler
// Type: ExpressionCode.Debugger.DebugEventArgs
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

using System;

namespace ExpressionCode.Debugger
{
  public class DebugEventArgs : EventArgs
  {
    public readonly bool Breakpoint;
    public readonly ushort SrcLine;
    public readonly ushort SrcCol;
    public readonly ushort SrcLength;
    public readonly Variable[] Locals;
    public readonly string[] Callstack;

    public DebugAction Action { get; set; }

    public DebugEventArgs(
      bool breakPoint,
      ushort srcLine,
      ushort srcCol,
      ushort srcLength,
      Variable[] locals,
      string[] callstack)
    {
      this.Breakpoint = breakPoint;
      this.SrcLine = srcLine;
      this.SrcCol = srcCol;
      this.SrcLength = srcLength;
      this.Locals = locals;
      this.Callstack = callstack;
    }
  }
}
