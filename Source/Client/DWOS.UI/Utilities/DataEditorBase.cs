using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Utilities.Validation;
using DWOS.Shared;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinTree;
using NLog;

namespace DWOS.UI
{
    /// <summary>
    ///     Base form used as a data editor.
    /// </summary>
    public partial class DataEditorBase : Form, IDataEditor
    {
        #region Fields

        private CommandManager _cmdManager = new CommandManager();
        private bool _isDisposing;
        private bool _disposed;

        protected bool _loadingData;
        protected Logger _log; // The logger for this class, do not make it static so everyone who inherits uses the same one.
        private List <IDataPanel> _panels = new List <IDataPanel>();
        private bool _panelsInitialized;
        private EventHandler<UserUpdatedEventArgs> _userUpdated;
        private System.Drawing.Size _defaultAutoScrollMin;
        protected ValidatorManager _validators = new ValidatorManager();

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the command manager which holds all commands. Should use to ensure that all commands get disposed of.
        /// </summary>
        /// <value> The commands. </value>
        internal CommandManager Commands
        {
            get { return this._cmdManager; }
        }

        /// <summary>
        ///     Gets the data panels managed by this editor.
        /// </summary>
        /// <value> The data panels. </value>
        public List <IDataPanel> DataPanels
        {
            get { return this._panels; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to suspend validation.
        /// </summary>
        /// <value> <c>true</c> if [suspend validation]; otherwise, <c>false</c> . </value>
        public bool SuspendValidation { get; set; }

        /// <summary>
        ///     Gets the active panel.
        /// </summary>
        /// <value> The active panel. </value>
        public IDataPanel ActivePanel
        {
            get { return this._panels != null ? this._panels.FirstOrDefault(dp => dp.IsActivePanel) : null; }
        }

        /// <summary>
        /// Gets a value indicating if the user has clicked Apply or OK.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user saved changes; otherwise, <c>false</c>.
        /// </value>
        public bool SavedChanges { get; private set; }

        /// <summary>
        /// Gets or sets a value that indicates if this editor should allow changes.
        /// </summary>
        /// <value>
        /// <c>true</c> if edits are allowed; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool AllowChanges => true;

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataEditorBase" /> class.
        /// </summary>
        public DataEditorBase()
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                // Setup dependency injection to prevent design-time errors.
                DesignTime.DesignTimeUtilities.SetupDependencyInjection();
            }

            InitializeComponent();

            this._log = LogManager.GetLogger(About.ApplicationName);
            this._userUpdated = SecurityManager_OnUserUpdate;
            SecurityManager.Current.UserUpdated += this._userUpdated;
        }

        /// <summary>
        ///     Adds the data panel.
        /// </summary>
        /// <param name="panel"> The panel. </param>
        protected void AddDataPanel(IDataPanel panel)
        {
            if(panel == null)
                return;

            panel.Visible = false;
            panel.IsActivePanel = false;
            panel.Enabled = false;
            panel.UpdateCurrentNodeUI += UpdateCurrentNodeUI;

            this._panels.Add(panel);
        }

        /// <summary>
        ///     Ends all edits.
        /// </summary>
        protected void EndAllEdits() { this._panels.ForEach(p => p.EndEditing()); }

        /// <summary>
        ///     Cancels all edits.
        /// </summary>
        protected void CancelAllEdits() { this._panels.ForEach(p => p.CancelEdits()); }

        /// <summary>
        ///     Saves all changes from all data panels.
        /// </summary>
        /// <returns> </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected virtual bool SaveData() { throw new NotImplementedException("Method must be implemented in subclasses"); }

        /// <summary>
        ///     Reloads the TOC. Called after save to update changes.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        protected virtual void ReloadTOC() { throw new NotImplementedException("Method must be implemented in subclasses"); }

        /// <summary>
        ///     Loads the validators of all of the panels.
        /// </summary>
        protected void LoadValidators()
        {
            foreach(DataPanel item in this._panels)
                item.AddValidators(this._validators, this.errProvider);
        }

        /// <summary>
        ///     Validates that the changes to the current controls.
        /// </summary>
        /// <returns> Return True if validation passes. </returns>
        public bool IsValidControls()
        {
            //if not validating then just return success
            if(SuspendValidation)
                return true;

            bool success = false;

            if(this._isDisposing)
                return false;

            if(this.tvwTOC.SelectedNodes.Count > 0)
            {
                if(this.tvwTOC.SelectedNodes[0].Control != null)
                {
                    success = this._validators.ValidateControls();

                    if(success)
                        UpdateCurrentNodeUI();
                }
                else
                    return true;
            }

            return success;
        }

        /// <summary>
        /// Loads the node and its data panel into the view.
        /// </summary>
        /// <param name="node"> The node. </param>
        /// <exception cref="System.NotImplementedException">
        /// Thrown if not implemented in subclass.
        /// </exception>
        protected virtual void LoadNode(UltraTreeNode node)
        {
            throw new NotImplementedException("Method must be implemented in subclasses");
        }

        /// <summary>
        /// Loads data for nodes.
        /// </summary>
        /// <remarks>
        /// Implementors do not need to load all data for the node.
        /// </remarks>
        /// <param name="nodes"></param>
        protected virtual void LoadNodes(List <IDataNode> nodes)
        {
            _log.Debug($"LoadNodes: {nodes.Count}");
        }

        /// <summary>
        ///     Displays the selected panel and hides all other panels.
        /// </summary>
        /// <param name="panel"> The panel. </param>
        protected void DisplayPanel(DataPanel panel)
        {
            if(DesignMode)
                return;

            InitializePanels();

            foreach(DataPanel item in this._panels)
            {
                item.MinimumSizeChanged -= Item_MinimumSizeChanged;

                if(item == panel)
                {
                    item.Visible = true;
                    item.IsActivePanel = true;
                    item.Enabled = AllowChanges;
                    item.BringToFront();

                    UpdateAutoScrollMinSize();
                    item.MinimumSizeChanged += Item_MinimumSizeChanged;
                }
                else
                {
                    item.Visible = false;
                    item.Enabled = false;
                    item.IsActivePanel = false;
                }
            }
        }

        private void UpdateAutoScrollMinSize()
        {
            var activePanel = ActivePanel;
            var autoScrollMinHeight = Math.Max(activePanel.MinimumSize.Height, _defaultAutoScrollMin.Height);
            var autoScrollMinWidth = Math.Max(activePanel.MinimumSize.Width, _defaultAutoScrollMin.Width);
            splitContainer1.Panel2.AutoScrollMinSize = new System.Drawing.Size(autoScrollMinWidth, autoScrollMinHeight);
        }

        /// <summary>
        ///     Initializes all the panels by setting initial locations and docking appropriately.
        /// </summary>
        protected void InitializePanels()
        {
            if(!this._panelsInitialized)
            {
                foreach(IDataPanel item in this._panels)
                {
                    //item.Control.Location 		= new Point(2, 2);
                    //item.Control.Parent.Padding = new Padding(3);
                    //item.Control.Margin 		= new Padding(3);
                    item.Control.Dock = DockStyle.Fill;

                    //item.Control.Width 	= this.splitContainer1.Panel2.Width - 2;
                    //item.Control.Height = this.splitContainer1.Panel2.Height - 2;
                    //item.Control.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

                    item.Visible = false;
                    item.IsActivePanel = false;
                    item.Enabled = false;
                }

                this._panelsInitialized = true;
            }
        }

        /// <summary>
        ///     Saves the currently selected node.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        protected virtual void SaveSelectedNode() { throw new NotImplementedException("Method must be implemented in subclasses"); }

        /// <summary>
        ///     Restores the last selected node.
        /// </summary>
        /// <param name="nodeKey"> The node key. </param>
        protected void RestoreLastSelectedNode(string nodeKey)
        {
            if(!string.IsNullOrEmpty(nodeKey))
            {
                UltraTreeNode node = this.tvwTOC.GetNodeByKey(nodeKey);
                if(node != null && node.Visible)
                    this.tvwTOC.ActiveNode = node;
            }

            if(this.tvwTOC.Nodes.Count > 0 && this.tvwTOC.ActiveNode == null)
            {
                if(this.tvwTOC.Nodes[0].Nodes.Count > 0)
                    this.tvwTOC.ActiveNode = this.tvwTOC.Nodes[0].Nodes[0];
                else
                    this.tvwTOC.ActiveNode = this.tvwTOC.Nodes[0];
            }

            this.tvwTOC.SelectedNodes.Clear();
            this.tvwTOC.PerformAction(UltraTreeAction.SelectActiveNode, false, false);

            if(this.tvwTOC.SelectedNodes.Count == 1)
                this.tvwTOC.SelectedNodes[0].BringIntoView(true);
        }

        /// <summary>
        ///     Called when [dispose].
        /// </summary>
        protected virtual void OnDispose()
        {
            try
            {
                if (DesignMode || _disposed)
                    return;

                this._isDisposing = true;

                this._log.Debug("On data editor disposing: " + Name);

                if(this._userUpdated != null)
                {
                    SecurityManager.Current.UserUpdated -= this._userUpdated;
                    this._userUpdated = null;
                }

                if(this.tvwTOC != null && !this.tvwTOC.IsDisposed)
                    this.tvwTOC.Nodes.DisposeAll();

                if(this._validators != null)
                {
                    this._validators.Dispose();
                    this._validators = null;
                }

                if(this._panels != null)
                {
                    this._panels.ForEach(dp =>
                                         {
                                             dp.UpdateCurrentNodeUI -= UpdateCurrentNodeUI;
                                             dp.Control.Dispose();
                                         });

                    this._panels.Clear();
                    this._panels = null;
                }

                if(this._cmdManager != null)
                {
                    this._cmdManager.Dispose();
                    this._cmdManager = null;
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error disposing form.";
                this._log.Error(exc, errorMsg);
            }
            finally
            {
                this._log = null;
                _disposed = true;
            }
        }

        /// <summary>
        ///     Displays the close button only with the specified label.
        /// </summary>
        /// <param name="closeButtonLabel"> The close button label. </param>
        protected void DisplayCloseButtonOnly(string closeButtonLabel)
        {
            this.btnOK.Visible = false;
            this.btnApply.Visible = false;
            this.btnCancel.Location = this.btnApply.Location;
            this.btnCancel.Text = closeButtonLabel;
        }

        /// <summary>
        ///     Updates the current node UI.
        /// </summary>
        protected void UpdateCurrentNodeUI()
        {
            try
            {
                if(this.tvwTOC.SelectedNodes.Count > 0 && this.tvwTOC.SelectedNodes[0] is IDataNode)
                {
                    var node = this.tvwTOC.SelectedNodes[0] as IDataNode; //Cache handle to Node in case the update clears it from the selection
                    node.UpdateNodeUI();

                    if( ((UltraTreeNode)node).Visible &&  ((UltraTreeNode)node).Selected)
                        ((UltraTreeNode)node).BringIntoView();
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating node UI.");
            }
        }

        #endregion

        #region Events

        /// <summary>
        ///     Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="EventArgs" /> instance containing the event data. </param>
        protected virtual void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                using(new UsingWaitCursor(this))
                {
                    if(IsValidControls())
                    {
                        this._log.Info("On btnOK_Click: " + Name);

                        if(SaveData())
                        {
                            SavedChanges = true;
                            DialogResult = DialogResult.OK;
                            SaveSelectedNode();
                            Close();
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error closing form.", exc);
            }
        }

        /// <summary>
        ///     Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="EventArgs" /> instance containing the event data. </param>
        protected virtual void btnCancel_Click(object sender, EventArgs e)
        {
            this._log.Info("Canceling form saves btnCancel_Click: " + Name);
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        ///     Handles the Click event of the btnApply control.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="EventArgs" /> instance containing the event data. </param>
        protected virtual void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = IsValidControls();

                if(success)
                {
                    this._log.Info("On btnApply_Click: " + Name);

                    var selectedNode = this.tvwTOC.SelectedNode <UltraTreeNode>();

                    this.tvwTOC.SelectedNodes.Clear();

                    //find any nodes that have changes
                    var nodesWithChanges = new List <IDataNode>();
                    this.tvwTOC.Nodes.ForEachNode(n =>
                                                  {
                                                      if(n is IDataNode && ((IDataNode) n).HasChanges)
                                                          nodesWithChanges.Add((IDataNode) n);
                                                  });

                    if (SaveData())
                    {
                        SavedChanges = true;
                    }

                    //update UI for any nodes that have changes 
                    nodesWithChanges.ForEach(n =>
                                             {
                                                 if(n.IsRowValid)
                                                     n.UpdateNodeUI();
                                             });
                    if (selectedNode != null)
                    {
                        // Try to re-select node that was previously selected
                        var node = tvwTOC.Nodes[0].FindNode<UltraTreeNode>(tt => ReferenceEquals(tt, selectedNode));

                        //if don't find selected row then try then find parent
                        if (node == null && selectedNode.Parent != null)
                        {
                            node = tvwTOC.Nodes[0].FindNode<UltraTreeNode>(tt => ReferenceEquals(tt, selectedNode.Parent));
                        }

                        if (node != null)
                        {
                            this.tvwTOC.SelectedNodes.Clear();
                            node.Select();
                        }
                    }

                    //if nothing selected then select the root node
                    if(this.tvwTOC.SelectedNodes.Count < 1 && this.tvwTOC.Nodes.Count > 0)
                        this.tvwTOC.Nodes[0].Select();
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error applying changes to form.", exc);
            }
        }

        /// <summary>
        ///     Handles the AfterSelect event of the tvwTOC control.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="SelectEventArgs" /> instance containing the event data. </param>
        protected virtual void tvwTOC_AfterSelect(object sender, SelectEventArgs e)
        {
            try
            {
                if(e.NewSelections.Count == 1)
                {
                    UltraTreeNode node = e.NewSelections[0];
                    LoadNode(node);
                    node.BringIntoView(true);
                    node.Expanded = true;
                }
                else if (e.NewSelections.Count > 1)
                {
                    List <IDataNode> nodes = e.NewSelections.All.OfType <IDataNode>().ToList();
                    if(nodes.Count > 0)
                        LoadNodes(nodes);

                    e.NewSelections[e.NewSelections.Count - 1].BringIntoView(true);
                }
            }
            catch(Exception exc)
            {
                this._log.Warn(exc, "Error processing selection.");
            }
        }

        /// <summary>
        ///     Handles the BeforeSelect event of the tvwTOC control.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="BeforeSelectEventArgs" /> instance containing the event data. </param>
        protected virtual void tvwTOC_BeforeSelect(object sender, BeforeSelectEventArgs e)
        {
            try
            {
                //if a node is selected then validate it before moving to the next node
                if(this.tvwTOC.SelectedNodes.Count > 0)
                {
                    if(IsValidControls())
                        EndAllEdits();
                    else
                        e.Cancel = true;
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error processing selection.", exc);
            }
        }

        /// <summary>
        ///     Securities the manager_ on user change.
        /// </summary>
        private void SecurityManager_OnUserUpdate(object sender, UserUpdatedEventArgs args)
        {
            this._log.Debug("SecurityManager_OnUserChange in DataEditorBase." + Name);

            if(InvokeRequired)
                BeginInvoke(new Action(Close));
            else
            {
                //if user changes then close form w/o saving
                Close();
            }
        }

        /// <summary>
        ///     Handles the KeyUp event of the tvwTOC control.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="KeyEventArgs" /> instance containing the event data. </param>
        protected virtual void tvwTOC_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if(Commands != null)
                {
                    ICommandBase cmd = Commands.FindCommandByKeyMapping(e.KeyCode);

                    if(cmd != null)
                    {
                        if(cmd.Enabled)
                        {
                            this._log.Info("Found hot key mapping for: " + e.KeyCode);
                            cmd.OnClick();
                            e.Handled = true;
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on TOC key up.");
            }
        }

        /// <summary>
        ///     Handles the KeyUp event of the DataEditorBase control.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="KeyEventArgs" /> instance containing the event data. </param>
        private void DataEditorBase_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if(e.KeyCode == Keys.Escape)
                {
                    this._log.Debug("User hit escape key.");
                    CancelAllEdits();
                    e.Handled = true;
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on data editor key up.");
            }
        }

        private void tvwTOC_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    return;
                }

                var nodeAtPoint = tvwTOC.GetNodeFromPoint(e.Location);

                if (nodeAtPoint == null)
                {
                    tvwTOC.SelectedNodes.Clear();
                }
                else if (e.Button != MouseButtons.Left)
                {
                    // UltraTree has 'left click to select' built-in.
                    // Must manually select nodes when using other buttons.

                    // If using right-click, make sure that the list doesn't
                    // include the item before changing the selection.
                    var skipSelection = nodeAtPoint == null
                        || (e.Button == MouseButtons.Right && tvwTOC.SelectedNodes.IndexOf(nodeAtPoint) >= 0);

                    if (!skipSelection)
                    {
                        nodeAtPoint.Select(false);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling mouse up event.");
            }
        }

        private void DataEditorBase_Load(object sender, EventArgs e)
        {
            try
            {
                _defaultAutoScrollMin = splitContainer1.Panel2.AutoScrollMinSize;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error loading data editor");
            }
        }

        private void Item_MinimumSizeChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateAutoScrollMinSize();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling MinimumSize change event.");
            }
        }

        #endregion
    }

    /// <summary>
    ///     Simple interface for the data editor.
    /// </summary>
    public interface IDataEditor
    {
        /// <summary>
        ///     Gets or sets a value indicating whether [suspend validation].
        /// </summary>
        /// <value> <c>true</c> if [suspend validation]; otherwise, <c>false</c> . </value>
        bool SuspendValidation { get; set; }

        /// <summary>
        ///     Gets the active panel.
        /// </summary>
        /// <value> The active panel. </value>
        IDataPanel ActivePanel { get; }
    }

    #region IDataPanel

    /// <summary>
    ///     Inteface of the data panels that go into the data editor.
    /// </summary>
    public interface IDataPanel
    {
        /// <summary>
        ///     Gets the control.
        /// </summary>
        /// <value> The control. </value>
        UserControl Control { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IDataPanel" /> is editable.
        /// </summary>
        /// <value> <c>true</c> if editable; otherwise, <c>false</c> . </value>
        bool Editable { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is active panel.
        /// </summary>
        /// <value> <c>true</c> if this instance is active panel; otherwise, <c>false</c> . </value>
        bool IsActivePanel { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IDataPanel" /> is visible.
        /// </summary>
        /// <value> <c>true</c> if visible; otherwise, <c>false</c> . </value>
        bool Visible { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IDataPanel" /> is enabled.
        /// </summary>
        /// <value> <c>true</c> if enabled; otherwise, <c>false</c> . </value>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the minimum size of the panel.
        /// </summary>
        /// <value>The minimum size of the control.</value>
        System.Drawing.Size MinimumSize { get; set; }

        /// <summary>
        ///     Occurs when something changes that would affect the selected nodes UI. (i.e. the name of the object changes)
        ///     Call this in the datapanel to get the data editor to update the nodes ui
        /// </summary>
        event Action UpdateCurrentNodeUI;

        /// <summary>
        ///     Ends the editing.
        /// </summary>
        void EndEditing();

        /// <summary>
        ///     Cancels the edits.
        /// </summary>
        void CancelEdits();

        /// <summary>
        ///     Moves to record.
        /// </summary>
        /// <param name="id"> The id. </param>
        /// <returns> </returns>
        bool MoveToRecord(object id);

        /// <summary>
        ///     Adds the validators.
        /// </summary>
        /// <param name="manager"> The manager. </param>
        /// <param name="errProvider"> The err provider. </param>
        void AddValidators(ValidatorManager manager, ErrorProvider errProvider);
    }

    #endregion
}