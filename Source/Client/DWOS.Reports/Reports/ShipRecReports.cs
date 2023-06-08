using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Datasets.OrderShipmentDataSetTableAdapters;
using DWOS.Data.Reports;
using DWOS.Data.Reports.ProcessPartsReportTableAdapters;
using DWOS.Data.Reports.ShipmentReportTableAdapters;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using OrderTableAdapter = DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter;
using DWOS.Reports.ReportData;
using DWOS.Shared.Utilities;
using System.Threading;

namespace DWOS.Reports
{
    /// <summary>
    /// The packing slip for shipping.
    /// </summary>
    public class PackingListReport : ExcelBaseReport
    {
        #region Fields

        private const string NOT_AVAILABLE = "NA";

        private readonly OrderShipmentDataSet.OrderShipmentRow[] _orderShipments;
        private readonly OrderShipmentDataSet.ShipmentPackageRow _shipment;
        private readonly Color AccentBackgroundColor = Color.FromArgb(54, 96, 146);

        private const string WEIGHT_FIELD_NAME = "Weight";
        private const string FIELD_CATEGORY_PARTS = "Part";
        private const string WEIGHT_CELL_FORMAT = "0.00";
        private const string WEIGHT_CELL_LBS_FORMAT = "0.00 \"lbs.\"";

        #endregion

        #region Properties

        public override string Title =>
            "PACKING SLIP";

        public int HeaderRow { get; set; }

        protected override PageOrientation ReportPageOrientation =>
            PageOrientation.Portrait;

        private bool IsWeightEnabled { get; set; }

        private int LastColumnIndex { get; set; }

        #endregion

        #region Methods

        public PackingListReport(OrderShipmentDataSet.ShipmentPackageRow shipment,
            OrderShipmentDataSet.OrderShipmentRow[] orderShipments)
        {
            _orderShipments = orderShipments ?? throw new ArgumentNullException(nameof(orderShipments));
            _shipment = shipment ?? throw new ArgumentNullException(nameof(shipment));
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            // Initialize report
            SetupReportInfo();
            _section.PageMargins.All = 20;

            var worksheet = CreateWorksheet(Title);
            worksheet.PrintOptions.Orientation = Orientation.Portrait;

            // Initialize IsWeightEnabled setting
            var fields = new ApplicationSettingsDataSet.FieldsDataTable();
            using (var ta = new FieldsTableAdapter())
                ta.FillByCategory(fields, FIELD_CATEGORY_PARTS);

            var weightField = fields.FirstOrDefault(f => f.Name == WEIGHT_FIELD_NAME);
            if (weightField != null && weightField.IsVisible)
                IsWeightEnabled = true;

            //Get the fields to use in this report and customer, add fields based on settings from ReportManager
            List<ReportToken> reportTokens;

            using (var dtCustomer = new CustomersDataset.CustomerDataTable())
            {
                var firstOrder = _orderShipments.FirstOrDefault();

                using (var taCustomer = new Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter())
                {
                    taCustomer.FillByOrderID(dtCustomer, firstOrder.OrderID);
                }

                var customer = dtCustomer.FirstOrDefault();

                reportTokens = customer != null
                    ? ReportFieldMapper.GetFieldsByReport(ReportFieldMapper.enumReportType.PackingSlip, customer.CustomerID)
                    : ReportFieldMapper.GetFieldsByReport(ReportFieldMapper.enumReportType.PackingSlip);
            }

            // Create report
            LastColumnIndex = Math.Max(reportTokens.Count() - 1, 0);
            var rowIndex = AddHeaderRows(worksheet) + 1;
            rowIndex = AddShipmentInfoRows(worksheet, rowIndex) + 1;

            rowIndex = AddOrderHeaderRow(worksheet, rowIndex, reportTokens) + 1;

            if (this._orderShipments.Length > 0)
            {
                rowIndex = AddOrders(worksheet, rowIndex, reportTokens) + 1;
                rowIndex = AddSignatureRow(worksheet, rowIndex) + 1;
            }

            AddAddressRow(worksheet, rowIndex);

            FitAllColumnsOnSinglePage(worksheet);
        }

        private int AddHeaderRows(Worksheet worksheet)
        {
            // Minimum column index to use for merged cell regions
            const int minimumLastColumnIndex = 5;

            const int titleFontHeight = 25 * MediaUtilities.TWIPS_PER_POINT;

            var rowIndex = 0;

            worksheet.Columns[0].SetWidth(15, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[1].SetWidth(15, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[2].SetWidth(15, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[3].SetWidth(15, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[4].SetWidth(15, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[5].SetWidth(15, WorksheetColumnWidthUnit.Character);

            worksheet.Columns[LastColumnIndex].SetWidth(15, WorksheetColumnWidthUnit.Character);

            //Create the merged region that will be a header to the column headers
            var titleRegion = worksheet.MergedCellsRegions.Add(rowIndex, 0, rowIndex, Math.Max(LastColumnIndex, minimumLastColumnIndex));
            titleRegion.Value = "Packing Slip";
            titleRegion.CellFormat.Font.Name = "Verdana";
            titleRegion.CellFormat.Font.Height = titleFontHeight;
            titleRegion.CellFormat.Alignment = HorizontalCellAlignment.Right;
            titleRegion.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            rowIndex++;

            //Company Logo or Name
            var addCompanyLogo = ApplicationSettings.Current.ShowCompanyLogoOnPackingSlip &&
                !string.IsNullOrEmpty(ApplicationSettings.Current.CompanyLogoImagePath);

            if (addCompanyLogo)
            {
                const int maxCompanyLogoWidth = 260;
                const int maxCompanyLogoHeight = 80;

                Image companyLogo;

                // Manually resize image - SetBoundsInTwips has problems
                // with keeping the aspect ratio
                using (var originalCompanyLogo = new Bitmap(ApplicationSettings.Current.CompanyLogoImagePath))
                {
                    var newSize = MediaUtilities.Resize(originalCompanyLogo.Size, new Size(maxCompanyLogoWidth, maxCompanyLogoHeight));
                    companyLogo = new Bitmap(newSize.Width, newSize.Height);

                    using (var gfx = Graphics.FromImage(companyLogo))
                    {
                        gfx.DrawImage(originalCompanyLogo, 0, 0, newSize.Width, newSize.Height);
                    }
                }

                var dpi = companyLogo.HorizontalResolution;
                var imageShape = new WorksheetImage(companyLogo);

                var boundsInTwips = new Rectangle(0,
                    0,
                    MediaUtilities.PixelsToTwips(companyLogo.Width, dpi),
                    MediaUtilities.PixelsToTwips(companyLogo.Height, dpi));

                imageShape.SetBoundsInTwips(worksheet, boundsInTwips);

                imageShape.Fill = new ShapeFillSolid(Color.Transparent);
                imageShape.Outline = new ShapeOutlineSolid(Color.Transparent);
                worksheet.Shapes.Add(imageShape);

                const int logoRowHeightOffsetTwips = 100; // determined through trial & error

                var calculatedRowHeight = MediaUtilities.PixelsToTwips(companyLogo.Height, dpi) -
                                          titleFontHeight - logoRowHeightOffsetTwips;

                var minRowHeight = MediaUtilities.PixelsToTwips(20, dpi);

                worksheet.Rows[rowIndex].Height = Math.Max(minRowHeight, calculatedRowHeight);
            }

            var companyRegion = worksheet.MergedCellsRegions.Add(rowIndex, 0, rowIndex, Math.Max(LastColumnIndex, 5));
            companyRegion.Value = ApplicationSettings.Current.CompanyName;
            companyRegion.CellFormat.Font.Name = "Verdana";
            companyRegion.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            companyRegion.CellFormat.Font.Height = 9 * MediaUtilities.TWIPS_PER_POINT;
            companyRegion.CellFormat.Alignment = HorizontalCellAlignment.Right;
            companyRegion.CellFormat.VerticalAlignment = VerticalCellAlignment.Top;

            rowIndex++;

            //Company Slogan
            var sloganRegion = worksheet.MergedCellsRegions.Add(rowIndex, 0, rowIndex + 1, Math.Max(LastColumnIndex, 5));
            sloganRegion.CellFormat.WrapText = ExcelDefaultableBoolean.True;

            var slogan = ApplicationSettings.Current.CompanyTagline?.Remove("</br>");

            sloganRegion.Value = slogan;
            sloganRegion.CellFormat.Font.Name = "Verdana";
            sloganRegion.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            sloganRegion.CellFormat.Font.Height = 12 * 20;
            sloganRegion.CellFormat.Font.Italic = ExcelDefaultableBoolean.True;
            sloganRegion.CellFormat.Alignment = HorizontalCellAlignment.Left;
            sloganRegion.CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
            sloganRegion.CellFormat.Indent = 1;
            
            rowIndex++;

            return rowIndex;
        }

        private int AddShipmentInfoRows(Worksheet worksheet, int rowIndex)
        {
            var firstOrder = _orderShipments.FirstOrDefault();

            int maxPackageNumber;
            ShipmentInfo shipmentInfo;

            using (var dsCustomer = new CustomersDataset { EnforceConstraints = false })
            {
                using (var taCustomer = new Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter())
                {
                    taCustomer.FillByOrderID(dsCustomer.Customer, firstOrder.OrderID);
                }

                if (!firstOrder.IsCustomerAddressIDNull())
                {
                    using (var taCustomerAddress = new Data.Datasets.CustomersDatasetTableAdapters.CustomerAddressTableAdapter())
                    {
                        taCustomerAddress.FillByCustomerAddress(dsCustomer.CustomerAddress, firstOrder.CustomerAddressID);
                    }
                }

                using (var taCountry = new Data.Datasets.CustomersDatasetTableAdapters.CountryTableAdapter())
                {
                    taCountry.Fill(dsCustomer.Country);
                }

                using (var taShipmentPackage = new Data.Datasets.OrderShipmentDataSetTableAdapters.ShipmentPackageTableAdapter())
                {
                    if (_shipment.Active || _shipment.CloseDate.Date == DateTime.Today)
                    {
                        maxPackageNumber =
                            taShipmentPackage.GetTodayMaxPackageNumber(_shipment.CustomerID, _shipment.ShipmentPackageTypeID) ??
                            _shipment.PackageNumber;
                    }
                    else
                    {
                        maxPackageNumber =
                            taShipmentPackage.GetClosedMaxPackageNumber(_shipment.CloseDate, _shipment.CustomerID,
                                _shipment.ShipmentPackageTypeID) ??
                            _shipment.PackageNumber;
                    }
                }

                var customer = dsCustomer.Customer.FirstOrDefault();
                shipmentInfo = GenerateShipmentInfo(customer, firstOrder);
            }

            var cityStateZipLine = "{0}, {1} {2}".FormatWith(
                    shipmentInfo.City,
                    shipmentInfo.State,
                    shipmentInfo.Zip);

            if (shipmentInfo.CountryId != ApplicationSettings.Current.CompanyCountry)
            {
                cityStateZipLine += $" {shipmentInfo.CountryName}";
            }

            var packageNumberLine = $"{_shipment.PackageNumber} of {maxPackageNumber}";

            //Date Shipped
            var dateShippedCell = worksheet.Rows[rowIndex].Cells[0];
            dateShippedCell.Value = "Date Shipped:";
            ApplyShipmentInfoLabelFormat(dateShippedCell.CellFormat);

            //Date Value
            var dateShippedValueCell = worksheet.MergedCellsRegions.Add(rowIndex, 1, rowIndex, 2);
            dateShippedValueCell.Value = firstOrder.DateShipped.ToShortDateString();
            ApplyShipmentInfoValueFormat(dateShippedValueCell.CellFormat);
            dateShippedValueCell.CellFormat.FormatString = DATE_FORMAT;


            var shipToCell = worksheet.Rows[rowIndex].Cells[3];
            shipToCell.Value = "Ship To:";
            ApplyShipmentInfoLabelFormat(shipToCell.CellFormat);

            var shipToValueCell = worksheet.MergedCellsRegions.Add(rowIndex, 4, rowIndex, 5);
            shipToValueCell.Value = shipmentInfo.Name;
            ApplyShipmentInfoValueFormat(shipToValueCell.CellFormat);

            rowIndex++;

            //Carrier
            var carrierCell = worksheet.Rows[rowIndex].Cells[0];
            carrierCell.Value = "Carrier:";
            ApplyShipmentInfoLabelFormat(carrierCell.CellFormat);

            var carrierValueCell = worksheet.MergedCellsRegions.Add(rowIndex, 1, rowIndex, 2);
            carrierValueCell.Value = shipmentInfo.ShipmentCarrier;
            ApplyShipmentInfoValueFormat(carrierValueCell.CellFormat);

            //Address 1
            if (shipmentInfo.HasAddress)
            {
                var address1ValueCell = worksheet.MergedCellsRegions.Add(rowIndex, 4, rowIndex, 5);
                address1ValueCell.Value = shipmentInfo.Address1;
                ApplyShipmentInfoValueFormat(address1ValueCell.CellFormat);
            }

            rowIndex++;

            //Tracking Number
            var trackingCell = worksheet.Rows[rowIndex].Cells[0];
            trackingCell.Value = "Tracking:";
            ApplyShipmentInfoLabelFormat(trackingCell.CellFormat);

            var trackingValueCell = worksheet.MergedCellsRegions.Add(rowIndex, 1, rowIndex, 2);
            trackingValueCell.Value = shipmentInfo.TrackingNumber;
            ApplyShipmentInfoValueFormat(trackingValueCell.CellFormat);

            //Address 2 OR CityStateZip
            if (shipmentInfo.HasAddress)
            {
                var address2ValueCell = worksheet.MergedCellsRegions.Add(rowIndex, 4, rowIndex, 5);
                address2ValueCell.Value = string.IsNullOrEmpty(shipmentInfo.Address2) ? cityStateZipLine : shipmentInfo.Address2;
                ApplyShipmentInfoValueFormat(address2ValueCell.CellFormat);
            }

            rowIndex++;

            //Shipped By
            var shipByCell = worksheet.Rows[rowIndex].Cells[0];
            shipByCell.Value = "Shipped By:";
            ApplyShipmentInfoLabelFormat(shipByCell.CellFormat);

            var shipByValueCell = worksheet.MergedCellsRegions.Add(rowIndex, 1, rowIndex, 2);
            shipByValueCell.Value = shipmentInfo.UserName;
            ApplyShipmentInfoValueFormat(shipByValueCell.CellFormat);

            //CityStateZip
            if (shipmentInfo.HasAddress)
            {
                var address3ValueCell = worksheet.MergedCellsRegions.Add(rowIndex, 4, rowIndex, 5);
                address3ValueCell.Value = string.IsNullOrEmpty(shipmentInfo.Address2) ? string.Empty : cityStateZipLine;
                ApplyShipmentInfoValueFormat(address3ValueCell.CellFormat);
            }

            rowIndex++;

            // Package ID
            var packageCell = worksheet.Rows[rowIndex].Cells[0];
            packageCell.Value = "Package #:";
            ApplyShipmentInfoLabelFormat(packageCell.CellFormat);

            var packageValueCell = worksheet.MergedCellsRegions.Add(rowIndex, 1, rowIndex, 2);
            packageValueCell.Value = shipmentInfo.ShipmentPackageId;
            ApplyShipmentInfoValueFormat(packageValueCell.CellFormat);

            // Package Number
            string packageTypeName;
            using (var taShipmentPackageType = new Data.Datasets.OrderShipmentDataSetTableAdapters.ShipmentPackageTypeTableAdapter())
            {
                packageTypeName = taShipmentPackageType.GetDataById(_shipment.ShipmentPackageTypeID).FirstOrDefault()?.Name;
            }

            var packageNumberCell = worksheet.Rows[rowIndex].Cells[3];
            packageNumberCell.Value = $"{packageTypeName} #:";
            ApplyShipmentInfoLabelFormat(packageNumberCell.CellFormat);

            var packageNumberValueCell = worksheet.MergedCellsRegions.Add(rowIndex, 4, rowIndex, 5);
            packageNumberValueCell.Value = packageNumberLine;
            ApplyShipmentInfoValueFormat(packageNumberValueCell.CellFormat);

            rowIndex++;

            //Blank Line
            var blankLine = worksheet.MergedCellsRegions.Add(rowIndex, 0, rowIndex, 5);
            blankLine.CellFormat.Font.Height = 16 * 20;

            return rowIndex;
        }

        private static void ApplyShipmentInfoValueFormat(IWorksheetCellFormat cellFormat)
        {
            if (cellFormat == null)
            {
                throw new ArgumentNullException(nameof(cellFormat));
            }

            cellFormat.Font.Name = "Verdana";
            cellFormat.Font.Height = 8 * 20;
            cellFormat.Alignment = HorizontalCellAlignment.Left;
            cellFormat.VerticalAlignment = VerticalCellAlignment.Center;
        }

        private static void ApplyShipmentInfoLabelFormat(IWorksheetCellFormat cellFormat)
        {
            if (cellFormat == null)
            {
                throw new ArgumentNullException(nameof(cellFormat));
            }

            cellFormat.Font.Name = "Verdana";
            cellFormat.Font.Height = 8 * 20;
            cellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            cellFormat.Alignment = HorizontalCellAlignment.Left;
            cellFormat.VerticalAlignment = VerticalCellAlignment.Center;
        }

        private ShipmentInfo GenerateShipmentInfo(CustomersDataset.CustomerRow customer, OrderShipmentDataSet.OrderShipmentRow firstOrder)
        {
            if (customer == null || firstOrder == null)
            {
                return null;
            }

            string userName = null;
            if (firstOrder.UsersRow == null)
            {
                using (var taUsers = new UsersTableAdapter())
                {
                    userName = taUsers.GetUserName(firstOrder.ShippingUserID);
                }
            }

            CustomersDataset.CustomerAddressRow shippingAddress = null;
            if (!firstOrder.IsCustomerAddressIDNull())
            {
                shippingAddress = customer.GetCustomerAddressRows()
                    .FirstOrDefault(addr => addr.IsValidState() && addr.CustomerAddressID == firstOrder.CustomerAddressID);
            }

            return  shippingAddress == null
                ? ShipmentInfoFromCustomer(customer, firstOrder, userName)
                : ShipmentInfoFromAddress(shippingAddress, firstOrder, userName);
        }

        private static ShipmentInfo ShipmentInfoFromCustomer(CustomersDataset.CustomerRow customer,
            OrderShipmentDataSet.OrderShipmentRow firstOrder, string userName)
        {
            if (customer == null || firstOrder == null)
            {
                return null;
            }

            return new ShipmentInfo()
            {
                ShipmentPackageId = firstOrder.ShipmentPackageID,
                Name = customer.Name,
                Address1 = customer.IsAddress1Null() ? string.Empty : customer.Address1,
                Address2 = customer.IsAddress2Null() ? string.Empty : customer.Address2,
                City = customer.IsCityNull() ? string.Empty : customer.City,
                State = customer.IsStateNull() ? string.Empty : customer.State,
                Zip = customer.IsZipNull() ? string.Empty : customer.Zip,
                CountryId = customer.CountryID,
                CountryName = customer.CountryRow.Name,

                ShipmentCarrier = string.IsNullOrEmpty(firstOrder.ShippingCarrierID) ? NOT_AVAILABLE : firstOrder.ShippingCarrierID,
                TrackingNumber = firstOrder.IsTrackingNumberNull() ? NOT_AVAILABLE : firstOrder.TrackingNumber,
                UserName = userName
            };
        }

        private ShipmentInfo ShipmentInfoFromAddress(CustomersDataset.CustomerAddressRow shippingAddress,
            OrderShipmentDataSet.OrderShipmentRow firstOrder, string userName)
        {
            if (shippingAddress == null || firstOrder == null)
            {
                return null;
            }

            return new ShipmentInfo()
            {
                ShipmentPackageId = firstOrder.ShipmentPackageID,
                Name = shippingAddress.Name,
                Address1 = shippingAddress.IsAddress1Null() ? string.Empty : shippingAddress.Address1,
                Address2 = shippingAddress.IsAddress2Null() ? string.Empty : shippingAddress.Address2,
                City = shippingAddress.IsCityNull() ? string.Empty : shippingAddress.City,
                State = shippingAddress.IsStateNull() ? string.Empty : shippingAddress.State,
                Zip = shippingAddress.IsZipNull() ? string.Empty : shippingAddress.Zip,
                CountryId = shippingAddress.CountryID,
                CountryName = shippingAddress.CountryRow.Name,

                ShipmentCarrier = string.IsNullOrEmpty(firstOrder.ShippingCarrierID) ? NOT_AVAILABLE : firstOrder.ShippingCarrierID,
                TrackingNumber = firstOrder.IsTrackingNumberNull() ? NOT_AVAILABLE : firstOrder.TrackingNumber,
                UserName = userName
            };
        }

        private int AddOrderHeaderRow(Worksheet worksheet, int rowIndex, List<ReportToken> tokens)
        {
            WorksheetCell cell = null;
            int colIndex = 0;
            bool isFirstCell = false;
            HeaderRow = rowIndex;

            tokens = tokens.OrderBy(d => d.DisplayOrder).ToList();

            //Create header cell based on report field token
            foreach (var token in tokens)
            {
                isFirstCell = true;
                cell = CreateOrderHeaderCell(worksheet, rowIndex, colIndex++, token.DisplayName, token.Width);
                cell.CellFormat.WrapText = ExcelDefaultableBoolean.True;

                if (isFirstCell)
                {
                    cell.CellFormat.LeftBorderColorInfo = new WorkbookColorInfo(Color.White);
                    isFirstCell = false;
                }
            }

            //Last cell
            if (cell != null)
            {
                cell.CellFormat.RightBorderColorInfo = new WorkbookColorInfo(Color.Black);
            }

            return rowIndex;
        }

        private int AddOrders(Worksheet worksheet, int rowIndex, List<ReportToken> tokens)
        {
            const string notAvailable = "NA";

            int colIndex = 0;

            //Add Rows
            var taParts             = new ShipmentPartTableAdapter();
            var taOrderCustomFields = new OrderCustomFieldsTableAdapter();
            var taOrderContainers   = new OrderContainersTableAdapter();
            var taOrders            = new OrderTableAdapter();
            var taOrderSerialNumber = new Data.Datasets.OrdersDataSetTableAdapters.OrderSerialNumberTableAdapter();

            var firstDataRow    = rowIndex;

            var presentFields = new HashSet<ReportFieldMapper.enumReportTokens>();

            foreach(var shipment in _orderShipments)
            {
                var isAlternateRow = (rowIndex - firstDataRow) % 2 != 0;

                var parts           = taParts.GetData(shipment.OrderID);

                var dtOrderCustomFields  = new OrdersDataSet.OrderCustomFieldsDataTable();
                var dsContainers    = new OrdersDataSet.OrderContainersDataTable();
                var dtContainerItem = new OrdersDataSet.OrderContainerItemDataTable();
                var dtPackageType = new OrdersDataSet.ShipmentPackageTypeDataTable();
                var dsOrders        = new OrdersDataSet.OrderDataTable();
                var dsOrderSerialNumber = new OrdersDataSet.OrderSerialNumberDataTable();
                var dtRework = new OrdersDataSet.InternalReworkDataTable();
                var dtReworkReason = new ListsDataSet.d_ReworkReasonDataTable();

                var dtCustomFields = new OrdersDataSet.CustomFieldDataTable();

                taOrderCustomFields.FillByOrder(dtOrderCustomFields, shipment.OrderID);
                taOrderContainers.FillByOrder(dsContainers, shipment.OrderID);
                taOrders.FillByOrderID(dsOrders, shipment.OrderID);
                taOrderSerialNumber.FillByOrder(dsOrderSerialNumber, shipment.OrderID);

                using (var taCustomFields = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.CustomFieldTableAdapter())
                {
                    taCustomFields.Fill(dtCustomFields);
                }

                using (var taInternalRework = new InternalReworkTableAdapter { ClearBeforeFill = false })
                {
                    taInternalRework.FillByOriginalOrderID(dtRework, shipment.OrderID);
                    taInternalRework.FillByReworkOrderID(dtRework, shipment.OrderID);
                }

                using (var taReworkReason = new Data.Datasets.ListsDataSetTableAdapters.d_ReworkReasonTableAdapter())
                {
                    taReworkReason.Fill(dtReworkReason);
                }

                using (var taContainerItem = new Data.Datasets.OrdersDataSetTableAdapters.OrderContainerItemTableAdapter())
                {
                    taContainerItem.FillByOrder(dtContainerItem, shipment.OrderID);
                }

                using (var taPackageType = new Data.Datasets.OrdersDataSetTableAdapters.ShipmentPackageTypeTableAdapter())
                {
                    taPackageType.Fill(dtPackageType);
                }

                var order = dsOrders.FirstOrDefault();

                //Add field values based on report field token
                foreach (ReportToken token in tokens)
                {
                    ReportFieldMapper.enumReportTokens field;

                    if (Enum.TryParse(token.FieldName, out field))
                    {
                        presentFields.Add(field);

                        switch (field)
                        {
                            case ReportFieldMapper.enumReportTokens.OrderID:
                                worksheet.Rows[rowIndex].Cells[colIndex].Value = shipment.OrderID;
                                break;
                            case ReportFieldMapper.enumReportTokens.CustomerWO:
                                worksheet.Rows[rowIndex].Cells[colIndex].Value = order == null || order.IsCustomerWONull() ? "<None>" : order.CustomerWO;
                                break;
                            case ReportFieldMapper.enumReportTokens.PurchaseOrder:
                                worksheet.Rows[rowIndex].Cells[colIndex].Value = order == null || order.IsPurchaseOrderNull() ? "<None>" : order.PurchaseOrder;
                                break;
                            case ReportFieldMapper.enumReportTokens.PartID:
                                worksheet.Rows[rowIndex].Cells[colIndex].Value = GetPartText(order, parts[0], dtRework, dtReworkReason);
                                break;
                            case ReportFieldMapper.enumReportTokens.PartQuantity:
                                worksheet.Rows[rowIndex].Cells[colIndex].Value = shipment.PartQuantity;
                                break;
                            case ReportFieldMapper.enumReportTokens.Weight:
                                worksheet.Rows[rowIndex].Cells[colIndex].Value = parts[0] == null || parts[0].IsWeightNull() ? 0 : parts[0].Weight;
                                break;
                            case ReportFieldMapper.enumReportTokens.SalesOrder:
                                worksheet.Rows[rowIndex].Cells[colIndex].Value = order == null || order.IsSalesOrderIDNull() ? "<None>" : order.SalesOrderID.ToString();
                                break;
                            case ReportFieldMapper.enumReportTokens.OrderPrice:
                                worksheet.Rows[rowIndex].Cells[colIndex].Value = GetPrice(order).ToString(OrderPrice.CurrencyFormatString);
                                break;
                            case ReportFieldMapper.enumReportTokens.ContainerCount:
                                worksheet.Rows[rowIndex].Cells[colIndex].Value = dsContainers.Count;
                                break;
                            case ReportFieldMapper.enumReportTokens.ContainerDescription:
                                worksheet.Rows[rowIndex].Cells[colIndex].Value = GetContainerDisplayText(dsContainers, dtContainerItem, dtPackageType);
                                break;
                            case ReportFieldMapper.enumReportTokens.GrossWeight:
                                worksheet.Rows[rowIndex].Cells[colIndex].Value = Data.Order.OrderUtilities.CalculateGrossWeight(order);
                                worksheet.Rows[rowIndex].Cells[colIndex].CellFormat.FormatString = WEIGHT_CELL_FORMAT;
                                break;
                            case ReportFieldMapper.enumReportTokens.PartDescription:
                                string partDescription;

                                if (parts[0] == null || parts[0].IsDescriptionNull())
                                {
                                    partDescription = notAvailable;
                                }
                                else
                                {
                                    partDescription = parts[0].Description;
                                }

                                worksheet.Rows[rowIndex].Cells[colIndex].Value = partDescription;
                                break;
                            case ReportFieldMapper.enumReportTokens.ProcessAlias:
                                worksheet.Rows[rowIndex].Cells[colIndex].Value = GetProcessText(order);
                                break;
                            case ReportFieldMapper.enumReportTokens.SerialNumber:
                                var serialNumberValue = "<None>";

                                var serialNumbers = dsOrderSerialNumber.Where(s => s.OrderID == shipment.OrderID && s.Active).ToList();

                                if (serialNumbers.Count > 0)
                                {
                                    serialNumberValue = string.Join(", ",
                                        serialNumbers
                                            .Select(s => s.Number));
                                }
                                worksheet.Rows[rowIndex].Cells[colIndex].Value = serialNumberValue;
                                break;
                            case ReportFieldMapper.enumReportTokens.OrderWeight:
                                worksheet.Rows[rowIndex].Cells[colIndex].Value = Data.Order.OrderUtilities.GetOrderWeight(order);
                                worksheet.Rows[rowIndex].Cells[colIndex].CellFormat.FormatString = WEIGHT_CELL_FORMAT;
                                break;
                            default:
                                //Custom field identifiers are numeric
                                int customFieldID = 0;
                                if (int.TryParse(token.FieldName, out customFieldID))
                                {
                                    var customField = dtCustomFields.FirstOrDefault(f => f.CustomFieldID == customFieldID);

                                    if (customField != null && !order.IsCustomerIDNull() && customField.CustomerID != order.CustomerID)
                                    {
                                        // Custom field is for the wrong customer.
                                        _log.Debug("Report includes a custom field that does not belong to customer ID = {0}", order.CustomerID);
                                        _log.Debug("Attempting to replace custom field");

                                        string customFieldName = customField.Name;
                                        var secondCustomFieldMatch = dtCustomFields.FirstOrDefault(f => f.CustomerID == order.CustomerID && f.Name == customFieldName);
                                        customFieldID = secondCustomFieldMatch == null ? customFieldID : secondCustomFieldMatch.CustomFieldID;
                                    }
                                    var orderCustomField = dtOrderCustomFields.FirstOrDefault(row => row.CustomFieldID == customFieldID);
                                    worksheet.Rows[rowIndex].Cells[colIndex].Value = orderCustomField != null && !orderCustomField.IsValueNull()
                                        ? orderCustomField.Value
                                        : string.Empty;
                                }

                                break;
                        }
                    }
                    else if (token.IsCustom)
                    {
                        // Custom token
                        var customFieldId = dtCustomFields.FirstOrDefault(f => f.Name == token.FieldName)?.CustomFieldID ?? 0;
                        var orderCustomField = dtOrderCustomFields.FirstOrDefault(row => row.CustomFieldID == customFieldId);
                        worksheet.Rows[rowIndex].Cells[colIndex].Value = orderCustomField != null && !orderCustomField.IsValueNull()
                            ? orderCustomField.Value
                            : string.Empty;
                    }
                    else
                    {
                        _log.Error("A report token had an unexpected value for FieldName\n\tDisplayName: {0}\n\tFieldName: {1}\n\tCustomerID: {2}",
                            token.DisplayName,
                            token.FieldName,
                            order.IsCustomerIDNull() ? -1 : order.CustomerID);
                    }

                    var cellFormat = worksheet.Rows[rowIndex].Cells[colIndex].CellFormat;
                    cellFormat.Font.Name = "Verdana";
                    cellFormat.Font.Height = 8 * 20;

                    cellFormat.Alignment = field == ReportFieldMapper.enumReportTokens.ProcessAlias ?
                        HorizontalCellAlignment.Left :
                        HorizontalCellAlignment.Center;

                    cellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    cellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
                    cellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
                    cellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
                    cellFormat.WrapText = ExcelDefaultableBoolean.True;

                    if (isAlternateRow)
                    {
                        cellFormat.Fill = CellFill.CreateSolidFill(Color.LightGray);
                    }

                    colIndex++;
                }

                rowIndex++;
                colIndex = 0;
            }

            var lastValueIndex = rowIndex;

            //Totals Header
            rowIndex++;
            var totalHeaderCell = CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 1, "Totals:");
            totalHeaderCell.Value = "Totals:";
            totalHeaderCell.CellFormat.Font.Name = "Verdana";
            totalHeaderCell.CellFormat.Font.Height = 8 * 20;
            totalHeaderCell.CellFormat.Font.ColorInfo = new WorkbookColorInfo(Color.White);
            totalHeaderCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            totalHeaderCell.CellFormat.Alignment = HorizontalCellAlignment.Center;
            totalHeaderCell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            totalHeaderCell.CellFormat.Fill = CellFill.CreateSolidFill(AccentBackgroundColor);

            //Add Totals

            //Add Total Orders
            rowIndex++;
            var totalOrdersCell = worksheet.Rows[rowIndex].Cells[0];
            totalOrdersCell.Value = "Orders:";
            FormatTotalHeaderCell(totalOrdersCell);
            var ordersSumCell = worksheet.Rows[rowIndex].Cells[1];
            Formula.Parse("=COUNTA(R{0}C{1}:R{2}C{3})".FormatWith(firstDataRow + 1, 2, lastValueIndex, 2), CellReferenceMode.R1C1).ApplyTo(ordersSumCell);
            FormatSumCell(ordersSumCell);

            //Add Total Weight
            if (IsWeightEnabled && presentFields.Contains(ReportFieldMapper.enumReportTokens.Weight))
            {
                rowIndex++;
                var totalWeightCell = worksheet.Rows[rowIndex].Cells[0];
                totalWeightCell.Value = "Weight:";
                FormatTotalHeaderCell(totalWeightCell);
                var weightSumCell = worksheet.Rows[rowIndex].Cells[1];
                FormatSumCell(weightSumCell);
                foreach (var cell in worksheet.Rows[HeaderRow].Cells)
                {
                    if ((string) cell.Value == "Weight (lbs.)")
                    {
                        int sumColumn = cell.ColumnIndex + 1;
                        int sumRowIndex = firstDataRow;
                        Formula.Parse("=SUM(R{0}C{1}:R{2}C{3})".FormatWith(sumRowIndex + 1, sumColumn, lastValueIndex, sumColumn), CellReferenceMode.R1C1).ApplyTo(weightSumCell);
                    }
                }
            }

            // Add Gross Weight
            if (presentFields.Contains(ReportFieldMapper.enumReportTokens.GrossWeight))
            {
                rowIndex++;
                var totalGrossWeightCell = worksheet.Rows[rowIndex].Cells[0];
                totalGrossWeightCell.Value = "Gross Weight:";
                FormatTotalHeaderCell(totalGrossWeightCell);

                var grossWeightSumCell = worksheet.Rows[rowIndex].Cells[1];
                FormatSumCell(grossWeightSumCell);

                if (_shipment.IsGrossWeightNull())
                {
                    var grossWeightHeader = ReportFieldMapper.GetDisplayName(ReportFieldMapper.enumReportTokens.GrossWeight);
                    foreach (var cell in worksheet.Rows[HeaderRow].Cells)
                    {
                        var headerText = cell.Value.ToString();

                        if (headerText == grossWeightHeader)
                        {
                            int sumColumn = cell.ColumnIndex + 1;
                            int sumRowIndex = firstDataRow;
                            Formula.Parse("=SUM(R{0}C{1}:R{2}C{3})".FormatWith(sumRowIndex + 1, sumColumn, lastValueIndex, sumColumn), CellReferenceMode.R1C1).ApplyTo(grossWeightSumCell);
                            grossWeightSumCell.CellFormat.FormatString = WEIGHT_CELL_LBS_FORMAT;
                        }
                    }
                }
                else
                {
                    grossWeightSumCell.Value = _shipment.GrossWeight;
                    grossWeightSumCell.CellFormat.FormatString =
                        $"0.{string.Concat(Enumerable.Repeat("0", ApplicationSettings.Current.WeightDecimalPlaces))}";
                }

            }

            //Add Total Part Quantity
            if (presentFields.Contains(ReportFieldMapper.enumReportTokens.PartQuantity))
            {
                rowIndex++;
                var totalQuantityCell = worksheet.Rows[rowIndex].Cells[0];
                totalQuantityCell.Value = "Parts:";
                FormatTotalHeaderCell(totalQuantityCell);
                var quantitySumCell = worksheet.Rows[rowIndex].Cells[1];
                FormatSumCell(quantitySumCell);
                foreach (var cell in worksheet.Rows[HeaderRow].Cells)
                {
                    if ((string)cell.Value == "Quantity")
                    {
                        int quantitySumColumn = cell.ColumnIndex + 1;
                        int quantitySumRowIndex = firstDataRow;

                        Formula.Parse("=SUM(R{0}C{1}:R{2}C{3})".FormatWith(quantitySumRowIndex + 1, quantitySumColumn, lastValueIndex, quantitySumColumn), CellReferenceMode.R1C1).ApplyTo(quantitySumCell);
                    }
                }
            }

            return rowIndex;
        }

        private static string GetContainerDisplayText(OrdersDataSet.OrderContainersDataTable dtContainers,
            OrdersDataSet.OrderContainerItemDataTable dtContainerItem, OrdersDataSet.ShipmentPackageTypeDataTable dtPackageType)
        {
            if (dtContainers == null || dtContainerItem == null ||  dtPackageType == null)
            {
                return string.Empty;
            }

            // Create a display string by grouping each item/container by its
            // type's name, creating a display string for each group, and
            // joining the display strings together using a utilty method.
            var itemStringParts = dtContainerItem
                .GroupBy(item => dtPackageType.FindByShipmentPackageTypeID(item.ShipmentPackageTypeID)?.Name ?? "N/A")
                .Select(group => new Tuple<string, long>(group.Key.ToLower(), group.LongCount()))
                .OrderBy(item => item.Item1)
                .Select(item => $"{item.Item2} {(item.Item2 != 1 ? StringUtilities.ToPlural(item.Item1) : item.Item1)}");

            var itemString = StringUtilities.ToDisplayText(itemStringParts);

            var containerStringParts = dtContainers
                .GroupBy(item => dtPackageType.FindByShipmentPackageTypeID(item.ShipmentPackageTypeID)?.Name ?? "N/A")
                .Select(group => new Tuple<string, long>(group.Key.ToLower(), group.LongCount()))
                .OrderBy(item => item.Item1)
                .Select(item => $"{item.Item2} {(item.Item2 != 1 ? StringUtilities.ToPlural(item.Item1) : item.Item1)}");

            var containerString = StringUtilities.ToDisplayText(containerStringParts);

            return string.IsNullOrEmpty(itemString)
                ? containerString
                : $"{itemString} in {containerString}";
        }

        private string GetPartText(OrdersDataSet.OrderRow order, OrderShipmentDataSet.ShipmentPartRow part,
            OrdersDataSet.InternalReworkDataTable dtRework, ListsDataSet.d_ReworkReasonDataTable dtReworkReason)
        {
            if (order == null || dtRework == null)
            {
                return "";
            }

            var partName = part?.Name ?? "NA";

            var reworkItems = new List<string>();
            var fullRework = dtRework.Where(r => r.OriginalOrderID == order.OrderID && r.IsReworkOrderIDNull()).ToList();
            var splitRework = dtRework.Where(r => !r.IsReworkOrderIDNull() && r.ReworkOrderID == order.OrderID).ToList();

            foreach (var rework in fullRework.Concat(splitRework))
            {
                var reason = dtReworkReason.FirstOrDefault(r => !rework.IsReworkReasonIDNull() && r.ReworkReasonID == rework.ReworkReasonID);

                if (reason == null || !reason.ShowOnDocuments)
                {
                    continue;
                }

                reworkItems.Add(reason.Name);
            }

            if (reworkItems.Count == 0)
            {
                return partName;
            }

            return partName + "\n" + string.Join("\n", reworkItems);
        }

        private void FormatTotalHeaderCell(WorksheetCell cell)
        {
            cell.CellFormat.Font.Name = "Verdana";
            cell.CellFormat.Font.Height = 8 * 20;
            cell.CellFormat.Font.ColorInfo = new WorkbookColorInfo(Color.White);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            cell.CellFormat.Alignment = HorizontalCellAlignment.Center;
            cell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            cell.CellFormat.Fill = CellFill.CreateSolidFill(AccentBackgroundColor);
            cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
            cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
            cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
            cell.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
        }

        private void FormatSumCell(WorksheetCell cell)
        {
            cell.CellFormat.Font.Name = "Verdana";
            cell.CellFormat.Font.Height = 8 * 20;
            cell.CellFormat.Font.ColorInfo = new WorkbookColorInfo(Color.Black);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            cell.CellFormat.Alignment = HorizontalCellAlignment.Center;
            cell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            cell.CellFormat.Fill = CellFill.CreateSolidFill(Color.LightGray);
            cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
            cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
            cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
            cell.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
        }

        private int AddSignatureRow(Worksheet worksheet, int rowIndex)
        {
            rowIndex += 2; //skip 2 lines

            //Received By
            var receivedByCell = worksheet.Rows[rowIndex].Cells[0];
            receivedByCell.Value = "Received By:";
            receivedByCell.CellFormat.Font.Name = "Verdana";
            receivedByCell.CellFormat.Font.Height = 8 * 20;
            receivedByCell.CellFormat.Font.ColorInfo = new WorkbookColorInfo(Color.Black);
            receivedByCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            receivedByCell.CellFormat.Alignment = HorizontalCellAlignment.Right;
            receivedByCell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

            //Signature Line
            var signatureLineCell = worksheet.MergedCellsRegions.Add(rowIndex, 1, rowIndex, 2);
            signatureLineCell.CellFormat.Font.Name = "Verdana";
            signatureLineCell.CellFormat.Font.Height = 8 * 20;
            signatureLineCell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;

            //Date Signed
            var dateCell = worksheet.Rows[rowIndex].Cells[3];
            dateCell.Value = "Date:";
            dateCell.CellFormat.SetFormatting(receivedByCell.CellFormat);

            //Date Line
            var dateLineCell = worksheet.MergedCellsRegions.Add(rowIndex, 4, rowIndex, 5);
            dateLineCell.CellFormat.SetFormatting(signatureLineCell.CellFormat);

            return rowIndex;
        }

        private int AddAddressRow(Worksheet worksheet, int rowIndex)
        {
            rowIndex += 2; //skip 2 lines

            var address = ApplicationSettings.Current.CompanyName + ", " + ApplicationSettings.Current.CompanyAddress1 + ", " + ApplicationSettings.Current.CompanyCity + ", " + ApplicationSettings.Current.CompanyState + " " + ApplicationSettings.Current.CompanyZip + " - " + ApplicationSettings.Current.CompanyPhone;

            var companyCell = worksheet.MergedCellsRegions.Add(rowIndex, 0, rowIndex, 5);
            companyCell.Value = address;
            companyCell.CellFormat.Font.Name = "Verdana";
            companyCell.CellFormat.Font.Height = 8 * 20;
            companyCell.CellFormat.Alignment = HorizontalCellAlignment.Center;
            companyCell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

            rowIndex++;

            if (ApplicationSettings.Current.CompanyUrl != null)
            {
                var urlCell = worksheet.MergedCellsRegions.Add(rowIndex, 0, rowIndex, 5);
                urlCell.Value = ApplicationSettings.Current.CompanyUrl;
                urlCell.CellFormat.SetFormatting(companyCell.CellFormat);
            }

            return rowIndex;
        }

        private WorksheetCell CreateOrderHeaderCell(Worksheet worksheet, int rowIndex, int colIndex, string headerText, int width)
        {
            WorksheetCell worksheetCell = worksheet.Rows[rowIndex].Cells[colIndex];
            worksheetCell.Value = headerText;
            this.ApplyDefaultCellFormat(worksheetCell);
            worksheet.Columns[colIndex].Width = Convert.ToInt32(_workbook.PixelsToCharacterWidth256ths(width));

            return worksheetCell;
        }

        private void ApplyDefaultCellFormat(WorksheetCell cell)
        {
            cell.CellFormat.Font.Name = "Verdana";
            cell.CellFormat.Font.Height = 8 * 20;
            cell.CellFormat.Font.ColorInfo = new WorkbookColorInfo(Color.White);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            cell.CellFormat.Alignment = HorizontalCellAlignment.Center;
            cell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            cell.CellFormat.Fill = CellFill.CreateSolidFill(AccentBackgroundColor);
            cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
            cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
            cell.CellFormat.RightBorderColorInfo = new WorkbookColorInfo(Color.White);
            cell.CellFormat.LeftBorderColorInfo = new WorkbookColorInfo(Color.White);
        }

        /// <summary>
        /// Gets the price.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        private decimal GetPrice(Data.Datasets.OrdersDataSet.OrderRow order)
        {
            try
            {
                if(order == null)
                    return 0;

                using(var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderFeesTableAdapter())
                {
                    using(var taFeeType = new Data.Datasets.OrdersDataSetTableAdapters.OrderFeeTypeTableAdapter())
                    {
                        var orderFees = new OrdersDataSet.OrderFeesDataTable();
                        var fees = 0M;

                        ta.FillByOrder(orderFees, order.OrderID);

                        var priceUnit = order.IsPriceUnitNull() ? string.Empty : order.PriceUnit;
                        var partQuantity = order.IsPartQuantityNull() ? 0 : order.PartQuantity;
                        var weight = order.IsWeightNull() ? 0M : order.Weight;
                        var basePrice = order.IsBasePriceNull() ? 0 : order.BasePrice;

                        foreach (var fee in orderFees)
                        {
                            if(fee.OrderFeeTypeID != null)
                            {
                                var chargeType = taFeeType.GetFeeType(fee.OrderFeeTypeID);
                                fees += OrderPrice.CalculateFees(chargeType, fee.Charge, basePrice, partQuantity, priceUnit, weight);
                            }
                        }

                        return OrderPrice.CalculatePrice(basePrice, priceUnit, fees, partQuantity, weight);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Warn(exc, "Unable to determine order price.");
                return 0;
            }
        }

        /// <summary>
        /// Gets text representing processes in an order.
        /// </summary>
        /// <remarks>
        /// This string should include only order processes that are marked
        /// for inclusion on the COC.
        /// </remarks>
        /// <param name="order"></param>
        /// <returns>String representation of processes.</returns>
        private string GetProcessText(OrdersDataSet.OrderRow order)
        {
            OrdersDataSet.OrderProcessesDataTable dtOrderProcesses = null;
            OrdersDataSet.ProcessAliasSummaryDataTable dtProcessAlias = null;

            OrderProcessesTableAdapter taOrderProcesses = null;
            Data.Datasets.OrdersDataSetTableAdapters.ProcessAliasSummaryTableAdapter taProcessAlias = null;

            try
            {
                if (order == null)
                {
                    return NOT_AVAILABLE;
                }

                dtOrderProcesses = new OrdersDataSet.OrderProcessesDataTable();
                dtProcessAlias = new OrdersDataSet.ProcessAliasSummaryDataTable();

                taOrderProcesses = new OrderProcessesTableAdapter();
                taProcessAlias = new Data.Datasets.OrdersDataSetTableAdapters.ProcessAliasSummaryTableAdapter();

                taOrderProcesses.FillBy(dtOrderProcesses, order.OrderID);
                taProcessAlias.FillByOrder(dtProcessAlias, order.OrderID);

                var processEntries = new List<string>();
                foreach (var process in dtOrderProcesses.Where(op => op.COCData).OrderBy(op => op.StepOrder))
                {
                    var processAlias = dtProcessAlias.FindByProcessAliasID(process.ProcessAliasID);

                    if (processAlias != null)
                    {
                        processEntries.Add(processAlias.Name);
                    }
                }

                return string.Join("\n", processEntries);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to get process text.");

                return NOT_AVAILABLE;
            }
            finally
            {
                dtOrderProcesses?.Dispose();
                dtProcessAlias?.Dispose();

                taOrderProcesses?.Dispose();
                taProcessAlias?.Dispose();
            }
        }

        #endregion

        #region ShippingAddress

        private sealed class ShipmentInfo
        {
            #region Properties

            public int ShipmentPackageId { get; set; }

            public string Name { get; set; }

            public string Address1 { get; set; }

            public string Address2 { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public string Zip { get; set; }

            public int CountryId { get; set; }

            public string CountryName { get; set; }

            public string ShipmentCarrier { get; internal set; }

            public string TrackingNumber { get; internal set; }

            public object UserName { get; internal set; }

            public bool HasAddress => !string.IsNullOrEmpty(Address1)
                || !string.IsNullOrEmpty(City)
                || !string.IsNullOrEmpty(State)
                || !string.IsNullOrEmpty(Zip);

            #endregion
        }

        #endregion
    }

    public class ShipRecReport : ExcelBaseReport
    {
        #region Fields

        public enum enumGroupBy
        {
            Day,
            Week,
            Month
        }

        private DateTime _fromDate;
        private DateTime _toDate;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Shipping And Receiving Report"; }
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
        [DisplayName("Date From")]
        [Category("Report")]
        public DateTime FromDate
        {
            get { return this._fromDate; }
            set { this._fromDate = value; }
        }

        [Browsable(true)]
        [Description("The to date of the report.")]
        [DisplayName("Date To")]
        [Category("Report")]
        public DateTime ToDate
        {
            get { return this._toDate; }
            set { this._toDate = value; }
        }

        [Browsable(true)]
        [Description("Determines how to group the production report orders by.")]
        [DisplayName("Group By")]
        [Category("Report")]
        public enumGroupBy GroupBy { get; set; }

        #endregion

        #region Methods

        public ShipRecReport()
        {
            this._fromDate = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            this._toDate = DateUtilities.GetLastDayOfMonth(DateTime.Now);
            GroupBy = enumGroupBy.Day;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            //ensure to get correct time with same date
            this._fromDate = new DateTime(this._fromDate.Year, this._fromDate.Month, this._fromDate.Day, 0, 0, 0);
            this._toDate = new DateTime(this._toDate.Year, this._toDate.Month, this._toDate.Day, 23, 59, 59);

            CreateReportExcel();
        }

        private void CreateReportExcel()
        {
            var worksheet = CreateWorksheet(Title);
            var rowIndex = AddCompanyHeaderRows(worksheet, 8) + 2;

            // Add the to/from dates and the grouping
            CreateCell(worksheet, rowIndex, 0, "From Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, FromDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex++;
            CreateCell(worksheet, rowIndex, 0, "To Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, ToDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex++;
            CreateCell(worksheet, rowIndex, 0, "Group By:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, GroupBy, false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex = rowIndex + 2;

            // Create the header row
            CreateHeaderCell(worksheet, rowIndex, 0, "Date");
            CreateHeaderCell(worksheet, rowIndex, 1, "Orders Received");
            CreateHeaderCell(worksheet, rowIndex, 2, "Pieces Received");
            CreateHeaderCell(worksheet, rowIndex, 3, "Orders Created");
            CreateHeaderCell(worksheet, rowIndex, 4, "Pieces Created");
            CreateHeaderCell(worksheet, rowIndex, 5, "Orders Shipped");
            CreateHeaderCell(worksheet, rowIndex, 6, "Pieces Shipped");
            CreateHeaderCell(worksheet, rowIndex, 7, "Total Price Shipped");
            CreateHeaderCell(worksheet, rowIndex, 8, "Piece Price Shipped");
            CreateHeaderCell(worksheet, rowIndex, 9, "Average Order Revenue");
            rowIndex++;

            // Add the data
            List <TransactionSummary> transactions = LoadSummaries(this._fromDate, this._toDate);
            transactions.Sort(new SortByDate());
            AddSummary(worksheet, transactions, rowIndex);

            // Set column widths
            worksheet.Columns[0].SetWidth(15, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[1].SetWidth(20, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[2].SetWidth(20, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[3].SetWidth(20, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[4].SetWidth(20, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[5].SetWidth(20, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[6].SetWidth(20, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[7].SetWidth(25, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[8].SetWidth(25, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[9].SetWidth(30, WorksheetColumnWidthUnit.Character);
        }

        private void AddSummary(Worksheet worksheet, List <TransactionSummary> transactions, int rowIndex)
        {
            var startRowIndex = rowIndex;

            foreach(var trans in transactions)
            {
                CreateCell(worksheet, rowIndex, 0, GetDateTimeStampDisplayValue(trans.DateTimeStamp), false, HorizontalCellAlignment.Center);
                CreateCell(worksheet, rowIndex, 1, trans.OrdersRec, false, HorizontalCellAlignment.Center);
                CreateCell(worksheet, rowIndex, 2, trans.PartRec, false, HorizontalCellAlignment.Center);
                CreateCell(worksheet, rowIndex, 3, trans.OrdersCcreated, false, HorizontalCellAlignment.Center);
                CreateCell(worksheet, rowIndex, 4, trans.PartsCreated, false, HorizontalCellAlignment.Center);
                CreateCell(worksheet, rowIndex, 5, trans.OrdersShip, false, HorizontalCellAlignment.Center);
                CreateCell(worksheet, rowIndex, 6, trans.PartShip, false, HorizontalCellAlignment.Center);
                CreateCell(worksheet, rowIndex, 7, trans.TotalPrice, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, MONEY_FORMAT);
                CreateCell(worksheet, rowIndex, 8, trans.PricePerPart, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, MONEY_FORMAT);

                string avg = string.Format("=IF(R{0}C6=0, 0, R{0}C8/R{0}C6)",
                    rowIndex + 1);

                CreateFormulaCell(worksheet, rowIndex, 9, avg, CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, MONEY_FORMAT);
                rowIndex++;
            }

            if (transactions.Count == 0)
            {
                for (int colIndex = 0; colIndex < 9; colIndex++)
                {
                    CreateCell(worksheet, rowIndex, colIndex, string.Empty, false, HorizontalCellAlignment.Center);
                }

                rowIndex++;
            }

            // Add the 'Totals' row
            CreateCell(worksheet, rowIndex, 0, "Total", false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 1, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 2, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 3, rowIndex, 3), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 3, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 4, rowIndex, 4), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 4, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 5, rowIndex, 5), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 5, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 6, rowIndex, 6), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 6, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 7, rowIndex, 7), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 7, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 8, rowIndex, 8), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 8, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 9, rowIndex, 9), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 9, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 10, rowIndex, 10), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;

            // Add the 'Average' row
            CreateCell(worksheet, rowIndex, 0, "Average", false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 1, "=ROUND(AVERAGE(R{0}C{1}:R{2}C{3}), 0)".FormatWith(startRowIndex + 1, 2, rowIndex - 1, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 2, "=ROUND(AVERAGE(R{0}C{1}:R{2}C{3}), 0)".FormatWith(startRowIndex + 1, 3, rowIndex - 1, 3), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 3, "=ROUND(AVERAGE(R{0}C{1}:R{2}C{3}), 0)".FormatWith(startRowIndex + 1, 4, rowIndex - 1, 4), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 4, "=ROUND(AVERAGE(R{0}C{1}:R{2}C{3}), 0)".FormatWith(startRowIndex + 1, 5, rowIndex - 1, 5), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 5, "=ROUND(AVERAGE(R{0}C{1}:R{2}C{3}), 0)".FormatWith(startRowIndex + 1, 6, rowIndex - 1, 6), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 6, "=ROUND(AVERAGE(R{0}C{1}:R{2}C{3}), 0)".FormatWith(startRowIndex + 1, 7, rowIndex - 1, 7), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 7, "=AVERAGE(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 8, rowIndex - 1, 8), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 8, "=AVERAGE(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 9, rowIndex - 1, 9), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 9, "=AVERAGE(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 10, rowIndex - 1, 10), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
        }

        private List <TransactionSummary> LoadSummaries(DateTime fromDate, DateTime toDate)
        {
            var shipments = new Data.Reports.ProcessPartsReport.ShippingProductionDataTable();
            var taShipping = new ShippingProductionTableAdapter();

            var reciving = new Data.Reports.ProcessPartsReport.ReceivingProductionDataTable();
            var taReciving = new ReceivingProductionTableAdapter();

            var taOrderCreated = new OrderCreationTableAdapter();

            var transactions = new List <TransactionSummary>();

            try
            {
                //Get all orders
                taShipping.Fill(shipments, fromDate, toDate);
                taReciving.Fill(reciving, fromDate, toDate);
                Data.Reports.ProcessPartsReport.OrderCreationDataTable ordersCreated = taOrderCreated.GetData(fromDate, toDate);

                TransactionSummary currentTrans = null;

                foreach(var item in shipments)
                {
                    long dateTimeStamp = GetDateTimeStamp(item.DateShipped);
                    currentTrans = transactions.Find(find => find.DateTimeStamp == dateTimeStamp);

                    //if no dept then create one
                    if(currentTrans == null)
                    {
                        currentTrans = new TransactionSummary();
                        currentTrans.DateTimeStamp = dateTimeStamp;
                        transactions.Add(currentTrans);
                    }

                    if(!item.IsPartQuantityNull())
                    {
                        //add order and parts summary
                        currentTrans.OrdersShip += 1;
                        currentTrans.PartShip += item.PartQuantity;

                        //add total price summary
                        if(!item.IsBasePriceNull() && !item.IsPriceUnitNull())
                        {
                            decimal fees = item.IsFeesNull() ? 0 : item.Fees;
                            decimal weight = item.IsWeightNull() ? 0 : item.Weight;
                            decimal price = OrderPrice.CalculatePrice(item.BasePrice, item.PriceUnit, fees, item.PartQuantity, weight);
                            currentTrans.BasePrice += price - fees;
                            currentTrans.FeesPrice += fees;
                            currentTrans.TotalPrice += price;

                            Debug.WriteLine("{0} - Fees: {1} = Total {2}", item.OrderID, fees.ToString(OrderPrice.CurrencyFormatString), price.ToString(OrderPrice.CurrencyFormatString));
                        }
                    }
                }

                foreach(var item in reciving)
                {
                    long dateTimeStamp = GetDateTimeStamp(DateTime.Parse(item.CheckIn));
                    currentTrans = transactions.Find(find => find.DateTimeStamp == dateTimeStamp);

                    //if no dept then create one
                    if(currentTrans == null)
                    {
                        currentTrans = new TransactionSummary();
                        currentTrans.DateTimeStamp = dateTimeStamp;
                        transactions.Add(currentTrans);
                    }

                    //add order and parts summary
                    currentTrans.OrdersRec += 1;
                    currentTrans.PartRec += item.PartQuantity;
                }

                foreach(var item in ordersCreated)
                {
                    long dateTimeStamp = GetDateTimeStamp(item.OrderDate);
                    currentTrans = transactions.Find(find => find.DateTimeStamp == dateTimeStamp);

                    //if no dept then create one
                    if(currentTrans == null)
                    {
                        currentTrans = new TransactionSummary();
                        currentTrans.DateTimeStamp = dateTimeStamp;
                        transactions.Add(currentTrans);
                    }

                    if(!item.IsPartQuantityNull())
                    {
                        //add order and parts summary
                        currentTrans.OrdersCcreated += 1;
                        currentTrans.PartsCreated += item.PartQuantity;
                    }
                }

                return transactions;
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error ");
                return transactions;
            }
            finally
            {
                if(shipments != null)
                    shipments.Dispose();
                if(taShipping != null)
                    taShipping.Dispose();
                if(reciving != null)
                    reciving.Dispose();
                if(taReciving != null)
                    taReciving.Dispose();
            }
        }

        private long GetDateTimeStamp(DateTime date)
        {
            switch(GroupBy)
            {
                case enumGroupBy.Week: //get first monday
                    return new DateTime(date.Year, date.Month, date.Day).StartOfWeek(DayOfWeek.Monday).Ticks;
                case enumGroupBy.Month: //get first day of month
                    return new DateTime(date.Year, date.Month, 1).Ticks;
                case enumGroupBy.Day: //use just the day
                default:
                    //May be working weekends, don't push the date, per TTP #659
                    //if(date.DayOfWeek == DayOfWeek.Saturday)
                    //    date = date.AddDays(-1); //Move to Friday
                    //else if(date.DayOfWeek == DayOfWeek.Sunday)
                    //    date = date.AddDays(1); //move to Monday

                    return new DateTime(date.Year, date.Month, date.Day).Ticks;
            }
        }

        private DateTime GetDateTimeStampDisplayValue(long dateTimeStamp)
        {
            switch(GroupBy)
            {
                case enumGroupBy.Week: //get week of year
                    //return new DateTime(dateTimeStamp).Month + "/" + new DateTime(dateTimeStamp).Day;
                    return new DateTime(dateTimeStamp).Date;
                case enumGroupBy.Month: //get first day of month
                    //return new DateTime(dateTimeStamp).Month + "/" + new DateTime(dateTimeStamp).Year;
                    return new DateTime(dateTimeStamp).Date;
                case enumGroupBy.Day: //use just the day
                default:
                    return new DateTime(dateTimeStamp).Date;
            }
        }

        #endregion

        #region DepartmentSummary

        private class TransactionSummary
        {
            public decimal BasePrice;
            public long DateTimeStamp;
            public decimal FeesPrice;
            public int OrdersCcreated;
            public int OrdersRec;
            public int OrdersShip;
            public int PartRec;
            public int PartShip;
            public int PartsCreated;
            public decimal TotalPrice;

            public decimal PricePerPart
            {
                get
                {
                    if(this.PartShip > 0)
                        return this.BasePrice / this.PartShip;
                    return 0;
                }
            }
        }

        #endregion

        #region Sorters

        private class SortByDate : IComparer <TransactionSummary>
        {
            #region IComparer<TransactionSummary> Members

            public int Compare(TransactionSummary x, TransactionSummary y) { return x.DateTimeStamp.CompareTo(y.DateTimeStamp); }

            #endregion
        }

        #endregion
    }

    public class ShippingDetailReport : ExcelBaseReport
    {
        #region Properties

        public override string Title
        {
            get { return "Shipping Detail"; }
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

        /// <summary>
        ///     Gets or sets the customer ID. If NULL, then show all customers.
        /// </summary>
        /// <value>The customer ID.</value>
        public int? CustomerID { get; set; }

        #endregion

        #region Methods

        public ShippingDetailReport()
        {
            FromDate = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            ToDate = DateUtilities.GetLastDayOfMonth(DateTime.Now);
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            //ensure to get correct time with same date
            FromDate = new DateTime(FromDate.Year, FromDate.Month, FromDate.Day, 0, 0, 0);
            ToDate = new DateTime(ToDate.Year, ToDate.Month, ToDate.Day, 23, 59, 59);

            var taShipSummary = new ShipmentSummaryTableAdapter();
            var taOrderShipSummary = new OrderShipmentSummaryTableAdapter();

            ShipmentReport.OrderShipmentSummaryDataTable orderShip = !CustomerID.HasValue ? taOrderShipSummary.GetData(FromDate, ToDate) : taOrderShipSummary.GetDataByCustomer(FromDate, ToDate, CustomerID.Value);
            ShipmentReport.ShipmentSummaryDataTable shipSummary = !CustomerID.HasValue ? taShipSummary.GetData(FromDate, ToDate) : taShipSummary.GetDataByCustomer(FromDate, ToDate, CustomerID.Value);

            CreateReportExcel(orderShip, shipSummary);
        }

        private void CreateReportExcel(ShipmentReport.OrderShipmentSummaryDataTable orderShip, ShipmentReport.ShipmentSummaryDataTable shipSummary)
        {
            //For serial numbers
            OrdersReport dsOrdersReport = new OrdersReport();
            dsOrdersReport.EnforceConstraints = false;
            using (var taOrderSerialNumbers = new Data.Reports.OrdersReportTableAdapters.OrderSerialNumberTableAdapter())
            {
                taOrderSerialNumbers.FillActive(dsOrdersReport.OrderSerialNumber);
            }

            // For order costs
            using (var taOrderFeeType = new Data.Reports.OrdersReportTableAdapters.OrderFeeTypeTableAdapter())
            {
                taOrderFeeType.Fill(dsOrdersReport.OrderFeeType);
            }

            //Get each Customer
            DataRow[] customerRows = shipSummary.DefaultView.ToTable(true, "CustomerName", "CustomerID").Select("", "CustomerName");
            Worksheet customerWks = CreateWorksheet(Title);
            int columnIndex = 0, rowIndex = 0;
            rowIndex = base.AddCompanyHeaderRows(customerWks, 7, "") + 2;

            var headerRowIndex = rowIndex;
            CreateHeaderCell(customerWks, rowIndex, columnIndex++, "Customer", 30);
            CreateHeaderCell(customerWks, rowIndex, columnIndex++, "Box", 15);
            CreateHeaderCell(customerWks, rowIndex, columnIndex++, "WO", 15);
            CreateHeaderCell(customerWks, rowIndex, columnIndex++, "Close Date", 20);
            CreateHeaderCell(customerWks, rowIndex, columnIndex++, "Date Shipped", 20);
            CreateHeaderCell(customerWks, rowIndex, columnIndex++, "PO", 15);
            CreateHeaderCell(customerWks, rowIndex, columnIndex++, "Part", 20);
            CreateHeaderCell(customerWks, rowIndex, columnIndex++, "Quantity", 15);
            CreateHeaderCell(customerWks, rowIndex, columnIndex++, "Lost Qty", 15);
            CreateHeaderCell(customerWks, rowIndex, columnIndex++, "Quarantined Qty", 25);
            CreateHeaderCell(customerWks, rowIndex, columnIndex++, "Ship To", 20);
            CreateHeaderCell(customerWks, rowIndex, columnIndex++, "Carrier", 20);
            CreateHeaderCell(customerWks, rowIndex, columnIndex++, "Tracking", 40);
            CreateHeaderCell(customerWks, rowIndex, columnIndex++, "Serial Number(s)", 30);
            CreateHeaderCell(customerWks, rowIndex, columnIndex++, "Cost", 15);

            if (customerRows.Any())
            {
                var firstDataRow = rowIndex + 1; //next row is first data row
                foreach(var customerRow in customerRows)
                {
                    var customerID = Convert.ToInt32(customerRow[1]);
                    var customerShipments = shipSummary.Select("CustomerID = " + customerID, "CloseDate, ShipmentPackageID") as ShipmentReport.ShipmentSummaryRow[];
                    foreach(var shipment in customerShipments)
                    {
                        var orders = orderShip.Select("ShipmentPackageID = " + shipment.ShipmentPackageID) as ShipmentReport.OrderShipmentSummaryRow[];

                        foreach(var order in orders)
                        {
                            ++rowIndex;
                            columnIndex = 0;

                            // Get any lost or quarantined orders associated with the current order
                            var reworkInfo = GetReworkInfo(order.OrderID);
                            var quarantinedPartQuantity = 0;
                            var quarantinedOrderIDs = string.Empty;
                            var lostOrderPartQty = 0;
                            var lostOrderIDs = string.Empty;
                            foreach(var rework in reworkInfo)
                            {
                                if(rework.ReworkType == "Lost")
                                {
                                    lostOrderPartQty += rework.PartQuantity;
                                    if(lostOrderIDs == string.Empty)
                                        lostOrderIDs += rework.WorkOrder + " - " + rework.OrderDate.ToShortDateString();
                                    else
                                        lostOrderIDs += ", " + rework.WorkOrder + " - " + rework.OrderDate.ToShortDateString();
                                }
                                else if(rework.ReworkType == "Quarantine")
                                {
                                    quarantinedPartQuantity += rework.PartQuantity;
                                    if(quarantinedOrderIDs == string.Empty)
                                        quarantinedOrderIDs += rework.WorkOrder + " - " + rework.OrderDate.ToShortDateString();
                                    else
                                        quarantinedOrderIDs += ", " + rework.WorkOrder + " (" + rework.PartQuantity + ")";
                                }
                            }

                            customerWks.Rows[rowIndex].Cells[columnIndex++].Value = shipment.CustomerName;
                            customerWks.Rows[rowIndex].Cells[columnIndex++].Value = shipment.PackageNumber;
                            customerWks.Rows[rowIndex].Cells[columnIndex++].Value = order.OrderID;
                            customerWks.Rows[rowIndex].Cells[columnIndex++].Value = shipment.CloseDate.Date;
                            customerWks.Rows[rowIndex].Cells[columnIndex++].Value = order.DateShipped.Date;
                            customerWks.Rows[rowIndex].Cells[columnIndex++].Value = order.IsPurchaseOrderNull() ? string.Empty : order.PurchaseOrder;
                            customerWks.Rows[rowIndex].Cells[columnIndex++].Value = order.PartName;
                            customerWks.Rows[rowIndex].Cells[columnIndex++].Value = order.PartQuantity;

                            // Add any lost order data
                            var lostCell = customerWks.Rows[rowIndex].Cells[columnIndex++];
                            if(lostOrderPartQty > 0)
                            {
                                lostCell.Value = lostOrderPartQty;
                                lostCell.Comment = new WorksheetCellComment {Text = new FormattedString("WO" + lostOrderIDs), Visible = false};
                            }
                            else
                                lostCell.Value = string.Empty;

                            // Add any quarantined order data
                            var quarantinedCell = customerWks.Rows[rowIndex].Cells[columnIndex++];
                            if(quarantinedPartQuantity > 0)
                            {
                                quarantinedCell.Value = quarantinedPartQuantity;
                                quarantinedCell.Comment = new WorksheetCellComment {Text = new FormattedString("WO" + quarantinedOrderIDs), Visible = false};
                            }
                            else
                                quarantinedCell.Value = string.Empty;

                            customerWks.Rows[rowIndex].Cells[columnIndex++].Value = shipment.IsCustomerAddressNameNull() ? string.Empty : shipment.CustomerAddressName;
                            customerWks.Rows[rowIndex].Cells[columnIndex++].Value = shipment.IsShippingCarrierNull() ? string.Empty : shipment.ShippingCarrier;
                            customerWks.Rows[rowIndex].Cells[columnIndex++].Value = shipment.IsTrackingNumberNull() ? string.Empty : shipment.TrackingNumber;

                            // Load order data for serial numbers and cost
                            using (var taOrders = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
                            {
                                taOrders.FillByOrder(dsOrdersReport.Order, order.OrderID);
                            }

                            using (var taOrderFees = new Data.Reports.OrdersReportTableAdapters.OrderFeesTableAdapter())
                            {
                                taOrderFees.FillByOrder(dsOrdersReport.OrderFees, order.OrderID);
                            }

                            using (var taLaborSummary = new Data.Reports.OrdersReportTableAdapters.LaborSummaryTableAdapter())
                            {
                                taLaborSummary.FillByOrder(dsOrdersReport.LaborSummary, order.OrderID);
                            }

                            // Serial numbers
                            IEnumerable<string> serialNumbers = Enumerable.Empty<string>();

                            if (dsOrdersReport.Order.Count > 0)
                            {
                                serialNumbers = dsOrdersReport.Order.First()
                                    .GetOrderSerialNumberRows()
                                    .Select(s => s.IsNumberNull() ? string.Empty : s.Number);
                            }

                            customerWks.Rows[rowIndex].Cells[columnIndex++].Value = string.Join(", ", serialNumbers);

                            // Cost
                            var orderCost = OrderCostData.From(dsOrdersReport.Order.First(), DateTime.Now);
                            customerWks.Rows[rowIndex].Cells[columnIndex].Value = orderCost.MaterialCost + orderCost.BurdenCost + orderCost.LaborCost;
                            customerWks.Rows[rowIndex].Cells[columnIndex++].CellFormat.FormatString = MONEY_FORMAT;
                        }
                    }
                }

                // Create the Totals row and format the cells
                var totalRowCell = CreateCell(customerWks, rowIndex + 1, 0, "Total:", true);
                totalRowCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                totalRowCell.CellFormat.LeftBorderStyle = CellBorderLineStyle.None;
                totalRowCell = CreateCell(customerWks, rowIndex + 1, 1, "Orders:", true, HorizontalCellAlignment.Right);
                totalRowCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                totalRowCell.CellFormat.LeftBorderStyle = CellBorderLineStyle.None;
                totalRowCell = CreateCell(customerWks, rowIndex + 1, 2, "", true, HorizontalCellAlignment.Right);
                totalRowCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                totalRowCell.CellFormat.LeftBorderStyle = CellBorderLineStyle.None;
                totalRowCell = CreateCell(customerWks, rowIndex + 1, 3, "", true);
                totalRowCell.CellFormat.LeftBorderStyle = CellBorderLineStyle.None;
                totalRowCell = CreateCell(customerWks, rowIndex + 1, 4, "", true);
                totalRowCell.CellFormat.LeftBorderStyle = CellBorderLineStyle.None;
                totalRowCell = CreateCell(customerWks, rowIndex + 1, 5, "", true);
                totalRowCell.CellFormat.LeftBorderStyle = CellBorderLineStyle.None;
                totalRowCell = CreateCell(customerWks, rowIndex + 1, 6, "Parts:", true, HorizontalCellAlignment.Right);
                totalRowCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                totalRowCell.CellFormat.LeftBorderStyle = CellBorderLineStyle.None;
                totalRowCell = CreateCell(customerWks, rowIndex + 1, 7, "", true, HorizontalCellAlignment.Right);
                totalRowCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                totalRowCell.CellFormat.LeftBorderStyle = CellBorderLineStyle.None;
                totalRowCell = CreateCell(customerWks, rowIndex + 1, 8, "", true);
                totalRowCell.CellFormat.LeftBorderStyle = CellBorderLineStyle.None;
                totalRowCell = CreateCell(customerWks, rowIndex + 1, 9, "", true);
                totalRowCell.CellFormat.LeftBorderStyle = CellBorderLineStyle.None;
                totalRowCell = CreateCell(customerWks, rowIndex + 1, 10, "", true);
                totalRowCell.CellFormat.LeftBorderStyle = CellBorderLineStyle.None;
                totalRowCell = CreateCell(customerWks, rowIndex + 1, 11, "", true);
                totalRowCell.CellFormat.LeftBorderStyle = CellBorderLineStyle.None;
                totalRowCell = CreateCell(customerWks, rowIndex + 1, 12, "", true);
                totalRowCell.CellFormat.LeftBorderStyle = CellBorderLineStyle.None;
                totalRowCell = CreateCell(customerWks, rowIndex + 1, 13, "", true);
                totalRowCell.CellFormat.LeftBorderStyle = CellBorderLineStyle.None;
                totalRowCell = CreateCell(customerWks, rowIndex + 1, 14, "", true);
                totalRowCell.CellFormat.LeftBorderStyle = CellBorderLineStyle.None;

                //Apply COUNT to number of orders
                var countFormula = Formula.Parse("=COUNT(R{0}C3:R{1}C3)".FormatWith(firstDataRow + 1, rowIndex + 1), CellReferenceMode.R1C1);
                countFormula.ApplyTo(customerWks.Rows[rowIndex + 1].Cells[2]);

                //Apply SUM to number of parts
                var sumFormula = Formula.Parse("=SUM(R{0}C8:R{1}C8)".FormatWith(firstDataRow + 1, rowIndex + 1), CellReferenceMode.R1C1);
                sumFormula.ApplyTo(customerWks.Rows[rowIndex + 1].Cells[7]);
            }
            
            FitAllColumnsOnSinglePage(customerWks);
            AddTopLogo(customerWks, 0, 9, 4, 10);
            CreateTable(customerWks, headerRowIndex + 1, columnIndex - 1, rowIndex + 1, true);
        }

        private List <ReworkItem> GetReworkInfo(int orderID)
        {
            var dt = new OrdersDataSet.InternalReworkDataTable();
            using(var internalReworkTableAdapter = new InternalReworkTableAdapter {ClearBeforeFill = true})
            {
                internalReworkTableAdapter.FillByOriginalOrderID(dt, orderID);
                if(dt.Rows.Count == 0)
                {
                    // try getting data by rework ID
                    internalReworkTableAdapter.FillByReworkOrderID(dt, orderID);
                }
            }

            var reworkList = new List <ReworkItem>();
            if(dt.Rows.Count > 0)
            {
                foreach(var row in dt.Rows)
                {
                    OrdersReport.OrderDataTable dtRework = null;
                    var item = row as OrdersDataSet.InternalReworkRow;
                    var reworkItem = new ReworkItem();
                    reworkItem.ReworkType = item.ReworkType;
                    reworkItem.WorkOrder = item.IsReworkOrderIDNull() ? item.OriginalOrderID : item.ReworkOrderID;

                    using(var taOrders = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
                        dtRework = taOrders.GetByOrder(reworkItem.WorkOrder);

                    if(dtRework.Rows.Count > 0)
                    {
                        var reworkRow = dtRework.Rows[0] as OrdersReport.OrderRow;
                        reworkItem.PartQuantity = !reworkRow.IsPartQuantityNull() ? reworkRow.PartQuantity : 0;
                        reworkItem.OrderDate = !reworkRow.IsCompletedDateNull() ? reworkRow.CompletedDate : DateTime.MinValue;
                    }

                    reworkList.Add(reworkItem);
                }
            }

            return reworkList;
        }

        private class ReworkItem
        {
            public int WorkOrder { get; set; }
            public string ReworkType { get; set; }
            public int PartQuantity { get; set; }
            public DateTime OrderDate { get; set; }
        }

        #endregion
    }

    public class RepairStatementReport : Report
    {
        #region Fields

        private readonly RepairStatementData _reportData;
        private readonly DateTime _orderDate;

        #endregion

        #region Properties

        public override string Title => "Statement of Repairs";

        protected override PageOrientation ReportPageOrientation => PageOrientation.Portrait;

        #endregion

        #region Methods

        public RepairStatementReport(RepairStatementData data, DateTime orderDate)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            _reportData = data;
            _orderDate = orderDate;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            SetupReportInfo();
            _section.PageMargins.All = 20;
            AddStatementHeader();
            AddStatementSection();
            AddPartsSection();
            AddInfoSection();
        }

        private void AddStatementHeader()
        {
            var normalFont = new Infragistics.Documents.Reports.Graphics.Font("Calibri", 14);

            var headerGroup = _section.AddGroup();
            headerGroup.Layout = Layout.Horizontal;

            var companyLogo = headerGroup.AddContainer("logo");
            companyLogo.Width = new RelativeWidth(50);
            AddStatementLogo(companyLogo);
            var dateText = companyLogo.AddText();
            dateText.AddLineBreak();
            dateText.AddContent($"Date: {_reportData.ShippedDate.Date:d}");

            var titleContainer = headerGroup.AddContainer("title");
            titleContainer.Width = new RelativeWidth(50);

            companyLogo.Alignment.Horizontal = Alignment.Left;
            companyLogo.Alignment.Vertical = Alignment.Top;
            titleContainer.Alignment.Horizontal = Alignment.Right;
            titleContainer.Alignment.Vertical = Alignment.Bottom;

            IText title = titleContainer.AddText();
            title.Alignment.Horizontal = Alignment.Right;
            title.Alignment.Vertical = Alignment.Bottom;
            title.Style = DefaultStyles.RedXLargeStyle;

            var superSizedFont = new Infragistics.Documents.Reports.Graphics.Font(DefaultStyles.RedXLargeStyle.Font.Name, DefaultStyles.RedXLargeStyle.Font.Size, Infragistics.Documents.Reports.Graphics.FontStyle.Bold);
            var superSized = new Style(superSizedFont, DefaultStyles.BlackXLargeStyle.Brush);
            title.Margins.Top = 5;
            title.Margins.Bottom = 0;
            title.Margins.Left = 5;

            title.AddContent("Statement of Repairs, Alterations, or Processing", superSized);
            title.AddLineBreak();
            title.AddContent("US Customs", new Style(normalFont, DefaultStyles.BlueLargeStyle.Brush));
            title.AddLineBreak();
            title.AddContent("SEC.18.8(E) C.R.", new Style(normalFont, DefaultStyles.BlackLargeStyle.Brush));
        }

        private static void AddStatementLogo(Infragistics.Documents.Reports.Report.IContainer logoContainer)
        {
            var logo = ApplicationSettings.Current.ShippingRepairsStatementLogo;

            if (string.IsNullOrEmpty(logo))
            {
                logo = ApplicationSettings.Current.CompanyLogoImagePath;
            }

            if (string.IsNullOrEmpty(logo))
            {
                // Infragistics expects logo to have something
                logoContainer.AddText();
                return;
            }

            Infragistics.Documents.Reports.Graphics.Image pic;
            using (var sourceImg = MediaUtilities.GetImage(logo))
            {
                pic = new Infragistics.Documents.Reports.Graphics.Image(MediaUtilities.PrepareForReport(sourceImg));
            }

            IImage image = logoContainer.AddImage(pic);
            image.KeepRatio = true;
            int pWidth = pic.Width;
            int pHeight = pic.Height;
            double standardHeight = 80;

            // Prevent the image from being too big on the report
            if (pic.Height > 80)
            {
                double percent = standardHeight / pHeight;
                image.Height = new FixedHeight(80);
                double width = percent * pWidth;
                image.Width = new FixedWidth((float)width);
            }
            else if (pic.Width > 185)
                image.Width = new FixedWidth(185);

            image.Margins.Top = 10;
        }

        private void AddStatementSection()
        {
            var statementGroup = _section.AddGroup();
            statementGroup.Paddings.Top = 10;
            statementGroup.Paddings.Bottom = 10;
            var statementText = statementGroup.AddText();
            statementText.Style = DefaultStyles.NormalStyle;

            // Statement text
            var statementContent = string.Empty;
            ApplicationSettingsDataSet.TemplatesRow template = null;
            using(var ta = new TemplatesTableAdapter())
            {
                template = ta.GetDataById("RepairsExport").FirstOrDefault();
            }

            if (template != null)
            {
                statementContent = template.Template;
            }

            if (string.IsNullOrEmpty(statementContent))
            {
                return;
            }

            statementContent = statementContent
                .Replace("%COMPANY%", ApplicationSettings.Current.CompanyName)
                .Replace("%RECEIVEDDATE%", _orderDate.ToShortDateString())
                .Replace("%CUSTOMERNAME%", _reportData.CustomerName)
                .Replace("&edsp;", "&nbsp;")
                .Replace("\"8pt\"", "\"8\"");

            statementGroup.AddText().AddRichContent(statementContent, false);
        }

        private void AddPartsSection()
        {
            const int relativeWidthMarks = 2;
            const int relativeWidthDescription = 3;
            const int relativeWidthImport = 1;
            const int relativeWidthRepair = 1;
            const int relativeWidthTotal = 1;

            const string headerMarks = "Marks & Numbers";
            const string headerDescription = "Description of Articles and of repairs, alterations, or processing";
            const string headerImport = "Value of the imported material";
            const string headerRepair = "Value of the repairs";
            const string headerTotal = "Total Value";

            var partGroup = _section.AddGroup();
            partGroup.Margins.All = 5;
            partGroup.Borders = DefaultStyles.DefaultBorders;
            partGroup.Background = DefaultStyles.DefaultBackground;

            var partTable = partGroup.AddTable();
            partTable.CreateTableHeaderCell(relativeWidthMarks).AddText(headerMarks, DefaultStyles.NormalStyle, TextAlignment.Center);
            partTable.CreateTableHeaderCell(relativeWidthDescription).AddText(headerDescription, DefaultStyles.NormalStyle, TextAlignment.Center);
            partTable.CreateTableHeaderCell(relativeWidthImport).AddText(headerImport, DefaultStyles.NormalStyle, TextAlignment.Center);
            partTable.CreateTableHeaderCell(relativeWidthRepair).AddText(headerRepair, DefaultStyles.NormalStyle, TextAlignment.Center);
            partTable.CreateTableHeaderCell(relativeWidthTotal).AddText(headerTotal, DefaultStyles.NormalStyle, TextAlignment.Center);

            var currencyFormatString = OrderPrice.CurrencyFormatString;
            var parts = _reportData.GetParts(_orderDate);
            foreach (var part in parts)
            {
                var imported = part.ImportedPrice.ToString(currencyFormatString);
                var repair = part.OrderPrice.ToString(currencyFormatString);
                var total = part.TotalPrice.ToString(currencyFormatString);

                var partRow = partTable.AddRow();
                partRow.CreateTableCell(relativeWidthMarks).AddText(part.SerialNumber, DefaultStyles.NormalStyle, TextAlignment.Center);
                partRow.CreateTableCell(relativeWidthDescription).AddText(DescriptionText(part), DefaultStyles.NormalStyle, TextAlignment.Center);
                partRow.CreateTableCell(relativeWidthImport).AddText(imported, DefaultStyles.NormalStyle, TextAlignment.Center);
                partRow.CreateTableCell(relativeWidthRepair).AddText(repair, DefaultStyles.NormalStyle, TextAlignment.Center);
                partRow.CreateTableCell(relativeWidthTotal).AddText(total, DefaultStyles.NormalStyle, TextAlignment.Center);
            }
        }

        private void AddInfoSection()
        {
            var infoGroup = _section.AddGroup();
            infoGroup.Paddings.Top = 10;

            var infoTable = infoGroup.AddTable();
            var firstRow = infoTable.AddRow();

            var firstRowCells = new List<ITableCell>();
            firstRowCells.Add(firstRow.AddCell(1, "Signature:", DefaultStyles.NormalStyle, TextAlignment.Left));

            if (_reportData.User != null)
            {
                // Show user's signature
                var signatureCell = firstRow.AddCell();
                signatureCell.Width = new RelativeWidth(3);
                var group = signatureCell.AddGroup();
                group.Alignment = Infragistics.Documents.Reports.Report.ContentAlignment.Left;
                group.Layout = Layout.Vertical;
                group.Width = new RelativeWidth(75);

                if (_reportData.User.Signature != null)
                {
                    IImage image = group.AddImage(new Infragistics.Documents.Reports.Graphics.Image(_reportData.User.Signature));
                    var maxSize = new Size(_reportData.User.Signature.Width, 40);
                    var imgSize = MediaUtilities.Resize(_reportData.User.Signature.Size, maxSize);
                    image.Height = new FixedHeight(imgSize.Height);
                    image.Width = new FixedWidth(imgSize.Width);
                    image.KeepRatio = true;
                }
                else
                {
                    // Leave some space for the user to sign the document
                    group.Margins = new Margins(0, 0, 10, 0);
                }

                //Add Signature Line
                IRule rule = group.AddRule();
                rule.Pen = new Infragistics.Documents.Reports.Graphics.Pen(new Infragistics.Documents.Reports.Graphics.Color(Color.Black));
                rule.Pen.Width = 1;
                rule.Margins = new VerticalMargins(2, 2);

                //Add User Name
                IText userName = group.AddText();
                userName.Style = DefaultStyles.NormalStyle;
                var displayName = _reportData.User.Name;
                if (!string.IsNullOrWhiteSpace(_reportData.User.Title))
                {
                    displayName += ", " + _reportData.User.Title;
                }

                userName.AddContent(displayName);
                firstRowCells.Add(signatureCell);
            }
            else
            {
                // Add blank signature
                firstRowCells.Add(firstRow.AddCell(3, "__________________________________", DefaultStyles.NormalStyle, TextAlignment.Left));
            }

            firstRowCells.Add(firstRow.AddCell(1, "Capacity:", DefaultStyles.NormalStyle, TextAlignment.Left));
            firstRowCells.Add(firstRow.AddCell(3, _reportData.GetCapacity(_orderDate).ToString(), DefaultStyles.NormalStyle, TextAlignment.Left));

            foreach (var cell in firstRowCells)
            {
                cell.Paddings.Bottom = 10;
            }

            // Add address
            var secondRow = infoTable.AddRow();
            secondRow.AddCell(1, "Address:", DefaultStyles.NormalStyle, TextAlignment.Left);

            var address1 = _reportData.SenderAddress.Address1;
            var address2 = _reportData.SenderAddress.Address2;
            var city = _reportData.SenderAddress.City;
            var state = _reportData.SenderAddress.State;
            var zip = _reportData.SenderAddress.Zip;

            var addressCell = secondRow.AddCell();
            addressCell.Width = new RelativeWidth(3);
            var addressText = addressCell.AddText();
            addressText.Style = DefaultStyles.NormalStyle;
            addressText.AddContent(address1);
            addressText.AddLineBreak();

            if (!string.IsNullOrEmpty(address2))
            {
                addressText.AddContent(address2);
                addressText.AddLineBreak();
            }

            addressText.AddContent($"{city}, {state} {zip}");

            // Add empty cell so that second row's columns match the first row
            secondRow.AddCell().Width = new RelativeWidth(4);
        }

        private string DescriptionText(RepairStatementData.ReportPart part)
        {
            if (part == null)
            {
                return string.Empty;
            }

            if (part.Fees != null && part.Fees.Count > 0)
            {
                var currencyFormatString = OrderPrice.CurrencyFormatString;

                var feeText = part.Fees
                    .Select(fee => $"{fee.Name} - {fee.TotalPrice.ToString(currencyFormatString)}");

                return part.Description + Environment.NewLine +
                    string.Join(Environment.NewLine, feeText);
            }

            return part.Description;
        }

        #endregion
    }

    public class BillOfLadingReport : Report
    {
        #region Fields

        private const int BLANK_LINE_MARGIN = 12;
        private const float BLANK_LINE_WIDTH = 1f;

        #endregion

        #region Properties

        public override string Title =>
            "Bill of Lading";

        protected override PageOrientation ReportPageOrientation =>
            PageOrientation.Portrait;

        public int BillOfLadingId { get; }

        #endregion

        #region Methods

        public BillOfLadingReport(int billOfLadingId)
        {
            BillOfLadingId = billOfLadingId;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            SetupReportInfo();

            var reportData = ReportData.From(BillOfLadingId);
            AddHeader(reportData);
            AddAddressSection(reportData);
            AddCargoSection(reportData);
            AddRateSection();
            AddLiabilitySection();
            AddSignaturesSection();
        }

        private void AddHeader(ReportData data)
        {
            var headerGroup = _section.AddGroup();
            headerGroup.Layout = Layout.Horizontal;
            headerGroup.Margins = new Margins(0, 0, 0, 10);

            var companyLogo = headerGroup.AddContainer("logo");
            companyLogo.Width = new RelativeWidth(50);
            AddCompanyLogo(companyLogo);
            companyLogo.AddText().AddContent($"Date: {data.DateShipped:d}");

            companyLogo.AddText().AddContent("Received, subject to individually" +
                " determined rates or contracts that have been agreed upon in" +
                " writing between the carrier and shipper, if applicable," +
                " otherwise to the rates, classifications, and rules that" +
                " have been established by the carrier and are available to" +
                " the shipper, on request, and to all applicable state and" +
                " federal regulations.",
                DefaultStyles.SmallStyle);

            var titleContainer = headerGroup.AddContainer("title");
            titleContainer.Width = new RelativeWidth(50);

            companyLogo.Alignment.Horizontal = Alignment.Left;
            companyLogo.Alignment.Vertical = Alignment.Top;
            titleContainer.Alignment.Horizontal = Alignment.Right;
            titleContainer.Alignment.Vertical = Alignment.Bottom;

            IText title = titleContainer.AddText();
            title.Alignment.Horizontal = Alignment.Right;
            title.Alignment.Vertical = Alignment.Bottom;

            var superSizedFont = new Infragistics.Documents.Reports.Graphics.Font(DefaultStyles.RedXLargeStyle.Font.Name, DefaultStyles.RedXLargeStyle.Font.Size, Infragistics.Documents.Reports.Graphics.FontStyle.Bold);
            var superSized = new Style(superSizedFont, DefaultStyles.BlackXLargeStyle.Brush);
            title.Margins.Top = 5;
            title.Margins.Bottom = 0;
            title.Margins.Left = 5;

            title.AddContent("Bill of Lading - Non-Negotiable", superSized);
            title.AddLineBreak();

            var titleFont = new Infragistics.Documents.Reports.Graphics.Font("Calibri", 18);
            var titleStyle = new Style(titleFont, DefaultStyles.BlackLargeStyle.Brush);
            title.AddContent($"Package {data.ShipmentPackageId}", titleStyle);
            title.AddLineBreak();
            title.AddContent($"Bill of Lading {data.BillOfLadingId}", titleStyle);
        }

        private void AddAddressSection(ReportData data)
        {
            // Left size - Addresses
            var addressSection = _section.AddGroup();
            addressSection.Layout = Layout.Horizontal;
            addressSection.Width = new RelativeWidth(100);
            var addressGroup = addressSection.AddGroup();
            addressGroup.Width = new RelativeWidth(50);

            // Ship From
            addressGroup.AddText().AddContent("Ship From", DefaultStyles.BlueLargeStyle);
            var companyAddressText = addressGroup.AddText();
            companyAddressText.AddContent(data.CompanyName);
            companyAddressText.AddLineBreak();
            companyAddressText.AddContent(data.CompanyAddress1);
            companyAddressText.AddLineBreak();
            companyAddressText.AddContent($"{data.CompanyCity}, {data.CompanyState} {data.CompanyZip}");
            companyAddressText.Margins.Bottom = 20;

            // Ship To
            addressGroup.AddText().AddContent("Ship To", DefaultStyles.BlueLargeStyle);
            var customerAddressText = addressGroup.AddText();
            customerAddressText.AddContent(data.CustomerName);
            customerAddressText.AddLineBreak();

            if (data.HasCustomerAddress)
            {
                customerAddressText.AddContent(data.CustomerAddress1);
                customerAddressText.AddLineBreak();

                if (!string.IsNullOrEmpty(data.CustomerAddress2))
                {
                    customerAddressText.AddContent(data.CustomerAddress2);
                    customerAddressText.AddLineBreak();
                }

                customerAddressText.AddContent($"{data.CustomerCity}, {data.CustomerState} {data.CustomerZip}");

                if (string.IsNullOrEmpty(data.CustomerAddress2))
                {
                    // Add empty line
                    customerAddressText.AddLineBreak();
                    customerAddressText.AddContent(" ");
                }
                customerAddressText.Margins.Bottom = 20;
            }

            // Right side - carrier & charge terms
            var carrierGroup = addressSection.AddGroup();
            carrierGroup.Width = new RelativeWidth(50);

            // Carrier
            carrierGroup.AddText().AddContent("Carrier", DefaultStyles.BlueLargeStyle);

            var carrierTable = carrierGroup.AddTable();
            carrierTable.Margins.Bottom = 20;
            var carrierNameRow = carrierTable.AddRow();
            carrierNameRow.Margins = new VerticalMargins(0, 5);
            var labelColumnWidth = new RelativeWidth(30);
            var contentColumnWidth = new RelativeWidth(70);
            carrierNameRow.AddCell(labelColumnWidth).AddText("Name:", DefaultStyles.BlackLargeStyle, TextAlignment.Left);
            carrierNameRow.AddCell(contentColumnWidth).AddText(data.CarrierName, DefaultStyles.BlackLargeStyle, TextAlignment.Left);

            var scacRow = carrierTable.AddRow();
            scacRow.Margins = new VerticalMargins(0, 5);
            scacRow.AddCell(labelColumnWidth).AddText("SCAC:", DefaultStyles.BlackLargeStyle, TextAlignment.Left);
            var scacRule = scacRow.AddCell(contentColumnWidth).AddRule();
            scacRule.Pen = new Infragistics.Documents.Reports.Graphics.Pen(new Infragistics.Documents.Reports.Graphics.Color(Color.Black));
            scacRule.Pen.Width = BLANK_LINE_WIDTH;
            scacRule.Margins = new VerticalMargins(BLANK_LINE_MARGIN, 0);

            var proNumberRow = carrierTable.AddRow();
            proNumberRow.AddCell(labelColumnWidth).AddText("Pro Number:", DefaultStyles.BlackLargeStyle, TextAlignment.Left);
            var proNumberRule = proNumberRow.AddCell(contentColumnWidth).AddRule();
            proNumberRule.Pen = new Infragistics.Documents.Reports.Graphics.Pen(new Infragistics.Documents.Reports.Graphics.Color(Color.Black));
            proNumberRule.Pen.Width = BLANK_LINE_WIDTH;
            proNumberRule.Margins = new VerticalMargins(BLANK_LINE_MARGIN, 0);

            // Right Side - Charge Terms
            var chargeTermsGroup = carrierGroup.AddGroup();
            chargeTermsGroup.AddText().AddContent("Charge Terms", DefaultStyles.BlueLargeStyle);
            chargeTermsGroup.AddText().AddContent("(Freight charges are prepaid unless marked otherwise.)");

            const int rectangleDimension = 9;

            var chargeTermsTable = chargeTermsGroup.AddTable();

            var prepaidRow = chargeTermsTable.AddRow();
            var prepaidCheckbox = prepaidRow.AddCell(AutoWidth.Instance).AddCanvas();
            prepaidCheckbox.Width = new FixedWidth(rectangleDimension + 3);
            prepaidCheckbox.Height = new FixedHeight(rectangleDimension + 2);
            prepaidCheckbox.Pen = new Infragistics.Documents.Reports.Graphics.Pen(Infragistics.Documents.Reports.Graphics.Colors.Black, 1f);
            prepaidCheckbox.DrawRectangle(1, 1, rectangleDimension, rectangleDimension, Infragistics.Documents.Reports.Graphics.PaintMode.Stroke);

            var prepaidText = prepaidRow.AddCell(AutoWidth.Instance, new HorizontalMargins(4f, 0f)).AddText();
            prepaidText.Alignment = new TextAlignment(Alignment.Left, Alignment.Middle);
            prepaidText.AddContent("Prepaid");

            var collectRow = chargeTermsTable.AddRow();
            var collectCheckbox = collectRow.AddCell(AutoWidth.Instance).AddCanvas();
            collectCheckbox.Width = new FixedWidth(rectangleDimension + 3);
            collectCheckbox.Height = new FixedHeight(rectangleDimension + 2);
            collectCheckbox.Pen = new Infragistics.Documents.Reports.Graphics.Pen(Infragistics.Documents.Reports.Graphics.Colors.Black, 1f);
            collectCheckbox.DrawRectangle(1, 1, rectangleDimension, rectangleDimension, Infragistics.Documents.Reports.Graphics.PaintMode.Stroke);

            var collectText = collectRow.AddCell(AutoWidth.Instance, new HorizontalMargins(4f, 0f)).AddText();
            collectText.Alignment = new TextAlignment(Alignment.Left, Alignment.Middle);
            collectText.AddContent("Collect");
        }

        private void AddCargoSection(ReportData data)
        {
            const int packageWidth = 10;
            const int descriptionWidth = 60;
            const int weightWidth = 10;
            const int classWidth = 10;
            const int checkWidth = 10;

            var cargoTable = _section.AddTable();
            cargoTable.CreateTableHeaderCell(packageWidth, DefaultStyles.DefaultBorders).AddText("Number of Packages");
            cargoTable.CreateTableHeaderCell(descriptionWidth, DefaultStyles.DefaultBorders).AddText("Description");
            cargoTable.CreateTableHeaderCell(weightWidth, DefaultStyles.DefaultBorders).AddText("Weight");
            cargoTable.CreateTableHeaderCell(classWidth, DefaultStyles.DefaultBorders).AddText("Class of Rate");
            cargoTable.CreateTableHeaderCell(checkWidth, DefaultStyles.DefaultBorders).AddText("Check Column");

            foreach (var shipmentInfo in data.Shipments.OrderBy(s => s.Description))
            {
                var shipmentRow = cargoTable.AddRow();
                shipmentRow.AddCell(packageWidth, $"{shipmentInfo.Quantity}", DefaultStyles.NormalStyle, DefaultStyles.DefaultBorders, 2, TextAlignment.Center);
                shipmentRow.AddCell(descriptionWidth, $"{shipmentInfo.Description}", DefaultStyles.NormalStyle, DefaultStyles.DefaultBorders, 2, TextAlignment.Center);
                shipmentRow.AddCell(weightWidth, string.Empty, DefaultStyles.NormalStyle, DefaultStyles.DefaultBorders, 2, TextAlignment.Center);
                shipmentRow.AddCell(classWidth, string.Empty, DefaultStyles.NormalStyle, DefaultStyles.DefaultBorders, 2, TextAlignment.Center);
                shipmentRow.AddCell(checkWidth, string.Empty, DefaultStyles.NormalStyle, DefaultStyles.DefaultBorders, 2, TextAlignment.Center);
            }

            const int minCargoRows = 14;
            if (data.Shipments.Count < minCargoRows)
            {
                // Add empty rows so that the cargo section is a consistent
                // size on most bill of lading
                for (var i = data.Shipments.Count; i < minCargoRows; ++i)
                {
                    var blankShipmentRow = cargoTable.AddRow();

                    // First cell has space in it - otherwise, Infragistics will not actually add the row
                    blankShipmentRow.AddCell(packageWidth, " ", DefaultStyles.NormalStyle, DefaultStyles.DefaultBorders, 2, TextAlignment.Center);
                    blankShipmentRow.AddCell(descriptionWidth, string.Empty, DefaultStyles.NormalStyle, DefaultStyles.DefaultBorders, 2, TextAlignment.Center);
                    blankShipmentRow.AddCell(weightWidth, string.Empty, DefaultStyles.NormalStyle, DefaultStyles.DefaultBorders, 2, TextAlignment.Center);
                    blankShipmentRow.AddCell(classWidth, string.Empty, DefaultStyles.NormalStyle, DefaultStyles.DefaultBorders, 2, TextAlignment.Center);
                    blankShipmentRow.AddCell(checkWidth, string.Empty, DefaultStyles.NormalStyle, DefaultStyles.DefaultBorders, 2, TextAlignment.Center);
                }
            }
        }

        private void AddRateSection()
        {
            var rateGroup = _section.AddGroup();
            rateGroup.AddText().AddContent("Where the rate is dependent on " +
                "value, shippers are required to state specifically in writing" +
                " the agreed or declared value of the property as follows: " +
                "\"The agreed or declared value of the property is " +
                "specifically stated by the shipper to be not exceeding " +
                "_______________ per _______________.\"",
                DefaultStyles.NormalStyle);

            rateGroup.Margins.Bottom = 20;
        }

        private void AddLiabilitySection()
        {
            var legalGroup = _section.AddGroup();
            legalGroup.AddText().AddContent("Note: Liability limitation for " +
                "loss or damage in this shipment may be applicable. See 49 USC " +
                "§ 14706(c)(1)(A) and (B).",
                DefaultStyles.NormalStyle);

            legalGroup.Margins.Bottom = 20;
        }

        private void AddSignaturesSection()
        {
            var signatureSectionHeight = new FixedHeight(75);
            var signaturesGroup = _section.AddGroup();

            // Shipper Signature #1 - 'payment of charges' section
            signaturesGroup.AddText().AddContent("The carrier shall not make delivery of this shipment without payment of charges and all other lawful fees.");
            var paymentOfChargesTable = signaturesGroup.AddTable();
            paymentOfChargesTable.Margins = new Margins(0, 0, 10, 20);
            var paymentOfChargesRow = paymentOfChargesTable.AddRow();

            paymentOfChargesRow.AddCell(AutoWidth.Instance).AddText().AddContent("Shipper Signature:");
            var paymentOfChargesSignature = paymentOfChargesRow.AddCell(new RelativeWidth(100)).AddRule();
            paymentOfChargesSignature.Pen = new Infragistics.Documents.Reports.Graphics.Pen(new Infragistics.Documents.Reports.Graphics.Color(Color.Black));
            paymentOfChargesSignature.Pen.Width = BLANK_LINE_WIDTH;
            paymentOfChargesSignature.Margins = new VerticalMargins(BLANK_LINE_MARGIN, 0);

            // Other signatures
            var shipperCarrierGroup = signaturesGroup.AddGroup();
            shipperCarrierGroup.Layout = Layout.Horizontal;

            // Shipper Signature #2 - with date
            var shipperGroup = shipperCarrierGroup.AddGroup();
            shipperGroup.Width = new RelativeWidth(50);
            shipperGroup.Height = signatureSectionHeight;
            shipperGroup.Paddings = new Paddings(5);
            shipperGroup.Borders = DefaultStyles.DefaultBorders;
            shipperGroup.AddText().AddContent("Shipper Signature/Date:");

            var shipperGroupSignature = shipperGroup.AddRule();
            shipperGroupSignature.Pen = new Infragistics.Documents.Reports.Graphics.Pen(new Infragistics.Documents.Reports.Graphics.Color(Color.Black));
            shipperGroupSignature.Pen.Width = BLANK_LINE_WIDTH;
            shipperGroupSignature.Margins = new VerticalMargins(BLANK_LINE_MARGIN, 0);
            shipperGroup.AddText().AddContent("This is to certify that the " +
                "above named materials are properly classified, packaged, marked, " +
                "and labeled, and are in proper condition for transportation " +
                "according to the applicable regulations of the DOT.",
                DefaultStyles.SmallStyle);


            // Carrier Signature - with date
            var carrierGroup = shipperCarrierGroup.AddGroup();
            carrierGroup.Width = new RelativeWidth(50);
            carrierGroup.Height = signatureSectionHeight;
            carrierGroup.Paddings = new Paddings(5);
            carrierGroup.Borders = DefaultStyles.DefaultBorders;
            carrierGroup.AddText().AddContent("Carrier Signature/Pickup Date:");

            var carrierGroupSignature = carrierGroup.AddRule();
            carrierGroupSignature.Pen = new Infragistics.Documents.Reports.Graphics.Pen(new Infragistics.Documents.Reports.Graphics.Color(Color.Black));
            carrierGroupSignature.Pen.Width = BLANK_LINE_WIDTH;
            carrierGroupSignature.Margins = new VerticalMargins(BLANK_LINE_MARGIN, 0);

            carrierGroup.AddText().AddContent("Carrier acknowledges receipt " +
                "of packages and required placards. Carrier certifies emergency " +
                "response information was made available and/or carrier has " +
                "the DOT emergency response guidebook or equivalent documentation " +
                "in the vehicle. Property described above is received in good " +
                "order, except as noted.",
                DefaultStyles.SmallStyle);
        }

        private static void AddCompanyLogo(Infragistics.Documents.Reports.Report.IContainer logoContainer)
        {
            var logo = ApplicationSettings.Current.CompanyLogoImagePath;

            if (string.IsNullOrEmpty(logo))
            {
                // Infragistics expects logo to have something
                logoContainer.AddText();
                return;
            }

            Infragistics.Documents.Reports.Graphics.Image pic;
            using (var sourceImg = MediaUtilities.GetImage(logo))
            {
                pic = new Infragistics.Documents.Reports.Graphics.Image(MediaUtilities.PrepareForReport(sourceImg));
            }

            IImage image = logoContainer.AddImage(pic);
            image.KeepRatio = true;
            int pWidth = pic.Width;
            int pHeight = pic.Height;
            double standardHeight = 80;

            // Prevent the image from being too big on the report
            if (pic.Height > 80)
            {
                double percent = standardHeight / pHeight;
                image.Height = new FixedHeight(80);
                double width = percent * pWidth;
                image.Width = new FixedWidth((float)width);
            }
            else if (pic.Width > 185)
                image.Width = new FixedWidth(185);

            image.Margins.Top = 10;
        }

        #endregion

        #region ReportData

        private class ReportData
        {
            public int ShipmentPackageId { get; set; }
            public int BillOfLadingId { get; set; }
            public DateTime DateShipped { get; set; }
            public string CarrierName { get; set; }

            public string CompanyName { get; set; }
            public string CompanyAddress1 { get; set; }
            public string CompanyState { get; set; }
            public string CompanyCity { get; set; }

            public string CompanyZip { get; set; }
            public string CustomerName { get; set; }
            public string CustomerAddress1 { get; set; }
            public string CustomerAddress2 { get; set; }
            public string CustomerCity { get; set; }
            public string CustomerState { get; set; }
            public string CustomerZip { get; set; }

            public List<ContainerLineItem> Shipments { get; set; }

            public bool HasCustomerAddress =>
                !string.IsNullOrEmpty(CustomerAddress1)
                    || !string.IsNullOrEmpty(CustomerCity)
                    || !string.IsNullOrEmpty(CustomerState)
                    || !string.IsNullOrEmpty(CustomerZip);

            public static ReportData From(int billOfLadingId)
            {
                var appSettings = ApplicationSettings.Current;

                using (var dsOrderShipment = new OrderShipmentDataSet())
                {
                    // Load Data
                    dsOrderShipment.EnforceConstraints = false;

                    using (var taBillOfLading = new Data.Datasets.OrderShipmentDataSetTableAdapters.BillOfLadingTableAdapter())
                    {
                        taBillOfLading.FillByBillOfLading(dsOrderShipment.BillOfLading, billOfLadingId);
                    }

                    using (var taBillOfLadingOrder = new Data.Datasets.OrderShipmentDataSetTableAdapters.BillOfLadingOrderTableAdapter())
                    {
                        taBillOfLadingOrder.FillByBillOfLading(dsOrderShipment.BillOfLadingOrder, billOfLadingId);
                    }

                    var billOfLadingRow = dsOrderShipment.BillOfLading.FindByBillOfLadingID(billOfLadingId);

                    using (var taShipment = new Data.Datasets.OrderShipmentDataSetTableAdapters.ShipmentPackageTableAdapter())
                    {
                        taShipment.FillByShipmentPackageID(dsOrderShipment.ShipmentPackage, billOfLadingRow?.ShipmentPackageID ?? -1);
                    }

                    var shipmentRow = dsOrderShipment.ShipmentPackage.FindByShipmentPackageID(billOfLadingRow?.ShipmentPackageID ?? -1);

                    using (var taCustomer = new Data.Datasets.OrderShipmentDataSetTableAdapters.CustomerTableAdapter())
                    {
                        taCustomer.FillByShipmentPackage(dsOrderShipment.Customer, shipmentRow?.ShipmentPackageID ?? -1);
                    }

                    using (var taCustomerAddress = new Data.Datasets.OrderShipmentDataSetTableAdapters.CustomerAddressTableAdapter())
                    {
                        taCustomerAddress.FillByShipmentPackage(dsOrderShipment.CustomerAddress, shipmentRow?.ShipmentPackageID ?? -1);
                    }

                    var customerRow = dsOrderShipment.Customer.FindByCustomerID(shipmentRow?.CustomerID ?? -1);

                    var customerAddressRow = shipmentRow.IsCustomerAddressIDNull()
                        ? null
                        : dsOrderShipment.CustomerAddress.FindByCustomerAddressID(shipmentRow?.CustomerAddressID ?? -1);

                    dsOrderShipment.EnforceConstraints = true;

                    if (billOfLadingRow == null || shipmentRow == null || customerRow == null)
                    {
                        // Cannot find necessary data
                        return null;
                    }

                    // Build report data
                    string customerAddress1;
                    string customerAddress2;
                    string customerCity;
                    string customerState;
                    string customerZip;

                    if (customerAddressRow != null)
                    {
                        customerAddress1 = customerAddressRow.IsAddress1Null() ? string.Empty : customerAddressRow.Address1;
                        customerAddress2 = customerAddressRow.IsAddress2Null() ? string.Empty : customerAddressRow.Address2;
                        customerCity = customerAddressRow.IsCityNull() ? string.Empty : customerAddressRow.City;
                        customerState = customerAddressRow.IsStateNull() ? string.Empty : customerAddressRow.State;
                        customerZip = customerAddressRow.IsZipNull() ? string.Empty : customerAddressRow.Zip;
                    }
                    else
                    {
                        customerAddress1 = customerRow.IsAddress1Null() ? string.Empty : customerRow.Address1;
                        customerAddress2 = customerRow.IsAddress2Null() ? string.Empty : customerRow.Address2;
                        customerCity = customerRow.IsCityNull() ? string.Empty : customerRow.City;
                        customerState = customerRow.IsStateNull() ? string.Empty : customerRow.State;
                        customerZip = customerRow.IsZipNull() ? string.Empty : customerRow.Zip;
                    }

                    return new ReportData
                    {
                        BillOfLadingId = billOfLadingRow.BillOfLadingID,
                        ShipmentPackageId = shipmentRow.ShipmentPackageID,
                        DateShipped = billOfLadingRow.DateCreated,

                        CompanyName = appSettings.CompanyName,
                        CompanyAddress1 = appSettings.CompanyAddress1,
                        CompanyCity = appSettings.CompanyCity,
                        CompanyState = appSettings.CompanyState,
                        CompanyZip = appSettings.CompanyZip,

                        CustomerName = customerRow.Name,
                        CustomerAddress1 = customerAddress1,
                        CustomerAddress2 = customerAddress2,
                        CustomerCity = customerCity,
                        CustomerState = customerState,
                        CustomerZip = customerZip,

                        CarrierName = shipmentRow.ShippingCarrierID,

                        Shipments = appSettings.BillOfLadingIncludeContainers
                            ? GetContainers(dsOrderShipment.BillOfLadingOrder.Select(o => o.OrderID))
                            : new List<ContainerLineItem>()
                    };
                }
            }

            private static List<ContainerLineItem> GetContainers(IEnumerable<int> orderIds)
            {
                if (orderIds == null)
                {
                    return new List<ContainerLineItem>();
                }

                var typeDict = new Dictionary<int, int>();

                using (var taOrderContainer = new Data.Datasets.OrdersDataSetTableAdapters.OrderContainersTableAdapter())
                {
                    foreach (var orderId in orderIds)
                    {
                        using (var dtOrderContainer = new OrdersDataSet.OrderContainersDataTable())
                        {
                            taOrderContainer.FillByOrder(dtOrderContainer, orderId);
                            foreach (var containerRow in dtOrderContainer)
                            {
                                var shipmentPackageTypeId = containerRow.ShipmentPackageTypeID;
                                if (typeDict.ContainsKey(shipmentPackageTypeId))
                                {
                                    typeDict[shipmentPackageTypeId] += 1;
                                }
                                else
                                {
                                    typeDict[shipmentPackageTypeId] = 1;
                                }
                            }
                        }
                    }
                }

                var lineItems = new List<ContainerLineItem>();
                using (var dtShipmentPackageType = new OrdersDataSet.ShipmentPackageTypeDataTable())
                {
                    using (var taContainerType = new Data.Datasets.OrdersDataSetTableAdapters.ShipmentPackageTypeTableAdapter())
                    {
                        taContainerType.Fill(dtShipmentPackageType);
                    }

                    foreach (var idQuantityPair in typeDict)
                    {
                        var typeRow = dtShipmentPackageType.FindByShipmentPackageTypeID(idQuantityPair.Key);

                        if (typeRow == null)
                        {
                            continue;
                        }

                        lineItems.Add(new ContainerLineItem
                        {
                            Description = typeRow.Name,
                            Quantity = idQuantityPair.Value
                        });
                    }
                }

                return lineItems;
            }
        }

        #endregion

        #region ContainerLineItem

        private class ContainerLineItem
        {
            public string Description { get; set; }

            public int Quantity { get; set; }
        }

        #endregion
    }
}