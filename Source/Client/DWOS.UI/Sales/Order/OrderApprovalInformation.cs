using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DWOS.UI.Sales.Order
{
    /// <summary>
    /// Shows information about an order approval in a <see cref="DataPanel"/>.
    /// </summary>
    public partial class OrderApprovalInformation : DataPanel
    {
        #region Properties

        public OrdersDataSet Dataset
        {
            get => _dataset as OrdersDataSet;
            set => _dataset = value;
        }

        protected override string BindingSourcePrimaryKey =>
            Dataset.OrderApproval.OrderApprovalIDColumn.ColumnName;

        #endregion

        #region Methods

        public OrderApprovalInformation()
        {
            InitializeComponent();
        }

        public void LoadData(OrdersDataSet dataset)
        {
            _dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.OrderApproval.TableName;

            bsTerms.DataSource = Dataset;
            bsTerms.DataMember = Dataset.OrderApprovalTerm.TableName;

            bsMedia.DataSource = Dataset;
            bsMedia.DataMember = Dataset.Media.TableName;

            BindValue(cboUser, Dataset.OrderApproval.UserIDColumn.ColumnName);
            BindValue(dteDateAdded, Dataset.OrderApproval.DateCreatedColumn.ColumnName);
            BindValue(txtStatus, Dataset.OrderApproval.StatusColumn.ColumnName);
            BindValue(cboTerms, Dataset.OrderApproval.OrderApprovalTermIDColumn.ColumnName);
            BindValue(txtNotes, Dataset.OrderApproval.NotesColumn.ColumnName);
            BindValue(cboContact, Dataset.OrderApproval.ContactIDColumn.ColumnName);
            BindValue(txtContactNotes, Dataset.OrderApproval.ContactNotesColumn.ColumnName);

            BindList(cboUser, Dataset.UserSummary, Dataset.UserSummary.UserIDColumn.ColumnName, Dataset.UserSummary.NameColumn.ColumnName);

            cboTerms.DataSource = bsTerms;
            cboTerms.ValueMember = Dataset.OrderApprovalTerm.OrderApprovalTermIDColumn.ColumnName;
            cboTerms.DisplayMember = Dataset.OrderApprovalTerm.NameColumn.ColumnName;

            cboPrimaryMedia.DataSource = bsMedia;
            cboPrimaryMedia.ValueMember = Dataset.Media.MediaIDColumn.ColumnName;
            cboPrimaryMedia.DisplayMember = Dataset.Media.NameColumn.ColumnName;

            cboAdditionalMedia.DataSource = bsMedia;
            cboAdditionalMedia.ValueMember = Dataset.Media.MediaIDColumn.ColumnName;
            cboAdditionalMedia.DisplayMember = Dataset.Media.NameColumn.ColumnName;

            BindList(cboContact, Dataset.ContactSummary, Dataset.ContactSummary.ContactIDColumn.ColumnName, Dataset.ContactSummary.NameColumn.ColumnName);

            _panelLoaded = true;
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            base.BeforeMoveToNewRecord(id);
            cboPrimaryMedia.ValueChanged -= cboPrimaryMedia_ValueChanged;
            cboPrimaryMedia.Value = null;
            cboAdditionalMedia.ValueChanged -= cboAdditionalMedia_ValueChanged;
            cboAdditionalMedia.Value = null;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            try
            {
                var currentApprovalRow = CurrentRecord as OrdersDataSet.OrderApprovalRow;

                // Filter terms - show active but include inactive, current terms
                if (currentApprovalRow != null && !currentApprovalRow.IsOrderApprovalTermIDNull())
                {
                    bsTerms.Filter = $"Active = 1 OR OrderApprovalTermID = {currentApprovalRow.OrderApprovalTermID}";
                }
                else
                {
                    bsTerms.Filter = "Active = 1";
                }

                // Filter media - show WO's but include current media
                if (currentApprovalRow == null)
                {
                    // Order is unknown - show no media.
                    bsMedia.Filter = "1 = 0";
                }
                else
                {
                    int? primaryMediaId = null;
                    var mediaIds = new HashSet<int>();
                    var secondaryMediaIds = new HashSet<int>();

                    foreach (var approvalMediaRow in currentApprovalRow.GetOrderApprovalMediaRows())
                    {
                        mediaIds.Add(approvalMediaRow.MediaID);

                        if (approvalMediaRow.IsPrimary)
                        {
                            primaryMediaId = approvalMediaRow.MediaID;
                        }
                        else
                        {
                            secondaryMediaIds.Add(approvalMediaRow.MediaID);
                        }
                    }

                    foreach (var orderMedia in currentApprovalRow.OrderRow.GetOrder_MediaRows())
                    {
                        mediaIds.Add(orderMedia.MediaID);
                    }

                    if (mediaIds.Count == 0)
                    {
                        // No media to select
                        bsMedia.Filter = "1 = 0";
                    }
                    else
                    {
                        bsMedia.Filter = $"MediaID IN ({string.Join(",", mediaIds)})";
                    }

                    cboPrimaryMedia.Value = primaryMediaId;

                    foreach (var item in cboAdditionalMedia.Items)
                    {
                        var itemMediaId = item.DataValue as int? ?? -1;

                        if (secondaryMediaIds.Contains(itemMediaId))
                        {
                            item.CheckState = CheckState.Checked;
                        }
                    }
                }

                // Refresh bindings - fixes issues where they can be blank
                cboTerms.DataBindings[0].ReadValue();

                // Do not allow changes to terms unless approval is still pending
                var status = (OrderApprovalStatus)Enum.Parse(
                    typeof(OrderApprovalStatus),
                    currentApprovalRow?.Status ?? nameof(OrderApprovalStatus.Pending));

                cboTerms.Enabled = status == OrderApprovalStatus.Pending;
            }
            finally
            {
                cboPrimaryMedia.ValueChanged += cboPrimaryMedia_ValueChanged;
                cboAdditionalMedia.ValueChanged += cboAdditionalMedia_ValueChanged;
            }
        }

        private byte[] GetBytes(OrdersDataSet.MediaRow row)
        {
            if (row.IsMediaNull())
            {
                byte[] bytes;
                using (var taMedia = new MediaTableAdapter())
                {
                    bytes = taMedia.GetMediaStream(row.MediaID);
                }

                //if row has no changes then cache the bytes and set it back to no changes
                if (row.RowState == DataRowState.Unchanged)
                {
                    row.Media = bytes;
                    row.AcceptChanges();
                }
                else
                {
                    row.Media = bytes;
                }
            }

            return row.Media;
        }

        #endregion

        #region Events

        private void cboTerms_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                if (e.Button.Key != "Clear")
                {
                    return;
                }

                // Clear terms selection
                cboTerms.Value = null;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling editor button click.");
            }
        }


        private void cboMedia_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                if (e.Button.Key == "Clear")
                {
                    cboPrimaryMedia.Value = null;
                }
                else if (e.Button.Key == "View")
                {
                    var mediaId = cboPrimaryMedia.Value as int? ?? -1;
                    var mediaRow = Dataset.Media.FindByMediaID(mediaId);

                    if (mediaRow == null)
                    {
                        MessageBoxUtilities.ShowMessageBoxError(
                            "Could not open the selected media file.",
                            "Media");
                    }
                    else
                    {
                        var openedMedia = MediaUtilities.OpenMedia(
                            mediaRow.MediaID,
                            mediaRow.IsFileExtensionNull() ? null : mediaRow.FileExtension,
                            GetBytes(mediaRow));

                        if (!openedMedia)
                        {
                            MessageBoxUtilities.ShowMessageBoxError(
                                "Could not open the selected media file. It may be empty.",
                                "Media");
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling editor button click.");
            }
        }

        private void cboPrimaryMedia_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!(CurrentRecord is OrdersDataSet.OrderApprovalRow currentApproval))
                {
                    return;
                }

                // Save change in primary media to dataset

                var currentPrimaryMedia = currentApproval
                    .GetOrderApprovalMediaRows()
                    .FirstOrDefault(m => m.IsPrimary);

                var mediaId = cboPrimaryMedia.Value as int?;

                if (mediaId.HasValue)
                {
                    if (currentPrimaryMedia == null)
                    {
                        Dataset.OrderApprovalMedia.AddOrderApprovalMediaRow(
                            currentApproval,
                            mediaId.Value,
                            true);
                    }
                    else
                    {
                        currentPrimaryMedia.MediaID = mediaId.Value;
                    }
                }
                else if (currentPrimaryMedia != null)
                {
                    currentPrimaryMedia.Delete();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing primary media.");
            }
        }

        private void cboAdditionalMedia_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!(CurrentRecord is OrdersDataSet.OrderApprovalRow currentApproval))
                {
                    return;
                }

                // Save change in secondary media to dataset

                var currentMedia = currentApproval
                    .GetOrderApprovalMediaRows()
                    .Where(m => !m.IsPrimary)
                    .ToList();

                var mediaToKeep = new HashSet<int>();

                foreach (var checkedItem in cboAdditionalMedia.CheckedItems)
                {
                    var mediaId = (checkedItem.DataValue as int?) ?? -1;
                    var matchingMediaLink = currentMedia.FirstOrDefault(m => m.MediaID == mediaId);

                    if (matchingMediaLink != null)
                    {
                        // No change
                        mediaToKeep.Add(mediaId);
                        continue;
                    }

                    // Create new entry for media
                    Dataset.OrderApprovalMedia.AddOrderApprovalMediaRow(
                        currentApproval,
                        mediaId,
                        false);
                }

                // Process existing media
                foreach (var media in currentMedia)
                {
                    if (mediaToKeep.Contains(media.MediaID))
                    {
                        // Media is still selected.
                        continue;
                    }

                    // User deselected media.
                    media.Delete();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing additional media.");
            }
        }

        #endregion
    }
}
