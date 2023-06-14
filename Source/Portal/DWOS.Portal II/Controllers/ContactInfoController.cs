using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using DWOS.Portal.Filters;
using DWOS.Portal.Models;
using DWOS.Portal.Utilities;

namespace DWOS.Portal.Controllers
{
    /// <summary>
    /// Web API controller for contact information.
    /// </summary>
    [DwosAuthorize]
    public class ContactInfoController : ApiController
    {
        public async Task<ContactInfo> GetContactInfo()
        {
            var user = await DataAccess.GetUser(RequestContext.Principal.LoginIdentity());
            return await DataAccess.GetContactInfo(user.ContactId);
        }

        [HttpPut]
        public async Task<IHttpActionResult> UpdateContactInfo([FromBody]ContactInfo info)
        {
            if (info == null)
            {
                return BadRequest();
            }

            var user = await DataAccess.GetUser(RequestContext.Principal.LoginIdentity());

            if (user.ContactId != info.ContactId)
            {
                // Users can only update their own contact information
                return StatusCode(HttpStatusCode.Forbidden);
            }

            await DataAccess.UpdateContactInfo(info);
            return Ok();
        }
    }
}
