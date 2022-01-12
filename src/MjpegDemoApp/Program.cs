using nanoFramework.Media;
using nanoFramework.UI;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MjpegDemoApp
{
    public class Program
    {
        public static void Main()
        {
            var stream = new FileStream($@"A:\128x160.mjpeg", FileMode.Open);

            var settings = new Mjpeg.Setting();
            settings.BufferSize = 16 * 1024;
            settings.BufferCount = 3;

            var mjpegDecoder = new Mjpeg(settings);

            mjpegDecoder.FrameReceived += MjpegDecoder_FrameDecodedEvent;      

            mjpegDecoder.StartDecode(stream); // Non-block function

            Thread.Sleep(Timeout.Infinite);
           

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }

      

        private static void MjpegDecoder_FrameDecodedEvent(byte[] data)
        {
            using (var image = new Bitmap(data, Bitmap.BitmapImageType.Jpeg))
            {
                image.Flush();
                /*
                if (graphic != null)
                {
                    graphic.DrawImage(image, 0, 0, image.Width, image.Height);
                    graphic.Flush();
                }*/
            }
            GC.WaitForPendingFinalizers();// helps in clearing out the RAM
        }
    }
}
