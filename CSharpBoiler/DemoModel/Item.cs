using DemoInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBoiler.DemoModel
{
    public class Item
    {
        public Equipment item { get; set; }
        public int price { get; set; }
        public int pickupTick { get; set; }
        public int pickupRound { get; set; }
        public int dropTick { get; set; }
        public int dropRound { get; set; }
    }
}
