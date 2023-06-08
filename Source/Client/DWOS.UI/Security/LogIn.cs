using System;
using System.Windows.Forms;

namespace DWOS.UI.Utilities
{
    public partial class LogIn: Form
    {
        public LogIn()
        {
            this.InitializeComponent();
        }

        public string UserPin
        {
            get { return this.txtPin.Text; }
        }

        private void LogIn_Shown(object sender, EventArgs e)
        {
            this.txtPin.Focus();
        }
    }
}