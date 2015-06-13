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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Depressurizer.SteamFileAccess;
using Depressurizer.Games;
using System.Collections.Generic;
using Depressurizer.SteamFileAccess.ApplicationManifest;

namespace DepressurizerUnitTests.Games
{
    [TestClass]
    public class InstalledGamesTest
    {
        InstalledGames InstalledGames;

        [TestInitialize()]
        public void Initialize() {
            Dictionary<int, AppManifest> gamesList = new Dictionary<int, AppManifest>();
            addToGamesList(gamesList, 3);
            addToGamesList(gamesList, 600);
            addToGamesList(gamesList, 9001);
            InstalledGames = new InstalledGames(gamesList);
        }

        private void addToGamesList(Dictionary<int, AppManifest> gamesList, int appId)
        {
            gamesList.Add(appId, new AppManifestStub(appId));
        }

        [TestMethod]
        public void BlankWillReturnBlankSetOfResults()
        {
            // given

            // when 
            Dictionary<int, AppManifest> games = InstalledGames.GetMatchingGames(new List<int>());

            // then
            Assert.AreEqual(games.Count, 0);
        }

        [TestMethod]
        public void ListOfNonMatchingWillReturnBlankSetOfResults()
        {
            // given

            // when 
            Dictionary<int, AppManifest> games = InstalledGames.GetMatchingGames(new List<int>() { 1 });

            // then
            Assert.AreEqual(games.Count, 0);
        }

        [TestMethod]
        public void ListOfOneMatchingWillReturnSetOfOneMatchingResult()
        {
            // given

            // when 
            Dictionary<int, AppManifest> games = InstalledGames.GetMatchingGames(new List<int>() { 600 });

            // then
            Assert.AreEqual(games.Count, 1);
            Assert.IsTrue(games.ContainsKey(600));
        }

        [TestMethod]
        public void ListOfManyMatchingAndNonMatchingWillOnlyReturnMatching()
        {
            // given

            // when 
            Dictionary<int, AppManifest> games = InstalledGames.GetMatchingGames(new List<int>() { 1, 17, 600, 900, 9001 });

            // then
            Assert.AreEqual(games.Count, 2);
            Assert.IsTrue(games.ContainsKey(600));
            Assert.IsTrue(games.ContainsKey(9001));
        }

        private class AppManifestStub : AppManifest
        {
            private int _appId;

            public AppManifestStub(int appId)
            {
                _appId = appId;
            }

            public int AppId
            {
                get { return _appId; }
            }

            public string Name
            {
                get { throw new NotImplementedException(); }
            }

            public int AutoUpdateBehavior
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public int AllowOtherDownloadsWhileRunning
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public Depressurizer.VdfFileNode ExportToNode()
            {
                throw new NotImplementedException();
            }
        }
    }
}
