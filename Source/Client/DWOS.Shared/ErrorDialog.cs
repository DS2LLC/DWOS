using System;
using System.Windows.Forms;
using NLog;

namespace DWOS.Shared
{
    internal partial class ErrorDialog: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        private Exception CurrentException { get; set; }

        #endregion

        #region Methods

        public ErrorDialog(Exception exc)
        {
            this.InitializeComponent();
            this.CurrentException = exc;
        }

        #endregion

        #region Events

        private void ErrorDialog_Load(object sender, EventArgs e)
        {
            try
            {
                this.txtErrorMsg.Text = this.CurrentException.Message + Environment.NewLine + Environment.NewLine + this.CurrentException;
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error setting error message.");
            }
        }

        #endregion
    }

}