using System.IO;
using System.Linq;
using System.Reflection;
using DWOS.Data;
using DWOS.Plugin;
using DWOS.Shared.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DWOS.Server.Admin.SettingsPanels
{
    /// <summary>
    /// Interface for adding/removing DWOS Client plugins.
    /// </summary>
    public partial class SettingsPlugins : UserControl, ISettingsPanel
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private Data.Datasets.PlugInDataSet.PlugInDataTable _plugins;

        #endregion

        #region Properties

        public string PanelKey
        {
            get { return "PlugIn"; }
        }

        public bool IsValid
        {
            get { return true; }
        }

        #endregion

        #region Methods

        public SettingsPlugins()
        {
            InitializeComponent();
        }

        #endregion Methods

        #region ISettingsPanel Members

        public bool Editable
        {
            get { return true; } 
        }

        public void LoadData()
        {
            try
            {
                Enabled = this.Editable;
                
                using(var taPlugin = new Data.Datasets.PlugInDataSetTableAdapters.PlugInTableAdapter())
                {
                    _plugins = taPlugin.GetPluginSummary();
                    
                    foreach (var plugin in _plugins)
                    {
                        AddPlugin(plugin);
                    }
                }
            }
            catch (Exception exc)
            {
                const string errorMsg = "Error loading settings.";
                _log.Error(exc, errorMsg);
            }
        }

        private void AddPlugin(Data.Datasets.PlugInDataSet.PlugInRow row)
        {
            var size = row.IsFileZipNull() ? new Data.Datasets.PlugInDataSetTableAdapters.PlugInTableAdapter().GetFileSize(row.PlugInID) : row.FileZip.LongLength;
            var fileSize = FileSystem.ConvertBytesToString(size.GetValueOrDefault());

            var item = new Infragistics.Win.UltraWinListView.UltraListViewItem(row.DisplayName, new object[] { row.IsDescriptionNull() ? "" : row.Description, row.IsSecurityRoleNull() ? null : row.SecurityRole, fileSize });
            item.Key = row.PlugInID.ToString();
            item.Appearance.Image = Properties.Resources.Puzzle_16;
            item.Tag = row;
            lvwPlugins.Items.Add(item);
        }

        private void DeletePlugin(Infragistics.Win.UltraWinListView.UltraListViewItem item)
        {
            try
            {
                if (item != null)
                {
                    lvwPlugins.Items.Remove(item);

                    var row = item.Tag as Data.Datasets.PlugInDataSet.PlugInRow;

                    if (row != null)
                        row.Delete();
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error deleting plug in.");
            }
        }

        public void SaveData()
        {
            try
            {
                using (var taPlugin = new Data.Datasets.PlugInDataSetTableAdapters.PlugInTableAdapter())
                {
                    taPlugin.Update(_plugins);
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error saving plugins.");
            }
        }

        private void AddPlugin()
        {
            try
            {
                var fd = new OpenFileDialog();
                fd.Filter = "Zip Files(*.zip)|*.zip";

                if (fd.ShowDialog() == DialogResult.OK)
                {
                    //copy zip to temp folder
                    var tempFolder = Path.Combine(Path.GetTempPath(), "DWOS_Plugin_" + new Random().Next(100000, 999999));
                    var tempFileName = Path.Combine(tempFolder, Path.GetFileName(fd.FileName));
                    Directory.CreateDirectory(tempFolder);

                    File.Copy(fd.FileName, tempFileName);

                    //extract file contents
                    var zipFolder = Ionic.Zip.ZipFile.Read(tempFileName);
                    zipFolder.ExtractAll(tempFolder);

                    //inspect for dll's to find plugin interface
                    foreach (var file in Directory.GetFiles(tempFolder, "*.dll"))
                    {
                        if (file.Contains("DWOS.Plugin.dll")) //skip
                            continue;

                        var assembly = Assembly.LoadFrom(file);
                        var types = GetLoadableTypes(assembly);

                        foreach (var ti in types)
                        {
                            if (ti.GetInterface("IPluginCommand") != null)
                            {
                                var pluginInstance = Activator.CreateInstance(ti) as IPluginCommand;

                                if (pluginInstance != null)
                                {
                                    var plugin = _plugins.NewPlugInRow();

                                    plugin.DisplayName = pluginInstance.Name;
                                    plugin.SecurityRole = pluginInstance.SecurityRoleID;
                                    plugin.Description = pluginInstance.Description;
                                    plugin.Image = pluginInstance.Image != null ? pluginInstance.Image.GetImageAsBytesPng() : null;
                                    plugin.FullName = ti.FullName;
                                    plugin.ExternalExecution = true;
                                    plugin.PluginFileName = Path.GetFileName(file);
                                    plugin.FileZip = File.ReadAllBytes(fd.FileName);

                                    _plugins.AddPlugInRow(plugin);
                                    AddPlugin(plugin);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error adding new plugin.");
            }
        }

        public static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        #endregion

        #region Events

        private void btnAdd_Click(object sender, EventArgs e) { AddPlugin(); }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(lvwPlugins.SelectedItems.Count == 1)
                DeletePlugin(lvwPlugins.SelectedItems[0]);
        }
        
        private void lvwPlugins_ItemSelectionChanged(object sender, Infragistics.Win.UltraWinListView.ItemSelectionChangedEventArgs e) { btnDelete.Enabled = lvwPlugins.SelectedItems.Count == 1; }

        #endregion
    }
}
