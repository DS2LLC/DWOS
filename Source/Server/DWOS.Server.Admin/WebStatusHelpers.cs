using System;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using DWOS.Data;
using Microsoft.Web.Administration;
using NLog;

namespace DWOS.Server.Admin
{
    public static class WebStatusHelpers
    {
        #region Fields

        private const string WEB_DB_CONNECTION_NAME = "DWOS.Data.Properties.Settings.DWOSDataConnectionString";

        #endregion

        #region Properties

        public static bool IsPortalInstalled => CheckPortalInstalled();

        #endregion

        #region Methods

        private static bool IsConnectionValid(string connection)
        {
            var isValid = false;
            try
            {
                using (var conn = new SqlConnection(connection))
                {
                    conn.Open();

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText =
                            "SELECT [Value] FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion'";

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isValid = true;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error validating db connection for website.");
            }

            return isValid;
        }

        public static UpdateResult UpdatePortalConfiguration()
        {
            return UpdatePortalConfiguration(ServerSettings.Default.DBConnectionString);
        }

        public static UpdateResult UpdatePortalConfiguration(string connectionString)
        {
            try
            {
                var customerPortalServer = string.IsNullOrWhiteSpace(ServerSettings.Default.CustomerPortalServer)
                    ? "localhost"
                    : ServerSettings.Default.CustomerPortalServer;

                using (var server = ServerManager.OpenRemote(customerPortalServer))
                {
                    if (server == null || string.IsNullOrWhiteSpace(ServerSettings.Default.CustomerPortalWebSite))
                    {
                        return new UpdateResult
                        {
                            Success = false,
                            ErrorMessage = "Could not locate Portal."
                        };
                    }

                    string webSite;
                    string webApp;

                    var updateSuccessful = false;
                    if (ServerSettings.Default.GetCustomerPortalWebSiteAndApp(out webSite, out webApp))
                    {
                        var site = server.Sites.FirstOrDefault(s => s.Name == webSite);

                        var app = site?.Applications.FirstOrDefault(a => a.Path == @"/" + webApp);

                        if (app != null)
                        {
                            var config = app.GetWebConfiguration();

                            foreach (
                                var configCollection in config.GetSection("connectionStrings").GetCollection())
                            {
                                if (configCollection.Attributes["name"].Value.ToString() ==
                                    WEB_DB_CONNECTION_NAME)
                                {
                                    configCollection.SetAttributeValue("connectionString",
                                        connectionString);
                                    server.CommitChanges();

                                    updateSuccessful = true;
                                }
                            }


                        }
                    }
                    return new UpdateResult
                    {
                        Success = updateSuccessful,
                        ErrorMessage = updateSuccessful ? null : "Could not find configuration settings."
                    };
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error updating portal connection string.");

                return new UpdateResult
                {
                    Success = false,
                    ErrorMessage = "Error occurred while locating configuration settings. Please check the server logs."
                };
            }
        }


        public static bool IsServerValid()
        {
            try
            {
                var isServerValid = false;

                var portalServerName = string.IsNullOrWhiteSpace(ServerSettings.Default.CustomerPortalServer)
                    ? "localhost"
                    : ServerSettings.Default.CustomerPortalServer;

                using (var server = ServerManager.OpenRemote(portalServerName))
                {
                    if (server != null && !string.IsNullOrWhiteSpace(ServerSettings.Default.CustomerPortalWebSite))
                    {
                        string webSite;
                        string webApp;

                        if (ServerSettings.Default.GetCustomerPortalWebSiteAndApp(out webSite, out webApp))
                        {
                            var site = server.Sites.FirstOrDefault(s => s.Name == webSite);

                            var app = site?.Applications.FirstOrDefault(a => a.Path == @"/" + webApp);

                            if (app != null)
                            {
                                var config = app.GetWebConfiguration();

                                foreach (
                                    var configCollection in config.GetSection("connectionStrings").GetCollection())
                                {
                                    if (configCollection.Attributes["name"].Value.ToString() ==
                                        WEB_DB_CONNECTION_NAME)
                                    {
                                        var connectionString =
                                            configCollection.Attributes["connectionString"].Value.ToString();

                                        isServerValid = IsConnectionValid(connectionString);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                return isServerValid;
            }
            catch (COMException exc)
            {
                LogManager.GetCurrentClassLogger().Info(exc, "Server may not be installed.");
                return false;
            }
        }

        private static bool CheckPortalInstalled()
        {
            try
            {
                var portalServerName = string.IsNullOrWhiteSpace(ServerSettings.Default.CustomerPortalServer)
                    ? "localhost"
                    : ServerSettings.Default.CustomerPortalServer;

                using (var server = ServerManager.OpenRemote(portalServerName))
                {
                    if (server != null && !string.IsNullOrWhiteSpace(ServerSettings.Default.CustomerPortalWebSite))
                    {
                        string webSite;
                        string webApp;

                        if (ServerSettings.Default.GetCustomerPortalWebSiteAndApp(out webSite, out webApp))
                        {
                            var site = server.Sites.FirstOrDefault(s => s.Name == webSite);

                            var app = site?.Applications.FirstOrDefault(a => a.Path == @"/" + webApp);

                            if (app != null)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            catch (COMException exc)
            {
                LogManager.GetCurrentClassLogger().Info(exc, "Server may not be installed.");
                return false;
            }
        }

        #endregion

        #region UpdateResult

        public class UpdateResult
        {
            public bool Success { get; set; }

            public string ErrorMessage { get; set; }
        }

        #endregion
    }
}