using DWOS.Data;
using DWOS.UI.Utilities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DWOS.UI.Admin
{
    /// <summary>
    /// Interaction logic for SysproNewCommentFieldDialog.xaml
    /// </summary>
    public partial class SysproNewCommentFieldDialog
    {
        #region Fields

        private const string TYPE_LITERAL = "Literal";
        private const string TYPE_FIELD = "Field";
        private const string TYPE_CUSTOM = "Custom Field";

        #endregion

        #region Methods

        public SysproNewCommentFieldDialog()
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

            var commentTypes = Enum.GetValues(typeof(SysproInvoiceSettings.CommentType))
                .Cast<SysproInvoiceSettings.CommentType>()
                .OrderBy(m => m.ToString());

            foreach (var commentType in commentTypes)
            {
                CommentTypeComboBox.Items.Add(commentType);
            }

            TypeComboBox.Items.Add(TYPE_FIELD);
            TypeComboBox.Items.Add(TYPE_LITERAL);
            TypeComboBox.Items.Add(TYPE_CUSTOM);

            PositionComboBox.Items.Add(SysproInvoiceSettings.CommentPosition.AfterEverythingElse);
            PositionComboBox.Items.Add(SysproInvoiceSettings.CommentPosition.AfterStockLine);
        }

        public SysproInvoiceSettings.ICommentField CreateField()
        {
            if (CommentTypeComboBox.SelectedValue == null)
            {
                return null;
            }

            var fieldType = TypeComboBox.SelectedValue?.ToString();
            var commentType = (SysproInvoiceSettings.CommentType)CommentTypeComboBox.SelectedValue;
            var position = (SysproInvoiceSettings.CommentPosition)PositionComboBox.SelectedValue;

            SysproInvoiceSettings.ICommentField field = null;
            switch (fieldType)
            {
                case TYPE_LITERAL:
                    field = new SysproInvoiceSettings.CommentLiteral
                    {
                        Type = commentType,
                        Value = LiteralTextBox.Text,
                        Position = position
                    };

                    break;
                case TYPE_CUSTOM:
                    field = new SysproInvoiceSettings.CommentCustomField
                    {
                        Type = commentType,
                        Format = FormatTextBox.Text,
                        TokenName = CustomFieldTextBox.Text,
                        Position = position
                    };

                    break;
                case TYPE_FIELD:
                    if (FieldComboBox.SelectedValue != null)
                    {
                        field = new SysproInvoiceSettings.CommentField
                        {
                            Type = commentType,
                            Format = FormatTextBox.Text,
                            Dwos = (SysproInvoiceSettings.FieldType)FieldComboBox.SelectedValue,
                            Position = position
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
            FormatLabel.Visibility = Visibility.Collapsed;
            FormatTextBox.Visibility = Visibility.Collapsed;

            switch (fieldType)
            {
                case TYPE_LITERAL:
                    LiteralLabel.Visibility = Visibility.Visible;
                    LiteralTextBox.Visibility = Visibility.Visible;
                    break;
                case TYPE_CUSTOM:
                    CustomFieldLabel.Visibility = Visibility.Visible;
                    CustomFieldTextBox.Visibility = Visibility.Visible;
                    FormatLabel.Visibility = Visibility.Visible;
                    FormatTextBox.Visibility = Visibility.Visible;
                    break;
                case TYPE_FIELD:
                    FieldLabel.Visibility = Visibility.Visible;
                    FieldComboBox.Visibility = Visibility.Visible;
                    FormatLabel.Visibility = Visibility.Visible;
                    FormatTextBox.Visibility = Visibility.Visible;
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
