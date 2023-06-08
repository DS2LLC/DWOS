using System.Windows;
using System.Windows.Controls;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.QBExport.Syncing;
using DWOS.Shared;
using DWOS.UI.Admin;
using DWOS.UI.Admin.OrderReset;
using DWOS.UI.Admin.Schedule;
using DWOS.UI.QA;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinStatusBar;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using System;
using System.Windows.Forms;
using DWOS.Shared.Utilities;
using System.IO;

namespace DWOS.UI.Tools
{
    internal class AddOrderSummaryCommand: CommandBase
    {
        #region Properties

        public override bool Enabled
        {
            get { return base.Enabled; }
        }

        #endregion

        #region Methods

        public AddOrderSummaryCommand(ToolBase tool)
            : base(tool)
        {
            
        }

        public override void OnClick()
        {
            try
            {
                DWOSApp.MainForm.AddTab(DwosTabFactory.CreateTab(OrderSummary2.DATA_TYPE, Guid.NewGuid().ToString(), "Orders"));

                if (!DWOSApp.MainForm.HasOrderData)
                {
                    DWOSApp.MainForm.RefreshData();
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error adding new order summary.");
            }
        }

        #endregion
    }

    internal class AddBatchSummaryCommand : CommandBase
    {
        #region Properties

        public override bool Enabled
        {
            get { return base.Enabled; }
        }

        #endregion

        #region Methods

        public AddBatchSummaryCommand(ToolBase tool)
            : base(tool)
        {

        }

        public override void OnClick()
        {
            try
            {
                DWOSApp.MainForm.AddTab(DwosTabFactory.CreateTab(BatchSummary.DATA_TYPE, Guid.NewGuid().ToString(), "Batches"));

                if (!DWOSApp.MainForm.HasBatchData)
                {
                    DWOSApp.MainForm.RefreshData();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding new order summary.");
            }
        }

        #endregion
    }

    internal class RenameTabCommand : CommandBase
    {
        #region Fields

        private Main _main = null;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get { return _main.ActiveTab != null; }
        }

        #endregion

        #region Methods

        public RenameTabCommand(ToolBase tool, Main main)
            : base(tool)
        {
            _main = main;
            _main.SelectedTabChanged += _tabManager_TabActivated;
        }

        public override void OnClick()
        {
            try
            {
                if(!this.Enabled)
                    return;

                var tab = _main.ActiveTab;

                using(var txt = new TextBoxForm())
                {
                    txt.FormLabel.Text = "Name:";
                    txt.Text = "Change Name";

                    if(txt.ShowDialog(Form.ActiveForm) == DialogResult.OK && !String.IsNullOrWhiteSpace(txt.FormTextBox.Text))
                    {
                        _main.RenameTab(tab, txt.FormTextBox.Text);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error renaming tab.");
            }
        }

        public override void Dispose()
        {
            _main = null;
            base.Dispose();
        }

        #endregion

        #region Events
        
        private void _tabManager_TabActivated(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }

    internal class SchedulingTabCommand: CommandBase
    {
        #region Properties

        public override bool Enabled => base.Enabled
            && ApplicationSettings.Current.UsingManualScheduling;

        #endregion

        #region Methods

        public SchedulingTabCommand(ToolBase tool)
            : base(tool, "OrderSchedule")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
            HideIfDisabled = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                DWOSApp.MainForm.AddTab(DwosTabFactory.CreateTab(Admin.Schedule.Manual.SchedulingTab.DATA_TYPE, Guid.NewGuid().ToString(), "Scheduling"));

                if (!DWOSApp.MainForm.HasOrderData)
                {
                    DWOSApp.MainForm.RefreshData();
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error showing scheduling tab.";
                _log.Error(exc, errorMsg);
            }
        }

        #endregion
    }

    #region PresentationModeCommand

    internal class PresentationModeCommand : CommandBase
    {
        #region Fields

        private Main _main = null;

        #endregion

        #region Methods

        public PresentationModeCommand(ToolBase tool, Main main)
            : base(tool)
        {
            _main = main;
        }

        public override void OnClick()
        {
            try
            {
                if (this._main.IsInPresentationMode)
                {
                    this._main.StopPresentationMode();
                }
                else
                {
                    this._main.StartPresentationMode();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error enabling (or disabling) presentation mode.");
            }
        }

        public override void Dispose()
        {
            _main = null;
            base.Dispose();
        }

        #endregion
    }
    #endregion

    internal class DeleteTabCommand : CommandBase
    {
        #region Fields

        private Main _main = null;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get { return _main.ActiveTab != null; }
        }

        #endregion

        #region Methods

        public DeleteTabCommand(ToolBase tool, Main main)
            : base(tool)
        {
            _main = main;
            _main.SelectedTabChanged += _tabManager_TabActivated;
        }

        public override void OnClick()
        {
            try
            {
                if (!this.Enabled)
                    return;

                _main.CloseTab(_main.ActiveTab);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error closing tab.");
            }
        }

        public override void Dispose()
        {
            _main = null;
            base.Dispose();
        }

        #endregion

        #region Events

        private void _tabManager_TabActivated(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }

    internal class ExportTabCommand : CommandBase
    {
        #region Fields

        private Main _main;

        #endregion

        #region Properties

        public override bool Enabled => _main.ActiveTab != null;

        #endregion

        #region Methods

        public ExportTabCommand(ToolBase tool, Main main)
            : base(tool)
        {
            _main = main;
            _main.SelectedTabChanged += _tabManager_TabActivated;
        }

        public override void OnClick()
        {
            try
            {
                var tab = _main.ActiveTab?.Export();
                if (!Enabled || tab == null)
                {
                    return;
                }

                //Assing a new unique ID, see bug 15163
                tab.Key = Guid.NewGuid().ToString();

                DwosTabDataUtilities.DoExport(tab);
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error exporting tab.", exc);
            }
        }

        public override void Dispose()
        {
            _main = null;
            base.Dispose();
        }

        #endregion

        #region Events

        private void _tabManager_TabActivated(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }

    internal class DashboardLoadCommand : CommandBase
    {
        #region Fields

        private Main _main = null;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get { return _main.ActiveTab is Dashboard.DashboardTab; }
        }

        #endregion

        #region Methods

        public DashboardLoadCommand(ToolBase tool, Main main)
            : base(tool)
        {
            _main = main;
            _main.SelectedTabChanged += _tabManager_TabActivated;
        }

        public override void OnClick()
        {
            const string fileFilter = "Dashboard Layout (*.dashboard)|*.dashboard";

            try
            {
                if (!this.Enabled)
                    return;

                var dash = _main.ActiveTab as Dashboard.DashboardTab;

                if(dash != null)
                {
                    using (var openFileDialog = new OpenFileDialog {Title = "Load Layout", Filter = fileFilter})
                    {
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            dash.LoadDashboardFile(openFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error closing tab.");
            }
        }

        public override void Dispose()
        {
            _main = null;
            base.Dispose();
        }

        #endregion

        #region Events

        private void _tabManager_TabActivated(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }

    internal class DashboardSaveCommand : CommandBase
    {
        #region Fields

        private Main _main = null;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get { return _main.ActiveTab is Dashboard.DashboardTab; }
        }

        #endregion

        #region Methods

        public DashboardSaveCommand(ToolBase tool, Main main)
            : base(tool)
        {
            _main = main;
            _main.SelectedTabChanged += _tabManager_TabActivated;
        }

        public override void OnClick()
        {
            const string fileFilter = "Dashboard Layout (*.dashboard)|*.dashboard";

            try
            {
                if (!this.Enabled)
                    return;

                var dash = _main.ActiveTab as Dashboard.DashboardTab;

                if (dash != null)
                {

                    using (var saveDialog = new SaveFileDialog {Title = "Save Layout", Filter = fileFilter})
                    {
                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            dash.SaveLayout(saveDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error closing tab.");
            }
        }

        public override void Dispose()
        {
            _main = null;
            base.Dispose();
        }

        #endregion

        #region Events

        private void _tabManager_TabActivated(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }

    internal class DashboardAddWidgetCommand : CommandBase
    {
        #region Fields

        private Main _main = null;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get { return _main.ActiveTab is Dashboard.DashboardTab; }
        }

        #endregion

        #region Methods

        public DashboardAddWidgetCommand(ToolBase tool, Main main)
            : base(tool)
        {
            _main = main;
            _main.SelectedTabChanged += _tabManager_TabActivated;

            var pop = tool as PopupGalleryTool;
            
            if(pop != null)
                pop.GalleryToolItemClick += pop_GalleryToolItemClick;
        }
        
        public override void OnClick()
        {
            
        }

        private void AddWidget(string key)
        {
            try
            {
                if (!this.Enabled)
                    return;

                var dash = _main.ActiveTab as Dashboard.DashboardTab;

                if (dash != null)
                {
                    dash.AddWidget(key);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error closing tab.");
            }
        }

        public override void Dispose()
        {
            _main = null;
            base.Dispose();
        }

        #endregion

        #region Events

        private void _tabManager_TabActivated(object sender, EventArgs e)
        {
            Refresh();
        }

        private void pop_GalleryToolItemClick(object sender, GalleryToolItemEventArgs e)
        {
            if(e.Item != null)
            {
                var key = e.Item.Key;
                AddWidget(key);
            }
        }

        #endregion
    }

    internal class AddHoldListTabCommand : CommandBase
    {
        #region Fields

        private Infragistics.Windows.DockManager.XamDockManager _dockManager = null;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get { return base.Enabled; }
        }

        #endregion

        #region Methods

        public AddHoldListTabCommand(ToolBase tool, Infragistics.Windows.DockManager.XamDockManager dockManager)
            : base(tool) { _dockManager = dockManager; }

        public override void OnClick()
        {
            try
            {
            //    <igWPF:XamDockManager.Panes>
            //    <igWPF:SplitPane igWPF:XamDockManager.InitialLocation="DockedBottom" SplitterOrientation="Horizontal" Width="100">
            //        <igWPF:ContentPane IsPinned="False" Header="Holds" AllowClose="False" x:Name="paneHolds" Image="Resources/images/Hold_16.png">
            //            <local:HoldList x:Name="holdList"></local:HoldList>
            //        </igWPF:ContentPane>
            //    </igWPF:SplitPane>
            //</igWPF:XamDockManager.Panes>

                Infragistics.Windows.DockManager.SplitPane splitPane = null;

                foreach(var pane in _dockManager.Panes)
                {
                    if(pane.Tag is string && pane.Tag.ToString() == "HOLD")
                    {
                        splitPane = pane;
                        break;
                    }
                }

                if(splitPane == null)
                {
                    splitPane = new Infragistics.Windows.DockManager.SplitPane();
                    splitPane.Tag = "HOLD";
                    splitPane.SplitterOrientation = System.Windows.Controls.Orientation.Horizontal;
                    Infragistics.Windows.DockManager.XamDockManager.SetInitialLocation(splitPane, Infragistics.Windows.DockManager.InitialPaneLocation.DockedBottom);
                    _dockManager.Panes.Add(splitPane);
                }

                splitPane.Panes.Add(new HoldContentPane());
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding new holds tab.");
            }
        }

        #endregion
    }
}