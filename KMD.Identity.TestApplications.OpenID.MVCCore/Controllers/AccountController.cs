using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace KMD.Identity.TestApplications.OpenID.MVCCore.Controllers
{
    public class AccountController : Controller
    {
        public async Task Login(string returnUrl = "/", string domainHint = null)
        {
            await HttpContext.ChallengeAsync("AD FS", new AuthenticationProperties() { RedirectUri = returnUrl, Items = { { "domain_hint", domainHint } } });
        }

        [Authorize]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync("AD FS");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        // This endpoint has been configured on KMD Identity.
        // KMD Identity will call this endpoint when:
        //      A: An user has logged out from this application
        //      B: An user has logged out from a different application connected to KMD Identity
        public async Task SingleLogout(string sid, string iss)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
