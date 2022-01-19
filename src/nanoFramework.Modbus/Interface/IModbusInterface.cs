// Decompiled with JetBrains decompiler
// Type: nanoFramework.Modbus.Interface.IModbusInterface
// Assembly: nanoFramework.Modbus, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B923EC90-60E3-4FDC-BF8E-BE964652F39A
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFramework.Modbus.dll

namespace nanoFramework.Modbus.Interface
{
  public interface IModbusInterface
  {
    short MaxDataLength { get; }

    short MaxTelegramLength { get; }

    void CreateTelegram(
      byte addr,
      byte fkt,
      short dataLength,
      byte[] buffer,
      out short telegramLength,
      out short dataPos,
      bool isResponse,
      ref object telegramContext);

    void PrepareWrite();

    void PrepareRead();

    void SendTelegram(byte[] buffer, short telegramLength);

    bool ReceiveTelegram(
      byte[] buffer,
      short desiredDataLength,
      int timeout,
      out short telegramLength);

    bool ParseTelegram(
      byte[] buffer,
      short telegramLength,
      bool isResponse,
      ref object telegramContext,
      out byte address,
      out byte fkt,
      out short dataPos,
      out short dataLength);

    bool IsDataAvailable { get; }

    void ClearInputBuffer();

    bool IsConnectionOk { get; }
  }
}
