using System;
using System.Linq;
using System.Threading.Tasks;
using KMD.Identity.TestApplications.OpenID.API.Extensions;
using KMD.Identity.TestApplications.OpenID.API.Models;
using KMD.Identity.TestApplications.OpenID.API.Models.Audit;
using KMD.Identity.TestApplications.OpenID.API.Repositories.Delegation;
using KMD.Identity.TestApplications.OpenID.API.Services.Audit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KMD.Identity.TestApplications.OpenID.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly ILogger<AuditController> _logger;
        private readonly IAuditService auditService;
        private readonly IDelegationRepository delegationRepository;

        public AuditController(
            ILogger<AuditController> logger,
            IAuditService auditService,
            IDelegationRepository delegationRepository)
        {
            _logger = logger;
            this.auditService = auditService;
            this.delegationRepository = delegationRepository;
        }

        [Route("/api/audit/delegation")]
        [HttpGet]
        public async Task<OperationResult<AuditItem[]>> DelegationAudit(Guid accessDelegationId)
        {
            if (User.HasRole("Citizen"))
            {
                var delegation = delegationRepository.GetAccessDelegation(accessDelegationId);
                if(delegation == null) return OperationResult<AuditItem[]>.Fail("No record");
                if(delegation.UserData.Sub != User.GetSubject())return OperationResult<AuditItem[]>.Fail("Not your data");

                return OperationResult<AuditItem[]>.Pass(auditService.Get(accessDelegationId).ToArray());
            }

            if (User.HasRole("CaseWorker"))
            {
                return OperationResult<AuditItem[]>.Pass(auditService.Get(accessDelegationId).ToArray());
            }

            return OperationResult<AuditItem[]>.Fail("Unknown role");
        }
    }
}

