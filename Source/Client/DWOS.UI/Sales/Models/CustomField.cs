using System;
using System.Collections.Generic;

namespace DWOS.UI.Sales.Models
{
    public class CustomField
    {
        public int CustomFieldId { get; }

        public string Name { get; }

        public bool IsRequired { get; }

        public string Description { get; }

        public string DefaultValue { get; }

        public List<string> ListItems { get; }

        private CustomField(int customFieldId, string name, bool isRequired, string description, string defaultValue, List<string> listItems)
        {
            CustomFieldId = customFieldId;
            Name = name;
            IsRequired = isRequired;
            DefaultValue = defaultValue;
            Description = description;
            ListItems = listItems ?? throw new ArgumentNullException(nameof(listItems));
        }

        public static CustomField NewTextField(int customFieldId, string name, bool isRequired, string description, string defaultValue) =>
            new CustomField(customFieldId, name, isRequired, description, defaultValue, new List<string>());

        public static CustomField NewComboBoxField(int customFieldId, string name, bool isRequired, string description, string defaultValue, List<string> listItems)
        {
            List<string> newListItems;
            if (!string.IsNullOrEmpty(defaultValue) && !listItems.Contains(defaultValue))
            {
                newListItems = new List<string> { defaultValue };
                newListItems.AddRange(listItems);
            }
            else
            {
                newListItems = new List<string>(listItems);
            }

            return new CustomField(customFieldId, name, isRequired, description, defaultValue, listItems);
        }
    }
}
