using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Server.Admin.Tasks;
using DWOS.Shared.Wizard;

namespace DWOS.Server.Admin.Wizards.InstallWizard
{
    /// <summary>
    /// The 'validate database connection' step of the installer wizard.
    /// </summary>
    public partial class ConnectToDatabase : UserControl, IWizardPanel
    {
        #region Fields

        private bool _installed;

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
                OnValidStateChanged?.Invoke(this);

                using (var connection = new SqlConnection(ServerSettings.Default.DBConnectionString))
                {
                    connection.Open();

                    var connBuilder = new SqlConnectionStringBuilder(ServerSettings.Default.DBConnectionString);

                    //If a initial catalog was selected (i.e. the database) then verify it is a valid DWOS database
                    if(!String.IsNullOrWhiteSpace(connBuilder.InitialCatalog))
                    {
                        try
                        {
                            using(SqlCommand command = connection.CreateCommand())
                            {
                                //attempt to get application settings count
                                command.CommandText = String.Format("SELECT COUNT(*) FROM ApplicationSettings;");
                                using(SqlDataReader reader = command.ExecuteReader())
                                {
                                    if(reader.Read())
                                    {
                                        string count = reader[0].ToString();
                                        //since didn't fail then we have a valid database already
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // Error occurred when accessing initial catalog.
                            // Remove it from the connection builder and connection string.
                            connBuilder.InitialCatalog = string.Empty;
                            ServerSettings.Default.DBConnectionString = connBuilder.ConnectionString;
                        }
                    }
                }

                this.txtStatus.Text = "Valid";
                this._installed = true;
            }
            catch
            {
                this.txtStatus.Text = "Invalid";
                this._installed = false;
            }
            finally
            {
                OnValidStateChanged?.Invoke(this);
            }
        }

        private void UpdateStatus()
        {
            if(!String.IsNullOrEmpty(ServerSettings.Default.DBConnectionString))
            {
                var connBuilder = new SqlConnectionStringBuilder(ServerSettings.Default.DBConnectionString);
                this.txtServerName.Text = connBuilder.DataSource;
                this.txtDatabaseName.Text = connBuilder.InitialCatalog;
            }
        }

        private string ClearInitialCatalog(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }

            var connBuilder = new SqlConnectionStringBuilder(connectionString);
            connBuilder.InitialCatalog = string.Empty;

            return connBuilder.ConnectionString;
        }

        #endregion

        #region Events

        private void btnTestConnection_Click(object sender, EventArgs e) { ValidateConnection(); }

        private void btnConfigure_Click(object sender, EventArgs e)
        {
            var connString = ClearInitialCatalog(ServerSettings.Default.DBConnectionString);

            using(var frm = new DatabaseConfiguration {DisableDatabaseName = true, DBConnectionString = connString})
            {
                if(frm.ShowDialog() == DialogResult.OK)
                {
                    OnMoveTo(); //set connection string from dialog
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

        public void Initialize(WizardController controller) { }

        public Control PanelControl
        {
            get { return this; }
        }

        public bool IsValid
        {
            get { return this._installed; }
        }

        public void OnMoveTo() { UpdateStatus(); }

        public void OnMoveFrom() { }

        public void OnFinished() { }

        #endregion
    }
}