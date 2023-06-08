using System;
using System.Globalization;
using DWOS.Data;
using DWOS.UI.Utilities.Convertors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.UI.Tests.Utilities.Convertors
{
    [TestClass]
    public class DescriptionConverterTests
    {
        [TestMethod]
        public void ConvertTest()
        {
            var culture = CultureInfo.InvariantCulture;
            var value = OrderPrice.enumPriceUnit.EachByWeight;
            var expected = "Each By Weight";

            var target = new DescriptionConverter();
            var actual = target.Convert(value, typeof(string), null, culture) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ConvertBackTest()
        {
            var target = new DescriptionConverter();
            target.ConvertBack(null, null, null, null);
        }
    }
}
