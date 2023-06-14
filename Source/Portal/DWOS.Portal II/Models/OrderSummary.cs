using System;
using System.Collections.Generic;

namespace DWOS.Portal.Models
{
    /// <summary>
    /// Contains a summary of order information.
    /// </summary>
    public class OrderSummary
    {
        public string PartName { get; set; }

        public string ManufacturerId { get; set; }

        public string CustomerName { get; set; }

        public int OrderId { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? RequiredDate { get; set; }

        public string Status { get; set; }

        public DateTime? CompletedDate { get; set; }

        public string Priority { get; set; }

        public int? PartQuantity { get; set; }

        public string CurrentLocation { get; set; }

        public string WorkStatus { get; set; }

        public string CurrentProcess { get; set; }

        public DateTime? CurrentProcessStartDate { get; set; }

        public DateTime? EstShipDate { get; set; }

        public string CustomerWorkOrder { get; set; }

        public string PurchaseOrder { get; set; }

        public int CustomerId { get; set; }

        public string TrackingNumber { get; set; }

        public string ShippingCarrier { get; set; }

        public int CertificationId { get; set; }

        public string CertificationUrl { get; set; }

        public List<string> SerialNumbers { get; set; }
    }
}