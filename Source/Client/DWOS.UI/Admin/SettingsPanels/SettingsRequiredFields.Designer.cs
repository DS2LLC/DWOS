namespace DWOS.UI.Admin.SettingsPanels
{
    partial class SettingsRequiredFields
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Sync order weight and quantity", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsRequiredFields));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Product Class Editor", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Serial Number Editor", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Fields", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FieldID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Alias");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Category");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsRequired");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsSystem");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsVisible");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsCustomer");
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.chkSyncWeight = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.cboProductClassEditor = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboSerialEditor = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.grdFields = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsApplicationSettings = new DWOS.Data.Datasets.ApplicationSettingsDataSet();
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkSyncWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProductClassEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSerialEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdFields)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsApplicationSettings)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.chkSyncWeight);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox1.Controls.Add(this.cboProductClassEditor);
            this.ultraGroupBox1.Controls.Add(this.cboSerialEditor);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Controls.Add(this.grdFields);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(483, 445);
            this.ultraGroupBox1.TabIndex = 0;
            this.ultraGroupBox1.Text = "Required Fields";
            // 
            // chkSyncWeight
            // 
            this.chkSyncWeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkSyncWeight.AutoSize = true;
            this.chkSyncWeight.Location = new System.Drawing.Point(6, 414);
            this.chkSyncWeight.Name = "chkSyncWeight";
            this.chkSyncWeight.Size = new System.Drawing.Size(147, 17);
            this.chkSyncWeight.TabIndex = 6;
            this.chkSyncWeight.Text = "Sync weight and quantity";
            ultraToolTipInfo1.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo1.ToolTipTextFormatted");
            ultraToolTipInfo1.ToolTipTitle = "Sync order weight and quantity";
            this.tipManager.SetUltraToolTip(this.chkSyncWeight, ultraToolTipInfo1);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(6, 391);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(110, 14);
            this.ultraLabel2.TabIndex = 6;
            this.ultraLabel2.Text = "Product Class Editor:";
            // 
            // cboProductClassEditor
            // 
            this.cboProductClassEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboProductClassEditor.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboProductClassEditor.Location = new System.Drawing.Point(123, 387);
            this.cboProductClassEditor.Name = "cboProductClassEditor";
            this.cboProductClassEditor.Size = new System.Drawing.Size(357, 21);
            this.cboProductClassEditor.TabIndex = 5;
            ultraToolTipInfo2.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo2.ToolTipTextFormatted");
            ultraToolTipInfo2.ToolTipTitle = "Product Class Editor";
            this.tipManager.SetUltraToolTip(this.cboProductClassEditor, ultraToolTipInfo2);
            // 
            // cboSerialEditor
            // 
            this.cboSerialEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboSerialEditor.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboSerialEditor.Location = new System.Drawing.Point(123, 360);
            this.cboSerialEditor.Name = "cboSerialEditor";
            this.cboSerialEditor.Size = new System.Drawing.Size(357, 21);
            this.cboSerialEditor.TabIndex = 4;
            ultraToolTipInfo3.ToolTipTextFormatted = "Determines the type of serial number editor to use.<br/><b>Basic</b>: Allows ente" +
    "ring a single serial number.<br/><b>Advanced:</b> Allows entering multiple seria" +
    "l numbers.";
            ultraToolTipInfo3.ToolTipTitle = "Serial Number Editor";
            this.tipManager.SetUltraToolTip(this.cboSerialEditor, ultraToolTipInfo3);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 364);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(112, 14);
            this.ultraLabel1.TabIndex = 3;
            this.ultraLabel1.Text = "Serial Number Editor:";
            // 
            // grdFields
            // 
            this.grdFields.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdFields.DataMember = "Fields";
            this.grdFields.DataSource = this.dsApplicationSettings;
            ultraGridColumn15.Header.VisiblePosition = 0;
            ultraGridColumn15.Hidden = true;
            ultraGridColumn16.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn16.Header.VisiblePosition = 1;
            ultraGridColumn16.Width = 215;
            ultraGridColumn17.Header.VisiblePosition = 2;
            ultraGridColumn17.Hidden = true;
            ultraGridColumn18.Header.VisiblePosition = 3;
            ultraGridColumn19.Header.Caption = "Required";
            ultraGridColumn19.Header.VisiblePosition = 4;
            ultraGridColumn19.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn20.Header.VisiblePosition = 5;
            ultraGridColumn20.Hidden = true;
            ultraGridColumn21.Header.Caption = "Visible";
            ultraGridColumn21.Header.VisiblePosition = 6;
            ultraGridColumn21.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn1.Header.VisiblePosition = 7;
            ultraGridColumn1.Hidden = true;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn20,
            ultraGridColumn21,
            ultraGridColumn1});
            ultraGridBand1.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdFields.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdFields.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdFields.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdFields.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.grdFields.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdFields.Location = new System.Drawing.Point(6, 26);
            this.grdFields.Name = "grdFields";
            this.grdFields.Size = new System.Drawing.Size(471, 328);
            this.grdFields.TabIndex = 2;
            this.grdFields.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChange;
            this.grdFields.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdFields_AfterCellUpdate);
            this.grdFields.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdFields_InitializeLayout);
            this.grdFields.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdFields_InitializeRow);
            // 
            // dsApplicationSettings
            // 
            this.dsApplicationSettings.DataSetName = "ApplicationSettingsDataSet";
            this.dsApplicationSettings.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // SettingsRequiredFields
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "SettingsRequiredFields";
            this.Size = new System.Drawing.Size(483, 445);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkSyncWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProductClassEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSerialEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdFields)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsApplicationSettings)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdFields;
        private Data.Datasets.ApplicationSettingsDataSet dsApplicationSettings;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboSerialEditor;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboProductClassEditor;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkSyncWeight;
    }
}
