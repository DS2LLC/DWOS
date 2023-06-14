using System;
using System.Threading.Tasks;
using System.Web.Http;
using DWOS.Portal.Filters;
using DWOS.Portal.Models;
using DWOS.Portal.Utilities;

namespace DWOS.Portal.Controllers
{
    [DwosAuthorize]
    public class OrderApprovalMediaController : ApiController
    {
        public async Task<IHttpActionResult> GetMedia(int id)
        {
            var user = await DataAccess.GetUser(RequestContext.Principal.LoginIdentity());
            var media = await DataAccess.GetApprovalMedia(id, user);

            if (media == null)
            {
                return NotFound();
            }

            return Ok(media);
        }
    }
}
