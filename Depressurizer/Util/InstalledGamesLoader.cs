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
using Depressurizer.Service;
using Depressurizer.SteamFileAccess.ApplicationManifest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Depressurizer.Util
{
    public static class InstalledGamesLoader
    {

        public const string InstalledAppsSubPath = "/steamapps/";
        private const string AppManifestFileExt = ".acf";
        private const string RegexString = ".*appmanifest_(\\d+)\\" + AppManifestFileExt +"$";

        public static InstalledGames Import()
        {
            string installedAppsPath = Settings.Instance.SteamPath + InstalledAppsSubPath;

            Dictionary<int, string> files = GetAppManifestFilesInLocation(installedAppsPath);

            Dictionary<int, AppManifest> games = new Dictionary<int, AppManifest>();

            foreach (KeyValuePair<int, string> installedGameIdAndLoc in files)
            {
                try
                {
                    AppManifestFileWrapper wrapper = GetAppManifestFromFile(installedGameIdAndLoc);
                    games.Add(wrapper.AppId, wrapper);
                }
                catch (Exception e)
                {
                    InstanceContainer.Logger.Write(
                        Rallion.LoggerLevel.Warning, 
                        "Error loading file: %s. Exception: %s.", 
                        new Object[] { installedGameIdAndLoc, e });
                }
            }

            IReadOnlyDictionary<int, AppManifest> unmodifiableGamesList = games;

            return new InstalledGames(unmodifiableGamesList);
        }

        public static void Export(Dictionary<int, AppManifest> gamesManifestsToExport)
        {
            foreach (AppManifest manifest in gamesManifestsToExport.Values)
            {
                SaveAppManifestToFile(manifest);
            }


        }

        private static Dictionary<int, string> GetAppManifestFilesInLocation(string installedAppsPath)
        {
            Regex appManifestRegex = new Regex(@RegexString);

            Dictionary<int, string> gameFiles = new Dictionary<int, string>();

            string[] filesInDir = Directory
                .GetFiles(installedAppsPath, "*" + AppManifestFileExt);

            foreach (string filePath in filesInDir)
            {
                if (appManifestRegex.IsMatch(filePath))
                {
                    MatchCollection mc = appManifestRegex.Matches(filePath);

                    Match match = mc[0];

                    string filename = match.Groups[0].Value;
                    int appId = Convert.ToInt32(match.Groups[1].Value);

                    gameFiles.Add(appId, filename);
                }
            }

            return gameFiles;
        }

        private static AppManifestFileWrapper GetAppManifestFromFile(KeyValuePair<int, string> installedGameIdAndLoc)
        {
            VdfFileNode fileData = GetSteamFileNodeFromFile(installedGameIdAndLoc.Value);
            AppManifestFileWrapper wrapper = new AppManifestFileWrapper(fileData);

            if (installedGameIdAndLoc.Key != wrapper.AppId)
            {
                throw new FileLoadException("Invalid app manifest file. File name doesn't match contained appId");
            }

            return wrapper;
        }

        private static VdfFileNode GetSteamFileNodeFromFile(string filePath)
        {

            VdfFileNode fileData;
            using (StreamReader reader = new StreamReader(filePath, false))
            {
                fileData = VdfFileNode.LoadFromText(reader, false);
            }
            return fileData;
        }

        private static void SaveAppManifestToFile(AppManifest manifest)
        {
            string appManifestFileName = string.Format(Properties.Resources.GameManifestFilePath, Settings.Instance.SteamPath, manifest.AppId);
            SaveSteamFileNodeToFile(manifest.ExportToNode(), appManifestFileName);
        }

        private static void SaveSteamFileNodeToFile(VdfFileNode manifestNode, string fileName)
        {
            using (StreamWriter writer = new StreamWriter(File.Open(fileName, FileMode.Create)))
            {
                manifestNode.SaveAsText(writer, 0);
            }
        }
    }
}
