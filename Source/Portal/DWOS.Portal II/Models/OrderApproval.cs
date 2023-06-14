using System;
using System.Collections.Generic;

namespace DWOS.Portal.Models
{
    public class OrderApproval
    {
        public int OrderApprovalId { get; set; }

        public int OrderId { get; set; }

        public string Status { get; set; }

        public FileData PrimaryMedia { get; set; }

        public List<string> MediaUrls { get; set; }

        public string Terms { get; set; }

        public string Notes { get; set; }

        public DateTime DateCreated { get; set; }

        public int? ContactId { get; set; }

        public string ContactNotes { get; set; }
    }
}