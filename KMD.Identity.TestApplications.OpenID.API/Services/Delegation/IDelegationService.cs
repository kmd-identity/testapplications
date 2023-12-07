using System;
using System.Collections.Generic;
using System.Security.Claims;
using KMD.Identity.TestApplications.OpenID.API.Models.Delegation;

namespace KMD.Identity.TestApplications.OpenID.API.Services.Delegation
{
    public interface IDelegationService
    {
        AccessDelegation New(IEnumerable<Claim> subjectClaims, string operation);
        (bool success, string error) StartDelegatingAccess(Guid flowId, string subject);
        (bool success, string error) FinishDelegatingAccess(Guid flowId, string subject);
        (bool success, string error) Revoke(bool isCitizen, bool isCaseWorker, string subject, Guid accessDelegationId);
    }
}