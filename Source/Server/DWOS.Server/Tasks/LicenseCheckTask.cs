using System;
using System.ServiceModel;
using System.Threading.Tasks;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.LicenseManager;
//using DWOS.Server.LicenseActivation;
using DWOS.Server.Utilities;
using NLog;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;

namespace DWOS.Server.Tasks
{
    internal class LicenseCheckTask : IJob
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        public async Task Execute(IJobExecutionContext context)
        {
           
        }


        #endregion
    }
}