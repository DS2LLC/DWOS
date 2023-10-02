using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Admin.Schedule.Manual;
using DWOS.UI.Dashboard;
using DWOS.UI.Properties;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Windows.DockManager;
using Infragistics.Windows.DockManager.Events;
using NLog;

namespace DWOS.UI
{
    internal partial class Main : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private IDwosTab _activeTab;

        private CommandManager _commands;
        private bool _isClosing;
        private MainDock _mainDockManager;
        private RetryPolicy _retryPolicy = new RetryPolicy(TimeSpan.FromSeconds(ApplicationSettings.Current.ClientUpdateIntervalSeconds));
        private List <TabData> _tabs = new List <TabData>();
        private BatchSummaryBarcodeScanner _batchScanner;
        private OrderSummaryBarcodeScanner _orderScanner;
        private BadgeCountManager _badgeCountManager;
        private int _isRefreshing; // type is int because Interlocked does not support atomic operations on bool
        private WipData _wipData;
        private object _timerNextTabLock = new object();
        private readonly Data.Date.ICalendarPersistence _calendarPersistence;
        private bool _showedLowDiskSpaceError;

        public event EventHandler SelectedGridRowChanged;
        public event EventHandler SelectedTabChanged;
        public event EventHandler DataRefreshed;
        
        private static readonly string WORK_STATUS_PARTMARKING =  ApplicationSettings.Current.WorkStatusPartMarking;
        private static readonly string WORK_STATUS_FINALINSPECTION =  ApplicationSettings.Current.WorkStatusFinalInspection;
        private static readonly string WORK_STATUS_PENDINGINSPECTION =  ApplicationSettings.Current.WorkStatusPendingQI;
        private static readonly string WORK_STATUS_SHIPPING =  ApplicationSettings.Current.WorkStatusShipping;
        private static readonly string WORK_STATUS_PENDINGREWORKPLANNING =  ApplicationSettings.Current.WorkStatusPendingReworkPlanning;
        private static readonly string WORK_STATUS_PENDINGOR =  ApplicationSettings.Current.WorkStatusPendingOR;

        private static readonly string WORK_STATUS_PENDING_IMPORT_EXPORT_REVIEW =
            ApplicationSettings.Current.WorkStatusPendingImportExportReview;

        private static readonly string WORK_STATUS_PENDINGJOIN =  ApplicationSettings.Current.WorkStatusPendingJoin;
        private static readonly string WORK_STATUS_HOLD =  ApplicationSettings.Current.WorkStatusHold;
        private static readonly string WORK_STATUS_INPROCESS =  ApplicationSettings.Current.WorkStatusInProcess;
        private static readonly DateTime LAST_PROCESS_DATE_FLAG = DateTime.Now.AddYears(-10);

        #endregion

        #region Properties

        public FlyoutManager FlyoutManager { get; }

        public bool HasOrderData => _wipData?.DataType.HasFlag(WipData.WipDataType.Orders) ?? false;

        public bool HasBatchData => _wipData?.DataType.HasFlag(WipData.WipDataType.Batches) ?? false;

        internal bool EnableRefresh { get; set; } = true;

        internal int SelectedWO
        {
            get { return ActiveTab is IOrderSummary ? ((IOrderSummary) ActiveTab).SelectedWO : -1; }
        }

        internal int SelectedPartID
        {
            get
            {
                int orderID = SelectedWO;

                if(orderID > 0)
                    return this.taOrderStatus.GetPartID(orderID).GetValueOrDefault(0);

                return 0;
            }
        }

        internal string SelectedWorkStatus
        {
            get { return ActiveTab is IOrderSummary ? ((IOrderSummary) ActiveTab).SelectedWorkStatus : null; }
        }

        internal bool SelectedHoldStatus
        {
            get { return ActiveTab is IOrderSummary && ((IOrderSummary) ActiveTab).SelectedHoldStatus.GetValueOrDefault(); }
        }

        internal OrderType? SelectedOrderType
        {
            get { return ActiveTab is IOrderSummary ? ((IOrderSummary) ActiveTab).SelectedOrderType : null; }
        }

        internal string SelectedLocation
        {
            get { return ActiveTab is IOrderSummary ? ((IOrderSummary) ActiveTab).SelectedLocation : null; }
        }

        internal int? SelectedLine => (ActiveTab as IOrderSummary)?.SelectedLine;

        internal bool SelectedInBatch
        {
            get { return this.ActiveTab is IOrderSummary && ((IOrderSummary)this.ActiveTab).SelectedInBatch; }
        }

        internal int SelectedActiveTimerCount
        {
            get
            {
                return (this.ActiveTab as IOrderSummary)?.SelectedActiveTimerCount ?? 0;
            }
        }

        internal CommandManager Commands
        {
            get { return this._commands; }
            set { this._commands = value; }
        }

        internal IDwosTab ActiveTab
        {
            get { return this._activeTab; }
            private set
            {
                if(value != null)
                {
                    this._activeTab = value;

                    if(SelectedTabChanged != null)
                        SelectedTabChanged(this, EventArgs.Empty);

                    if (!this.IsInPresentationMode)
                    {
                        if(this._activeTab is DashboardTab)
                        {
                            this.toolbarManager.Ribbon.ContextualTabGroups["Dashboard"].Visible = true;
                            this.toolbarManager.Ribbon.SelectedTab = this.toolbarManager.Ribbon.Tabs["Dashboard"];
                        }
                        else
                            this.toolbarManager.Ribbon.ContextualTabGroups["Dashboard"].Visible = false;

                        if(this._activeTab is BatchSummary)
                        {
                            this.toolbarManager.Ribbon.ContextualTabGroups["Batches"].Visible = true;
                            this.toolbarManager.Ribbon.SelectedTab = this.toolbarManager.Ribbon.Tabs["Batches"];
                        }
                        else
                            this.toolbarManager.Ribbon.ContextualTabGroups["Batches"].Visible = false;
                    }
                }
            }
        }

        internal BadgeCountManager BadgeCounter 
        {
            get { return _badgeCountManager ?? (_badgeCountManager = new BadgeCountManager(this.toolbarManager)); }
        }

        internal bool IsInPresentationMode
        {
            get
            {
                lock (this._timerNextTabLock)
                {
                    return this.timerNextTab.Enabled;
                }
            }
        }


        #endregion

        #region Methods

        public Main(Data.Date.ICalendarPersistence calendarPersistence)
        {
            _calendarPersistence = calendarPersistence ?? throw new ArgumentNullException(nameof(calendarPersistence));
            FlyoutManager = new FlyoutManager(this);
            InitializeComponent();
            UpdateRibbonVisibility();

#if NIGHTLY
            Text = $@"{System.Windows.Forms.Application.ProductName} {System.Windows.Forms.Application.ProductVersion } - {About.ApplicationReleaseDate:d} Nightly";
#elif DEBUG
            Text = $@"{System.Windows.Forms.Application.ProductName} {System.Windows.Forms.Application.ProductVersion } - Dev";
#else
            Text = System.Windows.Forms.Application.ProductName + " " + System.Windows.Forms.Application.ProductVersion;
#endif
            Settings.Default.SettingsSaving += (s, e) => { this.ultraTouchProvider1.Enabled = Settings.Default.TouchEnabled; };

            //create main window dock manager
            this.mainWindowHost.Child = this._mainDockManager = new MainDock();
            this._mainDockManager.dockManager.ActivePaneChanged += dockManager_ActivePaneChanged;

            this.userProfileThumb.ResetUser();
            LoadCommands();

            SecurityManager.Current.UserUpdated += SecurityManager_OnUserChange; //wire up before Form.Shown event fired and after LoadCommands (Ensure UpdateRibbonVisibility after Commands updated)

            //set the thumbnail image based on the the type of authentication provider we are using
            this.statusBar.Panels[0].Appearance.Image = SecurityManager.Current.AuthenticationProviderThumbnail;

            RestoreLayouts();
        }

        private void LoadData()
        {
            try
            {
                if (_isClosing)
                {
                    return;
                }

                if (!BeginLoadingData(out var reason))
                {
                    _log.Info("Skipping refresh: {0}", reason);
                    return;
                }

                Task.Factory.StartNew(() =>
                {
                    if(this.dsOrderStatus.Priority.Rows.Count < 1)
                        this.taPriority.Fill(this.dsOrderStatus.Priority);

                    var preloadSettings = WipData.WipDataType.None;

                    //update order summary if anyone needs it
                    if(_tabs.Exists(tb => tb.Tab is IOrderSummary || tb.Tab is ISchedulingTab))
                    {
                        dsOrderStatus.OrderStatus.BeginLoadData();
                        this.taOrderStatus.FillForClientDisplay(this.dsOrderStatus.OrderStatus);
                        AdjustOrderData(dsOrderStatus.OrderStatus);

                        dsOrderStatus.OrderStatus.EndLoadData();
                        preloadSettings = preloadSettings | WipData.WipDataType.Orders;
                    }

                    //update batch summary if anyone needs it
                    if(_tabs.Exists(tb => tb.Tab is IBatchSummary || tb.Tab is ISchedulingTab))
                    {
                        dsOrderStatus.BatchStatus.BeginLoadData();
                        dsOrderStatus.BatchOrderStatus.BeginLoadData();

                        this.taBatchStatus.FillForClientDisplay(this.dsOrderStatus.BatchStatus);
                        this.taBatchOrderStatus.FillActive(this.dsOrderStatus.BatchOrderStatus);

                        dsOrderStatus.BatchOrderStatus.EndLoadData();
                        dsOrderStatus.BatchStatus.EndLoadData();
                        preloadSettings = preloadSettings | WipData.WipDataType.Batches;
                    }

                    return new WipData(dsOrderStatus, preloadSettings);
                }, TaskCreationOptions.LongRunning).ContinueWith(originalTask =>
                {
                    var orderCount = 0;
                    try
                    {
                        if (originalTask.Exception != null)
                        {
                            _retryPolicy.OnFailure();
                            ErrorMessageBox.ShowDialog("Error loading data from database server.", originalTask.Exception);
                            return;
                        }

                        _wipData = originalTask.Result;
                        orderCount = _wipData.Orders.Count;

                        using (new UsingWaitCursor(this))
                        {
                            foreach(var tabData in this._tabs)
                            {
                                tabData.Tab.RefreshData(_wipData);
                            }
                        }

                        if (!this._isClosing)
                        {
                            this._commands.RefreshAll();

                            DataRefreshed?.Invoke(this, EventArgs.Empty);

                            this._retryPolicy.OnSuccess();
                        }
                    }
                    catch(Exception exc)
                    {
                       this._retryPolicy.OnFailure();
                        ErrorMessageBox.ShowDialog("Error loading data from database server.", exc);
                    }
                    finally
                    {
                        UpdateStatusBar(orderCount);
                        FinishLoadingData();
                    }
                }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch(Exception exc)
            {
                this._retryPolicy.OnFailure();
                ErrorMessageBox.ShowDialog("Error loading data from database server.", exc);
            }
        }

        private void UpdateStatusBar(int orderCount)
        {
            this.statusBar.Panels["TotalOrderCount"].Text = orderCount.ToString();
        }

        private void LoadCommands()
        {
            this._commands = new CommandManager();

            this._commands.AddCommand("LogIn", new LogInCommand(this.toolbarManager.Tools["LogIn"], this));
            this._commands.AddCommand("Help", new HelpCommand(this.toolbarManager.Tools["Help"], this));
            this._commands.AddCommand("LoggedInUsers", new UserActivationsCommand(this.toolbarManager.Tools["LoggedInUsers"]));

            if(ApplicationSettings.Current.LoginType == LoginType.PinOrSmartcard)
            {
                this.toolbarManager.Tools["AuthenticationType"].SharedProps.Visible = true;
                this.toolbarManager.Tools["LogIn"].SharedProps.Visible = false;
                this.toolbarManager.Tools["AuthenticationType"].SharedPropsInternal.AppearancesLarge.Appearance.Image = Properties.Resources.duel_login;
                this._commands.AddCommand("Pin", new PinAuthenticationCommand(this.toolbarManager.Tools["Pin"], this.toolbarManager.Tools["AuthenticationType"], this.toolbarManager.Tools["LogIn"], this.statusBar));
                this._commands.AddCommand("Smartcard", new SmartcardAuthenticationCommand(this.toolbarManager.Tools["Smartcard"], this.toolbarManager.Tools["AuthenticationType"], this.toolbarManager.Tools["LogIn"], this.statusBar));
            }
            else
                this.toolbarManager.Tools["AuthenticationType"].SharedProps.Visible = false;

            //Create COC command if app uses it
            if(ApplicationSettings.Current.COCEnabled)
                this._commands.AddCommand("COC", new COCCommand(this.toolbarManager.Tools["COC"], this));
            else
                this.toolbarManager.Tools["COC"].SharedProps.Visible = false;

            //Create Part Marking command if app uses it
            if(ApplicationSettings.Current.PartMarkingEnabled)
                this._commands.AddCommand("PartMarking", new PartMarkingCommand(this.toolbarManager.Tools["PartMarking"], this));
            else
                this.toolbarManager.Tools["PartMarking"].SharedProps.Visible = false;

            this._commands.AddCommand("cmdCheckForUpdates", new UpdateCommand(this.toolbarManager.Tools["cmdCheckForUpdates"]));
            this._commands.AddCommand("Update", new UpdateAvailableCommand(this.toolbarManager.Tools["Update"]));
            this._commands.AddCommand("BulkShipping", new ShippingManagerCommand(this.toolbarManager.Tools["BulkShipping"]));
            this._commands.AddCommand("Customers", new CustomerCommand(this.toolbarManager.Tools["Customers"]));
            this._commands.AddCommand("Orders", new OrderEntryCommand(this.toolbarManager.Tools["Orders"], this));
            this._commands.AddCommand("OrderReview", new OrderReviewCommand(this.toolbarManager.Tools["OrderReview"], this));
            this._commands.AddCommand("ImportExportReview", new ImportExportReviewCommand(this.toolbarManager.Tools["ImportExportReview"], this));
            this._commands.AddCommand("Parts", new PartsCommand(this.toolbarManager.Tools["Parts"], this));
            this._commands.AddCommand("Processes", new ProcessesCommand(this.toolbarManager.Tools["Processes"]));
            this._commands.AddCommand("ReviseWizard", new RevisePartProcessWizardCommand(this.toolbarManager.Tools["ReviseWizard"]));
            this._commands.AddCommand("ProcessPackages", new ProcessPackagesCommand(this.toolbarManager.Tools["ProcessPackages"]));
            this._commands.AddCommand("ListManager", new ListEditorCommand(this.toolbarManager.Tools["ListManager"]));
            this._commands.AddCommand("ProductClassManager", new ProductClassEditorCommand(this.toolbarManager.Tools["ProductClassManager"]));
            this._commands.AddCommand("OrderApprovalTermsManager", new OrderApprovalTermsEditorCommand(this.toolbarManager.Tools["OrderApprovalTermsManager"]));
            this._commands.AddCommand("WorkDescriptionEditor", new WorkDescriptionEditorCommand(this.toolbarManager.Tools["WorkDescriptionEditor"]));
            this._commands.AddCommand("OrderProcessing", new OrderProcessingCommand(this.toolbarManager.Tools["OrderProcessing"], this));
            //this._commands.AddCommand("BatchOrderProcessing", new BatchOrderProcessingCommand(this.toolbarManager.Tools["BatchOrderProcessing"]));
            this._commands.AddCommand("StartProcess", new StartProcessTimerCommand(this.toolbarManager.Tools["StartProcess"], this));
            this._commands.AddCommand("StopProcess", new StopProcessTimerCommand(this.toolbarManager.Tools["StopProcess"], this));
            this._commands.AddCommand("Inspection", new PartInspectionCommand(this.toolbarManager.Tools["Inspection"], this));
            this._commands.AddCommand("Quote", new QuoteCommand(this.toolbarManager.Tools["Quote"]));
            this._commands.AddCommand("UserManager", new UserManagerCommand(this.toolbarManager.Tools["UserManager"], this));

            var departmentLocationCommand = new DepartmentLocationCommand(this.toolbarManager.Tools["DeptLocation"], this);
            departmentLocationCommand.DepartmentChanged += DepartmentLocationCommandOnDepartmentChanged;
            this._commands.AddCommand("DeptLocation", departmentLocationCommand);

            this._commands.AddCommand("ProcessingLine", new ProcessingLineCommand(this.toolbarManager.Tools["ProcessingLine"], this));
            this._commands.AddCommand("ShipmentPackageTypeManager", new ShipmentPackageTypeManagerCommand(this.toolbarManager.Tools["ShipmentPackageTypeManager"]));
            this._commands.AddCommand("Refresh", new RefreshCommand(this.toolbarManager.Tools["Refresh"], this));
            this._commands.AddCommand("Exit", new ExitCommand(this.toolbarManager.Tools["Exit"], this));
            this._commands.AddCommand("PartCheckIn", new PartCheckInCommand(this.toolbarManager.Tools["PartCheckIn"], this));
            this._commands.AddCommand("Receiving", new ReceivingCommand(this.toolbarManager.Tools["Receiving"]));
            this._commands.AddCommand("RackParts", new RackPartsCommand(this.toolbarManager.Tools["RackParts"],this));

            this._commands.AddCommand("StartLabor", new StartLaborTimerCommand(this.toolbarManager.Tools["StartLabor"], this));
            this._commands.AddCommand("StopLabor", new StopLaborTimerCommand(this.toolbarManager.Tools["StopLabor"], this));
            this._commands.AddCommand("PauseLabor", new PauseLaborTimerCommand(this.toolbarManager.Tools["PauseLabor"], this));

            this._commands.AddCommand("Settings", new SettingsCommand(this.toolbarManager.Tools["Settings"]));
            this._commands.AddCommand("Sort", new SortGridCommand(this.toolbarManager.Tools["Sort"], this));
            this._commands.AddCommand("Dashboard", new DashboardCommand(this.toolbarManager.Tools["Dashboard"]));
            this._commands.AddCommand("OrderScheduling", new OrderScheduleCommand(this.toolbarManager.Tools["OrderScheduling"]));
            this._commands.AddCommand("SchedulingTab", new SchedulingTabCommand(this.toolbarManager.Tools["SchedulingTab"]));
            this._commands.AddCommand("btnHolidayManager", new HolidayManagerCommand(this.toolbarManager.Tools["btnHolidayManager"]));
            this._commands.AddCommand("WorkWeekManager", new WorkWeekManagerCommand(this.toolbarManager.Tools["WorkWeekManager"]));
            this._commands.AddCommand("OrderPriority", new OrderPriorityCommand(this.toolbarManager.Tools["OrderPriority"]));
            this._commands.AddCommand("PrintGrid", new PrintGridCommand(this.toolbarManager.Tools["PrintGrid"], this));
            this._commands.AddCommand("NotificationManager", new NotificationManagerCommand(this.toolbarManager.Tools["NotificationManager"]));

            this._commands.AddCommand("TermsManager", new TermsManagerCommand(this.toolbarManager.Tools["TermsManager"]));
            this._commands.AddCommand("PriceUnits", new PriceUnitManagerCommand(this.toolbarManager.Tools["PriceUnits"]));
            this._commands.AddCommand("PricePoints", new PricePointManagerCommand(this.toolbarManager.Tools["PricePoints"]));

            this._commands.AddCommand("InternalRework", new InternalReworkIdentifyCommand(this.toolbarManager.Tools["InternalRework"], this));
            this._commands.AddCommand("ReworkPlanning", new InternalReworkPlanCommand(this.toolbarManager.Tools["ReworkPlanning"], this));
            this._commands.AddCommand("JoinRework", new InternalReworkJoinCommand(this.toolbarManager.Tools["JoinRework"], this));

            this._commands.AddCommand("HoldLocation", new HoldLocationEditorCommand(this.toolbarManager.Tools["HoldLocation"]));
            this._commands.AddCommand("HoldReason", new HoldReasonEditorCommand(this.toolbarManager.Tools["HoldReason"]));
            this._commands.AddCommand("ReworkReason", new InternalReworkReasonEditorCommand(this.toolbarManager.Tools["ReworkReason"], ReworkCategoryType.Rework));
            this._commands.AddCommand("LostReasons", new InternalReworkReasonEditorCommand(this.toolbarManager.Tools["LostReasons"], ReworkCategoryType.Lost));
            this._commands.AddCommand("QuarantineReasons", new InternalReworkReasonEditorCommand(this.toolbarManager.Tools["QuarantineReasons"], ReworkCategoryType.Quarantine));

            this._commands.AddCommand("SplitReason", new SplitReasonEditorCommand(this.toolbarManager.Tools["SplitReason"]));
            this._commands.AddCommand("ExternalReworkReason", new ExternalReworkReasonEditorCommand(this.toolbarManager.Tools["ExternalReworkReason"]));
            this._commands.AddCommand("RejoinReason", new RejoinReasonEditorCommand(this.toolbarManager.Tools["RejoinReason"]));
            this._commands.AddCommand("ProcessCategories", new ProcessCategoryEditorCommand(this.toolbarManager.Tools["ProcessCategories"]));

            //CustomerPartVolumeReport

            this._commands.AddCommand("ControlInspections", new CustomReportCommand(this.toolbarManager.Tools["ControlInspections"], typeof(InspectionReport), "InspectionReport"));
            this._commands.AddCommand("CustomerVolume", new CustomerVolumeCommand(this.toolbarManager.Tools["CustomerVolume"]));
            this._commands.AddCommand("DepartmentVolume", new DepartmentVolumeCommand(this.toolbarManager.Tools["DepartmentVolume"]));
            this._commands.AddCommand("ProcessUsageSummary", new ProcessUsageReportCommand(this.toolbarManager.Tools["ProcessUsageSummary"]));
            this._commands.AddCommand("ProcessRevenue", new ProcessRevenueReportCommand(this.toolbarManager.Tools["ProcessRevenue"]));
            this._commands.AddCommand("ClosedOrders", new ClosedOrdersReportCommand(this.toolbarManager.Tools["ClosedOrders"]));
            this._commands.AddCommand("StatusReport", new StatusReportCommand(this.toolbarManager.Tools["StatusReport"]));
            this._commands.AddCommand("ProductionSales", new ProductionSalesReportCommand(this.toolbarManager.Tools["ProductionSales"]));
            this._commands.AddCommand("ShipRec", new CustomReportCommand(this.toolbarManager.Tools["ShipRec"], typeof(ShipRecReport), "ShipRecReport"));
            this._commands.AddCommand("OrderFees", new CustomReportCommand(this.toolbarManager.Tools["OrderFees"], typeof(FeeSummaryReport), "OrderFeesReport"));
            this._commands.AddCommand("OrderTurnOver", new OrderLeadTimeReportCommand(this.toolbarManager.Tools["OrderTurnOver"]));
            this._commands.AddCommand("ShippingReport", new ShippingDetailReportCommand(this.toolbarManager.Tools[" ShippingReport"]));
            this._commands.AddCommand("ScheduleCompletion", new CustomReportCommand(this.toolbarManager.Tools["ScheduleCompletion"], typeof(WorkScheduleActualsReport), "ScheduleCompletionReport"));
            this._commands.AddCommand("OrderCountByUserReport", new CustomReportCommand(this.toolbarManager.Tools["OrderCountByUserReport"], typeof(OrderCountByUserReport), "OrderCountByUserReport"));
            this._commands.AddCommand("OrdersByProcessReport", new OrdersByProcessReportCommand(this.toolbarManager.Tools["OrdersByProcessReport"]));
            this._commands.AddCommand("UnInvoicedOrders", new UnInvoicedReportCommand(this.toolbarManager.Tools["UnInvoicedOrders"]));
            this._commands.AddCommand("LateOrders", new LateOrdersReportCommand(this.toolbarManager.Tools["LateOrders"]));
            this._commands.AddCommand("ReworkOrders", new ExternalReworkOrdersReportCommand(this.toolbarManager.Tools["ReworkOrders"]));
            this._commands.AddCommand("btnInspectionManager", new QIManagerCommand(this.toolbarManager.Tools["btnInspectionManager"]));
            this._commands.AddCommand("SecurityRoles", new SecurityGroupManagerCommand(this.toolbarManager.Tools["SecurityRoles"]));
            this._commands.AddCommand("SmartCardManager", new SmartCardManagerCommand(this.toolbarManager.Tools["SmartCardManager"]));
            this._commands.AddCommand("AirframeManager", new AirframeManagerCommand(this.toolbarManager.Tools["AirframeManager"]));
            this._commands.AddCommand("Manufacturers", new ManufacturersCommand(this.toolbarManager.Tools["Manufacturers"]));
            this._commands.AddCommand("Materials", new MaterialsCommand(this.toolbarManager.Tools["Materials"]));
            this._commands.AddCommand("ResetOrderProcesses", new ResetOrderProcessesCommand(this.toolbarManager.Tools["ResetOrderProcesses"]));
            this._commands.AddCommand("ChangeWorkOrder", new ChangeWorkOrderCommand(this.toolbarManager.Tools["ChangeWorkOrder"]));
            this._commands.AddCommand("ExportToQuickbooks", new ExportInvoiceCommand(this.toolbarManager.Tools["ExportToQuickbooks"]));
            this._commands.AddCommand("Syspro", new SysproManagerCommand(this.toolbarManager.Tools["Syspro"]));
            this._commands.AddCommand("QuickBooksSync", new QuickBooksSyncWizardCommand(this.toolbarManager.Tools["QuickBooksSync"]));
            this._commands.AddCommand("FieldMigration", new FieldMigrationWizardCommand(this.toolbarManager.Tools["FieldMigration"]));
            this._commands.AddCommand("FeeManager", new OrderFeeManagerCommand(this.toolbarManager.Tools["FeeManager"]));
            this._commands.AddCommand("DepartmentManager", new DepartmentManagerCommand(this.toolbarManager.Tools["DepartmentManager"]));
            this._commands.AddCommand("ProcessingLineManager", new ProcessingLineManagerCommand(this.toolbarManager.Tools["ProcessingLineManager"]));
            this._commands.AddCommand("ShippingCarrierManager", new ShippingCarrierManagerCommand(this.toolbarManager.Tools["ShippingCarrierManager"]));
            this._commands.AddCommand("CountryManager", new CountryManagerCommand(this.toolbarManager.Tools["CountryManager"]));
            this._commands.AddCommand("ProcessingAnswersReport", new ProcessingAnswersReportCommand(this.toolbarManager.Tools["ProcessingAnswersReport"]));
            this._commands.AddCommand("TurnAroundTimeReport", new TurnAroundTimeReportCommand(this.toolbarManager.Tools["TurnAroundTimeReport"]));

            //this._commands.AddCommand("Feedback", new FeedbackCommand(this.toolbarManager.Tools["Feedback"]));
           // this._commands.AddCommand("KnowledgeBase", new KnowledgeBaseCommand(this.toolbarManager.Tools["KnowledgeBase"]));
           // this._commands.AddCommand("Announcements", new AnnouncementsCommand(this.toolbarManager.Tools["Announcements"]));
            this._commands.AddCommand("Tickets", new TicketCommand(this.toolbarManager.Tools["Tickets"]));
            this._commands.AddCommand("OpenOrderValues", new OpenOrderValuesReportCommand(this.toolbarManager.Tools["OpenOrderValues"]));
            this._commands.AddCommand("OrderStatusByCustomer", new OrderStatusByCustomerReportCommand(this.toolbarManager.Tools["OrderStatusByCustomer"]));
            this._commands.AddCommand("LostQuarantinedOrders", new LostAndQuarantinedOrdersReportCommand(this.toolbarManager.Tools["LostQuarantinedOrders"]));
            this._commands.AddCommand("OrdersOnHold", new OrdersOnHoldReportCommand(this.toolbarManager.Tools["OrdersOnHold"]));
            this._commands.AddCommand("InternalReworkOrders", new InternalReworkOrdersReportCommand(this.toolbarManager.Tools["InternalReworkOrders"]));
            this._commands.AddCommand("ProductionByDept", new ProductionByDepartmentReportCommand(this.toolbarManager.Tools["ProductionByDept"]));
            this._commands.AddCommand("WorkInProcess", new WorkInProcessReportCommand(this.toolbarManager.Tools["WorkInProcess"]));
            this._commands.AddCommand("RevenueByProgram", new RevenueByProgramReportCommand(this.toolbarManager.Tools["RevenueByProgram"]));
            this._commands.AddCommand("WorkInProcessHistory", new WIPHistoryReportCommand(this.toolbarManager.Tools["WorkInProcessHistory"]));

            this._commands.AddCommand("DocumentManager", new DocumentManagerCommand(this.toolbarManager.Tools["DocumentManager"]));

            this._commands.AddCommand("AddOrderSummary", new AddOrderSummaryCommand(this.toolbarManager.Tools["AddOrderSummary"]));

            this._commands.AddCommand("RenameTab", new RenameTabCommand(this.toolbarManager.Tools["RenameTab"], this));
            this._commands.AddCommand("DeleteTab", new DeleteTabCommand(this.toolbarManager.Tools["DeleteTab"], this));
            this._commands.AddCommand("ExportTab", new ExportTabCommand(this.toolbarManager.Tools["ExportTab"], this));
            this._commands.AddCommand("Presentation", new PresentationModeCommand(this.toolbarManager.Tools["Presentation"], this));

            this._commands.AddCommand("LoadDashboard", new DashboardLoadCommand(this.toolbarManager.Tools["LoadDashboard"], this));
            this._commands.AddCommand("SaveDashboard", new DashboardSaveCommand(this.toolbarManager.Tools["SaveDashboard"], this));
            this._commands.AddCommand("WidgetGallery", new DashboardAddWidgetCommand(this.toolbarManager.Tools["WidgetGallery"], this));

            this._commands.AddCommand("ProcessScheduleReport", new ProcessScheduleReportCommand(this.toolbarManager.Tools["ProcessScheduleReport"]));

            this._commands.AddCommand("OrderNotes", new OrderNoteCommand(this.toolbarManager.Tools["OrderNotes"], this));
            this._commands.AddCommand("OrderHold", new OrderHoldCommand(this.toolbarManager.Tools["OrderHold"], this));

            this._commands.AddCommand("DiscrepancyReport", new DiscrepancyReportCommand(this.toolbarManager.Tools["DiscrepancyReport"]));
            
            this._commands.AddCommand("OrderHoldsView", new AddHoldListTabCommand(this.toolbarManager.Tools["OrderHoldsView"], this._mainDockManager.dockManager));

            this._commands.AddCommand("LabelManager", new LabelManagerCommand(this.toolbarManager.Tools["LabelManager"]));
            this._commands.AddCommand("ReportManager", new ReportManagerCommand(this.toolbarManager.Tools["ReportManager"]));
            
            this._commands.AddCommand("AddBatchSummary", new AddBatchSummaryCommand(this.toolbarManager.Tools["AddBatchSummary"]));
            this._commands.AddCommand("AddBatch", new BatchAddCommand(this.toolbarManager.Tools["AddBatch"], this));
            this._commands.AddCommand("DeleteBatch", new BatchDeleteCommand(this.toolbarManager.Tools["DeleteBatch"], this));
            this._commands.AddCommand("EditBatch", new BatchEditCommand(this.toolbarManager.Tools["EditBatch"], this));
            this._commands.AddCommand("PrintBatchTraveler", new BatchPrintCommand(this.toolbarManager.Tools["PrintBatchTraveler"], this));
            this._commands.AddCommand("BatchCheckIn", new BatchCheckInCommand(this.toolbarManager.Tools["BatchCheckIn"], this));
            this._commands.AddCommand("BatchProcessing", new BatchProcessingCommand(this.toolbarManager.Tools["BatchProcessing"], this));
            this._commands.AddCommand("BatchInspection", new BatchInspectionCommand(this.toolbarManager.Tools["BatchInspection"], this));
            this._commands.AddCommand("BatchPartMarking", new BatchPartMarkingCommand(this.toolbarManager.Tools["BatchPartMarking"], this));
            this._commands.AddCommand("BatchCOC", new BatchCocCommand(this.toolbarManager.Tools["BatchCOC"], this));

            this._commands.AddCommand("StartBatchProcess", new StartBatchProcessTimerCommand(this.toolbarManager.Tools["StartBatchProcess"], this));
            this._commands.AddCommand("StopBatchProcess", new StopBatchProcessTimerCommand(this.toolbarManager.Tools["StopBatchProcess"], this));
            this._commands.AddCommand("StartBatchLabor", new StartBatchLaborTimerCommand(this.toolbarManager.Tools["StartBatchLabor"], this));
            this._commands.AddCommand("StopBatchLabor", new StopBatchLaborTimerCommand(this.toolbarManager.Tools["StopBatchLabor"], this));
            this._commands.AddCommand("PauseBatchLabor", new PauseBatchLaborTimerCommand(this.toolbarManager.Tools["PauseBatchLabor"], this));
            this._commands.AddCommand("TimeManager", new TimeManagerCommand(this.toolbarManager.Tools["TimeManager"]));

            this._commands.AddCommand("LogFile", new LogFileCommand(this.toolbarManager.Tools["LogFile"]));
            this._commands.AddCommand("BlanketPO", new BlanketPOCommand(this.toolbarManager.Tools["BlanketPO"], this));
            this._commands.AddCommand("AddPOOrder", new NewPOOrderCommand(this.toolbarManager.Tools["AddPOOrder"], this));
            this._commands.AddCommand("AddSalesOrder", new AddSalesOrderCommand(this.toolbarManager.Tools["AddSalesOrder"], this));
            this._commands.AddCommand("OrderContainers", new OrderContainersCommand(this.toolbarManager.Tools["OrderContainers"], this));
            this._commands.AddCommand("SplitOrder", new SplitOrderCommand(this.toolbarManager.Tools["SplitOrder"], this));
            this._commands.AddCommand("RejoinOrder", new RejoinOrderCommand(this.toolbarManager.Tools["RejoinOrder"], this));

            this._commands.AddCommand("SalesByESDReport", new SalesByESDReportCommand(this.toolbarManager.Tools["SalesByESDReport"]));
            this._commands.AddCommand("PartHistory", new PartHistoryReportCommand(this.toolbarManager.Tools["PartHistory"]));
            this._commands.AddCommand("BatchProduction", new BatchProductionReportCommand(this.toolbarManager.Tools["BatchProduction"]));
            this._commands.AddCommand("TimeTracking", new TimeTrackingReportCommand(this.toolbarManager.Tools["TimeTracking"]));
            this._commands.AddCommand("OrderCost", new OrderCostReportCommand(this.toolbarManager.Tools["OrderCost"]));
            this._commands.AddCommand("ProfitComparison", new ProfitComparisonReportCommand(this.toolbarManager.Tools["ProfitComparison"]));
            this._commands.AddCommand("RevenueByPart", new RevenueByPartReportCommand(this.toolbarManager.Tools["RevenueByPart"]));
            this._commands.AddCommand("DeliveryPerformance", new DeliveryPerformanceReportCommand(this.toolbarManager.Tools["DeliveryPerformance"]));
            this._commands.AddCommand("EmployeeReceiving", new EmployeeReceivingReportCommand(this.toolbarManager.Tools["EmployeeReceiving"]));
            this._commands.AddCommand("EmployeeProcessingPerformance", new EmployeeProcessingReportCommand(this.toolbarManager.Tools["EmployeeProcessingPerformance"]));
            this._commands.AddCommand("ShippedByPriority", new ShippedByPriorityReportCommand(this.toolbarManager.Tools["ShippedByPriority"]));
            this._commands.AddCommand("cmdUserEventsHistory", new UserEventHistoryCommand(this.toolbarManager.Tools["cmdUserEventsHistory"]));

            var pm = new Plugin.PluginManager();
            pm.LoadPlugins(this._commands, toolbarManager);
        }

        internal void RefreshData()
        {
            RefreshData(RefreshType.Order);
        }

        internal void RefreshData(RefreshType refreshType)
        {
            if (!EnableRefresh)
            {
                return;
            }

            if (InvokeRequired)
            {
                Action<RefreshType> action = RefreshData;
                Invoke(action, refreshType);
                return;
            }

            StopFadeIn();

            if (refreshType.HasFlag(RefreshType.Order))
            {
                _log.Info("Refreshing Data.");
                LoadData();
            }

            if (refreshType.HasFlag(RefreshType.Department))
            {
                var deptCmd = this._commands.FindCommand<DepartmentLocationCommand>() as DepartmentLocationCommand;

                deptCmd?.LoadDepartments();

                foreach (var tabData in _tabs)
                {
                    var tab = tabData.Tab as IDashboard;
                    tab?.OnDepartmentsChanged();
                }
            }

            if (refreshType.HasFlag(RefreshType.Line))
            {
                var lineCmd = _commands.FindCommand<ProcessingLineCommand>() as ProcessingLineCommand;
                lineCmd?.LoadProcessingLines(Settings.Default.CurrentDepartment);
            }

            if (refreshType.HasFlag(RefreshType.Settings))
            {
                this._commands.RefreshAll();
                this.UpdateRibbonVisibility();
                foreach (var scheduleTab in _tabs.Select(t => t.Tab).OfType<ISchedulingTab>())
                {
                    scheduleTab.RefreshSettings();
                }

                foreach (var orderSummaryTab in _tabs.Select(t => t.Tab).OfType<IOrderSummary>())
                {
                    orderSummaryTab.RefreshSettings();
                }

                _orderScanner.OnSettingsChange();
                _batchScanner.OnSettingsChange();
            }

            if (refreshType.HasFlag(RefreshType.WorkingDays))
            {
                _calendarPersistence.Refresh();
            }
        }

        private void UpdateUserName()
        {
            if(SecurityManager.Current.CurrentUser != null)
            {
                this.statusBar.Panels[0].Text = SecurityManager.Current.UserName;
                this.userProfileThumb.SetUser(SecurityManager.Current.CurrentUser);
            }
            else
            {
                this.statusBar.Panels[0].Text = "No User Logged In";
                this.userProfileThumb.ResetUser();
            }
        }

        private void UpdateRibbonVisibility()
        {
            try
            {
                //if no user then show everything except for labor (play/pause/stop)
                if(SecurityManager.Current.CurrentUser == null)
                {
                    foreach (var tab in this.toolbarManager.Ribbon.Tabs.OfType<RibbonTab>())
                    {
                        tab.Visible = true;
                        foreach (var group in tab.Groups.OfType<RibbonGroup>())
                        {
                            group.Visible = group.Key != "ribbonGroupLabor";
                        }
                    }
                }
                else
                {
                    //else update visibility to only show items with visible buttons.
                    foreach(RibbonTab tab in this.toolbarManager.Ribbon.Tabs)
                    {
                        var anyGroupVisible = false;

                        foreach(RibbonGroup group in tab.Groups)
                        {
                            var anyButtonVisible = group.Tools.Count > 0 && group.Tools.OfType<ToolBase>().Any(t => t.VisibleResolved);

                            //if there is a change then set new state
                            group.Visible = anyButtonVisible;

                            //if a group is visible then the tab should be visible
                            if(anyButtonVisible)
                                anyGroupVisible = true;
                        }

                        tab.Visible = anyGroupVisible;
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating ribbon visibility.");
            }
        }

        private void UpdateUserTabs()
        {
            try
            {
                var currentUser = SecurityManager.Current.CurrentUser;

                if (currentUser == null)
                {
                    return;
                }

                // Open user's tabs with their layouts.
                var tabs = currentUser.GetUser_SecurityGroupRows()
                    .SelectMany(s => s.SecurityGroupRow?.GetSecurityGroupTabRows())
                    .Where(s => s != null)
                    .ToList();

                if (tabs.Count > 0)
                {
                    _log.Info("Opening group-specific tabs for user.");

                    // Close all WIP screen tabs
                    var documents = _mainDockManager.dockManager
                        .GetPanes(PaneNavigationOrder.VisibleOrder)
                        .ToList(); // ToList() call is needed - won't close tabs otherwise

                    if (documents.Count > 0)
                    {
                        _log.Info("Clearing all open WIP screen tabs.");
                    }

                    foreach (var doc in documents)
                    {
                        doc.ExecuteCommand(ContentPaneCommands.Close);
                    }

                    var addedTabKeys = new HashSet<string>();

                    // Add tabs in reverse order so that they appear on the WIP screen in the correct order.
                    foreach (var tab in tabs.OrderByDescending(t => t.SecurityGroupID).ThenByDescending(t => t.TabOrder))
                    {
                        if (addedTabKeys.Contains(tab.TabKey))
                        {
                            _log.Info("Skipping duplicate tab {0}", tab.TabKey);
                            continue;
                        }

                        var dwosTab = DwosTabFactory.CreateTab(tab.DataType, tab.TabKey, tab.Name);
                        AddTab(dwosTab, false);

                        if (tab.IsLayoutNull() || string.IsNullOrWhiteSpace(tab.Layout))
                        {
                            _log.Info($"Loading system-specific layout for tab {dwosTab.TabName} ({dwosTab.Key})");
                            dwosTab.LoadLayout();
                        }
                        else
                        {
                            _log.Info($"Loading database layout for tab {dwosTab.TabName} ({dwosTab.Key})");
                            dwosTab.LoadLayout(tab.Layout);
                        }

                        addedTabKeys.Add(tab.TabKey);
                    }

                    if (_tabs.Count > 1)
                    {
                        // The last tab added is the first tab on the WIP screen.
                        _tabs.LastOrDefault()?.Pane?.Activate();
                    }

                    RefreshData();
                    _log.Info("Finished setting WIP screen tabs to group defaults.");
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating per-user tabs.");
            }
        }

        private bool BeginLoadingData(out string reason)
        {
            // lock (and Monitor) check which thread is requesting the lock.
            // During testing, the same thread called this method multiple times,
            // causing the lock be ineffective. The lock is emulated by using
            // a field and atomic access using Interlocked.Exchange.
            if (InvokeRequired || Interlocked.Exchange(ref _isRefreshing, 1) != 0)
            {
                reason = InvokeRequired ? "Invoke required." : "Currently refreshing";
                return false;
            }

            reason = null;
            timerRefresh.Stop();
            return true;
        }

        private void FinishLoadingData(bool reset = false)
        {
            _isRefreshing = 0;

            if(reset)
                this._retryPolicy.OnSuccess();

            if(this._retryPolicy.ShouldContinue)
            {
                this.timerRefresh.Interval = this._retryPolicy.RefreshTimeInMilliSeconds;
                this.timerRefresh.Start();
            }
        }

        internal void StopPresentationMode()
        {
            lock (this._timerNextTabLock)
            {
                this.timerNextTab.Stop();
            }

            try
            {
                var appearance = this.toolbarManager.Tools["Presentation"].SharedPropsInternal.AppearancesLarge.Appearance;
                appearance.Image = Properties.Resources.Play32;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Eror changing appearance of 'Presentation Mode' toolbar item.");
            }
        }

        internal void StartPresentationMode()
        {
            lock (this._timerNextTabLock)
            {
                this.timerNextTab.Interval = 1000 * UserSettings.Default.PresentationModeSpeed; // Interval is in ms
                this.timerNextTab.Start();
            }

            try
            {
                var appearance = this.toolbarManager.Tools["Presentation"].SharedPropsInternal.AppearancesLarge.Appearance;
                appearance.Image = Properties.Resources.Stop32;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Eror changing appearance of 'Presentation Mode' toolbar item.");
            }
        }

        public void AddTab(IDwosTab tab, bool activate = true)
        {
            try
            {
                if (tab == null)
                    return;

                tab.LayoutError += TabOnLayoutError;

                var wipData = _wipData ?? WipData.Empty();

                if (String.IsNullOrWhiteSpace(tab.TabName))
                    tab.TabName = "New Tab";

                var newContentPane = this._mainDockManager.dockManager.AddDocument(tab.TabName, tab.TabControl);
                newContentPane.TabHeaderTemplate = CreateTabHeader(tab.DataType, tab.TabName);

                newContentPane.CloseAction = PaneCloseAction.RemovePane;
                newContentPane.Closed += newContentPane_Closed;

                this._tabs.Add(new TabData { Pane = newContentPane, Tab = tab });

                tab.Initialize(wipData);
                if (tab is IOrderSummary)
                {
                    ((IOrderSummary)tab).AfterSelectedRowChanged += orderSummary_AfterSelectedRowChanged;
                }
                else if (tab is IBatchSummary)
                {
                    ((IBatchSummary)tab).AfterSelectedRowChanged += orderSummary_AfterSelectedRowChanged;
                }

                if (activate)
                {
                    newContentPane.Activate();
                }

                // RefreshData initializes layout-related data for some tab types.
                tab.RefreshData(wipData);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error adding tab.");
            }
        }

        private void RemoveTab(TabData tabData)
        {
            if(tabData != null)
            {

                var tab = tabData.Tab;
                tab.LayoutError -= TabOnLayoutError;
                this._tabs.Remove(tabData);

                if(tabData.Pane != null)
                {
                    tabData.Pane.ExecuteCommand(ContentPaneCommands.Close);
                    tabData.Pane.Closed -= newContentPane_Closed;
                }

                var orderSummaryTab = tab as IOrderSummary;
                if(orderSummaryTab != null)
                    orderSummaryTab.AfterSelectedRowChanged -= orderSummary_AfterSelectedRowChanged;

                var dashboardTab = tab as IDashboard;
                dashboardTab?.OnClose();

                var schedulingTab = tab as ISchedulingTab;
                schedulingTab?.OnClose();
            }

            //Select the next tab to ensure last tab is cleared
            var newTab = this._tabs.FirstOrDefault();

            if(newTab != null)
                newTab.Pane.Activate();
        }

        public void RenameTab(IDwosTab tab, string name)
        {
            var tabData = this._tabs.FirstOrDefault(t => t.Tab == tab);

            if(tabData != null)
            {
                tabData.Pane.Header = name;
                tab.TabName = name;
                tabData.Pane.TabHeaderTemplate = CreateTabHeader(tab.DataType, tab.TabName);
            }
        }

        public void CloseTab(IDwosTab tab)
        {
            var tabData = this._tabs.FirstOrDefault(t => t.Tab == tab);

            if(tabData != null)
                tabData.Pane.ExecuteCommand(ContentPaneCommands.Close);
        }

        /// <summary>
        /// Updates the selected WO for open <see cref="HoldList"/> to match
        /// the WO selected by the active order summary tab.
        /// </summary>
        public void UpdateHoldSelection()
        {
            var orderSummary = _activeTab as IOrderSummary;

            if (orderSummary == null)
            {
                return;
            }

            var holdLists = _mainDockManager.dockManager.Panes.OfType<SplitPane>()
                .SelectMany(i => i.Panes)
                .OfType<HoldContentPane>()
                .Select(pane => pane.HoldList);

            foreach (var holdList in holdLists)
            {
                holdList.SelectWO(orderSummary.SelectedWO);
            }
        }

        private DataTemplate CreateTabHeader(string tabType, string tabName)
        {
            FrameworkElementFactory headerImage = null;
            FrameworkElementFactory headerText = null;
            FrameworkElementFactory tabHeader = null;

            tabHeader = new FrameworkElementFactory(typeof(System.Windows.Controls.StackPanel));
            tabHeader.SetValue(System.Windows.Controls.StackPanel.OrientationProperty, System.Windows.Controls.Orientation.Horizontal);
            
            headerImage = new FrameworkElementFactory(typeof(System.Windows.Controls.Image));
            headerImage.SetValue(FrameworkElement.MarginProperty, new System.Windows.Thickness(0, 0, 3, 0));
            
            headerText = new FrameworkElementFactory(typeof(System.Windows.Controls.TextBlock));
            headerText.SetValue(FrameworkElement.MarginProperty, new System.Windows.Thickness(0, 0, 0, 0));

            if (tabType == OrderSummary2.DATA_TYPE)
                headerImage.SetValue(System.Windows.Controls.Image.SourceProperty, new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/DWOS.UI;component/Resources/images/Order_16.png", UriKind.Absolute)));
            else if (tabType == DashboardTab.DATA_TYPE)
                headerImage.SetValue(System.Windows.Controls.Image.SourceProperty, new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/DWOS.UI;component/Resources/images/Dashboard-16.png", UriKind.Absolute)));
            else if (tabType == BatchSummary.DATA_TYPE)
                headerImage.SetValue(System.Windows.Controls.Image.SourceProperty, new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/DWOS.UI;component/Resources/images/Batch16.png", UriKind.Absolute)));
            else if (tabType == SchedulingTab.DATA_TYPE)
                headerImage.SetValue(System.Windows.Controls.Image.SourceProperty, new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/DWOS.UI;component/Resources/images/Calendar_16.png", UriKind.Absolute)));

            headerText.SetValue(System.Windows.Controls.TextBlock.TextProperty, tabName);

            tabHeader.AppendChild(headerImage);
            tabHeader.AppendChild(headerText);
            
            return new DataTemplate { VisualTree = tabHeader };
        }

        private void RestoreLayouts()
        {
            try
            {
                //load saved tabs
                if (UserSettings.Default.TabData != null && UserSettings.Default.TabData.Tabs != null)
                {
                    foreach (var tabInfo in UserSettings.Default.TabData.Tabs)
                    {
                        var tab = DwosTabFactory.CreateTab(tabInfo.DataType, tabInfo.Key, tabInfo.Name);
                        AddTab(tab);
                        tab.LoadLayout();
                    }
                }

                //Add a default OrderSummary tab if there are none
                if(!this._tabs.Any(t => t.Tab is IOrderSummary))
                    AddTab(DwosTabFactory.CreateTab(OrderSummary2.DATA_TYPE, Guid.NewGuid().ToString(), "Orders"));

                RefreshData();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error restoring tabs.");
            }
        }

        private void SaveLayouts()
        {
            try
            {
                UserSettings.Default.TabData = new TabInfoCollection() { Tabs = new List<TabInfo>()};
                
                if(ActiveTab != null)
                    UserSettings.Default.LastSelectedTab = ActiveTab.Key;

                _tabs.ForEach(tabData =>
                                   {
                                       var tab = tabData.Tab;
                                       tab.SaveLayout();
                                       UserSettings.Default.TabData.Tabs.Add(new TabInfo() { Key = tab.Key, Name = tab.TabName, DataType = tab.DataType});
                                   });
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error saving tabs.");
            }
        }

        /// <summary>
        /// Adjust processing-related fields for orders before showing them.
        /// </summary>
        /// <param name="orderStatus"></param>
        public static void AdjustOrderData(OrderStatusDataSet.OrderStatusDataTable orderStatus)
        {
            const string nextDeptNone = "None";

            foreach (var or in orderStatus)
            {
                //use hack to determine if this is the last processed date
                if (!or.IsCurrentProcessDueNull() && or.CurrentProcessDue < LAST_PROCESS_DATE_FLAG)
                {
                    //get back to original last process date
                    or.CurrentProcessDue = or.CurrentProcessDue.AddYears(100);
                }

                if (or.WorkStatus == WORK_STATUS_INPROCESS)
                {
                    if (!or.IsNextDeptNull() && or.NextDept == nextDeptNone)
                    {
                        or.NextDept = PostProcessingDepartment(or);
                    }
                }
                else if (or.WorkStatus == WORK_STATUS_PARTMARKING)
                {
                    or.CurrentProcess = WORK_STATUS_PARTMARKING;
                    or.NextDept = WORK_STATUS_FINALINSPECTION;

                    //use hack to determine if this is the last processes date
                    if (!or.IsCurrentProcessDueNull())
                    {
                        //add processing time for part marking
                        or.CurrentProcessDue = or.CurrentProcessDue.AddBusinessDays(Convert.ToInt32(Math.Ceiling(ApplicationSettings.Current.PartMarkingLeadTime)));
                    }
                }
                else if (or.WorkStatus == WORK_STATUS_FINALINSPECTION)
                {
                    or.CurrentProcess = WORK_STATUS_FINALINSPECTION;
                    or.NextDept = WORK_STATUS_SHIPPING;

                    //use hack to determine if this is the last processes date
                    if (!or.IsCurrentProcessDueNull())
                    {
                        // Does not take part marking into consideration

                        //add processing time for COC
                        or.CurrentProcessDue = or.CurrentProcessDue.AddBusinessDays(Convert.ToInt32(Math.Ceiling(ApplicationSettings.Current.COCLeadTime)));
                    }
                }
                else if (or.WorkStatus == WORK_STATUS_PENDINGINSPECTION)
                {
                    or.CurrentProcess = "Control Inspection";

                    if (!or.IsNextDeptNull() && or.NextDept == nextDeptNone)
                    {
                        or.NextDept = PostProcessingDepartment(or);
                    }
                }
                else if (or.WorkStatus == WORK_STATUS_SHIPPING)
                {
                    or.CurrentProcess = WORK_STATUS_SHIPPING;
                    or.NextDept = nextDeptNone;

                    //use hack to determine if this is the last processes date
                    if (!or.IsCurrentProcessDueNull())
                    {
                        // Does not take part marking into consideration

                        //add processing time for COC
                        if (ApplicationSettings.Current.COCEnabled)
                            or.CurrentProcessDue = or.CurrentProcessDue.AddBusinessDays(Convert.ToInt32(Math.Ceiling(ApplicationSettings.Current.COCLeadTime)));

                        or.CurrentProcessDue = or.CurrentProcessDue.AddBusinessDays(Convert.ToInt32(Math.Ceiling(ApplicationSettings.Current.ShippingLeadTime)));
                    }
                }
                else if (or.WorkStatus == WORK_STATUS_PENDINGREWORKPLANNING)
                {
                    or.CurrentProcess = "Rework Planning";
                    or.NextDept = "TBD";
                }
                else if (or.WorkStatus == WORK_STATUS_PENDINGOR)
                    or.CurrentProcess = "Order Review";
                else if (or.WorkStatus == WORK_STATUS_PENDING_IMPORT_EXPORT_REVIEW)
                    or.CurrentProcess = "Import/Export Review";
                else if (or.WorkStatus == WORK_STATUS_PENDINGJOIN)
                    or.CurrentProcess = WORK_STATUS_PENDINGJOIN;
                else if (or.WorkStatus == WORK_STATUS_HOLD)
                    or.CurrentProcess = "On Hold";

                if (or.RowState == System.Data.DataRowState.Modified)
                {
                    or.EndEdit();
                    or.AcceptChanges();
                }
            }
        }

        private static string PostProcessingDepartment(OrderStatusDataSet.OrderStatusRow orderStatus)
        {
            if (!orderStatus.IsHasPartMarkNull() && orderStatus.HasPartMark)
            {
                return ApplicationSettings.Current.DepartmentPartMarking;
            }

            if (ApplicationSettings.Current.COCEnabled)
            {
                return WORK_STATUS_FINALINSPECTION;
            }

            return ApplicationSettings.Current.DepartmentShipping;
        }

        private void ShowSQLEditor()
        {
            using (var txt = new TextBoxForm())
            {
                txt.FormTextBox.PasswordChar = '*';
                txt.FormLabel.Text = "Password:";
                txt.Text = "Admin Password";

                if (txt.ShowDialog(Form.ActiveForm) == DialogResult.OK && txt.FormTextBox.Text == "P@ssword!")
                {                    
                    var frm = new Utilities.SqlEditorWindow();
                    var helper = new WindowInteropHelper(frm) { Owner = DWOSApp.MainForm.Handle };

                    frm.ShowDialog();
                }
                else
                    MessageBoxUtilities.ShowMessageBoxWarn("Invalid password!", "Password", "Must have valid password to use this feature. Contact support for more information.");
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.F9 | Keys.Control))
            {
                ShowSQLEditor();
                return true;    // indicate that you handled this keystroke
            }

            if (keyData == Keys.F12)
            {
                _orderScanner.ShowScanPrompt();
                _batchScanner.ShowScanPrompt();
                return true;
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void StopFadeIn()
        {
            Opacity = 1;
            this.startUpTimer.Enabled = false;
        }

        /// <summary>
        /// Checks disk space for the drive containing the user's profile.
        /// </summary>
        private void CheckForLowDiskSpace()
        {
            if (_showedLowDiskSpaceError)
            {
                return;
            }

            const int minimumDiskSpaceBytes = 200000000;
            const int bytesPerMegabyte = 1000000;

            var documentsDrive = new DriveInfo(
                Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));

            if (documentsDrive.AvailableFreeSpace < minimumDiskSpaceBytes)
            {
                var spaceRemainingMegabytes = documentsDrive.AvailableFreeSpace / bytesPerMegabyte;
                FlyoutManager.DisplayFlyout("Low Disk Space",
                    $"You have {spaceRemainingMegabytes} MB remaining on your main hard drive.",
                    true);

                _showedLowDiskSpaceError = true;
            }
        }

#endregion

#region Events

        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                ReportNotifier.ReportCreated += OnReportCreated;
                CheckForLowDiskSpace();
            }
            catch(Exception exc)
            {
                const string errorMsg = "Error loading main form.";
                _log.Error(exc, errorMsg);
            }
        }

        private static void OnReportCreated(object sender, ReportCreatedEventArgs args)
        {
            try
            {
                // Save Work Order Print History
                if (!ApplicationSettings.Current.SaveWorkOrderPrintHistory || args == null)
                {
                    return;
                }

                OrderHistoryDataSet.UpdateOrderHistory(
                    args.OrderId,
                    "Print",
                    $"Printed {args.Title}.",
                    args.UserName);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating Order History to include report.");
            }
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            try
            {
                _log.Info("On Main form shown.");

                this.startUpTimer.Enabled = true;

                //Load data after UI has loaded
                ThreadPool.QueueUserWorkItem(cb =>
                                             {
                                                 Thread.Sleep(1000);
                                                 BeginInvoke(new Action(RefreshData));
                                             });

                //Select last selected tab when closed
                if(!String.IsNullOrWhiteSpace(UserSettings.Default.LastSelectedTab))
                {
                    var selectedTab = _tabs.FirstOrDefault(t => t.Tab.Key == UserSettings.Default.LastSelectedTab);
                    if(selectedTab != null)
                    {
                        ActiveTab = selectedTab.Tab;
                        selectedTab.Pane.Activate();
                    }
                }
                
                //Select first tab
                if(this.ActiveTab == null)
                {
                    var tab = this._tabs.FirstOrDefault();
                    if(tab != null)
                        tab.Pane.Activate();
                }

                _batchScanner = new BatchSummaryBarcodeScanner(this);
                _batchScanner.Scanner.BarcodingStarted += ScannerOnBarcodingStarted;
                _batchScanner.Scanner.BarcodingFinished += ScannerOnBarcodingFinished;
                _batchScanner.Scanner.BarcodingCancelled += ScannerOnBarcodingCancelled;

                _orderScanner = new OrderSummaryBarcodeScanner(this);
                _orderScanner.Scanner.BarcodingStarted += ScannerOnBarcodingStarted;
                _orderScanner.Scanner.BarcodingFinished += ScannerOnBarcodingFinished;
                _orderScanner.Scanner.BarcodingCancelled += ScannerOnBarcodingCancelled;

                _log.Info("Application started successfully.");
            }
            catch(SqlException excSql)
            {
                ErrorMessageBox.ShowDialog("Error connecting to database.", excSql);
                this.toolbarManager.Enabled = false;
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error loading the application.", exc);
                this.toolbarManager.Enabled = false;
            }
            finally
            {
                SplashScreen.Stop();
            }
        }

        private void ScannerOnBarcodingStarted(object sender, EventArgs eventArgs)
        {
            // Prevent a barcode's input from being entered on the WIP screen.
            try
            {
                _log.Info("Barcode scan started");
                _mainDockManager.AllowTextInput = false;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling 'barcode started' event.");
            }
        }

        private void ScannerOnBarcodingFinished(object sender, BarcodeScanner.BarcodeScannerEventArgs barcodeScannerEventArgs)
        {
            // Re-enable text entry on the WIP screen after scanning a barcode.
            try
            {
                _log.Info("Barcode scan finished");
                _mainDockManager.AllowTextInput = true;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling 'barcode finished' event.");
            }
        }

        private void ScannerOnBarcodingCancelled(object sender, EventArgs eventArgs)
        {

            // Re-enable text entry on the WIP screen after scanner times out
            try
            {
                _log.Info("Barcode scan cancelled");
                _mainDockManager.AllowTextInput = true;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling 'barcode cancelled' event.");
            }
        }

        private void SecurityManager_OnUserChange(object sender, UserUpdatedEventArgs args)
        {
            if(InvokeRequired)
                BeginInvoke(new EventHandler<UserUpdatedEventArgs>(SecurityManager_OnUserChange), sender, args);
            else
            {
                UpdateUserName();
                UpdateRibbonVisibility();

                if (args.UserIdChanged)
                {
                    UpdateUserTabs();
                }
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                this._isClosing = true;

                timerRefresh.Stop();

                SaveLayouts();

                //save settings if they have been loaded in case user clicked on the close button without closing the File Menu
                this.userSettings1.SaveSettings();

                //stop timer and save settings
                SecurityManager.Current.UserUpdated -= SecurityManager_OnUserChange;
                Settings.Default.Save();
                UserSettings.Default.Save();

                UserLogging.AddAnalytics(null); //FLUSH Analytics

                //trigger WPF to shutdown also
                if(System.Windows.Application.Current != null)
                    System.Windows.Application.Current.Shutdown();

                // May be unable to show message box errors when closing application.
                ErrorMessageBox.PreventUIInteraction = true;
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error closing down main form.");
            }
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            try
            {
                CheckForLowDiskSpace();
                RefreshData();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error on timer refresh tick.");
            }
        }

        private void toolbarManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "Setttings")
            {
                userSettings1.LoadSettings();
            }
            else if (e.Tool.Key == "DeptLocation")
            {
                // Hide tooltip so that it does not cover the department combobox
                // This might be the only way to hide a toolbar manager's tooltip, so
                // the AfterToolCloseup handler has to re-enable it.
                toolbarManager.ToolTipDisplayStyle = ToolTipDisplayStyle.None;
            }
        }

        private void toolbarManager_AfterToolCloseup(object sender, ToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "Setttings")
            {
                userSettings1.SaveSettings();
            }
            else if (e.Tool.Key == "DeptLocation")
            {
                toolbarManager.ToolTipDisplayStyle = ToolTipDisplayStyle.Formatted;
            }
        }

        private void orderSummary_AfterSelectedRowChanged(object sender, EventArgs e)
        {
            SelectedGridRowChanged?.Invoke(sender, e);
            UpdateHoldSelection();
        }

        private void dockManager_ActivePaneChanged(object sender, RoutedPropertyChangedEventArgs <ContentPane> e) { ActiveTab = e.NewValue == null ? null : e.NewValue.Content as IDwosTab; }

        private void newContentPane_Closed(object sender, PaneClosedEventArgs e)
        {
            try
            {
                var tabData = this._tabs.FirstOrDefault(td => ReferenceEquals(td.Pane, sender));

                if(tabData != null)
                    RemoveTab(tabData);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error during pane close.");
            }
        }

        private void startUpTimer_Tick(object sender, EventArgs e)
        {
            Opacity += .1;

            if (Opacity >= 1)
            {
                StopFadeIn();
            }
        }

        private void timerNextTab_Tick(object sender, EventArgs e)
        {
            try
            {
                ContentPane paneToActive = null;
                lock (_timerNextTabLock)
                {
                    this.timerNextTab.Stop();

                    bool selectNext = false;

                    foreach (var pane in this._mainDockManager.dockManager.GetPanes(PaneNavigationOrder.VisibleOrder))
                    {
                        if (pane.IsActiveDocument)
                        {
                            selectNext = true;
                        }
                        else if (selectNext)
                        {
                            selectNext = false;
                            paneToActive = pane;
                            break;
                        }
                    }

                    if (selectNext)
                    {
                        // Currently selected tab is last - select the first one
                        paneToActive = this._mainDockManager.dockManager.GetPanes(PaneNavigationOrder.VisibleOrder).First();
                    }

                    this.timerNextTab.Start();
                }

                if (paneToActive != null)
                {
                    paneToActive.Activate();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error encountered while changing active pane in Presentation Mode");
            }
        }

        private void DepartmentLocationCommandOnDepartmentChanged(object sender, EventArgs eventArgs)
        {
            try
            {              
                // Refresh commands
                Commands.RefreshAll();

                // Load processing lines for current department
                var lineCmd = _commands.FindCommand<ProcessingLineCommand>() as ProcessingLineCommand;
                lineCmd?.LoadProcessingLines(Settings.Default.CurrentDepartment);

                //Notify UI
                RefreshData();

            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error after changing departments.");
            }
        }

        private void TabOnLayoutError(object sender, EventArgs args)
        {
            try
            {
                var tab = sender as IDwosTab;
                FlyoutManager.DisplayFlyout(
                    "WIP Screen",
                    $"Encountered an error while trying to show {tab?.TabName ?? "WIP Screen Tab"}.",
                    true);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error showing WIP screen tab layout error.");
            }
        }

#endregion

#region TabData

        private class TabData
        {
            public ContentPane Pane { get; set; }
            public IDwosTab Tab { get; set; }
        }

        #endregion
    }
}