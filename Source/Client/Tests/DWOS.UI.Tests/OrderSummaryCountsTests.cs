using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.UI.Tests
{
    [TestClass]
    public class OrderSummaryCountsTests
    {
        private const double Delta = 0.00001;

        [TestMethod]
        public void PercentageTests()
        {
            var target = new OrderSummaryCounts
            {
                TotalCount = 20,
                LateCount = 0,
                Day1Count = 1,
                Day2Count = 2,
                Day3Count = 3,
                DueTodayCount = 4,
                QuarantineCount = 1,
                ExtReworkCount = 2,
                IntReworkCount = 3,
                HoldCount = 4,
            };

            Assert.AreEqual(0, target.LateCountPercent, Delta);
            Assert.AreEqual(0.05, target.Day1CountPercent, Delta);
            Assert.AreEqual(0.1, target.Day2CountPercent, Delta);
            Assert.AreEqual(0.15, target.Day3CountPercent, Delta);
            Assert.AreEqual(0.05, target.QuarantineCountPercent, Delta);
            Assert.AreEqual(0.1, target.ExtReworkCountPercent, Delta);
            Assert.AreEqual(0.15, target.IntReworkCountPercent, Delta);
            Assert.AreEqual(0.2, target.HoldCountPercent, Delta);
        }
    }
}
