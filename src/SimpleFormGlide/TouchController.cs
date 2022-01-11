using FocalTech.FT5xx6;
using System.Device.Gpio;
using System.Device.I2c;
using System.Diagnostics;

namespace SimpleFormGlide
{
    public class TouchController
    {



        private delegate void NullParamsDelegate();

        /// <summary>The delegate that is used to handle the capacitive touch events.</summary>
        /// <param name="sender">The DisplayNHVN object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        public delegate void CapacitiveTouchEventHandler(object sender, TouchEventArgs e);

        /// <summary>Raised when the module detects a capacitive press.</summary>
        public event CapacitiveTouchEventHandler CapacitiveScreenPressed;

        /// <summary>Raised when the module detects a capacitive release.</summary>
        public event CapacitiveTouchEventHandler CapacitiveScreenReleased;

        /// <summary>Raised when the module detects a capacitive release.</summary>
        public event CapacitiveTouchEventHandler CapacitiveScreenMove;

        /// <summary>Constructs a new instance.</summary>
        /// <param name="Pin9OnGSocket">Backlight pin.</param>
        /// <param name="Pin3OnISocket">Interrupt pin for capacitive touch panel (i2c).</param>
        public TouchController(int InteruptPin)
        {   
            SetupCapacitiveTouchController(InteruptPin);
        }
        
        /// <summary>
        /// Event arguments for the capacitive touch events.
        /// </summary>
        public class TouchEventArgs
        {
            /// <summary>
            /// The X coordinate of the touch event.
            /// </summary>
            public int X { get; set; }

            /// <summary>
            /// The Y coordinate of the touch event.
            /// </summary>
            public int Y { get; set; }

            internal TouchEventArgs(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        #region TouchController
        private GpioController gpioController { set; get; }
        private FT5xx6Controller touch { set; get; }

        private void SetupCapacitiveTouchController(int InteruptPin)
        {
            if(gpioController== null)
                gpioController = new GpioController();
            var conf = FT5xx6Controller.GetConnectionSettings();
            var dev = I2cDevice.Create(conf);
            var gpio = gpioController.OpenPin(InteruptPin);//I don't know which pin to use for interrupt
            touch = new FT5xx6Controller(dev, gpio);//UCMStandard.GpioPin.B - ref:https://docs.ghielectronics.com/hardware/ucm/standard.html#pin-assignments

            touch.TouchDown += (_, e) => {
                if (this.CapacitiveScreenPressed != null)
                    this.CapacitiveScreenPressed(this, new TouchEventArgs(e.X, e.Y));
            };
            touch.TouchUp += (_, e) =>
            {
                if (this.CapacitiveScreenReleased != null)
                    this.CapacitiveScreenReleased(this, new TouchEventArgs(e.X, e.Y));
            };
            touch.TouchMove += (_, e) =>
            {
                if (this.CapacitiveScreenMove != null)
                    this.CapacitiveScreenMove(this, new TouchEventArgs(e.X, e.Y));
            };



        }


        #endregion
    }

}
