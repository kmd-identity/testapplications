using KMD.Identity.TestApplications.OpenID.MAUI.ViewModels;
using Microsoft.Identity.Client;
using System.Diagnostics;

namespace KMD.Identity.TestApplications.OpenID.MAUI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {   
            InitializeComponent();
            BindingContext = App.ViewModel;
        }

        private async void OnLoginStart(object sender, EventArgs e)
        {
            var result = await App.GetAuthenticationToken();
            App.ViewModel.AfterAuthentication(result);
        }
    }
}