namespace DWOS.UI.Admin
{
    partial class ProcessPicker
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
            OnDispose();
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Process ID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessAlias ID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Process Alias");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Department");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Process");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsCustomerAlias");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Customer Preferred Only", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessPicker));
            this.grdProcesses = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.chkPreferred = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.lblCustomer = new Infragistics.Win.Misc.UltraLabel();
            this.dsProcesses = new DWOS.Data.Datasets.ProcessesDataset();
            this.taProcessAlias = new DWOS.Data.Datasets.ProcessesDatasetTableAdapters.ProcessAliasTableAdapter();
            this.taCustomerProcess = new DWOS.Data.Datasets.ProcessesDatasetTableAdapters.CustomerProcessAliasTableAdapter();
            this.taProcess = new DWOS.Data.Datasets.ProcessesDatasetTableAdapters.ProcessTableAdapter();
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.persistWindowState1 = new DWOS.UI.Utilities.PersistWindowState(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblCustomerProcesses = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCustomerCount = new System.Windows.Forms.Label();
            this.lblTotalCount = new System.Windows.Forms.Label();
            this.pnlCustomerInfo = new Infragistics.Win.Misc.UltraPanel();
            ((System.ComponentModel.ISupportInitialize)(this.grdProcesses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPreferred)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsProcesses)).BeginInit();
            this.pnlCustomerInfo.ClientArea.SuspendLayout();
            this.pnlCustomerInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // grdProcesses
            // 
            this.grdProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdProcesses.DisplayLayout.Appearance = appearance1;
            this.grdProcesses.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn1.Width = 155;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Hidden = true;
            ultraGridColumn2.Width = 183;
            ultraGridColumn3.Header.VisiblePosition = 3;
            ultraGridColumn3.Width = 277;
            ultraGridColumn6.Header.VisiblePosition = 2;
            ultraGridColumn6.Width = 116;
            ultraGridColumn4.Header.VisiblePosition = 4;
            ultraGridColumn4.Width = 326;
            ultraGridColumn5.Header.Caption = "Preferred";
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn5.Hidden = true;
            ultraGridColumn5.Width = 92;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn6,
            ultraGridColumn4,
            ultraGridColumn5});
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdProcesses.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdProcesses.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.grdProcesses.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdProcesses.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.grdProcesses.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdProcesses.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.grdProcesses.DisplayLayout.MaxColScrollRegions = 1;
            this.grdProcesses.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdProcesses.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdProcesses.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.grdProcesses.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdProcesses.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdProcesses.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdProcesses.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.grdProcesses.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdProcesses.DisplayLayout.Override.CellAppearance = appearance8;
            this.grdProcesses.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.grdProcesses.DisplayLayout.Override.CellPadding = 0;
            this.grdProcesses.DisplayLayout.Override.FilterOperatorDefaultValue = Infragistics.Win.UltraWinGrid.FilterOperatorDefaultValue.Contains;
            this.grdProcesses.DisplayLayout.Override.FilterUIType = Infragistics.Win.UltraWinGrid.FilterUIType.FilterRow;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.grdProcesses.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.grdProcesses.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.grdProcesses.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdProcesses.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.grdProcesses.DisplayLayout.Override.RowAppearance = appearance11;
            this.grdProcesses.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdProcesses.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdProcesses.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdProcesses.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.grdProcesses.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdProcesses.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdProcesses.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdProcesses.Location = new System.Drawing.Point(12, 37);
            this.grdProcesses.Name = "grdProcesses";
            this.grdProcesses.Size = new System.Drawing.Size(740, 327);
            this.grdProcesses.TabIndex = 36;
            this.grdProcesses.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdProcesses_InitializeLayout);
            this.grdProcesses.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdProcesses_InitializeRow);
            this.grdProcesses.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.grdProcesses_DoubleClickRow);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(674, 370);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 35;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(592, 370);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 34;
            this.btnOK.Text = "OK";
            // 
            // chkPreferred
            // 
            this.chkPreferred.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkPreferred.Location = new System.Drawing.Point(3, 3);
            this.chkPreferred.Name = "chkPreferred";
            this.chkPreferred.Size = new System.Drawing.Size(225, 20);
            this.chkPreferred.TabIndex = 37;
            this.chkPreferred.Text = "Show Customer Preferred Only";
            ultraToolTipInfo1.ToolTipTextFormatted = "Toggle to display only customer preferred process alias names only.";
            ultraToolTipInfo1.ToolTipTitle = "Customer Preferred Only";
            this.tipManager.SetUltraToolTip(this.chkPreferred, ultraToolTipInfo1);
            this.chkPreferred.CheckedChanged += new System.EventHandler(this.chkPreferred_CheckedChanged);
            // 
            // lblCustomer
            // 
            this.lblCustomer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCustomer.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomer.Location = new System.Drawing.Point(12, 12);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(740, 19);
            this.lblCustomer.TabIndex = 38;
            this.lblCustomer.Text = "Sample Customer";
            this.lblCustomer.UseAppStyling = false;
            // 
            // dsProcesses
            // 
            this.dsProcesses.DataSetName = "ProcessesDataset";
            this.dsProcesses.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taProcessAlias
            // 
            this.taProcessAlias.ClearBeforeFill = true;
            // 
            // taCustomerProcess
            // 
            this.taCustomerProcess.ClearBeforeFill = true;
            // 
            // taProcess
            // 
            this.taProcess.ClearBeforeFill = true;
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // persistWindowState1
            // 
            this.persistWindowState1.FileNamePrefix = null;
            this.persistWindowState1.ParentForm = this;
            this.persistWindowState1.Splitter = null;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(57, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 13);
            this.label3.TabIndex = 40;
            this.label3.Text = "Customer Preferred";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.BackColor = System.Drawing.Color.LightGreen;
            this.label4.Location = new System.Drawing.Point(34, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 39;
            // 
            // lblCustomerProcesses
            // 
            this.lblCustomerProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCustomerProcesses.AutoSize = true;
            this.lblCustomerProcesses.Location = new System.Drawing.Point(254, 23);
            this.lblCustomerProcesses.Name = "lblCustomerProcesses";
            this.lblCustomerProcesses.Size = new System.Drawing.Size(129, 13);
            this.lblCustomerProcesses.TabIndex = 41;
            this.lblCustomerProcesses.Text = "Customer Processes:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(254, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 42;
            this.label1.Text = "Total Processes:";
            // 
            // lblCustomerCount
            // 
            this.lblCustomerCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCustomerCount.AutoSize = true;
            this.lblCustomerCount.Location = new System.Drawing.Point(389, 23);
            this.lblCustomerCount.Name = "lblCustomerCount";
            this.lblCustomerCount.Size = new System.Drawing.Size(14, 13);
            this.lblCustomerCount.TabIndex = 43;
            this.lblCustomerCount.Text = "0";
            // 
            // lblTotalCount
            // 
            this.lblTotalCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalCount.AutoSize = true;
            this.lblTotalCount.Location = new System.Drawing.Point(389, 6);
            this.lblTotalCount.Name = "lblTotalCount";
            this.lblTotalCount.Size = new System.Drawing.Size(14, 13);
            this.lblTotalCount.TabIndex = 44;
            this.lblTotalCount.Text = "0";
            // 
            // pnlCustomerInfo
            // 
            this.pnlCustomerInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            // 
            // pnlCustomerInfo.ClientArea
            // 
            this.pnlCustomerInfo.ClientArea.Controls.Add(this.chkPreferred);
            this.pnlCustomerInfo.ClientArea.Controls.Add(this.lblTotalCount);
            this.pnlCustomerInfo.ClientArea.Controls.Add(this.label4);
            this.pnlCustomerInfo.ClientArea.Controls.Add(this.lblCustomerCount);
            this.pnlCustomerInfo.ClientArea.Controls.Add(this.label3);
            this.pnlCustomerInfo.ClientArea.Controls.Add(this.label1);
            this.pnlCustomerInfo.ClientArea.Controls.Add(this.lblCustomerProcesses);
            this.pnlCustomerInfo.Location = new System.Drawing.Point(11, 365);
            this.pnlCustomerInfo.Name = "pnlCustomerInfo";
            this.pnlCustomerInfo.Size = new System.Drawing.Size(447, 40);
            this.pnlCustomerInfo.TabIndex = 45;
            this.pnlCustomerInfo.Visible = false;
            // 
            // ProcessPicker
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(764, 405);
            this.Controls.Add(this.pnlCustomerInfo);
            this.Controls.Add(this.lblCustomer);
            this.Controls.Add(this.grdProcesses);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProcessPicker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Process";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ProcessPicker_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdProcesses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPreferred)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsProcesses)).EndInit();
            this.pnlCustomerInfo.ClientArea.ResumeLayout(false);
            this.pnlCustomerInfo.ClientArea.PerformLayout();
            this.pnlCustomerInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal Infragistics.Win.UltraWinGrid.UltraGrid grdProcesses;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Utilities.PersistWindowState persistWindowState1;
        private Data.Datasets.ProcessesDataset dsProcesses;
        private Data.Datasets.ProcessesDatasetTableAdapters.ProcessAliasTableAdapter taProcessAlias;
        private Data.Datasets.ProcessesDatasetTableAdapters.CustomerProcessAliasTableAdapter taCustomerProcess;
        private Data.Datasets.ProcessesDatasetTableAdapters.ProcessTableAdapter taProcess;
        private Infragistics.Win.Misc.UltraLabel lblCustomer;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPreferred;
        protected Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblCustomerProcesses;
        private System.Windows.Forms.Label lblCustomerCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTotalCount;
        private Infragistics.Win.Misc.UltraPanel pnlCustomerInfo;
    }
}