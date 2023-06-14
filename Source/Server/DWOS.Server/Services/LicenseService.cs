using System;
using System.ServiceModel;
using DWOS.Data;
using DWOS.LicenseManager;
using DWOS.Server.Tasks;
using DWOS.Shared;
using NLog;

namespace DWOS.Server.Services
{
    /// <summary>
    ///   Client License Service provides the communication to local DWOS Clients for user license activation and status. It provides a WCF interface for the License Manager.
    /// </summary>
    public class LicenseService: ILicenseService
    {
        #region ILicenseService Members

        public bool CheckOutLicense(string computerName, int userId, string userName, Guid uid)
        {
            return LicenseManager.LicenseManager.Default.CheckOutLicense(computerName, userId, userName, uid);
        }

        public void CheckInLicense(Guid uid)
        {
            LicenseManager.LicenseManager.Default.CheckInLicense(uid);
        }

        public bool KeepLicenseAlive(Guid uid)
        {
            return LicenseManager.LicenseManager.Default.KeepAlive(uid);
        }

        public LicenseSummary GetLicenseSummary()
        {
            return LicenseManager.LicenseManager.Default.GetLicenseSummary();
        }

        public ApplicationInfo GetApplicationInfo()
        {
            return new ApplicationInfo{DatabaseConnection = ServerSettings.Default.DBConnectionString, ServerVersion = About.ApplicationVersion, MinimumClientVersion = ServerSettings.Default.MinimumClientVersion };
        }

        /// <summary>
        ///   Validates the license activation by contacting home server to check status.
        /// </summary>
        public void ValidateLicenseActivation()
        {
            try
            {
                var task = new LicenseCheckTask();
                task.Execute(null).Wait();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error re-checking license activation.");
            }
        }

        /// <summary>
        /// Reloads the company info from the registry settings. Used in case someone else updates them (i.e. like the server admin utility).
        /// </summary>
        public void ReloadCompanyInfo()
        {
            try
            {
                //reload registry settings
                ServerSettings.Default.ReLoad();

                ApplicationSettings.Current.PreLoadSettings();

                //reload license status
                var licenseCheck = new LicenseCheckTask();
                licenseCheck.Execute(null).Wait();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error reloading company info.");
            }
            
        }

        #endregion
    }

    [ServiceContract]
    public interface ILicenseService
    {
        [OperationContract]
        bool CheckOutLicense(string computerName, int userId, string userName, Guid uid);

        [OperationContract]
        void CheckInLicense(Guid uid);

        [OperationContract]
        bool KeepLicenseAlive(Guid uid);

        [OperationContract]
        LicenseSummary GetLicenseSummary();

        [OperationContract]
        ApplicationInfo GetApplicationInfo();

        [OperationContract]
        void ValidateLicenseActivation();

        [OperationContract]
        void ReloadCompanyInfo();
    }

    public class ApplicationInfo
    {
        public string DatabaseConnection { get; set; }
        public string ServerVersion { get; set; }
        public string MinimumClientVersion { get; set; }
    }
}