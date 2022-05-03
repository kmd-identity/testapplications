using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;

namespace KMD.Identity.TestApplications.OpenID.MVCCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();

            services.AddSession(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
            })
            .AddOpenIdConnect("AD FS", o =>
            {
                o.ClientId = Configuration["Security:ClientId"];
                o.ClientSecret = Configuration["Security:ClientSecret"];
                o.MetadataAddress = Configuration["Security:IdPMetadataUrl"];
                o.ResponseType = "code";
                o.UsePkce = true;
                o.MapInboundClaims = false;
                o.Scope.Clear();
                o.Scope.Add(Configuration["Security:ApiScope"]);

                // Setting this to true will create a big cookie with id_token and access_token
                // If this is set to false (default value), then make sure you have an OnTokenResponseReceived to store the access_token
                // If this is set to true then you can use await HttpContext.GetTokenAsync("AD FS", "access_token"); to retrieve the access_token, otherwise you will have to manually retrieve the access_token based on how you stored it in OnTokenResponseReceived
                //options.SaveTokens = true;

                o.Events = new OpenIdConnectEvents()
                {
                    OnRedirectToIdentityProvider = context =>
                    {
                        const string domainHintKey = "domain_hint";
                        if (context.Properties.Items.ContainsKey(domainHintKey))
                        {
                            var domainHint = context.Properties.Items[domainHintKey];
                            if (!string.IsNullOrWhiteSpace(domainHint))
                            {
                                context.ProtocolMessage.DomainHint = domainHint;
                            }

                            context.Properties.Items.Remove(domainHintKey);
                        }

                        return Task.FromResult(0);
                    },
                    OnTokenResponseReceived = (context) =>
                    {
                        // This method is only relevant if you are not setting options.SaveTokens = true
                        var accessToken = context.TokenEndpointResponse.AccessToken;
                        context.HttpContext.Session.SetString("access_token", accessToken);

                        var idToken = context.TokenEndpointResponse.IdToken;
                        context.HttpContext.Session.SetString("id_token", idToken);

                        return Task.CompletedTask;
                    },
                    OnRedirectToIdentityProviderForSignOut = context =>
                    {
                        // Id token hint is needed to redirect back to application
                        context.ProtocolMessage.IdTokenHint = context.HttpContext.Session.GetString("id_token");
                        // Client-id is required by kmd.identity
                        context.ProtocolMessage.ClientId = Configuration["Security:ClientId"];
                        return Task.FromResult(0);
                    },
                    OnSignedOutCallbackRedirect = context =>
                    {
                        context.HttpContext.Session.Remove("id_token");
                        context.HttpContext.Session.Remove("access_token");
                        return Task.FromResult(0);
                    },
                };
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsEnvironment("Development"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
