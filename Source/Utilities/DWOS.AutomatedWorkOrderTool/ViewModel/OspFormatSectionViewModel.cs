using DWOS.AutomatedWorkOrderTool.Model;

namespace DWOS.AutomatedWorkOrderTool.ViewModel
{
    public class OspFormatSectionViewModel
    {
        #region Properties

        public int OspFormatId { get; set; }

        public int? OspFormatSectionId { get; set; }

        public RoleType Role { get; set; }

        public string Department { get; set; }

        public int SectionOrder { get; set; }


        // TODO - Trigger updates to display text when set
        public string DisplayText =>
            $"{SectionOrder} - {Department}";

        #endregion
    }
}
