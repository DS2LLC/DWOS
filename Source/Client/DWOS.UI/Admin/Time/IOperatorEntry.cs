namespace DWOS.UI.Admin.Time
{
    public interface IOperatorEntry
    {
        string Id { get; }

        int UserId { get; }

        string UserName { get; }

        int DurationMinutes { get; }

        bool HasActiveTimer
        {
            get;
        }

        void StartLaborTimer();

        void PauseLaborTimer();

        void StopLaborTimer();

        /// <summary>
        /// Moves the WO/batch associated with this instance to
        /// another issue.
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// newUserId is the same as <see cref="UserId"/>.
        /// </exception>
        /// <param name="newUserId"></param>
        void MoveToUser(int newUserId);
    }
}
