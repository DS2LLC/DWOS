using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Infragistics.UltraChart.Core;
using NLog;

namespace DWOS.Server.Admin.StatusPanels
{
    public partial class IpAddressesDialog : Form
    {
        public IpAddressesDialog()
        {
            InitializeComponent();
        }

        public void LoadIpAddresses(IEnumerable<string> ipAddresses)
        {
            lstIpAddresses.Items.Clear();

            foreach (var ipAddress in ipAddresses)
            {
                lstIpAddresses.Items.Add(ipAddress);
            }
        }

        private void CopySelectionToClipboard()
        {
            if (lstIpAddresses.SelectedItems.Count == 0)
            {
                return;
            }

            var ipAddresses = string.Join(Environment.NewLine, lstIpAddresses.SelectedItems.OfType<object>().Select(ip => ip.ToString()));
            Clipboard.SetText(ipAddresses);
        }

        private void btnCopy_Click(object sender, System.EventArgs e)
        {
            try
            {
                CopySelectionToClipboard();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error copying selected IP addresses to clipboard.");
            }
        }

        private void lstIpAddresses_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    CopySelectionToClipboard();
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error copying selected IP addresses to clipboard.");
            }
        }

        private void lstIpAddresses_SelectedValueChanged(object sender, EventArgs e)
        {
            btnCopy.Enabled = lstIpAddresses.SelectedItems.Count > 0;
        }
    }
}
