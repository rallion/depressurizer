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
            return GetGameData();
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
