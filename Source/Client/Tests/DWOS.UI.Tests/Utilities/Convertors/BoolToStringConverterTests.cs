using System.Globalization;
using DWOS.UI.Utilities.Convertors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.UI.Tests.Utilities.Convertors
{
    [TestClass]
    public class BoolToStringConverterTests
    {
        private const string True = "TrueValue";
        private const string False = "FalseValue";

        [TestMethod]
        public void ConvertTest()
        {
            var culture = CultureInfo.InvariantCulture;
            var target = new BoolToStringConverter
            {
                TrueValue = True,
                FalseValue = False
            };

            Assert.AreEqual(True, target.Convert(true, typeof(bool), null, culture));
            Assert.AreEqual(False, target.Convert(false, typeof(bool), null, culture));
        }

        [TestMethod]
        public void ConvertBackTest()
        {
            var culture = CultureInfo.InvariantCulture;
            var target = new BoolToStringConverter
            {
                TrueValue = True,
                FalseValue = False
            };

            Assert.AreEqual(true, target.ConvertBack(True, typeof(string), null, culture));
            Assert.AreEqual(false, target.ConvertBack(False, typeof(string), null, culture));
        }
    }
}
