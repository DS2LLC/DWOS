using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DWOS.PluginSample
{
    public class SamplePlugin: DWOS.Plugin.IPluginCommand
    {
        public void Execute(Plugin.PluginContext context)
        {
            MessageBox.Show("Hello World");
        }

        public string Name
        {
            get { return "Sample PlugIn"; }
        }

        public string Description
        {
            get { return "Sample Plugin Hello World"; }
        }

        public string SecurityRoleID
        {
            get { return null; }
        }

        public System.Drawing.Image Image
        {
            get
            {
                return Properties.Resources.Puzzle_32;
            }
        }
    }
}
