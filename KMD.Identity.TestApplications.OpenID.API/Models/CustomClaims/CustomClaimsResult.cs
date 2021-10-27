namespace KMD.Identity.TestApplications.OpenID.API.Models.CustomClaims
{
    /// <summary>
    /// This model represents standard result, you can extend it with custom claims base on the rules specified in the github WIKI.
    /// </summary>
    public class CustomClaimsResult: ICustomClaimsResult
    {
        public string TestSingleValue { get; set; }

        public string[] TestMultipleValues { get; set; }
    }
}
