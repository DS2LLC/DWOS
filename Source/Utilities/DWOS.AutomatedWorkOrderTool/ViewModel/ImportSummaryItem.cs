using System;

namespace DWOS.AutomatedWorkOrderTool.ViewModel
{
    public class ImportSummaryItem
    {
        #region Properties

        public ItemType Type { get; }

        public string Message { get; }

        public DateTime Time { get; }

        #endregion

        #region Methods

        public ImportSummaryItem(ItemType type, string message)
            : this(type, message, DateTime.Now)
        {

        }

        public ImportSummaryItem(ItemType type, string message, DateTime time)
        {
            Type = type;
            Message = message;
            Time = time;
        }

        #endregion

        #region ItemType

        public enum ItemType
        {
            Info,
            Warning,
            Error
        }

        #endregion
    }
}
