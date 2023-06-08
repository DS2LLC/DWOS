using System;

namespace DWOS.UI.Utilities
{
    public class SavedDataEventArgs : EventArgs
    {
        #region Properties

        public int Id { get; private set; }

        #endregion

        #region Methods

        public SavedDataEventArgs(int id)
        {
            Id = id;
        }

        #endregion
    }
}
