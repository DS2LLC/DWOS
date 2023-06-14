using System.Collections.Generic;

namespace DWOS.Services.Messages
{
    /// <summary>
    /// Server response containing an <see cref="ApplicationSettingsInfo"/>
    /// instance.
    /// </summary>
    public class ApplicationSettingsResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets settings for this instance.
        /// </summary>
        public ApplicationSettingsInfo ApplicationSettings { get; set; }
    }

    /// <summary>
    /// Represents DWOS application options.
    /// </summary>
    public class ApplicationSettingsInfo
    {
        #region Fields

        /// <summary>
        /// The most recent API version available when this was released.
        /// </summary>
        public const int CURRENT_API_VERSION = 2;

        private const int SERVER_PORT = 8081;
        private const string SERVER_URI_PATH = "/dwos";
        private const string LICENSE_SERVER_URI_PATH = "8082/LicenseService";
        private const string CONTROL_INSPECTION_ROLE = "ControlInspection";
        private const string ORDER_PROCESSING_ROLE = "OrderProcessing";
        private const string PART_CHECK_IN_ROLE = "PartCheckIn";
        private const string BATCH_ORDER_PROCESSING_ROLE = "BatchOrderProcessing";
        private const string ADD_ORDER_NOTE = "AddOrderNote";
        private const string EDIT_ORDER = "OrderEntry.Edit";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the server's product version.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this is 18.2.0.0 or newer, the mobile app should not check for
        /// an exact version match.
        /// </para>
        /// <para>
        /// This property is essential for clients being able to determine
        /// what the server's version is and should be renamed with care.
        /// </para>
        /// </remarks>
        public string ServerVersion { get; set; }

        /// <summary>
        /// Gets or sets the server's API version.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Version 18.2.0.0 or newer should check this property for an exact match.
        /// </para>
        /// <para>
        /// This property is essential for clients being able to determine
        /// what the server's version is and should be renamed with care.
        /// </para>
        /// </remarks>
        public int ServerApiVersion { get; set; }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the work status associated with orders
        /// that are in process.
        /// </summary>
        public string WorkStatusInProcess { get; set; }

        /// <summary>
        /// Gets or sets the work status associated with orders
        /// changing departments.
        /// </summary>
        public string WorkStatusChangingDepartment { get; set; }

        /// <summary>
        /// Gets or sets the work status associated with orders
        /// pending inspection.
        /// </summary>
        public string WorkStatusPendingInspection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if this system uses
        /// manual scheduling.
        /// </summary>
        /// <value>
        /// <c>true</c> if manual scheduling is being used; otherwise,
        /// <c>false</c>.
        /// </value>
        public bool UsingManualScheduling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if this system uses multiple lines.
        /// </summary>
        /// <value>
        /// <c>true</c> if multiple lines are being used; otherwise,
        /// <c>false</c>.
        /// </value>
        public bool UsingMultipleLines { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the system uses time tracking.
        /// </summary>
        /// <value>
        /// <c>true</c> if the system uses time tracking; otherwise, <c>false</c>.
        /// </value>
        public bool UsingTimeTracking { get; set; }

        /// <summary>
        /// Gets or sets the client update interval in seconds.
        /// </summary>
        public int ClientUpdateIntervalSeconds { get; set; }

        /// <summary>
        /// Gets or sets a list of departments.
        /// </summary>
        public List<string> Departments { get; set; }

        /// <summary>
        /// Gets or sets a list of processing lines.
        /// </summary>
        public List<string> ProcessingLines { get; set; }

        /// <summary>
        /// Gets the partial license server URI for this instance.
        /// </summary>
        public string LicenseServerUriPath => LICENSE_SERVER_URI_PATH;

        /// <summary>
        /// Gets the partial server URI for this instance.
        /// </summary>
        public string ServerUriPath => SERVER_URI_PATH;

        /// <summary>
        /// Gets the port for the API server.
        /// </summary>
        public int ServerPort => SERVER_PORT;

        /// <summary>
        /// Gets the role associated with control inspections.
        /// </summary>
        public string ControlInspectionRole => CONTROL_INSPECTION_ROLE;

        /// <summary>
        /// Gets the role associated with order processing.
        /// </summary>
        public string OrderProcessingRole => ORDER_PROCESSING_ROLE;

        /// <summary>
        /// Gets the role associated with check in.
        /// </summary>
        public string PartCheckInRole => PART_CHECK_IN_ROLE;

        /// <summary>
        /// Gets the role associated with batch processing.
        /// </summary>
        public string BatchOrderProcessingRole => BATCH_ORDER_PROCESSING_ROLE;

        /// <summary>
        /// Gets the role associated with adding order notes.
        /// </summary>
        public string OrderNoteAddRole => ADD_ORDER_NOTE;

        /// <summary>
        /// Gets the role associated with editing order information.
        /// </summary>
        public string EditOrderRole => EDIT_ORDER;

        #endregion
    }
}
