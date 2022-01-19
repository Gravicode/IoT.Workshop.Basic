// Decompiled with JetBrains decompiler
// Type: nanoFramework.Modbus.ModbusUtils
// Assembly: nanoFramework.Modbus, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B923EC90-60E3-4FDC-BF8E-BE964652F39A
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFramework.Modbus.dll

namespace nanoFramework.Modbus
{
  public static class ModbusUtils
  {
    public static void InsertUShort(byte[] buffer, int pos, ushort value)
    {
      buffer[pos] = (byte) (((int) value & 65280) >> 8);
      buffer[pos + 1] = (byte) ((uint) value & (uint) byte.MaxValue);
    }

    public static ushort ExtractUShort(byte[] buffer, int pos) => (ushort) (((uint) buffer[pos] << 8) + (uint) buffer[pos + 1]);

    public static ushort CalcCrc(byte[] buffer, int count)
    {
      ushort num1 = ushort.MaxValue;
      for (int index1 = 0; index1 < count; ++index1)
      {
        num1 ^= (ushort) buffer[index1];
        for (int index2 = 0; index2 < 8; ++index2)
        {
          int num2 = ((uint) num1 & 1U) > 0U ? 1 : 0;
          num1 = (ushort) ((int) num1 >> 1 & (int) short.MaxValue);
          if (num2 != 0)
            num1 ^= (ushort) 40961;
        }
      }
      return num1;
    }
  }
}
