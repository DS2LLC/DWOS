using System.Drawing;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.Shared.Utilities;
using DWOS.UI.Admin.Templates;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinListView;
using Infragistics.Win.UltraWinTree;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using NLog;

namespace DWOS.UI.Admin
{
    public partial class TemplateManager : Form
    {
        #region Fields

        public const string PART_TEMPLATE_FILE_TYPE = "PartTemp";

        //private PartsDataset _dsParts;
        //private Data.Datasets.PartsDatasetTableAdapters.TableAdapterManager _taManager;
        private Data.Datasets.PartsDatasetTableAdapters.MediaTableAdapter _taMedia;
        private Data.Datasets.PartsDatasetTableAdapters.ProcessAliasTableAdapter _taProcessAlias;
        private Data.Datasets.PartsDatasetTableAdapters.ProcessTableAdapter _taProcess;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Properties

        public string FilterText { get; set; }
        
        public PartTemplate SelectedTemplate { get; private set; }
       
        private Data.Datasets.PartsDatasetTableAdapters.MediaTableAdapter MediaAdapter
        {
            get { return this._taMedia ?? (this._taMedia = new Data.Datasets.PartsDatasetTableAdapters.MediaTableAdapter()); }
        }

        private Data.Datasets.PartsDatasetTableAdapters.ProcessAliasTableAdapter ProcessAliasAdapter
        {
            get
            {
                return this._taProcessAlias
                       ?? (this._taProcessAlias = new Data.Datasets.PartsDatasetTableAdapters.ProcessAliasTableAdapter());
            }
        }

        private Data.Datasets.PartsDatasetTableAdapters.ProcessTableAdapter ProcessAdapter
        {
            get
            {
                return this._taProcess
                       ?? (this._taProcess = new Data.Datasets.PartsDatasetTableAdapters.ProcessTableAdapter());
            }
        }

        public bool IsManagerMode { get; set; }
        
        #endregion Properties

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateManager"/> class.
        /// </summary>
        public TemplateManager()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        private void LoadData()
        {
            tvwTemplates.Nodes.Clear();

            //load all part templates from media table
            MediaAdapter.ClearBeforeFill = false;
            MediaAdapter.FillByFileTypeWithoutMedia(dsParts.Media, PART_TEMPLATE_FILE_TYPE);

            ProcessAliasAdapter.ClearBeforeFill = false;
            ProcessAliasAdapter.Fill(dsParts.ProcessAlias);

            ProcessAdapter.ClearBeforeFill = false;
            ProcessAdapter.Fill(dsParts.Process);

            var partTemplates = dsParts.Media.Where(m => m.FileExtension == PART_TEMPLATE_FILE_TYPE);
            UltraTreeNode selectedTemplate = null;

            foreach (var partTemplate in partTemplates)
            {
                var node = tvwTemplates.Nodes.Add(partTemplate.MediaID.ToString(), partTemplate.Name);
                node.Override.NodeAppearance.Image = "Template";

                if(FilterText == node.Text)
                    selectedTemplate = node;
            }

            if(selectedTemplate != null)
                selectedTemplate.Select(true);
            else if(tvwTemplates.Nodes.Count > 0)
                tvwTemplates.Nodes[0].Select(true);

            toolbarManager.Tools["Delete"].SharedProps.Enabled = tvwTemplates.SelectedNodes.Count > 0;
            toolbarManager.Tools["Rename"].SharedProps.Enabled = tvwTemplates.SelectedNodes.Count == 1;
        }
        
        private void SaveData()
        {
            var parts = dsParts.Media
                .Where(m => m.RowState == DataRowState.Deleted || m.RowState == DataRowState.Modified)
                .Where(m => m[dsParts.Media.FileExtensionColumn, DataRowVersion.Original].ToString() == PART_TEMPLATE_FILE_TYPE);

            var updateCount = MediaAdapter.Update2(parts);

            _logger.Info($"Saved information for {updateCount} template(s).");
        }

        /// <summary>
        /// Loads the template information.
        /// </summary>
        /// <param name="key">The key.</param>
        private void LoadTemplateInfo(string key)
        {
            try
            {
                SelectedTemplate = null;

                var mediaRow = dsParts.Media.FindByMediaID(Convert.ToInt32(key));

                if (mediaRow != null && mediaRow.IsMediaNull())
                {
                    using(var orderMedia = new Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter())
                    {
                        mediaRow.Media = orderMedia.GetMediaStream(mediaRow.MediaID);
                        mediaRow.AcceptChanges(); //prevent from being updated if we fetched media blob
                    }
                }

                var template = mediaRow != null ? PartTemplate.LoadTemplate(mediaRow.Media) : new PartTemplate();

                tePartName.Text = template.Name;
                teAssembly.Text = template.Assembly;
                teRevision.Text = template.Revision;
                teManufacturer.Text = template.Manufacturer;
                teModel.Text = template.Model;
                teMaterial.Text = template.Material;
                tePartShape.Text = template.Shape;
                numSurfaceArea.Value = template.SurfaceArea;
                numLength.Value = template.Length;
                numWidth.Value = template.Width;
                numHeight.Value = template.Height;
                
                curEachPrice.Value = template.EachPrice;
                curLotPrice.Value = template.LotPrice;
                
                txtNotes.Text = template.Notes;
                chkIsActive.Checked = template.IsActive;

                this.LoadMedia(template.Media);
                this.LoadProcesses(template.ProcessAliasIDs);

                if (mediaRow != null)
                    SelectedTemplate = template;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading template.");
            }
        }

        private void Delete(string templateKey)
        {
            try
            {
                tvwTemplates.SelectedNodes.Clear();

                ResetText();
                ClearMedia();

                //delete row
                var mediaRow = dsParts.Media.FindByMediaID(Convert.ToInt32(templateKey));

                if (mediaRow != null)
                    mediaRow.Delete();

                //delete node
                var node = tvwTemplates.GetNodeByKey(templateKey);

                if (node != null)
                {
                    var sibling = node.GetSibling(NodePosition.Previous) ?? node.GetSibling(NodePosition.Next);

                    if (sibling != null)
                        sibling.Select(true);

                    node.Remove();
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error deleting template.");
            }
        }
        
        /// <summary>
        /// Loads the selected media.
        /// </summary>
        private void LoadSelectedMediaThumbnail(string mediaId)
        {
            try
            {
                if (this.partImage.Image is Image)
                    ((Image)this.partImage.Image).Dispose();

                this.partImage.Image = null;

                var mediaRow = dsParts.Media.FindByMediaID(Convert.ToInt32(mediaId));

                if (mediaRow != null)
                    this.partImage.Image = Data.MediaUtilities.GetThumbnail(mediaRow.MediaID, mediaRow.FileExtension);
                else
                    this.partImage.Image = Properties.Resources.NoImage;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading thumbnail.");
            }
        }

        /// <summary>
        /// Loads the media.
        /// </summary>
        /// <param name="media">The media template.</param>
        private void LoadMedia(List<PartTemplate.MediaTemplate> media)
        {
            try
            {
                ClearMedia();

                if (media != null && media.Count > 0)
                {
                    foreach (var mediaTemplate in media)
                    {
                        var mediaRow = dsParts.Media.FindByMediaID(mediaTemplate.MediaId);

                        if (mediaRow == null)
                        {
                            MediaAdapter.FillByMediaIdWithoutMedia(dsParts.Media, mediaTemplate.MediaId);
                            mediaRow = dsParts.Media.FindByMediaID(mediaTemplate.MediaId);
                        }

                        if (mediaRow != null)
                            tvwMedia.Nodes.Add(new UltraTreeNode(mediaRow.MediaID.ToString(), mediaRow.Name));
                    }

                    if (tvwMedia.Nodes.Count > 0)
                        tvwMedia.Nodes[0].Selected = true; // select the first node
                }
                else
                {
                    // load the default image if there are none associated with the part.
                    this.partImage.Image = Properties.Resources.NoImage;
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading media.");
            }
        }

        /// <summary>
        /// Loads the processes.
        /// </summary>
        /// <param name="processAliasIDs">The process alias ids.</param>
        private void LoadProcesses(List<int> processAliasIDs)
        {
            tvwProcesses.Nodes.Clear();

            if (processAliasIDs != null)
            {
                for (var aliasIndex = 0; aliasIndex < processAliasIDs.Count; ++aliasIndex)
                {
                    var processAliasId = processAliasIDs[aliasIndex];
                    var processAliasRow = dsParts.ProcessAlias.FirstOrDefault(p => p.ProcessAliasID == processAliasId);

                    if (processAliasRow != null)
                    {
                        var process = dsParts.Process.FindByProcessID(processAliasRow.ProcessID);
                        var processName = process?.ProcessName ?? processAliasRow.ProcessID.ToString();

                        tvwProcesses.Nodes.Add(new UltraTreeNode(aliasIndex.ToString(), processName));
                    }
                }
            }
        }

        private void ClearMedia()
        {
            try
            {
                this.tvwMedia.Nodes.ForEachNode(n => n.Dispose());
                this.tvwMedia.Nodes.Clear();

                if (this.partImage.Image is Image)
                    ((Image)this.partImage.Image).Dispose();

                this.partImage.Image = null;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error clearing media.");
            }
        }

        private void DeleteSelected()
        {
            var keys = new List<string>();

            foreach (var sn in tvwTemplates.SelectedNodes)
                keys.Add(sn.Key);

            foreach (var key in keys)
                Delete(key);
        }

        private void RenameSelected()
        {
            try
            {
                if (tvwTemplates.SelectedNodes.Count != 1)
                    return;

                var node = tvwTemplates.SelectedNodes[0];

                using (var form = new TextBoxForm())
                {
                    form.Text = "Edit Name";
                    form.FormLabel.Text = "Name:";
                    form.FormTextBox.Text = node.Text;
                    form.FormTextBox.MaxLength = dsParts.Media.NameColumn.MaxLength;


                    if (form.ShowDialog(this) == DialogResult.OK && !String.IsNullOrWhiteSpace(form.FormTextBox.Text))
                    {
                        var mediaRow = dsParts.Media.FindByMediaID(Convert.ToInt32(node.Key));
                        if (mediaRow != null)
                            node.Text = mediaRow.Name = form.FormTextBox.Text.Trim().Replace("'", "*");
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error renaming selected node.");
            }
        }

        private void DisposeMe()
        {
            _taMedia = null;
        }

        #endregion Methods

        #region Events

        private void TemplateManager_Load(object sender, EventArgs e)
        {
            this.LoadData();

            toolbarManager.Visible = IsManagerMode;
        }

        private void tvwMedia_AfterSelect(object sender, SelectEventArgs e)
        {
            if (e.NewSelections.Count > 0)
                this.LoadSelectedMediaThumbnail(e.NewSelections[0].Key);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.SaveData();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lvwTemplates_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete && tvwTemplates.SelectedNodes.Count > 0)
                {
                    DeleteSelected();
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on template list key up.");
            }
        }

        private void tvwTemplates_AfterSelect(object sender, SelectEventArgs e)
        {
            try
            {
                toolbarManager.Tools["Delete"].SharedProps.Enabled = e.NewSelections.Count > 0;
                toolbarManager.Tools["Rename"].SharedProps.Enabled = e.NewSelections.Count == 1;
            
                if (e.NewSelections.Count > 0)
                    this.LoadTemplateInfo(e.NewSelections[0].Key);
                else
                    this.LoadTemplateInfo("0"); //load blank
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on after template selected.");
            }
        }

        private void toolbarManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "Delete":    // ButtonTool
                    DeleteSelected();
                    break;
                case "Rename":    // ButtonTool
                    RenameSelected();
                    break;

            }
        }

        #endregion Events
    }
}
