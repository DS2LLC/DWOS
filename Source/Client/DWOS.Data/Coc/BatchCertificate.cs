using DWOS.Shared.Utilities;
using System;
using System.Collections.Generic;

namespace DWOS.Data.Coc
{
    public class BatchCertificate
    {
        #region Properties

        public int BatchCocId { get; set; }

        public ISecurityUserInfo QualityInspector { get; set; }

        public CertificateBatch Batch { get; set; }

        public DateTime DateCertified { get; set; }

        public string InfoHtml { get; set; }

        public List<CertificateBatchOrder> Orders { get; set; }

        #endregion
    }
}
