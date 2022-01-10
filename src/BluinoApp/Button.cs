using System;
using System.Device.Gpio;

namespace BluinoApp
{
	public class Button
	{
		private GpioPin input;
		
		private ButtonEventHandler onButtonEvent;

		/// <summary>Represents the delegate that is used to handle the <see cref="ButtonReleased" /> and <see cref="ButtonPressed" /> events.</summary>
		/// <param name="sender">The <see cref="Button" /> object that raised the event.</param>
		/// <param name="state">The state of the Button</param>
		public delegate void ButtonEventHandler(Button sender, ButtonState state);

		/// <summary>Raised when the button is released.</summary>
		public event ButtonEventHandler ButtonReleased;

		/// <summary>Raised when the button is pressed.</summary>
		public event ButtonEventHandler ButtonPressed;

		/// <summary>Whether or not the button is pressed.</summary>
		public bool Pressed
		{
			get
			{
				return !(this.input.Read()==PinValue.High);
			}
		}

		/// <summary>Whether or not the LED is currently on or off.</summary>

		/// <summary>The state of the button.</summary>
		public enum ButtonState
		{

			/// <summary>The button is pressed.</summary>
			Pressed = 0,

			/// <summary>The button is released.</summary>
			Released = 1
		}

		/// <summary>The various modes a LED can be set to.</summary>
		public enum LedMode
		{

			/// <summary>The LED is on regardless of the button state.</summary>
			On,

			/// <summary>The LED is off regardless of the button state.</summary>
			Off,

			/// <summary>The LED changes state whenever the button is pressed.</summary>
			ToggleWhenPressed,

			/// <summary>The LED changes state whenever the button is released.</summary>
			ToggleWhenReleased,

			/// <summary>The LED is on while the button is pressed.</summary>
			OnWhilePressed,

			/// <summary>The LED is on except when the button is pressed.</summary>
			OnWhileReleased
		}

		/// <summary>Constructs a new instance.</summary>
		/// <param name="socketNumber">The mainboard socket that has the module plugged into it.</param>
		public Button(int pinNumber)
		{
			var s_GpioController = new GpioController();

			this.input = s_GpioController.OpenPin(pinNumber, PinMode.InputPullUp);//GTI.InterruptInputFactory.Create(socket, GT.Socket.Pin.Three, GTI.GlitchFilterMode.On, GTI.ResistorMode.PullUp, GTI.InterruptMode.RisingAndFallingEdge, this);
            this.input.ValueChanged += this.OnInterrupt;
		}

 

     
		private void OnInterrupt(object sender, PinValueChangedEventArgs e)
		{
			var state = this.input.Read()==PinValue.High ? ButtonState.Released : ButtonState.Pressed;
			this.OnButtonEvent(this, state);
		}

		private void OnButtonEvent(Button sender, ButtonState state)
		{
			if (this.onButtonEvent == null)
				this.onButtonEvent = this.OnButtonEvent;

				switch (state)
				{
					case ButtonState.Released: this.ButtonReleased(sender, state); break;
					case ButtonState.Pressed: this.ButtonPressed(sender, state); break;
				}
		}
	}
}
