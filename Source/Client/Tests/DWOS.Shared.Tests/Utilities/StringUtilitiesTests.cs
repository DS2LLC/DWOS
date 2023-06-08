using DWOS.Shared.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DWOS.Shared.Tests.Utilities
{
    [TestClass]
    public class StringUtilitiesTests
    {
        [TestMethod]
        public void ToDisplayTextTest()
        {
            // Case - 0
            var items = new List<string>();
            Assert.AreEqual(string.Empty, StringUtilities.ToDisplayText(items));

            // Case - 1
            items.Add("A");
            Assert.AreEqual("A", StringUtilities.ToDisplayText(items));

            // Case - 2
            items.Add("B");
            Assert.AreEqual("A and B", StringUtilities.ToDisplayText(items));

            // Case - 3
            items.Add("C");
            Assert.AreEqual("A, B, and C", StringUtilities.ToDisplayText(items));
        }

        [TestMethod]
        public void ToPluralTest()
        {
            // Case - empty
            Assert.AreEqual(string.Empty, StringUtilities.ToPlural(string.Empty));

            // Case - not in list
            Assert.AreEqual("crates", StringUtilities.ToPlural("crate"));
            Assert.AreEqual("Crates", StringUtilities.ToPlural("Crate"));

            // Case - in list
            Assert.AreEqual("boxes", StringUtilities.ToPlural("box"));
            Assert.AreEqual("Boxes", StringUtilities.ToPlural("Box"));
        }
    }
}
