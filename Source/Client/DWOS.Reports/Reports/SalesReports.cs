using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AngleSharp;
using AngleSharp.Parser.Html;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Data.Reports;
using DWOS.Data.Reports.OrdersReportTableAdapters;
using DWOS.Data.Reports.ProcessPartsReportTableAdapters;
using DWOS.Data.Reports.QuoteReportTableAdapters;
using DWOS.Reports.Properties;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Itenso.TimePeriod;
using DWOS.Reports.ReportData;
using System.Threading;

namespace DWOS.Reports
{
    public class QuoteReport : Report
    {
        private const int RELATIVE_WIDTH_PART_INFO = 40;
        private const int RELATIVE_WIDTH_FEES = 13;
        private const int RELATIVE_WIDTH_PRICE = 15;
        private const int RELATIVE_WIDTH_EXTENDED = 12;
        private const int RELATIVE_WIDTH_QTY = 8;

        #region Fields

        private readonly QuoteDataSet.QuoteRow _quoteRow;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Quote"; }
        }

        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }

        protected override int FilenameIdentifier => _quoteRow?.QuoteID ?? 0;

        #endregion

        #region Methods

        public QuoteReport(QuoteDataSet.QuoteRow quoteRow) { this._quoteRow = quoteRow; }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            SetupReportInfo();
            _section.PageMargins.All = 20;

            AddHeader("QUOTE " + this._quoteRow.QuoteID, true);
            AddCustomerSection();
            AddPartSection();
            AddNotes();
            AddTerms();
        }

        private void AddCustomerSection()
        {
            var dsCustomer = new CustomersDataset();
            var taCustomer = new CustomerTableAdapter();
            CustomersDataset.CustomerRow customer = null;

            try
            {
                dsCustomer.EnforceConstraints = false;
                taCustomer.FillBy(dsCustomer.Customer, this._quoteRow.CustomerID);
                customer = dsCustomer.Customer[0];

                IGroup headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Paddings.Horizontal = 5;
                headerGroup.Height = new FixedHeight(75);

                //Add Customer Address
                IText companyText = headerGroup.AddText();
                companyText.Alignment = TextAlignment.Left;
                companyText.Style = DefaultStyles.NormalStyle;
                companyText.Width = new RelativeWidth(30);
                companyText.Height = new RelativeHeight(100);
                companyText.Borders = DefaultStyles.DefaultBorders;
                companyText.Paddings.All = 5;
                companyText.Margins.Right = 5;
                companyText.Background = DefaultStyles.DefaultBackground;

                companyText.AddContent("Customer:", DefaultStyles.BlueLargeStyle);
                companyText.AddLineBreak();
                if(this._quoteRow.d_ContactSummaryRow != null && !this._quoteRow.d_ContactSummaryRow.IsNameNull())
                {
                    companyText.AddContent("   ATTN: " + this._quoteRow.d_ContactSummaryRow.Name, DefaultStyles.NormalStyle);
                    companyText.AddLineBreak();
                }
                companyText.AddContent("   " + customer.Name, DefaultStyles.BoldStyle);
                companyText.AddLineBreak();

                if (!customer.IsAddress1Null())
                {
                    companyText.AddContent("   " + customer.Address1, DefaultStyles.NormalStyle);
                }

                companyText.AddLineBreak();
                if (!customer.IsAddress2Null() && !string.IsNullOrWhiteSpace(customer.Address2))
                {
                    companyText.AddContent("   " + customer.Address2, DefaultStyles.NormalStyle);
                    companyText.AddLineBreak();
                }

                companyText.AddContent("   ");

                if (!customer.IsCityNull())
                {
                    companyText.AddContent(customer.City + ", ", DefaultStyles.NormalStyle);
                }

                if (!customer.IsStateNull())
                {
                    companyText.AddContent(customer.State + " ", DefaultStyles.NormalStyle);
                }

                if (!customer.IsZipNull())
                {
                    companyText.AddContent(customer.Zip, DefaultStyles.NormalStyle);
                }

                //Add Order Information
                Infragistics.Documents.Reports.Report.IContainer orderContainer = headerGroup.AddContainer("quote");
                orderContainer.Width = new RelativeWidth(70);
                orderContainer.Height = new RelativeHeight(100);
                orderContainer.Borders = DefaultStyles.DefaultBorders;
                orderContainer.Paddings.All = 5;
                orderContainer.Margins.Right = 5;
                orderContainer.Background = DefaultStyles.DefaultBackground;

                ITable orderTable = orderContainer.AddTable();
                orderTable.AddRow().AddCells(DefaultStyles.BlueLargeStyle, null, 0, TextAlignment.Left, "Quote Information:");
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Quote Number:", this._quoteRow.QuoteID + (this._quoteRow.IsRevisionNull() ? "" : "-" + this._quoteRow.Revision), "RFQ:", this._quoteRow.IsRFQNull() ? "None" : this._quoteRow.RFQ);
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Date:", this._quoteRow.CreatedDate.ToShortDateString(), "Sales Person:", this._quoteRow.UsersRow.Name);
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Expiration:", this._quoteRow.ExpirationDate.ToShortDateString(), "Program:", this._quoteRow.IsProgramNull() ? "NA" : this._quoteRow.Program);
            }
            catch(Exception exc)
            {
                _log.Fatal(exc, "Error adding customer section to COC report.");
            }
            finally
            {
                if(dsCustomer != null)
                    dsCustomer.Dispose();
                if(taCustomer != null)
                    taCustomer.Dispose();
            }
        }

        private void AddPartSection()
        {
            DWOS.Data.Datasets.ProcessesDatasetTableAdapters.CustomerProcessAliasTableAdapter taCustomerProcessAlias = null;
            DWOS.Data.Datasets.ProcessesDataset.CustomerProcessAliasDataTable customerProcessAliases = null;

            try
            {
                taCustomerProcessAlias = new DWOS.Data.Datasets.ProcessesDatasetTableAdapters.CustomerProcessAliasTableAdapter();
                customerProcessAliases = new DWOS.Data.Datasets.ProcessesDataset.CustomerProcessAliasDataTable();

                // Get quantity/totals visibility
                var displayQuantityAndTotals = FieldUtilities.IsFieldEnabled("Quote", "Quantity")
                    && _quoteRow.GetQuotePartRows().All(p => !p.IsQuantityNull());

                taCustomerProcessAlias.FillByCustomerID(customerProcessAliases, _quoteRow.CustomerID, true);
                var priceUnitPersistence = new Data.Order.PriceUnitPersistence();
                var pricePoints = PricePointsFor(_quoteRow, priceUnitPersistence);

                var hasFees = _quoteRow.GetQuotePartRows().Any(p => p.GetQuotePartFeesRows().Any());

                IGroup headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Margins.All = 5;
                headerGroup.Borders = DefaultStyles.DefaultBorders;
                headerGroup.Background = DefaultStyles.DefaultBackground;

                ITable orderTable = headerGroup.AddTable();
                orderTable.Width = new RelativeWidth(100);
                orderTable.CreateTableHeaderCell(RELATIVE_WIDTH_PART_INFO).AddText("Part Information", DefaultStyles.NormalStyle, TextAlignment.Center);

                if (displayQuantityAndTotals)
                {
                    orderTable.CreateTableHeaderCell(RELATIVE_WIDTH_QTY).AddText("Qty", DefaultStyles.NormalStyle, TextAlignment.Center);
                }

                foreach (var pricePoint in pricePoints)
                {
                    ITableCell pointCell = orderTable.CreateTableHeaderCell(CalculateRelativeWidth(RELATIVE_WIDTH_PRICE, pricePoints.Count, hasFees));
                    IText pointText = pointCell.AddText();
                    pointText.Alignment = TextAlignment.Center;
                    pointText.AddContent(GetTitle(pricePoint), DefaultStyles.NormalStyle);
                    pointText.AddLineBreak();

                    var description = GetDescription(pricePoint);
                    if (!string.IsNullOrEmpty(description))
                    {
                        pointText.AddContent(description, DefaultStyles.SmallStyle);
                    }
                }

                if (displayQuantityAndTotals)
                {
                    if (hasFees)
                    {
                        orderTable.CreateTableHeaderCell(CalculateRelativeWidth(RELATIVE_WIDTH_FEES, pricePoints.Count, true))
                                  .AddText("Fees", DefaultStyles.NormalStyle, TextAlignment.Center);
                    }

                    orderTable.CreateTableHeaderCell(RELATIVE_WIDTH_EXTENDED)
                              .AddText("Extended", DefaultStyles.NormalStyle, TextAlignment.Center);
                }

                decimal quoteTotalPrice = 0;
                int totalQty = 0;
                decimal totalWeight = 0;

                //Add each part to the table
                foreach (QuoteDataSet.QuotePartRow quotePart in this._quoteRow.GetQuotePartRows().OrderBy(r => r.Name))
                {
                    var partData = AddPart(
                        QuotePartData.From(quotePart, priceUnitPersistence, hasFees, displayQuantityAndTotals),
                        orderTable,
                        customerProcessAliases,
                        pricePoints);

                    if (partData.PriceBy == PriceByType.Quantity)
                    {
                        totalQty += partData.Quantity;
                    }

                    if (partData.PriceBy == PriceByType.Weight)
                    {
                        totalWeight += partData.Weight;
                    }

                    quoteTotalPrice += partData.TotalPrice;
                }

                if (displayQuantityAndTotals)
                {
                    //Add summary row
                    var weightFormat = $"F{ApplicationSettings.Current.WeightDecimalPlaces}";
                    string totalString;
                    if (totalQty > 0 && totalWeight > 0)
                    {
                        totalString = $"{totalQty} Pieces & {totalWeight.ToString(weightFormat)} lb.";
                    }
                    else if (totalWeight > 0)
                    {
                        totalString = $"{totalWeight.ToString(weightFormat)} lb.";
                    }
                    else
                    {
                        totalString = totalQty.ToString();
                    }

                    ITableRow summaryRow = orderTable.AddRow();
                    summaryRow.CreateTableCell(RELATIVE_WIDTH_PART_INFO).AddText("Total", DefaultStyles.BoldMediumStyle, TextAlignment.Right);
                    summaryRow.CreateTableCell(RELATIVE_WIDTH_QTY).AddText(totalString, DefaultStyles.NormalStyle, TextAlignment.Center);

                    foreach (var pricePoint in pricePoints)
                    {
                        summaryRow.CreateTableCell(CalculateRelativeWidth(RELATIVE_WIDTH_PRICE, pricePoints.Count, hasFees))
                                  .AddText(string.Empty, DefaultStyles.NormalStyle, TextAlignment.Center);
                    }

                    if (hasFees)
                    {
                        summaryRow.CreateTableCell(CalculateRelativeWidth(RELATIVE_WIDTH_FEES, pricePoints.Count, true))
                                  .AddText(string.Empty, DefaultStyles.NormalStyle, TextAlignment.Center);
                    }

                    summaryRow.CreateTableCell(RELATIVE_WIDTH_EXTENDED)
                              .AddText(quoteTotalPrice.ToString(OrderPrice.CurrencyFormatString), DefaultStyles.NormalStyle, TextAlignment.Center);
                }
            }
            catch (Exception exc)
            {
                _log.Fatal(exc, "Error adding part section to COC report.");
            }
            finally
            {
                if (customerProcessAliases != null)
                {
                    customerProcessAliases.Dispose();
                }

                if (taCustomerProcessAlias != null)
                {
                    taCustomerProcessAlias.Dispose();
                }
            }
        }

        private static QuotePartSummary AddPart(QuotePartData quotePart, ITable orderTable,
            ProcessesDataset.CustomerProcessAliasDataTable customerProcessAliases,
            IList<PricePointData> pricePoints)
        {
            const string indent = "  ";
            var processText = new StringBuilder();
            var feeText = new StringBuilder();
            processText.Append(Environment.NewLine + indent + "Processes:");

            ITableRow partRow = orderTable.AddRow();

            var quantity = quotePart.Row.IsQuantityNull()
                ? 0
                : quotePart.Row.Quantity;
            var weight = quotePart.Row.IsTotalWeightNull()
                ? 0M
                : quotePart.Row.TotalWeight;

            //Get all processes
            var processes = quotePart.Row.GetQuotePart_ProcessRows().OrderBy(p => p.StepOrder);
            processes.ForEach((proc) => { if (proc.IsShowOnQuoteNull()) proc.ShowOnQuote = true; });
            if (processes.All(p => p.ShowOnQuote == false))
                processText.Clear();
            else
            {
                foreach (var process in processes)
                {
                    if (!process.ShowOnQuote) continue;

                    string processName;

                    var customerAlias = customerProcessAliases.FirstOrDefault(a => a.ProcessAliasID == process.ProcessAliasID);
                    if (customerAlias != null)
                    {
                        processName = customerAlias.Name;
                    }
                    else
                    {
                        processName = process.ProcessAliasRow.Name;
                    }

                    processText.Append(Environment.NewLine)
                        .Append(indent)
                        .Append(indent)
                        .Append(processName);
                }
            }
            
            //add part marking to list
            if (quotePart.Row.PartMarking)
                processText.Append(Environment.NewLine + indent + indent + "Part Marking");

            //Fee Text
            var preferredPrice = quotePart.Prices
                .FirstOrDefault(p => p.Preferred);

            if (quotePart.ShowFees && quotePart.Row.GetQuotePartFeesRows().Any())
            {
                feeText.Insert(0, Environment.NewLine + indent + "Fees:");

                foreach (var fee in quotePart.Row.GetQuotePartFeesRows())
                {
                    var feeDescription = fee.FeeType;
                    if (!Enum.TryParse<OrderPrice.enumFeeType>(fee.FeeCalculationType, out var feeType))
                    {
                        feeType = OrderPrice.enumFeeType.Fixed;
                    }

                    if (quotePart.ShowQuantityAndTotals &&
                        feeType == OrderPrice.enumFeeType.Percentage)
                    {
                        feeDescription += $" ({fee.Charge / 100:P})";
                    }

                    feeText.Append(Environment.NewLine + indent + indent + feeDescription);

                    if (quotePart.ShowQuantityAndTotals && preferredPrice != null)
                    {
                        var feeTotal = OrderPrice.CalculateFees(fee.FeeCalculationType,
                            fee.Charge,
                            preferredPrice.BasePrice,
                            quotePart.Row.IsQuantityNull() 
                                ? 0 
                                : quotePart.Row.Quantity,
                            preferredPrice.PricePoint.PriceUnit.ToString(),
                            weight);

                        feeText.Append(": " + feeTotal.ToString(OrderPrice.CurrencyFormatString));
                    }
                    else if (feeType == OrderPrice.enumFeeType.Percentage)
                    {
                        var priceUnit = quotePart.Prices.First().PricePoint.PriceUnit;
                        feeText.Append($": {fee.Charge / 100:P}");
                    }
                    else
                    {
                        feeText.Append(": " + fee.Charge.ToString(OrderPrice.CurrencyFormatString));
                    }
                }
            }

            partRow.CreateTableCell(RELATIVE_WIDTH_PART_INFO).AddText(quotePart.PartName + processText + feeText, DefaultStyles.NormalStyle, TextAlignment.Left);

            // Quantity
            if(quotePart.ShowQuantityAndTotals)
            {
                string quantityText;

                if (quotePart.PriceBy == PriceByType.Quantity)
                {
                    quantityText = quantity.ToString();
                }
                else if (quotePart.PriceBy == PriceByType.Weight)
                {
                    var weightFormat = $"F{ApplicationSettings.Current.WeightDecimalPlaces}";
                    quantityText = $"{weight.ToString(weightFormat)} lb.";
                }
                else
                {
                    quantityText = "N/A";
                }

                partRow.CreateTableCell(RELATIVE_WIDTH_QTY).AddText(quantityText, DefaultStyles.NormalStyle, TextAlignment.Center);
            }

            // Price Points
            foreach (var pricePoint in pricePoints)
            {
                // Get price
                var quotePartPrice = quotePart.Prices
                    .FirstOrDefault(price => price.PricePoint.Equals(pricePoint));

                if (quotePartPrice == null)
                {
                    partRow.CreateTableCell(CalculateRelativeWidth(RELATIVE_WIDTH_PRICE, pricePoints.Count, quotePart.ShowFees))
                        .AddText(string.Empty, DefaultStyles.NormalStyle, TextAlignment.Center);
                }
                else
                {
                    var amountText = quotePartPrice.BasePrice.ToString(OrderPrice.CurrencyFormatString);

                    var amountTextStyle = quotePart.ShowQuantityAndTotals && quotePartPrice.Preferred
                        ? DefaultStyles.BoldStyle
                        : DefaultStyles.NormalStyle;

                    partRow.CreateTableCell(CalculateRelativeWidth(RELATIVE_WIDTH_PRICE, pricePoints.Count, quotePart.ShowFees))
                        .AddText(amountText, amountTextStyle, TextAlignment.Center);
                }

            }

            // Fees
            decimal fees = 0;
            decimal partTotalPrice = 0;

            if (quotePart.ShowQuantityAndTotals)
            {
                if (preferredPrice != null)
                {
                    if (quotePart.ShowFees)
                    {
                        foreach (var fee in quotePart.Row.GetQuotePartFeesRows())
                        {
                            fees += OrderPrice.CalculateFees(fee.FeeCalculationType,
                                fee.Charge,
                                preferredPrice.BasePrice,
                                quantity,
                                preferredPrice.PricePoint.PriceUnit.ToString(),
                                weight);
                        }

                        partRow.CreateTableCell(CalculateRelativeWidth(RELATIVE_WIDTH_FEES, pricePoints.Count, true))
                               .AddText(fees.ToString(OrderPrice.CurrencyFormatString));
                    }

                    // Extended
                    partTotalPrice = preferredPrice.CalculatePrice(fees, quantity, weight);
                    partRow.CreateTableCell(RELATIVE_WIDTH_EXTENDED)
                           .AddText(partTotalPrice.ToString(OrderPrice.CurrencyFormatString), DefaultStyles.NormalStyle, TextAlignment.Center);
                }
                else
                {
                    partRow.CreateTableCell(RELATIVE_WIDTH_EXTENDED)
                           .AddText(string.Empty, DefaultStyles.NormalStyle, TextAlignment.Center);
                }
            }

            return new QuotePartSummary
            {
                PriceBy = quotePart.PriceBy,
                Quantity = quantity,
                Weight = weight,
                TotalPrice = partTotalPrice,
            };
        }

        private void AddNotes()
        {
            if(this._quoteRow.IsNotesNull() || String.IsNullOrWhiteSpace(this._quoteRow.Notes))
                return;

            IGroup headerGroup = _section.AddGroup();
            headerGroup.Layout = Layout.Horizontal;
            // headerGroup.Margins.Vertical = 5;

            Infragistics.Documents.Reports.Report.IContainer container = headerGroup.AddContainer("notes");
            container.Width = new RelativeWidth(100);
            container.Alignment.Horizontal = Alignment.Left;
            container.Alignment.Vertical = Alignment.Bottom;

            IText text = container.AddText();
            text.Style = DefaultStyles.NormalStyle;
            text.Borders = DefaultStyles.DefaultBorders;
            text.Paddings.All = 5;
            text.Margins.Horizontal = 5;
            text.Background = DefaultStyles.DefaultBackground;
            text.AddContent("Notes:", DefaultStyles.BlueLargeStyle);
            text.AddLineBreak();
            text.AddContent(this._quoteRow.IsNotesNull() ? "" : this._quoteRow.Notes, DefaultStyles.NormalStyle);
        }

        private void AddTerms()
        {
            IGroup headerGroup = _section.AddGroup();
            headerGroup.Layout = Layout.Horizontal;
            headerGroup.Margins.All = 5;

            Infragistics.Documents.Reports.Report.IContainer container = headerGroup.AddContainer("terms");
            container.Width = new RelativeWidth(100);
            container.Alignment.Horizontal = Alignment.Left;
            container.Alignment.Vertical = Alignment.Bottom;

            IText termsText = container.AddText();
            termsText.Style = DefaultStyles.NormalStyle;
            termsText.Paddings.Horizontal = 5;

            var terms = _quoteRow.d_TermsRow.Terms;
            termsText.AddRichContent(PrepareTerms(terms));
        }

        private static string PrepareTerms(string terms)
        {
            if (string.IsNullOrEmpty(terms))
            {
                return terms;
            }

            // Fix styling - dialog to create terms uses span tags & style
            // attributes instead of b/i tags that work with Infragistics
            // reports.

            // Assumption - terms is valid HTML because its dialog requires it
            var parser = new HtmlParser(Configuration.Default.WithCss());
            var document = parser.Parse(terms.Replace("&edsp;", "&nbsp;"));

            var content = document.QuerySelector("body").ChildNodes.OfType<AngleSharp.Dom.IElement>().ToList();
            foreach (var node in content)
            {
                ConvertStyle(node, document);
            }

            return document.QuerySelector("body").InnerHtml;
        }

        private static void ConvertStyle(AngleSharp.Dom.IElement element, AngleSharp.Dom.IDocument document)
        {
            FixStyle(element);
            var childElements = element.ChildNodes.OfType<AngleSharp.Dom.IElement>().ToList();
            foreach (var childNode in childElements)
            {
                ConvertStyle(childNode, document);
            }

            var textElements = element.ChildNodes.OfType<AngleSharp.Dom.IText>().ToList();

            var computed = element.Style;
            var isBold = computed?.FontWeight == "bold";
            var isItalics = computed?.FontStyle == "italic";

            foreach (var childNode in textElements)
            {
                if (isBold && isItalics)
                {
                    var bold = document.CreateElement("b");
                    var italics = document.CreateElement("i");
                    italics.AppendChild(document.CreateTextNode(childNode.TextContent));
                    bold.AppendChild(italics);

                    element.ReplaceChild(bold, childNode);
                }
                else if (isBold)
                {
                    var bold = document.CreateElement("b");
                    bold.AppendChild(document.CreateTextNode(childNode.TextContent));
                    element.ReplaceChild(bold, childNode);
                }
                else if (isItalics)
                {
                    var italics = document.CreateElement("i");
                    italics.AppendChild(document.CreateTextNode(childNode.TextContent));
                    element.ReplaceChild(italics, childNode);
                }
            }

            element.RemoveAttribute("style");
        }

        /// <summary>
        /// Applies <see cref="AngleSharp.Dom.IElement.ParentElement"/>
        /// bold/italics to <paramref name="element"/> unless the element
        /// manually overrides it.
        /// </summary>
        /// <remarks>
        /// Workaround for possible issue where AngleSharp does not  compute
        /// styles as expected.
        /// </remarks>
        /// <param name="element"></param>
        private static void FixStyle(AngleSharp.Dom.IElement element)
        {
            var isBold = element.Style?.FontWeight == "bold";
            var isWeightNormal = element.Style?.FontWeight == "normal";
            var isItalic = element.Style?.FontStyle == "italic";
            var isStyleNormal = element.Style?.FontStyle == "normal";

            var isParentBold = element.ParentElement?.Style?.FontWeight == "bold";
            var isParentItalic = element.ParentElement?.Style?.FontStyle == "italic";


            if (element.Style != null)
            {
                if (!isBold && isParentBold && !isWeightNormal)
                {
                    element.Style.FontWeight = "bold";
                }

                if (!isItalic && isParentItalic && !isStyleNormal)
                {
                    element.Style.FontStyle = "italic";
                }
            }
        }
        private static int CalculateRelativeWidth(int baseRelativeWidth, int pricePointsCount, bool showFees)
        {
            if (baseRelativeWidth < 0)
            {
                throw new ArgumentException("Width cannot be negative.",
                    nameof(baseRelativeWidth));
            }
            else if (pricePointsCount < 0)
            {
                throw new ArgumentException("Cannot have negative price points",
                    nameof(pricePointsCount));
            }

            int columnCount = pricePointsCount;

            if (showFees)
            {
                columnCount++;
            }

            // These widths were originally designed for 2 price units.
            float ratio = 2 / Convert.ToSingle(columnCount);

            return Convert.ToInt32(baseRelativeWidth * ratio);
        }

        /// <summary>
        /// Creates price points for a quote.
        /// </summary>
        /// <param name="quoteRow">Quote to use</param>
        /// <param name="priceUnitPersistence">Price unit persistence for default values</param>
        /// <returns>An ordered collection of price points.</returns>
        public static IList<PricePointData> PricePointsFor(QuoteDataSet.QuoteRow quoteRow,
            Data.Order.IPriceUnitPersistence priceUnitPersistence)
        {
            if (quoteRow == null)
            {
                throw new ArgumentNullException(nameof(quoteRow));
            }
            else if (priceUnitPersistence == null)
            {
                throw new ArgumentNullException(nameof(priceUnitPersistence));
            }

            if (ApplicationSettings.Current.PartPricingType == PricingType.Process)
            {
                var pricePointHash = new HashSet<PricePointData>();
                foreach (var part in quoteRow.GetQuotePartRows())
                {
                    foreach (var pricePoint in PricePointsFor(part, priceUnitPersistence))
                    {
                        pricePointHash.Add(pricePoint);
                    }
                }

                return pricePointHash
                    .OrderBy(pricePoint => pricePoint.MinQuantity ?? pricePoint.MinWeight ?? 0M)
                    .ToList();
            }
            else
            {
                var priceByTypes = quoteRow.GetQuotePartRows()
                    .Select(part => (PriceByType)Enum.Parse(typeof(PriceByType), part.PriceBy))
                    .ToList();

                return FromPersistence(quoteRow.CustomerID, priceUnitPersistence)
                    .Where(pricePoint => priceByTypes.Contains(OrderPrice.GetPriceByType(pricePoint.PriceUnit)))
                    .OrderBy(pricePoint => pricePoint.MinQuantity ?? pricePoint.MinWeight ?? 0M)
                    .ToList();
            }
        }

        /// <summary>
        /// Creates price points for a part.
        /// </summary>
        /// <param name="quotePartRow">Part to use.</param>
        /// <param name="priceUnitPersistence">Price unit persistence for default values.</param>
        /// <returns>An ordered collection of price points.</returns>
        public static IList<PricePointData> PricePointsFor(QuoteDataSet.QuotePartRow quotePartRow,
            Data.Order.IPriceUnitPersistence priceUnitPersistence)
        {
            if (quotePartRow == null)
            {
                throw new ArgumentNullException(nameof(quotePartRow));
            }
            else if (priceUnitPersistence == null)
            {
                throw new ArgumentNullException(nameof(priceUnitPersistence));
            }

            var priceBy = (PriceByType)Enum.Parse(typeof(PriceByType), quotePartRow.PriceBy);

            var processPrices = quotePartRow
                .GetQuotePart_ProcessRows()
                .SelectMany(proc => proc.GetQuotePartProcessPriceRows());

            var usingVDP = ApplicationSettings.Current.PartPricingType == PricingType.Process
                && processPrices.Any(priceRow => OrderPrice.GetPriceByType(OrderPrice.ParsePriceUnit(priceRow.PriceUnit)) == priceBy && !priceRow.IsMinValueNull());

            if (usingVDP)
            {
                var pricePointHash = new HashSet<PricePointData>();

                foreach (var price in processPrices)
                {
                    var processPricePoint = PricePointData.From(price);

                    if (OrderPrice.GetPriceByType(processPricePoint.PriceUnit) == priceBy)
                    {
                        pricePointHash.Add(processPricePoint);
                    }
                }

                return pricePointHash
                    .OrderBy(point => point.MinQuantity)
                    .ToList();
            }
            else
            {
                return FromPersistence(quotePartRow.QuoteRow.CustomerID, priceUnitPersistence)
                    .Where(pricePoint => OrderPrice.GetPriceByType(pricePoint.PriceUnit) == priceBy)
                    .ToList();
            }
        }

        /// <summary>
        /// Creates a list of <see cref="PricePointData"/> instances using
        /// persisted price point data.
        /// </summary>
        /// <remarks>
        /// Should be ordered by Lot, then Each.
        /// </remarks>
        /// <returns>
        /// A collection of <see cref="PricePointData"/> instances.
        /// </returns>
        public static IList<PricePointData> FromPersistence(int customerId, Data.Order.IPriceUnitPersistence priceUnitPersistence)
        {
            if (priceUnitPersistence == null)
            {
                throw new ArgumentNullException(nameof(priceUnitPersistence));
            }

            var pricePoints = new List<PricePointData>();

            foreach (var priceUnit in OrderPrice.AllPriceUnits)
            {
                var pricePoint = PricePointData.From(
                    priceUnitPersistence.FindByPriceUnitId(customerId, priceUnit.ToString()));

                if (pricePoint == null)
                {
                    _log.Warn($"Did not find {priceUnit} price point.");
                }
                else
                {
                    pricePoints.Add(pricePoint);
                }
            }

            return pricePoints;
        }

        public string GetTitle(PricePointData data)
        {
            if (data == null)
            {
                return null;
            }

            var pricingStrategy = OrderPrice.GetPricingStrategy(data.PriceUnit);

            if (data.PriceUnit == OrderPrice.enumPriceUnit.Each)
            {
                return "Each Price";
            }
            else if (data.PriceUnit == OrderPrice.enumPriceUnit.EachByWeight)
            {
                return "Price";
            }

            return "Minimum Lot Price";
        }

        public string GetDescription(PricePointData data)
        {
            if (data == null || !ApplicationSettings.Current.UsePriceUnitQuantities)
            {
                return null;
            }

            var pricingStrategy = OrderPrice.GetPriceByType(data.PriceUnit);

            if (pricingStrategy == PriceByType.Quantity)
            {
                if (data.MaxQuantity.HasValue)
                {
                    return $"({data.MinQuantity}-{data.MaxQuantity.Value} Pieces)";
                }
                else
                {
                    return $"({data.MinQuantity} or more Pieces)";
                }
            }
            else if (pricingStrategy == PriceByType.Weight)
            {
                if (data.MaxWeight.HasValue)
                {
                    return $"({data.MinWeight}-{data.MaxWeight.Value} lb.)";
                }
                else
                {
                    return $"({data.MinWeight} lb. or more)";
                }
            }

            return null;
        }

        #endregion

        #region QuotePartData

        /// <summary>
        /// Represents quote part data in <see cref="QuoteReport"/>.
        /// </summary>
        private sealed class QuotePartData
        {
            #region Properties

            public QuoteDataSet.QuotePartRow Row
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets a collection of price instances.
            /// </summary>
            /// <remarks>
            /// Should be ordered by quantity and have exactly one instance
            /// where <see cref="QuotePartPrice.Preferred"/> is true.
            /// </remarks>
            public IList<QuotePartPrice> Prices
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the display name of the part.
            /// </summary>
            public string PartName
            {
                get
                {
                    var partName = Row.Name;
                    if (!Row.IsSurfaceAreaNull() && Row.SurfaceArea > 0)
                        partName += " [~" + Row.SurfaceArea.ToString("F2") + " Sq. Inches]";

                    return partName;
                }
            }

            public bool ShowFees { get; private set; }

            public bool ShowQuantityAndTotals { get; private set; }

            public PriceByType PriceBy =>
                (PriceByType)Enum.Parse(typeof(PriceByType), Row.PriceBy);

            #endregion

            #region Methods

            /// <summary>
            /// Creates part data.
            /// </summary>
            /// <param name="row"></param>
            /// <param name="priceUnitPersistence"></param>
            /// <param name="showFees"></param>
            /// <returns></returns>
            public static QuotePartData From(QuoteDataSet.QuotePartRow row,
                Data.Order.IPriceUnitPersistence priceUnitPersistence,
                bool showFees,
                bool showQuantityAndTotals)
            {
                if (row == null)
                {
                    throw new ArgumentNullException(nameof(row));
                }
                else if (priceUnitPersistence == null)
                {
                    throw new ArgumentNullException(nameof(priceUnitPersistence));
                }

                var priceBy = (PriceByType)Enum.Parse(typeof(PriceByType), row.PriceBy);

                var hasProcessPrices = row
                    .GetQuotePart_ProcessRows()
                    .SelectMany(proc => proc.GetQuotePartProcessPriceRows())
                    .Any(priceRow => OrderPrice.GetPriceByType(OrderPrice.ParsePriceUnit(priceRow.PriceUnit)) == priceBy && !priceRow.IsMinValueNull());

                var usingVDP = ApplicationSettings.Current.PartPricingType == PricingType.Process
                    && hasProcessPrices;

                IList<QuotePartPrice> prices;
                if (usingVDP)
                {
                    prices = FromPartProcessPrices(row, priceUnitPersistence);
                }
                else
                {
                    prices = FromPartPrices(row, priceUnitPersistence);
                }

                var partData = new QuotePartData
                {
                    Row = row
                };

                partData.Prices = prices;
                partData.ShowFees = showFees;
                partData.ShowQuantityAndTotals = showQuantityAndTotals;
                return partData;
            }

            private static IList<QuotePartPrice> FromPartProcessPrices(QuoteDataSet.QuotePartRow row,
                Data.Order.IPriceUnitPersistence priceUnitPersistence)
            {
                // Assumption - row uses per-process volume discount pricing
                var pricePoints = PricePointsFor(row, priceUnitPersistence);
                var pointAmounts = new Dictionary<PricePointData, decimal>();
                foreach (var pricePoint in pricePoints)
                {
                    pointAmounts.Add(pricePoint, 0M);
                }

                var processPrices = row.GetQuotePart_ProcessRows()
                    .SelectMany(proc => proc.GetQuotePartProcessPriceRows());

                foreach (var price in processPrices)
                {
                    var pricePoint = PricePointData.From(price);

                    if (pointAmounts.ContainsKey(pricePoint)) // should always be true
                    {
                        pointAmounts[pricePoint] += price.Amount;
                    }
                }

                var prices = new List<QuotePartPrice>();
                foreach (var pointTotal in pointAmounts)
                {
                    var pricePoint = pointTotal.Key;
                    var amount = pointTotal.Value;

                    var preferred = pricePoint.Matches(row.IsQuantityNull() 
                            ? 0 
                            : row.Quantity, 
                        row.IsTotalWeightNull() 
                            ? 0M 
                            : row.TotalWeight);
                    prices.Add(new QuotePartPrice(pricePoint, amount, preferred));
                }

                prices.Sort((priceA, priceB) => priceA.PricePoint.CompareTo(priceB.PricePoint));

                return prices;
            }

            /// <summary>
            /// Creates price points for a quote.
            /// </summary>
            /// <param name="quoteRow">Quote to use</param>
            /// <param name="priceUnitPersistence">Price unit persistence for default values</param>
            /// <returns>An ordered collection of price points.</returns>
            public static IList<PricePointData> PricePointsFor(QuoteDataSet.QuoteRow quoteRow,
                Data.Order.IPriceUnitPersistence priceUnitPersistence)
            {
                if (quoteRow == null)
                {
                    throw new ArgumentNullException(nameof(quoteRow));
                }
                else if (priceUnitPersistence == null)
                {
                    throw new ArgumentNullException(nameof(priceUnitPersistence));
                }

                if (ApplicationSettings.Current.PartPricingType == PricingType.Process)
                {
                    var pricePointHash = new HashSet<PricePointData>();
                    foreach (var part in quoteRow.GetQuotePartRows())
                    {
                        var priceByForQuote = (PriceByType)Enum.Parse(typeof(PriceByType), part.PriceBy);

                        foreach (var pricePoint in PricePointsFor(part, priceUnitPersistence))
                        {
                            if (OrderPrice.GetPriceByType(pricePoint.PriceUnit) == priceByForQuote)
                            {
                                pricePointHash.Add(pricePoint);
                            }
                        }
                    }

                    return pricePointHash
                        .OrderBy(point => point.MinQuantity)
                        .ToList();
                }
                else
                {
                    var priceByTypes = quoteRow.GetQuotePartRows()
                        .Select(part => (PriceByType)Enum.Parse(typeof(PriceByType), part.PriceBy))
                        .ToList();

                    return FromPersistence(quoteRow.CustomerID, priceUnitPersistence)
                        .Where(pricePoint => priceByTypes.Contains(OrderPrice.GetPriceByType(pricePoint.PriceUnit)))
                        .ToList();
                }
            }

            /// <summary>
            /// Creates price points for a part.
            /// </summary>
            /// <param name="quotePartRow">Part to use.</param>
            /// <param name="priceUnitPersistence">Price unit persistence for default values.</param>
            /// <returns>An ordered collection of price points.</returns>
            public static IList<PricePointData> PricePointsFor(QuoteDataSet.QuotePartRow quotePartRow,
                Data.Order.IPriceUnitPersistence priceUnitPersistence)
            {
                if (quotePartRow == null)
                {
                    throw new ArgumentNullException(nameof(quotePartRow));
                }
                else if (priceUnitPersistence == null)
                {
                    throw new ArgumentNullException(nameof(priceUnitPersistence));
                }

                var priceByForQuote = (PriceByType)Enum.Parse(typeof(PriceByType), quotePartRow.PriceBy);

                var processPrices = quotePartRow
                    .GetQuotePart_ProcessRows()
                    .SelectMany(proc => proc.GetQuotePartProcessPriceRows());

                var usingVDP = ApplicationSettings.Current.PartPricingType == PricingType.Process &&
                    processPrices.Any(price => !price.IsMinValueNull());

                if (usingVDP)
                {
                    var pricePointHash = new HashSet<PricePointData>();

                    foreach (var price in processPrices)
                    {
                        var pricePoint = PricePointData.From(price);

                        if (OrderPrice.GetPriceByType(pricePoint.PriceUnit) == priceByForQuote)
                        {
                            pricePointHash.Add(pricePoint);
                        }
                    }

                    var pricePoints = pricePointHash
                        .OrderBy(point => point.MinQuantity ?? point.MinWeight ?? 0M)
                        .ToList();

                    if (pricePoints.Count == 0)
                    {
                        _log.Warn("Quote Part uses VDP but does not have any price points defined for its price-by type.");
                        return FromPersistence(quotePartRow.QuoteRow.CustomerID, priceUnitPersistence)
                            .Where(pricePoint => OrderPrice.GetPriceByType(pricePoint.PriceUnit) == priceByForQuote)
                            .ToList();
                    }

                    return pricePoints;
                }
                else
                {
                    return FromPersistence(quotePartRow.QuoteRow.CustomerID, priceUnitPersistence);
                }
            }

            private static IList<QuotePartPrice> FromPartPrices(QuoteDataSet.QuotePartRow row, Data.Order.IPriceUnitPersistence priceUnitPersistence)
            {
                var priceByForQuote = (PriceByType)Enum.Parse(typeof(PriceByType), row.PriceBy);

                var pricePoints = FromPersistence(row.QuoteRow.CustomerID, priceUnitPersistence)
                    .Where(pricePoint => OrderPrice.GetPriceByType(pricePoint.PriceUnit) == priceByForQuote)
                    .ToList();

                decimal lotPrice = row.IsLotPriceNull() ? 0 : row.LotPrice;
                decimal eachPrice = row.IsEachPriceNull() ? 0 : row.EachPrice;

                PricePointData preferredPricePoint;
                if (ApplicationSettings.Current.UsePriceUnitQuantities)
                {
                    // Preferred contains current qty/weight.
                    preferredPricePoint = pricePoints
                        .FirstOrDefault(point => point.Matches(row.IsQuantityNull() 
                                ? 0
                                : row.Quantity, 
                            row.IsTotalWeightNull() 
                                ? 0M 
                                : row.TotalWeight));
                }
                else
                {
                    // The preferred price unit is the one with the highest total.
                    // Originally implemented as part of TTP 817.
                    if (!row.IsQuantityNull() && lotPrice >= (eachPrice * row.Quantity))
                    {
                        preferredPricePoint = pricePoints
                            .FirstOrDefault(point => OrderPrice.GetPricingStrategy(point.PriceUnit) == PricingStrategy.Lot);
                    }
                    else
                    {
                        preferredPricePoint = pricePoints
                            .FirstOrDefault(point => OrderPrice.GetPricingStrategy(point.PriceUnit) == PricingStrategy.Each);
                    }
                }

                var prices = new List<QuotePartPrice>();

                foreach (var pricePoint in pricePoints)
                {
                    var price = OrderPrice.GetPricingStrategy(pricePoint.PriceUnit) == PricingStrategy.Lot
                        ? lotPrice
                        : eachPrice;

                    prices.Add(new QuotePartPrice(pricePoint, price, pricePoint == preferredPricePoint));
                }

                prices.Sort((priceA, priceB) => priceA.PricePoint.CompareTo(priceB.PricePoint));

                return prices;
            }

            #endregion
        }

        #endregion

        #region QuotePartPrice

        /// <summary>
        /// Represents price data in <see cref="QuotePartData"/>.
        /// </summary>
        private sealed class QuotePartPrice
        {
            #region Properties

            public PricePointData PricePoint
            {
                get;
                private set;
            }

            public decimal BasePrice
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets a value indicating preferred status.
            /// </summary>
            /// <remarks>
            /// If true, indicates that this instance should be used when
            /// calculating the total price of a quote.
            /// </remarks>
            public bool Preferred
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public QuotePartPrice(PricePointData pricePoint, decimal basePrice, bool preferred)
            {
                if (pricePoint == null)
                {
                    throw new ArgumentNullException(nameof(pricePoint));
                }

                PricePoint = pricePoint;
                BasePrice = basePrice;
                Preferred = preferred;
            }

            public decimal CalculatePrice(decimal fees, int quantity, decimal weight)
            {
                return OrderPrice.CalculatePrice(BasePrice,
                    PricePoint.PriceUnit.ToString(),
                    fees,
                    quantity,
                    weight);
            }

            #endregion
        }

        #endregion

        #region QuotePartSummary

        private class QuotePartSummary
        {
            public int Quantity { get; set; }

            public decimal Weight { get; set; }

            public PriceByType PriceBy { get; set; }

            public decimal TotalPrice { get; set; }
        }

        #endregion
    }

    public class QuoteLogReport : ExcelBaseReport
    {
        #region Fields

        private DateTime _fromDate;
        private DateTime _toDate;

        private enum enumStatus
        {
            Open,
            Closed
        };

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Quote Status"; }
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

        #endregion

        #region Methods

        public QuoteLogReport()
        {
            this._fromDate = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            this._toDate = DateUtilities.GetLastDayOfMonth(DateTime.Now);
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            this._fromDate = this._fromDate.StartOfDay();
            this._toDate = this._toDate.EndOfDay();

            CreateSummaryWorksheet();
            CreateQuoteStatusWorksheet();
        }

        /// <summary>
        ///     Creates the summary worksheet.
        /// </summary>
        private void CreateSummaryWorksheet()
        {
            DateTime startMonthDate;
            DateTime endMonthDate;
            DateTime currentMonth;
            int count;
            decimal amount;
            int countConverted = 0;
            int countNonConverted = 0;
            decimal amountConverted = 0;
            decimal amountNonConverted = 0;

            //Get start and end dates for summary
            startMonthDate = DateUtilities.GetFirstDayOfMonth(this._fromDate).StartOfDay();
            endMonthDate = DateUtilities.GetLastDayOfMonth(this._toDate).EndOfDay();

            //Get the total number of months for the summary
            int totalMonths = DateUtilities.TotalMonths(startMonthDate, endMonthDate);
            if(totalMonths == 0)
                totalMonths = 1;

            Worksheet wks = CreateWorksheet("Quote Summary", 1, 3);

            //Create and format column header region
            SetColumnWidths(wks, 0, 0, 18);
            SetColumnWidths(wks, 1, 12, 15);
            CreateMergedHeader(wks, 0, 0, 0, totalMonths + 1, "Quote Summary");
            CreateMergedHeader(wks, 1, 0, 2, 0, "Category");
            CreateMergedHeader(wks, 1, 1, 1, totalMonths, "Date");

            //Create summary based on the number of months derived from the start and end date given by the user
            for(int curMonthIndex = 1; curMonthIndex <= totalMonths; curMonthIndex++)
            {
                //Get the first month or calculate the next month to display
                if(curMonthIndex == 1)
                    currentMonth = startMonthDate;
                else
                    currentMonth = startMonthDate.AddMonths(curMonthIndex - 1);

                //Create the header for the month
                CreateHeaderCell(wks, 2, curMonthIndex, currentMonth.ToString("MMM") + " '" + currentMonth.ToString("yy"));

                //Get the data for the Open quotes for this month
                GetQuoteInfoForMonth(currentMonth, out count, out amount, enumStatus.Open);
                CreateCell(wks, 3, curMonthIndex, count, false, HorizontalCellAlignment.Right);
                CreateCell(wks, 4, curMonthIndex, amount, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);

                //Get the data for the Closed quotes for this month
                GetQuoteInfoForMonth(currentMonth, out count, out amount, enumStatus.Closed);
                CreateCell(wks, 5, curMonthIndex, count, false, HorizontalCellAlignment.Right);
                CreateCell(wks, 6, curMonthIndex, amount, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);

                //Get the data for the Converted and NonConverted quotes for this month
                GetQuotePartsByMonth(currentMonth, out countConverted, out countNonConverted, out amountConverted, out amountNonConverted);
                CreateCell(wks, 7, curMonthIndex, countConverted, false, HorizontalCellAlignment.Right);
                CreateCell(wks, 8, curMonthIndex, amountConverted, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
                CreateCell(wks, 9, curMonthIndex, countNonConverted, false, HorizontalCellAlignment.Right);
                CreateCell(wks, 10, curMonthIndex, amountNonConverted, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);

                //Get the Conversion Ratio for this month, e.g. =(B8/(B8+B10))
                if((countConverted + countNonConverted) == 0)
                    CreateCell(wks, 11, curMonthIndex, 0, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, PERCENT_FORMAT);
                else
                    CreateFormulaCell(wks, 11, curMonthIndex, "=(R8C" + (curMonthIndex + 1) + "/(R8C" + (curMonthIndex + 1) + " + R10C" + (curMonthIndex + 1) + "))", CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, PERCENT_FORMAT);
            }

            CreateMergedHeader(wks, 1, totalMonths + 1, 2, totalMonths + 1, "Total");

            //Create and format row header region
            CreateMergedCell(wks, 3, 0, 4, 0, "Opened");
            CreateMergedCell(wks, 5, 0, 6, 0, "Closed");
            CreateMergedCell(wks, 7, 0, 8, 0, "Converted");
            CreateMergedCell(wks, 9, 0, 10, 0, "Non-Converted");
            CreateCell(wks, 11, 0, "Conversion Ratio");
            wks.Rows[11].Height = 30 * 20; //The default row height in twips (1/20th of a point)

            //Create the cell formulas to calculate the totals for the given date range
            //The rowIndex is 0 based. Cell formulas use the actual row numbers
            CreateFormulaCell(wks, 3, totalMonths + 1, "=SUM(R4C2" + ":R4C" + (totalMonths + 1) + ")", CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin);
            CreateFormulaCell(wks, 4, totalMonths + 1, "=SUM(R5C2" + ":R5C" + (totalMonths + 1) + ")", CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
            CreateFormulaCell(wks, 5, totalMonths + 1, "=SUM(R6C2" + ":R6C" + (totalMonths + 1) + ")", CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin);
            CreateFormulaCell(wks, 6, totalMonths + 1, "=SUM(R7C2" + ":R7C" + (totalMonths + 1) + ")", CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
            CreateFormulaCell(wks, 7, totalMonths + 1, "=SUM(R8C2" + ":R8C" + (totalMonths + 1) + ")", CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin);
            CreateFormulaCell(wks, 8, totalMonths + 1, "=SUM(R9C2" + ":R9C" + (totalMonths + 1) + ")", CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
            CreateFormulaCell(wks, 9, totalMonths + 1, "=SUM(R10C2" + ":R10C" + (totalMonths + 1) + ")", CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin);
            CreateFormulaCell(wks, 10, totalMonths + 1, "=SUM(R11C2" + ":R11C" + (totalMonths + 1) + ")", CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
            CreateFormulaCell(wks, 11, totalMonths + 1, "=SUM(R12C2" + ":R12C" + (totalMonths + 1) + ")", CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin);

            //Get the Conversion Ratio for the entire date range
            if((countConverted + countNonConverted) == 0)
                CreateCell(wks, 11, totalMonths + 1, 0, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, PERCENT_FORMAT);
            else
                CreateFormulaCell(wks, 11, totalMonths + 1, "=(R8C" + (totalMonths + 2) + "/(R8C" + (totalMonths + 2) + " + R10C" + (totalMonths + 2) + "))", CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, PERCENT_FORMAT);
        }

        /// <summary>
        ///     Creates the quote status worksheet.
        /// </summary>
        private void CreateQuoteStatusWorksheet()
        {
            int columnIndex = 0;
            int rowIndex = 0;

            //Run the query and get the data
            var dtQuoteLog = new Data.Reports.QuoteReport.QuoteLogDataTable();
            dtQuoteLog.Constraints.Clear();
            using(var taQuoteLog = new QuoteLogTableAdapter())
                taQuoteLog.FillByQuoteLogByDate(dtQuoteLog, this._fromDate, this._toDate);

            Worksheet wks = CreateWorksheet("Quote Status", 0, 4);
            SetColumnWidths(wks, 0, 15, 20);

            CreateCell(wks, rowIndex, 0, "From Date:", false, HorizontalCellAlignment.Right, CellBorderLineStyle.None);
            CreateCell(wks, rowIndex, 1, this._fromDate.ToShortDateString(), false, HorizontalCellAlignment.Right, CellBorderLineStyle.None);
            CreateCell(wks, ++rowIndex, 0, "To Date:", false, HorizontalCellAlignment.Right, CellBorderLineStyle.None);
            CreateCell(wks, rowIndex, 1, this._toDate.ToShortDateString(), false, HorizontalCellAlignment.Right, CellBorderLineStyle.None);

            //Blank row
            rowIndex = 3;

            CreateHeaderCell(wks, rowIndex, columnIndex, "Created");
            CreateHeaderCell(wks, rowIndex, ++columnIndex, "Expiration");
            CreateHeaderCell(wks, rowIndex, ++columnIndex, "Status");
            CreateHeaderCell(wks, rowIndex, ++columnIndex, "Operator");
            CreateHeaderCell(wks, rowIndex, ++columnIndex, "Quote Number");
            CreateHeaderCell(wks, rowIndex, ++columnIndex, "Customer");
            CreateHeaderCell(wks, rowIndex, ++columnIndex, "Part");
            CreateHeaderCell(wks, rowIndex, ++columnIndex, "Processes");
            CreateHeaderCell(wks, rowIndex, ++columnIndex, "Program");
            CreateHeaderCell(wks, rowIndex, ++columnIndex, "Quantity");
            CreateHeaderCell(wks, rowIndex, ++columnIndex, "Lot");
            CreateHeaderCell(wks, rowIndex, ++columnIndex, "Each");
            CreateHeaderCell(wks, rowIndex, ++columnIndex, "Extended");
            CreateHeaderCell(wks, rowIndex, ++columnIndex, "Conversion");

            //First row for actual data
            columnIndex = 0;
            rowIndex = 4;
            foreach(Data.Reports.QuoteReport.QuoteLogRow quote in dtQuoteLog)
            {
                CreateCell(wks, rowIndex, columnIndex++, quote.CreatedDate.ToShortDateString());
                CreateCell(wks, rowIndex, columnIndex++, quote.ExpirationDate.ToShortDateString());
                CreateCell(wks, rowIndex, columnIndex++, quote.Status);
                CreateCell(wks, rowIndex, columnIndex++, quote.Operator);
                CreateCell(wks, rowIndex, columnIndex++, quote.QuoteID, false, HorizontalCellAlignment.Right);
                CreateCell(wks, rowIndex, columnIndex++, quote.CustomerName);
                SetColumnWidths(wks, columnIndex, columnIndex, 35);
                CreateCell(wks, rowIndex, columnIndex++, quote.PartName);

                //Get the process for the quotepart
                var sbProcess = new StringBuilder();
                int count = 0;
                var dtQuotePartProcess = new Data.Reports.QuoteReport.QuotePartProcessDataTable();
                using (var taQuotePartProcess = new QuotePartProcessTableAdapter())
                    taQuotePartProcess.FillByQuotePartID(dtQuotePartProcess, quote.QuotePartID);

                foreach (var process in dtQuotePartProcess)
                {
                    if(count > 0)
                        sbProcess.Append("/" + process.Name);
                    else
                        sbProcess.Append(process.Name);

                    count++;
                }

                SetColumnWidths(wks, columnIndex, columnIndex, 35);
                CreateCell(wks, rowIndex, columnIndex++, sbProcess.ToString());
                CreateCell(wks, rowIndex, columnIndex++, quote.IsProgramNull() ? " " : quote.Program);
                CreateCell(wks, rowIndex, columnIndex++, quote.IsQuantityNull() ? 0 : quote.Quantity, false, HorizontalCellAlignment.Right);
                CreateCell(wks, rowIndex, columnIndex++, quote.IsLotPriceNull() ? 0 : quote.LotPrice, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
                CreateCell(wks, rowIndex, columnIndex++, quote.IsEachPriceNull() ? 0 : quote.EachPrice, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);

                //The rowIndex is 0 based. Cell formulas use the actual row numbers
                if(IsLotPricing(quote))
                    CreateFormulaCell(wks, rowIndex, columnIndex++, "=J" + (rowIndex + 1) + "*K" + (rowIndex + 1), CellReferenceMode.A1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
                else
                    CreateFormulaCell(wks, rowIndex, columnIndex++, "=J" + (rowIndex + 1) + "*L" + (rowIndex + 1), CellReferenceMode.A1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);

                //See if it was converted in the time frame given
                var dtPartConverted = new Data.Reports.QuoteReport.PartIdAndOrderDateDataTable();
                dtPartConverted.Constraints.Clear();
                using(var taQuoteLog = new PartIdAndOrderDateTableAdapter())
                    taQuoteLog.FillByPartIdAndOrderDate(dtPartConverted, quote.QuotePartID, DateUtilities.GetFirstDayOfMonth(this._fromDate), DateUtilities.GetLastDayOfMonth(this._toDate));

                if(dtPartConverted.Rows.Count == 0)
                    CreateCell(wks, rowIndex, columnIndex, "N", false, HorizontalCellAlignment.Center);
                else if(dtPartConverted.Rows.Count == 1)
                    CreateCell(wks, rowIndex, columnIndex, "Y", false, HorizontalCellAlignment.Center);

                columnIndex = 0;
                rowIndex++;
            }
        }

        /// <summary>
        ///     Gets the quote parts by month.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <param name="countConverted">The count converted.</param>
        /// <param name="countNonConverted">The count non converted.</param>
        /// <param name="amountConverted">The amount converted.</param>
        /// <param name="amountNonConverted">The amount non converted.</param>
        private void GetQuotePartsByMonth(DateTime month, out int countConverted, out int countNonConverted, out decimal amountConverted, out decimal amountNonConverted)
        {
            countConverted = 0;
            countNonConverted = 0;
            amountConverted = 0;
            amountNonConverted = 0;

            //Get the quotes for the full date range
            var dtQuoteLog = new Data.Reports.QuoteReport.QuoteLogDataTable();
            dtQuoteLog.Constraints.Clear();
            using(var taQuoteLog = new QuoteLogTableAdapter())
                taQuoteLog.FillByQuoteLogByDate(dtQuoteLog, DateUtilities.GetFirstDayOfMonth(this._fromDate), DateUtilities.GetLastDayOfMonth(this._toDate));

            //Get the quote part and see if any orders contain the part for this month
            foreach(Data.Reports.QuoteReport.QuoteLogRow quote in dtQuoteLog)
            {
                var dtPartConverted = new Data.Reports.QuoteReport.PartIdAndOrderDateDataTable();
                dtPartConverted.Constraints.Clear();
                using(var taQuoteLog = new PartIdAndOrderDateTableAdapter())
                    taQuoteLog.FillByPartIdAndOrderDate(dtPartConverted, quote.QuotePartID, DateUtilities.GetFirstDayOfMonth(month), DateUtilities.GetLastDayOfMonth(month));

                //Not Converted this month
                if(dtPartConverted.Rows.Count == 0)
                {
                    //So we don't double count across months
                    if(quote.CreatedDate.Month == month.Month && quote.CreatedDate.Year == month.Year)
                    {
                        countNonConverted++;
                        amountNonConverted += DeterminePricing(quote);
                    }
                }
                else if(dtPartConverted.Rows.Count == 1) //Converted this month
                {
                    countConverted++;
                    amountConverted += DeterminePricing(quote);
                }
            }
        }

        /// <summary>
        ///     Gets the quote info for month.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="count">The count.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="status">The status.</param>
        private void GetQuoteInfoForMonth(DateTime date, out int count, out decimal amount, enumStatus status)
        {
            count = 0;
            amount = 0;
            var dtQuoteLog = new Data.Reports.QuoteReport.QuoteLogDataTable();

            //Get the months data
            dtQuoteLog.Constraints.Clear();
            using(var taQuoteLog = new QuoteLogTableAdapter())
                taQuoteLog.FillByQuoteDateAndStatus(dtQuoteLog, DateUtilities.GetFirstDayOfMonth(date), DateUtilities.GetLastDayOfMonth(date), status.ToString());

            //Sum up the counts
            foreach(Data.Reports.QuoteReport.QuoteLogRow quote in dtQuoteLog)
            {
                count++;
                amount += DeterminePricing(quote);
            }
        }

        /// <summary>
        ///     Determines the pricing based on LotPrice or EachPrice
        /// </summary>
        /// <param name="quote">The quote.</param>
        /// <returns></returns>
        private decimal DeterminePricing(Data.Reports.QuoteReport.QuoteLogRow quote)
        {
            var quantity = quote.IsQuantityNull() ? 0 : quote.Quantity;
            var weight = quote.IsWeightNull() ? 0 : quote.Weight;
            var priceUnits = new Data.Order.PriceUnitPersistence();

            OrderPrice.enumPriceUnit priceUnit;
            if (quote.PriceBy == nameof(PriceByType.Quantity))
            {
                var priceUnitEach = priceUnits.FindByPriceUnitId(quote.CustomerID, nameof(OrderPrice.enumPriceUnit.Each));

                priceUnit = quantity < priceUnitEach.MinQuantity
                    ? OrderPrice.enumPriceUnit.Lot
                    : OrderPrice.enumPriceUnit.Each;
            }
            else if (quote.PriceBy == nameof(PriceByType.Weight))
            {
                var priceUnitEach = priceUnits.FindByPriceUnitId(quote.CustomerID, nameof(OrderPrice.enumPriceUnit.EachByWeight));

                priceUnit = weight < priceUnitEach.MinWeight
                    ? OrderPrice.enumPriceUnit.LotByWeight
                    : OrderPrice.enumPriceUnit.EachByWeight;
            }
            else
            {
                priceUnit = OrderPrice.enumPriceUnit.Each;
            }

            decimal basePrice;
            if (OrderPrice.GetPricingStrategy(priceUnit) == PricingStrategy.Each)
            {
                basePrice = quote.IsEachPriceNull() ? 0M : quote.EachPrice;
            }
            else
            {
                basePrice = quote.IsLotPriceNull() ? 0M : quote.LotPrice;
            }

            return OrderPrice.CalculatePrice(
                basePrice,
                priceUnit.ToString(),
                0M,
                quantity,
                weight);
        }

        /// <summary>
        ///     Determines whether [is lot pricing] [the specified quote].
        /// </summary>
        /// <param name="quote">The quote.</param>
        /// <returns>
        ///     <c>true</c> if [is lot pricing] [the specified quote]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsLotPricing(Data.Reports.QuoteReport.QuoteLogRow quote)
        {
            var quantity = quote.IsQuantityNull() ? 0 : quote.Quantity;
            var weight = quote.IsWeightNull() ? 0 : quote.Weight;
            var priceUnits = new Data.Order.PriceUnitPersistence();

            OrderPrice.enumPriceUnit priceUnit;
            if (quote.PriceBy == nameof(PriceByType.Quantity))
            {
                var priceUnitEach = priceUnits.FindByPriceUnitId(quote.CustomerID, nameof(OrderPrice.enumPriceUnit.Each));

                priceUnit = quantity < priceUnitEach.MinQuantity
                    ? OrderPrice.enumPriceUnit.Lot
                    : OrderPrice.enumPriceUnit.Each;
            }
            else if (quote.PriceBy == nameof(PriceByType.Weight))
            {
                var priceUnitEach = priceUnits.FindByPriceUnitId(quote.CustomerID, nameof(OrderPrice.enumPriceUnit.EachByWeight));

                priceUnit = weight <= priceUnitEach.MinWeight
                    ? OrderPrice.enumPriceUnit.LotByWeight
                    : OrderPrice.enumPriceUnit.EachByWeight;
            }
            else
            {
                priceUnit = OrderPrice.enumPriceUnit.Each;
            }

            return OrderPrice.GetPricingStrategy(priceUnit) == PricingStrategy.Lot;
        }

        #endregion
    }

    public class UnInvoicedOrdersReport : ExcelBaseReport
    {
        #region Properties

        public override string Title
        {
            get { return "Uninvoiced Orders"; }
        }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Landscape; }
        }

        #endregion

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            CreateWorkBook();
            var worksheet = CreateWorksheet(Title);

            var rowIndex = AddCompanyHeaderRows(worksheet, 8, string.Empty) + 2;
            var startingRowIndex = rowIndex;

            CreateHeaderCell(worksheet, rowIndex, 0, "WO", 20);
            CreateHeaderCell(worksheet, rowIndex, 1, "Customer", 35);
            CreateHeaderCell(worksheet, rowIndex, 2, "Date Required", 25);
            CreateHeaderCell(worksheet, rowIndex, 3, "Date Completed", 25);
            CreateHeaderCell(worksheet, rowIndex, 4, "Priority", 20);
            CreateHeaderCell(worksheet, rowIndex, 5, "PO", 30);
            CreateHeaderCell(worksheet, rowIndex, 6, "Price", 30);
            CreateHeaderCell(worksheet, rowIndex, 7, "Part Name", 35);
            CreateHeaderCell(worksheet, rowIndex, 8, "Serial Numbers", 30);

            rowIndex++;

            //Orders Summary
            var expeditePriorities = new List<string>
            {
                "Expedite",
                "First Priority",
                "Weekend Expedite"
            };

            var rushPriorities = new List<string>
            {
                "Rush"
            };

            var orders = GetOrders();

            foreach (var order in orders)
            {
                worksheet.Rows[rowIndex].Cells[0].Value = order.OrderId;
                worksheet.Rows[rowIndex].Cells[1].Value = order.CustomerName;

                if (order.RequiredDate.HasValue)
                {
                    worksheet.Rows[rowIndex].Cells[2].Value =
                        order.RequiredDate;

                    worksheet.Rows[rowIndex].Cells[2].CellFormat.FormatString =
                        DATE_FORMAT;
                }
                else
                {
                    worksheet.Rows[rowIndex].Cells[2].Value =
                        "NA";
                }

                if (order.CompletedDate.HasValue)
                {
                    worksheet.Rows[rowIndex].Cells[3].Value =
                        order.CompletedDate;

                    worksheet.Rows[rowIndex].Cells[3].CellFormat.FormatString =
                        DATE_FORMAT;

                    if (order.CompletedDate.Value < DateTime.Now.AddDays(10))
                    {
                        worksheet.Rows[rowIndex].Cells[3].CellFormat.Font.ColorInfo =
                            new WorkbookColorInfo(Color.Red);
                    }
                }
                else
                {
                    worksheet.Rows[rowIndex].Cells[3].Value = "NA";
                }

                worksheet.Rows[rowIndex].Cells[4].Value = order.Priority;

                if (expeditePriorities.Contains(order.Priority))
                {
                    worksheet.Rows[rowIndex].Cells[4].CellFormat.Font.ColorInfo =
                        new WorkbookColorInfo(Color.Red);

                    worksheet.Rows[rowIndex].Cells[4].CellFormat.Font.Bold =
                        ExcelDefaultableBoolean.True;
                }
                else if (rushPriorities.Contains(order.Priority))
                {
                    worksheet.Rows[rowIndex].Cells[4].CellFormat.Font.ColorInfo =
                        new WorkbookColorInfo(Color.DarkOrange);
                }

                worksheet.Rows[rowIndex].Cells[5].Value = order.PurchaseOrder;

                if (order.TotalPrice.HasValue)
                {
                    worksheet.Rows[rowIndex].Cells[6].Value = order.TotalPrice;

                    worksheet.Rows[rowIndex].Cells[6].CellFormat.FormatString =
                        MONEY_FORMAT;

                    if (order.TotalPrice.Value <= 0M)
                    {
                        worksheet.Rows[rowIndex].Cells[6].CellFormat.Font.ColorInfo =
                            new WorkbookColorInfo(Color.Red);
                    }
                }
                else
                {
                    worksheet.Rows[rowIndex].Cells[6].Value = "Unknown";

                    worksheet.Rows[rowIndex].Cells[6].CellFormat.Font.ColorInfo =
                        new WorkbookColorInfo(Color.Red);

                    worksheet.Rows[rowIndex].Cells[6].CellFormat.Alignment = HorizontalCellAlignment.Right;
                }

                worksheet.Rows[rowIndex].Cells[7].Value = order.PartName;
                worksheet.Rows[rowIndex].Cells[8].Value = string.Join(", ", order.SerialNumbers);

                rowIndex++;
            }

            CreateTable(worksheet, startingRowIndex + 1, 8, rowIndex, true);

            // Totals
            CreateHeaderCell(worksheet, rowIndex, 0, "Count:");
            Bold(CreateFormulaCell(worksheet, rowIndex, 1, $"=COUNT(R{startingRowIndex + 2}C1:R{rowIndex}C1)", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center));
            CreateMergedHeader(worksheet, rowIndex, 2, rowIndex, 5, "Total:").CellFormat.Alignment = HorizontalCellAlignment.Right;
            Bold(CreateFormulaCell(worksheet, rowIndex, 6, $"=SUM(R{startingRowIndex + 2}C7:R{rowIndex}C7)", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center));
            CreateHeaderCell(worksheet, rowIndex, 7, string.Empty);
            CreateHeaderCell(worksheet, rowIndex, 8, string.Empty);
        }

        private static List<UninvoicedOrder> GetOrders()
        {
            OrdersReport dsOrdersReport = new OrdersReport();
            dsOrdersReport.EnforceConstraints = false;

            using (var taOrder = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
            {
                taOrder.FillUnInvoicedOrders(dsOrdersReport.Order);
            }

            using (var taOrderFees = new Data.Reports.OrdersReportTableAdapters.OrderFeesTableAdapter() { ClearBeforeFill = false })
            {
                foreach (var order in dsOrdersReport.Order)
                {
                    taOrderFees.FillByOrder(dsOrdersReport.OrderFees, order.OrderID);
                }
            }

            using (var taFeeType = new Data.Reports.OrdersReportTableAdapters.OrderFeeTypeTableAdapter())
            {
                taFeeType.Fill(dsOrdersReport.OrderFeeType);
            }

            using (var taOrderSerialNumbers = new Data.Reports.OrdersReportTableAdapters.OrderSerialNumberTableAdapter())
            {
                taOrderSerialNumbers.FillActiveForUnInvoiced(dsOrdersReport.OrderSerialNumber);
            }

            return dsOrdersReport.Order
                .Select(UninvoicedOrder.From)
                .ToList();
        }

        #endregion

        #region UninvoicedOrder

        private class UninvoicedOrder
        {
            public int OrderId { get; private set; }

            public string CustomerName { get; private set; }

            public DateTime? RequiredDate { get; private set; }

            public DateTime? CompletedDate { get; private set; }

            public string Priority { get; private set; }

            public string PurchaseOrder { get; private set; }

            public decimal? BasePrice { get; private set; }

            public decimal? TotalPrice { get; private set; }

            public string PartName { get; private set; }

            public List<string> SerialNumbers { get; private set; }

            public static UninvoicedOrder From(OrdersReport.OrderRow orderRow)
            {
                if (orderRow == null)
                {
                    return null;
                }

                //Price
                decimal? price = null;

                if (!orderRow.IsBasePriceNull() && !orderRow.IsPriceUnitNull() && !orderRow.IsPartQuantityNull())
                {
                    decimal weight = orderRow.IsWeightNull() ? 0M : orderRow.Weight;
                    decimal fees = OrderPrice.CalculateFees(orderRow, orderRow.BasePrice);
                    price = OrderPrice.CalculatePrice(orderRow.BasePrice, orderRow.PriceUnit, fees, orderRow.PartQuantity, weight);
                }

                return new UninvoicedOrder
                {
                    OrderId = orderRow.OrderID,
                    CustomerName = orderRow.CustomerName,
                    RequiredDate = orderRow.IsRequiredDateNull()
                        ? (DateTime?)null
                        : orderRow.RequiredDate,

                    CompletedDate = orderRow.IsCompletedDateNull()
                        ? (DateTime?)null
                        : orderRow.CompletedDate,

                    Priority = orderRow.IsPriorityNull()
                        ? "NA"
                        : orderRow.Priority,

                    PurchaseOrder = orderRow.IsPurchaseOrderNull()
                        ? "NA"
                        : orderRow.PurchaseOrder,

                    BasePrice = orderRow.IsBasePriceNull()
                        ? (decimal?)null
                        : orderRow.BasePrice,

                    TotalPrice = price,

                    PartName = orderRow.PartName,

                    SerialNumbers = orderRow
                        .GetOrderSerialNumberRows()
                        .Select(s => s.IsNumberNull() ? string.Empty : s.Number)
                        .ToList()
                };
            }
        }

        #endregion
    }

    public class ProductionSalesReportExcel : ExcelBaseReport
    {
        #region Fields

        public enum GroupByDate
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
            get { return "Production"; }
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
        public GroupByDate GroupBy { get; set; }

        #endregion

        #region Methods

        public ProductionSalesReportExcel()
        {
            this._fromDate = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            this._toDate = DateUtilities.GetLastDayOfMonth(DateTime.Now);
            GroupBy = GroupByDate.Day;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            //ensure to get correct time with same date
            this._fromDate = new DateTime(this._fromDate.Year, this._fromDate.Month, this._fromDate.Day, 0, 0, 0);
            this._toDate = new DateTime(this._toDate.Year, this._toDate.Month, this._toDate.Day, 23, 59, 59);

            var departments = LoadSummaries(this._fromDate, this._toDate);

            CreateSummary(departments);
            CreateDetails(departments);
        }

        private void CreateSummary(List<DepartmentSummary> departments)
        {
            try
            {
                var worksheet = CreateWorksheet("Summary");

                var rowIndex = base.AddCompanyHeaderRows(worksheet, 4, "Sales") + 2;
                int column = 0;

                CreateHeaderCell(worksheet, rowIndex, column++, "Department", 20);
                CreateHeaderCell(worksheet, rowIndex, column++, "Orders", 20);
                CreateHeaderCell(worksheet, rowIndex, column++, "Parts", 20);
                CreateHeaderCell(worksheet, rowIndex, column++, "Surface Area", 20);
                CreateHeaderCell(worksheet, rowIndex, column++, "Price", 20);
                rowIndex++;
                var startRowIndex = rowIndex;
                var deptSummaries = SumByDept(departments);

                foreach (var departmentSummary in deptSummaries)
                {
                    CreateCell(worksheet, rowIndex, 0, departmentSummary.Department);
                    CreateCell(worksheet, rowIndex, 1, departmentSummary.Orders, cellFormat: "#,###");
                    CreateCell(worksheet, rowIndex, 2, departmentSummary.PartQuantity, cellFormat: "#,###");
                    CreateCell(worksheet, rowIndex, 3, departmentSummary.TotalSA, cellFormat: "#,###.00");
                    CreateCell(worksheet, rowIndex, 4, departmentSummary.Price, false, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT);
                    rowIndex++;
                }

                if (deptSummaries.Count == 0)
                {
                    for (int colIndex = 0; colIndex < 5; colIndex++)
                    {
                        CreateCell(worksheet, rowIndex, colIndex, string.Empty);
                    }

                    rowIndex++;
                }

                //Add Total
                this.CreateCell(worksheet, rowIndex, 0, "Total:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 1, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, true, cellFormat: "#,###").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 2, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 3, rowIndex, 3), CellReferenceMode.R1C1, true, cellFormat: "#,###").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 3, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 4, rowIndex, 4), CellReferenceMode.R1C1, true, cellFormat: "#,###.00").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 4, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 5, rowIndex, 5), CellReferenceMode.R1C1, true, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                rowIndex++;

                //Add Average
                this.CreateCell(worksheet, rowIndex, 0, "Average:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 1, "=AVERAGE(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex - 1, 2), CellReferenceMode.R1C1, true, cellFormat: "#,###").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 2, "=AVERAGE(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 3, rowIndex - 1, 3), CellReferenceMode.R1C1, true, cellFormat: "#,###").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 3, "=AVERAGE(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 4, rowIndex - 1, 4), CellReferenceMode.R1C1, true, cellFormat: "#,###.00").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(worksheet, rowIndex, 4, "=AVERAGE(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 5, rowIndex - 1, 5), CellReferenceMode.R1C1, true, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to create Late Order Summary Report.");
            }
        }

        private List<DepartmentSummary> SumByDept(List<DepartmentSummary> departments)
        {
            var departmentSummaries = new List<DepartmentSummary>();

            var departmentGroups = departments.GroupBy(d => d.Department);
            foreach (var departmentGroup in departmentGroups)
            {
                var dept = new DepartmentSummary { Department = departmentGroup.Key };
                departmentGroup.ForEach(dg =>
                {
                    dept.Orders += dg.Orders;
                    dept.PartsWithTotalSA += dg.PartsWithTotalSA;
                    dept.TotalSA += dg.TotalSA;
                    dept.Price += dg.Price;

                    try
                    {
                        dept.PartQuantity += dg.PartQuantity;
                    }
                    catch (OverflowException)
                    {
                        _log.Warn("Arithmetic overflow occurred while running report");
                        dept.PartQuantity = long.MaxValue;
                    }
                });

                departmentSummaries.Add(dept);
            }

            departmentSummaries.Sort((d1, d2) => d1.Department.CompareTo(d2.Department));
            return departmentSummaries;
        }

        private void CreateDetails(List<DepartmentSummary> departments)
        {
            try
            {
                var distinctNames = departments.Convert(ds => ds.Department).Distinct();

                foreach (var deptName in distinctNames)
                {
                    var worksheet = CreateWorksheet(deptName);

                    var rowIndex = base.AddCompanyHeaderRows(worksheet, 4, " - " + deptName) + 2;
                    int column = 0;

                    CreateHeaderCell(worksheet, rowIndex, column++, "Date", 20);
                    CreateHeaderCell(worksheet, rowIndex, column++, "Orders", 20);
                    CreateHeaderCell(worksheet, rowIndex, column++, "Parts", 20);
                    CreateHeaderCell(worksheet, rowIndex, column++, "Surface Area", 20);
                    CreateHeaderCell(worksheet, rowIndex++, column++, "Price", 20);
                    rowIndex++;
                    var startRowIndex = rowIndex;

                    var deptSummaries = departments.Where(d => d.Department == deptName).ToList();
                    deptSummaries.Sort((d1, d2) => d1.DateTimeStamp.CompareTo((d2.DateTimeStamp)));

                    var orderCounts = new List<double>();
                    var partCounts = new List<double>();
                    var saCounts = new List<double>();
                    var priceCounts = new List<double>();

                    foreach (var departmentSummary in deptSummaries)
                    {
                        CreateCell(worksheet, rowIndex, 0, new DateTime(departmentSummary.DateTimeStamp));
                        CreateCell(worksheet, rowIndex, 1, departmentSummary.Orders, cellFormat: "#,###");
                        CreateCell(worksheet, rowIndex, 2, departmentSummary.PartQuantity, cellFormat: "#,###");
                        CreateCell(worksheet, rowIndex, 3, departmentSummary.TotalSA, cellFormat: "#,###.00");
                        CreateCell(worksheet, rowIndex, 4, departmentSummary.Price, false, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT);

                        orderCounts.Add(departmentSummary.Orders);
                        partCounts.Add(departmentSummary.PartQuantity);
                        saCounts.Add(departmentSummary.TotalSA);
                        priceCounts.Add(Convert.ToDouble(departmentSummary.Price));
                        rowIndex++;
                    }

                    //Add Summary
                    this.CreateCell(worksheet, rowIndex, 0, "Total:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    CreateFormulaCell(worksheet, rowIndex, 1, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, cellFormat: "#,###").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    CreateFormulaCell(worksheet, rowIndex, 2, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 3, rowIndex, 3), CellReferenceMode.R1C1, cellFormat: "#,###").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    CreateFormulaCell(worksheet, rowIndex, 3, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 4, rowIndex, 4), CellReferenceMode.R1C1, cellFormat: "#,###.00").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    CreateFormulaCell(worksheet, rowIndex, 4, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 5, rowIndex, 5), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex++;

                    // Add Averages
                    this.CreateCell(worksheet, rowIndex, 0, "Average:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    CreateFormulaCell(worksheet, rowIndex, 1, "=AVERAGE(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 2, rowIndex - 1, 2), CellReferenceMode.R1C1, cellFormat: "#,###").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    CreateFormulaCell(worksheet, rowIndex, 2, "=AVERAGE(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 3, rowIndex - 1, 3), CellReferenceMode.R1C1, cellFormat: "#,###").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    CreateFormulaCell(worksheet, rowIndex, 3, "=AVERAGE(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 4, rowIndex - 1, 4), CellReferenceMode.R1C1, cellFormat: "#,###.00").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    CreateFormulaCell(worksheet, rowIndex, 3, "=AVERAGE(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 5, rowIndex - 1, 5), CellReferenceMode.R1C1, cellFormat: "#,###.00").CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex++;

                    var orderStats = StatInfo.Create(orderCounts, "Orders");
                    var partStats = StatInfo.Create(partCounts, "Count");
                    var saStats = StatInfo.Create(saCounts, "in2");
                    var priceStats = StatInfo.Create(priceCounts, "Price");

                    // Add Min
                    this.CreateCell(worksheet, rowIndex, 0, "Min:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 1, orderStats.Min, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 2, partStats.Min, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 3, saStats.Min, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 4, priceStats.Min, false, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex++;

                    // Add Max
                    this.CreateCell(worksheet, rowIndex, 0, "Max:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 1, orderStats.Max, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 2, partStats.Max, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 3, saStats.Max, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 4, priceStats.Max, false, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex++;

                    // Add StdDeviation
                    this.CreateCell(worksheet, rowIndex, 0, "Std Deviation:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 1, orderStats.StdDeviation, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 2, partStats.StdDeviation, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 3, saStats.StdDeviation, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 4, priceStats.StdDeviation, false, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex++;

                    // Add Variance
                    this.CreateCell(worksheet, rowIndex, 0, "Variance:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 1, orderStats.Variance, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 2, partStats.Variance, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 3, saStats.Variance, false).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    this.CreateCell(worksheet, rowIndex, 4, priceStats.Variance, false, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    rowIndex++;

                }
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to create Late Order Summary Report.");
            }
        }

        private List<DepartmentSummary> LoadSummaries(DateTime fromDate, DateTime toDate)
        {
            var orders = new Data.Reports.ProcessPartsReport.OrderProductionDataTable();
            var taOrders = new OrderProductionTableAdapter();

            var shipments = new Data.Reports.ProcessPartsReport.ShippingProductionDataTable();
            var taShipping = new ShippingProductionTableAdapter();

            var reciving = new Data.Reports.ProcessPartsReport.ReceivingProductionDataTable();
            var taReciving = new ReceivingProductionTableAdapter();

            var departments = new List<DepartmentSummary>();

            try
            {
                //Get all orders
                taOrders.Fill(orders, fromDate, toDate);
                taShipping.Fill(shipments, fromDate, toDate);
                taReciving.Fill(reciving, fromDate, toDate);

                DepartmentSummary currentDepartment = null;

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
                        currentDepartment = new DepartmentSummary();
                        currentDepartment.Department = item.Department;
                        currentDepartment.DateTimeStamp = dateTimeStamp;
                        departments.Add(currentDepartment);
                    }

                    //add order and parts summary
                    currentDepartment.Orders += 1;

                    if (!item.IsPartQuantityNull())
                    {
                        try
                        {
                            currentDepartment.PartQuantity += item.PartQuantity;
                        }
                        catch (OverflowException)
                        {
                            _log.Warn("Arithmetic overflow occurred while running report");
                            currentDepartment.PartQuantity = long.MaxValue;
                        }
                    }

                    var weight = item.IsWeightNull() ? 0M : item.Weight;
                    var basePrice = item.IsBasePriceNull() ? 0M : item.BasePrice;
                    var partQuantity = item.IsPartQuantityNull() ? 0 : item.PartQuantity;

                    currentDepartment.Price += OrderPrice.CalculatePrice(basePrice, item.PriceUnit, 0M, partQuantity, weight);

                    if (!item.IsSurfaceAreaNull() && !item.IsPartQuantityNull() && item.PartQuantity > 0)
                    {
                        currentDepartment.TotalSA += (item.SurfaceArea * item.PartQuantity);
                        currentDepartment.PartsWithTotalSA += 1;
                    }
                }

                //Get all new orders created
                using (var taOC = new OrderCreationTableAdapter())
                {
                    const string SALES_DEPT = "Sales (New Orders)";
                    using (Data.Reports.ProcessPartsReport.OrderCreationDataTable dtOC = taOC.GetData(fromDate, toDate))
                    {
                        foreach (Data.Reports.ProcessPartsReport.OrderCreationRow item in dtOC)
                        {
                            long dateTimeStamp = GetDateTimeStamp(item.OrderDate);
                            currentDepartment = departments.Find(find => find.DateTimeStamp == dateTimeStamp && find.Department == SALES_DEPT);

                            //if no dept then create one
                            if (currentDepartment == null)
                            {
                                currentDepartment = new DepartmentSummary();
                                currentDepartment.Department = SALES_DEPT;
                                currentDepartment.DateTimeStamp = dateTimeStamp;
                                departments.Add(currentDepartment);
                            }

                            //add order and parts summary
                            currentDepartment.Orders += 1;

                            if (!item.IsPartQuantityNull())
                            {
                                try
                                {
                                    currentDepartment.PartQuantity += item.PartQuantity;
                                }
                                catch (OverflowException)
                                {
                                    _log.Warn("Arithmetic overflow occurred while running report");
                                    currentDepartment.PartQuantity = long.MaxValue;
                                }
                            }

                            currentDepartment.Price += OrderPrice.CalculatePrice(
                                item.IsBasePriceNull() ? 0M : item.BasePrice,
                                item.PriceUnit,
                                0M,
                                item.IsPartQuantityNull() ? 0 : item.PartQuantity,
                                item.IsWeightNull() ? 0M : item.Weight);

                            if (!item.IsSurfaceAreaNull() && !item.IsPartQuantityNull() && item.PartQuantity > 0)
                            {
                                currentDepartment.TotalSA += (item.SurfaceArea * item.PartQuantity);
                                currentDepartment.PartsWithTotalSA += 1;
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
                        currentDepartment = new DepartmentSummary();
                        currentDepartment.Department = ApplicationSettings.Current.DepartmentShipping;
                        currentDepartment.DateTimeStamp = dateTimeStamp;
                        departments.Add(currentDepartment);
                    }

                    //add order and parts summary
                    currentDepartment.Orders += 1;

                    if (!item.IsPartQuantityNull())
                    {
                        try
                        {
                            currentDepartment.PartQuantity += item.PartQuantity;
                        }
                        catch (OverflowException)
                        {
                            _log.Warn("Arithmetic overflow occurred while running report");
                            currentDepartment.PartQuantity = long.MaxValue;
                        }
                    }

                    currentDepartment.Price += OrderPrice.CalculatePrice(
                        item.IsBasePriceNull() ? 0M : item.BasePrice,
                        item.PriceUnit,
                        0M,
                        item.IsPartQuantityNull() ? 0 : item.PartQuantity,
                        item.IsWeightNull() ? 0M : item.Weight);

                    if (!item.IsSurfaceAreaNull() && !item.IsPartQuantityNull() && item.PartQuantity > 0)
                    {
                        currentDepartment.TotalSA += (item.SurfaceArea * item.PartQuantity);
                        currentDepartment.PartsWithTotalSA += 1;
                    }
                }

                //Get all received orders
                foreach (Data.Reports.ProcessPartsReport.ReceivingProductionRow item in reciving)
                {
                    long dateTimeStamp = GetDateTimeStamp(DateTime.Parse(item.CheckIn));
                    currentDepartment = departments.Find(find => find.DateTimeStamp == dateTimeStamp && find.Department == "Receiving");

                    //if no dept then create one
                    if (currentDepartment == null)
                    {
                        currentDepartment = new DepartmentSummary();
                        currentDepartment.Department = "Receiving";
                        currentDepartment.DateTimeStamp = dateTimeStamp;
                        departments.Add(currentDepartment);
                    }

                    //add order and parts summary
                    currentDepartment.Orders += 1;

                    try
                    {
                        currentDepartment.PartQuantity += item.PartQuantity;
                    }
                    catch (OverflowException)
                    {
                        _log.Warn("Arithmetic overflow occurred while running report");
                        currentDepartment.PartQuantity = long.MaxValue;
                    }


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
                if (reciving != null)
                    reciving.Dispose();
                if (taReciving != null)
                    taReciving.Dispose();
            }
        }

        private long GetDateTimeStamp(DateTime date)
        {
            switch (GroupBy)
            {
                case GroupByDate.Week: //get first monday
                    return new DateTime(date.Year, date.Month, date.Day).StartOfWeek(DayOfWeek.Monday).Ticks;
                case GroupByDate.Month: //get first day of month
                    return new DateTime(date.Year, date.Month, 1).Ticks;
                case GroupByDate.Day: //use just the day
                default:
                    if (date.DayOfWeek == DayOfWeek.Saturday)
                        date = date.AddDays(-1); //Move to Friday
                    else if (date.DayOfWeek == DayOfWeek.Sunday)
                        date = date.AddDays(1); //move to Monay

                    return new DateTime(date.Year, date.Month, date.Day).Ticks;
            }
        }

        #endregion

        #region DepartmentSummary

        private class DepartmentSummary
        {
            public long DateTimeStamp;
            public string Department;
            public decimal Price;
            public int Orders;
            public long PartQuantity;
            public int PartsWithTotalSA;
            public double TotalSA;
        }

        #endregion
    }

    public class PartSummaryReport : Report
    {
        #region Fields

        private readonly PartsDataset.PartRow _part;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Part Summary"; }
        }

        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }

        #endregion

        #region Methods

        public PartSummaryReport(PartsDataset.PartRow part) { this._part = part; }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            SetupReportInfo();
            _section.PageMargins.All = 20;

            Infragistics.Documents.Reports.Report.IContainer standardBarcodeContainer = AddHeader("PART " + this._part.Name, false);

            AddCustomerSection();
            AddPartSection();
            AddProcessSection();
        }

        private void AddCustomerSection()
        {
            CustomersDataset.CustomerRow customer = null;

            try
            {
                //dsCustomer.EnforceConstraints = false;
                using(var ta = new CustomerTableAdapter())
                {
                    var customerTable = new CustomersDataset.CustomerDataTable();
                    {
                        ta.FillBy(customerTable, this._part.CustomerID);

                        if(customerTable.Rows.Count > 0)
                            customer = customerTable[0];
                    }
                }

                IGroup headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Margins = new Infragistics.Documents.Reports.Report.Margins(0, 0, 0, 0);
                headerGroup.Paddings.Horizontal = 5;

                //Add Customer Address
                IText companyText = headerGroup.AddText();
                companyText.Alignment = TextAlignment.Left;
                companyText.Style = DefaultStyles.NormalStyle;
                companyText.Width = new RelativeWidth(50);
                companyText.Borders = DefaultStyles.DefaultBorders;
                companyText.Paddings.All = 5;
                companyText.Margins.Right = 5;
                companyText.Background = DefaultStyles.DefaultBackground;
                companyText.Height = new FixedHeight(95);

                companyText.AddContent("Customer:", DefaultStyles.BlueLargeStyle);
                companyText.AddLineBreak();
                companyText.AddContent("   " + customer.Name, DefaultStyles.BoldStyle);
                companyText.AddLineBreak();

                if (customer.HasBillingAddress)
                {
                    companyText.AddContent("   " + (customer.IsAddress1Null() ? string.Empty : customer.Address1), DefaultStyles.NormalStyle);
                    companyText.AddLineBreak();
                    if (!customer.IsAddress2Null())
                    {
                        companyText.AddContent("   " + customer.Address2, DefaultStyles.NormalStyle);
                        companyText.AddLineBreak();
                    }
                    companyText.AddContent("   " + (customer.IsCityNull() ? string.Empty : customer.City) + ", ", DefaultStyles.NormalStyle);
                    companyText.AddContent((customer.IsStateNull() ? string.Empty : customer.State) + " ", DefaultStyles.NormalStyle);
                    companyText.AddContent(customer.IsZipNull() ? "" : customer.Zip, DefaultStyles.NormalStyle);
                }
                companyText.AddLineBreak();
                companyText.AddLineBreak();
                companyText.AddLineBreak();
                companyText.AddLineBreak();
            }
            catch(Exception exc)
            {
                _log.Fatal(exc, "Error adding customer section to report.");
            }
        }

        private void AddPartSection()
        {
            try
            {
                IGroup headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;
                headerGroup.Margins = new Infragistics.Documents.Reports.Report.Margins(5, 5, 3, 0);
                headerGroup.Borders = DefaultStyles.DefaultBorders;
                headerGroup.Background = DefaultStyles.DefaultBackground;

                Infragistics.Documents.Reports.Report.IContainer orderContainer = headerGroup.AddContainer("order");
                orderContainer.Width = new RelativeWidth(45);
                orderContainer.Paddings.All = 5;
                orderContainer.Margins.Right = 5;

                ITable orderTable = orderContainer.AddTable();
                orderTable.AddRow().AddCells(DefaultStyles.BlueLargeStyle, null, 0, TextAlignment.Left, "Part Information:");
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Part Number:", this._part.Name);
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Material:", (!this._part.IsMaterialNull() ? this._part.Material : "Unknown"));
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Manufacturer:", (!this._part.IsManufacturerIDNull() ? this._part.ManufacturerID : "None"));
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Airframe:", (!this._part.IsAirframeNull() ? this._part.Airframe : "None"));
                orderTable.AddRow().AddCells(DefaultStyles.NormalStyle, null, 0, new TextAlignment(Alignment.Left, Alignment.Middle), "   Notes:", (!this._part.IsNotesNull() ? this._part.Notes : "None"));

                //Add Part Image
                Infragistics.Documents.Reports.Report.IContainer imgContainer = headerGroup.AddContainer("partImage");
                imgContainer.Alignment.Vertical = Alignment.Middle;
                imgContainer.Alignment.Horizontal = Alignment.Center;
                imgContainer.Paddings.All = 5;
                imgContainer.Margins.Horizontal = 5;
                imgContainer.Width = new RelativeWidth(55);

                Image img = null;
                PartsDataset.Part_MediaRow[] partMedias = this._part.GetPart_MediaRows();
                if (partMedias.Length > 0)
                {
                    PartsDataset.MediaRow mr = null;

                    foreach(PartsDataset.Part_MediaRow item in partMedias)
                    {
                        if(item.DefaultMedia)
                            mr = item.MediaRow;
                    }

                    if (mr == null)
                    {
                        _log.Warn("Unable to find default media for part.");
                    }
                    else
                    {
                        img = MediaUtilities.GetImage(mr.MediaID, mr.FileExtension);
                    }
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
                    IImage image = imgContainer.AddImage(new Infragistics.Documents.Reports.Graphics.Image(img));
                    image.Width = new FixedWidth(250);
                    image.Height = new FixedHeight(225);
                    image.KeepRatio = true;
                }
            }
            catch(Exception exc)
            {
                _log.Fatal(exc, "Error adding part section to report.");
            }
        }

        /// <summary>
        ///     Adds the process section describing what processes are going to done on the part.
        /// </summary>
        private void AddProcessSection()
        {
            try
            {
                PartsDataset.PartProcessRow[] processes = this._part.GetPartProcessRows();

                IGroup group = _section.AddGroup();
                group.Layout = Layout.Horizontal;
                group.Margins = new Infragistics.Documents.Reports.Report.Margins(5, 5, 3, 0);
                group.Paddings.All = 5;
                group.Borders = DefaultStyles.DefaultBorders;
                group.Background = DefaultStyles.DefaultBackground;

                ITable table = group.AddTable();

                //Add Header
                ITableRow row = table.AddRow();
                ITableCell cell = row.AddCell();
                cell.AddText("Processes:", DefaultStyles.BlueLargeStyle, TextAlignment.Left);

                var sortedProcesses = processes
                    .OrderBy(partProc => partProc.StepOrder)
                    .ThenBy(partProc => partProc.PartProcessID);

                foreach(var partProc in sortedProcesses)
                {
                    row = table.AddRow();
                    cell = row.AddCell();
                    cell.Width = new RelativeWidth(100);
                    cell.Margins = new HorizontalMargins(20, 2);

                    //Add process name
                    cell.AddText(partProc.StepOrder + " - " + partProc.ProcessRow.ProcessName, DefaultStyles.NormalStyle, TextAlignment.Left);
                }

            }
            catch(Exception exc)
            {
                _log.Fatal(exc, "Error adding process section to COC report.");
            }
        }

        #endregion
    }

    public class CustomerPartVolumeReport : ExcelBaseReport
    {
        #region Fields

        #endregion

        #region Properties

        public override string Title => "Customer Part Volume Report";

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation => PageOrientation.Landscape;

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        private List<IGrouping<DateTime, DbWeek>> WeekHeaders
        {
            get
            {
                // Headers for weeks
                // Combines 'last week of year' with 'first week of next year'
                // if they started on the same date.
                return DbWeek.WeeksInRange(FromDate, ToDate)
                    .GroupBy(w => w.StartDateOfThisWeek)
                    .OrderBy(w => w.Key)
                    .ToList();
            }
        }

        #endregion

        #region Methods

        public CustomerPartVolumeReport()
        {
            FromDate = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            ToDate = DateUtilities.GetLastDayOfMonth(DateTime.Now);
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            //ensure to get correct time with same date
            FromDate = FromDate.StartOfWeek(DayOfWeek.Monday).StartOfDay();
            ToDate = ToDate.EndOfWeek(DayOfWeek.Monday).AddDays(1).EndOfDay();

            CreateCustomerSummaryWorksheet();

            if (FieldUtilities.IsFieldEnabled("Order", "Product Class"))
            {
                CreateProductClassSummaryWorksheet();
            }
        }

        private void CreateCustomerSummaryWorksheet()
        {
            OrdersReport.CustomerPartSummaryDataTable dtSummary = null;
            CustomerPartSummaryTableAdapter taSummary = null;

            try
            {
                var wks = CreateWorksheet("Summary");
                dtSummary = new OrdersReport.CustomerPartSummaryDataTable();
                taSummary = new CustomerPartSummaryTableAdapter();

                taSummary.FillByMonth(dtSummary, FromDate, ToDate);

                if (dtSummary.Count < 1)
                    return;

                var distinctCustomers = dtSummary.Select(s => s.CustomerName).Distinct().ToList();
                var headerRowIndex = AddCompanyHeaderRows(wks, distinctCustomers.Count, "") + 2;
                CreateHeaderCell(wks, headerRowIndex, 0, "Customer");

                var customerRowIndex = new Dictionary<string, int>();
                var weekHeaders = WeekHeaders;

                for (var colIndex = 1; colIndex <= weekHeaders.Count; ++colIndex)
                {
                    var weeksWithSameStartDate = weekHeaders[colIndex - 1];
                    var headerText = weeksWithSameStartDate.First().ToDisplayString();
                    CreateHeaderCell(wks, headerRowIndex, colIndex, headerText);
                }

                // Headers for customers
                for (var row = 0; row < distinctCustomers.Count; row++)
                {
                    var rowIndex = row + 1 + headerRowIndex;
                    wks.Rows[rowIndex].Cells[0].Value = distinctCustomers[row];
                    customerRowIndex[distinctCustomers[row]] = rowIndex;
                }

                var summariesGroupedByColumn = dtSummary
                    .GroupBy(summary => 1 + weekHeaders.IndexOf(wh => wh.Any(dbWeek => dbWeek.Year == summary.Year && dbWeek.WeekInYear == summary.Week)));

                foreach (var column in summariesGroupedByColumn)
                {
                    foreach (var summary in column)
                    {
                        var rowIndex = customerRowIndex[summary.CustomerName];
                        wks.Rows[rowIndex].Cells[column.Key].Value = summary.IsPartQuantityNull()
                            ? 0
                            : summary.PartQuantity;
                    }
                }

                // Add the totals and percentages
                long totalPartQuantity;

                try
                {
                    totalPartQuantity = dtSummary.Sum(s => s.IsPartQuantityNull() ? 0 : s.PartQuantity);
                }
                catch (ArithmeticException exc)
                {
                    _log.Warn(exc, "Overflow occurred");
                    totalPartQuantity = long.MaxValue;
                }

                var totalColIndex = weekHeaders.Count + 1;
                CreateHeaderCell(wks, headerRowIndex, totalColIndex, "Total");
                CreateHeaderCell(wks, headerRowIndex, totalColIndex + 1, "Percent");
                for (var i = headerRowIndex + 1; i < (customerRowIndex.Count + headerRowIndex + 1); i++)
                {
                    var totalCustomerParts = 0;
                    for (var j = 1; j < totalColIndex; j++)
                    {
                        totalCustomerParts += Convert.ToInt32(wks.Rows[i].Cells[j].Value);
                    }

                    wks.Rows[i].Cells[totalColIndex].Value = totalCustomerParts;
                    wks.Rows[i].Cells[totalColIndex + 1].Value = (double)totalCustomerParts / totalPartQuantity;
                    wks.Rows[i].Cells[totalColIndex + 1].CellFormat.FormatString = PERCENT_FORMAT;
                }

                var lastRowIndex = customerRowIndex.Count + headerRowIndex + 1;
                CreateHeaderCell(wks, lastRowIndex, totalColIndex - 1, "Total:");
                CreateFormulaCell(wks, lastRowIndex, totalColIndex, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(headerRowIndex + 1, totalColIndex + 1, lastRowIndex, totalColIndex + 1), CellReferenceMode.R1C1, true, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                CreateFormulaCell(wks, lastRowIndex, totalColIndex + 1, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(headerRowIndex + 1, totalColIndex + 2, lastRowIndex, totalColIndex + 2), CellReferenceMode.R1C1, true, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, PERCENT_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

                // Add the table range
                wks.Tables.Add("A{0}:{1}{2}".FormatWith(headerRowIndex + 1, ExcelColumnIndexToName(totalColIndex + 1), lastRowIndex), true);
                wks.Tables[0].DisplayBandedRows = false;

                // Set column widths
                for (var colIndex = 0; colIndex <= totalColIndex + 1; colIndex++)
                {
                    wks.Columns[colIndex].SetWidth(20, WorksheetColumnWidthUnit.Character);
                }
            }
            finally
            {
                dtSummary?.Dispose();
                taSummary?.Dispose();
            }
        }

        private void CreateProductClassSummaryWorksheet()
        {
            const string defaultProductClass = "N/A";

            OrdersReport.ProductClassPartSummaryDataTable dtSummary = null;
            ProductClassPartSummaryTableAdapter taSummary = null;

            try
            {
                var wks = CreateWorksheet("Product Class");
                dtSummary = new OrdersReport.ProductClassPartSummaryDataTable();
                taSummary = new ProductClassPartSummaryTableAdapter();

                taSummary.FillByMonth(dtSummary, FromDate, ToDate);

                if (dtSummary.Count < 1)
                    return;
                var productClasses = dtSummary.Select(s => s.IsProductClassNameNull() ? defaultProductClass : s.ProductClassName).Distinct().OrderBy(s => s).ToList();
                var headerRowIndex = AddCompanyHeaderRows(wks, productClasses.Count, "") + 2;
                CreateHeaderCell(wks, headerRowIndex, 0, "Product Class");

                var productClassRowIndex = new Dictionary<string, int>();

                // Headers for weeks
                var weekHeaders = WeekHeaders;

                for (var colIndex = 1; colIndex <= weekHeaders.Count; ++colIndex)
                {
                    var weeksWithSameStartDate = weekHeaders[colIndex - 1];
                    var headerText = weeksWithSameStartDate.First().ToDisplayString();
                    CreateHeaderCell(wks, headerRowIndex, colIndex, headerText);
                }

                // Headers for customers
                for (var row = 0; row < productClasses.Count; row++)
                {
                    var rowIndex = row + 1 + headerRowIndex;
                    wks.Rows[rowIndex].Cells[0].Value = productClasses[row];
                    productClassRowIndex[productClasses[row]] = rowIndex;
                }

                var summariesGroupedByColumn = dtSummary
                    .GroupBy(summary => 1 + weekHeaders.IndexOf(wh => wh.Any(dbWeek => dbWeek.Year == summary.Year && dbWeek.WeekInYear == summary.Week)));

                foreach (var column in summariesGroupedByColumn)
                {
                    foreach (var summary in column)
                    {
                        var productClass = summary.IsProductClassNameNull()
                            ? defaultProductClass
                            : summary.ProductClassName;

                        var rowIndex = productClassRowIndex[productClass];

                        wks.Rows[rowIndex].Cells[column.Key].Value = summary.IsPartQuantityNull()
                            ? 0
                            : summary.PartQuantity;
                    }
                }

                // Add the totals and percentages
                long totalPartQuantity;

                try
                {
                    totalPartQuantity = dtSummary.Sum(s => s.IsPartQuantityNull() ? 0 : (long) s.PartQuantity);
                }
                catch (ArithmeticException exc)
                {
                    _log.Warn(exc, "Overflow occurred");
                    totalPartQuantity = long.MaxValue;
                }

                var totalColIndex = weekHeaders.Count + 1;
                CreateHeaderCell(wks, headerRowIndex, totalColIndex, "Total");
                CreateHeaderCell(wks, headerRowIndex, totalColIndex + 1, "Percent");
                for (var i = headerRowIndex + 1; i < (productClassRowIndex.Count + headerRowIndex + 1); i++)
                {
                    var totalProductClassParts = 0;
                    for (var j = 1; j < totalColIndex; j++)
                    {
                        totalProductClassParts += Convert.ToInt32(wks.Rows[i].Cells[j].Value);
                    }

                    wks.Rows[i].Cells[totalColIndex].Value = totalProductClassParts;
                    wks.Rows[i].Cells[totalColIndex + 1].Value = (double) totalProductClassParts / totalPartQuantity;
                    wks.Rows[i].Cells[totalColIndex + 1].CellFormat.FormatString = PERCENT_FORMAT;
                }

                var lastRowIndex = productClassRowIndex.Count + headerRowIndex + 1;
                CreateHeaderCell(wks, lastRowIndex, totalColIndex - 1, "Total:");

                CreateFormulaCell(wks, lastRowIndex, totalColIndex,
                            "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(headerRowIndex + 1, totalColIndex + 1, lastRowIndex,
                                totalColIndex + 1), CellReferenceMode.R1C1, true, HorizontalCellAlignment.Right).CellFormat.Font
                        .Bold =
                    ExcelDefaultableBoolean.True;

                CreateFormulaCell(wks, lastRowIndex, totalColIndex + 1,
                    "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(headerRowIndex + 1, totalColIndex + 2, lastRowIndex,
                        totalColIndex + 2), CellReferenceMode.R1C1, true, HorizontalCellAlignment.Right,
                    CellBorderLineStyle.Thin, PERCENT_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

                // Add the table range
                wks.Tables.Add(
                    "A{0}:{1}{2}".FormatWith(headerRowIndex + 1, ExcelColumnIndexToName(totalColIndex + 1), lastRowIndex),
                    true);
                wks.Tables[0].DisplayBandedRows = false;

                // Set column widths
                for (var colIndex = 0; colIndex <= totalColIndex + 1; colIndex++)
                {
                    wks.Columns[colIndex].SetWidth(20, WorksheetColumnWidthUnit.Character);
                }
            }
            finally
            {
                dtSummary?.Dispose();
                taSummary?.Dispose();
            }
        }

        #endregion
    }

    public class DepartmentPartVolumeReport : ExcelBaseReport
    {
        #region Properties

        public override string Title => "Department Part Volume Report";

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation => PageOrientation.Landscape;

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        private List<IGrouping<DateTime, DbWeek>> WeekHeaders
        {
            get
            {
                // Headers for weeks
                // Combines 'last week of year' with 'first week of next year'
                // if they started on the same date.
                return DbWeek.WeeksInRange(FromDate, ToDate)
                    .GroupBy(w => w.StartDateOfThisWeek)
                    .OrderBy(w => w.Key)
                    .ToList();
            }
        }

        #endregion

        #region Methods

        public DepartmentPartVolumeReport()
        {
            FromDate = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            ToDate = DateUtilities.GetLastDayOfMonth(DateTime.Now);
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            OrdersReport.DepartmentVolumeDataTable dtSummary = null;

            try
            {
                var wks = CreateWorksheet("Summary");

                // Correct dates
                FromDate = FromDate.StartOfWeek(DayOfWeek.Monday).StartOfDay();
                ToDate = ToDate.EndOfWeek(DayOfWeek.Monday).AddDays(1).EndOfDay();

                using (var taSummary = new DepartmentVolumeTableAdapter())
                {
                    dtSummary = taSummary.GetByWeek(FromDate, ToDate);
                }

                if (dtSummary.Count == 0)
                {
                    return;
                }

                var deptInfo = DepartmentInfo.From(dtSummary);
                var weekHeaders = WeekHeaders;

                var rowIndex = AddCompanyHeaderRows(wks, 4, "") + 2;
                var headerRowIndex = rowIndex;
                CreateHeaderCell(wks, rowIndex, 0, "Department");

                for (var colIndex = 1; colIndex <= weekHeaders.Count; ++colIndex)
                {
                    var weeksWithSameStartDate = weekHeaders[colIndex - 1];
                    var headerText = weeksWithSameStartDate.First().ToDisplayString();
                    CreateHeaderCell(wks, headerRowIndex, colIndex, headerText);
                }

                var totalColIndex = weekHeaders.Count + 1;
                var percentColIndex = weekHeaders.Count + 2;

                CreateHeaderCell(wks, rowIndex, totalColIndex, "Total");
                CreateHeaderCell(wks, rowIndex, percentColIndex, "Percent");
                rowIndex++;

                var firstDeptRow = rowIndex;
                var lastDeptRow = rowIndex + deptInfo.Count - 1;

                foreach (var dept in deptInfo.OrderBy(d => d.Name))
                {
                    // Add row
                    wks.Rows[rowIndex].Cells[0].Value = dept.Name;

                    for (int colIndex = 1; colIndex < weekHeaders.Count; colIndex++)
                    {
                        var weekHeader = weekHeaders[colIndex - 1];

                        var deptCount = 0L;
                        foreach (var actualWeek in weekHeader)
                        {
                            if (dept.WeekCount.ContainsKey(actualWeek))
                            {
                                deptCount += dept.WeekCount[actualWeek];
                            }
                        }

                        if (deptCount != 0)
                        {
                            wks.Rows[rowIndex].Cells[colIndex].Value = deptCount;
                        }
                    }

                    // Total
                    CreateFormulaCell(wks, rowIndex, totalColIndex, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(rowIndex + 1, 2, rowIndex + 1, totalColIndex), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.None);

                    // Percentage
                    var deptTotalCellPosition = $"R{rowIndex + 1}C{totalColIndex + 1}";
                    var totalCellPosition = $"R{lastDeptRow + 2 }C{totalColIndex + 1}";
                    var deptPercentCell = CreateFormulaCell(wks, rowIndex, percentColIndex, $"=({deptTotalCellPosition})/({totalCellPosition})", CellReferenceMode.R1C1, false, HorizontalCellAlignment.Right, CellBorderLineStyle.None);
                    deptPercentCell.CellFormat.FormatString = PERCENT_FORMAT;
                    rowIndex++;
                }

                // Totals
                CreateHeaderCell(wks, rowIndex, totalColIndex - 1, "Total:");

                var firstDeptTotalPosition = $"R{firstDeptRow + 1}C{totalColIndex + 1}";
                var lastDeptTotalPosition = $"R{lastDeptRow + 1}C{totalColIndex + 1}";
                var totalCell = CreateFormulaCell(wks, rowIndex, totalColIndex, $"=SUM({firstDeptTotalPosition}:{lastDeptTotalPosition})", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Right);
                totalCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

                var firstDeptPercentPosition = $"R{firstDeptRow + 1}C{percentColIndex + 1}";
                var lastDeptPercentPosition = $"R{lastDeptRow + 1}C{percentColIndex + 1}";
                var percentCell = CreateFormulaCell(wks, rowIndex, percentColIndex, $"=SUM({firstDeptPercentPosition}:{lastDeptPercentPosition})", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Right);
                percentCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

                // Add the table range
                wks.Tables.Add("A{0}:{1}{2}".FormatWith(headerRowIndex + 1, ExcelColumnIndexToName(totalColIndex + 1), rowIndex), true);
                wks.Tables[0].DisplayBandedRows = false;

                // Set column widths
                for (var colIndex = 0; colIndex <= percentColIndex; colIndex++)
                {
                    wks.Columns[colIndex].SetWidth(20, WorksheetColumnWidthUnit.Character);
                }
            }
            finally
            {
                dtSummary?.Dispose();
            }
        }

        #endregion

        #region DepartmentInfo

        /// <summary>
        /// Volume info for a department.
        /// </summary>
        private class DepartmentInfo
        {
            #region Properties

            /// <summary>
            /// Gets the name of the department.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Gets the volume per week.
            /// </summary>
            public IDictionary<DbWeek, long> WeekCount { get; } = new Dictionary<DbWeek, long>();

            #endregion

            #region Methods

            private DepartmentInfo(string name)
            {
                Name = name;
            }

            public static List<DepartmentInfo> From(OrdersReport.DepartmentVolumeDataTable dtSummary)
            {
                if (dtSummary == null)
                {
                    return new List<DepartmentInfo>();
                }

                var departments = new List<DepartmentInfo>();

                foreach (var summary in dtSummary)
                {
                    if (summary.IsPartQuantityNull())
                    {
                        continue;
                    }

                    var dept = departments.FirstOrDefault(d => d.Name == summary.Department);

                    if (dept == null)
                    {
                        dept = new DepartmentInfo(summary.Department);
                        departments.Add(dept);
                    }

                    var week = new DbWeek(summary.Year, summary.Week);

                    if (dept.WeekCount.ContainsKey(week))
                    {
                        dept.WeekCount[week] += summary.PartQuantity;
                    }
                    else
                    {
                        dept.WeekCount[week] = summary.PartQuantity;
                    }
                }

                return departments;
            }

            #endregion
        }

        #endregion
    }

    public class FeeSummaryReport : Report
    {
        #region Fields

        private DateTime _fromDate;
        private DateTime _toDate;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Fees Report"; }
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

        public FeeSummaryReport()
        {
            this._fromDate = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            this._toDate = DateUtilities.GetLastDayOfMonth(DateTime.Now);
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            var dsOrdersReport = new OrdersReport();
            OrderFeeSummary2TableAdapter taFees = new OrderFeeSummary2TableAdapter();
            OrdersReport.OrderFeeSummary2DataTable dt = new OrdersReport.OrderFeeSummary2DataTable();
            decimal totalFees = 0;
            decimal fees = 0;
            string currentOrderFeeType = string.Empty;
            string currentFeeType = string.Empty;

            //ensure to get correct time with same date
            this._fromDate = new DateTime(this._fromDate.Year, this._fromDate.Month, this._fromDate.Day, 0, 0, 0);
            this._toDate = new DateTime(this._toDate.Year, this._toDate.Month, this._toDate.Day, 23, 59, 59);

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

            dt.Constraints.Clear();
            dsOrdersReport.EnforceConstraints = false;
            taFees.FillByDates(dt, this._fromDate, this._toDate);

            AddHeaderRow(summaryTable);

            if (dt.Rows.Count > 0)
            {
                currentOrderFeeType = dt.Rows[0]["OrderFeeTypeID"].ToString();
                foreach (OrdersReport.OrderFeeSummary2Row row in dt)
                {
                    decimal weight = row.IsWeightNull() ? 0M : row.Weight;
                    var basePrice = row.IsBasePriceNull() ? 0M : row.BasePrice;
                    var partQuantity = row.IsPartQuantityNull() ? 0 : row.PartQuantity;
                    var priceUnit = row.IsPriceUnitNull() ? OrderPrice.enumPriceUnit.Each.ToString() : row.PriceUnit;

                    if (currentOrderFeeType.EquivalentTo(row.OrderFeeTypeID))
                    {
                        //Set the current fee type so we'll know what it is when group breaks
                        currentFeeType = row.FeeType;
                        fees += DWOS.Data.OrderPrice.CalculateFees(row.FeeType, row.Charge, basePrice, partQuantity, priceUnit, weight);
                    }
                    else
                    {
                        //Group break
                        AddFeeRow(currentOrderFeeType, currentFeeType, fees, summaryTable);
                        totalFees += fees;

                        //Reset
                        currentOrderFeeType = row.OrderFeeTypeID;
                        currentFeeType = row.FeeType;
                        fees = DWOS.Data.OrderPrice.CalculateFees(row.FeeType, row.Charge, basePrice, partQuantity, priceUnit, weight);
                    }
                }
            }

            //Add the last row and totals
            totalFees += fees;
            AddFeeRow(currentOrderFeeType, currentFeeType, fees, summaryTable);
            AddSummaryRow(totalFees, summaryTable);
        }

        private void AddFeeRow(string orderFeeType, string feeType, decimal fees, ITable table)
        {
            ITableCell cell = null;
            ITableRow row = null;

            try
            {
                //  - Order Row
                row = table.AddRow();

                cell = row.CreateTableCell(50);
                cell.AddText(orderFeeType);

                cell = row.CreateTableCell(50);
                cell.AddText(feeType);

                cell = row.CreateTableCell(50);
                cell.AddText(fees.ToString(OrderPrice.CurrencyFormatString));
            }
            catch(Exception exc)
            {
                _log.Warn(exc, "Unable to determine order details.");
            }
        }

        private void AddHeaderRow(ITable table)
        {
            ITableCell cell = null;
            ITableRow headerRow = table.AddRow();

            cell = headerRow.CreateTableCell(50);
            cell.Background = new Background(Infragistics.Documents.Reports.Graphics.Brushes.AliceBlue);
            cell.AddText("Fee Name", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(50);
            cell.Background = new Background(Infragistics.Documents.Reports.Graphics.Brushes.AliceBlue);
            cell.AddText("Fee Type", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(50);
            cell.Background = new Background(Infragistics.Documents.Reports.Graphics.Brushes.AliceBlue);
            cell.AddText("Price", DefaultStyles.BoldStyle, TextAlignment.Center);
        }

        private void AddSummaryRow(decimal feeSumary, ITable table)
        {
            ITableRow headerRow = table.AddRow();

            ITableCell cell = headerRow.CreateTableCell(50);
            cell.Background = new Background(Infragistics.Documents.Reports.Graphics.Brushes.LightBlue);
            cell.AddText("Total Fees", DefaultStyles.NormalStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(50);
            cell.Background = new Background(Infragistics.Documents.Reports.Graphics.Brushes.LightBlue);
            cell.AddText("", DefaultStyles.NormalStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(50);
            cell.Background = new Background(Infragistics.Documents.Reports.Graphics.Brushes.LightBlue);
            cell.AddText(feeSumary.ToString(OrderPrice.CurrencyFormatString), DefaultStyles.NormalStyle, TextAlignment.Center);
        }

        #endregion
    }

    public class LeadTimeReport : ExcelBaseReport
    {
        #region Fields

        private const string DEFAULT_PRODUCT_CLASS = "N/A";

        #endregion

        #region Properties

        public override string Title => "Lead Time";

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
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

        [Browsable(true)]
        [TypeConverter(typeof(CustomerConverterAll))]
        [DisplayName("Customer")]
        [Category("Report")]
        public int CustomerID { get; set; }

        #endregion

        #region Methods

        public LeadTimeReport()
        {
            FromDate = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            ToDate = DateUtilities.GetLastDayOfMonth(DateTime.Now);
            CustomerID = Settings.Default.LastReportCustomerID;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            //ensure to get correct time with same date
            FromDate = FromDate.StartOfDay();
            ToDate = ToDate.EndOfDay();

            CreateExcelLeadReports();
        }

        protected void CreateExcelLeadReports()
        {
            int rowIndex = 0;

            //Create Summary Worksheet
            var summaryWks = CreateWorksheet("Lead Time Summary");
            rowIndex = AddExcelLeadMainHeaderSummary(summaryWks, "Lead Time Summary");

            var singleCustName = string.Empty;

            if (CustomerID > 0)
            {
                var dataTable = new CustomersDataset.CustomerDataTable();

                using (var taCust = new CustomerTableAdapter())
                    taCust.FillBy(dataTable, CustomerID);

                singleCustName = dataTable.FindByCustomerID(CustomerID).Name;
            }

            var leadList = GetLeadTimeSummaries(FromDate, ToDate);

            if (leadList.Count > 0)
            {
                FillLeadSummary(SummaryType.Customer, summaryWks, leadList, singleCustName, rowIndex);
            }

            // If using product class
            if (FieldUtilities.IsFieldEnabled("Order", "Product Class"))
            {
                var productClassWorksheet = CreateWorksheet("Product Class Summary");
                rowIndex = AddExcelLeadMainHeaderSummary(productClassWorksheet, "Product Class Summary");

                var productClassList = GetLeadTimeProductClassSummaries(FromDate, ToDate);

                if (productClassList.Count > 0)
                {
                    FillLeadSummary(SummaryType.ProductClass, productClassWorksheet, productClassList, string.Empty, rowIndex);
                }
            }

            //if doing an individual customer
            if (CustomerID > 0)
            {
                var custWks = CreateWorksheet("Details By Customer");
                rowIndex = AddExcelLeadMainHeaderDetails(custWks);

                var custList = GetLeadTimeDetails(FromDate, ToDate, CustomerID);

                FillLeadCustomer(custWks, custList, rowIndex);
            }
        }

        private static List<LeadTimeSummary> GetLeadTimeSummaries(DateTime fromDate, DateTime toDate)
        {
            using (var taOrderLT = new OrderLeadTimeTableAdapter())
            {
                using (var orders = taOrderLT.GetOrderLeadTime(toDate, fromDate))
                {
                    var leadList = new List<LeadTimeSummary>();
                    foreach (var item in orders)
                    {
                        var leadRow = new LeadTimeSummary
                        {
                            PartCount = item.PartCount,
                            OrderCount = item.OrderCount,
                            LeadTime = Convert.ToInt32(Math.Ceiling(item.LeadTime)),
                            GroupName = item.CustomerName
                        };

                        using (var ta = new OrderTableAdapter())
                        {
                            var orderList = ta.GetLeadTimeData(toDate, fromDate, item.CustomerID);

                            var onTimeCount = 0;
                            foreach (var order in orderList)
                            {
                                if (!order.IsCompletedDateNull() && order.CompletedDate.Date <= order.EstShipDate.Date)
                                {
                                    onTimeCount += 1;
                                }
                            }
                            leadRow.OnTimeCount = onTimeCount;
                        }

                        leadList.Add(leadRow);
                    }

                    return leadList;
                }
            }
        }

        private static List<LeadTimeSummary> GetLeadTimeProductClassSummaries(DateTime fromDate, DateTime toDate)
        {
            using (var taLeadTime = new ProductClassLeadTimeTableAdapter())
            {
                using (var productClass = taLeadTime.GetLeadTime(toDate, fromDate))
                {
                    var leadList = new List<LeadTimeSummary>();
                    foreach (var item in productClass)
                    {
                        var leadRow = new LeadTimeSummary
                        {
                            PartCount = item.PartCount,
                            OrderCount = item.OrderCount,
                            LeadTime = Convert.ToInt32(Math.Ceiling(item.AvgLeadTime)),
                            GroupName = item.IsProductClassNameNull() ? DEFAULT_PRODUCT_CLASS : item.ProductClassName,
                            OnTimeCount = item.OnTimeCount
                        };

                        leadList.Add(leadRow);
                    }

                    return leadList;
                }
            }
        }

        private static List<LeadTimeDetail> GetLeadTimeDetails(DateTime fromDate, DateTime toDate, int customerId)
        {
            using (var dsOrderReport = new OrdersReport())
            {
                dsOrderReport.EnforceConstraints = false;

                using (var taOrderSerialNumbers = new Data.Reports.OrdersReportTableAdapters.OrderSerialNumberTableAdapter())
                    taOrderSerialNumbers.FillActive(dsOrderReport.OrderSerialNumber);

                using (var taOrderProductClass = new Data.Reports.OrdersReportTableAdapters.OrderProductClassTableAdapter())
                {
                    taOrderProductClass.Fill(dsOrderReport.OrderProductClass);
                }

                using (var taOrders = new OrderTableAdapter())
                {
                    taOrders.FillLeadTimeData(dsOrderReport.Order, toDate, fromDate, customerId);
                }

                var custList = new List<LeadTimeDetail>();
                foreach (var item in dsOrderReport.Order)
                {
                    var leadRow = new LeadTimeDetail
                    {
                        PartCount = item.IsPartQuantityNull() ? 0 : item.PartQuantity,
                        OrderDate = item.IsOrderDateNull() ? new DateTime() : item.OrderDate,
                        CompletedDate = item.IsCompletedDateNull() ? new DateTime() : item.CompletedDate,
                        WorkOrder = item.OrderID,
                        CustomerName = item.CustomerName,
                        PartName = item.PartName,
                        Priority = item.Priority
                    };

                    if (!item.IsEstShipDateNull())
                        leadRow.EstShipDate = item.EstShipDate;

                    leadRow.SerialNumbers = item.GetOrderSerialNumberRows();

                    var productClassRow = item.GetOrderProductClassRows().FirstOrDefault();

                    leadRow.ProductClass = productClassRow == null || productClassRow.IsProductClassNull()
                        ? DEFAULT_PRODUCT_CLASS
                        : productClassRow.ProductClass;

                    custList.Add(leadRow);
                }
                return custList;
            }
        }

        private void FillLeadCustomer(Worksheet worksheet, List <LeadTimeDetail> custList, int rowIndex)
        {
            rowIndex = AddExcelLeadTimeDetailsHeader(worksheet, ++rowIndex, 0, custList);
            var startIndex = rowIndex;

            foreach(LeadTimeDetail leadRow in custList)
            {
                leadRow.LeadTime = DateUtilities.GetBusinessDays(leadRow.OrderDate, leadRow.CompletedDate);

                if(leadRow.EstShipDate.HasValue)
                    leadRow.DaysLate = Convert.ToInt32(leadRow.CompletedDate.Date.Subtract(leadRow.EstShipDate.Value.Date).TotalDays);

                //if not same day then add 1 more day to include full day of processing once completed
                if(!TimeCompare.IsSameDay(leadRow.OrderDate, leadRow.CompletedDate))
                    leadRow.LeadTime++;
                else
                    leadRow.LeadTime = 1; //else min value should be 1 day

                AddExcelLeadCustomerRow(leadRow, leadRow.LeadTime, worksheet, rowIndex, 0);
                rowIndex++;
            }

            if (custList.Count == 0)
            {
                // Add empty column
                for (var col = 0; col < 10; ++col)
                {
                    worksheet.Rows[rowIndex].Cells[col].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
                }

                rowIndex++;
            }

            worksheet.Tables.Add("A{0}:K{1}".FormatWith(startIndex, rowIndex), true);

            AddTotalCustomerSummaryRow(worksheet, rowIndex, startIndex);
            custList.Clear();
        }

        private void FillLeadSummary(SummaryType summaryType, Worksheet worksheet, List<LeadTimeSummary> leadList, string customerName, int rowIndex)
        {
            int totalLeadTime = 0;
            int totalOrderCount = 0;

            rowIndex = AddExcelLeadTimeSummaryHeader(summaryType, worksheet, rowIndex, 0);
            var startRowIndex = rowIndex;
            foreach(var leadRow in leadList)
            {
                totalOrderCount += leadRow.OrderCount;
                totalLeadTime += leadRow.LeadTime;
                AddExcelLeadRow(worksheet, leadRow, customerName, rowIndex, 0);
                rowIndex++;
            }

            long totalPartCount = 0;

            try
            {
                totalPartCount = leadList.Sum(l => l.PartCount);
            }
            catch (OverflowException)
            {
                _log.Warn("Arithmetic overflow occurred while running report");
                totalPartCount = long.MaxValue;
            }

            AddTotalSummaryRow(leadList, totalOrderCount, totalPartCount, (totalLeadTime / leadList.Count()), worksheet, rowIndex);

            // Create a table of the data
            this.CreateTable(worksheet, startRowIndex, 5, rowIndex, true);
        }

        private int AddExcelLeadMainHeaderSummary(Worksheet worksheet, string title)
        {
            int rowIndex = 0;

            //Create Top Level Header
            var region = CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 5, title);
            region.CellFormat.Alignment = HorizontalCellAlignment.Left;
            region.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            worksheet.Rows[rowIndex].Height = 18 * 20;
            region.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            region.CellFormat.Font.Name = DefaultFontName;
            region.CellFormat.Font.Height = 18 * 20;
            region.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "From Date:  " + this.FromDate.ToShortDateString();
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 5, "Company:    " + ApplicationSettings.Current.CompanyName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "To Date:  " + this.ToDate.ToShortDateString();
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 5, "User:             " + SecurityManager.UserName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 5, "Date:             " + DateTime.Now).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;
            return rowIndex;
        }

        private int AddExcelLeadMainHeaderDetails(Worksheet worksheet)
        {
            int rowIndex = 0;

            //Create Top Level Header
            var region = CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 7, "Lead Time By Customer");
            region.CellFormat.Alignment = HorizontalCellAlignment.Left;
            region.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            worksheet.Rows[rowIndex].Height = 18 * 20;
            region.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            region.CellFormat.Font.Name = DefaultFontName;
            region.CellFormat.Font.Height = 18 * 20;
            region.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);

            rowIndex++;
            var region1 = CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 3, "From Date:  " + this.FromDate);
            region1.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            region1.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            foreach(WorksheetCell cell in region1)
                ApplyCellBorders(cell);
            CreateMergedCell(worksheet, rowIndex, 4, rowIndex, 7, "Company:    " + ApplicationSettings.Current.CompanyName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;
            var region2 = CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 3, "To Date: " + this.ToDate);
            region2.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            region2.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            foreach(WorksheetCell cell in region2)
                ApplyCellBorders(cell);
            CreateMergedCell(worksheet, rowIndex, 4, rowIndex, 7, "User:             " + SecurityManager.UserName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;
            CreateMergedCell(worksheet, rowIndex, 4, rowIndex, 7, "Date:             " + DateTime.Now).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;
            return rowIndex;
        }

        private int AddExcelLeadTimeDetailsHeader(Worksheet worksheet, int rowIndex, int startColumn, List <LeadTimeDetail> leadList)
        {
            const int maxColumnIndex = 10;

            var region = CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, maxColumnIndex, leadList.Count > 0 ? leadList[0]?.CustomerName : "");
            region.CellFormat.Font.Height = 18 * 15;
            worksheet.Rows[rowIndex].Height = 18 * 15;
            rowIndex++;

            CreateHeaderCell(worksheet, rowIndex, startColumn++, "WO", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Priority", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Quantity", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Part", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Order Date", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Est. Ship Date", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Date Completed", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Days Late", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Lead Time", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Serial Number(s)", 30);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Product Class", 25);

            rowIndex++;
            return rowIndex;
        }

        private int AddExcelLeadTimeSummaryHeader(SummaryType summaryType, Worksheet worksheet, int rowIndex, int startColumn)
        {
            var groupHeaderText = summaryType == SummaryType.ProductClass
                ? "Product Class"
                : "Customer";

            CreateHeaderCell(worksheet, rowIndex, startColumn++, groupHeaderText, 40);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Part Quantity", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Orders", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Lead Time", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "On Time", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Over Due", 15);

            rowIndex++;
            return rowIndex;
        }

        #region LeadTimeByCustomer

        private void AddExcelLeadCustomerRow(LeadTimeDetail leadDetail, int leadTime, Worksheet worksheet, int rowIndex, int startColumn)
        {
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[startColumn]);
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = leadDetail.WorkOrder;

            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[startColumn]);
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = leadDetail.Priority;

            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[startColumn]);
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = leadDetail.PartCount;

            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[startColumn]);
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = leadDetail.PartName;

            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[startColumn]);
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = leadDetail.OrderDate.ToShortDateString();

            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[startColumn]);
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = leadDetail.EstShipDate.HasValue ? leadDetail.EstShipDate.Value.ToShortDateString() : "NA";

            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[startColumn]);
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = leadDetail.CompletedDate.ToShortDateString();

            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[startColumn]);
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = leadDetail.DaysLate;

            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[startColumn]);
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = leadTime;

            if (leadDetail.SerialNumbers != null)
            {
                worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Left;
                worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
                ApplyCellBorders(worksheet.Rows[rowIndex].Cells[startColumn]);

                var serialNumbers = leadDetail.SerialNumbers.Select(s => s.IsNumberNull() ? string.Empty : s.Number);
                worksheet.Rows[rowIndex].Cells[startColumn].Value = string.Join(", ", serialNumbers);
            }

            startColumn++;
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[startColumn]);
            worksheet.Rows[rowIndex].Cells[startColumn].Value = leadDetail.ProductClass;

        }

        private void AddTotalCustomerSummaryRow(Worksheet worksheet, int rowIndex, int startRowIndex)
        {
            worksheet.Rows[rowIndex].Cells[0].Value = "Totals: ";
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);

            var totalOrderCell = this.CreateFormulaCell(worksheet, rowIndex, 1, "=COUNT(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 1, rowIndex, 1), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, NUMBER_FORMAT);
            totalOrderCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            totalOrderCell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);
            ApplyCellBorders(totalOrderCell);

            var totalPartCell = this.CreateFormulaCell(worksheet, rowIndex, 2, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 3, rowIndex, 3), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, NUMBER_FORMAT);
            totalPartCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            totalPartCell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);
            ApplyCellBorders(totalPartCell);

            for (int i = 3; i < 7; i++)
            {
                var blankCell = worksheet.Rows[rowIndex].Cells[i];
                blankCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                blankCell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);
                ApplyCellBorders(blankCell);
            }

            var avgLate = this.CreateFormulaCell(worksheet, rowIndex, 7, "=AVERAGE(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 8, rowIndex, 8), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, NUMBER_FORMAT);
            avgLate.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            avgLate.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);
            ApplyCellBorders(avgLate);

            var avgLead = this.CreateFormulaCell(worksheet, rowIndex, 8, "=AVERAGE(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 9, rowIndex, 9), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, NUMBER_FORMAT);
            avgLead.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            avgLead.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);
            ApplyCellBorders(avgLead);

            for (int i = 9; i < 11; i++)
            {
                var blankCell = worksheet.Rows[rowIndex].Cells[i];
                blankCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                blankCell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);
                ApplyCellBorders(blankCell);
            }
        }

        #endregion

        #region LeadTimeBySummary

        private void AddExcelLeadRow(Worksheet worksheet, LeadTimeSummary leadDetail, string customerName, int rowIndex, int startColumn)
        {
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = customerName == leadDetail.GroupName ? new CellFillPattern(new WorkbookColorInfo(Color.LightSkyBlue), null, FillPatternStyle.Solid) : new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[startColumn]);
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = leadDetail.GroupName;

            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = customerName == leadDetail.GroupName ? new CellFillPattern(new WorkbookColorInfo(Color.LightSkyBlue), null, FillPatternStyle.Solid) : new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[startColumn]);
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = leadDetail.PartCount;

            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = customerName == leadDetail.GroupName ? new CellFillPattern(new WorkbookColorInfo(Color.LightSkyBlue), null, FillPatternStyle.Solid) : new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[startColumn]);
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = leadDetail.OrderCount;

            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = customerName == leadDetail.GroupName ? new CellFillPattern(new WorkbookColorInfo(Color.LightSkyBlue), null, FillPatternStyle.Solid) : new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[startColumn]);
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = leadDetail.LeadTime;

            var latePct = (double)leadDetail.OnTimeCount / leadDetail.OrderCount;
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = customerName == leadDetail.GroupName ? new CellFillPattern(new WorkbookColorInfo(Color.LightSkyBlue), null, FillPatternStyle.Solid) : new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.FormatString = PERCENT_FORMAT;
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[startColumn]);
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = latePct;

            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Alignment = HorizontalCellAlignment.Center;
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.Fill = customerName == leadDetail.GroupName ? new CellFillPattern(new WorkbookColorInfo(Color.LightSkyBlue), null, FillPatternStyle.Solid) : new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            worksheet.Rows[rowIndex].Cells[startColumn].CellFormat.FormatString = PERCENT_FORMAT;
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[startColumn]);
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = 1 - latePct;
        }

        private void AddTotalSummaryRow(List<LeadTimeSummary> leadList, int orderCount, long partCount, int leadTime, Worksheet worksheet, int rowIndex)
        {
            worksheet.Rows[rowIndex].Cells[0].Value = "Totals: ";
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);

            worksheet.Rows[rowIndex].Cells[1].Value = "Total Parts: " + partCount;
            worksheet.Rows[rowIndex].Cells[1].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[1].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[1].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[1]);

            worksheet.Rows[rowIndex].Cells[2].Value = "Total Orders: " + orderCount;
            worksheet.Rows[rowIndex].Cells[2].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[2].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[2].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[2]);

            worksheet.Rows[rowIndex].Cells[3].Value = "Avg. Lead Time: " + leadTime;
            worksheet.Rows[rowIndex].Cells[3].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[3].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[3].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[3]);

            double totalOrders = 0;
            leadList.ForEach(lead => totalOrders += lead.OrderCount);

            double totalOnTimeOrders = 0;
            leadList.ForEach(lead => totalOnTimeOrders += lead.OnTimeCount);
            var pctOnTime = totalOnTimeOrders / totalOrders;

            var cell = this.CreateCell(worksheet, rowIndex, 4, pctOnTime, true, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, PERCENT_FORMAT);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);
            ApplyCellBorders(cell);

            cell = this.CreateCell(worksheet, rowIndex, 5, 1-pctOnTime, true, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, PERCENT_FORMAT); // this.CreateFormulaCell(worksheet, rowIndex, 5, "=AVERAGE(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, 6, rowIndex, 6), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center, CellBorderLineStyle.Thin, PERCENT_FORMAT);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);
            ApplyCellBorders(cell);
        }

        #endregion

        #endregion

        #region LeadTimeSummary

        public class LeadTimeSummary
        {
            public long PartCount { get; set; }

            public int OrderCount { get; set; }

            public int LeadTime { get; set; }

            public string GroupName { get; set; }

            public int OnTimeCount { get; set; }
        }
        #endregion

        #region LeadTimeDetail

        public class LeadTimeDetail
        {
            public long PartCount { get; set; }

            public int LeadTime { get; set; }

            public int WorkOrder { get; set; }

            public DateTime OrderDate { get; set; }

            public DateTime CompletedDate { get; set; }

            public string CustomerName { get; set; }

            public string Priority { get; set; }

            public string PartName { get; set; }

            public DateTime? EstShipDate { get; set; }

            public int DaysLate { get; set; }

            public OrdersReport.OrderSerialNumberRow[] SerialNumbers { get; set; }
            public string ProductClass { get; internal set; }
        }

        #endregion

        #region SummaryType

        private enum SummaryType
        {
            Customer,
            ProductClass
        }

        #endregion
    }

    public class RevenueByProgramReport : ExcelBaseReport
    {
        #region Fields

        public enum enumGroupBy
        {
            Week,
            Month
        }

        public enum ProgramType
        {
            ModelAndManufacturer,
            ProductClass
        }

        private int _totalColumns;

        #endregion

        #region Properties

        public override string Title => "Revenue By Program Report";

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation => PageOrientation.Landscape;

        [Browsable(true)]
        [Description("The from date of the report.")]
        [DisplayName("Date From")]
        [Category("Report")]
        public DateTime FromDate { get; set; }

        [Browsable(true)]
        [Description("The to date of the report.")]
        [DisplayName("Date To")]
        [Category("Report")]
        public DateTime ToDate { get; set; }

        [Browsable(true)]
        [Description("Determines how to group the production report orders by.")]
        [DisplayName("Group By")]
        [Category("Report")]
        public enumGroupBy GroupBy { get; set; }

        public ProgramType Program { get; set; }

        #endregion Properties

        #region Methods

        public RevenueByProgramReport()
        {
            this.FromDate = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            this.ToDate = DateUtilities.GetLastDayOfMonth(DateTime.Now);
            GroupBy = enumGroupBy.Month;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            //ensure to get correct time with same date
            this.FromDate = new DateTime(this.FromDate.Year, this.FromDate.Month, this.FromDate.Day, 0, 0, 0);
            this.ToDate = new DateTime(this.ToDate.Year, this.ToDate.Month, this.ToDate.Day, 23, 59, 59);

            CreateReportExcel();
        }

        private void CreateReportExcel()
        {
            var worksheet = CreateWorksheet(this.Title);
            var rowIndex = AddCompanyHeaderRows(worksheet, 4, "") + 2;

            // Add the to/from dates and the grouping
            this.CreateCell(worksheet, rowIndex, 0, "From Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(worksheet, rowIndex, 1, FromDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex++;
            this.CreateCell(worksheet, rowIndex, 0, "To Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(worksheet, rowIndex, 1, ToDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex++;
            this.CreateCell(worksheet, rowIndex, 0, "Group By:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(worksheet, rowIndex, 1, GroupBy, false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex = rowIndex + 2;

            if (Program == ProgramType.ModelAndManufacturer)
            {
                var modelInfo = GetModelManufacturerData(FromDate, ToDate);
                AddModelManufacturerHeaders(worksheet, rowIndex);
                BuildReportContent(worksheet, rowIndex, modelInfo);

                // Set the column widths
                worksheet.Columns[0].SetWidth(25, WorksheetColumnWidthUnit.Character);
                worksheet.Columns[1].SetWidth(30, WorksheetColumnWidthUnit.Character);
                for (var colIndex = 2; colIndex < _totalColumns; colIndex++)
                {
                    worksheet.Columns[colIndex].SetWidth(15, WorksheetColumnWidthUnit.Character);
                }
            }
            else
            {
                var productClassInfo = GetProductClassData(FromDate, ToDate);
                AddProductClassHeaders(worksheet, rowIndex);
                BuildReportContent(worksheet, rowIndex, productClassInfo);

                // Set the column widths
                worksheet.Columns[0].SetWidth(25, WorksheetColumnWidthUnit.Character);
                worksheet.Columns[1].SetWidth(30, WorksheetColumnWidthUnit.Character);
                for (var colIndex = 2; colIndex < _totalColumns; colIndex++)
                {
                    worksheet.Columns[colIndex].SetWidth(15, WorksheetColumnWidthUnit.Character);
                }

                // Add tabs for product classes
                BuildProductClassWorksheets(productClassInfo);
            }
        }

        private void BuildReportContent(Worksheet worksheet, int rowIndex, List<ModelSummary> modelInfos)
        {
            var startRowIndex = rowIndex + 1;

            // group the model infos by manufacturer
            var groupedByCustomer = modelInfos
                .OrderBy(mi => mi.Model)
                .GroupBy(mi => mi.CustomerId)
                .ToList();

            foreach(var customerGroup in groupedByCustomer)
            {
                var groupedByManufacturer = customerGroup.GroupBy(mi => mi.Manufacturer);

                foreach(var group in groupedByManufacturer)
                {
                    var previousModel = string.Empty;

                    foreach(var value in group)
                    {
                        if(previousModel != value.Model)
                            rowIndex++;

                        this.CreateCell(worksheet, rowIndex, 0, value.CustomerName);
                        this.CreateCell(worksheet, rowIndex, 1, value.Manufacturer);
                        this.CreateCell(worksheet, rowIndex, 2, value.Model);

                        var currentCol = 3;
                        var currentDate = this.FromDate;
                        while(currentDate <= this.ToDate)
                        {
                            // Increment the value
                            DateTime nextDate;
                            switch(GroupBy)
                            {
                                case enumGroupBy.Week: //get week of year
                                    nextDate = currentDate.AddDays(7).StartOfWeek(DayOfWeek.Monday);
                                    break;
                                case enumGroupBy.Month: //get first day of month
                                default:
                                    nextDate = currentDate.AddMonths(1);
                                    break;
                            }

                            // get values that fall between current and to dates
                            var infos = group.Where(mi => mi.OrderDate >= currentDate && mi.OrderDate <= nextDate && mi.Model == value.Model && mi.Manufacturer == value.Manufacturer).ToList();
                            var totalCost = infos.Sum(info => info.TotalCost);
                            this.CreateCell(worksheet, rowIndex, currentCol, totalCost, false, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT);
                            currentCol++;
                            currentDate = nextDate;
                        }

                        // Add the total column value
                        this.CreateFormulaCell(worksheet, rowIndex, currentCol, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(rowIndex + 1, 4, rowIndex + 1, currentCol), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT);
                        previousModel = value.Model;
                    }
                }
            }

            if (!groupedByCustomer.Any())
            {
                rowIndex++;
                for (int col = 0; col < this._totalColumns; ++col)
                {
                    this.CreateCell(worksheet, rowIndex, col, string.Empty);
                }
            }

            // Create a table for sorting/filtering
            this.CreateTable(worksheet, startRowIndex, _totalColumns - 1, rowIndex + 1, true);

            // Add a total row
            rowIndex++;
            this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 2, "Grand Total").CellFormat.Alignment = HorizontalCellAlignment.Left;
            for (var colIndex = 3; colIndex < _totalColumns; colIndex++)
            {
                this.CreateFormulaCell(worksheet, rowIndex, colIndex, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 1, colIndex + 1, rowIndex, colIndex + 1), CellReferenceMode.R1C1, true, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT);
            }

        }

        private void BuildReportContent(Worksheet worksheet, int rowIndex, List<ProductClassSummary> modelInfos)
        {
            var startRowIndex = rowIndex + 1;

            var groupedByCustomer = modelInfos
                .GroupBy(mi => mi.CustomerId)
                .ToList();

            foreach(var customerGroup in groupedByCustomer)
            {
                var groupedByProductClass = customerGroup
                    .GroupBy(mi => mi.ProductClass)
                    .OrderBy(g => g.Key);

                foreach(var group in groupedByProductClass)
                {
                    var previousProductClass = string.Empty;

                    foreach(var order in group)
                    {
                        if(previousProductClass != order.ProductClass)
                            rowIndex++;

                        CreateCell(worksheet, rowIndex, 0, order.CustomerName);
                        CreateCell(worksheet, rowIndex, 1, order.ProductClass);

                        var currentCol = 2;
                        var currentDate = FromDate;
                        while(currentDate <= ToDate)
                        {
                            // Increment the value
                            DateTime nextDate;
                            switch(GroupBy)
                            {
                                case enumGroupBy.Week: //get week of year
                                    nextDate = currentDate.AddDays(7).StartOfWeek(DayOfWeek.Monday);
                                    break;
                                case enumGroupBy.Month: //get first day of month
                                default:
                                    nextDate = currentDate.AddMonths(1);
                                    break;
                            }

                            // get values that fall between current and to dates
                            var infos = group.Where(mi => mi.OrderDate >= currentDate && mi.OrderDate <= nextDate).ToList();
                            var totalCost = infos.Sum(info => info.TotalCost);
                            CreateCell(worksheet, rowIndex, currentCol, totalCost, false, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT);
                            currentCol++;
                            currentDate = nextDate;
                        }

                        // Add the total column value
                        CreateFormulaCell(worksheet, rowIndex, currentCol, $"=SUM(R{rowIndex+1}C{3}:R{rowIndex + 1}C{currentCol})", CellReferenceMode.R1C1, false, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT);
                        previousProductClass = order.ProductClass;
                    }
                }
            }

            if (!groupedByCustomer.Any())
            {
                rowIndex++;
                for (var col = 0; col < _totalColumns; ++col)
                {
                    CreateCell(worksheet, rowIndex, col, string.Empty);
                }
            }

            // Create a table for sorting/filtering
            CreateTable(worksheet, startRowIndex, _totalColumns - 1, rowIndex + 1, true);

            // Add a total row
            rowIndex++;
            CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 1, "Grand Total").CellFormat.Alignment = HorizontalCellAlignment.Left;
            for (var colIndex = 2; colIndex < _totalColumns; colIndex++)
            {
                CreateFormulaCell(worksheet, rowIndex, colIndex, $"=SUM(R{startRowIndex + 1}C{colIndex + 1}:R{rowIndex}C{colIndex + 1})", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT);
            }

        }

        private void BuildProductClassWorksheets(IEnumerable<ProductClassSummary> productClassInfo)
        {
            foreach (var productClassGroup in productClassInfo.GroupBy(o => o.ProductClass).OrderBy(g => g.Key))
            {
                var productClass = productClassGroup.Key;

                // Set worksheet name based on product class.
                // The first 31 characters of a product class's name may not be unique
                var worksheetName = FormatWorksheetTitle(productClass);

                if (_workbook.Worksheets.Any(c => c.Name == worksheetName))
                {
                    worksheetName = FormatWorksheetTitle(productClass, " #" + productClassGroup.First().OrderId);
                }

                var wsProductClass = CreateWorksheet(worksheetName);
                var rowIndex = 0;

                // Headers
                CreateMergedHeader(wsProductClass, rowIndex, 0, 0, 3, productClass);
                rowIndex++;
                CreateHeaderCell(wsProductClass, rowIndex, 0, "WO", 10);
                CreateHeaderCell(wsProductClass, rowIndex, 1, "Customer Name", 25);
                CreateHeaderCell(wsProductClass, rowIndex, 2, "Order Date", 15);
                CreateHeaderCell(wsProductClass, rowIndex, 3, "Total Price", 15);

                // Content
                var orders = productClassGroup.Select(o => o).OrderBy(o => o.OrderDate);
                foreach (var order in orders)
                {
                    rowIndex++;
                    CreateCell(wsProductClass, rowIndex, 0, order.OrderId);
                    CreateCell(wsProductClass, rowIndex, 1, order.CustomerName);
                    CreateCell(wsProductClass, rowIndex, 2, order.OrderDate, cellFormat:DATE_FORMAT);
                    CreateCell(wsProductClass, rowIndex, 3, order.TotalCost, cellFormat:MONEY_FORMAT);
                }
            }
        }

        private void AddModelManufacturerHeaders(Worksheet worksheet, int rowIndex)
        {
            var colIndex = 0;

            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Customer");
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Manufacturer");
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Model");

            var currentDate = this.FromDate;
            while (currentDate <= this.ToDate)
            {
                CreateHeaderCell(worksheet, rowIndex, colIndex++, this.GetDateTimeStampDisplayValue(this.GetDateTimeStamp(currentDate)));

                // Increment the value
                switch (GroupBy)
                {
                    case enumGroupBy.Week: //get week of year
                        currentDate = currentDate.AddDays(7).StartOfWeek(DayOfWeek.Monday);
                        break;
                    case enumGroupBy.Month: //get first day of month
                    default:
                        currentDate = currentDate.AddMonths(1);
                        break;
                }
            }

            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Total");
            _totalColumns = colIndex;
        }

        private void AddProductClassHeaders(Worksheet worksheet, int rowIndex)
        {
            var colIndex = 0;

            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Customer");
            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Product Class");

            var currentDate = FromDate;
            while (currentDate <= ToDate)
            {
                CreateHeaderCell(worksheet, rowIndex, colIndex++, GetDateTimeStampDisplayValue(GetDateTimeStamp(currentDate)));

                // Increment the value
                switch (GroupBy)
                {
                    case enumGroupBy.Week: //get week of year
                        currentDate = currentDate.AddDays(7).StartOfWeek(DayOfWeek.Monday);
                        break;
                    case enumGroupBy.Month: //get first day of month
                    default:
                        currentDate = currentDate.AddMonths(1);
                        break;
                }
            }

            CreateHeaderCell(worksheet, rowIndex, colIndex++, "Total");
            _totalColumns = colIndex;
        }

        private static List<ModelSummary> GetModelManufacturerData(DateTime fromDate, DateTime toDate)
        {
            var modelInfos = new List<ModelSummary>();

            // Get the info
            var dtManufacturerAndModelData = new OrdersReport.ManufacturerAndModelDataDataTable();
            dtManufacturerAndModelData.Constraints.Clear();

            using (var taMM = new ManufacturerAndModelDataTableAdapter())
                dtManufacturerAndModelData = taMM.GetManufacturerAndModelInfo(fromDate, toDate);

            foreach (var data in dtManufacturerAndModelData)
            {
                if (data.IsTotalPriceNull())
                {
                    continue;
                }

                //add total price summary
                modelInfos.Add(new ModelSummary
                {
                    CustomerId = data.CustomerID,
                    CustomerName = data.Name,
                    Model = data.IsAirframeNull() ? "Other" : data.Airframe,
                    Manufacturer = data.IsManufacturerIDNull() ? "Other" : data.ManufacturerID,
                    OrderDate = data.CompletedDate,
                    TotalCost = data.TotalPrice
                });
            }

            return modelInfos;
        }

        private static List<ProductClassSummary> GetProductClassData(DateTime fromDate, DateTime toDate)
        {
            var orderSummaries = new List<ProductClassSummary>();

            var dtProductClassData = new OrdersReport.ProductClassInfoDataTable();

            using (var taProductClassInfo = new ProductClassInfoTableAdapter())
            {
                taProductClassInfo.Fill(dtProductClassData, fromDate, toDate);
            }

            foreach (var data in dtProductClassData)
            {
                if (data.IsTotalPriceNull())
                {
                    continue;
                }

                //add total price summary
                orderSummaries.Add(new ProductClassSummary
                {
                    OrderId = data.OrderID,
                    CustomerId = data.CustomerID,
                    CustomerName = data.Name,
                    ProductClass = data.IsProductClassNull() ? "N/A" : data.ProductClass,
                    OrderDate = data.CompletedDate,
                    TotalCost = data.TotalPrice
                });
            }

            return orderSummaries;
        }

        private long GetDateTimeStamp(DateTime date)
        {
            switch (GroupBy)
            {
                case enumGroupBy.Week: //get first monday
                    return new DateTime(date.Year, date.Month, date.Day).StartOfWeek(DayOfWeek.Monday).Ticks;
                case enumGroupBy.Month: //get first day of month
                default:
                    return new DateTime(date.Year, date.Month, 1).Ticks;
            }
        }

        private string GetDateTimeStampDisplayValue(long dateTimeStamp)
        {
            switch (GroupBy)
            {
                case enumGroupBy.Week: //get week of year
                    return new DateTime(dateTimeStamp).Month + "/" + new DateTime(dateTimeStamp).Day;
                case enumGroupBy.Month: //get first day of month
                default:
                    return new DateTime(dateTimeStamp).Month + "/" + new DateTime(dateTimeStamp).Year;
            }
        }

        #endregion Methods

        #region ModelSummary

        private class ModelSummary
        {
            public string Model { get; set; }
            public string Manufacturer { get; set; }
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public decimal TotalCost { get; set; }
            public DateTime OrderDate { get; set; }
        }

        #endregion ModelSummary

        #region ProductClassSummary

        private class ProductClassSummary
        {
            public string ProductClass { get; set; }

            public int CustomerId { get; set; }

            public string CustomerName { get; set; }

            public decimal TotalCost { get; set; }

            public DateTime OrderDate { get; set; }

            public int OrderId { get; internal set; }
        }

        #endregion
    }

    public class SalesByEstShipDateReport : ExcelBaseReport
    {
        #region Fields

        /// <summary>
        /// Dictionary of customer names and the row their table starts on 
        /// in the 'By Customer' worksheet
        /// </summary>
       // private Dictionary<string, int> _customerTableStartRows;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Sales by Est Ship Date"; }
        }

        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }

        #endregion Properties

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            try
            {
                CreateWorkBook();

                var orders = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter().GetOpenOrders();
                var fees = new Data.Reports.OrdersReportTableAdapters.OrderFeesTableAdapter().GetFeesByOpenOrders();
                var feeTypes = new Data.Reports.OrdersReportTableAdapters.OrderFeeTypeTableAdapter().GetData();

                // Get the order info
                var orderInfos = new List<OrderInfo>();

                foreach (OrdersReport.OrderRow item in orders)
                {
                    if (item.IsEstShipDateNull())
                    {
                        continue;
                    }

                    var custRow = new OrderInfo
                    {
                        CustomerName = item.CustomerName,
                        CustomerId = item.IsCustomerIDNull() ? -1 : item.CustomerID,
                        PartQuantity = item.IsPartQuantityNull() ? 0 : item.PartQuantity,
                        PriceUnit = item.PriceUnit,
                        WO = item.OrderID,
                        ReqDate = item.IsRequiredDateNull() ? (DateTime?) null : item.RequiredDate,
                        Priority = item.Priority,
                        PartName = item.PartName,
                        OrderType = (OrderType)item.OrderType,
                        EstShipDate = item.EstShipDate
                    };

                    var orderFees = 0m;
                    decimal weight = item.IsWeightNull() ? 0M : item.Weight;

                    foreach(var fee in fees.Where(of => of.OrderID == item.OrderID))
                    {
                        var feeType = feeTypes.FirstOrDefault(f => f.OrderFeeTypeID == fee.OrderFeeTypeID);
                        orderFees += OrderPrice.CalculateFees(
                            feeType == null ? string.Empty : feeType.FeeType,
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

                    orderInfos.Add(custRow);
                }

                var allDates = GetAllDates(orderInfos);
                var customerSummaries = GroupByCustomer(orderInfos);

                // Create the summary worksheet and add the header
                var wsSummary = CreateWorksheet(Title + " Summary");
                var rowIndex = base.AddCompanyHeaderRows(wsSummary, 4, "Summary") + 2;
                this.AddSummaryData(wsSummary, customerSummaries, rowIndex, allDates);

                foreach(var customer in customerSummaries)
                {
                    // Add the customer data
                    var worksheetName = FormatWorksheetTitle(customer.CustomerName);
                    if (_workbook.Worksheets.Any(c => c.Name == worksheetName))
                    {
                        worksheetName = FormatWorksheetTitle(customer.CustomerName, " #" + customer.CustomerId);
                    }

                    var wsCustomer = this.CreateWorksheet(worksheetName);
                    this.AddCustomerData(wsCustomer, orderInfos.Where(oi => oi.CustomerName == customer.CustomerName).ToList());
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to create Sales by ESD Report.");
            }
        }

        private void AddSummaryData(Worksheet worksheet, List<CustomerSummaryInfo> customerSummaries, int rowIndex, List<Itenso.TimePeriod.Date> allDates)
        {
            try
            {
                /*
                *       Customer -    Late   -   Due Today    -   +1  -   +2  -   +3  -   +4
                *       My customer     $10         $20           $5
                *
                *
                */

                // Add header cells
                this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, allDates.Count, "Summary");
                worksheet.Columns[0].SetWidth(40, WorksheetColumnWidthUnit.Character);

                rowIndex++;

                var columnIndex = 0;
                this.CreateHeaderCell(worksheet, rowIndex, columnIndex++, "Customer");

                //write every date as a column header
                foreach (var date in allDates)
                {
                    this.CreateHeaderCell(worksheet, rowIndex, columnIndex++, date.DateTime.ToShortDateString());
                    worksheet.Columns[columnIndex].SetWidth(25, WorksheetColumnWidthUnit.Character);
                }

                //set name of first date as late instead of real date
                this.CreateHeaderCell(worksheet, rowIndex, 1, "Late");
                worksheet.Columns[1].SetWidth(25, WorksheetColumnWidthUnit.Character);

                // Add summary info for each customer
                var startRowIndex = rowIndex;

                foreach (var customerRow in customerSummaries)
                {
                    rowIndex++;

                    //write customer name
                    this.CreateCell(worksheet, rowIndex, 0, customerRow.CustomerName);

                    foreach(var date in allDates)
                    {
                        var colIndex = allDates.IndexOf(date) + 1; //offset to skip the customer name column
                        var orderSummary = customerRow.OrderValueByEstShipDate.FirstOrDefault(f => f.EstShipDate == date);
                        this.CreateCell(worksheet, rowIndex, colIndex, orderSummary == null ? 0 :  orderSummary.Price, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
                    }
                }

                if (customerSummaries.Count == 0)
                {
                    rowIndex++;

                    for (int allDatesIndex = 0; allDatesIndex < allDates.Count; ++allDatesIndex)
                    {
                        var colIndex = allDatesIndex + 1;
                        this.CreateCell(worksheet, rowIndex, colIndex, string.Empty, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
                    }
                }

                // Add summary totals
                rowIndex++;

                this.CreateCell(worksheet, rowIndex, 0, "Total:", false, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

                foreach (var date in allDates)
                {
                    var colIndex = allDates.IndexOf(date) + 1; //offset to skip the customer name column + 1 for cell
                    this.CreateFormulaCell(worksheet, rowIndex, colIndex, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startRowIndex + 2, colIndex + 1, rowIndex, colIndex + 1), CellReferenceMode.R1C1, false, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                }

                rowIndex = rowIndex + 2;
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to create Open Order Values Summary Report.");
            }
        }

        private void AddCustomerData(Worksheet worksheet, List<OrderInfo> customerList)
        {
            var rowIndex = 0;

            // Add header rows
            this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 7, customerList.First().CustomerName);
            rowIndex++;

            this.CreateHeaderCell(worksheet, rowIndex, 0, "WO");
            this.CreateHeaderCell(worksheet, rowIndex, 1, "Type");
            this.CreateHeaderCell(worksheet, rowIndex, 2, "Date Required");
            this.CreateHeaderCell(worksheet, rowIndex, 3, "Part");
            this.CreateHeaderCell(worksheet, rowIndex, 4, "Qty");
            this.CreateHeaderCell(worksheet, rowIndex, 5, "Priority");
            this.CreateHeaderCell(worksheet, rowIndex, 6, "Unit");
            this.CreateHeaderCell(worksheet, rowIndex, 7, "Price");

            worksheet.Columns[0].SetWidth(26, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[1].SetWidth(11, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[2].SetWidth(16, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[3].SetWidth(20, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[4].SetWidth(10, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[5].SetWidth(18, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[6].SetWidth(14, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[7].SetWidth(20, WorksheetColumnWidthUnit.Character);

            var startRowIndex = rowIndex + 1;
            foreach (var order in customerList)
            {
                rowIndex++;
                this.CreateCell(worksheet, rowIndex, 0, order.WO, false, HorizontalCellAlignment.Center);
                this.CreateCell(worksheet, rowIndex, 1, order.OrderType, false, HorizontalCellAlignment.Center);

                if (order.ReqDate.HasValue)
                {
                    var cellFill = order.ReqDate >= DateTime.Now
                        ? CellFill.NoColor
                        : CellFill.CreateSolidFill(Color.Red);

                    CreateCell(worksheet, rowIndex, 2, order.ReqDate.Value.Date, false, HorizontalCellAlignment.Center,
                            CellBorderLineStyle.Thin, DATE_FORMAT).CellFormat.Fill = cellFill;
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
        }

        private List<Itenso.TimePeriod.Date> GetAllDates(List<OrderInfo> orderInfos)
        {
            var startDate   = new Itenso.TimePeriod.Date(DateTime.Now.AddDays(-1)); //yesterday

            Date endDate;
            if (orderInfos.Count > 0)
            {
                endDate     = new Date(orderInfos.Max(oi => oi.EstShipDate));
            }
            else
            {
                endDate = new Date(DateTime.Now);
            }

            var allDates    = new List<Itenso.TimePeriod.Date>();

            //fill in all business days
            for (var currentDate = startDate; currentDate <= endDate; currentDate = new Date(DateUtilities.AddBusinessDays(currentDate.DateTime, 1)))
                allDates.Add(currentDate);

            return allDates;
        }

        private List<CustomerSummaryInfo> GroupByCustomer(List<OrderInfo> orderInfos)
        {
            var customerSummaries = new List<CustomerSummaryInfo>();
            var lateDate = new Itenso.TimePeriod.Date(DateTime.Now.AddDays(-1)); //yesterday

            foreach(var oi in orderInfos)
            {
                var customSummary = customerSummaries.FirstOrDefault(f => f.CustomerName == oi.CustomerName);

                if(customSummary == null)
                {
                    customSummary = new CustomerSummaryInfo() { CustomerName = oi.CustomerName, CustomerId = oi.CustomerId};
                    customerSummaries.Add(customSummary);
                }

                var date = new Date(oi.EstShipDate);

                //dont track exact date if late, if its late we do not care how late it is
                if(date <= lateDate)
                    date = lateDate;

                var orderSummary = customSummary.OrderValueByEstShipDate.FirstOrDefault(f => f.EstShipDate == date);

                if (orderSummary == null)
                {
                    orderSummary = new CustomerSummaryInfo.OrderSummaryInfo() { EstShipDate = date, Price = 0 };
                    customSummary.OrderValueByEstShipDate.Add(orderSummary);
                }

                orderSummary.Price += oi.Price;
            }

            return customerSummaries;
        }

        #endregion Methods

        #region CustomerRow

        private class OrderInfo
        {
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public int WO { get; set; }
            public DateTime? ReqDate { get; set; }
            public DateTime EstShipDate { get; set; }
            public string Priority { get; set; }
            public int PartQuantity { get; set; }
            public decimal Price { get; set; }
            public string PriceUnit { get; set; }
            public string PartName { get; set; }
            public OrderType OrderType { get; set; }
        }

        private class CustomerSummaryInfo
        {
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public List<OrderSummaryInfo> OrderValueByEstShipDate {get; set;}

            public CustomerSummaryInfo()
            {
                this.OrderValueByEstShipDate = new List<OrderSummaryInfo>();
            }

            public class OrderSummaryInfo
            {
                public Date EstShipDate { get; set; }
                public decimal Price { get; set; }
            }
        }

        #endregion CustomerRow
    }

    public class RevenueByPartReport : ExcelBaseReport
    {
        #region  Properties

        public override string Title => "Revenue By Part Report";

        protected override PageOrientation ReportPageOrientation => PageOrientation.Landscape;

        public DateTime FromDate
        {
            get; set;
        }

        public DateTime ToDate
        {
            get; set;
        }

        #endregion

        #region Methods

        public RevenueByPartReport()
        {
            var now = DateTime.Now;
            FromDate = DateUtilities.GetFirstDayOfMonth(now);
            ToDate = DateUtilities.GetLastDayOfMonth(now);
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            FromDate = FromDate.StartOfDay();
            ToDate = ToDate.EndOfDay();

            var data = RevenueByPartData.GetReportData(FromDate, ToDate);

            if (data != null)
            {
                CreateSummaryWorksheet(data);
                CreateDetailsWorksheet(data);
            }
        }

        private void CreateSummaryWorksheet(RevenueByPartData data)
        {
            var worksheet = CreateWorksheet("Summary");

            // Header
            var rowIndex = AddCompanyHeaderRows(worksheet, 5, string.Empty);
            rowIndex += 2;

            this.CreateCell(worksheet, rowIndex, 0, "From Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(worksheet, rowIndex, 1, FromDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex++;

            this.CreateCell(worksheet, rowIndex, 0, "To Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(worksheet, rowIndex, 1, ToDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex += 2;

            CreateHeaderCell(worksheet, rowIndex, 0, "Customer");
            CreateHeaderCell(worksheet, rowIndex, 1, "Part");
            CreateHeaderCell(worksheet, rowIndex, 2, "Manufacturer");
            CreateHeaderCell(worksheet, rowIndex, 3, "Model");

            var headerColumn = 4;
            var monthsInRange = DateUtilities.MonthsInRange(FromDate, ToDate).ToList();
            var totalColumns = 5 + monthsInRange.Count;

            foreach (var currentDate in monthsInRange)
            {
                var formattedMonth = currentDate.ToString("M/yyyy");
                CreateHeaderCell(worksheet, rowIndex, headerColumn, formattedMonth);
                headerColumn++;
            }

            CreateHeaderCell(worksheet, rowIndex, headerColumn, "Total");
            rowIndex++;

            // Content
            var startRowIndex = rowIndex;
            var orders = (data.Orders ?? Enumerable.Empty<RevenueByPartData.Order>()).ToList();

            foreach (var customerGroup in orders.GroupBy(o => o.CustomerName))
            {
                foreach (var partGroup in customerGroup.GroupBy(o => o.PartId))
                {
                    var firstOrder = partGroup.First();
                    var customerName = firstOrder.CustomerName;
                    var partName = firstOrder.PartName;
                    var manufacturer = firstOrder.Manufacturer;
                    var model = firstOrder.Model;

                    var contentCol = 0;

                    CreateCell(worksheet, rowIndex, contentCol++, customerName);
                    CreateCell(worksheet, rowIndex, contentCol++, partName);
                    CreateCell(worksheet, rowIndex, contentCol++, manufacturer);
                    CreateCell(worksheet, rowIndex, contentCol++, model);

                    foreach (var currentDate in monthsInRange)
                    {
                        var monthTotal = partGroup
                            .Where(o => o.DateCompleted.Month == currentDate.Month && o.DateCompleted.Year == currentDate.Year)
                            .Sum(o => o.TotalPrice);

                        CreateCell(worksheet, rowIndex, contentCol++, monthTotal, false, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT);
                    }

                    var rowTotalFormula = $"=SUM(R{rowIndex + 1}C{5}:R{rowIndex + 1}C{contentCol})";
                    CreateFormulaCell(worksheet, rowIndex, contentCol, rowTotalFormula, CellReferenceMode.R1C1, false, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT);
                    rowIndex++;
                }
            }

            if (!orders.Any())
            {
                rowIndex++;
            }

            // Table for sorting/filtering
            CreateTable(worksheet, startRowIndex, totalColumns -1 , rowIndex, true);

            // Totals
            var grandTotalCell = CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 3, "Grand Total");
            grandTotalCell.CellFormat.Alignment = HorizontalCellAlignment.Left;

            for (var totalCol = 4; totalCol < totalColumns; ++totalCol)
            {
                var monthTotalFormula = $"=SUM(R{startRowIndex + 1}C{totalCol + 1}:R{rowIndex}C{totalCol + 1})";
                CreateFormulaCell(worksheet, rowIndex, totalCol, monthTotalFormula, CellReferenceMode.R1C1, true, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT);
            }

            // Set the column widths
            worksheet.Columns[0].SetWidth(25, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[1].SetWidth(30, WorksheetColumnWidthUnit.Character);
            for (var colIndex = 2; colIndex < totalColumns; colIndex++)
            {
                worksheet.Columns[colIndex].SetWidth(15, WorksheetColumnWidthUnit.Character);
            }
        }

        private void CreateDetailsWorksheet(RevenueByPartData data)
        {
            var worksheet = CreateWorksheet("Details");
            var rowIndex = 0;

            // Header
            CreateHeaderCell(worksheet, rowIndex, 0, "WO", 10);
            CreateHeaderCell(worksheet, rowIndex, 1, "Customer", 20);
            CreateHeaderCell(worksheet, rowIndex, 2, "Part", 20);
            CreateHeaderCell(worksheet, rowIndex, 3, "Manufacturer", 20);
            CreateHeaderCell(worksheet, rowIndex, 4, "Model", 20);
            CreateHeaderCell(worksheet, rowIndex, 5, "Quantity", 16);
            CreateHeaderCell(worksheet, rowIndex, 6, "Date Completed", 22);
            CreateHeaderCell(worksheet, rowIndex, 7, "Fees", 18);
            CreateHeaderCell(worksheet, rowIndex, 8, "Total Price", 18);

            rowIndex++;

            // Content
            foreach (var order in data.Orders.OrderBy(o => o.OrderId))
            {
                CreateCell(worksheet, rowIndex, 0, order.OrderId);
                CreateCell(worksheet, rowIndex, 1, order.CustomerName);
                CreateCell(worksheet, rowIndex, 2, order.PartName);
                CreateCell(worksheet, rowIndex, 3, order.Manufacturer);
                CreateCell(worksheet, rowIndex, 4, order.Model);
                CreateCell(worksheet, rowIndex, 5, order.PartQuantity);
                CreateCell(worksheet, rowIndex, 6, order.DateCompleted.ToShortDateString());
                CreateCell(worksheet, rowIndex, 7, order.Fees, false, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT);
                CreateCell(worksheet, rowIndex, 8, order.TotalPrice, false, HorizontalCellAlignment.Left, CellBorderLineStyle.Thin, MONEY_FORMAT);

                rowIndex++;
            }

            // Table for sorting/filtering
            CreateTable(worksheet, 1, 8, rowIndex, true);
        }

        #endregion
    }
}