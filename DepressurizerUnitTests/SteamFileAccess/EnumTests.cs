using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Depressurizer.SteamFileAccess;

namespace DepressurizerUnitTests.SteamFileAccess
{
    [TestClass]
    public class EnumTests
    {
        [TestMethod]
        public void AssertEnumsCastCorrectly()
        {
            // given
            Enums.GameAutoUpdateSetting enumValue = Enums.GameAutoUpdateSetting.HighPriority;

            // when 
            int intValue = (int)enumValue;

            // then
            Assert.AreEqual(intValue, 2);
        }
    }
}
