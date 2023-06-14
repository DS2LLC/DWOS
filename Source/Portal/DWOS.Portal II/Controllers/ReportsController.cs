using System;
using System.Threading.Tasks;
using System.Web.Http;
using DWOS.Portal.Filters;
using DWOS.Portal.Models;
using DWOS.Portal.Utilities;
using DWOS.Reports;

namespace DWOS.Portal.Controllers
{
    /// <summary>
    ///  Web API controller for reports and documents.
    /// </summary>
    [DwosAuthorize]
    public class ReportsController : ApiController
    {
        [HttpGet]
        public async Task<FileData> OpenOrders()
        {
            var user = await DataAccess.GetUser(RequestContext.Principal.LoginIdentity());
            var reportDate = DateTime.Now;
            var currentOrdersReport = new CurrentOrderStatusReport(reportDate, reportDate, user.AllCustomerIds);

            var bytes = currentOrdersReport.PublishExcelReportToBinary(user.Name);

            return new FileData
            {
                Content = Convert.ToBase64String(bytes),
                Type = "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet"
            };
        }

        [HttpGet]
        public async Task<IHttpActionResult> Coc(int id)
        {
            if (await DataAccess.DoesCocExist(id))
            {
                var cocReport = new COCReport(id);
                var reportData = cocReport.PublishPDFReportToBinary();
                return Ok(new FileData
                {
                    Content = Convert.ToBase64String(reportData),
                    Type = "application/pdf"
                });
            }

            return NotFound();
        }
    }
}
