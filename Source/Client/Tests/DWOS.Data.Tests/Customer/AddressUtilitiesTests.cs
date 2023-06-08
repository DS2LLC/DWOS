using DWOS.Data.Customer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.Data.Tests.Customer
{
    [TestClass]
    public class AddressUtilitiesTests
    {
        [TestMethod]
        public void GetCountryIdTest()
        {
            Assert.AreEqual(AddressUtilities.GetCountryId("AA"), AddressUtilities.COUNTRY_ID_UNKNOWN);
            Assert.AreEqual(AddressUtilities.GetCountryId("FL"), AddressUtilities.COUNTRY_ID_USA);
            Assert.AreEqual(AddressUtilities.GetCountryId("BC"), AddressUtilities.COUNTRY_ID_CANADA);
        }

        [TestMethod]
        public void IsInUnitedStatesTest()
        {
            Assert.IsTrue(AddressUtilities.IsInUnitedStates("FL"));
            Assert.IsTrue(AddressUtilities.IsInUnitedStates("fl"));
            Assert.IsTrue(AddressUtilities.IsInUnitedStates("Fl"));

            Assert.IsFalse(AddressUtilities.IsInUnitedStates("BC"));
            Assert.IsFalse(AddressUtilities.IsInUnitedStates(null));
            Assert.IsFalse(AddressUtilities.IsInUnitedStates(string.Empty));
        }

        [TestMethod]
        public void IsInCanadaTest()
        {
            Assert.IsTrue(AddressUtilities.IsInCanada("AB"));
            Assert.IsTrue(AddressUtilities.IsInCanada("ab"));
            Assert.IsTrue(AddressUtilities.IsInCanada("Ab"));

            Assert.IsFalse(AddressUtilities.IsInCanada("FL"));
            Assert.IsFalse(AddressUtilities.IsInCanada(null));
            Assert.IsFalse(AddressUtilities.IsInCanada(string.Empty));
        }
    }
}
