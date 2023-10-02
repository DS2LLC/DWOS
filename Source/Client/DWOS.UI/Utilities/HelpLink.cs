using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using NLog;

namespace DWOS.UI.Utilities
{
    public partial class HelpLink : UserControl
    {
        #region Fields

        public static readonly string HELP_URL = @DWOS.Data.Properties.Settings.Default.DWOSHelp.ToString().FormatWith(About.ApplicationVersionMajorMinor);

        #endregion

        #region Properties

        [Browsable(true)]
        [Category("Help")]
        [Description("The name of the help page, i.e. 'getting_started.htm'.")]
        public string HelpPage { get; set; }

        #endregion

        #region Methods
        
        public HelpLink()
        {
            InitializeComponent();
        }
        
        private void ShowHelp()
        {
            try
            {
                Process.Start(HELP_URL + this.HelpPage);
            }
            catch(Exception exc)
            {
                MessageBoxUtilities.ShowMessageBoxWarn("Default web browser is not found or is unable to start.", "Unable to Start");
                LogManager.GetCurrentClassLogger().Debug(exc, "Error starting process for " + HELP_URL + this.HelpPage);
            }
        }

        #endregion

        #region Events
        
        private void lblHelp_Click(object sender, EventArgs e)
        {
            ShowHelp();
        }

        private void HelpLink_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F1)
            {
                e.Handled = true;
                ShowHelp();
            }
        }

        private void HelpLink_Load(object sender, EventArgs e)
        {
            var form = this.FindForm();
            if (form != null)
            {
                form.KeyPreview = true;
                form.KeyUp += HelpLink_KeyUp;
            }
        }

        #endregion
    }
}