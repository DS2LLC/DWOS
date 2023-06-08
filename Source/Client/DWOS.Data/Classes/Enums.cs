namespace DWOS.Data
{
    /// <summary>
    /// Represents a login type
    /// </summary>
    public enum LoginType
    {
        /// <summary>
        /// PIN-only login
        /// </summary>
        Pin = 0,

        /// <summary>
        /// Smartcard-only login
        /// </summary>
        Smartcard = 1,

        /// <summary>
        /// Login type requiring PIN and smartcard.
        /// </summary>
        PinAndSmartcard = 2,

        /// <summary>
        /// Login type requiring either PIN or smartcard.
        /// </summary>
        PinOrSmartcard = 3
    }

    /// <summary>
    /// Represents a scheduling type.
    /// </summary>
    public enum SchedulerType
    {
        /// <summary>
        /// Process Lead Time by day
        /// </summary>
        ProcessLeadTime = 0,

        /// <summary>
        /// Production Capacity
        /// </summary>
        ProductionCapacity = 1,

        /// <summary>
        /// Manual scheduling by department
        /// </summary>
        Manual = 2,

        /// <summary>
        /// Manual scheduling for all departments
        /// </summary>
        ManualAllDepartments = 3,

        /// <summary>
        /// Process Lead Time by hour
        /// </summary>
        ProcessLeadTimeHour
    }

    /// <summary>
    /// Represents a type of lead time for use with lead time scheduling.
    /// </summary>
    public enum LeadTimeType
    {
        /// <summary>
        /// Per load.
        /// </summary>
        Load,

        /// <summary>
        /// Per piece.
        /// </summary>
        Piece
    }

    /// <summary>
    /// Represents a type of printer.
    /// </summary>
    public enum PrinterType
    {
        /// <summary>
        /// Document printer that prints pages
        /// </summary>
        Document = 0,

        /// <summary>
        /// Label printer
        /// </summary>
        Label = 1
    }

    /// <summary>
    /// Specifies type of scale to use.
    /// </summary>
    public enum ScaleType
    {
        /// <summary>
        /// None (no scale attached)
        /// </summary>
        None = 0,

        /// <summary>
        /// Sterling Scale
        /// </summary>
        Sterling = 1
    }

    /// <summary>
    /// Specifies the type of part marking printer to use.
    /// </summary>
    public enum PartMarkingDeviceType
    {
        VideoJetExcel,
        VideoJet1000Line
    }

    /// <summary>
    /// Represents an invoice level type.
    /// </summary>
    public enum InvoiceLevelType
    {
        /// <summary>
        /// Default - not valid as a top-level option
        /// </summary>
        Default,

        /// <summary>
        /// Sales Order, using <see cref="WorkOrder"/> as a fallback.
        /// </summary>
        SalesOrder,

        /// <summary>
        ///  Work Order
        /// </summary>
        WorkOrder,

        /// <summary>
        ///  Shipping Package
        /// </summary>
        Package
    }

    /// <summary>
    /// Represents an additional invoice item type.
    /// </summary>
    public enum InvoiceItemType
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// Work Order
        /// </summary>
        WO,

        /// <summary>
        /// Customer WO
        /// </summary>
        CustomerWO,

        /// <summary>
        /// Weight
        /// </summary>
        Weight,

        /// <summary>
        /// Tracking Number
        /// </summary>
        TrackingNumber,

        /// <summary>
        /// Purchase Order
        /// </summary>
        PO,

        /// <summary>
        ///  Packing Slip
        /// </summary>
        PackingSlip
    }

    /// <summary>
    /// Represents an invoice type.
    /// </summary>
    public enum InvoiceType
    {
        /// <summary>
        /// Comma-Separated Values
        /// </summary>
        CSV,

        /// <summary>
        /// QuickBooks
        /// </summary>
        Quickbooks,

        /// <summary>
        /// SYSPRO
        /// </summary>
        Syspro
    }

    /// <summary>
    /// Represents a type of scanner settings
    /// </summary>
    public enum ScannerSettingsType
    {
        /// <summary>
        /// Order (including receiving orders)
        /// </summary>
        Order,

        /// <summary>
        /// Part (including quotes)
        /// </summary>
        Part
    }

    /// <summary>
    /// Represents types of markup.
    /// </summary>
    public enum MarkupType
    {
        /// <summary>
        /// Fixed markup.
        /// </summary>
        Fixed,

        /// <summary>
        /// Percentage-based markup.
        /// </summary>
        Percentage
    }

    /// <summary>
    /// Represents an order pricing type
    /// </summary>
    public enum PricingType
    {
        /// <summary>
        /// Pricing Per-Part
        /// </summary>
        Part,

        /// <summary>
        /// Pricing Per-Process
        /// </summary>
        Process
    }

    /// <summary>
    /// Represents the invoicing type to use for QuickBooks\CSV invoicing.
    /// </summary>
    public enum InvoiceLineItemType
    {
        /// <summary>
        /// Invoice by part.
        /// </summary>
        /// <remarks>
        /// Invoices created using 'Part'should have a single line item for
        /// each order that represents all processing for that order.
        /// </remarks>
        Part,

        /// <summary>
        /// Invoice by department.
        /// </summary>
        /// <remarks>
        /// Invoices created using 'Department' should have a line item for
        /// each department that processed the part.
        /// Unless the company is using process-level pricing, there will not
        /// be enough information to invoice by department.
        /// </remarks>
        Department,

        /// <summary>
        /// Invoice by product class.
        /// </summary>
        /// <remarks>
        /// Invoices created using 'ProductClass' should have a line item for
        /// each product class of the Work Order.
        /// </remarks>
        ProductClass
    }

    /// <summary>
    /// Represents the field to use for calculating order prices.
    /// </summary>
    public enum PriceByType
    {
        Quantity,

        Weight
    }

    /// <summary>
    /// Represents the strategy to use when calculating order prices.
    /// </summary>
    public enum PricingStrategy
    {
        /// <summary>
        /// Each - multiply field determined by <see cref="PriceByType"/> by
        /// the base price.
        /// </summary>
        Each,

        /// <summary>
        /// Lot - just use the base price.
        /// </summary>
        Lot
    }

    /// <summary>
    /// Represents the status of an operator or process operator.
    /// </summary>
    public enum OperatorStatus
    {
        /// <summary>
        /// Operator is active - does not necessarily mean that they have an
        /// active timer.
        /// </summary>
        Active,

        /// <summary>
        /// Operator is inactive.
        /// </summary>
        Inactive
    }

    /// <summary>
    /// Represents an processing and inspection severity level.
    /// </summary>
    public enum ProcessStrictnessLevel
    {
        /// <summary>
        /// Strictest setting
        /// </summary>
        Strict,

        /// <summary>
        /// Normal setting
        /// </summary>
        Normal,

        /// <summary>
        /// Normal setting with auto-processing and auto-inspections.
        /// </summary>
        /// <remarks>
        /// This option may be removed in a future release.
        /// </remarks>
        AutoComplete
    }

    /// <summary>
    /// Represents a print setting for reports.
    /// </summary>
    public enum ReportPrintSetting
    {
        /// <summary>
        /// Do nothing
        /// </summary>
        Nothing,

        /// <summary>
        /// Print the report.
        /// </summary>
        Printer,

        /// <summary>
        /// Export the report to PDF.
        /// </summary>
        Pdf,

    }

    /// <summary>
    /// Represents a question's field.
    /// </summary>
    public enum QuestionField
    {
        /// <summary>
        /// Answer
        /// </summary>
        Answer,

        /// <summary>
        /// Minimum value
        /// </summary>
        MinValue,

        /// <summary>
        /// Maximum value
        /// </summary>
        MaxValue,

        /// <summary>
        /// Tolerance
        /// </summary>
        Tolerance,

        /// <summary>
        /// Answer out
        /// </summary>
        /// <remarks>
        /// Identifies the field that a process answer should be written to.
        /// </remarks>
        AnswerOut
    }

    /// <summary>
    /// Represents the status of a Syspro invoice.
    /// </summary>
    public enum SysproInvoiceStatus
    {
        Pending,
        Failed,
        Successful,
    }

    /// <summary>
    /// Represents an editor type for serial numbers.
    /// </summary>
    public enum SerialNumberType
    {
        /// <summary>
        /// One per order
        /// </summary>
        Basic,

        /// <summary>
        /// One or more per order.
        /// </summary>
        Advanced
    }

    /// <summary>
    /// Represents an editor type for product classes.
    /// </summary>
    public enum ProductClassType
    {
        /// <summary>
        /// Textbox - enter any product class.
        /// </summary>
        Textbox,

        /// <summary>
        /// Combobox/dropdown - select pre-defined product classes.
        /// </summary>
        Combobox
    }

    /// <summary>
    /// Represents a 'process confirmation type' for travelers.
    /// </summary>
    public enum TravelerProcessConfirmationType
    {
        /// <summary>
        ///  Shows nothing.
        /// </summary>
        None,

        /// <summary>
        /// Shows fields for quantity, date, and operator.
        /// </summary>
        QtyDateBy,

        /// <summary>
        ///  Shows only a checkbox indicating completion.
        /// </summary>
        CompletedCheckbox,

        /// <summary>
        /// Shows fields for time-in and time-out
        /// </summary>
        TimeInTimeOut
    }

    /// <summary>
    /// Represents the status for an OrderApproval record.
    /// </summary>
    public enum OrderApprovalStatus
    {
        /// <summary>
        /// The approval is pending customer input.
        /// </summary>
        Pending,

        /// <summary>
        ///  Customer has accepted the approval.
        /// </summary>
        Accepted,

        /// <summary>
        ///  Customer rejected the approval.
        /// </summary>
        Rejected
    }
}
