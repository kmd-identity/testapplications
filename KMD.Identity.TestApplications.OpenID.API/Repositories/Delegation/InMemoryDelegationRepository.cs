using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using KMD.Identity.TestApplications.OpenID.API.Models.Delegation;

namespace KMD.Identity.TestApplications.OpenID.API.Repositories.Delegation
{
    public class InMemoryDelegationRepository : IDelegationRepository
    {
        private static ConcurrentDictionary<Guid, AccessDelegation> store = new();

        public void StoreAccessDelegation(AccessDelegation accessDelegation)
        {
            store[accessDelegation.AccessDelegationId] = accessDelegation;
        }

        public AccessDelegation GetAccessDelegation(Guid accessDelegationId)
        {
            if (store.ContainsKey(accessDelegationId)) return store[accessDelegationId];

            return null;
        }

        public AccessDelegation FindByFlowId(Guid flowId)
        {
            foreach (var storeValue in store.Values)
            {
                if (storeValue.FlowId == flowId) return storeValue;
            }

            return null;
        }

        public AccessDelegation FindByActFlowId(Guid flowId)
        {
            foreach (var storeValue in store.Values)
            {
                foreach (var act in storeValue.Acts)
                {
                    if (act.FlowId == flowId) return storeValue;
                }
            }

            return null;
        }

        public IEnumerable<AccessDelegation> FindBySubject(string subject)
        {
            foreach (var storeValue in store.Values)
            {
                if (storeValue.UserData.Sub == subject) yield return storeValue;
            }
        }

        public IEnumerable<AccessDelegation> FindAll()
        {
            foreach (var storeValue in store.Values)
            {
                yield return storeValue;
            }
        }

        public void RemoveAccessDelegation(Guid accessDelegationId)
        {
            store.Remove(accessDelegationId, out var _);
        }
    }
}