using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using DWOS.Services.Messages;

namespace DWOS.Android
{
    /// <summary>
    /// Adapter that makes views for <see cref="BatchInfo"/> instances.
    /// </summary>
    public class BatchListAdapter : BaseAdapter<BatchInfo>
    {
        #region Private Classes

        /// <summary>
        /// Utility class that can hold Views for an Adapter so it doesn't have to call FindViewById
        /// repeatedly.
        /// </summary>
        private class BatchListViewHolder : Java.Lang.Object
        {
            public TextView Title { get; set; }
            public TextView Subtitle { get; set; }
            public TextView Location { get; set; }
            public TextView WorkStatus { get; set; }
        }

        #endregion

        #region Fields

        private Activity _activity;

        #endregion

        #region Properties

        public List<BatchInfo> Batches { get; set; }

        #endregion

        #region Methods

        public BatchListAdapter(Activity activity, List<BatchInfo> batches)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            if (batches == null)
            {
                throw new ArgumentNullException(nameof(batches));
            }

            _activity = activity;
            Batches = batches;
        }

        public override BatchInfo this[int position] { get { return Batches[position]; } }

        public override int Count { get { return Batches.Count; } }

        public override long GetItemId(int position)
        {
            if (position > Batches.Count)
                return -1;

            return Batches[position].BatchId;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (position > Batches.Count)
                return null;

            BatchListViewHolder holder = null;
            var batch = Batches[position];
            var view = convertView;
            if (view != null)
                holder = view.Tag as BatchListViewHolder;
            if (holder == null)
            {
                view = _activity.LayoutInflater.Inflate(Resource.Layout.OrderListItemLayout, null);
                holder = new BatchListViewHolder()
                {
                    Title = view.FindViewById<TextView>(Resource.Id.textViewOrderTitle),
                    Subtitle = view.FindViewById<TextView>(Resource.Id.textViewOrderSubtitle),
                    Location = view.FindViewById<TextView>(Resource.Id.textViewOrderLocation),
                    WorkStatus = view.FindViewById<TextView>(Resource.Id.textViewOrderStatus)
                };
                view.Tag = holder;
            }

            holder.Title.Text = batch.BatchId.ToString();

            holder.Subtitle.Text = string.IsNullOrEmpty(batch.CurrentLine)
                ? batch.Location
                : $"{batch.Location} - {batch.CurrentLine}";

            if (holder.Location != null)
                holder.Location.Text = string.Empty;

            if (holder.WorkStatus != null)
                holder.WorkStatus.Text = batch.WorkStatus;

            return view;
        }

        public int GetPosition(int batchId)
        {
            var orderIndex = Batches
                .Select((batchInfo, index) => new { Batch = batchInfo, Index = index })
                .Where(tuple => tuple.Batch.BatchId == batchId)
                .Select(tuple => tuple.Index)
                .FirstOrDefault();

            return orderIndex;
        }

        #endregion
    }
}