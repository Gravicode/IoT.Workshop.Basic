// Decompiled with JetBrains decompiler
// Type: nanoFramework.Modbus.ModbusFunctionCode
// Assembly: nanoFramework.Modbus, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B923EC90-60E3-4FDC-BF8E-BE964652F39A
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFramework.Modbus.dll

namespace nanoFramework.Modbus
{
  public enum ModbusFunctionCode : byte
  {
    ReadCoils = 1,
    ReadDiscreteInputs = 2,
    ReadHoldingRegisters = 3,
    ReadInputRegisters = 4,
    WriteSingleCoil = 5,
    WriteSingleRegister = 6,
    ReadExceptionStatus = 7,
    Diagnostics = 8,
    GetCommEventCounter = 11, // 0x0B
    GetCommEventLog = 12, // 0x0C
    ReadDeviceIdentification2 = 14, // 0x0E
    WriteMultipleCoils = 15, // 0x0F
    WriteMultipleRegisters = 16, // 0x10
    ReadWriteMultipleRegisters = 23, // 0x17
    ReadDeviceIdentification = 43, // 0x2B
  }
}
