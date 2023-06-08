using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinTabControl;

namespace DWOS.Utilities.Validation
{
    /// <summary>
    /// Class TabValidationDisplay adds glpyhs to each tab that has an error in it.
    /// </summary>
    public class TabValidationDisplay : IValidationSummary, IUIElementDrawFilter
    {
        private static Image Error_Image = DWOS.UI.Properties.Resources.Error_16;

        private Dictionary <UltraTab, int> _tabErrorCount = new Dictionary <UltraTab, int>();
        private UltraTabControl TabControl { get; set; }

        public TabValidationDisplay(UltraTabControl tabControl)
        {
            TabControl = tabControl;
            TabControl.DrawFilter = this;
        }

        public void Reset()
        {
            _tabErrorCount.Clear();
        }

        public void StatusUpdate(DisplayValidator validator, bool isValid)
        {
            var control = validator.Validator.ValidatingControl;
            var tab = FindTab(control);
            
            if(tab == null)
                return;

            if(!_tabErrorCount.ContainsKey(tab))
                _tabErrorCount.Add(tab, 0);

            _tabErrorCount[tab] = _tabErrorCount[tab] + 1;
        }

        public void Complete()
        {
            //force controls to redraw
            TabControl.Invalidate();
        }

        private UltraTab FindTab(Control control)
        {
            if (control == null || control.Parent == null)
                return null;

            if (control.Parent is UltraTabPageControl)
                return ((UltraTabPageControl)control.Parent).Tab;

            return FindTab(control.Parent);
        }

        public bool DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
        {
            if (drawPhase == DrawPhase.AfterDrawElement)
            {
                if (drawParams.Element is Infragistics.Win.UltraWinTabs.TabItemUIElement)
                {
                    var tab = drawParams.Element as Infragistics.Win.UltraWinTabs.TabItemUIElement;
                    
                    if(tab.TabItem is UltraTab)
                    {
                        if(_tabErrorCount.ContainsKey((UltraTab) tab.TabItem) && _tabErrorCount[(UltraTab) tab.TabItem] > 0)
                        {
                            Rectangle rect = tab.Rect;
                            drawParams.AppearanceData.ImageHAlign = HAlign.Right;

                            rect.X = rect.Right - 20;
                            rect.Y = 3;
                            rect.Width = 16;
                            rect.Height = 16;
                            drawParams.DrawImage(Error_Image, rect, true, new System.Drawing.Imaging.ImageAttributes() {});
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
        {
            return DrawPhase.AfterDrawElement;
        }
    }
}