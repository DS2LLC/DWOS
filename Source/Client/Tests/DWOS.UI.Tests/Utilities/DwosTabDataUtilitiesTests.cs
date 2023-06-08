using DWOS.UI.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.UI.Tests.Utilities
{
    [TestClass]
    public class DwosTabDataUtilitiesTests
    {
        [TestMethod]
        public void ImportExportTest()
        {
            var tabData = new DwosTabData
            {
                Name = "Test",
                Layout = "Layout String",
                Version = 1,
                DataType = "Type",
                Key = "AAA"
            };
            Assert.IsTrue(tabData.IsValid);

            var xmlDocument = DwosTabDataUtilities.CreateDocument(tabData);
            Assert.IsNotNull(xmlDocument);

            var actualData = DwosTabDataUtilities.Import(xmlDocument);
            Assert.IsNotNull(actualData);
            Assert.AreEqual(tabData.IsValid, true);
            Assert.AreEqual(tabData.Name, actualData.Name);
            Assert.AreEqual(tabData.DataType, actualData.DataType);
            Assert.AreEqual(tabData.Key, actualData.Key);
            Assert.AreEqual(tabData.Layout, actualData.Layout);
            Assert.AreEqual(tabData.Version, actualData.Version);
        }
    }
}
