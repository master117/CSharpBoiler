using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProtoBuf;
using System.IO;
using Microsoft.Win32;
using System.Net;
using System.ComponentModel;
using ICSharpCode.SharpZipLib.BZip2;

namespace CSharpBoiler
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<MatchData> matchDataList = new List<MatchData>();
        private DemoComments demoComments = new DemoComments();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainListView.ItemsSource = matchDataList;

            //DemoCommentsDeserialization
            if (File.Exists("DemoComments.xml"))
            {
                System.Xml.Serialization.XmlSerializer xmlReader =
                new System.Xml.Serialization.XmlSerializer(typeof(DemoComments));
                System.IO.StreamReader streamReader = new System.IO.StreamReader(
                    "DemoComments.xml");
                demoComments = (DemoComments)xmlReader.Deserialize(streamReader);
            }

            FillInData();
        }

        public void FillInData()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "dat files (*.dat)|*.dat";
            openFileDialog.ShowDialog();
            FileStream dataFileStream = File.Open(openFileDialog.FileName, FileMode.Open);
            string steamID = openFileDialog.SafeFileName.Split('_')[0];

            CMsgGCCStrike15_v2_MatchList.Builder matchListBuilder = new CMsgGCCStrike15_v2_MatchList.Builder();
            CMsgGCCStrike15_v2_MatchList matchlist = matchListBuilder.MergeFrom(dataFileStream).Build();

            ParseMatchData(matchlist, steamID);

            matchDataList.Reverse();
            ColorDownloadedDemos();
        }


        public void UpdateMatchList()
        {
            ProcessStartInfo boilerProcessStartInfo = new ProcessStartInfo("Boiler\\Boiler.exe");

            Process boilerProcess = Process.Start(boilerProcessStartInfo);
            boilerProcess.Close();
        }



        //
        // Databinding of the Table
        //

        //Parses The Data from MatchList to the UI Model
        public void ParseMatchData(CMsgGCCStrike15_v2_MatchList matchlist, string steamID)
        {
            for(int i = 0; i < matchlist.MatchesCount; i++)
            {
                //Creating new MatchData
                MatchData matchData = new MatchData();
                //Date
                matchData.Date = UnixTimeStampToDateTime(matchlist.GetMatches(i).Matchtime).ToString("yyyy-MM-dd hh:mm");
                //Overall Score, ex. 16:13
                matchData.Result = matchlist.GetMatches(i).Roundstats.GetTeamScores(0).ToString() + ":" + matchlist.GetMatches(i).Roundstats.GetTeamScores(1).ToString();
                //Finding the Position of our own entries
                int j = 0;
                for (; j < matchlist.GetMatches(i).Roundstats.Reservation.AccountIdsCount; j++)
                {
                    if (matchlist.GetMatches(i).Roundstats.Reservation.GetAccountIds(j).ToString() == steamID)
                        break;
                }
                //Kills, Assists, Deaths, MVP, Score
                matchData.Kills = matchlist.GetMatches(i).Roundstats.GetKills(j);
                matchData.Assists = matchlist.GetMatches(i).Roundstats.GetAssists(j);
                matchData.Deaths = matchlist.GetMatches(i).Roundstats.GetDeaths(j);
                matchData.MVPs = matchlist.GetMatches(i).Roundstats.GetMvps(j);
                matchData.Score = matchlist.GetMatches(i).Roundstats.GetScores(j);
                matchData.KD = ((double)((int)(((double)matchData.Kills / matchData.Deaths)*100))/100);
                matchData.Demo = matchlist.GetMatches(i).Roundstats.Map; //new DemoButtonUserControl(matchlist.GetMatches(i).Roundstats.Map);
                if (demoComments.Contains(matchData.Demo))
                {
                    matchData.DemoComment = demoComments.Get(matchData.Demo);
                }


                //Calculating if we won the match
                matchData.Won = (matchlist.GetMatches(i).Roundstats.GetTeamScores(0) > matchlist.GetMatches(i).Roundstats.GetTeamScores(1) && j < (double)matchlist.GetMatches(i).Roundstats.Reservation.AccountIdsCount / 2
                                || matchlist.GetMatches(i).Roundstats.GetTeamScores(0) < matchlist.GetMatches(i).Roundstats.GetTeamScores(1) && j > (double)matchlist.GetMatches(i).Roundstats.Reservation.AccountIdsCount / 2);
                     

                //adding to the MainList of MatchData
                matchDataList.Add(matchData);
            }
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        #region DemoDownload
        /// 
        /// Demo Download Section
        /// 

        public void ColorDownloadedDemos()
        {
            foreach(MatchData tempMatchData in MainListView.Items)
            {
                string[] tempURLSplit = tempMatchData.Demo.Split('/');
                string tempDemoFileName = tempURLSplit[tempURLSplit.Length - 1];
                if (File.Exists("Demos/" + tempDemoFileName))
                {
                    //MainListView
                }
            }
        }

        private void DemoButton_Click(object sender, RoutedEventArgs e)
        {
            Button tempButton = (Button)sender;
            string URL = (string)tempButton.Content;

            string[] tempURLSplit = URL.Split('/');
            string tempDemoFileName = tempURLSplit[tempURLSplit.Length - 1];

            if (!Directory.Exists("Demos"))
                Directory.CreateDirectory("Demos");

            if (!File.Exists("Demos/" + tempDemoFileName))
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                webClient.DownloadFileAsync(new Uri(URL), "Demos/" + tempDemoFileName);
                tempButton.Background = Brushes.LightGreen;
            }
            else
            {
                MessageBox.Show("Demo is already Downloaded");
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //DemoButtonProgressbar.Value = e.ProgressPercentage;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            ColorDownloadedDemos();
            unzipDemos();
            MessageBox.Show("Download completed! Unzipping completed!");
        }

        private void unzipDemos()
        {
            if (Directory.Exists("Demos"))
            {
                foreach (string tempFile in Directory.GetFiles("Demos"))
                {
                    if (File.Exists(tempFile) && tempFile.Substring(tempFile.Length - 4, 4) == ".bz2" && !File.Exists(tempFile.Substring(0, tempFile.Length - 4)))
                    {
                        FileStream inputStream = File.Open(tempFile, FileMode.Open);
                        //- .bz2
                        FileStream outputStream = File.Create(tempFile.Substring(0, tempFile.Length - 4));
                        BZip2.Decompress(inputStream, outputStream, true);
                    }
                }
            }
        }

        #endregion


        //
        // Demo Comment Serialization
        //

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //DemoCommentsSerialization
            ExtractDemoComments();

            System.Xml.Serialization.XmlSerializer xmlWriter =
            new System.Xml.Serialization.XmlSerializer(typeof(DemoComments));

            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(
            "DemoComments.xml");
            xmlWriter.Serialize(streamWriter, demoComments);

            streamWriter.Close();
        }

        private void ExtractDemoComments()
        {
            foreach(var a in MainGridView.Columns)
            {
                
            }

            foreach (MatchData tempMatchData in MainListView.Items)
            {
                if(tempMatchData.DemoComment != null)
                {
                    demoComments.AddComment(tempMatchData.Demo, tempMatchData.DemoComment);
                }
            }
        }
    }
}
