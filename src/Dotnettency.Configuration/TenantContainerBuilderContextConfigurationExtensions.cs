using Dotnettency.Container;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Dotnettency
{
    public static class TenantContainerBuilderContextConfigurationExtensions
    {
        public static Task<IConfiguration> GetConfigurationAsync<TTenant>(this TenantContainerBuilderContext<TTenant> context)
            where TTenant : class
        {
            return context.GetShellItemAsync<TTenant, IConfiguration>();            
        }
    }

}
