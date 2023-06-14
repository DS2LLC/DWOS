using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DWOS.AutomatedWorkOrderTool.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using DWOS.AutomatedWorkOrderTool.Services;

namespace DWOS.AutomatedWorkOrderTool.ViewModel
{
    public class AddManufacturerViewModel : ViewModelBase
    {
        #region Fields

        private ManufacturerViewModel _selectedManufacturer;

        #endregion

        #region Properties

        public IDataManager DataManager { get; }
        public ICustomerManager CustomerManager { get; }
        public ObservableCollection<ManufacturerViewModel> Manufacturers { get; } =
            new ObservableCollection<ManufacturerViewModel>();

        public ManufacturerViewModel SelectedManufacturer
        {
            get => _selectedManufacturer;
            set => Set(nameof(SelectedManufacturer), ref _selectedManufacturer, value);
        }

        #endregion

        #region Methods

        public AddManufacturerViewModel(IDataManager dataManager, ICustomerManager customerManager, IMessenger messenger) :
            base(messenger)
        {
            DataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
            CustomerManager = customerManager ?? throw new ArgumentNullException(nameof(customerManager));
        }

        public void LoadData(CustomerViewModel customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            var existingManufacturers = customer.Formats.Select(f => f.Manufacturer).ToList();

            Manufacturers.Clear();

            var mfgToShow = new List<ManufacturerViewModel>();

            using (var dtManufacturerDataTable = new AwotDataSet.d_ManufacturerDataTable())
            {
                DataManager.LoadManufacturers(dtManufacturerDataTable);

                foreach (var mfgRow in dtManufacturerDataTable)
                {
                    if (existingManufacturers.Contains(mfgRow.ManufacturerID))
                    {
                        continue;
                    }

                    mfgToShow.Add(ManufacturerViewModel.From(mfgRow));
                }
            }

            var sortedMfg = mfgToShow.OrderBy(c => c.Name).ToList();
            foreach (var mfg in sortedMfg)
            {
                Manufacturers.Add(mfg);
            }

            SelectedManufacturer = sortedMfg.FirstOrDefault();
        }

        #endregion
    }
}
