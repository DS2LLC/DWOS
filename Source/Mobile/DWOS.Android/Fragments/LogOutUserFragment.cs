using Android.App;
using Android.OS;
using DWOS.Utilities;
using DWOS.ViewModels;

namespace DWOS.Android
{
    /// <summary>
    /// Alerts the user that their session is expired / license is bad.
    /// </summary>
    public class LogOutUserFragment : DialogFragment
    {
        #region Fields

        public const string LOGOUT_FRAGMENT_TAG = "LogoutFragment";

        #endregion

        #region Methods

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var alertDialogBuilder = new AlertDialog.Builder(Activity);
            alertDialogBuilder.SetTitle(Resource.String.ExpiredLogOutTitle);
            alertDialogBuilder.SetMessage(Resource.String.ExpiredLogOut);
            alertDialogBuilder.SetPositiveButton(Resource.String.ExpiredOK, (sender, args) =>
            {
                var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
                loginViewModel.Logout();
            });
            alertDialogBuilder.SetNegativeButton(Resource.String.Cancel, (sender, eventArgs) => { });
            
            return alertDialogBuilder.Create();
        }

        #endregion
    }
}