// Decompiled with JetBrains decompiler
// Type: Drivers.Vnc.StreamReader
// Assembly: Drivers.Vnc, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9F530C3B-A831-444F-9A20-B91A45E3436E
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\Drivers.Vnc.dll

using System;
using System.IO;

namespace Drivers.Vnc
{
  internal sealed class StreamReader : Stream
  {
    private readonly byte[] buff = new byte[4];
    private readonly Stream stream;

    public override bool CanRead => throw new NotImplementedException();

    public override bool CanSeek => throw new NotImplementedException();

    public override bool CanWrite => throw new NotImplementedException();

    public override long Length => throw new NotImplementedException();

    public override long Position
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public StreamReader(Stream input) => this.stream = input;

    public StreamReader(Stream input, System.Text.Encoding encoding) => this.stream = input;

    public ushort ReadUInt16()
    {
      this.FillBuff(2);
      return (ushort) ((uint) this.buff[1] | (uint) this.buff[0] << 8);
    }

    public short ReadInt16()
    {
      this.FillBuff(2);
      return (short) ((int) this.buff[1] & (int) byte.MaxValue | (int) this.buff[0] << 8);
    }

    public uint ReadUInt32()
    {
      this.FillBuff(4);
      return (uint) ((int) this.buff[3] & (int) byte.MaxValue | (int) this.buff[2] << 8 | (int) this.buff[1] << 16 | (int) this.buff[0] << 24);
    }

    public int ReadInt32()
    {
      this.FillBuff(4);
      return (int) this.buff[3] | (int) this.buff[2] << 8 | (int) this.buff[1] << 16 | (int) this.buff[0] << 24;
    }

    public byte[] ReadBytes(int count)
    {
      byte[] buffer = new byte[count];
      int num = 0;
      while (num < count)
        num += this.stream.Read(buffer, 0, count - num);
      return buffer;
    }

    private void FillBuff(int totalBytes)
    {
      int offset = 0;
      do
      {
        int num = this.stream.Read(this.buff, offset, totalBytes - offset);
        if (num == 0)
          throw new IOException("Unable to read next byte(s).");
        offset += num;
      }
      while (offset < totalBytes);
    }

    public override void Flush() => this.stream.Flush();

    public override long Seek(long offset, SeekOrigin origin) => this.stream.Seek(offset, origin);

    public override void SetLength(long value) => this.stream.SetLength(value);

    public override int Read(byte[] buffer, int offset, int count) => this.stream.Read(buffer, offset, count);

    public override void Write(byte[] buffer, int offset, int count) => this.stream.Write(buffer, offset, count);

    public virtual bool DataAvailable => this.stream.DataAvailable;
  }
}
