using System.Text.Json.Serialization;

namespace KMD.Identity.TestApplications.OpenID.API.Models.CustomClaims
{
    public class UserForbiddenResult: ICustomClaimsResult
    {
        [JsonPropertyName("http://access.userforbidden")]
        public bool UserForbidden { get; set; } = true;
    }
}
