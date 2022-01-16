using GHIElectronics.TinyCLR.Drivers.Motor.Servo;
using System;
using System.Diagnostics;
using System.Threading;

namespace ServoDemoApp
{
    public class Program
    {
        public static void Main()
        {
            var servo = new ServoController(nanoFramework.Hardware.Esp32.Gpio.IO16);

            while (true)
            {
                servo.Set(0); // 0 degree

                Thread.Sleep(2000);

                servo.Set(45.0); // 45 degree

                Thread.Sleep(2000);

                servo.Set(90.0);  // 90 degree

                Thread.Sleep(2000);

                servo.Set(180.0); // 180 degree

                Thread.Sleep(4000);
            }

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
    }
}
