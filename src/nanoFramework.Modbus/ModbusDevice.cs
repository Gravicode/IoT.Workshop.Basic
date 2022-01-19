// Decompiled with JetBrains decompiler
// Type: nanoFramework.Modbus.ModbusDevice
// Assembly: nanoFramework.Modbus, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B923EC90-60E3-4FDC-BF8E-BE964652F39A
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFramework.Modbus.dll

using nanoFramework.Modbus.Interface;
using System;
using System.Collections;
using System.Threading;

namespace nanoFramework.Modbus
{
  public abstract class ModbusDevice
  {
    private readonly ArrayList interfaces = new ArrayList();
    private readonly byte deviceAddress;
    private readonly object syncObject;
    private byte[] buffer;
    private Thread thread;

    protected byte[] Buffer => this.Buffer1;

    protected ModbusDevice(byte deviceAddress, object syncObject = null)
    {
      this.deviceAddress = deviceAddress;
      this.syncObject = syncObject ?? new object();
    }

    protected ModbusDevice(IModbusInterface intf, byte deviceAddress, object syncObject = null)
      : this(deviceAddress, syncObject)
    {
      intf.PrepareRead();
      this.interfaces.Add((object) intf);
      this.Buffer1 = new byte[(int) intf.MaxTelegramLength];
    }

    public void AddInterface(IModbusInterface intf)
    {
      lock (this.syncObject)
      {
        if (this.Buffer1 == null || (int) intf.MaxTelegramLength > this.Buffer1.Length)
          this.Buffer1 = new byte[(int) intf.MaxTelegramLength];
        intf.PrepareRead();
        this.interfaces.Add((object) intf);
      }
    }

    public void RemoveInterface(IModbusInterface intf)
    {
      lock (this.syncObject)
      {
        this.interfaces.Remove((object) intf);
        if (this.interfaces.Count != 0)
          return;
        this.Buffer1 = (byte[]) null;
      }
    }

    public void Start()
    {
      if (this.IsRunning)
        return;
      this.thread = new Thread(new ThreadStart(this.Run));
      this.IsRunning = true;
      this.thread.Start();
    }

    public void Stop() => this.IsRunning = false;

    public bool IsRunning { get; private set; }

    public byte[] Buffer1
    {
      get => this.Buffer2;
      set => this.Buffer2 = value;
    }

    public byte[] Buffer2
    {
      get => this.buffer;
      set => this.buffer = value;
    }

    private void Run()
    {
      object telegramContext = (object) null;
      while (this.IsRunning)
      {
        try
        {
          lock (this.syncObject)
          {
            for (int index = this.interfaces.Count - 1; index >= 0; --index)
            {
              IModbusInterface modbusInterface = (IModbusInterface) this.interfaces[index];
              try
              {
                if (modbusInterface.IsDataAvailable)
                {
                  short telegramLength;
                  if (modbusInterface.ReceiveTelegram(this.Buffer1, (short) -1, 1000, out telegramLength))
                  {
                    try
                    {
                      byte address;
                      byte fkt;
                      short dataPos;
                      short dataLength;
                      modbusInterface.ParseTelegram(this.Buffer1, telegramLength, false, ref telegramContext, out address, out fkt, out dataPos, out dataLength);
                      modbusInterface.PrepareWrite();
                      this.OnMessageReceived(modbusInterface, address, (ModbusFunctionCode) fkt);
                      bool isBroadcast = address == (byte) 0;
                      if (!isBroadcast && this.deviceAddress != (byte) 248)
                      {
                        if ((int) address != (int) this.deviceAddress)
                          goto label_12;
                      }
                      this.OnHandleTelegram(modbusInterface, address, isBroadcast, telegramLength, telegramContext, (ModbusFunctionCode) fkt, dataPos, dataLength);
                    }
                    catch
                    {
                      modbusInterface.ClearInputBuffer();
                    }
                    finally
                    {
                      modbusInterface.PrepareRead();
                    }
                  }
                }
              }
              catch
              {
              }
label_12:
              if (!modbusInterface.IsConnectionOk)
              {
                this.interfaces.RemoveAt(index);
                if (modbusInterface is IDisposable disposable8)
                {
                  try
                  {
                    disposable8.Dispose();
                  }
                  catch
                  {
                  }
                }
              }
            }
          }
          Thread.Sleep(1);
        }
        catch
        {
        }
      }
      this.thread = (Thread) null;
    }

    protected virtual void OnMessageReceived(
      IModbusInterface modbusInterface,
      byte deviceAddress,
      ModbusFunctionCode functionCode)
    {
    }

    protected virtual void OnHandleTelegram(
      IModbusInterface intf,
      byte deviceAddress,
      bool isBroadcast,
      short telegramLength,
      object telegramContext,
      ModbusFunctionCode fc,
      short dataPos,
      short dataLength)
    {
      try
      {
        switch (fc)
        {
          case ModbusFunctionCode.ReadCoils:
            this.ReadCoils(intf, isBroadcast, telegramContext, fc, dataPos, dataLength);
            break;
          case ModbusFunctionCode.ReadDiscreteInputs:
            this.ReadDiscreteInputs(intf, isBroadcast, telegramContext, fc, dataPos, dataLength);
            break;
          case ModbusFunctionCode.ReadHoldingRegisters:
            this.ReadHoldingRegisters(intf, isBroadcast, telegramContext, fc, dataPos, dataLength);
            break;
          case ModbusFunctionCode.ReadInputRegisters:
            this.ReadInputRegisters(intf, isBroadcast, telegramContext, fc, dataPos, dataLength);
            break;
          case ModbusFunctionCode.WriteSingleCoil:
            this.WriteSingleCoil(intf, isBroadcast, telegramContext, fc, dataPos, dataLength);
            break;
          case ModbusFunctionCode.WriteSingleRegister:
            this.WriteSingleRegister(intf, isBroadcast, telegramContext, fc, dataPos, dataLength);
            break;
          case ModbusFunctionCode.ReadDeviceIdentification2:
          case ModbusFunctionCode.ReadDeviceIdentification:
            this.ReadDeviceIdentification(intf, isBroadcast, telegramContext, fc, dataPos, dataLength);
            break;
          case ModbusFunctionCode.WriteMultipleCoils:
            this.WriteMultipleCoils(intf, isBroadcast, telegramContext, fc, dataPos, dataLength);
            break;
          case ModbusFunctionCode.WriteMultipleRegisters:
            this.WriteMultipleRegisters(intf, isBroadcast, telegramContext, fc, dataPos, dataLength);
            break;
          case ModbusFunctionCode.ReadWriteMultipleRegisters:
            this.ReadWriteMultipleRegisters(intf, isBroadcast, telegramContext, fc, dataPos, dataLength);
            break;
          default:
            if (this.OnCustomTelegram(intf, isBroadcast, this.Buffer1, telegramLength, telegramContext, fc, dataPos, dataLength))
              break;
            this.SendErrorResult(intf, isBroadcast, this.deviceAddress, telegramContext, fc, ModbusErrorCode.IllegalFunction);
            break;
        }
      }
      catch
      {
        this.SendErrorResult(intf, isBroadcast, this.deviceAddress, telegramContext, fc, ModbusErrorCode.ServerDeviceFailure);
      }
    }

    private void ReadCoils(
      IModbusInterface intf,
      bool isBroadcast,
      object telegramContext,
      ModbusFunctionCode fc,
      short dataPos,
      short dataLength)
    {
      if (dataLength < (short) 4)
      {
        this.SendErrorResult(intf, isBroadcast, this.deviceAddress, telegramContext, fc, ModbusErrorCode.IllegalDataValue);
      }
      else
      {
        ushort startAddress = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos);
        ushort coilCount = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos + 2);
        byte[] coils;
        if ((int) coilCount % 8 == 0)
        {
          coils = new byte[(int) coilCount / 8];
        }
        else
        {
          coils = new byte[(int) coilCount / 8 + 1];
          coils[coils.Length - 1] = (byte) 0;
        }
        ModbusErrorCode modbusErrorCode = this.OnReadCoils(isBroadcast, startAddress, coilCount, coils);
        if (isBroadcast)
          return;
        if (modbusErrorCode != ModbusErrorCode.NoError)
        {
          this.SendErrorResult(intf, false, this.deviceAddress, telegramContext, fc, modbusErrorCode);
        }
        else
        {
          short telegramLength;
          intf.CreateTelegram(this.deviceAddress, (byte) fc, (short) (1 + coils.Length), this.Buffer1, out telegramLength, out dataPos, true, ref telegramContext);
          this.Buffer1[(int) dataPos] = (byte) coils.Length;
          Array.Copy((Array) coils, 0, (Array) this.Buffer1, (int) dataPos + 1, coils.Length);
          intf.SendTelegram(this.Buffer1, telegramLength);
        }
      }
    }

    protected virtual ModbusErrorCode OnReadCoils(
      bool isBroadcast,
      ushort startAddress,
      ushort coilCount,
      byte[] coils)
    {
      return ModbusErrorCode.IllegalFunction;
    }

    private void ReadDiscreteInputs(
      IModbusInterface intf,
      bool isBroadcast,
      object telegramContext,
      ModbusFunctionCode fc,
      short dataPos,
      short dataLength)
    {
      if (dataLength < (short) 4)
      {
        this.SendErrorResult(intf, isBroadcast, this.deviceAddress, telegramContext, fc, ModbusErrorCode.IllegalDataValue);
      }
      else
      {
        ushort startAddress = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos);
        ushort inputCount = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos + 2);
        byte[] inputs;
        if ((int) inputCount % 8 == 0)
        {
          inputs = new byte[(int) inputCount / 8];
        }
        else
        {
          inputs = new byte[(int) inputCount / 8 + 1];
          inputs[inputs.Length - 1] = (byte) 0;
        }
        ModbusErrorCode modbusErrorCode = this.OnReadDiscreteInputs(isBroadcast, startAddress, inputCount, inputs);
        if (isBroadcast)
          return;
        if (modbusErrorCode != ModbusErrorCode.NoError)
        {
          this.SendErrorResult(intf, false, this.deviceAddress, telegramContext, fc, modbusErrorCode);
        }
        else
        {
          short telegramLength;
          intf.CreateTelegram(this.deviceAddress, (byte) fc, (short) (1 + inputs.Length), this.Buffer1, out telegramLength, out dataPos, true, ref telegramContext);
          this.Buffer1[(int) dataPos] = (byte) inputs.Length;
          Array.Copy((Array) inputs, 0, (Array) this.Buffer1, (int) dataPos + 1, inputs.Length);
          intf.SendTelegram(this.Buffer1, telegramLength);
        }
      }
    }

    protected virtual ModbusErrorCode OnReadDiscreteInputs(
      bool isBroadcast,
      ushort startAddress,
      ushort inputCount,
      byte[] inputs)
    {
      return ModbusErrorCode.IllegalFunction;
    }

    private void ReadHoldingRegisters(
      IModbusInterface intf,
      bool isBroadcast,
      object telegramContext,
      ModbusFunctionCode fc,
      short dataPos,
      short dataLength)
    {
      if (dataLength < (short) 4)
      {
        this.SendErrorResult(intf, isBroadcast, this.deviceAddress, telegramContext, fc, ModbusErrorCode.IllegalDataValue);
      }
      else
      {
        ushort startAddress = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos);
        ushort num = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos + 2);
        ushort[] registers = new ushort[(int) num];
        ModbusErrorCode modbusErrorCode = this.OnReadHoldingRegisters(isBroadcast, startAddress, registers);
        if (isBroadcast)
          return;
        if (modbusErrorCode != ModbusErrorCode.NoError)
        {
          this.SendErrorResult(intf, false, this.deviceAddress, telegramContext, fc, modbusErrorCode);
        }
        else
        {
          short telegramLength;
          intf.CreateTelegram(this.deviceAddress, (byte) fc, (short) (1 + 2 * registers.Length), this.Buffer1, out telegramLength, out dataPos, true, ref telegramContext);
          this.Buffer1[(int) dataPos] = (byte) (2 * registers.Length);
          for (int index = 0; index < (int) num; ++index)
            ModbusUtils.InsertUShort(this.Buffer1, (int) dataPos + 1 + 2 * index, registers[index]);
          intf.SendTelegram(this.Buffer1, telegramLength);
        }
      }
    }

    protected virtual ModbusErrorCode OnReadHoldingRegisters(
      bool isBroadcast,
      ushort startAddress,
      ushort[] registers)
    {
      return ModbusErrorCode.IllegalFunction;
    }

    private void ReadInputRegisters(
      IModbusInterface intf,
      bool isBroadcast,
      object telegramContext,
      ModbusFunctionCode fc,
      short dataPos,
      short dataLength)
    {
      if (dataLength < (short) 4)
      {
        this.SendErrorResult(intf, isBroadcast, this.deviceAddress, telegramContext, fc, ModbusErrorCode.IllegalDataValue);
      }
      else
      {
        ushort startAddress = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos);
        ushort num = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos + 2);
        ushort[] registers = new ushort[(int) num];
        ModbusErrorCode modbusErrorCode = this.OnReadInputRegisters(isBroadcast, startAddress, registers);
        if (isBroadcast)
          return;
        if (modbusErrorCode != ModbusErrorCode.NoError)
        {
          this.SendErrorResult(intf, false, this.deviceAddress, telegramContext, fc, modbusErrorCode);
        }
        else
        {
          short telegramLength;
          intf.CreateTelegram(this.deviceAddress, (byte) fc, (short) (1 + 2 * registers.Length), this.Buffer1, out telegramLength, out dataPos, true, ref telegramContext);
          this.Buffer1[(int) dataPos] = (byte) (2 * registers.Length);
          for (int index = 0; index < (int) num; ++index)
            ModbusUtils.InsertUShort(this.Buffer1, (int) dataPos + 1 + 2 * index, registers[index]);
          intf.SendTelegram(this.Buffer1, telegramLength);
        }
      }
    }

    protected virtual ModbusErrorCode OnReadInputRegisters(
      bool isBroadcast,
      ushort startAddress,
      ushort[] registers)
    {
      return ModbusErrorCode.IllegalFunction;
    }

    private void WriteSingleCoil(
      IModbusInterface intf,
      bool isBroadcast,
      object telegramContext,
      ModbusFunctionCode fc,
      short dataPos,
      short dataLength)
    {
      if (dataLength < (short) 4)
      {
        this.SendErrorResult(intf, isBroadcast, this.deviceAddress, telegramContext, fc, ModbusErrorCode.IllegalDataValue);
      }
      else
      {
        ushort address = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos);
        ushort num = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos + 2);
        ModbusErrorCode modbusErrorCode = this.OnWriteSingleCoil(isBroadcast, address, num > (ushort) 0);
        if (isBroadcast)
          return;
        if (modbusErrorCode != ModbusErrorCode.NoError)
        {
          this.SendErrorResult(intf, false, this.deviceAddress, telegramContext, fc, modbusErrorCode);
        }
        else
        {
          short telegramLength;
          intf.CreateTelegram(this.deviceAddress, (byte) fc, (short) 4, this.Buffer1, out telegramLength, out dataPos, true, ref telegramContext);
          ModbusUtils.InsertUShort(this.Buffer1, (int) dataPos, address);
          ModbusUtils.InsertUShort(this.Buffer1, (int) dataPos + 2, num);
          intf.SendTelegram(this.Buffer1, telegramLength);
        }
      }
    }

    protected virtual ModbusErrorCode OnWriteSingleCoil(
      bool isBroadcast,
      ushort address,
      bool value)
    {
      return ModbusErrorCode.IllegalFunction;
    }

    private void WriteSingleRegister(
      IModbusInterface intf,
      bool isBroadcast,
      object telegramContext,
      ModbusFunctionCode fc,
      short dataPos,
      short dataLength)
    {
      if (dataLength < (short) 4)
      {
        this.SendErrorResult(intf, isBroadcast, this.deviceAddress, telegramContext, fc, ModbusErrorCode.IllegalDataValue);
      }
      else
      {
        ushort address = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos);
        ushort num = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos + 2);
        ModbusErrorCode modbusErrorCode = this.OnWriteSingleRegister(isBroadcast, address, num);
        if (isBroadcast)
          return;
        if (modbusErrorCode != ModbusErrorCode.NoError)
        {
          this.SendErrorResult(intf, false, this.deviceAddress, telegramContext, fc, modbusErrorCode);
        }
        else
        {
          short telegramLength;
          intf.CreateTelegram(this.deviceAddress, (byte) fc, (short) 4, this.Buffer1, out telegramLength, out dataPos, true, ref telegramContext);
          ModbusUtils.InsertUShort(this.Buffer1, (int) dataPos, address);
          ModbusUtils.InsertUShort(this.Buffer1, (int) dataPos + 2, num);
          intf.SendTelegram(this.Buffer1, telegramLength);
        }
      }
    }

    protected virtual ModbusErrorCode OnWriteSingleRegister(
      bool isBroadcast,
      ushort address,
      ushort value)
    {
      return ModbusErrorCode.IllegalFunction;
    }

    private void WriteMultipleCoils(
      IModbusInterface intf,
      bool isBroadcast,
      object telegramContext,
      ModbusFunctionCode fc,
      short dataPos,
      short dataLength)
    {
      if (dataLength < (short) 5)
      {
        this.SendErrorResult(intf, isBroadcast, this.deviceAddress, telegramContext, fc, ModbusErrorCode.IllegalDataValue);
      }
      else
      {
        ushort startAddress = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos);
        ushort outputCount = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos + 2);
        byte[] values = new byte[(int) this.Buffer1[(int) dataPos + 4]];
        Array.Copy((Array) this.Buffer1, (int) dataPos + 5, (Array) values, 0, values.Length);
        ModbusErrorCode modbusErrorCode = this.OnWriteMultipleCoils(isBroadcast, startAddress, outputCount, values);
        if (isBroadcast)
          return;
        if (modbusErrorCode != ModbusErrorCode.NoError)
        {
          this.SendErrorResult(intf, false, this.deviceAddress, telegramContext, fc, modbusErrorCode);
        }
        else
        {
          short telegramLength;
          intf.CreateTelegram(this.deviceAddress, (byte) fc, (short) 4, this.Buffer1, out telegramLength, out dataPos, true, ref telegramContext);
          ModbusUtils.InsertUShort(this.Buffer1, (int) dataPos, startAddress);
          ModbusUtils.InsertUShort(this.Buffer1, (int) dataPos + 2, outputCount);
          intf.SendTelegram(this.Buffer1, telegramLength);
        }
      }
    }

    protected virtual ModbusErrorCode OnWriteMultipleCoils(
      bool isBroadcast,
      ushort startAddress,
      ushort outputCount,
      byte[] values)
    {
      return ModbusErrorCode.IllegalFunction;
    }

    private void WriteMultipleRegisters(
      IModbusInterface intf,
      bool isBroadcast,
      object telegramContext,
      ModbusFunctionCode fc,
      short dataPos,
      short dataLength)
    {
      if (dataLength < (short) 5)
      {
        this.SendErrorResult(intf, isBroadcast, this.deviceAddress, telegramContext, fc, ModbusErrorCode.IllegalDataValue);
      }
      else
      {
        ushort startAddress = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos);
        ushort num = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos + 2);
        ushort[] registers = new ushort[(int) num];
        for (int index = 0; index < (int) num; ++index)
          registers[index] = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos + 5 + 2 * index);
        ModbusErrorCode modbusErrorCode = this.OnWriteMultipleRegisters(isBroadcast, startAddress, registers);
        if (isBroadcast)
          return;
        if (modbusErrorCode != ModbusErrorCode.NoError)
        {
          this.SendErrorResult(intf, false, this.deviceAddress, telegramContext, fc, modbusErrorCode);
        }
        else
        {
          short telegramLength;
          intf.CreateTelegram(this.deviceAddress, (byte) fc, (short) 4, this.Buffer1, out telegramLength, out dataPos, true, ref telegramContext);
          ModbusUtils.InsertUShort(this.Buffer1, (int) dataPos, startAddress);
          ModbusUtils.InsertUShort(this.Buffer1, (int) dataPos + 2, num);
          intf.SendTelegram(this.Buffer1, telegramLength);
        }
      }
    }

    protected virtual ModbusErrorCode OnWriteMultipleRegisters(
      bool isBroadcast,
      ushort startAddress,
      ushort[] registers)
    {
      return ModbusErrorCode.IllegalFunction;
    }

    private void ReadWriteMultipleRegisters(
      IModbusInterface intf,
      bool isBroadcast,
      object telegramContext,
      ModbusFunctionCode fc,
      short dataPos,
      short dataLength)
    {
      if (dataLength < (short) 9)
      {
        this.SendErrorResult(intf, isBroadcast, this.deviceAddress, telegramContext, fc, ModbusErrorCode.IllegalDataValue);
      }
      else
      {
        ushort readStartAddress = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos);
        ushort num1 = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos + 2);
        ushort writeStartAddress = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos + 4);
        ushort num2 = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos + 6);
        ushort[] writeRegisters = new ushort[(int) num2];
        for (int index = 0; index < (int) num2; ++index)
          writeRegisters[index] = ModbusUtils.ExtractUShort(this.Buffer1, (int) dataPos + 5 + 2 * index);
        ushort[] readRegisters = new ushort[(int) num1];
        ModbusErrorCode modbusErrorCode = this.OnReadWriteMultipleRegisters(isBroadcast, writeStartAddress, writeRegisters, readStartAddress, readRegisters);
        if (isBroadcast)
          return;
        if (modbusErrorCode != ModbusErrorCode.NoError)
        {
          this.SendErrorResult(intf, false, this.deviceAddress, telegramContext, fc, modbusErrorCode);
        }
        else
        {
          short telegramLength;
          intf.CreateTelegram(this.deviceAddress, (byte) fc, (short) (1 + 2 * readRegisters.Length), this.Buffer1, out telegramLength, out dataPos, true, ref telegramContext);
          this.Buffer1[(int) dataPos] = (byte) (2 * readRegisters.Length);
          for (int index = 0; index < (int) num1; ++index)
            ModbusUtils.InsertUShort(this.Buffer1, (int) dataPos + 1 + 2 * index, readRegisters[index]);
          intf.SendTelegram(this.Buffer1, telegramLength);
        }
      }
    }

    protected virtual ModbusErrorCode OnReadWriteMultipleRegisters(
      bool isBroadcast,
      ushort writeStartAddress,
      ushort[] writeRegisters,
      ushort readStartAddress,
      ushort[] readRegisters)
    {
      return ModbusErrorCode.IllegalFunction;
    }

    protected virtual void ReadDeviceIdentification(
      IModbusInterface intf,
      bool isBroadcast,
      object telegramContext,
      ModbusFunctionCode fc,
      short dataPos,
      short dataLength)
    {
      if (isBroadcast)
        return;
      if (dataLength < (short) 3)
      {
        this.SendErrorResult(intf, false, this.deviceAddress, telegramContext, fc, ModbusErrorCode.IllegalDataValue);
      }
      else
      {
        byte num1 = this.Buffer1[(int) dataPos + 1];
        if (num1 < (byte) 1 || num1 > (byte) 4)
        {
          this.SendErrorResult(intf, false, this.deviceAddress, telegramContext, fc, ModbusErrorCode.IllegalDataValue);
        }
        else
        {
          byte num2 = this.Buffer1[(int) dataPos + 2];
          byte num3;
          switch (num1)
          {
            case 0:
              num3 = (byte) 2;
              break;
            case 1:
              num3 = (byte) 127;
              break;
            case 2:
              num3 = byte.MaxValue;
              break;
            default:
              num3 = num2;
              break;
          }
          byte[] numArray1 = new byte[(int) intf.MaxTelegramLength - 6];
          byte num4 = 0;
          short num5 = 0;
          bool flag = false;
          byte num6 = 0;
          for (short index1 = (short) num2; (int) index1 <= (int) num3; ++index1)
          {
            string deviceIdentification = this.OnGetDeviceIdentification((ModbusObjectId) index1);
            if (deviceIdentification != null)
            {
              if (numArray1.Length - ((int) num5 + 2) >= deviceIdentification.Length)
              {
                ++num4;
                byte[] numArray2 = numArray1;
                int index2 = (int) num5;
                short num7 = (short) (index2 + 1);
                int num8 = (int) (byte) index1;
                numArray2[index2] = (byte) num8;
                byte[] numArray3 = numArray1;
                int index3 = (int) num7;
                num5 = (short) (index3 + 1);
                int length = (int) (byte) deviceIdentification.Length;
                numArray3[index3] = (byte) length;
                for (int index4 = 0; index4 < deviceIdentification.Length; ++index4)
                  numArray1[(int) num5++] = (byte) deviceIdentification[index4];
              }
              else
              {
                flag = true;
                num6 = (byte) ((uint) index1 + 1U);
                break;
              }
            }
            else
              break;
          }
          short telegramLength;
          intf.CreateTelegram(this.deviceAddress, (byte) fc, (short) (6 + (int) num5), this.Buffer1, out telegramLength, out dataPos, true, ref telegramContext);
          this.Buffer1[(int) dataPos] = (byte) 14;
          this.Buffer1[(int) dataPos + 1] = num1;
          this.Buffer1[(int) dataPos + 2] = (byte) (this.GetConformityLevel() & (ModbusConformityLevel) 128);
          this.Buffer1[(int) dataPos + 3] = flag ? byte.MaxValue : (byte) 0;
          this.Buffer1[(int) dataPos + 4] = num6;
          this.Buffer1[(int) dataPos + 5] = num4;
          Array.Copy((Array) numArray1, 0, (Array) this.Buffer1, (int) dataPos + 6, (int) num5);
          intf.SendTelegram(this.Buffer1, telegramLength);
        }
      }
    }

    protected abstract string OnGetDeviceIdentification(ModbusObjectId objectId);

    protected abstract ModbusConformityLevel GetConformityLevel();

    protected virtual bool OnCustomTelegram(
      IModbusInterface intf,
      bool isBroadcast,
      byte[] buffer,
      short telegramLength,
      object telegramContext,
      ModbusFunctionCode fc,
      short dataPos,
      short dataLength)
    {
      return false;
    }

    protected virtual void SendErrorResult(
      IModbusInterface intf,
      bool isBroadcast,
      byte deviceAddress,
      object telegramContext,
      ModbusFunctionCode fc,
      ModbusErrorCode modbusErrorCode)
    {
      if (isBroadcast)
        return;
      short telegramLength;
      short dataPos;
      intf.CreateTelegram(deviceAddress, (byte) (fc | (ModbusFunctionCode) 128), (short) 1, this.Buffer1, out telegramLength, out dataPos, true, ref telegramContext);
      this.Buffer1[(int) dataPos] = (byte) modbusErrorCode;
      intf.SendTelegram(this.Buffer1, telegramLength);
    }
  }
}
