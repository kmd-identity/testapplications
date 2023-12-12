using System;
using System.Collections.Generic;
using KMD.Identity.TestApplications.OpenID.API.Models.Audit;

namespace KMD.Identity.TestApplications.OpenID.API.Services.Audit
{
    public interface IAuditService
    {
        void Add(Guid entityId, string subject, string operation);
        IEnumerable<AuditItem> Get(Guid entityId);

        //technical method
        void CleanupAll();
        //technical method
        void Cleanup(IEnumerable<Guid> entityIds);
    }
}