using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Utilities.Validation;
using System;
using System.Data;
using System.Windows.Forms;

namespace DWOS.UI.Admin.LabelPanels
{
    public partial class LabelInfoPanel: DataPanel
    {
        #region Properties

        public LabelDataSet Dataset
        {
            get { return base._dataset as LabelDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return this.Dataset.Labels.LabelIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public LabelInfoPanel()
        {
            this.InitializeComponent();
        }

        public void LoadData(LabelDataSet dataset)
        {
            this.Dataset = dataset;
            bsData.DataSource = this.Dataset;
            bsData.DataMember = this.Dataset.Labels.TableName;

            //bind column to control
            base.BindValue(this.cboCustomer, this.Dataset.Labels.CustomerIDColumn.ColumnName, true, DataSourceUpdateMode.OnPropertyChanged);
            base.BindValue(this.cboLabelType, this.Dataset.Labels.LabelTypeColumn.ColumnName, true, DataSourceUpdateMode.OnPropertyChanged);

            //bind lists
            base.BindList(this.cboCustomer, this.Dataset.LabelCustomerSummary, this.Dataset.LabelCustomerSummary.CustomerIDColumn.ColumnName, this.Dataset.LabelCustomerSummary.NameColumn.ColumnName);
            base.BindList(this.cboLabelType, this.Dataset.LabelType, this.Dataset.LabelType.LabelTypeIDColumn.ColumnName, this.Dataset.LabelType.NameColumn.ColumnName);

            base._panelLoaded = true;
        }

        public override void AddValidators(ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboLabelType, "Label type required."), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboCustomer, "Customer required.") { PreserveWhitespace = true }, errProvider));
        }

        public LabelDataSet.LabelsRow Add(int labelTypeID)
        {
            var rowVw       = bsData.AddNew() as DataRowView;
            var cr          = rowVw.Row as LabelDataSet.LabelsRow;
            cr.LabelType    = labelTypeID;
            cr.Version      = 1;
            
            if(this.Dataset.LabelCustomerSummary.Count > 0)
                cr.CustomerID   = this.Dataset.LabelCustomerSummary[0].CustomerID;

            return cr;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            var current = this.CurrentRecord as LabelDataSet.LabelsRow;
            LoadPreviewImage(current);
            LoadData(current);
        }

        private void LoadPreviewImage(LabelDataSet.LabelsRow labelRow)
        {
            if (this.picPreview.Image is IDisposable)
                ((IDisposable)this.picPreview.Image).Dispose();

            this.picPreview.Image = null;

            if (labelRow != null && labelRow.LabelMediaRow != null)
            {
                var mediaRow = labelRow.LabelMediaRow;

                string extension = mediaRow.FileExtension;

                //if is not an image then just set to that
                if (!MediaUtilities.IsImageExtension(extension))
                    this.picPreview.Image = MediaUtilities.GetGenericThumbnail(extension);
                else //else create a proper thumbnail of the image
                    this.picPreview.Image = MediaUtilities.GetImage(mediaRow.MediaID, extension, GetBytes(mediaRow));
            }
        }

        private byte[] GetBytes(LabelDataSet.LabelMediaRow row)
        {
            if (row.IsMediaNull())
            {
                using(var mediaAdapter = new Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter())
                {
                    var bytes = mediaAdapter.GetMediaStream(row.MediaID);
                    
                    //if row has no changes then cache the bytes and set it back to no changes
                    if(row.RowState == DataRowState.Unchanged)
                    {
                        row.Media = bytes;
                        row.AcceptChanges();
                    }
                    else
                        row.Media = bytes;

                    return bytes;
                }
            }
            return (byte[])row.Media;
        }

        private void LoadData(LabelDataSet.LabelsRow row)
        {
            if (row == null || row.RowState != DataRowState.Unchanged || !row.IsDataNull())
            {
                return;
            }

            using (var taLabel = new Data.Datasets.LabelDataSetTableAdapters.LabelsTableAdapter())
            {
                row.Data = taLabel.GetLabelData(row.LabelID);
            }

            row.AcceptChanges();
        }

        #endregion
    }
}