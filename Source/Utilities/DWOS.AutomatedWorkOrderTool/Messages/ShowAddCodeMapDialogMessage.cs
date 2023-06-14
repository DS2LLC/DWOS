using System;
using System.Collections.Generic;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.AutomatedWorkOrderTool.Messages
{
    public class ShowAddCodeMapDialogMessage : MessageBase
    {
        #region Properties

        public OspFormatEditorViewModel CurrentFormat { get; }

        public List<OspFormatSectionViewModel> CurrentSections { get; set; }

        #endregion

        #region Methods

        public ShowAddCodeMapDialogMessage(OspFormatEditorViewModel currentFormat, List<OspFormatSectionViewModel> currentSections)
        {
            CurrentSections = currentSections ?? throw new ArgumentNullException(nameof(currentSections));
            CurrentFormat = currentFormat ?? throw new ArgumentNullException(nameof(currentFormat));
        }

        #endregion
    }
}
