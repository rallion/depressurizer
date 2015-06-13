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
using Depressurizer;
using System.IO;
using Rallion;

namespace DepressurizerUnitTests
{
    class VdfFileTest{

        private const string SimpleConfig = "Data/simple_sharedconfig.vdf";
        private const string SimpleLocalConfig = "Data/simple_localconfig.vdf";
        private const string ComplexedConfig = "Data/complexed_sharedconfig.vdf";
        private const string ComplexedLocalConfig = "Data/complexed_localconfig.vdf";
        private const string AppOneConfig = "Data/appmanifest_212680.acf";
        private const string AppTwoConfig = "Data/appmanifest_244850.acf";

        [TestClass]
        public class VdfFileNodeTest
        {
            [TestMethod]
            public void Constructor_WithBlankInput_CreatesBlankArray()
            {

                // given

                // when
                VdfFileNode vdfNode = new VdfFileNode() { };

                // then
                Assert.AreEqual(vdfNode.NodeType, Depressurizer.ValueType.Array);
            }

            [TestMethod]
            public void Constructor_WithBasicLocalFileInput_CreatesStringVdf()
            {

                // given
                string filePath = SimpleLocalConfig;

                // when
                VdfFileNode fileData;
                using (StreamReader reader = new StreamReader(filePath, false))
                {
                    fileData = VdfFileNode.LoadFromText(reader, true);
                }

                // then
                Assert.AreEqual(fileData.NodeType, Depressurizer.ValueType.Array);
            }

            [TestMethod]
            public void Constructor_WithBasicSharedFileInput_CreatesVdf()
            {

                // given
                string filePath = SimpleConfig;

                // when
                VdfFileNode fileData;
                using (StreamReader reader = new StreamReader(filePath, false))
                {
                    fileData = VdfFileNode.LoadFromText(reader, true);
                }

                // then
                Assert.AreEqual(fileData.NodeType, Depressurizer.ValueType.Array);
                Assert.IsTrue(fileData.ContainsKey("Software"));
            }

            [TestMethod]
            public void Constructor_WithComplexedSharedFileInput_CreatesStringValue()
            {

                // given
                string filePath = ComplexedConfig;

                // when
                VdfFileNode fileData;
                using (StreamReader reader = new StreamReader(filePath, false))
                {
                    fileData = VdfFileNode.LoadFromText(reader, true);
                }

                // then
                Assert.AreEqual(fileData.NodeType, Depressurizer.ValueType.Array);
            }

            [TestMethod]
            public void Constructor_WithComplexedLocalFileInput_CreatesVdf()
            {

                // given
                string filePath = ComplexedLocalConfig;

                // when
                VdfFileNode fileData;
                using (StreamReader reader = new StreamReader(filePath, false))
                {
                    fileData = VdfFileNode.LoadFromText(reader, true);
                }

                // then
                Assert.AreEqual(fileData.NodeType, Depressurizer.ValueType.Array);
                Assert.IsTrue(fileData.ContainsKey("friends"));
            }

            [TestMethod]
            public void Constructor_WithAcfFileInput_CreatesVdf()
            {

                // given
                string filePath = AppOneConfig;

                // when
                VdfFileNode fileData;
                using (StreamReader reader = new StreamReader(filePath, false))
                {
                    fileData = VdfFileNode.LoadFromText(reader, true);
                }

                // then
                Assert.AreEqual(fileData.NodeType, Depressurizer.ValueType.Array);
                Assert.IsFalse(fileData.ContainsKey("friends"));
                Assert.IsTrue(fileData.ContainsKey("AutoUpdateBehavior"));
            }

            [TestMethod]
            public void Constructor_WithOtherAcfFileInput_CreatesVdf()
            {

                // given
                string filePath = AppTwoConfig;

                // when
                VdfFileNode fileData;
                using (StreamReader reader = new StreamReader(filePath, false))
                {
                    fileData = VdfFileNode.LoadFromText(reader, true);
                }

                // then
                Assert.AreEqual(fileData.NodeType, Depressurizer.ValueType.Array);
                Assert.IsFalse(fileData.ContainsKey("friends"));
                Assert.IsTrue(fileData.ContainsKey("AutoUpdateBehavior"));
            }
        }
    }
}
