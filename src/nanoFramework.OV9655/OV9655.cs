using System;
using System.Device.I2c;
using System.Threading;

namespace nanoFramework.OV9655
{

    public class Ov9655Controller
    {
        private const byte I2C_ADDRESS = 48;
        private I2cDevice i2cDevice;
        //private CameraController cameraController;
        private byte[] data;
        private TimeSpan timeout = TimeSpan.FromMilliseconds(500);

        public byte[] Buffer => this.data;

        public TimeSpan Timeout
        {
            get => this.timeout;
            set => this.timeout = value;
        }

        public event Ov9655Controller.FrameReceivedEventHandler FrameReceived;

        public Ov9655Controller()
        {
            /*
            I2cConnectionSettings connectionSettings = new I2cConnectionSettings(48)
            {
                BusSpeed = 100000,
                AddressFormat = I2cAddressFormat.SevenBit
            };*/
            var setting = new I2cConnectionSettings(1, I2C_ADDRESS, I2cBusSpeed.StandardMode);
            var device = new I2cDevice(setting);
            this.i2cDevice = device;// i2cController.GetDevice(connectionSettings);
            //this.cameraController = CameraController.GetDefault();
            //this.cameraController.SetActiveSettings(CaptureRate.AllFrame, false, true, true, SynchronizationMode.Hardware, ExtendedDataMode.Extended8bit, 16000000U);
            //this.cameraController.Enable();
            this.Reset();
        }

        public string ReadId()
        {
            this.WriteRegister(byte.MaxValue, (byte)1);
            return ((int)this.ReadRegister((byte)10) << 8 | (int)this.ReadRegister((byte)11)).ToString("x");
        }

        public bool Capture()
        {
            try
            {
                if (this.data != null)
                {
                    int size = data.Length;
                    //int size = this.cameraController.Capture(this.data, (int)this.timeout.TotalMilliseconds);
                    Ov9655Controller.FrameReceivedEventHandler frameReceived = this.FrameReceived;
                    if (frameReceived != null)
                        frameReceived(this.data, size);
                    return size == this.data.Length;
                }
            }
            catch
            {
            }
            if (this.data == null)
                throw new Exception("Need SetActiveSettings.");
            return false;
        }

        public void SetResolution(Ov9655Controller.Resolution size)
        {
            switch (size)
            {
                case Ov9655Controller.Resolution.Vga:
                    this.data = new byte[614400];
                    this.SetVga();
                    break;
            }
        }

        private void SetVga()
        {
            Thread.Sleep(2);
            this.WriteRegister((byte)0, (byte)0);
            Thread.Sleep(2);
            this.WriteRegister((byte)1, (byte)128);
            Thread.Sleep(2);
            this.WriteRegister((byte)2, (byte)128);
            Thread.Sleep(2);
            this.WriteRegister((byte)181, (byte)0);
            Thread.Sleep(2);
            this.WriteRegister((byte)53, (byte)0);
            Thread.Sleep(2);
            this.WriteRegister((byte)168, (byte)193);
            Thread.Sleep(2);
            this.WriteRegister((byte)58, (byte)204);
            Thread.Sleep(2);
            this.WriteRegister((byte)61, (byte)153);
            Thread.Sleep(2);
            this.WriteRegister((byte)119, (byte)2);
            Thread.Sleep(2);
            this.WriteRegister((byte)19, (byte)231);
            Thread.Sleep(2);
            this.WriteRegister((byte)38, (byte)114);
            Thread.Sleep(2);
            this.WriteRegister((byte)39, (byte)8);
            Thread.Sleep(2);
            this.WriteRegister((byte)40, (byte)8);
            Thread.Sleep(2);
            this.WriteRegister((byte)44, (byte)8);
            Thread.Sleep(2);
            this.WriteRegister((byte)171, (byte)4);
            Thread.Sleep(2);
            this.WriteRegister((byte)110, (byte)0);
            Thread.Sleep(2);
            this.WriteRegister((byte)109, (byte)85);
            Thread.Sleep(2);
            this.WriteRegister((byte)0, (byte)17);
            Thread.Sleep(2);
            this.WriteRegister((byte)16, (byte)123);
            Thread.Sleep(2);
            this.WriteRegister((byte)187, (byte)174);
            Thread.Sleep(2);
            this.WriteRegister((byte)17, (byte)1);
            Thread.Sleep(2);
            this.WriteRegister((byte)114, (byte)0);
            Thread.Sleep(2);
            this.WriteRegister((byte)62, (byte)12);
            Thread.Sleep(2);
            this.WriteRegister((byte)116, (byte)58);
            Thread.Sleep(2);
            this.WriteRegister((byte)118, (byte)1);
            Thread.Sleep(2);
            this.WriteRegister((byte)117, (byte)53);
            Thread.Sleep(2);
            this.WriteRegister((byte)115, (byte)0);
            Thread.Sleep(2);
            this.WriteRegister((byte)199, (byte)128);
            Thread.Sleep(2);
            this.WriteRegister((byte)98, (byte)0);
            Thread.Sleep(2);
            this.WriteRegister((byte)99, (byte)0);
            Thread.Sleep(2);
            this.WriteRegister((byte)100, (byte)2);
            Thread.Sleep(2);
            this.WriteRegister((byte)101, (byte)32);
            Thread.Sleep(2);
            this.WriteRegister((byte)102, (byte)1);
            Thread.Sleep(2);
            this.WriteRegister((byte)195, (byte)78);
            Thread.Sleep(2);
            this.WriteRegister((byte)51, (byte)0);
            Thread.Sleep(2);
            this.WriteRegister((byte)164, (byte)80);
            Thread.Sleep(2);
            this.WriteRegister((byte)170, (byte)146);
            Thread.Sleep(2);
            this.WriteRegister((byte)194, (byte)1);
            Thread.Sleep(2);
            this.WriteRegister((byte)193, (byte)200);
            Thread.Sleep(2);
            this.WriteRegister((byte)30, (byte)20);
            Thread.Sleep(2);
            this.WriteRegister((byte)169, (byte)239);
            Thread.Sleep(2);
            this.WriteRegister((byte)14, (byte)97);
            Thread.Sleep(2);
            this.WriteRegister((byte)57, (byte)87);
            Thread.Sleep(2);
            this.WriteRegister((byte)15, (byte)72);
            Thread.Sleep(2);
            this.WriteRegister((byte)36, (byte)60);
            Thread.Sleep(2);
            this.WriteRegister((byte)37, (byte)54);
            Thread.Sleep(2);
            this.WriteRegister((byte)18, (byte)99);
            Thread.Sleep(2);
            this.WriteRegister((byte)3, (byte)18);
            Thread.Sleep(2);
            this.WriteRegister((byte)50, byte.MaxValue);
            Thread.Sleep(2);
            this.WriteRegister((byte)23, (byte)22);
            Thread.Sleep(2);
            this.WriteRegister((byte)24, (byte)2);
            Thread.Sleep(2);
            this.WriteRegister((byte)25, (byte)1);
            Thread.Sleep(2);
            this.WriteRegister((byte)26, (byte)61);
            Thread.Sleep(2);
            this.WriteRegister((byte)54, (byte)250);
            Thread.Sleep(2);
            this.WriteRegister((byte)105, (byte)10);
            Thread.Sleep(2);
            this.WriteRegister((byte)140, (byte)141);
            Thread.Sleep(2);
            this.WriteRegister((byte)192, (byte)170);
            Thread.Sleep(2);
            this.WriteRegister((byte)64, (byte)208);
            Thread.Sleep(2);
            this.WriteRegister((byte)67, (byte)20);
            Thread.Sleep(2);
            this.WriteRegister((byte)68, (byte)240);
            Thread.Sleep(2);
            this.WriteRegister((byte)69, (byte)70);
            Thread.Sleep(2);
            this.WriteRegister((byte)70, (byte)98);
            Thread.Sleep(2);
            this.WriteRegister((byte)71, (byte)42);
            Thread.Sleep(2);
            this.WriteRegister((byte)72, (byte)60);
            Thread.Sleep(2);
            this.WriteRegister((byte)89, (byte)133);
            Thread.Sleep(2);
            this.WriteRegister((byte)90, (byte)169);
            Thread.Sleep(2);
            this.WriteRegister((byte)91, (byte)100);
            Thread.Sleep(2);
            this.WriteRegister((byte)92, (byte)132);
            Thread.Sleep(2);
            this.WriteRegister((byte)93, (byte)83);
            Thread.Sleep(2);
            this.WriteRegister((byte)94, (byte)14);
            Thread.Sleep(2);
            this.WriteRegister((byte)108, (byte)12);
            Thread.Sleep(2);
            this.WriteRegister((byte)198, (byte)133);
            Thread.Sleep(2);
            this.WriteRegister((byte)203, (byte)240);
            Thread.Sleep(2);
            this.WriteRegister((byte)204, (byte)216);
            Thread.Sleep(2);
            this.WriteRegister((byte)113, (byte)120);
            Thread.Sleep(2);
            this.WriteRegister((byte)165, (byte)104);
            Thread.Sleep(2);
            this.WriteRegister((byte)111, (byte)158);
            Thread.Sleep(2);
            this.WriteRegister((byte)66, (byte)192);
            Thread.Sleep(2);
            this.WriteRegister((byte)63, (byte)130);
            Thread.Sleep(2);
            this.WriteRegister((byte)138, (byte)35);
            Thread.Sleep(2);
            this.WriteRegister((byte)20, (byte)58);
            Thread.Sleep(2);
            this.WriteRegister((byte)59, (byte)204);
            Thread.Sleep(2);
            this.WriteRegister((byte)52, (byte)61);
            Thread.Sleep(2);
            this.WriteRegister((byte)65, (byte)64);
            Thread.Sleep(2);
            this.WriteRegister((byte)201, (byte)224);
            Thread.Sleep(2);
            this.WriteRegister((byte)202, (byte)232);
            Thread.Sleep(2);
            this.WriteRegister((byte)205, (byte)147);
            Thread.Sleep(2);
            this.WriteRegister((byte)122, (byte)32);
            Thread.Sleep(2);
            this.WriteRegister((byte)123, (byte)28);
            Thread.Sleep(2);
            this.WriteRegister((byte)124, (byte)40);
            Thread.Sleep(2);
            this.WriteRegister((byte)125, (byte)60);
            Thread.Sleep(2);
            this.WriteRegister((byte)126, (byte)90);
            Thread.Sleep(2);
            this.WriteRegister((byte)127, (byte)104);
            Thread.Sleep(2);
            this.WriteRegister((byte)128, (byte)118);
            Thread.Sleep(2);
            this.WriteRegister((byte)129, (byte)128);
            Thread.Sleep(2);
            this.WriteRegister((byte)130, (byte)136);
            Thread.Sleep(2);
            this.WriteRegister((byte)131, (byte)143);
            Thread.Sleep(2);
            this.WriteRegister((byte)132, (byte)150);
            Thread.Sleep(2);
            this.WriteRegister((byte)133, (byte)163);
            Thread.Sleep(2);
            this.WriteRegister((byte)134, (byte)175);
            Thread.Sleep(2);
            this.WriteRegister((byte)135, (byte)196);
            Thread.Sleep(2);
            this.WriteRegister((byte)136, (byte)215);
            Thread.Sleep(2);
            this.WriteRegister((byte)137, (byte)232);
            Thread.Sleep(2);
            this.WriteRegister((byte)79, (byte)152);
            Thread.Sleep(2);
            this.WriteRegister((byte)80, (byte)152);
            Thread.Sleep(2);
            this.WriteRegister((byte)81, (byte)0);
            Thread.Sleep(2);
            this.WriteRegister((byte)82, (byte)40);
            Thread.Sleep(2);
            this.WriteRegister((byte)83, (byte)112);
            Thread.Sleep(2);
            this.WriteRegister((byte)84, (byte)152);
            Thread.Sleep(2);
            this.WriteRegister((byte)88, (byte)26);
            Thread.Sleep(2);
            this.WriteRegister((byte)107, (byte)90);
            Thread.Sleep(2);
            this.WriteRegister((byte)144, (byte)146);
            Thread.Sleep(2);
            this.WriteRegister((byte)145, (byte)146);
            Thread.Sleep(2);
            this.WriteRegister((byte)159, (byte)144);
            Thread.Sleep(2);
            this.WriteRegister((byte)160, (byte)144);
            Thread.Sleep(2);
            this.WriteRegister((byte)22, (byte)36);
            Thread.Sleep(2);
            this.WriteRegister((byte)42, (byte)0);
            Thread.Sleep(2);
            this.WriteRegister((byte)43, (byte)0);
            Thread.Sleep(2);
            this.WriteRegister((byte)172, (byte)128);
            Thread.Sleep(2);
            this.WriteRegister((byte)173, (byte)128);
            Thread.Sleep(2);
            this.WriteRegister((byte)174, (byte)128);
            Thread.Sleep(2);
            this.WriteRegister((byte)175, (byte)128);
            Thread.Sleep(2);
            this.WriteRegister((byte)178, (byte)242);
            Thread.Sleep(2);
            this.WriteRegister((byte)179, (byte)32);
            Thread.Sleep(2);
            this.WriteRegister((byte)180, (byte)32);
            Thread.Sleep(2);
            this.WriteRegister((byte)182, (byte)175);
            Thread.Sleep(2);
            this.WriteRegister((byte)41, (byte)21);
            Thread.Sleep(2);
            this.WriteRegister((byte)157, (byte)2);
            Thread.Sleep(2);
            this.WriteRegister((byte)158, (byte)2);
            Thread.Sleep(2);
            this.WriteRegister((byte)158, (byte)2);
            Thread.Sleep(2);
            this.WriteRegister((byte)4, (byte)3);
            Thread.Sleep(2);
            this.WriteRegister((byte)5, (byte)46);
            Thread.Sleep(2);
            this.WriteRegister((byte)6, (byte)46);
            Thread.Sleep(2);
            this.WriteRegister((byte)7, (byte)46);
            Thread.Sleep(2);
            this.WriteRegister((byte)8, (byte)46);
            Thread.Sleep(2);
            this.WriteRegister((byte)47, (byte)46);
            Thread.Sleep(2);
            this.WriteRegister((byte)74, (byte)233);
            Thread.Sleep(2);
            this.WriteRegister((byte)75, (byte)221);
            Thread.Sleep(2);
            this.WriteRegister((byte)76, (byte)221);
            Thread.Sleep(2);
            this.WriteRegister((byte)77, (byte)221);
            Thread.Sleep(2);
            this.WriteRegister((byte)78, (byte)221);
            Thread.Sleep(2);
            this.WriteRegister((byte)112, (byte)6);
            Thread.Sleep(2);
            this.WriteRegister((byte)166, (byte)64);
            Thread.Sleep(2);
            this.WriteRegister((byte)188, (byte)2);
            Thread.Sleep(2);
            this.WriteRegister((byte)189, (byte)1);
            Thread.Sleep(2);
            this.WriteRegister((byte)190, (byte)2);
            Thread.Sleep(2);
            this.WriteRegister((byte)191, (byte)1);
            Thread.Sleep(2);
            this.WriteRegister((byte)9, (byte)17);
            Thread.Sleep(2);
            this.WriteRegister((byte)9, (byte)1);
        }

        private void Reset()
        {
            this.WriteRegister((byte)18, (byte)128);
            Thread.Sleep(100);
        }

        private void WriteRegister(byte register, byte value) => this.i2cDevice.Write(new byte[2]
        {
      register,
      value
        });

        private byte ReadRegister(byte register)
        {
            byte[] writeBuffer = new byte[1] { register };
            byte[] readBuffer = new byte[1];
            this.i2cDevice.WriteRead(writeBuffer, readBuffer);
            return readBuffer[0];
        }

        public delegate void FrameReceivedEventHandler(byte[] data, int size);

        public enum Resolution
        {
            Vga,
            Qvga,
            Qqvga,
        }
    }
}
