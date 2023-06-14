using Newtonsoft.Json;
using System;

namespace DWOS.Services.Messages
{
    /// <summary>
    /// Base class for server responses.
    /// </summary>
    public class ResponseBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating if this instance represents a
        /// successful response.
        /// </summary>
        /// <value>
        /// <c>true</c> if the request was successful; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("success", DefaultValueHandling = DefaultValueHandling.Include)]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the error message for this instance.
        /// </summary>
        [JsonProperty("errorMessage", DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
        public string ErrorMessage { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new instance of <see cref="ResponseBase"/> that
        /// indicates an invalid response.
        /// </summary>
        /// <returns></returns>
        public static ResponseBase InvalidData() => Error("Invalid data.");

        /// <summary>
        /// Creates a new instance of <see cref="ResponseBase"/> that
        /// indicates an error.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static ResponseBase Error(string errorMessage)
        {
            return new ResponseBase
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }

        /// <summary>
        /// Creates a new instance of <see cref="ResponseBase"/> that
        /// indicates an exception.
        /// </summary>
        /// <param name="exc"></param>
        /// <returns></returns>
        public static ResponseBase Error(Exception exc) =>
            new ResponseBase
            {
                Success = false,
                ErrorMessage = exc?.Message
            };

        #endregion
    }
}
