using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KMD.Identity.TestApplications.OpenID.MVCCore.Controllers
{
    public class AccountController : Controller
    {
        public async Task Login(string loginMethod = "Redirect", string returnUrl = "/", string domainHint = null, string loginHint = null)
        {
            var authProperties = new AuthenticationProperties 
            { 
                RedirectUri = returnUrl, 
                Items = 
                { 
                    { "domain_hint", domainHint }, 
                    { "login_hint", loginHint },
                    { "AuthenticationMethod", loginMethod },
                } 
            };
            
            await HttpContext.ChallengeAsync("AD FS", authProperties);

            //To use the Unilogin connection with a different flow than the default (one factor), 
            //add a query string parameter, to read more about this go to our Wiki, example below, additional code in Startup.cs: 
            //await HttpContext.ChallengeAsync("AD FS", new AuthenticationProperties() { RedirectUri = returnUrl, Items = { { "domain_hint", domainHint }, {"unilogin_loa","TwoFactor"} } });
        }

        [Authorize]
        public async Task Logout(string logoutMethod = "Post")
        {
            var authProperties = new AuthenticationProperties();
           
            authProperties.Items.Add("AuthenticationMethod", logoutMethod);

            await HttpContext.SignOutAsync("AD FS", authProperties);
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
