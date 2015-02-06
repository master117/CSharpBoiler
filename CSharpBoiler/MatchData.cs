using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBoiler
{
    public class MatchData : ObservableCollection<MatchData>, INotifyPropertyChanged
    {
        public string Date { get; set; }
        public string Result { get; set; }
        public int Kills { get; set; }
        public int Assists { get; set; }
        public int Deaths { get; set; }
        public int MVPs { get; set; }
        public int Score { get; set; }
        public double KD { get; set; }
        public string Demo { get; set; }
        public bool Won { get; set; }
        public bool Downloaded { get; set; }
        public string DemoComment { get; set; }
        public int HS { get; set; }
        public int K3 { get; set; }
        public int K4 { get; set; }
        public int K5 { get; set; }
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

    }
}
