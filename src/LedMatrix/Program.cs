using BMC.Drivers.BasicGraphics;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using WS2812;

namespace LedMatrix
{
    public class Program
    {
        static LedMatrix screen;
        static Random rnd;
        const int cols = 32;
        const int rows = 8;
        static void Main()
        {
            rnd = new Random();
            var gpioController = new GpioController();

            var led = gpioController.OpenPin(nanoFramework.Hardware.Esp32.Gpio.IO12, PinMode.Output);
            screen = new LedMatrix(nanoFramework.Hardware.Esp32.Gpio.IO13, cols, rows);

            Thread th2 = new Thread(new ThreadStart(Animation));
            th2.Start();
            while (true)
            {
                led.Write(PinValue.High);
                Thread.Sleep(200);
                led.Write(PinValue.Low);
                Thread.Sleep(200);
            }
        }

        static void Animation()
        {
            string[] words = { "THIS", "IS", "A", "PLACE", "OF", "THE", "CHAMP" };
            string[] words2 = { "READ", "QURAN", "ALL", "THE", "TIME" };
            string[] words3 = { "SALAT", "ON", "TIME", };
            string[] words4 = { "GOD", "WILL", "LOVE", "YOU" };
            while (true)
            {
                CountDownAnimation(0, 100, 1);
                BrickAnimation();
                CharAnimation(words);
                LineAnimation();
                CharAnimation(words2);
                LineAnimation2();
                CharAnimation(words3);
                LineAnimation();
                CharAnimation(words4);
                LineAnimation2();
                BallAnimation(200);
            }
        }
        static void BrickAnimation(int Moves = 16 * 4, int Delay = 100)
        {
            screen.Clear();
            var col = LedMatrix.ColorFromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
            var MaxX = cols / 2;
            var MaxY = rows / 2;
            int x, y;
            for (int i = 0; i < Moves; i++)
            {
                x = rnd.Next(MaxX);
                y = rnd.Next(MaxY);
                screen.DrawRectangle(col, x * 2, y * 2, 2, 2);
                screen.Flush();
                Thread.Sleep(Delay);
                col = LedMatrix.ColorFromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));

            }
        }
        static void BallAnimation(int Moves = 1000, int Delay = 50)
        {
            var x = rnd.Next(cols);
            var y = rnd.Next(rows);
            var ax = 1 + rnd.Next(2);
            var ay = 1 + rnd.Next(2);
            screen.Clear();
            var col = LedMatrix.ColorFromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
            var current = 0;
            while (current < Moves)
            {

                screen.Clear();
                screen.DrawCircle(col, x, y, 1);
                screen.Flush();
                Thread.Sleep(Delay);
                x += ax;
                y += ay;
                if (x + ax > cols || x < 0)
                {
                    ax = -ax;
                }
                if (ay + y > rows || y < 0)
                {
                    ay = -ay;
                }
                current++;
            }
        }
        static void LineAnimation2(int Delay = 10)
        {
            screen.Clear();
            var col = LedMatrix.ColorFromRgb(0, 20, 50);
            var rnd = new Random();
            for (int x = 0; x < cols; x++)
            {
                col = LedMatrix.ColorFromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
                if (x % 2 == 0)
                {
                    for (int y = 0; y < rows; y++)
                    {
                        screen.SetPixel(x, y, col);
                        screen.Flush();
                        Thread.Sleep(Delay);
                    }
                }
                else
                {
                    for (int y = rows - 1; y >= 0; y--)
                    {
                        screen.SetPixel(x, y, col);
                        screen.Flush();
                        Thread.Sleep(Delay);
                    }
                }
            }
        }
        static void LineAnimation(int Delay = 10)
        {
            screen.Clear();
            var col = LedMatrix.ColorFromRgb(0, 20, 50);
            var rnd = new Random();
            for (int y = 0; y < rows; y++)
            {
                col = LedMatrix.ColorFromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
                if (y % 2 == 0)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        screen.SetPixel(x, y, col);
                        screen.Flush();
                        Thread.Sleep(Delay);
                    }
                }
                else
                {
                    for (int x = cols - 1; x >= 0; x--)
                    {
                        screen.SetPixel(x, y, col);
                        screen.Flush();
                        Thread.Sleep(Delay);
                    }
                }
            }
        }
        static void CharAnimation(string[] Words, int Delay = 500)
        {
            screen.Clear();
            var col = LedMatrix.ColorFromRgb(0, 20, 50);


            foreach (var word in Words)
            {
                col = LedMatrix.ColorFromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
                var statement = string.Empty;
                for (int i = 0; i < word.Length; i++)
                {
                    statement += word[i];
                    screen.Clear();
                    screen.DrawString(statement.ToString(), col, 0, 0);
                    screen.Flush();
                    Thread.Sleep(Delay);
                }
            }
        }
        static void CountDownAnimation(int From, int To, int Incr)
        {
            screen.Clear();
            var col = LedMatrix.ColorFromRgb(0, 20, 50);


            int current = From;
            while (true)
            {
                screen.Clear();
                screen.DrawString(current.ToString(), col, 0, 0);
                screen.Flush();
                Thread.Sleep(10);
                if (current % 10 == 0)
                {
                    col = LedMatrix.ColorFromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
                }
                if (current >= Int32.MaxValue) current = 0;
                if (current == To) break;
                current += Incr;
            }
        }
    }
    class LedMatrix : BasicGraphics
    {
        private uint row, column;
        PixelController leds;

        public LedMatrix(int pin, uint column, uint row)
        {
            this.row = row;
            this.column = column;
            this.leds = new PixelController(pin, this.row * this.column, false);

            //this.leds = new WS2812Controller(pin, this.row * this.column, WS2812Controller.DataFormat.rgb565);

            Clear();
        }

        public override void Clear()
        {
            for(short i = 0; i < this.leds.PixelsCount; i++)
            {
                leds.SetColor(i, 0, 0, 0);
            }
        }

        public override void SetPixel(int x, int y, uint color)
        {
            if (x < 0 || x >= this.column) return;
            if (y < 0 || y >= this.row) return;

            // even columns are inverted
            if ((x & 0x01) != 0)
            {
                y = (int)(this.row - 1 - y);
            }

            var index = x * this.row + y;

            leds.SetColor((short)index, (byte)(color >> 16), (byte)(color >> 8), (byte)(color >> 0));
        }
        public void Flush()
        {
            //leds.Flush();
        }
    }
}
