using Depressurizer.Games;
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

        private const string InstalledAppsSubPath = "\\steamapps\\";
        private const string AppManifestFileExt = ".acf";
        private const string RegexString = "^appmanifest_(\\d+)\\" + AppManifestFileExt + "$";

        public static InstalledGames GetGames()
        {
            string installedAppsPath = Settings.Instance.SteamPath + InstalledAppsSubPath;

            IEnumerable<string> files = GetAppManifestFilesInLocation(installedAppsPath);

            Dictionary<int, AppManifest> games = new Dictionary<int, AppManifest>();

            foreach (string filePath in files)
            {
                AppManifestFileWrapper wrapper = GetAppManifestFromFile(filePath);
                games.Add(wrapper.AppId, wrapper);
            }

            IReadOnlyDictionary<int, AppManifest> unmodifiableGamesList = games;

            return new InstalledGames(unmodifiableGamesList);
        }

        private static AppManifestFileWrapper GetAppManifestFromFile(string filePath)
        {
            VdfFileNode fileData = GetSteamFileNodeFromFile(filePath);
            AppManifestFileWrapper wrapper = new AppManifestFileWrapper(fileData);
            return wrapper;
        }

        private static VdfFileNode GetSteamFileNodeFromFile(string filePath)
        {

            VdfFileNode fileData;
            using (StreamReader reader = new StreamReader(filePath, false))
            {
                fileData = VdfFileNode.LoadFromText(reader, true);
            }
            return fileData;
        }

        private static IEnumerable<string> GetAppManifestFilesInLocation(string installedAppsPath)
        {
            Regex appManifestRegex = new Regex(@RegexString);

            IEnumerable<string> files = Directory
                .GetFiles(installedAppsPath, "*" + AppManifestFileExt)
                .Where(path => appManifestRegex.IsMatch(path));
            return files;
        }


    }
}
