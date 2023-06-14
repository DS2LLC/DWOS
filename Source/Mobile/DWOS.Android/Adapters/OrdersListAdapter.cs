using System;
using Android.Widget;
using DWOS.Services.Messages;
using Android.Views;
using Android.App;
using System.Collections.Generic;
using System.Linq;
using DWOS.Utilities;

namespace DWOS.Android
{
    /// <summary>
    /// <see cref="BaseAdapter"/> implementation for Order Summaries
    /// </summary>
    public class OrdersListAdapter : BaseAdapter<OrderInfo>
    {
        #region Private Classes
        /// <summary>
        /// Utility class that can hold Views for an Adapter so it doesn't have to call FindViewById
        /// repeatedly.
        /// </summary>
        private class OrdersListViewHolder : Java.Lang.Object
        {
            public TextView Title { get; set; }
            public TextView Subtitle { get; set; }
            public TextView Location { get; set; }
            public TextView WorkStatus { get; set; }
        } 
        #endregion

        #region Fields

        Activity _context;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the list of orders for this instance.
        /// </summary>
        public IList<OrderInfo> Orders { get; set; }

        #endregion

        #region Methods

        public OrdersListAdapter(Activity activity, IList<OrderInfo> orders)
            : base()
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            if (orders == null)
            {
                throw new ArgumentNullException(nameof(orders));
            }

            Orders = orders;
            _context = activity;
        }

        public override long GetItemId (int position)
        {
            if (position > Orders.Count)
                return -1;

            return Orders[position].OrderId;
        }

        public override View GetView (int position, View convertView, ViewGroup parent)
        {
            if (position > Orders.Count)
                return null;

            OrdersListViewHolder holder = null;
            var order = Orders [position];
            var view = convertView;
            if (view != null)
                holder = view.Tag as OrdersListViewHolder;

            try
            {
                if (holder == null)
                {
                    view = _context.LayoutInflater.Inflate(Resource.Layout.OrderListItemLayout, null);
                    holder = new OrdersListViewHolder()
                    {
                        Title = view.FindViewById<TextView>(Resource.Id.textViewOrderTitle),
                        Subtitle = view.FindViewById<TextView>(Resource.Id.textViewOrderSubtitle),
                        Location = view.FindViewById<TextView>(Resource.Id.textViewOrderLocation),
                        WorkStatus = view.FindViewById<TextView>(Resource.Id.textViewOrderStatus)
                    };
                    view.Tag = holder;
                }

                holder.Title.Text = order.OrderId.ToString();
                holder.Subtitle.Text = order.PartName;

                if (holder.Location != null)
                {
                    if (string.IsNullOrEmpty(order.CurrentLine))
                    {
                        holder.Location.Text = order.Location;
                    }
                    else
                    {
                        holder.Location.Text = $"{order.Location} - {order.CurrentLine}";
                    }
                }

                if (holder.WorkStatus != null)
                    holder.WorkStatus.Text = order.WorkStatus;
            }
            catch (Exception exception)
            {
                var logSvc = ServiceContainer.Resolve<ILogService>();
                logSvc.LogError("Error in GetView", exception);
            }

            return view;
        }
        public override int Count { get { return Orders.Count; } }

        public override OrderInfo this [int index] { get { return Orders [index]; } }

        public int GetPosition(int orderId)
        {
            var orderIndex = Orders
                .Select((orderInfo, index) => new { Order = orderInfo, Index = index })
                .Where(tuple => tuple.Order.OrderId == orderId)
                .Select(tuple => tuple.Index)
                .FirstOrDefault();

            return orderIndex;
        }
        #endregion
    }
}

