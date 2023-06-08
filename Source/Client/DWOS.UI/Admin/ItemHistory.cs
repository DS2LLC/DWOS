using System.Data;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ItemHistoryDataSetTableAdapters;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;

namespace DWOS.UI.Admin
{
    public partial class ItemHistory: Form
    {
        #region Fields

        private IItemHistoryLoader _historyLoader;

        #endregion

        #region Methods

        public ItemHistory()
        {
            this.InitializeComponent();
        }

        public void LoadHistory(IItemHistoryLoader historyLoader)
        {
            this._historyLoader = historyLoader;
            historyLoader.LoadNodes(this.tvwNodes);

            this.grdValues.DataSource = null;
            this.grdValues.DataMember = null;

            if(this.tvwNodes.Nodes.Count > 0)
                this.tvwNodes.Nodes[0].Select();
        }

        #endregion

        #region Events

        private void tvwNodes_AfterSelect(object sender, SelectEventArgs e)
        {
            if(e.NewSelections.Count == 1 && e.NewSelections[0] is ItemHistoryNode)
                this.grdValues.DataSource = ((ItemHistoryNode)e.NewSelections[0]).Table;
            else
                this.grdValues.DataSource = null;
        }

        private void grdValues_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if(this._historyLoader != null)
                this._historyLoader.UpdateLayout(e.Layout);
        }

        #endregion

        #region ItemHistoryNode

        public class ItemHistoryNode: UltraTreeNode
        {
            public DataTable Table { get; set; }
        }

        #endregion
    }

    public interface IItemHistoryLoader
    {
        void LoadNodes(UltraTree tree);
        void UpdateLayout(UltraGridLayout gridLayout);
    }

    public class PartHistoryLoader: IItemHistoryLoader
    {
        public PartHistoryLoader(int partID)
        {
            this.PartID = partID;
        }

        public int PartID { get; set; }

        #region IItemHistoryLoader Members

        public void LoadNodes(UltraTree tree)
        {
            tree.Nodes.Clear();
            this.AddPartHistory(tree);
        }

        public void UpdateLayout(UltraGridLayout gridLayout) {}

        #endregion

        private void AddPartHistory(UltraTree tree)
        {
            using(var taAudit = new AuditTableAdapter())
            {
                ItemHistoryDataSet.AuditDataTable dtAudit = taAudit.GetData("dbo.Part", this.PartID.ToString());
                var partNode = new ItemHistory.ItemHistoryNode{Text = "Part " + this.PartID, Table = dtAudit};
                tree.Nodes.Add(partNode);

                //this.AddPartProcessHistory(partNode);
            }
        }

        //private void AddOrdersHistory(UltraTreeNode partNode)
        //{
        //    //using(Data.Data.ItemHistoryDataSetTableAdapters.AuditTableAdapter taAudit = new DWOS.Data.Datasets.ItemHistoryDataSetTableAdapters.AuditTableAdapter())
        //    //{
        //    //    UltraTreeNode processesNode = new UltraTreeNode() { Text = "Processes" };
        //    //    DWOS.Data.Datasets.ItemHistoryDataSet.AuditDataTable dtAudit = taAudit.GetData("dbo.Part", PartID.ToString());
        //    //    ItemHistory.ItemHistoryNode partNode = new ItemHistory.ItemHistoryNode() { Text = "Part " + PartID.ToString(), Table = dtAudit };
        //    //    tree.Nodes.Add(partNode);
        //    //}
        //}

        //private void AddPartProcessHistory(UltraTreeNode partNode)
        //{
        //    //using(Data.Data.ItemHistoryDataSetTableAdapters.AuditTableAdapter taAudit = new DWOS.Data.Datasets.ItemHistoryDataSetTableAdapters.AuditTableAdapter())
        //    //{
        //    //    UltraTreeNode processesNode = new UltraTreeNode() {Text = "Processes" };
        //    //    DWOS.Data.Datasets.ItemHistoryDataSet.AuditDataTable dtAudit = taAudit.GetData("dbo.Part", PartID.ToString());
        //    //    ItemHistory.ItemHistoryNode partNode = new ItemHistory.ItemHistoryNode() { Text = "Part " + PartID.ToString(), Table = dtAudit };
        //    //    tree.Nodes.Add(partNode);
        //    //}
        //}
    }
}