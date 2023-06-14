namespace DWOS.Server.Admin.SettingsPanels
{
    partial class SettingsPlugins
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn1 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("Description");
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn2 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("Security Role");
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn3 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("File Size");
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn4 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("ID");
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnDelete = new Infragistics.Win.Misc.UltraButton();
            this.btnAdd = new Infragistics.Win.Misc.UltraButton();
            this.lvwPlugins = new Infragistics.Win.UltraWinListView.UltraListView();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lvwPlugins)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.btnDelete);
            this.ultraGroupBox1.Controls.Add(this.btnAdd);
            this.ultraGroupBox1.Controls.Add(this.lvwPlugins);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(670, 298);
            this.ultraGroupBox1.TabIndex = 0;
            this.ultraGroupBox1.Text = "Plug Ins";
            // 
            // btnDelete
            // 
            appearance1.Image = global::DWOS.Server.Admin.Properties.Resources.Delete_16;
            this.btnDelete.Appearance = appearance1;
            this.btnDelete.AutoSize = true;
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(6, 81);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(26, 26);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            appearance2.Image = global::DWOS.Server.Admin.Properties.Resources.Add_16;
            this.btnAdd.Appearance = appearance2;
            this.btnAdd.AutoSize = true;
            this.btnAdd.Location = new System.Drawing.Point(6, 49);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(26, 26);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lvwPlugins
            // 
            this.lvwPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwPlugins.Location = new System.Drawing.Point(38, 27);
            this.lvwPlugins.MainColumn.Key = "Display Name";
            this.lvwPlugins.MainColumn.VisiblePositionInDetailsView = 1;
            this.lvwPlugins.Name = "lvwPlugins";
            this.lvwPlugins.Size = new System.Drawing.Size(626, 265);
            ultraListViewSubItemColumn1.Key = "Description";
            ultraListViewSubItemColumn1.VisiblePositionInDetailsView = 2;
            ultraListViewSubItemColumn2.Key = "Security Role";
            ultraListViewSubItemColumn2.VisiblePositionInDetailsView = 3;
            ultraListViewSubItemColumn3.Key = "File Size";
            ultraListViewSubItemColumn3.VisiblePositionInDetailsView = 4;
            ultraListViewSubItemColumn4.DataType = typeof(int);
            ultraListViewSubItemColumn4.Key = "ID";
            ultraListViewSubItemColumn4.VisibleInDetailsView = Infragistics.Win.DefaultableBoolean.False;
            ultraListViewSubItemColumn4.VisiblePositionInDetailsView = 0;
            this.lvwPlugins.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn1,
            ultraListViewSubItemColumn2,
            ultraListViewSubItemColumn3,
            ultraListViewSubItemColumn4});
            this.lvwPlugins.TabIndex = 1;
            this.lvwPlugins.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.lvwPlugins.ViewSettingsDetails.ColumnAutoSizeMode = ((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header | Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems)));
            this.lvwPlugins.ViewSettingsDetails.ColumnHeaderImageSize = new System.Drawing.Size(0, 0);
            this.lvwPlugins.ItemSelectionChanged += new Infragistics.Win.UltraWinListView.ItemSelectionChangedEventHandler(this.lvwPlugins_ItemSelectionChanged);
            // 
            // SettingsPlugins
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "SettingsPlugins";
            this.Size = new System.Drawing.Size(670, 298);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lvwPlugins)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.UltraWinListView.UltraListView lvwPlugins;
        private Infragistics.Win.Misc.UltraButton btnAdd;
        private Infragistics.Win.Misc.UltraButton btnDelete;
    }
}
