using System.Linq;
using System.Threading.Tasks;
using KMD.Identity.TestApplications.OpenID.API.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KMD.Identity.TestApplications.OpenID.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CustomClaimsController : ControllerBase
    {
        private readonly ILogger<CustomClaimsController> _logger;
        private readonly ICustomClaimsHandler customClaimsHandler;

        public CustomClaimsController(ILogger<CustomClaimsController> logger, ICustomClaimsHandler customClaimsHandler)
        {
            _logger = logger;
            this.customClaimsHandler = customClaimsHandler;
        }

        /// <summary>
        /// This is main method of getting custom claims.
        /// As a KMD Identity consumer you have the opportunity to setup URL to this method in KMD Identity system.
        /// This method will be called during the authentication flow, but before the final token is created.
        /// Input to this method is completely based on JWT token issued by KMD Identity.
        /// For security reasons there is no additional parameter to this method. Everything is signed in JWT, so you can trust it.
        /// WARNING. This endpoint must be accessible from the internet.
        /// </summary>
        /// <returns></returns>
        [Route("GetCustomClaims")]
        [HttpGet]
        public async Task<object> CustomClaims()
        {
            return await this.customClaimsHandler.Handle(User.Claims.ToArray());
        }

        [Route("ReturnRequireCpr")]
        [HttpGet]
        public async Task<object> ReturnRequireCprCustomClaim()
        {
            return await this.customClaimsHandler.ReturnRequireCpr(User.Claims.ToArray());
        }
    }
}

