using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depressurizer.SteamFileAccess.ApplicationManifest
{
    public interface AppManifest
    {
        int AppId
        {
            get;
        }

        string Name
        {
            get;
        }

        int AutoUpdateBehavior
        {
            get;
            set;
        }

        int AllowOtherDownloadsWhileRunning
        {
            get;
            set;
        }

        VdfFileNode ExportToNode();
    }
}
