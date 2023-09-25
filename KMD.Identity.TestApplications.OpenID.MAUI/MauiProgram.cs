using KMD.Identity.TestApplications.OpenID.MAUI.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Identity.Client;

namespace KMD.Identity.TestApplications.OpenID.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureLifecycleEvents(events =>
                {
#if ANDROID
            events.AddAndroid(platform =>
            {
                platform.OnActivityResult((activity, rc, result, data) =>
                {
                    AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(rc, result, data);
                });
            });
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton(ConfigurationLoader.Load<AuthConfiguration>("AuthConfiguration.json"));
            builder.Services.AddSingleton(ConfigurationLoader.Load<ApiConfiguration>("ApiConfiguration.json"));
            builder.Services.AddSingleton<ApiPage>();

            return builder.Build();
        }
    }
}