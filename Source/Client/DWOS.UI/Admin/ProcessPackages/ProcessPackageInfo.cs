using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using DWOS.Utilities.Validation;
using DWOS.Data.Datasets;
using DWOS.Shared;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;

namespace DWOS.UI.Admin.ProcessPackagePanels
{
    public partial class ProcessPackageInfo: DataPanel
    {
        #region Fields

        private MultiLineForm _addProcessCBO;

        #endregion

        #region Properties

        public ProcessPackageDataset Dataset
        {
            get { return base._dataset as ProcessPackageDataset; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return this.Dataset.ProcessPackage.ProcessPackageIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public ProcessPackageInfo()
        {
            this.InitializeComponent();
        }

        public void LoadData(ProcessPackageDataset dataset)
        {
            this.Dataset = dataset;

            bsData.DataSource = this.Dataset;
            bsData.DataMember = this.Dataset.ProcessPackage.TableName;

            //bind column to control
            base.BindValue(this.txtName, this.Dataset.ProcessPackage.NameColumn.ColumnName);

            base._panelLoaded = true;

            this.tvwProcesses.Override.SortComparer = new ProcessNodeSorter();

            var deleteProcess = new DeleteButtonCommand(this.btnDeleteProcess, this.tvwProcesses);
            deleteProcess.AfterDelete += (node) => this.ReIndexProcessNodes();
        }

        public override void AddValidators(ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtName, "Process Package name required.") { DefaultValue = "New Process Package" }, errProvider));
        }

        public ProcessPackageDataset.ProcessPackageRow AddRow()
        {
            var rowVw = bsData.AddNew() as DataRowView;
            var cr    = rowVw.Row as ProcessPackageDataset.ProcessPackageRow;
            cr.Name   = "New Process Package";

            return cr;
        }

        private void AddProcess()
        {
            try
            {
                var packRow = base.CurrentRecord as ProcessPackageDataset.ProcessPackageRow;

                if(packRow != null)
                {
                    int selectedProcessID = -1, selectedProcessAliasID = -1;

                    if(this._addProcessCBO == null)
                    {
                        this._addProcessCBO = new MultiLineForm();
                        var dt = new DataTable();
                        dt.Columns.Add("ProcessID", typeof(int));
                        dt.Columns.Add("ProcessAliasID", typeof(int));
                        dt.Columns.Add("Name", typeof(string));
                        dt.Columns.Add("Code", typeof(string));

                        foreach(ProcessPackageDataset.ProcessAliasRow pa in this.Dataset.ProcessAlias)
                        {
                            ProcessPackageDataset.ProcessRow pr = pa.ProcessRow;
                            if(pr != null && pr.Active)
                                dt.Rows.Add(pa.ProcessID, pa.ProcessAliasID, pa.Name, pr.Name);
                        }

                        this._addProcessCBO.dropDown.DataSource = dt;

                        this._addProcessCBO.LayoutName = "PartManger_Processesv4";
                        this._addProcessCBO.dropDown.DisplayLayout.Bands[0].Columns[0].Hidden = true;
                        this._addProcessCBO.dropDown.DisplayLayout.Bands[0].Columns[1].Hidden = true;
                        this._addProcessCBO.dropDown.DisplayLayout.Bands[0].Columns[2].SortIndicator = SortIndicator.Ascending;
                        this._addProcessCBO.dropDown.Visible = true;
                        this._addProcessCBO.FormLabel.Text = "Processes:";
                        this._addProcessCBO.Text = "Select Process";
                    }

                    if(this._addProcessCBO.ShowDialog(this) == DialogResult.OK && this._addProcessCBO.dropDown.Selected.Rows.Count == 1)
                    {
                        selectedProcessID = Convert.ToInt32(this._addProcessCBO.dropDown.Selected.Rows[0].Cells["ProcessID"].Value);
                        selectedProcessAliasID = Convert.ToInt32(this._addProcessCBO.dropDown.Selected.Rows[0].Cells["ProcessAliasID"].Value);
                    }

                    if(selectedProcessID >= 0)
                    {
                        ProcessPackageDataset.ProcessRow pr = this.Dataset.Process.FindByProcessID(selectedProcessID);

                        //create new relation
                        ProcessPackageDataset.ProcessPackage_ProcessesRow pppRow = this.Dataset.ProcessPackage_Processes.AddProcessPackage_ProcessesRow(packRow, pr, this.GetNextProcessStepOrder(packRow), this.Dataset.ProcessAlias.FindByProcessAliasID(selectedProcessAliasID));

                        //create new ui nodes
                        var cn = new ProcessNode(pppRow);
                        this.tvwProcesses.Nodes.Add(cn);

                        //select new nodes
                        cn.Select();
                    }
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error adding the process to the part.", exc);
            }
        }

        private int GetNextProcessStepOrder(ProcessPackageDataset.ProcessPackageRow partRow)
        {
            ProcessPackageDataset.ProcessPackage_ProcessesRow[] rows = partRow.GetProcessPackage_ProcessesRows();
            int maxCount = 0;

            foreach(ProcessPackageDataset.ProcessPackage_ProcessesRow row in rows)
            {
                if(row.StepOrder > maxCount)
                    maxCount = row.StepOrder;
            }

            return maxCount + 1;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            var processRow = base.CurrentRecord as ProcessPackageDataset.ProcessPackageRow;

            this.tvwProcesses.Nodes.Clear();

            if(processRow != null)
            {
                //Load Process Rows
                foreach(ProcessPackageDataset.ProcessPackage_ProcessesRow row in processRow.GetProcessPackage_ProcessesRows())
                    this.tvwProcesses.Nodes.Add(new ProcessNode(row));

                if(this.tvwProcesses.Nodes.Count > 0)
                    this.tvwProcesses.Nodes[0].Select();
            }

            base.AfterMovedToNewRecord(id);
        }

        private void MoveProcessUp()
        {
            if(this.tvwProcesses.SelectedNodes.Count == 1)
            {
                var pn = this.tvwProcesses.SelectedNodes[0] as ProcessNode;

                //if not first one
                if(pn.HasSibling(NodePosition.Previous))
                {
                    this.tvwProcesses.Override.Sort = SortType.None;

                    var pnPrev = pn.GetSibling(NodePosition.Previous) as ProcessNode;
                    pn.Reposition(pnPrev, NodePosition.Previous);

                    this.ReIndexProcessNodes();

                    this.tvwProcesses.Override.Sort = SortType.Ascending;
                }

                pn.Select();
            }
        }

        private void MoveProcessDown()
        {
            if(this.tvwProcesses.SelectedNodes.Count == 1)
            {
                var pn = this.tvwProcesses.SelectedNodes[0] as ProcessNode;

                if(pn.HasSibling(NodePosition.Next))
                {
                    this.tvwProcesses.Override.Sort = SortType.None; //disable sort

                    var pnNext = pn.GetSibling(NodePosition.Next) as ProcessNode;
                    pn.Reposition(pnNext, NodePosition.Next);

                    this.ReIndexProcessNodes();

                    this.tvwProcesses.Override.Sort = SortType.Ascending; //reenable sort
                }

                pn.Select();
            }
        }

        private void ReIndexProcessNodes()
        {
            foreach(UltraTreeNode item in this.tvwProcesses.Nodes)
            {
                var pn = item as ProcessNode;

                if(pn != null)
                {
                    int currentIndex = pn.Index;
                    pn.DataRow.StepOrder = currentIndex + 1;
                    pn.UpdateNodeUI();
                }
            }
        }

        #endregion

        #region Events

        private void btnAddProcess_Click(object sender, EventArgs e)
        {
            try
            {
                this.AddProcess();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error adding new process.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            try
            {
                this.MoveProcessUp();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error moving process up.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            try
            {
                this.MoveProcessDown();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error moving process down.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion

        #region Nodes

        internal class ProcessNode: DataNode<ProcessPackageDataset.ProcessPackage_ProcessesRow>
        {
            public const string KEY_PREFIX = "PP";

            #region Methods

            public ProcessNode(ProcessPackageDataset.ProcessPackage_ProcessesRow cr)
                : base(cr, cr.ProcessPackage_Process_ID.ToString(), KEY_PREFIX, cr.ProcessID.ToString())
            {
                LeftImages.Add(Properties.Resources.Process_16);

                this.UpdateNodeUI();

                if (!base.DataRow.ProcessRow.Active)
                    base.LeftImages.Add(Properties.Resources.RoundDashRed_32);
            }

            public override bool CanDelete
            {
                get { return true; }
            }

            public override void UpdateNodeUI()
            {
                Text = base.DataRow.StepOrder + " - " + base.DataRow.ProcessAliasRow.Name + " [" + base.DataRow.ProcessRow.Name + "]";
            }

            #endregion
        }

        #endregion

        #region PartProcessNodeNodesSorter

        /// <summary>
        ///   Customer sorter to sort process nodes by there step order
        /// </summary>
        private class ProcessNodeSorter: IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                var xNode = x as ProcessNode;
                var yNode = y as ProcessNode;

                if (xNode != null && yNode != null && xNode.IsRowValid && yNode.IsRowValid)
                    return xNode.DataRow.StepOrder.CompareTo(yNode.DataRow.StepOrder);
                
                return (x ?? "").ToString().CompareTo((y ?? "").ToString());
            }

            #endregion
        }

        #endregion
    }
}