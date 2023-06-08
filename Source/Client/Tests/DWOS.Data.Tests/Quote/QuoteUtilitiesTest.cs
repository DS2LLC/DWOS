using DWOS.Data.Datasets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static DWOS.Data.Quote.QuoteUtilities;

namespace DWOS.Data.Tests.Quote
{
    [TestClass]
    public class QuoteUtilitiesTest
    {
        [TestMethod]
        public void GetDisplayStringTest()
        {
            using (var dsTest = new QuoteDataSet())
            {
                // No customer
                var quote = dsTest.Quote.NewQuoteRow();
                quote.QuoteID = 1000;
                Assert.AreEqual("1000", GetDisplayString(quote, "%ID%"));
                Assert.AreEqual("1000", GetDisplayString(quote, "%ID% %REQUIREDDATE%"));
                Assert.AreEqual("1000", GetDisplayString(quote, "%ID% %REQUIREDDATE% %CUSTOMERNAME%"));

                // With customer
                var customer = dsTest.Customer.NewCustomerRow();
                customer.Name = "Customer Name";
                customer.LeadTime = 30;
                dsTest.Customer.AddCustomerRow(customer);
                quote.CustomerRow = customer;

                Assert.AreEqual("1000", GetDisplayString(quote, "%ID%"));
                Assert.AreEqual("1000", GetDisplayString(quote, "%ID% %REQUIREDDATE%"));
                Assert.AreEqual("1000 Customer Name", GetDisplayString(quote, "%ID% %REQUIREDDATE% %CUSTOMERNAME%"));

                // No format
                Assert.AreEqual("1000", GetDisplayString(quote, null));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetDisplayStringNullTest()
        {
            GetDisplayString(null, null);
        }
    }
}
