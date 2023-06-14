using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.AutomatedWorkOrderTool.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.AutomatedWorkOrderTool.ViewModel
{
    public class AddOspFormatSectionViewModel : ViewModelBase
    {
        #region Fields

        private RoleType _selectedRole;
        private string _selectedDept;
        private int _ospFormatId;
        private int _nextOrderNumber;

        #endregion

        #region Properties

        public IDepartmentManager DeptManager { get; }

        public List<RoleType> Roles { get; } =
            Enum.GetValues(typeof(RoleType)).OfType<RoleType>().ToList();

        public RoleType SelectedRole
        {
            get => _selectedRole;
            set
            {
                if (Set(nameof(SelectedRole), ref _selectedRole, value))
                {
                    RaisePropertyChanged(nameof(CanSelectDepartment));
                    if (_selectedRole == RoleType.PartMark)
                    {
                        SelectedDepartment = DeptManager.PartMarkDepartment;
                    }
                }
            }
        }

        public bool CanSelectDepartment =>
            _selectedRole != RoleType.PartMark;

        public ObservableCollection<string> Departments { get; } =
            new ObservableCollection<string>();

        public string SelectedDepartment
        {
            get => _selectedDept;
            set => Set(nameof(SelectedDepartment), ref _selectedDept, value);
        }

        #endregion

        #region Methods

        public AddOspFormatSectionViewModel(IMessenger messenger, IDepartmentManager deptManager)
            : base(messenger)
        {
            DeptManager = deptManager ?? throw new ArgumentNullException(nameof(deptManager));
        }

        public void LoadData(int ospFormatId, List<OspFormatSectionViewModel> existingSections)
        {
            if (existingSections == null)
            {
                throw new ArgumentNullException(nameof(existingSections));
            }

            Departments.Clear();
            foreach (var dept in DeptManager.AllDepartments.OrderBy(d => d))
            {
                Departments.Add(dept);
            }

            SelectedDepartment = Departments.FirstOrDefault();

            _ospFormatId = ospFormatId;

            if (existingSections.Count > 0)
            {
                _nextOrderNumber = 1 + existingSections.Max(f => f.SectionOrder);
            }
            else
            {
                _nextOrderNumber = 1;
            }
        }

        public OspFormatSectionViewModel CreateFormatSection()
        {
            return new OspFormatSectionViewModel
            {
                OspFormatId = _ospFormatId,
                Role = _selectedRole,
                Department = _selectedDept,
                SectionOrder = _nextOrderNumber
            };
        }

        #endregion
    }
}
