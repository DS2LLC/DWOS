using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using DWOS.Server.Admin.Tasks;
using DWOS.Shared.Wizard;

namespace DWOS.Server.Admin.Wizards.UpgradeWizard
{
    /// <summary>
    /// The 'validate database connection' step of the upgrade wizard.
    /// </summary>
    public partial class ConnectToDatabase : UserControl, IWizardPanel
    {
        #region Fields

        private bool _isValid;
        private UpgradeWizardController _controller;

        #endregion

        #region Methods

        public ConnectToDatabase()
        {
            InitializeComponent();
        }

        private void ValidateConnection()
        {
            try
            {
                this._isValid = false;

                if(_controller.DBConnectionString != null)
                {
                    using(var connection = new SqlConnection(_controller.DBConnectionString))
                    {
                        connection.Open();

                        using(var command = connection.CreateCommand())
                        {
                            //attempt to get application settings count
                            command.CommandText = String.Format("SELECT IS_MEMBER('db_backupoperator')");

                            using(var reader = command.ExecuteReader())
                            {
                                if(reader.Read())
                                {
                                    var isMemberObj = reader[0];

                                    //since didn't fail then we have a valid database already
                                    this.txtStatus.Text = "Valid";
                                    this._isValid = true;

                                    var isAuthorized = isMemberObj != null && isMemberObj != DBNull.Value && Convert.ToInt32(isMemberObj) == 1;
                                    lblWarning.Visible = !isAuthorized;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                this.txtStatus.Text = "Invalid";
                this._isValid = false;
            }
            finally
            {
                OnValidStateChanged?.Invoke(this);
            }
        }

        private void UpdateStatus()
        {
            if (!String.IsNullOrEmpty(_controller.DBConnectionString))
            {
                var connBuilder = new SqlConnectionStringBuilder(_controller.DBConnectionString);
                this.txtServerName.Text = connBuilder.DataSource;
                this.txtDatabaseName.Text = connBuilder.InitialCatalog;
            }
        }
        
        #endregion

        #region Events
        
        private void btnTestConnection_Click(object sender, EventArgs e) { ValidateConnection(); }

        private void btnConfigure_Click(object sender, EventArgs e)
        {
            using(var frm = new DatabaseConfiguration {DisableDatabaseName = false, SaveSettingsToRegistry = false, DBConnectionString = _controller.DBConnectionString})
            {
                if(frm.ShowDialog() == DialogResult.OK)
                {
                    _controller.DBConnectionString = frm.DBConnectionString;

                    ValidateConnection();
                    UpdateStatus(); //validate connection is good so we can move on
                }
            }
        }

        #endregion

        #region IWizardPanel

        public string Title
        {
            get { return "Connect to Database"; }
        }

        public string SubTitle
        {
            get { return "Validate the connection to the database."; }
        }

        public Action <IWizardPanel> OnValidStateChanged { get; set; }

        public void Initialize(WizardController controller)
        {
            _controller = controller as UpgradeWizardController;
        }

        public Control PanelControl
        {
            get { return this; }
        }

        public bool IsValid
        {
            get { return this._isValid; }
        }

        public void OnMoveTo()
        {
            UpdateStatus();
        }

        public void OnMoveFrom() { }

        public void OnFinished() { }

        #endregion
    }
}