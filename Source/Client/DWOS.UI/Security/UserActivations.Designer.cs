namespace DWOS.UI.Security
{
    partial class UserActivations
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
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn1 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("SubItemColumn 0");
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn2 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("Logged In");
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn3 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("Last Activity");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserActivations));
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.FormLabel = new Infragistics.Win.Misc.UltraLabel();
            this.lvwUsers = new Infragistics.Win.UltraWinListView.UltraListView();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.lblLicenseInfo = new Infragistics.Win.Misc.UltraLabel();
            this.btnRefesh = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.lvwUsers)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(384, 203);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Close";
            // 
            // FormLabel
            // 
            this.FormLabel.AutoSize = true;
            this.FormLabel.Location = new System.Drawing.Point(12, 12);
            this.FormLabel.Name = "FormLabel";
            this.FormLabel.Size = new System.Drawing.Size(80, 15);
            this.FormLabel.TabIndex = 32;
            this.FormLabel.Text = "Active Users:";
            // 
            // lvwUsers
            // 
            this.lvwUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwUsers.ItemSettings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            this.lvwUsers.ItemSettings.HotTracking = true;
            this.lvwUsers.Location = new System.Drawing.Point(22, 33);
            this.lvwUsers.MainColumn.Text = "User Name";
            this.lvwUsers.MainColumn.VisiblePositionInDetailsView = 0;
            this.lvwUsers.MainColumn.Width = 100;
            this.lvwUsers.Name = "lvwUsers";
            this.lvwUsers.Size = new System.Drawing.Size(453, 139);
            ultraListViewSubItemColumn1.Key = "SubItemColumn 0";
            ultraListViewSubItemColumn1.Text = "Computer Name";
            ultraListViewSubItemColumn1.VisiblePositionInDetailsView = 3;
            ultraListViewSubItemColumn2.Key = "Logged In";
            ultraListViewSubItemColumn2.VisiblePositionInDetailsView = 1;
            ultraListViewSubItemColumn3.Key = "Last Activity";
            ultraListViewSubItemColumn3.VisiblePositionInDetailsView = 2;
            this.lvwUsers.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn1,
            ultraListViewSubItemColumn2,
            ultraListViewSubItemColumn3});
            this.lvwUsers.TabIndex = 33;
            this.lvwUsers.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.lvwUsers.ViewSettingsDetails.ColumnAutoSizeMode = ((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header | Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems)));
            this.lvwUsers.ViewSettingsDetails.FullRowSelect = true;
            this.lvwUsers.ViewSettingsDetails.ImageSize = new System.Drawing.Size(16, 16);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(22, 178);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(90, 15);
            this.ultraLabel1.TabIndex = 34;
            this.ultraLabel1.Text = "License Usage:";
            // 
            // lblLicenseInfo
            // 
            this.lblLicenseInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLicenseInfo.AutoSize = true;
            this.lblLicenseInfo.Location = new System.Drawing.Point(145, 178);
            this.lblLicenseInfo.Name = "lblLicenseInfo";
            this.lblLicenseInfo.Size = new System.Drawing.Size(37, 15);
            this.lblLicenseInfo.TabIndex = 35;
            this.lblLicenseInfo.Text = "0 of 0";
            // 
            // btnRefesh
            // 
            this.btnRefesh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance1.Image = global::DWOS.UI.Properties.Resources.Reload_32;
            this.btnRefesh.Appearance = appearance1;
            this.btnRefesh.AutoSize = true;
            this.btnRefesh.Location = new System.Drawing.Point(12, 200);
            this.btnRefesh.Name = "btnRefesh";
            this.btnRefesh.Size = new System.Drawing.Size(26, 26);
            this.btnRefesh.TabIndex = 36;
            this.btnRefesh.Click += new System.EventHandler(this.btnRefesh_Click);
            // 
            // UserActivations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 238);
            this.Controls.Add(this.btnRefesh);
            this.Controls.Add(this.lblLicenseInfo);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.lvwUsers);
            this.Controls.Add(this.FormLabel);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UserActivations";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Logged In Users";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.UserActivations_Load);
            ((System.ComponentModel.ISupportInitialize)(this.lvwUsers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnCancel;
        public Infragistics.Win.Misc.UltraLabel FormLabel;
        private Infragistics.Win.UltraWinListView.UltraListView lvwUsers;
        public Infragistics.Win.Misc.UltraLabel ultraLabel1;
        public Infragistics.Win.Misc.UltraLabel lblLicenseInfo;
        private Infragistics.Win.Misc.UltraButton btnRefesh;
    }
}