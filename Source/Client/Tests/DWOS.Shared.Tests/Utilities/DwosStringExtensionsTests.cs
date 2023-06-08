using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DWOS.Shared.Utilities;

namespace DWOS.Shared.Tests.Utilities
{
    [TestClass]
    public class DwosStringExtensionsTests
    {
        [TestMethod]
        public void ToInitialsFirstTwoTest()
        {
            string name;
            string expected;

            // Empty
            name = string.Empty;
            expected = string.Empty;
            Assert.AreEqual(expected, name.ToInitials(StringInitialOption.FirstTwoInitials));

            // Whitespace
            name = "   ";
            expected = string.Empty;
            Assert.AreEqual(expected, name.ToInitials(StringInitialOption.FirstTwoInitials));

            // First Name
            name = "Test";
            expected = "T";
            Assert.AreEqual(expected, name.ToInitials(StringInitialOption.FirstTwoInitials));

            // First & Last Names
            name = "Test User";
            expected = "TU";
            Assert.AreEqual(expected, name.ToInitials(StringInitialOption.FirstTwoInitials));

            // First, Middle, & Last Names
            name = "Test A. User";
            expected = "TA";
            Assert.AreEqual(expected, name.ToInitials(StringInitialOption.FirstTwoInitials));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToInitialsFirstTwoNullTest()
        {
            string nullValue = null;
            nullValue.ToInitials(StringInitialOption.FirstTwoInitials);
        }

        [TestMethod]
        public void ToInitialsAllInitialsTest()
        {
            string name;
            string expected;

            // Empty
            name = string.Empty;
            expected = string.Empty;
            Assert.AreEqual(expected, name.ToInitials(StringInitialOption.AllInitials));

            // Whitespace
            name = "   ";
            expected = string.Empty;
            Assert.AreEqual(expected, name.ToInitials(StringInitialOption.AllInitials));

            // First Name
            name = "Test";
            expected = "T";
            Assert.AreEqual(expected, name.ToInitials(StringInitialOption.AllInitials));

            // First & Last Names
            name = "Test User";
            expected = "TU";
            Assert.AreEqual(expected, name.ToInitials(StringInitialOption.AllInitials));

            // First, Middle, & Last Names
            name = "Test A. User";
            expected = "TAU";
            Assert.AreEqual(expected, name.ToInitials(StringInitialOption.AllInitials));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToInitialsAllInitialsNullTest()
        {
            string nullValue = null;
            nullValue.ToInitials(StringInitialOption.AllInitials);
        }
    }
}
