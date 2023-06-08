using DWOS.UI.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWOS.UI.Sales.ViewModels
{
    public class TextFieldViewModel : Utilities.ViewModelBase, ICustomFieldViewModel
    {
        private const string DefaultDescription = "No description available.";

        private string _value;

        private TextFieldViewModel(int customFieldId, string name, bool isRequired, string description)
        {
            CustomFieldId = customFieldId;
            Name = name;
            IsRequired = isRequired;

            Description = string.IsNullOrEmpty(description)
                ? DefaultDescription
                : description;
        }

        public static TextFieldViewModel From(CustomField sourceField)
        {
            if (sourceField == null)
            {
                throw new ArgumentNullException(nameof(sourceField));
            }

            return new TextFieldViewModel(sourceField.CustomFieldId, sourceField.Name, sourceField.IsRequired, sourceField.Description)
            {
                Value = sourceField.DefaultValue
            };
        }

        public override string Validate(string propertyName)
        {
            if (propertyName != nameof(Value))
            {
                return null;
            }

            if (string.IsNullOrEmpty(_value) && IsRequired)
            {
                return $"{Name} is required.";
            }

            return null;
        }

        public override string ValidateAll() =>
            Validate(nameof(Value));

        #region ICustomFieldViewModel Members

        public int CustomFieldId { get; }

        public string Name { get; }

        public bool IsRequired { get; }

        public string Description { get; }

        public string Value
        {
            get => _value;
            set => Set(nameof(Value), ref _value, value);
        }

        #endregion
    }
}
