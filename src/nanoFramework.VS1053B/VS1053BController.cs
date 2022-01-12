// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.Drivers.VlsiSolution.VS1053B.VS1053BController
// Assembly: GHIElectronics.TinyCLR.Drivers.VlsiSolution.VS1053B, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 886B73D0-A6B5-4155-9454-0DFB8B12E979
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\GHIElectronics.TinyCLR.Drivers.VlsiSolution.VS1053B.dll

//using GHIElectronics.TinyCLR.Devices.Gpio;
//using GHIElectronics.TinyCLR.Devices.Spi;
using System;
using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;

namespace VlsiSolution.VS1053B
{
    public sealed class VS1053BController
    {
        //private readonly SpiDevice spi;
        private readonly SpiDevice spiSetting;
        private readonly SpiDevice spiData;
        private readonly GpioPin dreq;
        private readonly GpioPin reset;
        private readonly SpiConnectionSettings dataSetting;
        private readonly SpiConnectionSettings cmdSetting;
        private const ushort SM_SDINEW = 2048;
        private const ushort SM_RESET = 4;
        private const int SCI_MODE = 0;
        private const int SCI_VOL = 11;
        private const int SCI_CLOCKF = 3;
        private byte[] block = new byte[32];
        private byte[] cmdBuffer = new byte[4];

        public VS1053BController(
          //SpiDevice spi,
          GpioPin dreq,
          GpioPin reset,
          GpioPin dataChipSelect,
          GpioPin commandChipSelect)
        {
            this.dataSetting = new SpiConnectionSettings(1);
            //this.dataSetting.c = SpiChipSelectType.Gpio;
            this.dataSetting.ChipSelectLine = dataChipSelect.PinNumber;
            this.dataSetting.ClockFrequency = 2000000;
            this.dataSetting.Mode = SpiMode.Mode0;
            this.dataSetting.ChipSelectLineActiveState = false;
            this.spiData = new SpiDevice(cmdSetting);
            
            this.cmdSetting = new SpiConnectionSettings(1);
            //this.cmdSetting.ChipSelectType = SpiChipSelectType.Gpio;
            this.cmdSetting.ChipSelectLine = commandChipSelect.PinNumber;
            this.cmdSetting.ClockFrequency = 2000000;
            this.cmdSetting.Mode = SpiMode.Mode0;
            this.cmdSetting.ChipSelectLineActiveState = false;
            this.spiSetting = new SpiDevice(cmdSetting);

            this.reset = reset;
            this.reset.SetPinMode(PinMode.Output);
            this.dreq = dreq;
            this.dreq.SetPinMode(PinMode.InputPullUp);
            //this.spi = spi;
            this.Reset();
            this.CommandWrite((byte)0, (ushort)2048);
            this.CommandWrite((byte)3, (ushort)38912);
            this.CommandWrite((byte)11, (ushort)257);
            if (this.CommandRead((byte)11) != (ushort)257)
                throw new Exception("Failed to initialize MP3 Decoder.");
        }

        private void Reset()
        {
            this.reset.Write(PinValue.Low);
            Thread.Sleep(1);
            this.reset.Write(PinValue.High);
            Thread.Sleep(100);
        }

        private void CommandWrite(byte address, ushort data)
        {
            while (this.dreq.Read() == PinValue.Low)
                Thread.Sleep(1);
            SpiDevice device = this.spiSetting; //this.spi.GetDevice(this.cmdSetting);
            this.cmdBuffer[0] = (byte)2;
            this.cmdBuffer[1] = address;
            this.cmdBuffer[2] = (byte)((uint)data >> 8);
            this.cmdBuffer[3] = (byte)data;
            byte[] cmdBuffer = this.cmdBuffer;
            device.Write(cmdBuffer);
        }

        private ushort CommandRead(byte address)
        {
            while (this.dreq.Read() == PinValue.Low)
                Thread.Sleep(1);
            SpiDevice device = this.spiSetting; //this.spi.GetDevice(this.cmdSetting);
            this.cmdBuffer[0] = (byte)3;
            this.cmdBuffer[1] = address;
            this.cmdBuffer[2] = (byte)0;
            this.cmdBuffer[3] = (byte)0;
            byte[] numArray = new byte[4];
            byte[] cmdBuffer = this.cmdBuffer;
            int length1 = this.cmdBuffer.Length;
            byte[] readBuffer = numArray;
            int length2 = numArray.Length;
            device.TransferFullDuplex(new SpanByte(cmdBuffer, 0, length1), new SpanByte( readBuffer, 0, length2));
            return (ushort)((uint)(ushort)((uint)numArray[2] << 8) + (uint)numArray[3]);
        }

        public void SetVolume(byte left_channel, byte right_channel) => this.CommandWrite((byte)11, (ushort)((int)byte.MaxValue - (int)left_channel << 8 | (int)byte.MaxValue - (int)right_channel));

        public void SendData(byte[] data)
        {
            int num = data.Length - data.Length % 32;
            SpiDevice device = this.spiData;//this.spi.GetDevice(this.dataSetting);
            for (int sourceIndex = 0; sourceIndex < num; sourceIndex += 32)
            {
                while (this.dreq.Read() == PinValue.Low)
                    Thread.Sleep(1);
                Array.Copy((Array)data, sourceIndex, (Array)this.block, 0, 32);
                device.Write(this.block);
            }
        }
    }
}
