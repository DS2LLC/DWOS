using DWOS.Data;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    internal class SettingsProvider : ISettingsProvider
    {
        #region ISettingsManager Members

        public int PriceDecimalPlaces =>
            ApplicationSettings.Current.PriceDecimalPlaces;

        public PricingType PartPricingType =>
            ApplicationSettings.Current.PartPricingType;

        public bool DisplayProcessCocByDefault =>
            ApplicationSettings.Current.DisplayProcessCOCByDefault;

        public bool UseLeadTimeDayScheduling =>
            ApplicationSettings.Current.SchedulerType == SchedulerType.ProcessLeadTime &&
            ApplicationSettings.Current.SchedulingEnabled;

        public bool UseLeadTimeHourScheduling =>
            ApplicationSettings.Current.SchedulerType == SchedulerType.ProcessLeadTimeHour &&
            ApplicationSettings.Current.SchedulingEnabled;

        public string WorkStatusChangingDepartment =>
            ApplicationSettings.Current.WorkStatusChangingDepartment;

        public string WorkStatusPendingOrderReview =>
            ApplicationSettings.Current.WorkStatusPendingOR;

        public bool OrderReviewEnabled =>
            ApplicationSettings.Current.OrderReviewEnabled;

        public string DepartmentSales =>
            ApplicationSettings.Current.DepartmentSales;

        #endregion
    }
}
