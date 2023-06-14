using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Services.Messages;
using NLog;
using System;
using System.Collections.Generic;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using System.Web.Http;

namespace DWOS.Services
{
    public class OrderNotesController : ApiController
    {
        #region Methods

        [HttpGet]
        [ServiceExceptionFilter("Error getting order notes.")]
        public ResponseBase Get(int orderId)
        {
            return new OrderNotesResponse { Success = true, ErrorMessage = null, Notes = CreateNotes(orderId) };
        }

        [HttpPost]
        [DwosAuthenticate]
        [DwosAuthorize(Roles ="AddOrderNote,OrderEntry.Edit")]
        [ServiceExceptionFilter("Error saving order note.")]
        public SaveOrderNotesResponse Save(SaveOrderNotesRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return Save(request.Note, request.UserId);
        }

        #endregion

        #region Factories

        private static List<OrderNote> CreateNotes(int orderId)
        {
            using (var dtOrderNote = new OrdersDataSet.OrderNoteDataTable())
            {
                using (var taOrderNote = new OrderNoteTableAdapter())
                {
                    taOrderNote.FillByOrder(dtOrderNote, orderId);
                }

                return dtOrderNote
                    .OrderBy(noteRow => noteRow.OrderNoteID)
                    .Select(AsOrderNote)
                    .ToList();
            }
        }

        private static OrderNote AsOrderNote(OrdersDataSet.OrderNoteRow row)
        {
            if (row == null)
            {
                return null;
            }

            return new OrderNote
            {
                OrderId = row.OrderID,
                OrderNoteId = row.OrderNoteID,
                NoteType = row.NoteType,
                Note = row.IsNotesNull() ? string.Empty : row.Notes
            };
        }

        private static SaveOrderNotesResponse Save(OrderNote note, int userId)
        {
            using (var dtOrderNote = new OrdersDataSet.OrderNoteDataTable())
            {
                using (var taOrderNote = new OrderNoteTableAdapter())
                {
                    taOrderNote.FillByOrder(dtOrderNote, note.OrderId);

                    var existingNoteRow = dtOrderNote.FindByOrderNoteID(note.OrderNoteId);

                    if (existingNoteRow == null)
                    {
                        // Create new
                        var newNoteRow = dtOrderNote.NewOrderNoteRow();
                        newNoteRow.UserID = userId;
                        newNoteRow.OrderID = note.OrderId;
                        newNoteRow.Notes = note.Note;
                        newNoteRow.NoteType = note.NoteType;
                        newNoteRow.TimeStamp = DateTime.Now;
                        dtOrderNote.AddOrderNoteRow(newNoteRow);
                    }
                    else
                    {
                        // Update existing
                        existingNoteRow.Notes = note.Note;
                        existingNoteRow.NoteType = note.NoteType;
                    }

                    taOrderNote.Update(dtOrderNote);
                }
            }

            return new SaveOrderNotesResponse { Success = true };
        }

        #endregion
    }
}