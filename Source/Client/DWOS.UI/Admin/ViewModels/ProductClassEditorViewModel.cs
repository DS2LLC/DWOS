using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DWOS.UI.Admin.ViewModels
{
    internal class ProductClassEditorViewModel : ViewModelBase
    {
        #region Fields

        public event EventHandler Saved;
        private ProductClassItem _selectedProductClass;
        private readonly ApplicationSettingsDataSet.ProductClassDataTable _dtProductClass =
            new ApplicationSettingsDataSet.ProductClassDataTable();

        #endregion

        #region Methods

        public ObservableCollection<ProductClassItem> ProductClasses { get; } =
            new ObservableCollection<ProductClassItem>();

        public ProductClassItem SelectedProductClass
        {
            get => _selectedProductClass;
            set => Set(nameof(SelectedProductClass), ref _selectedProductClass, value);
        }

        public ICommand AddProductClass { get; }

        public ICommand DeleteProductClass { get; }

        public ICommand Save { get; }

        #endregion

        #region Methods

        public ProductClassEditorViewModel()
        {
            AddProductClass = new RelayCommand(DoAddProductClass);

            DeleteProductClass = new RelayCommand(
                DoDeleteProductClass,
                () => _selectedProductClass != null);

            Save = new RelayCommand(DoSave, CanSave);
        }

        private void DoAddProductClass()
        {
            try
            {
                var newProductClassRow = _dtProductClass.NewProductClassRow();
                newProductClassRow.Name = "Product Class";
                _dtProductClass.AddProductClassRow(newProductClassRow);
                ProductClasses.Add(new ProductClassItem(newProductClassRow));
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error adding product class in Product Class Editor.");
            }
        }

        private void DoDeleteProductClass()
        {
            try
            {
                var selection = _selectedProductClass;

                if (selection == null)
                {
                    return;
                }

                SelectedProductClass = null;
                ProductClasses.Remove(selection);
                selection.Row.Delete();

            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error deleting product class in Product Class Editor.");
            }
        }

        private void DoSave()
        {
            try
            {
                using (var taProductClass = new ProductClassTableAdapter())
                {
                    taProductClass.Update(_dtProductClass);
                }

                Saved?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error saving data in Product Class Editor.");
            }
        }

        private bool CanSave() =>
            CheckDuplicateNames()
                && ProductClasses.All(pc => string.IsNullOrEmpty(pc.ValidateAll()));

        private bool CheckDuplicateNames()
        {
            var duplicateNames = ProductClasses
                .GroupBy(pc => pc.Name)
                .Where(pcGroup => pcGroup.Count() > 1)
                .Select(pcGroup => pcGroup.Key)
                .ToList();

            foreach (var productClass in ProductClasses)
            {
                productClass.MarkAsDuplicate(duplicateNames.Contains(productClass.Name));
            }

            return duplicateNames.Count == 0;
        }

        public void LoadData()
        {
            using (var taProductClass = new ProductClassTableAdapter())
            {
                taProductClass.Fill(_dtProductClass);
            }

            foreach (var productClass in _dtProductClass.OrderBy(pc => pc.Name))
            {
                ProductClasses.Add(new ProductClassItem(productClass));
            }
        }

        #endregion

        #region ProductClassItem

        public class ProductClassItem : ViewModelBase
        {
            #region Fields

            private bool _isDuplicate;

            #endregion

            #region Properties

            public ApplicationSettingsDataSet.ProductClassRow Row { get; }

            public string Name
            {
                get => Row.Name;
                set
                {
                    var nonNullValue = value ?? string.Empty;
                    if (Row.Name != nonNullValue)
                    {
                        Row.Name = nonNullValue;
                        RaisePropertyChanged(nameof(Name));
                    }
                }
            }

            public string AccountingCode
            {
                get => Row.IsAccountingCodeNull()
                    ? null
                    : Row.AccountingCode;

                set
                {
                    var originalAccountingCode = Row.IsAccountingCodeNull()
                        ? null
                        : Row.AccountingCode;

                    if (originalAccountingCode != value)
                    {
                        Row.AccountingCode = value;
                        RaisePropertyChanged(nameof(AccountingCode));
                    }
                }
            }

            public string EmailAddress
            {
                get => Row.IsEmailAddressNull()
                    ? null
                    : Row.EmailAddress;

                set
                {
                    var originalEmailAddress = Row.IsEmailAddressNull()
                        ? null
                        : Row.EmailAddress;

                    if (!string.Equals(originalEmailAddress, value, StringComparison.Ordinal))
                    {
                        Row.EmailAddress = value;
                        RaisePropertyChanged(nameof(EmailAddress));
                    }
                }
            }

            public string Address1
            {
                get => Row.IsAddress1Null()
                    ? null
                    : Row.Address1;

                set
                {
                    var originalAddress = Row.IsAddress1Null()
                        ? null
                        : Row.Address1;

                    if (!string.Equals(originalAddress, value, StringComparison.Ordinal))
                    {
                        Row.Address1 = value;
                        RaisePropertyChanged(nameof(Address1));
                    }
                }
            }

            public string Address2
            {
                get => Row.IsAddress2Null()
                    ? null
                    : Row.Address2;

                set
                {
                    var originalAddress = Row.IsAddress2Null()
                        ? null
                        : Row.Address2;

                    if (!string.Equals(originalAddress, value, StringComparison.Ordinal))
                    {
                        Row.Address2 = value;
                        RaisePropertyChanged(nameof(Address2));
                    }
                }
            }

            public string City
            {
                get => Row.IsCityNull()
                    ? null
                    : Row.City;

                set
                {
                    var originalCity = Row.IsCityNull()
                        ? null
                        : Row.City;

                    if (!string.Equals(originalCity, value, StringComparison.Ordinal))
                    {
                        Row.City = value;
                        RaisePropertyChanged(nameof(City));
                    }
                }
            }

            public string State
            {
                get => Row.IsStateNull()
                    ? null
                    : Row.State;

                set
                {
                    var originalState = Row.IsStateNull()
                        ? null
                        : Row.State;

                    if (!string.Equals(originalState, value, StringComparison.Ordinal))
                    {
                        Row.State = value;
                        RaisePropertyChanged(nameof(State));
                    }
                }
            }

            public string Zip
            {
                get => Row.IsZipNull()
                    ? null
                    : Row.Zip;

                set
                {
                    var originalZip = Row.IsZipNull()
                        ? null
                        : Row.Zip;

                    if (!string.Equals(originalZip, value, StringComparison.Ordinal))
                    {
                        Row.Zip = value;
                        RaisePropertyChanged(nameof(Zip));
                    }
                }
            }

            #endregion

            #region Methods

            public ProductClassItem(ApplicationSettingsDataSet.ProductClassRow row)
            {
                Row = row ?? throw new ArgumentNullException(nameof(row));
            }

            public override string Validate(string propertyName)
            {
                if (propertyName == nameof(Name))
                {
                    if (string.IsNullOrEmpty(Name))
                    {
                        return "Name is required.";
                    }

                    if (_isDuplicate)
                    {
                        return "Name cannot be duplicate.";
                    }
                }

                // Validation of EmailAddress occurs in UI

                return null;
            }

            public override string ValidateAll() => Validate(nameof(Name));

            public void MarkAsDuplicate(bool isDuplicate)
            {
                if (_isDuplicate != isDuplicate)
                {
                    _isDuplicate = isDuplicate;
                    RaisePropertyChanged(nameof(Name));
                }
            }

            #endregion
        }

        #endregion
    }
}
