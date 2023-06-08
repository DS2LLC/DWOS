using System;

namespace DWOS.Shared.Utilities
{
    public class NotificationMessage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the notification ID for this instance.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the notification level for this instance.
        /// </summary>
        public NotificationLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the message for this instance.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the timestamp for this instance.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        #endregion

        #region Methods
        public NotificationMessage()
        {
            this.Level = NotificationLevel.Info;
            this.TimeStamp = DateTime.Now;
        }

        #endregion
    }

}
