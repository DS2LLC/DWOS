using DWOS.Data;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    public interface ISettingsProvider
    {
        int PriceDecimalPlaces { get; }

        PricingType PartPricingType { get; }

        bool DisplayProcessCocByDefault { get; }

        bool UseLeadTimeDayScheduling { get; }

        bool UseLeadTimeHourScheduling { get; }

        string WorkStatusChangingDepartment { get; }

        string WorkStatusPendingOrderReview { get; }

        bool OrderReviewEnabled { get; }

        string DepartmentSales { get; }
    }
}
