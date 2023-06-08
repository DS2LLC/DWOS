namespace DWOS.UI.Sales
{
	partial class OrderBulkFieldChange
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The customer work order, if available.", Infragistics.Win.ToolTipImage.Default, "Customer Work Order", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the order is required by the customer. ", Infragistics.Win.ToolTipImage.Default, "Required Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the order was entered.", Infragistics.Win.ToolTipImage.Default, "Order Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The priority of the order.", Infragistics.Win.ToolTipImage.Default, "Priority", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The number of parts in the order.", Infragistics.Win.ToolTipImage.Default, "Quantity", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("Add");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton2 = new Infragistics.Win.UltraWinEditors.EditorButton("Delete");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton3 = new Infragistics.Win.UltraWinEditors.EditorButton("View");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Document for the purchase order.", Infragistics.Win.ToolTipImage.Default, "Purchase Order Document", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The unit price of the part. ", Infragistics.Win.ToolTipImage.Default, "Unit Price", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The purchase order number received from the customer.", Infragistics.Win.ToolTipImage.Default, "Purchase Order Number", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The estimated shipping date of the order.", Infragistics.Win.ToolTipImage.Default, "Estimated Ship Date", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderBulkFieldChange));
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.cboCustomerWO = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel19 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel16 = new Infragistics.Win.Misc.UltraLabel();
            this.dtOrderRequiredDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.dtOrderDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.cboPriority = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.numPartQty = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.txtMedia = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.numUnitPrice = new Infragistics.Win.UltraWinEditors.UltraCurrencyEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtPONumber = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel14 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.chkPO = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkOrderDate = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkOrderRequiredDate = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkCustomerWO = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkMedia = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkPartQty = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkPriority = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkUnitPrice = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.dtShipDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.chkEstShipDate = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.helpLink1 = new DWOS.UI.Utilities.HelpLink();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomerWO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOrderRequiredDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOrderDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPriority)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMedia)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUnitPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPONumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkOrderDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkOrderRequiredDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCustomerWO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMedia)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPartQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPriority)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkUnitPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtShipDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEstShipDate)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(90, 293);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 23);
            this.btnOK.TabIndex = 18;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(181, 293);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 19;
            this.btnCancel.Text = "&Cancel";
            // 
            // cboCustomerWO
            // 
            this.cboCustomerWO.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCustomerWO.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            this.cboCustomerWO.Enabled = false;
            this.cboCustomerWO.Location = new System.Drawing.Point(131, 130);
            this.cboCustomerWO.Name = "cboCustomerWO";
            this.cboCustomerWO.Size = new System.Drawing.Size(133, 22);
            this.cboCustomerWO.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.cboCustomerWO.TabIndex = 9;
            ultraToolTipInfo9.ToolTipText = "The customer work order, if available.";
            ultraToolTipInfo9.ToolTipTitle = "Customer Work Order";
            this.ultraToolTipManager.SetUltraToolTip(this.cboCustomerWO, ultraToolTipInfo9);
            // 
            // ultraLabel19
            // 
            this.ultraLabel19.AutoSize = true;
            this.ultraLabel19.Location = new System.Drawing.Point(12, 240);
            this.ultraLabel19.Name = "ultraLabel19";
            this.ultraLabel19.Size = new System.Drawing.Size(64, 15);
            this.ultraLabel19.TabIndex = 100;
            this.ultraLabel19.Text = "Unit Price:";
            // 
            // ultraLabel16
            // 
            this.ultraLabel16.AutoSize = true;
            this.ultraLabel16.Location = new System.Drawing.Point(11, 128);
            this.ultraLabel16.Name = "ultraLabel16";
            this.ultraLabel16.Size = new System.Drawing.Size(88, 15);
            this.ultraLabel16.TabIndex = 101;
            this.ultraLabel16.Text = "Customer WO:";
            // 
            // dtOrderRequiredDate
            // 
            this.dtOrderRequiredDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtOrderRequiredDate.Enabled = false;
            this.dtOrderRequiredDate.Location = new System.Drawing.Point(131, 102);
            this.dtOrderRequiredDate.Name = "dtOrderRequiredDate";
            this.dtOrderRequiredDate.Size = new System.Drawing.Size(133, 22);
            this.dtOrderRequiredDate.TabIndex = 7;
            this.dtOrderRequiredDate.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextSection;
            ultraToolTipInfo2.ToolTipText = "The date the order is required by the customer. ";
            ultraToolTipInfo2.ToolTipTitle = "Required Date";
            this.ultraToolTipManager.SetUltraToolTip(this.dtOrderRequiredDate, ultraToolTipInfo2);
            // 
            // dtOrderDate
            // 
            this.dtOrderDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtOrderDate.Enabled = false;
            this.dtOrderDate.Location = new System.Drawing.Point(131, 46);
            this.dtOrderDate.Name = "dtOrderDate";
            this.dtOrderDate.Size = new System.Drawing.Size(133, 22);
            this.dtOrderDate.TabIndex = 3;
            this.dtOrderDate.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextSection;
            ultraToolTipInfo3.ToolTipText = "The date the order was entered.";
            ultraToolTipInfo3.ToolTipTitle = "Order Date";
            this.ultraToolTipManager.SetUltraToolTip(this.dtOrderDate, ultraToolTipInfo3);
            // 
            // cboPriority
            // 
            this.cboPriority.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPriority.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
            this.cboPriority.DropDownListWidth = -1;
            this.cboPriority.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboPriority.Enabled = false;
            this.cboPriority.Location = new System.Drawing.Point(131, 158);
            this.cboPriority.Name = "cboPriority";
            this.cboPriority.Nullable = false;
            this.cboPriority.Size = new System.Drawing.Size(133, 22);
            this.cboPriority.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.cboPriority.TabIndex = 11;
            ultraToolTipInfo4.ToolTipText = "The priority of the order.";
            ultraToolTipInfo4.ToolTipTitle = "Priority";
            this.ultraToolTipManager.SetUltraToolTip(this.cboPriority, ultraToolTipInfo4);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(11, 212);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(29, 15);
            this.ultraLabel1.TabIndex = 97;
            this.ultraLabel1.Text = "Qty:";
            // 
            // numPartQty
            // 
            this.numPartQty.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numPartQty.Enabled = false;
            this.numPartQty.Location = new System.Drawing.Point(131, 214);
            this.numPartQty.MaskInput = "nnn,nnn";
            this.numPartQty.MaxValue = 999999;
            this.numPartQty.MinValue = 0;
            this.numPartQty.Name = "numPartQty";
            this.numPartQty.NullText = "0";
            this.numPartQty.PromptChar = ' ';
            this.numPartQty.Size = new System.Drawing.Size(133, 22);
            this.numPartQty.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.OnMouseEnter;
            this.numPartQty.TabIndex = 15;
            ultraToolTipInfo5.ToolTipText = "The number of parts in the order.";
            ultraToolTipInfo5.ToolTipTitle = "Quantity";
            this.ultraToolTipManager.SetUltraToolTip(this.numPartQty, ultraToolTipInfo5);
            this.numPartQty.Value = 1;
            // 
            // txtMedia
            // 
            this.txtMedia.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::DWOS.UI.Properties.Resources.Add_16;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            editorButton1.Appearance = appearance1;
            editorButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton1.Key = "Add";
            appearance2.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Center;
            editorButton2.Appearance = appearance2;
            editorButton2.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton2.Key = "Delete";
            appearance3.Image = global::DWOS.UI.Properties.Resources.View;
            appearance3.ImageHAlign = Infragistics.Win.HAlign.Center;
            editorButton3.Appearance = appearance3;
            editorButton3.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton3.Key = "View";
            this.txtMedia.ButtonsRight.Add(editorButton1);
            this.txtMedia.ButtonsRight.Add(editorButton2);
            this.txtMedia.ButtonsRight.Add(editorButton3);
            this.txtMedia.Enabled = false;
            this.txtMedia.HideSelection = false;
            this.txtMedia.Location = new System.Drawing.Point(131, 186);
            this.txtMedia.Name = "txtMedia";
            this.txtMedia.ReadOnly = true;
            this.txtMedia.Size = new System.Drawing.Size(133, 22);
            this.txtMedia.TabIndex = 13;
            this.txtMedia.TabStop = false;
            ultraToolTipInfo6.ToolTipText = "Document for the purchase order.";
            ultraToolTipInfo6.ToolTipTitle = "Purchase Order Document";
            this.ultraToolTipManager.SetUltraToolTip(this.txtMedia, ultraToolTipInfo6);
            this.txtMedia.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtMedia_EditorButtonClick);
            // 
            // numUnitPrice
            // 
            this.numUnitPrice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numUnitPrice.Enabled = false;
            this.numUnitPrice.Location = new System.Drawing.Point(131, 242);
            this.numUnitPrice.MaxValue = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numUnitPrice.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numUnitPrice.Name = "numUnitPrice";
            this.numUnitPrice.Size = new System.Drawing.Size(133, 22);
            this.numUnitPrice.TabIndex = 17;
            ultraToolTipInfo7.ToolTipText = "The unit price of the part. ";
            ultraToolTipInfo7.ToolTipTitle = "Unit Price";
            this.ultraToolTipManager.SetUltraToolTip(this.numUnitPrice, ultraToolTipInfo7);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(12, 185);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(51, 15);
            this.ultraLabel2.TabIndex = 99;
            this.ultraLabel2.Text = "PO Doc:";
            // 
            // txtPONumber
            // 
            this.txtPONumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPONumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtPONumber.Enabled = false;
            this.txtPONumber.Location = new System.Drawing.Point(131, 18);
            this.txtPONumber.MaxLength = 50;
            this.txtPONumber.Name = "txtPONumber";
            this.txtPONumber.Size = new System.Drawing.Size(133, 22);
            this.txtPONumber.TabIndex = 1;
            ultraToolTipInfo8.ToolTipText = "The purchase order number received from the customer.";
            ultraToolTipInfo8.ToolTipTitle = "Purchase Order Number";
            this.ultraToolTipManager.SetUltraToolTip(this.txtPONumber, ultraToolTipInfo8);
            // 
            // ultraLabel14
            // 
            this.ultraLabel14.AutoSize = true;
            this.ultraLabel14.Location = new System.Drawing.Point(12, 22);
            this.ultraLabel14.Name = "ultraLabel14";
            this.ultraLabel14.Size = new System.Drawing.Size(74, 15);
            this.ultraLabel14.TabIndex = 98;
            this.ultraLabel14.Text = "PO Number:";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(11, 156);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(50, 15);
            this.ultraLabel4.TabIndex = 96;
            this.ultraLabel4.Text = "Priority:";
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(12, 100);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(66, 15);
            this.ultraLabel7.TabIndex = 95;
            this.ultraLabel7.Text = "Req. Date:";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(12, 50);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(72, 15);
            this.ultraLabel5.TabIndex = 94;
            this.ultraLabel5.Text = "Order Date:";
            // 
            // chkPO
            // 
            this.chkPO.AutoSize = true;
            this.chkPO.Location = new System.Drawing.Point(111, 20);
            this.chkPO.Name = "chkPO";
            this.chkPO.Size = new System.Drawing.Size(14, 13);
            this.chkPO.TabIndex = 0;
            this.chkPO.AfterCheckStateChanged += new Infragistics.Win.CheckEditor.AfterCheckStateChangedHandler(this.chkPO_AfterCheckStateChanged);
            // 
            // chkOrderDate
            // 
            this.chkOrderDate.AutoSize = true;
            this.chkOrderDate.Location = new System.Drawing.Point(111, 48);
            this.chkOrderDate.Name = "chkOrderDate";
            this.chkOrderDate.Size = new System.Drawing.Size(14, 13);
            this.chkOrderDate.TabIndex = 2;
            this.chkOrderDate.AfterCheckStateChanged += new Infragistics.Win.CheckEditor.AfterCheckStateChangedHandler(this.chkOrderDate_AfterCheckStateChanged);
            // 
            // chkOrderRequiredDate
            // 
            this.chkOrderRequiredDate.AutoSize = true;
            this.chkOrderRequiredDate.Location = new System.Drawing.Point(111, 102);
            this.chkOrderRequiredDate.Name = "chkOrderRequiredDate";
            this.chkOrderRequiredDate.Size = new System.Drawing.Size(14, 13);
            this.chkOrderRequiredDate.TabIndex = 6;
            this.chkOrderRequiredDate.AfterCheckStateChanged += new Infragistics.Win.CheckEditor.AfterCheckStateChangedHandler(this.chkOrderRequiredDate_AfterCheckStateChanged);
            // 
            // chkCustomerWO
            // 
            this.chkCustomerWO.AutoSize = true;
            this.chkCustomerWO.Location = new System.Drawing.Point(111, 130);
            this.chkCustomerWO.Name = "chkCustomerWO";
            this.chkCustomerWO.Size = new System.Drawing.Size(14, 13);
            this.chkCustomerWO.TabIndex = 8;
            this.chkCustomerWO.AfterCheckStateChanged += new Infragistics.Win.CheckEditor.AfterCheckStateChangedHandler(this.chkCustomerWO_AfterCheckStateChanged);
            // 
            // chkMedia
            // 
            this.chkMedia.AutoSize = true;
            this.chkMedia.Location = new System.Drawing.Point(111, 185);
            this.chkMedia.Name = "chkMedia";
            this.chkMedia.Size = new System.Drawing.Size(14, 13);
            this.chkMedia.TabIndex = 12;
            this.chkMedia.CheckedChanged += new System.EventHandler(this.chkMedia_CheckedChanged);
            // 
            // chkPartQty
            // 
            this.chkPartQty.AutoSize = true;
            this.chkPartQty.Location = new System.Drawing.Point(111, 214);
            this.chkPartQty.Name = "chkPartQty";
            this.chkPartQty.Size = new System.Drawing.Size(14, 13);
            this.chkPartQty.TabIndex = 14;
            this.chkPartQty.CheckedChanged += new System.EventHandler(this.chkPartQty_CheckedChanged);
            // 
            // chkPriority
            // 
            this.chkPriority.AutoSize = true;
            this.chkPriority.Location = new System.Drawing.Point(111, 158);
            this.chkPriority.Name = "chkPriority";
            this.chkPriority.Size = new System.Drawing.Size(14, 13);
            this.chkPriority.TabIndex = 10;
            this.chkPriority.AfterCheckStateChanged += new Infragistics.Win.CheckEditor.AfterCheckStateChangedHandler(this.ultraCheckEditor7_AfterCheckStateChanged);
            // 
            // chkUnitPrice
            // 
            this.chkUnitPrice.AutoSize = true;
            this.chkUnitPrice.Location = new System.Drawing.Point(111, 242);
            this.chkUnitPrice.Name = "chkUnitPrice";
            this.chkUnitPrice.Size = new System.Drawing.Size(14, 13);
            this.chkUnitPrice.TabIndex = 16;
            this.chkUnitPrice.CheckedChanged += new System.EventHandler(this.chkUnitPrice_CheckedChanged);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // dtShipDate
            // 
            this.dtShipDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtShipDate.Enabled = false;
            this.dtShipDate.Location = new System.Drawing.Point(131, 74);
            this.dtShipDate.Name = "dtShipDate";
            this.dtShipDate.Size = new System.Drawing.Size(133, 22);
            this.dtShipDate.TabIndex = 5;
            this.dtShipDate.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextSection;
            ultraToolTipInfo1.ToolTipText = "The estimated shipping date of the order.";
            ultraToolTipInfo1.ToolTipTitle = "Estimated Ship Date";
            this.ultraToolTipManager.SetUltraToolTip(this.dtShipDate, ultraToolTipInfo1);
            // 
            // chkEstShipDate
            // 
            this.chkEstShipDate.AutoSize = true;
            this.chkEstShipDate.Location = new System.Drawing.Point(111, 74);
            this.chkEstShipDate.Name = "chkEstShipDate";
            this.chkEstShipDate.Size = new System.Drawing.Size(14, 13);
            this.chkEstShipDate.TabIndex = 4;
            this.chkEstShipDate.AfterCheckStateChanged += new Infragistics.Win.CheckEditor.AfterCheckStateChangedHandler(this.chkEstShipDate_AfterCheckStateChanged);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(11, 74);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(60, 15);
            this.ultraLabel3.TabIndex = 104;
            this.ultraLabel3.Text = "Est. Ship:";
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // helpLink1
            // 
            this.helpLink1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.helpLink1.BackColor = System.Drawing.Color.Transparent;
            this.helpLink1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLink1.HelpPage = "bulk_field_change_dialog.htm";
            this.helpLink1.Location = new System.Drawing.Point(12, 300);
            this.helpLink1.Name = "helpLink1";
            this.helpLink1.Size = new System.Drawing.Size(33, 16);
            this.helpLink1.TabIndex = 20;
            // 
            // OrderBulkFieldChange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(250, 325);
            this.ClientSize = new System.Drawing.Size(271, 328);
            this.Controls.Add(this.helpLink1);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.chkEstShipDate);
            this.Controls.Add(this.dtShipDate);
            this.Controls.Add(this.chkUnitPrice);
            this.Controls.Add(this.chkPriority);
            this.Controls.Add(this.chkPartQty);
            this.Controls.Add(this.chkMedia);
            this.Controls.Add(this.chkCustomerWO);
            this.Controls.Add(this.chkOrderRequiredDate);
            this.Controls.Add(this.chkOrderDate);
            this.Controls.Add(this.chkPO);
            this.Controls.Add(this.cboCustomerWO);
            this.Controls.Add(this.ultraLabel19);
            this.Controls.Add(this.ultraLabel16);
            this.Controls.Add(this.dtOrderRequiredDate);
            this.Controls.Add(this.dtOrderDate);
            this.Controls.Add(this.cboPriority);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.numPartQty);
            this.Controls.Add(this.txtMedia);
            this.Controls.Add(this.numUnitPrice);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.txtPONumber);
            this.Controls.Add(this.ultraLabel14);
            this.Controls.Add(this.ultraLabel4);
            this.Controls.Add(this.ultraLabel7);
            this.Controls.Add(this.ultraLabel5);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "OrderBulkFieldChange";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Bulk Field Change";
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomerWO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOrderRequiredDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOrderDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPriority)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMedia)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUnitPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPONumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkOrderDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkOrderRequiredDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCustomerWO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMedia)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPartQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPriority)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkUnitPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtShipDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEstShipDate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCustomerWO;
		private Infragistics.Win.Misc.UltraLabel ultraLabel19;
		private Infragistics.Win.Misc.UltraLabel ultraLabel16;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dtOrderRequiredDate;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dtOrderDate;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboPriority;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.UltraWinEditors.UltraNumericEditor numPartQty;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtMedia;
		private Infragistics.Win.UltraWinEditors.UltraCurrencyEditor numUnitPrice;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPONumber;
		private Infragistics.Win.Misc.UltraLabel ultraLabel14;
		private Infragistics.Win.Misc.UltraLabel ultraLabel4;
		private Infragistics.Win.Misc.UltraLabel ultraLabel7;
		private Infragistics.Win.Misc.UltraLabel ultraLabel5;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPO;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkOrderDate;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkOrderRequiredDate;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkCustomerWO;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkMedia;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPartQty;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPriority;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkUnitPrice;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkEstShipDate;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dtShipDate;
        private Utilities.HelpLink helpLink1;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
    }
}