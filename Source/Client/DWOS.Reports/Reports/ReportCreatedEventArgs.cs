using System;

namespace DWOS.Reports
{
    public class ReportCreatedEventArgs : EventArgs
    {
        public int OrderId { get; }

        public string Title { get; }

        public string UserName { get; }

        public ReportCreatedEventArgs(int orderId, string title, string userName)
        {
            OrderId = orderId;
            Title = title;
            UserName = userName;
        }
    }
}
