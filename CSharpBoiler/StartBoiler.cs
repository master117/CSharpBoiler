using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSharpBoiler
{
    static class StartBoiler
    {
        public static int StartAndGetSteamID()
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "BoilerFetcher.exe";
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            Console.WriteLine(output);

            string matchedSteamIdString = Regex.Match(output, "[0-9]*").ToString();
            int steamID = 0;

            try
            {
                int.TryParse(matchedSteamIdString, out steamID);
            }
            catch (Exception)
            {
                
                throw;
            }

            return steamID;
        }
    }
}
