namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// Represents the method that will handle an event that uses a single
    /// item for event data.
    /// </summary>
    /// <typeparam name="T">Type of the sender</typeparam>
    /// <typeparam name="K">Data type</typeparam>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void EventHandlerTemplate<T, K>(T sender, EventArgsTemplate <K> e);
}
