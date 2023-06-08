using DWOS.Data;
using System;
using System.Drawing;

namespace DWOS.UI.Utilities
{
    internal interface IAuthenticationProvider : IDisposable
    {
        /// <summary>
        ///   Gets or sets a value indicating whether this <see cref="IAuthenticationProvider" /> is enabled.
        /// </summary>
        /// <value> <c>true</c> if enabled; otherwise, <c>false</c> . </value>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets the provider thumbnail.
        /// </summary>
        /// <value>The provider thumbnail.</value>
        Image ProviderThumbnail { get; }

        /// <summary>
        ///   Gets the userId of the user that was authenticated.
        /// </summary>
        /// <returns> </returns>
        int? GetUserID();

        LoginType LogInType { get; }
    }
}
