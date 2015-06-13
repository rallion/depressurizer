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
