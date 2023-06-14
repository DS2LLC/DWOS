using DWOS.Data;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    internal class ServerManager : IServerManager
    {
        #region IServerManager Members

        public DwosServerInfo ServerInfo { get; private set; }

        public void Initialize(DwosServerInfo serverInfo)
        {
            ServerInfo = serverInfo;
        }

        #endregion
    }
}
