using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DWOS.Utilities;

namespace DWOS.Android
{
    /// <summary>
    /// Implementation of Version Services for the App
    /// </summary>
    public class VersionService : IVersionService
    {
        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <returns></returns>
        public string GetAppVersion()
        {
            var version = string.Empty;
            try
            {
                Activity activity;
                if (DWOSApplication.Current.CurrentActivity.TryGetTarget(out activity))
                {
                    var info = activity.PackageManager.GetPackageInfo(activity.PackageName, flags: 0);
                    return info.VersionName;
                }
            }
            catch (Exception exception)
            {
                var errorService = ServiceContainer.Resolve<ILogService>();
                var message = string.Format("Error in {0}", "VersionService.GetAppVersion");
                errorService.LogError(message, exception);
            }

            return version;
        }
    }
}