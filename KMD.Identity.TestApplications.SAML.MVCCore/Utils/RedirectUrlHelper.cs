using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using System;

namespace KMD.Identity.TestApplications.SAML.MVCCore.Utils
{
    public static class RedirectUrlHelper
    {
        private static readonly string defaultReturnUrl = "/";
        public static string GetRedirectUrl(string returnUrl, HttpContext httpContext)
        {
            if (returnUrl == defaultReturnUrl) return defaultReturnUrl;
            return IsReturnUrlValid(returnUrl, httpContext) ? returnUrl : defaultReturnUrl;
        }
        private static bool IsReturnUrlValid(string returnUrl, HttpContext httpContext)
        {
            try
            {
                var returnUrlDomain = GetDomainFromHost(new Uri(returnUrl).Host);
                var applicationDomain = GetDomainFromHost(httpContext.Request.GetUri().Host);
                if (string.Equals(applicationDomain, returnUrlDomain))
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
        private static string GetDomainFromHost(string host)
        {
            return host.Substring(host.LastIndexOf('.', host.LastIndexOf('.') - 1) + 1);
        }
    }
}