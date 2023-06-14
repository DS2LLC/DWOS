using System;
using System.Windows.Forms;
using DWOS.Data;
using NLog;

namespace DWOS.Server.Admin.Tasks
{
    /// <summary>
    /// Prompts the user for portal site settings and saves them to
    /// <see cref="ServerSettings"/>.
    /// </summary>
    public partial class WebServerConfiguration : Form
    {
        #region Fields

        const string WEB_SITE_NAME = "Default Web Site";
        const string WEB_APPLICATION_NAME = "dwos";
        
        #endregion

        #region Methods
      
        public WebServerConfiguration()
        {
            this.InitializeComponent();
        }

        private void LoadSettings()
        {
            txtSite.Text = WEB_SITE_NAME;
            txtApplication.Text = WEB_APPLICATION_NAME;
            txtServerName.Text = "localhost";

            if (!String.IsNullOrWhiteSpace(ServerSettings.Default.CustomerPortalServer))
                txtServerName.Text = ServerSettings.Default.CustomerPortalServer;

            var webSite = "";
            var webApp = "";

            if(ServerSettings.Default.GetCustomerPortalWebSiteAndApp(out webSite, out webApp))
            { 
                txtSite.Text = webSite;
                txtApplication.Text = webApp;
            }
        }

        private void SaveSettings()
        {
            ServerSettings.Default.CustomerPortalServer = this.txtServerName.Text;
            ServerSettings.Default.CustomerPortalWebSite = this.txtSite.Text + "\\" + txtApplication.Text;
            ServerSettings.Default.Save();
        }

        #endregion

        #region Events

        private void ServerConfiguration_Load(object sender, EventArgs e) { LoadSettings(); }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveSettings();
            DialogResult = DialogResult.OK;
            Close();
        }

        #endregion
    }
}