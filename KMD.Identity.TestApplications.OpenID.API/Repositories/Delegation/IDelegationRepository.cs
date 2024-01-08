using System;
using System.Collections.Generic;
using KMD.Identity.TestApplications.OpenID.API.Models;
using KMD.Identity.TestApplications.OpenID.API.Models.Delegation;

namespace KMD.Identity.TestApplications.OpenID.API.Repositories.Delegation
{
    public interface IDelegationRepository
    {
        OperationResult<AccessDelegation> StoreAccessDelegation(AccessDelegation accessDelegation);
        AccessDelegation GetAccessDelegation(Guid accessDelegationId);
        AccessDelegation FindByFlowId(Guid flowId);
        AccessDelegation FindByActFlowId(Guid flowId);
        IEnumerable<AccessDelegation> FindBySubject(string subject);
        IEnumerable<AccessDelegation> FindAll();
        
        //technical method
        void CleanupAll();
        //technical method
        IEnumerable<Guid> Cleanup(string subject);
    }
}