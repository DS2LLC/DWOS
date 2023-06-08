namespace DWOS.UI.Utilities
{
    partial class ServerConnection
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The port to connect to the server on.", Infragistics.Win.ToolTipImage.Default, "Server Port", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The name of the server.", Infragistics.Win.ToolTipImage.Default, "Server Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("If you want to connect to the demo server, then click this button to load the dem" +
        "o server settings.", Infragistics.Win.ToolTipImage.Default, "Demo", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Server Name", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerConnection));
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtPort = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lblDemo = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.cboServerName = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            ((System.ComponentModel.ISupportInitialize)(this.txtPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboServerName)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(93, 94);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(12, 37);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel1.TabIndex = 2;
            this.ultraLabel1.Text = "Name:";
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(12, 66);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(32, 15);
            this.ultraLabel2.TabIndex = 5;
            this.ultraLabel2.Text = "Port:";
            ultraToolTipInfo3.ToolTipText = "The port to connect to the server on.";
            ultraToolTipInfo3.ToolTipTitle = "Server Port";
            this.ultraToolTipManager1.SetUltraToolTip(this.ultraLabel2, ultraToolTipInfo3);
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(65, 62);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(171, 22);
            this.txtPort.TabIndex = 1;
            ultraToolTipInfo4.ToolTipText = "The name of the server.";
            ultraToolTipInfo4.ToolTipTitle = "Server Name";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtPort, ultraToolTipInfo4);
            // 
            // lblDemo
            // 
            appearance1.Cursor = System.Windows.Forms.Cursors.Hand;
            appearance1.FontData.UnderlineAsString = "True";
            appearance1.ForeColor = System.Drawing.Color.Blue;
            this.lblDemo.Appearance = appearance1;
            this.lblDemo.AutoSize = true;
            this.lblDemo.Location = new System.Drawing.Point(12, 99);
            this.lblDemo.Name = "lblDemo";
            this.lblDemo.Size = new System.Drawing.Size(38, 15);
            this.lblDemo.TabIndex = 7;
            this.lblDemo.Text = "Demo";
            ultraToolTipInfo2.ToolTipText = "If you want to connect to the demo server, then click this button to load the dem" +
    "o server settings.";
            ultraToolTipInfo2.ToolTipTitle = "Demo";
            this.ultraToolTipManager1.SetUltraToolTip(this.lblDemo, ultraToolTipInfo2);
            this.lblDemo.UseAppStyling = false;
            this.lblDemo.Click += new System.EventHandler(this.lblDemo_Click);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(12, 12);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(118, 15);
            this.ultraLabel3.TabIndex = 6;
            this.ultraLabel3.Text = "Server Information:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(174, 94);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // cboServerName
            // 
            this.cboServerName.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            this.cboServerName.DropDownListWidth = -1;
            this.cboServerName.Location = new System.Drawing.Point(65, 33);
            this.cboServerName.Name = "cboServerName";
            this.cboServerName.Size = new System.Drawing.Size(171, 22);
            this.cboServerName.TabIndex = 25;
            ultraToolTipInfo1.ToolTipTextFormatted = "Click the drop down to search for local servers.";
            ultraToolTipInfo1.ToolTipTitle = "Server Name";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboServerName, ultraToolTipInfo1);
            this.cboServerName.SelectionChanged += new System.EventHandler(this.cboServerName_SelectionChanged);
            this.cboServerName.BeforeDropDown += new System.ComponentModel.CancelEventHandler(this.cboServerName_BeforeDropDown);
            // 
            // ServerConnection
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 129);
            this.ControlBox = false;
            this.Controls.Add(this.cboServerName);
            this.Controls.Add(this.lblDemo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServerConnection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Server Connection";
            this.Load += new System.EventHandler(this.ServerConnection_Load);
            this.Shown += new System.EventHandler(this.ServerConnection_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.txtPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboServerName)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPort;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private Infragistics.Win.Misc.UltraLabel lblDemo;
        public Infragistics.Win.UltraWinEditors.UltraComboEditor cboServerName;
    }
}