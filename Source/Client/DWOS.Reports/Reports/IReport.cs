using System;
using System.Threading;

namespace DWOS.Reports
{
    public interface IReport : IDisposable
    {
        void DisplayReport(CancellationToken cancellationToken);
    }
}