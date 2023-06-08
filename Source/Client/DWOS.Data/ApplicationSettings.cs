using DWOS.Shared.Settings;
using DWOS.Shared.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DWOS.Data
{
    /// <summary>
    /// Manages application-wide settings.
    /// </summary>
    public class ApplicationSettings : SettingsBase
    {
        #region Fields

        private static readonly Lazy<ApplicationSettings> _lazyAppSettings = new Lazy<ApplicationSettings>(
            NewApplicationSettings);

        private static object _reloadLock = new object();

        private string _companyLogoEncoded;

        private const PricingType DEFAULT_PRICING_TYPE = PricingType.Part;

        private const InvoiceLineItemType DEFAULT_INVOICING_TYPE = InvoiceLineItemType.Part;

        private const ProcessStrictnessLevel DEFAULT_STRICTNESS_LEVEL = ProcessStrictnessLevel.Strict;
        private const string COMPANY_LOGO = "CompanyLogo.png";
        private const string QUALITY_SIGNATURE = "QualitySignature.png";
        private const string REPAIR_STATEMENT_LOGO = "RepairStatementLogo.png";
        private const string ACCREDITATION_LOGO = "AccreditationLogo.png";

        #endregion

        #region Properties

        /// <summary>
        /// Gets the default instance of the application settings class.
        /// </summary>
        public static ApplicationSettings Current
        {
            get
            {
                return _lazyAppSettings.Value;
            }
        }

        /// <summary>
        /// Gets the path provider for this instance.
        /// </summary>
        public IPathProvider PathProvider
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value that indicates if the server is a demo server.
        /// </summary>
        /// <remarks>
        /// You may have to manually set this setting in the database to
        /// <c>true</c> if you want to enable it.
        /// </remarks>
        /// <value>
        /// <c>true</c> if this is the demo server; otherwise, <c>false</c>.
        /// </value>
        public bool IsDemoServer
        {
            get => GetSettingValue<bool>("IsDemoServer", null, () => false.ToString());
            set => SetSettingValue("IsDemoServer", value.ToString(), value);
        }

        /// <summary>
        /// The current cache version, appended to files saved to the local file system. Used to help force files to be re-cached between application revisions.
        /// </summary>
        public int CacheVersion
        {
            get
            {
                return GetSettingValue<int>("CacheVersion", null, () => 1.ToString());
            }
            set { SetSettingValue("CacheVersion", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the warranty text to show on COCs.
        /// </summary>
        public string COCWarranty
        {
            get { return GetSettingValue<string>("COCWarranty"); }
            set { SetSettingValue("COCWarranty", value); }
        }

        /// <summary>
        /// Gets or sets the default name to use for the COC signature.
        /// </summary>
        public string COCSignatureName
        {
            get { return GetSettingValue<string>("COCSignatureName"); }
            set { SetSettingValue("COCSignatureName", value); }
        }

        /// <summary>
        /// Gets or sets the default title to use for the COC signature.
        /// </summary>
        public string COCSignatureTitle
        {
            get { return GetSettingValue<string>("COCSignatureTitle"); }
            set { SetSettingValue("COCSignatureTitle", value); }
        }

        /// <summary>
        /// Gets or sets the default image to use for the COC signature.
        /// </summary>
        public string COCSignatureImagePath
        {
            get
            {
                return GetSettingValue<string>("COCSignature", (encodedImage) => SaveEncodedImageToFile(encodedImage, QUALITY_SIGNATURE), () => GetEncodedImageFromFile(QUALITY_SIGNATURE));
            }
            set
            {
                string imgEncoded = "";
                if(value != null)
                    imgEncoded = Convert.ToBase64String(File.ReadAllBytes(value));
                SetSettingValue("COCSignature", imgEncoded, value);
            }
        }

        /// <summary>
        /// Gets or sets the default print setting to use when creating a COC.
        /// </summary>
        public ReportPrintSetting DefaultCOCPrintSetting
        {
            get
            {
                return GetSettingValue("COCPrint",
                    (value) =>
                    {
                        ReportPrintSetting returnValue = ReportPrintSetting.Pdf;

                        if (!Enum.TryParse(value, out returnValue))
                        {
                            returnValue = ReportPrintSetting.Pdf;
                        }

                        return returnValue;
                    },

                    () => ReportPrintSetting.Pdf.ToString());
            }
            set
            {
                SetSettingValue("COCPrint", value.ToString(), value);
            }
        }

        /// <summary>
        /// Gets or sets the logo to use for statements of repair.
        /// </summary>
        public string ShippingRepairsStatementLogo
        {
            get
            {
                return GetSettingValue<string>("RepairStatementLogo",
                    (encodedImage) => SaveEncodedImageToFile(encodedImage, REPAIR_STATEMENT_LOGO),
                    null);
            }
            set
            {
                var imgEncoded = Convert.ToBase64String(File.ReadAllBytes(value));
                SetSettingValue("RepairStatementLogo", imgEncoded, value);
            }
        }

        /// <summary>
        /// Gets or sets the first line of the company's address.
        /// </summary>
        public string CompanyAddress1
        {
            get { return GetSettingValue<string>("Company Address1"); }
            set { SetSettingValue("Company Address1", value); }
        }

        /// <summary>
        /// Gets or sets the city part of the company's address.
        /// </summary>
        public string CompanyCity
        {
            get { return GetSettingValue<string>("Company City"); }
            set { SetSettingValue("Company City", value); }
        }

        /// <summary>
        /// Gets or sets the company's fax number.
        /// </summary>
        public string CompanyFax
        {
            get { return GetSettingValue<string>("Company Fax"); }
            set { SetSettingValue("Company Fax", value); }
        }

        /// <summary>
        /// Gets or sets the file path to the company's logo.
        /// </summary>
        public string CompanyLogoImagePath
        {
            get
            {
                return GetSettingValue<string>("CompanyLogo", (imgEncoded) => SaveEncodedImageToFile(imgEncoded, COMPANY_LOGO), () => GetEncodedImageFromFile(COMPANY_LOGO));
            }
            set
            {
                var imgEncoded = Convert.ToBase64String(File.ReadAllBytes(value));
                SetSettingValue("CompanyLogo", imgEncoded, value);
            }
        }

        /// <summary>
        /// Gets the company logo image as the raw encoded 64 value. Used by website.
        /// </summary>
        public string CompanyLogoImageEncoded64
        {
            get
            {
                if(_companyLogoEncoded == null)
                {
                    var path = CompanyLogoImagePath; //Call to force image to be pulled from DB

                    var setting = base.FindSettingValue("CompanyLogo");

                    if(setting != null)
                        _companyLogoEncoded = setting.Value;
                }

                return _companyLogoEncoded;
            }
        }

        /// <summary>
        /// Gets or sets the path to the accreditation logo.
        /// </summary>
        public string AccreditationLogoImagePath
        {
            get
            {
                return GetSettingValue<string>("AccreditationLogo", (imgEncoded) => SaveEncodedImageToFile(imgEncoded, ACCREDITATION_LOGO), () => GetEncodedImageFromFile(ACCREDITATION_LOGO));
            }
            set
            {
                var imgEncoded = Convert.ToBase64String(File.ReadAllBytes(value));
                SetSettingValue("AccreditationLogo", imgEncoded, value);
            }
        }

        /// <summary>
        /// Gets or sets the company's name.
        /// </summary>
        public string CompanyName
        {
            get { return GetSettingValue<string>("Company Name"); }
            set { SetSettingValue("Company Name", value); }
        }

        /// <summary>
        /// Gets or sets the company's phone number.
        /// </summary>
        public string CompanyPhone
        {
            get { return GetSettingValue<string>("Company Phone"); }
            set { SetSettingValue("Company Phone", value); }
        }

        /// <summary>
        /// Gets or sets the state part of the company's address.
        /// </summary>
        public string CompanyState
        {
            get { return GetSettingValue<string>("Company State"); }
            set { SetSettingValue("Company State", value); }
        }

        /// <summary>
        /// Gets or sets the zip part of the company's address.
        /// </summary>
        public string CompanyZip
        {
            get { return GetSettingValue<string>("Company Zip"); }
            set { SetSettingValue("Company Zip", value); }
        }

        /// <summary>
        /// Gets or sets the ID for the company's country.
        /// </summary>
        public int CompanyCountry
        {
            get => GetSettingValue<int>("Company Country", null, () => 0.ToString());
            set => SetSettingValue("Company Country", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets the URL to the company's website.
        /// </summary>
        public string CompanyUrl
        {
            get { return GetSettingValue<string>("CompanyUrl"); }
            set { SetSettingValue("CompanyUrl", value); }
        }

        /// <summary>
        /// Gets or sets the URL to the company's portal website.
        /// </summary>
        public string PortalUrl
        {
            get { return GetSettingValue<string>("PortalUrl"); }
            set { SetSettingValue("PortalUrl", value); }
        }

        /// <summary>
        /// Gets or sets the prt to use for sending emails.
        /// </summary>
        public string EmailPort
        {
            get { return GetSettingValue<string>("EmailPort"); }
            set { SetSettingValue("EmailPort", value); }
        }

        /// <summary>
        /// Gets or sets the user name to use for sending emails.
        /// </summary>
        /// <remarks>
        /// This is different from the sender's email address. This is a
        /// server login.
        /// </remarks>
        public string EmailUserName
        {
            get { return GetSettingValue<string>("EmailUserName"); }
            set { SetSettingValue("EmailUserName", value); }
        }

        /// <summary>
        /// Gets or sets the password to use for sending emails.
        /// </summary>
        public string EmailPassword
        {
            get { return GetSettingValue<string>("EmailPassword"); }
            set { SetSettingValue("EmailPassword", value); }
        }

        /// <summary>
        /// Gets or sets the address to send automated emails from.
        /// </summary>
        public string EmailFromAddress
        {
            get { return GetSettingValue<string>("EmailFromAddress"); }
            set { SetSettingValue("EmailFromAddress", value); }
        }

        /// <summary>
        /// Gets or sets the SMTP server to use for sending emails.
        /// </summary>
        public string EmailSMTPServer
        {
            get { return GetSettingValue<string>("EmailSMTPServer"); }
            set { SetSettingValue("EmailSMTPServer", value); }
        }

        /// <summary>
        /// Gets or sets the authentication method to use for sending emails.
        /// </summary>
        /// <remarks>
        /// This is an nsoftware.IPWorks.HtmlmailerAuthMechanisms value.
        /// </remarks>
        public string EmailAuthentication
        {
            get { return GetSettingValue<string>("EmailAuthentication"); }
            set { SetSettingValue("EmailAuthentication", value); }
        }

        /// <summary>
        /// Gets or sets the SSL start mode to use when sending emails.
        /// </summary>
        /// <remarks>
        /// This is an nsoftware.IPWorks.HtmlmailerSSLStartModes value.
        /// </remarks>
        public string EmailSslStartMode
        {
            get => GetSettingValue<string>("EmailSslStartMode", null, () => "sslAutomatic");
            set => SetSettingValue("EmailSslStartMode", value);
        }

        /// <summary>
        /// Gets or sets the shipping rollover time to use when calculating an
        /// order's the shipment date for shipment notifications.
        /// </summary>
        public TimeSpan ShippingRolloverTime
        {
            get
            {
                return GetSettingValue<TimeSpan>("ShipppingRolloverTime", (shipTime) =>
                                                                          {
                                                                              int hour = 16;
                                                                              int min = 0;
                                                                              int sec = 0;

                                                                              if(shipTime != null)
                                                                              {
                                                                                  var times = shipTime.Split(':');

                                                                                  if(times.Length >= 1)
                                                                                      int.TryParse(times[0], out hour);
                                                                                  if(times.Length >= 2)
                                                                                      int.TryParse(times[1], out min);
                                                                                  if(times.Length >= 3)
                                                                                      int.TryParse(times[2], out sec);
                                                                              }

                                                                              return new TimeSpan(hour, min, sec);
                                                                          }, () => "16:00");
            }
            set
            {
                SetSettingValue("ShipppingRolloverTime", value.ToString(@"hh\:mm"), value);
            }
        }


        /// <summary>
        /// Gets or sets a value that indicates if DWOS requires all orders in
        /// a shipment package to have the same product class.
        /// </summary>
        /// <remarks>
        /// If a company uses product class to mark the facility that a work
        /// order is in, enabling this option adds a safeguard to shipping
        /// that helps guarantee that orders from different facilities
        /// do not go in the same shipment.
        /// </remarks>
        /// <value>
        /// <c>true</c> to require the same product class for all orders;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool RequireSingleProductClassForShipments
        {
            get => GetSettingValue<bool>("RequireSingleProductClassForShipments", null, () => false.ToString());
            set => SetSettingValue("RequireSingleProductClassForShipments", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets the receiving rollover time.
        /// </summary>
        /// <remarks>
        /// The time used to determine when receiving an order at such a late
        /// time that it must be processed the next day. Used to represent an
        /// hour, minute, and second in a date.
        /// </remarks>
        public TimeSpan ReceivingRolloverTime
        {
            get
            {
                return GetSettingValue<TimeSpan>("ReceivingRolloverTime", ParseTimeSpan, () => "8:00");
            }
            set
            {
                SetSettingValue("ReceivingRolloverTime", value.ToString(@"hh\:mm"), value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if automatic schedule reset
        /// occurs for manual scheduling.
        /// </summary>
        /// <value>
        /// <c>true</c> if automatic schedule reset is enabled;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool ScheduleResetEnabled
        {
            get { return GetSettingValue<bool>("ScheduleResetEnabled", null, () => true.ToString()); }
            set {  SetSettingValue("ScheduleResetEnabled", value.ToString(), value);}
        }

        /// <summary>
        /// Gets or sets a value indicating if automatic schedule reset
        /// occurs for New Orders.
        /// </summary>
        /// <value>
        /// <c>true</c> if Apply Default Fees is enabled;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool ApplyDefaultFeesEnabled
        {
            get { return GetSettingValue<bool>("ApplyDefaultFees", null, () => true.ToString()); }
            set { SetSettingValue("ApplyDefaultFees", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the time at which the server should reset manual scheduling.
        /// </summary>
        public TimeSpan ScheduleResetTime
        {
            get
            {
                return GetSettingValue("ScheduleResetTime", ParseTimeSpan, () => "0:00");
            }
            set
            {
                SetSettingValue("ScheduleResetTime", value.ToString(@"hh\:mm"), value);
            }
        }


        /// <summary>
        /// Gets or sets a setting to disable sending automated emails from
        /// the server.
        /// </summary>
        public bool DisableNotifications
        {
            get { return GetSettingValue<bool>("DisableNotifications", null, () => false.ToString()); }
            set { SetSettingValue("DisableNotifications", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the type of login to use.
        /// </summary>
        /// <value>
        /// The type of the login.
        /// </value>
        public LoginType LoginType
        {
            get
            {
                return GetSettingValue<LoginType>("LoginType", (loginType) =>
                                                               {
                                                                   var lt = Data.LoginType.Pin;

                                                                   if (!Enum.TryParse(loginType, out lt))
                                                                       lt = Data.LoginType.Pin;

                                                                   return lt;
                                                               },
                                                               () => LoginType.Pin.ToString());
            }
            set
            {
                SetSettingValue("LoginType", value.ToString(), value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if scheduling is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if scheduling is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool SchedulingEnabled
        {
            get { return GetSettingValue<bool>("SchedulingEnabled", null, () => false.ToString()); }
            set { SetSettingValue("SchedulingEnabled", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the type of scheduling to use.
        /// </summary>
        public SchedulerType SchedulerType
        {
            get
            {
                return GetSettingValue<SchedulerType>("SchedulerType", (value) =>
                {
                    var lt = SchedulerType.ProductionCapacity;

                    if (!Enum.TryParse(value, out lt))
                        lt = SchedulerType.ProductionCapacity;

                    return lt;
                },
                () => SchedulerType.ProductionCapacity.ToString());
            }
            set
            {
                SetSettingValue("SchedulerType", value.ToString(), value);
            }
        }

        /// <summary>
        /// Gets a value that indicates if manual scheduling is in-use.
        /// </summary>
        /// <value>
        /// <c>true</c> if scheduling is enabled and the scheduler type is a
        /// manual one; otherwise, <c>false</c>.
        /// </value>
        public bool UsingManualScheduling => SchedulingEnabled
            && (SchedulerType == SchedulerType.Manual || SchedulerType == SchedulerType.ManualAllDepartments);

        /// <summary>
        /// Gets a value that indicates if lead time scheduling is in-use.
        /// </summary>
        /// <value>
        /// <c>true</c> if scheduling is enabled and the scheduler type is
        /// a lead time one; otherwise, <c>false</c>.
        /// </value>
        public bool UsingLeadTimeScheduling => SchedulingEnabled
            && (SchedulerType == SchedulerType.ProcessLeadTime || SchedulerType == SchedulerType.ProcessLeadTimeHour);

        /// <summary>
        /// Gets or sets the default printer type to use for printing.
        /// </summary>
        public PrinterType DefaultPrinterType
        {
            get
            {
                return GetSettingValue<PrinterType>("DefaultPrinterType", (value) =>
                {
                    var lt = PrinterType.Document;

                    if (!Enum.TryParse(value, out lt))
                        lt = PrinterType.Document;

                    return lt;
                },
                () => PrinterType.Document.ToString());
            }
            set
            {
                SetSettingValue("DefaultPrinterType", value.ToString(), value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the part marking workflow is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if park marking is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool PartMarkingEnabled
        {
            get { return GetSettingValue<bool>("PartMarkingEnabled", null, () => true.ToString()); }
            set { SetSettingValue("PartMarkingEnabled", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if the COC workflow is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if COC is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool COCEnabled
        {
            get { return GetSettingValue<bool>("COCEnabled", null, () => true.ToString()); }
            set { SetSettingValue("COCEnabled", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or skips a value indicating if the COC workflow can be
        /// skipped on an individual order.
        /// </summary>
        /// <value>
        /// <c>true</c> if users can skip the COC workflow for a work order;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool AllowSkippingCoc
        {
            get => GetSettingValue<bool>("COCSkipEnabled", null, () => false.ToString());
            set => SetSettingValue("COCSkipEnabled", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value indicating if the time tracking workflow is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if time tracking is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool TimeTrackingEnabled
        {
            get { return GetSettingValue<bool>("TimeTrackingEnabled", null, () => false.ToString()); }
            set { SetSettingValue("TimeTrackingEnabled", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if the multiple lines workflow is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if multiple lines are enabled; otherwise, <c>false</c>.
        /// </value>
        public bool MultipleLinesEnabled
        {
            get { return GetSettingValue<bool>("MultipleLinesEnabled", null, () => false.ToString()); }
            set { SetSettingValue("MultipleLinesEnabled", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if the order review workflow is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if order review is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool OrderReviewEnabled
        {
            get { return GetSettingValue<bool>("OrderReviewEnabled", null, () => true.ToString()); }
            set { SetSettingValue("OrderReviewEnabled", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if the order check-in workflow
        /// is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if orders need to be checked into a department prior
        /// to processing; otherwise, <c>false</c>.
        /// </value>
        public bool OrderCheckInEnabled
        {
            get { return GetSettingValue<bool>("OrderCheckInEnabled", null, () => true.ToString()); }
            set { SetSettingValue("OrderCheckInEnabled", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if the statement of repairs
        /// workflow is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if statement of repairs is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool RepairStatementEnabled
        {
            get { return GetSettingValue<bool>("RepairStatementEnabled", null, () => false.ToString()); }
            set { SetSettingValue("RepairStatementEnabled", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if the bill of lading
        /// workflow is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if bill of lading is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool BillOfLadingEnabled
        {
            get => GetSettingValue<bool>("BillOfLadingEnabled", null, () => false.ToString());
            set => SetSettingValue("BillOfLadingEnabled", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value indicating if the order approval workflow
        /// is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if order approval is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool OrderApprovalEnabled
        {
            get => GetSettingValue<bool>("OrderApprovalEnabled", null, () => false.ToString());
            set => SetSettingValue("OrderApprovalEnabled", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets the number of days to wait for a contact to
        /// accept/reject an order approval before sending a reminder to the
        /// current department and/or product class.
        /// </summary>
        public int OrderApprovalReminderDays
        {
            get => GetSettingValue<int>("OrderApprovalNotificationDays", null, () => 1.ToString());
            set => SetSettingValue("OrderApprovalNotificationDays", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value indicating if the import/export approval
        /// workflow is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if the workflow is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool ImportExportApprovalEnabled
        {
            get => GetSettingValue<bool>("ImportExportApprovalEnabled", null, () => false.ToString());
            set => SetSettingValue("ImportExportApprovalEnabled", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value indicating if the bill of lading should
        /// include containers.
        /// </summary>
        /// <value>
        /// <c>true</c> if the bill of lading should list containers;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool BillOfLadingIncludeContainers
        {
            get => GetSettingValue<bool>("BillOfLadingIncludeContainers", null, () => true.ToString());
            set => SetSettingValue("BillOfLadingIncludeContainers", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets the default lead time to use for orders.
        /// </summary>
        /// <remarks>
        /// 1 = 1 day
        /// </remarks>
        public int OrderLeadTime
        {
            get { return GetSettingValue<int>("OrderLeadTime", null, () => 10.ToString()); }
            set { SetSettingValue("OrderLeadTime", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the name of the shipping department.
        /// </summary>
        /// <remarks>
        /// Used internally to identify the shipping department without doing a
        /// lookup.
        /// </remarks>
        public string DepartmentShipping
        {
            get { return GetSettingValue<string>("DepartmentShipping", null, () => "Shipping"); }
            set { SetSettingValue("DepartmentShipping", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the name of the part marking department.
        /// </summary>
        /// <remarks>
        /// Used internally to identify the part marking department without doing a
        /// lookup.
        /// </remarks>
        public string DepartmentPartMarking
        {
            get { return GetSettingValue<string>("DepartmentPartMarking", null, () => "PartMarking"); }
            set { SetSettingValue("DepartmentPartMarking", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the name of the quality assurance department.
        /// </summary>
        /// <remarks>
        /// Used internally to identify the quality assurance department
        /// without doing a lookup.
        /// </remarks>
        public string DepartmentQA
        {
            get { return GetSettingValue<string>("DepartmentQA", null, () => "QA"); }
            set { SetSettingValue("DepartmentQA", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the name of the sales department.
        /// </summary>
        /// <remarks>
        /// Used internally to identify the sales department without doing a
        /// lookup.
        /// </remarks>
        public string DepartmentSales
        {
            get { return GetSettingValue<string>("DepartmentSales", null, () => "Sales"); }
            set { SetSettingValue("DepartmentSales", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the name of the outside processing department.
        /// </summary>
        /// <remarks>
        /// Used internally to identify the outside processing department
        /// without doing a lookup.
        /// </remarks>
        public string DepartmentOutsideProcessing
        {
            get { return GetSettingValue<string>("DepartmentOutsideProcessing", null, () => "Outside Processing"); }
            set { SetSettingValue("DepartmentOutsideProcessing", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the company key.
        /// </summary>
        /// <remarks>
        /// Used by the error reporter to identify the customer. This is not
        /// the authoritative source for the customer key, the server holds
        /// the key.
        /// </remarks>
        public string CompanyKey
        {
            get { return GetSettingValue<string>("CompanyKey"); }
            set { SetSettingValue("CompanyKey", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the email to CC portal authorization emails to.
        /// </summary>
        public string PortalAuthorizationEmailCC
        {
            get { return GetSettingValue<string>("PortalAuthorizationEmailCC"); }
            set { SetSettingValue("PortalAuthorizationEmailCC", value.ToString(), value); }
        }

        /// <summary>
        /// The default id of the Document Administrator Security Group
        /// </summary>
        /// <remarks>
        /// This group is a special one because it controls permissions in
        /// document manager. The default value here is 20, but it should be
        /// unused because the 16.1.2 update script sets it.
        /// </remarks>
        public int DocumentAdministratorSecurityGroupId
        {
            get { return GetSettingValue<int>("DocumentAdministratorSecurityGroupId", null, () => 20.ToString()); }
            set { SetSettingValue("DocumentAdministratorSecurityGroupId", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates if users can review orders
        /// they create.
        /// </summary>
        /// <value>
        /// <c>true</c> if users can review their own orders; otherwise, <c>false</c>
        /// </value>
        public bool AllowReviewYourOwnOrders
        {
            get { return GetSettingValue<bool>("AllowReviewYourOwnOrders", null, () => false.ToString()); }
            set { SetSettingValue("AllowReviewYourOwnOrders", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the minimum length of a PIN.
        /// </summary>
        public int UserPinMinLength
        {
            get { return GetSettingValue<int>("UserPinMinLength", null, () => 4.ToString()); }
            set { SetSettingValue("UserPinMinLength", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the tagline for the company.
        /// </summary>
        public string CompanyTagline
        {
            get { return GetSettingValue<string>("CompanyTagline"); }
            set { SetSettingValue("CompanyTagline", value); }
        }

        /// <summary>
        /// Gets or sets the name of the portal's visual style.
        /// </summary>
        public string SkinStyle
        {
            get { return GetSettingValue<string>("SkinStyle"); }
            set { SetSettingValue("SkinStyle", value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the Portal site should show
        /// an option for late order notifications.
        /// </summary>
        /// <value>
        /// <c>true</c> to show an option for Late Order Notifications;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool ShowLateOrderNotificationSetting
        {
            get => GetSettingValue<bool>("ShowLateOrderNotificationSetting", null, () => false.ToString());
            set => SetSettingValue("ShowLateOrderNotificationSetting", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value to multiply priority values by during
        /// scheduling.
        /// </summary>
        public int ScheduleOrderPriorityMultiplier
        {
            get { return GetSettingValue<int>("ScheduleOrderPriorityMultiplier", null, () => 2.ToString()); }
            set { SetSettingValue("ScheduleOrderPriorityMultiplier", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value to multiply counts by during scheduling.
        /// </summary>
        public int ScheduleProcessCountMultiplier
        {
            get { return GetSettingValue<int>("ScheduleProcessCountMultiplier", null, () => 3.ToString()); }
            set { SetSettingValue("ScheduleProcessCountMultiplier", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value to multiply days late counts by during scheduling.
        /// </summary>
        public int ScheduleDaysLateCountMultiplier
        {
            get { return GetSettingValue<int>("ScheduleDaysLateCountMultiplier", null, () => 10.ToString()); }
            set { SetSettingValue("ScheduleDaysLateCountMultiplier", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a setting indicating if processes go on COCs by
        /// default.
        /// </summary>
        /// <value>
        /// <c>true</c> if processes go on the COC by default; otherwise, <c>false</c>
        /// </value>
        public bool DisplayProcessCOCByDefault
        {
            get { return GetSettingValue<bool>("DisplayProcessCOCByDefault", null, () => true.ToString()); }
            set { SetSettingValue("DisplayProcessCOCByDefault", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if customer process aliases should
        /// go on the COC.
        /// </summary>
        /// <value>
        /// <c>true</c> if the COC should use customer process aliases when
        /// available. Otherwise, <c>false</c>.
        /// </value>
        public bool DisplayCustomerProcessAliasOnCoc
        {
            get => GetSettingValue<bool>("DisplayCustomerProcessAliasOnCoc", null, () => false.ToString());
            set => SetSettingValue("DisplayCustomerProcessAliasOnCoc", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value indicating if process dates goes on work
        /// order travelers.
        /// </summary>
        /// <value>
        /// <c>true</c> if processes go on the COC by default; otherwise, <c>false</c>
        /// </value>
        public bool IncludeProcessDateOnTraveler
        {
            get { return GetSettingValue<bool>("IncludeProcessDateOnTraveler", null, () => true.ToString()); }
            set { SetSettingValue("IncludeProcessDateOnTraveler", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the process confirmation type to use on
        /// Work Order Travelers.
        /// </summary>
        public TravelerProcessConfirmationType TravelerProcessConfirmation
        {
            get
            {
                return GetSettingValue("TravelerProcessConfirmation",
                    value =>
                    {
                        TravelerProcessConfirmationType returnVal;

                        if (!Enum.TryParse(value, out returnVal))
                        {
                            returnVal = TravelerProcessConfirmationType.QtyDateBy;
                        }

                        return returnVal;
                    },

                    () => TravelerProcessConfirmationType.QtyDateBy.ToString());
            }

            set {  SetSettingValue("TravelerProcessConfirmation", value.ToString(), value);}
        }

        /// <summary>
        /// Gets or sets the default per-process lead time.
        /// </summary>
        /// <remarks>
        /// 1 = 1 day
        /// </remarks>
        public int DefaultProcessLeadTime
        {
            get { return GetSettingValue<int>("DefaultProcessLeadTime", null, () => 1.ToString()); }
            set { SetSettingValue("DefaultProcessLeadTime", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the shipping lead time.
        /// </summary>
        /// <remarks>
        /// 1d = 1 day
        /// </remarks>
        public double ShippingLeadTime
        {
            get { return GetSettingValue<double>("ShippingLeadTime", null, () => 1.ToString()); }
            set { SetSettingValue("ShippingLeadTime", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the COC lead time.
        /// </summary>
        /// <remarks>
        /// 1d = 1 day
        /// </remarks>
        public double COCLeadTime
        {
            get { return GetSettingValue<double>("COCLeadTime", null, () => .25.ToString()); }
            set { SetSettingValue("COCLeadTime", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the COC lead time.
        /// </summary>
        /// <remarks>
        /// 1d = 1 day
        /// </remarks>
        public double PartMarkingLeadTime
        {
            get { return GetSettingValue<double>("PartMarkingLeadTime", null, () => .25.ToString()); }
            set { SetSettingValue("PartMarkingLeadTime", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the default number of days before a quote expires.
        /// </summary>
        public int QuoteExpirationDays
        {
            get { return GetSettingValue<int>("QuoteExpirationDays", null, () => 30.ToString()); }
            set { SetSettingValue("QuoteExpirationDays", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the minimum price for orders.
        /// </summary>
        public double MinimumOrderPrice
        {
            get { return GetSettingValue<double>("MinimumOrderPrice", null, () => 0.ToString()); }
            set { SetSettingValue("MinimumOrderPrice", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the minimum process price.
        /// </summary>
        /// <remarks>
        /// This seems to only be used for a settings screen.
        /// </remarks>
        public double MinimumProcessPrice
        {
            get { return GetSettingValue<double>("MinimumProcessPrice", null, () => 0.ToString()); }
            set { SetSettingValue("MinimumProcessPrice", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a setting indicating if the system should allow
        /// partial process loads.
        /// </summary>
        /// <value>
        /// <c>true</c> to allow partial process loads; otherwise, <c>false</c>
        /// </value>
        public bool AllowPartialProcessLoads
        {
            get { return GetSettingValue<bool>("AllowPartialProcessLoads", null, () => false.ToString()); }
            set { SetSettingValue("AllowPartialProcessLoads", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a setting that indicates invoice export method.
        /// </summary>
        public InvoiceType InvoiceExportType
        {
            get
            {
                return GetSettingValue<InvoiceType>("InvoiceExportType",
                    (value) =>
                    {
                        InvoiceType returnValue = InvoiceType.CSV;

                        if (!Enum.TryParse(value, out returnValue))
                        {
                            returnValue = InvoiceType.CSV;
                        }

                        return returnValue;
                    },

                    () => InvoiceType.CSV.ToString());
            }
            set
            {
                SetSettingValue("InvoiceExportType", value.ToString(), value);
            }
        }

        /// <summary>
        /// Gets or sets a setting indicating if SYSPRO integration has been
        /// enabled on the server.
        /// </summary>
        /// <remarks>
        /// Unlike other invoicing types, SYSPRO integration requires
        /// server-side setup.
        /// </remarks>
        /// <value>
        /// <c>true</c> if SYSPRO integration is enabled;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool SysproIntegrationEnabled
        {
            get { return GetSettingValue<bool>("SysproIntegration", null, () => false.ToString()); }
            set { SetSettingValue("SysproIntegration", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets settings specific to SYSPRO invoicing.
        /// </summary>
        public SysproInvoiceSettings SysproInvoiceSettings
        {
            get { return GetSettingValue("SysproInvoice", JsonConvert.DeserializeObject<SysproInvoiceSettings>, null); }
            set { SetSettingValue("SysproInvoice", JsonConvert.SerializeObject(value), value); }
        }

        /// <summary>
        /// Gets or sets the automatic invoicing interval to show in
        /// invoice dialogs.
        /// </summary>
        /// <remarks>
        /// This value should be used in the client to inform users that
        /// automatic invoicing is enabled. The server may use other values
        /// (such as those in <see cref="ServerSettings"/>) as the actual
        /// update interval.
        /// </remarks>
        public int? InvoiceIntervalMinutes
        {
            get { return GetSettingValue<int?>("InvoiceInterval", null, () => null); }
            set {  SetSettingValue("InvoiceInterval", value?.ToString(), value);}
        }

        /// <summary>
        /// Gets or sets the default level to use for invoicing.
        /// </summary>
        public InvoiceLevelType InvoiceLevel
        {
            get
            {
                return GetSettingValue("InvoiceLevel",
                    (value) =>
                    {
                        InvoiceLevelType returnValue = InvoiceLevelType.WorkOrder;

                        if (!Enum.TryParse(value, out returnValue))
                        {
                            returnValue = InvoiceLevelType.WorkOrder;
                        }

                        return returnValue;
                    },

                    () => InvoiceLevelType.WorkOrder.ToString());
            }
            set
            {
                SetSettingValue("InvoiceLevel", value.ToString(), value);
            }
        }

        /// <summary>
        /// Gets or sets the value to use for QuickBooks's "other 1" field.
        /// </summary>
        public InvoiceItemType InvoiceItem1
        {
            get
            {
                return GetSettingValue("InvoiceItem1",
                    (value) =>
                    {
                        InvoiceItemType returnValue = InvoiceItemType.None;

                        if (!Enum.TryParse(value, out returnValue))
                        {
                            returnValue = InvoiceItemType.None;
                        }

                        return returnValue;
                    },

                    () => InvoiceItemType.None.ToString());
            }
            set
            {
                SetSettingValue("InvoiceItem1", value.ToString(), value);
            }
        }

        /// <summary>
        /// Gets or sets the value to use for QuickBooks's "other 2" field.
        /// </summary>
        public InvoiceItemType InvoiceItem2
        {
            get
            {
                return GetSettingValue("InvoiceItem2",
                    (value) =>
                    {
                        InvoiceItemType returnValue = InvoiceItemType.None;

                        if (!Enum.TryParse(value, out returnValue))
                        {
                            returnValue = InvoiceItemType.None;
                        }

                        return returnValue;
                    },

                    () => InvoiceItemType.None.ToString());
            }
            set
            {
                SetSettingValue("InvoiceItem2", value.ToString(), value);
            }
        }

        /// <summary>
        /// Gets or sets the CSV export format.
        /// </summary>
        public string InvoiceExportTokens
        {
            get { return GetSettingValue<string>("InvoiceExportTokens"); }
            set { SetSettingValue("InvoiceExportTokens", value); }
        }

        /// <summary>
        /// Gets or sets the name of a part invoice item.
        /// </summary>
        public string InvoicePartItemName
        {
            get { return GetSettingValue<string>("InvoicePartItemName"); }
            set { SetSettingValue("InvoicePartItemName", value); }
        }

        /// <summary>
        /// Gets or sets the sales account to use for part invoice items.
        /// </summary>
        /// <remarks>
        /// Only used by CSV invoicing.
        /// </remarks>
        public string InvoicePartItemCode
        {
            get { return GetSettingValue<string>("InvoicePartItemCode"); }
            set { SetSettingValue("InvoicePartItemCode", value); }
        }

        /// <summary>
        /// Gets or sets the connection string to use for QuickBooks.
        /// </summary>
        public string QBConnectionString
        {
            get { return GetSettingValue<string>("QBConnectionString"); }
            set { SetSettingValue("QBConnectionString", value); }
        }

        /// <summary>
        /// Gets or sets the class name to use for QuickBooks invoices.
        /// </summary>
        public string QBClass
        {
            get { return GetSettingValue<string>("QBClass"); }
            set { SetSettingValue("QBClass", value); }
        }

        /// <summary>
        /// Gets or sets the custom field to use for Customer WO in QuickBooks.
        /// </summary>
        public string InvoiceCustomerWOField
        {
            get { return GetSettingValue<string>("InvoiceCustomerWOField"); }
            set { SetSettingValue("InvoiceCustomerWOField", value); }
        }

        /// <summary>
        /// Gets or sets the custom field to use for shipment tracking number
        /// in QuickBooks.
        /// </summary>
        public string InvoiceTrackingNumberField
        {
            get { return GetSettingValue<string>("InvoiceTrackingNumberField"); }
            set { SetSettingValue("InvoiceTrackingNumberField", value); }
        }

        /// <summary>
        /// Gets or sets the prefix to use for automatically generated invoice
        /// numbers generated using <see cref="InvoiceLevelType.WorkOrder"/>.
        /// </summary>
        public string InvoiceWorkOrderPrefix
        {
            get { return GetSettingValue<string>("InvoiceWorkOrderPrefix"); }
            set { SetSettingValue("InvoiceWorkOrderPrefix", value); }
        }

        /// <summary>
        /// Gets or sets the prefix to use for automatically generated invoice
        /// numbers generated using <see cref="InvoiceLevelType.SalesOrder"/>.
        /// </summary>
        public string InvoiceSalesOrderPrefix
        {
            get
            {
                string salesOrderPrefix = GetSettingValue<string>("InvoiceSalesOrderPrefix");

                if (string.IsNullOrWhiteSpace(salesOrderPrefix))
                {
                    return InvoiceWorkOrderPrefix;
                }
                else
                {
                    return salesOrderPrefix;
                }
            }
            set
            {
                SetSettingValue("InvoiceSalesOrderPrefix", value);
            }
        }

        /// <summary>
        /// Gets or sets the prefix to use for automatically generated invoice
        /// numbers generated using <see cref="InvoiceLevelType.Package"/>.
        /// </summary>
        public string InvoicePackagePrefix
        {
            get
            {
                return GetSettingValue<string>("InvoicePackagePrefix");
            }
            set
            {
                SetSettingValue("InvoicePackagePrefix", value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum allowed number of errors when
        /// exporting invoices.
        /// </summary>
        public int InvoiceExportMaxErrors
        {
            get { return GetSettingValue<int>("InvoiceExportMaxErrors", null, () => 100.ToString()); }
            set { SetSettingValue("InvoiceExportMaxErrors", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the maximum number of invoices that can be exported at 1 time.
        /// </summary>
        /// <remarks>
        /// Quickbooks is slow...
        /// </remarks>
        /// <value>The invoice maximum batch export.</value>
        public int InvoiceMaxBatchExport
        {
            get { return GetSettingValue<int>("InvoiceMaxBatchExport", v => String.IsNullOrWhiteSpace(v) || Convert.ToInt32(v) < 1 ? 100 : Convert.ToInt32(v), () => "100"); }
            set
            {
                if(value < 1)
                    value = 100;

                SetSettingValue("InvoiceMaxBatchExport", value.ToString(), value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if invoicing should check total
        /// price instead of base price when determining what orders to include.
        /// </summary>
        /// <returns>
        /// <c>true</c> if invoicing should check total price;
        /// <c>false</c> if invoicing should check base price.
        /// </returns>
        public bool InvoiceCheckTotal
        {
            get => GetSettingValue<bool>("InvoiceCheckTotal", null, () => false.ToString());
            set => SetSettingValue("InvoiceCheckTotal", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value indicating if a batch can have multiple processes in it.
        /// </summary>
        /// <value><c>true</c> if batches can have multiple processes; otherwise, <c>false</c>.</value>
        public bool BatchMultipleProcesses
        {
            get { return GetSettingValue<bool>("BatchMultipleProcesses", null, () => true.ToString()); }
            set { SetSettingValue("BatchMultipleProcesses", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if DWOS should show a confirmation
        /// prompt when deleting a batch.
        /// </summary>
        /// <value>
        /// <c>true</c> if users should see a confirmation prompt when
        /// deleting a batch; otherwise, <c>false</c>.
        /// </value>
        public bool ShowBatchDeletePrompt
        {
            get => GetSettingValue<bool>("ShowBatchDeletePrompt", null, () => true.ToString());
            set => SetSettingValue("ShowBatchDeletePrompt", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets the work status indicating that an order is in process.
        /// </summary>
        public string WorkStatusInProcess
        {
            get { return GetSettingValue<string>("WorkStatusInProcess", null, () => "In Process", true); }
            set { SetSettingValue("WorkStatusInProcess", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the work status indicating that an order is changing departments.
        /// </summary>
        public string WorkStatusChangingDepartment
        {
            get { return GetSettingValue<string>("WorkStatusChangingDepartment", null, () => "Changing Departments", true); }
            set { SetSettingValue("WorkStatusChangingDepartment", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the work status indicating that an order is completed.
        /// </summary>
        public string WorkStatusCompleted
        {
            get { return GetSettingValue<string>("WorkStatusCompleted", null, () => "Completed", true); }
            set { SetSettingValue("WorkStatusCompleted", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the work status indicating that an order is ready for COC.
        /// </summary>
        public string WorkStatusFinalInspection
        {
            get { return GetSettingValue<string>("WorkStatusFinalInspection", null, () => "Final Inspection", true); }
            set { SetSettingValue("WorkStatusFinalInspection", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the work status indicating that an order is on hold.
        /// </summary>
        public string WorkStatusHold
        {
            get { return GetSettingValue<string>("WorkStatusHold", null, () => "Hold", true); }
            set { SetSettingValue("WorkStatusHold", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the work status indicating that an order is ready for part marking.
        /// </summary>
        public string WorkStatusPartMarking
        {
            get { return GetSettingValue<string>("WorkStatusPartMarking", null, () => "Part Marking", true); }
            set { SetSettingValue("WorkStatusPartMarking", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the work status indicating that an order is pending a join.
        /// </summary>
        public string WorkStatusPendingJoin
        {
            get { return GetSettingValue<string>("WorkStatusPendingJoin", null, () => "Pending Join", true); }
            set { SetSettingValue("WorkStatusPendingJoin", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the work status indicating that an order is pending order review.
        /// </summary>
        public string WorkStatusPendingOR
        {
            get { return GetSettingValue<string>("WorkStatusPendingOR", null, () => "Pending Order Review", true); }
            set { SetSettingValue("WorkStatusPendingOR", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the work status indicating that an order is pending Import/Export review.
        /// </summary>
        public string WorkStatusPendingImportExportReview
        {
            get => GetSettingValue<string>("WorkStatusPendingImportExportReview", null, () => "Pending Import/Export Review", true);
            set => SetSettingValue("WorkStatusPendingImportExportReview", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets the work status indicating that an order is pending quality inspection.
        /// </summary>
        public string WorkStatusPendingQI
        {
            get { return GetSettingValue<string>("WorkStatusPendingQI", null, () => "Pending Inspection", true); }
            set { SetSettingValue("WorkStatusPendingQI", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the work status indicating that an order is pending rework planning.
        /// </summary>
        public string WorkStatusPendingReworkPlanning
        {
            get { return GetSettingValue<string>("WorkStatusPendingReworkPlanning", null, () => "Pending Rework Planning", true); }
            set { SetSettingValue("WorkStatusPendingReworkPlanning", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the work status indicating that an order is reading for shipping.
        /// </summary>
        public string WorkStatusShipping
        {
            get { return GetSettingValue<string>("WorkStatusShipping", null, () => "Shipping", true); }
            set { SetSettingValue("WorkStatusShipping", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the work status indicating that an order is pending rework assessment.
        /// </summary>
        public string WorkStatusPendingReworkAssessment
        {
            get { return GetSettingValue<string>("WorkStatusPendingReworkAssessment", null, () => "Pending Rework Assessment", true); }
            set { SetSettingValue("WorkStatusPendingReworkAssessment", value.ToString(), value); }
        }

        /// <summary>
        /// Gets a collection of work statuses involved in processing.
        /// </summary>
        public ICollection<string> ProcessingWorkStatuses =>
            new List<string>
            {
                WorkStatusInProcess,
                WorkStatusPendingQI
            };

        /// <summary>
        /// Gets or sets a value indicating if unit price should be
        /// calculated for CSV export.
        /// </summary>
        /// <value>
        /// <c>true</c> if unit prices should be calculated; otherwise, <c>false</c>
        /// </value>
        public bool InvoiceCalcUnitPrice
        {
            get { return GetSettingValue<bool>("InvoiceCalcUnitPrice", null, () => true.ToString()); }
            set { SetSettingValue("InvoiceCalcUnitPrice", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if CSV documents created by
        /// invoicing should have a header.
        /// </summary>
        /// <value>
        /// <c>true</c> to include a header; otherwise, <c>false</c>
        /// </value>
        public bool InvoiceHeaderRow
        {
            get { return GetSettingValue<bool>("InvoiceHeaderRow", null, () => false.ToString()); }
            set { SetSettingValue("InvoiceHeaderRow", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the number of decimal places to use for price values.
        /// </summary>
        public int PriceDecimalPlaces
        {
            get { return GetSettingValue<int>("PriceDecimalPlaces", null, () => 2.ToString()); }
            set { SetSettingValue("PriceDecimalPlaces", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the number of decimal places to use for weight values.
        /// </summary>
        public int WeightDecimalPlaces
        {
            get { return GetSettingValue<int>("WeightDecimalPlaces", null, () => 2.ToString()); }
            set { SetSettingValue("WeightDecimalPlaces", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the number of decimal places to use for decimal
        /// values during processing.
        /// </summary>
        public int ProcessingDecimalPlaces
        {
            get { return GetSettingValue<int>("ProcessDecimalPlaces", null, () => 4.ToString()); }
            set { SetSettingValue("ProcessDecimalPlaces", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if price units should be enforced.
        /// </summary>
        /// <remarks>
        /// This setting affects quotes and orders different. Please see
        /// individual uses of this property.
        /// </remarks>
        /// <value>
        /// <c>true</c> if price units should be enforced; otherwise, <c>false</c>.
        /// </value>
        public bool UsePriceUnitQuantities
        {
            get { return GetSettingValue<bool>("UsePriceUnitQuantities", null, () => true.ToString()); }
            set { SetSettingValue("UsePriceUnitQuantities", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if load capacity is in-use.
        /// </summary>
        /// <value>
        /// <c>true</c> if load capacity is in-use; otherwise, <c>false</c>.
        /// </value>
        public bool UseLoadCapacity
        {
            get { return GetSettingValue<bool>("UseLoadCapacity", null, () => true.ToString()); }
            set { SetSettingValue("UseLoadCapacity", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the default fees. Used to quickly add the default fees to an order or quote.
        /// </summary>
        /// <value>The default fees.</value>
        public string DefaultFees
        {
            get { return GetSettingValue<string>("DefaultFees", null, () => ""); }
            set { SetSettingValue("DefaultFees", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the type of pricing to use for parts.
        /// </summary>
        public PricingType PartPricingType
        {
            get
            {
                return GetSettingValue<PricingType>("PartPricingType",
                    (input) =>
                    {
                        PricingType result;
                        if (Enum.TryParse<PricingType>(input, out result))
                        {
                            return result;
                        }
                        else
                        {
                            return DEFAULT_PRICING_TYPE;
                        }
                    },
                    () => DEFAULT_PRICING_TYPE.ToString());
            }
            set { SetSettingValue("PartPricingType", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if process prices should be enforced.
        /// </summary>
        /// <value>
        /// <c>true</c> if process prices should be enforced; otherwise, <c>false</c>.
        /// </value>
        public bool ProcessPriceWarningEnabled
        {
            get
            {
                return GetSettingValue<bool>("ProcessPriceWarningEnabled", null, () => true.ToString());
            }
            set
            {
                SetSettingValue("ProcessPriceWarningEnabled", value.ToString(), value);
            }

        }

        /// <summary>
        /// Gets or sets a value indicating the current line item type
        /// for invoices.
        /// </summary>
        public InvoiceLineItemType InvoiceLineItemType
        {
            get
            {
                return GetSettingValue("InvoicingLineItemType",
                    (input) =>
                    {
                        InvoiceLineItemType result;
                        if (Enum.TryParse(input, out result))
                        {
                            return result;
                        }
                        else
                        {
                            return DEFAULT_INVOICING_TYPE;
                        }
                    },
                    () => DEFAULT_INVOICING_TYPE.ToString());
            }

            set { SetSettingValue("InvoicingLineItemType", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the midpoint rounding type
        /// for QuickBooks invoicing.
        /// </summary>
        public MidpointRounding QuickBooksInvoiceMidpointRounding
        {
            get => GetSettingValue("InvoiceMidpointRounding", input => Enum.TryParse(input, out MidpointRounding result)
                ? result
                : MidpointRounding.ToEven);

            set => SetSettingValue("InvoiceMidpointRounding", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value indicating if volume discounts are allowed.
        /// </summary>
        /// <remarks>
        /// Volume discounts allow users to give different prices for
        /// different quantity ranges on a per-part basis.
        /// </remarks>
        /// <value>
        /// <c>true</c> if volume discounts are allowed; otherwise, false.
        /// </value>
        public bool EnableVolumePricing
        {
            get
            {
                return GetSettingValue<bool>("VolumePricing", null, () => false.ToString());
            }
            set
            {
                SetSettingValue("VolumePricing", value.ToString(), value);
            }
        }

        /// <summary>
        /// Gets or sets the path to a default image to use in reports.
        /// </summary>
        public string ReportPlaceholderImagePath
        {
            get
            {
                return GetSettingValue<string>("ReportPlaceholder",
                    (encodedImage) => SaveEncodedImageToFile(encodedImage, "ReportPlaceholder.png"),
                    null);
            }
            set
            {
                var imgEncoded = Convert.ToBase64String(File.ReadAllBytes(value));
                SetSettingValue("ReportPlaceholder", imgEncoded, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if reports should use
        /// <see cref="ReportPlaceholderImagePath"/> when there is no other
        /// image to use.
        /// </summary>
        /// <value>
        /// <c>true</c> to use the placeholder image; <c>false</c> to
        /// show nothing.
        /// </value>
        public bool UseReportPlaceholderImage
        {
            get
            {
                return GetSettingValue<bool>("UseReportPlaceholder", null, () => true.ToString());
            }
            set
            {
                SetSettingValue("UseReportPlaceholder", value.ToString(), value);
            }
        }

        /// <summary>
        /// Gets or sets the strictness level of control inspections.
        /// </summary>
        public ProcessStrictnessLevel ProcessStrictnessLevel
        {
            get
            {
                return GetSettingValue("ControlInspectionLevel",
                    (input) =>
                    {
                        ProcessStrictnessLevel result;
                        if (Enum.TryParse(input, out result))
                        {
                            if (result == ProcessStrictnessLevel.AutoComplete)
                            {
                                _log.Error("Company is using deprecated 'AutoComplete' option.");
                            }

                            return result;
                        }
                        else
                        {
                            return DEFAULT_STRICTNESS_LEVEL;
                        }
                    },
                    () => DEFAULT_STRICTNESS_LEVEL.ToString());
            }

            set { SetSettingValue("ControlInspectionLevel", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if the packing slip should include
        /// the company logo.
        /// </summary>
        /// <value>
        /// <c>true</c> if the packing slip should include the company logo;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool ShowCompanyLogoOnPackingSlip
        {
            get
            {
                return GetSettingValue<bool>("CompanyLogoPackingSlip", null, () => false.ToString());
            }
            set
            {
                SetSettingValue("CompanyLogoPackingSlip", value.ToString(), value);
            }
        }

        public bool ShowCODOnTraveler
        {
            get
            {
                return GetSettingValue<bool>("ShowCODOnTraveler", null, () => false.ToString());
            }
            set
            {
                SetSettingValue("ShowCODOnTraveler", value.ToString(), value);
            }
        }

        /// <summary>
        /// Gets or sets the format to use when listing work orders, sales
        /// orders, blanket POs, and quotes.
        /// </summary>
        public string OrderItemFormat
        {
            get
            {
                return GetSettingValue<string>("OrderEntry_ItemFormat", null, () => "%ID% %REQUIREDDATE%");
            }
            set
            {
                SetSettingValue("OrderEntry_ItemFormat", value, value);
            }
        }

        /// <summary>
        /// Gets or sets the serial number editor type.
        /// </summary>
        public SerialNumberType SerialNumberEditorType
        {
            get
            {
                const SerialNumberType defaultEditorType = SerialNumberType.Basic;

                return GetSettingValue("SerialNumberEditorType",
                    (input) =>
                    {
                        SerialNumberType result;
                        if (Enum.TryParse(input, out result))
                        {
                            return result;
                        }

                        return defaultEditorType;
                    },
                    () => defaultEditorType.ToString());
            }

            set { SetSettingValue("SerialNumberEditorType", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the product class editor type.
        /// </summary>
        public ProductClassType ProductClassEditorType
        {
            get
            {
                const ProductClassType defaultEditorType = ProductClassType.Textbox;

                return GetSettingValue("ProductClassEditorType",
                    (input) =>
                    {
                        ProductClassType result;
                        if (Enum.TryParse(input, out result))
                        {
                            return result;
                        }

                        return defaultEditorType;
                    },
                    () => defaultEditorType.ToString());
            }

            set => SetSettingValue("ProductClassEditorType", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value indicating if processes are required on parts.
        /// </summary>
        /// <value>
        /// <c>true</c> if [part process required]; otherwise, <c>false</c>.
        /// </value>
        public bool PartProcessRequired
        {
            get { return GetSettingValue<bool>("PartProcessRequired", null, () => true.ToString()); }
            set { SetSettingValue("PartProcessRequired", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets the client update interval in seconds.
        /// </summary>
        /// <value>
        /// Seconds between WIP screen refreshes in client.
        /// Seconds between order/batch refreshes in mobile.
        /// </value>
        public int ClientUpdateIntervalSeconds
        {
            get
            {
                const int defaultUpdateInterval = 180; // 3 minutes
                return GetSettingValue<int>("ClientUpdateInterval", null, () => defaultUpdateInterval.ToString());
            }
            set { SetSettingValue("ClientUpdateInterval", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if multiple features in DWOS will
        /// use additional customers for contacts.
        /// </summary>
        /// <value>
        /// <c>true</c> if users can add additional customers for contacts;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool AllowAdditionalCustomersForContacts
        {
            get { return GetSettingValue<bool>("UseCustomerRelationships", null, () => false.ToString()); }
            set { SetSettingValue("UseCustomerRelationships", value.ToString(), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if DWOS saves a history of Work
        /// Order printouts to Order History.
        /// </summary>
        /// <value>
        /// <c>true</c> to save print history for an order;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool SaveWorkOrderPrintHistory
        {
            get => GetSettingValue<bool>("SaveWorkOrderPrintHistory", null, () => false.ToString());
            set => SetSettingValue("SaveWorkOrderPrintHistory", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value indicating if process suggestions are enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if process suggestions are enabled;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool UseProcessSuggestions
        {
            get => GetSettingValue<bool>("UseProcessSuggestions", null, () => false.ToString());
            set => SetSettingValue("UseProcessSuggestions", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets the amount of time (in milliseconds) to wait before
        /// canceling incomplete barcode scans.
        /// </summary>
        public int BarcodeIntervalMilliseconds
        {
            get => GetSettingValue("BarcodeIntervalMilliseconds", str =>
            {
                // Ensure that value cannot be below 1 second
                var dbValue = Convert.ToInt32(str);
                return Math.Max(dbValue, 1000);
            }, () => 1000.ToString(), true);

            set => SetSettingValue("BarcodeIntervalMilliseconds", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value that indicates if Work Order Summaries need to
        /// include summaries for rejoined orders.
        /// </summary>
        /// <value>
        /// <c>true</c> to include summaries for rejoined orders;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool PrintSummariesForRejoinedOrders
        {
            get => GetSettingValue<bool>("PrintSummariesForRejoinedOrders", null, () => false.ToString());
            set => SetSettingValue("PrintSummariesForRejoinedOrders", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value that indicates if quantity and weigh
        /// should be synced for orders.
        /// </summary>
        /// <value>
        /// <c>true</c> if sync is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool SyncQuantityAndWeightForOrders
        {
            get => GetSettingValue<bool>("SyncQuantityAndWeightForOrders", null, () => true.ToString());
            set => SetSettingValue("SyncQuantityAndWeightForOrders", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets the contract review text.
        /// </summary>
        public string ContractReviewText
        {
            get => GetSettingValue<string>("ContractReviewText", null, () => ApplicationSettingsResources.ContractReview);
            set => SetSettingValue("ContractReviewText", value, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates if any user can access the Sales
        /// Order Wizard.
        /// </summary>
        /// <value>
        /// <c>true</c> if the Sales Order Wizard is enabled;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool SalesOrderWizardEnabled
        {
            get => GetSettingValue<bool>("SalesOrderWizardEnabled", null, () => false.ToString());
            set => SetSettingValue("SalesOrderWizardEnabled", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value that indicates if users can create parts
        /// through the Sales Order Wizard.
        /// </summary>
        /// <remarks>
        /// A.K.A. Redline for Sales Order Wizard. Option can be disabled if
        /// users are entering bad parts through the wizard.
        /// </remarks>
        /// <value>
        /// <c>true</c> if redline is enabled for the Sales Order Wizard;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool CanCreatePartsInSalesOrderWizard
        {
            get => GetSettingValue<bool>("SalesOrderWizardRedline", null, () => true.ToString());
            set => SetSettingValue("SalesOrderWizardRedline", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value that indicates if users can add Work Orders
        /// with different sets of processes to the same Sales Order through
        /// the wizard.
        /// </summary>
        /// <value>
        /// <c>true</c> if different processes are allowed;
        /// otherwise, <c>false</c>
        /// </value>
        public bool AllowDifferentProcessesInSalesOrderWizard
        {
            get => GetSettingValue<bool>("SalesOrderWizardAllowDifferentProcesses", null, () => false.ToString());
            set => SetSettingValue("SalesOrderWizardAllowDifferentProcesses", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value that indicates if the WIP screen should
        /// include on-hold orders when listing/counting late orders.
        /// </summary>
        /// <value>
        /// <c>true</c> if the WIP screen ignores 'on hold' status when
        /// checking to see if an order is late; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeHoldsInLateOrders
        {
            get => GetSettingValue<bool>("IncludeHoldsInLateOrders", null, () => true.ToString());
            set => SetSettingValue("IncludeHoldsInLateOrders", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value that indicates if DWOS needs to automatically
        /// create batches for Sales Orders.
        /// </summary>
        /// <value>
        /// <c>true</c> if DWOS automatically creates batches for sales orders;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool AutomaticallyBatchSalesOrder
        {
            get => GetSettingValue<bool>("AutoBatchSalesOrder", null, () => false.ToString());
            set => SetSettingValue("AutoBatchSalesOrder", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value that indicates if batches should continue
        /// after processing into part marking and final inspection (but
        /// not shipping).
        /// </summary>
        /// <value>
        /// <c>true</c> for post-processing of batches; otherwise, <c>false</c>.
        /// </value>
        public bool ContinueBatchAfterProcessing
        {
            get => GetSettingValue<bool>("ContinueBatchAfterProcessing", null, () => false.ToString());
            set => SetSettingValue("ContinueBatchAfterProcessing", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value that indicates if processes should be auto-imported in 
        /// order entry.
        /// </summary>
        public bool AutoImportProcessesToOrder
        {
            get => GetSettingValue<bool>("AutoImportProcessPackages", null, () => false.ToString());
            set => SetSettingValue("AutoImportProcessPackages", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value that indicates if the Receiving department can
        /// add new parts to the system.
        /// </summary>
        public bool ReceivingCanAddParts
        {
            get => GetSettingValue<bool>("ReceivingCanAddParts", null, () => false.ToString());
            set => SetSettingValue("ReceivingCanAddParts", value.ToString(), value);
        }

        /// <summary>
        /// Gets or sets a value that indicates if sales orders should be 
        /// indexed during invoicing. I.E. SO-1 would return SO-1 the first time it
        /// was exported. After that it would invoice as SO-1-1, SO-1-2, and so on.
        /// </summary>
        public bool IndexSOInvoices
        {
            get => GetSettingValue<bool>("IndexSOInvoices", null, () => false.ToString());
            set => SetSettingValue("IndexSOInvoices", value.ToString(), value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ApplicationSettings"/> class.
        /// </summary>
        /// <param name="persistence"></param>
        /// <param name="pathProvider"></param>
        public ApplicationSettings(ISettingsPersistence persistence, IPathProvider pathProvider)
            : base(persistence)
        {
            if (pathProvider == null)
            {
                throw new ArgumentNullException(nameof(pathProvider));
            }

            PathProvider = pathProvider;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ApplicationSettings"/> class using a default
        /// <see cref="ISettingsPersistence"/> and <see cref="IPathProvider"/>.
        /// </summary>
        /// <returns></returns>
        public static ApplicationSettings NewApplicationSettings()
        {
            return new ApplicationSettings(new SettingsPersistence()
            {
                TableName = "ApplicationSettings",
                SettingNameColumn = "SettingName",
                SettingValueColumn = "Value"
            }, new PathProvider());
        }

        /// <summary>
        /// Updates the database connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        public void UpdateDatabaseConnection(string connectionString)
        {
            Persistence.ConnectionString = connectionString;
        }

        /// <summary>
        /// Pre-loads the <see cref="CompanyLogoImagePath"/> setting.
        /// </summary>
        public void PreLoadSettings()
        {
            //load the company logo from file system if it is there
            var companyLogoPath = GetImageFilePath(COMPANY_LOGO);

            if (File.Exists(companyLogoPath))
            {
                CompanyLogoImagePath = companyLogoPath;
            }

            // Preload and check work statuses for errors
            // Check helps diagnose any possible issues where these are null.
            // See VSTS #16949.
            var workStatuses = new List<string>
            {
                WorkStatusPendingOR,
                WorkStatusPendingImportExportReview,
                WorkStatusChangingDepartment,
                WorkStatusInProcess,
                WorkStatusPendingQI,
                WorkStatusPendingReworkPlanning,
                WorkStatusPendingJoin,
                WorkStatusPartMarking,
                WorkStatusFinalInspection,
                WorkStatusShipping,
                WorkStatusCompleted,
                WorkStatusHold
            };

            if (workStatuses.Any(string.IsNullOrEmpty))
            {
                _log.Warn("One or more work statuses are invalid.");
            }
        }

        /// <summary>
        /// Clears any cached images
        /// </summary>
        public void ClearImageCache()
        {
            var cachedImages = new List<string>
            {
                GetImageFilePath(COMPANY_LOGO),
                GetImageFilePath(QUALITY_SIGNATURE),
                GetImageFilePath(REPAIR_STATEMENT_LOGO),
                GetImageFilePath(ACCREDITATION_LOGO)
            };

            foreach (var imgPath in cachedImages)
            {
                if (!string.IsNullOrEmpty(imgPath) && File.Exists(imgPath))
                {
                    File.Delete(imgPath);
                }
            }
        }

        /// <summary>
        /// Reloads the settings from the database.
        /// </summary>
        public void ReloadSettings()
        {
            try
            {
                lock (_reloadLock)
                {
                    _log.Info("Begin RE-loading application settings.");

                    base.ClearCachedSettings();
                    PreLoadSettings();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error reloading application settings.");
            }
        }

        private string SaveEncodedImageToFile(string imgEncoded, string fileName)
        {
            try
            {
                if (imgEncoded != null && imgEncoded.Length > 8)
                {
                    var path = GetImageFilePath(fileName);

                    if (!File.Exists(path))
                    {
                        var bmp = FileSystem.StringToImage(imgEncoded);
                        bmp.Save(path);
                    }

                    return path;
                }

                return String.Empty;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error saving file to {1} from encoded string '{0}'.".FormatWith(imgEncoded.TrimToMaxLength(50, "..."), fileName));
                return null;
            }
        }

        private string GetEncodedImageFromFile(string fileName)
        {
            try
            {
                var path = GetImageFilePath(fileName);

                if (File.Exists(path))
                    return FileSystem.ImageToString(System.Drawing.Bitmap.FromFile(path), System.Drawing.Imaging.ImageFormat.Png);

                return null;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error getting image from file '{0}'.".FormatWith(fileName));
                return null;
            }
        }

        private string GetImageFilePath(string fileName)
        {
            string directory = PathProvider.ImageDirectory;

            //Append cache version to it
            fileName = Path.GetFileNameWithoutExtension(fileName) + "_v" + this.CacheVersion.ToString() + Path.GetExtension(fileName);

            return Path.Combine(directory, fileName);
        }

        private static TimeSpan ParseTimeSpan(string spanString)
        {
            int hour = 16;
            int min = 0;
            int sec = 0;

            if (spanString != null)
            {
                var times = spanString.Split(':');

                if (times.Length >= 1)
                    int.TryParse(times[0], out hour);
                if (times.Length >= 2)
                    int.TryParse(times[1], out min);
                if (times.Length >= 3)
                    int.TryParse(times[2], out sec);
            }

            return new TimeSpan(hour, min, sec);
        }

        #endregion
    }
}
