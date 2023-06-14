using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Service.Licensing;
using DWOS.Shared;

namespace DWOS.Service
{
    public partial class ServerConnection : Form
    {
        public ServerConnection()
        {
            InitializeComponent();
        }
        
        private void FindServer()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var dc = new DiscoveryClient(new UdpDiscoveryEndpoint());
                var fc = new FindCriteria(typeof(Licensing.ILicenseService));
                fc.MaxResults = 1; //limited to speed up the search [TBD]
                fc.Duration = TimeSpan.FromSeconds(1);
                FindResponse fr = dc.Find(fc);

                // here is the really nasty part
                // i am just returning the first channel, but it may not work.
                // you have to do some logic to decide which uri to use from the discovered uris
                // for example, you may discover "127.0.0.1", but that one is obviously useless.
                // also, catch exceptions when no endpoints are found and try again.
                if (fr.Endpoints.Count > 0)
                {
                    var endPoint = fr.Endpoints.FirstOrDefault(a => !a.Address.Uri.IsLoopback) ?? fr.Endpoints.FirstOrDefault();
                    txtServerAddress.Text = endPoint.Address.Uri.Host;
                    txtPort.Text = endPoint.Address.Uri.Port.ToString();
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error finding server automatically.");
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private bool ValidateServer()
        {
            try
            {
                var client = new LicenseServiceClient(new NetTcpBinding(SecurityMode.None, false), new EndpointAddress("net.tcp://" + this.txtServerAddress.Text + ":" + this.txtPort.Text + "/LicenseService"));
                var response = client.GetApplicationInfo(new GetApplicationInfoRequest());
                var appInfo = response.GetApplicationInfoResult;
                client.Close();

                //if not on same version then do allow connection
                if (!Program.IsClientServerConnectionValid(About.ApplicationVersion, appInfo))
                {
                    var minimumClientVersion = String.IsNullOrWhiteSpace(appInfo.MinimumClientVersion) ? appInfo.ServerVersion : appInfo.MinimumClientVersion;
                    MessageBox.Show("You do not have the correct client version of DWOS. Please install client version " + minimumClientVersion + ".", About.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                
                return true;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Info(exc, "Error validating server.");
                return false;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(txtPort.Text == null)
            {
                MessageBox.Show("Port required.", About.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
             
            if(txtServerAddress.Text == null)
            {
                MessageBox.Show("Server name is required.", About.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ValidateServer())
            {
                UserSettings.Default.ServerAddress = txtServerAddress.Text;
                UserSettings.Default.ServerPort = Convert.ToInt32(this.txtPort.Text);
                UserSettings.Default.Save();
                
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else
                MessageBox.Show("Unable to connect to the server. Please ensure that server settings are correct.", About.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            FindServer();
        }
    }
}
