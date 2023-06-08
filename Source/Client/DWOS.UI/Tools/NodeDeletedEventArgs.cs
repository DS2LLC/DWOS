using DWOS.UI.Utilities;
using System;
namespace DWOS.UI.Tools
{
    public class NodeDeletedEventArgs : EventArgs
    {
        public IDeleteNode Node { get; set; }
    }

}
