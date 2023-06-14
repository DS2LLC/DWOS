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
    public class NavigationService: IViewNavigationService
    {
        /// <summary>
        /// Navigates to view.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        public void NavigateToView(ViewName viewName)
        {
            Activity currentActivity;
            if (DWOSApplication.Current.CurrentActivity.TryGetTarget(out currentActivity))
            {
                if (viewName == ViewName.Login)
                {
                    var intent = new Intent(currentActivity, typeof(LogInActivity));
                    intent.AddFlags(ActivityFlags.ClearTop);
                    currentActivity.StartActivity(intent);
                }
            }
        }
    }
}