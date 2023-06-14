namespace DWOS.Server.Admin.Wizards.UpgradeWizard
{
    partial class StartService
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.txtState = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.btnStop = new Infragistics.Win.Misc.UltraButton();
            this.btnStart = new Infragistics.Win.Misc.UltraButton();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.lblInfoMessage = new Infragistics.Win.Misc.UltraLabel();
            this.SuspendLayout();
            // 
            // txtState
            // 
            this.txtState.Location = new System.Drawing.Point(60, 51);
            this.txtState.Name = "txtState";
            this.txtState.Size = new System.Drawing.Size(94, 15);
            this.txtState.TabIndex = 33;
            this.txtState.Text = "------------";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(14, 51);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(40, 15);
            this.ultraLabel5.TabIndex = 32;
            this.ultraLabel5.Text = "State:";
            // 
            // btnStop
            // 
            appearance1.Image = global::DWOS.Server.Admin.Properties.Resources.Stop;
            this.btnStop.Appearance = appearance1;
            this.btnStop.AutoSize = true;
            this.btnStop.ImageSize = new System.Drawing.Size(24, 24);
            this.btnStop.Location = new System.Drawing.Point(100, 74);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(34, 34);
            this.btnStop.TabIndex = 35;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            appearance2.Image = global::DWOS.Server.Admin.Properties.Resources.Play;
            this.btnStart.Appearance = appearance2;
            this.btnStart.AutoSize = true;
            this.btnStart.ImageSize = new System.Drawing.Size(24, 24);
            this.btnStart.Location = new System.Drawing.Point(60, 74);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(34, 34);
            this.btnStart.TabIndex = 34;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 2500;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // lblInfoMessage
            // 
            this.lblInfoMessage.AutoSize = true;
            this.lblInfoMessage.Location = new System.Drawing.Point(14, 14);
            this.lblInfoMessage.Name = "lblInfoMessage";
            this.lblInfoMessage.Size = new System.Drawing.Size(253, 15);
            this.lblInfoMessage.TabIndex = 36;
            this.lblInfoMessage.Text = "Service must be stopped before continuing.";
            // 
            // StartService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblInfoMessage);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.txtState);
            this.Controls.Add(this.ultraLabel5);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "StartService";
            this.Size = new System.Drawing.Size(571, 191);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel txtState;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.Misc.UltraButton btnStop;
        private Infragistics.Win.Misc.UltraButton btnStart;
        private System.Windows.Forms.Timer timerUpdate;
        private Infragistics.Win.Misc.UltraLabel lblInfoMessage;
    }
}
