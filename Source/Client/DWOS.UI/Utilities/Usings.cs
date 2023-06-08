using System;
using System.Collections;
using System.Data;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinListView;

namespace DWOS.UI.Utilities
{

    public class UsingListViewLoad : IDisposable
    {
        #region Fields

        private IComparer _comp;
        private UltraListView _listView;
        private Sorting _sorting;

        #endregion

        #region Methods

        public UsingListViewLoad(UltraListView listView)
        {
            this._listView = listView;
            Begin();
        }

        private void Begin()
        {
            this._listView.BeginUpdate();

            this._sorting = this._listView.MainColumn.Sorting;
            this._comp = this._listView.MainColumn.SortComparer;
            this._listView.MainColumn.SortComparer = null;
            this._listView.MainColumn.Sorting = Sorting.None;
        }

        private void End()
        {
            this._listView.MainColumn.Sorting = this._sorting;
            this._listView.MainColumn.SortComparer = this._comp;

            this._listView.EndUpdate();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            End();

            this._listView = null;
            this._comp = null;
        }

        #endregion
    }

    public class UsingGridLoad : IDisposable
    {
        #region Fields

        private UltraGrid _grid;

        #endregion

        #region Methods

        public UsingGridLoad(UltraGrid grid)
        {
            this._grid = grid;
            Begin();
        }

        private void Begin()
        {
            this._grid.BeginUpdate();
            this._grid.SuspendRowSynchronization();
        }

        private void End()
        {
            this._grid.ResumeRowSynchronization();
            this._grid.EndUpdate();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            End();
            this._grid = null;
        }

        #endregion
    }

    /// <summary>
    ///     Manages the BeginLoadData and EndLoadData methods
    /// </summary>
    public class UsingDataTableLoad : IDisposable
    {
        #region Fields

        private DataTable _table;

        #endregion

        #region Methods

        public UsingDataTableLoad(DataTable grid)
        {
            this._table = grid;
            Begin();
        }

        private void Begin() { this._table.BeginLoadData(); }

        private void End() { this._table.EndLoadData(); }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            End();
            this._table = null;
        }

        #endregion
    }
}
