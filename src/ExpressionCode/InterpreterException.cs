// Decompiled with JetBrains decompiler
// Type: ExpressionCode.InterpreterException
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

using System;

namespace ExpressionCode
{
  public class InterpreterException : Exception
  {
    public ushort SrcLine { get; private set; }

    public ushort SrcCol { get; private set; }

    public InterpreterException(
      string message,
      ushort srcLine,
      ushort srcCol,
      Exception innerException = null)
      : base(message, innerException)
    {
      this.SrcLine = srcLine;
      this.SrcCol = srcCol;
    }

    public InterpreterException(string message, Exception innerException = null)
      : base(message, innerException)
    {
      this.SrcLine = ushort.MaxValue;
      this.SrcCol = ushort.MaxValue;
    }
  }
}
