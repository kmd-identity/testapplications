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

        public static IActionResult ToActionResultWithParameters(this Saml2PostBinding binding, string domainHint, string loginHint = null, string uniloginLOA = null)
        {
            var additionalFields = CreateAdditionalHiddenFormFields(domainHint, uniloginLOA, loginHint);
            var responseHtml = CreateResponseForm(binding.PostContent, additionalFields);
            return new ContentResult { ContentType = "text/html", Content = responseHtml };
        }

        private static string CreateAdditionalHiddenFormFields(string domainHint, string uniloginLOA, string loginHint)
        {
            var additionalFields = "";
            if (!string.IsNullOrEmpty(domainHint))
            {
                additionalFields += $"\n        <input type=\"hidden\" name=\"domain_hint\" value=\"{domainHint}\" />";
            }
            if (!string.IsNullOrEmpty(uniloginLOA))
            {
                additionalFields += $"\n        <input type=\"hidden\" name=\"unilogin_loa\" value=\"{uniloginLOA}\" />";
            }
            if (!string.IsNullOrEmpty(loginHint))
            {
                additionalFields += $"\n        <input type=\"hidden\" name=\"login_hint\" value=\"{loginHint}\" />";
            }
            return additionalFields;
        }

        private static string CreateResponseForm(string originalHtml, string additionalFields)
        {
            if (string.IsNullOrEmpty(additionalFields))
            {
                return originalHtml;
            }

            var insertPosition = originalHtml.IndexOf("</form>");
            if (insertPosition < 0)
            {
                throw new InvalidOperationException("Unable to find closing </form> tag in SAML POST binding HTML content. Cannot add additional parameters.");
            }

            return originalHtml.Insert(insertPosition, additionalFields);
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
