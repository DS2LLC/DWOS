using System;
using System.Windows.Forms;

namespace DWOS.QBExport
{
    /// <summary>
    /// Dialog for adding terms.
    /// </summary>
    public partial class AddTerms : Form
    {
        #region Properties

        /// <summary>
        /// Gets the due date for this instance.
        /// </summary>
        public int DueDate => Convert.ToInt32(numDueDate.Value);

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="AddTerms"/> class.
        /// </summary>
        public AddTerms()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the dialog.
        /// </summary>
        /// <param name="termName"></param>
        public void Initialize(string termName)
        {
            txtTerms.Text = termName;
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (DueDate == 0)
            {
                // Cannot currently use 0 as a due date - see #16397
                MessageBox.Show(this,
                    @"Cannot currently set the due date as 0. Please use another due date and set it to 0 within QuickBooks.",
                    @"Add Standard Terms",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else
            {
                DialogResult = DialogResult.OK;
            }
        }

        #endregion
    }
}
