using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.UI.Sales.Models;
using DWOS.UI.Sales.ViewModels;
using DWOS.UI.Utilities;
using Infragistics.Windows.Editors;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.UI.Sales
{
    /// <summary>
    /// Interaction logic for CustomerFeesDialog.xaml
    /// </summary>
    public partial class CustomerFeesDialog
    {
        #region Fields

        private readonly PersistWpfWindowState _persistState;

        #endregion

        #region Properties

        public IEnumerable<CustomerFeeViewModel> CustomerFees => ViewModel.CustomerFees;

        #endregion

        #region Methods

        public CustomerFeesDialog()
        {
            _persistState = new PersistWpfWindowState(this);
            Icon = Properties.Resources.MoneyBagUSDollar32.ToWpfImage();
            InitializeComponent();
            (Resources["CustomerFeeOptions"] as ComboBoxItemsProvider).ItemsSource = ViewModel.OrderFeeTypeIds;

            var priceDecimalPlaces = ApplicationSettings.Current.PriceDecimalPlaces;
            Resources["CurrencyMask"] = $"{{currency:-5.{priceDecimalPlaces}}}";
            Resources["PercentageMask"] = $"-nnn.{string.Concat(Enumerable.Repeat('n', priceDecimalPlaces))} %";
        }

        public void LoadData(CustomersDataset dsCustomers, int customerId)
        {
            ViewModel.LoadData(dsCustomers, customerId);
        }

        #endregion

        #region Events

        private void ViewModel_Accepted(object sender, System.EventArgs e)
        {
            DialogResult = true;
        }

        #endregion

    }
}
