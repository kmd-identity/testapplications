using ITfoxtec.Identity.Saml2;

namespace KMD.Identity.TestApplications.SAML.MVCCore.Config;

public class ExtendedSaml2Configuration : Saml2Configuration
{
    public string IdPMetadata { get; set; }
    public string SigningCertificateThumbprint { get; set; }
    public string SigningCertificateFile { get; set; }
    public string SigningCertificatePassword { get; set; }
}
