using DWOS.Data;
using DWOS.UI.Sales.Models;
using DWOS.UI.Sales.ViewModels;
using DWOS.UI.Utilities;
using Infragistics.Windows.Editors;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.UI.Sales.Dialogs
{
    /// <summary>
    /// Interaction logic for OrderFeesDialog.xaml
    /// </summary>
    public partial class OrderFeesDialog
    {
        #region Fields

        private readonly PersistWpfWindowState _persistState;

        #endregion

        #region Properties

        public IEnumerable<OrderFeeViewModel> OrderFees => ViewModel.OrderFees;

        #endregion

        #region Methods

        public OrderFeesDialog()
        {
            _persistState = new PersistWpfWindowState(this);
            Icon = Properties.Resources.MoneyBagUSDollar32.ToWpfImage();
            InitializeComponent();
            (Resources["OrderFeeOptions"] as ComboBoxItemsProvider).ItemsSource = ViewModel.OrderFeeTypeIds;

            var priceDecimalPlaces = ApplicationSettings.Current.PriceDecimalPlaces;
            Resources["CurrencyMask"] = $"{{currency:-5.{priceDecimalPlaces}:c}}";
            Resources["PercentageMask"] = $"-nnn.{string.Concat(Enumerable.Repeat('n', priceDecimalPlaces))} %";
        }

        public void LoadData(IEnumerable<OrderFeeViewModel> fees, CustomerSummary customer)
        {
            ViewModel.LoadData(fees, customer);
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
