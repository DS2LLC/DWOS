using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Shared;
using DWOS.UI.Properties;
using Infragistics.Win.UltraWinTree;
using NLog;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// An version of <see cref="MediaWidget"/> designed to be embedded into
    /// a WPF.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This exists because Dynamic .NET TWAIN for WPF is built so that you
    /// cannot reference it and the WinForms version of the assembly.
    /// </para>
    /// <para>
    /// The 'update existing media/links' part of this widget have not been tested.
    /// </para>
    /// </remarks>
    public partial class MediaWidgetEmbeddable : UserControl
    {
        #region Fields

        //Media Table Columns
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        private bool _allowEdit;
        private bool _inAfterCheck;
        private int? _parentRowKey;
        private int _nextMediaId = -1;
        private int _nextDocumentId = -1;
        private Dynamsoft.DotNet.TWAIN.DynamicDotNetTwain _twain;
        private Random _random = new Random();

        public event EventHandler<FileChangedEventArgs> FileAdded;

        private readonly DisplayDisabledTooltips _displayTips;

        #endregion

        #region Properties

        public ObservableCollection<MediaLink> MediaLinks { get; private set; }

        public ObservableCollection<DocumentLink> DocumentLinks { get; private set; }

        public int MediaCount
        {
            get { return tvwMedia.Nodes.Count; }
        }

        public bool AllowEditing
        {
            get { return _allowEdit; }
            set
            {
                _allowEdit = value;
                UpdateButtonEnabledStates();
            }
        }

        public bool ShowDefault
        {
            get; set;
        }

        public ScannerSettingsType ScannerSettingsType { get; private set; }

        #endregion

        #region Methods

        public MediaWidgetEmbeddable()
        {
            InitializeComponent();
            _displayTips = new DisplayDisabledTooltips(MediaWidget_Fill_Panel, ultraToolTipManager1);
        }

        public void Setup(SetupArgs args)
        {
            if (args != null)
            {
                MediaLinks = args.MediaLinks;
                DocumentLinks = args.DocumentLinks;
                AllowEditing = args.AllowEditing;
                ShowDefault = args.ShowDefault;
                ScannerSettingsType = args.ScannerSettingsType;
            }
        }

        public void LoadMedia(List<MediaLink> mediaLinks, List<DocumentLink> documentLinks, int parentRowKey)
        {
            try
            {
                ClearMedia();
                _parentRowKey = parentRowKey;

                DocumentNodeBase selectedNode = null;

                foreach (var link in mediaLinks)
                {
                    var node = AddNewMediaNode(link);
                    if (node != null && link.IsDefault)
                    {
                        selectedNode = node;
                    }
                }

                foreach (var link in documentLinks)
                {
                    AddNewLinkNode(link);
                }

                //if didn't select the default one then just select the first one
                if (selectedNode == null && tvwMedia.Nodes.Count > 0)
                {
                    selectedNode = tvwMedia.Nodes.OfType<DocumentNodeBase>().FirstOrDefault();
                }

                if (selectedNode != null)
                {
                    //select node to fire all selection events NOTE: tvw will not allow selection if not enabled
                    selectedNode.Select();

                    if (ShowDefault)
                    {
                        selectedNode.CheckedState = CheckState.Checked;
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading media into widget.");
            }
        }

        public void ClearMedia()
        {
            try
            {
                _parentRowKey = null;
                tvwMedia.Nodes.ForEachNode(n => n.Dispose());
                tvwMedia.Nodes.Clear();

                if (picPartImage.Image is Image)
                    ((Image) picPartImage.Image).Dispose();

                picPartImage.Image = null;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error clearing media.");
            }
        }

        private int NewMediaId()
        {
            return _nextMediaId--;
        }

        private int NewDocumentLinkId()
        {
            return _nextDocumentId--;
        }

        private MediaNode AddNewMediaNode(MediaLink mediaLink)
        {
            if (mediaLink == null || mediaLink.Item == null)
            {
                return null;
            }

            _log.Debug("Adding new media node " + mediaLink.Item.Name);

            //add to Media Tree
            var mn = new MediaNode(mediaLink, this);
            this.tvwMedia.Nodes.Add(mn);
            return mn;
        }

        private DocumentLinkNode AddNewLinkNode(DocumentLink documentLink)
        {
            try
            {
                if (documentLink == null)
                {
                    return null;
                }
                else
                {
                    var node = new DocumentLinkNode(documentLink, Documents.DocumentUtilities.GetFileName((int)documentLink.DocumentInfoId), this);
                    this.tvwMedia.Nodes.Add(node);
                    return node;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding new document link node.");
                return null;
            }
        }

        private void AddNewMedia()
        {
            var bytes = MediaUtilities.GetNewMedia(out var fileName, Settings.Default.PartManagerMediaDirectory);
            AddNewMedia(fileName, bytes);
        }

        private void AddNewMedia(string fileName, byte[] bytes)
        {
            const int maxExtensionLength = 10;

            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            if (bytes == null || bytes.Length == 0)
            {
                MessageBoxUtilities.ShowMessageBoxError(
                    "Unable to save blank document.",
                    "Media");

                return;
            }

            var name = Path.GetFileNameWithoutExtension(fileName).TrimToMaxLength(50);
            var fName = Path.GetFileName(fileName).TrimToMaxLength(50);
            var ext = Path.GetExtension(fileName).Replace(".", "");
            if (ext.Length > maxExtensionLength)
            {
                MessageBoxUtilities.ShowMessageBoxError(
                    $"Unable to save a file with an extension that's longer than {maxExtensionLength} characters in length.\nPlease rename the file and try again.",
                    "Media");

                return;
            }

            _log.Info($"Adding new media named '{Path.GetFileName(fileName)}'");

            if (string.IsNullOrWhiteSpace(ext))
            {
                const string extensionWarningMessage = "The file that you " +
                    "selected has a blank file extension." +
                    "\nYou may be unable to open this file.";

                MessageBoxUtilities.ShowMessageBoxWarn
                    (extensionWarningMessage,
                    "Media");
            }

            // Create new record
            var mediaItem = new MediaItem
            {
                MediaId = NewMediaId(),
                Name = name,
                FileName = fName,
                FileExtension = ext,
                Data = bytes,
            };

            // Add relation
            var mediaLink = new MediaLink
            {
                Item = mediaItem,
                ParentId = _parentRowKey ?? -1,
                IsDefault = ShowDefault
            };

            MediaLinks.Add(mediaLink);

            // Add node
            var mn = AddNewMediaNode(mediaLink);

            if (mediaLink.IsDefault)
            {
                mn.CheckedState = CheckState.Checked;
            }

            FileAdded?.Invoke(this, new FileChangedEventArgs {FilePath = fileName});
        }

        private void AddNewDocumentLink()
        {
            try
            {
                List<DWOS.Data.Datasets.DocumentsDataSet.DocumentInfoRow> docs;
                using (var docManager = new DWOS.UI.Documents.DocumentManager())
                {
                    docManager.ShowDialog(this);
                    docs = docManager.SelectedDocuments;
                }

                var parentRowID = _parentRowKey.GetValueOrDefault();

                if (docs != null)
                {
                    foreach (var doc in docs)
                    {
                        var documentLink = new DocumentLink
                        {
                            DocumentInfoId = doc.DocumentInfoID,
                            LinkId = NewDocumentLinkId(),
                            ParentId = parentRowID
                        };

                        DocumentLinks.Add(documentLink);
                        AddNewLinkNode(documentLink);
                    }
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error adding new document link.", exc);
            }
        }

        private void UpdateButtonEnabledStates()
        {
            this.btnAddMedia.Enabled = this._allowEdit;
            this.btnDelete.Enabled = this._allowEdit && this.tvwMedia.SelectedNodes.Count > 0 && SecurityManager.Current.IsInRole("DeleteMedia");
            this.btnScanner.Enabled = this._allowEdit;

            // Menu items
            ultraToolbarsManager.Tools["EditExtension"].SharedProps.Enabled = _allowEdit
                && SecurityManager.Current.IsInRole("ChangeMediaFileExtension");

            ultraToolbarsManager.Tools["Rename"].SharedProps.Enabled = _allowEdit;
        }

        private void RenameCurrentSelection()
        {
            var selection = tvwMedia.SelectedNode<MediaNode>();

            if (selection == null)
            {
                MessageBox.Show("Cannot rename selected media.");
                return;
            }

            selection.ShowRenameDialog();
        }

        private void EditExtensionForCurrentSelection()
        {
            var selection = tvwMedia.SelectedNode<MediaNode>();

            if (selection == null)
            {
                MessageBox.Show("Cannot change file extension for selected media.");
                return;
            }

            selection.ShowExtensionDialog();
        }

        private void LoadNode(DocumentNodeBase node)
        {
            try
            {
                if (this.picPartImage.Image is Image)
                    ((Image) this.picPartImage.Image).Dispose();

                this.picPartImage.Image = null;

                if (node != null)
                {
                    this.picPartImage.Image = node.Thumbnail;
                }

                UpdateButtonEnabledStates();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading media node.");
            }
        }

        private void OnDisposing()
        {
            try
            {
                _log.Debug("Disposing media widget.");

                FileAdded = null;
                LoadNode(null);

                if (this.tvwMedia != null && !this.tvwMedia.IsDisposed)
                    this.tvwMedia.Nodes.DisposeAll();

                _displayTips?.Dispose();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error disposing of media widget.");
            }
        }

        /// <summary>
        /// Retrieves the most current instance of <see cref="ScannerSettings"/>
        /// from <see cref="UserSettings.Default"/> using this instance's
        /// current scanner settings type.
        /// </summary>
        /// <returns></returns>
        private ScannerSettings RetrieveWidgetSettings()
        {
            return UserSettings.Default.Media.Get(this.ScannerSettingsType) ??
                   ScannerSettings.DefaultFrom(UserSettings.Default);
        }

        private void SaveScan(byte[] bytes, string fileExt, string name)
        {
            try
            {
                if (bytes == null || bytes.Length == 0)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(
                        "Unable to save blank document.",
                        "Scan Error");

                    return;
                }

                _log.Info("Saving scan {0} of size {1}.".FormatWith(name, bytes.Length));

                var mediaName = name.Length > 49 ? name.Substring(0, 49) : name;

                // Create new record
                var mediaItem = new MediaItem
                {
                    MediaId = NewMediaId(),
                    Name = mediaName,
                    FileName = _random.Next(100000, 999999) + "." + fileExt,
                    FileExtension = fileExt,
                    Data = bytes
                };


                // Add relation
                var mediaLink = new MediaLink
                {
                    ParentId = _parentRowKey ?? -1,
                    Item = mediaItem,
                    IsDefault = ShowDefault
                };

                MediaLinks.Add(mediaLink);

                var mn = AddNewMediaNode(mediaLink);

                if (mediaLink.IsDefault)
                {
                    mn.CheckedState = CheckState.Checked;
                }

                mn.Select(true);

                FileAdded?.Invoke(this, new FileChangedEventArgs
                {
                    FilePath = string.Empty
                });
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error saving scan.");
            }
        }

        private void StartScan()
        {
            var isWebCam = false;
            try
            {
                //ensure previous scan closed
                if (_twain != null)
                    CloseTwainScanner();

                _twain = new Dynamsoft.DotNet.TWAIN.DynamicDotNetTwain(MediaWidget.TWAIN_LICENSE);
                _twain.SupportedDeviceType = Dynamsoft.DotNet.TWAIN.Enums.EnumSupportedDeviceType.SDT_ALL;

                //use selected source if set
                var widgetSettings = RetrieveWidgetSettings();
                if (!string.IsNullOrWhiteSpace(widgetSettings.ScanDeviceName))
                {
                    _twain.OpenSourceManager();

                    for (short lngNum = 0; lngNum < _twain.SourceCount; lngNum++)
                    {
                        if (_twain.SourceNameItems(lngNum) == widgetSettings.ScanDeviceName)
                        {
                            _twain.SelectSourceByIndex(lngNum);
                            break;
                        }
                    }
                }

                //if no source then pop up dialog to select it
                if (string.IsNullOrWhiteSpace(_twain.CurrentSourceName))
                {
                    _twain.SelectSource();
                    widgetSettings.ScanDeviceName = _twain.CurrentSourceName;
                    UserSettings.Default.Media.Set(this.ScannerSettingsType, widgetSettings);
                }

                //if still no source then exit
                if (String.IsNullOrWhiteSpace(_twain.CurrentSourceName))
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("No valid scanner source selected.", "No Scanner");
                    CloseTwainScanner();
                    return; // no source
                }

                _twain.OpenSource();

                //if unable to open source then exit
                if (_twain.ErrorCode != Dynamsoft.DotNet.TWAIN.Enums.ErrorCode.Succeed)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("Scan Error: " + _twain.ErrorString, "Error Opening Scanner");
                    _log.Info(new ApplicationException(_twain.ErrorString), "Error on scan.");
                    CloseTwainScanner();
                    return; // error opening the source
                }

                isWebCam = IsDeviceWebCam(_twain, widgetSettings.ScanDeviceName);
                _twain.IfShowIndicator = true;
                _twain.IfShowUI = widgetSettings.ScanShowFullUI;
                _twain.PDFCompressionType = Dynamsoft.DotNet.TWAIN.Enums.DWTPDFCompressionType.enumJPEGEncode;
                _twain.TIFFCompressionType = Dynamsoft.DotNet.TWAIN.Enums.DWTTIFFCompressionType.TIFF_JPEG;
                _twain.JPEGQuality = Convert.ToInt16(widgetSettings.ScanQuality);
                _twain.Resolution = widgetSettings.ScanResolution;
                _twain.IfDisableSourceAfterAcquire = true;

                //if is webcam and wants to see the preview then capture it
                if (isWebCam && widgetSettings.ScanShowFullUI)
                {
                    using (new UsingWaitCursor(this.ParentForm))
                    {
                        _twain.SetVideoContainer(picWebCamPreview);
                        pnlWebCam.Size = new Size(320, 240);
                        popWebCamPreview.Show(btnScanner);
                        btnSave.Focus();
                        _twain.OpenSource();
                    }
                }
                else
                {
                    if (_twain.AcquireImage())
                    {
                        FinishScan(_twain, isWebCam);
                    }
                    else
                    {
                        var errorMsg = isWebCam
                            ? "There was a problem with retrieving an image from your webcam. Please try again."
                            : "There was a problem while scanning. Please try again.";

                        MessageBoxUtilities.ShowMessageBoxWarn(errorMsg, "Scan Error");
                        _log.Info("Scan error during acquire: {0}", _twain.ErrorString);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error scanning document.");
            }
            finally
            {
                if (!isWebCam)
                {
                    CloseTwainScanner();
                }
            }
        }

        private void FinishScan(Dynamsoft.DotNet.TWAIN.DynamicDotNetTwain twain, bool isWebCam)
        {
            if (twain == null)
                return;

            if (twain.HowManyImagesInBuffer > 0)
            {
                if (this.IsDisposed)
                    return;

                _log.Info("Scan Finished.");

                byte[] bytes;
                var fileExt = "";
                var fileName = "";

                if (isWebCam)
                {
                    //force to JPEG
                    var tmpFileName = System.IO.Path.GetTempFileName();
                    twain.SaveAsJPEG(tmpFileName, 0);
                    bytes = MediaUtilities.CreateMediaStream(tmpFileName);
                    fileExt = "jpg";
                    fileName = "Camera " + DateTime.Now.ToString("M.dd.yy");
                }
                else
                {
                    var widgetSettings = RetrieveWidgetSettings();
                    bytes = widgetSettings.ScanOutputPDF
                        ? twain.SaveAllAsPDFToBytes()
                        : twain.SaveAllAsMultiPageTIFFToBytes();
                    fileExt = widgetSettings.ScanOutputPDF ? "pdf" : "tiff";
                    fileName = "Scan " + DateTime.Now.ToString("M.dd.yy");
                }

                SaveScan(bytes, fileExt, fileName);
            }
            else if (_twain.ErrorString != "Successful.")
            {
                _log.Info("Scan error after acquire: {0}", _twain.ErrorString);
            }
            else
            {
                _log.Info("Could not retrieve image from scanner.");
            }
        }

        private void CloseTwainScanner()
        {
            try
            {
                if (popWebCamPreview.IsDisplayed)
                    popWebCamPreview.Close();

                if (_twain != null)
                {
                    _twain.RemoveAllImages();
                    _twain.Dispose();
                    _twain = null;
                }

                GC.Collect();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error closing scanner.");
            }
        }

        private bool IsDeviceWebCam(Dynamsoft.DotNet.TWAIN.DynamicDotNetTwain twain, string deviceName)
        {
            //Find source index by name
            var widgetSettings = RetrieveWidgetSettings();
            for (short lngNum = 0; lngNum < twain.SourceCount; lngNum++)
            {
                if (twain.SourceNameItems(lngNum) == widgetSettings.ScanDeviceName)
                {
                    return twain.GetSourceType(lngNum) == Dynamsoft.DotNet.TWAIN.Enums.EnumDeviceType.SDT_WEBCAM;
                }
            }

            return false;
        }

        private void HandleAddMediaError(Exception exc)
        {
            if (exc is IOException)
            {
                _log.Warn(exc, "Error adding new media - file may be locked by other program.");

                MessageBoxUtilities.ShowMessageBoxError(
                    "The file that you selected is in-use. Please close all other programs using the file and try again.",
                    "Media");
            }
            else
            {
                ErrorMessageBox.ShowDialog("Error adding new media.", exc);
            }
        }

        #endregion

        #region Events

        private void btnAddMedia_Click(object sender, EventArgs e)
        {
            try
            {
                AddNewMedia();
            }
            catch (Exception exc)
            {
                HandleAddMediaError(exc);
            }
        }

        private void tvwMedia_AfterCheck(object sender, NodeEventArgs e)
        {
            try
            {
                if(!this._inAfterCheck)
                {
                    this._inAfterCheck = true;

                    tvwMedia.Nodes.OfType <MediaNode>().ForEach(n =>
                                                                         {
                                                                             var isDefault = n == e.TreeNode;
                                                                             n.CheckedState = isDefault ? CheckState.Checked : CheckState.Unchecked;
                                                                             n.MediaLink.IsDefault = isDefault;
                                                                         });

                    this._inAfterCheck = false;
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error after media checked.");
            }
        }

        private void tvwMedia_AfterSelect(object sender, SelectEventArgs e)
        {
            try
            {
                LoadNode(this.tvwMedia.SelectedNode<DocumentNodeBase>());
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on media after select.");
            }
        }

        private void tvwMedia_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                // Select on right click
                // Adapted from code in DataEditorBase
                if (e.Button == MouseButtons.Left)
                {
                    return;
                }

                var nodeAtPoint = tvwMedia.GetNodeFromPoint(e.Location);

                if (nodeAtPoint == null)
                {
                    tvwMedia.SelectedNodes.Clear();
                }
                else if (e.Button != MouseButtons.Left)
                {
                    // UltraTree has 'left click to select' built-in.
                    // Must manually select nodes when using other buttons.

                    // If using right-click, make sure that the list doesn't
                    // include the item before changing the selection.
                    var skipSelection = nodeAtPoint == null
                        || (e.Button == MouseButtons.Right && tvwMedia.SelectedNodes.IndexOf(nodeAtPoint) >= 0);

                    if (!skipSelection)
                    {
                        nodeAtPoint.Select(false);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling mouse up event.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedNode = this.tvwMedia.SelectedNode<DocumentNodeBase>();

                if (selectedNode != null)
                {
                    selectedNode.Delete();
                    this.tvwMedia.SelectedNodes.Clear();
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error deleting media.");
            }
        }

        private void tvwMedia_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var selectedNode = this.tvwMedia.SelectedNode<DocumentNodeBase>();

                if (selectedNode != null)
                {
                    selectedNode.DoubleClick();

                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error renaming media.");
            }
        }

        private void picPartImage_Click(object sender, EventArgs e)
        {
            try
            {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Info, "Opening media for viewing.");
                
                var selectedNode = this.tvwMedia.SelectedNode<DocumentNodeBase>();
                if (selectedNode != null)
                    selectedNode.ImageClick();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on part image click.");
            }
        }
        
        private void btnScanner_Click(object sender, EventArgs e)
        {
            _log.Info("Scanner clicked.");

            //if no device selected then show the settings dialog.
            if (String.IsNullOrWhiteSpace(RetrieveWidgetSettings().ScanDeviceName))
                this.btnScanner.DropDown();
            else
               StartScan();
        }

        private void btnScanner_ClosedUp(object sender, EventArgs e)
        {
            try
            {
                scannerProperties1.SaveData(this.ScannerSettingsType);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error closing scanner properties.");
            }
        }

        private void btnScanner_DroppingDown(object sender, CancelEventArgs e)
        {
            try
            {
                _log.Info("Scanner drop down.");
                this.scannerProperties1.LoadData(this.ScannerSettingsType);
            }
            catch (Exception exc)
            {
                if(exc.Message.Contains("Error getting first source."))
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("No image acquisition sources found.", "No Sources", "Ensure your scanner or camera is plugged in and working properly.");
                }
                else
                    NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading scanner properties.");
            }
        }

        private void tvwMedia_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void tvwMedia_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                foreach (var file in files)
                {
                    if (!File.Exists(file))
                    {
                        continue;
                    }

                    AddNewMedia(file, File.ReadAllBytes(file));
                }
            }
            catch (Exception exc)
            {
                HandleAddMediaError(exc);
            }
        }

        private void ultraToolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            try
            {
                switch (e.Tool.Key)
                {
                    case "AddMedia":
                        AddNewMedia();
                        break;
                    case "LinkDocument":
                        AddNewDocumentLink();
                        break;
                    case "Rename":
                        RenameCurrentSelection();
                        break;
                    case "EditExtension":
                        EditExtensionForCurrentSelection();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exc)
            {
                switch (e.Tool.Key)
                {
                    case "AddMedia":
                        HandleAddMediaError(exc);
                        break;
                    default:
                        _log.Error(exc, "Error on tool click: " + e.Tool.Key);
                        break;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.IsDisposed || _twain == null)
                    return;

                var widgetSettings = RetrieveWidgetSettings();

                if (widgetSettings.ScanShowFullUI && IsDeviceWebCam(_twain, widgetSettings.ScanDeviceName))
                {
                    _twain.EnableSource();
                }

                _twain.AcquireImage();
                FinishScan(_twain, true);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error on save scan click.");
            }
            finally
            {
                popWebCamPreview.Close();
            }
        }
        
        private void popWebCamPreview_Closed(object sender, EventArgs e) { CloseTwainScanner(); }

        #endregion

        #region Nested type: FileChangedEventArgs

        public class FileChangedEventArgs : EventArgs
        {
            public string FilePath { get; set; }
        }

        #endregion

        #region Nested type: DocumentNodeBase

        private abstract class DocumentNodeBase : UltraTreeNode, ISortable, IDeleteNode
        {
            #region Properties

            public abstract Image Thumbnail { get; }

            #endregion

            #region Methods

            public DocumentNodeBase(string id, string keyPrefix, string name)
                : base(CreateKey(keyPrefix, id), name)
            {
            }

            private static string CreateKey(string keyPrefix, string id) { return keyPrefix + "-" + id; }

            public abstract void DoubleClick();

            public abstract void ImageClick();

            #endregion

            #region ISortable Members

            public string SortKey => Text;

            #endregion

            #region IDeleteNode Members

            public abstract void Delete();

            public bool CanDelete => true;

            #endregion
        }

        #endregion

        #region Nested type: MediaNode

        private class MediaNode : DocumentNodeBase
        {
            #region Fields

            private const string KEY_PREFIX = "MEDIA";
            private MediaWidgetEmbeddable _widgetInstance;

            #endregion

            #region Properties

            public override Image Thumbnail
            {
                get
                {
                    Image thumbnail;
                    string extension = MediaLink.Item.FileExtension;

                    //if is not an image then just set to that
                    if (!MediaUtilities.IsImageExtension(extension))
                    {
                        thumbnail = MediaUtilities.GetGenericThumbnail(extension);
                    }
                    else //else create a proper thumbnail of the image
                    {
                        thumbnail = MediaUtilities.GetThumbnail(Convert.ToInt32(MediaLink.Item.MediaId), extension, GetBytes(MediaLink.Item));
                    }

                    return thumbnail;
                }
            }

            public MediaLink MediaLink
            {
                get;
            }

            #endregion

            #region Methods

            public MediaNode(MediaLink link, MediaWidgetEmbeddable widgetInstance) : base(link.Item.MediaId.ToString(), KEY_PREFIX, link.Item.Name)
            {
                _widgetInstance = widgetInstance;
                MediaLink = link;
                base.LeftImages.Clear();
                base.LeftImages.Add(MediaUtilities.GetGenericThumbnail(link.Item.FileExtension));

                //add the OptionButton if using a Default media id
                if (_widgetInstance.ShowDefault)
                {
                    Override.NodeStyle = NodeStyle.OptionButton;
                }
            }

            public void UpdateNodeUI() { Text = MediaLink.Item.Name; }

            public override void DoubleClick() =>
                ShowRenameDialog();

            public void ShowRenameDialog()
            {
                if (_widgetInstance.AllowEditing)
                {
                    using (var tb = new TextBoxForm())
                    {
                        tb.Text = "Rename Media";
                        tb.FormLabel.Text = "Name:";
                        tb.FormTextBox.MaxLength = 50;
                        tb.FormTextBox.Text = MediaLink.Item.Name;
                        tb.FormTextBox.SelectAllText();

                        tb.FormTextBox.Focus();

                        if (tb.ShowDialog(_widgetInstance) == DialogResult.OK)
                        {
                            if (!String.IsNullOrEmpty(tb.FormTextBox.Text))
                            {
                                MediaLink.Item.Name = tb.FormTextBox.Text;
                                MediaLink.Item.Data = GetBytes(MediaLink.Item); //have to set the bytes so when the row updates it won't wipe this column
                                UpdateNodeUI();
                            }
                        }
                    }
                }
            }

            public void ShowExtensionDialog()
            {
                if (!_widgetInstance.AllowEditing)
                {
                    return;
                }

                using (var tb = new TextBoxForm())
                {
                    tb.Text = "Change File Extension";
                    tb.FormLabel.Text = "Extension:";
                    tb.FormTextBox.MaxLength = 50;
                    tb.FormTextBox.Text = MediaLink.Item.FileExtension;
                    tb.FormTextBox.SelectAllText();

                    tb.FormTextBox.Focus();

                    if (tb.ShowDialog(_widgetInstance) == DialogResult.OK)
                    {
                        var newFileExtension = tb.FormTextBox.Text;

                        MediaLink.Item.FileExtension = newFileExtension;
                        MediaLink.Item.Data = GetBytes(MediaLink.Item); //have to set the bytes so when the row updates it won't wipe this column
                        UpdateNodeUI();
                    }
                }
            }

            public override void ImageClick()
            {
                var openedMedia = MediaUtilities.OpenMedia(MediaLink.Item.MediaId, MediaLink.Item.FileExtension, GetBytes(MediaLink.Item));

                if (!openedMedia)
                {
                    MessageBoxUtilities.ShowMessageBoxError(
                        "Could not open the selected media file. It may be empty.",
                        "Media");
                }
            }

            public override void Delete()
            {
                _widgetInstance.MediaLinks.Remove(MediaLink);
                Remove();
            }

            private byte[] GetBytes(MediaItem item)
            {
                if (item.Data == null)
                {
                    byte[] bytes;
                    using (var taMedia = new MediaTableAdapter())
                    {
                        bytes = taMedia.GetMediaStream(item.MediaId);
                    }

                    item.Data = bytes;

                    return bytes;
                }

                return item.Data;
            }


            #endregion
        }

        #endregion

        #region Nested type: DocumentLinkNode

        private class DocumentLinkNode : DocumentNodeBase
        {
            #region Fields

            private const string KEY_PREFIX = "DOC";
            private readonly MediaWidgetEmbeddable _widgetInstance;


            #endregion

            #region Properties

            public override Image Thumbnail
            {
                get
                {
                    string extension = Path.GetExtension(FileName).Replace(".", string.Empty);
                    return MediaUtilities.GetGenericThumbnail(extension);
                }
            }

            public string FileName
            {
                get;
                private set;
            }

            public DocumentLink DocumentLink
            {
                get;
            }

            #endregion

            #region Methods

            public DocumentLinkNode(DocumentLink link, string fileName, MediaWidgetEmbeddable widgetInstance)
                : base(link.LinkId.ToString(), KEY_PREFIX, fileName)
            {
                _widgetInstance = widgetInstance;
                DocumentLink = link;
                base.LeftImages.Clear();
                base.LeftImages.Add(DWOS.UI.Properties.Resources.Link_16);
                FileName = fileName;
            }

            public override void DoubleClick()
            {
                // Do nothing - cannot rename links.
            }

            public override void ImageClick()
            {
                string filePath = DWOS.UI.Documents.DocumentUtilities.DownloadDocument(DocumentLink.DocumentInfoId);

                if (!string.IsNullOrEmpty(filePath))
                {
                    DWOS.Shared.Utilities.FileLauncher.New()
                        .HandleErrorsWith((exception, errorMsg) => MessageBoxUtilities.ShowMessageBoxWarn(errorMsg, "Unable to Start", filePath))
                        .Launch(filePath);
                }
            }

            public override void Delete()
            {
                _widgetInstance.DocumentLinks.Remove(DocumentLink);
                Remove();
            }

            #endregion
        }

        #endregion

        #region Nested type: SetupArgs

        /// <summary>
        /// Arguments for MediaWidgetEmbeddable's Setup method.
        /// </summary>
        public sealed class SetupArgs
        {
            public ObservableCollection<MediaLink> MediaLinks { get; set; }

            public ObservableCollection<DocumentLink> DocumentLinks { get; set; }

            public bool AllowEditing { get; set; }

            public bool ShowDefault { get; set; }

            /// <summary>
            /// Gets a value indicating the type of scanner settings to use.
            /// </summary>
            public ScannerSettingsType ScannerSettingsType { get; set; }
        }

        #endregion

    }
}