using Dotnettency.Container;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Dotnettency
{
    public static class TenantShellItemBuilderContextConfigurationExtensions
    {
        public static Task<IConfiguration> GetConfigurationAsync<TTenant>(this TenantShellItemBuilderContext<TTenant> context)
            where TTenant : class
        {
            return context.GetShellItemAsync<IConfiguration>();            
        }
    }

}
