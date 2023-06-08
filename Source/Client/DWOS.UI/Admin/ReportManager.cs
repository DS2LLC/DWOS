using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Windows.Annotations;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Reports;
using DWOS.Data;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using Newtonsoft.Json;
using NLog;

namespace DWOS.UI.Admin
{
    public partial class ReportManager: DataEditorBase
    {
        #region Fields

        public const string REPORT_TYPE_TABLENAME = "ReportType";
        public const string REPORT_TYPE_ID_COLUMN = "ReportTypeID";
        public const string REPORT_NAME_COLUMN = "Name";

        #endregion

        #region Methods

        public ReportManager()
        {
            this.InitializeComponent();
        }

        private void LoadData()
        {
            try
            {
                dsReportFields.EnforceConstraints = false;
                dsReportFields.BeginInit();
                taReportFields.Fill(dsReportFields.ReportFields);
                taReport.Fill(dsReportFields.Report);
                taReportFieldsCustomerSummary.Fill(dsReportFields.ReportFieldsCustomerSummary);
                taCustomField.Fill(dsReportFields.CustomFieldName);
                dsReportFields.EndInit();

                //Create in-memory DataTable (also DataRow later) required by DataNode and DataPanel types
                // There is an existing ReportType Table but it is not related to this (CustomReports)
                this.AddReportTypeTable();

                pnlReportType.LoadData(dsReportFields);
                base.AddDataPanel(pnlReportType);

                pnlReportInfo.LoadData(dsReportFields, taReportFields, taReport);
                base.AddDataPanel(pnlReportInfo);
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error loading report data.", exc);
            }
        }

        private void LoadTOC()
        {
            using (new UsingTreeLoad(tvwTOC))
            {
                tvwTOC.Nodes.Clear();

                var rootNode = new ReportsRootNode();
                tvwTOC.Nodes.Add(rootNode);
                rootNode.Expanded = true;
                
                foreach(DataRow reportType in dsReportFields.Tables[REPORT_TYPE_TABLENAME].Rows)
                {
                    var reportTypeNode = new ReportTypeNode(reportType);
                    var reportTypeId = Convert.ToInt32(reportType[REPORT_TYPE_ID_COLUMN]);
                    reportTypeNode.Report = dsReportFields.Report.GetDefaultReport((ReportFieldMapper.enumReportType) reportTypeId);

                    rootNode.Nodes.Add(reportTypeNode);

                    //don't load NULL customer as this is the company default values for this report type
                    var reports = dsReportFields.Report.Where(rep => !rep.IsCustomerIDNull() && rep.ReportType == reportTypeId);

                    foreach (var report in reports)
                        reportTypeNode.Nodes.Add(new ReportNode(report));
                }
            }
        }

        protected override void ReloadTOC()
        {
            this.LoadTOC();
        }

        protected override bool SaveData()
        {
            try
            {
                base.EndAllEdits();
                
                this.taManager.UpdateAll(this.dsReportFields);
                
                return true;
            }
            catch(Exception exc)
            {
                _log.Warn("DataSet Errors: " + dsReportFields.GetDataErrors());

                ErrorMessageBox.ShowDialog("Error saving data.", exc);
                return false;
            }
        }

        private void LoadCommands()
        {
            var addP = base.Commands.AddCommand("Add", new AddCommand(toolbarManager.Tools["Add"], tvwTOC)) as AddCommand;
            addP.AddNode += (s, e) => AddNode();
            
            var dc = base.Commands.AddCommand("Delete", new DeleteCommand(toolbarManager.Tools["Delete"], tvwTOC, this)) as DeleteCommand;
        }

        private void AddNode()
        {
            var selectedNode = tvwTOC.SelectedNode<UltraTreeNode>();

            if(IsValidControls())
            {
                _validators.Enabled = false;
                AddReport(selectedNode);
                _validators.Enabled = true;
            }
        }

        private void AddReport(UltraTreeNode pn)
        {
            _log.Info("Adding a report node.");

            //if on report type node then cool, else see if parent is one.
            var reportTypeNode = pn as ReportTypeNode ?? pn.Parent as ReportTypeNode;

            if (reportTypeNode != null)
            {
                var reportRow = pnlReportInfo.Add(reportTypeNode.ReportType);

                if (reportRow == null)
                {
                    // Show warning
                    MessageBoxUtilities.ShowMessageBoxWarn(
                        "Unable to add a report.\nPlease add at least one customer and try again.",
                        "Report Manager");
                }
                else
                {
                    var cn = new ReportNode(reportRow);

                    reportTypeNode.Nodes.Add(cn);
                    cn.Select();
                    LoadNode(cn);
                }
            }
        }

        protected override void LoadNode(UltraTreeNode node)
        {
            DataPanel curDataPanel  = null;
            string dataID           = null;

            if (node is ReportNode)
            {
                var prNode = (ReportNode)node;
                curDataPanel = this.pnlReportInfo;
                dataID = prNode.ID;
            }
            else if (node is ReportTypeNode && ((ReportTypeNode)node).Report != null)
            {
                var prNode = (ReportTypeNode)node;
                curDataPanel = this.pnlReportType;
                dataID = prNode.Report.ReportID.ToString();
            }
           
            //if panel set then move to it
            if (curDataPanel != null)
            {
                curDataPanel.MoveToRecord(dataID);
                DisplayPanel(curDataPanel);
            }
            else
                DisplayPanel(null);
        }

        protected override void SaveSelectedNode()
        {
            // Do nothing - must be implemented
        }

        /// <summary>
        /// Adds the report type table to exisiting DataSet.
        /// </summary>
        private void AddReportTypeTable()
        {
            //Create in-memory DataTable required by DataNode and DataPanel types
            DataTable table = dsReportFields.Tables.Add(REPORT_TYPE_TABLENAME);
            
            //Create columns and primary key (used by DataPanel)
            DataColumn[] key = new DataColumn[1];
            key[0] = table.Columns.Add(REPORT_TYPE_ID_COLUMN, typeof(int)); 
            table.Columns.Add(REPORT_NAME_COLUMN, typeof(string));
            table.PrimaryKey = key;

            //Add customer report types rows
            dsReportFields.Tables[REPORT_TYPE_TABLENAME].Rows.Add(ReportFieldMapper.enumReportType.PackingSlip, ReportFieldMapper.GetReportName(ReportFieldMapper.enumReportType.PackingSlip));
        }

        #endregion

        #region Events

        private void ReportManager_Load(object sender, EventArgs e)
        {
            _loadingData = true; //prevent TOC from loading

            try
            {
                if(DesignMode)
                    return;

                tvwTOC.Override.SelectionType = SelectType.Extended;
                LoadCommands();
                this.LoadData();
                this.LoadTOC();
                LoadValidators();

                tvwTOC.Override.Sort = SortType.Ascending;

                _loadingData = false;
            }
            catch(Exception exc)
            {
                splitContainer1.Enabled = false;
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
                _log.Fatal(exc, "Error loading form.");
            }
        }
        
        #endregion

        #region Nodes

        #region Nested type: ReportTypeNode

        private class ReportTypeNode : DataNode<DataRow>
        {
            #region Fields

            public const string KEY_PREFIX = "FOLDER";
            private static Image _imageCache = Properties.Resources.Folder_16;

            #endregion

            #region Properties

            public ReportFieldMapper.enumReportType ReportType { get; private set;}

            public ReportFieldsDataSet.ReportRow Report { get; set; }

            #endregion

            #region Methods

            public ReportTypeNode(DataRow row)
                : base(row, row["ReportTypeID"].ToString(), KEY_PREFIX, row["Name"].ToString())
            {
                LeftImages.Add(_imageCache);

                var reportTypeId = Convert.ToInt32(row["ReportTypeID"]);
                this.ReportType = (ReportFieldMapper.enumReportType) reportTypeId;
            }
           
            public override bool CanDelete
            {
                get { return false; }
            }
            
            #endregion
        }

        //Future:  For use with ReportType Table
        //private class ReportTypeNode : DataNode<ReportFieldsDataSet.ReportFieldsTypeRow>
        //{
        //    #region Fields

        //    public const string KEY_PREFIX = "FOLDER";
        //    private static Image _imageCache = Properties.Resources.Folder_16;

        //    #endregion

        //    #region Properties

        //    #endregion

        //    #region Methods

        //    public ReportTypeNode(ReportFieldsDataSet.ReportFieldsTypeRow row)
        //        : base(row, row.ReportTypeID.ToString(), KEY_PREFIX, row.DisplayName)
        //    {
        //        LeftImages.Add(_imageCache);
        //    }

        //    public override bool CanDelete
        //    {
        //        get { return false; }
        //    }

        //    #endregion
        //}

        #endregion

        #region Nested type: ReportNode

        private class ReportNode : DataNode<ReportFieldsDataSet.ReportRow>
        {
            #region Fields

            public const string KEY_PREFIX = "RF";
            private static Image _imageCache = null;

            #endregion

            #region Properties


            #endregion

            #region Methods

            public ReportNode(ReportFieldsDataSet.ReportRow cr)
                : base(cr, cr.ReportID.ToString(), KEY_PREFIX, "New Report")
            {
                if (_imageCache == null)
                    _imageCache = Properties.Resources.Report16;

                LeftImages.Add(_imageCache);

                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                try
                { 
                    Text = (base.DataRow == null ? "-Default-" : base.DataRow.ReportFieldsCustomerSummaryRow == null ? "New" : base.DataRow.ReportFieldsCustomerSummaryRow.Name);
                }
                catch(Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error updating node UI.");
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: ReportsRootNode

        private class ReportsRootNode : UltraTreeNode
        {
            #region Methods

            public ReportsRootNode()
                : base("ROOT", "Reports")
            {
                LeftImages.Add(Properties.Resources.ReportManager_16);
            }

            #endregion
        }

        #endregion

        #endregion

        #region Commands

        #region Nested type: AddCommand

        private class AddCommand : TreeNodeCommandBase
        {
            #region Fields

            public EventHandler AddNode;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get
                {
                    return _node is ReportTypeNode;
                }
            }

            #endregion

            #region Methods

            public AddCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                if(this.AddNode != null)
                    this.AddNode(this, EventArgs.Empty);
            }

            #endregion
        }

        #endregion

        #endregion
    }
}