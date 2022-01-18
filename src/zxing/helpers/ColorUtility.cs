using System;
using System.Collections;
using nanoFramework.UI;
using System.Text;
using System.Threading;
using nanoFramework.Presentation.Media;

namespace ZXing {
    public static class ColorUtility {
        public static Color ColorFromRGB(byte r, byte g, byte b) {
            return Colors.FromArgb(((b << 16) | (g << 8) | r));
        }

        public static byte GetRValue(Color color) {
            return new Colors((long)color).R;
        }

        public static byte GetGValue(Color color) {
            return new Colors((long)color).G;
        }

        public static byte GetBValue(Color color) {
            return new Colors((long)color).B;
        }
    }
    /*
    public enum Color : uint {
        Black = 0x00000000,
        White = 0x00ffffff,
    }*/
}
