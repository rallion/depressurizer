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

using Depressurizer.Games;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depressurizer.Util
{
    public static class GamesAggregator
    {
        public static List<int> Matching(InstalledGames installedGames, Dictionary<int, GameInfo> ownedGames)
        {
            List<int> ownedAndInstalled = new List<int>();

            IEnumerable installedGamesList = installedGames.getInstalledGamesList();

            foreach(int installedGameId in installedGamesList)
            {
                if (ownedGames.ContainsKey(installedGameId))
                {
                    ownedAndInstalled.Add(installedGameId);
                }
            }

            return ownedAndInstalled;
        }
    }
}
