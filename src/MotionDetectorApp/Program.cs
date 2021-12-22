using Iot.Device.Hcsr501;
using nanoFramework.Hardware.Esp32;
using System;
using System.Diagnostics;
using System.Threading;

namespace MotionDetectorApp
{
    public class Program
    {
        static Hcsr501 pir;
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");
            pir = new Hcsr501(Gpio.IO32);
            pir.Hcsr501ValueChanged += Pir_Hcsr501ValueChanged;

            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }

        private static void Pir_Hcsr501ValueChanged(object sender, Hcsr501ValueChangedEventArgs e)
        {
            Debug.WriteLine($"Deteksi Gerakan: { (pir.IsMotionDetected ? "Ya" : "Tidak") }");
        }
    }
}
