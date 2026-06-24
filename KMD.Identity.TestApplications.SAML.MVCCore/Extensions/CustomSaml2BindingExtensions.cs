using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Cryptography;
using ITfoxtec.Identity.Saml2.Http;
using KMD.Identity.TestApplications.SAML.MVCCore.Config;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Web;

namespace KMD.Identity.TestApplications.SAML.MVCCore.Extensions
{
    public static class CustomSaml2BindingExtensions
    {
        public static Saml2RedirectBinding BindWithParameters(this Saml2RedirectBinding binding, Saml2AuthnRequest request, string domainHint = null, string loginHint = null, string accr = null)
        {
            var builder = new UriBuilder(request.Destination);
            var query = HttpUtility.ParseQueryString(builder.Query);

            if (!string.IsNullOrEmpty(domainHint))
            {
                query.Add("domain_hint", domainHint);
            }
            if (!string.IsNullOrEmpty(loginHint))
            {
                query.Add("login_hint", loginHint);
            }
            if (!string.IsNullOrEmpty(accr))
            {
                query.Add("accr", accr);
            }

            builder.Query = query.ToString();
            request.Destination = builder.Uri;

            return binding.Bind(request);
        }

        public static Saml2PostBinding BindWithParameters(this Saml2PostBinding binding, Saml2AuthnRequest request, string domainHint = null, string loginHint = null, string accr = null)
        {
            var builder = new UriBuilder(request.Destination);
            var query = HttpUtility.ParseQueryString(builder.Query);

            if (!string.IsNullOrEmpty(domainHint))
            {
                query.Add("domain_hint", domainHint);
            }
            if (!string.IsNullOrEmpty(loginHint))
            {
                query.Add("login_hint", loginHint);
            }
            if (!string.IsNullOrEmpty(accr))
            {
                query.Add("accr", accr);
            }

            builder.Query = query.ToString();
            request.Destination = builder.Uri;

            return binding.Bind(request);
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
