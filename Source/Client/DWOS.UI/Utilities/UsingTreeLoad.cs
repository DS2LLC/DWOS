using System;
using System.Collections;
using Infragistics.Win.UltraWinTree;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Utility class that prepares a <see cref="UltraTree"/> instance for
    /// loading data.
    /// </summary>
    public class UsingTreeLoad : IDisposable
    {
        #region Fields

        private IComparer _comp;
        private SortType _sortType;
        private UltraTree _tree;

        #endregion

        #region Methods

        public UsingTreeLoad(UltraTree tree)
        {
            _tree = tree;
            Begin();
        }

        private void Begin()
        {
            if (_tree == null)
            {
                return;
            }

            _tree.BeginUpdate();

            _sortType = _tree.Override.Sort;
            _tree.Override.Sort = SortType.None;

            _comp = _tree.Override.SortComparer;
            _tree.Override.SortComparer = null;
        }

        private void End()
        {
            if (_tree == null)
            {
                return;
            }

            _tree.Override.Sort = _sortType;
            _tree.Override.SortComparer = _comp;
            _tree.EndUpdate();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            End();

            _tree = null;
            _comp = null;
        }

        #endregion
    }
}
