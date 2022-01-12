// Decompiled with JetBrains decompiler
// Type: ExpressionCode.IO.TextReaderEx
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

using System;
using System.Text;

namespace ExpressionCode.IO
{
  internal abstract class TextReaderEx : IDisposable
  {
    private const int BufferSize = 64;

    public virtual void Close() => this.Dispose();

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    public virtual int Peek() => -1;

    public virtual int Read() => -1;

    public virtual int Read(char[] buffer, int index, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException();
      if (index < 0 || count < 0 || buffer.Length - index > count)
        throw new ArgumentOutOfRangeException();
      int num1 = 0;
      do
      {
        int num2 = this.Read();
        if (num2 != -1)
        {
          buffer[index + num1] = (char) num2;
          ++num1;
        }
        else
          break;
      }
      while (num1 < count);
      return num1;
    }

    public virtual string ReadToEnd()
    {
      char[] buffer = new char[64];
      StringBuilder stringBuilder = new StringBuilder(64);
      int charCount;
      while ((charCount = this.Read(buffer, 0, buffer.Length)) > 0)
        stringBuilder.Append(buffer, 0, charCount);
      return stringBuilder.ToString();
    }

    public virtual int ReadBlock(char[] buffer, int index, int count)
    {
      int num1 = 0;
      int num2;
      do
      {
        num2 = this.Read(buffer, index + num1, count - num1);
        num1 += num2;
      }
      while (num2 > 0 && num1 < count);
      return num1;
    }

    public virtual string ReadLine()
    {
      StringBuilder stringBuilder = new StringBuilder();
      int num;
      while (true)
      {
        num = this.Read();
        switch (num)
        {
          case -1:
            goto label_6;
          case 10:
          case 13:
            goto label_2;
          default:
            stringBuilder.Append((char) num);
            continue;
        }
      }
label_2:
      if (num == 13 && this.Peek() == 10)
        this.Read();
      return stringBuilder.ToString();
label_6:
      return stringBuilder.Length > 0 ? stringBuilder.ToString() : (string) null;
    }
  }
}
