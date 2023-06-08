using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DWOS.Shared.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.Shared.Tests.Utilities
{
    [TestClass]
    public class VersionUtilitiesTests
    {
        [TestMethod]
        public void MinTest()
        {
            var a = new Version(1, 0);
            var b = new Version(2, 0);

            Assert.AreEqual(a, VersionUtilities.Min(a, b));
            Assert.AreEqual(a, VersionUtilities.Min(b, a));
        }

        [TestMethod]
        public void MinNullTest()
        {
            var a = new Version(1, 0);
            var b = new Version(2, 0);

            Assert.AreEqual(null, VersionUtilities.Min(null, null));
            Assert.AreEqual(null, VersionUtilities.Min(a, null));
            Assert.AreEqual(null, VersionUtilities.Min(null, b));
        }
    }
}
