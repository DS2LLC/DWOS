using System;
using System.Windows.Forms;
using NLog;

namespace DWOS.UI.Utilities
{
    /// <summary>
    ///     Used within a using to set wait cursor on instantiation and unset wait cursor on disposal.
    /// </summary>
    public class UsingWaitCursor : IDisposable
    {
        #region Fields

        private Form _form;

        #endregion

        #region Methods

        public UsingWaitCursor(Form form)
        {
            try
            {
                _form = form;

                if (_form != null)
                {
                    _form.UseWaitCursor = true;
                    _form.Cursor = Cursors.WaitCursor;
                }
                else
                    Application.UseWaitCursor = true;

                Application.DoEvents();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error on using wait cursor.");
            }
        }

        public UsingWaitCursor()
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
                if (_form != null) //WinForms
                {
                    _form.UseWaitCursor = false;
                    _form.Cursor = Cursors.Default;
                    _form = null;
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