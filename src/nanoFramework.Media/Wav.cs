// Decompiled with JetBrains decompiler
// Type: nanoFramework.Media.Wav
// Assembly: nanoFramework.Media, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 935D92BB-1807-4679-8C53-FF3D34F9DA49
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFramework.Media.dll

using System;

namespace nanoFramework.Media
{
  public sealed class Wav
  {
    private int index;
    private int dataSize;
    private int sampleRate;

    public Wav(byte[] wav)
    {
      this.index = 0;
      if (wav[this.index] != (byte) 82 || wav[this.index + 1] != (byte) 73 || wav[this.index + 2] != (byte) 70 || wav[this.index + 3] != (byte) 70)
        throw new Exception("File is not RIFF");
      this.index += 4;
      int uint32_1 = (int) BitConverter.ToUInt32(wav, this.index);
      this.index += 4;
      if (wav[this.index] != (byte) 87 || wav[this.index + 1] != (byte) 65 || wav[this.index + 2] != (byte) 86 || wav[this.index + 3] != (byte) 69)
        throw new Exception("File is not WAVE format");
      this.index += 4;
      if (wav[this.index] != (byte) 102 || wav[this.index + 1] != (byte) 109 || wav[this.index + 2] != (byte) 116 || wav[this.index + 3] != (byte) 32)
        throw new Exception("Unexpected fmt subchunk!");
      this.index += 4;
      uint uint32_2 = BitConverter.ToUInt32(wav, this.index);
      this.index += 4;
      bool flag;
      if (uint32_2 == 16U)
      {
        flag = true;
      }
      else
      {
        if (uint32_2 != 18U)
          throw new Exception("Invalid Subchunk1Size.");
        flag = false;
      }
      int uint16_1 = (int) BitConverter.ToUInt16(wav, this.index);
      this.index += 2;
      if (uint16_1 != 1)
        throw new Exception("AudioFormat invalid.");
      int uint16_2 = (int) BitConverter.ToUInt16(wav, this.index);
      this.index += 2;
      if (uint16_2 != 1)
        throw new Exception("Must be mono.");
      this.sampleRate = BitConverter.ToInt32(wav, this.index);
      this.index += 4;
      if (this.sampleRate != 8000)
        throw new Exception("Sample rate must be 8000KHz.");
      int uint16_3 = (int) BitConverter.ToUInt16(wav, this.index);
      this.index += 4;
      int uint16_4 = (int) BitConverter.ToUInt16(wav, this.index);
      this.index += 2;
      if (flag)
      {
        int uint16_5 = (int) BitConverter.ToUInt16(wav, this.index);
        this.index += 2;
        if (uint16_5 != 8)
          throw new Exception("Must be 8 bit.");
      }
      else
      {
        int uint32_3 = (int) BitConverter.ToUInt32(wav, this.index);
        this.index += 4;
        if (uint32_3 != 8)
          throw new Exception("Must be 8 bit.");
      }
      if (wav[this.index] != (byte) 100 || wav[this.index + 1] != (byte) 97 || wav[this.index + 2] != (byte) 116 || wav[this.index + 3] != (byte) 97)
        throw new Exception("Unexpected data subchunk!");
      this.index += 4;
      uint uint32_4 = BitConverter.ToUInt32(wav, this.index);
      this.index += 4;
      this.dataSize = (int) uint32_4;
    }

    public int GetDataIndex() => this.index;

    public int GetDataSize() => this.dataSize;

    public int GetSampleRate() => this.sampleRate;
  }
}
