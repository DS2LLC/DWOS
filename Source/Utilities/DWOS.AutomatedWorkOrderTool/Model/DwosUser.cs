using System.Collections.Generic;

namespace DWOS.AutomatedWorkOrderTool.Model
{
    /// <summary>
    /// Represents a DWOS user.
    /// </summary>
    public class DwosUser
    {
        public const string MASTER_LIST_ROLE = "AWOT.MasterList";
        public const string SHIPPING_MANIFEST_ROLE = "AWOT.CreateOrders";

        #region Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Department { get; set; }

        public int? MediaId { get; set; }

        public List<string> Roles { get; set; }

        #endregion

        #region Methods

        public bool IsInRole(string role) => Roles?.Contains(role) ?? false;

        #endregion
    }
}
