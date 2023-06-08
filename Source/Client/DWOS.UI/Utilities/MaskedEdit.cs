using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Version of UltraMaskedEdit with different OnMouseDown behavior.
    /// </summary>
    public sealed class MaskedEdit : Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit
    {
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            bool hadFocus = Focused;
            base.OnMouseDown(e);

            if (!hadFocus && (Value == null || string.IsNullOrEmpty(Value.ToString())))
            {
                SelectionStart = FocusedSelectionStart;
                SelectionLength = 0;
            }
        }

        /// <summary>
        /// Gets or sets the SelectionStart value to use for empty, focused MaskedEdit instances.
        /// </summary>
        [Description("If the control's value is empty, clicking the control sets SelectionStart to this value.")]
        [Category("Behavior")]
        public int FocusedSelectionStart
        {
            get;
            set;
        }
    }
}
