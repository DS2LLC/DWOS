using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace DWOS.Server.Admin.Tasks
{
    /// <summary>
    /// Prompts the user for database information and (conditionally) saves
    /// it to <see cref="Data.ServerSettings"/>.
    /// </summary>
    public partial class DatabaseConfiguration: Form
    {
        #region Fields
        
        private const int SQL_AUTHENTICATION = 1;
        private const int WINDOWS_AUTHENTICATION = 0;
        private bool _databaseServersLoaded;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether to disable changes to the
        /// database name.
        /// </summary>
        /// <value>
        /// <c>true</c> if changes to database name should be disabled;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool DisableDatabaseName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to save database settings
        /// to the registry.
        /// </summary>
        /// <value>
       /// <c>true</c> to save values to the registry; otherwise, <c>false</c>
        /// </value>
        public bool SaveSettingsToRegistry { get; set; }

        /// <summary>
        /// Gets or sets the database connection string.
        /// </summary>
        public string DBConnectionString { get; set; }

        #endregion
        
        #region Methods

        public DatabaseConfiguration()
        {
            this.InitializeComponent();
            SaveSettingsToRegistry = true;
        }

        private SqlConnection BuildConnection(bool includeDatabase = true)
        {
            var connStringBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            connStringBuilder.DataSource = cboServerName.Text;

            if (cboAuthentication.SelectedIndex == SQL_AUTHENTICATION)
            {
                connStringBuilder.UserID = txtUserName.Text;
                connStringBuilder.Password = txtPassword.Text;
                connStringBuilder.IntegratedSecurity = false;
                connStringBuilder.PersistSecurityInfo = true;
            }
            else
            {
                connStringBuilder.IntegratedSecurity = true;
            }

            if (includeDatabase)
                connStringBuilder.InitialCatalog = cboDatabaseName.Text;

            connStringBuilder.ConnectRetryCount = numRetryCount.Value as int? ?? 1;
            connStringBuilder.ConnectRetryInterval = numRetryInterval.Value as int? ?? 10;

            return new System.Data.SqlClient.SqlConnection(connStringBuilder.ConnectionString);
        }

        private List<string> EnumerateServers()
        {
            // Perform the enumeration
            DataTable dataTable = null;
            List<string> servers = new List<string>();

            try
            {
                dataTable = System.Data.Sql.SqlDataSourceEnumerator.Instance.GetDataSources();
            }
            catch
            {
                dataTable = new DataTable();
                dataTable.Locale = System.Globalization.CultureInfo.InvariantCulture;
            }

            // Create the object array of server names (with instances appended)
            servers.Clear();

            foreach (DataRow row in dataTable.Rows)
            {
                string name     = row["ServerName"].ToString();
                string instance = row["InstanceName"].ToString();

                if (instance.Length == 0)
                    servers.Add(name);
                else
                    servers.Add(name + "\\" + instance);
            }

            // Sort the list
            servers.Sort();
            return servers;
        }

        private List<string> EnumerateDatabases()
        {
            // Perform the enumeration
            DataTable dataTable = null;
            IDbConnection connection = null;
            IDataReader reader = null;

            try
            {
                // Get a basic connection
                connection = BuildConnection(false);

                // Create a command to check if the database is on SQL AZure.
                IDbCommand command = connection.CreateCommand();
                command.CommandText = "SELECT CASE WHEN SERVERPROPERTY(N'EDITION') = 'SQL Data Services' OR SERVERPROPERTY(N'EDITION') = 'SQL Azure' THEN 1 ELSE 0 END";

                // Open the connection
                connection.Open();

                // SQL AZure doesn't support HAS_DBACCESS at this moment.
                // Change the command text to get database names accordingly
                if ((Int32)(command.ExecuteScalar()) == 1)
                {
                    command.CommandText = "SELECT name FROM master.dbo.sysdatabases ORDER BY name";
                }
                else
                {
                    command.CommandText = "SELECT name FROM master.dbo.sysdatabases WHERE HAS_DBACCESS(name) = 1 ORDER BY name";
                }

                // Execute the command
                reader = command.ExecuteReader();

                // Read into the data table
                dataTable = new DataTable();
                dataTable.Locale = System.Globalization.CultureInfo.CurrentCulture;
                dataTable.Load(reader);
            }
            catch
            {
                dataTable = new DataTable();
                dataTable.Locale = System.Globalization.CultureInfo.InvariantCulture;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
                if (connection != null)
                {
                    connection.Dispose();
                }
            }

            // Create the object array of database names
            List<string> databases = new List<string>();
            foreach (DataRow row in dataTable.Rows)
                databases.Add(row["name"].ToString());

            return databases;
        }

        private bool TestConnection()
        {
            try
            {
                using (var connection = BuildConnection())
                    connection.Open();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void LoadConnection(string connectionString)
        {
           var connStringBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);

            cboServerName.Text = connStringBuilder.DataSource;

            if (!String.IsNullOrEmpty(connStringBuilder.UserID))
            {
                cboAuthentication.SelectedIndex = SQL_AUTHENTICATION;
                txtUserName.Text = connStringBuilder.UserID;
                txtPassword.Text = connStringBuilder.Password;
                numRetryCount.Value = connStringBuilder.ConnectRetryCount;
                numRetryInterval.Value = connStringBuilder.ConnectRetryInterval;
            }
            else
            {
                cboAuthentication.SelectedIndex = WINDOWS_AUTHENTICATION;
            }

            cboDatabaseName.Text = connStringBuilder.InitialCatalog;
        }
        
        private void SaveSettings()
        {
            try
            {
                this.DBConnectionString = BuildConnection().ConnectionString;

                if(SaveSettingsToRegistry)
                {
                    DWOS.Data.ServerSettings.Default.DBConnectionString = DBConnectionString;
                    DWOS.Data.ServerSettings.Default.UpdateDatabaseConnection("DWOS Server Admin");
                    DWOS.Data.ServerSettings.Default.Save();
                }
            }
            catch (System.Security.SecurityException exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error("You do not have permission to save settings to the registry. Please run this application as administrator to make system changes.", exc);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error saving database connection settings.");
            }
        }
        
        #endregion

        #region Events

        private void cboServerName_BeforeDropDown(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_databaseServersLoaded)
                return;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                
                _databaseServersLoaded = true;

                cboServerName.Items.Clear();
                var servers = EnumerateServers();
                servers.ForEach(s => cboServerName.Items.Add(s));
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error ");
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        
        private void cboServerName_SelectionChanged(object sender, EventArgs e)
        {
            //clear database name if server changed
            cboDatabaseName.ResetText();
        }

        private void cboDatabaseName_BeforeDropDown(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                cboDatabaseName.ResetText();

                cboDatabaseName.Items.Clear();
                var database = EnumerateDatabases();
                database.ForEach(db => cboDatabaseName.Items.Add(db));
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error  ");
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
               this.Cursor = Cursors.WaitCursor;

                if (TestConnection())
                {
                    MessageBox.Show("Connection successful!", "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBox.Show("Unable to connect to database.", "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error  ");
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void DatabaseConfiguration_Load(object sender, EventArgs e)
        {
            if(DBConnectionString == null)
                DBConnectionString = DWOS.Data.ServerSettings.Default.DBConnectionString;

            cboAuthentication.SelectedIndex = SQL_AUTHENTICATION;
            LoadConnection(DBConnectionString);

            //if not allowed to set database name
            if (DisableDatabaseName)
                cboDatabaseName.Enabled = false;
        }

        private void cboAuthentication_SelectionChanged(object sender, EventArgs e)
        {
            txtPassword.Enabled = cboAuthentication.SelectedIndex == SQL_AUTHENTICATION;
            txtUserName.Enabled = cboAuthentication.SelectedIndex == SQL_AUTHENTICATION;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                SaveSettings();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error closing database connection.");
            }
        }
        
        #endregion
    }
}