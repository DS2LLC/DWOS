using System;
using System.Threading.Tasks;
using NLog;
using Quartz;

namespace DWOS.Server.Tasks
{
    internal class LicensePruneTask : IJob
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await Task.Run(() => BeginProcessing()).ConfigureAwait(false);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error pruning licenses.");
            }
        }

        private void BeginProcessing()
        {
            _log.Info("BEGIN: LicensePruneTask");

            try
            {
                //prune inactive users
                LicenseManager.LicenseManager.Default.PruneInactiveUsers();
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error processing license pruning.");
            }
            finally
            {
                _log.Info("END: LicensePruneTask");
            }
        }

        #endregion
    }
}