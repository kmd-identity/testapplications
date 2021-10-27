using System.Text.Json.Serialization;

namespace KMD.Identity.TestApplications.OpenID.API.Models.CustomClaims
{
    public class CustomNameIdResult : CustomClaimsResult
    {
        [JsonPropertyName("customnameid")]
        public string CustomNameId { get; set; }

        [JsonPropertyName("customnameidformat")]
        public string CustomNameIdFormat { get; set; } = CustomClaimsConsts.Outbound.PersistenNameIdFormat;
    }
}