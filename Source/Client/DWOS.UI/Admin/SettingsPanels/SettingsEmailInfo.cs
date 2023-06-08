using System;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Shared;
using DWOS.UI.Utilities;
using NLog;
using nsoftware.IPWorks;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsEmailInfo: UserControl, ISettingsPanel
    {
        #region Fields

        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public string PanelKey => "Email";

        #endregion

        #region Methods
        public bool CanDock
        {
            get { return true; }
        }
        public SettingsEmailInfo()
        {
            InitializeComponent();
        }

        public bool Editable => SecurityManager.Current.IsInRole("ApplicationSettings.Edit");

        public void LoadData()
        {
            Enabled = Editable;

            LoadAuthTypes();
            LoadSslStartModes();

            txtFromAddress.Text = ApplicationSettings.Current.EmailFromAddress;
            txtServerAddress.Text = ApplicationSettings.Current.EmailSMTPServer;
            txtPort.Text = ApplicationSettings.Current.EmailPort;
            txtUserName.Text = ApplicationSettings.Current.EmailUserName;
            txtPassword.Text = ApplicationSettings.Current.EmailPassword;

            // Authentication
            var authType = ApplicationSettings.Current.EmailAuthentication;
            Enum.TryParse(authType, out HtmlmailerAuthMechanisms authMech);
            
            var selectedItem = cboAuthType.FindItemByValue<HtmlmailerAuthMechanisms>(v => v == authMech);
            if(selectedItem != null)
                cboAuthType.SelectedItem = selectedItem;
            else
                cboAuthType.SelectedIndex = 0;

            // SSL Start Mode
            Enum.TryParse(ApplicationSettings.Current.EmailSslStartMode, out HtmlmailerSSLStartModes sslStartMode);

            var sslItem = cboSslStartMode.FindItemByValue<HtmlmailerSSLStartModes>(v => v == sslStartMode);

            if (sslItem != null)
            {
                cboSslStartMode.SelectedItem = sslItem;
            }
            else
            {
                cboSslStartMode.SelectedIndex = 0;
            }
        }

        public void SaveData()
        {
            ApplicationSettings.Current.EmailFromAddress = txtFromAddress.Text;
            ApplicationSettings.Current.EmailSMTPServer = txtServerAddress.Text;
            ApplicationSettings.Current.EmailPort = txtPort.Text;
            ApplicationSettings.Current.EmailUserName = txtUserName.Text;
            ApplicationSettings.Current.EmailPassword = txtPassword.Text;
            ApplicationSettings.Current.EmailAuthentication = cboAuthType.SelectedItem?.DataValue.ToString() ?? HtmlmailerAuthMechanisms.amUserPassword.ToString();
            ApplicationSettings.Current.EmailSslStartMode = cboSslStartMode.SelectedItem?.DataValue.ToString() ?? HtmlmailerSSLStartModes.sslAutomatic.ToString();
        }

        private void LoadAuthTypes()
        {
            cboAuthType.Items.Clear();

            cboAuthType.Items.Add(HtmlmailerAuthMechanisms.amNTLM, "NTLM (Windows)");
            var item = cboAuthType.Items.Add(HtmlmailerAuthMechanisms.amUserPassword, "User/Password");
            cboAuthType.Items.Add(HtmlmailerAuthMechanisms.amKerberos, "Kerberos (Windows)");
            cboAuthType.Items.Add(HtmlmailerAuthMechanisms.amCRAMMD5, "CRAM-MD5");
            cboAuthType.Items.Add(HtmlmailerAuthMechanisms.amSASLPlain, "SASL Plain");
            cboAuthType.Items.Add(HtmlmailerAuthMechanisms.amXOAUTH2, "XOAuth2");

            cboAuthType.SelectedItem = item;
        }

        private void LoadSslStartModes()
        {
            cboSslStartMode.Items.Clear();
            cboSslStartMode.Items.Add(HtmlmailerSSLStartModes.sslAutomatic, "Automatic");
            cboSslStartMode.Items.Add(HtmlmailerSSLStartModes.sslExplicit, "Explicit");
            cboSslStartMode.Items.Add(HtmlmailerSSLStartModes.sslImplicit, "Implicit");
            cboSslStartMode.Items.Add(HtmlmailerSSLStartModes.sslNone, "None");
        }

        #endregion

        #region Events

        private void BtnTest_Click(object sender, EventArgs e)
        {
            try
            {
                btnTest.Enabled = false;

                if(htmlmailer1.Connected)
                    htmlmailer1.Disconnect();

                htmlmailer1.User = txtUserName.Text;
                htmlmailer1.Password = txtPassword.Text;
                htmlmailer1.MailServer = txtServerAddress.Text;
                htmlmailer1.AuthMechanism = (HtmlmailerAuthMechanisms?) cboAuthType.SelectedItem?.DataValue ?? HtmlmailerAuthMechanisms.amUserPassword;
                htmlmailer1.SSLStartMode = (HtmlmailerSSLStartModes?)cboSslStartMode.SelectedItem?.DataValue ?? HtmlmailerSSLStartModes.sslAutomatic;

                if(int.TryParse(txtPort.Text, out var port))
                    htmlmailer1.MailPort = port;

                txtOutput.Clear();
                htmlmailer1.Connect();
            }
            catch (Exception exc)
            {
                _log.Warn(exc, "Error connecting to email server.");
                ErrorMessageBox.ShowDialog("Error connecting to email server.", exc, false);

                txtOutput.AppendText("Error connecting to email server\r\n");
                txtOutput.ScrollToCaret();
            }
            finally
            {
                btnTest.Enabled = true;
            }
        }

        private void Htmlmailer1_OnPITrail(object sender, HtmlmailerPITrailEventArgs e)
        {
            _log.Info("Direction {0}, Message {1}.", e.Direction, e.Message);

            string direction = "Client";
            switch(e.Direction)
            {
                case 1:
                    direction = "Server";
                    break;
                case 2:
                    direction = "Info";
                    break;
            }

            txtOutput.AppendText($"{direction}: {e.Message}{Environment.NewLine}");
            txtOutput.ScrollToCaret();
        }

        private void htmlmailer1_OnSSLStatus(object sender, HtmlmailerSSLStatusEventArgs e)
        {
            var msg = $"SSL: {e.Message}";
            _log.Info(msg);

            txtOutput.AppendText(msg);
            txtOutput.AppendText(Environment.NewLine);
            txtOutput.ScrollToCaret();
        }

        #endregion
    }
}