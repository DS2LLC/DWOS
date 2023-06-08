using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace DWOS.Shared.Tests
{
    [TestClass]
    public class AboutTests
    {
        [TestMethod]
        public void CurrentAssemblyTest()
        {
            var expected = Assembly.GetCallingAssembly();
            var actual = About.CurrentAssembly;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.FullName, actual.FullName);
        }

        [TestMethod]
        public void ApplicationReleaseDateTest()
        {
            // Currently Assembly is one for running VS Unit Tests.
            var actual = About.ApplicationReleaseDate;
            Assert.IsFalse(actual.HasValue);
        }

        [TestMethod]
        public void GetReleaseDateTest()
        {
            var expectedMinimum = new DateTime(2016, 2, 26);
            var actual = About.GetReleaseDate(typeof(AboutTests).Assembly);
            Assert.IsTrue(actual.HasValue);
            Assert.IsTrue(actual.Value >= expectedMinimum);
        }
    }
}
