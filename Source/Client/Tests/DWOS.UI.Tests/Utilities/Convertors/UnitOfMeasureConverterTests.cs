using System.Globalization;
using DWOS.UI.Utilities;
using DWOS.UI.Utilities.Convertors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.UI.Tests.Utilities.Convertors
{
    [TestClass]
    public class UnitOfMeasureConverterTests
    {
        [TestMethod]
        public void ConvertFeetTest()
        {
            var culture = CultureInfo.InvariantCulture;
            const UnitOfMeasure value = UnitOfMeasure.Feet;
            const string expected = "ft";

            var target = new UnitOfMeasureConverter();

            var actual = target.Convert(value, typeof(string), null, culture) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertInchTest()
        {
            var culture = CultureInfo.InvariantCulture;
            const UnitOfMeasure value = UnitOfMeasure.Inch;
            const string expected = "in";

            var target = new UnitOfMeasureConverter();

            var actual = target.Convert(value, typeof(string), null, culture) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertPoundTest()
        {
            var culture = CultureInfo.InvariantCulture;
            const UnitOfMeasure value = UnitOfMeasure.Pound;
            const string expected = "lb";

            var target = new UnitOfMeasureConverter();

            var actual = target.Convert(value, typeof(string), null, culture) as string;
            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void ConvertFeetSquaredTest()
        {
            var culture = CultureInfo.InvariantCulture;
            const UnitOfMeasure value = UnitOfMeasure.Feet;
            const string expected = "ft²";

            var target = new UnitOfMeasureConverter();

            var actual = target.Convert(value, typeof(string), "SQUARED", culture) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertInchSquaredTest()
        {
            var culture = CultureInfo.InvariantCulture;
            const UnitOfMeasure value = UnitOfMeasure.Inch;
            const string expected = "in²";

            var target = new UnitOfMeasureConverter();

            var actual = target.Convert(value, typeof(string), "SQUARED", culture) as string;
            Assert.AreEqual(expected, actual);
        }
    }
}
