using DWOS.Data.Datasets;
using System;
using System.Linq;
using System.Collections.Generic;
using NLog.LayoutRenderers;

namespace DWOS.Data
{
    /// <summary>
    /// Defines extension methods for data-related classes.
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// Returns a collection of email addresses that are stored in
        /// <see cref="OrderShipmentDataSet.ShipmentPackageRow.NotificationEmails"/>.
        /// </summary>
        /// <param name="shipmentPackage">Shipment package to retrieve email addresses from.</param>
        /// <returns>
        /// A collection of strings if <see cref="OrderShipmentDataSet.ShipmentPackageRow.NotificationEmails"/>
        /// is not null; otherwise, returns null.
        /// </returns>
        /// <exception cref="ArgumentNullException">shipmentPackage is null</exception>
        public static IEnumerable<string> GetNotificationEmailAddresses(this OrderShipmentDataSet.ShipmentPackageRow shipmentPackage)
        {
            string[] addresses;
            if (shipmentPackage == null)
            {
                throw new ArgumentNullException("shipmentPackage", "shipmentPackage cannot be null.");            
            }
            else if (shipmentPackage.IsNotificationEmailsNull())

            {
                throw new ArgumentNullException("shipmentPackage", "shipmentPackage email address cannot be null.");
            }
            else
            {
                addresses = shipmentPackage.NotificationEmails.Split(',');
            }
            return addresses
                .Select(addr => addr.Trim())
                .Where(addr => !string.IsNullOrEmpty(addr));
        }
    }
}
