namespace DWOS.Server.Admin.StatusPanels
{
    partial class IpAddressesDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IpAddressesDialog));
            this.lstIpAddresses = new System.Windows.Forms.ListBox();
            this.btnCopy = new Infragistics.Win.Misc.UltraButton();
            this.SuspendLayout();
            // 
            // lstIpAddresses
            // 
            this.lstIpAddresses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstIpAddresses.FormattingEnabled = true;
            this.lstIpAddresses.Location = new System.Drawing.Point(12, 12);
            this.lstIpAddresses.Name = "lstIpAddresses";
            this.lstIpAddresses.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstIpAddresses.Size = new System.Drawing.Size(260, 199);
            this.lstIpAddresses.TabIndex = 0;
            this.lstIpAddresses.SelectedValueChanged += new System.EventHandler(this.lstIpAddresses_SelectedValueChanged);
            this.lstIpAddresses.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstIpAddresses_KeyDown);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.Enabled = false;
            this.btnCopy.Location = new System.Drawing.Point(12, 226);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(260, 23);
            this.btnCopy.TabIndex = 1;
            this.btnCopy.Text = "Copy to clipboard";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // IpAddressesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.lstIpAddresses);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "IpAddressesDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IP Addresses";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstIpAddresses;
        private Infragistics.Win.Misc.UltraButton btnCopy;
    }
}