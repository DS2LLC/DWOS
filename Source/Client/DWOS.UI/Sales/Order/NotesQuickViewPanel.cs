using System;
using System.ComponentModel;
using System.Linq;
using DWOS.Data.Datasets;

namespace DWOS.UI.Sales.Order
{
    public partial class NotesQuickViewPanel : DataPanel
    {
        #region Fields

        private readonly BindingList<NoteInfo> _notes =
            new BindingList<NoteInfo>();

        #endregion

        #region Properties

        public OrdersDataSet Dataset
        {
            get { return _dataset as OrdersDataSet; }
            set { _dataset = value; }
        }

        protected override string BindingSourcePrimaryKey => Dataset.Order.OrderIDColumn.ColumnName;

        #endregion

        #region Methods

        public NotesQuickViewPanel()
        {
            InitializeComponent();
        }

        public void LoadData(OrdersDataSet dsOrders)
        {
            _dataset = dsOrders;
            bsData.DataSource = dsOrders;
            bsData.DataMember = dsOrders.Order.TableName;
            _panelLoaded = true;

            grdNotes.DataSource = _notes;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            _notes.Clear();

            var currentOrder = CurrentRecord as OrdersDataSet.OrderRow;

            if (currentOrder != null)
            {
                foreach (var noteRow in currentOrder.GetOrderNoteRows().OrderBy(n => n.TimeStamp))
                {
                    _notes.Add(new NoteInfo(noteRow));
                }
            }
        }

        #endregion

        #region NoteInfo

        public class NoteInfo
        {
            #region Properties

            public OrdersDataSet.OrderNoteRow Row { get; }

            public string Note => Row == null || Row.IsNotesNull()
                ? string.Empty
                : Row.Notes;
            #endregion

            #region Methods

            public NoteInfo(OrdersDataSet.OrderNoteRow row)
            {
                Row = row ?? throw new ArgumentNullException(nameof(row));
            }

            #endregion
        }

        #endregion
    }
}
