using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KMD.Identity.TestApplications.OpenID.API.Models;
using KMD.Identity.TestApplications.OpenID.API.Models.CustomClaims;
using Microsoft.Extensions.Logging;

namespace KMD.Identity.TestApplications.OpenID.API.Handlers
{
    public class CustomClaimsHandler : ICustomClaimsHandler
    {
        private readonly ILogger<CustomClaimsHandler> logger;

        public CustomClaimsHandler(ILogger<CustomClaimsHandler> logger)
        {
            this.logger = logger;
        }

        public async Task<ICustomClaimsResult> Handle(Claim[] claims)
        {
            logger.LogInformation($"Custom Claims endpoint called at {DateTime.UtcNow}");

            //First part is to determine what claims represents useful information.
            // We can define couple of claims, that will be always here.
            var identityProvider = claims.First(c => c.Type == CustomClaimsConsts.Inbound.IdentityProviderClaim).Value;
            var identityProviderSource = claims.First(c => c.Type == CustomClaimsConsts.Inbound.IdentityProviderSourceClaim).Value;
            var subject = claims.First(c => c.Type == CustomClaimsConsts.Inbound.SubjectClaim).Value;

            logger.LogInformation($"User {subject} has used {identityProvider}/{identityProviderSource} to login");

            //there are also some claims, that you will rely on, but we cannot guarantee that you will get it (i.e. Identity Provider doesn't have this claim or doesn't want to share)
            var email = claims.FirstOrDefault(c => c.Type == CustomClaimsConsts.Inbound.EmailClaim)?.Value;

            //---------------------------------------------------------------------------------------------------------
            //1. If you want to stop the flow for some reason - i.e. user is blocked then you can return http://access.userforbidden claim.
            // When you do email check, then you should always validate the identityProvider or identityProviderSource.
            //  It's a security check, because email can be easily changed by Identity Provider to something matching email from other Identity Provider.
            if (string.IsNullOrWhiteSpace(email))
            {
                logger.LogWarning($"User {subject} is not allowed to access the system");
                return new UserForbiddenResult();
            }

            //---------------------------------------------------------------------------------------------------------
            //2. Normally you would just return custom claims which will be added to the token you will get from KMD Identity.
            if (new Random().Next(100) % 2 == 0)
            {
                return new CustomClaimsResult
                {
                    TestSingleValue = Guid.NewGuid().ToString(),
                    TestMultipleValues = new[]
                    {
                        Guid.NewGuid().ToString(),
                        Guid.NewGuid().ToString()
                    }
                };
            }

            //---------------------------------------------------------------------------------------------------------
            //3. With KMD Identity you have the opportunity to change the NameID that your application will receive.
            // WARNING It's valid only for SAML applications
            return new CustomNameIdResult
            {
                CustomNameId = email,
                CustomNameIdFormat = CustomClaimsConsts.Outbound.EmailNameIdFormat,
                TestSingleValue = Guid.NewGuid().ToString(),
                TestMultipleValues = new[]
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString()
                }
            };
        }
        public async Task<ICustomClaimsResult> ReturnRequireCpr(Claim[] claims)
        {
            //Return require cpr claim for the user to be prompted to enter CPR if logging in using the nemlogin-3-private connection.
            return new RequireCprResult()
            {
                requirecpr = "true"
            };

        }

    }
}