using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using KMD.Identity.TestApplications.OpenID.API.Models.Audit;

namespace KMD.Identity.TestApplications.OpenID.API.Services.Audit
{
    public class InMemoryAuditService : IAuditService
    {
        private static ConcurrentDictionary<Guid, List<AuditItem>> store = new();

        public void Add(Guid entityId, string subject, string operation)
        {
            if (!store.ContainsKey(entityId))
            {
                store.TryAdd(entityId, new List<AuditItem> { new AuditItem { EntityId = entityId, Operation = operation, Subject = subject, Timestamp = DateTime.UtcNow } });
                return;
            }

            store[entityId].Add(new AuditItem { EntityId = entityId, Operation = operation, Subject = subject, Timestamp = DateTime.UtcNow });
        }

        public IEnumerable<AuditItem> Get(Guid entityId)
        {
            if (store.ContainsKey(entityId))
                return store[entityId];

            return Array.Empty<AuditItem>();
        }

        //technical method
        public void CleanupAll()
        {
            store.Clear();
        }

        //technical method
        public void Cleanup(IEnumerable<Guid> entityIds)
        {
            foreach (var entityId in entityIds)
            {
                store.TryRemove(entityId, out var _);
            }
        }
    }
}
