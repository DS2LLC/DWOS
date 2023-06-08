using System;
using System.Data;

namespace DWOS.Data
{
    /// <summary>
    /// Automatically sets up a <see cref="DataSet"/> instance for loading data.
    /// </summary>
    public sealed class UsingDataSetLoad : IDisposable
    {
        #region Fields

        private DataSet _dataset;
        private bool _enforceConstraints;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="UsingDataSetLoad"/> test.
        /// </summary>
        /// <param name="dataset"></param>
        public UsingDataSetLoad(DataSet dataset)
        {
            _dataset = dataset;
            Begin();
        }

        private void Begin()
        {
            _enforceConstraints = _dataset.EnforceConstraints;
            _dataset.EnforceConstraints = false;
            _dataset.BeginInit();

            foreach (DataTable table in _dataset.Tables)
                table.BeginLoadData();
        }

        private void End()
        {
            foreach (DataTable table in _dataset.Tables)
                table.EndLoadData();

            _dataset.EndInit();
            _dataset.EnforceConstraints = _enforceConstraints;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            End();
            _dataset = null;
        }

        #endregion
    }
}
