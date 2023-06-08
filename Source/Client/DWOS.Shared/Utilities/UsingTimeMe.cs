using System;
using System.Diagnostics;
using NLog;

namespace DWOS.Shared.Utilities
{
    /// <summary>
    ///     Timer used to time how long an action takes between the time this class is created and it is disposed.
    /// </summary>
    public class UsingTimeMe : IDisposable
    {

        #region Fields

        private readonly Stopwatch _sw;

        #endregion

        #region Properties

        public string Action { get; }

        public LogLevel LogLevel { get; }

        #endregion

        #region Methods

        public UsingTimeMe(string action) : this(action, LogLevel.Debug)
        {
        }

        public UsingTimeMe(string action, LogLevel logLevel)
        {
            Action = action;
            LogLevel = logLevel;

            _sw = Stopwatch.StartNew();

            LogManager.GetCurrentClassLogger().Log(LogLevel, "Starting " + Action);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _sw.Stop();
            var durationSeconds = _sw.Elapsed.TotalSeconds;
            LogManager.GetCurrentClassLogger().Log(LogLevel, $"Finishing {Action}, Duration: {durationSeconds} seconds.");
        }

        #endregion
    }
}
