using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.QuoteDataSetTableAdapters;
using DWOS.Data.Quote;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Admin;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using NLog;

namespace DWOS.UI
{
    public partial class Quotes: DataEditorBase
    {
        #region Fields

        private const string STATUS_OPEN = "Open";
        private const string STATUS_CLOSED = "Closed";
        private bool _inSavedData;
        private bool _isInactiveDataLoaded;

        #endregion

        #region Properties

        private bool ShowInactive
        {
            get { return ((StateButtonTool)base.toolbarManager.Tools["DisplayInactive"]).Checked; }
            set { ((StateButtonTool)base.toolbarManager.Tools["DisplayInactive"]).Checked = value; }
        }

        #endregion

        #region Methods

        public Quotes()
        {
            this.InitializeComponent();

            tvwTOC.Override.SortComparer = new QuoteNodeSorter();
        }

        private void LoadData()
        {
            this.dsQuotes.EnforceConstraints = false;

            using(var ta = new d_TermsTableAdapter())
                ta.Fill(this.dsQuotes.d_Terms);
            using(var ta = new CustomerTableAdapter())
                ta.Fill(this.dsQuotes.Customer);
            using (var ta = new OrderFeeTypeTableAdapter())
                ta.Fill(this.dsQuotes.OrderFeeType);

            this.taQuote.FillByStatus(this.dsQuotes.Quote, STATUS_OPEN);
            this.taProcess.Fill(this.dsQuotes.Process);
            this.taProcessAlias.Fill(this.dsQuotes.ProcessAlias);

            this.dpQuote.LoadData(this.dsQuotes);
            this.dpQuotePart.LoadData(this.dsQuotes, new Data.Order.PriceUnitPersistence());

            base.AddDataPanel(this.dpQuote);
            base.AddDataPanel(this.dpQuotePart);

            this.dsQuotes.Quote.QuoteRowChanged += this.Quote_QuoteRowChanged;
        }

        

        private void LoadTOC()
        {
            using (new UsingTreeLoad(tvwTOC))
            {
                tvwTOC.Nodes.Clear();

                UltraTreeNode rootNode = new QuotesRootNode(this);
                tvwTOC.Nodes.Add(rootNode);
                rootNode.Expanded = true;

                var showAll = ((StateButtonTool)toolbarManager.Tools["DisplayInActive"]).Checked;

                foreach (var cr in dsQuotes.Quote.Where(q => q.IsValidState()))
                {
                    rootNode.Nodes.Add(new QuoteNode(cr, this) { Visible = showAll || cr.Status == STATUS_OPEN });
                }

                //Do this so there is a node selected which will allow closing dialog when 'Ok' button is clicked
                rootNode.Select();
            }
        }

        protected override void ReloadTOC()
        {
            this.LoadTOC();
        }

        private void LoadCommands()
        {
            if(SecurityManager.Current.IsInRole("Quote.Edit"))
            {
                base.Commands.AddCommand("AddQuote", new AddNodeCommand<UltraTreeNode>(toolbarManager.Tools["AddQuote"], this, tvwTOC){AddNode = this.AddQuote});
                base.Commands.AddCommand("AddPart", new AddNodeCommand<QuoteNode>(toolbarManager.Tools["AddPart"], this, tvwTOC){AddNode = this.AddPart});
                base.Commands.AddCommand("Delete", new DeleteCommand(toolbarManager.Tools["Delete"], tvwTOC, this));
                base.Commands.AddCommand("Close", new DeactivateNodeCommand(toolbarManager.Tools["Close"], tvwTOC));
                base.Commands.AddCommand("Copy", new CopyPasteCommand(toolbarManager.Tools["Copy"], tvwTOC));
                base.Commands.AddCommand("QuoteLogReport", new QuoteLogReportCommand(toolbarManager.Tools["QuoteLogReport"]));
                base.Commands.AddCommand("ImportPart", new PartImportCommand(toolbarManager.Tools["ImportPart"], this.tvwTOC, this));
                
                var displayInactive = base.Commands.AddCommand("DisplayInActive", new DisplayInactiveCommand((StateButtonTool)toolbarManager.Tools["DisplayInActive"], tvwTOC)) as DisplayInactiveCommand;
                displayInactive.BeforeClick += ToggleActiveStatus;
            }

            base.Commands.AddCommand("Search", new QuoteSearchCommand(toolbarManager.Tools["AdvancedSearch"], tvwTOC, this));
            base.Commands.AddCommand("Print", new PrintNodeCommand(toolbarManager.Tools["Print"], tvwTOC));
            base.Commands.AddCommand("VideoTutorial", new VideoCommand(toolbarManager.Tools["VideoTutorial"]) { Url = VideoLinks.QuotingTutorial });
        }

        protected override bool SaveData()
        {
            try
            {
                this._inSavedData = true;

                base.EndAllEdits();
                this.taManager.UpdateAll(this.dsQuotes);

                return true;
            }
            catch(Exception exc)
            {
                _log.Warn("DataSet Errors: " + dsQuotes.GetDataErrors());
                ErrorMessageBox.ShowDialog("Error saving data.", exc);
                return false;
            }
            finally
            {
                this._inSavedData = false;
            }
        }

        private void AddPart(QuoteNode rn)
        {
            //create new data source
            var cr = this.dpQuotePart.AddRow(rn.QuoteID);

            //create new ui nodes
            var cn = new QuotePartNode(cr);
            rn.Nodes.Add(cn);
            cn.Select();
        }

        private void AddPart(QuoteNode rn, int partId)
        {
            //create new data source
            var cr = this.dpQuotePart.AddRow(rn.QuoteID, partId);

            //create new ui nodes
            var cn = new QuotePartNode(cr);
            rn.Nodes.Add(cn);
            cn.Select();
        }

        private void AddQuote(UltraTreeNode node)
        {
            if (IsValidControls())
            {
                // Disabling, then enabling validators prevents an issue where
                // new quotes have empty fields.

                _validators.Enabled = false;

                // Create new quote
                var cr = dsQuotes.Quote.NewQuoteRow();
                cr.CreatedDate = DateTime.Now;
                cr.UserId = SecurityManager.Current.CurrentUser.UserID;
                cr.Status = STATUS_OPEN;
                cr.ExpirationDate = DateTime.Now.AddDays(ApplicationSettings.Current.QuoteExpirationDays).Date;
                cr.CustomerID = -1;
                cr.ContactID = -1;
                cr.TermsID = dsQuotes.d_Terms.Count > 0 ? dsQuotes.d_Terms[0].TermsID : -1;

                dsQuotes.Quote.AddQuoteRow(cr);

                //create new ui nodes
                var cn = new QuoteNode(cr, this);
                tvwTOC.Nodes[0].Nodes.Add(cn);
                cn.Select();
                _validators.Enabled = true;
            }
        }

        protected override void LoadNode(UltraTreeNode node)
        {
            if(node is QuoteNode quote)
            {
                //Lazy load children Data and Nodes
                quote.LoadChildrenNodes();

                if (!quote.UsageCount.HasValue)
                {
                    quote.UsageCount = quote.DataRow.RowState == DataRowState.Added
                        ? 0
                        : taQuote.GetUsageCount(quote.DataRow.QuoteID).GetValueOrDefault();
                }

                this.dpQuote.MoveToRecord(quote.QuoteID);
                DisplayPanel(this.dpQuote);
            }
            else if(node is QuotePartNode qp)
            {
                if (!qp.UsageCount.HasValue)
                {
                    qp.UsageCount = qp.DataRow.RowState == DataRowState.Added
                        ? 0
                        : taQuotePart.GetUsageCount(qp.DataRow.QuotePartID).GetValueOrDefault();
                }

                qp.LoadData(this);

                this.dpQuotePart.MoveToRecord((object)qp.ID);
                DisplayPanel(this.dpQuotePart);
            }
            else
                DisplayPanel(null);
        }

        protected override void SaveSelectedNode()
        {
            if(tvwTOC.SelectedNodes.Count > 0)
            {
                Properties.Settings.Default.LastSelectedCustomer = tvwTOC.SelectedNodes[0].Key;
            }
        }

        protected override void OnDispose()
        {
            try
            {
                if (this.dsQuotes != null)
                {
                    this.dsQuotes.Quote.QuoteRowChanged -= this.Quote_QuoteRowChanged;
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error on Quote Manager form close.";
                _log.Error(exc, errorMsg);
            }

            base.OnDispose();
        }

        /// <summary>
        ///   Loads all of this nodes children to ensure all data is loaded properly for them.
        /// </summary>
        /// <param name="node"> The node. </param>
        private void PreLoadNodeChildren(UltraTreeNode node)
        {
            try
            {
                foreach (var pn in node.Nodes.OfType<QuotePartNode>())
                {
                    pn.LoadData(this);
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading node children");
            }
        }

        private void GoToQuote(QuoteDataSet.QuoteSearchRow quoteSearchRow)
        {
            if (!quoteSearchRow.IsClosedDateNull())
            {
                this.ShowInactive = true;
            }

            var quote = dsQuotes.Quote.FindByQuoteID(quoteSearchRow.QuoteID);

            if(quote == null)
            {
                taQuote.FillBy(dsQuotes.Quote, quoteSearchRow.QuoteID);
                quote = dsQuotes.Quote.FindByQuoteID(quoteSearchRow.QuoteID);
            }

            if(quote != null)
            {
                var quoteNode = tvwTOC.Nodes.FindNode <QuoteNode>(qn => qn.QuoteID == quoteSearchRow.QuoteID);
                if(quoteNode == null)
                {
                    quoteNode = new QuoteNode(quote, this);
                    tvwTOC.Nodes[0].Nodes.Add(quoteNode);
                }

                quoteNode.Select();
            }
        }

        
        #endregion

        #region Events

        protected override void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if(IsValidControls())
                {
                    if(this.SaveData())
                    {
                        this.SaveSelectedNode();
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error closing form.", exc);
                _log.Fatal(exc, "Error closing form.");
            }
        }

        private void Quotes_Load(object sender, EventArgs e)
        {
            _loadingData = true; //prevent TOC from loading

            try
            {
                this.LoadCommands();
                this.LoadData();
                this.LoadTOC();
                base.LoadValidators();

                //select first node customer node
                base.RestoreLastSelectedNode(Properties.Settings.Default.LastSelectedCustomer);
            }
            catch(Exception exc)
            {
                splitContainer1.Enabled = false;
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
            }
            finally
            {
                _loadingData = false;
            }
        }

        private void Quote_QuoteRowChanged(object sender, QuoteDataSet.QuoteRowChangeEvent e)
        {
            try
            {
                if (_inSavedData)
                {
                    return;
                }

                if (e.Action == DataRowAction.Change)
                {
                    var n = tvwTOC.Nodes[0].FindNode<QuoteNode>(q => q.QuoteID == e.Row.QuoteID);

                    if(n != null)
                        n.UpdateNodeUI();
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on quote change.");
            }
        }

        private void ToggleActiveStatus(object sender, EventArgs e)
        {
            try
            {
                if (ShowInactive && !_isInactiveDataLoaded)
                {
                    _loadingData = true;
                    LoadNode(null);
                    this.taQuote.FillByStatus(dsQuotes.Quote, STATUS_CLOSED);
                    _isInactiveDataLoaded = true;
                    _loadingData = false;
                }

                ReloadTOC();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error toggling display inactive quotes.");
            }
        }


        #endregion

        #region Nodes

        #region Nested type: QuoteNode

        private class QuoteNode: DataNode<QuoteDataSet.QuoteRow>, IActive, IReportNode
        {
            #region Fields

            private Quotes _quotesManager;

            #endregion

            #region Properties

            private const string KEY_PREFIX = "QUOTE";

            public bool IsDataLoaded { get; set; }

            public int QuoteID => DataRow.QuoteID;

            public override bool HasChanges
            {
                get
                {
                    this.LoadChildrenNodes();

                    return DataRow != null
                        && (DataRow.RowState != DataRowState.Unchanged || DataRow.GetQuotePartRows().Any(qpr => qpr.RowState != DataRowState.Unchanged));
                }
            }
            
            public bool IsActiveData
            {
                get => DataRow.IsValidState() && DataRow.Status == STATUS_OPEN;
                set
                {
                    if (DataRow != null)
                    {
                        if (value)
                        {
                            DataRow.Status = STATUS_OPEN;
                            DataRow.SetClosedDateNull();
                        }
                        else
                        {
                            DataRow.Status = STATUS_CLOSED;
                            DataRow.ClosedDate = DateTime.Now;
                        }
                    }

                    this.UpdateNodeUI();
                }
            }

            public int? UsageCount
            {
                get;
                set;
            }

            public override bool CanDelete
            {
                get
                {
                    return UsageCount.HasValue && UsageCount.Value <= 0;
                }
            }

            #endregion

            #region Methods

            public QuoteNode(QuoteDataSet.QuoteRow cr, Quotes quotesManager)
                : base(cr, cr.QuoteID.ToString(), KEY_PREFIX, cr.QuoteID.ToString())
            {
                this._quotesManager = quotesManager;
                //update UI
                LeftImages.Add(Properties.Resources.Quote_16);
                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                if (DataRow != null && DataRow.IsValidState())
                {
                    Text = QuoteUtilities.GetDisplayString(
                        DataRow,
                        ApplicationSettings.Current.OrderItemFormat);

                    if (IsActiveData)
                    {
                        Override.NodeAppearance.ResetForeColor();
                    }
                    else
                    {
                        Override.NodeAppearance.ForeColor = Color.Red;
                    }
                }
                else
                {
                    Text = string.Empty;
                }
            }

            internal void LoadChildrenNodes()
            {
                try
                {
                    if (DataRow.RowState == DataRowState.Added || IsDataLoaded)
                    {
                        IsDataLoaded = true;
                        return;
                    }

                    using(new UsingTimeMe("Loading Nodes for quote " + base.DataRow.QuoteID))
                    {
                        using(new UsingTreeLoad(base.Control))
                        {
                            var quoteID = DataRow.QuoteID;

                            //LOAD QuoteParts
                            this._quotesManager.taQuotePart.FillBy(this._quotesManager.dsQuotes.QuotePart, quoteID);
                            this._quotesManager.taQuotePartFees.FillByQuoteId(this._quotesManager.dsQuotes.QuotePartFees, quoteID);
                            this._quotesManager.taQuotePartArea.FillByQuote(this._quotesManager.dsQuotes.QuotePartArea, quoteID);
                            this._quotesManager.taQuotePartAreaDimension.FillByQuote(this._quotesManager.dsQuotes.QuotePartAreaDimension, quoteID);

                            foreach (var r in DataRow.GetQuotePartRows())
                            {
                                Nodes.Add(new QuotePartNode(r));
                            }

                            this.IsDataLoaded = true;
                        }
                    }
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error loading quote node children.";
                    LogManager.GetCurrentClassLogger().Error(exc, errorMsg);

                    this.IsDataLoaded = true;
                }
            }

            protected override void OnDispose()
            {
                this._quotesManager = null;
                base.OnDispose();
            }

            public override string ClipboardDataFormat => GetType().FullName;

            public override UltraTreeNode PasteData(string format, DataRowProxy proxy)
            {
                if (proxy == null)
                {
                    return null;
                }

                if (DataRow == null)
                {
                    LogManager.GetCurrentClassLogger().Warn("Node was disposed - skipping paste");
                    return null;
                }

                var ds = DataRow.Table.DataSet as QuoteDataSet;

                //add new data row
                var dr = DataNode<DataRow>.AddPastedDataRows(proxy, ds.QuotePart) as QuoteDataSet.QuotePartRow;

                var copiedPart = new QuotePartNode(dr);
                base.Nodes.Add(copiedPart);

                copiedPart.Select();
                return copiedPart;
            }

            public override bool CanPasteData(string format) =>
                format == typeof(QuotePartNode).FullName;

            #endregion

            #region IReportNode Members

            public IReport CreateReport(string reportType)
            {
                //Ensure all child data is loaded (Parts)
                this.LoadChildrenNodes();
                this._quotesManager.PreLoadNodeChildren(this);

                return new QuoteReport(DataRow);
            }

            public string[] ReportTypes() => new[]
            {
                "Quote"
            };

            #endregion
        }

        #endregion

        #region Nested type: QuotePartNode

        private class QuotePartNode: DataNode<QuoteDataSet.QuotePartRow>
        {
            #region Fields

            private const string KEY_PREFIX = "PART";

            #endregion

            #region Properties

            public int? UsageCount { get; set; }

            public override bool CanDelete =>
                UsageCount.HasValue && UsageCount.Value <= 0;

            private bool IsDataLoaded { get; set; }

            #endregion

            #region Methods

            public QuotePartNode(QuoteDataSet.QuotePartRow cr)
                : base(cr, cr.QuotePartID.ToString(), KEY_PREFIX, cr.Name)
            {
                LeftImages.Add(Properties.Resources.Part_16);
                this.UpdateNodeUI();
            }

            public override string ClipboardDataFormat => GetType().FullName;

            public override void UpdateNodeUI()
            {
                Text = base.DataRow.IsNull("Name") ? "" : base.DataRow.Name;
            }

            public void LoadData(Quotes quoteWindow)
            {
                if (IsDataLoaded || !(DataRow is QuoteDataSet.QuotePartRow quotePart))
                {
                    return;
                }

                // Price
                using (var taPrice = new QuotePartPriceTableAdapter() { ClearBeforeFill = false })
                {
                    taPrice.FillByQuotePart(quoteWindow.dsQuotes.QuotePartPrice, quotePart.QuotePartID);
                }

                // Media
                using (var taQPMedia = new QuotePart_MediaTableAdapter() { ClearBeforeFill = false })
                {
                    taQPMedia.FillByQuotePart(quoteWindow.dsQuotes.QuotePart_Media, quotePart.QuotePartID);
                }

                using (var taMedia = new MediaTableAdapter() { ClearBeforeFill = false })
                {
                    taMedia.FillByQuotePart(quoteWindow.dsQuotes.Media, quotePart.QuotePartID);
                }

                using (var taQPLinks = new QuotePart_DocumentLinkTableAdapter() { ClearBeforeFill = false })
                {
                    taQPLinks.FillByQuotePart(quoteWindow.dsQuotes.QuotePart_DocumentLink, quotePart.QuotePartID);
                }

                // Processes
                using (var ta = new QuotePart_ProcessTableAdapter() { ClearBeforeFill = false })
                {
                    ta.FillByQuotePart(quoteWindow.dsQuotes.QuotePart_Process, quotePart.QuotePartID);
                }

                using (var ta = new QuotePartProcessPriceTableAdapter() {  ClearBeforeFill = false })
                {
                    ta.FillByQuotePart(quoteWindow.dsQuotes.QuotePartProcessPrice, quotePart.QuotePartID);
                }

                IsDataLoaded = true;
            }

            #endregion
        }

        #endregion

        #region Nested type: QuotesRootNode

        private class QuotesRootNode: UltraTreeNode, ICopyPasteNode
        {
            #region Properties

            private Quotes _quotes;

            #endregion

            #region Methods

            public QuotesRootNode(Quotes quotes)
                : base("ROOT", "Quotes")
            {
                this._quotes = quotes;
                LeftImages.Add(Properties.Resources.Folder_16);
            }

            public override void Dispose()
            {
                this._quotes = null;
                base.Dispose();
            }

            #endregion

            #region ICopyPasteNode Members

            public UltraTreeNode PasteData(string format, DataRowProxy proxy)
            {
                if (proxy == null)
                {
                    return null;
                }

                if (_quotes == null)
                {
                    LogManager.GetCurrentClassLogger().Warn("Node was disposed - skipping paste");
                    return null;
                }

                var quote = this._quotes.dsQuotes.Quote.FindByQuoteID(Convert.ToInt32(proxy.OriginalPrimaryKey));

                if (quote == null)
                {
                    LogManager.GetCurrentClassLogger().Warn("Can't find selected quote - skipping paste.");
                    return null;
                }

                // Manually copy child elements
                proxy.ChildProxies?.Clear();

                //Create the copied new QuoteSummary
                var newQuote = DataNode<DataRow>.AddPastedDataRows(proxy, this._quotes.dsQuotes.Quote) as QuoteDataSet.QuoteRow;
                newQuote.Status = STATUS_OPEN; // new copy should always be open

                //Create new Quote
                var quoteItems = quote.ItemArray;
                quoteItems[this._quotes.dsQuotes.Quote.QuoteIDColumn.Ordinal] = null;
                quoteItems[this._quotes.dsQuotes.Quote.CreatedDateColumn.Ordinal] = DateTime.Now;
                quoteItems[this._quotes.dsQuotes.Quote.UserIdColumn.Ordinal] = SecurityManager.Current.UserID;
                quoteItems[_quotes.dsQuotes.Quote.StatusColumn.Ordinal] = STATUS_OPEN;

                var newQuoteNode = new QuoteNode(newQuote, _quotes)
                {
                    IsDataLoaded = true
                };

                base.Nodes.Add(newQuoteNode);

                //Copy Parts
                foreach(var origQuotePart in quote.GetQuotePartRows())
                {
                    var quotePartItems = origQuotePart.ItemArray;
                    quotePartItems[this._quotes.dsQuotes.QuotePart.QuotePartIDColumn.Ordinal] = null;
                    quotePartItems[this._quotes.dsQuotes.QuotePart.QuoteIDColumn.Ordinal] = newQuote.QuoteID;

                    var newQuotePart = this._quotes.dsQuotes.QuotePart.Rows.Add(quotePartItems) as QuoteDataSet.QuotePartRow;
                    var newPartNode = new QuotePartNode(newQuotePart);
                    newQuoteNode.Nodes.Add(newPartNode);

                    //Force load of the quote part in the data panel to ensure all related media and processes are loaded
                    this._quotes.dpQuotePart.MoveToRecord(origQuotePart.QuotePartID);

                    //Add Part Media Relations
                    foreach(var origPartMedia in origQuotePart.GetQuotePart_MediaRows())
                        this._quotes.dsQuotes.QuotePart_Media.AddQuotePart_MediaRow(newQuotePart, origPartMedia.MediaRow);

                    //Add Part Process Relations
                    foreach(var origPartProcess in origQuotePart.GetQuotePart_ProcessRows())
                        this._quotes.dsQuotes.QuotePart_Process.AddQuotePart_ProcessRow(newQuotePart, origPartProcess.ProcessRow, origPartProcess.StepOrder, origPartProcess.ProcessAliasRow, origPartProcess.ShowOnQuote);
                }

                return newQuoteNode;
            }

            public bool CanPasteData(string format) =>
                format == typeof(QuoteNode).FullName;

            public string ClipboardDataFormat => null;

            #endregion
        }

        #endregion

        #endregion

        #region SearchCommand

        private class QuoteSearchCommand : TreeNodeCommandBase
        {
            #region Fields

            private Quotes _quoteManager = null;

            #endregion

            #region Properties

            public override bool Enabled =>
                TreeView != null && TreeView.Nodes.Count > 0;

            #endregion

            #region Methods

            public QuoteSearchCommand(ToolBase tool, UltraTree toc, Quotes quoteManager)
                : base(tool)
            {
                base.TreeView = toc;
                _quoteManager = quoteManager;
            }

            public override void OnClick()
            {
                try
                {
                    using (var frm = new QuoteSearch())
                    {
                        if (frm.ShowDialog(_quoteManager) == DialogResult.OK && frm.SelectedQuote != null)
                        {
                            _quoteManager.GoToQuote(frm.SelectedQuote);
                        }
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error running quote search command.");
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: SearchCommand

        private class PartImportCommand : TreeNodeCommandBase
        {
            #region Fields

            private Quotes _quotes;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return _node is QuoteNode; }
            }

            #endregion

            #region Methods

            public PartImportCommand(ToolBase tool, UltraTree toc, Quotes quoteManager)
                : base(tool)
            {
                base.TreeView = toc;
                this._quotes = quoteManager;
            }

            public override void OnClick()
            {
                try
                {
                    if(!(_node is QuoteNode quoteNode))
                    {
                        return;
                    }

                    using (var frm = new PartSearch())
                    {
                        if (frm.ShowDialog(this._quotes) == DialogResult.OK && frm.SelectedPart != null)
                        {
                            _quotes.AddPart(quoteNode, frm.SelectedPart.PartID);
                        }
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error updating visibility of nodes.");
                }
            }

            public override void Dispose()
            {
                this._quotes = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region QuoteNodeSorter

        private class QuoteNodeSorter : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                if (x is QuoteNode && y is QuoteNode)
                {
                    int xID = ((QuoteNode)x).QuoteID;
                    int yID = ((QuoteNode)y).QuoteID;

                    //if both negative (new) then return reverse sort
                    if(xID < 0 && yID < 0)
                        yID.CompareTo(xID);
                    //if only x new the return greater than
                    if(xID < 0)
                        return 1;
                    //if only y new the return less than
                    if(yID < 0)
                        return -1;

                    //else just compare numbers ASC
                    return xID.CompareTo(yID);
                }

                if (x is UltraTreeNode && y is UltraTreeNode)
                {
                    return ((UltraTreeNode)x).Text.CompareTo(((UltraTreeNode)y).Text);
                }

                return 0;
            }

            #endregion
        }

        #endregion
    }
}