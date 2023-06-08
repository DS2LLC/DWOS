using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Utilities.Validation;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinTree;
using NLog;
using DWOS.Shared.Utilities;

namespace DWOS.UI.Admin.Users
{
    public partial class UserInformation: DataPanel
    {
        #region Fields

        private const string DEFAULT_NAME = "New User";
        private readonly DisplayDisabledTooltips _displayDisabledTooltips;

        #endregion

        #region Properties

        public SecurityDataSet Dataset
        {
            get => _dataset as SecurityDataSet;
            set => _dataset = value;
        }

        protected override string BindingSourcePrimaryKey =>
            Dataset.Users.UserIDColumn.ColumnName;

        #endregion

        #region Methods

        public UserInformation()
        {
            this.InitializeComponent();
            _displayDisabledTooltips = new DisplayDisabledTooltips(grpData, tipManager);
        }

        public void LoadData(SecurityDataSet dataset)
        {
            this.Dataset = dataset;
            bsData.DataSource = this.Dataset;
            bsData.DataMember = this.Dataset.Users.TableName;

            //bind column to control
            base.BindValue(this.txtName, this.Dataset.Users.NameColumn.ColumnName);
            base.BindValue(this.txtLogOn, this.Dataset.Users.UserLogOnColumn.ColumnName);
            base.BindValue(this.chkActive, this.Dataset.Users.ActiveColumn.ColumnName);
            base.BindValue(this.chkOrderReview, this.Dataset.Users.RequireOrderReviewColumn.ColumnName);
            base.BindValue(this.txtDepartment, this.Dataset.Users.DepartmentColumn.ColumnName);
            base.BindValue(this.txtTitle, this.Dataset.Users.TitleColumn.ColumnName);
            base.BindValue(this.txtPinUser, this.Dataset.Users.LoginPinColumn.ColumnName);
            base.BindValue(this.txtEmailAddress, this.Dataset.Users.EmailAddressColumn.ColumnName);
            base.BindValue(this.chkSignCOC, "CheckState", this.Dataset.Users.SignCOCColumn.ColumnName);

            //load available Roles
            foreach(SecurityDataSet.SecurityGroupRow role in dataset.SecurityGroup)
                this.tvwRoles.Nodes.Add(new SecurityGroupNode(role));

            this.tvwRoles.Override.SortComparer = new SecurityRoleComparer();
            this.tvwRoles.Override.Sort = SortType.Ascending;

            base._panelLoaded = true;
        }

        public override void AddValidators(ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtName, "User name required.") { DefaultValue = DEFAULT_NAME }, errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtLogOn, "Log On ID required."), errProvider));
            manager.Add(new ImageDisplayValidator(new UserPinControlValiditor(this.txtPinUser, this), errProvider){RequiredStyleSetName = null});

            // Do not require or validate email
        }

        public SecurityDataSet.UsersRow AddUserRow()
        {
            var rowVw               = bsData.AddNew() as DataRowView;

            var cr                  = rowVw.Row as SecurityDataSet.UsersRow;
            cr.Name                 = DEFAULT_NAME;
            cr.UserLogOn            = RandomUtils.GetRandomDigits(4);
            cr.RequireOrderReview   = true;

            return cr;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            this.LoadUserRoles();
            this.LoadImage();
            this.LoadSignature();

            var user = base.CurrentRecord as SecurityDataSet.UsersRow;

            if (user != null)
            {
                using (var tableAdapter = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.User_SecurityRolesTableAdapter())
                {
                    tableAdapter.FillAllByUser(this.Dataset.User_SecurityRoles, user.UserID);
                }

                chkSignCOC.Enabled = Dataset.User_SecurityRoles
                    .Any(role => role.SecurityRoleID == "COCSign");
            }
            else
            {
                this.chkSignCOC.Enabled = true;
            }

            base.AfterMovedToNewRecord(id);
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            this.SaveUserRoles();
        }

        /// <summary>
        ///   Loads the users picture image.
        /// </summary>
        private void LoadImage()
        {
            try
            {
                this.ClearCurrentImage();

                var user = CurrentRecord as SecurityDataSet.UsersRow;

                //Load media if not already loaded
                if (user == null)
                {
                    return;
                }

                if(!user.IsMediaIDNull() && user.MediaRow != null)
                {
                    //if media has not been fetched from db then do so
                    if(user.MediaRow.IsMediaNull())
                    {
                        //Manually load the media via GetMediaStream
                        using(var ta = new MediaTableAdapter())
                        {
                            user.MediaRow.Media = ta.GetMediaStream(user.MediaID);
                            user.MediaRow.AcceptChanges(); //don't take this as a change to the row
                        }
                    }

                    //load into picture box
                    if(!user.MediaRow.IsMediaNull() && user.MediaRow.Media.Length > 0)
                        this.picUserImage.Image = MediaUtilities.GetImage(user.MediaRow.Media);
                    else
                    {
                        this.picUserImage.Image = DWOS.UI.Properties.Resources.nopicture_thumb;
                        _log.Info("Error loading user image from remote data stream.");
                    }
                }
                else if (!string.IsNullOrEmpty(txtName.Text) && txtName.Text != DEFAULT_NAME)
                {
                    SetNewImage(UserImageGenerator.GetImage(txtName.Text));
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error loading image.");
            }
        }

        private void LoadSignature()
        {
            ClearCurrentSignature();

            var user = CurrentRecord as SecurityDataSet.UsersRow;

            if (user != null)
            {
                if (!user.IsSignatureMediaIDNull() && user.MediaRowByFK_Users_SignatureMedia != null)
                {
                    var mediaRow = user.MediaRowByFK_Users_SignatureMedia;
                    if (mediaRow.IsMediaNull())
                    {
                        using(var ta = new MediaTableAdapter())
                        {
                            mediaRow.Media = ta.GetMediaStream(user.SignatureMediaID);
                            mediaRow.AcceptChanges(); //don't take this as a change to the row
                        }
                    }

                    if (!mediaRow.IsMediaNull() && mediaRow.Media.Length > 0)
                    {
                        this.picSignature.Image = MediaUtilities.GetImage(mediaRow.Media);
                    }
                    else
                    {
                        this.picSignature.Image = DWOS.UI.Properties.Resources.NoImage;
                        _log.Info("Error loading user image from remote data stream.");
                    }
                }
            }
        }

        private void ClearCurrentImage()
        {

            try
            {
                if(this.picUserImage.Image is Image)
                    ((Image)this.picUserImage.Image).Dispose();

                this.picUserImage.Image = null;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error clearing the currently selected image.";
                _log.Info(exc, errorMsg);
            }
        }

        private void ClearCurrentSignature()
        {
            try
            {
                Image currentImage = this.picSignature.Image as Image;

                if (currentImage != null)
                {
                    currentImage.Dispose();
                }

                this.picSignature.Image = null;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error clearing the currently selected signature.";
                _log.Info(exc, errorMsg);
            }
        }

        private void SetNewImage()
        {
            try
            {
                var user = CurrentRecord as SecurityDataSet.UsersRow;
                if(user != null)
                {
                    string imgPath = MediaUtilities.SelectImageDialog();

                    if(!String.IsNullOrWhiteSpace(imgPath))
                    {
                        var image = Image.FromFile(imgPath);
                        SetNewImage(image);
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting new image.");
            }
        }

        private void SetNewImage(Image image)
        {
            try
            {
                var user = CurrentRecord as SecurityDataSet.UsersRow;

                if (user != null)
                {

                    if (image != null)
                    {
                        //resize image
                        this.picUserImage.Image = image;
                        byte[] imageBytes       = MediaUtilities.GetImageAsBytes(image, 90);

                        //create new media row
                        if (user.IsMediaIDNull())
                            user.MediaRow = this.Dataset.Media.AddMediaRow(user.UserID.ToString(), "UserPicture.jpg", "jpg", imageBytes);
                        else //else just update the media itself
                            user.MediaRow.Media = imageBytes;
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting new image.");
            }
        }

        private void SetNewSignature()
        {
            try
            {
                var user = CurrentRecord as SecurityDataSet.UsersRow;
                if(user != null)
                {
                    string imgPath = MediaUtilities.SelectImageDialog();

                    if (!String.IsNullOrWhiteSpace(imgPath))
                    {
                        var image = Image.FromFile(imgPath);
                        SetNewSignature(image);
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting new image.");
            }
        }

        private void SetNewSignature(Image image)
        {
            try
            {
                var user = CurrentRecord as SecurityDataSet.UsersRow;

                if (user != null && image != null)
                {
                    this.picSignature.Image = image;
                    byte[] imageBytes = MediaUtilities.GetImageAsBytes(image, 90);

                    if (user.IsSignatureMediaIDNull())
                    {
                        user.MediaRowByFK_Users_SignatureMedia = Dataset.Media.AddMediaRow(user.UserID.ToString(),
                            "UserSignature.jpg",
                            "jpg",
                            imageBytes);
                    }
                    else
                    {
                        user.MediaRowByFK_Users_SignatureMedia.Media = imageBytes;
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting new image.");
            }
        }

        public override void EndEditing()
        {
            //ensure roles are saved for current user
            this.SaveUserRoles();
            base.EndEditing();
        }

        private void LoadUserRoles()
        {
            this.tvwRoles.Tag = null;
            var user = base.CurrentRecord as SecurityDataSet.UsersRow;

            if(user != null)
            {
                SecurityDataSet.User_SecurityGroupRow[] userRoles = user.GetUser_SecurityGroupRows();

                foreach(SecurityGroupNode roleNode in this.tvwRoles.Nodes.OfType<SecurityGroupNode>())
                {
                    //find if user has current role, then check role if user has it already
                    var matchingRole = userRoles
                        .FirstOrDefault(r => r.SecurityGroupID == roleNode.DataRow.SecurityGroupID);

                    roleNode.CheckedState = matchingRole != null ? CheckState.Checked : CheckState.Unchecked;
                }

                this.tvwRoles.Tag = user;
            }
        }

        private void SaveUserRoles()
        {
            var user = base.CurrentRecord as SecurityDataSet.UsersRow;

            if(user != null && this.tvwRoles.Tag == user)
            {
                // Sync all users security groups with changes to selected groups
                SecurityDataSet.User_SecurityGroupRow[] userSecurityGroups = user.GetUser_SecurityGroupRows();

                foreach(SecurityGroupNode sgNode in this.tvwRoles.Nodes.OfType<SecurityGroupNode>())
                {
                    //if user has this role
                    var matchingRole = userSecurityGroups
                        .FirstOrDefault(r => r.RowState != DataRowState.Deleted && r.RowState != DataRowState.Detached && r.SecurityGroupID == sgNode.DataRow.SecurityGroupID);

                    if(sgNode.CheckedState == CheckState.Checked)
                    {
                        //if no role then add it
                        if(matchingRole == null)
                        {
                            this.Dataset.User_SecurityGroup.AddUser_SecurityGroupRow(sgNode.DataRow, user);
                        }
                    }
                    else if(sgNode.CheckedState == CheckState.Unchecked)
                    {
                        //if role exists then remove
                        if(matchingRole != null)
                        {
                            matchingRole.Delete();
                        }
                    }
                }
            }
        }

        protected override void OnDispose()
        {
            try
            {
                _displayDisabledTooltips?.Dispose();
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Error on {nameof(UserInformation)} dispose");
            }

            base.OnDispose();
        }

        #endregion

        #region Events

        private void picUserImage_Click(object sender, EventArgs e)
        {
            try
            {
                this.ClearCurrentImage();
                this.SetNewImage();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting a new image.");
            }
        }

        private void txtName_Leave(object sender, EventArgs e)
        {
            try
            {
                bool skipImageChange = picUserImage.Image != null ||
                                       string.IsNullOrEmpty(this.txtName.Text) ||
                                       this.txtName.Text == DEFAULT_NAME;

                if (skipImageChange)
                {
                    return;
                }

                var dialogResult = MessageBoxUtilities.ShowMessageBoxYesOrNo(
                    "Do you want to load a default user image based on the user's name?",
                    "User Manager");

                if (dialogResult == DialogResult.Yes)
                {
                    txtName.DataBindings[0].WriteValue(); // otherwise, name won't change
                    SetNewImage(UserImageGenerator.GetImage(txtName.Text));
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error setting default image.");
            }
        }

        private void ultraLabel6_Click(object sender, EventArgs e)
        {
            try
            {
                Gravatar.BeginGetImage(txtEmailAddress.Text, SetNewImage);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Warn(exc, "Error setting gravatar on label click.");
            }
        }

        private void lblDefaultImage_Click(object sender, EventArgs e)
        {
            try
            {
                txtName.DataBindings[0].WriteValue(); // otherwise, name may not change
                SetNewImage(UserImageGenerator.GetImage(txtName.Text));
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error setting image on label click.");
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                this.ClearCurrentSignature();
                this.SetNewSignature();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting a new signature image.");
            }
        }

        private void chkSignCOC_CheckStateChanged(object sender, EventArgs e)
        {
            // Prevent checkbox from showing (or having) indeterminate state.
            // This usually occurs when the User.SignCOC column has a null, default value.
            if (chkSignCOC.CheckState == CheckState.Indeterminate)
            {
                chkSignCOC.CheckState = CheckState.Unchecked;
            }
        }

        #endregion

        #region RoleNode

        internal class SecurityGroupNode : DataNode<SecurityDataSet.SecurityGroupRow>
        {
            public const string KEY_PREFIX = "SG";

            #region Methods

            public SecurityGroupNode(SecurityDataSet.SecurityGroupRow cr)
                : base(cr, cr.SecurityGroupID.ToString(), KEY_PREFIX, cr.Name)
            {
                base.Override.NodeStyle = NodeStyle.CheckBox;
            }

            public override bool CanDelete
            {
                get { return false; }
            }

            #endregion

            public override bool CanPasteData(string format)
            {
                return false;
            }
        }

        #endregion

        #region UserPin Validator

        public class UserPinControlValiditor: ControlValidatorBase
        {
            #region Fields

            private UserInformation _userInfo;

            #endregion

            #region Methods

            public UserPinControlValiditor(Control control, UserInformation userInfo)
                : base(control)
            {
                this._userInfo = userInfo;

                var editor = Control as UltraTextEditor;
                if(editor != null)
                    editor.KeyPress += this.editor_KeyPress;
            }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                var editor = Control as UltraTextEditor;

                if(editor != null && editor.Enabled)
                {
                    //it is ok to be null
                    if(String.IsNullOrWhiteSpace(editor.Text))
                    {
                        e.Cancel = false;
                        return;
                    }

                    var newPin = editor.Text.Trim();
                    editor.Text = newPin;

                    if(newPin.Length < ApplicationSettings.Current.UserPinMinLength)
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "New pin must be at least {0} characters long.".FormatWith(ApplicationSettings.Current.UserPinMinLength));
                        return;
                    }

                    if(!newPin.All(Char.IsNumber))
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "New pin must contain only numbers.");
                        return;
                    }

                    bool alreadyInUse = false;
                    alreadyInUse = this._userInfo.Dataset.Users.Any(ur => ur.RowState != DataRowState.Deleted && !ur.IsLoginPinNull() && ur.LoginPin == newPin && ur != this._userInfo.CurrentRecord);
                    
                    if(alreadyInUse || SecurityManager.ReservedPins.Contains(newPin))
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "Invalid pin, please choose another.");
                        return;
                    }
                }

                //passed
                e.Cancel = false;
                FireAfterValidation(true, String.Empty);
            }

            public override void Dispose()
            {
                this._userInfo = null;

                var editor = Control as UltraTextEditor;
                if(editor != null)
                    editor.KeyPress -= this.editor_KeyPress;

                base.Dispose();
            }

            #endregion

            #region Events

            private void editor_KeyPress(object sender, KeyPressEventArgs e)
            {
                if(!Char.IsNumber(e.KeyChar) && !Char.IsControl(e.KeyChar))
                    e.Handled = true;
            }

            #endregion
        }

        #endregion

        #region SecurityRoleComparer

        private class SecurityRoleComparer : IComparer<SecurityDataSet.SecurityRoleRow>, IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                if (x is SecurityDataSet.SecurityRoleRow && y is SecurityDataSet.SecurityRoleRow)
                    return this.Compare((SecurityDataSet.SecurityRoleRow)x, (SecurityDataSet.SecurityRoleRow)y);

                if (x == null)
                    return 1;
                if (y == null)
                    return -1;

                return x.ToString().CompareTo(y.ToString());
            }

            #endregion

            #region IComparer<SecurityRoleRow> Members

            public int Compare(SecurityDataSet.SecurityRoleRow x, SecurityDataSet.SecurityRoleRow y)
            {
                return x.SecurityRoleID.CompareTo(y.SecurityRoleID);
            }

            #endregion

            public bool Equals(SecurityDataSet.SecurityRoleRow x, SecurityDataSet.SecurityRoleRow y)
            {
                return x.SecurityRoleID == y.SecurityRoleID;
            }

            public int GetHashCode(SecurityDataSet.SecurityRoleRow obj)
            {
                return obj.GetHashCode();
            }
        }

        #endregion
    }
}