// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.Drivers.EastRising.ERC12864.ERC12864Controller
// Assembly: GHIElectronics.TinyCLR.Drivers.EastRising.ERC12864, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 53D284A6-A289-460A-BD63-63573AA663EC
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\GHIElectronics.TinyCLR.Drivers.EastRising.ERC12864.dll

//using GHIElectronics.TinyCLR.Devices.Gpio;
//using GHIElectronics.TinyCLR.Devices.Spi;
using System;
using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;

namespace nanoFramework.ERC12864
{
  public class ERC12864Controller
  {
    private readonly byte[] vram;
    private readonly SpiDevice spi;
    private readonly GpioPin control;
    private readonly GpioPin reset;

    public int Width => 128;

    public int Height => 64;

        public static SpiConnectionSettings GetConnectionSettings(
            int BusId,
          PinValue chipSelectType,
          int chipSelectLine)
        {
            var con = new SpiConnectionSettings(BusId, chipSelectLine);

            con.Mode = SpiMode.Mode0;
            con.ClockFrequency = 2000000;
            con.ChipSelectLineActiveState = chipSelectType;
            con.ChipSelectLine = chipSelectLine;
            return con;
        }

    public ERC12864Controller(SpiDevice spi, GpioPin control)
      : this(spi, control, (GpioPin) null, ERC12864Controller.Flip.None)
    {
    }

    public ERC12864Controller(SpiDevice spi, GpioPin control, GpioPin reset)
      : this(spi, control, reset, ERC12864Controller.Flip.None)
    {
    }

    public ERC12864Controller(
      SpiDevice spi,
      GpioPin ctrl,
      GpioPin reset,
      ERC12864Controller.Flip flip)
    {
      this.vram = new byte[this.Width * this.Height / 8];
      this.spi = spi;
      this.control = ctrl;
      this.reset = reset;
      this.control.SetPinMode(PinMode.Output);
      this.reset.SetPinMode(PinMode.Output);
      this.Reset();
      this.Initialize(flip);
    }

    private void Reset()
    {
      this.reset?.Write(PinValue.Low);
      Thread.Sleep(100);
      this.reset?.Write(PinValue.High);
      Thread.Sleep(100);
    }

    private void Initialize(ERC12864Controller.Flip flip)
    {
      switch (flip)
      {
        case ERC12864Controller.Flip.None:
          this.SendCommand((byte) 161);
          this.SendCommand((byte) 200);
          break;
        case ERC12864Controller.Flip.X:
          this.SendCommand((byte) 160);
          this.SendCommand((byte) 200);
          break;
        case ERC12864Controller.Flip.Y:
          this.SendCommand((byte) 161);
          this.SendCommand((byte) 192);
          break;
        case ERC12864Controller.Flip.XY:
          this.SendCommand((byte) 160);
          this.SendCommand((byte) 192);
          break;
      }
      this.SendCommand((byte) 162);
      this.PowerControl((byte) 7);
      this.RegulorResistorSelect((byte) 5);
      this.SetContrastControlRegister((byte) 30);
      this.InitialDisplayLine((byte) 0);
      this.DisplayOn();
    }

    private void DisplayOn() => this.SendCommand((byte) 175);

    private void PowerControl(byte volt) => this.SendCommand((byte) (40U | (uint) volt));

    private void RegulorResistorSelect(byte r) => this.SendCommand((byte) (32U | (uint) r));

    private void SetContrastControlRegister(byte mod)
    {
      this.SendCommand((byte) 129);
      this.SendCommand(mod);
    }

    private void InitialDisplayLine(byte line)
    {
      line |= (byte) 64;
      this.SendCommand(line);
    }

    private void SendCommand(byte command)
    {
      this.control.Write(PinValue.Low);
      this.spi.Write(new SpanByte( new byte[1]{ command }, 0, 1));
    }

    private void SendData(byte[] data, int offset, int count)
    {
      this.control.Write(PinValue.High);
      this.spi.Write(new SpanByte( data, offset, count));
    }

    private void SetPageAddress(int add)
    {
      add = 176 | add;
      this.SendCommand((byte) add);
    }

    private void SetColumnAddress(int add)
    {
      this.SendCommand((byte) (16 | add >> 4));
      this.SendCommand((byte) (15 & add | 4));
    }

    private void Flush()
    {
      int offset = 0;
      for (int add = 0; add < 8; ++add)
      {
        this.SetPageAddress(add);
        this.SetColumnAddress(0);
        this.SendData(this.vram, offset, this.Width);
        offset += this.Width;
      }
    }

    public void Enable() => this.SendCommand((byte) 175);

    public void Disable() => this.SendCommand((byte) 174);

    public void SetContrast(byte level)
    {
      level &= (byte) 63;
      this.SetContrastControlRegister(level);
    }

    public void Dispose()
    {
      this.spi.Dispose();
      this.control.Dispose();
      this.reset?.Dispose();
    }

    public void DrawBufferNative(byte[] buffer) => this.DrawBufferNative(buffer, 0, buffer.Length);

    public void DrawBufferNative(byte[] buffer, int offset, int count)
    {
      Array.Copy((Array) buffer, offset, (Array) this.vram, 0, count);
      this.Flush();
    }

    public enum Flip
    {
      None,
      X,
      Y,
      XY,
    }
  }
}
