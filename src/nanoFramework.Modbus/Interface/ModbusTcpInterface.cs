// Decompiled with JetBrains decompiler
// Type: nanoFramework.Modbus.Interface.ModbusTcpInterface
// Assembly: nanoFramework.Modbus, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B923EC90-60E3-4FDC-BF8E-BE964652F39A
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFramework.Modbus.dll

using System;
using System.Net;
using System.Net.Sockets;

namespace nanoFramework.Modbus.Interface
{
  public class ModbusTcpInterface : IModbusInterface, IDisposable
  {
    private Socket socket;
    private bool ownsSocket;
    private ushort nextTransactionId;

    public ModbusTcpInterface(Socket socket, short maxDataLength = 252, bool ownsSocket = false)
    {
      this.socket = socket;
      this.ownsSocket = ownsSocket;
      this.MaxDataLength = maxDataLength;
      this.MaxTelegramLength = (short) ((int) maxDataLength + 8);
    }

    public ModbusTcpInterface(string host, int port = 502, short maxDataLength = 252)
    {
      IPAddress address;
      try
      {
        address = IPAddress.Parse(host);
      }
      catch
      {
        address = Dns.GetHostEntry(host).AddressList[0];
      }
      this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      try
      {
        this.socket.Connect((EndPoint) new IPEndPoint(address, port));
        this.ownsSocket = true;
      }
      catch
      {
        this.socket = (Socket) null;
        this.ownsSocket = false;
        throw;
      }
      this.MaxDataLength = maxDataLength;
      this.MaxTelegramLength = (short) ((int) maxDataLength + 8);
    }

    public static ModbusTcpListener StartDeviceListener(
      ModbusDevice device,
      int port = 502,
      int maxConnections = 5,
      short maxDataLength = 252)
    {
      return new ModbusTcpListener(device, port, maxConnections, maxDataLength);
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
      telegramLength = (short) (8 + (int) dataLength);
      if (isResponse)
      {
        if (telegramContext is ushort)
          ModbusUtils.InsertUShort(buffer, 0, (ushort) telegramContext);
        else
          ModbusUtils.InsertUShort(buffer, 0, (ushort) 0);
      }
      else
      {
        telegramContext = (object) this.nextTransactionId;
        if (this.nextTransactionId == ushort.MaxValue)
        {
          ModbusUtils.InsertUShort(buffer, 0, ushort.MaxValue);
          this.nextTransactionId = (ushort) 0;
        }
        else
          ModbusUtils.InsertUShort(buffer, 0, this.nextTransactionId++);
      }
      ModbusUtils.InsertUShort(buffer, 2, (ushort) 0);
      ModbusUtils.InsertUShort(buffer, 4, (ushort) ((uint) telegramLength - 6U));
      buffer[6] = addr;
      buffer[7] = fkt;
      dataPos = (short) 8;
    }

    public void PrepareWrite()
    {
    }

    public void PrepareRead()
    {
    }

    public void SendTelegram(byte[] buffer, short telegramLength)
    {
      if (this.socket == null)
        throw new ObjectDisposedException("ModbusTcp interface");
      this.socket.Send(buffer, 0, (int) telegramLength, SocketFlags.None);
    }

    public bool ReceiveTelegram(
      byte[] buffer,
      short desiredDataLength,
      int timeout,
      out short telegramLength)
    {
      if (this.socket == null)
        throw new ObjectDisposedException("ModbusTcp interface");
      if (!this.socket.Poll(timeout * 1000, SelectMode.SelectRead) || this.socket.Available == 0)
      {
        telegramLength = (short) 0;
        return false;
      }
      int offset = 0;
      while (offset < 6)
        offset += this.socket.Receive(buffer, offset, 6 - offset, SocketFlags.None);
      telegramLength = (short) ((int) ModbusUtils.ExtractUShort(buffer, 4) + 6);
      while (offset < (int) telegramLength)
        offset += this.socket.Receive(buffer, offset, (int) telegramLength - offset, SocketFlags.None);
      return true;
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
      if (telegramLength < (short) 8)
        throw new ModbusException(ModbusErrorCode.ResponseTooShort);
      if (isResponse)
      {
        if (telegramContext is ushort && (int) ModbusUtils.ExtractUShort(buffer, 0) != (int) (ushort) telegramContext)
        {
          address = (byte) 0;
          fkt = (byte) 0;
          dataPos = (short) 0;
          dataLength = (short) 0;
          return false;
        }
      }
      else
        telegramContext = (object) ModbusUtils.ExtractUShort(buffer, 0);
      address = buffer[6];
      fkt = buffer[7];
      dataPos = (short) 8;
      dataLength = (short) ((int) telegramLength - 8);
      return true;
    }

    public bool IsDataAvailable
    {
      get
      {
        try
        {
          return this.socket != null && this.socket.Available > 0;
        }
        catch
        {
          return false;
        }
      }
    }

    public void ClearInputBuffer()
    {
      byte[] buffer = new byte[1024];
      while (this.socket.Available > 0)
        this.socket.Receive(buffer, Math.Min(1024, this.socket.Available), SocketFlags.None);
    }

    public bool IsSocketConnected { get; set; }

    public bool IsConnectionOk
    {
      get
      {
        if (this.socket != null)
        {
          try
          {
            int num = this.socket.Poll(0, SelectMode.SelectError) ? 1 : 0;
            bool flag = this.socket.Poll(0, SelectMode.SelectRead);
            return num == 0 && (!flag || this.socket.Available > 0);
          }
          catch
          {
          }
        }
        return false;
      }
    }

    public void Dispose()
    {
      if (!this.ownsSocket)
        return;
      try
      {
        this.socket.Close();
      }
      catch
      {
      }
      this.socket = (Socket) null;
      this.ownsSocket = false;
    }
  }
}
