using System;

namespace KMD.Identity.TestApplications.OpenID.API.Models.Delegation
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

        public (bool success, string error) StartDelegatingAccess(string subject)
        {
            if (subject != UserData.Sub) return (false, "Not your access delegation");
            if (Status != AccessDelegationStatus.New) return (false, $"Invalid status {Status} for operation");
            
            Status = AccessDelegationStatus.WaitingForDelegation;

            return (true, null);
        }

        public (bool success, string error) FinishDelegatingAccess(string subject)
        {
            if (subject != UserData.Sub) return (false, "Not your access delegation");
            if (Status != AccessDelegationStatus.WaitingForDelegation) return (false, $"Invalid status {Status} for operation");
            
            Status = AccessDelegationStatus.Delegated;
            DelegatedAt = DateTime.UtcNow;

            return (true, null);
        }

        public (bool success, string error) Revoke(bool isCitizen, bool isCaseWorker, string subject)
        {
            if (isCitizen)
            {
                if (subject != UserData.Sub) return (false, "Not your access delegation");

                Status  = AccessDelegationStatus.Revoked;
                RevokedAt = DateTime.UtcNow;

                return (true, null);
            }
            else if (isCaseWorker)
            {
                Status = AccessDelegationStatus.Revoked;
                RevokedAt = DateTime.UtcNow;

                return (true, null);
            }
            else
            {
                return (false, "No access for this method");
            }
        }
    }
}