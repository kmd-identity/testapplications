using System;

namespace KMD.Identity.TestApplications.OpenID.MVCCore.Models.Delegation
{
    public class AccessDelegationAct
    {  
        public Guid AccessDelegationActId { get; set; }
        public Guid FlowId { get; set; }
        public string Who { get; set; }
        public DateTime ActingStartedTimestamp { get; set; }
        public DateTime? ActingFinishedTimestamp { get; set; }
        public AccessDelegationActingStatus Status { get; set; }
    }
}
