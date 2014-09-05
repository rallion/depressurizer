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
using System.Globalization;
using System.Threading;
using Depressurizer.Lib;

namespace Depressurizer
{
    internal enum StartupAction
    {
        None,
        Load,
        Create
    }

    internal enum GameListSource
    {
        XmlPreferred,
        XmlOnly,
        WebsiteOnly
    }

    internal enum UserLanguage
    {
        windows,
        en,
        es
    }

    internal class Settings : AppSettings
    {
        private static Settings instance;
        private int _configBackupCount = 3;
        private GameListSource _listSource = GameListSource.XmlPreferred;
        private int _logBackups = 1;
        private LoggerLevel _logLevel = LoggerLevel.Info;
        private int _logSize = 2000000;
        private string _profileToLoad;
        private bool _removeExtraEntries = true;
        private bool _singleCatMode;
        private StartupAction _startupAction = StartupAction.Create;

        private string _steamPath;
        private UserLanguage _userLanguage = UserLanguage.windows;

        private Settings()
        {
            FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                       @"\Depressurizer\Settings.xml";
        }

        public int SettingsVersion
        {
            get { return 2; }
        }

        public string SteamPath
        {
            get { return _steamPath; }
            set
            {
                if (_steamPath != value)
                {
                    _steamPath = value;
                    outOfDate = true;
                }
            }
        }

        public int ConfigBackupCount
        {
            get { return _configBackupCount; }
            set
            {
                if (_configBackupCount != value)
                {
                    _configBackupCount = value;
                    outOfDate = true;
                }
            }
        }

        public StartupAction StartupAction
        {
            get { return _startupAction; }
            set
            {
                if (_startupAction != value)
                {
                    _startupAction = value;
                    outOfDate = true;
                }
            }
        }

        public string ProfileToLoad
        {
            get { return _profileToLoad; }
            set
            {
                if (_profileToLoad != value)
                {
                    _profileToLoad = value;
                    outOfDate = true;
                }
            }
        }

        public bool RemoveExtraEntries
        {
            get { return _removeExtraEntries; }
            set
            {
                if (_removeExtraEntries != value)
                {
                    _removeExtraEntries = value;
                    outOfDate = true;
                }
            }
        }

        public GameListSource ListSource
        {
            get { return _listSource; }
            set
            {
                if (_listSource != value)
                {
                    _listSource = value;
                    outOfDate = true;
                }
            }
        }

        public LoggerLevel LogLevel
        {
            get { return _logLevel; }
            set
            {
                Program.Logger.Level = value;
                if (_logLevel != value)
                {
                    _logLevel = value;
                    outOfDate = true;
                }
            }
        }

        public int LogSize
        {
            get { return _logSize; }
            set
            {
                Program.Logger.MaxFileSize = value;
                if (_logSize != value)
                {
                    _logSize = value;
                    outOfDate = true;
                }
            }
        }

        public int LogBackups
        {
            get { return _logBackups; }
            set
            {
                Program.Logger.MaxBackup = value;
                if (_logBackups != value)
                {
                    _logBackups = value;
                    outOfDate = true;
                }
            }
        }

        public UserLanguage UserLang
        {
            get { return _userLanguage; }
            set
            {
                if (_userLanguage != value)
                {
                    _userLanguage = value;
                    outOfDate = true;
                    changeLanguage(_userLanguage);
                }
            }
        }

        public bool SingleCatMode
        {
            get { return _singleCatMode; }
            set
            {
                if (_singleCatMode != value)
                {
                    _singleCatMode = value;
                    outOfDate = true;
                }
            }
        }

        public static Settings Instance()
        {
            return instance ?? (instance = new Settings());
        }

        private void changeLanguage(UserLanguage userLanguage)
        {
            CultureInfo newCulture;

            switch (userLanguage)
            {
                case UserLanguage.en:
                    newCulture = new CultureInfo("en");
                    break;
                case UserLanguage.es:
                    newCulture = new CultureInfo("es");
                    break;
                default:
                    newCulture = Thread.CurrentThread.CurrentCulture;
                    break;
            }

            Thread.CurrentThread.CurrentUICulture = newCulture;
        }
    }
}