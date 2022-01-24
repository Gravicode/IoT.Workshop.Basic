// Decompiled with JetBrains decompiler
// Type: nanoFramework.ILI9341
// Assembly: GHIElectronics.TinyCLR.Drivers.HiLetgo.ILI9341, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D6AEC6E3-6A2A-4FFF-B379-B855C36D4A3B
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\GHIElectronics.TinyCLR.Drivers.HiLetgo.ILI9341.dll

//using GHIElectronics.TinyCLR.Devices.Gpio;
//using GHIElectronics.TinyCLR.Devices.Spi;
using nanoFramework.Tools;
using System;
using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;

namespace nanoFramework.ILI9341
{
  public class ILI9341
  {
    public enum ILI9341CommandId : byte
    {
      MADCTL_RGB = 0,
      SWRESET = 1,
      MADCTL_BGR = 8,
      SLPOUT = 17, // 0x11
      INVOFF = 32, // 0x20
      MADCTL_MV = 32, // 0x20
      INVON = 33, // 0x21
      GAMMASET = 38, // 0x26
      DISPOFF = 40, // 0x28
      DISPON = 41, // 0x29
      CASET = 42, // 0x2A
      PASET = 43, // 0x2B
      RAMWR = 44, // 0x2C
      MADCTL = 54, // 0x36
      PIXFMT = 58, // 0x3A
      MADCTL_MX = 64, // 0x40
      MADCTL_MY = 128, // 0x80
      FRMCTR1 = 177, // 0xB1
      DFUNCTR = 182, // 0xB6
      EMSET = 183, // 0xB7
    }

    public class ILI9341Controller
    {
      private readonly byte[] buffer1 = new byte[1];
      private readonly SpiDevice spi;
      private readonly GpioPin control;
      private readonly GpioPin reset;
      private bool rowColumnSwapped;
      private int bpp = 16;

      public int Width { get; private set; }

      public int Height { get; private set; }

      public int MaxWidth => !this.rowColumnSwapped ? 240 : 320;

      public int MaxHeight => !this.rowColumnSwapped ? 320 : 240;

            public static SpiConnectionSettings GetConnectionSettings(int BusId,
              PinValue chipSelectType,
              int chipSelectLine)
            {
                var con = new SpiConnectionSettings(BusId, chipSelectLine);

                con.Mode = SpiMode.Mode0;
                con.ClockFrequency = 12000000;
                con.ChipSelectLineActiveState = chipSelectType;
                con.ChipSelectLine = chipSelectLine;
                return con;
            }

      public ILI9341Controller(SpiDevice spi, GpioPin control)
        : this(spi, control, (GpioPin) null)
      {
      }

      public ILI9341Controller(SpiDevice spi, GpioPin control, GpioPin reset)
      {
        this.spi = spi;
        this.control = control;
        this.control.SetPinMode(PinMode.Output);
        this.reset = reset;
        this.reset?.SetPinMode(PinMode.Output);
        this.Reset();
        this.Initialize();
        this.SetDataAccessControl(false, false, false, true);
        this.SetDrawWindow(0, 0, this.MaxWidth - 1, this.MaxHeight - 1);
        this.Enable();
      }

      private void Reset()
      {
        this.reset?.Write(PinValue.Low);
        Thread.Sleep(50);
        this.reset?.Write(PinValue.High);
        Thread.Sleep(200);
      }

      private void Initialize()
      {
        this.SendCommand(ILI9341CommandId.SWRESET);
        Thread.Sleep(10);
        this.SendCommand(ILI9341CommandId.DISPOFF);
        this.SendCommand(ILI9341CommandId.MADCTL);
        this.SendData((byte) 72);
        this.SendCommand(ILI9341CommandId.PIXFMT);
        this.SendData((byte) 85);
        this.SendCommand(ILI9341CommandId.FRMCTR1);
        this.SendData((byte) 0);
        this.SendData((byte) 27);
        this.SendCommand(ILI9341CommandId.GAMMASET);
        this.SendData((byte) 1);
        this.SendCommand(ILI9341CommandId.CASET);
        this.SendData((byte) 0);
        this.SendData((byte) 0);
        this.SendData((byte) 0);
        this.SendData((byte) 239);
        this.SendCommand(ILI9341CommandId.PASET);
        this.SendData((byte) 0);
        this.SendData((byte) 0);
        this.SendData((byte) 1);
        this.SendData((byte) 63);
        this.SendCommand(ILI9341CommandId.EMSET);
        this.SendData((byte) 7);
        this.SendCommand(ILI9341CommandId.DFUNCTR);
        this.SendData((byte) 10);
        this.SendData((byte) 130);
        this.SendData((byte) 39);
        this.SendData((byte) 0);
        this.SendCommand(ILI9341CommandId.SLPOUT);
        Thread.Sleep(120);
        this.SendCommand(ILI9341CommandId.DISPON);
        Thread.Sleep(100);
      }

      public void Dispose()
      {
        this.spi.Dispose();
        this.control.Dispose();
        this.reset?.Dispose();
      }

      public void Enable() => this.SendCommand(ILI9341CommandId.DISPON);

      public void Disable() => this.SendCommand(ILI9341CommandId.DISPOFF);

      private void SendCommand(ILI9341CommandId command)
      {
        this.buffer1[0] = (byte) command;
        this.control.Write(PinValue.Low);
        this.spi.Write(this.buffer1);
      }

      private void SendData(byte data)
      {
        this.buffer1[0] = data;
        this.control.Write(PinValue.High);
        this.spi.Write(this.buffer1);
      }

      private void SendData(byte[] data)
      {
        this.control.Write(PinValue.High);
        this.spi.Write(data);
      }

      public void SetDrawWindow(int x, int y, int width, int height)
      {
        int num1 = x + width;
        int num2 = y + height;
        this.SendCommand(ILI9341CommandId.CASET);
        this.SendData((byte) (x >> 8));
        this.SendData((byte) x);
        this.SendData((byte) (num1 >> 8));
        this.SendData((byte) num1);
        this.SendCommand(ILI9341CommandId.PASET);
        this.SendData((byte) (y >> 8));
        this.SendData((byte) y);
        this.SendData((byte) (num2 >> 8));
        this.SendData((byte) num2);
        this.SendCommand(ILI9341CommandId.RAMWR);
        this.Width = width;
        this.Height = height;
      }

      public void SetDataAccessControl(
        bool swapRowColumn,
        bool invertRow,
        bool invertColumn,
        bool useBgrPanel)
      {
        byte data = 0;
        if (useBgrPanel)
          data |= (byte) 8;
        if (swapRowColumn)
          data |= (byte) 32;
        if (invertColumn)
          data |= (byte) 64;
        if (invertRow)
          data |= (byte) 128;
        this.SendCommand(ILI9341CommandId.MADCTL);
        this.SendData(data);
        this.rowColumnSwapped = swapRowColumn;
      }

      private void SendDrawCommand()
      {
        this.SendCommand(ILI9341CommandId.RAMWR);
        this.control.Write(PinValue.High);
      }

      public void DrawBuffer(byte[] buffer)
      {
        this.SendDrawCommand();
        if (this.bpp == 16)
          BitConverterHelper.SwapEndianness(buffer, 2);
        this.spi.Write(new SpanByte( buffer, 0, this.Height * this.Width * this.bpp / 8));
        if (this.bpp != 16)
          return;
        BitConverterHelper.SwapEndianness(buffer, 2);
      }

      public void DrawBuffer(byte[] buffer, int x, int y, int width, int height)
      {
        this.SetDrawWindow(x, y, width, height);
        this.DrawBuffer(buffer, x, y, width, height, this.MaxWidth, 1, 1);
      }

            public void DrawBuffer(
              byte[] buffer,
              int x,
              int y,
              int width,
              int height,
              int originalWidth,
              int columnMultiplier,
              int rowMultiplier)
            {
                if (this.bpp != 16)
                    throw new NotSupportedException();
                this.SendDrawCommand();
                BitConverterHelper.SwapEndianness(buffer, 2);
                var arr = new byte[buffer.Length + 7];
                for (var i = 0; i < buffer.Length; i++)
                {
                    arr[i] = buffer[i];
                }
                arr[buffer.Length + 0] = (byte)x;
                arr[buffer.Length + 1] = (byte)y;
                arr[buffer.Length + 2] = (byte)width;
                arr[buffer.Length + 3] = (byte)height;
                arr[buffer.Length + 4] = (byte)originalWidth;
                arr[buffer.Length + 5] = (byte)columnMultiplier;
                arr[buffer.Length + 6] = (byte)rowMultiplier;

                this.spi.Write(new SpanByte(arr));
                //this.spi.Write(buffer, x, y, width, height, originalWidth, columnMultiplier, rowMultiplier);
                BitConverterHelper.SwapEndianness(buffer, 2);
            }

      public void DrawBufferNative(byte[] buffer) => this.DrawBufferNative(buffer, 0, buffer.Length);

      public void DrawBufferNative(byte[] buffer, int offset, int count)
      {
        this.SendDrawCommand();
        this.spi.Write(new SpanByte( buffer, offset, count));
      }
    }
  }
}
