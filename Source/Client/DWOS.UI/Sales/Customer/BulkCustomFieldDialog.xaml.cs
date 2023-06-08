using System;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using DWOS.UI.Utilities;
using System.Collections.Generic;
using DWOS.Data.Datasets;
using NLog;
using GalaSoft.MvvmLight.CommandWpf;

namespace DWOS.UI.Sales.Customer
{
    /// <summary>
    /// Interaction logic for BulkCustomFieldDialog.xaml
    /// </summary>
    public partial class BulkCustomFieldDialog : Window
    {
        #region Properties

        public DialogContext Data
        {
            get
            {
                return DataContext as DialogContext;
            }
        }

        public IEnumerable<CustomersDataset.CustomFieldRow> ExistingFields { get; }

        public CustomersDataset.ListsDataTable ListsTable { get; }

        #endregion

        #region Methods

        public BulkCustomFieldDialog(IEnumerable<CustomersDataset.CustomFieldRow> existingFields, CustomersDataset.ListsDataTable dtLists)
        {
            InitializeComponent();
            ExistingFields = existingFields;
            ListsTable = dtLists;
            Icon = Properties.Resources.wizard.ToWpfImage();
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var context = new DialogContext(ExistingFields, ListsTable);
            context.Accept += Context_Accept;
            context.PropertyChanged += ContextOnPropertyChanged;
            DataContext = context;
        }

        private void Context_Accept(object sender, EventArgs e)
        {
            DialogResult = true;
        }

        #endregion

        #region Events

        private void ContextOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            try
            {
                if (propertyChangedEventArgs.PropertyName == nameof(DialogContext.IsVisible))
                {
                    var context = DataContext as DialogContext;

                    if (context != null && !context.IsVisible)
                    {
                        context.IsRequired = false;
                    }
                }
                else if (propertyChangedEventArgs.PropertyName == nameof(DialogContext.IsRequired))
                {
                    var context = DataContext as DialogContext;

                    if (context != null && context.IsRequired)
                    {
                        context.IsVisible = true;
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing form value.");
            }
        }

        #endregion

        #region DialogContext

        public sealed class DialogContext : INotifyPropertyChanged, IDataErrorInfo
        {
            #region Fields

            /// <summary>
            /// Raised when the window should close successfully.
            /// </summary>
            public event EventHandler Accept;

            private string _displayName;
            private string _description;
            private string _tokenName;
            private string _defaultValue;
            private int? _listId;
            private bool _printOnTraveler;
            private bool _printOnCOC;
            private bool _isRequired;
            private bool _isProcessUnique;
            private bool _isVisible;

            #endregion

            #region Properties

            public string DisplayName
            {
                get
                {
                    return _displayName;
                }

                set
                {
                    if (_displayName != value)
                    {
                        _displayName = value;
                        OnPropertyChanged(nameof(DisplayName));
                    }
                }
            }

            public string Description
            {
                get
                {
                    return _description;
                }

                set
                {
                    if (_description != value)
                    {
                        _description = value;
                        OnPropertyChanged(nameof(Description));
                    }
                }
            }

            public string TokenName
            {
                get
                {
                    return _tokenName;
                }

                set
                {
                    if (_tokenName != value)
                    {
                        _tokenName = value;
                        OnPropertyChanged(nameof(TokenName));
                    }
                }
            }

            public string DefaultValue
            {
                get
                {
                    return _defaultValue;
                }
                set
                {
                    if (_defaultValue != value)
                    {
                        _defaultValue = value;
                        OnPropertyChanged(nameof(DefaultValue));
                    }
                }
            }

            public int? ListId
            {
                get { return _listId; }
                set
                {
                    if (_listId != value)
                    {
                        _listId = value;
                        OnPropertyChanged(nameof(ListId));
                    }
                }
            }

            public bool PrintOnTraveler
            {
                get
                {
                    return _printOnTraveler;
                }

                set
                {
                    if (_printOnTraveler != value)
                    {
                        _printOnTraveler = value;
                        OnPropertyChanged(nameof(PrintOnTraveler));
                    }
                }
            }

            public bool PrintOnCOC
            {
                get
                {
                    return _printOnCOC;
                }

                set
                {
                    if (_printOnCOC != value)
                    {
                        _printOnCOC = value;
                        OnPropertyChanged(nameof(PrintOnCOC));
                    }
                }
            }

            public bool IsRequired
            {
                get
                {
                    return _isRequired;
                }

                set
                {
                    if (_isRequired != value)
                    {
                        _isRequired = value;
                        OnPropertyChanged(nameof(IsRequired));
                    }
                }
            }

            public bool IsVisible
            {
                get { return _isVisible; }
                set
                {
                    if (_isVisible != value)
                    {
                        _isVisible = value;
                        OnPropertyChanged(nameof(IsVisible));
                    }
                }
            }

            public bool IsProcessUnique
            {
                get
                {
                    return _isProcessUnique;
                }
                set
                {
                    if (_isProcessUnique != value)
                    {
                        _isProcessUnique = value;
                        OnPropertyChanged(nameof(IsProcessUnique));
                    }
                }
            }

            public IList<string> FieldNames
            {
                get;
                private set;
            }

            public IEnumerable<CustomersDataset.CustomFieldRow> Fields
            {
                get;
                private set;
            }

            public CustomersDataset.ListsDataTable ListsTable { get; }

            public ICommand AcceptCommand
            {
                get;
                private set;
            }

            public ICommand ClearListCommand { get; }

            #endregion

            #region Methods

            public DialogContext(IEnumerable<CustomersDataset.CustomFieldRow> fields, CustomersDataset.ListsDataTable listsTable)
            {
                AcceptCommand = new AcceptDialogCommand(this);

                ClearListCommand = new RelayCommand(
                    () =>
                    {
                        ListId = null;
                    });

                Fields = fields;

                FieldNames = fields
                    .Where(field => field.IsValidState())
                    .Select(field => field.Name)
                    .Distinct()
                    .ToList();

                ListsTable = listsTable;
            }

            public void OnAccept()
            {
                var handler = Accept;
                handler?.Invoke(this, new EventArgs());
            }

            private void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                if (propertyName == nameof(DisplayName) && FieldNames.Contains(_displayName))
                {
                    var firstField = Fields.OrderBy(field => field.CustomerRow.Name)
                        .FirstOrDefault(i => i.Name == _displayName);

                    if (firstField != null)
                    {
                        UpdateProperties(firstField);
                    }
                }
            }

            private void UpdateProperties(CustomersDataset.CustomFieldRow firstField)
            {
                if (firstField == null)
                {
                    return;
                }

                DisplayName = firstField.Name;
                Description = firstField.IsDescriptionNull() ? null : firstField.Description;
                TokenName = firstField.IsTokenNameNull() ? null : firstField.TokenName;
                DefaultValue = firstField.IsDefaultValueNull() ? null : firstField.DefaultValue;
                ListId = firstField.IsListIDNull() ? (int?)null : firstField.ListID;
                PrintOnCOC = firstField.DisplayOnCOC;
                PrintOnTraveler = firstField.DisplayOnTraveler;
                IsRequired = firstField.Required;
                IsVisible = firstField.IsVisible;
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
                    return this[nameof(DisplayName)];
                }
            }

            public string this[string columnName]
            {
                get
                {
                    string errorMsg = string.Empty;

                    if (columnName == nameof(DisplayName))
                    {
                        if (string.IsNullOrEmpty(_displayName))
                        {
                            errorMsg = "Display Name is required.";
                        }
                        else if (_displayName.Length > 50)
                        {
                            errorMsg = "Display Name must be less than 50 characters long.";
                        }
                    }

                    return errorMsg;
                }
            }

            #endregion
        }

        #endregion

        #region AcceptCommand

        private sealed class AcceptDialogCommand : ICommand
        {
            #region Properties

            public DialogContext Context { get; private set; }

            #endregion

            #region Methods

            public AcceptDialogCommand(DialogContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context), "context cannot be null");
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
                return string.IsNullOrEmpty(Context.Error);
            }

            public void Execute(object parameter)
            {
                if (CanExecute(parameter))
                {
                    Context.OnAccept();
                }
            }

            #endregion
        }

        #endregion
    }
}
