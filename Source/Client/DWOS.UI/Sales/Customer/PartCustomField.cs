using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Utilities.Validation;
using DWOS.Data.Datasets;
using DWOS.UI.Admin;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinEditors;

namespace DWOS.UI.Sales.Customer
{
    public partial class PartCustomField : DataPanel
    {
        #region Properties

        public CustomersDataset Dataset
        {
            get { return _dataset as CustomersDataset; }
            set { _dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.PartLevelCustomField.PartLevelCustomFieldIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public PartCustomField()
        {
            InitializeComponent();
        }

        public void LoadData(CustomersDataset dataset)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.PartLevelCustomField.TableName;

            //bind column to control
            BindValue(txtName, Dataset.PartLevelCustomField.NameColumn.ColumnName);
            BindValue(txtDescription, Dataset.PartLevelCustomField.DescriptionColumn.ColumnName);
            BindValue(chkPrintTraveler, Dataset.PartLevelCustomField.DisplayOnTravelerColumn.ColumnName);
            BindValue(chkPrintCOC, Dataset.PartLevelCustomField.DisplayOnCOCColumn.ColumnName);
            BindValue(chkVisible, Dataset.PartLevelCustomField.IsVisibleColumn.ColumnName);
            BindValue(txtDefaultValue, Dataset.PartLevelCustomField.DefaultValueColumn.ColumnName);

            BindValue(cboList, Dataset.Lists.ListIDColumn.ColumnName, true);
            BindList(cboList, Dataset.Lists, Dataset.Lists.ListIDColumn.ColumnName, Dataset.Lists.NameColumn.ColumnName);

            _panelLoaded = true;
        }

        public override void AddValidators(ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(txtName, "Name is required.") {DefaultValue = "New Custom Field"}, errProvider));
        }

        #endregion

        #region Events

        private void cboList_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                var currentField = CurrentRecord as CustomersDataset.PartLevelCustomFieldRow;

                if (e.Button.Key == "Delete")
                {
                    currentField?.SetListIDNull();
                }
                else if (e.Button.Key == "ShowListDialog")
                {
                    var listIds = Dataset.PartLevelCustomField
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
                            Dataset.EnforceConstraints = false;

                            using (var taLists = new Data.Datasets.CustomersDatasetTableAdapters.ListsTableAdapter())
                            {
                                taLists.Fill(Dataset.Lists);
                            }

                            Dataset.EnforceConstraints = true;
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
    }
}