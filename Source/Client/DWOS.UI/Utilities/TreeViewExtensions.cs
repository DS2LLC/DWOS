using System;
using System.Collections.Generic;
using System.Linq;
using Infragistics.Win.UltraWinTree;
using NLog;

namespace DWOS.UI.Utilities
{
    public static class TreeViewExtensions
    {
        /// <summary>
        /// Performs an action on each child.
        /// </summary>
        /// <param name="tree"> The tree. </param>
        /// <param name="performAction"> The perform action. </param>
        public static void ForEachNode(this UltraTree tree, Action <UltraTreeNode> performAction) { tree.Nodes.ForEachNode(performAction); }

        /// <summary>
        /// Performs an action on each child.
        /// </summary>
        /// <param name="items"> The items. </param>
        /// <param name="performAction"> The perform action. </param>
        public static void ForEachNode(this TreeNodesCollection items, Action <UltraTreeNode> performAction)
        {
            if(items != null)
            {
                foreach(UltraTreeNode item in items)
                {
                    UltraTreeNode tvi = item;

                    if(tvi != null)
                    {
                        tvi.Nodes.ForEachNode(performAction);
                        performAction(tvi);
                    }
                }
            }
        }

        /// <summary>
        /// Finds the node based on the predicate.
        /// </summary>
        /// <param name="tree"> The tree. </param>
        /// <param name="where"> The where. </param>
        /// <returns> </returns>
        public static UltraTreeNode FindNode(this UltraTree tree, Predicate <UltraTreeNode> where) { return tree.Nodes.FindNode(where); }

        /// <summary>
        /// Finds the node based on the predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodes"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static T FindNode<T>(this TreeNodesCollection nodes, Predicate <T> where) where T : UltraTreeNode
        {
            if(nodes != null)
            {
                foreach(UltraTreeNode node in nodes)
                {
                    if(node != null)
                    {
                        if(node is T && where((T) node))
                            return (T) node;

                        T found = node.Nodes.FindNode(where);

                        if(found != null)
                            return found;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the node based on the predicate.
        /// </summary>
        /// <param name="nodes"> The items. </param>
        /// <param name="where"> The where. </param>
        /// <returns> </returns>
        public static UltraTreeNode FindNode(this TreeNodesCollection nodes, Predicate <UltraTreeNode> where)
        {
            if(nodes != null)
            {
                foreach(UltraTreeNode node in nodes)
                {
                    if(node != null)
                    {
                        if(where(node))
                            return node;

                        UltraTreeNode found = node.Nodes.FindNode(where);

                        if(found != null)
                            return found;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the node using a search algorithm that searches
        /// <paramref name="nodes"/> first and then searches every child node
        /// depth-first.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static UltraTreeNode FindNodeBFS(this TreeNodesCollection nodes, Predicate <UltraTreeNode> where)
        {
            if(nodes != null)
            {
                //search siblings first
                foreach(UltraTreeNode node in nodes)
                {
                    if(where(node))
                        return node;
                }

                //then search children
                foreach(UltraTreeNode node in nodes)
                {
                    UltraTreeNode found = node.Nodes.FindNode(where);

                    if(found != null)
                        return found;
                }
            }

            return null;
        }

        public static IEnumerable<T> NodesOfType<T>(this UltraTree tree) where T : UltraTreeNode
        {
            if (tree == null)
            {
                throw new ArgumentNullException(nameof(tree));
            }

            var matches = new List<T>();
            foreach (var node in tree.Nodes)
            {
                var typedNode = node as T;
                if (typedNode != null)
                {
                    matches.Add(typedNode);
                }

                var innerMatches = NodesOfType<T>(node.Nodes);
                matches.AddRange(innerMatches);
            }

            return matches;
        }

        private static List<T> NodesOfType<T>(TreeNodesCollection nodes) where T : UltraTreeNode
        {
            if (nodes == null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            var matches = new List<T>();
            foreach (var node in nodes)
            {
                var typedNode = node as T;
                if (typedNode != null)
                {
                    matches.Add(typedNode);
                }

                var innerMatches = NodesOfType<T>(node.Nodes);
                matches.AddRange(innerMatches);
            }

            return matches;
        }

        /// <summary>
        /// Filters the tree visibility based on the predicate. Will stop at each level once visibility is set to false.
        /// </summary>
        /// <param name="tree"> The tree. </param>
        /// <param name="hideNode"> The hide node. </param>
        public static void FilterTree(this UltraTree tree, Predicate <UltraTreeNode> hideNode)
        {
            tree.Nodes.FilterTree(hideNode);

            //if currently selected item is no longer visible then unselect it
            if(tree.SelectedNodes.Count > 0)
                tree.PerformAction(UltraTreeAction.ClearAllSelectedNodes, false, false);
        }

        /// <summary>
        /// Filters the tree visibility based on the predicate. Will stop at each level once visibility is set to false.
        /// </summary>
        /// <param name="items"> The items. </param>
        /// <param name="hideNode"> The hide node. </param>
        public static void FilterTree(this TreeNodesCollection items, Predicate <UltraTreeNode> hideNode)
        {
            foreach(UltraTreeNode item in items)
            {
                UltraTreeNode tviChild = item;

                if(tviChild != null)
                {
                    bool isVisible = !hideNode(tviChild);

                    tviChild.Visible = isVisible;

                    //if still visible check children
                    if(isVisible)
                        tviChild.Nodes.FilterTree(hideNode);
                }
            }
        }

        /// <summary>
        /// Selects the specified node in the TOC, safe for trees that have multi select selection type.
        /// </summary>
        /// <param name="node"> The node. </param>
        /// <param name="bringIntoView"> if set to <c>true</c> [bring into view]. </param>
        /// <returns> </returns>
        public static UltraTreeNode Select(this UltraTreeNode node, bool bringIntoView = true)
        {
            try
            {
                if(node != null && node.Control != null)
                {
                    node.Control.SelectedNodes.Clear();

                    //default method of selecting
                    node.Selected = true;

                    if(!node.Selected)
                        node.Control.SelectedNodes.AddRange(new[] {node}, true);

                    node.Control.ActiveNode = node;

                    if(bringIntoView)
                        node.BringIntoView();
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error selecting node.";
                LogManager.GetCurrentClassLogger().Warn(exc, errorMsg);
            }

            return node;
        }

        /// <summary>
        /// Disposes all the nodes and there children.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="clearSelectionFirst">The clear selection first.</param>
        public static void DisposeAll(this TreeNodesCollection nodes, bool clearSelectionFirst = true)
        {
            try
            {
                if(nodes == null || nodes.Control == null)
                    return;

                //clear handles before disposing
                if(clearSelectionFirst)
                {
                    try
                    {
                        nodes.Control.SelectedNodes.Clear();
                    }
                    catch (Exception exc)
                    {
                        //NOTE: The statement 'nodes.Control.ActiveNode = null' will sometimes throw a weird error about input string not in correct format, can't determine why
                        LogManager.GetCurrentClassLogger().Warn(exc, "Error clearing nodes");
                    }
                }

                //dispose children
                foreach(UltraTreeNode childNode in nodes)
                    DisposeNodesInt(childNode);

                nodes.Clear(); //remove all children
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error disposing of all nodes.");
            }
        }

        /// <summary>
        /// Disposes all nodes in the tree - disposes leaf nodes first.
        /// </summary>
        /// <param name="node"></param>
        private static void DisposeNodesInt(this UltraTreeNode node)
        {
            //dispose children
            foreach(UltraTreeNode childNode in node.Nodes)
                DisposeNodesInt(childNode);

            node.Dispose(); //dispose of this node after all children were disposed.
        }

        /// <summary>
        /// Gets the first selected node.
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="tree"> The tree. </param>
        /// <returns> </returns>
        public static T SelectedNode<T>(this UltraTree tree) where T : UltraTreeNode { return tree.SelectedNodes.Count > 0 ? tree.SelectedNodes[0] as T : null; }

        public static List <T> SelectedNodesOfType<T>(this UltraTree tree) where T : UltraTreeNode
        {
            if(tree.SelectedNodes.Count > 0)
                return tree.SelectedNodes.OfType <T>().ToList();

            return new List <T>();
        }

        /// <summary>
        /// Gets the next sibling of the specified type T, excluding invisible nodes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node">The node.</param>
        /// <returns>T.</returns>
        public static T GetNextVisibleSibling<T>(this T node) where T : UltraTreeNode
        {
            if (node == null)
                return null;

            var parentNode = node.ParentNodesCollection;

            //if no parent or at end of list
            if (parentNode == null || node.Index >= parentNode.Count - 1)
                return null;

            for (var i = node.Index + 1; i < parentNode.Count; i++)
            {
                if (parentNode[i].Visible && parentNode[i] is T)
                    return parentNode[i] as T;
            }

            return null;
        }
    }
}