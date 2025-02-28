using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using KMD.Identity.TestApplications.OpenID.MVCCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
            using var httpClient = new HttpClient();
            // If you have set options.SaveTokens = true (in Startup.cs) then you can retrieve access_token as stated below
            //var rawAccessToken = await HttpContext.GetTokenAsync("AD FS", "access_token");

            // Retrieving access_token from session because that's how we stored it in OnTokenResponseReceived in Startup.cs
            var rawAccessToken = HttpContext.Session.GetString("access_token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", rawAccessToken);

            ////-------------------------------------------------------------------------------------
            ////-------------------------------------------------------------------------------------
            //// FIRST PART - call API A
            ////-------------------------------------------------------------------------------------
            ////-------------------------------------------------------------------------------------
            var response = await httpClient.GetAsync(new Uri(new Uri(Configuration["Security:ApiUrl"]!), "/api/claims"));
            var apiCallResult = await response.Content.ReadAsStringAsync();

            ViewBag.ResultApi = apiCallResult;

            ////-------------------------------------------------------------------------------------
            ////-------------------------------------------------------------------------------------
            //// SECOND PART - call API B from API A using on behalf of
            //// NOTE - This application is forbidden to call API B directly
            ////-------------------------------------------------------------------------------------
            ////-------------------------------------------------------------------------------------
            var responseOnBehalfOf = await httpClient.GetAsync(new Uri(new Uri(Configuration["Security:ApiUrl"]!), "/api/claims/onbehalfof"));
            var apiCallResultOnBehalfOf = await responseOnBehalfOf.Content.ReadAsStringAsync();

            ViewBag.ResultApiOnBehalfOf = apiCallResultOnBehalfOf;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
