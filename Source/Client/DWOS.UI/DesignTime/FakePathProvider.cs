using DWOS.Data;

namespace DWOS.UI.DesignTime
{
    internal class FakePathProvider : IPathProvider
    {
        #region IPathProvider Members

        public string ImageDirectory => "Images";

        #endregion
    }
}