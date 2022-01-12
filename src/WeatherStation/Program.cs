using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace WeatherStation
{
    public class Program
    {
        public static void Main()
        {
            //LORA using RN2903 Microchip
            var th1 = new Thread(new ThreadStart(loop));
            th1.Start();
            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
        static SerialPort uart;
        static void ReadSerial()
        {
            //uart.DataReceived += uart_DataReceived;
            /*
            while (true)
            {
                // read one byte
                read_count = uart.Read(rx_byte, 0, 1);
                if (read_count > 0)// do we have data?
                {
                    // create a string
                    string counter_string =
                            "You typed: " + rx_byte[0].ToString() + "\r\n";
                    // convert the string to bytes
                    byte[] buffer = Encoding.UTF8.GetBytes(counter_string);
                    // send the bytes on the serial port
                    uart.Write(buffer, 0, buffer.Length);
                    //wait...
                    Thread.Sleep(10);
                }
            }*/

        }
        static double temp;
        static byte[] databuffer = new byte[35];
        /*
        void uart_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int read_count = 0;
            byte[] rx_byte = new byte[35];
            read_count = uart.Read(rx_byte, 0, rx_byte.Length);
            if (read_count > 0)// do we have data?
            {
                string buffer = new string( Encoding.UTF8.GetChars(rx_byte));
                databuffer += buffer;
                if (buffer.IndexOf("\r\n")>-1)
                {
                    Debug.WriteLine("rec: " + databuffer);
                    databuffer = string.Empty;
                }
            }
        }*/
        static void loop()
        {

            string ComPort = "COM1"; //just sample port name for station sensor
            uart = new SerialPort(ComPort, 9600);

            uart.Open();

            UART = new SimpleSerial("COM2", 57600); //port name for lora module
            UART.Port.ReadTimeout = 0;
            UART.Port.DataReceived += UART_DataReceived;
            Debug.WriteLine("57600");
            Debug.WriteLine("RN2483 Test");
            PrintToLcd("RN2483 Test");
            var controller = new GpioController();
            GpioPin reset = controller.OpenPin(nanoFramework.Hardware.Esp32.Gpio.IO13, PinMode.Output);
            reset.Write(PinValue.Low);
            var reset2 = controller.OpenPin(nanoFramework.Hardware.Esp32.Gpio.IO12, PinMode.Output);
            reset2.Write(PinValue.Low);

            reset.Write(true);
            reset2.Write(true);

            Thread.Sleep(100);
            reset.Write(false);
            reset2.Write(false);

            Thread.Sleep(100);
            reset.Write(true);
            reset2.Write(true);

            Thread.Sleep(100);

            waitForResponse();

            sendCmd("sys factoryRESET");
            sendCmd("sys get hweui");
            sendCmd("mac get deveui");
            Thread.Sleep(3000);
            // For TTN
            sendCmd("mac set devaddr AAABBBDD");  // Set own address
            Thread.Sleep(3000);
            sendCmd("mac set appskey 2B7E151628AED2A6ABF7158809CF4F3D");
            Thread.Sleep(3000);

            sendCmd("mac set nwkskey 2B7E151628AED2A6ABF7158809CF4F3D");
            Thread.Sleep(3000);

            sendCmd("mac set adr off");
            Thread.Sleep(3000);

            sendCmd("mac set rx2 3 868400000");//869525000
            Thread.Sleep(3000);

            sendCmd("mac join abp");
            Thread.Sleep(3000);
            sendCmd("mac get status");
            sendCmd("mac get devaddr");
            Thread.Sleep(2000);


            while (true)
            {
                getBuffer();
                //lora
                var data = new SensorData()
                {
                    WindDirection = WindDirection(),
                    WindSpeedMax = WindSpeedMax(),
                    BarPressure = BarPressure(),
                    Humidity = Humidity(),
                    RainfallOneDay = RainfallOneDay(),
                    RainfallOneHour = RainfallOneHour(),
                    Temperature = Temperature()
                    ,
                    WindSpeedAverage = WindSpeedAverage()


                };//Begin!
                Debug.WriteLine("Wind Direction: " + data.WindDirection);
                Debug.WriteLine("Average Wind Speed (One Minute): " + data.WindSpeedAverage + "m/s  ");
                Debug.WriteLine("Max Wind Speed (Five Minutes): " + data.WindSpeedMax + "m/s");
                Debug.WriteLine("Rain Fall (One Hour): " + data.RainfallOneHour + "mm  ");
                Debug.WriteLine("Rain Fall (24 Hour): " + data.RainfallOneDay + "mm");
                Debug.WriteLine("Temperature: " + data.Temperature + "C  ");
                Debug.WriteLine("Humidity: " + data.Humidity + "%  ");
                Debug.WriteLine("Barometric Pressure: " + data.BarPressure + "hPa");
                Debug.WriteLine("----------------------");


                var jsonStr = nanoFramework.Json.JsonConvert.SerializeObject(data);
                Debug.WriteLine("kirim :" + jsonStr);
                sendData(jsonStr);
                Thread.Sleep(5000);
                byte[] rx_data = new byte[20];

                if (UART.Port.IsOpen)
                {
                    var count = UART.Port.Read(rx_data, 0, rx_data.Length);
                    if (count > 0)
                    {
                        Debug.WriteLine("count:" + count);
                        var hasil = new string(System.Text.Encoding.UTF8.GetChars(rx_data));
                        Debug.WriteLine("read:" + hasil);
                        //mac_rx 2 AABBCC
                    }
                }
                var TimeStr = DateTime.UtcNow.ToString("dd/MM/yy HH:mm");
                var th2 = new Thread(new ThreadStart(blinkLed));
                th2.Start();
                Thread.Sleep(2000);
            }



        }


        static void blinkLed()
        {
            bool state = true;
            for (int i = 0; i < 6; i++)
            {
                //blink led here
                Thread.Sleep(300);
                state = !state;
            }

        }
        static void getBuffer()                                                                    //Get weather status data
        {
            int index;
            for (index = 0; index < 35; index++)
            {
                if (uart.BytesToRead > 0)
                {
                    databuffer[index] = (byte)uart.ReadByte();
                    if (databuffer[0] != 'c')
                    {
                        index = -1;
                    }
                }
                else
                {
                    index--;
                }
            }
        }

        static int transCharToInt(byte[] _buffer, int _start, int _stop)                               //char to int）
        {
            int _index;
            int result = 0;
            int num = _stop - _start + 1;
            var _temp = new int[num];
            for (_index = _start; _index <= _stop; _index++)
            {
                _temp[_index - _start] = _buffer[_index] - '0';
                result = 10 * result + _temp[_index - _start];
            }
            return result;
        }

        static int WindDirection()                                                                  //Wind Direction
        {
            return transCharToInt(databuffer, 1, 3);
        }

        static double WindSpeedAverage()                                                             //air Speed (1 minute)
        {
            temp = 0.44704 * transCharToInt(databuffer, 5, 7);
            return temp;
        }

        static double WindSpeedMax()                                                                 //Max air speed (5 minutes)
        {
            temp = 0.44704 * transCharToInt(databuffer, 9, 11);
            return temp;
        }

        static double Temperature()                                                                  //Temperature ("C")
        {
            temp = (transCharToInt(databuffer, 13, 15) - 32.00) * 5.00 / 9.00;
            return temp;
        }

        static double RainfallOneHour()                                                              //Rainfall (1 hour)
        {
            temp = transCharToInt(databuffer, 17, 19) * 25.40 * 0.01;
            return temp;
        }

        static double RainfallOneDay()                                                               //Rainfall (24 hours)
        {
            temp = transCharToInt(databuffer, 21, 23) * 25.40 * 0.01;
            return temp;
        }

        static int Humidity()                                                                       //Humidity
        {
            return transCharToInt(databuffer, 25, 26);
        }

        static double BarPressure()                                                                  //Barometric Pressure
        {
            temp = transCharToInt(databuffer, 28, 32);
            return temp / 10.00;
        }

        private static string[] _dataInLora;
        private static string rx;


        static void UART_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            _dataInLora = UART.Deserialize();
            for (int index = 0; index < _dataInLora.Length; index++)
            {
                rx = _dataInLora[index];
                //if error
                if (_dataInLora[index].Length > 5)
                {

                    //if receive data
                    if (rx.Substring(0, 6) == "mac_rx")
                    {
                        string hex = _dataInLora[index].Substring(9);

                        //update display

                        byte[] data = StringToByteArrayFastest(hex);
                        string decoded = new String(UTF8Encoding.UTF8.GetChars(data));
                        Debug.WriteLine("decoded:" + decoded);
                        //txtMessage.Text = decoded;//Unpack(hex);

                    }
                }
            }
            Debug.WriteLine(rx);
        }

        public static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        static SimpleSerial UART = null;

        static void PrintToLcd(string Message)
        {
            //update display
            //txtTime.Text = DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss");
            //txtMessage.Text = "Data Transmitted Successfully.";
            //txtTime.Invalidate();
            //txtMessage.Invalidate();
            //window.Invalidate();
        }



        static void sendCmd(string cmd)
        {
            byte[] rx_data = new byte[20];
            Debug.WriteLine(cmd);
            Debug.WriteLine("\n");
            // flush all data
            //UART.Port.Flush();
            // send some data
            var tx_data = Encoding.UTF8.GetBytes(cmd);
            UART.Port.Write(tx_data, 0, tx_data.Length);
            tx_data = Encoding.UTF8.GetBytes("\r\n");
            UART.Port.Write(tx_data, 0, tx_data.Length);
            Thread.Sleep(100);
            while (!UART.Port.IsOpen)
            {
                UART.Open();
                Thread.Sleep(100);
            }
            if (UART.Port.BytesToRead>0)
            {
                var count = UART.Port.Read(rx_data, 0, rx_data.Length);
                if (count > 0)
                {
                    Debug.WriteLine("count cmd:" + count);
                    var hasil = new string(System.Text.Encoding.UTF8.GetChars(rx_data));
                    Debug.WriteLine("read cmd:" + hasil);
                }
            }
        }

        static void waitForResponse()
        {
            byte[] rx_data = new byte[20];

            while (!UART.Port.IsOpen)
            {
                UART.Open();
                Thread.Sleep(100);
            }
            if (UART.Port.BytesToRead>0)
            {
                var count = UART.Port.Read(rx_data, 0, rx_data.Length);
                if (count > 0)
                {
                    Debug.WriteLine("count res:" + count);
                    var hasil = new string(System.Text.Encoding.UTF8.GetChars(rx_data));
                    Debug.WriteLine("read res:" + hasil);
                }

            }
        }
        public static string Unpack(string input)
        {
            byte[] b = new byte[input.Length / 2];

            for (int i = 0; i < input.Length; i += 2)
            {
                b[i / 2] = (byte)((FromHex(input[i]) << 4) | FromHex(input[i + 1]));
            }
            return new string(Encoding.UTF8.GetChars(b));
        }
        public static int FromHex(char digit)
        {
            if ('0' <= digit && digit <= '9')
            {
                return (int)(digit - '0');
            }

            if ('a' <= digit && digit <= 'f')
                return (int)(digit - 'a' + 10);

            if ('A' <= digit && digit <= 'F')
                return (int)(digit - 'A' + 10);

            throw new ArgumentException("digit");
        }

        static char getHexHi(char ch)
        {
            int nibbleInt = ch >> 4;
            char nibble = (char)nibbleInt;
            int res = (nibble > 9) ? nibble + 'A' - 10 : nibble + '0';
            return (char)res;
        }
        static char getHexLo(char ch)
        {
            int nibbleInt = ch & 0x0f;
            char nibble = (char)nibbleInt;
            int res = (nibble > 9) ? nibble + 'A' - 10 : nibble + '0';
            return (char)res;
        }

        static void sendData(string msg)
        {
            byte[] rx_data = new byte[20];
            char[] data = msg.ToCharArray();
            Debug.WriteLine("mac tx uncnf 1 ");
            var tx_data = Encoding.UTF8.GetBytes("mac tx uncnf 1 ");
            UART.Port.Write(tx_data, 0, tx_data.Length);

            // Write data as hex characters
            foreach (char ptr in data)
            {
                tx_data = Encoding.UTF8.GetBytes(new string(new char[] { getHexHi(ptr) }));
                UART.Port.Write(tx_data, 0, tx_data.Length);
                tx_data = Encoding.UTF8.GetBytes(new string(new char[] { getHexLo(ptr) }));
                UART.Port.Write(tx_data, 0, tx_data.Length);


                Debug.WriteLine(new string(new char[] { getHexHi(ptr) }));
                Debug.WriteLine(new string(new char[] { getHexLo(ptr) }));
            }
            tx_data = Encoding.UTF8.GetBytes("\r\n");
            UART.Port.Write(tx_data, 0, tx_data.Length);
            Debug.WriteLine("\n");
            Thread.Sleep(5000);

            if (UART.Port.BytesToRead>0)
            {
                var count = UART.Port.Read(rx_data, 0, rx_data.Length);
                if (count > 0)
                {
                    Debug.WriteLine("count after:" + count);
                    var hasil = new string(System.Text.Encoding.UTF8.GetChars(rx_data));
                    Debug.WriteLine("read after:" + hasil);
                }
            }
        }
    }

    public class SensorData
    {
        public double WindSpeedAverage { get; set; }
        public double WindDirection { get; set; }
        public double WindSpeedMax { get; set; }
        public double RainfallOneHour { get; set; }
        public double RainfallOneDay { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double BarPressure { get; set; }
    }
}
