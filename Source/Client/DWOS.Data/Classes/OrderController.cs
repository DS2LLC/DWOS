namespace DWOS.Data
{
    public static class OrderControllerExtensions
    {
        public static string GetNewOrderWorkStatus(int customerID, bool userRequiresOrderReview)
        {
            var systemUsesOrderReview = ApplicationSettings.Current.OrderReviewEnabled;

            var customerUsesOrderReview = false;

            using(var ta = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.CustomerSummaryTableAdapter {ClearBeforeFill = false})
            {
                customerUsesOrderReview = ta.GetOrderReview(customerID).GetValueOrDefault();
            }

            if (systemUsesOrderReview && customerUsesOrderReview && userRequiresOrderReview)
            {
                return ApplicationSettings.Current.WorkStatusPendingOR;
            }

            //else then skip order review
            return ApplicationSettings.Current.WorkStatusChangingDepartment;
        }    
    }
}