using System.Data;
using DWOS.Reports;
using Infragistics.Win.UltraWinTree;

namespace DWOS.UI.Utilities
{
    public interface IDataNode
    {
        DataRow DataRow { get; }
        bool HasChanges { get; }
        bool IsRowValid { get; }
        void UpdateNodeUI();
    }

    public interface ICopyPasteNode
    {
        /// <summary>
        ///     Gets the clipboard data format that this instance represents.
        /// </summary>
        /// <value> The clipboard data format. </value>
        string ClipboardDataFormat { get; }

        /// <summary>
        ///     Pastes the data based on the format type as a new child node.
        /// </summary>
        /// <param name="format"> The format. </param>
        /// <param name="dr"> The dr. </param>
        UltraTreeNode PasteData(string format, DataRowProxy dr);

        /// <summary>
        ///     Determines whether this instance can paste data of the specified format.
        /// </summary>
        /// <param name="format"> The format. </param>
        /// <returns> <c>true</c> if this instance [can paste data] the specified format; otherwise, <c>false</c> . </returns>
        bool CanPasteData(string format);
    }

    public interface IDeleteNode
    {
        /// <summary>
        ///     Gets a value indicating whether this node can be deleted.
        /// </summary>
        /// <value> <c>true</c> if this instance can delete; otherwise, <c>false</c> . </value>
        bool CanDelete { get; }

        /// <summary>
        ///     Deletes this instance.
        /// </summary>
        void Delete();
    }

    /// <summary>
    /// Interface for node classes that have reports.
    /// </summary>
    /// <remarks>
    /// This interface extends <see cref="ISortable"/> because DWOS may need
    /// to show/print reports in a specific order.
    /// </remarks>
    internal interface IReportNode : ISortable
    {
        /// <summary>
        ///     Creates the report for this node.
        /// </summary>
        /// <returns> </returns>
        IReport CreateReport(string reportType);

        string[] ReportTypes();
    }

    public interface ISortable
    {
        string SortKey { get; }
    }

    public interface IActive
    {
        /// <summary>
        ///     Get or set a value indicating whether this instance is active.
        /// </summary>
        /// <value> <c>true</c> if this instance is active; otherwise, <c>false</c> . </value>
        bool IsActiveData { get; set; }
    }
}