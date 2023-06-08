using System;
using System.Windows.Forms;
using DWOS.Shared;
using DWOS.UI.Licensing;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinListView;
using NLog;

namespace DWOS.UI.Security
{
    public partial class UserActivations: Form
    {
        public UserActivations()
        {
            this.InitializeComponent();
        }

        private void LoadData()
        {
            try
            {
                using(new UsingWaitCursor(this))
                {
                    this.btnRefesh.Enabled = false;

                    var client = SecurityManager.Current.CreateLicenseClient();
                    client.GetLicenseSummaryCompleted += (s, e) =>
                                                         {
                                                             this.btnRefesh.Enabled = true;
                                                             if(e.Error != null)
                                                                 ErrorMessageBox.ShowDialog("Error checking licenses.", e.Error);
                                                             else
                                                             {
                                                                 var results              = e.Result.GetLicenseSummaryResult;
                                                                 this.lblLicenseInfo.Text = results.CurrentActivations.Count + " of " + results.TotalActivations;

                                                                 this.lvwUsers.Items.Clear();
                                                                 results.CurrentActivations.ForEach(u =>
                                                                                                    {
                                                                                                        var info = new[] { u.ComputerName, u.Activated.ToString(), u.LastCommunication.ToString() };
                                                                                                        var item = new UltraListViewItem(u.UserName, info);
                                                                                                        item.Appearance.Image = Properties.Resources.User_32;
                                                                                                        this.lvwUsers.Items.Add(item);
                                                                                                    });
                                                             }
                                                         };

                    client.GetLicenseSummaryAsync(new GetLicenseSummaryRequest());
                }
            }
            catch(Exception exc)
            {
                this.btnRefesh.Enabled = true;
                LogManager.GetCurrentClassLogger().Info(exc, "Error getting license info.");
            }
        }

        private void UserActivations_Load(object sender, EventArgs e)
        {
            this.LoadData();
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            this.LoadData();
        }
    }
}