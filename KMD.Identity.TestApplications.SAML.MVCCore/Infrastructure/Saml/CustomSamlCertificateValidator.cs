using ITfoxtec.Identity.Saml2.Util;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace KMD.Identity.TestApplications.SAML.MVCCore.Infrastructure.Saml
{
    /// <summary>
    /// This is an extension to the ItFoxtec certificate validator which allows you to soften the validation requirements for certificates.
    /// The following scenarios will cause certificate validation to fail in the base class but can be ignored in this class using the validation flag:":
    /// - Root certificate is not trusted (case for OCES3)
    /// - Certificate revocation endpoint returns bad response
    /// - Self signed certificate is used (case for many ADFS)
    /// </summary>
    public class CustomSamlCertificateValidator : Saml2CertificateValidator
    {
        private readonly X509VerificationFlags validationFlags;

        public CustomSamlCertificateValidator(X509VerificationFlags validationFlags)
        {
            this.validationFlags = validationFlags;
        }

        /// <summary>
        /// Just like in base class, but with flags specification.
        /// </summary>
        /// <param name="certificate"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SecurityTokenValidationException"></exception>
        public override void Validate(X509Certificate2 certificate)
        {
            if (certificate == null)
                throw new ArgumentNullException(nameof(certificate));
            
            using X509Chain x509Chain = new X509Chain(this.TrustedStoreLocation == StoreLocation.LocalMachine)
            {
                ChainPolicy = new X509ChainPolicy()
                {
                    VerificationTime = DateTimeOffset.UtcNow.UtcDateTime,
                    RevocationMode = RevocationMode,
                    VerificationFlags = validationFlags
                }
            };
            if (!x509Chain.Build(certificate))
                throw new SecurityTokenValidationException(
                    "Invalid X509 certificate chain." + GetCertificateInformation(certificate) + GetChainStatusInformation(x509Chain.ChainStatus) + ".");

            var certResult = new StringBuilder();
            certResult.AppendLine($"{certificate.Thumbprint} Checking {certificate.Subject}");
            if (x509Chain.ChainStatus.Length > 0)
            {
                certResult.AppendLine(string.Join(" ", x509Chain.ChainStatus.Select(r => r.StatusInformation)));
                foreach (var elem in x509Chain.ChainElements)
                {
                    certResult.AppendLine(elem.Certificate.Subject + " " + string.Join(" ", elem.ChainElementStatus.Select(r => r.StatusInformation)));
                }
            }
            else
            {
                certResult.AppendLine("OK");
            }

            if (!Directory.Exists("certvalidation"))
                Directory.CreateDirectory("certvalidation");
            File.WriteAllText($"certvalidation\\{DateTime.UtcNow:yyyy-MM-ddHHMMss}.txt", certResult.ToString());

        }

        /// <summary>
        /// Private in base class. Needed to write again.
        /// </summary>
        private string GetChainStatusInformation(X509ChainStatus[] chainStatus)
        {
            if (chainStatus == null) return string.Empty;
            return " Chain Status:'" + string.Join(' ', chainStatus.Select(c => c.StatusInformation)) + "'.";
        }

        /// <summary>
        /// Private in base class. Needed to write again.
        /// </summary>
        private string GetCertificateInformation(X509Certificate2 certificate)
        {
            return $" Certificate name:'{certificate.SubjectName?.Name}' and thumbprint:'{certificate.Thumbprint}'.";
        }
    }
}
