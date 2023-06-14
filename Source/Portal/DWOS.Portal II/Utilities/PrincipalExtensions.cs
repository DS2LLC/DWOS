using DWOS.Portal.Filters;
using System;
using System.Security.Principal;

namespace DWOS.Portal.Utilities
{
    public static class PrincipalExtensions
    {
        /// <summary>
        /// Gets the <see cref="DwosAuthorizeAttribute.LoginIdentity"/> of the
        /// current principal.
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static DwosAuthorizeAttribute.LoginIdentity LoginIdentity(this IPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return principal.Identity as DwosAuthorizeAttribute.LoginIdentity;
        }
    }
}