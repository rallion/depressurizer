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
        private const string RegexString = "^c:\\\\program files \\(x86\\)\\\\steam\\\\steamapps\\\\appmanifest_(\\d+)\\.acf$";

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

        public static void Export(Dictionary<int, AppManifest> gamesManifestsToExport)
        {
            foreach(AppManifest manifest in gamesManifestsToExport.Values)
            {
                SaveAppManifestToFile(manifest);
            }
            

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
                fileData = VdfFileNode.LoadFromText(reader, false);
            }
            return fileData;
        }

        private static IEnumerable<string> GetAppManifestFilesInLocation(string installedAppsPath)
        {
            Regex appManifestRegex = new Regex(@RegexString);

            string[] filesInDir = Directory
                .GetFiles(installedAppsPath, "*" + AppManifestFileExt);

            IEnumerable<string> files = filesInDir
                .Where(path => appManifestRegex.IsMatch(path));
            return files;
        }

        private static void SaveAppManifestToFile(AppManifest manifest)
        {
            string appManifestFileName = string.Format(Properties.Resources.GameManifestFilePath, Settings.Instance.SteamPath, manifest.AppId);
            SaveSteamFileNodeToFile(manifest, appManifestFileName);
        }

        private static void SaveSteamFileNodeToFile(AppManifest manifest, string fileName)
        {
            using (StreamWriter writer = new StreamWriter(File.Open(fileName, FileMode.Create)))
            {
                VdfFileNode node = manifest.ExportToNode();
                node.SaveAsText(writer, 0);
            }
        }
    }
}
