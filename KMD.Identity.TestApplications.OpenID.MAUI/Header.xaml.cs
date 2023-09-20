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
        await app.Logout();
        await Shell.Current.GoToAsync("//MainPage");
    }
}