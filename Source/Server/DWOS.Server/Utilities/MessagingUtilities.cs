using DWOS.Data;
using DWOS.Shared.Utilities;
using NLog;
using nsoftware.IPWorks;
using System;
using System.Collections.Generic;
using System.IO;

namespace DWOS.Server.Utilities
{
    internal class MessagingUtilities
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Private <see cref="ApplicationSettings"/> instance that can be
        /// refreshed independently of
        /// <see cref="ApplicationSettings.Current"/>.
        /// </summary>
        private static readonly ApplicationSettings _appSettings;

        /// <summary>Object to acquire lock for when refreshing settings.</summary>
        /// <remarks>
        /// Be sure not to acquire a lock on this then require a lock on
        /// <see cref="_mailerLock"/> as it can create a deadlock.
        /// </remarks>
        private static readonly object _settingsRefreshLock = new object();

        /// <summary>
        /// The time to wait between refreshes.
        /// </summary>
        private static readonly TimeSpan _timeBetweenRefreshes = TimeSpan.FromSeconds(15);

        /// <summary>
        /// Date/time when settings were last refreshed.
        /// </summary>
        /// <remarks>
        /// Used with <see cref="_timeBetweenRefreshes"/> to make sure that
        /// settings are regularly refreshed without refreshing them every
        /// time you call <see cref="BeginProcessingEmails"/>.
        /// </remarks>
        private static DateTime _lastSettingsRefresh;

        private readonly object _mailerLock = new object();
        private DateTime? _lastSuccessfulSend;
        private readonly Lazy<Htmlmailer> _mailerLazy = new Lazy<Htmlmailer>(GenerateMailer);

        #endregion

        #region Properties

        public string EmailPickUpFolderName { get; }

        public string EmailPickupDirectory { get; }

        #endregion

        #region Methods

        static MessagingUtilities()
        {
            _appSettings = ApplicationSettings.NewApplicationSettings();
            _appSettings.UpdateDatabaseConnection(ServerSettings.Default.DBConnectionString);
            _lastSettingsRefresh = DateTime.Now;
        }

        public MessagingUtilities(string emailPickUpFolderName)
        {
            if (string.IsNullOrEmpty(emailPickUpFolderName))
            {
                throw new ArgumentException("Folder name cannot be null or empty.", nameof(emailPickUpFolderName));
            }

            EmailPickUpFolderName = emailPickUpFolderName;
            EmailPickupDirectory = Path.Combine(FileSystem.CommonAppDataPath(), EmailPickUpFolderName);

            if (!Directory.Exists(EmailPickupDirectory))
            {
                Directory.CreateDirectory(EmailPickupDirectory);
            }
        }

        public void QueueEmail(MessageInfo message)
        {
            if(_appSettings.DisableNotifications)
            {
                _log.Info("Notifications are disabled, message NOT sent.");
                return;
            }

            lock(_mailerLock)
            {
                var mailer = _mailerLazy.Value;
                if (mailer == null)
                {
                    return;
                }

                //clear previous email sent
                mailer.ResetHeaders();
                mailer.Attachments.Clear();
                mailer.MessageRecipients.Clear();

                mailer.From = message.FromAddress.ToString();
                mailer.SendTo = string.Join(",", message.ToAddresses.ConvertAll(e => e.ToString()));
                mailer.Subject = message.Subject;

                if(message.IsHtml)
                    mailer.MessageHTML = message.Body;
                else
                    mailer.MessageText = message.Body;

                foreach(var fileName in message.Attachments)
                    mailer.AddAttachment(fileName);

                mailer.Queue(this.EmailPickupDirectory);
            }
        }

        public void BeginProcessingEmails()
        {
            lock (_mailerLock)
            {
                try
                {
                    var mailer = _mailerLazy.Value;

                    if (mailer == null)
                    {
                        return;
                    }

                    // Refresh settings (if needed)
                    lock (_settingsRefreshLock)
                    {
                        if (DateTime.Now - _lastSettingsRefresh > _timeBetweenRefreshes)
                        {
                            _lastSettingsRefresh = DateTime.Now;
                            _appSettings.ReloadSettings();
                        }
                    }

                    // Update mailer from settings
                    if (mailer.User != _appSettings.EmailUserName)
                    {
                        mailer.User = _appSettings.EmailUserName;
                        _lastSuccessfulSend = null;
                        _log.Info("Updated user name");
                    }

                    if (mailer.Password != _appSettings.EmailPassword)
                    {
                        mailer.Password = _appSettings.EmailPassword;
                        _lastSuccessfulSend = null;
                        _log.Info("Updated password.");
                    }

                    if (mailer.MailServer != _appSettings.EmailSMTPServer)
                    {
                        mailer.MailServer = _appSettings.EmailSMTPServer;
                        _lastSuccessfulSend = null;
                        _log.Info("Updated mail server.");
                    }

                    var authMechanism = AuthMechanism();
                    if (mailer.AuthMechanism != authMechanism)
                    {
                        mailer.AuthMechanism = authMechanism;
                        _lastSuccessfulSend = null;
                        _log.Info("Updated authentication mechanism.");
                    }

                    var sslStartMode = SSLStartMode();
                    if (mailer.SSLStartMode != sslStartMode)
                    {
                        mailer.SSLStartMode = sslStartMode;
                        _lastSuccessfulSend = null;
                        _log.Info("Updated SSL start mode.");
                    }

                    var port = EmailPort();
                    if (mailer.MailPort != port)
                    {
                        mailer.MailPort = port;
                        _lastSuccessfulSend = null;
                        _log.Info("Updated mail port.");
                    }

                    // Check access to email server
                    var sendEmail = true;
                    if (!_lastSuccessfulSend.HasValue)
                    {
                        sendEmail = CanConnect(mailer);
                    }

                    // Send email unless the connection test failed.
                    if (sendEmail)
                    {
                        mailer.ProcessQueue(EmailPickupDirectory);
                        _lastSuccessfulSend = DateTime.Now;
                    }
                }
                catch (IPWorksHtmlmailerException exc)
                {
                    // Connection failed due to an internal error.
                    var errorMsg = $"Error processing email queue: {exc.Code}";
                    _log.Error(exc.InnerException ?? exc, errorMsg);
                    _lastSuccessfulSend = null;
                }
                catch(Exception exc)
                {
                    // Connection failed due to an unknown error.
                    const string errorMsg = "Error processing email queue.";
                    _lastSuccessfulSend = null;
                    _log.Error(exc, errorMsg);
                }
            }
        }

        /// <summary>
        /// Quickly send a single email.
        /// </summary>
        /// <param name="toAddress">To address.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        public static void QuickSendEmail(string toAddress, string subject, string message)
        {
            try
            {
                var messager = new MessagingUtilities("Email");
                var messageInfo = new MessageInfo()
                {
                    Body = message,
                    FromAddress = new EmailAddress() { Address = Properties.Settings.Default.DWOSSupportEmail, DisplayName = "DWOS Support" },
                    ToAddresses = new List<EmailAddress>() { new EmailAddress() { Address = toAddress }, new EmailAddress() { Address = Properties.Settings.Default.DWOSSupportEmail, DisplayName = "DWOS Support" } },
                    Subject = subject
                };

                messager.QueueEmail(messageInfo);
                messager.BeginProcessingEmails();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error sending email.");
            }
        }

        private static Htmlmailer GenerateMailer()
        {
            if (_appSettings.EmailSMTPServer == null)
            {
                _log.Info("SMTP Settings are not specified, messages NOT sent.");
                return null;
            }

            var mailer = new Htmlmailer();
            mailer.User = _appSettings.EmailUserName;
            mailer.Password = _appSettings.EmailPassword;
            mailer.MailServer = _appSettings.EmailSMTPServer;
            mailer.AuthMechanism = AuthMechanism();
            mailer.SSLStartMode = SSLStartMode();
            mailer.RuntimeLicense = "31504E4241413153554252413153554238414244343530335800000000000000000000000000000048333157504148300000324E525837453044355857520000";

            mailer.Config("KeepQueue =True"); //set to mark messages as sent in the queue so they are not deleted
            mailer.OnEndTransfer += mailer_OnEndTransfer;
            mailer.OnError += mailer_OnError;
            mailer.OnConnectionStatus += mailer_OnConnectionStatus;
            mailer.MailPort = EmailPort();
            return mailer;
        }

        private static HtmlmailerAuthMechanisms AuthMechanism()
        {
            var authString = _appSettings.EmailAuthentication;
            var authMech = HtmlmailerAuthMechanisms.amUserPassword;
            if (!string.IsNullOrEmpty(authString))
            {
                Enum.TryParse(authString, out authMech);
            }

            return authMech;
        }

        private static HtmlmailerSSLStartModes SSLStartMode()
        {
            var startModeString = _appSettings.EmailSslStartMode;
            var startMode = HtmlmailerSSLStartModes.sslAutomatic;
            if (!string.IsNullOrEmpty(startModeString))
            {
                Enum.TryParse(startModeString, out startMode);
            }

            return startMode;
        }

        private static int EmailPort()
        {
            int port = 25;
            int.TryParse(_appSettings.EmailPort, out port);
            return port;
        }

        private static bool CanConnect(Htmlmailer mailer)
        {
            try
            {
                mailer.Connect();
                mailer.Disconnect();
                return true;
            }
            catch (Exception exc)
            {
                _log.Warn(exc, "Failed to connect to mail server.");
                return false;
            }
        }

        #endregion

        #region Events

        private static void mailer_OnEndTransfer(object sender, HtmlmailerEndTransferEventArgs e)
        {
            _log.Debug("On End Transfer " + e.Direction);
        }

        private static void mailer_OnError(object sender, HtmlmailerErrorEventArgs e)
        {
            _log.Info("Mailer Send Error: {0} {1}", e.ErrorCode, e.Description);
        }

        private static void mailer_OnConnectionStatus(object sender, HtmlmailerConnectionStatusEventArgs e)
        {
            _log.Debug("Mailer Connection Status: {0} {1}", e.ConnectionEvent, e.StatusCode);
        }

        #endregion
    }
}