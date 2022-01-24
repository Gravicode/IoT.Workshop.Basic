// Decompiled with JetBrains decompiler
// Type: Drivers.Vnc.RawRectangle
// Assembly: Drivers.Vnc, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9F530C3B-A831-444F-9A20-B91A45E3436E
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\Drivers.Vnc.dll

//using GHIElectronics.TinyCLR.Native;
using nanoFramework.Presentation.Media;
using nanoFramework.Tools;
using System;
//using System.Drawing;

namespace Drivers.Vnc
{
  internal sealed class RawRectangle : EncodedRectangle, IDisposable
  {
    private UnmanagedBuffer unmanagedBuffer;

    public RawRectangle(FrameBuffer framebuffer)
      : base(framebuffer)
    {
      if (Memory.UnmanagedMemory.FreeBytes > 0L)
      {
        this.unmanagedBuffer = new UnmanagedBuffer(this.Width * this.Height * (this.framebuffer.BitsPerPixel / 8));
        this.data = this.unmanagedBuffer.Bytes;
      }
      else
        this.data = new byte[this.Width * this.Height * (this.framebuffer.BitsPerPixel / 8)];
    }

    public void Dispose()
    {
      if (this.unmanagedBuffer == null)
        return;
      this.data = (byte[]) null;
      this.unmanagedBuffer.Dispose();
      this.unmanagedBuffer = (UnmanagedBuffer) null;
    }

    public override void Encode()
    {
      Color.ColorFormat colorFormat = Color.ColorFormat.Rgb8888;
      int num = 4;
      switch (this.framebuffer.BitsPerPixel)
      {
        case 8:
          colorFormat = Color.ColorFormat.Rgb332;
          num = 1;
          break;
        case 16:
          colorFormat = Color.ColorFormat.Rgb565;
          num = 2;
          break;
      }
      Color.Convert(this.framebuffer.Data, this.data, colorFormat);
      BitConverterHelper.SwapEndianness(this.data, num);
    }

    ~RawRectangle() => this.Dispose();
  }
}
