using System;
using System.Linq;
using System.Threading;
using DWOS.Data;
using nsoftware.IPWorks;

namespace DWOS.UI.Support
{
    public class EmailSupportClient : ISupportTicketClient
    {
        /// <inheritdoc />
        public void AddTicket(Ticket ticket)
        {
            ThreadPool.QueueUserWorkItem(t => BeginAddTicket((Ticket)t), ticket);
        }

        private void BeginAddTicket(Ticket ticket)
        {
            using(var htmlMailer = new Htmlmailer())
            {
                htmlMailer.User = ApplicationSettings.Current.EmailUserName;
                htmlMailer.Password = ApplicationSettings.Current.EmailPassword;
                htmlMailer.MailServer = ApplicationSettings.Current.EmailSMTPServer;
                htmlMailer.AuthMechanism = Enum.TryParse<HtmlmailerAuthMechanisms>(ApplicationSettings.Current.EmailAuthentication, out var authMechanism) ? authMechanism : HtmlmailerAuthMechanisms.amUserPassword;
                htmlMailer.SSLStartMode = Enum.TryParse<HtmlmailerSSLStartModes>(ApplicationSettings.Current.EmailSslStartMode, out var sslStartMode) ? sslStartMode : HtmlmailerSSLStartModes.sslAutomatic;

                htmlMailer.From = ticket.FromAddress;
                htmlMailer.SendTo = "mail@mail.com";
                htmlMailer.Subject = ticket.Subject + $" - User: { ticket.UserName}";
                htmlMailer.MessageText = ticket.Message;

                htmlMailer.Connect();

                htmlMailer.Send();

                htmlMailer.Disconnect();
            }
        }
    }
}