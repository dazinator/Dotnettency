using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using StructureMap.AspNetCore;

namespace Sample.RazorPages
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://*:5000", "http://*:5001", "http://*:5002", "http://*:5003", "http://*:5004")
                .UseStructureMap()
                .UseStartup<Startup>()
                .Build();
    }
}
