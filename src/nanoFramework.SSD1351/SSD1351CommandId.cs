// Decompiled with JetBrains decompiler
// Type: nanoFrameworkSSD1351.SSD1351CommandId
// Assembly: nanoFrameworkSSD1351, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C46C74DA-2DAB-4500-BB44-2EC6E1E34CF2
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFrameworkSSD1351.dll

namespace nanoFrameworkSSD1351
{
  public enum SSD1351CommandId : byte
  {
    NOP = 0,
    SETCOLUMN = 21, // 0x15
    WRITERAM = 92, // 0x5C
    READRAM = 93, // 0x5D
    SETROW = 117, // 0x75
    HORIZSCROLL = 150, // 0x96
    STOPSCROLL = 158, // 0x9E
    STARTSCROLL = 159, // 0x9F
    SETREMAP = 160, // 0xA0
    STARTLINE = 161, // 0xA1
    DISPLAYOFFSET = 162, // 0xA2
    DISPLAYALLOFF = 164, // 0xA4
    DISPLAYALLON = 165, // 0xA5
    NORMALDISPLAY = 166, // 0xA6
    INVERTDISPLAY = 167, // 0xA7
    FUNCTIONSELECT = 171, // 0xAB
    DISPLAYOFF = 174, // 0xAE
    DISPLAYON = 175, // 0xAF
    PRECHARGE = 177, // 0xB1
    DISPLAYENHANCE = 178, // 0xB2
    CLOCKDIV = 179, // 0xB3
    SETVSL = 180, // 0xB4
    SETGPIO = 181, // 0xB5
    PRECHARGE2 = 182, // 0xB6
    SETGRAY = 184, // 0xB8
    USELUT = 185, // 0xB9
    PRECHARGELEVEL = 187, // 0xBB
    VCOMH = 190, // 0xBE
    CONTRASTABC = 193, // 0xC1
    CONTRASTMASTER = 199, // 0xC7
    MUXRATIO = 202, // 0xCA
    COMMANDLOCK = 253, // 0xFD
  }
}
