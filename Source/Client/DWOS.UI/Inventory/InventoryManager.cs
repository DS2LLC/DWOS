using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.QuoteDataSetTableAdapters;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Admin;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using NLog;

namespace DWOS.UI.Inventory
{
    public partial class InventoryManager: DataEditorBase
    {
        #region Fields

        private bool _inSavedData;

        #endregion

        #region Properties

        private bool ShowInactive
        {
            get { return ((StateButtonTool)base.toolbarManager.Tools["DisplayInactive"]).Checked; }
            set { ((StateButtonTool)base.toolbarManager.Tools["DisplayInactive"]).Checked = value; }
        }

        #endregion

        #region Methods

        public InventoryManager()
        {
            this.InitializeComponent();

            tvwTOC.Override.SortComparer = new QuoteNodeSorter();
        }

        private void LoadData()
        {
            this.dsQuotes.EnforceConstraints = false;

            using(var ta = new d_TermsTableAdapter())
            {
                ta.Fill(this.dsQuotes.d_Terms);
            }
            using(var ta = new CustomerTableAdapter())
            {
                ta.Fill(this.dsQuotes.Customer);
            }

            //Load only quote summaries and list of available quote processes, rest is dynamically loaded
            this.taQuoteSummary.FillByStatus(this.dsQuotes.QuoteSummary, "Open");
            this.taQuoteProcess.Fill(this.dsQuotes.QuoteProcess);

            //this.dpQuote.LoadData(this.dsQuotes);
            //this.dpQuotePart.LoadData(this.dsQuotes, this.taQuotePart_Media, this.taMedia);

            //base.AddDataPanel(this.dpQuote);
            //base.AddDataPanel(this.dpQuotePart);

            this.dsQuotes.Quote.QuoteRowChanged += this.Quote_QuoteRowChanged;
            this.dsQuotes.QuoteSummary.QuoteSummaryRowDeleted += this.QuoteSummary_QuoteSummaryRowDeleted;
        }

        private void LoadTOC()
        {
            using(new UsingTreeLoad(tvwTOC))
            {
                tvwTOC.Nodes.Clear();

                UltraTreeNode rootNode = new InventoryRootNode(this);
                tvwTOC.Nodes.Add(rootNode);
                rootNode.Expanded = true;

                var showAll = ((StateButtonTool)toolbarManager.Tools["DisplayInActive"]).Checked;

                foreach(QuoteDataSet.QuoteSummaryRow cr in this.dsQuotes.QuoteSummary)
                    rootNode.Nodes.Add(new QuoteNode(cr, this){Visible = showAll || cr.Status == "Open"});
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
                //base.Commands.AddCommand("AddQuote", new AddNodeCommand<UltraTreeNode>(toolbarManager.Tools["AddQuote"], this, tvwTOC){AddNode = this.AddQuote});
                //base.Commands.AddCommand("AddPart", new AddNodeCommand<QuoteNode>(toolbarManager.Tools["AddPart"], this, tvwTOC){AddNode = this.AddPart});
                base.Commands.AddCommand("Delete", new DeleteCommand(toolbarManager.Tools["Delete"], tvwTOC, this));
                base.Commands.AddCommand("DisplayInActive", new DisplayInactiveCommand((StateButtonTool)toolbarManager.Tools["DisplayInActive"], tvwTOC));
                base.Commands.AddCommand("Close", new DeactivateNodeCommand(toolbarManager.Tools["Close"], tvwTOC));
                base.Commands.AddCommand("Copy", new CopyPasteCommand(toolbarManager.Tools["Copy"], tvwTOC));
                base.Commands.AddCommand("QuoteLogReport", new QuoteLogReportCommand(toolbarManager.Tools["QuoteLogReport"]));
            }

            //base.Commands.AddCommand("Search", new SearchCommand(toolbarManager.Tools["AdvancedSearch"], tvwTOC, this));
            base.Commands.AddCommand("Print", new PrintNodeCommand(toolbarManager.Tools["Print"], tvwTOC));
            base.Commands.AddCommand("VideoTutorial", new VideoCommand(toolbarManager.Tools["VideoTutorial"]) { Url = "http://www.youtube.com/watch?feature=player_embedded&v=urCrtbF2WQo" });
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
            ////create new data source
            //var cr = this.dpQuotePart.AddRow(rn.QuoteID);

            ////create new ui nodes
            //var cn = new QuotePartNode(cr);
            //rn.Nodes.Add(cn);
            //cn.Select();
        }

        protected override void LoadNode(UltraTreeNode node)
        {
            if(node is QuoteNode)
            {
                var quote = (QuoteNode)node;

                //Lazy load children Data and Nodes
                quote.LoadChildrenNodes();

                //this.dpQuote.MoveToRecord(quote.QuoteID);
                //DisplayPanel(this.dpQuote);
            }
            else
                DisplayPanel(null);
        }

        protected override void SaveSelectedNode()
        {
            if(tvwTOC.SelectedNodes.Count > 0)
                Properties.Settings.Default.LastSelectedCustomer = tvwTOC.SelectedNodes[0].Key;
        }

        protected override void OnDispose()
        {
            if(this.dsQuotes != null)
            {
                this.dsQuotes.Quote.QuoteRowChanged -= this.Quote_QuoteRowChanged;
                this.dsQuotes.QuoteSummary.QuoteSummaryRowDeleted += this.QuoteSummary_QuoteSummaryRowDeleted;
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
                if(node is QuoteNode)
                {
                    //var partNodes = node.Nodes.OfType<QuotePartNode>();
                    //partNodes.ForEach(pn => this.dpQuotePart.MoveToRecord(pn.ID));
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error("Error loading node children", exc);
            }
        }

        private void GoToPart(QuoteDataSet.QuoteSearchRow quoteSearchRow)
        {
            var active = quoteSearchRow.IsClosedDateNull();

            if(!active)
                this.ShowInactive = true;

            var quote = dsQuotes.QuoteSummary.FindByQuoteID(quoteSearchRow.QuoteID);

            if(quote == null)
            {
                taQuoteSummary.FillByID(dsQuotes.QuoteSummary, quoteSearchRow.QuoteID);
            }

            quote = dsQuotes.QuoteSummary.FindByQuoteID(quoteSearchRow.QuoteID);

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
                _log.Fatal("Error closing form.", exc);
            }
        }

        private void Customers_Load(object sender, EventArgs e)
        {
            _loadingData = true; //prevent TOC from loading

            try
            {
                this.LoadCommands();
                this.LoadData();
                this.LoadTOC();
                base.LoadValidators();

                _loadingData = false;

                //select first node customer node
                base.RestoreLastSelectedNode(Properties.Settings.Default.LastSelectedCustomer);

                //splitContainer1.Panel2.Enabled = SecurityManager.Current.IsInRole("Customers.Edit");
            }
            catch(Exception exc)
            {
                splitContainer1.Enabled = false;
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
                _log.Fatal("Error loading form.", exc);
            }
        }

        private void Quote_QuoteRowChanged(object sender, QuoteDataSet.QuoteRowChangeEvent e)
        {
            try
            {
                if(this._inSavedData)
                    return;

                if(e.Action == DataRowAction.Change)
                {
                    var n = tvwTOC.Nodes[0].FindNode<QuoteNode>(q => q.QuoteID == e.Row.QuoteID);

                    if(n != null)
                        n.UpdateNodeUI();
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error("Error on quote change.", exc);
            }
        }

        private void QuoteSummary_QuoteSummaryRowDeleted(object sender, QuoteDataSet.QuoteSummaryRowChangeEvent e)
        {
            try
            {
                //if  summary is deleted, then we need to also delete the corresponding actual quote row
                var quoteID = e.Row["QuoteID", DataRowVersion.Original];

                var quote = this.dsQuotes.Quote.FindByQuoteID(Convert.ToInt32(quoteID));
                if(quote != null)
                    quote.Delete();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error("Error  ", exc);
            }
        }

        #endregion

        #region Nodes

        #region Nested type: QuoteNode

        private class QuoteNode: DataNode<QuoteDataSet.QuoteSummaryRow>, IActive, IReportNode
        {
            #region Fields

            private InventoryManager _quotesManager;

            #endregion

            #region Properties

            private const string KEY_PREFIX = "QUOTE";

            public QuoteDataSet.QuoteRow Quote { get; set; }

            public bool IsDataLoaded { get; set; }

            public int QuoteID
            {
                get { return this.Quote == null || this.Quote.RowState == DataRowState.Detached ? DataRow.QuoteID : this.Quote.QuoteID; }
            }

            public override bool HasChanges
            {
                get
                {
                    this.LoadChildrenNodes();

                    return this.Quote != null && (this.Quote.RowState != DataRowState.Unchanged || this.Quote.GetQuotePartRows().Any(qpr => qpr.RowState != DataRowState.Unchanged));
                }
            }
            public bool IsActiveData
            {
                get { return this.Quote == null ? DataRow.Status == "Open" : this.Quote.Status == "Open"; }
                set
                {
                    if(this.Quote != null)
                        this.Quote.Status = value ? "Open" : "Closed";
                    if(DataRow != null)
                        DataRow.Status = value ? "Open" : "Closed";

                    this.UpdateNodeUI();
                }
            }

            #endregion

            #region Methods

            public QuoteNode(QuoteDataSet.QuoteSummaryRow cr, InventoryManager quotesManager)
                : base(cr, cr.QuoteID.ToString(), KEY_PREFIX, cr.QuoteID.ToString())
            {
                this._quotesManager = quotesManager;
                //update UI
                LeftImages.Add(Properties.Resources.Quote_16);
                this.UpdateNodeUI();
            }

            public override bool CanDelete
            {
                get { return true; }
            }

            public override void UpdateNodeUI()
            {
                Text = this.QuoteID.ToString();

                if(this.IsActiveData)
                    Override.NodeAppearance.ResetForeColor();
                else
                    Override.NodeAppearance.ForeColor = Color.Red;
            }

            internal void LoadChildrenNodes()
            {
                try
                {
                    //if this is a new order then don't bother loading child rows, there aren't any
                    if(DataRow.RowState == DataRowState.Added)
                        return;

                    if(!this.IsDataLoaded)
                    {
                        using(new UsingTimeMe("Loading Nodes for quote " + base.DataRow.QuoteID))
                        {
                            using(new UsingTreeLoad(base.Control))
                            {
                                var quoteID = DataRow.QuoteID;

                                //LOAD Quote
                                if(this.Quote == null)
                                {
                                    this._quotesManager.taQuote.FillBy(this._quotesManager.dsQuotes.Quote, quoteID);
                                    this.Quote = this._quotesManager.dsQuotes.Quote.FindByQuoteID(quoteID);
                                }

                                //LOAD QuoteParts
                                this._quotesManager.taQuotePart.FillBy(this._quotesManager.dsQuotes.QuotePart, quoteID);

                                foreach(var r in this.Quote.GetQuotePartRows())
                                    Nodes.Add(new QuotePartNode(r));

                                this.IsDataLoaded = true;
                            }
                        }
                    }
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error loading quote node children.";
                    LogManager.GetCurrentClassLogger().Error(errorMsg, exc);

                    this.IsDataLoaded = true;
                }
            }

            protected override void OnDispose()
            {
                this._quotesManager = null;
                base.OnDispose();
            }

            #endregion

            public override string ClipboardDataFormat
            {
                get
                {
                    return GetType().FullName;
                    ;
                }
            }

            #region IReportNode Members

            public IReport CreateReport(string reportType)
            {
                //Ensure all child data is loaded (Parts)
                this.LoadChildrenNodes();
                this._quotesManager.PreLoadNodeChildren(this);

                return new QuoteReport(this.Quote);
            }

            public string[] ReportTypes()
            {
                return new[]{"Quote"};
            }

            #endregion

            public override UltraTreeNode PasteData(string format, DataRowProxy proxy)
            {
                var ds = DataRow.Table.DataSet as QuoteDataSet;

                //add new data row
                var dr = DataNode<DataRow>.AddPastedDataRows(proxy, ds.QuotePart) as QuoteDataSet.QuotePartRow;

                var copiedPart = new QuotePartNode(dr);
                base.Nodes.Add(copiedPart);

                copiedPart.Select();
                return copiedPart;
            }

            public override bool CanPasteData(string format)
            {
                return format == typeof(QuotePartNode).FullName;
            }
        }

        #endregion

        #region Nested type: QuotePartNode

        private class QuotePartNode: DataNode<QuoteDataSet.QuotePartRow>
        {
            private const string KEY_PREFIX = "PART";

            #region Methods

            public QuotePartNode(QuoteDataSet.QuotePartRow cr)
                : base(cr, cr.QuotePartID.ToString(), KEY_PREFIX, cr.Name)
            {
                LeftImages.Add(Properties.Resources.Part_16);
                this.UpdateNodeUI();
            }

            public override string ClipboardDataFormat
            {
                get { return GetType().FullName; }
            }

            public override void UpdateNodeUI()
            {
                Text = base.DataRow.IsNull("Name") ? "" : base.DataRow.Name;
            }

            #endregion
        }

        #endregion

        #region Nested type: InventoryRootNode

        private class InventoryRootNode: UltraTreeNode, ICopyPasteNode
        {
            #region Properties

            private InventoryManager _quotes;

            #endregion

            #region Methods

            public InventoryRootNode(InventoryManager quotes)
                : base("ROOT", "Inventory")
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
                //clear any child proxies
                if(proxy.ChildProxies != null)
                    proxy.ChildProxies.Clear();

                //Create the copied new QuoteSummary
                var quoteSummary = DataNode<DataRow>.AddPastedDataRows(proxy, this._quotes.dsQuotes.QuoteSummary) as QuoteDataSet.QuoteSummaryRow;

                //Get Original Quote
                var quote = this._quotes.dsQuotes.Quote.FindByQuoteID(Convert.ToInt32(proxy.OriginalPrimaryKey));

                //Create new Quote
                var quoteItems = quote.ItemArray;
                quoteItems[this._quotes.dsQuotes.Quote.QuoteIDColumn.Ordinal] = null;
                quoteItems[this._quotes.dsQuotes.Quote.CreatedDateColumn.Ordinal] = DateTime.Now;
                quoteItems[this._quotes.dsQuotes.Quote.UserIdColumn.Ordinal] = SecurityManager.Current.UserID;

                var newQuote = this._quotes.dsQuotes.Quote.Rows.Add(quoteItems) as QuoteDataSet.QuoteRow;
                var newQuoteNode = new QuoteNode(quoteSummary, this._quotes){Quote = newQuote, IsDataLoaded = true};
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
                    //this._quotes.dpQuotePart.MoveToRecord(origQuotePart.QuotePartID);

                    //Add Part Media Relations
                    foreach(var origPartMedia in origQuotePart.GetQuotePart_MediaRows())
                        this._quotes.dsQuotes.QuotePart_Media.AddQuotePart_MediaRow(newQuotePart, origPartMedia.MediaRow);

                    //Add Part Process Relations
                    foreach(var origPartProcess in origQuotePart.GetQuotePart_QuoteProcessRows())
                        this._quotes.dsQuotes.QuotePart_QuoteProcess.AddQuotePart_QuoteProcessRow(newQuotePart, origPartProcess.QuoteProcessRow, origPartProcess.StepOrder);
                }

                return newQuoteNode;
            }

            public bool CanPasteData(string format)
            {
                return format == typeof(QuoteNode).FullName;
            }

            public string ClipboardDataFormat
            {
                get { return null; }
            }

            #endregion
        }

        #endregion

        #endregion

        private class SearchCommand : TreeNodeCommandBase
        {
            #region Fields

            private Quotes _quoteManager = null;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.TreeView != null && base.TreeView.Nodes.Count > 0; }
            }

            #endregion

            #region Methods

            public SearchCommand(ToolBase tool, UltraTree toc, Quotes quoteManager)
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
                        if (frm.ShowDialog(_quoteManager) == DialogResult.OK)
                        {
                            if (frm.SelectedQuote != null)
                            {
                               // _quoteManager.GoToPart(frm.SelectedQuote);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error("Error running quote search command.", exc);
                }
            }

            #endregion
        }

        #region QuoteNodeSorter

        private class QuoteNodeSorter : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                if(x is QuoteNode && y is QuoteNode)
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
                    //else if (x is ISortByDate && y is ISortByDate)
                    //    return ((ISortByDate)x).SortByDate.GetValueOrDefault().CompareTo(((ISortByDate)y).SortByDate.GetValueOrDefault());
                else if(x is UltraTreeNode && y is UltraTreeNode)
                    return ((UltraTreeNode)x).Text.CompareTo(((UltraTreeNode)y).Text);
                else
                    return 0;
            }

            #endregion
        }

        #endregion
    }
}