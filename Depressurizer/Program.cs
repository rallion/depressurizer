/*
This file is part of Depressurizer.
Copyright 2011, 2012, 2013 Steve Labbe.

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
using System.Windows.Forms;
using Rallion;
using Depressurizer.Service;

namespace Depressurizer {
    static class Program {


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            FatalError.InitializeHandler();

            SetupLogger();

            Settings.Instance.Load();

            InstanceContainer.Logger.Write(LoggerLevel.Info, GlobalStrings.Program_ProgramInitialized, InstanceContainer.Logger.Level);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            Application.Run( new FormMain() );

            Settings.Instance.Save();

            InstanceContainer.Logger.Write(LoggerLevel.Info, GlobalStrings.Program_ProgramClosing);
            InstanceContainer.Logger.EndSession();
        }

        private static void SetupLogger()
        {
            InstanceContainer.Logger = new AppLogger();

            InstanceContainer.Logger.Level = LoggerLevel.None;
            InstanceContainer.Logger.DateFormat = "HH:mm:ss'.'ffffff";

            InstanceContainer.Logger.MaxFileSize = 2000000;
            InstanceContainer.Logger.MaxBackup = 1;
            InstanceContainer.Logger.FileNameTemplate = "Depressurizer.log";
        }
    }
}
