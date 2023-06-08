using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.SecurityDataSetTableAdapters;
using NLog;
using Infragistics.Win.Misc;

namespace DWOS.UI.Utilities
{
    /// <summary>
    ///     Displays the thubmnail of the user.
    /// </summary>
    public partial class UserProfileThumbnail : UserControl
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private object _userLock = new object();
        private UltraPopupControlContainer _popUp = null;

        #endregion

        #region Properties

        private SecurityDataSet.UsersRow User { get; set; }

        #endregion

        #region Methods

        public UserProfileThumbnail() { InitializeComponent(); }

        public void SetUser(SecurityDataSet.UsersRow usersRow)
        {
            try
            {
                lock (_userLock)
                {
                    Enabled = true;
                    User = usersRow;
                    this.txtName.Text = usersRow.Name;
                    this.txtDepartment.Text = usersRow.IsDepartmentNull() ? "" : usersRow.Department;
                    this.txtTitle.Text = usersRow.IsTitleNull() ? "" : usersRow.Title;
                }

                SetUserImage();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting user in thumbnail.");
            }
        }

        /// <summary>
        ///     Resets the user.
        /// </summary>
        public void ResetUser()
        {
            try
            {
                lock (_userLock)
                {
                    Enabled = false;
                    User = null;
                    this.txtName.Text = "No User";
                    this.txtDepartment.Text = "";
                    this.txtTitle.Text = "";
                }

                ClearCurrentImage();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error reseting user in thumbnail.");
            }
        }

        /// <summary>
        ///     Clears the current image.
        /// </summary>
        private void ClearCurrentImage()
        {
            try
            {
                if(this.picUserImage.Image is Image)
                    ((Image) this.picUserImage.Image).Dispose();

                this.picUserImage.Image = null;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error clearing the currently selected image.";
                _log.Info(exc, errorMsg);
            }
        }

        private void SetUserImage()
        {
            try
            {
                ClearCurrentImage();

                lock (_userLock)
                {
                    if (User != null && !User.IsMediaIDNull())
                    {
                        using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter())
                        {
                            byte[] imageBytes = ta.GetMediaStream(User.MediaID);
                            if (imageBytes != null && imageBytes.Length > 0)
                                this.picUserImage.Image = MediaUtilities.GetImage(imageBytes);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error setting user image.");
            }
        }

        private void ShowProfileEditor()
        {
            try
            {
                lock (_userLock)
                {
                    if (User != null && SecurityManager.Current.IsValidUser)
                    {
                        _popUp = new UltraPopupControlContainer();
                        var userDialog = new UserProfileDialog();
                        userDialog.Load(this.User.UserID);

                        var host = new ElementHost { Width = 360, Height = 300, Child = userDialog, BackColor = System.Drawing.Color.Transparent };
                        _popUp.PopupControl = host;

                        host.Dock = DockStyle.Fill;
                        _popUp.Closed += popUp_Closed;
                        _popUp.Show(this);
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on popup closed.");
            }
        }

        #endregion

        #region Events

        private void popUp_Closed(object sender, EventArgs e)
        {
            try
            {
                if (_popUp != null)
                {
                    var profile = ((ElementHost)_popUp.PopupControl).Child as UserProfileDialog;
                    profile.SaveData();

                    ((ElementHost)_popUp.PopupControl).Child = null;
                    _popUp.Dispose();
                    _popUp = null;

                    SetUserImage();
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on popup closed.");
            }
        }

        /// <summary>
        ///     Handles the Click event of the picUserImage control.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="EventArgs" /> instance containing the event data. </param>
        private void picUserImage_Click(object sender, EventArgs e) { ShowProfileEditor(); }

        private void ultraPictureBox1_Click(object sender, EventArgs e) { ShowProfileEditor(); }

        #endregion
    }
}