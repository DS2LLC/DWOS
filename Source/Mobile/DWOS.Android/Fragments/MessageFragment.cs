using System;
using Android.App;
using Android.OS;

namespace DWOS.Android
{
    /// <summary>
    /// Provider of a Dialog with Title and Message;
    /// </summary>
    public class MessageDialogFragment : DialogFragment
    {
        #region Fields

        public const string MESSAGE_FRAGMENT_TAG = "AboutFragment";

        #endregion

        #region Properties

        public string Title { get; set; }
        public string Message { get; set; }
        public string OKButtonConent { get; set; }
        public Action OnOkButtonClick { get; set; }

        #endregion

        #region Methods

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var alertDialogBuilder = new AlertDialog.Builder(Activity);
            alertDialogBuilder.SetTitle(Title)
                .SetMessage(Message)
                .SetPositiveButton(OKButtonConent, (s, a) => OnOkButtonClick?.Invoke());
            return alertDialogBuilder.Create();
        }

        #endregion
    }
}