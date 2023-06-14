namespace DWOS.Server.Admin.Tasks
{
    partial class ServerConfiguration
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Stop the DWOS Server service.", Infragistics.Win.ToolTipImage.Default, "Stop Service", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Start the DWOS Server service.", Infragistics.Win.ToolTipImage.Default, "Start Service", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerConfiguration));
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnStop = new Infragistics.Win.Misc.UltraButton();
            this.btnStart = new Infragistics.Win.Misc.UltraButton();
            this.txtState = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraPictureBox2 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.serviceController = new System.ServiceProcess.ServiceController();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.btnClose = new Infragistics.Win.Misc.UltraButton();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtRestartWaitTime = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.chkRestartService = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRestartWaitTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRestartService)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Controls.Add(this.btnStop);
            this.ultraGroupBox2.Controls.Add(this.btnStart);
            this.ultraGroupBox2.Controls.Add(this.txtState);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel5);
            this.ultraGroupBox2.Controls.Add(this.ultraPictureBox2);
            this.ultraGroupBox2.Location = new System.Drawing.Point(12, 12);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(375, 85);
            this.ultraGroupBox2.TabIndex = 14;
            this.ultraGroupBox2.Text = "Install";
            // 
            // btnStop
            // 
            appearance1.Image = global::DWOS.Server.Admin.Properties.Resources.Stop;
            this.btnStop.Appearance = appearance1;
            this.btnStop.AutoSize = true;
            this.btnStop.ImageSize = new System.Drawing.Size(24, 24);
            this.btnStop.Location = new System.Drawing.Point(267, 18);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(34, 34);
            this.btnStop.TabIndex = 33;
            ultraToolTipInfo1.ToolTipText = "Stop the DWOS Server service.";
            ultraToolTipInfo1.ToolTipTitle = "Stop Service";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnStop, ultraToolTipInfo1);
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            appearance2.Image = global::DWOS.Server.Admin.Properties.Resources.Play;
            this.btnStart.Appearance = appearance2;
            this.btnStart.AutoSize = true;
            this.btnStart.ImageSize = new System.Drawing.Size(24, 24);
            this.btnStart.Location = new System.Drawing.Point(227, 18);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(34, 34);
            this.btnStart.TabIndex = 32;
            ultraToolTipInfo2.ToolTipText = "Start the DWOS Server service.";
            ultraToolTipInfo2.ToolTipTitle = "Start Service";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnStart, ultraToolTipInfo2);
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // txtState
            // 
            this.txtState.Location = new System.Drawing.Point(116, 28);
            this.txtState.Name = "txtState";
            this.txtState.Size = new System.Drawing.Size(94, 15);
            this.txtState.TabIndex = 31;
            this.txtState.Text = "------------";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(70, 28);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(40, 15);
            this.ultraLabel5.TabIndex = 30;
            this.ultraLabel5.Text = "State:";
            // 
            // ultraPictureBox2
            // 
            this.ultraPictureBox2.BorderShadowColor = System.Drawing.Color.Empty;
            this.ultraPictureBox2.Image = ((object)(resources.GetObject("ultraPictureBox2.Image")));
            this.ultraPictureBox2.Location = new System.Drawing.Point(8, 20);
            this.ultraPictureBox2.Name = "ultraPictureBox2";
            this.ultraPictureBox2.Size = new System.Drawing.Size(56, 47);
            this.ultraPictureBox2.TabIndex = 29;
            // 
            // serviceController
            // 
            this.serviceController.ServiceName = "DWOSServer";
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 2500;
            this.timerUpdate.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(312, 195);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 15;
            this.btnClose.Text = "Close";
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.txtRestartWaitTime);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Controls.Add(this.chkRestartService);
            this.ultraGroupBox1.Location = new System.Drawing.Point(20, 103);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(367, 80);
            this.ultraGroupBox1.TabIndex = 16;
            this.ultraGroupBox1.Text = "Recovery";
            // 
            // txtRestartWaitTime
            // 
            this.txtRestartWaitTime.Location = new System.Drawing.Point(195, 43);
            this.txtRestartWaitTime.MaskInput = "nn";
            this.txtRestartWaitTime.MaxValue = 60;
            this.txtRestartWaitTime.MinValue = 0;
            this.txtRestartWaitTime.Name = "txtRestartWaitTime";
            this.txtRestartWaitTime.Size = new System.Drawing.Size(98, 22);
            this.txtRestartWaitTime.TabIndex = 2;
            this.txtRestartWaitTime.ValueChanged += new System.EventHandler(this.txtRestartWaitTime_ValueChanged);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(7, 47);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(182, 15);
            this.ultraLabel1.TabIndex = 1;
            this.ultraLabel1.Text = "Restart wait time (in minutes):";
            // 
            // chkRestartService
            // 
            this.chkRestartService.AutoSize = true;
            this.chkRestartService.Location = new System.Drawing.Point(6, 20);
            this.chkRestartService.Name = "chkRestartService";
            this.chkRestartService.Size = new System.Drawing.Size(156, 18);
            this.chkRestartService.TabIndex = 0;
            this.chkRestartService.Text = "Restart service on error";
            this.chkRestartService.CheckedChanged += new System.EventHandler(this.chkRestartService_CheckedChanged);
            // 
            // ServerConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 230);
            this.Controls.Add(this.ultraGroupBox1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.ultraGroupBox2);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ServerConfiguration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Server Configuration";
            this.Load += new System.EventHandler(this.ServerConfiguration_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRestartWaitTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRestartService)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private Infragistics.Win.Misc.UltraButton btnStart;
        private Infragistics.Win.Misc.UltraLabel txtState;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox2;
        private Infragistics.Win.Misc.UltraButton btnStop;
        private System.ServiceProcess.ServiceController serviceController;
        private System.Windows.Forms.Timer timerUpdate;
        private Infragistics.Win.Misc.UltraButton btnClose;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkRestartService;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor txtRestartWaitTime;

    }
}