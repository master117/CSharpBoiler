using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CSharpBoiler.Helpers;

namespace CSharpBoiler
{
    static class BoilerHandler
    {
        public static long StartAndGetAccountId()
        {
            try
            {
                Process p = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        FileName = "BoilerFetcher.exe"
                    }
                };
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                string matchedAccountIdString = Regex.Match(output, "[0-9]*").ToString();
                long accountId = 0;
                long.TryParse(matchedAccountIdString, out accountId);

                return accountId;
            }
            catch (Exception e)
            {
                ConsoleManager.Show();
                Console.WriteLine(e.Message);
            }

            return 0;
        }
    }
}
