using Depressurizer.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepressurizerUnitTests.TestUtils
{
    static class TestIO
    {

        public static void setUpTempFolder()
        {
            CreateIfDoesntExist(getTempFolderPath());
            CreateIfDoesntExist(getTempFolderAppsFullPath());
        }

        private static void CreateIfDoesntExist(string fullPath)
        {
            if (!System.IO.Directory.Exists(fullPath))
            {
                System.IO.Directory.CreateDirectory(fullPath);
            }
        }

        public static void tearDownTempFolder()
        {
            System.IO.DirectoryInfo tempSteamFolder = new System.IO.DirectoryInfo(getTempFolderPath());

            foreach (System.IO.FileInfo file in tempSteamFolder.GetFiles())
            {
                file.Delete();
            }
            foreach (System.IO.DirectoryInfo dir in tempSteamFolder.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private static string getTempFolderAppsFullPath()
        {
            string tmpPath = getTempFolderPath();

            string subPath = InstalledGamesLoader.InstalledAppsSubPath;

            string fullPath = tmpPath + subPath;
            return fullPath;
        }

        public static string getTempFolderPath()
        {
            return System.IO.Path.GetTempPath() + "/DepressurizerTest/";
        }
    }
}
