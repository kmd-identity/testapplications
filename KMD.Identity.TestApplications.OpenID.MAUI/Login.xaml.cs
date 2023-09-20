using AndroidX.Lifecycle;
using KMD.Identity.TestApplications.OpenID.MAUI.ViewModels;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace KMD.Identity.TestApplications.OpenID.MAUI;

public partial class Login : ContentPage
{
    private LoginViewModel viewModel = new LoginViewModel();

    public Login()
    {
        InitializeComponent();
    }

    private async void OnLoginStart(object sender, EventArgs e)
    {
        var parentApp = (App)Application.Current!;
        await parentApp.Login(viewModel.DomainHint);
    }

    private async void Login_OnLoaded(object sender, EventArgs e)
    {
        viewModel = new LoginViewModel();
        BindingContext = viewModel;

        var parentApp = (App)Application.Current!;
        var currentToken = await parentApp.CheckActiveRefreshToken();

        if (currentToken?.ClaimsPrincipal?.Claims?.Any() == true)
        {
            var isAvailable = await CrossFingerprint.Current.IsAvailableAsync(true);
            if (isAvailable)
            {
                var request = new AuthenticationRequestConfiguration("Login using biometrics", "Confirm login with your biometrics");
                request.AllowAlternativeAuthentication = false;
                request.CancelTitle = "Cancel";
                var result = await CrossFingerprint.Current.AuthenticateAsync(request);
                if (result.Authenticated)
                {
                    await parentApp.ContinueWithValidToken(currentToken);
                }
            }
        }
    }
}