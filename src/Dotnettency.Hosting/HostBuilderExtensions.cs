using Dotnettency;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.Hosting
{
    public static class HostBuilderExtensions
    {

        public static IHostBuilder UseDotnettencyServiceProviderFactory<TTenant>(this IHostBuilder builder)
             where TTenant : class
        {
            var providerFactory = new DotnettencyServiceProviderFactory<TTenant>();
            return builder.UseServiceProviderFactory(providerFactory);
        }
    }
}
