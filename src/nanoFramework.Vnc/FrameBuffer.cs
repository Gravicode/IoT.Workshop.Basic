// Decompiled with JetBrains decompiler
// Type: Drivers.Vnc.FrameBuffer
// Assembly: Drivers.Vnc, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9F530C3B-A831-444F-9A20-B91A45E3436E
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\Drivers.Vnc.dll

using System;

namespace Drivers.Vnc
{
  internal class FrameBuffer
  {
    private string name;
    private int bpp;
    private int depth;
    private bool bigEndian;
    private bool trueColor;
    private int redMax;
    private int greenMax;
    private int blueMax;
    private int redShift;
    private int greenShift;
    private int blueShift;

    internal int X { get; set; }

    internal int Y { get; set; }

    internal int Width { get; set; }

    internal int Height { get; set; }

    public byte[] Data { get; internal set; }

    public FrameBuffer(int width, int height)
    {
      this.X = 0;
      this.Y = 0;
      this.Width = width;
      this.Height = height;
    }

    public int BitsPerPixel
    {
      get => this.bpp;
      set => this.bpp = value == 32 || value == 16 || value == 8 ? value : throw new ArgumentException("Wrong value for BitsPerPixel");
    }

    public int Depth
    {
      get => this.depth;
      set => this.depth = value;
    }

    public bool BigEndian
    {
      get => this.bigEndian;
      set => this.bigEndian = value;
    }

    public bool TrueColor
    {
      get => this.trueColor;
      set => this.trueColor = value;
    }

    public int RedMax
    {
      get => this.redMax;
      set => this.redMax = value;
    }

    public int GreenMax
    {
      get => this.greenMax;
      set => this.greenMax = value;
    }

    public int BlueMax
    {
      get => this.blueMax;
      set => this.blueMax = value;
    }

    public int RedShift
    {
      get => this.redShift;
      set => this.redShift = value;
    }

    public int GreenShift
    {
      get => this.greenShift;
      set => this.greenShift = value;
    }

    public int BlueShift
    {
      get => this.blueShift;
      set => this.blueShift = value;
    }

    public string ServerName
    {
      get => this.name;
      set => this.name = value ?? throw new ArgumentNullException(nameof (ServerName));
    }

    public byte[] ToPixelFormat() => new byte[16]
    {
      (byte) this.bpp,
      (byte) this.depth,
      this.bigEndian ? (byte) 1 : (byte) 0,
      this.trueColor ? (byte) 1 : (byte) 0,
      (byte) (this.redMax >> 8 & (int) byte.MaxValue),
      (byte) (this.redMax & (int) byte.MaxValue),
      (byte) (this.greenMax >> 8 & (int) byte.MaxValue),
      (byte) (this.greenMax & (int) byte.MaxValue),
      (byte) (this.blueMax >> 8 & (int) byte.MaxValue),
      (byte) (this.blueMax & (int) byte.MaxValue),
      (byte) this.redShift,
      (byte) this.greenShift,
      (byte) this.blueShift,
      (byte) 0,
      (byte) 0,
      (byte) 0
    };

    public static FrameBuffer FromPixelFormat(byte[] b, int width, int height)
    {
      if (b.Length != 16)
        throw new ArgumentException("Length of b must be 16 bytes.");
      return new FrameBuffer(width, height)
      {
        BitsPerPixel = (int) b[0],
        Depth = (int) b[1],
        BigEndian = b[2] > (byte) 0,
        TrueColor = b[3] > (byte) 0,
        RedMax = (int) b[5] | (int) b[4] << 8,
        GreenMax = (int) b[7] | (int) b[6] << 8,
        BlueMax = (int) b[9] | (int) b[8] << 8,
        RedShift = (int) b[10],
        GreenShift = (int) b[11],
        BlueShift = (int) b[12]
      };
    }
  }
}
