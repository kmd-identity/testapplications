using KMD.Identity.TestApplications.OpenID.MVCCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace KMD.Identity.TestApplications.OpenID.MVCCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public IConfiguration Configuration { get; }

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> CallApi()
        {
            using (var httpClient = new HttpClient())
            {
                // If you have set options.SaveTokens = true (in Startup.cs) then you can retrieve access_token as stated below
                //var rawAccessToken = await HttpContext.GetTokenAsync("AD FS", "access_token");

                // Retrieving access_token from session because that's how we stored it in OnTokenResponseReceived in Startup.cs
                var rawAccessToken = HttpContext.Session.GetString("access_token");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", rawAccessToken);
                var response = await httpClient.GetAsync(Configuration["Security:ApiUrl"]);
                var apiCallResult = await response.Content.ReadAsStringAsync();

                ViewBag.Result = apiCallResult;

                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
