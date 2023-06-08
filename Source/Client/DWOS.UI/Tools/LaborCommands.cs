using DWOS.Data;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinToolbars;
using System;

namespace DWOS.UI.Tools
{
    internal class StartLaborTimerCommand: GridCommand
    {
        #region Fields

        private LaborCommandUtilities _utils;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get
            {
                var currentSecurityManager = SecurityManager.Current;
                var selectedWorkStatus = _frmMain.SelectedWorkStatus;

                return base.Enabled &&
                    _frmMain.SelectedWO > 0 &&
                    !_frmMain.SelectedInBatch &&
                    _utils.HasCorrectRole(currentSecurityManager, selectedWorkStatus) &&
                    (!_utils.IsProcessingStatus(selectedWorkStatus) || _utils.IsCurrentDepartment(_frmMain.SelectedLocation)) &&
                    !_utils.HasActiveTimer(_frmMain.SelectedWO, selectedWorkStatus, currentSecurityManager.UserID);
            }
        }

        #endregion

        #region Methods

        public StartLaborTimerCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain)
        {
            _utils = new LaborCommandUtilities();
        }

        public override void OnClick()
        {
            using (new MainRefreshHelper(_frmMain))
            {
                Data.Order.TimeCollectionUtilities
                    .StartOrderLaborTimer(_frmMain.SelectedWO, SecurityManager.Current.UserID);

                if (_utils.IsProcessingStatus(_frmMain.SelectedWorkStatus))
                {
                    Data.Order.TimeCollectionUtilities
                        .StartOrderProcessTimer(_frmMain.SelectedWO);
                }
            }
        }

        public override bool Refresh()
        {
            bool baseRefresh = base.Refresh();
            bool usingTimers = ApplicationSettings.Current.TimeTrackingEnabled;
            Button.Visible = usingTimers;

            return baseRefresh && usingTimers;
        }

        public override void Dispose()
        {
            base.Dispose();

            _utils?.Dispose();
            _utils = null;
        }

        #endregion
    }

    internal class StopLaborTimerCommand: GridCommand
    {
        #region Fields

        private LaborCommandUtilities _utils;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get
            {
                var currentSecurityManager = SecurityManager.Current;
                var selectedWorkStatus = _frmMain.SelectedWorkStatus;

                return base.Enabled &&
                    _frmMain.SelectedWO > 0 &&
                    !_frmMain.SelectedInBatch &&
                    _utils.HasCorrectRole(currentSecurityManager, selectedWorkStatus) &&
                    (!_utils.IsProcessingStatus(selectedWorkStatus) || _utils.IsCurrentDepartment(_frmMain.SelectedLocation)) &&
                    _utils.IsActiveOperator(_frmMain.SelectedWO, selectedWorkStatus, currentSecurityManager.UserID);
            }
        }

        #endregion

        #region Methods

        public StopLaborTimerCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain)
        {
            _utils = new LaborCommandUtilities();
        }

        public override void OnClick()
        {
            using (new MainRefreshHelper(_frmMain))
            {
                Data.Order.TimeCollectionUtilities
                    .StopOrderLaborTimer(_frmMain.SelectedWO, SecurityManager.Current.UserID);
            }
        }

        public override bool Refresh()
        {
            bool baseRefresh = base.Refresh();
            bool usingTimers = ApplicationSettings.Current.TimeTrackingEnabled;
            Button.Visible = usingTimers;

            return baseRefresh && usingTimers;
        }

        public override void Dispose()
        {
            base.Dispose();

            _utils?.Dispose();
            _utils = null;
        }

        #endregion
    }

    internal class PauseLaborTimerCommand: GridCommand
    {
        #region Fields

        private LaborCommandUtilities _utils;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get
            {
                var selectedWorkStatus = _frmMain.SelectedWorkStatus;
                var currentSecurityManager = SecurityManager.Current;

                return base.Enabled &&
                    _frmMain.SelectedWO > 0 &&
                    !_frmMain.SelectedInBatch &&
                    _utils.HasCorrectRole(currentSecurityManager, selectedWorkStatus) &&
                    (!_utils.IsProcessingStatus(selectedWorkStatus) || _utils.IsCurrentDepartment(_frmMain.SelectedLocation)) &&
                    _utils.HasActiveTimer(_frmMain.SelectedWO, selectedWorkStatus, currentSecurityManager.UserID);
            }
        }

        #endregion

        #region Methods

        public PauseLaborTimerCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain)
        {
            _utils = new LaborCommandUtilities();
        }

        public override void OnClick()
        {
            using (new MainRefreshHelper(_frmMain))
            {
                Data.Order.TimeCollectionUtilities
                    .PauseOrderLaborTimer(_frmMain.SelectedWO, SecurityManager.Current.UserID);
            }
        }

        public override bool Refresh()
        {
            bool baseRefresh = base.Refresh();
            bool usingTimers = ApplicationSettings.Current.TimeTrackingEnabled;
            Button.Visible = usingTimers;

            return baseRefresh && usingTimers;
        }

        public override void Dispose()
        {
            base.Dispose();

            _utils.Dispose();
            _utils = null;
        }

        #endregion
    }

    internal sealed class LaborCommandUtilities : IDisposable
    {
        private bool _disposed;

        private Data.Datasets.OrderProcessingDataSetTableAdapters.LaborTimeTableAdapter _taLaborTime =
            new Data.Datasets.OrderProcessingDataSetTableAdapters.LaborTimeTableAdapter();

        private Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesOperatorTableAdapter _taProcessOperator =
            new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesOperatorTableAdapter();

        private Data.Datasets.OrderProcessingDataSetTableAdapters.OrderOperatorTableAdapter _taOrderOperator =
            new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderOperatorTableAdapter();

        private Data.Datasets.OrderProcessingDataSetTableAdapters.OrderOperatorTimeTableAdapter _taOperatorTime =
            new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderOperatorTimeTableAdapter();

        public bool HasCorrectRole(SecurityManager currentSecurityManager, string selectedWorkStatus)
        {
            if (currentSecurityManager == null)
            {
                return false;
            }

            string correctRole = null;
            var settings = ApplicationSettings.Current;

            // Allow any user to log time for orders in Hold,
            // but require the correct role for every other status.
            if (selectedWorkStatus == settings.WorkStatusPendingOR)
            {
                correctRole = "OrderReview";
            }
            else if (selectedWorkStatus == settings.WorkStatusChangingDepartment)
            {
                correctRole = "PartCheckIn";
            }
            if (selectedWorkStatus == settings.WorkStatusInProcess)
            {
                correctRole = "OrderProcessing";
            }
            else if (selectedWorkStatus == settings.WorkStatusPendingQI)
            {
                correctRole = "ControlInspection";
            }
            else if (selectedWorkStatus == settings.WorkStatusPendingReworkAssessment)
            {
                correctRole = "InternalRework.Assessment";
            }
            else if (selectedWorkStatus == settings.WorkStatusPendingReworkPlanning)
            {
                correctRole = "InternalRework.Planning";
            }
            else if (selectedWorkStatus == settings.WorkStatusPendingJoin)
            {
                correctRole = "InternalRework.Join";
            }
            else if (selectedWorkStatus == settings.WorkStatusPartMarking)
            {
                correctRole = "PartMarking";
            }
            else if (selectedWorkStatus == settings.WorkStatusFinalInspection)
            {
                correctRole = "COC";
            }
            else if (selectedWorkStatus == settings.WorkStatusShipping)
            {
                correctRole = "ShippingManager";
            }

            return string.IsNullOrEmpty(correctRole) || currentSecurityManager.IsInRole(correctRole);
        }

        public bool IsProcessingStatus(string selectedWorkStatus) =>
            ApplicationSettings.Current.ProcessingWorkStatuses.Contains(selectedWorkStatus);

        public bool IsCurrentDepartment(string selectedLocation) =>
            selectedLocation == Properties.Settings.Default.CurrentDepartment;

        public bool HasActiveTimer(int orderId, string workStatus, int userId)
        {
            if (IsProcessingStatus(workStatus))
            {
                var countForProcess = _taLaborTime.GetOrderUserActiveTimerCount(orderId, userId) ?? 0;
                return countForProcess > 0;
            }

            var countForOrder = _taOperatorTime.GetUserActiveTimerCount(orderId, userId) ?? 0;
            return countForOrder > 0;
        }

        public bool IsActiveOperator(int orderId, string workStatus, int userId)
        {
            if (IsProcessingStatus(workStatus))
            {
                var countForProcess = _taProcessOperator.GetUserOperatorCount(nameof(OperatorStatus.Active), userId, orderId) ?? 0;
                return countForProcess > 0;
            }

            var countForOrder = _taOrderOperator.GetUserOperatorCount(nameof(OperatorStatus.Active), userId, orderId) ?? 0;
            return countForOrder > 0;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _taLaborTime.Dispose();
            _taProcessOperator.Dispose();
            _taOrderOperator.Dispose();
            _taOperatorTime.Dispose();

            _disposed = true;
        }
    }
}
