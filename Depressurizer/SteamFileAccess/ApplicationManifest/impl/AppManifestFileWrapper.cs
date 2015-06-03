using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depressurizer.SteamFileAccess.ApplicationManifest
{
    class AppManifestFileWrapper : AppManifest
    {
        private const string AppIdKey = "appid";
        private const string AppNameKey = "name";
        private const string AutoUpdateBehaviorKey = "AutoUpdateBehavior";
        private const string AllowOtherDownloadsWhileRunningKey = "AllowOtherDownloadsWhileRunning";
        private VdfFileNode fileData;

        public AppManifestFileWrapper(VdfFileNode fileData)
        {
            this.fileData = fileData;
        }

        public int AppId
        {
            get
            {
                VerifyExists(AppIdKey);
                return fileData.GetNodeAt(AppIdKey).NodeInt;
            }
        }

        public string Name
        {
            get
            {
                VerifyExists(AppNameKey);
                return fileData.GetNodeAt(AppNameKey).NodeString;
            }
        }

        public int AutoUpdateBehavior
        {
            get
            {
                VerifyExists(AutoUpdateBehaviorKey);
                return fileData.GetNodeAt(AutoUpdateBehaviorKey).NodeInt;
            }
            set
            {
                fileData.PutNodeAt(AutoUpdateBehaviorKey, AutoUpdateBehavior);
            }
        }

        public int AllowOtherDownloadsWhileRunning
        {
            get
            {
                VerifyExists(AllowOtherDownloadsWhileRunningKey);
                return fileData.GetNodeAt(AllowOtherDownloadsWhileRunningKey).NodeInt;
            }
            set
            {
                fileData.PutNodeAt(AllowOtherDownloadsWhileRunningKey, AllowOtherDownloadsWhileRunning);
            }
        }

        private void VerifyExists(string nodeName)
        {

            if (!fileData.ContainsKey(nodeName))
            {
                throw new ContextMarshalException("Failed to find " + nodeName + " in App Manifest file");
            }
        }
    }
}
