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

namespace DWOS.Android
{
    /// <summary>
    /// Implementation of <see cref="IDialogService"/>
    /// </summary>
    class DialogService : IDialogService
    {
        public void Show(string title, string message, string OKButtonContent = "OK", Action onOkButtonClick = null)
        {
            var dialogFragment = new MessageDialogFragment()
            {
                Title = title,
                Message = message,
                OKButtonConent = OKButtonContent,
                OnOkButtonClick = onOkButtonClick
            };

            Activity activity;
            if (DWOSApplication.Current.CurrentActivity.TryGetTarget(out activity))
            {
                var transaction = activity.FragmentManager.BeginTransaction();
                dialogFragment.Show(transaction, MessageDialogFragment.MESSAGE_FRAGMENT_TAG);
            }
        }

        public void ShowToast(string message, bool isShort = true)
        {
            Activity activity;
            if (DWOSApplication.Current.CurrentActivity.TryGetTarget(out activity))
            {
                var toastLength = isShort ? ToastLength.Short : ToastLength.Long;
                Toast.MakeText(activity, message, toastLength)
                    .Show();
            }
        }
    }
}