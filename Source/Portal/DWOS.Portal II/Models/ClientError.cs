using System.Collections.Generic;

namespace DWOS.Portal.Models
{
    /// <summary>
    /// Represents a client-side error.
    /// </summary>
    public class ClientError
    {
        public List<string> StackTrace { get; set; }

        public string UserAgent { get; set; }
    }
}