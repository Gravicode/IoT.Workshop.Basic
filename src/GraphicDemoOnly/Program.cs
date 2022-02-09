using BMC.Drivers.BasicGraphics;
using nanoFramework.Hardware.Esp32;
using nanoFramework.UI;
using System;
using System.Diagnostics;
using System.Threading;

namespace GraphicDemoOnly
{
    public class Program
    {
        public static void Main()
        {
            //demo using internal graphic controller
            DemoBasic();
            Thread.Sleep(Timeout.Infinite);

            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }

        static void DemoBasic()
        {

            int backLightPin = 18;
            int chipSelect = 13;
            int dataCommand = 12;
            int reset = 14;
            // Add the nanoFramework.Hardware.Esp32 to the solution
            Configuration.SetPinFunction(25, DeviceFunction.SPI1_MISO);
            Configuration.SetPinFunction(23, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(19, DeviceFunction.SPI1_CLOCK);
            // Adjust as well the size of your screen and the position of the screen on the driver
            DisplayControl.Initialize(new SpiConfiguration(1, chipSelect, dataCommand, reset, backLightPin), new ScreenConfiguration(0, 0, 160, 128));
            // Depending on you ESP32, you may also have to use either PWM either GPIO to set the backlight pin mode on
            // GpioController.OpenPin(backLightPin, PinMode.Output);
            // GpioController.Write(backLightPin, PinValue.High);

            var basicGfx = new BasicGraphicsImp(160, 128);
            var colorBlue = BasicGraphics.ColorFromRgb(0, 0, 255);
            var colorGreen = BasicGraphics.ColorFromRgb(0, 255, 0);
            var colorRed = BasicGraphics.ColorFromRgb(255, 0, 0);
            //var colorWhite = BasicGraphics.ColorFromRgb(255, 255, 255);

            basicGfx.Clear();
            basicGfx.DrawString("NanoFramework Kick Ass!", colorGreen, 15, 15, 1, 1);
            basicGfx.DrawString("BMC Training", colorBlue, 35, 40, 1, 1);
            basicGfx.DrawString("ESP32 - STM32F4", colorRed, 35, 60, 1, 1);


            //Random color = new Random();
            //for (var i = 1; i < 100; i++)
            //    basicGfx.DrawCircle((uint)color.Next(), i, 80, 5);

            basicGfx.Flush();

            Thread.Sleep(3000);
            //bounching balls demo
            //var balls = new BouncingBalls(basicGfx);
            //Thread.Sleep(500);

        }
    }

    public class BasicGraphicsImp : BasicGraphics, IDisposable
    {
        private Bitmap screen;
        public BasicGraphicsImp(int width, int height)
        {
            screen = new Bitmap(width, height);
            screen.Clear();
            this.Height = height;
            this.Width = width;

        }
        public override void SetPixel(int x, int y, uint color)
        {
            screen.SetPixel(x, y, (nanoFramework.Presentation.Media.Color)color);
            // add code to buffer pixels or send directly to display
        }
        public override void Clear()
        {
            screen.Clear();
            // add optional clear if buffer is used
        }
        // You may need to add this to send an optional buffer...
        public void Flush()
        {

            screen.Flush();
        }

        public void Dispose()
        {
            screen.Dispose();
        }
    }
} 

