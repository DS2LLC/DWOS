using DWOS.Data;
using DWOS.Shared;
using DWOS.UI.Licensing;
using DWOS.Utilities.Validation;
using NLog;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Windows.Forms;

namespace DWOS.UI.Utilities
{
    public partial class ServerConnection : Form
    {
        #region Fields

        private ValidatorManager _validators = new ValidatorManager();
        private List <ServerAddressEntry> _servers = new List <ServerAddressEntry>();
        private bool _isServersLoaded;

        #endregion

        #region Properties

        public string ServerAddress
        {
            get
            {
                return this.cboServerName.Text;
            }
        }

        public int ServerPort
        {
            get
            {
                return Convert.ToInt32(this.txtPort.Text);
            }
        }

        #endregion

        #region Methods

        public ServerConnection()
        {
            InitializeComponent();

            this._validators.Add(new ImageDisplayValidator(new TextControlValidator(this.cboServerName, "Server Address required!"), this.errorProvider1));
            this._validators.Add(new ImageDisplayValidator(new TextControlValidator(this.txtPort, "Server Port required!"), this.errorProvider1));
        }

        private void FindServer()
        {
            try
            {
                SetLoading(true);
                
                var dc = new DiscoveryClient(new UdpDiscoveryEndpoint());
                var fc = new FindCriteria(typeof(ILicenseService));
                fc.MaxResults = 5; 
                fc.Duration   = TimeSpan.FromSeconds(3);

                dc.FindCompleted += Server_FindCompleted;
                dc.FindAsync(fc);               
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error finding server automatically.");
            }
        }

        private void SetLoading(bool isLoading)
        {
            if(isLoading)
            {
                Cursor.Current = Cursors.WaitCursor;
                this.Cursor = Cursors.WaitCursor;
                btnOK.Enabled = true;
            }
            else
            {
                Cursor.Current = Cursors.Default;
                this.Cursor = Cursors.Default;
                btnOK.Enabled = true;
            }
        }
        
        private bool ValidateServer()
        {
            const string errorMsg = "Unable to connect to the server. Please ensure that server settings are correct.";
            const string errorTitle = "Connection Error";
            const string errorFooter = "Valid connection to server required.";

            try
            {
                int serverPort;

                if (!int.TryParse(txtPort.Text, out serverPort))
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(errorMsg, errorTitle, errorFooter);
                    return false;
                }

                var serverInfo = new DwosServerInfo()
                {
                    ServerAddress = cboServerName.Text,
                    ServerPort = serverPort
                };

                var client = new LicenseServiceClient(new NetTcpBinding(SecurityMode.None, false), new EndpointAddress(serverInfo.ServerAddressUri));
                var response = client.GetApplicationInfo(new GetApplicationInfoRequest());
                var appInfo = response.GetApplicationInfoResult;
                client.Close();

                //if not on same version then do allow connection
                if(!Program.IsClientServerConnectionValid(About.ApplicationVersion, appInfo))
                {
                    var minimumClientVersion = String.IsNullOrWhiteSpace(appInfo.MinimumClientVersion) ? appInfo.ServerVersion : appInfo.MinimumClientVersion;
                    MessageBox.Show("You do not have the correct client version of DWOS. Please install client version " + minimumClientVersion + ".", About.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                return true;
            }
            catch(Exception exc)
            {
                MessageBoxUtilities.ShowMessageBoxWarn(errorMsg, errorTitle, errorFooter);
                LogManager.GetCurrentClassLogger().Info(exc, "Error validating server.");
                return false;
            }
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (this._validators.ValidateControls() && ValidateServer())
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error during ok button.");
            }
        }

        private void ServerConnection_Load(object sender, EventArgs e)
        {
            Activate();
        }

        private void ServerConnection_Shown(object sender, EventArgs e)
        {
            this.cboServerName.Text = UserSettings.Default.ServerAddress;
            this.txtPort.Text = UserSettings.Default.ServerPort.ToString();
        }

        private void lblDemo_Click(object sender, EventArgs e)
        {
            this.cboServerName.Text = UserSettings.DemoServerAddress;
            this.txtPort.Text = UserSettings.DemoServerPort.ToString();
        }

        private void cboServerName_SelectionChanged(object sender, EventArgs e)
        {
            if (cboServerName.SelectedItem != null && cboServerName.SelectedItem.ListObject is ServerAddressEntry)
            {
                var serverAddress = cboServerName.SelectedItem.ListObject as ServerAddressEntry;
                cboServerName.Text = serverAddress.Name;
                txtPort.Text = serverAddress.Port.ToString();
            }
        }

        private void Server_FindCompleted(object sender, FindCompletedEventArgs e)
        {
            try
            {
                _servers.Clear();
                _isServersLoaded = true;

                // here is the really nasty part
                // i am just returning the first channel, but it may not work.
                // you have to do some logic to decide which uri to use from the discovered uris
                // for example, you may discover "127.0.0.1", but that one is obviously useless.
                // also, catch exceptions when no endpoints are found and try again.
                if (e.Result.Endpoints.Count > 0)
                {
                    foreach (var endPoint in e.Result.Endpoints)
                    {
                        if (endPoint.Address.Uri.IsLoopback)
                            continue;
                        if (endPoint.Address.Uri.Scheme != DwosServerInfo.SERVER_ADDRESS_SCHEME)
                            continue;
                        
                        var serverAddress = new ServerAddressEntry() { Name = endPoint.Address.Uri.Host, Port = endPoint.Address.Uri.Port };
                        _servers.Add(serverAddress);
                    }
                }

                _servers.Add(new ServerAddressEntry() { Name = UserSettings.DemoServerAddress, Port = UserSettings.DemoServerPort });                
                cboServerName.DataSource = _servers;
                
                //force drop down refresh
                cboServerName.CloseUp();
                cboServerName.DropDown();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error finding servers.");

            }
            finally
            {
                SetLoading(false);
            }
        }

        private void cboServerName_BeforeDropDown(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_isServersLoaded)
                FindServer();
        }

        #endregion

        #region ServerAddress

        private class ServerAddressEntry
        {
            public string Name { get; set; }
            public int Port { get; set; }

            public override string ToString()
            {
                return Name + ":" + Port;
            }
        }

        #endregion
    }
}