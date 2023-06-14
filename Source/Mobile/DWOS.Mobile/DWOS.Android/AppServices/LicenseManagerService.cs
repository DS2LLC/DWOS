using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using DWOS.Utilities;
using DWOS.ViewModels;

namespace DWOS.Android.AppServices
{
    [Service(Exported = false)]
    public class LicenseManagerService : Service
    {
        #region Fields
        private const int SERVICE_ID = 10000;
        private const string NOTIFICATION_NORMAL_CHANNEL_ID = "NotificationChannel_License";
        private const string NOTIFICATION_IMPORTANT_CHANNEL_ID = "NotificationChannel_LicenseImportant";

        private NotificationChannel _normalChannel;
        private NotificationChannel _importantChannel;
        private readonly LogInViewModel _loginViewModel;
        private readonly NotificationCompat.Builder _builder;
        private readonly Timer _licenseTimer;

        #endregion

        #region Properties

        #endregion

        #region Methods

        public LicenseManagerService()
        {
            _loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            _builder = new NotificationCompat.Builder(this);
            _licenseTimer = new Timer(LicenseTimerCallback, null, TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(3));
        }

        private void LicenseTimerCallback(object state)
        {
            try
            {
                if (_loginViewModel.IsBusy || !_loginViewModel.IsLoggedIn)
                {
                    return;
                }

                _loginViewModel.CheckLicense();
            }
            catch (Exception exc)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                logService.LogError("Error handling 'renew license' timer event.", exc);
            }
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                _normalChannel = new NotificationChannel(
                    NOTIFICATION_NORMAL_CHANNEL_ID,
                    GetString(Resource.String.NotificationChannel_LicenseCheck),
                    NotificationImportance.Default);
                _normalChannel.LockscreenVisibility = NotificationVisibility.Public;
                (GetSystemService(NotificationService) as NotificationManager).CreateNotificationChannel(_normalChannel);

                _importantChannel = new NotificationChannel(
                    NOTIFICATION_IMPORTANT_CHANNEL_ID,
                    GetString(Resource.String.NotificationChannel_LicenseCheck),
                    NotificationImportance.High);
                _importantChannel.LockscreenVisibility = NotificationVisibility.Public;
                (GetSystemService(NotificationService) as NotificationManager).CreateNotificationChannel(_importantChannel);

            }

            _loginViewModel.IsBusyChanged += LoginViewModel_IsBusyChanged;
            StartForeground(SERVICE_ID, BuildNotification());
            return StartCommandResult.RedeliverIntent;
        }

        private Notification BuildNotification()
        {
            string contextText;

            if (!_loginViewModel.IsLoggedIn)
            {
                contextText = Resources.GetString(Resource.String.LicenseManager_LoggedOut);
            }
            else if (!_loginViewModel.IsLicenseActivated)
            {
                contextText = Resources.GetString(Resource.String.LicenseManager_LicenseDeactivated);
            }
            else
            {
                contextText = Resources.GetString(Resource.String.LicenseManager_LoggedInFormat, _loginViewModel.UserProfile?.Name);
            }

            _builder
                .SetContentTitle(Resources.GetString(Resource.String.ApplicationShortName))
                .SetContentText(contextText)
                .SetStyle(new NotificationCompat.BigTextStyle().BigText(contextText))
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetVisibility((int)NotificationVisibility.Public)
                .SetOngoing(true);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                _builder.SetChannelId( _loginViewModel.IsLoggedIn && !_loginViewModel.IsLicenseActivated
                    ? NOTIFICATION_IMPORTANT_CHANNEL_ID
                    : NOTIFICATION_NORMAL_CHANNEL_ID);
            }
            else
            {
                _builder.SetPriority(_loginViewModel.IsLoggedIn && !_loginViewModel.IsLicenseActivated
                    ? (int)NotificationPriority.High
                    : (int)NotificationPriority.Default);
            }

            return _builder.Build();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _loginViewModel.IsBusyChanged -= LoginViewModel_IsBusyChanged;
        }

        public override IBinder OnBind(Intent intent)
        {
            return new LicenseCheckBinder(this);
        }

        #endregion

        #region Events

        private void LoginViewModel_IsBusyChanged(object sender, EventArgs e)
        {
            try
            {
                if (_loginViewModel.IsBusy)
                {
                    return;
                }

                (GetSystemService(NotificationService) as NotificationManager).Notify(SERVICE_ID, BuildNotification());
            }
            catch (Exception exc)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                logService.LogError("Error handling 'is busy changed' event.", exc);
            }
        }

        #endregion

        #region LicenseCheckBinder

        public class LicenseCheckBinder : Binder
        {
            public LicenseManagerService Service { get; }

            public LicenseCheckBinder(LicenseManagerService service)
            {
                Service = service;
            }
        }

        #endregion
    }
}