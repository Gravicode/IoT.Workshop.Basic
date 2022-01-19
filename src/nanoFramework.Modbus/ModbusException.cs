// Decompiled with JetBrains decompiler
// Type: nanoFramework.Modbus.ModbusException
// Assembly: nanoFramework.Modbus, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B923EC90-60E3-4FDC-BF8E-BE964652F39A
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFramework.Modbus.dll

using System;
using System.Diagnostics;

namespace nanoFramework.Modbus
{
  [DebuggerDisplay("ErrorCode = {ErrorCode}")]
  public class ModbusException : Exception
  {
    public ModbusException(string message)
      : base(message)
    {
      this.ErrorCode = ModbusErrorCode.Unspecified;
    }

    public ModbusException(ModbusErrorCode errorCode) => this.ErrorCode = errorCode;

    public ModbusErrorCode ErrorCode { get; private set; }
  }
}
