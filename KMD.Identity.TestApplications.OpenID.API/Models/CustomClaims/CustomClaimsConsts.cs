namespace KMD.Identity.TestApplications.OpenID.API.Models.CustomClaims
{
    public static class CustomClaimsConsts
    {
        public static class Inbound
        {
            public const string IdentityProviderClaim = "identityprovider";
            public const string IdentityProviderSourceClaim = "identityprovidersource";
            public const string SubjectClaim = "sub";
            public const string EmailClaim = "email";
        }

        public static class Outbound
        {
            public const string PersistenNameIdFormat = "urn:oasis:names:tc:SAML:2.0:nameid-format:persistent";
            public const string EmailNameIdFormat = "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress";
        }
    }
}