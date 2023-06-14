using System.Collections.Generic;

namespace DWOS.Services.Messages
{
    /// <summary>
    /// Server response to a request for order notes.
    /// </summary>
    public class OrderNotesResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the notes for this instance.
        /// </summary>
        public List<OrderNote> Notes { get; set; }
    }

    /// <summary>
    /// Represents a single note that belongs to an order.
    /// </summary>
    public class OrderNote
    {
        /// <summary>
        /// External note type.
        /// </summary>
        public const string External = "External";

        /// <summary>
        /// Internal note type.
        /// </summary>
        public const string Internal = "Internal";

        /// <summary>
        /// Gets or sets the order ID for this instance.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the order note ID for this instance.
        /// </summary>
        public int OrderNoteId { get; set; }

        /// <summary>
        /// Gets or sets the content for this instance.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the note type for this instance.
        /// </summary>
        public string NoteType { get; set; }
    }

    /// <summary>
    /// Client-side request to save a note.
    /// </summary>
    /// <remarks>
    /// Can represent a request to add a new note or edit an existing note.
    /// </remarks>
    public class SaveOrderNotesRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the note for this instance.
        /// </summary>
        public OrderNote Note { get; set; }
    }

    /// <summary>
    /// Server-side response to a request for saving a note.
    /// </summary>
    public class SaveOrderNotesResponse : ResponseBase
    {
    }
}
