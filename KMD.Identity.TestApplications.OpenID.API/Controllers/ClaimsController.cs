using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace KMD.Identity.TestApplications.OpenID.API.Controllers
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
                Title = "Hello for API",
                Claims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToArray()
            };
        }
    }
}
