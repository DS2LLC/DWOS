using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using DWOS.Data;
using DWOS.Portal.Filters;
using DWOS.Portal.Models;
using DWOS.Portal.Utilities;
using NLog;

namespace DWOS.Portal.Controllers
{
    /// <summary>
    /// Web API controller for order approvals.
    /// </summary>
    [DwosAuthorize]
    public class OrderApprovalsController : ApiController
    {
        #region Methods

        public async Task<List<OrderApprovalSummary>> GetApprovals()
        {
            var user = await DataAccess.GetUser(RequestContext.Principal.LoginIdentity());
            return await DataAccess.GetApprovalSummaries(user);
        }

        public async Task<IHttpActionResult> GetApproval(int id)
        {
            var user = await DataAccess.GetUser(RequestContext.Principal.LoginIdentity());
            var approval = await DataAccess.GetApproval(id, user);

            if (approval == null)
            {
                return NotFound();
            }

            return Ok(approval);
        }

        [HttpPut]
        public async Task<IHttpActionResult> UpdateOrderApproval([FromBody]OrderApproval approval)
        {
            if (approval == null)
            {
                return BadRequest();
            }

            if (!Enum.TryParse<OrderApprovalStatus>(approval.Status, out _))
            {
                // Invalid Status
                return BadRequest();
            }

            var user = await DataAccess.GetUser(RequestContext.Principal.LoginIdentity());
            var order = await DataAccess.GetOrder(approval.OrderId, user);

            if (order == null || !user.AllCustomerIds.Contains(order.CustomerId))
            {
                // Users can only update approvals for their customers
                return StatusCode(HttpStatusCode.Forbidden);
            }

            var originalApproval = await DataAccess.GetApproval(approval.OrderApprovalId, user);

            if (originalApproval == null || originalApproval.OrderId != order.OrderId)
            {
                // Bad request - user attempted to change OrderID
                return BadRequest();
            }

            await DataAccess.UpdateOrderApproval(approval, user);
            var emailSuccess = await SendApprovalNotification(approval);

            if (!emailSuccess)
            {
                return InternalServerError();
            }

            return Ok();
        }

        private async Task<bool> SendApprovalNotification(OrderApproval approval)
        {
            if (approval == null)
            {
                return true;
            }

            var toAddresses = (await DataAccess.GetInternalEmailAddresses(approval.OrderId))
                .Distinct()
                .ToList();

            if (toAddresses.Count > 0)
            {
                var messageInfo = new Messaging.MessageInfo
                {
                    FromAddress = DataAccess.NewApplicationSettings().EmailFromAddress,
                    Subject = $"DWOS - Approval {approval.OrderApprovalId} {approval.Status}",
                    Body = $"Approval {approval.OrderApprovalId} for WO " +
                        $"{approval.OrderId} was {approval.Status} at " +
                        $"{DateTime.Now:hh:mm tt}.",
                    IsHtml = false
                };

                messageInfo.ToAddresses.AddRange(toAddresses);
                return await Messaging.SendMessage(messageInfo);
            }
            else
            {
                LogManager.GetCurrentClassLogger()
                    .Warn($"Not configured to send approval notifications for WO {approval.OrderId}");

                return true;
            }
        }

        #endregion
    }
}
