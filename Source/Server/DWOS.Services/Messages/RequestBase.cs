namespace DWOS.Services.Messages
{
    /// <summary>
    /// Base for client requests.
    /// </summary>
    public class RequestBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the ID of the user making the request.
        /// </summary>
        public int UserId { get; set; }

        #endregion
    }


}
