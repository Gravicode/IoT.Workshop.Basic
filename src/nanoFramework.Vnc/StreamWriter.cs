﻿// Decompiled with JetBrains decompiler
// Type: Drivers.Vnc.StreamWriter
// Assembly: Drivers.Vnc, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9F530C3B-A831-444F-9A20-B91A45E3436E
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\Drivers.Vnc.dll

using System;
using System.IO;

namespace Drivers.Vnc
{
  internal sealed class StreamWriter : Stream
  {
    private readonly Stream stream;

    public StreamWriter(Stream input) => this.stream = input;

    public override bool CanRead => throw new NotImplementedException();

    public override bool CanSeek => throw new NotImplementedException();

    public override bool CanWrite => throw new NotImplementedException();

    public override long Length => throw new NotImplementedException();

    public override long Position
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public override void Flush() => this.stream.Flush();

    public override int Read(byte[] buffer, int offset, int count) => this.stream.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotImplementedException();

    public void Write(byte value) => this.stream.WriteByte(value);

    public void Write(ushort value) => this.FlipAndWrite(BitConverter.GetBytes(value));

    public void Write(short value) => this.FlipAndWrite(BitConverter.GetBytes(value));

    public void Write(uint value) => this.FlipAndWrite(BitConverter.GetBytes(value));

    public void Write(int value) => this.FlipAndWrite(BitConverter.GetBytes(value));

    public void Write(ulong value) => this.FlipAndWrite(BitConverter.GetBytes(value));

    public void Write(long value) => this.FlipAndWrite(BitConverter.GetBytes(value));

    public void Write(byte[] buffer) => this.stream.Write(buffer, 0, buffer.Length);

    public override void Write(byte[] buffer, int offset, int count) => this.stream.Write(buffer, offset, count);

    private void FlipAndWrite(byte[] b)
    {
      BitConverter.SwapEndianness(b, b.Length);
      this.stream.Write(b, 0, b.Length);
    }
  }
}
