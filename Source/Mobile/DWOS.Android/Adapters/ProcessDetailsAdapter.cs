using System;
using System.Collections.Generic;

using Android.App;
using Android.Views;
using Android.Widget;
using DWOS.Services.Messages;
using DWOS.ViewModels;
using DWOS.Utilities;
using Android.Support.V4.Content;
using Android.Graphics;

namespace DWOS.Android
{
    /// <summary>
    /// Adapter that makes views for <see cref="OrderProcessInfo"/> instances.
    /// </summary>
    public class ProcessDetailsAdapter : BaseAdapter<OrderProcessInfo>
    {
        #region Fields
        Activity _context;
        ProcessViewModel _processViewModel;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of processes for this instance.
        /// </summary>
        public IList<OrderProcessInfo> Processes { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessDetailsAdapter"/> class.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="processes">The processes.</param>
        public ProcessDetailsAdapter(Activity activity, IList<OrderProcessInfo> processes)
        {
            Processes = processes;
            _context = activity;
            _processViewModel = ServiceContainer.Resolve<ProcessViewModel>();
        }

        public override OrderProcessInfo this[int position]
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
            view.FindViewById<TextView>(Resource.Id.textViewProcessAliasName).Text = process.ProcessAliasName;
            var dateValue = string.Empty;
            if (process.Started > DateTime.MinValue)
                dateValue = process.Started.ToShortDateString();
            if (process.Ended > DateTime.MinValue)
                dateValue += " -  " + process.Ended.ToShortDateString();
            view.FindViewById<TextView>(Resource.Id.textViewProcessDate).Text = dateValue;

            var fixtureCountView = view.FindViewById<TextView>(Resource.Id.textViewFixtureCount);
            var fixtureWeightView = view.FindViewById<TextView>(Resource.Id.textViewFixtureWeight);

            if (process.FixtureCount.HasValue)
            {
                fixtureCountView.Text = string.Format("Number of Fixtures: {0}",
                    process.FixtureCount.Value);

                string fixtureWeightContent;

                if (process.WeightPerFixture.HasValue)
                {
                    fixtureWeightContent = string.Format("Weight per Fixture: {0:F2} lbs.",
                        process.WeightPerFixture.Value);
                }
                else
                {
                    fixtureWeightContent = "Weight per Fixture: Unknown";
                }

                fixtureWeightView.Text = fixtureWeightContent;

                fixtureCountView.Visibility = ViewStates.Visible;
                fixtureWeightView.Visibility = ViewStates.Visible;
            }
            else
            {
                fixtureCountView.Text = string.Empty;
                fixtureCountView.Visibility = ViewStates.Gone;

                fixtureWeightView.Text = string.Empty;
                fixtureWeightView.Visibility = ViewStates.Gone;
            }

            var progressView = view.FindViewById<View>(Resource.Id.viewProcessStatus);
            var processStatus = _processViewModel.GetProcessStatus(process);

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

    }
}