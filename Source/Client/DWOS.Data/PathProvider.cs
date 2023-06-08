using DWOS.Shared.Utilities;

namespace DWOS.Data
{
    /// <summary>
    /// Implementation of <see cref="IPathProvider"/> that uses the
    /// <see cref="FileSystem"/> class.
    /// </summary>
    public sealed class PathProvider : IPathProvider
    {
        #region IPathProvider Members

        public string ImageDirectory
        {
            get
            {
                //Change to common data path for web to hopefully prevent permission issues

                return FileSystem.IsWebApplication() ?
                    FileSystem.CommonAppDataPathVersion() :
                    FileSystem.UserAppDataPathVersion();
            }
        }

        #endregion
    }
}
