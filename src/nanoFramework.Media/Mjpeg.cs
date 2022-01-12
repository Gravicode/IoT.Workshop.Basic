// Decompiled with JetBrains decompiler
// Type: nanoFramework.Media.Mjpeg
// Assembly: nanoFramework.Media, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 935D92BB-1807-4679-8C53-FF3D34F9DA49
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFramework.Media.dll

//using GHIElectronics.TinyCLR.Native;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace nanoFramework.Media
{
  public sealed class Mjpeg : IDisposable
  {
    private const int BLOCK_SIZE = 4096;
    private Stream stream;
    private int delayBetweenFrames;
    private Queue queue;
    private Mjpeg.Setting setting;
    private byte[][] buffer;
    private UnmanagedBuffer[] unmanagedBuffer;
    private uint currentBufferIdx;
    private Mjpeg.HeaderInfo headerInfo;

    public event Mjpeg.DataDecodedEventHandler FrameReceived;

    public bool IsDecoding { get; internal set; }

        public Mjpeg(Mjpeg.Setting setting)
        {

            this.currentBufferIdx = 0U;
            this.queue = new Queue();
            this.setting = setting;
            this.headerInfo = new Mjpeg.HeaderInfo();
            this.delayBetweenFrames = (int)setting.DelayBetweenFramesMilliseconds.TotalMilliseconds;
            this.unmanagedBuffer = new UnmanagedBuffer[this.setting.BufferCount];
            this.buffer = new byte[this.setting.BufferCount][];
            for (int index = 0; index < this.setting.BufferCount; ++index)
            {
                /*
                if (Memory.UnmanagedMemory.FreeBytes > (long) this.setting.BufferSize)
                {
                  this.unmanagedBuffer[index] = new UnmanagedBuffer(this.setting.BufferSize);
                  this.buffer[index] = this.unmanagedBuffer[index].Bytes;
                }
                else*/
                this.buffer[index] = new byte[this.setting.BufferSize];
            }
        }

    public void StartDecode(Stream stream)
    {
      this.stream = stream ?? throw new ArgumentNullException();
      this.IsDecoding = true;
      new Thread(new ThreadStart(this.Buffering)).Start();
      new Thread(new ThreadStart(this.Decoding)).Start();
    }

    public void StopDecode() => this.IsDecoding = false;

    private void Buffering()
    {
      long length = this.stream.Length;
      int num1 = 0;
      while ((long) num1 < length)
      {
        Thread.Sleep(1);
        if (!this.IsDecoding)
          break;
        lock (this.queue)
        {
          if (this.queue.Count == this.setting.BufferCount)
            continue;
        }
        int num2 = (long) this.setting.BufferSize < length - (long) num1 ? this.setting.BufferSize : (int) (length - (long) num1);
        int num3 = num2 / 4096;
        int count = num2 % 4096;
        int offset = 0;
        while (num3 > 0)
        {
          this.stream.Read(this.buffer[(int) this.currentBufferIdx], offset, 4096);
          offset += 4096;
          --num3;
          num1 += 4096;
          Thread.Sleep(1);
        }
        if (count > 0)
        {
          this.stream.Read(this.buffer[(int) this.currentBufferIdx], offset, count);
          num1 += count;
        }
        lock (this.queue)
          this.queue.Enqueue((object) this.buffer[(int) this.currentBufferIdx]);
        ++this.currentBufferIdx;
        if ((long) this.currentBufferIdx == (long) this.setting.BufferCount)
          this.currentBufferIdx = 0U;
      }
    }

    private void Decoding()
    {
      bool flag1 = true;
      while (this.IsDecoding)
      {
        Thread.Sleep(1);
        lock (this.queue)
        {
          if (this.queue.Count == 0)
            continue;
        }
        byte[] bytes;
        lock (this.queue)
          bytes = (byte[]) this.queue.Dequeue();
        for (int sourceIndex = 0; sourceIndex < this.setting.BufferSize - 4; ++sourceIndex)
        {
          DateTime now = DateTime.UtcNow;
          bool flag2 = false;
          if (flag1)
          {
            string str1 = Encoding.UTF8.GetString(bytes, 0, 4);
            string str2 = Encoding.UTF8.GetString(bytes, 8, 4);
            if (str1.CompareTo("RIFF") == 0 || str2.CompareTo("AVI ") == 0)
            {
              int index1 = 32;
              this.headerInfo.TimeBetweenFrames = ((int) bytes[index1] | (int) bytes[index1 + 1] << 8 | (int) bytes[index1 + 2] << 16 | (int) bytes[index1 + 3] << 24) / 1000;
              int index2 = index1 + 16;
              this.headerInfo.TotalFrames = (int) bytes[index2] | (int) bytes[index2 + 1] << 8 | (int) bytes[index2 + 2] << 16 | (int) bytes[index2 + 3] << 24;
              int index3 = index2 + 12;
              this.headerInfo.SuggestedBufferSize = (int) bytes[index3] | (int) bytes[index3 + 1] << 8 | (int) bytes[index3 + 2] << 16 | (int) bytes[index3 + 3] << 24;
              int index4 = index3 + 4;
              this.headerInfo.Width = (int) bytes[index4] | (int) bytes[index4 + 1] << 8 | (int) bytes[index4 + 2] << 16 | (int) bytes[index4 + 3] << 24;
              int index5 = index4 + 4;
              this.headerInfo.Height = (int) bytes[index5] | (int) bytes[index5 + 1] << 8 | (int) bytes[index5 + 2] << 16 | (int) bytes[index5 + 3] << 24;
              sourceIndex = index5 + 4;
            }
            flag1 = false;
          }
          if (bytes[sourceIndex] == (byte) 48 && bytes[sourceIndex + 1] == (byte) 48 && bytes[sourceIndex + 2] == (byte) 100 && bytes[sourceIndex + 3] == (byte) 99)
          {
            int index = sourceIndex + 4;
            int length = (int) bytes[index] | (int) bytes[index + 1] << 8 | (int) bytes[index + 2] << 16 | (int) bytes[index + 3] << 24;
            sourceIndex = index + 4;
            if (sourceIndex + length <= bytes.Length)
            {
              if (length > 0)
              {
                byte[] data = new byte[length];
                Array.Copy((Array) bytes, sourceIndex, (Array) data, 0, length);
                Mjpeg.DataDecodedEventHandler frameReceived = this.FrameReceived;
                if (frameReceived != null)
                  frameReceived(data);
                sourceIndex += length - 1;
                flag2 = true;
              }
            }
            else
              break;
          }
          if (flag2)
          {
            TimeSpan timeSpan = DateTime.UtcNow - now;
            if ((int) timeSpan.TotalMilliseconds < this.delayBetweenFrames)
              Thread.Sleep(this.delayBetweenFrames - (int) timeSpan.TotalMilliseconds);
          }
        }
      }
    }

    public void Dispose()
    {
      this.IsDecoding = false;
      this.queue.Clear();
      if (this.unmanagedBuffer == null)
        return;
      for (int index = 0; index < this.setting.BufferCount; ++index)
        this.unmanagedBuffer[index].Dispose();
    }

    public delegate void DataDecodedEventHandler(byte[] data);

    public class Setting
    {
      public int BufferSize { get; set; } = 2097152;

      public int BufferCount { get; set; } = 3;

      public TimeSpan DelayBetweenFramesMilliseconds { get; set; } = TimeSpan.FromMilliseconds(62);
    }

    private class HeaderInfo
    {
      public int TimeBetweenFrames { get; internal set; }

      public int Width { get; internal set; }

      public int Height { get; internal set; }

      public int SuggestedBufferSize { get; internal set; }

      public int TotalFrames { get; internal set; }
    }
  }
}
