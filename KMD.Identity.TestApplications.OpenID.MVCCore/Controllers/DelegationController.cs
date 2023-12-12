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
using KMD.Identity.TestApplications.OpenID.MVCCore.Models;
using KMD.Identity.TestApplications.OpenID.MVCCore.Models.Audit;
using System.Reflection;

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

        public async Task<IActionResult> IndexDelegated()
        {
            TempData["Error"] = User.Claims.FirstOrDefault(c => c.Type.Equals("DelegationError", StringComparison.InvariantCultureIgnoreCase))?.Value;
            TempData["DelegationMessage"] = User.Claims.FirstOrDefault(c => c.Type.Equals("DelegationMessage", StringComparison.InvariantCultureIgnoreCase))?.Value;

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Index()
        {
            var model = new DelegationViewModel();

            if (User.HasRole("Citizen"))
            {
                model.DelegatedAccess = (await ApiGet<ApiCallResult<AccessDelegation[]>>("/api/delegation/delegatedbysubject")).Result;
            }
            else if (User.HasRole("CaseWorker"))
            {
                model.DelegatedAccess = (await ApiGet<ApiCallResult<AccessDelegation[]>>("/api/delegation/delegated")).Result;

                var delegationSub = User.Claims.FirstOrDefault(c => c.Type.Equals("DelegationSub", StringComparison.InvariantCultureIgnoreCase))?.Value;
                if (!string.IsNullOrWhiteSpace(delegationSub))
                {
                    model.DelegationSubject = delegationSub;
                }
            }

            var delegationErrors = new List<string>();
            if (TempData["Error"] != null)
            {
                delegationErrors.Add((string)TempData["Error"]);
            }

            var delegationMessages = new List<string>();
            if (TempData["DelegationMessage"] != null)
            {
                delegationMessages.Add((string)TempData["DelegationMessage"]);
            }
            model.Errors = delegationErrors.ToArray();
            model.Messages = delegationMessages.ToArray();
            
            return View(model);
        }

        [HttpGet]
        public async Task DelegateAccess(string returnUrl = "/Delegation/IndexDelegated")
        {
            //start access delegation
            var result = await ApiPost<ApiCallResult<AccessDelegation>>("/api/delegation/delegateaccess", new { });

            if (!result.Success)
            {
                TempData["Error"] = result.Error;
                HttpContext.Response.Redirect(Url.Action(nameof(Index)));
                return;
            }

            await HttpContext.ChallengeAsync("AD FS", new AuthenticationProperties() { RedirectUri = returnUrl, Items = { { "flowid", $"{result.Result.FlowId}" } } });
        }

        [HttpGet]
        public async Task RevokeAccess(Guid accessDelegationId, string returnUrl = "/Delegation/IndexDelegated")
        {
            var result = await ApiPost<ApiCallResult>($"/api/delegation/revoke?accessDelegationId={accessDelegationId}", new { });

            if (!result.Success)
            {
                TempData["Error"] = result.Error;
                HttpContext.Response.Redirect(Url.Action(nameof(Index)));
                return;
            }

            if (User.HasRole("Citizen"))
            {
                HttpContext.Response.Redirect(Url.Action(nameof(Index)));
                return;
            }

            if (User.HasRole("CaseWorker"))
            {
                await HttpContext.ChallengeAsync("AD FS", new AuthenticationProperties() { RedirectUri = returnUrl });
            }
        }
        
        [HttpGet]
        public async Task Act(Guid accessDelegationId, string returnUrl = "/Delegation/IndexDelegated")
        {
            //start acting
            var result = await ApiPost<ApiCallResult<AccessDelegationAct>>($"/api/delegation/act?accessDelegationId={accessDelegationId}", new { });

            if (!result.Success)
            {
                TempData["Error"] = result.Error;
                HttpContext.Response.Redirect(Url.Action(nameof(Index)));
                return;
            }

            await HttpContext.ChallengeAsync("AD FS", new AuthenticationProperties() { RedirectUri = returnUrl, Items = { { "flowid", $"{result.Result.FlowId}" } } });
        }

        [HttpGet]
        public async Task<IActionResult> AuditInformation(Guid accessDelegationId)
        {
            var result = await ApiGet<ApiCallResult<AuditItem[]>>($"/api/audit/delegation?accessDelegationId={accessDelegationId}");

            return PartialView(result);
        }

        [HttpGet]
        public async Task<IActionResult> Pay()
        {
            var result = await ApiGet<ApiCallResult<decimal[]>>("/api/financial/pay");

            return PartialView(result);
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
