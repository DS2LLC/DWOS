namespace DWOS.UI.Sales.Order
{
    partial class BatchInformation
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
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("btnFilterBatch");
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The system defined number of the batch.", Infragistics.Win.ToolTipImage.Default, "Batch", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the batch was opened.", Infragistics.Win.ToolTipImage.Default, "Date Opened", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the batch was closed.", Infragistics.Win.ToolTipImage.Default, "Date Closed", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The fixture of the batch.", Infragistics.Win.ToolTipImage.Default, "Fixture", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The status of the batch.", Infragistics.Win.ToolTipImage.Default, "Status", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The current location of the batch.", Infragistics.Win.ToolTipImage.Default, "Current Location", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The orders in the batch.", Infragistics.Win.ToolTipImage.Default, "Orders", Infragistics.Win.DefaultableBoolean.Default);
            this.txtBatch = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.dteOpenDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.dteCloseDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.txtFixture = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtWorkStatus = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtCurrentLocation = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.grdBatchOrder = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBatch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteOpenDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteCloseDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFixture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWorkStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentLocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdBatchOrder)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.ultraLabel7);
            this.grpData.Controls.Add(this.ultraLabel6);
            this.grpData.Controls.Add(this.ultraLabel5);
            this.grpData.Controls.Add(this.ultraLabel4);
            this.grpData.Controls.Add(this.ultraLabel3);
            this.grpData.Controls.Add(this.ultraLabel2);
            this.grpData.Controls.Add(this.ultraLabel1);
            this.grpData.Controls.Add(this.grdBatchOrder);
            this.grpData.Controls.Add(this.txtCurrentLocation);
            this.grpData.Controls.Add(this.txtWorkStatus);
            this.grpData.Controls.Add(this.txtFixture);
            this.grpData.Controls.Add(this.dteCloseDate);
            this.grpData.Controls.Add(this.dteOpenDate);
            this.grpData.Controls.Add(this.txtBatch);
            appearance13.Image = global::DWOS.UI.Properties.Resources.Batch16;
            this.grpData.HeaderAppearance = appearance13;
            this.grpData.Text = "Batch";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.txtBatch, 0);
            this.grpData.Controls.SetChildIndex(this.dteOpenDate, 0);
            this.grpData.Controls.SetChildIndex(this.dteCloseDate, 0);
            this.grpData.Controls.SetChildIndex(this.txtFixture, 0);
            this.grpData.Controls.SetChildIndex(this.txtWorkStatus, 0);
            this.grpData.Controls.SetChildIndex(this.txtCurrentLocation, 0);
            this.grpData.Controls.SetChildIndex(this.grdBatchOrder, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel3, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel4, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel5, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel6, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel7, 0);
            // 
            // txtBatch
            // 
            appearance12.Image = global::DWOS.UI.Properties.Resources.Filter_16;
            editorButton1.Appearance = appearance12;
            editorButton1.Key = "btnFilterBatch";
            this.txtBatch.ButtonsRight.Add(editorButton1);
            this.txtBatch.Location = new System.Drawing.Point(59, 36);
            this.txtBatch.Name = "txtBatch";
            this.txtBatch.ReadOnly = true;
            this.txtBatch.Size = new System.Drawing.Size(143, 22);
            this.txtBatch.TabIndex = 0;
            ultraToolTipInfo7.ToolTipText = "The system defined number of the batch.";
            ultraToolTipInfo7.ToolTipTitle = "Batch";
            this.tipManager.SetUltraToolTip(this.txtBatch, ultraToolTipInfo7);
            this.txtBatch.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtBatch_EditorButtonClick);
            // 
            // dteOpenDate
            // 
            this.dteOpenDate.Location = new System.Drawing.Point(102, 64);
            this.dteOpenDate.Name = "dteOpenDate";
            this.dteOpenDate.ReadOnly = true;
            this.dteOpenDate.Size = new System.Drawing.Size(100, 22);
            this.dteOpenDate.TabIndex = 2;
            ultraToolTipInfo6.ToolTipText = "The date the batch was opened.";
            ultraToolTipInfo6.ToolTipTitle = "Date Opened";
            this.tipManager.SetUltraToolTip(this.dteOpenDate, ultraToolTipInfo6);
            // 
            // dteCloseDate
            // 
            this.dteCloseDate.Location = new System.Drawing.Point(310, 64);
            this.dteCloseDate.Name = "dteCloseDate";
            this.dteCloseDate.ReadOnly = true;
            this.dteCloseDate.Size = new System.Drawing.Size(100, 22);
            this.dteCloseDate.TabIndex = 3;
            ultraToolTipInfo5.ToolTipText = "The date the batch was closed.";
            ultraToolTipInfo5.ToolTipTitle = "Date Closed";
            this.tipManager.SetUltraToolTip(this.dteCloseDate, ultraToolTipInfo5);
            // 
            // txtFixture
            // 
            this.txtFixture.Location = new System.Drawing.Point(281, 36);
            this.txtFixture.Name = "txtFixture";
            this.txtFixture.ReadOnly = true;
            this.txtFixture.Size = new System.Drawing.Size(129, 22);
            this.txtFixture.TabIndex = 1;
            ultraToolTipInfo4.ToolTipText = "The fixture of the batch.";
            ultraToolTipInfo4.ToolTipTitle = "Fixture";
            this.tipManager.SetUltraToolTip(this.txtFixture, ultraToolTipInfo4);
            // 
            // txtWorkStatus
            // 
            this.txtWorkStatus.Location = new System.Drawing.Point(132, 92);
            this.txtWorkStatus.Name = "txtWorkStatus";
            this.txtWorkStatus.ReadOnly = true;
            this.txtWorkStatus.Size = new System.Drawing.Size(278, 22);
            this.txtWorkStatus.TabIndex = 4;
            ultraToolTipInfo3.ToolTipText = "The status of the batch.";
            ultraToolTipInfo3.ToolTipTitle = "Status";
            this.tipManager.SetUltraToolTip(this.txtWorkStatus, ultraToolTipInfo3);
            // 
            // txtCurrentLocation
            // 
            this.txtCurrentLocation.Location = new System.Drawing.Point(132, 120);
            this.txtCurrentLocation.Name = "txtCurrentLocation";
            this.txtCurrentLocation.ReadOnly = true;
            this.txtCurrentLocation.Size = new System.Drawing.Size(278, 22);
            this.txtCurrentLocation.TabIndex = 5;
            ultraToolTipInfo2.ToolTipText = "The current location of the batch.";
            ultraToolTipInfo2.ToolTipTitle = "Current Location";
            this.tipManager.SetUltraToolTip(this.txtCurrentLocation, ultraToolTipInfo2);
            // 
            // grdBatchOrder
            // 
            this.grdBatchOrder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grdBatchOrder.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            this.grdBatchOrder.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance1.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance1.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance1.BorderColor = System.Drawing.SystemColors.Window;
            this.grdBatchOrder.DisplayLayout.GroupByBox.Appearance = appearance1;
            appearance2.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdBatchOrder.DisplayLayout.GroupByBox.BandLabelAppearance = appearance2;
            this.grdBatchOrder.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance3.BackColor2 = System.Drawing.SystemColors.Control;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdBatchOrder.DisplayLayout.GroupByBox.PromptAppearance = appearance3;
            this.grdBatchOrder.DisplayLayout.MaxColScrollRegions = 1;
            this.grdBatchOrder.DisplayLayout.MaxRowScrollRegions = 1;
            appearance4.BackColor = System.Drawing.SystemColors.Window;
            appearance4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdBatchOrder.DisplayLayout.Override.ActiveCellAppearance = appearance4;
            appearance5.BackColor = System.Drawing.SystemColors.Highlight;
            appearance5.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdBatchOrder.DisplayLayout.Override.ActiveRowAppearance = appearance5;
            this.grdBatchOrder.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdBatchOrder.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdBatchOrder.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.grdBatchOrder.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.grdBatchOrder.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdBatchOrder.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            this.grdBatchOrder.DisplayLayout.Override.CardAreaAppearance = appearance6;
            appearance7.BorderColor = System.Drawing.Color.Silver;
            appearance7.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdBatchOrder.DisplayLayout.Override.CellAppearance = appearance7;
            this.grdBatchOrder.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdBatchOrder.DisplayLayout.Override.CellPadding = 0;
            appearance8.BackColor = System.Drawing.SystemColors.Control;
            appearance8.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance8.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance8.BorderColor = System.Drawing.SystemColors.Window;
            this.grdBatchOrder.DisplayLayout.Override.GroupByRowAppearance = appearance8;
            appearance9.TextHAlignAsString = "Left";
            this.grdBatchOrder.DisplayLayout.Override.HeaderAppearance = appearance9;
            this.grdBatchOrder.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdBatchOrder.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance10.BackColor = System.Drawing.SystemColors.Window;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            this.grdBatchOrder.DisplayLayout.Override.RowAppearance = appearance10;
            this.grdBatchOrder.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdBatchOrder.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdBatchOrder.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdBatchOrder.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance11.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdBatchOrder.DisplayLayout.Override.TemplateAddRowAppearance = appearance11;
            this.grdBatchOrder.Location = new System.Drawing.Point(11, 189);
            this.grdBatchOrder.Name = "grdBatchOrder";
            this.grdBatchOrder.Size = new System.Drawing.Size(399, 242);
            this.grdBatchOrder.TabIndex = 6;
            this.grdBatchOrder.Text = "Orders";
            ultraToolTipInfo1.ToolTipText = "The orders in the batch.";
            ultraToolTipInfo1.ToolTipTitle = "Orders";
            this.tipManager.SetUltraToolTip(this.grdBatchOrder, ultraToolTipInfo1);
            this.grdBatchOrder.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdBatchOrder_InitializeLayout);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(12, 40);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(41, 15);
            this.ultraLabel1.TabIndex = 8;
            this.ultraLabel1.Text = "Batch:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.ultraLabel2.Location = new System.Drawing.Point(226, 40);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(49, 15);
            this.ultraLabel2.TabIndex = 9;
            this.ultraLabel2.Text = "Fixture:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(12, 71);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(84, 15);
            this.ultraLabel3.TabIndex = 10;
            this.ultraLabel3.Text = "Date Opened:";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(226, 71);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(78, 15);
            this.ultraLabel4.TabIndex = 11;
            this.ultraLabel4.Text = "Date Closed:";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(12, 96);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(46, 15);
            this.ultraLabel5.TabIndex = 12;
            this.ultraLabel5.Text = "Status:";
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.ultraLabel6.Location = new System.Drawing.Point(12, 124);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(104, 15);
            this.ultraLabel6.TabIndex = 13;
            this.ultraLabel6.Text = "Current Location:";
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.ultraLabel7.Location = new System.Drawing.Point(12, 168);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(48, 15);
            this.ultraLabel7.TabIndex = 14;
            this.ultraLabel7.Text = "Orders:";
            // 
            // BatchInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "BatchInformation";
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBatch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteOpenDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteCloseDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFixture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWorkStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentLocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdBatchOrder)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtBatch;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCurrentLocation;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtWorkStatus;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtFixture;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteCloseDate;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteOpenDate;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdBatchOrder;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
    }
}
