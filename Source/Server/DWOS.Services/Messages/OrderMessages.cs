using System;
using System.Collections.Generic;

namespace DWOS.Services.Messages
{
    #region Order Info

    /// <summary>
    /// Server response containing all orders.
    /// </summary>
    public class OrdersResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the orders for this instance.
        /// </summary>
        public List<OrderInfo> Orders { get; set; }
    }

    /// <summary>
    /// Represents order summary information.
    /// </summary>
    public class OrderInfo
    {
        /// <summary>
        /// Gets or sets the order ID for this instance.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the part name for this instance.
        /// </summary>
        public string PartName { get; set; }

        /// <summary>
        /// Gets or sets the current work status for this instance.
        /// </summary>
        public string WorkStatus { get; set; }
        /// <summary>
        /// Gets or sets the current location for this instance.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the current line for this instance.
        /// </summary>
        public string CurrentLine { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail data for this instance.
        /// </summary>
        /// <value>
        /// Thumbnail data if it is present; otherwise, <c>null</c>.
        /// </value>
        public byte[] Image { get; set; }

        /// <summary>
        /// Gets or sets the schedule priority for this instance.
        /// </summary>
        public int SchedulePriority { get; set; }
    }

    /// <summary>
    /// Client request to check in an order.
    /// </summary>
    public class CheckInRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the order ID for this instance.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the next department for this instance.
        /// </summary>
        public string NextDepartment { get; set; }
    }

    #endregion

    #region Order Detail

    /// <summary>
    /// Server response containing order information.
    /// </summary>
    public class OrderDetailResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the order for this instance.
        /// </summary>
        public OrderDetailInfo OrderDetail { get; set; }
    }

    /// <summary>
    /// Represents detailed order information.
    /// </summary>
    public class OrderDetailInfo
    {
        /// <summary>
        /// Gets or sets the order ID for this instance.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the part name for this instance.
        /// </summary>
        public string PartName { get; set; }

        /// <summary>
        /// Gets or sets the current work status for this instance.
        /// </summary>
        public string WorkStatus { get; set; }

        /// <summary>
        /// Gets or sets the current location for this instance.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the current line for this instance.
        /// </summary>
        public string CurrentLine { get; set; }

        /// <summary>
        /// Gets or sets the part image for this instance.
        /// </summary>
        /// <value>
        /// A thumbnail of a part's image if one is available;
        /// otherwise <c>null</c>.
        /// </value>
        public byte[] PartImage { get; set; }

        /// <summary>
        /// Gets or sets the order data for this instance.
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Gets or sets the estimated shipping date for this instance.
        /// </summary>
        public DateTime EstShipDate { get; set; }

        /// <summary>
        /// Gets or sets the customer's requested date for this instance.
        /// </summary>
        public DateTime ReqDate { get; set; }

        /// <summary>
        /// Gets or sets the customer name for this instance.
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or sets the customer work order number for this instance.
        /// </summary>
        public string CustomerWO { get; set; }

        /// <summary>
        /// Gets or sets the purchase order number for this instance.
        /// </summary>
        public string PO { get; set; }

        /// <summary>
        /// Gets or sets the priority for this instance.
        /// </summary>
        public string Priority { get; set; }

        /// <summary>
        /// Gets or sets the schedule priority for this instance.
        /// </summary>
        public int SchedulePriority { get; set; }

        /// <summary>
        /// Gets or sets the quantity for this instance.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the shipping method for this instance.
        /// </summary>
        public string ShippingMethod { get; set; }

        /// <summary>
        /// Gets or sets the order type for this instance.
        /// </summary>
        /// <value>
        /// Display string for the order's type.
        /// </value>
        public string OrderType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the order is on hold.
        /// </summary>
        /// <value>
        /// <c>true</c> if the order is on hold; otherwise, <c>false</c>.
        /// </value>
        public bool Hold { get; set; }

        /// <summary>
        /// Gets or sets the part ID for this instance.
        /// </summary>
        public int PartId { get; set; }

        public int OrderNoteCount { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if this instance represents
        /// an order that is currently in a batch.
        /// </summary>
        /// <value>
        /// <c>true</c> if the order is currently in a batch;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool IsInBatch { get; set; }

        /// <summary>
        /// Gets or sets part marking info for this instance.
        /// </summary>
        public PartMarkInfo PartMarkLines { get; set; }

        /// <summary>
        /// Gets or sets the documents for this instance.
        /// </summary>
        public List<DocumentInfo> Documents { get; set; }

        /// <summary>
        /// Gets or sets the media for this instance.
        /// </summary>
        public List<MediaSummary> Media { get; set; }
    }

    #endregion

    #region OrderSchedule

    /// <summary>
    /// Server response containing order schedule data.
    /// </summary>
    public class OrderScheduleResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the schedule for this instance.
        /// </summary>
        public OrderSchedule Schedule { get; set; }
    }

    /// <summary>
    ///  Represents a schedule of orders.
    /// </summary>
    public class OrderSchedule
    {
        /// <summary>
        /// Gets or sets the list of order IDs for this instance.
        /// </summary>
        public List<int> OrderIds { get; set; }
    }

    #endregion

    /// <summary>
    /// Represents part marking information for an order.
    /// </summary>
    public class PartMarkInfo
    {
        /// <summary>
        /// Gets or sets the first line.
        /// </summary>
        public string Line1 { get; set; }

        /// <summary>
        /// Gets or sets the second line.
        /// </summary>
        public string Line2 { get; set; }

        /// <summary>
        /// Gets or sets the third line.
        /// </summary>
        public string Line3 { get; set; }

        /// <summary>
        /// Gets or sets the fourth line.
        /// </summary>
        public string Line4 { get; set; }

        /// <summary>
        /// Gets an enumeration of all part mark lines.
        /// </summary>
        public IEnumerable<string> Lines => GetLines();

        private IEnumerable<string> GetLines()
        {
            var list = new List<string>();

            if (!string.IsNullOrEmpty(Line1))
            {
                list.Add(Line1);
            }

            if (!string.IsNullOrEmpty(Line2))
            {
                list.Add(Line2);
            }

            if (!string.IsNullOrEmpty(Line3))
            {
                list.Add(Line3);
            }

            if (!string.IsNullOrEmpty(Line4))
            {
                list.Add(Line4);
            }

            return list;
        }
    }
}
