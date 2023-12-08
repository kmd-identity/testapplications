using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using KMD.Identity.TestApplications.OpenID.API.Models;
using KMD.Identity.TestApplications.OpenID.API.Models.Delegation;
using KMD.Identity.TestApplications.OpenID.API.Repositories.Delegation;

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

        public OperationResult<AccessDelegation> StartDelegatingAccess(Guid flowId, string subject)
        {
            var delegation = delegationRepository.FindByFlowId(flowId);
            if (delegation == null) return OperationResult<AccessDelegation>.Fail("Access delegation not exists");

            var result = delegation.StartDelegatingAccess(subject);

            if (!result.Success) return result;

            delegationRepository.StoreAccessDelegation(delegation);

            return result;
        }

        public OperationResult FinishDelegatingAccess(Guid flowId, string subject)
        {
            var delegation = delegationRepository.FindByFlowId(flowId);
            if (delegation == null) return OperationResult.Fail("Access delegation not exists");

            var result = delegation.FinishDelegatingAccess(subject);

            if (!result.Success) return result;

            delegationRepository.StoreAccessDelegation(delegation);

            return result;
        }

        public OperationResult Revoke(bool isCitizen, bool isCaseWorker, string subject, Guid accessDelegationId)
        {
            var delegation = delegationRepository.GetAccessDelegation(accessDelegationId);
            if (delegation == null) return OperationResult.Fail("Access delegation not exists");

            var result = delegation.Revoke(isCitizen, isCaseWorker, subject);

            if (!result.Success) return result;

            delegationRepository.StoreAccessDelegation(delegation);

            return result;
        }

        public OperationResult<AccessDelegationAct> StartActing(Guid accessDelegationId, string actor)
        {
            var delegation = delegationRepository.GetAccessDelegation(accessDelegationId);
            if (delegation == null) return OperationResult<AccessDelegationAct>.Fail("Access delegation not exists");

            var result = delegation.StartActing(actor);

            if (!result.Success) return result;

            delegationRepository.StoreAccessDelegation(delegation);

            return result;
        }

        public OperationResult FinishActing(Guid flowId, string actor)
        {
            var delegation = delegationRepository.FindByActFlowId(flowId);
            if (delegation == null) return OperationResult.Fail("Access delegation not exists");

            var result = delegation.FinishActing(actor);

            if (!result.Success) return result;

            delegationRepository.StoreAccessDelegation(delegation);

            return result;
        }
    }
}