using nanoFramework.SDCard;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace DemoMicroSD
{
    public class Program
    {
        static void Main()
        {
            TestNewApi();
            Thread.Sleep(-1);
            //TestOldApi();
        }

        static void TestNewApi()
        {
            //esp32 pin12 dan 13
            var drive = FileSystem.Mount(1,nanoFramework.Hardware.Esp32.Gpio.IO12,nanoFramework.Hardware.Esp32.Gpio.IO13);

            //Show a list of files in the root directory
            var directory = new DirectoryInfo(drive.Name);
            var subdir = directory.CreateSubdirectory("data");
            var dirs = directory.GetDirectories();

            var files = subdir.GetFiles();
            foreach (var f in files)
            {
                System.Diagnostics.Debug.WriteLine(f.FullName);
                //f.Delete();
            }

            //Create a text file and save it to the SD card.
            //var file = new FileStream($@"{drive.Name}Test1.txt", FileMode.OpenOrCreate);
            //var i = 4;
            for (int i = 1; i < 3; i++)
            {
                var file = new FileStream($@"{subdir.FullName}\\Test{i}.txt", FileMode.OpenOrCreate);
                var bytes = Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString() +
                    '\n');

                file.Write(bytes, 0, bytes.Length);

                file.Flush();

                //file.Dispose();

            }
            //FileSystem.Flush();
        }
    }
}
