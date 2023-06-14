using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using DWOS.Services.Messages;

namespace DWOS.Android.Adapters
{
    /// <summary>
    /// Adapter that returns views for <see cref="DocumentInfo"/> and
    /// <see cref="MediaSummary"/> instances.
    /// </summary>
    /// <remarks>
    /// <see cref="DocumentListAdapter"/> uses an inner class
    /// (<see cref="DocumentListItem"/>) to show both types of files to the
    /// user in the same control.
    /// </remarks>
    public class DocumentListAdapter : BaseAdapter<DocumentListAdapter.DocumentListItem>
    {
        #region Fields

        Activity _context;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of documents for this instance.
        /// </summary>
        public IList<DocumentInfo> Documents
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of media for this instance.
        /// </summary>
        public IList<MediaSummary> Media
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of document/media items for this instance.
        /// </summary>
        public IList<DocumentListItem> Items
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating if this instance should include
        /// document type in display names.
        /// </summary>
        public bool IncludeDocumentType
        {
            get;
            set;
        }

        public override int Count
        {
            get
            {
                return Items.Count;
            }
        }

        public override DocumentListItem this[int position]
        {
            get
            {
                return Items[position];
            }
        }

        #endregion

        #region Methods

        public DocumentListAdapter(Activity activity, IList<DocumentInfo> documents, IList<MediaSummary> media)
        {
            _context = activity;
            Documents = documents ?? Enumerable.Empty<DocumentInfo>().ToList();
            Media = media ?? Enumerable.Empty<MediaSummary>().ToList();

            Items = new List<DocumentListItem>();

            var itemCount = Media.Count + Documents.Count;

            string firstItemText;
            if (itemCount == 0)
            {
                firstItemText = "No Documents";
            }
            else if (itemCount == 1)
            {
                firstItemText = "1 Document";
            }
            else
            {
                firstItemText = string.Format("{0} Documents",
                    itemCount);
            }

            Items.Add(DocumentListItem.FromText(firstItemText));

            foreach (var mediaItem in Media)
            {
                Items.Add(DocumentListItem.FromMedia(mediaItem));
            }
            foreach (var documentItem in Documents)
            {
                Items.Add(DocumentListItem.FromDocument(documentItem));
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (position > Items.Count)
                return null;

            var view = convertView;

            if (view == null)
            {
                view = _context.LayoutInflater.Inflate(Resource.Layout.DocumentListItemLayout, null);
            }

            var document = Items[position];
            string itemName;
            if (IncludeDocumentType && !string.IsNullOrEmpty(document.DocumentType))
            {
                itemName = string.Format("{0} ({1})",
                    document.Name,
                    document.DocumentType);
            }
            else
            {
                itemName = document.Name;
            }

            view.FindViewById<TextView>(Resource.Id.textViewDocumentName).Text = itemName;
            return view;
        }

        #endregion

        #region DocumentListItem

        /// <summary>
        /// May contains either a <see cref="DocumentInfo"/> instance or a
        /// <see cref="MediaSummary"/> instance.
        /// </summary>
        public class DocumentListItem
        {
            #region Properties

            /// <summary>
            /// Gets the document for this instance.
            /// </summary>
            public DocumentInfo Document { get; private set; }

            /// <summary>
            /// Gets the media for this instance.
            /// </summary>
            public MediaSummary Media { get; private set; }

            /// <summary>
            /// Gets the display name of this instance.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets the document type for this instance.
            /// </summary>
            public string DocumentType { get; set; }

            #endregion

            #region Methods

            /// <summary>
            /// Returns a new instance of <see cref="DocumentListItem"/>
            /// for a document.
            /// </summary>
            /// <param name="document"></param>
            /// <returns></returns>
            public static DocumentListItem FromDocument(DocumentInfo document)
            {
                if (document == null)
                {
                    return null;
                }

                return new DocumentListItem()
                {
                    Document = document,
                    Name = document.Name,
                    DocumentType = GetDocumentType(document.DocumentType)
                };
            }

            /// <summary>
            /// Returns a new instance of <see cref="DocumentListItem"/>
            /// for media.
            /// </summary>
            /// <param name="media"></param>
            /// <returns></returns>
            public static DocumentListItem FromMedia(MediaSummary media)
            {
                if (media == null)
                {
                    return null;
                }

                return new DocumentListItem()
                {
                    Media = media,
                    Name = string.Format("{0}.{1}", media.Name, media.FileExtension)
                };
            }

            /// <summary>
            /// Returns a new instance of <see cref="DocumentListItem"/>
            /// for display text.
            /// </summary>
            /// <remarks>
            /// This is intended for use as the first, placeholder item in
            /// a spinner.
            /// </remarks>
            /// <param name="text"></param>
            /// <returns></returns>
            public static DocumentListItem FromText(string text)
            {
                return new DocumentListItem()
                {
                    Name = text
                };
            }

            private static string GetDocumentType(string documentType)
            {
                if (documentType == "ProcessAlias")
                {
                    return "Alias";
                }
                else
                {
                    return documentType;
                }
            }

            #endregion
        }

        #endregion
    }
}