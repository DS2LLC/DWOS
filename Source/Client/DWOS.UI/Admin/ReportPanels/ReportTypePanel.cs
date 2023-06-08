using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using Infragistics.Win;
using System.ComponentModel;
using Infragistics.Win.UltraWinGrid;

namespace DWOS.UI.Admin.ReportPanels
{
    public partial class ReportTypePanel : DataPanel
    {
        #region Fields

        private const int MAX_FIELD_WIDTH = 65535;

        /// <summary>
        /// Collection of all fields in the current report.
        /// </summary>
        private readonly BindingList<ReportFieldsDataSet.ReportFieldsRow> _currentReportFields =
            new BindingList<ReportFieldsDataSet.ReportFieldsRow>();

        /// <summary>
        /// Contains widths for deleted rows - used when setting default width of new row.
        /// </summary>
        private readonly Dictionary<string, int> _deletedRowWidths = new Dictionary<string, int>();

        private readonly List<ReportFieldsDataSet.ReportFieldsRow> _rowsToDelete = new List<ReportFieldsDataSet.ReportFieldsRow>();

        private readonly List<ReportField> _customFields = new List<ReportField>();

        #endregion

        #region Properties

        public ReportFieldsDataSet Dataset
        {
            get { return _dataset as ReportFieldsDataSet; }
            set { _dataset = value; }
        }

        private ReportFieldsDataSet.ReportRow CurrentReport => CurrentRecord as ReportFieldsDataSet.ReportRow;

        protected override string BindingSourcePrimaryKey => Dataset.Report.ReportIDColumn.ColumnName;

        #endregion

        #region Methods

        public ReportTypePanel()
        {
            InitializeComponent();
        }

        public void LoadData(ReportFieldsDataSet dataset)
        {
            Dataset = dataset;

            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.Report.TableName;

            grdFields.DataSource = _currentReportFields;

            BindValue(txtReportName, Dataset.Report.ReportNameColumn.ColumnName);

            _customFields.AddRange(dataset.CustomFieldName.Select(ReportField.From));

            _panelLoaded = true;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);
            int reportId;
            _currentReportFields.Clear();
            if (int.TryParse(id.ToString(), out reportId) && reportId > -1)
            {
                foreach (var field in Dataset.ReportFields.Where(f => f.IsValidState() && f.ReportID == reportId))
                {
                    _currentReportFields.Add(field);
                }
            }

            btnAddField.Enabled = GetAvailableFields().Count > 0;
        }

        private List<ReportField> GetAvailableFields()
        {
            var availableFields = Enum.GetValues(typeof(ReportFieldMapper.enumReportTokens))
                .OfType<ReportFieldMapper.enumReportTokens>()
                .Select(ReportField.From)
                .Concat(_customFields)
                .ToList();

            foreach (var row in grdFields.Rows)
            {
                if (row == null || !row.IsDataRow)
                {
                    continue;
                }

                var fieldInGrid = row.ListObject as ReportFieldsDataSet.ReportFieldsRow;
                if (fieldInGrid != null && fieldInGrid.IsValidState())
                {
                    availableFields.Remove(f => f.FieldName == fieldInGrid.FieldName);
                }
            }

            return availableFields;
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the InitializeLayout event of the grdFields control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="InitializeLayoutEventArgs"/> instance containing the event data.</param>
        private void grdFields_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                //Hide all the columns
                foreach (var col in grdFields.DisplayLayout.Bands[0].Columns)
                {
                    col.Hidden = true;
                }

                //Just the columns we need, set the order and add user friendly column names
                grdFields.DisplayLayout.Bands[0].Columns["FieldName"].Header.Caption = "Field Name";
                grdFields.DisplayLayout.Bands[0].Columns["FieldName"].Header.VisiblePosition = 0;
                grdFields.DisplayLayout.Bands[0].Columns["FieldName"].CellAppearance.TextHAlign = HAlign.Left;
                grdFields.DisplayLayout.Bands[0].Columns["FieldName"].Hidden = false;
                grdFields.DisplayLayout.Bands[0].Columns["FieldName"].CellActivation = Activation.NoEdit;
                grdFields.DisplayLayout.Bands[0].Columns["DisplayName"].Hidden = false;
                grdFields.DisplayLayout.Bands[0].Columns["DisplayName"].Header.VisiblePosition = 1;
                grdFields.DisplayLayout.Bands[0].Columns["DisplayName"].Header.Caption = @"Display Name";
                grdFields.DisplayLayout.Bands[0].Columns["DisplayName"].CellAppearance.TextHAlign = HAlign.Left;
                grdFields.DisplayLayout.Bands[0].Columns["Width"].Hidden = false;
                grdFields.DisplayLayout.Bands[0].Columns["Width"].Header.VisiblePosition = 2;
                grdFields.DisplayLayout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = HAlign.Right;
                grdFields.DisplayLayout.Bands[0].Columns["Width"].CellActivation = Activation.AllowEdit;
                grdFields.DisplayLayout.Bands[0].Columns["DisplayOrder"].Hidden = false;
                grdFields.DisplayLayout.Bands[0].Columns["DisplayOrder"].Header.Caption = @"Display Order";
                grdFields.DisplayLayout.Bands[0].Columns["DisplayOrder"].Header.VisiblePosition = 3;
                grdFields.DisplayLayout.Bands[0].Columns["DisplayOrder"].CellAppearance.TextHAlign = HAlign.Right;
                grdFields.DisplayLayout.Bands[0].Columns["DisplayOrder"].CellActivation = Activation.NoEdit;

                // Width column styling
                grdFields.DisplayLayout.Bands[0].Columns["Width"].MinValue = 0;
                grdFields.DisplayLayout.Bands[0].Columns["Width"].MaxValue = MAX_FIELD_WIDTH;
                grdFields.DisplayLayout.Bands[0].Columns["Width"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.IntegerWithSpin;
                grdFields.DisplayLayout.Bands[0].Columns["Width"].ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.OnCellActivate;

                e.Layout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
                grdFields.DisplayLayout.Bands[0].SortedColumns.Add("DisplayOrder", false);
                grdFields.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.Select;
                grdFields.DisplayLayout.Override.SelectTypeCol = SelectType.None;

                grdFields.DisplayLayout.Bands[0].Columns["DisplayName"].MaxLength = 50;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error initializing report fields grid.");
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
                if (CurrentReport == null)
                {
                    return;
                }

                var addFieldPnl = new AddReportFieldPanel(GetAvailableFields().Select(f => f.DisplayName).ToList());
                addFieldPnl.ShowDialog();

                if (addFieldPnl.DialogResult == DialogResult.OK)
                {
                    var addField = addFieldPnl.SelectedField;

                    var isCustomField = Dataset.CustomFieldName.Any(f => f.Name == addField);

                    var fieldName = isCustomField
                        ? addField
                        : ReportFieldMapper.GetReportTokenFieldName(addField);

                    int width;

                    if (!_deletedRowWidths.TryGetValue(fieldName, out width))
                    {
                        width = ReportFieldMapper.DefaultWidth(fieldName);
                    }

                    int displayOrder = 1 + Dataset.ReportFields.Count(field => field.IsValidState() && field.ReportID == CurrentReport.ReportID);
                    var newRow = Dataset.ReportFields.AddReportFieldsRow(fieldName, addField, width, displayOrder, isCustomField, CurrentReport);
                    _currentReportFields.Add(newRow);
                }

                btnAddField.Enabled = GetAvailableFields().Count > 0;
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
                        row.Cells["DisplayOrder"].Value = orderCount;
                        orderCount++;
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
                if (grdFields.ActiveRow != null)
                {
                    //Get the row to delete
                    UltraGridRow activeRow = grdFields.ActiveRow;
                    activeRow?.Delete();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error deleting customer report fields.");
            }
        }

        /// <summary>
        /// Handles the BeforeRowsDeleted event of the grdFields control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BeforeRowsDeletedEventArgs"/> instance containing the event data.</param>
        private void grdFields_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            try
            {
                _rowsToDelete.Clear();

                e.DisplayPromptMsg = false;

                //Get the list of ReportField rows to delete
                foreach (var deletedRow in e.Rows)
                {
                    var reportFieldRow = deletedRow.ListObject as ReportFieldsDataSet.ReportFieldsRow;
                    if (reportFieldRow != null)
                    {
                        _rowsToDelete.Add(reportFieldRow);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error before rows deleted in customer report fields.");
            }
        }

        /// <summary>
        /// Handles the AfterRowsDeleted event of the grdFields control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs"/> instance containing the event data.</param>
        private void grdFields_AfterRowsDeleted(object sender, EventArgs e)
        {
            try
            {
                grdFields.UpdateData();

                //Delete the actual ReportField row
                foreach (var deletedField in _rowsToDelete)
                {
                    _deletedRowWidths.Add(deletedField.FieldName, deletedField.Width);
                    deletedField.Delete();
                }

                // At least one field is available for use after deleting at least one
                btnAddField.Enabled = true;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error after rows deleted in customer report fields.");
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

                    //Get the row to move
                    UltraGridRow activeRow = grdFields.ActiveRow;
                    var originalOrderValue = activeRow.Cells["DisplayOrder"].Value;
                    var aboveRowValue = Convert.ToInt32(originalOrderValue) - 1;

                    //Get the row above our row
                    foreach (var row in grdFields.Rows.Where(row => Convert.ToInt32(row.Cells["DisplayOrder"].Value) == aboveRowValue))
                    {
                        row.Cells["DisplayOrder"].Value = originalOrderValue;
                        activeRow.Cells["DisplayOrder"].Value = Convert.ToInt32(activeRow.Cells["DisplayOrder"].Value) - 1;

                        grdFields.Rows.Move(activeRow, aboveRowValue - 1);
                        grdFields.Rows.Move(row, Convert.ToInt32(originalOrderValue) - 1);
                    }

                    grdFields.UpdateData();
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
                    //Get the row to move
                    UltraGridRow activeRow = grdFields.ActiveRow;
                    var originalOrderValue = activeRow.Cells["DisplayOrder"].Value;
                    var belowRowValue = Convert.ToInt32(originalOrderValue) + 1;

                    //Get the row above our row
                    foreach (var row in grdFields.Rows.Where(row => Convert.ToInt32(row.Cells["DisplayOrder"].Value) == belowRowValue))
                    {
                        row.Cells["DisplayOrder"].Value = originalOrderValue;
                        activeRow.Cells["DisplayOrder"].Value = Convert.ToInt32(activeRow.Cells["DisplayOrder"].Value) + 1;

                        grdFields.Rows.Move(activeRow, belowRowValue - 1);
                        grdFields.Rows.Move(row, Convert.ToInt32(originalOrderValue) - 1);
                    }

                    grdFields.UpdateData();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error moving report field down.");
            }
        }

        #endregion

        #region ReportField

        public class ReportField
        {
            public string FieldName { get; private set; }

            public string DisplayName { get; set; }

            public static ReportField From(ReportFieldMapper.enumReportTokens token)
            {
                return new ReportField
                {
                    FieldName = token.ToString(),
                    DisplayName = ReportFieldMapper.GetDisplayName(token)
                };
            }

            public static ReportField From(ReportFieldsDataSet.CustomFieldNameRow row)
            {
                return new ReportField
                {
                    FieldName = row.Name,
                    DisplayName = row.Name
                };
            }
        }

        #endregion
    }
}