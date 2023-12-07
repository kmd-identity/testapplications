using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using KMD.Identity.TestApplications.OpenID.MVCCore.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using KMD.Identity.TestApplications.OpenID.MVCCore.Models.Delegation;

namespace KMD.Identity.TestApplications.OpenID.MVCCore.Controllers
{
    [Authorize]
    public class DelegationController : Controller
    {
        private readonly IConfiguration configuration;

        public DelegationController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AccessDelegationsViewModel();

            if (User.HasRole("Citizen"))
                model.DelegatedAccess = await ApiGet<AccessDelegationViewModel[]>("/api/delegation/delegatedbysubject");

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> RevokeAccess(Guid accessDelegationId, string returnUrl = "/Delegation")
        {
            if (User.HasRole("Citizen"))
            {
                await ApiPost<OperationResult>($"/api/delegation/revoke?accessDelegationId={accessDelegationId}", new { });

                return RedirectToAction(nameof(Index));
            }
            else if (User.HasRole("CaseWorker") && User.HasClaim(c => c.Type == "flowid"))
            {
                await ApiPost<OperationResult>("/api/delegation/revoke", new { });
                await HttpContext.ChallengeAsync("AD FS", new AuthenticationProperties() { RedirectUri = returnUrl });
            }

            return null;
        }
        
        [HttpGet]
        public async Task DelegateAccess(string returnUrl = "/Delegation")
        {
            if (!User.HasRole("Citizen")) throw new SecurityException("Not Citizen");

            //start access delegation
            var result = await ApiPost<AccessDelegationViewModel>("/api/delegation/delegateaccess", new { });
            
            await HttpContext.ChallengeAsync("AD FS", new AuthenticationProperties() { RedirectUri = returnUrl, Items = { { "flowid", $"{result.FlowId}" } } });
        }

        private async Task<T> ApiGet<T>(string path)
        {
            using var httpClient = new HttpClient();
            var rawAccessToken = HttpContext.Session.GetString("access_token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", rawAccessToken);
            
            var response = await httpClient.GetAsync(GetApiUrl(path));
            var apiCallResult = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<T>(apiCallResult);
        }

        private async Task<T> ApiPost<T>(string path, object data)
        {
            using var httpClient = new HttpClient();
            var rawAccessToken = HttpContext.Session.GetString("access_token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", rawAccessToken);

            var response = await httpClient.PostAsync(GetApiUrl(path), new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
            var apiCallResult = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(apiCallResult);
        }

        private string GetApiUrl(string path)
        {
            //needs to be more sophisticated
            return configuration["Security:ApiUrl"].Replace("/api/claims", path);
        }
    }
}
