using Dotnettency.Configuration;
using Dotnettency.Container;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Dotnettency
{
    public static class TenantContainerBuilderContextConfigurationExtensions
    {
        public static async Task<IConfiguration> GetConfiguration<TTenant>(this TenantContainerBuilderContext<TTenant> context)
            where TTenant : class
        {
            var sp = context.ApplicationServices;
            var accessor = sp.GetRequiredService<ITenantConfigurationAccessor<TTenant, IConfiguration>>();
            var accessDelegate = accessor.ConfigFactory(sp);
            var config = await accessDelegate.Value;
            return config;
        }
    }

}
