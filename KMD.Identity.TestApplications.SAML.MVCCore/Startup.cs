using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.MvcCore.Configuration;
using ITfoxtec.Identity.Saml2.Util;
using KMD.Identity.TestApplications.SAML.MVCCore.Infrastructure.Saml;
using KMD.Identity.TestApplications.SAML.MVCCore.Extensions;
using KMD.Identity.TestApplications.SAML.MVCCore.Config;
using System.Runtime.InteropServices;

namespace KMD.Identity.TestApplications.SAML.MVCCore
{
    public class Startup
    {
        public static IWebHostEnvironment AppEnvironment { get; private set; }

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            AppEnvironment = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();

            services.Configure<ExtendedSaml2Configuration>(Configuration.GetSection("Saml2"));
            //Note we're using the same certificate for signing and encryption 
            services.Configure<ExtendedSaml2Configuration>(saml2Configuration =>
            {
                if (AppEnvironment.IsEnvironment("Development"))
                {
                    saml2Configuration.SigningCertificate = CertificateUtil.Load(AppEnvironment.MapToPhysicalFilePath(saml2Configuration.SigningCertificateFile), saml2Configuration.SigningCertificatePassword);
                    saml2Configuration.DecryptionCertificates =
                    [
                        CertificateUtil.Load(AppEnvironment.MapToPhysicalFilePath(saml2Configuration.SigningCertificateFile), saml2Configuration.SigningCertificatePassword)
                    ];
                }
                else
                {
                    // Depending on the hosting environment you may load the Signing Certificate in different way, below are examples:
                    // - Hosting on Azure as Windows App Service
                    //   There are several options:
                    //
                    //   a. Loading from Certificate Store 
                    //
                    //      X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                    //      certStore.Open(OpenFlags.ReadOnly);
                    //      X509Certificate2Collection certCollection = certStore.Certificates.Find(
                    //          X509FindType.FindByThumbprint, saml2Configuration.SigningCertificateThumbprint, false);
                    //      var certificate = certCollection[0];
                    //      saml2Configuration.SigningCertificate = certificate;
                    //      saml2Configuration.DecryptionCertificates = [certificate];
                    //      certStore.Close();
                    //
                    //    b. Using KeyVault (look at https://github.com/ITfoxtec/ITfoxtec.Identity.Saml2/tree/main/test/TestWebAppCoreAzureKeyVault)
                    //
                    // - Hosting on Azure as Linux App Service
                    //   The most common way is to load it from disk:
                    //
                    //    a. Loading from Disk - WEBSITE_LOAD_CERTIFICATES Environment Variable specified Thumbprint(s),
                    //                          and you can use the Thumbprint to access locally stored certificate (Azure ensures it's copied to disk)
                    //                          Configured SigningCertificateFile must be in format /var/ssl/private/THUMBPRINT.p12
                    //
                    //       var bytes = File.ReadAllBytes(saml2Configuration.SigningCertificateFile);
                    //       var certificate = new X509Certificate2(bytes);
                    //       saml2Configuration.SigningCertificate = certificate;
                    //       saml2Configuration.DecryptionCertificates = [certificate];
                    //
                    //    b. Using KeyVault (look at https://github.com/ITfoxtec/ITfoxtec.Identity.Saml2/tree/main/test/TestWebAppCoreAzureKeyVault)
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                        certStore.Open(OpenFlags.ReadOnly);
                        X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, saml2Configuration.SigningCertificateThumbprint, false);
                        var certificate = certCollection[0];
                        saml2Configuration.SigningCertificate = certificate;
                        saml2Configuration.DecryptionCertificates = [certificate];
                        certStore.Close();
                    }
                    else
                    {
                        var bytes = File.ReadAllBytes(saml2Configuration.SigningCertificateFile);
                        var certificate = new X509Certificate2(bytes);
                        saml2Configuration.SigningCertificate = certificate;
                        saml2Configuration.DecryptionCertificates = [certificate];
                    }
                }

                if (saml2Configuration.CertificateValidationMode == X509CertificateValidationMode.Custom)
                {
                    saml2Configuration.CustomCertificateValidator = new CustomSamlCertificateValidator(
                        // - Root certificate is not trusted (case for OCES3)
                        // - Self signed certificate is used (case for many ADFS)
                        X509VerificationFlags.AllowUnknownCertificateAuthority
                        // - Certificate revocation endpoint returns bad response
                        | X509VerificationFlags.IgnoreCertificateAuthorityRevocationUnknown
                        | X509VerificationFlags.IgnoreEndRevocationUnknown
                    )
                    {
                        RevocationMode = saml2Configuration.RevocationMode
                    };
                }

                saml2Configuration.AllowedAudienceUris.Add(saml2Configuration.Issuer);

                saml2Configuration.ReloadMetadata(new Uri(saml2Configuration.IdPMetadataUrl));
            });

            services.AddSaml2(slidingExpiration: true);

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

            app.UseRouting();

            app.UseSaml2();
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
