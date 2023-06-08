using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DWOS.Shared.Utilities;
using Quartz;
using System.Collections.Generic;

namespace DWOS.Shared.Tests.Utilities
{
    [TestClass]
    public class CronExpressionBuilderTests
    {
        private const string INVALID_EXPRESSION = "Expression is invalid.";

        [TestMethod]
        public void BuildDefaultTest()
        {
            string expected = "0 * * ? * 1-7 *";
            string actual = CronExpressionBuilder.Create().Build().ToString();
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(CronExpression.IsValidExpression(actual), INVALID_EXPRESSION);
        }

        [TestMethod]
        public void BuildTest()
        {
            string expected = "0 1 1 ? * 1,2,3,4,5,6,7 *";
            string actual = CronExpressionBuilder.Create()
                .SetHour(1)
                .SetMinute(1)
                .Add(new List<DayOfWeek>()
                {
                    DayOfWeek.Sunday,
                    DayOfWeek.Monday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Friday,
                    DayOfWeek.Saturday,
                })
                .Build();

            Assert.AreEqual(expected, actual);
            Assert.IsTrue(CronExpression.IsValidExpression(actual), INVALID_EXPRESSION);

            expected = "0 50 1 ? * 1 *";
            actual = CronExpressionBuilder.Create()
                .SetHour(1)
                .SetMinute(50)
                .Add(DayOfWeek.Sunday)
                .Build();

            Assert.AreEqual(expected, actual);
            Assert.IsTrue(CronExpression.IsValidExpression(actual), INVALID_EXPRESSION);
        }

        [TestMethod]
        public void BuildSetTimeTest()
        {
            string expected = CronExpressionBuilder.Create()
                .SetHour(1)
                .SetMinute(50)
                .Add(DayOfWeek.Sunday)
                .Build();

            string actual = CronExpressionBuilder.Create()
                .SetTime(new DateTime(2015, 1, 1, 1, 50, 0).TimeOfDay)
                .Add(DayOfWeek.Sunday)
                .Build();

            Assert.AreEqual(expected, actual);
            Assert.IsTrue(CronExpression.IsValidExpression(actual), INVALID_EXPRESSION);
        }
    }
}
