using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KMD.Identity.TestApplications.OpenID.API.Extensions;
using KMD.Identity.TestApplications.OpenID.API.Models;
using KMD.Identity.TestApplications.OpenID.API.Models.Delegation;
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
    public class FinancialController : ControllerBase
    {
        private readonly ILogger<FinancialController> _logger;
        private readonly IAuditService auditService;
        private readonly IDelegationRepository delegationRepository;
        private static readonly ConcurrentDictionary<string, List<decimal>> payments = new ConcurrentDictionary<string, List<decimal>>();
        private readonly Random randomPayments = new Random();

        public FinancialController(
            ILogger<FinancialController> logger,
            IAuditService auditService,
            IDelegationRepository delegationRepository)
        {
            _logger = logger;
            this.auditService = auditService;
            this.delegationRepository = delegationRepository;
        }

        [Route("/api/financial/pay")]
        [HttpGet]
        public async Task<OperationResult<List<decimal>>> Pay()
        {
            if (User.HasRole("Citizen"))
            {
                if(payments.ContainsKey(User.GetSubject()))
                    return OperationResult<List<decimal>>.Pass(payments[User.GetSubject()]);

                return OperationResult<List<decimal>>.Fail("You're not allowed to pay. Ask Case Worker.");
            }

            if (User.HasRole("CaseWorker"))
            {
                var delegationSub = User.Claims.FirstOrDefault(c => c.Type.Equals("DelegationSub", StringComparison.InvariantCultureIgnoreCase))?.Value;
                var delegationId = User.Claims.FirstOrDefault(c => c.Type.Equals("DelegationId", StringComparison.InvariantCultureIgnoreCase))?.Value;

                if (string.IsNullOrWhiteSpace(delegationSub)) return OperationResult<List<decimal>>.Fail($"No access delegation granted");

                var delegation = delegationRepository.GetAccessDelegation(Guid.Parse(delegationId));
                if(delegation.Status != AccessDelegationStatus.Delegated) return OperationResult<List<decimal>>.Fail($"No access delegation granted");

                if (payments.ContainsKey(delegationSub))
                {
                    var existing = payments[delegationSub];

                    if(existing.Count > 1) return OperationResult<List<decimal>>.Fail("Already paid. Only two payments allowed.");

                    existing.Add(randomPayments.Next(2, 100));

                    auditService.Add(delegation.AccessDelegationId, User.GetSubject(), $"Paid {existing.Last()}$");

                    payments[delegationSub] = existing;

                    return OperationResult<List<decimal>>.Pass(existing);
                }
                else
                {
                    var newPay = new List<decimal> { randomPayments.Next(2, 100) };
                    payments[delegationSub] = newPay;
                    auditService.Add(delegation.AccessDelegationId, User.GetSubject(), $"Paid {newPay.Last()}$");
                    return OperationResult<List<decimal>>.Pass(newPay);
                }
            }

            return OperationResult<List<decimal>>.Fail("Unknown role");
        }
    }
}

