using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Windows.Forms;
using Newtonsoft.Json;
using HttpClient = System.Net.Http.HttpClient;

namespace KMD.Identity.TestApplications.OpenID.WinForms
{
    public partial class MainForm : Form
    {
        private readonly LogoutType logoutType;
        public MainForm(LogoutType logoutType)
        {
            InitializeComponent();
            this.logoutType = logoutType;
        }

        private void bIdToken_Click(object sender, EventArgs e)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(UserContext.Current.AuthenticatedUser.IdToken);

            tResult.Text = JsonConvert.SerializeObject(jsonToken.Payload, Formatting.Indented);
        }

        private void bAccessToken_Click(object sender, EventArgs e)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(UserContext.Current.AuthenticatedUser.AccessToken);

            tResult.Text = JsonConvert.SerializeObject(jsonToken.Payload, Formatting.Indented);
        }

        private async void bCallApi_Click(object sender, EventArgs e)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", UserContext.Current.AuthenticatedUser.AccessToken);

            var result = await httpClient.GetAsync(Properties.Settings.Default.ApiUrl);
            result.EnsureSuccessStatusCode();

            var content = await result.Content.ReadAsStringAsync();

            tResult.Text =
                JsonConvert.SerializeObject(JsonConvert.DeserializeObject<dynamic>(content), Formatting.Indented);
        }

        private void logout_Click(object sender, EventArgs e)
        {
            //Some login types use browser, these also need you to sign out in browser
            if (logoutType == LogoutType.UseBrowser)
            {
                var logoutUrl =
                    $"https://identity.kmd.dk/adfs/oauth2/logout?id_token_hint={UserContext.Current.AuthenticatedUser.IdToken}&" +
                    $"client_id={Properties.Settings.Default.ClientId}";

                System.Diagnostics.Process.Start(logoutUrl);
            }
            UserContext.RemoveUser();
            Close();
        }
    }
}
