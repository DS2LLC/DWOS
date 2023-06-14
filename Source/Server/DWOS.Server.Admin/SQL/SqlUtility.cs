using System;
using System.Data.SqlClient;

namespace DWOS.Server.Admin.SQL
{
    /// <summary>
    /// Provides a set of static methods for performing miscellaneous
    /// SQL tasks.
    /// </summary>
    internal static class SqlUtility
    {
        /// <summary>
        /// Retrieves a database's version number.
        /// </summary>
        /// <param name="conn"></param>
        /// <returns>
        /// A new <see cref="Version"/> instance if a version was found;
        /// otherwise, <c>null</c>.
        /// </returns>
        public static Version GetVersion(SqlConnection conn)
        {
            Version returnValue = null;

            using (var command = conn.CreateCommand())
            {
                command.CommandText = "SELECT [Value] FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion'";
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        returnValue = new Version(reader[0].ToString());
                }
            }

            return returnValue;
        }
    }
}
