using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Shared;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;

namespace DWOS.UI.Admin
{
    public partial class ProcessPackages: DataEditorBase
    {
        #region Properties

        /// <summary>
        /// Gets the currently selected process package row.
        /// </summary>
        public ProcessPackageDataset.ProcessPackageRow SelectedProcessPackage =>
            tvwTOC.SelectedNodesOfType<PackageNode>().FirstOrDefault()?.DataRow;

        #endregion

        #region Methods

        public ProcessPackages()
        {
            this.InitializeComponent();
        }

        private void LoadData()
        {
            this.dsProcesses.EnforceConstraints = false;
            this.dsProcesses.ProcessPackage.BeginLoadData();
            this.dsProcesses.ProcessPackage_Processes.BeginLoadData();

            this.taProcessAlias.Fill(this.dsProcesses.ProcessAlias);
            this.taProcessPackage.Fill(this.dsProcesses.ProcessPackage);
            this.taProcessPackage_Processes.Fill(this.dsProcesses.ProcessPackage_Processes);
            this.taProcess.Fill(this.dsProcesses.Process);

            this.dsProcesses.ProcessPackage.EndLoadData();
            this.dsProcesses.ProcessPackage_Processes.EndLoadData();
            this.dsProcesses.EnforceConstraints = true;

            this.dpProcessPackageInfo.LoadData(this.dsProcesses);
            base.AddDataPanel(this.dpProcessPackageInfo);
        }

        private void LoadTOC()
        {
            tvwTOC.Nodes.Clear();

            UltraTreeNode rootNode = new PackageRootNode();
            tvwTOC.Nodes.Add(rootNode);
            rootNode.Expanded = true;

            foreach(ProcessPackageDataset.ProcessPackageRow pr in this.dsProcesses.ProcessPackage)
                rootNode.Nodes.Add(new PackageNode(pr));

            if(rootNode.Nodes.Count > 0)
                rootNode.Nodes[0].Select();
            else
                rootNode.Select();
        }

        protected override void ReloadTOC()
        {
            this.LoadTOC();
        }

        protected override bool SaveData()
        {
            try
            {
                base.EndAllEdits();

                this.taManager.UpdateAll(this.dsProcesses);

                return true;
            }
            catch(Exception exc)
            {
                _log.Warn("DataSet Errors: " + this.dsProcesses.GetDataErrors());
                ErrorMessageBox.ShowDialog("Error saving data.", exc);
                return false;
            }
        }

        private void LoadCommands()
        {
            if(SecurityManager.Current.IsInRole("ProcessPackages.Edit"))
            {
                var add = new AddCommand(toolbarManager.Tools["Add"], tvwTOC);
                add.AddNode = this.AddNode;

                new DeleteCommand(toolbarManager.Tools["Delete"], tvwTOC, this);
                new UpdateProcessesCommand(toolbarManager.Tools["UpdateProcesses"], this);
            }
        }

        private void AddProcessPackage(PackageRootNode rn)
        {
            _log.Info("Adding a PackageNode.");

            //create new data source
            ProcessPackageDataset.ProcessPackageRow cr = this.dpProcessPackageInfo.AddRow();

            //create new ui nodes
            var cn = new PackageNode(cr);
            rn.Nodes.Add(cn);
            cn.Select();
        }

        protected override void LoadNode(UltraTreeNode node)
        {
            if(node is PackageNode)
            {
                var packNode = (PackageNode)node;

                DisplayPanel(this.dpProcessPackageInfo);
                this.dpProcessPackageInfo.MoveToRecord(packNode.ID);
            }
            else
                DisplayPanel(null);
        }

        protected override void SaveSelectedNode()
        {
            if (tvwTOC.SelectedNodes.Count > 0)
            {
                Properties.Settings.Default.LastSelectedProcessPackage = tvwTOC.SelectedNodes[0].Key;
            }
        }


        private void UpdateProcesses(PackageNode packageNode)
        {
            if (packageNode == null)
            {
                return;
            }

            var inactivePackageProcesses = packageNode.DataRow
                .GetProcessPackage_ProcessesRows()
                .Where(p => p.ProcessRow != null && !p.ProcessRow.Active);

            using (var taProcess = new Data.Datasets.PartsDatasetTableAdapters.ProcessTableAdapter())
            {
                foreach (var packageProcess in inactivePackageProcesses)
                {
                    // Find newest version
                    var revisedProcessId = taProcess.Get_RevisedProcess(packageProcess.ProcessID) as int?;
                    var revisedProcess = dsProcesses.Process.FindByProcessID(revisedProcessId ?? -1);

                    if (revisedProcess == null || revisedProcess.ProcessID == packageProcess.ProcessID)
                    {
                        continue;
                    }

                    // Find alias
                    var revisedAlias = revisedProcess.GetProcessAliasRows().FirstOrDefault();
                    foreach (var alias in revisedProcess.GetProcessAliasRows())
                    {
                        //if they have the same name
                        if (alias.Name == packageProcess.ProcessAliasRow.Name)
                        {
                            // Exact match
                            revisedAlias = alias;
                            break;
                        }
                    }

                    // Update process & alias for package
                    if (revisedAlias != null)
                    {
                        packageProcess.ProcessRow = revisedProcess;
                        packageProcess.ProcessAliasRow = revisedAlias;
                    }
                }
            }

            // Selecting the node refreshes data
            packageNode.Select();
        }

        #endregion

        #region Events

        private void Part_Load(object sender, EventArgs e)
        {
            _loadingData = true; //prevent TOC from loading

            try
            {
                Cursor = Cursors.WaitCursor;
                this.LoadCommands();
                Application.DoEvents();

                this.LoadData();
                this.LoadTOC();
                LoadValidators();

                tvwTOC.Override.Sort = SortType.Ascending;

                _loadingData = false;

                splitContainer1.Panel2.Enabled = SecurityManager.Current.IsInRole("ProcessPackages.Edit");

                // Select previously selected process package
                var lastSelectedPackage = Properties.Settings.Default.LastSelectedProcessPackage;
                if (!string.IsNullOrEmpty(lastSelectedPackage))
                {
                    RestoreLastSelectedNode(lastSelectedPackage);
                }
            }
            catch(Exception exc)
            {
                _log.Warn("DataSet Errors: " + this.dsProcesses.GetDataErrors());
                splitContainer1.Enabled = false;
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void cboCustomerFilter_ValueChanged(object sender, EventArgs e)
        {
            if(!_loadingData)
                this.ReloadTOC();
        }

        private void AddNode(object sender, EventArgs e)
        {
            UltraTreeNode selectedNode = tvwTOC.SelectedNodes[0];

            if(IsValidControls())
            {
                _validators.Enabled = false;

                if(selectedNode is PackageRootNode)
                    this.AddProcessPackage((PackageRootNode)selectedNode);

                _validators.Enabled = true;
            }
        }

        #endregion

        #region Nodes

        #region Nested type: PackageNode

        internal class PackageNode: DataNode<ProcessPackageDataset.ProcessPackageRow>
        {
            #region Properties

            public const string KEY_PREFIX = "PP";

            #endregion

            #region Methods

            public PackageNode(ProcessPackageDataset.ProcessPackageRow cr)
                : base(cr, cr.ProcessPackageID.ToString(), KEY_PREFIX, cr.Name)
            {
                LeftImages.Add(Properties.Resources.Part_16);

                this.UpdateNodeUI();
            }

            public override bool CanDelete
            {
                get { return true; }
            }

            public override void UpdateNodeUI()
            {
                Text = base.DataRow.Name;
                Override.NodeAppearance.FontData.Italic = DefaultableBoolean.False;
                Override.NodeAppearance.ForeColor = Color.Black;
            }

            #endregion

            public override string ClipboardDataFormat
            {
                get { return GetType().FullName; }
            }
        }

        #endregion

        #region Nested type: PackageRootNode

        internal class PackageRootNode: UltraTreeNode
        {
            #region Methods

            public PackageRootNode()
                : base("ROOT", "Process Packages")
            {
                LeftImages.Add(Properties.Resources.Part_16);
            }

            #endregion
        }

        #endregion

        #endregion

        #region Commands

        internal class AddCommand: TreeNodeCommandBase
        {
            #region Fields

            public EventHandler AddNode;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return _node is PackageRootNode; }
            }

            #endregion

            #region Methods

            public AddCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                if(_node is PackageRootNode)
                {
                    if(this.AddNode != null)
                        this.AddNode(this, EventArgs.Empty);
                }
            }

            #endregion
        }

        #endregion

        #region UpdateProcessesCommand

        internal class UpdateProcessesCommand : TreeNodeCommandBase
        {
            public override bool Enabled => _node is PackageNode;

            public ProcessPackages ProcessPackages { get; }

            public UpdateProcessesCommand(ToolBase toolBase, ProcessPackages processPackages)
                : base(toolBase)
            {
                if (processPackages == null)
                {
                    throw new ArgumentNullException(nameof(processPackages));
                }

                TreeView = processPackages.tvwTOC;
                ProcessPackages = processPackages ?? throw new ArgumentNullException(nameof(processPackages));
            }

            public override void OnClick()
            {
                ProcessPackages.UpdateProcesses(_node as PackageNode);
            }
        }

        #endregion
    }
}