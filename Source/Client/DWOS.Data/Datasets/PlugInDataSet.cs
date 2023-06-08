namespace DWOS.Data.Datasets
{


    public partial class PlugInDataSet
    {
        public const string PLUGIN_FOLDER_NAME = "Plugins";
        public const string PLUGIN_CTX_FILE_NAME = "PluginContext.config";

        public static string GetPluginSubFolderName(PlugInRow plugin) { return "PlugIn" + "_" + plugin.PlugInID; }
    }
}
