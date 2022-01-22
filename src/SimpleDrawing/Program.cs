using BMC.Drivers.BasicGraphics;
using nanoFramework.SSD1306B;
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
            //DemoST7735();
            
            //demossd1306
            DemoSSD1306();

            Thread.Sleep(Timeout.Infinite);

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
            var balls = new BouncingBalls(basicGfx);
            Thread.Sleep(500);

        }*/
        static void DemoST7735()
        {
            //pin esp32
            //cs = 16, control = 17, reset = 23
            var basicGfx = new ST7735Imp(Sitronix.ST7735.ScreenSize._160x128,nanoFramework.Hardware.Esp32.Gpio.IO16, nanoFramework.Hardware.Esp32.Gpio.IO17, nanoFramework.Hardware.Esp32.Gpio.IO23);
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
            var balls = new BouncingBalls(basicGfx);
            Thread.Sleep(500);

        } 
        static void DemoSSD1306()
        {
            var basicGfx = new SSD1306Imp();
            var colorBlue = BasicGraphics.ColorFromRgb(0, 0, 255);
            var colorGreen = BasicGraphics.ColorFromRgb(0, 255, 0);
            var colorRed = BasicGraphics.ColorFromRgb(255, 0, 0);
            //var colorWhite = BasicGraphics.ColorFromRgb(255, 255, 255);

            basicGfx.Clear();
            basicGfx.DrawString("NanoFramework", colorGreen, 1, 10, 1, 1);
            basicGfx.DrawString("Kick Ass", colorBlue, 1, 20, 1, 1);
            basicGfx.DrawString("--BMC--", colorRed, 1, 30, 1, 1);

            Random color = new Random();
            for (var i = 10; i < 100; i++)
                basicGfx.DrawCircle((uint)color.Next(), i, 40, 2);

            basicGfx.Flush();

            Thread.Sleep(3000);
            //bounching balls demo
            var balls = new BouncingBalls(basicGfx);
            Thread.Sleep(500);

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
        public ST7735Imp(Sitronix.ST7735.ScreenSize screenSize, int CSPinNumber, int PinControl, int PinReset)
        {
            var gpio = new GpioController();
            var pinCS = gpio.OpenPin(CSPinNumber,PinMode.Output);
            var pinControl = gpio.OpenPin(PinControl,PinMode.Output);
            var pinReset = gpio.OpenPin(PinReset,PinMode.Output);

            var spiConn = Sitronix.ST7735.ST7735Controller.GetConnectionSettings(PinValue.High,pinCS);
            SpiDevice spi = new SpiDevice(spiConn);
            screen = new Sitronix.ST7735.ST7735Controller(spi, pinControl, pinReset, screenSize);
            this.Width = screen.Width;
            this.Height = screen.Height;
            screen.SetDataAccessControl(true, true, false, false); //Rotate the screen.
            screen.SetDrawWindow(0, 0, Width, Height);
            screen.Enable();
        }
       
        // You may need to add this to send an optional buffer...
        public void Flush()
        {
            screen.DrawBuffer(this.Buffer);
        }

        public void Dispose()
        {
            screen.Dispose();
        }
    }

    public class SSD1306Imp : BasicGraphics, IDisposable
    {

        SSD1306 screen;
        public SSD1306Imp():base(128,64,ColorFormat.OneBpp)
        {

            screen = new SSD1306("I2C1",128, 64, 0x3C);
            screen.SetEntireDisplayON(true);
            screen.Init();
            screen.Clear();
            //this.Width = screen.Width;
            //this.Height = screen.Height;
            
        }
        public override void SetPixel(int x, int y, uint color)
        {
            screen.DrawPixel(x,y,true);
            // add code to buffer pixels or send directly to display
        }

        // You may need to add this to send an optional buffer...
        public void Flush()
        {
            screen.Display();
            //do nothing
        }

        public void Dispose()
        {
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

        public BouncingBalls(BasicGraphics fullScreen)
        {
            Screen = fullScreen;
            SetupBalls();

            for (int iCount = 0; iCount < 180; iCount++)
            {
                MoveBalls();
                DrawBalls();
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
                int width = rand.Next(3, 50);
                BallLocation[iBall] = new Rectangle
                {
                    X = rand.Next(0, Screen.Width -  width),
                    Y = rand.Next(0, Screen.Height -  width),
                    Width = width,
                    Height = width
                };
                // Setup 1/2 the balls with different speeds
                if (iBall % 2 == 0)
                {
                    vx = rand.Next(1, 2);
                    vy = rand.Next(1, 2);
                }
                else
                {
                    vx = rand.Next(1, 2);
                    vy = rand.Next(1, 2);
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
                if (new_x < 0)
                {
                    BallVelocity[ball_num].X = -BallVelocity[ball_num].X;
                }
                else if (new_x + BallLocation[ball_num].Width > Screen.Width)
                {
                    BallVelocity[ball_num].X = -BallVelocity[ball_num].X;
                }
                if (new_y < 0)
                {
                    BallVelocity[ball_num].Y = -BallVelocity[ball_num].Y;
                }
                else if (new_y + BallLocation[ball_num].Height > Screen.Height)
                {
                    BallVelocity[ball_num].Y = -BallVelocity[ball_num].Y;
                }

                BallLocation[ball_num] = new Rectangle(new_x, new_y,
                                                       BallLocation[ball_num].Width,
                                                       BallLocation[ball_num].Height);
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
