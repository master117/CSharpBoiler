using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBoiler.DemoModel
{
    class KillEvent
    {
        public Player killer { get; set; }
        public Player victim { get; set; }
        public Item weapon { get; set; }
        public int eventTick { get; set; }
        public int eventRound { get; set; }
    }
}
