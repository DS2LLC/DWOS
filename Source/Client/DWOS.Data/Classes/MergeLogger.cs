using NLog;
using System;
using System.Data;
using System.Linq;

namespace DWOS.Data
{
    /// <summary>
    /// Logs detailed information for merging two <see cref="DataTable"/> instances.
    /// </summary>
    public class MergeLogger
    {
        #region Fields

        private static readonly ILogger ClassLogger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the client data table for this instance.
        /// </summary>
        public DataTable Client { get; }

        /// <summary>
        /// Gets the server data table for this instance.
        /// </summary>
        public DataTable Server { get; }

        /// <summary>
        /// Gets the identification column name for both data tables.
        /// </summary>
        public string IdColumn { get; }

        /// <summary>
        /// Gets or sets the logger for this instance.
        /// </summary>
        public ILogger Logger { get; set; } = ClassLogger;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeLogger"/> class.
        /// </summary>
        /// <param name="client">The client data table.</param>
        /// <param name="server">The server data table.</param>
        /// <param name="idColumn">The identification column name.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="client"/> or <paramref name="server"/> is null or
        /// <paramref name="idColumn"/> is null or empty.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="client"/> or <paramref name="server"/> do not contain
        /// the column represented by <paramref name="idColumn"/>.
        /// </exception>
        public MergeLogger(DataTable client, DataTable server, string idColumn)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (server == null)
            {
                throw new ArgumentNullException(nameof(server));
            }

            if (string.IsNullOrEmpty(idColumn))
            {
                throw new ArgumentNullException(nameof(idColumn));
            }

            if (!client.Columns.Contains(idColumn))
            {
                throw new ArgumentException($@"Schema does not contain {idColumn}", nameof(client));
            }

            if (!server.Columns.Contains(idColumn))
            {
                throw new ArgumentException($@"Schema does not contain {idColumn}", nameof(server));
            }

            Client = client;
            Server = server;
            IdColumn = idColumn;
        }

        /// <summary>
        /// Logs every value for each row to merge.
        /// </summary>
        public void LogValues()
        {
            const string separator = ",";

            foreach (var rowToMerge in Server.Rows.OfType<DataRow>())
            {
                var id = rowToMerge[IdColumn];
                Logger.Info($"---Merge info for {Client.TableName} {id}---");

                var original = Client.Select($"{IdColumn} = {id}").FirstOrDefault();

                Logger.Info(original == null
                    ? "Cannot find original row"
                    : $"Client: {string.Join(separator, original.ItemArray)}");

                Logger.Info($"Server: {string.Join(separator, rowToMerge.ItemArray)}");
            }
        }

        #endregion
    }
}
