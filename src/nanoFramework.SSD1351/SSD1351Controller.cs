// Decompiled with JetBrains decompiler
// Type: nanoFrameworkSSD1351.SSD1351Controller
// Assembly: nanoFrameworkSSD1351, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C46C74DA-2DAB-4500-BB44-2EC6E1E34CF2
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFrameworkSSD1351.dll

//using GHIElectronics.TinyCLR.Devices.Gpio;
//using GHIElectronics.TinyCLR.Devices.Spi;
using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;

namespace nanoFrameworkSSD1351
{
  public class SSD1351Controller
  {
    private readonly byte[] buffer1 = new byte[1];
    private readonly byte[] buffer2 = new byte[2];
    private readonly SpiDevice spi;
    private readonly GpioPin control;
    private readonly GpioPin reset;
    private bool rowColumnSwapped;

    public int Width { get; private set; }

    public int Height { get; private set; }

    public int MaxWidth => !this.rowColumnSwapped ? 128 : 96;

    public int MaxHeight => !this.rowColumnSwapped ? 96 : 128;

    public static SpiConnectionSettings GetConnectionSettings(int BusId,
      PinValue chipSelectType,
      int chipSelectLine)
    {
            var con = new SpiConnectionSettings(BusId,chipSelectLine);

            con.Mode = SpiMode.Mode3;
            con.ClockFrequency = 8000000;
            con.ChipSelectLineActiveState = chipSelectType;
            con.ChipSelectLine = chipSelectLine;
            return con;
      
    }

    public SSD1351Controller(SpiDevice spi, GpioPin control)
      : this(spi, control, (GpioPin) null)
    {
    }

    public SSD1351Controller(SpiDevice spi, GpioPin control, GpioPin reset)
    {
      this.spi = spi;
      this.control = control;
      this.control.SetPinMode(PinMode.Output);
      this.reset = reset;
      this.reset?.SetPinMode(PinMode.Output);
      this.Reset();
      this.Initialize();
      this.SetDataAccessControl(false, true, true, false);
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
      this.SendCommand(SSD1351CommandId.COMMANDLOCK);
      this.SendData((byte) 18);
      this.SendCommand(SSD1351CommandId.COMMANDLOCK);
      this.SendData((byte) 177);
      this.SendCommand(SSD1351CommandId.DISPLAYOFF);
      this.SendCommand(SSD1351CommandId.MUXRATIO);
      this.SendData((byte) 127);
      this.SendCommand(SSD1351CommandId.SETREMAP);
      this.SendData((byte) 116);
      this.SendCommand(SSD1351CommandId.SETCOLUMN);
      this.SendData((byte) 0);
      this.SendData((byte) 127);
      this.SendCommand(SSD1351CommandId.SETROW);
      this.SendData((byte) 0);
      this.SendData((byte) 127);
      this.SendCommand(SSD1351CommandId.STARTLINE);
      this.SendData((byte) 0);
      this.SendCommand(SSD1351CommandId.DISPLAYOFFSET);
      this.SendData((byte) 0);
      this.SendCommand(SSD1351CommandId.SETGPIO);
      this.SendData((byte) 0);
      this.SendCommand(SSD1351CommandId.FUNCTIONSELECT);
      this.SendData((byte) 1);
      this.SendCommand(SSD1351CommandId.NORMALDISPLAY);
      this.SendCommand(SSD1351CommandId.CONTRASTABC);
      this.SendData((byte) 200);
      this.SendData((byte) 128);
      this.SendData((byte) 200);
      this.SendCommand(SSD1351CommandId.CONTRASTMASTER);
      this.SendData((byte) 15);
      this.SendCommand(SSD1351CommandId.SETVSL);
      this.SendData((byte) 160);
      this.SendData((byte) 181);
      this.SendData((byte) 85);
      this.SendCommand(SSD1351CommandId.PRECHARGE2);
      this.SendData((byte) 1);
      this.SendCommand(SSD1351CommandId.DISPLAYON);
    }

    public void Dispose()
    {
      this.spi.Dispose();
      this.control.Dispose();
      this.reset?.Dispose();
    }

    private void SendCommand(SSD1351CommandId command)
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
      byte data = 100;
      if (useBgrPanel)
        data = (byte) 96;
      if (swapRowColumn)
        data |= (byte) 1;
      if (invertColumn)
        data |= (byte) 2;
      if (invertRow)
        data |= (byte) 16;
      this.SendCommand(SSD1351CommandId.SETREMAP);
      this.SendData(data);
      this.SendCommand(SSD1351CommandId.STARTLINE);
      this.SendData(invertRow ? (byte) this.Height : (byte) 0);
      this.rowColumnSwapped = swapRowColumn;
    }

    private void SetDrawWindow(int x, int y, int width, int height)
    {
      this.Width = width;
      this.Height = height;
      int num1 = x + width;
      int num2 = y + height;
      if (this.rowColumnSwapped)
      {
        int num3 = x;
        x = y;
        y = num3;
        int num4 = num1;
        num1 = num2;
        num2 = num4;
      }
      this.buffer2[0] = (byte) x;
      this.buffer2[1] = (byte) num1;
      this.SendCommand(SSD1351CommandId.SETCOLUMN);
      this.SendData(this.buffer2);
      this.buffer2[0] = (byte) y;
      this.buffer2[1] = (byte) num2;
      this.SendCommand(SSD1351CommandId.SETROW);
      this.SendData(this.buffer2);
      this.SendCommand(SSD1351CommandId.WRITERAM);
    }

    private void SendDrawCommand() => this.control.Write(PinValue.High);

    public void DrawBuffer(byte[] buffer) => this.DrawBufferNative(buffer, 0, buffer.Length);

    public void DrawBufferNative(byte[] buffer) => this.DrawBufferNative(buffer, 0, buffer.Length);

    public void DrawBufferNative(byte[] buffer, int offset, int count)
    {
      this.SendDrawCommand();
      this.spi.Write(new System.SpanByte( buffer, offset, count));
    }
  }
}
