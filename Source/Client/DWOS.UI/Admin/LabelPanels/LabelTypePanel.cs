using DWOS.Data;
using DWOS.Data.Datasets;
using System;
using System.Data;

namespace DWOS.UI.Admin.LabelPanels
{
    public partial class LabelTypePanel: DataPanel
    {
        #region Properties

        public LabelDataSet Dataset
        {
            get { return base._dataset as LabelDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return this.Dataset.LabelType.LabelTypeIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public LabelTypePanel()
        {
            this.InitializeComponent();
        }

        public void LoadData(LabelDataSet dataset)
        {
            this.Dataset = dataset;

            bsData.DataSource = this.Dataset;
            bsData.DataMember = this.Dataset.LabelType.TableName;

            //bind column to control
            base.BindValue(this.cboLabelType, this.Dataset.LabelType.LabelTypeIDColumn.ColumnName, true);
            base.BindList(this.cboLabelType, this.Dataset.LabelType, this.Dataset.LabelType.LabelTypeIDColumn.ColumnName, this.Dataset.LabelType.NameColumn.ColumnName);

            base._panelLoaded = true;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            var current = this.CurrentRecord as LabelDataSet.LabelTypeRow;
            LoadPreviewImage(current);
        }

        private void LoadPreviewImage(LabelDataSet.LabelTypeRow labelRow)
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
                using (var mediaAdapter = new Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter())
                {
                    var bytes = mediaAdapter.GetMediaStream(row.MediaID);

                    //if row has no changes then cache the bytes and set it back to no changes
                    if (row.RowState == DataRowState.Unchanged)
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

        #endregion
    }
}