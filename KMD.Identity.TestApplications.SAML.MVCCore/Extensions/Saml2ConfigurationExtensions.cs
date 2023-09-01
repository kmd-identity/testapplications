using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using System;
using System.Linq;

namespace KMD.Identity.TestApplications.SAML.MVCCore.Extensions;

public static class Saml2ConfigurationExtensions
{
    public static void ReloadMetadata(this Saml2Configuration saml2Configuration, Uri metadataUrl)
    {
        var entityDescriptor = new EntityDescriptor();
        entityDescriptor.ReadIdPSsoDescriptorFromUrl(metadataUrl);
        if (entityDescriptor.IdPSsoDescriptor != null)
        {
            saml2Configuration.AllowedIssuer = entityDescriptor.EntityId;
            saml2Configuration.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location;
            saml2Configuration.SingleLogoutDestination = entityDescriptor.IdPSsoDescriptor.SingleLogoutServices.First().Location;
            saml2Configuration.SignatureValidationCertificates.AddRange(entityDescriptor.IdPSsoDescriptor.SigningCertificates);
            if (entityDescriptor.IdPSsoDescriptor.WantAuthnRequestsSigned.HasValue)
            {
                saml2Configuration.SignAuthnRequest = entityDescriptor.IdPSsoDescriptor.WantAuthnRequestsSigned.Value;
            }
        }
        else
        {
            throw new Exception("IdPSsoDescriptor not loaded from metadata.");
        }
    }
}
