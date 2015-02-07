﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBoiler
{
    public class MatchData : INotifyPropertyChanged
    {
        public string Date { get; set; }
        public string Result { get; set; }
        public bool Won { get; set; }
        public int Kills { get; set; }
        public int Assists { get; set; }
        public int Deaths { get; set; }
        public int MVPs { get; set; }
        public int Score { get; set; }
        public double KD { get; set; }
        public string Demo { get; set; }      
        public bool Downloaded { get; set; }
        public int _DownloadProgress { get; set; }
        public int DownloadProgress
        {
            get { return _DownloadProgress; }
            set
            {
                _DownloadProgress = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DownloadProgress"));
            }
        }
        public string DemoComment { get; set; }
        public int _K3 { get; set; }
        public int K3
        {
            get { return _K3; }
            set
            {
                _K3 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("K3"));
            }
        }
        public int _K4 { get; set; }
        public int K4
        {
            get { return _K4; }
            set
            {
                _K4 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("K4"));
            }
        }
        public int _K5 { get; set; }
        public int K5
        {
            get { return _K5; }
            set
            {
                _K5 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("K5"));
            }
        }
        public int _HS { get; set; }
        public int HS
        {
            get { return _HS; }
            set
            {
                _HS = value;
                OnPropertyChanged(new PropertyChangedEventArgs("HS"));
            }
        }
        public int _AnalysisProgress { get; set; }
        public int AnalysisProgress
        {
            get { return _AnalysisProgress; }
            set
            {
                _AnalysisProgress = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AnalysisProgress"));
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        /*
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }
         * */

        /*
        public int _AnalysisProgress;
        public int AnalysisProgress
        {
            get { return _AnalysisProgress; }
            set
            {
                _AnalysisProgress = value;
                NotifyPropertyChanged("AnalysisProgress");
            }
        }

        public PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        */
    }
}
