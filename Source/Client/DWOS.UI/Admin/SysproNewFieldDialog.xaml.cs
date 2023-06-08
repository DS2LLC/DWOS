using DWOS.Data;
using DWOS.UI.Utilities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DWOS.UI.Admin
{
    /// <summary>
    /// Interaction logic for SysproNewFieldDialog.xaml
    /// </summary>
    public partial class SysproNewFieldDialog
    {
        #region Fields

        private const string TYPE_LITERAL = "Literal";
        private const string TYPE_FIELD = "Field";
        private const string TYPE_CUSTOM = "Custom Field";

        #endregion

        #region Methods

        public SysproNewFieldDialog()
        {
            InitializeComponent();
            Icon = Properties.Resources.Settings_32.ToWpfImage();

            var fieldTypes = Enum.GetValues(typeof(SysproInvoiceSettings.FieldType))
                .Cast<SysproInvoiceSettings.FieldType>()
                .OrderBy(m => m.ToString());

            foreach (var fieldType in fieldTypes)
            {
                FieldComboBox.Items.Add(fieldType);
            }

            FieldComboBox.SelectedIndex = 0;

            TypeComboBox.Items.Add(TYPE_FIELD);
            TypeComboBox.Items.Add(TYPE_LITERAL);
            TypeComboBox.Items.Add(TYPE_CUSTOM);
        }

        public SysproInvoiceSettings.IField CreateField()
        {
            var fieldType = TypeComboBox.SelectedValue?.ToString();

            SysproInvoiceSettings.IField field = null;
            switch (fieldType)
            {
                case TYPE_LITERAL:
                    field = new SysproInvoiceSettings.Literal
                    {
                        Syspro = SysproTextBox.Text,
                        Value = LiteralTextBox.Text
                    };

                    break;
                case TYPE_CUSTOM:
                    field = new SysproInvoiceSettings.CustomField
                    {
                        Syspro = SysproTextBox.Text,
                        TokenName = CustomFieldTextBox.Text
                    };

                    break;
                case TYPE_FIELD:
                    if (FieldComboBox.SelectedValue != null)
                    {
                        field = new SysproInvoiceSettings.Field
                        {
                            Syspro = SysproTextBox.Text,
                            Dwos = (SysproInvoiceSettings.FieldType)FieldComboBox.SelectedValue
                        };
                    }

                    break;
            }

            return field;
        }

        #endregion

        #region Events

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fieldType = TypeComboBox.SelectedValue?.ToString();
            FieldLabel.Visibility = Visibility.Collapsed;
            FieldComboBox.Visibility = Visibility.Collapsed;
            CustomFieldLabel.Visibility = Visibility.Collapsed;
            CustomFieldTextBox.Visibility = Visibility.Collapsed;
            LiteralLabel.Visibility = Visibility.Collapsed;
            LiteralTextBox.Visibility = Visibility.Collapsed;

            switch (fieldType)
            {
                case TYPE_LITERAL:
                    LiteralLabel.Visibility = Visibility.Visible;
                    LiteralTextBox.Visibility = Visibility.Visible;
                    break;
                case TYPE_CUSTOM:
                    CustomFieldLabel.Visibility = Visibility.Visible;
                    CustomFieldTextBox.Visibility = Visibility.Visible;
                    break;
                case TYPE_FIELD:
                    FieldLabel.Visibility = Visibility.Visible;
                    FieldComboBox.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        #endregion
    }
}
