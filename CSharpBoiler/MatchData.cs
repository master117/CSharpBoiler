using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBoiler
{
    public class MatchData : ObservableCollection<MatchData>
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
    }
}
