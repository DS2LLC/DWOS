using System;
using System.Windows.Forms;
using NLog;

namespace DWOS.UI.Utilities
{
    public class UsingWaitCursorWpf : IDisposable
    {
        #region Fields

        private System.Windows.UIElement _element;
        private System.Windows.Input.Cursor _previousCursor;

        #endregion

        #region Methods

        public UsingWaitCursorWpf(System.Windows.UIElement element)
        {
            try
            {
                _element = element;
                _previousCursor = System.Windows.Input.Mouse.OverrideCursor;
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                Application.UseWaitCursor = true;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error on using wait cursor.");
            }
        }

        public UsingWaitCursorWpf()
        {
            Application.UseWaitCursor = true;
            Application.DoEvents();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if(_element != null) //WPF
                {
                    _element = null;
                    System.Windows.Input.Mouse.OverrideCursor = _previousCursor;
                    _previousCursor = null;
                    Application.UseWaitCursor = false;
                }
                else
                    Application.UseWaitCursor = false;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on disposing wait cursor.");
            }
        }

        #endregion
    }
}
