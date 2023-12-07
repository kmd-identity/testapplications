using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using KMD.Identity.TestApplications.OpenID.API.Models.Delegation;
using KMD.Identity.TestApplications.OpenID.API.Repositories.Delegation;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace KMD.Identity.TestApplications.OpenID.API.Services.Delegation
{
    public class DelegationService : IDelegationService
    {
        private readonly IDelegationRepository delegationRepository;

        public DelegationService(IDelegationRepository delegationRepository)
        {
            this.delegationRepository = delegationRepository;
        }

        public AccessDelegation New(IEnumerable<Claim> subjectClaims, string operation)
        {
            var accessDelegation = new AccessDelegation
            {
                AccessDelegationId = Guid.NewGuid(),
                FlowId = Guid.NewGuid(),
                Scope = new AccessDelegationScope
                {
                    OperationName = operation
                },
                Status = AccessDelegationStatus.New,
                UserData = new AccessDelegationUserData
                {
                    Sub = subjectClaims.First(c => c.Type.Equals("sub", StringComparison.InvariantCultureIgnoreCase)).Value
                }
            };

            delegationRepository.StoreAccessDelegation(accessDelegation);

            return accessDelegation;
        }

        public (bool success, string error) StartDelegatingAccess(Guid flowId, string subject)
        {
            var delegation = delegationRepository.FindByFlowId(flowId);
            if (delegation == null) return (false, "Access delegation not exists");

            var result = delegation.StartDelegatingAccess(subject);

            if (!result.success) return result;

            delegationRepository.StoreAccessDelegation(delegation);

            return result;
        }

        public (bool success, string error) FinishDelegatingAccess(Guid flowId, string subject)
        {
            var delegation = delegationRepository.FindByFlowId(flowId);
            if (delegation == null) return (false, "Access delegation not exists");

            var result = delegation.FinishDelegatingAccess(subject);

            if (!result.success) return result;

            delegationRepository.StoreAccessDelegation(delegation);

            return result;
        }

        public (bool success, string error) Revoke(bool isCitizen, bool isCaseWorker, string subject, Guid accessDelegationId)
        {
            var delegation = delegationRepository.GetAccessDelegation(accessDelegationId);
            if (delegation == null) return (false, "Access delegation not exists");

            var result = delegation.Revoke(isCitizen, isCaseWorker, subject);

            if (!result.success) return result;

            delegationRepository.StoreAccessDelegation(delegation);

            return result;
        }
    }
}