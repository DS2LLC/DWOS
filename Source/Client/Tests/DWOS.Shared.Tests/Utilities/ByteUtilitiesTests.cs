using DWOS.Shared.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.Shared.Tests.Utilities
{
    [TestClass]
    public class ByteUtilitiesTests
    {
        [TestMethod]
        public void AreEqualTest()
        {
            Assert.IsTrue(ByteUtilities.AreEqual(null, null));
            Assert.IsFalse(ByteUtilities.AreEqual(new byte[0], null));
            Assert.IsFalse(ByteUtilities.AreEqual(null, new byte[0]));
            Assert.IsTrue(ByteUtilities.AreEqual(new byte[0], new byte[0]));
            Assert.IsFalse(ByteUtilities.AreEqual(new byte[1], new byte[0]));

            var a = new byte[] { 1, 2, 3 };
            var b = new byte[] { 1, 2, 3 };

            Assert.IsTrue(ByteUtilities.AreEqual(a, a));
            Assert.IsTrue(ByteUtilities.AreEqual(a, b));
            Assert.IsTrue(ByteUtilities.AreEqual(b, b));

            b = new byte[] { 1, 2};

            Assert.IsFalse(ByteUtilities.AreEqual(a, b));
        }
    }
}
