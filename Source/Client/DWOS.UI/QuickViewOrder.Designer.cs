namespace DWOS.UI
{
	partial class QuickViewOrder
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
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuickViewOrder));
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.pnlOrderInfo = new DWOS.UI.Sales.OrderInformation();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.pnlOrderProcessingInfo = new DWOS.UI.Sales.OrderProcessingInfo();
            this.ultraTabPageControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.pnlSerialNumbers = new DWOS.UI.Sales.Order.SerialNumberQuickViewPanel();
            this.ultraTabPageControl4 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.notesQuickViewPanel = new DWOS.UI.Sales.Order.NotesQuickViewPanel();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.dsOrders = new DWOS.Data.Datasets.OrdersDataSet();
            this.taOrder = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter();
            this.taPartSummary = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.PartSummaryTableAdapter();
            this.taMedia = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter();
            this.taCustomerSummary = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.CustomerSummaryTableAdapter();
            this.taPriority = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.d_PriorityTableAdapter();
            this.taOrderStatus = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.d_OrderStatusTableAdapter();
            this.taUserSummary = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.UserSummaryTableAdapter();
            this.taPriceUnit = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.PriceUnitTableAdapter();
            this.taCustomerShippingSummary = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.CustomerShippingSummaryTableAdapter();
            this.btnPrint = new Infragistics.Win.Misc.UltraButton();
            this.QuickViewOrder_Fill_Panel = new System.Windows.Forms.Panel();
            this.ultraTabControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.taOrderFees = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderFeesTableAdapter();
            this.taOrderProcesses = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderProcessesTableAdapter();
            this.taOrder_Media = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.Order_MediaTableAdapter();
            this.taOrderFeeType = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderFeeTypeTableAdapter();
            this.taOrderDocumentLink = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.Order_DocumentLinkTableAdapter();
            this.taCustomerAddress = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.CustomerAddressTableAdapter();
            this.taOrderSerialNumber = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderSerialNumberTableAdapter();
            this.taProcessingLine = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.ProcessingLineTableAdapter();
            this.taManager = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.TableAdapterManager();
            this.taOrderNote = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderNoteTableAdapter();
            this.taWorkDescription = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.WorkDescriptionTableAdapter();
            this.taOrderWorkDescription = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderWorkDescriptionTableAdapter();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.persistWindowState = new DWOS.UI.Utilities.PersistWindowState(this.components);
            this.ultraTabPageControl1.SuspendLayout();
            this.ultraTabPageControl2.SuspendLayout();
            this.ultraTabPageControl3.SuspendLayout();
            this.ultraTabPageControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrders)).BeginInit();
            this.QuickViewOrder_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl1)).BeginInit();
            this.ultraTabControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.AutoScroll = true;
            this.ultraTabPageControl1.Controls.Add(this.pnlOrderInfo);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(456, 561);
            // 
            // pnlOrderInfo
            // 
            this.pnlOrderInfo.AutoScroll = true;
            this.pnlOrderInfo.CurrentCustomerID = 0;
            this.pnlOrderInfo.Dataset = null;
            this.pnlOrderInfo.DisableCustomerSelection = false;
            this.pnlOrderInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOrderInfo.Editable = true;
            this.pnlOrderInfo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlOrderInfo.IsActivePanel = false;
            this.pnlOrderInfo.IsQuickView = false;
            this.pnlOrderInfo.Location = new System.Drawing.Point(0, 0);
            this.pnlOrderInfo.MinimumSize = new System.Drawing.Size(425, 800);
            this.pnlOrderInfo.Name = "pnlOrderInfo";
            this.pnlOrderInfo.Padding = new System.Windows.Forms.Padding(5);
            this.pnlOrderInfo.PartsLoading = false;
            this.pnlOrderInfo.Size = new System.Drawing.Size(456, 800);
            this.pnlOrderInfo.TabIndex = 16;
            this.pnlOrderInfo.ViewOnly = false;
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.pnlOrderProcessingInfo);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(456, 561);
            // 
            // pnlOrderProcessingInfo
            // 
            this.pnlOrderProcessingInfo.AutoScroll = true;
            this.pnlOrderProcessingInfo.Dataset = null;
            this.pnlOrderProcessingInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOrderProcessingInfo.Editable = true;
            this.pnlOrderProcessingInfo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlOrderProcessingInfo.IsActivePanel = false;
            this.pnlOrderProcessingInfo.Location = new System.Drawing.Point(0, 0);
            this.pnlOrderProcessingInfo.Name = "pnlOrderProcessingInfo";
            this.pnlOrderProcessingInfo.Padding = new System.Windows.Forms.Padding(5);
            this.pnlOrderProcessingInfo.Size = new System.Drawing.Size(456, 561);
            this.pnlOrderProcessingInfo.TabIndex = 19;
            this.pnlOrderProcessingInfo.ViewOnly = false;
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Controls.Add(this.pnlSerialNumbers);
            this.ultraTabPageControl3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl3.Name = "ultraTabPageControl3";
            this.ultraTabPageControl3.Size = new System.Drawing.Size(456, 561);
            // 
            // pnlSerialNumbers
            // 
            this.pnlSerialNumbers.Dataset = null;
            this.pnlSerialNumbers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSerialNumbers.Editable = true;
            this.pnlSerialNumbers.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlSerialNumbers.IsActivePanel = false;
            this.pnlSerialNumbers.Location = new System.Drawing.Point(0, 0);
            this.pnlSerialNumbers.Name = "pnlSerialNumbers";
            this.pnlSerialNumbers.Padding = new System.Windows.Forms.Padding(3);
            this.pnlSerialNumbers.Size = new System.Drawing.Size(456, 561);
            this.pnlSerialNumbers.TabIndex = 0;
            // 
            // ultraTabPageControl4
            // 
            this.ultraTabPageControl4.Controls.Add(this.notesQuickViewPanel);
            this.ultraTabPageControl4.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl4.Name = "ultraTabPageControl4";
            this.ultraTabPageControl4.Size = new System.Drawing.Size(456, 561);
            // 
            // notesQuickViewPanel
            // 
            this.notesQuickViewPanel.Dataset = null;
            this.notesQuickViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.notesQuickViewPanel.Editable = true;
            this.notesQuickViewPanel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.notesQuickViewPanel.IsActivePanel = false;
            this.notesQuickViewPanel.Location = new System.Drawing.Point(0, 0);
            this.notesQuickViewPanel.Name = "notesQuickViewPanel";
            this.notesQuickViewPanel.Padding = new System.Windows.Forms.Padding(3);
            this.notesQuickViewPanel.Size = new System.Drawing.Size(456, 561);
            this.notesQuickViewPanel.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(383, 605);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 15;
            this.btnOK.Text = "Close";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // dsOrders
            // 
            this.dsOrders.DataSetName = "OrdersDataSet";
            this.dsOrders.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taOrder
            // 
            this.taOrder.ClearBeforeFill = true;
            // 
            // taPartSummary
            // 
            this.taPartSummary.ClearBeforeFill = true;
            // 
            // taMedia
            // 
            this.taMedia.ClearBeforeFill = false;
            // 
            // taCustomerSummary
            // 
            this.taCustomerSummary.ClearBeforeFill = true;
            // 
            // taPriority
            // 
            this.taPriority.ClearBeforeFill = true;
            // 
            // taOrderStatus
            // 
            this.taOrderStatus.ClearBeforeFill = true;
            // 
            // taUserSummary
            // 
            this.taUserSummary.ClearBeforeFill = true;
            // 
            // taPriceUnit
            // 
            this.taPriceUnit.ClearBeforeFill = true;
            // 
            // taCustomerShippingSummary
            // 
            this.taCustomerShippingSummary.ClearBeforeFill = true;
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrint.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnPrint.Location = new System.Drawing.Point(12, 605);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(76, 23);
            this.btnPrint.TabIndex = 18;
            this.btnPrint.Text = "Print";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // QuickViewOrder_Fill_Panel
            // 
            this.QuickViewOrder_Fill_Panel.Controls.Add(this.ultraTabControl1);
            this.QuickViewOrder_Fill_Panel.Controls.Add(this.btnPrint);
            this.QuickViewOrder_Fill_Panel.Controls.Add(this.btnOK);
            this.QuickViewOrder_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.QuickViewOrder_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.QuickViewOrder_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.QuickViewOrder_Fill_Panel.Name = "QuickViewOrder_Fill_Panel";
            this.QuickViewOrder_Fill_Panel.Size = new System.Drawing.Size(484, 640);
            this.QuickViewOrder_Fill_Panel.TabIndex = 0;
            // 
            // ultraTabControl1
            // 
            this.ultraTabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraTabControl1.Controls.Add(this.ultraTabSharedControlsPage1);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl1);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl2);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl3);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl4);
            this.ultraTabControl1.Location = new System.Drawing.Point(12, 12);
            this.ultraTabControl1.Name = "ultraTabControl1";
            this.ultraTabControl1.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.ultraTabControl1.Size = new System.Drawing.Size(460, 587);
            this.ultraTabControl1.TabIndex = 20;
            ultraTab3.TabPage = this.ultraTabPageControl1;
            ultraTab3.Text = "Order";
            ultraTab4.TabPage = this.ultraTabPageControl2;
            ultraTab4.Text = "Processes";
            ultraTab1.Key = "SerialNumbers";
            ultraTab1.TabPage = this.ultraTabPageControl3;
            ultraTab1.Text = "Serial Numbers";
            ultraTab1.Visible = false;
            ultraTab2.Key = "Notes";
            ultraTab2.TabPage = this.ultraTabPageControl4;
            ultraTab2.Text = "Notes";
            this.ultraTabControl1.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab3,
            ultraTab4,
            ultraTab1,
            ultraTab2});
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(456, 561);
            // 
            // taOrderFees
            // 
            this.taOrderFees.ClearBeforeFill = false;
            // 
            // taOrderProcesses
            // 
            this.taOrderProcesses.ClearBeforeFill = false;
            // 
            // taOrder_Media
            // 
            this.taOrder_Media.ClearBeforeFill = false;
            // 
            // taOrderFeeType
            // 
            this.taOrderFeeType.ClearBeforeFill = false;
            // 
            // taOrderDocumentLink
            // 
            this.taOrderDocumentLink.ClearBeforeFill = true;
            // 
            // taCustomerAddress
            // 
            this.taCustomerAddress.ClearBeforeFill = false;
            // 
            // taOrderSerialNumber
            // 
            this.taOrderSerialNumber.ClearBeforeFill = true;
            // 
            // taProcessingLine
            // 
            this.taProcessingLine.ClearBeforeFill = true;
            // 
            // taManager
            // 
            this.taManager.BackupDataSetBeforeUpdate = false;
            this.taManager.BatchCOCNotificationTableAdapter = null;
            this.taManager.BatchCOCOrderTableAdapter = null;
            this.taManager.BatchCOCTableAdapter = null;
            this.taManager.BatchOperatorTableAdapter = null;
            this.taManager.BatchOperatorTimeTableAdapter = null;
            this.taManager.BatchOrderTableAdapter = null;
            this.taManager.BatchProcess_OrderProcessTableAdapter = null;
            this.taManager.BatchProcessesOperatorTableAdapter = null;
            this.taManager.BatchProcessesTableAdapter = null;
            this.taManager.BatchTableAdapter = null;
            this.taManager.BillOfLadingDocumentLinkTableAdapter = null;
            this.taManager.BillOfLadingMediaTableAdapter = null;
            this.taManager.BillOfLadingOrderTableAdapter = null;
            this.taManager.BillOfLadingTableAdapter = null;
            this.taManager.BulkCOCOrderTableAdapter = null;
            this.taManager.BulkCOCTableAdapter = null;
            this.taManager.COCTableAdapter = null;
            this.taManager.CustomerAddressTableAdapter = this.taCustomerAddress;
            this.taManager.CustomerCommunicationTableAdapter = null;
            this.taManager.CustomerDefaultPriceTableAdapter = null;
            this.taManager.CustomerFeeTableAdapter = null;
            this.taManager.CustomerPricePointDetailTableAdapter = null;
            this.taManager.CustomerPricePointTableAdapter = null;
            this.taManager.d_FeeTypeTableAdapter = null;
            this.taManager.d_HoldLocationTableAdapter = null;
            this.taManager.d_HoldReasonTableAdapter = null;
            this.taManager.d_ReworkReasonTableAdapter = null;
            this.taManager.InternalReworkTableAdapter = null;
            this.taManager.LaborTimeTableAdapter = null;
            this.taManager.ListsTableAdapter = null;
            this.taManager.ListValuesTableAdapter = null;
            this.taManager.MediaTableAdapter = this.taMedia;
            this.taManager.Order_DocumentLinkTableAdapter = this.taOrderDocumentLink;
            this.taManager.Order_MediaTableAdapter = this.taOrder_Media;
            this.taManager.OrderApprovalTableAdapter = null;
            this.taManager.OrderApprovalTermTableAdapter = null;
            this.taManager.OrderChangeTableAdapter = null;
            this.taManager.OrderContainerItemTableAdapter = null;
            this.taManager.OrderContainersTableAdapter = null;
            this.taManager.OrderCustomFieldsTableAdapter = null;
            this.taManager.OrderFeesTableAdapter = this.taOrderFees;
            this.taManager.OrderFeeTypeTableAdapter = this.taOrderFeeType;
            this.taManager.OrderHoldNotificationTableAdapter = null;
            this.taManager.OrderHoldTableAdapter = null;
            this.taManager.OrderNoteTableAdapter = this.taOrderNote;
            this.taManager.OrderOperatorTableAdapter = null;
            this.taManager.OrderOperatorTimeTableAdapter = null;
            this.taManager.OrderPartMarkTableAdapter = null;
            this.taManager.OrderProcessAnswerTableAdapter = null;
            this.taManager.OrderProcessesOperatorTableAdapter = null;
            this.taManager.OrderProcessesTableAdapter = this.taOrderProcesses;
            this.taManager.OrderProductClassTableAdapter = null;
            this.taManager.OrderReviewTableAdapter = null;
            this.taManager.OrderSerialNumberTableAdapter = this.taOrderSerialNumber;
            this.taManager.OrderShipmentTableAdapter = null;
            this.taManager.OrderTableAdapter = this.taOrder;
            this.taManager.OrderTemplateTableAdapter = null;
            this.taManager.OrderWorkDescriptionTableAdapter = null;
            this.taManager.PartInspectionAnswerTableAdapter = null;
            this.taManager.PartInspectionTableAdapter = null;
            this.taManager.PartInspectionTypeTableAdapter = null;
            this.taManager.PartSummaryTableAdapter = this.taPartSummary;
            this.taManager.PricePointDetailTableAdapter = null;
            this.taManager.PricePointTableAdapter = null;
            this.taManager.PriceUnitTableAdapter = this.taPriceUnit;
            this.taManager.ProcessingLineTableAdapter = this.taProcessingLine;
            this.taManager.ProductClassTableAdapter = null;
            this.taManager.QuoteTableAdapter = null;
            this.taManager.SalesOrder_DocumentLinkTableAdapter = null;
            this.taManager.SalesOrder_MediaTableAdapter = null;
            this.taManager.SalesOrderTableAdapter = null;
            this.taManager.ShipmentPackageTableAdapter = null;
            this.taManager.ShipmentPackageTypeTableAdapter = null;
            this.taManager.UpdateOrder = DWOS.Data.Datasets.OrdersDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            this.taManager.WorkDescriptionTableAdapter = null;
            // 
            // taOrderNote
            // 
            this.taOrderNote.ClearBeforeFill = true;
            // 
            // taWorkDescription
            // 
            this.taWorkDescription.ClearBeforeFill = true;
            // 
            // taOrderWorkDescription
            // 
            this.taOrderWorkDescription.ClearBeforeFill = true;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // persistWindowState
            // 
            this.persistWindowState.FileNamePrefix = null;
            this.persistWindowState.ParentForm = this;
            this.persistWindowState.Splitter = null;
            // 
            // QuickViewOrder
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 640);
            this.Controls.Add(this.QuickViewOrder_Fill_Panel);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QuickViewOrder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Order Summary";
            this.Load += new System.EventHandler(this.QuickViewPart_Load);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.ultraTabPageControl2.ResumeLayout(false);
            this.ultraTabPageControl3.ResumeLayout(false);
            this.ultraTabPageControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dsOrders)).EndInit();
            this.QuickViewOrder_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl1)).EndInit();
            this.ultraTabControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnOK;
		private DWOS.UI.Sales.OrderInformation pnlOrderInfo;
		private DWOS.Data.Datasets.OrdersDataSet dsOrders;
		private DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter taOrder;
		private DWOS.Data.Datasets.OrdersDataSetTableAdapters.PartSummaryTableAdapter taPartSummary;
		private DWOS.Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter taMedia;
		private DWOS.Data.Datasets.OrdersDataSetTableAdapters.CustomerSummaryTableAdapter taCustomerSummary;
		private DWOS.Data.Datasets.OrdersDataSetTableAdapters.d_PriorityTableAdapter taPriority;
		private DWOS.Data.Datasets.OrdersDataSetTableAdapters.d_OrderStatusTableAdapter taOrderStatus;
		private DWOS.Data.Datasets.OrdersDataSetTableAdapters.UserSummaryTableAdapter taUserSummary;
		private DWOS.Data.Datasets.OrdersDataSetTableAdapters.PriceUnitTableAdapter taPriceUnit;
		private DWOS.Data.Datasets.OrdersDataSetTableAdapters.CustomerShippingSummaryTableAdapter taCustomerShippingSummary;
		private Infragistics.Win.Misc.UltraButton btnPrint;
		private Sales.OrderProcessingInfo pnlOrderProcessingInfo;
		private System.Windows.Forms.Panel QuickViewOrder_Fill_Panel;
		private Data.Datasets.OrdersDataSetTableAdapters.OrderFeesTableAdapter taOrderFees;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl ultraTabControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Data.Datasets.OrdersDataSetTableAdapters.OrderProcessesTableAdapter taOrderProcesses;
        private Data.Datasets.OrdersDataSetTableAdapters.Order_MediaTableAdapter taOrder_Media;
        private Data.Datasets.OrdersDataSetTableAdapters.OrderFeeTypeTableAdapter taOrderFeeType;
        private Data.Datasets.OrdersDataSetTableAdapters.Order_DocumentLinkTableAdapter taOrderDocumentLink;
        private Data.Datasets.OrdersDataSetTableAdapters.CustomerAddressTableAdapter taCustomerAddress;
        private Data.Datasets.OrdersDataSetTableAdapters.OrderSerialNumberTableAdapter taOrderSerialNumber;
        private Data.Datasets.OrdersDataSetTableAdapters.ProcessingLineTableAdapter taProcessingLine;
        private Data.Datasets.OrdersDataSetTableAdapters.TableAdapterManager taManager;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl3;
        private Sales.Order.SerialNumberQuickViewPanel pnlSerialNumbers;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl4;
        private Sales.Order.NotesQuickViewPanel notesQuickViewPanel;
        private Data.Datasets.OrdersDataSetTableAdapters.OrderNoteTableAdapter taOrderNote;
        private Data.Datasets.OrdersDataSetTableAdapters.WorkDescriptionTableAdapter taWorkDescription;
        private Data.Datasets.OrdersDataSetTableAdapters.OrderWorkDescriptionTableAdapter taOrderWorkDescription;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private Utilities.PersistWindowState persistWindowState;
    }
}