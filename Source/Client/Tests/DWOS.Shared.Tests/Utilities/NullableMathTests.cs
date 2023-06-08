using DWOS.Shared.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWOS.Shared.Tests.Utilities
{
    [TestClass]
    public class NullableMathTests
    {
        [TestMethod]
        public void RoundDecimalTest()
        {
            decimal expected;
            decimal? actual;

            expected = 5M;
            actual = NullableMath.Round(5.45M, 0);
            Assert.AreEqual(expected, actual);

            expected = 6M;
            actual = NullableMath.Round(5.5M, 0);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RoundNullDecimalTest()
        {
            var result = NullableMath.Round((decimal?)null, 0);
            Assert.IsFalse(result.HasValue);
        }
    }
}
