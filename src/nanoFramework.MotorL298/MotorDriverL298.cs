using System;
using System.Device.Gpio;
using System.Threading;
using Windows.Devices.Pwm;

namespace nanoFramework.MotorL298
{
	public class MotorDriverL298 
	{
		private const int STEP_FACTOR = 250;

		private PwmPin pwm;
		//private GpioPin direction;
		private double lastSpeed;

		/// <summary>Used to set the PWM frequency for the motors because some motors require a certain frequency in order to operate properly. It defaults to 25KHz (25000).</summary>
		public int Frequency { get; set; }

		/// <summary>Constructs a new instance.</summary>
		/// <param name="socketNumber">The socket that this module is plugged in to.</param>
		public MotorDriverL298(int pinPwmNumber)
		{

			PwmController pwmCtl = PwmController.GetDefault();
			this.pwm = pwmCtl.OpenPin(pinPwmNumber);

			//this.direction = 

			this.lastSpeed = 0.0;

			this.Frequency = 25000;

			this.StopAll();
		}

		/// <summary>Stops all motors.</summary>
		public void StopAll()
		{
			this.SetSpeed(0);
		}

		/// <summary>Sets the given motor's speed.</summary>
		/// <param name="motor">The motor to set the speed for.</param>
		/// <param name="speed">The desired speed of the motor between -1 and 1.</param>
		public void SetSpeed(double speed)
		{
			if (speed > 1 || speed < -1) new ArgumentOutOfRangeException("speed", "speed must be between -1 and 1.");
	
			if (speed == 1.0)
				speed = 0.99;

			if (speed == -1.0)
				speed = -0.99;

			//this.direction.Write(speed < 0);
			this.pwm.Controller.SetDesiredFrequency(this.Frequency);
			this.pwm.SetActiveDutyCyclePercentage(speed < 0 ? 1 + speed : speed);
			this.lastSpeed = speed;
		}

		/// <summary>Sets the given motor's speed.</summary>
		/// <param name="motor">The motor to set the speed for.</param>
		/// <param name="speed">The desired speed of the motor between -1 and 1.</param>
		/// <param name="time">How many milliseconds the motor should take to reach the specified speed.</param>
		public void SetSpeed(double speed, int time)
		{
			if (speed > 1 || speed < -1) new ArgumentOutOfRangeException("speed", "speed must be between -1  and 1.");
		
			double currentSpeed = this.lastSpeed;

			if (currentSpeed == speed)
				return;

			int sleep = (int)(time / (Math.Abs(speed - currentSpeed) * MotorDriverL298.STEP_FACTOR));
			double step = 1.0 / MotorDriverL298.STEP_FACTOR;

			if (sleep < 1)
				throw new ArgumentOutOfRangeException("time", "You cannot move to a speed this close to the existing speed in so little time.");

			if (speed < currentSpeed)
				step *= -1;

			while (Math.Abs(speed - currentSpeed) >= 0.01)
			{
				currentSpeed += step;

				this.SetSpeed(currentSpeed);

				Thread.Sleep(sleep);
			}
		}
	}
}
