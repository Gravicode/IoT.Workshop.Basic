// Decompiled with JetBrains decompiler
// Type: Drivers.Vnc.EncodedRectangle
// Assembly: Drivers.Vnc, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9F530C3B-A831-444F-9A20-B91A45E3436E
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\Drivers.Vnc.dll

namespace Drivers.Vnc
{
  internal abstract class EncodedRectangle
  {
    protected FrameBuffer framebuffer;
    protected byte[] data;
    protected int x;
    protected int y;
    protected int width;
    protected int height;
    protected int bitsPerPixel;

    public EncodedRectangle(FrameBuffer framebuffer)
    {
      this.framebuffer = framebuffer;
      this.x = framebuffer.X;
      this.y = framebuffer.Y;
      this.width = framebuffer.Width;
      this.height = framebuffer.Height;
      this.bitsPerPixel = this.framebuffer.BitsPerPixel;
    }

    public byte[] Data
    {
      get => this.data;
      set => this.data = value;
    }

    public int BitsPerPixel
    {
      get => this.bitsPerPixel;
      set => this.bitsPerPixel = value;
    }

    public int X
    {
      get => this.x;
      set => this.x = value;
    }

    public int Y
    {
      get => this.y;
      set => this.y = value;
    }

    public int Width
    {
      get => this.width;
      set => this.width = value;
    }

    public int Height
    {
      get => this.height;
      set => this.height = value;
    }

    public abstract void Encode();
  }
}
