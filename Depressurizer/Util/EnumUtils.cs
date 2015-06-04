using Depressurizer.SteamFileAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depressurizer.Util
{
    public static class EnumUtils
    {
        public static Enums.GameAutoUpdateSetting GetUpdateSettingForStyleAndFavourite(
            GameAutoUpdateStyle style, bool favouriteGame)
        {
            if(GameAutoUpdateStyle.DEFAULT.Equals(style))
            {
                throw new ArgumentException("For default game auto update style, don't try to update the games at all");
            }

            if (GameAutoUpdateStyle.NONE.Equals(style))
            {
                return Enums.GameAutoUpdateSetting.OnlyOnLaunch;
            }

            if (GameAutoUpdateStyle.ONLY_FAV.Equals(style))
            {
                if (favouriteGame)
                {
                    return Enums.GameAutoUpdateSetting.AlwaysUpToDate;
                }
                return Enums.GameAutoUpdateSetting.OnlyOnLaunch;
            }

            if (GameAutoUpdateStyle.FAV_PRIORITY.Equals(style))
            {
                if (favouriteGame)
                {
                    return Enums.GameAutoUpdateSetting.HighPriority;
                }
                return Enums.GameAutoUpdateSetting.AlwaysUpToDate;
            }

            throw new ArgumentException("No valid result found");

        }
    }
}
