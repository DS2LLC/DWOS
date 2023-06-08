using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Reports;
using DWOS.Data.Reports.OrdersReportTableAdapters;
using DWOS.Data.Reports.QAReportTableAdapters;
using DWOS.Data.Utilities;
using DWOS.Reports.Properties;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using ContentAlignment = Infragistics.Documents.Reports.Report.ContentAlignment;
using DWOS.Data.Reports.ProcessPartsReportTableAdapters;
using DWOS.Data.Order;
using DWOS.Data.Process;
using NLog;
using DWOS.Reports.ReportData;
using DWOS.Reports.Utilities;
using System.Threading;
using DWOS.Data.Datasets.ItemHistoryDataSetTableAdapters;

namespace DWOS.Reports
{
    public class WorkOrderTravelerReport : Report
    {
        #region Fields

        private const string SMALL_FONT_START = "<font face=\"Verdana\" size=\"9\">";
        private const string FONT_END = "</font>";
        private bool _isCustomerCOD = false;
        private OrdersDataSet.OrderRow _order;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Work Order Traveler"; }
        }

        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }

        protected override int FilenameIdentifier => _order?.OrderID ?? 0;

        protected override string FooterIdentifier
        {
            get
            {
                return _order.OrderID.ToString();
            }
        }

        // This report puts barcodes at the top of the footer.
        // Space required was determined through trial & error to match what it was before
        // moving barcodes to the footer.
        protected override float AdditionalFooterSpace => 56;

        #endregion

        #region Methods

        public WorkOrderTravelerReport(OrdersDataSet.OrderRow order) { this._order = order; }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            try
            {
                if (_order != null && _order.OrderID >= 0)
                {
                    ReportNotifier.OnReportCreated(new ReportCreatedEventArgs(_order.OrderID, Title, SecurityManager?.UserName));
                }

                SetupReportInfo();

                var subtitle = AddHeader("WORK ORDER TRAVELER ", "Work Order: ", this._order.OrderID, false, this._order, ReportType.WorkOrderTraveler);

                if (subtitle != null && _order != null)
                {
                    //Add subtitle text if this is not a standard order
                    IText subtitleText = subtitle.AddText();
                    subtitleText.Alignment.Horizontal = Alignment.Right;
                    subtitleText.Alignment.Vertical = Alignment.Top;
                    subtitleText.Style = DefaultStyles.RedLargeStyle;
                    subtitleText.Paddings = new Paddings(5, 5, 5, 5);
                    
                    if (_order.OrderType != (int)OrderType.Normal)
                    {                       
                        string originalOrderContent = string.Empty;

                        switch (_order.OrderType)
                        {
                            case 3:
                                originalOrderContent = GetOrignalOrderId(_order.OrderID);
                                subtitleText.AddContent($"Rework External{originalOrderContent}");
                                break;
                            case 4:
                                originalOrderContent = GetOrignalOrderId(_order.OrderID);
                                subtitleText.AddContent($"Rework Internal{originalOrderContent}");
                                break;
                            case 5:
                                originalOrderContent = GetOrignalOrderId(_order.OrderID);
                                subtitleText.AddContent($"Rework Hold{originalOrderContent}");
                                break;
                            case 7:
                                originalOrderContent = GetOrignalOrderId(_order.OrderID);
                                subtitleText.AddContent($"Quarantine{originalOrderContent}");
                                break;
                        }
                    }
                    else
                    {
                        //Check if the order was split (split orders get marked as OrderType.Normal) 
                        var splitOrderContent = GetSplitOrderId(_order.OrderID);
                        if (!string.IsNullOrEmpty(splitOrderContent))
                            subtitleText.AddContent($"Rework Internal{splitOrderContent}");
                    }
                   
                    AddCustomerSection();
                    AddPartSection();

                    if (_order.GetOrderSerialNumberRows().Any(i => i.IsValidState() && i.Active))
                    {
                        AddSerialNumberSection();
                    }

                    AddProcessSection();

                    if (GetInternalReworks(_order).Count > 0)
                    {
                        AddReworkSection();
                    }

                    AddBarCodeCommands();
                }
            }
            catch (Exception exc)
            {
                _log.Fatal(exc, "Error creating WorkOrderTraveler report.");
            }
        }

        private string GetOrignalOrderId(int orderId)
        {
            int originalOrderId = -1;
            using (var ta = new InternalReworkTableAdapter())
            {
                var dt = ta.GetByReworkOrderID(orderId);
                if (dt.Rows.Count > 0)
                    originalOrderId = dt[0].OriginalOrderID;
            }
            return originalOrderId > 0 ? $" - Parent Order: {originalOrderId}" : string.Empty;
        }

        private string GetSplitOrderId(int orderId)
        {
            int originalOrderId = -1;
            using (var ta = new InternalReworkTableAdapter())
            {
                var dt = ta.GetByReworkOrderID(orderId);
                if (dt.Rows.Count > 0 && dt[0].ReworkType == ReworkType.Split.ToString())
                    originalOrderId = dt[0].OriginalOrderID;
            }
            return originalOrderId > 0 ? $" - Split From: {originalOrderId}" : string.Empty;
        }

        private static List<OrdersDataSet.InternalReworkRow> GetInternalReworks(OrdersDataSet.OrderRow order)
        {
            if (order.GetInternalReworkRows().Length > 0)
            {
                // Internal reworks were previously loaded
                return order.GetInternalReworkRows().ToList();
            }

            using (var dtInternalRework = new OrdersDataSet.InternalReworkDataTable())
            {
                using (var taInternalRework = new InternalReworkTableAdapter())
                {
                    taInternalRework.FillByOriginalOrderID(dtInternalRework, order.OrderID);
                }

                return dtInternalRework.ToList();
            }
        }

        private void AddCustomerSection()
        {
            const int containerHeight = 110;

            try
            {
                IGroup headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Background = DefaultStyles.DefaultBackground;
                headerGroup.Margins = new Margins(5, 5, 3, 0);

                Infragistics.Documents.Reports.Report.IContainer customerContainer = headerGroup.AddContainer("customer");
                customerContainer.Width = new RelativeWidth(33);
                customerContainer.Borders = DefaultStyles.DefaultBorders;
                customerContainer.Paddings.All = 4;
                customerContainer.Margins.Right = 5;
                customerContainer.Background = DefaultStyles.DefaultBackground;
                customerContainer.Height = new FixedHeight(containerHeight);

                AddCustomerTable(customerContainer);

                Infragistics.Documents.Reports.Report.IContainer parentContainer = headerGroup.AddContainer("parent");
                parentContainer.Width = new FixedWidth(373);
                parentContainer.Alignment.Vertical = Alignment.Middle;
                parentContainer.Alignment.Horizontal = Alignment.Left;
                ITable parentTable = parentContainer.AddTable();
                ITableRow prow1 = parentTable.AddRow();

                prow1.Height = new FixedHeight(containerHeight);
                ITableCell pcell1 = prow1.AddCell();
                pcell1.Alignment.Horizontal = Alignment.Left;
                pcell1.Alignment.Vertical = Alignment.Middle;
                pcell1.Width = new FixedWidth(190);
                Infragistics.Documents.Reports.Report.IContainer orderContainer = pcell1.AddContainer("order");
                ITableCell pcell2 = prow1.AddCell();
                pcell2.Alignment.Horizontal = Alignment.Left;
                pcell2.Alignment.Vertical = Alignment.Middle;
                Infragistics.Documents.Reports.Report.IContainer reqDateContainer = pcell2.AddContainer("reqDate");

                orderContainer.Alignment.Vertical = Alignment.Top;
                orderContainer.Alignment.Horizontal = Alignment.Left;
                orderContainer.Paddings.Left = 5;
                orderContainer.Paddings.Top = 4;
                orderContainer.Borders = DefaultStyles.DefaultBorders;
                orderContainer.Height = new FixedHeight(containerHeight);

                reqDateContainer.Paddings.Left = 5;
                reqDateContainer.Paddings.Top = 4;
                reqDateContainer.Height = new FixedHeight(containerHeight);
                reqDateContainer.Alignment.Vertical = Alignment.Top;
                reqDateContainer.Alignment.Horizontal = Alignment.Left;
                reqDateContainer.Borders = DefaultStyles.DefaultBorders;
                reqDateContainer.Margins.Left = 5;

                AddRequiredDateTable(reqDateContainer);

                AddOrderTable(orderContainer);

            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding customer section to report.");
            }
        }

        private void AddCustomerTable(Infragistics.Documents.Reports.Report.IContainer companyContainer)
        {
            const int maxLength = 35;
            const int maxCustomerLength = 30;
            const string ellipsis = "…";

            CustomersDataset dsCustomer = null;

            try
            {
                dsCustomer = new CustomersDataset()
                {
                    EnforceConstraints = false
                };

                using (var taCustomer = new CustomerTableAdapter())
                {
                    taCustomer.FillByOrderID(dsCustomer.Customer, this._order.OrderID);
                }

                var customer = dsCustomer.Customer.FirstOrDefault();

                if (!_order.IsCustomerAddressIDNull())
                {
                    using (var taCustomerAddress = new Data.Datasets.CustomersDatasetTableAdapters.CustomerAddressTableAdapter())
                    {
                        taCustomerAddress.FillByCustomerAddress(dsCustomer.CustomerAddress, _order.CustomerAddressID);
                    }

                    using (var taCountry = new CountryTableAdapter())
                    {
                        taCountry.Fill(dsCustomer.Country);
                    }
                }

                var companyTable = companyContainer.AddTable();
                ITableRow companyTitle = companyTable.AddRow();
                companyTitle.AddCell(100, "Customer:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                ITableRow companyName = companyTable.AddRow();

                companyName.AddCell(100, "   " + customer.Name.TrimToMaxLength(maxCustomerLength, ellipsis),
                    DefaultStyles.BoldMediumStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                if (customer.HasBillingAddress)
                {
                    if (!customer.IsAddress1Null() && customer.Address1 != customer.Name)
                    {
                        ITableRow companyAddress = companyTable.AddRow();
                        companyAddress.AddCell(50, "   " + customer.Address1.TrimToMaxLength(maxLength, ellipsis),
                            DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                    }

                    ITableRow addressRow = companyTable.AddRow();
                    string zip = customer.IsZipNull() ? "" : customer.Zip;
                    var addressLastLine = (customer.IsCityNull() ? "-" : customer.City) + " " + (customer.IsStateNull() ? "-" : customer.State) + ", " + zip;

                    if (customer.CountryID != ApplicationSettings.Current.CompanyCountry)
                    {
                        addressLastLine += $" {customer.CountryRow.Name}";
                    }

                    addressRow.AddCell(50, "   " + addressLastLine.TrimToMaxLength(maxLength, ellipsis),
                        DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                }

                var customerAddress = customer.GetCustomerAddressRows().FirstOrDefault();
                if (customerAddress != null)
                {
                    var shippingTitle = companyTable.AddRow();

                    shippingTitle.AddCell(100, "Ship To:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                    var shippingNameRow = companyTable.AddRow();
                    shippingNameRow.AddCell(50, "   " + customerAddress.Name.TrimToMaxLength(maxLength, ellipsis),
                        DefaultStyles.NormalStyle,
                        new TextAlignment(Alignment.Left, Alignment.Middle));


                    if (!customerAddress.IsAddress1Null() && customerAddress.Address1 != customer.Name)
                    {
                        var shippingStreetAddressRow = companyTable.AddRow();
                        shippingStreetAddressRow.AddCell(50, "   " + customerAddress.Address1.TrimToMaxLength(maxLength, ellipsis),
                            DefaultStyles.NormalStyle,
                            new TextAlignment(Alignment.Left, Alignment.Middle));
                    }

                    if (!customerAddress.IsAddress2Null() && !string.IsNullOrEmpty(customerAddress.Address2))
                    {
                        var shippingStreetAddressRow = companyTable.AddRow();
                        shippingStreetAddressRow.AddCell(50, "   " + customerAddress.Address2.TrimToMaxLength(maxLength, ellipsis),
                            DefaultStyles.NormalStyle,
                            new TextAlignment(Alignment.Left, Alignment.Middle));
                    }

                    var shippingAddressRow = companyTable.AddRow();
                    var shipCity = customerAddress.IsCityNull() ? "-" : customerAddress.City;
                    var shipZip = customerAddress.IsZipNull() ? "" : customerAddress.Zip;
                    var shipState = customerAddress.IsStateNull() ? "-" : customerAddress.State;
                    var shipLastLine = shipCity + " " + shipState + ", " + shipZip;

                    if (customerAddress.CountryID != ApplicationSettings.Current.CompanyCountry)
                    {
                        shipLastLine += $" {customerAddress.CountryRow.Name}";
                    }

                    shippingAddressRow.AddCell(50, "   " + shipLastLine.TrimToMaxLength(maxLength, ellipsis),
                        DefaultStyles.NormalStyle,
                        new TextAlignment(Alignment.Left, Alignment.Middle));
                }
                if (!customer.IsPaymentTermsNull() && customer.PaymentTerms == "COD")
                {
                    _isCustomerCOD = true;
                }
            }
            finally
            {
                dsCustomer?.Dispose();
            }
        }

        private void AddRequiredDateTable(Infragistics.Documents.Reports.Report.IContainer reqDateContainer)
        {
            ITable reqDateTable = reqDateContainer.AddTable();
            ITableRow row5 = reqDateTable.AddRow();
            row5.Height = new FixedHeight(20);

            ITableCell cell1 = row5.AddCell(40, "Est. Ship Date:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            cell1.Height = new FixedHeight(20);
            ITableRow row6 = reqDateTable.AddRow();
            row6.Height = new FixedHeight(20);
            ITableCell cell2 = row6.AddCell(40,
                this._order.IsEstShipDateNull() ? "NA" : _order.EstShipDate.ToShortDateString(),
                new Style(new Font(DefaultStyles.RedXLargeStyle.Font.Name, 20, FontStyle.Bold), DefaultStyles.BlackXLargeStyle.Brush),
                new TextAlignment(Alignment.Center, Alignment.Middle));

            cell2.Height = new FixedHeight(20);
        }

        private void AddOrderTable(Infragistics.Documents.Reports.Report.IContainer orderContainer)
        {
            const int cellRelativeWidth = 15;
            const int maxLength = 17;

            var labelCellMargins = new HorizontalMargins(0, 0);

            var orderTable = orderContainer.AddTable();
            var orderLabelCell = orderTable.AddRow().AddCell(100, "Order:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

            var purchaseOrderRow = orderTable.AddRow();
            var purchaseOrderLabelCell = purchaseOrderRow.AddCell(cellRelativeWidth, "   Purchase Order:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            purchaseOrderLabelCell.Margins = labelCellMargins;
            purchaseOrderRow.AddCell(cellRelativeWidth, _order.IsPurchaseOrderNull() ? "NA" : _order.PurchaseOrder.TrimToMaxLength(maxLength, "."), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

            var customerWorkOrderRow = orderTable.AddRow();
            var customerWoLabelCell = customerWorkOrderRow.AddCell(cellRelativeWidth, "   Customer WO:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            customerWoLabelCell.Margins = labelCellMargins;
            customerWorkOrderRow.AddCell(cellRelativeWidth, _order.IsCustomerWONull() ? "None" : _order.CustomerWO.TrimToMaxLength(maxLength, "."), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

            var receivedRow = orderTable.AddRow();
            var receivedLabelCell = receivedRow.AddCell(cellRelativeWidth, "   Received Date:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            receivedLabelCell.Margins = labelCellMargins;
            receivedRow.AddCell(cellRelativeWidth, _order.OrderDate.ToShortDateString(), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

            var orderReceiptRow = orderTable.AddRow();
            var orderReceiptLabelCell = orderReceiptRow.AddCell(cellRelativeWidth, "   Order Receipt:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            orderReceiptLabelCell.Margins = labelCellMargins;
            orderReceiptRow.AddCell(cellRelativeWidth, _order.IsReceivingIDNull() ? "None" : _order.ReceivingID.ToString(), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

            // Sales Order
            if (!_order.IsSalesOrderIDNull())
            {
                var salesOrderRow = orderTable.AddRow();
                var salesOrderLabelCell = salesOrderRow.AddCell(cellRelativeWidth, "   Sales Order:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                salesOrderLabelCell.Margins = labelCellMargins;
                salesOrderRow.AddCell(cellRelativeWidth, _order.SalesOrderID.ToString(), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            }

            // Shipping method
            var shippingMethodRow = orderTable.AddRow();
            var shippingMethodLabelCell = shippingMethodRow.AddCell(cellRelativeWidth, "   Shipping Method:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            shippingMethodLabelCell.Margins = labelCellMargins;
            shippingMethodRow.AddCell(cellRelativeWidth, GetCustomerShippingName(_order), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

            // Payment Terms if COD
            if(ApplicationSettings.Current.ShowCODOnTraveler && _isCustomerCOD)
            {
                var TermsRow = orderTable.AddRow();
                var TermsLabelCell = TermsRow.AddCell(cellRelativeWidth, "   Payment Terms:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                TermsLabelCell.Margins = labelCellMargins;
                var RedBold = new Style(new Font(DefaultStyles.RedXLargeStyle.Font.Name, DefaultStyles.RedXLargeStyle.Font.Size + 10, FontStyle.Bold), DefaultStyles.RedXLargeStyle.Brush);
                TermsRow.AddCell(cellRelativeWidth, "C.O.D.", RedBold, new TextAlignment(Alignment.Left, Alignment.Middle));

            }


            // Current Line
            if (ApplicationSettings.Current.MultipleLinesEnabled)
            {
                var lineRow = orderTable.AddRow();
                var lineLabelCell = lineRow.AddCell(cellRelativeWidth, "   Processing Line:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                lineLabelCell.Margins = labelCellMargins;
                lineRow.AddCell(cellRelativeWidth, GetLineName(_order), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            }
        }

        private static string GetCustomerShippingName(OrdersDataSet.OrderRow order)
        {
            const string defaultCustomerShippingName = "None";

            if (order.IsShippingMethodNull())
            {
                return defaultCustomerShippingName;
            }

            if (order.CustomerShippingSummaryRow != null)
            {
                return order.CustomerShippingSummaryRow.Name;
            }

            // Retrieve customer shipping from database
            using (var dsCustomerShipping = new OrdersDataSet.CustomerShippingSummaryDataTable())
            {
                using (var taCustomerShipping = new CustomerShippingSummaryTableAdapter())
                {
                    taCustomerShipping.FillByOrder(dsCustomerShipping, order.OrderID);
                }

                return dsCustomerShipping.FirstOrDefault(s => s.CustomerShippingID == order.ShippingMethod)?.Name ?? defaultCustomerShippingName;
            }
        }

        private static string GetLineName(OrdersDataSet.OrderRow order)
        {
            const string defaultLineName = "NA";

            if (order.IsCurrentLineNull())
            {
                return defaultLineName;
            }

            if (order.ProcessingLineRow != null)
            {
                return order.ProcessingLineRow.Name;
            }

            // Retrieve processing line from database
            using (var taProcessingLine = new Data.Datasets.OrdersDataSetTableAdapters.ProcessingLineTableAdapter())
            {
                using (var dtProcessingLine = taProcessingLine.GetData())
                {
                    return dtProcessingLine.FirstOrDefault(l => l.ProcessingLineID == order.CurrentLine)?.Name ?? defaultLineName;
                }
            }
        }

        private void AddPartSection()
        {
            var appSettings = ApplicationSettings.Current;
            var dtMedia = new OrderProcessingDataSet.MediaDataTable();
            var dtPart = new OrderProcessingDataSet.PartDataTable();
            var taMedia = new Data.Datasets.OrderProcessingDataSetTableAdapters.MediaTableAdapter();
            var taParts = new PartTableAdapter();
            const int NOTE_MAX_LENGTH = 2000;

            try
            {
                bool isMaterialVisible;
                bool isSurfaceAreaVisible;
                bool isManufacturerVisible;
                bool isRevisionVisible;

                using (var taField = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
                {
                    using (var dtPartFields = taField.GetByCategory("Part"))
                    {
                        // Material
                        var materialField = dtPartFields.FirstOrDefault(field => field.Name == "Material");
                        isMaterialVisible = materialField != null && materialField.IsVisible;

                        // Surface Area
                        var surfaceAreaField = dtPartFields.FirstOrDefault(field => field.Name == "Surface Area");
                        isSurfaceAreaVisible = surfaceAreaField != null && surfaceAreaField.IsVisible;

                        // Manufacturer
                        var manufacturerField = dtPartFields.FirstOrDefault(field => field.Name == "Manufacturer");
                        isManufacturerVisible = manufacturerField != null && manufacturerField.IsVisible;

                        // Revision
                        var revisionField = dtPartFields.FirstOrDefault(field => field.Name == "Part Rev.");
                        isRevisionVisible = revisionField != null && revisionField.IsVisible;
                    }
                }

                IGroup headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Margins = new Margins(5, 5, 3, 0);
                headerGroup.Borders = DefaultStyles.DefaultBorders;
                headerGroup.Background = DefaultStyles.DefaultBackground;

                Infragistics.Documents.Reports.Report.IContainer orderContainer = headerGroup.AddContainer("order");
                orderContainer.Width = new RelativeWidth(45);
                orderContainer.Paddings.All = 5;
                orderContainer.Margins.Right = 5;

                // Check if partID was valid (user can clear/delete part field in OrderEntry then click print which causes problem)
                if (!this._order.IsPartIDNull())
                    taParts.FillBy(dtPart, this._order.PartID);

                //Add Part info table
                ITable orderTable = orderContainer.AddTable();
                orderTable.AddRow().AddCells(DefaultStyles.BlueLargeStyle, new SolidColorBrush(new Color(System.Drawing.Color.WhiteSmoke)), null, 0, TextAlignment.Left, "Part Information:");

                var partName = (dtPart.Count > 0 ? dtPart[0].Name : "Unknown");

                if (isRevisionVisible)
                {
                    var revision = dtPart.Count > 0 && !dtPart[0].IsRevisionNull() ? dtPart[0].Revision : string.Empty;

                    if (!string.IsNullOrEmpty(revision))
                    {
                        partName += $" Rev. {revision}";
                    }
                }

                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Part Number:", partName);
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Description:", (dtPart.Count > 0 && !dtPart[0].IsDescriptionNull() ? dtPart[0].Description : "Unknown"));

                if (isMaterialVisible)
                {
                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Material:", (dtPart.Count > 0 && !dtPart[0].IsMaterialNull() ? dtPart[0].Material : "Unknown"));
                }

                if (isSurfaceAreaVisible)
                {
                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Dimensions:", (dtPart.Count > 0 ? PartAreaUtilities.PartDimensionString(dtPart[0]) : "None"));
                }

                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Quantity:", (this._order.IsPartQuantityNull() ? "Unknown" : this._order.PartQuantity.ToString()));

                var weightFormat = $"0.{"0".Repeat(appSettings.WeightDecimalPlaces)}";
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle),
                    "   Total Weight:",
                    _order.IsWeightNull() ? "Unknown" : $"{_order.Weight.ToString(weightFormat)} lbs.");

                if (isManufacturerVisible)
                {
                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Manufacturer:", (dtPart.Count > 0 && !dtPart[0].IsManufacturerIDNull() ? dtPart[0].ManufacturerID : "None"));
                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Model:", (dtPart.Count > 0 && !dtPart[0].IsAirframeNull() ? dtPart[0].Airframe : "None"));
                }

                //Add Custom Fields to traveler
                using (var taOrderCustomField = new Data.Datasets.OrdersDataSetTableAdapters.OrderCustomFieldsTableAdapter())
                {
                    foreach (var orderCustomField in taOrderCustomField.GetWOTravelerValues(_order.CustomerID, _order.OrderID))
                    {
                        if (orderCustomField.IsValueNull() || string.IsNullOrWhiteSpace(orderCustomField.Value))
                        {
                            continue;
                        }

                        var fieldName = $"   {orderCustomField["Name"]}:";
                        var fieldValue = orderCustomField.Value;

                        orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0,
                            new TextAlignment(Alignment.Left, Alignment.Middle), fieldName, fieldValue);
                    }
                }

                // Add part-level custom fields.
                if (!_order.IsPartIDNull())
                {
                    using (var dsParts = new PartsDataset { EnforceConstraints = false })
                    {
                        using (var taPartLevelCustomField = new Data.Datasets.PartsDatasetTableAdapters.PartLevelCustomFieldTableAdapter())
                        {
                            taPartLevelCustomField.FillByPartID(dsParts.PartLevelCustomField, _order.PartID);
                        }

                        using (var taPartCustomFields = new Data.Datasets.PartsDatasetTableAdapters.PartCustomFieldsTableAdapter())
                        {
                            taPartCustomFields.FillByPartID(dsParts.PartCustomFields, _order.PartID);
                        }

                        foreach (var customField in dsParts.PartCustomFields)
                        {
                            if (customField.PartLevelCustomFieldRow == null || !customField.PartLevelCustomFieldRow.DisplayOnTraveler)
                            {
                                continue;
                            }

                            var fieldName = $"   {customField.PartLevelCustomFieldRow.Name}:";
                            var fieldValue = customField.IsValueNull() ? string.Empty : customField.Value;

                            orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0,
                                new TextAlignment(Alignment.Left, Alignment.Middle), fieldName, fieldValue);
                        }
                    }
                }


                if (dtPart.Count > 0 && !dtPart[0].IsNotesNull())
                {
                    orderTable.AddRow().AddCells(DefaultStyles.BlueBoldStyle, new SolidColorBrush(new Color(System.Drawing.Color.WhiteSmoke)), null, 0, TextAlignment.Left, "Part Notes:");
                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), dtPart[0].Notes.TrimToMaxLength(NOTE_MAX_LENGTH, "..."))[0].Paddings.Left = 5;
                }

                //Get any Order Notes
                var dtNotes = new OrdersDataSet.OrderNoteDataTable();
                using (var taOrderNotes = new OrderNoteTableAdapter())
                    taOrderNotes.FillByOrder(dtNotes, this._order.OrderID);

                // Add the order notes
                if (dtNotes.Count > 0)
                {
                    IEnumerable<OrdersDataSet.OrderNoteRow> internalNotes = dtNotes.Where(o => o.NoteType == "Internal");
                    IEnumerable<OrdersDataSet.OrderNoteRow> externalNotes = dtNotes.Where(o => o.NoteType == "External");

                    var noteSummary = new StringBuilder();
                    foreach (OrdersDataSet.OrderNoteRow externalNote in externalNotes)
                        noteSummary.AppendLine("   " + externalNote.Notes.TrimToMaxLength(NOTE_MAX_LENGTH, "..."));

                    if (internalNotes.Any())
                        noteSummary.AppendLine("See INTERNAL notes.");

                    orderTable.AddRow().AddCells(DefaultStyles.BlueBoldStyle, new SolidColorBrush(new Color(System.Drawing.Color.WhiteSmoke)), null, 0, TextAlignment.Left, "Order Notes:");
                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), noteSummary.ToString())[0].Paddings.Left = 5;
                }

                //Add Part Marking paragraph
                using (var taOPM = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderPartMarkTableAdapter())
                {
                    var orderPMTable = new OrderProcessingDataSet.OrderPartMarkDataTable();
                    taOPM.Fill(orderPMTable, this._order.OrderID);

                    var orderPMRow = orderPMTable.FirstOrDefault();

                    //if there is a order template then use that
                    if (orderPMRow != null)
                    {
                        // Check if partID was valid (user can clear/delete part field in OrderEntry then click print which causes problem)
                        if (dtPart.Rows.Count > 0 && !this._order.IsPartIDNull())
                        {
                            using (var taPart = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
                            {
                                var partDT = new PartsDataset.PartDataTable();

                                taPart.FillByPartID(partDT, dtPart[0].PartID);

                                Infragistics.Documents.Reports.Report.IContainer pmCon = orderContainer.AddContainer("PM");
                                pmCon.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);

                                ITable partMarkingTable = pmCon.AddTable();

                                partMarkingTable.AddRow().AddCells(DefaultStyles.BlueBoldStyle, new SolidColorBrush(new Color(System.Drawing.Color.WhiteSmoke)), null, 0, TextAlignment.Left, "Part Marking:");
                                if (!orderPMRow.IsLine1Null())
                                    partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(orderPMRow.Line1, partDT[0], this._order));
                                if (!orderPMRow.IsLine2Null())
                                    partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(orderPMRow.Line2, partDT[0], this._order));
                                if (!orderPMRow.IsLine3Null())
                                    partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(orderPMRow.Line3, partDT[0], this._order));
                                if (!orderPMRow.IsLine4Null())
                                    partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(orderPMRow.Line4, partDT[0], this._order));
                            }
                        }
                    }
                    else if (dtPart[0].PartMarking && !dtPart[0].IsAirframeNull()) //else use the parts PM template, if exists
                    {
                        // Check if partID was valid (user can clear/delete part field in OrderEntry then click print which causes problem)
                        if (dtPart.Rows.Count > 0 && !this._order.IsPartIDNull())
                        {
                            using (var taPM = new Data.Datasets.OrderProcessingDataSetTableAdapters.PartMarkingTableAdapter())
                            {
                                var pmDT = taPM.GetDataByPart(dtPart[0].PartID);

                                if (pmDT.Count > 0)
                                {
                                    using (var taPart = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
                                    {
                                        var partDT = new PartsDataset.PartDataTable();
                                        taPart.FillByPartID(partDT, dtPart[0].PartID);

                                        OrderProcessingDataSet.PartMarkingRow pmRow = pmDT[0];

                                        Infragistics.Documents.Reports.Report.IContainer pmCon = orderContainer.AddContainer("PM");
                                        pmCon.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);

                                        ITable partMarkingTable = pmCon.AddTable();

                                        partMarkingTable.AddRow().AddCells(DefaultStyles.BlueBoldStyle, new SolidColorBrush(new Color(System.Drawing.Color.WhiteSmoke)), null, 0, TextAlignment.Left, "Part Marking:");
                                        if (!pmRow.IsDef1Null())
                                            partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(pmRow.Def1, partDT[0], this._order));
                                        if (!pmRow.IsDef2Null())
                                            partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(pmRow.Def2, partDT[0], this._order));
                                        if (!pmRow.IsDef3Null())
                                            partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(pmRow.Def3, partDT[0], this._order));
                                        if (!pmRow.IsDef4Null())
                                            partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(pmRow.Def4, partDT[0], this._order));
                                    }
                                }
                            }
                        }
                    }
                }



                // Check if partID was valid (user can clear/delete part field in OrderEntry then click print which causes problem)
                if (!this._order.IsPartIDNull())
                    taMedia.FillWithoutMedia(dtMedia, this._order.PartID);

                System.Drawing.Image img = null;

                if (dtMedia.Count > 0)
                {
                    img = MediaUtilities.GetImage(dtMedia[0].MediaID, dtMedia[0].FileExtension);
                }
                else if (ApplicationSettings.Current.UseReportPlaceholderImage)
                {
                    if (File.Exists(ApplicationSettings.Current.ReportPlaceholderImagePath))
                    {
                        img = MediaUtilities.GetImage(File.ReadAllBytes(ApplicationSettings.Current.ReportPlaceholderImagePath));
                    }
                    else
                    {
                        img = Data.Properties.Resources.NoImage;
                    }
                }

                if (img == null)
                {
                    orderContainer.Width = new RelativeWidth(100);
                }
                else
                {

                    //Add Part Image
                    Infragistics.Documents.Reports.Report.IContainer imgContainer = headerGroup.AddContainer("partImage");
                    imgContainer.Alignment.Vertical = Alignment.Middle;
                    imgContainer.Alignment.Horizontal = Alignment.Right;
                    imgContainer.Paddings.All = 5;
                    imgContainer.Paddings.Right = 5;
                    imgContainer.Margins.Left = 5;
                    imgContainer.Margins.Right = 8;
                    imgContainer.Width = new RelativeWidth(55);


                    IImage image = imgContainer.AddImage(new Image(img));
                    image.KeepRatio = true;
                    int pWidth = img.Width;
                    int pHeight = img.Height;
                    double standardWidth = 270;

                    if (img.Width > 270)
                    {
                        double percent = standardWidth / pWidth;
                        image.Width = new FixedWidth(270);
                        double height = percent * pHeight;
                        image.Height = new FixedHeight((float)height);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Fatal(exc, "Error adding part section to report.");
            }
            finally
            {
                if (dtMedia != null)
                    dtMedia.Dispose();
                if (dtPart != null)
                    dtPart.Dispose();
                if (taMedia != null)
                    dtMedia.Dispose();
                if (taParts != null)
                    taParts.Dispose();

                dtMedia = null;
                dtPart = null;
                taMedia = null;
                taParts = null;
            }
        }

        private void AddSerialNumberSection()
        {
            try
            {
                var headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Margins = new Margins(5, 5, 3, 0);
                headerGroup.Borders = DefaultStyles.DefaultBorders;
                headerGroup.Background = DefaultStyles.DefaultBackground;

                var serialNumberContainer = headerGroup.AddContainer("serial");
                serialNumberContainer.Width = new RelativeWidth(100);
                serialNumberContainer.Paddings.All = 5;
                serialNumberContainer.Margins.Right = 5;

                var serialTable = serialNumberContainer.AddTable();
                serialTable.AddRow()
                    .AddCells(DefaultStyles.BlueLargeStyle,
                        null, 0, TextAlignment.Left,
                        "Serial Numbers:");

                var activeSerialNumbers = _order.GetOrderSerialNumberRows()
                    .Where(s => s.IsValidState() && s.Active && !s.IsNumberNull())
                    .OrderBy(s => s.PartOrder)
                    .Select(s => s.Number);

                serialTable.AddRow()
                    .AddCells(DefaultStyles.NormalStyle, null, 0, TextAlignment.Left, string.Join(", ", activeSerialNumbers));
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding serial number section to report.");
            }
        }

        /// <summary>
        ///     Adds the process section describing what processes are going to done on the part.
        /// </summary>
        private void AddProcessSection()
        {
            try
            {
                var dsOP = new OrderProcessingDataSet { EnforceConstraints = false };

                using (var taProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                    taProcesses.FillBy(dsOP.OrderProcesses, this._order.OrderID);
                using (var taProcesses = new ProcessAliasTableAdapter())
                    taProcesses.Fill(dsOP.ProcessAlias, this._order.OrderID);
                using (var taProcesses = new ProcessTableAdapter())
                    taProcesses.FillByOrder(dsOP.Process, this._order.OrderID);

                IGroup group = _section.AddGroup();
                group.Layout = Layout.Horizontal;
                group.Margins = new Margins(5, 5, 3, 0);
                group.Paddings.All = 5;
                group.Borders = DefaultStyles.DefaultBorders;
                group.Background = DefaultStyles.DefaultBackground;

                ITable table = group.AddTable();

                //Add Header
                ITableRow disclaimerRow = table.AddRow();
                disclaimerRow.AddCell(100, "*For reference only. See DWOS for processing*", DefaultStyles.RedStyle, TextAlignment.Center, new HorizontalMargins(5, 5));
                ITableRow row = table.AddRow();
                ITableCell cell = row.AddCell();
                cell.AddText("Processes:", DefaultStyles.BlueLargeStyle, TextAlignment.Left);
                

                var stepStyle = new Style(new Font("Verdana", 9, FontStyle.Bold), Brushes.Black);
                var signStyle = new Style(new Font("Verdana", 9), Brushes.Black);

                if (_order.WorkStatus == "Pending Rework Planning")
                {
                    row = table.AddRow();
                    row.AddCell(100, "** Pending Rework Planning **", DefaultStyles.RedXLargeStyle, TextAlignment.Center, new HorizontalMargins(5, 5));
                    return;
                }

                if (_order.OrderType == 5 && _order.Hold) //Rework HOLD
                {
                    row = table.AddRow();
                    row.AddCell(100, "** Pending Rework Join **", DefaultStyles.RedXLargeStyle, TextAlignment.Center, new HorizontalMargins(5, 5));
                    return;
                }

                foreach (OrderProcessingDataSet.OrderProcessesRow orderProcess in dsOP.OrderProcesses)
                {

                    //if [SOMEADMINPROP = TRUE]
                    //if (!orderProcess.IsEndDateNull()) continue;

                    const int relativeMainWidth = 30;
                    const int relativeConfirmWidth = 70;
                    const int maxInlineNotesLength = 50;

                    var orderProcessAlias = orderProcess.ProcessAliasRow;

                    var processHeaderRow = table.AddRow();

                    var dueDateString = string.Empty;
                    if (ApplicationSettings.Current.IncludeProcessDateOnTraveler && !orderProcess.IsEstEndDateNull())
                    {
                        dueDateString = $"Due: {FormatDueDate(orderProcess.EstEndDate)} ";
                    }

                    processHeaderRow.AddCell(relativeMainWidth, orderProcess.StepOrder + " - " + orderProcess.Department, stepStyle, TextAlignment.Left, new HorizontalMargins(5, 2));

                    var processConfirmType = ApplicationSettings.Current.TravelerProcessConfirmation;

                    if (processConfirmType == TravelerProcessConfirmationType.QtyDateBy)
                    {
                        processHeaderRow.AddCell(relativeConfirmWidth, dueDateString + "Qty:______ Date:__________  By:__________ ", signStyle, TextAlignment.Right, new HorizontalMargins(2, 20));
                    }
                    else if (processConfirmType == TravelerProcessConfirmationType.CompletedCheckbox)
                    {
                        processHeaderRow.AddCell(10, string.Empty, signStyle, TextAlignment.Right);

                        const int rectangleDimension = 9;
                        var checkboxCell = processHeaderRow.AddCell();
                        checkboxCell.Width = new RelativeWidth(50);

                        var checkboxTable = checkboxCell.AddTable();
                        var innerRow = checkboxTable.AddRow();
                        var checkboxCanvas = innerRow.AddCell(AutoWidth.Instance).AddCanvas();
                        checkboxCanvas.Width = new FixedWidth(rectangleDimension + 3);
                        checkboxCanvas.Height = new FixedHeight(rectangleDimension + 2);
                        checkboxCanvas.Pen = new Pen(Colors.Black, 1f);
                        checkboxCanvas.DrawRectangle(1, 1, rectangleDimension, rectangleDimension, PaintMode.Stroke);

                        var checkboxText = innerRow.AddCell(AutoWidth.Instance, new HorizontalMargins(4f, 0f)).AddText();
                        checkboxText.Alignment = new TextAlignment(Alignment.Left, Alignment.Middle);
                        checkboxText.AddContent("Completed", signStyle);
                    }
                    else if (processConfirmType == TravelerProcessConfirmationType.TimeInTimeOut)
                    {
                        processHeaderRow.AddCell(relativeConfirmWidth, dueDateString + "Time In:__________ Time Out:__________", signStyle,
                            TextAlignment.Right, new HorizontalMargins(2, 20));
                    }

                    var textCell = table.AddRow().AddCell();
                    textCell.Width = new RelativeWidth(100);
                    textCell.Margins = new HorizontalMargins(10, 2);
                    IText txt = textCell.AddText();
                    txt.Alignment = TextAlignment.Left;

                    string processName = orderProcess.ProcessRow.Name;

                    var includeNotesInline = !orderProcessAlias.IsTravelerNotesNull()
                        && orderProcessAlias.TravelerNotes.Length <= maxInlineNotesLength
                        && !orderProcessAlias.TravelerNotes.Contains(Environment.NewLine);

                    if (includeNotesInline)
                    {
                        processName += " <b>[" + orderProcessAlias.TravelerNotes + "]</b>";
                    }

                    if (!orderProcessAlias.Name.Contains(orderProcess.ProcessRow.Name))
                        processName += " [" + orderProcessAlias.Name + "]";

                    txt.AddRichContent(SMALL_FONT_START + processName + FONT_END);

                    if (!orderProcessAlias.IsTravelerNotesNull() && !includeNotesInline)
                    {
                        var noteText = textCell.AddText();
                        noteText.Alignment = TextAlignment.Left;

                        var formattedTravelerNotes = orderProcessAlias
                            .TravelerNotes
                            .Replace(Environment.NewLine, "<br/>");

                        noteText.AddRichContent($"{SMALL_FONT_START}<b>{formattedTravelerNotes}</b>{FONT_END}");
                    }

                    AddProcessLoadCapacity(table, orderProcess);
                }

                //if this is an internal rework than add a join step to at the end
                if (_order.OrderType == (int)OrderType.ReworkInt)
                {
                    row = table.AddRow();
                    row.AddCell(100, (dsOP.OrderProcesses.Max(op => op.StepOrder) + 1).ToString() + " - Rework Join", stepStyle, TextAlignment.Left, new HorizontalMargins(5, 2));

                    using (var taIR = new Data.Datasets.OrdersDataSetTableAdapters.InternalReworkTableAdapter())
                    {
                        var internalReworkTable = new Data.Datasets.OrdersDataSet.InternalReworkDataTable();
                        taIR.FillByReworkOrderID(internalReworkTable, _order.OrderID);

                        var internalReworkRow = internalReworkTable.OrderBy(ir => Convert.ToInt32(ir.Active)).LastOrDefault();
                        if (internalReworkRow != null)
                        {
                            row = table.AddRow();
                            row.AddCell(100, "Join Order With Order " + internalReworkRow.OriginalOrderID, stepStyle, TextAlignment.Left, new HorizontalMargins(10, 2));
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Fatal(exc, "Error adding process section to WorkOrder Traveler report.");
            }
        }

        private void AddProcessLoadCapacity(ITable processTable, OrderProcessingDataSet.OrderProcessesRow orderProcess)
        {
            if (!ApplicationSettings.Current.UseLoadCapacity)
            {
                return;
            }

            var capacity = LoadCapacity.FromOrderProcess(_order, orderProcess) ??
                LoadCapacity.FromMatchingPartProcess(_order, orderProcess.OrderProcessesID) ??
                LoadCapacity.FromProcess(_order, orderProcess.ProcessID);

            if (capacity != null && capacity.Source != LoadCapacity.CapacitySource.OrderProcess)
            {
                _log.Info($"Retrieved load capacity data for order process {orderProcess.OrderProcessesID} from {capacity.Source}");
            }

            var fixtureCount = capacity?.FixtureCount;
            var weightPerFixture = capacity?.WeightPerFixture;

            if (fixtureCount.HasValue)
            {
                // Show count & weight
                var fixtureRow = processTable.AddRow();

                string fixtureWeightContent;
                if (weightPerFixture.HasValue)
                {
                    fixtureWeightContent = $"Weight per Fixture: {weightPerFixture.Value:F2} lbs.";
                }
                else
                {
                    fixtureWeightContent = "Weight per Fixture: Unknown";
                }

                var fixtureCountCell = fixtureRow.AddCell();
                fixtureCountCell.Width = new RelativeWidth(50);
                fixtureCountCell.Margins = new HorizontalMargins(10, 0);

                fixtureCountCell.AddText()
                    .AddRichContent(SMALL_FONT_START + $"Number of fixtures: {fixtureCount}" + FONT_END);

                var fixtureWeightCell = fixtureRow.AddCell();
                fixtureWeightCell.Width = new RelativeWidth(50);
                fixtureWeightCell.Margins = new HorizontalMargins(0, 2);

                fixtureWeightCell.AddText()
                    .AddRichContent(SMALL_FONT_START + fixtureWeightContent + FONT_END);

                fixtureRow.Margins = new VerticalMargins(0, 10);
            }
        }

        private void AddReworkSection()
        {
            try
            {
                var headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Margins = new Margins(5, 5, 3, 0);
                headerGroup.Borders = DefaultStyles.DefaultBorders;
                headerGroup.Background = DefaultStyles.DefaultBackground;

                var reworkContainer = headerGroup.AddContainer("rework");
                reworkContainer.Width = new RelativeWidth(45);
                reworkContainer.Paddings.All = 5;
                reworkContainer.Margins.Right = 5;

                var reworkTable = reworkContainer.AddTable();
                reworkTable.AddRow()
                    .AddCells(DefaultStyles.BlueLargeStyle,
                        null, 0, TextAlignment.Left,
                        "Rework:");

                foreach (var internalRework in GetInternalReworks(_order))
                {
                    var notes = internalRework.IsNotesNull() ? string.Empty : internalRework.Notes;

                    if (string.IsNullOrWhiteSpace(notes))
                    {
                        notes = "N/A";
                    }

                    var dateRow = reworkTable.AddRow();
                    dateRow.AddCells(DefaultStyles.NormalStyle, null, 0, TextAlignment.Left, "   Date:", internalRework.DateCreated.ToShortDateString());

                    var notesRow = reworkTable.AddRow();
                    notesRow.Margins = new VerticalMargins(0, 10);
                    notesRow.AddCells(DefaultStyles.NormalStyle, null, 0, TextAlignment.Left, "   Notes:", notes);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error creating rework section of WO Traveler.");
            }
        }

        /// <summary>
        ///     Adds the bar code commands to the bottom of the document.
        /// </summary>
        private void AddBarCodeCommands()
        {
            try
            {
                if (_footer == null)
                {
                    _log.Warn("Could not find footer");
                    return;
                }

                IGroup group = _footer.AddGroup(5, 0);
                group.Layout = Layout.Horizontal;
                group.Margins.All = 5;
                group.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);

                ITable table = group.AddTable();
                ITableRow row = table.AddRow();

                //Add Order CheckIn Barcode
                ITableCell cell = row.AddCell();
                cell.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);
                IGroup checkInGroup = cell.AddGroup();
                checkInGroup.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);

                IText checkInText = checkInGroup.AddText();
                checkInText.AddContent("Order Check In", DefaultStyles.NormalStyle);
                checkInText.Alignment = new TextAlignment(Alignment.Center, Alignment.Bottom);

                IImage imageOCIBarCode = checkInGroup.AddImage(new Image(CreateOrderCheckInBarcode(this._order.OrderID)));
                imageOCIBarCode.KeepRatio = true;
                imageOCIBarCode.Margins.All = 5;

                //Add Shipping Barcode
                cell = row.AddCell();
                cell.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);
                IGroup shipGroup = cell.AddGroup();
                shipGroup.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);

                IText shipText = shipGroup.AddText();
                shipText.AddContent("Shipping/Batch Order Processing", DefaultStyles.NormalStyle);
                shipText.Alignment = new TextAlignment(Alignment.Center, Alignment.Bottom);

                IImage imageShipBarCode = shipGroup.AddImage(new Image(CreateOrderActionBarcode(this._order.OrderID)));
                imageShipBarCode.KeepRatio = true;
                imageShipBarCode.Margins.All = 5;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding barcode section to Work Order Traveler report.");
            }
        }

        private static string FormatDueDate(DateTime dueDate)
        {
            if (ApplicationSettings.Current.SchedulerType == SchedulerType.ProcessLeadTimeHour)
            {
                return dueDate.ToString("MM/dd/yy hh:mm tt");
            }

            return dueDate.ToString("MM / dd / yy");
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            this._order = null;
            // base.Dispose();
        }

        #endregion
    }

    public class SalesOrderTravelerReport : Report
    {
        #region Fields

        private const string SMALL_FONT_START = "<font face=\"Verdana\" size=\"9\">";
        private const string FONT_END = "</font>";

        #endregion

        #region Properties

        public override string Title =>
            "Sales Order Traveler";

        protected override PageOrientation ReportPageOrientation =>
            PageOrientation.Portrait;

        protected override string FooterIdentifier =>
            SalesOrder.SalesOrderID.ToString();

        protected override int FilenameIdentifier =>
            SalesOrder.SalesOrderID;

        // This report puts barcodes at the top of the footer.
        protected override float AdditionalFooterSpace => 68;

        public OrdersDataSet.SalesOrderRow SalesOrder { get; }

        #endregion

        #region Methods

        public SalesOrderTravelerReport(OrdersDataSet.SalesOrderRow salesOrder)
        {
            SalesOrder = salesOrder
                ?? throw new ArgumentNullException(nameof(salesOrder));
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            SetupReportInfo();

            AddHeader("SALES ORDER TRAVELER",
                "Sales Order: ",
                SalesOrder.SalesOrderID,
                false,
                null,
                ReportType.BatchOrder);

            AddTopSection();
            AddOrdersSection();
            AddProcessesSection();
            AddBarcodeFooter();
        }

        private void AddTopSection()
        {
            const int containerHeight = 110;

            try
            {
                var headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Background = DefaultStyles.DefaultBackground;
                headerGroup.Margins = new Margins(5, 5, 3, 0);

                var customerContainer = headerGroup.AddContainer("customer");
                customerContainer.Width = new RelativeWidth(33);
                customerContainer.Borders = DefaultStyles.DefaultBorders;
                customerContainer.Paddings.All = 4;
                customerContainer.Margins.Right = 5;
                customerContainer.Background = DefaultStyles.DefaultBackground;
                customerContainer.Height = new FixedHeight(containerHeight);

                AddCustomerTable(customerContainer);

                var parentContainer = headerGroup.AddContainer("parent");
                parentContainer.Width = new FixedWidth(373);
                parentContainer.Alignment.Vertical = Alignment.Middle;
                parentContainer.Alignment.Horizontal = Alignment.Left;
                var parentTable = parentContainer.AddTable();
                var prow1 = parentTable.AddRow();

                prow1.Height = new FixedHeight(containerHeight);
                var pcell1 = prow1.AddCell();
                pcell1.Alignment.Horizontal = Alignment.Left;
                pcell1.Alignment.Vertical = Alignment.Middle;
                pcell1.Width = new FixedWidth(190);
                var orderContainer = pcell1.AddContainer("order");
                var pcell2 = prow1.AddCell();
                pcell2.Alignment.Horizontal = Alignment.Left;
                pcell2.Alignment.Vertical = Alignment.Middle;
                var reqDateContainer = pcell2.AddContainer("reqDate");

                orderContainer.Alignment.Vertical = Alignment.Top;
                orderContainer.Alignment.Horizontal = Alignment.Left;
                orderContainer.Paddings.Left = 5;
                orderContainer.Paddings.Top = 4;
                orderContainer.Borders = DefaultStyles.DefaultBorders;
                orderContainer.Height = new FixedHeight(containerHeight);

                reqDateContainer.Paddings.Left = 5;
                reqDateContainer.Paddings.Top = 4;
                reqDateContainer.Height = new FixedHeight(containerHeight);
                reqDateContainer.Alignment.Vertical = Alignment.Top;
                reqDateContainer.Alignment.Horizontal = Alignment.Left;
                reqDateContainer.Borders = DefaultStyles.DefaultBorders;
                reqDateContainer.Margins.Left = 5;

                AddRequiredDateTable(reqDateContainer);

                AddSalesOrderTable(orderContainer);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding customer section to report.");
            }
        }

        private void AddCustomerTable(Infragistics.Documents.Reports.Report.IContainer companyContainer)
        {
            const int maxLength = 35;
            const int maxCustomerLength = 30;
            const string ellipsis = "…";

            CustomersDataset dsCustomer = null;

            try
            {
                dsCustomer = new CustomersDataset()
                {
                    EnforceConstraints = false
                };

                using (var taCustomer = new CustomerTableAdapter())
                {
                    taCustomer.FillBy(dsCustomer.Customer, SalesOrder.CustomerID);
                }

                var customer = dsCustomer.Customer.FirstOrDefault();

                using (var taCustomerAddress = new Data.Datasets.CustomersDatasetTableAdapters.CustomerAddressTableAdapter { ClearBeforeFill = false })
                {
                    foreach (var order in SalesOrder.GetOrderRows())
                    {
                        if (order.IsCustomerAddressIDNull())
                        {
                            continue;
                        }

                        taCustomerAddress.FillByCustomerAddress(dsCustomer.CustomerAddress, order.CustomerAddressID);
                    }
                }

                using (var taCountry = new CountryTableAdapter())
                {
                    taCountry.Fill(dsCustomer.Country);
                }

                var companyTable = companyContainer.AddTable();
                companyTable.AddRow().AddCell(100, "Customer:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                companyTable.AddRow().AddCell(100, "   " + customer.Name.TrimToMaxLength(maxCustomerLength, ellipsis),
                    DefaultStyles.BoldMediumStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                if (customer.HasBillingAddress)
                {
                    if (!customer.IsAddress1Null() && customer.Address1 != customer.Name)
                    {
                        ITableRow companyAddress = companyTable.AddRow();
                        companyAddress.AddCell(50, "   " + customer.Address1.TrimToMaxLength(maxLength, ellipsis),
                            DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                    }

                    ITableRow addressRow = companyTable.AddRow();
                    string zip = customer.IsZipNull() ? "" : customer.Zip;
                    var addressLastLine = (customer.IsCityNull() ? "-" : customer.City) + " " + (customer.IsStateNull() ? "-" : customer.State) + ", " + zip;

                    if (customer.CountryID != ApplicationSettings.Current.CompanyCountry)
                    {
                        addressLastLine += $" {customer.CountryRow.Name}";
                    }

                    addressRow.AddCell(50, "   " + addressLastLine.TrimToMaxLength(maxLength, ellipsis),
                        DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                }

                var customerAddresses = customer.GetCustomerAddressRows();
                if (customerAddresses.Length > 1)
                {
                    companyTable.AddRow().AddCell(100, "Ship To:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                    companyTable.AddRow().AddCell(50, "   Multiple Addresses",
                        DefaultStyles.NormalStyle,
                        new TextAlignment(Alignment.Left, Alignment.Middle));
                }
                else if (customerAddresses.Length == 1)
                {
                    var customerAddress = customerAddresses.FirstOrDefault();

                    companyTable.AddRow().AddCell(100, "Ship To:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                    companyTable.AddRow().AddCell(50, "   " + customerAddress.Name.TrimToMaxLength(maxLength, ellipsis),
                        DefaultStyles.NormalStyle,
                        new TextAlignment(Alignment.Left, Alignment.Middle));


                    if (!customerAddress.IsAddress1Null() && customerAddress.Address1 != customer.Name)
                    {
                        companyTable.AddRow().AddCell(50, "   " + customerAddress.Address1.TrimToMaxLength(maxLength, ellipsis),
                            DefaultStyles.NormalStyle,
                            new TextAlignment(Alignment.Left, Alignment.Middle));
                    }

                    if (!customerAddress.IsAddress2Null() && !string.IsNullOrEmpty(customerAddress.Address2))
                    {
                        companyTable.AddRow().AddCell(50, "   " + customerAddress.Address2.TrimToMaxLength(maxLength, ellipsis),
                            DefaultStyles.NormalStyle,
                            new TextAlignment(Alignment.Left, Alignment.Middle));
                    }

                    var shippingAddressRow = companyTable.AddRow();
                    var shipCity = customerAddress.IsCityNull() ? "-" : customerAddress.City;
                    var shipZip = customerAddress.IsZipNull() ? "" : customerAddress.Zip;
                    var shipState = customerAddress.IsStateNull() ? "-" : customerAddress.State;
                    var shipLastLine = shipCity + " " + shipState + ", " + shipZip;

                    if (customerAddress.CountryID != ApplicationSettings.Current.CompanyCountry)
                    {
                        shipLastLine += $" {customerAddress.CountryRow.Name}";
                    }

                    shippingAddressRow.AddCell(50, "   " + shipLastLine.TrimToMaxLength(maxLength, ellipsis),
                        DefaultStyles.NormalStyle,
                        new TextAlignment(Alignment.Left, Alignment.Middle));
                }
            }
            finally
            {
                dsCustomer?.Dispose();
            }
        }

        private void AddRequiredDateTable(Infragistics.Documents.Reports.Report.IContainer reqDateContainer)
        {
            ITable reqDateTable = reqDateContainer.AddTable();
            ITableRow row5 = reqDateTable.AddRow();
            row5.Height = new FixedHeight(20);

            ITableCell cell1 = row5.AddCell(40, "Est. Ship Date:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            cell1.Height = new FixedHeight(20);
            ITableRow row6 = reqDateTable.AddRow();
            row6.Height = new FixedHeight(20);
            ITableCell cell2 = row6.AddCell(40,
                SalesOrder.IsEstShipDateNull() ? "NA" : SalesOrder.EstShipDate.ToShortDateString(),
                new Style(new Font(DefaultStyles.RedXLargeStyle.Font.Name, 20, FontStyle.Bold), DefaultStyles.BlackXLargeStyle.Brush),
                new TextAlignment(Alignment.Center, Alignment.Middle));

            cell2.Height = new FixedHeight(20);
        }

        private void AddSalesOrderTable(Infragistics.Documents.Reports.Report.IContainer orderContainer)
        {
            const int relativeFieldWidth = 15;
            const int maxValueLength = 17;

            ITable orderTable = orderContainer.AddTable();
            ITableCell orderCell = orderTable.AddRow().AddCell(100, "Sales Order:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            ITableRow row = orderTable.AddRow();
            ITableCell cell3 = row.AddCell(relativeFieldWidth, "   Purchase Order:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            cell3.Margins = new HorizontalMargins(0, 0);
            row.AddCell(relativeFieldWidth, this.SalesOrder.IsPurchaseOrderNull() ? "NA" : SalesOrder.PurchaseOrder.TrimToMaxLength(maxValueLength, "."), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

            ITableRow row1 = orderTable.AddRow();
            ITableCell cell4 = row1.AddCell(relativeFieldWidth, "   Customer WO:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            cell4.Margins = new HorizontalMargins(0, 0);
            row1.AddCell(relativeFieldWidth, SalesOrder.IsCustomerWONull() ? "NA" : this.SalesOrder.CustomerWO.TrimToMaxLength(maxValueLength, "."), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

            ITableRow row2 = orderTable.AddRow();
            ITableCell cell5 = row2.AddCell(relativeFieldWidth, "   Received Date:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            cell5.Margins = new HorizontalMargins(0, 0);
            row2.AddCell(relativeFieldWidth, SalesOrder.OrderDate.ToShortDateString(), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

            // Shipping method
            var shippingMethodRow = orderTable.AddRow();
            var shippingMethodLabelCell = shippingMethodRow.AddCell(relativeFieldWidth, "   Shipping Method:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            shippingMethodLabelCell.Margins = new HorizontalMargins(0, 0);
            shippingMethodRow.AddCell(relativeFieldWidth, GetCustomerShippingName(SalesOrder), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
        }

        private static string GetCustomerShippingName(OrdersDataSet.SalesOrderRow order)
        {
            const string defaultCustomerShippingName = "None";

            var distinctShippingMethods = order.GetOrderRows()
                .Where(wo => !wo.IsShippingMethodNull())
                .Select(wo => wo.ShippingMethod)
                .Distinct()
                .ToList();

            if (distinctShippingMethods.Count == 0)
            {
                return defaultCustomerShippingName;
            }

            if (distinctShippingMethods.Count > 1)
            {
                return "Multiple";
            }

            var firstOrderRow = order.GetOrderRows().First();
            var shippingSummaryRow = firstOrderRow
                .CustomerShippingSummaryRow;

            if (shippingSummaryRow != null)
            {
                return shippingSummaryRow.Name;
            }

            // Retrieve customer shipping from database
            using (var dsCustomerShipping = new OrdersDataSet.CustomerShippingSummaryDataTable())
            {
                using (var taCustomerShipping = new CustomerShippingSummaryTableAdapter())
                {
                    taCustomerShipping.FillByOrder(dsCustomerShipping, firstOrderRow.OrderID);
                }

                return dsCustomerShipping.FirstOrDefault()?.Name
                    ?? defaultCustomerShippingName;
            }
        }

        private void AddOrdersSection()
        {
            var appSettings = ApplicationSettings.Current;
            var dtMedia = new OrderProcessingDataSet.MediaDataTable();
            var dtPart = new OrderProcessingDataSet.PartDataTable();
            var taMedia = new Data.Datasets.OrderProcessingDataSetTableAdapters.MediaTableAdapter();
            var taParts = new PartTableAdapter();
            const int noteMaxLength = 255;
            const int infoWidth = 75;
            const int imageWidth = 25;

            try
            {
                bool isMaterialVisible;
                bool isSurfaceAreaVisible;
                bool isManufacturerVisible;
                bool isRevisionVisible;
                bool isSerialNumberVisible;

                using (var taField = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
                {
                    using (var dtPartFields = taField.GetByCategory("Part"))
                    {
                        // Material
                        var materialField = dtPartFields.FirstOrDefault(field => field.Name == "Material");
                        isMaterialVisible = materialField != null && materialField.IsVisible;

                        // Surface Area
                        var surfaceAreaField = dtPartFields.FirstOrDefault(field => field.Name == "Surface Area");
                        isSurfaceAreaVisible = surfaceAreaField != null && surfaceAreaField.IsVisible;

                        // Manufacturer
                        var manufacturerField = dtPartFields.FirstOrDefault(field => field.Name == "Manufacturer");
                        isManufacturerVisible = manufacturerField != null && manufacturerField.IsVisible;

                        // Revision
                        var revisionField = dtPartFields.FirstOrDefault(field => field.Name == "Part Rev.");
                        isRevisionVisible = revisionField != null && revisionField.IsVisible;
                    }

                    using (var dtPartFields = taField.GetByCategory("Order"))
                    {
                        // Serial number
                        var serialNumberField = dtPartFields.FirstOrDefault(field => field.Name == "Serial Number");
                        isSerialNumberVisible = serialNumberField != null && serialNumberField.IsVisible;
                    }
                }

                IGroup headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Margins = new Margins(5, 5, 3, 0);
                headerGroup.Borders = DefaultStyles.DefaultBorders;
                headerGroup.Background = DefaultStyles.DefaultBackground;

                Infragistics.Documents.Reports.Report.IContainer salesOrderContainer = headerGroup.AddContainer("Orders");
                salesOrderContainer.Width = new RelativeWidth(100);
                salesOrderContainer.Paddings.All = 5;
                salesOrderContainer.Margins.Right = 5;

                var partInfoHeader = salesOrderContainer.AddText();
                partInfoHeader.Background = new Background(new SolidColorBrush(new Color(System.Drawing.Color.WhiteSmoke)));
                partInfoHeader.AddContent("Part Information:", DefaultStyles.BlueLargeStyle);
                partInfoHeader.Margins = new Margins(0, 0, 0, 5);

                var isFirstOrder = true;
                foreach (var order in SalesOrder.GetOrderRows().OrderBy(wo => wo.OrderID))
                {
                    if (!isFirstOrder)
                    {
                        var horizontalRule = salesOrderContainer.AddRule();
                        horizontalRule.Margins = new VerticalMargins(5);
                    }

                    var mainTable = salesOrderContainer.AddTable();
                    var partRow = mainTable.AddRow();
                    var partInfoCell = partRow.AddCell();
                    partInfoCell.Width = new RelativeWidth(infoWidth);

                    var partInfoTable = partInfoCell.AddTable();

                    // Check if partID was valid (user can clear/delete part field in OrderEntry then click print which causes problem)
                    if (!order.IsPartIDNull())
                    {
                        taParts.FillBy(dtPart, order.PartID);
                    }

                    // Add Order Info
                    partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Work Order:", order.OrderID.ToString());

                    //Add Part infoPart Notes:
                    var partName = (dtPart.Count > 0 ? dtPart[0].Name : "Unknown");

                    if (isRevisionVisible)
                    {
                        var revision = dtPart.Count > 0 && !dtPart[0].IsRevisionNull() ? dtPart[0].Revision : string.Empty;

                        if (!string.IsNullOrEmpty(revision))
                        {
                            partName += $" Rev. {revision}";
                        }
                    }

                    partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Part Number:", partName);
                    partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Description:", (dtPart.Count > 0 && !dtPart[0].IsDescriptionNull() ? dtPart[0].Description : "Unknown"));

                    if (isMaterialVisible)
                    {
                        partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Material:", (dtPart.Count > 0 && !dtPart[0].IsMaterialNull() ? dtPart[0].Material : "Unknown"));
                    }

                    if (isSurfaceAreaVisible)
                    {
                        partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Dimensions:", (dtPart.Count > 0 ? PartAreaUtilities.PartDimensionString(dtPart[0], false) : "None"));
                    }

                    partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Quantity:", (order.IsPartQuantityNull() ? "Unknown" : order.PartQuantity.ToString()));

                    var weightFormat = $"0.{"0".Repeat(appSettings.WeightDecimalPlaces)}";
                    partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle),
                        "   Total Weight:",
                        order.IsWeightNull() ? "Unknown" : $"{order.Weight.ToString(weightFormat)} lbs.");


                    if (isManufacturerVisible)
                    {
                        partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Manufacturer:", (dtPart.Count > 0 && !dtPart[0].IsManufacturerIDNull() ? dtPart[0].ManufacturerID : "None"));
                        partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Model:", (dtPart.Count > 0 && !dtPart[0].IsAirframeNull() ? dtPart[0].Airframe : "None"));
                    }

                    //Add Custom Fields to traveler
                    using (var taOrderCustomField = new Data.Datasets.OrdersDataSetTableAdapters.OrderCustomFieldsTableAdapter())
                    {
                        foreach (var orderCustomField in taOrderCustomField.GetWOTravelerValues(order.CustomerID, order.OrderID))
                        {
                            if (orderCustomField.IsValueNull() || string.IsNullOrWhiteSpace(orderCustomField.Value))
                            {
                                continue;
                            }

                            var fieldName = $"   {orderCustomField["Name"]}:";
                            var fieldValue = orderCustomField.Value;

                            partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0,
                                new TextAlignment(Alignment.Left, Alignment.Middle), fieldName, fieldValue);
                        }
                    }

                    // Add part-level custom fields.
                    if (!order.IsPartIDNull())
                    {
                        using (var dsParts = new PartsDataset { EnforceConstraints = false })
                        {
                            using (var taPartLevelCustomField = new Data.Datasets.PartsDatasetTableAdapters.PartLevelCustomFieldTableAdapter())
                            {
                                taPartLevelCustomField.FillByPartID(dsParts.PartLevelCustomField, order.PartID);
                            }

                            using (var taPartCustomFields = new Data.Datasets.PartsDatasetTableAdapters.PartCustomFieldsTableAdapter())
                            {
                                taPartCustomFields.FillByPartID(dsParts.PartCustomFields, order.PartID);
                            }

                            foreach (var customField in dsParts.PartCustomFields)
                            {
                                if (customField.PartLevelCustomFieldRow == null || !customField.PartLevelCustomFieldRow.DisplayOnTraveler)
                                {
                                    continue;
                                }

                                var fieldName = $"   {customField.PartLevelCustomFieldRow.Name}:";
                                var fieldValue = customField.IsValueNull() ? string.Empty : customField.Value;

                                partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0,
                                    new TextAlignment(Alignment.Left, Alignment.Middle), fieldName, fieldValue);
                            }
                        }
                    }


                    if (dtPart.Count > 0 && !dtPart[0].IsNotesNull())
                    {
                        partInfoTable.AddRow().AddCells(DefaultStyles.BlueBoldStyle, new SolidColorBrush(new Color(System.Drawing.Color.WhiteSmoke)), null, 0, TextAlignment.Left, "Part Notes:");
                        partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), dtPart[0].Notes.TrimToMaxLength(noteMaxLength, "..."))[0].Paddings.Left = 5;
                    }

                    // Add the order notes
                    if (order.GetOrderNoteRows().Length > 0)
                    {
                        var externalNotes = order.GetOrderNoteRows().Where(o => o.NoteType == "External");

                        var noteSummary = new StringBuilder();
                        foreach (OrdersDataSet.OrderNoteRow externalNote in externalNotes)
                            noteSummary.AppendLine("   " + externalNote.Notes.TrimToMaxLength(noteMaxLength, "..."));

                        if (order.GetOrderNoteRows().Any(o => o.NoteType == "Internal"))
                        {
                            noteSummary.AppendLine("See INTERNAL notes.");
                        }

                        partInfoTable.AddRow().AddCells(DefaultStyles.BlueBoldStyle, new SolidColorBrush(new Color(System.Drawing.Color.WhiteSmoke)), null, 0, TextAlignment.Left, "Order Notes:");
                        partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), noteSummary.ToString())[0].Paddings.Left = 5;
                    }

                    // Serial numbers
                    var serialNumbers = order.GetOrderSerialNumberRows()
                        .Where(serialNumber => serialNumber.Active && !serialNumber.IsNumberNull())
                        .OrderBy(serialNumber => serialNumber.PartOrder)
                        .Select(serialNumber => serialNumber.Number)
                        .ToList();

                    if (isSerialNumberVisible && serialNumbers.Count > 0)
                    {
                        var serialNumberString = string.Join(", ", serialNumbers);
                        partInfoTable.AddRow().AddCells(DefaultStyles.BlueBoldStyle, new SolidColorBrush(new Color(System.Drawing.Color.WhiteSmoke)), null, 0, TextAlignment.Left, "Serial Numbers:");
                        partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), serialNumberString)[0].Paddings.Left = 5;
                    }

                    // Rework information
                    var internalReworks = order.GetInternalReworkRows();
                    if (internalReworks.Length > 0)
                    {
                        partInfoTable.AddRow().AddCells(DefaultStyles.BlueBoldStyle, new SolidColorBrush(new Color(System.Drawing.Color.WhiteSmoke)), null, 0, TextAlignment.Left, "Rework:");
                        foreach (var internalRework in internalReworks)
                        {
                            var notes = internalRework.IsNotesNull() ? string.Empty : internalRework.Notes;

                            if (string.IsNullOrWhiteSpace(notes))
                            {
                                notes = "N/A";
                            }

                            var dateRow = partInfoTable.AddRow();
                            dateRow.AddCells(DefaultStyles.NormalStyle, null, 0, TextAlignment.Left, "   Date:", internalRework.DateCreated.ToShortDateString());

                            var notesRow = partInfoTable.AddRow();
                            notesRow.Margins = new VerticalMargins(0, 10);
                            notesRow.AddCells(DefaultStyles.NormalStyle, null, 0, TextAlignment.Left, "   Notes:", notes);
                        }
                    }

                    //Add Part Image
                    var imgCell = partRow.AddCell();
                    imgCell.Width = new RelativeWidth(imageWidth);
                    imgCell.Margins = new HorizontalMargins(5, 0);

                    // Check if partID was valid (user can clear/delete part field in OrderEntry then click print which causes problem)
                    if (!order.IsPartIDNull())
                    {
                        taMedia.FillWithoutMedia(dtMedia, order.PartID);
                    }

                    System.Drawing.Image img;

                    if (dtMedia.Count > 0)
                    {
                        img = MediaUtilities.GetImage(dtMedia[0].MediaID, dtMedia[0].FileExtension);
                    }
                    else if (ApplicationSettings.Current.UseReportPlaceholderImage)
                    {
                        if (File.Exists(ApplicationSettings.Current.ReportPlaceholderImagePath))
                        {
                            img = MediaUtilities.GetImage(File.ReadAllBytes(ApplicationSettings.Current.ReportPlaceholderImagePath));
                        }
                        else
                        {
                            img = Data.Properties.Resources.NoImage;
                        }
                    }
                    else
                    {
                        img = null;
                    }

                    if (img != null)
                    {
                        IImage image = imgCell.AddImage(new Image(img));
                        image.KeepRatio = true;

                        var maximumSize = new System.Drawing.Size(150, 100);
                        var imageSize = MediaUtilities.Resize(img.Size, maximumSize);
                        image.Width = new FixedWidth(imageSize.Width);
                        image.Height = new FixedHeight(imageSize.Height);
                    }

                    //Add Part Marking paragraph
                    using (var taOPM = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderPartMarkTableAdapter())
                    {
                        var orderPMTable = new OrderProcessingDataSet.OrderPartMarkDataTable();
                        taOPM.Fill(orderPMTable, order.OrderID);

                        var orderPMRow = orderPMTable.FirstOrDefault();

                        //if there is a order template then use that
                        if (orderPMRow != null)
                        {
                            // Check if partID was valid (user can clear/delete part field in OrderEntry then click print which causes problem)
                            if (dtPart.Rows.Count > 0 && !order.IsPartIDNull())
                            {
                                using (var taPart = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
                                {
                                    var partDT = new PartsDataset.PartDataTable();

                                    taPart.FillByPartID(partDT, dtPart[0].PartID);

                                    var pmCon = salesOrderContainer.AddContainer($"PM-{order.OrderID}");
                                    pmCon.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);

                                    ITable partMarkingTable = pmCon.AddTable();

                                    partMarkingTable.AddRow().AddCells(DefaultStyles.BlueBoldStyle, new SolidColorBrush(new Color(System.Drawing.Color.WhiteSmoke)), null, 0, TextAlignment.Left, "Part Marking:");
                                    if (!orderPMRow.IsLine1Null())
                                        partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(orderPMRow.Line1, partDT[0], order));
                                    if (!orderPMRow.IsLine2Null())
                                        partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(orderPMRow.Line2, partDT[0], order));
                                    if (!orderPMRow.IsLine3Null())
                                        partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(orderPMRow.Line3, partDT[0], order));
                                    if (!orderPMRow.IsLine4Null())
                                        partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(orderPMRow.Line4, partDT[0], order));
                                }
                            }
                        }
                        else if (dtPart[0].PartMarking && !dtPart[0].IsAirframeNull()) //else use the parts PM template, if exists
                        {
                            // Check if partID was valid (user can clear/delete part field in OrderEntry then click print which causes problem)
                            if (dtPart.Rows.Count > 0 && !order.IsPartIDNull())
                            {
                                using (var taPM = new Data.Datasets.OrderProcessingDataSetTableAdapters.PartMarkingTableAdapter())
                                {
                                    var pmDT = taPM.GetDataByPart(dtPart[0].PartID);

                                    if (pmDT.Count > 0)
                                    {
                                        using (var taPart = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
                                        {
                                            var partDT = new PartsDataset.PartDataTable();
                                            taPart.FillByPartID(partDT, dtPart[0].PartID);

                                            OrderProcessingDataSet.PartMarkingRow pmRow = pmDT[0];

                                            var pmCon = salesOrderContainer.AddContainer($"PM-{order.OrderID}");
                                            pmCon.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);

                                            ITable partMarkingTable = pmCon.AddTable();

                                            partMarkingTable.AddRow().AddCells(DefaultStyles.BlueBoldStyle, new SolidColorBrush(new Color(System.Drawing.Color.WhiteSmoke)), null, 0, TextAlignment.Left, "Part Marking:");
                                            if (!pmRow.IsDef1Null())
                                                partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(pmRow.Def1, partDT[0], order));
                                            if (!pmRow.IsDef2Null())
                                                partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(pmRow.Def2, partDT[0], order));
                                            if (!pmRow.IsDef3Null())
                                                partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(pmRow.Def3, partDT[0], order));
                                            if (!pmRow.IsDef4Null())
                                                partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(pmRow.Def4, partDT[0], order));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    isFirstOrder = false;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding part section to report.");
            }
            finally
            {
                dtMedia?.Dispose();
                dtPart?.Dispose();
                dtMedia?.Dispose();
                taParts?.Dispose();
            }
        }

        private void AddProcessesSection()
        {
            const int relativeMainWidth = 30;
            const int relativeConfirmWidth = 70;

            try
            {
                IGroup group = _section.AddGroup();
                group.Layout = Layout.Vertical;
                group.Margins = new Margins(5, 5, 3, 0);
                group.Paddings.All = 5;
                group.Borders = DefaultStyles.DefaultBorders;
                group.Background = DefaultStyles.DefaultBackground;


                var stepStyle = new Style(new Font("Verdana", 9, FontStyle.Bold), Brushes.Black);
                var signStyle = new Style(new Font("Verdana", 9), Brushes.Black);

                var orderProcessesInBatch = new HashSet<int>();

                foreach (var batch in SalesOrder.GetBatchRows())
                {
                    ITable table = group.AddTable();

                    //Add Header
                    ITableRow row = table.AddRow();
                    ITableCell cell = row.AddCell();
                    cell.AddText($"Processes - Batch {batch.BatchID}:", DefaultStyles.BlueLargeStyle, TextAlignment.Left);

                    foreach (var batchProcess in batch.GetBatchProcessesRows().OrderBy(proc => proc.StepOrder))
                    {
                        foreach (var relationRow in batchProcess.GetBatchProcess_OrderProcessRows())
                        {
                            orderProcessesInBatch.Add(relationRow.OrderProcessID);
                        }

                        var processHeaderRow = table.AddRow();
                        processHeaderRow.AddCell(relativeMainWidth, batchProcess.StepOrder + " - " + batchProcess.Department, stepStyle, TextAlignment.Left, new HorizontalMargins(5, 2));

                        var processConfirmType = ApplicationSettings.Current.TravelerProcessConfirmation;

                        var dueDateString = string.Empty;

                        if (ApplicationSettings.Current.IncludeProcessDateOnTraveler)
                        {
                            var orderProcesses = batchProcess.GetBatchProcess_OrderProcessRows()
                                .Select(relationRow => relationRow.OrderProcessesRow)
                                .ToList();

                            if (orderProcesses.Count > 0 && orderProcesses.All(op => !op.IsEstEndDateNull()))
                            {
                                var dueDate = orderProcesses.Max(op => op.EstEndDate);
                                dueDateString = $"Due: {FormatDueDate(dueDate)} ";
                            }
                        }

                        if (processConfirmType == TravelerProcessConfirmationType.QtyDateBy)
                        {
                            processHeaderRow.AddCell(relativeConfirmWidth, dueDateString + "Qty:______ Date:__________  By:__________ ", signStyle, TextAlignment.Right, new HorizontalMargins(2, 20));
                        }
                        else if (processConfirmType == TravelerProcessConfirmationType.CompletedCheckbox)
                        {
                            processHeaderRow.AddCell(10, string.Empty, signStyle, TextAlignment.Right);

                            const int rectangleDimension = 9;
                            var checkboxCell = processHeaderRow.AddCell();
                            checkboxCell.Width = new RelativeWidth(relativeConfirmWidth - 10);

                            var checkboxTable = checkboxCell.AddTable();
                            var innerRow = checkboxTable.AddRow();

                            var checkboxCanvas = innerRow.AddCell(AutoWidth.Instance).AddCanvas();
                            checkboxCanvas.Width = new FixedWidth(rectangleDimension + 3);
                            checkboxCanvas.Height = new FixedHeight(rectangleDimension + 2);
                            checkboxCanvas.Pen = new Pen(Colors.Black, 1f);
                            checkboxCanvas.DrawRectangle(1, 1, rectangleDimension, rectangleDimension, PaintMode.Stroke);

                            var checkboxText = innerRow.AddCell(AutoWidth.Instance, new HorizontalMargins(4f, 0f)).AddText();
                            checkboxText.Alignment = new TextAlignment(Alignment.Left, Alignment.Middle);
                            checkboxText.AddContent("Completed", signStyle);
                        }
                        else if (processConfirmType == TravelerProcessConfirmationType.TimeInTimeOut)
                        {
                            processHeaderRow.AddCell(relativeConfirmWidth, dueDateString + "Time In:__________ Time Out:__________", signStyle,
                                TextAlignment.Right, new HorizontalMargins(2, 20));
                        }

                        var textCell = table.AddRow().AddCell();
                        textCell.Width = new RelativeWidth(100);
                        textCell.Margins = new HorizontalMargins(10, 2);
                        IText txt = textCell.AddText();
                        txt.Alignment = TextAlignment.Left;

                        txt.AddRichContent(SMALL_FONT_START + GetProcessName(batchProcess.ProcessID) + FONT_END);
                    }
                }

                // Section for each order that has processes outside of a batch
                foreach (var order in SalesOrder.GetOrderRows().OrderBy(wo => wo.OrderID))
                {
                    var isOrderRework = order.OrderType == (int)OrderType.ReworkInt;

                    var processesOutsideOfBatch = order.GetOrderProcessesRows()
                        .Where(proc => !orderProcessesInBatch.Contains(proc.OrderProcessesID))
                        .OrderBy(proc => proc.StepOrder)
                        .ToList();

                    if (processesOutsideOfBatch.Count == 0 && !isOrderRework)
                    {
                        continue;
                    }

                    ITable table = group.AddTable();

                    //Add Header
                    ITableRow headerRow = table.AddRow();
                    ITableCell cell = headerRow.AddCell();
                    cell.AddText($"Additional Processes - Order {order.OrderID}:", DefaultStyles.BlueLargeStyle, TextAlignment.Left);

                    foreach (var orderProcess in processesOutsideOfBatch)
                    {
                        var processHeaderRow = table.AddRow();
                        processHeaderRow.AddCell(relativeMainWidth, orderProcess.StepOrder + " - " + orderProcess.Department, stepStyle, TextAlignment.Left, new HorizontalMargins(5, 2));

                        var processConfirmType = ApplicationSettings.Current.TravelerProcessConfirmation;

                        var dueDateString = string.Empty;

                        if (ApplicationSettings.Current.IncludeProcessDateOnTraveler && !orderProcess.IsEstEndDateNull())
                        {
                            dueDateString = $"Due: {FormatDueDate(orderProcess.EstEndDate)} ";
                        }

                        if (processConfirmType == TravelerProcessConfirmationType.QtyDateBy)
                        {
                            processHeaderRow.AddCell(relativeConfirmWidth, dueDateString + "Qty:______ Date:__________  By:__________ ", signStyle, TextAlignment.Right, new HorizontalMargins(2, 20));
                        }
                        else if (processConfirmType == TravelerProcessConfirmationType.CompletedCheckbox)
                        {
                            processHeaderRow.AddCell(10, string.Empty, signStyle, TextAlignment.Right);

                            const int rectangleDimension = 9;
                            var checkboxCell = processHeaderRow.AddCell();
                            checkboxCell.Width = new RelativeWidth(relativeConfirmWidth - 10);

                            var checkboxTable = checkboxCell.AddTable();
                            var innerRow = checkboxTable.AddRow();

                            var checkboxCanvas = innerRow.AddCell(AutoWidth.Instance).AddCanvas();
                            checkboxCanvas.Width = new FixedWidth(rectangleDimension + 3);
                            checkboxCanvas.Height = new FixedHeight(rectangleDimension + 2);
                            checkboxCanvas.Pen = new Pen(Colors.Black, 1f);
                            checkboxCanvas.DrawRectangle(1, 1, rectangleDimension, rectangleDimension, PaintMode.Stroke);

                            var checkboxText = innerRow.AddCell(AutoWidth.Instance, new HorizontalMargins(4f, 0f)).AddText();
                            checkboxText.Alignment = new TextAlignment(Alignment.Left, Alignment.Middle);
                            checkboxText.AddContent("Completed", signStyle);
                        }
                        else if (processConfirmType == TravelerProcessConfirmationType.TimeInTimeOut)
                        {
                            processHeaderRow.AddCell(relativeConfirmWidth, dueDateString + "Time In:__________ Time Out:__________", signStyle,
                                TextAlignment.Right, new HorizontalMargins(2, 20));
                        }

                        var textCell = table.AddRow().AddCell();
                        textCell.Width = new RelativeWidth(100);
                        textCell.Margins = new HorizontalMargins(10, 2);
                        IText txt = textCell.AddText();
                        txt.Alignment = TextAlignment.Left;

                        var processName = GetProcessAliasSummaryRow(orderProcess).ProcessName;

                        txt.AddRichContent(SMALL_FONT_START + processName + FONT_END);
                    }

                    if (isOrderRework)
                    {
                        var reworkRow = table.AddRow();
                        reworkRow.AddCell(100, (order.GetOrderProcessesRows().Max(op => op.StepOrder) + 1).ToString() + " - Rework Join", stepStyle, TextAlignment.Left, new HorizontalMargins(5, 2));

                        using (var dtInternalRework = new OrdersDataSet.InternalReworkDataTable())
                        {
                            using (var taInternalRework = new InternalReworkTableAdapter())
                            {
                                taInternalRework.FillByReworkOrderID(dtInternalRework, order.OrderID);
                            }

                            var internalReworkRow = dtInternalRework.OrderBy(ir => Convert.ToInt32(ir.Active)).LastOrDefault();
                            if (internalReworkRow != null)
                            {
                                reworkRow = table.AddRow();
                                reworkRow.AddCell(100, "Join Order With Order " + internalReworkRow.OriginalOrderID, stepStyle, TextAlignment.Left, new HorizontalMargins(10, 2));
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding process section to Sales Order Traveler report.");
            }
        }

        private static OrdersDataSet.ProcessAliasSummaryRow GetProcessAliasSummaryRow(OrdersDataSet.OrderProcessesRow orderProcess)
        {
            if (orderProcess.ProcessAliasSummaryRow != null)
            {
                return orderProcess.ProcessAliasSummaryRow;
            }

            using (var dtProcessAlias = new OrdersDataSet.ProcessAliasSummaryDataTable())
            {
                using (var taProcessAlias = new Data.Datasets.OrdersDataSetTableAdapters.ProcessAliasSummaryTableAdapter())
                {
                    taProcessAlias.FillById(dtProcessAlias, orderProcess.ProcessAliasID);
                }

                return dtProcessAlias.FirstOrDefault();
            }
        }

        private string GetProcessName(int processId)
        {
            using (var taProcess = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter())
            {
                return taProcess.GetProcessName(processId);
            }
        }

        private static string FormatDueDate(DateTime dueDate)
        {
            if (ApplicationSettings.Current.SchedulerType == SchedulerType.ProcessLeadTimeHour)
            {
                return dueDate.ToString("MM/dd/yy hh:mm tt");
            }

            return dueDate.ToString("MM / dd / yy");
        }

        private void AddBarcodeFooter()
        {
            try
            {
                if (_footer == null)
                {
                    _log.Warn("Could not find footer");
                    return;
                }

                var group = _footer.AddGroup(5, 0);
                group.Margins.All = 5;
                group.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);
                var table = group.AddTable();
                table.Margins.All = 5;
                table.KeepSolid = true;

                var barcodesRow = table.AddRow();

                //Add Shipping barcde
                var workOrderCell = barcodesRow.AddCell();
                workOrderCell.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);
                var checkInGroup = workOrderCell.AddGroup();
                checkInGroup.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);

                var checkInText = checkInGroup.AddText();
                checkInText.AddContent("Shipping");
                checkInText.Alignment = new TextAlignment(Alignment.Center, Alignment.Bottom);

                var shippingBarcode = checkInGroup.AddImage(new Image(CreateSalesOrderActionBarcode(SalesOrder.SalesOrderID)));
                shippingBarcode.KeepRatio = true;
                shippingBarcode.Margins.All = 5;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding barcodes to report.");
            }
        }

        #endregion
    }

    /// <summary>
    ///     Creates the traveler that will be used to identify a PO that has been entered by receiving.
    /// </summary>
    public class OrderReceivingReport : Report
    {
        #region Fields

        private readonly PartsDataset.ReceivingSummaryRow _row;

        #endregion

        #region Properties

        public override string Title => "Order Receipt";

        protected override PageOrientation ReportPageOrientation => PageOrientation.Portrait;

        #endregion

        #region Methods

        public OrderReceivingReport(PartsDataset.ReceivingSummaryRow row) { this._row = row; }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            SetupReportInfo();
            _section.PageMargins.All = 20;

            Infragistics.Documents.Reports.Report.IContainer standardBarcodeContainer = AddHeader("ORDER RECEIPT ", "Receiving Order: ", this._row.ReceivingID, false, null, ReportType.BatchOrder);

            //Add default Bar Code Image
            IImage imageBarCode = standardBarcodeContainer.AddImage(new Image(CreateOrderBarcode(this._row.ReceivingID, true)));
            imageBarCode.Margins = new Margins(0, 5, 0, 3);
            imageBarCode.KeepRatio = true;

            AddCustomerSection();
            AddPartSection();
        }

        private void AddCustomerSection()
        {
            const float headerGroupHeight = 70;

            var dsCustomer = new CustomersDataset();
            var taCustomer = new CustomerTableAdapter();

            try
            {
                dsCustomer.EnforceConstraints = false;
                taCustomer.FillBy(dsCustomer.Customer, this._row.CustomerID);
                var customer = dsCustomer.Customer[0];

                IGroup headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Margins = new Margins(0, 0, 0, 0);
                headerGroup.Paddings.Horizontal = 5;

                //Add Customer Address
                var companyText = headerGroup.AddText();
                companyText.Alignment = TextAlignment.Left;
                companyText.Style = DefaultStyles.NormalStyle;
                companyText.Width = new RelativeWidth(50);
                companyText.Borders = DefaultStyles.DefaultBorders;
                companyText.Paddings.All = 5;
                companyText.Margins.Right = 5;
                companyText.Background = DefaultStyles.DefaultBackground;
                companyText.Height = new FixedHeight(headerGroupHeight);

                companyText.AddContent("Customer:", DefaultStyles.BlueLargeStyle);
                companyText.AddLineBreak();
                companyText.AddContent("   " + customer.Name, DefaultStyles.BoldStyle);
                companyText.AddLineBreak();
                if (customer.HasBillingAddress)
                {
                    companyText.AddContent("   " + (customer.IsAddress1Null() ? string.Empty : customer.Address1), DefaultStyles.NormalStyle);
                    companyText.AddLineBreak();
                    companyText.AddContent("   " + (customer.IsCityNull() ? string.Empty : customer.City) + ", ", DefaultStyles.NormalStyle);
                    companyText.AddContent((customer.IsStateNull() ? string.Empty : customer.State) + " ", DefaultStyles.NormalStyle);
                    companyText.AddContent(customer.IsZipNull() ? string.Empty : customer.Zip, DefaultStyles.NormalStyle);
                }

                //Add Order Information
                Infragistics.Documents.Reports.Report.IContainer orderContainer = headerGroup.AddContainer("order");
                orderContainer.Width = new RelativeWidth(50);
                orderContainer.Borders = DefaultStyles.DefaultBorders;
                orderContainer.Paddings.All = 5;
                orderContainer.Margins.Right = 5;
                orderContainer.Background = DefaultStyles.DefaultBackground;
                orderContainer.Height = new FixedHeight(headerGroupHeight);

                var orderTable = orderContainer.AddTable();
                orderTable.AddRow().AddCells(DefaultStyles.BlueLargeStyle, null, 0, TextAlignment.Left, "Receiving Order:");

                var receivingId = _row.ReceivingID.ToString();
                var customerWo = _row.IsCustomerWONull() ? "None" : _row.CustomerWO;
                var purchaseOrder = _row.IsPurchaseOrderNull() ? "None" : _row.PurchaseOrder;
                var priority = _row.IsPriorityNull() ? "N/A" : _row.Priority;

                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Receiving Order:", receivingId);
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Customer WO:", customerWo);
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Purchase Order:", purchaseOrder);
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Priority:", priority);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding customer section to report.");
            }
            finally
            {
                dsCustomer.Dispose();
                taCustomer.Dispose();
            }
        }

        private void AddPartSection()
        {
            var dtMedia = new OrderProcessingDataSet.MediaDataTable();
            var taMedia = new Data.Datasets.OrderProcessingDataSetTableAdapters.MediaTableAdapter();

            var dtPart = new OrderProcessingDataSet.PartDataTable();
            var taParts = new PartTableAdapter();
            var taPartProcesses = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter();

            try
            {
                var headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Margins.All = 5;
                headerGroup.Borders = DefaultStyles.DefaultBorders;
                headerGroup.Background = DefaultStyles.DefaultBackground;

                Infragistics.Documents.Reports.Report.IContainer orderContainer = headerGroup.AddContainer("order");
                orderContainer.Width = new RelativeWidth(45);
                orderContainer.Paddings.All = 5;
                orderContainer.Margins.Right = 5;

                taParts.FillBy(dtPart, this._row.PartID);
                var partProcessCount = taPartProcesses.GetNumberPartProcesses(this._row.PartID);
                var processStyle = partProcessCount.HasValue ? DefaultStyles.GreenStyle : DefaultStyles.RedStyle;

                var orderTable = orderContainer.AddTable();
                orderTable.AddRow().AddCells(DefaultStyles.BlueLargeStyle, null, 0, TextAlignment.Left, "Part Information:");

                var partRow = dtPart.Count > 0 ? dtPart[0] : null;
                var partName = partRow != null ? partRow.Name : "Unknown";
                var partDescription = partRow != null && !partRow.IsDescriptionNull() ? partRow.Description : "Unknown";
                var processCountString = partProcessCount?.ToString() ?? "None";
                var partMaterial = partRow != null && !partRow.IsMaterialNull() ? partRow.Material : "Unknown";
                var partQuantity = _row.PartQuantity.ToString();
                var partManufacturer = partRow != null && !partRow.IsManufacturerIDNull() ? partRow.ManufacturerID : "None";
                var partModel = partRow != null && !partRow.IsAirframeNull() ? partRow.Airframe : "None";
                var partNotes = partRow != null && !partRow.IsNotesNull() ? partRow.Notes : "None";

                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Part Number:", partName);
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Description:", partDescription);
                orderTable.AddRow().AddCells(processStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Processes:", processCountString);
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Material:", partMaterial);
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Quantity:", partQuantity);
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Manufacturer:", partManufacturer);
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Model:", partModel);
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Notes:", partNotes);

                //if no processes then add a NEW PART to receipt
                if (partProcessCount.GetValueOrDefault() < 1)
                {
                    var newPartRow = orderTable.AddRow();
                    newPartRow.Height = new FixedHeight(100);
                    var newPartCell = newPartRow.AddCell();
                    newPartCell.Paddings = new Paddings(10, 40);
                    newPartCell.AddText("New Part", DefaultStyles.RedXLargeStyle, new TextAlignment(Alignment.Center, Alignment.Middle));
                }

                //Add Part Image
                Infragistics.Documents.Reports.Report.IContainer imgContainer = headerGroup.AddContainer("partImage");
                imgContainer.Alignment.Vertical = Alignment.Middle;
                imgContainer.Alignment.Horizontal = Alignment.Right;
                imgContainer.Paddings.All = 5;
                imgContainer.Paddings.Right = 5;
                imgContainer.Margins.Left = 5;
                imgContainer.Margins.Right = 8;
                imgContainer.Width = new RelativeWidth(55);

                taMedia.FillWithoutMedia(dtMedia, this._row.PartID);
                System.Drawing.Image img = null;

                if (dtMedia.Count > 0)
                {
                    img = MediaUtilities.GetImage(dtMedia[0].MediaID, dtMedia[0].FileExtension);
                }
                else if (ApplicationSettings.Current.UseReportPlaceholderImage)
                {
                    if (File.Exists(ApplicationSettings.Current.ReportPlaceholderImagePath))
                    {
                        img = MediaUtilities.GetImage(File.ReadAllBytes(ApplicationSettings.Current.ReportPlaceholderImagePath));
                    }
                    else
                    {
                        img = Data.Properties.Resources.NoImage;
                    }
                }

                if (img != null)
                {
                    IImage image = imgContainer.AddImage(new Image(img));
                    image.KeepRatio = true;
                    int pWidth = img.Width;
                    int pHeight = img.Height;
                    double standardWidth = 270;

                    if (img.Width > 270)
                    {
                        double percent = standardWidth / pWidth;
                        image.Width = new FixedWidth(270);
                        double height = percent * pHeight;
                        image.Height = new FixedHeight((float)height);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding part section to report.");
            }
            finally
            {
                dtMedia.Dispose();
                dtPart.Dispose();
                dtMedia.Dispose();
                taParts.Dispose();
                taPartProcesses.Dispose();
            }
        }

        #endregion
    }

    public class WorkOrderSummaryReport : Report
    {
        #region Fields

        private readonly OrdersDataSet.OrderRow _order;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Work Order Summary"; }
        }

        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }

        protected override int FilenameIdentifier => _order?.OrderID ?? 0;

        protected override string FooterIdentifier
        {
            get
            {
                return _order?.OrderID.ToString();
            }
        }

        public bool HideIncompleteProcesses { get; set; }

        #endregion

        #region Methods

        public WorkOrderSummaryReport(OrdersDataSet.OrderRow order) { this._order = order; }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            SetupReportInfo();

            if (_order == null)
            {
                return;
            }

            _section.PageMargins.All = 20;

            AddHeader("WORK ORDER SUMMARY", "Work Order: ", this._order.OrderID, false, this._order, ReportType.BatchOrder);
            AddCustomerSection();
            AddPartSection();

            if (_order.GetOrderSerialNumberRows().Any(i => i.IsValidState() && i.Active))
            {
                AddSerialNumberSection();
            }

            using (var dtOrderChange = new OrdersDataSet.OrderChangeDataTable())
            {
                using (var taOrderChange = new Data.Datasets.OrdersDataSetTableAdapters.OrderChangeTableAdapter())
                {
                    taOrderChange.FillByOrderID(dtOrderChange, _order.OrderID);
                }

                var rejoinInfo = dtOrderChange
                    .Where(o => o.ChangeType == (int)OrderChangeType.Rejoin)
                    .ToList();

                if (rejoinInfo.Count > 0)
                {
                    AddRejoinSection(rejoinInfo);
                }
            }

            AddProcessSection();
        }

        private void AddCustomerSection()
        {
            var dsCustomer = new CustomersDataset();
            var taCustomer = new CustomerTableAdapter();
            CustomersDataset.CustomerRow customer = null;
            var taCOC = new Data.Datasets.COCDatasetTableAdapters.COCTableAdapter();
            const int height = 62;
            var superSized = new Style(new Font(DefaultStyles.RedXLargeStyle.Font.Name, DefaultStyles.RedXLargeStyle.Font.Size - 3, FontStyle.Bold), DefaultStyles.BlackXLargeStyle.Brush);
            try
            {
                dsCustomer.EnforceConstraints = false;
                taCustomer.FillByOrderID(dsCustomer.Customer, this._order.OrderID);
                customer = dsCustomer.Customer[0];
                var headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Background = DefaultStyles.DefaultBackground;
                headerGroup.Margins = new Margins(5, 5, 3, 0);

                var companyText = headerGroup.AddContainer("customer");
                companyText.Width = new RelativeWidth(33);
                companyText.Borders = DefaultStyles.DefaultBorders;
                companyText.Paddings.All = 4;
                companyText.Margins.Right = 5;
                companyText.Background = DefaultStyles.DefaultBackground;
                companyText.Height = new FixedHeight(height);

                var companyTable = companyText.AddTable();
                var companyTitle = companyTable.AddRow();
                companyTitle.AddCell(100, "Customer:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                var companyName = companyTable.AddRow();

                companyName.AddCell(100, "   " + customer.Name, DefaultStyles.BoldMediumStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                if (customer.HasBillingAddress)
                {
                    if (!customer.IsAddress1Null() && customer.Address1 != customer.Name)
                    {
                        var companyAddress = companyTable.AddRow();
                        companyAddress.AddCell(50, "   " + customer.Address1, DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                    }

                    var addressRow = companyTable.AddRow();
                    var zip = customer.IsZipNull() ? "" : customer.Zip;
                    addressRow.AddCell(50, "   " + (customer.IsCityNull() ? "-" : customer.City) + " " + (customer.IsStateNull() ? "-" : customer.State) + ", " + zip, DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                }

                var parentContainer = headerGroup.AddContainer("parent");
                parentContainer.Width = new FixedWidth(373);
                companyText.Height = new FixedHeight(height);
                parentContainer.Alignment.Vertical = Alignment.Middle;
                parentContainer.Alignment.Horizontal = Alignment.Left;
                var parentTable = parentContainer.AddTable();
                var prow1 = parentTable.AddRow();

                prow1.Height = new FixedHeight(height);
                var pcell1 = prow1.AddCell();
                pcell1.Alignment.Horizontal = Alignment.Left;
                pcell1.Alignment.Vertical = Alignment.Middle;
                pcell1.Width = new FixedWidth(190);
                var orderContainer = pcell1.AddContainer("order");
                var pcell2 = prow1.AddCell();
                pcell2.Alignment.Horizontal = Alignment.Left;
                pcell2.Alignment.Vertical = Alignment.Middle;
                var ReqDateContainer = pcell2.AddContainer("reqDate");

                orderContainer.Alignment.Vertical = Alignment.Top;
                orderContainer.Alignment.Horizontal = Alignment.Left;
                orderContainer.Paddings.Left = 5;
                orderContainer.Paddings.Top = 4;
                orderContainer.Borders = DefaultStyles.DefaultBorders;
                orderContainer.Height = new FixedHeight(height);

                ReqDateContainer.Paddings.Left = 5;
                ReqDateContainer.Paddings.Top = 4;
                ReqDateContainer.Height = new FixedHeight(height);
                ReqDateContainer.Alignment.Vertical = Alignment.Top;
                ReqDateContainer.Alignment.Horizontal = Alignment.Left;
                ReqDateContainer.Borders = DefaultStyles.DefaultBorders;
                ReqDateContainer.Margins.Left = 5;

                var reqDateTable = ReqDateContainer.AddTable();
                var row5 = reqDateTable.AddRow();
                row5.Height = new FixedHeight(20);

                var cell1 = row5.AddCell(40, "Required Date:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                cell1.Height = new FixedHeight(20);
                var row6 = reqDateTable.AddRow();
                row6.Height = new FixedHeight(20);
                var cell2 = row6.AddCell(40, _order.IsRequiredDateNull() ? "NA" : _order.RequiredDate.ToShortDateString(), superSized, new TextAlignment(Alignment.Center, Alignment.Middle));
                cell2.Height = new FixedHeight(20);

                var orderTable = orderContainer.AddTable();
                var orderCell = orderTable.AddRow().AddCell(100, "Order:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                //orderCell.Height = new FixedHeight(18);
                var row = orderTable.AddRow();
                var cell3 = row.AddCell(15, "   Purchase Order:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                cell3.Margins = new HorizontalMargins(0, 0);
                var trouble = row.AddCell(15, this._order.IsPurchaseOrderNull() ? "NA" : this._order.PurchaseOrder, DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                var row1 = orderTable.AddRow();
                var cell4 = row1.AddCell(15, "   Customer WO:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                cell4.Margins = new HorizontalMargins(0, 0);
                row1.AddCell(15, this._order.IsCustomerWONull() ? "None" : this._order.CustomerWO, DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                var row2 = orderTable.AddRow();
                var cell5 = row2.AddCell(15, "   Received Date:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                cell5.Margins = new HorizontalMargins(0, 0);
                row2.AddCell(15, this._order.OrderDate.ToShortDateString(), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                var row3 = orderTable.AddRow();
                var cell6 = row3.AddCell(15, "   Order Receipt:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                cell6.Margins = new HorizontalMargins(0, 0);
                row3.AddCell(15, this._order.IsReceivingIDNull() ? "None" : this._order.ReceivingID.ToString(), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding customer section to report.");
            }
            finally
            {
                if (dsCustomer != null)
                    dsCustomer.Dispose();
                if (taCustomer != null)
                    taCustomer.Dispose();
                if (taCOC != null)
                    taCOC.Dispose();

                dsCustomer = null;
                customer = null;
                taCustomer = null;
                taCOC = null;
            }
        }

        private void AddPartSection()
        {
            var dtMedia = new OrderProcessingDataSet.MediaDataTable();
            var dtPart = new OrderProcessingDataSet.PartDataTable();
            var taMedia = new Data.Datasets.OrderProcessingDataSetTableAdapters.MediaTableAdapter();
            var taParts = new PartTableAdapter();

            try
            {
                bool isMaterialVisible;
                bool isSurfaceAreaVisible;
                bool isManufacturerVisible;

                using (var taField = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
                {
                    using (var dtPartFields = taField.GetByCategory("Part"))
                    {
                        var materialField = dtPartFields.FirstOrDefault(field => field.Name == "Material");
                        isMaterialVisible = materialField != null && materialField.IsVisible;

                        var surfaceAreaField = dtPartFields.FirstOrDefault(field => field.Name == "Surface Area");
                        isSurfaceAreaVisible = surfaceAreaField != null && surfaceAreaField.IsVisible;

                        var manufacturerField = dtPartFields.FirstOrDefault(field => field.Name == "Manufacturer");
                        isManufacturerVisible = manufacturerField != null && manufacturerField.IsVisible;
                    }
                }
                var headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Margins = new Margins(5, 5, 3, 0);
                headerGroup.Borders = DefaultStyles.DefaultBorders;
                headerGroup.Background = DefaultStyles.DefaultBackground;

                var orderContainer = headerGroup.AddContainer("order");
                orderContainer.Width = new RelativeWidth(45);
                orderContainer.Paddings.All = 5;
                orderContainer.Margins.Right = 5;

                taParts.FillBy(dtPart, this._order.PartID);

                //Add Part info table
                var orderTable = orderContainer.AddTable();
                orderTable.AddRow().AddCells(DefaultStyles.BlueLargeStyle, new SolidColorBrush(new Color(System.Drawing.Color.WhiteSmoke)), null, 0, TextAlignment.Left, "Part Information:");
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Part Number:", (dtPart.Count > 0 ? dtPart[0].Name : "Unknown"));
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Description:", (dtPart.Count > 0 && !dtPart[0].IsDescriptionNull() ? dtPart[0].Description : "Unknown"));

                if (isMaterialVisible)
                {
                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Material:", (dtPart.Count > 0 && !dtPart[0].IsMaterialNull() ? dtPart[0].Material : "Unknown"));
                }

                if (isSurfaceAreaVisible)
                {
                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Dimensions:", (dtPart.Count > 0 ? PartAreaUtilities.PartDimensionString(dtPart[0]) : "None"));
                }

                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Quantity:", (this._order.IsPartQuantityNull() ? "Unknown" : this._order.PartQuantity.ToString()));

                if (isManufacturerVisible)
                {
                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Manufacturer:", (dtPart.Count > 0 && !dtPart[0].IsManufacturerIDNull() ? dtPart[0].ManufacturerID : "None"));
                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Model:", (dtPart.Count > 0 && !dtPart[0].IsAirframeNull() ? dtPart[0].Airframe : "None"));
                }

                //Add Custom Fields to traveler
                using (var taOrderCustomField = new Data.Datasets.OrdersDataSetTableAdapters.OrderCustomFieldsTableAdapter())
                {
                    foreach (var orderCustomField in taOrderCustomField.GetWOTravelerValues(_order.CustomerID, _order.OrderID))
                    {
                        if (orderCustomField.IsValueNull() || string.IsNullOrWhiteSpace(orderCustomField.Value))
                        {
                            continue;
                        }

                        var fieldName = $"   {orderCustomField["Name"]}:";
                        var fieldValue = orderCustomField.Value;

                        orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0,
                            new TextAlignment(Alignment.Left, Alignment.Middle), fieldName, fieldValue);
                    }
                }

                // Add part-level custom fields.
                if (!_order.IsPartIDNull())
                {
                    using (var dsParts = new PartsDataset { EnforceConstraints = false })
                    {
                        using (var taPartLevelCustomField = new Data.Datasets.PartsDatasetTableAdapters.PartLevelCustomFieldTableAdapter())
                        {
                            taPartLevelCustomField.FillByPartID(dsParts.PartLevelCustomField, _order.PartID);
                        }

                        using (var taPartCustomFields = new Data.Datasets.PartsDatasetTableAdapters.PartCustomFieldsTableAdapter())
                        {
                            taPartCustomFields.FillByPartID(dsParts.PartCustomFields, _order.PartID);
                        }

                        foreach (var customField in dsParts.PartCustomFields)
                        {
                            if (customField.PartLevelCustomFieldRow == null || !customField.PartLevelCustomFieldRow.DisplayOnTraveler)
                            {
                                continue;
                            }

                            var fieldName = $"   {customField.PartLevelCustomFieldRow.Name}:";
                            var fieldValue = customField.IsValueNull() ? string.Empty : customField.Value;

                            orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0,
                                new TextAlignment(Alignment.Left, Alignment.Middle), fieldName, fieldValue);
                        }
                    }
                }

                //Add notes after any custom fields are added
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Notes:", (dtPart.Count > 0 && !dtPart[0].IsNotesNull() ? dtPart[0].Notes : "None"));

                //Add Order Notes
                var orderNotes = this._order.GetOrderNoteRows();
                if (orderNotes != null && orderNotes.Length > 0)
                {
                    var internalNotes = orderNotes.Where(on => on.NoteType == "Internal");
                    var externalNotes = orderNotes.Where(on => on.NoteType != "Internal");

                    var noteSummary = new StringBuilder();
                    foreach (var externalNote in externalNotes)
                        noteSummary.AppendLine(externalNote.Notes.TrimToMaxLength(120, "..."));

                    if (internalNotes.Any())
                        noteSummary.AppendLine("See INTERNAL notes.");

                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Order Notes:", noteSummary.ToString());
                }

                //Add Part Marking paragraph
                if (dtPart[0].PartMarking && !dtPart[0].IsAirframeNull())
                {
                    using (var taOPM = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderPartMarkTableAdapter())
                    {
                        var orderPMTable = new OrderProcessingDataSet.OrderPartMarkDataTable();
                        taOPM.Fill(orderPMTable, this._order.OrderID);

                        var orderPMRow = orderPMTable.FirstOrDefault();

                        //if there is an order template then use that
                        if (orderPMRow != null)
                        {
                            using (var taPart = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
                            {
                                var partDT = new PartsDataset.PartDataTable();
                                taPart.FillByPartID(partDT, dtPart[0].PartID);
                                var pmCon = orderContainer.AddContainer("PM");
                                pmCon.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);

                                var partMarkingTable = pmCon.AddTable();

                                partMarkingTable.AddRow().AddCells(DefaultStyles.BlueLargeStyle, new SolidColorBrush(new Color(System.Drawing.Color.WhiteSmoke)), null, 0, TextAlignment.Left, "Part Marking:");
                                if (!orderPMRow.IsLine1Null())
                                    partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(orderPMRow.Line1, partDT[0], this._order));
                                if (!orderPMRow.IsLine2Null())
                                    partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(orderPMRow.Line2, partDT[0], this._order));
                                if (!orderPMRow.IsLine3Null())
                                    partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(orderPMRow.Line3, partDT[0], this._order));
                                if (!orderPMRow.IsLine4Null())
                                    partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(orderPMRow.Line4, partDT[0], this._order));
                            }
                        }
                        else //else use the parts PM template
                        {
                            using (var taPM = new Data.Datasets.OrderProcessingDataSetTableAdapters.PartMarkingTableAdapter())
                            {
                                var pmDT = taPM.GetDataByPart(dtPart[0].PartID);

                                if (pmDT.Count > 0)
                                {
                                    using (var taPart = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
                                    {
                                        var partDT = new PartsDataset.PartDataTable();
                                        taPart.FillByPartID(partDT, dtPart[0].PartID);

                                        var pmRow = pmDT[0];

                                        var pmCon = orderContainer.AddContainer("PM");
                                        pmCon.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);

                                        var partMarkingTable = pmCon.AddTable();

                                        partMarkingTable.AddRow().AddCells(DefaultStyles.BlueLargeStyle, new SolidColorBrush(new Color(System.Drawing.Color.WhiteSmoke)), null, 0, TextAlignment.Left, "Part Marking:");
                                        if (!pmRow.IsDef1Null())
                                            partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(pmRow.Def1, partDT[0], this._order));
                                        if (!pmRow.IsDef2Null())
                                            partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(pmRow.Def2, partDT[0], this._order));
                                        if (!pmRow.IsDef3Null())
                                            partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(pmRow.Def3, partDT[0], this._order));
                                        if (!pmRow.IsDef4Null())
                                            partMarkingTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   " + Interperter.Interpert(pmRow.Def4, partDT[0], this._order));
                                    }
                                }
                            }
                        }
                    }
                }

                //Add Part Quantity History paragraph
                using (var taAudit = new AuditTableAdapter())
                {
                    var partQuantityHistory = taAudit.GetPartQuantityChangesByOrderId(_order.OrderID.ToString());
                    if (partQuantityHistory.Count > 0)
                    {
                        var pqhCon = orderContainer.AddContainer("PQH");
                        pqhCon.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);

                        var partQuantityHistoryTable = pqhCon.AddTable();

                        partQuantityHistoryTable.AddRow().AddCells(DefaultStyles.BlueLargeStyle, new SolidColorBrush(new Color(System.Drawing.Color.WhiteSmoke)), null, 0, TextAlignment.Left, "Part Quantity History:");

                        foreach (var record in partQuantityHistory)
                        {
                            partQuantityHistoryTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), $"   Qty. Changed From {record.OldValue} to {record.NewValue} On {record.AuditDate:MM/dd/yyyy HH:mm}");
                        }

                    }
                }

                //Add Part Image
                var imgContainer = headerGroup.AddContainer("partImage");
                imgContainer.Alignment.Vertical = Alignment.Middle;
                imgContainer.Alignment.Horizontal = Alignment.Right;
                imgContainer.Paddings.All = 5;
                imgContainer.Paddings.Right = 5;
                imgContainer.Margins.Left = 5;
                imgContainer.Margins.Right = 8;
                imgContainer.Width = new RelativeWidth(55);

                taMedia.FillWithoutMedia(dtMedia, this._order.PartID);
                System.Drawing.Image img = null;

                if (dtMedia.Count > 0)
                {
                    img = MediaUtilities.GetImage(dtMedia[0].MediaID, dtMedia[0].FileExtension);
                }
                else if (ApplicationSettings.Current.UseReportPlaceholderImage)
                {
                    if (File.Exists(ApplicationSettings.Current.ReportPlaceholderImagePath))
                    {
                        img = MediaUtilities.GetImage(File.ReadAllBytes(ApplicationSettings.Current.ReportPlaceholderImagePath));
                    }
                    else
                    {
                        img = Data.Properties.Resources.NoImage;
                    }
                }

                if (img != null)
                {
                    var image = imgContainer.AddImage(new Image(img));
                    image.KeepRatio = true;
                    double pWidth = img.Width;
                    double pHeight = img.Height;
                    double standardWidth = 270;

                    if (img.Width > 270)
                    {
                        double percent = standardWidth / pWidth;
                        image.Width = new FixedWidth(270);
                        double height = percent * pHeight;
                        image.Height = new FixedHeight((float)height);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Fatal(exc, "Error adding part section to report.");
            }
            finally
            {
                if (dtMedia != null)
                    dtMedia.Dispose();
                if (dtPart != null)
                    dtPart.Dispose();
                if (taMedia != null)
                    dtMedia.Dispose();
                if (taParts != null)
                    taParts.Dispose();

                dtMedia = null;
                dtPart = null;
                taMedia = null;
                taParts = null;
            }
        }

        private void AddSerialNumberSection()
        {
            try
            {
                var headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Margins = new Margins(5, 5, 3, 0);
                headerGroup.Borders = DefaultStyles.DefaultBorders;
                headerGroup.Background = DefaultStyles.DefaultBackground;

                var serialNumberContainer = headerGroup.AddContainer("serial");
                serialNumberContainer.Width = new RelativeWidth(100);
                serialNumberContainer.Paddings.All = 5;
                serialNumberContainer.Margins.Right = 5;

                var serialTable = serialNumberContainer.AddTable();
                serialTable.AddRow()
                    .AddCells(DefaultStyles.BlueLargeStyle,
                        null, 0, TextAlignment.Left,
                        "Serial Numbers:");

                var activeSerialNumbers = _order.GetOrderSerialNumberRows()
                    .Where(s => s.IsValidState() && s.Active && !s.IsNumberNull())
                    .OrderBy(s => s.PartOrder)
                    .Select(s => s.Number);

                serialTable.AddRow()
                    .AddCells(DefaultStyles.NormalStyle, null, 0, TextAlignment.Left, string.Join(", ", activeSerialNumbers));
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding serial number section to report.");
            }
        }

        private void AddRejoinSection(List<OrdersDataSet.OrderChangeRow> rejoinInfo)
        {
            try
            {
                var headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Margins = new Margins(5, 5, 3, 0);
                headerGroup.Borders = DefaultStyles.DefaultBorders;
                headerGroup.Background = DefaultStyles.DefaultBackground;

                var rejoinContainer = headerGroup.AddContainer("rejoin");
                rejoinContainer.Width = new RelativeWidth(45);
                rejoinContainer.Paddings.All = 5;
                rejoinContainer.Margins.Right = 5;

                var rejoinTable = rejoinContainer.AddTable();
                rejoinTable.AddRow()
                    .AddCells(DefaultStyles.BlueLargeStyle,
                        null, 0, TextAlignment.Left,
                        "Work Order Rejoin History:");

                foreach (var joinedToChange in rejoinInfo.Where(r => r.ParentOrderID == _order.OrderID))
                {
                    rejoinTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, TextAlignment.Left,
                        "Joined To:",
                        joinedToChange.ChildOrderID.ToString());
                }

                foreach (var joinedFromChange in rejoinInfo.Where(r => r.ChildOrderID == _order.OrderID))
                {
                    rejoinTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, TextAlignment.Left,
                        "Joined From:",
                        joinedFromChange.ParentOrderID.ToString());
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding rejoin section to report.");
            }
        }

        private void AddProcessSection()
        {
            var dsOrderProcess = new OrderProcessingDataSet();
            var dtPartInspections = new PartInspectionDataSet.PartInspectionDataTable();
            dsOrderProcess.EnforceConstraints = false;

            using (var taProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                taProcesses.FillBy(dsOrderProcess.OrderProcesses, this._order.OrderID);
            using (var taProcesses = new ProcessAliasTableAdapter())
                taProcesses.Fill(dsOrderProcess.ProcessAlias, this._order.OrderID);
            using (var taProcesses = new ProcessTableAdapter())
                taProcesses.FillByOrder(dsOrderProcess.Process, this._order.OrderID);
            using (var taOrderProcessAnswer = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessAnswerTableAdapter())
                taOrderProcessAnswer.FillBy(dsOrderProcess.OrderProcessAnswer, this._order.OrderID);
            using (var taPartInspection = new Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionTableAdapter())
                taPartInspection.FillByOrderID(dtPartInspections, this._order.OrderID);

            try
            {


                // Add Process Summary section
                var summaryGroup = _section.AddGroup();
                summaryGroup.Layout = Layout.Vertical;
                summaryGroup.Margins.All = 5;
                summaryGroup.Paddings.All = 5;
                summaryGroup.Borders = DefaultStyles.DefaultBorders;
                summaryGroup.Background = DefaultStyles.DefaultBackground;

                var summaryText = summaryGroup.AddText();
                summaryText.Width = new RelativeWidth(100);
                summaryText.Alignment = TextAlignment.Left;
                summaryText.Style = DefaultStyles.NormalStyle;
                summaryText.Margins.Right = 5;
                summaryText.AddContent("Summary of Processes:", DefaultStyles.BlueLargeStyle);
                summaryText.AddLineBreak();

                var processSummaryTable = summaryGroup.AddTable();
                processSummaryTable.Borders = DefaultStyles.DefaultBorders;
                processSummaryTable.AddRow().AddHeaderCells(DefaultStyles.BoldStyle, DefaultStyles.DefaultBorders, 2, new TextAlignment(Alignment.Left, Alignment.Middle), "Dept.", "Process Name", "Start Date", "End Date");


                var group = _section.AddGroup();
                group.Layout = Layout.Vertical;
                group.Margins.All = 5;
                group.Paddings.All = 5;
                group.Borders = DefaultStyles.DefaultBorders;
                group.Background = DefaultStyles.DefaultBackground;

                var text = group.AddText();
                text.Width = new RelativeWidth(100);
                text.Alignment = TextAlignment.Left;
                text.Style = DefaultStyles.NormalStyle;
                text.Margins.Right = 5;
                text.AddContent("Processes:", DefaultStyles.BlueLargeStyle);
                text.AddLineBreak();

                var borders = DefaultStyles.DefaultBorders;
                var processTable = group.AddTable();
                processTable.Borders = borders;

                var ta = new TextAlignment(Alignment.Left, Alignment.Middle);
                var processStep = 1;

                processTable.AddRow().AddHeaderCells(DefaultStyles.BoldStyle, borders, 2, ta, "Step", "Dept.", "Name", "Description", "Rev.");

                foreach (var process in dsOrderProcess.OrderProcesses)
                {
                    if (HideIncompleteProcesses && process.IsEndDateNull())
                    {
                        continue;
                    }

                    var dtProcess = new Data.Datasets.OrderProcessingDataSet.ProcessDataTable();
                    using (var taProcess = new ProcessTableAdapter())
                        taProcess.FillByProcess(dtProcess, process.ProcessID);

                    processTable.AddRow().AddCells(DefaultStyles.NormalStyle, new SolidColorBrush(new Color(System.Drawing.Color.Wheat)), borders, 2, ta, processStep.ToString(), process.Department, process.ProcessRow.Name, process.ProcessRow.Description, process.ProcessRow.Revision);
                    processSummaryTable.AddRow().AddCells(DefaultStyles.NormalStyle, new SolidColorBrush(new Color(System.Drawing.Color.Wheat)), borders, 2, ta, process.Department, process.ProcessRow.Name, process.IsStartDateNull() ? "N/A" : process.StartDate.ToShortDateString(), process.IsEndDateNull() ? "N/A" : process.EndDate.ToShortDateString());
                    processStep++;

                    using (var taProcessSteps = new ProcessStepsTableAdapter())
                        taProcessSteps.FillBy(dsOrderProcess.ProcessSteps, process.ProcessID);
                    using (var taCon = new ProcessStepConditionTableAdapter())
                        taCon.Fill(dsOrderProcess.ProcessStepCondition, process.ProcessID);
                    using (var taProcessQuestion = new ProcessQuestionTableAdapter())
                        taProcessQuestion.FillBy(dsOrderProcess.ProcessQuestion, process.ProcessID);
                    using (var taProcessInspections = new ProcessInspectionsTableAdapter())
                        taProcessInspections.FillByProcess(dsOrderProcess.ProcessInspections, process.ProcessID);

                    var stepTable = processTable.AddRow().AddCell().AddTable();
                    stepTable.Margins.All = 10;
                    stepTable.Borders = borders;


                    //add cells here
                    var colWidths = new List<int> {20,80};

                    stepTable.AddRow().AddHeaderCells(DefaultStyles.BoldStyle, borders, 2, ta, colWidths, "Step","Name");
                    var isPaperless = dtProcess.AsEnumerable().Select(r => r.IsPaperless).FirstOrDefault();
                    if (!isPaperless)
                    {
                        var stepTitleRow = stepTable.AddRow();
                        stepTitleRow.AddCell(20, "This process is paper based", DefaultStyles.NormalStyle, borders, 2, ta);
                    }
                    else
                    {
                        foreach (var step in dsOrderProcess.ProcessSteps)
                        {
                            var stepTitleRow = stepTable.AddRow();
                            stepTitleRow.AddCell(20, step.StepOrder.ToString(), DefaultStyles.NormalStyle, borders, 2, ta);
                            stepTitleRow.AddCell(80, step.Name, DefaultStyles.NormalStyle, borders, 2, ta);

                            if (IsStepExcluded(step) && step.GetProcessStepConditionRows().Any())
                            {
                                stepTable.AddRow().AddCells(DefaultStyles.RedStyle, borders, 2, ta, "Step excluded from processing due to process conditionals.");
                            }
                            else if (step.GetProcessQuestionRows().Any()) //Add this section only if there are records
                            {
                                var answerTable = stepTable.AddRow().AddCell().AddTable();
                                answerTable.Margins.All = 10;
                                answerTable.Borders = borders;
                                answerTable.AddRow().AddHeaderCells(DefaultStyles.BoldStyle, borders, 2, ta, "Question", "Answer", "Completed", "Completed By", "Date Completed");

                                foreach (var question in step.GetProcessQuestionRows())
                                {
                                    var answer = question.GetOrderProcessAnswerRows().FirstOrDefault(w => w.OrderProcessesID == process.OrderProcessesID);

                                    if (answer != null)
                                    {
                                        var completedByString = (answer.IsCompletedByNull() ? string.Empty : GetUserName(answer.CompletedBy));
                                        var completedDateString = (answer.IsCompletedDataNull() ? string.Empty : answer.CompletedData.ToShortDateString());

                                        string answerString;

                                        if (answer.IsAnswerNull() || string.IsNullOrEmpty(answer.Answer))
                                        {
                                            answerString = "No Answer";
                                        }
                                        else if (answer.ProcessQuestionRow.InputType == InputType.PartQty.ToString() || answer.ProcessQuestionRow.IsNumericUntisNull())
                                        {
                                            answerString = answer.Answer;
                                        }
                                        else
                                        {
                                            answerString = string.Format("{0} {1}",
                                                answer.Answer,
                                                answer.ProcessQuestionRow.NumericUntis);
                                        }

                                        answerTable.AddRow().AddCells(DefaultStyles.NormalStyle, borders, 2, ta, question.Name, answerString, answer.Completed.ToString(), completedByString, completedDateString);
                                    }
                                    else
                                        answerTable.AddRow().AddCells(DefaultStyles.NormalStyle, borders, 2, ta, question.Name, "No Answer");
                                }
                            }
                        }

                        foreach (var processInspection in dsOrderProcess.ProcessInspections)
                        {
                            var inspectTable = processTable.AddRow().AddCell().AddTable();
                            inspectTable.Margins.All = 10;
                            inspectTable.Borders = borders;
                            inspectTable.AddRow().AddHeaderCells(DefaultStyles.BoldStyle, borders, 2, ta, "QA Inspection");
                            var cells = inspectTable.AddRow().AddHeaderCells(DefaultStyles.BoldStyle, borders, 2, ta, "Inspection Type", "Accepted Qty", "Rejected Qty", "Status", "Inspected Date", "Inspected By");
                            FormatQACellWidths(cells);

                            //FIX: Updated to pull answers for inspection for this current order process
                            var partInspection = dtPartInspections
                                .FirstOrDefault(partInsp => !partInsp.IsOrderProcessIDNull() && partInsp.OrderProcessID == process.OrderProcessesID && partInsp.PartInspectionTypeID == processInspection.PartInspectionTypeID);

                            //Add the inspection info
                            if (partInspection != null)
                            {
                                if (partInspection.IsQAUserIDNull())
                                    cells = inspectTable.AddRow().AddCells(DefaultStyles.NormalStyle, borders, 2, ta, processInspection.Name, partInspection.AcceptedQty.ToString(), partInspection.RejectedQty.ToString(), partInspection.IsStatusNull() ? "Incomplete" : (partInspection.Status ? "Pass" : "Fail"), (partInspection.IsInspectionDateNull() ? "-" : partInspection.InspectionDate.ToShortDateString()), "Unknown");
                                else
                                    cells = inspectTable.AddRow().AddCells(DefaultStyles.NormalStyle, borders, 2, ta, processInspection.Name, partInspection.AcceptedQty.ToString(), partInspection.RejectedQty.ToString(), partInspection.IsStatusNull() ? "Incomplete" : (partInspection.Status ? "Pass" : "Fail"), (partInspection.IsInspectionDateNull() ? "-" : partInspection.InspectionDate.ToShortDateString()), GetUserName(partInspection.QAUserID));

                                FormatQACellWidths(cells);
                            }
                            else
                            {
                                cells = inspectTable.AddRow().AddCells(DefaultStyles.NormalStyle, borders, 2, ta, processInspection.Name, " ", " ", "Pending", " ", " ");
                                FormatQACellWidths(cells);
                            }

                            if (partInspection != null)
                            {
                                //Get the QA question/answers for the part inspection
                                QAReport.PartInspectionAnswerDataTable dtQaInspections;
                                using (var qaInspections = new Data.Reports.QAReportTableAdapters.PartInspectionAnswerTableAdapter())
                                    dtQaInspections = qaInspections.GetByPartInspectionID(partInspection.PartInspectionID);

                                if (dtQaInspections.Any())
                                {
                                    inspectTable.AddRow().AddHeaderCells(DefaultStyles.BoldStyle, borders, 2, ta, "Question", "Answer", "Completed", "Date Completed", "Completed By");

                                    foreach (var qa in dtQaInspections)
                                    {
                                        //Get and display the QA question
                                        var dtQaQuestion = new QAReport.PartInspectionQuestionDataTable();
                                        using (var qaQuestion = new Data.Reports.QAReportTableAdapters.PartInspectionQuestionTableAdapter())
                                            qaQuestion.FillByPartInspectionQuestionID(dtQaQuestion, qa.PartInspectionQuestionID);

                                        //Display the QA answer
                                        string uom = qa.ItemArray[10].ToString();
                                        inspectTable.AddRow().AddCells(DefaultStyles.NormalStyle, borders, 2, ta,
                                            dtQaQuestion[0].Name,
                                            qa.IsAnswerNull() ? string.Empty : (qa.Answer + uom),
                                            qa.Completed.ToString(),
                                            qa.IsCompletedDataNull() ? "" : qa.CompletedData.ToShortDateString(),
                                            qa.IsCompletedByNull() ? "" : GetUserName(qa.CompletedBy));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Fatal(exc, "Error adding process section to report.");
            }
            finally
            {
                if (dsOrderProcess != null)
                    dsOrderProcess.Dispose();

                dsOrderProcess = null;
            }
        }

        /// <summary>
        /// Determines whether the step is excluded because all answers where excluded.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// <c>true</c> if the is step is excluded; otherwise, <c>false</c>.
        /// </returns>
        private bool IsStepExcluded(OrderProcessingDataSet.ProcessStepsRow step) =>
            step.GetProcessQuestionRows()
                .SelectMany(q => q.GetOrderProcessAnswerRows())
                .All(opa => opa.Completed && opa.IsCompletedByNull() && opa.IsAnswerNull());

        /// <summary>
        ///     Formatting for the QA cells
        /// </summary>
        /// <param name="cells"></param>
        private void FormatQACellWidths(ITableCell[] cells)
        {
            if (cells[0] != null)
                cells[0].Width = new RelativeWidth(32);
            if (cells[1] != null)
                cells[1].Width = new RelativeWidth(13);
            if (cells[2] != null)
                cells[2].Width = new RelativeWidth(13);
            if (cells[3] != null)
                cells[3].Width = new RelativeWidth(8);
            if (cells[4] != null)
                cells[4].Width = new RelativeWidth(16);
            if (cells[5] != null)
                cells[5].Width = new RelativeWidth(18);
        }

        private string GetUserName(int userID)
        {
            var dtUser = this._order.Table.DataSet.Tables["UserSummary"] as OrdersDataSet.UserSummaryDataTable;
            OrdersDataSet.UserSummaryRow user = dtUser.FindByUserID(userID);

            if (user != null)
                return user.Name;
            else
                return "Unknown User";
        }

        #endregion
    }

    public class LateOrderReport : ExcelBaseReport
    {
        #region Fields

        private const string DEFAULT_PRODUCT_CLASS = "N/A";
        #endregion

        #region Properties

        public override string Title => "Late Orders";

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation => PageOrientation.Portrait;

        /// <summary>
        /// Gets the group type for this instance.
        /// </summary>
        public GroupByType Type { get; }

        private string GroupByName
        {
            get
            {
                switch (Type)
                {
                    case GroupByType.Customer:
                        return "Customer";
                    case GroupByType.Department:
                        return "Department";
                    case GroupByType.ProductClass:
                        return "Product Class";
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion

        #region Methods

        public LateOrderReport(GroupByType type)
        {
            Type = type;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            CreateWorkBook();
            CreateExcelLateOrderSummary();
            CreateExcelDetail();
        }

        protected void CreateExcelDetail()
        {
            OrdersReport dsOrderReport = new OrdersReport();
            dsOrderReport.EnforceConstraints = false;

            Data.Reports.OrdersReportTableAdapters.OrderTableAdapter taOrders = null;

            try
            {
                taOrders = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter();
                DateTime leadDateTime = DateTime.Now.StartOfDay();
                taOrders.FillLateOrders(dsOrderReport.Order, leadDateTime);

                Worksheet lateWks = CreateWorksheet(Title + " Detail");
                var rowIndex = base.AddCompanyHeaderRows(lateWks, 8, $"Details - {GroupByName}") + 2;

                using (var taOrderSerialNumbers = new Data.Reports.OrdersReportTableAdapters.OrderSerialNumberTableAdapter())
                {
                    taOrderSerialNumbers.FillActive(dsOrderReport.OrderSerialNumber);
                }

                using (var taOrderProductClass = new Data.Reports.OrdersReportTableAdapters.OrderProductClassTableAdapter())
                {
                    taOrderProductClass.Fill(dsOrderReport.OrderProductClass);
                }

                var custRows = new List<OrderData>();

                foreach (OrdersReport.OrderRow item in dsOrderReport.Order)
                {
                    if (item.IsEstShipDateNull())
                    {
                        _log.Warn($"Late Order Report query returned an order {item.OrderID} w/o an estimated ship date");
                        continue;
                    }

                    OrderData row = OrderData.From(item);

                    custRows.Add(row);
                }

                FillLateDetails(lateWks, custRows, rowIndex);
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to create Late Details Report.");
            }
            finally
            {
                taOrders?.Dispose();
            }
        }

        protected void CreateExcelLateOrderSummary()
        {
            OrdersReport dsReport = null;
            Data.Reports.OrdersReportTableAdapters.OrderTableAdapter taOrders = null;
            Data.Reports.OrdersReportTableAdapters.OrderProductClassTableAdapter taOrderProductClass = null;

            try
            {
                dsReport = new OrdersReport();
                taOrders = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter();
                taOrderProductClass = new Data.Reports.OrdersReportTableAdapters.OrderProductClassTableAdapter { ClearBeforeFill = false };

                DateTime currentDateTime = DateTime.Now;
                DateTime leadDateTime = currentDateTime.AddBusinessDays(3).StartOfDay();

                taOrders.FillLateOrders(dsReport.Order, leadDateTime);


                Worksheet lateWks = CreateWorksheet(Title + " Summary");
                var rowIndex = base.AddCompanyHeaderRows(lateWks, 7, $"Summary - {GroupByName}") + 2;
                rowIndex = AddExcelLateOrderSummaryHeader(lateWks, rowIndex);

                var customerList = new List<OrderSummary>();

                foreach (var item in dsReport.Order)
                {
                    taOrderProductClass.FillByOrder(dsReport.OrderProductClass, item.OrderID);
                    var custRow = OrderSummary.NewSummaryRow(item);
                    customerList.Add(custRow);
                }

                FillLateSummary(lateWks, customerList, rowIndex);
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to create Late Order Summary Report.");
            }
            finally
            {
                dsReport?.Dispose();
                taOrders?.Dispose();
                taOrderProductClass?.Dispose();
            }
        }

        private void FillLateDetails(Worksheet lateWks, List<OrderData> orders, int rowIndex)
        {
            try
            {
                IEnumerable<IGrouping<string, OrderData>> groupedOrders;

                if (Type == GroupByType.Customer)
                {
                    groupedOrders = orders
                        .GroupBy(o => o.CustomerName);
                }
                else if (Type == GroupByType.Department)
                {
                    groupedOrders = orders
                        .GroupBy(o => o.CurrentLocation);
                }
                else
                {
                    groupedOrders = orders
                        .GroupBy(o => o.ProductClass);
                }

                foreach (var group in groupedOrders.OrderBy(o => o.Key))
                {
                    rowIndex++;
                    var totalSummary = new OrdersSummary();
                    WorksheetMergedCellsRegion region = CreateMergedHeader(lateWks, rowIndex, 0, rowIndex, 7, group.Key);
                    region.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.FromArgb(170, 170, 170)), null, FillPatternStyle.Solid);
                    lateWks.Rows[rowIndex].CellFormat.Alignment = HorizontalCellAlignment.Left;
                    rowIndex++;
                    AddExcelLateOrderHeader(lateWks, rowIndex);
                    rowIndex++;

                    foreach (var order in group)
                    {
                        AddExcelOrderRow(lateWks, order, rowIndex, 0);
                        int regionIndex = rowIndex + 1;
                        WorksheetRegion borderRegion = lateWks.GetRegion("A" + regionIndex + ":H" + regionIndex);
                        foreach (WorksheetCell cell in borderRegion)
                        {
                            AddCellBorders(cell);
                            cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                        }

                        totalSummary.AddRow(order);
                        rowIndex++;
                    }

                    var avgDaysLateTotal = totalSummary.AverageDaysLate;

                    AddExcelSummaryRow(totalSummary.Price.ToString(OrderPrice.CurrencyFormatString),
                        totalSummary.Orders, totalSummary.Parts, avgDaysLateTotal, lateWks, rowIndex);

                    rowIndex++;
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to fill Late Details Report.");
            }
        }

        private void FillLateSummary(Worksheet worksheet, List<OrderSummary> orders, int rowIndex)
        {
            try
            {
                bool colorFlag = false;

                List<OrderCounts> orderCounts;

                if (Type == GroupByType.Customer)
                {
                    orderCounts = OrderCounts
                        .GetCustomerCounts(orders);
                }
                else if (Type == GroupByType.Department)
                {
                    orderCounts = OrderCounts
                        .GetDepartmentCounts(orders);
                }
                else
                {
                    orderCounts = OrderCounts
                        .GetProductClassCounts(orders);
                }

                foreach (var customerCount in orderCounts.OrderBy(c => c.Name))
                {
                    colorFlag = !colorFlag;

                    for (var colIndex = 0; colIndex <= 8; ++colIndex)
                    {
                        var cell = worksheet.Rows[rowIndex].Cells[colIndex];

                        if (colIndex != 0)
                        {
                            cell.CellFormat.Alignment = HorizontalCellAlignment.Center;
                        }

                        AddCellColor(colorFlag, cell);
                        AddCellBorders(cell);
                    }

                    worksheet.Rows[rowIndex].Cells[0].Value = customerCount.Name;
                    worksheet.Rows[rowIndex].Cells[1].Value = customerCount.Lead3;
                    worksheet.Rows[rowIndex].Cells[2].Value = customerCount.Lead2;
                    worksheet.Rows[rowIndex].Cells[3].Value = customerCount.Lead1;
                    worksheet.Rows[rowIndex].Cells[4].Value = customerCount.Today;
                    worksheet.Rows[rowIndex].Cells[5].Value = customerCount.Late5;
                    worksheet.Rows[rowIndex].Cells[6].Value = customerCount.Late10;
                    worksheet.Rows[rowIndex].Cells[7].Value = customerCount.Late11;
                    worksheet.Rows[rowIndex].Cells[8].Value = customerCount.Total;
                    rowIndex++;
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to fill Late Order Summary Report.");
            }
        }

        private void AddCellColor(bool colorFlag, WorksheetCell cell)
        {
            var backgroundColorInfo = colorFlag ? new WorkbookColorInfo(System.Drawing.Color.FromArgb(161, 200, 247)) : new WorkbookColorInfo(System.Drawing.Color.White);

            cell.CellFormat.Fill = new CellFillPattern(backgroundColorInfo, null, FillPatternStyle.Solid);
        }

        private void AddCellBorders(WorksheetCell cell)
        {
            try
            {
                cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
                cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
                cell.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
                cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add cell borders.");
            }
        }

        private void AddExcelLateOrderHeader(Worksheet worksheet, int rowIndex)
        {
            var colIndex = 0;
            try
            {
                CreateHeaderCell(worksheet, rowIndex, colIndex++, "WO", 20);
                CreateHeaderCell(worksheet, rowIndex, colIndex++, "DateRequired", 20);
                CreateHeaderCell(worksheet, rowIndex, colIndex++, "Est Ship Date", 20);
                CreateHeaderCell(worksheet, rowIndex, colIndex++, "Priority", 20);
                CreateHeaderCell(worksheet, rowIndex, colIndex++, "Part Quantity", 20);
                CreateHeaderCell(worksheet, rowIndex, colIndex++, "Price", 20);
                CreateHeaderCell(worksheet, rowIndex, colIndex++, "Working Days Late", 30);
                CreateHeaderCell(worksheet, rowIndex, colIndex, "Serial Number(s)", 30);
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add late order header.");
            }
        }

        private int AddExcelLateOrderSummaryHeader(Worksheet worksheet, int rowIndex)
        {
            var now = DateTime.Now;

            int startColumn = 0;
            CreateHeaderCell(worksheet, rowIndex, 0, "");
            CreateMergedHeader(worksheet, rowIndex, 1, rowIndex, 3, "Approaching Due Date");
            CreateHeaderCell(worksheet, rowIndex, 4, "Today", 20);
            CreateMergedHeader(worksheet, rowIndex, 5, rowIndex, 7, "Days Late");
            CreateHeaderCell(worksheet, rowIndex, 8, "");
            rowIndex++;
            CreateHeaderCell(worksheet, rowIndex, startColumn++, GroupByName, 50);

            for (int i = 3; i > 0; i--) //Always show 3 days before Today since Lead days was removed.
            {
                DateTime preDays = now.AddBusinessDays(i);
                CreateHeaderCell(worksheet, rowIndex, startColumn++, preDays.ToShortDateString(), 20);
            }

            CreateHeaderCell(worksheet, rowIndex, startColumn++, now.ToShortDateString(), 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "1-5 Days Late", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "6-10 Days Late", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "11+ Days Late", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn, "Total Late", 20);
            rowIndex++;
            return rowIndex;
        }

        private void AddExcelSummaryRow(string customerPriceSummary, int customerOrderCount, int customerPartCount, int avgDaysLate, Worksheet worksheet, int rowIndex)
        {
            try
            {
                var backgroundColorInfo = new WorkbookColorInfo(System.Drawing.Color.FromArgb(211, 211, 211));

                WorksheetMergedCellsRegion totalsRegion = CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 1, "Totals");
                totalsRegion.CellFormat.Fill = new CellFillPattern(backgroundColorInfo, null, FillPatternStyle.Solid);
                totalsRegion.CellFormat.Alignment = HorizontalCellAlignment.Center;

                WorksheetMergedCellsRegion lateOrdersRegion = CreateMergedCell(worksheet, rowIndex, 2, rowIndex, 3, "Late Orders: " + customerOrderCount);
                lateOrdersRegion.CellFormat.Fill = new CellFillPattern(backgroundColorInfo, null, FillPatternStyle.Solid);
                lateOrdersRegion.CellFormat.Alignment = HorizontalCellAlignment.Center;

                WorksheetCell partCountRegion = CreateCell(worksheet, rowIndex, 4, customerPartCount.ToString());
                partCountRegion.CellFormat.Fill = new CellFillPattern(backgroundColorInfo, null, FillPatternStyle.Solid);
                partCountRegion.CellFormat.Alignment = HorizontalCellAlignment.Center;

                WorksheetCell customerPriceRegion = CreateCell(worksheet, rowIndex, 5, customerPriceSummary);
                customerPriceRegion.CellFormat.Fill = new CellFillPattern(backgroundColorInfo, null, FillPatternStyle.Solid);
                customerPriceRegion.CellFormat.Alignment = HorizontalCellAlignment.Center;

                WorksheetCell avgDaysLateRegion = CreateCell(worksheet, rowIndex, 6, avgDaysLate.ToString());
                avgDaysLateRegion.CellFormat.Fill = new CellFillPattern(backgroundColorInfo, null, FillPatternStyle.Solid);
                avgDaysLateRegion.CellFormat.Alignment = HorizontalCellAlignment.Center;

                WorksheetCell snRegion = CreateCell(worksheet, rowIndex, 7, String.Empty);
                snRegion.CellFormat.Fill = new CellFillPattern(backgroundColorInfo, null, FillPatternStyle.Solid);
                snRegion.CellFormat.Alignment = HorizontalCellAlignment.Center;
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add customer summary row.");
            }
        }

        private void AddExcelOrderRow(Worksheet worksheet, OrderData custRow, int rowIndex, int startColumn)
        {
            try
            {
                worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
                worksheet.Rows[rowIndex].Cells[startColumn++].Value = custRow.WO;
                worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;

                if (custRow.ReqDate.HasValue)
                {
                    worksheet.Rows[rowIndex].Cells[startColumn++].Value =
                        custRow.ReqDate.Value.Date;
                }
                else
                {
                    worksheet.Rows[rowIndex].Cells[startColumn++].Value = "NA";
                }

                worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
                worksheet.Rows[rowIndex].Cells[startColumn++].Value = custRow.EstShipDate.Date;
                worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
                worksheet.Rows[rowIndex].Cells[startColumn++].Value = custRow.Priority;
                worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
                worksheet.Rows[rowIndex].Cells[startColumn++].Value = custRow.PartQuantity;
                worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
                worksheet.Rows[rowIndex].Cells[startColumn++].Value = custRow.Price;
                worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
                worksheet.Rows[rowIndex].Cells[startColumn++].Value = custRow.WorkingDaysLate;
                
                if (custRow.SerialNumbers != null)
                {
                    var serialNumbers = custRow.SerialNumbers.Select(s => s.IsNumberNull() ? string.Empty : s.Number);

                    worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Left;
                    worksheet.Rows[rowIndex].Cells[startColumn].Value = string.Join(", ", serialNumbers);
                }

            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        #endregion

        #region ReportType

        public enum GroupByType
        {
            Customer,
            Department,
            ProductClass
        }

        #endregion

        #region OrderData

        private class OrderData
        {
            #region Properties

            public string CustomerName { get; set; }
            public int WO { get; set; }
            public DateTime? ReqDate { get; set; }
            public DateTime EstShipDate { get; set; }
            public string Priority { get; set; }
            public int PartQuantity { get; set; }
            public decimal Price { get; set; }
            public int WorkingDaysLate { get; set; }
            public string CurrentLocation { get; set; }

            public OrdersReport.OrderSerialNumberRow[] SerialNumbers { get; set; }

            public string ProductClass { get; set; }

            #endregion

            #region Methods

            public static OrderData From(OrdersReport.OrderRow item)
            {
                if (item == null)
                {
                    return null;
                }

                var productClassRow = item.GetOrderProductClassRows().FirstOrDefault();

                var productClass = productClassRow == null || productClassRow.IsProductClassNull()
                    ? DEFAULT_PRODUCT_CLASS
                    : productClassRow.ProductClass;

                return new OrderData
                {
                    CustomerName = item.CustomerName,
                    WO = item.OrderID,
                    ReqDate = item.IsRequiredDateNull() ? (DateTime?)null : item.RequiredDate,
                    EstShipDate = item.EstShipDate,
                    Priority = item.Priority,
                    PartQuantity = item.IsPartQuantityNull() ? 0 : item.PartQuantity,
                    Price = GetPrice(item),
                    WorkingDaysLate = GetNumDaysLate(item),
                    CurrentLocation = item.CurrentLocation,
                    SerialNumbers = item.GetOrderSerialNumberRows(),
                    ProductClass = productClass
                };
            }


            private static decimal GetPrice(OrdersReport.OrderRow order)
            {
                try
                {
                    decimal price = 0;
                    if (!order.IsBasePriceNull() || !order.IsPriceUnitNull() || !order.IsPartQuantityNull())
                    {
                        decimal weight = order.IsWeightNull() ? 0M : order.Weight;
                        var basePrice = order.IsBasePriceNull() ? 0M : order.BasePrice;
                        var partQuantity = order.IsPartQuantityNull() ? 0 : order.PartQuantity;

                        decimal fees = OrderPrice.CalculateFees(order, basePrice);
                        price = OrderPrice.CalculatePrice(basePrice, order.PriceUnit, fees, partQuantity, weight);
                    }
                    return price;
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, "Unable to determine order price.");
                    return 0;
                }
            }

            private static int GetNumDaysLate(OrdersReport.OrderRow order)
            {
                int daysLate = 0;
                try
                {
                    if (!order.IsEstShipDateNull())
                        daysLate = DateUtilities.GetBusinessDays(order.EstShipDate, DateTime.Now);
                    return daysLate;
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, "Unable to determine order working days late.");
                    return 0;
                }
            }

            #endregion
        }

        #endregion

        #region OrderSummary

        private class OrderSummary
        {
            #region Properties

            public string CustomerName { get; private set; }
            public string CurrentLocation { get; set; }
            public DateTime EstShipDate { get; private set; }
            public string ProductClass { get; private set; }

            #endregion

            #region Methods

            public static OrderSummary NewSummaryRow(OrdersReport.OrderRow item)
            {
                if (item == null)
                {
                    return null;
                }

                var productClassRow = item.GetOrderProductClassRows().FirstOrDefault();
                string productClass;

                if (productClassRow == null || productClassRow.IsProductClassNull())
                {
                    productClass = DEFAULT_PRODUCT_CLASS;
                }
                else
                {
                    productClass = productClassRow.ProductClass;
                }

                return new OrderSummary
                {
                    CustomerName = item.CustomerName,
                    EstShipDate = item.EstShipDate,
                    CurrentLocation = item.CurrentLocation,
                    ProductClass = productClass
                };
            }

            #endregion
        }

        #endregion

        #region OrderCounts

        private class OrderCounts
        {
            #region Properties

            public string Name { get; }

            public int Lead1 { get; private set; }

            public int Lead2 { get; private set; }

            public int  Lead3 { get; private set; }

            public int Today { get; private set; }

            public int Late5 { get; private set; }

            public int Late10 { get; private set; }

            public int Late11 { get; private set; }

            public int Total => Late5 + Late10 + Late11;

            #endregion

            #region Methods

            private OrderCounts(string customerName)
            {
                Name = customerName;
            }

            public static List<OrderCounts> GetCustomerCounts(IEnumerable<OrderSummary> orders)
            {
                if (orders == null)
                {
                    return new List<OrderCounts>();
                }

                var counts = new List<OrderCounts>();
                var now = DateTime.Now;
                foreach (var order in orders)
                {
                    var count = counts.FirstOrDefault(c => c.Name == order.CustomerName);

                    if (count == null)
                    {
                        count = new OrderCounts(order.CustomerName);
                        counts.Add(count);
                    }

                    count.Add(order, now);
                }

                return counts;
            }

            public static List<OrderCounts> GetDepartmentCounts(IEnumerable<OrderSummary> orders)
            {
                if (orders == null)
                {
                    return new List<OrderCounts>();
                }

                var counts = new List<OrderCounts>();
                var now = DateTime.Now;
                foreach (var order in orders)
                {
                    var count = counts.FirstOrDefault(c => c.Name == order.CurrentLocation);

                    if (count == null)
                    {
                        count = new OrderCounts(order.CurrentLocation);
                        counts.Add(count);
                    }

                    count.Add(order, now);
                }

                return counts;
            }

            public static List<OrderCounts> GetProductClassCounts(IEnumerable<OrderSummary> orders)
            {
                if (orders == null)
                {
                    return new List<OrderCounts>();
                }

                var counts = new List<OrderCounts>();
                var now = DateTime.Now;
                foreach (var order in orders)
                {
                    var count = counts.FirstOrDefault(c => c.Name == order.ProductClass);

                    if (count == null)
                    {
                        count = new OrderCounts(order.ProductClass);
                        counts.Add(count);
                    }

                    count.Add(order, now);
                }

                return counts;
            }

            private void Add(OrderSummary order, DateTime now)
            {
                if (order.EstShipDate.Date == now.Date)
                {
                    Today++;
                }
                else if (order.EstShipDate.Date < now.Date)
                {
                    if (order.EstShipDate.Date > now.AddBusinessDays(-6).Date)
                    {
                        Late5++;
                    }
                    else if (order.EstShipDate.Date < now.AddBusinessDays(-5).Date &&
                             order.EstShipDate.Date > now.AddBusinessDays(-11).Date)
                    {
                        Late10++;
                    }
                    else if (order.EstShipDate.Date <= now.AddBusinessDays(-11).Date)
                    {
                        Late11++;
                    }
                }
                else if (order.EstShipDate.Date > now.Date)
                {
                    if (order.EstShipDate.Date < now.AddBusinessDays(2))
                    {
                        Lead1++;
                    }
                    else if (order.EstShipDate.Date > now.AddBusinessDays(1) && order.EstShipDate.Date < now.AddBusinessDays(3))
                    {
                        Lead2++;
                    }
                    else if (order.EstShipDate.Date > now.AddBusinessDays(2) && order.EstShipDate.Date < now.AddBusinessDays(4))
                    {
                        Lead3++;
                    }
                }
            }

            #endregion
        }


        #endregion

        #region OrdersSummary

        private class OrdersSummary
        {
            #region Properties

            public decimal Price { get; set; }
            public int Orders { get; set; }
            public int Parts { get; set; }
            public int TotalDaysLate { get; set; }

            public int AverageDaysLate
            {
                get
                {
                    var avgDaysLate = 0;
                    if (Orders > 0)
                    {
                        avgDaysLate = (TotalDaysLate/Orders);
                    }

                    return avgDaysLate;
                }
            }

            #endregion

            #region Methods

            public void AddRow(OrderData row)
            {
                TotalDaysLate += row.WorkingDaysLate;
                Price += row.Price;
                Orders += 1;
                Parts += row.PartQuantity;
            }

            #endregion
        }

        #endregion
    }

    public class CurrentOrderStatusReport : ExcelBaseReport
    {
        #region Fields

        private const int RECEIVED_LEAD_DAYS = 3;
        private const int INHOUSE_COLUMN_COUNT = 14;
        private const int AWAITING_COLUMN_COUNT = 8;
        private const int SHIPPED_COLUMN_COUNT = 11;
        private const int RECEIVED_COLUMN_COUNT = 10;
        private const int ISSUES_COLUMN_COUNT = 12;

        /// <summary>
        /// Customer ID that indicates that a specific customer was not selected.
        /// </summary>
        /// <remarks>
        /// Client may use this as a Customer ID when the user selects
        /// 'All Customers'.
        /// </remarks>
        private const int NO_CUSTOMER_ID = -1;

        #endregion

        #region Properties

        public override string Title => "Order Status";

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation => PageOrientation.Landscape;

        [Browsable(true)]
        [Description("The starting date of the report.")]
        [DisplayName("From Date")]
        [Category("Report")]
        public DateTime FromDate { get; set; }

        [Browsable(true)]
        [Description("The ending date of the report.")]
        [DisplayName("To Date")]
        [Category("Report")]
        public DateTime ToDate { get; set; }

        /// <summary>
        /// Gets or sets the customer IDs for this instance.
        /// </summary>
        public List<int> CustomerIds { get; set; }

        protected override int FilenameIdentifier => CustomerIds == null || CustomerIds.Count == 0
            ? NO_CUSTOMER_ID
            : CustomerIds.First();

        #endregion

        #region Methods

        public CurrentOrderStatusReport(DateTime fromDate, DateTime toDate, List<int> customerIds)
        {
            this.FromDate = fromDate.Date;
            this.ToDate = toDate.EndOfDay();
            CustomerIds = new List<int>(customerIds);
        }

        public CurrentOrderStatusReport()
        {
            this.FromDate = DateTime.Now.Date;
            this.ToDate = this.FromDate.EndOfDay();
            CustomerIds = new List<int> { Settings.Default.LastReportCustomerID };
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            string loggingOutput;

            var firstCustomerId = CustomerIds == null || CustomerIds.Count == 0
                ? NO_CUSTOMER_ID
                : CustomerIds.First();

            if (firstCustomerId == NO_CUSTOMER_ID)
            {
                loggingOutput = "Running report for all customers.";
            }
            else if (CustomerIds.Count == 1)
            {
                loggingOutput = $"Running report for customer {firstCustomerId}";
            }
            else
            {
                loggingOutput = $"Running report for customers {string.Join(", ", CustomerIds)}";
            }

            _log.Debug(loggingOutput);

            //ensure to get correct time with same date
            this.FromDate = this.FromDate.Date;
            this.ToDate = this.ToDate.EndOfDay();

            using (var persistence = new CurrentOrderStatusPersistence(ApplicationSettings.Current))
            {
                var customerIds = new List<int>(CustomerIds ?? Enumerable.Empty<int>());

                CreateWorkBook();

                // This report loads a large amount of data, and wrapping code
                // in this manner may allow earlier garbage collection of
                // the large lists used by this method.
                //
                // This code saves the count of each list for later use.
                int receivedCount;
                {
                    var received = persistence.GetReceivedData(customerIds, FromDate, ToDate);
                    CreateOrdersReceivedReport(received);
                    receivedCount = received.Count;
                }

                int inHouseCount;
                {
                    var inHouse = persistence.GetInHouseData(customerIds, FromDate, ToDate);
                    CreateInHouseReport(inHouse);
                    inHouseCount = inHouse.Count;
                }

                int awaitingShipmentCount;
                {
                    var awaitingShipment = persistence.GetAwaitingShipmentData(customerIds, FromDate, ToDate);
                    CreateAwaitingShipmentReport(awaitingShipment);
                    awaitingShipmentCount = awaitingShipment.Count;
                }

                int shippedCount;
                {
                    var shipped = persistence.GetShippedData(customerIds, FromDate, ToDate);
                    CreateOrdersShippedReport(shipped);
                    shippedCount = shipped.Count;
                }

                int issuesCount;
                {
                    var issues = persistence.GetIssuesData(customerIds, FromDate, ToDate);
                    CreateIssuesReport(issues);
                    issuesCount = issues.Count;
                }

                HasData = receivedCount > 0
                    || inHouseCount > 0
                    || awaitingShipmentCount > 0
                    || shippedCount > 0
                    || issuesCount > 0;
            }
        }

        private void CreateDateHeader(Worksheet worksheet, int rowIndex, int lastColumn)
        {
            CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, lastColumn, "Date : " + this.FromDate.ToShortDateString() + " to " + this.ToDate.ToShortDateString()).CellFormat.Alignment = HorizontalCellAlignment.Center;
        }

        #region In House Report

        private void CreateInHouseReport(List<CurrentOrderStatusPersistence.OrderInfo> orderInfos)
        {
            Worksheet inHouseWks = CreateWorksheet("In-House");
            int rowIndex = 0;

            inHouseWks.GetCell("R1C1", CellReferenceMode.R1C1).Comment = new WorksheetCellComment { Text = new FormattedString("All orders currently in process"), Visible = false };

            CreateHeaderFooter(inHouseWks, "Orders In-House");
            CreateDateHeader(inHouseWks, rowIndex, INHOUSE_COLUMN_COUNT);
            rowIndex++;

            if (orderInfos.Count > 0)
            {
                rowIndex = AddInHouseHeader(inHouseWks, rowIndex);

                foreach (var item in orderInfos)
                    AddInHouseRow(inHouseWks, item, rowIndex++, 0);

                long totalPartQuantity;

                try
                {
                    totalPartQuantity = orderInfos.Sum(oi => (long)oi.PartQuantity);
                }
                catch (OverflowException)
                {
                    _log.Warn("Arithmetic overflow occurred while running report");
                    totalPartQuantity = long.MaxValue;
                }

                AddInHouseSummaryRow(orderInfos.Count, totalPartQuantity, inHouseWks, rowIndex);
            }
            else
            {
                AddInHouseHeader(inHouseWks, rowIndex++);
                AddEmptyInHouseRow(inHouseWks, rowIndex++, 0);
                AddInHouseSummaryRow(0, 0, inHouseWks, rowIndex);
            }
        }

        private int AddInHouseHeader(Worksheet worksheet, int rowIndex)
        {
            int startColumn = 0;
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Customer", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "WO", 10);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "PO", 10);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Status", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Customer WO", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Part", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Quantity", 12);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Current Location", 18);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Current Process", 30);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Process Start Date", 16);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Date Received", 18);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Est. Ship Date", 16);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Adjusted Ship Date", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Serial Number(s)", 30);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Product Class", 25);
            rowIndex++;

            return rowIndex;
        }

        private void AddInHouseRow(Worksheet worksheet, CurrentOrderStatusPersistence.OrderInfo orderInfo, int rowIndex, int startColumn)
        {
            try
            {
                var skipCells = new List<int>();
                int columnIndex = startColumn;

                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.CustomerName;
                if ((OrderType)orderInfo.OrderType == OrderType.Lost)
                {
                    worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Red), null, FillPatternStyle.Solid);
                    worksheet.Rows[rowIndex].Cells[startColumn].Comment = new WorksheetCellComment { Text = new FormattedString("Lost Order"), Visible = false };
                    skipCells.Add(startColumn);
                }
                else if ((OrderType)orderInfo.OrderType == OrderType.Quarantine)
                {
                    worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Red), null, FillPatternStyle.Solid);
                    worksheet.Rows[rowIndex].Cells[startColumn].Comment = new WorksheetCellComment { Text = new FormattedString("Quarantined Order"), Visible = false };
                    skipCells.Add(startColumn);
                }
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.OrderID;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PurchaseOrder;


                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.Status;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.CustomerWO;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PartDescription;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PartQuantity;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.CurrentLocation;

                if (!string.IsNullOrEmpty(orderInfo.CurrentProcess))
                {
                    worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.CurrentProcess;
                }
                else
                {
                    worksheet.Rows[rowIndex].Cells[columnIndex++].Value = string.Empty;
                }

                if (orderInfo.CurrentProcessStartDate.HasValue)
                {
                    worksheet.Rows[rowIndex].Cells[columnIndex++].Value =
                        orderInfo.CurrentProcessStartDate.Value;
                }
                else
                {
                    worksheet.Rows[rowIndex].Cells[columnIndex++].Value = string.Empty;
                }

                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.ReceivedDate.Date;
                if (orderInfo.EstShipDate != DateTime.MinValue)
                {
                    worksheet.Rows[rowIndex].Cells[columnIndex].Value = orderInfo.EstShipDate;
                }
                else
                {
                    worksheet.Rows[rowIndex].Cells[columnIndex].Value = string.Empty;
                }

                columnIndex++;

                if (orderInfo.AdjustedEstShipDate.HasValue)
                {
                    worksheet.Rows[rowIndex].Cells[columnIndex].Value = orderInfo.AdjustedEstShipDate;
                }
                else
                {
                    worksheet.Rows[rowIndex].Cells[columnIndex].Value = string.Empty;
                }

                columnIndex++;

                if (orderInfo.SerialNumbers != null)
                {
                    worksheet.Rows[rowIndex].Cells[columnIndex].Value = string.Join(", ", orderInfo.SerialNumbers);
                }

                columnIndex++;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.ProductClass;

                foreach (WorksheetCell cell in worksheet.Rows[rowIndex].Cells.Take(columnIndex))
                {
                    if (skipCells.Contains(cell.ColumnIndex))
                        continue;
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                    cell.CellFormat.Alignment = HorizontalCellAlignment.Center;
                    ApplyCellBorders(cell);
                }

                //Reset serial number and product class columns
                worksheet.Rows[rowIndex].Cells[INHOUSE_COLUMN_COUNT - 1].CellFormat.Alignment = HorizontalCellAlignment.Left;
                worksheet.Rows[rowIndex].Cells[INHOUSE_COLUMN_COUNT].CellFormat.Alignment = HorizontalCellAlignment.Left;
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        private void AddEmptyInHouseRow(Worksheet worksheet, int rowIndex, int startColumn)
        {
            try
            {
                WorksheetMergedCellsRegion region1 = CreateMergedHeader(worksheet, rowIndex, startColumn, rowIndex, startColumn + INHOUSE_COLUMN_COUNT, "None");
                region1.CellFormat.Alignment = HorizontalCellAlignment.Center;
                region1.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                ApplyCellBorders(region1.CellFormat);
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        private void AddInHouseSummaryRow(int orderCount, long totalPartCount, Worksheet worksheet, int rowIndex)
        {
            CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 4, "Total: ");
            CreateMergedHeader(worksheet, rowIndex, 5, rowIndex, 9, "Total Orders: " + orderCount.ToString("N0")).CellFormat.Alignment = HorizontalCellAlignment.Center;
            CreateMergedHeader(worksheet, rowIndex, 10, rowIndex, 14, "Total Parts: " + totalPartCount.ToString("N0")).CellFormat.Alignment = HorizontalCellAlignment.Center;
        }

        #endregion

        #region Awaiting Shipment Report

        private void CreateAwaitingShipmentReport(List<CurrentOrderStatusPersistence.OrderInfo> orderInfos)
        {
            Worksheet shipmentWks = CreateWorksheet("Awaiting Shipment");
            int rowIndex = 0;

            shipmentWks.GetCell("R1C1", CellReferenceMode.R1C1).Comment = new WorksheetCellComment { Text = new FormattedString("All orders completed, but awaiting to be picked up by the shipping carrier"), Visible = false };

            CreateHeaderFooter(shipmentWks, "Orders Awaiting Shipment");
            CreateDateHeader(shipmentWks, rowIndex, AWAITING_COLUMN_COUNT);
            rowIndex++;

            if (orderInfos.Count > 0)
            {
                rowIndex = AddAwaitingShipmentHeader(shipmentWks, rowIndex);

                foreach (var item in orderInfos)
                {
                    AddAwaitingShipmentRow(shipmentWks, item, rowIndex, 0);
                    rowIndex++;
                }

                long totalPartQuantity;

                try
                {
                    totalPartQuantity = orderInfos.Sum(oi => (long)oi.PartQuantity);
                }
                catch (OverflowException)
                {
                    _log.Warn("Arithmetic overflow occurred while running report");
                    totalPartQuantity = long.MaxValue;
                }

                AddAwaitingShipmentSummaryRow(orderInfos.Count, totalPartQuantity, shipmentWks, rowIndex);
            }
            else
            {
                AddAwaitingShipmentHeader(shipmentWks, rowIndex++);
                AddEmptyAwaitingShipmentRow(shipmentWks, rowIndex++, 0);
                AddAwaitingShipmentSummaryRow(0, 0, shipmentWks, rowIndex);
            }
        }

        private int AddAwaitingShipmentHeader(Worksheet worksheet, int rowIndex)
        {
            int startColumn = 0;
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Customer", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "WO", 10);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "COC", 10);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "PO", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Customer WO", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Part", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Quantity", 12);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Serial Number(s)", 30);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Product Class", 25);
            rowIndex++;
            return rowIndex;
        }

        private void AddAwaitingShipmentRow(Worksheet worksheet, CurrentOrderStatusPersistence.OrderInfo orderInfo, int rowIndex, int startColumn)
        {
            try
            {
                var skipCells = new List<int>();
                int columnIndex = startColumn;

                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.CustomerName;
                if ((OrderType)orderInfo.OrderType == OrderType.Lost)
                {
                    worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Red), null, FillPatternStyle.Solid);
                    worksheet.Rows[rowIndex].Cells[startColumn].Comment = new WorksheetCellComment { Text = new FormattedString("Lost Order"), Visible = false };
                    skipCells.Add(startColumn);
                }
                else if ((OrderType)orderInfo.OrderType == OrderType.Quarantine)
                {
                    worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Red), null, FillPatternStyle.Solid);
                    worksheet.Rows[rowIndex].Cells[startColumn].Comment = new WorksheetCellComment { Text = new FormattedString("Quarantined Order"), Visible = false };
                    skipCells.Add(startColumn);
                }
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.OrderID;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.COC;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PurchaseOrder;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.CustomerWO;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PartDescription;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PartQuantity;

                if (orderInfo.SerialNumbers != null)
                {
                    worksheet.Rows[rowIndex].Cells[columnIndex].Value = string.Join(", ", orderInfo.SerialNumbers);
                }

                columnIndex++;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.ProductClass;

                foreach (WorksheetCell cell in worksheet.Rows[rowIndex].Cells.Take(columnIndex))
                {
                    if (skipCells.Contains(cell.ColumnIndex))
                        continue;
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                    cell.CellFormat.Alignment = HorizontalCellAlignment.Center;
                    ApplyCellBorders(cell);

                    //Reset serial num col alignment
                    worksheet.Rows[rowIndex].Cells[AWAITING_COLUMN_COUNT].CellFormat.Alignment = HorizontalCellAlignment.Left;
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        private void AddEmptyAwaitingShipmentRow(Worksheet worksheet, int rowIndex, int startColumn)
        {
            try
            {
                WorksheetMergedCellsRegion region1 = CreateMergedHeader(worksheet, rowIndex, startColumn, rowIndex, startColumn + AWAITING_COLUMN_COUNT, "None");
                region1.CellFormat.Alignment = HorizontalCellAlignment.Center;
                region1.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                ApplyCellBorders(region1.CellFormat);
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        private void AddAwaitingShipmentSummaryRow(int orderCount, long totalPartCount, Worksheet worksheet, int rowIndex)
        {
            CreateHeaderCell(worksheet, rowIndex, 0, "Total: ");
            CreateMergedHeader(worksheet, rowIndex, 1, rowIndex, 4, "Total Orders: " + orderCount.ToString("N0")).CellFormat.Alignment = HorizontalCellAlignment.Center;
            CreateMergedHeader(worksheet, rowIndex, 5, rowIndex, 8, "Total Parts: " + totalPartCount.ToString("N0")).CellFormat.Alignment = HorizontalCellAlignment.Center;
        }

        #endregion

        #region Shipped Report

        private void CreateOrdersShippedReport(List<CurrentOrderStatusPersistence.OrderInfo> orderInfos)
        {
            Worksheet shippedWks = CreateWorksheet("Shipped");
            int rowIndex = 0;

            shippedWks.GetCell("R1C1", CellReferenceMode.R1C1).Comment = new WorksheetCellComment { Text = new FormattedString("All orders shipped on the date specified"), Visible = false };

            CreateHeaderFooter(shippedWks, "Orders Shipped");
            CreateDateHeader(shippedWks, rowIndex, SHIPPED_COLUMN_COUNT);
            rowIndex++;

            if (orderInfos.Count > 0)
            {
                rowIndex = AddShippedHeader(shippedWks, rowIndex);

                foreach (var item in orderInfos)
                    AddShippedRow(shippedWks, item, rowIndex++, 0);

                double? avgLeadDays = orderInfos.Average(of => of.LeadDays);

                long partQuantityTotal;

                try
                {
                    partQuantityTotal = orderInfos.Sum(of => (long)of.PartQuantity);
                }
                catch (OverflowException)
                {
                    _log.Warn("Arithmetic overflow occurred while running report");
                    partQuantityTotal = long.MaxValue;
                }

                AddShippedSummaryRow(orderInfos.Count, partQuantityTotal, Convert.ToInt32(avgLeadDays.GetValueOrDefault()), shippedWks, rowIndex);
            }
            else
            {
                AddShippedHeader(shippedWks, rowIndex++);
                AddEmptyShippedRow(shippedWks, rowIndex++, 0);
                AddShippedSummaryRow(0, 0, 0, shippedWks, rowIndex);
            }
        }

        private int AddShippedHeader(Worksheet worksheet, int rowIndex)
        {
            int startColumn = 0;

            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Customer", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "WO", 10);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "COC", 10);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "PO", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Customer WO", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Part", 18);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Quantity", 10);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Lead Days", 12);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Carrier", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Tracking Number", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Serial Number(s)", 30);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Product Class", 25);
            rowIndex++;

            return rowIndex;
        }

        private void AddShippedRow(Worksheet worksheet, CurrentOrderStatusPersistence.OrderInfo orderInfo, int rowIndex, int startColumn)
        {
            try
            {
                var skipCells = new List<int>();
                int columnIndex = startColumn;

                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.CustomerName;
                if ((OrderType)orderInfo.OrderType == OrderType.Lost)
                {
                    worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Red), null, FillPatternStyle.Solid);
                    worksheet.Rows[rowIndex].Cells[startColumn].Comment = new WorksheetCellComment { Text = new FormattedString("Lost Order"), Visible = false };
                    skipCells.Add(startColumn);
                }
                else if ((OrderType)orderInfo.OrderType == OrderType.Quarantine)
                {
                    worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Red), null, FillPatternStyle.Solid);
                    worksheet.Rows[rowIndex].Cells[startColumn].Comment = new WorksheetCellComment { Text = new FormattedString("Quarantined Order"), Visible = false };
                    skipCells.Add(startColumn);
                }
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.OrderID;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.COC;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PurchaseOrder;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.CustomerWO;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PartDescription;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PartQuantity;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.LeadDays;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.ShippingCarrer;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.TrackingNumber;

                if (orderInfo.SerialNumbers != null)
                {
                    worksheet.Rows[rowIndex].Cells[columnIndex].Value = string.Join(", ", orderInfo.SerialNumbers);

                    worksheet.Rows[rowIndex].Cells[columnIndex].CellFormat.Alignment = HorizontalCellAlignment.Left;
                }

                columnIndex++;
                worksheet.Rows[rowIndex].Cells[columnIndex].Value = orderInfo.ProductClass;
                worksheet.Rows[rowIndex].Cells[columnIndex++].CellFormat.Alignment = HorizontalCellAlignment.Left;

                foreach (WorksheetCell cell in worksheet.Rows[rowIndex].Cells.Take(columnIndex))
                {
                    if (skipCells.Contains(cell.ColumnIndex))
                        continue;
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                    cell.CellFormat.Alignment = HorizontalCellAlignment.Center;
                    ApplyCellBorders(cell);

                    //Reset serial num col alignment
                    worksheet.Rows[rowIndex].Cells[SHIPPED_COLUMN_COUNT].CellFormat.Alignment = HorizontalCellAlignment.Left;
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        private void AddShippedSummaryRow(int orderCount, long totalPartCount, int avgLeadDays, Worksheet worksheet, int rowIndex)
        {
            CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 1, "Totals: ").CellFormat.Alignment = HorizontalCellAlignment.Center;
            CreateMergedHeader(worksheet, rowIndex, 2, rowIndex, 4, "Total Orders: " + orderCount.ToString("N0")).CellFormat.Alignment = HorizontalCellAlignment.Center;
            CreateMergedHeader(worksheet, rowIndex, 5, rowIndex, 7, "Avg. Lead Days: " + avgLeadDays.ToString("N0")).CellFormat.Alignment = HorizontalCellAlignment.Center;
            CreateMergedHeader(worksheet, rowIndex, 8, rowIndex, 11, "Total Parts: " + totalPartCount.ToString("N0")).CellFormat.Alignment = HorizontalCellAlignment.Center;
        }

        private void AddEmptyShippedRow(Worksheet worksheet, int rowIndex, int startColumn)
        {
            try
            {
                WorksheetMergedCellsRegion region1 = CreateMergedHeader(worksheet, rowIndex, startColumn, rowIndex, startColumn + SHIPPED_COLUMN_COUNT, "None");
                region1.CellFormat.Alignment = HorizontalCellAlignment.Center;
                region1.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                ApplyCellBorders(region1.CellFormat);
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        #endregion

        #region Received Report

        private void CreateOrdersReceivedReport(List<CurrentOrderStatusPersistence.OrderInfo> orderInfos)
        {
            Worksheet receivedWks = CreateWorksheet("Received");
            int rowIndex = 0;

            receivedWks.GetCell("R1C1", CellReferenceMode.R1C1).Comment = new WorksheetCellComment { Text = new FormattedString("All orders received on the date specified"), Visible = false };

            CreateHeaderFooter(receivedWks, "Orders Received");
            CreateDateHeader(receivedWks, rowIndex, RECEIVED_COLUMN_COUNT - 1);
            rowIndex++;

            if (orderInfos.Count > 0)
            {
                rowIndex = AddReceivedHeader(receivedWks, rowIndex);

                foreach (var item in orderInfos)
                    AddReceivedRow(receivedWks, item, rowIndex++, 0);

                long totalPartQuantity;

                try
                {
                    totalPartQuantity = orderInfos.Sum(oi => (long)oi.PartQuantity);
                }
                catch (OverflowException)
                {
                    _log.Warn("Arithmetic overflow occurred while running report");
                    totalPartQuantity = long.MaxValue;
                }


                AddReceivedSummaryRow(orderInfos.Count, totalPartQuantity, receivedWks, rowIndex);
            }
            else
            {
                AddReceivedHeader(receivedWks, rowIndex++);
                AddEmptyReceivedRow(receivedWks, rowIndex++, 0);
                AddReceivedSummaryRow(0, 0, receivedWks, rowIndex);
            }
        }

        private int AddReceivedHeader(Worksheet worksheet, int rowIndex)
        {
            int startColumn = 0;
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Customer", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "WO", 10);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "PO", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Customer WO", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Part", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Quantity", 12);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Est. Ship Date", 16);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Adjusted Ship Date", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Serial Number(s)", 30);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Product Class", 25);
            rowIndex++;
            return rowIndex;
        }

        private void AddReceivedRow(Worksheet worksheet, CurrentOrderStatusPersistence.OrderInfo orderInfo, int rowIndex, int startColumn)
        {
            try
            {
                var skipCells = new List<int>();

                worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
                worksheet.Rows[rowIndex].Cells[startColumn++].Value = orderInfo.CustomerName;

                worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
                if ((OrderType)orderInfo.OrderType == OrderType.Lost)
                {
                    worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Red), null, FillPatternStyle.Solid);
                    worksheet.Rows[rowIndex].Cells[startColumn].Comment = new WorksheetCellComment { Text = new FormattedString("Lost Order"), Visible = false };
                    skipCells.Add(startColumn);
                }
                else if ((OrderType)orderInfo.OrderType == OrderType.Quarantine)
                {
                    worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Red), null, FillPatternStyle.Solid);
                    worksheet.Rows[rowIndex].Cells[startColumn].Comment = new WorksheetCellComment { Text = new FormattedString("Quarantined Order"), Visible = false };
                    skipCells.Add(startColumn);
                }
                worksheet.Rows[rowIndex].Cells[startColumn++].Value = orderInfo.OrderID;

                worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
                worksheet.Rows[rowIndex].Cells[startColumn++].Value = orderInfo.PurchaseOrder;
                worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
                worksheet.Rows[rowIndex].Cells[startColumn++].Value = orderInfo.CustomerWO;
                worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
                worksheet.Rows[rowIndex].Cells[startColumn++].Value = orderInfo.PartDescription;
                worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
                worksheet.Rows[rowIndex].Cells[startColumn++].Value = orderInfo.PartQuantity;
                worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
                if (orderInfo.EstShipDate != DateTime.MinValue)
                {
                    worksheet.Rows[rowIndex].Cells[startColumn].Value = orderInfo.EstShipDate;
                }
                else
                {
                    worksheet.Rows[rowIndex].Cells[startColumn].Value = string.Empty;
                }

                startColumn++;

                if (orderInfo.AdjustedEstShipDate.HasValue)
                {
                    worksheet.Rows[rowIndex].Cells[startColumn].Value = orderInfo.AdjustedEstShipDate.Value;
                }
                else
                {
                    worksheet.Rows[rowIndex].Cells[startColumn].Value = string.Empty;
                }

                startColumn++;

                if(orderInfo.SerialNumbers != null)
                {
                    worksheet.Rows[rowIndex].Cells[startColumn].Value = string.Join(", ", orderInfo.SerialNumbers);

                    worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Left;
                }

                startColumn++;
                worksheet.Rows[rowIndex].Cells[startColumn].Value = orderInfo.ProductClass;
                worksheet.Rows[rowIndex].Cells[startColumn++].CellFormat.Alignment = HorizontalCellAlignment.Left;

                int count = 0;
                foreach (WorksheetCell cell in worksheet.Rows[rowIndex].Cells)
                {
                    if (count > RECEIVED_COLUMN_COUNT || skipCells.Contains(cell.ColumnIndex))
                        continue;
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                    ApplyCellBorders(cell);
                    count++;
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        private void AddEmptyReceivedRow(Worksheet worksheet, int rowIndex, int startColumn)
        {
            try
            {
                for (var columnIndex = startColumn; columnIndex < RECEIVED_COLUMN_COUNT; ++columnIndex)
                {
                    worksheet.Rows[rowIndex].Cells[columnIndex].CellFormat.Alignment = HorizontalCellAlignment.Center;
                    worksheet.Rows[rowIndex].Cells[columnIndex].Value = "None";
                }

                int count = 0;
                foreach (WorksheetCell cell in worksheet.Rows[rowIndex].Cells)
                {
                    if (count > RECEIVED_COLUMN_COUNT)
                        break;
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                    ApplyCellBorders(cell);
                    count++;
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        private void AddReceivedSummaryRow(int orderCount, long totalPartCount, Worksheet worksheet, int rowIndex)
        {
            CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 3, "Total: ");
            CreateMergedHeader(worksheet, rowIndex, 4, rowIndex, 5, "Total Orders: " + orderCount.ToString("N0")).CellFormat.Alignment = HorizontalCellAlignment.Center;
            CreateMergedHeader(worksheet, rowIndex, 6, rowIndex, 9, "Total Parts: " + totalPartCount.ToString("N0")).CellFormat.Alignment = HorizontalCellAlignment.Center;
        }

        #endregion

        #region Issues Report

        private void CreateIssuesReport(List<CurrentOrderStatusPersistence.IssueInfo> issues)
        {
            try
            {
                var worksheet = this.CreateWorksheet("Issues");
                var rowIndex = 0;

                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, ISSUES_COLUMN_COUNT, "Date(s): " + FromDate.ToShortDateString() + " to " + ToDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the column headers
                CreateHeaderCell(worksheet, rowIndex, 0, "WO");
                CreateHeaderCell(worksheet, rowIndex, 1, "Type");
                CreateHeaderCell(worksheet, rowIndex, 2, "Original WO");
                CreateHeaderCell(worksheet, rowIndex, 3, "Customer");
                CreateHeaderCell(worksheet, rowIndex, 4, "PO");
                CreateHeaderCell(worksheet, rowIndex, 5, "Department");
                CreateHeaderCell(worksheet, rowIndex, 6, "Est. Ship Date");
                CreateHeaderCell(worksheet, rowIndex, 7, "Adjusted Ship Date");
                CreateHeaderCell(worksheet, rowIndex, 8, "Reason");
                CreateHeaderCell(worksheet, rowIndex, 9, "Process");
                CreateHeaderCell(worksheet, rowIndex, 10, "Part Qty");
                CreateHeaderCell(worksheet, rowIndex, 11, "Serial Number(s)");
                CreateHeaderCell(worksheet, rowIndex, 12, "Product Class");
                rowIndex++;

                if (issues.Count > 0)
                {
                    var startRowIndex = rowIndex;
                    foreach (var issue in issues)
                    {
                        switch (issue.OrderType)
                        {
                            case OrderType.Lost:
                                CreateCell(worksheet, rowIndex, 1, "Lost", false, HorizontalCellAlignment.Center);
                                break;
                            case OrderType.Quarantine:
                                CreateCell(worksheet, rowIndex, 1, "Quarantined", false, HorizontalCellAlignment.Center);
                                break;
                            default:
                                CreateCell(worksheet, rowIndex, 1, string.Empty, false, HorizontalCellAlignment.Center);

                                break;
                        }

                        CreateCell(worksheet, rowIndex, 3, issue.CustomerName, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 4, issue.PurchaseOrder, false, HorizontalCellAlignment.Center);

                        if (issue.EstShipDate.HasValue)
                        {
                            CreateCell(worksheet, rowIndex, 6, issue.EstShipDate, false, HorizontalCellAlignment.Center);
                        }
                        else
                        {
                            CreateCell(worksheet, rowIndex, 6, string.Empty, false, HorizontalCellAlignment.Center);
                        }

                        if (issue.AdjustedEstShipDate.HasValue)
                        {
                            CreateCell(worksheet, rowIndex, 7, issue.AdjustedEstShipDate.Value, false, HorizontalCellAlignment.Center);
                        }
                        else
                        {
                            CreateCell(worksheet, rowIndex, 7, string.Empty, false, HorizontalCellAlignment.Center);
                        }

                        CreateCell(worksheet, rowIndex, 8, issue.Reason, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 10, issue.PartQuantity.HasValue ? issue.PartQuantity.ToString() : "NA", false, HorizontalCellAlignment.Center);

                        CreateCell(worksheet, rowIndex, 11, string.Join(", ", issue.SerialNumbers), false, HorizontalCellAlignment.Left);
                        CreateCell(worksheet, rowIndex, 12, issue.ProductClass);

                        if (issue.ReworkOrderId.HasValue)
                        {
                            CreateCell(worksheet, rowIndex, 0, issue.ReworkOrderId, false, HorizontalCellAlignment.Center);
                        }
                        else
                        {
                            CreateCell(worksheet, rowIndex, 0, "NA", false, HorizontalCellAlignment.Center);
                        }

                        if (issue.OriginalOrderId.HasValue)
                        {
                            CreateCell(worksheet, rowIndex, 2, issue.OriginalOrderId, false, HorizontalCellAlignment.Center);

                        }
                        else
                        {
                            CreateCell(worksheet, rowIndex, 2, "NA", false, HorizontalCellAlignment.Center);
                        }

                        CreateCell(worksheet, rowIndex, 5, issue.HoldLocation, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 9, issue.Process, false, HorizontalCellAlignment.Center);

                        rowIndex++;
                    }

                    // Add the total count
                    var cell = this.CreateCell(worksheet, rowIndex, 0, "Total:");
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    cell = this.CreateFormulaCell(worksheet, rowIndex, 1, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                }
                else
                {
                    // Add 'None' row
                    this.CreateMergedCell(worksheet, rowIndex, 0, rowIndex, ISSUES_COLUMN_COUNT, "None", false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex++;

                    // set first to cells in the next row
                    var cell = this.CreateCell(worksheet, rowIndex, 0, "Total:");
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    cell = this.CreateCell(worksheet, rowIndex, 1, 0, false, HorizontalCellAlignment.Center);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                }

                this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 7, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 9, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 10, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 11, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 12, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);

                // Set column widths
                worksheet.Columns[0].SetWidth(10, WorksheetColumnWidthUnit.Character);
                worksheet.Columns[1].SetWidth(17, WorksheetColumnWidthUnit.Character);
                worksheet.Columns[2].SetWidth(17, WorksheetColumnWidthUnit.Character);
                worksheet.Columns[3].SetWidth(20, WorksheetColumnWidthUnit.Character);
                worksheet.Columns[4].SetWidth(8, WorksheetColumnWidthUnit.Character);
                worksheet.Columns[5].SetWidth(21, WorksheetColumnWidthUnit.Character);
                worksheet.Columns[6].SetWidth(16, WorksheetColumnWidthUnit.Character);
                worksheet.Columns[7].SetWidth(25, WorksheetColumnWidthUnit.Character);
                worksheet.Columns[8].SetWidth(25, WorksheetColumnWidthUnit.Character);
                worksheet.Columns[9].SetWidth(21, WorksheetColumnWidthUnit.Character);
                worksheet.Columns[10].SetWidth(12, WorksheetColumnWidthUnit.Character);
                worksheet.Columns[11].SetWidth(30, WorksheetColumnWidthUnit.Character);
                worksheet.Columns[12].SetWidth(25, WorksheetColumnWidthUnit.Character);
                worksheet.Columns[13].SetWidth(25, WorksheetColumnWidthUnit.Character);

            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to create 'Issues' Report.");
            }
        }

        #endregion Issues Report

        #endregion
    }

    public class OpenOrdersByCustomerReport : Report
    {
        #region Fields

        private DateTime _fromDate;
        private DateTime _toDate;
        private Workbook _workbook;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Open Orders By Customer"; }
        }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }

        [Browsable(true)]
        [Description("The from date of the report.")]
        [DisplayName("From Date")]
        [Category("Report")]
        public DateTime FromDate
        {
            get { return this._fromDate; }
            set { this._fromDate = value; }
        }

        [Browsable(true)]
        [Description("The to date of the report.")]
        [DisplayName("To Date")]
        [Category("Report")]
        public DateTime ToDate
        {
            get { return this._toDate; }
            set { this._toDate = value; }
        }

        [Browsable(true)]
        [TypeConverter(typeof(CustomerConverter))]
        [DisplayName("Customer")]
        [Category("Report")]
        public int CustomerID { get; set; }

        [Browsable(true)]
        [DisplayName("Report Type")]
        [Category("Report")]
        public ReportExportType ReportType { get; set; }

        protected override int FilenameIdentifier => CustomerID;

        #endregion

        #region Methods

        public OpenOrdersByCustomerReport()
        {
            this._fromDate = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            this._toDate = DateUtilities.GetLastDayOfMonth(DateTime.Now);

            CustomerID = Settings.Default.LastReportCustomerID;
        }

        public override void DisplayReport(CancellationToken cancellationToken)
        {
            try
            {
                string path = FileSystem.GetFolder(FileSystem.enumFolderType.Reports, true);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (!cancellationToken.IsCancellationRequested)
                {
                    //create it
                    CreateReport();
                }

                if (!cancellationToken.IsCancellationRequested)
                {
                    string reportName = Title.Replace("/", "_") + "_" + DateTime.Now.ToString("MM.dd.yyyy.H.mm.ss") + (this._workbook == null ? ".pdf" : ".xlsx");
                    string fileName = System.IO.Path.Combine(path, reportName);

                    //export it
                    if (this._workbook != null)
                        this._workbook.Save(fileName);
                    else
                        _report.Publish(fileName, FileFormat.PDF);

                    //show it
                    FileLauncher.New()
                        .HandleErrorsWith((exception, errorMsg) => { throw new Exception(errorMsg, exception); })
                        .Launch(fileName);
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error displaying report.";
                _log.Fatal(exc, errorMsg);
            }
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            OrdersReport.OrderDataTable orders = null;
            var taOrders = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter();

            //ensure to get correct time with same date
            this._fromDate = _fromDate.StartOfDay();
            this._toDate = _toDate.EndOfDay();

            orders = taOrders.GetOpenOrdersByCustomerAndDate(this._toDate, this._fromDate, CustomerID);

            if (ReportType == ReportExportType.PDF)
                CreateReportPDF(orders);
            else
                CreateReportExcel(orders);
        }

        protected void FormatBorder(WorksheetCell cell, CellBorderLineStyle borderStyle)
        {
            cell.CellFormat.BottomBorderStyle = borderStyle;
            cell.CellFormat.LeftBorderStyle = borderStyle;
            cell.CellFormat.RightBorderStyle = borderStyle;
            cell.CellFormat.TopBorderStyle = borderStyle;
        }

        private void CreateReportExcel(OrdersReport.OrderDataTable orders)
        {
            this._workbook = new Workbook(WorkbookFormat.Excel2007);
            this._workbook.DocumentProperties.Author = SecurityManager.UserName;
            this._workbook.DocumentProperties.Company = ApplicationSettings.Current.CompanyName;
            this._workbook.DocumentProperties.Title = Title;

            //Orders Summary
            Worksheet wks = this._workbook.Worksheets.Add((ApplicationSettings.Current.CompanyName + " Orders").TrimToMaxLength(31));
            wks.Rows[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            wks.Rows[0].CellFormat.Alignment = HorizontalCellAlignment.Center;
            FormatBorder(wks.Rows[0].Cells[0], CellBorderLineStyle.Thin);
            FormatBorder(wks.Rows[0].Cells[1], CellBorderLineStyle.Thin);
            FormatBorder(wks.Rows[0].Cells[2], CellBorderLineStyle.Thin);
            FormatBorder(wks.Rows[0].Cells[3], CellBorderLineStyle.Thin);
            FormatBorder(wks.Rows[0].Cells[4], CellBorderLineStyle.Thin);
            FormatBorder(wks.Rows[0].Cells[5], CellBorderLineStyle.Thin);
            FormatBorder(wks.Rows[0].Cells[6], CellBorderLineStyle.Thin);
            FormatBorder(wks.Rows[0].Cells[7], CellBorderLineStyle.Thin);

            wks.Columns[0].Width = 25 * 256;
            wks.Columns[1].Width = 10 * 256;
            wks.Columns[2].Width = 16 * 256;
            wks.Columns[3].Width = 14 * 256;
            wks.Columns[4].Width = 10 * 256;
            wks.Columns[5].Width = 10 * 256;
            wks.Columns[6].Width = 9 * 256;
            wks.Columns[7].Width = 13 * 256;

            wks.Columns[1].CellFormat.Alignment = HorizontalCellAlignment.Center;
            wks.Columns[3].CellFormat.Alignment = HorizontalCellAlignment.Center;
            wks.Columns[4].CellFormat.Alignment = HorizontalCellAlignment.Right;
            wks.Columns[5].CellFormat.Alignment = HorizontalCellAlignment.Center;
            wks.Columns[6].CellFormat.Alignment = HorizontalCellAlignment.Center;
            wks.Columns[7].CellFormat.Alignment = HorizontalCellAlignment.Right;

            wks.Rows[0].Cells[0].Value = "Customer";
            wks.Rows[0].Cells[1].Value = "WO";
            wks.Rows[0].Cells[2].Value = "Date Required";
            wks.Rows[0].Cells[3].Value = "Priority";
            wks.Rows[0].Cells[4].Value = "Quantity";
            wks.Rows[0].Cells[5].Value = "Invoice";
            wks.Rows[0].Cells[6].Value = "Unit";
            wks.Rows[0].Cells[7].Value = "Price";

            int customerOrderCount = 0;

            foreach (OrdersReport.OrderRow order in orders)
            {
                ++customerOrderCount;

                FormatBorder(wks.Rows[customerOrderCount].Cells[0], CellBorderLineStyle.Thin);
                FormatBorder(wks.Rows[customerOrderCount].Cells[1], CellBorderLineStyle.Thin);
                FormatBorder(wks.Rows[customerOrderCount].Cells[2], CellBorderLineStyle.Thin);
                FormatBorder(wks.Rows[customerOrderCount].Cells[3], CellBorderLineStyle.Thin);
                FormatBorder(wks.Rows[customerOrderCount].Cells[4], CellBorderLineStyle.Thin);
                FormatBorder(wks.Rows[customerOrderCount].Cells[5], CellBorderLineStyle.Thin);
                FormatBorder(wks.Rows[customerOrderCount].Cells[6], CellBorderLineStyle.Thin);
                FormatBorder(wks.Rows[customerOrderCount].Cells[7], CellBorderLineStyle.Thin);

                wks.Rows[customerOrderCount].Cells[0].Value = order.CustomerName;
                wks.Rows[customerOrderCount].Cells[1].Value = order.OrderID;
                wks.Rows[customerOrderCount].Cells[2].Value = order.IsRequiredDateNull() ? "NA" : order.RequiredDate.ToShortDateString();
                wks.Rows[customerOrderCount].Cells[3].Value = order.IsPriorityNull() ? "NA" : order.Priority;
                if (order.IsPartQuantityNull())
                    wks.Rows[customerOrderCount].Cells[4].Value = "NA";
                else
                    wks.Rows[customerOrderCount].Cells[4].Value = order.PartQuantity;
                wks.Rows[customerOrderCount].Cells[5].Value = order.IsInvoiceNull() ? "None" : order.Invoice;
                wks.Rows[customerOrderCount].Cells[6].Value = order.IsPriceUnitNull() ? "None" : order.PriceUnit;

                if (order.IsBasePriceNull() || order.IsPriceUnitNull() || order.IsPartQuantityNull())
                    wks.Rows[customerOrderCount].Cells[7].Value = "Unknown";
                else
                {
                    decimal price = 0;
                    //decimal fees = order.IsNull("OrderFees") ? 0 : Convert.ToDecimal(order["OrderFees"]);
                    decimal fees = OrderPrice.CalculateFees(order, order.BasePrice);
                    decimal weight = order.IsWeightNull() ? 0M : order.Weight;
                    price = OrderPrice.CalculatePrice(order.BasePrice, order.PriceUnit, fees, order.PartQuantity, weight);
                    wks.Rows[customerOrderCount].Cells[7].Value = price;
                }

                wks.Rows[customerOrderCount].Cells[7].CellFormat.FormatString = "\"$\"#,##0.00_);[Black](\"$\"#,##0.00)";
            }

            //Summary Row
            customerOrderCount++;
            wks.Rows[customerOrderCount].Cells[0].Value = "Total Orders:  " + (customerOrderCount - 1);
            Formula formula = Formula.Parse("=SUM(E2:E" + customerOrderCount + ")", CellReferenceMode.A1);
            formula.ApplyTo(wks.Rows[customerOrderCount].Cells[4]);
            wks.Rows[customerOrderCount].Cells[4].CellFormat.FormatString = "#,##0";
            formula = Formula.Parse("=SUM(H2:H" + customerOrderCount + ")", CellReferenceMode.A1);
            formula.ApplyTo(wks.Rows[customerOrderCount].Cells[7]);
            wks.Rows[customerOrderCount].Cells[7].CellFormat.FormatString = "\"$\"#,##0.00_);[Black](\"$\"#,##0.00)";
        }

        private void CreateReportPDF(OrdersReport.OrderDataTable orders)
        {
            SetupReportInfo();

            IGroup headerGroup = _section.AddGroup();
            headerGroup.Layout = Layout.Horizontal;

            // Report Info
            IText uiDate = headerGroup.AddText();
            uiDate.Alignment = TextAlignment.Left;
            uiDate.Style = DefaultStyles.NormalStyle;
            uiDate.Width = new RelativeWidth(50);
            uiDate.Borders = DefaultStyles.DefaultBorders;
            uiDate.Margins.Bottom = 5;
            uiDate.Margins.Right = 5;
            uiDate.Paddings = new Paddings(5, 5, 5, 10);
            uiDate.Background = DefaultStyles.DefaultBackground;

            uiDate.AddContent(Title, DefaultStyles.BlueXLargeStyle);
            uiDate.AddLineBreak();
            uiDate.AddContent("From Date:  " + FromDate.ToShortDateString());
            uiDate.AddLineBreak();
            uiDate.AddContent("To Date:     " + ToDate.ToShortDateString());

            // User Info
            IText uiText = headerGroup.AddText();
            uiText.Alignment = TextAlignment.Right;
            uiText.Style = DefaultStyles.NormalStyle;
            uiText.Width = new RelativeWidth(50);
            uiText.Borders = DefaultStyles.DefaultBorders;
            uiText.Margins.Bottom = 5;
            uiText.Margins.Right = 5;
            uiText.Paddings = new Paddings(5, 5, 5, 10);
            uiText.Background = DefaultStyles.DefaultBackground;

            uiText.AddContent(ApplicationSettings.Current.CompanyName, DefaultStyles.RedLargeStyle);
            uiText.AddLineBreak();
            uiText.AddContent(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            uiText.AddLineBreak();
            uiText.AddContent(SecurityManager.UserName);

            //Orders Summary
            ITable stepsTable = null;
            stepsTable = _section.AddTable();
            stepsTable.Borders = DefaultStyles.DefaultBorders;
            stepsTable.Margins.Vertical = 5;
            stepsTable.Width = new RelativeWidth(100);

            int customerOrderCount = 0, customerPartCount = 0;

            foreach (OrdersReport.OrderRow item in orders)
            {
                if (customerOrderCount == 0) //add initial customer row					
                    AddCustomerHeaderRow(item, stepsTable);

                ++customerOrderCount;
                customerPartCount += item.PartQuantity;
                AddOrderRow(item, stepsTable);
            }

            AddSummaryRow(customerOrderCount, customerPartCount, stepsTable);

            Settings.Default.LastReportCustomerID = CustomerID;
        }

        private void AddOrderRow(OrdersReport.OrderRow order, ITable table)
        {
            ITableCell cell = null;
            ITableRow row = null;

            try
            {
                //  - Order Row
                row = table.AddRow();

                //WO
                cell = row.CreateTableCell(10);
                cell.AddText(order.OrderID.ToString());

                //PO
                cell = row.CreateTableCell(15);
                cell.AddText(order.IsPurchaseOrderNull() ? "NA" : order.PurchaseOrder);

                //Customer WO
                cell = row.CreateTableCell(15);
                cell.AddText(order.IsCustomerWONull() ? "NA" : order.CustomerWO);

                //Part Name
                cell = row.CreateTableCell(23);
                cell.AddText(order.PartName);

                //Quantity
                cell = row.CreateTableCell(10);
                cell.AddText(order.PartQuantity.ToString());

                //Priority
                cell = row.CreateTableCell(12);
                cell.AddText(order.IsPriorityNull() ? "NA" : order.Priority, order.IsPriorityNull() ? DefaultStyles.NormalStyle : GetPriorityStyle(order.Priority));

                //Date Completed
                cell = row.CreateTableCell(15);
                if (order.IsRequiredDateNull())
                    cell.AddText("NA");
                else
                    cell.AddText(order.RequiredDate.ToShortDateString(), order.RequiredDate >= DateTime.Now ? DefaultStyles.NormalStyle : DefaultStyles.RedStyle);
            }
            catch (Exception exc)
            {
                _log.Warn(exc, "Unable to determine order details.");
            }
        }

        private void AddCustomerHeaderRow(OrdersReport.OrderRow order, ITable table)
        {
            ITableCell cell = table.AddRow().CreateTableCell(100);
            cell.Background = new Background(Brushes.LightBlue);
            cell.AddText(order.CustomerName, DefaultStyles.BoldStyle, TextAlignment.Left);

            ITableRow headerRow = table.AddRow();

            cell = headerRow.CreateTableCell(10);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("WO", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(15);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("PO", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(15);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("Customer WO", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(23);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("Part", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(10);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("Quantity", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(12);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("Priority", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(15);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("Date Required", DefaultStyles.BoldStyle, TextAlignment.Center);
        }

        private void AddSummaryRow(int orderCount, int totalPartCount, ITable table)
        {
            ITableRow headerRow = table.AddRow();

            ITableCell cell = headerRow.CreateTableCell(33);
            cell.Background = new Background(Brushes.LightBlue);
            cell.AddText("Total Orders", DefaultStyles.NormalStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(33);
            cell.Background = new Background(Brushes.LightBlue);
            cell.AddText("Total Parts", DefaultStyles.NormalStyle, TextAlignment.Center);

            ITableRow row = table.AddRow();

            row.CreateTableCell(33).AddText(orderCount.ToString(), DefaultStyles.BlueStyle, TextAlignment.Center);
            row.CreateTableCell(33).AddText(totalPartCount.ToString(), DefaultStyles.BlueStyle, TextAlignment.Center);
        }

        #endregion
    }

    public class ClosedOrderReport : ExcelBaseReport
    {
        #region Fields

        private DateTime _fromDate;
        private DateTime _toDate;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Closed Orders"; }
        }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }

        [Browsable(true)]
        [Description("The from date of the report.")]
        [DisplayName("From Date")]
        [Category("Report")]
        public DateTime FromDate
        {
            get { return this._fromDate; }
            set { this._fromDate = value; }
        }

        [Browsable(true)]
        [Description("The to date of the report.")]
        [DisplayName("To Date")]
        [Category("Report")]
        public DateTime ToDate
        {
            get { return this._toDate; }
            set { this._toDate = value; }
        }

        [Browsable(true)]
        [Description("Include orders that are in shipping in a box, but not yet closed.")]
        [DisplayName("Include Boxed Orders")]
        [Category("Report")]
        public bool IncludeBoxedOrders { get; set; }

        /// <summary>
        /// Gets or sets the order grouping method for this instance.
        /// </summary>
        public GroupBy GroupOrdersBy { get; set; }

        #endregion

        #region Methods

        public ClosedOrderReport()
        {
            this._fromDate = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            this._toDate = DateUtilities.GetLastDayOfMonth(DateTime.Now);
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            CreateWorkBook();
            CreateClosedOrdersByCustomerReport();
        }

        private void CreateClosedOrdersByCustomerReport()
        {
            OrdersReport dsOrdersReport = new OrdersReport();
            var taOrders = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter();
            var taFees = new Data.Reports.OrdersReportTableAdapters.OrderFeesTableAdapter();
            var taFeeType = new Data.Reports.OrdersReportTableAdapters.OrderFeeTypeTableAdapter();

            //ensure to get correct time with same date
            this._fromDate = _fromDate.StartOfDay();
            this._toDate = _toDate.EndOfDay();

            dsOrdersReport.EnforceConstraints = false;
            taFees.FillByClosed(dsOrdersReport.OrderFees);
            taFeeType.Fill(dsOrdersReport.OrderFeeType);
            taOrders.FillByClosedOrders(dsOrdersReport.Order, this._toDate, this._fromDate);

            if (IncludeBoxedOrders)
            {
                taOrders.ClearBeforeFill = false;
                taOrders.FillByOpenInShippingPackage(dsOrdersReport.Order);
            }

            using (var taOrderSerialNumbers = new Data.Reports.OrdersReportTableAdapters.OrderSerialNumberTableAdapter())
            {
                taOrderSerialNumbers.FillActive(dsOrdersReport.OrderSerialNumber);
            }

            using (var taOrderProductClass = new Data.Reports.OrdersReportTableAdapters.OrderProductClassTableAdapter())
            {
                taOrderProductClass.Fill(dsOrdersReport.OrderProductClass);
            }

            var orderInfos = new List<OrderInfo>();

            foreach (OrdersReport.OrderRow item in dsOrdersReport.Order)
            {
                var info = new OrderInfo();
                info.WO = item.OrderID;
                info.PO = !item.IsPurchaseOrderNull() ? item.PurchaseOrder : string.Empty;
                info.CustomerWO = !item.IsCustomerWONull() ? item.CustomerWO : string.Empty;
                info.Part = item.PartName;
                info.Qty = !item.IsPartQuantityNull() ? item.PartQuantity : 0;
                info.Priority = !item.IsPriorityNull() ? item.Priority : string.Empty;
                info.CompletedDate = !item.IsCompletedDateNull() ? item.CompletedDate : DateTime.MinValue;
                info.CustomerId = item.CustomerID;
                info.CustomerName = item.CustomerName;
                info.Fees = OrderPrice.CalculateFees(item, item.IsBasePriceNull() ? 0 : item.BasePrice);
                info.Price = OrderPrice.CalculatePrice(item.IsBasePriceNull() ? 0 : item.BasePrice,
                    item.IsPriceUnitNull() ? string.Empty : item.PriceUnit,
                    info.Fees,
                    item.IsPartQuantityNull() ? 0 : item.PartQuantity,
                    item.IsWeightNull() ? 0 : item.Weight);

                info.Invoice = item.IsInvoiceNull() ? "" : item.Invoice;
                info.SerialNumbers = item.GetOrderSerialNumberRows();


                var productClassRow = item.GetOrderProductClassRows().FirstOrDefault();

                info.ProductClass = productClassRow == null || productClassRow.IsProductClassNull()
                    ? "N/A"
                    : productClassRow.ProductClass;

                orderInfos.Add(info);
            }

            if (orderInfos.Count < 0)
            {
                HasData = false;
                return;
            }

            CreateSummaryWorksheet(orderInfos);

            if (GroupOrdersBy == GroupBy.Customer)
            {
                CreateCustomerWorksheets(orderInfos);
            }
            else
            {
                CreateProductClassWorksheets(orderInfos);
            }
        }

        private void CreateSummaryWorksheet(List<OrderInfo> orderInfos)
        {
            IEnumerable<IGrouping<string, OrderInfo>> ordersByCustomers;

            if (GroupOrdersBy == GroupBy.Customer)
            {
                ordersByCustomers = orderInfos.GroupBy(oi => oi.CustomerName);
            }
            else
            {
                ordersByCustomers = orderInfos.GroupBy(oi => oi.ProductClass);
            }

            var closedWks = CreateWorksheet("Summary");
            //var rowIndex = AddCompanyHeaderRows(closedWks, 6, "Summary") + 2;
            int rowIndex = 0;

            CreateHeaderFooter(closedWks, "Closed Orders Summary");
            CreateMergedHeader(closedWks, rowIndex, 0, rowIndex, 4, "Closed Orders " + this._fromDate.ToShortDateString() + " - " + this.ToDate.ToShortDateString()).CellFormat.Alignment = HorizontalCellAlignment.Center;
            rowIndex++;

            AddSummaryHeader(closedWks, rowIndex++);

            var startRowIndex = rowIndex + 1;

            if (orderInfos.Count > 0)
            {
                foreach (var ordersByCustomer in ordersByCustomers)
                {
                    long partCount;
                    try
                    {
                        partCount = ordersByCustomer.Sum(oi => (long)oi.Qty);
                    }
                    catch (OverflowException)
                    {
                        _log.Warn("Arithmetic overflow occurred while running report");
                        partCount = long.MaxValue;
                    }

                    var os = new OrderSummaryInfo()
                    {
                        CustomerName = ordersByCustomer.Key,
                        OrderCount = ordersByCustomer.Count(),
                        PartCount = partCount,
                        Fees = ordersByCustomer.Sum(oi => oi.Fees),
                        Price = ordersByCustomer.Sum(oi => oi.Price)
                    };

                    AddSummaryRow(closedWks, os, rowIndex++, 0);
                }

                this.CreateCell(closedWks, rowIndex, 0, "", false, HorizontalCellAlignment.Right);
                //Orders
                this.CreateFormulaCell(closedWks, rowIndex, 1, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                //Part
                this.CreateFormulaCell(closedWks, rowIndex, 2, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 3, rowIndex, 3), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                //Fees
                this.CreateFormulaCell(closedWks, rowIndex, 3, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 4, rowIndex, 4), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                //Price
                this.CreateFormulaCell(closedWks, rowIndex, 4, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 5, rowIndex, 5), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            }
            else
                this.HasData = false;
        }

        private void CreateCustomerWorksheets(List<OrderInfo> orderInfos)
        {
            var ordersByCustomers = orderInfos.GroupBy(oi => oi.CustomerId);

            foreach (var ordersByCustomer in ordersByCustomers)
            {
                var customerName = ordersByCustomer.First().CustomerName;

                // Set worksheet name based on customer name.
                // The first 31 characters of a customer's name may not be unique
                var worksheetName = FormatWorksheetTitle(customerName);

                if (_workbook.Worksheets.Any(c => c.Name == worksheetName))
                {
                    worksheetName = FormatWorksheetTitle(customerName, " #" + ordersByCustomer.First().CustomerId);
                }

                var closedWks = CreateWorksheet(worksheetName);
                int rowIndex = 0;

                CreateHeaderFooter(closedWks, ordersByCustomer.Key + " Closed Orders");
                CreateMergedHeader(closedWks, rowIndex, 0, rowIndex, 10, customerName + " " + this._fromDate.ToShortDateString() + " - " + this.ToDate.ToShortDateString()).CellFormat.Alignment = HorizontalCellAlignment.Center;

                rowIndex++;

                rowIndex = AddHeader(closedWks, rowIndex++);
                var startRowIndex = rowIndex + 1;

                foreach (OrderInfo item in ordersByCustomer)
                    AddRow(closedWks, item, rowIndex++, 0);

                //Orders
                this.CreateFormulaCell(closedWks, rowIndex, 0, "=COUNTA(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 1, rowIndex, 1), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateCell(closedWks, rowIndex, 1, "", false, HorizontalCellAlignment.Right);
                this.CreateCell(closedWks, rowIndex, 2, "", false, HorizontalCellAlignment.Right);
                this.CreateCell(closedWks, rowIndex, 3, "", false, HorizontalCellAlignment.Right);
                //Part
                this.CreateFormulaCell(closedWks, rowIndex, 4, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 5, rowIndex, 5), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateCell(closedWks, rowIndex, 5, "", false, HorizontalCellAlignment.Right);
                this.CreateCell(closedWks, rowIndex, 6, "", false, HorizontalCellAlignment.Right);
                this.CreateCell(closedWks, rowIndex, 7, "", false, HorizontalCellAlignment.Right);
                //Fees
                this.CreateFormulaCell(closedWks, rowIndex, 8, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 9, rowIndex, 9), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                //Price
                this.CreateFormulaCell(closedWks, rowIndex, 9, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 10, rowIndex, 10), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateCell(closedWks, rowIndex, 10, "", false, HorizontalCellAlignment.Right);
            }
        }

        private void CreateProductClassWorksheets(List<OrderInfo> orderInfos)
        {
            var ordersByProductClass = orderInfos.GroupBy(oi => oi.ProductClass);

            foreach (var ordersInProductClass in ordersByProductClass)
            {
                var productClass = ordersInProductClass.First().ProductClass;

                // Set worksheet name based on product class.
                // The first 31 characters of a product class's name may not be unique
                var worksheetName = FormatWorksheetTitle(productClass);

                if (_workbook.Worksheets.Any(c => c.Name == worksheetName))
                {
                    worksheetName = FormatWorksheetTitle(productClass, " #" + ordersInProductClass.First().WO);
                }

                var closedWks = CreateWorksheet(worksheetName);
                int rowIndex = 0;

                CreateHeaderFooter(closedWks, ordersInProductClass.Key + " Closed Orders");
                CreateMergedHeader(closedWks, rowIndex, 0, rowIndex, 10, productClass + " " + this._fromDate.ToShortDateString() + " - " + this.ToDate.ToShortDateString()).CellFormat.Alignment = HorizontalCellAlignment.Center;

                rowIndex++;

                rowIndex = AddHeader(closedWks, rowIndex++);
                var startRowIndex = rowIndex + 1;

                foreach (OrderInfo item in ordersInProductClass)
                    AddRow(closedWks, item, rowIndex++, 0);

                //Orders
                this.CreateFormulaCell(closedWks, rowIndex, 0, "=COUNTA(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 1, rowIndex, 1), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateCell(closedWks, rowIndex, 1, "", false, HorizontalCellAlignment.Right);
                this.CreateCell(closedWks, rowIndex, 2, "", false, HorizontalCellAlignment.Right);
                this.CreateCell(closedWks, rowIndex, 3, "", false, HorizontalCellAlignment.Right);
                //Part
                this.CreateFormulaCell(closedWks, rowIndex, 4, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 5, rowIndex, 5), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateCell(closedWks, rowIndex, 5, "", false, HorizontalCellAlignment.Right);
                this.CreateCell(closedWks, rowIndex, 6, "", false, HorizontalCellAlignment.Right);
                this.CreateCell(closedWks, rowIndex, 7, "", false, HorizontalCellAlignment.Right);
                //Fees
                this.CreateFormulaCell(closedWks, rowIndex, 8, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 9, rowIndex, 9), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                //Price
                this.CreateFormulaCell(closedWks, rowIndex, 9, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 10, rowIndex, 10), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateCell(closedWks, rowIndex, 10, "", false, HorizontalCellAlignment.Right);
            }
        }

        private int AddHeader(Worksheet worksheet, int rowIndex)
        {
            int startColumn = 0;
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "WO", 10);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "PO", 10);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Customer WO", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Part", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Quantity", 12);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Priority", 18);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Date Completed", 18);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Invoice", 18);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Fees", 18);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Total Price", 18);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Serial Number(s)", 30);
            rowIndex++;

            return rowIndex;
        }

        private void AddRow(Worksheet worksheet, OrderInfo orderInfo, int rowIndex, int startColumn)
        {
            try
            {
                int columnIndex = startColumn;

                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.WO;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PO;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.CustomerWO;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.Part;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.Qty;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.Priority;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.CompletedDate.ToShortDateString();
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.Invoice;

                worksheet.Rows[rowIndex].Cells[columnIndex].Value = orderInfo.Fees;
                worksheet.Rows[rowIndex].Cells[columnIndex++].CellFormat.FormatString = MONEY_FORMAT;

                worksheet.Rows[rowIndex].Cells[columnIndex].Value = orderInfo.Price;
                worksheet.Rows[rowIndex].Cells[columnIndex++].CellFormat.FormatString = MONEY_FORMAT;

                if (orderInfo.SerialNumbers != null)
                {
                    var serialNumbers = orderInfo.SerialNumbers.Select(s => s.IsNumberNull() ? string.Empty : s.Number);
                    worksheet.Rows[rowIndex].Cells[columnIndex++].Value = string.Join(", ", serialNumbers);
                }

                foreach (WorksheetCell cell in worksheet.Rows[rowIndex].Cells.Take(columnIndex))
                {
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                    cell.CellFormat.Alignment = HorizontalCellAlignment.Center;
                    ApplyCellBorders(cell);
                }

                //Reset serial num col alignment
                worksheet.Rows[rowIndex].Cells[10].CellFormat.Alignment = HorizontalCellAlignment.Left;
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        private int AddSummaryHeader(Worksheet worksheet, int rowIndex)
        {
            int startColumn = 0;
            CreateHeaderCell(worksheet, rowIndex, startColumn++, GroupOrdersBy ==  GroupBy.Customer ? "Customer" : "Product Class", 40);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Orders", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Parts", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Fees", 18);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Price", 18);
            rowIndex++;

            return rowIndex;
        }

        private void AddSummaryRow(Worksheet worksheet, OrderSummaryInfo orderInfo, int rowIndex, int startColumn)
        {
            try
            {
                int columnIndex = startColumn;

                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.CustomerName;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.OrderCount;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PartCount;

                worksheet.Rows[rowIndex].Cells[columnIndex].Value = orderInfo.Fees;
                worksheet.Rows[rowIndex].Cells[columnIndex++].CellFormat.FormatString = MONEY_FORMAT;

                worksheet.Rows[rowIndex].Cells[columnIndex].Value = orderInfo.Price;
                worksheet.Rows[rowIndex].Cells[columnIndex++].CellFormat.FormatString = MONEY_FORMAT;

                foreach (WorksheetCell cell in worksheet.Rows[rowIndex].Cells.Take(columnIndex))
                {
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                    cell.CellFormat.Alignment = HorizontalCellAlignment.Center;
                    ApplyCellBorders(cell);
                }

                worksheet.Rows[rowIndex].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        #endregion

        #region OrderInfo

        public class OrderInfo
        {
            public int WO { get; set; }
            public int Qty { get; set; }
            public string Part { get; set; }
            public string PO { get; set; }
            public string CustomerWO { get; set; }
            public string Priority { get; set; }
            public DateTime CompletedDate { get; set; }

            public int CustomerId { get; set; }

            public string CustomerName { get; set; }

            public decimal Fees { get; set; }

            public decimal Price { get; set; }

            public string Invoice { get; set; }

            public OrdersReport.OrderSerialNumberRow[] SerialNumbers { get; set; }
            public string ProductClass { get; set; }
        }

        public class OrderSummaryInfo
        {
            public string CustomerName { get; set; }
            public int OrderCount { get; set; }
            public long PartCount { get; set; }
            public decimal Fees { get; set; }
            public decimal Price { get; set; }
        }

        #endregion

        #region GroupBy

        public enum GroupBy
        {
            Customer,
            ProductClass
        }

        #endregion
    }

    public class ClosedOrdersByCustomerReport : ExcelBaseReport
    {
        #region Fields

        private string _custName;
        private DateTime _fromDate;
        private DateTime _toDate;

        private const int CLOSED_COLUMN_COUNT = 13;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Closed Orders By Customer"; }
        }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }

        [Browsable(true)]
        [Description("The from date of the report.")]
        [DisplayName("From Date")]
        [Category("Report")]
        public DateTime FromDate
        {
            get { return this._fromDate; }
            set { this._fromDate = value; }
        }

        [Browsable(true)]
        [Description("The to date of the report.")]
        [DisplayName("To Date")]
        [Category("Report")]
        public DateTime ToDate
        {
            get { return this._toDate; }
            set { this._toDate = value; }
        }

        [Browsable(true)]
        [TypeConverter(typeof(CustomerConverter))]
        [DisplayName("Customer")]
        [Category("Report")]
        public int CustomerID { get; set; }

        #endregion

        #region Methods

        #region Header/Footer

        private void AddCompanyAndDate(Worksheet worksheet, int rowIndex, int lastColumn)
        {
            CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, lastColumn,
                    "Company: " + this._custName + " | Date : " + this._fromDate.ToShortDateString()).CellFormat.Alignment =
                HorizontalCellAlignment.Center;
        }

        private int AddCompanyAddress(Worksheet worksheet, int startingRowIndex)
        {
            var dtCustomer = new CustomersDataset.CustomerDataTable();
            using (var taCustomer = new CustomerTableAdapter())
            {
                taCustomer.FillBy(dtCustomer, CustomerID);
            }

            var customer = dtCustomer.FirstOrDefault();

            if (customer == null)
            {
                _log.Warn("Could not load customer data for closed orders report.");
                return startingRowIndex;
            }

            var address1 = customer.IsAddress1Null() ? string.Empty : customer.Address1;
            var address2 = customer.IsAddress2Null() ? string.Empty : customer.Address2;
            var city = customer.IsCityNull() ? string.Empty : customer.City;
            var state = customer.IsStateNull() ? string.Empty : customer.State;
            var zip = customer.IsZipNull() ? string.Empty : customer.Zip;

            var headerText = string.Empty;

            if (customer.HasBillingAddress)
            {
                if (string.IsNullOrEmpty(address2))
                {
                    headerText = $"Company Address: {address1}, {city}, {state} {zip}";
                }
                else
                {
                    headerText = $"Company Address: {address1}, {address2}, {city}, {state} {zip}";
                }
            }

            var headerRegion = CreateMergedHeader(worksheet, startingRowIndex, 0, startingRowIndex, CLOSED_COLUMN_COUNT,
                headerText);

            headerRegion.CellFormat.Alignment =
                HorizontalCellAlignment.Center;

            return startingRowIndex + 1;
        }

        #endregion

        public ClosedOrdersByCustomerReport()
        {
            this._fromDate = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            this._toDate = DateUtilities.GetLastDayOfMonth(DateTime.Now);
            CustomerID = Settings.Default.LastReportCustomerID;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            using (var taOrders = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
                this._custName = taOrders.GetCustomerNameByID(CustomerID).ToString();

            CreateWorkBook();
            CreateClosedOrdersByCustomerReport();
        }

        private void CreateClosedOrdersByCustomerReport()
        {
            //OrdersReport.OrderDataTable orders = null;
            OrdersReport dsOrderReport = new OrdersReport();
            dsOrderReport.EnforceConstraints = false;

            var taOrders = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter();

            //ensure to get correct time with same date
            this._fromDate = _fromDate.StartOfDay();
            this._toDate = _toDate.EndOfDay();

            taOrders.FillClosedOrderByCustomer(dsOrderReport.Order, this._toDate, this._fromDate, CustomerID);

            using (var taOrderSerialNumbers = new Data.Reports.OrdersReportTableAdapters.OrderSerialNumberTableAdapter())
            {
                taOrderSerialNumbers.FillActive(dsOrderReport.OrderSerialNumber);
            }

            using (var taOrderProductClass = new Data.Reports.OrdersReportTableAdapters.OrderProductClassTableAdapter())
            {
                taOrderProductClass.Fill(dsOrderReport.OrderProductClass);
            }

            var orderInfos = new List<OrderInfo>();

            using (var taCustomerAddress = new Data.Datasets.OrdersDataSetTableAdapters.CustomerAddressTableAdapter())
            {
                foreach (OrdersReport.OrderRow item in dsOrderReport.Order)
                {
                    var info = new OrderInfo();
                    info.WO = item.OrderID;
                    info.PO = !item.IsPurchaseOrderNull() ? item.PurchaseOrder : string.Empty;
                    info.CustomerWO = !item.IsCustomerWONull() ? item.CustomerWO : string.Empty;
                    info.Part = item.PartName;
                    info.Qty = !item.IsPartQuantityNull() ? item.PartQuantity : 0;
                    info.Priority = !item.IsPriorityNull() ? item.Priority : string.Empty;
                    info.CompletedDate = !item.IsCompletedDateNull() ? item.CompletedDate : DateTime.MinValue;

                    var itemAddress = taCustomerAddress.GetByOrder(item.OrderID).FirstOrDefault();
                    if (itemAddress != null)
                    {
                        info.Address = new OrderInfoAddress
                        {
                            Address1 = itemAddress.IsAddress1Null() ? string.Empty : itemAddress.Address1,
                            Address2 = itemAddress.IsAddress2Null() ? string.Empty : itemAddress.Address2,
                            City = itemAddress.IsCityNull() ? string.Empty : itemAddress.City,
                            State = itemAddress.IsStateNull() ? string.Empty : itemAddress.State,
                            Zip = itemAddress.IsZipNull() ? string.Empty : itemAddress.Zip
                        };
                    }

                    info.SerialNumbers = item.GetOrderSerialNumberRows();

                    var productClassRow = item.GetOrderProductClassRows().FirstOrDefault();

                    info.ProductClass = productClassRow == null || productClassRow.IsProductClassNull()
                        ? "N/A"
                        : productClassRow.ProductClass;

                    orderInfos.Add(info);
                }
            }

            FillClosedOrdersByCustomer(orderInfos);
        }

        private void FillClosedOrdersByCustomer(List<OrderInfo> orderInfos)
        {
            Worksheet closedWks = CreateWorksheet("Summary");
            var rowIndex = AddCompanyHeaderRows(closedWks, CLOSED_COLUMN_COUNT, "Summary") + 2;
            //int rowIndex = 0;

            CreateHeaderFooter(closedWks, "Closed Orders By Customer Report");
            AddCompanyAndDate(closedWks, rowIndex, CLOSED_COLUMN_COUNT);
            rowIndex++;
            rowIndex = AddCompanyAddress(closedWks, rowIndex);

            if (orderInfos.Count > 0)
            {
                rowIndex = AddHeader(closedWks, rowIndex++);
                foreach (OrderInfo item in orderInfos)
                    AddRow(closedWks, item, rowIndex++);

                long totalPartQuantity;

                try
                {
                    totalPartQuantity = orderInfos.Sum(oi => (long)oi.Qty);
                }
                catch (OverflowException)
                {
                    _log.Warn("Arithmetic overflow occurred while running report");
                    totalPartQuantity = long.MaxValue;
                }

                AddSummaryRow(orderInfos.Count, totalPartQuantity, closedWks, rowIndex);
            }
            else
            {
                AddHeader(closedWks, rowIndex++);
                AddEmptyRow(closedWks, rowIndex++, 0);
                AddSummaryRow(0, 0, closedWks, rowIndex);
            }
        }

        private int AddHeader(Worksheet worksheet, int rowIndex)
        {
            int startColumn = 0;
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "WO", 10);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "PO", 10);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Customer WO", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Part", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Quantity", 12);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Priority", 18);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Date Completed", 18);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Address 1", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Address 2", 18);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "City", 12);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "State", 10);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Zip", 10);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Serial Number(s)", 30);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Product Class", 25);
            rowIndex++;

            return rowIndex;
        }

        private void AddRow(Worksheet worksheet, OrderInfo orderInfo, int rowIndex)
        {
            try
            {
                worksheet.Rows[rowIndex].Cells[00].Value = orderInfo.WO;
                worksheet.Rows[rowIndex].Cells[01].Value = orderInfo.PO;
                worksheet.Rows[rowIndex].Cells[02].Value = orderInfo.CustomerWO;
                worksheet.Rows[rowIndex].Cells[03].Value = orderInfo.Part;
                worksheet.Rows[rowIndex].Cells[04].Value = orderInfo.Qty;
                worksheet.Rows[rowIndex].Cells[05].Value = orderInfo.Priority;
                worksheet.Rows[rowIndex].Cells[06].Value = orderInfo.CompletedDate.ToShortDateString();
                worksheet.Rows[rowIndex].Cells[07].Value = orderInfo.Address?.Address1;
                worksheet.Rows[rowIndex].Cells[08].Value = orderInfo.Address?.Address2;
                worksheet.Rows[rowIndex].Cells[09].Value = orderInfo.Address?.City;
                worksheet.Rows[rowIndex].Cells[10].Value = orderInfo.Address?.State;
                worksheet.Rows[rowIndex].Cells[11].Value = orderInfo.Address?.Zip;

                if (orderInfo.SerialNumbers != null)
                {
                    var serialNumbers = orderInfo.SerialNumbers.Select(s => s.IsNumberNull() ? string.Empty : s.Number);
                    worksheet.Rows[rowIndex].Cells[12].Value = string.Join(", ", serialNumbers);
                }

                worksheet.Rows[rowIndex].Cells[13].Value = orderInfo.ProductClass;

                foreach (WorksheetCell cell in worksheet.Rows[rowIndex].Cells.Take(CLOSED_COLUMN_COUNT + 1))
                {
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                    cell.CellFormat.Alignment = HorizontalCellAlignment.Center;
                    cell.CellFormat.WrapText = ExcelDefaultableBoolean.True;
                    ApplyCellBorders(cell);
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        private void AddEmptyRow(Worksheet worksheet, int rowIndex, int startColumn)
        {
            try
            {
                WorksheetMergedCellsRegion region1 = CreateMergedHeader(worksheet, rowIndex, startColumn, rowIndex, startColumn + CLOSED_COLUMN_COUNT, "None");
                region1.CellFormat.Alignment = HorizontalCellAlignment.Center;
                region1.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                ApplyCellBorders(region1.CellFormat);
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        private void AddSummaryRow(int orderCount, long totalPartCount, Worksheet worksheet, int rowIndex)
        {
            CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 2, "Total: ");
            CreateMergedHeader(worksheet, rowIndex, 3, rowIndex, 7, "Total Orders: " + orderCount.ToString("N0")).CellFormat.Alignment = HorizontalCellAlignment.Center;
            CreateMergedHeader(worksheet, rowIndex, 8, rowIndex, 13, "Total Parts: " + totalPartCount.ToString("N0")).CellFormat.Alignment = HorizontalCellAlignment.Center;
        }

        #endregion

        #region OrderInfo

        public class OrderInfo
        {
            public int WO { get; set; }
            public int Qty { get; set; }
            public string Part { get; set; }
            public string PO { get; set; }
            public string CustomerWO { get; set; }
            public string Priority { get; set; }
            public DateTime CompletedDate { get; set; }
            public OrderInfoAddress Address { get; set; }
            public OrdersReport.OrderSerialNumberRow[] SerialNumbers { get; set; }
            public string ProductClass { get; set; }
        }

        #endregion

        #region OrderInfoAddress

        public class OrderInfoAddress
        {
            public string Address1 { get; set; }

            public string Address2 { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public string Zip { get; set; }
        }
        #endregion
    }

    public class InspectionReport : ExcelBaseReport
    {
        #region Fields

        private DateTime _fromDate;
        private int _inspectionType;
        private DateTime _toDate;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Inspection Report"; }
        }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Landscape; }
        }

        [Browsable(true)]
        [Description("The from date of the report.")]
        [DisplayName("From Date")]
        [Category("Report")]
        public DateTime FromDate
        {
            get { return this._fromDate; }
            set { this._fromDate = value; }
        }

        [Browsable(true)]
        [Description("The to date of the report.")]
        [DisplayName("To Date")]
        [Category("Report")]
        public DateTime ToDate
        {
            get { return this._toDate; }
            set { this._toDate = value; }
        }

        [Browsable(true)]
        [Description("The type of Inspection")]
        [DisplayName("Inspection Type")]
        [Category("Report")]
        [TypeConverter(typeof(InspectionTypeConverter))]
        public int InspectionType
        {
            get { return this._inspectionType; }
            set { this._inspectionType = value; }
        }

        public string InspectionName { get; set; }

        #endregion

        #region Methods

        private const int INSPECTION_COLUMN_COUNT = 7;

        public InspectionReport()
        {
            this._fromDate = DateTime.Now.Date;
            this._toDate = DateTime.Now.EndOfDay();

            var table = new QAReport.PartInspectionTypeDataTable();

            using (var taInpsectionType = new Data.Reports.QAReportTableAdapters.PartInspectionTypeTableAdapter())
                taInpsectionType.Fill(table);

            InspectionType = table.FirstOrDefault().PartInspectionTypeID;
        }

        private void AddInspectionAndDate(Worksheet worksheet, int rowIndex, int lastColumn)
        {
            string date = this._fromDate.Date == this._toDate.Date ? this._fromDate.Date.ToShortDateString() : this._fromDate.Date.ToShortDateString() + " - " + this._toDate.Date.ToShortDateString();
            CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, lastColumn, "Inspection: " + InspectionName + " | Date : " + date).CellFormat.Alignment = HorizontalCellAlignment.Center;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            //ensure to get correct time with same date
            this._fromDate = this._fromDate.Date;
            this._toDate = this._toDate.EndOfDay();
            this._inspectionType = InspectionType;

            CreateWorkBook();
            CreateInspectionReport();
        }

        protected void CreateInspectionReport()
        {
            var dsQA = new QAReport();
            dsQA.EnforceConstraints = false;

            using (var taInpsectionType = new Data.Reports.QAReportTableAdapters.PartInspectionTypeTableAdapter())
                taInpsectionType.FillBy(dsQA.PartInspectionType, InspectionType);

            using (var taQuestion = new Data.Reports.QAReportTableAdapters.PartInspectionQuestionTableAdapter())
                taQuestion.FillByType(dsQA.PartInspectionQuestion, InspectionType);

            using (var taInspection = new PartInspection())
                taInspection.FillPartInspectionAndQuestionByDate(dsQA.PartInspection, this._fromDate, this._toDate, this._inspectionType);

            using (var ta = new Data.Reports.QAReportTableAdapters.PartInspectionAnswerTableAdapter())
                ta.FillByInspectionTypeAndDate(dsQA.PartInspectionAnswer, this._inspectionType, this._fromDate, this._toDate);

            var orderInfos = new List<OrderInfo>();
            var questions = new List<QuestionInfo>();

            foreach (QAReport.PartInspectionRow inspection in dsQA.PartInspection)
            {
                var info = new OrderInfo();

                info.WO = inspection.OrderID;
                info.AcceptedQty = inspection.AcceptedQty;
                info.RejectedQty = inspection.RejectedQty;

                if (!inspection.IsPartQuantityNull())
                    info.PartQuantity = inspection.PartQuantity;
                if (!inspection.IsUserNameNull())
                    info.Inspector = inspection.UserName;
                if (!inspection.IsPartNameNull())
                    info.PartName = inspection.PartName;
                if (!inspection.IsInspectionDateNull())
                    info.InspectionDate = inspection.InspectionDate;
                if (!inspection.IsCustomerNameNull())
                    info.Customer = inspection.CustomerName;

                foreach (QAReport.PartInspectionAnswerRow ans in inspection.GetPartInspectionAnswerRows())
                {
                    info.Answers.Add(new AnswerInfo
                    {
                        Answer = ans.IsAnswerNull() ? string.Empty : ans.Answer,
                        QuestionID = ans.PartInspectionQuestionID
                    });
                }

                orderInfos.Add(info);
            }

            QAReport.PartInspectionTypeRow inspectionType = dsQA.PartInspectionType.FindByPartInspectionTypeID(InspectionType);
            if (inspectionType != null)
            {
                InspectionName = inspectionType.Name;

                foreach (QAReport.PartInspectionQuestionRow question in inspectionType.GetPartInspectionQuestionRows())
                    questions.Add(new QuestionInfo { QuestionID = question.PartInspectionQuestionID, Name = question.Name });
            }

            HasData = orderInfos.Count > 0;
            FillInspectionReport(orderInfos, questions);
        }

        private void FillInspectionReport(List<OrderInfo> info, List<QuestionInfo> questions)
        {
            Worksheet inspectionWks = CreateWorksheet("Inspections");
            int rowIndex = 0;

            CreateHeaderFooter(inspectionWks, "Inspection Report");
            AddInspectionAndDate(inspectionWks, rowIndex, INSPECTION_COLUMN_COUNT + questions.Count);
            rowIndex++;

            if (info.Count > 0)
            {
                rowIndex = AddInspectionHeader(inspectionWks, rowIndex, questions);

                foreach (OrderInfo item in info)
                    AddInspectionRow(inspectionWks, item, rowIndex++, 0, questions);
            }
            else
            {
                AddInspectionHeader(inspectionWks, rowIndex++, questions);
                AddEmptyInspectionRow(inspectionWks, rowIndex++, 0, questions);
            }
        }

        private int AddInspectionHeader(Worksheet worksheet, int rowIndex, List<QuestionInfo> questions)
        {
            int startColumn = 0;
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "WO", 10);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Part", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Quantity", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Accepted", 12);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Rejected", 12);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Inspector", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Customer", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Inspect. Date", 16);

            //add question headers
            foreach (QuestionInfo questionInfo in questions)
                CreateHeaderCell(worksheet, rowIndex, startColumn++, questionInfo.Name, Convert.ToInt32(questionInfo.Name.Length * 1.3));

            rowIndex++;

            return rowIndex;
        }

        private void AddInspectionRow(Worksheet worksheet, OrderInfo orderInfo, int rowIndex, int startColumn, List<QuestionInfo> questions)
        {
            try
            {
                int columnIndex = startColumn;

                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.WO;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PartName;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PartQuantity;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.AcceptedQty;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.RejectedQty;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.Inspector;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.Customer;

                if (orderInfo.InspectionDate != DateTime.MinValue)
                    worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.InspectionDate.Date;

                foreach (QuestionInfo question in questions)
                {
                    AnswerInfo answer = orderInfo.Answers.Find(ai => ai.QuestionID == question.QuestionID);
                    worksheet.Rows[rowIndex].Cells[columnIndex++].Value = answer == null ? "NA" : answer.Answer;
                }

                foreach (WorksheetCell cell in worksheet.Rows[rowIndex].Cells.Take(columnIndex))
                {
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                    cell.CellFormat.Alignment = HorizontalCellAlignment.Center;
                    ApplyCellBorders(cell);
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        private void AddEmptyInspectionRow(Worksheet worksheet, int rowIndex, int startColumn, List<QuestionInfo> questions)
        {
            try
            {
                WorksheetMergedCellsRegion region1 = CreateMergedHeader(worksheet, rowIndex, startColumn, rowIndex, startColumn + INSPECTION_COLUMN_COUNT + questions.Count, "None");
                region1.CellFormat.Alignment = HorizontalCellAlignment.Center;
                region1.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                ApplyCellBorders(region1.CellFormat);
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        #endregion

        #region Order Info

        private class AnswerInfo
        {
            public string Answer { get; set; }
            public int QuestionID { get; set; }
        }

        private class OrderInfo
        {
            public OrderInfo() { Answers = new List<AnswerInfo>(); }

            public int WO { get; set; }
            public int PartQuantity { get; set; }
            public int AcceptedQty { get; set; }
            public int RejectedQty { get; set; }
            public string PartName { get; set; }
            public string Inspector { get; set; }
            public string Customer { get; set; }
            public DateTime InspectionDate { get; set; }
            public List<AnswerInfo> Answers { get; set; }
        }

        private class QuestionInfo
        {
            public string Name { get; set; }
            public int QuestionID { get; set; }
        }

        #endregion
    }

    public class OrderStatusReport : ExcelBaseReport
    {
        #region Fields

        private readonly OrdersReport.OrderDataTable _orders;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Order Status"; }
        }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }

        #endregion

        #region Methods

        public OrderStatusReport(OrdersReport.OrderDataTable orders)
        {
            this._orders = orders;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            SetupReportInfo();

            var wks = CreateWorksheet(Title);
            var rowIndex = AddHeaderRow(wks);
            var startRowIndex = rowIndex;

            CreateHeaderFooter(wks, Title);

            foreach (var item in this._orders)
            {
                AddOrderRow(item, wks, ref rowIndex);
                rowIndex++;
            }

            AddSummaryRow(wks, rowIndex);

            CreateTable(wks, startRowIndex, 9, rowIndex, true);
        }

        private void AddOrderRow(OrdersReport.OrderRow order, Worksheet wks, ref int rowIndex)
        {
            try
            {
                int colIndex = 0;

                CreateCell(wks, rowIndex, colIndex++, order.CustomerName, horizontalAlignment: HorizontalCellAlignment.Left);
                CreateCell(wks, rowIndex, colIndex++, order.OrderID, horizontalAlignment: HorizontalCellAlignment.Left);
                CreateCell(wks, rowIndex, colIndex++, order.IsRequiredDateNull() ? (object)"NA" : order.RequiredDate, horizontalAlignment: HorizontalCellAlignment.Left);
                CreateCell(wks, rowIndex, colIndex++, order.IsCompletedDateNull() ? (object)"NA" : order.CompletedDate, horizontalAlignment: HorizontalCellAlignment.Left);
                CreateCell(wks, rowIndex, colIndex++, order.IsPriorityNull() ? "NA" : order.Priority, horizontalAlignment: HorizontalCellAlignment.Left);
                CreateCell(wks, rowIndex, colIndex++, order.IsPurchaseOrderNull() ? "" : order.PurchaseOrder, horizontalAlignment: HorizontalCellAlignment.Left);
                CreateCell(wks, rowIndex, colIndex++, order.IsEstShipDateNull() ? (object)"NA" : order.EstShipDate, horizontalAlignment: HorizontalCellAlignment.Left);
                CreateCell(wks, rowIndex, colIndex++, order.WorkStatus, horizontalAlignment: HorizontalCellAlignment.Left);
                CreateCell(wks, rowIndex, colIndex++, order.PartName, horizontalAlignment: HorizontalCellAlignment.Left);
                CreateCell(wks, rowIndex, colIndex++, order.IsPartQuantityNull() ? 0 : order.PartQuantity, horizontalAlignment: HorizontalCellAlignment.Left);
            }
            catch (Exception exc)
            {
                _log.Warn(exc, "Unable to determine order details.");
            }
        }

        private int AddHeaderRow(Worksheet wks)
        {
            var rowIndex = base.AddCompanyHeaderRows(wks, 4, String.Empty) + 2;
            int colIndex = 0;

            CreateHeaderCell(wks, rowIndex, colIndex++, "Customer Name", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "WO", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Req. Date", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Comp. Date", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Priority", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "PO", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Est Ship Date", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Work Status", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Part Name", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Quantity", 20);

            return ++rowIndex;
        }

        private void AddSummaryRow(Worksheet wks, int rowIndex)
        {
            var cell = this.CreateCell(wks, rowIndex, 0, "Total:");
            cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            //orders
            cell = this.CreateFormulaCell(wks, rowIndex, 1, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(7, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
            cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            //parts
            cell = this.CreateFormulaCell(wks, rowIndex, 9, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(7, 10, rowIndex, 10), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
            cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
        }

        #endregion
    }

    public class BatchTraveler2Report : Report
    {
        #region Fields

        private readonly int _batchId;
        private Data.Datasets.OrderProcessingDataSet _dsOrderProcessing;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Batch Traveler"; }
        }

        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }

        // This report puts barcodes at the top of the footer.
        // Space required was determined through trial & error to match what it was before
        // moving barcodes to the footer.
        protected override float AdditionalFooterSpace => 56;

        #endregion

        #region Methods

        public BatchTraveler2Report(int batchID) { this._batchId = batchID; }

        protected override void CreateReport(System.Threading.CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            SetupReportInfo();

            AddHeader("BATCH TRAVELER ", "Batch: ", this._batchId, false, null, ReportType.BatchOrder);

            _dsOrderProcessing = new OrderProcessingDataSet { EnforceConstraints = false };

            using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
                ta.FillBy(_dsOrderProcessing.Batch, _batchId);

            using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchOrderTableAdapter())
                ta.FillBy(_dsOrderProcessing.BatchOrder, _batchId);

            using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesTableAdapter())
                ta.FillBy(_dsOrderProcessing.BatchProcesses, _batchId);

            using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.PartTableAdapter())
                ta.FillByBatch(_dsOrderProcessing.Part, _batchId);

            using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter())
                ta.FillByBatch2(_dsOrderProcessing.OrderSummary, _batchId);

            using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter())
                ta.FillByBatch(_dsOrderProcessing.Process, _batchId);

            AddOrdersSection();
            AddProcessSection();
            AddBarCodeCommands();
        }

        private void AddOrdersSection()
        {
            try
            {
                IGroup group = _section.AddGroup();
                group.Layout = Layout.Horizontal;
                group.Margins = new Margins(5, 5, 3, 0);
                group.Paddings.All = 5;
                group.Borders = DefaultStyles.DefaultBorders;
                group.Background = DefaultStyles.DefaultBackground;

                ITable table = group.AddTable();

                //Add Header
                ITableRow row = table.AddRow();
                row.Margins = new VerticalMargins(0, 5);
                ITableCell cell = row.AddCell();
                cell.Width = new FixedWidth(100);

                cell.AddText("Orders:", DefaultStyles.BlueLargeStyle, TextAlignment.Left);

                cell = row.AddCell();
                cell.Width = new RelativeWidth(100);
                cell.AddText("Fixture: " + (_dsOrderProcessing.Batch[0].IsFixtureNull() || String.IsNullOrWhiteSpace(_dsOrderProcessing.Batch[0].Fixture) ? "None" : _dsOrderProcessing.Batch[0].Fixture), DefaultStyles.BlueLargeStyle, TextAlignment.Right);

                row = table.AddRow();

                var borders = new Borders(new Pen(new Color(System.Drawing.Color.Black), .5F));
                var ta = new TextAlignment(Alignment.Left, Alignment.Middle);

                row.AddHeaderCells(DefaultStyles.NormalStyle, borders, 2, ta, "WO", "Customer", "Part", "Quantity", "Surface Area", "Weight");
                decimal totalWeight = 0;
                double totalSA = 0;
                int totalParts = 0;
                int totalOrders = 0;

                foreach (var batch in _dsOrderProcessing.BatchOrder)
                {
                    var order = _dsOrderProcessing.OrderSummary.FindByOrderID(batch.OrderID);
                    var part = _dsOrderProcessing.Part.FindByPartID(order.PartID);
                    var surfaceArea = batch.PartQuantity * part.SurfaceArea;

                    decimal weight;
                    if (order.IsWeightNull())
                    {
                        weight = batch.PartQuantity * (part.IsWeightNull() ? 0 : part.Weight);
                    }
                    else
                    {
                        weight = order.Weight;
                    }
                    table.AddRow().AddCells(DefaultStyles.NormalStyle, borders, 2, ta, order.OrderID.ToString(), order.CustomerName, part.Name, batch.PartQuantity.ToString(), surfaceArea.ToString("N0") + " in²", weight.ToString("N0") + " lbs.");

                    totalWeight += weight;
                    totalSA += surfaceArea;
                    totalParts += batch.PartQuantity;
                    totalOrders++;
                }

                var footerCells = table.AddRow().AddCells(DefaultStyles.BlackLargeStyle, borders, 2, ta, "TOTALS:", totalOrders.ToString("N0"), "", totalParts.ToString("N0"), totalSA.ToString("N0") + " in²", totalWeight.ToString("N0") + " lbs.");
                footerCells.ForEach(fc => fc.Background = new Background(Colors.LightSteelBlue));
            }
            catch (Exception exc)
            {
                _log.Fatal(exc, "Error adding process section to WorkOrder Traveler report.");
            }
        }

        /// <summary>
        ///     Adds the process section describing what processes are going to done on the part.
        /// </summary>
        private void AddProcessSection()
        {
            try
            {
                IGroup group = _section.AddGroup();
                group.Layout = Layout.Vertical;
                group.Margins = new Margins(5, 5, 3, 0);
                group.Paddings.All = 5;
                group.Borders = DefaultStyles.DefaultBorders;
                group.Background = DefaultStyles.DefaultBackground;

                ITable table = group.AddTable();

                //Add Header
                ITableRow row = table.AddRow();
                ITableCell cell = row.AddCell();
                cell.AddText("Processes:", DefaultStyles.BlueLargeStyle, TextAlignment.Left);

                var stepStyle = new Style(new Font("Verdana", 9, FontStyle.Bold), Brushes.Black);
                var signStyle = new Style(new Font("Verdana", 9), Brushes.Black);

                foreach (var orderProcess in _dsOrderProcessing.BatchProcesses.OrderBy(bp => bp.StepOrder))
                {
                    row = table.AddRow();
                    var estEndDate = "";

                    //if (!orderProcess.IsEstEndDateNull() && ApplicationSettings.Current.IncludeProcessDateOnTraveler)
                    //    estEndDate = "Due: " + orderProcess.EstEndDate.ToString("MM/dd/yy") + " ";

                    row.AddCell(40, orderProcess.StepOrder + " - " + orderProcess.Department, stepStyle, TextAlignment.Left, new HorizontalMargins(5, 2));
                    row.AddCell(60, estEndDate + "Qty:______ Date:__________  By:__________ ", signStyle, TextAlignment.Right, new HorizontalMargins(2, 20));

                    row = table.AddRow();
                    cell = row.AddCell();
                    cell.Width = new RelativeWidth(100);
                    cell.Margins = new HorizontalMargins(10, 2);
                    IText txt = cell.AddText();
                    txt.Alignment = TextAlignment.Left;

                    string processName = orderProcess.ProcessRow.Name;

                    txt.AddRichContent("<font face=\"Verdana\" size=\"9\">" + processName + "</font>");
                }

                // Total Amperage
                var batchRow = _dsOrderProcessing.Batch.FirstOrDefault();

                if (batchRow != null && !batchRow.IsAmpsPerSquareFootNull())
                {
                    var totalAreaInches = 0d;
                    foreach (var batch in _dsOrderProcessing.BatchOrder)
                    {
                        var order = _dsOrderProcessing.OrderSummary.FindByOrderID(batch.OrderID);
                        var part = _dsOrderProcessing.Part.FindByPartID(order.PartID);

                        var surfaceArea = batch.PartQuantity * part.SurfaceArea;

                        totalAreaInches += surfaceArea;
                    }

                    var totalAreaFeet = totalAreaInches / 144;
                    var totalAmps = Math.Round(totalAreaFeet * batchRow.AmpsPerSquareFoot, 2);
                    var ampsText = group.AddText();
                    ampsText.Paddings.Top = 10;
                    ampsText.Paddings.Bottom = 2;
                    ampsText.Paddings.Left = 2;
                    ampsText.Paddings.Right = 2;

                    ampsText.AddContent($"Total Amperage: {totalAmps:N2}  A", DefaultStyles.BlackLargeStyle);
                }
            }
            catch (Exception exc)
            {
                _log.Fatal(exc, "Error adding process section to WorkOrder Traveler report.");
            }
        }

        /// <summary>
        ///     Adds the bar code commands to the bottom of the document.
        /// </summary>
        private void AddBarCodeCommands()
        {
            try
            {
                if (_footer == null)
                {
                    return;
                }

                IGroup group = _footer.AddGroup(5, 0);
                group.Layout = Layout.Horizontal;
                group.Margins.All = 5;
                group.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);

                ITable table = group.AddTable();
                ITableRow row = table.AddRow();

                //Add CheckIn Barcode
                ITableCell cell = row.AddCell();
                cell.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);
                IGroup checkInGroup = cell.AddGroup();
                checkInGroup.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);

                IText checkInText = checkInGroup.AddText();
                checkInText.AddContent("Batch Check In", DefaultStyles.NormalStyle);
                checkInText.Alignment = new TextAlignment(Alignment.Center, Alignment.Bottom);

                IImage imageOCIBarCode = checkInGroup.AddImage(new Image(CreateBatchCheckInBarcode(_batchId)));
                imageOCIBarCode.KeepRatio = true;
                imageOCIBarCode.Margins.All = 5;

                //Add Action Barcode
                cell = row.AddCell();
                cell.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);
                IGroup shipGroup = cell.AddGroup();
                shipGroup.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);

                IText shipText = shipGroup.AddText();
                shipText.AddContent("Batch Processing", DefaultStyles.NormalStyle);
                shipText.Alignment = new TextAlignment(Alignment.Center, Alignment.Bottom);

                IImage imageShipBarCode = shipGroup.AddImage(new Image(CreateBatchActionBarcode(_batchId)));
                imageShipBarCode.KeepRatio = true;
                imageShipBarCode.Margins.All = 5;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding barcode section to Batch Traveler report.");
            }
        }

        #endregion
    }

    public class OrderCountByUserReport : Report
    {
        #region Fields

        private DateTime _fromDate;
        private DateTime _toDate;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Orders By User Report"; }
        }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }

        [Browsable(true)]
        [Description("The from date of the report.")]
        [DisplayName("From Date")]
        [Category("Report")]
        public DateTime FromDate
        {
            get { return this._fromDate; }
            set { this._fromDate = value; }
        }

        [Browsable(true)]
        [Description("The to date of the report.")]
        [DisplayName("To Date")]
        [Category("Report")]
        public DateTime ToDate
        {
            get { return this._toDate; }
            set { this._toDate = value; }
        }

        #endregion

        #region Methods

        public OrderCountByUserReport()
        {
            this._fromDate = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            this._toDate = DateUtilities.GetLastDayOfMonth(DateTime.Now);
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            //ensure to get correct time with same date
            this._fromDate = _fromDate.StartOfDay();
            this._toDate = _toDate.EndOfDay();

            SetupReportInfo();

            IGroup headerGroup = _section.AddGroup();
            headerGroup.Layout = Layout.Horizontal;

            // Report Info
            IText uiDate = headerGroup.AddText();
            uiDate.Alignment = TextAlignment.Left;
            uiDate.Style = DefaultStyles.NormalStyle;
            uiDate.Width = new RelativeWidth(50);
            uiDate.Borders = DefaultStyles.DefaultBorders;
            uiDate.Margins.Bottom = 5;
            uiDate.Margins.Right = 5;
            uiDate.Paddings = new Paddings(5, 5, 5, 10);
            uiDate.Background = DefaultStyles.DefaultBackground;

            uiDate.AddContent(Title, DefaultStyles.BlueXLargeStyle);
            uiDate.AddLineBreak();
            uiDate.AddContent("From Date:  " + FromDate.ToShortDateString());
            uiDate.AddLineBreak();
            uiDate.AddContent("To Date:     " + ToDate.ToShortDateString());

            // User Info
            IText uiText = headerGroup.AddText();
            uiText.Alignment = TextAlignment.Right;
            uiText.Style = DefaultStyles.NormalStyle;
            uiText.Width = new RelativeWidth(50);
            uiText.Borders = DefaultStyles.DefaultBorders;
            uiText.Margins.Bottom = 5;
            uiText.Margins.Right = 5;
            uiText.Paddings = new Paddings(5, 5, 5, 10);
            uiText.Background = DefaultStyles.DefaultBackground;

            uiText.AddContent(ApplicationSettings.Current.CompanyName, DefaultStyles.RedLargeStyle);
            uiText.AddLineBreak();
            uiText.AddContent(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            uiText.AddLineBreak();
            uiText.AddContent(SecurityManager.UserName);

            //Add summary Table
            ITable summaryTable = _section.AddTable();
            summaryTable.Margins.Vertical = 5;
            summaryTable.Borders = DefaultStyles.DefaultBorders;
            summaryTable.Width = new RelativeWidth(100);

            ITable mainTable = _section.AddTable();
            mainTable.Margins.Vertical = 5;
            mainTable.Borders = DefaultStyles.DefaultBorders;
            mainTable.Width = new RelativeWidth(100);
            int totalOrders = 0;
            var orderCountByUser = new Dictionary<string, int>(); //keep list or order count by user

            using (var ta = new OrderCountByUserTableAdapter())
            {
                using (OrdersReport.OrderCountByUserDataTable dt = ta.GetData(this._fromDate, this._toDate))
                {
                    if (dt.Count > 0)
                    {
                        AddHeaderRow(mainTable);

                        foreach (OrdersReport.OrderCountByUserRow item in dt)
                        {
                            AddRow(item, mainTable);
                            totalOrders += item.OrderCount;

                            if (!orderCountByUser.ContainsKey(item.Name))
                                orderCountByUser.Add(item.Name, 0);

                            orderCountByUser[item.Name] = orderCountByUser[item.Name] + item.OrderCount;
                        }

                        AddSummaryRow(totalOrders, mainTable);

                        AddSummationTable(orderCountByUser, summaryTable);
                    }
                }
            }
        }

        private void AddSummationTable(Dictionary<string, int> orderCountByUser, ITable summaryTable)
        {
            try
            {
                ITableCell cell = null;
                ITableRow headerRow = summaryTable.AddRow();

                cell = headerRow.CreateTableCell(50);
                cell.Background = new Background(Brushes.AliceBlue);
                cell.AddText("User Name", DefaultStyles.BoldStyle, TextAlignment.Center);

                cell = headerRow.CreateTableCell(50);
                cell.Background = new Background(Brushes.AliceBlue);
                cell.AddText("Orders", DefaultStyles.BoldStyle, TextAlignment.Center);

                foreach (var orderRow in orderCountByUser)
                {
                    ITableCell rowCell = null;
                    ITableRow row = null;

                    row = summaryTable.AddRow();

                    rowCell = row.CreateTableCell(50);
                    rowCell.AddText(orderRow.Key); //User Name

                    rowCell = row.CreateTableCell(50);
                    rowCell.AddText(orderRow.Value.ToString()); //User Total
                }
            }
            catch (Exception exc)
            {
                _log.Warn(exc, "Unable to determine order details.");
            }
        }

        private void AddRow(OrdersReport.OrderCountByUserRow orderRow, ITable table)
        {
            ITableCell cell = null;
            ITableRow row = null;

            try
            {
                //  - Order Row
                row = table.AddRow();

                cell = row.CreateTableCell(33);
                cell.AddText(orderRow.Name);

                cell = row.CreateTableCell(33);
                cell.AddText(orderRow.OrderCount.ToString());

                cell = row.CreateTableCell(33);
                cell.AddText(orderRow.OrderDate);
            }
            catch (Exception exc)
            {
                _log.Warn(exc, "Unable to determine order details.");
            }
        }

        private void AddHeaderRow(ITable table)
        {
            ITableCell cell = null;
            ITableRow headerRow = table.AddRow();

            cell = headerRow.CreateTableCell(33);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("User Name", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(33);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("Orders", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(33);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("Date", DefaultStyles.BoldStyle, TextAlignment.Center);
        }

        private void AddSummaryRow(int orderTotal, ITable table)
        {
            ITableRow headerRow = table.AddRow();

            ITableCell cell = headerRow.CreateTableCell(66);
            cell.Background = new Background(Brushes.LightBlue);
            cell.AddText("Total Orders", DefaultStyles.NormalStyle, TextAlignment.Right);

            cell = headerRow.CreateTableCell(50);
            cell.Background = new Background(Brushes.LightBlue);
            cell.AddText(orderTotal.ToString(), DefaultStyles.NormalStyle, TextAlignment.Center);
        }

        #endregion
    }

    public class OpenOrderValuesReport : ExcelBaseReport
    {
        #region Fields

        /// <summary>
        /// Dictionary of customer names and the row their table starts on 
        /// in the 'By Customer' worksheet
        /// </summary>
        private readonly Dictionary<string, int> _customerTableStartRows =
            new Dictionary<string, int>();

        #endregion

        #region Properties

        public override string Title => "Open Order Sales";

        protected override PageOrientation ReportPageOrientation => PageOrientation.Portrait;

        #endregion Properties

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            try
            {
                CreateWorkBook();
                var openOrders = GetOpenOrders();

                // Create the summary worksheet
                var wsSummary = CreateWorksheet(Title + " Summary");

                // Add the customer data
                var wsCustomer = this.CreateWorksheet("Values By Customer");
                this.AddCustomerData(wsCustomer, openOrders);

                // Add the summary data (done after customers are added so that we have reference to the row the customer table starts on)
                this.AddSummaryData(wsSummary, wsCustomer, openOrders);

                if (FieldUtilities.IsFieldEnabled("Order", "Product Class"))
                {
                    var wsProductClass = CreateWorksheet("Values By Product Class");
                    AddProductClassData(wsProductClass, openOrders);
                }

                // Add the 'all open order values' worksheet
                var wsAll = _workbook.Worksheets.Add("All Open Order Values");
                this.AddAllOpenOrderValuesWorksheet(wsAll, openOrders);
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to create Open Order Values Summary Report.");
            }
        }

        private static List<OrderItem> GetOpenOrders()
        {
            Data.Reports.OrdersReportTableAdapters.OrderProductClassTableAdapter taOrderProductClass = null;

            try
            {
                var orders = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter().GetOpenOrders();
                var fees = new Data.Reports.OrdersReportTableAdapters.OrderFeesTableAdapter().GetFeesByOpenOrders();
                var feeTypes = new Data.Reports.OrdersReportTableAdapters.OrderFeeTypeTableAdapter().GetData();

                taOrderProductClass = new Data.Reports.OrdersReportTableAdapters.OrderProductClassTableAdapter();

                // Get the order info
                var openOrders = new List<OrderItem>();
                foreach (var item in orders)
                {
                    var custRow = new OrderItem
                    {
                        CustomerName = item.CustomerName,
                        PartQuantity = item.IsPartQuantityNull() ? 0 : item.PartQuantity,
                        PriceUnit = item.PriceUnit,
                        WO = item.OrderID,
                        ReqDate = item.IsRequiredDateNull() ? (DateTime?) null : item.RequiredDate,
                        Priority = item.Priority,
                        PartName = item.PartName,
                        OrderType = (OrderType) item.OrderType,
                        EstShipDate = item.IsEstShipDateNull() ? (DateTime?) null : item.EstShipDate
                    };

                    var orderFees = 0m;

                    decimal weight = item.IsWeightNull() ? 0M : item.Weight;
                    foreach (var fee in fees.Where(of => of.OrderID == item.OrderID))
                    {
                        var feeType = feeTypes.FirstOrDefault(f => f.OrderFeeTypeID == fee.OrderFeeTypeID);
                        orderFees += OrderPrice.CalculateFees(feeType == null ? string.Empty : feeType.FeeType,
                            fee.Charge,
                            item.IsBasePriceNull() ? 0M : item.BasePrice,
                            item.IsPartQuantityNull() ? 0 : item.PartQuantity,
                            item.PriceUnit,
                            weight);
                    }

                    custRow.Price = OrderPrice.CalculatePrice(
                        item.IsBasePriceNull() ? 0M : item.BasePrice,
                        item.PriceUnit,
                        orderFees,
                        item.IsPartQuantityNull() ? 0 : item.PartQuantity,
                        weight);

                    custRow.ProductClass = taOrderProductClass.GetDataByOrder(item.OrderID).FirstOrDefault()?.ProductClass ?? "N/A";

                    openOrders.Add(custRow);
                }
                return openOrders;
            }
            finally
            {
                taOrderProductClass?.Dispose();
            }
        }

        private void AddSummaryData(Worksheet worksheet, Worksheet wsCustomer, List<OrderItem> orders)
        {
            try
            {
                // Add header
                var rowIndex = AddCompanyHeaderRows(worksheet, 4, "Summary") + 2;
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 4, "Summary");
                rowIndex++;

                this.CreateHeaderCell(worksheet, rowIndex, 0, "Customer");
                this.CreateHeaderCell(worksheet, rowIndex, 1, "Total Orders");
                this.CreateHeaderCell(worksheet, rowIndex, 2, "Total Parts");
                this.CreateHeaderCell(worksheet, rowIndex, 3, "Total Price");
                this.CreateHeaderCell(worksheet, rowIndex, 4, "Average Price Per Part");

                // Add summary info
                var startRowIndex = rowIndex;

                foreach (var customer in orders.GroupBy(c => c.CustomerName))
                {
                    var customerName = customer.Key;
                    rowIndex++;
                    var cell = this.CreateCell(worksheet, rowIndex, 0, customerName, false, HorizontalCellAlignment.Center);
                    if (_customerTableStartRows.ContainsKey(customerName))
                    {
                        var customerCell = wsCustomer.GetCell("A" + _customerTableStartRows[customerName]);
                        cell.ApplyFormula(@"=HYPERLINK(""#{0}"",""{1}"")".FormatWith(customerCell.ToString(), customerName));
                    }

                    this.CreateCell(worksheet, rowIndex, 1, "", false, HorizontalCellAlignment.Center).Value = customer.Count();
                    this.CreateCell(worksheet, rowIndex, 2, "", false, HorizontalCellAlignment.Center).Value = customer.Sum(o => o.PartQuantity);
                    this.CreateCell(worksheet, rowIndex, 3, customer.Sum(o => o.Price), false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
                    this.CreateCell(worksheet, rowIndex, 4, Convert.ToDecimal(worksheet.Rows[rowIndex].Cells[3].Value) / Convert.ToDecimal(worksheet.Rows[rowIndex].Cells[2].Value), false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
                }

                if (orders.Count == 0)
                {
                    this.CreateCell(worksheet, rowIndex, 1, string.Empty, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 2, string.Empty, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 3, string.Empty, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
                    this.CreateCell(worksheet, rowIndex, 4, string.Empty, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
                    rowIndex++;
                }

                // Add summary totals
                rowIndex++;
                this.CreateCell(worksheet, rowIndex, 0, "Total:", false, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateFormulaCell(worksheet, rowIndex, 1, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 2, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateFormulaCell(worksheet, rowIndex, 2, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 2, 3, rowIndex, 3), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateFormulaCell(worksheet, rowIndex, 3, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 2, 4, rowIndex, 4), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to create Open Order Values Summary Report.");
            }

            // Set column widths
            worksheet.Columns[0].SetWidth(50, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[1].SetWidth(16, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[2].SetWidth(16, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[3].SetWidth(20, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[4].SetWidth(25, WorksheetColumnWidthUnit.Character);
        }

        private void AddCustomerData(Worksheet worksheet, List<OrderItem> orders)
        {
            var rowIndex = 0;
            var customers = orders.GroupBy(row => row.CustomerName).ToList();
            foreach (var customerOrders in customers)
            {
                // Add header rows
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 7, customerOrders.Key);
                rowIndex++;
                _customerTableStartRows.Add(customerOrders.Key, rowIndex);

                this.CreateHeaderCell(worksheet, rowIndex, 0, "WO");
                this.CreateHeaderCell(worksheet, rowIndex, 1, "Type");
                this.CreateHeaderCell(worksheet, rowIndex, 2, "Date Required");
                this.CreateHeaderCell(worksheet, rowIndex, 3, "Part");
                this.CreateHeaderCell(worksheet, rowIndex, 4, "Qty");
                this.CreateHeaderCell(worksheet, rowIndex, 5, "Priority");
                this.CreateHeaderCell(worksheet, rowIndex, 6, "Unit");
                this.CreateHeaderCell(worksheet, rowIndex, 7, "Price");

                var startRowIndex = rowIndex + 2;
                foreach (var order in customerOrders)
                {
                    rowIndex++;
                    this.CreateCell(worksheet, rowIndex, 0, order.WO, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 1, order.OrderType, false, HorizontalCellAlignment.Center);

                    if (order.ReqDate.HasValue)
                    {
                        var fillColor = order.ReqDate >= DateTime.Now
                            ? CellFill.NoColor
                            : CellFill.CreateSolidFill(System.Drawing.Color.Red);

                        CreateCell(worksheet, rowIndex, 2, order.ReqDate.Value.Date, false,
                                HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, DATE_FORMAT).CellFormat.Fill = fillColor;
                    }
                    else
                    {
                        CreateCell(worksheet, rowIndex, 2, "NA");
                    }

                    this.CreateCell(worksheet, rowIndex, 3, order.PartName, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 4, order.PartQuantity, false, HorizontalCellAlignment.Center);

                    switch (order.Priority)
                    {
                        case "Expedite":
                        case "First ":
                        case "Weekend Expedite":
                            // red
                            this.CreateCell(worksheet, rowIndex, 5, order.Priority, false, HorizontalCellAlignment.Center).CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Red);
                            break;
                        case "Rush":
                            // orange
                            this.CreateCell(worksheet, rowIndex, 5, order.Priority, false, HorizontalCellAlignment.Center).CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Orange);
                            break;
                        default:
                            this.CreateCell(worksheet, rowIndex, 5, order.Priority, false, HorizontalCellAlignment.Center);
                            break;
                    }
                    this.CreateCell(worksheet, rowIndex, 6, order.PriceUnit, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 7, order.Price, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
                }

                // Add the totals for the group
                rowIndex++;
                this.CreateCell(worksheet, rowIndex, 0, "", false, HorizontalCellAlignment.Right);
                this.CreateCell(worksheet, rowIndex, 1, "Orders:", false, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateFormulaCell(worksheet, rowIndex, 2, "=COUNTA(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateCell(worksheet, rowIndex, 3, "Parts:", false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, DATE_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateFormulaCell(worksheet, rowIndex, 4, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 5, rowIndex, 5), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateCell(worksheet, rowIndex, 5, "", false, HorizontalCellAlignment.Right);
                this.CreateCell(worksheet, rowIndex, 6, "Price:", false, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateFormulaCell(worksheet, rowIndex, 7, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 8, rowIndex, 8), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

                rowIndex = rowIndex + 2;
            }

            // Set column widths
            worksheet.Columns[0].SetWidth(26, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[1].SetWidth(11, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[2].SetWidth(16, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[3].SetWidth(20, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[4].SetWidth(10, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[5].SetWidth(18, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[6].SetWidth(14, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[7].SetWidth(20, WorksheetColumnWidthUnit.Character);
        }

        private void AddProductClassData(Worksheet worksheet, List<OrderItem> orders)
        {
            const int lastColumnIndex = 8;

            var rowIndex = 0;
            var productClasses = orders
                .GroupBy(row => row.ProductClass)
                .OrderBy(g => g.Key)
                .ToList();

            foreach (var productClass in productClasses)
            {
                // Add header rows
                CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, lastColumnIndex, productClass.Key);
                rowIndex++;
                _customerTableStartRows.Add(productClass.Key, rowIndex);

                CreateHeaderCell(worksheet, rowIndex, 0, "WO");
                CreateHeaderCell(worksheet, rowIndex, 1, "Customer");
                CreateHeaderCell(worksheet, rowIndex, 2, "Type");
                CreateHeaderCell(worksheet, rowIndex, 3, "Date Required");
                CreateHeaderCell(worksheet, rowIndex, 4, "Part");
                CreateHeaderCell(worksheet, rowIndex, 5, "Qty");
                CreateHeaderCell(worksheet, rowIndex, 6, "Priority");
                CreateHeaderCell(worksheet, rowIndex, 7, "Unit");
                CreateHeaderCell(worksheet, rowIndex, 8, "Price");

                var startRowIndex = rowIndex + 2;
                foreach (var order in productClass)
                {
                    rowIndex++;
                    CreateCell(worksheet, rowIndex, 0, order.WO, false, HorizontalCellAlignment.Center);
                    CreateCell(worksheet, rowIndex, 1, order.CustomerName, false, HorizontalCellAlignment.Center);
                    CreateCell(worksheet, rowIndex, 2, order.OrderType, false, HorizontalCellAlignment.Center);

                    if (order.ReqDate.HasValue)
                    {
                        var fillColor = order.ReqDate >= DateTime.Now
                            ? CellFill.NoColor
                            : CellFill.CreateSolidFill(System.Drawing.Color.Red);

                        CreateCell(worksheet, rowIndex, 3, order.ReqDate.Value.Date, false,
                                HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, DATE_FORMAT).CellFormat.Fill = fillColor;
                    }
                    else
                    {
                        CreateCell(worksheet, rowIndex, 3, "NA");
                    }

                    CreateCell(worksheet, rowIndex, 4, order.PartName, false, HorizontalCellAlignment.Center);
                    CreateCell(worksheet, rowIndex, 5, order.PartQuantity, false, HorizontalCellAlignment.Center);

                    switch (order.Priority)
                    {
                        case "Expedite":
                        case "First ":
                        case "Weekend Expedite":
                            // red
                            CreateCell(worksheet, rowIndex, 6, order.Priority, false, HorizontalCellAlignment.Center).CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Red);
                            break;
                        case "Rush":
                            // orange
                            CreateCell(worksheet, rowIndex, 6, order.Priority, false, HorizontalCellAlignment.Center).CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Orange);
                            break;
                        default:
                            CreateCell(worksheet, rowIndex, 6, order.Priority, false, HorizontalCellAlignment.Center);
                            break;
                    }

                    CreateCell(worksheet, rowIndex, 7, order.PriceUnit, false, HorizontalCellAlignment.Center);
                    CreateCell(worksheet, rowIndex, 8, order.Price, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
                }

                // Add the totals for the group
                rowIndex++;
                CreateCell(worksheet, rowIndex, 0, string.Empty, false, HorizontalCellAlignment.Right);
                CreateCell(worksheet, rowIndex, 1, "Orders:", false, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 2, "=COUNTA(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateCell(worksheet, rowIndex, 3, string.Empty, false, HorizontalCellAlignment.Right);
                CreateCell(worksheet, rowIndex, 4, "Parts:", false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, DATE_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 5, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 6, rowIndex, 6), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateCell(worksheet, rowIndex, 6, string.Empty, false, HorizontalCellAlignment.Right);
                CreateCell(worksheet, rowIndex, 7, "Price:", false, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 8, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 9, rowIndex, 9), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

                rowIndex = rowIndex + 2;
            }

            // Set column widths
            worksheet.Columns[0].SetWidth(26, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[1].SetWidth(16, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[2].SetWidth(11, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[3].SetWidth(16, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[4].SetWidth(20, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[5].SetWidth(10, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[6].SetWidth(18, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[7].SetWidth(14, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[8].SetWidth(20, WorksheetColumnWidthUnit.Character);
        }

        private void AddAllOpenOrderValuesWorksheet(Worksheet worksheet, List<OrderItem> orders)
        {
            const int lastColumnIndex = 11;

            var rowIndex = 0;

            // Add header rows
            this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, lastColumnIndex, "All Open Order Values");
            rowIndex++;

            this.CreateHeaderCell(worksheet, rowIndex, 0, "Customer");
            this.CreateHeaderCell(worksheet, rowIndex, 1, "WO");
            this.CreateHeaderCell(worksheet, rowIndex, 2, "Product Class");
            this.CreateHeaderCell(worksheet, rowIndex, 3, "Type");
            this.CreateHeaderCell(worksheet, rowIndex, 4, "Required Date");
            this.CreateHeaderCell(worksheet, rowIndex, 5, "Part");
            this.CreateHeaderCell(worksheet, rowIndex, 6, "Qty");
            this.CreateHeaderCell(worksheet, rowIndex, 7, "Priority");
            this.CreateHeaderCell(worksheet, rowIndex, 8, "Unit");
            this.CreateHeaderCell(worksheet, rowIndex, 9, "Price");
            this.CreateHeaderCell(worksheet, rowIndex, 10, "Est Ship Date");
            this.CreateHeaderCell(worksheet, rowIndex, 11, "Days Late");

            var startRowIndex = rowIndex + 2;
            foreach (var order in orders)
            {
                rowIndex++;
                this.CreateCell(worksheet, rowIndex, 0, order.CustomerName);
                this.CreateCell(worksheet, rowIndex, 1, order.WO, false, HorizontalCellAlignment.Center);
                this.CreateCell(worksheet, rowIndex, 2, order.ProductClass, false, HorizontalCellAlignment.Center);
                this.CreateCell(worksheet, rowIndex, 3, order.OrderType, false, HorizontalCellAlignment.Center);

                if (order.ReqDate.HasValue)
                {
                    var cellFillColor = order.ReqDate >= DateTime.Now
                        ? CellFill.NoColor
                        : CellFill.CreateSolidFill(System.Drawing.Color.Red);

                    CreateCell(worksheet, rowIndex, 4, order.ReqDate.Value.Date, false, HorizontalCellAlignment.Center,
                            CellBorderLineStyle.Thin, DATE_FORMAT).CellFormat.Fill = cellFillColor;
                }
                else
                {
                    CreateCell(worksheet, rowIndex, 4, "NA");
                }
                this.CreateCell(worksheet, rowIndex, 5, order.PartName, false, HorizontalCellAlignment.Center);
                this.CreateCell(worksheet, rowIndex, 6, order.PartQuantity, false, HorizontalCellAlignment.Center);
                switch (order.Priority)
                {
                    case "Expedite":
                    case "First ":
                    case "Weekend Expedite":
                        // red
                        this.CreateCell(worksheet, rowIndex, 7, order.Priority, false, HorizontalCellAlignment.Center).CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Red);
                        break;
                    case "Rush":
                        // orange
                        this.CreateCell(worksheet, rowIndex, 7, order.Priority, false, HorizontalCellAlignment.Center).CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Orange);
                        break;
                    default:
                        this.CreateCell(worksheet, rowIndex, 7, order.Priority, false, HorizontalCellAlignment.Center);
                        break;
                }
                this.CreateCell(worksheet, rowIndex, 8, order.PriceUnit, false, HorizontalCellAlignment.Center);
                this.CreateCell(worksheet, rowIndex, 9, order.Price, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);

                this.CreateCell(worksheet, rowIndex, 10, order.EstShipDate, false, HorizontalCellAlignment.Center);

                if (order.EstShipDate.HasValue)
                {
                    var timespan = order.EstShipDate - DateTime.Now;
                    var days = timespan.Value.Days;
                    this.CreateCell(worksheet, rowIndex, 11, days, false, HorizontalCellAlignment.Right);
                }
                else
                {
                    this.CreateCell(worksheet, rowIndex, 11, "NA", false, HorizontalCellAlignment.Right);
                }
            }

            if (orders.Count == 0)
            {
                rowIndex++;
                for (int colIndex = 0; colIndex < 12; colIndex++)
                {
                    this.CreateCell(worksheet, rowIndex, colIndex, string.Empty, false, HorizontalCellAlignment.Right);
                }
            }

            // Add the totals for the group
            rowIndex++;
            CreateCell(worksheet, rowIndex, 0, "Orders:", false, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 1, "=COUNTA(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 2, string.Empty, false, HorizontalCellAlignment.Right);
            CreateCell(worksheet, rowIndex, 3, string.Empty, false, HorizontalCellAlignment.Right);
            CreateCell(worksheet, rowIndex, 4, string.Empty, false, HorizontalCellAlignment.Right);
            CreateCell(worksheet, rowIndex, 5, "Parts:", false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, DATE_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 6, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 7, rowIndex, 7), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 7, string.Empty, false, HorizontalCellAlignment.Right);
            CreateCell(worksheet, rowIndex, 8, "Price:", false, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 9, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 10, rowIndex, 10), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 10, string.Empty, false, HorizontalCellAlignment.Right);
            CreateCell(worksheet, rowIndex, 11, string.Empty, false, HorizontalCellAlignment.Right);

            // Set colum widths
            worksheet.Columns[00].SetWidth(16, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[01].SetWidth(8, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[02].SetWidth(16, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[03].SetWidth(11, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[04].SetWidth(16, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[05].SetWidth(20, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[06].SetWidth(7, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[07].SetWidth(18, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[08].SetWidth(7, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[09].SetWidth(20, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[10].SetWidth(11, WorksheetColumnWidthUnit.Character);
        }

        #endregion Methods

        #region CustomerRow

        private class OrderItem
        {
            public string CustomerName { get; set; }
            public int WO { get; set; }
            public DateTime? ReqDate { get; set; }
            public DateTime? EstShipDate { get; set; }
            public string Priority { get; set; }
            public int PartQuantity { get; set; }
            public decimal Price { get; set; }
            public string PriceUnit { get; set; }
            public string PartName { get; set; }
            public OrderType OrderType { get; set; }
            public string ProductClass { get; set; }
        }

        #endregion CustomerRow
    }

    public class LostAndQuarantinedOrdersReport : ExcelBaseReport
    {
        #region Fields

        private DateTime _fromDate;
        private DateTime _toDate;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public override string Title
        {
            get
            {
                return "Lost and Quarantined Orders";
            }
        }

        /// <summary>
        /// Gets the report page orientation.
        /// </summary>
        /// <value>
        /// The report page orientation.
        /// </value>
        protected override PageOrientation ReportPageOrientation
        {
            get
            {
                return PageOrientation.Portrait;
            }
        }

        [Browsable(true)]
        [Description("The from date of the report.")]
        [DisplayName("From Date")]
        [Category("Report")]
        public DateTime FromDate
        {
            get { return this._fromDate; }
            set { this._fromDate = value; }
        }

        [Browsable(true)]
        [Description("The to date of the report.")]
        [DisplayName("To Date")]
        [Category("Report")]
        public DateTime ToDate
        {
            get { return this._toDate; }
            set { this._toDate = value; }
        }

        #endregion Properties

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        /// <summary>
        /// Creates the report.
        /// </summary>
        private void CreateReport()
        {
            CreateWorkBook();

            OrdersReport.OrderDataTable orders = null;
            var taOrders = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter();
            orders = taOrders.GetDataByLostAndQuarantined(_fromDate, _toDate);

            // Create the 'summary' worksheet and add the header
            var wsSummary = CreateWorksheet("Summary");
            var rowIndex = base.AddCompanyHeaderRows(wsSummary, 3, "Summary") + 2;
            this.AddSummaryData(wsSummary, orders, rowIndex);
            wsSummary.Columns[0].SetWidth(40, WorksheetColumnWidthUnit.Character);
            wsSummary.Columns[1].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsSummary.Columns[2].SetWidth(18, WorksheetColumnWidthUnit.Character);
            wsSummary.Columns[3].SetWidth(12, WorksheetColumnWidthUnit.Character);

            // Create the 'All' worksheet
            var wsAll = this.CreateWorksheet("All");
            this.AddAllData(wsAll, orders);
            wsAll.Columns[0].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[1].SetWidth(17, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[2].SetWidth(17, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[3].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[4].SetWidth(8, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[5].SetWidth(21, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[6].SetWidth(16, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[7].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[8].SetWidth(21, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[9].SetWidth(12, WorksheetColumnWidthUnit.Character);

            //Create the 'by customers' worksheet
            var wsCustomers = this.CreateWorksheet("By Customer");
            this.AddDataByCustomer(wsCustomers, orders);
            wsCustomers.Columns[0].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wsCustomers.Columns[1].SetWidth(17, WorksheetColumnWidthUnit.Character);
            wsCustomers.Columns[2].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsCustomers.Columns[3].SetWidth(17, WorksheetColumnWidthUnit.Character);
            wsCustomers.Columns[4].SetWidth(8, WorksheetColumnWidthUnit.Character);
            wsCustomers.Columns[5].SetWidth(21, WorksheetColumnWidthUnit.Character);
            wsCustomers.Columns[6].SetWidth(16, WorksheetColumnWidthUnit.Character);
            wsCustomers.Columns[7].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsCustomers.Columns[8].SetWidth(21, WorksheetColumnWidthUnit.Character);
            wsCustomers.Columns[9].SetWidth(12, WorksheetColumnWidthUnit.Character);

            // Create the 'by reason' worksheet
            var wsReasons = this.CreateWorksheet("By Reason");
            this.AddDataByReason(wsReasons, orders);
            wsReasons.Columns[0].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsReasons.Columns[1].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wsReasons.Columns[2].SetWidth(17, WorksheetColumnWidthUnit.Character);
            wsReasons.Columns[3].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsReasons.Columns[4].SetWidth(17, WorksheetColumnWidthUnit.Character);
            wsReasons.Columns[5].SetWidth(8, WorksheetColumnWidthUnit.Character);
            wsReasons.Columns[6].SetWidth(21, WorksheetColumnWidthUnit.Character);
            wsReasons.Columns[7].SetWidth(16, WorksheetColumnWidthUnit.Character);
            wsReasons.Columns[8].SetWidth(21, WorksheetColumnWidthUnit.Character);
            wsReasons.Columns[9].SetWidth(12, WorksheetColumnWidthUnit.Character);
        }

        /// <summary>
        /// Adds the data by reason.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="orders">The orders.</param>
        private void AddDataByReason(Worksheet worksheet, OrdersReport.OrderDataTable orders)
        {
            try
            {
                var rowIndex = 0;

                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, "Date(s): " + _fromDate.ToShortDateString() + " to " + _toDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                var ordersByReason = this.GetByReason(orders);

                foreach (var group in ordersByReason)
                {
                    var reasonStartRowIndex = rowIndex;

                    // Add the customer name section
                    this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, group.Key).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    rowIndex++;

                    // Add the headers
                    this.CreateHeaderCell(worksheet, rowIndex, 0, "Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 1, "Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 2, "Type").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 3, "WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 4, "Original WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 5, "PO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 6, "Department").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 7, "Est. Ship Date").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 8, "Process").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 9, "Part Qty").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    rowIndex++;

                    foreach (var order in group.Value)
                    {
                        var dt = new OrdersDataSet.InternalReworkDataTable();
                        using (var internalReworkTableAdapter = new InternalReworkTableAdapter() { ClearBeforeFill = true })
                        {
                            internalReworkTableAdapter.FillByOriginalOrderID(dt, order.OrderID);
                            if (dt.Rows.Count == 0)
                            {
                                // try getting data by rework ID
                                internalReworkTableAdapter.FillByReworkOrderID(dt, order.OrderID);
                            }
                        }

                        this.CreateCell(worksheet, rowIndex, 0, group.Key, false, HorizontalCellAlignment.Center);
                        this.CreateCell(worksheet, rowIndex, 1, order.CustomerName, false, HorizontalCellAlignment.Center);

                        switch (order.OrderType)
                        {
                            // OrderType = 6 is lost orders
                            // OrderType = 7 is quarantined orders
                            case 6:
                                this.CreateCell(worksheet, rowIndex, 2, "Lost", false, HorizontalCellAlignment.Center);
                                break;
                            case 7:
                                this.CreateCell(worksheet, rowIndex, 2, "Quarantined", false, HorizontalCellAlignment.Center);
                                break;
                        }

                        this.CreateCell(worksheet, rowIndex, 5, order.IsPurchaseOrderNull() ? "NA" : order.PurchaseOrder, false, HorizontalCellAlignment.Center);
                        if (!order.IsEstShipDateNull())
                            this.CreateCell(worksheet, rowIndex, 7, order.EstShipDate.Date, false, HorizontalCellAlignment.Center);
                        this.CreateCell(worksheet, rowIndex, 9, order.IsPartQuantityNull() ? "NA" : order.PartQuantity.ToString(), false, HorizontalCellAlignment.Center);

                        if (dt.Rows.Count > 0)
                        {
                            this.CreateCell(worksheet, rowIndex, 3, dt[0].IsReworkOrderIDNull() ? "NA" : dt[0].ReworkOrderID.ToString(), false, HorizontalCellAlignment.Center);
                            this.CreateCell(worksheet, rowIndex, 4, dt[0].OriginalOrderID, false, HorizontalCellAlignment.Center);
                            this.CreateCell(worksheet, rowIndex, 6, dt[0].IsHoldLocationIDNull() ? "NA" : dt[0].HoldLocationID, false, HorizontalCellAlignment.Center);

                            var paDt = new ProcessesDataset.ProcessAliasDataTable();
                            if (!dt[0].IsProcessAliasIDNull())
                            {
                                using (var taProcessAlias = new Data.Datasets.ProcessesDatasetTableAdapters.ProcessAliasTableAdapter())
                                    taProcessAlias.GetAliasName(paDt, dt[0].ProcessAliasID);
                            }

                            this.CreateCell(worksheet, rowIndex, 8, paDt.Rows.Count > 0 ? paDt[0].Name : "NA", false, HorizontalCellAlignment.Center);
                        }
                        else
                        {
                            this.CreateCell(worksheet, rowIndex, 3, order.OrderID, false, HorizontalCellAlignment.Center);
                            this.CreateCell(worksheet, rowIndex, 4, "NA", false, HorizontalCellAlignment.Center);
                            this.CreateCell(worksheet, rowIndex, 6, "NA", false, HorizontalCellAlignment.Center);
                            this.CreateCell(worksheet, rowIndex, 8, "NA", false, HorizontalCellAlignment.Center);
                        }

                        rowIndex++;
                    }

                    // Add the reason total
                    var cell = this.CreateCell(worksheet, rowIndex, 0, "Total:");
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 1, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 7, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell = this.CreateFormulaCell(worksheet, rowIndex, 9, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(reasonStartRowIndex + 3, 1, rowIndex, 1), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex = rowIndex + 2; // skip a line between reasons
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to create Lost and Quarantined By Reason Report.");
            }
        }

        /// <summary>
        /// Adds the data by customer.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="orders">The orders.</param>
        private void AddDataByCustomer(Worksheet worksheet, OrdersReport.OrderDataTable orders)
        {
            try
            {
                var rowIndex = 0;

                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, "Date(s): " + _fromDate.ToShortDateString() + " to " + _toDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                var ordersByCustomer = orders.GroupBy(o => o.CustomerName).ToList();
                var startRowIndex = rowIndex; // used for totals formulas, need to add 1 for row indexing in formula
                foreach (var customerOrders in ordersByCustomer)
                {
                    var customerStartRowIndex = rowIndex;

                    // Add the customer name section
                    this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, customerOrders.Key).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    rowIndex++;

                    // Add the headers
                    this.CreateHeaderCell(worksheet, rowIndex, 0, "Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 1, "Type").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 2, "WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 3, "Original WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 4, "PO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 5, "Department").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 6, "Est. Ship Date").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 7, "Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 8, "Process").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 9, "Part Qty").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    rowIndex++;

                    // Add the orders
                    foreach (var order in customerOrders)
                    {
                        var dt = new OrdersDataSet.InternalReworkDataTable();
                        using (var internalReworkTableAdapter = new InternalReworkTableAdapter() { ClearBeforeFill = true })
                        {
                            internalReworkTableAdapter.FillByOriginalOrderID(dt, order.OrderID);
                            if (dt.Rows.Count == 0)
                            {
                                // try getting data by rework ID
                                internalReworkTableAdapter.FillByReworkOrderID(dt, order.OrderID);
                            }
                        }

                        var reasons = new ListsDataSet.d_ReworkReasonDataTable();
                        using (var taReworkReasons = new Data.Datasets.ListsDataSetTableAdapters.d_ReworkReasonTableAdapter())
                            taReworkReasons.Fill(reasons);

                        ListsDataSet.d_ReworkReasonRow reason = null;
                        if (dt.Rows.Count > 0)
                        {
                            reason = reasons.FindByReworkReasonID(dt[0].ReworkReasonID);
                        }

                        this.CreateCell(worksheet, rowIndex, 0, order.CustomerName, false, HorizontalCellAlignment.Center);
                        switch (order.OrderType)
                        {
                            // OrderType = 6 is lost orders
                            // OrderType = 7 is quarantined orders
                            case 6:
                                this.CreateCell(worksheet, rowIndex, 1, "Lost", false, HorizontalCellAlignment.Center);
                                break;
                            case 7:
                                this.CreateCell(worksheet, rowIndex, 1, "Quarantined", false, HorizontalCellAlignment.Center);
                                break;
                        }


                        this.CreateCell(worksheet, rowIndex, 4, order.IsPurchaseOrderNull() ? "NA" : order.PurchaseOrder, false, HorizontalCellAlignment.Center);
                        if (!order.IsEstShipDateNull())
                            this.CreateCell(worksheet, rowIndex, 6, order.EstShipDate.Date, false, HorizontalCellAlignment.Center);
                        this.CreateCell(worksheet, rowIndex, 7, reason != null ? reason.Name : "Other", false, HorizontalCellAlignment.Center);
                        this.CreateCell(worksheet, rowIndex, 9, order.IsPartQuantityNull() ? "NA" : order.PartQuantity.ToString(), false, HorizontalCellAlignment.Center);

                        if (dt.Rows.Count > 0)
                        {
                            this.CreateCell(worksheet, rowIndex, 2, dt[0].IsReworkOrderIDNull() ? "NA" : dt[0].ReworkOrderID.ToString(), false, HorizontalCellAlignment.Center);
                            this.CreateCell(worksheet, rowIndex, 3, dt[0].OriginalOrderID, false, HorizontalCellAlignment.Center);
                            this.CreateCell(worksheet, rowIndex, 5, dt[0].IsHoldLocationIDNull() ? "NA" : dt[0].HoldLocationID, false, HorizontalCellAlignment.Center);

                            var paDt = new ProcessesDataset.ProcessAliasDataTable();
                            if (!dt[0].IsProcessAliasIDNull())
                            {
                                using (var taProcessAlias = new Data.Datasets.ProcessesDatasetTableAdapters.ProcessAliasTableAdapter())
                                    taProcessAlias.GetAliasName(paDt, dt[0].ProcessAliasID);
                            }

                            this.CreateCell(worksheet, rowIndex, 8, paDt.Rows.Count > 0 ? paDt[0].Name : "NA", false, HorizontalCellAlignment.Center);
                        }
                        else
                        {
                            this.CreateCell(worksheet, rowIndex, 2, order.OrderID, false, HorizontalCellAlignment.Center);
                            this.CreateCell(worksheet, rowIndex, 3, "NA", false, HorizontalCellAlignment.Center);
                            this.CreateCell(worksheet, rowIndex, 6, "NA", false, HorizontalCellAlignment.Center);
                            this.CreateCell(worksheet, rowIndex, 8, "NA", false, HorizontalCellAlignment.Center);
                        }

                        rowIndex++;
                    }

                    // Add the customer total
                    var cell = this.CreateCell(worksheet, rowIndex, 0, "Total:");
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 1, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 7, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell = this.CreateFormulaCell(worksheet, rowIndex, 9, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(customerStartRowIndex + 3, 1, rowIndex, 1), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex = rowIndex + 2; // skip a line between customers
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to create Lost and Quarantined By Customer Report.");
            }
        }

        /// <summary>
        /// Adds all data.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="orders">The orders.</param>
        private void AddAllData(Worksheet worksheet, OrdersReport.OrderDataTable orders)
        {
            try
            {
                var rowIndex = 0;

                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, "Date(s): " + _fromDate.ToShortDateString() + " to " + _toDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the column headers
                this.CreateHeaderCell(worksheet, rowIndex, 0, "WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 1, "Type").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 2, "Original WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 3, "Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 4, "PO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 5, "Department").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 6, "Est. Ship Date").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 7, "Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 8, "Process").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 9, "Part Qty").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                var startRowIndex = rowIndex;
                foreach (var order in orders)
                {
                    var dt = new OrdersDataSet.InternalReworkDataTable();
                    using (var internalReworkTableAdapter = new InternalReworkTableAdapter() { ClearBeforeFill = true })
                    {
                        internalReworkTableAdapter.FillByOriginalOrderID(dt, order.OrderID);
                        if (dt.Rows.Count == 0)
                        {
                            // try getting data by rework ID
                            internalReworkTableAdapter.FillByReworkOrderID(dt, order.OrderID);
                        }
                    }

                    var reasons = new ListsDataSet.d_ReworkReasonDataTable();
                    using (var taReworkReasons = new Data.Datasets.ListsDataSetTableAdapters.d_ReworkReasonTableAdapter())
                        taReworkReasons.Fill(reasons);

                    ListsDataSet.d_ReworkReasonRow reason = null;
                    if (dt.Rows.Count > 0)
                    {
                        reason = reasons.FindByReworkReasonID(dt[0].ReworkReasonID);
                    }

                    switch (order.OrderType)
                    {
                        // OrderType = 6 is lost orders
                        // OrderType = 7 is quarantined orders
                        case 6:
                            this.CreateCell(worksheet, rowIndex, 1, "Lost", false, HorizontalCellAlignment.Center);
                            break;
                        case 7:
                            this.CreateCell(worksheet, rowIndex, 1, "Quarantined", false, HorizontalCellAlignment.Center);
                            break;
                    }


                    this.CreateCell(worksheet, rowIndex, 3, order.CustomerName, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 4, order.IsPurchaseOrderNull() ? "NA" : order.PurchaseOrder, false, HorizontalCellAlignment.Center);
                    if (!order.IsEstShipDateNull())
                        this.CreateCell(worksheet, rowIndex, 6, order.EstShipDate.Date, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 7, reason != null ? reason.Name : "Other", false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 9, order.IsPartQuantityNull() ? "NA" : order.PartQuantity.ToString(), false, HorizontalCellAlignment.Center);

                    if (dt.Rows.Count > 0)
                    {
                        this.CreateCell(worksheet, rowIndex, 0, dt[0].IsReworkOrderIDNull() ? "NA" : dt[0].ReworkOrderID.ToString(), false, HorizontalCellAlignment.Center);
                        this.CreateCell(worksheet, rowIndex, 2, dt[0].OriginalOrderID, false, HorizontalCellAlignment.Center);
                        this.CreateCell(worksheet, rowIndex, 5, dt[0].IsHoldLocationIDNull() ? "NA" : dt[0].HoldLocationID, false, HorizontalCellAlignment.Center);

                        var paDt = new ProcessesDataset.ProcessAliasDataTable();
                        if (!dt[0].IsProcessAliasIDNull())
                        {
                            using (var taProcessAlias = new Data.Datasets.ProcessesDatasetTableAdapters.ProcessAliasTableAdapter())
                                taProcessAlias.GetAliasName(paDt, dt[0].ProcessAliasID);
                        }

                        this.CreateCell(worksheet, rowIndex, 8, paDt.Rows.Count > 0 ? paDt[0].Name : "NA", false, HorizontalCellAlignment.Center);
                    }
                    else
                    {
                        this.CreateCell(worksheet, rowIndex, 0, order.OrderID, false, HorizontalCellAlignment.Center);
                        this.CreateCell(worksheet, rowIndex, 2, "NA", false, HorizontalCellAlignment.Center);
                        this.CreateCell(worksheet, rowIndex, 5, "NA", false, HorizontalCellAlignment.Center);
                        this.CreateCell(worksheet, rowIndex, 8, "NA", false, HorizontalCellAlignment.Center);
                    }

                    rowIndex++;
                }

                // Add the total count
                var cell = this.CreateCell(worksheet, rowIndex, 0, "Total:");
                cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateCell(worksheet, rowIndex, 1, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 7, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                cell = this.CreateFormulaCell(worksheet, rowIndex, 9, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to create Lost and Quarantined All Orders Report.");
            }
        }

        /// <summary>
        /// Adds the summary data.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="orders">The orders.</param>
        /// <param name="rowIndex">Index of the row.</param>
        private void AddSummaryData(Worksheet worksheet, OrdersReport.OrderDataTable orders, int rowIndex)
        {
            try
            {
                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 3, "Date(s): " + _fromDate.ToShortDateString() + " to " + _toDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the 'All' section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 3, "All Orders").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the column headers
                this.CreateHeaderCell(worksheet, rowIndex, 0, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 1, "Lost").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 2, "Quarantined").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 3, "Total").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the data totals
                var lostOrders = orders.Where(o => o.OrderType == 6).ToList();
                var quarantinedOrders = orders.Where(o => o.OrderType == 7).ToList();
                this.CreateCell(worksheet, rowIndex, 0, "Total:").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 1, lostOrders.Count, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 2, quarantinedOrders.Count, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateFormulaCell(worksheet, rowIndex, 3, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(rowIndex + 1, 2, rowIndex + 1, 3), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add orders by customer section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 3, "Orders By Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                var ordersByCustomer = orders.GroupBy(o => o.CustomerName).ToList();
                var startRowIndex = rowIndex; // used for totals formulas, need to add 1 for row indexing in formula
                foreach (var order in ordersByCustomer)
                {
                    var lostOrdersCount = 0;
                    var quarantinedOrdersCount = 0;

                    // add the customer name
                    this.CreateCell(worksheet, rowIndex, 0, order.Key);

                    foreach (var row in order)
                    {
                        // OrderType = 6 is lost orders
                        // OrderType = 7 is quarantined orders
                        switch (row.OrderType)
                        {
                            case 6:
                                lostOrdersCount += 1;
                                break;
                            case 7:
                                quarantinedOrdersCount += 1;
                                break;
                        }
                    }

                    // Set the data from the last customer before moving to the next
                    this.CreateCell(worksheet, rowIndex, 1, lostOrdersCount, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 2, quarantinedOrdersCount, false, HorizontalCellAlignment.Center);
                    this.CreateFormulaCell(worksheet, rowIndex, 3, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(rowIndex + 1, 2, rowIndex + 1, 3), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    rowIndex++;
                }

                if (ordersByCustomer.Count == 0)
                {
                    this.CreateCell(worksheet, rowIndex, 1, 0, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 2, 0, false, HorizontalCellAlignment.Center);
                    this.CreateFormulaCell(worksheet, rowIndex, 3, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(rowIndex + 1, 2, rowIndex + 1, 3), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    rowIndex++;
                }

                this.CreateCell(worksheet, rowIndex, 0, "Total:").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateFormulaCell(worksheet, rowIndex, 1, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateFormulaCell(worksheet, rowIndex, 2, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 3, rowIndex, 3), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateFormulaCell(worksheet, rowIndex, 3, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 4, rowIndex, 4), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                //By reason
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 3, "Orders By Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                startRowIndex = rowIndex; // used for totals formulas, need to add 1 for row indexing in formula
                var byReasonDict = this.GetByReason(orders);
                foreach (var item in byReasonDict)
                {
                    var lostOrdersCount = 0;
                    var quarantinedOrdersCount = 0;

                    this.CreateCell(worksheet, rowIndex, 0, item.Key);

                    foreach (var order in item.Value)
                    {
                        // OrderType = 6 is lost orders
                        // OrderType = 7 is quarantined orders
                        switch (order.OrderType)
                        {
                            case 6:
                                lostOrdersCount += 1;
                                break;
                            case 7:
                                quarantinedOrdersCount += 1;
                                break;
                        }
                    }

                    this.CreateCell(worksheet, rowIndex, 1, lostOrdersCount, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 2, quarantinedOrdersCount, false, HorizontalCellAlignment.Center);
                    this.CreateFormulaCell(worksheet, rowIndex, 3, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(rowIndex + 1, 2, rowIndex + 1, 3), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    rowIndex++;
                }

                if (byReasonDict.Count == 0)
                {
                    this.CreateCell(worksheet, rowIndex, 1, 0, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 2, 0, false, HorizontalCellAlignment.Center);
                    this.CreateFormulaCell(worksheet, rowIndex, 3, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(rowIndex + 1, 2, rowIndex + 1, 3), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    rowIndex++;
                }

                this.CreateCell(worksheet, rowIndex, 0, "Total:").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateFormulaCell(worksheet, rowIndex, 1, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateFormulaCell(worksheet, rowIndex, 2, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 3, rowIndex, 3), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateFormulaCell(worksheet, rowIndex, 3, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 4, rowIndex, 4), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to create Lost and Quarantined Summary Report.");
            }
        }

        /// <summary>
        /// Gets the data by reason.
        /// </summary>
        /// <param name="orders">The orders.</param>
        /// <returns>Dictionary; KEY = reason order is lost/quarantined, VALUE = list of OrdersReport.OrderRow objects</returns>
        private Dictionary<string, List<OrdersReport.OrderRow>> GetByReason(OrdersReport.OrderDataTable orders)
        {
            var byReasonDictionary = new Dictionary<string, List<OrdersReport.OrderRow>>();

            try
            {
                foreach (var order in orders)
                {
                    var dt = new OrdersDataSet.InternalReworkDataTable();
                    using (var internalReworkTableAdapter = new InternalReworkTableAdapter() { ClearBeforeFill = true })
                    {
                        internalReworkTableAdapter.FillByOriginalOrderID(dt, order.OrderID);
                        if (dt.Rows.Count == 0)
                        {
                            // try getting data by rework ID
                            internalReworkTableAdapter.FillByReworkOrderID(dt, order.OrderID);
                        }
                    }

                    var reasons = new ListsDataSet.d_ReworkReasonDataTable();
                    using (
                        var taReworkReasons = new Data.Datasets.ListsDataSetTableAdapters.d_ReworkReasonTableAdapter()) taReworkReasons.Fill(reasons);

                    ListsDataSet.d_ReworkReasonRow reason = null;
                    if (dt.Rows.Count > 0)
                    {
                        reason = reasons.FindByReworkReasonID(dt[0].ReworkReasonID);
                    }

                    if (reason != null)
                    {
                        if (byReasonDictionary.ContainsKey(reason.Name))
                        {
                            var list = byReasonDictionary[reason.Name];
                            list.Add(order);
                            byReasonDictionary[reason.Name] = list;
                        }
                        else
                        {
                            byReasonDictionary.Add(reason.Name, new List<OrdersReport.OrderRow> { order });
                        }
                    }
                    else
                    {
                        if (byReasonDictionary.ContainsKey("Other"))
                        {
                            var list = byReasonDictionary["Other"];
                            list.Add(order);
                            byReasonDictionary["Other"] = list;
                        }
                        else
                        {
                            byReasonDictionary.Add("Other", new List<OrdersReport.OrderRow> { order });
                        }
                    }
                }

                return byReasonDictionary;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to get orders by reason.");
                return byReasonDictionary;
            }
        }

        #endregion Methods
    }

    public class OrdersOnHoldReport : ExcelBaseReport
    {
        #region Fields

        private DateTime _fromDate;
        private DateTime _toDate;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public override string Title
        {
            get { return "Orders On Hold"; }
        }

        /// <summary>
        /// Gets the report page orientation.
        /// </summary>
        /// <value>
        /// The report page orientation.
        /// </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }


        [Browsable(true)]
        [Description("The from date of the report.")]
        [DisplayName("From Date")]
        [Category("Report")]
        public DateTime FromDate
        {
            get { return this._fromDate; }
            set { this._fromDate = value; }
        }

        [Browsable(true)]
        [Description("The to date of the report.")]
        [DisplayName("To Date")]
        [Category("Report")]
        public DateTime ToDate
        {
            get { return this._toDate; }
            set { this._toDate = value; }
        }

        #endregion Properties

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        /// <summary>
        /// Creates the report.
        /// </summary>
        private void CreateReport()
        {
            OrdersReport dsOrderReport = new OrdersReport();
            dsOrderReport.EnforceConstraints = false;

            var taOrders = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter();
            taOrders.FillByOrdersOnHold(dsOrderReport.Order, _fromDate, _toDate);

            using (var taOrderSerialNumbers = new Data.Reports.OrdersReportTableAdapters.OrderSerialNumberTableAdapter())
                taOrderSerialNumbers.FillActive(dsOrderReport.OrderSerialNumber);

            using (var taOrderProductClass = new Data.Reports.OrdersReportTableAdapters.OrderProductClassTableAdapter())
            {
                taOrderProductClass.Fill(dsOrderReport.OrderProductClass);
            }

            // Create the summary worksheet
            var wsSummary = this.CreateWorksheet("Summary");
            var rowIndex = this.AddCompanyHeaderRows(wsSummary, 1, "Summary") + 2;
            this.AddSummaryData(wsSummary, dsOrderReport.Order, rowIndex);
            wsSummary.Columns[0].SetWidth(50, WorksheetColumnWidthUnit.Character);
            wsSummary.Columns[1].SetWidth(25, WorksheetColumnWidthUnit.Character);

            var orderData = CreateOrderData(dsOrderReport.Order);

            // Create the 'All' worksheet
            var wsAll = this.CreateWorksheet("All");
            this.AddAllData(wsAll, orderData);
            wsAll.Columns[0].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[1].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[2].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[3].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[4].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[5].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[6].SetWidth(35, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[7].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[8].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[9].SetWidth(30, WorksheetColumnWidthUnit.Character);

            // Create the 'By Customer' worksheet
            var wsByCustomer = this.CreateWorksheet("By Customer");
            AddDataByCustomer(wsByCustomer, orderData);
            wsByCustomer.Columns[0].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsByCustomer.Columns[1].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wsByCustomer.Columns[2].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsByCustomer.Columns[3].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsByCustomer.Columns[4].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsByCustomer.Columns[5].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsByCustomer.Columns[6].SetWidth(35, WorksheetColumnWidthUnit.Character);
            wsByCustomer.Columns[7].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsByCustomer.Columns[8].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wsByCustomer.Columns[9].SetWidth(30, WorksheetColumnWidthUnit.Character);

            // 'By Reason' worksheet
            var wsByReason = this.CreateWorksheet("By Reason");
            AddDataByReason(wsByReason, orderData);
            wsByReason.Columns[0].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[1].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[2].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[3].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[4].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[5].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[6].SetWidth(35, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[7].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[8].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[9].SetWidth(30, WorksheetColumnWidthUnit.Character);

            // 'By Department' worksheet
            var wsByDepartment = CreateWorksheet("By Department");
            AddDataByDepartment(wsByDepartment, orderData);
            wsByDepartment.Columns[0].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsByDepartment.Columns[1].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wsByDepartment.Columns[2].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsByDepartment.Columns[3].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsByDepartment.Columns[4].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsByDepartment.Columns[5].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsByDepartment.Columns[6].SetWidth(35, WorksheetColumnWidthUnit.Character);
            wsByDepartment.Columns[7].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsByDepartment.Columns[8].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wsByDepartment.Columns[9].SetWidth(30, WorksheetColumnWidthUnit.Character);

            // 'By Product Class' worksheet
            if (FieldUtilities.IsFieldEnabled("Order", "Product Class"))
            {
                var wsProductClass = CreateWorksheet("By Product Class");
                AddDataByProductClass(wsProductClass, orderData);
                wsProductClass.Columns[0].SetWidth(10, WorksheetColumnWidthUnit.Character);
                wsProductClass.Columns[1].SetWidth(20, WorksheetColumnWidthUnit.Character);
                wsProductClass.Columns[2].SetWidth(25, WorksheetColumnWidthUnit.Character);
                wsProductClass.Columns[3].SetWidth(15, WorksheetColumnWidthUnit.Character);
                wsProductClass.Columns[4].SetWidth(25, WorksheetColumnWidthUnit.Character);
                wsProductClass.Columns[5].SetWidth(15, WorksheetColumnWidthUnit.Character);
                wsProductClass.Columns[6].SetWidth(35, WorksheetColumnWidthUnit.Character);
                wsProductClass.Columns[7].SetWidth(10, WorksheetColumnWidthUnit.Character);
                wsProductClass.Columns[8].SetWidth(20, WorksheetColumnWidthUnit.Character);
                wsProductClass.Columns[9].SetWidth(30, WorksheetColumnWidthUnit.Character);
            }
        }

        /// <summary>
        /// Adds the data by reason.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="orders">The orders.</param>
        private void AddDataByReason(Worksheet worksheet, IEnumerable<OrderData> orders)
        {
            try
            {
                var rowIndex = 0;

                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, "Date(s): " + _fromDate.ToShortDateString() + " to " + _toDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the customer name section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, "Orders On Hold By Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                foreach (var group in orders.GroupBy(o => o.HoldReason))
                {
                    var reasonStartRowIndex = rowIndex;

                    // Add the customer name section
                    this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, group.Key).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    rowIndex++;

                    // Add the headers
                    this.CreateHeaderCell(worksheet, rowIndex, 0, "WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 1, "Child WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 2, "Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 3, "PO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 4, "Hold Location").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 5, "Est. Ship Date").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 6, "Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 7, "Part Qty").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 8, "Notes").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 9, "Serial Number(s)").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    rowIndex++;

                    foreach (var order in group)
                    {
                        CreateCell(worksheet, rowIndex, 0, order.OrderId, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 1, order.RelatedOrderString, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 2, order.CustomerName, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 3, order.PurchaseOrder, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 4, order.HoldLocation, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 5, order.EstShipDate, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 6, order.HoldReason, false, HorizontalCellAlignment.Center);

                        if (order.PartQuantity.HasValue)
                        {
                            CreateCell(worksheet, rowIndex, 7, order.PartQuantity, false, HorizontalCellAlignment.Center);
                        }
                        else
                        {
                            CreateCell(worksheet, rowIndex, 7, "NA", false, HorizontalCellAlignment.Center);
                        }

                        CreateCell(worksheet, rowIndex, 8, order.Notes, false, HorizontalCellAlignment.Center);

                        if (order.SerialNumbers != null)
                        {
                            var serialNumbers = order.SerialNumbers.Select(s => s.IsNumberNull() ? string.Empty : s.Number);
                            CreateCell(worksheet, rowIndex, 9, string.Join(", ", serialNumbers), false, HorizontalCellAlignment.Left);
                        }

                        rowIndex++;
                    }

                    // Add the customer total
                    var cell = this.CreateCell(worksheet, rowIndex, 0, "Total:");
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 1, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell = this.CreateFormulaCell(worksheet, rowIndex, 7, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(reasonStartRowIndex + 3, 1, rowIndex, 1), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 9, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);

                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex = rowIndex + 2; // skip a line between customers
                }

            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to create Orders On Hold By Reason Report.");
            }
        }

        /// <summary>
        /// Adds the data by customer.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="orders">The orders.</param>
        private void AddDataByCustomer(Worksheet worksheet, IEnumerable<OrderData> orders)
        {
            try
            {
                var rowIndex = 0;

                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, "Date(s): " + _fromDate.ToShortDateString() + " to " + _toDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the customer name section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, "Orders On Hold By Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                var ordersByCustomer = orders.GroupBy(o => o.CustomerName).ToList();
                foreach (var customerGroup in ordersByCustomer)
                {
                    var customerStartRowIndex = rowIndex;

                    // Add the customer name section
                    this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, customerGroup.Key).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    rowIndex++;

                    // Add the column headers
                    this.CreateHeaderCell(worksheet, rowIndex, 0, "WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 1, "Related").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 2, "Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 3, "PO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 4, "Hold Location").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 5, "Est. Ship Date").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 6, "Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 7, "Part Qty").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 8, "Notes").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 9, "Serial Number(s)").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    rowIndex++;

                    // Add the orders
                    foreach (var order in customerGroup)
                    {
                        CreateCell(worksheet, rowIndex, 0, order.OrderId, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 1, order.RelatedOrderString, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 2, order.CustomerName, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 3, order.PurchaseOrder, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 4, order.HoldLocation, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 5, order.EstShipDate, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 6, order.HoldReason, false, HorizontalCellAlignment.Center);

                        if (order.PartQuantity.HasValue)
                        {
                            CreateCell(worksheet, rowIndex, 7, order.PartQuantity, false, HorizontalCellAlignment.Center);
                        }
                        else
                        {
                            CreateCell(worksheet, rowIndex, 7, "NA", false, HorizontalCellAlignment.Center);
                        }


                        CreateCell(worksheet, rowIndex, 8, order.Notes, false, HorizontalCellAlignment.Center);

                        if (order.SerialNumbers != null)
                        {
                            var serialNumbers = order.SerialNumbers.Select(s => s.IsNumberNull() ? string.Empty : s.Number);
                            CreateCell(worksheet, rowIndex, 9, string.Join(", ", serialNumbers), false, HorizontalCellAlignment.Left);
                        }

                        rowIndex++;
                    }

                    // Add the customer total
                    var cell = this.CreateCell(worksheet, rowIndex, 0, "Total:");
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 1, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell = this.CreateFormulaCell(worksheet, rowIndex, 7, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(customerStartRowIndex + 3, 1, rowIndex, 1), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 9, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);

                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex = rowIndex + 2; // skip a line between customers
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to create Orders On Hold By Customer Report.");
            }
        }

        /// <summary>
        /// Adds the data by department.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="orders">The orders.</param>
        private void AddDataByDepartment(Worksheet worksheet, IEnumerable<OrderData> orders)
        {
            try
            {
                var rowIndex = 0;

                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, "Date(s): " + _fromDate.ToShortDateString() + " to " + _toDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the customer name section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, "Orders On Hold By Department").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                foreach (var deptGroup in orders.GroupBy(o => o.CurrentLocation))
                {
                    var customerStartRowIndex = rowIndex;

                    // Add the customer name section
                    this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, deptGroup.Key).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    rowIndex++;

                    // Add the column headers
                    this.CreateHeaderCell(worksheet, rowIndex, 0, "WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 1, "Related").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 2, "Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 3, "PO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 4, "Hold Location").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 5, "Est. Ship Date").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 6, "Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 7, "Part Qty").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 8, "Notes").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 9, "Serial Number(s)").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    rowIndex++;

                    // Add the orders
                    foreach (var order in deptGroup)
                    {
                        CreateCell(worksheet, rowIndex, 0, order.OrderId, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 1, order.RelatedOrderString, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 2, order.CustomerName, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 3, order.PurchaseOrder, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 4, order.HoldLocation, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 5, order.EstShipDate, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 6, order.HoldReason, false, HorizontalCellAlignment.Center);

                        if (order.PartQuantity.HasValue)
                        {
                            CreateCell(worksheet, rowIndex, 7, order.PartQuantity, false, HorizontalCellAlignment.Center);
                        }
                        else
                        {
                            CreateCell(worksheet, rowIndex, 7, "NA", false, HorizontalCellAlignment.Center);
                        }


                        CreateCell(worksheet, rowIndex, 8, order.Notes, false, HorizontalCellAlignment.Center);

                        if (order.SerialNumbers != null)
                        {
                            var serialNumbers = order.SerialNumbers.Select(s => s.IsNumberNull() ? string.Empty : s.Number);
                            CreateCell(worksheet, rowIndex, 9, string.Join(", ", serialNumbers), false, HorizontalCellAlignment.Left);
                        }

                        rowIndex++;
                    }

                    // Add the department total
                    var cell = this.CreateCell(worksheet, rowIndex, 0, "Total:");
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 1, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell = this.CreateFormulaCell(worksheet, rowIndex, 7, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(customerStartRowIndex + 3, 1, rowIndex, 1), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 9, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);

                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex = rowIndex + 2; // skip a line between departments
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to create Orders On Hold By Customer Report.");
            }
        }

        /// <summary>
        /// Adds the data by product class.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="orders">The orders.</param>
        private void AddDataByProductClass(Worksheet worksheet, IEnumerable<OrderData> orders)
        {
            try
            {
                var rowIndex = 0;

                // Add the to/from dates
                CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, "Date(s): " + _fromDate.ToShortDateString() + " to " + _toDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the customer name section
                CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, "Orders On Hold By Product Class").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                foreach (var productClass in orders.GroupBy(o => o.ProductClass))
                {
                    var customerStartRowIndex = rowIndex;

                    // Add the customer name section
                    CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, productClass.Key).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    rowIndex++;

                    // Add the column headers
                    CreateHeaderCell(worksheet, rowIndex, 0, "WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    CreateHeaderCell(worksheet, rowIndex, 1, "Related").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    CreateHeaderCell(worksheet, rowIndex, 2, "Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    CreateHeaderCell(worksheet, rowIndex, 3, "PO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    CreateHeaderCell(worksheet, rowIndex, 4, "Hold Location").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    CreateHeaderCell(worksheet, rowIndex, 5, "Est. Ship Date").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    CreateHeaderCell(worksheet, rowIndex, 6, "Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    CreateHeaderCell(worksheet, rowIndex, 7, "Part Qty").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    CreateHeaderCell(worksheet, rowIndex, 8, "Notes").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    CreateHeaderCell(worksheet, rowIndex, 9, "Serial Number(s)").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    rowIndex++;

                    // Add the orders
                    foreach (var order in productClass)
                    {
                        CreateCell(worksheet, rowIndex, 0, order.OrderId, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 1, order.RelatedOrderString, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 2, order.CustomerName, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 3, order.PurchaseOrder, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 4, order.HoldLocation, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 5, order.EstShipDate, false, HorizontalCellAlignment.Center);
                        CreateCell(worksheet, rowIndex, 6, order.HoldReason, false, HorizontalCellAlignment.Center);

                        if (order.PartQuantity.HasValue)
                        {
                            CreateCell(worksheet, rowIndex, 7, order.PartQuantity, false, HorizontalCellAlignment.Center);
                        }
                        else
                        {
                            CreateCell(worksheet, rowIndex, 7, "NA", false, HorizontalCellAlignment.Center);
                        }


                        CreateCell(worksheet, rowIndex, 8, order.Notes, false, HorizontalCellAlignment.Center);

                        if (order.SerialNumbers != null)
                        {
                            var serialNumbers = order.SerialNumbers.Select(s => s.IsNumberNull() ? string.Empty : s.Number);
                            CreateCell(worksheet, rowIndex, 9, string.Join(", ", serialNumbers));
                        }

                        rowIndex++;
                    }

                    // Add the department total
                    var cell = CreateCell(worksheet, rowIndex, 0, "Total:");
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    CreateCell(worksheet, rowIndex, 1, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell = CreateFormulaCell(worksheet, rowIndex, 7, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(customerStartRowIndex + 3, 1, rowIndex, 1), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    CreateCell(worksheet, rowIndex, 9, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);

                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex = rowIndex + 2; // skip a line between product classes
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to create Orders On Hold By Product Class.");
            }
        }

        /// <summary>
        /// Adds all data.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="orders">The orders.</param>
        private void AddAllData(Worksheet worksheet, IEnumerable<OrderData> orders)
        {
            try
            {
                var rowIndex = 0;

                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, "Date(s): " + _fromDate.ToShortDateString() + " to " + _toDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the customer name section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, "All Orders On Hold").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the column headers
                this.CreateHeaderCell(worksheet, rowIndex, 0, "WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 1, "Related").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 2, "Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 3, "PO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 4, "Hold Location").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 5, "Est. Ship Date").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 6, "Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 7, "Part Qty").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 8, "Notes").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 9, "Serial Number(s)").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                var startRowIndex = rowIndex;
                foreach (var order in orders)
                {
                    CreateCell(worksheet, rowIndex, 0, order.OrderId, false, HorizontalCellAlignment.Center);
                    CreateCell(worksheet, rowIndex, 1, order.RelatedOrderString, false, HorizontalCellAlignment.Center);
                    CreateCell(worksheet, rowIndex, 2, order.CustomerName, false, HorizontalCellAlignment.Center);
                    CreateCell(worksheet, rowIndex, 3, order.PurchaseOrder, false, HorizontalCellAlignment.Center);
                    CreateCell(worksheet, rowIndex, 4, order.HoldLocation, false, HorizontalCellAlignment.Center);
                    CreateCell(worksheet, rowIndex, 5, order.EstShipDate, false, HorizontalCellAlignment.Center);
                    CreateCell(worksheet, rowIndex, 6, order.HoldReason, false, HorizontalCellAlignment.Center);

                    if (order.PartQuantity.HasValue)
                    {
                        CreateCell(worksheet, rowIndex, 7, order.PartQuantity, false, HorizontalCellAlignment.Center);
                    }
                    else
                    {
                        CreateCell(worksheet, rowIndex, 7, "NA", false, HorizontalCellAlignment.Center);
                    }


                    CreateCell(worksheet, rowIndex, 8, order.Notes, false, HorizontalCellAlignment.Center);

                    if (order.SerialNumbers != null)
                    {
                        var serialNumbers = order.SerialNumbers.Select(s => s.IsNumberNull() ? string.Empty : s.Number);
                        CreateCell(worksheet, rowIndex, 9, string.Join(", ", serialNumbers), false, HorizontalCellAlignment.Left);
                    }

                    rowIndex++;
                }

                var totalIndex = rowIndex;

                // Add the total count
                //Only add totals if there are orders
                if (startRowIndex != totalIndex)
                {
                    var cell = this.CreateCell(worksheet, rowIndex, 0, "Total:");
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 1, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);

                    cell = this.CreateFormulaCell(worksheet, rowIndex, 7, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 9, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to create All Orders On Hold Report.");
            }
        }

        #region Summary

        /// <summary>
        /// Adds the summary data.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="orders">The orders.</param>
        /// <param name="rowIndex">Index of the row.</param>
        private void AddSummaryData(Worksheet worksheet, OrdersReport.OrderDataTable orders, int rowIndex)
        {
            try
            {
                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 1, "Date(s): " + _fromDate.ToShortDateString() + " to " + _toDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the 'All' section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 1, "All Holds").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the total count
                var cell = this.CreateCell(worksheet, rowIndex, 0, "Total:");
                cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                cell = this.CreateCell(worksheet, rowIndex, 1, orders.Rows.Count, false, HorizontalCellAlignment.Center);
                cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                rowIndex++;

                rowIndex = CustomerSummary(worksheet, orders, rowIndex);
                rowIndex++;

                rowIndex = ReasonSummary(worksheet, orders, rowIndex);
                rowIndex++;

                rowIndex = DepartmentSummary(worksheet, orders, rowIndex);

                if (FieldUtilities.IsFieldEnabled("Order", "Product Class"))
                {
                    rowIndex++;
                    ProductClassSummary(worksheet, orders, rowIndex);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to create Orders On Hold Summary Report.");
            }
        }

        private int ReasonSummary(Worksheet worksheet, OrdersReport.OrderDataTable orders, int initialRowIndex)
        {
            var rowIndex = initialRowIndex;
            WorksheetCell cell;
            this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 1, "Holds By Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
            rowIndex++;
            this.CreateHeaderCell(worksheet, rowIndex, 0, "Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
            this.CreateHeaderCell(worksheet, rowIndex, 1, "Orders On Hold").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
            rowIndex++;
            var ordersByReason = this.GetByReason(orders);
            var startRowIndex = rowIndex;
            foreach (var kvp in ordersByReason)
            {
                this.CreateCell(worksheet, rowIndex, 0, kvp.Key);
                this.CreateCell(worksheet, rowIndex, 1, kvp.Value.Count, false, HorizontalCellAlignment.Center);
                rowIndex++;
            }

            if (ordersByReason.Count == 0)
            {
                this.CreateCell(worksheet, rowIndex, 0, string.Empty);
                this.CreateCell(worksheet, rowIndex, 1, string.Empty, false, HorizontalCellAlignment.Center);
                rowIndex++;
            }

            cell = this.CreateCell(worksheet, rowIndex, 0, "Total:");
            cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            cell = this.CreateFormulaCell(worksheet, rowIndex, 1, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
            cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            return rowIndex;
        }

        private int CustomerSummary(Worksheet worksheet, OrdersReport.OrderDataTable orders, int initialRowIndex)
        {
            var rowIndex = initialRowIndex;
            WorksheetCell cell;
            this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 1, "Holds By Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
            rowIndex++;
            this.CreateHeaderCell(worksheet, rowIndex, 0, "Company").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
            this.CreateHeaderCell(worksheet, rowIndex, 1, "Orders On Hold").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
            rowIndex++;
            var ordersByCustomer = orders.GroupBy(o => o.CustomerName).ToList();
            var startRowIndex = rowIndex; // used for totals formulas, need to add 1 for row indexing in formula
            foreach (var customerOrders in ordersByCustomer)
            {
                this.CreateCell(worksheet, rowIndex, 0, customerOrders.Key);
                this.CreateCell(worksheet, rowIndex, 1, customerOrders.Count(), false, HorizontalCellAlignment.Center);
                rowIndex++;
            }

            if (ordersByCustomer.Count == 0)
            {
                this.CreateCell(worksheet, rowIndex, 0, string.Empty);
                this.CreateCell(worksheet, rowIndex, 1, string.Empty, false, HorizontalCellAlignment.Center);
                rowIndex++;
            }

            cell = this.CreateCell(worksheet, rowIndex, 0, "Total:");
            cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            cell = this.CreateFormulaCell(worksheet, rowIndex, 1, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
            cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            return rowIndex;
        }

        private int DepartmentSummary(Worksheet worksheet, OrdersReport.OrderDataTable orders, int initialRowIndex)
        {
            var rowIndex = initialRowIndex;
            WorksheetCell cell;
            this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 1, "Holds By Department").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
            rowIndex++;
            this.CreateHeaderCell(worksheet, rowIndex, 0, "Department").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
            this.CreateHeaderCell(worksheet, rowIndex, 1, "Orders On Hold").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
            rowIndex++;
            var orderByDepartments = orders.GroupBy(o => o.CurrentLocation).ToList();
            var startRowIndex = rowIndex; // used for totals formulas, need to add 1 for row indexing in formula
            foreach (var deptGroup in orderByDepartments)
            {
                this.CreateCell(worksheet, rowIndex, 0, deptGroup.Key);
                this.CreateCell(worksheet, rowIndex, 1, deptGroup.Count(), false, HorizontalCellAlignment.Center);
                rowIndex++;
            }

            if (!orderByDepartments.Any())
            {
                this.CreateCell(worksheet, rowIndex, 0, string.Empty);
                this.CreateCell(worksheet, rowIndex, 1, string.Empty, false, HorizontalCellAlignment.Center);
                rowIndex++;
            }

            cell = this.CreateCell(worksheet, rowIndex, 0, "Total:");
            cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            cell = this.CreateFormulaCell(worksheet, rowIndex, 1, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
            cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            return rowIndex;
        }

        private int ProductClassSummary(Worksheet worksheet, OrdersReport.OrderDataTable orders, int initialRowIndex)
        {
            var rowIndex = initialRowIndex;
            CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 1, "Holds By Product Class").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
            rowIndex++;
            CreateHeaderCell(worksheet, rowIndex, 0, "Product Class").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
            CreateHeaderCell(worksheet, rowIndex, 1, "Orders On Hold").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
            rowIndex++;
            var ordersByProductClass = orders
                .GroupBy(o =>
                {
                    var productClassRow = o.GetOrderProductClassRows().FirstOrDefault();

                    return productClassRow == null || productClassRow.IsProductClassNull()
                        ? "N/A"
                        : productClassRow.ProductClass;
                })
                .ToList();

            var startRowIndex = rowIndex; // used for totals formulas, need to add 1 for row indexing in formula
            foreach (var productClass in ordersByProductClass)
            {
                CreateCell(worksheet, rowIndex, 0, productClass.Key);
                CreateCell(worksheet, rowIndex, 1, productClass.Count(), false, HorizontalCellAlignment.Center);
                rowIndex++;
            }

            if (!ordersByProductClass.Any())
            {
                CreateCell(worksheet, rowIndex, 0, string.Empty);
                CreateCell(worksheet, rowIndex, 1, string.Empty, false, HorizontalCellAlignment.Center);
                rowIndex++;
            }

            var cell = CreateCell(worksheet, rowIndex, 0, "Total:");
            cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            cell = CreateFormulaCell(worksheet, rowIndex, 1, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
            cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            return rowIndex;
        }

        #endregion

        /// <summary>
        /// Gets the orders by reason.
        /// </summary>
        /// <param name="orders">The orders.</param>
        /// <returns>Dictionary; KEY = reason for hold, VALUE = list of orders</returns>
        private Dictionary<string, List<OrdersReport.OrderRow>> GetByReason(OrdersReport.OrderDataTable orders)
        {
            var byReasonDictionary = new Dictionary<string, List<OrdersReport.OrderRow>>();

            try
            {
                foreach (var order in orders)
                {
                    string holdReason;
                    using (var taOrders = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
                        holdReason = taOrders.GetHoldReason(order.OrderID);

                    if (string.IsNullOrEmpty(holdReason))
                        holdReason = "Other";

                    if (byReasonDictionary.ContainsKey(holdReason))
                    {
                        var list = byReasonDictionary[holdReason];
                        list.Add(order);
                        byReasonDictionary[holdReason] = list;
                    }
                    else
                    {
                        byReasonDictionary.Add(holdReason, new List<OrdersReport.OrderRow> { order });
                    }
                }

                return byReasonDictionary;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to get orders by reason.");
                return byReasonDictionary;
            }

        }

        private static List<OrderData> CreateOrderData(IEnumerable<OrdersReport.OrderRow> orders)
        {
            if (orders == null)
            {
                return new List<OrderData>();
            }

            var ordersData = new List<OrderData>();

            foreach (var order in orders)
            {
                var dtOrig = new OrdersDataSet.InternalReworkDataTable();
                using (var internalReworkTableAdapter = new InternalReworkTableAdapter() { ClearBeforeFill = true })
                {
                    internalReworkTableAdapter.FillByOriginalOrderID(dtOrig, order.OrderID);

                    if (dtOrig.Rows.Count <= 0)
                        internalReworkTableAdapter.FillByReworkOrderID(dtOrig, order.OrderID);
                }

                string holdReason;
                using (var taOrders = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
                    holdReason = taOrders.GetHoldReason(order.OrderID);

                if (string.IsNullOrEmpty(holdReason))
                    holdReason = "Other";

                string relatedOrderString;
                string holdLocation;
                if (dtOrig.Rows.Count > 0)
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i < dtOrig.Rows.Count; i++)
                    {
                        var item = dtOrig[i];
                        if (i == 0)
                        {
                            sb.Append(item.IsReworkOrderIDNull() ? "NA" : item.ReworkOrderID.ToString());
                        }
                        else
                        {
                            sb.Append(", " + (item.IsReworkOrderIDNull() ? "NA" : item.ReworkOrderID.ToString()));
                        }
                    }

                    relatedOrderString = sb.ToString();
                    holdLocation = dtOrig[0].IsHoldLocationIDNull() ? "NA" : dtOrig[0].HoldLocationID;
                }
                else
                {
                    relatedOrderString = "NA";
                    holdLocation = "NA";
                }

                // Get the notes from the order hold table, we don't want internal rework notes in there
                var dtOrderHold = new OrdersDataSet.OrderHoldDataTable();
                using (var ta = new OrderHoldTableAdapter())
                    ta.Fill(dtOrderHold, order.OrderID);

                var notes = string.Empty;
                if (dtOrderHold.Rows.Count > 0)
                {
                    var lastHold = dtOrderHold.LastOrDefault();
                    if (lastHold != null)
                    {
                        notes = lastHold.Notes.Replace(Environment.NewLine, " ");
                    }
                }

                var productClassRow = order.GetOrderProductClassRows().FirstOrDefault();

                var productClass = productClassRow == null || productClassRow.IsProductClassNull()
                    ? "N/A"
                    : productClassRow.ProductClass;

                ordersData.Add(new OrderData
                {
                    OrderId = order.OrderID,
                    CustomerName = order.CustomerName,
                    PurchaseOrder = order.IsPurchaseOrderNull() ? "NA" : order.PurchaseOrder,
                    EstShipDate = order.IsEstShipDateNull() ? (DateTime?)null : order.EstShipDate.Date,
                    HoldReason = holdReason,

                    PartQuantity = order.IsPartQuantityNull() ? (int?)null : order.PartQuantity,
                    RelatedOrderString = relatedOrderString,
                    HoldLocation = holdLocation,
                    Notes = notes,

                    CurrentLocation = order.CurrentLocation,
                    SerialNumbers = order.GetOrderSerialNumberRows(),
                    ProductClass = productClass
                });
            }

            return ordersData;
        }

        private class OrderData
        {
            public string CustomerName { get; set; }
            public string HoldReason { get; set; }
            public int OrderId { get; set; }
            public int? PartQuantity { get; set; }
            public string PurchaseOrder { get; set; }
            public DateTime? EstShipDate { get; set; }
            public string RelatedOrderString { get; set; }
            public string HoldLocation { get; set; }
            public string Notes { get; set; }
            public string CurrentLocation { get; set; }

            public string ProductClass { get; set; }

            public OrdersReport.OrderSerialNumberRow[] SerialNumbers { get; set; }
        }

        #endregion
    }

    public class ProductionByDepartmentReport : ExcelBaseReport
    {
        #region Fields

        public const string TITLE = "Order Production";

        #endregion

        #region Properties

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public override string Title
        {
            get { return TITLE; }
        }

        /// <summary>
        /// Gets the report page orientation.
        /// </summary>
        /// <value>
        /// The report page orientation.
        /// </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }

        [Browsable(true)]
        [Description("The from date of the report.")]
        [DisplayName("From Date")]
        [Category("Report")]
        public DateTime FromDate
        {
            get; set;
        }

        [Browsable(true)]
        [Description("The to date of the report.")]
        [DisplayName("To Date")]
        [Category("Report")]
        public DateTime ToDate
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        /// <summary>
        /// Creates the report.
        /// </summary>
        private void CreateReport()
        {
            var departments = LoadSummaries(this.FromDate, this.ToDate);
            CreateSummary(departments);
            CreateProductionByDeptReport(departments);
        }

        private void CreateSummary(List<DepartmentDateSummary> departments)
        {

            const int columnCount = 6;

            try
            {
                var worksheet = CreateWorksheet("Summary");

                var rowIndex = base.AddCompanyHeaderRows(worksheet, columnCount - 1, "Summary") + 2;
                int column = 0;

                CreateHeaderCell(worksheet, rowIndex, column++, "Department", 20);
                CreateHeaderCell(worksheet, rowIndex, column++, "Orders", 20);
                CreateHeaderCell(worksheet, rowIndex, column++, "Parts", 20);
                CreateHeaderCell(worksheet, rowIndex, column++, "Surface Area", 20);
                CreateHeaderCell(worksheet, rowIndex, column++, "Lead Time (Hrs.)", 25);
                rowIndex++;

                var startRowIndex = rowIndex;
                var deptTotalSummaries = GetTotalSummaries(departments);

                foreach (var departmentSummary in deptTotalSummaries)
                {
                    CreateCell(worksheet, rowIndex, 0, departmentSummary.Department);
                    CreateCell(worksheet, rowIndex, 1, departmentSummary.Orders, cellFormat: "#,###");
                    CreateCell(worksheet, rowIndex, 2, departmentSummary.PartQuantity, cellFormat: "#,###");
                    CreateCell(worksheet, rowIndex, 3, departmentSummary.TotalSA, cellFormat: "#,###.00");
                    CreateCell(worksheet, rowIndex, 4, departmentSummary.TotalLeadTime, cellFormat: "#,###;;0");
                    rowIndex++;
                }

                if (deptTotalSummaries.Count == 0)
                {
                    for (int colIndex = 0; colIndex < 5; colIndex++)
                    {

                        CreateCell(worksheet, rowIndex, colIndex, string.Empty);
                    }
                    rowIndex++;
                }

                //Add Total
                const string sumFormat = "=SUM(R{0}C{1}:R{2}C{3})";
                const string avgFormat = "=AVERAGE(R{0}C{1}:R{2}C{3})";
                this.CreateCell(worksheet, rowIndex, 0, "Total:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 1, sumFormat.FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, true, cellFormat: "#,###").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 2, sumFormat.FormatWith(startRowIndex + 1, 3, rowIndex, 3), CellReferenceMode.R1C1, true, cellFormat: "#,###").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 3, sumFormat.FormatWith(startRowIndex + 1, 4, rowIndex, 4), CellReferenceMode.R1C1, true, cellFormat: "#,###.00").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 4, sumFormat.FormatWith(startRowIndex + 1, 5, rowIndex, 5), CellReferenceMode.R1C1, true, cellFormat: "#,###;;0").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                rowIndex++;

                //Add Average
                this.CreateCell(worksheet, rowIndex, 0, "Average:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 1, avgFormat.FormatWith(startRowIndex + 1, 2, rowIndex - 1, 2), CellReferenceMode.R1C1, true, cellFormat: "#,###").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 2, avgFormat.FormatWith(startRowIndex + 1, 3, rowIndex - 1, 3), CellReferenceMode.R1C1, true, cellFormat: "#,###").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 3, avgFormat.FormatWith(startRowIndex + 1, 4, rowIndex - 1, 4), CellReferenceMode.R1C1, true, cellFormat: "#,###.00").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 4, avgFormat.FormatWith(startRowIndex + 1, 5, rowIndex - 1, 5), CellReferenceMode.R1C1, true, cellFormat: "#,###;;0").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to create Summary tab .");
            }
        }

        /// <summary>
        /// Creates the Production Details report.
        /// </summary>
        private void CreateProductionByDeptReport(List<DepartmentDateSummary> summaries)
        {
            try
            {
                var ta = new Data.Reports.ProcessPartsReportTableAdapters.OrderProductionTableAdapter();
                var processingByDept = ta.GetByDepartment(this.FromDate, this.ToDate);

                if (processingByDept.Rows.Count > 0)
                {
                    CreateDepartmentSheets(processingByDept, summaries);
                }
                else
                {
                    // Start a new worksheet for the new department
                    Worksheet ws = this.CreateWorksheet("Empty");
                    int rowIndex = this.AddCompanyHeaderRows(ws, 4) + 2;
                    this.CreateCell(ws,
                        rowIndex,
                        0,
                        "*There were no completed processes found for the date(s): " + this.FromDate.ToShortDateString() + " to " + this.ToDate.ToShortDateString(),
                        false,
                        HorizontalCellAlignment.Left,
                        CellBorderLineStyle.None);
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to create Production Details Report.");
            }
        }

        private void CreateDepartmentSheets(Data.Reports.ProcessPartsReport.OrderProductionDataTable processingByDept, List<DepartmentDateSummary> summaries)
        {
            const int COLUMN_COUNT = 6;
            if (processingByDept == null)
            {
                return;
            }

            foreach (var itemGroup in processingByDept.GroupBy(item => item.Department))
            {
                string department = itemGroup.Key;
                var deptSummaries = summaries.Where(d => d.Department == department).ToList();
                deptSummaries.Sort((d1, d2) => d1.DateTimeStamp.CompareTo((d2.DateTimeStamp)));

                Worksheet ws = this.CreateWorksheet(department);
                int rowIndex = this.AddCompanyHeaderRows(ws, COLUMN_COUNT - 1, " - " + department) + 2;

                // Set the column widths
                this.SetColWidths(ws);
                rowIndex = AddSummary(deptSummaries, ws, rowIndex);
                rowIndex++;
                AddDetails(itemGroup, ws, rowIndex);
            }
        }

        private int AddSummary(IEnumerable<DepartmentDateSummary> summaryItems, Worksheet ws, int rowIndex)
        {
            if (summaryItems == null || !summaryItems.Any())
            {
                return rowIndex;
            }

            CreateHeaderCell(ws, rowIndex, 0, "Date", 20);
            CreateHeaderCell(ws, rowIndex, 1, "Orders", 20);
            CreateHeaderCell(ws, rowIndex, 2, "Parts", 20);
            CreateHeaderCell(ws, rowIndex, 3, "Surface Area", 20);
            CreateHeaderCell(ws, rowIndex, 4, "Lead Time (Hrs.)", 20);

            rowIndex++;
            int startRowIndex = rowIndex;

            var orderCounts = new List<double>();
            var partCounts = new List<double>();
            var saCounts = new List<double>();
            var leadTimeCounts = new List<double>();

            foreach (var departmentSummary in summaryItems)
            {
                CreateCell(ws, rowIndex, 0, new DateTime(departmentSummary.DateTimeStamp));
                CreateCell(ws, rowIndex, 1, departmentSummary.Orders, cellFormat: "#,###");
                CreateCell(ws, rowIndex, 2, departmentSummary.PartQuantity, cellFormat: "#,###");
                CreateCell(ws, rowIndex, 3, departmentSummary.TotalSA, cellFormat: "#,###.00");
                CreateCell(ws, rowIndex, 4, departmentSummary.LeadTime, cellFormat: "#,###;;0");

                orderCounts.Add(departmentSummary.Orders);
                partCounts.Add(departmentSummary.PartQuantity);
                saCounts.Add(departmentSummary.TotalSA);
                leadTimeCounts.Add(departmentSummary.LeadTime);

                rowIndex++;
            }

            //Add Summary
            const string sumFormat = "=SUM(R{0}C{1}:R{2}C{3})";
            const string avgFormat = "=AVERAGE(R{0}C{1}:R{2}C{3})";

            this.CreateCell(ws, rowIndex, 0, "Total:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(ws, rowIndex, 1, sumFormat.FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, cellFormat: "#,###").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(ws, rowIndex, 2, sumFormat.FormatWith(startRowIndex + 1, 3, rowIndex, 3), CellReferenceMode.R1C1, cellFormat: "#,###").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(ws, rowIndex, 3, sumFormat.FormatWith(startRowIndex + 1, 4, rowIndex, 4), CellReferenceMode.R1C1, cellFormat: "#,###.00").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(ws, rowIndex, 4, sumFormat.FormatWith(startRowIndex + 1, 5, rowIndex, 5), CellReferenceMode.R1C1, cellFormat: "#,###;;0").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;

            // Add Averages
            this.CreateCell(ws, rowIndex, 0, "Average:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(ws, rowIndex, 1, avgFormat.FormatWith(startRowIndex + 1, 2, rowIndex - 1, 2), CellReferenceMode.R1C1, cellFormat: "#,###").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(ws, rowIndex, 2, avgFormat.FormatWith(startRowIndex + 1, 3, rowIndex - 1, 3), CellReferenceMode.R1C1, cellFormat: "#,###").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(ws, rowIndex, 3, avgFormat.FormatWith(startRowIndex + 1, 4, rowIndex - 1, 4), CellReferenceMode.R1C1, cellFormat: "#,###.00").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(ws, rowIndex, 4, avgFormat.FormatWith(startRowIndex + 1, 5, rowIndex - 1, 5), CellReferenceMode.R1C1, cellFormat: "#,###;;0").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;

            var orderStats = StatInfo.Create(orderCounts, "Orders");
            var partStats = StatInfo.Create(partCounts, "Count");
            var saStats = StatInfo.Create(saCounts, "in2");
            var leadTimeStats = StatInfo.Create(leadTimeCounts, "Hrs.");

            // Add Min
            this.CreateCell(ws, rowIndex, 0, "Min:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(ws, rowIndex, 1, orderStats.Min, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(ws, rowIndex, 2, partStats.Min, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(ws, rowIndex, 3, saStats.Min, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(ws, rowIndex, 4, leadTimeStats.Min, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;

            // Add Max
            this.CreateCell(ws, rowIndex, 0, "Max:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(ws, rowIndex, 1, orderStats.Max, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(ws, rowIndex, 2, partStats.Max, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(ws, rowIndex, 3, saStats.Max, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(ws, rowIndex, 4, leadTimeStats.Max, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;

            // Add StdDeviation
            this.CreateCell(ws, rowIndex, 0, "Std Deviation:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(ws, rowIndex, 1, orderStats.StdDeviation, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(ws, rowIndex, 2, partStats.StdDeviation, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(ws, rowIndex, 3, saStats.StdDeviation, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(ws, rowIndex, 4, leadTimeStats.StdDeviation, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;

            // Add Variance
            this.CreateCell(ws, rowIndex, 0, "Variance:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(ws, rowIndex, 1, orderStats.Variance, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(ws, rowIndex, 2, partStats.Variance, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(ws, rowIndex, 3, saStats.Variance, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(ws, rowIndex, 4, leadTimeStats.Variance, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;

            return rowIndex;
        }

        private void AddDetails(IGrouping<string, Data.Reports.ProcessPartsReport.OrderProductionRow> itemGroup, Worksheet ws, int rowIndex)
        {
            this.AddColHeaders(ws, rowIndex);
            int dataStartRowIndex = rowIndex;
            rowIndex++;

            foreach (var item in itemGroup)
            {
                this.CreateCell(ws, rowIndex, 0, item.EndDate.Date);
                this.CreateCell(ws, rowIndex, 1, item["ProcessName"]);
                this.CreateCell(ws, rowIndex, 2, item.OrderID);
                this.CreateCell(ws, rowIndex, 3, item.IsPartQuantityNull() ? 0 : item.PartQuantity);
                this.CreateCell(ws, rowIndex, 4, item.IsSurfaceAreaNull() ? 0 : item.SurfaceArea);

                object businessHoursContent;

                if (item.IsStartDateNull() || item.IsEndDateNull())
                {
                    businessHoursContent = "NA";
                }
                else
                {
                    businessHoursContent = DateUtilities.GetBusinessHours(item.StartDate, item.EndDate);
                }
                this.CreateCell(ws, rowIndex, 5, businessHoursContent);
                rowIndex++;
            }

            this.AddTotalsRow(ws, rowIndex, dataStartRowIndex);
            this.AddAveragesRow(ws, rowIndex + 1, rowIndex, dataStartRowIndex);
        }

        /// <summary>
        /// Adds the totals row.
        /// </summary>
        /// <param name="ws">The ws.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="startRowIndex">Start index of the row.</param>
        private void AddTotalsRow(Worksheet ws, int rowIndex, int startRowIndex)
        {
            const string sumFormat = "=SUM(R{0}C{1}:R{2}C{3})";
            this.CreateMergedCell(ws, rowIndex, 0, rowIndex, 1, "Total:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateFormulaCell(ws, rowIndex, 2, "=COUNTA(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 2, 3, rowIndex, 3), CellReferenceMode.R1C1, true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateFormulaCell(ws, rowIndex, 3, sumFormat.FormatWith(startRowIndex + 2, 4, rowIndex, 4), CellReferenceMode.R1C1, true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateFormulaCell(ws, rowIndex, 4, sumFormat.FormatWith(startRowIndex + 2, 5, rowIndex, 5), CellReferenceMode.R1C1, true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateFormulaCell(ws, rowIndex, 5, sumFormat.FormatWith(startRowIndex + 2, 6, rowIndex, 6), CellReferenceMode.R1C1, true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
        }

        /// <summary>
        /// Adds the averages row.
        /// </summary>
        /// <param name="ws">The ws.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="endRowIndex">End index of the row.</param>
        /// <param name="startRowIndex">Start index of the row.</param>
        private void AddAveragesRow(Worksheet ws, int rowIndex, int endRowIndex, int startRowIndex)
        {
            const string avgFormat = "=AVERAGE(R{0}C{1}:R{2}C{3})";
            this.CreateMergedCell(ws, rowIndex, 0, rowIndex, 1, "Average:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateFormulaCell(ws, rowIndex, 2, avgFormat.FormatWith(startRowIndex + 2, 3, endRowIndex, 3), CellReferenceMode.R1C1, true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateFormulaCell(ws, rowIndex, 3, avgFormat.FormatWith(startRowIndex + 2, 4, endRowIndex, 4), CellReferenceMode.R1C1, true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateFormulaCell(ws, rowIndex, 4, avgFormat.FormatWith(startRowIndex + 2, 5, endRowIndex, 5), CellReferenceMode.R1C1, true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateFormulaCell(ws, rowIndex, 5, avgFormat.FormatWith(startRowIndex + 2, 6, endRowIndex, 6), CellReferenceMode.R1C1, true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
        }

        /// <summary>
        /// Sets the col widths.
        /// </summary>
        /// <param name="ws">The ws.</param>
        private void SetColWidths(Worksheet ws)
        {
            ws.Columns[0].SetWidth(20, WorksheetColumnWidthUnit.Character);
            ws.Columns[1].SetWidth(20, WorksheetColumnWidthUnit.Character);
            ws.Columns[2].SetWidth(15, WorksheetColumnWidthUnit.Character);
            ws.Columns[3].SetWidth(10, WorksheetColumnWidthUnit.Character);
            ws.Columns[4].SetWidth(25, WorksheetColumnWidthUnit.Character);
            ws.Columns[5].SetWidth(25, WorksheetColumnWidthUnit.Character);
        }

        /// <summary>
        /// Adds the col headers.
        /// </summary>
        /// <param name="ws">The ws.</param>
        /// <param name="rowIndex">Index of the row.</param>
        private void AddColHeaders(Worksheet ws, int rowIndex)
        {
            this.CreateHeaderCell(ws, rowIndex, 0, "Date");
            this.CreateHeaderCell(ws, rowIndex, 1, "Process");
            this.CreateHeaderCell(ws, rowIndex, 2, "Work Order");
            this.CreateHeaderCell(ws, rowIndex, 3, "Parts");
            this.CreateHeaderCell(ws, rowIndex, 4, "Surface Area (Sq. In.)");
            this.CreateHeaderCell(ws, rowIndex, 5, "Lead Time (Hrs.)");
        }

        private List<DepartmentDateSummary> LoadSummaries(DateTime fromDate, DateTime toDate)
        {
            const string SALES_DEPT = "Sales (New Orders)";
            const string RECEIVING_DEPT = "Receiving";

            var orders = new Data.Reports.ProcessPartsReport.OrderProductionDataTable();
            var taOrders = new OrderProductionTableAdapter();

            var shipments = new Data.Reports.ProcessPartsReport.ShippingProductionDataTable();
            var taShipping = new ShippingProductionTableAdapter();

            var receiving = new Data.Reports.ProcessPartsReport.ReceivingProductionDataTable();
            var taReceiving = new ReceivingProductionTableAdapter();

            var departments = new List<DepartmentDateSummary>();

            try
            {
                //Get all orders
                taOrders.Fill(orders, fromDate, toDate);
                taShipping.Fill(shipments, fromDate, toDate);
                taReceiving.Fill(receiving, fromDate, toDate);

                DepartmentDateSummary currentDepartment = null;

                foreach (Data.Reports.ProcessPartsReport.OrderProductionRow item in orders)
                {
                    if (item.Department == ApplicationSettings.Current.DepartmentSales) //skip sales dept as they do not process orders
                        continue;

                    //try and find if department for end date exists
                    long dateTimeStamp = GetDateTimeStamp(item.EndDate);
                    currentDepartment = departments.Find(find => find.DateTimeStamp == dateTimeStamp && find.Department == item.Department);

                    //if no dept then create one
                    if (currentDepartment == null)
                    {
                        currentDepartment = new DepartmentDateSummary(item.Department, dateTimeStamp);
                        departments.Add(currentDepartment);
                    }

                    //add order and parts summary
                    currentDepartment.Orders += 1;

                    if (!item.IsPartQuantityNull() && item.PartQuantity > 0)
                    {
                        currentDepartment.PartQuantity += item.PartQuantity;

                        if (!item.IsSurfaceAreaNull())
                        {
                            currentDepartment.TotalSA += (item.SurfaceArea * item.PartQuantity);
                            currentDepartment.PartsWithTotalSA += 1;

                            if (!item.IsStartDateNull() && !item.IsEndDateNull())
                            {
                                currentDepartment.LeadTime += DateUtilities.GetBusinessHours(item.StartDate, item.EndDate);
                            }
                        }
                    }
                }

                //Get all new orders created
                using (var taOC = new OrderCreationTableAdapter())
                {
                    using (Data.Reports.ProcessPartsReport.OrderCreationDataTable dtOC = taOC.GetData(fromDate, toDate))
                    {
                        foreach (Data.Reports.ProcessPartsReport.OrderCreationRow item in dtOC)
                        {
                            long dateTimeStamp = GetDateTimeStamp(item.OrderDate);
                            currentDepartment = departments.Find(find => find.DateTimeStamp == dateTimeStamp && find.Department == SALES_DEPT);

                            //if no dept then create one
                            if (currentDepartment == null)
                            {
                                currentDepartment = new DepartmentDateSummary(SALES_DEPT, dateTimeStamp);
                                departments.Add(currentDepartment);
                            }

                            //add order and parts summary
                            currentDepartment.Orders += 1;

                            if (!item.IsPartQuantityNull())
                            {
                                currentDepartment.PartQuantity += item.PartQuantity;

                                if (!item.IsSurfaceAreaNull() && item.PartQuantity > 0)
                                {
                                    currentDepartment.TotalSA += (item.SurfaceArea * item.PartQuantity);
                                    currentDepartment.PartsWithTotalSA += 1;
                                }
                            }
                        }
                    }
                }

                //Get all shipped orders
                foreach (Data.Reports.ProcessPartsReport.ShippingProductionRow item in shipments)
                {
                    long dateTimeStamp = GetDateTimeStamp(item.DateShipped);
                    currentDepartment = departments.Find(find => find.DateTimeStamp == dateTimeStamp && find.Department == ApplicationSettings.Current.DepartmentShipping);

                    //if no dept then create one
                    if (currentDepartment == null)
                    {
                        currentDepartment = new DepartmentDateSummary(ApplicationSettings.Current.DepartmentShipping, dateTimeStamp);
                        departments.Add(currentDepartment);
                    }

                    //add order and parts summary
                    currentDepartment.Orders += 1;

                    if (!item.IsPartQuantityNull() && item.PartQuantity > 0)
                    {
                        currentDepartment.PartQuantity += item.PartQuantity;

                        if (!item.IsSurfaceAreaNull())
                        {
                            currentDepartment.TotalSA += (item.SurfaceArea * item.PartQuantity);
                            currentDepartment.PartsWithTotalSA += 1;
                        }
                    }
                }

                //Get all received orders
                foreach (Data.Reports.ProcessPartsReport.ReceivingProductionRow item in receiving)
                {
                    long dateTimeStamp = GetDateTimeStamp(DateTime.Parse(item.CheckIn));
                    currentDepartment = departments.Find(find => find.DateTimeStamp == dateTimeStamp && find.Department == RECEIVING_DEPT);

                    //if no dept then create one
                    if (currentDepartment == null)
                    {
                        currentDepartment = new DepartmentDateSummary(RECEIVING_DEPT, dateTimeStamp);
                        departments.Add(currentDepartment);
                    }

                    //add order and parts summary
                    currentDepartment.Orders += 1;
                    currentDepartment.PartQuantity += item.PartQuantity;

                    if (!item.IsSurfaceAreaNull() && item.PartQuantity > 0)
                    {
                        currentDepartment.TotalSA += (item.SurfaceArea * item.PartQuantity);
                        currentDepartment.PartsWithTotalSA += 1;
                    }
                }

                return departments;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error ");
                return departments;
            }
            finally
            {
                if (orders != null)
                    orders.Dispose();
                if (taOrders != null)
                    taOrders.Dispose();
                if (shipments != null)
                    shipments.Dispose();
                if (taShipping != null)
                    taShipping.Dispose();
                if (receiving != null)
                    receiving.Dispose();
                if (taReceiving != null)
                    taReceiving.Dispose();
            }
        }

        private List<DepartmentTotalSummary> GetTotalSummaries(List<DepartmentDateSummary> deptDateSummaries)
        {
            var departmentSummaries = new List<DepartmentTotalSummary>();

            foreach (var departmentGroup in deptDateSummaries.GroupBy(d => d.Department))
            {
                var dept = new DepartmentTotalSummary(departmentGroup.Key);

                foreach (var dg in departmentGroup)
                {
                    dept.Orders += dg.Orders;
                    dept.TotalSA += dg.TotalSA;
                    dept.TotalLeadTime += dg.LeadTime;

                    try
                    {
                        dept.PartQuantity += dg.PartQuantity;
                    }
                    catch (OverflowException)
                    {
                        _log.Warn("Arithmetic overflow occurred while running report");
                        dept.PartQuantity = long.MaxValue;
                    }
                }

                departmentSummaries.Add(dept);
            }

            departmentSummaries.Sort((d1, d2) => d1.Department.CompareTo(d2.Department));
            return departmentSummaries;
        }

        private long GetDateTimeStamp(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday)
                date = date.AddDays(-1); //Move to Friday
            else if (date.DayOfWeek == DayOfWeek.Sunday)
                date = date.AddDays(1); //move to Monay

            return new DateTime(date.Year, date.Month, date.Day).Ticks;
        }

        #endregion Methods

        #region DepartmentDateSummary

        /// <summary>
        /// Represents a summary of production for a date, time & department.
        /// </summary>
        private sealed class DepartmentDateSummary
        {
            public long DateTimeStamp { get; private set; }
            public string Department { get; private set; }
            public int Orders { get; set; }
            public long PartQuantity { get; set; }
            public int PartsWithTotalSA { get; set; }
            public double TotalSA { get; set; }

            /// <summary>
            /// Gets or sets the lead time in hours.
            /// </summary>
            public int LeadTime { get; internal set; }

            public DepartmentDateSummary(string department, long dateTimeStamp)
            {
                Department = department;
                DateTimeStamp = dateTimeStamp;
            }
        }

        #endregion

        #region DepartmentSummary

        /// <summary>
        /// Represents a summary of production for a department.
        /// </summary>
        private sealed class DepartmentTotalSummary
        {
            public string Department { get; private set; }
            public int Orders { get; set; }
            public long PartQuantity { get; set; }
            public double TotalSA { get; set; }

            /// <summary>
            ///  Gets or sets the total lead time in hours.
            /// </summary>
            public int TotalLeadTime { get; set; }

            public DepartmentTotalSummary(string department)
            {
                Department = department;
            }
        }

        #endregion
    }

    public class WorkInProgressReport : ExcelBaseReport
    {
        #region Fields

        private const int COLUMN_COUNT = 14;
        private static readonly DateTime LAST_PROCESS_DATE_FLAG = DateTime.Now.AddYears(-10);
        private static readonly string WORK_STATUS_PARTMARKING = ApplicationSettings.Current.WorkStatusPartMarking;
        private static readonly string WORK_STATUS_FINALINSPECTION = ApplicationSettings.Current.WorkStatusFinalInspection;
        private static readonly string WORK_STATUS_SHIPPING = ApplicationSettings.Current.WorkStatusShipping;
        private static readonly string WORK_STATUS_INPROCESS = ApplicationSettings.Current.WorkStatusInProcess;

        private int _startRowIndex;
        private int _endRowIndex;

        #endregion Fields

        #region Properties

        public override string Title => "Work In Progress";

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation => PageOrientation.Landscape;

        public bool HasSalesReportPermission { get; set; }

        #endregion

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            _log.Debug("Work in Process report is being run");

            CreateWorkInProgressReport();
        }

        #region Work In Progress Report

        private void CreateWorkInProgressReport()
        {
            // Determine if user has sales report permission
            using(var dtUserRoles = new SecurityDataSet.User_SecurityRolesDataTable())
            {
                using(var taSecurityUserRoles = new Data.Datasets.SecurityDataSetTableAdapters.User_SecurityRolesTableAdapter())
                {
                    taSecurityUserRoles.FillAllByUser(dtUserRoles, SecurityManager.UserID);
                    HasSalesReportPermission = dtUserRoles.AsEnumerable().Any(d => d.SecurityRoleID == "SalesReports");
                }
            }

            // Retrieve data
            var orderInfos = RetrieveOrderData();

            if (orderInfos.Count > 0)
                HasData = true;

            // Create report
            FillWorkInProcessReport(orderInfos);
        }

        private List<OrderInfo> RetrieveOrderData()
        {
            var orderInfos = new List<OrderInfo>();

            OrderStatusDataSet dsOrderStatus = new OrderStatusDataSet {EnforceConstraints = false};

            using(var taOrderStatus = new Data.Datasets.OrderStatusDataSetTableAdapters.OrderStatusTableAdapter())
                taOrderStatus.Fill(dsOrderStatus.OrderStatus);

            using(var taOrderSerialNumbers = new Data.Datasets.OrderStatusDataSetTableAdapters.OrderSerialNumberTableAdapter())
                taOrderSerialNumbers.FillActive(dsOrderStatus.OrderSerialNumber);

            foreach(var item in dsOrderStatus.OrderStatus)
            {
                var info = new OrderInfo
                {
                    OrderID = item.WO,
                    PurchaseOrder = item.IsPONull() ? "NA" : item.PO,
                    CustomerName = item.IsCustomerNull() ? "NA" : item.Customer,
                    EstShipDate = item.IsEstShipDateNull() ? DateTime.MinValue : item.EstShipDate,
                    PartName = item.IsPartNull() ? "NA" : item.Part,
                    PartQuantity = item.IsPartQuantityNull() ? 0 : item.PartQuantity,
                    Priority = item.IsPriorityNull() ? "NA" : item.Priority,
                    CurrentDepartment = item.CurrentLocation,
                    Status = item.WorkStatus,
                    CurrentProcess = item.IsCurrentProcessNull() ? "NA" : item.CurrentProcess,
                    NextDepartment = item.IsNextDeptNull() ? "None" : item.NextDept,
                    Type = (OrderType) item.OrderType,
                    SerialNumbers = item.GetOrderSerialNumberRows()
                };

                if(HasSalesReportPermission)
                {
                    var dsOrders = new OrdersDataSet.OrderDataTable();

                    using(var taOrders = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter())
                        taOrders.FillByOrderID(dsOrders, info.OrderID);

                    var order = dsOrders.FirstOrDefault();

                    if(order != null)
                        info.OrderPrice = GetPrice(order);
                }

                if(!item.IsCurrentProcessDueNull())
                {
                    if(item.CurrentProcessDue < LAST_PROCESS_DATE_FLAG)
                        item.CurrentProcessDue = item.CurrentProcessDue.AddYears(100);

                    info.ProcessBy = item.CurrentProcessDue;
                }

                if(item.WorkStatus == WORK_STATUS_INPROCESS)
                {
                    //short circuit the odd work statuses below
                }
                else if(item.WorkStatus == WORK_STATUS_PARTMARKING)
                {
                    info.CurrentProcess = WORK_STATUS_PARTMARKING;
                    info.NextDepartment = WORK_STATUS_FINALINSPECTION;

                    //use hack to determine if this is the last processes date
                    if(!item.IsCurrentProcessDueNull())
                    {
                        //add processing time for part marking
                        info.ProcessBy = item.CurrentProcessDue.AddBusinessDays(Convert.ToInt32(Math.Ceiling(ApplicationSettings.Current.PartMarkingLeadTime)));
                    }
                }
                else if(item.WorkStatus == WORK_STATUS_FINALINSPECTION)
                {
                    info.CurrentProcess = WORK_STATUS_FINALINSPECTION;
                    info.NextDepartment = WORK_STATUS_SHIPPING;

                    //use hack to determine if this is the last processes date
                    if(!item.IsCurrentProcessDueNull())
                    {
                        //to do processing for part marking

                        //add processing time for COC
                        info.ProcessBy = item.CurrentProcessDue.AddBusinessDays(Convert.ToInt32(Math.Ceiling(ApplicationSettings.Current.COCLeadTime)));
                    }
                }

                orderInfos.Add(info);
            }

            return orderInfos;
        }

        private void FillWorkInProcessReport(List<OrderInfo> orderInfos)
        {
            var ws = CreateWorksheet(Title);
            var rowIndex = this.AddCompanyHeaderRows(ws, COLUMN_COUNT, " - All") + 1;
            CreateHeaderFooter(ws, Title);
            rowIndex++;

            if (orderInfos.Count > 0)
            {
                _startRowIndex = rowIndex;
                rowIndex = AddWorkInProcessHeader(ws, rowIndex);

                foreach (OrderInfo item in orderInfos)
                    AddWorkInProcessRow(ws, item, rowIndex++, 0);

                _endRowIndex = rowIndex;
                AddWorkInProcessSummaryRow(ws, rowIndex);
            }
            else
            {
                AddWorkInProcessHeader(ws, rowIndex++);
                AddEmptyWorkInProcessRow(ws, rowIndex++, 0);
                AddWorkInProcessSummaryRow(ws, rowIndex);
            }

            ws.Tables.Add("A{0}:{1}{2}".FormatWith(_startRowIndex + 1, this.ExcelColumnIndexToName(COLUMN_COUNT), _endRowIndex + 1), true);
        }

        private int AddWorkInProcessHeader(Worksheet worksheet, int rowIndex)
        {
            var startColumn = 0;
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "WO", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "PO", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Customer", 35);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Est Ship Date", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Part", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Quantity", 12);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Priority", 12);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Status", 30);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Current Department", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Process", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Next Department", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Order Type", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Days Remaining", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Process By", 20);

            if (HasSalesReportPermission)
            {
                var cell = CreateHeaderCell(worksheet, rowIndex, startColumn++, "Order Price", 20);
                cell.CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.White);
            }

            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Serial Number(s)", 30);

            rowIndex++;

            return rowIndex;
        }

        private void AddWorkInProcessRow(Worksheet worksheet, OrderInfo orderInfo, int rowIndex, int startColumn)
        {
            try
            {
                int columnCount;

                if (HasSalesReportPermission)
                    columnCount = COLUMN_COUNT + 1;
                else
                    columnCount = COLUMN_COUNT;

                //format all of the cells
                for (int i = 0; i <= columnCount; i++)
                {
                    var cell = worksheet.Rows[rowIndex].Cells[i];
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                    cell.CellFormat.Alignment = HorizontalCellAlignment.Center;
                    ApplyCellBorders(cell);
                }

                var columnIndex = startColumn;

                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.OrderID;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PurchaseOrder;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.CustomerName;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.EstShipDate;  //orderInfo.EstShipDate == DateTime.MinValue ? "NA";

                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PartName;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PartQuantity;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.Priority;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.Status;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.CurrentDepartment;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.CurrentProcess;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.NextDepartment;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.Type;

                if (orderInfo.EstShipDate != DateTime.MinValue)
                {
                    var daysTillLate = DateUtilities.GetBusinessDays(DateTime.Now.Date, orderInfo.EstShipDate.Date);
                    if (orderInfo.EstShipDate.Date < DateTime.Now.Date)
                        daysTillLate *= -1;

                    var lateCell = worksheet.Rows[rowIndex].Cells[columnIndex++];
                    lateCell.Value = daysTillLate;

                    if (daysTillLate < 0)
                    {
                        lateCell.CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Red);
                        lateCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    }
                }
                else
                    worksheet.Rows[rowIndex].Cells[columnIndex++].Value = "NA";

                if (orderInfo.ProcessBy != DateTime.MinValue)
                    worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.ProcessBy;
                else
                    columnIndex++;

                if (HasSalesReportPermission)
                {
                    worksheet.Rows[rowIndex].Cells[columnIndex].Value = orderInfo.OrderPrice;
                    worksheet.Rows[rowIndex].Cells[columnIndex++].CellFormat.FormatString = MONEY_FORMAT;
                }

                if (orderInfo.SerialNumbers != null)
                {
                    var serialNumbers = orderInfo.SerialNumbers.Select(s => s.IsNumberNull() ? string.Empty : s.Number);
                    worksheet.Rows[rowIndex].Cells[columnIndex].Value = string.Join(", ", serialNumbers);
                    worksheet.Rows[rowIndex].Cells[columnIndex].CellFormat.Alignment = HorizontalCellAlignment.Left;
                }

            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        private void AddEmptyWorkInProcessRow(Worksheet worksheet, int rowIndex, int startColumn)
        {
            try
            {
                var region1 = CreateMergedHeader(worksheet, rowIndex, startColumn, rowIndex, startColumn + COLUMN_COUNT, "None");
                region1.CellFormat.Alignment = HorizontalCellAlignment.Center;
                region1.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                ApplyCellBorders(region1.CellFormat);
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        private void AddWorkInProcessSummaryRow(Worksheet worksheet, int rowIndex)
        {
            base.FormatHeaderCells(worksheet, rowIndex, 0, 13);
            var cell = this.CreateCell(worksheet, rowIndex, 0, "Total:");
            cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            //orders
            cell = this.CreateFormulaCell(worksheet, rowIndex, 1, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(7, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
            cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            //parts
            cell = this.CreateFormulaCell(worksheet, rowIndex, 5, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(7, 6, rowIndex, 6), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
            cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            if (HasSalesReportPermission)
            {
                //price
                cell = this.CreateFormulaCell(worksheet, rowIndex, 14, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(7, 15, rowIndex, 15), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            }
        }

        /// <summary>
        /// Gets the price.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        private decimal GetPrice(OrdersDataSet.OrderRow order)
        {
            if (order == null)
                return 0;

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderFeesTableAdapter())
            {
                using (var taFeeType = new Data.Datasets.OrdersDataSetTableAdapters.OrderFeeTypeTableAdapter())
                {
                    var orderFees = new OrdersDataSet.OrderFeesDataTable();
                    var fees = 0M;

                    ta.FillByOrder(orderFees, order.OrderID);

                    foreach (var fee in orderFees)
                    {
                        if (fee.OrderFeeTypeID != null)
                        {
                            var chargeType = taFeeType.GetFeeType(fee.OrderFeeTypeID);
                            fees += OrderPrice.CalculateFees(chargeType,
                                fee.Charge,
                                order.IsBasePriceNull() ? 0 : order.BasePrice,
                                order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                                order.IsPriceUnitNull() ? string.Empty : order.PriceUnit,
                                order.IsWeightNull() ? 0M : order.Weight);
                        }
                    }

                    return OrderPrice.CalculatePrice(
                        order.IsBasePriceNull() ? 0M : order.BasePrice,
                        order.IsPriceUnitNull() ? string.Empty : order.PriceUnit,
                        fees,
                        order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                        order.IsWeightNull() ? 0M : order.Weight);
                }
            }
        }

        #endregion

        #endregion

        #region OrderInfo

        private class OrderInfo
        {
            public int OrderID { get; set; }
            public string PurchaseOrder { get; set; }
            public string CustomerName { get; set; }
            public DateTime EstShipDate { get; set; }
            public string PartName { get; set; }
            public int PartQuantity { get; set; }
            public string Priority { get; set; }
            public string Status { get; set; }
            public string CurrentDepartment { get; set; }
            public string CurrentProcess { get; set; }
            public string NextDepartment { get; set; }
            public OrderType Type { get; set; }
            public DateTime ProcessBy { get; set; }
            public decimal OrderPrice { get; set; }

            public OrderStatusDataSet.OrderSerialNumberRow[] SerialNumbers { get; set; }
        }

        #endregion
    }

    public class WorkInProgressHistoryReport : ExcelBaseReport
    {
        #region Fields

        private const int COLUMN_COUNT = 9;

        private int _startRowIndex;
        private int _endRowIndex;

        #endregion Fields

        #region Properties

        public override string Title
        {
            get { return "Work In Progress History"; }
        }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Landscape; }
        }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        #endregion

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            _log.Debug("Work in Process report is being run");

            CreateWorkInProgressReport();
        }

        private void CreateWorkInProgressReport()
        {
            var orders = new Data.Reports.OrdersReport.OrderDataTable();

            var orderInfos = new List<OrderInfo>();

            using (var taOrderStatus = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
                orders = taOrderStatus.GetOpenWithinDateRange(ToDate.EndOfDay(), FromDate.StartOfDay());

            Data.Reports.OrdersReportTableAdapters.OrderProductClassTableAdapter taOrderProductClass = null;
            OrdersReport.OrderProductClassDataTable dtProductClass = null;

            try
            {
                dtProductClass = new OrdersReport.OrderProductClassDataTable();
                taOrderProductClass = new Data.Reports.OrdersReportTableAdapters.OrderProductClassTableAdapter();

                foreach (var item in orders)
                {
                    taOrderProductClass.FillByOrder(dtProductClass, item.OrderID);

                    var productClassRow = dtProductClass.FirstOrDefault();

                    var productClass = productClassRow == null || productClassRow.IsProductClassNull()
                        ? "N/A"
                        : productClassRow.ProductClass;

                    var info = new OrderInfo
                    {
                        OrderID = item.OrderID,
                        CustomerName = item.CustomerName,
                        CreatedDate = item.OrderDate,
                        EstShipDate = item.IsEstShipDateNull() ? DateTime.MinValue : item.EstShipDate,
                        PartName = item.PartName,
                        PartQuantity = item.IsPartQuantityNull() ? 0 : item.PartQuantity,
                        Priority = item.IsPriorityNull() ? "NA" : item.Priority,
                        Status = item.Status,
                        Type = (OrderType)item.OrderType,
                        BasePrice = item.IsBasePriceNull() ? 0 : item.BasePrice,
                        OrderFees = item.IsNull("OrderFees") ? 0 : Convert.ToDecimal(item["OrderFees"]),
                        PriceUnit = item.IsPriceUnitNull() ? OrderPrice.enumPriceUnit.Lot.ToString() : item.PriceUnit,
                        Weight = item.IsWeightNull() ? 0M : item.Weight,
                        ProductClass = productClass
                    };

                    orderInfos.Add(info);
                }

                if (orderInfos.Count > 0)
                    HasData = true;

                FillWorkInProcessReport(orderInfos);
            }
            finally
            {
                taOrderProductClass?.Dispose();
                dtProductClass?.Dispose();
            }

        }

        private void FillWorkInProcessReport(List<OrderInfo> orderInfos)
        {
            var ws = CreateWorksheet(Title);
            var rowIndex = this.AddCompanyHeaderRows(ws, COLUMN_COUNT, "") + 1;
            CreateHeaderFooter(ws, Title);
            rowIndex++;

            if (orderInfos.Count > 0)
            {
                _startRowIndex = rowIndex;
                rowIndex = AddWorkInProcessHeader(ws, rowIndex);

                foreach (OrderInfo item in orderInfos)
                    AddWorkInProcessRow(ws, item, rowIndex++, 0);

                _endRowIndex = rowIndex;

                long totalPartQuantity;

                try
                {
                    totalPartQuantity = orderInfos.Sum(oi => (long)oi.PartQuantity);
                }
                catch (OverflowException)
                {
                    _log.Warn("Arithmetic overflow occurred while running report");
                    totalPartQuantity = long.MaxValue;
                }

                AddWorkInProcessSummaryRow(orderInfos.Count, totalPartQuantity, ws, rowIndex);
            }
            else
            {
                AddWorkInProcessHeader(ws, rowIndex++);
                AddEmptyWorkInProcessRow(ws, rowIndex++, 0);
                AddWorkInProcessSummaryRow(0, 0, ws, rowIndex);
            }

            ws.Tables.Add("A{0}:{1}{2}".FormatWith(_startRowIndex + 1, this.ExcelColumnIndexToName(COLUMN_COUNT), _endRowIndex + 1), true);
        }

        private int AddWorkInProcessHeader(Worksheet worksheet, int rowIndex)
        {
            var startColumn = 0;
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "WO", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Customer", 35);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Est Ship Date", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Part", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Quantity", 12);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Price", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Priority", 12);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Status", 30);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Order Type", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Product Class", 25);

            rowIndex++;

            return rowIndex;
        }

        private void AddWorkInProcessRow(Worksheet worksheet, OrderInfo orderInfo, int rowIndex, int startColumn)
        {
            try
            {
                //format all of the cells
                for (int i = 0; i <= COLUMN_COUNT; i++)
                {
                    var cell = worksheet.Rows[rowIndex].Cells[i];
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                    cell.CellFormat.Alignment = HorizontalCellAlignment.Center;
                    ApplyCellBorders(cell);
                }

                var columnIndex = startColumn;

                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.OrderID;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.CustomerName;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.EstShipDate == DateTime.MinValue ? "NA" : orderInfo.EstShipDate.ToShortDateString();
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PartName;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PartQuantity;
                worksheet.Rows[rowIndex].Cells[columnIndex].CellFormat.FormatString = MONEY_FORMAT;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = OrderPrice.CalculatePrice(orderInfo.BasePrice, orderInfo.PriceUnit, orderInfo.OrderFees, orderInfo.PartQuantity, orderInfo.Weight);
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.Priority;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.Status;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.Type;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.ProductClass;
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        private void AddEmptyWorkInProcessRow(Worksheet worksheet, int rowIndex, int startColumn)
        {
            try
            {
                var region1 = CreateMergedHeader(worksheet, rowIndex, startColumn, rowIndex, startColumn + COLUMN_COUNT, "None");
                region1.CellFormat.Alignment = HorizontalCellAlignment.Center;
                region1.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                ApplyCellBorders(region1.CellFormat);
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        private void AddWorkInProcessSummaryRow(int orderCount, long totalPartCount, Worksheet worksheet, int rowIndex)
        {
            CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 2, "Total Orders: " + orderCount.ToString("N0")).CellFormat.Alignment = HorizontalCellAlignment.Center;
            CreateMergedHeader(worksheet, rowIndex, 3, rowIndex, 4, "Total Parts: " + totalPartCount.ToString("N0")).CellFormat.Alignment = HorizontalCellAlignment.Center;
        }

        #endregion

        #region OrderInfo

        private class OrderInfo
        {
            public int OrderID { get; set; }
            public string CustomerName { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime EstShipDate { get; set; }
            public string PartName { get; set; }
            public int PartQuantity { get; set; }
            public string Priority { get; set; }
            public string Status { get; set; }
            public decimal BasePrice { get; set; }
            public decimal OrderFees { get; set; }
            public decimal Weight { get; set; }
            public string PriceUnit { get; set; }
            public OrderType Type { get; set; }
            public string ProductClass { get; set; }
        }

        #endregion
    }

    public class OrderReceiptReport : ExcelBaseReport
    {
        #region Fields

        private DateTime _fromDate;
        private DateTime _toDate;
        private int _lastColumnIndex;
        private int _lastRowIndex;
        private bool _isTableRequired;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public override string Title
        {
            get { return "Order Receipt"; }
        }

        /// <summary>
        /// Gets the report page orientation.
        /// </summary>
        /// <value>
        /// The report page orientation.
        /// </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Landscape; }
        }

        [Browsable(true)]
        [Description("The from date of the report.")]
        [DisplayName("From Date")]
        [Category("Report")]
        public DateTime FromDate
        {
            get { return this._fromDate; }
            set { this._fromDate = value; }
        }

        [Browsable(true)]
        [Description("The to date of the report.")]
        [DisplayName("To Date")]
        [Category("Report")]
        public DateTime ToDate
        {
            get { return this._toDate; }
            set { this._toDate = value; }
        }

        /// <summary>
        /// Gets or sets the customer IDs for this instance.
        /// </summary>
        public List<int> CustomerIds { get; set; }

        protected override int FilenameIdentifier => CustomerIds == null || CustomerIds.Count == 0
            ? -1
            : CustomerIds.First();

        #endregion Properties

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        /// <summary>
        /// Creates the report.
        /// </summary>
        private void CreateReport()
        {
            this.CreateWorkBook();

            // Create the summary worksheet
            var wks = this.CreateWorksheet(" ");
            var rowIndex = this.AddCompanyHeaderRows(wks, 6, "Summary") + 2;

            this.AddReportData(wks, rowIndex);

            // Set the column widths
            wks.Columns[0].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wks.Columns[1].SetWidth(12, WorksheetColumnWidthUnit.Character);
            wks.Columns[2].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wks.Columns[3].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wks.Columns[4].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wks.Columns[5].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wks.Columns[6].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wks.Columns[7].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wks.Columns[8].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wks.Columns[9].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wks.Columns[10].SetWidth(20, WorksheetColumnWidthUnit.Character);

            // Build a table from the data
            if (_isTableRequired)
                this.CreateTable(wks, rowIndex + 1, _lastColumnIndex, _lastRowIndex + 1, true);

            this.FitAllColumnsOnSinglePage(wks);
        }

        /// <summary>
        /// Adds the report data.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="rowIndex">Index of the row.</param>
        private void AddReportData(Worksheet worksheet, int rowIndex)
        {
            // Get the Orders
            var dtOrders = new OrdersReport.OrderDataTable();
            using (var ta = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter { ClearBeforeFill = false })
            {
                foreach (var customerId in CustomerIds)
                {
                    ta.FillAllOrdersByDateAndCustomer(dtOrders, ToDate, FromDate, customerId);
                }
            }

            // Create the headers
            var columnIndex = 0;
            this.CreateHeaderCell(worksheet, rowIndex, columnIndex++, "Customer");
            this.CreateHeaderCell(worksheet, rowIndex, columnIndex++, "Order ID");
            this.CreateHeaderCell(worksheet, rowIndex, columnIndex++, "Customer WO");
            this.CreateHeaderCell(worksheet, rowIndex, columnIndex++, "PO");
            this.CreateHeaderCell(worksheet, rowIndex, columnIndex++, "Order Date");
            this.CreateHeaderCell(worksheet, rowIndex, columnIndex++, "Est. Ship Date");
            this.CreateHeaderCell(worksheet, rowIndex, columnIndex++, "Part");
            this.CreateHeaderCell(worksheet, rowIndex, columnIndex++, "Part Qty");
            this.CreateHeaderCell(worksheet, rowIndex, columnIndex++, "Part Price");
            this.CreateHeaderCell(worksheet, rowIndex, columnIndex++, "Price Unit");
            this.CreateHeaderCell(worksheet, rowIndex, columnIndex, "Total Price");

            _lastColumnIndex = columnIndex;

            // Add the data
            var startRowIndex = rowIndex + 2;
            if (dtOrders.Count > 0)
            {
                foreach (var order in dtOrders)
                {
                    rowIndex++;
                    columnIndex = 0;
                    this.CreateCell(worksheet, rowIndex, columnIndex++, order.CustomerName);
                    this.CreateCell(worksheet, rowIndex, columnIndex++, order.OrderID);
                    this.CreateCell(worksheet, rowIndex, columnIndex++, order.IsCustomerWONull() ? "NA" : order.CustomerWO);
                    this.CreateCell(worksheet, rowIndex, columnIndex++, order.IsPurchaseOrderNull() ? "NA" : order.PurchaseOrder);
                    this.CreateCell(worksheet, rowIndex, columnIndex++, order.IsOrderDateNull() ? DateTime.MinValue : order.OrderDate);
                    this.CreateCell(worksheet, rowIndex, columnIndex++, order.IsEstShipDateNull() ? DateTime.MinValue : order.EstShipDate);
                    this.CreateCell(worksheet, rowIndex, columnIndex++, order.PartName);

                    var qty = order.IsPartQuantityNull() ? 0 : order.PartQuantity;
                    var price = order.IsBasePriceNull() ? 0 : order.BasePrice;
                    var priceUnit = order.IsPriceUnitNull() ? "NA" : order.PriceUnit;
                    var weight = order.IsWeightNull() ? 0M : order.Weight;

                    decimal fees = OrderPrice.CalculateFees(order, price);
                    var totalPrice = OrderPrice.CalculatePrice(price, priceUnit, fees, qty, weight);
                    //var totalPrice = OrderPrice.CalculatePrice(price, priceUnit, 0, qty);

                    this.CreateCell(worksheet, rowIndex, columnIndex++, qty);
                    this.CreateCell(worksheet, rowIndex, columnIndex++, price, false, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT);
                    this.CreateCell(worksheet, rowIndex, columnIndex++, priceUnit);
                    this.CreateCell(worksheet, rowIndex, columnIndex, totalPrice, false, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT);
                }
                // Set the last row index as all that is left is the totals row and it should not be included in the table
                _lastRowIndex = rowIndex;
                _isTableRequired = true;
                this.HasData = true; // Needs to be set for server email purposes.

                // Add a row for totals
                rowIndex++;
                columnIndex = 0;
                this.CreateCell(worksheet, rowIndex, columnIndex++, "Totals:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateCell(worksheet, rowIndex, columnIndex++, "Orders:", true, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateFormulaCell(worksheet, rowIndex, columnIndex++, "=COUNTA(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 2, rowIndex, 2), CellReferenceMode.R1C1, true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateCell(worksheet, rowIndex, columnIndex++, "", true);
                this.CreateCell(worksheet, rowIndex, columnIndex++, "", true);
                this.CreateCell(worksheet, rowIndex, columnIndex++, "", true);
                this.CreateCell(worksheet, rowIndex, columnIndex++, "Parts:", true, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateFormulaCell(worksheet, rowIndex, columnIndex++, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 8, rowIndex, 8), CellReferenceMode.R1C1, true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.CreateCell(worksheet, rowIndex, columnIndex++, "", true);
                this.CreateCell(worksheet, rowIndex, columnIndex++, "", true);
                this.CreateFormulaCell(worksheet, rowIndex, columnIndex, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex, 11, rowIndex, 11), CellReferenceMode.R1C1, true, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            }
            else
            {
                // No orders, add an empty row
                rowIndex++;
                this.CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 5, "NONE", false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                this.HasData = false; // Needs to be set for server email purposes.
            }
        }

        #endregion Methods
    }

    public class OrderProcessSheetReport : ExcelBaseReport
    {
        #region Fields

        private const int COLUMN_COUNT = 6;

        #endregion

        #region Properties

        public override string Title =>
            "Process Sheet";

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation =>
            PageOrientation.Landscape;

        public int OrderProcessId { get; set; }

        public int ProcessId { get; set; }

        #endregion

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            _log.Debug("Work in Process report is being run");
            CreateWorkBook();
            CreateWorkInProgressReport();
        }

        private void CreateWorkInProgressReport()
        {
            using (var dsOrderProcess = new OrderProcessingDataSet { EnforceConstraints = false })
            {
                if (OrderProcessId > 0)
                {
                    using (var taOrderProcess = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                        taOrderProcess.FillByID(dsOrderProcess.OrderProcesses, this.OrderProcessId);

                    var orderProcess = dsOrderProcess.OrderProcesses.FirstOrDefault();

                    if (orderProcess == null)
                    {
                        this.HasData = false;
                        return;
                    }

                    this.ProcessId = orderProcess.ProcessID;

                    using (var taOrder = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter())
                        taOrder.FillById(dsOrderProcess.OrderSummary, orderProcess.OrderID);

                    var orderSummary = dsOrderProcess.OrderSummary.FindByOrderID(orderProcess.OrderID);

                    using (var taPart = new PartTableAdapter())
                        taPart.FillBy(dsOrderProcess.Part, orderSummary.PartID);
                }

                using (var taProcess = new ProcessTableAdapter())
                    taProcess.FillByProcess(dsOrderProcess.Process, this.ProcessId);

                using (var ta = new ProcessStepsTableAdapter())
                    ta.FillBy(dsOrderProcess.ProcessSteps, this.ProcessId);

                using (var ta = new ProcessStepConditionTableAdapter())
                    ta.Fill(dsOrderProcess.ProcessStepCondition, this.ProcessId);

                using (var ta = new ProcessQuestionTableAdapter())
                    ta.FillBy(dsOrderProcess.ProcessQuestion, this.ProcessId);

                FillWorkInProcessReport(dsOrderProcess);
            }
        }

        private void FillWorkInProcessReport(OrderProcessingDataSet orderProcessingData)
        {
            try
            {
                if (orderProcessingData == null || orderProcessingData.OrderProcesses == null || orderProcessingData.Process == null)
                {
                    return;
                }

                var orderProcess = orderProcessingData.OrderProcesses.FirstOrDefault();
                if (orderProcess != null)
                {
                    var orderSummary = orderProcessingData.OrderSummary.FindByOrderID(orderProcess.OrderID);
                    var process = orderProcessingData.Process.FindByProcessID(this.ProcessId);
                    if (process != null)
                    {
                        var ws = CreateWorksheet($"Process {orderProcess.StepOrder}");
                        var rowIndex = this.AddCompanyHeaderRows(ws, COLUMN_COUNT, "") + 1;

                        CreateHeaderFooter(ws, "Work In Process");
                        ws.PrintOptions.Header = null;
                        ws.PrintOptions.TopMargin = 0.50;
                        ws.PrintOptions.LeftMargin = 0.25;
                        ws.PrintOptions.RightMargin = 0.25;
                        rowIndex++;

                        //Create Work Order / Part Row
                        CreateMergedHeader(ws, rowIndex, 0, rowIndex, 1, "Work Order:").CellFormat.Alignment = HorizontalCellAlignment.Left;
                        var woCell = CreateMergedHeader(ws, rowIndex, 2, rowIndex, 2, orderProcess.OrderID.ToString());
                        woCell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);

                        CreateMergedHeader(ws, rowIndex, 3, rowIndex, 3, "Part:").CellFormat.Alignment = HorizontalCellAlignment.Left;
                        var partNameCell = CreateMergedHeader(ws, rowIndex, 4, rowIndex, COLUMN_COUNT, orderSummary == null ? "" : orderSummary.PartRow.Name);
                        partNameCell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);

                        rowIndex++;

                        //Create Quantity / Weight Row
                        CreateMergedHeader(ws, rowIndex, 0, rowIndex, 1, "Quantity:").CellFormat.Alignment = HorizontalCellAlignment.Left;
                        var quantityCell = CreateMergedHeader(ws, rowIndex, 2, rowIndex, 2, orderSummary == null || orderSummary.IsPartQuantityNull() ? string.Empty : orderSummary.PartQuantity.ToString());
                        quantityCell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);

                        CreateMergedHeader(ws, rowIndex, 3, rowIndex, 3, "Weight:").CellFormat.Alignment = HorizontalCellAlignment.Left;
                        var weightCell = CreateMergedHeader(ws, rowIndex, 4, rowIndex, COLUMN_COUNT, orderSummary == null || orderSummary.IsWeightNull() ? string.Empty : $"{orderSummary.Weight:F2} lbs.");
                        weightCell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);

                        rowIndex++;

                        //Create Process Name Row
                        CreateMergedHeader(ws, rowIndex, 0, rowIndex, 1, "Process:").CellFormat.Alignment = HorizontalCellAlignment.Left;
                        var processNameCell = CreateMergedHeader(ws, rowIndex, 2, rowIndex, COLUMN_COUNT, process.Name + (process.IsRevisionNull() ? "" : " - Rev. " + process.Revision));
                        processNameCell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                        processNameCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                        rowIndex++;

                        //Create Process Description Row
                        CreateMergedHeader(ws, rowIndex, 0, rowIndex, 1, "Description:").CellFormat.Alignment = HorizontalCellAlignment.Left;
                        var processDescription = CreateMergedHeader(ws, rowIndex, 2, rowIndex, COLUMN_COUNT, process.IsDescriptionNull() ? "" : process.Description);
                        processDescription.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                        processDescription.CellFormat.Alignment = HorizontalCellAlignment.Left;
                        rowIndex++;

                        //Create Department Row
                        CreateMergedHeader(ws, rowIndex, 0, rowIndex, 1, "Department:").CellFormat.Alignment = HorizontalCellAlignment.Left;
                        var deptCell = CreateMergedHeader(ws, rowIndex, 2, rowIndex, COLUMN_COUNT, process.Department);
                        deptCell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                        deptCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                        rowIndex++;
                        rowIndex++;

                        //Begin adding questions
                        rowIndex = AddHeaderRow(ws, rowIndex);

                        var order = orderProcessingData.OrderSummary.FirstOrDefault();

                        foreach (var processStep in process.GetProcessStepsRows().OrderBy(step => step.StepOrder))
                        {
                            rowIndex = AddProcessStepRow(ws, processStep, rowIndex);

                            foreach (var processQuestion in processStep.GetProcessQuestionRows().OrderBy(q => q.StepOrder))
                            {
                                rowIndex = AddProcessQuestionRow(ws, processQuestion, order, rowIndex);

                                if (!processQuestion.IsNotesNull())
                                {
                                    rowIndex = AddProcessQuestionNotesRow(ws, processQuestion, rowIndex);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //No records, just create the header so they at least know report was run
                    var ws = CreateWorksheet("Process");
                    AddCompanyHeaderRows(ws, COLUMN_COUNT, "");
                    CreateHeaderFooter(ws, "Work In Process");
                    ws.PrintOptions.Header = null;
                    ws.PrintOptions.TopMargin = 0.50;
                    ws.PrintOptions.LeftMargin = 0.25;
                    ws.PrintOptions.RightMargin = 0.25;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to fill work in process report.");
            }
        }

        private int AddHeaderRow(Worksheet worksheet, int rowIndex)
        {
            CreateHeaderCell(worksheet, rowIndex, 0, "Step", 12);
            CreateMergedHeader(worksheet, rowIndex, 1, rowIndex, 2, "Question");
            worksheet.Columns[2].Width = 25 * 256;

            CreateHeaderCell(worksheet, rowIndex, 3, "Answer", 35);
            CreateHeaderCell(worksheet, rowIndex, 4, "Range", 25);
            CreateHeaderCell(worksheet, rowIndex, 5, "Initials", 15);
            CreateHeaderCell(worksheet, rowIndex, 6, "Date", 12);
            rowIndex++;

            return rowIndex;
        }

        private int AddProcessStepRow(Worksheet worksheet, OrderProcessingDataSet.ProcessStepsRow processStep, int startingRowIndex)
        {
            var rowIndex = startingRowIndex;

            try
            {
                var stepCell = worksheet.Rows[rowIndex].Cells[0];
                stepCell.Value = processStep.StepOrder.ToString();
                stepCell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Beige), null, FillPatternStyle.Solid);
                stepCell.CellFormat.Alignment = HorizontalCellAlignment.Center;
                ApplyCellBorders(stepCell);

                var nameCell = CreateMergedHeader(worksheet, rowIndex, 1, rowIndex, COLUMN_COUNT, processStep.Name);
                nameCell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Beige), null, FillPatternStyle.Solid);
                nameCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                ApplyCellBorders(nameCell.CellFormat);

                rowIndex++;

                foreach (var condition in processStep.GetProcessStepConditionRows())
                {
                    var conditionText = Data.Conditionals.ConditionEvaluator.ConditionToString(condition);
                    var conditionCell = CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, COLUMN_COUNT, "\t* - " + conditionText);
                    conditionCell.CellFormat.Font.Height = 180;
                    conditionCell.CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Red);
                    conditionCell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Beige), null, FillPatternStyle.Solid);
                    conditionCell.CellFormat.Alignment = HorizontalCellAlignment.Right;
                    worksheet.Rows[rowIndex].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Right;
                    ApplyCellBorders(conditionCell.CellFormat);

                    rowIndex++;
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add process step row.");
            }

            return rowIndex;
        }

        private static int AddProcessQuestionRow(Worksheet worksheet,
            OrderProcessingDataSet.ProcessQuestionRow processQuestion,
            OrderProcessingDataSet.OrderSummaryRow order,
            int startingRowIndex)
        {
            var rowIndex = startingRowIndex;

            try
            {
                var stepCell = worksheet.Rows[rowIndex].Cells[1];
                stepCell.Value = processQuestion.StepOrder.ToString();
                stepCell.CellFormat.Alignment = HorizontalCellAlignment.Center;
                ApplyCellBorders(stepCell);

                var nameCell = worksheet.Rows[rowIndex].Cells[2];
                nameCell.Value = processQuestion.Name;
                nameCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                ApplyCellBorders(nameCell.CellFormat);

                var answerCell = worksheet.Rows[rowIndex].Cells[3];
                answerCell.Value = "";
                answerCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                ApplyCellBorders(answerCell.CellFormat);

                var rangeCell = worksheet.Rows[rowIndex].Cells[4];
                rangeCell.Value = GetQuestionTypeText(processQuestion, order);
                rangeCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                ApplyCellBorders(rangeCell.CellFormat);

                var initialsCell = worksheet.Rows[rowIndex].Cells[5];
                initialsCell.Value = "";
                initialsCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                ApplyCellBorders(initialsCell.CellFormat);

                var dateCell = worksheet.Rows[rowIndex].Cells[6];
                dateCell.Value = "";
                dateCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                ApplyCellBorders(dateCell.CellFormat);

                rowIndex++;
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }

            return rowIndex;
        }

        private int AddProcessQuestionNotesRow(Worksheet worksheet, OrderProcessingDataSet.ProcessQuestionRow processQuestion, int startingRowIndex)
        {
            var rowIndex = startingRowIndex;
            try
            {
                if (processQuestion.IsNotesNull())
                {
                    return rowIndex;
                }

                var formattedNotes = processQuestion.Notes.StripHtml();

                var notesCell = CreateMergedHeader(worksheet, rowIndex, 2, rowIndex, COLUMN_COUNT, formattedNotes);
                notesCell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                notesCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                ApplyCellBorders(notesCell.CellFormat);

                rowIndex++;
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }

            return rowIndex;
        }

        private static string GetQuestionTypeText(OrderProcessingDataSet.ProcessQuestionRow question,
            OrderProcessingDataSet.OrderSummaryRow order)
        {
            var inputType = (InputType)Enum.Parse(typeof(InputType), question.InputType);
            string text = inputType.ToString();

            switch (inputType)
            {
                case InputType.Decimal:
                case InputType.DecimalBefore:
                case InputType.DecimalAfter:
                case InputType.DecimalDifference:
                case InputType.PreProcessWeight:
                case InputType.PostProcessWeight:
                    text += " [";

                    var decMaxValue = ProcessUtilities.MaxValueDecimal(question, order);
                    var decMinValue = ProcessUtilities.MinValueDecimal(question, order);

                    if (decMinValue.HasValue && decMaxValue.HasValue)
                    {
                        text += decMinValue + " - " + decMaxValue;
                    }

                    if (!question.IsNumericUntisNull())
                    {
                        text += (decMaxValue.HasValue ? " " : string.Empty) + question.NumericUntis;
                    }

                    text += "]";
                    break;
                case InputType.Integer:
                case InputType.TimeDuration:
                case InputType.RampTime:
                    text += " [";

                    var intMaxValue = ProcessUtilities.MaxValueInt32(question, order);
                    var intMinValue = ProcessUtilities.MinValueInt32(question, order);

                    if (intMinValue.HasValue && intMaxValue.HasValue)
                    {
                        text += intMinValue + " - " + intMaxValue;
                    }

                    if (!question.IsNumericUntisNull())
                    {
                        text += (intMaxValue.HasValue ? " " : string.Empty) + question.NumericUntis;
                    }

                    text += "]";
                    break;
            }

            return text;
        }

        #endregion
    }

    public class SalesOrderPriceReport : Report
    {
        #region Fields

        private readonly OrdersDataSet.SalesOrderRow _salesOrder;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Sales Order Price Sheet"; }
        }

        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }

        protected override int FilenameIdentifier => _salesOrder?.SalesOrderID ?? 0;

        #endregion

        #region Methods

        public SalesOrderPriceReport(OrdersDataSet.SalesOrderRow salesOrder) { _salesOrder = salesOrder; }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            SetupReportInfo();
            _section.PageMargins.All = 20;

            AddHeader("SALES ORDER " + _salesOrder.SalesOrderID, true);
            AddWOSection();
        }

        private void AddWOSection()
        {
            var dsOrder = new OrdersDataSet()
            {
                EnforceConstraints = false
            };

            var taOrders = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter();
            var taOrderFeeType = new Data.Datasets.OrdersDataSetTableAdapters.OrderFeeTypeTableAdapter();
            var taOrderFees = new Data.Datasets.OrdersDataSetTableAdapters.OrderFeesTableAdapter();

            try
            {
                var headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Margins = new Margins(5, 5, 3, 0);
                headerGroup.Borders = DefaultStyles.DefaultBorders;
                headerGroup.Background = DefaultStyles.DefaultBackground;

                taOrders.FillBySalesOrder(dsOrder.Order, _salesOrder.SalesOrderID);
                taOrderFeeType.Fill(dsOrder.OrderFeeType);

                //Add WO info table
                Infragistics.Documents.Reports.Report.IContainer orderContainer = null;
                foreach (var order in dsOrder.Order)
                {
                    taOrderFees.FillByOrder(dsOrder.OrderFees, order.OrderID);

                    if (orderContainer == null)
                    {
                        orderContainer = headerGroup.AddContainer("order");
                    }

                    var orderTable = orderContainer.AddTable();
                    orderTable.Borders = DefaultStyles.DefaultBorders;
                    orderTable.Paddings.All = 5;

                    orderTable.AddRow().AddCells(DefaultStyles.BlueLargeStyle, new SolidColorBrush(new Color(System.Drawing.Color.WhiteSmoke)), null, 0, TextAlignment.Left,
                        "Order: " + order.OrderID);

                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle),
                        "   Customer WO:", order.IsCustomerWONull() ? string.Empty : order.CustomerWO);

                    //Get the part for the order
                    OrdersDataSet.PartSummaryDataTable partTable = new OrdersDataSet.PartSummaryDataTable();
                    using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.PartSummaryTableAdapter { ClearBeforeFill = false })
                    {
                        ta.FillByPart(partTable, order.PartID);
                        //Should only be one part per order
                        if (partTable.Rows.Count == 1)
                        {
                            OrdersDataSet.PartSummaryRow partName = partTable.Rows[0] as OrdersDataSet.PartSummaryRow;
                            orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   PartName:", partName.Name);
                        }
                    }

                    var partQuantity = order.IsPartQuantityNull() ? 0 : order.PartQuantity;
                    var basePrice = order.IsBasePriceNull() ? 0M : order.BasePrice;

                    var totalPrice = OrderPrice.CalculatePrice(basePrice,
                        order.IsPriceUnitNull() ? OrderPrice.enumPriceUnit.Each.ToString() : order.PriceUnit,
                        OrderPrice.CalculateFees(order, basePrice),
                        partQuantity,
                        order.IsWeightNull() ? 0M : order.Weight);

                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Part Quantity:", partQuantity.ToString());
                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Base Price:", basePrice.ToString("F2"));
                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Price Unit:", order.PriceUnit);
                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Total Price:", string.Format("{0:#.00}", totalPrice));
                }
            }
            catch (Exception exc)
            {
                _log.Fatal(exc, "Error adding WO section to report.");
            }
            finally
            {
                dsOrder?.Dispose();
                taOrders?.Dispose();
                taOrderFeeType?.Dispose();
                taOrderFees?.Dispose();
            }
        }

        #endregion
    }

    public class OrderHistoryReport : ExcelBaseReport
    {
        #region Fields

        private int _order;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Order History"; }
        }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }

        #endregion

        #region Methods

        public OrderHistoryReport(int orderId)
        {
            this._order = orderId;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            SetupReportInfo();

            var wks = CreateWorksheet(Title);
            var rowIndex = AddHeaderRow(wks);
            var startRowIndex = rowIndex;

            CreateHeaderFooter(wks, Title + ": " + _order);

            var ds = new DWOS.Data.Datasets.OrderHistoryDataSet();
            using (var ta = new Data.Datasets.OrderHistoryDataSetTableAdapters.OrderHistoryTableAdapter())
            {
                ta.FillByOrder(ds.OrderHistory, _order);

                foreach (var item in ds.OrderHistory)
                {
                    AddOrderRow(item, wks, ref rowIndex);
                    rowIndex++;
                }
            }

            AddSummaryRow(wks, rowIndex);

            CreateTable(wks, startRowIndex, 5, rowIndex, true);
        }

        private void AddOrderRow(OrderHistoryDataSet.OrderHistoryRow order, Worksheet wks, ref int rowIndex)
        {
            try
            {
                int colIndex = 0;

                CreateCell(wks, rowIndex, colIndex++, order.OrderID);
                CreateCell(wks, rowIndex, colIndex++, order.Category);
                CreateCell(wks, rowIndex, colIndex++, order.Description);
                CreateCell(wks, rowIndex, colIndex++, order.UserName);
                CreateCell(wks, rowIndex, colIndex++, order.Machine);
                CreateCell(wks, rowIndex, colIndex++, order.IsDateCreatedNull() ? (object)"NA" : order.DateCreated);
            }
            catch (Exception exc)
            {
                _log.Warn(exc, "Unable to determine order details.");
            }
        }

        private int AddHeaderRow(Worksheet wks)
        {
            var rowIndex = base.AddCompanyHeaderRows(wks, 4, String.Empty) + 2;
            int colIndex = 0;

            CreateHeaderCell(wks, rowIndex, colIndex++, "WO", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Category", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Description", 80);
            CreateHeaderCell(wks, rowIndex, colIndex++, "User Name", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Machine", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Date", 20);

            return ++rowIndex;
        }

        private void AddSummaryRow(Worksheet wks, int rowIndex)
        {

        }

        #endregion
    }

    public sealed class OrderCostReport : ExcelBaseReport
    {
        #region Fields

        private const int LAST_COLUMMN_INDEX = 14;

        #endregion

        #region Properties

        [Browsable(true)]
        [Description("The from date of the report.")]
        [DisplayName("From Date")]
        [Category("Report")]
        public DateTime FromDate
        {
            get;
            set;
        }

        [Browsable(true)]
        [Description("The to date of the report.")]
        [DisplayName("To Date")]
        [Category("Report")]
        public DateTime ToDate
        {
            get;
            set;
        }

        [Browsable(true)]
        [TypeConverter(typeof(CustomerConverterAll))]
        [DisplayName("Customer")]
        [Category("Report")]
        public int CustomerID
        {
            get;
            set;
        }

        public override string Title
        {
            get
            {
                return "Order Cost";
            }
        }

        #endregion

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            try
            {
                var adjustedFromDate = DateUtilities.StartOfDay(FromDate);
                var adjustedToDate = DateUtilities.EndOfDay(ToDate);

                var reportData = GetReportData(adjustedFromDate, adjustedToDate, CustomerID);

                var worksheet = CreateWorksheet(Title);

                var rowIndex = CreateTopHeader(worksheet);

                var contentStartRowIndex = rowIndex;

                rowIndex = CreateContentHeader(worksheet, rowIndex);

                if (reportData.Count() > 0)
                {
                    rowIndex = CreateContent(worksheet, rowIndex, reportData);
                }
                else
                {
                    rowIndex++;
                }

                CreateSummary(worksheet, contentStartRowIndex, rowIndex);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error running {0}", this.Title);
            }
        }

        private int CreateTopHeader(Worksheet worksheet)
        {
            int rowIndex = 0;

            //Add Header Title
            WorksheetMergedCellsRegion region = CreateMergedCell(worksheet, rowIndex, 0, rowIndex, LAST_COLUMMN_INDEX, Title);

            //Set the cell alignment of the merged region. Since a cell and its merged region shared a cell format, this will ultimately set the format of the merged region
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            worksheet.Rows[rowIndex].Height = 18 * 20;
            region.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            region.CellFormat.Font.Name = DefaultFontName;
            region.CellFormat.Font.Height = 18 * 20;
            region.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);

            rowIndex++;

            CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 1, "From Date:  " + this.FromDate.ToShortDateString()).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateMergedCell(worksheet, rowIndex, 2, rowIndex, 4, "Company:    " + ApplicationSettings.Current.CompanyName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;

            CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 1, "To Date:  " + this.ToDate.ToShortDateString()).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateMergedCell(worksheet, rowIndex, 2, rowIndex, 4, "User:             " + SecurityManager.UserName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;

            CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 1, string.Empty);
            CreateMergedCell(worksheet, rowIndex, 2, rowIndex, 4, "Date:             " + DateTime.Now).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;
            return rowIndex;
        }

        private int CreateContentHeader(Worksheet worksheet, int startingRowIndex)
        {
            var rowIndex = startingRowIndex;
            var colIndex = 0;

            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Customer", 25);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "WO", 10);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Part Quantity", 15);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Revenue", 18);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Labor Hours", 15);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Material Cost", 15);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "%", 12);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Labor Cost", 15);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "%", 12);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Burden Cost", 15);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "%", 12);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Total Cost", 15);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "%", 12);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Margin", 18);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "%", 12);

            rowIndex++;
            return rowIndex;
        }

        private int CreateContent(Worksheet worksheet, int startingRowIndex, IEnumerable<OrderCostData> reportData)
        {
            var rowIndex = startingRowIndex;

            foreach (var order in reportData.OrderBy(i => i.OrderId))
            {
                CreateCell(worksheet, rowIndex, 0, order.Customer);
                CreateCell(worksheet, rowIndex, 1, order.OrderId);

                if (order.PartQuantity.HasValue)
                {
                    CreateCell(worksheet, rowIndex, 2, order.PartQuantity);
                }
                else
                {
                    CreateCell(worksheet, rowIndex, 2, "NA");
                }

                if (order.Revenue.HasValue)
                {
                    CreateCell(worksheet, rowIndex, 3, order.Revenue, cellFormat: MONEY_FORMAT);
                }
                else
                {
                    CreateCell(worksheet, rowIndex, 3, "NA");
                }

                CreateCell(worksheet, rowIndex, 4, order.LaborMinutes/ DateUtilities.MINUTES_PER_HOUR, cellFormat: DURATION_FORMAT);
                CreateCell(worksheet, rowIndex, 5, order.MaterialCost, cellFormat: MONEY_FORMAT);

                var materialPercentFormula = string.Format("=IF(R{0}C{1}=0, 0, R{0}C{2}/R{0}C{1})",
                    rowIndex + 1,
                    4, // Revenue
                    6); // Material

                CreateFormulaCell(worksheet, rowIndex, 6, materialPercentFormula, CellReferenceMode.R1C1, cellFormat: PERCENT_FORMAT);

                CreateCell(worksheet, rowIndex, 7, order.LaborCost, cellFormat: MONEY_FORMAT);

                var laborPercentFormula = string.Format("=IF(R{0}C{1}=0, 0, R{0}C{2}/R{0}C{1})",
                    rowIndex + 1,
                    4, // Revenue
                    8); // Material

                CreateFormulaCell(worksheet, rowIndex, 8, laborPercentFormula, CellReferenceMode.R1C1, cellFormat: PERCENT_FORMAT);

                CreateCell(worksheet, rowIndex, 9, order.BurdenCost, cellFormat: MONEY_FORMAT);

                var burdenPercentFormula  = string.Format("=IF(R{0}C{1}=0, 0, R{0}C{2}/R{0}C{1})",
                    rowIndex + 1,
                    4, // Revenue
                    10); // Burden

                CreateFormulaCell(worksheet, rowIndex, 10, burdenPercentFormula, CellReferenceMode.R1C1, cellFormat: PERCENT_FORMAT);

                var totalCostFormula = string.Format("=R{0}C{1} + R{0}C{2} + R{0}C{3}",
                    rowIndex + 1,
                    6, // Material
                    8, // Labor
                    10); // Burden

                CreateFormulaCell(worksheet, rowIndex, 11, totalCostFormula, CellReferenceMode.R1C1, cellFormat: MONEY_FORMAT);

                var totalPercentFormula  = string.Format("=IF(R{0}C{1}=0, 0, R{0}C{2}/R{0}C{1})",
                    rowIndex + 1,
                    4, // Revenue
                    12); // Total

                CreateFormulaCell(worksheet, rowIndex, 12, totalPercentFormula, CellReferenceMode.R1C1, cellFormat: PERCENT_FORMAT);

                var marginFormula = string.Format("=R{0}C{1} - R{0}C{2}",
                    rowIndex + 1,
                    4,   //Revenue
                    12); // Total

                CreateFormulaCell(worksheet, rowIndex, 13, marginFormula, CellReferenceMode.R1C1, cellFormat: MONEY_FORMAT);

                var marginPercentFormula  = string.Format("=IF(R{0}C{1}=0, 0, R{0}C{2}/R{0}C{1})",
                    rowIndex + 1,
                    4, // Revenue
                    14); // Margin

                CreateFormulaCell(worksheet, rowIndex, 14, marginPercentFormula, CellReferenceMode.R1C1, cellFormat: PERCENT_FORMAT);

                rowIndex++;
            }

            return rowIndex;
        }

        private void CreateSummary(Worksheet worksheet, int contentStartRowIndex, int rowIndex)
        {
            Bold(CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 2,
                "Totals:", true, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin));

            var percentageColumns = new List<int>()
            {
                6,  // Material %
                8,  // Labor %
                10, // Burden %
                12, // Total %
                14  // Margin %
            };

            var durationColumns = new List<int>()
            {
                4 // Labor Duration
            };

            for (int colIndex = 3; colIndex <= LAST_COLUMMN_INDEX; colIndex++)
            {
                string totalFormula;
                string cellFormat;

                if (percentageColumns.Contains(colIndex))
                {
                    totalFormula = string.Format("=AVERAGE(R{0}C{1}:R{2}C{1})",
                        contentStartRowIndex + 1,
                        colIndex + 1,
                        rowIndex);

                    cellFormat = PERCENT_FORMAT;
                }
                else if (durationColumns.Contains(colIndex))
                {
                    totalFormula = string.Format("=SUM(R{0}C{1}:R{2}C{1})",
                        contentStartRowIndex + 1,
                        colIndex + 1,
                        rowIndex);

                    cellFormat = DURATION_FORMAT;
                }
                else
                {
                    totalFormula = string.Format("=SUM(R{0}C{1}:R{2}C{1})",
                        contentStartRowIndex + 1,
                        colIndex + 1,
                        rowIndex);

                    cellFormat = MONEY_FORMAT;
                }

                Bold(CreateFormulaCell(worksheet, rowIndex, colIndex, totalFormula, CellReferenceMode.R1C1, true, cellFormat: cellFormat));
            }
        }

        private static IEnumerable<OrderCostData> GetReportData(DateTime fromDate, DateTime toDate, int customerId)
        {
            OrdersReport dsReport = null;

            try
            {
                var today = DateTime.Today;
                var costData = new List<OrderCostData>();

                dsReport = new OrdersReport()
                {
                    EnforceConstraints = false
                };

                using (var taOrder = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter() { ClearBeforeFill = false })
                {
                    if (customerId > 0)
                    {
                        taOrder.FillClosedOrderByCustomer(dsReport.Order, toDate, fromDate, customerId);

                        if (toDate >= today && fromDate <= today)
                        {
                            taOrder.FillOpenOrdersByCustomer(dsReport.Order, customerId);
                        }
                    }
                    else
                    {
                        taOrder.FillByClosedOrders(dsReport.Order, toDate, fromDate);

                        if (toDate >= today && fromDate <= today)
                        {
                            taOrder.FillOpenOrders(dsReport.Order);
                        }
                    }
                }

                using (var taOrderFee = new Data.Reports.OrdersReportTableAdapters.OrderFeesTableAdapter() { ClearBeforeFill = false })
                {
                    foreach (var order in dsReport.Order)
                    {
                        taOrderFee.FillByOrder(dsReport.OrderFees, order.OrderID);
                    }
                }

                using (var taOrderFeeType = new Data.Reports.OrdersReportTableAdapters.OrderFeeTypeTableAdapter())
                {
                    taOrderFeeType.Fill(dsReport.OrderFeeType);
                }

                using (var taLaborSummary = new LaborSummaryTableAdapter() { ClearBeforeFill = false } )
                {
                    foreach (var order in dsReport.Order)
                    {
                        taLaborSummary.FillByOrder(dsReport.LaborSummary, order.OrderID);
                    }
                }

                var defaultEndTime = DateTime.Now;

                var validWorkStatuses = new List<string>()
                {
                    ApplicationSettings.Current.WorkStatusFinalInspection,
                    ApplicationSettings.Current.WorkStatusShipping,
                    ApplicationSettings.Current.WorkStatusCompleted
                };

                foreach (var order in dsReport.Order)
                {
                    if (validWorkStatuses.Contains(order.WorkStatus))
                    {
                        costData.Add(OrderCostData.From(order, defaultEndTime));
                    }
                }

                return costData;
            }
            finally
            {
                dsReport?.Dispose();
            }
        }

        #endregion
    }

    public sealed class ProfitComparisonReport : ExcelBaseReport, IPartReport
    {
        #region Fields

        private const int LAST_COLUMMN_INDEX_PRIMARY = 12;
        private const int LAST_COLUMMN_INDEX_DETAIL = 5;

        #endregion

        #region Properties

        public override string Title => "Profit Comparison";

        #endregion

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            try
            {
                var adjustedFromDate = FromDate.StartOfDay();
                var adjustedToDate = ToDate.EndOfDay();

                var reportData = GetReportData(adjustedFromDate, adjustedToDate, PartID);

                CreatePrimaryWorksheet(reportData);
                CreateProcessBreakdownWorksheet(reportData);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error running {0}", Title);
            }
        }

        private void CreatePrimaryWorksheet(List<OrderInfo> reportData)
        {
            if (reportData == null)
            {
                throw new ArgumentNullException(nameof(reportData));
            }

            var worksheet = CreateWorksheet(Title);
            var rowIndex = CreateTopHeader(worksheet, Title, LAST_COLUMMN_INDEX_PRIMARY);
            rowIndex = CreatePrimaryContentHeader(worksheet, rowIndex);

            var contentStartRowIndex = rowIndex;
            rowIndex = CreatePrimaryContent(worksheet, rowIndex, reportData);

            if (!reportData.Any())
            {
                rowIndex++;
            }

            CreatePrimarySummary(worksheet, contentStartRowIndex, rowIndex);
        }

        private void CreateProcessBreakdownWorksheet(List<OrderInfo> reportData)
        {
            if (reportData == null)
            {
                throw new ArgumentNullException(nameof(reportData));
            }

            var worksheet = CreateWorksheet("Details");

            var rowIndex = CreateTopHeader(worksheet, "Profit Comparison Details", LAST_COLUMMN_INDEX_DETAIL);
            rowIndex = CreateDetailsContentHeader(worksheet, rowIndex);

            var contentStartRowIndex = rowIndex;
            rowIndex = CreateDetailsContent(worksheet, rowIndex, reportData);

            if (!reportData.Any())
            {
                rowIndex++;
            }

            CreateDetailsSummary(worksheet, contentStartRowIndex, rowIndex);
        }

        private int CreateTopHeader(Worksheet worksheet, string title, int lastColumnIndex)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException(nameof(worksheet));
            }

            int rowIndex = 0;

            //Add Header Title
            WorksheetMergedCellsRegion region = CreateMergedCell(worksheet, rowIndex, 0, rowIndex, lastColumnIndex, title);

            //Set the cell alignment of the merged region. Since a cell and its merged region shared a cell format, this will ultimately set the format of the merged region
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            worksheet.Rows[rowIndex].Height = 18 * 20;
            region.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            region.CellFormat.Font.Name = DefaultFontName;
            region.CellFormat.Font.Height = 18 * 20;
            region.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);

            rowIndex++;

            CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 1, "From Date:  " + this.FromDate.ToShortDateString()).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateMergedCell(worksheet, rowIndex, 2, rowIndex, 4, "Company:    " + ApplicationSettings.Current.CompanyName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;

            CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 1, "To Date:  " + this.ToDate.ToShortDateString()).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateMergedCell(worksheet, rowIndex, 2, rowIndex, 4, "User:             " + SecurityManager.UserName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;

            CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 1, string.Empty);
            CreateMergedCell(worksheet, rowIndex, 2, rowIndex, 4, "Date:             " + DateTime.Now).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;
            return rowIndex;
        }

        #region Primary

        private int CreatePrimaryContentHeader(Worksheet worksheet, int startingRowIndex)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException(nameof(worksheet));
            }

            var rowIndex = startingRowIndex;

            var colIndex = 0;

            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Part #", 25);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "WO", 10);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Price Per Part", 18);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Part Quantity", 15);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Revenue", 18);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Employees", 15);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Employee IDs", 20);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Labor Hours", 15);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Labor Cost", 25);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Material Cost", 15);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Burden Cost", 15);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Total Cost", 15);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Margin", 18);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Margin %", 12);

            rowIndex++;
            return rowIndex;
        }

        private int CreatePrimaryContent(Worksheet worksheet, int startingRowIndex, List<OrderInfo> reportData)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException(nameof(worksheet));
            }

            if (reportData == null)
            {
                throw new ArgumentNullException(nameof(reportData));
            }

            var rowIndex = startingRowIndex;

            foreach (var orderInfo in reportData.OrderBy(i => i.OrderId))
            {
                var orderProcesses = orderInfo.Processes ?? new List<OrderProcessInfo>();

                var employees = orderProcesses
                    .SelectMany(i => i.Employees ?? new List<OrderProcessEmployeeInfo>())
                    .ToList();

                var laborHours = employees.Sum(i => i.Minutes / DateUtilities.MINUTES_PER_HOUR);

                var orderOperatorInitials = new List<string>();

                // Initials for each operator
                foreach (var userId in employees.Select(i => i.UserId).Distinct())
                {
                    var firstOperator = employees.First(i => i.UserId == userId);
                    orderOperatorInitials.Add(firstOperator.Initials);
                }

                orderOperatorInitials.Sort();

                CreateCell(worksheet, rowIndex, 0, orderInfo.PartName);
                CreateCell(worksheet, rowIndex, 1, orderInfo.OrderId);
                CreateCell(worksheet, rowIndex, 2, orderInfo.PricePerPart, cellFormat: MONEY_FORMAT);

                if (orderInfo.PartQuantity.HasValue)
                {
                    CreateCell(worksheet, rowIndex, 3, orderInfo.PartQuantity.Value);
                }
                else
                {
                    CreateCell(worksheet, rowIndex, 3, "NA");
                }

                if (orderInfo.Revenue.HasValue)
                {
                    CreateCell(worksheet, rowIndex, 4, orderInfo.Revenue, cellFormat: MONEY_FORMAT);
                }
                else
                {
                    CreateCell(worksheet, rowIndex, 4, "NA");
                }

                CreateCell(worksheet, rowIndex, 5, employees.Count);
                WrapText(CreateCell(worksheet, rowIndex, 6, string.Join(", ", orderOperatorInitials)));
                CreateCell(worksheet, rowIndex, 7, laborHours, cellFormat: DURATION_FORMAT);
                CreateCell(worksheet, rowIndex, 8, employees.Sum(i => i.LaborWageCost + i.LaborBurdenCost), cellFormat: MONEY_FORMAT);
                CreateCell(worksheet, rowIndex, 9, orderProcesses.Sum(i => i.MaterialCost), cellFormat: MONEY_FORMAT);
                CreateCell(worksheet, rowIndex, 10, orderProcesses.Sum(i => i.ProcessBurdenCost), cellFormat: MONEY_FORMAT);

                // Total Cost
                var totalCostFormula = string.Format("=R{0}C{1} + R{0}C{2} + R{0}C{3}",
                    rowIndex + 1,
                    9,   // Labor
                    10,  // Material
                    11); // Burden

                CreateFormulaCell(worksheet, rowIndex, 11, totalCostFormula, CellReferenceMode.R1C1, cellFormat: MONEY_FORMAT);

                // Margin
                var marginFormula = string.Format("=R{0}C{1} - R{0}C{2}",
                    rowIndex + 1,
                    5,   //Revenue
                    12); // Total Cost

                CreateFormulaCell(worksheet, rowIndex, 12, marginFormula, CellReferenceMode.R1C1, cellFormat: MONEY_FORMAT);

                // Margin %
                var marginPercentFormula  = string.Format("=IF(R{0}C{1}=0, 0, R{0}C{2}/R{0}C{1})",
                    rowIndex + 1,
                    5, // Revenue
                    13); // Margin

                CreateFormulaCell(worksheet, rowIndex, 13, marginPercentFormula, CellReferenceMode.R1C1, cellFormat: PERCENT_FORMAT);

                rowIndex++;
            }

            return rowIndex;
        }

        private void CreatePrimarySummary(Worksheet worksheet, int contentStartRowIndex, int startingRowIndex)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException(nameof(worksheet));
            }

            var rowIndex = startingRowIndex;

            Bold(CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 3,
                "Totals:", true, HorizontalCellAlignment.Right));

            var percentageColumns = new List<int>()
            {
                12  // Margin %
            };

            var durationColumns = new List<int>()
            {
                7 // Labor Cost
            };

            var skipColumns = new List<int>()
            {
                5, // Employees
                6  // Employee IDs
            };

            for (int colIndex = 4; colIndex <= LAST_COLUMMN_INDEX_PRIMARY; colIndex++)
            {
                if (skipColumns.Contains(colIndex))
                {
                    Bold(CreateCell(worksheet, rowIndex, colIndex, string.Empty, true));
                    continue;
                }

                string totalFormula;
                string cellFormat;

                if (percentageColumns.Contains(colIndex))
                {
                    totalFormula = string.Format("=AVERAGE(R{0}C{1}:R{2}C{1})",
                        contentStartRowIndex + 1,
                        colIndex + 1,
                        rowIndex);

                    cellFormat = PERCENT_FORMAT;
                }
                else if (durationColumns.Contains(colIndex))
                {
                    totalFormula = string.Format("=SUM(R{0}C{1}:R{2}C{1})",
                        contentStartRowIndex + 1,
                        colIndex + 1,
                        rowIndex);

                    cellFormat = DURATION_FORMAT;
                }
                else
                {
                    totalFormula = string.Format("=SUM(R{0}C{1}:R{2}C{1})",
                        contentStartRowIndex + 1,
                        colIndex + 1,
                        rowIndex);

                    cellFormat = MONEY_FORMAT;
                }

                Bold(CreateFormulaCell(worksheet, rowIndex, colIndex,
                    totalFormula, CellReferenceMode.R1C1, true, cellFormat: cellFormat));
            }
        }

        #endregion

        #region Details

        private int CreateDetailsContentHeader(Worksheet worksheet, int startingRowIndex)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException(nameof(worksheet));
            }

            var rowIndex = startingRowIndex;

            int colIndex = 0;
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Part #", 25);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "WO", 10);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Process", 25);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Employees", 15);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Employee IDs", 20);
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Labor Hours", 15);

            rowIndex++;
            return rowIndex;
        }

        private int CreateDetailsContent(Worksheet worksheet, int startingRowIndex, List<OrderInfo> reportData)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException(nameof(worksheet));
            }

            if (reportData == null)
            {
                throw new ArgumentNullException(nameof(reportData));
            }

            var rowIndex = startingRowIndex;

            foreach (var order in reportData.OrderBy(i => i.OrderId))
            {
                var orderProcesses = order.Processes ?? Enumerable.Empty<OrderProcessInfo>();

                foreach (var orderProcess in orderProcesses.OrderBy(i => i.StepOrder))
                {
                    var employees = orderProcess.Employees ?? new List<OrderProcessEmployeeInfo>();
                    var laborHours = employees.Sum(i => i.Minutes / DateUtilities.MINUTES_PER_HOUR);

                    CreateCell(worksheet, rowIndex, 0, order.PartName);
                    CreateCell(worksheet, rowIndex, 1, order.OrderId);
                    CreateCell(worksheet, rowIndex, 2, orderProcess.ProcessName);
                    CreateCell(worksheet, rowIndex, 3, employees.Count);
                    CreateCell(worksheet, rowIndex, 4, string.Join(", ", employees.Select(i => i.Initials).OrderBy(i => i)));
                    CreateCell(worksheet, rowIndex, 5, laborHours, cellFormat: DURATION_FORMAT);

                    rowIndex++;
                }
            }

            return rowIndex;
        }

        private void CreateDetailsSummary(Worksheet worksheet, int contentStartRowIndex, int rowIndex)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException(nameof(worksheet));
            }

            Bold(CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 4,
                "Totals:", true, HorizontalCellAlignment.Right));

            // Labor Hours
            var totalFormula = string.Format("=SUM(R{0}C{1}:R{2}C{1})",
                contentStartRowIndex + 1,
                6, // Labor Hours
                rowIndex);

            Bold(CreateFormulaCell(worksheet, rowIndex, 5,
                totalFormula, CellReferenceMode.R1C1, true, cellFormat: DURATION_FORMAT));
        }

        #endregion

        private static List<OrderInfo> GetReportData(DateTime fromDate, DateTime toDate, int partId)
        {
            OrdersReport dsReport = null;

            try
            {
                var today = DateTime.Today;
                var reportData = new List<OrderInfo>();

                dsReport = new OrdersReport()
                {
                    EnforceConstraints = false
                };

                using (var taOrder = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter() { ClearBeforeFill = false })
                {
                    taOrder.FillCompletedOrdersByDateAndPart(dsReport.Order, toDate, fromDate, partId);

                    if (toDate >= today && fromDate <= today)
                    {
                        taOrder.FillOpenOrdersByPart(dsReport.Order, partId);
                    }
                }

                using (var taOrderFee = new Data.Reports.OrdersReportTableAdapters.OrderFeesTableAdapter() { ClearBeforeFill = false })
                {
                    foreach (var order in dsReport.Order)
                    {
                        taOrderFee.FillByOrder(dsReport.OrderFees, order.OrderID);
                    }
                }

                using (var taOrderFeeType = new Data.Reports.OrdersReportTableAdapters.OrderFeeTypeTableAdapter())
                {
                    taOrderFeeType.Fill(dsReport.OrderFeeType);
                }

                using (var taLaborSummary = new LaborSummaryTableAdapter() { ClearBeforeFill = false } )
                {
                    foreach (var order in dsReport.Order)
                    {
                        taLaborSummary.FillByOrder(dsReport.LaborSummary, order.OrderID);
                    }
                }

                var defaultEndTime = DateTime.Now;

                var validWorkStatuses = new List<string>()
                {
                    ApplicationSettings.Current.WorkStatusFinalInspection,
                    ApplicationSettings.Current.WorkStatusShipping,
                    ApplicationSettings.Current.WorkStatusCompleted
                };

                foreach (var order in dsReport.Order)
                {
                    if (validWorkStatuses.Contains(order.WorkStatus))
                    {
                        reportData.Add(OrderInfo.From(order, defaultEndTime));
                    }
                }

                return reportData;
            }
            finally
            {
                dsReport?.Dispose();
            }
        }

        #endregion

        #region IPartReport Members

        [Browsable(true)]
        [Description("The from date of the report.")]
        [DisplayName("From Date")]
        [Category("Report")]
        public DateTime FromDate
        {
            get;
            set;
        }

        [Browsable(true)]
        [Description("The to date of the report.")]
        [DisplayName("To Date")]
        [Category("Report")]
        public DateTime ToDate
        {
            get;
            set;
        }

        [Browsable(true)]
        [TypeConverter(typeof(CustomerConverterAll))]
        [DisplayName("Customer")]
        [Category("Report")]
        public int CustomerID
        {
            get;
            set;
        }

        [Browsable(true)]
        [DisplayName("PartID")]
        [Category("Report")]
        public int PartID
        {
            get;
            set;
        }

        #endregion

        #region OrderInfo

        private sealed class OrderInfo
        {
            #region Properties

            public string PartName
            {
                get;
                private set;
            }

            public int OrderId
            {
                get;
                private set;
            }

            public decimal PricePerPart
            {
                get;
                private set;
            }

            public int? PartQuantity
            {
                get;
                private set;
            }

            public decimal? Revenue
            {
                get;
                private set;
            }

            public List<OrderProcessInfo> Processes
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public static OrderInfo From(OrdersReport.OrderRow order, DateTime defaultEndTime)
            {
                OrdersDataSet dsOrders = null;
                SecurityDataSet dsSecurity = null;
                LaborSummaryTableAdapter taLaborSummary = null;

                try
                {
                    if (order == null)
                    {
                        throw new ArgumentNullException(nameof(order));
                    }

                    dsOrders = new OrdersDataSet()
                    {
                        EnforceConstraints = false
                    };

                    dsSecurity = new SecurityDataSet()
                    {
                        EnforceConstraints = false
                    };

                    taLaborSummary = new LaborSummaryTableAdapter();

                    using (var taUser = new Data.Datasets.SecurityDataSetTableAdapters.UsersTableAdapter())
                    {
                        taUser.Fill(dsSecurity.Users);
                    }

                    using (var taUserSalary = new Data.Datasets.SecurityDataSetTableAdapters.UserSalaryTableAdapter())
                    {
                        taUserSalary.Fill(dsSecurity.UserSalary);
                    }

                    var basePrice = order.IsBasePriceNull() ? 0 : order.BasePrice;
                    var partQuantity = order.IsPartQuantityNull() ? (int?)null : order.PartQuantity;
                    var priceUnit = order.IsPriceUnitNull() ? OrderPrice.enumPriceUnit.Each.ToString() : order.PriceUnit;
                    var weight = order.IsWeightNull() ? 0M : order.Weight;

                    decimal? total;

                    if (order.IsPriceUnitNull() || order.IsPartQuantityNull() || order.IsBasePriceNull())
                    {
                        total = null;
                    }
                    else
                    {
                        total = OrderPrice.CalculatePrice(basePrice,
                            priceUnit,
                            OrderPrice.CalculateFees(order, basePrice),
                            partQuantity ?? 0,
                            weight);
                    }

                    var orderInfo = new OrderInfo()
                    {
                        OrderId = order.OrderID,
                        PartName = order.PartName,
                        PartQuantity = partQuantity,
                        PricePerPart = OrderPrice.CalculateEachPrice(basePrice, priceUnit, partQuantity ?? 0, weight),
                        Revenue = total
                    };

                    // Order Processes
                    using (var taOrderProcess = new Data.Datasets.OrdersDataSetTableAdapters.OrderProcessesTableAdapter())
                    {
                        taOrderProcess.FillBy(dsOrders.OrderProcesses, order.OrderID);
                    }

                    using (var taProcessAlias = new Data.Datasets.OrdersDataSetTableAdapters.ProcessAliasSummaryTableAdapter())
                    {
                        taProcessAlias.FillByOrder(dsOrders.ProcessAliasSummary, order.OrderID);
                    }

                    var processes = new List<OrderProcessInfo>();

                    foreach (var orderProcessRow in dsOrders.OrderProcesses)
                    {
                        var process = new OrderProcessInfo()
                        {
                            MaterialCost = orderProcessRow.IsMaterialCostNull() ?
                                0M :
                                orderProcessRow.MaterialCost,

                            ProcessBurdenCost = orderProcessRow.IsBurdenCostNull() ?
                                0M :
                                orderProcessRow.BurdenCost,

                            ProcessName = orderProcessRow.ProcessAliasSummaryRow.IsProcessNameNull() ?
                                orderProcessRow.ProcessID.ToString() :
                                orderProcessRow.ProcessAliasSummaryRow.ProcessName,

                            StepOrder = orderProcessRow.StepOrder
                        };

                        // Employees
                        var employees = new List<OrderProcessEmployeeInfo>();

                        var laborForProcess = order.GetLaborSummaryRows()
                            .Where(i => i.OrderProcessesID == orderProcessRow.OrderProcessesID);

                        foreach (var laborSummaryGroup in laborForProcess.GroupBy(row => row.UserID))
                        {
                            var user = dsSecurity.Users.FindByUserID(laborSummaryGroup.Key);

                            if (user == null)
                            {
                                continue;
                            }

                            var salaryTotal = 0M;
                            var burdenTotal = 0M;
                            double minutesTotal = 0d;

                            foreach (var laborSummary in laborSummaryGroup)
                            {
                                var startTime = laborSummary.StartTime;

                                var endTime = laborSummary.IsEndTimeNull() ?
                                    defaultEndTime :
                                    laborSummary.EndTime;

                                var salaryLaborTotal = 0M;
                                var burdenLaborTotal = 0M;

                                foreach (var summaryMinutesPerDate in DateUtilities.MinutesGroupedByDay(startTime, endTime))
                                {
                                    var summaryDate = summaryMinutesPerDate.Key;
                                    var summaryMinutes = summaryMinutesPerDate.Value;

                                    var salaryData = user.GetUserSalaryRows()
                                        .Where(salary => salary.EffectiveDate <= summaryDate)
                                        .OrderByDescending(salary => salary.EffectiveDate)
                                        .FirstOrDefault();

                                    if (salaryData != null)
                                    {
                                        salaryLaborTotal += summaryMinutes * (salaryData.Salary / DateUtilities.MINUTES_PER_HOUR);
                                        burdenLaborTotal += summaryMinutes * (salaryData.Burden / DateUtilities.MINUTES_PER_HOUR);
                                    }
                                }

                                var laborMinutes = (endTime - startTime).TotalMinutes; // non-rounded

                                if (!laborSummary.IsBatchProcessIDNull() && (salaryLaborTotal != 0M || burdenLaborTotal != 0M))
                                {
                                    // Split price
                                    var totalQty = taLaborSummary.GetTotalBatchQuantity(laborSummary.BatchProcessID);
                                    var qtyInBatch = taLaborSummary.GetQuantityInBatch(laborSummary.OrderID, laborSummary.BatchProcessID);

                                    var fractionToCount = Convert.ToDecimal(qtyInBatch) / Convert.ToDecimal(totalQty);

                                    salaryLaborTotal = salaryLaborTotal * fractionToCount;
                                    burdenLaborTotal = burdenLaborTotal * fractionToCount;
                                    laborMinutes = laborMinutes * Convert.ToDouble(fractionToCount);
                                }

                                salaryTotal += salaryLaborTotal;
                                burdenTotal += burdenLaborTotal;

                                minutesTotal += laborMinutes;
                            }

                            employees.Add(new OrderProcessEmployeeInfo()
                            {
                                UserId = user.UserID,
                                LaborBurdenCost = burdenTotal,
                                Initials = user.Name.ToInitials(StringInitialOption.AllInitials),
                                LaborWageCost = salaryTotal,
                                Minutes = minutesTotal 
                            });
                        }

                        process.Employees = employees;
                        processes.Add(process);
                    }

                    orderInfo.Processes = processes;

                    return orderInfo;
                }
                finally
                {
                    dsOrders?.Dispose();
                    dsSecurity?.Dispose();
                    taLaborSummary?.Dispose();
                }
            }

            #endregion
        }

        #endregion

        #region OrderProcessInfo

        private sealed class OrderProcessInfo
        {
            #region Properties

            public string ProcessName
            {
                get;
                set;
            }

            public int StepOrder
            {
                get;
                set;
            }

            public decimal MaterialCost
            {
                get;
                set;
            }

            public decimal ProcessBurdenCost
            {
                get;
                set;
            }

            public List<OrderProcessEmployeeInfo> Employees
            {
                get;
                set;
            }

            #endregion
        }

        #endregion

        #region EmployeeInfo

        private sealed class OrderProcessEmployeeInfo
        {
            #region Properties

            public int UserId
            {
                get;
                set;
            }

            public string Initials
            {
                get;
                set;
            }

            public double Minutes
            {
                get;
                set;
            }

            public decimal LaborWageCost
            {
                get;
                set;
            }

            public decimal LaborBurdenCost
            {
                get;
                set;
            }

            #endregion
        }

        #endregion
    }
}