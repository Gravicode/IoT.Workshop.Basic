using System;
using System.Device.Gpio;

namespace nanoFramework.PIR
{
	public class PIR
	{
		private GpioPin interrupt;
		private MotionEventHandler onMotionSensed;

		/// <summary>Represents the delegate that is used to handle the <see cref="MotionSensed" /> event.</summary>
		/// <param name="sender">The <see cref="PIR" /> object that raised the event.</param>
		/// <param name="e">The event arguments.</param>
		public delegate void MotionEventHandler(PIR sender, EventArgs e);

		/// <summary>Raised when the state of <see cref="PIR" /> is high.</summary>
		public event MotionEventHandler MotionSensed;

		/// <summary>Whether or not the sensor is still high after detecthing motion.</summary>
		public bool SensorStillActive
		{
			get
			{
				return this.interrupt.Read()==PinValue.High;
			}
		}

		/// <summary>Constructs a new instance.</summary>
		/// <param name="socketNumber">The socket that this module is plugged in to.</param>
		public PIR(int interuptPin)
		{
			//var socket = Socket.GetSocket(socketNumber, true, this, null);

			this.onMotionSensed = this.OnMotionSensed;

			var gpio = new GpioController();
			this.interrupt = gpio.OpenPin(interuptPin,PinMode.InputPullUp);
			//GTI.InterruptInputFactory.Create(socket, GT.Socket.Pin.Three, GTI.GlitchFilterMode.On, GTI.ResistorMode.PullUp, GTI.InterruptMode.RisingAndFallingEdge, this);
			this.interrupt.ValueChanged += (a, b) => {
				
				if (this.interrupt.Read()==PinValue.High)
					this.OnMotionSensed(this, null);
			};
		}

		private void OnMotionSensed(PIR sender, EventArgs e)
		{
			//if (Program.CheckAndInvoke(this.MotionSensed, this.onMotionSensed, sender, e))
				this.MotionSensed(sender, e);
		}
	}
}
