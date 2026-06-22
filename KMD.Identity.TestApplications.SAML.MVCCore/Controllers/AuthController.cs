using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using KMD.Identity.TestApplications.SAML.MVCCore.Config;
using KMD.Identity.TestApplications.SAML.MVCCore.Extensions;
using KMD.Identity.TestApplications.SAML.MVCCore.Identity;
using KMD.Identity.TestApplications.SAML.MVCCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace KMD.Identity.TestApplications.SAML.MVCCore.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        const string relayStateReturnUrl = "ReturnUrl";
        private readonly ExtendedSaml2Configuration config;

        public AuthController(IOptions<ExtendedSaml2Configuration> configAccessor)
        {
            config = configAccessor.Value;
        }

        public IActionResult Login(string loginMethod = null, string returnUrl = null, string domainHint = null, string loginHint = null)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Content("~/"));
            }

            if (string.Equals(loginMethod, "Post", StringComparison.OrdinalIgnoreCase))
            {
                var binding = new Saml2PostBinding();
                binding.SetRelayStateQuery(new Dictionary<string, string> { { relayStateReturnUrl, returnUrl ?? Url.Content("~/") } });
                return binding.Bind(new Saml2AuthnRequest(config)).ToActionResultWithParameters(domainHint, loginHint: loginHint);
            }
            else
            {
                var binding = new Saml2RedirectBinding();
                binding.SetRelayStateQuery(new Dictionary<string, string> { { relayStateReturnUrl, returnUrl ?? Url.Content("~/") } });
                return binding.Bind(new Saml2AuthnRequest(config)).ToActionResultWithParameters(domainHint, loginHint: loginHint);
            }

            //To use the Unilogin connection with a different flow than the default (one factor), 
            //add a query string parameter, to read more about this go to our Wiki, example below: 
            //return binding.Bind(new Saml2AuthnRequest(config)).ToActionResultWithParameters(domainHint, uniloginLOA: "TwoFactor");
        }

        [HttpPost]
        public async Task<IActionResult> AssertionConsumerService()
        {
            var binding = new Saml2PostBinding();
            var saml2AuthnResponse = new Saml2AuthnResponse(config);

            binding.ReadSamlResponse(Request.ToGenericHttpRequest(), saml2AuthnResponse);
            if (saml2AuthnResponse.Status != Saml2StatusCodes.Success)
            {
                return View("Error", new SamlErrorViewModel
                {
                    Error = $"{saml2AuthnResponse.Status}",
                    Description = saml2AuthnResponse.StatusMessage
                });
            }

            binding.UnbindWithMetadataRefreshOnValidationError(
                Request.ToGenericHttpRequest(),
                saml2AuthnResponse,
                config
            );

            await saml2AuthnResponse.CreateSession(HttpContext, claimsTransform: (claimsPrincipal) => ClaimsTransform.Transform(claimsPrincipal));

            var relayStateQuery = binding.GetRelayStateQuery();
            var returnUrl = relayStateQuery.ContainsKey(relayStateReturnUrl) ? relayStateQuery[relayStateReturnUrl] : Url.Content("~/");
            return Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Content("~/"));
        }

        public async Task<IActionResult> Logout(string logoutMethod = null)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect(Url.Content("~/"));
            }

            var saml2LogoutRequest = await new Saml2LogoutRequest(config, User).DeleteSession(HttpContext);

            if (string.Equals(logoutMethod, "Redirect", StringComparison.OrdinalIgnoreCase))
            {
                var binding = new Saml2RedirectBinding();
                return binding.Bind(saml2LogoutRequest).ToActionResult();
}
            else
            {
                var binding = new Saml2PostBinding();
                return binding.Bind(saml2LogoutRequest).ToActionResult();
            }
        }

        public IActionResult LoggedOut()
        {
            var binding = new Saml2PostBinding();
            var saml2LogoutResponse = new Saml2LogoutResponse(config);
            binding.Unbind(Request.ToGenericHttpRequest(), saml2LogoutResponse);

            if (saml2LogoutResponse.Status != Saml2StatusCodes.Success)
            {
                throw new AuthenticationException($"SAML Logout Response status: {saml2LogoutResponse.Status}");
            }

            return Redirect(Url.Content("~/"));
        }

        public async Task<IActionResult> SingleLogout()
        {
            Saml2StatusCodes status;
            var requestBinding = new Saml2PostBinding();
            var logoutRequest = new Saml2LogoutRequest(config, User);
            try
            {
                requestBinding.Unbind(Request.ToGenericHttpRequest(), logoutRequest);
                status = Saml2StatusCodes.Success;
                await logoutRequest.DeleteSession(HttpContext);
            }
            catch
            {
                status = Saml2StatusCodes.RequestDenied;
            }

            var responsebinding = new Saml2PostBinding();
            responsebinding.RelayState = requestBinding.RelayState;
            var saml2LogoutResponse = new Saml2LogoutResponse(config)
            {
                InResponseToAsString = logoutRequest.IdAsString,
                Status = status,
            };
            return responsebinding.Bind(saml2LogoutResponse).ToActionResult();
        }
    }
}
