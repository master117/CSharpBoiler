using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CSharpBoiler.Models;
using Newtonsoft.Json.Linq;
using SteamKit2.GC.CSGO.Internal;

namespace CSharpBoiler.NetworkHelper
{
    static class VACStat_usSender
    {
        private static bool initialized = false;
        private static HashSet<string> steamIdsHashSet;

        private const string APIURI = "http://vacstat.us/api/v1/";
        private const string APIURIGETLISTS = "list?_key=";
        private const string APIADDMANY = "list/add/many";

        internal static void Initialize(HashSet<string> tempSteamIdsList)
        {
            steamIdsHashSet = tempSteamIdsList;
            initialized = true;
        }

        public static Dictionary<int, string> GetLists(string APIKey)
        {
            if (!initialized)
                return null;

            using (WebClient client = new WebClient())
            {
                try
                {
                    string tempUri = APIURI + APIURIGETLISTS + APIKey;

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(tempUri);
                    request.Method = "GET";
                    String responseString = String.Empty;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        responseString = reader.ReadToEnd();
                        reader.Close();
                        if(dataStream != null)
                            dataStream.Close();
                    }

                    var jsonData = (JArray)JObject.Parse(responseString)["my_list"];
                    Dictionary<int, string> userListDictionary = new Dictionary<int, string>();

                    for (int i = 0; i < jsonData.Count; i++)
                        userListDictionary.Add((int)jsonData[i]["id"], (string)jsonData[i]["title"]);
                    
                    return userListDictionary;
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

        public static void Send(string APIKey, int listId)
        {
            if (!initialized)
                return;

            using (var client = new WebClient())
            {
                string steamIds = "";
                foreach (var steamIdHashSetEntry in steamIdsHashSet)
                {
                    steamIds += steamIdHashSetEntry + ",";
                }
                steamIds = steamIds.Substring(1, steamIds.Length - 1);


                string apiParameters = "?_key=" + APIKey + "&list_id=" + listId.ToString() + "&search=" + steamIds;
                string targetUrl = APIURI + APIADDMANY + apiParameters;

                /*
                NameValueCollection  nameValueCollection = new NameValueCollection()
                {
                    { "_key", APIKey },
                    { "list_id", listId.ToString() },
                    {"search", steamIds}
                };
                */

                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var response = client.UploadValues(targetUrl, "POST", new NameValueCollection());

                string result = System.Text.Encoding.UTF8.GetString(response);

                Console.WriteLine(result);
            };
        }
    }
}
