using nanoFramework.Media;
using System;
using System.Device.Dac;
using System.Diagnostics;
using System.Threading;

namespace WavDemoApp
{
    public class Program
    {
        public static void Main()
        {
            string devices = DacController.GetDeviceSelector();
            Debug.WriteLine("DAC controllers: " + devices);

            // get default controller
            DacController dac = DacController.GetDefault();

            // open channel 0
            DacChannel analogOut = dac.OpenChannel(0);

            //var analogOut = dac.OpenChannel(SC20100.DacChannel.PA4);

            //wav file in resources has been renamed to extension .dat
            var byteFile = Resources.GetBytes
                (Resources.BinaryResources.wavsample);

            var wavFile = new Wav(byteFile);
            var dataIndex = wavFile.GetDataIndex();
            var size = wavFile.GetDataSize();
            var sampleRate = wavFile.GetSampleRate();

            if (sampleRate == 8000)
            {
                for (int i = dataIndex; i < size; i++)
                {
                    analogOut.WriteValue(byteFile[i]);

                    for (int timer = 0; timer < 58; timer++) { }
                }
            }
            else
            {
                Debug.WriteLine("Sorry, file does not have an 8 kHz sample rate.");
            }

            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
    }
}
