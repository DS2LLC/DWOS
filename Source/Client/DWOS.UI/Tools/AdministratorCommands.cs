using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Interop;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.QBExport.Syncing;
using DWOS.Shared;
using DWOS.UI.Admin;
using DWOS.UI.Admin.FieldMigration;
using DWOS.UI.Admin.OrderReset;
using DWOS.UI.Admin.RevisePartProcess;
using DWOS.UI.Admin.Schedule;
using DWOS.UI.QA;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinStatusBar;
using Infragistics.Win.UltraWinToolbars;
using DWOS.UI.Admin.ChangeWorkOrder;

namespace DWOS.UI.Tools
{
    internal class LogInCommand: CommandBase
    {
        #region Fields

        private readonly Form _mainForm;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get { return true; }
        }

        #endregion

        #region Methods

        public LogInCommand(ToolBase tool, Form mainForm)
            : base(tool)
        {
            this._mainForm = mainForm;
            this._mainForm.Shown += this.mainForm_Shown;
            SecurityManager.Current.UserUpdated += this.OnUserUpdated;

            //if using smartcard then no need to show UI
            tool.SharedProps.Visible = ApplicationSettings.Current.LoginType != LoginType.Smartcard;
        }

        public override void OnClick()
        {
            try
            {
                _log.Info("LogIn OnClick.");

                if(SecurityManager.Current.IsValidUser)
                    SecurityManager.Current.LogOut();
                else
                    SecurityManager.Current.Login();

                base.Button.Caption = SecurityManager.Current.IsValidUser ? "Logout" : "Login";
            }
            catch(Exception exc)
            {
                _log.Info(exc, "Error during login command.");
            }
        }

        private void mainForm_Shown(object sender, EventArgs e)
        {
            this.OnClick();
        }


        private void OnUserUpdated(object sender, UserUpdatedEventArgs args)
        {
            base.Button.Caption = SecurityManager.Current.IsValidUser ? "Logout" : "Login";
        }


        #endregion
    }

    internal class PinAuthenticationCommand : CommandBase
    {
        #region Fields

        private ToolBase _parentTool;
        private ToolBase _loginButton;
        private UltraStatusBar _statusBar;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get { return true; }
        }

        #endregion

        #region Methods

        public PinAuthenticationCommand(ToolBase tool, ToolBase parent, ToolBase loginButton, UltraStatusBar statusBar)
            : base(tool)
        {
            this._parentTool = parent;
            this._loginButton = loginButton;
            this._statusBar = statusBar;

            if (Properties.Settings.Default.UserLogInType == LoginType.Pin)
            {
                this._parentTool.SharedPropsInternal.AppearancesLarge.Appearance.Image = Properties.Resources.Asterik;
                this._loginButton.SharedProps.Visible = true;
            }
        }

        public override void OnClick()
        {
            try
            {
                _log.Info("Pin authentication OnClick.");

                this._parentTool.SharedPropsInternal.AppearancesLarge.Appearance.Image = Properties.Resources.Asterik;
                this._loginButton.SharedProps.Visible = true;
                
                if (SecurityManager.Current.IsValidUser)
                    SecurityManager.Current.LogOut();
                
                Properties.Settings.Default.UserLogInType = LoginType.Pin;
                SecurityManager.Current.Login();
                _statusBar.Panels[0].Appearance.Image = SecurityManager.Current.AuthenticationProviderThumbnail;
                
                if(SecurityManager.Current.IsValidUser)
                    this._loginButton.SharedProps.Caption = "Logout";
            }
            catch (Exception exc)
            {
                _log.Info(exc, "Error during login command.");
            }
        }

        #endregion
    }

    internal class SmartcardAuthenticationCommand : CommandBase
    {
        #region Fields

        private  ToolBase _loginButton;
        private ToolBase _parentTool;
        private UltraStatusBar _statusBar;
        
        #endregion

        #region Properties

        public override bool Enabled
        {
            get { return true; }
        }

        #endregion

        #region Methods

        public SmartcardAuthenticationCommand(ToolBase tool, ToolBase parent, ToolBase loginButton, UltraStatusBar statusBar)
            : base(tool)
        {
            this._loginButton = loginButton;
            this._parentTool = parent;
            this._statusBar = statusBar;

            if (Properties.Settings.Default.UserLogInType == LoginType.Smartcard)
            {
                this._parentTool.SharedPropsInternal.AppearancesLarge.Appearance.Image = Properties.Resources.SmartCard;
                this._loginButton.SharedProps.Visible = false;
            }
        }

        public override void OnClick()
        {
            try
            {
                _log.Info("Smartcard Authentication OnClick.");

                this._parentTool.SharedPropsInternal.AppearancesLarge.Appearance.Image = Properties.Resources.SmartCard;
                this._loginButton.SharedProps.Visible = false;

                if (SecurityManager.Current.IsValidUser)
                    SecurityManager.Current.LogOut();

                Properties.Settings.Default.UserLogInType = LoginType.Smartcard;
                SecurityManager.Current.Login();
                _statusBar.Panels[0].Appearance.Image = SecurityManager.Current.AuthenticationProviderThumbnail;
            }
            catch (Exception exc)
            {
                _log.Info(exc, "Error during login command.");
            }
        }

        #endregion
    }

    internal class OrderScheduleCommand: CommandBase
    {
        #region Properties

        public override bool Enabled
        {
            //get { return base.Enabled && ApplicationSettings.Current.SchedulingEnabled && ApplicationSettings.Current.SchedulerType == SchedulerType.ProductionCapacity; }
            get { return base.Enabled && ApplicationSettings.Current.SchedulingEnabled && (ApplicationSettings.Current.SchedulerType == SchedulerType.ProductionCapacity || ApplicationSettings.Current.SchedulerType == SchedulerType.ProcessLeadTime); }

        }

        #endregion

        #region Methods

        public OrderScheduleCommand(ToolBase tool)
            : base(tool, "OrderSchedule")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using (var frm = new ProcessingSchedule())
                {
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error displaying order summary.");
            }
        }


        public override bool Refresh()
        {
            var refreshSuccessful = base.Refresh();

            Button.Visible = Button.Visible
                && !ApplicationSettings.Current.UsingManualScheduling;

            return refreshSuccessful;
        }


        #endregion
    }
    
    internal class HolidayManagerCommand : CommandBase
    {
        #region Properties

        public override bool Enabled
        {
            get { return base.Enabled && ApplicationSettings.Current.SchedulingEnabled; }
        }

        #endregion

        #region Methods

        public HolidayManagerCommand(ToolBase tool)
            : base(tool, "HolidayManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using (var frm = new HolidayManager())
                {
                    if (frm.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                    {
                        DWOSApp.MainForm.RefreshData(RefreshType.WorkingDays);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error displaying holiday manager.");
            }
        }

        #endregion
    }

    internal class WorkWeekManagerCommand : CommandBase
    {
        #region Properties

        public override bool Enabled =>
            base.Enabled && ApplicationSettings.Current.SchedulingEnabled;

        #endregion

        #region Methods

        public WorkWeekManagerCommand(ToolBase tool)
            : base(tool, "HolidayManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                var dialog = new WorkWeekManager();
                dialog.LoadData();
                var helper = new WindowInteropHelper(dialog) { Owner = DWOSApp.MainForm.Handle };

                if (dialog.ShowDialog() ?? false)
                {
                    DWOSApp.MainForm.RefreshData(RefreshType.WorkingDays);
                }

                GC.KeepAlive(helper);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error displaying workweek manager.");
            }
        }

        #endregion
    }

    internal class OrderPriorityCommand: CommandBase
    {
        #region Properties

        public int SelecteOrderID { get; set; }

        public override bool Enabled => base.Enabled
            && ApplicationSettings.Current.SchedulingEnabled
            && !ApplicationSettings.Current.UsingManualScheduling;

        #endregion

        #region Methods

        public OrderPriorityCommand(ToolBase tool)
            : base(tool, "OrderPriority")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using(var frm = new OrderPriority())
                {
                    frm.SelecteOrderID = this.SelecteOrderID;
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error displaying order summary.";
                _log.Error(exc, errorMsg);
            }
        }

        #endregion
    }

    internal class DashboardCommand: CommandBase
    {
        #region Methods

        public DashboardCommand(ToolBase tool)
            : base(tool, "Dashboard")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                DWOSApp.MainForm.AddTab(new Dashboard.DashboardTab());
            }
            catch(Exception exc)
            {
                string errorMsg = "Error displaying order summary.";
                _log.Error(exc, errorMsg);
            }
        }

        #endregion
    }

    internal class SettingsCommand: CommandBase
    {
        #region Methods

        public SettingsCommand(ToolBase tool)
            : base(tool, "Settings")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            using (new MainRefreshHelper(DWOSApp.MainForm, RefreshType.Settings))
            {
                using (var frm = new Settings())
                {
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
        }

        #endregion
    }

    internal class SecurityGroupManagerCommand: CommandBase
    {
        #region Methods

        public SecurityGroupManagerCommand(ToolBase tool)
            : base(tool, "SecurityGroupManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            bool savedChanges;
            using(var frm = new SecurityGroupManager())
            {
                frm.ShowDialog(Form.ActiveForm);
                savedChanges = frm.SavedChanges;
            }

            if (savedChanges)
            {
                SecurityManager.Current.ReLoadUserSecurityRoles();
                Documents.DocumentManagerSecurity.Current.ReloadSecurityRoles();
            }
        }

        #endregion
    }

    internal class SmartCardManagerCommand: CommandBase
    {
        #region Methods

        public SmartCardManagerCommand(ToolBase tool)
            : base(tool, "SmartCardManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            var currentUserID = 0;

            if(Enabled)
            {
                try
                {
                    currentUserID = SecurityManager.Current.CurrentUser.UserID;
                    SecurityManager.Current.SuspendAuthentication();

                    using(var frm = new SmartCardEditor())
                    {
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
                finally
                {
                    SecurityManager.Current.ResumeAuthentication();

                    if (currentUserID != 0)
                        SecurityManager.Current.DoLogin(currentUserID);
                    else
                        SecurityManager.Current.Login();
                }
            }
        }

        #endregion
    }

    internal class QIManagerCommand: CommandBase
    {
        #region Methods

        public QIManagerCommand(ToolBase tool)
            : base(tool, "QIManager.Edit")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                using(var frm = new QIManager())
                {
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
        }

        #endregion
    }

    internal class OrderFeeManagerCommand: CommandBase
    {
        #region Methods

        public OrderFeeManagerCommand(ToolBase tool)
            : base(tool, "OrderFeesManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                using(var frm = new OrderFeeManager())
                {
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
        }

        #endregion
    }

    internal class PriceUnitManagerCommand: CommandBase
    {
        #region Methods

        public PriceUnitManagerCommand(ToolBase tool)
            : base(tool, "PriceUnitManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                using(var frm = new PriceUnitManager())
                {
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
        }

        #endregion
    }
    internal class PricePointManagerCommand: CommandBase
    {
        #region Methods

        public PricePointManagerCommand(ToolBase tool)
            : base(tool, "PriceUnitManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            var window = new Sales.PricePointDialog();
            var helper = new WindowInteropHelper(window) { Owner = DWOSApp.MainForm.Handle };
            window.ShowDialog();
        }

        #endregion
    }

    internal class DepartmentManagerCommand: CommandBase
    {
        #region Fields

        private bool _userAcceptedDialog = false;

        #endregion

        #region Methods

        public DepartmentManagerCommand(ToolBase tool)
            : base(tool, "DepartmentManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                using (new MainRefreshHelper(DWOSApp.MainForm, RefreshType.Department | RefreshType.Order))
                {
                    using (var frm = new DepartmentManager())
                    {
                        var result = frm.ShowDialog(Form.ActiveForm);

                        _userAcceptedDialog = result == DialogResult.OK;
                    }
                }
            }
        }

        #endregion
    }

    internal class ShippingCarrierManagerCommand: CommandBase
    {
        #region Methods

        public ShippingCarrierManagerCommand(ToolBase tool)
            : base(tool, "ShippingCarrierManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                using (var frm = new DomainValueEditor() { Text = "Shipping Carrier Editor" })
                {
                    using (var ta = new Data.Datasets.OrderShipmentDataSetTableAdapters.d_ShippingCarrierTableAdapter())
                    {
                        var dt = new Data.Datasets.OrderShipmentDataSet.d_ShippingCarrierDataTable();
                        ta.Fill(dt);

                        frm.MaxNameLength = dt.CarrierIDColumn.MaxLength;

                        frm.AddValue = () =>
                        {
                            var random = new Random();
                            var newRow = dt.Addd_ShippingCarrierRow("New Shipping Carrier " + random.Next(1000, 9999));
                            return new DomainValueEditor.DomainValue() { Name = newRow.CarrierID, AllowDelete = true, AllowEdit = true, Row = newRow };
                        };
                        
                        frm.NameChanged = (dv) =>
                        {
                            ((Data.Datasets.OrderShipmentDataSet.d_ShippingCarrierRow)dv.Row).CarrierID = dv.Name.Replace("'", "*"); 
                        };

                        dt.ForEach(row =>
                                   {
                                       var usage = ta.GetUsageCount(row.CarrierID);
                                       frm.AddDomainValue(new DomainValueEditor.DomainValue() { Name = row.CarrierID, AllowDelete = usage == null || Convert.ToInt32(usage) < 1, AllowEdit = true, Row = row });
                                   });

                        if (frm.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                            ta.Update(dt);
                    }
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Command that shows <see cref="CountryManager"/>.
    /// </summary>
    internal class CountryManagerCommand : CommandBase
    {
        #region Methods

        public CountryManagerCommand(ToolBase tool)
            : base(tool, "Settings")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            var dialog = new CountryManager();
            var helper = new WindowInteropHelper(dialog) { Owner = DWOSApp.MainForm.Handle };

            dialog.ShowDialog();
        }

        #endregion
    }

    internal class ProcessingLineManagerCommand: CommandBase
    {
        #region Properties

        public override bool Enabled => base.Enabled && ApplicationSettings.Current.MultipleLinesEnabled;

        #endregion

        #region Methods

        public ProcessingLineManagerCommand(ToolBase tool)
            : base(tool, "DepartmentManager")
        {
            HideIfDisabled = true;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            using (new MainRefreshHelper(DWOSApp.MainForm, RefreshType.Line | RefreshType.Order))
            {
                using (var frm = new ProcessingLineManager())
                {
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
        }

        #endregion
    }

    internal class ShipmentPackageTypeManagerCommand : CommandBase
    {
        #region Methods

        public ShipmentPackageTypeManagerCommand(ToolBase tool)
            : base(tool, "ShipmentContainerTypeManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            using (var frm = new ShipmentPackageTypeManager())
            {
                frm.ShowDialog(Form.ActiveForm);
            }
        }

        #endregion
    }

    internal class ResetOrderProcessesCommand : CommandBase
    {
        #region Methods

        public ResetOrderProcessesCommand(ToolBase tool)
            : base(tool, "ResetOrder")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using (new MainRefreshHelper(DWOSApp.MainForm))
                {
                    using (var frm = new OrderResetMain())
                    {
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error during reset order.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }

    /// <summary>
    /// Command for the "Change Locked Work Order" tool.
    /// </summary>
    internal class ChangeWorkOrderCommand : CommandBase
    {
        #region Methods

        public ChangeWorkOrderCommand(ToolBase tool)
            : base(tool, "ResetOrder")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using (new MainRefreshHelper(DWOSApp.MainForm))
                {
                    using (var frm = new ChangeWorkOrderMain())
                    {
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
            }
            catch (Exception exc)
            {
                var errorMsg = "Unhandled error while using Change Work Order tool.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }

    internal class ManufacturersCommand: CommandBase
    {
        #region Methods

        public ManufacturersCommand(ToolBase tool)
            : base(tool, "ManufacturerManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using(var frm = new ManufacturerManager())
                {
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error update manufacturing values.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }

    internal class TermsManagerCommand: CommandBase
    {
        #region Methods

        public TermsManagerCommand(ToolBase tool)
            : base(tool, "TermsManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using(var frm = new TermsManager())
                {
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error update quote process values.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }

    internal class MaterialsCommand: CommandBase
    {
        #region Methods

        public MaterialsCommand(ToolBase tool)
            : base(tool, "MaterialManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using(var frm = new MaterialManager())
                {
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error update manufacturing values.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }

    internal class AirframeManagerCommand: CommandBase
    {
        #region Methods

        public AirframeManagerCommand(ToolBase tool)
            : base(tool, "AirframeManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using(var frm = new AirframeManager())
                {
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error update AirframeManagerCommand values.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }

    internal class UserManagerCommand : CommandBase
    {
        #region Fields

        private Main _frmMain;

        #endregion

        #region Methods

        public UserManagerCommand(ToolBase tool, Main frmMain)
            : base(tool, "UserManager")
        {
            _frmMain = frmMain;
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                using(var frm = new UserManager())
                {
                    frm.ShowDialog(Form.ActiveForm);

                    //force refresh of tools because of security role changes
                    _frmMain.Commands.RefreshAll();
                }
            }
        }

        #endregion
    }

    internal class ProcessesCommand : CommandBase
    {
        #region Methods

        public ProcessesCommand(ToolBase tool)
            : base(tool, "ProcessManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                // ProcessManager can add departments
                using (new MainRefreshHelper(DWOSApp.MainForm, RefreshType.Department))
                {
                    using (var p = new ProcessManager())
                    {
                        p.ShowDialog(Form.ActiveForm);
                    }
                }
            }
        }

        #endregion
    }

    internal class ProcessPackagesCommand : CommandBase
    {
        #region Methods

        public ProcessPackagesCommand(ToolBase tool)
            : base(tool, "ProcessPackagesManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                using(var p = new ProcessPackages())
                {
                    p.ShowDialog(Form.ActiveForm);
                }
            }
        }

        #endregion
    }

    internal class ListEditorCommand : CommandBase
    {
        #region Methods

        public ListEditorCommand(ToolBase tool)
            : base(tool, "ListManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                using(var le = new ListEditor())
                {
                    le.ShowDialog(Form.ActiveForm);
                }
            }
        }

        #endregion
    }

    internal class ProductClassEditorCommand : CommandBase
    {
        #region Fields

        private bool _productClassVisible;

        #endregion

        #region Properties

        public override bool Enabled =>
            _productClassVisible && base.Enabled;

        #endregion

        #region Methods

        public ProductClassEditorCommand(ToolBase tool)
            : base(tool, "ListManager")
        {
            HideIfDisabled = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            if (ApplicationSettings.Current.ProductClassEditorType == ProductClassType.Textbox)
            {
                MessageBoxUtilities.ShowMessageBoxWarn(
                    "You will not be able to use these product classes in Order Entry unless you are using the 'combobox' editor for product class.",
                    "Product Class Manager");
            }

            var dialog = new ProductClassEditor();
            var helper = new WindowInteropHelper(dialog)
            {
                Owner = DWOSApp.MainForm.Handle
            };

            dialog.ShowDialog();
        }

        public override bool Refresh()
        {
            using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
            {
                using (var fields = ta.GetByCategory("Order"))
                {
                    var productClassField = fields.FirstOrDefault(f => f.Name == "Product Class");
                    _productClassVisible = productClassField == null || productClassField.IsVisible || productClassField.IsRequired;
                }
            }

            return base.Refresh();
        }

        #endregion
    }

    internal class OrderApprovalTermsEditorCommand : CommandBase
    {
        #region Properties

        public override bool Enabled =>
            ApplicationSettings.Current.OrderApprovalEnabled && base.Enabled;

        #endregion

        #region Methods

        public OrderApprovalTermsEditorCommand(ToolBase tool)
            : base(tool, "ListManager")
        {
            HideIfDisabled = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            var dialog = new OrderApprovalTermsEditor();
            var helper = new WindowInteropHelper(dialog)
            {
                Owner = DWOSApp.MainForm.Handle
            };

            dialog.ShowDialog();
        }

        #endregion
    }

    internal class WorkDescriptionEditorCommand : CommandBase
    {
        #region Properties

        public override bool Enabled => ApplicationSettings.Current.RepairStatementEnabled
            && base.Enabled;

        #endregion

        #region Methods

        public WorkDescriptionEditorCommand(ToolBase tool)
            : base(tool, "ListManager")
        {
            HideIfDisabled = Properties.Settings.Default.AutoHideDisabledButtons;

        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            var dialog = new WorkDescriptionEditor();
            var helper = new WindowInteropHelper(dialog)
            {
                Owner = DWOSApp.MainForm.Handle
            };

            dialog.ShowDialog();
        }

        #endregion
    }

    internal class ExportInvoiceCommand: CommandBase
    {
        #region Methods

        //For now, security level for exporting invoices is based on Quickbooks authorization
        public ExportInvoiceCommand(ToolBase tool)
            : base(tool, "ExportToQuickbooks")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                using (var le = new ExportInvoicesDialog())
                {
                    le.ShowDialog(Form.ActiveForm);
                }
            }
        }

        #endregion
    }

    internal class SysproManagerCommand: CommandBase
    {
        #region Methods

        public override bool Enabled => base.Enabled && ApplicationSettings.Current.SysproIntegrationEnabled;

        public SysproManagerCommand(ToolBase tool)
            : base(tool, "ExportToQuickbooks")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
            HideIfDisabled = true;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            using (new MainRefreshHelper(DWOSApp.MainForm))
            {
                var manager = new Admin.Syspro.SysproInvoiceManager();
                var helper = new WindowInteropHelper(manager)
                {
                    Owner = DWOSApp.MainForm.Handle
                };

                manager.ShowDialog();
                GC.KeepAlive(helper);
            }
        }

        #endregion
    }

    internal class TimeManagerCommand : CommandBase
    {
        #region Properties

        public override bool Enabled
        {
            get
            {
                if (SecurityManager.Current == null)
                {
                    return false;
                }

                return SecurityManager.Current.IsInRole("OrderProcessing") ||
                    SecurityManager.Current.IsInRole("ControlInspection") ||
                    SecurityManager.Current.IsInRole("TimeTrackingManager");
            }
        }

        #endregion

        #region Methods

        public TimeManagerCommand(ToolBase tool)
            : base(tool)
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            using (new MainRefreshHelper(DWOSApp.MainForm))
            {
                var laborManager = new Admin.Time.TimeManager();

                if (SecurityManager.Current.IsInRole("TimeTrackingManager"))
                {
                    laborManager.LoadAdminData();
                }
                else
                {
                    laborManager.LoadData(SecurityManager.Current.UserID);
                }

                var helper = new WindowInteropHelper(laborManager)
                {
                    Owner = DWOSApp.MainForm.Handle
                };

                laborManager.ShowDialog();
                GC.KeepAlive(helper);
            }
        }

        public override bool Refresh()
        {
            bool baseRefresh = base.Refresh();
            bool usingTimers = ApplicationSettings.Current.TimeTrackingEnabled;
            Button.Visible = usingTimers;

            return baseRefresh && usingTimers;
        }

        #endregion
    }

    internal class HoldLocationEditorCommand : CommandBase
    {
        #region Fields

        private Dictionary<Data.Datasets.ListsDataSet.d_HoldLocationRow, int> _rowUsages = new Dictionary<Data.Datasets.ListsDataSet.d_HoldLocationRow, int>();

        #endregion

        #region Methods

        public HoldLocationEditorCommand(ToolBase tool)
            : base(tool, "ListManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using (var frm = new DomainValueEditor() { Text = "Hold Location Editor"})
                {
                    using(var ta = new Data.Datasets.ListsDataSetTableAdapters.d_HoldLocationTableAdapter())
                    {
                        var dt = new Data.Datasets.ListsDataSet.d_HoldLocationDataTable();
                        ta.Fill(dt);

                        frm.AddValue = () =>
                                       {
                                           var random = new Random();
                                           var newRow = dt.Addd_HoldLocationRow("New Location " + random.Next(1000, 9999));
                                           return new DomainValueEditor.DomainValue() { Name = newRow.HoldLocation, AllowDelete = true, AllowEdit = true, Row = newRow };
                                       };
                        frm.NameChanged = (dv) =>
                                          {
                                              ((Data.Datasets.ListsDataSet.d_HoldLocationRow)dv.Row).HoldLocation = dv.Name.Replace("'", "*"); 
                                          };
                        frm.DeleteValue = () =>
                                        {
                                            //Check to see if list item is being used
                                            var rowToDelete = frm.SelectedValue.Row as Data.Datasets.ListsDataSet.d_HoldLocationRow;

                                            if (rowToDelete != null)
                                            {
                                                using (var taOrderHoldLocation = new Data.Datasets.OrdersDataSetTableAdapters.d_HoldLocationTableAdapter())
                                                {
                                                    if (!_rowUsages.ContainsKey(rowToDelete))
                                                        _rowUsages.Add(rowToDelete, taOrderHoldLocation.GetUsageCount(rowToDelete.HoldLocation).GetValueOrDefault());

                                                    if (_rowUsages[rowToDelete] > 0)
                                                    {
                                                        MessageBoxUtilities.ShowMessageBoxWarn("Unable to delete hold locations that are in use.", "Unable to Delete");
                                                        return false;
                                                    }
                                                }
                                            }

                                            return true;
                                        };

                        dt.ForEach(hold => frm.AddDomainValue(new DomainValueEditor.DomainValue() { Name = hold.HoldLocation, AllowDelete = true, AllowEdit = true, Row = hold }));

                        if(frm.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                            ta.Update(dt);
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error editing hold location changes.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }

    internal class HoldReasonEditorCommand : CommandBase
    {
        #region Fields

        private Dictionary<Data.Datasets.ListsDataSet.d_HoldReasonRow, int> _rowUsages = new Dictionary<Data.Datasets.ListsDataSet.d_HoldReasonRow, int>();

        #endregion

        #region Methods

        public HoldReasonEditorCommand(ToolBase tool)
            : base(tool, "ListManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public HoldReasonEditorCommand(EditorButton button)
            : base(button, "ListManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using (var frm = new DomainValueEditor() { Text = "Hold Reason Editor" })
                {
                    using (var ta = new Data.Datasets.ListsDataSetTableAdapters.d_HoldReasonTableAdapter())
                    {
                        var dt = new Data.Datasets.ListsDataSet.d_HoldReasonDataTable();
                        ta.Fill(dt);

                        frm.AddValue = () =>
                        {
                            var random = new Random();
                            var newRow = dt.Addd_HoldReasonRow("New Hold Reason " + random.Next(1000, 9999));
                            return new DomainValueEditor.DomainValue() { Name = newRow.Name, AllowDelete = true, AllowEdit = true, Row = newRow };
                        };
                        frm.NameChanged = (dv) =>
                        {
                            ((Data.Datasets.ListsDataSet.d_HoldReasonRow)dv.Row).Name = dv.Name.Replace("'", "*"); 
                        };
                        frm.DeleteValue = () =>
                        {
                            //Check to see if list item is being used
                            var rowToDelete = frm.SelectedValue.Row as Data.Datasets.ListsDataSet.d_HoldReasonRow;

                            if (rowToDelete != null)
                            {
                                using (var taOrderHoldReason = new Data.Datasets.OrdersDataSetTableAdapters.d_HoldReasonTableAdapter())
                                {
                                    if (!_rowUsages.ContainsKey(rowToDelete))
                                        _rowUsages.Add(rowToDelete, taOrderHoldReason.GetUsageCount(rowToDelete.HoldReasonID).GetValueOrDefault());

                                    if (_rowUsages[rowToDelete] > 0)
                                    {
                                        MessageBoxUtilities.ShowMessageBoxWarn("Unable to delete hold reasons that are in use.", "Unable to Delete");
                                        return false;
                                    }
                                }
                            }

                            return true;
                        };

                        dt.ForEach(hold => frm.AddDomainValue(new DomainValueEditor.DomainValue() { Name = hold.Name, AllowDelete = true, AllowEdit = true, Row = hold }));

                        if (frm.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                            ta.Update(dt);
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error editing hold location changes.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }

    internal class InternalReworkReasonEditorCommand : CommandBase
    {
        #region Properties

        public ReworkCategoryType Category { get; set; }

        #endregion

        #region Methods

        public InternalReworkReasonEditorCommand(ToolBase tool, ReworkCategoryType category)
            : base(tool, "ListManager")
        {
            this.Category = category;
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public InternalReworkReasonEditorCommand(EditorButton button, ReworkCategoryType category)
            : base(button, "ListManager")
        {
            this.Category = category;
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                var reasonDialog = new InternalReworkReasonsEditor();
                var helper = new WindowInteropHelper(reasonDialog) { Owner = DWOSApp.MainForm.Handle };
                reasonDialog.LoadReworkReasons(Category);
                reasonDialog.ShowDialog();

                GC.KeepAlive(helper);

            }
            catch (Exception exc)
            {
                string errorMsg = "Error editing rework reason changes.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }
    
    internal class SplitReasonEditorCommand : CommandBase
    {
        #region Methods

        public SplitReasonEditorCommand(ToolBase tool)
            : base(tool, "ListManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using (var frm = new DomainValueEditor() { Text = "Split Reason Editor" })
                {
                    using (var ta = new Data.Datasets.ListsDataSetTableAdapters.d_OrderChangeReasonTableAdapter())
                    {
                        var dt = new Data.Datasets.ListsDataSet.d_OrderChangeReasonDataTable();
                        ta.FillByChangeType(dt, (int)OrderChangeType.Split);

                        frm.AddValue = () =>
                        {
                            var random = new Random();
                            var newRow = dt.Addd_OrderChangeReasonRow((int)OrderChangeType.Split, "New Split Reason " + random.Next(1000, 9999));
                            return new DomainValueEditor.DomainValue() { Name = newRow.Name, AllowDelete = true, AllowEdit = true, Row = newRow };
                        };
                        frm.NameChanged = (dv) =>
                        {
                            ((Data.Datasets.ListsDataSet.d_OrderChangeReasonRow)dv.Row).Name = dv.Name.Replace("'", "*"); 
                        };

                        dt.ForEach(hold => frm.AddDomainValue(new DomainValueEditor.DomainValue() { Name = hold.Name, AllowDelete = true, AllowEdit = true, Row = hold }));

                        if (frm.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                            ta.Update(dt);
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error editing rework reason changes.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }

    internal class RejoinReasonEditorCommand : CommandBase
    {
        #region Methods

        public RejoinReasonEditorCommand(ToolBase tool)
            : base(tool, "ListManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using (var frm = new DomainValueEditor() { Text = "Rejoin Reason Editor" })
                {
                    using (var ta = new Data.Datasets.ListsDataSetTableAdapters.d_OrderChangeReasonTableAdapter())
                    {
                        var dt = new ListsDataSet.d_OrderChangeReasonDataTable();
                        ta.FillByChangeType(dt, (int)OrderChangeType.Rejoin);

                        frm.AddValue = () =>
                        {
                            var random = new Random();
                            var newRow = dt.Addd_OrderChangeReasonRow((int)OrderChangeType.Rejoin, "New Rejoin Reason " + random.Next(1000, 9999));
                            return new DomainValueEditor.DomainValue() { Name = newRow.Name, AllowDelete = true, AllowEdit = true, Row = newRow };
                        };
                        frm.NameChanged = (dv) =>
                        {
                            ((ListsDataSet.d_OrderChangeReasonRow)dv.Row).Name = dv.Name.Replace("'", "*"); 
                        };

                        foreach (var hold in dt)
                        {
                            frm.AddDomainValue(new DomainValueEditor.DomainValue() { Name = hold.Name, AllowDelete = true, AllowEdit = true, Row = hold });
                        }

                        if (frm.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                        {
                            ta.Update(dt);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error editing rejoin reason changes.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }

    internal class ExternalReworkReasonEditorCommand : CommandBase
    {
        #region Methods

        public ExternalReworkReasonEditorCommand(ToolBase tool)
            : base(tool, "ListManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using (var frm = new DomainValueEditor() { Text = "External Rework Reason Editor" })
                {
                    using (var ta = new Data.Datasets.ListsDataSetTableAdapters.d_OrderChangeReasonTableAdapter())
                    {
                        var dt = new Data.Datasets.ListsDataSet.d_OrderChangeReasonDataTable();
                        ta.FillByChangeType(dt, (int)OrderChangeType.ExtRework);

                        frm.AddValue = () =>
                        {
                            var random = new Random();
                            var newRow = dt.Addd_OrderChangeReasonRow((int)OrderChangeType.ExtRework, "New Reason" + random.Next(1000, 9999));
                            return new DomainValueEditor.DomainValue() { Name = newRow.Name, AllowDelete = true, AllowEdit = true, Row = newRow };
                        };
                        frm.NameChanged = (dv) =>
                        {
                            ((Data.Datasets.ListsDataSet.d_OrderChangeReasonRow)dv.Row).Name = dv.Name.Replace("'", "*"); 
                        };

                        dt.ForEach(hold => frm.AddDomainValue(new DomainValueEditor.DomainValue() { Name = hold.Name, AllowDelete = true, AllowEdit = true, Row = hold }));

                        if (frm.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                            ta.Update(dt);
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error editing rework reason changes.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }
    
    internal class ProcessCategoryEditorCommand : CommandBase
    {
        #region Methods

        public ProcessCategoryEditorCommand(ToolBase tool)
            : base(tool, "ListManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using(var settings = new Admin.Schedule.ProcessLeadTimeSettings())
                    settings.ShowDialog();

                //using (var frm = new DomainValueEditor() { Text = "Process Category Editor" })
                //{
                //    using (var ta = new Data.Datasets.ProcessesDatasetTableAdapters.d_ProcessCategoryTableAdapter())
                //    {
                //        var dt = new Data.Datasets.ProcessesDataset.d_ProcessCategoryDataTable();
                //        ta.Fill(dt);

                //        frm.AddValue = () =>
                //        {
                //            var random = new Random();
                //            var newRow = dt.Addd_ProcessCategoryRow("New Category " + random.Next(1000, 9999));
                //            return new DomainValueEditor.DomainValue() { Name = newRow.ProcessCategory, AllowDelete = true, AllowEdit = true, Row = newRow };
                //        };
                //        frm.NameChanged = (dv) =>
                //        {
                //            ((Data.Datasets.ProcessesDataset.d_ProcessCategoryRow)dv.Row).ProcessCategory = dv.Name.Replace("'", "*");
                //        };

                //        dt.ForEach(hold => frm.AddDomainValue(new DomainValueEditor.DomainValue() { Name = hold.ProcessCategory, AllowDelete = false, AllowEdit = true, Row = hold }));

                //        if (frm.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                //            ta.Update(dt);
                //    }
                //}
            }
            catch (Exception exc)
            {
                string errorMsg = "Error editing process category changes.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }

    internal class QuickBooksSyncWizardCommand : CommandBase
    {
        #region Methods

        public QuickBooksSyncWizardCommand(ToolBase tool)
            : base(tool, "ExportToQuickbooks")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using (var frm = QBSyncController.CreateWizard())
                {
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error during QuickBooks synchronization.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }

    internal class FieldMigrationWizardCommand : CommandBase
    {
        #region Methods

        public FieldMigrationWizardCommand(ToolBase tool)
            : base(tool, "FieldMigration")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using (var controller = new FieldMigrationController())
                {
                    using (var frm = controller.NewDialog())
                    {
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error during custom field migration.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }

    internal class RevisePartProcessWizardCommand : CommandBase
    {
        #region Methods

        public RevisePartProcessWizardCommand(ToolBase tool)
            : base(tool, "ProcessManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                using (var controller = new RevisePartProcessController())
                {
                    using (var frm = controller.NewDialog())
                    {
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
            }
            catch(Exception exc)
            {
                var errorMsg = "Error during part process revision.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }

    internal class LabelManagerCommand : CommandBase
    {
        #region Methods

        public LabelManagerCommand(ToolBase tool)
            : base(tool, "Settings")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            using (var frm = new LabelManager())
            {
                frm.ShowDialog(Form.ActiveForm);
            }
        }

        #endregion
    }

    internal class ReportManagerCommand : CommandBase
    {
        #region Methods

        public ReportManagerCommand(ToolBase tool)
            : base(tool, "Settings")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            using (var frm = new ReportManager())
            {
                frm.ShowDialog(Form.ActiveForm);
            }
        }

        #endregion
    }

    internal class UserEventHistoryCommand : CommandBase
    {
        #region Methods

        public UserEventHistoryCommand(ToolBase tool)
            : base(tool)
        {
        }

        public override void OnClick()
        {
            try
            {
                var frm = new Utilities.UserEventLogHistory();
                var helper = new WindowInteropHelper(frm) { Owner = DWOSApp.MainForm.Handle };

                frm.LoadData();
                frm.ShowDialog();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading user event history.");
            }
        }

     
        #endregion
    }

    /// <summary>
    /// Command that shows <see cref="ReportScheduleManager"/>.
    /// </summary>
    internal class NotificationManagerCommand: CommandBase
    {
        #region Methods

        public NotificationManagerCommand(ToolBase tool)
            : base(tool, "Settings")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            var dialog = new ReportScheduleManager();
            var helper = new WindowInteropHelper(dialog) { Owner = DWOSApp.MainForm.Handle };

            dialog.ShowDialog();
        }

        #endregion
    }


}