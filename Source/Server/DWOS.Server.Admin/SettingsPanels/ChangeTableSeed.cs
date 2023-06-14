using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Shared;

namespace DWOS.Server.Admin.SettingsPanels
{
    /// <summary>
    /// Changes the seed for a specific table.
    /// </summary>
    public partial class ChangeTableSeed : Form
    {
        #region Fields

        private string _columnName;

        #endregion

        #region Methods

        public ChangeTableSeed()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads data for this instance.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName">
        /// </param>
        /// <param name="currentSeed"></param>
        public void LoadData(string tableName, string columnName, int currentSeed)
        {
            txtCurrentSeed.Text = currentSeed.ToString();
            txtTable.Text = tableName;
            _columnName = columnName;

            numNewSeed.MinValue = currentSeed;
            numNewSeed.Value = currentSeed + 1;
        }

        private void SaveData()
        {
            try
            {
                var sql = "DBCC CHECKIDENT('{0}', RESEED, {1})".FormatWith(txtTable.Text, numNewSeed.Value);
                using(var cmd = new SqlCommand(sql, new SqlConnection(ServerSettings.Default.DBConnectionString)))
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                }
                
                MessageBox.Show("Updating table id completed.");
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error reseeding database table {0}.".FormatWith(txtTable.Text));
            }
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e) 
        {
            if(MessageBox.Show("Are you sure you want to udpate the id of this table?", About.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                this.SaveData();
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
