using Microsoft.Identity.Client;
using KMD.Identity.TestApplications.OpenID.MAUI.ViewModels;
using KMD.Identity.TestApplications.OpenID.MAUI.Configuration;

namespace KMD.Identity.TestApplications.OpenID.MAUI
{
    public partial class App : Application
    {
        private readonly AuthConfiguration settings;
        private IPublicClientApplication identityClient;
        public readonly AuthViewModel AuthViewModel = new AuthViewModel();

        public App(AuthConfiguration settings)
        {
            this.settings = settings;
            InitializeComponent();
            BindingContext = AuthViewModel;
            MainPage = new Login();
        }

        public async Task<AuthenticationResult> ReuseActiveRefreshToken()
        {
            EnsureIdentityClientInitialized();
            
            var accounts = await identityClient.GetAccountsAsync();
            AuthenticationResult result = null;

            try
            {
                result = await identityClient
                    .AcquireTokenSilent(settings.Scopes, accounts.FirstOrDefault())
                    .ExecuteAsync();

                AuthViewModel.AfterAuthentication(result);
            }
            catch (MsalUiRequiredException)
            {
                return null;
            }
            catch (Exception ex)
            {
                await MainPage!.DisplayAlert("Error", $"MSAL Silent Error: {ex.Message}", "Close");
            }

            return result;
        }

        public async Task ContinueWithValidToken(AuthenticationResult result)
        {
            AuthViewModel.AfterAuthentication(result);
            if (AuthViewModel.IsAuthenticated)
            {
                MainPage = new AppShell();
            }
        }

        public async Task Login(string domainHint)
        {
            EnsureIdentityClientInitialized();
            
            AuthenticationResult result = null;
            
            try
            {
                var context = identityClient.AcquireTokenInteractive(settings.Scopes);
                if (!string.IsNullOrWhiteSpace(domainHint))
                    context = context.WithExtraQueryParameters(new Dictionary<string, string>() { { "domain_hint", domainHint } });

                result = await context.WithPrompt(Prompt.ForceLogin).ExecuteAsync();
            }
            catch (Exception ex)
            {
                await MainPage!.DisplayAlert("Error", $"\"MSAL Interactive Error: {ex.Message}", "Close");
            }

            AuthViewModel.AfterAuthentication(result);
            if (AuthViewModel.IsAuthenticated)
            {
                MainPage = new AppShell();
            }
        }

        public async Task Logout()
        {
            var idToken = AuthViewModel.IdToken;

            var accounts = await identityClient.GetAccountsAsync();
            foreach (var account in accounts)
            {
                await identityClient.RemoveAsync(account);
            }

            AuthViewModel.AfterLogout();
            MainPage = new Login();
            
            var logoutUrl =
                $"{settings.AuthorityUrl}/oauth2/logout?id_token_hint={idToken}" +
                $"&client_id={settings.ApplicationId}" +
                $"&post_logout_redirect_uri={settings.PostLogoutRedirectUrl}";
            await Browser.Default.OpenAsync(logoutUrl, BrowserLaunchMode.SystemPreferred);
        }

        private void EnsureIdentityClientInitialized()
        {

            if (identityClient == null)
            {
#if ANDROID
                identityClient = PublicClientApplicationBuilder
                    .Create(settings.ApplicationId)
                    .WithAdfsAuthority(settings.AuthorityUrl)
                    .WithRedirectUri(settings.RedirectUrl)
                    .WithParentActivityOrWindow(() => Platform.CurrentActivity)
                    .Build();
#elif IOS
        identityClient = PublicClientApplicationBuilder
            .Create(settings.ApplicationId)
            .WithAdfsAuthority(settings.AuthorityUrl)
            .WithRedirectUri(settings.RedirectUrl)
            .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
            .Build();
#else
                identityClient = PublicClientApplicationBuilder
                    .Create(settings.ApplicationId)
                    .WithAdfsAuthority(settings.AuthorityUrl)
                    .WithRedirectUri(settings.RedirectUrl)
                    .Build();
#endif
            }
        }
    }
}