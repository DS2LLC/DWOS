using BarcodeLib;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Preferences.Printing;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using NLog;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;
using Color = Infragistics.Documents.Reports.Graphics.Color;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using FontStyle = Infragistics.Documents.Reports.Graphics.FontStyle;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Pen = Infragistics.Documents.Reports.Graphics.Pen;

namespace DWOS.Reports
{
    public abstract class Report : IDisposable, IReport
    {
        #region Fields
        // Remote desktop connection may have trouble with barcodes that have
        // prefixes requiring use of Shift key.
        public const char BARCODE_ORDER_CHECKIN_PREFFIX = '~';          //Used by Order Check In within the 'DWOS Order Check In' system tray application
        public const char BARCODE_ORDER_ACTION_PREFFIX = '`';           //Used by Shipping Manager to add shipment
        public const char BARCODE_ORDER_PROCESS_PREFIX= ';';            //Used by Order Processing to select order
        public const char BARCODE_ORDER_PROCESS_OLD_PREFIX = '!';       //Used in previous Order Processing barcodes
        public const char BARCODE_SHIPPING_PACKAGE_PREFIX = '^';        //Used by Shipping Package Label to select the package

        public const char BARCODE_BATCH_ACTION_PREFFIX = '.';           //Used by Batch Processing to select batch
        public const char BARCODE_BATCH_CHECKIN_PREFFIX = '+';          //Used by Batch Check In

        public const char BARCODE_SALES_ORDER_ACTION_PREFFIX = '_';       //Used by Shipping Manager to add Sales Order shipment

        protected internal static readonly Logger _log = LogManager.GetCurrentClassLogger();
        protected static readonly Random _random = new Random();

        protected Infragistics.Documents.Reports.Report.Report _report = new Infragistics.Documents.Reports.Report.Report();
        protected ISection _section;
        protected ISectionFooter _footer;

        internal static class DefaultStyles
        {
            #region FontSize enum

            public enum FontSize
            {
                Small = 6,
                Normal = 8,
                Medium = 10,
                Large = 12,
                XLarge = 18
            }

            #endregion

            public const string FontFamily = "Verdana";
            public static Style NormalStyle;
            public static Style SmallStyle;
            public static Style NormalUnderlineStyle;
            public static Style RedSubscriptStyle;
            public static Style BlueStyle;
            public static Style BlueBoldStyle;
            public static Style RedStyle;
            public static Style OrangeStyle;
            public static Style BoldStyle;
            public static Style BoldMediumStyle;
            public static Style BlueUnderlineStyle;
            public static Style BlueLargeStyle;
            public static Style BlueXLargeStyle;
            public static Style BlackLargeStyle;
            public static Style BlackMediumStyle;
            public static Style RedXLargeStyle;
            public static Style RedLargeStyle;
            public static Style BlackXLargeStyle;
            public static Style GreenStyle;

            public static Style GreenStrikeOutStyle;

            public static Borders DefaultBorders;
            public static Background DefaultBackground;

            static DefaultStyles()
            {
                SmallStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.Small), Infragistics.Documents.Reports.Graphics.Brushes.Black);
                NormalStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.Normal), Infragistics.Documents.Reports.Graphics.Brushes.Black);
                NormalUnderlineStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.Normal, Infragistics.Documents.Reports.Graphics.FontStyle.Underline), Infragistics.Documents.Reports.Graphics.Brushes.Black);
                RedSubscriptStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.Normal), Infragistics.Documents.Reports.Graphics.Brushes.Red) { FontVariant = FontVariant.Subscript };
                BlueStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.Normal), Infragistics.Documents.Reports.Graphics.Brushes.DarkBlue);
                BlueBoldStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.Normal, FontStyle.Bold), Infragistics.Documents.Reports.Graphics.Brushes.DarkBlue);
                RedStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.Normal), Infragistics.Documents.Reports.Graphics.Brushes.Red);
                OrangeStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.Normal), Infragistics.Documents.Reports.Graphics.Brushes.DarkOrange);
                BoldStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.Normal, Infragistics.Documents.Reports.Graphics.FontStyle.Bold), Infragistics.Documents.Reports.Graphics.Brushes.Black);
                BoldMediumStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.Normal, Infragistics.Documents.Reports.Graphics.FontStyle.Bold), Infragistics.Documents.Reports.Graphics.Brushes.Black);
                BlueUnderlineStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.Small, Infragistics.Documents.Reports.Graphics.FontStyle.Underline), Infragistics.Documents.Reports.Graphics.Brushes.DarkBlue);
                BlueLargeStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.Large), Infragistics.Documents.Reports.Graphics.Brushes.DarkBlue);
                BlueXLargeStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.XLarge), Infragistics.Documents.Reports.Graphics.Brushes.DarkBlue);
                BlackMediumStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.Medium), Infragistics.Documents.Reports.Graphics.Brushes.Black);
                BlackLargeStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.Large), Infragistics.Documents.Reports.Graphics.Brushes.Black);
                BlackXLargeStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.XLarge, Infragistics.Documents.Reports.Graphics.FontStyle.Bold), Infragistics.Documents.Reports.Graphics.Brushes.Black);
                RedXLargeStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.XLarge, Infragistics.Documents.Reports.Graphics.FontStyle.Bold), Infragistics.Documents.Reports.Graphics.Brushes.Red);
                RedLargeStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.Large, Infragistics.Documents.Reports.Graphics.FontStyle.Bold), Infragistics.Documents.Reports.Graphics.Brushes.Red);
                GreenStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, (int)FontSize.Normal), Infragistics.Documents.Reports.Graphics.Brushes.DarkGreen);

                GreenStrikeOutStyle = new Style(new Infragistics.Documents.Reports.Graphics.Font(FontFamily, 8, Infragistics.Documents.Reports.Graphics.FontStyle.Strikeout), Infragistics.Documents.Reports.Graphics.Brushes.DarkGreen);

                DefaultBorders = new Borders(new Pen(Colors.Black, 1), 1);
                DefaultBackground = new Background(new SolidColorBrush(new Color(250, 250, 250)));
            }
        }

        #endregion

        #region Properties

        [Browsable(false)]
        public abstract string Title { get; }

        protected abstract PageOrientation ReportPageOrientation { get; }

        public static ISecurityUserInfo SecurityManager { get; set; }

        [Browsable(false)]
        public Infragistics.Documents.Reports.Report.Report IGReport
        {
            get { return this._report; }
        }

        [Browsable(false)]
        public string CompanyLogoPath
        {
            get { return ApplicationSettings.Current.CompanyLogoImagePath; }
        }

        [Browsable(false)]
        public bool IsWeb { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance has any valid data. Used to see if it is an empty report
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has data; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        public bool? HasData { get; protected set; }

        protected DateTime DateCreated { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets a randomly-generated identifier associated with this report.
        /// </summary>
        /// <remarks>
        /// This value not guaranteed to be unique, but it is used in creating
        /// a unique filename.
        /// </remarks>
        protected virtual int FilenameIdentifier { get; } = _random.Next(0, 99);

        /// <summary>
        /// Gets an identifier to show in the footer for every page.
        /// </summary>
        /// <remarks>
        /// By default, there is no identifier to show. Individual reports can
        /// override this property to include a footer.
        /// </remarks>
        protected virtual string FooterIdentifier
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the amount of space to add to the report footer.
        /// </summary>
        protected virtual float AdditionalFooterSpace =>
            0;

        protected virtual string ReportFileName =>
            Title.Replace("/", "_") + "_" + DateCreated.ToString("MM.dd.yyyy.H.mm.ss") + "_" + FilenameIdentifier + ".pdf";

        #endregion

        #region Methods

        public virtual void DisplayReport(CancellationToken cancellationToken)
        {
            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            try
            {
                string path = FileSystem.GetFolder(FileSystem.enumFolderType.Reports, true);

                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string filename = string.Empty;
                if (!cancellationToken.IsCancellationRequested)
                {
                    filename = PublishReport(path, cancellationToken);
                }

                if (!string.IsNullOrEmpty(filename) && !cancellationToken.IsCancellationRequested)
                {
                    FileLauncher.New()
                        .HandleErrorsWith((exception, errorMsg) => { throw new Exception(errorMsg, exception); })
                        .Launch(filename);
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error running report.";
                _log.Fatal(exc, errorMsg);
            }
        }

        /// <summary>
        /// Displays the report in its specified format.
        /// </summary>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        public string PublishReport(string outputPath) =>
            PublishReport(outputPath, CancellationToken.None);

        /// <summary>
        /// Displays the report in its specified format.
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual string PublishReport(string outputPath, CancellationToken cancellationToken)
        {
            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            CreateReport(cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
            {
                string reportName = System.IO.Path.Combine(outputPath, ReportFileName);

                // Publishing to a stream instead of a file may use less memory.
                // http://www.infragistics.com/community/forums/t/67557.aspx
                using (var fileStream = File.OpenWrite(reportName))
                {
                    this._report.Publish(fileStream, FileFormat.PDF);
                }

                return System.IO.Path.Combine(outputPath, reportName);
            }

            return string.Empty;
        }

        public virtual string GetFileName(string outputPath) =>
            System.IO.Path.Combine(outputPath, ReportFileName);

        public virtual byte[] PublishPDFReportToBinary()
        {
            CreateReport(CancellationToken.None);

            using(var ms = new MemoryStream())
            {
                this._report.Publish(ms, FileFormat.PDF);

                return ms.ToArray();
            }
        }

        public void PublishReport(Stream stream)
        {
            CreateReport(CancellationToken.None);
            this._report.Publish(stream, FileFormat.PDF);
        }

        public void CreateReportInt() { CreateReport(CancellationToken.None); }

        protected static void ApplyCellBorders(WorksheetCell cell)
        {
            cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
            cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
            cell.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
            cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
        }

        protected static void ApplyCellBorders(IWorksheetCellFormat cellFormat)
        {
            cellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
            cellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
            cellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
            cellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
        }

        /// <summary>
        /// Creates the report content.
        /// </summary>
        /// <remarks>
        /// Implementers do not need to use <paramref name="cancellationToken"/>,
        /// but it is recommended that reports that take a long time to run
        /// use the token to handle user cancellation.
        /// </remarks>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        protected abstract void CreateReport(CancellationToken cancellationToken);

        protected void SetupReportInfo()
        {
            this._report = new Infragistics.Documents.Reports.Report.Report();

            this._report.Info.Title = Title;
            this._report.Info.Author = SecurityManager == null ? "NA" : SecurityManager.UserName;
            this._report.Info.Creator = About.ApplicationName;
            this._report.Info.Copyright = DateCreated.Year.ToString();

            this._report.Preferences.Printing.PaperSize = Infragistics.Documents.Reports.Report.Preferences.Printing.PaperSize.Auto;
            this._report.Preferences.Printing.PaperOrientation = PaperOrientation.Auto;
            this._report.Preferences.Printing.FitToMargins = true;
            this._report.Preferences.PDF.Viewer.FitWindow = true;

            SetupDefaultSection();
        }

        protected void SetupDefaultSection()
        {
            this._section = this._report.AddSection();
            this._section.PageSize = PageSizes.Letter;
            this._section.PageMargins = new Margins(20, 20, 20, 10);
            this._section.PagePaddings = new Paddings(0);
            this._section.PageOrientation = ReportPageOrientation;

            _footer = this._section.AddFooter();
            _footer.Height = 15 + AdditionalFooterSpace;

            IText dateText;
            if (string.IsNullOrEmpty(FooterIdentifier))
            {
                dateText = _footer.AddText(5, 0 + AdditionalFooterSpace);
            }
            else
            {
                dateText = _footer.AddText(5, -4 + AdditionalFooterSpace);

                IText identifierText = _footer.AddText(5, 4 + AdditionalFooterSpace);
                identifierText.Alignment = new TextAlignment(Alignment.Left, Alignment.Bottom);
                identifierText.AddContent(FooterIdentifier, DefaultStyles.NormalStyle);
            }

            dateText.Alignment = new TextAlignment(Alignment.Left, Alignment.Bottom);
            dateText.AddContent(DateCreated.ToShortDateString(), DefaultStyles.NormalStyle);

            IText versionNum = _footer.AddText(0, -4 + AdditionalFooterSpace);
            versionNum.Alignment = new TextAlignment(Alignment.Center, Alignment.Top);
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            versionNum.AddContent("Powered By DWOS " + ver, DefaultStyles.NormalStyle);

            IText ds2Text = _footer.AddText(0, 4 + AdditionalFooterSpace);
            ds2Text.Alignment = new TextAlignment(Alignment.Center, Alignment.Bottom);
            ds2Text.AddContent("A Product of Dynamic Software Solutions", DefaultStyles.NormalStyle);

            this._section.PageNumbering = new Infragistics.Documents.Reports.Report.Section.PageNumbering();
            this._section.PageNumbering.Style = DefaultStyles.NormalStyle;
            this._section.PageNumbering.Template = "Page [Page #] of [TotalPages]";
            this._section.PageNumbering.Continue = true;
            this._section.PageNumbering.SkipFirst = false;
            this._section.PageNumbering.Alignment = PageNumberAlignment.Right;
            this._section.PageNumbering.OffsetY = -25.5f;
            this._section.PageNumbering.OffsetX = -5;
        }

        protected Style GetPriorityStyle(string priority)
        {
            if(String.IsNullOrEmpty(priority))
                return DefaultStyles.NormalStyle;

            if(priority == "Expedite" || priority == "First Priority" || priority == "Weekend Expedite")
                return DefaultStyles.RedStyle;
            if(priority == "Rush")
                return DefaultStyles.OrangeStyle;
            if(priority == "Normal")
                return DefaultStyles.NormalStyle;

            return DefaultStyles.NormalStyle;
        }

        /// <summary>
        ///     Adds the header.
        /// </summary>
        /// <param name="titleText"> The title text. </param>
        /// <param name="showNadcapLogo">
        ///     if set to <c>true</c> [show nadcap logo].
        /// </param>
        /// <returns> The container used for the nadcap logo on the second line. </returns>
        protected Infragistics.Documents.Reports.Report.IContainer AddHeader(string titleText, bool showNadcapLogo)
        {
            ApplicationSettings appSettings = ApplicationSettings.Current;

            IGroup titleGroup = this._section.AddGroup();
            titleGroup.Layout = Layout.Horizontal;

            Infragistics.Documents.Reports.Report.IContainer companyLogo = titleGroup.AddContainer("companyLogo");
            Infragistics.Documents.Reports.Report.IContainer titleContainer = titleGroup.AddContainer("title");
            companyLogo.Width = new RelativeWidth(50);
            titleContainer.Width = new RelativeWidth(50);
            //titleContainer.Height		= new FixedHeight(50);

            companyLogo.Alignment.Horizontal = Alignment.Left;
            companyLogo.Alignment.Vertical = Alignment.Top;
            titleContainer.Alignment.Horizontal = Alignment.Right;
            titleContainer.Alignment.Vertical = Alignment.Bottom;

            AddCompanyLogo(companyLogo);

            //Document Title
            IText title = titleContainer.AddText();
            title.Alignment.Horizontal = Alignment.Right;
            title.Alignment.Vertical = Alignment.Bottom;
            title.Style = DefaultStyles.RedXLargeStyle;

            var superSized = new Style(new Font(DefaultStyles.RedXLargeStyle.Font.Name, DefaultStyles.RedXLargeStyle.Font.Size + 6, FontStyle.Bold), DefaultStyles.RedXLargeStyle.Brush);
            title.Margins.All = 5;
            title.Margins.Left = 5;

            foreach(string word in titleText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries))
            {
                //if is a number then add as normal
                if(char.IsDigit(word[0]))
                    title.AddContent(word[0]);
                else //else if is alpha then add first letter supersized
                    title.AddContent(word[0], superSized);

                title.AddContent(word.Substring(1) + " ");
            }

            IGroup headerGroup = this._section.AddGroup();
            headerGroup.Layout = Layout.Horizontal;

            //Second Line
            Infragistics.Documents.Reports.Report.IContainer compAddressCont = headerGroup.AddContainer("company");
            Infragistics.Documents.Reports.Report.IContainer nadcapLogo = headerGroup.AddContainer("logo2");

            nadcapLogo.Width = new RelativeWidth(50);
            nadcapLogo.Alignment.Horizontal = Alignment.Right;
            nadcapLogo.Alignment.Vertical = Alignment.Middle;

            compAddressCont.Width = new RelativeWidth(50);
            compAddressCont.Alignment.Horizontal = Alignment.Right;

            //Add Nadcap Logo
            if(showNadcapLogo && !string.IsNullOrEmpty(ApplicationSettings.Current.AccreditationLogoImagePath))
            {
                IImage image2 = nadcapLogo.AddImage(new Image(ApplicationSettings.Current.AccreditationLogoImagePath));
                image2.Width = new FixedWidth(125);
                image2.Paddings.Horizontal = 15;
            }

            //Add Company Address
            IText companyText = compAddressCont.AddText();
            companyText.Alignment.Horizontal = Alignment.Left;
            companyText.Alignment.Vertical = Alignment.Top;
            companyText.Style = DefaultStyles.NormalStyle;
            //companyText.Margins.Bottom = 5;
            //companyText.Margins.Right = 5;
            companyText.Paddings = new Paddings(15, 5, 5, 10);

            companyText.AddContent(appSettings.CompanyName);
            companyText.AddLineBreak();
            companyText.AddContent(appSettings.CompanyAddress1);
            companyText.AddLineBreak();
            companyText.AddContent(appSettings.CompanyCity + ", ");
            companyText.AddContent(appSettings.CompanyState + " ");
            companyText.AddContent(appSettings.CompanyZip);
            companyText.AddLineBreak();
            companyText.AddContent(appSettings.CompanyPhone + " Phone");
            //companyText.AddLineBreak();
            //companyText.AddContent(appSettings.CompanyFax + " Fax");

            return nadcapLogo;
        }

        /// <summary>
        /// Adds the header.
        /// </summary>
        /// <param name="titleText">The title text.</param>
        /// <param name="workOrderText">The work order text.</param>
        /// <param name="orderNum">The order number.</param>
        /// <param name="accredidationLogo">if set to <c>true</c> [show nadcap logo].</param>
        /// <param name="_order">The _order.</param>
        /// <param name="type">The type.</param>
        /// <returns>The container used for the nadcap logo on the second line.</returns>
        protected Infragistics.Documents.Reports.Report.IContainer AddHeader(string titleText, string workOrderText, int orderNum, bool accredidationLogo, OrdersDataSet.OrderRow _order, ReportType type)
        {
            ApplicationSettings appSettings = ApplicationSettings.Current;

            IGroup titleGroup = this._section.AddGroup();
            titleGroup.Layout = Layout.Horizontal;

            Infragistics.Documents.Reports.Report.IContainer companyLogo = titleGroup.AddContainer("companyLogo");
            Infragistics.Documents.Reports.Report.IContainer titleContainer = titleGroup.AddContainer("title");
            companyLogo.Width = new RelativeWidth(50);
            titleContainer.Width = new RelativeWidth(50);
            //titleContainer.Height		= new FixedHeight(50);

            companyLogo.Alignment.Horizontal = Alignment.Left;
            companyLogo.Alignment.Vertical = Alignment.Top;
            titleContainer.Alignment.Horizontal = Alignment.Right;
            titleContainer.Alignment.Vertical = Alignment.Bottom;

            AddCompanyLogo(companyLogo);

            //Document Title
            IText title = titleContainer.AddText();
            title.Alignment.Horizontal = Alignment.Right;
            title.Alignment.Vertical = Alignment.Bottom;
            title.Style = DefaultStyles.RedXLargeStyle;

            var superSized = new Style(new Font(DefaultStyles.RedXLargeStyle.Font.Name, DefaultStyles.RedXLargeStyle.Font.Size, FontStyle.Bold), DefaultStyles.BlackXLargeStyle.Brush);
            title.Margins.Top = 5;
            title.Margins.Bottom = 0;
            title.Margins.Left = 5;
            var custFont1 = new Font("Calibri", 18);
            var workOrderStyle = new Style(custFont1, DefaultStyles.BlackLargeStyle.Brush);

            title.AddContent(titleText, superSized);
            title.AddLineBreak();
            title.AddContent(workOrderText, workOrderStyle);
            title.AddContent(orderNum.ToString());
            //string theTitle = titleText + workOrderText + orderNum;

            IGroup headerGroup = this._section.AddGroup();
            headerGroup.Layout = Layout.Horizontal;

            //Second Line
            Infragistics.Documents.Reports.Report.IContainer compAddressCont = headerGroup.AddContainer("company");
            Infragistics.Documents.Reports.Report.IContainer nadcapLogo = headerGroup.AddContainer("logo2");

            nadcapLogo.Width = new RelativeWidth(50);
            nadcapLogo.Alignment.Horizontal = Alignment.Right;
            nadcapLogo.Alignment.Vertical = Alignment.Middle;

            compAddressCont.Width = new RelativeWidth(50);
            compAddressCont.Alignment.Horizontal = Alignment.Right;

            //Add accredidation Logo
            if (accredidationLogo && !string.IsNullOrEmpty(ApplicationSettings.Current.AccreditationLogoImagePath))
            {
                IImage image2 = nadcapLogo.AddImage(new Image(ApplicationSettings.Current.AccreditationLogoImagePath));
                image2.Width = new FixedWidth(125);
                image2.Paddings.Horizontal = 15;
            }

            var priorityStyle = new Style(new Font(DefaultStyles.RedXLargeStyle.Font.Name, DefaultStyles.RedXLargeStyle.Font.Size - 1, FontStyle.Bold), DefaultStyles.BlackXLargeStyle.Brush);

            //Add Company Address
            IText companyText = compAddressCont.AddText();
            companyText.Alignment.Horizontal = Alignment.Left;
            companyText.Alignment.Vertical = Alignment.Top;
            companyText.Style = DefaultStyles.NormalStyle;
            companyText.Paddings = new Paddings(5, 5, 5, 5);

            //Add per TTP #677
            companyText.AddContent(appSettings.CompanyName);
            companyText.AddLineBreak();
            companyText.AddContent(appSettings.CompanyAddress1);
            companyText.AddLineBreak();
            companyText.AddContent(appSettings.CompanyCity + ", ");
            companyText.AddContent(appSettings.CompanyState + " ");
            companyText.AddContent(appSettings.CompanyZip);
            companyText.AddLineBreak();
            companyText.AddContent(appSettings.CompanyPhone + " Phone");
            companyText.AddLineBreak();


            if (type == ReportType.WorkOrderTraveler && _order.Priority != "Normal")
            {
                var priorityText = _order.Priority;

                // Workaround for DPS & any other companies that have a
                // "First Priority" priority instead of a "First" priority.
                if (_order.Priority == "First Priority")
                {
                    priorityText = "First";
                }

                companyText.AddContent("***" + "Priority: ", priorityStyle);
                companyText.AddContent(priorityText + "***", priorityStyle);
            }

            return nadcapLogo;
        }

        private void AddCompanyLogo(Infragistics.Documents.Reports.Report.IContainer companyLogo)
        {
            //Company Logo
            if (string.IsNullOrEmpty(CompanyLogoPath))
            {
                // Infragistics expects companyLogo to have something
                companyLogo.AddText();
            }
            else
            {
                Image pic;
                using (var sourceImg = MediaUtilities.GetImage(CompanyLogoPath))
                {
                    pic = new Image(MediaUtilities.PrepareForReport(sourceImg));
                }

                IImage image = companyLogo.AddImage(pic);

                // Resize company logo without cropping or distorting it
                image.KeepRatio = true;

                var originalSize = new System.Drawing.Size(pic.Width, pic.Height);
                var maximumSize = new System.Drawing.Size(185, 80);
                var newSize = MediaUtilities.Resize(originalSize, maximumSize);

                image.Width = new FixedWidth(newSize.Width);
                image.Height = new FixedHeight(newSize.Height);

                // Set paddings/margins for logo
                image.Paddings.Left = 15;
                image.Margins.Top = 10;
            }
        }

        protected System.Drawing.Image CreateOrderBarcode(int orderID, bool large) { return Barcode.DoEncode(TYPE.CODE128, orderID.ToString(), false, 250, large ? 50 : 40); }

        /// <summary>
        ///     Creates the order check in barcode used to checkin an order.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <returns>System.Drawing.Image.</returns>
        protected System.Drawing.Image CreateOrderCheckInBarcode(int orderID) { return Barcode.DoEncode(TYPE.CODE128, BARCODE_ORDER_CHECKIN_PREFFIX + orderID.ToString() + BARCODE_ORDER_CHECKIN_PREFFIX, false, 250, 40); }

        /// <summary>
        ///     Creates the order action barcode used for adding an order to a shipment or adding an order to a batch.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <returns>System.Drawing.Image.</returns>
        protected System.Drawing.Image CreateOrderActionBarcode(int orderID) { return Barcode.DoEncode(TYPE.CODE128, BARCODE_ORDER_ACTION_PREFFIX + orderID.ToString() + BARCODE_ORDER_ACTION_PREFFIX, false, 250, 40); }

        protected System.Drawing.Image CreateBatchCheckInBarcode(int batchId) { return Barcode.DoEncode(TYPE.CODE128, BARCODE_BATCH_CHECKIN_PREFFIX + batchId.ToString() + BARCODE_BATCH_CHECKIN_PREFFIX, false, 250, 40); }

        protected System.Drawing.Image CreateBatchActionBarcode(int batchId) { return Barcode.DoEncode(TYPE.CODE128, BARCODE_BATCH_ACTION_PREFFIX + batchId.ToString() + BARCODE_BATCH_ACTION_PREFFIX, false, 250, 40); }

        protected System.Drawing.Image CreateSalesOrderActionBarcode(int salesOrderId)
        {
            return Barcode.DoEncode(TYPE.CODE128, BARCODE_SALES_ORDER_ACTION_PREFFIX + salesOrderId.ToString() + BARCODE_SALES_ORDER_ACTION_PREFFIX, false, 250, 40);
        }

        public static string GetValue(object o, string defaultValue)
        {
            if(o == null || o == DBNull.Value)
                return defaultValue;
            else
                return o.ToString();
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            this._report = null;
            this._section = null;
        }

        #endregion
    }
}