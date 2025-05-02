using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace KMD.Identity.TestApplications.OpenID.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureWebHost(host =>
                {
                    host.ConfigureKestrel(options =>
                    {
                        options.AddServerHeader = false;
                    });
                });
    }
}
