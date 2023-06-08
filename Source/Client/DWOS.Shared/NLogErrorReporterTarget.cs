using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using Mindscape.Raygun4Net;
using Mindscape.Raygun4Net.Builders;
using NLog.Targets;

namespace NLog
{
    /// <summary>
    /// Target for logging exceptions to Raygun.
    /// </summary>
    [Targets.Target("ErrorReporter")]
    public sealed class ErrorReporterTarget: TargetWithLayout
    {
        #region Fields

        private static DateTime _startUpTime;

        private readonly CircularBuffer<LogEventInfo> _logBuffer = new CircularBuffer<LogEventInfo>(MAX_LOG_HISTORY);
        
        private Exception _lastException;
        private DateTime _lastExceptionOccured;
        private int _lastExceptionCount = 0;
        
        private const int MAX_LOG_HISTORY = 8;
        private const int MINUTES_BETWEEN_FAILURE = 2;
        private const int MAX_DUPLICATE_FAILURES = 10;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the get additional error info function used by external entities to provide additional information for bug reports.
        /// </summary>
        /// <value>The get additional error info.</value>
        public static Func<Dictionary<string, string>> GetAdditionalErrorInfo { get; set; }

        /// <summary>
        /// Gets or sets the get additional user info function used by external entities to provide additional information for bug reports.
        /// </summary>
        /// <value>The get additional user info.</value>
        public static Func<ClientUserInfo> GetAdditionalUserInfo { get; set; }

        #endregion

        #region Methods

        static ErrorReporterTarget()
        {
            _startUpTime    = DateTime.Now;
        }
        
        protected override void Write(LogEventInfo logEvent)
        {
            try
            {
                _logBuffer.Enqueue(logEvent);

#if !DEBUG
                if(logEvent.Level >= LogLevel.Error)
                    this.SendErrorReportRayGun(logEvent);
#endif
            }
            catch (Exception exc)
            {
                EventLog.WriteEntry("Application",
                    $"Failed to queue/log Raygun error.\nMessage:\n{exc.Message}\n\nStack Trace:\n{exc.StackTrace}\n\nOriginal Error:\n{logEvent.Message}",
                    EventLogEntryType.Error);
            }
        }

        private void SendErrorReportRayGun(LogEventInfo logEvent)
        {
            var logEventException = logEvent.Exception;
            if (logEventException == null)
                return;

            // Use TargetInvocationException's inner exception.
            if (logEventException is TargetInvocationException)
            {
                logEventException = logEventException.InnerException ?? logEventException;
            }

            // If AggregateException only contains one exception, use its inner exception.
            if (logEventException is AggregateException aggregateException && aggregateException.InnerExceptions.Count == 1)
            {
                logEventException = aggregateException.InnerExceptions.First();
            }

            if(RaygunClientProvider.Raygun == null)
                return;

            var exceptionID = CalculateExceptionUniqueID(logEventException);
            if (ExcludeException(logEventException))
                return;

            var minutes = DateTime.Now.Subtract(_lastExceptionOccured).TotalMinutes;
            
            //if this is duplicate exception that has occurred in then last 2 minutes; then don't send error report
            if (_lastException != null && _lastException.Message == logEventException.Message && minutes < MINUTES_BETWEEN_FAILURE)
            {
                //AND it has NOT had 10 occurrences within failure timespan 
                if (_lastExceptionCount < MAX_DUPLICATE_FAILURES)
                {
                    _lastExceptionCount++;
                    return;
                }
            }
        
            _lastException        = logEventException;
            _lastExceptionOccured = DateTime.Now;
            _lastExceptionCount   = 0;

            //Create Message
            var msg                 = new Mindscape.Raygun4Net.Messages.RaygunMessage();
            msg.Details.MachineName = System.Environment.MachineName;
            msg.Details.Version     = About.ApplicationVersion;
            msg.Details.Tags        = new List <string> {exceptionID.ToString()};

            //Environment
            msg.Details.Environment = RaygunEnvironmentMessageBuilder.Build();
            
            //Custom Data
            msg.Details.UserCustomData = new Dictionary <string, object>();
            msg.Details.UserCustomData.Add("UserName", System.Environment.UserName);
            msg.Details.UserCustomData.Add("Startup Time", _startUpTime.ToUniversalTime());
            msg.Details.UserCustomData.Add("Error Time", DateTime.Now.ToUniversalTime());
            msg.Details.UserCustomData.Add("CLR Version", System.Environment.Version);

            if (About.ApplicationReleaseDate.HasValue)
            {
                msg.Details.UserCustomData.Add("Release Date", About.ApplicationReleaseDate.Value.ToUniversalTime());
            }
            
            if (GetAdditionalErrorInfo != null)
            {
                var errorInfo = GetAdditionalErrorInfo();

                if (errorInfo != null && errorInfo.Count > 0)
                {
                    foreach (var kvp in errorInfo)
                        msg.Details.UserCustomData.Add(kvp.Key, kvp.Value);
                }
            }

            msg.Details.User = new Mindscape.Raygun4Net.Messages.RaygunIdentifierMessage(System.Environment.UserName);

            if (GetAdditionalUserInfo != null)
            {
                var userInfo = GetAdditionalUserInfo();

                if (userInfo != null)
                {
                    msg.Details.User.Identifier = userInfo.Identifier;
                    msg.Details.User.FirstName = userInfo.FirstName;
                    msg.Details.User.FullName = userInfo.FullName;
                    msg.Details.User.Email = userInfo.EmailAddress;
                    msg.Details.User.UUID = userInfo.UUID;
                }
            }

            //Exception
            msg.Details.Error = RaygunErrorMessageBuilder.Build(logEventException);
            
            msg.Details.Error.Data.Add("Exception Id", exceptionID);
            msg.Details.Error.Data.Add("Log Level", logEvent.Level.ToString());
            msg.Details.Error.Data.Add("Logger Name", logEvent.LoggerName);
            msg.Details.Error.Data.Add("LogHistory", GetLogHistory());

            RaygunClientProvider.Raygun.SendInBackground(msg);
        }

        /// <summary>
        /// Determine if the exception should not be submitted.
        /// </summary>
        /// <param name="exc">The exception to check.</param>
        /// <returns>true if the exception should not be sent to Raygun; otherwise, false.</returns>
        private bool ExcludeException(Exception exc)
        {
            var exceptionMessage = exc.Message.ToLower();
            var exceptionString = exc.ToString().ToLower();

            return exceptionString.Contains("get_islastviewablesibling()") // Infragistics internal error
                || exceptionMessage.StartsWith("if no file is open in quickbooks, a file must be specified."); // No QuickBooks file
        }

        /// <summary>
        /// Configures the target at the specified level and adds to NLog to start out putting..
        /// </summary>
        /// <param name="logLevel"> The log level. </param>
        public static void ConfigureTargetForLogging(LogLevel logLevel)
        {
            var target = new ErrorReporterTarget();
            var asyncWr = new Targets.Wrappers.AsyncTargetWrapper(target, 10, Targets.Wrappers.AsyncTargetWrapperOverflowAction.Discard);
            
            Config.SimpleConfigurator.ConfigureForTargetLogging(asyncWr, logLevel);
        }

        /// <summary>
        /// Calculates the exception unique ID so that the same exceptions can be grouped together.
        /// </summary>
        /// <param name="exc">The exc.</param>
        /// <returns>System.String.</returns>
        private static int CalculateExceptionUniqueID(Exception exc)
        {
            if(exc.StackTrace != null)
                return Math.Abs(exc.StackTrace.GetHashCode());
            
            return 0;
        }

        private string GetLogHistory()
        {
            var history = new StringBuilder();
            history.AppendLine(Environment.NewLine);

            foreach (var entry in _logBuffer.GetAll())
            {
                history.AppendLine(FormatEntry(entry));
            }

            return history.ToString();
        }

        private static string FormatEntry(LogEventInfo entry)
        {
            if (entry.Exception != null)
            {
                string exceptionMessage;
                if (entry.Exception is AggregateException aggregateException)
                {
                    exceptionMessage = string.Join(",",
                        aggregateException.InnerExceptions.Select(exc => exc.GetType()));
                }
                else
                {
                    exceptionMessage = entry.Exception.GetType().ToString();
                }

                return $"{entry.TimeStamp.ToShortTimeString()} -- {entry.FormattedMessage}({exceptionMessage})";
            }

            return $"{entry.TimeStamp.ToShortTimeString()} -- {entry.FormattedMessage}";
        }

        #endregion
    }
}