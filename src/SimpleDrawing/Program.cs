using BMC.Drivers.BasicGraphics;
using nanoFramework.UI;
using System;
using System.Diagnostics;
using System.Threading;

namespace SimpleDrawing
{
    public class Program
    {
        public static void Main()
        {
            var basicGfx = new BasicGraphicsImp(320,240);
            var colorBlue = BasicGraphics.ColorFromRgb(0, 0, 255);
            var colorGreen = BasicGraphics.ColorFromRgb(0, 255, 0);
            var colorRed = BasicGraphics.ColorFromRgb(255, 0, 0);
            var colorWhite = BasicGraphics.ColorFromRgb(255, 255, 255);

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
            

            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
    }

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
        private BasicGraphicsImp Screen { get; set; }

        public BouncingBalls(BasicGraphicsImp fullScreen)
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
                    X = rand.Next(0, Screen.Width - 2 * width),
                    Y = rand.Next(0, Screen.Height - 2 * width),
                    Width = width,
                    Height = width
                };
                // Setup 1/2 the balls with different speeds
                if (iBall % 2 == 0)
                {
                    vx = rand.Next(2, 10);
                    vy = rand.Next(2, 10);
                }
                else
                {
                    vx = rand.Next(12, 25);
                    vy = rand.Next(12, 25);
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
                Screen.DrawCircle((uint)nanoFramework.Presentation.Media.Color.Yellow, BallLocation[i].X, BallLocation[i].Y, BallLocation[i].Height/2);
            }
            Screen.Flush();
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
