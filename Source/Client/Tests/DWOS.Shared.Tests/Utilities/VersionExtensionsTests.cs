using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DWOS.Shared.Tests.Utilities
{
    [TestClass]
    public class VersionExtensionsTests
    {
        [TestMethod]
        public void ToMajorMinorBuildTest()
        {
            var target = new Version(1, 2, 3, 4);
            var majorMinorBuild = target.ToMajorMinorBuild();

            Assert.IsNotNull(majorMinorBuild);
            Assert.AreEqual(1, majorMinorBuild.Major);
            Assert.AreEqual(2, majorMinorBuild.Minor);
            Assert.AreEqual(3, majorMinorBuild.Build);
            Assert.AreEqual(0, majorMinorBuild.Revision);
        }
    }
}
