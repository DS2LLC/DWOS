using System.Collections.Generic;
using System.Threading.Tasks;
using DWOS.AutomatedWorkOrderTool.Model;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    public interface IPartManager
    {
        Task ValidateAsync(int customerId, IEnumerable<MasterListPart> parts);

        PartProcessingInfo Decode(AwotDataSet dsAwot, string ospCode);
    }
}
