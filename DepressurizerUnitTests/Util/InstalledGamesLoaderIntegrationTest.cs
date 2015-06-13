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
using Depressurizer;
using Depressurizer.Util;
using Depressurizer.Service;

using System.Linq;
using Rallion;
using DepressurizerUnitTests.TestUtils;


namespace DepressurizerUnitTests.Util
{
    [TestClass]
    public class InstalledGamesLoaderIntegrationTest
    {

        [TestInitialize()]
        public void Initialize()
        {
            Settings.Instance.SteamPath = "./Data";
            InstanceContainer.Logger = new AppLogger(); //TODO: Extract this as a form of injection? / mocking / assert recieves a message

            TestIO.setUpTempFolder();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            TestIO.tearDownTempFolder();
        }

        [TestMethod]
        public void LoaderLoadsCorrectlyNamedFiles()
        {
            // given

            // when 
            InstalledGames installedGames = InstalledGamesLoader.Import();

            // then
            IEnumerable<int> gamesList = installedGames.getInstalledGamesList();

            Assert.AreEqual(2, gamesList.Count());
            Assert.IsTrue(gamesList.Contains(212680));
            Assert.IsTrue(gamesList.Contains(244850));
        }

        [TestMethod]
        public void LoaderIsErrorResistant()
        {
            // given
            Settings.Instance.SteamPath = "./Data/TestForInvalidManifest_MixedError";

            // when 
            InstalledGames installedGames = InstalledGamesLoader.Import();

            // then
            IEnumerable<int> gamesList = installedGames.getInstalledGamesList();

            Assert.AreEqual(2, gamesList.Count());
            Assert.IsTrue(gamesList.Contains(212680));
            Assert.IsTrue(gamesList.Contains(244850));
        }

        [TestMethod]
        public void LoaderWorksForBarebonesFiles()
        {
            // given
            Settings.Instance.SteamPath = "./Data/TestForInvalidManifest_Shell";

            // when 
            InstalledGames installedGames = InstalledGamesLoader.Import();

            // then
            IEnumerable<int> gamesList = installedGames.getInstalledGamesList();

            Assert.AreEqual(1, gamesList.Count());
            Assert.IsTrue(gamesList.Contains(12345));
        }

        [TestMethod]
        public void LoaderDoesntLoadIncorrectlyNamedFiles()
        {
            // given
            Settings.Instance.SteamPath = "./Data/TestForInvalidManifest_WrongName";

            // when 
            InstalledGames installedGames = InstalledGamesLoader.Import();

            // then
            IEnumerable<int> gamesList = installedGames.getInstalledGamesList();

            Assert.AreEqual(0, gamesList.Count());
        }

        [TestMethod]
        public void LoaderLogsWarningDoesntIncludeResultOnBlankFiles()
        {
            // given
            Settings.Instance.SteamPath = "./Data/TestForInvalidManifest_Blank";

            // when 
            InstalledGames installedGames = InstalledGamesLoader.Import();

            // then
            IEnumerable<int> gamesList = installedGames.getInstalledGamesList();

            Assert.AreEqual(0, gamesList.Count());
        }

        [TestMethod]
        public void LoaderIgnoresFilesWithMismatchedFilenameAndAppid()
        {
            // given
            Settings.Instance.SteamPath = "./Data/TestForInvalidManifest_NotMatching";

            // when 
            InstalledGames installedGames = InstalledGamesLoader.Import();

            // then
            IEnumerable<int> gamesList = installedGames.getInstalledGamesList();

            Assert.AreEqual(0, gamesList.Count());
        }

        [TestMethod]
        public void CanReadAndWriteFilesTheSame()
        {
            // given
            InstalledGames installedGames = InstalledGamesLoader.Import();
            Dictionary<int, AppManifest> allGames = getAll(installedGames);

            Settings.Instance.SteamPath = TestIO.getTempFolderPath();

            // when 
            InstalledGamesLoader.Export(allGames);
            installedGames = InstalledGamesLoader.Import();
            allGames = getAll(installedGames);
            InstalledGamesLoader.Export(allGames);
            installedGames = InstalledGamesLoader.Import();
            allGames = getAll(installedGames);

            // then


            IEnumerable<int> gamesList = installedGames.getInstalledGamesList();

            Assert.AreEqual(2, gamesList.Count());
            Assert.IsTrue(gamesList.Contains(212680));
            Assert.IsTrue(gamesList.Contains(244850));

        }

        private static Dictionary<int, AppManifest> getAll(InstalledGames installedGames)
        {
            return installedGames.GetMatchingGames(installedGames.getInstalledGamesList().ToList());
        }
    }
}
