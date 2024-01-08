using System;

namespace KMD.Identity.TestApplications.OpenID.API.Models.Audit
{
    public class AuditItem
    {
        public Guid EntityId { get; set; }

        public string Subject { get; set; }

        public string Operation { get; set; }

        public DateTime Timestamp { get; set; }
    }
}