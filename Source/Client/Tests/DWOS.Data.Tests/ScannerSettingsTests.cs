using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.Data.Tests
{
    [TestClass]
    public class ScannerSettingsTests
    {
        [TestMethod]
        public void JsonConverterTest()
        {
            var settings = new ScannerSettings()
            {
                ScanDeviceName = "Example",
                ScanOutputPDF = true,
                ScanQuality = 80,
                ScanResolution = 300,
                ScanShowFullUI = true
            };

            var target = new ScannerSettings.JsonConverter();
            object serialized = target.ConvertToField(settings);
            Assert.IsNotNull(serialized);

            var deserialized = target.ConvertFromField(serialized) as ScannerSettings;
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(settings.ScanDeviceName, deserialized.ScanDeviceName);
            Assert.AreEqual(settings.ScanOutputPDF, deserialized.ScanOutputPDF);
            Assert.AreEqual(settings.ScanQuality, deserialized.ScanQuality);
            Assert.AreEqual(settings.ScanResolution, deserialized.ScanResolution);
            Assert.AreEqual(settings.ScanShowFullUI, deserialized.ScanShowFullUI);
        }

        [TestMethod]
        public void JsonConverterNullTest()
        {
            var target = new ScannerSettings.JsonConverter();
            Assert.IsNull(target.ConvertToField(null));
            Assert.IsNull(target.ConvertFromField(null));
        }
    }
}
