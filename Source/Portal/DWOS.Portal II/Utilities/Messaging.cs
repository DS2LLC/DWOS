using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DWOS.Data;
using NLog;
using nsoftware.IPWorks;

namespace DWOS.Portal.Utilities
{
    /// <summary>
    /// Provides a utility method for sending emails.
    /// </summary>
    public static class Messaging
    {
        #region Fields

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="messageInfo">The message info.</param>
        public static async Task<bool> SendMessage(MessageInfo messageInfo)
        {
            Logger.Info("Sending message.");

            try
            {
                await Task.Run(() => DoSendMessage(messageInfo));
                Logger.Info("Successfully sent message.");
                return true;
            }
            catch (Exception exc)
            {
                Logger.Error(exc, "Error sending message.");
                return false;
            }
        }

        private static void DoSendMessage(MessageInfo messageInfo)
        {
            using (var mailer = GenerateMailer())
            {
                if (mailer == null)
                {
                    return;
                }

                mailer.From = messageInfo.FromAddress;
                mailer.SendTo = string.Join(",", messageInfo.ToAddresses);
                mailer.Subject = messageInfo.Subject;

                if (messageInfo.IsHtml)
                {
                    mailer.MessageHTML = messageInfo.Body;
                }
                else
                {
                    mailer.MessageText = messageInfo.Body;
                }

                mailer.Send();
            }
        }

        private static Htmlmailer GenerateMailer()
        {
            var appSettings = ApplicationSettings.Current;
            if (appSettings.EmailSMTPServer == null)
            {
                Logger.Info("SMTP Settings are not specified, messages NOT sent.");
                return null;
            }

            var mailer = new Htmlmailer
            {
                RuntimeLicense = "31504E4241413153554252413153554238414244343530335800000000000000000000000000000048333157504148300000324E525837453044355857520000",
                User = appSettings.EmailUserName,
                Password = appSettings.EmailPassword,
                MailServer = appSettings.EmailSMTPServer,
                AuthMechanism = AuthMechanism(),
                SSLStartMode = SSLStartMode(),
                MailPort = EmailPort()
            };
            return mailer;
        }

        private static HtmlmailerAuthMechanisms AuthMechanism()
        {
            var authString = ApplicationSettings.Current.EmailAuthentication;
            var authMech = HtmlmailerAuthMechanisms.amUserPassword;
            if (!string.IsNullOrEmpty(authString))
            {
                Enum.TryParse(authString, out authMech);
            }

            return authMech;
        }

        private static HtmlmailerSSLStartModes SSLStartMode()
        {
            var startModeString = ApplicationSettings.Current.EmailSslStartMode;
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
            int.TryParse(ApplicationSettings.Current.EmailPort, out port);
            return port;
        }

        #endregion

        #region MessageInfo

        /// <summary>
        /// Represents an email.
        /// </summary>
        public class MessageInfo
        {
            #region Properties

            /// <summary>
            /// Gets or sets from address.
            /// </summary>
            /// <value>
            /// From address.
            /// </value>
            public string FromAddress { get; set; }

            /// <summary>
            /// Gets or sets to addresses.
            /// </summary>
            /// <value>
            /// To addresses.
            /// </value>
            public List<string> ToAddresses { get; set; }

            /// <summary>
            /// Gets or sets the subject.
            /// </summary>
            /// <value>
            /// The subject.
            /// </value>
            public string Subject { get; set; }

            /// <summary>
            /// Gets or sets the body.
            /// </summary>
            /// <value>
            /// The body.
            /// </value>
            public string Body { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is HTML.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is HTML; otherwise, <c>false</c>.
            /// </value>
            public bool IsHtml { get; set; }

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the <see cref="MessageInfo"/> class.
            /// </summary>
            public MessageInfo()
            {
                ToAddresses = new List<string>();
            }

            #endregion
        }

        #endregion
    }
}