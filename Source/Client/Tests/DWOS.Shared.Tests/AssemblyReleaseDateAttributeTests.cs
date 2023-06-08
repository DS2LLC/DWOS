using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWOS.Shared.Tests
{
    [TestClass]
    public class AssemblyReleaseDateAttributeTests
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var target = new AssemblyReleaseDateAttribute("2016-02-01");
            Assert.IsTrue(target.Date.HasValue);
            Assert.AreEqual(target.Date.Value, new DateTime(2016, 2, 1));
        }

        [TestMethod]
        public void ConstructorNullTest()
        {
            var target = new AssemblyReleaseDateAttribute(null);
            Assert.IsFalse(target.Date.HasValue);
        }

        [TestMethod]
        public void ConstructorInvalidTest()
        {
            var target = new AssemblyReleaseDateAttribute("test string");
            Assert.IsFalse(target.Date.HasValue);
        }
    }
}
