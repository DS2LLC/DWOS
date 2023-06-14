using System;
using Android.App;
using Android.OS;

namespace DWOS.Android
{
    /// <summary>
    /// Shows the user a save confirmation dialog.
    /// </summary>
    class SaveConfirmFragment : DialogFragment
    {
        #region Fields

        public const string LOGOUT_CONFIRM_FRAGMENT_TAG = "SaveConfirmFragment";
        private const string BUNDLE_MESSAGE = "MESSAGE";
        private const string BUNDLE_TITLE = "TITLE";
        private string _title;
        private string _message;
        private Action<Dialog> _neutralCallback;
        private Action<Dialog> _negativeCallback;
        private Action<Dialog> _positiveCallback;

        #endregion

        #region Methods

        /// <summary>
        /// Returns a new instance of the <see cref="SaveConfirmFragment"/> class.
        /// </summary>
        /// <param name="message">Message to show.</param>
        /// <param name="title">Titel to show.</param>
        /// <param name="positiveCallback">Positive button click handler.</param>
        /// <param name="negativeCallback">Negative button click handler.</param>
        /// <param name="neutralCallback">Neutral button click handler.</param>
        /// <returns></returns>
        public static SaveConfirmFragment New(string message, string title, 
            Action<Dialog> positiveCallback, Action<Dialog> negativeCallback, Action<Dialog> neutralCallback)
        {
            var fragment = new SaveConfirmFragment();
            var bundle = new Bundle();
            bundle.PutString(BUNDLE_MESSAGE, message);
            bundle.PutString(BUNDLE_TITLE, title);

            fragment.Arguments = bundle;
            fragment._positiveCallback = positiveCallback;
            fragment._negativeCallback = negativeCallback;
            fragment._neutralCallback = neutralCallback;

            return fragment;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            if (Arguments != null)
            {
                _message = Arguments.GetString(BUNDLE_MESSAGE);
                _title = Arguments.GetString(BUNDLE_TITLE);
            }

            var alertDialogBuilder = new AlertDialog.Builder(Activity);
            alertDialogBuilder.SetTitle(_title);
            alertDialogBuilder.SetMessage(_message);
            alertDialogBuilder.SetPositiveButton(Resource.String.SaveYes, (sender, args) =>
            {
                _positiveCallback?.Invoke(sender as Dialog);
                ReleaseCallbacks();
            });
            alertDialogBuilder.SetNegativeButton(Resource.String.SaveNo, (sender, eventArgs) =>
            {
                _negativeCallback?.Invoke(sender as Dialog);
                ReleaseCallbacks();
            });
            alertDialogBuilder.SetNeutralButton(Resource.String.SaveCancel, (sender, eventArgs) =>
            {
                _neutralCallback?.Invoke(sender as Dialog);
                ReleaseCallbacks();
            });

            return alertDialogBuilder.Create();
        }

        public void ReleaseCallbacks()
        {
            _positiveCallback = null;
            _negativeCallback = null;
            _neutralCallback = null;
        }

        #endregion
    }
}