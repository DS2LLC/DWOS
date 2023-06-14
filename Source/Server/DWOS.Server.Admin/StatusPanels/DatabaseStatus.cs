using System;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Server.Admin.Properties;
using DWOS.Server.Admin.Tasks;
using DWOS.Shared.Utilities;
using NLog;

namespace DWOS.Server.Admin.StatusPanels
{
    /// <summary>
    /// Shows database status information.
    /// </summary>
    public partial class DatabaseStatus: UserControl
    {
        #region Methods
        
        public DatabaseStatus()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Starts checking database status asynchronously.
        /// </summary>
        public void BeginUpdateStatus()
        {
            ThreadPool.QueueUserWorkItem(cb =>
            {
                Thread.Sleep(500); // HACK - not sure about this delay
                this.GetDatabaseStatus();
            });
            
        }

        private void UpdatStatus(bool isConfigured)
        {
            this.picActivate.Image  = isConfigured ? Resources.Check32 : Resources.Question64;
            this.txtStatus.Text     = isConfigured ? "Complete" : "Incomplete";
        }
       
        private void GetDatabaseStatus()
        {
            try
            {
                if(ServerSettings.Default.DBConnectionString != null)
                {
                    using(var conn = new SqlConnection(ServerSettings.Default.DBConnectionString))
                    {
                        conn.Open();
                        
                        UpdateConnectionInfo(conn.DataSource, conn.Database);

                        using (var command = conn.CreateCommand())
                        {
                            command.CommandText = "SELECT [Value] FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion'";
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                    UpdateSchemaVersion(reader[0].ToString());
                            }
                        }

                        using (var command = conn.CreateCommand())
                        {
                            command.CommandText = "SELECT SERVERPROPERTY('productversion') as 'Version', SERVERPROPERTY ('edition') as 'Edition'";
                            using(var reader = command.ExecuteReader())
                            {
                                if(reader.Read())
                                    UpdateDBInfo(reader["Version"].ToString(), reader["Edition"].ToString());
                            }
                        }

                        using (var command2 = conn.CreateCommand())
                        {
                            // Try and get the DB Size, may not return anything if we don't have permisson.
                            command2.CommandText = "SELECT DB_NAME(database_id) AS DatabaseName, Name AS Logical_Name, Physical_Name, size*8 Bytes FROM sys.master_files WHERE DB_NAME(database_id) = '" + conn.Database + "'";

                            using(var reader = command2.ExecuteReader())
                            {
                                if(reader.Read())
                                {
                                    var bytes           = Convert.ToInt64(reader["Bytes"]);
                                    UpdateDBSize(FileSystem.ConvertBytesToMegabytes(bytes).ToString("N") + " MB");
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Info(exc, "Error getting current database status.");
            }
        }

        private void UpdateSchemaVersion(string schmeaVersion)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(UpdateSchemaVersion), schmeaVersion);
            }
            else
            {
                this.lblSchemaVersion.Text = schmeaVersion;
            }
        }

        private void UpdateDBInfo(string dbVersion, string dbEdition)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string, string>(UpdateDBInfo), dbVersion, dbEdition);
            }
            else
            {
                this.txtVersion.Text            = dbVersion;
                this.txtDatabaseEdition.Text    = dbEdition;
            }
        }

        private void UpdateDBSize(string dbSize)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(UpdateDBSize), dbSize);
            }
            else
                this.txtSize.Text = dbSize;
        }

        private void UpdateConnectionInfo(string dbServerName, string dbName)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string, string>(UpdateConnectionInfo), dbServerName, dbName);
            }
            else
            {
                this.txtServerName.Text = dbServerName;
                this.txtDatabase.Text   = dbName;

                //if there is a database name and server set then set as successfully configured
                this.UpdatStatus(dbName != null && dbServerName != null);
            }
        }

        #endregion

        #region Events
        
        private void btnActivate_Click(object sender, EventArgs e)
        {
            using(var frm = new DatabaseConfiguration())
            {
                frm.ShowDialog(this);
            }

            BeginUpdateStatus();
            //force dwos server to reload from registry
            Main.ForceServerToReload();
        }

        private void LicenseStatus_Load(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            this.UpdatStatus(false);
            this.BeginUpdateStatus();
        }
        
        #endregion
    }
}