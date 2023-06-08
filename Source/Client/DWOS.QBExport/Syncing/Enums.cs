namespace DWOS.QBExport.Syncing
{
    /// <summary>
    /// Represents a source for a sync.
    /// </summary>
    public enum SyncSource
    {
        DWOS = 0,
        QuickBooks = 1
    }

    /// <summary>
    /// Represents the type of message of a sync result.
    /// </summary>
    public enum MessageType
    {
        Error = 0,
        Warning = 1,
        Success = 2,
        Normal = 3
    }
}
