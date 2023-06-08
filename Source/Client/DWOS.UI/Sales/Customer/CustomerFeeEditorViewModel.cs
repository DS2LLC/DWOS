using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Date;
using DWOS.Shared.Utilities;
using DWOS.UI.Sales.Models;
using GalaSoft.MvvmLight.CommandWpf;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace DWOS.UI.Sales
{
    public class CustomerFeeEditorViewModel : Utilities.ViewModelBase
    {
        #region Fields

        public event EventHandler Accepted;
        private readonly List<CustomerFeeViewModel> _removedFees =
            new List<CustomerFeeViewModel>();

        #endregion

        #region Properties

        public ObservableCollection<OrderFeeType> FeeTypes { get; } =
            new ObservableCollection<OrderFeeType>();

        public ObservableCollection<string> OrderFeeTypeIds { get; } =
            new ObservableCollection<string>();

        public ObservableCollection<CustomerFeeViewModel> CustomerFees { get; } =
            new ObservableCollection<CustomerFeeViewModel>();

        public ICommand AddDefaultFees { get; }

        public ICommand Accept { get; }

        public IDwosApplicationSettingsProvider SettingsProvider { get; }

        internal OrderFeePersistence Persistence { get; }

        public CustomersDataset Dataset { get; private set; }

        public int CustomerId { get; private set; }

        #endregion

        #region Methods

        public CustomerFeeEditorViewModel()
        {
            if (IsInDesignMode)
            {
                SettingsProvider = new DesignTime.FakeSettingsProvider();
            }
            else
            {
                SettingsProvider = DependencyContainer.Resolve<IDwosApplicationSettingsProvider>();
            }

            Persistence = new OrderFeePersistence(SettingsProvider);
            CustomerFees.CollectionChanged += CustomerFees_CollectionChanged;

            AddDefaultFees = new RelayCommand(DoAddDefaultFees);
            Accept = new RelayCommand(DoAccept, CanAccept);
        }

        public void LoadData(CustomersDataset dataset, int customerId)
        {
            Dataset = dataset
                ?? throw new ArgumentNullException(nameof(dataset));

            CustomerId = customerId;

            var feeTypes = Persistence.RetrieveOrderFeeTypes();

            foreach (var feeType in feeTypes.OrderBy(type => type.OrderFeeTypeId))
            {
                FeeTypes.Add(feeType);
                OrderFeeTypeIds.Add(feeType.OrderFeeTypeId);
            }

            var customerRow = dataset.Customer.FindByCustomerID(customerId);

            if (customerRow != null)
            {
                foreach (var feeRow in customerRow.GetCustomerFeeRows())
                {
                    var feeType = FeeTypes.FirstOrDefault(type => type.OrderFeeTypeId == feeRow.OrderFeeTypeID);

                    if (feeType != null)
                    {
                        CustomerFees.Add(CustomerFeeViewModel.From(feeRow, feeType));
                    }
                }
            }
        }

        private void DoAddDefaultFees()
        {
            try
            {
                // Add system-default fees
                foreach (var defaultFeeType in FeeTypes.Where(f => f.IsDefault))
                {
                    var hasMatchingFee = CustomerFees
                        .Any(orderFee => orderFee.OrderFeeTypeId == defaultFeeType.OrderFeeTypeId);

                    if (!hasMatchingFee)
                    {
                        CustomerFees.Add(CustomerFeeViewModel.From(defaultFeeType));
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error adding default fees.");
            }
        }


        private void DoAccept()
        {
            try
            {
                if (!CanAccept() || Dataset == null || CustomerId == 0)
                {
                    return;
                }

                foreach (var fee in CustomerFees)
                {
                    if (fee.Row == null)
                    {
                        // Create new
                        var feeRow = Dataset.CustomerFee.NewCustomerFeeRow();
                        feeRow.CustomerID = CustomerId;
                        feeRow.OrderFeeTypeID = fee.OrderFeeTypeId;
                        feeRow.Charge = fee.Charge;

                        Dataset.CustomerFee.AddCustomerFeeRow(feeRow);
                    }
                    else
                    {
                        // Update existing
                        if (fee.Row.OrderFeeTypeID != fee.OrderFeeTypeId)
                        {
                            fee.Row.OrderFeeTypeID = fee.OrderFeeTypeId;
                        }

                        if (fee.Row.Charge != fee.Charge)
                        {
                            fee.Row.Charge = fee.Charge;
                        }
                    }
                }

                foreach (var fee in _removedFees)
                {
                    fee.Row?.Delete();
                }

                _removedFees.Clear();

                Accepted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error accepting dialog.");
            }
        }

        private bool CanAccept() => CustomerFees
            .All(fee => string.IsNullOrEmpty(fee.ValidateAll()));

        #endregion

        #region Events

        private void CustomerFees_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == NotifyCollectionChangedAction.Move)
                {
                    return;
                }

                var oldFees = e.OldItems?.OfType<CustomerFeeViewModel>()
                    ?? Enumerable.Empty<CustomerFeeViewModel>();

                foreach (var oldFee in oldFees)
                {
                    oldFee.PropertyChanged -= NewFee_PropertyChanged;
                    _removedFees.Add(oldFee);
                }

                var newFees = e.NewItems?.OfType<CustomerFeeViewModel>()
                    ?? Enumerable.Empty<CustomerFeeViewModel>();

                foreach (var newFee in newFees)
                {
                    newFee.PropertyChanged += NewFee_PropertyChanged;

                    // Make sure that the new fee isn't removed on apply;
                    // probably not needed.
                    _removedFees.Remove(newFee);
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling collection changed event.");
            }
        }

        private void NewFee_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                var feeViewModel = sender as CustomerFeeViewModel;

                if (feeViewModel == null || e.PropertyName != nameof(CustomerFeeViewModel.OrderFeeTypeId))
                {
                    return;
                }

                var matchingFeeType = FeeTypes
                    .FirstOrDefault(type => type.OrderFeeTypeId == feeViewModel.OrderFeeTypeId);

                if (matchingFeeType == null)
                {
                    LogManager.GetCurrentClassLogger()
                        .Warn($"Cannot find fee type {feeViewModel.OrderFeeTypeId}");

                    feeViewModel.FeeType = OrderPrice.enumFeeType.Fixed;
                    feeViewModel.Charge = 1M;
                    feeViewModel.IsDiscount = false;
                }
                else
                {
                    feeViewModel.FeeType = matchingFeeType.FeeType;
                    feeViewModel.IsDiscount = matchingFeeType.Price < 0;
                    feeViewModel.Charge = matchingFeeType.Price;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling order fee type change");
            }
        }

        #endregion
    }
}
