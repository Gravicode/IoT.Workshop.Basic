// Decompiled with JetBrains decompiler
// Type: nanoFramework.Modbus.Interface.ModbusTcpListener
// Assembly: nanoFramework.Modbus, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B923EC90-60E3-4FDC-BF8E-BE964652F39A
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFramework.Modbus.dll

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace nanoFramework.Modbus.Interface
{
  public class ModbusTcpListener : IDisposable
  {
    private readonly ModbusDevice device;
    private readonly short maxDataLength;
    private readonly Socket listenSocket;
    private Thread listenThread;

    public ModbusTcpListener(
      ModbusDevice device,
      int port,
      int maxConnections,
      short maxDataLength)
    {
      this.device = device;
      this.maxDataLength = maxDataLength;
      this.listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      this.listenSocket.Bind((EndPoint) new IPEndPoint(IPAddress.Any, port));
      this.listenSocket.Listen(1);
      this.listenThread = new Thread(new ThreadStart(this.ListenProc));
      this.listenThread.Start();
    }

    private void ListenProc()
    {
      while (this.listenThread != null)
      {
        try
        {
          this.device.AddInterface((IModbusInterface) new ModbusTcpInterface(this.listenSocket.Accept(), this.maxDataLength, true));
        }
        catch
        {
        }
      }
    }

    public void Dispose()
    {
      Thread listenThread = this.listenThread;
      this.listenThread = (Thread) null;
      if (this.listenSocket != null)
      {
        try
        {
          this.listenSocket.Close();
        }
        catch
        {
        }
      }
      if(listenThread!=null)
          listenThread.Join(5000);
    }
  }
}
