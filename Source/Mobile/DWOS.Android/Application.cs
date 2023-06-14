using Android.App;
using Android.Runtime;
using DWOS.Utilities;
using Mindscape.Raygun4Net;
using System;

namespace DWOS.Android
{
    #if DEBUG
    [Application(Label = "DWOS", Theme = "@style/Theme.Customdwosactionbar")]
    #else
    [Application(Label = "DWOS", Theme = "@style/Theme.Customdwosactionbar", Debuggable=false)]
#endif
    public class DWOSApplication : global::Android.App.Application
    {
        public const string PREFS_NAME = "com.ds2.DWOS";
        public const string SERVER_URL_PREFNAME = "ServerUrl";
        const string _raygunKey = "K5eqnIh3q/KIqe8TfTB2ew==";

        /// <summary>
        /// Gets the current <see cref="DWOSApplication"/> instance.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public static DWOSApplication Current { get; private set; }

        /// <summary>
        /// Gets the current activity.
        /// </summary>
        /// <value>
        /// The current activity.
        /// </value>
        public WeakReference<Activity> CurrentActivity { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to attempt to restore main activity from crash.
        /// </summary>
        /// <value>
        /// <c>true</c> if [restore main activity from crash]; otherwise, <c>false</c>.
        /// </value>
        public bool RestoreMainActivityFromCrash { get; set; }

        /// <summary>
        /// Must implement this constructor for subclassing the application class.
        /// Will act as a global application class throughout the app.
        /// </summary>
        /// <param name="javaReference">pointer to java</param>
        /// <param name="transfer">transfer enumeration</param>
        public DWOSApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
            {
                RestoreMainActivityFromCrash = true;
                var logger = ServiceContainer.Resolve<ILogService>();
                if (logger != null)
                    logger.LogError("UnhandledException from AndroidEnvironment.UnhandledExceptionRaiser", args.Exception);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                RestoreMainActivityFromCrash = true;
                var logger = ServiceContainer.Resolve<ILogService>();
                if (logger != null)
                    logger.LogError("UnhandledException from AppDomain.CurrentDomain.UnhandledException", args.ExceptionObject as Exception);
            };
        }

        /// <summary>
        /// Override on create to instantiate the service container to be persistant.
        /// </summary>
        public override void OnCreate()
        {
            base.OnCreate();

            Current = this;
            RaygunClient.Attach(_raygunKey);
            RaygunClient.Current.SendingMessage += Raygun_SendingMessage;

            // Register services
            LogService logService = new LogService();
            ServiceContainer.Register<IFileService>(() => new FileService());
            ServiceContainer.Register<ILogService>(() => logService);
            ServiceContainer.Register<IDialogService>(() => new DialogService());
            ServiceContainer.Register<IViewNavigationService>(() => new NavigationService());
            ServiceContainer.Register<IVersionService>(() => new VersionService());

            //Register core library services
            ServiceRegistrar.Startup(logService);
        }

        /// <summary>
        /// Sets the current activity.
        /// </summary>
        /// <param name="activity">The activity.</param>
        public void SetCurrentActivity(Activity activity)
        {
            if (activity != null)
            {
                var currentActivity = new WeakReference<Activity>(activity);
                CurrentActivity = currentActivity;
            }
        }

        private void Raygun_SendingMessage(object sender, RaygunSendingMessageEventArgs e)
        {
#if DEBUG
            // Prevent Raygun from sending messages for debug/development builds.
            e.Cancel = true;
#endif
        }
    }
}