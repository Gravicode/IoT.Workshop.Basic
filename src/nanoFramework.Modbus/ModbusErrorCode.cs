// Decompiled with JetBrains decompiler
// Type: nanoFramework.Modbus.ModbusErrorCode
// Assembly: nanoFramework.Modbus, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B923EC90-60E3-4FDC-BF8E-BE964652F39A
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFramework.Modbus.dll

namespace nanoFramework.Modbus
{
  public enum ModbusErrorCode : ushort
  {
    NoError = 0,
    IllegalFunction = 1,
    IllegalDataAddress = 2,
    IllegalDataValue = 3,
    ServerDeviceFailure = 4,
    Acknowledge = 5,
    ServerDeviceBusy = 6,
    NegativeAcknowledgement = 7,
    MemoryParityError = 8,
    GatewayPathUnavailable = 10, // 0x000A
    GatewayTargetDeviceFailedToRespond = 11, // 0x000B
    Unspecified = 256, // 0x0100
    Timeout = 257, // 0x0101
    CrcError = 258, // 0x0102
    ResponseTooShort = 259, // 0x0103
  }
}
