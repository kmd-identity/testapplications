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
            MainPage = new AppShell();
        }

        public async Task Login()
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
                    result = await identityClient
                        .AcquireTokenInteractive(settings.Scopes)
                        .ExecuteAsync();
                }
                catch (Exception ex)
                {
                    await MainPage!.DisplayAlert("Error", $"\"MSAL Interactive Error: {ex.Message}", "Close");
                }
            }

            AuthViewModel.AfterAuthentication(result);
        }

        public async Task Logout()
        {
            var accounts = await identityClient.GetAccountsAsync();
            foreach (var account in accounts)
            {
                await identityClient.RemoveAsync(account);
            }

            AuthViewModel.AfterLogout();
        }
    }
}