using System.Collections.Generic;
using DWOS.AutomatedWorkOrderTool.ViewModel;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    public class UnsavedDataService
    {
        #region  Fields

        private readonly HashSet<int> _unsavedOspFormats =
            new HashSet<int>();

        #endregion

        #region  Methods

        public void Clear(OspFormatViewModel vm)
        {
            if(vm == null)
            {
                return;
            }

            _unsavedOspFormats.Remove(vm.OspFormatId);
        }

        public bool HasUnsavedChanges(OspFormatViewModel vm)
        {
            return vm != null && _unsavedOspFormats.Contains(vm.OspFormatId);
        }

        public void SetUnsaved(OspFormatViewModel vm)
        {
            if(vm == null)
            {
                return;
            }

            _unsavedOspFormats.Add(vm.OspFormatId);
        }

        #endregion
    }
}