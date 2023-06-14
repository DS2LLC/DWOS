using System.Threading.Tasks;
using System.Web.Http;
using DWOS.Portal.Models;

namespace DWOS.Portal.Controllers
{
    /// <summary>
    /// Web API controller for Portal settings.
    /// </summary>
    public class SettingsController : ApiController
    {
        public async Task<PortalSettings> Get()
        {
            return await DataAccess.GetAppSettings();
        }
    }
}