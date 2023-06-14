using DWOS.Data;
using DWOS.Shared.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace DWOS.Server.Admin.SQL
{
    /// <summary>
    /// Upgrades an existing DWOS database using scripts retrieved by 
    /// <see cref="UpdateScript.FromEmbeddedResources"/>.
    /// </summary>
    internal sealed class DbUpgrader
    {
        #region Properties

        /// <summary>
        /// Gets the notifier for this instance.
        /// </summary>
        public IUserNotifier Notifier
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="DbUpgrader"/> class.
        /// </summary>
        /// <param name="notifier">Notifier to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="notifier"/> is null.
        /// </exception>
        public DbUpgrader(IUserNotifier notifier)
        {
            if (notifier == null)
            {
                throw new ArgumentNullException(nameof(notifier));
            }

            Notifier = notifier;
        }

        /// <summary>
        /// Upgrades the database from one version to another.
        /// </summary>
        /// <param name="conn">An open SQL connection.</param>
        /// <param name="fromVersion"></param>
        /// <param name="toVersion"></param>
        /// <returns>Results of the upgrade.</returns>
        public UpgradeResult UpgradeDb(SqlConnection conn, Version fromVersion, Version toVersion)
        {
            Notifier.ShowNotification("Begin upgrade process.");

            var files = UpdateScript.FromEmbeddedResources()
                .Where(sf => sf.UpgradeVersion > fromVersion && sf.UpgradeVersion <= toVersion);

            var hasErrors = false;
            Version upgradeToVersion = null;

            //execute each file to upgrade the database
            foreach (var sqlScriptFile in files)
            {
                Notifier.ShowNotification("Upgrading to {0}...".FormatWith(sqlScriptFile.UpgradeVersion));
                var transaction = conn.BeginTransaction("DWOSUpgrade_" + sqlScriptFile.UpgradeVersion);

                try
                {
                    using (var command = conn.CreateCommand())
                    {
                        var sql = sqlScriptFile.Content;

                        if (!string.IsNullOrEmpty(sql))
                        {
                            var sqlCmds = SplitBatchIntoCommands(sql);
                            foreach (var sqlCmd in sqlCmds)
                            {
                                if (!string.IsNullOrWhiteSpace(sqlCmd))
                                {
                                    command.Transaction = transaction;
                                    command.CommandText = sqlCmd;
                                    command.ExecuteNonQuery();
                                }
                            }

                            if (transaction.Connection != null)
                                transaction.Commit();

                            upgradeToVersion = sqlScriptFile.UpgradeVersion;
                        }
                    }
                }
                catch (Exception exc)
                {
                    transaction.Rollback();
                    LogManager.GetCurrentClassLogger().Error(exc, "Error upgrading database with script {0}.".FormatWith(sqlScriptFile));
                    hasErrors = true;
                    break;
                }

                Notifier.ShowNotification("... upgraded to {0} completed.".FormatWith(sqlScriptFile.UpgradeVersion));
            }

            return new UpgradeResult()
            {
                HasErrors = hasErrors,
                UpgradeVersion = upgradeToVersion
            };
        }

        private List<string> SplitBatchIntoCommands(string batchScript)
        {
            var sqlScriptSplitRegEx = new Regex(@"^\s*GO\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
            var scripts = sqlScriptSplitRegEx.Split(batchScript);
            return new List<string>(scripts);
        }

        #endregion

        #region UpgradeResult

        /// <summary>
        /// Contains results of a database upgrade.
        /// </summary>
        internal sealed class UpgradeResult
        {
            #region Properties

            /// <summary>
            /// Gets or sets the value indicating existence of upgrade errors.
            /// </summary>
            public bool HasErrors
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the new version of the database.
            /// </summary>
            public Version UpgradeVersion
            {
                get;
                set;
            }

            #endregion
        }

        #endregion
    }
}
