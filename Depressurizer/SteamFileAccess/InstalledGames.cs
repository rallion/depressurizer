/*
Copyright 2011, 2012, 2013 Steve Labbe.

This file is part of Depressurizer.

Depressurizer is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Depressurizer is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Depressurizer.  If not, see <http://www.gnu.org/licenses/>.
*/

using Depressurizer.SteamFileAccess.ApplicationManifest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depressurizer.Games
{
    public class InstalledGames
    {
        private readonly IReadOnlyDictionary<int, AppManifest> UnmodifiableGamesList;

        public InstalledGames()
        {
            UnmodifiableGamesList = new Dictionary<int, AppManifest>();
        }

        public InstalledGames(IReadOnlyDictionary<int, AppManifest> UnmodifiableGamesList)
        {
            this.UnmodifiableGamesList = UnmodifiableGamesList;
        }

        public IEnumerable<int> getInstalledGamesList()
        {
            return UnmodifiableGamesList.Keys;
        }

        public Dictionary<int, AppManifest> GetMatchingGames(List<int> gameIds)
        {

            Dictionary<int, AppManifest> matchingGames = new Dictionary<int, AppManifest>();

            foreach(int gameId in gameIds)
            {
                AppManifest gameManifest = null;
                if (UnmodifiableGamesList.TryGetValue(gameId, out gameManifest))
                {
                    matchingGames[gameManifest.AppId] = gameManifest;
                }
            }

            return matchingGames;
        }
    }
}
