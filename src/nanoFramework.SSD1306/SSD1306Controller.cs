// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.Drivers.SolomonSystech.SSD1306.SSD1306Controller
// Assembly: GHIElectronics.TinyCLR.Drivers.SolomonSystech.SSD1306, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FB675157-8F3A-4BE6-8D8B-E96028CF729A
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\GHIElectronics.TinyCLR.Drivers.SolomonSystech.SSD1306.dll

//using GHIElectronics.TinyCLR.Devices.I2c;
using System;
using System.Device.I2c;

namespace SolomonSystech.SSD1306
{
    public class SSD1306Controller
    {
        private readonly byte[] vram = new byte[1025];
        private readonly byte[] buffer2 = new byte[2];
        private readonly I2cDevice i2c;

        public int Width => 128;

        public int Height => 64;

        public static I2cConnectionSettings GetConnectionSettings() {
            var i2c = new I2cConnectionSettings(1, 60,I2cBusSpeed.FastMode);

            //i2c.AddressFormat = I2cAddressFormat.SevenBi;
            //i2c.BusSpeed = I2cBusSpeed.FastMode;
            return i2c;
        }

    public SSD1306Controller(I2cDevice i2c)
    {
      this.vram[0] = (byte) 64;
      this.i2c = i2c;
      this.Initialize();
    }

    private void Initialize()
    {
      this.SendCommand((byte) 174);
      this.SendCommand((byte) 0);
      this.SendCommand((byte) 16);
      this.SendCommand((byte) 64);
      this.SendCommand((byte) 129);
      this.SendCommand((byte) 207);
      this.SendCommand((byte) 161);
      this.SendCommand((byte) 166);
      this.SendCommand((byte) 168);
      this.SendCommand((byte) 63);
      this.SendCommand((byte) 211);
      this.SendCommand((byte) 0);
      this.SendCommand((byte) 213);
      this.SendCommand((byte) 128);
      this.SendCommand((byte) 217);
      this.SendCommand((byte) 241);
      this.SendCommand((byte) 218);
      this.SendCommand((byte) 18);
      this.SendCommand((byte) 219);
      this.SendCommand((byte) 64);
      this.SendCommand((byte) 141);
      this.SendCommand((byte) 20);
      this.SendCommand((byte) 175);
      this.SendCommand((byte) 200);
      this.SendCommand((byte) 32);
      this.SendCommand((byte) 0);
      this.SendCommand((byte) 33);
      this.SendCommand((byte) 0);
      this.SendCommand((byte) 127);
      this.SendCommand((byte) 34);
      this.SendCommand((byte) 0);
      this.SendCommand((byte) 7);
    }

    public void Dispose() => this.i2c.Dispose();

    private void SendCommand(byte cmd)
    {
      this.buffer2[1] = cmd;
      this.i2c.Write(this.buffer2);
    }

    public void SetColorFormat(bool invert) => this.SendCommand(invert ? (byte) 167 : (byte) 166);

    public void DrawBufferNative(byte[] buffer) => this.DrawBufferNative(buffer, 0, buffer.Length);

    public void DrawBufferNative(byte[] buffer, int offset, int count)
    {
      Array.Copy((Array) buffer, offset, (Array) this.vram, 1, count);
      this.i2c.Write(this.vram);
    }
  }
}
