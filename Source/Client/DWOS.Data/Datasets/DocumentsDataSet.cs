using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using NLog;

namespace DWOS.Data.Datasets
{


    public partial class DocumentsDataSet
    {
        partial class DocumentInfoDataTable
        {
        }
    }
}

namespace DWOS.Data.Datasets.DocumentsDataSetTableAdapters 
{
 

    partial class DocumentRevisionTableAdapter
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static bool? _useFileStream;
        
        #endregion

        #region Properties

        private bool? UseFileStream
        {
            get
            {
                //if using integrated security and not a SQL LocalDb then use FileStream
                if (!_useFileStream.HasValue)
                    _useFileStream = this.Connection.ConnectionString.Contains(false, "Integrated Security=True") && !this.Connection.ConnectionString.Contains(false, "localdb");

                return _useFileStream.GetValueOrDefault();
            }
        }
        
        #endregion

        #region Methods
        
        public byte[] GetMediaStream(int documentRevisionID)
        {
            if (this.Connection.State != System.Data.ConnectionState.Open)
                this.Connection.Open();

            //if using integrated security, this is the only way we can use FileStream
            if (UseFileStream.GetValueOrDefault())
            {
                string path = this.GetDocumentServerFilePath(documentRevisionID).ToString();
                _log.Info("Getting document from " + path);

                using (var transaction = this.Connection.BeginTransaction("DocumentTrans"))
                {
                    using (var cmd = new SqlCommand { Connection = this.Connection, Transaction = transaction, CommandText = "SELECT GET_FILESTREAM_TRANSACTION_CONTEXT()" })
                    {
                        var txContext = cmd.ExecuteScalar() as byte[];

                        using (var fs = new SqlFileStream(path, txContext, FileAccess.Read))
                        {
                            var buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, buffer.Length);
                            fs.Close();
                            return buffer;
                        }
                    }
                }
            }
            else //else have to use normal method to access media; Drawback is it consumes memory on server side
            {
                return this.GetDocumentData(documentRevisionID);
            }
        }
        
        #endregion
    }
}
