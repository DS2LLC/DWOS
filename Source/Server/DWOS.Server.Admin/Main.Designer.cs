namespace DWOS.Server.Admin
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupWizardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.upgradWizardMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backupDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLogsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openServerLogsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webStatus1 = new DWOS.Server.Admin.StatusPanels.WebStatus();
            this.serverStatus1 = new DWOS.Server.Admin.StatusPanels.ServerStatus();
            this.databaseStatus1 = new DWOS.Server.Admin.StatusPanels.DatabaseStatus();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(437, 24);
            this.menuStrip1.TabIndex = 14;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setupWizardToolStripMenuItem,
            this.upgradWizardMenuItem,
            this.backupDatabaseToolStripMenuItem,
            this.toolStripSeparator1,
            this.checkForUpdatesToolStripMenuItem,
            this.configureUpdatesToolStripMenuItem,
            this.toolStripSeparator2,
            this.settingsToolStripMenuItem,
            this.openLogsToolStripMenuItem,
            this.openServerLogsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // setupWizardToolStripMenuItem
            // 
            this.setupWizardToolStripMenuItem.Name = "setupWizardToolStripMenuItem";
            this.setupWizardToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.setupWizardToolStripMenuItem.Text = "New Install Wizard";
            this.setupWizardToolStripMenuItem.Click += new System.EventHandler(this.setupWizardToolStripMenuItem_Click);
            // 
            // upgradWizardMenuItem
            // 
            this.upgradWizardMenuItem.Name = "upgradWizardMenuItem";
            this.upgradWizardMenuItem.Size = new System.Drawing.Size(209, 22);
            this.upgradWizardMenuItem.Text = "Database Upgrade Wizard";
            this.upgradWizardMenuItem.Click += new System.EventHandler(this.upgradWizardMenuItem_Click);
            // 
            // backupDatabaseToolStripMenuItem
            // 
            this.backupDatabaseToolStripMenuItem.Name = "backupDatabaseToolStripMenuItem";
            this.backupDatabaseToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.backupDatabaseToolStripMenuItem.Text = "Backup Database";
            this.backupDatabaseToolStripMenuItem.Click += new System.EventHandler(this.backupDatabaseToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(206, 6);
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.checkForUpdatesToolStripMenuItem.Text = "Check For Updates";
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesToolStripMenuItem_Click);
            // 
            // configureUpdatesToolStripMenuItem
            // 
            this.configureUpdatesToolStripMenuItem.Name = "configureUpdatesToolStripMenuItem";
            this.configureUpdatesToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.configureUpdatesToolStripMenuItem.Text = "Configure Updates";
            this.configureUpdatesToolStripMenuItem.Click += new System.EventHandler(this.configureUpdatesToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(206, 6);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // openLogsToolStripMenuItem
            // 
            this.openLogsToolStripMenuItem.Name = "openLogsToolStripMenuItem";
            this.openLogsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.openLogsToolStripMenuItem.Text = "Open Admin Logs...";
            this.openLogsToolStripMenuItem.Click += new System.EventHandler(this.openLogsToolStripMenuItem_Click);
            // 
            // openServerLogsToolStripMenuItem
            // 
            this.openServerLogsToolStripMenuItem.Name = "openServerLogsToolStripMenuItem";
            this.openServerLogsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.openServerLogsToolStripMenuItem.Text = "Open Server Logs...";
            this.openServerLogsToolStripMenuItem.Click += new System.EventHandler(this.openServerLogsToolStripMenuItem_Click);
            // 
            // webStatus1
            // 
            this.webStatus1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.webStatus1.Location = new System.Drawing.Point(9, 408);
            this.webStatus1.Name = "webStatus1";
            this.webStatus1.Size = new System.Drawing.Size(416, 142);
            this.webStatus1.TabIndex = 15;
            // 
            // serverStatus1
            // 
            this.serverStatus1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serverStatus1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverStatus1.Location = new System.Drawing.Point(9, 212);
            this.serverStatus1.Name = "serverStatus1";
            this.serverStatus1.Size = new System.Drawing.Size(416, 190);
            this.serverStatus1.TabIndex = 12;
            // 
            // databaseStatus1
            // 
            this.databaseStatus1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.databaseStatus1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.databaseStatus1.Location = new System.Drawing.Point(9, 26);
            this.databaseStatus1.Name = "databaseStatus1";
            this.databaseStatus1.Size = new System.Drawing.Size(416, 180);
            this.databaseStatus1.TabIndex = 11;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 556);
            this.Controls.Add(this.webStatus1);
            this.Controls.Add(this.serverStatus1);
            this.Controls.Add(this.databaseStatus1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DWOS Administration Utility";
            this.Load += new System.EventHandler(this.Main_Load);
            this.Shown += new System.EventHandler(this.Main_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
        private StatusPanels.DatabaseStatus databaseStatus1;
        private StatusPanels.ServerStatus serverStatus1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupWizardToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configureUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem upgradWizardMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLogsToolStripMenuItem;
        private StatusPanels.WebStatus webStatus1;
        private System.Windows.Forms.ToolStripMenuItem openServerLogsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backupDatabaseToolStripMenuItem;
    }
}

