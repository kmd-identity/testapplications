using Microsoft.Identity.Client;
using System.Diagnostics;
using KMD.Identity.TestApplications.OpenID.MAUI.ViewModels;

namespace KMD.Identity.TestApplications.OpenID.MAUI
{
    public partial class App : Application
    {
        public static IPublicClientApplication IdentityClient { get; set; }

        public static MainPageViewModel ViewModel { get; set; } = new MainPageViewModel();

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        private class Constants
        {
            public static string ApplicationId { get; set; } = "6c133e1c-e215-4780-bea8-2b62678581f4";
            public static string AuthorityUrl { get; set; } = "https://identity.kmd.dk/adfs";
            public static string RedirectUrl { get; set; } = "kmdidentity://postlogin";
            public static string[] Scopes { get; set; } = new[] { "openid" };
        }

        public static async Task<AuthenticationResult> GetAuthenticationToken()
        {
            if (IdentityClient == null)
            {
#if ANDROID
        IdentityClient = PublicClientApplicationBuilder
            .Create(Constants.ApplicationId)
            .WithAdfsAuthority(Constants.AuthorityUrl)
            .WithRedirectUri(Constants.RedirectUrl)
            .WithParentActivityOrWindow(() => Platform.CurrentActivity)
            .Build();
#elif IOS
        IdentityClient = PublicClientApplicationBuilder
            .Create(Constants.ApplicationId)
            .WithAdfsAuthority(Constants.AuthorityUrl)
            .WithRedirectUri(Constants.RedirectUrl)
            .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
            .Build();
#else
                IdentityClient = PublicClientApplicationBuilder
                    .Create(Constants.ApplicationId)
                    .WithAdfsAuthority(Constants.AuthorityUrl)
                    .WithRedirectUri(Constants.RedirectUrl)
                    .Build();
#endif
            }

            var accounts = await IdentityClient.GetAccountsAsync();
            AuthenticationResult result = null;
            bool tryInteractiveLogin = false;

            try
            {
                result = await IdentityClient
                    .AcquireTokenSilent(Constants.Scopes, accounts.FirstOrDefault())
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                tryInteractiveLogin = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MSAL Silent Error: {ex.Message}");
            }

            if (tryInteractiveLogin)
            {
                try
                {
                    result = await IdentityClient
                        .AcquireTokenInteractive(Constants.Scopes)
                        .ExecuteAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"MSAL Interactive Error: {ex.Message}");
                }
            }

            return result;
        }
    }
}