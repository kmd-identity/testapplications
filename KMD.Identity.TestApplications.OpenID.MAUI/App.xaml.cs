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

        public async Task<AuthenticationResult> CheckActiveRefreshToken()
        {
            EnsureIdentityClientInitialized();
            
            var accounts = await identityClient.GetAccountsAsync();
            AuthenticationResult result = null;

            try
            {
                result = await identityClient
                    .AcquireTokenSilent(settings.Scopes, accounts.FirstOrDefault())
                    .ExecuteAsync();
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

            var accounts = await identityClient.GetAccountsAsync();
            AuthenticationResult result = null;
            bool tryInteractiveLogin = false;

            try
            {
                result = await identityClient
                    .AcquireTokenSilent(settings.Scopes, accounts.FirstOrDefault())
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                tryInteractiveLogin = true;
            }
            catch (Exception ex)
            {
                await MainPage!.DisplayAlert("Error", $"MSAL Silent Error: {ex.Message}", "Close");
            }

            if (tryInteractiveLogin)
            {
                try
                {
                    var context = identityClient.AcquireTokenInteractive(settings.Scopes);
                    if (!string.IsNullOrWhiteSpace(domainHint))
                        context = context.WithExtraQueryParameters(new Dictionary<string, string>() { { "domain_hint", domainHint } });

                    result = await context.ExecuteAsync();
                }
                catch (Exception ex)
                {
                    await MainPage!.DisplayAlert("Error", $"\"MSAL Interactive Error: {ex.Message}", "Close");
                }
            }

            AuthViewModel.AfterAuthentication(result);
            if (AuthViewModel.IsAuthenticated)
            {
                MainPage = new AppShell();
            }
        }

        public async Task Logout()
        {
            var accounts = await identityClient.GetAccountsAsync();
            foreach (var account in accounts)
            {
                await identityClient.RemoveAsync(account);
            }

            AuthViewModel.AfterLogout();
            MainPage = new Login();
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