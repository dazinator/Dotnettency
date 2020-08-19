using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Dotnettency
{
    public static class TenantShellItemBuilderContextConfigurationExtensions
    {
        public static async Task<IConfiguration> GetConfigurationAsync<TTenant>(this TenantShellItemBuilderContext<TTenant> context)
            where TTenant : class
        {
            var task = await context.GetShellItemAsync<Task<IConfiguration>>();
            return await task;
        }
    }

}
