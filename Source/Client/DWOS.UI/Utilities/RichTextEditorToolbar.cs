using System;
using System.ComponentModel;
using System.Windows.Forms;
using Infragistics.Win.FormattedLinkLabel;
using Infragistics.Win.UltraWinToolbars;
using NLog;

namespace DWOS.UI.Utilities
{
    public partial class RichTextEditorToolbar : UserControl
    {
        public RichTextEditorToolbar() { InitializeComponent(); }

        [Browsable(true)]
        public UltraFormattedTextEditor RichTextEditor { get; set; }

        private void ultraToolbarsManager1_ToolClick(object sender, ToolClickEventArgs e)
        {
            try
            {
                switch(e.Tool.Key)
                {
                    case "Cut": // ButtonTool
                        if(RichTextEditor.EditInfo.CanPerformAction(FormattedLinkEditorAction.Cut))
                            RichTextEditor.EditInfo.PerformAction(FormattedLinkEditorAction.Cut);
                        break;

                    case "Copy": // ButtonTool
                        if(RichTextEditor.EditInfo.CanPerformAction(FormattedLinkEditorAction.Copy))
                            RichTextEditor.EditInfo.PerformAction(FormattedLinkEditorAction.Copy);
                        break;

                    case "Paste": // ButtonTool
                        if(RichTextEditor.EditInfo.CanPerformAction(FormattedLinkEditorAction.Paste))
                            RichTextEditor.EditInfo.PerformAction(FormattedLinkEditorAction.Paste);
                        break;

                    case "Font": // ButtonTool
                        if(RichTextEditor.EditInfo.SelectionLength > 0)
                            RichTextEditor.EditInfo.ShowFontDialog();
                        break;

                    case "Image": // ButtonTool
                        RichTextEditor.EditInfo.ShowImageDialog();
                        break;

                    case "Link": // ButtonTool
                        if(RichTextEditor.EditInfo.SelectionLength > 0)
                            RichTextEditor.EditInfo.ShowLinkDialog();
                        break;

                    case "Bold": // ButtonTool
                        if(RichTextEditor.EditInfo.CanPerformAction(FormattedLinkEditorAction.ToggleBold))
                            RichTextEditor.EditInfo.PerformAction(FormattedLinkEditorAction.ToggleBold);
                        break;

                    case "Italics": // ButtonTool
                        if(RichTextEditor.EditInfo.CanPerformAction(FormattedLinkEditorAction.ToggleItalics))
                            RichTextEditor.EditInfo.PerformAction(FormattedLinkEditorAction.ToggleItalics);
                        break;

                    case "Underline": // ButtonTool
                        if(RichTextEditor.EditInfo.CanPerformAction(FormattedLinkEditorAction.ToggleUnderline))
                            RichTextEditor.EditInfo.PerformAction(FormattedLinkEditorAction.ToggleUnderline);
                        break;
                    case "Clear Style": // ButtonTool
                        RichTextEditor.EditInfo.ClearAllStyleAttributes();
                        break;
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error on rich text editor tool click.");
            }
        }
    }
}