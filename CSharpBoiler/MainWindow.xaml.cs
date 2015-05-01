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
using System.Collections.ObjectModel;
using System.Windows.Threading;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;
using System.Threading;
using ICSharpCode.SharpZipLib.BZip2;
using System.Xml.Serialization;
using CSharpBoiler.Helpers;
using CSharpBoiler.Models;
using CSharpBoiler.NetworkHelper;

namespace CSharpBoiler
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<MatchData> matchDataList = new ObservableCollection<MatchData>();
        private AdditionalMatchData _additionalMatchData = new AdditionalMatchData();
        private long _accountId;
        private const string DEMOFOLDER = "Demos/";
        private const string DATAENDING = ".dat";
        private const string MATCHLISTTAG = "_matchlist";
        private const string ADDITIONALDEMODATAFILE = "AdditionalMatchData.xml";
        //Object where we store the matchList
        private CMsgGCCStrike15_v2_MatchList mainMatchList;
        private const long VOLVOMAGICNUMBER = 76561197960265728;

        #region Constructor
        public MainWindow(long tempAccountId)
        {
            InitializeComponent();
            MainGrid.DataContext = this;

            //Setting AccountID
            _accountId = tempAccountId;

            UpdateMatchlist();

            //If CSGO is started and finished while CSharpBoiler is Running we want to UpdateTheMatchList
            ThreadStart monitorCSGOThreadStart = MonitorCSGO;
            Thread monitorCSGOThread = new Thread(monitorCSGOThreadStart);
            monitorCSGOThread.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Binding the table to the dataObject
            MainDataGrid.ItemsSource = matchDataList;
            //Disabling selection on the table
            MainDataGrid.SelectionChanged += (obj, ev) =>
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            MainDataGrid.UnselectAll()));
        }
        #endregion

        #region CSGOStatus Control

        //Monitors CSGO, if CSGO finishes we want to update the Matchlist
        private void MonitorCSGO()
        {
            bool wasRunning = false;

            while (true)
            {
                if (Process.GetProcessesByName("csgo").Length != 0)
                {
                    wasRunning = true; 
                    VibranceAndAudioUserControlInstance.CSGOIsRunning();
                }

                if (Process.GetProcessesByName("csgo").Length == 0 && wasRunning)
                {
                    wasRunning = false;
                    VibranceAndAudioUserControlInstance.CSGOWasRunning();

                    long tempAccountId = BoilerHandler.StartAndGetAccountId();
                    if (tempAccountId != 0 && tempAccountId != _accountId)
                    {
                        _accountId = tempAccountId;
                        Properties.Settings.Default.LastAccountId = tempAccountId;
                    }
                    UpdateMatchlist();
                }

                Thread.Sleep(5000);
            }
        }

        #endregion

        #region UpdateMatchList

        private void UpdateMatchlist()
        {
            //Deserialize our stored data
            DeserializeMatchData();
            DeserializeAdditionalDemoData();

            //Parsing MatchData for UI
            Application.Current.Dispatcher.Invoke(new Action(() => { ParseMatchData(mainMatchList, _accountId); }));

            //Thread for Uploading MatchLinks to DB
            ThreadStart uploadThreadStart = this.UploadMatchData;
            Thread uploadThread = new Thread(uploadThreadStart);
            uploadThread.Start();

            //Initializing VACStat_us Control
            HashSet<string> steamIdsHashSet = new HashSet<string>();
            foreach (var cDataGccStrike15V2MatchInfo in mainMatchList.matches)
                foreach (var accountId in cDataGccStrike15V2MatchInfo.roundstats.reservation.account_ids)
                    steamIdsHashSet.Add((accountId + VOLVOMAGICNUMBER).ToString());
            VACStat_usSender.Initialize(steamIdsHashSet);
        }

        private void DeserializeMatchData()
        {
            //Retrieving old matches
            if (File.Exists(_accountId + MATCHLISTTAG + DATAENDING))
            {
                FileStream datFileStream = new FileStream(_accountId + MATCHLISTTAG + DATAENDING, FileMode.Open, FileAccess.Read);
                var tempMatchList = Serializer.Deserialize<CMsgGCCStrike15_v2_MatchList>(datFileStream);
                datFileStream.Close();

                mainMatchList = tempMatchList;
            }
        }

        private void DeserializeAdditionalDemoData()
        {
            //AdditionalDemoDataDeserialization
            if (Directory.Exists(DEMOFOLDER))
            {
                if (File.Exists(DEMOFOLDER + ADDITIONALDEMODATAFILE))
                {
                    System.Xml.Serialization.XmlSerializer xmlReader =
                    new System.Xml.Serialization.XmlSerializer(typeof(AdditionalMatchData));
                    System.IO.StreamReader streamReader = new System.IO.StreamReader(DEMOFOLDER + ADDITIONALDEMODATAFILE);
                    _additionalMatchData = (AdditionalMatchData)xmlReader.Deserialize(streamReader);

                    streamReader.Close();
                }
            }
        }

        //Parses The Data from MatchList to the UI Model
        public void ParseMatchData(CMsgGCCStrike15_v2_MatchList matchlist, long accountId)
        {
            for (int i = matchlist.matches.Count - 1; i >= 0; i--)
            {
                try
                {
                    //Creating new MatchData
                    MatchData matchData = new MatchData();
                    //Date
                    matchData.Date = TimeHelper.UnixTimeStampToDateTime(matchlist.matches[i].matchtime).ToLocalTime(); //.ToString("yyyy-MM-dd hh:mm");
                    //Finding the Position of our own entries
                    int j = 0;
                    for (; j < matchlist.matches[i].roundstats.reservation.account_ids.Count; j++)
                    {
                        if (matchlist.matches[i].roundstats.reservation.account_ids[j] == (uint)accountId)
                            break;
                    }
                    //If the users AccountId is not in the Game, return
                    //TODO: Figure out how this can happen
                    if (j == 10)
                        continue;

                    //Kills, Assists, Deaths, MVP, Score, KD, Demo
                    matchData.Kills = matchlist.matches[i].roundstats.kills[j];
                    matchData.Assists = matchlist.matches[i].roundstats.assists[j];
                    matchData.Deaths = matchlist.matches[i].roundstats.deaths[j];
                    matchData.MVPs = matchlist.matches[i].roundstats.mvps[j];
                    matchData.Score = matchlist.matches[i].roundstats.scores[j];

                    if (matchData.Kills != 0 && matchData.Deaths != 0)
                        matchData.KD = ((double)((int)(((double)matchData.Kills / matchData.Deaths) * 100)) / 100);
                    else
                        matchData.KD = 0;

                    matchData.Demo = matchlist.matches[i].roundstats.map; //new DemoButtonUserControl(matchlist.matches[i].roundstats.Map);

                    //Getting DemoComment
                    if (_additionalMatchData.ContainsComment(matchData.Demo))
                    {
                        matchData.DemoComment = _additionalMatchData.GetComment(matchData.Demo);
                    }

                    //Calculating if we won the match
                    if (j < matchlist.matches[i].roundstats.reservation.account_ids.Count / 2)
                    {
                        //Overall Score, ex. 16:13
                        matchData.Result = matchlist.matches[i].roundstats.team_scores[0].ToString() + ":" + matchlist.matches[i].roundstats.team_scores[1].ToString();
                        matchData.Won = (matchlist.matches[i].roundstats.team_scores[0] > matchlist.matches[i].roundstats.team_scores[1]);
                    }
                    else
                    {
                        //Overall Score, ex. 16:13
                        matchData.Result = matchlist.matches[i].roundstats.team_scores[1].ToString() + ":" + matchlist.matches[i].roundstats.team_scores[0].ToString();
                        matchData.Won = (matchlist.matches[i].roundstats.team_scores[1] > matchlist.matches[i].roundstats.team_scores[0]);
                    }

                    //Calculating if we downloaded the Demo
                    UpdateDownloadedList();

                    //adding to the MainList of MatchData
                    matchDataList.Add(matchData);
                }
                catch (Exception e)
                {
                    ConsoleManager.Show();
                    Console.Write(e.Message);
                }
            }
        }

        private void UpdateDownloadedList()
        {
            //We could only check for Demos that are not already marked as downloaded, but i like doublechecking here
            foreach (var matchData in matchDataList)
            {
                string[] tempUrlSplit = matchData.Demo.Split('/');
                string tempDemoFileName = tempUrlSplit[tempUrlSplit.Length - 1];
                matchData.Downloaded =
                    (File.Exists(DEMOFOLDER + tempDemoFileName.Substring(0, tempDemoFileName.Length - 4)));
            }
        }
        #endregion

        #region MatchLinkUpload

        public void UploadMatchData()
        {
            //
            // Sending MatchList to Server for Statistic Purposes
            //
            try
            {
                MatchLinkSender matchLinkSender = new MatchLinkSender(mainMatchList);
                int uploadedMatchLinks = matchLinkSender.Send();

                Application.Current.Dispatcher.Invoke(
                    new Action(() => { UploadedMatchLinksUserControlInstance.SetUploadedMatchLinksCount(uploadedMatchLinks, mainMatchList.matches.Count); }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        #region DemoDownload
        /// 
        /// Demo Download Section
        /// 

        private void DemoDownloadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button tempButton = (Button)sender;
                string URL = (string)tempButton.Tag.ToString();

                string[] tempURLSplit = URL.Split('/');
                string tempZippedDemoFileName = tempURLSplit[tempURLSplit.Length - 1];
                //case: .bz2
                string tempDemoFileName = tempZippedDemoFileName.Substring(0, tempZippedDemoFileName.Length - 4);

                if (!Directory.Exists(DEMOFOLDER))
                    Directory.CreateDirectory(DEMOFOLDER);

                if (!File.Exists(DEMOFOLDER + tempDemoFileName))
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFileCompleted += (senderv, ev) => Completed(senderv, ev, tempZippedDemoFileName);
                    webClient.DownloadProgressChanged += (senderv, ev) => ProgressChanged(senderv, ev, URL); //new DownloadProgressChangedEventHandler(ProgressChanged);
                    webClient.DownloadFileAsync(new Uri(URL), DEMOFOLDER + tempZippedDemoFileName);
                }
                else
                {
                    File.Delete(DEMOFOLDER + tempDemoFileName);
                    MessageBox.Show("Demo was already Downloaded, deleted Demo");
                }
            }
            catch (Exception ex)
            {
                ConsoleManager.Show();
                Console.WriteLine(ex.Message);
            }
        }

        private async void ProgressChanged(object sender, DownloadProgressChangedEventArgs e, string URL)
        {
            await Task.Run(() =>
            {
                var matchData = matchDataList.SingleOrDefault(i => i.Demo == URL);
                if (matchData != null)
                    matchData.DownloadProgress = e.ProgressPercentage;

            }).ConfigureAwait(continueOnCapturedContext: false);
        }

        private async void Completed(object sender, AsyncCompletedEventArgs e, string tempDemoFileName)
        {
            await Task.Run(() =>
            {
                //Checking if the Download was succesful
                if (e.Error != null)
                {
                    MessageBox.Show("Demo is too old, Download no Longer Available");
                    return;
                }

                //Unzipping
                UnZipDemo(tempDemoFileName);

                //Deleting the zipped remains
                if (Directory.Exists(DEMOFOLDER) && File.Exists(DEMOFOLDER + tempDemoFileName))
                    File.Delete(DEMOFOLDER + tempDemoFileName);

                //Updating the list which Demos are already Downloaded
                UpdateDownloadedList();
            }).ConfigureAwait(false);
        }

        private void UnZipDemo(string zippedDemoName)
        {
            //case: .bz2
            if (Directory.Exists(DEMOFOLDER)
                && File.Exists(DEMOFOLDER + zippedDemoName)
                && zippedDemoName.Substring(zippedDemoName.Length - 4, 4) == ".bz2"
                && !File.Exists(DEMOFOLDER + zippedDemoName.Substring(0, zippedDemoName.Length - 4)))
            {
                FileStream inputStream = File.Open(DEMOFOLDER + zippedDemoName, FileMode.Open, FileAccess.Read);
                
                FileStream outputStream =
                    File.Create(DEMOFOLDER + zippedDemoName.Substring(0, zippedDemoName.Length - 4));
                BZip2.Decompress(inputStream, outputStream, true);
                inputStream.Close();
                outputStream.Close();
            }
        }
        #endregion

        #region Closing & Demo Comment Serialization
        //
        // Closing & Comment Serialization
        //

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //AdditionalMatchDataSerialization
            ExtractAdditionalDemoData();

            if (!Directory.Exists(DEMOFOLDER))
                Directory.CreateDirectory(DEMOFOLDER);

            XmlSerializer xmlWriter = new XmlSerializer(typeof(AdditionalMatchData));
            StreamWriter additionalDemoDataStreamWriter = new StreamWriter(DEMOFOLDER + "AdditionalMatchData.xml");
            xmlWriter.Serialize(additionalDemoDataStreamWriter, _additionalMatchData);
            additionalDemoDataStreamWriter.Close();

            //Saving Changed Settings
            Properties.Settings.Default.Save();

            //Shutdown
            Application.Current.Shutdown(1);
        }

        private void ExtractAdditionalDemoData()
        {
            foreach (MatchData tempMatchData in matchDataList)
                _additionalMatchData.AddComment(tempMatchData.Demo, tempMatchData.DemoComment);
        }
        #endregion



        #region Events

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            SettingGrid.Visibility = SettingGrid.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
        }

        private void ShowComment_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the Selected Row Button
                Button button = (Button)sender;

                // Check if the Button is null  
                if (button != null)
                {
                    // Return the row which contains the specified button
                    DataGridRow dataGridRowSelectedRow = DataGridRow.GetRowContainingElement(button);

                    // Check if the DataGridRow is null  
                    if (dataGridRowSelectedRow != null)
                    {
                        // if Collapsed -> Visible 
                        if (dataGridRowSelectedRow.DetailsVisibility == System.Windows.Visibility.Collapsed)
                            dataGridRowSelectedRow.DetailsVisibility = System.Windows.Visibility.Visible;
                        // else collaps row Details  
                        else
                            dataGridRowSelectedRow.DetailsVisibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion
    }
}
