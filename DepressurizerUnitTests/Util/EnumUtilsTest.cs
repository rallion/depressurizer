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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Depressurizer.SteamFileAccess;
using Depressurizer.Games;
using System.Collections.Generic;
using Depressurizer.SteamFileAccess.ApplicationManifest;
using Depressurizer;
using Depressurizer.Util;

namespace DepressurizerUnitTests.Util
{
    [TestClass]
    public class EnumUtilsTest
    {

        [TestMethod]
        public void EveryValidResultReturnsAnEnum()
        {
            // given

            // when 
            foreach (GameAutoUpdateStyle style in Enum.GetValues(typeof(GameAutoUpdateStyle)))
            {
                foreach (bool favouriteGame in (new bool[] { true, false }))
                {
                    if (GameAutoUpdateStyle.DEFAULT != style)
                    {
                        EnumUtils.GetUpdateSettingForStyleAndFavourite(style, favouriteGame);
                    }
                }
            }

            // then
            // doesn't throw an exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DefaultThrowsAnErrorBecauseTheresNoLogicalMapping()
        {
            // given

            // when 
            EnumUtils.GetUpdateSettingForStyleAndFavourite(GameAutoUpdateStyle.DEFAULT, false);

            // then
            // exception thrown
        }
    }
}
