namespace DWOS.UI.Admin
{
	partial class SplitOrder
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
			if(disposing && (components != null))
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The Work Order as defined by DWOS.", Infragistics.Win.ToolTipImage.Default, "Work Order", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The number of orders to create.", Infragistics.Win.ToolTipImage.Default, "Split Orders", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Total number of parts in the order.", Infragistics.Win.ToolTipImage.Default, "Total Parts", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The reason for splitting the order.", Infragistics.Win.ToolTipImage.Default, "Reason", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Print Work Order Travelers", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Order");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Parts");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplitOrder));
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtWorkOrder = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.numSplits = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.txtTotalParts = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboReasonCode = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.chkPrint = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.grdOrders = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.helpLink1 = new DWOS.UI.Utilities.HelpLink();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.txtWorkOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSplits)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalParts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReasonCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdOrders)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(12, 12);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(75, 15);
            this.ultraLabel2.TabIndex = 9;
            this.ultraLabel2.Text = "Work Order:";
            // 
            // txtWorkOrder
            // 
            this.txtWorkOrder.Location = new System.Drawing.Point(93, 8);
            this.txtWorkOrder.Name = "txtWorkOrder";
            this.txtWorkOrder.ReadOnly = true;
            this.txtWorkOrder.Size = new System.Drawing.Size(180, 22);
            this.txtWorkOrder.TabIndex = 8;
            this.txtWorkOrder.TabStop = false;
            ultraToolTipInfo5.ToolTipText = "The Work Order as defined by DWOS.";
            ultraToolTipInfo5.ToolTipTitle = "Work Order";
            this.tipManager.SetUltraToolTip(this.txtWorkOrder, ultraToolTipInfo5);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(196, 419);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "&Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(114, 419);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "&OK";
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            this.tipManager.ToolTipImage = Infragistics.Win.ToolTipImage.Info;
            // 
            // numSplits
            // 
            this.numSplits.Location = new System.Drawing.Point(93, 93);
            this.numSplits.MaskInput = "nnn,nnn";
            this.numSplits.MaxValue = 999999;
            this.numSplits.MinValue = 2;
            this.numSplits.Name = "numSplits";
            this.numSplits.NullText = "1";
            this.numSplits.PromptChar = ' ';
            this.numSplits.Size = new System.Drawing.Size(180, 22);
            this.numSplits.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.OnMouseEnter;
            this.numSplits.SpinIncrement = 1;
            this.numSplits.TabIndex = 1;
            this.numSplits.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextControl;
            ultraToolTipInfo3.ToolTipText = "The number of orders to create.";
            ultraToolTipInfo3.ToolTipTitle = "Split Orders";
            this.tipManager.SetUltraToolTip(this.numSplits, ultraToolTipInfo3);
            this.numSplits.Value = 2;
            this.numSplits.ValueChanged += new System.EventHandler(this.numSplits_ValueChanged);
            // 
            // txtTotalParts
            // 
            appearance1.TextHAlignAsString = "Right";
            this.txtTotalParts.Appearance = appearance1;
            this.txtTotalParts.Location = new System.Drawing.Point(93, 65);
            this.txtTotalParts.Name = "txtTotalParts";
            this.txtTotalParts.ReadOnly = true;
            this.txtTotalParts.Size = new System.Drawing.Size(180, 22);
            this.txtTotalParts.TabIndex = 28;
            this.txtTotalParts.TabStop = false;
            ultraToolTipInfo2.ToolTipText = "Total number of parts in the order.";
            ultraToolTipInfo2.ToolTipTitle = "Total Parts";
            this.tipManager.SetUltraToolTip(this.txtTotalParts, ultraToolTipInfo2);
            // 
            // cboReasonCode
            // 
            this.cboReasonCode.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboReasonCode.Location = new System.Drawing.Point(93, 37);
            this.cboReasonCode.Name = "cboReasonCode";
            this.cboReasonCode.Size = new System.Drawing.Size(180, 22);
            this.cboReasonCode.TabIndex = 0;
            ultraToolTipInfo4.ToolTipText = "The reason for splitting the order.";
            ultraToolTipInfo4.ToolTipTitle = "Reason";
            this.tipManager.SetUltraToolTip(this.cboReasonCode, ultraToolTipInfo4);
            // 
            // chkPrint
            // 
            this.chkPrint.AutoSize = true;
            this.chkPrint.Location = new System.Drawing.Point(12, 363);
            this.chkPrint.Name = "chkPrint";
            this.chkPrint.Size = new System.Drawing.Size(128, 18);
            this.chkPrint.TabIndex = 3;
            this.chkPrint.Text = "Print WO Travelers";
            ultraToolTipInfo1.ToolTipTextFormatted = "If checked, prints the traveler for the original order<br/>and each new order spl" +
    "it from it.";
            ultraToolTipInfo1.ToolTipTitle = "Print Work Order Travelers";
            this.tipManager.SetUltraToolTip(this.chkPrint, ultraToolTipInfo1);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(12, 40);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(51, 15);
            this.ultraLabel4.TabIndex = 18;
            this.ultraLabel4.Text = "Reason:";
            // 
            // grdOrders
            // 
            this.grdOrders.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.Header.VisiblePosition = 0;
            appearance2.TextHAlignAsString = "Right";
            ultraGridColumn2.CellAppearance = appearance2;
            ultraGridColumn2.DefaultCellValue = "2";
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.MaskInput = "nn,nnn";
            ultraGridColumn2.MinValue = 1;
            ultraGridColumn2.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn2.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.IntegerPositiveWithSpin;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2});
            this.grdOrders.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdOrders.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            this.grdOrders.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            this.grdOrders.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdOrders.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdOrders.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdOrders.Location = new System.Drawing.Point(12, 121);
            this.grdOrders.Name = "grdOrders";
            this.grdOrders.Size = new System.Drawing.Size(261, 236);
            this.grdOrders.TabIndex = 2;
            this.grdOrders.Text = "Orders";
            this.grdOrders.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdOrders_InitializeLayout);
            this.grdOrders.AfterExitEditMode += new System.EventHandler(this.grdOrders_AfterExitEditMode);
            this.grdOrders.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdOrders_CellChange);
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(10, 97);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(77, 15);
            this.ultraLabel5.TabIndex = 24;
            this.ultraLabel5.Text = "Split Orders:";
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(12, 69);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(70, 15);
            this.ultraLabel6.TabIndex = 27;
            this.ultraLabel6.Text = "Total Parts:";
            // 
            // helpLink1
            // 
            this.helpLink1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.helpLink1.BackColor = System.Drawing.Color.Transparent;
            this.helpLink1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLink1.HelpPage = "split_order_dialog.htm";
            this.helpLink1.Location = new System.Drawing.Point(7, 419);
            this.helpLink1.Name = "helpLink1";
            this.helpLink1.Size = new System.Drawing.Size(33, 16);
            this.helpLink1.TabIndex = 20;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(43, 398);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(230, 15);
            this.ultraLabel1.TabIndex = 30;
            this.ultraLabel1.Text = "Clicking \'OK\' will save all unsaved data.";
            // 
            // SplitOrder
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(286, 448);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.chkPrint);
            this.Controls.Add(this.txtTotalParts);
            this.Controls.Add(this.ultraLabel6);
            this.Controls.Add(this.ultraLabel5);
            this.Controls.Add(this.grdOrders);
            this.Controls.Add(this.numSplits);
            this.Controls.Add(this.helpLink1);
            this.Controls.Add(this.cboReasonCode);
            this.Controls.Add(this.ultraLabel4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.txtWorkOrder);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "SplitOrder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Split Order";
            this.Load += new System.EventHandler(this.SplitOrder_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtWorkOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSplits)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalParts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReasonCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdOrders)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtWorkOrder;
		private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboReasonCode;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Utilities.HelpLink helpLink1;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdOrders;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numSplits;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTotalParts;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPrint;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
    }
}