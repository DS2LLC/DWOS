using System.Web.Mvc;

namespace DWOS.Portal.Controllers
{
    /// <summary>
    /// MVC controller that bootstraps the single-page web application.
    /// </summary>
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}