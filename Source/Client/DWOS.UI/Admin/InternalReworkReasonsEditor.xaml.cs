using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DWOS.Data.Datasets;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Utilities;
using Infragistics.Windows.DataPresenter;
using NLog;

namespace DWOS.UI.Admin
{
    /// <summary>
    /// Interaction logic for ReworkReasonsEditor.xaml
    /// </summary>
    public partial class InternalReworkReasonsEditor
    {
        #region Fields

        private readonly GridSettingsPersistence<XamDataGridSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<XamDataGridSettings>("InternalReworkReasons", new XamDataGridSettings());

        #endregion

        #region Properties

        private EditorDataContext ViewModel => DataContext as EditorDataContext;

        #endregion

        #region Methods

        public InternalReworkReasonsEditor()
        {
            InitializeComponent();
            DataContext = new EditorDataContext();
        }

        public void LoadReworkReasons(ReworkCategoryType type)
        {
            Title = $"{type} Reason Editor";
            ViewModel?.Load(type);
        }

        #endregion

        #region Events

        private void InternalReworkReasonsEditor_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;

            if (vm == null)
            {
                return;
            }

            vm.Completed += ViewModelOnCompleted;
            vm.ReasonAdded += ViewModelOnReasonAdded;

            // Load settings
            _gridSettingsPersistence.LoadSettings().ApplyTo(ReasonGrid);
        }

        private void InternalReworkReasonsEditor_OnUnloaded(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;

            if (vm == null)
            {
                return;
            }

            vm.Completed -= ViewModelOnCompleted;
            vm.ReasonAdded -= ViewModelOnReasonAdded;

            // Save settings
            var settings = new XamDataGridSettings();
            settings.RetrieveSettingsFrom(ReasonGrid);
            _gridSettingsPersistence.SaveSettings(settings);
        }

        private void ViewModelOnCompleted(object sender, EventArgs eventArgs)
        {
            Close();
        }

        private void ViewModelOnReasonAdded(object sender, EventArgsTemplate<Reason> eventArgsTemplate)
        {
            var reason = eventArgsTemplate?.Item;
            if (reason == null)
            {
                return;
            }

            var lastRecord = ReasonGrid.Records.OfType<DataRecord>().LastOrDefault();

            if (lastRecord != null)
            {
                ReasonGrid.ActiveCell = lastRecord.Cells[0];
                ReasonGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
            }
        }

        #endregion

        #region EditorDataContext

        private class EditorDataContext : INotifyPropertyChanged
        {
            #region Fields

            public event EventHandler Completed;
            public event EventHandler<EventArgsTemplate<Reason>> ReasonAdded;
            private readonly Random _rnd;
            private ListsDataSet.d_ReworkReasonDataTable _dtReworkReason;
            private Reason _selectedReason;
            private ReworkCategoryType _currentReworkCategory;

            #endregion

            #region Properties

            public ICommand AddReason { get; }

            public ICommand DeleteReason { get; }

            public ICommand Save { get; }

            public Reason SelectedReason
            {
                get { return _selectedReason; }
                set
                {
                    if (_selectedReason != value)
                    {
                        _selectedReason = value;
                        OnPropertyChanged(nameof(SelectedReason));
                    }
                }
            }

            public ObservableCollection<Reason> Reasons { get; } = new ObservableCollection<Reason>();

            #endregion

            #region Methods

            public EditorDataContext()
            {
                _rnd = new Random();
                AddReason = new AddReasonCommand(this);
                DeleteReason = new DeleteReasonCommand(this);
                Save = new SaveCommand(this);
            }

            public void Load(ReworkCategoryType type)
            {
                Reasons.Clear();

                using (var ta = new Data.Datasets.ListsDataSetTableAdapters.d_ReworkReasonTableAdapter())
                {
                    _dtReworkReason = new ListsDataSet.d_ReworkReasonDataTable();
                    ta.FillByCategory(_dtReworkReason, type.ToString());

                    foreach (var row in _dtReworkReason)
                    {
                        Reasons.Add(new Reason(row));
                    }
                }

                _currentReworkCategory = type;
            }

            private void OnPropertyChanged(string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public void DoAdd()
            {
                var newRow = _dtReworkReason.Newd_ReworkReasonRow();
                newRow.ReworkCategory = _currentReworkCategory.ToString();
                newRow.Name = $"New Rework Reason {_rnd.Next(1000, 9999)}";
                _dtReworkReason.Addd_ReworkReasonRow(newRow);
                var newReason = new Reason(newRow);
                Reasons.Add(newReason);
                ReasonAdded?.Invoke(this, new EventArgsTemplate<Reason>(newReason));
            }

            public void DoDelete(Reason reason)
            {
                if (reason == null)
                {
                    return;
                }

                using (var taReworkReason = new Data.Datasets.OrdersDataSetTableAdapters.d_ReworkReasonTableAdapter())
                {
                    var usageCount = taReworkReason.GetUsageCount(reason.ReworkReasonID) ?? 0;
                    if (usageCount == 0)
                    {
                        if (_selectedReason == reason)
                        {
                            SelectedReason = null;
                        }

                        Reasons.Remove(reason);
                        reason.Row.Delete();
                    }
                    else
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn("Unable to delete rework reasons that are in use.", "Unable to Delete");
                    }
                }
            }

            public void DoSave()
            {
                try
                {
                    using (var ta = new Data.Datasets.ListsDataSetTableAdapters.d_ReworkReasonTableAdapter())
                    {
                        ta.Update(_dtReworkReason);
                    }
                    Completed?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error saving rework reasons.", exc);
                }
            }
            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

        #region Reason

        private class Reason : IDataErrorInfo
        {
            private string _name;

            public ListsDataSet.d_ReworkReasonRow Row { get; }

            public string Name
            {
                get { return _name; }
                set
                {
                    _name = value?.Replace("'", "*");

                    if (!string.IsNullOrEmpty(_name))
                    {
                        Row.Name = _name;
                    }
                }
            }

            public bool ShowOnDocuments
            {
                get { return Row.ShowOnDocuments; }
                set
                {
                    Row.ShowOnDocuments = value;
                }
            }

            public int ReworkReasonID => Row.ReworkReasonID;

            public bool IsValid => string.IsNullOrEmpty(this[nameof(Name)]);

            public Reason(ListsDataSet.d_ReworkReasonRow row)
            {
                if (row == null)
                {
                    throw new ArgumentNullException(nameof(row));
                }

                Row = row;
                _name = row.Name;
            }

            #region IDataErrorInfo Members

            public string this[string columnName]
            {
                get
                {
                    if (columnName == nameof(Name) && string.IsNullOrEmpty(_name))
                    {
                        return "Name is required.";
                    }

                    return null;
                }
            }

            public string Error
            {
                get
                {
                    if (string.IsNullOrEmpty(_name))
                    {
                        return "Name is required.";
                    }

                    return null;
                }
            }

            #endregion
        }

        #endregion

        #region AddReasonCommand

        private class AddReasonCommand : ICommand
        {
            public EditorDataContext Context { get; }

            public AddReasonCommand(EditorDataContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                Context = context;
            }

            #region ICommand Members

            public void Execute(object parameter)
            {
                try
                {
                    Context.DoAdd();
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error adding rework reason.");

                }
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged
            {
                add
                {
                    CommandManager.RequerySuggested += value;
                }
                remove
                {
                    CommandManager.RequerySuggested -= value;
                }
            }

            #endregion
        }

        #endregion

        #region DeleteReasonCommand

        private class DeleteReasonCommand : ICommand
        {
            public EditorDataContext Context { get; }

            public DeleteReasonCommand(EditorDataContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                Context = context;
            }

            #region ICommand Members

            public void Execute(object parameter)
            {
                try
                {
                    Context.DoDelete(Context.SelectedReason);
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error adding rework reason.");

                }
            }

            public bool CanExecute(object parameter)
            {
                return Context.SelectedReason != null;
            }

            public event EventHandler CanExecuteChanged
            {
                add
                {
                    CommandManager.RequerySuggested += value;
                }
                remove
                {
                    CommandManager.RequerySuggested -= value;
                }
            }

            #endregion
        }

        #endregion

        #region SaveCommand

        private class SaveCommand : ICommand
        {
            public EditorDataContext Context { get; }

            public SaveCommand(EditorDataContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                Context = context;
            }

            #region ICommand Members

            public void Execute(object parameter)
            {
                Context.DoSave();
            }

            public bool CanExecute(object parameter)
            {
                return Context.Reasons.All(r => r.IsValid);
            }

            public event EventHandler CanExecuteChanged
            {
                add
                {
                    CommandManager.RequerySuggested += value;
                }
                remove
                {
                    CommandManager.RequerySuggested -= value;
                }
            }

            #endregion
        }

        #endregion
    }
}
