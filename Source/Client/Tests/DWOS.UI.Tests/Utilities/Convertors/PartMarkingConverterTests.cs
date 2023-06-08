using System.Globalization;
using DWOS.Data;
using DWOS.UI.Utilities.Convertors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.UI.Tests.Utilities.Convertors
{
    [TestClass]
    public class PartMarkingConverterTests
    {
        [TestMethod]
        public void ConvertTest()
        {
            var culture = CultureInfo.InvariantCulture;

            const string value = "<> Test";
            var expected = value.ToPartMarkingString();

            var target = new PartMarkingConverter();
            var actual = target.Convert(value, typeof(string), null, culture) as string;

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(actual, target.ConvertBack(actual, typeof(string), null, culture));
        }
    }
}
