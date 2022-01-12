// Decompiled with JetBrains decompiler
// Type: ExpressionCode.IO.StringReaderEx
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

using System;

namespace ExpressionCode.IO
{
  internal class StringReaderEx : TextReaderEx
  {
    private string s;
    private int pos;
    private int len;

    public StringReaderEx(string s)
    {
      this.s = s ?? throw new ArgumentNullException();
      this.len = s.Length;
    }

    protected override void Dispose(bool disposing)
    {
      this.s = (string) null;
      this.pos = 0;
      this.len = 0;
      base.Dispose(disposing);
    }

    public override int Peek()
    {
      if (this.s == null)
        throw new ObjectDisposedException(null);
      return this.pos == this.len ? -1 : (int) this.s[this.pos];
    }

    public override int Read()
    {
      if (this.s == null)
        throw new ObjectDisposedException(null);
      return this.pos == this.len ? -1 : (int) this.s[this.pos++];
    }

    public override string ReadToEnd()
    {
      if (this.s == null)
        throw new ObjectDisposedException(null);
      string str = this.pos != 0 ? this.s.Substring(this.pos, this.len - this.pos) : this.s;
      this.pos = this.len;
      return str;
    }
  }
}
