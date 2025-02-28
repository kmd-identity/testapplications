using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMD.Identity.TestApplications.OpenID.APIOnBehalfOf.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ILogger<ClaimsController> _logger;

        public ClaimsController(ILogger<ClaimsController> logger)
        {
            _logger = logger;
        }
        
        [HttpGet]
        public ActionResult<object> Get()
        {
            return new
            {
                Title = "Hello from API on behalf of",
                Claims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToArray()
            };
        }
    }
}
