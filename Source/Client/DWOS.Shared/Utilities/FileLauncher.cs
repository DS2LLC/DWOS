using NLog;
using System;
using System.Diagnostics;
using System.IO;

namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// Launches files using the default applications for their extensions.
    /// </summary>
    public class FileLauncher
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private Action<Exception, string> _errorHandler;

        #endregion

        #region Methods

        /// <summary>
        /// Ininitializes a new instance of this class; please use factory method instead.
        /// </summary>
        private FileLauncher()
        {
        }

        /// <summary>
        /// Returns a new instance of the <see cref="FileLauncher"/> class.
        /// </summary>
        /// <returns>New instance.</returns>
        public static FileLauncher New()
        {
            return new FileLauncher();
        }

        /// <summary>
        /// Sets the handler for errors.
        /// </summary>
        /// <param name="errorHandler">Method that shows error messages.</param>
        /// <returns>This instance.</returns>
        public FileLauncher HandleErrorsWith(Action<Exception, string> errorHandler)
        {
            _errorHandler = errorHandler;
            return this;
        }

        /// <summary>
        /// Launches a file using its default application.
        /// </summary>
        /// <param name="fileName">Name of file; can be absolute or relative..</param>
        public void Launch(string fileName)
        {
            if (_errorHandler == null)
            {
                throw new InvalidOperationException("ShowErrorsWith must be set.");
            }

            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                bool fileSuccessfullyLaunched = false;

                try
                {
                    Process.Start(fileName);
                    fileSuccessfullyLaunched = true;
                }
                catch (Exception exc)
                {
                    _log.Warn(exc, "Error starting process for: {0}", fileName);
                }

                if (!fileSuccessfullyLaunched)
                {
                    // Try to open its folder with Explorer
                    try
                    {
                        ProcessStartInfo explorerProcessInfo = new ProcessStartInfo()
                        {
                            FileName = "explorer.exe",
                            Arguments = string.Format("/select,\"{0}\"", fileName)
                        };

                        Process.Start(explorerProcessInfo);
                    }
                    catch (Exception exc)
                    {
                        var directoryName = Path.GetDirectoryName(fileName);
                        _log.Warn(exc, "Error starting Explorer process for: {0}", directoryName);
                        _errorHandler(exc, "The folder that contains this file is not found or is unable to be opened.");
                    }
                }
            }
        }

        #endregion
    }
}
