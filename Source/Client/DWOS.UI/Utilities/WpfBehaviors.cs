using System;
using System.Windows;
using System.Windows.Controls;
using static Infragistics.Windows.Utilities;
namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Defines behaviors for WPF development.
    /// </summary>
    public static class WpfBehaviors
    {
        #region Fields

        public static readonly DependencyProperty SelectAllOnFocusProperty =
            DependencyProperty.RegisterAttached("SelectAllOnFocus", typeof(bool), typeof(WpfBehaviors),
                new PropertyMetadata(SelectAllonFocusChanged));

        #endregion

        #region Methods

        public static void SetSelectAllOnFocus(UIElement element, bool value)
        {
            element.SetValue(SelectAllOnFocusProperty, value);
        }

        public static bool GetSelectAllOnFocus(UIElement element)
        {
            return element.GetValue(SelectAllOnFocusProperty) as bool? ?? false;
        }

        #endregion

        #region Events

        private static void SelectAllonFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is UIElement element))
            {
                return;
            }

            var oldValue = e.OldValue as bool? ?? false;
            var newValue = e.NewValue as bool? ?? false;

            if (newValue && !oldValue)
            {
                element.GotFocus += Element_GotFocus;
            }
            else if (!newValue && oldValue)
            {
                element.GotFocus -= Element_GotFocus;
            }
        }

        private static void Element_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
            else if (sender is Infragistics.Controls.Editors.XamMaskedInput maskedEditor)
            {
                // Needs Dispatcher.BeginInvoke call to work consistently.
                maskedEditor.Dispatcher.BeginInvoke(new Action(() => maskedEditor.SelectAll()));
            }
            else if (sender is Infragistics.Windows.Editors.XamMaskedEditor winMaskedEditor)
            {
                winMaskedEditor.SelectAll();
            }
            else if (sender is Infragistics.Windows.Editors.XamComboEditor winComboEditor)
            {
                winComboEditor.SelectAll();
            }
            else if (sender is Infragistics.Controls.Editors.XamComboEditor comboEditor)
            {
                // ---HACK---
                // This version of Infragistics's XamComboEditor does not have
                // a SelectAll method. Use its inner text box.
                // https://www.infragistics.com/community/forums/f/ultimate-ui-for-wpf/101155/highlighting-the-text-in-xamcomboeditor-with-mouse-click
                var innerTextBox = GetDescendantFromName(comboEditor, "TextBoxPresenter")
                    as Infragistics.Controls.Editors.Primitives.SpecializedTextBox;

                if (innerTextBox != null)
                {
                    // Needs Dispatcher.BeginInvoke call to work.
                    // This inner text box was potentially created on a
                    // different thread.
                    innerTextBox.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        innerTextBox.SelectAll();
                    }));
                }
            }
        }

        #endregion
    }
}
