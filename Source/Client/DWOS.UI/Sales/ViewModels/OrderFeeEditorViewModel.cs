using DWOS.Data;
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

namespace DWOS.UI.Sales.ViewModels
{
    public class OrderFeeEditorViewModel : Utilities.ViewModelBase
    {
        #region Fields

        public event EventHandler Accepted;

        #endregion

        #region Properties

        public ObservableCollection<OrderFeeType> FeeTypes { get; } =
            new ObservableCollection<OrderFeeType>();

        public ObservableCollection<string> OrderFeeTypeIds { get; } =
            new ObservableCollection<string>();

        public ObservableCollection<OrderFeeViewModel> OrderFees { get; } =
            new ObservableCollection<OrderFeeViewModel>();

        public ObservableCollection<CustomerFee> DefaultCustomerFees { get; } =
            new ObservableCollection<CustomerFee>();

        public ICommand AddDefaultFees { get; }

        public ICommand Accept { get; }

        public IDwosApplicationSettingsProvider SettingsProvider { get; }

        internal SalesOrderWizardPersistence Persistence { get; }

        #endregion

        #region Methods

        public OrderFeeEditorViewModel()
        {
            Utilities.ISecurityManager securityManager;

            if (IsInDesignMode)
            {
                SettingsProvider = new DesignTime.FakeSettingsProvider();
                securityManager = new DesignTime.FakeSecurityManager();
            }
            else
            {
                SettingsProvider = DependencyContainer.Resolve<IDwosApplicationSettingsProvider>();
                securityManager = Utilities.SecurityManager.Current;
            }

            Persistence = new SalesOrderWizardPersistence(SettingsProvider, securityManager, new DateTimeNowProvider());
            OrderFees.CollectionChanged += OrderFees_CollectionChanged;

            AddDefaultFees = new RelayCommand(DoAddDefaultFees);
            Accept = new RelayCommand(DoAccept, CanAccept);
        }

        public void LoadData(IEnumerable<OrderFeeViewModel> existingFees, CustomerSummary customer)
        {
            foreach (var fee in existingFees)
            {
                OrderFees.Add(fee);
            }

            var feeTypes = Persistence.RetrieveOrderFeeTypes();

            foreach (var feeType in feeTypes.OrderBy(type => type.OrderFeeTypeId))
            {
                FeeTypes.Add(feeType);
                OrderFeeTypeIds.Add(feeType.OrderFeeTypeId);
            }

            foreach (var customerFee in Persistence.RetrieveDefaultFees(customer).OrderBy(fee => fee.OrderFeeTypeId))
            {
                DefaultCustomerFees.Add(customerFee);
            }
        }

        private void DoAddDefaultFees()
        {
            try
            {
                // Add customer-default fees
                foreach (var defaultCustomerFee in DefaultCustomerFees)
                {
                    var hasMatchingFee = OrderFees
                        .Any(orderFee => orderFee.OrderFeeTypeId == defaultCustomerFee.OrderFeeTypeId);

                    var matchingFeeType = FeeTypes
                        .FirstOrDefault(feeType => feeType.OrderFeeTypeId == defaultCustomerFee.OrderFeeTypeId);

                    if (!hasMatchingFee && matchingFeeType != null)
                    {
                        var defaultFee = OrderFeeViewModel.From(matchingFeeType);
                        defaultFee.Charge = defaultCustomerFee.Charge;
                        OrderFees.Add(defaultFee);
                    }
                }

                // Add system-default fees
                foreach (var defaultFeeType in FeeTypes.Where(f => f.IsDefault))
                {
                    var hasMatchingFee = OrderFees
                        .Any(orderFee => orderFee.OrderFeeTypeId == defaultFeeType.OrderFeeTypeId);

                    if (!hasMatchingFee)
                    {
                        OrderFees.Add(OrderFeeViewModel.From(defaultFeeType));
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
                Accepted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error accepting dialog.");
            }
        }

        private bool CanAccept() => OrderFees
            .All(fee => string.IsNullOrEmpty(fee.ValidateAll()));

        #endregion

        #region Events

        private void OrderFees_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == NotifyCollectionChangedAction.Move)
                {
                    return;
                }

                var oldFees = e.OldItems?.OfType<OrderFeeViewModel>()
                    ?? Enumerable.Empty<OrderFeeViewModel>();

                foreach (var oldFee in oldFees)
                {
                    oldFee.PropertyChanged -= NewFee_PropertyChanged;
                }

                var newFees = e.NewItems?.OfType<OrderFeeViewModel>()
                    ?? Enumerable.Empty<OrderFeeViewModel>();

                foreach (var newFee in newFees)
                {
                    newFee.PropertyChanged += NewFee_PropertyChanged;
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
                var feeViewModel = sender as OrderFeeViewModel;

                if (feeViewModel == null || e.PropertyName != nameof(OrderFeeViewModel.OrderFeeTypeId))
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
