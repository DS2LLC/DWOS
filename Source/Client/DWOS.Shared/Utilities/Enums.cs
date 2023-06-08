using System;

namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// Option for the <see cref="DwosStringExtensions.ToInitials(string, StringInitialOption)"/> method.
    /// </summary>
    public enum StringInitialOption
    {
        FirstTwoInitials,
        AllInitials
    }

    /// <summary>
    /// Represents a level of notification.
    /// </summary>
    public enum NotificationLevel
    {
        Info,
        Warning,
        Error
    }
}