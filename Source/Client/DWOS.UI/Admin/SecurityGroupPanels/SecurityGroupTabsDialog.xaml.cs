using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using System.IO;
using System.Linq;
using System.Windows.Data;
using DWOS.Shared.Utilities;
using System.Windows.Forms;
using NLog;
using GalaSoft.MvvmLight.CommandWpf;

namespace DWOS.UI.Admin.SecurityGroupPanels
{
    /// <summary>
    /// Interaction logic for SecurityGroupTabsDialog.xaml
    /// </summary>
    public partial class SecurityGroupTabsDialog
    {
        #region Properties

        private DialogDataContext ViewModel => DataContext as DialogDataContext;
        private readonly GridSettingsPersistence<XamDataGridSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<XamDataGridSettings>("SecurityGroupTabsDialog", new XamDataGridSettings());

        #endregion

        #region Methods

        public SecurityGroupTabsDialog()
        {
            InitializeComponent();
            DataContext = new DialogDataContext();
            Icon = Properties.Resources.Paper32.ToWpfImage();
        }

        public void Load(SecurityDataSet dsSecurity, SecurityDataSet.SecurityGroupRow group)
        {
            var vm = ViewModel;
            vm?.LoadDataSet(dsSecurity);
            vm?.LoadGroup(group);
        }

        #endregion

        #region Events

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;

            if (vm != null)
            {
                vm.Exit += ViewModelExit;
                vm.ExportTab += VmOnExportTab;
                vm.TabChanged += VmOnTabChanged;
            }

            // Load settings
            _gridSettingsPersistence.LoadSettings().ApplyTo(TabGrid);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;

            if (vm != null)
            {
                vm.Exit -= ViewModelExit;
                vm.ExportTab -= VmOnExportTab;
                vm.TabChanged -= VmOnTabChanged;
            }

            // Save settings
            var settings = new XamDataGridSettings();
            settings.RetrieveSettingsFrom(TabGrid);
            _gridSettingsPersistence.SaveSettings(settings);
        }

        private void ViewModelExit(object sender, EventArgs e)
        {
            DialogResult = true;
        }

        private void VmOnExportTab(object sender, EventArgsTemplate<DwosTabData> args)
        {
            var tab = args?.Item;

            if (tab != null)
            {
                DwosTabDataUtilities.DoExport(tab);
            }
        }

        private void VmOnTabChanged(object sender, EventArgs e)
        {
            try
            {
                TabGrid.Records.RefreshSort();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error refreshing sort.");
            }
        }

        #endregion

        #region DialogDataContext

        private class DialogDataContext : INotifyPropertyChanged
        {
            #region Fields

            private Tab _selectedTab;
            private string _groupName;
            public event EventHandler TabChanged;
            public event EventHandler Exit;
            public event EventHandler<EventArgsTemplate<DwosTabData>> ExportTab;

            #endregion

            #region Properties

            public SecurityDataSet DataSet { get; private set; }

            public SecurityDataSet.SecurityGroupRow Group { get; private set; }

            public ObservableCollection<Tab> Tabs { get; } = new ObservableCollection<Tab>();

            public Tab SelectedTab
            {
                get { return _selectedTab; }
                set
                {
                    if (_selectedTab != value)
                    {
                        _selectedTab = value;
                        OnPropertyChanged(nameof(SelectedTab));
                    }
                }
            }

            public string GroupName
            {
                get { return _groupName; }
                set
                {
                    if (_groupName != value)
                    {
                        _groupName = value;
                        OnPropertyChanged(nameof(GroupName));
                    }
                }
            }

            public ICommand Add { get; }

            public ICommand Remove { get; }

            public ICommand Rename { get; }

            public ICommand SaveToFile { get; }

            public ICommand MoveUp { get; }

            public ICommand MoveDown { get; }

            public ICommand Accept { get; }

            #endregion

            #region Methods

            public DialogDataContext()
            {
                Add = new AddCommand(this);
                Remove = new RemoveCommand(this);
                Rename = new RenameCommand(this);
                SaveToFile = new RelayCommand(
                    () =>
                    {
                        if (_selectedTab != null)
                        {
                            ExportTab?.Invoke(this, new EventArgsTemplate<DwosTabData>(_selectedTab.AsTabData()));
                        }
                    },
                    () => _selectedTab != null);

                MoveUp = new RelayCommand(
                    () =>
                    {
                        DoMoveUp(_selectedTab);
                    },
                    
                    () => _selectedTab != null);

                MoveDown = new RelayCommand(
                    () =>
                    {
                        DoMoveDown(_selectedTab);
                    },
                    
                    () => _selectedTab != null);

                Accept = new RelayCommand(DoAccept);
            }

            private void DoMoveUp(Tab movingTab)
            {
                if (movingTab == null)
                {
                    return;
                }

                var previousTab = Tabs.FirstOrDefault(c => c.TabOrder == movingTab.TabOrder - 1);

                if (previousTab != null)
                {
                    previousTab.TabOrder++;
                    movingTab.TabOrder--;
                    TabChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            private void DoMoveDown(Tab movingTab)
            {
                if (movingTab == null)
                {
                    return;
                }

                var nextTab = Tabs.FirstOrDefault(c => c.TabOrder == movingTab.TabOrder + 1);

                if (nextTab != null)
                {
                    nextTab.TabOrder--;
                    movingTab.TabOrder++;
                    TabChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            private void DoAccept()
            {
                var nonDeletedTabRows = Tabs
                    .Select(t => t.OriginalRow)
                    .Where(t => t != null)
                    .ToList();

                // Remove
                foreach (var tabRow in Group.GetSecurityGroupTabRows())
                {
                    if (nonDeletedTabRows.Contains(tabRow))
                    {
                        continue;
                    }

                    tabRow.Delete();
                }

                // Add & edit
                foreach (var tab in Tabs)
                {
                    if (tab.OriginalRow == null)
                    {
                        // Create new row
                        var newRow = DataSet.SecurityGroupTab.NewSecurityGroupTabRow();
                        newRow.SecurityGroupRow = Group;
                        newRow.Name = tab.Name;
                        newRow.DataType = tab.DataType;
                        newRow.TabKey = tab.Key;
                        newRow.Layout = tab.Layout;
                        newRow.TabOrder = tab.TabOrder;

                        if (tab.Version.HasValue)
                        {
                            newRow.Version = tab.Version.Value;
                        }

                        DataSet.SecurityGroupTab.AddSecurityGroupTabRow(newRow);
                    }
                    else
                    {
                        // Edit existing
                        if (tab.OriginalRow.Name != tab.Name)
                        {
                            tab.OriginalRow.Name = tab.Name;
                        }

                        if (tab.OriginalRow.DataType != tab.DataType)
                        {
                            tab.OriginalRow.DataType = tab.DataType;
                        }

                        if (tab.OriginalRow.TabKey != tab.Key)
                        {
                            tab.OriginalRow.TabKey = tab.Key;
                        }

                        if (tab.OriginalRow.TabOrder != tab.TabOrder)
                        {
                            tab.OriginalRow.TabOrder = tab.TabOrder;
                        }

                        var originalLayout = tab.OriginalRow.IsLayoutNull()
                            ? string.Empty
                            : tab.OriginalRow.Layout;

                        var newLayout = tab.Layout ?? string.Empty;

                        if (originalLayout != newLayout)
                        {
                            tab.OriginalRow.Layout = tab.Layout;
                        }

                        var originalVersion = tab.OriginalRow.IsVersionNull()
                            ? (int?)null
                            : tab.OriginalRow.Version;

                        var newVersion = tab.Version;

                        if (originalVersion != newVersion)
                        {
                            if (newVersion.HasValue)
                            {
                                tab.OriginalRow.Version = newVersion.Value;
                            }
                            else
                            {
                                tab.OriginalRow.SetVersionNull();
                            }
                        }
                    }
                }

                Exit?.Invoke(this, EventArgs.Empty);
            }

            public void LoadDataSet(SecurityDataSet dsSecurity)
            {
                if (dsSecurity == null)
                {
                    throw new ArgumentNullException(nameof(dsSecurity));
                }

                DataSet = dsSecurity;
            }

            public void LoadGroup(SecurityDataSet.SecurityGroupRow group)
            {
                if (group == null)
                {
                    throw new ArgumentNullException(nameof(group));
                }

                Group = group;

                Tabs.Clear();

                foreach (var tabRow in group.GetSecurityGroupTabRows())
                {
                    if (tabRow.IsValidState())
                    {
                        Tabs.Add(new Tab(tabRow));
                    }
                }

                GroupName = group.Name;
                TabChanged?.Invoke(this, EventArgs.Empty);
            }

            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion

            #region ICommand Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            public void RemoveTab(Tab tab)
            {
                var removedTabOrder = tab.TabOrder;
                SelectedTab = null;
                Tabs.Remove(tab);

                foreach (var tabUnderRemoved in Tabs.Where(t => t.TabOrder > removedTabOrder))
                {
                    tabUnderRemoved.TabOrder -= 1;
                }

                TabChanged?.Invoke(this, EventArgs.Empty);
            }

            public void AddTab(DwosTabData tab)
            {
                if (tab == null)
                {
                    return;
                }

                var tabOrder = Tabs.Count > 0
                    ? 1 + Tabs.Max(t => t.TabOrder)
                    : 1;

                Tabs.Add(new Tab
                {
                    Name = tab.Name,
                    DataType = tab.DataType,
                    Key = tab.Key,
                    Layout = tab.Layout,
                    Version = tab.Version,
                    TabOrder = tabOrder
                });

                TabChanged?.Invoke(this, EventArgs.Empty);
            }

            public void RenameSelectedTab(string tabName)
            {
                if (_selectedTab == null)
                {
                    return;
                }

                _selectedTab.Name = tabName;
                TabChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Tab

        public class Tab : INotifyPropertyChanged
        {
            private string _name;
            private string _dataType;
            private string _key;
            private string _layout;
            private int _tabOrder;
            private int? _version;

            public SecurityDataSet.SecurityGroupTabRow OriginalRow { get; }

            public string Name
            {
                get { return _name; }
                set
                {
                    if (_name != value)
                    {
                        _name = value;
                        OnPropertyChanged(nameof(Name));
                    }
                }
            }

            public string DataType
            {
                get { return _dataType; }
                set
                {
                    if (_dataType != value)
                    {
                        _dataType = value;
                        OnPropertyChanged(nameof(DataType));
                    }
                }
            }

            public string Key
            {
                get { return _key; }
                set
                {
                    if (_key != value)
                    {
                        _key = value;
                        OnPropertyChanged(nameof(Key));
                    }
                }
            }

            public string Layout
            {
                get { return _layout; }
                set
                {
                    if (_layout != value)
                    {
                        _layout = value;
                        OnPropertyChanged(nameof(Layout));
                        OnPropertyChanged(nameof(HasLayout));
                    }
                }
            }

            public bool HasLayout =>
                !string.IsNullOrEmpty(_layout);

            public int TabOrder
            {
                get { return _tabOrder; }
                set
                {
                    if (_tabOrder != value)
                    {
                        _tabOrder = value;
                        OnPropertyChanged(nameof(TabOrder));
                    }
                }
            }

            public int? Version
            {
                get
                {
                    return _version;
                }
                set
                {
                    if (_version != value)
                    {
                        _version = value;
                        OnPropertyChanged(nameof(Version));
                    }
                }
            }

            public Tab(SecurityDataSet.SecurityGroupTabRow originalRow)
            {
                if (originalRow == null)
                {
                    throw new ArgumentNullException(nameof(originalRow));
                }

                OriginalRow = originalRow;
                Name = originalRow.Name;
                DataType = originalRow.DataType;
                Key = originalRow.TabKey;
                Layout = originalRow.IsLayoutNull() ? null : originalRow.Layout;
                TabOrder = originalRow.TabOrder;
                Version = originalRow.IsVersionNull() ? (int?)null : originalRow.Version;
            }

            public Tab()
            {
            }

            public DwosTabData AsTabData()
            {
                return new DwosTabData
                {
                    Name = Name,
                    DataType = DataType,
                    Key = Key,
                    Layout = Layout,
                    Version = Version
                };
            }

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #region Events

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

        #region AddCommand

        private class AddCommand : ICommand
        {
            #region Fields

            private DialogDataContext Context { get; }

            #endregion

            #region Methods

            public AddCommand(DialogDataContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                Context = context;
            }

            #endregion

            #region ICommand Members

            public void Execute(object parameter)
            {
                const string messageBoxHeader = "Security Group Tabs";

                var fileName = string.Empty;

                // Show dialog
                using (var fileDialog = new OpenFileDialog())
                {
                    var initialDirectory = Path.Combine(FileSystem.UserDocumentPath(), "Tabs");
                    if (Directory.Exists(initialDirectory))
                    {
                        Directory.CreateDirectory(initialDirectory);
                    }

                    fileDialog.AddExtension = true;
                    fileDialog.Title = @"Import DWOS Tab";
                    fileDialog.Filter = @"DWOS Tab (*.dwostab)|*.dwostab";
                    fileDialog.InitialDirectory = initialDirectory;

                    if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        fileName = fileDialog.FileName;
                    }
                }

                if (!string.IsNullOrEmpty(fileName))
                {
                    var tab = DwosTabDataUtilities.Import(fileName);
                    if (tab == null)
                    {
                        // Invalid file
                        MessageBoxUtilities.ShowMessageBoxWarn("The file that you selected is not a valid tab file.", messageBoxHeader);
                    }
                    else if (!tab.IsValid)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn("Invalid tab.", messageBoxHeader);
                    }
                    else if (Context.Tabs.Any(t => t.Key == tab.Key))
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn("Duplicate tab.", messageBoxHeader);
                    }
                    else
                    {
                        Context.AddTab(tab);
                    }
                }
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            #endregion
        }

        #endregion

        #region RemoveCommand

        private class RemoveCommand : ICommand
        {
            #region Fields

            private DialogDataContext Context { get; }

            #endregion

            #region Methods

            public RemoveCommand(DialogDataContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                Context = context;
            }

            #endregion

            #region ICommand Members

            public void Execute(object parameter)
            {
                if (Context.SelectedTab == null)
                {
                    return;
                }

                Context.RemoveTab(Context.SelectedTab);
            }

            public bool CanExecute(object parameter)
            {
                return Context.SelectedTab != null;
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            #endregion
        }

        #endregion

        #region RenameCommand

        private class RenameCommand : ICommand
        {
            #region Fields

            private DialogDataContext Context { get; }

            #endregion

            #region Methods

            public RenameCommand(DialogDataContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                Context = context;
            }

            #endregion

            #region ICommand Members

            public void Execute(object parameter)
            {
                if (Context.SelectedTab == null)
                {
                    return;
                }

                // Show rename dialog
                using (var form = new TextBoxForm())
                {
                    form.Text = @"Edit Name";
                    form.FormLabel.Text = @"Name:";
                    form.FormTextBox.Text = Context.SelectedTab.Name;

                    var userAcceptsDialog = form.ShowDialog(DWOSApp.MainForm) == System.Windows.Forms.DialogResult.OK &&
                                            !string.IsNullOrWhiteSpace(form.FormTextBox.Text);

                    if (userAcceptsDialog)
                    {
                        Context.RenameSelectedTab(form.FormTextBox.Text.Trim());
                    }
                }
            }

            public bool CanExecute(object parameter)
            {
                return Context.SelectedTab != null;
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            #endregion
        }

        #endregion
    }
}