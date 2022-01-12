using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using VlsiSolution.VS1053B;

namespace Mp3DemoApp
{
    public class Program
    {
        public static void Main()
        {
            //Debug.WriteLine("Hello from nanoFramework!");
            //Play MP3 with VS1053 module
            var gpio = new GpioController();
            var dreq = gpio.OpenPin(nanoFramework.Hardware.Esp32.Gpio.IO12);
            var reset = gpio.OpenPin(nanoFramework.Hardware.Esp32.Gpio.IO13);
            var dataChipSelect = gpio.OpenPin(nanoFramework.Hardware.Esp32.Gpio.IO26);
            var commandChipSelect = gpio.OpenPin(nanoFramework.Hardware.Esp32.Gpio.IO27);
            //var spi = SpiController.FromName(SC13048.SpiBus.Spi1);

            var mp3decoder = new VS1053BController( dreq, reset, dataChipSelect, commandChipSelect);
            var mp3Bytes = Resources.GetBytes(Resources.BinaryResources.sound);
            mp3decoder.SetVolume(250, 250);
            mp3decoder.SendData(mp3Bytes);

            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
    }
}
