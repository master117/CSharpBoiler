using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpBoilerAutoStarter
{
    class Program
    {
        public const string CSHARPBOILEREXE = "CSharpBoiler.exe";

        public enum CSGOStatus
        {
            Unknown,
            NotRunning,
            Running,
            Ended
        };

        static void Main(string[] args)
        {
            #region Using Polling:
            
            CSGOStatus status = CSGOStatus.Unknown;

            //Alternative: pname[0].WaitForExit();

            while (true)
            {
                Process[] pname = Process.GetProcessesByName("csgo");

                if (pname.Length == 0)
                {
                    if (status == CSGOStatus.Running)
                    {
                        if (status != CSGOStatus.Ended)
                        {
                            status = CSGOStatus.Ended;
                            Console.WriteLine("CSGO has terminated, starting CSharpBoiler");

                            ProcessStartInfo processStartInfo = new ProcessStartInfo(CSHARPBOILEREXE);

                            Process.Start(processStartInfo);
                        }
                    }
                    else
                    {
                        if (status != CSGOStatus.NotRunning)
                        {
                            status = CSGOStatus.NotRunning;
                            Console.WriteLine("CSGO not running");
                        }
                    }                    
                }
                else
                {
                    if (status != CSGOStatus.Running)
                    {
                        status = CSGOStatus.Running;
                        Console.WriteLine("CSGO is running");
                    }
                }

                Thread.Sleep(10000);
            }
            
            #endregion
        }
    }


}
