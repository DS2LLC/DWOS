using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using nsoftware.InQB;

namespace DWOS.QBExport
{
    /// <summary>
    /// Dialog for adding a list item.
    /// </summary>
    public partial class AddListItem : Form
    {
        #region Properties

        /// <summary>
        /// Gets the account type of the item.
        /// </summary>
        public string ItemAccountType 
        {
            get { return cboAccount.Text; }
        }

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string ItemName
        {
            get { return txtItemName.Text; }
            set { txtItemName.Text = value; }
        }

        /// <summary>
        /// Gets the type of the item.
        /// </summary>
        public ItemItemTypes ItemType
        {
            get
            {
                return (ItemItemTypes)cboItemType.SelectedItem.DataValue;
            }
        }

        /// <summary>
        /// Gets or sets the description of the item.
        /// </summary>
        public string ItemDescription
        {
            get { return txtDescription.Text; }
            set { txtDescription.Text = value; }
        }
        
        #endregion

        #region Methods
        
        public AddListItem()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads data from QuickBooks for the dialog.
        /// </summary>
        public void LoadData()
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
            cboItemType.Items.Add(ItemItemTypes.itService, "Non-inventory Part");
            cboItemType.Items.Add(ItemItemTypes.itService, "Other Charge");
            cboItemType.Items.Add(ItemItemTypes.itService, "Subtotal");
            cboItemType.Items.Add(ItemItemTypes.itService, "Group");
            cboItemType.Items.Add(ItemItemTypes.itService, "Discount");
            cboItemType.Items.Add(ItemItemTypes.itService, "Payment");
            cboItemType.SelectedIndex = 0;
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        #endregion
    }
}
