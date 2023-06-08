using DWOS.Data;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Provides a <see cref="DwosServerInfo"/> instance.
    /// </summary>
    internal sealed class DwosServerInfoProvider
    {
        #region Properties

        /// <summary>
        /// Gets or sets connection info for the server.
        /// </summary>
        public DwosServerInfo ConnectionInfo
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="DwosServerInfoProvider"/> class.
        /// </summary>
        /// <param name="connInfo">Server info to use.</param>
        public DwosServerInfoProvider(DwosServerInfo connInfo)
        {
            ConnectionInfo = connInfo;
        }

        #endregion
    }
}
