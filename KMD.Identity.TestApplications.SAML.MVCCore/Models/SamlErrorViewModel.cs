namespace KMD.Identity.TestApplications.SAML.MVCCore.Models
{
    public class SamlErrorViewModel
    {
        public string Error { get; set; }

        public string Description { get; set; }

        public bool HasDescription => !string.IsNullOrWhiteSpace(Description);
    }
}
