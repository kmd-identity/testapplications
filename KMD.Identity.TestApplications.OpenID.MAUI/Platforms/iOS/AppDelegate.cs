using Foundation;
using KMD.Identity.TestApplications.OpenID.MAUI.Configuration;
using Microsoft.Identity.Client;
using UIKit;

namespace KMD.Identity.TestApplications.OpenID.MAUI
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            var authConfig = Services.GetService<AuthConfiguration>();

            if (url.AbsoluteString?.IndexOf(authConfig.PostLogoutRedirectUrl, StringComparison.InvariantCultureIgnoreCase) >= 0) return true;

            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
            return true;
        }

        public void HandleLogout(string url)
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
        }
    }
}