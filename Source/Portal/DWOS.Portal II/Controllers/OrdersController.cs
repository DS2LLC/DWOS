using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using DWOS.Portal.Filters;
using DWOS.Portal.Models;
using DWOS.Portal.Utilities;
using DWOS.Shared.Utilities;
using NLog;

namespace DWOS.Portal.Controllers
{

    /// <summary>
    /// Web API controller for work orders.
    /// </summary>
    [DwosAuthorize]
    public class OrdersController : ApiController
    {
        public async Task<IEnumerable<OrderSummary>> GetOrders()
        {
            var user = await DataAccess.GetUser(RequestContext.Principal.LoginIdentity());

            IEnumerable<OrderSummary> summaries;

            using (new UsingTimeMe($"Retrieve Orders - ({ string.Join(",", user.AllCustomerIds) })", LogLevel.Info))
            {
                summaries = await DataAccess.GetSummaries(user);
            }

            return summaries;
        }

        public async Task<IHttpActionResult> GetOrder(int id)
        {
            var user = await DataAccess.GetUser(RequestContext.Principal.LoginIdentity());
            var order = await DataAccess.GetOrder(id, user);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
    }
}