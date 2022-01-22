using Iot.Device.Bmxx80;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.I2c;
using System.Diagnostics;
using System.Threading;

namespace MeasureTempApp
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
            //for ESP 32
            Configuration.SetPinFunction(nanoFramework.Hardware.Esp32.Gpio.IO22, DeviceFunction.I2C1_CLOCK);
            Configuration.SetPinFunction(nanoFramework.Hardware.Esp32.Gpio.IO21, DeviceFunction.I2C1_DATA);

            // I2C bus 1 is using GPIO 18 and GPIO 19 on the ESP32
            const int busId = 1;
            I2cConnectionSettings i2cSettings = new(busId, Bmp280.SecondaryI2cAddress,I2cBusSpeed.StandardMode);
            I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
                var i2CBmp280 = new Bmp280(i2cDevice);

                // set higher sampling
                i2CBmp280.TemperatureSampling = Sampling.LowPower;
                i2CBmp280.PressureSampling = Sampling.UltraHighResolution;


                while (true)
                {
                    var readResult = i2CBmp280.Read();
                    if (readResult != null)
                    {
                        Debug.WriteLine($"Temp: {readResult.Temperature.DegreesCelsius} C, Pressure: {readResult.Pressure.Bars} bar");
                    }
                    Thread.Sleep(500);
                }
            
        }
    }
}
