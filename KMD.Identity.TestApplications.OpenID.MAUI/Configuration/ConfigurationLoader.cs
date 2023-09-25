using System.Reflection;
using System.Text.Json;

namespace KMD.Identity.TestApplications.OpenID.MAUI.Configuration
{
    public static class ConfigurationLoader
    {
        public static T Load<T>(string fileName)
        {
            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"{typeof(ConfigurationLoader).Namespace}.{fileName}")!;
            stream.Position = 0;
            return JsonSerializer.Deserialize<T>(stream);
        }
    }
}
