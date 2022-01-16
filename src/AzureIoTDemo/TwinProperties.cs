using System;
using System.Text;

namespace AzureIoTDemo
{
    public class TwinProperties
    {
        public Desired desired { get; set; }
        public Reported reported { get; set; }
    }

    public class Desired
    {
        public int TimeToSleep { get; set; }
    }

    public class Reported
    {
        public string Firmware { get; set; }

        public int TimeToSleep { get; set; }
    }

}