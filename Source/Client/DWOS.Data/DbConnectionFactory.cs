using System.Data.SqlClient;

namespace DWOS.Data
{
    /// <summary>
    /// Generates <see cref="SqlConnection"/> instances using a default
    /// connection string.
    /// </summary>
    internal static class DbConnectionFactory
    {
        /// <summary>
        /// Returns a new connection instance.
        /// </summary>
        /// <returns></returns>
        public static SqlConnection NewConnection() =>
            new SqlConnection(Properties.Settings.Default.DWOSDataConnectionString);
    }
}
