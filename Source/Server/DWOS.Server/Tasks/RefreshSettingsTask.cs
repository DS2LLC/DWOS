using System;
using System.Threading.Tasks;
using DWOS.Data;
using NLog;
using Quartz;

namespace DWOS.Server.Tasks
{
    internal class RefreshSettingsTask : IJob
    {
        #region Methods

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await Task.Run(() => ApplicationSettings.Current.ReloadSettings()).ConfigureAwait(false);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error refreshing application settings.");
            }
        }

        #endregion
    }
}