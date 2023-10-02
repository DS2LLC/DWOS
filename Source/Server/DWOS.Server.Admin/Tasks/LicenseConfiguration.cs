using System;
using System.Windows.Forms;
using DWOS.Data;
//using DWOS.Server.Admin.LicenseActivation;
using DWOS.Shared;
using NLog;

namespace DWOS.Server.Admin.Tasks
{
    /// <summary>
    /// Prompts the user for licensing information and saves it to
    /// <see cref="ServerSettings"/>.
    /// </summary>
    public partial class LicenseConfiguration: Form
    {
        #region Fields

        //private ActivationServiceClient _client;

        #endregion

        #region Methods

        public LicenseConfiguration()
        {
            this.InitializeComponent();
        }

        private void BeginRegistration()
        {
            try
            {
                this.btnOK.Enabled     = false;
                this.btnCancel.Enabled = false;
                Cursor                 = Cursors.WaitCursor;
                this.prgStatus.Visible = true;
                this.txtStatus.Text    = null;

                //if(this._client == null)
                //{
                //    this._client                    = new ActivationServiceClient();
                //    this._client.ActivateCompleted  += this.client_ActivateCompleted;
                //}

                //this.txtStatus.Text = "Beginning activation...";

                //this._client.ActivateAsync(this.txtCustomerKey.Text, this.txtFingerprint.Text, this.txtLicenseKey.Text);
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error registering.", exc);
                this.btnCancel.Enabled = true;
            }
        }

        #endregion

        #region Events

        private void Registration_Load(object sender, EventArgs e)
        {
            this.txtCustomerKey.Text = ServerSettings.Default.CompanyKey;
            this.txtFingerprint.Text = ServerSettings.Default.Fingerprint;
            this.txtLicenseKey.Text = ServerSettings.Default.LicenseKey;
        }

        private void client_ActivateCompleted(object sender)
        {
            try
            {
                //if(e.Error != null)
                //{
                //    this.txtStatus.Text += "\r\nError during activation...";
                //    this.txtStatus.Text += "\r\nError: " + e.Error.Message;
                //    return;
                //}

                //if(!String.IsNullOrEmpty(e.Result.ErrorInformation))
                //{
                //    this.txtStatus.Text += "\r\nError: " + e.Result.ErrorInformation;
                //    return;
                //}

                //if(e.Result.Successful)
                //    this.txtStatus.Text += "\r\nSuccesfully activated!";
                //else
                    this.txtStatus.Text += "\r\nUnable to activate.";
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error during activation.");
            }
            finally
            {
                this.btnOK.Enabled = true;
                this.btnCancel.Enabled = true;
                Cursor = Cursors.Default;
                this.prgStatus.Visible = false;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(this.txtCustomerKey.Text != null)
            {
                //Save the company key and fingerprint
                ServerSettings.Default.CompanyKey = this.txtCustomerKey.Text;
                ServerSettings.Default.Fingerprint = this.txtFingerprint.Text;
                ServerSettings.Default.LicenseKey = this.txtLicenseKey.Text;
                ServerSettings.Default.Save();

                this.BeginRegistration();
            }
        }

        private void Registration_FormClosed(object sender, FormClosedEventArgs e)
        {
            //try
            //{
            //    if(this._client != null)
            //        this._client.Close();

            //    this._client = null;
            //}
            //catch(Exception exc)
            //{
            //    LogManager.GetCurrentClassLogger().Error(exc, "Error closing client.");
            //}
        }

        #endregion
    }
}