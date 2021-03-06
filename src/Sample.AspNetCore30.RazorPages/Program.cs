using Dotnettency;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Sample.Pages
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseDotnettencyServiceProviderFactory<Tenant>()               
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
                
        }
    }

}

