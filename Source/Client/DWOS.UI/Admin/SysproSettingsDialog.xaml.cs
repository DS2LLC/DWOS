using DWOS.Data;
using DWOS.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace DWOS.UI.Admin
{
    /// <summary>
    /// Interaction logic for SysproConfigurationDialog.xaml
    /// </summary>
    public partial class SysproSettingsDialog
    {
        #region Properties

        public SysproInvoiceSettings Settings
        {
            get
            {
                var vm = DataContext as DialogContext;
                return vm?.Settings;
            }
        }

        #endregion

        #region Methods

        public SysproSettingsDialog()
        {
            InitializeComponent();
            Icon = Properties.Resources.Settings_32.ToWpfImage();
            DataContext = new DialogContext();

            Width = 350;
            Height = 600;
        }

        public void Load(SysproInvoiceSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var vm = DataContext as DialogContext;
            if (vm == null)
            {
                return;
            }

            vm.Load(settings);
        }

        #endregion

        #region Events

        private void AddOrderHeaderButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = DataContext as DialogContext;
            if (vm == null)
            {
                return;
            }

            var dialog = new SysproNewFieldDialog() { Owner = this };
            if (dialog.ShowDialog() ?? false)
            {
                vm.Settings.OrderHeaderMap.Add(dialog.CreateField());
                vm.Refresh();
            }
        }

        private void AddStockLineButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = DataContext as DialogContext;
            if (vm == null)
            {
                return;
            }

            var dialog = new SysproNewFieldDialog() { Owner = this };
            if (dialog.ShowDialog() ?? false)
            {
                vm.Settings.StockLineMap.Add(dialog.CreateField());
                vm.Refresh();
            }
        }

        private void AddChargeLineButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = DataContext as DialogContext;
            if (vm == null)
            {
                return;
            }

            var dialog = new SysproNewFieldDialog() { Owner = this };
            if (dialog.ShowDialog() ?? false)
            {
                vm.Settings.StockLineFeeMap.Add(dialog.CreateField());
                vm.Refresh();
            }
        }

        private void AddCommentButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = DataContext as DialogContext;
            if (vm == null)
            {
                return;
            }

            var dialog = new SysproNewCommentFieldDialog() { Owner = this };
            if (dialog.ShowDialog() ?? false)
            {
                var field = dialog.CreateField();

                if (field != null)
                {
                    var existingFields = vm.Settings.CommentMap.AllFields();

                    if (existingFields.Count > 0)
                    {
                        field.Order = existingFields.Max(f => f.Order) + 1;
                    }
                    else
                    {
                        field.Order = 1;
                    }
                }

                vm.Settings.CommentMap.Add(field);
                vm.Refresh();
            }
        }

        private void OkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void RemoveOrderHeaderButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = DataContext as DialogContext;
            if (vm == null)
            {
                return;
            }

            vm.Settings?.OrderHeaderMap?.Remove(vm.SelectedHeaderField);
            vm.Refresh();
        }

        private void RemoveStockLineButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = DataContext as DialogContext;
            if (vm == null)
            {
                return;
            }

            vm.Settings?.StockLineMap?.Remove(vm.SelectedStockField);
            vm.Refresh();
        }

        private void RemoveChargeLineButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = DataContext as DialogContext;
            if (vm == null)
            {
                return;
            }

            vm.Settings?.StockLineFeeMap?.Remove(vm.SelectedChargeField);
            vm.Refresh();
        }

        private void RemoveCommentButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = DataContext as DialogContext;
            if (vm == null)
            {
                return;
            }

            vm.Settings?.CommentMap?.Remove(vm.SelectedCommentField);
            vm.Refresh();
        }

        private void UpCommentButton_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as DialogContext;
            if (vm?.Settings?.CommentMap == null || vm.SelectedCommentField == null)
            {
                return;
            }

            var comments = vm.Settings.CommentMap.AllFields();

            var movingComment = vm.SelectedCommentField;
            var previousComment = comments.FirstOrDefault(c => c.Order == movingComment.Order - 1);

            if (previousComment != null)
            {
                previousComment.Order++;
                movingComment.Order--;
            }

            vm.Refresh();
        }

        private void DownCommentButton_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as DialogContext;
            if (vm?.Settings?.CommentMap == null || vm.SelectedCommentField == null)
            {
                return;
            }

            var comments = vm.Settings.CommentMap.AllFields();

            var movingComment = vm.SelectedCommentField;
            var nextComment = comments.FirstOrDefault(c => c.Order == movingComment.Order + 1);

            if (nextComment != null)
            {
                nextComment.Order--;
                movingComment.Order++;
            }

            vm.Refresh();
        }

        private void ImportButtonOnClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as DialogContext;
            if (vm == null)
            {
                return;
            }

            var openFileDialog = new OpenFileDialog {Filter = "Settings (*.sysprosettings)|*.sysprosettings"};

            SysproInvoiceSettings settings = null;
            if (openFileDialog.ShowDialog() ?? false)
            {
                try
                {
                    settings = JsonConvert.DeserializeObject<SysproInvoiceSettings>(File.ReadAllText(openFileDialog.FileName));
                }
                catch (JsonReaderException)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("Unable to import settings.", "DWOS");
                }
            }

            if (settings != null)
            {
                vm.Load(settings);
                MessageBoxUtilities.ShowMessageBoxOK("Successfully imported settings.", "DWOS");
            }
        }

        private void ExportButtonOnClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as DialogContext;
            if (vm == null)
            {
                return;
            }

            var saveFileDialog = new SaveFileDialog {Filter = "Settings (*.sysprosettings)|*.sysprosettings"};

            if (saveFileDialog.ShowDialog() ?? false)
            {
                File.WriteAllText(saveFileDialog.FileName, JsonConvert.SerializeObject(vm.Settings, Formatting.Indented));
                MessageBoxUtilities.ShowMessageBoxOK("Successfully exported settings","DWOS");
            }
        }

        #endregion

        #region DialogContext

        private sealed class DialogContext : INotifyPropertyChanged
        {
            #region Fields

            public event PropertyChangedEventHandler PropertyChanged;
            private SysproInvoiceSettings.IField _selectedHeaderField;
            private SysproInvoiceSettings.IField _selectedStockField;
            private SysproInvoiceSettings.IField _selectedChargeField;
            private SysproInvoiceSettings.ICommentField _selectedCommentField;
            private readonly List<PriceUnitOfMeasure> _priceUnits;
            private readonly ObservableCollection<SysproInvoiceSettings.IField> _header;
            private readonly ObservableCollection<SysproInvoiceSettings.IField> _stock;
            private readonly ObservableCollection<SysproInvoiceSettings.IField> _charge;
            private readonly ObservableCollection<SysproInvoiceSettings.ICommentField> _comments;

            #endregion

            #region Properties

            public SysproInvoiceSettings Settings
            {
                get; private set;
            }

            public SysproInvoiceSettings.LineItemType LineItem
            {
                get { return Settings?.LineItem ?? SysproInvoiceSettings.LineItemType.Part; }
                set
                {
                    if (Settings == null || Settings.LineItem == value)
                    {
                        return;
                    }

                    Settings.LineItem = value;
                    OnPropertyChanged(nameof(LineItem));
                }
            }

            public IEnumerable<SysproInvoiceSettings.LineItemType> LineItemTypes { get; }

            public IEnumerable<PriceUnitOfMeasure> PriceUnits => _priceUnits;

            public ObservableCollection<SysproInvoiceSettings.IField> OrderHeader => _header;

            public ObservableCollection<SysproInvoiceSettings.IField> StockLine => _stock;

            public ObservableCollection<SysproInvoiceSettings.IField> MiscChargeLine => _charge;

            public ObservableCollection<SysproInvoiceSettings.ICommentField> Comment => _comments;

            public bool IncludeEmptyFreightLine
            {
                get { return Settings.IncludeEmptyFreightLine; }
                set
                {
                    if (Settings.IncludeEmptyFreightLine == value)
                    {
                        return;
                    }

                    Settings.IncludeEmptyFreightLine = value;
                    OnPropertyChanged(nameof(IncludeEmptyFreightLine));
                }
            }

            public bool GenerateSingleFile
            {
                get => Settings.GenerateSingleFile;
                set
                {
                    if (Settings.GenerateSingleFile == value)
                    {
                        return;
                    }

                    Settings.GenerateSingleFile = value;
                    OnPropertyChanged(nameof(GenerateSingleFile));
                }
            }

            public bool IncludeDiscountsInFees
            {
                get => Settings.IncludeDiscountsInFees;
                set
                {
                    if (Settings.IncludeDiscountsInFees == value)
                    {
                        return;
                    }

                    Settings.IncludeDiscountsInFees = value;
                    OnPropertyChanged(nameof(IncludeDiscountsInFees));
                }
            }

            public SysproInvoiceSettings.IField SelectedHeaderField
            {
                get { return _selectedHeaderField; }
                set
                {
                    if (_selectedHeaderField == value)
                    {
                        return;
                    }

                    _selectedHeaderField = value;
                    OnPropertyChanged(nameof(SelectedHeaderField));
                }
            }

            public SysproInvoiceSettings.IField SelectedStockField
            {
                get { return _selectedStockField; }
                set
                {
                    if (_selectedStockField == value)
                    {
                        return;
                    }

                    _selectedStockField = value;
                    OnPropertyChanged(nameof(SelectedStockField));
                }
            }

            public SysproInvoiceSettings.IField SelectedChargeField
            {
                get { return _selectedChargeField; }
                set
                {
                    if (_selectedChargeField == value)
                    {
                        return;
                    }

                    _selectedChargeField = value;
                    OnPropertyChanged(nameof(SelectedChargeField));
                }
            }

            public SysproInvoiceSettings.ICommentField SelectedCommentField
            {
                get { return _selectedCommentField; }
                set
                {
                    if (_selectedCommentField == value)
                    {
                        return;
                    }

                    _selectedCommentField = value;
                    OnPropertyChanged(nameof(SelectedCommentField));
                }
            }

            #endregion

            #region Methods

            public DialogContext()
            {
                _priceUnits = new List<PriceUnitOfMeasure>();
                _header = new ObservableCollection<SysproInvoiceSettings.IField>();
                _stock = new ObservableCollection<SysproInvoiceSettings.IField>();
                _charge = new ObservableCollection<SysproInvoiceSettings.IField>();
                _comments = new ObservableCollection<SysproInvoiceSettings.ICommentField>();

                LineItemTypes = new List<SysproInvoiceSettings.LineItemType>
                {
                    SysproInvoiceSettings.LineItemType.Part,
                    SysproInvoiceSettings.LineItemType.Process
                };
            }

            public void Load(SysproInvoiceSettings settings)
            {
                if (settings == null)
                {
                    throw new ArgumentNullException(nameof(settings));
                }

                Settings = settings.Copy();

                Refresh();

                foreach (var uom in PriceUnits)
                {
                    uom.PropertyChanged -= PriceUnitChanged;
                }

                _priceUnits.Clear();
                var eachUom = new PriceUnitOfMeasure("Each", Settings.PriceUnitEach);
                var lotUom = new PriceUnitOfMeasure("Lot", Settings.PriceUnitLot);
                eachUom.PropertyChanged += PriceUnitChanged;
                lotUom.PropertyChanged += PriceUnitChanged;
                _priceUnits.Add(eachUom);
                _priceUnits.Add(lotUom);

                SelectedHeaderField = null;
                SelectedStockField = null;
                SelectedChargeField = null;
                SelectedCommentField = null;

                OnPropertyChanged(nameof(LineItem));
            }

            public void Refresh()
            {
                _header.Clear();
                _stock.Clear();
                _charge.Clear();
                _comments.Clear();

                if (Settings == null)
                {
                    return;
                }

                foreach (var field in Settings.OrderHeaderMap?.AllFields() ?? Enumerable.Empty<SysproInvoiceSettings.IField>())
                {
                    _header.Add(field);
                }

                foreach (var field in Settings.StockLineMap?.AllFields() ?? Enumerable.Empty<SysproInvoiceSettings.IField>())
                {
                    _stock.Add(field);
                }

                foreach (var field in Settings.StockLineFeeMap?.AllFields() ?? Enumerable.Empty<SysproInvoiceSettings.IField>())
                {
                    _charge.Add(field);
                }

                var comments = Settings.CommentMap?.AllFields() ?? new List<SysproInvoiceSettings.ICommentField>();

                if (comments.All(i => i.Order < 0))
                {
                    // Set Order for all comments.
                    for (var i = 0; i < comments.Count; ++i)
                    {
                        comments[i].Order = i;
                    }
                }

                foreach (var comment in comments)
                {
                    _comments.Add(comment);
                }

                OnPropertyChanged(nameof(IncludeEmptyFreightLine));
                OnPropertyChanged(nameof(GenerateSingleFile));
            }

            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion

            #region Events

            private void PriceUnitChanged(object sender, PropertyChangedEventArgs e)
            {
                var uom = sender as PriceUnitOfMeasure;

                if (Settings == null || uom == null || e.PropertyName != nameof(PriceUnitOfMeasure.Syspro))
                {
                    return;
                }

                if (uom.Dwos == "Each")
                {
                    Settings.PriceUnitEach = uom.Syspro;
                }
                else if (uom.Dwos == "Lot")
                {
                    Settings.PriceUnitLot = uom.Syspro;
                }
            }

            #endregion
        }

        #endregion

        #region PriceUnitOfMeasure

        private sealed class PriceUnitOfMeasure : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private string _syspro;

            public string Syspro
            {
                get { return _syspro; }
                set
                {
                    if (_syspro == value)
                    {
                        return;
                    }

                    _syspro = value;
                    OnPropertyChanged(nameof(Syspro));
                }
            }

            public string Dwos { get; }

            public PriceUnitOfMeasure(string dwosValue, string sysproValue)
            {
                Dwos = dwosValue;
                _syspro = sysproValue;
            }

            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
