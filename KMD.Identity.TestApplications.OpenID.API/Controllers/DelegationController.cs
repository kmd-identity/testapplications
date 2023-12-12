using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using KMD.Identity.TestApplications.OpenID.API.Extensions;
using KMD.Identity.TestApplications.OpenID.API.Models;
using KMD.Identity.TestApplications.OpenID.API.Models.Delegation;
using KMD.Identity.TestApplications.OpenID.API.Repositories.Delegation;
using KMD.Identity.TestApplications.OpenID.API.Services.Audit;
using KMD.Identity.TestApplications.OpenID.API.Services.Delegation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KMD.Identity.TestApplications.OpenID.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DelegationController : ControllerBase
    {
        private readonly ILogger<DelegationController> _logger;
        private readonly IDelegationService delegationService;
        private readonly IDelegationRepository delegationRepository;
        private readonly IAuditService auditService;

        public DelegationController(
            ILogger<DelegationController> logger,
            IDelegationService delegationService,
            IDelegationRepository delegationRepository,
            IAuditService auditService)
        {
            _logger = logger;
            this.delegationService = delegationService;
            this.delegationRepository = delegationRepository;
            this.auditService = auditService;
        }

        [Route("/api/delegation/delegateaccess")]
        [HttpPost]
        public async Task<OperationResult<AccessDelegation>> DelegateAccess()
        {
            if (!User.HasRole("Citizen")) return OperationResult<AccessDelegation>.Fail("User is not a Citizen");

            var initialResult = delegationService.New(User.GetSubject(), "all");
            if (!initialResult.Success) return initialResult;

            var result = delegationService.StartDelegatingAccess(initialResult.Result.FlowId, User.GetSubject());
            
            return result;
        }

        [Route("/api/delegation/revoke")]
        [HttpPost]
        public async Task<OperationResult> RevokeAccess(Guid accessDelegationId)
        {
            var result = delegationService.Revoke(
                User.HasRole("Citizen"),
                User.HasRole("CaseWorker"),
                User.GetSubject(),
                accessDelegationId);

            return result;
        }

        [Route("/api/delegation/delegatedbysubject")]
        [HttpGet]
        public async Task<OperationResult<AccessDelegation[]>> DelegatedBySubject()
        {
            if (!User.HasRole("Citizen")) return OperationResult<AccessDelegation[]>.Fail("User is not a Citizen");

            return OperationResult<AccessDelegation[]>.Pass(delegationRepository.FindBySubject(User.GetSubject()).ToArray());
        }

        [Route("/api/delegation/delegated")]
        [HttpGet]
        public async Task<OperationResult<AccessDelegation[]>> Delegated()
        {
            if (!User.HasRole("CaseWorker")) return OperationResult<AccessDelegation[]>.Fail("User is not a Case Worker");

            return OperationResult<AccessDelegation[]>.Pass(delegationRepository.FindAll().ToArray());
        }

        [Route("/api/delegation/act")]
        [HttpPost]
        public async Task<OperationResult<AccessDelegationAct>> Act(Guid accessDelegationId)
        {
            auditService.Add(accessDelegationId, User.GetSubject(), $"Acting requested");

            if (!User.HasRole("CaseWorker")) return OperationResult<AccessDelegationAct>.Fail("User is not a Case Worker");

            var result = delegationService.StartActing(accessDelegationId, User.GetSubject());

            return result;
        }

        [Route("GetCustomClaims")]
        [HttpGet]
        public async Task<object> CustomClaims()
        {
            var claims = User.Claims.ToArray();

            var flowId = claims.FirstOrDefault(c => c.Type.Equals("flowid", StringComparison.InvariantCultureIgnoreCase))?.Value;

            //below is something you probably won't get from KMD Identity (unless IdP will issue such claims)
            // in other cases you would need to call your internal PMS to determine what roles/permissions given user has
            // User.GetSubject() gives you the subject of the user
            var isCitizen = claims.Any(c => c.Type.Equals("role", StringComparison.InvariantCultureIgnoreCase) 
                                            && c.Value.Equals("citizen", StringComparison.InvariantCultureIgnoreCase));
            var isCaseWorker = claims.Any(c => c.Type.Equals("role", StringComparison.InvariantCultureIgnoreCase)
                                            && c.Value.Equals("caseworker", StringComparison.InvariantCultureIgnoreCase));

            if (string.IsNullOrEmpty(flowId))
            {
                //not a delegation flow
                return new
                {
                    SomeCustomClaim = "Some claim value"
                };
            }

            //citizen tries to finish delegating access process
            //which will allow case workers to act on behalf of him
            if (isCitizen)
            {
                var result = delegationService.FinishDelegatingAccess(Guid.Parse(flowId), User.GetSubject());

                if (!result.Success)
                {
                    return new
                    {
                        DelegationError = result.Error
                    };
                }

                return new
                {
                    DelegationMessage = $"Access delegated for flow id{flowId}"
                };
            }
            
            //case worker tries to finish acting process
            //which will allow him to act on behalf of citizen
            if (isCaseWorker)
            {
                var result = delegationService.FinishActing(Guid.Parse(flowId), User.GetSubject());

                if (!result.Success)
                {
                    return new
                    {
                        DelegationError = result.Error
                    };
                }

                return new
                {
                    DelegationMessage = $"Now you're acting as: {result.Result.UserData.Sub} for scope: {result.Result.Scope.OperationName}",
                    DelegationSub = result.Result.UserData.Sub,
                    DelegationScope = result.Result.Scope.OperationName,
                    DelegationId = result.Result.AccessDelegationId
                };
            }

            return new
            {
                DelegationMessage = "Nothing to process. Have you forgotten about selecting proper claims in KMD Identity Test IdP?"
            };
        }
    }
}

