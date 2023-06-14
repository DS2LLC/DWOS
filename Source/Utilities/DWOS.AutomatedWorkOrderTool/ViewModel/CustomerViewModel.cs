using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DWOS.AutomatedWorkOrderTool.Model;
using GalaSoft.MvvmLight;

namespace DWOS.AutomatedWorkOrderTool.ViewModel
{
    public class CustomerViewModel : ViewModelBase, ISelectable
    {
        #region Fields

        private bool _isSelected;

        #endregion

        #region Properties

        public int Id { get; private set; }

        public string Name { get; private set; }

        public ObservableCollection<OspFormatViewModel> Formats { get; } =
            new ObservableCollection<OspFormatViewModel>();

        #endregion

        #region Methods

        private CustomerViewModel()
        {

        }

        public static CustomerViewModel Test(int id, string name, IEnumerable<OspFormatViewModel> formats)
        {
            var customer = new CustomerViewModel
            {
                Id = id,
                Name = name
            };

            foreach (var format in formats)
            {
                customer.Formats.Add(format);
            }

            return customer;
        }

        internal static CustomerViewModel From(AwotDataSet.CustomerRow customerRow)
        {
            if (customerRow == null)
            {
                return null;
            }

            var customer = new CustomerViewModel
            {
                Id = customerRow.CustomerID,
                Name = customerRow.Name
            };

            foreach (var ospFormat in customerRow.GetOSPFormatRows().OrderBy(f => f.ManufacturerID))
            {
                customer.Formats.Add(OspFormatViewModel.From(ospFormat));
            }

            return customer;
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
