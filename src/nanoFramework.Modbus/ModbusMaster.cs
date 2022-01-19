// Decompiled with JetBrains decompiler
// Type: nanoFramework.Modbus.ModbusMaster
// Assembly: nanoFramework.Modbus, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B923EC90-60E3-4FDC-BF8E-BE964652F39A
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFramework.Modbus.dll

using nanoFramework.Modbus.Interface;
using System;
using System.Collections;

namespace nanoFramework.Modbus
{
  public class ModbusMaster
  {
    private readonly IModbusInterface @interface;
    private readonly object syncObject;
    private readonly byte[] buffer;

    public ModbusMaster(IModbusInterface intf, object syncObject = null)
    {
      this.@interface = intf;
      this.syncObject = syncObject ?? new object();
      this.buffer = new byte[(int) this.@interface.MaxTelegramLength];
      this.@interface.PrepareWrite();
    }

    protected virtual short SendReceive(
      byte deviceAddress,
      ModbusFunctionCode fc,
      int timeout,
      short telegramLength,
      short desiredDataLength,
      object telegramContext,
      ref short dataPos)
    {
      lock (this.syncObject)
      {
        try
        {
          this.@interface.SendTelegram(this.buffer, telegramLength);
          this.@interface.PrepareRead();
          if (deviceAddress == (byte) 0)
            return 0;
          byte fkt = 0;
          short dataLength = 0;
          while (timeout > 0)
          {
            DateTime now = DateTime.UtcNow;
            long ticks = now.Ticks;
            if (!this.@interface.ReceiveTelegram(this.buffer, desiredDataLength, timeout, out telegramLength))
              throw new ModbusException(ModbusErrorCode.Timeout);
            int num1 = timeout;
            now = DateTime.UtcNow;
            int num2 = (int) ((now.Ticks - ticks) / 10000L);
            timeout = num1 - num2;
            byte address;
            if (!this.@interface.ParseTelegram(this.buffer, telegramLength, true, ref telegramContext, out address, out fkt, out dataPos, out dataLength) || (int) address != (int) deviceAddress || (ModbusFunctionCode) ((int) fkt & (int) sbyte.MaxValue) != fc)
            {
              if (timeout <= 0)
                throw new ModbusException(ModbusErrorCode.Timeout);
            }
            else
              break;
          }
          if (((int) fkt & 128) != 0)
            throw new ModbusException((ModbusErrorCode) this.buffer[(int) dataPos]);
          return dataLength;
        }
        finally
        {
          this.@interface.PrepareWrite();
        }
      }
    }

    public virtual byte[] ReadCoils(
      byte deviceAddress,
      ushort startAddress,
      ushort coilCount,
      int timeout = 2000)
    {
      object telegramContext = (object) null;
      short telegramLength;
      short dataPos;
      this.@interface.CreateTelegram(deviceAddress, (byte) 1, (short) 4, this.buffer, out telegramLength, out dataPos, false, ref telegramContext);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos, startAddress);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos + 2, coilCount);
      short desiredDataLength = (short) (1 + (int) coilCount / 8);
      if ((int) coilCount % 8 != 0)
        ++desiredDataLength;
      short num = this.SendReceive(deviceAddress, ModbusFunctionCode.ReadCoils, timeout, telegramLength, desiredDataLength, telegramContext, ref dataPos);
      if (deviceAddress == (byte) 0)
        return (byte[]) null;
      byte[] numArray = new byte[(int) num - 1];
      Array.Copy((Array) this.buffer, (int) dataPos + 1, (Array) numArray, 0, (int) num - 1);
      return numArray;
    }

    public virtual byte[] ReadDiscreteInputs(
      byte deviceAddress,
      ushort startAddress,
      ushort inputCount,
      int timeout = 2000)
    {
      object telegramContext = (object) null;
      short telegramLength;
      short dataPos;
      this.@interface.CreateTelegram(deviceAddress, (byte) 2, (short) 4, this.buffer, out telegramLength, out dataPos, false, ref telegramContext);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos, startAddress);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos + 2, inputCount);
      short desiredDataLength = (short) (1 + (int) inputCount / 8);
      if ((int) inputCount % 8 != 0)
        ++desiredDataLength;
      short num = this.SendReceive(deviceAddress, ModbusFunctionCode.ReadDiscreteInputs, timeout, telegramLength, desiredDataLength, telegramContext, ref dataPos);
      if (deviceAddress == (byte) 0)
        return (byte[]) null;
      byte[] numArray = new byte[(int) num - 1];
      Array.Copy((Array) this.buffer, (int) dataPos + 1, (Array) numArray, 0, (int) num - 1);
      return numArray;
    }

    public virtual ushort[] ReadHoldingRegisters(
      byte deviceAddress,
      ushort startAddress,
      ushort registerCount,
      int timeout = 2000)
    {
      object telegramContext = (object) null;
      short telegramLength;
      short dataPos;
      this.@interface.CreateTelegram(deviceAddress, (byte) 3, (short) 4, this.buffer, out telegramLength, out dataPos, false, ref telegramContext);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos, startAddress);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos + 2, registerCount);
      short desiredDataLength = (short) (1 + 2 * (int) registerCount);
      short num = this.SendReceive(deviceAddress, ModbusFunctionCode.ReadHoldingRegisters, timeout, telegramLength, desiredDataLength, telegramContext, ref dataPos);
      if (deviceAddress == (byte) 0)
        return (ushort[]) null;
      ushort[] numArray = new ushort[((int) num - 1) / 2];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = ModbusUtils.ExtractUShort(this.buffer, (int) dataPos + 1 + 2 * index);
      return numArray;
    }

    public virtual ushort[] ReadInputRegisters(
      byte deviceAddress,
      ushort startAddress,
      ushort registerCount,
      int timeout = 2000)
    {
      object telegramContext = (object) null;
      short telegramLength;
      short dataPos;
      this.@interface.CreateTelegram(deviceAddress, (byte) 4, (short) 4, this.buffer, out telegramLength, out dataPos, false, ref telegramContext);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos, startAddress);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos + 2, registerCount);
      short desiredDataLength = (short) (1 + 2 * (int) registerCount);
      short num = this.SendReceive(deviceAddress, ModbusFunctionCode.ReadInputRegisters, timeout, telegramLength, desiredDataLength, telegramContext, ref dataPos);
      if (deviceAddress == (byte) 0)
        return (ushort[]) null;
      ushort[] numArray = new ushort[((int) num - 1) / 2];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = ModbusUtils.ExtractUShort(this.buffer, (int) dataPos + 1 + 2 * index);
      return numArray;
    }

    public virtual void WriteSingleCoil(
      byte deviceAddress,
      ushort address,
      bool value,
      int timeout = 2000)
    {
      this.WriteSingleCoil(deviceAddress, address, value ? (ushort) 65280 : (ushort) 0, timeout);
    }

    public virtual void WriteSingleCoil(
      byte deviceAddress,
      ushort address,
      ushort value,
      int timeout = 2000)
    {
      object telegramContext = (object) null;
      short telegramLength;
      short dataPos;
      this.@interface.CreateTelegram(deviceAddress, (byte) 5, (short) 4, this.buffer, out telegramLength, out dataPos, false, ref telegramContext);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos, address);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos + 2, value);
      int num = (int) this.SendReceive(deviceAddress, ModbusFunctionCode.WriteSingleCoil, timeout, telegramLength, (short) 4, telegramContext, ref dataPos);
    }

    public virtual void WriteSingleRegister(
      byte deviceAddress,
      ushort address,
      ushort value,
      int timeout = 2000)
    {
      object telegramContext = (object) null;
      short telegramLength;
      short dataPos;
      this.@interface.CreateTelegram(deviceAddress, (byte) 6, (short) 4, this.buffer, out telegramLength, out dataPos, false, ref telegramContext);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos, address);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos + 2, value);
      int num = (int) this.SendReceive(deviceAddress, ModbusFunctionCode.WriteSingleRegister, timeout, telegramLength, (short) 4, telegramContext, ref dataPos);
    }

    public virtual byte ReadExceptionStatus(byte deviceAddress, int timeout = 2000)
    {
      object telegramContext = (object) null;
      short telegramLength;
      short dataPos;
      this.@interface.CreateTelegram(deviceAddress, (byte) 7, (short) 0, this.buffer, out telegramLength, out dataPos, false, ref telegramContext);
      int num = (int) this.SendReceive(deviceAddress, ModbusFunctionCode.ReadExceptionStatus, timeout, telegramLength, (short) 1, telegramContext, ref dataPos);
      return deviceAddress == (byte) 0 ? (byte) 0 : this.buffer[(int) dataPos];
    }

    public virtual ushort[] Diagnostics(
      byte deviceAddress,
      ushort subFunction,
      ushort[] data,
      int timeout = 2000)
    {
      object telegramContext = (object) null;
      short telegramLength;
      short dataPos;
      this.@interface.CreateTelegram(deviceAddress, (byte) 8, (short) (2 + 2 * data.Length), this.buffer, out telegramLength, out dataPos, false, ref telegramContext);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos, subFunction);
      for (int index = 0; index < data.Length; ++index)
        ModbusUtils.InsertUShort(this.buffer, (int) dataPos + 2 + 2 * index, data[index]);
      short num = this.SendReceive(deviceAddress, ModbusFunctionCode.Diagnostics, timeout, telegramLength, (short) (2 + 2 * data.Length), telegramContext, ref dataPos);
      if (deviceAddress == (byte) 0)
        return (ushort[]) null;
      data = new ushort[((int) num - 2) / 2];
      for (int index = 0; index < data.Length; ++index)
        data[index] = ModbusUtils.ExtractUShort(this.buffer, (int) dataPos + 2 + 2 * index);
      return data;
    }

    public virtual void GetCommEventCounter(
      byte deviceAddress,
      out ushort status,
      out ushort eventCount,
      int timeout = 2000)
    {
      object telegramContext = (object) null;
      short telegramLength;
      short dataPos;
      this.@interface.CreateTelegram(deviceAddress, (byte) 11, (short) 0, this.buffer, out telegramLength, out dataPos, false, ref telegramContext);
      int num = (int) this.SendReceive(deviceAddress, ModbusFunctionCode.GetCommEventCounter, timeout, telegramLength, (short) 4, telegramContext, ref dataPos);
      if (deviceAddress == (byte) 0)
      {
        status = (ushort) 0;
        eventCount = (ushort) 0;
      }
      else
      {
        status = ModbusUtils.ExtractUShort(this.buffer, (int) dataPos);
        eventCount = ModbusUtils.ExtractUShort(this.buffer, (int) dataPos + 2);
      }
    }

    public virtual void GetCommEventLog(
      byte deviceAddress,
      out ushort status,
      out ushort eventCount,
      out ushort messageCount,
      out byte[] events,
      int timeout = 2000)
    {
      object telegramContext = (object) null;
      short telegramLength;
      short dataPos;
      this.@interface.CreateTelegram(deviceAddress, (byte) 12, (short) 0, this.buffer, out telegramLength, out dataPos, false, ref telegramContext);
      int num1 = (int) this.SendReceive(deviceAddress, ModbusFunctionCode.GetCommEventLog, timeout, telegramLength, (short) -1, telegramContext, ref dataPos);
      if (deviceAddress == (byte) 0)
      {
        status = (ushort) 0;
        eventCount = (ushort) 0;
        messageCount = (ushort) 0;
        events = (byte[]) null;
      }
      else
      {
        byte num2 = this.buffer[(int) dataPos];
        status = ModbusUtils.ExtractUShort(this.buffer, (int) dataPos + 1);
        eventCount = ModbusUtils.ExtractUShort(this.buffer, (int) dataPos + 3);
        messageCount = ModbusUtils.ExtractUShort(this.buffer, (int) dataPos + 5);
        events = new byte[(int) num2 - 6];
        Array.Copy((Array) this.buffer, (int) dataPos + 7, (Array) events, 0, events.Length);
      }
    }

    public virtual void WriteMultipleCoils(
      byte deviceAddress,
      ushort startAddress,
      ushort coilCount,
      byte[] coils,
      int timeout = 2000)
    {
      object telegramContext = (object) null;
      byte num1 = (byte) ((uint) coilCount / 8U);
      if ((int) coilCount % 8 > 0)
        ++num1;
      short telegramLength;
      short dataPos;
      this.@interface.CreateTelegram(deviceAddress, (byte) 15, (short) (5 + (int) num1), this.buffer, out telegramLength, out dataPos, false, ref telegramContext);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos, startAddress);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos + 2, coilCount);
      this.buffer[(int) dataPos + 4] = num1;
      Array.Copy((Array) coils, 0, (Array) this.buffer, (int) dataPos + 5, (int) num1);
      int num2 = (int) this.SendReceive(deviceAddress, ModbusFunctionCode.WriteMultipleCoils, timeout, telegramLength, (short) 4, telegramContext, ref dataPos);
    }

    public virtual void WriteMultipleRegisters(
      byte deviceAddress,
      ushort startAddress,
      ushort[] registers,
      int timeout = 2000)
    {
      this.WriteMultipleRegisters(deviceAddress, startAddress, registers, 0, registers.Length, timeout);
    }

    public virtual void WriteMultipleRegisters(
      byte deviceAddress,
      ushort startAddress,
      ushort[] registers,
      int offset,
      int count,
      int timeout = 2000)
    {
      if (offset < 0 || offset >= registers.Length)
        throw new ArgumentException(nameof (offset));
      if (offset + count > registers.Length)
        throw new ArgumentException(nameof (count));
      object telegramContext = (object) null;
      short telegramLength;
      short dataPos;
      this.@interface.CreateTelegram(deviceAddress, (byte) 16, (short) (5 + 2 * count), this.buffer, out telegramLength, out dataPos, false, ref telegramContext);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos, startAddress);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos + 2, (ushort) count);
      this.buffer[(int) dataPos + 4] = (byte) (2 * count);
      for (int index = 0; index < count; ++index)
        ModbusUtils.InsertUShort(this.buffer, (int) dataPos + 5 + 2 * index, registers[offset + index]);
      int num = (int) this.SendReceive(deviceAddress, ModbusFunctionCode.WriteMultipleRegisters, timeout, telegramLength, (short) 4, telegramContext, ref dataPos);
    }

    public virtual ushort[] ReadWriteMultipleRegisters(
      byte deviceAddress,
      ushort writeStartAddress,
      ushort[] writeRegisters,
      ushort readStartAddress,
      ushort readCount,
      int timeout = 2000)
    {
      object telegramContext = (object) null;
      short telegramLength;
      short dataPos;
      this.@interface.CreateTelegram(deviceAddress, (byte) 23, (short) (9 + 2 * writeRegisters.Length), this.buffer, out telegramLength, out dataPos, false, ref telegramContext);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos, readStartAddress);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos + 2, readCount);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos + 4, writeStartAddress);
      ModbusUtils.InsertUShort(this.buffer, (int) dataPos + 6, (ushort) writeRegisters.Length);
      this.buffer[(int) dataPos + 8] = (byte) (2 * writeRegisters.Length);
      for (int index = 0; index < writeRegisters.Length; ++index)
        ModbusUtils.InsertUShort(this.buffer, (int) dataPos + 9 + 2 * index, writeRegisters[index]);
      int num = (int) this.SendReceive(deviceAddress, ModbusFunctionCode.ReadWriteMultipleRegisters, timeout, telegramLength, (short) (1 + 2 * (int) readCount), telegramContext, ref dataPos);
      if (deviceAddress == (byte) 0)
        return (ushort[]) null;
      ushort[] numArray = new ushort[(int) readCount];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = ModbusUtils.ExtractUShort(this.buffer, (int) dataPos + 1 + 2 * index);
      return numArray;
    }

    public virtual DeviceIdentification[] ReadDeviceIdentification(
      byte deviceAddress,
      ModbusConformityLevel deviceIdCode,
      int timeout = 2000)
    {
      bool moreFollows = true;
      ModbusObjectId objectId = ModbusObjectId.VendorName;
      ArrayList arrayList = new ArrayList();
      while (moreFollows)
      {
        foreach (object obj in this.ReadDeviceIdentification(deviceAddress, deviceIdCode, ref objectId, out moreFollows, timeout))
          arrayList.Add(obj);
      }
      return (DeviceIdentification[]) arrayList.ToArray(typeof (DeviceIdentification));
    }

    public virtual DeviceIdentification[] ReadDeviceIdentification(
      byte deviceAddress,
      ModbusConformityLevel deviceIdCode,
      ref ModbusObjectId objectId,
      out bool moreFollows,
      int timeout = 2000)
    {
      object telegramContext = (object) null;
      short telegramLength;
      short dataPos;
      this.@interface.CreateTelegram(deviceAddress, (byte) 43, (short) 3, this.buffer, out telegramLength, out dataPos, false, ref telegramContext);
      this.buffer[(int) dataPos] = (byte) 14;
      this.buffer[(int) dataPos + 1] = (byte) deviceIdCode;
      this.buffer[(int) dataPos + 2] = (byte) objectId;
      int num1 = (int) this.SendReceive(deviceAddress, ModbusFunctionCode.ReadDeviceIdentification, timeout, telegramLength, (short) -1, telegramContext, ref dataPos);
      if (deviceAddress == (byte) 0)
      {
        moreFollows = false;
        return (DeviceIdentification[]) null;
      }
      moreFollows = this.buffer[(int) dataPos + 3] > (byte) 0;
      objectId = (ModbusObjectId) this.buffer[(int) dataPos + 4];
      byte num2 = this.buffer[(int) dataPos + 5];
      DeviceIdentification[] deviceIdentificationArray = new DeviceIdentification[(int) num2];
      short num3 = (short) ((int) dataPos + 6);
      for (int index1 = 0; index1 < (int) num2; ++index1)
      {
        char[] chArray = new char[(int) this.buffer[(int) num3 + 1]];
        for (int index2 = 0; index2 < chArray.Length; ++index2)
          chArray[index2] = (char) this.buffer[(int) num3 + 2 + index2];
        deviceIdentificationArray[index1] = new DeviceIdentification()
        {
          ObjectId = (ModbusObjectId) this.buffer[(int) num3],
          Value = new string(chArray)
        };
        num3 += (short) (2 + chArray.Length);
      }
      return deviceIdentificationArray;
    }

    public virtual DeviceIdentification ReadSpecificDeviceIdentification(
      byte deviceAddress,
      ModbusObjectId objectId,
      int timeout = 4000)
    {
      object telegramContext = (object) null;
      short telegramLength;
      short dataPos;
      this.@interface.CreateTelegram(deviceAddress, (byte) 43, (short) 3, this.buffer, out telegramLength, out dataPos, false, ref telegramContext);
      this.buffer[(int) dataPos] = (byte) 14;
      this.buffer[(int) dataPos + 1] = (byte) 4;
      this.buffer[(int) dataPos + 2] = (byte) objectId;
      int num1 = (int) this.SendReceive(deviceAddress, ModbusFunctionCode.ReadDeviceIdentification, timeout, telegramLength, (short) -1, telegramContext, ref dataPos);
      if (deviceAddress == (byte) 0)
        return new DeviceIdentification();
      if (this.buffer[(int) dataPos + 5] == (byte) 0)
        return new DeviceIdentification();
      short num2 = (short) ((int) dataPos + 6);
      char[] chArray = new char[(int) this.buffer[(int) num2 + 1]];
      for (int index = 0; index < chArray.Length; ++index)
        chArray[index] = (char) this.buffer[(int) num2 + 2 + index];
      return new DeviceIdentification()
      {
        ObjectId = (ModbusObjectId) this.buffer[(int) num2],
        Value = new string(chArray)
      };
    }
  }
}
