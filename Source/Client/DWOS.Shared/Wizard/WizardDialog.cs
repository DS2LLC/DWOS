using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NLog;

namespace DWOS.Shared.Wizard
{
    /// <summary>
    /// Multi-panel dialog for guiding users through a multi-step process.
    /// </summary>
    public partial class WizardDialog : Form
    {
        #region Fields
        
        private IWizardPanel _currentPanel;
        private List <IWizardPanel> _panels;
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        
        #endregion

        #region Properties

        private WizardController Controller { get; set; }

        /// <summary>
        /// Gets the panels for this instance.
        /// </summary>
        public IEnumerable<IWizardPanel> Panels
        {
            get
            {
                return _panels.AsReadOnly();
            }
        }
        
        #endregion

        #region Methods
        
        public WizardDialog()
        {
            InitializeComponent();
            _panels = new List<IWizardPanel>();
        }

        /// <summary>
        /// Initializes this instance prior to display.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="panels"></param>
        public void InitializeWizard(WizardController controller, List<IWizardPanel> panels)
        {
            this.Controller = controller;
            this.Controller.Dialog = this;
            this.Text       = controller.WizardTitle;

            _panels.Clear();
            panels.ForEach(pnl =>
                           {
                               this.InitializePanel(pnl);
                               _panels.Add(pnl);
                           });

            MoveTo(panels[0]);
        }

        public void AddPanel(IWizardPanel panel)
        {
            this.InitializePanel(panel);
            _panels.Add(panel);

            UpdateWizardButtons();
        }

        public void RemovePanel(IWizardPanel panel)
        {
            panel.OnValidStateChanged = null;
            panel.Dispose();
            _panels.Remove(panel);

            UpdateWizardButtons();
        }

        private void UpdateWizardButtons()
        {
            if(_currentPanel != null)
            {
                var panelIndex = _panels.IndexOf(_currentPanel);
                var isFirst = panelIndex == 0;
                var isLast = panelIndex == _panels.Count - 1;

                btnNext.Text = isLast ? "Finish" : "Next >";
                btnNext.Enabled = _currentPanel.IsValid;

                btnBack.Enabled = !isFirst;

                this.Text = this.Controller.WizardTitle + " ({0} of {1})".FormatWith(panelIndex + 1, _panels.Count);
            }
        }

        private void MoveTo(IWizardPanel panel)
        {
            _log.Info("Moving to panel {0}.", panel.Title);
           
            if(_currentPanel != null)
                _currentPanel.OnMoveFrom();
            
            //remove control
            pnlBody.ClientArea.Controls.Clear();

            this.lblTitle.Text = panel.Title;
            this.lblDescription.Text = panel.SubTitle;

            //Add control
            pnlBody.ClientArea.Controls.Add(panel.PanelControl);
            panel.PanelControl.Dock = DockStyle.Fill;

            _currentPanel = panel;

            UpdateWizardButtons();

            //ensure move to is last step incase it calls ValidStateChange in OnMoveTo
            panel.OnMoveTo();
        }

        private void InitializePanel(IWizardPanel panel)
        {
            panel.Initialize(this.Controller);
            panel.OnValidStateChanged = ValidStateChange;
        }

        private void ValidStateChange(IWizardPanel panel)
        {
            if(_currentPanel == panel)
            {
                btnNext.Enabled = panel.IsValid;
            }
        }
        
        private void OnDisposeMe()
        {
            _currentPanel = null;

            if(_panels != null)
            {
                _panels.ForEach(p => { p.OnValidStateChanged = null; p.Dispose(); });
                _panels = null;
            }

            this.Controller = null;
        }

        #endregion

        #region Events

        private void btnNext_Click(object sender, EventArgs e)
        {
            var isLast = _panels.IndexOf(_currentPanel) == _panels.Count - 1;

            if (isLast)
            {
                _log.Info("Finishing wizard.");
                _panels.ForEach(wp => wp.OnFinished());
                this.Controller.Finished();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else
            {
                var nextPanel = _panels[_panels.IndexOf(_currentPanel) + 1];
                MoveTo(nextPanel);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            var isFirst = _panels.IndexOf(_currentPanel) == 0;

            if (!isFirst)
            {
                var previousPanel = _panels[_panels.IndexOf(_currentPanel) - 1];
                MoveTo(previousPanel);
            }
        }
         
        #endregion
   }
}
