using System;
using System.ComponentModel;
using System.Windows.Forms;
using DWOS.Data.Datasets.QuoteDataSetTableAdapters;
using DWOS.Shared;
using Infragistics.Win.SupportDialogs.FormattedTextEditor;

namespace DWOS.UI.Admin
{
    public partial class TermsManager: Form
    {
        public TermsManager()
        {
            this.InitializeComponent();
        }

        private void LoadData()
        {
            this.dsQuotes.EnforceConstraints = false;

            using(var ta = new d_TermsTableAdapter())
            {
                ta.Fill(this.dsQuotes.d_Terms);
            }
        }

        private bool SaveData()
        {
            try
            {
                this.grdManufacturer.UpdateData();

                using(var ta = new d_TermsTableAdapter())
                {
                    ta.Update(this.dsQuotes.d_Terms);
                }

                return true;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error saving data.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);

                return false;
            }
        }

        private void ManufacturerManager_Load(object sender, EventArgs e)
        {
            try
            {
                this.LoadData();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error loading Manufacturer Manager.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if(this.SaveData())
                    Close();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error ";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void grdManufacturer_BeforeEnterEditMode(object sender, CancelEventArgs e)
        {
            if (grdManufacturer.ActiveCell.Column.Key != "Terms")
            {
                return;
            }

            var editorForm = new FormattedTextUIEditorForm
            {
                Value = grdManufacturer.ActiveCell.Value,
                TopMost = true
            };

            var dresult = editorForm.ShowDialog();

            if (dresult == DialogResult.OK)
            {
                grdManufacturer.ActiveCell.Value = editorForm.Value;
            }

            e.Cancel = true;
        }
    }
}