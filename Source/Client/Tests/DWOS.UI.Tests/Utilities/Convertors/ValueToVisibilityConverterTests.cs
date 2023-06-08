using System;
using System.Globalization;
using System.Windows;
using DWOS.UI.Utilities.Convertors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.UI.Tests.Utilities.Convertors
{
    [TestClass]
    public class ValueToVisibilityConverterTests
    {
        [TestMethod]
        public void ConvertTest()
        {
            var culture = CultureInfo.InvariantCulture;
            var value = new object();
            const Visibility expected = Visibility.Visible;

            var target = new ValueToVisibilityConverter();
            var actual = target.Convert(value, typeof(Visibility), null, culture) as Visibility? ?? default(Visibility);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertNullTest()
        {
            var culture = CultureInfo.InvariantCulture;
            object value = null;
            const Visibility expected = Visibility.Collapsed;

            var target = new ValueToVisibilityConverter();
            var actual = target.Convert(value, typeof(Visibility), null, culture) as Visibility? ?? default(Visibility);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertDbNullTest()
        {
            var culture = CultureInfo.InvariantCulture;
            var value = DBNull.Value;
            const Visibility expected = Visibility.Collapsed;

            var target = new ValueToVisibilityConverter();
            var actual = target.Convert(value, typeof(Visibility), null, culture) as Visibility? ?? default(Visibility);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertEmptyStringTest()
        {
            var culture = CultureInfo.InvariantCulture;
            var value = string.Empty;
            const Visibility expected = Visibility.Collapsed;

            var target = new ValueToVisibilityConverter();
            var actual = target.Convert(value, typeof(Visibility), null, culture) as Visibility? ?? default(Visibility);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertStringTest()
        {
            var culture = CultureInfo.InvariantCulture;
            var value = "Test";
            const Visibility expected = Visibility.Visible;

            var target = new ValueToVisibilityConverter();
            var actual = target.Convert(value, typeof(Visibility), null, culture) as Visibility? ?? default(Visibility);
            Assert.AreEqual(expected, actual);
        }
    }
}
