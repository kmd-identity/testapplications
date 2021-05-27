using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography.X509Certificates;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace KMD.Identity.TestApplications.SAML.MVCCore.Controllers
{
    [AllowAnonymous]
    public class MetadataController : Controller
    {
        private readonly Saml2Configuration config;

        public MetadataController(IOptions<Saml2Configuration> configAccessor)
        {
            config = configAccessor.Value;
        }

        public IActionResult Index()
        {
            var defaultSite = new Uri($"{Request.Scheme}://{Request.Host.ToUriComponent()}/");

            var entityDescriptor = new EntityDescriptor(config);
            entityDescriptor.SPSsoDescriptor = new SPSsoDescriptor
            {
                WantAssertionsSigned = true,
                SigningCertificates = new X509Certificate2[]
                {
                    config.SigningCertificate
                },
                SingleLogoutServices = new SingleLogoutService[]
                {
                    new SingleLogoutService
                    {
                        Binding = ProtocolBindings.HttpPost, Location = new Uri(defaultSite, "Auth/SingleLogout"),
                        ResponseLocation = new Uri(defaultSite, "Auth/LoggedOut")
                    }
                },
                NameIDFormats = new Uri[] { NameIdentifierFormats.X509SubjectName },
                AssertionConsumerServices = new AssertionConsumerService[]
                {
                    new AssertionConsumerService
                    {
                        Binding = ProtocolBindings.HttpPost,
                        Location = new Uri(defaultSite, "Auth/AssertionConsumerService")
                    },
                    new AssertionConsumerService
                    {
                        Binding = ProtocolBindings.HttpPost,
                        Location = new Uri(defaultSite, "Auth/AssertionConsumerService-test"), IsDefault = false
                    }
                },
                AttributeConsumingServices = new AttributeConsumingService[]
                {
                    new AttributeConsumingService
                    {
                        ServiceName = new ServiceName("Some SP", "en")
                    }
                },
            };
            entityDescriptor.ContactPersons = new[]
            {
                new ContactPerson(ContactTypes.Administrative)
                {
                    Company = "Some Company",
                    GivenName = "Some Given Name",
                    SurName = "Some Sur Name",
                    EmailAddress = "some@some-domain.com",
                    TelephoneNumber = "11111111",
                },
                new ContactPerson(ContactTypes.Technical)
                {
                    Company = "Some Company",
                    GivenName = "Some tech Given Name",
                    SurName = "Some tech Sur Name",
                    EmailAddress = "sometech@some-domain.com",
                    TelephoneNumber = "22222222",
                }
            };
            return new Saml2Metadata(entityDescriptor).CreateMetadata().ToActionResult();
        }
    }
}
