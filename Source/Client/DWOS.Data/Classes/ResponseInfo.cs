namespace DWOS.Data
{
    /// <summary>
    /// Contains an object along with a detailed description of it.
    /// </summary>
    /// <remarks>
    /// <see cref="bool"/> is regularly used as <typeparamref name="T"/>. In this
    /// case, the value represents a success/failure value, where success is
    /// <c>true</c> and failure is <c>false</c>.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class ResponseInfo<T>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the response object for this instance.
        /// </summary>
        public T Response { get; set; }

        /// <summary>
        /// Gets or sets the description for this instance.
        /// </summary>
        public string Description { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of <see cref="ResponseInfo{T}"/>
        /// without a description.
        /// </summary>
        /// <param name="response"></param>
        public ResponseInfo(T response) { this.Response = response; }

        /// <summary>
        /// Initializes a new instance of <see cref="ResponseInfo{T}"/>.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="description"></param>
        public ResponseInfo(T response, string description)
        {
            this.Response = response;
            this.Description = description;
        }

        #endregion
    }
}
