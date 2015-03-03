using SteamKit2.GC.CSGO.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBoiler.Helpers
{
    class MatchComparer : IEqualityComparer<CDataGCCStrike15_v2_MatchInfo>
    {
        public bool Equals(CDataGCCStrike15_v2_MatchInfo x, CDataGCCStrike15_v2_MatchInfo y)
        {
            return (x.matchid == y.matchid);
        }

        public int GetHashCode(CDataGCCStrike15_v2_MatchInfo obj)
        {
            return (int)obj.matchid;
        }
    }
}
