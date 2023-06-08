using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DWOS.Reports.Utilities;

namespace DWOS.Reports.Tests.Utilities
{
    [TestClass]
    public class ReportStringUtilitiesTests
    {

        [TestMethod]
        public void StripHtmlTest()
        {
            // Empty
            var input = string.Empty;
            var expected = string.Empty;
            Assert.AreEqual(expected, input.StripHtml());

            // Whitespace
            input = "   ";
            expected = string.Empty;
            Assert.AreEqual(expected, input.StripHtml());

            // Text
            input = " Test ";
            expected = "Test";
            Assert.AreEqual(expected, input.StripHtml());

            // HTML
            input = "<i>Test</i>";
            expected = "Test";
            Assert.AreEqual(expected, input.StripHtml());

            input = "<i>Test <b>String</b></i>";
            expected = "Test String";
            Assert.AreEqual(expected, input.StripHtml());

            // Infragistics-specific formatting
            input = "Test&edsp;String";
            expected = "Test String";
            Assert.AreEqual(expected, input.StripHtml());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StripHtmlNullTest()
        {
            string input = null;
            input.StripHtml();
        }
    }
}
