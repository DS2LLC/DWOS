using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Interface declaration for an Error provider
    /// </summary>
    public interface IErrorService
    {
        void LogError(string message, Exception exception = default(Exception), object context = default(object), bool toast = default(bool));
    }
}
