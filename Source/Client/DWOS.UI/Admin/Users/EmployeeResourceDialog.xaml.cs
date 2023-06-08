using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using Infragistics.Windows.DataPresenter;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Infragistics.Windows.DataPresenter.Events;

namespace DWOS.UI.Admin.Users
{
    /// <summary>
    /// Dialog for managing employee resource (salary) information.
    /// </summary>
    public partial class EmployeeResourceDialog : Window
    {
        #region Fields

        private readonly GridSettingsPersistence<XamDataGridSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<XamDataGridSettings>("EmployeeResourceDialog", new XamDataGridSettings());

        #endregion

        #region Properties

        private UserSalaryDataContext ViewModel
        {
            get
            {
                return DataContext as UserSalaryDataContext;
            }
        }

        #endregion

        #region Methods

        public EmployeeResourceDialog()
        {
            InitializeComponent();
            Icon = Properties.Resources.Paper32.ToWpfImage();

        }

        public void LoadData(SecurityDataSet dataSet, SecurityDataSet.UsersRow user)
        {
            if (dataSet == null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            DataContext = new UserSalaryDataContext(dataSet, user);
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.Exit += ViewModel_Exit;
                ViewModel.SalaryEntries.ListChanged += SalaryEntries_ListChanged;
            }

            // Load settings
            _gridSettingsPersistence.LoadSettings().ApplyTo(salaryDataGrid);
        }

        private void SalaryDataGrid_OnFieldLayoutInitialized(object sender, FieldLayoutInitializedEventArgs e)
        {
            var fieldLayout = salaryDataGrid.FieldLayouts.First();
            var currencyMask = "{currency:6." + ApplicationSettings.Current.PriceDecimalPlaces + "}";

            var salaryField = fieldLayout.Fields[nameof(UserSalaryInfo.Salary)] as CurrencyField;

            if (salaryField != null)
            {
                salaryField.Mask = currencyMask;
            }

            var burdenField = fieldLayout.Fields[nameof(UserSalaryInfo.Burden)] as CurrencyField;

            if (burdenField != null)
            {
                burdenField.Mask = currencyMask;
            }
        }

        private void SalaryEntries_ListChanged(object sender, ListChangedEventArgs e)
        {
            salaryDataGrid.Records.RefreshSort();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.Exit -= ViewModel_Exit;
                ViewModel.SalaryEntries.ListChanged -= SalaryEntries_ListChanged;
            }

            // Save settings
            var settings = new XamDataGridSettings();
            settings.RetrieveSettingsFrom(salaryDataGrid);
            _gridSettingsPersistence.SaveSettings(settings);
        }

        private void ViewModel_Exit(object sender, EventArgs e)
        {
            DialogResult = true;
        }

        #endregion

        #region UserSalaryDataContext

        private sealed class UserSalaryDataContext : INotifyPropertyChanged, IDataErrorInfo
        {
            #region Fields

            public event EventHandler Exit;

            private UserSalaryInfo _selectedSalaryEntry;

            #endregion

            #region Properties

            public SecurityDataSet DataSet
            {
                get;
                private set;
            }

            public SecurityDataSet.UsersRow User
            {
                get;
                private set;
            }

            public ICommand ConfirmCommand
            {
                get;
                private set;
            }

            public ICommand AddEntryCommand
            {
                get;
                private set;
            }

            public ICommand RemoveEntryCommand
            {
                get;
                private set;
            }

            public string UserName
            {
                get
                {
                    return User.Name;
                }
            }

            public BindingList<UserSalaryInfo> SalaryEntries
            {
                get;
                private set;
            }

            public UserSalaryInfo SelectedSalaryEntry
            {
                get
                {
                    return _selectedSalaryEntry;
                }
                set
                {
                    if (_selectedSalaryEntry != value)
                    {
                        _selectedSalaryEntry = value;
                    }
                }
            }

            #endregion

            #region Methods

            public UserSalaryDataContext(SecurityDataSet dataSet, SecurityDataSet.UsersRow user)
            {
                if (dataSet == null)
                {
                    throw new ArgumentNullException(nameof(dataSet));
                }

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                DataSet = dataSet;
                User = user;

                ConfirmCommand = new ConfirmCommand(this);
                AddEntryCommand = new AddEntryCommand(this);
                RemoveEntryCommand = new RemoveEntryCommand(this);

                var entries = new List<UserSalaryInfo>();

                foreach (var salaryRow in user.GetUserSalaryRows())
                {
                    var entryForRow = new UserSalaryInfo()
                    {
                        Burden = salaryRow.Burden,
                        EffectiveDate = salaryRow.EffectiveDate.Date,
                        Salary = salaryRow.Salary
                    };


                    entries.Add(entryForRow);
                }

                SalaryEntries = new BindingList<UserSalaryInfo>(entries);
            }

            public void Confirm()
            {
                if (!string.IsNullOrEmpty(Error))
                {
                    return;
                }

                var effectiveDates = SalaryEntries.Select(e => e.EffectiveDate);

                var groupedRows = User.GetUserSalaryRows().GroupBy(row => row.EffectiveDate);

                foreach (var group in groupedRows)
                {
                    // Delete removed
                    var originalRow = group.First();
                    if (!effectiveDates.Contains(originalRow.EffectiveDate))
                    {
                        originalRow.Delete();
                    }

                    // Delete duplicate
                    foreach (var duplicateRow in group.Skip(1))
                    {
                        duplicateRow.Delete();
                    }
                }

                // Sync entries
                foreach (var entry in SalaryEntries)
                {
                    var originalRowMatch = User.GetUserSalaryRows()
                        .FirstOrDefault(row => row.EffectiveDate == entry.EffectiveDate);

                    if (originalRowMatch == null)
                    {
                        var newSalaryRow = DataSet.UserSalary.NewUserSalaryRow();

                        newSalaryRow.UsersRow = User;
                        newSalaryRow.EffectiveDate = entry.EffectiveDate.Date;
                        newSalaryRow.Salary = entry.Salary;
                        newSalaryRow.Burden = entry.Burden;
                        DataSet.UserSalary.AddUserSalaryRow(newSalaryRow);
                    }
                    else
                    {
                        originalRowMatch.Salary = entry.Salary;
                        originalRowMatch.Burden = entry.Burden;
                    }
                }

                Exit?.Invoke(this, EventArgs.Empty);
            }

            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            #region IDataErrorInfo Members

            public string Error
            {
                get
                {
                    return this[nameof(SalaryEntries)];
                }
            }

            public string this[string columnName]
            {
                get
                {
                    if (columnName == nameof(SalaryEntries))
                    {
                        // Mark duplicate entries
                        var entriesGroupByDate = SalaryEntries.GroupBy(e => e.EffectiveDate);
                        foreach (var group in entriesGroupByDate)
                        {
                            if (group.Count() > 1)
                            {
                                foreach (var entry in group)
                                {
                                    entry.HasDateError = true;
                                }
                            }
                            else
                            {
                                group.First().HasDateError = false;
                            }
                        }

                        if (!SalaryEntries.All(e => string.IsNullOrEmpty(e.Error)))
                        {
                            return "One or more entries are not valid.";
                        }
                    }

                    return string.Empty;
                }
            }

            #endregion
        }

        #endregion

        #region UserSalaryInfo

        private sealed class UserSalaryInfo : INotifyPropertyChanged, IDataErrorInfo
        {
            #region Fields

            private const decimal MAX_SALARY = 999999.99999M;

            private bool _hasDateError;
            private DateTime _effectiveDate;
            private decimal _salary;
            private decimal _burden;

            #endregion

            #region Properties

            public DateTime EffectiveDate
            {
                get
                {
                    return _effectiveDate;
                }
                set
                {
                    if (_effectiveDate != value)
                    {
                        _effectiveDate = value;
                        OnPropertyChanged(nameof(EffectiveDate));
                    }
                }
            }

            public bool HasDateError
            {
                get
                {
                    return _hasDateError;
                }
                set
                {
                    if (_hasDateError != value)
                    {
                        _hasDateError = value;
                        OnPropertyChanged(nameof(HasDateError));

                        // Trigger validation
                        OnPropertyChanged(nameof(EffectiveDate));
                    }
                }
            }

            public decimal Salary
            {
                get
                {
                    return _salary;
                }
                set
                {
                    if (_salary != value)
                    {
                        _salary = value;
                        OnPropertyChanged(nameof(Salary));
                    }
                }
            }

            public decimal Burden
            {
                get
                {
                    return _burden;
                }
                set
                {
                    if (_burden != value)
                    {
                        _burden = value;
                        OnPropertyChanged(nameof(Burden));
                    }
                }
            }

            #endregion

            #region Methods

            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            public string Error
            {
                get
                {
                    var errorValues = new List<string>()
                        {
                            this[nameof(Salary)],
                            this[nameof(Burden)],
                            this[nameof(EffectiveDate)]
                        };

                    if (errorValues.Any(error => error.Length > 0))
                    {
                        return "There is an error with this entry.";
                    }

                    return string.Empty;
                }
            }

            public string this[string columnName]
            {
                get
                {
                    if (columnName == nameof(Salary))
                    {
                        if (_salary < 0M)
                        {
                            return "Salary cannot be negative.";
                        }
                        else if (_salary > MAX_SALARY)
                        {
                            return string.Format("Salary must be below {0:C5}", MAX_SALARY);
                        }
                    }
                    else if (columnName == nameof(Burden))
                    {
                        if (_burden < 0M)
                        {
                            return "Burden cannot be negative.";
                        }
                        else if (_burden > MAX_SALARY)
                        {
                            return string.Format("Burden must be below {0:C5}", MAX_SALARY);
                        }
                    }
                    else if (columnName == nameof(EffectiveDate) && _hasDateError)
                    {
                        return "There is an entry with the same effective date.";
                    }

                    return string.Empty;
                }
            }
        }

        #endregion

        #region ConfirmCommand

        private sealed class ConfirmCommand : ICommand
        {
            #region Properties

            public UserSalaryDataContext Context
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public ConfirmCommand(UserSalaryDataContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                Context = context;
            }

            #endregion

            #region ICommand Members

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

            public bool CanExecute(object parameter)
            {
                try
                {
                    return string.IsNullOrEmpty(Context.Error);
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error checking CanExecute for {0}", nameof(ConfirmCommand));
                    return false;
                }
            }

            public void Execute(object parameter)
            {
                try
                {
                    if (!CanExecute(parameter))
                    {
                        return;
                    }

                    Context.Confirm();
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error running {0}", nameof(ConfirmCommand));
                }
            }

            #endregion
        }

        #endregion

        #region AddEntryCommand

        private sealed class AddEntryCommand : ICommand
        {
            #region Properties

            public UserSalaryDataContext Context
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public AddEntryCommand(UserSalaryDataContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                Context = context;
            }

            #endregion

            #region ICommand Members

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

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                try
                {
                    if (!CanExecute(parameter))
                    {
                        return;
                    }

                    DateTime today = DateTime.Today;

                    var mostRecentEntry = Context.SalaryEntries
                        .OrderByDescending(e => e.EffectiveDate)
                        .FirstOrDefault();

                    DateTime effectiveDate;
                    if (mostRecentEntry == null || mostRecentEntry.EffectiveDate < today)
                    {
                        effectiveDate = today;
                    }
                    else
                    {
                        effectiveDate = mostRecentEntry.EffectiveDate.AddDays(1);
                    }

                    Context.SalaryEntries.Add(new UserSalaryInfo()
                    {
                        EffectiveDate = effectiveDate,
                        Salary = mostRecentEntry?.Salary ?? 0M,
                        Burden = mostRecentEntry?.Burden ?? 0M,
                    });
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error running {0}", nameof(AddEntryCommand));
                }
            }

            #endregion
        }

        #endregion

        #region RemoveEntryCommand

        private sealed class RemoveEntryCommand : ICommand
        {
            #region Properties

            public UserSalaryDataContext Context
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public RemoveEntryCommand(UserSalaryDataContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                Context = context;
            }

            #endregion

            #region ICommand Members

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

            public bool CanExecute(object parameter)
            {
                try
                {
                    return Context.SelectedSalaryEntry != null;
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error checking CanExecute for {0}", nameof(RemoveEntryCommand));
                    return false;
                }
            }

            public void Execute(object parameter)
            {
                try
                {
                    var selection = Context.SelectedSalaryEntry;

                    if (!CanExecute(parameter) || selection == null)
                    {
                        return;
                    }

                    Context.SelectedSalaryEntry = null;
                    Context.SalaryEntries.Remove(selection);
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error running {0}", nameof(RemoveEntryCommand));
                }
            }

            #endregion
        }

        #endregion


    }
}
