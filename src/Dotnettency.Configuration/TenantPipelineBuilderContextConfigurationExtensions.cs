using Dotnettency.MiddlewarePipeline;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public static class TenantPipelineBuilderContextConfigurationExtensions
    {
        public static Task<IConfiguration> GetConfiguration<TTenant>(this TenantShellItemBuilderContext<TTenant> context)
            where TTenant : class
        {
            return context.GetShellItemAsync<IConfiguration>();
        }
    }

}
