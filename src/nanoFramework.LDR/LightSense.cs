using System;
using System.Diagnostics;
using Windows.Devices.Adc;

namespace nanoFramework.LDR
{
	public class LightSense 
	{

		/// <summary>The maximum amount of lux the sensor can detect before becoming saturated.</summary>
		public const double MAX_ILLUMINANCE = 1000;
		private AdcChannel input;

		/// <summary>Constructs a new instance.</summary>
		/// <param name="socketNumber">The socket that this module is plugged in to.</param>
		public LightSense(int channelNum)
		{
			//Socket socket = Socket.GetSocket(socketNumber, true, this, null);
			//socket.EnsureTypeIsSupported('A', this);
			string devs = AdcController.GetDeviceSelector();

			Debug.WriteLine("devs=" + devs);

			AdcController adc1 = AdcController.GetDefault();
			this.input = adc1.OpenChannel(channelNum);
			//this.input = GTI.AnalogInputFactory.Create(socket, Socket.Pin.Three, this);
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

		/// <summary>Returns the current sensor reading in lux.</summary>
		/// <returns>A reading in lux between 0 and MAX_ILLUMINANCE.</returns>
		public double GetIlluminance()
		{
			
			return this.input.ReadRatio() * LightSense.MAX_ILLUMINANCE;
		}
	}
}
