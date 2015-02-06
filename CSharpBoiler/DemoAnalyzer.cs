using DemoInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBoiler
{
    class DemoAnalyzer
    {
        MatchData mainMatchData;
        string mainSteamID;
        int mainPlayerID;
        Player mainPlayer;
        DemoParser demoParser;

        //Parameters for Start->End calculations
        int K3;
        int K4;
        int K5;

        //Parameters for Round->Round calculations
        int pkills = 0;



        public DemoAnalyzer(MatchData tempMatchData, string tempSteamID)
        {
            mainMatchData = tempMatchData;
            mainSteamID = tempSteamID;
        }

        public bool Analyze()
        {
            string[] tempURLSplit = mainMatchData.Demo.Split('/');
            string tempDemoFileName = tempURLSplit[tempURLSplit.Length - 1];
            string demoName = tempDemoFileName.Substring(0, tempDemoFileName.Length - 4);

            //error Checking
            if (!Directory.Exists("Demos") || !File.Exists("Demos/" + demoName) || demoName.Substring(demoName.Length - 4, 4) != ".dem")
                return false;

            demoParser = new DemoParser(File.OpenRead("Demos/" + demoName));

            demoParser.TickDone += parser_TickDone;
            demoParser.MatchStarted += parser_MatchStarted;
            demoParser.PlayerKilled += HandlePlayerKilled;
            demoParser.WeaponFired += HandleWeaponFired;
            demoParser.RoundStart += demoParser_RoundStart;

            demoParser.ParseHeader();

            demoParser.ParseToEnd();

            mainMatchData.K3 = K3;
            mainMatchData.K4 = K4;
            mainMatchData.K5 = K5;

            return true;
        }

        //Most Calculation will be done here
        void demoParser_RoundStart(object sender, RoundStartedEventArgs e)
        {
            if (mainPlayer == null)
            {
                foreach (var player in demoParser.PlayingParticipants)
                {
                    if (player.SteamID.ToString() == mainSteamID)
                    {
                        mainPlayerID = player.EntityID;
                        mainPlayer = player;
                    }

                }
            }

            switch(pkills)
            {
                case 3:
                    K3++;
                    break;
                case 4:
                    K4++;
                    break;
                case 5:
                    K5++;
                    break;
            }

            pkills = 0;
        }

        private void parser_TickDone(object sender, TickDoneEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void parser_MatchStarted(object sender, MatchStartedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void HandlePlayerKilled(object sender, PlayerKilledEventArgs e)
        {
            if (mainPlayer != null && e.Killer == mainPlayer && e.DeathPerson.Team != e.Killer.Team)
                pkills++;
        }

        private void HandleWeaponFired(object sender, WeaponFiredEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
