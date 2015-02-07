/*
Boiler
Copyright (C) 2015  Johannes Gocke

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/


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
using System.Collections.ObjectModel;

namespace CSharpBoiler
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<MatchData> matchDataList = new ObservableCollection<MatchData>();
        private AdditionalDemoData additionalDemoData = new AdditionalDemoData();
        private string steamID;
        private const string DEMOFOLDER = "Demos/";

        public MainWindow()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainDataGrid.ItemsSource = matchDataList;
            System.Diagnostics.PresentationTraceSources.SetTraceLevel(MainDataGrid.ItemContainerGenerator, System.Diagnostics.PresentationTraceLevel.High);
            //MainListView.ItemsSource = matchDataList;

            //DemoCommentsDeserialization
            if (Directory.Exists(DEMOFOLDER))
            {
                if (File.Exists("Demos/AdditionalDemoData.xml"))
                {
                    System.Xml.Serialization.XmlSerializer xmlReader =
                    new System.Xml.Serialization.XmlSerializer(typeof(AdditionalDemoData));
                    System.IO.StreamReader streamReader = new System.IO.StreamReader(
                        "Demos/AdditionalDemoData.xml");
                    additionalDemoData = (AdditionalDemoData)xmlReader.Deserialize(streamReader);

                    streamReader.Close();
                }
            }

            //UpdateMatchList
            FillInData();
        }

        public void UpdateMatchList()
        {
            ProcessStartInfo boilerProcessStartInfo = new ProcessStartInfo("Boiler\\Boiler.exe");

            Process boilerProcess = Process.Start(boilerProcessStartInfo);
            boilerProcess.Close();
        }

        public void FillInData()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "dat files (*.dat)|*.dat";
                openFileDialog.ShowDialog();
                FileStream dataFileStream;

                dataFileStream = File.Open(openFileDialog.FileName, FileMode.Open);
                steamID = openFileDialog.SafeFileName.Split('_')[0];

                CMsgGCCStrike15_v2_MatchList.Builder matchListBuilder = new CMsgGCCStrike15_v2_MatchList.Builder();
                CMsgGCCStrike15_v2_MatchList matchlist = matchListBuilder.MergeFrom(dataFileStream).Build();

                dataFileStream.Close();

                ParseMatchData(matchlist, steamID);
            }
            catch (Exception e)
            {
                this.Close();
            }
        }

        #region MatchData Parrsing
        //
        // Databinding of the Table
        //

        //Parses The Data from MatchList to the UI Model
        public void ParseMatchData(CMsgGCCStrike15_v2_MatchList matchlist, string steamID)
        {
            for (int i = matchlist.MatchesCount - 1; i >= 0; i--)
            {
                //Creating new MatchData
                MatchData matchData = new MatchData();
                //Date
                matchData.Date = UnixTimeStampToDateTime(matchlist.GetMatches(i).Matchtime).ToString("yyyy-MM-dd hh:mm");
                //Finding the Position of our own entries
                int j = 0;
                for (; j < matchlist.GetMatches(i).Roundstats.Reservation.AccountIdsCount; j++)
                {
                    if (matchlist.GetMatches(i).Roundstats.Reservation.GetAccountIds(j).ToString() == steamID)
                        break;
                }
                //Kills, Assists, Deaths, MVP, Score, KD, Demo
                matchData.Kills = matchlist.GetMatches(i).Roundstats.GetKills(j);
                matchData.Assists = matchlist.GetMatches(i).Roundstats.GetAssists(j);
                matchData.Deaths = matchlist.GetMatches(i).Roundstats.GetDeaths(j);
                matchData.MVPs = matchlist.GetMatches(i).Roundstats.GetMvps(j);
                matchData.Score = matchlist.GetMatches(i).Roundstats.GetScores(j);
                matchData.KD = ((double)((int)(((double)matchData.Kills / matchData.Deaths)*100))/100);
                matchData.Demo = matchlist.GetMatches(i).Roundstats.Map; //new DemoButtonUserControl(matchlist.GetMatches(i).Roundstats.Map);
                //Getting DemoComment, K3, K4, K5, HS
                if (additionalDemoData.ContainsComment(matchData.Demo))
                {
                    matchData.DemoComment = additionalDemoData.GetComment(matchData.Demo);
                }
                if (additionalDemoData.ContainsK3(matchData.Demo))
                {
                    matchData.K3 = additionalDemoData.GetK3(matchData.Demo);
                }
                if (additionalDemoData.ContainsK4(matchData.Demo))
                {
                    matchData.K4 = additionalDemoData.GetK4(matchData.Demo);
                }
                if (additionalDemoData.ContainsK5(matchData.Demo))
                {
                    matchData.K5 = additionalDemoData.GetK5(matchData.Demo);
                }
                if (additionalDemoData.ContainsHS(matchData.Demo))
                {
                    matchData.HS = additionalDemoData.GetHS(matchData.Demo);
                }


                //Calculating if we won the match
                if (j < matchlist.GetMatches(i).Roundstats.Reservation.AccountIdsCount / 2)
                {
                    //Overall Score, ex. 16:13
                    matchData.Result = matchlist.GetMatches(i).Roundstats.GetTeamScores(0).ToString() + ":" + matchlist.GetMatches(i).Roundstats.GetTeamScores(1).ToString();
                    matchData.Won = (matchlist.GetMatches(i).Roundstats.GetTeamScores(0) > matchlist.GetMatches(i).Roundstats.GetTeamScores(1));
                }
                else
                {
                    //Overall Score, ex. 16:13
                    matchData.Result = matchlist.GetMatches(i).Roundstats.GetTeamScores(1).ToString() + ":" + matchlist.GetMatches(i).Roundstats.GetTeamScores(0).ToString();
                    matchData.Won = (matchlist.GetMatches(i).Roundstats.GetTeamScores(1) > matchlist.GetMatches(i).Roundstats.GetTeamScores(0));
                }

                //Calculating if we downloaded the Demo
                string[] tempURLSplit = matchData.Demo.Split('/');
                string tempDemoFileName = tempURLSplit[tempURLSplit.Length - 1];
                matchData.Downloaded = (File.Exists(DEMOFOLDER + tempDemoFileName.Substring(0, tempDemoFileName.Length - 4)));

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

        #endregion

        #region DemoDownload
        /// 
        /// Demo Download Section
        /// 

        private void DemoDownloadButton_Click(object sender, RoutedEventArgs e)
        {
            Button tempButton = (Button)sender;
            string URL = (string)tempButton.Content;

            string[] tempURLSplit = URL.Split('/');
            string tempDemoFileName = tempURLSplit[tempURLSplit.Length - 1];

            if (!Directory.Exists(DEMOFOLDER))
                Directory.CreateDirectory(DEMOFOLDER);

            if (!File.Exists(DEMOFOLDER + tempDemoFileName))
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += (senderv, ev) => Completed(senderv, ev, tempDemoFileName);
                webClient.DownloadProgressChanged += (senderv, ev) => ProgressChanged(senderv, ev, URL); //new DownloadProgressChangedEventHandler(ProgressChanged);
                webClient.DownloadFileAsync(new Uri(URL), DEMOFOLDER + tempDemoFileName);
            }
            else
            {
                MessageBox.Show("Demo is already Downloaded");
            }
        }

        private async void ProgressChanged(object sender, DownloadProgressChangedEventArgs e, string URL)
        {
            await Task.Run(() =>
            {
                foreach(var matchData in matchDataList)
                {
                    if (matchData.Demo == URL)
                        matchData.DownloadProgress = e.ProgressPercentage;
                }
            }).ConfigureAwait(continueOnCapturedContext: false);
        }

        private async void Completed(object sender, AsyncCompletedEventArgs e, string tempDemoFileName)
        {
            await Task.Run(() =>
            {
                UnZipDemo(tempDemoFileName);

                UpdateDownloadedList();

                MessageBox.Show("Download of: " + tempDemoFileName + " completed! Unzipping completed!");

            }).ConfigureAwait(continueOnCapturedContext: false);        
        }

        private void UnZipDemo(string zippedDemoName)
        {
            if (Directory.Exists(DEMOFOLDER))
            {
                if (File.Exists(DEMOFOLDER + zippedDemoName) && zippedDemoName.Substring(zippedDemoName.Length - 4, 4) == ".bz2" && !File.Exists(DEMOFOLDER + zippedDemoName.Substring(0, zippedDemoName.Length - 4)))
                    {
                        FileStream inputStream = File.Open(DEMOFOLDER + zippedDemoName, FileMode.Open, FileAccess.Read);
                        //- .bz2
                        FileStream outputStream = File.Create(DEMOFOLDER + zippedDemoName.Substring(0, zippedDemoName.Length - 4));
                        BZip2.Decompress(inputStream, outputStream, true);
                        inputStream.Close();
                        outputStream.Close();
                    }
            }
        }

        private void UpdateDownloadedList()
        {
            foreach (var matchData in matchDataList)
            {
                if (!matchData.Downloaded)
                {
                    string[] tempURLSplit = matchData.Demo.Split('/');
                    string tempDemoFileName = tempURLSplit[tempURLSplit.Length - 1];
                    matchData.Downloaded = (File.Exists(DEMOFOLDER + tempDemoFileName.Substring(0, tempDemoFileName.Length - 4)));
                }
            }
        }

        #endregion

        private async void DemoAnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            string url = ((Button)sender).Tag.ToString();

            MatchData tempMatchData = GetMatchData(url);

            if(!tempMatchData.Downloaded)
            {
                MessageBox.Show("Demo is not (fully) Downloaded or Unzipped, please wait a second or Start the Download if you didn't already do so.");
                return;
            }

            DemoAnalyzer tempDemoAnalyzer = new DemoAnalyzer(tempMatchData, steamID);

            bool successfulAnalysis = await tempDemoAnalyzer.Analyze();

            if(successfulAnalysis)
                MessageBox.Show("Analysis completed!");
            else
                MessageBox.Show("Error in Analyze, be wary of errors");
        }

        public MatchData GetMatchData(string demoURL)
        { 
            foreach(var matchData in matchDataList)
            {
                if(matchData.Demo == demoURL)
                    return matchData;
            }
            return null;
        }

        #region Closing & Demo Comment Serialization
        //
        // Closing & Demo Comment Serialization
        //

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //DemoCommentsSerialization
            ExtractAdditionalDemoData();

            if (!Directory.Exists(DEMOFOLDER))
                Directory.CreateDirectory(DEMOFOLDER);

            System.Xml.Serialization.XmlSerializer xmlWriter =
            new System.Xml.Serialization.XmlSerializer(typeof(AdditionalDemoData));

            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(
            "Demos/AdditionalDemoData.xml");
            xmlWriter.Serialize(streamWriter, additionalDemoData);

            streamWriter.Close();
        }

        private void ExtractAdditionalDemoData()
        {
            foreach (MatchData tempMatchData in matchDataList)
            {
                if(tempMatchData.DemoComment != null)
                {
                    additionalDemoData.AddComment(tempMatchData.Demo, tempMatchData.DemoComment);
                }

                if (tempMatchData.K3 != null)
                {
                    additionalDemoData.AddK3(tempMatchData.Demo, tempMatchData.K3);
                }

                if (tempMatchData.K4 != null)
                {
                    additionalDemoData.AddK4(tempMatchData.Demo, tempMatchData.K4);
                }

                if (tempMatchData.K5 != null)
                {
                    additionalDemoData.AddK5(tempMatchData.Demo, tempMatchData.K5);
                }

                if (tempMatchData.HS != null)
                {
                    additionalDemoData.AddHS(tempMatchData.Demo, tempMatchData.HS);
                }
            }
        }
        #endregion


    }
}
