namespace DWOS.UI.Sales
{
	partial class BatchOrderProcessing
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
            DisposeForm();

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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BatchOrderProcessing));
            Infragistics.Win.UltraWinToolbars.PopupControlContainerTool popupControlContainerTool1 = new Infragistics.Win.UltraWinToolbars.PopupControlContainerTool("Settings");
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab1 = new Infragistics.Win.UltraWinToolbars.RibbonTab("ribbon1");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup1 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup1");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddBatch");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddOrder");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Complete");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup2 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("Reports");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("BatchTraveler");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup3 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup2");
            Infragistics.Win.UltraWinToolbars.ControlContainerTool controlContainerTool2 = new Infragistics.Win.UltraWinToolbars.ControlContainerTool("OrderCount");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddOrder");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Print");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Complete");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddBatch");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ControlContainerTool controlContainerTool1 = new Infragistics.Win.UltraWinToolbars.ControlContainerTool("OrderCount");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("BatchTraveler");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupControlContainerTool popupControlContainerTool2 = new Infragistics.Win.UltraWinToolbars.PopupControlContainerTool("Settings");
            Infragistics.UltraGauge.Resources.SegmentedDigitalGauge segmentedDigitalGauge1 = new Infragistics.UltraGauge.Resources.SegmentedDigitalGauge();
            Infragistics.UltraGauge.Resources.SolidFillBrushElement solidFillBrushElement1 = new Infragistics.UltraGauge.Resources.SolidFillBrushElement();
            Infragistics.UltraGauge.Resources.SolidFillBrushElement solidFillBrushElement2 = new Infragistics.UltraGauge.Resources.SolidFillBrushElement();
            this.dsOrderProcessing = new DWOS.Data.Datasets.OrderProcessingDataSet();
            this.taOrderProcesses = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter();
            this.taOrderBatch = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderBatchTableAdapter();
            this.taProcess = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter();
            this.taOrderBatchItem = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderBatchItemTableAdapter();
            this.pnlBatchOrderInfo = new DWOS.UI.Sales.BatchOrderPanels.BatchOrderInfo();
            this.taOrder = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter();
            this.taManager = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.TableAdapterManager();
            this.taOrderProcessAnswer = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessAnswerTableAdapter();
            this.taProcessSteps = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessStepsTableAdapter();
            this.taProcessQuestion = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessQuestionTableAdapter();
            this.guagePartCount = new Infragistics.Win.UltraWinGauge.UltraGauge();
            this.batchSettings1 = new DWOS.UI.Sales.BatchOrderPanels.BatchSettings();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrderProcessing)).BeginInit();
            this.SuspendLayout();
            // 
            // toolbarManager
            // 
            this.toolbarManager.MenuSettings.ForceSerialization = true;
            appearance1.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance1.ImageBackground")));
            appearance1.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.toolbarManager.Ribbon.ApplicationMenu2010.ContentArea.Settings.Appearance = appearance1;
            this.toolbarManager.Ribbon.ApplicationMenu2010.NavigationMenu.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupControlContainerTool1});
            this.toolbarManager.Ribbon.FileMenuStyle = Infragistics.Win.UltraWinToolbars.FileMenuStyle.ApplicationMenu2010;
            ribbonTab1.Caption = "Home";
            ribbonGroup1.Caption = "Order Batch";
            buttonTool10.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool1.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool3.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool9.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool10,
            buttonTool1,
            buttonTool3,
            buttonTool9});
            ribbonGroup2.Caption = "Reports";
            buttonTool2.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool2});
            ribbonGroup3.Caption = "Batch Orders";
            controlContainerTool2.ControlName = "guagePartCount";
            controlContainerTool2.InstanceProps.IsFirstInGroup = true;
            controlContainerTool2.InstanceProps.Width = 146;
            ribbonGroup3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            controlContainerTool2});
            ribbonTab1.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup1,
            ribbonGroup2,
            ribbonGroup3});
            this.toolbarManager.Ribbon.NonInheritedRibbonTabs.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonTab[] {
            ribbonTab1});
            this.toolbarManager.Ribbon.QuickAccessToolbar.Settings.ShowToolTips = Infragistics.Win.DefaultableBoolean.True;
            this.toolbarManager.Ribbon.Visible = true;
            this.toolbarManager.ToolbarSettings.ForceSerialization = true;
            appearance2.Image = global::DWOS.UI.Properties.Resources.Add_32;
            buttonTool6.SharedPropsInternal.AppearancesLarge.Appearance = appearance2;
            buttonTool6.SharedPropsInternal.Caption = "Add Order";
            buttonTool6.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool6.SharedPropsInternal.Enabled = false;
            buttonTool6.SharedPropsInternal.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            buttonTool6.SharedPropsInternal.ToolTipText = "Add an order to a selected load.";
            buttonTool6.SharedPropsInternal.ToolTipTitle = "Add Order";
            appearance3.Image = global::DWOS.UI.Properties.Resources.Delete_32;
            buttonTool7.SharedPropsInternal.AppearancesLarge.Appearance = appearance3;
            buttonTool7.SharedPropsInternal.Caption = "Cancel";
            buttonTool7.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool7.SharedPropsInternal.Enabled = false;
            buttonTool7.SharedPropsInternal.ToolTipText = "Cancel the selected load.";
            buttonTool8.SharedPropsInternal.Caption = "Shipping Report";
            buttonTool8.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool8.SharedPropsInternal.Enabled = false;
            appearance4.Image = global::DWOS.UI.Properties.Resources.Process_32;
            buttonTool5.SharedPropsInternal.AppearancesLarge.Appearance = appearance4;
            buttonTool5.SharedPropsInternal.Caption = "Batch Processing";
            buttonTool5.SharedPropsInternal.Enabled = false;
            buttonTool5.SharedPropsInternal.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            buttonTool5.SharedPropsInternal.ToolTipText = "Process the selected load.";
            appearance5.Image = global::DWOS.UI.Properties.Resources.Add_32;
            buttonTool4.SharedPropsInternal.AppearancesLarge.Appearance = appearance5;
            buttonTool4.SharedPropsInternal.Caption = "Create Load";
            buttonTool4.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool4.SharedPropsInternal.Enabled = false;
            buttonTool4.SharedPropsInternal.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
            buttonTool4.SharedPropsInternal.ToolTipText = "Create a new empty load.";
            buttonTool4.SharedPropsInternal.ToolTipTitle = "Create Load";
            controlContainerTool1.ControlName = "guagePartCount";
            controlContainerTool1.SharedPropsInternal.ToolTipText = "The number of batched orders closed today in this department.";
            controlContainerTool1.SharedPropsInternal.ToolTipTitle = "Batch Orders Count";
            controlContainerTool1.SharedPropsInternal.Width = 146;
            appearance6.Image = global::DWOS.UI.Properties.Resources.Print_32;
            buttonTool11.SharedPropsInternal.AppearancesLarge.Appearance = appearance6;
            buttonTool11.SharedPropsInternal.Caption = "Batch Traveler";
            buttonTool11.SharedPropsInternal.Enabled = false;
            popupControlContainerTool2.ControlName = "batchSettings1";
            popupControlContainerTool2.DropDownArrowStyle = Infragistics.Win.UltraWinToolbars.DropDownArrowStyle.Standard;
            popupControlContainerTool2.SharedPropsInternal.Caption = "Settings";
            this.toolbarManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool6,
            buttonTool7,
            buttonTool8,
            buttonTool5,
            buttonTool4,
            controlContainerTool1,
            buttonTool11,
            popupControlContainerTool2});
            // 
            // tvwTOC
            // 
            this.tvwTOC.Size = new System.Drawing.Size(240, 835);
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.guagePartCount);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.batchSettings1);
            this.splitContainer1.Panel2.Controls.Add(this.pnlBatchOrderInfo);
            this.splitContainer1.Size = new System.Drawing.Size(914, 835);
            this.splitContainer1.SplitterDistance = 240;
            // 
            // helpLink1
            // 
            this.helpLink1.HelpPage = "batch_order_processing_dialog.htm";
            // 
            // dsOrderProcessing
            // 
            this.dsOrderProcessing.DataSetName = "OrderProcessingDataSet";
            this.dsOrderProcessing.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taOrderProcesses
            // 
            this.taOrderProcesses.ClearBeforeFill = false;
            // 
            // taOrderBatch
            // 
            this.taOrderBatch.ClearBeforeFill = true;
            // 
            // taProcess
            // 
            this.taProcess.ClearBeforeFill = true;
            // 
            // taOrderBatchItem
            // 
            this.taOrderBatchItem.ClearBeforeFill = true;
            // 
            // pnlBatchOrderInfo
            // 
            this.pnlBatchOrderInfo.Editable = true;
            this.pnlBatchOrderInfo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlBatchOrderInfo.IsActivePanel = false;
            this.pnlBatchOrderInfo.Location = new System.Drawing.Point(35, 6);
            this.pnlBatchOrderInfo.Name = "pnlBatchOrderInfo";
            this.pnlBatchOrderInfo.Padding = new System.Windows.Forms.Padding(5);
            this.pnlBatchOrderInfo.Size = new System.Drawing.Size(366, 553);
            this.pnlBatchOrderInfo.TabIndex = 0;
            // 
            // taOrder
            // 
            this.taOrder.ClearBeforeFill = false;
            // 
            // taManager
            // 
            this.taManager.BackupDataSetBeforeUpdate = false;
            this.taManager.OrderBatchItemTableAdapter = this.taOrderBatchItem;
            this.taManager.OrderBatchTableAdapter = this.taOrderBatch;
            this.taManager.OrderPartMarkTableAdapter = null;
            this.taManager.OrderProcessAnswerTableAdapter = null;
            this.taManager.OrderProcessesTableAdapter = null;
            this.taManager.UpdateOrder = DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            // 
            // taOrderProcessAnswer
            // 
            this.taOrderProcessAnswer.ClearBeforeFill = false;
            // 
            // taProcessSteps
            // 
            this.taProcessSteps.ClearBeforeFill = false;
            // 
            // taProcessQuestion
            // 
            this.taProcessQuestion.ClearBeforeFill = false;
            // 
            // guagePartCount
            // 
            this.guagePartCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.guagePartCount.BackColor = System.Drawing.Color.Transparent;
            solidFillBrushElement1.Color = System.Drawing.Color.Black;
            segmentedDigitalGauge1.BrushElements.Add(solidFillBrushElement1);
            segmentedDigitalGauge1.Digits = 6;
            segmentedDigitalGauge1.DigitSpacing = 3D;
            solidFillBrushElement2.Color = System.Drawing.Color.Red;
            segmentedDigitalGauge1.FontBrushElements.Add(solidFillBrushElement2);
            segmentedDigitalGauge1.Mode = Infragistics.UltraGauge.Resources.SegmentMode.FourteenSegment;
            segmentedDigitalGauge1.Text = "999999";
            this.guagePartCount.Gauges.Add(segmentedDigitalGauge1);
            this.guagePartCount.Location = new System.Drawing.Point(157, 855);
            this.guagePartCount.Name = "guagePartCount";
            this.guagePartCount.Size = new System.Drawing.Size(146, 73);
            this.guagePartCount.TabIndex = 6;
            this.guagePartCount.Visible = false;
            // 
            // batchSettings1
            // 
            this.batchSettings1.BackColor = System.Drawing.Color.Transparent;
            this.batchSettings1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.batchSettings1.Location = new System.Drawing.Point(489, 184);
            this.batchSettings1.Name = "batchSettings1";
            this.batchSettings1.SettingTool = null;
            this.batchSettings1.Size = new System.Drawing.Size(367, 145);
            this.batchSettings1.TabIndex = 1;
            // 
            // BatchOrderProcessing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(930, 1059);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BatchOrderProcessing";
            this.Text = "Batch Order Processing";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BatchOrderProcessingManager_FormClosing);
            this.Load += new System.EventHandler(this.BatchOrderProcessingManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dsOrderProcessing)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private DWOS.Data.Datasets.OrderProcessingDataSet dsOrderProcessing;
		private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter taOrderProcesses;
		private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderBatchTableAdapter taOrderBatch;
		private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter taProcess;
		private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderBatchItemTableAdapter taOrderBatchItem;
		private BatchOrderPanels.BatchOrderInfo pnlBatchOrderInfo;
		private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter taOrder;
		private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.TableAdapterManager taManager;
		private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessAnswerTableAdapter taOrderProcessAnswer;
		private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessStepsTableAdapter taProcessSteps;
		private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessQuestionTableAdapter taProcessQuestion;
		private Infragistics.Win.UltraWinGauge.UltraGauge guagePartCount;
        private BatchOrderPanels.BatchSettings batchSettings1;

	}
}
