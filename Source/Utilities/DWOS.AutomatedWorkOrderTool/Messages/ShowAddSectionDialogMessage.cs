using System;
using System.Collections.Generic;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.AutomatedWorkOrderTool.Messages
{
    public class ShowAddSectionDialogMessage : MessageBase
    {
        #region Properties

        public OspFormatViewModel CurrentFormat { get; }

        public List<OspFormatSectionViewModel> CurrentSections { get; set; }

        #endregion

        #region Methods

        public ShowAddSectionDialogMessage(OspFormatViewModel currentFormat, List<OspFormatSectionViewModel> currentSections)
        {
            CurrentSections = currentSections ?? throw new ArgumentNullException(nameof(currentSections));
            CurrentFormat = currentFormat ?? throw new ArgumentNullException(nameof(currentFormat));
        }

        #endregion
    }
}
