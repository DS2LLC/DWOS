using System.Web.Http;
using DWOS.Portal.Models;
using System.Net.Mail;
using System;
using System.Net;
using System.Threading.Tasks;
using DWOS.Portal.Filters;
using DWOS.Portal.Utilities;
using DWOS.Shared.Utilities;

namespace DWOS.Portal.Controllers
{
    /// <summary>
    /// Web API controller that handles login-related functionality.
    /// </summary>
    public class LoginController : ApiController
    {

        #region Methods

        [ActionName("login")]
        [HttpPost]
        [DwosAuthorize]
        public async Task<IHttpActionResult> Login()
        {
            var user = await DataAccess.GetUser(RequestContext.Principal.LoginIdentity());
            await DataAccess.AddLoginHistory(user);
            return Ok();
        }

        [ActionName("reset_password")]
        [HttpPost]
        public async Task<IHttpActionResult> ResetPassword([FromBody]ContactAddress emailAddress)
        {
            if (emailAddress == null)
            {
                return BadRequest("Invalid email address.");
            }

            try
            {
                new MailAddress(emailAddress.EmailAddress);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid email address.");
            }
            // Create a new password, set it in the database and send it to the user.
            var newPassword = RandomUtils.CreateRandomPassword(10);

            var passwordUpdateSuccess = await DataAccess.UpdatePassword(emailAddress.EmailAddress, newPassword);

            // If user was found, send them an email with their new password.
            if (passwordUpdateSuccess)
            {
                var messageInfo = new Messaging.MessageInfo
                {
                    FromAddress = DataAccess.NewApplicationSettings().EmailFromAddress,
                    Subject = "DWOS Password Recovery",
                    Body = "Your new DWOS password is: " + newPassword,
                    IsHtml = false
                };
                messageInfo.ToAddresses.Add(emailAddress.EmailAddress);
                var emailSuccess = await Messaging.SendMessage(messageInfo);

                if (!emailSuccess)
                {
                    return InternalServerError();
                }
            }

            return Ok();
        }

        [HttpPost]
        [DwosAuthorize]
        public async Task<IHttpActionResult> UpdatePassword([FromBody] PasswordData data)
        {
            if (data == null)
            {
                return BadRequest();
            }

            var user = await DataAccess.GetUser(RequestContext.Principal.LoginIdentity());

            if (!await DataAccess.IsPasswordCorrect(user.Email, data.CurrentPassword))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (await DataAccess.UpdatePassword(user.Email, data.NewPassword))
            {
                return Ok();
            }

            return InternalServerError();
        }

        #endregion

        #region PasswordData

        public class PasswordData
        {
            public string CurrentPassword { get; set; }

            public string NewPassword { get; set; }
        }

        #endregion
    }
}
