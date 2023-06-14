using System;
using System.Linq;
using Android.App;
using DWOS.Utilities;
using System.Threading;
using Android.Content.PM;
using DWOS.ViewModels;
using Android.Support.V7.App;

namespace DWOS.Android
{
    /// <summary>
    /// Base activity for all activities, handles global OnPause/OnResume 
    /// </summary>
    public abstract class BaseActivity : AppCompatActivity
    {
        #region Fields

        protected TaskFragment _taskFragment;
        protected bool _isPaused = false;
        protected const string _standardErrorMessage = "Whoops! We've encountered some kind of problem.";

        #endregion

        #region Methods

        /// <summary>
        /// Called as part of the activity lifecycle when an activity is going into
        /// the background, but has not (yet) been killed.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            _isPaused = true;
            DWOSApplication.Current.SetCurrentActivity(null);
            UnregisterViewModelEvents();
            var licenseService = ServiceContainer.Resolve<ILicenseSessionService>();
            if (licenseService != null)
                licenseService.Stop();
        }

        /// <summary>
        /// Called after <c><see cref="M:Android.App.Activity.OnRestoreInstanceState(Android.OS.Bundle)" /></c>, <c><see cref="M:Android.App.Activity.OnRestart" /></c>, or
        /// <c><see cref="M:Android.App.Activity.OnPause" /></c>, for your activity to start interacting with the user.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            _isPaused = false;

            if (DWOSApplication.Current.RestoreMainActivityFromCrash && (this.GetType() != typeof(LogInActivity)))
            {
                this.RestartApp();
                return;
            }

            DWOSApplication.Current.SetCurrentActivity(this);
            RegisterViewModelEvents();
            var licenseService = ServiceContainer.Resolve<ILicenseSessionService>();
            licenseService?.KeepAlive();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (grantResults.Length == 0)
            {
                return;
            }

            var dialogService = ServiceContainer.Resolve<IDialogService>();

            switch (requestCode)
            {
                case RequestCodes.ReadWriteMedia:
                    if (grantResults.All(result => result == Permission.Granted))
                    {
                        dialogService.ShowToast("You can now open media.", false);
                    }
                    else
                    {
                        dialogService.ShowToast("Cannot open media without permission.", false);
                    }

                    break;
            }
        }

        /// <summary>
        /// Called when it is time to register to view model events.
        /// </summary>
        protected abstract void RegisterViewModelEvents();

        /// <summary>
        /// Called when it is time to unregister from view model events.
        /// </summary>
        protected abstract void UnregisterViewModelEvents();

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void LogInfo(string message)
        {
            var logService = ServiceContainer.Resolve<ILogService>();
            logService.LogInfoAsync(message, this);
        }

        /// <summary>
        /// Logs the current user out and restarts the application.
        /// </summary>
        protected void RestartApp()
        {
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            loginViewModel.Logout();
            DWOSApplication.Current.RestoreMainActivityFromCrash = false;
            // kill this activity
            base.Finish();
        }

        /// <summary>
        /// Logs the current user out while showing an expiration message.
        /// </summary>
        protected void LogOutUserWithExpiredMessage()
        {
            var alertDialog = new LogOutUserFragment();
            var transaction = FragmentManager.BeginTransaction();
            alertDialog.Show(transaction, LogOutUserFragment.LOGOUT_FRAGMENT_TAG);
        }

        /// <summary>
        /// Ensures the task fragment exists; for use when executing asynchronous tasks.
        /// </summary>
        /// <param name="tag">The tag.</param>
        protected void EnsureTaskFragmentExists(string tag)
        {
            _taskFragment = FragmentManager.FindFragmentByTag<TaskFragment>(tag);
            if (_taskFragment == null)
            {
                _taskFragment = new TaskFragment();
                var transaction = FragmentManager.BeginTransaction();
                transaction.Add(_taskFragment, tag);
                transaction.Commit();
            }
        }

        #endregion
    }
}