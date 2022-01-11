using nanoFramework.UI;
using System;
using System.Text;

namespace Glide
{
    internal partial class Resources
    {
        internal static Bitmap GetBitmap(Resources.BinaryResources id)
        {
            var bytes = Resources.GetBytes(id);
            var bmp = new Bitmap(bytes, Bitmap.BitmapImageType.Gif);
            return bmp;
        }
    }
}