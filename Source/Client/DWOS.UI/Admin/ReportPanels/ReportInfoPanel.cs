using System;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Utilities.Validation;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ReportFieldsDataSetTableAdapters;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace DWOS.UI.Admin.ReportPanels
{
    public partial class ReportInfoPanel: DataPanel, INotifyPropertyChanged
    {
        #region Fields

        private const int MAX_WIDTH = 65535;

        private ReportFieldsTableAdapter _taReportFields = null;
        private List<ReportFieldsDataSet.ReportFieldsRow> rowsToDelete = null;
        private List<string> _collAddField = new List<string>();
        private int _reportID;
     
        #endregion

        #region Properties

        public ReportFieldsDataSet Dataset
        {
            get { return base._dataset as ReportFieldsDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return this.Dataset.Report.ReportIDColumn.ColumnName; }
        }

        public List<string> CollAddField
        {
            get { return _collAddField; }
            set
            {
                _collAddField = value;
                OnPropertyChanged("CollAddField");
            }
        }
        public int NumberOfGridRows { get; set; }
        public ReportFieldsDataSet.ReportFieldsRow[] ReportFieldRows { get; set; }
        public int ReportId { get; set; }
        public string[] DefaultFields { get; set; }
        public string[] CustomFields { get; set; }
        public BindingList<ReportFieldsDataSet.ReportFieldsRow> ReportFields { get; set; }
        public CustomersDataset.CustomFieldDataTable CustomFieldsTable { get; set; }
        public ReportFieldsDataSet.ReportFieldsDataTable DefaultFieldsTable { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportInfoPanel"/> class.
        /// </summary>
        public ReportInfoPanel()
        {
            this.InitializeComponent();
            ReportFieldRows = null;
            grdFields.DisplayLayout.Override.AllowColSizing = AllowColSizing.Free;
            
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <param name="taReportFields">The ta report fields.</param>
        /// <param name="taReport">The ta report.</param>
        public void LoadData(ReportFieldsDataSet dataset, ReportFieldsTableAdapter taReportFields, ReportTableAdapter taReport)
        {
            this.Dataset = dataset;
            _taReportFields = taReportFields;
           
            bsData.DataSource = this.Dataset;
            bsData.DataMember = this.Dataset.Report.TableName;

            //bind column to control
            base.BindValue(this.cboCustomer, this.Dataset.Report.CustomerIDColumn.ColumnName, true);

            //bind lists
            base.BindList(this.cboCustomer, this.Dataset.ReportFieldsCustomerSummary, this.Dataset.ReportFieldsCustomerSummary.CustomerIDColumn.ColumnName, this.Dataset.ReportFieldsCustomerSummary.NameColumn.ColumnName);

            //Fill the list based on report type enum
            foreach (ReportFieldMapper.enumReportType enumValue in Enum.GetValues(typeof(ReportFieldMapper.enumReportType)))
            {
                cboReportType.Items.Add(new ValueListItem(enumValue, ReportFieldMapper.GetReportName(enumValue)));
            }

            if(cboReportType.Items.Count > 0)
                cboReportType.SelectedIndex = 0;

            base._panelLoaded = true;
        }

        public override void AddValidators(DWOS.Utilities.Validation.ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboReportType, "Report type required."), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboCustomer, "Customer required.") { PreserveWhitespace = true }, errProvider));
        }

        /// <summary>
        /// Adds a customer report row for the given report type.
        /// </summary>
        /// <param name="reportType"></param>
        /// <returns>
        /// New instance if there is at least one customer; otherwise, false
        /// </returns>
        public ReportFieldsDataSet.ReportRow Add(ReportFieldMapper.enumReportType reportType)
        {
            if (this.Dataset.ReportFieldsCustomerSummary.Count == 0)
            {
                return null;
            }

            var rowVw     = bsData.AddNew() as DataRowView;
            var cr        = rowVw.Row as ReportFieldsDataSet.ReportRow;
            cr.ReportType = Convert.ToInt32(reportType);
            cr.ReportName = ReportFieldMapper.GetReportName(reportType);
           

            //Just set to first customer for now
            cr.CustomerID = this.Dataset.ReportFieldsCustomerSummary[0].CustomerID;

            int count = 1;
            foreach (var token in ReportFieldMapper.GetReportTokens(reportType, cr.CustomerID))
            {
                this.Dataset.ReportFields.AddReportFieldsRow(token.FieldName, token.DisplayName, ReportToken.WIDTH, count++, token.IsCustom, cr);                
            }

            return cr;
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            cboCustomer.ValueChanged -= cboCustomer_ValueChanged;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            cboCustomer.ValueChanged += cboCustomer_ValueChanged;
            ReportId = Convert.ToInt32(id);
            base.AfterMovedToNewRecord(id);

            var current = this.CurrentRecord as ReportFieldsDataSet.ReportRow;
            LoadFields(current);
        }

        private void LoadFields(ReportFieldsDataSet.ReportRow reportRow)
        {
            var reportFields = new BindingList<ReportFieldsDataSet.ReportFieldsRow>();
            if (reportRow != null)
            {
                this._reportID = reportRow.ReportID;

                cboCustomer.Enabled = false;
                ReportFieldsDataSet.ReportFieldsRow[] rows = reportRow.GetReportFieldsRows();
                ReportFieldRows = rows;
                foreach (var row in rows)
                    reportFields.Add(row);
                ReportFields = reportFields;
                if (!ReportFields.Any())
                {
                    InitializeDefaultGrid();
                }

                grdFields.DataSource = ReportFields;
                reportFields.AllowNew = true;
                cboCustomer.Enabled = reportRow.ReportID <= -1;
            }
            BuildAddFieldList();
        }

        /// <summary>
        /// Gets the default fields.
        /// </summary>
        private void GetDefaultFields()
        {
            if (cboCustomer.Value != null)
            {
                var reportTokens = ReportFieldMapper.GetReportTokens(ReportFieldMapper.enumReportType.PackingSlip, Convert.ToInt32(cboCustomer.Value));
                var _reportFieldsDataTable = new ReportFieldsDataSet.ReportFieldsDataTable();
                _taReportFields = new ReportFieldsTableAdapter();
                _taReportFields.Fill(_reportFieldsDataTable);
                DefaultFields = reportTokens.Select(s => s.DisplayName).ToArray();
                DefaultFieldsTable = _reportFieldsDataTable;
            }
        }

        /// <summary>
        /// Gets the cutom fields.
        /// </summary>
        private void GetCustomFields()
        {
            if (cboCustomer.Value != null)
            {
                int customerId = Convert.ToInt32(cboCustomer.Value);
                var _customerDataTable = new CustomersDataset.CustomFieldDataTable();
                var _taCustomFields = new CustomFieldTableAdapter();
                _taCustomFields.FillByCustomer(_customerDataTable, customerId);
                CustomFields = _customerDataTable.AsEnumerable().Select(s => s.Field<string>("Name")).ToArray<string>();
                CustomFieldsTable = _customerDataTable;
                
            }
        }

        /// <summary>
        /// Resets the coll add field.
        /// </summary>
        private void ResetCollAddField()
        {
            CollAddField.Clear();
            CollAddField.AddRange(GetNonDefaultFields());
        }

        private static List<string> GetNonDefaultFields()
        {
            return new List<string>()
            {
                ReportFieldMapper.GetDisplayName(ReportFieldMapper.enumReportTokens.SalesOrder),
                ReportFieldMapper.GetDisplayName(ReportFieldMapper.enumReportTokens.OrderPrice),
                ReportFieldMapper.GetDisplayName(ReportFieldMapper.enumReportTokens.ContainerCount),
                ReportFieldMapper.GetDisplayName(ReportFieldMapper.enumReportTokens.GrossWeight),
                ReportFieldMapper.GetDisplayName(ReportFieldMapper.enumReportTokens.PartDescription),
                ReportFieldMapper.GetDisplayName(ReportFieldMapper.enumReportTokens.ProcessAlias),
                ReportFieldMapper.GetDisplayName(ReportFieldMapper.enumReportTokens.SerialNumber),
                ReportFieldMapper.GetDisplayName(ReportFieldMapper.enumReportTokens.OrderWeight),
                ReportFieldMapper.GetDisplayName(ReportFieldMapper.enumReportTokens.ContainerDescription),
            };
        }

        /// <summary>
        /// Filters the fields list.
        /// </summary>
        private void FilterFieldsList()
        {
            ResetCollAddField();
            var fieldNames = new List<string>();
            foreach (var row in grdFields.Rows)
            {
                if (row != null && row.IsDataRow)
                {
                    var dataRowView = row.ListObject as ReportFieldsDataSet.ReportFieldsRow;
                    if (dataRowView != null && dataRowView.IsValidState())
                    {
                       if (row.Cells["DisplayName"].Value != null)
                            {
                                fieldNames.Add(row.Cells["DisplayName"].Value.ToString());
                            }
                        }
                    }
                }
            
            if (fieldNames.Count > 0)
            {
                var fieldsToRemove = fieldNames.Where(f => !string.IsNullOrEmpty(f)).ToList();

                var nonDefaultFields = GetNonDefaultFields();

                foreach (var nonDefaultFieldToRemove in fieldsToRemove.Where(f => nonDefaultFields.Contains(f)))
                {
                    CollAddField.Remove(nonDefaultFieldToRemove);
                }
               
                if (fieldsToRemove.Count > 0)
                {
                    if (DefaultFields != null)
                    {
                        foreach (var name in DefaultFields.Where(name => !fieldNames.Contains(name)))
                        {
                            if (!CollAddField.Contains(name))
                            {
                                CollAddField.Add(name);
                            }
                        }
                    }
                    if (CustomFields != null)
                    {
                        foreach (var field in CustomFields.Where(field => !fieldNames.Contains(field)))
                        {
                            if (!CollAddField.Contains(field))
                            {
                                CollAddField.Add(field);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Builds the add field list.
        /// </summary>
        private void BuildAddFieldList()
        {
            GetDefaultFields();
            GetCustomFields();
            FilterFieldsList();

            btnAddField.Enabled = CollAddField.Count > 0;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Events

        private void grdFields_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                //Hide all the columns
                foreach (var col in grdFields.DisplayLayout.Bands[0].Columns)
                    col.Hidden = true;

                //Just the columns we need, set the order and add user friendly column names
                grdFields.DisplayLayout.Bands[0].Columns["FieldName"].Header.Caption = "Field Name";
                grdFields.DisplayLayout.Bands[0].Columns["FieldName"].Header.VisiblePosition = 0;
                grdFields.DisplayLayout.Bands[0].Columns["FieldName"].CellAppearance.TextHAlign = HAlign.Left;
                grdFields.DisplayLayout.Bands[0].Columns["FieldName"].Hidden = false;
                grdFields.DisplayLayout.Bands[0].Columns["FieldName"].CellActivation = Activation.NoEdit;
                grdFields.DisplayLayout.Bands[0].Columns["DisplayName"].Header.Caption = "Display Name";
                grdFields.DisplayLayout.Bands[0].Columns["DisplayName"].Header.VisiblePosition = 1;
                grdFields.DisplayLayout.Bands[0].Columns["DisplayName"].CellAppearance.TextHAlign = HAlign.Left;
                grdFields.DisplayLayout.Bands[0].Columns["DisplayName"].Hidden = false;
                grdFields.DisplayLayout.Bands[0].Columns["Width"].Header.VisiblePosition = 2;
                grdFields.DisplayLayout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = HAlign.Right;
                grdFields.DisplayLayout.Bands[0].Columns["Width"].Hidden = false;
                grdFields.DisplayLayout.Bands[0].Columns["DisplayOrder"].Header.Caption = "Display Order";
                grdFields.DisplayLayout.Bands[0].Columns["DisplayOrder"].Header.VisiblePosition = 3;
                grdFields.DisplayLayout.Bands[0].Columns["DisplayOrder"].CellAppearance.TextHAlign = HAlign.Right;
                grdFields.DisplayLayout.Bands[0].Columns["DisplayOrder"].Hidden = false;
                grdFields.DisplayLayout.Bands[0].Columns["DisplayOrder"].CellActivation = Activation.NoEdit;


                e.Layout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

                this.grdFields.DisplayLayout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
                grdFields.DisplayLayout.Bands[0].Columns["DisplayName"].ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.OnCellActivate;

                //Set column to spinner style
                grdFields.DisplayLayout.Bands[0].Columns["Width"].MinValue = 0;
                grdFields.DisplayLayout.Bands[0].Columns["Width"].MaxValue = MAX_WIDTH;
                grdFields.DisplayLayout.Bands[0].Columns["Width"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.IntegerWithSpin;
                grdFields.DisplayLayout.Bands[0].Columns["Width"].ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.OnCellActivate;
                

                grdFields.DisplayLayout.Bands[0].Columns["DisplayOrder"].MinValue = 1;
                grdFields.DisplayLayout.Bands[0].Columns["DisplayOrder"].MaxValue = 100;
                grdFields.DisplayLayout.Bands[0].Columns["DisplayOrder"].ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.OnCellActivate;

                grdFields.DisplayLayout.Bands[0].SortedColumns.Add("DisplayOrder", false);
                grdFields.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.Select;
                grdFields.DisplayLayout.Override.SelectTypeCol = SelectType.None;

                grdFields.DisplayLayout.Bands[0].Columns["DisplayName"].MaxLength = 50;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error initializing grid for customer report fields.");
            }
        }

        private void grdFields_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            try
            {
                rowsToDelete = new List<ReportFieldsDataSet.ReportFieldsRow>();

                e.DisplayPromptMsg = false;

                //Get the list of ReportField rows to delete
                foreach (var deletedRow in e.Rows)
                {
                    var delRow = deletedRow.ListObject as ReportFieldsDataSet.ReportFieldsRow;
                    if (delRow != null)
                        rowsToDelete.Add(delRow);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error before rows deleted in customer report fields.");
            }
        }

        private void grdFields_AfterRowsDeleted(object sender, EventArgs e)
        {
            try
            {
                grdFields.UpdateData();

                //Delete the actual ReportField row
                foreach (ReportFieldsDataSet.ReportFieldsRow row in rowsToDelete)
                    row.Delete();

                BuildAddFieldList();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error after rows deleted in customer report fields.");
            }
        }

        /// <summary>
        /// Handles the Click event of the btnAddField control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnAddField_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboCustomer.Value == null)
                {
                    return;
                }

                var customerID = Convert.ToInt32(cboCustomer.Value);

                BuildAddFieldList();
                var addFieldPnl = new AddReportFieldPanel(CollAddField);
                addFieldPnl.ShowDialog();
                if (addFieldPnl.DialogResult == DialogResult.OK)
                {
                    var rowVw = bsData.Current as DataRowView;

                    if (rowVw != null)
                    {
                        var cr = rowVw.Row as ReportFieldsDataSet.ReportRow;
                        var rowToAdd = DefaultFieldsTable.FirstOrDefault(c => c.DisplayName == addFieldPnl.SelectedField && c.ReportID == this._reportID);
                        var customRow = CustomFieldsTable.FirstOrDefault(c => c.Name == addFieldPnl.SelectedField);

                        if (rowToAdd != null && customRow != null && rowToAdd.FieldName != customRow.CustomFieldID.ToString())
                        {
                            // rowToAdd is for a custom field that belongs to a different customer
                            rowToAdd = null;
                        }


                        NumberOfGridRows = 0;
                        
                        //Get number of grid rows
                        foreach (var row in grdFields.Rows)
                        {
                            var dataRowView = row.ListObject as ReportFieldsDataSet.ReportFieldsRow;
                            if (dataRowView != null && dataRowView.IsValidState())
                            {
                                NumberOfGridRows++;
                            }
                        }

                        if (rowToAdd != null)
                        {
                            var addRow = this.Dataset.ReportFields.AddReportFieldsRow(rowToAdd.FieldName, rowToAdd.DisplayName, rowToAdd.Width, NumberOfGridRows + 1, rowToAdd.IsCustomField, cr);
                            ReportFields.Add(addRow);
                        }
                        else if(customRow != null)
                        {
                            var reportFieldRow = this.Dataset.ReportFields.AddReportFieldsRow(customRow.CustomFieldID.ToString(), customRow.Name, 100, NumberOfGridRows + 1, true, cr);
                            ReportFields.Add(reportFieldRow);
                        }
                        else
                        {
                            var fieldName = ReportFieldMapper.GetReportTokenFieldName(addFieldPnl.SelectedField);
                            var reportRow = this.Dataset.ReportFields.AddReportFieldsRow(fieldName, addFieldPnl.SelectedField, 100, NumberOfGridRows + 1, false, cr);
                            ReportFields.Add(reportRow);
                        }
                    }
                }

                //rebuild the list after new fields are added
                BuildAddFieldList();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding customer report fields.");
            }
        }

        /// <summary>
        /// Handles the Click event of the btnDeleteField control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnDeleteField_Click(object sender, EventArgs e)
        {
            try
            {
                //Get the row to move
                UltraGridRow activeRow = grdFields.ActiveRow;

                if (activeRow != null)
                {
                    var originalOrderValue = Convert.ToInt32(activeRow.Cells["DisplayOrder"].Value);
                    int orderCount = originalOrderValue;

                    //Get the row above our row
                    foreach (var row in grdFields.Rows.Where(row => Convert.ToInt32(row.Cells["DisplayOrder"].Value) > originalOrderValue))
                    {
                        UltraGridRow belowRow = row;
                        if (belowRow != null)
                        {
                            belowRow.Cells["DisplayOrder"].Value = orderCount;
                            orderCount++;
                        }
                    }
                    grdFields.UpdateData();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error moving report field down.");
            }
            try
            {
                //Get the row to delete
                UltraGridRow row = grdFields.ActiveRow;
                if (row != null)
                    row.Delete();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error deleting customer report fields.");
            }
        }

        /// <summary>
        /// Handles the Click event of the btnMoveFieldUp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnMoveFieldUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdFields.ActiveRow != null)
                {
                    var dataRowView = grdFields.ActiveRow.ListObject as ReportFieldsDataSet.ReportFieldsRow;
                    if (dataRowView != null && dataRowView.IsValidState())
                    {
                        //Get the row to move
                        UltraGridRow activeRow = grdFields.ActiveRow;
                        var originalOrderValue = activeRow.Cells["DisplayOrder"].Value;
                        var aboveRowValue = Convert.ToInt32(originalOrderValue) - 1;

                        //Get the row above our row
                        foreach (var row in grdFields.Rows.Where(row => Convert.ToInt32(row.Cells["DisplayOrder"].Value) == aboveRowValue))
                        {
                            UltraGridRow aboveRow = row;

                            if (aboveRow != null)
                            {
                                aboveRow.Cells["DisplayOrder"].Value = originalOrderValue;
                                activeRow.Cells["DisplayOrder"].Value = Convert.ToInt32(activeRow.Cells["DisplayOrder"].Value) - 1;

                                grdFields.Rows.Move(activeRow, aboveRowValue - 1);
                                grdFields.Rows.Move(aboveRow, Convert.ToInt32(originalOrderValue) - 1);
                            }
                        }
                        grdFields.UpdateData();
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error moving report field up.");
            }
        }

        /// <summary>
        /// Handles the Click event of the btnMoveFieldDown control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnMoveFieldDown_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdFields.ActiveRow != null)
                {
                    var dataRowView = grdFields.ActiveRow.ListObject as ReportFieldsDataSet.ReportFieldsRow;
                    if (dataRowView != null && dataRowView.IsValidState())
                    {
                        //Get the row to move
                        UltraGridRow activeRow = grdFields.ActiveRow;
                        var originalOrderValue = activeRow.Cells["DisplayOrder"].Value;
                        var belowRowValue = Convert.ToInt32(originalOrderValue) + 1;

                        //Get the row above our row
                        foreach (var row in grdFields.Rows.Where(row => Convert.ToInt32(row.Cells["DisplayOrder"].Value) == belowRowValue))
                        {
                            UltraGridRow belowRow = row;

                            if (belowRow != null)
                            {
                                belowRow.Cells["DisplayOrder"].Value = originalOrderValue;
                                activeRow.Cells["DisplayOrder"].Value = Convert.ToInt32(activeRow.Cells["DisplayOrder"].Value) + 1;

                                grdFields.Rows.Move(activeRow, belowRowValue - 1);
                                grdFields.Rows.Move(belowRow, Convert.ToInt32(originalOrderValue) - 1);
                            }
                        }
                        grdFields.UpdateData();
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error moving report field down.");
            }
        }

        /// <summary>
        /// Handles the ValueChanged event of the cboCustomer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cboCustomer_ValueChanged(object sender, EventArgs e)
        {  
            InitializeDefaultGrid();
            BuildAddFieldList();
        }

        /// <summary>
        /// Initializes the default grid.
        /// </summary>
        private void InitializeDefaultGrid()
        {
            var reportFields = new BindingList<ReportFieldsDataSet.ReportFieldsRow>();
            var rowVw = bsData.Current as DataRowView;

            if (rowVw != null)
            {
                var cr = rowVw.Row as ReportFieldsDataSet.ReportRow;

                // Delete all current report fields
                foreach (var field in cr.GetReportFieldsRows())
                {
                    field.Delete();
                }

                if (cboCustomer.Value != null)
                {
                    var reportTokens = ReportFieldMapper.GetReportTokens(ReportFieldMapper.enumReportType.PackingSlip, Convert.ToInt32(cboCustomer.Value));
                    int index = 1;
                    foreach (var token in reportTokens)
                    {
                        var rowToAdd = this.Dataset.ReportFields.AddReportFieldsRow(token.FieldName, token.DisplayName, token.Width, index, token.IsCustom, cr);
                        reportFields.Add(rowToAdd);
                        index++;
                    }
                    ReportFields = reportFields;
                    grdFields.DataSource = ReportFields;
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}