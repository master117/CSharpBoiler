using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CSharpBoiler.Helpers;
using CSharpBoiler.Models;
using CSharpBoiler.Properties;
using Newtonsoft.Json.Linq;
using SteamKit2.GC.CSGO.Internal;

namespace CSharpBoiler.NetworkHelper
{
    static class VACStat_usSender
    {
        private static bool initialized = false;
        private static HashSet<long> steamIdsHashSet;

        private const string APIURI = "http://vacstat.us/api/v1/";
        private const string APIURIGETLISTS = "list?_key=";
        private const string APIADDMANY = "list/add/many";

        //VACStat.us WebApi only supports ~100 steamids per request
        const int NumberOfIDsPerRequest = 80;

        internal static void Initialize(HashSet<long> tempSteamIdsList)
        {
            steamIdsHashSet = tempSteamIdsList;          

            //TODO: decide if outsend really should be a part of initialize, 
            //i guess it shouldn't, but that would mean i have to autostartn upload in a different way
            if (Properties.Settings.Default.UploadToVacStats_us && !String.IsNullOrEmpty(Properties.Settings.Default.VACStat_usKey) && Properties.Settings.Default.VACStat_usListId != 0)
                Send(Properties.Settings.Default.VACStat_usKey, Properties.Settings.Default.VACStat_usListId);

            initialized = true;
        }

        public static Dictionary<int, string> GetListsAsDictionary(string APIKey)
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
                    ConsoleManager.Show();
                    Console.WriteLine(e.Message + Resources.VACStat_usSender_GetLists_Problem_with_VacStat_us);
                }
                catch (Exception e)
                {
                    ConsoleManager.Show();
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
                try
                {
                    int numberOfRuns = (int)Math.Ceiling((double)steamIdsHashSet.Count / (double)NumberOfIDsPerRequest);

                    for (int i = 0; i < numberOfRuns; i++)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(APIURI + APIADDMANY + "?_key=" + APIKey + "&list_id=" + listId.ToString() + "&search=");

                        for (int j = 0; j < NumberOfIDsPerRequest; j++)
                        {
                            if (steamIdsHashSet.Count <= i * NumberOfIDsPerRequest + j)
                                break;

                            sb.Append(steamIdsHashSet.ElementAt(i * NumberOfIDsPerRequest + j) + ",");
                        }
                        sb.Remove(sb.Length - 1, 1);

                        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        var response = client.UploadValues(sb.ToString(), "POST", new NameValueCollection());
                    }

                    //string result = System.Text.Encoding.UTF8.GetString(response);
                    //ConsoleManager.Show();
                    //Console.WriteLine(result);
                }
                catch (Exception e)
                {
                    Properties.Settings.Default.VACStat_usKey = "";
                    Properties.Settings.Default.VACStat_usListId = 0;

                    ConsoleManager.Show();
                    Console.WriteLine(e.Message + Resources.VACStat_usSender_Send_Problem_with_VacStat_us);
                    Console.WriteLine(Resources.VACStat_usSender_Send_Removed_APIKey_and_ListId_to_ensure_security_and_stability_);
                }
            };
        }
    }
}
