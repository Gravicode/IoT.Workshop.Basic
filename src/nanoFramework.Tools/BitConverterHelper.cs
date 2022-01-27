using System;

namespace nanoFramework.Tools
{
    public class BitConverterHelper
    {
        /*
        public static byte[] GetBytes(uint value)
        {
            return new byte[4] {
                    (byte)(value & 0xFF),
                    (byte)((value >> 8) & 0xFF),
                    (byte)((value >> 16) & 0xFF),
                    (byte)((value >> 24) & 0xFF) };
        }

        public static unsafe byte[] GetBytes(float value)
        {
            uint val = *((uint*)&value);
            return GetBytes(val);
        }

        public static unsafe byte[] GetBytes(float value, ByteOrder order)
        {
            byte[] bytes = GetBytes(value);
            if (order != ByteOrder.LittleEndian)
            {
                ReverseArray(bytes);
            }
            return bytes;
        }

        public static uint ToUInt32(byte[] value, int index)
        {
            return (uint)(
                value[0 + index] << 0 |
                value[1 + index] << 8 |
                value[2 + index] << 16 |
                value[3 + index] << 24);
        }

        public static unsafe float ToSingle(byte[] value, int index)
        {
            uint i = ToUInt32(value, index);
            return *(((float*)&i));
        }

        public static unsafe float ToSingle(byte[] value, int index, ByteOrder order)
        {
            if (order != ByteOrder.LittleEndian)
            {
                ReverseArray(value, index, value.Length);
            }
            return ToSingle(value, index);
        }

        public enum ByteOrder
        {
            LittleEndian,
            BigEndian
        }

        static public bool IsLittleEndian
        {
            get
            {
                unsafe
                {
                    int i = 1;
                    char* p = (char*)&i;

                    return (p[0] == 1);
                }
            }
        }

        static void ReverseArray(byte[] arr)
        {
            for (int i = 0; i < arr.Length / 2; i++)
            {
                var tmp = arr[i];
                arr[i] = arr[arr.Length - i - 1];
                arr[arr.Length - i - 1] = tmp;
            }
        }
        static void ReverseArray(byte[] arr,int i,int j)
        {
            //int i = 0;
            //int j = arr.Length - 1;
            while (i < j)
            {
                var temp = arr[i];
                arr[i] = arr[j];
                arr[j] = temp;
                i++;
                j--;
            }

        }*/
        /*
        public static void SwapEndianness(byte[] data, int groupsize)
        {
            //FlipInt32(data);
        }
        */
        //check if this method is correct or no
        public static void SwapEndianness(byte[] data, int groupsize)
        {
            for (int i = 0; i < groupsize; i++)
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
        /*
        static void Swap(ref byte a, ref byte b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        static byte[] FlipInt16(byte[] rawData)
        {
            for (var i = 0; i < rawData.Length; i += 2) // Step two for 2x8 bits=16
                Swap(ref rawData[i], ref rawData[i + 1]);
            return rawData;
        }

        static byte[] FlipInt32(byte[] rawData)
        {
            for (var i = 0; i < rawData.Length; i += 4)
            {// Step four for 4x8 bits=32
                Swap(ref rawData[i + 0], ref rawData[i + 2]);
                Swap(ref rawData[i + 1], ref rawData[i + 3]);
            }
           
            return rawData;
        }
        */

    }
}
