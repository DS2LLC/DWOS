using System;
using System.Globalization;
using DWOS.Data;
using DWOS.Data.Date;
using DWOS.Shared.Utilities;
using DWOS.UI.Utilities.Convertors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DWOS.UI.Tests.Utilities.Convertors
{
    [TestClass]
    public class TimeToTimeDifferenceConverterTests
    {
        private readonly DateTime _now = new DateTime(2017, 7, 19, 13, 12, 0);
        private Mock<IDateTimeNowProvider> _nowProviderMock;

        [TestInitialize]
        public void Initialize()
        {
            _nowProviderMock = new Mock<IDateTimeNowProvider>();
            _nowProviderMock.Setup(m => m.Now).Returns(_now);
        }

        [TestMethod]
        public void ConvertTest()
        {
            var value = _now.AddMinutes(-30);
            var expected = DateUtilities.ToDifferenceShortHand(30);
            var culture = CultureInfo.InvariantCulture;

            var target = new TimeToTimeDifferenceConverter(_nowProviderMock.Object);
            var actual = target.Convert(value, typeof(DateTime), null, culture) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ConvertBackTest()
        {
            var target = new TimeToTimeDifferenceConverter(_nowProviderMock.Object);
            target.ConvertBack(null, null, null, null);
        }
    }
}
