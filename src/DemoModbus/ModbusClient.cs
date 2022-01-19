using nanoFramework.Modbus;
using System;
using System.Diagnostics;
using System.Text;

namespace DemoModbus
{
    public class ModbusClient : ModbusDevice
    {
        public ModbusClient(byte deviceAddress, object syncObject = null)
           : base(deviceAddress, syncObject) { }

        protected override string OnGetDeviceIdentification(ModbusObjectId objectId)
        {
            switch (objectId)
            {
                case ModbusObjectId.VendorName:
                    return "GHI Electronics";
                case ModbusObjectId.ProductCode:
                    return "101";
                case ModbusObjectId.MajorMinorRevision:
                    return "1.0";
                case ModbusObjectId.VendorUrl:
                    return "ghielectronics.com";
                case ModbusObjectId.ProductName:
                    return "SitCore";
                case ModbusObjectId.ModelName:
                    return "SCM20260D";
                case ModbusObjectId.UserApplicationName:
                    return "Modbus Slave Test";
            }
            return null;
        }

        protected override ModbusConformityLevel GetConformityLevel()
        {
            return ModbusConformityLevel.Regular;
        }

        protected override ModbusErrorCode OnReadCoils(bool isBroadcast, ushort startAddress, ushort coilCount, byte[] coils)
        {
            try
            {
                for (int n = 0; n < coilCount; ++n)
                {
                    coils[n] = 1;
                }
                Debug.WriteLine("Master read coils");
                return ModbusErrorCode.NoError;
            }
            catch
            {
                Debug.WriteLine("error in on read coils registers");
                return base.OnReadCoils(isBroadcast, startAddress, coilCount, coils);
            }
        }

        protected override ModbusErrorCode OnReadHoldingRegisters(bool isBroadcast, ushort startAddress, ushort[] registers)
        {
            try
            {
                for (int n = 0; n < registers.Length; ++n)
                {
                    registers[n] = 65530; // set number in each register for testing               
                }
                Debug.WriteLine("Master Read Holding Registers - " + registers[0].ToString());
                return ModbusErrorCode.NoError;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("error in on read holding registers");
            }
            return base.OnReadHoldingRegisters(isBroadcast, startAddress, registers);
        }
    }


    // override On<ModusFunction> methods here

}
