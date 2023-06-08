using System;
using System.Configuration;
using System.Linq;
using DWOS.Shared;

namespace DWOS.UI.Properties
{
    public sealed partial class Settings
    {
        #region Methods

        public Settings()
        {
            try
            {
                //if we need to upgrade then upgrade the
                if(this.CallUpgrade)
                {
                    //upgrade database if first time in it
                    Upgrade();
                    this.CallUpgrade = false;
                }
            }
            catch(Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error upgrading previous version of your settings file.");
            }
        }

        #endregion
    }
}