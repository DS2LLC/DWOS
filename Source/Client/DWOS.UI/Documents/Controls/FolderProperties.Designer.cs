namespace DWOS.UI.Documents.Controls
{
    partial class FolderProperties
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
            DisposeMe();

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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn1 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("Type");
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderProperties));
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.txtLocalDirectory = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lbLocalDirectory = new Infragistics.Win.Misc.UltraLabel();
            this.txtFolderName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.btnDestroy = new Infragistics.Win.Misc.UltraButton();
            this.btnRestore = new Infragistics.Win.Misc.UltraButton();
            this.lvwRemovedItems = new Infragistics.Win.UltraWinListView.UltraListView();
            this.ultraTabPageControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.lvwSecurityGroups = new Infragistics.Win.UltraWinListView.UltraListView();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.tabFileProperties = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.helpLink1 = new DWOS.UI.Utilities.HelpLink();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLocalDirectory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFolderName)).BeginInit();
            this.ultraTabPageControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lvwRemovedItems)).BeginInit();
            this.ultraTabPageControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lvwSecurityGroups)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabFileProperties)).BeginInit();
            this.tabFileProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.txtLocalDirectory);
            this.ultraTabPageControl1.Controls.Add(this.lbLocalDirectory);
            this.ultraTabPageControl1.Controls.Add(this.txtFolderName);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel1);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(799, 432);
            // 
            // txtLocalDirectory
            // 
            this.txtLocalDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocalDirectory.Location = new System.Drawing.Point(156, 40);
            this.txtLocalDirectory.Name = "txtLocalDirectory";
            this.txtLocalDirectory.ReadOnly = true;
            this.txtLocalDirectory.Size = new System.Drawing.Size(591, 22);
            this.txtLocalDirectory.TabIndex = 3;
            // 
            // lbLocalDirectory
            // 
            this.lbLocalDirectory.AutoSize = true;
            this.lbLocalDirectory.Location = new System.Drawing.Point(20, 44);
            this.lbLocalDirectory.Name = "lbLocalDirectory";
            this.lbLocalDirectory.Size = new System.Drawing.Size(94, 15);
            this.lbLocalDirectory.TabIndex = 2;
            this.lbLocalDirectory.Text = "Local Directory:";
            // 
            // txtFolderName
            // 
            this.txtFolderName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolderName.Location = new System.Drawing.Point(156, 12);
            this.txtFolderName.Name = "txtFolderName";
            this.txtFolderName.ReadOnly = true;
            this.txtFolderName.Size = new System.Drawing.Size(591, 22);
            this.txtFolderName.TabIndex = 1;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(20, 16);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(44, 15);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Folder:";
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.btnDestroy);
            this.ultraTabPageControl2.Controls.Add(this.btnRestore);
            this.ultraTabPageControl2.Controls.Add(this.lvwRemovedItems);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(799, 432);
            // 
            // btnDestroy
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Delete_32;
            this.btnDestroy.Appearance = appearance1;
            this.btnDestroy.AutoSize = true;
            this.btnDestroy.Enabled = false;
            this.btnDestroy.ImageSize = new System.Drawing.Size(24, 24);
            this.btnDestroy.Location = new System.Drawing.Point(91, 14);
            this.btnDestroy.Name = "btnDestroy";
            this.btnDestroy.Size = new System.Drawing.Size(82, 34);
            this.btnDestroy.TabIndex = 14;
            this.btnDestroy.Text = "Destroy";
            // 
            // btnRestore
            // 
            appearance2.Image = global::DWOS.UI.Properties.Resources.Add_32;
            this.btnRestore.Appearance = appearance2;
            this.btnRestore.AutoSize = true;
            this.btnRestore.Enabled = false;
            this.btnRestore.ImageSize = new System.Drawing.Size(24, 24);
            this.btnRestore.Location = new System.Drawing.Point(3, 14);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(82, 34);
            this.btnRestore.TabIndex = 13;
            this.btnRestore.Text = "Restore";
            // 
            // lvwRemovedItems
            // 
            this.lvwRemovedItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwRemovedItems.ItemSettings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            this.lvwRemovedItems.ItemSettings.HideSelection = false;
            this.lvwRemovedItems.Location = new System.Drawing.Point(3, 54);
            this.lvwRemovedItems.MainColumn.DataType = typeof(string);
            this.lvwRemovedItems.MainColumn.Sorting = Infragistics.Win.UltraWinListView.Sorting.Ascending;
            this.lvwRemovedItems.MainColumn.Text = "Item";
            this.lvwRemovedItems.MainColumn.Width = 50;
            this.lvwRemovedItems.Name = "lvwRemovedItems";
            this.lvwRemovedItems.Size = new System.Drawing.Size(793, 375);
            ultraListViewSubItemColumn1.Key = "Type";
            this.lvwRemovedItems.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn1});
            this.lvwRemovedItems.TabIndex = 12;
            this.lvwRemovedItems.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.lvwRemovedItems.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns;
            this.lvwRemovedItems.ViewSettingsDetails.FullRowSelect = true;
            this.lvwRemovedItems.ViewSettingsDetails.ImageSize = new System.Drawing.Size(0, 0);
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Controls.Add(this.lvwSecurityGroups);
            this.ultraTabPageControl3.Controls.Add(this.ultraLabel2);
            this.ultraTabPageControl3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl3.Name = "ultraTabPageControl3";
            this.ultraTabPageControl3.Padding = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.ultraTabPageControl3.Size = new System.Drawing.Size(799, 432);
            // 
            // lvwSecurityGroups
            // 
            this.lvwSecurityGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwSecurityGroups.ItemSettings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            this.lvwSecurityGroups.ItemSettings.HideSelection = false;
            this.lvwSecurityGroups.Location = new System.Drawing.Point(9, 49);
            this.lvwSecurityGroups.MainColumn.DataType = typeof(string);
            this.lvwSecurityGroups.MainColumn.Key = "SecurityGroup";
            this.lvwSecurityGroups.MainColumn.Sorting = Infragistics.Win.UltraWinListView.Sorting.Ascending;
            this.lvwSecurityGroups.MainColumn.Text = "Security Group";
            this.lvwSecurityGroups.MainColumn.Width = 50;
            this.lvwSecurityGroups.Name = "lvwSecurityGroups";
            this.lvwSecurityGroups.Size = new System.Drawing.Size(781, 375);
            this.lvwSecurityGroups.TabIndex = 13;
            this.lvwSecurityGroups.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.lvwSecurityGroups.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns;
            this.lvwSecurityGroups.ViewSettingsDetails.CheckBoxStyle = Infragistics.Win.UltraWinListView.CheckBoxStyle.CheckBox;
            this.lvwSecurityGroups.ViewSettingsDetails.FullRowSelect = true;
            this.lvwSecurityGroups.ViewSettingsDetails.ImageSize = new System.Drawing.Size(0, 0);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(9, 28);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(101, 15);
            this.ultraLabel2.TabIndex = 1;
            this.ultraLabel2.Text = "Security Groups:";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(627, 476);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(799, 432);
            // 
            // tabFileProperties
            // 
            this.tabFileProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabFileProperties.Controls.Add(this.ultraTabSharedControlsPage1);
            this.tabFileProperties.Controls.Add(this.ultraTabPageControl1);
            this.tabFileProperties.Controls.Add(this.ultraTabPageControl2);
            this.tabFileProperties.Controls.Add(this.ultraTabPageControl3);
            this.tabFileProperties.Location = new System.Drawing.Point(14, 12);
            this.tabFileProperties.Name = "tabFileProperties";
            this.tabFileProperties.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.tabFileProperties.Size = new System.Drawing.Size(803, 458);
            this.tabFileProperties.TabIndex = 6;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "General";
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "Removed Items";
            ultraTab3.TabPage = this.ultraTabPageControl3;
            ultraTab3.Text = "Security";
            this.tabFileProperties.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2,
            ultraTab3});
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCancel.Location = new System.Drawing.Point(725, 476);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // helpLink1
            // 
            this.helpLink1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.helpLink1.BackColor = System.Drawing.Color.Transparent;
            this.helpLink1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLink1.HelpPage = "folder_properties_dialog.htm";
            this.helpLink1.Location = new System.Drawing.Point(15, 482);
            this.helpLink1.Name = "helpLink1";
            this.helpLink1.Size = new System.Drawing.Size(33, 16);
            this.helpLink1.TabIndex = 9;
            // 
            // FolderProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(831, 511);
            this.Controls.Add(this.helpLink1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tabFileProperties);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FolderProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Folder Properties";
            this.ultraTabPageControl1.ResumeLayout(false);
            this.ultraTabPageControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLocalDirectory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFolderName)).EndInit();
            this.ultraTabPageControl2.ResumeLayout(false);
            this.ultraTabPageControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lvwRemovedItems)).EndInit();
            this.ultraTabPageControl3.ResumeLayout(false);
            this.ultraTabPageControl3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lvwSecurityGroups)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabFileProperties)).EndInit();
            this.tabFileProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtLocalDirectory;
        private Infragistics.Win.Misc.UltraLabel lbLocalDirectory;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtFolderName;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl tabFileProperties;
        private Infragistics.Win.Misc.UltraButton btnRestore;
        private Infragistics.Win.UltraWinListView.UltraListView lvwRemovedItems;
        private Infragistics.Win.Misc.UltraButton btnDestroy;
        private Infragistics.Win.UltraWinListView.UltraListView lvwSecurityGroups;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Utilities.HelpLink helpLink1;

    }
}