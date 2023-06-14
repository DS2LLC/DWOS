namespace DWOS.Services.Messages
{
    // These request classes are not used by the backend server and can be replaced.

    /// <summary>
    /// Client request to find batches.
    /// </summary>
    public class FindBatchesRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the search text for this instance.
        /// </summary>
        public string SearchValue { get; set; }
    }

    /// <summary>
    /// Client request for detailed batch information.
    /// </summary>
    public class BatchDetailRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the batch ID to get detailed information for.
        /// </summary>
        public int BatchId { get; set; }
    }


    /// <summary>
    /// Client request for batch schedule.
    /// </summary>
    public class BatchScheduleRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the department for this instance.
        /// </summary>
        public string Department { get; set; }
    }

    /// <summary>
    /// Client request for a batch's processes.
    /// </summary>
    public class BatchProcessesRequest : RequestBase
    {
        public int BatchId { get; set; }
    }

    /// <summary>
    /// Client request for a batch's current process.
    /// </summary>
    public class BatchCurrentProcessRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the batch ID for this instance.
        /// </summary>
        public int BatchId { get; set; }
    }

    /// <summary>
    /// Client request for a specific document.
    /// </summary>
    public class DocumentRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the revision ID for this instance.
        /// </summary>
        /// <remarks>
        /// Revision ID identifies a specific revision of a document.
        /// </remarks>
        public int RevisionId { get; set; }
    }

    /// <summary>
    /// Client request for media.
    /// </summary>
    public class MediaDetailRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the media ID for this instance.
        /// </summary>
        public int MediaId { get; set; }
    }

    /// <summary>
    /// Client request for an order's inspection.
    /// </summary>
    public class InspectionByOrderRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the order ID for this instance.
        /// </summary>
        public int OrderId { get; set; }
    }


    /// <summary>
    /// Client request to find orders.
    /// </summary>
    public class FindOrdersRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets a value indicating if orders should include
        /// image data.
        /// </summary>
        /// <value>
        /// <c>true</c> if orders should include images;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool IncludeImage { get; set; }

        /// <summary>
        /// Gets or sets the length and width to use for thumbnails.
        /// </summary>
        public int ImageSize { get; set; }

        /// <summary>
        /// Gets or sets the search value for this instance.
        /// </summary>
        public string SearchValue { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="FindOrdersRequest"/>
        /// with default settings.
        /// </summary>
        public FindOrdersRequest()
        {
            IncludeImage = false;
            ImageSize = 64;
        }
    }


    /// <summary>
    /// Client request for order details.
    /// </summary>
    public class OrderDetailRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the order ID for this instance.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the length and width to use for thumbnails.
        /// </summary>
        /// <remarks>
        /// If <see cref="ImageSize"/> is below a minimum (as defined by the
        /// server), the minimum is used instead of <see cref="ImageSize"/>.
        /// </remarks>
        public int ImageSize { get; set; }
    }

    /// <summary>
    /// Client request for order schedule.
    /// </summary>
    public class OrderScheduleRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the department for this instance.
        /// </summary>
        public string Department { get; set; }
    }

    public class OrderNotesRequest : RequestBase
    {
        public int OrderId { get; set; }
    }

    /// <summary>
    /// Client request for an order's processes.
    /// </summary>
    public class OrderProcessesRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the order ID for this instance.
        /// </summary>
        public int OrderId { get; set; }
    }

    /// <summary>
    /// Client request for order process details.
    /// </summary>
    public class OrderProcessDetailRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the order process ID for this instance.
        /// </summary>
        public int OrderProcessId { get; set; }
    }

    /// <summary>
    /// Client request for an order's current process.
    /// </summary>
    public class OrderCurrentProcessRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the order ID for this instance.
        /// </summary>
        public int OrderId { get; set; }
    }

    /// <summary>
    /// Client request for an order's part details.
    /// </summary>
    public class PartDetailRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the order ID for this instance.
        /// </summary>
        public int OrderId { get; set; }
    }


    /// <summary>
    /// Client request for a process.
    /// </summary>
    public class ProcessRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the process ID for this instance.
        /// </summary>
        public int ProcessId { get; set; }
    }


    /// <summary>
    /// Client request for user information.
    /// </summary>
    public class UserRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the requested user ID for this instance.
        /// </summary>
        public int RequestedUserId { get; set; }
    }
}
