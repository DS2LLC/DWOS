using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DWOS.UI.Admin.ViewModels
{
    public class WorkDescriptionEditorViewModel : ViewModelBase, IDisposable
    {
        #region Fields

        public event EventHandler Accepted;

        private readonly OrdersDataSet.WorkDescriptionDataTable _dtWorkDescription =
            new OrdersDataSet.WorkDescriptionDataTable();

        private readonly Random _rnd = new Random();

        private WorkDescriptionItem _selectedItem;

        #endregion

        #region Properties

        public ObservableCollection<WorkDescriptionItem> Items { get; } =
            new ObservableCollection<WorkDescriptionItem>();

        public WorkDescriptionItem SelectedItem
        {
            get => _selectedItem;
            set => Set(nameof(SelectedItem), ref _selectedItem, value);
        }

        public ICommand Add { get; }

        public ICommand DeleteSelection { get; }

        public ICommand Save { get; }

        #endregion

        #region Methods

        public WorkDescriptionEditorViewModel()
        {
            Add = new RelayCommand(
                () =>
                {
                    try
                    {
                        var newRow = _dtWorkDescription.NewWorkDescriptionRow();
                        newRow.Description = $"Description #{_rnd.Next(1, 1000)}";
                        newRow.IsDefault = false;
                        _dtWorkDescription.AddWorkDescriptionRow(newRow);

                        Items.Add(new WorkDescriptionItem(newRow, 0));
                    }
                    catch (Exception exc)
                    {
                        LogManager.GetCurrentClassLogger()
                            .Error(exc, "Error adding work description.");
                    }
                });

            DeleteSelection = new RelayCommand(
                () =>
                {
                    try
                    {
                        var selection = _selectedItem;
                        if (selection == null || selection.UsageCount != 0)
                        {
                            return;
                        }

                        Items.Remove(selection);
                        selection.Row.Delete();
                    }
                    catch (Exception exc)
                    {
                        LogManager.GetCurrentClassLogger()
                            .Error(exc, "Error removing selected work description.");
                    }
                },
                () => _selectedItem != null && _selectedItem.UsageCount == 0);

            Save = new RelayCommand(DoSave, CanSave);
        }

        public void LoadData()
        {
            Items.Clear();

            using (var taWorkDescription = new WorkDescriptionTableAdapter())
            {
                taWorkDescription.Fill(_dtWorkDescription);

                foreach (var row in _dtWorkDescription)
                {
                    var usageCount = taWorkDescription.GetUsageCount(row.WorkDescriptionID) ?? 0;
                    Items.Add(new WorkDescriptionItem(row, usageCount));
                }
            }
        }

        private void DoSave()
        {
            try
            {
                using (var taWorkDescription = new WorkDescriptionTableAdapter())
                {
                    taWorkDescription.Update(_dtWorkDescription);
                    Accepted?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error accepting WorkDescriptionEditor.");
            }
        }

        private bool CanSave() =>
            Items.All(item => string.IsNullOrEmpty(item.ValidateAll()));

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _dtWorkDescription.Dispose();
        }

        #endregion

        #region WorkDescriptionItem

        public class WorkDescriptionItem : ViewModelBase
        {
            #region Properties

            public OrdersDataSet.WorkDescriptionRow Row { get; }

            public string Description
            {
                get => Row.Description;
                set
                {
                    if (!string.Equals(Row.Description, value, StringComparison.Ordinal))
                    {
                        Row.Description = value;
                        RaisePropertyChanged(nameof(Description));
                    }
                }
            }

            public bool IsDefault
            {
                get => Row.IsDefault;
                set
                {
                    if (Row.IsDefault != value)
                    {
                        Row.IsDefault = value;
                        RaisePropertyChanged(nameof(IsDefault));
                    }
                }
            }

            public int UsageCount { get; }

            #endregion

            #region Methods

            public WorkDescriptionItem(OrdersDataSet.WorkDescriptionRow row, int usageCount)
            {
                Row = row;
                UsageCount = usageCount;
            }

            public override string Validate(string propertyName)
            {
                if (propertyName == nameof(Description))
                {
                    if (string.IsNullOrEmpty(Row.Description))
                    {
                        return "Description is required.";
                    }

                    if (Row.Description.Length > 255)
                    {
                        return "Description must be shorter than 256 characters.";
                    }
                }

                return null;
            }

            public override string ValidateAll() => Validate(nameof(Description));

            #endregion
        }

        #endregion
    }
}
