// Decompiled with JetBrains decompiler
// Type: nanoFramework.Modbus.ModbusGateway
// Assembly: nanoFramework.Modbus, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B923EC90-60E3-4FDC-BF8E-BE964652F39A
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFramework.Modbus.dll

using nanoFramework.Modbus.Interface;
using System;
using System.Collections;

namespace nanoFramework.Modbus
{
  public class ModbusGateway : ModbusDevice
  {
    private readonly Hashtable masterMap = new Hashtable(247);

    public ModbusGateway(byte deviceAddress)
      : base(deviceAddress)
    {
    }

    public ModbusGateway(byte deviceAddress, IModbusInterface masterInterface, int timeout)
      : this(deviceAddress)
    {
      this.AddMaster(masterInterface, timeout);
    }

    public void AddMaster(IModbusInterface masterInterface, int timeout)
    {
      Hashtable addressMap = new Hashtable();
      for (byte index = 1; index < (byte) 248; ++index)
        addressMap.Add((object) index, (object) index);
      this.AddMaster(masterInterface, timeout, addressMap);
    }

    public void AddMaster(IModbusInterface masterInterface, int timeout, Hashtable addressMap)
    {
      masterInterface.PrepareWrite();
      foreach (byte key in (IEnumerable) addressMap.Keys)
        this.masterMap[(object) key] = (object) new ModbusGateway.GatewayMaster(masterInterface, (byte) addressMap[(object) key], timeout);
    }

    protected override string OnGetDeviceIdentification(ModbusObjectId objectId) => "";

    protected override ModbusConformityLevel GetConformityLevel() => ModbusConformityLevel.Basic;

    protected override void OnHandleTelegram(
      IModbusInterface intf,
      byte deviceAddress,
      bool isBroadcast,
      short telegramLength,
      object telegramContext,
      ModbusFunctionCode fc,
      short dataPos,
      short dataLength)
    {
      ModbusGateway.GatewayMaster master = (ModbusGateway.GatewayMaster) this.masterMap[(object) deviceAddress];
      if (master != null)
      {
        object telegramContext1 = (object) null;
        short telegramLength1;
        short dataPos1;
        master.MasterInterface.CreateTelegram(master.TargetAddress, (byte) fc, dataLength, master.Buffer, out telegramLength1, out dataPos1, false, ref telegramContext1);
        Array.Copy((Array) this.Buffer, (int) dataPos, (Array) master.Buffer, (int) dataPos1, (int) dataLength);
        try
        {
          short dataLength1 = this.SendReceive(master.MasterInterface, master.Buffer, master.TargetAddress, fc, master.Timeout, telegramLength1, (short) -1, telegramContext1, ref dataPos1);
          intf.CreateTelegram(deviceAddress, (byte) fc, dataLength1, this.Buffer, out telegramLength, out dataPos, true, ref telegramContext);
          Array.Copy((Array) master.Buffer, (int) dataPos1, (Array) this.Buffer, (int) dataPos, (int) dataLength1);
          intf.SendTelegram(this.Buffer, telegramLength);
        }
        catch (ModbusException ex)
        {
          try
          {
            if (ex.ErrorCode == ModbusErrorCode.Timeout)
              this.SendErrorResult(intf, false, deviceAddress, telegramContext, fc, ModbusErrorCode.GatewayTargetDeviceFailedToRespond);
            else if ((ex.ErrorCode & (ModbusErrorCode) 65280) != ModbusErrorCode.NoError)
              this.SendErrorResult(intf, false, deviceAddress, telegramContext, fc, ModbusErrorCode.GatewayTargetDeviceFailedToRespond);
            else
              this.SendErrorResult(intf, false, deviceAddress, telegramContext, fc, ex.ErrorCode);
          }
          catch
          {
          }
        }
        catch
        {
          try
          {
            this.SendErrorResult(intf, false, deviceAddress, telegramContext, fc, ModbusErrorCode.GatewayPathUnavailable);
          }
          catch
          {
          }
        }
      }
      else
        base.OnHandleTelegram(intf, deviceAddress, isBroadcast, telegramLength, telegramContext, fc, dataPos, dataLength);
    }

    private short SendReceive(
      IModbusInterface masterInterface,
      byte[] buffer,
      byte deviceAddress,
      ModbusFunctionCode fc,
      int timeout,
      short telegramLength,
      short desiredDataLength,
      object telegramContext,
      ref short dataPos)
    {
      lock (masterInterface)
      {
        masterInterface.SendTelegram(buffer, telegramLength);
        if (deviceAddress == (byte) 0)
          return 0;
        try
        {
          masterInterface.PrepareRead();
          byte fkt = 0;
          short dataLength = 0;
          while (timeout > 0)
          {
            DateTime now = DateTime.UtcNow;
            long ticks = now.Ticks;
            if (!masterInterface.ReceiveTelegram(buffer, desiredDataLength, timeout, out telegramLength))
              throw new ModbusException(ModbusErrorCode.Timeout);
            int num1 = timeout;
            now = DateTime.UtcNow;
            int num2 = (int) ((now.Ticks - ticks) / 10000L);
            timeout = num1 - num2;
            byte address;
            if (!masterInterface.ParseTelegram(buffer, telegramLength, true, ref telegramContext, out address, out fkt, out dataPos, out dataLength) || (int) address != (int) deviceAddress || (ModbusFunctionCode) ((int) fkt & (int) sbyte.MaxValue) != fc)
            {
              if (timeout <= 0)
                throw new ModbusException(ModbusErrorCode.Timeout);
            }
            else
              break;
          }
          if (((int) fkt & 128) != 0)
            throw new ModbusException((ModbusErrorCode) buffer[(int) dataPos]);
          return dataLength;
        }
        finally
        {
          masterInterface.PrepareWrite();
        }
      }
    }

    private class GatewayMaster
    {
      public readonly IModbusInterface MasterInterface;
      public readonly byte TargetAddress;
      public readonly int Timeout;
      public readonly byte[] Buffer;

      public GatewayMaster(IModbusInterface masterInterface, byte targetAddress, int timeout)
      {
        this.MasterInterface = masterInterface;
        this.TargetAddress = targetAddress;
        this.Timeout = timeout;
        this.Buffer = new byte[(int) masterInterface.MaxTelegramLength];
      }
    }
  }
}
