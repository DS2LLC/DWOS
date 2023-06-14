using System.Collections.Generic;

namespace DWOS.Server.Utilities
{
    /// <summary>
    /// Represents an email to send.
    /// </summary>
    internal class MessageInfo
    {
        /// <summary>
        /// Gets or sets the 'from address' for this instance.
        /// </summary>
        public EmailAddress FromAddress { get; set; } =
            EmailAddress.Default;

        /// <summary>
        /// Gets or sets the list of 'to addresses' for this instance.
        /// </summary>
        public List<EmailAddress> ToAddresses { get; set; } =
            new List<EmailAddress>();

        /// <summary>
        /// Gets or sets the subject line for this instance.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body for this instance.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if <see cref="Body"/> uses HTML.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="Body"/> is in HTML;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool IsHtml { get; set; }

        /// <summary>
        /// Gets or sets the list of file attachments for this instance.
        /// </summary>
        public List<string> Attachments { get; set; } =
            new List<string>();
    }
}
