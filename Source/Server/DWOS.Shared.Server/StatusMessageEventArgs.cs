using System;

namespace DWOS.Shared.Server
{
    /// <summary>
    /// Represents data for a status message event.
    /// </summary>
    public class StatusMessageEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the message for this instance.
        /// </summary>
        public string Message { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message"></param>
        public StatusMessageEventArgs(string message)
        {
            Message = message;
        }

        #endregion
    }
}