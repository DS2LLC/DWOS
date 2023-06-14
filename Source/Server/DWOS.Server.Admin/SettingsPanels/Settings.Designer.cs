namespace DWOS.Server.Admin.SettingsPanels
{
    partial class Settings
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
            Infragistics.Win.UltraWinTree.UltraTreeNode ultraTreeNode2 = new Infragistics.Win.UltraWinTree.UltraTreeNode();
            Infragistics.Win.UltraWinTree.UltraTreeNode ultraTreeNode11 = new Infragistics.Win.UltraWinTree.UltraTreeNode();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            Infragistics.Win.UltraWinTree.UltraTreeNode ultraTreeNode1 = new Infragistics.Win.UltraWinTree.UltraTreeNode();
            Infragistics.Win.UltraWinTree.UltraTreeNode ultraTreeNode3 = new Infragistics.Win.UltraWinTree.UltraTreeNode();
            Infragistics.Win.UltraWinTree.UltraTreeNode ultraTreeNode4 = new Infragistics.Win.UltraWinTree.UltraTreeNode();
            Infragistics.Win.UltraWinTree.UltraTreeNode ultraTreeNode5 = new Infragistics.Win.UltraWinTree.UltraTreeNode();
            this.ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvwSettings = new Infragistics.Win.UltraWinTree.UltraTree();
            this.ultraPanel2 = new Infragistics.Win.Misc.UltraPanel();
            this.pnlAccounting = new DWOS.Server.Admin.SettingsPanels.SettingsAccounting();
            this.pnlBackup = new DWOS.Server.Admin.SettingsPanels.SettingsBackup();
            this.pnlSettings = new DWOS.Server.Admin.SettingsPanels.SettingsPlugins();
            this.pnlSettingsWebPortalInfo = new DWOS.Server.Admin.SettingsPanels.SettingsWebPortalInfo();
            this.pnlSettingsAuthenticationInfo = new DWOS.Server.Admin.SettingsPanels.SettingsAuthenticationInfo();
            this.ultraPanel1.ClientArea.SuspendLayout();
            this.ultraPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tvwSettings)).BeginInit();
            this.ultraPanel2.ClientArea.SuspendLayout();
            this.ultraPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraPanel1
            // 
            // 
            // ultraPanel1.ClientArea
            // 
            this.ultraPanel1.ClientArea.Controls.Add(this.btnOK);
            this.ultraPanel1.ClientArea.Controls.Add(this.btnCancel);
            this.ultraPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ultraPanel1.Location = new System.Drawing.Point(0, 508);
            this.ultraPanel1.Name = "ultraPanel1";
            this.ultraPanel1.Size = new System.Drawing.Size(670, 39);
            this.ultraPanel1.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(504, 8);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(586, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvwSettings);
            this.splitContainer1.Panel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(5);
            this.splitContainer1.Panel1MinSize = 150;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ultraPanel2);
            this.splitContainer1.Panel2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(5);
            this.splitContainer1.Size = new System.Drawing.Size(670, 508);
            this.splitContainer1.SplitterDistance = 223;
            this.splitContainer1.TabIndex = 2;
            // 
            // tvwSettings
            // 
            this.tvwSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwSettings.HideSelection = false;
            this.tvwSettings.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tvwSettings.Location = new System.Drawing.Point(5, 5);
            this.tvwSettings.Name = "tvwSettings";
            this.tvwSettings.NodeConnectorColor = System.Drawing.SystemColors.ControlDark;
            ultraTreeNode2.Key = "AppSettings";
            ultraTreeNode11.LeftImages.Add(((object)(resources.GetObject("ultraTreeNode11.LeftImages"))));
            ultraTreeNode11.Tag = "Authentication";
            ultraTreeNode11.Text = "General";
            ultraTreeNode1.LeftImages.Add(((object)(resources.GetObject("ultraTreeNode1.LeftImages"))));
            ultraTreeNode1.Tag = "Web";
            ultraTreeNode1.Text = "Web Portal";
            ultraTreeNode3.LeftImages.Add(((object)(resources.GetObject("ultraTreeNode3.LeftImages"))));
            ultraTreeNode3.Tag = "PlugIn";
            ultraTreeNode3.Text = "Plug Ins";
            ultraTreeNode4.LeftImages.Add(((object)(resources.GetObject("ultraTreeNode4.LeftImages"))));
            ultraTreeNode4.Tag = "Backup";
            ultraTreeNode4.Text = "Backup";
            ultraTreeNode5.LeftImages.Add(((object)(resources.GetObject("ultraTreeNode5.LeftImages"))));
            ultraTreeNode5.Tag = "Accounting";
            ultraTreeNode5.Text = "Accounting";
            ultraTreeNode2.Nodes.AddRange(new Infragistics.Win.UltraWinTree.UltraTreeNode[] {
            ultraTreeNode11,
            ultraTreeNode1,
            ultraTreeNode3,
            ultraTreeNode4,
            ultraTreeNode5});
            ultraTreeNode2.Text = "Application Settings";
            this.tvwSettings.Nodes.AddRange(new Infragistics.Win.UltraWinTree.UltraTreeNode[] {
            ultraTreeNode2});
            this.tvwSettings.Size = new System.Drawing.Size(213, 498);
            this.tvwSettings.TabIndex = 0;
            this.tvwSettings.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.tvwSettings_AfterSelect);
            this.tvwSettings.BeforeSelect += new Infragistics.Win.UltraWinTree.BeforeNodeSelectEventHandler(this.tvwSettings_BeforeSelect);
            // 
            // ultraPanel2
            // 
            // 
            // ultraPanel2.ClientArea
            // 
            this.ultraPanel2.ClientArea.Controls.Add(this.pnlAccounting);
            this.ultraPanel2.ClientArea.Controls.Add(this.pnlBackup);
            this.ultraPanel2.ClientArea.Controls.Add(this.pnlSettings);
            this.ultraPanel2.ClientArea.Controls.Add(this.pnlSettingsWebPortalInfo);
            this.ultraPanel2.ClientArea.Controls.Add(this.pnlSettingsAuthenticationInfo);
            this.ultraPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel2.Location = new System.Drawing.Point(5, 5);
            this.ultraPanel2.Name = "ultraPanel2";
            this.ultraPanel2.Size = new System.Drawing.Size(433, 498);
            this.ultraPanel2.TabIndex = 0;
            // 
            // pnlAccounting
            // 
            this.pnlAccounting.Location = new System.Drawing.Point(20, 151);
            this.pnlAccounting.Name = "pnlAccounting";
            this.pnlAccounting.Size = new System.Drawing.Size(395, 168);
            this.pnlAccounting.TabIndex = 42;
            // 
            // pnlBackup
            // 
            this.pnlBackup.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlBackup.Location = new System.Drawing.Point(73, 55);
            this.pnlBackup.Name = "pnlBackup";
            this.pnlBackup.Size = new System.Drawing.Size(342, 378);
            this.pnlBackup.TabIndex = 41;
            // 
            // pnlSettings
            // 
            this.pnlSettings.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlSettings.Location = new System.Drawing.Point(104, 21);
            this.pnlSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(670, 298);
            this.pnlSettings.TabIndex = 40;
            // 
            // pnlSettingsWebPortalInfo
            // 
            this.pnlSettingsWebPortalInfo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlSettingsWebPortalInfo.Location = new System.Drawing.Point(64, 200);
            this.pnlSettingsWebPortalInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlSettingsWebPortalInfo.Name = "pnlSettingsWebPortalInfo";
            this.pnlSettingsWebPortalInfo.Size = new System.Drawing.Size(323, 277);
            this.pnlSettingsWebPortalInfo.TabIndex = 39;
            // 
            // pnlSettingsAuthenticationInfo
            // 
            this.pnlSettingsAuthenticationInfo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlSettingsAuthenticationInfo.Location = new System.Drawing.Point(64, 55);
            this.pnlSettingsAuthenticationInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlSettingsAuthenticationInfo.Name = "pnlSettingsAuthenticationInfo";
            this.pnlSettingsAuthenticationInfo.Size = new System.Drawing.Size(323, 101);
            this.pnlSettingsAuthenticationInfo.TabIndex = 38;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 547);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.ultraPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Settings_FormClosing);
            this.Load += new System.EventHandler(this.Settings_Load);
            this.ultraPanel1.ClientArea.ResumeLayout(false);
            this.ultraPanel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tvwSettings)).EndInit();
            this.ultraPanel2.ClientArea.ResumeLayout(false);
            this.ultraPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Infragistics.Win.UltraWinTree.UltraTree tvwSettings;
        private SettingsPanels.SettingsAuthenticationInfo pnlSettingsAuthenticationInfo;
        private Infragistics.Win.Misc.UltraPanel ultraPanel2;
        private SettingsPanels.SettingsWebPortalInfo pnlSettingsWebPortalInfo;
        private SettingsPlugins pnlSettings;
        private SettingsBackup pnlBackup;
        private SettingsAccounting pnlAccounting;
    }
}