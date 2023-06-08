using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.PartsDatasetTableAdapters;
using DWOS.Shared.Wizard;
using DWOS.UI.Utilities;
using NLog;
using DWOS.Shared;
using DWOS.UI.Tools;
using DWOS.Data;

namespace DWOS.UI.Admin.RevisePartProcess
{
    public partial class RevisePartsPanel : UserControl, IWizardPanel
    {
        #region Fields

        private RevisePartProcessController _controller;
        private readonly PartsData _partsData = new PartsData();
        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("ReviseParts", new UltraGridBandSettings());

        #endregion

        #region Methods

        public RevisePartsPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region IWizardPanel Members

        public string Title => "Outdated Parts";

        public string SubTitle => string.Empty;

        public Action<IWizardPanel> OnValidStateChanged { get; set; }

        public Control PanelControl => this;

        public bool IsValid { get; set; }

        public void Initialize(WizardController controller)
        {
            _controller = controller as RevisePartProcessController;
        }

        public void OnFinished()
        {
            _controller = null;
            _partsData.Dispose();
        }

        public void OnMoveFrom()
        {

        }

        public void OnMoveTo()
        {
            _partsData.LoadData();
            grdParts.DataSource = _partsData.Parts;

            if (_partsData.Parts.Count == 0)
            {
                // Skip part revision
                btnRevise.Enabled = false;
                IsValid = true;
                OnValidStateChanged?.Invoke(this);
            }
        }

        private void RefreshInterfaceForSettings()
        {
            var showNextRevision = chkRevise.Checked;
            grdParts.DisplayLayout.Bands[0].Columns["NextRevision"].Hidden = !showNextRevision;

            btnRevise.Text = showNextRevision ? "Revise Parts" : "Update Parts";
        }

        #endregion

        #region Events

        private void grdParts_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                grdParts.AfterColPosChanged -= grdParts_AfterColPosChanged;
                grdParts.AfterSortChange -= grdParts_AfterSortChange;

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(grdParts.DisplayLayout.Bands[0]);

                RefreshInterfaceForSettings();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing part grid");
            }
            finally
            {
                grdParts.AfterColPosChanged += grdParts_AfterColPosChanged;
                grdParts.AfterSortChange += grdParts_AfterSortChange;
            }
        }

        private void grdParts_AfterColPosChanged(object sender, Infragistics.Win.UltraWinGrid.AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdParts.DisplayLayout.Bands[0]);

                // Remove 'Next Revision' setting from hidden - gets hidden automatically
                settings.HiddenColumns.Remove("NextRevision");

                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column position in grid.");
            }
        }

        private void grdParts_AfterSortChange(object sender, Infragistics.Win.UltraWinGrid.BandEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdParts.DisplayLayout.Bands[0]);

                // Remove 'Next Revision' setting from hidden - gets hidden automatically
                settings.HiddenColumns.Remove("NextRevision");

                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column sort in grid.");
            }
        }

        private void chkRevise_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                RefreshInterfaceForSettings();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing 'revise' checkbox");
            }
        }

        private void btnRevise_Click(object sender, EventArgs e)
        {
            try
            {
                btnRevise.Enabled = false;
                var reviseParts = chkRevise.Checked;

                if (reviseParts)
                {
                    _controller.Parts = _partsData.Revise();
                }
                else
                {
                    _controller.Parts = _partsData.Update();
                }
                progressBar.Value = 100;
                IsValid = true;
                OnValidStateChanged?.Invoke(this);
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error revising processes.", exc);
            }
        }

        #endregion

        #region PartsData

        private class PartsData : IDisposable
        {
            #region Fields

            private readonly PartsDataset _dsParts = new PartsDataset();
            private readonly TableAdapterManager _taManager;
            private readonly ProcessTableAdapter _taProcess = new ProcessTableAdapter();
            private bool _isLoaded;

            #endregion

            #region Properties

            public List<PartData> Parts { get; } = new List<PartData>();

            #endregion

            #region Methods

            public PartsData()
            {
                _taManager = new TableAdapterManager
                {
                    d_AirframeTableAdapter = new d_AirframeTableAdapter { ClearBeforeFill = false },
                    d_ManufacturerTableAdapter = new d_ManufacturerTableAdapter { ClearBeforeFill = false },
                    d_MaterialTableAdapter = new d_MaterialTableAdapter { ClearBeforeFill = false },
                    PartTableAdapter = new PartTableAdapter { ClearBeforeFill = false },
                    MediaTableAdapter = new MediaTableAdapter { ClearBeforeFill = false },
                    Part_MediaTableAdapter = new Part_MediaTableAdapter { ClearBeforeFill = false },
                    PartProcessTableAdapter = new PartProcessTableAdapter { ClearBeforeFill = false },
                    PartProcessVolumePriceTableAdapter = new PartProcessVolumePriceTableAdapter { ClearBeforeFill = false },
                    PartProcessAnswerTableAdapter = new PartProcessAnswerTableAdapter { ClearBeforeFill = false },
                    PartAreaTableAdapter = new PartAreaTableAdapter { ClearBeforeFill = false },
                    PartAreaDimensionTableAdapter = new PartAreaDimensionTableAdapter { ClearBeforeFill = false },
                    Part_DocumentLinkTableAdapter = new Part_DocumentLinkTableAdapter { ClearBeforeFill = false },
                    Part_PartMarkingTableAdapter = new Part_PartMarkingTableAdapter { ClearBeforeFill = false }
                };
            }

            public void LoadData()
            {
                if (_isLoaded)
                {
                    return;
                }

                using (new UsingDataSetLoad(_dsParts))
                {
                    _taManager.d_AirframeTableAdapter.Fill(_dsParts.d_Airframe);
                    _taManager.d_ManufacturerTableAdapter.Fill(_dsParts.d_Manufacturer);
                    _taManager.d_MaterialTableAdapter.Fill(_dsParts.d_Material);

                    using (var taCustomer = new CustomerTableAdapter())
                    {
                        taCustomer.Fill(_dsParts.Customer);
                    }

                    using (var taPriceUnit = new PriceUnitTableAdapter())
                    {
                        taPriceUnit.Fill(_dsParts.PriceUnit);
                    }

                    using (var taProcessAlias = new ProcessAliasTableAdapter())
                    {
                        taProcessAlias.Fill(_dsParts.ProcessAlias);
                    }

                    _taProcess.Fill(_dsParts.Process);
                    _taManager.PartTableAdapter.FillActiveWithOutdatedProcesses(_dsParts.Part);

                    foreach (var part in _dsParts.Part)
                    {
                        Parts.Add(PartData.From(part));
                        _taManager.MediaTableAdapter.FillByPartIDWithoutMedia(_dsParts.Media, part.PartID);
                        _taManager.Part_MediaTableAdapter.FillByPartID(_dsParts.Part_Media, part.PartID);
                        _taManager.PartProcessTableAdapter.FillByPart(_dsParts.PartProcess, part.PartID);
                        _taManager.PartProcessVolumePriceTableAdapter.FillByPartID(_dsParts.PartProcessVolumePrice, part.PartID);

                        _taManager.PartAreaTableAdapter.FillByPartID(_dsParts.PartArea, part.PartID);
                        _taManager.PartAreaDimensionTableAdapter.FillByPartID(_dsParts.PartAreaDimension, part.PartID);
                        _taManager.Part_DocumentLinkTableAdapter.FillByPartID(_dsParts.Part_DocumentLink, part.PartID);
                        _taManager.Part_PartMarkingTableAdapter.FillByPartID(_dsParts.Part_PartMarking, part.PartID);
                    }
                }

                _isLoaded = true;
            }

            public RevisionResults<PartData> Revise()
            {
                var results = new RevisionResults<PartData>
                {
                    Success = new List<PartData>(),
                    Failure = new List<PartData>(),
                };

                foreach (var part in Parts)
                {
                    if (RevisePart(_dsParts.Part.FindByPartID(part.PartId), part.NextRevision))
                    {
                        results.Success.Add(part);
                    }
                    else
                    {
                        results.Failure.Add(part); 
                    }
                }

                var addedPartRows = DataUtilities
                    .GetRowsByRowState(_dsParts.Part, DataRowState.Added)
                    .OfType<PartsDataset.PartRow>();

                _taManager.UpdateAll(_dsParts);

                // Update history for successfully revised parts.
                // Assumption: New parts are revisions.
                using (var taHistory = new Data.Datasets.OrderHistoryDataSetTableAdapters.PartHistoryTableAdapter())
                {
                    foreach (var part in addedPartRows)
                    {
                        taHistory.UpdatePartHistory(part.PartID,
                            "Revise Parts and Processes",
                            "Part revised.",
                            SecurityManager.Current.UserName);
                    }
                }

                return results;
            }

            public RevisionResults<PartData> Update()
            {
                var results = new RevisionResults<PartData>
                {
                    Success = new List<PartData>(),
                    Failure = new List<PartData>(),
                };

                foreach (var part in Parts)
                {
                    if (UpdatePart(_dsParts.Part.FindByPartID(part.PartId)))
                    {
                        results.Success.Add(part);
                    }
                    else
                    {
                        results.Failure.Add(part); 
                    }
                }

                _taManager.UpdateAll(_dsParts);

                // Update history for successfully updated parts.
                using (var taHistory = new Data.Datasets.OrderHistoryDataSetTableAdapters.PartHistoryTableAdapter())
                {
                    foreach (var part in results.Success)
                    {
                        taHistory.UpdatePartHistory(part.PartId,
                            "Revise Parts and Processes",
                            "Updated out-of-date processes for part.",
                            SecurityManager.Current.UserName);
                    }
                }

                return results;
            }

            /// <summary>
            /// Updates a part in-place.
            /// </summary>
            /// <param name="partRow"></param>
            private bool UpdatePart(PartsDataset.PartRow partRow)
            {
                var errorDuringUpdate = false;
                foreach (var partProcess in partRow.GetPartProcessRows())
                {
                    // Find most recent process revision
                    var revisedProcessId = _taProcess.Get_RevisedProcess(partProcess.ProcessID) as int?;

                    var revisedProcess = _dsParts.Process.FindByProcessID(revisedProcessId ?? -1);

                    if (revisedProcess == null || revisedProcess.GetProcessAliasRows().Length == 0)
                    {
                        if (revisedProcessId.HasValue)
                        {
                            LogManager.GetCurrentClassLogger()
                                .Error(
                                    revisedProcess == null
                                        ? "Unable to find row for Process ID {0}"
                                        : "Unable to find any aliases for Process ID {0}", revisedProcessId);
                        }

                        errorDuringUpdate = true;
                        continue;
                    }

                    var aliasName = partProcess.ProcessAliasRow?.Name;

                    // Swap process & alias in PartProcess with new values
                    partProcess.ProcessRow = revisedProcess;
                    partProcess.ProcessAliasRow = revisedProcess.GetProcessAliasRows().FirstOrDefault(a => a.Name == aliasName) ??
                        revisedProcess.GetProcessAliasRows().FirstOrDefault();
                }

                return !errorDuringUpdate;
            }

            /// <summary>
            /// Creates a new part revision with updated processes.
            /// </summary>
            /// <param name="partRow"></param>
            /// <param name="revision"></param>
            private bool RevisePart(PartsDataset.PartRow partRow, string revision)
            {
                // Copy process
                var disallowedRelations = new List<string>
                {
                    "FK_Part_Media_Part"
                };

                var revisionProxy = CopyCommand.CopyRows(partRow);
                revisionProxy.Remove(w => disallowedRelations.Contains(w.ParentRelation));

                // Revise
                if (DataNode <DataRow>.AddPastedDataRows(revisionProxy, _dsParts.Part) is PartsDataset.PartRow partRevision)
                {
                    partRow.Active = false;
                    partRevision.Active = true;
                    partRevision.ParentID = partRow.PartID;
                    partRevision.Revision = revision;

                    // Copy media
                    foreach (var originalPartMedia in partRow.GetPart_MediaRows())
                    {
                        var newPartMedia = _dsParts.Part_Media.NewPart_MediaRow();
                        newPartMedia.PartRow = partRevision;
                        newPartMedia.MediaRow = originalPartMedia.MediaRow;

                        if (!originalPartMedia.IsDefaultMediaNull())
                        {
                            newPartMedia.DefaultMedia = originalPartMedia.DefaultMedia;
                        }

                        _dsParts.Part_Media.AddPart_MediaRow(newPartMedia);
                    }

                    // Update revision as if it were the original part
                    return UpdatePart(partRevision);
                }
                return false;
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                _taManager.Dispose();
                _dsParts.Dispose();
                _taProcess.Dispose();
            }

            #endregion
        }

        #endregion
    }
}
