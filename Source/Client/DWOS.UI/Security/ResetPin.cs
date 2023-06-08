using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.SecurityDataSetTableAdapters;
using NLog;

namespace DWOS.UI.Utilities
{
    public partial class ResetPin: Form
    {
        public ResetPin()
        {
            this.InitializeComponent();
            this.RequireOldPin = true;
        }

        public SecurityDataSet.UsersRow User { get; set; }

        public bool RequireOldPin { get; set; }

        private bool ValidatePin()
        {
            if(this.User != null)
            {
                if(this.RequireOldPin)
                {
                    var originalPin = this.txtOriginal.Text;
                    bool validOriginalPin = false;

                    using(var ta = new UsersTableAdapter())
                    {
                        var pinUserID = ta.GetUserIdByUserLoginPin(originalPin);
                        validOriginalPin = pinUserID.GetValueOrDefault() == this.User.UserID;
                    }

                    if(!validOriginalPin)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn("Original pin did not match.", "Reset Pin");
                        return false;
                    }
                }

                if(String.IsNullOrWhiteSpace(this.txtNew.Text))
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("New pin not set.", "Reset Pin");
                    return false;
                }

                if(this.txtNew.Text != this.txtNew2.Text)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("New pin does not match.", "Reset Pin");
                    return false;
                }

                var newPin = this.txtNew.Text.Trim();

                if(newPin.Length < ApplicationSettings.Current.UserPinMinLength)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("New pin must be at least {0} characters long.".FormatWith(ApplicationSettings.Current.UserPinMinLength), "Reset Pin");
                    return false;
                }

                if(!newPin.All(Char.IsNumber))
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("New pin must contain only numbers.", "Reset Pin");
                    return false;
                }

                bool alreadyInUse = false;
                using(var ta = new UsersTableAdapter())
                {
                    var pinUserID = ta.GetUserIdByUserLoginPin(newPin);
                    alreadyInUse = pinUserID.HasValue && pinUserID.GetValueOrDefault() > 0;
                }

                //ensure not resetting to a reserved pin
                if(SecurityManager.ReservedPins.Contains(newPin))
                    alreadyInUse = true;

                if(alreadyInUse)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("Invalid pin, please choose another.", "Reset Pin");
                    return false;
                }

                return true;
            }

            return false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if(this.ValidatePin())
                {
                    using(var ta = new UsersTableAdapter())
                    {
                        ta.UpdateUserLoginPin(this.txtNew.Text.Trim(), this.User.UserID);
                    }

                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error reseting users pin.");
            }
        }

        private void ResetPin_Load(object sender, EventArgs e)
        {
            if(this.User == null)
                this.btnOK.Enabled = false;
            else
            {
                this.lblUserName.Text = this.User == null ? "NA" : this.User.Name;
                this.txtOriginal.Enabled = this.RequireOldPin;
            }
        }
    }
}