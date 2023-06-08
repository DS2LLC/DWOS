using NLog.Config;
using NLog.Targets;
using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace NLog
{
    /// <summary>
    /// Defines extension methods for <see cref="Logger"/>.
    /// </summary>
    public static class NLogExtensions
    {
        /// <summary>
        /// Returns the file name of the log file for the first file target.
        /// </summary>
        /// <returns></returns>
        public static string FindLogFileName()
        {
            try
            {
                if (LogManager.Configuration != null && LogManager.Configuration.ConfiguredNamedTargets.Count != 0)
                {
                    var fileTarget = LogManager.Configuration.AllTargets.OfType<FileTarget>().FirstOrDefault();

                    if (fileTarget != null)
                    {
                        // Need to set timestamp here if filename uses date. 
                        // For example - filename="${basedir}/logs/${shortdate}/trace.log"
                        var file = fileTarget.FileName.Render(new LogEventInfo { TimeStamp = DateTime.Now });

                        //remove any double backslashes
                        if(file != null)
                            file = file.Replace("\\\\", "\\");

                        return file;
                    }
                }

                return null;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error finding logger file name.");
                return null;
            }
        }

        /// <summary>
        /// Loads the log configuration from a string configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="baseDir">The base dir to use if you want to overide the default value.</param>
        public static void LoadNLogConfigFromString(string configuration, string baseDir = null)
        {
            try
            {
                var xr = XmlReader.Create(new StringReader(configuration));
                var config = new XmlLoggingConfiguration(xr, null);

                if (baseDir != null)
                {
                    config.ConfiguredNamedTargets.ForEach(t =>
                    {
                        if (t is NLog.Targets.Wrappers.AsyncTargetWrapper && ((NLog.Targets.Wrappers.AsyncTargetWrapper)t).WrappedTarget is FileTarget)
                        {
                            var fileTarget = ((NLog.Targets.Wrappers.AsyncTargetWrapper)t).WrappedTarget as FileTarget;

                            baseDir = baseDir.EnsureEndsWith("\\");

                            //fileName="${basedir}Logs\${shortdate}.log"
                            //var path = Path.Combine(FileSystem.ApplicationPath(), "Logs", DateTime.Now.ToShortDateString().Replace("/", ".") + ".log");
                            ((Layouts.SimpleLayout)fileTarget.FileName).Text = ((Layouts.SimpleLayout)fileTarget.FileName).Text.Replace("${basedir}", baseDir);
                        }
                    });
                }

                LogManager.Configuration = config;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading the configuration from a string.");
            }
        }
    }


}
