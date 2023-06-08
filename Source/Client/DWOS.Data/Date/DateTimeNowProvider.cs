using System;

namespace DWOS.Data.Date
{
    /// <summary>
    /// Implementation of <see cref="IDateTimeNowProvider"/> that uses
    /// <see cref="DateTime.Now"/>.
    /// </summary>
    public class DateTimeNowProvider : IDateTimeNowProvider
    {
        public DateTime Now => DateTime.Now;
    }
}
