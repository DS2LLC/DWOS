using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;

using DWOS.Data.Datasets.OrderShipmentDataSetTableAdapters;
using DWOS.Reports;
using DWOS.Server.Utilities;
using HtmlAgilityPack;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using COCDA = DWOS.Data.Datasets.COCDatasetTableAdapters;

namespace DWOS.Server.Tasks
{
    /// <summary>
    ///     Process all order shipments.
    /// </summary>
    public class ShipmentNotificationTask : IJob
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static readonly MessagingUtilities _messaging;

        private EmailLogoCopier _copier = new EmailLogoCopier();

        private bool _hasCOCsIncluded;

        private readonly Lazy<ApplicationSettingsDataSet.FieldsRow> _serialNumberFieldLazy = new Lazy
            <ApplicationSettingsDataSet.FieldsRow>(
                () =>
                {
                    ApplicationSettingsDataSet.FieldsDataTable fields;

                    using (var ta = new FieldsTableAdapter())
                    {
                        fields = ta.GetByCategory("Order");
                    }

                    var requiredDateField = fields.FirstOrDefault(f => f.Name == "Serial Number");
                    return requiredDateField;
                });

        #endregion

        #region Properties

        public bool IsSerialNumberEnabled
        {
            get
            {
                var serialNumberField = _serialNumberFieldLazy.Value;
                return serialNumberField == null || serialNumberField.IsRequired || serialNumberField.IsVisible;
            }
        }

        #endregion

        #region Methods

        static ShipmentNotificationTask()
        {
            _messaging = new MessagingUtilities("ShippingNotifications");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await Task.Run(() => BeginProcessing()).ConfigureAwait(false);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error running shipment notifications.");
            }
        }

        private void BeginProcessing()
        {
            _log.Info("BEGIN: ShipmentNotificationTask");

            try
            {
                _copier.CopyCompanyLogo(_messaging.EmailPickupDirectory);

                var shipmentDataset = GetData();
                var shipments = shipmentDataset.ShipmentPackage;
                var contacts = shipmentDataset.d_Contact;

                using(var taShipmentPackages = new ShipmentPackageTableAdapter())
                {
                    //for each shipment that was shipped
                    foreach(OrderShipmentDataSet.ShipmentPackageRow shipment in shipments)
                    {
                        QueueEmailNotifications(contacts, shipment);
                        shipment.NotificationSent = DateTime.Now;
                        taShipmentPackages.Update(shipment);
                    }
                }
            }
            catch(Exception exc)
            {
                const string errorMsg = "Error sending email notifications.";
                _log.Error(exc, errorMsg);
            }
            finally
            {
                _messaging.BeginProcessingEmails();
                _log.Info("END: ShipmentNotificationTask");
            }
        }

        private void QueueEmailNotifications(OrderShipmentDataSet.d_ContactDataTable contacts, OrderShipmentDataSet.ShipmentPackageRow shipment)
        {
            var html = new OrderShipmentNotificationHTML(_copier.CopiedLogoPath);

            if (IsSerialNumberEnabled)
            {
                html.AddSerialNumberColumn();
            }

            var msgInfo = new MessageInfo { FromAddress = new EmailAddress { Address = ApplicationSettings.Current.EmailFromAddress, DisplayName = "No Reply" }, Subject = "Order Shipment Notification" };

            //get the distinct people to mail to
            List<string> addresses = GetValidCustomerAddresses(shipment);
            addresses.RemoveAll(string.IsNullOrEmpty);

            if (addresses.Count > 0)
            {
                // Set customer info
                CustomersDataset.CustomerRow customer = GetCustomer(shipment.CustomerID);
                CustomersDataset.CustomerAddressRow customerAddress = GetCustomerAddress(shipment.IsCustomerAddressIDNull() ? -1 : shipment.CustomerAddressID);

                //Check if any addresses need to receive COC attachments
                List<CustomersDataset.ContactRow> customersWithCOC = GetCustomersWithCOC(customer, addresses);

                MessageInfo msgInfoWithCOC = new MessageInfo();
                List<string> addressesWithCOC = new List<string>();

                if (customersWithCOC.Count > 0)
                {
                    _hasCOCsIncluded = true;
                    msgInfoWithCOC = new MessageInfo { FromAddress = new EmailAddress { Address = ApplicationSettings.Current.EmailFromAddress, DisplayName = "No Reply" }, Subject = "Order Shipment Notification" };
                    addressesWithCOC = customersWithCOC.Select(c => c.EmailAddress).ToList();
                    addressesWithCOC.ForEach(add => addresses.Remove(add));
                    addressesWithCOC.ForEach(add => msgInfoWithCOC.ToAddresses.Add(new EmailAddress { Address = add }));
                }
                else
                {
                    msgInfoWithCOC = null; //Dispose
                    addressesWithCOC = null;
                }
                    

                //Add filtered out addresses to basic msgInfo
                addresses.ForEach(add => msgInfo.ToAddresses.Add(new EmailAddress { Address = add }));
               
                // Show the customer's ship-to address on this notification
                if (customerAddress != null)
                {
                    html.SetCustomer(customer.Name,
                        customerAddress.IsAddress1Null() ? string.Empty : customerAddress.Address1,
                        customerAddress.IsCityNull() ? string.Empty : customerAddress.City,
                        customerAddress.IsStateNull() ? string.Empty : customerAddress.State,
                        customerAddress.IsZipNull() ? string.Empty : customerAddress.Zip);
                }
                else
                {
                    html.SetCustomer(customer.Name,
                        customer.IsAddress1Null() ? string.Empty : customer.Address1,
                        customer.IsCityNull() ? string.Empty : customer.City,
                        customer.IsStateNull() ? string.Empty : customer.State,
                        customer.IsZipNull() ? string.Empty : customer.Zip);
                }

                //Set shipping info
                var shipDate = shipment.GetOrderShipmentRows()?.FirstOrDefault()?.DateShipped ?? shipment.CloseDate;
                if (shipDate.TimeOfDay > ApplicationSettings.Current.ShippingRolloverTime)
                {
                    shipDate = shipDate.AddBusinessDays(1);
                }

                html.SetShippingInfo(shipment.IsShippingCarrierIDNull() ? "None" : shipment.ShippingCarrierID, shipDate.ToShortDateString(), shipment.IsTrackingNumberNull() ? "None" : shipment.TrackingNumber);

                //Add each work order to the message
                AddWorkOrders(html, shipment);

                var allAddresses = new List<string>(addresses);
                if (addressesWithCOC != null)
                { 
                    allAddresses.AddRange(addressesWithCOC);
                    //update communications for contacts
                    AddContactsCommunications(shipment, contacts, allAddresses);
                }
               

                //check for COC attachments
                if (_hasCOCsIncluded)
                {
                    AddCOCAttachments(shipment, msgInfoWithCOC);
                    msgInfoWithCOC.Body = html.GetCompleteHtml();
                    msgInfoWithCOC.IsHtml = true;
                }
               
                msgInfo.Body = html.GetCompleteHtml();
                msgInfo.IsHtml = true;

                //queue email to be sent
                try
                {
                    if (msgInfo.ToAddresses.Count > 0)
                    _messaging.QueueEmail(msgInfo);
                    if (_hasCOCsIncluded)
                        _messaging.QueueEmail(msgInfoWithCOC);
                }
                catch (Exception exc)
                {
                    _log.Error(exc, "Error queueing emails for shipment.");
                }
            }
        }

    
        private OrderShipmentDataSet GetData()
        {
            var dsOrderShipment = new OrderShipmentDataSet();

            using (var taPackages = new ShipmentPackageTableAdapter())
            {
                taPackages.FillByShipmentsToNotify(dsOrderShipment.ShipmentPackage);
            }

            using (var taOrderShipment = new OrderShipmentTableAdapter { ClearBeforeFill = false })
            {
                foreach (var shipment in dsOrderShipment.ShipmentPackage)
                {
                    taOrderShipment.FillByShipmentPackageID(dsOrderShipment.OrderShipment, shipment.ShipmentPackageID);
                }
            }

            using (var taContacts = new d_ContactTableAdapter())
            {
                taContacts.Fill(dsOrderShipment.d_Contact);
            }

            using (ContactAdditionalCustomerSummaryTableAdapter taContactAdditionalCustomer = new ContactAdditionalCustomerSummaryTableAdapter())
            {
                taContactAdditionalCustomer.Fill(dsOrderShipment.ContactAdditionalCustomerSummary);
            }

            return dsOrderShipment;
        }

        private List <string> GetValidCustomerAddresses(params OrderShipmentDataSet.ShipmentPackageRow[] shipmentPackages)
        {
            List<string> customerAddresses = new List<string>();

            foreach(OrderShipmentDataSet.ShipmentPackageRow shipment in shipmentPackages)
            {
                var addresses = shipment.GetNotificationEmailAddresses() ?? Enumerable.Empty<string>();

                foreach (string address in addresses.Where(address => !customerAddresses.Contains(address) && !string.IsNullOrEmpty(address)))
                {
                    if (address == null || !address.IsValidEmail())
                    {
                        _log.Warn("Unable to send an email to \"{0}\"; this email address is invalid.", address);

                    }
                    else
                    {
                        customerAddresses.Add(address);
                    }
                }

            }

            return customerAddresses;
        }

        private CustomersDataset.CustomerRow GetCustomer(int customerID)
        {
            using(var taCustomer = new Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter())
            {
                using(var dtCustomer = new CustomersDataset.CustomerDataTable())
                {
                    taCustomer.FillBy(dtCustomer, customerID);
                    if(dtCustomer.Rows.Count > 0)
                        return dtCustomer[0];
                }
            }

            return null;
        }

        private CustomersDataset.CustomerAddressRow GetCustomerAddress(int customerAddressId)
        {
            using (var taCustomerAddress = new Data.Datasets.CustomersDatasetTableAdapters.CustomerAddressTableAdapter())
            {
                using (var dtCustomerAddress = new CustomersDataset.CustomerAddressDataTable())
                {
                    taCustomerAddress.FillByCustomerAddress(dtCustomerAddress, customerAddressId);
                    return dtCustomerAddress.FirstOrDefault();
                }
            }
        }
        private List<CustomersDataset.ContactRow> GetCustomersWithCOC(CustomersDataset.CustomerRow customer, List<string> addresses)
        {
            using (var taCustomerContact = new Data.Datasets.CustomersDatasetTableAdapters.ContactTableAdapter())
            {
                using (var dtCustomerContact = new CustomersDataset.ContactDataTable())
                {
                    taCustomerContact.FillBy(dtCustomerContact, customer.CustomerID);
                    return dtCustomerContact.Where(c => addresses.Contains(c.EmailAddress) && c.IncludeCOCInShippingNotifications).ToList();
                }
            }
        }

        /// <summary>
        ///     Adds a new communication to each contacts that a email was sent.
        /// </summary>
        /// <param name="shipment"> The shipment. </param>
        /// <param name="contacts"> The contacts. </param>
        /// <param name="addresses"> The addresses. </param>
        private void AddContactsCommunications(OrderShipmentDataSet.ShipmentPackageRow shipment, OrderShipmentDataSet.d_ContactDataTable contacts, List <string> addresses)
        {
            using(var taCustomerCommunication = new Data.Datasets.OrdersDataSetTableAdapters.CustomerCommunicationTableAdapter())
            {
                foreach(var order in shipment.GetOrderShipmentRows())
                {
                    foreach(string address in addresses)
                    {
                        var matchingContacts = contacts.FormattedSelect("CustomerID = {0} AND EmailAddress = '{1}'", shipment.CustomerID, address)
                            as OrderShipmentDataSet.d_ContactRow[];

                        if (matchingContacts.Length == 0 && ApplicationSettings.Current.AllowAdditionalCustomersForContacts)
                        {
                            // Try to match secondary contact
                            matchingContacts = contacts
                                .Where(c =>
                                    c.GetContactAdditionalCustomerSummaryRows().Any(customer => customer.IncludeInShippingNotifications && customer.CustomerID == shipment.CustomerID)
                                    && !c.IsEmailAddressNull()
                                    && c.EmailAddress == address)
                                .ToArray();
                        }

                        if (matchingContacts.Length > 0)
                            taCustomerCommunication.Insert(matchingContacts[0].ContactID, order.OrderID, "Shipment Notification Sent", DateTime.Now, order.ShippingUserID);
                    }
                }
            }
        }

        private void AddWorkOrders(OrderShipmentNotificationHTML htmlMsg,
            OrderShipmentDataSet.ShipmentPackageRow shipment)
        {
            ShipmentPartTableAdapter taParts = null;
            OrderShipmentTableAdapter taOrder = null;
            OrderSerialNumberTableAdapter taOrderSerialNumber = null;

            try
            {
                taParts = new ShipmentPartTableAdapter();
                taOrder = new OrderShipmentTableAdapter();
                taOrderSerialNumber = new OrderSerialNumberTableAdapter();

                //Add each work order to the message
                foreach (var orderShipment in taOrder.GetShipmentsByShipmentPackageID(shipment.ShipmentPackageID))
                {
                    var parts = taParts.GetData(orderShipment.OrderID);
                    var serialNumbers = taOrderSerialNumber.GetActiveByOrder(orderShipment.OrderID);

                    var workOrderInfo = new OrderShipmentNotificationHTML.WorkOrderInfo
                    {
                        OrderId = orderShipment.OrderID,
                        CustomerWo = taOrder.GetCustomerWO(orderShipment.OrderID) ?? "<None>",
                        PurchaseOrder = taOrder.GetPurchaseOrder(orderShipment.OrderID) ?? "<None>",
                        PartName = parts[0].Name,
                        Quantity = orderShipment.PartQuantity,
                        SerialNumbers = serialNumbers
                            .OrderBy(s => s.PartOrder)
                            .Select(s => s.IsNumberNull() ? string.Empty : s.Number)
                            .ToList()
                    };

                    htmlMsg.AddWorkOrder(workOrderInfo);
                }
            }
            finally
            {
                taOrder?.Dispose();
                taParts?.Dispose();
                taOrderSerialNumber?.Dispose();
            }
        }

        private void AddCOCAttachments(OrderShipmentDataSet.ShipmentPackageRow shipment, MessageInfo msgInfo)
        {
            List<string> COCPaths = new List<string>();
            var taCOC = new COCDA.COCTableAdapter();
            foreach (var order in shipment.GetOrderShipmentRows())
            {
                var cocID = taCOC.GetMostRecentCOCIDByOrderID(order.OrderID);
                if (cocID != null)
                {
                    var report = new COCReport(cocID.Value);
                    var reportPath = report.PublishReport(_messaging.EmailPickupDirectory);
                    COCPaths.Add(reportPath);
                }
            }
            msgInfo.Attachments = COCPaths;
        }
        #endregion
    }

    internal static class ShippingURLFormatter
    {
        #region Fields

        public const string UPS = "UPS";

        #endregion

        #region Methods

        public static string CreateURL(string carrierID, string trackingNumber)
        {
            if(string.IsNullOrEmpty(trackingNumber))
                return "None";

            if(carrierID == UPS)
                return String.Format("<a href=\"http://wwwapps.ups.com/WebTracking/processInputRequest?sort_by=status&tracknums_displayed=1&TypeOfInquiryNumber=T&loc=en_US&InquiryNumber1={0}&track.x=0&track.y=0\">{0}</a>", trackingNumber);

            return HttpUtility.HtmlEncode(trackingNumber);
        }

        #endregion
    }

    /// <summary>
    ///     This class loads the HTML message with the appropriate customer information.
    /// </summary>
    internal class OrderShipmentNotificationHTML : HtmlNotification
    {
        #region Fields

        private static string TAG_CUSTOMER_NAME = "customerName";
        private static string TAG_ADDRESS1 = "address1";
        private static string TAG_CITYSTATEZIP = "cityStateZip";

        private static string TAG_CARRIER = "shippingCarrier";
        private static string TAG_DATESHIPPED = "dateShipped";
        private static string TAG_TRACKINGNUMBER = "trackingNumber";

        private static string TAG_ROW_START = "shipmentTemplate";
        private static string TAG_TABLE = "shipmentTable";
        private bool _hasSerialNumberColumn;

        #endregion

        #region Properties

        private HtmlDocument Document { get; set; }

        private HtmlNode RowTemplate { get; set; }

        #endregion

        #region Methods

        public OrderShipmentNotificationHTML(string logoFilePath)
        {
            Document        = new HtmlDocument();

            base.ReplaceTokens(new Token(TAG_LOGO, logoFilePath));
            base.ReplaceSystemTokens();

            Document.Load(new StringReader(HTMLOutput));
        }
        
        protected override void CreateNotification()
        {
            using (var ta = new TemplatesTableAdapter())
            {
                ApplicationSettingsDataSet.TemplatesDataTable templates = ta.GetDataById("ShipmentNotification");
                HTMLOutput = templates.FirstOrDefault().Template;
            }
        }

        public void SetCustomer(string customerName, string address1, string city, string state, string zip)
        {
            var navigate = Document.CreateNavigator() as HtmlNodeNavigator;

            if(navigate.MoveToId(TAG_CUSTOMER_NAME))
                navigate.CurrentNode.InnerHtml = HttpUtility.HtmlEncode(customerName);
            if(navigate.MoveToId(TAG_ADDRESS1))
                navigate.CurrentNode.InnerHtml = HttpUtility.HtmlEncode(address1);
            if(navigate.MoveToId(TAG_CITYSTATEZIP))
                navigate.CurrentNode.InnerHtml = String.Format("{0}, {1} {2}", HttpUtility.HtmlEncode(city), HttpUtility.HtmlEncode(state), HttpUtility.HtmlEncode(zip));
        }

        public void SetShippingInfo(string carrier, string dateShipped, string trackingNumber)
        {
            var navigate = Document.CreateNavigator() as HtmlNodeNavigator;

            if(navigate.MoveToId(TAG_CARRIER))
                navigate.CurrentNode.InnerHtml = HttpUtility.HtmlEncode(carrier);
            if(navigate.MoveToId(TAG_DATESHIPPED))
                navigate.CurrentNode.InnerHtml = HttpUtility.HtmlEncode(dateShipped);
            if(navigate.MoveToId(TAG_TRACKINGNUMBER))
                navigate.CurrentNode.InnerHtml = ShippingURLFormatter.CreateURL(carrier, trackingNumber);
        }

        public void AddWorkOrder(WorkOrderInfo workOrderInfo)
        {
            var navigate = Document.CreateNavigator() as HtmlNodeNavigator;

            if (navigate == null || workOrderInfo == null)
            {
                return;
            }

            //get the template row
            if(RowTemplate == null && navigate.MoveToId(TAG_ROW_START))
            {
                RowTemplate = navigate.CurrentNode;
                RowTemplate.Id = string.Empty; //reset id so it does not get cloned
                RowTemplate.Remove();
            }

            //go to the table node and add new row based on template row
            if(RowTemplate != null && navigate.MoveToId(TAG_TABLE))
            {
                HtmlNode tableNode = navigate.CurrentNode;

                //Update the new rows style
                HtmlNode woRow = RowTemplate.Clone();
                string woRowStyle = woRow.GetAttributeValue("style", string.Empty);
                woRowStyle = woRowStyle.Replace("visibility: collapse;", string.Empty); //ensure style does not hide rows

                if(tableNode.ChildNodes.Count - 1 % 2 == 0)
                    woRowStyle += " background-color: #DDDDDD;"; //set alt background style

                woRow.SetAttributeValue("style", woRowStyle);

                //Set the rows column values
                var sb = new StringBuilder();
                sb.AppendLine("<td>" + HttpUtility.HtmlEncode(workOrderInfo.OrderId.ToString()) + "</td>");
                sb.AppendLine("<td>" + (workOrderInfo.CustomerWo == null ? "<None>" : HttpUtility.HtmlEncode(workOrderInfo.CustomerWo)) + "</td>");
                sb.AppendLine("<td>" + (workOrderInfo.PurchaseOrder == null ? "<None>" : HttpUtility.HtmlEncode(workOrderInfo.PurchaseOrder)) + "</td>");
                sb.AppendLine("<td>" + (workOrderInfo.PartName == null ? "<None>" : HttpUtility.HtmlEncode(workOrderInfo.PartName)) + "</td>");
                sb.AppendLine("<td>" + HttpUtility.HtmlEncode(workOrderInfo.Quantity.ToString()) + "</td>");

                if (_hasSerialNumberColumn)
                {
                    var serialNumberContent =
                        string.Join("<br />", workOrderInfo.SerialNumbers);

                    sb.AppendLine("<td>" + serialNumberContent + "</td>");
                }

                woRow.InnerHtml = sb.ToString();

                //add new row to end of table
                tableNode.AppendChild(woRow);
            }
        }

        public string GetCompleteHtml()
        {
            return Document.DocumentNode.OuterHtml;
        }

        public void AddSerialNumberColumn()
        {
            var navigate = Document.CreateNavigator() as HtmlNodeNavigator;

            if (navigate == null || !navigate.MoveToId(TAG_TABLE))
            {
                return;
            }

            var headerNode = navigate.CurrentNode.ChildNodes.FirstOrDefault(c => c.Name == "tr");
            var serialHeaderNode = headerNode?.ChildNodes.LastOrDefault(s => s.Name == "td")?.Clone();

            if (serialHeaderNode != null)
            {
                serialHeaderNode.InnerHtml = "Serial Number";
                headerNode.AppendChild(serialHeaderNode);

                _hasSerialNumberColumn = true;
            }
        }

        #endregion

        #region WorkOrderInfo

        public class WorkOrderInfo
        {
            public int OrderId
            {
                get; set;
            }

            public string CustomerWo
            {
                get; set;
            }

            public string PurchaseOrder
            {
                get; set;
            }

            public string PartName
            {
                get; set;
            }

            public int Quantity
            {
                get; set;
            }

            public IList<string> SerialNumbers
            {
                get; set;
            }
        }

        #endregion
    }
}