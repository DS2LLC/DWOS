using System;
using System.Collections.Generic;
using DWOS.Data.Datasets;

namespace DWOS.Portal.Models
{
    /// <summary>
    /// Represents a work order.
    /// </summary>
    public class Order
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

        public DateTime? EstShipDate { get; set; }

        public string CustomerWorkOrder { get; set; }

        public string PurchaseOrder { get; set; }

        public int CustomerId { get; set; }

        public string TrackingNumber { get; set; }

        public string ShippingCarrier { get; set; }

        public int CertificationId { get; set; }

        public string CertificationUrl { get; set; }

        public FileData Image { get; set; }

        public List<OrderProcess> Processes { get; set; }

        public OrderType OrderType { get; set; }

        public bool IsOnHold { get; set; }

        public string HoldReason { get; set; }

        public decimal? BasePrice { get; set; }

        public string PriceUnit { get; set; }

        public decimal? Weight { get; set; }

        public List<OrderFee> Fees { get; set; }

        public Address ShipToAddress { get; set; }

        public List<string> SerialNumbers { get; set; }
    }
}