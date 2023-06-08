using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DWOS.Data;
using DWOS.Data.Coc;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.COCDatasetTableAdapters;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Reports;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;

namespace DWOS.Reports
{
    using Data.Order;
    using Infragistics.Documents.Reports.Graphics;
    using System.Threading;
    using Color = System.Drawing.Color;
    using Font = System.Drawing.Font;
    using FontStyle = System.Drawing.FontStyle;
    using Image = System.Drawing.Image;

    public class COCReport : Report
    {
        #region Fields

        private readonly COCDataset _cocDataset;

        #endregion

        #region Properties

        public override string Title => "COC";

        protected override PageOrientation ReportPageOrientation => PageOrientation.Portrait;

        private int OrderID => _cocDataset.COC[0].OrderID;

        protected override string FooterIdentifier => _cocDataset.COC[0].OrderID.ToString();

        private bool HasMedia => _cocDataset.Part_Media.Count > 0;

        // COC Report puts barcodes at the top of the footer.
        // Space required was determined through trial & error to match what it was before
        // moving barcodes to the footer.
        protected override float AdditionalFooterSpace => 68;

        #endregion

        #region Methods

        public COCReport(COCDataset cocDataset) { this._cocDataset = cocDataset; }

        public COCReport(int cocID)
        {
            _log.Info("Creating coc report.");

            Data.Datasets.COCDatasetTableAdapters.COCTableAdapter taCOC = null;
            COCPartTableAdapter taCOCPart = null;
            UsersTableAdapter taUsers = null;
            Part_MediaTableAdapter taPartMedia = null;
            Data.Datasets.COCDatasetTableAdapters.MediaTableAdapter taMedia = null;

            try
            {
                this._cocDataset = new COCDataset();
                taCOC = new Data.Datasets.COCDatasetTableAdapters.COCTableAdapter();
                taCOCPart = new COCPartTableAdapter();
                taUsers = new UsersTableAdapter();
                taMedia = new Data.Datasets.COCDatasetTableAdapters.MediaTableAdapter();
                taPartMedia = new Part_MediaTableAdapter();

                this._cocDataset.EnforceConstraints = false;
                this._cocDataset.COC.BeginLoadData();
                this._cocDataset.COCPart.BeginLoadData();
                this._cocDataset.Users.BeginLoadData();
                this._cocDataset.Media.BeginLoadData();

                taCOC.FillBy(this._cocDataset.COC, cocID);
                taCOCPart.FillBy(this._cocDataset.COCPart, cocID);
                taUsers.Fill(this._cocDataset.Users);

                // Calling Fill() populates the entire table; only signatures are needed.
                taMedia.FillSignatureMedia(this._cocDataset.Media);

                if(this._cocDataset.COCPart.Count > 0)
                {
                    int partId = this._cocDataset.COCPart[0].PartID;
                    taPartMedia.FillDefaultByPart(this._cocDataset.Part_Media, partId);
                }

                this._cocDataset.COC.EndLoadData();
                this._cocDataset.COCPart.EndLoadData();
                this._cocDataset.Users.EndLoadData();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error creating COC.";
                _log.Error(exc, errorMsg);
            }
            finally
            {
                _cocDataset?.Dispose();
                taCOC?.Dispose();
                taCOCPart?.Dispose();
                taUsers?.Dispose();
                taPartMedia?.Dispose();
                taMedia?.Dispose();
            }
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            SetupReportInfo();
            _section.PageMargins.All = 20;

            AddHeader("CERTIFICATE OF CONFORMANCE", "Work Order: ", OrderID, true, null, ReportType.BatchOrder);
            AddCustomerSection();
            AddPartSection();
            AddProcessSection();
            AddSignature();
            AddBarCodeCommands();
        }

        private void AddCustomerSection()
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
                var approvalDateContainer = pcell2.AddContainer("reqDate");

                orderContainer.Alignment.Vertical = Alignment.Top;
                orderContainer.Alignment.Horizontal = Alignment.Left;
                orderContainer.Paddings.Left = 5;
                orderContainer.Paddings.Top = 4;
                orderContainer.Borders = DefaultStyles.DefaultBorders;
                orderContainer.Height = new FixedHeight(containerHeight);

                approvalDateContainer.Paddings.Left = 5;
                approvalDateContainer.Paddings.Top = 4;
                approvalDateContainer.Height = new FixedHeight(containerHeight);
                approvalDateContainer.Alignment.Vertical = Alignment.Top;
                approvalDateContainer.Alignment.Horizontal = Alignment.Left;
                approvalDateContainer.Borders = DefaultStyles.DefaultBorders;
                approvalDateContainer.Margins.Left = 5;

                AddApprovalDateTable(approvalDateContainer);

                AddOrderTable(orderContainer);

                // Add the 'VOID' decorations if:
                // - This is not the latest COC, or
                // - This is a new COC.
                bool showVoidDecorations;
                using (var taCOC = new Data.Datasets.COCDatasetTableAdapters.COCTableAdapter())
                {
                    var mostRecentCocId = taCOC.GetMostRecentCOCIDByOrderID(OrderID).GetValueOrDefault();
                    showVoidDecorations = _cocDataset.COC.Rows.Count > 0 && mostRecentCocId != _cocDataset.COC[0].COCID;
                }

                if (showVoidDecorations)
                {
                    var decorationTop = _section.AddDecoration();
                    decorationTop.MasterRange = MasterRange.All;
                    decorationTop.Rear = false;
                    var decorationTopText = decorationTop.AddText(200, 50, 0);
                    decorationTopText.Style = new Style(new Infragistics.Documents.Reports.Graphics.Font("Verdana", 50), Brushes.Red);
                    decorationTopText.Width = new RelativeWidth(35);
                    decorationTopText.Borders = new Borders(new Pen(Colors.Red, 3, DashStyle.Solid), 10);
                    decorationTopText.AddContent("  VOID");

                    var decorationBottom = _section.AddDecoration();
                    decorationBottom.MasterRange = MasterRange.All;
                    decorationBottom.Rear = false;
                    var decorationBottomText = decorationBottom.AddText(200, 600, 0);
                    decorationBottomText.Style = new Style(new Infragistics.Documents.Reports.Graphics.Font("Verdana", 50), Brushes.Red);
                    decorationBottomText.Width = new RelativeWidth(35);
                    decorationBottomText.Borders = new Borders(new Pen(Colors.Red, 3, DashStyle.Solid), 10);
                    decorationBottomText.AddContent("  VOID");
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding customer section to report.");
            }
        }

        private void AddCustomerTable(Infragistics.Documents.Reports.Report.IContainer customerContainer)
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

                using (CustomerTableAdapter taCustomer = new CustomerTableAdapter())
                {
                    taCustomer.FillByOrderID(dsCustomer.Customer, this.OrderID);
                }

                using (var taCustomerAddress = new Data.Datasets.CustomersDatasetTableAdapters.CustomerAddressTableAdapter())
                {
                    taCustomerAddress.FillByOrderID(dsCustomer.CustomerAddress, OrderID);
                }

                using (var taCountry = new CountryTableAdapter())
                {
                    taCountry.Fill(dsCustomer.Country);
                }

                var companyTable = customerContainer.AddTable();
                var companyTitle = companyTable.AddRow();
                companyTitle.AddCell(100, "Customer:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                var companyName = companyTable.AddRow();

                if (dsCustomer.Customer.Rows.Count > 0)
                {
                    var customer = dsCustomer.Customer[0];

                    companyName.AddCell(100, "   " + customer.Name.TrimToMaxLength(maxCustomerLength, ellipsis),
                        DefaultStyles.BoldMediumStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                    if (customer.HasBillingAddress)
                    {
                        if (!customer.IsAddress1Null() && customer.Address1 != customer.Name)
                        {
                            var companyAddress = companyTable.AddRow();
                            companyAddress.AddCell(50, "   " + customer.Address1.TrimToMaxLength(maxLength, ellipsis),
                                DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                        }

                        var addressRow = companyTable.AddRow();
                        var zip = customer.IsZipNull() ? "" : customer.Zip;
                        var addressLastRow = (customer.IsCityNull() ? "-" : customer.City) + ", " + (customer.IsStateNull() ? "-" : customer.State) + " " + zip;

                        if (customer.CountryID != ApplicationSettings.Current.CompanyCountry)
                        {
                            addressLastRow += $" {customer.CountryRow.Name}";
                        }

                        addressRow.AddCell(50, "   " + addressLastRow.TrimToMaxLength(maxLength, ellipsis),
                            DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                    }

                    if (customer.GetCustomerAddressRows().Length > 0)
                    {
                        var customerAddress = customer.GetCustomerAddressRows().First();

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
                        var shipLastLine = shipCity + ", " + shipState + " " + shipZip;

                        if (customerAddress.CountryID != ApplicationSettings.Current.CompanyCountry)
                        {
                            shipLastLine += $" {customerAddress.CountryRow.Name}";
                        }

                        shippingAddressRow.AddCell(50, "   " + shipLastLine.TrimToMaxLength(maxLength, ellipsis),
                            DefaultStyles.NormalStyle,
                            new TextAlignment(Alignment.Left, Alignment.Middle));
                    }
                }
            }
            finally
            {
                dsCustomer?.Dispose();
            }
        }

        private void AddApprovalDateTable(Infragistics.Documents.Reports.Report.IContainer approvalDateContainer)
        {
            var approvalDateTable = approvalDateContainer.AddTable();
            var approvalDateRow = approvalDateTable.AddRow();
            approvalDateRow.Height = new FixedHeight(20);

            var approvalDateCell = approvalDateRow.AddCell(40, "Approval Date:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            approvalDateCell.Height = new FixedHeight(20);
            var approvalDateValueRow = approvalDateTable.AddRow();
            approvalDateValueRow.Height = new FixedHeight(20);

            var superSized = new Style(new Infragistics.Documents.Reports.Graphics.Font(new Font(DefaultStyles.RedXLargeStyle.Font.Name, DefaultStyles.RedXLargeStyle.Font.Size - 3, FontStyle.Bold)), DefaultStyles.BlackXLargeStyle.Brush);
            ITableCell approvalDateValueCell = null;

            if (this._cocDataset.COC.Rows.Count > 0)
                approvalDateValueCell = approvalDateValueRow.AddCell(40, this._cocDataset.COC[0].DateCertified.ToShortDateString(), superSized, new TextAlignment(Alignment.Center, Alignment.Middle));
            else
                approvalDateValueCell = approvalDateValueRow.AddCell(40, "Unknown", superSized, new TextAlignment(Alignment.Center, Alignment.Middle));

            approvalDateValueCell.Height = new FixedHeight(20);
        }

        private void AddOrderTable(Infragistics.Documents.Reports.Report.IContainer orderContainer)
        {
            const int orderLeftRelativeWidth = 42;
            const int orderRightRelativeWidth = 58;
            const int orderRightColumnLength = 19;

            Data.Datasets.COCDatasetTableAdapters.COCTableAdapter taCOC = null;

            try
            {
                taCOC = new Data.Datasets.COCDatasetTableAdapters.COCTableAdapter();
                var poValue = taCOC.GetPurcharseOrder(OrderID);
                var formattedPurchaseOrder = FormatValue(String.IsNullOrWhiteSpace(poValue) ? "None" : poValue, orderRightColumnLength);

                var customerWoValue = taCOC.GetCustomerWO(OrderID);
                var formattedCustomerWo = FormatValue(String.IsNullOrWhiteSpace(customerWoValue) ? "None" : customerWoValue, orderRightColumnLength);

                var orderTable = orderContainer.AddTable();
                orderTable.AddRow().AddCell(100, "Order:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                var workOrderRow = orderTable.AddRow();
                var workOrderCell = workOrderRow.AddCell(orderLeftRelativeWidth, "   Work Order:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                workOrderCell.Margins = new HorizontalMargins(0, 0);
                var orderID = FormatValue(OrderID.ToString(), orderRightColumnLength);
                workOrderRow.AddCell(orderRightRelativeWidth, orderID, DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                var purchaseOrderRow = orderTable.AddRow();
                var purchaseOrderCell = purchaseOrderRow.AddCell(orderLeftRelativeWidth, "   Purchase Order:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                purchaseOrderCell.Margins = new HorizontalMargins(0, 0);

                purchaseOrderRow.AddCell(orderRightRelativeWidth, formattedPurchaseOrder, DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                var customerWoRow = orderTable.AddRow();
                var customerWoCell = customerWoRow.AddCell(orderLeftRelativeWidth, "   Customer WO:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                customerWoCell.Margins = new HorizontalMargins(0, 0);

                customerWoRow.AddCell(orderRightRelativeWidth, formattedCustomerWo, DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                var cocRow = orderTable.AddRow();
                var cocCell = cocRow.AddCell(orderLeftRelativeWidth, "   COC Number:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                cocCell.Margins = new HorizontalMargins(0, 0);

                if (this._cocDataset.COC.Rows.Count > 0)
                {
                    var coc = FormatValue(this._cocDataset.COC[0].COCID.ToString(), orderRightColumnLength);
                    cocRow.AddCell(orderRightRelativeWidth, coc, DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                }
            }
            finally
            {
                taCOC?.Dispose();
            }
        }

        /// <summary>
        /// Formats the value of a string so that it fits the supplied length.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="maxLength">The length.</param>
        /// <returns>Substring with ellipse (…) if needed</returns>
        private static string FormatValue(string value, int maxLength)
        {
            string formattedValue;
            if (value.Length <= maxLength)
            {
                formattedValue = value;
            }
            else
            {
                formattedValue = value.Substring(0, maxLength - 1) + "…";
            }

            return formattedValue;
        }

        private void AddPartSection()
        {
            const int orderWidthPercentage = 70;
            const int imgWidthPercentage = 30;

            try
            {
                bool isMaterialVisible;
                bool isRevisionVisible;
                using (var taField = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
                {
                    using (var dtPartFields = taField.GetByCategory("Part"))
                    {
                        var materialField = dtPartFields.FirstOrDefault(field => field.Name == "Material");
                        isMaterialVisible = materialField != null && materialField.IsVisible;

                        var revisionField = dtPartFields.FirstOrDefault(field => field.Name == "Part Rev.");
                        isRevisionVisible = revisionField != null && revisionField.IsVisible;
                    }
                }

                var headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Margins = new Margins(5, 5, 3, 0);
                headerGroup.Borders = DefaultStyles.DefaultBorders;
                headerGroup.Background = DefaultStyles.DefaultBackground;

                var orderContainer = headerGroup.AddContainer("order");
                orderContainer.Width = new RelativeWidth(orderWidthPercentage);
                orderContainer.Paddings.All = 5;
                orderContainer.Margins.Right = 5;

                //Add Part info table
                var partNumber = string.Empty;

                if (this._cocDataset.COCPart[0].IsRevisionNull() || !isRevisionVisible)
                {
                    partNumber = this._cocDataset.COCPart[0].Name;
                }
                else
                {
                    partNumber = $"{this._cocDataset.COCPart[0].Name} Rev. {this._cocDataset.COCPart[0].Revision}";
                }

                var orderTable = orderContainer.AddTable();
                orderTable.AddRow().AddCells(DefaultStyles.BlueLargeStyle, new SolidColorBrush(new Infragistics.Documents.Reports.Graphics.Color(System.Drawing.Color.WhiteSmoke)), null, 0, TextAlignment.Left, "Part Information:");
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Part Number:", partNumber);

                if (isMaterialVisible)
                {
                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Material:", this._cocDataset.COCPart[0].IsMaterialNull() ? "Unknown" : this._cocDataset.COCPart[0].Material);
                }

                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Quantity:", this._cocDataset.COC[0].PartQuantity.ToString());

                if (!this._cocDataset.COCPart[0].IsDescriptionNull())
                {
                    orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Description:", this._cocDataset.COCPart[0].Description);
                }

                var qualText = orderContainer.AddText();
                qualText.Width = new RelativeWidth(100);
                qualText.Alignment = TextAlignment.Left;
                qualText.Alignment.Horizontal = Alignment.Left;
                qualText.Style = DefaultStyles.BlueLargeStyle;
                qualText.Paddings.Top = 15;
                qualText.Margins.Right = 5;
                qualText.AddContent("Quality:");

                var warText = orderContainer.AddText();
                warText.Width = new RelativeWidth(100);
                warText.Alignment = TextAlignment.Left;
                warText.Alignment.Horizontal = Alignment.Justify;
                warText.Style = DefaultStyles.NormalStyle;
                warText.Paddings.Vertical = 5;
                warText.Margins.Right = 5;

                //if no manufacture specified for the part then use default warranty
                if (this._cocDataset.COCPart[0].IsManufacturerIDNull() || String.IsNullOrWhiteSpace(this._cocDataset.COCPart[0].ManufacturerID))
                {
                    var warranty = ApplicationSettings.Current.COCWarranty;
                    warText.AddRichContent(warranty == null || String.IsNullOrEmpty(warranty) ? "" : warranty.ToString());
                    warText.AddLineBreak();
                }
                else //else use manufactures defined warranty
                {
                    var taPart = new COCPartTableAdapter();
                    var warranty = taPart.GetManufacturerWarranty(this._cocDataset.COCPart[0].ManufacturerID);
                    warText.AddRichContent(warranty == null || String.IsNullOrEmpty(warranty) ? "" : warranty.ToString());
                }

                //Add Part Image
                if (HasMedia || ApplicationSettings.Current.UseReportPlaceholderImage)
                {
                    var imgContainer = headerGroup.AddContainer("partImage");
                    imgContainer.Alignment.Vertical = Alignment.Middle;
                    imgContainer.Alignment.Horizontal = Alignment.Right;
                    imgContainer.Paddings.All = 5;
                    imgContainer.Paddings.Right = 5;
                    imgContainer.Margins.Left = 5;
                    imgContainer.Margins.Right = 8;
                    imgContainer.Width = new RelativeWidth(imgWidthPercentage);

                    Image img = null;

                    if (HasMedia)
                    {
                        var partMediaRow = this._cocDataset.Part_Media
                            .FirstOrDefault(media => !media.IsDefaultMediaNull() && media.DefaultMedia);

                        string fileExtension;
                        using (var taPartMedia = new Part_MediaTableAdapter())
                        {
                            fileExtension = taPartMedia.GetFileExtension(partMediaRow.MediaID);
                        }

                        img = MediaUtilities.GetImage(partMediaRow.MediaID, fileExtension);
                    }
                    else if (System.IO.File.Exists(ApplicationSettings.Current.ReportPlaceholderImagePath))
                    {
                        img = MediaUtilities.GetImage(System.IO.File.ReadAllBytes(ApplicationSettings.Current.ReportPlaceholderImagePath));
                    }
                    else
                    {
                        img = Data.Properties.Resources.NoImage;
                    }

                    var image = imgContainer.AddImage(new Infragistics.Documents.Reports.Graphics.Image(img));
                    image.KeepRatio = true;

                    var maximumSize = new System.Drawing.Size(150, 150);
                    var imageSize = MediaUtilities.Resize(img.Size, maximumSize);
                    image.Width = new FixedWidth(imageSize.Width);
                    image.Height = new FixedHeight(imageSize.Height);
                }
                else
                {
                    // Resize quality statement but not part info
                    orderContainer.Width = new RelativeWidth(100);
                    orderTable.Width = new RelativeWidth(45);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding part section to COC report.");
            }
        }

        private void AddProcessSection()
        {
            try
            {
                IGroup group = _section.AddGroup();
                group.Layout = Layout.Horizontal;
                group.Margins = new Margins(5, 5, 3, 0); ;
                group.Paddings.All = 5;
                group.Borders = DefaultStyles.DefaultBorders;
                group.Background = DefaultStyles.DefaultBackground;

                IGroup processGroup = group.AddGroup();
                processGroup.Width = new RelativeWidth(90);
                processGroup.Margins.Right = 5;

                IText title = processGroup.AddText();
                title.Width = new RelativeWidth(100);
                title.Alignment = TextAlignment.Left;
                title.Style = DefaultStyles.NormalStyle;

                title.AddContent("Processes:", DefaultStyles.BlueLargeStyle);
                title.AddLineBreak();

                IText processText = processGroup.AddText();
                processText.Alignment = TextAlignment.Left;
                processText.Style = DefaultStyles.NormalStyle;

                string cocInfo = this._cocDataset.COC[0].COCInfo;

                if(this._cocDataset.COC[0].IsCompressed)
                    cocInfo = cocInfo.DecompressString();

                if(String.IsNullOrEmpty(cocInfo))
                    cocInfo = "No Info";
                else
                {
                    cocInfo = cocInfo.Replace("&edsp;", "&nbsp;");
                    cocInfo = cocInfo.Replace("\"8pt\"", "\"8\""); //added 06.26.09 found the pt to cause an issue
                }

                processText.AddRichContent(cocInfo, false);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error adding process section to COC report.");
            }
        }

        private void AddSignature()
        {
            try
            {
                var qaUser = _cocDataset.COC.First().UsersRow;

                var signatureData = SignatureData.From(qaUser) ?? SignatureData.Default();

                ITable headerTable = _section.AddTable();
                headerTable.Margins = new Margins(5, 5, 0, 0);

                //Add auth signature text, image of signature, and date
                ITableRow row = headerTable.AddRow();
                ITableCell signatureContainer = row.AddCell();
                ITableCell signatureImgContainer = row.AddCell();
                ITableCell spacerContainer = row.AddCell();

                signatureContainer.Width = new RelativeWidth(30);
                signatureContainer.Alignment.Horizontal = Alignment.Right;
                signatureContainer.Alignment.Vertical = Alignment.Middle;

                signatureImgContainer.Width = new RelativeWidth(70);
                signatureImgContainer.Height = new FixedHeight(75);
                signatureImgContainer.Alignment.Horizontal = Alignment.Left;
                signatureImgContainer.Alignment.Vertical = Alignment.Middle;


                spacerContainer.Width = new RelativeWidth(50);
                spacerContainer.Alignment.Horizontal = Alignment.Right;
                spacerContainer.Alignment.Vertical = Alignment.Middle;

                //Add Authorized Signature Text
                IText companyText = signatureContainer.AddText();
                companyText.Alignment.Horizontal = Alignment.Left;
                companyText.Alignment.Vertical = Alignment.Middle;
                companyText.Height = new FixedHeight(15);
                companyText.Style = DefaultStyles.NormalStyle;
                companyText.Paddings.Horizontal = 5;
                companyText.AddContent("Authorized Signature:", DefaultStyles.NormalStyle);
                companyText.AddLineBreak();

                //Add Signature Image
                IGroup group = signatureImgContainer.AddGroup();
                group.Alignment = Infragistics.Documents.Reports.Report.ContentAlignment.Left;
                group.Layout = Layout.Vertical;

                if (signatureData.Image != null)
                {
                    IImage image = group.AddImage(signatureData.Image);
                    var maxSize = new System.Drawing.Size(signatureData.Image.Width, 40);
                    var imgSize = MediaUtilities.Resize(new System.Drawing.Size(signatureData.Image.Width, signatureData.Image.Height), maxSize);
                    image.Height = new FixedHeight(imgSize.Height);
                    image.Width = new FixedWidth(imgSize.Width);
                    image.KeepRatio = true;
                }

                //Add Signature Line
                IRule rule = group.AddRule();
                rule.Pen = new Infragistics.Documents.Reports.Graphics.Pen(new Infragistics.Documents.Reports.Graphics.Color(Color.Black));
                rule.Pen.Width = 1;
                rule.Margins = new VerticalMargins(2, 2);

                //Add User Name
                IText userName = group.AddText();
                userName.Style = DefaultStyles.NormalStyle;
                string displayName = signatureData.Name;
                if (!String.IsNullOrWhiteSpace(signatureData.Title))
                    displayName += ", " + signatureData.Title;
                userName.AddContent(displayName);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error creating signature on COC report.");
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

                var group = _footer.AddGroup(5, 0);
                group.Margins.All = 5;
                group.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);
                var table = group.AddTable();
                table.Margins.All = 5;
                table.KeepSolid = true;

                ITableRow row = table.AddRow();

                //Add Work Order Barcode
                var workOrderCell = row.AddCell();
                workOrderCell.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);
                IGroup checkInGroup = workOrderCell.AddGroup();
                checkInGroup.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);

                IText checkInText = checkInGroup.AddText();
                checkInText.AddContent("Work Order");
                checkInText.Alignment = new TextAlignment(Alignment.Center, Alignment.Bottom);

                IImage imageOCIBarCode = checkInGroup.AddImage(new Infragistics.Documents.Reports.Graphics.Image(base.CreateOrderBarcode(OrderID, false)));
                imageOCIBarCode.KeepRatio = true;
                imageOCIBarCode.Margins.All = 5;

                //Add Shipping Barcode
                var shippingCell = row.AddCell();
                shippingCell.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);
                IGroup shipGroup = shippingCell.AddGroup();
                shipGroup.Alignment = new ContentAlignment(Alignment.Center, Alignment.Bottom);

                IText shipText = shipGroup.AddText();
                shipText.AddContent("Shipping");
                shipText.Alignment = new TextAlignment(Alignment.Center, Alignment.Bottom);

                IImage imageShipBarCode = shipGroup.AddImage(new Infragistics.Documents.Reports.Graphics.Image(base.CreateOrderActionBarcode(OrderID)));
                imageShipBarCode.KeepRatio = true;
                imageShipBarCode.Margins.All = 5;
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error adding barcodes to COC report.");
            }
        }

        #endregion
    }

    public class BulkCOCReport : Report
    {
        #region Properties

        public int BulkCOCID
        {
            get;
            private set;
        }

        public override string Title
        {
            get
            {
                return "COC";
            }
        }

        protected override PageOrientation ReportPageOrientation
        {
            get
            {
                return PageOrientation.Portrait;
            }
        }

        #endregion

        #region Methods

        public BulkCOCReport(int bulkCOCID)
        {
            BulkCOCID = bulkCOCID;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            COCDataset dsCOC = null;
            CustomersDataset dsCustomer = null;

            Data.Datasets.COCDatasetTableAdapters.BulkCOCTableAdapter taBulkCOC = null;
            CustomerTableAdapter taCustomer = null;
            Data.Datasets.COCDatasetTableAdapters.OrderSummaryTableAdapter taOrderSummary = null;
            Data.Datasets.COCDatasetTableAdapters.OrderProcessesTableAdapter taOrderProcesses = null;
            UsersTableAdapter taUsers = null;
            Data.Datasets.COCDatasetTableAdapters.MediaTableAdapter taMedia = null;

            try
            {
                dsCOC = new COCDataset()
                {
                    EnforceConstraints = false
                };

                dsCustomer = new CustomersDataset()
                {
                    EnforceConstraints = false
                };

                taCustomer = new CustomerTableAdapter();
                taUsers = new UsersTableAdapter();
                taMedia = new Data.Datasets.COCDatasetTableAdapters.MediaTableAdapter();
                taOrderSummary = new Data.Datasets.COCDatasetTableAdapters.OrderSummaryTableAdapter();
                taOrderProcesses = new Data.Datasets.COCDatasetTableAdapters.OrderProcessesTableAdapter();
                taBulkCOC = new Data.Datasets.COCDatasetTableAdapters.BulkCOCTableAdapter();

                taBulkCOC.FillBy(dsCOC.BulkCOC, BulkCOCID);

                if (dsCOC.BulkCOC.Count == 0)
                {
                    throw new ApplicationException("Bulk Certificate was not found.");
                }

                var bulkCOC = dsCOC.BulkCOC.First();

                taCustomer.FillByShipment(dsCustomer.Customer, bulkCOC.ShipmentPackageID);

                using (var taCountry = new CountryTableAdapter())
                {
                    taCountry.Fill(dsCustomer.Country);
                }

                taUsers.Fill(dsCOC.Users);

                taMedia.FillSignatureMedia(dsCOC.Media);

                taOrderSummary.FillByBulkCOC(dsCOC.OrderSummary, bulkCOC.BulkCOCID);

                taOrderProcesses.FillByBulkCOC(dsCOC.OrderProcesses, bulkCOC.BulkCOCID);

                SetupReportInfo();
                AddHeader("CERTIFICATE", false);

                AddCustomerSection(bulkCOC, dsCustomer.Customer.FirstOrDefault());
                AddOrderSection(dsCOC.OrderSummary);
                AddSignature(dsCOC.Users.FirstOrDefault(i => i.UserID == bulkCOC.QAUser));
            }
            finally
            {
                dsCOC?.Dispose();
                dsCustomer?.Dispose();
                taBulkCOC?.Dispose();
                taCustomer?.Dispose();
                taOrderSummary?.Dispose();
                taOrderProcesses?.Dispose();
                taUsers?.Dispose();
                taMedia?.Dispose();
            }
        }


        private void AddCustomerSection(COCDataset.BulkCOCRow bulkCOC, CustomersDataset.CustomerRow customer)
        {
            const int height = 62;

            try
            {
                var superSized = new Style(new Infragistics.Documents.Reports.Graphics.Font(new Font(DefaultStyles.RedXLargeStyle.Font.Name, DefaultStyles.RedXLargeStyle.Font.Size - 3, FontStyle.Bold)), DefaultStyles.BlackXLargeStyle.Brush);

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

                if (customer != null)
                {
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
                        var addressLastLine = (customer.IsCityNull() ? "-" : customer.City) + " " + (customer.IsStateNull() ? "-" : customer.State) + ", " + zip;

                        if (customer.CountryID != ApplicationSettings.Current.CompanyCountry)
                        {
                            addressLastLine += $" {customer.CountryRow.Name}";
                        }

                        addressRow.AddCell(50, "   " + addressLastLine, DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                    }
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
                var approvalDateContainer = pcell2.AddContainer("reqDate");

                orderContainer.Alignment.Vertical = Alignment.Top;
                orderContainer.Alignment.Horizontal = Alignment.Left;
                orderContainer.Paddings.Left = 5;
                orderContainer.Paddings.Top = 4;
                orderContainer.Borders = DefaultStyles.DefaultBorders;
                orderContainer.Height = new FixedHeight(height);

                approvalDateContainer.Paddings.Left = 5;
                approvalDateContainer.Paddings.Top = 4;
                approvalDateContainer.Height = new FixedHeight(height);
                approvalDateContainer.Alignment.Vertical = Alignment.Top;
                approvalDateContainer.Alignment.Horizontal = Alignment.Left;
                approvalDateContainer.Borders = DefaultStyles.DefaultBorders;
                approvalDateContainer.Margins.Left = 5;

                var reqDateTable = approvalDateContainer.AddTable();
                var row5 = reqDateTable.AddRow();
                row5.Height = new FixedHeight(20);

                var cell1 = row5.AddCell(40, "Approval Date:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                cell1.Height = new FixedHeight(20);
                var row6 = reqDateTable.AddRow();
                row6.Height = new FixedHeight(20);

                ITableCell cell2 = row6.AddCell(40, bulkCOC.DateCertified.ToShortDateString(), superSized, new TextAlignment(Alignment.Center, Alignment.Middle));

                cell2.Height = new FixedHeight(20);

                var certificateTable = orderContainer.AddTable();
                var orderCell = certificateTable.AddRow().AddCell(100, "Certificate:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                const int orderLeftRelativeWidth = 42;
                const int orderRightRelativeWidth = 58;

                var cocIDRow = certificateTable.AddRow();
                var cell6 = cocIDRow.AddCell(orderLeftRelativeWidth, "   COC Number:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                cell6.Margins = new HorizontalMargins(0, 0);
                cocIDRow.AddCell(orderRightRelativeWidth, BulkCOCID.ToString(), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                var shipmentIDRow = certificateTable.AddRow();
                var cell7 = shipmentIDRow.AddCell(orderLeftRelativeWidth, "   Shipment:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                cell7.Margins = new HorizontalMargins(0, 0);
                shipmentIDRow.AddCell(orderRightRelativeWidth, bulkCOC.ShipmentPackageID.ToString(), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                var shippedDate = GetShippedDate(bulkCOC);

                var shipmentDateRow = certificateTable.AddRow();
                var cell8 = shipmentDateRow.AddCell(orderLeftRelativeWidth, "   Shipment Date:", DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                cell8.Margins = new HorizontalMargins(0, 0);
                shipmentDateRow.AddCell(orderRightRelativeWidth, shippedDate.ToShortDateString(), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding customer section to report.");
            }
        }

        private void AddOrderSection(IEnumerable<COCDataset.OrderSummaryRow> orders)
        {
            const int qtyWidth = 3;
            const int weightWidth = 4;
            const int informationWidth = 23;
            const int containerWidth = 4;

            const int descriptionLabelWidth = 5;
            const int descriptionContentWidth = 19;

            try
            {
                var orderedOrders = EntriesFrom(orders) ?? Enumerable.Empty<BulkCOCEntry>();

                var headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Margins.All = 5;
                headerGroup.Borders = DefaultStyles.DefaultBorders;
                headerGroup.Background = DefaultStyles.DefaultBackground;

                var headerTable = headerGroup.AddTable();
                headerTable.Width = new RelativeWidth(100);
                headerTable.CreateTableHeaderCell(informationWidth).AddText("Order Information");
                headerTable.CreateTableHeaderCell(containerWidth).AddText("Containers");
                headerTable.CreateTableHeaderCell(weightWidth).AddText("Gross Weight (lbs.)");
                headerTable.CreateTableHeaderCell(qtyWidth).AddText("Qty");

                var totalGrossWeight = 0M;
                var totalQty = 0;
                foreach (var order in orderedOrders)
                {
                    var orderRow = headerTable.AddRow();

                    // Order Information
                    var descriptionTable = orderRow
                        .CreateTableCell(informationWidth)
                        .AddTable();

                    var partRow = descriptionTable.AddRow();
                    partRow.AddCell(descriptionLabelWidth, "Part: ", DefaultStyles.NormalStyle, TextAlignment.Left);
                    partRow.AddCell(descriptionContentWidth, order.PartName, DefaultStyles.NormalStyle, TextAlignment.Left);

                    var poRow = descriptionTable.AddRow();
                    poRow.AddCell(descriptionLabelWidth, "Purchase Order: ", DefaultStyles.NormalStyle, TextAlignment.Left);
                    poRow.AddCell(descriptionContentWidth, order.PurchaseOrder, DefaultStyles.NormalStyle, TextAlignment.Left);

                    var customerWORow = descriptionTable.AddRow();
                    customerWORow.AddCell(descriptionLabelWidth, "Customer WO: ", DefaultStyles.NormalStyle, TextAlignment.Left);
                    customerWORow.AddCell(descriptionContentWidth, order.CustomerWO, DefaultStyles.NormalStyle, TextAlignment.Left);

                    var processesRow = descriptionTable.AddRow();
                    processesRow.AddCell(descriptionLabelWidth, "Processes: ", DefaultStyles.NormalStyle, TextAlignment.Left);

                    var processDescriptionCell = processesRow.AddCell();
                    processDescriptionCell.Width = new RelativeWidth(descriptionContentWidth);
                    processDescriptionCell.Paddings.Top = 1f;
                    processDescriptionCell.AddText().AddRichContent(order.ProcessRichContent);

                    // Containers
                    orderRow.CreateTableCell(containerWidth)
                        .AddText(order.ContainerCount.ToString(), DefaultStyles.NormalStyle, TextAlignment.Center);

                    // Gross Weight
                    orderRow.CreateTableCell(weightWidth)
                        .AddText(order.GrossWeight.ToString("N0"), DefaultStyles.NormalStyle, TextAlignment.Center);

                    // Quantity
                    orderRow.CreateTableCell(qtyWidth)
                        .AddText(order.PartQuantity?.ToString() ?? "Unknown", DefaultStyles.NormalStyle, TextAlignment.Center);

                    totalGrossWeight += order.GrossWeight;
                    totalQty += order.PartQuantity ?? 0;
                }

                // Totals
                var totalRow = headerTable.AddRow();
                totalRow.CreateTableCell(informationWidth + containerWidth)
                    .AddText("Total", DefaultStyles.BoldMediumStyle, TextAlignment.Right);

                totalRow.CreateTableCell(weightWidth)
                    .AddText(totalGrossWeight.ToString("N0"), DefaultStyles.NormalStyle, TextAlignment.Center);

                totalRow.CreateTableCell(qtyWidth)
                    .AddText(totalQty.ToString(), DefaultStyles.NormalStyle, TextAlignment.Center);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding order section to report.");
            }
        }

        private static string GetProcessRichContent(COCDataset.OrderSummaryRow order)
        {
            const string indent = "&nbsp;";

            COCDataset.ProcessInspectionsDataTable dtProcessInspection = null;
            COCDataset.ProcessDataTable dtProcess = null;
            ProcessTableAdapter taProcess = null;
            ProcessInspectionsTableAdapter taProcessInspection = null;
            PartInspectionAnswersTableAdapter taAnswers = null;
            Data.Datasets.COCDatasetTableAdapters.PartInspectionTableAdapter taPartInspection = null;

            try
            {
                if (order == null)
                {
                    return string.Empty;
                }
                else if (order.GetOrderProcessesRows().Length == 0)
                {
                    _log.Warn("Did not retrieve process info for order {0}", order.OrderID);
                    return string.Empty;
                }

                dtProcessInspection = new COCDataset.ProcessInspectionsDataTable();
                dtProcess = new COCDataset.ProcessDataTable();
                taProcess = new ProcessTableAdapter();
                taProcessInspection = new ProcessInspectionsTableAdapter();
                taAnswers = new PartInspectionAnswersTableAdapter();
                taPartInspection = new Data.Datasets.COCDatasetTableAdapters.PartInspectionTableAdapter();

                taProcess.Fill(dtProcess, order.OrderID);

                var returnLines = new List<string>();
                foreach (var orderProcess in order.GetOrderProcessesRows())
                {
                    var process = dtProcess.FindByProcessID(orderProcess.ProcessID);

                    if (process == null || !orderProcess.COCData)
                    {
                        continue;
                    }

                    var processLine =  process.Description +
                        " Per " +
                        taProcess.GetProcessAliasName(orderProcess.OrderProcessesID);

                    returnLines.Add(processLine);

                    taProcessInspection.FillByProcess(dtProcessInspection, orderProcess.ProcessID);
                    var partInspections = taPartInspection.GetData(orderProcess.OrderProcessesID);

                    foreach (var processInspection in dtProcessInspection)
                    {
                        if (!processInspection.COCData)
                        {
                            continue;
                        }

                        var inspection = partInspections
                            .FirstOrDefault(i => i.PartInspectionTypeID == processInspection.PartInspectionTypeID);

                        if (inspection != null)
                        {
                            using (var answers = taAnswers.GetData(inspection.PartInspectionID))
                            {
                                if (answers.Count > 0)
                                {
                                    returnLines.Add(string.Empty);
                                    returnLines.Add(indent.Repeat(4) + inspection.Name);
                                }
                                else if (inspection.RejectedQty == 0)
                                {
                                    var passedText = " <font color='red'>Pass</font>";
                                    returnLines.Add(string.Empty);
                                    returnLines.Add(indent.Repeat(4) + inspection.Name + passedText);
                                }

                                foreach (var answer in answers)
                                {
                                    string answerText = string.Format(
                                        "\t\t{0}: <font color='red'>{1}</font>",
                                        answer.QuestionName,
                                        answer.IsAnswerNull() ? "Unknown" : answer.Answer);

                                    returnLines.Add(indent.Repeat(8) + answerText);
                                }
                            }
                        }
                    }
                }

                return "<font size='8'>" +
                    string.Join("<br />", returnLines) +
                    "</font>";
            }
            finally
            {
                dtProcessInspection?.Dispose();
                dtProcess?.Dispose();
                taProcess?.Dispose();
                taProcessInspection?.Dispose();
                taAnswers?.Dispose();
                taPartInspection?.Dispose();
            }
        }

        private void AddSignature(COCDataset.UsersRow qaUser)
        {
            try
            {
                var signatureData = SignatureData.From(qaUser) ??
                    SignatureData.Default();

                ITable headerTable = _section.AddTable();
                headerTable.Margins.All = 5;

                ITableRow row = headerTable.AddRow();
                ITableCell signatureContainer = row.AddCell();
                ITableCell signatureImgContainer = row.AddCell();
                ITableCell dateContainer = row.AddCell();

                signatureContainer.Width = new RelativeWidth(20);
                signatureContainer.Alignment.Horizontal = Alignment.Right;
                signatureContainer.Alignment.Vertical = Alignment.Middle;

                signatureImgContainer.Width = new RelativeWidth(30);
                signatureImgContainer.Alignment.Horizontal = Alignment.Left;
                signatureImgContainer.Alignment.Vertical = Alignment.Middle;

                dateContainer.Width = new RelativeWidth(50);
                dateContainer.Alignment.Horizontal = Alignment.Right;
                dateContainer.Alignment.Vertical = Alignment.Middle;

                //Add Signature Name
                IText companyText = signatureContainer.AddText();
                companyText.Alignment.Horizontal = Alignment.Left;
                companyText.Alignment.Vertical = Alignment.Middle;
                companyText.Height = new FixedHeight(15);
                companyText.Style = DefaultStyles.NormalStyle;
                companyText.Paddings.Horizontal = 5;
                companyText.AddContent("Authorized Signature:", DefaultStyles.NormalStyle);
                companyText.AddLineBreak();

                //Add Logo
                IGroup group = signatureImgContainer.AddGroup();
                group.Alignment = ContentAlignment.Left;
                group.Layout = Layout.Vertical;

                if (signatureData.Image != null)
                {
                    IImage image = group.AddImage(signatureData.Image);
                    image.Height = new FixedHeight(25);
                    image.Width = new FixedWidth((signatureData.Image.Width * 25) / signatureData.Image.Height);
                }

                //Add Line
                IRule rule = group.AddRule();
                rule.Pen = new Pen(new Infragistics.Documents.Reports.Graphics.Color(Color.Black));
                rule.Pen.Width = 1;
                rule.Margins = new VerticalMargins(2, 2);

                //Add User Name
                IText userName = group.AddText();
                userName.Style = DefaultStyles.NormalStyle;
                string displayName = signatureData.Name;
                if (!string.IsNullOrWhiteSpace(signatureData.Title))
                {
                    displayName += ", " + signatureData.Title;
                }

                userName.AddContent(displayName);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error creating signature on COC report.");
            }
        }

        private static DateTime GetShippedDate(COCDataset.BulkCOCRow bulkCOC)
        {
            bool shipmentActive = false;

            using (var taShipmentPackage = new Data.Datasets.OrderShipmentDataSetTableAdapters.ShipmentPackageTableAdapter())
            {
                using (var dtShipmentPackage = new OrderShipmentDataSet.ShipmentPackageDataTable())
                {
                    taShipmentPackage.FillByShipmentPackageID(dtShipmentPackage, bulkCOC.ShipmentPackageID);
                    shipmentActive = dtShipmentPackage.Count > 0 && dtShipmentPackage.First().Active;
                }
            }

            DateTime shippedDate;

            if (shipmentActive)
            {
                shippedDate = DateTime.Today;
            }
            else
            {
                using (var taOrderShipment = new OrderShipmentTableAdapter())
                {
                    using (var dtOrderShipment = new OrdersDataSet.OrderShipmentDataTable())
                    {
                        taOrderShipment.FillByShipment(dtOrderShipment, bulkCOC.ShipmentPackageID);

                        shippedDate = dtOrderShipment.FirstOrDefault()?.DateShipped ?? DateTime.Today;
                    }
                }
            }

            return shippedDate;
        }

        private static IEnumerable<BulkCOCEntry> EntriesFrom(IEnumerable<COCDataset.OrderSummaryRow> orders)
        {
            const int defaultContainerCount = 1;

            if (orders == null)
            {
                return null;
            }

            var orderGroups = orders.GroupBy((o) =>
            {
                var purchaseOrder = o.IsPurchaseOrderNull() ? null : o.PurchaseOrder;
                var customerWO = o.IsCustomerWONull() ? null : o.CustomerWO;

                return purchaseOrder + customerWO + o.PartName;
            });

            var entries = new List<BulkCOCEntry>();

            foreach (var orderGroup in orderGroups)
            {
                var firstOrder = orderGroup.OrderBy(o => o.OrderID).First();

                entries.Add(new BulkCOCEntry()
                {
                    PurchaseOrder = firstOrder.IsPurchaseOrderNull() ? "None" : firstOrder.PurchaseOrder,
                    CustomerWO = firstOrder.IsCustomerWONull() ? "None" : firstOrder.CustomerWO,
                    PartName = firstOrder.PartName,
                    ProcessRichContent = GetProcessRichContent(firstOrder),

                    GrossWeight = orderGroup.Sum(o => OrderUtilities.CalculateGrossWeight(o)),
                    PartQuantity = orderGroup.Sum(o => o.IsPartQuantityNull() ? (int?)null : o.PartQuantity),
                    ContainerCount = orderGroup.Sum(o => o.IsContainerCountNull() ? defaultContainerCount : o.ContainerCount)
                });
            }

            return entries;
        }

        #endregion

        #region BulkCOCEntry

        private sealed class BulkCOCEntry
        {
            #region Properties

            public int ContainerCount { get; set; }

            public string CustomerWO { get; set; }

            public decimal GrossWeight { get; set; }

            public string PartName { get; set; }

            public int? PartQuantity { get; set; }

            public string ProcessRichContent { get; set; }

            public string PurchaseOrder { get; set; }

            #endregion
        }

        #endregion
    }

    public class BatchCocReport : Report
    {
        #region Fields

        private readonly BatchCertificate _batchCertificate;

        #endregion

        #region Properties

        public override string Title => "Batch COC";

        public string IdentifierType => _batchCertificate.Batch.SalesOrderId.HasValue
            ? "Sales Order"
            : "Batch";

        public int Identifier => _batchCertificate.Batch.SalesOrderId ??
            _batchCertificate.Batch.BatchId;

        protected override PageOrientation ReportPageOrientation => PageOrientation.Portrait;

        protected override string FooterIdentifier => _batchCertificate.BatchCocId.ToString();

        #endregion

        #region Methods

        public BatchCocReport(BatchCertificate batchCertificate)
        {
            _batchCertificate = batchCertificate
                ?? throw new ArgumentNullException(nameof(batchCertificate));

            if (batchCertificate.Batch == null)
            {
                throw new ArgumentException(
                    "Certificate must have batch.",
                    nameof(batchCertificate));
            }
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            SetupReportInfo();
            _section.PageMargins.All = 20;

            AddHeader("CERTIFICATE OF CONFORMANCE",
                $"{IdentifierType}: ",
                Identifier,
                true,
                null,
                ReportType.BatchOrder);

            AddCustomerSection();
            AddPartSection();
            AddProcessSection();
            AddSignature();
        }

        private void AddCustomerSection()
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
                var approvalDateContainer = pcell2.AddContainer("reqDate");

                orderContainer.Alignment.Vertical = Alignment.Top;
                orderContainer.Alignment.Horizontal = Alignment.Left;
                orderContainer.Paddings.Left = 5;
                orderContainer.Paddings.Top = 4;
                orderContainer.Borders = DefaultStyles.DefaultBorders;
                orderContainer.Height = new FixedHeight(containerHeight);

                approvalDateContainer.Paddings.Left = 5;
                approvalDateContainer.Paddings.Top = 4;
                approvalDateContainer.Height = new FixedHeight(containerHeight);
                approvalDateContainer.Alignment.Vertical = Alignment.Top;
                approvalDateContainer.Alignment.Horizontal = Alignment.Left;
                approvalDateContainer.Borders = DefaultStyles.DefaultBorders;
                approvalDateContainer.Margins.Left = 5;

                AddApprovalDateTable(approvalDateContainer);

                AddInfoTable(orderContainer);

                // Add the 'VOID' decorations if the cert is unsaved
                if (_batchCertificate.BatchCocId <= 0)
                {
                    var decorationTop = _section.AddDecoration();
                    decorationTop.MasterRange = MasterRange.All;
                    decorationTop.Rear = false;
                    var decorationTopText = decorationTop.AddText(200, 50, 0);
                    decorationTopText.Style = new Style(new Infragistics.Documents.Reports.Graphics.Font("Verdana", 50), Brushes.Red);
                    decorationTopText.Width = new RelativeWidth(35);
                    decorationTopText.Borders = new Borders(new Pen(Colors.Red, 3, DashStyle.Solid), 10);
                    decorationTopText.AddContent("  VOID");

                    var decorationBottom = _section.AddDecoration();
                    decorationBottom.MasterRange = MasterRange.All;
                    decorationBottom.Rear = false;
                    var decorationBottomText = decorationBottom.AddText(200, 600, 0);
                    decorationBottomText.Style = new Style(new Infragistics.Documents.Reports.Graphics.Font("Verdana", 50), Brushes.Red);
                    decorationBottomText.Width = new RelativeWidth(35);
                    decorationBottomText.Borders = new Borders(new Pen(Colors.Red, 3, DashStyle.Solid), 10);
                    decorationBottomText.AddContent("  VOID");
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding customer section to batch certificate.");
            }
        }

        private void AddCustomerTable(Infragistics.Documents.Reports.Report.IContainer customerContainer)
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

                // Assumption: All orders in the certificate belong to
                // the same customer.
                var customerId = _batchCertificate.Orders.FirstOrDefault().CustomerId;

                using (CustomerTableAdapter taCustomer = new CustomerTableAdapter())
                {
                    taCustomer.FillBy(dsCustomer.Customer, customerId);
                }

                using (var taCustomerAddress = new Data.Datasets.CustomersDatasetTableAdapters.CustomerAddressTableAdapter { ClearBeforeFill = false })
                {
                    foreach (var order in _batchCertificate.Orders)
                    {
                        taCustomerAddress.FillByOrderID(dsCustomer.CustomerAddress, order.OrderId);
                    }
                }

                using (var taCountry = new CountryTableAdapter())
                {
                    taCountry.Fill(dsCustomer.Country);
                }

                var companyTable = customerContainer.AddTable();
                var companyTitle = companyTable.AddRow();
                companyTitle.AddCell(100, "Customer:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                var companyName = companyTable.AddRow();

                if (dsCustomer.Customer.Rows.Count > 0)
                {
                    var customer = dsCustomer.Customer[0];

                    companyName.AddCell(100, "   " + customer.Name.TrimToMaxLength(maxCustomerLength, ellipsis),
                        DefaultStyles.BoldMediumStyle, new TextAlignment(Alignment.Left, Alignment.Middle));

                    if (customer.HasBillingAddress)
                    {
                        if (!customer.IsAddress1Null() && customer.Address1 != customer.Name)
                        {
                            var companyAddress = companyTable.AddRow();
                            companyAddress.AddCell(50, "   " + customer.Address1, DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                        }

                        var addressRow = companyTable.AddRow();
                        var zip = customer.IsZipNull() ? "" : customer.Zip;
                        var shipLastRow = (customer.IsCityNull() ? "-" : customer.City) + ", " + (customer.IsStateNull() ? "-" : customer.State) + " " + zip;

                        if (customer.CountryID != ApplicationSettings.Current.CompanyCountry)
                        {
                            shipLastRow += $" {customer.CountryRow.Name}";
                        }

                        addressRow.AddCell(50, "   " + shipLastRow.TrimToMaxLength(maxLength, ellipsis),
                            DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                    }

                    if (customer.GetCustomerAddressRows().Length == 1)
                    {
                        var customerAddress = customer.GetCustomerAddressRows().First();

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
                        var shipLastLine = shipCity + ", " + shipState + " " + shipZip;

                        if (customerAddress.CountryID != ApplicationSettings.Current.CompanyCountry)
                        {
                            shipLastLine += $" {customerAddress.CountryRow.Name}";
                        }

                        shippingAddressRow.AddCell(50, "   " + shipLastLine.TrimToMaxLength(maxLength, ellipsis),
                            DefaultStyles.NormalStyle,
                            new TextAlignment(Alignment.Left, Alignment.Middle));
                    }
                    else if (customer.GetCustomerAddressRows().Length > 1)
                    {
                        // Going to multiple addresses
                        var shippingTitle = companyTable.AddRow();

                        shippingTitle.AddCell(100, "Ship To:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                        var shippingNameRow = companyTable.AddRow();
                        shippingNameRow.AddCell(50, "   Multiple Addresses",
                            DefaultStyles.NormalStyle,
                            new TextAlignment(Alignment.Left, Alignment.Middle));
                    }
                }
            }
            finally
            {
                dsCustomer?.Dispose();
            }
        }

        private void AddInfoTable(Infragistics.Documents.Reports.Report.IContainer orderContainer)
        {
            const int maxPurchaseOrderLength = 40;

            var purchaseOrders = new List<string>();
            using (var taCOC = new Data.Datasets.COCDatasetTableAdapters.COCTableAdapter())
            {
                foreach (var order in _batchCertificate.Orders)
                {
                    var purchaseOrder = taCOC.GetPurcharseOrder(order.OrderId);

                    if (string.IsNullOrEmpty(purchaseOrder))
                    {
                        purchaseOrders.Add("N/A");
                    }
                    else
                    {
                        purchaseOrders.Add(purchaseOrder);
                    }
                }
            }

            var distinctPurchaseOrders = purchaseOrders
                .Where(po => !string.IsNullOrEmpty(po))
                .Distinct()
                .OrderBy(po => po)
                .ToList();

            var poValue = string.Join(",", distinctPurchaseOrders);
            var formattedPurchaseOrder = FormatValue(string.IsNullOrWhiteSpace(poValue) ? "N/A" : poValue, maxPurchaseOrderLength);

            var infoTable = orderContainer.AddTable();

            infoTable.AddRow().AddCell(100, "Batch COC:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            infoTable.AddRow().AddCell(100, _batchCertificate.BatchCocId.ToString(), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));


            if (_batchCertificate.Batch.SalesOrderId.HasValue)
            {
                infoTable.AddRow().AddCell(100, "Batch:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
                infoTable.AddRow().AddCell(100, _batchCertificate.Batch.BatchId.ToString(), DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            }

            var purchaseOrderTitle = distinctPurchaseOrders.Count > 1
                ? "Purchase Orders:"
                : "Purchase Order:";

            infoTable.AddRow().AddCell(100, purchaseOrderTitle, DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            infoTable.AddRow().AddCell(100, formattedPurchaseOrder, DefaultStyles.NormalStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
        }

        private void AddApprovalDateTable(Infragistics.Documents.Reports.Report.IContainer approvalDateContainer)
        {
            var approvalDateTable = approvalDateContainer.AddTable();
            var approvalDateRow = approvalDateTable.AddRow();
            approvalDateRow.Height = new FixedHeight(20);

            var approvalDateCell = approvalDateRow.AddCell(40, "Approval Date:", DefaultStyles.BlueLargeStyle, new TextAlignment(Alignment.Left, Alignment.Middle));
            approvalDateCell.Height = new FixedHeight(20);
            var approvalDateValueRow = approvalDateTable.AddRow();
            approvalDateValueRow.Height = new FixedHeight(20);

            var superSized = new Style(new Infragistics.Documents.Reports.Graphics.Font(new Font(DefaultStyles.RedXLargeStyle.Font.Name, DefaultStyles.RedXLargeStyle.Font.Size - 3, FontStyle.Bold)), DefaultStyles.BlackXLargeStyle.Brush);
            ITableCell approvalDateValueCell = approvalDateValueRow.AddCell(40, _batchCertificate.DateCertified.ToShortDateString(), superSized, new TextAlignment(Alignment.Center, Alignment.Middle));
            approvalDateValueCell.Height = new FixedHeight(20);
        }

        private void AddPartSection()
        {
            const int orderWidthPercentage = 70;
            const int imgWidthPercentage = 30;

            var dsCoc = new COCDataset
            {
                EnforceConstraints = false
            };

            try
            {
                bool isMaterialVisible;
                bool isRevisionVisible;
                bool isSerialNumberVisible;
                using (var taField = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
                {
                    using (var dtPartFields = taField.GetByCategory("Part"))
                    {
                        var materialField = dtPartFields.FirstOrDefault(field => field.Name == "Material");
                        isMaterialVisible = materialField != null && materialField.IsVisible;

                        var revisionField = dtPartFields.FirstOrDefault(field => field.Name == "Part Rev.");
                        isRevisionVisible = revisionField != null && revisionField.IsVisible;
                    }

                    using (var dtOrderFields = taField.GetByCategory("Order"))
                    {
                        var serialNumberField = dtOrderFields.FirstOrDefault(field => field.Name == "Serial Number");
                        isSerialNumberVisible = serialNumberField != null && serialNumberField.IsVisible;
                    }
                }

                using (var taPart = new COCPartTableAdapter { ClearBeforeFill = false })
                {
                    using (var taPartMedia = new Part_MediaTableAdapter { ClearBeforeFill = false })
                    {
                        foreach (var order in _batchCertificate.Orders)
                        {
                            taPart.FillByOrder(dsCoc.COCPart, order.OrderId);
                        }

                        foreach (var partRow in dsCoc.COCPart)
                        {
                            taPartMedia.FillByPart(dsCoc.Part_Media, partRow.PartID);
                        }
                    }
                }

                var headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Margins = new Margins(5, 5, 3, 0);
                headerGroup.Borders = DefaultStyles.DefaultBorders;
                headerGroup.Background = DefaultStyles.DefaultBackground;

                var orderContainer = headerGroup.AddContainer("order");
                orderContainer.Width = new RelativeWidth(100);
                orderContainer.Paddings.All = 5;
                orderContainer.Margins.Right = 5;

                //Add Part info table
                var partInfoHeader = orderContainer.AddText();
                partInfoHeader.Background = new Background(new SolidColorBrush(new Infragistics.Documents.Reports.Graphics.Color(System.Drawing.Color.WhiteSmoke)));
                partInfoHeader.AddContent("Part Information:", DefaultStyles.BlueLargeStyle);
                partInfoHeader.Margins = new Margins(0, 0, 0, 5);

                var isFirstPart = true;

                foreach (var cocPartRow in dsCoc.COCPart.OrderBy(part => part.Name))
                {
                    if (!isFirstPart)
                    {
                        var horizontalRule = orderContainer.AddRule();
                        horizontalRule.Margins = new VerticalMargins(5);
                    }

                    var partNumber = string.Empty;
                    if (cocPartRow.IsRevisionNull() || !isRevisionVisible)
                    {
                        partNumber = cocPartRow.Name;
                    }
                    else
                    {
                        partNumber = $"{cocPartRow.Name} Rev. {cocPartRow.Revision}";
                    }

                    var mainTable = orderContainer.AddTable();

                    var mainTableRow = mainTable.AddRow();
                    var partInfoTable = mainTableRow.AddCell(new RelativeWidth(orderWidthPercentage)).AddTable();
                    partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Part Number:", partNumber);

                    if (isMaterialVisible)
                    {
                        partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Material:", cocPartRow.IsMaterialNull() ? "Unknown" : cocPartRow.Material);
                    }

                    var ordersWithPart = _batchCertificate.Orders
                        .Where(wo => wo.PartId == cocPartRow.PartID)
                        .ToList();

                    var partQuantity = ordersWithPart
                        .Sum(wo => (long)wo.BatchQuantity);

                    partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Quantity:", partQuantity.ToString());

                    if (!cocPartRow.IsDescriptionNull())
                    {
                        partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Description:", cocPartRow.Description);
                    }

                    if (isSerialNumberVisible)
                    {
                        var serialNumbersString = string.Join(", ",
                            ordersWithPart.SelectMany(wo => wo.SerialNumbers));

                        if (string.IsNullOrEmpty(serialNumbersString))
                        {
                            serialNumbersString = "N/A";
                        }

                        partInfoTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Serial Numbers:", serialNumbersString);
                    }

                    //Add Part Image
                    var partMedia = cocPartRow.GetPart_MediaRows();
                    if (partMedia.Length > 0 || ApplicationSettings.Current.UseReportPlaceholderImage)
                    {
                        var imgContainer = mainTableRow.AddCell(new RelativeWidth(imgWidthPercentage)).AddContainer($"part-{cocPartRow.PartID}");
                        imgContainer.Alignment.Vertical = Alignment.Middle;
                        imgContainer.Alignment.Horizontal = Alignment.Right;
                        imgContainer.Paddings.All = 5;
                        imgContainer.Paddings.Right = 5;
                        imgContainer.Margins.Left = 5;
                        imgContainer.Margins.Right = 8;

                        Image img = null;

                        var partMediaRow = partMedia.FirstOrDefault(m => !m.IsDefaultMediaNull() && m.DefaultMedia)
                            ?? partMedia.FirstOrDefault();

                        if (partMediaRow != null)
                        {
                            string fileExtension;
                            using (var taPartMedia = new Part_MediaTableAdapter())
                            {
                                fileExtension = taPartMedia.GetFileExtension(partMediaRow.MediaID);
                            }

                            img = MediaUtilities.GetImage(partMediaRow.MediaID, fileExtension);
                        }
                        else if (System.IO.File.Exists(ApplicationSettings.Current.ReportPlaceholderImagePath))
                        {
                            img = MediaUtilities.GetImage(System.IO.File.ReadAllBytes(ApplicationSettings.Current.ReportPlaceholderImagePath));
                        }
                        else
                        {
                            img = Data.Properties.Resources.NoImage;
                        }

                        var image = imgContainer.AddImage(new Infragistics.Documents.Reports.Graphics.Image(img));
                        image.KeepRatio = true;

                        var maximumSize = new System.Drawing.Size(200, 50);
                        var imageSize = MediaUtilities.Resize(img.Size, maximumSize);
                        image.Width = new FixedWidth(imageSize.Width);
                        image.Height = new FixedHeight(imageSize.Height);
                    }

                    isFirstPart = false;
                }

                // Warranty
                var qualText = orderContainer.AddText();
                qualText.Width = new RelativeWidth(100);
                qualText.Alignment = TextAlignment.Left;
                qualText.Alignment.Horizontal = Alignment.Left;
                qualText.Style = DefaultStyles.BlueLargeStyle;
                qualText.Paddings.Top = 15;
                qualText.Margins.Right = 5;
                qualText.AddContent("Quality:");

                var warText = orderContainer.AddText();
                warText.Width = new RelativeWidth(100);
                warText.Alignment = TextAlignment.Left;
                warText.Alignment.Horizontal = Alignment.Justify;
                warText.Style = DefaultStyles.NormalStyle;
                warText.Paddings.Vertical = 5;
                warText.Margins.Right = 5;

                var manufacturers = dsCoc.COCPart
                    .Select(part => part.IsManufacturerIDNull() ? string.Empty : part.ManufacturerID)
                    .Distinct()
                    .ToList();

                if (manufacturers.Count == 1 && !string.IsNullOrEmpty(manufacturers.First()))
                {
                    // Every part has the same manufacturer - use their warranty
                    var manufacturerId = manufacturers.First();
                    string warranty;
                    using (var taPart = new COCPartTableAdapter())
                    {
                        warranty = taPart.GetManufacturerWarranty(manufacturerId);
                    }
                    warText.AddRichContent(warranty ?? string.Empty);
                }
                else
                {
                    // Use default manufacturer
                    var warranty = ApplicationSettings.Current.COCWarranty;
                    warText.AddRichContent(warranty ?? string.Empty);
                    warText.AddLineBreak();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding part section to batch COC.");
            }
            finally
            {
                dsCoc.Dispose();
            }
        }

        private void AddProcessSection()
        {
            try
            {
                IGroup group = _section.AddGroup();
                group.Layout = Layout.Horizontal;
                group.Margins = new Margins(5, 5, 3, 0);
                group.Paddings.All = 5;
                group.Borders = DefaultStyles.DefaultBorders;
                group.Background = DefaultStyles.DefaultBackground;

                IGroup processGroup = group.AddGroup();
                processGroup.Width = new RelativeWidth(90);
                processGroup.Margins.Right = 5;

                IText title = processGroup.AddText();
                title.Width = new RelativeWidth(100);
                title.Alignment = TextAlignment.Left;
                title.Style = DefaultStyles.NormalStyle;

                title.AddContent("Processes:", DefaultStyles.BlueLargeStyle);
                title.AddLineBreak();

                IText processText = processGroup.AddText();
                processText.Alignment = TextAlignment.Left;
                processText.Style = DefaultStyles.NormalStyle;

                var cocInfo = _batchCertificate.InfoHtml;

                if (string.IsNullOrEmpty(cocInfo))
                {
                    cocInfo = "No Info";
                }
                else
                {
                    cocInfo = cocInfo.Replace("&edsp;", "&nbsp;");
                    cocInfo = cocInfo.Replace("\"8pt\"", "\"8\""); //added 06.26.09 found the pt to cause an issue
                }

                processText.AddRichContent(cocInfo, false);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding process section to batch COC.");
            }
        }

        private void AddSignature()
        {
            try
            {
                var qaUser = _batchCertificate.QualityInspector;

                var signatureData = SignatureData.From(qaUser) ?? SignatureData.Default();

                ITable headerTable = _section.AddTable();
                headerTable.Margins = new Margins(5, 5, 0, 0);

                //Add auth signature text, image of signature, and date
                ITableRow row = headerTable.AddRow();
                ITableCell signatureContainer = row.AddCell();
                ITableCell signatureImgContainer = row.AddCell();
                ITableCell spacerContainer = row.AddCell();

                signatureContainer.Width = new RelativeWidth(30);
                signatureContainer.Alignment.Horizontal = Alignment.Right;
                signatureContainer.Alignment.Vertical = Alignment.Middle;

                signatureImgContainer.Width = new RelativeWidth(70);
                signatureImgContainer.Height = new FixedHeight(75);
                signatureImgContainer.Alignment.Horizontal = Alignment.Left;
                signatureImgContainer.Alignment.Vertical = Alignment.Middle;


                spacerContainer.Width = new RelativeWidth(50);
                spacerContainer.Alignment.Horizontal = Alignment.Right;
                spacerContainer.Alignment.Vertical = Alignment.Middle;

                //Add Authorized Signature Text
                IText companyText = signatureContainer.AddText();
                companyText.Alignment.Horizontal = Alignment.Left;
                companyText.Alignment.Vertical = Alignment.Middle;
                companyText.Height = new FixedHeight(15);
                companyText.Style = DefaultStyles.NormalStyle;
                companyText.Paddings.Horizontal = 5;
                companyText.AddContent("Authorized Signature:", DefaultStyles.NormalStyle);
                companyText.AddLineBreak();

                //Add Signature Image
                IGroup group = signatureImgContainer.AddGroup();
                group.Alignment = Infragistics.Documents.Reports.Report.ContentAlignment.Left;
                group.Layout = Layout.Vertical;

                if (signatureData.Image != null)
                {
                    IImage image = group.AddImage(signatureData.Image);
                    var maxSize = new System.Drawing.Size(signatureData.Image.Width, 40);
                    var imgSize = MediaUtilities.Resize(new System.Drawing.Size(signatureData.Image.Width, signatureData.Image.Height), maxSize);
                    image.Height = new FixedHeight(imgSize.Height);
                    image.Width = new FixedWidth(imgSize.Width);
                }

                //Add Signature Line
                IRule rule = group.AddRule();
                rule.Pen = new Pen(new Infragistics.Documents.Reports.Graphics.Color(Color.Black))
                {
                    Width = 1
                };

                rule.Margins = new VerticalMargins(2, 2);

                //Add User Name
                IText userName = group.AddText();
                userName.Style = DefaultStyles.NormalStyle;
                string displayName = signatureData.Name;
                if (!string.IsNullOrWhiteSpace(signatureData.Title))
                {
                    displayName += ", " + signatureData.Title;
                }

                userName.AddContent(displayName);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error creating signature on COC report.");
            }
        }

        /// <summary>
        /// Formats the value of a string so that it fits the supplied length.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="maxLength">The length.</param>
        /// <returns>Substring with ellipse (…) if needed</returns>
        private static string FormatValue(string value, int maxLength)
        {
            string formattedValue;
            if (value.Length <= maxLength)
            {
                formattedValue = value;
            }
            else
            {
                formattedValue = value.Substring(0, maxLength - 1) + "…";
            }

            return formattedValue;
        }

        public static BatchCocReport FromBatchCocId(int batchCocId)
        {
            var persistence = new CocPersistence(ApplicationSettings.Current);
            return new BatchCocReport(persistence.GetBatchCoc(batchCocId));
        }

        public static BatchCocReport From(OrdersDataSet.BatchCOCRow batchCocRow)
        {
            var batchRow = batchCocRow.BatchRow;

            if (batchRow == null)
            {
                _log.Error($"Batch not loaded: {batchCocRow.BatchID}");
                return null;
            }

            var salesOrderId = !batchRow.IsSalesOrderIDNull()
                ? batchRow.SalesOrderID
                : (int?)null;

            var batchProcesses = new List<CertificateBatch.BatchProcess>();

            foreach (var batchProcessRow in batchRow.GetBatchProcessesRows())
            {
                batchProcesses.Add(new CertificateBatch.BatchProcess(batchProcessRow.BatchProcessID,
                    batchProcessRow.ProcessID,
                    batchProcessRow.GetBatchProcess_OrderProcessRows()
                        .Select(bp => bp.OrderProcessID)
                        .ToList()));
            }

            var orders = new List<CertificateBatchOrder>();

            foreach (var batchCocOrderRow in batchCocRow.GetBatchCOCOrderRows())
            {
                var orderRow = batchCocOrderRow.OrderRow;

                if (orderRow == null)
                {
                    _log.Error($"Order not loaded: {batchCocOrderRow.OrderID}");
                    continue;
                }

                var batchOrderRow = batchRow.GetBatchOrderRows()
                    .FirstOrDefault(batchOrder => batchOrder.OrderID == orderRow.OrderID);

                if (batchOrderRow == null)
                {
                    _log.Error($"Batch Order not loaded for WO:{orderRow.OrderID}");
                    continue;
                }

                var processes = new List<CertificateBatchOrder.OrderProcess>();

                foreach (var orderProcessRow in orderRow.GetOrderProcessesRows())
                {
                    // Assumption: description/steps/inspection are only needed
                    // when creating the certificate's text.
                    processes.Add(new CertificateBatchOrder.OrderProcess(
                        orderProcessRow.OrderProcessesID,
                        orderProcessRow.ProcessID,
                        orderProcessRow.ProcessAliasSummaryRow?.Name,
                        string.Empty,
                        orderProcessRow.COCData,
                        new List<CertificateBatchOrder.OrderProcessStep>(),
                        new List<CertificateBatchOrder.PartInspection>()));
                }

                // Assumption: part mark is only needed when
                // creating certificate text

                orders.Add(new CertificateBatchOrder(
                    orderRow.OrderID,
                    orderRow.IsCustomerIDNull() ? -1 : orderRow.CustomerID,
                    orderRow.IsPartIDNull() ? -1 : orderRow.PartID,
                    orderRow.PartSummaryRow?.Name,
                    batchOrderRow.PartQuantity,
                    orderRow.GetOrderSerialNumberRows()
                        .Where(s => s.Active && !s.IsNumberNull())
                        .Select(s => s.Number)
                        .ToList(),
                    orderRow.IsImportedPriceNull() ? (decimal?)null : orderRow.ImportedPrice,
                    processes,
                    orderRow.GetOrderCustomFieldsRows()
                        .Select(field => new CertificateBatchOrder.CustomField(field.CustomFieldID, field.CustomFieldRow?.Name, field.IsValueNull() ? string.Empty : field.Value, field.CustomFieldRow?.DisplayOnCOC ?? false))
                        .ToList(),
                    LoadPartLevelCustomFields(orderRow.IsPartIDNull() ? -1 : orderRow.PartID),
                    null,
                    orderRow.GetInternalReworkRows().Select(rework => new CertificateBatchOrder.OrderRework(rework.d_ReworkReasonRow?.Name, rework.d_ReworkReasonRow?.ShowOnDocuments ?? false))
                        .ToList()));
            }

            var batchCoc = new BatchCertificate
            {
                Batch = new CertificateBatch(batchCocRow.BatchID, salesOrderId, batchProcesses),
                BatchCocId = batchCocRow.BatchCOCID,
                DateCertified = batchCocRow.DateCertified,
                InfoHtml = batchCocRow.IsCompressed
                    ? batchCocRow.COCInfo.DecompressString()
                    : batchCocRow.COCInfo,
                QualityInspector = new User(batchCocRow.QAUser,
                    batchCocRow.UserSummaryRow?.Name),
                Orders = orders
            };

            return new BatchCocReport(batchCoc);
        }

        private static List<CertificateBatchOrder.PartCustomField> LoadPartLevelCustomFields(int partId)
        {
            var partFields = new List<CertificateBatchOrder.PartCustomField>();

            using (var taPartCustomFields = new COCPartCustomFieldsTableAdapter())
            {
                using (var dtPartCustomField = taPartCustomFields.GetByPart(partId))
                {
                    foreach (var fieldRow in dtPartCustomField)
                    {
                        partFields.Add(new CertificateBatchOrder.PartCustomField(
                            fieldRow.PartLevelCustomFieldID,
                            fieldRow.Name,
                            fieldRow.IsValueNull() ? null : fieldRow.Value,
                            fieldRow.DisplayOnCOC));
                    }
                }
            }

            return partFields;
        }

        #endregion
    }

    public class InternalReworkReport : ExcelBaseReport
    {
        #region Fields

        private DateTime _fromDate;
        private DateTime _toDate;

        #endregion Fields

        #region Properties

        public override string Title
        {
            get { return "Internal Rework Orders"; }
        }

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
            OrdersDataSet.InternalReworkDataTable internalReworks = null;
            using (var taInternalReworks = new InternalReworkTableAdapter())
                internalReworks = taInternalReworks.GetInternalReworks(_toDate, _fromDate);

            var wsSummary = this.CreateWorksheet("Internal Rework Summary");
            var row = AddCompanyHeaderRows(wsSummary, 3, "Summary") + 2;
            this.AddInternalReworkSummaryData(wsSummary, internalReworks, row);
            wsSummary.Columns[0].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsSummary.Columns[1].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsSummary.Columns[2].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsSummary.Columns[3].SetWidth(20, WorksheetColumnWidthUnit.Character);

            var wsAll = this.CreateWorksheet("All");
            this.AddAllInternalReworkOrdersData(wsAll, internalReworks);
            wsAll.Columns[0].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[1].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[2].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[3].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[4].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[5].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[6].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[7].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[8].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[9].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[10].SetWidth(65, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[11].SetWidth(10, WorksheetColumnWidthUnit.Character);

            var byCustomer = this.CreateWorksheet("By Customer");
            this.AddInternalReworkByCustomerData(byCustomer, internalReworks);
            byCustomer.Columns[0].SetWidth(25, WorksheetColumnWidthUnit.Character);
            byCustomer.Columns[1].SetWidth(15, WorksheetColumnWidthUnit.Character);
            byCustomer.Columns[2].SetWidth(15, WorksheetColumnWidthUnit.Character);
            byCustomer.Columns[3].SetWidth(20, WorksheetColumnWidthUnit.Character);
            byCustomer.Columns[4].SetWidth(10, WorksheetColumnWidthUnit.Character);
            byCustomer.Columns[5].SetWidth(15, WorksheetColumnWidthUnit.Character);
            byCustomer.Columns[6].SetWidth(25, WorksheetColumnWidthUnit.Character);
            byCustomer.Columns[7].SetWidth(15, WorksheetColumnWidthUnit.Character);
            byCustomer.Columns[8].SetWidth(15, WorksheetColumnWidthUnit.Character);
            byCustomer.Columns[9].SetWidth(20, WorksheetColumnWidthUnit.Character);
            byCustomer.Columns[10].SetWidth(65, WorksheetColumnWidthUnit.Character);
            byCustomer.Columns[11].SetWidth(10, WorksheetColumnWidthUnit.Character);

            var byReason = this.CreateWorksheet("By Reason");
            this.AddInternalReworkByReasonData(byReason, internalReworks);
            byReason.Columns[0].SetWidth(25, WorksheetColumnWidthUnit.Character);
            byReason.Columns[1].SetWidth(15, WorksheetColumnWidthUnit.Character);
            byReason.Columns[2].SetWidth(15, WorksheetColumnWidthUnit.Character);
            byReason.Columns[3].SetWidth(20, WorksheetColumnWidthUnit.Character);
            byReason.Columns[4].SetWidth(10, WorksheetColumnWidthUnit.Character);
            byReason.Columns[5].SetWidth(15, WorksheetColumnWidthUnit.Character);
            byReason.Columns[6].SetWidth(25, WorksheetColumnWidthUnit.Character);
            byReason.Columns[7].SetWidth(15, WorksheetColumnWidthUnit.Character);
            byReason.Columns[8].SetWidth(15, WorksheetColumnWidthUnit.Character);
            byReason.Columns[9].SetWidth(20, WorksheetColumnWidthUnit.Character);
            byReason.Columns[10].SetWidth(65, WorksheetColumnWidthUnit.Character);
            byReason.Columns[11].SetWidth(10, WorksheetColumnWidthUnit.Character);

            var holdsAffected = this.CreateWorksheet("Holds Affected");
            this.AddHoldsAffectedByReworkData(holdsAffected, internalReworks);
            holdsAffected.Columns[0].SetWidth(25, WorksheetColumnWidthUnit.Character);
            holdsAffected.Columns[1].SetWidth(15, WorksheetColumnWidthUnit.Character);
            holdsAffected.Columns[2].SetWidth(20, WorksheetColumnWidthUnit.Character);
            holdsAffected.Columns[3].SetWidth(10, WorksheetColumnWidthUnit.Character);
            holdsAffected.Columns[4].SetWidth(15, WorksheetColumnWidthUnit.Character);
            holdsAffected.Columns[5].SetWidth(25, WorksheetColumnWidthUnit.Character);
            holdsAffected.Columns[6].SetWidth(15, WorksheetColumnWidthUnit.Character);
            holdsAffected.Columns[7].SetWidth(50, WorksheetColumnWidthUnit.Character);
            holdsAffected.Columns[8].SetWidth(25, WorksheetColumnWidthUnit.Character);
            holdsAffected.Columns[9].SetWidth(10, WorksheetColumnWidthUnit.Character);
        }

        /// <summary>
        /// Adds the internal rework summary data.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="internalReworks">The internal reworks.</param>
        /// <param name="rowIndex">Index of the row.</param>
        private void AddInternalReworkSummaryData(Worksheet worksheet, OrdersDataSet.InternalReworkDataTable internalReworks, int rowIndex)
        {
            try
            {
                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 4, "Date(s): " + _fromDate.ToShortDateString() + " to " + _toDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the 'All' section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 4, "All Reworks").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;
                this.CreateHeaderCell(worksheet, rowIndex, 0, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 1, "Full").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 2, "Split").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 3, "Split/Hold").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 4, "Total").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                var ordersByReworkType = this.GetOrdersByReworkType(internalReworks);
                var fullCount = ordersByReworkType.ContainsKey("Full") ? ordersByReworkType["Full"].Count : 0;
                var splitCount = ordersByReworkType.ContainsKey("Split") ? ordersByReworkType["Split"].Count : 0;
                var splitHoldCount = ordersByReworkType.ContainsKey("SplitHold") ? ordersByReworkType["SplitHold"].Count : 0;

                this.CreateCell(worksheet, rowIndex, 0, "Totals").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 1, fullCount, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 2, splitCount, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 3, splitHoldCount, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateFormulaCell(worksheet, rowIndex, 4, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(rowIndex + 1, 2, rowIndex + 1, 4), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the 'By Customer' section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 4, "Reworks By Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;
                this.CreateHeaderCell(worksheet, rowIndex, 0, "Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 1, "Full").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 2, "Split").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 3, "Split/Hold").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 4, "Total").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                var startRowIndex = rowIndex;
                foreach (var kvp in ordersByReworkType)
                {
                    var ordersByCustomer = this.GetOrdersByCustomer(kvp.Value);
                    var custSummaryDict = this.GetReworkCounts(ordersByCustomer);

                    foreach (var customer in custSummaryDict)
                    {
                        this.CreateCell(worksheet, rowIndex, 0, customer.Key);
                        this.CreateCell(worksheet, rowIndex, 1, customer.Value.FullCount, false, HorizontalCellAlignment.Center);
                        this.CreateCell(worksheet, rowIndex, 2, customer.Value.SplitCount, false, HorizontalCellAlignment.Center);
                        this.CreateCell(worksheet, rowIndex, 3, customer.Value.SplitHoldCount, false, HorizontalCellAlignment.Center);
                        this.CreateFormulaCell(worksheet, rowIndex, 4, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(rowIndex + 1, 2, rowIndex + 1, 4), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                        rowIndex++;
                    }
                }

                if (startRowIndex < rowIndex)
                {
                    // Add the total row
                    this.CreateCell(worksheet, rowIndex, 0, "Totals").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateFormulaCell(worksheet, rowIndex, 1, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateFormulaCell(worksheet, rowIndex, 2, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 3, rowIndex, 3), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateFormulaCell(worksheet, rowIndex, 3, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 4, rowIndex, 4), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateFormulaCell(worksheet, rowIndex, 4, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 5, rowIndex, 5), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                }
                else
                {
                    // Add the total row
                    this.CreateCell(worksheet, rowIndex, 0, "Totals").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 1, "0", false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 2, "0", false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "0", false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "0", false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                }
                rowIndex++;

                // Add the 'By Reason' section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 4, "Reworks By Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;
                this.CreateHeaderCell(worksheet, rowIndex, 0, "Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 1, "Full").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 2, "Split").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 3, "Split/Hold").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 4, "Total").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                startRowIndex = rowIndex;
                var ordersByReason = this.GetByReason(internalReworks);

                var reasonSummaryDict = this.GetReworkCounts(ordersByReason);
                foreach (var customer in reasonSummaryDict)
                {
                    this.CreateCell(worksheet, rowIndex, 0, customer.Key);
                    this.CreateCell(worksheet, rowIndex, 1, customer.Value.FullCount, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 2, customer.Value.SplitCount, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 3, customer.Value.SplitHoldCount, false, HorizontalCellAlignment.Center);
                    this.CreateFormulaCell(worksheet, rowIndex, 4, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(rowIndex + 1, 2, rowIndex + 1, 4), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    rowIndex++;
                }

                // Add the total row
                if (startRowIndex < rowIndex)
                {
                    this.CreateCell(worksheet, rowIndex, 0, "Totals").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateFormulaCell(worksheet, rowIndex, 1, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateFormulaCell(worksheet, rowIndex, 2, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 3, rowIndex, 3), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateFormulaCell(worksheet, rowIndex, 3, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 4, rowIndex, 4), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateFormulaCell(worksheet, rowIndex, 4, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 5, rowIndex, 5), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                }
                else
                {
                    this.CreateCell(worksheet, rowIndex, 0, "Totals").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 1, "0", false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 2, "0", false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "0", false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "0", false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to add 'summary' worksheet to the Internal Rework Orders Report.");
            }
        }

        /// <summary>
        /// Adds all internal rework orders data.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="internalReworks">The internal reworks.</param>
        private void AddAllInternalReworkOrdersData(Worksheet worksheet, OrdersDataSet.InternalReworkDataTable internalReworks)
        {
            try
            {
                var rowIndex = 0;

                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 11, "Date(s): " + _fromDate.ToShortDateString() + " to " + _toDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the 'All' section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 11, "All Reworks").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;
                this.CreateHeaderCell(worksheet, rowIndex, 0, "Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 1, "WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 2, "Original WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 3, "Type").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 4, "PO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 5, "Priority").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 6, "Department").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 7, "Date Required").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 8, "Est. Ship Date").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 9, "Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 10, "Failed Process").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 11, "Part Qty").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                var startRowIndex = rowIndex;
                var ordersByCustomer = this.GetOrdersByCustomer(internalReworks);

                if (ordersByCustomer.Count > 0)
                {
                    foreach (var kvp in ordersByCustomer)
                    {
                        foreach (var order in kvp.Value)
                        {
                            this.AddDataRow(worksheet, order, rowIndex);
                            rowIndex++;
                        }
                    }

                    // Add the total count
                    var cell = this.CreateCell(worksheet, rowIndex, 0, "Totals:");
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    cell = this.CreateFormulaCell(worksheet, rowIndex, 1, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    cell.Value = "Orders: " + cell.Value;
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 7, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 9, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 10, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    cell = this.CreateFormulaCell(worksheet, rowIndex, 11, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 12, rowIndex, 12), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                }
                else
                {
                    this.CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 11, "None", false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex++;

                    // Add the total count
                    this.CreateCell(worksheet, rowIndex, 0, "Totals:").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 1, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 7, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 9, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 10, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 11, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to add 'all' worksheet to the Internal Rework Orders Report.");
            }
        }

        /// <summary>
        /// Adds the internal rework by customer data.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="internalReworks">The internal reworks.</param>
        private void AddInternalReworkByCustomerData(Worksheet worksheet, OrdersDataSet.InternalReworkDataTable internalReworks)
        {
            try
            {
                var rowIndex = 0;
                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 11, "Date(s): " + _fromDate.ToShortDateString() + " to " + _toDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the 'All' section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 11, "Reworks By Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                var ordersByCustomer = this.GetOrdersByCustomer(internalReworks);

                if (ordersByCustomer.Count > 0)
                {
                    foreach (var kvp in ordersByCustomer)
                    {
                        // Add the 'Customer' section
                        this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 11, kvp.Key).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        rowIndex++;

                        this.CreateHeaderCell(worksheet, rowIndex, 0, "Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 1, "WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 2, "Original WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 3, "Type").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 4, "PO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 5, "Priority").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 6, "Department").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 7, "Date Required").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 8, "Est. Ship Date").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 9, "Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 10, "Failed Process").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 11, "Part Qty").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        rowIndex++;

                        var startRowIndex = rowIndex;
                        foreach (var order in kvp.Value)
                        {
                            this.AddDataRow(worksheet, order, rowIndex);
                            rowIndex++;
                        }

                        // Add the totals 
                        // Add the total count
                        var cell = this.CreateCell(worksheet, rowIndex, 0, "Totals:");
                        cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                        cell = this.CreateFormulaCell(worksheet, rowIndex, 1, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                        cell.Value = "Orders: " + cell.Value;
                        cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                        this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        this.CreateCell(worksheet, rowIndex, 7, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        this.CreateCell(worksheet, rowIndex, 9, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        this.CreateCell(worksheet, rowIndex, 10, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        cell = this.CreateFormulaCell(worksheet, rowIndex, 11, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 12, rowIndex, 12), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                        cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                        rowIndex = rowIndex + 2;
                    }
                }
                else
                {
                    this.CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 11, "None", false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex++;

                    // Add the total count
                    this.CreateCell(worksheet, rowIndex, 0, "Totals:").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 1, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 7, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 9, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 10, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 11, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to add 'by customer' worksheet to the Internal Rework Orders Report.");
            }
        }

        /// <summary>
        /// Adds the internal rework by reason data.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="internalReworks">The internal reworks.</param>
        private void AddInternalReworkByReasonData(Worksheet worksheet, OrdersDataSet.InternalReworkDataTable internalReworks)
        {
            try
            {
                var rowIndex = 0;
                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 11, "Date(s): " + _fromDate.ToShortDateString() + " to " + _toDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the 'All' section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 11, "Reworks By Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                var ordersByCustomer = this.GetByReason(internalReworks);

                if (ordersByCustomer.Count > 0)
                {
                    foreach (var kvp in ordersByCustomer)
                    {
                        // Add the 'Customer' section
                        this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 11, kvp.Key).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        rowIndex++;

                        this.CreateHeaderCell(worksheet, rowIndex, 0, "Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 1, "WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 2, "Original WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 3, "Type").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 4, "PO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 5, "Priority").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 6, "Department").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 7, "Date Required").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 8, "Est. Ship Date").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 9, "Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 10, "Failed Process").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        this.CreateHeaderCell(worksheet, rowIndex, 11, "Part Qty").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                        rowIndex++;

                        var startRowIndex = rowIndex;
                        foreach (var order in kvp.Value)
                        {
                            this.AddDataRow(worksheet, order, rowIndex);
                            rowIndex++;
                        }

                        // Add the totals 
                        // Add the total count
                        var cell = this.CreateCell(worksheet, rowIndex, 0, "Totals:");
                        cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                        cell = this.CreateFormulaCell(worksheet, rowIndex, 1, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                        cell.Value = "Orders: " + cell.Value;
                        cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                        this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        this.CreateCell(worksheet, rowIndex, 7, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        this.CreateCell(worksheet, rowIndex, 9, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        this.CreateCell(worksheet, rowIndex, 10, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        cell = this.CreateFormulaCell(worksheet, rowIndex, 11, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 12, rowIndex, 12), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                        cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                        cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                        rowIndex = rowIndex + 2;
                    }
                }
                else
                {
                    this.CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 11, "None", false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex++;

                    // Add the total count
                    this.CreateCell(worksheet, rowIndex, 0, "Totals:").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 1, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 7, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 9, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 10, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 11, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to add 'by customer' worksheet to the Internal Rework Orders Report.");
            }
        }

        /// <summary>
        /// Adds the holds affected by rework data.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="internalReworks">The internal reworks.</param>
        private void AddHoldsAffectedByReworkData(Worksheet worksheet, OrdersDataSet.InternalReworkDataTable internalReworks)
        {
            try
            {
                var rowIndex = 0;

                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 10, "Date(s): " + _fromDate.ToShortDateString() + " to " + _toDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the 'All' section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 10, "Holds Affected By Rework").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;
                this.CreateHeaderCell(worksheet, rowIndex, 0, "Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 1, "WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 2, "Type").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 3, "PO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 4, "Priority").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 5, "Department").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 6, "Est. Ship Date").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 7, "Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 8, "Process").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 9, "Location").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 10, "Part Qty").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                var startRowIndex = rowIndex;
                var ordersByCustomer = this.GetOrdersByCustomer(internalReworks);

                if (ordersByCustomer.Count > 0)
                {
                    foreach (var customerOrder in ordersByCustomer)
                    {
                        foreach (var internalRework in customerOrder.Value)
                        {
                            var dtOrder = new OrdersDataSet.OrderDataTable();
                            using (var taOrder = new OrderTableAdapter())
                                taOrder.FillByOrderID(dtOrder, internalRework.OriginalOrderID);

                            var dtCustomers = new Data.Datasets.CustomersDataset.CustomerDataTable();
                            using (var taCust = new CustomerTableAdapter())
                                taCust.FillByOrderID(dtCustomers, internalRework.OriginalOrderID);

                            var order = dtOrder[0];
                            var customerName = dtCustomers[0].Name;

                            var dt = new OrdersDataSet.InternalReworkDataTable();
                            using (var internalReworkTableAdapter = new InternalReworkTableAdapter() { ClearBeforeFill = true })
                            {
                                internalReworkTableAdapter.FillByReworkOrderID(dt, order.OrderID);
                            }

                            var reasons = new ListsDataSet.d_ReworkReasonDataTable();
                            using (var taReworkReasons = new Data.Datasets.ListsDataSetTableAdapters.d_ReworkReasonTableAdapter())
                                taReworkReasons.Fill(reasons);

                            this.CreateCell(worksheet, rowIndex, 0, customerName, false, HorizontalCellAlignment.Center);
                            this.CreateCell(worksheet, rowIndex, 3, order.IsPurchaseOrderNull() ? "NA" : order.PurchaseOrder, false, HorizontalCellAlignment.Center);
                            this.CreateCell(worksheet, rowIndex, 4, order.IsPriorityNull() ? "NA" : order.Priority, false, HorizontalCellAlignment.Center);
                            this.CreateCell(worksheet, rowIndex, 5, order.CurrentLocation, false, HorizontalCellAlignment.Center);
                            this.CreateCell(worksheet, rowIndex, 6, order.IsEstShipDateNull() ? "NA" : order.EstShipDate.ToShortDateString(), false, HorizontalCellAlignment.Center);
                            this.CreateCell(worksheet, rowIndex, 10, order.IsPartQuantityNull() ? "NA" : order.PartQuantity.ToString(), false, HorizontalCellAlignment.Center);

                            if (dt.Rows.Count > 0)
                            {
                                ListsDataSet.d_ReworkReasonRow reason = null;
                                if (dt.Rows.Count > 0)
                                {
                                    reason = reasons.FindByReworkReasonID(dt[0].ReworkReasonID);
                                }

                                var paDt = new ProcessesDataset.ProcessAliasDataTable();
                                if (!dt[0].IsProcessAliasIDNull())
                                {
                                    using (var taProcessAlias = new Data.Datasets.ProcessesDatasetTableAdapters.ProcessAliasTableAdapter())
                                        taProcessAlias.GetAliasName(paDt, dt[0].ProcessAliasID);
                                }

                                this.CreateCell(worksheet, rowIndex, 1, dt[0].OriginalOrderID, false, HorizontalCellAlignment.Center);
                                this.CreateCell(worksheet, rowIndex, 2, dt[0].ReworkType, false, HorizontalCellAlignment.Center);
                                this.CreateCell(worksheet, rowIndex, 7, reason != null ? reason.Name : "Other", false, HorizontalCellAlignment.Center);
                                this.CreateCell(worksheet, rowIndex, 8, paDt.Rows.Count > 0 ? paDt[0].Name : "NA", false, HorizontalCellAlignment.Center);
                                this.CreateCell(worksheet, rowIndex, 9, dt[0].IsHoldLocationIDNull() ? "NA" : dt[0].HoldLocationID, false, HorizontalCellAlignment.Center);
                            }
                            else
                            {
                                this.CreateCell(worksheet, rowIndex, 1, "NA", false, HorizontalCellAlignment.Center);
                                this.CreateCell(worksheet, rowIndex, 2, "NA", false, HorizontalCellAlignment.Center);
                                this.CreateCell(worksheet, rowIndex, 7, "Other", false, HorizontalCellAlignment.Center);
                                this.CreateCell(worksheet, rowIndex, 8, "NA", false, HorizontalCellAlignment.Center);
                                this.CreateCell(worksheet, rowIndex, 9, "NA", false, HorizontalCellAlignment.Center);
                            }

                            rowIndex++;
                        }
                    }

                    // Add the total count
                    var cell = this.CreateCell(worksheet, rowIndex, 0, "Totals:");
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    cell = this.CreateFormulaCell(worksheet, rowIndex, 1, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    cell.Value = "Orders: " + cell.Value;
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 7, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 9, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    cell = this.CreateFormulaCell(worksheet, rowIndex, 10, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 11, rowIndex, 11), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                }
                else
                {
                    this.CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 10, "None", false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex++;

                    // Add the total count
                    this.CreateCell(worksheet, rowIndex, 0, "Totals:").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 1, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 7, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 9, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 10, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to add 'holds affected' worksheet to the Internal Rework Orders Report.");
            }
        }

        /// <summary>
        /// Adds the data row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="internalRework">The internal rework.</param>
        /// <param name="rowIndex">Index of the row.</param>
        private void AddDataRow(Worksheet worksheet, OrdersDataSet.InternalReworkRow internalRework, int rowIndex)
        {
            var dtOrder = new OrdersDataSet.OrderDataTable();
            using (var taOrder = new OrderTableAdapter())
                taOrder.FillByOrderID(dtOrder, internalRework.OriginalOrderID);

            var dtCustomers = new Data.Datasets.CustomersDataset.CustomerDataTable();
            using (var taCust = new CustomerTableAdapter())
                taCust.FillByOrderID(dtCustomers, internalRework.OriginalOrderID);

            var order = dtOrder[0];
            var customerName = dtCustomers[0].Name;

            var reasons = new ListsDataSet.d_ReworkReasonDataTable();
            using (var taReworkReasons = new Data.Datasets.ListsDataSetTableAdapters.d_ReworkReasonTableAdapter())
                taReworkReasons.Fill(reasons);

            this.CreateCell(worksheet, rowIndex, 0, customerName);
            this.CreateCell(worksheet, rowIndex, 4, order.IsPurchaseOrderNull() ? "NA" : order.PurchaseOrder, false, HorizontalCellAlignment.Center);
            this.CreateCell(worksheet, rowIndex, 5, order.IsPriorityNull() ? "NA" : order.Priority, false, HorizontalCellAlignment.Center);
            if(!order.IsRequiredDateNull())
                this.CreateCell(worksheet, rowIndex, 7, order.RequiredDate.Date, false, HorizontalCellAlignment.Center);
            if(!order.IsEstShipDateNull())
                this.CreateCell(worksheet, rowIndex, 8, order.EstShipDate.Date, false, HorizontalCellAlignment.Center);
            this.CreateCell(worksheet, rowIndex, 11, order.IsPartQuantityNull() ? 0 : order.PartQuantity, false, HorizontalCellAlignment.Center);

            if (!internalRework.IsReworkReasonIDNull())
            {
                ListsDataSet.d_ReworkReasonRow reason = reasons.FindByReworkReasonID(internalRework.ReworkReasonID);

                var paDt = new ProcessesDataset.ProcessAliasDataTable();
                if (!internalRework.IsProcessAliasIDNull())
                {
                    using (var taProcessAlias = new Data.Datasets.ProcessesDatasetTableAdapters.ProcessAliasTableAdapter())
                        taProcessAlias.GetAliasName(paDt, internalRework.ProcessAliasID);
                }

                this.CreateCell(worksheet, rowIndex, 1, internalRework.IsReworkOrderIDNull() ? "NA" : internalRework.ReworkOrderID.ToString(), false, HorizontalCellAlignment.Center);
                this.CreateCell(worksheet, rowIndex, 2, internalRework.OriginalOrderID, false, HorizontalCellAlignment.Center);
                this.CreateCell(worksheet, rowIndex, 3, internalRework.ReworkType, false, HorizontalCellAlignment.Center);
                this.CreateCell(worksheet, rowIndex, 6, internalRework.IsHoldLocationIDNull() ? "NA" : internalRework.HoldLocationID, false, HorizontalCellAlignment.Center);
                this.CreateCell(worksheet, rowIndex, 9, reason != null ? reason.Name : "Other", false, HorizontalCellAlignment.Center);
                this.CreateCell(worksheet, rowIndex, 10, paDt.Rows.Count > 0 ? paDt[0].Name : "NA", false, HorizontalCellAlignment.Center);
            }
            else
            {
                this.CreateCell(worksheet, rowIndex, 1, "NA", false, HorizontalCellAlignment.Center);
                this.CreateCell(worksheet, rowIndex, 2, "NA", false, HorizontalCellAlignment.Center);
                this.CreateCell(worksheet, rowIndex, 3, "Other", false, HorizontalCellAlignment.Center);
                this.CreateCell(worksheet, rowIndex, 6, "NA", false, HorizontalCellAlignment.Center);
                this.CreateCell(worksheet, rowIndex, 9, "NA", false, HorizontalCellAlignment.Center);
                this.CreateCell(worksheet, rowIndex, 10, "NA", false, HorizontalCellAlignment.Center);
            }
        }

        /// <summary>
        /// Gets the type of the orders by rework.
        /// </summary>
        /// <param name="internalReworks">The internal reworks.</param>
        /// <returns></returns>
        private Dictionary<string, List<OrdersDataSet.InternalReworkRow>> GetOrdersByReworkType(OrdersDataSet.InternalReworkDataTable internalReworks)
        {
            var ordersByReworkType = new Dictionary<string, List<OrdersDataSet.InternalReworkRow>>();

            try
            {
                foreach (var order in internalReworks)
                {
                    var reworkType = order.ReworkType;

                    if (string.Compare(reworkType, "Full", StringComparison.OrdinalIgnoreCase) == 0
                        || string.Compare(reworkType, "Split", StringComparison.OrdinalIgnoreCase) == 0
                        || string.Compare(reworkType, "SplitHold", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (ordersByReworkType.ContainsKey(reworkType))
                        {
                            var customerSummaryList = ordersByReworkType[reworkType];
                            customerSummaryList.Add(order);
                            ordersByReworkType[reworkType] = customerSummaryList;
                        }
                        else
                        {
                            ordersByReworkType.Add(reworkType, new List<OrdersDataSet.InternalReworkRow> { order });
                        }
                    }
                }

                return ordersByReworkType;
            }
            catch (Exception exc)
            {
                _log.Warn(exc, "Error grouping orders by rework type.");
                return ordersByReworkType;
            }
        }

        /// <summary>
        /// Gets the type of the orders by rework.
        /// </summary>
        /// <param name="internalReworks">The internal reworks.</param>
        /// <returns></returns>
        private Dictionary<string, List<OrdersDataSet.InternalReworkRow>> GetOrdersByReworkType(List<OrdersDataSet.InternalReworkRow> internalReworks)
        {
            var ordersByReworkType = new Dictionary<string, List<OrdersDataSet.InternalReworkRow>>();

            try
            {
                foreach (var order in internalReworks)
                {
                    var reworkType = order.ReworkType;

                    if (string.Compare(reworkType, "Full", StringComparison.OrdinalIgnoreCase) == 0
                        || string.Compare(reworkType, "Split", StringComparison.OrdinalIgnoreCase) == 0
                        || string.Compare(reworkType, "SplitHold", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (ordersByReworkType.ContainsKey(reworkType))
                        {
                            var customerSummaryList = ordersByReworkType[reworkType];
                            customerSummaryList.Add(order);
                            ordersByReworkType[reworkType] = customerSummaryList;
                        }
                        else
                        {
                            ordersByReworkType.Add(reworkType, new List<OrdersDataSet.InternalReworkRow> { order });
                        }
                    }
                }

                return ordersByReworkType;
            }
            catch (Exception exc)
            {
                _log.Warn(exc, "Error grouping orders by rework type.");
                return ordersByReworkType;
            }
        }

        /// <summary>
        /// Gets the orders by customer.
        /// </summary>
        /// <param name="internalReworks">The internal reworks.</param>
        /// <returns></returns>
        private Dictionary<string, List<OrdersDataSet.InternalReworkRow>> GetOrdersByCustomer(OrdersDataSet.InternalReworkDataTable internalReworks)
        {
            var ordersByCustomer = new Dictionary<string, List<OrdersDataSet.InternalReworkRow>>();

            try
            {
                foreach (var order in internalReworks)
                {
                    var dtCustomers = new Data.Datasets.CustomersDataset.CustomerDataTable();
                    using (var taCust = new CustomerTableAdapter())
                    {
                        taCust.FillByOrderID(dtCustomers, order.OriginalOrderID);
                    }

                    var reworkType = order.ReworkType;
                    var customerName = dtCustomers[0].Name;

                    if (string.Compare(reworkType, "Full", StringComparison.OrdinalIgnoreCase) == 0
                        || string.Compare(reworkType, "Split", StringComparison.OrdinalIgnoreCase) == 0
                        || string.Compare(reworkType, "SplitHold", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (ordersByCustomer.ContainsKey(customerName))
                        {
                            var customerSummaryList = ordersByCustomer[customerName];
                            customerSummaryList.Add(order);
                            ordersByCustomer[customerName] = customerSummaryList;
                        }
                        else
                        {
                            ordersByCustomer.Add(customerName, new List<OrdersDataSet.InternalReworkRow> { order });
                        }
                    }
                }
                
                return ordersByCustomer;
            }
            catch (Exception exc)
            {
                _log.Warn(exc, "Error grouping orders by customer.");
                return ordersByCustomer;
            }
        }

        /// <summary>
        /// Gets the orders by customer.
        /// </summary>
        /// <param name="internalReworks">The internal reworks.</param>
        /// <returns></returns>
        private Dictionary<string, List<OrdersDataSet.InternalReworkRow>> GetOrdersByCustomer(List<OrdersDataSet.InternalReworkRow> internalReworks)
        {
            var ordersByCustomer = new Dictionary<string, List<OrdersDataSet.InternalReworkRow>>();

            try
            {
                foreach (var order in internalReworks)
                {
                    var dtCustomers = new Data.Datasets.CustomersDataset.CustomerDataTable();
                    using (var taCust = new CustomerTableAdapter())
                    {
                        taCust.FillByOrderID(dtCustomers, order.OriginalOrderID);
                    }

                    var reworkType = order.ReworkType;
                    var customerName = dtCustomers[0].Name;

                    if (string.Compare(reworkType, "Full", StringComparison.OrdinalIgnoreCase) == 0
                        || string.Compare(reworkType, "Split", StringComparison.OrdinalIgnoreCase) == 0
                        || string.Compare(reworkType, "SplitHold", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (ordersByCustomer.ContainsKey(customerName))
                        {
                            var customerSummaryList = ordersByCustomer[customerName];
                            customerSummaryList.Add(order);
                            ordersByCustomer[customerName] = customerSummaryList;
                        }
                        else
                        {
                            ordersByCustomer.Add(customerName, new List<OrdersDataSet.InternalReworkRow> { order });
                        }
                    }
                }

                return ordersByCustomer;
            }
            catch (Exception exc)
            {
                _log.Warn(exc, "Error grouping orders by customer.");
                return ordersByCustomer;
            }
        }

        /// <summary>
        /// Gets the rework counts.
        /// </summary>
        /// <param name="ordersByCustomer">The orders by customer.</param>
        /// <returns></returns>
        private Dictionary<string, CustomerSummary> GetReworkCounts(Dictionary<string, List<OrdersDataSet.InternalReworkRow>> ordersByCustomer)
        {
            var custSummaryDict = new Dictionary<string, CustomerSummary>();
            foreach (var customerOrder in ordersByCustomer)
            {
                var byReworkType = this.GetOrdersByReworkType(customerOrder.Value);

                if (custSummaryDict.ContainsKey(customerOrder.Key))
                {
                    var customer = custSummaryDict[customerOrder.Key];
                    customer.FullCount += byReworkType.ContainsKey("Full") ? byReworkType["Full"].Count : 0;
                    customer.SplitCount += byReworkType.ContainsKey("Split") ? byReworkType["Split"].Count : 0;
                    customer.SplitHoldCount += byReworkType.ContainsKey("SplitHold") ? byReworkType["SplitHold"].Count : 0;

                    custSummaryDict[customerOrder.Key] = customer;
                }
                else
                {
                    custSummaryDict.Add(customerOrder.Key, new CustomerSummary
                    {
                        CustomerName = customerOrder.Key,
                        FullCount = byReworkType.ContainsKey("Full") ? byReworkType["Full"].Count : 0,
                        SplitCount = byReworkType.ContainsKey("Split") ? byReworkType["Split"].Count : 0,
                        SplitHoldCount = byReworkType.ContainsKey("SplitHold") ? byReworkType["SplitHold"].Count : 0
                    });
                }
            }

            return custSummaryDict;
        }

        /// <summary>
        /// Gets the reworks by reason.
        /// </summary>
        /// <param name="internalReworks">The internal reworks.</param>
        /// <returns></returns>
        private Dictionary<string, List<OrdersDataSet.InternalReworkRow>> GetByReason(OrdersDataSet.InternalReworkDataTable internalReworks)
        {
            var byReasonDictionary = new Dictionary<string, List<OrdersDataSet.InternalReworkRow>>();

            try
            {
                var reasons = new ListsDataSet.d_ReworkReasonDataTable();
                    
                using (var taReworkReasons = new Data.Datasets.ListsDataSetTableAdapters.d_ReworkReasonTableAdapter()) 
                    taReworkReasons.Fill(reasons);
                
                foreach (var order in internalReworks)
                {
                    if(!order.IsReworkReasonIDNull())
                    {
                        var reason = reasons.FindByReworkReasonID(order.ReworkReasonID);

                        if(reason != null)
                        {
                            var reworkType = order.ReworkType;

                            if(string.Compare(reworkType, "Full", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(reworkType, "Split", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(reworkType, "SplitHold", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                if(byReasonDictionary.ContainsKey(reason.Name))
                                {
                                    var list = byReasonDictionary[reason.Name];
                                    list.Add(order);
                                    byReasonDictionary[reason.Name] = list;
                                }
                                else
                                {
                                    byReasonDictionary.Add(reason.Name, new List <OrdersDataSet.InternalReworkRow> {order});
                                }
                            }
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

        #region CustomerSummary

        private class CustomerSummary
        {
            public string CustomerName { get; set; }
            public int FullCount { get; set; }
            public int SplitCount { get; set; }
            public int SplitHoldCount { get; set; }
        }

        #endregion
    }

    public class ExternalReworkReport : ExcelBaseReport
    {
        #region Properties

        public override string Title => "External Rework Orders";

        protected override PageOrientation ReportPageOrientation => PageOrientation.Portrait;

        [Browsable(true)]
        [Description("The from date of the report.")]
        [DisplayName("From Date")]
        [Category("Report")]
        public DateTime FromDate { get; set; }

        [Browsable(true)]
        [Description("The to date of the report.")]
        [DisplayName("To Date")]
        [Category("Report")]
        public DateTime ToDate { get; set; }

        #endregion

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        /// <summary>
        /// Creates the report.
        /// </summary>
        private void CreateReport()
        {
            OrdersReport dsOrderReport = new OrdersReport
            {
                EnforceConstraints = false
            };

            using (var taOrders = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
                taOrders.FillExternalReworkOrders(dsOrderReport.Order, FromDate, ToDate);

            using (var taOrderChange = new Data.Reports.OrdersReportTableAdapters.OrderChangeTableAdapter())
                taOrderChange.Fill(dsOrderReport.OrderChange);

            using (var taOrderChangeReasons = new Data.Reports.OrdersReportTableAdapters.d_OrderChangeReasonTableAdapter())
                taOrderChangeReasons.Fill(dsOrderReport.d_OrderChangeReason);

            var wsSummary = this.CreateWorksheet("External Rework Summary");
            var rowIndex = base.AddCompanyHeaderRows(wsSummary, 3, "Summary") + 2;
            this.AddExternalReworkSummaryData(wsSummary, dsOrderReport.Order, rowIndex);
            wsSummary.Columns[0].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsSummary.Columns[1].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsSummary.Columns[2].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsSummary.Columns[3].SetWidth(20, WorksheetColumnWidthUnit.Character);

            var wsAll = this.CreateWorksheet("All External Reworks");
            this.AddAllExternalReworkOrdersData(wsAll, dsOrderReport.Order);
            wsAll.Columns[0].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[1].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[2].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[3].SetWidth(16, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[4].SetWidth(16, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[5].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[6].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wsAll.Columns[7].SetWidth(30, WorksheetColumnWidthUnit.Character);

            var wsByCustomer = this.CreateWorksheet("External Reworks By Customer");
            this.AddExternalReworkByCustomerData(wsByCustomer, dsOrderReport.Order);
            wsByCustomer.Columns[0].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsByCustomer.Columns[1].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsByCustomer.Columns[2].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsByCustomer.Columns[3].SetWidth(16, WorksheetColumnWidthUnit.Character);
            wsByCustomer.Columns[4].SetWidth(16, WorksheetColumnWidthUnit.Character);
            wsByCustomer.Columns[5].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsByCustomer.Columns[6].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wsByCustomer.Columns[7].SetWidth(30, WorksheetColumnWidthUnit.Character);

            var wsByReason = this.CreateWorksheet("By Reason");
            this.AddExternalReworkByReasonData(wsByReason, dsOrderReport.Order);
            wsByReason.Columns[0].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[1].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[2].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[3].SetWidth(20, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[4].SetWidth(10, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[5].SetWidth(15, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[6].SetWidth(25, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[7].SetWidth(30, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[8].SetWidth(30, WorksheetColumnWidthUnit.Character);
            wsByReason.Columns[9].SetWidth(20, WorksheetColumnWidthUnit.Character);
        }

        /// <summary>
        /// Adds the external rework summary data.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="orders">The orders.</param>
        /// <param name="rowIndex">Index of the row.</param>
        private void AddExternalReworkSummaryData(Worksheet worksheet, OrdersReport.OrderDataTable orders, int rowIndex)
        {
            try
            {
                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 3, "Date(s): " + FromDate.ToShortDateString() + " to " + ToDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the 'All' section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 3, "All Reworks").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;
                this.CreateHeaderCell(worksheet, rowIndex, 0, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 1, "Total Orders").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 2, "Total Parts").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 3, "Cost").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                var orderCount = 0;
                var partCount = 0;
                decimal totalCost = 0;
                this.GetTotalCostAndOrderCount(orders, out orderCount, out partCount, out totalCost);

                this.CreateCell(worksheet, rowIndex, 0, "Total:").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 1, orderCount, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 2, partCount, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                this.CreateCell(worksheet, rowIndex, 3, totalCost, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the 'Rework By Customer' section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 3, "All Reworks By Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;
                this.CreateHeaderCell(worksheet, rowIndex, 0, "Company").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 1, "Total Orders").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 2, "Total Cost").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 3, "Cost").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                var startRowIndex = rowIndex;
                foreach (var kvp in GetOrdersByCustomer(orders))
                {
                    var totalCustomerOrders = 0;
                    var totalCustomerParts = 0;
                    decimal totalCustomerCost = 0;
                    foreach (var customerSummary in kvp.Value)
                    {
                        totalCustomerOrders++;
                        totalCustomerParts += customerSummary.PartQuantity;
                        totalCustomerCost += customerSummary.Price;
                    }

                    this.CreateCell(worksheet, rowIndex, 0, kvp.Key);
                    this.CreateCell(worksheet, rowIndex, 1, totalCustomerOrders, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 2, totalCustomerParts, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 3, totalCustomerCost, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, MONEY_FORMAT);
                    rowIndex++;
                }

                if (startRowIndex > rowIndex)
                {
                    this.CreateCell(worksheet, rowIndex, 0, "Total:").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateFormulaCell(worksheet, rowIndex, 1, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateFormulaCell(worksheet, rowIndex, 2, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 3, rowIndex, 3), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateFormulaCell(worksheet, rowIndex, 3, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 4, rowIndex, 4), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to add 'summary' worksheet to the External Rework Orders Report.");
            }
        }

        /// <summary>
        /// Adds all external rework orders data.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="orders">The orders.</param>
        private void AddAllExternalReworkOrdersData(Worksheet worksheet, OrdersReport.OrderDataTable orders)
        {
            try
            {
                var rowIndex = 0;

                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 7, "Date(s): " + FromDate.ToShortDateString() + " to " + ToDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the 'All' section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 7, "All Reworks").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;
                this.CreateHeaderCell(worksheet, rowIndex, 0, "Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 1, "WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 2, "Priority").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 3, "Date Required").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 4, "Est. Ship Date").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 5, "Part Qty").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 6, "Price").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                this.CreateHeaderCell(worksheet, rowIndex, 7, "Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                var startRowIndex = rowIndex;
                foreach (var order in orders)
                {
                    this.CreateCell(worksheet, rowIndex, 0, order.CustomerName);
                    this.CreateCell(worksheet, rowIndex, 1, order.OrderID, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 2, order.IsPriorityNull() ? "NA" : order.Priority, false, HorizontalCellAlignment.Center);
                    if(!order.IsRequiredDateNull())
                        this.CreateCell(worksheet, rowIndex, 3, order.RequiredDate.Date, false, HorizontalCellAlignment.Center);
                    if(!order.IsEstShipDateNull())
                        this.CreateCell(worksheet, rowIndex, 4, order.EstShipDate.Date, false, HorizontalCellAlignment.Center);
                    this.CreateCell(worksheet, rowIndex, 5, order.IsPartQuantityNull() ? 0 : order.PartQuantity, false, HorizontalCellAlignment.Center);
                    decimal totalCost = 0;

                    decimal fees = OrderPrice.CalculateFees(order, order.BasePrice);
                    if (!order.IsBasePriceNull() || !order.IsPriceUnitNull() || !order.IsPartQuantityNull())
                    {
                        decimal weight = order.IsWeightNull() ? 0M : order.Weight;
                        totalCost = OrderPrice.CalculatePrice(order.BasePrice, order.PriceUnit, fees, order.PartQuantity, weight);
                    }
                    this.CreateCell(worksheet, rowIndex, 6, totalCost, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, MONEY_FORMAT);

                    var changeRows = order.GetOrderChangeRows();

                    if (changeRows.Any())
                    {
                        var change = changeRows.FirstOrDefault();
                        var reasons = change.Getd_OrderChangeReasonRows();
                        if(reasons.Any())
                        {
                            var reason = reasons.FirstOrDefault();
                            this.CreateCell(worksheet, rowIndex, 7, reason.Name, false, HorizontalCellAlignment.Center);
                        }
                    }

                    rowIndex++;
                }

                if (startRowIndex > rowIndex)
                {
                    // Add the total count
                    var cell = this.CreateCell(worksheet, rowIndex, 0, "Totals:");
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    cell = this.CreateFormulaCell(worksheet, rowIndex, 1, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    cell.Value = "Orders: " + cell.Value;
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell = this.CreateFormulaCell(worksheet, rowIndex, 5, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 6, rowIndex, 6), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    cell = this.CreateFormulaCell(worksheet, rowIndex, 6, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 7, rowIndex, 7), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, MONEY_FORMAT);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 7, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to add 'all' worksheet to the External Rework Orders Report.");
            }
        }

        /// <summary>
        /// Adds the external rework by customer data.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="orders">The orders.</param>
        private void AddExternalReworkByCustomerData(Worksheet worksheet, OrdersReport.OrderDataTable orders)
        {
            try
            {
                var rowIndex = 0;
                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 7, "Date(s): " + FromDate.ToShortDateString() + " to " + ToDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the 'All' section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 7, "Reworks By Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                foreach (var kvp in GetOrdersByCustomer(orders))
                {
                    // Add the 'All' section
                    this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 7, kvp.Key).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    rowIndex++;

                    this.CreateHeaderCell(worksheet, rowIndex, 0, "Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 1, "WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 2, "Priority").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 3, "Date Required").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 4, "Est. Ship Date").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 5, "Part Qty").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 6, "Price").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 7, "Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.DimGray), null, FillPatternStyle.Solid);
                    rowIndex++;

                    var startRowIndex = rowIndex;
                    foreach (var order in kvp.Value)
                    {
                        this.CreateCell(worksheet, rowIndex, 0, order.CustomerName);
                        this.CreateCell(worksheet, rowIndex, 1, order.OrderID, false, HorizontalCellAlignment.Center);
                        this.CreateCell(worksheet, rowIndex, 2, order.Priority, false, HorizontalCellAlignment.Center);
                        this.CreateCell(worksheet, rowIndex, 3, order.ReqDate, false, HorizontalCellAlignment.Center);
                        this.CreateCell(worksheet, rowIndex, 4, order.ShipDate, false, HorizontalCellAlignment.Center);
                        this.CreateCell(worksheet, rowIndex, 5, order.PartQuantity, false, HorizontalCellAlignment.Center);
                        this.CreateCell(worksheet, rowIndex, 6, order.Price, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, MONEY_FORMAT);
                        this.CreateCell(worksheet, rowIndex, 7, order.ReworkReason, false, HorizontalCellAlignment.Center);

                        rowIndex++;
                    }

                    // Add the totals 
                    var cell = this.CreateCell(worksheet, rowIndex, 0, "Totals:");
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    cell = this.CreateFormulaCell(worksheet, rowIndex, 1, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    cell.Value = "Orders: " + cell.Value;
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell = this.CreateFormulaCell(worksheet, rowIndex, 5, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 6, rowIndex, 6), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    cell = this.CreateFormulaCell(worksheet, rowIndex, 6, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 7, rowIndex, 7), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, MONEY_FORMAT);
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 7, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                    rowIndex += 2;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to add 'by customer' worksheet to the External Rework Orders Report.");
            }
        }

        /// <summary>
        /// Adds the external rework by reason data.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="orders">The orders.</param>
        private void AddExternalReworkByReasonData(Worksheet worksheet, OrdersReport.OrderDataTable orders)
        {
            try
            {
                var rowIndex = 0;

                // Add the to/from dates
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, "Date(s): " + FromDate.ToShortDateString() + " to " + ToDate.ToShortDateString()).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), null, FillPatternStyle.Solid);
                rowIndex++;

                // Add the 'All' section
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, "Reworks By Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                rowIndex++;

                var ordersByCustomer = this.GetByReason(orders);

                foreach (var kvp in ordersByCustomer)
                {
                    // Add the 'Customer' section by 'Reason'
                    this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 9, kvp.Key).CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                    rowIndex++;

                    this.CreateHeaderCell(worksheet, rowIndex, 0, "Customer").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 1, "WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 2, "Original WO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 3, "PO").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 4, "Priority").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 5, "Date Required").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 6, "Est. Ship Date").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 7, "Reason").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 8, "Notes").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                    this.CreateHeaderCell(worksheet, rowIndex, 9, "Part Qty").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.DimGray), null, FillPatternStyle.Solid);
                    rowIndex++;

                    var startRowIndex = rowIndex;
                    var count = 0;
                    foreach (var order in kvp.Value)
                    {
                        this.AddDataRow(worksheet, order, rowIndex);
                        count++;
                        rowIndex++;
                    }

                    // Add the totals 
                    // Add the total count
                    var cell = this.CreateCell(worksheet, rowIndex, 0, "Totals:");
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    cell = this.CreateFormulaCell(worksheet, rowIndex, 1, "=CountA(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center);
                    cell.Value = "Orders: " + count;
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 7, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 9, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    rowIndex += 2;
                }
                if (ordersByCustomer.Count == 0)
                {
                    this.CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 11, "None", false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex++;

                    // Add the total count
                    this.CreateCell(worksheet, rowIndex, 0, "Totals:").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 1, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 2, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 3, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 4, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 5, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 6, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 7, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 8, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                    this.CreateCell(worksheet, rowIndex, 9, "").CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.LightGray), null, FillPatternStyle.Solid);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to add 'by reason' worksheet to the External Rework Orders Report.");
            }
        }

        /// <summary>
        /// Gets the total cost and order count.
        /// </summary>
        /// <param name="orders">The orders.</param>
        /// <param name="orderCount">The order count.</param>
        /// <param name="partCount">The part count.</param>
        /// <param name="totalCost">The total cost.</param>
        private void GetTotalCostAndOrderCount(OrdersReport.OrderDataTable orders, out int orderCount, out int partCount, out decimal totalCost)
        {
            orderCount = 0;
            partCount = 0;
            totalCost = 0;

            foreach (var order in orders)
            {
                orderCount++;
                partCount += order.IsPartQuantityNull() ? 0 : order.PartQuantity;
                if (!order.IsBasePriceNull() || !order.IsPriceUnitNull() || !order.IsPartQuantityNull())
                {
                    decimal fees = OrderPrice.CalculateFees(order, order.BasePrice);
                    decimal weight = order.IsWeightNull() ? 0M : order.Weight;
                    totalCost += OrderPrice.CalculatePrice(order.BasePrice, order.PriceUnit, fees, order.PartQuantity, weight);
                }
            }
        }

        /// <summary>
        /// Gets the orders by customer.
        /// </summary>
        /// <param name="orders">The orders.</param>
        /// <returns>Dictionary; KEY = customer name, VALUE = List of CustomerSummary objects</returns>
        private static Dictionary<string, List<OrderSummary>> GetOrdersByCustomer(OrdersReport.OrderDataTable orders)
        {
            var ordersByCustomer = new Dictionary<string, List<OrderSummary>>();

            foreach (var order in orders)
            {
                //Get the order rework reason
                string reason = String.Empty;
                var changeRows = order.GetOrderChangeRows();
                if (changeRows.Any())
                {
                    var change = changeRows.FirstOrDefault();
                    var reasons = change.Getd_OrderChangeReasonRows();
                    if (reasons.Any())
                        reason = reasons.FirstOrDefault().Name;
                }

                decimal totalCost = 0;
                if (!order.IsBasePriceNull() || !order.IsPriceUnitNull() || !order.IsPartQuantityNull())
                {
                    decimal fees = OrderPrice.CalculateFees(order, order.BasePrice);
                    decimal weight = order.IsWeightNull() ? 0M : order.Weight;
                    totalCost = OrderPrice.CalculatePrice(order.BasePrice, order.PriceUnit, fees, order.PartQuantity, weight);
                }

                bool hasRequiredDate = order.IsRequiredDateNull();
                bool hasEstShipDate = order.IsEstShipDateNull();

                var customerSummary = new OrderSummary
                {
                    PartQuantity = order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                    Price = totalCost,
                    CustomerName = order.CustomerName,
                    OrderID = order.OrderID,
                    Priority = order.IsPriorityNull() ? "NA" : order.Priority,
                    ReworkReason = reason
                };

                if (!order.IsRequiredDateNull())
                    customerSummary.ReqDate = order.RequiredDate.Date;
                if (!order.IsEstShipDateNull())
                    customerSummary.ShipDate = order.EstShipDate.Date;

                if (ordersByCustomer.ContainsKey(order.CustomerName))
                {
                    var customerSummaryList = ordersByCustomer[order.CustomerName];
                    customerSummaryList.Add(customerSummary);
                    ordersByCustomer[order.CustomerName] = customerSummaryList;
                }
                else
                {
                    ordersByCustomer.Add(order.CustomerName, new List<OrderSummary> { customerSummary });
                }
            }

            return ordersByCustomer;
        }

        private Dictionary<string, List<OrdersReport.OrderRow>> GetByReason(OrdersReport.OrderDataTable orders)
        {
            var byReasonDictionary = new Dictionary<string, List<OrdersReport.OrderRow>>();

            try
            {
                foreach (var order in orders)
                {
                    //Get the order rework reason
                    string reason = string.Empty;
                    var changeRows = order.GetOrderChangeRows();
                    if (changeRows.Any())
                    {
                        var change = changeRows.FirstOrDefault();
                        var reasons = change.Getd_OrderChangeReasonRows();
                        if (reasons.Any())
                            reason = reasons.FirstOrDefault().Name;
                    }

                    if (byReasonDictionary.ContainsKey(reason))
                    {
                        var list = byReasonDictionary[reason];
                        list.Add(order);
                        byReasonDictionary[reason] = list;
                    }
                    else
                    {
                        byReasonDictionary.Add(reason, new List<OrdersReport.OrderRow> { order });
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

        private void AddDataRow(Worksheet worksheet, OrdersReport.OrderRow orderRow, int rowIndex)
        {
            this.CreateCell(worksheet, rowIndex, 0, orderRow.CustomerName);
            this.CreateCell(worksheet, rowIndex, 1, orderRow.OrderID.ToString(), false, HorizontalCellAlignment.Center);

            //Get the order rework reason
            string reason = String.Empty;
            var changeNotes = String.Empty;
            var changeRows = orderRow.GetOrderChangeRows();
            if (changeRows.Any())
            {
                var change = changeRows.FirstOrDefault();

                if (!change.IsNotesNull())
                {
                    changeNotes = change.Notes;
                }

                var reasons = change.Getd_OrderChangeReasonRows();
                if (reasons.Any())
                {
                    reason = reasons.FirstOrDefault().Name;
                    this.CreateCell(worksheet, rowIndex, 2, changeRows.FirstOrDefault().ParentOrderID.ToString(), false, HorizontalCellAlignment.Center);
                }
            }

            this.CreateCell(worksheet, rowIndex, 3, orderRow.IsPurchaseOrderNull() ? string.Empty : orderRow.PurchaseOrder, false, HorizontalCellAlignment.Center);
            this.CreateCell(worksheet, rowIndex, 4, orderRow.IsPriorityNull() ? "NA" : orderRow.Priority, false, HorizontalCellAlignment.Center);
            if (!orderRow.IsRequiredDateNull())
                this.CreateCell(worksheet, rowIndex, 5, orderRow.RequiredDate.Date, false, HorizontalCellAlignment.Center);
            if (!orderRow.IsEstShipDateNull())
                this.CreateCell(worksheet, rowIndex, 6, orderRow.EstShipDate.Date, false, HorizontalCellAlignment.Center);
            this.CreateCell(worksheet, rowIndex, 7, reason != null ? reason : "Unknown", false, HorizontalCellAlignment.Center);
            this.CreateCell(worksheet, rowIndex, 8, changeNotes != null ? changeNotes : "NA", false, HorizontalCellAlignment.Center);
            this.CreateCell(worksheet, rowIndex, 9, orderRow.IsPartQuantityNull() ? 0 : orderRow.PartQuantity, false, HorizontalCellAlignment.Center);
        }

        #endregion

        #region OrderSummary

        private class OrderSummary
        {
            public decimal Price { get; set; }
            public int PartQuantity { get; set; }
            public string CustomerName { get; set; }
            public int OrderID { get; set; }
            public string Priority { get; set; }
            public DateTime ReqDate { get; set; }
            public DateTime ShipDate { get; set; }
            public string ReworkReason { get; set; }
        }

        #endregion
    }

    public class COCDiscrepancyReport : ExcelBaseReport
    {
        #region Fields

        private DateTime _fromDate;
        private DateTime _toDate;

        #endregion Fields

        #region Properties

        public override string Title
        {
            get
            {
                return "COC Discrepancy Report";
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

        #endregion

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            SetupReportInfo();

            using (var ta = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
            {
                var discrepancies = ta.GetCOCDiscrepancies(FromDate.StartOfDay(), ToDate.EndOfDay());
                var wks           = CreateWorksheet(Title);
                var rowIndex      = this.AddHeaderRow(wks);

                foreach(var row in discrepancies)
                {
                    var cocQty = row.IsNull("COCPartQuantity") ? 0 : Convert.ToInt32(row["COCPartQuantity"]);
                    var cocDate = row.IsNull("DateCertified") ? "NA" : Convert.ToDateTime(row["DateCertified"]).ToShortDateString();
                    var orderQty = row.IsPartQuantityNull() ? 0 : row.PartQuantity;
                    
                    this.CreateCell(wks, rowIndex, 0, row.OrderID);
                    this.CreateCell(wks, rowIndex, 1, row.CustomerName);
                    this.CreateCell(wks, rowIndex, 2, row.PartName);
                    this.CreateCell(wks, rowIndex, 3, cocDate);
                    this.CreateCell(wks, rowIndex, 4, cocQty);
                    this.CreateCell(wks, rowIndex, 5, orderQty);

                    var diffCell = this.CreateCell(wks, rowIndex, 6, orderQty - cocQty);
                    diffCell.CellFormat.Fill = Infragistics.Documents.Excel.CellFill.CreateSolidFill(System.Drawing.Color.Red);
                    rowIndex++;
                }

            }
        }

        private int AddHeaderRow(Infragistics.Documents.Excel.Worksheet wks)
        {
            //Add the company info
            var rowIndex = base.AddCompanyHeaderRows(wks, 4, String.Empty) + 2;
            int colIndex = 0;

            CreateHeaderCell(wks, rowIndex, colIndex++, "Order", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Customer", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Part", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Date Certified", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "COC Quantity", 30);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Order Quantity", 30);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Difference", 20);

            return ++rowIndex;
        }

        #endregion Methods

    }
}