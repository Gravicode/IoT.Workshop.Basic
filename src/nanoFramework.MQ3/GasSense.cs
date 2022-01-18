using System;
using System.Device.Gpio;
using System.Diagnostics;
using Windows.Devices.Adc;

namespace nanoFramework.MQ3
{
	public class GasSense 
	{
		private AdcChannel input;
		private GpioPin enable;

		/// <summary>Turns the heating element on or off. This may take up to 10 seconds befre a proper reading is taken.</summary>
		public bool HeatingElementEnabled
		{
			get
			{
				return this.enable.Read()==PinValue.High;
			}

			set
			{
				this.enable.Write(value?PinValue.High:PinValue.Low);
			}
		}

		/// <summary>Constructs a new instance.</summary>
		/// <param name="socketNumber">The socket that this module is plugged in to.</param>
		public GasSense(int channelADC,int heatingPin)
		{
			//Socket socket = Socket.GetSocket(socketNumber, true, this, null);
			//socket.EnsureTypeIsSupported('A', this);
			string devs = AdcController.GetDeviceSelector();

			Debug.WriteLine("devs=" + devs);

			AdcController adc1 = AdcController.GetDefault();
			
			this.input = adc1.OpenChannel(channelADC);
			//GTI.AnalogInputFactory.Create(socket, Socket.Pin.Three, this);
			var gpio = new GpioController();
			this.enable = gpio.OpenPin(heatingPin, PinMode.Output);
			this.enable.Write(PinValue.Low);
			//GTI.DigitalOutputFactory.Create(socket, Socket.Pin.Four, false, this);
		}

		/// <summary>The voltage returned from the sensor.</summary>
		/// <returns>The voltage value between 0.0 and 3.3</returns>
		public double ReadVoltage()
		{
			return this.input.ReadValue();
		}

		/// <summary>The proportion returned from the sensor.</summary>
		/// <returns>The value between 0.0 and 1.0</returns>
		public double ReadProportion()
		{
			return this.input.ReadRatio();
		}
	}
}
