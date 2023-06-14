using System;
using Android.App;
using Android.OS;
using System.Threading.Tasks;
using DWOS.ViewModels;

namespace DWOS.Android
{
    /// <summary>
    /// Provides a Stateful execution of an asynchronous task on behalf of an Activity or Fragment
    /// </summary>
    public class TaskFragment : Fragment
    {
        #region Methods

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
        }

        public async Task ExecuteTaskAsync(Func<Task<ViewModelResult>> func)
        {
            var result = await Task<ViewModelResult>.Run(() => func());
            CallbackWithResults(result);
        }

        private void CallbackWithResults(ViewModelResult result)
        {
            var callback = Activity as ITaskFragmentCallback;
            if (callback == null)
                callback = ParentFragment as ITaskFragmentCallback;

            if (callback != null)
                callback.TaskCompleted(result, Tag);
        }

        #endregion
    }
}