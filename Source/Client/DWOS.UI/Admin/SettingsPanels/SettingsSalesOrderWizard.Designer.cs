namespace DWOS.UI.Admin.SettingsPanels
{
    partial class SettingsSalesOrderWizard
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Enable Sales Order Wizard", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Allow Work Orders with Different Processes", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsSalesOrderWizard));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Allow users to create parts.", Infragistics.Win.DefaultableBoolean.Default);
            this.grpMain = new Infragistics.Win.Misc.UltraGroupBox();
            this.chkEnableWizard = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkAllowDifferentProcesses = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkRedline = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grpMain)).BeginInit();
            this.grpMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkEnableWizard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAllowDifferentProcesses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRedline)).BeginInit();
            this.SuspendLayout();
            // 
            // grpMain
            // 
            this.grpMain.Controls.Add(this.chkEnableWizard);
            this.grpMain.Controls.Add(this.chkAllowDifferentProcesses);
            this.grpMain.Controls.Add(this.chkRedline);
            this.grpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpMain.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.grpMain.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.grpMain.Location = new System.Drawing.Point(0, 0);
            this.grpMain.Name = "grpMain";
            this.grpMain.Size = new System.Drawing.Size(343, 253);
            this.grpMain.TabIndex = 0;
            this.grpMain.Text = "Sales Order Wizard";
            // 
            // chkEnableWizard
            // 
            this.chkEnableWizard.AutoSize = true;
            this.chkEnableWizard.Location = new System.Drawing.Point(6, 32);
            this.chkEnableWizard.Name = "chkEnableWizard";
            this.chkEnableWizard.Size = new System.Drawing.Size(173, 18);
            this.chkEnableWizard.TabIndex = 0;
            this.chkEnableWizard.Text = "Enable Sales Order Wizard";
            ultraToolTipInfo1.ToolTipTextFormatted = "If <b>checked</b>, enables the Sales Order Wizard for users with the OrderEntry.E" +
    "dit permission.<br/>If <b>unchecked</b> (default), disables the Sales Order Wiza" +
    "rd for all users.";
            ultraToolTipInfo1.ToolTipTitle = "Enable Sales Order Wizard";
            this.ultraToolTipManager.SetUltraToolTip(this.chkEnableWizard, ultraToolTipInfo1);
            // 
            // chkAllowDifferentProcesses
            // 
            this.chkAllowDifferentProcesses.AutoSize = true;
            this.chkAllowDifferentProcesses.Location = new System.Drawing.Point(6, 80);
            this.chkAllowDifferentProcesses.Name = "chkAllowDifferentProcesses";
            this.chkAllowDifferentProcesses.Size = new System.Drawing.Size(267, 18);
            this.chkAllowDifferentProcesses.TabIndex = 2;
            this.chkAllowDifferentProcesses.Text = "Allow Work Orders with different processes";
            ultraToolTipInfo2.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo2.ToolTipTextFormatted");
            ultraToolTipInfo2.ToolTipTitle = "Allow Work Orders with Different Processes";
            this.ultraToolTipManager.SetUltraToolTip(this.chkAllowDifferentProcesses, ultraToolTipInfo2);
            // 
            // chkRedline
            // 
            this.chkRedline.AutoSize = true;
            this.chkRedline.Location = new System.Drawing.Point(6, 56);
            this.chkRedline.Name = "chkRedline";
            this.chkRedline.Size = new System.Drawing.Size(178, 18);
            this.chkRedline.TabIndex = 1;
            this.chkRedline.Text = "Allow users to create parts.";
            ultraToolTipInfo3.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo3.ToolTipTextFormatted");
            ultraToolTipInfo3.ToolTipTitle = "Allow users to create parts.";
            this.ultraToolTipManager.SetUltraToolTip(this.chkRedline, ultraToolTipInfo3);
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // SettingsSalesOrderWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpMain);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(340, 250);
            this.Name = "SettingsSalesOrderWizard";
            this.Size = new System.Drawing.Size(343, 253);
            ((System.ComponentModel.ISupportInitialize)(this.grpMain)).EndInit();
            this.grpMain.ResumeLayout(false);
            this.grpMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkEnableWizard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAllowDifferentProcesses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRedline)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox grpMain;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkRedline;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkAllowDifferentProcesses;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkEnableWizard;
    }
}
