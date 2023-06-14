using System;
using System.Linq;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.AutomatedWorkOrderTool.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows.Input;
using DWOS.AutomatedWorkOrderTool.Messages;
using GalaSoft.MvvmLight.CommandWpf;
using NLog;

namespace DWOS.AutomatedWorkOrderTool.ViewModel
{
    public class OspFormatEditorViewModel : ViewModelBase
    {
        #region Fields

        public event EventHandler SectionOrderChanged;
        public event EventHandler CodeMapOrderChanged;

        private bool _isLoading;
        private string _customerName;
        private string _manufacturerName;
        private string _manufacturerCode;
        private AwotDataSet _dsAwot;
        private OspFormatViewModel _currentFormat;
        private FormatSection _selectedSection;
        private CodeMap _selectedCodeMap;

        #endregion

        #region Properties

        public IUserManager UserManager { get; }

        public IDataManager DataManager { get; }

        public UnsavedDataService UnsavedData { get; }

        public string CustomerName
        {
            get => _customerName;
            private set => Set(nameof(CustomerName), ref _customerName, value);
        }

        public string ManufacturerName
        {
            get => _manufacturerName;
            private set => Set(nameof(ManufacturerName), ref _manufacturerName, value);
        }

        public string ManufacturerCode
        {
            get => _manufacturerCode;
            set
            {
                if (Set(nameof(ManufacturerCode), ref _manufacturerCode, value))
                {
                    // Update dependent property
                    RaisePropertyChanged(nameof(FormatExample));

                    // Update manufacturer code used by sections
                    foreach (var section in Sections)
                    {
                        section.ManufacturerCode = value;
                    }
                }
            }
        }

        public string FormatExample => GenerateExampleFormat();

        public ObservableCollection<FormatSection> Sections { get; } =
            new ObservableCollection<FormatSection>();

        public FormatSection SelectedSection
        {
            get => _selectedSection;
            set => Set(nameof(SelectedSection), ref _selectedSection, value);
        }

        public ObservableCollection<CodeMap> CodeMaps { get; } =
            new ObservableCollection<CodeMap>();

        public CodeMap SelectedCodeMap
        {
            get => _selectedCodeMap;
            set => Set(nameof(SelectedCodeMap), ref _selectedCodeMap, value);
        }

        public ICommand Save { get; }

        public ICommand Reset { get; }

        public ICommand ShowAddSectionDialog { get; }

        public ICommand DeleteSectionDialog { get; }

        public ICommand MoveSectionUp { get; }

        public ICommand MoveSectionDown { get; }

        public ICommand ShowAddCodeMapDialog { get; }

        public ICommand DeleteCodeMapDialog { get; }

        public bool CanEdit => UserManager.CurrentUser != null &&
            UserManager.CurrentUser.IsInRole(DwosUser.MASTER_LIST_ROLE);

        #endregion

        #region Methods

        public OspFormatEditorViewModel(IMessenger messenger, IDataManager dataManager, IUserManager userManager, UnsavedDataService unsavedData)
            : base(messenger)
        {
            DataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            UnsavedData = unsavedData ?? throw new ArgumentNullException(nameof(unsavedData));

            Save = new RelayCommand(DoSave, CanSave);
            Reset = new RelayCommand(DoReset);

            MessengerInstance.Register<AddOspFormatSectionMessage>(this, HandleNewOspFormatSection);
            MessengerInstance.Register<AddOspProcessMessage>(this, HandleNewCodeMap);
            MessengerInstance.Register<AddOspPartMarkMessage>(this, HandleNewCodeMap);

            ShowAddSectionDialog = new RelayCommand(() =>
            {
                MessengerInstance.Send(new ShowAddSectionDialogMessage(_currentFormat, Sections.Select(s => s.ToModel()).ToList()));
            });

            DeleteSectionDialog = new RelayCommand(
                () =>
                {
                    MessengerInstance.Send(new ConfirmActionMessage("Delete Section",
                        "Would you like to delete the selected section and all of its code maps?",
                        DeleteSelectedSection));
                },
                IsSectionSelected);

            MoveSectionUp = new RelayCommand(DoMoveSectionUp, IsSectionSelected);
            MoveSectionDown = new RelayCommand(DoMoveSectionDown, IsSectionSelected);

            ShowAddCodeMapDialog = new RelayCommand(() =>
            {
                MessengerInstance.Send(new ShowAddCodeMapDialogMessage(this, Sections.Select(s => s.ToModel()).ToList()));
            });

            DeleteCodeMapDialog = new RelayCommand(() =>
            {
                MessengerInstance.Send(new ConfirmActionMessage("Delete Code Map",
                    "Would you like to delete the selected code map?",
                    DeleteSelectedCodeMap));
            },
            IsCodeMapSelected);

            // Register 'unsaved data' handlers
            PropertyChanged += OnDataChanged;
            CodeMaps.CollectionChanged += OnDataChanged;
            Sections.CollectionChanged += OnDataChanged;
        }

        private void DoMoveSectionUp()
        {
            try
            {
                if (_selectedSection == null)
                {
                    return;
                }

                var previousSection = Sections.OrderBy(o => o.SectionOrder)
                    .LastOrDefault(o => o.SectionOrder < _selectedSection.SectionOrder);

                if (previousSection != null)
                {
                    var selectedOrder = _selectedSection.SectionOrder;
                    var previousOrder = previousSection.SectionOrder;
                    previousSection.SectionOrder = selectedOrder;
                    _selectedSection.SectionOrder = previousOrder;
                }

                // Trigger UI events
                SectionOrderChanged?.Invoke(this, EventArgs.Empty);
                CodeMapOrderChanged?.Invoke(this, EventArgs.Empty);

                // Do not trigger update of the OSP Format example - same number of sections
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error moving section down"));
            }
        }

        private void DoMoveSectionDown()
        {
            try
            {
                if (_selectedSection == null)
                {
                    return;
                }

                var nextSection = Sections.OrderBy(o => o.SectionOrder)
                    .FirstOrDefault(o => o.SectionOrder > _selectedSection.SectionOrder);

                if (nextSection != null)
                {
                    var selectedOrder = _selectedSection.SectionOrder;
                    var previousPriority = nextSection.SectionOrder;
                    nextSection.SectionOrder = selectedOrder;
                    _selectedSection.SectionOrder = previousPriority;
                }

                // Trigger UI events
                SectionOrderChanged?.Invoke(this, EventArgs.Empty);
                CodeMapOrderChanged?.Invoke(this, EventArgs.Empty);

                // Do not trigger update of the OSP Format example - same number of sections
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error moving section down"));
            }
        }

        public void LoadData(OspFormatViewModel ospFormat)
        {
            _isLoading = true;
            _currentFormat = ospFormat;
            _dsAwot?.Dispose();
            _dsAwot = null;
            Sections.Clear();
            CodeMaps.Clear();

            if (ospFormat == null)
            {
                _isLoading = false;
                return;
            }

            _dsAwot = new AwotDataSet();
            DataManager.LoadInitialData(_dsAwot, ospFormat.CustomerId, ospFormat.OspFormatId);

            var customer = _dsAwot.Customer.FirstOrDefault();
            CustomerName = customer?.Name;

            ManufacturerName = ospFormat.Manufacturer;

            var ospFormatRow = _dsAwot.OSPFormat.FirstOrDefault();
            if (ospFormatRow != null)
            {
                ManufacturerCode = ospFormatRow.Code;

                foreach (var sectionRow in ospFormatRow.GetOSPFormatSectionRows().OrderBy(f => f.SectionOrder))
                {
                    var sectionViewModel = new FormatSection(sectionRow, _manufacturerCode);
                    Sections.Add(sectionViewModel);

                    // Add Code Maps
                    // Assumption: Code maps from the database cannot have the same code and section.
                    foreach (var codeMapRow in sectionRow.GetOSPFormatSectionProcessRows())
                    {
                        CodeMaps.Add(new CodeMap(sectionViewModel, codeMapRow));
                    }

                    foreach (var codeMapRow in sectionRow.GetOSPFormatSectionPartMarkRows())
                    {
                        CodeMaps.Add(new CodeMap(sectionViewModel, codeMapRow));
                    }
                }

                // Refresh FormatExample so that it includes all sections
                RaisePropertyChanged(nameof(FormatExample));
            }
            else
            {
                // Should not happen, but handle it just in case
                ManufacturerCode = null;
            }

            _isLoading = false;
        }

        public override void Cleanup()
        {
            _dsAwot?.Dispose();
            base.Cleanup();
        }

        private string GenerateExampleFormat()
        {
            var parts = new List<string>
            {
                ManufacturerCode
            };

            foreach (var section in Sections.OrderBy(s => s.SectionOrder))
            {
                parts.Add(ManufacturerCode + section.SectionOrder);
            }

            return string.Join("-", parts);
        }

        private void DoSave()
        {
            try
            {
                if (!CanSave())
                {
                    return;
                }

                var formatRow = _dsAwot.OSPFormat.FirstOrDefault();

                if (formatRow != null)
                {
                    formatRow.Code = _manufacturerCode;
                }

                DataManager.SaveOspData(_dsAwot);
                UnsavedData.Clear(_currentFormat);
                MessengerInstance.Send(new SuccessMessage(SuccessMessage.SuccessType.SaveOspFormat));
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error resetting manufacturer data."));
            }
        }

        private void DoReset()
        {
            try
            {
                if (_currentFormat != null)
                {
                    LoadData(_currentFormat);
                    UnsavedData.Clear(_currentFormat);
                }
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error resetting manufacturer data."));
            }
        }

        private bool CanSave() => !string.IsNullOrEmpty(_manufacturerCode) &&
            _manufacturerCode.Length < 3 &&
            CodeMaps.All(c => c.IsValid);

        private void HandleNewOspFormatSection(AddOspFormatSectionMessage msg)
        {
            try
            {
                var formatSection = msg?.NewSection;
                if (formatSection == null || formatSection.OspFormatId != _currentFormat.OspFormatId)
                {
                    return;
                }

                var sectionRow = _dsAwot.OSPFormatSection.NewOSPFormatSectionRow();
                sectionRow.DepartmentID = formatSection.Department;
                sectionRow.OSPFormatID = formatSection.OspFormatId;
                sectionRow.Role = formatSection.Role.ToString();
                sectionRow.SectionOrder = formatSection.SectionOrder;
                _dsAwot.OSPFormatSection.AddOSPFormatSectionRow(sectionRow);

                Sections.Add(new FormatSection(sectionRow, _manufacturerCode));

                // Update format example
                RaisePropertyChanged(nameof(FormatExample));
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error adding new OSP format section."));
                throw;
            }
        }

        private void HandleNewCodeMap(AddOspProcessMessage msg)
        {
            try
            {
                if (msg?.NewProcess == null)
                {
                    return;
                }

                var matchingSection = Sections.FirstOrDefault(s => s.SectionRow.OSPFormatSectionID == msg.NewProcess.OspFormatSectionId);

                if (matchingSection == null)
                {
                    return;
                }

                var processRow = _dsAwot.OSPFormatSectionProcess.NewOSPFormatSectionProcessRow();
                processRow.OSPFormatSectionID = msg.NewProcess.OspFormatSectionId;
                processRow.Code = msg.NewProcess.Code;
                processRow.ProcessID = msg.NewProcess.ProcessId;
                processRow.ProcessAliasID = msg.NewProcess.ProcessAliasId;
                _dsAwot.OSPFormatSectionProcess.AddOSPFormatSectionProcessRow(processRow);
                CodeMaps.Add(new CodeMap(matchingSection, processRow));
                CodeMapOrderChanged?.Invoke(this, EventArgs.Empty);
                Validate(CodeMaps);
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error adding new process code map."));
            }
        }

        private void HandleNewCodeMap(AddOspPartMarkMessage msg)
        {
            try
            {
                if (msg?.NewPartMark == null)
                {
                    return;
                }

                var matchingSection = Sections.FirstOrDefault(s => s.SectionRow.OSPFormatSectionID == msg.NewPartMark.OspFormatSectionId);

                if (matchingSection == null)
                {
                    return;
                }

                var partMarkRow = _dsAwot.OSPFormatSectionPartMark.NewOSPFormatSectionPartMarkRow();
                partMarkRow.OSPFormatSectionID = msg.NewPartMark.OspFormatSectionId;
                partMarkRow.Code = msg.NewPartMark.Code;
                partMarkRow.ProcessSpec = msg.NewPartMark.ProcessSpec;
                partMarkRow.Def1 = msg.NewPartMark.Def1;
                partMarkRow.Def2 = msg.NewPartMark.Def2;
                partMarkRow.Def3 = msg.NewPartMark.Def3;
                partMarkRow.Def4 = msg.NewPartMark.Def4;

                _dsAwot.OSPFormatSectionPartMark.AddOSPFormatSectionPartMarkRow(partMarkRow);

                CodeMaps.Add(new CodeMap(matchingSection, partMarkRow));
                CodeMapOrderChanged?.Invoke(this, EventArgs.Empty);
                Validate(CodeMaps);
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error adding new part mark code map."));
            }
        }

        private static void Validate(IEnumerable<CodeMap> codeMaps)
        {
            var sections = codeMaps.GroupBy(c => c.ParentSection.SectionOrder);
            foreach (var section in sections)
            {
                foreach (var codeMapSection in section.GroupBy(map => map.Code))
                {
                    var isValid = codeMapSection.Count() == 1;
                    foreach (var codeMap in codeMapSection)
                    {
                        codeMap.IsValid = isValid;
                    }
                }
            }
        }

        private void DeleteSelectedSection()
        {
            try
            {
                if (!IsSectionSelected())
                {
                    return;
                }

                // Remove section.
                var section = _selectedSection;
                Sections.Remove(section);
                section.SectionRow.Delete();

                // Remove code maps for section
                foreach (var codeMap in CodeMaps.Where(m => m.ParentSection == section).ToList())
                {
                    CodeMaps.Remove(codeMap);
                    codeMap.Row.Delete();
                }

                // Correct order for remaining sections.
                var sectionOrder = 1;
                foreach (var existingSection in Sections.OrderBy(s => s.SectionOrder).ToList())
                {
                    existingSection.SectionOrder = sectionOrder;
                    sectionOrder++;
                }

                // Update format example
                RaisePropertyChanged(nameof(FormatExample));
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error deleting selected OSP Format section."));
            }
        }

        private void DeleteSelectedCodeMap()
        {
            try
            {
                var selection = _selectedCodeMap;

                if (selection == null)
                {
                    return;
                }

                CodeMaps.Remove(selection);
                selection.Row.Delete();

                Validate(CodeMaps);
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error deleting selected OSP Format code map."));
            }
        }

        private bool IsSectionSelected() => _selectedSection != null;

        private bool IsCodeMapSelected() => _selectedCodeMap != null;

        #endregion

        #region Events

        private void OnDataChanged(object sender, EventArgs args)
        {
            if (_isLoading)
            {
                return;
            }

            UnsavedData.SetUnsaved(_currentFormat);
        }

        #endregion

        #region FormatSection

        public class FormatSection : ViewModelBase
        {
            #region Fields

            private string _manufacturerCode;

            #endregion

            #region Properties

            public AwotDataSet.OSPFormatSectionRow SectionRow { get; }

            public string SectionName =>
                $"{_manufacturerCode}{SectionOrder}";

            public int SectionOrder
            {
                get => SectionRow.SectionOrder;
                set
                {
                    if (SectionRow.SectionOrder != value)
                    {
                        SectionRow.SectionOrder = value;
                        RaisePropertyChanged(nameof(SectionOrder));
                        RaisePropertyChanged(nameof(SectionName));
                    }
                }
            }

            public string Role => SectionRow.Role;

            public string Department => SectionRow.DepartmentID;

            public string ManufacturerCode
            {
                get => _manufacturerCode;
                set
                {
                    if (Set(nameof(ManufacturerCode), ref _manufacturerCode, value))
                    {
                        RaisePropertyChanged(nameof(SectionName));
                    }
                }
            }

            #endregion

            #region Methods

            public FormatSection(AwotDataSet.OSPFormatSectionRow sectionRow, string manufacturerCode)
            {
                SectionRow = sectionRow;
                ManufacturerCode = manufacturerCode;
            }

            public OspFormatSectionViewModel ToModel()
            {
                return new OspFormatSectionViewModel
                {
                    OspFormatSectionId = SectionRow.OSPFormatSectionID,
                    Department = SectionRow.DepartmentID,
                    OspFormatId = SectionRow.OSPFormatID,
                    SectionOrder = SectionRow.SectionOrder,
                    Role = (RoleType)Enum.Parse(typeof(RoleType), SectionRow.Role)
                };
            }

            #endregion
        }

        #endregion

        #region CodeMap

        public class CodeMap : ViewModelBase
        {

            #region Fields

            private string _section;
            private bool _isValid = true;

            #endregion

            #region Properties

            public FormatSection ParentSection { get; }

            public DataRow Row { get; }

            public string Section
            {
                get => _section;
                set => Set(nameof(Section), ref _section, value);
            }

            public string Code { get; }

            public string ProcessName { get; }

            public string AliasName { get; }

            public bool IsValid
            {
                get => _isValid;
                set => Set(nameof(IsValid), ref _isValid, value);
            }

            #endregion

            #region Methods

            public CodeMap(FormatSection parentSection, AwotDataSet.OSPFormatSectionProcessRow processRow)
            {
                ParentSection = parentSection ?? throw new ArgumentNullException(nameof(parentSection));
                Row = processRow ?? throw new ArgumentNullException(nameof(processRow));

                Code = processRow.Code;
                ProcessName = processRow.ProcessRow?.Name ?? "N/A";
                AliasName = processRow.ProcessAliasRow?.Name ?? "N/A";

                Section = ParentSection.SectionName;
                ParentSection.PropertyChanged += ParentSectionOnPropertyChanged;
            }

            public CodeMap(FormatSection parentSection, AwotDataSet.OSPFormatSectionPartMarkRow partMarkRow)
            {
                ParentSection = parentSection ?? throw new ArgumentNullException(nameof(parentSection));
                Row = partMarkRow ?? throw new ArgumentNullException(nameof(partMarkRow));

                Code = partMarkRow.Code;
                ProcessName = "Part Marking";
                AliasName = partMarkRow.IsProcessSpecNull() ? "N/A" : partMarkRow.ProcessSpec;

                Section = ParentSection.SectionName;
                ParentSection.PropertyChanged += ParentSectionOnPropertyChanged;
            }

            #endregion

            #region Events

            private void ParentSectionOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
            {
                try
                {
                    if (propertyChangedEventArgs.PropertyName != nameof(FormatSection.SectionName))
                    {
                        return;
                    }

                    Section = ParentSection.SectionName;
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error updating section.");
                }
            }

            #endregion
        }

        #endregion
    }
}
