using DWOS.AutomatedWorkOrderTool.ViewModel;

namespace DWOS.AutomatedWorkOrderTool.Messages
{
    public class AddOspFormatMessage
    {
        #region Properties

        public CustomerViewModel Customer { get; }

        public ManufacturerViewModel Manufacturer { get; }

        #endregion

        #region Methods

        public AddOspFormatMessage(CustomerViewModel customer, ManufacturerViewModel manufacturer)
        {
            Customer = customer;
            Manufacturer = manufacturer;
        }

        #endregion
    }
}
