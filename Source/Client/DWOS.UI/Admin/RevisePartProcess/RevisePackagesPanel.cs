using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DWOS.Shared.Wizard;
using DWOS.UI.Utilities;
using NLog;
using DWOS.Shared;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ProcessPackageDatasetTableAdapters;
using System.Linq;
using DWOS.Data;

namespace DWOS.UI.Admin.RevisePartProcess
{
    public partial class RevisePackagesPanel : UserControl, IWizardPanel
    {
        #region Fields

        private RevisePartProcessController _controller;
        private readonly PackagesData _packagesData = new PackagesData();
        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("RevisePackages", new UltraGridBandSettings());

        #endregion

        #region Methods

        public RevisePackagesPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void btnRevise_Click(object sender, EventArgs e)
        {
            try
            {
                btnRevise.Enabled = false;
                _controller.ProcessPackages = _packagesData.Revise();
                progressBar.Value = 100;
                IsValid = true;
                OnValidStateChanged?.Invoke(this);
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error revising process packages.", exc);
            }
        }

        private void grdPackages_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                grdPackages.AfterColPosChanged -= grdPackages_AfterColPosChanged;
                grdPackages.AfterSortChange -= grdPackages_AfterSortChange;

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(grdPackages.DisplayLayout.Bands[0]);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing export grid layout.");
            }
            finally
            {
                grdPackages.AfterColPosChanged += grdPackages_AfterColPosChanged;
                grdPackages.AfterSortChange += grdPackages_AfterSortChange;
            }
        }

        private void grdPackages_AfterColPosChanged(object sender, Infragistics.Win.UltraWinGrid.AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdPackages.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column position in grid.");
            }
        }

        private void grdPackages_AfterSortChange(object sender, Infragistics.Win.UltraWinGrid.BandEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdPackages.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column position in grid.");
            }
        }

        #endregion

        #region IWizardPanel Members

        public string Title => "Outdated Process Packages";

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
            _packagesData.Dispose();
        }

        public void OnMoveFrom()
        {
        }

        public void OnMoveTo()
        {
            _packagesData.LoadData();
            grdPackages.DataSource = _packagesData.Packages;

            if (_packagesData.Packages.Count == 0)
            {
                // Skip part revision
                btnRevise.Enabled = false;
                IsValid = true;
                OnValidStateChanged?.Invoke(this);
            }
        }

        #endregion

        #region PackagesData

        private class PackagesData : IDisposable
        {
            #region Fields

            private readonly ProcessPackageDataset _dsPackage = new ProcessPackageDataset();
            private readonly TableAdapterManager _taManager;
            private readonly Data.Datasets.PartsDatasetTableAdapters.ProcessTableAdapter _taProcess =
                new Data.Datasets.PartsDatasetTableAdapters.ProcessTableAdapter();

            private bool _isLoaded;

            #endregion


            #region Properties

            public List<ProcessPackageData> Packages { get; } = new List<ProcessPackageData>();

            #endregion


            #region Methods

            public PackagesData()
            {
                _taManager = new TableAdapterManager
                {
                    ProcessPackageTableAdapter = new ProcessPackageTableAdapter(),
                    ProcessPackage_ProcessesTableAdapter = new ProcessPackage_ProcessesTableAdapter { ClearBeforeFill = false }
                };
            }

            public void LoadData()
            {
                if (_isLoaded)
                {
                    return;
                }

                using (new UsingDataSetLoad(_dsPackage))
                {
                    // Fill processes
                    using (var taProcess = new ProcessTableAdapter())
                    {
                        taProcess.Fill(_dsPackage.Process);
                    }

                    using (var taProcessAlias = new ProcessAliasTableAdapter())
                    {
                        taProcessAlias.Fill(_dsPackage.ProcessAlias);
                    }

                    _taManager.ProcessPackageTableAdapter.FillWithOutdatedProcesses(_dsPackage.ProcessPackage);

                    foreach (var processPackageRow in _dsPackage.ProcessPackage)
                    {
                        _taManager.ProcessPackage_ProcessesTableAdapter.FillByProcessPackageID(_dsPackage.ProcessPackage_Processes,
                            processPackageRow.ProcessPackageID);
                    }
                }

                foreach (var processPackageRow in _dsPackage.ProcessPackage)
                {
                    Packages.Add(ProcessPackageData.From(processPackageRow));
                }

                _isLoaded = true;
            }

            public RevisionResults<ProcessPackageData> Revise()
            {
                var results = new RevisionResults<ProcessPackageData>
                {
                    Success = new List<ProcessPackageData>(),
                    Failure = new List<ProcessPackageData>(),
                };

                foreach (var package in Packages)
                {
                    if (RevisePackage(_dsPackage.ProcessPackage.FindByProcessPackageID(package.ProcessPackageId)))
                    {
                        results.Success.Add(package);
                    }
                    else
                    {
                        results.Failure.Add(package);
                    }
                }

                _taManager.UpdateAll(_dsPackage);
                return results;
            }

            private bool RevisePackage(ProcessPackageDataset.ProcessPackageRow processPackageRow)
            {
                var inactivePackageProcesses = processPackageRow
                    .GetProcessPackage_ProcessesRows()
                    .Where(p => p.ProcessRow != null && !p.ProcessRow.Active);

                var errorDuringUpdate = false;
                foreach (var packageProcess in inactivePackageProcesses)
                {
                    // Find newest version
                    var revisedProcessId = _taProcess.Get_RevisedProcess(packageProcess.ProcessID) as int?;
                    var revisedProcess = _dsPackage.Process.FindByProcessID(revisedProcessId ?? -1);

                    if (revisedProcess == null)
                    {
                        LogManager.GetCurrentClassLogger()
                            .Error("Unable to find row for Process ID {0}", packageProcess.ProcessID);

                        errorDuringUpdate = true;
                        continue;
                    }
                    if (revisedProcess.ProcessID == packageProcess.ProcessID)
                    {
                        // Process is already at latest revision.
                        continue;
                    }

                    // Find alias
                    ProcessPackageDataset.ProcessAliasRow revisedAlias = revisedProcess
                        .GetProcessAliasRows()
                        .FirstOrDefault();

                    foreach (var alias in revisedProcess.GetProcessAliasRows())
                    {
                        if (alias.Name == packageProcess.ProcessAliasRow.Name)
                        {
                            // Exact match
                            revisedAlias = alias;
                            break;
                        }
                    }

                    // Update process & alias for package
                    if (revisedAlias == null)
                    {
                        LogManager.GetCurrentClassLogger()
                            .Error("Unable to find alias for Process ID {0}", packageProcess.ProcessID);

                        errorDuringUpdate = true;
                    }
                    else
                    {
                        packageProcess.ProcessRow = revisedProcess;
                        packageProcess.ProcessAliasRow = revisedAlias;
                    }
                }

                return !errorDuringUpdate;
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                _taManager.Dispose();
                _dsPackage.Dispose();
                _taProcess?.Dispose();
            }

            #endregion
        }

        #endregion
    }
}
