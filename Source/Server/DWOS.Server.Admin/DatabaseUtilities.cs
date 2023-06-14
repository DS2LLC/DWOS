using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWOS.Server.Admin
{
    internal static class DatabaseUtilities
    {
        public static Version GetDbVersion(string dbConnectionString)
        {
            var conn = new SqlConnection(dbConnectionString);
            conn.Open();

            using (var command = conn.CreateCommand())
            {
                command.CommandText = "SELECT [Value] FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion'";
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        return new Version(reader[0].ToString());
                }
            }

            return null;
        }
    }
}
