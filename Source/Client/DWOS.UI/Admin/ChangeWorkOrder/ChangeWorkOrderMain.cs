using NLog;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DWOS.UI.Admin.ChangeWorkOrder
{
    /// <summary>
    /// Main form for the Change Locked Work Order tool.
    /// </summary>
    public partial class ChangeWorkOrderMain : Form
    {
        #region Fields

        private int _currentPanelIndex = -1;
        private List<IChangeWorkOrderPanel> _panels;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the selected order ID for this instance.
        /// </summary>
        public int? SelectedOrderId { get; set; }

        #endregion

        #region Methods

        public ChangeWorkOrderMain()
        {
            InitializeComponent();
        }

        private void InitializeWizard()
        {
            _panels = new List<IChangeWorkOrderPanel>
            {
                new SelectWorkOrderPanel
                {
                    MainForm = this,
                    OnCanNavigateToNextPanelChange = OnCanNavigateToNextPanelChanged
                },

                new UpdateWorkOrderPanel
                {
                    MainForm = this,
                    OnCanNavigateToNextPanelChange = OnCanNavigateToNextPanelChanged
                }
            };
        }

        private void MoveToPanel(int panelIndex)
        {
            if (!IsValidPanelIndex(panelIndex))
            {
                return;
            }

            // Notify current panel that we are leaving it
            if (_currentPanelIndex != panelIndex && IsValidPanelIndex(_currentPanelIndex))
            {
                _panels[_currentPanelIndex].OnNavigateFrom();
            }

            _currentPanelIndex = panelIndex;

            var panel = _panels[panelIndex] as Control;
            pnlHolder.Controls.Clear();
            pnlHolder.Controls.Add(panel);
            panel.Dock = DockStyle.Fill;

            _panels[panelIndex].OnNavigateTo();

            // Update buttons based on current index
            btnNext.Text = _currentPanelIndex == _panels.Count - 1 ? "Finish" : "Next >";
            btnNext.Enabled = _panels[_currentPanelIndex].CanNavigateToNextPanel;
            btnPrevious.Enabled = _currentPanelIndex > 0;
        }

        private void Finish()
        {
            foreach (var panel in _panels)
            {
                panel.Finish();
            }

            btnNext.Enabled = false;
            btnPrevious.Enabled = false;
            btnCancel.Text = "Close";
        }

        private bool IsValidPanelIndex(int panelIndex)
        {
            return (panelIndex >= 0) && (panelIndex < _panels.Count);
        }

        private void OnDisposing()
        {
            foreach (var rp in _panels)
            {
                ((Control)rp).Dispose();
            }

            _panels.Clear();
        }

        #endregion

        #region Events

        private void ChangeWorkOrderMain_Load(object sender, EventArgs e)
        {
            try
            {
                InitializeWizard();
                MoveToPanel(0);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error in ChangeWorkOrderMain");
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentPanelIndex > 0)
                    MoveToPanel(_currentPanelIndex - 1);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error in ChangeWorkOrderMain");
            }

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentPanelIndex < _panels.Count - 1)
                {
                    if (IsValidPanelIndex(_currentPanelIndex))
                    {
                        if (!_panels[_currentPanelIndex].CanNavigateToNextPanel)
                            return;
                    }

                    MoveToPanel(_currentPanelIndex + 1);
                }
                else if (_currentPanelIndex == _panels.Count - 1)
                    Finish();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error in ChangeWorkOrderMain");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error in ChangeWorkOrderMain");
            }
        }

        private void OnCanNavigateToNextPanelChanged(IChangeWorkOrderPanel panel, bool status)
        {
            try
            {
                if (IsValidPanelIndex(_currentPanelIndex) && _panels[_currentPanelIndex] == panel)
                    btnNext.Enabled = status;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error in ChangeWorkOrderMain");
            }
        }

        #endregion
    }
}