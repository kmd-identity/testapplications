using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Desktop;

namespace KMD.Identity.TestApplications.OpenID.WinForms
{
    public partial class LogonForm : Form
    {
        public LogoutType LogoutType;
        public LogonForm()
        {
            InitializeComponent();
        }

        private async void bAuthenticateDefault_Click(object sender, EventArgs e)
        {
            //Set to specify how to properly sign out when authenticating this way
            LogoutType = LogoutType.UseBrowser;

            var clientApp = PublicClientApplicationBuilder.Create(Properties.Settings.Default.ClientId)
                .WithAdfsAuthority(Properties.Settings.Default.AuthorityUrl)
                // Perhaps have a list of ports and check if any of them are available on the machine. Make sure the list of ports are also defined on KMD Identity as "approved" redirect uris.
                .WithRedirectUri(Properties.Settings.Default.RedirectUri)
                .Build();

            var authResult = await clientApp
                .AcquireTokenInteractive(Properties.Settings.Default.Scopes.Cast<string>().ToArray())
                // Must be false, due to embedded view not supporting javascript correctly
                .WithUseEmbeddedWebView(false)
                .WithSystemWebViewOptions(new SystemWebViewOptions()
                {
                    HtmlMessageSuccess = "You have successfully logged in. Close this browser and continue in your application.",
                    HtmlMessageError = "An error occurred. Close this browser and try logging in again."
                })
                .ExecuteAsync();

            if (string.IsNullOrWhiteSpace(authResult.AccessToken) || string.IsNullOrWhiteSpace(authResult.AccessToken))
                return;

            UserContext.FromResult(authResult);

            DialogResult = DialogResult.OK;
        }

        private async void bAuthenticateWindows_Click(object sender, EventArgs e)
        {

            //Set to specify how to properly sign out when authenticating this way
            LogoutType = LogoutType.CacheOnly;

            var clientApp = PublicClientApplicationBuilder.Create(Properties.Settings.Default.ClientId)
                .WithAdfsAuthority(Properties.Settings.Default.AuthorityUrl)
                // Perhaps have a list of ports and check if any of them are available on the machine. Make sure the list of ports are also defined on KMD Identity as "approved" redirect uris.
                .WithRedirectUri(Properties.Settings.Default.RedirectUri)
                .Build();

            var authResult = await clientApp
                .AcquireTokenInteractive(Properties.Settings.Default.Scopes.Cast<string>().ToArray())
                // Can be true if IdP will use Windows Token, like i.e. ADFS
                // Can be false, but then user will have to close browser that will pop up
                .WithUseEmbeddedWebView(true)
                .WithExtraQueryParameters(new Dictionary<string, string>() { { "domain_hint", "kmd-ad-prod" } })
                .ExecuteAsync();

            if (string.IsNullOrWhiteSpace(authResult.AccessToken) || string.IsNullOrWhiteSpace(authResult.AccessToken))
                return;

            UserContext.FromResult(authResult);

            DialogResult = DialogResult.OK;
        }

        private async void bAuthenticateWithHint_Click(object sender, EventArgs e)
        {
            //Set to specify how to properly sign out when authenticating this way
            LogoutType = LogoutType.UseBrowser;

            var clientApp = PublicClientApplicationBuilder.Create(Properties.Settings.Default.ClientId)
                .WithAdfsAuthority(Properties.Settings.Default.AuthorityUrl)
                // Perhaps have a list of ports and check if any of them are available on the machine. Make sure the list of ports are also defined on KMD Identity as "approved" redirect uris.
                .WithRedirectUri(Properties.Settings.Default.RedirectUri)
                .Build();

            var authResult = await clientApp
                .AcquireTokenInteractive(Properties.Settings.Default.Scopes.Cast<string>().ToArray())
                // Must be false, due to embedded view not supporting javascript correctly
                .WithUseEmbeddedWebView(false)
                .WithSystemWebViewOptions(new SystemWebViewOptions()
                {
                    HtmlMessageSuccess = "You have successfully logged in. Close this browser and continue in your application.",
                    HtmlMessageError = "An error occurred. Close this browser and try logging in again."
                })
                .WithExtraQueryParameters(new Dictionary<string, string>() { { "domain_hint", "nemlogin-3-test-public" } })
                .ExecuteAsync();

            if (string.IsNullOrWhiteSpace(authResult.AccessToken) || string.IsNullOrWhiteSpace(authResult.AccessToken))
                return;

            UserContext.FromResult(authResult);

            DialogResult = DialogResult.OK;
        }

        private async void bAuthenticateWebView2_Click(object sender, EventArgs e)
        {
            // follow instruction from here: https://docs.microsoft.com/en-us/microsoft-edge/webview2/get-started/winforms and read more here: https://developer.microsoft.com/en-us/microsoft-edge/webview2/
            // you must include the WebView Runtime installer together with your application setup file, to ensure, that user can use WebView2

            //Set to specify how to properly sign out when authenticating this way
            LogoutType = LogoutType.CacheOnly;

            var clientApp = PublicClientApplicationBuilder.Create(Properties.Settings.Default.ClientId)
                .WithAdfsAuthority(Properties.Settings.Default.AuthorityUrl)
                // Desktop Features enable WebView2
                .WithDesktopFeatures()
                // Perhaps have a list of ports and check if any of them are available on the machine. Make sure the list of ports are also defined on KMD Identity as "approved" redirect uris.
                .WithRedirectUri(Properties.Settings.Default.RedirectUri)
                .Build();

            var authResult = await clientApp
                .AcquireTokenInteractive(Properties.Settings.Default.Scopes.Cast<string>().ToArray())
                // This time using WebView 2
                .WithUseEmbeddedWebView(true)
                .ExecuteAsync();

            if (string.IsNullOrWhiteSpace(authResult.AccessToken) || string.IsNullOrWhiteSpace(authResult.AccessToken))
                return;

            UserContext.FromResult(authResult);

            DialogResult = DialogResult.OK;
        }
    }
}
