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
    }
}
