using System;
using System.Linq;
using KMD.Identity.TestApplications.OpenID.API.Models;
using KMD.Identity.TestApplications.OpenID.API.Models.Delegation;
using KMD.Identity.TestApplications.OpenID.API.Repositories.Delegation;
using KMD.Identity.TestApplications.OpenID.API.Services.Audit;

namespace KMD.Identity.TestApplications.OpenID.API.Services.Delegation
{
    public class DelegationService : IDelegationService
    {
        private readonly IDelegationRepository delegationRepository;
        private readonly IAuditService auditService;

        public DelegationService(IDelegationRepository delegationRepository, IAuditService auditService)
        {
            this.delegationRepository = delegationRepository;
            this.auditService = auditService;
        }

        public OperationResult<AccessDelegation> New(string subject, string operation)
        {
            var currentSubjectDelegations = delegationRepository.FindBySubject(subject);
            if(currentSubjectDelegations.Any(s=>s.Status != AccessDelegationStatus.Revoked ))
                return OperationResult<AccessDelegation>.Fail("Only one delegation possible. Revoke previous.");

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
                    Sub = subject
                }
            };

            return delegationRepository.StoreAccessDelegation(accessDelegation);
        }

        public OperationResult<AccessDelegation> StartDelegatingAccess(Guid flowId, string subject)
        {
            var delegation = delegationRepository.FindByFlowId(flowId);
            if (delegation == null) return OperationResult<AccessDelegation>.Fail("Access delegation not exists");
            
            auditService.Add(delegation.AccessDelegationId, subject, $"Access delegation process starting");
            
            var result = delegation.StartDelegatingAccess(subject);

            auditService.Add(delegation.AccessDelegationId, subject, $"Access delegation starting ended with {(result.Success ? "success" : $"error: {result.Error}")}");

            if (!result.Success) return result;

            delegationRepository.StoreAccessDelegation(delegation);

            return result;
        }

        public OperationResult FinishDelegatingAccess(Guid flowId, string subject)
        {
            var delegation = delegationRepository.FindByFlowId(flowId);
            if (delegation == null) return OperationResult.Fail("Access delegation not exists");

            auditService.Add(delegation.AccessDelegationId, subject, $"Access delegation process finishing");

            var result = delegation.FinishDelegatingAccess(subject);

            auditService.Add(delegation.AccessDelegationId, subject, $"Access delegation finishing ended with {(result.Success ? "success" : $"error: {result.Error}")}");

            if (!result.Success) return result;

            delegationRepository.StoreAccessDelegation(delegation);

            return result;
        }

        public OperationResult Revoke(bool isCitizen, bool isCaseWorker, string subject, Guid accessDelegationId)
        {
            var delegation = delegationRepository.GetAccessDelegation(accessDelegationId);
            if (delegation == null) return OperationResult.Fail("Access delegation not exists");

            auditService.Add(delegation.AccessDelegationId, subject, $"Access revocation starting");

            var result = delegation.Revoke(isCitizen, isCaseWorker, subject);

            auditService.Add(delegation.AccessDelegationId, subject, $"Access revocation ended with {(result.Success ? "success" : $"error: {result.Error}")}");

            if (!result.Success) return result;

            delegationRepository.StoreAccessDelegation(delegation);

            return result;
        }

        public OperationResult<AccessDelegationAct> StartActing(Guid accessDelegationId, string actor)
        {
            var delegation = delegationRepository.GetAccessDelegation(accessDelegationId);
            if (delegation == null) return OperationResult<AccessDelegationAct>.Fail("Access delegation not exists");

            auditService.Add(delegation.AccessDelegationId, actor, $"Acting process starting");

            var result = delegation.StartActing(actor);

            auditService.Add(delegation.AccessDelegationId, actor, $"Acting starting ended with {(result.Success ? "success" : $"error: {result.Error}")}");

            if (!result.Success) return result;

            delegationRepository.StoreAccessDelegation(delegation);

            return result;
        }

        public OperationResult<AccessDelegation> FinishActing(Guid flowId, string actor)
        {
            var delegation = delegationRepository.FindByActFlowId(flowId);
            if (delegation == null) return OperationResult<AccessDelegation>.Fail("Access delegation not exists");

            auditService.Add(delegation.AccessDelegationId, actor, $"Acting process finishing");

            var result = delegation.FinishActing(actor);

            auditService.Add(delegation.AccessDelegationId, actor, $"Acting finishing ended with {(result.Success ? "success" : $"error: {result.Error}")}");

            if (!result.Success) return result;

            delegationRepository.StoreAccessDelegation(delegation);

            return result;
        }
    }
}