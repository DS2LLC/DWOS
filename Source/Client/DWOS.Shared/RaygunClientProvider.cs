using System;
using Mindscape.Raygun4Net;
using NLog;

namespace DWOS.Shared
{
    public static class RaygunClientProvider
    {
        #region Fields

        private static readonly Lazy<RaygunClient> RaygunClientLazy = new Lazy<RaygunClient>(
            () =>
            {
                var apiKey = About.RaygunApiKey;

                if (string.IsNullOrEmpty(apiKey))
                {
                    LogManager.GetCurrentClassLogger()
                        .Warn("*** No Raygun API Key defined. Raygun integrations is disabled.");
                    return null;
                }

                return new RaygunClient(apiKey);
            });

        #endregion

        #region Properties

        /// <summary>
        /// Gets the raygun client.
        /// </summary>
        /// <value>The raygun.</value>
        public static RaygunClient Raygun => RaygunClientLazy.Value;

        #endregion
    }
}
