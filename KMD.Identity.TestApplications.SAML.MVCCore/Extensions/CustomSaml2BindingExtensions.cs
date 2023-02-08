using System.ServiceModel.Security.Tokens;
using ITfoxtec.Identity.Saml2;
using Microsoft.AspNetCore.Mvc;

namespace KMD.Identity.TestApplications.SAML.MVCCore.Extensions
{
    public static class CustomSaml2BindingExtensions
    {
        public static IActionResult
            ToActionResultWithDomainHint(this Saml2RedirectBinding binding, string domainHint) =>
            string.IsNullOrEmpty(domainHint)
                ? new RedirectResult($"{binding.RedirectLocation.OriginalString}")
                : new RedirectResult($"{binding.RedirectLocation.OriginalString}&domain_hint={domainHint}");

        public static IActionResult ToActionResultWithParameters(this Saml2RedirectBinding binding, string domainHint, string uniloginLOA)
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

            return new RedirectResult($"{binding.RedirectLocation.OriginalString}{parametersToAdd}");
        }
    }
}
