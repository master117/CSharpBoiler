using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SteamKit2.GC.CSGO.Internal;

namespace CSharpBoiler.NetworkHelper
{
    static class VACStat_usSender
    {
        private static bool initialized = false;
        private static CMsgGCCStrike15_v2_MatchList mainMatchList;
        private static long steamID;

        private const string APIURI = "http://vacstat.us/api/v1/list";
        private const string APIMatchLink = "DemoUrl=";
        private const string APIMatchDate = "MatchDate=";
        private const string APIEvent = "Event=";
        private const string APISeperator = "&";

        internal static void Initialize(SteamKit2.GC.CSGO.Internal.CMsgGCCStrike15_v2_MatchList tempMatchList, long tempSteamID)
        {
            mainMatchList = tempMatchList;
            steamID = tempSteamID;
            initialized = true;
        }

        public static List<string> GetLists(string APIKey)
        {
            if (!initialized)
                return null;

            using (WebClient client = new WebClient())
            {
                try
                {
                    string tempUri = APIURI;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    var dataString = "{_key:"+ APIKey +"}";

                    string response = client.UploadString(tempUri, "POST", dataString);

                    Console.WriteLine(response);
                }
                catch (WebException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return null;
        }

        public static void Send()
        {
            if (!initialized)
                return;


        }


    }
}
