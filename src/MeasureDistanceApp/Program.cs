using Iot.Device;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace MeasureDistanceApp
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");
            var th1 = new Thread(new ThreadStart(Loop));
            th1.Start();
            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }

        static void Loop()
        {
            var gpio = new GpioController();
           
            var distanceSensor = new Iot.Device.Hcsr04.Hcsr04(Gpio.IO05,Gpio.IO18);

            while (true)
            {
                try
                {
                    Debug.WriteLine($"jarak: {distanceSensor.Distance.Centimeters} cm");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                Thread.Sleep(500);
            }
        }
    }
}
