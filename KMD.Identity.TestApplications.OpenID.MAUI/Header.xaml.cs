namespace KMD.Identity.TestApplications.OpenID.MAUI;

public partial class Header : ContentView
{
    public Header()
    {
        InitializeComponent();
    }

    private async void Button_OnClicked(object sender, EventArgs e)
    {
        await ((App)Application.Current!).Logout();
    }
}