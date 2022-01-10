using System;
using System.Device.Gpio;
//using System.Device.Pwm;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.Pwm;

namespace BluinoApp
{
    public class Program
    {
        #region tunes
        const int NOTE_C = 261;
        const int NOTE_D = 294;
        const int NOTE_E = 330;
        const int NOTE_F = 349;
        const int NOTE_G = 392;

        const int WHOLE_DURATION = 2000;
        const int EIGHTH = WHOLE_DURATION / 8;
        const int QUARTER = WHOLE_DURATION / 4;
        const int QUARTERDOT = WHOLE_DURATION / 3;
        const int HALF = WHOLE_DURATION / 2;
        const int WHOLE = WHOLE_DURATION;

        //Make sure the two below arrays match in length. Each duration element corresponds to
        //  one note element.
        private static int[] note = { NOTE_E, NOTE_E, NOTE_F, NOTE_G, NOTE_G, NOTE_F, NOTE_E,
                          NOTE_D, NOTE_C, NOTE_C, NOTE_D, NOTE_E, NOTE_E, NOTE_D,
                          NOTE_D, NOTE_E, NOTE_E, NOTE_F, NOTE_G, NOTE_G, NOTE_F,
                          NOTE_E, NOTE_D, NOTE_C, NOTE_C, NOTE_D, NOTE_E, NOTE_D,
                          NOTE_C, NOTE_C};

        private static int[] duration = { QUARTER, QUARTER, QUARTER, QUARTER, QUARTER, QUARTER,
                              QUARTER, QUARTER, QUARTER, QUARTER, QUARTER, QUARTER,
                              QUARTERDOT, EIGHTH, HALF, QUARTER, QUARTER, QUARTER, QUARTER,
                              QUARTER, QUARTER, QUARTER, QUARTER, QUARTER, QUARTER,
                              QUARTER, QUARTER, QUARTERDOT, EIGHTH, WHOLE};
        #endregion
        private static GpioController s_GpioController;
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");
            s_GpioController = new GpioController();

            //test led
            Thread thLed = new Thread(new ThreadStart(TestLed));
            thLed.Start();

            //test buzzer
            Thread thBuz = new Thread(new ThreadStart(TestBuzzer));
            thBuz.Start();

            //test button and relay
            var relay = s_GpioController.OpenPin(nanoFramework.Hardware.Esp32.Gpio.IO12, PinMode.Output);
            Button btn = new Button(nanoFramework.Hardware.Esp32.Gpio.IO13);
            btn.ButtonPressed += (a, b) => { relay.Write(PinValue.Low); };
            btn.ButtonReleased += (a, b) => { relay.Write(PinValue.High); };

            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }

        static void TestBuzzer()
        {
            PwmController controller = PwmController.GetDefault();
            PwmPin pwmPin = controller.OpenPin(nanoFramework.Hardware.Esp32.Gpio.IO16);
          
            pwmPin.SetActiveDutyCyclePercentage( 0.5);
            while (true)
            {
                pwmPin.Start();

                for (int i = 0; i < note.Length; i++)
                {
                    pwmPin.Controller.SetDesiredFrequency(note[i]);
                   
                    Thread.Sleep(duration[i]);
                }

                pwmPin.Stop();

                Thread.Sleep(1000);
            }
        }

        static void TestLed()
        {
          
            GpioPin led = s_GpioController.OpenPin(nanoFramework.Hardware.Esp32.Gpio.IO23, PinMode.Output);
            led.Write(PinValue.Low);

            while (true)
            {
                led.Toggle();
                Thread.Sleep(125);
                led.Toggle();
                Thread.Sleep(125);
                led.Toggle();
                Thread.Sleep(125);
                led.Toggle();
                Thread.Sleep(525);
            }
        }
    }
}
