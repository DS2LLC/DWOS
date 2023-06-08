using System.Threading;

namespace DWOS.Reports
{
    /// <summary>
    /// Wraps around another report.
    /// </summary>
    /// <remarks>
    /// Some reports require passing a <see cref="System.Data.DataRow"/> as a
    /// constructor parameter. This abstract class provides a framework to use
    /// these reports with just an ID and encapsulate the logic needed to
    /// retrieve and dispose of necessary data.
    /// </remarks>
    public abstract class ReportWrapper : IReport
    {
        #region Properties

        public abstract IReport InnerReport { get; }

        #endregion

        #region IReport Members

        public void DisplayReport(CancellationToken cancellationToken)
        {
            InnerReport.DisplayReport(cancellationToken);
        }

        public abstract void Dispose();

        #endregion
    }
}
