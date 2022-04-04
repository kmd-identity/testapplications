using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Identity.Client;

namespace KMD.Identity.TestApplications.OpenID.WinForms
{
    public partial class LogonForm : Form
    {
        public LogonForm()
        {
            InitializeComponent();
        }

        private async void bAuthenticate_Click(object sender, EventArgs e)
        {
            var clientApp = PublicClientApplicationBuilder.Create(Properties.Settings.Default.ClientId)
                .WithAdfsAuthority(Properties.Settings.Default.AuthorityUrl)
                .WithRedirectUri(Properties.Settings.Default.RedirectUri) // Perhaps have a list of ports and check if any of them are available on the machine. Make sure the list of ports are also defined on KMD Identity as "approved" redirect uris.
                .Build();
            
            var authResult = await clientApp
                .AcquireTokenInteractive(Properties.Settings.Default.Scopes.Cast<string>().ToArray())
                .WithUseEmbeddedWebView(false) // Must be false, due to embedded view not supporting javascript correctly
                                               //.WithExtraQueryParameters(new Dictionary<string, string>() { { "domain_hint", "nemlogin-3-test-public" } }) // TODO domain hint does not work currently
                                               //.WithExtraQueryParameters(new Dictionary<string, string>() { { "domain_hint", "kmd-ad-prod" } })
                .WithSystemWebViewOptions(new SystemWebViewOptions()
                {
                    HtmlMessageSuccess = "You have successfully logged in. Close this browser and continue in your application.", // In Danish?
                    HtmlMessageError = "An error occurred. Close this browser and try logging in again." // In Danish?
                })
                .ExecuteAsync();

            if (string.IsNullOrWhiteSpace(authResult.AccessToken) || string.IsNullOrWhiteSpace(authResult.AccessToken))
                return;

            UserContext.FromResult(authResult);
            
            DialogResult = DialogResult.OK;
        }
    }
}
