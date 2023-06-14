using DWOS.Data;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    /// <summary>
    /// Manages server information.
    /// </summary>
    interface IServerManager
    {
        /// <summary>
        /// Gets the <see cref="DwosServerInfo"/> for this instance.
        /// </summary>
        DwosServerInfo ServerInfo { get; }

        /// <summary>
        /// Initializes server information.
        /// </summary>
        /// <param name="serverInfo"></param>
        void Initialize(DwosServerInfo serverInfo);
    }
}
