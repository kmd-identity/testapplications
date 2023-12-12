using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KMD.Identity.TestApplications.OpenID.API.Extensions;
using KMD.Identity.TestApplications.OpenID.API.Models;
using KMD.Identity.TestApplications.OpenID.API.Models.Delegation;
using KMD.Identity.TestApplications.OpenID.API.Repositories.Delegation;
using KMD.Identity.TestApplications.OpenID.API.Services.Audit;
using KMD.Identity.TestApplications.OpenID.API.Services.Financial;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KMD.Identity.TestApplications.OpenID.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class FinancialController : ControllerBase
    {
        private readonly ILogger<FinancialController> _logger;
        private readonly IAuditService auditService;
        private readonly IDelegationRepository delegationRepository;
        private readonly IFinancialService financialService;

        public FinancialController(
            ILogger<FinancialController> logger,
            IAuditService auditService,
            IDelegationRepository delegationRepository,
            IFinancialService financialService)
        {
            _logger = logger;
            this.auditService = auditService;
            this.delegationRepository = delegationRepository;
            this.financialService = financialService;
        }

        /// <summary>
        /// This is complete made up situation that shows usage of additional claims, that Case Worker got when Acting on behalf of subject
        /// </summary>
        /// <returns></returns>
        [Route("/api/financial/get")]
        [HttpGet]
        public async Task<OperationResult<List<decimal>>> Get()
        {
            if (User.HasRole("Citizen"))
            {
                return financialService.Get(User.GetSubject());
            }

            if (User.HasRole("CaseWorker"))
            {
                var delegationSub = User.Claims.FirstOrDefault(c => c.Type.Equals("DelegationSub", StringComparison.InvariantCultureIgnoreCase))?.Value;
                var delegationId = User.Claims.FirstOrDefault(c => c.Type.Equals("DelegationId", StringComparison.InvariantCultureIgnoreCase))?.Value;

                if (string.IsNullOrWhiteSpace(delegationSub)) return OperationResult<List<decimal>>.Fail($"No access delegation granted");

                var delegation = delegationRepository.GetAccessDelegation(Guid.Parse(delegationId));
                if (delegation.Status != AccessDelegationStatus.Delegated) return OperationResult<List<decimal>>.Fail($"No access delegation granted");

                auditService.Add(delegation.AccessDelegationId, User.GetSubject(), $"Payments loaded");
                return financialService.Get(delegationSub);
            }

            return OperationResult<List<decimal>>.Fail("Unknown role");
        }

        /// <summary>
        /// This is complete made up situation that shows usage of additional claims, that Case Worker got when Acting on behalf of subject
        /// </summary>
        /// <returns></returns>
        [Route("/api/financial/pay")]
        [HttpGet]
        public async Task<OperationResult<List<decimal>>> Pay()
        {
            if (User.HasRole("Citizen"))
            {
                return OperationResult<List<decimal>>.Fail("You're not allowed to pay. Ask Case Worker.");
            }

            if (User.HasRole("CaseWorker"))
            {
                var delegationSub = User.Claims.FirstOrDefault(c => c.Type.Equals("DelegationSub", StringComparison.InvariantCultureIgnoreCase))?.Value;
                var delegationId = User.Claims.FirstOrDefault(c => c.Type.Equals("DelegationId", StringComparison.InvariantCultureIgnoreCase))?.Value;

                if (string.IsNullOrWhiteSpace(delegationSub)) return OperationResult<List<decimal>>.Fail($"No access delegation granted");

                var delegation = delegationRepository.GetAccessDelegation(Guid.Parse(delegationId));
                if(delegation.Status != AccessDelegationStatus.Delegated) return OperationResult<List<decimal>>.Fail($"No access delegation granted");

                var result = financialService.Pay(delegationSub);

                auditService.Add(delegation.AccessDelegationId, User.GetSubject(), result.Success ? $"Paid {result.Result.Last()}$" : $"Payment failed with error: {result.Error}");

                return result;
            }

            return OperationResult<List<decimal>>.Fail("Unknown role");
        }
    }
}

