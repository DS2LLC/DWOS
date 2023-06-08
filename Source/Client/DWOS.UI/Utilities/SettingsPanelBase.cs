using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;

namespace DWOS.UI.Utilities
{
    public partial class SettingsPanelBase : UserControl
    {
        private bool _loaded;

        protected SettingsPanelBase() { InitializeComponent(); }

        public PopupControlContainerTool SettingTool { get; set; }

        protected virtual void SaveSettings() { }

        protected virtual void LoadSettings() { }

        public void Initialze(PopupControlContainerTool settingTool)
        {
            SettingTool = settingTool;

            if(!this._loaded)
            {
                if(SettingTool != null)
                {
                    SettingTool.BeforeToolDropdown += toolbarManager_BeforeToolDropdown;
                    SettingTool.AfterToolCloseup += toolbarManager_AfterToolCloseup;

                    FindForm().FormClosing += SettingsPanelBase_FormClosing;
                }

                this._loaded = true;
            }
        }

        private void SettingsPanelBase_FormClosing(object sender, FormClosingEventArgs e) { SaveSettings(); }

        private void toolbarManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if(e.Tool.Key == SettingTool.Key)
                LoadSettings();
        }

        private void toolbarManager_AfterToolCloseup(object sender, ToolDropdownEventArgs e)
        {
            if(e.Tool.Key == SettingTool.Key)
                SaveSettings();
        }
    }
}