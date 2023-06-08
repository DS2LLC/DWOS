using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DWOS.Data.Datasets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.UI.Tests
{
    [TestClass]
    public class OrderTypeToNameConverterTests
    {
        [TestMethod]
        public void ConvertTest()
        {
            var target = new OrderTypeToNameConverter();
            var culture= CultureInfo.InvariantCulture;

            var expected = "External Rework";
            var type = (int)OrderType.ReworkExt;
            Assert.AreEqual(expected, target.Convert(type, typeof(OrderType), null, culture));

            expected = "Internal Rework";
            type = (int)OrderType.ReworkInt;
            Assert.AreEqual(expected, target.Convert(type, typeof(OrderType), null, culture));

            expected = "Rework Hold";
            type = (int)OrderType.ReworkHold;
            Assert.AreEqual(expected, target.Convert(type, typeof(OrderType), null, culture));

            expected = OrderType.Lost.ToString();
            type = (int)OrderType.Lost;
            Assert.AreEqual(expected, target.Convert(type, typeof(OrderType), null, culture));

            expected = OrderType.Normal.ToString();
            type = (int)OrderType.Normal;
            Assert.AreEqual(expected, target.Convert(type, typeof(OrderType), null, culture));

            expected = OrderType.Quarantine.ToString();
            type = (int)OrderType.Quarantine;
            Assert.AreEqual(expected, target.Convert(type, typeof(OrderType), null, culture));
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ConvertBackTest()
        {
            var target = new OrderTypeToNameConverter();
            target.ConvertBack(null, null, null, null);
        }
    }
}
