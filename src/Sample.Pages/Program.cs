using Dotnettency;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

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
            var builder = new DotnettencyServiceProviderFactory<Tenant>();

            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(builder)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    //.UseKestrel(kestrelOptions =>
                    //{
                    //    //kestrelOptions.Listen()
                    //    kestrelOptions.Listen(IPAddress.IPv6Any, 5000, (listenOptions) =>
                    //    {
                    //        var loggerFactory = listenOptions.KestrelServerOptions.ApplicationServices.GetRequiredService<ILoggerFactory>();
                    //        var logger = loggerFactory.CreateLogger<BadRequestDiagnosticAdapter>();

                    //        listenOptions.ConnectionAdapters.Add(new BadRequestDiagnosticAdapter(logger, bufferSize: 16384));

                    //    });
                    //});
                });

        }
    }

}

