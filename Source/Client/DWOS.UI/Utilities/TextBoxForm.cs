using System;
using System.Windows.Forms;
using DWOS.Utilities.Validation;

namespace DWOS.UI.Utilities
{
    public partial class TextBoxForm : Form
    {
        public TextBoxForm()
        {
            InitializeComponent();
            Validator = new ValidatorManager();
        }

        public ValidatorManager Validator { get; set; }

        public void MakeMultiline(int formHeight, int formWidth)
        {
            this.FormTextBox.Multiline = true;
            this.Height = formHeight;
            this.Width = formWidth;
        }

        private void TextBoxForm_Load(object sender, EventArgs e)
        {
            this.FormTextBox.Focus();
            this.FormTextBox.Select();
        }

        private void TextBoxForm_Shown(object sender, EventArgs e)
        {
            this.FormTextBox.Focus();
            this.FormTextBox.Select();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(Validator != null)
            {
                if(!Validator.ValidateControls())
                    return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}