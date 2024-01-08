using System;
using KMD.Identity.TestApplications.OpenID.API.Models;
using KMD.Identity.TestApplications.OpenID.API.Models.Delegation;

namespace KMD.Identity.TestApplications.OpenID.API.Services.Delegation
{
    public interface IDelegationService
    {
        OperationResult<AccessDelegation> New(string subject, string operation);
        OperationResult<AccessDelegation> StartDelegatingAccess(Guid flowId, string subject);
        OperationResult FinishDelegatingAccess(Guid flowId, string subject);
        OperationResult Revoke(bool isCitizen, bool isCaseWorker, string subject, Guid accessDelegationId);
        OperationResult<AccessDelegationAct> StartActing(Guid accessDelegationId, string actor);
        OperationResult<AccessDelegation> FinishActing(Guid flowId, string actor);
    }
}