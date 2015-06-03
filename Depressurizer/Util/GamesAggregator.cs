using Depressurizer.Games;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depressurizer.Util
{
    class GamesAggregator
    {
        public List<int> Matching(InstalledGames installedGames, GameList ownedGames)
        {
            List<int> ownedAndInstalled = new List<int>();

            IEnumerable installedGamesList = installedGames.getInstalledGamesList();

            foreach(int installedGameId in installedGamesList)
            {
                if (ownedGames.Games.ContainsKey(installedGameId))
                {
                    ownedAndInstalled.Add(installedGameId);
                }
            }

            return ownedAndInstalled;
        }

    }
}
