// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.Drivers.Infrared.NecIRDecoder
// Assembly: GHIElectronics.TinyCLR.Drivers.Infrared, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F6B02D28-5B55-4DDA-90EE-10C8AFDECCD1
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\GHIElectronics.TinyCLR.Drivers.Infrared.dll

//using GHIElectronics.TinyCLR.Devices.Gpio;
using System;
using System.Device.Gpio;

namespace nanoFramework.Infrared
{
    public class NecIRDecoder
    {
        private NecIRDecoder.Status status;
        private const int BurstPreTime = 9000;
        private const int SpacePreTime = 4500;
        private const int BurstBitTime = 562;
        private const int ZeroBitTime = 562;
        private const int OneBitTime = 1688;
        private const int RepeatTime = 2250;
        private const int RepeatEndTime = 562;
        private long lastTick;
        private uint necMessage;
        private int bitIndex;
        private readonly GpioPin receivePin;

        public int ErrorCounter { get; set; }

        public event NecIRDecoder.RepeatEventHandler OnRepeatEvent;

        public event NecIRDecoder.DataReceivedEventHandler OnDataReceivedEvent;

        public NecIRDecoder(GpioPin receivePin)
        {
            this.receivePin = receivePin;
            this.lastTick = DateTime.UtcNow.Ticks;
            this.receivePin.SetPinMode(PinMode.Input);
            //this.receivePin.ValueChangedEdge = GpioPinEdge.FallingEdge | GpioPinEdge.RisingEdge;
            this.receivePin.DebounceTimeout = TimeSpan.FromMilliseconds(0);
            this.receivePin.ValueChanged += Rx_ValueChanged;
            this.status = NecIRDecoder.Status.PreBurst;
        }


        private bool InRange(int value, int expected)
        {
            double num1 = 0.8;
            double num2 = 1.2;
            return (double)value > (double)expected * num1 && (double)value < (double)expected * num2;
        }

        private void Rx_ValueChanged(object sender, PinValueChangedEventArgs e)
        {
            int num1 = (int)((DateTime.UtcNow.Ticks - this.lastTick) / 10L);
            this.lastTick = DateTime.UtcNow.Ticks;
            switch (this.status)
            {
                case NecIRDecoder.Status.PreBurst:
                    if (!this.InRange(num1, 9000))
                        break;
                    this.status = NecIRDecoder.Status.PostPreBurst;
                    this.necMessage = 0U;
                    this.bitIndex = 0;
                    break;
                case NecIRDecoder.Status.PostPreBurst:
                    if (this.InRange(num1, 4500))
                    {
                        this.status = NecIRDecoder.Status.BurstBit;
                        break;
                    }
                    if (this.InRange(num1, 2250))
                    {
                        this.status = NecIRDecoder.Status.RepeatEnd;
                        break;
                    }
                    ++this.ErrorCounter;
                    this.status = NecIRDecoder.Status.PreBurst;
                    break;
                case NecIRDecoder.Status.RepeatEnd:
                    if (this.InRange(num1, 562))
                    {
                        NecIRDecoder.RepeatEventHandler onRepeatEvent = this.OnRepeatEvent;
                        if (onRepeatEvent == null)
                            break;
                        onRepeatEvent();
                        break;
                    }
                    ++this.ErrorCounter;
                    this.status = NecIRDecoder.Status.PreBurst;
                    break;
                case NecIRDecoder.Status.BurstBit:
                    if (this.InRange(num1, 562))
                    {
                        this.status = NecIRDecoder.Status.DataBit;
                        break;
                    }
                    ++this.ErrorCounter;
                    this.status = NecIRDecoder.Status.PreBurst;
                    break;
                case NecIRDecoder.Status.DataBit:
                    this.status = NecIRDecoder.Status.BurstBit;
                    if (this.InRange(num1, 562))
                        ++this.bitIndex;
                    else if (this.InRange(num1, 1688))
                    {
                        this.necMessage |= (uint)(1 << this.bitIndex);
                        ++this.bitIndex;
                    }
                    else
                    {
                        ++this.ErrorCounter;
                        this.status = NecIRDecoder.Status.PreBurst;
                    }
                    if (this.bitIndex != 32)
                        break;
                    byte necMessage = (byte)this.necMessage;
                    byte num2 = (byte)~(this.necMessage >> 8);
                    byte command = (byte)(this.necMessage >> 16);
                    byte num3 = (byte)~(this.necMessage >> 24);
                    if ((int)necMessage == (int)num2 && (int)command == (int)num3)
                    {
                        NecIRDecoder.DataReceivedEventHandler dataReceivedEvent = this.OnDataReceivedEvent;
                        if (dataReceivedEvent != null)
                            dataReceivedEvent(necMessage, command);
                    }
                    else
                        ++this.ErrorCounter;
                    this.status = NecIRDecoder.Status.PreBurst;
                    break;
                default:
                    this.status = NecIRDecoder.Status.PreBurst;
                    break;
            }
        }

        private enum Status
        {
            PreBurst,
            PostPreBurst,
            RepeatEnd,
            BurstBit,
            DataBit,
        }

        public delegate void RepeatEventHandler();

        public delegate void DataReceivedEventHandler(byte address, byte command);
    }
}
