using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using KMD.Identity.TestApplications.OpenID.API.Models;
using KMD.Identity.TestApplications.OpenID.API.Services.Audit;

namespace KMD.Identity.TestApplications.OpenID.API.Services.Financial
{
    public class InMemoryFinancialService : IFinancialService
    {
        private readonly IAuditService auditService;
        private static readonly ConcurrentDictionary<string, List<decimal>> store = new ConcurrentDictionary<string, List<decimal>>();
        private static readonly Random randomPayments = new Random();

        public InMemoryFinancialService(IAuditService auditService)
        {
            this.auditService = auditService;
        }

        public OperationResult<List<decimal>> Get(string subject)
        {
            if(!store.ContainsKey(subject))
                return OperationResult<List<decimal>>.Pass(new List<decimal>());

            return OperationResult<List<decimal>>.Pass(store[subject]);
        }

        public OperationResult<List<decimal>> Pay(string subject)
        {
            if (store.ContainsKey(subject))
            {
                var existing = store[subject];
                if (existing.Count > 1) return OperationResult<List<decimal>>.Fail("Already paid. Only two payments allowed.");

                existing.Add(randomPayments.Next(2, 100));
                store[subject] = existing;

                return OperationResult<List<decimal>>.Pass(existing);
            }
            else
            {
                var newPay = new List<decimal> { randomPayments.Next(2, 100) };
                store[subject] = newPay;
                return OperationResult<List<decimal>>.Pass(newPay);
            }
        }

        //technical method
        public void CleanupAll()
        {
            store.Clear();
        }

        //technical method
        public void Cleanup(string subject)
        {
            store.TryRemove(subject, out var _);
        }
    }
}
