using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.DocumentsDataSetTableAdapters;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Properties;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Ionic.Zlib;
using NLog;

namespace DWOS.UI.Documents.Controls
{
    public partial class DocLinkList : UserControl
    {
        #region Fields

        private bool _inSelectingLink = false;

        #endregion

        #region Methods

        public DocLinkList() { InitializeComponent(); }

        public void ClearLinks()
        {
            this.cboLinks.Items.Clear();
            
            UpdateListState();
        }


        /// <summary>
        /// Loads the links based on type and id, may be called more than one time (i.e. process and process alias).
        /// </summary>
        /// <param name="linkType">Type of the link.</param>
        /// <param name="linkID">The link identifier.</param>
        public void LoadLinks(LinkType linkType, int linkID)
        {
            try
            {
                CreateAppearance(linkType);

                using(var ta = new DocumentLinkTableAdapter() {ClearBeforeFill = false})
                {
                    var documentLinks = new DocumentsDataSet.DocumentLinkDataTable();
                    ta.FillBy(documentLinks, linkType.ToString(), linkID);
                    var namePostfix = linkType == LinkType.ProcessAlias ? " (Alias)" : (linkType == LinkType.Process ? " (Process)" : "");
                    
                    foreach(var documentLink in documentLinks)
                    {
                        var displayName = DocumentUtilities.GetFileName(documentLink.DocumentInfoID) + namePostfix;
                        var linkItem = new ValueListItem(documentLink.DocumentInfoID, displayName)
                        {
                            Tag = linkType,
                            Appearance = this.cboLinks.Appearances[linkType.ToString()]
                        };

                        cboLinks.Items.Add(linkItem);
                    }
                }

                UpdateListState();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading links for ID {0} of type {1}.".FormatWith(linkID, linkType));
            }
        }

        public void SelectDefaultLink()
        {
            if (cboLinks.Items.Count > 0)
            {
                _inSelectingLink = true;

                var linkItem = this.cboLinks.Items.Cast <ValueListItem>().FirstOrDefault(item => item.Tag is LinkType && ((LinkType) item.Tag) == LinkType.ProcessAlias);

                if(linkItem == null && cboLinks.Items.Count > 0)
                    linkItem = cboLinks.Items[0];

                if(linkItem != null)
                    cboLinks.SelectedItem = linkItem;

                _inSelectingLink = false;
            }
        }

        private void UpdateListState()
        {
            cboLinks.ButtonsLeft[0].Enabled = cboLinks.Items.Count > 0;
        }

        private void ShowSelectedLink()
        {
            try
            {
                if (this.cboLinks.SelectedItem != null)
                {
                    var documentInfoID = Convert.ToInt32(this.cboLinks.SelectedItem.DataValue);

                    string filePath = DocumentUtilities.DownloadDocument(documentInfoID);

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        FileLauncher.New()
                            .HandleErrorsWith((exception, errorMsg) => MessageBoxUtilities.ShowMessageBoxWarn(errorMsg, "Unable to Start", filePath))
                            .Launch(filePath);
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error showing selected link.");
            }
        }

        private void CreateAppearance(LinkType linkType)
        {
            if(!cboLinks.Appearances.Exists(linkType.ToString()))
            {
                var app = cboLinks.Appearances.Add(linkType.ToString());
            
                switch(linkType)
                {
                    case LinkType.Process:
                        app.ForeColor = System.Drawing.Color.Blue;
                        break;
                    case LinkType.ProcessAlias:
                        app.ForeColor = System.Drawing.Color.Orange;
                        break;
                    case LinkType.ProcessSteps:
                        app.ForeColor = System.Drawing.Color.Green;
                        break;
                    case LinkType.ControlInspection:
                        app.ForeColor = System.Drawing.Color.Yellow;
                        break;
                }
            }
        }

        #endregion

        #region Events

        private void cboLinks_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if(!_inSelectingLink)
                    ShowSelectedLink();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on selecting link.");
            }
        }

        private void cboLinks_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e) { ShowSelectedLink(); }

        #endregion
    }
}