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
    }
}
