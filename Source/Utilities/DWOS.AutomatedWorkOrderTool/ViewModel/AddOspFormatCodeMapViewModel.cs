using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DWOS.AutomatedWorkOrderTool.Messages;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.AutomatedWorkOrderTool.Services;
using GalaSoft.MvvmLight;
using DWOS.Data.Utilities;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.AutomatedWorkOrderTool.ViewModel
{
    public class AddOspFormatCodeMapViewModel : ViewModelBase
    {
        #region Fields

        private OspFormatEditorViewModel _parentViewModel;
        private OspFormatSectionViewModel _selectedSection;
        private string _ospCode;
        private string _processSpec;
        private readonly List<bool> _isDefEditable = new List<bool> { false, false, false, false };
        private readonly List<string> _def = new List<string> { string.Empty, string.Empty, string.Empty, string.Empty };
        private ProcessItem _selectedProcess;
        private ProcessAliasItem _selectedProcessAlias;

        #endregion

        #region Properties

        public IDataManager DataManager { get; }

        public string CustomerName => _parentViewModel?.CustomerName;

        public string ManufacturerName => _parentViewModel?.ManufacturerName;

        public string OspCode
        {
            get => _ospCode;
            set
            {
                if (Set(nameof(OspCode), ref _ospCode, value))
                {
                    RaisePropertyChanged(nameof(IsValid));
                }
            }
        }

        public ObservableCollection<OspFormatSectionViewModel> Sections { get; } =
            new ObservableCollection<OspFormatSectionViewModel>();

        public OspFormatSectionViewModel SelectedSection
        {
            get => _selectedSection;
            set
            {
                if (Set(nameof(SelectedSection), ref _selectedSection, value))
                {
                    // Dependent properties
                    RaisePropertyChanged(nameof(ShowProcessFields));
                    RaisePropertyChanged(nameof(ShowPartMarkFields));
                    LoadProcesses(_selectedSection?.Department);
                }
            }
        }

        public bool ShowProcessFields => _selectedSection?.Role == RoleType.Process;

        public ObservableCollection<ProcessItem> Processes { get; } =
            new ObservableCollection<ProcessItem>();

        public ProcessItem SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                if (Set(nameof(SelectedProcess), ref _selectedProcess, value))
                {
                    SelectedProcessAlias = value?.Aliases.FirstOrDefault();
                    RaisePropertyChanged(nameof(IsValid));
                }
            }
        }

        public ProcessAliasItem SelectedProcessAlias
        {
            get => _selectedProcessAlias;
            set
            {
                if (Set(nameof(SelectedProcessAlias), ref _selectedProcessAlias, value))
                {
                    RaisePropertyChanged(nameof(IsValid));
                }
            }
        }

        public bool ShowPartMarkFields => _selectedSection?.Role == RoleType.PartMark;

        public string ProcessSpec
        {
            get => _processSpec;
            set => Set(nameof(ProcessSpec), ref _processSpec, value);
        }

        public string Def1
        {
            get => _def[0];
            set
            {
                if (_def[0] != value)
                {
                    _def[0] = value;
                    RaisePropertyChanged(nameof(Def1));
                }
            }
        }

        public bool IsDef1Editable
        {
            get => _isDefEditable[0];
            set
            {
                if (_isDefEditable[0] != value)
                {
                    _isDefEditable[0] = value;
                    RaisePropertyChanged(nameof(IsDef1Editable));

                    if (!value)
                    {
                        Def1 = string.Empty;
                    }
                }
            }
        }

        public string Def2
        {
            get => _def[1];
            set
            {
                if (_def[1] != value)
                {
                    _def[1] = value;
                    RaisePropertyChanged(nameof(Def2));
                }
            }
        }

        public bool IsDef2Editable
        {
            get => _isDefEditable[1];
            set
            {
                if (_isDefEditable[1] != value)
                {
                    _isDefEditable[1] = value;
                    RaisePropertyChanged(nameof(IsDef2Editable));

                    if (!value)
                    {
                        Def2 = string.Empty;
                    }
                }
            }
        }

        public string Def3
        {
            get => _def[2];
            set
            {
                if (_def[2] != value)
                {
                    _def[2] = value;
                    RaisePropertyChanged(nameof(Def3));
                }
            }
        }

        public bool IsDef3Editable
        {
            get => _isDefEditable[2];
            set
            {
                if (_isDefEditable[2] != value)
                {
                    _isDefEditable[2] = value;
                    RaisePropertyChanged(nameof(IsDef3Editable));

                    if (!value)
                    {
                        Def3 = string.Empty;
                    }
                }
            }
        }

        public string Def4
        {
            get => _def[3];
            set
            {
                if (_def[3] != value)
                {
                    _def[3] = value;
                    RaisePropertyChanged(nameof(Def4));
                }
            }
        }

        public bool IsDef4Editable
        {
            get => _isDefEditable[3];
            set
            {
                if (_isDefEditable[3] != value)
                {
                    _isDefEditable[3] = value;
                    RaisePropertyChanged(nameof(IsDef4Editable));

                    if (!value)
                    {
                        Def4 = string.Empty;
                    }
                }
            }
        }

        public List<MarkerCode> MarkerCodes { get; } = new List<MarkerCode>
        {
            MarkerCode.From(Interperter.enumInterperterCommands.DATE, Interperter.ParseCommand(Interperter.enumInterperterCommands.DATE)),
            MarkerCode.From(Interperter.enumInterperterCommands.DATE2, Interperter.ParseCommand(Interperter.enumInterperterCommands.DATE2)),
            MarkerCode.From(Interperter.enumInterperterCommands.TIME, Interperter.ParseCommand(Interperter.enumInterperterCommands.TIME)),
            MarkerCode.From(Interperter.enumInterperterCommands.CUSTOMERWO, "123456"),
            MarkerCode.From(Interperter.enumInterperterCommands.PARTNUMBER, "00146879-2"),
            MarkerCode.From(Interperter.enumInterperterCommands.PARTQTY, "5"),
            MarkerCode.From(Interperter.enumInterperterCommands.PARTREV, "1"),
            MarkerCode.From(Interperter.enumInterperterCommands.ASSEMBLY, "ABC-1234"),
        };

        public bool IsValid => 
            (IsProcessValid() || IsPartMarkValid());

        #endregion

        #region Methods

        public AddOspFormatCodeMapViewModel(IMessenger messenger, IDataManager dataManager)
            : base(messenger)
        {
            DataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
        }

        public void Load(OspFormatEditorViewModel parentViewModel, List<OspFormatSectionViewModel> sections)
        {
            var currentSectionOrder = _selectedSection?.SectionOrder;
            _parentViewModel = parentViewModel;

            // Reset general fields
            SelectedSection = null;
            Sections.Clear();
            OspCode = string.Empty;
            RaisePropertyChanged(nameof(CustomerName));
            RaisePropertyChanged(nameof(ManufacturerName));

            // Reset part marking fields
            ProcessSpec = string.Empty;
            IsDef1Editable = false;
            IsDef2Editable = false;
            IsDef3Editable = false;
            IsDef4Editable = false;

            if (parentViewModel == null || sections == null)
            {
                return;
            }

            // Sections
            foreach (var section in sections.OrderBy(s => s.SectionOrder))
            {
                Sections.Add(section);
            }

            SelectedSection = Sections.FirstOrDefault(s => s.SectionOrder == currentSectionOrder) ??
                Sections.FirstOrDefault();
        }

        private void LoadProcesses(string department)
        {
            try
            {
                SelectedProcess = null;
                SelectedProcessAlias = null;
                Processes.Clear();

                using (var dataSet = new AwotDataSet())
                {
                    DataManager.LoadProcesses(dataSet, department);

                    foreach (var processRow in dataSet.Process.OrderBy(p => p.Name))
                    {
                        Processes.Add(ProcessItem.From(processRow));
                    }
                }

                SelectedProcess = Processes.FirstOrDefault();
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error loading processes."));
            }
        }

        private bool IsProcessValid() => _selectedSection != null && _selectedSection.Role == RoleType.Process &&
                                         !string.IsNullOrEmpty(_ospCode) &&
                                         _ospCode.Length < 11 &&
                                         _selectedProcess != null && _selectedProcessAlias != null;

        private bool IsPartMarkValid() => _selectedSection != null && _selectedSection.Role == RoleType.PartMark &&
                                          !string.IsNullOrEmpty(_ospCode) &&
                                          _ospCode.Length < 11;

        #endregion

        #region MarkerCode

        public class MarkerCode
        {
            #region Properties

            public string Code { get; set; }

            public string Sample { get; set; }

            #endregion

            #region Methods

            public static MarkerCode From(Interperter.enumInterperterCommands cmd, string sample)
            {
                return new MarkerCode
                {
                    Code = $"<{cmd}>",
                    Sample = sample
                };
            }

            #endregion
        }

        #endregion

        #region ProcessItem

        public class ProcessItem
        {
            public int ProcessId { get; private set; }

            public string Name { get; private set; }

            public List<ProcessAliasItem> Aliases { get; private set; }

            public static ProcessItem From(AwotDataSet.ProcessRow processRow)
            {
                if (processRow == null)
                {
                    return null;
                }

                return new ProcessItem
                {
                    ProcessId = processRow.ProcessID,
                    Name = processRow.Name,
                    Aliases = processRow.GetProcessAliasRows().Select(ProcessAliasItem.From).ToList()
                };
            }
        }

        #endregion

        #region ProcessAliasItem

        public class ProcessAliasItem
        {
            public int ProcessAliasId { get; private set; }

            public string Name { get; private set; }

            public static ProcessAliasItem From(AwotDataSet.ProcessAliasRow processAlias)
            {
                if (processAlias == null)
                {
                    return null;
                }

                return new ProcessAliasItem
                {
                    ProcessAliasId = processAlias.ProcessAliasID,
                    Name = processAlias.Name
                };
            }
        }
        #endregion
    }
}
