using BMC.Drivers.BasicGraphics;
using nanoFramework.Hardware.Esp32;
//using nanoFramework.SSD1306B;
using SolomonSystech.SSD1306;
//using nanoFramework.UI;
using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Device.Spi;
using System.Diagnostics;
using System.Threading;

namespace SimpleDrawing
{
    public class Program
    {
        public static void Main()
        {
            //demo using internal graphic controller
            //DemoBasic();
            
            //demo st7735
            DemoST7735();
            
            //demossd1306
            //DemoSSD1306();

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
        /*
        static void DemoBasic()
        {
            var basicGfx = new BasicGraphicsImp(320, 240);
            var colorBlue = BasicGraphics.ColorFromRgb(0, 0, 255);
            var colorGreen = BasicGraphics.ColorFromRgb(0, 255, 0);
            var colorRed = BasicGraphics.ColorFromRgb(255, 0, 0);
            //var colorWhite = BasicGraphics.ColorFromRgb(255, 255, 255);

            basicGfx.Clear();
            basicGfx.DrawString("NanoFramework Kick Ass!", colorGreen, 15, 15, 2, 1);
            basicGfx.DrawString("BMC Training", colorBlue, 35, 40, 2, 2);
            basicGfx.DrawString("ESP32 - STM32F4", colorRed, 35, 60, 2, 2);

            Random color = new Random();
            for (var i = 20; i < 140; i++)
                basicGfx.DrawCircle((uint)color.Next(), i, 100, 15);

            basicGfx.Flush();

            Thread.Sleep(3000);
            //bounching balls demo
            //var balls = new BouncingBalls(basicGfx);
            //Thread.Sleep(500);

        }*/
        static void DemoST7735()
        {
            Configuration.SetPinFunction(nanoFramework.Hardware.Esp32.Gpio.IO23, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(nanoFramework.Hardware.Esp32.Gpio.IO18, DeviceFunction.SPI1_CLOCK);
            Configuration.SetPinFunction(nanoFramework.Hardware.Esp32.Gpio.IO19, DeviceFunction.SPI1_MISO);
            //pin esp32
            //cs = 13, control = 12, reset = 14
            var basicGfx = new ST7735Imp(Sitronix.ST7735.ScreenSize._160x128,nanoFramework.Hardware.Esp32.Gpio.IO13, nanoFramework.Hardware.Esp32.Gpio.IO12, nanoFramework.Hardware.Esp32.Gpio.IO14,160,128);
            var colorBlue = BasicGraphics.ColorFromRgb(0, 0, 255);
            var colorGreen = BasicGraphics.ColorFromRgb(0, 255, 0);
            var colorRed = BasicGraphics.ColorFromRgb(255, 0, 0);
            //var colorWhite = BasicGraphics.ColorFromRgb(255, 255, 255);

            basicGfx.Clear();
            basicGfx.DrawString("NanoFramework Kick Ass!", colorGreen, 15, 15, 1, 1);
            basicGfx.DrawString("BMC Training", colorBlue, 35, 40, 1, 1);
            basicGfx.DrawString("ESP32 - STM32F4", colorRed, 35, 60, 1, 1);

            Random color = new Random();
            for (var i = 1; i < 100; i++)
                basicGfx.DrawCircle((uint)color.Next(), i, 80, 5);

            //basicGfx.Flush();

            Thread.Sleep(3000);
            //bounching balls demo
            var balls = new BouncingBalls(basicGfx);
            Thread.Sleep(500);

        } 
        static void DemoSSD1306()
        {
            //for ESP 32
            Configuration.SetPinFunction(nanoFramework.Hardware.Esp32.Gpio.IO22, DeviceFunction.I2C1_CLOCK);
            Configuration.SetPinFunction(nanoFramework.Hardware.Esp32.Gpio.IO21, DeviceFunction.I2C1_DATA);

            var basicGfx = new SSD1306Imp();
            var colorBlue = BasicGraphics.ColorFromRgb(0, 0, 255);
            var colorGreen = BasicGraphics.ColorFromRgb(0, 255, 0);
            var colorRed = BasicGraphics.ColorFromRgb(255, 0, 0);
            //var colorWhite = BasicGraphics.ColorFromRgb(255, 255, 255);

            basicGfx.Clear();
            basicGfx.DrawString("NanoFramework", colorGreen, 1, 1, 1, 1);
            basicGfx.DrawString("Kick Ass", colorBlue, 1, 20, 1, 1);
            basicGfx.DrawString("--BMC--", colorRed, 1, 40, 1, 1);

            Random color = new Random();
            for (var i = 10; i < 100; i++)
                basicGfx.DrawCircle((uint)color.Next(), i, 60, 2);

            basicGfx.Flush();

            Thread.Sleep(3000);
            //bounching balls demo
            var balls = new BouncingBalls(basicGfx);
            Thread.Sleep(Timeout.Infinite);

        }
    }
    
    /*
    public class BasicGraphicsImp : BasicGraphics, IDisposable
    {
        private Bitmap screen;
        public BasicGraphicsImp(int width,int height)
        {
            screen = new Bitmap(width,height);
            screen.Clear();
            this.Height = height;
            this.Width = width;
 
        }
        public override void SetPixel(int x, int y, uint color)
        {
            screen.SetPixel(x,y,(nanoFramework.Presentation.Media.Color)color);
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
    }*/
    public class ST7735Imp : BasicGraphics, IDisposable
    {
        
        Sitronix.ST7735.ST7735Controller screen;
        //ST7735 screen;
        public ST7735Imp(Sitronix.ST7735.ScreenSize screenSize, int CSPinNumber, int PinControl, int PinReset,uint ScreenWidth,uint ScreenHeight):base(ScreenWidth,ScreenHeight,ColorFormat.Rgb565)
        {


            var gpio = new GpioController();
            //var pinCS = gpio.OpenPin(CSPinNumber, PinMode.Output);
            var pinControl = gpio.OpenPin(PinControl, PinMode.Output);
            var pinReset = gpio.OpenPin(PinReset, PinMode.Output);

            //pinControl.Write(PinValue.Low);
            //pinCS.Write(PinValue.Low);

            var spiConn = Sitronix.ST7735.ST7735Controller.GetConnectionSettings(PinValue.Low, CSPinNumber);
            //var spiConn = ST7735.GetConnectionSettings(PinValue.Low, CSPinNumber);
            try
            {
                SpiDevice spi = new SpiDevice(spiConn);
                screen = new Sitronix.ST7735.ST7735Controller(spi, pinControl, pinReset, screenSize);
                //screen = new ST7735(CSPinNumber, PinControl, PinReset, spi);//, pinControl, pinReset, screenSize);
                //this.Width = screen.Width;
                //this.Height = screen.Height;
                screen.SetDataAccessControl(true, true, false, false); //Rotate the screen.
                screen.SetDrawWindow(0, 0, Width, Height);
                screen.Enable();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }


        // You may need to add this to send an optional buffer...
        public void Flush()
        {

            screen.DrawBuffer(this.Buffer);
        }
        /*
        public override void Clear()
        {
            screen.ClearScreen();
            // add optional clear if buffer is used
        }
        public override void SetPixel(int x, int y, uint color)
        {
            screen.DrawPixel(x, y, (ushort)color);
            // add code to buffer pixels or send directly to display
        }*/

        public void Dispose()
        {
            screen.Dispose();
        }
    }

    public class SSD1306Imp : BasicGraphics, IDisposable
    {

        //SSD1306 screen;
        SSD1306Controller screen;
        public SSD1306Imp():base(128,64,ColorFormat.OneBpp)
        {
            var con = SSD1306Controller.GetConnectionSettings();
            I2cDevice dev = new I2cDevice(con);
            screen = new SSD1306Controller(dev); //new SSD1306("I2C1",128, 64, 0x3C);
            //screen.SetEntireDisplayON(true);
            //screen.Init();
            //screen.Clear();
            //this.Width = screen.Width;
            //this.Height = screen.Height;
            
        }
        /*
        public override void Clear()
        {
            screen.Clear();
            // add optional clear if buffer is used
        }
        public override void SetPixel(int x, int y, uint color)
        {
            screen.DrawPixel(x,y,true);
            // add code to buffer pixels or send directly to display
        }*/

        // You may need to add this to send an optional buffer...
        public void Flush()
        {
            screen.DrawBufferNative(this.Buffer);
            //do nothing
        }

        public void Dispose()
        {
            screen.Dispose();
            //do nothing
        }
    }
    public class BouncingBalls
    {
        struct Rectangle
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
            public Rectangle(int x, int y, int width, int height)
            {
                this.X = x;
                this.Y = y;
                this.Width = width;
                this.Height = height;
            }
        }

        struct Point { public int X; public int Y; };
        private Rectangle[] BallLocation;
        private Point[] BallVelocity;
        private BasicGraphics Screen { get; set; }

        public BouncingBalls(SSD1306Imp fullScreen)
        {
            Screen = fullScreen;
            SetupBalls();
            looping = new Thread(new ThreadStart(Loop));
            looping.Start();
            
        }
        public BouncingBalls(ST7735Imp fullScreen)
        {
            Screen = fullScreen;
            SetupBalls();
            looping = new Thread(new ThreadStart(Loop));
            looping.Start();

        }

        Thread looping;
        void Loop()
        {
            while (true)
            {
                
                MoveBalls();
                DrawBalls();
                if (Screen is SimpleDrawing.SSD1306Imp)
                    ((SSD1306Imp)Screen).Flush();
                //else
                //    ((ST7735Imp)Screen).Flush();
                Thread.Sleep(1);
            }
        }

        private void SetupBalls()
        {
            Random rand = new Random();
            const int num_balls = 12;
            int vx = 0;
            int vy = 0;

            BallLocation = new Rectangle[num_balls];
            BallVelocity = new Point[num_balls];

            for (int iBall = 0; iBall < num_balls; iBall++)
            {
                int width = rand.Next(4, 10);
                BallLocation[iBall] = new Rectangle
                {
                    X = 1+rand.Next(Screen.Width-11),
                    Y = 1+rand.Next(Screen.Height-11),
                    Width = width,
                    Height = width
                };
                // Setup 1/2 the balls with different speeds
                
                if (iBall % 2 == 0)
                {
                    vx = rand.Next(1, 5);
                    vy = rand.Next(1, 5);
                }
                else
                {
                    vx = rand.Next(6, 10);
                    vy = rand.Next(6, 10);
                }
                
                // Setup random directions
                if (rand.Next(0, 2) == 0) vx = -vx;
                if (rand.Next(0, 2) == 0) vy = -vy;
                BallVelocity[iBall] = new Point { X = vx, Y = vy };
            }
        }

        private void MoveBalls()
        {
            for (int ball_num = 0;
                ball_num < BallLocation.Length;
                ball_num++)
            {
                // Move the ball.
                int new_x = BallLocation[ball_num].X +
                    BallVelocity[ball_num].X;
                int new_y = BallLocation[ball_num].Y +
                    BallVelocity[ball_num].Y;
                if (new_x <= 1)
                {
                    BallVelocity[ball_num].X = -BallVelocity[ball_num].X;
                }
                else if (new_x + BallLocation[ball_num].Width >= Screen.Width-1)
                {
                    BallVelocity[ball_num].X = -BallVelocity[ball_num].X;
                }
                if (new_y <= 1)
                {
                    BallVelocity[ball_num].Y = -BallVelocity[ball_num].Y;
                }
                else if (new_y + BallLocation[ball_num].Height >= Screen.Height-1)
                {
                    BallVelocity[ball_num].Y = -BallVelocity[ball_num].Y;
                }

                BallLocation[ball_num].X = new_x;
                BallLocation[ball_num].Y = new_y;
                /*
                = new Rectangle(new_x, new_y,
                                                       BallLocation[ball_num].Width,
                                                       BallLocation[ball_num].Height);
                */
            }
        }

        private void DrawBalls()
        {
            Screen.Clear();

            for (int i = 0; i < BallLocation.Length; i++)
            {
                //teal
                Screen.DrawCircle((uint)8421376, BallLocation[i].X, BallLocation[i].Y, BallLocation[i].Height/2);
            }
            //Screen.Flush();
        }
    }
    public static class Extensions
    {
        public static int Next(this Random rand, int min, int max)
        {
            if (max - min == 0)
                return min;
            return min + rand.Next(max - min);
        }
    }
}
