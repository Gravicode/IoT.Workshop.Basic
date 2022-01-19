using nanoFramework.Modbus;
using nanoFramework.Modbus.Interface;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace DemoModbus
{
    public class Program
    {
        public static void Main()
        {
            //demo modbus RTU
            DemoModbusRTU();

            //demo modbus IP (greater distance)
            //DemoModbusIP();

            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
        static void DemoModbusIP()
        {
            ModbusDevice ModbusTCP_Device;
            ModbusTCP_Device = new ModbusClient(248);
            ModbusTcpListener mbListner;
            mbListner = new ModbusTcpListener(ModbusTCP_Device, 502, 5, 1000);
            Thread.Sleep(100);
            ModbusTCP_Device.Start();

        }
        static void DemoModbusRTU()
        {
            var ComPort = "COM1";
            var serial = new SerialPort(ComPort,19200, Parity.None,8,StopBits.One );
            /*
            var uartSetting = new UartSetting()
            {
                BaudRate = 19200,
                DataBits = 8,
                Parity = UartParity.None,
                StopBits = UartStopBitCount.One,
                Handshaking = UartHandshake.None,
            };
            
            serial.SetActiveSettings(uartSetting);
            serial.Enable();

             */

            IModbusInterface mbInterface;
            mbInterface = new ModbusRtuInterface(
                serial,
                19200,
                8,
                StopBits.One,
                Parity.None);

            ModbusMaster mbMaster;
            mbMaster = new ModbusMaster(mbInterface);

            var mbTimeout = false;

            ushort[] reply = null;
            int count = 0;

            while (true)
            {
                try
                {
                    mbTimeout = false;

                    reply = mbMaster.ReadHoldingRegisters(10, 0, 1, 3333);
                    count++;

                    if (count == 5)
                        break;
                }
                catch (Exception error)
                {
                    Debug.WriteLine("Modbus Timeout");
                    mbTimeout = true;
                }

                if (!mbTimeout)
                {
                    Debug.WriteLine("Modbus : " + (object)reply[0].ToString());
                }

                Thread.Sleep(1000);
            }

        }
    }
}
