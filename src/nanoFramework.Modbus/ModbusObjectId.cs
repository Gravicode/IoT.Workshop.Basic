// Decompiled with JetBrains decompiler
// Type: nanoFramework.Modbus.ModbusObjectId
// Assembly: nanoFramework.Modbus, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B923EC90-60E3-4FDC-BF8E-BE964652F39A
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFramework.Modbus.dll

namespace nanoFramework.Modbus
{
  public enum ModbusObjectId : byte
  {
    VendorName = 0,
    ProductCode = 1,
    MajorMinorRevision = 2,
    VendorUrl = 3,
    ProductName = 4,
    ModelName = 5,
    UserApplicationName = 6,
    ReservedFirst = 7,
    ReservedLast = 127, // 0x7F
    PrivareFirst = 128, // 0x80
    PrivateLast = 255, // 0xFF
  }
}
