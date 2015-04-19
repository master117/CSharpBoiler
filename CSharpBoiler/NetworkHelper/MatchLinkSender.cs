using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SteamKit2.GC.CSGO.Internal;
using System.IO;
using CSharpBoiler.Models;

namespace CSharpBoiler.NetworkHelper
{
    class MatchLinkSender
    {
        public List<UploadedDemoData> uploadedDemoDataList = new List<UploadedDemoData>();

        private const string APIURI = "http://localhost:61663/api/MatchLink?"; //"Http://www.boiler-stats.com/api/MatchLink?";
        private const string APIMatchLink = "DemoUrl=";
        private const string APIMatchDate = "MatchDate=";
        private const string APIEvent = "Event=";
        private const string APISeperator = "&";

        private const string filename = "UploadedDemoData.xml";

        public MatchLinkSender(SteamKit2.GC.CSGO.Internal.CMsgGCCStrike15_v2_MatchList mainMatchList)
        {
            DeSerializeDemoData();

            foreach (var match in mainMatchList.matches)
            {
                if (uploadedDemoDataList.FirstOrDefault(x => x.demoUrl == match.roundstats.map) != null)
                    continue;

                UploadedDemoData uploadedDemoData = new UploadedDemoData();
                uploadedDemoData.demoUrl = match.roundstats.map;
                uploadedDemoData.matchDate = Convert.ToString(match.matchtime);
                uploadedDemoData.eventString = "";

                uploadedDemoDataList.Add(uploadedDemoData);
            }
        }

        //Uploads MatchLink to Server, return the Number of successfully uploaded MatchLinks
        internal int Send()
        {
            int uploadedMatchLinksCounter = 0;

            using (WebClient client = new WebClient())
            {
                foreach (var demoData in uploadedDemoDataList)
                {
                    if (demoData.isSend)
                    {
                        uploadedMatchLinksCounter++;
                        continue;
                    }

                    try
                    {
                        string tempUri = APIURI
                                         + APIMatchLink + demoData.demoUrl + APISeperator
                                         + APIMatchDate + demoData.matchDate + APISeperator
                                         + APIEvent + demoData.eventString;
                        byte[] response = client.UploadValues(tempUri, "POST", new NameValueCollection());

                        var a = client.ResponseHeaders[client.ResponseHeaders.Count - 1];
                        string result = System.Text.Encoding.UTF8.GetString(response);

                        Console.WriteLine(result);

                        demoData.isSend = true;
                        uploadedMatchLinksCounter++;
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
            }

            SerializeDemoData();

            return uploadedMatchLinksCounter;
        }

        private void DeSerializeDemoData()
        {
            if (File.Exists(filename))
            {
                try
                {
                    System.Xml.Serialization.XmlSerializer xmlReader =
                    new System.Xml.Serialization.XmlSerializer(typeof(List<UploadedDemoData>));

                    System.IO.StreamReader streamReader = new System.IO.StreamReader(filename);

                    uploadedDemoDataList = (List<UploadedDemoData>)xmlReader.Deserialize(streamReader);

                    streamReader.Close();
                }
                catch (Exception)
                {
                    
                    throw;
                }
            }
        }

        private void SerializeDemoData()
        {
            try
            {
                XmlSerializer xmlWriter = new XmlSerializer(typeof (List<UploadedDemoData>));

                StreamWriter additionalDemoDataStreamWriter = new StreamWriter(filename);

                xmlWriter.Serialize(additionalDemoDataStreamWriter, uploadedDemoDataList);
                additionalDemoDataStreamWriter.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
