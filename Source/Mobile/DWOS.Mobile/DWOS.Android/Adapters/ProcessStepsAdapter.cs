using Android.Widget;
using DWOS.Services.Messages;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using DWOS.ViewModels;
using DWOS.Utilities;
using Android.Graphics;
using Android.Support.V4.Content;

namespace DWOS.Android
{
    /// <summary>
    /// Adapter that makes views for <see cref="ProcessStepInfo"/> instances.
    /// </summary>
    public class ProcessStepsAdapter : BaseAdapter<ProcessStepInfo>
    {
        #region Private Classes

        /// <summary>
        /// Utility class that can hold Views for an Adapter so it doesn't have to call FindViewById
        /// repeatedly.
        /// </summary>
        private class StepListViewHolder : Java.Lang.Object
        {
            public TextView Name { get; set; }
            public TextView StepOrder { get; set; }
            public View Progress { get; set; }
        } 

        #endregion

        #region Fields
        IList<ProcessStepInfo> _steps;
        Activity _context;
        ProcessViewModel _processViewModel;
        BatchProcessViewModel _batchProcessViewModel;
        private Mode _currentMode;
        #endregion

        #region Methods
        public ProcessStepsAdapter(Activity activity, List<ProcessStepInfo> steps, Mode CurrentMode)
        {
            _steps = steps;
            _context = activity;
            _processViewModel = ServiceContainer.Resolve<ProcessViewModel>();
            _batchProcessViewModel = ServiceContainer.Resolve<BatchProcessViewModel>();
            _currentMode = CurrentMode;
        }

        public override ProcessStepInfo this[int position]
        {
            get { return _steps[position]; }
        }

        public override int Count
        {
            get { return _steps.Count;  }
        }

        public override long GetItemId(int position)
        {
            return _steps[position].ProcessStepId;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (position > _steps.Count)
                return null;

            StepListViewHolder holder = null;
            var step = _steps[position];
            var view = convertView;
            if (view != null)
                holder = view.Tag as StepListViewHolder;
            if (view == null)
            {
                view = _context.LayoutInflater.Inflate(Resource.Layout.ProcessStepListItemLayout, null);
                holder = new StepListViewHolder()
                {
                    StepOrder = view.FindViewById<TextView>(Resource.Id.textViewStepOrder),
                    Name = view.FindViewById<TextView>(Resource.Id.textViewStepName),
                    Progress = view.FindViewById<View>(Resource.Id.viewStepStatus)
                };
                view.Tag = holder;
            }

            holder.StepOrder.Text = step.StepOrder.ToString();
            holder.Name.Text = step.Name;

            var stepStatus = _currentMode == Mode.Orders ? _processViewModel.GetStepStatus(step) : _batchProcessViewModel.GetStepStatus(step);

            int backgroundColorID;
            switch (stepStatus)
            {
                case StepStatus.Completed:
                case StepStatus.Skipped:
                    backgroundColorID = Resource.Color.stepStatusComplete;
                    break;
                case StepStatus.InProgress:
                    backgroundColorID = Resource.Color.stepStatusInProgress;
                    break;
                case StepStatus.Incomplete:
                default:
                    backgroundColorID = Resource.Color.stepStatusIncomplete;
                    break;
            }

            holder.Progress.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, backgroundColorID)));

            var isValid = _processViewModel.IsStepValid(step);
            var color = isValid == true ? Color.Black : Color.Red;
            holder.Name.SetTextColor(color);
            holder.StepOrder.SetTextColor(color);

            view.Visibility = stepStatus != StepStatus.Skipped ? ViewStates.Visible : ViewStates.Gone;
            return view;
        } 

        #endregion
    }
}
