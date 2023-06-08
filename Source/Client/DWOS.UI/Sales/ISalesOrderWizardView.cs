using DWOS.UI.Sales.ViewModels;
using static DWOS.UI.Sales.ViewModels.SalesOrderWizardViewModel;

namespace DWOS.UI.Sales
{

    public interface ISalesOrderWizardView
    {
        WorkOrderViewModel ShowAddOrderDialog(SalesOrderWizardViewModel mainViewModel);

        WorkOrderViewModel ShowEditOrderDialog(SalesOrderWizardViewModel mainViewModel, WorkOrderViewModel workOrder);

        bool? ShowDeleteOrderDialog();

        bool? ShowEditFeesDialog(SalesOrderWizardViewModel salesOrderWizardViewModel);

        bool ShowPriorityWarning();

        void ShowWorkOrderProcessesError();

        void ShowRushChargeWarning(decimal currentRushCharge, decimal minimumRushCharge);
    }
}
