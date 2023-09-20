using KMD.Identity.TestApplications.OpenID.MAUI.ViewModels;
using Microsoft.Identity.Client;
using System.Diagnostics;

namespace KMD.Identity.TestApplications.OpenID.MAUI
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel viewModel = new MainPageViewModel();

        public MainPage()
        {   
            InitializeComponent();
        }

        private async void OnLoginStart(object sender, EventArgs e)
        {
            var parentApp = (App)Application.Current!;
            await parentApp.Login(viewModel.DomainHint);
        }

        private void MainPage_OnLoaded(object sender, EventArgs e)
        {
            viewModel = new MainPageViewModel();
            BindingContext = viewModel;
        }
    }
}