using System;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.UI.Properties;
using DWOS.UI.QA;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinToolbars;
using DWOS.Data.Order.Activity;

namespace DWOS.UI.Tools
{
    internal class PartInspectionCommand: GridCommand
    {
        #region Properties

        public override bool Enabled
        {
            get { return base.Enabled && _frmMain.SelectedWorkStatus ==  ApplicationSettings.Current.WorkStatusPendingQI && !_frmMain.SelectedInBatch; }
        }

        #endregion

        #region Methods

        public PartInspectionCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain, "ControlInspection")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                if (!Enabled)
                {
                    return;
                }

                var orderId = _frmMain.SelectedWO;
                int autoInspectionCount = 0;

                using (new MainRefreshHelper(_frmMain))
                {
                    while (true)
                    {
                        var activity = new ControlInspectionActivity(orderId, SecurityManager.Current.UserID);
                        activity.Initialize();

                        var canAutoComplete = ApplicationSettings.Current.ProcessStrictnessLevel == ProcessStrictnessLevel.AutoComplete &&
                                              activity.CanAutoComplete;

                        if (canAutoComplete)
                        {
                            _log.Info("Automatically saving inspection data for order " + activity.OrderID);

                            var results = activity.AutoCompleteInspection();
                            autoInspectionCount++;
                            if (results.RemainingInspections == 0)
                            {
                                break;
                            }
                        }
                        else
                        {
                            using (var op = new PartInspection())
                            {
                                op.Activity = activity;

                                //if canceled OR no remaining inspections abort quit loop
                                if (op.ShowDialog(Form.ActiveForm) != DialogResult.OK || !op.RemainingInspections)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (autoInspectionCount > 0)
                {
                    ShowAutoInspectionFlyout(autoInspectionCount, orderId);
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error running part inspection.";
                _log.Error(exc, errorMsg);
            }
        }

        private static void ShowAutoInspectionFlyout(int autoInspectionCount, int orderId)
        {
            var title = "Order";

            var msgFormat = autoInspectionCount == 1
                ? "Automatically passed {0} inspection for Order {1}"
                : "Automatically passed {0} inspections for Order {1}";

            var msg = string.Format(msgFormat,
                autoInspectionCount,
                orderId);

            DWOSApp.MainForm.FlyoutManager.DisplayFlyout(title, msg);
        }

        #endregion
    }

    internal class OrderContainersCommand: GridCommand
    {
        #region Properties

        public override bool Enabled
        {
            get { return base.Enabled && _frmMain.SelectedWO > 0; }
        }

        #endregion

        #region Methods

        public OrderContainersCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain, "COCPrintContainerLabels")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(this.Enabled)
            {
                using (new MainRefreshHelper(_frmMain))
                {
                    using (var oc = new OrderContainers())
                    {
                        oc.LoadData(_frmMain.SelectedWO);
                        oc.ShowDialog(Form.ActiveForm);
                    }
                }
            }
        }

        #endregion
    }

    internal class COCCommand : GridCommand
    {
        #region Properties

        public override bool Enabled => base.Enabled
            && ApplicationSettings.Current.COCEnabled
            && !_frmMain.SelectedInBatch
            && _frmMain.SelectedWorkStatus == ApplicationSettings.Current.WorkStatusFinalInspection;

        #endregion

        #region Methods

        public COCCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain, "COC")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!this.Enabled)
            {
                return;
            }

            using (new MainRefreshHelper(_frmMain))
            {
                bool checkContainers = false;

                using (var coc = new CreateCOC())
                {
                    coc.OrderID = _frmMain.SelectedWO;

                    checkContainers = coc.ShowDialog(Form.ActiveForm) == DialogResult.OK &&
                                      SecurityManager.Current.IsInRole("COCPrintContainerLabels");
                }

                if (checkContainers)
                {
                    int? containerCount = 0;

                    using (var ta = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderContainersTableAdapter())
                        containerCount = ta.GetContainerCount(_frmMain.SelectedWO);

                    if (containerCount.HasValue && containerCount.Value > 0)
                    {
                        using (var oc = new OrderContainers())
                        {
                            oc.LoadData(_frmMain.SelectedWO);
                            oc.PrintCurrentLabel = Settings.Default.FinalInspectionPrintLabel;
                            if (oc.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                            {
                                Settings.Default.FinalInspectionPrintLabel = oc.PrintCurrentLabel;
                                Settings.Default.Save();
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }

    internal class InternalReworkIdentifyCommand : GridCommand
    {
        #region Properties

        public override bool Enabled
        {
            get
            {
                return base.Enabled && _frmMain.SelectedWO > 0 && !_frmMain.SelectedHoldStatus && _frmMain.SelectedWorkStatus != ApplicationSettings.Current.WorkStatusPendingReworkPlanning && _frmMain.SelectedWorkStatus != ApplicationSettings.Current.WorkStatusPendingJoin && _frmMain.SelectedWorkStatus != ApplicationSettings.Current.WorkStatusPendingOR;
            }
        }

        #endregion

        #region Methods

        public InternalReworkIdentifyCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain, "InternalRework.Assessment")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (this.Enabled)
            {
                try
                {
                    if(_frmMain.SelectedWO > 0)
                    {
                        using (new MainRefreshHelper(_frmMain))
                        {
                            using (var frm = new QA.ReworkAssessment() { OrderID = _frmMain.SelectedWO })
                                frm.ShowDialog(Form.ActiveForm);
                        }
                    }
                }
                catch (Exception exc)
                {
                    string errorMsg = "Error running part inspection.";
                    _log.Error(exc, errorMsg);
                }
            }
        }

        #endregion
    }

    internal class InternalReworkPlanCommand : GridCommand
    {
        #region Properties

        public override bool Enabled
        {
            get
            {
                return base.Enabled && _frmMain.SelectedWO > 0 && _frmMain.SelectedWorkStatus == ApplicationSettings.Current.WorkStatusPendingReworkPlanning;
            }
        }

        #endregion

        #region Methods

        public InternalReworkPlanCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain, "InternalRework.Planning")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (this.Enabled)
            {
                try
                {
                    if (_frmMain.SelectedWO > 0)
                    {
                        using (new MainRefreshHelper(_frmMain))
                        {
                            //Add new processes to hold process
                            using (var frm = new QA.ReworkPlan() { OrderID = _frmMain.SelectedWO })
                                frm.ShowDialog(Form.ActiveForm);
                        }
                    }
                }
                catch (Exception exc)
                {
                    string errorMsg = "Error running part inspection.";
                    _log.Error(exc, errorMsg);
                }
            }
        }

        #endregion
    }

    internal class InternalReworkJoinCommand : GridCommand
    {
        #region Properties

        public override bool Enabled
        {
            get
            {
                //Only IntRework orders that are pending a join
                return base.Enabled && _frmMain.SelectedWO > 0 && _frmMain.SelectedWorkStatus == ApplicationSettings.Current.WorkStatusPendingJoin && _frmMain.SelectedOrderType.GetValueOrDefault(OrderType.Normal) == OrderType.ReworkInt;
            }
        }

        #endregion

        #region Methods

        public InternalReworkJoinCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain, "InternalRework.Join")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (this.Enabled)
            {
                try
                {
                    if (_frmMain.SelectedWO > 0)
                    {
                        using (new MainRefreshHelper(_frmMain))
                        {
                            using (var frm = new ReworkJoin() { ReworkOrderID = _frmMain.SelectedWO })
                                frm.ShowDialog(Form.ActiveForm);
                        }
                    }
                }
                catch (Exception exc)
                {
                    string errorMsg = "Error running part inspection.";
                    _log.Error(exc, errorMsg);
                }
            }
        }

        #endregion
    }
}