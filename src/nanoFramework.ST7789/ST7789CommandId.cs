// Decompiled with JetBrains decompiler
// Type: nanoFramework.ST7789.ST7789CommandId
// Assembly: nanoFramework.ST7789, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4EDF34B8-FD3A-42B4-A2F9-6903BB949B07
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\nanoFramework.ST7789.dll

namespace nanoFramework.ST7789
{
  public enum ST7789CommandId : byte
  {
    NOP = 0,
    SWRESET = 1,
    RDDID = 4,
    RDDST = 9,
    RDDPM = 10, // 0x0A
    RDDMADCTL = 11, // 0x0B
    RDDCOLMOD = 12, // 0x0C
    RDDIM = 13, // 0x0D
    RDDSM = 14, // 0x0E
    SLPIN = 16, // 0x10
    SLPOUT = 17, // 0x11
    PTLON = 18, // 0x12
    NORON = 19, // 0x13
    INVOFF = 32, // 0x20
    INVON = 33, // 0x21
    GAMSET = 38, // 0x26
    DISPOFF = 40, // 0x28
    DISPON = 41, // 0x29
    CASET = 42, // 0x2A
    RASET = 43, // 0x2B
    RAMWR = 44, // 0x2C
    RAMRD = 46, // 0x2E
    PTLAR = 48, // 0x30
    TEOFF = 52, // 0x34
    TEON = 53, // 0x35
    MADCTL = 54, // 0x36
    IDMOFF = 56, // 0x38
    IDMON = 57, // 0x39
    COLMOD = 58, // 0x3A
    FRMCTR1 = 177, // 0xB1
    FRMCTR2 = 178, // 0xB2
    FRMCTR3 = 179, // 0xB3
    INVCTR = 180, // 0xB4
    DISSET5 = 182, // 0xB6
    PWCTR1 = 192, // 0xC0
    PWCTR2 = 193, // 0xC1
    PWCTR3 = 194, // 0xC2
    PWCTR4 = 195, // 0xC3
    PWCTR5 = 196, // 0xC4
    VMCTR1 = 197, // 0xC5
    VMOFCTR = 199, // 0xC7
    WRID2 = 209, // 0xD1
    WRID3 = 210, // 0xD2
    NVCTR1 = 217, // 0xD9
    RDID1 = 218, // 0xDA
    RDID2 = 219, // 0xDB
    RDID3 = 220, // 0xDC
    NVCTR2 = 222, // 0xDE
    NVCTR3 = 223, // 0xDF
    GAMCTRP1 = 224, // 0xE0
    GAMCTRN1 = 225, // 0xE1
  }
}
