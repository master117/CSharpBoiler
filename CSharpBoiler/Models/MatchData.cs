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
using System.ComponentModel;

namespace CSharpBoiler.Models
{
    public class MatchData : INotifyPropertyChanged
    {
        public DateTime Date { get; set; }
        public string Result { get; set; }
        public bool Won { get; set; }
        public int Kills { get; set; }
        public int Assists { get; set; }
        public int Deaths { get; set; }
        public int MVPs { get; set; }
        public int Score { get; set; }
        public double KD { get; set; }
        public string Demo { get; set; }      
        public bool _Downloaded { get; set; }
        public bool Downloaded
        {
            get { return _Downloaded; }
            set
            {
                _Downloaded = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Downloaded"));
            }
        }
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
        public string _DemoComment { get; set; }
        public string DemoComment
        {
            get { return _DemoComment; }
            set
            {
                _DemoComment = value;
                Commented = value != "";
            }

        }
        public bool _Commented { get; set; }
        public bool Commented
        {
            get { return _Commented; }
            set
            {
                _Commented = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Commented"));
            }
        }
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
    }
}
