using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.PlugInDataSetTableAdapters;
using DWOS.Plugin;
using DWOS.Shared.Utilities;
using XSockets.Plugin.Framework;

namespace DWOS.PluginExecutor
{
    /// <summary>
    /// The plugin executor executes plugins within a separate process. This is done to prevent any load or runtime issues with a plugin from de-stabilizing the main DWOS process.
    /// </summary>
    class Program
    {
        public const string ID_ARG = "ID";

        static void Main(string[] args)
        {
            if(args.Length < 1)
                return;

            Console.WriteLine("Loading plugin.");

            var exe = new DWOSPlugInExecutor();
            exe.Execute();
        }
    }

    public class DWOSPlugInExecutor
    {
        public string PluginsFolder
        {
            get
            {
                //get context from file system
                var pluginFolder = Path.Combine(FileSystem.UserAppDataPathVersion(), PlugInDataSet.PLUGIN_FOLDER_NAME);

                //create plug in folder if not there
                if (!Directory.Exists(pluginFolder))
                    Directory.CreateDirectory(pluginFolder);

                return pluginFolder;
            }
        }

        public void Execute()
        {
            var args = Environment.GetCommandLineArgs();
            
            if (args.Length < 2)
                return;

            var id = args[1].Replace("ID=", "");
            int pluginId = 0;
            
            if(!int.TryParse(id, out pluginId))
                return;

            Execute(pluginId);
        }

        public void Execute(int pluginId)
        {
            try
            {
                Console.WriteLine("Initializing plugin '{0}'.".FormatWith(pluginId));

                var ctx = LoadPluginContext();
                var cmd = LoadPlugin(pluginId);

                if(cmd != null)
                {
                    Console.WriteLine("Executing plugin " + cmd.Name);
                    cmd.Execute(ctx);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error executing plugin " + pluginId + ", Error: " + exc.Message);
            }
        }

        private Plugin.PluginContext LoadPluginContext()
        {
            var contextFile = Path.Combine(PluginsFolder, Data.Datasets.PlugInDataSet.PLUGIN_CTX_FILE_NAME);

            object ctxObj = new Plugin.PluginContext();

            if (File.Exists(contextFile))
                FileSystem.DeserializeJson(contextFile, ref ctxObj);

            return ctxObj as Plugin.PluginContext;
        }

        private Plugin.IPluginCommand LoadPlugin(int pluginId)
        {
            try
            {
                using (var taPlugin = new Data.Datasets.PlugInDataSetTableAdapters.PlugInTableAdapter())
                {
                    var pluginTable = taPlugin.GetByPluginNoFile(pluginId);
                    var pluginRow = pluginTable.FindByPlugInID(pluginId);

                    if (pluginRow == null)
                    {
                        Console.WriteLine("Unable to find plugin '{0}' in database.".FormatWith(pluginId));
                        return null;
                    }

                    var pluginFolder = Path.Combine(PluginsFolder, PlugInDataSet.GetPluginSubFolderName(pluginRow));
                    var pluginFileName = Path.Combine(pluginFolder, pluginRow.PluginFileName);

                    //if no file then need to download it
                    if (!File.Exists(pluginFileName))
                    {
                        if (!Directory.Exists(pluginFolder))
                            Directory.CreateDirectory(pluginFolder);

                        //delete any existing files
                        Directory.GetFiles(pluginFolder).ForEach(File.Delete);

                        //download zip package
                        var bytes = taPlugin.GetFileById(pluginId) as byte[];
                        var zipFile = Path.Combine(pluginFolder, "plugin.zip");

                        using (var ms = new MemoryStream(bytes))
                            ms.SaveToFile(zipFile);

                        //unzip files
                        var zipFiles = Ionic.Zip.ZipFile.Read(zipFile);
                        zipFiles.ExtractAll(pluginFolder);
                    }

                    if (!File.Exists(pluginFileName))
                    {
                        Console.WriteLine("Unable to find plugin '{0}' file '{1}' in zip.".FormatWith(pluginId, pluginFileName));
                        return null;
                    }

                    //http://xsockets.net/docs/4/the-plugin-framework
                    Composable.LoadAssembly(pluginFileName);
                    Composable.RegisterExport<Plugin.IPluginCommand>();
                    Composable.ReCompose();

                    return Composable.GetExports<Plugin.IPluginCommand>().FirstOrDefault(t => t.GetType().FullName == pluginRow.FullName);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error finding plugin " + pluginId + ", Error: " + exc.Message);
                return null;
            }
        }
    }

}
