namespace DWOS.AutomatedWorkOrderTool.Model
{
    public class CustomField
    {
        public int CustomFieldId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string DefaultValue { get; set; }

        public bool IsRequired { get; set; }

        public bool IsVisible { get; set; }
    }
}
