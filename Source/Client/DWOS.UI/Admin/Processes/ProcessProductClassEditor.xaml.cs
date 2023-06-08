using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using Infragistics.Windows.Editors;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DWOS.UI.Admin.Processes
{
    /// <summary>
    /// Interaction logic for ProcessProductClassEditor.xaml
    /// </summary>
    public partial class ProcessProductClassEditor
    {
        #region Fields

        private readonly GridSettingsPersistence<XamDataGridSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<XamDataGridSettings>("ProcessProductClassEditor", new XamDataGridSettings());

        #endregion

        #region Properties

        private DialogContext ViewModel =>
            DataContext as DialogContext;

        #endregion

        #region Methods

        public ProcessProductClassEditor()
        {
            InitializeComponent();
            DataContext = new DialogContext();
            (Resources["ProductClassOptions"] as ComboBoxItemsProvider).ItemsSource = ViewModel.ProductClassOptions;
            Icon = Properties.Resources.Process_32.ToWpfImage();
        }

        public void Load(ProcessesDataset dsProcesses, ProcessesDataset.ProcessRow processRow)
        {
            ViewModel?.Load(dsProcesses, processRow);
        }

        public void ApplyChanges()
        {
            ViewModel?.ApplyChanges();
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Set mask for material cost
                var decimalPlaces = ApplicationSettings.Current.PriceDecimalPlaces;
                Resources["CurrencyMask"] = $"{{currency:3.{decimalPlaces}}}";

                // Load layout
                _gridSettingsPersistence.LoadSettings().ApplyTo(ProductClassGrid);

                // Register view model event handlers
                var vm = ViewModel;

                if (vm != null)
                {
                    vm.Accepted += Vm_Accepted;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading process product class editor.");
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save layout
                var settings = new XamDataGridSettings();
                settings.RetrieveSettingsFrom(ProductClassGrid);
                _gridSettingsPersistence.SaveSettings(settings);

                // Unregister view model event handlers
                var vm = ViewModel;

                if (vm != null)
                {
                    vm.Accepted -= Vm_Accepted;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error unloading process product class editor.");
            }
        }

        private void Vm_Accepted(object sender, EventArgs e)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error handling accepted event.");
            }
        }

        #endregion

        #region DialogContext

        private class DialogContext : ViewModelBase
        {
            #region Fields

            public event EventHandler Accepted;
            private ProductClassItem _selectedProductClass;
            private ProcessesDataset _dsProcesses;
            private ProcessesDataset.ProcessRow _processRow;
            private string _errorMessage;
            private readonly List<ProcessesDataset.ProcessProductClassRow> _rowsToDelete =
                new List<ProcessesDataset.ProcessProductClassRow>();

            #endregion

            #region Properties

            public string ProcessName => _processRow?.Name;

            public ObservableCollection<ProductClassItem> ProductClasses { get; } =
                new ObservableCollection<ProductClassItem>();

            public ObservableCollection<string> ProductClassOptions { get; } =
                new ObservableCollection<string>();

            public ProductClassItem SelectedProductClass
            {
                get => _selectedProductClass;
                set
                {
                    if (_selectedProductClass != value)
                    {
                        _selectedProductClass = value;
                        RaisePropertyChanged(nameof(SelectedProductClass));
                    }
                }
            }

            public ICommand Accept { get; }

            public ICommand AddProductClass { get; }

            public ICommand DeleteProductClass { get; }

            public string ErrorMessage
            {
                get => _errorMessage;
                set
                {
                    if (_errorMessage != value)
                    {
                        _errorMessage = value;
                        RaisePropertyChanged(nameof(ErrorMessage));
                    }
                }
            }

            #endregion

            #region Methods

            public DialogContext()
            {
                Accept = new RelayCommand(DoAccept, CanAccept);
                AddProductClass = new RelayCommand(DoAddProductClass);
                DeleteProductClass = new RelayCommand(DoDeleteProductClass, CanDeleteProductClass);
            }

            private void DoAccept()
            {
                if (!CanAccept())
                {
                    return;
                }

                Accepted?.Invoke(this, EventArgs.Empty);
            }

            private bool CanAccept()
            {
                if (!ProductClasses.All(c => string.IsNullOrEmpty(c.Error)))
                {
                    ErrorMessage = "Empty product class.";
                    return false;
                }

                var distinctProductClassNames = ProductClasses.Select(c => c.ProductClass).Distinct();
                if (distinctProductClassNames.Count() != ProductClasses.Count)
                {
                    ErrorMessage = "Duplicate product class";
                    return false;
                }

                ErrorMessage = string.Empty;
                return true;
            }

            private void DoAddProductClass()
            {
                ProductClasses.Add(new ProductClassItem());
            }

            private void DoDeleteProductClass()
            {
                if (!CanDeleteProductClass())
                {
                    return;
                }

                if (_selectedProductClass.Row != null)
                {
                    _rowsToDelete.Add(_selectedProductClass.Row);
                }

                ProductClasses.Remove(_selectedProductClass);
                SelectedProductClass = null;
            }

            private bool CanDeleteProductClass() =>
                _selectedProductClass != null;

            public void ApplyChanges()
            {
                // Delete rows
                foreach (var rowToDelete in _rowsToDelete)
                {
                    rowToDelete.Delete();
                }

                // Add/update
                foreach (var productClassItem in ProductClasses)
                {
                    if (productClassItem.Row == null)
                    {
                        // Create new row
                        var newRow = _dsProcesses.ProcessProductClass.NewProcessProductClassRow();
                        newRow.ProcessRow = _processRow;
                        newRow.ProductClass = productClassItem.ProductClass;

                        if (productClassItem.MaterialUnitCost.HasValue)
                        {
                            newRow.MaterialUnitCost = productClassItem.MaterialUnitCost.Value;
                        }

                        if (!string.IsNullOrEmpty(productClassItem.MaterialUnit))
                        {
                            newRow.MaterialUnit = productClassItem.MaterialUnit;
                        }

                        _dsProcesses.ProcessProductClass.AddProcessProductClassRow(newRow);
                    }
                    else
                    {
                        // Edit existing row
                        var originalRow = productClassItem.Row;
                        originalRow.ProductClass = productClassItem.ProductClass;

                        if (productClassItem.MaterialUnitCost.HasValue)
                        {
                            originalRow.MaterialUnitCost = productClassItem.MaterialUnitCost.Value;
                        }
                        else
                        {
                            originalRow.SetMaterialUnitCostNull();
                        }

                        if (!string.IsNullOrEmpty(productClassItem.MaterialUnit))
                        {
                            originalRow.MaterialUnit = productClassItem.MaterialUnit;
                        }
                        else
                        {
                            originalRow.SetMaterialUnitNull();
                        }
                    }
                }
            }

            public void Load(ProcessesDataset dsProcesses, ProcessesDataset.ProcessRow processRow)
            {
                _dsProcesses = dsProcesses ?? throw new ArgumentNullException(nameof(dsProcesses));
                _processRow = processRow ?? throw new ArgumentNullException(nameof(processRow));
                RaisePropertyChanged(nameof(ProcessName));

                _rowsToDelete.Clear();
                ProductClassOptions.Clear();
                ProductClasses.Clear();

                // Load options for product class dropdown
                foreach (var productClass in _dsProcesses.ProductClass.OrderBy(r => r.Name))
                {
                    ProductClassOptions.Add(productClass.Name);
                }

                // Load process <-> product class relations
                foreach (var productClassRow in processRow.GetProcessProductClassRows().OrderBy(r => r.IsProductClassNull() ? null : r.ProductClass))
                {
                    ProductClasses.Add(new ProductClassItem(productClassRow));
                }
            }

            #endregion
        }

        #endregion

        #region ProductClassItem

        private class ProductClassItem : ViewModelBase
        {
            #region Fields

            private string _productClass;
            private decimal? _materialUnitCost;
            private string _materialUnit;

            #endregion

            #region Properties

            public ProcessesDataset.ProcessProductClassRow Row { get; }

            public string ProductClass
            {
                get => _productClass;
                set
                {
                    if (_productClass != value)
                    {
                        _productClass = value;
                        RaisePropertyChanged(nameof(ProductClass));
                    }
                }
            }

            public decimal? MaterialUnitCost
            {
                get => _materialUnitCost;
                set
                {
                    if (_materialUnitCost != value)
                    {
                        _materialUnitCost = value;
                        RaisePropertyChanged(nameof(MaterialUnitCost));
                    }
                }
            }

            public string MaterialUnit
            {
                get => _materialUnit;
                set
                {
                    if (_materialUnit != value)
                    {
                        _materialUnit = value;
                        RaisePropertyChanged(nameof(MaterialUnit));
                    }
                }
            }

            #endregion

            #region Methods

            public ProductClassItem()
            {

            }

            public ProductClassItem(ProcessesDataset.ProcessProductClassRow processProductClassRow)
            {
                Row = processProductClassRow ?? throw new ArgumentNullException(nameof(processProductClassRow));
                _productClass = Row.IsProductClassNull() ? null : Row.ProductClass;
                _materialUnitCost = Row.IsMaterialUnitCostNull() ? (decimal?) null : Row.MaterialUnitCost;
                _materialUnit = Row.IsMaterialUnitNull() ? null : Row.MaterialUnit;
            }

            public override string Validate(string propertyName)
            {
                switch (propertyName)
                {
                    case nameof(ProductClass):
                        {
                            return string.IsNullOrEmpty(_productClass)
                                ? "Product class is required."
                                : string.Empty;
                        }

                    case nameof(MaterialUnit):
                        {
                            return _materialUnitCost.HasValue && string.IsNullOrEmpty(_materialUnit)
                               ? "Material Unit is required."
                               : string.Empty;
                        }

                    default:
                        {
                            return string.Empty;
                        }
                }
            }

            public override string ValidateAll()
            {
                return Validate(nameof(ProductClass));
            }

            #endregion
        }

        #endregion
    }
}
