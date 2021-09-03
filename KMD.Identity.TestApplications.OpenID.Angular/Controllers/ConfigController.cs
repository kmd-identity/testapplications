using KMD.Identity.TestApplications.OpenID.Angular.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace KMD.Identity.TestApplications.OpenID.Angular.Controllers
{
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly SecurityConfig config;

        public ConfigController(IOptions<SecurityConfig> config)
        {
            this.config = config.Value;
        }

        [HttpGet]
        [Route("[controller]/auth")]
        public SecurityConfig Auth()
        {
            return this.config;
        }
    }
}
