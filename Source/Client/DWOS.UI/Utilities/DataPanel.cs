using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using NLog;

namespace DWOS.UI
{
    public partial class DataPanel : UserControl, IDataPanel
    {
        #region Fields

        private CommandManager _cmdManager = new CommandManager();
        protected DataSet _dataset;

        private bool _editable = true;
        private List <Control> _editableControls;
        protected Logger _log;

        /// <summary>
        ///     Flag to determine if the panel has been completely loaded properly
        /// </summary>
        protected bool _panelLoaded;

        /// <summary>
        ///     Flag to determine if a record is currently being loaded/bound
        /// </summary>
        protected bool _recordLoading;

        public event Action UpdateCurrentNodeUI;
        public event EventHandler EditableStatusChanged;
        public event EventHandler MovedToNewRecord;
        public event EventHandler MinimumSizeChanged;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the primary key of the binding source underlying data source.
        /// </summary>
        /// <value> The binding source primary key. </value>
        protected virtual string BindingSourcePrimaryKey
        {
            get { return null; }
        }

        /// <summary>
        ///     Gets the currently selected record in the binding source.
        /// </summary>
        /// <value> The current record. </value>
        internal DataRow CurrentRecord
        {
            get
            {
                if (this.Disposing)
                    return null;

                if(!this.bsData.IsBindingSuspended && this.bsData.Current != null)
                    return ((DataRowView) this.bsData.Current).Row;

                return null;
            }
        }

        /// <summary>
        ///     Get value if the current record is a new record.
        /// </summary>
        protected bool? IsNewRow
        {
            get
            {
                if(CurrentRecord == null)
                    return null;

                return CurrentRecord.RowState == DataRowState.Added;
            }
        }

        /// <summary>
        ///     Gets the editable controls that are controlled by the Editable flag.
        /// </summary>
        /// <value> The editable controls. </value>
        protected List <Control> EditableControls
        {
            get { return this._editableControls; }
        }

        internal CommandManager Commands
        {
            get { return this._cmdManager; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="DataPanel" /> is editable.
        ///     Updating this will cause all editable controls enabled property to be set to a matching value.
        /// </summary>
        /// <value> <c>true</c> if editable; otherwise, <c>false</c> . </value>
        public bool Editable
        {
            get { return this._editable; }
            set
            {
                bool original = this._editable;

                this._editable = value;
                this.picLockImage.Visible = !this._editable;

                OnEditableStatusChange(this._editable);

                if(original != value && EditableStatusChanged != null)
                    EditableStatusChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Gets or Sets if this is the active panel in the editor or not.
        /// </summary>
        public bool IsActivePanel { get; set; }

        bool IDataPanel.Visible
        {
            get { return Visible; }
            set { Visible = value; }
        }

        bool IDataPanel.Enabled
        {
            get { return Enabled; }
            set { Enabled = value; }
        }

        UserControl IDataPanel.Control
        {
            get { return this; }
        }

        protected Size? FirstMinimumSize
        {
            get;
            set;
        }

        public override Size MinimumSize
        {
            get
            {
                return base.MinimumSize;
            }

            set
            {
                if (base.MinimumSize == value)
                {
                    return;
                }

                base.MinimumSize = value;
                MinimumSizeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Methods

        public DataPanel()
        {
            InitializeComponent();

            if(DesignMode)
                return;

            this._log = LogManager.GetCurrentClassLogger();

            this.bsData.Filter = "1 = 0"; //set bogus filter to prevent any data from being loaded
        }

        /// <summary>
        ///     Ends the editing of the panels binding source. Required prior to saving data.
        /// </summary>
        public virtual void EndEditing()
        {
            this._log.Debug("Ending edits for " + Name);
            this.bsData.EndEdit();

            //Added to fix the datarowview not being taken out of edit mode even when the EndEdit on the binding source was called
            //  Error arose from settings a required field with a FK to -1 and not moving to another bsData record, would still pass the -1 to Database
            if(this.bsData.Current is DataRowView && ((DataRowView) this.bsData.Current).IsEdit)
                ((DataRowView) this.bsData.Current).EndEdit();
        }

        public virtual void CancelEdits()
        {
            this._log.Debug("Canceling edits for " + Name);
            this.bsData.CancelEdit();
        }

        /// <summary>
        ///     Moves to record the record based on the record id. Returns success.
        /// </summary>
        /// <param name="id"> The id. </param>
        /// <returns> </returns>
        public bool MoveToRecord(object id)
        {
            //notify before moving to new record
            BeforeMoveToNewRecord(id);

            this._recordLoading = true;

            if(this.bsData.IsBindingSuspended)
                this.bsData.ResumeBinding(); //ensure nobody suspended binding

            //NOTE: Changed to use the filter method instead of position method, can't mix using the two
            this.bsData.Filter = BindingSourcePrimaryKey + " = " + id;

            Debug.Assert(this.bsData.Position >= 0, "Unable to find correct data item?");

            //force controls to read values
            this.bsData.ResetCurrentItem();

            this._recordLoading = false;

            //notify after moved to new record
            AfterMovedToNewRecord(id);

            MovedToNewRecord?.Invoke(this, EventArgs.Empty);

            //return success
            return this.bsData.Position >= 0;
        }

        /// <summary>
        ///     Adds the validators to the validator manager and error provider of the main page.
        /// </summary>
        /// <param name="manager"> The manager. </param>
        /// <param name="errProvider"> The err provider. </param>
        public virtual void AddValidators(DWOS.Utilities.Validation.ValidatorManager manager, ErrorProvider errProvider) { }

        /// <summary>
        ///     Called when moving to new record, but before the record is moved to.
        /// </summary>
        /// <param name="id"> The id. </param>
        protected virtual void BeforeMoveToNewRecord(object id) { }

        /// <summary>
        ///     Called when binding source moved to new record.
        /// </summary>
        /// <param name="id"> The id. </param>
        protected virtual void AfterMovedToNewRecord(object id) { }

        /// <summary>
        ///     Called when editable status changes.
        /// </summary>
        /// <param name="editable"> if set to <c>true</c> [editable]. </param>
        protected virtual void OnEditableStatusChange(bool editable)
        {
            if (_editableControls == null)
            {
                return;
            }

            SuspendLayout();

            foreach(Control item in this._editableControls)
                item.Enabled = editable;

            ResumeLayout();
        }

        protected virtual void OnDispose()
        {
            try
            {
                LogManager.GetCurrentClassLogger().Debug("On data panel disposing: " + Name);

                _dataset = null;

                _editableControls?.Clear();
                _editableControls = null;

                bsData?.Dispose();
                bsData = null;

                _cmdManager?.Dispose();
                _cmdManager = null;
                _log = null;
            }
            catch(Exception exc)
            {
                const string errorMsg = "Error disposing panel.";
                _log?.Error(exc, errorMsg);
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            // Initialize FirstMinimumSize
            // May not be initialized prior to becoming visible.
            if (!FirstMinimumSize.HasValue && Visible)
            {
                FirstMinimumSize = MinimumSize;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            // Fix for 'tvwRoles does not properly get resized for certain layouts' bug
            // Explanation here:
            // http://stackoverflow.com/questions/7583699/winforms-application-shows-differently-on-different-computers-when-using-infragi
            try
            {
                if (Handle != null)
                {
                    BeginInvoke((Action)(() => base.OnSizeChanged(e)));
                }
                else
                {
                    base.OnSizeChanged(e);
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Unable to invoke OnSizeChanged(e).");
            }
        }

        /// <summary>
        ///     Called when something changed that requires the node's text to update.
        /// </summary>
        protected void OnUpdateCurrentNodeUI()
        {
            UpdateCurrentNodeUI?.Invoke();
        }

        private void ResizeLockImage()
        {
            if(!DesignMode)
                this.picLockImage.Location = new Point(Right - 30, 2);
        }

        #region Binding

        /// <summary>
        ///     Binds the column from the binding source to the control.
        /// </summary>
        /// <param name="control"> The control. </param>
        /// <param name="columnName"> Name of the column. </param>
        /// <param name="updateMode">Binding update mode</param>
        protected void BindValue(Control control, string columnName,
            DataSourceUpdateMode updateMode = DataSourceUpdateMode.OnValidation)
        {
            BindValue(control, ControlUtilities.GetBindingProperty(control), columnName, false, updateMode);
        }

        /// <summary>
        ///     Binds the control to the column.
        /// </summary>
        /// <param name="control"> The control. </param>
        /// <param name="columnName"> Name of the column. </param>
        /// <param name="editableSynch"> if set to <c>true</c> than control will be synchronized with the editability of the current record. </param>
        /// <param name="updateMode">Binding update mode</param>
        protected void BindValue(Control control, string columnName, bool editableSynch,
            DataSourceUpdateMode updateMode = DataSourceUpdateMode.OnValidation)
        {
            BindValue(control, ControlUtilities.GetBindingProperty(control), columnName, editableSynch, updateMode);
        }

        /// <summary>
        ///     Binds the control to the column using a property name.
        /// </summary>
        /// <param name="control"> The control. </param>
        /// <param name="propertyName"> Property name to use for binding. </param>
        /// <param name="columnName"> Name of the column. </param>
        /// <param name="updateMode">Binding update mode</param>
        protected void BindValue(Control control, string propertyName, string columnName,
            DataSourceUpdateMode updateMode = DataSourceUpdateMode.OnValidation)
        {
            BindValue(control, propertyName, columnName, false, updateMode);
        }

        /// <summary>
        ///     Binds the control to the column using a property name.
        /// </summary>
        /// <param name="control"> The control. </param>
        /// <param name="propertyName"> Property name to use for binding. </param>
        /// <param name="columnName"> Name of the column. </param>
        /// <param name="editableSynch"> if set to <c>true</c> than control will be synchronized with the editability of the current record. </param>
        /// <param name="updateMode">Binding update mode</param>
        protected void BindValue(Control control, string propertyName, string columnName, bool editableSynch, DataSourceUpdateMode updateMode)
        {
            if(editableSynch)
            {
                if(this._editableControls == null)
                    this._editableControls = new List <Control>();

                this._editableControls.Add(control);
            }

            var binding = new Binding(propertyName, this.bsData, columnName, true)
            {
                DataSourceUpdateMode = updateMode
            };

            control.DataBindings.Add(binding);

            //update text box max length
            if(control is UltraTextEditor && this._dataset.Tables[this.bsData.DataMember].Columns[columnName].DataType.Name == "String" && this._dataset.Tables[this.bsData.DataMember].Columns[columnName].MaxLength > 0)
                ((UltraTextEditor) control).MaxLength = this._dataset.Tables[this.bsData.DataMember].Columns[columnName].MaxLength;
        }

        /// <summary>
        ///     Bind a list to a combobox. Fills list with all the values of the datatable.
        /// </summary>
        /// <param name="cbo"> The cbo. </param>
        /// <param name="dt"> The dt. </param>
        /// <param name="valueMember"> The value member. </param>
        /// <param name="displayMember"> The display member. </param>
        protected void BindList(UltraComboEditor cbo, DataTable dt, string valueMember, string displayMember)
        {
            cbo.DataSource = new DataView(dt);
            cbo.DisplayMember = displayMember;
            cbo.ValueMember = valueMember;
        }

        /// <summary>
        /// Bind a list to a combobox. Fills list with all the values of the dataview.
        /// </summary>
        /// <param name="cbo"> The cbo. </param>
        /// <param name="dv"> The dv. </param>
        /// <param name="valueMember"> The value member. </param>
        /// <param name="displayMember"> The display member. </param>
        protected void BindList(UltraComboEditor cbo, DataView dv, string valueMember, string displayMember)
        {
            cbo.DataSource = dv;
            cbo.DisplayMember = displayMember;
            cbo.ValueMember = valueMember;
        }

        /// <summary>
        ///     Binds the list as distinct values of the table.
        /// </summary>
        /// <param name="cbo"> The cbo. </param>
        /// <param name="dt"> The dt. </param>
        /// <param name="displayMember"> The display member. </param>
        protected void BindListAsDistinctValues(UltraComboEditor cbo, DataTable dt, string displayMember)
        {
            cbo.DataSource = dt.AsDataView().ToTable(dt.TableName + "_Distinct_" + displayMember, true, displayMember);
            cbo.DisplayMember = displayMember;
            cbo.ValueMember = displayMember;
        }

        /// <summary>
        ///     Updates the row filter of the datasource of the combobox so only the required records are shown.
        /// </summary>
        /// <param name="cbo"> The cbo. </param>
        /// <param name="filter"> The filter. </param>
        protected void UpdateFilter(UltraComboEditor cbo, string filter)
        {
            if (!this._panelLoaded)
            {
                return;
            }

            var dv = cbo.DataSource as DataView;

            if (dv == null)
                throw new InvalidOperationException("Data source is not bound to a DataView.");

            if (dv.RowFilter != filter)
            {
                dv.RowFilter = filter;
                cbo.DataBind();

                //reread value from data source to ensure correct item is selected in list
                if (cbo.DataBindings.Count > 0)
                    cbo.DataBindings[0].ReadValue();

                this._log.Info($"Updating {cbo.Name} filter to '{filter}'");
            }
        }

        #endregion

        #endregion

        #region Events

        private void DataPanel_Resize(object sender, EventArgs e) { ResizeLockImage(); }

        private void DataPanel_Load(object sender, EventArgs e) { ResizeLockImage(); }

        #endregion

        #region Nested type: LinkedDataTableDataFilter

        protected class LinkedDataTableDataFilter : IEditorDataFilter
        {
            #region Fields

            private readonly string _displayColumn;
            private readonly Type _primaryKeyDataType;
            private readonly DataTable _table;

            #endregion

            #region Methods

            public LinkedDataTableDataFilter(DataTable dt, string displayColumnName)
            {
                this._table = dt;
                this._displayColumn = displayColumnName;

                if(this._table.PrimaryKey.Length > 0)
                    this._primaryKeyDataType = this._table.PrimaryKey[0].DataType;
            }

            private bool IsValidPrimaryKey(object value)
            {
                if (this._primaryKeyDataType.Name != "Int32")
                {
                    return false;
                }

                int i;
                return int.TryParse(value.ToString(), out i);
            }

            #endregion

            #region IEditorDataFilter Members

            public object Convert(EditorDataFilterConvertArgs conversionArgs)
            {
                try
                {
                    switch(conversionArgs.Direction)
                    {
                        case ConversionDirection.EditorToOwner:
                            if(conversionArgs.Value != null && conversionArgs.Value.ToString() == "<None>")
                            {
                                conversionArgs.Handled = true;
                                return null;
                            }
                            if(conversionArgs.Value != null)
                            {
                                conversionArgs.Handled = true;
                                return conversionArgs.Value.ToString();
                            }
                            break;
                        case ConversionDirection.OwnerToEditor:
                            if(conversionArgs.Value != null && !string.IsNullOrEmpty(conversionArgs.Value.ToString()))
                            {
                                if(conversionArgs.Value.ToString() == "<None>")
                                {
                                    conversionArgs.Handled = true;
                                    return null;
                                }

                                if(IsValidPrimaryKey(conversionArgs.Value))
                                {
                                    DataRow dr = this._table.Rows.Find(conversionArgs.Value);

                                    if(dr != null && !dr.IsNull(this._displayColumn))
                                    {
                                        conversionArgs.Handled = true;
                                        return dr[this._displayColumn].ToString();
                                    }
                                }
                                else
                                    Debug.WriteLine("Value to be converted is of wrong type: " + conversionArgs.Value);
                            }

                            break;
                    }

                    if(conversionArgs.Value != null && conversionArgs.Value.ToString() != "<None>" && !string.IsNullOrEmpty(conversionArgs.Value.ToString()))
                    {
                        conversionArgs.Handled = true;
                        return conversionArgs.Value;
                    }
                    conversionArgs.Handled = true;
                    return "<None>";
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error converting name of field from: " + conversionArgs.Value;
                    Debug.WriteLine(errorMsg + " --- " + exc);
                    return null;
                }
            }

            #endregion
        }

        #endregion
    }
}