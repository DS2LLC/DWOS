using DWOS.AutomatedWorkOrderTool.Model;
using GalaSoft.MvvmLight;

namespace DWOS.AutomatedWorkOrderTool.ViewModel
{
    public class OspFormatViewModel : ViewModelBase, ISelectable
    {
        #region Fields

        private bool _isSelected;

        #endregion

        #region Properties

        public int CustomerId { get; private set; }

        public string Manufacturer { get; private set; }

        public int OspFormatId { get; private set; }

        #endregion

        #region Methods

        private OspFormatViewModel()
        {

        }

        public static OspFormatViewModel Test(int customerId, string manufacturer, int ospFormatId) =>
            new OspFormatViewModel
            {
                CustomerId = customerId,
                Manufacturer = manufacturer,
                OspFormatId = ospFormatId
            };

        internal static OspFormatViewModel From(AwotDataSet.OSPFormatRow formatRow)
        {
            if (formatRow == null)
            {
                return null;
            }

            return new OspFormatViewModel
            {
                CustomerId = formatRow.CustomerID,
                Manufacturer = formatRow.ManufacturerID,
                OspFormatId = formatRow.OSPFormatID
            };
        }

        #endregion

        #region ISelectable Members

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(nameof(IsSelected), ref _isSelected, value);
        }

        #endregion
    }
}
