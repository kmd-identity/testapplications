using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using KMD.Identity.TestApplications.OpenID.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace KMD.Identity.TestApplications.OpenID.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ILogger<ClaimsController> _logger;
        private readonly IConfiguration configuration;
        private readonly IOptionsMonitor<JwtBearerOptions> jwtOptions;

        public ClaimsController(ILogger<ClaimsController> logger, IConfiguration configuration, IOptionsMonitor<JwtBearerOptions> jwtOptions)
        {
            _logger = logger;
            this.configuration = configuration;
            this.jwtOptions = jwtOptions;
        }
        
        [HttpGet]
        public ActionResult<object> Get()
        {
            return new
            {
                Title = "Hello from API",
                Claims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToArray()
            };
        }

        [HttpGet]
        [Route("onbehalfof")]
        public async Task<ActionResult<object>> GetOnBehalfOf(CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient();

            //FIRST - get TOKEN using on behalf of flow
            var originalBearerToken = Request.Headers.Authorization.First().Split(' ').Last().Trim();
            var adfsTokenEndpoint =
                (await jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme).ConfigurationManager!
                    .GetConfigurationAsync(cancellationToken)).TokenEndpoint;

            var onBehalfOfParameters = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
                new("client_id", configuration["OnBehalfOf:ClientId"]),
                new("client_secret", configuration["OnBehalfOf:ClientSecret"]),
                new("assertion", originalBearerToken),
                new("requested_token_use", "on_behalf_of"),
                new("scope", configuration["OnBehalfOf:Scopes"])
            };
                
            using var onBehalfOfRequest = new HttpRequestMessage(HttpMethod.Post, adfsTokenEndpoint) { Content = new FormUrlEncodedContent(onBehalfOfParameters) };
            using var onBehalfOfResponse = await httpClient.SendAsync(onBehalfOfRequest, cancellationToken);
                
            var onBehalfOfContent = await onBehalfOfResponse.Content.ReadAsStringAsync(cancellationToken);
            var onBehalfOfOAuthResponse = JsonConvert.DeserializeObject<TokenResponse>(onBehalfOfContent);

            //SECOND - call other API using on behalf of token
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", onBehalfOfOAuthResponse.AccessToken);
            var response = await httpClient.GetAsync(configuration["OnBehalfOf:ApiUrl"], cancellationToken);
            var apiCallResult = await response.Content.ReadAsStringAsync(cancellationToken);

            return apiCallResult;
        }
    }
}
