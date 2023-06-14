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
using System.Threading.Tasks;
using Android.Util;

namespace DWOS.Android
{
    public class LogService : ILogService
    {
        private Exception _lastException;
        private DateTime _lastExceptionOccured;
        private int _lastExceptionCount = 0;
        private const int MAX_LOG_HISTORY = 8;
        private const int MINUTES_BETWEEN_FAILURE = 2;
        private const int MAX_DUPLICATE_FAILURES = 10;
        private const string ELAPSED_TIME_TAG = "ElapsedTime";

        public void LogError(string message, Exception exception = default(Exception), object context = default(object), bool toast = default(bool))
        {
            var currentContext = context != null ? context as Context : Application.Context;
            LogRaygunError(message, exception, currentContext);

            if (toast && currentContext != null)
                Toast.MakeText(currentContext, message, ToastLength.Short).Show();
        }

        public async Task LogInfoAsync(string message, object context = default(object))
        {
            await LogInfoToDWOSServerAsync(message);
        }

        public void LogElapsedTime(Uri requestUri, TimeSpan elapsedTime)
        {
            Log.Info(ELAPSED_TIME_TAG, "Path: {0} Elapsed Time: {1:c}", requestUri.PathAndQuery, elapsedTime);
        }

        private async Task LogInfoToDWOSServerAsync(string message)
        {
            var dwosLoggingService = ServiceContainer.Resolve<IDWOSLoggingService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (loginViewModel.UserProfile != null && !string.IsNullOrEmpty(loginViewModel.ServerUrlWellFormed))
            {
                dwosLoggingService.RootUrl = loginViewModel.ServerUrlWellFormed;
                await dwosLoggingService.LogInfoAsync(message, loginViewModel.UserProfile);
            }
        }

        private void LogRaygunError(string message, Exception exception, Context context)
        {
            if (exception != null && (ExcludeFrequentException() || ExcludeSpecificException(exception)))
                return;

            _lastException = exception;
            _lastExceptionOccured = DateTime.Now;
            _lastExceptionCount = 0;

            var msg = RaygunMessageBuilder.New;
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            msg.SetEnvironmentDetails();
            msg.SetExceptionDetails(exception);
            msg.SetClientDetails();
            msg.SetVersion(context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode.ToString());
            msg.SetUserCustomData(new Dictionary<string, string>() { { "Activity", context == null ? "Unknown" : context.GetType().FullName } });
            var userId = loginViewModel.UserProfile != null ? loginViewModel.UserProfile.UserId.ToString() : "NONE";
            var userName = loginViewModel.UserProfile != null && !(string.IsNullOrEmpty(loginViewModel.UserProfile.Name)) 
                ? loginViewModel.UserProfile.Name.ToString() : "NONE";
            var userTitle = loginViewModel.UserProfile != null && !(string.IsNullOrEmpty(loginViewModel.UserProfile.Title)) 
                ? loginViewModel.UserProfile.Title.ToString() : "NONE";
            var company = ApplicationSettings.Settings.CompanyName;
            var server = ApplicationSettings.Settings.ServerVersion;
            var user = new RaygunIdentifierMessage(userId);
            var userDataDictionary = new Dictionary<string, string>() 
            { 
                { "userName", userName },
                { "userTitle", userTitle },
                { "userId", userId }, 
                { "company", company }, 
                { "server", server } 
            };
            msg.SetUserCustomData(userDataDictionary);
            RaygunClient.Current.SendInBackground(msg.Build());
        }

        private bool ExcludeFrequentException()
        {
            bool exclude = false;
            var minutesSinceLastFailure = DateTime.Now.Subtract(_lastExceptionOccured).TotalMinutes;

            if (_lastException != null && minutesSinceLastFailure < MINUTES_BETWEEN_FAILURE)
            {
                if (_lastExceptionCount < MAX_DUPLICATE_FAILURES)
                {
                    _lastExceptionCount++;
                    exclude = true;
                }
            }

            return exclude;
        }

        private bool ExcludeSpecificException(Exception exception)
        {
            if (exception is System.Net.WebException)
            {
                var exceptionMessage = exception.Message.ToLower();
                return exceptionMessage.StartsWith("error: connectfailure") ||
                    exceptionMessage.StartsWith("error: nameresolutionfailure");
            }
            else if (exception is System.IO.IOException)
            {
                var innerExceptionMsg = exception.InnerException?.Message?.ToLower() ?? string.Empty;
                return innerExceptionMsg.StartsWith("connection timed out");
            }
            else
            {
                return false;
            }
        }
    }
}