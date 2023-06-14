using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DWOS.ViewModels;
using Android.Views.InputMethods;
using DWOS.Utilities;
using Android.Text;
using AndroidHUD;

namespace DWOS.Android
{
    /// <summary>
    /// Login activity.
    /// </summary>
    [Activity(MainLauncher = true, Icon = "@drawable/icon", Name = "dwos.android.LogInActivity", WindowSoftInputMode = SoftInput.AdjustPan, Label = "DWOS")]
    public class LogInActivity : BaseActivity, TextView.IOnEditorActionListener, ITaskFragmentCallback
    {
        #region Fields

        const string LoginTaskFragmentTag = "LoginTaskFragment";
        const string BUNDLE_LOGGINGIN = "LoggingIn";

        static Intent _licenseServiceIntent;

        LogInViewModel _loginViewModel;
        EditText _pinEditText;
        EditText _serverEditText;
        Button _demoServerButton;
        Button _logInButton;
        EventHandler<TextChangedEventArgs> _serverTextChangedHandler;
        EventHandler<TextChangedEventArgs> _pinTextChangedHandler;
        EventHandler _logInClickHandler;
        EventHandler _demoClickHandler;

        #endregion

        #region Methods

        public LogInActivity()
        {
            _loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (_loginViewModel.IsLoggedIn && _loginViewModel.IsLicenseActivated)
            {
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }

            EnsureTaskFragmentExists(LoginTaskFragmentTag);

            SetContentView(Resource.Layout.LogInLayout);
            _pinEditText = FindViewById<EditText>(Resource.Id.userPin);
            _serverEditText = FindViewById<EditText>(Resource.Id.loginServer);
            _logInButton = FindViewById<Button>(Resource.Id.logIn);
            _demoServerButton = FindViewById<Button>(Resource.Id.textViewDemo);

            _pinEditText.SetOnEditorActionListener(this);
            _serverEditText.SetOnEditorActionListener(this);

            _loginViewModel.ServerUrl = LoadServerUrlPref();
            _serverEditText.Text = _loginViewModel.ServerUrl;
            _loginViewModel.UserPin = _pinEditText.Text;
            _logInButton.Enabled = _loginViewModel.IsValid;

            if (bundle != null)
            {
                var isloggingIn = bundle.GetBoolean(BUNDLE_LOGGINGIN, defaultValue: false);
                if (isloggingIn)
                    AndHUD.Shared.Show(this, "Please wait");
            }
        }

        /// <summary>
        /// Called when the Fragment is no longer resumed.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            UnregisterViewModelEvents();
            UnregisterEventHandlers();
        }

        protected override void OnResume()
        {
            base.OnResume();

            //We've set these up already in OnCreate (Prefs) however Android may change them because of an orientation change
            //if that happens and they are different than the View Model.  Set the View Model otherwise View won't equal ViewModel
            if (_serverEditText.Text != _loginViewModel.ServerUrl)
                _loginViewModel.ServerUrl = _serverEditText.Text;

            if (_pinEditText.Text != _loginViewModel.UserPin)
                _loginViewModel.UserPin = _pinEditText.Text;

            if (!string.IsNullOrEmpty(_serverEditText.Text))
                _pinEditText.RequestFocus();

            RegisterViewModelEvents();
            RegisterEventHandlers();

            if (_licenseServiceIntent == null)
            {
                _licenseServiceIntent = new Intent(ApplicationContext, typeof(AppServices.LicenseManagerService));
                base.ApplicationContext.StartForegroundServiceCompat(_licenseServiceIntent);
            }
        }

        protected override void OnDestroy()
        {
            _pinEditText.Dispose();
            _serverEditText.Dispose();
            _logInButton.Dispose();

            if (_licenseServiceIntent != null)
            {
                StopService(_licenseServiceIntent);
                _licenseServiceIntent = null;
            }
            
            base.OnDestroy();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutBoolean(BUNDLE_LOGGINGIN, _loginViewModel.IsBusy);
        }

        public bool OnEditorAction(TextView textView, ImeAction actionId, KeyEvent keyEvent)
        {
            if (actionId == ImeAction.Go)
            {
                if (_loginViewModel.IsValid)
                {
                    LogIn();
                }
                return true;
            }
            else if (actionId == ImeAction.Next)
            {
                _pinEditText.RequestFocus();
                return true;
            }
            return false;
        }

        private async void LogIn()
        {
            var imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(_pinEditText.WindowToken, HideSoftInputFlags.NotAlways);

            _logInButton.Visibility = ViewStates.Invisible;

            AndHUD.Shared.Show(this, "Please wait");
            await _taskFragment.ExecuteTaskAsync(() => _loginViewModel.LoginAsync());
        }

        private void SaveServerUrlPref(string url)
        {
            var pref = GetSharedPreferences(DWOSApplication.PREFS_NAME, FileCreationMode.Private);
            using (var editTransaction = pref.Edit())
            {
                editTransaction.PutString(DWOSApplication.SERVER_URL_PREFNAME, url);
                editTransaction.Commit();
            }
        }

        private string LoadServerUrlPref()
        {
            var pref = GetSharedPreferences(DWOSApplication.PREFS_NAME, FileCreationMode.Private);
            return pref.GetString(DWOSApplication.SERVER_URL_PREFNAME, string.Empty);
        }

        /// <summary>
        /// Called when it is time to register to view model events.
        /// </summary>
        protected override void RegisterViewModelEvents()
        {
            _loginViewModel.IsValidChanged += OnViewModelIsValidChanged;
        }

        /// <summary>
        /// Called when it is time to unregister from view model events.
        /// </summary>
        protected override void UnregisterViewModelEvents()
        {
            _loginViewModel.IsValidChanged -= OnViewModelIsValidChanged;
        }

        private void RegisterEventHandlers()
        {
            _serverTextChangedHandler = (sender, e) => _loginViewModel.ServerUrl = _serverEditText.Text;
            _serverEditText.TextChanged += _serverTextChangedHandler;

            _pinTextChangedHandler = (sender, e) => _loginViewModel.UserPin = _pinEditText.Text;
            _pinEditText.TextChanged += _pinTextChangedHandler;

            _logInClickHandler = (sender, e) => {
                LogIn(); 
            };
            _logInButton.Click += _logInClickHandler;

            var demoServerUrl = GetString(Resource.String.DemoUrl); ;
            _demoClickHandler = (sender, e) => _serverEditText.Text = demoServerUrl;
            _demoServerButton.Click += _demoClickHandler;
        }

        private void UnregisterEventHandlers()
        {
            _serverEditText.TextChanged -= _serverTextChangedHandler;
            _pinEditText.TextChanged -= _pinTextChangedHandler;
            _logInButton.Click -= _logInClickHandler;
            _demoServerButton.Click -= _demoClickHandler;
        }

        private void OnViewModelIsValidChanged(object sender, EventArgs e)
        {
            var hashCode = GetHashCode();
            if (_logInButton != null)
                _logInButton.Enabled = _loginViewModel.IsValid;
        }

        public void TaskCompleted(ViewModelResult result, object tag)
        {
            _logInButton.Visibility = ViewStates.Visible;
            AndHUD.Shared.Dismiss(this);
            if (result.Success)
            {
                SaveServerUrlPref(_loginViewModel.ServerUrl);
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }
            else
            {
                var dialogBuilder = new AlertDialog.Builder(this);
                dialogBuilder.SetMessage(result.ErrorMessage)
                    .SetTitle("Log In Failed")
                    .SetPositiveButton("OK", (sender, args) => { })
                    .Create()
                    .Show();
            }
        }

        #endregion
    }
}