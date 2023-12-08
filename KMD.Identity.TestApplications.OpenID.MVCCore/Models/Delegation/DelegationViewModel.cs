using System;
using System.Linq;

namespace KMD.Identity.TestApplications.OpenID.MVCCore.Models.Delegation
{
    public class DelegationViewModel
    {
        public AccessDelegation[] DelegatedAccess { get; set; }= Array.Empty<AccessDelegation>();

        public string[] Errors { get; set; } = Array.Empty<string>();

        public bool HasError => Errors.Any();

        public string[] Messages { get; set; } = Array.Empty<string>();

        public bool HasMessages => Messages.Any();
    }
}
