using System;
using Microsoft.Owin.Hosting;

namespace Sample.Owin.SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {

            // Tenants all have different urls.
            var startOptions = new StartOptions("http://localhost:5000");
            startOptions.Urls.Add("http://localhost:5001");
            startOptions.Urls.Add("http://localhost:5002");
            startOptions.Urls.Add("http://localhost:5003");
            startOptions.Urls.Add("http://localhost:5004");


            using (WebApp.Start<Startup>(startOptions))
            {
                Console.WriteLine("Press [enter] to quit...");
                Console.ReadLine();
            }
        }
    }
}
