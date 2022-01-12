// Decompiled with JetBrains decompiler
// Type: Drivers.Vnc.VncHost
// Assembly: Drivers.Vnc, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9F530C3B-A831-444F-9A20-B91A45E3436E
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\Drivers.Vnc.dll

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Drivers.Vnc
{
  internal class VncHost
  {
    protected int verMajor = 3;
    protected int verMinor = 8;
    public string cutText;
    protected Socket localClient;
    protected Socket serverSocket;
    protected NetworkStream stream;
    protected StreamReader reader;
    protected StreamWriter writer;
    internal bool hostRunning;
    private RawRectangle encodedRectangle;

    internal event VncHost.PointerChangedEventHandler PointerChangedEvent;

    internal event VncHost.KeyChangedEventHandler KeyChangedEvent;

    internal event VncHost.FrameSentEventHandler FrameSentEvent;

    internal event VncHost.ClientRequestUpdateEventHandler ClientRequestUpdateEvent;

    internal event VncHost.ConnectionChanged ConnectionChangedEvent;

    public bool Shared { get; set; }

    public int Port { get; set; }

    public uint[] Encodings { get; private set; }

    public VncHost.Encoding GetPreferredEncoding() => VncHost.Encoding.RawEncoding;

    public VncHost(int port) => this.Port = port;

    public float ServerVersion => (float) this.verMajor + (float) this.verMinor * 0.1f;

    public void Start()
    {
      this.hostRunning = true;
      try
      {
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, this.Port);
        this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        this.serverSocket.Bind((EndPoint) ipEndPoint);
        this.serverSocket.Listen(1);
      }
      catch (Exception ex)
      {
        return;
      }
      try
      {
        this.localClient = this.serverSocket.Accept();
        this.localClient.SendTimeout = 3000;
        IPAddress.Parse(((IPEndPoint) this.localClient.RemoteEndPoint).Address.ToString());
        this.stream = new NetworkStream(this.localClient, true);
        this.reader = new StreamReader((Stream) this.stream);
        this.writer = new StreamWriter((Stream) this.stream);
        VncHost.ConnectionChanged connectionChangedEvent = this.ConnectionChangedEvent;
        if (connectionChangedEvent == null)
          return;
        connectionChangedEvent(true);
      }
      catch (Exception ex)
      {
      }
    }

    public void ReadProtocolVersion()
    {
      try
      {
        byte[] numArray = this.reader.ReadBytes(12);
        if (numArray[0] != (byte) 82 || numArray[1] != (byte) 70 || numArray[2] != (byte) 66 || numArray[3] != (byte) 32 || numArray[4] != (byte) 48 || numArray[5] != (byte) 48 || numArray[6] != (byte) 51 || numArray[7] != (byte) 46 || numArray[8] != (byte) 48 && numArray[8] != (byte) 56 || numArray[9] != (byte) 48 && numArray[9] != (byte) 56 || numArray[10] != (byte) 51 && numArray[10] != (byte) 54 && numArray[10] != (byte) 55 && numArray[10] != (byte) 56 && numArray[10] != (byte) 57 || numArray[11] != (byte) 10)
          throw new NotSupportedException("Only versions 3.3, 3.7, and 3.8 of the RFB Protocol are supported.");
        this.verMajor = 3;
        switch (numArray[10])
        {
          case 51:
          case 54:
            this.verMinor = 3;
            break;
          case 55:
            this.verMinor = 7;
            break;
          case 56:
            this.verMinor = 8;
            break;
          case 57:
            this.verMinor = 8;
            break;
        }
      }
      catch (IOException ex)
      {
        this.Close();
      }
    }

    public void WriteProtocolVersion()
    {
      try
      {
        this.writer.Write(VncHost.GetBytes(string.Format("RFB 003.00{0}\n", new object[1]
        {
          (object) this.verMinor.ToString()
        })));
        this.writer.Flush();
      }
      catch (IOException ex)
      {
        this.Close();
      }
    }

    public void Close()
    {
      this.hostRunning = false;
      if (this.serverSocket != null)
      {
        this.serverSocket.Close();
        this.serverSocket = (Socket) null;
      }
      if (this.localClient != null)
      {
        this.localClient.Close();
        this.localClient = (Socket) null;
      }
      if (this.encodedRectangle == null)
        return;
      this.encodedRectangle.Dispose();
      this.encodedRectangle = (RawRectangle) null;
    }

    public bool WriteAuthentication(string password)
    {
      if (!string.IsNullOrEmpty(password))
        return false;
      if (this.verMinor == 3)
      {
        this.WriteUint32(1U);
      }
      else
      {
        byte[] numArray = new byte[1]{ (byte) 1 };
        this.writer.Write((byte) numArray.Length);
        for (int index = 0; index < numArray.Length; ++index)
          this.writer.Write(numArray[index]);
      }
      if (this.verMinor >= 7)
        this.reader.ReadByte();
      if (this.verMinor == 8)
        this.WriteSecurityResult(0U);
      return true;
    }

    public void WriteSecurityResult(uint sr) => this.writer.Write(sr);

    public bool ReadClientInit()
    {
      bool flag = false;
      try
      {
        this.Shared = this.reader.ReadByte() == 1;
        flag = this.Shared;
        return this.Shared;
      }
      catch (IOException ex)
      {
        this.Close();
      }
      return flag;
    }

    public void WriteServerInit(FrameBuffer fb)
    {
      try
      {
        this.writer.Write((ushort) fb.Width);
        this.writer.Write((ushort) fb.Height);
        this.writer.Write(fb.ToPixelFormat());
        this.writer.Write((uint) fb.ServerName.Length);
        this.writer.Write(VncHost.GetBytes(fb.ServerName));
        this.writer.Flush();
      }
      catch (IOException ex)
      {
        this.Close();
      }
    }

    public FrameBuffer ReadSetPixelFormat(int w, int h)
    {
      FrameBuffer frameBuffer = (FrameBuffer) null;
      try
      {
        this.ReadPadding(3);
        return FrameBuffer.FromPixelFormat(this.ReadBytes(16), w, h);
      }
      catch (IOException ex)
      {
        this.Close();
      }
      return frameBuffer;
    }

    public void ReadSetEncodings()
    {
      try
      {
        this.ReadPadding(1);
        ushort num = this.reader.ReadUInt16();
        uint[] numArray = new uint[(int) num];
        for (int index = 0; index < (int) num; ++index)
          numArray[index] = this.reader.ReadUInt32();
        this.Encodings = numArray;
      }
      catch (IOException ex)
      {
        this.Close();
      }
    }

    public void ReadFrameBufferUpdateRequest(FrameBuffer fb)
    {
      try
      {
        bool incremental = this.reader.ReadByte() != 0;
        ushort num1 = this.reader.ReadUInt16();
        ushort num2 = this.reader.ReadUInt16();
        ushort num3 = this.reader.ReadUInt16();
        ushort num4 = this.reader.ReadUInt16();
        this.DoFrameBufferUpdate(fb, incremental, (int) num1, (int) num2, (int) num3, (int) num4);
      }
      catch (IOException ex)
      {
        this.Close();
      }
    }

    private void DoFrameBufferUpdate(
      FrameBuffer fb,
      bool incremental,
      int x,
      int y,
      int width,
      int height)
    {
      int width1 = fb.Width;
      int height1 = fb.Height;
      if (x < 0 || y < 0 || width <= 0 || height <= 0 || x + width > width1 || y + height > height1)
        return;
      this.WriteFrameBufferUpdate(fb, incremental, x, y, width, height);
    }

    public void WriteFrameBufferUpdate(
      FrameBuffer fb,
      bool incremental,
      int x,
      int y,
      int width,
      int height)
    {
      if (!incremental)
      {
        VncHost.ClientRequestUpdateEventHandler requestUpdateEvent = this.ClientRequestUpdateEvent;
        if (requestUpdateEvent != null)
          requestUpdateEvent(x, y, width, height);
      }
      int num1 = 1;
      bool flag = false;
      if (fb.Data != null)
      {
        lock (fb)
        {
          if (this.encodedRectangle == null)
            this.encodedRectangle = new RawRectangle(fb);
          this.encodedRectangle.Encode();
          flag = true;
        }
      }
      byte[] buffer = new byte[16];
      buffer[0] = (byte) 0;
      buffer[1] = (byte) 0;
      buffer[2] = (byte) (num1 >> 8);
      buffer[3] = (byte) num1;
      if (flag)
      {
        buffer[4] = (byte) (this.encodedRectangle.X >> 8);
        buffer[5] = (byte) this.encodedRectangle.X;
        buffer[6] = (byte) (this.encodedRectangle.Y >> 8);
        buffer[7] = (byte) this.encodedRectangle.Y;
        buffer[8] = (byte) (this.encodedRectangle.Width >> 8);
        buffer[9] = (byte) this.encodedRectangle.Width;
        buffer[10] = (byte) (this.encodedRectangle.Height >> 8);
        buffer[11] = (byte) this.encodedRectangle.Height;
      }
      buffer[12] = (byte) 0;
      buffer[13] = (byte) 0;
      buffer[14] = (byte) 0;
      buffer[15] = (byte) 0;
      this.Write(buffer);
      if (flag)
      {
        byte[] data = this.encodedRectangle.Data;
        int num2 = 1024;
        int num3 = data.Length / num2;
        int offset = 0;
        int num4 = 0;
        for (; num3 > 0; --num3)
        {
          if (((Stream) this.reader).DataAvailable)
          {
            switch (this.ReadServerMessageType())
            {
              case VncHost.ClientMessages.KeyEvent:
                this.ReadKeyEvent();
                break;
              case VncHost.ClientMessages.PointerEvent:
                this.ReadPointerEvent();
                break;
              case VncHost.ClientMessages.ClientCutText:
                this.ReadClientCutText();
                break;
            }
          }
          int count = data.Length - num4 > num2 ? num2 : data.Length - num4;
          this.Write(data, offset, count);
          offset += count;
          num4 += count;
        }
      }
      fb.Data = (byte[]) null;
      VncHost.FrameSentEventHandler frameSentEvent = this.FrameSentEvent;
      if (frameSentEvent == null)
        return;
      frameSentEvent(true);
    }

    public void ReadKeyEvent()
    {
      try
      {
        bool pressed = this.reader.ReadByte() == 1;
        this.ReadPadding(2);
        uint key = this.reader.ReadUInt32();
        VncHost.KeyChangedEventHandler keyChangedEvent = this.KeyChangedEvent;
        if (keyChangedEvent == null)
          return;
        keyChangedEvent(key, pressed);
      }
      catch (IOException ex)
      {
        this.Close();
      }
    }

    public void ReadPointerEvent()
    {
      try
      {
        byte num1 = (byte) this.reader.ReadByte();
        ushort num2 = this.reader.ReadUInt16();
        ushort num3 = this.reader.ReadUInt16();
        VncHost.PointerChangedEventHandler pointerChangedEvent = this.PointerChangedEvent;
        if (pointerChangedEvent == null)
          return;
        pointerChangedEvent((int) num2, (int) num3, num1 > (byte) 0);
      }
      catch (IOException ex)
      {
        this.Close();
      }
    }

    public void ReadClientCutText()
    {
      try
      {
        this.ReadPadding(3);
        this.cutText = VncHost.GetString(this.reader.ReadBytes((int) this.reader.ReadUInt32()));
      }
      catch (IOException ex)
      {
        this.Close();
      }
    }

    public VncHost.ClientMessages ReadServerMessageType()
    {
      byte num = 0;
      try
      {
        return (VncHost.ClientMessages) this.reader.ReadByte();
      }
      catch (IOException ex)
      {
        this.Close();
      }
      return (VncHost.ClientMessages) num;
    }

    private void WriteServerMessageType(VncHost.ServerMessages message)
    {
      try
      {
        this.writer.Write((byte) message);
      }
      catch (IOException ex)
      {
        this.Close();
      }
    }

    public void WriteServerCutText(string text)
    {
      try
      {
        this.WriteServerMessageType(VncHost.ServerMessages.ServerCutText);
        this.WritePadding(3);
        this.writer.Write((uint) text.Length);
        this.writer.Write(VncHost.GetBytes(text));
        this.writer.Flush();
      }
      catch (IOException ex)
      {
        this.Close();
      }
    }

    public uint ReadUint32() => this.reader.ReadUInt32();

    public ushort ReadUInt16() => this.reader.ReadUInt16();

    public byte ReadByte() => (byte) this.reader.ReadByte();

    public byte[] ReadBytes(int count) => this.reader.ReadBytes(count);

    public void WriteUint32(uint value) => this.writer.Write(value);

    public void WriteUInt16(ushort value) => this.writer.Write(value);

    public void WriteUInt32(uint value) => this.writer.Write(value);

    public void WriteByte(byte value) => this.writer.Write(value);

    public void Write(byte[] buffer) => this.writer.Write(buffer);

    public void Write(byte[] buffer, int offset, int count) => this.writer.Write(buffer, offset, count);

    public void Flush() => this.writer.Flush();

    public void ReadPadding(int length) => this.ReadBytes(length);

    public void WritePadding(int length)
    {
      byte[] buffer = new byte[length];
      this.writer.Write(buffer, 0, buffer.Length);
    }

    protected static byte[] GetBytes(string text) => System.Text.Encoding.UTF8.GetBytes(text);

    protected static string GetString(byte[] bytes) => System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);

    internal delegate void PointerChangedEventHandler(int x, int y, bool pressed);

    internal delegate void KeyChangedEventHandler(uint key, bool pressed);

    internal delegate void FrameSentEventHandler(bool success);

    internal delegate void ClientRequestUpdateEventHandler(int x, int y, int width, int height);

    internal delegate void ConnectionChanged(bool connected);

    public enum Encoding
    {
      RawEncoding,
    }

    public enum ServerMessages
    {
      FramebufferUpdate,
      SetColorMapEntries,
      Bell,
      ServerCutText,
    }

    public enum ClientMessages : byte
    {
      SetPixelFormat,
      ReadColorMapEntries,
      SetEncodings,
      FramebufferUpdateRequest,
      KeyEvent,
      PointerEvent,
      ClientCutText,
    }
  }
}
