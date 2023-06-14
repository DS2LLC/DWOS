using System.Web.Mvc;

namespace DWOS.Portal.Controllers
{
    /// <summary>
    /// MVC controller that redirects to the site root.
    /// </summary>
    public class IndexRedirectController : Controller
    {
        public ActionResult Index()
        {
            return RedirectPermanent("~");
        }
    }
}