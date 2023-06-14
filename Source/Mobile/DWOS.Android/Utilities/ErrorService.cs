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
using Mindscape.Raygun4Net;
using DWOS.Utilities;
using DWOS.ViewModels;
using Mindscape.Raygun4Net.Messages;
using System.Diagnostics;

namespace DWOS.Android
{
    public class ErrorService : IErrorService
    {
        public void LogError(string message, Exception exception = default(Exception), object context = default(object), bool toast = default(bool))
        {
            var currentContext = context != null ? context as Context : Application.Context;
            LogRaygunError(message, exception, currentContext);

            if (toast && currentContext != null)
                Toast.MakeText(currentContext, message, ToastLength.Short).Show();
        }

        [Conditional("RELEASE")]
        private void LogRaygunError(string message, Exception exception, Context context)
        {
            var msg = RaygunMessageBuilder.New;
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            msg.SetEnvironmentDetails();
            msg.SetExceptionDetails(new ApplicationException(message, exception));
            msg.SetClientDetails();
            msg.SetVersion(context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode.ToString());
            msg.SetUserCustomData(new Dictionary<string, string>() { { "Activity", context == null ? "Unknown" : context.GetType().FullName } });
            var userId = loginViewModel.UserProfile != null ? loginViewModel.UserProfile.UserId.ToString() : "NONE";
            var company = ApplicationSettings.Settings.CompanyName;
            var user = new RaygunIdentifierMessage(userId);
            msg.SetUser(user);
            RaygunClient.Current.User = userId + " - " + company;
            RaygunClient.Current.SendInBackground(msg.Build());
        }
    }
}