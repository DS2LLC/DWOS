using System;

namespace DWOS.Shared
{
    /// <summary>
    /// Assembly attribute for storing a Raygun API key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class RaygunApiKeyAttribute: Attribute
    {
        /// <summary>
        /// Gets or sets the API key for this instance.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RaygunApiKeyAttribute"/> class.
        /// </summary>
        public RaygunApiKeyAttribute() { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RaygunApiKeyAttribute"/> class.
        /// </summary>
        /// <param name="apiKey"></param>
        public RaygunApiKeyAttribute(string apiKey) { this.ApiKey = apiKey; }
    }
}
