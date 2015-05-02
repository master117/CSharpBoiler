using System;

namespace CSharpBoiler.Helpers
{
    public static class TimeHelper
    {
        public static DateTime UnixTimeStampInSecondsToDateTime(long unixTimeStamp)
        {
            try
            {
                // Unix timestamp is seconds past epoch
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds((double)unixTimeStamp).ToUniversalTime();
                return dtDateTime;
            }
            catch (ArgumentOutOfRangeException e)
            {    
                ConsoleManager.Show();
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
