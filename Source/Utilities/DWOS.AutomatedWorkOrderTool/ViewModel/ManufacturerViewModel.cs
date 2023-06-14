using DWOS.AutomatedWorkOrderTool.Model;

namespace DWOS.AutomatedWorkOrderTool.ViewModel
{
    public class ManufacturerViewModel
    {
        public string Name { get; set; }

        internal static ManufacturerViewModel From(AwotDataSet.d_ManufacturerRow manufacturer)
        {
            if (manufacturer == null)
            {
                return null;
            }

            return new ManufacturerViewModel
            {
                Name = manufacturer.ManufacturerID
            };
        }
    }
}
