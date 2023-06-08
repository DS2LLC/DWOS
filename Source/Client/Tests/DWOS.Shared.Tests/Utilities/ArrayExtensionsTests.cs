using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.Shared.Tests.Utilities
{
    [TestClass]
    public class ArrayExtensionsTests
    {
        [TestMethod]
        public void ContainsTest()
        {
            string[] target;
            string value;

            target = new string[] { "One", "Two", "Three" };
            value = "One";
            Assert.IsTrue(target.Contains(value, true));
            value = "Two";
            Assert.IsTrue(target.Contains(value, true));
            value = "Three";
            Assert.IsTrue(target.Contains(value, true));
            value = "three";
            Assert.IsFalse(target.Contains(value, true));
            value = "THREE";
            Assert.IsFalse(target.Contains(value, true));

            target = new string[0];
            Assert.IsFalse(target.Contains("One", true));
        }

        [TestMethod]
        public void ContainsIgnoreCaseTest()
        {
            string[] target;
            string value;

            target = new string[] { "One", "Two", "Three" };
            value = "one";
            Assert.IsTrue(target.Contains(value, false));
            value = "two";
            Assert.IsTrue(target.Contains(value, false));
            value = "three";
            Assert.IsTrue(target.Contains(value, false));
            value = "THREE";
            Assert.IsTrue(target.Contains(value, false));

            target = new string[0];
            Assert.IsFalse(target.Contains("One", false));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ContainsNullTest()
        {
            string[] target = null;
            target.Contains(string.Empty, true);
        }
    }
}
