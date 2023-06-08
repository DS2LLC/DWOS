using DWOS.Data.Datasets;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DWOS.UI.Utilities
{
    public partial class MediaDialog : Form
    {
        private MediaDialog()
        {
            InitializeComponent();
        }

        public static void ShowWorkOrderDialog(int orderID)
        {
            OrdersDataSet dataset = null;

            Data.Datasets.OrdersDataSetTableAdapters.Order_MediaTableAdapter taOrderMedia = null;
            Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter taMedia = null;
            Data.Datasets.OrdersDataSetTableAdapters.Order_DocumentLinkTableAdapter taOrderDocument = null;
            Data.Datasets.OrdersDataSetTableAdapters.TableAdapterManager taManager = null;

            try
            {
                dataset = new OrdersDataSet()
                {
                    EnforceConstraints = false // skips having to load many tables
                };

                taOrderMedia = new Data.Datasets.OrdersDataSetTableAdapters.Order_MediaTableAdapter();
                taMedia = new Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter();
                taOrderDocument = new Data.Datasets.OrdersDataSetTableAdapters.Order_DocumentLinkTableAdapter();

                taManager = new Data.Datasets.OrdersDataSetTableAdapters.TableAdapterManager()
                {
                    Order_MediaTableAdapter = taOrderMedia,
                    MediaTableAdapter = taMedia,
                    Order_DocumentLinkTableAdapter = taOrderDocument
                };

                taOrderMedia.FillByOrder(dataset.Order_Media, orderID);
                taMedia.FillByOrder(dataset.Media, orderID);
                taOrderDocument.FillByOrder(dataset.Order_DocumentLink, orderID);

                using (var dialog = new MediaDialog())
                {
                    dialog.Icon = Icon.FromHandle(Properties.Resources.Order_16.GetHicon());
                    dialog.lblIdentifier.Text = "Work Order:";
                    dialog.txtIdentifier.Text = orderID.ToString();
                    dialog.Text = "Work Order Media";

                    var tooltip = dialog.ultraToolTipManager.GetUltraToolTip(dialog.txtIdentifier);

                    if (tooltip != null)
                    {
                        tooltip.ToolTipTitle = "Work Order";
                        tooltip.ToolTipText = "Work Order Number";
                    }

                    dialog.mediaWidget.Setup(new MediaWidget.SetupArgs()
                    {
                        MediaJunctionTable = dataset.Order_Media,
                        MediaTable = dataset.Media,
                        MediaJunctionTableParentRowColumn = dataset.Order_Media.OrderIDColumn,
                        AllowEditing = true,
                        MediaLinkType = Documents.LinkType.WorkOrder,
                        DocumentLinkTable = dataset.Order_DocumentLink,
                        ScannerSettingsType = Data.ScannerSettingsType.Order
                    });

                    dialog.mediaWidget.LoadMedia(dataset.Order_Media.ToList<DataRow>(),
                        dataset.Order_DocumentLink.ToList<DataRow>(),
                        orderID);

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        taManager.UpdateAll(dataset);
                    }
                }
            }
            catch (Exception)
            {
                if (dataset != null)
                {
                    NLog.LogManager.GetCurrentClassLogger().Debug(dataset.GetDataErrors());
                }

                throw;
            }
            finally
            {
                dataset?.Dispose();
                taOrderMedia?.Dispose();
                taMedia?.Dispose();
                taOrderDocument?.Dispose();
                taManager?.Dispose();
            }
        }

        public static void ShowPartDialog(int partID)
        {
            PartsDataset dataset = null;

            Data.Datasets.PartsDatasetTableAdapters.Part_MediaTableAdapter taPartMedia = null;
            Data.Datasets.PartsDatasetTableAdapters.MediaTableAdapter taMedia = null;
            Data.Datasets.PartsDatasetTableAdapters.Part_DocumentLinkTableAdapter taPartDocument = null;
            Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter taPart = null;
            Data.Datasets.PartsDatasetTableAdapters.TableAdapterManager taManager = null;

            try
            {
                dataset = new PartsDataset()
                {
                    EnforceConstraints = false // skips having to load many tables
                };

                taPartMedia = new Data.Datasets.PartsDatasetTableAdapters.Part_MediaTableAdapter();
                taMedia = new Data.Datasets.PartsDatasetTableAdapters.MediaTableAdapter();
                taPartDocument = new Data.Datasets.PartsDatasetTableAdapters.Part_DocumentLinkTableAdapter();
                taPart = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter();

                taManager = new Data.Datasets.PartsDatasetTableAdapters.TableAdapterManager()
                {
                    Part_MediaTableAdapter = taPartMedia,
                    MediaTableAdapter = taMedia,
                    Part_DocumentLinkTableAdapter = taPartDocument
                };

                taPartMedia.FillByPartID(dataset.Part_Media, partID);
                taMedia.FillByPartIDWithoutMedia(dataset.Media, partID);
                taPartDocument.FillByPartID(dataset.Part_DocumentLink, partID);
                taPart.FillByPartID(dataset.Part, partID);

                using (var dialog = new MediaDialog())
                {
                    dialog.Icon = Icon.FromHandle(Properties.Resources.Part_16.GetHicon());
                    dialog.lblIdentifier.Text = "Part:";
                    dialog.txtIdentifier.Text = dataset.Part.FirstOrDefault()?.Name ?? string.Empty;
                    dialog.Text = "Part Media";

                    var tooltip = dialog.ultraToolTipManager.GetUltraToolTip(dialog.txtIdentifier);

                    if (tooltip != null)
                    {
                        tooltip.ToolTipTitle = "Part Name";
                        tooltip.ToolTipText = "The name of the part.";
                    }

                    dialog.mediaWidget.Setup(new MediaWidget.SetupArgs()
                    {
                        MediaJunctionTable = dataset.Part_Media,
                        MediaTable = dataset.Media,
                        MediaJunctionTableParentRowColumn = dataset.Part_Media.PartIDColumn,
                        MediaJunctionTableDefaultColumn = dataset.Part_Media.DefaultMediaColumn,
                        AllowEditing = true,
                        MediaLinkType = Documents.LinkType.Part,
                        DocumentLinkTable = dataset.Part_DocumentLink,
                        ScannerSettingsType = Data.ScannerSettingsType.Part
                    });

                    dialog.mediaWidget.LoadMedia(dataset.Part_Media.ToList<DataRow>(),
                        dataset.Part_DocumentLink.ToList<DataRow>(),
                        partID);

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        taManager.UpdateAll(dataset);
                    }
                }
            }
            catch (Exception)
            {
                if (dataset != null)
                {
                    NLog.LogManager.GetCurrentClassLogger().Debug(dataset.GetDataErrors());
                }

                throw;
            }
            finally
            {
                dataset?.Dispose();
                taPartMedia?.Dispose();
                taMedia?.Dispose();
                taPartDocument?.Dispose();
                taPart?.Dispose();
                taManager?.Dispose();
            }
        }

        private void OnDisposing(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            mediaWidget.ClearMedia();
        }
    }
}
