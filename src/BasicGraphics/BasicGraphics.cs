
using System;

namespace BMC.Drivers.BasicGraphics
{
  public class BasicGraphics
  {
    private ColorFormat colorFormat;
    private byte[] buffer;
    private int width;
    private int height;
    private readonly byte[] mono5x5 = new byte[475]
    {
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 0,
      (byte) 8,
      (byte) 10,
      (byte) 74,
      (byte) 64,
      (byte) 0,
      (byte) 0,
      (byte) 10,
      (byte) 95,
      (byte) 234,
      (byte) 95,
      (byte) 234,
      (byte) 14,
      (byte) 217,
      (byte) 46,
      (byte) 211,
      (byte) 110,
      (byte) 25,
      (byte) 50,
      (byte) 68,
      (byte) 137,
      (byte) 51,
      (byte) 12,
      (byte) 146,
      (byte) 76,
      (byte) 146,
      (byte) 77,
      (byte) 8,
      (byte) 8,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 4,
      (byte) 136,
      (byte) 8,
      (byte) 8,
      (byte) 4,
      (byte) 8,
      (byte) 4,
      (byte) 132,
      (byte) 132,
      (byte) 136,
      (byte) 0,
      (byte) 10,
      (byte) 68,
      (byte) 138,
      (byte) 64,
      (byte) 0,
      (byte) 4,
      (byte) 142,
      (byte) 196,
      (byte) 128,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 4,
      (byte) 136,
      (byte) 0,
      (byte) 0,
      (byte) 14,
      (byte) 192,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 8,
      (byte) 0,
      (byte) 1,
      (byte) 34,
      (byte) 68,
      (byte) 136,
      (byte) 16,
      (byte) 12,
      (byte) 146,
      (byte) 82,
      (byte) 82,
      (byte) 76,
      (byte) 4,
      (byte) 140,
      (byte) 132,
      (byte) 132,
      (byte) 142,
      (byte) 28,
      (byte) 130,
      (byte) 76,
      (byte) 144,
      (byte) 30,
      (byte) 30,
      (byte) 194,
      (byte) 68,
      (byte) 146,
      (byte) 76,
      (byte) 6,
      (byte) 202,
      (byte) 82,
      (byte) 95,
      (byte) 226,
      (byte) 31,
      (byte) 240,
      (byte) 30,
      (byte) 193,
      (byte) 62,
      (byte) 2,
      (byte) 68,
      (byte) 142,
      (byte) 209,
      (byte) 46,
      (byte) 31,
      (byte) 226,
      (byte) 68,
      (byte) 136,
      (byte) 16,
      (byte) 14,
      (byte) 209,
      (byte) 46,
      (byte) 209,
      (byte) 46,
      (byte) 14,
      (byte) 209,
      (byte) 46,
      (byte) 196,
      (byte) 136,
      (byte) 0,
      (byte) 8,
      (byte) 0,
      (byte) 8,
      (byte) 0,
      (byte) 0,
      (byte) 4,
      (byte) 128,
      (byte) 4,
      (byte) 136,
      (byte) 2,
      (byte) 68,
      (byte) 136,
      (byte) 4,
      (byte) 130,
      (byte) 0,
      (byte) 14,
      (byte) 192,
      (byte) 14,
      (byte) 192,
      (byte) 8,
      (byte) 4,
      (byte) 130,
      (byte) 68,
      (byte) 136,
      (byte) 14,
      (byte) 209,
      (byte) 38,
      (byte) 192,
      (byte) 4,
      (byte) 14,
      (byte) 209,
      (byte) 53,
      (byte) 179,
      (byte) 108,
      (byte) 12,
      (byte) 146,
      (byte) 94,
      (byte) 210,
      (byte) 82,
      (byte) 28,
      (byte) 146,
      (byte) 92,
      (byte) 146,
      (byte) 92,
      (byte) 14,
      (byte) 208,
      (byte) 16,
      (byte) 16,
      (byte) 14,
      (byte) 28,
      (byte) 146,
      (byte) 82,
      (byte) 82,
      (byte) 92,
      (byte) 30,
      (byte) 208,
      (byte) 28,
      (byte) 144,
      (byte) 30,
      (byte) 30,
      (byte) 208,
      (byte) 28,
      (byte) 144,
      (byte) 16,
      (byte) 14,
      (byte) 208,
      (byte) 19,
      (byte) 113,
      (byte) 46,
      (byte) 18,
      (byte) 82,
      (byte) 94,
      (byte) 210,
      (byte) 82,
      (byte) 28,
      (byte) 136,
      (byte) 8,
      (byte) 8,
      (byte) 28,
      (byte) 31,
      (byte) 226,
      (byte) 66,
      (byte) 82,
      (byte) 76,
      (byte) 18,
      (byte) 84,
      (byte) 152,
      (byte) 20,
      (byte) 146,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 30,
      (byte) 17,
      (byte) 59,
      (byte) 117,
      (byte) 177,
      (byte) 49,
      (byte) 17,
      (byte) 57,
      (byte) 53,
      (byte) 179,
      (byte) 113,
      (byte) 12,
      (byte) 146,
      (byte) 82,
      (byte) 82,
      (byte) 76,
      (byte) 28,
      (byte) 146,
      (byte) 92,
      (byte) 144,
      (byte) 16,
      (byte) 12,
      (byte) 146,
      (byte) 82,
      (byte) 76,
      (byte) 134,
      (byte) 28,
      (byte) 146,
      (byte) 92,
      (byte) 146,
      (byte) 81,
      (byte) 14,
      (byte) 208,
      (byte) 12,
      (byte) 130,
      (byte) 92,
      (byte) 31,
      (byte) 228,
      (byte) 132,
      (byte) 132,
      (byte) 132,
      (byte) 18,
      (byte) 82,
      (byte) 82,
      (byte) 82,
      (byte) 76,
      (byte) 17,
      (byte) 49,
      (byte) 49,
      (byte) 42,
      (byte) 68,
      (byte) 17,
      (byte) 49,
      (byte) 53,
      (byte) 187,
      (byte) 113,
      (byte) 18,
      (byte) 82,
      (byte) 76,
      (byte) 146,
      (byte) 82,
      (byte) 17,
      (byte) 42,
      (byte) 68,
      (byte) 132,
      (byte) 132,
      (byte) 30,
      (byte) 196,
      (byte) 136,
      (byte) 16,
      (byte) 30,
      (byte) 14,
      (byte) 200,
      (byte) 8,
      (byte) 8,
      (byte) 14,
      (byte) 16,
      (byte) 8,
      (byte) 4,
      (byte) 130,
      (byte) 65,
      (byte) 14,
      (byte) 194,
      (byte) 66,
      (byte) 66,
      (byte) 78,
      (byte) 4,
      (byte) 138,
      (byte) 64,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 31,
      (byte) 8,
      (byte) 4,
      (byte) 128,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 14,
      (byte) 210,
      (byte) 82,
      (byte) 79,
      (byte) 16,
      (byte) 16,
      (byte) 28,
      (byte) 146,
      (byte) 92,
      (byte) 0,
      (byte) 14,
      (byte) 208,
      (byte) 16,
      (byte) 14,
      (byte) 2,
      (byte) 66,
      (byte) 78,
      (byte) 210,
      (byte) 78,
      (byte) 12,
      (byte) 146,
      (byte) 92,
      (byte) 144,
      (byte) 14,
      (byte) 6,
      (byte) 200,
      (byte) 28,
      (byte) 136,
      (byte) 8,
      (byte) 14,
      (byte) 210,
      (byte) 78,
      (byte) 194,
      (byte) 76,
      (byte) 16,
      (byte) 16,
      (byte) 28,
      (byte) 146,
      (byte) 82,
      (byte) 8,
      (byte) 0,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 2,
      (byte) 64,
      (byte) 2,
      (byte) 66,
      (byte) 76,
      (byte) 16,
      (byte) 20,
      (byte) 152,
      (byte) 20,
      (byte) 146,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 6,
      (byte) 0,
      (byte) 27,
      (byte) 117,
      (byte) 177,
      (byte) 49,
      (byte) 0,
      (byte) 28,
      (byte) 146,
      (byte) 82,
      (byte) 82,
      (byte) 0,
      (byte) 12,
      (byte) 146,
      (byte) 82,
      (byte) 76,
      (byte) 0,
      (byte) 28,
      (byte) 146,
      (byte) 92,
      (byte) 144,
      (byte) 0,
      (byte) 14,
      (byte) 210,
      (byte) 78,
      (byte) 194,
      (byte) 0,
      (byte) 14,
      (byte) 208,
      (byte) 16,
      (byte) 16,
      (byte) 0,
      (byte) 6,
      (byte) 200,
      (byte) 4,
      (byte) 152,
      (byte) 8,
      (byte) 8,
      (byte) 14,
      (byte) 200,
      (byte) 7,
      (byte) 0,
      (byte) 18,
      (byte) 82,
      (byte) 82,
      (byte) 79,
      (byte) 0,
      (byte) 17,
      (byte) 49,
      (byte) 42,
      (byte) 68,
      (byte) 0,
      (byte) 17,
      (byte) 49,
      (byte) 53,
      (byte) 187,
      (byte) 0,
      (byte) 18,
      (byte) 76,
      (byte) 140,
      (byte) 146,
      (byte) 0,
      (byte) 17,
      (byte) 42,
      (byte) 68,
      (byte) 152,
      (byte) 0,
      (byte) 30,
      (byte) 196,
      (byte) 136,
      (byte) 30,
      (byte) 6,
      (byte) 196,
      (byte) 140,
      (byte) 132,
      (byte) 134,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 24,
      (byte) 8,
      (byte) 12,
      (byte) 136,
      (byte) 24,
      (byte) 0,
      (byte) 0,
      (byte) 12,
      (byte) 131,
      (byte) 96
    };
    private readonly byte[] mono8x5 = new byte[475]
    {
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 79,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 7,
      (byte) 0,
      (byte) 7,
      (byte) 0,
      (byte) 20,
      (byte) 127,
      (byte) 20,
      (byte) 127,
      (byte) 20,
      (byte) 36,
      (byte) 42,
      (byte) 127,
      (byte) 42,
      (byte) 18,
      (byte) 35,
      (byte) 19,
      (byte) 8,
      (byte) 100,
      (byte) 98,
      (byte) 54,
      (byte) 73,
      (byte) 85,
      (byte) 34,
      (byte) 32,
      (byte) 0,
      (byte) 5,
      (byte) 3,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 28,
      (byte) 34,
      (byte) 65,
      (byte) 0,
      (byte) 0,
      (byte) 65,
      (byte) 34,
      (byte) 28,
      (byte) 0,
      (byte) 20,
      (byte) 8,
      (byte) 62,
      (byte) 8,
      (byte) 20,
      (byte) 8,
      (byte) 8,
      (byte) 62,
      (byte) 8,
      (byte) 8,
      (byte) 80,
      (byte) 48,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 0,
      (byte) 96,
      (byte) 96,
      (byte) 0,
      (byte) 0,
      (byte) 32,
      (byte) 16,
      (byte) 8,
      (byte) 4,
      (byte) 2,
      (byte) 62,
      (byte) 81,
      (byte) 73,
      (byte) 69,
      (byte) 62,
      (byte) 0,
      (byte) 66,
      (byte) 127,
      (byte) 64,
      (byte) 0,
      (byte) 66,
      (byte) 97,
      (byte) 81,
      (byte) 73,
      (byte) 70,
      (byte) 33,
      (byte) 65,
      (byte) 69,
      (byte) 75,
      (byte) 49,
      (byte) 24,
      (byte) 20,
      (byte) 18,
      (byte) 127,
      (byte) 16,
      (byte) 39,
      (byte) 69,
      (byte) 69,
      (byte) 69,
      (byte) 57,
      (byte) 60,
      (byte) 74,
      (byte) 73,
      (byte) 73,
      (byte) 48,
      (byte) 1,
      (byte) 113,
      (byte) 9,
      (byte) 5,
      (byte) 3,
      (byte) 54,
      (byte) 73,
      (byte) 73,
      (byte) 73,
      (byte) 54,
      (byte) 6,
      (byte) 73,
      (byte) 73,
      (byte) 41,
      (byte) 30,
      (byte) 0,
      (byte) 54,
      (byte) 54,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 86,
      (byte) 54,
      (byte) 0,
      (byte) 0,
      (byte) 8,
      (byte) 20,
      (byte) 34,
      (byte) 65,
      (byte) 0,
      (byte) 20,
      (byte) 20,
      (byte) 20,
      (byte) 20,
      (byte) 20,
      (byte) 0,
      (byte) 65,
      (byte) 34,
      (byte) 20,
      (byte) 8,
      (byte) 2,
      (byte) 1,
      (byte) 81,
      (byte) 9,
      (byte) 6,
      (byte) 62,
      (byte) 65,
      (byte) 93,
      (byte) 85,
      (byte) 30,
      (byte) 126,
      (byte) 17,
      (byte) 17,
      (byte) 17,
      (byte) 126,
      (byte) 127,
      (byte) 73,
      (byte) 73,
      (byte) 73,
      (byte) 54,
      (byte) 62,
      (byte) 65,
      (byte) 65,
      (byte) 65,
      (byte) 34,
      (byte) 127,
      (byte) 65,
      (byte) 65,
      (byte) 34,
      (byte) 28,
      (byte) 127,
      (byte) 73,
      (byte) 73,
      (byte) 73,
      (byte) 65,
      (byte) 127,
      (byte) 9,
      (byte) 9,
      (byte) 9,
      (byte) 1,
      (byte) 62,
      (byte) 65,
      (byte) 73,
      (byte) 73,
      (byte) 122,
      (byte) 127,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 127,
      (byte) 0,
      (byte) 65,
      (byte) 127,
      (byte) 65,
      (byte) 0,
      (byte) 32,
      (byte) 64,
      (byte) 65,
      (byte) 63,
      (byte) 1,
      (byte) 127,
      (byte) 8,
      (byte) 20,
      (byte) 34,
      (byte) 65,
      (byte) 127,
      (byte) 64,
      (byte) 64,
      (byte) 64,
      (byte) 64,
      (byte) 127,
      (byte) 2,
      (byte) 12,
      (byte) 2,
      (byte) 127,
      (byte) 127,
      (byte) 4,
      (byte) 8,
      (byte) 16,
      (byte) 127,
      (byte) 62,
      (byte) 65,
      (byte) 65,
      (byte) 65,
      (byte) 62,
      (byte) 127,
      (byte) 9,
      (byte) 9,
      (byte) 9,
      (byte) 6,
      (byte) 62,
      (byte) 65,
      (byte) 81,
      (byte) 33,
      (byte) 94,
      (byte) 127,
      (byte) 9,
      (byte) 25,
      (byte) 41,
      (byte) 70,
      (byte) 38,
      (byte) 73,
      (byte) 73,
      (byte) 73,
      (byte) 50,
      (byte) 1,
      (byte) 1,
      (byte) 127,
      (byte) 1,
      (byte) 1,
      (byte) 63,
      (byte) 64,
      (byte) 64,
      (byte) 64,
      (byte) 63,
      (byte) 31,
      (byte) 32,
      (byte) 64,
      (byte) 32,
      (byte) 31,
      (byte) 63,
      (byte) 64,
      (byte) 56,
      (byte) 64,
      (byte) 63,
      (byte) 99,
      (byte) 20,
      (byte) 8,
      (byte) 20,
      (byte) 99,
      (byte) 7,
      (byte) 8,
      (byte) 112,
      (byte) 8,
      (byte) 7,
      (byte) 97,
      (byte) 81,
      (byte) 73,
      (byte) 69,
      (byte) 67,
      (byte) 0,
      (byte) 127,
      (byte) 65,
      (byte) 65,
      (byte) 0,
      (byte) 2,
      (byte) 4,
      (byte) 8,
      (byte) 16,
      (byte) 32,
      (byte) 0,
      (byte) 65,
      (byte) 65,
      (byte) 127,
      (byte) 0,
      (byte) 4,
      (byte) 2,
      (byte) 1,
      (byte) 2,
      (byte) 4,
      (byte) 64,
      (byte) 64,
      (byte) 64,
      (byte) 64,
      (byte) 64,
      (byte) 0,
      (byte) 0,
      (byte) 3,
      (byte) 5,
      (byte) 0,
      (byte) 32,
      (byte) 84,
      (byte) 84,
      (byte) 84,
      (byte) 120,
      (byte) 127,
      (byte) 68,
      (byte) 68,
      (byte) 68,
      (byte) 56,
      (byte) 56,
      (byte) 68,
      (byte) 68,
      (byte) 68,
      (byte) 68,
      (byte) 56,
      (byte) 68,
      (byte) 68,
      (byte) 68,
      (byte) 127,
      (byte) 56,
      (byte) 84,
      (byte) 84,
      (byte) 84,
      (byte) 24,
      (byte) 4,
      (byte) 4,
      (byte) 126,
      (byte) 5,
      (byte) 5,
      (byte) 8,
      (byte) 84,
      (byte) 84,
      (byte) 84,
      (byte) 60,
      (byte) 127,
      (byte) 8,
      (byte) 4,
      (byte) 4,
      (byte) 120,
      (byte) 0,
      (byte) 68,
      (byte) 125,
      (byte) 64,
      (byte) 0,
      (byte) 32,
      (byte) 64,
      (byte) 68,
      (byte) 61,
      (byte) 0,
      (byte) 127,
      (byte) 16,
      (byte) 40,
      (byte) 68,
      (byte) 0,
      (byte) 0,
      (byte) 65,
      (byte) 127,
      (byte) 64,
      (byte) 0,
      (byte) 124,
      (byte) 4,
      (byte) 124,
      (byte) 4,
      (byte) 120,
      (byte) 124,
      (byte) 8,
      (byte) 4,
      (byte) 4,
      (byte) 120,
      (byte) 56,
      (byte) 68,
      (byte) 68,
      (byte) 68,
      (byte) 56,
      (byte) 124,
      (byte) 20,
      (byte) 20,
      (byte) 20,
      (byte) 8,
      (byte) 8,
      (byte) 20,
      (byte) 20,
      (byte) 20,
      (byte) 124,
      (byte) 124,
      (byte) 8,
      (byte) 4,
      (byte) 4,
      (byte) 8,
      (byte) 72,
      (byte) 84,
      (byte) 84,
      (byte) 84,
      (byte) 36,
      (byte) 4,
      (byte) 4,
      (byte) 63,
      (byte) 68,
      (byte) 68,
      (byte) 60,
      (byte) 64,
      (byte) 64,
      (byte) 32,
      (byte) 124,
      (byte) 28,
      (byte) 32,
      (byte) 64,
      (byte) 32,
      (byte) 28,
      (byte) 60,
      (byte) 64,
      (byte) 48,
      (byte) 64,
      (byte) 60,
      (byte) 68,
      (byte) 40,
      (byte) 16,
      (byte) 40,
      (byte) 68,
      (byte) 12,
      (byte) 80,
      (byte) 80,
      (byte) 80,
      (byte) 60,
      (byte) 68,
      (byte) 100,
      (byte) 84,
      (byte) 76,
      (byte) 68,
      (byte) 8,
      (byte) 54,
      (byte) 65,
      (byte) 65,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 119,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 65,
      (byte) 65,
      (byte) 54,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 42,
      (byte) 28,
      (byte) 8
    };

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

    public ColorFormat ColorFormat
    {
      get => this.ColorFormat;
      set => this.ColorFormat = value;
    }

    public byte[] Buffer => this.buffer;

    public BasicGraphics()
    {
    }

    public BasicGraphics(uint width, uint height, ColorFormat colorFormat)
    {
      this.colorFormat = colorFormat;
      this.width = (int) width;
      this.height = (int) height;
      if (this.colorFormat == ColorFormat.Rgb565)
      {
        this.buffer = new byte[this.width * this.height * 2];
      }
      else
      {
        if (this.colorFormat != ColorFormat.OneBpp)
          return;
        this.buffer = new byte[this.width * this.height / 8];
      }
    }

    public virtual void Clear()
    {
      if (this.buffer == null)
        return;
      Array.Clear((Array) this.buffer, 0, this.buffer.Length);
    }

    public virtual void SetPixel(int x, int y, uint color)
    {
      if (x < 0 || y < 0 || x >= this.width || y >= this.height)
        return;
      if (this.colorFormat == ColorFormat.Rgb565)
      {
        int index = (y * this.width + x) * 2;
        uint num = color;
        this.buffer[index] = (byte) ((num & 7168U) >> 5 | (num & 248U) >> 3);
        this.buffer[index + 1] = (byte) ((num & 16252928U) >> 16 | (num & 57344U) >> 13);
      }
      else
      {
        if (this.colorFormat != ColorFormat.OneBpp)
          throw new Exception("Only 16bpp or 1bpp supported.");
        int index = (y >> 3) * this.width + x;
        if (color != 0U)
          this.buffer[index] |= (byte) (1 << (y & 7));
        else
          this.buffer[index] &= (byte) ~(1 << (y & 7));
      }
    }

    public void DrawLine(uint color, int x0, int y0, int x1, int y1)
    {
      int num1 = x1 - x0;
      int num2 = y1 - y0;
      int num3;
      if (num2 < 0)
      {
        num2 = -num2;
        num3 = -1;
      }
      else
        num3 = 1;
      int num4;
      if (num1 < 0)
      {
        num1 = -num1;
        num4 = -1;
      }
      else
        num4 = 1;
      int num5 = num2 << 1;
      int num6 = num1 << 1;
      this.SetPixel(x0, y0, color);
      if (num6 > num5)
      {
        int num7 = num5 - (num6 >> 1);
        while (x0 != x1)
        {
          if (num7 >= 0)
          {
            y0 += num3;
            num7 -= num6;
          }
          x0 += num4;
          num7 += num5;
          this.SetPixel(x0, y0, color);
        }
      }
      else
      {
        int num8 = num6 - (num5 >> 1);
        while (y0 != y1)
        {
          if (num8 >= 0)
          {
            x0 += num4;
            num8 -= num5;
          }
          y0 += num3;
          num8 += num6;
          this.SetPixel(x0, y0, color);
        }
      }
    }

    public void DrawRectangle(uint color, int x, int y, int width, int height)
    {
      if (width < 0 || height < 0)
        return;
      for (int x1 = x; x1 < x + width; ++x1)
      {
        this.SetPixel(x1, y, color);
        this.SetPixel(x1, y + height - 1, color);
      }
      for (int y1 = y; y1 < y + height; ++y1)
      {
        this.SetPixel(x, y1, color);
        this.SetPixel(x + width - 1, y1, color);
      }
    }

    public void DrawCircle(uint color, int x, int y, int radius)
    {
      if (radius <= 0)
        return;
      int x1 = x;
      int y1 = y;
      int num1 = 1 - radius;
      int num2 = 1;
      int num3 = -2 * radius;
      int num4 = 0;
      int num5 = radius;
      this.SetPixel(x1, y1 + radius, color);
      this.SetPixel(x1, y1 - radius, color);
      this.SetPixel(x1 + radius, y1, color);
      this.SetPixel(x1 - radius, y1, color);
      while (num4 < num5)
      {
        if (num1 >= 0)
        {
          --num5;
          num3 += 2;
          num1 += num3;
        }
        ++num4;
        num2 += 2;
        num1 += num2;
        this.SetPixel(x1 + num4, y1 + num5, color);
        this.SetPixel(x1 - num4, y1 + num5, color);
        this.SetPixel(x1 + num4, y1 - num5, color);
        this.SetPixel(x1 - num4, y1 - num5, color);
        this.SetPixel(x1 + num5, y1 + num4, color);
        this.SetPixel(x1 - num5, y1 + num4, color);
        this.SetPixel(x1 + num5, y1 - num4, color);
        this.SetPixel(x1 - num5, y1 - num4, color);
      }
    }

    public void DrawTinyString(string text, uint color, int x, int y) => this.DrawTinyString(text, color, x, y, false);

    public void DrawTinyString(string text, uint color, int x, int y, bool clear)
    {
      for (int index1 = 0; index1 < text.Length; ++index1)
      {
        this.DrawTinyCharacter(text[index1], color, x, y, clear);
        x += 6;
        if (clear)
        {
          for (int index2 = 0; index2 < 5; ++index2)
            this.SetPixel(x - 1, y + index2, 0U);
        }
      }
    }

    public void DrawString(string text, uint color, int x, int y) => this.DrawString(text, color, x, y, 1, 1);

    public void DrawString(string text, uint color, int x, int y, int hScale, int vScale)
    {
      if (hScale == 0 || vScale == 0)
        throw new ArgumentNullException();
      int num = x;
      for (int index = 0; index < text.Length; ++index)
      {
        if (text[index] >= ' ')
        {
          this.DrawCharacter(text[index], color, x, y, hScale, vScale);
          x += 6;
        }
        else
        {
          if (text[index] == '\n')
          {
            y += 9;
            x = num;
          }
          if (text[index] == '\r')
            x = num;
        }
      }
    }

    public void DrawTinyCharacter(char character, uint color, int x, int y) => this.DrawTinyCharacter(character, color, x, y, false);

    public void DrawTinyCharacter(char character, uint color, int x, int y, bool clear)
    {
      int num1 = 5 * ((int) character - 32);
      for (int index1 = 0; index1 < 5; ++index1)
      {
        byte num2 = this.mono5x5[num1 + index1];
        for (int index2 = 0; index2 < 5; ++index2)
        {
          if (((int) num2 & 1 << 4 - index2) != 0)
            this.SetPixel(x + index2, y + index1, color);
          else if (clear)
            this.SetPixel(x + index2, y + index1, 0U);
        }
      }
    }

    public void DrawCharacter(char character, uint color, int x, int y) => this.DrawCharacter(character, color, x, y, 1, 1);

    public void DrawCharacter(char character, uint color, int x, int y, int hScale, int vScale)
    {
      int num1 = 5 * ((int) character - 32);
      if (hScale != 1 || vScale != 1)
      {
        for (int index1 = 0; index1 < 5; ++index1)
        {
          int x1 = x + index1;
          byte num2 = this.mono8x5[num1 + index1];
          for (int index2 = 0; index2 < 8; ++index2)
          {
            if (((int) num2 & 1 << index2) != 0)
              this.SetPixel(x1, y + index2, hScale, vScale, color);
          }
        }
      }
      else
      {
        for (int index3 = 0; index3 < 5; ++index3)
        {
          int x2 = x + index3;
          byte num3 = this.mono8x5[num1 + index3];
          for (int index4 = 0; index4 < 8; ++index4)
          {
            if (((int) num3 & 1 << index4) != 0)
              this.SetPixel(x2, y + index4, color);
          }
        }
      }
    }

    private void SetPixel(int x, int y, int hScale, int vScale, uint color)
    {
      x *= hScale;
      y *= vScale;
      for (int index1 = 0; index1 < hScale; ++index1)
      {
        for (int index2 = 0; index2 < vScale; ++index2)
          this.SetPixel(x + index1, y + index2, color);
      }
    }

    public static uint ColorFromRgb(byte red, byte green, byte blue) => (uint) ((int) red << 16 | (int) green << 8) | (uint) blue;
  }
}
