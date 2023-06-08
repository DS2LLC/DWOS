using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DWOS.Data.Datasets;
using Infragistics.Win.UltraWinTree;

namespace DWOS.UI.Utilities
{
    internal class DataNode<T> : UltraTreeNode, IDataNode, ICopyPasteNode, ISortable, IDeleteNode where T : DataRow
    {
        #region Properties

        public T DataRow { get; set; }

        public virtual string ID
        {
            get { return DataRow == null ? null : DataRow.PrimaryKey(); }
        }

        /// <summary>
        ///     Gets the key used to sort the node by.
        /// </summary>
        public virtual string SortKey
        {
            get { return Text; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance's row is valid (not deleted / detached).
        /// </summary>
        /// <value><c>true</c> if this instance is row valid; otherwise, <c>false</c>.</value>
        public bool IsRowValid 
        {
            get { return this.DataRow.IsValidState(); }
        }

        #endregion

        #region Methods

        public DataNode(T cr, string id, string keyPrefix, string name) : base(CreateKey(keyPrefix, id), name) { DataRow = cr; }

        public static string CreateKey(string keyPrefix, string id) { return keyPrefix + "-" + id; }

        public override void Dispose()
        {
            DataRow = null;
            base.Dispose();
        }

        public static DataRow AddPastedDataRows(DataRowProxy proxy, DataTable dt, List<DataRowProxyResults> results = null)
        {
            if (proxy == null || dt == null)
            {
                return null;
            }

            var dr = dt.Rows.Add(proxy.ItemArray);
            results?.Add(new DataRowProxyResults {Row = dr, Proxy = proxy});

            foreach (var proxyChild in proxy.ChildProxies ?? Enumerable.Empty<DataRowProxy>())
            {
                var relation = dt.ChildRelations[proxyChild.ParentRelation];
                var childRow = AddPastedDataRows(proxyChild, relation.ChildTable, results);
                childRow.SetParentRow(dr, relation);
            }

            return dr;
        }

        #endregion

        #region ICopyPasteNode Members

        public virtual UltraTreeNode PasteData(string format, DataRowProxy dr) { return null; }

        public virtual bool CanPasteData(string format) { return false; }

        public virtual string ClipboardDataFormat
        {
            get { return null; }
        }

        #endregion

        #region IDataNode Members

        /// <summary>
        ///     Called when the data source this node represents changes.
        /// </summary>
        public virtual void UpdateNodeUI() { }

        DataRow IDataNode.DataRow
        {
            get { return DataRow; }
        }

        public virtual bool HasChanges
        {
            get { return DataRow != null && DataRow.RowState != DataRowState.Unchanged; }
        }

        #endregion

        #region IDeleteNode Members

        /// <summary>
        ///     Deletes this node from the tree and from datastore and all the children.
        /// </summary>
        public virtual void Delete()
        {
            foreach(UltraTreeNode dn in Nodes)
            {
                if(dn is IDeleteNode)
                    ((IDeleteNode) dn).Delete();
            }

            if(DataRow != null)
            {
                DataRow.CancelEdit();
                DataRow.Delete();
                DataRow = null;
            }

            Remove();

            NLog.LogManager.GetCurrentClassLogger().Info($"Deleted {Text}");
        }

        /// <summary>
        ///     Gets a value indicating whether this node can be deleted.
        /// </summary>
        /// <value> <c>true</c> if this instance can delete; otherwise, <c>false</c> . </value>
        public virtual bool CanDelete
        {
            get { return true; }
        }

        #endregion
    }

    public static class NodeExtensions
    {
        public static List <K> FindNodes<K>(this UltraTreeNode node, Predicate <K> p) where K : UltraTreeNode
        {
            var list = new List <K>();

            if(node != null)
            {
                foreach(UltraTreeNode n in node.Nodes)
                {
                    if(n is K && p((K) n))
                        list.Add((K) n);

                    if(n.Nodes.Count > 0)
                        list.AddRange(FindNodes(n, p));
                }
            }

            return list;
        }

        public static List<K> FindNodes<K>(this UltraTreeNode node) where K: UltraTreeNode
        {
            var list = new List<K>();

            if (node != null)
            {
                foreach (UltraTreeNode n in node.Nodes)
                {
                    if (n is K)
                    {
                        list.Add((K)n);
                    }

                    if (n.Nodes.Count > 0)
                        list.AddRange(FindNodes<K>(n));
                }
            }

            return list;
        }

        public static K FindNode<K>(this UltraTreeNode node, Predicate <K> p) where K : UltraTreeNode
        {
            if(node != null)
            {
                foreach(UltraTreeNode n in node.Nodes)
                {
                    if(n is K && p((K) n))
                        return (K) n;

                    K t = FindNode(n, p);
                    if(t != null)
                        return t;
                }
            }

            return null;
        }
    }

    public class DefaultNodeSorter : IComparer
    {
        #region IComparer Members

        public int Compare(object x, object y)
        {
            if(x is UltraTreeNode && y is UltraTreeNode)
            {
                string sortX = x is ISortable ? ((ISortable) x).SortKey : ((UltraTreeNode) x).Text;
                string sortY = y is ISortable ? ((ISortable) y).SortKey : ((UltraTreeNode) y).Text;

                if(sortX == null && sortY == null)
                    return 0;
                if(sortX == null)
                    return -1;
                if(sortY == null)
                    return 1;
                return sortX.CompareTo(sortY);
            }

            return 0;
        }

        #endregion
    }

    internal class ReportSorter : IComparer<IReportNode>
    {
        public int Compare(IReportNode x, IReportNode y)
        {
            var sortX = x?.SortKey;
            var sortY = y?.SortKey;

            if(sortX == null && sortY == null)
                return 0;
            if(sortX == null)
                return -1;
            if(sortY == null)
                return 1;
            return sortX.CompareTo(sortY);
        }
    }

    public class DataRowProxyResults
    {
        public DataRow Row { get; set; }
        public DataRowProxy Proxy { get; set; }
    }
}