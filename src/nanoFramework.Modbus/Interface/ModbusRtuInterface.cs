// Decompiled with JetBrains decompiler
// Type: nanoFramework.Modbus.Interface.ModbusRtuInterface
// Assembly: nanoFramework.Modbus, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B923EC90-60E3-4FDC-BF8E-BE964652F39A
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFramework.Modbus.dll


using System.IO.Ports;
using System;
using System.Threading;

namespace nanoFramework.Modbus.Interface
{
  public class ModbusRtuInterface : IModbusInterface
  {
    private readonly SerialPort serial;
    private readonly long halfCharLength;
    private long nextSend;
    private int baudRate;
    private int dataBits;
    private StopBits stopBits;
    private Parity parity;
    public bool isOpen = true;

    public SerialPort SerialPort => this.serial;

    public ModbusRtuInterface(
      SerialPort serial,
      int baudRate,
      int dataBits,
      StopBits stopBits,
      Parity parity,
      short maxDataLength = 252)
    {
      this.dataBits = dataBits;
      this.baudRate = baudRate;
      this.stopBits = stopBits;
      this.parity = parity;
      if (this.dataBits < 8)
        throw new ArgumentException("serial.DataBits must be >= 8");
      this.serial = serial;
      this.MaxDataLength = maxDataLength;
      this.MaxTelegramLength = (short) ((int) maxDataLength + 4);
      if (this.baudRate > 19200)
      {
        this.halfCharLength = 500L;
      }
      else
      {
        short dataBits1 = (short) this.dataBits;
        switch (this.stopBits)
        {
          case StopBits.One:
            ++dataBits1;
            break;
          case StopBits.OnePointFive:
          case StopBits.Two:
            dataBits1 += (short) 2;
            break;
        }
        if (this.parity != Parity.None)
          ++dataBits1;
        this.halfCharLength = (long) ((int) (short) ((int) dataBits1 * 1000 * 10000 / this.baudRate) >> 1);
      }
    }

    public short MaxDataLength { get; private set; }

    public short MaxTelegramLength { get; private set; }

    public void CreateTelegram(
      byte addr,
      byte fkt,
      short dataLength,
      byte[] buffer,
      out short telegramLength,
      out short dataPos,
      bool isResponse,
      ref object telegramContext)
    {
      telegramLength = (short) (4 + (int) dataLength);
      dataPos = (short) 2;
      buffer[0] = addr;
      buffer[1] = fkt;
    }

    public void PrepareWrite()
    {
    }

    public void PrepareRead()
    {
    }

    public void SendTelegram(byte[] buffer, short telegramLength)
    {
      ushort num1 = ModbusUtils.CalcCrc(buffer, (int) telegramLength - 2);
      buffer[(int) telegramLength - 2] = (byte) ((uint) num1 & (uint) byte.MaxValue);
      buffer[(int) telegramLength - 1] = (byte) (((int) num1 & 65280) >> 8);
      long num2 = this.nextSend - DateTime.UtcNow.Ticks;
      if (num2 > 0L)
        Thread.Sleep(Math.Max(1, (int) num2 / 10000));
      //this.serial.ClearReadBuffer();
      //this.serial.ClearWriteBuffer();
      this.nextSend = DateTime.UtcNow.Ticks + (long) ((int) telegramLength * 2 + 7) * this.halfCharLength;
      this.serial.Write(buffer, 0, (int) telegramLength);
    }

    public bool ReceiveTelegram(
      byte[] buffer,
      short desiredDataLength,
      int timeout,
      out short telegramLength)
    {
      short num1;
      if (desiredDataLength >= (short) 0)
      {
        num1 = (short) ((int) desiredDataLength + 4);
        if ((int) num1 > buffer.Length)
          throw new ArgumentException("buffer size (" + (object) buffer.Length + ") must be at least 4 byte larger than desiredDataLength (" + (object) desiredDataLength + ")");
      }
      else
        num1 = (short) -1;
      int offset = 0;
      DateTime dateTime = DateTime.UtcNow.AddMilliseconds((double) timeout);
      long num2 = 0;
      bool flag = false;
      while (!(DateTime.UtcNow > dateTime))
      {
        DateTime now;
        if (this.serial.BytesToRead > 0)
        {
          if (num1 > (short) 0)
            offset += this.serial.Read(buffer, offset, (int) num1 - offset);
          else
            offset += this.serial.Read(buffer, offset, buffer.Length - offset);
          now = DateTime.UtcNow;
          num2 = now.Ticks + 6L * this.halfCharLength;
        }
        if (!flag && offset >= 2)
        {
          flag = true;
          if (((int) buffer[1] & 128) != 0)
            num1 = (short) 5;
        }
        if (num1 > (short) 0 && offset == (int) num1)
        {
          telegramLength = (short) offset;
          return true;
        }
        if (num1 <= (short) 0 && offset >= 2)
        {
          now = DateTime.UtcNow;
          if (now.Ticks > num2 && this.serial.BytesToRead == 0)
          {
            ushort num3 = ModbusUtils.CalcCrc(buffer, offset - 2);
            if ((int) buffer[offset - 2] != (int) (byte) ((uint) num3 & (uint) byte.MaxValue) || (int) buffer[offset - 1] != (int) (byte) (((int) num3 & 65280) >> 8))
            {
              Thread.Sleep(1);
              now = DateTime.UtcNow;
              num2 = now.Ticks + 6L * this.halfCharLength;
            }
            else
            {
              telegramLength = (short) offset;
              return true;
            }
          }
        }
      }
      telegramLength = (short) 0;
      return false;
    }

    public bool ParseTelegram(
      byte[] buffer,
      short telegramLength,
      bool isResponse,
      ref object telegramContext,
      out byte address,
      out byte fkt,
      out short dataPos,
      out short dataLength)
    {
      ushort num = telegramLength >= (short) 4 ? ModbusUtils.CalcCrc(buffer, (int) telegramLength - 2) : throw new ModbusException(ModbusErrorCode.ResponseTooShort);
      if ((int) buffer[(int) telegramLength - 2] != (int) (byte) ((uint) num & (uint) byte.MaxValue) || (int) buffer[(int) telegramLength - 1] != (int) (byte) (((int) num & 65280) >> 8))
        throw new ModbusException(ModbusErrorCode.CrcError);
      address = buffer[0];
      fkt = buffer[1];
      dataPos = (short) 2;
      dataLength = (short) ((int) telegramLength - 4);
      return true;
    }

    public bool IsDataAvailable => this.serial.BytesToRead > 0;

        public void ClearInputBuffer()
        {
            //do nothing

            //this.serial.ClearReadBuffer();
        }

    public bool IsConnectionOk => this.serial != null && this.isOpen;
  }
}
