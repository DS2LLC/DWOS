using System.Collections.Generic;

using Android.Content;
using Android.Views;
using Android.Widget;
using DWOS.Services.Messages;

namespace DWOS.Android
{
    public class OrderNotesAdapter : BaseAdapter<OrderNote>
    {
        #region Fields

        Context _context;

        #endregion

        #region Properties

        public List<OrderNote> Notes { get; }

        public override int Count => Notes.Count;

        public override OrderNote this[int position] => Notes[position];


        #endregion

        #region Methods

        public OrderNotesAdapter(Context context, List<OrderNote> notes)
        {
            _context = context;
            Notes = notes;
        }

        public override long GetItemId(int position) => Notes[position].OrderNoteId;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (position > Notes.Count)
                return null;

            var note = Notes[position];
            var view = convertView;

            OrderNotesAdapterViewHolder viewHolder;
            if (view == null)
            {
                var inflater = _context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
                view = inflater.Inflate(Resource.Layout.OrderNotesItemLayout, null);
                viewHolder = new OrderNotesAdapterViewHolder
                {
                    TextViewNotes = view.FindViewById<TextView>(Resource.Id.textViewNote),
                    TextViewType = view.FindViewById<TextView>(Resource.Id.textViewNoteType)
                };

                view.Tag = viewHolder;
            }
            else
            {
                viewHolder = view.Tag as OrderNotesAdapterViewHolder;
            }

            if (viewHolder != null)
            {
                viewHolder.TextViewNotes.Text = note?.Note;
                viewHolder.TextViewType.Text = note?.NoteType;
            }

            return view;
        }

        #endregion

        #region OrderNotesAdapterViewHolder

        private class OrderNotesAdapterViewHolder : Java.Lang.Object
        {
            public TextView TextViewNotes { get; set; }

            public TextView TextViewType { get; set; }
        }

        #endregion
    }
}