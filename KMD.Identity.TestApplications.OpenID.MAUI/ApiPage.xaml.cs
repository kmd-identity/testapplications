using System.Net.Http.Headers;
using KMD.Identity.TestApplications.OpenID.MAUI.Configuration;
using KMD.Identity.TestApplications.OpenID.MAUI.ViewModels;
using Newtonsoft.Json;

namespace KMD.Identity.TestApplications.OpenID.MAUI;

public partial class ApiPage : ContentPage
{
    private readonly ApiConfiguration configuration;
    private ApiPageViewModel viewModel = new ApiPageViewModel();

    public ApiPage(ApiConfiguration configuration)
    {
        this.configuration = configuration;
        InitializeComponent();
    }

    private async void ApiCallBtn_OnClicked(object sender, EventArgs e)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", ((App)Application.Current).AuthViewModel.AccessToken);

        var result = await httpClient.GetAsync(configuration.ApiUrl);
        result.EnsureSuccessStatusCode();

        var content = await result.Content.ReadAsStringAsync();

        viewModel.ApiResult = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<dynamic>(content), Formatting.Indented);
    }

    private void ApiPage_OnNavigatedTo(object sender, NavigatedToEventArgs e)
    {
        viewModel = new ApiPageViewModel();
        BindingContext = viewModel;
    }
}