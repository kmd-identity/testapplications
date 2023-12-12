using System;
using System.Collections.Generic;
using System.Linq;

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

        public List<AccessDelegationAct> Acts { get; set; } = new List<AccessDelegationAct>();

        public OperationResult<AccessDelegation> StartDelegatingAccess(string subject)
        {
            if (subject != UserData.Sub) return OperationResult<AccessDelegation>.Fail("Not your access delegation");
            if (Status != AccessDelegationStatus.New) return OperationResult<AccessDelegation>.Fail($"Invalid status {Status} for operation");
            
            Status = AccessDelegationStatus.WaitingForDelegation;

            return OperationResult<AccessDelegation>.Pass(this);
        }

        public OperationResult FinishDelegatingAccess(string subject)
        {
            if (subject != UserData.Sub) return OperationResult.Fail("Not your access delegation");
            if (Status != AccessDelegationStatus.WaitingForDelegation) return OperationResult.Fail($"Invalid status {Status} for operation");
            
            Status = AccessDelegationStatus.Delegated;
            DelegatedAt = DateTime.UtcNow;

            return OperationResult.Pass();
        }

        public OperationResult Revoke(bool isCitizen, bool isCaseWorker, string subject)
        {
            if (isCitizen)
            {
                if (subject != UserData.Sub) return OperationResult.Fail("Not your access delegation");
                if (Status == AccessDelegationStatus.Revoked) return OperationResult.Fail($"It's already revoked");

                Status  = AccessDelegationStatus.Revoked;
                RevokedAt = DateTime.UtcNow;

                return OperationResult.Pass();
            }
            else if (isCaseWorker)
            {
                if (Status == AccessDelegationStatus.Revoked) return OperationResult.Fail($"It's already revoked");

                Status = AccessDelegationStatus.Revoked;
                RevokedAt = DateTime.UtcNow;

                return OperationResult.Pass();
            }
            else
            {
                return OperationResult.Fail("No access for this method");
            }
        }

        public OperationResult<AccessDelegationAct> StartActing(string actor)
        {
            if(Status != AccessDelegationStatus.Delegated) return OperationResult<AccessDelegationAct>.Fail("Not ready for delegation");

            var act = new AccessDelegationAct
            {
                AccessDelegationActId = Guid.NewGuid(),
                ActingStartedTimestamp = DateTime.UtcNow,
                FlowId = Guid.NewGuid(),
                Status = AccessDelegationActingStatus.Started,
                Who = actor
            };

            Acts.Add(act);

            return OperationResult<AccessDelegationAct>.Pass(act);
        }

        public OperationResult<AccessDelegation> FinishActing(string actor)
        {
            if (Status != AccessDelegationStatus.Delegated) return OperationResult<AccessDelegation>.Fail("Not ready for delegation");

            var act = Acts.FirstOrDefault(a => a.Who == actor && !a.ActingFinishedTimestamp.HasValue);

            if(act == null) return OperationResult<AccessDelegation>.Fail("Cannot finish acting when it was already completed or not started");

            act.ActingFinishedTimestamp = DateTime.UtcNow;
            
            return OperationResult<AccessDelegation>.Pass(this);
        }
    }
}