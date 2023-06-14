using ByteSizeLib;

namespace DWOS.DataArchiver.Model
{
    public class Media
    {
        #region Properties

        public int MediaId { get; set; }

        public string Name { get; set; }

        public string FileExtension { get; set; }

        public ByteSize Size { get; set; }

        #endregion
    }
}
