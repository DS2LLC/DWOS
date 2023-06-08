using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Utilities.Validation;
using DWOS.Data.Datasets;
using DWOS.Data;
using DWOS.UI.Admin;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinEditors;
using NLog;

namespace DWOS.UI.Sales.Customer
{
    public partial class CustomField : DataPanel
    {
        #region Properties

        public CustomersDataset Dataset
        {
            get { return base._dataset as CustomersDataset; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.CustomField.CustomFieldIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public CustomField() { InitializeComponent(); }

        public void LoadData(CustomersDataset dataset)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.CustomField.TableName;

            //bind column to control
            base.BindValue(this.txtName, Dataset.CustomField.NameColumn.ColumnName);
            base.BindValue(this.txtDescription, Dataset.CustomField.DescriptionColumn.ColumnName);
            base.BindValue(this.chkPrintTraveler, Dataset.CustomField.DisplayOnTravelerColumn.ColumnName);
            base.BindValue(this.chkPrintCOC, Dataset.CustomField.DisplayOnCOCColumn.ColumnName);
            base.BindValue(this.chkProcessUnique, Dataset.CustomField.ProcessUniqueColumn.ColumnName);
            base.BindValue(this.chkRequired, Dataset.CustomField.RequiredColumn.ColumnName);
            base.BindValue(this.chkVisible, Dataset.CustomField.IsVisibleColumn.ColumnName);
            base.BindValue(this.txtTokenName, Dataset.CustomField.TokenNameColumn.ColumnName);
            BindValue(txtDefaultValue, Dataset.CustomField.DefaultValueColumn.ColumnName);

            BindValue(cboList, Dataset.Lists.ListIDColumn.ColumnName, true);
            BindList(cboList, Dataset.Lists, Dataset.Lists.ListIDColumn.ColumnName, Dataset.Lists.NameColumn.ColumnName);

            base._panelLoaded = true;
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            base.BeforeMoveToNewRecord(id);

            chkVisible.CheckedChanged -= chkVisible_CheckedChanged;
            chkRequired.CheckedChanged -= chkRequired_CheckedChanged;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            chkVisible.CheckedChanged += chkVisible_CheckedChanged;
            chkRequired.CheckedChanged += chkRequired_CheckedChanged;

            base.AfterMovedToNewRecord(id);
        }

        public override void AddValidators(ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtName, "Name is required.") {DefaultValue = "New Custom Field"}, errProvider));
            manager.Add(new ImageDisplayValidator(new TokenNameValiditor(this.txtTokenName), errProvider));
        }

        public CustomersDataset.CustomFieldRow AddRow(int customerID)
        {
            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as CustomersDataset.CustomFieldRow;
            cr.CustomerID = customerID;
            cr.Name = "New Custom Field";
            cr.DisplayOnCOC = false;
            cr.DisplayOnTraveler = false;
            cr.Required = false;
            cr.ProcessUnique = false;
            cr.IsVisible = true;

            return cr;
        }

        #endregion

        #region Events

        private void chkRequired_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkRequired.Checked && !chkVisible.Checked)
                {
                    chkVisible.Checked = true;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error checking/unchecking Required field.");
            }
        }

        private void chkVisible_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!chkVisible.Checked && chkRequired.Checked)
                {
                    chkRequired.Checked = false;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error checking/unchecking Visible field.");
            }
        }

        private void cboList_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                var currentField = CurrentRecord as CustomersDataset.CustomFieldRow;

                if (e.Button.Key == "Delete")
                {
                    currentField?.SetListIDNull();
                }
                else if (e.Button.Key == "ShowListDialog")
                {
                    var listIds = Dataset.CustomField
                        .Where(s => s.IsValidState() && !s.IsListIDNull())
                        .Select(s => s.ListID)
                        .ToList();

                    listIds.Add(cboList.Value as int? ?? 0);

                    using (var editor = new ListEditor())
                    {
                        editor.DoNotAllowDeletionOf(listIds);
                        if (editor.ShowDialog(this) == DialogResult.OK)
                        {
                            // EnforceConstraints must be false before clearing
                            // the list table.
                            this.Dataset.EnforceConstraints = false;

                            using (var taLists = new Data.Datasets.CustomersDatasetTableAdapters.ListsTableAdapter())
                            {
                                taLists.Fill(this.Dataset.Lists);
                            }

                            this.Dataset.EnforceConstraints = true;
                            cboList.DataBind();

                            //attempt to select last selected list
                            var listId = editor.SelectedListID;

                            if (listId >= 0)
                            {
                                var vli = cboList.FindItemByValue<int>(v => v == listId);

                                if (vli != null)
                                {
                                    cboList.SelectedItem = vli;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error clicking list button");
            }
        }

        #endregion

        #region TokenNameValidator

        private class TokenNameValiditor : ControlValidatorBase
        {
            #region Methods

            public TokenNameValiditor(UltraTextEditor control) : base(control)
            {
                IsRequired = false;
            }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                try
                {
                    if(Control != null && Control.Enabled)
                    {
                        if(!String.IsNullOrWhiteSpace(Control.Text))
                        {
                            var value = Control.Text;
                            Control.Text = Control.Text.ToPartMarkingString();
                        }
                    }

                    //passed
                    e.Cancel = false;
                    FireAfterValidation(true, "");
                }
                catch(Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error validating part mark token.");
                }
            }

            #endregion
        }

        #endregion
    }
}