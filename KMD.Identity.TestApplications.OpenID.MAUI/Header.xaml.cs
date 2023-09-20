using KMD.Identity.TestApplications.OpenID.MAUI.Configuration;
using System;

namespace KMD.Identity.TestApplications.OpenID.MAUI;

public partial class Header : ContentView
{
    public Header()
    {
        InitializeComponent();
    }

    private async void Button_OnClicked(object sender, EventArgs e)
    {
        var app = (App)Application.Current!;
        var idToken = app.AuthViewModel.IdToken;

        await app.Logout();
        await Shell.Current.GoToAsync("//MainPage");

        var configuration = Application.Current!.Handler!.MauiContext!.Services.GetService<AuthConfiguration>();
        var logoutUrl =
            $"{configuration.AuthorityUrl}/oauth2/logout?id_token_hint={idToken}" +
            $"&client_id={configuration.ApplicationId}" +
            $"&post_logout_redirect_uri={configuration.PostLogoutRedirectUrl}";
        await Browser.Default.OpenAsync(logoutUrl, BrowserLaunchMode.SystemPreferred);
    }
}