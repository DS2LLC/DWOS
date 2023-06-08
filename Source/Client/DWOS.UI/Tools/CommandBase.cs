using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DWOS.Shared;
using DWOS.UI.Utilities;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using NLog;

namespace DWOS.UI.Tools
{
    internal abstract class CommandBase: ICommandBase
    {
        #region Fields

        protected static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private List<CommandBase> _commandsToRefresh;
        private string _toolName;
        public event EventHandler AfterClick;

        #endregion

        #region Properties

        public bool HideIfUnAuthorized { get; set; }

        public bool HideIfDisabled { get; set; }

        protected string SecurityRole { get; set; }

        protected IButtonAdapter Button { get; private set; }
        
        public virtual bool Enabled
        {
            get { return this.IsAuthorized(); }
        }

        /// <summary>
        ///   Gets or sets the key mapping. If this key is pressed then this method will be fired.
        /// </summary>
        /// <value> The key mapping. </value>
        public Keys KeyMapping { get; set; }

        /// <summary>
        /// Gets or sets the name of the tool. This is used for identifying the tool in the analytics. Overrideable in case multiple instances of a generic tool.
        /// </summary>
        /// <value>The name of the tool.</value>
        protected string ToolName 
        {
            get { return this._toolName ?? (this._toolName = this.GetType().Name); }
            set { _toolName = value; }
        }

        #endregion

        #region Methods

        protected CommandBase(IButtonAdapter tool)
        {
            this.Button = tool;

            if(this.Button != null)
                this.Button.OnButtonClick += this.Button_OnButtonClick;

            if(SecurityManager.Current != null)
                SecurityManager.Current.UserUpdated += this.SecurityManager_OnUserUpdated;
        }

        protected CommandBase(ToolBase tool, string securityRole = null)
            : this(new ToolBaseButtonAdapter(tool))
        {
            this.SecurityRole = securityRole;

            if(!String.IsNullOrWhiteSpace(this.SecurityRole) && tool != null)
            {
                //ensure tooltip has a title
                if(tool.SharedProps.ToolTipTitle == null)
                    tool.SharedProps.ToolTipTitle = tool.SharedProps.Caption;

                //update the tool tip to inlcude the security permission name
                if (!String.IsNullOrWhiteSpace(tool.SharedProps.ToolTipTextFormatted))
                    tool.SharedProps.ToolTipTextFormatted += "<br/><br/>Permission: [" + this.SecurityRole + "]";
                else if(tool.SharedProps.ToolTipText != null)
                    tool.SharedProps.ToolTipText += Environment.NewLine + Environment.NewLine + "Permission: [" + this.SecurityRole + "]";
                else
                    tool.SharedProps.ToolTipText = "Permission: [" + this.SecurityRole + "]";
            }
        }

        protected CommandBase(UltraButton tool, string securityRole = null)
            : this(new UltraButtonAdapter(tool))
        {
            this.SecurityRole = securityRole;
        }

        protected CommandBase(EditorButton button, string securityRole = null)
            : this(new EditorButtonAdapter(button))
        {
            this.SecurityRole = securityRole;
        }

        public virtual void OnClick() {}

        /// <summary>
        ///   Refresh the tools enabled property and return enabled state.
        /// </summary>
        public virtual bool Refresh()
        {
            try
            {
                bool enabled = this.Enabled;

                if(this.Button != null)
                {
                    this.Button.Enabled = enabled;

                    if(this.HideIfUnAuthorized)
                    {
                        //hide button if user logged in and not authorized, else show it anyways
                        this.Button.Visible = SecurityManager.Current.CurrentUser == null || this.IsAuthorized();
                    }

                    if (HideIfDisabled)
                    {
                        Button.Visible = Enabled;
                    }
                }

                return enabled;
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error refreshing command.");
                return false;
            }
        }

        /// <summary>
        ///   Adds the related command to be refreshed after this commands on click is fired. For Example, cut copy paste are related.
        /// </summary>
        /// <param name="command"> The command to refresh. </param>
        public void AddRelatedCommandToRefresh(CommandBase command)
        {
            if(this._commandsToRefresh == null)
                this._commandsToRefresh = new List<CommandBase>();

            this._commandsToRefresh.Add(command);
        }

        /// <summary>
        ///   Determines whether this instance is authorized to execute.
        /// </summary>
        /// <returns> <c>true</c> if this instance is authorized; otherwise, <c>false</c> . </returns>
        protected bool IsAuthorized()
        {
            if(!string.IsNullOrWhiteSpace(this.SecurityRole) && SecurityManager.Current != null)
                return SecurityManager.Current.IsInRole(this.SecurityRole);

            return true;
        }

        /// <summary>
        ///   Called when [after on click].
        /// </summary>
        protected void OnAfterOnClick()
        {
            if(AfterClick != null)
                AfterClick(this, EventArgs.Empty);
        }

        private void UpdateUsageAnalytics()
        {
            DWOS.Data.Datasets.UserLogging.AddAnalytics(this.ToolName);
        }

        /// <summary>
        /// Performs the on click to fully execute all click related events.
        /// </summary>
        public void PerformOnClick()
        {
            Button_OnButtonClick(this, EventArgs.Empty);
        }

        #endregion

        #region Events

        private void SecurityManager_OnUserUpdated(object sender, UserUpdatedEventArgs args)
        {
            this.Refresh();
        }

        private void Button_OnButtonClick(object sender, EventArgs e)
        {
            try
            {
                if(this.Button == null || !this.Button.IsUpdating)
                {
                    _log.Debug("Performing " + GetType().FullName + ".OnClick()");

                    this.OnClick();
                    this.OnAfterOnClick();
                    UpdateUsageAnalytics();

                    if(this._commandsToRefresh != null)
                    {
                        foreach(var cmd in this._commandsToRefresh)
                            cmd.Refresh();
                    }
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error during tool execution.", exc);
            }
        }

        #endregion

        #region ICommandBase Members

        public virtual void Dispose()
        {
            if(this.Button != null)
                this.Button.OnButtonClick -= this.Button_OnButtonClick;

            this.Button = null;

            if(this._commandsToRefresh != null)
                this._commandsToRefresh.Clear();

            this._commandsToRefresh = null;

            SecurityManager.Current.UserUpdated -= this.SecurityManager_OnUserUpdated;
        }

        #endregion
    }

    internal abstract class TreeNodeCommandBase: CommandBase
    {
        #region Fields

        protected UltraTreeNode _node;
        private UltraTree _tree;

        #endregion

        #region Properties

        protected UltraTree TreeView
        {
            get { return this._tree; }
            set
            {
                this._tree = value;

                if(this._tree != null)
                    this._tree.AfterSelect += this.tvw_AfterSelect;
            }
        }

        protected bool AllowMultipleSelection { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///   Constructor
        /// </summary>
        protected TreeNodeCommandBase(ToolBase tool, string securityRole = null)
            : base(tool, securityRole) {}

        protected TreeNodeCommandBase(UltraButton tool, string securityRole = null)
            : base(tool, securityRole) {}

        /// <summary>
        ///   Called when after select event occurs.
        /// </summary>
        /// <param name="selectedNode"> The selected node. </param>
        public virtual void OnAfterSelect(UltraTreeNode selectedNode)
        {
            try
            {
                //set current node
                this._node = selectedNode;

                if(!base.Refresh())
                    this._node = null;
            }
            catch(Exception exc)
            {
                _log.Error(exc);
            }
        }

        public override bool Refresh()
        {
            if(this.TreeView != null)
                this.OnAfterSelect(this.TreeView.SelectedNode<UltraTreeNode>());

            return Enabled;
        }

        public override void Dispose()
        {
            _log.Debug("Disposing command: " + GetType().Name);

            if(this._tree != null)
                this._tree.AfterSelect -= this.tvw_AfterSelect;

            this._node = null;
            this._tree = null;

            base.Dispose();
        }

        #endregion

        #region Events

        /// <summary>
        ///   Fired when tvw item is selected to determine ensure tool checks to see if enabled
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        protected void tvw_AfterSelect(object sender, SelectEventArgs e)
        {
            if(this.AllowMultipleSelection)
                this.OnAfterSelect(e.NewSelections.Count > 0 ? e.NewSelections[0] : null);
            else
                this.OnAfterSelect(e.NewSelections.Count == 1 ? e.NewSelections[0] : null);
        }

        #endregion
    }

    internal abstract class GridCommand: CommandBase
    {
        #region Fields

        protected internal Main _frmMain;

        #endregion

        #region Methods

        protected GridCommand(ToolBase tool, Main frmMain, string securityRole = null)
            : base(tool, securityRole)
        {
            this._frmMain = frmMain;
            frmMain.SelectedGridRowChanged += this.frmMain_SelectedGridRowChanged;
        }

        public override void Dispose()
        {
            if(this._frmMain != null)
                this._frmMain.SelectedGridRowChanged -= this.frmMain_SelectedGridRowChanged;

            this._frmMain = null;

            base.Dispose();
        }

        #endregion

        #region Events

        protected virtual void frmMain_SelectedGridRowChanged(object sender, EventArgs e)
        {
            base.Refresh();
        }

        #endregion
    }
}