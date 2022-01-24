// Decompiled with JetBrains decompiler
// Type: nanoFramework.ST7789.ST7789Controller
// Assembly: nanoFramework.ST7789, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4EDF34B8-FD3A-42B4-A2F9-6903BB949B07
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFramework.ST7789.dll

//using GHIElectronics.TinyCLR.Devices.Gpio;
//using GHIElectronics.TinyCLR.Devices.Spi;
using nanoFramework.Tools;
using System;
using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;

namespace nanoFramework.ST7789
{
  public class ST7789Controller
  {
    private readonly byte[] buffer1 = new byte[1];
    private readonly byte[] buffer4 = new byte[4];
    private readonly SpiDevice spi;
    private readonly GpioPin control;
    private readonly GpioPin reset;
    private int bpp;
    private bool rowColumnSwapped;

    public DataFormat DataFormat { get; private set; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public int MaxWidth => 240;

    public int MaxHeight => 240;

        public static SpiConnectionSettings GetConnectionSettings(
          int BusId,
          int chipSelectLine, PinValue CsType)
        {
            var con = new SpiConnectionSettings(BusId, chipSelectLine);

            con.Mode = SpiMode.Mode3;
            con.ClockFrequency = 12000000;
            con.ChipSelectLineActiveState = CsType;
            return con;

        }

    public ST7789Controller(SpiDevice spi, GpioPin control)
      : this(spi, control, (GpioPin) null)
    {
    }

    public ST7789Controller(SpiDevice spi, GpioPin control, GpioPin reset)
    {
      this.spi = spi;
      this.control = control;
      this.control.SetPinMode(PinMode.Output);
      this.reset = reset;
      this.reset?.SetPinMode(PinMode.Output);
      this.Reset();
      this.Initialize();
      this.SetDataFormat(DataFormat.Rgb565);
      this.SetDataAccessControl(false, false, false, false);
      this.SetDrawWindow(0, 0, this.MaxWidth - 1, this.MaxHeight - 1);
    }

    private void Reset()
    {
      if (this.reset == null)
        return;
      this.reset.Write(PinValue.Low);
      Thread.Sleep(50);
      this.reset.Write(PinValue.High);
      Thread.Sleep(200);
    }

    private void Initialize()
    {
      this.SendCommand(ST7789CommandId.MADCTL);
      this.SendData((byte) 112);
      this.SendCommand(ST7789CommandId.COLMOD);
      this.SendData((byte) 5);
      this.SendCommand(ST7789CommandId.FRMCTR2);
      this.SendData((byte) 12);
      this.SendData((byte) 12);
      this.SendData((byte) 0);
      this.SendData((byte) 51);
      this.SendData((byte) 51);
      this.SendCommand(ST7789CommandId.FRMCTR3 | ST7789CommandId.RDDID);
      this.SendData((byte) 53);
      this.SendCommand((ST7789CommandId) 187);
      this.SendData((byte) 25);
      this.SendCommand(ST7789CommandId.PWCTR1);
      this.SendData((byte) 44);
      this.SendCommand(ST7789CommandId.PWCTR3);
      this.SendData((byte) 1);
      this.SendCommand(ST7789CommandId.PWCTR4);
      this.SendData((byte) 18);
      this.SendCommand(ST7789CommandId.PWCTR5);
      this.SendData((byte) 32);
      this.SendCommand(ST7789CommandId.PWCTR3 | ST7789CommandId.RDDID);
      this.SendData((byte) 15);
      this.SendCommand(ST7789CommandId.PWCTR1 | ST7789CommandId.SLPIN);
      this.SendData((byte) 164);
      this.SendData((byte) 161);
      this.SendCommand(ST7789CommandId.GAMCTRP1);
      this.SendData((byte) 208);
      this.SendData((byte) 4);
      this.SendData((byte) 13);
      this.SendData((byte) 17);
      this.SendData((byte) 19);
      this.SendData((byte) 43);
      this.SendData((byte) 63);
      this.SendData((byte) 84);
      this.SendData((byte) 76);
      this.SendData((byte) 24);
      this.SendData((byte) 13);
      this.SendData((byte) 11);
      this.SendData((byte) 31);
      this.SendData((byte) 35);
      this.SendCommand(ST7789CommandId.GAMCTRN1);
      this.SendData((byte) 208);
      this.SendData((byte) 4);
      this.SendData((byte) 12);
      this.SendData((byte) 17);
      this.SendData((byte) 19);
      this.SendData((byte) 44);
      this.SendData((byte) 63);
      this.SendData((byte) 68);
      this.SendData((byte) 81);
      this.SendData((byte) 47);
      this.SendData((byte) 31);
      this.SendData((byte) 31);
      this.SendData((byte) 32);
      this.SendData((byte) 35);
      this.SendCommand(ST7789CommandId.INVON);
      this.SendCommand(ST7789CommandId.SLPOUT);
      Thread.Sleep(120);
      this.SendCommand(ST7789CommandId.DISPON);
    }

    public void Dispose()
    {
      this.spi.Dispose();
      this.control.Dispose();
      this.reset?.Dispose();
    }

    public void Enable() => this.SendCommand(ST7789CommandId.DISPON);

    public void Disable() => this.SendCommand(ST7789CommandId.DISPOFF);

    private void SendCommand(ST7789CommandId command)
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
      this.SendCommand(ST7789CommandId.MADCTL);
      this.SendData(data);
      this.rowColumnSwapped = swapRowColumn;
    }

    public void SetDataFormat(DataFormat dataFormat)
    {
      if (dataFormat != DataFormat.Rgb565)
      {
        if (dataFormat != DataFormat.Rgb444)
          throw new NotSupportedException();
        this.bpp = 12;
        this.SendCommand(ST7789CommandId.COLMOD);
        this.SendData((byte) 3);
      }
      else
      {
        this.bpp = 16;
        this.SendCommand(ST7789CommandId.COLMOD);
        this.SendData((byte) 5);
      }
      this.DataFormat = dataFormat;
    }

    public void SetDrawWindow(int x, int y, int width, int height)
    {
      this.Width = width;
      this.Height = height;
      this.buffer4[1] = (byte) x;
      this.buffer4[3] = (byte) (x + width);
      this.SendCommand(ST7789CommandId.CASET);
      this.SendData(this.buffer4);
      this.buffer4[1] = (byte) y;
      this.buffer4[3] = (byte) (y + height);
      this.SendCommand(ST7789CommandId.RASET);
      this.SendData(this.buffer4);
    }

    private void SendDrawCommand()
    {
      this.SendCommand(ST7789CommandId.RAMWR);
      this.control.Write(PinValue.High);
    }

    public void DrawBuffer(byte[] buffer)
    {
      this.SendDrawCommand();
      if (this.bpp == 16)
        BitConverterHelper.SwapEndianness(buffer, 2);
      this.spi.Write(buffer);
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
            var arr = new byte[buffer.Length+7];
            for(var i = 0;i<buffer.Length;i++)
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

            //this.spi.Write(buffer, x, y, width, height, originalWidth, columnMultiplier, rowMultiplier );
            this.spi.Write(new SpanByte(arr));
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
