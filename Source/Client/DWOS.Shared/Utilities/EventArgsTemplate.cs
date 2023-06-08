using System;

namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// Provides data of a specific type for an event.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventArgsTemplate<T> : EventArgs
    {
        /// <summary>
        /// Gets or sets the item for this instance.
        /// </summary>
        public T Item { get; set; }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="EventArgsTemplate{T}"/> class.
        /// </summary>
        /// <param name="item"></param>
        public EventArgsTemplate(T item) { this.Item = item; }
    }


}
