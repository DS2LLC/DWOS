
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
using Android.Graphics;
using Android.Views.InputMethods;
using DWOS.Utilities;
using Android.Support.V4.Content;
using Android.Graphics.Drawables;

namespace DWOS.Android
{
	/// <summary>
	/// Set of extension methods for working with Android
	/// </summary>
	public static class AndroidExtensions 
	{
        public static IMenuItem SetEnabled(this IMenuItem item, bool enabled, Context context, int iconId)
        {
            return item.SetEnabled(enabled, context, iconId, Color.Gray);
        }

        public static IMenuItem SetEnabled(this IMenuItem item, bool enabled, Context context, int iconId, Color disabledColor)
        {
            var resIcon = ContextCompat.GetDrawable(context, iconId);

            if (!enabled)
                resIcon.Mutate().SetColorFilter(disabledColor, PorterDuff.Mode.SrcIn);

            item.SetEnabled(enabled);
            item.SetIcon(resIcon);

            return item;
        }

        public static void ShowSoftKeyboard(this Activity activity, View view = null, int delay = 500)
        {
            new Handler().PostDelayed(delegate
            {
                view = view ?? activity.CurrentFocus;
                if (view != null)
                {
                    try
                    {
                        if (view.HasFocus)
                            view.ClearFocus(); //bug fix for older versions of android

                        view.RequestFocus();
                        InputMethodManager manager = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
                        manager.ShowSoftInput(view, 0);
                    }
                    catch (Exception exception)
                    {
                        var logService = ServiceContainer.Resolve<ILogService>();
                        logService.LogError("Error showing Keyboard", exception);
                    }

                }
            }, delay);
        }

        public static void HideSoftKeyboard(this Activity activity, View view = null, int delay = 500)
        {
            new Handler().PostDelayed(delegate
            {
                view = view ?? activity.CurrentFocus;
                if (view != null)
                {
                    try
                    {
                        if (view.HasFocus)
                            view.ClearFocus(); //bug fix for older versions of android

                        view.RequestFocus();

                        var imm = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
                        imm.HideSoftInputFromWindow(view.ApplicationWindowToken, HideSoftInputFlags.None);
                    }
                    catch (Exception exception)
                    {
                        var logService = ServiceContainer.Resolve<ILogService>();
                        logService.LogError("Error hiding Keyboard", exception);
                    }

                }
            }, delay);
        }

        /// <summary>
        /// Opens a file using an <see cref="Intent"/>.
        /// </summary>
        /// <param name="activity">The Activity instance.</param>
        /// <param name="path">Local path to the file</param>
        public static void OpenFile(this Activity activity, string path)
        {
            const string errorMsgFormat = "Unable to open file {0}";

            var fileToOpen = new Java.IO.File(path);
            Intent intent = new Intent(Intent.ActionView);

            var fileUri = FileProvider.GetUriForFile(activity.ApplicationContext, $"{activity.PackageName}.provider", fileToOpen);

            string mimeType = null;

            string extension = global::Android.Webkit.MimeTypeMap.GetFileExtensionFromUrl(path);

            if (!string.IsNullOrEmpty(extension))
            {
                // Android cannot find MIME type for non-lowercase extensions.
                mimeType = global::Android.Webkit.MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension.ToLowerInvariant());
            }

            if (string.IsNullOrEmpty(mimeType))
            {
                mimeType = "text/plain";
            }

            intent.SetDataAndType(fileUri, mimeType);
            intent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission | ActivityFlags.NewTask);

            try
            {
                activity.StartActivity(intent);
            }
            catch (ActivityNotFoundException)
            {
                Toast.MakeText(activity, text: string.Format(errorMsgFormat, path), duration: ToastLength.Short).Show();
            }
        }

        public static int CompatHour(this TimePicker picker)
        {
            if (picker == null)
            {
                throw new ArgumentNullException(nameof(picker));
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                return picker.Hour;
            }
            else
            {
#pragma warning disable 612, 618
                return (int)picker.CurrentHour;
#pragma warning restore 612, 618
            }
        }

        public static int CompatMinute(this TimePicker picker)
        {
            if (picker == null)
            {
                throw new ArgumentNullException(nameof(picker));
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                return picker.Minute;
            }
            else
            {
#pragma warning disable 612, 618
                return (int)picker.CurrentMinute;
#pragma warning restore 612, 618
            }
        }

        public static void CompatSetTint(this Drawable drawable, int color)
        {
            if (drawable == null)
            {
                throw new ArgumentNullException(nameof(drawable));
            }

            using (var tintableDrawable = global::Android.Support.V4.Graphics.Drawable.DrawableCompat.Wrap(drawable).Mutate())
            {
                global::Android.Support.V4.Graphics.Drawable.DrawableCompat.SetTint(tintableDrawable, color);
            }
        }

        public static void StartForegroundServiceCompat(this Context context, Intent intent)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                context.StartForegroundService(intent);
            }
            else
            {
                context.StartService(intent);
            }
        }
    }
}

