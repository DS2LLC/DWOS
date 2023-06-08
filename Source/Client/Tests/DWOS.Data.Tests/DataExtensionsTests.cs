using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DWOS.Data.Datasets;

namespace DWOS.Data.Tests
{
    [TestClass]
    public class DataExtensionsTests
    {
        [TestMethod]
        public void GetNotificationEmailAddressesTest()
        {
            using (var _dtShipment = new OrderShipmentDataSet.ShipmentPackageDataTable())
            {
                var shipment = _dtShipment.NewShipmentPackageRow();
                IEnumerable<string> emailAddresses;

                // Empty case
                shipment.NotificationEmails = string.Empty;
                emailAddresses = shipment.GetNotificationEmailAddresses();
                Assert.AreEqual(0, emailAddresses.Count());

                // 1
                shipment.NotificationEmails = "test@example.com";
                emailAddresses = shipment.GetNotificationEmailAddresses();
                Assert.AreEqual(1, emailAddresses.Count());
                Assert.IsTrue(emailAddresses.Contains("test@example.com"));

                // 2
                shipment.NotificationEmails = "test1@example.com,test2@example.com" ;
                emailAddresses = shipment.GetNotificationEmailAddresses();
                Assert.AreEqual(2, emailAddresses.Count());
                Assert.IsTrue(emailAddresses.Contains("test1@example.com"));
                Assert.IsTrue(emailAddresses.Contains("test2@example.com"));

                // 3
                shipment.NotificationEmails = "testA@example.com,testB@example.com,testC@example.com";
                emailAddresses = shipment.GetNotificationEmailAddresses();
                Assert.AreEqual(3, emailAddresses.Count());
                Assert.IsTrue(emailAddresses.Contains("testA@example.com"));
                Assert.IsTrue(emailAddresses.Contains("testB@example.com"));
                Assert.IsTrue(emailAddresses.Contains("testC@example.com"));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNotificationEmailAddressIsNullTest()
        {
            using (var _dtShipment = new OrderShipmentDataSet.ShipmentPackageDataTable())
            {
                var shipment = _dtShipment.NewShipmentPackageRow();
                IEnumerable<string> emailAddresses;

                // Null case
                emailAddresses = shipment.GetNotificationEmailAddresses();
                Assert.IsNull(emailAddresses);
            }
        }

        [TestMethod]
        public void GetNotificationEmailAddressesTrimTest()
        {
            using (var _dtShipment = new OrderShipmentDataSet.ShipmentPackageDataTable())
            {
                var shipment = _dtShipment.NewShipmentPackageRow();
                IEnumerable<string> emailAddresses;

                // Email is whitespace
                shipment.NotificationEmails = "  ";
                emailAddresses = shipment.GetNotificationEmailAddresses();
                Assert.AreEqual(0, emailAddresses.Count());

                // Email contains whitespace
                shipment.NotificationEmails = " test@example.com ";
                emailAddresses = shipment.GetNotificationEmailAddresses();
                Assert.AreEqual(1, emailAddresses.Count());
                Assert.IsTrue(emailAddresses.Contains("test@example.com"));

                // One email is empty
                shipment.NotificationEmails = "test@example.com,";
                emailAddresses = shipment.GetNotificationEmailAddresses();
                Assert.AreEqual(1, emailAddresses.Count());
                Assert.IsTrue(emailAddresses.Contains("test@example.com"));

                // Multiple empty emails
                shipment.NotificationEmails = ",test@example.com,,";
                emailAddresses = shipment.GetNotificationEmailAddresses();
                Assert.AreEqual(1, emailAddresses.Count());
                Assert.IsTrue(emailAddresses.Contains("test@example.com"));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNotificationEmailAddressesNullTest()
        {
            OrderShipmentDataSet.ShipmentPackageRow target = null;
            target.GetNotificationEmailAddresses();
        }
    }
}
