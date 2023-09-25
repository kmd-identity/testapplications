using KMD.Identity.TestApplications.OpenID.MAUI.ViewModels;
using Microsoft.Identity.Client;
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

    public async Task ShowBiometricLogin()
    {
        var request = new AuthenticationRequestConfiguration("Login using biometrics", "Confirm login with your biometrics");
        request.AllowAlternativeAuthentication = false;
        var result = await CrossFingerprint.Current.AuthenticateAsync(request);
        if (result.Authenticated)
        {
            await ContinueWithValidToken();
        }
    }

    public async Task ContinueWithValidToken()
    {
        var parentApp = (App)Application.Current!;
        await parentApp.ContinueWithValidToken(viewModel.AuthenticationResult);
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
        viewModel.AuthenticationResult = await parentApp.ReuseActiveRefreshToken();
        viewModel.HasValidToken = parentApp.AuthViewModel.IsAuthenticated;
#if ANDROID
       viewModel.HasBiometric = await CrossFingerprint.Current.IsAvailableAsync(false);
#elif IOS
        //INFO - Plugin.Fingerprint may work for ios with some specific configuration
        // https://github.com/smstuebe/xamarin-fingerprint
        viewModel.HasBiometric = false;
#else
        viewModel.HasBiometric = false;
#endif


        if (viewModel.HasValidToken)
        {
            if (viewModel.HasBiometric)
            {
                await ShowBiometricLogin();
            }
            else
            {
                await ContinueWithValidToken();
            }
        }
    }

    private async void BiometricButton_OnClicked(object sender, EventArgs e)
    {
        await ShowBiometricLogin();
    }

    private async void LogoutButton_OnClicked(object sender, EventArgs e)
    {
        await ((App)Application.Current!).Logout();
    }
}