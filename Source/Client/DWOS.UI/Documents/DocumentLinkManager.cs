using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinTree;

namespace DWOS.UI.Documents
{
    public partial class DocumentLinkManager : UserControl
    {
        #region Fields

        private const string LINK_INFO_ID = "DocumentInfoID";
        private const string LINK_TYPE = "LinkToType";
        private const string LINK_KEY = "LinkToKey";

        #endregion

        #region Properties

        public LinkType LinkType { get; set; }

        public DataTable ParentTable { get; set; }

        public DataTable DocumentLinkTable { get; set; }

        public string TableKeyColumn { get; set; }

        public DataRow CurrentRow { get; set; }

        #endregion

        #region Methods

        public DocumentLinkManager()
        {
            InitializeComponent();
        }

        public void InitializeData(LinkType linkType, DataTable parentTable, DataTable documentLinkTable)
        {
            if(DesignMode)
                return;

            LinkType = linkType;
            ParentTable = parentTable;
            DocumentLinkTable = documentLinkTable;

            DataColumn primaryKey = ParentTable.PrimaryKey.FirstOrDefault();
            TableKeyColumn = primaryKey.ColumnName;
        }

        public void ClearLinks()
        {
            this.tvwDocuments.Nodes.Clear();
        }

        public void LoadLinks(DataRow dataRow, IEnumerable<DataRow> documentLinks)
        {
            ClearLinks();

            CurrentRow = dataRow;
            int linkKey = dataRow.GetInt32(TableKeyColumn);

            foreach (var row in documentLinks)
            {
                LoadLink(row);
            }
        }

        private void LoadLink(DataRow linkRow)
        {
            var documentInfoID = Convert.ToInt32(linkRow[LINK_INFO_ID]);
            var node = new DocumentLinkNode(linkRow, DocumentUtilities.GetFileName(documentInfoID));
            this.tvwDocuments.Nodes.Add(node);
        }

        private void AddLink()
        {
            if (CurrentRow == null)
            {
                return;
            }

            using(var docManager = new DocumentManager())
            {
                docManager.ShowDialog(this);

                List <DocumentsDataSet.DocumentInfoRow> docs = docManager.SelectedDocuments;
                if(docs != null)
                {
                    var linkKey = CurrentRow.GetInt32(TableKeyColumn);
                    var existing = DocumentLinkTable.Select($"{LINK_KEY} = {linkKey}");

                    var duplicateDocs = new List<DocumentsDataSet.DocumentInfoRow>();
                    foreach (var doc in docs)
                    {
                        if (existing.Any(existingRow => Convert.ToInt32(existingRow[LINK_INFO_ID]) == doc.DocumentInfoID))
                        {
                            duplicateDocs.Add(doc);
                        }
                        else
                        {
                            AddLink(doc.DocumentInfoID, doc.Name, linkKey);
                        }
                    }

                    if (duplicateDocs.Count > 0)
                    {
                        var errorMsg = duplicateDocs.Count > 1
                            ? $"{duplicateDocs.Count} documents were already linked."
                            : $"{duplicateDocs.First().Name} was already linked.";

                        MessageBoxUtilities.ShowMessageBoxWarn(errorMsg,
                            "Documents");
                    }
                }
            }
        }

        private void AddLink(int documentInfoId, string name, int linkToKey)
        {
            var link = DocumentLinkTable.NewRow();
            link[LINK_INFO_ID] = documentInfoId;
            link[LINK_TYPE] = LinkType.ToString();
            link[LINK_KEY] = linkToKey;
            DocumentLinkTable.Rows.Add(link);

            LoadLink(link);
        }

        private void DeleteLink()
        {
            var mediaNode = this.tvwDocuments.SelectedNode <DocumentLinkNode>();

            if(mediaNode != null)
            {
                this.tvwDocuments.SelectedNodes.Clear();
                this.tvwDocuments.Nodes.Remove(mediaNode);

                mediaNode.DataRow.Delete();
            }
        }

        #endregion

        private void tvwDocuments_AfterSelect(object sender, SelectEventArgs e)
        {
            this.btnDelete.Enabled = e.NewSelections.Count > 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddLink();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteLink();
        }

        #region Nested type: DocumentLinkNode

        internal class DocumentLinkNode : DataNode <DataRow>
        {
            #region Fields

            public const string KEY_PREFIX = "LINK";

            #endregion

            #region Properties

            public string FileName
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public DocumentLinkNode(DataRow cr, string fileName) : base(cr, cr[LINK_INFO_ID].ToString(), KEY_PREFIX, fileName)
            {
                base.LeftImages.Clear();
                FileName = fileName;
            }

            #endregion
        }

        #endregion
    }
}