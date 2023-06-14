using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using DWOS.ViewModels;
using DWOS.Services.Messages;
using DWOS.Utilities;
using Android.Support.V4.Content;
using Android.Graphics;

namespace DWOS.Android
{
    /// <summary>
    /// Adapter that makes views for <see cref="BatchProcessInfo"/> instances.
    /// </summary>
    public class BatchProcessDetailsAdapter : BaseAdapter<BatchProcessInfo>
    {
        #region Fields
        Activity _context;
        BatchProcessViewModel _batchProcessViewModel;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of batch processes for this instance.
        /// </summary>
        public IList<BatchProcessInfo> Processes { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessDetailsAdapter"/> class.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="processes">The processes.</param>
        public BatchProcessDetailsAdapter(Activity activity, IList<BatchProcessInfo> processes)
        {
            Processes = processes;
            _context = activity;
            _batchProcessViewModel = ServiceContainer.Resolve<BatchProcessViewModel>();
        }

        public override BatchProcessInfo this[int position]
        {
            get { return Processes[position]; }
        }

        public override int Count
        {
            get { return Processes.Count; }
        }

        public override long GetItemId(int position)
        {
            return Processes[position].ProcessId;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (position > Processes.Count)
                return null;

            var process = Processes[position];
            var view = convertView;
            if (view == null)
                view = _context.LayoutInflater.Inflate(Resource.Layout.ProcessDetailsListItemLayout, null);

            view.FindViewById<TextView>(Resource.Id.textViewProcessStep).Text = process.StepOrder.ToString();
            view.FindViewById<TextView>(Resource.Id.textViewProcessName).Text = process.ProcessName;
            view.FindViewById<TextView>(Resource.Id.textViewProcessDepartment).Text = process.Department;

            // Show process aliases
            view.FindViewById<TextView>(Resource.Id.textViewProcessAliasName).Text =
                string.Join("\n", process.ProcessAliasNames);

            view.FindViewById<TextView>(Resource.Id.textViewFixtureCount).Visibility = ViewStates.Gone;
            view.FindViewById<TextView>(Resource.Id.textViewFixtureWeight).Visibility = ViewStates.Gone;

            var dateValue = string.Empty;
            if (process.Started > DateTime.MinValue)
                dateValue = process.Started.ToShortDateString();
            if (process.Ended > DateTime.MinValue)
                dateValue += " -  " + process.Ended.ToShortDateString();
            view.FindViewById<TextView>(Resource.Id.textViewProcessDate).Text = dateValue;
            var progressView = view.FindViewById<View>(Resource.Id.viewProcessStatus);
            var processStatus = _batchProcessViewModel.GetProcessStatus(process);

            int backgroundColorID;
            switch (processStatus)
            {
                case ProcessStatus.Completed:
                    backgroundColorID = Resource.Color.stepStatusComplete;
                    break;
                case ProcessStatus.InProgress:
                    backgroundColorID = Resource.Color.stepStatusInProgress;
                    break;
                case ProcessStatus.Incomplete:
                default:
                    backgroundColorID = Resource.Color.stepStatusIncomplete;
                    break;
            }

            progressView.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, backgroundColorID)));
            return view;
        }

        #endregion
    }
}