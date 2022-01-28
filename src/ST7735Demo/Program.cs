using BMC.Drivers.BasicGraphics;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Gpio;
using System.Device.Spi;
using System.Diagnostics;
using System.Threading;

namespace ST7735Demo
{
    public class Program
    {
        public static void Main()
        {
            //Debug.WriteLine("Hello from nanoFramework!");
            DemoST7735_2();
            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
        static void DemoST7735()
        {
            Configuration.SetPinFunction(nanoFramework.Hardware.Esp32.Gpio.IO23, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(nanoFramework.Hardware.Esp32.Gpio.IO18, DeviceFunction.SPI1_CLOCK);
            Configuration.SetPinFunction(nanoFramework.Hardware.Esp32.Gpio.IO19, DeviceFunction.SPI1_MISO);
            //pin esp32
            //cs = 13, control = 12, reset = 14
            var basicGfx = new ST7735Imp(nanoFramework.Hardware.Esp32.Gpio.IO13, nanoFramework.Hardware.Esp32.Gpio.IO12, nanoFramework.Hardware.Esp32.Gpio.IO14, 160, 128);
            var colorBlue = BasicGraphics.ColorFromRgb(0, 0, 255);
            var colorGreen = BasicGraphics.ColorFromRgb(0, 255, 0);
            var colorRed = BasicGraphics.ColorFromRgb(255, 0, 0);
            //var colorWhite = BasicGraphics.ColorFromRgb(255, 255, 255);

            basicGfx.Clear();
            basicGfx.DrawString("NanoFramework Kick Ass!", colorGreen, 15, 15, 1, 1);
            basicGfx.DrawString("BMC Training", colorBlue, 35, 40, 1, 1);
            basicGfx.DrawString("ESP32 - STM32F4", colorRed, 35, 60, 1, 1);
            /*
            Random color = new Random();
            for (var i = 1; i < 100; i++)
                basicGfx.DrawCircle((uint)color.Next(), i, 80, 5);
            */
            //basicGfx.Flush();

            //Thread.Sleep(3000);
            //bounching balls demo
            //var balls = new BouncingBalls(basicGfx);
            //Thread.Sleep(500);

        }

        static void DemoST7735_2()
        {
            Configuration.SetPinFunction(nanoFramework.Hardware.Esp32.Gpio.IO23, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(nanoFramework.Hardware.Esp32.Gpio.IO18, DeviceFunction.SPI1_CLOCK);
            Configuration.SetPinFunction(nanoFramework.Hardware.Esp32.Gpio.IO19, DeviceFunction.SPI1_MISO);
            //pin esp32
            //cs = 13, control = 12, reset = 14
            var basicGfx = new ST7735Imp2(nanoFramework.Hardware.Esp32.Gpio.IO13, nanoFramework.Hardware.Esp32.Gpio.IO12, nanoFramework.Hardware.Esp32.Gpio.IO14);
         
        }

        public class ST7735Imp : BasicGraphics, IDisposable
        {

            //Sitronix.ST7735.ST7735Controller screen;
            ST7735 screen;
            public ST7735Imp(int CSPinNumber, int PinControl, int PinReset, uint ScreenWidth, uint ScreenHeight) : base(ScreenWidth, ScreenHeight, ColorFormat.Rgb565)
            {


                //var gpio = new GpioController();
                //var pinCS = gpio.OpenPin(CSPinNumber, PinMode.Output);
                //var pinControl = gpio.OpenPin(PinControl, PinMode.Output);
                //var pinReset = gpio.OpenPin(PinReset, PinMode.Output);

                //pinControl.Write(PinValue.Low);
                //pinCS.Write(PinValue.Low);

                //var spiConn = Sitronix.ST7735.ST7735Controller.GetConnectionSettings(PinValue.Low, CSPinNumber);
                var spiConn = ST7735.GetConnectionSettings(PinValue.Low, CSPinNumber);
                try
                {
                    Random rnd = new Random();
                    SpiDevice spi = new SpiDevice(spiConn);
                    //screen = new Sitronix.ST7735.ST7735Controller(spi, pinControl, pinReset, screenSize);
                    screen = new ST7735(CSPinNumber, PinControl, PinReset, spi);//, pinControl, pinReset, screenSize);
                   
                    //this.Width = screen.Width;
                    //this.Height = screen.Height;
                    //screen.SetDataAccessControl(true, true, false, false); //Rotate the screen.
                    //screen.SetDrawWindow(0, 0, Width, Height);
                    //screen.Enable();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            /*
            // You may need to add this to send an optional buffer...
            public void Flush()
            {

                screen.DrawBuffer(this.Buffer);
            }
            */
            public override void Clear()
            {
                screen.ClearScreen();
                // add optional clear if buffer is used
            }
            public override void SetPixel(int x, int y, uint color)
            {
                screen.DrawPixel(x, y, (ushort)color);
                // add code to buffer pixels or send directly to display
            }

            public void Dispose()
            {
                screen.Dispose();
            }
        }

        public class ST7735Imp2 : IDisposable
        {

            //Sitronix.ST7735.ST7735Controller screen;
            ST7735 screen;
            public ST7735Imp2(int CSPinNumber, int PinControl, int PinReset) 
            {
                var spiConn = ST7735.GetConnectionSettings(PinValue.Low, CSPinNumber);
                try
                {
                    Random rnd = new Random();
                    SpiDevice spi = SpiDevice.Create(spiConn);
                    screen = new ST7735(CSPinNumber, PinControl, PinReset, spi);

                    screen.ClearScreen();

                    screen.DrawCircle(50, 50, 10, 20);
                    screen.DrawLine(10, 80, 80, 80, 25);
                    screen.Refresh();
                    


                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            public void Dispose()
            {
                screen.Dispose();
            }
        }
    }
}
