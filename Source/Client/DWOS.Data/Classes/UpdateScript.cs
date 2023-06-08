using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DWOS.Data
{
    /// <summary>
    /// Represents a database update script embedded in DWOS.Data.
    /// </summary>
    public sealed class UpdateScript
    {
        #region Fields

        private readonly Lazy<string> _content;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the content of the update file.
        /// </summary>
        public string Content
        {
            get
            {
                return _content.Value;
            }
        }

        /// <summary>
        /// Gets the version represented by the update file.
        /// </summary>
        public Version UpgradeVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the manifest resource name of the script.
        /// </summary>
        public string ManifestResourceName
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateScript"/> class.
        /// </summary>
        /// <param name="manifestResourceName"></param>
        public UpdateScript(string manifestResourceName)
        {
            if (string.IsNullOrEmpty(manifestResourceName))
            {
                throw new ArgumentNullException(nameof(manifestResourceName));
            }
            ManifestResourceName = manifestResourceName;

            const string updateRegexString = @"Upgrade_((?:\d|\.)+)";
            var upgradeRegex = new Regex(updateRegexString);

            var upgradeMatches = upgradeRegex.Match(manifestResourceName.Replace(".sql", string.Empty));

            if (!upgradeMatches.Success)
            {
                const string errorMsg = "Manifest resource name is not in the correct format.";
                throw new ArgumentException(errorMsg, nameof(manifestResourceName));
            }

            var versionString = upgradeMatches.Groups[1].Value;
            UpgradeVersion = new Version(versionString);

            _content = new Lazy<string>(
                () =>
                {
                    var sqlAssembly = GetType().Assembly;
                    string content = null;

                    using (var resStream = sqlAssembly.GetManifestResourceStream(ManifestResourceName))
                    {
                        if (resStream != null)
                        {
                            using (var reader = new StreamReader(resStream))
                            {
                                content = reader.ReadToEnd();
                            }
                        }
                    }

                    return content;
                });
        }

        /// <summary>
        /// Gets all update scripts embedded within the DWOS.Data assembly
        /// containing <see cref="UpdateScript"/>.
        /// </summary>
        /// <remarks>
        /// This does not pull scripts embedded within other assemblies.
        /// </remarks>
        /// <returns>List of update scripts, sorted by version in ascending order.</returns>
        public static IList<UpdateScript> FromEmbeddedResources()
        {
            var dataAssembly = typeof(UpdateScript).Assembly;

            var returnList = new List<UpdateScript>();

            var resourcePrefix = dataAssembly.GetName().Name + ".SQL";

            foreach (var resourceName in dataAssembly.GetManifestResourceNames())
            {
                if (resourceName.StartsWith(resourcePrefix))
                {
                    returnList.Add(new UpdateScript(resourceName));
                }
            }

            returnList.Sort((s1, s2) => s1.UpgradeVersion.CompareTo(s2.UpgradeVersion));
            return returnList;
        }

        #endregion
    }
}
