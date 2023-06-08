namespace DWOS.UI.Admin
{
	partial class ProcessingSchedule
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
            Infragistics.Win.UltraWinSchedule.DateInterval dateInterval2 = new Infragistics.Win.UltraWinSchedule.DateInterval();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinSchedule.DateInterval dateInterval3 = new Infragistics.Win.UltraWinSchedule.DateInterval();
            Infragistics.Win.UltraWinSchedule.DayOfWeek dayOfWeek1 = new Infragistics.Win.UltraWinSchedule.DayOfWeek(System.DayOfWeek.Monday);
            Infragistics.Win.UltraWinSchedule.DayOfWeek dayOfWeek2 = new Infragistics.Win.UltraWinSchedule.DayOfWeek(System.DayOfWeek.Tuesday);
            Infragistics.Win.UltraWinSchedule.DayOfWeek dayOfWeek3 = new Infragistics.Win.UltraWinSchedule.DayOfWeek(System.DayOfWeek.Wednesday);
            Infragistics.Win.UltraWinSchedule.DayOfWeek dayOfWeek4 = new Infragistics.Win.UltraWinSchedule.DayOfWeek(System.DayOfWeek.Thursday);
            Infragistics.Win.UltraWinSchedule.DayOfWeek dayOfWeek5 = new Infragistics.Win.UltraWinSchedule.DayOfWeek(System.DayOfWeek.Friday);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("OrderSchedule", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PurchaseOrder");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartQuantity", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Priority");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CurrentLocation");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WorkStatus");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Material");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RequiredDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SchedulePriority");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartName");
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EstShipDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("InitialEstShipDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TotalSurfaceArea");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderProcesses_OrderSchedule");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("NextProcessName", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Score", 1);
            Infragistics.Win.UltraWinGrid.SummarySettings summarySettings1 = new Infragistics.Win.UltraWinGrid.SummarySettings("", Infragistics.Win.UltraWinGrid.SummaryType.Sum, null, "TotalSurfaceArea", 14, true, "OrderSchedule", 0, Infragistics.Win.UltraWinGrid.SummaryPosition.UseSummaryPositionColumn, null, -1, false);
            Infragistics.Win.UltraWinGrid.SummarySettings summarySettings2 = new Infragistics.Win.UltraWinGrid.SummarySettings("", Infragistics.Win.UltraWinGrid.SummaryType.Sum, null, "PartQuantity", 2, true, "OrderSchedule", 0, Infragistics.Win.UltraWinGrid.SummaryPosition.UseSummaryPositionColumn, null, -1, false);
            Infragistics.Win.UltraWinGrid.SummarySettings summarySettings3 = new Infragistics.Win.UltraWinGrid.SummarySettings("", Infragistics.Win.UltraWinGrid.SummaryType.Count, null, "OrderID", 0, true, "OrderSchedule", 0, Infragistics.Win.UltraWinGrid.SummaryPosition.UseSummaryPositionColumn, null, -1, false);
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("OrderProcesses_OrderSchedule", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderProcessesID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StepOrder");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Department");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StartDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EndDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EstEndDate");
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinSchedule.DateInterval dateInterval1 = new Infragistics.Win.UltraWinSchedule.DateInterval();
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab1 = new Infragistics.Win.UltraWinToolbars.RibbonTab("ribbon1");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup1 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup2");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveSchedule");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("OrderPriority");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Find");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Reload");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Differences");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup2 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup4");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CreateReport");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Infragistics.Win.UltraWinToolbars.ButtonTool("PrintSchedule");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup3 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup3");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Settings");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveSettings");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup4 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup1");
            Infragistics.Win.UltraWinToolbars.ControlContainerTool controlContainerTool2 = new Infragistics.Win.UltraWinToolbars.ControlContainerTool("ControlContainerTool1");
            Infragistics.Win.UltraWinToolbars.ControlContainerTool controlContainerTool3 = new Infragistics.Win.UltraWinToolbars.ControlContainerTool("AvgLate");
            Infragistics.Win.UltraWinToolbars.ControlContainerTool controlContainerTool4 = new Infragistics.Win.UltraWinToolbars.ControlContainerTool("PercentLate");
            Infragistics.Win.UltraWinToolbars.ControlContainerTool controlContainerTool1 = new Infragistics.Win.UltraWinToolbars.ControlContainerTool("ControlContainerTool1");
            Infragistics.Win.UltraWinToolbars.ControlContainerTool controlContainerTool5 = new Infragistics.Win.UltraWinToolbars.ControlContainerTool("AvgLate");
            Infragistics.Win.UltraWinToolbars.ControlContainerTool controlContainerTool6 = new Infragistics.Win.UltraWinToolbars.ControlContainerTool("PercentLate");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Settings");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CreateReport");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("PrintSchedule");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Find");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ButtonTool1");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveSettings");
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Reload");
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveSchedule");
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("OrderPriority");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Differences");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessingSchedule));
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane1 = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.DockedBottom, new System.Guid("f3cda131-70f4-464f-90f1-a4b3bb16ebc6"));
            Infragistics.Win.UltraWinDock.DockableControlPane dockableControlPane1 = new Infragistics.Win.UltraWinDock.DockableControlPane(new System.Guid("8165810f-2285-4beb-9976-6de071fb4780"), new System.Guid("00000000-0000-0000-0000-000000000000"), -1, new System.Guid("f3cda131-70f4-464f-90f1-a4b3bb16ebc6"), -1);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.notificationPanel = new DWOS.UI.Utilities.NotificationPanel();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gvSchedule = new Infragistics.Win.UltraWinGanttView.UltraGanttView();
            this.calInfo = new Infragistics.Win.UltraWinSchedule.UltraCalendarInfo(this.components);
            this.calLook = new Infragistics.Win.UltraWinSchedule.UltraCalendarLook(this.components);
            this.grdOrders = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsSchedule = new DWOS.Data.Datasets.ScheduleDataset();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.timeline = new Infragistics.Win.UltraWinSchedule.UltraTimelineView();
            this.ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            this.guagePercentLate = new DWOS.UI.Utilities.DigitalGuage();
            this.guageAvgDaysLate = new DWOS.UI.Utilities.DigitalGuage();
            this.guageTotalOrders = new DWOS.UI.Utilities.DigitalGuage();
            this._ProcessingSchedule_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.toolbarManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._ProcessingSchedule_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ProcessingSchedule_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ProcessingSchedule_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this._ProcessingScheduleUnpinnedTabAreaLeft = new Infragistics.Win.UltraWinDock.UnpinnedTabArea();
            this.ultraDockManager1 = new Infragistics.Win.UltraWinDock.UltraDockManager(this.components);
            this._ProcessingScheduleUnpinnedTabAreaRight = new Infragistics.Win.UltraWinDock.UnpinnedTabArea();
            this._ProcessingScheduleUnpinnedTabAreaTop = new Infragistics.Win.UltraWinDock.UnpinnedTabArea();
            this._ProcessingScheduleUnpinnedTabAreaBottom = new Infragistics.Win.UltraWinDock.UnpinnedTabArea();
            this._ProcessingScheduleAutoHideControl = new Infragistics.Win.UltraWinDock.AutoHideControl();
            this.dockableWindow1 = new Infragistics.Win.UltraWinDock.DockableWindow();
            this.windowDockingArea1 = new Infragistics.Win.UltraWinDock.WindowDockingArea();
            this.ultraTabControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvSchedule)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdOrders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsSchedule)).BeginInit();
            this.ultraTabPageControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeline)).BeginInit();
            this.ultraPanel1.ClientArea.SuspendLayout();
            this.ultraPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDockManager1)).BeginInit();
            this._ProcessingScheduleAutoHideControl.SuspendLayout();
            this.dockableWindow1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl1)).BeginInit();
            this.ultraTabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notificationPanel
            // 
            this.notificationPanel.Location = new System.Drawing.Point(0, 18);
            this.notificationPanel.Name = "notificationPanel";
            this.notificationPanel.NotificationPane = null;
            this.notificationPanel.Size = new System.Drawing.Size(1244, 188);
            this.notificationPanel.TabIndex = 19;
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.splitContainer1);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(1232, 585);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gvSchedule);
            this.inboxControlStyler1.SetStyleSettings(this.splitContainer1.Panel1, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.grdOrders);
            this.splitContainer1.Size = new System.Drawing.Size(1232, 585);
            this.splitContainer1.SplitterDistance = 436;
            this.inboxControlStyler1.SetStyleSettings(this.splitContainer1, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.splitContainer1.TabIndex = 2;
            // 
            // gvSchedule
            // 
            this.gvSchedule.CalendarInfo = this.calInfo;
            this.gvSchedule.CalendarLook = this.calLook;
            this.gvSchedule.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("Constraint").Visible = Infragistics.Win.DefaultableBoolean.False;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("Constraint").VisiblePosition = 6;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("ConstraintDateTime").VisiblePosition = 7;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("Dependencies").Visible = Infragistics.Win.DefaultableBoolean.False;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("Dependencies").VisiblePosition = 4;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("Deadline").VisiblePosition = 8;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("Duration").Format = "dd";
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("Duration").VisiblePosition = 1;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("Duration").Width = 75;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("EndDateTime").VisiblePosition = 3;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("EndDateTime").Width = 100;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("Milestone").VisiblePosition = 9;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("Name").ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("Name").VisiblePosition = 0;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("Notes").VisiblePosition = 10;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("PercentComplete").VisiblePosition = 11;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("Resources").Visible = Infragistics.Win.DefaultableBoolean.False;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("Resources").VisiblePosition = 5;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("StartDateTime").VisiblePosition = 2;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("StartDateTime").Width = 100;
            this.gvSchedule.GridSettings.ColumnSettings.GetValue("RowNumber").VisiblePosition = 12;
            this.gvSchedule.Location = new System.Drawing.Point(0, 0);
            this.gvSchedule.Name = "gvSchedule";
            this.gvSchedule.Size = new System.Drawing.Size(1232, 436);
            this.gvSchedule.TabIndex = 0;
            dateInterval2.HeaderTextFormatStyle = Infragistics.Win.UltraWinSchedule.TimelineViewHeaderTextFormatStyle.RangeStart;
            dateInterval2.IntervalUnits = Infragistics.Win.UltraWinSchedule.DateIntervalUnits.Weeks;
            this.gvSchedule.TimelineSettings.AdditionalIntervals.Add(dateInterval2);
            this.gvSchedule.TimelineSettings.AllowedDragActions = Infragistics.Win.UltraWinSchedule.BarDragActions.None;
            this.gvSchedule.TimelineSettings.AutoAddAdditionalInterval = false;
            appearance17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.gvSchedule.TimelineSettings.NonWorkingHourAppearance = appearance17;
            dateInterval3.HeaderTextFormat = "ddd";
            this.gvSchedule.TimelineSettings.PrimaryInterval = dateInterval3;
            this.gvSchedule.VerticalSplitterMinimumResizeWidth = 10;
            this.gvSchedule.ActiveTaskChanged += new Infragistics.Win.UltraWinGanttView.ActiveTaskChangedHandler(this.gvSchedule_ActiveTaskChanged);
            // 
            // calInfo
            // 
            this.calInfo.DataBindingsForAppointments.BindingContextControl = this;
            this.calInfo.DataBindingsForOwners.BindingContextControl = this;
            dayOfWeek1.WorkDayEndTime = new System.DateTime(2001, 12, 31, 23, 0, 0, 0);
            dayOfWeek1.WorkDayStartTime = new System.DateTime(2001, 12, 31, 5, 0, 0, 0);
            dayOfWeek2.WorkDayEndTime = new System.DateTime(2001, 12, 31, 23, 0, 0, 0);
            dayOfWeek2.WorkDayStartTime = new System.DateTime(2001, 12, 31, 5, 0, 0, 0);
            dayOfWeek3.WorkDayEndTime = new System.DateTime(2001, 12, 31, 23, 0, 0, 0);
            dayOfWeek3.WorkDayStartTime = new System.DateTime(2001, 12, 31, 5, 0, 0, 0);
            dayOfWeek4.WorkDayEndTime = new System.DateTime(2001, 12, 31, 23, 0, 0, 0);
            dayOfWeek4.WorkDayStartTime = new System.DateTime(2001, 12, 31, 5, 0, 0, 0);
            dayOfWeek5.WorkDayEndTime = new System.DateTime(2001, 12, 31, 23, 0, 0, 0);
            dayOfWeek5.WorkDayStartTime = new System.DateTime(2001, 12, 31, 5, 0, 0, 0);
            this.calInfo.DaysOfWeek.Add(dayOfWeek1);
            this.calInfo.DaysOfWeek.Add(dayOfWeek2);
            this.calInfo.DaysOfWeek.Add(dayOfWeek3);
            this.calInfo.DaysOfWeek.Add(dayOfWeek4);
            this.calInfo.DaysOfWeek.Add(dayOfWeek5);
            this.calInfo.TaskDurationWorkingTimePerDay = System.TimeSpan.Parse("00:01:00");
            // 
            // calLook
            // 
            appearance2.BackColor = System.Drawing.Color.PaleVioletRed;
            this.calLook.HolidayAppearance = appearance2;
            this.calLook.ViewStyle = Infragistics.Win.UltraWinSchedule.ViewStyle.Standard;
            // 
            // grdOrders
            // 
            this.grdOrders.DataMember = "OrderSchedule";
            this.grdOrders.DataSource = this.dsSchedule;
            appearance18.BackColor = System.Drawing.SystemColors.Window;
            appearance18.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdOrders.DisplayLayout.Appearance = appearance18;
            this.grdOrders.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            ultraGridColumn1.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn1.Header.Caption = "Work Order";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 129;
            ultraGridColumn2.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn2.Header.Caption = "PO";
            ultraGridColumn2.Header.VisiblePosition = 2;
            ultraGridColumn2.Hidden = true;
            ultraGridColumn2.Width = 119;
            ultraGridColumn3.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn3.Header.Caption = "Quantity";
            ultraGridColumn3.Header.VisiblePosition = 8;
            ultraGridColumn3.Width = 147;
            ultraGridColumn4.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Width = 129;
            ultraGridColumn5.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn5.Hidden = true;
            ultraGridColumn5.Width = 151;
            ultraGridColumn6.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn6.Hidden = true;
            ultraGridColumn6.Width = 152;
            ultraGridColumn7.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn7.Header.Caption = "Customer";
            ultraGridColumn7.Header.VisiblePosition = 1;
            ultraGridColumn7.Width = 129;
            ultraGridColumn8.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn8.Header.VisiblePosition = 6;
            ultraGridColumn8.Hidden = true;
            ultraGridColumn8.Width = 103;
            ultraGridColumn9.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn9.Header.Caption = "Required Date";
            ultraGridColumn9.Header.VisiblePosition = 10;
            ultraGridColumn9.Width = 229;
            ultraGridColumn10.Header.VisiblePosition = 11;
            ultraGridColumn10.Hidden = true;
            ultraGridColumn10.Width = 67;
            ultraGridColumn11.Header.VisiblePosition = 15;
            ultraGridColumn11.Hidden = true;
            ultraGridColumn11.Width = 130;
            appearance19.FontData.UnderlineAsString = "True";
            appearance19.ForeColor = System.Drawing.Color.Blue;
            ultraGridColumn12.CellAppearance = appearance19;
            ultraGridColumn12.Header.Caption = "Part";
            ultraGridColumn12.Header.VisiblePosition = 7;
            ultraGridColumn12.Width = 129;
            ultraGridColumn13.Header.Caption = "Est Ship";
            ultraGridColumn13.Header.VisiblePosition = 13;
            ultraGridColumn13.Hidden = true;
            ultraGridColumn13.Width = 103;
            ultraGridColumn14.Header.VisiblePosition = 14;
            ultraGridColumn14.Hidden = true;
            ultraGridColumn14.Width = 97;
            ultraGridColumn15.Header.Caption = "Total SA";
            ultraGridColumn15.Header.VisiblePosition = 9;
            ultraGridColumn15.Width = 181;
            ultraGridColumn16.Header.VisiblePosition = 17;
            ultraGridColumn17.Header.VisiblePosition = 12;
            ultraGridColumn17.Hidden = true;
            ultraGridColumn17.Width = 95;
            ultraGridColumn27.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            ultraGridColumn27.DataType = typeof(int);
            ultraGridColumn27.Header.VisiblePosition = 16;
            ultraGridColumn27.Width = 129;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn27});
            ultraGridBand1.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridBand1.Summaries.AddRange(new Infragistics.Win.UltraWinGrid.SummarySettings[] {
            summarySettings1,
            summarySettings2,
            summarySettings3});
            ultraGridBand1.SummaryFooterCaption = "Totals:";
            ultraGridColumn19.Header.VisiblePosition = 0;
            ultraGridColumn19.Width = 200;
            ultraGridColumn20.Header.VisiblePosition = 1;
            ultraGridColumn20.Width = 107;
            ultraGridColumn21.Header.VisiblePosition = 2;
            ultraGridColumn21.Width = 124;
            ultraGridColumn22.Header.VisiblePosition = 3;
            ultraGridColumn22.Width = 127;
            ultraGridColumn23.Header.VisiblePosition = 4;
            ultraGridColumn23.Width = 171;
            ultraGridColumn24.Header.VisiblePosition = 5;
            ultraGridColumn24.Width = 152;
            ultraGridColumn25.Header.VisiblePosition = 6;
            ultraGridColumn25.Width = 152;
            ultraGridColumn26.Header.VisiblePosition = 7;
            ultraGridColumn26.Width = 152;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn19,
            ultraGridColumn20,
            ultraGridColumn21,
            ultraGridColumn22,
            ultraGridColumn23,
            ultraGridColumn24,
            ultraGridColumn25,
            ultraGridColumn26});
            ultraGridBand2.Hidden = true;
            this.grdOrders.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdOrders.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdOrders.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance20.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance20.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance20.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance20.BorderColor = System.Drawing.SystemColors.Window;
            this.grdOrders.DisplayLayout.GroupByBox.Appearance = appearance20;
            appearance21.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdOrders.DisplayLayout.GroupByBox.BandLabelAppearance = appearance21;
            this.grdOrders.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance22.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance22.BackColor2 = System.Drawing.SystemColors.Control;
            appearance22.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance22.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdOrders.DisplayLayout.GroupByBox.PromptAppearance = appearance22;
            this.grdOrders.DisplayLayout.MaxColScrollRegions = 1;
            this.grdOrders.DisplayLayout.MaxRowScrollRegions = 1;
            appearance23.BackColor = System.Drawing.SystemColors.Window;
            appearance23.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdOrders.DisplayLayout.Override.ActiveCellAppearance = appearance23;
            appearance24.BackColor = System.Drawing.SystemColors.Highlight;
            appearance24.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdOrders.DisplayLayout.Override.ActiveRowAppearance = appearance24;
            this.grdOrders.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdOrders.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdOrders.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.grdOrders.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdOrders.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance25.BackColor = System.Drawing.SystemColors.Window;
            this.grdOrders.DisplayLayout.Override.CardAreaAppearance = appearance25;
            appearance26.BorderColor = System.Drawing.Color.Silver;
            appearance26.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdOrders.DisplayLayout.Override.CellAppearance = appearance26;
            this.grdOrders.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdOrders.DisplayLayout.Override.CellPadding = 0;
            appearance27.BackColor = System.Drawing.SystemColors.Control;
            appearance27.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance27.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance27.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance27.BorderColor = System.Drawing.SystemColors.Window;
            this.grdOrders.DisplayLayout.Override.GroupByRowAppearance = appearance27;
            appearance28.TextHAlignAsString = "Left";
            this.grdOrders.DisplayLayout.Override.HeaderAppearance = appearance28;
            this.grdOrders.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdOrders.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance29.BackColor = System.Drawing.SystemColors.Window;
            appearance29.BorderColor = System.Drawing.Color.Silver;
            this.grdOrders.DisplayLayout.Override.RowAppearance = appearance29;
            this.grdOrders.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdOrders.DisplayLayout.Override.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.BottomFixed;
            appearance30.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdOrders.DisplayLayout.Override.TemplateAddRowAppearance = appearance30;
            this.grdOrders.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdOrders.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdOrders.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdOrders.Location = new System.Drawing.Point(0, 0);
            this.grdOrders.Name = "grdOrders";
            this.grdOrders.Size = new System.Drawing.Size(1232, 145);
            this.grdOrders.TabIndex = 1;
            this.grdOrders.Text = "Orders";
            this.grdOrders.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdOrders_InitializeLayout);
            this.grdOrders.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.grdOrders_AfterSelectChange);
            this.grdOrders.ClickCell += new Infragistics.Win.UltraWinGrid.ClickCellEventHandler(this.grdOrders_ClickCell);
            this.grdOrders.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.grdOrders_DoubleClickRow);
            // 
            // dsSchedule
            // 
            this.dsSchedule.DataSetName = "ScheduleDataset";
            this.dsSchedule.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.timeline);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(1232, 583);
            // 
            // timeline
            // 
            this.timeline.CalendarInfo = this.calInfo;
            this.timeline.CalendarLook = this.calLook;
            this.timeline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeline.Location = new System.Drawing.Point(0, 0);
            this.timeline.Name = "timeline";
            this.timeline.OwnerHeaderTextOrientation = new Infragistics.Win.TextOrientationInfo(0, Infragistics.Win.TextFlowDirection.Horizontal);
            this.timeline.PrimaryInterval = dateInterval1;
            this.timeline.Size = new System.Drawing.Size(1232, 583);
            this.timeline.TabIndex = 4;
            // 
            // ultraPanel1
            // 
            // 
            // ultraPanel1.ClientArea
            // 
            this.ultraPanel1.ClientArea.Controls.Add(this.guagePercentLate);
            this.ultraPanel1.ClientArea.Controls.Add(this.guageAvgDaysLate);
            this.ultraPanel1.ClientArea.Controls.Add(this.guageTotalOrders);
            this.ultraPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel1.Location = new System.Drawing.Point(8, 149);
            this.ultraPanel1.Name = "ultraPanel1";
            this.ultraPanel1.Size = new System.Drawing.Size(1236, 611);
            this.ultraPanel1.TabIndex = 3;
            // 
            // guagePercentLate
            // 
            this.guagePercentLate.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guagePercentLate.GuageLabel = "Percent Late";
            this.guagePercentLate.Location = new System.Drawing.Point(866, 22);
            this.guagePercentLate.Name = "guagePercentLate";
            this.guagePercentLate.Size = new System.Drawing.Size(134, 71);
            this.guagePercentLate.TabIndex = 1;
            // 
            // guageAvgDaysLate
            // 
            this.guageAvgDaysLate.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guageAvgDaysLate.GuageLabel = "Avg Days Late";
            this.guageAvgDaysLate.Location = new System.Drawing.Point(575, 22);
            this.guageAvgDaysLate.Name = "guageAvgDaysLate";
            this.guageAvgDaysLate.Size = new System.Drawing.Size(134, 71);
            this.guageAvgDaysLate.TabIndex = 3;
            // 
            // guageTotalOrders
            // 
            this.guageTotalOrders.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guageTotalOrders.GuageLabel = "Total Orders";
            this.guageTotalOrders.Location = new System.Drawing.Point(715, 22);
            this.guageTotalOrders.Name = "guageTotalOrders";
            this.guageTotalOrders.Size = new System.Drawing.Size(134, 71);
            this.guageTotalOrders.TabIndex = 2;
            // 
            // _ProcessingSchedule_Toolbars_Dock_Area_Left
            // 
            this._ProcessingSchedule_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ProcessingSchedule_Toolbars_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._ProcessingSchedule_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._ProcessingSchedule_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ProcessingSchedule_Toolbars_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._ProcessingSchedule_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 149);
            this._ProcessingSchedule_Toolbars_Dock_Area_Left.Name = "_ProcessingSchedule_Toolbars_Dock_Area_Left";
            this._ProcessingSchedule_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(8, 632);
            this._ProcessingSchedule_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarManager;
            // 
            // toolbarManager
            // 
            this.toolbarManager.DesignerFlags = 1;
            this.toolbarManager.DockWithinContainer = this;
            this.toolbarManager.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
            this.toolbarManager.Ribbon.FileMenuStyle = Infragistics.Win.UltraWinToolbars.FileMenuStyle.ApplicationMenu2010;
            ribbonTab1.Caption = "Schedule";
            ribbonGroup1.Caption = "Tools";
            ribbonGroup1.PreferredToolSize = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool15,
            buttonTool1,
            buttonTool7,
            buttonTool14,
            buttonTool20});
            ribbonGroup2.Caption = "Reports";
            ribbonGroup2.PreferredToolSize = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool18,
            buttonTool19});
            ribbonGroup3.Caption = "Settings";
            ribbonGroup3.PreferredToolSize = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool12,
            buttonTool13});
            ribbonGroup4.Caption = "Status";
            ribbonGroup4.PreferredToolSize = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            controlContainerTool2.ControlName = "guageTotalOrders";
            controlContainerTool2.InstanceProps.Width = 134;
            controlContainerTool3.ControlName = "guageAvgDaysLate";
            controlContainerTool3.InstanceProps.Width = 134;
            controlContainerTool4.ControlName = "guagePercentLate";
            controlContainerTool4.InstanceProps.Width = 134;
            ribbonGroup4.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            controlContainerTool2,
            controlContainerTool3,
            controlContainerTool4});
            ribbonTab1.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup1,
            ribbonGroup2,
            ribbonGroup3,
            ribbonGroup4});
            this.toolbarManager.Ribbon.NonInheritedRibbonTabs.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonTab[] {
            ribbonTab1});
            this.toolbarManager.Ribbon.Visible = true;
            this.toolbarManager.ShowFullMenusDelay = 500;
            this.toolbarManager.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2010;
            controlContainerTool1.ControlName = "guageTotalOrders";
            controlContainerTool1.SharedPropsInternal.Width = 134;
            controlContainerTool5.ControlName = "guageAvgDaysLate";
            controlContainerTool5.SharedPropsInternal.Width = 134;
            controlContainerTool6.ControlName = "guagePercentLate";
            controlContainerTool6.SharedPropsInternal.Width = 134;
            appearance3.Image = global::DWOS.UI.Properties.Resources.Settings_32;
            buttonTool3.SharedPropsInternal.AppearancesLarge.Appearance = appearance3;
            buttonTool3.SharedPropsInternal.Caption = "Schedule Settings";
            buttonTool3.SharedPropsInternal.ToolTipText = "Settings";
            buttonTool3.SharedPropsInternal.ToolTipTitle = "Schedule Settings";
            appearance4.Image = global::DWOS.UI.Properties.Resources.Print_32;
            buttonTool4.SharedPropsInternal.AppearancesLarge.Appearance = appearance4;
            buttonTool4.SharedPropsInternal.Caption = "Work Schedule";
            buttonTool4.SharedPropsInternal.ToolTipText = "Work Schedule";
            buttonTool4.SharedPropsInternal.ToolTipTitle = "Work Schedule";
            appearance5.Image = global::DWOS.UI.Properties.Resources.Print_32;
            buttonTool6.SharedPropsInternal.AppearancesLarge.Appearance = appearance5;
            buttonTool6.SharedPropsInternal.Caption = "Schedule Summary";
            buttonTool6.SharedPropsInternal.ToolTipText = "Schedule Summary";
            buttonTool6.SharedPropsInternal.ToolTipTitle = "Schedule Summary";
            appearance6.Image = global::DWOS.UI.Properties.Resources.Search_48;
            buttonTool9.SharedPropsInternal.AppearancesLarge.Appearance = appearance6;
            appearance7.Image = global::DWOS.UI.Properties.Resources.Search_16;
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance7;
            buttonTool9.SharedPropsInternal.Caption = "Find";
            buttonTool9.SharedPropsInternal.ToolTipText = "Find";
            buttonTool9.SharedPropsInternal.ToolTipTitle = "Find";
            buttonTool10.SharedPropsInternal.Caption = "ButtonTool1";
            appearance8.Image = global::DWOS.UI.Properties.Resources.Save_32;
            buttonTool11.SharedPropsInternal.AppearancesLarge.Appearance = appearance8;
            buttonTool11.SharedPropsInternal.Caption = "Save Settings";
            buttonTool11.SharedPropsInternal.ToolTipText = "Save Settings";
            buttonTool11.SharedPropsInternal.ToolTipTitle = "Save Settings";
            appearance9.Image = global::DWOS.UI.Properties.Resources.Reload_32;
            buttonTool16.SharedPropsInternal.AppearancesLarge.Appearance = appearance9;
            appearance10.Image = global::DWOS.UI.Properties.Resources.Reload_32;
            buttonTool16.SharedPropsInternal.AppearancesSmall.Appearance = appearance10;
            buttonTool16.SharedPropsInternal.Caption = "Reload";
            buttonTool16.SharedPropsInternal.ToolTipText = "Reload";
            buttonTool16.SharedPropsInternal.ToolTipTitle = "Reload";
            appearance11.Image = global::DWOS.UI.Properties.Resources.Save_32;
            buttonTool17.SharedPropsInternal.AppearancesLarge.Appearance = appearance11;
            appearance12.Image = global::DWOS.UI.Properties.Resources.Save_16;
            buttonTool17.SharedPropsInternal.AppearancesSmall.Appearance = appearance12;
            buttonTool17.SharedPropsInternal.Caption = "Save Schedule";
            buttonTool17.SharedPropsInternal.ToolTipText = "Save the new calculated ship dates.";
            buttonTool17.SharedPropsInternal.ToolTipTitle = "Save Schedule";
            appearance13.Image = global::DWOS.UI.Properties.Resources.Priority_32;
            buttonTool8.SharedPropsInternal.AppearancesLarge.Appearance = appearance13;
            appearance14.Image = global::DWOS.UI.Properties.Resources.Priority_32;
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance14;
            buttonTool8.SharedPropsInternal.Caption = "Order Priority";
            buttonTool8.SharedPropsInternal.ToolTipText = "Order Priority";
            buttonTool8.SharedPropsInternal.ToolTipTitle = "Order Priority";
            appearance15.Image = ((object)(resources.GetObject("appearance15.Image")));
            buttonTool5.SharedPropsInternal.AppearancesLarge.Appearance = appearance15;
            appearance16.Image = ((object)(resources.GetObject("appearance16.Image")));
            buttonTool5.SharedPropsInternal.AppearancesSmall.Appearance = appearance16;
            buttonTool5.SharedPropsInternal.Caption = "Differences";
            buttonTool5.SharedPropsInternal.ToolTipText = "Differences";
            buttonTool5.SharedPropsInternal.ToolTipTitle = "Differences";
            this.toolbarManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            controlContainerTool1,
            controlContainerTool5,
            controlContainerTool6,
            buttonTool3,
            buttonTool4,
            buttonTool6,
            buttonTool9,
            buttonTool10,
            buttonTool11,
            buttonTool16,
            buttonTool17,
            buttonTool8,
            buttonTool5});
            this.toolbarManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.ultraToolbarsManager1_ToolClick);
            // 
            // _ProcessingSchedule_Toolbars_Dock_Area_Right
            // 
            this._ProcessingSchedule_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ProcessingSchedule_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._ProcessingSchedule_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._ProcessingSchedule_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ProcessingSchedule_Toolbars_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._ProcessingSchedule_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(1244, 149);
            this._ProcessingSchedule_Toolbars_Dock_Area_Right.Name = "_ProcessingSchedule_Toolbars_Dock_Area_Right";
            this._ProcessingSchedule_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(8, 632);
            this._ProcessingSchedule_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarManager;
            // 
            // _ProcessingSchedule_Toolbars_Dock_Area_Top
            // 
            this._ProcessingSchedule_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ProcessingSchedule_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._ProcessingSchedule_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._ProcessingSchedule_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ProcessingSchedule_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._ProcessingSchedule_Toolbars_Dock_Area_Top.Name = "_ProcessingSchedule_Toolbars_Dock_Area_Top";
            this._ProcessingSchedule_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(1252, 149);
            this._ProcessingSchedule_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarManager;
            // 
            // _ProcessingSchedule_Toolbars_Dock_Area_Bottom
            // 
            this._ProcessingSchedule_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ProcessingSchedule_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._ProcessingSchedule_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._ProcessingSchedule_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ProcessingSchedule_Toolbars_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._ProcessingSchedule_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 781);
            this._ProcessingSchedule_Toolbars_Dock_Area_Bottom.Name = "_ProcessingSchedule_Toolbars_Dock_Area_Bottom";
            this._ProcessingSchedule_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(1252, 8);
            this._ProcessingSchedule_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarManager;
            // 
            // _ProcessingScheduleUnpinnedTabAreaLeft
            // 
            this._ProcessingScheduleUnpinnedTabAreaLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this._ProcessingScheduleUnpinnedTabAreaLeft.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._ProcessingScheduleUnpinnedTabAreaLeft.Location = new System.Drawing.Point(8, 149);
            this._ProcessingScheduleUnpinnedTabAreaLeft.Name = "_ProcessingScheduleUnpinnedTabAreaLeft";
            this._ProcessingScheduleUnpinnedTabAreaLeft.Owner = this.ultraDockManager1;
            this._ProcessingScheduleUnpinnedTabAreaLeft.Size = new System.Drawing.Size(0, 632);
            this.inboxControlStyler1.SetStyleSettings(this._ProcessingScheduleUnpinnedTabAreaLeft, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this._ProcessingScheduleUnpinnedTabAreaLeft.TabIndex = 14;
            // 
            // ultraDockManager1
            // 
            this.ultraDockManager1.CompressUnpinnedTabs = false;
            dockableControlPane1.Control = this.notificationPanel;
            dockableControlPane1.FlyoutSize = new System.Drawing.Size(-1, 206);
            dockableControlPane1.Key = "notificationPane";
            dockableControlPane1.OriginalControlBounds = new System.Drawing.Rectangle(520, 500, 659, 277);
            dockableControlPane1.Pinned = false;
            appearance1.Image = global::DWOS.UI.Properties.Resources.Info_30;
            dockableControlPane1.Settings.TabAppearance = appearance1;
            dockableControlPane1.Size = new System.Drawing.Size(100, 100);
            dockableControlPane1.Text = "Notifications";
            dockAreaPane1.Panes.AddRange(new Infragistics.Win.UltraWinDock.DockablePaneBase[] {
            dockableControlPane1});
            dockAreaPane1.Size = new System.Drawing.Size(1244, 206);
            this.ultraDockManager1.DockAreas.AddRange(new Infragistics.Win.UltraWinDock.DockAreaPane[] {
            dockAreaPane1});
            this.ultraDockManager1.HostControl = this;
            // 
            // _ProcessingScheduleUnpinnedTabAreaRight
            // 
            this._ProcessingScheduleUnpinnedTabAreaRight.Dock = System.Windows.Forms.DockStyle.Right;
            this._ProcessingScheduleUnpinnedTabAreaRight.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._ProcessingScheduleUnpinnedTabAreaRight.Location = new System.Drawing.Point(1244, 149);
            this._ProcessingScheduleUnpinnedTabAreaRight.Name = "_ProcessingScheduleUnpinnedTabAreaRight";
            this._ProcessingScheduleUnpinnedTabAreaRight.Owner = this.ultraDockManager1;
            this._ProcessingScheduleUnpinnedTabAreaRight.Size = new System.Drawing.Size(0, 632);
            this.inboxControlStyler1.SetStyleSettings(this._ProcessingScheduleUnpinnedTabAreaRight, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this._ProcessingScheduleUnpinnedTabAreaRight.TabIndex = 15;
            // 
            // _ProcessingScheduleUnpinnedTabAreaTop
            // 
            this._ProcessingScheduleUnpinnedTabAreaTop.Dock = System.Windows.Forms.DockStyle.Top;
            this._ProcessingScheduleUnpinnedTabAreaTop.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._ProcessingScheduleUnpinnedTabAreaTop.Location = new System.Drawing.Point(8, 149);
            this._ProcessingScheduleUnpinnedTabAreaTop.Name = "_ProcessingScheduleUnpinnedTabAreaTop";
            this._ProcessingScheduleUnpinnedTabAreaTop.Owner = this.ultraDockManager1;
            this._ProcessingScheduleUnpinnedTabAreaTop.Size = new System.Drawing.Size(1236, 0);
            this.inboxControlStyler1.SetStyleSettings(this._ProcessingScheduleUnpinnedTabAreaTop, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this._ProcessingScheduleUnpinnedTabAreaTop.TabIndex = 16;
            // 
            // _ProcessingScheduleUnpinnedTabAreaBottom
            // 
            this._ProcessingScheduleUnpinnedTabAreaBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._ProcessingScheduleUnpinnedTabAreaBottom.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._ProcessingScheduleUnpinnedTabAreaBottom.Location = new System.Drawing.Point(8, 760);
            this._ProcessingScheduleUnpinnedTabAreaBottom.Name = "_ProcessingScheduleUnpinnedTabAreaBottom";
            this._ProcessingScheduleUnpinnedTabAreaBottom.Owner = this.ultraDockManager1;
            this._ProcessingScheduleUnpinnedTabAreaBottom.Size = new System.Drawing.Size(1236, 21);
            this.inboxControlStyler1.SetStyleSettings(this._ProcessingScheduleUnpinnedTabAreaBottom, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this._ProcessingScheduleUnpinnedTabAreaBottom.TabIndex = 17;
            // 
            // _ProcessingScheduleAutoHideControl
            // 
            this._ProcessingScheduleAutoHideControl.Controls.Add(this.dockableWindow1);
            this._ProcessingScheduleAutoHideControl.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._ProcessingScheduleAutoHideControl.Location = new System.Drawing.Point(4, 753);
            this._ProcessingScheduleAutoHideControl.Name = "_ProcessingScheduleAutoHideControl";
            this._ProcessingScheduleAutoHideControl.Owner = this.ultraDockManager1;
            this._ProcessingScheduleAutoHideControl.Size = new System.Drawing.Size(1244, 11);
            this.inboxControlStyler1.SetStyleSettings(this._ProcessingScheduleAutoHideControl, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this._ProcessingScheduleAutoHideControl.TabIndex = 18;
            // 
            // dockableWindow1
            // 
            this.dockableWindow1.Controls.Add(this.notificationPanel);
            this.dockableWindow1.Location = new System.Drawing.Point(0, 5);
            this.dockableWindow1.Name = "dockableWindow1";
            this.dockableWindow1.Owner = this.ultraDockManager1;
            this.dockableWindow1.Size = new System.Drawing.Size(1244, 206);
            this.inboxControlStyler1.SetStyleSettings(this.dockableWindow1, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.dockableWindow1.TabIndex = 25;
            // 
            // windowDockingArea1
            // 
            this.windowDockingArea1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.windowDockingArea1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.windowDockingArea1.Location = new System.Drawing.Point(4, 553);
            this.windowDockingArea1.Name = "windowDockingArea1";
            this.windowDockingArea1.Owner = this.ultraDockManager1;
            this.windowDockingArea1.Size = new System.Drawing.Size(1244, 211);
            this.inboxControlStyler1.SetStyleSettings(this.windowDockingArea1, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.windowDockingArea1.TabIndex = 20;
            // 
            // ultraTabControl1
            // 
            this.ultraTabControl1.Controls.Add(this.ultraTabSharedControlsPage1);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl1);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl2);
            this.ultraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraTabControl1.Location = new System.Drawing.Point(8, 149);
            this.ultraTabControl1.Name = "ultraTabControl1";
            this.ultraTabControl1.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.ultraTabControl1.Size = new System.Drawing.Size(1236, 611);
            this.ultraTabControl1.TabIndex = 4;
            ultraTab2.TabPage = this.ultraTabPageControl1;
            ultraTab2.Text = "Department Schedule";
            ultraTab3.TabPage = this.ultraTabPageControl2;
            ultraTab3.Text = "Order Schedule";
            this.ultraTabControl1.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab2,
            ultraTab3});
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(1232, 585);
            // 
            // ProcessingSchedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1252, 789);
            this.Controls.Add(this._ProcessingScheduleAutoHideControl);
            this.Controls.Add(this.ultraTabControl1);
            this.Controls.Add(this.ultraPanel1);
            this.Controls.Add(this.windowDockingArea1);
            this.Controls.Add(this._ProcessingScheduleUnpinnedTabAreaTop);
            this.Controls.Add(this._ProcessingScheduleUnpinnedTabAreaBottom);
            this.Controls.Add(this._ProcessingScheduleUnpinnedTabAreaLeft);
            this.Controls.Add(this._ProcessingScheduleUnpinnedTabAreaRight);
            this.Controls.Add(this._ProcessingSchedule_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._ProcessingSchedule_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._ProcessingSchedule_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this._ProcessingSchedule_Toolbars_Dock_Area_Top);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProcessingSchedule";
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Processing Schedule";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProcessingSchedule_FormClosing);
            this.Load += new System.EventHandler(this.ProcessingSchedule_Load);
            this.Shown += new System.EventHandler(this.ProcessingSchedule_Shown);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvSchedule)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdOrders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsSchedule)).EndInit();
            this.ultraTabPageControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.timeline)).EndInit();
            this.ultraPanel1.ClientArea.ResumeLayout(false);
            this.ultraPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDockManager1)).EndInit();
            this._ProcessingScheduleAutoHideControl.ResumeLayout(false);
            this.dockableWindow1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl1)).EndInit();
            this.ultraTabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinGanttView.UltraGanttView gvSchedule;
		private DWOS.Data.Datasets.ScheduleDataset dsSchedule;
		private Infragistics.Win.UltraWinSchedule.UltraCalendarInfo calInfo;
		private Infragistics.Win.UltraWinGrid.UltraGrid grdOrders;
		private Infragistics.Win.Misc.UltraPanel ultraPanel1;
		private Infragistics.Win.UltraWinSchedule.UltraCalendarLook calLook;
		private Utilities.DigitalGuage guagePercentLate;
		private Utilities.DigitalGuage guageTotalOrders;
        private Utilities.DigitalGuage guageAvgDaysLate;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ProcessingSchedule_Toolbars_Dock_Area_Left;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager toolbarManager;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ProcessingSchedule_Toolbars_Dock_Area_Right;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ProcessingSchedule_Toolbars_Dock_Area_Top;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ProcessingSchedule_Toolbars_Dock_Area_Bottom;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
        private Infragistics.Win.UltraWinDock.AutoHideControl _ProcessingScheduleAutoHideControl;
        private Infragistics.Win.UltraWinDock.UltraDockManager ultraDockManager1;
        private Infragistics.Win.UltraWinDock.UnpinnedTabArea _ProcessingScheduleUnpinnedTabAreaBottom;
        private Infragistics.Win.UltraWinDock.UnpinnedTabArea _ProcessingScheduleUnpinnedTabAreaTop;
        private Infragistics.Win.UltraWinDock.UnpinnedTabArea _ProcessingScheduleUnpinnedTabAreaRight;
        private Infragistics.Win.UltraWinDock.UnpinnedTabArea _ProcessingScheduleUnpinnedTabAreaLeft;
        private Infragistics.Win.UltraWinDock.DockableWindow dockableWindow1;
        private Utilities.NotificationPanel notificationPanel;
        private Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea1;
        private Infragistics.Win.UltraWinSchedule.UltraTimelineView timeline;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl ultraTabControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
	}
}