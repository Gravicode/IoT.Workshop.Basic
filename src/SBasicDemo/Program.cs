using nanoFramework.Intepreter;
using System;
using System.Diagnostics;
using System.Threading;

namespace SBasicDemo
{
    public class Program
    {
        public static void Main()
        {
            SBASIC basic = new SBASIC();
            basic.Print += Basic_Print;
            basic.ClearScreen += Basic_ClearScreen;

            var test1 = Resources.GetString(Resources.StringResources.TEST);
            basic.Run(test1);
            Thread.Sleep(1000);

            var test2 = Resources.GetString(Resources.StringResources.TEST2);
            basic.Run(test2);
            Thread.Sleep(1000);

            var test3 = Resources.GetString(Resources.StringResources.TEST3);
            basic.Run(test3);
            Thread.Sleep(1000);

            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }

        private static void Basic_ClearScreen(SBASIC sender)
        {
            Debug.WriteLine("clear screen is called");
            //throw new NotImplementedException();
        }

        private static void Basic_Print(SBASIC sender, string value)
        {
            Debug.WriteLine(value);
        }
    }
}
