// Decompiled with JetBrains decompiler
// Type: Sitronix.ST7735.ST7735Controller
// Assembly: Sitronix.ST7735, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9398E5D-1F88-456A-976E-84F8D4504012
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\Sitronix.ST7735.dll

//using GHIElectronics.TinyCLR.Devices.Gpio;
//using GHIElectronics.TinyCLR.Devices.Spi;
using nanoFramework.Tools;
using System;
using System.Collections;
using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;

namespace Sitronix.ST7735
{
    public class ST7735Controller
    {
        private readonly byte[] buffer1 = new byte[1];
        private readonly byte[] buffer4 = new byte[4];
        private readonly SpiDevice spi;
        private readonly GpioPin control;
        private readonly GpioPin reset;
        private readonly ScreenSize screenSize;
        private int bpp;
        private bool rowColumnSwapped;

        public DataFormat DataFormat { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int MaxWidth
        {
            get
            {
                if (!this.rowColumnSwapped)
                    return this.screenSize != ScreenSize._160x128 && this.screenSize == ScreenSize._132x130 ? 130 : 128;
                if (this.screenSize == ScreenSize._160x128)
                    return 160;
                return this.screenSize != ScreenSize._132x130 ? 128 : 132;
            }
        }

        public int MaxHeight
        {
            get
            {
                if (!this.rowColumnSwapped)
                {
                    if (this.screenSize == ScreenSize._160x128)
                        return 160;
                    return this.screenSize != ScreenSize._132x130 ? 128 : 132;
                }
                return this.screenSize != ScreenSize._160x128 && this.screenSize == ScreenSize._132x130 ? 130 : 128;
            }
        }

        public static SpiConnectionSettings GetConnectionSettings(
          PinValue chipSelectType,
          int chipSelectLine)
        {
            var con = new SpiConnectionSettings(1, chipSelectLine);

            con.Mode = SpiMode.Mode3;// Mode3;
            con.ClockFrequency = 12000000;//12000000
            con.ChipSelectLineActiveState = chipSelectType;
            //ChipSelectLine = chipSelectLine
            return con;

        }

        public ST7735Controller(SpiDevice spi, GpioPin control)
          : this(spi, control, (GpioPin)null)
        {
        }

        public ST7735Controller(SpiDevice spi, GpioPin control, GpioPin reset)
          : this(spi, control, reset, ScreenSize._160x128)
        {
        }

        public ST7735Controller(SpiDevice spi, GpioPin control, GpioPin reset, ScreenSize screenSize)
        {
            this.spi = spi;
            this.control = control;
            this.control.SetPinMode(PinMode.Output);
            this.reset = reset;
            this.reset?.SetPinMode(PinMode.Output);
            this.screenSize = screenSize;
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
            this.SendCommand(ST7735CommandId.SWRESET);
            Thread.Sleep(120);
            this.SendCommand(ST7735CommandId.SLPOUT);
            Thread.Sleep(120);
            this.SendCommand(ST7735CommandId.FRMCTR1);
            this.SendData((byte)1);
            this.SendData((byte)44);
            this.SendData((byte)45);
            this.SendCommand(ST7735CommandId.FRMCTR2);
            this.SendData((byte)1);
            this.SendData((byte)44);
            this.SendData((byte)45);
            this.SendCommand(ST7735CommandId.FRMCTR3);
            this.SendData((byte)1);
            this.SendData((byte)44);
            this.SendData((byte)45);
            this.SendData((byte)1);
            this.SendData((byte)44);
            this.SendData((byte)45);
            this.SendCommand(ST7735CommandId.INVCTR);
            this.SendData((byte)7);
            this.SendCommand(ST7735CommandId.PWCTR1);
            this.SendData((byte)162);
            this.SendData((byte)2);
            this.SendData((byte)132);
            this.SendCommand(ST7735CommandId.PWCTR2);
            this.SendData((byte)197);
            this.SendCommand(ST7735CommandId.PWCTR3);
            this.SendData((byte)10);
            this.SendData((byte)0);
            this.SendCommand(ST7735CommandId.PWCTR4);
            this.SendData((byte)138);
            this.SendData((byte)42);
            this.SendCommand(ST7735CommandId.PWCTR5);
            this.SendData((byte)138);
            this.SendData((byte)238);
            this.SendCommand(ST7735CommandId.VMCTR1);
            this.SendData((byte)14);
            this.SendCommand(ST7735CommandId.GAMCTRP1);
            this.SendData((byte)15);
            this.SendData((byte)26);
            this.SendData((byte)15);
            this.SendData((byte)24);
            this.SendData((byte)47);
            this.SendData((byte)40);
            this.SendData((byte)32);
            this.SendData((byte)34);
            this.SendData((byte)31);
            this.SendData((byte)27);
            this.SendData((byte)35);
            this.SendData((byte)55);
            this.SendData((byte)0);
            this.SendData((byte)7);
            this.SendData((byte)2);
            this.SendData((byte)16);
            this.SendCommand(ST7735CommandId.GAMCTRN1);
            this.SendData((byte)15);
            this.SendData((byte)27);
            this.SendData((byte)15);
            this.SendData((byte)23);
            this.SendData((byte)51);
            this.SendData((byte)44);
            this.SendData((byte)41);
            this.SendData((byte)46);
            this.SendData((byte)48);
            this.SendData((byte)48);
            this.SendData((byte)57);
            this.SendData((byte)63);
            this.SendData((byte)0);
            this.SendData((byte)7);
            this.SendData((byte)3);
            this.SendData((byte)16);
        }

        public void Dispose()
        {
            this.spi.Dispose();
            this.control.Dispose();
            this.reset?.Dispose();
        }

        public void Enable() => this.SendCommand(ST7735CommandId.DISPON);

        public void Disable() => this.SendCommand(ST7735CommandId.DISPOFF);

        private void SendCommand(ST7735CommandId command)
        {
            this.buffer1[0] = (byte)command;
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
                data |= (byte)8;
            if (swapRowColumn)
                data |= (byte)32;
            if (invertColumn)
                data |= (byte)64;
            if (invertRow)
                data |= (byte)128;
            this.SendCommand(ST7735CommandId.MADCTL);
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
                this.SendCommand(ST7735CommandId.COLMOD);
                this.SendData((byte)3);
            }
            else
            {
                this.bpp = 16;
                this.SendCommand(ST7735CommandId.COLMOD);
                this.SendData((byte)5);
            }
            this.DataFormat = dataFormat;
        }

        public void SetDrawWindow(int x, int y, int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.buffer4[1] = (byte)x;
            this.buffer4[3] = (byte)(x + width);
            this.SendCommand(ST7735CommandId.CASET);
            this.SendData(this.buffer4);
            this.buffer4[1] = (byte)y;
            this.buffer4[3] = (byte)(y + height);
            this.SendCommand(ST7735CommandId.RASET);
            this.SendData(this.buffer4);
        }

        private void SendDrawCommand()
        {
            this.SendCommand(ST7735CommandId.RAMWR);
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
            var arr = new byte[buffer.Length + 7];
            for (var i = 0; i < buffer.Length; i++) arr[i] = buffer[i];
            arr[buffer.Length + 0] = (byte)x;
            arr[buffer.Length + 1] = (byte)y;
            arr[buffer.Length + 2] = (byte)width;
            arr[buffer.Length + 3] = (byte)height;
            arr[buffer.Length + 4] = (byte)originalWidth;
            arr[buffer.Length + 5] = (byte)columnMultiplier;
            arr[buffer.Length + 6] = (byte)rowMultiplier;
            //this.spi.Write(buffer, x, y, width, height, originalWidth, columnMultiplier, rowMultiplier);
            this.spi.Write(arr);
            BitConverterHelper.SwapEndianness(buffer, 2);
        }

        public void DrawBufferNative(byte[] buffer) => this.DrawBufferNative(buffer, 0, buffer.Length);

        public void DrawBufferNative(byte[] buffer, int offset, int count)
        {
            this.SendDrawCommand();
            this.spi.Write(buffer);//, offset, count);
        }
    }
    

}
