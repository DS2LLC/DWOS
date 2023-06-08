using System.Collections.Generic;

namespace DWOS.UI.Admin.RevisePartProcess
{
    public class RevisionResults<T>
    {
        #region Properties

        public List<T> Success { get; set; }

        public List<T> Failure { get; set; }

        #endregion
    }
}
