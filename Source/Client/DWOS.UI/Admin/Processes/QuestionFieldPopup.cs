using System;
using System.Data;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;

namespace DWOS.UI.Admin.Processes
{
    public partial class QuestionFieldPopup : UserControl
    {
        #region Events

        public event EventHandler OkClick;
        public event EventHandler CancelClick;

        #endregion

        #region Properties

        public ProcessesDataset.CustomFieldTokenDataTable TokenTable { get; private set; }

        public QuestionField Field { get; private set; }

        public string SelectedToken => cboToken.Value?.ToString();

        public bool IsFieldRemoved { get; set; }

        #endregion

        #region Methods

        public QuestionFieldPopup()
        {
            InitializeComponent();
        }

        public void Setup(ProcessesDataset.CustomFieldTokenDataTable dtToken)
        {
            if (dtToken == null)
            {
                throw new ArgumentNullException(nameof(dtToken));
            }

            TokenTable = dtToken;
        }

        public void LoadData(QuestionField field, string currentField)
        {
            Field = field;
            grpPopup.Text = string.IsNullOrEmpty(currentField)
                ? "Token"
                : $"Token - {currentField}";

            IsFieldRemoved = false;
            cboToken.Items.Clear();

            if (!string.IsNullOrEmpty(currentField))
            {
                cboToken.Items.Add(currentField);
            }

            var tokens = TokenTable
                .Where(t => !t.IsTokenNameNull() && !string.IsNullOrEmpty(t.TokenName))
                .OrderBy(t => t.TokenName);

            // Grab from db
            foreach (var tokenRow in tokens)
            {
                if (tokenRow.TokenName == currentField)
                {
                    continue;
                }

                cboToken.Items.Add(tokenRow.TokenName);
            }

            btnOK.Enabled = cboToken.Items.Count > 0;
            cboToken.Enabled = cboToken.Items.Count > 0;

            if (cboToken.Items.Count == 0)
            {
                cboToken.Items.Add("No Tokens Found");
            }

            cboToken.SelectedIndex = 0;
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            OkClick?.Invoke(this, e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelClick?.Invoke(this, e);
        }

        private void cboToken_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key != "Delete")
            {
                return;
            }

            cboToken.Enabled = false;
            cboToken.Items.Clear();
            cboToken.Items.Add("(Removed)");
            cboToken.SelectedIndex = 0;

            IsFieldRemoved = true;
        }

        #endregion
    }
}