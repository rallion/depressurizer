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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depressurizer.SteamFileAccess.ApplicationManifest
{
    class AppManifestFileWrapper : AppManifest
    {
        private const string AppBaseContainer = "AppState";
        private const string AppIdKey = "appid";
        private const string AppNameKey = "name";
        private const string AutoUpdateBehaviorKey = "AutoUpdateBehavior";
        private const string AllowOtherDownloadsWhileRunningKey = "AllowOtherDownloadsWhileRunning";
        private VdfFileNode fileData;

        public AppManifestFileWrapper(VdfFileNode fileData)
        {
            this.fileData = fileData;
        }

        private VdfFileNode GetGameData()
        {
            return fileData.GetNodeAt(AppBaseContainer);
        }

        public int AppId
        {
            get
            {
                VerifyExists(AppIdKey);
                return GetGameData().GetNodeAt(AppIdKey).NodeInt;
            }
        }

        public string Name
        {
            get
            {
                VerifyExists(AppNameKey);
                return GetGameData().GetNodeAt(AppNameKey).NodeString;
            }
        }

        public int AutoUpdateBehavior
        {
            get
            {
                VerifyExists(AutoUpdateBehaviorKey);
                return GetGameData().GetNodeAt(AutoUpdateBehaviorKey).NodeInt;
            }
            set
            {
                GetGameData().PutNodeAt(AutoUpdateBehaviorKey, value);
            }
        }

        public int AllowOtherDownloadsWhileRunning
        {
            get
            {
                VerifyExists(AllowOtherDownloadsWhileRunningKey);
                return GetGameData().GetNodeAt(AllowOtherDownloadsWhileRunningKey).NodeInt;
            }
            set
            {
                GetGameData().PutNodeAt(AllowOtherDownloadsWhileRunningKey, value);
            }
        }

        public VdfFileNode ExportToNode()
        {
            return fileData;
        }

        private void VerifyExists(string nodeName)
        {

            if (!GetGameData().ContainsKey(nodeName))
            {
                throw new ContextMarshalException("Failed to find " + nodeName + " in App Manifest file");
            }
        }
    }
}
