using System;
using System.Diagnostics;
using Windows.Devices.Adc;

namespace nanoFramework.TCRT5000
{
	public class Reflector
	{
		private AdcChannel input;


		/// <summary>The reflectors on the module.</summary>

		/// <summary>Constructs a new instance.</summary>
		/// <param name="socketNumber">The socket that this module is plugged in to.</param>
		public Reflector(int inputChannel = 0)
		{
			string devs = AdcController.GetDeviceSelector();

			Debug.WriteLine("devs=" + devs);

			AdcController adc1 = AdcController.GetDefault();


			input = adc1.OpenChannel(inputChannel);
		}

		/// <summary>Gets the reflective reading from one of the reflectors.</summary>
		/// <param name="reflector">The reflector to read from.</param>
		/// <returns>A number between 0 and 1 where 0 is no reflection and 1 is maximum reflection.</returns>
		public double Read()
		{
			return 1 - this.input.ReadRatio();
		
		}
	}
}
