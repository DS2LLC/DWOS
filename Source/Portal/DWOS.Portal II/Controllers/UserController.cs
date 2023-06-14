using System.Web.Http;
using DWOS.Portal.Filters;
using DWOS.Portal.Models;
using System.Threading.Tasks;
using DWOS.Portal.Utilities;

namespace DWOS.Portal.Controllers
{
    /// <summary>
    /// Web API controller for user information.
    /// </summary>
    public class UserController : ApiController
    {
        [DwosAuthorize]
        public async Task<User> GetUser()
        {
            return await DataAccess.GetUser(RequestContext.Principal.LoginIdentity());
        }
    }
}
