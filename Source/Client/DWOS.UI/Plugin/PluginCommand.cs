using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DWOS.Data;
using DWOS.Plugin;
using DWOS.Shared.Utilities;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinToolbars;

namespace DWOS.UI.Plugin
{
    /// <summary>
    /// Class PluginManager handles the creation of the plugin commands on the main windows toolbar.
    /// </summary>
    internal class PluginManager
    {
        public const string PLUGIN_RIBBON_TAB_NAME = "PlugIns";

        /// <summary>
        /// Loads the plugins into the toolbar.
        /// </summary>
        /// <param name="commands">The commands.</param>
        /// <param name="toolbar">The toolbar.</param>
        public void LoadPlugins(CommandManager commands, UltraToolbarsManager toolbar)
        {
            try
            {
                using (var taPlugin = new Data.Datasets.PlugInDataSetTableAdapters.PlugInTableAdapter())
                {
                    var pluginTable     = taPlugin.GetPluginSummary();
                    var pluginToolGroup = toolbar.Ribbon.Tabs[PLUGIN_RIBBON_TAB_NAME].Groups[0];

                    foreach (var plugin in pluginTable)
                    {
                        var tool = new ButtonTool("plugin_" + plugin.PlugInID);
                        tool.SharedProps.Caption = plugin.DisplayName;
                        tool.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.Add_32;
                        toolbar.Tools.Add(tool);
                        
                        pluginToolGroup.Tools.AddTool(tool.Key);

                        commands.AddCommand(tool.Key, new PluginCommand(tool, plugin));
                    }

                    pluginToolGroup.Visible = pluginToolGroup.Tools.Count > 0;
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading plugin tools.");
            }
        }
    }

    /// <summary>
    /// Class PluginCommand represents a plugin command that can be executed.
    /// </summary>
    internal class PluginCommand : CommandBase
    {
        #region Fields

        private DWOS.Data.Datasets.PlugInDataSet.PlugInRow _plugIn = null;

        #endregion

        #region Methods

        public PluginCommand(ToolBase tool, DWOS.Data.Datasets.PlugInDataSet.PlugInRow plugIn) : base(tool, plugIn.IsSecurityRoleNull() ? null : plugIn.SecurityRole)
        {
            _plugIn = plugIn;

            if(!plugIn.IsImageNull())
                ((ButtonTool) tool).SharedProps.AppearancesLarge.Appearance.Image = MediaUtilities.GetImage(plugIn.Image);
        }

        public override void OnClick()
        {
            try
            {
                using(new UsingWaitCursor(DWOSApp.MainForm))
                {
                    //save context
                    var ctx = new DWOS.Plugin.PluginContext() {CompanyLogoFilePath = ApplicationSettings.Current.CompanyLogoImagePath, DatabaseConnectionString = ApplicationSettings.Current.ConnectionString, UserId = SecurityManager.Current.UserID, UserName = SecurityManager.Current.UserName};
                    var pluginFolder = Path.Combine(FileSystem.UserAppDataPathVersion(), Data.Datasets.PlugInDataSet.PLUGIN_FOLDER_NAME);

                    if(!Directory.Exists(pluginFolder))
                        Directory.CreateDirectory(pluginFolder);

                    var contextFile = Path.Combine(pluginFolder, Data.Datasets.PlugInDataSet.PLUGIN_CTX_FILE_NAME);

                    if(File.Exists(contextFile))
                        File.Delete(contextFile);

                    FileSystem.SerializeJson(contextFile, ctx);

                    //run plugin
                    if(_plugIn.ExternalExecution)
                    {
                        //DWOS.PluginExecutor
                        var process = new Process();
                        process.StartInfo.FileName = Path.Combine(FileSystem.ApplicationPath(), "DWOS.PluginExecutor.exe");
                        process.StartInfo.Arguments = "ID=" + _plugIn.PlugInID;
                        process.OutputDataReceived += new DataReceivedEventHandler(process_OutputHandler);
                        process.Exited += process_Exited;

                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false; // Set UseShellExecute to false for redirection.
                        process.StartInfo.RedirectStandardOutput = true; // Redirect the standard output
                        process.StartInfo.RedirectStandardInput = true; // Redirect the standard input

                        process.Start();
                        process.BeginOutputReadLine();
                    }
                    else
                    {
                        //run plugin in process
                        var exe = new DWOS.PluginExecutor.DWOSPlugInExecutor();
                        exe.Execute(_plugIn.PlugInID);
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error running plugin command.");
            }
        }

        public override void Dispose()
        {
            _plugIn = null;
            base.Dispose();
        }

        #endregion

        #region Events
        
        private void process_OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            // Collect the sort command output. 
            if (!String.IsNullOrEmpty(outLine.Data))
                _log.Info(outLine.Data);
        }

        private void process_Exited(object sender, EventArgs e)
        {
            try
            {
                _log.Info("Plugin Exited");
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on process exited.");
            }
        }
        
        #endregion
    }

}
