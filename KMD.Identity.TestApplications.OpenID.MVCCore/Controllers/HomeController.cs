using KMD.Identity.TestApplications.OpenID.MVCCore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;

namespace KMD.Identity.TestApplications.OpenID.MVCCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private string ClientId => Configuration["Security:ClientId"];
        private string ClientSecret => Configuration["Security:ClientSecret"];
        private string TokenEndpoint => Configuration["Security:TokenEndpoint"];
        private string ClientCredentialsCertificate => Configuration["Security:ClientCredentialsCertificate"];
        private string ClientCredentialsCertificatePassword => Configuration["Security:ClientCredentialsCertificatePassword"];
        private string ClientCredentialsApiScope => Configuration["Security:ClientCredentialsApiScope"];

        public IConfiguration Configuration { get; }

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            Configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> CallApi()
        {
          
            // If you have set options.SaveTokens = true (in Startup.cs) then you can retrieve access_token as stated below
            //var rawAccessToken = await HttpContext.GetTokenAsync("AD FS", "access_token");

            // Retrieving access_token from session because that's how we stored it in OnTokenResponseReceived in Startup.cs
            var rawAccessToken = HttpContext.Session.GetString("access_token");
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", rawAccessToken);
            var response = await httpClient.GetAsync(Configuration["Security:ApiUrl"]);
            var apiCallResult = await response.Content.ReadAsStringAsync();

            ViewBag.Result = apiCallResult;

            return View();

        }

        [Authorize]
        public async Task<IActionResult> GetClientCredentialsWithSecret()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, TokenEndpoint);
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", ClientCredentialsApiScope),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret)
            });

            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.SendAsync(request);
            var token = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(token);
            var accessToken = jsonDoc.RootElement.GetProperty("access_token").GetString();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var apiResponse = await httpClient.GetAsync(Configuration["Security:ApiUrl"]);
            var apiCallResult = await apiResponse.Content.ReadAsStringAsync();

            ViewBag.Result = apiCallResult;

            return View("CallApi");
        }

        [Authorize]
        public async Task<IActionResult> GetClientCredentialsWithCertificate()
        {
            var clientCertificate = new X509Certificate2(ClientCredentialsCertificate, ClientCredentialsCertificatePassword);

            var now = DateTime.UtcNow;
            var securityKey = new X509SecurityKey(clientCertificate);

            var tokenHandler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("sub", ClientId),
                    new Claim("jti", Guid.NewGuid().ToString())
                }),
                Issuer = ClientId,
                Audience = TokenEndpoint,
                NotBefore = now,
                Expires = now.AddMinutes(5),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256)
            };

            var jwt = tokenHandler.CreateEncodedJwt(descriptor);


            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, TokenEndpoint);
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", ClientCredentialsApiScope),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
                new KeyValuePair<string, string>("client_assertion", jwt)
            });

            var httpClient = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(request);
            var token = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(token);
            var accessToken = jsonDoc.RootElement.GetProperty("access_token").GetString();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var apiResponse = await httpClient.GetAsync(Configuration["Security:ApiUrl"]);
            var apiCallResult = await apiResponse.Content.ReadAsStringAsync();

            ViewBag.Result = apiCallResult;

            return View("CallApi");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
