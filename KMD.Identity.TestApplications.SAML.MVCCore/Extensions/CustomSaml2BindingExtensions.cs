using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Cryptography;
using ITfoxtec.Identity.Saml2.Http;
using KMD.Identity.TestApplications.SAML.MVCCore.Config;
using Microsoft.AspNetCore.Mvc;
using System;

namespace KMD.Identity.TestApplications.SAML.MVCCore.Extensions
{
    public static class CustomSaml2BindingExtensions
    {
        public static IActionResult ToActionResultWithParameters(this Saml2RedirectBinding binding, string domainHint, string loginHint = null, string uniloginLOA = null)
        {
            var parametersToAdd = "";
            if (!string.IsNullOrEmpty(domainHint))
            {
                parametersToAdd  = $"&domain_hint={domainHint}";
            }
            if (!string.IsNullOrEmpty(uniloginLOA))
            {
                parametersToAdd += $"&unilogin_loa={uniloginLOA}";
            }
            if (!string.IsNullOrEmpty(loginHint))
            {
                parametersToAdd += $"&login_hint={loginHint}";
            }

            return new RedirectResult($"{binding.RedirectLocation.OriginalString}{parametersToAdd}");
        }

        public static void UnbindWithMetadataRefreshOnValidationError(this Saml2PostBinding binding, HttpRequest request, Saml2AuthnResponse saml2AuthnResponse, ExtendedSaml2Configuration saml2Configuration)
        {
            try
            {
                binding.Unbind(request, saml2AuthnResponse);
            }
            catch (InvalidSignatureException)
            {
                var metadataUrl = new Uri(saml2Configuration.IdPMetadataUrl);
                saml2Configuration.ReloadMetadata(metadataUrl);
                binding.Unbind(request, saml2AuthnResponse);
            }
        }
    }
}
