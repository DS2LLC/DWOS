using DWOS.Data;
using DWOS.Shared.Utilities;
using DWOS.UI.Utilities;
using System.Threading;

namespace DWOS.UI.DesignTime
{
    internal static class DesignTimeUtilities
    {
        #region Fields

        /// <summary>
        /// Accessed atomically to determine if this class has setup
        /// dependency injection.
        /// </summary>
        private static int SETUP_DI = 0;

        #endregion

        #region Methods

        public static void SetupDependencyInjection()
        {
            if (Interlocked.Exchange(ref SETUP_DI, 1) != 0)
            {
                return;
            }

            var connInfo = new DwosServerInfo
            {
                ServerAddress = "ds2-dwos-dev-1",
                ServerPort = 8080
            };

            DependencyContainer.Register<DwosServerInfoProvider>(() => new DwosServerInfoProvider(connInfo));

            DependencyContainer.Register<IDwosApplicationSettingsProvider>(() => new FakeSettingsProvider());
        }

        #endregion
    }
}
