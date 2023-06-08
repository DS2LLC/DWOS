using nsoftware.InQB;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DWOS.QBExport
{
    /// <summary>
    /// Dialog for adding a department item.
    /// </summary>
    public partial class AddDepartmentItem : Form
    {
        #region Properties

        /// <summary>
        /// Gets the account type for the department.
        /// </summary>
        public string AccountType
        {
            get
            {
                return cboAccount.Text;
            }
        }

        /// <summary>
        /// Gets the code for the department.
        /// </summary>
        public string AccountingCode
        {
            get
            {
                return txtAccountingCode.Text;
            }
        }

        /// <summary>
        /// Gets the type for the department.
        /// </summary>
        public ItemItemTypes ItemType
        {
            get
            {
                return (ItemItemTypes)cboItemType.SelectedItem.DataValue;
            }
        }

        public string Description
        {
            get
            {
                return txtDescription.Text;
            }
        }

        #endregion

        #region Methods

        public AddDepartmentItem()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the dialog.
        /// </summary>
        /// <param name="departmentName"></param>
        /// <param name="accountingCode"></param>
        public void Initialize(string departmentName, string accountingCode)
        {
            txtDepartmentName.Text = departmentName;

            if (!string.IsNullOrEmpty(accountingCode))
            {
                txtAccountingCode.Text = accountingCode;
                txtAccountingCode.ReadOnly = true;
            }

            LoadData();
        }

        private void LoadData()
        {
            //Load Account Types
            var accountSearch = new Objsearch
            {
                QueryType = ObjsearchQueryTypes.qtAccountSearch
            };
            accountSearch.Search();

            foreach (var result in accountSearch.Results.OrderBy(r => r.ResultName))
            {
                cboAccount.Items.Add(ItemItemTypes.itUnknown, result.ResultName);
            }
            cboAccount.SelectedIndex = 0;

            //Load Item Types
            cboItemType.Items.Add(ItemItemTypes.itService, "Service");
            cboItemType.SelectedIndex = 0;
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Check for existence of accounting code

            if (string.IsNullOrEmpty(txtAccountingCode.Text))
            {
                MessageBox.Show("Please enter an accounting code.");
                return;
            }
            DialogResult = DialogResult.OK;
        }

        #endregion
    }
}
