using System;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using KMD.Identity.TestApplications.OpenID.API.Extensions;
using KMD.Identity.TestApplications.OpenID.API.Repositories.Delegation;
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

        public DelegationController(
            ILogger<DelegationController> logger,
            IDelegationService delegationService,
            IDelegationRepository delegationRepository)
        {
            _logger = logger;
            this.delegationService = delegationService;
            this.delegationRepository = delegationRepository;
        }

        [Route("/api/delegation/delegateaccess")]
        [HttpPost]
        public async Task<object> DelegateAccess()
        {
            if (!User.HasRole("Citizen"))
                throw new SecurityException("User is not a Citizen");

            var accessDelegation = delegationService.New(User.Claims, "all");
            delegationService.StartDelegatingAccess(accessDelegation.FlowId, User.GetSubject());

            return accessDelegation;
        }

        [Route("/api/delegation/revoke")]
        [HttpPost]
        public async Task<object> RevokeAccess(Guid accessDelegationId)
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
        public async Task<object> DelegatedBySubject()
        {
            if (!User.HasRole("Citizen"))
                throw new SecurityException("User is not a Citizen");

            return delegationRepository.FindBySubject(User.GetSubject()).ToArray();
        }

        [Route("GetCustomClaims")]
        [HttpGet]
        public async Task<object> CustomClaims()
        {
            var claims = User.Claims.ToArray();

            var flowId = claims.FirstOrDefault(c => c.Type.Equals("flowid", StringComparison.InvariantCultureIgnoreCase))?.Value;
            var role = claims.FirstOrDefault(c => c.Type.Equals("role", StringComparison.InvariantCultureIgnoreCase))?.Value;

            if (string.IsNullOrEmpty(flowId))
            {
                //not a delegation flow, returning something
                return new
                {
                    Message = "Thanks for using Custom Claims"
                };
            }

            if (string.Equals(role, "citizen", StringComparison.InvariantCultureIgnoreCase))
            {
                var result = delegationService.FinishDelegatingAccess(Guid.Parse(flowId), User.GetSubject());

                if (!result.success)
                {
                    return new
                    {
                        Message = result.error
                    };
                }

                return new
                {
                    Message = $"Access delegated for flow id{flowId}"
                };
            }

            if (string.Equals(role, "caseworker", StringComparison.InvariantCultureIgnoreCase))
            {
                //acting
                return new
                {
                    Message = "Access Delegation process"
                };
            }

            return new
            {
                Message = "Nothing to process"
            };
        }
    }
}

