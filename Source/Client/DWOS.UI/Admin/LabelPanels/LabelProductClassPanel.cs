using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Utilities.Validation;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DWOS.UI.Admin.LabelPanels
{
    public partial class LabelProductClassPanel: DataPanel
    {
        #region Properties

        public LabelDataSet Dataset
        {
            get => _dataset as LabelDataSet;
            set => _dataset = value;
        }

        protected override string BindingSourcePrimaryKey =>
            Dataset.ProductClassLabels.ProductClassLabelIDColumn.ColumnName;

        private Control ProductClassControl =>
            ApplicationSettings.Current.ProductClassEditorType == ProductClassType.Textbox
            ? (Control)txtProductClass
            : (Control)cboProductClass;

        #endregion

        #region Methods

        public LabelProductClassPanel()
        {
            InitializeComponent();
        }

        public void LoadData(LabelDataSet dataset)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.ProductClassLabels.TableName;

            //bind column to controls
            BindValue(cboLabelType, Dataset.ProductClassLabels.LabelTypeColumn.ColumnName, true, DataSourceUpdateMode.OnPropertyChanged);
            BindValue(txtProductClass, Dataset.ProductClassLabels.ProductClassColumn.ColumnName, true, DataSourceUpdateMode.OnPropertyChanged);
            BindValue(cboProductClass, Dataset.ProductClassLabels.ProductClassColumn.ColumnName, true, DataSourceUpdateMode.OnPropertyChanged);

            //bind lists
            BindList(cboLabelType, Dataset.LabelType, Dataset.LabelType.LabelTypeIDColumn.ColumnName, Dataset.LabelType.NameColumn.ColumnName);

            // Show appropriate product class editor
            var productClassEditor = ApplicationSettings.Current.ProductClassEditorType;
            cboProductClass.Visible = productClassEditor == ProductClassType.Combobox;
            txtProductClass.Visible = productClassEditor == ProductClassType.Textbox;

            _panelLoaded = true;
        }

        public override void AddValidators(ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(cboLabelType, "Label type required."), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(ProductClassControl, "Product class required."), errProvider));
        }

        public LabelDataSet.ProductClassLabelsRow Add(int labelTypeId)
        {
            if (!(bsData.AddNew() is DataRowView rowVw))
            {
                return null;
            }

            if (!(rowVw.Row is LabelDataSet.ProductClassLabelsRow cr))
            {
                return null;
            }

            cr.LabelType = labelTypeId;
            cr.Version = 1;

            return cr;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            var current = CurrentRecord as LabelDataSet.ProductClassLabelsRow;
            LoadPreviewImage(current);
            LoadData(current);
        }

        private void LoadPreviewImage(LabelDataSet.ProductClassLabelsRow labelRow)
        {
            if (picPreview.Image is IDisposable)
                ((IDisposable)picPreview.Image).Dispose();

            picPreview.Image = null;

            if (labelRow != null && labelRow.LabelMediaRow != null)
            {
                var mediaRow = labelRow.LabelMediaRow;

                string extension = mediaRow.FileExtension;

                //if is not an image then just set to that
                if (!MediaUtilities.IsImageExtension(extension))
                    picPreview.Image = MediaUtilities.GetGenericThumbnail(extension);
                else //else create a proper thumbnail of the image
                    picPreview.Image = MediaUtilities.GetImage(mediaRow.MediaID, extension, GetBytes(mediaRow));
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
            return row.Media;
        }

        private void LoadData(LabelDataSet.ProductClassLabelsRow row)
        {
            if (row == null )
            {
                return;
            }

            // Ensure that label data has been loaded
            if (row.RowState == DataRowState.Unchanged && row.IsDataNull())
            {
                using (var taLabel = new Data.Datasets.LabelDataSetTableAdapters.ProductClassLabelsTableAdapter())
                {
                    row.Data = taLabel.GetLabelData(row.ProductClassLabelID);
                }

                row.AcceptChanges();
            }

            // Load product class combobox
            if (ProductClassControl == cboProductClass)
            {
                cboProductClass.Items.Clear();

                foreach (var productClassRow in Dataset.ProductClass.OrderBy(pc => pc.Name))
                {
                    cboProductClass.Items.Add(productClassRow.Name);
                }

                if (!row.IsProductClassNull())
                {
                    if (Dataset.ProductClass.All(pc => pc.Name != row.ProductClass))
                    {
                        cboProductClass.Items.Add(row.ProductClass);
                    }

                    cboProductClass.Text = row.ProductClass;
                }
            }
        }

        #endregion
    }
}