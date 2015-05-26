using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Depressurizer;
using System.IO;
using Rallion;

namespace DepressurizerUnitTests
{
    class VdfFileTests{

        private const string SimpleConfig = "Data/simple_sharedconfig.vdf";
        private const string SimpleLocalConfig = "Data/simple_localconfig.vdf";
        private const string ComplexedConfig = "Data/complexed_sharedconfig.vdf";
        private const string ComplexedLocalConfig = "Data/complexed_localconfig.vdf";
        private const string AppOneConfig = "Data/appmanifest_212680.acf";
        private const string AppTwoConfig = "Data/appmanifest_244850.acf";

        [TestClass]
        public class VdfFileNodeTests
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
