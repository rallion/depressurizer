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
using DepressurizerUnitTests.TestUtils;

namespace DepressurizerUnitTests
{
    class VdfFileTest{

        private const string SimpleConfigPath = "Data/simple_sharedconfig.vdf";
        private const string SimpleLocalConfigPath = "Data/simple_localconfig.vdf";
        private const string ComplexedConfigPath = "Data/complexed_sharedconfig.vdf";
        private const string ComplexedLocalConfigPath = "Data/complexed_localconfig.vdf";
        private const string OverloadedConfigPath = "Data/simple_overloadedconfig.vdf";
        private const string AppOneConfigPath = "Data/steamapps/appmanifest_212680.acf";
        private const string AppTwoConfigPath = "Data/steamapps/appmanifest_244850.acf";

        [TestClass]
        public class VdfFileNodeTest
        {

            [TestInitialize()]
            public void Initialize()
            {
                TestIO.setUpTempFolder();
            }

            [TestCleanup()]
            public void Cleanup()
            {
                TestIO.tearDownTempFolder();
            }

            [TestMethod]
            public void Constructor_WithBlankInput_CreatesBlankArray()
            {

                // given

                // when
                VdfFileNode vdfNode = new VdfFileNode();

                // then
                Assert.AreEqual(vdfNode.NodeType, Depressurizer.ValueType.Array);
            }

            [TestMethod]
            public void Constructor_WithBasicLocalFileInput_CreatesStringVdf()
            {

                // given

                // when
                VdfFileNode fileData = CreateVdfNodeFromFilePath(SimpleLocalConfigPath);

                // then
                Assert.AreEqual(fileData.NodeType, Depressurizer.ValueType.Array);
            }

            [TestMethod]
            public void Constructor_WithBasicSharedFileInput_CreatesVdf()
            {

                // given

                // when
                VdfFileNode fileData = CreateVdfNodeFromFilePath(SimpleConfigPath);

                // then
                Assert.AreEqual(fileData.NodeType, Depressurizer.ValueType.Array);
                Assert.IsTrue(fileData.ContainsKey("UserRoamingConfigStore"));
            }

            [TestMethod]
            public void Constructor_WithComplexedSharedFileInput_CreatesStringValue()
            {

                // given

                // when
                VdfFileNode fileData = CreateVdfNodeFromFilePath(ComplexedConfigPath);

                // then
                Assert.AreEqual(fileData.NodeType, Depressurizer.ValueType.Array);
            }

            [TestMethod]
            public void Constructor_WithComplexedLocalFileInput_CreatesVdf()
            {

                // given

                // when
                VdfFileNode fileData = CreateVdfNodeFromFilePath(ComplexedLocalConfigPath);

                // then
                Assert.AreEqual(fileData.NodeType, Depressurizer.ValueType.Array);
                Assert.IsTrue(fileData.ContainsKey("UserLocalConfigStore"));
            }

            [TestMethod]
            public void Constructor_WithAcfFileInput_CreatesVdf()
            {

                // given

                // when
                VdfFileNode fileData = CreateVdfNodeFromFilePath(AppOneConfigPath);

                // then
                Assert.AreEqual(fileData.NodeType, Depressurizer.ValueType.Array);
                Assert.IsTrue(fileData.ContainsKey("AppState"));

                VdfFileNode topLevel = fileData.GetNodeAt("AppState");
                Assert.IsFalse(topLevel.ContainsKey("friends"));
                Assert.IsTrue(topLevel.ContainsKey("AutoUpdateBehavior"));
            }

            [TestMethod]
            public void Constructor_WithOtherAcfFileInput_CreatesVdf()
            {

                // given

                // when
                VdfFileNode fileData = CreateVdfNodeFromFilePath(AppTwoConfigPath);

                // then
                Assert.AreEqual(fileData.NodeType, Depressurizer.ValueType.Array);
                Assert.IsTrue(fileData.ContainsKey("AppState"));

                VdfFileNode topLevel = fileData.GetNodeAt("AppState");
                Assert.IsFalse(topLevel.ContainsKey("friends"));
                Assert.IsTrue(topLevel.ContainsKey("AutoUpdateBehavior"));
            }

            [TestMethod]
            public void Constructor_MoreThanOneBase_IncludesAll()
            {

                // given

                // when
                VdfFileNode fileData = CreateVdfNodeFromFilePath(OverloadedConfigPath);

                // then
                Assert.AreEqual(fileData.NodeType, Depressurizer.ValueType.Array);
                Assert.IsFalse(fileData.ContainsKey("friends"));
                Assert.IsTrue(fileData.ContainsKey("UserRoamingConfigStore"));
                Assert.IsTrue(fileData.ContainsKey("NewNode"));
            }

            [TestMethod]
            public void Constructor_IntSavedAndLoaded_LoadsAsStringNotInt()
            {

                // given
                VdfFileNode fileData = new VdfFileNode();
                fileData.PutNodeAt("number", 2);
                string testFilePath = TestIO.getTempFolderPath() + "testFileName.vdf";

                // when
                SaveSteamFileNodeToFile(fileData, testFilePath);
                VdfFileNode retrieved = CreateVdfNodeFromFilePath(testFilePath);

                // then
                Assert.IsTrue(fileData.ContainsKey("number"));
                Assert.IsTrue(retrieved.ContainsKey("number"));
                Assert.AreEqual(fileData.GetNodeAt("number").NodeType, Depressurizer.ValueType.Int); // System actually has a ValueType
                Assert.AreEqual(retrieved.GetNodeAt("number").NodeType, Depressurizer.ValueType.String);
                Assert.AreNotEqual(retrieved, fileData);
            }

            [TestMethod]
            public void PutNodeAt_NewNode_AddsNewNode()
            {

                // given
                VdfFileNode fileData = CreateVdfNodeFromFilePath(SimpleConfigPath);
                string testFileName = TestIO.getTempFolderPath() + "testFileName.vdf";

                // when
                fileData.PutNodeAt("NewNode", "Hello");

                // then
                Assert.AreEqual(fileData.NodeType, Depressurizer.ValueType.Array);
                Assert.IsFalse(fileData.ContainsKey("friends"));
                Assert.IsTrue(fileData.ContainsKey("UserRoamingConfigStore"));
                Assert.IsTrue(fileData.ContainsKey("NewNode"));
            }

            [TestMethod]
            public void PutNodeAt_Base_SavesAndLoads()
            {

                // given
                VdfFileNode fileData = CreateVdfNodeFromFilePath(SimpleConfigPath);
                string testFilePath = TestIO.getTempFolderPath() + "testFileName.vdf";
                fileData.PutNodeAt("NewNode", "Hello");

                // when
                SaveSteamFileNodeToFile(fileData, testFilePath);
                VdfFileNode retrieved = CreateVdfNodeFromFilePath(testFilePath);

                // then
                Assert.IsFalse(fileData.ContainsKey("friends"));
                Assert.IsFalse(retrieved.ContainsKey("friends"));
                Assert.IsTrue(fileData.ContainsKey("UserRoamingConfigStore"));
                Assert.IsTrue(retrieved.ContainsKey("UserRoamingConfigStore"));
                Assert.IsTrue(fileData.ContainsKey("NewNode"));
                Assert.IsTrue(retrieved.ContainsKey("NewNode"));
                Assert.AreEqual(retrieved, fileData);
            }

            private static VdfFileNode CreateVdfNodeFromFilePath(string filePath)
            {
                VdfFileNode fileData;
                using (StreamReader reader = new StreamReader(filePath, false))
                {
                    fileData = VdfFileNode.LoadFromText(reader, false);
                }
                return fileData;
            }

            private static void SaveSteamFileNodeToFile(VdfFileNode manifestNode, string fileName)
            {
                using (StreamWriter writer = new StreamWriter(File.Open(fileName, FileMode.Create)))
                {
                    manifestNode.SaveAsText(writer, 0);
                }
            }
        }
    }
}
