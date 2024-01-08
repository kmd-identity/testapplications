using System;

namespace KMD.Identity.TestApplications.OpenID.MVCCore.Models.Audit
{
    public class AuditItem
    {
        public string Subject { get; set; }

        public string Operation { get; set; }

        public DateTime Timestamp { get; set; }
    }
}