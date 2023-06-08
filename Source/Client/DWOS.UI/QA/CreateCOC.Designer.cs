namespace DWOS.UI
{
	partial class CreateCOC
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
 			OnDisposing();
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo15 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Save and Print the COC.", Infragistics.Win.ToolTipImage.Default, "Complete", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The total number of parts in the order.", Infragistics.Win.ToolTipImage.Default, "Total Quantity", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The material the part is made of.", Infragistics.Win.ToolTipImage.Default, "Material", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The number of parts inspected in the order.", Infragistics.Win.ToolTipImage.Default, "Part Quantity", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The part name.", Infragistics.Win.ToolTipImage.Default, "Part Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateCOC));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo12 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The total weight of all containers in the order.", Infragistics.Win.ToolTipImage.Default, "Gross Weight", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo13 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The work order being inspected.", Infragistics.Win.ToolTipImage.Default, "Work Order", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo14 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The inspection certification number.", Infragistics.Win.ToolTipImage.Default, "Certification Number", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo10 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the parts where inspected.", Infragistics.Win.ToolTipImage.Default, "Date Inspected", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "COC Data", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo11 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The QA representative who inspected the parts.", Infragistics.Win.ToolTipImage.Default, "Quality Inspector", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.StateEditorButton stateEditorButton1 = new Infragistics.Win.UltraWinEditors.StateEditorButton("chkQuickPrint");
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Quick Print", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("If checked, allows you to view the COC before printing it.", Infragistics.Win.ToolTipImage.Default, "View", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Shows a preview of the COC.", Infragistics.Win.ToolTipImage.Default, "Preview", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Enter the email addresses that COC notifications should be sent to.", Infragistics.Win.ToolTipImage.Default, "Email COC To", Infragistics.Win.DefaultableBoolean.Default);
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.grpPart = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.numOrderQty = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.partMediaWidget = new DWOS.UI.Utilities.MediaWidget();
            this.txtPartID = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.numPartQty = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.bsCOC = new System.Windows.Forms.BindingSource(this.components);
            this.dsCOC = new DWOS.Data.Datasets.COCDataset();
            this.txtPartName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.grpOrder = new Infragistics.Win.Misc.UltraGroupBox();
            this.numGrossWeight = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.orderMediaWidget = new DWOS.UI.Utilities.MediaWidget();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraTextEditor3 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtCertNumber = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.dteDateCertified = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.grpInspection = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtCOCInfo = new Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor();
            this.cboInspector = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.numPrintQty = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.chkPdf = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.btnPreview = new Infragistics.Win.Misc.UltraButton();
            this.cboNotifications = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.lblQuickPrint = new Infragistics.Win.Misc.UltraLabel();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.taCOC = new DWOS.Data.Datasets.COCDatasetTableAdapters.COCTableAdapter();
            this.taManager = new DWOS.Data.Datasets.COCDatasetTableAdapters.TableAdapterManager();
            this.taNotification = new DWOS.Data.Datasets.COCDatasetTableAdapters.COCNotificationTableAdapter();
            this.taContact = new DWOS.Data.Datasets.COCDatasetTableAdapters.d_ContactTableAdapter();
            this.taMedia = new DWOS.Data.Datasets.COCDatasetTableAdapters.MediaTableAdapter();
            this.taOrder_DocumentLink = new DWOS.Data.Datasets.COCDatasetTableAdapters.Order_DocumentLinkTableAdapter();
            this.taOrder_Media = new DWOS.Data.Datasets.COCDatasetTableAdapters.Order_MediaTableAdapter();
            this.taPart_DocumentLink = new DWOS.Data.Datasets.COCDatasetTableAdapters.Part_DocumentLinkTableAdapter();
            this.taPart_Media = new DWOS.Data.Datasets.COCDatasetTableAdapters.Part_MediaTableAdapter();
            this.taCOCPart = new DWOS.Data.Datasets.COCDatasetTableAdapters.COCPartTableAdapter();
            this.taUsers = new DWOS.Data.Datasets.COCDatasetTableAdapters.UsersTableAdapter();
            this.taProcess = new DWOS.Data.Datasets.COCDatasetTableAdapters.ProcessTableAdapter();
            this.taOrderSummary = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter();
            this.taProcessQuestion = new DWOS.Data.Datasets.COCDatasetTableAdapters.ProcessQuestionTableAdapter();
            this.taProcessSteps = new DWOS.Data.Datasets.COCDatasetTableAdapters.ProcessStepsTableAdapter();
            this.taOrderProcesses = new DWOS.Data.Datasets.COCDatasetTableAdapters.OrderProcessesTableAdapter();
            this.taOrderProcessAnswer = new DWOS.Data.Datasets.COCDatasetTableAdapters.OrderProcessAnswerTableAdapter();
            this.taReworkSummary = new DWOS.Data.Datasets.COCDatasetTableAdapters.ReworkSummaryTableAdapter();
            this.helpLink1 = new DWOS.UI.Utilities.HelpLink();
            ((System.ComponentModel.ISupportInitialize)(this.grpPart)).BeginInit();
            this.grpPart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numOrderQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsCOC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsCOC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpOrder)).BeginInit();
            this.grpOrder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGrossWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditor3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCertNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDateCertified)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpInspection)).BeginInit();
            this.grpInspection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboInspector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrintQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPdf)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboNotifications)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(640, 510);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(558, 510);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "&Complete";
            ultraToolTipInfo15.ToolTipText = "Save and Print the COC.";
            ultraToolTipInfo15.ToolTipTitle = "Complete";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnOK, ultraToolTipInfo15);
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grpPart
            // 
            this.grpPart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPart.Controls.Add(this.ultraLabel4);
            this.grpPart.Controls.Add(this.numOrderQty);
            this.grpPart.Controls.Add(this.partMediaWidget);
            this.grpPart.Controls.Add(this.txtPartID);
            this.grpPart.Controls.Add(this.ultraLabel1);
            this.grpPart.Controls.Add(this.numPartQty);
            this.grpPart.Controls.Add(this.txtPartName);
            appearance3.Image = global::DWOS.UI.Properties.Resources.Part_16;
            this.grpPart.HeaderAppearance = appearance3;
            this.grpPart.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.grpPart.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.grpPart.Location = new System.Drawing.Point(4, 140);
            this.grpPart.Name = "grpPart";
            this.grpPart.Size = new System.Drawing.Size(714, 157);
            this.grpPart.TabIndex = 1;
            this.grpPart.Text = "Part";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(9, 114);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(62, 15);
            this.ultraLabel4.TabIndex = 11;
            this.ultraLabel4.Text = "Total Qty:";
            // 
            // numOrderQty
            // 
            this.numOrderQty.Location = new System.Drawing.Point(110, 110);
            this.numOrderQty.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.numOrderQty.MaskInput = "nnn,nnn,nnn";
            this.numOrderQty.MaxValue = 999999999;
            this.numOrderQty.MinValue = 0;
            this.numOrderQty.Name = "numOrderQty";
            this.numOrderQty.NullText = "0";
            this.numOrderQty.PromptChar = '0';
            this.numOrderQty.ReadOnly = true;
            this.numOrderQty.Size = new System.Drawing.Size(117, 22);
            this.numOrderQty.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.numOrderQty.TabIndex = 3;
            ultraToolTipInfo5.ToolTipText = "The total number of parts in the order.";
            ultraToolTipInfo5.ToolTipTitle = "Total Quantity";
            this.ultraToolTipManager1.SetUltraToolTip(this.numOrderQty, ultraToolTipInfo5);
            this.numOrderQty.ValueChanged += new System.EventHandler(this.numOrderQty_ValueChanged);
            // 
            // partMediaWidget
            // 
            this.partMediaWidget.AllowEditing = false;
            this.partMediaWidget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.partMediaWidget.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.partMediaWidget.Location = new System.Drawing.Point(233, 26);
            this.partMediaWidget.Name = "partMediaWidget";
            this.partMediaWidget.Size = new System.Drawing.Size(476, 125);
            this.partMediaWidget.TabIndex = 8;
            // 
            // txtPartID
            // 
            this.txtPartID.Location = new System.Drawing.Point(9, 54);
            this.txtPartID.Name = "txtPartID";
            this.txtPartID.NullText = "Material";
            appearance1.FontData.ItalicAsString = "True";
            appearance1.ForeColor = System.Drawing.Color.DarkGray;
            this.txtPartID.NullTextAppearance = appearance1;
            this.txtPartID.ReadOnly = true;
            this.txtPartID.Size = new System.Drawing.Size(218, 22);
            this.txtPartID.TabIndex = 1;
            ultraToolTipInfo6.ToolTipText = "The material the part is made of.";
            ultraToolTipInfo6.ToolTipTitle = "Material";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtPartID, ultraToolTipInfo6);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(9, 88);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(85, 15);
            this.ultraLabel1.TabIndex = 9;
            this.ultraLabel1.Text = "Accepted Qty:";
            // 
            // numPartQty
            // 
            this.numPartQty.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bsCOC, "PartQuantity", true));
            this.numPartQty.Location = new System.Drawing.Point(110, 82);
            this.numPartQty.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.numPartQty.MaskInput = "nnn,nnn,nnn";
            this.numPartQty.MinValue = 0;
            this.numPartQty.Name = "numPartQty";
            this.numPartQty.NullText = "0";
            this.numPartQty.PromptChar = '0';
            this.numPartQty.Size = new System.Drawing.Size(117, 22);
            this.numPartQty.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.numPartQty.TabIndex = 2;
            ultraToolTipInfo7.ToolTipText = "The number of parts inspected in the order.";
            ultraToolTipInfo7.ToolTipTitle = "Part Quantity";
            this.ultraToolTipManager1.SetUltraToolTip(this.numPartQty, ultraToolTipInfo7);
            this.numPartQty.Value = 1000;
            this.numPartQty.ValueChanged += new System.EventHandler(this.numPartQty_ValueChanged);
            // 
            // bsCOC
            // 
            this.bsCOC.DataMember = "COC";
            this.bsCOC.DataSource = this.dsCOC;
            // 
            // dsCOC
            // 
            this.dsCOC.DataSetName = "COCDataset";
            this.dsCOC.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // txtPartName
            // 
            this.txtPartName.Location = new System.Drawing.Point(9, 26);
            this.txtPartName.Name = "txtPartName";
            this.txtPartName.NullText = "Part Name";
            appearance2.FontData.ItalicAsString = "True";
            appearance2.ForeColor = System.Drawing.Color.DarkGray;
            this.txtPartName.NullTextAppearance = appearance2;
            this.txtPartName.ReadOnly = true;
            this.txtPartName.Size = new System.Drawing.Size(218, 22);
            this.txtPartName.TabIndex = 0;
            ultraToolTipInfo8.ToolTipText = "The part name.";
            ultraToolTipInfo8.ToolTipTitle = "Part Name";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtPartName, ultraToolTipInfo8);
            // 
            // grpOrder
            // 
            this.grpOrder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpOrder.Controls.Add(this.numGrossWeight);
            this.grpOrder.Controls.Add(this.ultraLabel7);
            this.grpOrder.Controls.Add(this.orderMediaWidget);
            this.grpOrder.Controls.Add(this.ultraLabel3);
            this.grpOrder.Controls.Add(this.ultraLabel2);
            this.grpOrder.Controls.Add(this.ultraTextEditor3);
            this.grpOrder.Controls.Add(this.txtCertNumber);
            appearance6.Image = ((object)(resources.GetObject("appearance6.Image")));
            this.grpOrder.HeaderAppearance = appearance6;
            this.grpOrder.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.grpOrder.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.grpOrder.Location = new System.Drawing.Point(4, 6);
            this.grpOrder.Name = "grpOrder";
            this.grpOrder.Size = new System.Drawing.Size(714, 128);
            this.grpOrder.TabIndex = 0;
            this.grpOrder.Text = "Order";
            // 
            // numGrossWeight
            // 
            this.numGrossWeight.FormatString = "";
            this.numGrossWeight.Location = new System.Drawing.Point(105, 84);
            this.numGrossWeight.MaskInput = "nnn,nnn.nnnnnnnn";
            this.numGrossWeight.MaxValue = new decimal(new int[] {
            276447231,
            23283,
            0,
            524288});
            this.numGrossWeight.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numGrossWeight.Name = "numGrossWeight";
            this.numGrossWeight.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.numGrossWeight.ReadOnly = true;
            this.numGrossWeight.Size = new System.Drawing.Size(122, 22);
            this.numGrossWeight.TabIndex = 2;
            ultraToolTipInfo12.ToolTipText = "The total weight of all containers in the order.";
            ultraToolTipInfo12.ToolTipTitle = "Gross Weight";
            this.ultraToolTipManager1.SetUltraToolTip(this.numGrossWeight, ultraToolTipInfo12);
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(14, 88);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(85, 15);
            this.ultraLabel7.TabIndex = 20;
            this.ultraLabel7.Text = "Gross Weight:";
            // 
            // orderMediaWidget
            // 
            this.orderMediaWidget.AllowEditing = false;
            this.orderMediaWidget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.orderMediaWidget.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.orderMediaWidget.Location = new System.Drawing.Point(233, 28);
            this.orderMediaWidget.Name = "orderMediaWidget";
            this.orderMediaWidget.Size = new System.Drawing.Size(473, 94);
            this.orderMediaWidget.TabIndex = 3;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(14, 60);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(83, 15);
            this.ultraLabel3.TabIndex = 18;
            this.ultraLabel3.Text = "Cert Number:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(14, 32);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(75, 15);
            this.ultraLabel2.TabIndex = 17;
            this.ultraLabel2.Text = "Work Order:";
            // 
            // ultraTextEditor3
            // 
            this.ultraTextEditor3.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bsCOC, "OrderID", true));
            this.ultraTextEditor3.Location = new System.Drawing.Point(105, 28);
            this.ultraTextEditor3.Name = "ultraTextEditor3";
            this.ultraTextEditor3.ReadOnly = true;
            this.ultraTextEditor3.Size = new System.Drawing.Size(122, 22);
            this.ultraTextEditor3.TabIndex = 0;
            this.ultraTextEditor3.Text = "999999";
            ultraToolTipInfo13.ToolTipText = "The work order being inspected.";
            ultraToolTipInfo13.ToolTipTitle = "Work Order";
            this.ultraToolTipManager1.SetUltraToolTip(this.ultraTextEditor3, ultraToolTipInfo13);
            // 
            // txtCertNumber
            // 
            this.txtCertNumber.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bsCOC, "COCID", true));
            this.txtCertNumber.Location = new System.Drawing.Point(105, 56);
            this.txtCertNumber.Name = "txtCertNumber";
            this.txtCertNumber.ReadOnly = true;
            this.txtCertNumber.Size = new System.Drawing.Size(122, 22);
            this.txtCertNumber.TabIndex = 1;
            ultraToolTipInfo14.ToolTipText = "The inspection certification number.";
            ultraToolTipInfo14.ToolTipTitle = "Certification Number";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtCertNumber, ultraToolTipInfo14);
            // 
            // dteDateCertified
            // 
            this.dteDateCertified.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bsCOC, "DateCertified", true));
            this.dteDateCertified.DateTime = new System.DateTime(2008, 12, 26, 0, 0, 0, 0);
            this.dteDateCertified.Location = new System.Drawing.Point(474, 29);
            this.dteDateCertified.Name = "dteDateCertified";
            this.dteDateCertified.Size = new System.Drawing.Size(144, 22);
            this.dteDateCertified.TabIndex = 1;
            ultraToolTipInfo10.ToolTipText = "The date the parts where inspected.";
            ultraToolTipInfo10.ToolTipTitle = "Date Inspected";
            this.ultraToolTipManager1.SetUltraToolTip(this.dteDateCertified, ultraToolTipInfo10);
            this.dteDateCertified.Value = new System.DateTime(2008, 12, 26, 0, 0, 0, 0);
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(373, 33);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(88, 15);
            this.ultraLabel6.TabIndex = 23;
            this.ultraLabel6.Text = "Date Certified:";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(14, 33);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(107, 15);
            this.ultraLabel5.TabIndex = 21;
            this.ultraLabel5.Text = "Quality Inspector:";
            // 
            // grpInspection
            // 
            this.grpInspection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpInspection.ContentPadding.Bottom = 5;
            this.grpInspection.ContentPadding.Left = 5;
            this.grpInspection.ContentPadding.Right = 5;
            this.grpInspection.Controls.Add(this.txtCOCInfo);
            this.grpInspection.Controls.Add(this.dteDateCertified);
            this.grpInspection.Controls.Add(this.ultraLabel6);
            this.grpInspection.Controls.Add(this.cboInspector);
            this.grpInspection.Controls.Add(this.ultraLabel5);
            appearance5.Image = global::DWOS.UI.Properties.Resources.Certificate_16;
            this.grpInspection.HeaderAppearance = appearance5;
            this.grpInspection.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.grpInspection.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.grpInspection.Location = new System.Drawing.Point(4, 303);
            this.grpInspection.MinimumSize = new System.Drawing.Size(655, 0);
            this.grpInspection.Name = "grpInspection";
            this.grpInspection.Size = new System.Drawing.Size(714, 163);
            this.grpInspection.TabIndex = 2;
            this.grpInspection.Text = "Certificate of Conformance";
            // 
            // txtCOCInfo
            // 
            this.txtCOCInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance4.FontData.BoldAsString = "False";
            appearance4.FontData.ItalicAsString = "False";
            appearance4.FontData.Name = "Verdana";
            appearance4.FontData.SizeInPoints = 8.25F;
            appearance4.FontData.StrikeoutAsString = "False";
            appearance4.FontData.UnderlineAsString = "False";
            this.txtCOCInfo.Appearance = appearance4;
            this.txtCOCInfo.ContextMenuItems = ((Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems)(((((((((((((((((Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Cut | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Copy) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Paste) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Delete) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Undo) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Redo) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.SelectAll) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Font) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Image) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Link) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.LineAlignment) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Paragraph) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Bold) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Italics) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Underline) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.SpellingSuggestions) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Reserved)));
            this.txtCOCInfo.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bsCOC, "COCInfo", true));
            this.txtCOCInfo.Location = new System.Drawing.Point(8, 59);
            this.txtCOCInfo.Name = "txtCOCInfo";
            this.txtCOCInfo.Size = new System.Drawing.Size(695, 93);
            this.txtCOCInfo.TabIndex = 2;
            this.txtCOCInfo.TextSectionBreakMode = Infragistics.Win.FormattedLinkLabel.TextSectionBreakMode.Word;
            this.txtCOCInfo.TextSmoothingMode = Infragistics.Win.FormattedLinkLabel.TextSmoothingMode.SystemSettings;
            ultraToolTipInfo9.ToolTipTextFormatted = "Displays information that will be on the the Certificate of Conformance.<br/><br/" +
    ">Right-Click to view additional editing options.<br/>";
            ultraToolTipInfo9.ToolTipTitle = "COC Data";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtCOCInfo, ultraToolTipInfo9);
            this.txtCOCInfo.Value = "";
            // 
            // cboInspector
            // 
            this.cboInspector.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bsCOC, "QAUser", true));
            this.cboInspector.DataMember = "Users";
            this.cboInspector.DataSource = this.dsCOC;
            this.cboInspector.DisplayMember = "Name";
            this.cboInspector.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboInspector.Location = new System.Drawing.Point(127, 29);
            this.cboInspector.Name = "cboInspector";
            this.cboInspector.ReadOnly = true;
            this.cboInspector.Size = new System.Drawing.Size(225, 22);
            this.cboInspector.TabIndex = 0;
            ultraToolTipInfo11.ToolTipText = "The QA representative who inspected the parts.";
            ultraToolTipInfo11.ToolTipTitle = "Quality Inspector";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboInspector, ultraToolTipInfo11);
            this.cboInspector.ValueMember = "UserID";
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // numPrintQty
            // 
            this.numPrintQty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            stateEditorButton1.CheckState = System.Windows.Forms.CheckState.Checked;
            stateEditorButton1.Key = "chkQuickPrint";
            this.numPrintQty.ButtonsLeft.Add(stateEditorButton1);
            this.numPrintQty.Location = new System.Drawing.Point(633, 471);
            this.numPrintQty.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.numPrintQty.MaskInput = "nnn,nnn";
            this.numPrintQty.MaxValue = 10;
            this.numPrintQty.MinValue = 1;
            this.numPrintQty.Name = "numPrintQty";
            this.numPrintQty.Size = new System.Drawing.Size(74, 22);
            this.numPrintQty.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.numPrintQty.TabIndex = 5;
            ultraToolTipInfo4.ToolTipTextFormatted = "Check the box to allow quick print to the default printer.<br/>Set the number of " +
    "copies to be printed.<br/><br/>";
            ultraToolTipInfo4.ToolTipTitle = "Quick Print";
            this.ultraToolTipManager1.SetUltraToolTip(this.numPrintQty, ultraToolTipInfo4);
            this.numPrintQty.Value = 2;
            this.numPrintQty.AfterEditorButtonCheckStateChanged += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.numPrintQty_AfterEditorButtonCheckStateChanged);
            // 
            // chkPdf
            // 
            this.chkPdf.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPdf.AutoSize = true;
            this.chkPdf.Location = new System.Drawing.Point(428, 470);
            this.chkPdf.Name = "chkPdf";
            this.chkPdf.Size = new System.Drawing.Size(77, 18);
            this.chkPdf.TabIndex = 4;
            this.chkPdf.Text = "View COC";
            ultraToolTipInfo3.ToolTipText = "If checked, allows you to view the COC before printing it.";
            ultraToolTipInfo3.ToolTipTitle = "View";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkPdf, ultraToolTipInfo3);
            this.chkPdf.CheckedChanged += new System.EventHandler(this.chkPdf_CheckedChanged);
            // 
            // btnPreview
            // 
            this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPreview.Location = new System.Drawing.Point(51, 510);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 23);
            this.btnPreview.TabIndex = 6;
            this.btnPreview.Text = "Preview";
            ultraToolTipInfo2.ToolTipText = "Shows a preview of the COC.";
            ultraToolTipInfo2.ToolTipTitle = "Preview";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnPreview, ultraToolTipInfo2);
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // cboNotifications
            // 
            this.cboNotifications.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboNotifications.CheckedListSettings.CheckBoxStyle = Infragistics.Win.CheckStyle.CheckBox;
            this.cboNotifications.CheckedListSettings.EditorValueSource = Infragistics.Win.EditorWithComboValueSource.CheckedItems;
            this.cboNotifications.CheckedListSettings.ItemCheckArea = Infragistics.Win.ItemCheckArea.Item;
            this.cboNotifications.CheckedListSettings.ListSeparator = ",";
            this.cboNotifications.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboNotifications.Location = new System.Drawing.Point(106, 471);
            this.cboNotifications.Name = "cboNotifications";
            this.cboNotifications.NullText = "<None>";
            this.cboNotifications.Size = new System.Drawing.Size(257, 22);
            this.cboNotifications.TabIndex = 3;
            ultraToolTipInfo1.ToolTipText = "Enter the email addresses that COC notifications should be sent to.";
            ultraToolTipInfo1.ToolTipTitle = "Email COC To";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboNotifications, ultraToolTipInfo1);
            // 
            // lblQuickPrint
            // 
            this.lblQuickPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblQuickPrint.AutoSize = true;
            this.lblQuickPrint.Location = new System.Drawing.Point(563, 475);
            this.lblQuickPrint.Name = "lblQuickPrint";
            this.lblQuickPrint.Size = new System.Drawing.Size(64, 15);
            this.lblQuickPrint.TabIndex = 43;
            this.lblQuickPrint.Text = "Print COC:";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(13, 475);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(87, 15);
            this.ultraLabel8.TabIndex = 45;
            this.ultraLabel8.Text = "Email COC To:";
            // 
            // taCOC
            // 
            this.taCOC.ClearBeforeFill = true;
            // 
            // taManager
            // 
            this.taManager.BackupDataSetBeforeUpdate = false;
            this.taManager.BatchCOCNotificationTableAdapter = null;
            this.taManager.BatchCOCOrderTableAdapter = null;
            this.taManager.BatchCOCTableAdapter = null;
            this.taManager.BulkCOCNotificationTableAdapter = null;
            this.taManager.BulkCOCOrderTableAdapter = null;
            this.taManager.BulkCOCTableAdapter = null;
            this.taManager.COCNotificationTableAdapter = this.taNotification;
            this.taManager.COCTableAdapter = this.taCOC;
            this.taManager.d_ContactTableAdapter = this.taContact;
            this.taManager.MediaTableAdapter = this.taMedia;
            this.taManager.Order_DocumentLinkTableAdapter = this.taOrder_DocumentLink;
            this.taManager.Order_MediaTableAdapter = this.taOrder_Media;
            this.taManager.OrderSerialNumberTableAdapter = null;
            this.taManager.Part_DocumentLinkTableAdapter = this.taPart_DocumentLink;
            this.taManager.Part_MediaTableAdapter = this.taPart_Media;
            this.taManager.UpdateOrder = DWOS.Data.Datasets.COCDatasetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            this.taManager.UsersTableAdapter = null;
            // 
            // taNotification
            // 
            this.taNotification.ClearBeforeFill = true;
            // 
            // taContact
            // 
            this.taContact.ClearBeforeFill = false;
            // 
            // taMedia
            // 
            this.taMedia.ClearBeforeFill = false;
            // 
            // taOrder_DocumentLink
            // 
            this.taOrder_DocumentLink.ClearBeforeFill = true;
            // 
            // taOrder_Media
            // 
            this.taOrder_Media.ClearBeforeFill = true;
            // 
            // taPart_DocumentLink
            // 
            this.taPart_DocumentLink.ClearBeforeFill = true;
            // 
            // taPart_Media
            // 
            this.taPart_Media.ClearBeforeFill = true;
            // 
            // taCOCPart
            // 
            this.taCOCPart.ClearBeforeFill = true;
            // 
            // taUsers
            // 
            this.taUsers.ClearBeforeFill = true;
            // 
            // taProcess
            // 
            this.taProcess.ClearBeforeFill = true;
            // 
            // taOrderSummary
            // 
            this.taOrderSummary.ClearBeforeFill = false;
            // 
            // taProcessQuestion
            // 
            this.taProcessQuestion.ClearBeforeFill = true;
            // 
            // taProcessSteps
            // 
            this.taProcessSteps.ClearBeforeFill = true;
            // 
            // taOrderProcesses
            // 
            this.taOrderProcesses.ClearBeforeFill = true;
            // 
            // taOrderProcessAnswer
            // 
            this.taOrderProcessAnswer.ClearBeforeFill = true;
            // 
            // taReworkSummary
            // 
            this.taReworkSummary.ClearBeforeFill = false;
            // 
            // helpLink1
            // 
            this.helpLink1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.helpLink1.BackColor = System.Drawing.Color.Transparent;
            this.helpLink1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLink1.HelpPage = "final_inspection_dialog.htm";
            this.helpLink1.Location = new System.Drawing.Point(12, 516);
            this.helpLink1.Name = "helpLink1";
            this.helpLink1.Size = new System.Drawing.Size(33, 16);
            this.helpLink1.TabIndex = 44;
            // 
            // CreateCOC
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(725, 538);
            this.Controls.Add(this.cboNotifications);
            this.Controls.Add(this.ultraLabel8);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.chkPdf);
            this.Controls.Add(this.helpLink1);
            this.Controls.Add(this.lblQuickPrint);
            this.Controls.Add(this.numPrintQty);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpPart);
            this.Controls.Add(this.grpInspection);
            this.Controls.Add(this.grpOrder);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "CreateCOC";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Final Inspection";
            this.Load += new System.EventHandler(this.CreateCOC_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grpPart)).EndInit();
            this.grpPart.ResumeLayout(false);
            this.grpPart.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numOrderQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsCOC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsCOC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpOrder)).EndInit();
            this.grpOrder.ResumeLayout(false);
            this.grpOrder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGrossWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditor3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCertNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDateCertified)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpInspection)).EndInit();
            this.grpInspection.ResumeLayout(false);
            this.grpInspection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboInspector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrintQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPdf)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboNotifications)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.Misc.UltraGroupBox grpPart;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPartID;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.UltraWinEditors.UltraNumericEditor numPartQty;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPartName;
		private Infragistics.Win.Misc.UltraGroupBox grpOrder;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor ultraTextEditor3;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCertNumber;
		private Infragistics.Win.Misc.UltraLabel ultraLabel3;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboInspector;
		private Infragistics.Win.Misc.UltraLabel ultraLabel5;
		private Infragistics.Win.Misc.UltraLabel ultraLabel6;
		private Infragistics.Win.Misc.UltraGroupBox grpInspection;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
		private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
		private DWOS.Data.Datasets.COCDataset dsCOC;
		private DWOS.Data.Datasets.COCDatasetTableAdapters.COCTableAdapter taCOC;
		private DWOS.Data.Datasets.COCDatasetTableAdapters.TableAdapterManager taManager;
		private System.Windows.Forms.BindingSource bsCOC;
		private DWOS.Data.Datasets.COCDatasetTableAdapters.COCPartTableAdapter taCOCPart;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteDateCertified;
		private DWOS.Data.Datasets.COCDatasetTableAdapters.UsersTableAdapter taUsers;
		private DWOS.Data.Datasets.COCDatasetTableAdapters.ProcessTableAdapter taProcess;
		private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter taOrderSummary;
		private DWOS.Data.Datasets.COCDatasetTableAdapters.ProcessQuestionTableAdapter taProcessQuestion;
		private DWOS.Data.Datasets.COCDatasetTableAdapters.ProcessStepsTableAdapter taProcessSteps;
		private Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor txtCOCInfo;
		private Infragistics.Win.Misc.UltraLabel lblQuickPrint;
		private Infragistics.Win.UltraWinEditors.UltraNumericEditor numPrintQty;
		private DWOS.Data.Datasets.COCDatasetTableAdapters.OrderProcessesTableAdapter taOrderProcesses;
		private DWOS.Data.Datasets.COCDatasetTableAdapters.OrderProcessAnswerTableAdapter taOrderProcessAnswer;
        private Utilities.HelpLink helpLink1;
        private Data.Datasets.COCDatasetTableAdapters.MediaTableAdapter taMedia;
        private Data.Datasets.COCDatasetTableAdapters.Part_MediaTableAdapter taPart_Media;
        private Data.Datasets.COCDatasetTableAdapters.Order_MediaTableAdapter taOrder_Media;
        private Utilities.MediaWidget partMediaWidget;
        private Data.Datasets.COCDatasetTableAdapters.Part_DocumentLinkTableAdapter taPart_DocumentLink;
        private Utilities.MediaWidget orderMediaWidget;
        private Data.Datasets.COCDatasetTableAdapters.Order_DocumentLinkTableAdapter taOrder_DocumentLink;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numOrderQty;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numGrossWeight;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPdf;
        private Infragistics.Win.Misc.UltraButton btnPreview;
        private Data.Datasets.COCDatasetTableAdapters.ReworkSummaryTableAdapter taReworkSummary;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboNotifications;
        private Infragistics.Win.Misc.UltraLabel ultraLabel8;
        private Data.Datasets.COCDatasetTableAdapters.d_ContactTableAdapter taContact;
        private Data.Datasets.COCDatasetTableAdapters.COCNotificationTableAdapter taNotification;
    }
}