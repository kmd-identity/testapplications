namespace KMD.Identity.TestApplications.OpenID.API.Models.Delegation
{
    public class AccessDelegationScope
    {
        // Who (all, actor, role, etc.)
        // What (all, api, endpoint, etc.)
        // When (always, 10 minutes, etc.)

        public string OperationName { get; set; }
    }
}