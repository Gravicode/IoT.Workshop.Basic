using System;

namespace Sitronix.ST7735
{
    public class BitConverterHelper
    {
        //check if this method is correct or no
        public static void SwapEndianness(byte[] data, int groupsize)
        {
            for(int i = 0; i < groupsize; i++)
            {
                data[i] = Swap(data[i]);
            }
        }
        static byte Swap(byte x)
        {
            return (byte)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }

    }
}
