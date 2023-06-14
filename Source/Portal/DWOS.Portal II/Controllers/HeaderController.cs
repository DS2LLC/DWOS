using System.Web.Http;
using DWOS.Portal.Models;

namespace DWOS.Portal.Controllers
{
    /// <summary>
    /// Web API controller for header information.
    /// </summary>
    public class HeaderController : ApiController
    {
        public HeaderData GetHeaderData()
        {
            return DataAccess.HeaderData();
        }
    }
}