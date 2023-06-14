using System.Web.Http;

namespace DWOS.Services
{
    public class IndexController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok();
        }
    }
}
