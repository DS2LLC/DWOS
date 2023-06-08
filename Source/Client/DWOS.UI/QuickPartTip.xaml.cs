using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.PartsDatasetTableAdapters;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI
{
    /// <summary>
    /// Interaction logic for QuickPartTip.xaml
    /// </summary>
    public partial class QuickPartTip
    {
        #region Fields

        private bool _unloaded;
        private BackgroundWorker _partWorker;

        #endregion

        #region Methods

        public QuickPartTip()
        {
            InitializeComponent();
            Unloaded += QuickPartTip_Unloaded;
        }

        private void QuickPartTip_Unloaded(object sender, RoutedEventArgs e)
        {
            _unloaded = true;

            if (_partWorker != null && _partWorker.IsBusy)
                _partWorker.CancelAsync();

            _partWorker = null;
        }

        public void LoadPart(int orderId)
        {
            try
            {
                _partWorker = new BackgroundWorker {WorkerSupportsCancellation = true};

                _partWorker.DoWork += (s, e) =>
                {
                    if (!e.Cancel)
                        e.Result = LoadPartRow(Convert.ToInt32(e.Argument));
                };

                _partWorker.RunWorkerCompleted += (s, e) =>
                {
                    if (e.Cancelled || !(e.Result is PartsDataset.PartRow part) || _unloaded)
                    {
                        return;
                    }

                    SetPartData(part);

                    var imageWorker = new BackgroundWorker {WorkerSupportsCancellation = true};
                    imageWorker.DoWork += (s1, e1) =>
                    {
                        if (!e1.Cancel)
                            e1.Result = LoadPartImage(Convert.ToInt32(e1.Argument));
                    };

                    imageWorker.RunWorkerCompleted += (s1, e1) =>
                    {
                        if (e1.Cancelled || _unloaded)
                            return;

                        if (e1.Result is Image image)
                            SetPartImage(image);
                    };

                    imageWorker.RunWorkerAsync(part.PartID);
                };

                _partWorker.RunWorkerAsync(orderId);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading part quick tip.");
            }
        }

        private void SetPartData(PartsDataset.PartRow part)
        {
            try
            {
                // Run on the UI thread - possible fix for #18854
                if (Dispatcher.CheckAccess())
                {
                    txtPartName.Text = part.Name;
                    txtManufacturer.Text = part.IsManufacturerIDNull() ? "NA" : part.ManufacturerID;
                    txtModel.Text = part.IsAirframeNull() ? "NA" : part.Airframe;
                    txtMaterial.Text = part.IsMaterialNull() ? "NA" : part.Material;
                }
                else
                {
                    Action<PartsDataset.PartRow> del = SetPartData;
                    Dispatcher.BeginInvoke(del, part);
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading part data for part tooltip.");
            }
        }

        private void SetPartImage(Image image)
        {
            try
            {
                // Run on the UI thread - possible fix for #18854
                if (Dispatcher.CheckAccess())
                {
                    imgPart.Source = image.ToWpfImage();
                }
                else
                {
                    Action<Image> del = SetPartImage;
                    Dispatcher.BeginInvoke(del, image);
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading part image for part tooltip.");
            }
        }

        private static PartsDataset.PartRow LoadPartRow(int orderId)
        {
            try
            {
                using (var ta = new PartTableAdapter())
                {
                    var parts = ta.GetByOrder(orderId);
                    var part = parts.FirstOrDefault();

                    return part;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error getting part.");
                return null;
            }
        }

        private static Image LoadPartImage(int partId)
        {
            try
            {
                using (var taMedia = new MediaTableAdapter())
                {
                    var media = new PartsDataset.MediaDataTable();
                    taMedia.FillByPartIDWithoutMedia(media, partId);
                    var mediaId = 0;
                    var extension = "";

                    foreach (var mediaRow in media)
                    {
                        if (mediaRow.IsFileExtensionNull() || !MediaUtilities.IsImageExtension(mediaRow.FileExtension))
                        {
                            continue;
                        }

                        mediaId = mediaRow.MediaID;
                        extension = mediaRow.FileExtension;
                        break;
                    }

                    if (mediaId > 0)
                    {
                        return MediaUtilities.GetThumbnail(mediaId, extension, null, 256);
                    }
                }

                return null;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error getting image.");
                return null;
            }
        }

        #endregion
    }
}
