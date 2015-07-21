using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CSharpBoiler.Helpers;
using CSharpBoiler.Models;
using CSharpBoiler.Properties;

namespace CSharpBoiler.NetworkHelper
{
    class VacWatch_nlSender
    {
        private static bool initialized = false;
        private static List<VacWatch_nlUploadedData> vacWatch_nlUploadedDataList = new List<VacWatch_nlUploadedData>();
        private static long userSteamId;
        private const string filename = "VacWatch_nlUploadedData.xml";

        private const string APIURI = "http://vacwatch.nl/api/record/add";

        //VacWatch WebApi only supports 50 steamids per request
        const int NumberOfIDsPerRequest = 50;

        //Prepare VacWatchUploadedDataList
        internal static void Initialize(HashSet<long> tempSteamIdsList, long tempUserSteamId)
        {
            //First Deserialize old Data
            DeSerializeVacWatch_nlUploadedData();

            //Add new SteamIds
            foreach (var tempSteamId in tempSteamIdsList)
            {
                if (vacWatch_nlUploadedDataList.FirstOrDefault(x => x.steamId == tempSteamId) != null)
                    continue;

                VacWatch_nlUploadedData vacWatch_nlUploadedData = new VacWatch_nlUploadedData();
                vacWatch_nlUploadedData.steamId = tempSteamId;
                vacWatch_nlUploadedData.isSend = false;
                vacWatch_nlUploadedDataList.Add(vacWatch_nlUploadedData);
            }

            //Set userSteamId and Mark as Initialized
            userSteamId = tempUserSteamId;
            initialized = true;

            //Todo: remove
            if (Properties.Settings.Default.UploadToVacWatch_nl)
                Send();
        }

        //Upload all steamIds to VacWatch.nl
        public static void Send()
        {
            if (!initialized)
                return;

            using (var client = new WebClient())
            {
                try
                {
                    //Calculate Number of Runs based on Number of Steamids and MaxNumber of SteamIds per Upload
                    int numberOfRuns = (int)Math.Ceiling((double)vacWatch_nlUploadedDataList.Count / (double)NumberOfIDsPerRequest);

                    for (int i = 0; i < numberOfRuns; i++)
                    {
                        //Adding all SteamIds that weren't already uploaded to a single string
                        StringBuilder sb = new StringBuilder();

                        for (int j = 0; j < NumberOfIDsPerRequest; j++)
                        {
                            if (vacWatch_nlUploadedDataList.Count <= i * NumberOfIDsPerRequest + j)
                                break;

                            if (vacWatch_nlUploadedDataList.ElementAt(i * NumberOfIDsPerRequest + j).isSend == false)
                                sb.Append(vacWatch_nlUploadedDataList.ElementAt(i * NumberOfIDsPerRequest + j).steamId.ToString() + ",");
                        }
                        //If we added no steamId at all, because they all were already uploaded: skip
                        if (sb.Length <= 0)
                            continue;
                        //Else, remove last ','
                        sb.Remove(sb.Length - 1, 1);                 

                        //Create NameValue Collection which is the main Object our Post is uploading
                        NameValueCollection nameValueCollection = new NameValueCollection();
                        nameValueCollection.Add("records", sb.ToString());
                        nameValueCollection.Add("userid", userSteamId.ToString());

                        //Upload SteamIds
                        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        var response = client.UploadValues(APIURI, "POST", nameValueCollection);
                        string result = System.Text.Encoding.UTF8.GetString(response);
                        
                        //We simply assume that the Upload was correct if we get no Exception during upload,
                        //this is wrong but i won't have to adapt to API changes
                        //Mark all Uploaded SteamIds so we don't upload them again
                        for (int j = 0; j < NumberOfIDsPerRequest; j++)
                        {
                            if (vacWatch_nlUploadedDataList.Count <= i * NumberOfIDsPerRequest + j)
                                break;

                            vacWatch_nlUploadedDataList.ElementAt(i*NumberOfIDsPerRequest + j).isSend = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    Properties.Settings.Default.UploadToVacWatch_nl = false;
                    ConsoleManager.Show();
                    Console.WriteLine(e.Message + Resources.VacWatch_nlSender_Send_Problem_with_VacWatch_nl);
                }

                SerializeVacWatch_nlUploadedData();
            };
        }

        private static void DeSerializeVacWatch_nlUploadedData()
        {
            if (File.Exists(filename))
            {
                try
                {
                    System.Xml.Serialization.XmlSerializer xmlReader =
                    new System.Xml.Serialization.XmlSerializer(typeof(List<VacWatch_nlUploadedData>));

                    System.IO.StreamReader streamReader = new System.IO.StreamReader(filename);

                    vacWatch_nlUploadedDataList = (List<VacWatch_nlUploadedData>)xmlReader.Deserialize(streamReader);

                    streamReader.Close();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private static void SerializeVacWatch_nlUploadedData()
        {
            try
            {
                XmlSerializer xmlWriter = new XmlSerializer(typeof(List<VacWatch_nlUploadedData>));

                StreamWriter streamWriter = new StreamWriter(filename);

                xmlWriter.Serialize(streamWriter, vacWatch_nlUploadedDataList);
                streamWriter.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
