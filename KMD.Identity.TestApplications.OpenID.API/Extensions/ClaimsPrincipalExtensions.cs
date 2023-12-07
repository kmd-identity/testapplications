using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace KMD.Identity.TestApplications.OpenID.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool HasRole(this ClaimsPrincipal principal, string roleName)
        {
            return principal.Claims.HasRole(roleName);
        }
        
        public static bool HasRole(this IEnumerable<Claim> claims, string roleName)
        {
            return claims.Any(c => c.Type.Equals("Role", StringComparison.InvariantCultureIgnoreCase)
                                             && c.Value.Equals(roleName, StringComparison.InvariantCultureIgnoreCase));
        }

        public static string GetSubject(this ClaimsPrincipal principal)
        {
            return principal.Claims.Single(c => c.Type == "sub").Value;
        }

    }
}
