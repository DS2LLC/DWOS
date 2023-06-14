using System;
using System.ServiceModel;
using System.Threading.Tasks;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.LicenseManager;
using DWOS.Server.LicenseActivation;
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
            try
            {
                bool success = BeginProcessing();

                if (!success && context != null)
                {
                    //if failed then reschedule for 1 hour from now
                    _log.Info("LicenseCheckTask failed and is being rescheduled.");

                    var rnd = new Random();
                    var taskName = "LicenseCheckTask" + rnd.Next();
                    var triggerName = "LicenseCheckTrigger" + rnd.Next();
                    var triggerOffset = new DateTimeOffset(DateTime.Now.AddHours(1));
                    await context.Scheduler.ScheduleJob(new JobDetailImpl(taskName, null, typeof(LicenseCheckTask)), new SimpleTriggerImpl(triggerName, triggerOffset));
                }
            }
            catch(Exception exc)
            {
                throw new JobExecutionException(exc);
            }
        }

        private bool BeginProcessing()
        {
            _log.Info("BEGIN: LicenseCheckTask");

            try
            {
                //force validation of license in case anything below fails
                LicenseManager.LicenseManager.Default.ValidateLicense();

                var client = new ActivationServiceClient();
                LicenseInfo licenseInfo = client.GetLicenseInfo(ServerSettings.Default.CompanyKey, ServerSettings.Default.Fingerprint);

                if(licenseInfo.ErrorInformation != null)
                {
                    _log.Error("Error getting license info: " + licenseInfo.ErrorInformation);

                    //Notify Admin
                    string adminEmail = ServerSettings.Default.AdminEmail;

                    if (!String.IsNullOrEmpty(adminEmail))
                    {
                        var errorMsg = string.Format("DWOS server on {0} could not connect to the DWOS License Activation server. {1}",
                            Environment.MachineName,
                            licenseInfo.ErrorInformation);

                        MessagingUtilities.QuickSendEmail(adminEmail, "DWOS: Server Issue", errorMsg);
                    }

                    return false;
                }
                else
                {
                    //create new key, save it, and set it to the default license manager
                    var key = new LicenseFile {Activations = licenseInfo.Activations, CompanyName = licenseInfo.CompanyName, LicenseExpiration = licenseInfo.LicenseExpiration};

                    _log.Info("LicenseCheckTask retrieved license from server: " + key);
                    LicenseManager.LicenseManager.Default.CurrentLicense = key;

                    //Set the Company Name in application settings on DB to the name received here... A form of anti-piracy
                    UpdateApplicationSettings(key.CompanyName, ServerSettings.Default.CompanyKey);

                    return true;
                }
            }
            catch(Exception exc)
            {
                var logLevel = exc is EndpointNotFoundException
                    ? LogLevel.Warn
                    : LogLevel.Error;

                _log.Log(logLevel, exc, "Error processing license check task.");
                return false;
            }
            finally
            {
                _log.Info("END: LicenseCheckTask");
            }
        }

        private void UpdateApplicationSettings(string companyName, string companyKey)
        {
            try
            {
                using(var ta = new ApplicationSettingsTableAdapter())
                {
                    ApplicationSettingsDataSet.ApplicationSettingsDataTable appSettings = ta.GetData();

                    ApplicationSettingsDataSet.ApplicationSettingsRow companyKeyRow = appSettings.FindBySettingName("CompanyKey");

                    if(companyKeyRow == null)
                        appSettings.AddApplicationSettingsRow("CompanyKey", companyKey);
                    else if(companyKeyRow.Value != companyKey)
                        companyKeyRow.Value = companyKey;

                    ApplicationSettingsDataSet.ApplicationSettingsRow companyNameRow = appSettings.FindBySettingName("Company Name");

                    if(companyNameRow == null)
                        appSettings.AddApplicationSettingsRow("Company Name", companyName);
                    else if(companyNameRow.Value != companyName)
                        companyNameRow.Value = companyName;

                    ta.Update(appSettings);
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating application settings in database.");
            }
        }

        #endregion
    }
}