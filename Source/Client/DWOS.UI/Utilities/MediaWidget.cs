using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using DWOS.Data.Datasets.DocumentsDataSetTableAdapters;

namespace DWOS.UI.Utilities
{
    public partial class MediaWidget : UserControl
    {
        #region Fields

        public const string TWAIN_LICENSE = "F9068B9D6574034B076FC7E935B588D5;CEA863DCEC04BD4C7F12F0DE82DEB22D";

        //Media Table Columns
        public const string MEDIA_ID = "MediaID";
        public const string MEDIA_NAME = "Name";
        public const string MEDIA_FILE_NAME = "FileName";
        public const string MEDIA_FILE_EXT = "FileExtension";
        public const string MEDIA_DATA = "Media";

        private const string LINK_ID = "DocumentLinkID";
        private const string LINK_INFO_ID = "DocumentInfoID";
        private const string LINK_TYPE = "LinkToType";
        private const string LINK_KEY = "LinkToKey";

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private bool _allowEdit;
        private bool _inAfterCheck;
        private int? _parentRowKey;
        private Dynamsoft.DotNet.TWAIN.DynamicDotNetTwain _twain;
        private Random _random = new Random();

        public event EventHandler<FileAddedEventArgs> FileAdded;

        private readonly DisplayDisabledTooltips _displayTips;

        #endregion

        #region Properties

        public DataTable MediaTable { get; private set; }

        public DataTable MediaJunctionTable { get; private set; }

        public DataTable DocumentLinkTable { get; private set; }

        public int MediaCount
        {
            get { return this.tvwMedia.Nodes.Count; }
        }

        public string MediaJunctionTableDefaultColumn { get; private set; }

        public string MediaJunctionTableParentRowColumn { get; private set; }

        public bool AllowEditing
        {
            get { return this._allowEdit; }
            set
            {
                this._allowEdit = value;
                UpdateButtonEnabledStates();
            }
        }

        public DWOS.UI.Documents.LinkType MediaLinkType { get; private set; }

        public ScannerSettingsType ScannerSettingsType { get; private set; }

        #endregion

        #region Methods

        public MediaWidget()
        {
            InitializeComponent();
            _displayTips = new DisplayDisabledTooltips(MediaWidget_Fill_Panel, ultraToolTipManager1);
        }

        public void Setup(SetupArgs args)
        {
            if (args != null)
            {
                this.MediaJunctionTable = args.MediaJunctionTable;
                this.MediaTable = args.MediaTable;

                if (args.MediaJunctionTableDefaultColumn != null)
                {
                    this.MediaJunctionTableDefaultColumn = args.MediaJunctionTableDefaultColumn.ColumnName;
                }

                if (args.MediaJunctionTableParentRowColumn != null)
                {
                    this.MediaJunctionTableParentRowColumn = args.MediaJunctionTableParentRowColumn.ColumnName;
                }

                this.AllowEditing = args.AllowEditing;
                this.MediaLinkType = args.MediaLinkType;
                this.DocumentLinkTable = args.DocumentLinkTable;
                this.ScannerSettingsType = args.ScannerSettingsType;

            }
        }

        public void LoadMedia(List<DataRow> junctionRows, List<DataRow> documentLinks, int parentRowKey)
        {
            try
            {
                ClearMedia();
                _parentRowKey = parentRowKey;

                DocumentNodeBase selectedNode = null;
                foreach (var jr in junctionRows)
                {
                    DataRow mediaRow = MediaTable.Rows.Find(jr[MEDIA_ID]);
                    try
                    {
                        var mn = AddNewMediaNode(jr, mediaRow);
                        if (!string.IsNullOrEmpty(MediaJunctionTableDefaultColumn) &&
                            Convert.ToBoolean(jr[MediaJunctionTableDefaultColumn]))
                            selectedNode = mn;
                    }
                    catch (Exception exc)
                    {
                        _log.Error(exc, $"Error adding list node for existing media - {MediaLinkType}");
                    }
                }

                foreach (var link in documentLinks)
                {
                    AddNewLinkNode(link);
                }

                //if didn't select the default one then just select the first one
                if (selectedNode == null && this.tvwMedia.Nodes.Count > 0)
                {
                    selectedNode = this.tvwMedia.Nodes.OfType<DocumentNodeBase>().FirstOrDefault();
                }

                if (selectedNode != null)
                {
                    //select node to fire all selection events NOTE: tvw will not allow selection if not enabled
                    selectedNode.Select();

                    if (!String.IsNullOrEmpty(MediaJunctionTableDefaultColumn))
                        selectedNode.CheckedState = CheckState.Checked;
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
                this._parentRowKey = null;
                this.tvwMedia.Nodes.ForEachNode(n => n.Dispose());
                this.tvwMedia.Nodes.Clear();

                if (this.picPartImage.Image is Image)
                    ((Image) this.picPartImage.Image).Dispose();

                this.picPartImage.Image = null;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error clearing media.");
            }
        }

        private MediaNode AddNewMediaNode(DataRow junctionRow, DataRow mediaRow)
        {
            if (mediaRow == null || junctionRow == null)
            {
                return null;
            }

            _log.Debug("Adding new media node " + mediaRow[MEDIA_NAME]);

            //add to Media Tree
            var mn = new MediaNode(junctionRow, mediaRow, this);
            this.tvwMedia.Nodes.Add(mn);
            return mn;
        }

        private DocumentLinkNode AddNewLinkNode(DataRow linkRow)
        {
            try
            {
                if (linkRow == null)
                {
                    return null;
                }
                else
                {
                    int documentID = Convert.ToInt32(linkRow[LINK_INFO_ID]);
                    var node = new DocumentLinkNode(linkRow, Documents.DocumentUtilities.GetFileName(documentID));
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

            //create new row
            var mediaRow = MediaTable.NewRow();
            mediaRow[MEDIA_NAME] = name;
            mediaRow[MEDIA_FILE_NAME] = fName;
            mediaRow[MEDIA_FILE_EXT] = ext;
            mediaRow[MEDIA_DATA] = bytes;
            MediaTable.Rows.Add(mediaRow);

            //add relation of part to this media
            DataRow junctionRow = null;
            if (String.IsNullOrEmpty(MediaJunctionTableDefaultColumn))
                junctionRow = MediaJunctionTable.Rows.Add(_parentRowKey, mediaRow[MEDIA_ID]);
            else
                junctionRow = MediaJunctionTable.Rows.Add(_parentRowKey, mediaRow[MEDIA_ID], true);

            var mn = AddNewMediaNode(junctionRow, mediaRow);

            if (!string.IsNullOrEmpty(MediaJunctionTableDefaultColumn))
            {
                mn.CheckedState = CheckState.Checked;
            }
            Settings.Default.PartManagerMediaDirectory = Path.GetFullPath(fileName);

            int.TryParse(mediaRow[MEDIA_ID].ToString(), out var mediaId);

            FileAdded?.Invoke(this, new FileAddedEventArgs
            {
                FilePath = fileName,
                MediaId = mediaId
            });
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

                int parentRowID;

                if (MediaLinkType == Documents.LinkType.Receiving && !_parentRowKey.HasValue)
                {
                    // Use special value that will be replaced when Receiving saves.
                    parentRowID = DWOS.UI.Documents.DocumentUtilities.RECEIVING_ID;
                }
                else if (MediaLinkType == Documents.LinkType.Part && !_parentRowKey.HasValue)
                {
                    // Use special value that will be replaced when Receiving part saves.
                    parentRowID = DWOS.UI.Documents.DocumentUtilities.RECEIVING_PART_ID;
                }
                else
                {
                    parentRowID = _parentRowKey.GetValueOrDefault();
                }

                if (docs != null)
                {
                    foreach (var doc in docs)
                    {
                        var linkRow = DocumentLinkTable.NewRow();
                        linkRow[LINK_INFO_ID] = doc.DocumentInfoID;
                        linkRow[LINK_TYPE] = MediaLinkType;
                        linkRow[LINK_KEY] = parentRowID;
                        DocumentLinkTable.Rows.Add(linkRow);
                        AddNewLinkNode(linkRow);
                    }
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error adding new document link.", exc);
            }
        }

        private DataRow[] GetJunctionRows(MediaNode node)
        {
            if (!_parentRowKey.HasValue)
                return new DataRow[] {};
            else
                return
                    MediaJunctionTable.Select(MEDIA_ID + " = " + node.DataRow[MEDIA_ID] + " AND " +
                                              MediaJunctionTableParentRowColumn + " = " + _parentRowKey);
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

                //create new row
                DataRow mediaRow = MediaTable.NewRow();
                mediaRow[MEDIA_NAME] = mediaName;
                mediaRow[MEDIA_FILE_NAME] = _random.Next(100000, 999999) + "." + fileExt;
                mediaRow[MEDIA_FILE_EXT] = fileExt;
                mediaRow[MEDIA_DATA] = bytes;
                MediaTable.Rows.Add(mediaRow);

                //add relation of part to this media
                DataRow junctionRow = null;
                if (String.IsNullOrEmpty(MediaJunctionTableDefaultColumn))
                    junctionRow = MediaJunctionTable.Rows.Add(_parentRowKey, mediaRow[MEDIA_ID]);
                else
                    junctionRow = MediaJunctionTable.Rows.Add(_parentRowKey, mediaRow[MEDIA_ID], true);

                var mn = AddNewMediaNode(junctionRow, mediaRow);

                if (!String.IsNullOrEmpty(MediaJunctionTableDefaultColumn))
                    mn.CheckedState = CheckState.Checked;

                mn.Select(true);

                int.TryParse(mediaRow[MEDIA_ID].ToString(), out var mediaId);
                FileAdded?.Invoke(this, new FileAddedEventArgs
                {
                    FilePath = string.Empty,
                    MediaId = mediaId
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

                _twain = new Dynamsoft.DotNet.TWAIN.DynamicDotNetTwain(TWAIN_LICENSE);
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

        private static void HandleAddMediaError(Exception exc)
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

                        this.tvwMedia.Nodes.OfType <MediaNode>().ForEach(n =>
                                                                         {
                                                                             n.CheckedState = n == e.TreeNode ? CheckState.Checked : CheckState.Unchecked;

                                                                             var junctionRows = GetJunctionRows(n);
                                                                             junctionRows.ForEach(jr => jr[MediaJunctionTableDefaultColumn] = n.CheckedState == CheckState.Checked);
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

        public class FileAddedEventArgs : EventArgs
        {
            public string FilePath { get; set; }

            public int MediaId { get; set; }
        }

        #endregion

        #region Nested type: DocumentNodeBase

        private abstract class DocumentNodeBase : DataNode<DataRow>
        {
            #region Properties

            public abstract Image Thumbnail { get; }

            #endregion

            #region Methods

            public DocumentNodeBase(DataRow cr, string id, string keyPrefix, string name)
                : base(cr, id, keyPrefix, name)
            {
            }

            public abstract void DoubleClick();

            public abstract void ImageClick();

            #endregion
        }

        #endregion

        #region Nested type: MediaNode

        private class MediaNode : DocumentNodeBase
        {
            #region Fields

            private const string KEY_PREFIX = "MEDIA";
            private MediaWidget _widgetInstance;

            #endregion

            #region Properties

            public override Image Thumbnail
            {
                get
                {
                    Image thumbnail;
                    string extension = MediaRow[MEDIA_FILE_EXT].ToString();

                    //if is not an image then just set to that
                    if (!MediaUtilities.IsImageExtension(extension))
                    {
                        thumbnail = MediaUtilities.GetGenericThumbnail(extension);
                    }
                    else //else create a proper thumbnail of the image
                    {
                        thumbnail = MediaUtilities.GetThumbnail(Convert.ToInt32(MediaRow[MEDIA_ID]), extension, GetBytes(MediaRow));
                    }

                    return thumbnail;
                }
            }

            public DataRow MediaRow
            {
                get;
                private set;
            }


            #endregion

            #region Methods

            public MediaNode(DataRow cr, DataRow mediaRow, MediaWidget widgetInstance) : base(cr, mediaRow[MEDIA_ID].ToString(), KEY_PREFIX, mediaRow[MEDIA_NAME].ToString())
            {
                _widgetInstance = widgetInstance;
                MediaRow = mediaRow;
                base.LeftImages.Clear();
                base.LeftImages.Add(MediaUtilities.GetGenericThumbnail(MediaRow[MEDIA_FILE_EXT].ToString()));

                //add the OptionButton if using a Default media id
                if (!String.IsNullOrEmpty(_widgetInstance.MediaJunctionTableDefaultColumn)&& widgetInstance.AllowEditing)
                {
                    Override.NodeStyle = NodeStyle.OptionButton;
                }
            }

            public override void UpdateNodeUI() { Text = MediaRow[MEDIA_NAME].ToString(); }

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
                        tb.FormTextBox.Text = MediaRow[MEDIA_NAME].ToString();
                        tb.FormTextBox.SelectAllText();

                        tb.FormTextBox.Focus();

                        if (tb.ShowDialog(_widgetInstance) == DialogResult.OK)
                        {
                            if (!String.IsNullOrEmpty(tb.FormTextBox.Text))
                            {
                                MediaRow[MEDIA_NAME] = tb.FormTextBox.Text;
                                MediaRow[MEDIA_DATA] = GetBytes(MediaRow); //have to set the bytes so when the row updates it won't wipe this column
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
                    tb.FormTextBox.Text = MediaRow[MEDIA_FILE_EXT].ToString();
                    tb.FormTextBox.SelectAllText();

                    tb.FormTextBox.Focus();

                    if (tb.ShowDialog(_widgetInstance) == DialogResult.OK)
                    {
                        var newFileExtension = tb.FormTextBox.Text;

                        MediaRow[MEDIA_FILE_EXT] = newFileExtension;
                        MediaRow[MEDIA_DATA] = GetBytes(MediaRow); //have to set the bytes so when the row updates it won't wipe this column
                        UpdateNodeUI();
                    }
                }
            }

            public override void ImageClick()
            {
                var openedMedia = MediaUtilities.OpenMedia(Convert.ToInt32(MediaRow[MEDIA_ID]), MediaRow[MEDIA_FILE_EXT].ToString(), GetBytes(MediaRow));

                if (!openedMedia)
                {
                    MessageBoxUtilities.ShowMessageBoxError(
                        "Could not open the selected media file. It may be empty.",
                        "Media");
                }
            }

            private byte[] GetBytes(DataRow row)
            {
                if (row.IsNull(MEDIA_DATA))
                {
                    int mediaID = Convert.ToInt32(row[MEDIA_ID]);
                    byte[] bytes;
                    using (var taMedia = new MediaTableAdapter())
                    {
                        bytes = taMedia.GetMediaStream(mediaID);
                    }

                    //if row has no changes then cache the bytes and set it back to no changes
                    if (row.RowState == DataRowState.Unchanged)
                    {
                        row[MEDIA_DATA] = bytes;
                        row.AcceptChanges();
                    }
                    else
                    {
                        row[MEDIA_DATA] = bytes;
                    }

                    return bytes;
                }

                return (byte[]) row[MEDIA_DATA];
            }

            #endregion
        }

        #endregion

        #region Nested type: DocumentLinkNode

        private class DocumentLinkNode : DocumentNodeBase
        {
            #region Fields

            private const string KEY_PREFIX = "DOC";

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

            #endregion

            #region Methods

            public DocumentLinkNode(DataRow cr, string fileName)
                : base(cr, cr[LINK_ID].ToString(), KEY_PREFIX, fileName)
            {
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
                string filePath = DWOS.UI.Documents.DocumentUtilities.DownloadDocument(Convert.ToInt32(DataRow[LINK_INFO_ID]));

                if (!string.IsNullOrEmpty(filePath))
                {
                    DWOS.Shared.Utilities.FileLauncher.New()
                        .HandleErrorsWith((exception, errorMsg) => MessageBoxUtilities.ShowMessageBoxWarn(errorMsg, "Unable to Start", filePath))
                        .Launch(filePath);
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: SetupArgs

        /// <summary>
        /// Arguments for MediaWidget's Setup method.
        /// </summary>
        public sealed class SetupArgs
        {
            public DataTable MediaJunctionTable { get; set; }

            public DataTable MediaTable { get; set; }

            public DataColumn MediaJunctionTableParentRowColumn { get; set; }

            public DataColumn MediaJunctionTableDefaultColumn { get; set; }

            public bool AllowEditing { get; set; }

            /// <summary>
            /// LinkType to use for document links.
            /// </summary>
            public DWOS.UI.Documents.LinkType MediaLinkType { get; set; }

            /// <summary>
            /// Used by the widget to hold document links.
            /// </summary>
            public DataTable DocumentLinkTable { get; set; }

            /// <summary>
            /// Gets a value indicating the type of scanner settings to use.
            /// </summary>
            public ScannerSettingsType ScannerSettingsType { get; set; }
        }

        #endregion

    }
}