using System;
using System.Collections.Generic;
using KMD.Identity.TestApplications.OpenID.API.Models.Delegation;

namespace KMD.Identity.TestApplications.OpenID.API.Repositories.Delegation
{
    public interface IDelegationRepository
    {
        void StoreAccessDelegation(AccessDelegation accessDelegation);
        AccessDelegation GetAccessDelegation(Guid accessDelegationId);
        AccessDelegation FindByFlowId(Guid flowId);
        IEnumerable<AccessDelegation> FindBySubject(string subject);
        void RemoveAccessDelegation(Guid accessDelegationId);
    }
}