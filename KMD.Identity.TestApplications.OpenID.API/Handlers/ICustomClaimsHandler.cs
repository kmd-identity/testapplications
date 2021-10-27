using System.Security.Claims;
using System.Threading.Tasks;
using KMD.Identity.TestApplications.OpenID.API.Models;
using KMD.Identity.TestApplications.OpenID.API.Models.CustomClaims;

namespace KMD.Identity.TestApplications.OpenID.API.Handlers
{
    public interface ICustomClaimsHandler
    {
        Task<ICustomClaimsResult> Handle(Claim[] claims);
    }
}
