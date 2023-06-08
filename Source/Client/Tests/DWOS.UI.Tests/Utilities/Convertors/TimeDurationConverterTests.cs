using System.Globalization;
using DWOS.Data;
using DWOS.UI.Utilities.Convertors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.UI.Tests.Utilities.Convertors
{
    [TestClass]
    public class TimeDurationConverterTests
    {
        [TestMethod]
        public void ConvertTest()
        {
            var culture = CultureInfo.InvariantCulture;
            const int valueMinutes = 55;
            var expected = DateUtilities.ToDifferenceShortHand(valueMinutes);

            var target = new TimeDurationConverter();
            var actual = target.Convert(valueMinutes, typeof(string), null, culture) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertBackTest()
        {
            var culture = CultureInfo.InvariantCulture;
            const int expectedMinutes = 55;
            var value = DateUtilities.ToDifferenceShortHand(expectedMinutes);

            var target = new TimeDurationConverter();
            var actualMinutes = target.ConvertBack(value, typeof(int), null, culture) as int? ?? default(int);
            Assert.AreEqual(expectedMinutes, actualMinutes);
        }
    }
}
