using DWOS.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Provides application with system wide settings relevant to this application and the 
    /// DWOS Server
    /// </summary>
    public static class ApplicationSettings
    {
        private static ApplicationSettingsInfo _settings;
        public static ApplicationSettingsInfo Settings
        {
            get
            {
                return _settings ?? (_settings = new ApplicationSettingsInfo());
            }
            internal set
            {
                _settings = value;
            }
        }
    }
}
