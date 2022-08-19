using KMD.Identity.TestApplications.OpenID.Angular.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace KMD.Identity.TestApplications.OpenID.Angular.Controllers
{
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly SecurityConfig _securityConfig;
        private readonly FeatureToggleConfig _featureToggleConfig;

        public ConfigController(IOptions<SecurityConfig> securityConfigOptions, IOptions<FeatureToggleConfig> featureToggleConfigOptions)
        {
            _securityConfig = securityConfigOptions.Value;
            _featureToggleConfig = featureToggleConfigOptions.Value;
        }

        [HttpGet]
        [Route("[controller]/appconfig")]
        public ApplicationConfig FeatureToggles()
        {
            return new ApplicationConfig {FeatureToggle = _featureToggleConfig, Security = _securityConfig};
        }
    }
}
