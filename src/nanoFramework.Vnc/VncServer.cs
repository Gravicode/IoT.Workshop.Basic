// Decompiled with JetBrains decompiler
// Type: Drivers.Vnc.VncServer
// Assembly: Drivers.Vnc, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9F530C3B-A831-444F-9A20-B91A45E3436E
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\Drivers.Vnc.dll

using System;
using System.Threading;

namespace Drivers.Vnc
{
  public class VncServer
  {
    private VncHost host;
    private readonly FrameBuffer frameBuffer;
    private bool serverRunning;

    public event VncServer.PointerChangedEventHandler PointerChangedEvent;

    public event VncServer.KeyChangedEventHandler KeyChangedEvent;

    public event VncServer.FrameSentEventHandler FrameSentEvent;

    public event VncServer.ClientRequestUpdateEventHandler ClientRequestUpdateEvent;

    public event VncServer.ConnectionChanged ConnectionChangedEvent;

    public int Port { get; private set; }

    internal string Password { get; private set; }

    public string ServerName { get; private set; } = "Default";

    public TimeSpan DelayBetweenFrame { get; set; } = TimeSpan.FromMilliseconds(10);

    public bool Connected { get; private set; }

    private int Width { get; set; }

    private int Height { get; set; }

    public VncServer(string serverName, int port, int width, int height)
    {
      this.Password = (string) null;
      this.Port = port;
      this.ServerName = serverName;
      this.frameBuffer = new FrameBuffer(width, height)
      {
        BitsPerPixel = 16,
        Depth = 16,
        BigEndian = false,
        TrueColor = false,
        RedShift = 11,
        GreenShift = 5,
        BlueShift = 0,
        BlueMax = 31,
        GreenMax = 63,
        RedMax = 31,
        ServerName = string.IsNullOrEmpty(this.ServerName) ? "Default" : this.ServerName
      };
      if (string.IsNullOrEmpty(this.ServerName))
        throw new ArgumentNullException("Name", "The VNC Server Name cannot be empty.");
      this.host = this.Port != 0 ? new VncHost(this.Port) : throw new ArgumentNullException(nameof (Port), "The VNC Server port cannot be zero.");
      this.host.KeyChangedEvent += (VncHost.KeyChangedEventHandler) ((a, b) =>
      {
        VncServer.KeyChangedEventHandler keyChangedEvent = this.KeyChangedEvent;
        if (keyChangedEvent == null)
          return;
        keyChangedEvent(a, b);
      });
      this.host.PointerChangedEvent += (VncHost.PointerChangedEventHandler) ((a, b, c) =>
      {
        VncServer.PointerChangedEventHandler pointerChangedEvent = this.PointerChangedEvent;
        if (pointerChangedEvent == null)
          return;
        pointerChangedEvent(a, b, c);
      });
      this.host.FrameSentEvent += (VncHost.FrameSentEventHandler) (a =>
      {
        VncServer.FrameSentEventHandler frameSentEvent = this.FrameSentEvent;
        if (frameSentEvent == null)
          return;
        frameSentEvent(a);
      });
      this.host.ClientRequestUpdateEvent += (VncHost.ClientRequestUpdateEventHandler) ((x, y, w, h) =>
      {
        VncServer.ClientRequestUpdateEventHandler requestUpdateEvent = this.ClientRequestUpdateEvent;
        if (requestUpdateEvent == null)
          return;
        requestUpdateEvent(x, y, w, h);
      });
      this.host.ConnectionChangedEvent += (VncHost.ConnectionChanged) (a =>
      {
        VncServer.ConnectionChanged connectionChangedEvent = this.ConnectionChangedEvent;
        if (connectionChangedEvent == null)
          return;
        connectionChangedEvent(a);
      });
      this.Width = width;
      this.Height = height;
    }

    private void Run()
    {
      while (this.serverRunning)
      {
        try
        {
          this.host.Start();
          this.host.WriteProtocolVersion();
          this.host.ReadProtocolVersion();
          if (!this.host.WriteAuthentication(this.Password))
          {
            this.host.Close();
          }
          else
          {
            if (!this.host.ReadClientInit())
            {
              this.host.Close();
              throw new InvalidOperationException("Read client init failed.");
            }
            this.host.WriteServerInit(this.frameBuffer);
            this.Connected = true;
            while (this.host.hostRunning)
            {
              DateTime now = DateTime.UtcNow;
              Thread.Sleep(1);
              switch (this.host.ReadServerMessageType())
              {
                case VncHost.ClientMessages.SetPixelFormat:
                  FrameBuffer frameBuffer = this.host.ReadSetPixelFormat(this.frameBuffer.Width, this.frameBuffer.Height);
                  if (frameBuffer != null)
                  {
                    this.frameBuffer.BitsPerPixel = frameBuffer.BitsPerPixel;
                    this.frameBuffer.Depth = frameBuffer.Depth;
                    this.frameBuffer.BigEndian = frameBuffer.BigEndian;
                    this.frameBuffer.TrueColor = frameBuffer.TrueColor;
                    this.frameBuffer.RedMax = frameBuffer.RedMax;
                    this.frameBuffer.GreenMax = frameBuffer.GreenMax;
                    this.frameBuffer.BlueMax = frameBuffer.BlueMax;
                    this.frameBuffer.RedShift = frameBuffer.RedShift;
                    this.frameBuffer.GreenShift = frameBuffer.GreenShift;
                    this.frameBuffer.BlueShift = frameBuffer.BlueShift;
                    continue;
                  }
                  continue;
                case VncHost.ClientMessages.ReadColorMapEntries:
                  this.host.Close();
                  throw new NotSupportedException("Read ReadColorMapEntry");
                case VncHost.ClientMessages.SetEncodings:
                  this.host.ReadSetEncodings();
                  continue;
                case VncHost.ClientMessages.FramebufferUpdateRequest:
                  this.host.ReadFrameBufferUpdateRequest(this.frameBuffer);
                  double totalMilliseconds1 = (DateTime.Now - now).TotalMilliseconds;
                  double num = totalMilliseconds1;
                  TimeSpan delayBetweenFrame = this.DelayBetweenFrame;
                  double totalMilliseconds2 = delayBetweenFrame.TotalMilliseconds;
                  if (num < totalMilliseconds2)
                  {
                    delayBetweenFrame = this.DelayBetweenFrame;
                    Thread.Sleep((int) (delayBetweenFrame.TotalMilliseconds - totalMilliseconds1));
                    continue;
                  }
                  continue;
                case VncHost.ClientMessages.KeyEvent:
                  this.host.ReadKeyEvent();
                  continue;
                case VncHost.ClientMessages.PointerEvent:
                  this.host.ReadPointerEvent();
                  continue;
                case VncHost.ClientMessages.ClientCutText:
                  this.host.ReadClientCutText();
                  continue;
                default:
                  continue;
              }
            }
          }
        }
        catch
        {
          this.Connected = false;
          this.host.Close();
        }
        Thread.Sleep(1);
      }
    }

    public void Start()
    {
      this.Connected = false;
      this.serverRunning = true;
      new Thread(new ThreadStart(this.Run)).Start();
    }

    public void Stop()
    {
      this.Connected = false;
      this.serverRunning = false;
      this.host.Close();
    }

    public void Send(byte[] data, int x, int y, int width, int height)
    {
      if (this.frameBuffer == null)
        return;
      if (x != 0 || y != 0 || x + width != this.Width || y + height != this.Height)
        throw new ArgumentException("Only full screen update is supported");
      this.frameBuffer.Data = data;
      this.frameBuffer.X = x;
      this.frameBuffer.Y = y;
      this.frameBuffer.Width = width;
      this.frameBuffer.Height = height;
    }

    public delegate void PointerChangedEventHandler(int x, int y, bool pressed);

    public delegate void KeyChangedEventHandler(uint key, bool pressed);

    public delegate void FrameSentEventHandler(bool success);

    public delegate void ClientRequestUpdateEventHandler(int x, int y, int width, int height);

    public delegate void ConnectionChanged(bool connected);
  }
}
