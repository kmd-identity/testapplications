using System;

namespace KMD.Identity.TestApplications.OpenID.MVCCore.Models.Delegation
{
    public class AccessDelegation
    {
        public Guid AccessDelegationId { get; set; }

        public Guid FlowId { get; set; }

        public AccessDelegationStatus Status { get; set; }

        public AccessDelegationUserData UserData { get; set; }

        public AccessDelegationScope Scope { get; set; }

        public DateTime? DelegatedAt { get; set; }

        public DateTime? RevokedAt { get; set; }
    }
}