using Android.App;
using Android.OS;
using DWOS.Utilities;
using DWOS.ViewModels;

namespace DWOS.Android
{
    /// <summary>
    /// Shows a confirmation dialog when logging out.
    /// </summary>
    public class LogOutConfirmUserFragment : DialogFragment
    {
        #region Fields

        public const string LOGOUT_CONFIRM_FRAGMENT_TAG = "LogoutConfirmFragment";

        #endregion

        #region Methods

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var alertDialogBuilder = new AlertDialog.Builder(Activity);
            alertDialogBuilder.SetTitle(Resource.String.ConfirmLogOutTitle);
            alertDialogBuilder.SetMessage(Resource.String.ConfirmLogOut);
            alertDialogBuilder.SetPositiveButton(Resource.String.ConfirmOK, (sender, args) =>
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